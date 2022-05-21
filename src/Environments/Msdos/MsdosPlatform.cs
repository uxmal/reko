#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Reko.Environments.Msdos
{
    /// <summary>
    /// Platform services for the MS-DOS real-mode operating environment.
    /// </summary>
    [Designer("Reko.Environments.Msdos.Design.MsdosPlatformDesigner,Reko.Environments.Msdos.Design")]
    public partial class MsdosPlatform : Platform
	{
		private SystemService [] realModeServices;
        private readonly HashSet<RegisterStorage> implicitRegs;

        public MsdosPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "ms-dos")
		{
            this.realModeServices = null!;
            implicitRegs = new HashSet<RegisterStorage>
            {
                Registers.cs,
                Registers.ss,
                Registers.sp,
                Registers.esp,
                Registers.Top,
            };
            this.StructureMemberAlignment = 1;
        }

        public override CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.MsvcKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return implicitRegs.Contains(reg);
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            return new MsdosEmulator();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            // On MS-DOS, C and Pascal compilers
            // typically saved bp, si, and di.
            return new HashSet<RegisterStorage>
            {
                Registers.ax,
                Registers.cx,
                Registers.dx,
                Registers.bx,
                Registers.sp,
                Registers.Top,
            };
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            ccName = ccName?.TrimStart('_') ?? string.Empty; // Default to cdecl (same as empty string)

            switch (ccName)
            {
            case "":
            case "cdecl":
                return new X86CallingConvention(
                    2,
                    4,      //$REVIEW: this is a far ptr.
                    true,
                    false);
            case "stdcall":
                return new X86CallingConvention(
                    2,
                    4,      //$REVIEW: this is a far ptr.
                    false,
                    false);
            case "pascal":
                return new X86CallingConvention(
                    2,
                    4,      //$REVIEW: this is a far ptr.
                    false,
                    true);
            }
            throw new ArgumentOutOfRangeException(string.Format("Unknown calling convention '{0}'.", ccName));
        }

        public override string? DetermineCallingConvention(FunctionType signature)
        {
            if (!signature.HasVoidReturn)
            {
                if (signature.ReturnValue.Storage is RegisterStorage reg)
                {
                    if (reg != Registers.al && reg != Registers.ax)
                        return null;
                }
                if (signature.ReturnValue.Storage is SequenceStorage seq && seq.Elements.Length == 2)
                {
                    if (seq.Elements[0] != Registers.dx || seq.Elements[1] != Registers.ax)
                        return null;
                }
            }
            if (signature.Parameters!.Any(p => p.Storage is not StackArgumentStorage))
                return null;
            if (signature.FpuStackDelta != 0 || signature.FpuStackArgumentMax >= 0)
                return null;
            if (signature.StackDelta == 0)
                return "__cdecl";
            else
                return "__pascal";
        }

        public override void EnsureTypeLibraries(string envName)
        {
            base.EnsureTypeLibraries(envName);
            LoadRealmodeServices(Architecture);
        }

        public override ImageSymbol? FindMainProcedure(Program program, Address addrStart)
        {
            var sf = new StartupFinder(Services, program, addrStart);
            return sf.FindMainAddress();
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
		{
            EnsureTypeLibraries(PlatformIdentifier);
			foreach (SystemService svc in realModeServices)
			{
				if (svc.SyscallInfo!.Matches(vector, state))
					return svc;
			}
			return null;
		}

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 16;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter m)
        {
            m.Assign(proc.Frame.EnsureRegister(Registers.Top), 0);
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public void LoadRealmodeServices(IProcessorArchitecture arch)
        {
            var prefix = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var libPath = Path.Combine(prefix, "realmodeintservices.xml");
            if (!File.Exists(libPath))
            {
                libPath = Path.Combine(Directory.GetCurrentDirectory(), "realmodeintservices.xml");
            }

            SerializedLibrary lib;
            var fsSvc = Services.RequireService<IFileSystemService>();
            using (Stream stm = fsSvc.CreateFileStream(libPath, FileMode.Open, FileAccess.Read))
            {
                lib = SerializedLibrary.LoadFromStream(stm);
            }

            realModeServices = lib.Procedures
                .Cast<SerializedService>()
                .Select(s => s.Build(this, Metadata))
                .ToArray();
        }

        public override Address? MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return null;
        }

        public override string DefaultCallingConvention
        {
            get { return "cdecl"; }
        }
	}
}

#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Environments.SysV.ArchSpecific;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Environments.SysV
{
    //$TODO: rename to Elf-Neutral? Or Posix?
    public class SysVPlatform : Platform
    {
        private RegisterStorage[] trashedRegs;
        private readonly ArchSpecificFactory archSpecificFactory;

        public SysVPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "elf-neutral")
        {
            this.trashedRegs = LoadTrashedRegisters();
            archSpecificFactory = new ArchSpecificFactory(services, arch);
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            var Architecture = this.Architecture;
            return archSpecificFactory.CreateCallingConverion(Architecture);
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>(this.trashedRegs);
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> rw, IRewriterHost host)
        {
            var finder = archSpecificFactory.CreateTrampolineDestinationFinder(this.Architecture);
            var target = finder(Architecture, addrInstr, rw, host);
            if (target is ProcedureConstant pc)
            {
                return pc.Procedure;
            }
            else if (target is Address addrTarget)
            {
                var arch = this.Architecture;
                ProcedureBase? proc = host.GetImportedProcedure(arch, addrTarget, addrInstr);
                if (proc != null)
                    return proc;
                return host.GetInterceptedCall(arch, addrTarget);
            }
            else
                return null;
        }

        public override void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter m)
        {
            switch (Architecture.Name)
            {
            case "mips-be-32":
            case "mips-le-32":
                // MIPS ELF ABI: r25 is _always_ set to the address of a procedure on entry.
                m.Assign(proc.Frame.EnsureRegister(Architecture.GetRegister("r25")!), Constant.Word32((uint)addr.ToLinear()));
                break;
            case "mips-be-64":
            case "mips-le-64":
                // MIPS ELF ABI: r25 is _always_ set to the address of a procedure on entry.
                m.Assign(proc.Frame.EnsureRegister(Architecture.GetRegister("r25")!), Constant.Word64((uint) addr.ToLinear()));
                break;
            case "x86-protected-32":
            case "x86-protected-64":
                m.Assign(
                    proc.Frame.EnsureRegister(Architecture.FpuStackRegister),
                    0);
                break;
            case "zSeries":
                // Stack parameters are passed in starting at offset +160 from the 
                // stack; everything at lower addresses is local to the called procedure's
                // frame.
                m.Assign(
                    proc.Frame.EnsureRegister(Architecture.GetRegister("r15")!),
                    m.ISub(
                        proc.Frame.FramePointer,
                        Constant.Int(proc.Frame.FramePointer.DataType, 160)));
                break;
            }
        }

        private RegisterStorage[] LoadTrashedRegisters()
        {
            if (Services != null)
            {
                var cfgSvc = Services.RequireService<IConfigurationService>();
                var pa = cfgSvc.GetEnvironment(this.PlatformIdentifier).Architectures.SingleOrDefault(a => a.Name == Architecture.Name);
                if (pa != null)
                {
                    return pa.TrashedRegisters
                        .Select(r => Architecture.GetRegister(r)!)
                        .Where(r => r != null)
                        .ToArray();
                }
            }
            return new RegisterStorage[0];
        } 

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            //$REVIEW: looks a lot like Win32library, perhaps push to parent class?
            EnsureTypeLibraries(PlatformIdentifier);
            var sig = Metadata.Lookup(procName);
            if (sig == null)
                return null;
            var proc = new ExternalProcedure(procName, sig);
            var characteristics = CharacteristicsLibs.Select(cl => cl.Lookup(procName))
                .Where(c => c != null)
                .FirstOrDefault();
            if (characteristics != null)
                proc.Characteristics = characteristics;
            return proc;
        }

        public override ProcedureBase_v1? SignatureFromName(string fnName)
        {
            StructField_v1? field;
            try
            {
                var gcc = new GccMangledNameParser(fnName, this.PointerType.Size);
                field = gcc.Parse();
            }
            catch (Exception ex)
            {
                Debug.Print("*** Error parsing {0}. {1}", fnName, ex.Message);
                return null;
            }
            if (field != null && field.Type is SerializedSignature sproc)
            {
                return new Procedure_v1
                {
                    Name = field.Name,
                    Signature = sproc,
                };
            }
            return null;
        }
    }
}

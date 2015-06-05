#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Configuration;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace Decompiler.Environments.Win32
{
	public class Win32Platform : Platform
	{
		private SystemService int3svc;
        private SystemService int29svc;

        //$TODO: http://www.delorie.com/djgpp/doc/rbinter/ix/29.html int 29 for console apps!
        //$TODO: http://msdn.microsoft.com/en-us/data/dn774154(v=vs.99).aspx

		public Win32Platform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch)
		{
            int3svc = new SystemService
            {
                SyscallInfo = new SyscallInfo
                {
                    Vector = 3,
                    RegisterValues = new RegValue[0],
                },
                Name = "int3",
                Signature = new ProcedureSignature(null, new Identifier[0]),
                Characteristics = new ProcedureCharacteristics(),
            };
            var frame = arch.CreateFrame();
            int29svc = new SystemService
            {
                SyscallInfo = new SyscallInfo
                {
                    Vector = 0x29,
                    RegisterValues = new RegValue[0]
                },
                Name = "__fastfail",
                Signature = new ProcedureSignature(
                    null,
                    frame.EnsureRegister(Registers.ecx)), //$bug what about win64?
                Characteristics = new ProcedureCharacteristics
                {
                    Terminates = true
                }
            };
        }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            var bitset = Architecture.CreateRegisterBitset();
            Registers.cs.SetAliases(bitset, true);
            Registers.ss.SetAliases(bitset, true);
            Registers.sp.SetAliases(bitset, true);
            Registers.esp.SetAliases(bitset, true);
            Registers.fs.SetAliases(bitset, true);
            Registers.gs.SetAliases(bitset, true);
            return bitset;
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader rdr, IRewriterHost host)
        {
            var rw = Architecture.CreateRewriter(
                rdr,
                Architecture.CreateProcessorState(),
                Architecture.CreateFrame(), host);
            var rtlc = rw.FirstOrDefault();
            if (rtlc == null || rtlc.Instructions.Count == 0)
                return null;
            var jump = rtlc.Instructions[0] as RtlGoto;
            if (jump == null)
                return null;
            var pc = jump.Target as ProcedureConstant;
            if (pc != null)
                return pc.Procedure;
            var access = jump.Target as MemoryAccess;
            if (access == null)
                return null;
            var addrTarget = access.EffectiveAddress as Address;
            if (addrTarget == null)
            {
                var wAddr = access.EffectiveAddress as Constant;
                if (wAddr == null)
                {
                    return null;
                }
                addrTarget = MakeAddressFromConstant(wAddr);
            }
            ProcedureBase proc = host.GetImportedProcedure(addrTarget, rtlc.Address);
            if (proc != null)
                return proc;
            return host.GetInterceptedCall(addrTarget);
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            //$REVIEW: common code with Win32_64 platform, consider pushing to base class?
            EnsureTypeLibraries("win32");
            return TypeLibs.Select(t => t.Lookup(procName))
                .Where(sig => sig != null)
                .Select(s => new ExternalProcedure(procName, s))
                .FirstOrDefault();
        }

        public override ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            EnsureTypeLibraries("win32");
            return TypeLibs
                .Where(t => string.Compare(t.ModuleName, moduleName, true) == 0)
                .Select(t =>
                {
                    SystemService svc;
                    if (t.ServicesByVector.TryGetValue(ordinal, out svc))
                    {
                        return new ExternalProcedure(svc.Name, svc.Signature);
                    }
                    else
                        return null;
                })
                .Where(p => p != null)
                .FirstOrDefault();
        }

		public override SystemService FindService(int vector, ProcessorState state)
		{
			if (int3svc.SyscallInfo.Matches(vector, state))
				return int3svc;
            if (int29svc.SyscallInfo.Matches(vector, state))
                return int29svc;
			throw new NotImplementedException("INT services are not supported by " + this.GetType().Name);
		}

        public override string DefaultCallingConvention
        {
            get { return "stdapi"; }
        }

        public override ProcedureSignature SignatureFromName(string fnName)
        {
            return SignatureGuesser.SignatureFromName(
                fnName, 
                new TypeLibraryLoader(Architecture, true),
                Architecture);
        }
	}
}

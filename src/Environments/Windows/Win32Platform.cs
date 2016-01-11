#region License
/* 
 * Copyright (C) 1999-2016 John K�ll�n.
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
using Reko.Core.CLanguage;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.Windows
{
	public class Win32Platform : Platform
	{
		private SystemService int3svc;
        private SystemService int29svc;

        //$TODO: http://www.delorie.com/djgpp/doc/rbinter/ix/29.html int 29 for console apps!
        //$TODO: http://msdn.microsoft.com/en-us/data/dn774154(v=vs.99).aspx

            //$BUG: we need a Win32Base platform, possibly with a Windows base platform, and make this
            // x86-specific.
		public Win32Platform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch)
		{
            //$REVIEW: should be loaded from configuration file.
            Heuristics.ProcedurePrologs = new BytePattern[] {
                new BytePattern
                {
                    Bytes = new byte[]{ 0x55, 0x8B, 0xEC },
                    Mask =  new byte[]{ 0xFF, 0xFF, 0xFF }
                }
            };
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

        public override string PlatformIdentifier { get { return "win32"; } }

        /// <summary>
        /// Some Win32 platforms (I'm looking at you ARM Thumb) will use addresses
        /// that are offset by 1. 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>Adjusted address</returns>
        public virtual Address AdjustProcedureAddress(Address addr)
        {
            return addr;
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            var bitset = new HashSet<RegisterStorage>()
            {
                 Registers.cs,
                 Registers.ss,
                 Registers.sp,
                 Registers.esp,
                 Registers.fs,
                 Registers.gs,
            };
            return bitset;
        }

        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            return new X86ProcedureSerializer((IntelArchitecture) Architecture, typeLoader, defaultConvention);
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Char: return 1;
            case CBasicType.Short: return 2;
            case CBasicType.Int: return 4;
            case CBasicType.Long: return 4;
            case CBasicType.LongLong: return 8;
            case CBasicType.Float: return 4;
            case CBasicType.Double: return 8;
            case CBasicType.LongDouble: return 8;
            case CBasicType.Int64: return 8;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
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
            EnsureTypeLibraries(PlatformIdentifier);
            return TypeLibs.Select(t => t.Lookup(procName))
                .Where(sig => sig != null)
                .Select(s => new ExternalProcedure(procName, s))
                .FirstOrDefault();
        }

        public override ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            EnsureTypeLibraries(PlatformIdentifier);
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
                new TypeLibraryLoader(this, true),
                this);
        }
	}
}

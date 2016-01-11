#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    public class Win_x86_64_Platform : Platform
    {
        private SystemService int3svc;

        public Win_x86_64_Platform(IServiceProvider sp, IProcessorArchitecture arch)
            : base(sp, arch, "win64")
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

        }

        public override string DefaultCallingConvention
        {
            get { throw new NotImplementedException(); }
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                Registers.rsp,
            };
        }


        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            return new X86ProcedureSerializer((IntelArchitecture)Architecture, typeLoader, defaultConvention);
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            if (int3svc.SyscallInfo.Matches(vector, state))
                return int3svc;
            throw new NotImplementedException("INT services are not supported by " + this.GetType().Name);
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
            EnsureTypeLibraries("win64");
            var ep = TypeLibs.Select(t => t.Lookup(procName))
                .Where(sig => sig != null)
                .Select(s => new ExternalProcedure(procName, s))
                .FirstOrDefault();
            if (ep != null)
            {
                var ch = CharacteristicsLibs
                    .Select(c => c.Lookup(ep.Name))
                    .FirstOrDefault();
                ep.Characteristics = ch;
            }
            return ep;
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
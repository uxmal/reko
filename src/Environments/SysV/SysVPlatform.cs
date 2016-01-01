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

using Reko.Core;
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
using System.Text;

namespace Reko.Environments.SysV
{
    //$TODO: rename to Elf-Neutral?
    public class SysVPlatform : Platform
    {
        private TypeLibrary[] typelibs;
        private CharacteristicsLibrary[] CharacteristicsLibs;

        public SysVPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch)
        {
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override string PlatformIdentifier { get { return "elf-neutral"; } }

        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            return new X86_64ProcedureSerializer(Architecture, typeLoader, defaultConvention);
        }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            return Architecture.CreateRegisterBitset();
        }

        private void EnsureTypeLibraries()
        {
            if (typelibs == null)
            {
                var cfgSvc = Services.RequireService<IConfigurationService>();
                var envCfg = cfgSvc.GetEnvironment("elf-neutral");
                var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();
                this.typelibs = ((System.Collections.IEnumerable)envCfg.TypeLibraries)
                    .OfType<ITypeLibraryElement>()
                    .Select(tl => tlSvc.LoadLibrary(this, tl.Name))
                    .Where(tl => tl != null).ToArray();
                this.CharacteristicsLibs = ((System.Collections.IEnumerable)envCfg.CharacteristicsLibraries)
                    .OfType<ITypeLibraryElement>()
                    .Select(cl => tlSvc.LoadCharacteristics(cl.Name))
                    .Where(cl => cl != null).ToArray();
            }
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            EnsureTypeLibraries();
            return this.typelibs
                .Where(t => t.ServicesByVector != null && t.ServicesByVector.Count > 0)
                .SelectMany(t => t.ServicesByVector)
                .Where(svc => svc.Value.SyscallInfo.Matches(vector, state))
                .Select(svc => svc.Value)
                .FirstOrDefault();
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

            // Match x86 pattern.
            // jmp [destination]
            Address addrTarget = null;
            var jump = rtlc.Instructions[0] as RtlGoto;
            if (jump != null)
            {
                var pc = jump.Target as ProcedureConstant;
                if (pc != null)
                    return pc.Procedure;
                var access = jump.Target as MemoryAccess;
                if (access == null)
                    return null;
                addrTarget = access.EffectiveAddress as Address;
                if (addrTarget == null)
                {
                    var wAddr = access.EffectiveAddress as Constant;
                    if (wAddr == null)
                    {
                        return null;
                    }
                    addrTarget = MakeAddressFromConstant(wAddr);
                }
            }
            if (addrTarget == null)
                return null;
            ProcedureBase proc = host.GetImportedProcedure(addrTarget, rtlc.Address);
            if (proc != null)
                return proc;
            return host.GetInterceptedCall(addrTarget);
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            //$REVIEW: looks a lot like Win32library, perhaps push to parent class?
            EnsureTypeLibraries();
            var proc = typelibs.Select(t => t.Lookup(procName))
                        .Where(sig => sig != null)
                        .Select(s => new ExternalProcedure(procName, s))
                        .FirstOrDefault();
            var characteristics = CharacteristicsLibs.Select(cl => cl.Lookup(procName))
                .Where(c => c != null)
                .FirstOrDefault();
            if (characteristics != null)
                proc.Characteristics = characteristics;
            return proc;
        }
    }
}

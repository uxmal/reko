#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Arch.Motorola.M68k.Machine;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Machine;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;

namespace Reko.Environments.AtariTOS
{
    public class AtariTOSPlatform : Platform
    {
        public AtariTOSPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "atariTOS")
        {
            this.StructureMemberAlignment = 4;
            this.TrashedRegisters = new HashSet<RegisterStorage>
            {
                Registers.a0,
                Registers.a1,
                Registers.d0,
                Registers.d1,
            };
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override ICallingConvention GetCallingConvention(string? ccName)
        {
            if (ccName == "TOScall")
                return new TOSCallingConvention(this.Architecture);
            return new CallingConvention(this.Architecture);
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            foreach (var module in metadata.Modules.Values)
            {
                if (!module.ServicesByVector.TryGetValue(vector, out var svc))
                    continue;
                foreach (SystemService service in svc)
                {
                    if (service.SyscallInfo is not null && service.SyscallInfo.Matches(vector, state))
                        return service;
                }
            }
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}

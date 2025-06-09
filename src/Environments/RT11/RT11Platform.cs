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

using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Pdp11;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Machine;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.RT11
{
    // http://mdfs.net/Docs/Comp/PDP11/RT11/EMTcalls
    public class RT11Platform : Platform
    {
        private Pdp11Architecture arch;
        private IServiceProvider services;

        public RT11Platform(IServiceProvider services, Pdp11Architecture arch) 
            : base(services, arch, "RT-11")
        {
            this.services = services;
            this.arch = arch;
            this.StructureMemberAlignment = 2;
            this.TrashedRegisters = new HashSet<RegisterStorage> {
                Registers.r0,
            };
        }

        public override string DefaultCallingConvention => "";

        public override ICallingConvention GetCallingConvention(string? ccName)
        {
            return new Rt11CallingConvention(this.arch);
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            var metadata = base.EnsureTypeLibraries(PlatformIdentifier);
            int uVec = vector & 0xFFFF;
            foreach (var svc in metadata.Modules.Values.SelectMany(m => m.ServicesByOrdinal.Values))
            {
                if (svc.SyscallInfo is not null && svc.SyscallInfo.Matches(uVec, state))
                {
                    return svc;
                }
            }
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
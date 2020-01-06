#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Arch.M68k;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.CLanguage;
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.Environments.AtariTOS
{
    public class AtariTOSPlatform : Platform
    {
        public AtariTOSPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "atariTOS")
        {
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }
        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            throw new NotImplementedException();
        }

        public override CallingConvention GetCallingConvention(string ccName)
        {
            if (ccName == "TOSCall")
                return new TOSCallingConvention(this.Architecture);
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                Registers.a0,
                Registers.a1,
                Registers.d0,
                Registers.d1,
            };
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            if (Metadata == null)
            { }
            EnsureTypeLibraries(PlatformIdentifier);
            foreach (var module in this.Metadata.Modules.Values)
            {
                if (!module.ServicesByVector.TryGetValue(vector, out var svc))
                    continue;
            }
            //$BUG: does no work?
            return null;
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}

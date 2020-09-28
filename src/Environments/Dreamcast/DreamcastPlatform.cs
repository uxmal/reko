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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.CLanguage;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Core.Rtl;

namespace Reko.Environments.Dreamcast
{
    // http://mc.pp.se/dc/memory.html
    // http://mc.pp.se/dc/sw.html
    public class DreamcastPlatform : Platform
    {
        public DreamcastPlatform(IServiceProvider services, IProcessorArchitecture arch, string platformId) : base(services, arch, platformId)
        {
        }

        public override string DefaultCallingConvention
        {
            get
            {
                throw new NotImplementedException();
            }
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
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ProcedureBase GetTrampolineDestination(IEnumerable<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}

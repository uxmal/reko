#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Hll.C;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.PalmOS
{
    public class PalmOSPlatform : Platform
    {
        public PalmOSPlatform(IServiceProvider services, IProcessorArchitecture arch, string platformId)
            : base(services, arch, platformId)
        {
        }

        public override string DefaultCallingConvention => throw new NotImplementedException();

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            if (vector != 0xF || state is null || segmentMap is null)
                return null;
            var addrTrapNo = state.InstructionPointer + 2;
            if (!segmentMap.TryFindSegment(addrTrapNo, out var segment))
                return null;
            var mem = segment.MemoryArea;
            if (!mem.TryReadBeUInt16(addrTrapNo, out ushort trapNo))
                return null;

            return Traps.GetTrapSignature(trapNo);
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

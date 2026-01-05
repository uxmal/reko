#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.UnitTests.Mocks
{
    public class RtlTraceBuilder : IEnumerable<RtlTrace>
    {
        public RtlTraceBuilder()
        {
            this.Traces = new SortedList<Address, RtlTrace>();
        }

        public SortedList<Address, RtlTrace> Traces { get; private set; }

        public void Add(RtlTrace trace)
        {
            Traces.Add(trace.StartAddress, trace);
        }

        public IEnumerator<RtlTrace> GetEnumerator()
        {
            return Traces.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class RtlTrace : IEnumerable<RtlInstructionCluster>
    {
        private Address addr;
        private List<RtlInstructionCluster> clusters;
        private InstrClass iclassOption;

        public RtlTrace(uint addr, InstrClass iclassOption = 0)
        {
            this.addr = Address.Ptr32(addr);
            this.StartAddress = this.addr;
            this.clusters = new List<RtlInstructionCluster>();
            this.iclassOption = iclassOption;
        }

        public Address StartAddress { get; private set; }

        public void Add(Action<RtlEmitter> generator)
        {
            Add(4, generator);
        }

        public void Add(int byteSize, Action<RtlEmitter> generator)
        {
            var rtls = new List<RtlInstruction>();
            generator(new RtlEmitter(rtls));
            var artls = rtls.ToArray();
            clusters.Add(new RtlInstructionCluster(addr, byteSize, artls)
            {
                Class = artls[^1].Class | iclassOption,
            });
            addr += byteSize;
        }


        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            return clusters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

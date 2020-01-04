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
using Reko.Core.Rtl;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Expressions;
using Reko.Core.Lib;

namespace Reko.UnitTests.Scanning
{
    public class RtlBackwalkHost : IBackWalkHost<RtlBlock, RtlInstruction>
    {
        private Program program;
        private DirectedGraph<RtlBlock> graph;

        public RtlBackwalkHost(Program program, DirectedGraph<RtlBlock> graph)
        {
            this.program = program;
            this.graph = graph;
        }

        public IProcessorArchitecture Architecture => program.Architecture;
        public Program Program => program;
        public SegmentMap SegmentMap => program.SegmentMap;

        public Tuple<Expression, Expression> AsAssignment(RtlInstruction instr)
        {
            throw new NotImplementedException();
        }

        public Expression AsBranch(RtlInstruction instr)
        {
            throw new NotImplementedException();
        }

        public Address GetBlockStartAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public int BlockInstructionCount(RtlBlock block)
        {
            return block.Instructions.Sum(b => b.Instructions.Length);
        }

        public IEnumerable<RtlInstruction> GetBlockInstructions(RtlBlock block)
        {
            return block.Instructions.SelectMany(rtlc => rtlc.Instructions);
        }

        public RtlBlock GetSinglePredecessor(RtlBlock block)
        {
            var p = graph.Predecessors(block).SingleOrDefault();
            return p;
        }

        public List<RtlBlock> GetPredecessors(RtlBlock block)
        {
            return graph.Predecessors(block).ToList();
        }

        public AddressRange GetSinglePredecessorAddressRange(Address block)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage GetSubregister(RegisterStorage rIdx, int v1, int v2)
        {
            throw new NotImplementedException();
        }

        public bool IsFallthrough(RtlInstruction instr, RtlBlock block)
        {
            throw new NotImplementedException();
        }

        public bool IsStackRegister(Storage storage)
        {
            throw new NotImplementedException();
        }

        public bool IsValidAddress(Address addr)
        {
            return program.SegmentMap.IsValidAddress(addr);
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            return program.Architecture.MakeAddressFromConstant(c, true);
        }

        public Address MakeSegmentedAddress(Constant selector, Constant offset)
        {
            throw new NotImplementedException();
        }
    }
}

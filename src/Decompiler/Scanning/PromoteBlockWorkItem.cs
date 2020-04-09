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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    public class PromoteBlockWorkItem : WorkItem
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(PromoteBlockWorkItem), "Trace the workings of PromoteBlockWorkItem") { Level = TraceLevel.Info };

        public Block Block; 
        public Procedure ProcNew;
        public IScanner Scanner;
        public Program Program;

        public PromoteBlockWorkItem(Address addr) : base(addr)
        {
        }

        public override void Process()
        {
            var movedBlocks = new HashSet<Block>();
            var stack = new Stack<IEnumerator<Block>>();
            stack.Push(new Block[] { Block }.Cast<Block>().GetEnumerator());
            var replacer = new IdentifierRelocator(ProcNew.Frame);
            while (stack.Count != 0)
            {
                DumpBlocks(Block.Procedure);
                var e = stack.Peek();
                if (!e.MoveNext())
                {
                    stack.Pop();
                    continue;
                }
                var b = e.Current;
                if (b.Procedure == ProcNew || b == b.Procedure.ExitBlock || b.Procedure.EntryBlock.Succ[0] == b)
                    continue;

                trace.Verbose("PBW:     Visiting block {0}, stack depth {1}", b.Name, stack.Count);
                b.Procedure.RemoveBlock(b);
                ProcNew.AddBlock(b);
                b.Procedure = ProcNew;
                movedBlocks.Add(b);
                foreach (var stm in b.Statements)
                {
                    stm.Instruction = replacer.ReplaceIdentifiers(stm.Instruction);
                }
                if (b.Succ.Count > 0)
                    stack.Push(b.Succ.GetEnumerator());
            }
            foreach (var b in movedBlocks)
            {
                FixExitEdges(b);
                FixInboundEdges(b);
                FixOutboundEdges(b);
               //  SanityCheck(b);
            }
        }

        [Conditional("DEBUG")]
        public static void SanityCheck(Block block)
        {
         //   Debug.Assert(block.Pred.Count == 0 && block != block.Procedure.EntryBlock);
        }

        [Conditional("DEBUG")]
        private void DumpBlocks(Procedure procedure)
        {
            trace.Verbose("{0}", procedure.Name);
            foreach (var block in procedure.ControlGraph.Blocks)
            {
                trace.Verbose("  {0}; {1}", block.Name, block.Procedure.Name);
            }
        }

        public void FixInboundEdges(Block blockToPromote)
        {
            trace.Verbose("PBW: Fixing inbound edges of {0}", blockToPromote.Name);

            // Get all blocks that are from "outside" blocks.
            var inboundBlocks = blockToPromote.Pred.Where(p => p.Procedure != ProcNew).ToArray();
            foreach (var inb in inboundBlocks)
            {
                if (inb.Statements.Count > 0)
                {
                    var lastAddress = GetAddressOfLastInstruction(inb);
                    var callRetThunkBlock = Scanner.CreateCallRetThunk(lastAddress, inb.Procedure, ProcNew);
                    ReplaceSuccessorsWith(inb, blockToPromote, callRetThunkBlock);
                    callRetThunkBlock.Pred.Add(inb);
                }
                else
                {
                    inb.Statements.Add(
                        inb.Address.ToLinear(),
                        new CallInstruction(
                            new ProcedureConstant(Program.Platform.PointerType, ProcNew),
                            new CallSite(0, 0)));
                    Program.CallGraph.AddEdge(inb.Statements.Last, ProcNew);
                    inb.Statements.Add(inb.Address.ToLinear(), new ReturnInstruction());
                    inb.Procedure.ControlGraph.AddEdge(inb, inb.Procedure.ExitBlock);
                }
            }
            foreach (var p in inboundBlocks)
            {
                blockToPromote.Pred.Remove(p);
            }
        }

        private Address GetAddressOfLastInstruction(Block inboundBlock)
        {
            if (inboundBlock.Statements.Count == 0)
                return Program.Platform.MakeAddressFromLinear(0, true);
            return inboundBlock.Address != null
                ? inboundBlock.Address + (inboundBlock.Statements.Last.LinearAddress - inboundBlock.Statements[0].LinearAddress)
                : Program.Platform.MakeAddressFromLinear(inboundBlock.Statements.Last.LinearAddress, true);
        }

        public void FixOutboundEdges(Block block)
        {
            trace.Verbose("PBW: Fixing outbound edges of {0}", block.Name);
            for (int i = 0; i < block.Succ.Count; ++i)
            {
                var s = block.Succ[i];
                if (s.Procedure == block.Procedure)
                    continue;
                if (s.Procedure.EntryBlock.Succ[0] == s)
                {
                    // s is the first block of a (different) procedure
                    var lastAddress = GetAddressOfLastInstruction(block);
                    var retCallThunkBlock = Scanner.CreateCallRetThunk(lastAddress, block.Procedure, s.Procedure);
                    block.Succ[i] = retCallThunkBlock;
                    retCallThunkBlock.Pred.Add(block);
                    s.Pred.Remove(block);
                }
            }
        }

        private void ReplaceSuccessorsWith(Block block, Block blockOld, Block blockNew)
        {
            for (int s = 0; s < block.Succ.Count; ++s)
            {
                if (block.Succ[s] == blockOld)
                    block.Succ[s] = blockNew;
            }
        }

        private void FixExitEdges(Block block)
        {
            trace.Verbose("PBW: Fixing exit edges of {0}", block.Name);
            for (int i = 0; i < block.Succ.Count; ++i)
            {
                var s = block.Succ[i];
                if (s.Procedure != ProcNew && s == s.Procedure.ExitBlock)
                {
                    s.Pred.Remove(block);
                    ProcNew.ExitBlock.Pred.Add(block);
                    block.Succ[i] = ProcNew.ExitBlock;
                }
            }
        }
    }
}

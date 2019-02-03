#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Services;

namespace Reko.Analysis
{
    /// <summary>
    /// This class cleans up the control flow graph by collecting together
    /// sequences of branches like:
    /// <code>
    /// Pred
    /// if (C) PreConditional
    ///  | \
    ///  |  PredConditional
    ///  | /
    ///  Block
    ///  if (C) SuccConditional
    ///  | \
    ///  |  SuccConditional
    ///  | /
    ///  Final
    /// skip2:
    /// </code>
    /// into the more easily read:
    /// <code>
    /// Pred
    /// if (C) PreConditional
    ///  | \
    ///  |  PredConditional
    ///  |  SuccConditional
    ///  | /
    ///  Final
    /// </code>
    /// </summary>
    /// <remarks>
    /// This kind of IL code is especially common when decompiling 
    /// ARM machine code, or any other machine code that uses predicated
    /// execution.
    /// </remarks>
    public class AdjacentBranchCollector
    {
        private static TraceSwitch trace = new TraceSwitch(nameof(AdjacentBranchCollector), nameof(AdjacentBranchCollector)) { Level = TraceLevel.Error };

        private readonly Procedure proc;
        private readonly DecompilerEventListener listener;
        private readonly ExpressionValueComparer cmp;

        public static void Transform(Program program, DecompilerEventListener eventListener)
        {
            foreach (var proc in program.Procedures.Values)
            {
                if (eventListener.IsCanceled())
                    return;
                var abc = new AdjacentBranchCollector(proc, eventListener);
                abc.Transform();
            }
        }
        public AdjacentBranchCollector(Procedure proc, DecompilerEventListener listener)
        {
            this.proc = proc;
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
        }

        private class Candidate
        {
            public Block Predecessor;
            public TestCondition PredecessorTest;
            public Block PredecessorConditional;
            public Block Block;
            public TestCondition BlockTest;
            public Block SuccessorConditional;
            public Block Final;
        }

        public void Transform()
        {
            var wl = new WorkList<Block>(proc.ControlGraph.Blocks);
            while (wl.GetWorkItem(out Block block))
            {
                if (listener.IsCanceled())
                    return;
                var c = DetermineCandidate(block);
                if (c == null)
                    continue;
                FuseIntoPredecessor(c);

                // We may need to mutate the predecessor again.
                wl.Add(c.Predecessor);
            }
        }

        /// <summary>
        /// Determine if the block is the middle block of the following
        /// graph pattern:
        /// Pred
        ///  | \
        ///  |  PredConditional
        ///  | /
        ///  Block
        ///  | \
        ///  |  SuccConditional
        ///  | /
        ///  Final
        /// </summary>
        private Candidate DetermineCandidate(Block block)
        {
            if (block.Pred.Count != 2)
                return null;
            if (block.Succ.Count != 2)
                return null;
            var blockTest = this.BlockTest(block, true);
            if (blockTest == null)
                return null;
            // Determine the blocks in the successor triangle
            var final = block.Succ[1];
            var succCond = block.Succ[0];
            if (succCond.Pred.Count != 1)
                return null;
            if (final.Pred.Count != 2)
                return null;
            // Determine the blocks in the predecessor triangle.
            var pred = block.Pred[0];
            var predCond = block.Pred[1];
            if (pred.Succ.Count != 2)
            {
                pred = block.Pred[1];
                predCond = block.Pred[0];
            }
            if (pred.Succ.Count != 2)
                return null;
            if (predCond.Pred.Count != 1)
                return null;
            var predTest = BlockTest(pred, false);
            if (predTest == null)
                return null;

            if (!cmp.Equals(predTest, blockTest))
                return null;


            var v = DetermineConditionalIdentifier(predTest);
            if (v == null)
                return null;
            if (BlockTrashesIdentifier(predCond, v))
                return null;

            DebugEx.PrintIf(trace.TraceVerbose, "ABC: Candidate pred: {0}, block {1}", pred.Name, block.Name);
            return new Candidate
            {
                Predecessor = pred,
                PredecessorTest = predTest,
                PredecessorConditional = predCond,
                Block = block,
                BlockTest = blockTest,
                SuccessorConditional = succCond,
                Final = final
            };
        }

        private bool BlockTrashesIdentifier(Block block, Identifier id)
        {
            bool IsTrashed(Identifier dst, Identifier src)
            {
                var dstStg = dst.Storage as FlagGroupStorage;
                var srcStg = src.Storage as FlagGroupStorage;
                if (dstStg != null && srcStg != null)
                {
                    //$TODO: when moved to analysis-development,
                    // simply use Overlap.
                    return (dstStg.FlagGroupBits & srcStg.FlagGroupBits) != 0;
                }
                return false;
            }

            bool ApplOutArgumentTrashesIdentifier(Expression e)
            {
                if (e is Application appl)
                {
                    if (appl.Arguments.OfType<OutArgument>()
                        .Any(a => a.Expression is Identifier i && 
                                    IsTrashed(i, id)))
                        return true;
                }
                return false;
            }

            foreach (var stm in block.Statements)
            {
                switch (stm.Instruction)
                {
                case Assignment ass:
                    if (IsTrashed(ass.Dst, id))
                        return true;
                    if (ApplOutArgumentTrashesIdentifier(ass.Src))
                        return true;
                    break;
                case SideEffect side:
                    if (ApplOutArgumentTrashesIdentifier(side.Expression))
                        return true;
                    break;
                case CallInstruction call:
                    // Pessimistic assumption
                    return true;
                }
            }
            return false;
        }

        private Identifier DetermineConditionalIdentifier(TestCondition predTest)
        {
            return predTest.Expression as Identifier;
        }

        /// <summary>
        /// If <paramref name="block"/> has single predecessor, and that
        /// predecessor has <paramref name="block"/> as its single successor,
        /// return it.
        /// </summary>
        private Block ApplicablePredecessor(Block block)
        {
            if (block.Pred.Count != 2)
                return null;
            var pred = block.Pred[0];
            if (pred.Succ.Count != 1 || pred.Succ[0] != block)
            {
                pred = block.Pred[1];
                if (pred.Succ.Count != 1 || pred.Succ[0] != block)
                {
                    return null;
                }
            }
            // We have a possible predecessor in pred.
            return pred;
        }

        private TestCondition BlockTest(Block block, bool singleStatement)
        {
            var c = block.Statements.Count;
            if (c == 0 || (singleStatement && c != 1))
                return null;
            if (!(block.Statements[c-1].Instruction is Branch branch))
                return null;
            return branch.Condition as TestCondition;
        }

        /// <summary>
        /// Given blocks whose test conditions are the same,
        /// mutate the control graph so that the <paramref name="pred"/> block
        /// contains the conditional blocks of both test blocks.
        /// </summary>
        private void FuseIntoPredecessor(Candidate c)
        {
            // Transfer all successor conditional statements first.
            var predStms = c.PredecessorConditional.Statements;
            foreach (var stm in c.SuccessorConditional.Statements)
            {
                stm.Block = c.PredecessorConditional;
                predStms.Add(stm);
            }
            // Redirect edges from pred and predCond to point to
            // final.
            Block.ReplaceJumpsFrom(c.Block, c.Predecessor);
            Block.ReplaceJumpsFrom(c.SuccessorConditional, c.PredecessorConditional);
            Block.ReplaceJumpsTo(c.Block, c.Final);
            c.Predecessor.Statements.Last.Instruction = new Branch(
                c.PredecessorTest, c.Final);
            // Delete the skipped blocks
            var cfg = proc.ControlGraph;
            cfg.RemoveEdge(c.Block, c.Final);
            cfg.RemoveEdge(c.Block, c.SuccessorConditional);
            cfg.Blocks.Remove(c.Block);
            cfg.Blocks.Remove(c.SuccessorConditional);
        }

    }
}

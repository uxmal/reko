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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Services;
using System.Diagnostics;
using System.Linq;

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
    ///  if (C) Final2
    ///  | \
    ///  |  Final2
    ///  |
    ///  Final1
    /// skip2:
    /// </code>
    /// into the more easily read:
    /// <code>
    /// Pred
    /// if (C) PreConditional
    ///  | \
    ///  |  PredConditional
    ///  |  |
    ///  |  Final2
    ///  |
    ///  Final1
    /// </code>
    /// Final1 and Final2 could be the same block or different blocks.
    /// </summary>
    /// <remarks>
    /// This kind of IL code is especially common when decompiling 
    /// ARM machine code, or any other machine code that uses predicated
    /// execution.
    /// </remarks>
    public class AdjacentBranchCollector
    {
        private static readonly TraceSwitch trace = new (nameof(AdjacentBranchCollector), nameof(AdjacentBranchCollector)) { Level = TraceLevel.Error };

        private readonly Procedure proc;
        private readonly IDecompilerEventListener listener;
        private readonly ExpressionValueComparer cmp;

        public static void Transform(Program program, IDecompilerEventListener eventListener)
        {
            foreach (var proc in program.Procedures.Values)
            {
                if (eventListener.IsCanceled())
                    return;
                var abc = new AdjacentBranchCollector(proc, eventListener);
                abc.Transform();
            }
        }

        public AdjacentBranchCollector(Procedure proc, IDecompilerEventListener listener)
        {
            this.proc = proc;
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
        }

        //$REVIEW: this is a Record, and ideally would have non-nullable fields. C# 9?
        private class Candidate
        {
            public Block? Predecessor;
            public Branch? PredecessorTest;
            public Block? PredecessorConditional;
            public Block? Block;
            public Branch? BlockTest;
            public Block? Final1;
            public Block? Final2;
        }

        public void Transform()
        {
            var wl = WorkList.Create(proc.ControlGraph.Blocks);
            while (wl.TryGetWorkItem(out Block? block))
            {
                if (listener.IsCanceled())
                    return;
                var c = DetermineCandidate(block);
                if (c is null)
                    continue;
                FuseIntoPredecessor(c);

                // We may need to mutate the predecessor again.
                wl.Add(c.Predecessor!);
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
        ///  |  Final2
        ///  |
        ///  Final
        /// </summary>
        private Candidate? DetermineCandidate(Block block)
        {
            if (block.Pred.Count != 2)
                return null;
            if (block.Succ.Count != 2)
                return null;
            var blockTest = BlockTest(block, true);
            if (blockTest is null)
                return null;
            // Determine the final blocks
            var final1 = block.Succ[1];
            var final2 = block.Succ[0];
            if (final2.Pred.Count != 1)
                return null;
            // Determine the blocks in the predecessor triangle.
            var pred = block.Pred[0];
            var predCond = block.Pred[1];
            if (pred.Succ.Count != 2)
            {
                pred = block.Pred[1];
                predCond = block.Pred[0];
            }
            if (pred == block)
                return null;
            if (pred.Succ.Count != 2)
                return null;
            if (predCond.Pred.Count != 1)
                return null;
            var predTest = BlockTest(pred, false);
            if (predTest is null)
                return null;

            if (!cmp.Equals(predTest.Condition, blockTest.Condition))
                return null;

            var v = DetermineConditionalIdentifier(predTest.Condition);
            if (v is null)
                return null;
            if (BlockTrashesIdentifier(predCond, v))
                return null;

            trace.Verbose("ABC: Candidate pred: {0}, block {1}", pred.Id, block.Id);
            return new Candidate
            {
                Predecessor = pred,
                PredecessorTest = predTest,
                PredecessorConditional = predCond,
                Block = block,
                BlockTest = blockTest,
                Final1 = final1,
                Final2 = final2,
            };
        }

        private static bool BlockTrashesIdentifier(Block block, Identifier id)
        {
            bool ApplOutArgumentTrashesIdentifier(Expression e)
            {
                if (e is Application appl)
                {
                    if (appl.Arguments.OfType<OutArgument>()
                        .Any(a => a.Expression is Identifier i && 
                                    i.Storage.OverlapsWith(id.Storage)))
                        return true;
                }
                return false;
            }

            foreach (var stm in block.Statements)
            {
                switch (stm.Instruction)
                {
                case Assignment ass:
                    if (ass.Dst.Storage.OverlapsWith(id.Storage))
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

        private static Identifier? DetermineConditionalIdentifier(Expression e)
        {
            return (e is TestCondition predTest)
                ? predTest.Expression as Identifier
                : null;
        }

        /// <summary>
        /// If <paramref name="block"/> has single predecessor, and that
        /// predecessor has <paramref name="block"/> as its single successor,
        /// return it.
        /// </summary>
        private static Block? ApplicablePredecessor(Block block)
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

        private static Branch? BlockTest(Block block, bool singleStatement)
        {
            var c = block.Statements.Count;
            if (c == 0 || (singleStatement && c != 1))
                return null;
            return block.Statements[c - 1].Instruction as Branch;
        }

        /// <summary>
        /// Given blocks whose test conditions are the same,
        /// mutate the control graph so that the <paramref name="pred"/> block
        /// contains the conditional blocks of both test blocks.
        /// </summary>
        private void FuseIntoPredecessor(Candidate c)
        {
            // 'Block' is dead, so unhook it from the graph.
            c.PredecessorConditional!.Succ[0] = c.Final2!;
            c.Final2!.Pred[0] = c.PredecessorConditional;
            c.PredecessorTest!.Target = c.Final1!;
            Block.ReplaceJumpsTo(c.Block!, c.Final1!);

            // Delete the dead block
            var cfg = proc.ControlGraph;
            cfg.Blocks.Remove(c.Block!);
        }
    }
}

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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Collections.Generic;

namespace Reko.Analysis;

/// <summary>
/// Replaces long comparison sequences with single statements.
/// </summary>
/// <remarks>
/// This class recognizes patterns like:
/// <code>
///     if (c1 &lt; 0) branch Label1
///     if (c2 != 0) branch Label2
///     if (c2 &lt; n) branch Label1
///    label2:
/// </code>
/// And replaces them with:
/// <code>
///     if (c1:c2 &lt;= 0:n) branch Label1
///    label2:
/// </code>
/// </remarks>
public class LongComparisonFuser : IAnalysis<SsaState>
{
    private readonly AnalysisContext context;

    /// <summary>
    /// Constructs an instance of <see cref="LongComparisonFuser"/>.
    /// </summary>
    /// <param name="context"></param>
    public LongComparisonFuser(AnalysisContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public string Id => "lcf";

    /// <inheritdoc/>
    public string Description => "Fuses partial comparisons into long comparisons";

    /// <inheritdoc/>
    public (SsaState, bool) Transform(SsaState ssa)
    {
        var worker = new Worker(ssa, context.EventListener);
        worker.Transform();
        return (ssa, true); //$TODO: actually compute the dirtiness.
    }

    private class Worker
    {
        private readonly SsaState ssa;
        private readonly IEventListener listener;
        private readonly ExpressionEmitter m;

        public Worker(SsaState ssa, IEventListener listener)
        {
            this.ssa = ssa;
            this.listener = listener;
            this.m = new ExpressionEmitter();
        }

        public bool Transform()
        {
            bool changed = false;
            var deadBlocks = new HashSet<Block>();
            foreach (var block in ssa.Procedure.ControlGraph.Blocks)
            {
                if (listener.IsCanceled())
                    return changed;
                var candidate = ProbeCandidate(block);
                if (candidate is not null)
                {
                    changed |= ApplyCandidate(candidate, deadBlocks);
                }
            }
            DestroyBlocks(deadBlocks);
            return changed;
        }

        private Candidate? ProbeCandidate(Block block)
        {
            // Find the three chained blocks with comparisons.
            if (block.Statements.Count == 0)
                return null;
            if (block.Statements[^1].Instruction is not Branch branch1)
                return null;
            //$TODO: architectures with delay slots are generating bad if-instructions
            // with 0 successor blocks.
            if (block.Succ.Count != 2)
                return null;
            var target1 = block.Succ[1];
            var block2 = block.Succ[0];

            if (block2.Statements.Count != 1)
                return null;
            if (block2.Statements[^1].Instruction is not Branch branch2)
                return null;
            var target2 = block2.Succ[1];
            var block3 = block2.Succ[0];

            if (block3.Statements.Count == 0)
                return null;
            if (block3.Statements[^1].Instruction is not Branch branch3)
                return null;
            var target3 = block3.Succ[1];
            var fallthrough = block3.Succ[0];

            if (target1 != target3)
                return null;
            if (target2 != fallthrough)
                return null;

            // Look at the comparisons and classify them.
            var cmp1 = Classify(branch1);
            var cmp2 = Classify(branch2);
            var cmp3 = Classify(branch3);
            if (cmp1.op is null || cmp2.op is null || cmp3.op is null)
                return null;

            if (cmp1.op == Operator.Lt &&
                cmp2.op == Operator.Ne)
            {
                return new Candidate(
                    cmp3.op,
                    cmp1.left!, cmp3.left!,
                    cmp1.right!, cmp3.right!,
                    block,
                    block2,
                    block3);
            }
            if (cmp1.op == Operator.Gt &&
                cmp2.op == Operator.Lt &&
                cmp3.op == Operator.Uge)
            {
                return new Candidate(
                    Operator.Ge,
                    cmp1.left!, cmp3.left!,
                    cmp1.right!, cmp3.right!,
                    block,
                    block2,
                    block3);
            }
            if (cmp1.op == Operator.Ugt &&
                cmp2.op == Operator.Ult &&
                cmp3.op == Operator.Uge)
            {
                return new Candidate(
                    Operator.Uge,
                    cmp1.left!, cmp3.left!,
                    cmp1.right!, cmp3.right!,
                    block,
                    block2,
                    block3);
            }
            if (cmp1.op == Operator.Ult &&
                cmp2.op == Operator.Ugt &&
                cmp3.op == Operator.Ule)
            {
                return new Candidate(
                    Operator.Ule,
                    cmp1.left!, cmp3.left!,
                    cmp1.right!, cmp3.right!,
                    block,
                    block2,
                    block3);
            }
            return null;
        }

        private (BinaryOperator? op, Expression? left, Expression? right) Classify(Branch branch)
        {
            if (branch.Condition is BinaryExpression bin)
            {
                return (bin.Operator, bin.Left, bin.Right);
            }
            else
            {
                return (null, null, null);
            }
        }

        private class Candidate
        {
            public BinaryOperator op;
            public Expression left1;
            public Expression left2;
            public Expression right1;
            public Expression right2;
            public Block block;
            public Block block2;
            public Block block3;

            public Candidate(
                BinaryOperator op, 
                Expression left1, Expression left2,
                Expression right1, Expression right2,
                Block block, 
                Block block2,
                Block block3)
            {
                this.op = op;
                this.left1 = left1;
                this.left2 = left2;
                this.right1 = right1;
                this.right2 = right2;
                this.block = block;
                this.block2 = block2;
                this.block3 = block3;
            }
        }

        private bool ApplyCandidate(Candidate candidate, ICollection<Block> deadBlocks)
        {
            var longLeft = Join(candidate.left1, candidate.left2);
            var longRight = Join(candidate.right1, candidate.right2);
            var newCmp = m.Bin(
                candidate.op,
                PrimitiveType.Bool,
                longLeft, longRight);
            var branch = (Branch) candidate.block.Statements[^1].Instruction;
            ssa.RemoveUses(candidate.block.Statements[^1]);
            candidate.block.Statements[^1].Instruction = new Branch(newCmp, branch.Target);
            ssa.AddUses(candidate.block.Statements[^1]);

            // Edit the graph to bypass the partial comparisons.
            candidate.block.ElseBlock.Pred.Remove(candidate.block);
            candidate.block.ElseBlock = candidate.block3.ElseBlock;
            candidate.block.ElseBlock.Pred.Add(candidate.block);

            // Destroy the unused blocks.
            deadBlocks.Add(candidate.block2);
            deadBlocks.Add(candidate.block3);
            return true;
        }

        private void DestroyBlocks(IEnumerable<Block> blocks)
        {
            foreach (var block in blocks)
            {
                foreach (var stm in block.Statements)
                {
                    ssa.ReplaceDefinitions(stm, null);
                    ssa.RemoveUses(stm);
                }
                block.Procedure.ControlGraph.RemoveBlock(block);
            }
        }

        private Expression Join(Expression left1, Expression left2)
        {
            var dtnew = PrimitiveType.CreateWord(left1.DataType.BitSize + left2.DataType.BitSize);
            return m.Seq(dtnew, left1, left2);
        }
    }
}

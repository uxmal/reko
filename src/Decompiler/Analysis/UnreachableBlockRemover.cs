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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis;

/// <summary>
/// Eliminates blocks that are unreachable; typically blocks guarded by 
/// 'if's that evaluate to false or non-false.
/// </summary>
public class UnreachableBlockRemover : IAnalysis<SsaState>
{
    private readonly AnalysisContext context;

    public UnreachableBlockRemover(AnalysisContext context)
    {
        this.context = context;
    }

    public string Id => "urb";

    public string Description => "Removes unreachable basic blocks";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        var worker = new Worker(ssa, context.EventListener);
        var changed= worker.Transform();
        return (ssa, changed);
    }

    private class Worker
    {
        private readonly SsaState ssa;
        private readonly IEventListener listener;
        private readonly BlockGraph cfg;

        public Worker(SsaState ssa, IEventListener listener)
        {
            this.ssa = ssa;
            this.listener = listener;
            this.cfg = ssa.Procedure.ControlGraph;
        }

        public bool Transform()
        {
            // Find all blocks which end in branches whose conditional expression is constant.
            var constBranchBlocks = ssa.Procedure.ControlGraph.Blocks.Where(HasConstantBranch).ToHashSet();
            if (constBranchBlocks.Count == 0)
                return false;

            // Determine which blocks are unreachable based on the constant branches.

            var unreachableBlocks = constBranchBlocks.Select(UnreachableBlock).ToHashSet();

            // Traverse the CFG of the procedure but don't enter unreachable blocks.

            var reachedBlocks = TraverseCfg(ssa.Procedure.EntryBlock, unreachableBlocks, new HashSet<Block>());
            if (listener.IsCanceled())
                return false;

            // Compute the dead blocks.
            var deadBlocks = ssa.Procedure.ControlGraph.Blocks.Except(reachedBlocks).ToHashSet();

            RemoveConstBranches(constBranchBlocks);
            if (listener.IsCanceled())
                return true;

            RemoveDeadBlocks(deadBlocks);
            return true;
        }

        private bool HasConstantBranch(Block block)
        {
            if (block.Statements.Count < 1)
                return false;
            var stmLast = block.Statements[^1];
            if (stmLast is not null && stmLast.Instruction is Branch branch)
            {
                return branch.Condition is Constant;
            }
            return false;
        }

        private Block UnreachableBlock(Block block)
        {
            var c = (Constant) ((Branch) block.Statements[^1].Instruction).Condition;
            if (c.IsZero)
                return block.ThenBlock;
            else
                return block.ElseBlock;
        }

        private HashSet<Block> TraverseCfg(Block block, HashSet<Block> unreachableBlocks, HashSet<Block> visited)
        {
            if (!visited.Contains(block))
            {
                visited.Add(block);
                foreach (var succ in block.Succ)
                {
                    if (!unreachableBlocks.Contains(succ))
                        TraverseCfg(succ, unreachableBlocks, visited);
                }
            }
            return visited;
        }

        private void RemoveConstBranches(IEnumerable<Block> constBranchBlocks)
        {
            foreach (var block in constBranchBlocks)
            {
                var c = (Constant) ((Branch) block.Statements[^1].Instruction).Condition;
                if (c.IsZero)
                {
                    cfg.RemoveEdge(block, block.ThenBlock);
                }
                else
                {
                    cfg.RemoveEdge(block, block.ElseBlock);
                }
                // Remove the branch statement at the end of the block.
                Debug.Assert(block.Statements[^1].Instruction is Branch);
                block.Statements.RemoveAt(block.Statements.Count - 1);
            }
        }

        private void RemoveDeadBlocks(HashSet<Block> deadBlocks)
        {
            foreach (var deadBlock in deadBlocks)
            {
                // Remove from PHI functions.
                RemoveFromPhiFunctions(deadBlock, deadBlocks);

                RemoveStatements(deadBlock);

                cfg.RemoveBlock(deadBlock);
            }
        }

        private void RemoveFromPhiFunctions(Block block, HashSet<Block> deadBlocks)
        {
            foreach (var succ in block.Succ)
            {
                // Don't bother with blocks that are dead.
                if (deadBlocks.Contains(succ))
                    continue;

                foreach (var stm in succ.Statements)
                {
                    if (stm.Instruction is PhiAssignment phi)
                    {
                    var args = new List<PhiArgument>();
                        foreach (var arg in phi.Src.Arguments)
                        {
                            if (arg.Block != block)
                            {
                                args.Add(arg);
                            }
                            else
                            {
                                ssa.Identifiers[(Identifier) arg.Value].Uses.Remove(stm);
                            }
                        }
                        if (args.Count != phi.Src.Arguments.Length)
                        {
                            if (args.Count != 1)
                            {
                                stm.Instruction = new PhiAssignment(phi.Dst, args.ToArray());
                            }
                            else
                            {
                                stm.Instruction = new Assignment(phi.Dst, args[0].Value);
                            }
                        }
                    }
                }
            }
        }

        private void RemoveStatements(Block block)
        {
            var deadStms = block.Statements.ToHashSet();
            var deadSids = new List<SsaIdentifier>();
            foreach (var sid in ssa.Identifiers)
            {
                if (sid.DefStatement is not null && deadStms.Contains(sid.DefStatement))
                {
                    deadSids.Add(sid);
                }
                sid.Uses.RemoveAll(u => deadStms.Contains(u));
            }
            foreach (var sid in deadSids)
            {
                ssa.Identifiers.Remove(sid);
            }
            block.Statements.Clear();
        }
    }
}

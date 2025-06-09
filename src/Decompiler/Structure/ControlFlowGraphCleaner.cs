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
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using System;
using System.Diagnostics;
using System.Linq;

namespace Reko.Structure
{
	/// <summary>
	/// Uses the CLEAN algorithm to clean up a control flow graph
	/// </summary>
	public class ControlFlowGraphCleaner
	{
		private readonly Procedure proc;
		private bool dirty;

		public ControlFlowGraphCleaner(Procedure proc)
		{
			this.proc = proc;
		}

		private bool BranchTargetsEqual(Block block)
		{
			return (block.ElseBlock == block.ThenBlock);
		}

		public Block Coalesce(Block block, Block next)
		{
			Block.Coalesce(block, next);
			dirty = true;
			return block;
		}

		private bool EndsInBranch(Block block)
		{
			if (block.Succ.Count != 2)
				return false;
			if (block.Statements.Count < 1)
				return false;
			return block.Statements[^1].Instruction is Branch;
		}

		public bool EndsInJump(Block block)
		{
            if (block.Succ.Count != 1)
                return false;
            if (block.Statements.Count < 1)
                return true;
            return !(block.Statements[^1].Instruction is SwitchInstruction);
		}

		private void ReplaceBranchWithJump(Block block)
		{
            Debug.Assert(block.Statements.Count >= 1);
            Debug.Assert(block.Statements[^1].Instruction is Branch);
            var branch = block.Statements[^1];
            var condition = ((Branch)branch.Instruction).Condition;
            block.Statements.Remove(branch);
            if (CriticalInstruction.IsCritical(condition))
            {
                var linearAddr = branch.Address;
                block.Statements.Add(linearAddr, new SideEffect(condition));
            }
            proc.ControlGraph.RemoveEdge(block, block.Succ[0]);
			dirty = true;
		}

		public void Transform()
		{
			do
			{
				dirty = false;

                foreach (var block in new DfsIterator<Block>(proc.ControlGraph).PostOrder().ToList())
                {
                    if (block is null)
                        continue;

                    if (EndsInBranch(block))
                    {
                        if (BranchTargetsEqual(block))
                        {
                            ReplaceBranchWithJump(block);
                        }
                        foreach (var s in block.Succ.ToList())
                        {
                            if (s.Statements.Count == 0 &&
                                s.Pred.Count == 1 &&
                                EndsInJump(s))
                            {
                                var sSucc = s.Succ[0];
                                Block.ReplaceJumpsTo(s, sSucc);
                                proc.ControlGraph.RemoveEdge(s, sSucc);
                                proc.ControlGraph.Blocks.Remove(s);
                                dirty = true;
                            }
                        }
                    }

                    if (EndsInJump(block))
                    {
                        Block next = block.Succ[0];
                        if (next.Pred.Count == 1 && next != proc.ExitBlock)
                        {
                            Coalesce(block, next);
                        }
                        else if (block != proc.EntryBlock && 
                            block.Statements.Count == 0 &&
                            next.Pred.Count == 1)
                        {
                            Block.ReplaceJumpsTo(block, next);
                            proc.ControlGraph.Blocks.Remove(block);
                            dirty = true;
                        }
#if IGNORE
						// This bollixes up the graphs for ForkedLoop.asm, so we can't use it.		
						// It's not as important as the other three clean stages.

						else if (EndsInBranch(next) && next.Statements.Count == 1)
						{
							ReplaceJumpWithBranch(block, next);
						}
#endif
                    }
                }
            } while (dirty);

			proc.Dump(true);
		}
	}
}

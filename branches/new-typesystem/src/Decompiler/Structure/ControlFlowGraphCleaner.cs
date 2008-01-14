/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using System;
using System.Collections;
using System.Diagnostics;

namespace Decompiler.Structure
{
	/// <summary>
	/// Uses the CLEAN algorithm to clean up a control flow graph
	/// </summary>
	public class ControlFlowGraphCleaner
	{
		private Procedure proc;
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
			return block.Statements.Last.Instruction is Branch;
		}

		public bool EndsInJump(Block block)
		{
			return (block.Succ.Count == 1);
		}

		private void ReplaceBranchWithJump(Block block)
		{
			block.Statements.RemoveAt(block.Statements.Count - 1);
			Block.RemoveEdge(block, block.Succ[0]);
			dirty = true;
		}


		private void ReplaceJumpWithBranch(Block b1, Block b2)
		{
			Branch br = b2.Statements.Last.Instruction as Branch;
			Block.RemoveEdge(b1, b2);
			b1.Statements.Add(new Branch(br.Condition));
			Block.AddEdge(b1, b2.Succ[0]);
			Block.AddEdge(b1, b2.Succ[1]);
		}

		public void Transform()
		{
			do
			{
				dirty = false;

				for (int i = proc.RpoBlocks.Count - 1; i >= 0; --i)
				{
					Block block = proc.RpoBlocks[i];
					if (block == null)
						continue;

					if (EndsInBranch(block))
					{
						if (BranchTargetsEqual(block))
						{
							ReplaceBranchWithJump(block);
						}
					}

					if (EndsInJump(block))
					{
						Block next = block.Succ[0];
						if (block != proc.EntryBlock && block.Statements.Count == 0)
						{
							if (Block.ReplaceJumpsTo(block, next))
								dirty = true;
						}
						if (next.Pred.Count == 1)
						{
							Coalesce(block, next);
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

					Debug.WriteLine("================");
					proc.Dump(true, false);
				}
			} while (dirty);

			proc.RenumberBlocks();
			proc.Dump(true, false);
		}
	}
}

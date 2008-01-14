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
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Diagnostics;

namespace Decompiler.Structure
{
	/// <summary>
	/// Rewrites sequences of branches to if-then and if-then-else statements.
	/// </summary>
	public class IfRewriter : StructureTransform
	{
		private Procedure proc;
		private BitSet blocks;
		private Block head;						// the first of the blocks to linearize.
		private Block continueTarget;			// this is where 'continue' statements go.
		private Block breakTarget;				// this is where 'break' statements go.
		private AbsynStatement linearized;

		private DominatorGraph domGraph;

		/// <summary>
		/// Creates an IfRewriter.
		/// </summary>
		/// <param name="proc">Procedure in which to work</param>
		/// <param name="dom">Dominator graph of the procedure</param>
		/// <param name="blocks">The blocks we want to linearize.</param>
		/// <param name="cont">If non-null, the header of an enclosing loop</param>
		/// <param name="brk">If non-null, the follow node of an enclosing loop</param>
		public IfRewriter(Procedure proc, DominatorGraph dom, BitSet blocks, Block head, Block cont, Block brk) : base(proc)
		{
			this.proc = proc;
			this.blocks = blocks;
			this.head = head;
			this.continueTarget = cont;
			this.breakTarget = brk;
			this.domGraph = dom;
		}


		/// <summary>
		/// Builds the if statements contained in the loop.
		/// </summary>
		/// <param name="blockHead">Header block of the loop</param>
		/// <param name="blockEnd">Block with lowest RPO number.</param>
		/// <param name="loopBlocks">Members of loop.</param>
		private void BuildAbsynIfs(BitSet loopBlocks)
		{
			proc.Dump(true, false);

			for (int i = proc.RpoBlocks.Count - 1; i >= 0; --i)
			{
				if (!loopBlocks[i])
					continue;

				Block block = proc.RpoBlocks[i];
				Branch branch = GetBranch(block);
				if (branch == null)
					continue;
				
				Block blockElse = block.ElseBlock;
				Block blockThen = block.ThenBlock;
				Block follow = null;

				// If-Then pattern

				if (blockThen == follow)
				{
					Block.RemoveEdge(block, blockElse);
					Block.RemoveEdge(blockElse, follow);
					block.Statements.Last.Instruction = new AbsynIf(branch.Condition.Invert(), ConvertBlock(blockElse), null);
					blockElse.Clear();
				} 
				else if (blockElse == follow)
				{
					Block.RemoveEdge(block, blockThen);
					Block.RemoveEdge(blockThen, follow);
					block.Statements.Last.Instruction = new AbsynIf(branch.Condition, ConvertBlock(blockThen), null);
					blockThen.Clear();
				}
				else if (blockElse.Succ.Count == 1 && blockThen.Succ.Count == 1)
				{
					// If-Then-Else pattern

					if (blockElse.Succ[0] != blockThen.Succ[0])
						throw new NotImplementedException("NYI");

					Debug.Assert(follow == blockElse.Succ[0]);

					// We have if-then-else. Remove the nodes from the graph.

					Block.RemoveEdge(block, blockElse);
					Block.RemoveEdge(block, blockThen);
					Debug.Assert(block.Succ.Count == 0);
					Block.RemoveEdge(blockElse, follow);
					Block.RemoveEdge(blockThen, follow);
					block.Statements.Last.Instruction = new AbsynIf(branch.Condition, ConvertBlock(blockThen), ConvertBlock(blockElse));
					Block.AddEdge(block, follow);

					blockThen.Clear();
					blockElse.Clear();
				}
				Debug.WriteLineIf(trace.TraceVerbose, "-------------");
				proc.Dump(trace.TraceVerbose, false);
			}
		}

		/// <summary>
		/// Converts a block into a an abstract syntax statement.
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		public AbsynStatement ConvertBlock(Block block)
		{
			AbsynStatementList stms = new AbsynStatementList();
			Branch branch = GetBranch(block);
			if (block.Statements.Count == 1 && branch == null)
				return ConvertInstruction(block.Statements[0].Instruction);
			else
			{
				AbsynStatementList list = new AbsynStatementList();
				foreach (Statement stm in block.Statements)
				{
					if (stm.Instruction is Branch)
						break;
					list.Add(ConvertInstruction(stm.Instruction));
				}
				return new AbsynCompoundStatement(list);
			}
		}


		private void RewriteIfThen(Block block, Expression cond, Block t, Block follow)
		{
			Block.RemoveEdge(block, t);
			Block.RemoveEdge(t, follow);
			block.Statements.Last.Instruction = new AbsynIf(cond, ConvertBlock(t), null);
			t.Clear();
		}

		private void RewriteIfThenElse(Block block, Expression cond, Block t, Block e, Block follow)
		{
			Block.RemoveEdge(block, t);
			Block.RemoveEdge(block, e);
			Debug.Assert(block.Succ.Count == 0);
			Block.RemoveEdge(t, follow);
			Block.RemoveEdge(e, follow);
			block.Statements.Last.Instruction = new AbsynIf(cond, ConvertBlock(t), ConvertBlock(e));
			Block.AddEdge(block, follow);

			t.Clear();
			e.Clear();
		}


		/// <summary>
		/// Finds all if-statements in a set of blocks.
		/// </summary>
		/// <remarks>
		/// We are guaranteed that the set blocks contains blocks either inside a loop or inside a procedure with
		/// no loops. All branches must therefore be if statements. Any unstructured branches are left
		/// as is; a postprocessing pass will handle them.
		/// </remarks>
		/// <param name="blocks">Only blocks in this bitset are examined for if-ness.</param>
		private void FindIfs(BitSet blocks)
		{
			BitSet unresolved = proc.CreateBlocksBitset();

			for (int b = proc.RpoBlocks.Count-1; b >= 0; --b)
			{
				if (!blocks[b])
					continue;

				Block block = proc.RpoBlocks[b];
				Branch branch = GetBranch(block);
				if (branch == null)
					continue;

				Debug.Assert(block.Succ.Count == 2);

				Block t = block.ThenBlock;
				Block e = block.ElseBlock;
				if (t.Succ.Count == 1 && e.Succ.Count == 1 && t.Succ[0] == e.Succ[0])
				{
					RewriteIfThenElse(block, branch.Condition, t, e, t.Succ[0]);
				}
				else if (t.Succ.Count == 1 && t.Succ[0] == e)
				{
					RewriteIfThen(block, branch.Condition, t, e);
				}
				else if (e.Succ.Count == 1 && e.Succ[0] == t)
				{
					RewriteIfThen(block, branch.Condition.Invert(), e, t);
				}
#if CIFUENTES
				// The following code fails if we have an if structure at the end of a loop.

				// Find all nodes that have the IF block as their immediate dominator.
				// They are the follow blocks of the if statement. If there are many follow nodes,
				// find the one with the highest number of in edges.

				Block blockFollow = null;
				int cMaxInEdges = 0;
				for (int s = b+1; s < proc.RpoBlocks.Count; ++s)
				{
					Block blockSucc = proc.RpoBlocks[s];
					if (domGraph.ImmediateDominator(blockSucc) != block)
						continue;

					if (blockSucc.Pred.Count >= cMaxInEdges)
					{
						blockFollow = blockSucc;
						cMaxInEdges = blockSucc.Pred.Count;
					}
				}
		
				if (blockFollow != null && cMaxInEdges >= 2)
				{
					blockInfos[block.RpoNumber].blockFollow = blockFollow;
					foreach (int i in unresolved)
					{
						blockInfos[i].blockFollow = blockFollow;
					}
					unresolved.SetAll(false);
					Debug.WriteLineIf(trace.TraceVerbose, string.Format("if statement at block {0}, follow block {1}", block.RpoNumber, blockFollow.RpoNumber));
				}
				else
				{
					unresolved[block.RpoNumber] = true;
				}
#endif
			}
		}

		private AbsynLabel BlockLabel(Block block)
		{
			if (block.Statements.Count != 0)
			{
				return block.Statements[0].Instruction as AbsynLabel;
			}
			return null;
		}

		public void ConvertBranch(Block block, Branch branch)
		{
			if (LeavesBlockSet(block.ThenBlock, blocks))
			{
				if (block.ThenBlock.Pred.Count == 1)
				{
					if (block.ThenBlock == this.continueTarget)
					{
						block.Statements.Last.Instruction = new AbsynIf(branch.Condition, new AbsynContinue());
					}
					else if (block.ThenBlock == this.breakTarget)
					{
						block.Statements.Last.Instruction = new AbsynIf(branch.Condition, new AbsynBreak());
					}
					else
					{
						InsertLabel(block.ThenBlock);
						block.Statements.Last.Instruction = 
							new AbsynIf(branch.Condition, new AbsynGoto(BlockLabel(block.ThenBlock).Name));
					}
				}
			}

		}

		private void ConvertBranches(BitSet blocks)
		{
			for (int i = 0; i < proc.RpoBlocks.Count; ++i)
			{
				if (!blocks[i])
					continue;
				Block block = proc.RpoBlocks[i];
				Branch branch = GetBranch(block);

				if (branch != null)
				{
					ConvertBranch(block, branch);
				}
			}
		}

		private void InsertLabel(Block block)
		{
			if (BlockLabel(block) == null)
			{
				block.Statements.Insert(0, new AbsynLabel(string.Format("l{0:4}", block.RpoNumber)));
			}
		}

		public bool LeavesBlockSet(Block block, BitSet blockSet)
		{
			return !blockSet[block.RpoNumber];
		}

		/// <summary>
		/// Makes one statement out of the blocks in the bitset.
		/// </summary>
		/// If there are any remaining branches, they are resolved as either 'break's, 'continue's,
		/// 'return's or 'goto's.
		/// <param name="blocks"></param>
		private void Linearize(BitSet blocks)
		{
			ConvertBranches(blocks);
			AbsynStatementList stms = new AbsynStatementList();
			for (int i = 0; i < proc.RpoBlocks.Count; ++i)
			{
				if (!blocks[i])
					continue;
				Block block = proc.RpoBlocks[i];
				foreach (Statement stm in block.Statements)
				{
					Branch br = stm.Instruction as Branch;
					if (br != null)
					{
						// Unconverted branch.
						throw new NotImplementedException("NYI");
					}
					else
					{
						stms.Add(ConvertInstruction(stm.Instruction));
					}
				}
				if (block != head)
					block.Clear();
				else
                    block.Statements.Clear();
			}
			if (stms.Count == 0)
				throw new NotImplementedException("NYI");
			if (stms.Count == 1)
				linearized = stms[0];
			else 
				linearized = new AbsynCompoundStatement(stms);
		}

		public AbsynStatement LinearizedStatement
		{
			get { return linearized; }
		}

		public override void Transform()
		{
			DumpBlockSetIf(trace.TraceVerbose, "Linearizing", this.blocks);
			FindIfs(this.blocks);
			Linearize(this.blocks);
		}
	}
}

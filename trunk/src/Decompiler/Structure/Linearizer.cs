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
using Decompiler.Core.Operators;
using System;
using System.Collections;
using System.Diagnostics;

namespace Decompiler.Structure
{
	/// <summary>
	/// Linearizes a (sub-)graph of blocks into structured if-then-elses or 
	/// unstructured if-gotos. 
	/// </summary>
	public class Linearizer
	{
		private Procedure proc;
		private BlockLinearizer blin;
		private Block loopHeader;
		private Block loopFollow; 
		private Block procedureExit;

		public Linearizer(Procedure proc, BlockLinearizer blin)
		{
			Init(proc, blin);
		}

		public BlockList Blocks
		{
			get { return proc.RpoBlocks; }
		}

		public AbsynIf BuildIfStatement(Expression cond, AbsynStatementList stmsThen, AbsynStatementList stmsElse)
		{
			AbsynStatement stmThen = null;
			AbsynStatement stmElse = null;
			if (stmsThen == null || stmsThen.Count == 0)
			{
				stmThen = stmsElse.MakeAbsynStatement();
				cond = cond.Invert();
			}
			else if (stmsElse == null || stmsElse.Count == 0)
			{
				stmThen = stmsThen.MakeAbsynStatement();
			}
			else
			{
				if (ShouldSwap(cond, stmsThen, stmsElse))
				{
					cond = cond.Invert();
					stmThen = stmsElse.MakeAbsynStatement();
					stmElse = stmsThen.MakeAbsynStatement();
				}
				else
				{
					stmThen = stmsThen.MakeAbsynStatement();
					stmElse = stmsElse.MakeAbsynStatement();
				}
			}
			return new AbsynIf(cond, stmThen, stmElse);
		}

		/// <summary>
		/// Finds all structured if-statements in a set of blocks and replaces them with if-statements.
		/// </summary>
		/// <remarks>
		/// We are guaranteed that the set blocks contains blocks either inside a loop or inside a procedure with
		/// no loops. All branches must therefore be if statements. Any unstructured branches are left
		/// as is; a postprocessing pass will handle them.
		/// </remarks>
		/// <param name="blockSet">Only blocks in this bitset are examined for if-ness.</param>
		public void BuildIfStatements(BitSet blockSet)
		{
			AbsynStatementList stms = new AbsynStatementList();
			foreach (int b in blockSet.Reverse)
			{
				Block block = Blocks[b];
				Branch branch = GetBranch(block);
				if (branch != null)
				{
					Block th = block.ThenBlock;
					Block el = block.ElseBlock;
					StraightPathWalker t = new StraightPathWalker(th);
					StraightPathWalker e = new StraightPathWalker(el);
					Block join = FindBranchJoin(t, e, blockSet, null);
					if (join != null)
					{
						Block.RemoveEdge(block, th);
						Block.RemoveEdge(block, el);
						AbsynStatementList stmsThen = LinearizeStraightPath(th, join);
						AbsynStatementList stmsElse = LinearizeStraightPath(el, join);
						block.Statements.Last.Instruction = 
							BuildIfStatement(branch.Condition, stmsThen, stmsElse);
						Block.AddEdge(block, join);
					}
					else 
					{
						Block p = PreferredUnstructuredExit(t.Current, e.Current);
						if (p == t.Current)
						{
							Block.RemoveEdge(block, th);
							AbsynStatementList stmsThen = LinearizeStraightPath(th, t.Current);
							stmsThen.Add(blin.MakeGoto(t.Current));
							block.Statements.Last.Instruction = 
								BuildIfStatement(branch.Condition, stmsThen, null);
						}
						else if (p == e.Current)
						{
							Block.RemoveEdge(block, el);
							AbsynStatementList stmsElse = LinearizeStraightPath(el, e.Current);
							stmsElse.Add(blin.MakeGoto(e.Current));
							block.Statements.Last.Instruction =
								BuildIfStatement(branch.Condition.Invert(), stmsElse, null);
						}						
						else
						{
							Block.RemoveEdge(block, th);
							Block.RemoveEdge(block, el);
							AbsynStatementList stmsThen = LinearizeStraightPath(th, join);
							AbsynStatementList stmsElse = LinearizeStraightPath(el, join);
							block.Statements.Last.Instruction = 
								BuildIfStatement(branch.Condition, stmsThen, stmsElse);

						}
					}
				}
				else
				{
					if (block != proc.EntryBlock && block.Succ.Count == 1 && block.Statements.Count == 0)
					{
						Block next = block.Succ[0];
						Block.ReplaceJumpsTo(block, next);
						block.Succ.Clear();
					}
				}
			}
		}
		
		public AbsynStatementList BuildStatementList(BitSet region, bool fConvertInEdges)
		{
			// At this point, there are no more branches left in the block set.
			// So, we concatenate the blocks in the region, starting with the first block 
			// in RP order.

			AbsynStatementList stms = new AbsynStatementList();
			foreach (int b in region)
			{
				Block block = Blocks[b];
				BlockLinearizer blin = new BlockLinearizer(loopFollow);
				while (block != null && region[block.RpoNumber])
				{
					Block next = (block.Succ.Count == 1) ? block.Succ[0] : null;
					blin.ConvertBlock(block, fConvertInEdges, stms);
					fConvertInEdges = true;
					if (next != null)
						Block.RemoveEdge(block, next);
					block.Statements.Clear();
					region[b] = false;
					block = next;
				} 
			}
			return stms;
		}

		/// <summary>
		/// Given a block with a branch, finds the block where paths from both branches join. The
		/// procedure gives up, returning null, if following both branches results in either leaving
		/// the set of blocks or reaching a node with a fork.
		/// </summary>
		/// <remarks>
		/// The paths that are followed from the 'then' and 'else' branch must be single-successor.
		/// If at any point another branch is found, the method returns null. 
		/// </remarks>
		/// <param name="branch">Block whose two branches are followed</param>
		/// <param name="blockSet">Branch paths must be in this set.</param>
		/// <returns></returns>
		public Block FindBranchJoin(StraightPathWalker t, StraightPathWalker e, BitSet blockSet, Block follow)
		{
			bool tBlocked = false;
			bool eBlocked = false;
			for (;;)
			{
				if (t.Current == e.Current)
					return t.Current;
				tBlocked = t.IsBlocked(blockSet) || t.Current == loopHeader;
				eBlocked = e.IsBlocked(blockSet) || e.Current == loopHeader;
				if (tBlocked && eBlocked)
					return null;
				if (!tBlocked && (eBlocked || t.Current.RpoNumber < e.Current.RpoNumber))
				{
					t.Advance();
				}
				else if (!eBlocked && (tBlocked || t.Current.RpoNumber > e.Current.RpoNumber))
				{
					e.Advance();
				}
			}
		}

		protected Branch GetBranch(Block block)
		{
			if (block.Statements.Count == 0)
				return null;
			return block.Statements.Last.Instruction as Branch;
		}

		private void Init(Procedure proc, BlockLinearizer blin)
		{
			this.proc = proc;
			this.blin = blin;
		}

		public bool IsInBlocks(Block block, BitSet blockSet)
		{
			return blockSet[block.RpoNumber];
		}

		public AbsynStatementList Linearize(BitSet region, bool fConvertInEdges)
		{
			BuildIfStatements(region);

			// At this point, there are no more branches left in the block set.
			// So, we concatenate the blocks in the region, starting with the first block 
			// in RP order.

			return BuildStatementList(region, fConvertInEdges);
		}
		
		public AbsynStatementList LinearizeStraightPath(Block from, Block to)
		{
			AbsynStatementList stms = new AbsynStatementList();
			while (from != to)
			{
				blin.ConvertBlock(from, stms);	
				from.Statements.Clear();
				if (from.Succ.Count == 0)
					break;
				Block next = from.Succ[0];
				Block.RemoveEdge(from, next);
				from = next;
			}
			return stms;
		}

		public Block LoopHeader
		{
			get { return loopHeader; }
			set { loopHeader = value; }
		}

		public Block LoopFollow
		{
			get { return loopFollow; }
			set { loopFollow = value; }
		}


		public Block PreferredUnstructuredExit(Block a, Block b)
		{
			if (a == LoopHeader)
				return b;
			if (b == LoopHeader)
				return a;

			if (a == LoopFollow)
				return a;
			if (b == LoopFollow)
				return b;

			if (a == ProcedureExit)
				return a;
			if (b == ProcedureExit)
				return b;
			
			return null;
		}

		public Block ProcedureExit
		{
			get { return procedureExit; }
			set { procedureExit = value; }
		}

		public bool ShouldSwap(Expression cond, AbsynStatementList stmsThen, AbsynStatementList stmsElse)
		{
			BinaryExpression b = cond as BinaryExpression;
			if (b == null)
				return false;

			if (b.op != Operator.ne)
				return false;

			if (stmsThen.Count < 1)
				return false;

			AbsynIf stmThen = stmsThen[0] as AbsynIf;
			if (stmThen == null)
				return false;

			BinaryExpression bThen = stmThen.Condition as BinaryExpression;
			if (bThen == null)
				return false;

			if (b.Left != bThen.Left)
				return false;

			if (bThen.op == Operator.ne)
			{
				SwapIf(stmThen);
			}
			return true;
		}

		//$REVIEW: need a pass that finds 'ifthenelseifs' and makes them into switch statements.
		public void SwapIf(AbsynIf stm)
		{
			AbsynStatement t = stm.Then;
			stm.Then = stm.Else;
			stm.Else = t;
			stm.Condition = stm.Condition.Invert();
		}
	}
}

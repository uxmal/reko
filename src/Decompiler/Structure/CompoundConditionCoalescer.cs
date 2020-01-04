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
using Reko.Core.Lib;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Structure
{
	/// <summary>
	/// Given a procedure, coalesces any compound conditions found.
	/// </summary>
	/// <remarks>
	/// A compound condition is expressed in C as
	/// <code>a && b</code> or <code>a || b</code>
	/// </remarks>
	public class CompoundConditionCoalescer
	{
		private Procedure proc;
        private ExpressionEmitter m;

		public CompoundConditionCoalescer(Procedure proc)
		{
			this.proc = proc;
            this.m = new ExpressionEmitter();
		}

		public void Transform()
		{
			bool fChanged;
			do
			{
				fChanged = false;
				foreach (var block in new DfsIterator<Block>(proc.ControlGraph).PostOrder())
				{
					if (block.Statements.Count > 0 && 
						block.Statements.Last.Instruction is Branch)
					{
						fChanged |= MaybeCoalesce(block);	
					}
				}
			} while (fChanged);
		}

		private void BuildCompoundCondition(
			Block blockFirst,
			Block blockSecond,
			Func<Expression,Expression,Expression> op, 
			bool fInvertFirst, 
			bool fInvertSecond)
		{
			Branch brFirst = (Branch) blockFirst.Statements.Last.Instruction;
			Branch brSecond = (Branch) blockSecond.Statements.Last.Instruction;
			if (fInvertFirst)
			{
				brFirst.Condition = brFirst.Condition.Invert();
			}
			if (fInvertSecond)
			{
				brSecond.Condition = brSecond.Condition.Invert();
			}
			brFirst.Condition = op(brFirst.Condition, brSecond.Condition);
		}

		/// <summary>
		/// Attempt to coalesce a block ending with a branch with a successor consisting of a single
		/// branch instruction.
		/// </summary>
		/// <param name="block"></param>
		private bool MaybeCoalesce(Block block)
		{
			bool fChanged = false;
			Block blockThen = block.ThenBlock;
			Block blockElse = block.ElseBlock;

			if (IsBlockSingleBranch(blockThen))
			{
				// if (x) then T:
				// E:
				// if (y) then T:    if (x && y) T
				// goto E         => E:
				if (blockThen.ElseBlock == blockElse)
				{
					BuildCompoundCondition(
						block,
						blockThen,
						m.Cand, false, false);
					RebuildCompoundGraph(block, blockThen, blockThen.ThenBlock, blockElse);
					fChanged = true;
				}	
				else if (blockThen.ThenBlock == blockElse)
				{
					// if (x) then T
					// E:
					// ...
					// 
					// T:                 if (x && !y) then Z
					//   if (y) then E => E:
					// Z:
					BuildCompoundCondition(
						block,
						blockThen,
						m.Cand, false, true);
					RebuildCompoundGraph(block, blockThen, blockThen.ElseBlock, blockElse);
					fChanged = true;
				}
			}
			else if (IsBlockSingleBranch(blockElse))
			{
				if (blockElse.ThenBlock == blockThen)
				{
					// if (x) then T
					// if (y) then T ==> if (x || y) then T

					BuildCompoundCondition(
						block,
						blockElse,
						m.Cor, false, false);
					RebuildCompoundGraph(block, blockElse, blockElse.ElseBlock, blockThen);
					fChanged = true;
				}
				else if (blockElse.ElseBlock == blockThen)
				{
					// if (x) then T      if (x) then T
					// if (y) then U      if (!y) then T      if (x || !y) then T
					// T:             => U:              => U:

					BuildCompoundCondition(
						block,
						blockElse,
						m.Cor, false, true);
					RebuildCompoundGraph(block, blockElse, blockElse.ThenBlock, blockThen);
					fChanged = true;
				}
			}
			return fChanged;
		}

		/// <summary>
		/// Returns true iff the block has a single instruction, a branch, and has a single predecessor.
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		private bool IsBlockSingleBranch(Block block)
		{
			if (block.Statements.Count != 1)
				return false;
			if (block.Pred.Count != 1)
				return false;
			return (block.Statements.Last.Instruction is Branch);
		}

		/// <summary>
		/// Removes the now redundant block from the procedure graph.
		/// </summary>
		/// <param name="block"></param>
		/// <param name="blockOld"></param>
		/// <param name="blockNew"></param>
		/// <param name="fTrueBranch"></param>
		private void RebuildCompoundGraph(
			Block block, 
			Block blockRedundant, 
			Block blockChain,
			Block blockCommon)
		{
			ReplaceBlock(block.Succ, blockRedundant, blockChain);
			blockRedundant.Pred.Remove(block);

			ReplaceBlock(blockChain.Pred, blockRedundant, block);
			blockRedundant.Succ.Remove(blockChain);

			proc.ControlGraph.RemoveEdge(blockRedundant, blockCommon);

			blockRedundant.Statements.Clear();
		}


		private void ReplaceBlock(List<Block> blocks, Block old, Block gnu)
		{
			for (int i = 0; i < blocks.Count; ++i)
				if (blocks[i] == old)
					blocks[i] = gnu;
		}
	}
}

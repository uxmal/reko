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
using System.Collections;
using System.Diagnostics;

namespace Decompiler.Structure
{
	/// <summary>
	/// Resolves the loop structure of an interval into its respective loop type (while, do/while)
	/// </summary>
	public class LoopFinder : StructureTransform
	{
		private Procedure proc;
		private DominatorGraph domGraph;

		public LoopFinder(Procedure proc, DominatorGraph dom) : base(proc)
		{
			this.proc = proc;
			this.domGraph = dom;
		}

		public void AbsorbExitingBranches(Loop loop)
		{
			bool changed;
			do
			{
				changed = false;
				foreach (int b in loop.Blocks)
				{
					if (b == loop.HeaderBlock.RpoNumber)
						continue;

					foreach (Block s in proc.RpoBlocks[b].Succ)
					{
						if (s != loop.FollowBlock && !loop.Blocks[s.RpoNumber] && AllPredecessorsInLoop(s, loop.Blocks))
						{
							loop.Blocks[s.RpoNumber] = true;
							changed = true;
						}
					}
				}
			} while (changed);
		}

		private bool AllPredecessorsInLoop(Block block, BitSet loopBlocks)
		{
			foreach (Block p in block.Pred)
			{
				if (!loopBlocks[p.RpoNumber])
					return false;
			}
			return true;
		}

		/// <summary>
		/// Appends the statements of the tail block to the head block.
		/// </summary>
		/// <param name="head">Block into which statements are appended</param>
		/// <param name="tail">Block from which statements are copied</param>
		private void AppendStatements(Block head, Block tail)
		{
			foreach (Statement stm in tail.Statements)
			{
				head.Statements.Add(stm.Instruction);
			}
		}

		private void InsertStatements(Block into, int iAt, Block from)
		{
			foreach (Statement stm in from.Statements)
			{
				into.Statements.Insert(iAt, stm);
				++iAt;
			}
		}

		/// <summary>
		/// Classifies and builds an abstract syntax loop.
		/// </summary>
		/// We want to know whether a loop is a  while loop, a repeat loop, or an infinite loop (with
		/// possible breaks out of the loop).
		/// <param name="loop">Previously identified loop region</param>
		public void BuildLoop(Loop loop)
		{
			Branch headBranch = GetBranch(loop.HeaderBlock);
			Branch endBranch = GetBranch(loop.EndBlock);

			if (endBranch != null)
			{
				if (headBranch != null)
				{
					if (loop.HeaderBlock == loop.EndBlock && loop.HeaderBlock.Statements.Count > 1 || 
						(IsMemberOf(loop.HeaderBlock.ThenBlock, loop.Blocks) &&	IsMemberOf(loop.HeaderBlock.ElseBlock, loop.Blocks)))
					{
						// The block jumps to itself, or the header has jumps
						// that both go into the loop.

						BuildRepeat(loop);
					}
					else
					{
						BuildWhile(loop);
					}
				}
				else
				{
					BuildRepeat(loop);
				}
			}
			else
			{
				if (headBranch != null)
				{
					BuildWhile(loop);
				}
				else
				{
					throw new NotImplementedException("Infinite loop not implemented yet");
				}
			}
		}

		private void BuildRepeat(Loop loop)
		{
			Block end = loop.EndBlock;
			Block head = loop.HeaderBlock;
			Branch br = GetBranch(loop.EndBlock);
			Block follow;
			if (IsMemberOf(end.ElseBlock, loop.Blocks))
			{
				br.Condition = br.Condition.Invert();
				follow = end.ThenBlock;
			}
			else
			{
				Debug.Assert(IsMemberOf(end.ThenBlock, loop.Blocks));
				follow = end.ElseBlock;
			}
			Debug.WriteLineIf(trace.TraceVerbose, string.Format("Building do/while loop, head {0}, end {1}, follow {2}", head.RpoNumber, end.RpoNumber, follow.RpoNumber));

			// Remove test and back edge to body.

			end.Statements.RemoveAt(end.Statements.Count - 1);
			Block.RemoveEdge(end, follow);

			Linearizer lin = new Linearizer(proc, new BlockLinearizer(follow));
			lin.LoopHeader = head;
			lin.LoopFollow = follow;
			head.Statements.Add(new AbsynDoWhile(lin.Linearize(loop.Blocks, false).MakeAbsynStatement(), br.Condition));
			Block.AddEdge(head, follow);
		}

		private void BuildWhile(Loop loop)
		{
			Block head = loop.HeaderBlock;
			Block end = loop.EndBlock;

			// Locate the follow node. Follow node is the node that follows 
			// the loop, i.e. the branch of the head that isn't a member of 
			// the loop.

			Branch br = GetBranch(head);
			Block follow;
			Block body;
			if (IsMemberOf(head.ThenBlock, loop.Blocks))
			{
				follow = head.ElseBlock;
				body = head.ThenBlock;
			}
			else
			{
				follow = head.ThenBlock;
				body = head.ElseBlock;
				br.Condition = br.Condition.Invert();
			}
			Debug.WriteLineIf(trace.TraceVerbose, string.Format("Building while loop, head {0}, end {1}, follow {2}", head.RpoNumber, loop.EndBlock.RpoNumber, follow.RpoNumber));

			// Remove the branch of the while loop, and the edge to the body.

			head.Statements.RemoveAt(head.Statements.Count - 1);
			Block.RemoveEdge(head, body);

			// If after remving the branch of the while loop, we stil lhave
			// statements in the loop header, we  have:
			// head:
			//     headcode()
			//     <branch follow>	; removed
			// body:
			//     bodycode()
			//     jmp head
			// follow:
			//
			// This needs to change to the following:
			//     while (1) {
			//	      headcode();
			//	      if (...) break;
			//  	  body() 
			//     }
			
			if (head.Statements.Count > 0)
			{
				head.Statements.Add(new AbsynIf(br.Condition.Invert(), new AbsynBreak()));
				InsertStatements(body, 0, head);
				br.Condition = Constant.True();
			}
			head.Statements.Clear();

			// Linearize the loop body.

			loop.Blocks[head.RpoNumber] = false;			// Exclude the loop header (it's empty anyway).
			Linearizer lin = new Linearizer(proc, new BlockLinearizer(follow));
			lin.LoopHeader = head;
			lin.LoopFollow = follow;
			AbsynWhile wh = new AbsynWhile(br.Condition, lin.Linearize(loop.Blocks, true).MakeAbsynStatement());
			head.Statements.Add(wh);
		}

		/// <summary>
		/// Returns a list of the blocks that are sources of back edges
		/// to the specified block.
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		public BlockList BackEdges(Block block)
		{
			BlockList blox = new BlockList();
			foreach (Block p in block.Pred)
			{
				if (IsBackEdge(p, block))
					blox.Add(p);
			}
			return blox;
		}

		private ArrayList CopyStatements(StatementList stms)
		{
			ArrayList s = new ArrayList(stms);
			return s;
		}

		public Loop Create(Block head, Block end, BitSet loopBlocks)
		{
			Branch headBranch = GetBranch(head);
			Branch endBranch = GetBranch(end);

			if (endBranch != null)
			{
				if (headBranch != null)
				{
					if (head == end && head.Statements.Count > 1 || 
						(loopBlocks[head.ThenBlock.RpoNumber] && loopBlocks[head.ElseBlock.RpoNumber]))
					{
						// The block jumps to itself, or the header has jumps
						// that both go into the loop.

						return new RepeatLoop(head, end, loopBlocks);
					}
					else
					{
						return new WhileLoop(head, end, loopBlocks);
					}
				}
				else
				{
					return new RepeatLoop(head, end, loopBlocks);
				}
			}
			else
			{
				if (headBranch != null)
				{
					return new WhileLoop(head, end, loopBlocks);
				}
				else
				{
					throw new NotImplementedException("Infinite loop not implemented yet");
				}
			}
		}


		/// <summary>
		/// Determines the blocks that are members of a loop.
		/// </summary>
		public void FindBlocksInLoop(Block head, BitSet loopBlocks, BitSet interval)
		{
			loopBlocks[head.RpoNumber] = true;		// Header is of course part of the loop.

			// The blocks in the loop are each predecessor to the header p, where p is dominated by the header.

			BitSet visited = proc.CreateBlocksBitset();
			WorkList wl = new WorkList();
			wl.Add(head);
			while (!wl.IsEmpty)
			{
				Block b = (Block) wl.GetWorkItem();
				if (visited[b.RpoNumber])
					continue;

				visited[b.RpoNumber] = true;
				foreach (Block p in b.Pred)
				{
					if (IsMemberOf(p, interval) && 
						domGraph.DominatesStrictly(head.RpoNumber, p.RpoNumber))
					{
						loopBlocks[p.RpoNumber] = true;
						wl.Add(p);
					}
				}
			}
		}

		/// <summary>
		/// Determines whether there is a loop, and if so, locates its end.
		/// </summary>
		/// The end of a loop is the back edge to the block <paramref name="head"/> whose 
		/// start node has the highest RPO number. 
		/// <param name="head">The loop header</param>
		/// <param name="intervalBlocks"></param>
		/// <returns>The loop end block, or null if there is no loop.</returns>

		public Block FindLoopEnd(Block head, BitSet intervalBlocks)
		{
			Block end = null;
			for (int p = 0; p < head.Pred.Count; ++p)
			{
				Block prev = head.Pred[p];
				if (IsBackEdge(prev, head) && IsMemberOf(prev, intervalBlocks))
				{
					// This is a back edge.
					if (end == null || prev.RpoNumber > end.RpoNumber)
					{
						end = prev;
					}
				}
			}
			return end;
		}

		private static bool IsBackEdge(Block pred, Block succ) 
		{
			return (pred.RpoNumber >= succ.RpoNumber); 
		}

		private bool IsMemberOf(Block b, BitSet s) 
		{
			return s[b.RpoNumber]; 
		}

		public Loop FindLoop(Interval interval)
		{
			DumpBlockSetIf(trace.TraceVerbose, "Finding loop in interval", interval.Blocks);
			Block blockHead = interval.Header;

			// If we found an end node, we have a loop.

			Block blockEndLoop = FindLoopEnd(blockHead, interval.Blocks);
			if (blockEndLoop != null)
			{
				Debug.WriteLineIf(trace.TraceVerbose, string.Format("Found loop, head: {0}, end: {1}", blockHead.RpoNumber, blockEndLoop.RpoNumber));
				BitSet loopBlocks = proc.CreateBlocksBitset();
				FindBlocksInLoop(blockHead, loopBlocks, interval.Blocks);
				Loop loop = Create(blockHead, blockEndLoop, loopBlocks);
				AbsorbExitingBranches(loop);
				DumpBlockSetIf(trace.TraceVerbose, "Blocks in loop:", loop.Blocks);
				return loop;
			}
			else
			{
				return null;
			}
		}
	}
}

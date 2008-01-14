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
using Decompiler.Core.Lib;
using System;
using System.Diagnostics;

namespace Decompiler.Structure
{
	public class CaseStatementRewriter
	{
		private Procedure proc;
		private DominatorGraph dom;

		public CaseStatementRewriter(Procedure proc)
		{
			this.proc = proc;
			this.dom = new DominatorGraph(proc);
		}

		public void TagNodesInCase(Block pBB, BitSet l, Block head, Block tail, BitSet visited)
			/* Recursive procedure to tag nodes that belong to the case described by
			 * the list l, head and tail (dfsLast index to first and exit node of the 
			 * case).                               */
		{
			visited[pBB.RpoNumber] = true;
			if (pBB != tail && !IsMultibranch(pBB) &&  l[dom.ImmediateDominator(pBB).RpoNumber])
			{
				l[pBB.RpoNumber] = true;
				//$ pBB.caseHead = head;
				foreach (Block i in pBB.Succ)
				{
					if (!visited[i.RpoNumber])
						TagNodesInCase(i, l, head, tail, visited);
				}
			}
		}

		public void FindCases(Procedure proc)
			/* Structures case statements.  This procedure is invoked only when proc
			 * has a case node.                         */
		{

			DominatorGraph dom = new DominatorGraph(proc);
			for (int i = proc.RpoBlocks.Count - 1; i >= 0; i--)
			{
				if (IsMultibranch(proc.RpoBlocks[i]))
				{
					Block caseHeader = proc.RpoBlocks[i];
					Block exitNode = null;   	/* case exit node           */
					BitSet caseNodes = proc.CreateBlocksBitset();   /* list of nodes in case */

					/* Find descendant node which has as immediate predecessor 
					 * the current header node, and is not a successor.    */
					for (int jj = i + 2; jj < proc.RpoBlocks.Count; jj++)
					{
						Block j = proc.RpoBlocks[jj];
						if (!caseHeader.Succ.Contains(j) && dom.ImmediateDominator(j) == caseHeader)
						{
							if (exitNode == null || exitNode.Pred.Count < j.Pred.Count) 
								exitNode = j;
						}
					}

					//$ proc.RpoBlocks[i].caseTail = exitNode;
         
					/* Tag nodes that belong to the case by recording the
					 * header field with caseHeader.           */
					caseNodes[i] = true; 
					//$ proc.RpoBlocks[i].caseHead = i;
					foreach (Block j in caseHeader.Succ)
					{
						TagNodesInCase(
							j,
							caseNodes, 
							caseHeader,
							exitNode, 
							proc.CreateBlocksBitset());
					}
					if (exitNode != null)
					{
						//$ proc.RpoBlocks[exitNode].caseHead = i;
					}
				}
			}
		}

		/// <summary>
		/// Finds the exit node for a case statement.
		/// </summary>
		/// <param name="caseHeader"></param>
		/// <returns></returns>
		public Block FindExitNode(Block caseHeader)
		{
			Debug.Assert(IsMultibranch(caseHeader));
			Block exitNode = null;

			// Find descendant node which has as immediate predecessor 
			// the current header node, and is not a successor. 
			for (int j = caseHeader.RpoNumber + 1; j < proc.RpoBlocks.Count; j++)
			{
				Block b = proc.RpoBlocks[j];
				if (!caseHeader.Succ.Contains(b) && dom.ImmediateDominator(b) == caseHeader)
				{
					if (exitNode == null || exitNode.Pred.Count < b.Pred.Count) 
						exitNode = b;
				}
			}
			return exitNode;
		}

		public bool IsMultibranch(Block b)
		{
			if (b.Statements.Count == 0)
				return false;
			return b.Statements.Last.Instruction is SwitchInstruction;
		}
	}

}
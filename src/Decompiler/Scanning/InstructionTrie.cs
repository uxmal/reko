#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using System.Collections.Generic;
using Hashtable = System.Collections.Hashtable;
using IComparer = System.Collections.IComparer;
using IEqualityComparer = System.Collections.IEqualityComparer;

namespace Reko.Scanning
{
	/// <summary>
	/// An InstructionTrie tallies instruction frequencies and instruction sequence lengths.
	/// </summary>
	public class InstructionTrie<TInstr>
	{
		private int count;
		private TrieNode root;

		public InstructionTrie(IEqualityComparer<TInstr> hasher)
		{
			this.root = new TrieNode(hasher);
		}

		public int Count
		{
			get { return count; }
		}

		public void AddInstructions(TInstr [] instrs)
		{
			TrieNode node = root;
			foreach (TInstr instr in instrs)
			{
				node = node.Add(instr);
				++node.Tally;
				++count;
			}
		}

		public long ScoreInstructions(TInstr [] instrs)
		{
			TrieNode node = root;
			long score = 0;
			foreach (var instr in instrs)
			{
				TrieNode subNode = node.Next(instr);
				if (subNode == null)
					break;
				score = score * node.Successors.Count + subNode.Tally;
				node = subNode;
			}
			return score;
		}

		private class TrieNode
		{
			public TInstr Instruction;
			public Dictionary<TInstr, TrieNode> Successors;
			public IEqualityComparer<TInstr> hasher;
			public IComparer cmp;
			public int Tally;

			public TrieNode(IEqualityComparer<TInstr> hasher)
			{
				Init(hasher);
			}

			public TrieNode(TInstr instruction, IEqualityComparer<TInstr> hasher)
			{
				Instruction = instruction;
				Init(hasher);
			}

			public TrieNode Add(TInstr instr)
			{
				TrieNode subNode = Next(instr);
				if (subNode == null)
				{
					subNode = new TrieNode(instr, hasher);
					Successors.Add(instr, subNode);
				}
				return subNode;
			}

			private void Init(IEqualityComparer<TInstr> hasher)
			{
				this.hasher = hasher;
				Successors  = new Dictionary<TInstr,TrieNode>(hasher);
			}

			public TrieNode Next(TInstr instr)
			{
				return Successors[instr];
			}
		}
	}
}

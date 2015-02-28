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

using System;
using IComparer = System.Collections.IComparer;
using IEqualityComparer = System.Collections.IEqualityComparer;
using Hashtable = System.Collections.Hashtable;

namespace Decompiler.Scanning
{
	/// <summary>
	/// An InstructionTrie tallies instruction frequencies and instruction sequence lengths.
	/// </summary>
	public class InstructionTrie<TInstr>
	{
		private int count;
		private TrieNode root;
		public InstructionTrie(IEqualityComparer hasher, IComparer comparer)
		{
			this.root = new TrieNode(hasher, comparer);
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
			foreach (object instr in instrs)
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
			public Hashtable Successors;
			public IEqualityComparer hasher;
			public IComparer cmp;
			public int Tally;

			public TrieNode(IEqualityComparer hasher, IComparer cmp)
			{
				Init(hasher, cmp);
			}

			public TrieNode(TInstr instruction, IEqualityComparer hasher, IComparer cmp)
			{
				Instruction = instruction;
				Init(hasher, cmp);
			}

			public TrieNode Add(TInstr instr)
			{
				TrieNode subNode = Next(instr);
				if (subNode == null)
				{
					subNode = new TrieNode(instr, hasher, cmp);
					Successors.Add(instr, subNode);
				}
				return subNode;
			}

			private void Init(IEqualityComparer hasher, IComparer cmp)
			{
				this.hasher = hasher;
				this.cmp = cmp;
				Successors = new Hashtable(hasher);
			}

			public TrieNode Next(object instr)
			{
				return (TrieNode) Successors[instr];
			}
		}
	}
}

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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
	/// <summary>
	/// An InstructionTrie tallies instruction frequencies and instruction
    /// sequence lengths.
	/// </summary>
	public class Trie<T>
	{
		private TrieNode root;

		public Trie(IEqualityComparer<T> hasher)
		{
			this.root = new TrieNode(hasher);
		}

		public int Count { get; private set; }

		public void Add(T [] instrs)
		{
			TrieNode node = root;
			foreach (T instr in instrs)
			{
				node = node.Add(instr);
				++node.Tally;
				++Count;
			}
		}

		public long ScoreInstructions(T [] instrs)
		{
			TrieNode node = root;
			long score = 0;
			foreach (var instr in instrs)
			{
				TrieNode subNode;
                if (!node.Next(instr, out subNode))
					break;
				score = score * node.Successors.Count + subNode.Tally;
				node = subNode;
			}
			return score;
		}

		private class TrieNode
		{
			public T Instruction;
			public Dictionary<T, TrieNode> Successors;
			public IEqualityComparer<T> hasher;
			public int Tally;

			public TrieNode(IEqualityComparer<T> hasher) : this(default(T), hasher)
			{
			}

			public TrieNode(T instruction, IEqualityComparer<T> hasher)
			{
				Instruction = instruction;
				this.hasher = hasher;
				Successors  = new Dictionary<T,TrieNode>(hasher);
			}

			public TrieNode Add(T instr)
			{
				TrieNode subNode;
                if (!Successors.TryGetValue(instr, out subNode))
				{
					subNode = new TrieNode(instr, hasher);
					Successors.Add(instr, subNode);
				}
				return subNode;
			}

			public bool Next(T instr, out TrieNode node)
			{
				return Successors.TryGetValue(instr, out node);
			}
		}

        public void Dump()
        {
            Dump(this.root, 0);
        }

        private void Dump(TrieNode n, int depth)
        {
            return;     // Very verbose output slows down regression tests, comment out for debugging.
#if VERBOSE
            var sl = n.Successors
                .ToSortedList(
                    k => k.Key.ToString(),
                    k => k.Value);
            foreach (var de in sl)
            {
                Debug.Print(
                    "{0}({1}: {2})",
                    new string(' ', depth*4),
                    de.Key,
                    de.Value.Tally);
                Dump(de.Value, depth + 1);
            }
#endif
        }
    }
}

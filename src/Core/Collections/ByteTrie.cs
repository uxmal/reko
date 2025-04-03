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

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Collections
{
    /// <summary>
    /// This class implements a trie that matches input bytes. The implementation
    /// has a fan-out of 16, rather than 256. Matching will take a little longer,
    /// but the memory usage will be a lot less.
    /// </summary>
    /// <typeparam name="TValue">The trie associates a value with each pattern.</typeparam>
    public class ByteTrie<TValue>
    {
        private readonly HighNode[] root;

        public ByteTrie()
        {
            this.root = new HighNode[16];
        }

        /// <summary>
        /// Add a byte pattern with its associated <see href="TValue"/> to the
        /// trie.
        /// </summary>
        /// <param name="pattern">Pattern as a sequence of bytes.</param>
        /// <param name="value">Value to associate with the pattern.</param>
        public void Add(ReadOnlySpan<byte> pattern, TValue value)
            => Add(pattern, value, false);

        /// <summary>
        /// Add a byte pattern with its associated <see href="TValue"/> to the
        /// trie, optionally replacing any previously added pattern.
        /// </summary>
        /// <param name="pattern">Pattern as a sequence of bytes.</param>
        /// <param name="value">Value to associate with the pattern.</param>
        /// <param name="replace">If true, replaces an existing pattern's value
        /// with <paramref name="value"/>. If false, duplicate patterns with raise
        /// an <see cref="InvalidOperationException"/>.
        /// </param>
        public void Add(ReadOnlySpan<byte> pattern, TValue value, bool replace)
        {
            if (pattern.Length == 0)
                throw new ArgumentException("Patterns must be at least one byte long.", nameof(pattern));
            var hiNodes = root;
            for (int i = 0; i < pattern.Length; ++i)
            {
                byte b = pattern[i];
                var hiNode = EnsureHighNode(hiNodes, b >> 4);
                var loNode = EnsureLowNode(hiNode, b & 0xF);
                if (i == pattern.Length - 1)
                {
                    if (!replace && loNode.Value is not null)
                        throw new InvalidOperationException("The pattern already exists.");
                    loNode.Value = value;
                    return;
                }
                else
                {
                    loNode.High = new HighNode[16];
                }
                hiNodes = loNode.High!;
            }
        }

        /// <summary>
        /// Add a byte pattern with its associated <see href="TValue"/> to the
        /// trie, together with a <paramref name="mask"/> that allows for 
        /// "wildcards".
        /// </summary>
        /// <param name="pattern">Pattern as a sequence of bytes.</param>
        /// <param name="mask">Mask bytes.</param>
        /// <param name="value">Value to associate with the pattern.</param>
        public void Add(ReadOnlySpan<byte> pattern, ReadOnlySpan<byte> mask, TValue value)
        {
            if (pattern.Length == 0)
                throw new ArgumentException("Patterns must be at least one byte long.", nameof(pattern));
            if (pattern.Length != mask.Length)
                throw new ArgumentException("The pattern and the mask have different lengths.");

            void AddRecursive(HighNode[] hiNodes, ReadOnlySpan<byte> pattern, ReadOnlySpan<byte> mask, int i)
            {
                byte b = pattern[i];
                byte m = mask[i];
                for (int c = 0; c < 256; ++c)
                {
                    if ((c & m) != b)
                        continue;

                    var hiNode = EnsureHighNode(hiNodes, c >> 4);
                    var loNode = EnsureLowNode(hiNode, c & 0xF);
                    if (i == pattern.Length - 1)
                    {
                        loNode.Value = value;
                    }
                    else
                    {
                        loNode.High ??= new HighNode[16];
                        AddRecursive(loNode.High, pattern, mask, i + 1);
                    }
                }
            }
            AddRecursive(root, pattern, mask, 0);
        }

        private static LowNode EnsureLowNode(HighNode hiNode, int loNybble)
        {
            ByteTrie<TValue>.LowNode loNode = hiNode.Low[loNybble];
            if (loNode is null)
            {
                loNode = new LowNode();
                hiNode.Low[loNybble] = loNode;
            }
            return loNode;
        }

        private static HighNode EnsureHighNode(HighNode[] hiNodes, int hiNybble)
        {
            var hiNode = hiNodes[hiNybble];
            if (hiNode.Low is null)
            {
                hiNode = new HighNode { Low = new LowNode[16] };
                hiNodes[hiNybble] = hiNode;
            }
            return hiNode;
        }

        /// <summary>
        /// Tries to find a position in the given byte <paramref name="data"/>
        /// that matches one of the patterns in the trie. 
        /// </summary>
        /// <param name="data">Data in which to look for patterns.</param>
        /// <param name="index">Index in the data in which to start looking.</param>
        /// <returns>An instance of <see cref="ByteTrieMatch{T}"/>, describing
        /// whether a match was found or not.
        /// </returns>
        public ByteTrieMatch<TValue> Match(ReadOnlySpan<byte> data, int index = 0)
        {
            var m = ByteTrieMatch<TValue>.Empty;
            for (int i = index; i < data.Length; ++i)
            {
                m = MatchAt(data, i);
                if (m.Success)
                    return m;
            }
            return m;
        }

        private ByteTrieMatch<TValue> MatchAt(ReadOnlySpan<byte> pattern, int index)
        {
            var hiNodes = root;
            LowNode? loNode;
            LowNode? bestNode = null;
            int i;
            int bestIndex = index;
            for (i = index; hiNodes is not null && i < pattern.Length; ++i)
            {
                byte b = pattern[i];
                var hiNybble = b >> 4;
                var hiNode = hiNodes[hiNybble];
                if (hiNode.Low is null)
                    break;
                var loNybble = b & 0xF;
                loNode = hiNode.Low[loNybble];
                if (loNode is null)
                    break;
                if (loNode.Value is not null)
                {
                    bestNode = loNode;
                    bestIndex = i + 1;
                }
                hiNodes = loNode.High;
            }
            if (bestNode is not null)
            {
                return new ByteTrieMatch<TValue>(this, index, bestIndex - index, bestNode.Value);
            }
            else 
            {
                return ByteTrieMatch<TValue>.Empty;
            }
        }

        public void Dump()
        {
            static void Dump(HighNode[] hinodes, int depth)
            {
                for (int i = 0; i < hinodes.Length; ++i)
                {
                    var low = hinodes[i].Low;
                    if (low is null)
                        continue;
                    for (int j = 0; j < low.Length; ++j)
                    {
                        var ll = low[j];
                        if (ll is null)
                            continue;

                        Console.Write(new string(' ', depth * 2));
                        Console.Write("  {0:X2}", (i << 4) | j);
                        if (ll.Value is not null)
                        {
                            Console.Write(" {0}", low[j].Value);
                        }
                        Console.WriteLine();
                        if (ll.High is not null)
                        {
                            Dump(ll.High, depth + 1);
                        }
                    }
                }
            }
            Dump(root, 0);
        }

        // The trie nodes consist of two alternating layers. First the 'HighNode's,
        // corresponding to the high nybble of the data being matched. These refer
        // to 'LowNodes' which contain the stored values when a pattern is matched,
        // as well as 16 new nigh nodes corresponding to the high bytes of the next 
        // level.
        private struct HighNode
        {
            public LowNode[] Low;   // 16 low nybbles
        }

        private class LowNode
        {
            public TValue? Value;
            public HighNode[]? High;
        }
    }
}

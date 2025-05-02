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
    /// Represents a match from using a <see cref="ByteTrie{TValue}"/> on a
    /// span of bytes.
    /// </summary>
    /// <typeparam name="T">Value associated with the match.</typeparam>
    public class ByteTrieMatch<T>
    {
        /// <summary>
        /// An empty match, which indicates that no match was found.
        /// </summary>
        public static ByteTrieMatch<T> Empty { get; } = new ByteTrieMatch<T>(null, -1, -1, default)
        {
            Success = false,
        };

        private readonly ByteTrie<T>? trie;

        internal ByteTrieMatch(ByteTrie<T>? trie, int index, int length, T? value)
        {
            this.trie = trie;
            this.Index = index;
            this.Length = length;
            this.Value = value;
            this.Success = trie is not null;
        }

        /// <summary>
        /// True if a match was found.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Index at which the match was found.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The length of the match.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// The value associated with the match.
        /// </summary>
        public T? Value { get; private set; }

        /// <summary>
        /// Given a match, try to find the next match.
        /// </summary>
        /// <param name="bytes">Area in which to find the match.</param>
        /// <returns>A new instance of <see cref="ByteTrieMatch{T}"/>.
        /// </returns>
        public ByteTrieMatch<T> NextMatch(ReadOnlySpan<byte> bytes)
        {
            if (trie is null) // This can only be the Empty match.
                return this;
            return trie.Match(bytes, this.Index + 1);
        }
    }
}

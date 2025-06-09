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
using System.Collections;

namespace Reko.Core.Dfa
{
    /// <summary>
    /// Parses a regexp pattern into a tree of TreeNodes.
    /// </summary>
    public class PatternParser
    {
        private readonly string pattern;
        private int idx;
        private int significantNodes;

        /// <summary>
        /// Creates a new parser for the specified pattern.
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        public PatternParser(string pattern)
        {
            this.pattern = pattern;
            this.idx = 0;
        }

        internal TreeNode? Parse()
        {
            var head = ParseOr();
            if (PeekAndDiscard(']'))
            {
                Expect('[');
                var tail = ParseOr();
                head = new TreeNode
                {
                    Type = NodeType.Cut,
                    Left = head,
                    Right = tail
                };
            }
            return head;
        }

        private void Expect(char ch)
        {
            if (idx >= pattern.Length || pattern[idx] != ch)
                throw new FormatException(string.Format("Expected character '{0}' (U+{1:X4} at position {2}.", ch, (int) ch, idx));
            ++idx;
        }

        private TreeNode? ParseOr()
        {
            var head = ParseCat();
            while (PeekAndDiscard('|'))
            {
                var tail = ParseCat();
                head = new TreeNode
                {
                    Type = NodeType.Or,
                    Left = head,
                    Right = tail,
                };
            }
            return head;
        }

        private TreeNode? ParseCat()
        {
            var prev = ParseFactor();
            for (; ; )
            {
                var tail = ParseFactor();
                if (tail is null)
                    break;
                var node = new TreeNode
                {
                    Type = NodeType.Cat,
                    Left = prev,
                    Right = tail
                };
                prev = node;
            }
            return prev;
        }

        private TreeNode? ParseFactor()
        {
            var atom = ParseAtom();
            if (atom is null)
                return null;
            if (PeekAndDiscard('+'))
            {
                return new TreeNode
                {
                    Type = NodeType.Plus,
                    Left = atom
                };
            }
            else if (PeekAndDiscard('*'))
            {
                return new TreeNode
                {
                    Type = NodeType.Star,
                    Left = atom
                };
            }
            return atom;
        }

        private TreeNode? ParseAtom()
        {
            if (!EatSpaces())
                return null;
            if (PeekAndDiscard('('))
            {
                var head = Parse();
                EatSpaces();
                Expect(')');
                return head;
            }
            if (Peek('['))
            {
                return ParseCharClass();
            }
            int d = MaybeHexByte();
            if (d >= 0)
            {
                return new TreeNode
                {
                    Type = NodeType.Char,
                    Value = (byte)d,
                    Number = ++significantNodes,
                };
            }
            if (Peek('?'))
            {
                Expect('?');
                Expect('?');
                return new TreeNode
                {
                    Type = NodeType.Any,
                    Number = ++significantNodes
                };
            }
            return null;
        }

        private bool EatSpaces()
        {
            while (this.idx < pattern.Length)
            {
                int ch = pattern[idx];
                if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n')
                    return true;
                ++idx;
            }
            return false;
        }

        private TreeNode ParseCharClass()
        {
            var bytes = new BitArray(256);
            Expect('[');
            while (!PeekAndDiscard(']'))
            {
                int d = MaybeHexByte();
                bytes[d] = true;
                if (PeekAndDiscard('-'))
                {
                    int e = MaybeHexByte();
                    for (int b = d + 1; b < e; ++b)
                    {
                        bytes[d] = true;
                    }
                }
            }
            return new TreeNode
            {
                Type = NodeType.CharClass,
                ValueClass = bytes,
                Number = ++significantNodes,
            };
        }

        private int HexNibble()
        {
            if (idx < pattern.Length)
            {
                char ch = pattern[idx++];
                if ('0' <= ch && ch <= '9')
                    return ch - '0';
                if ('A' <= ch && ch <= 'F')
                    return ch - 'A' + 10;
                if ('a' <= ch && ch <= 'f')
                    return ch - 'a' + 10;
                --idx;
                return -1;
            }
            return -1;
        }

        private int MaybeHexByte()
        {
            int h = HexNibble();
            if (h < 0)
                return h;
            int l = HexNibble();
            if (l < 0)
                return l;
            return (h << 4) | l;
        }

        private bool Peek(char c)
        {
            if (this.idx >= pattern.Length)
                return false;
            return this.pattern[idx] == c;
        }

        private bool PeekAndDiscard(char c)
        {
            if (this.idx >= pattern.Length)
                return false;
            if (this.pattern[idx] == c)
            {
                ++idx;
                return true;
            }
            return false;
        }
    }
}

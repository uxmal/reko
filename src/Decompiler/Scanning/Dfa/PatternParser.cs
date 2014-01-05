#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Scanning.Dfa
{
    /// <summary>
    /// Parses a regexp pattern into a tree of TreeNodes.
    /// </summary>
    public class PatternParser
    {
        private int idx;
        private string pattern;
        private int significantNodes;

        public PatternParser(string pattern)
        {
            this.pattern = pattern;
            this.idx = 0;
        }

        public TreeNode Parse()
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

        private bool AtEof()
        {
            return idx >= pattern.Length;
        }

        private TreeNode ParseOr()
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

        private TreeNode ParseCat()
        {
            var prev = ParseFactor();
            for (; ; )
            {
                var tail = ParseFactor();
                if (tail == null)
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

        private TreeNode ParseFactor()
        {
            var atom = ParseAtom();
            if (atom == null)
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

        private TreeNode ParseAtom()
        {
            if (this.idx >= pattern.Length)
                return null;
            if (PeekAndDiscard('('))
            {
                var head = Parse();
                Expect(']');
                return head;
            }
            if (Peek('['))
            {
                return ParseCharClass();
            }
            int d = ExpectHexByte();
            if (d < 0)
                throw new FormatException("Expected hexadecimal encoded byte.");
            return new TreeNode
            {
                Type = NodeType.Char,
                Value = (byte) d,
                Number = ++significantNodes,
            };
        }

        private TreeNode ParseCharClass()
        {
            var bytes = new BitArray(256);
            Expect('[');
            while (!PeekAndDiscard(']'))
            {
                int d = ExpectHexByte();
                bytes[d] = true;
                if (PeekAndDiscard('-'))
                {
                    int e = ExpectHexByte();
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
            }
            throw new FormatException("Expected a hexadecimal digit. ");
        }

        private int ExpectHexByte()
        {
            int h = HexNibble();
            int l = HexNibble();
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

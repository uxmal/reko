#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using BitArray = System.Collections.BitArray;

namespace Decompiler.Scanning
{
    public class DfaBuilder
    {
        private TreeNode t;
        private HashSet<State> Dstates;
        private List<Tuple<int, int, int>> Dtran;

        public DfaBuilder(TreeNode node)
        {
           this.t = node;
        }

        private class State
        {
            public State(SortedSet<TreeNode> set, int number)
            {
                this.Number = number;
                this.Nodes = set;
                this.hash = Nodes.Aggregate(
                    0,
                    (h, n) => h ^ n.GetHashCode());
            }

            public SortedSet<TreeNode> Nodes;
            public int Number;
            private int hash;

            public override bool Equals(object obj)
            {
                var other = obj as State;
                if (other == null)
                    return false;
                if (Nodes.Count != other.Nodes.Count)
                    return false;
                var ea = Nodes.GetEnumerator();
                var eb = other.Nodes.GetEnumerator();
                while (ea.MoveNext() && eb.MoveNext())
                {
                    if (ea.Current != eb.Current)
                        return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                return hash;
            }
        }

        public void BuildAutomaton(TreeNode t)
        {
            var state = new State(
                new SortedSet<TreeNode>(t.FirstPos),
                0);
            Dstates = new HashSet<State> { state };
            Dtran = new List<Tuple<int,int,int>>();
            var unmarked = new Queue<State>();
            unmarked.Enqueue(state);
            while (unmarked.Count > 0)
            {
                State T = unmarked.Dequeue();
                foreach (var p in T.Nodes.SelectMany(p => p.GetTransitionCharacters(), (p, c) => new { Node = p, Value = c }))
                {
                    State U = new State(
                        new SortedSet<TreeNode>(p.Node.FollowPos),
                        Dstates.Count);
                    if (U.Nodes.Count() > 0
                        && !Dstates.Contains(U))
                    {
                        Dstates.Add(U);
                    }
                    Dtran.Add(Tuple.Create(T.Number, (int) p.Value, U.Number));
                }
            }
        }

        public void BuildNullable(TreeNode node)
        {
            if (node == null)
                return;
            switch (node.Type)
            {
            case TreeNode.Epsilon:
                node.Nullable = true;
                break;
            case TreeNode.Star:
                BuildNullable(node.Left);
                node.Nullable = true;
                break;
            case TreeNode.Plus:
                BuildNullable(node.Left);
                node.Nullable = node.Left.Nullable;
                break;
            case TreeNode.Or:
                BuildNullable(node.Left);
                BuildNullable(node.Right);
                node.Nullable = node.Left.Nullable | node.Right.Nullable;
                break;
            case TreeNode.Cat:
                BuildNullable(node.Left);
                BuildNullable(node.Right);
                node.Nullable = node.Left.Nullable & node.Right.Nullable;
                break;
            default: 
                node.Nullable = false;
                break;
            }
        }

        public void BuildFirstPos(TreeNode node)
        {
            if (node == null)
                return;
            switch (node.Type)
            {
            case TreeNode.Epsilon:
                node.FirstPos = new HashSet<TreeNode>();
                break;
            case TreeNode.Star:
            case TreeNode.Plus:
                BuildFirstPos(node.Left);
                node.FirstPos = new HashSet<TreeNode>(node.Left.FirstPos);
                break;
            case TreeNode.Or:
                BuildFirstPos(node.Left);
                BuildFirstPos(node.Right);
                var set = new HashSet<TreeNode>(node.Left.FirstPos);
                set.UnionWith(node.Left.FirstPos);
                node.FirstPos = set;
                break;
            case TreeNode.Cat:
                BuildFirstPos(node.Left);
                BuildFirstPos(node.Right);
                var setc = new HashSet<TreeNode>(node.Left.FirstPos);
                if (node.Left.Nullable)
                {
                    setc.UnionWith(node.Right.FirstPos);
                }
                node.FirstPos = setc;
                break;
            default:
                node.FirstPos = new HashSet<TreeNode> { node } ;
                break;
            }
        }

        public void BuildLastPos(TreeNode node)
        {
            if (node == null)
                return;
            switch (node.Type)
            {
            case TreeNode.Epsilon:
                node.LastPos = new HashSet<TreeNode>();
                break;
            case TreeNode.Star:
            case TreeNode.Plus:
                BuildLastPos(node.Left);
                node.LastPos = new HashSet<TreeNode>(node.Left.LastPos);
                break;
            case TreeNode.Or:
                BuildLastPos(node.Left);
                BuildLastPos(node.Right);
                var set = new HashSet<TreeNode>(node.Left.LastPos);
                set.UnionWith(node.Left.LastPos);
                node.LastPos = set;
                break;
            case TreeNode.Cat:
                BuildLastPos(node.Left);
                BuildLastPos(node.Right);
                var setc = new HashSet<TreeNode>(node.Right.LastPos);
                if (node.Right.Nullable)
                {
                    setc.UnionWith(node.Left.LastPos);
                }
                node.FirstPos = setc;
                break;
            default: 
                node.LastPos = new HashSet<TreeNode> { node };
                break;
            }
        }

        public void BuildFollowPos(TreeNode node)
        {
            node.FollowPos = new HashSet<TreeNode>();
            switch (node.Type)
            { 
            case TreeNode.Cat:
                BuildFollowPos(node.Left);
                BuildFollowPos(node.Right);
                foreach (var i in node.Left.LastPos)
                {
                    i.FollowPos.UnionWith(node.Right.FirstPos);
                }
                break;
            case TreeNode.Star:
            case TreeNode.Plus:
                BuildFollowPos(node.Left);
                foreach (var i in node.Left.LastPos)
                {
                    i.FollowPos.UnionWith(node.FirstPos);
                }
                break;
            case TreeNode.Or:
                BuildFollowPos(node.Left);
                BuildFollowPos(node.Right);
                break;
            }
        }
        
        public class TreeNode
        {
            public const int EOS = -1;

            public const int Cut = 1;
            public const int Or = 2;
            public const int Cat = 3;
            public const int Star = 4;
            public const int Plus = 5;

            public const int Char = 6;
            public const int CharClass = 6;
            public const int Any = 7;
            public const int Epsilon = 8;

            public int Type;
            public byte Value;
            public BitArray ValueClass;
            public TreeNode Left;
            public TreeNode Right;
            public bool Nullable;
            public HashSet<TreeNode> FirstPos;
            public HashSet<TreeNode> LastPos;
            public HashSet<TreeNode> FollowPos;

            public IEnumerable<byte> GetTransitionCharacters()
            {
                if (Type == TreeNode.Char)
                {
                    yield return Value;
                }
                else if (Type == TreeNode.CharClass)
                {
                    int i = 0;
                    foreach (bool x in ValueClass)
                    {
                        if (x)
                            yield return (byte) i;
                        ++i;
                    }
                }
            }
        }

        public class DfaPatternParser
        {
            private int idx;
            private string pattern;

            public DfaPatternParser(string pattern)
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
                        Type = TreeNode.Cut,
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
                        Type = TreeNode.Or,
                        Left = head,
                        Right = tail,
                    };
                }
                return head;
            }

            private TreeNode ParseCat()
            {
                var head = ParseFactor();
                var prev = head;
                for (; ; )
                {
                    var tail = ParseFactor();
                    if (tail == null)
                        break;
                    prev.Right = new TreeNode
                    {
                        Type = TreeNode.Cat,
                        Left = tail,
                    };
                }
                return head;
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
                        Type = TreeNode.Plus,
                        Left = atom
                    };
                }
                else if (PeekAndDiscard('*'))
                {
                    return new TreeNode
                    {
                        Type = TreeNode.Star,
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
                    Type = TreeNode.Char,
                    Value = (byte) d,
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
                    Type = TreeNode.CharClass,
                    ValueClass = bytes
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
    
internal void ExtendWithEos()
{
            t  = new TreeNode { Type = TreeNode.Cat, Left = t, Right = new TreeNode { Type = TreeNode.EOS } };
}
public DfaState[] DfaStates { get; set; }

public int[,] Transitions { get; set; }
    }
}

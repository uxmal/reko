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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Core.Dfa
{
    /// <summary>
    /// A parsed node of a regular expresion.
    /// </summary>
    public class TreeNode
    {
        /// <summary>
        /// Type of node.
        /// </summary>
        public NodeType Type;

        /// <summary>
        /// Node number.
        /// </summary>
        public int Number;

        /// <summary>
        /// Value of the node.
        /// </summary>
        public byte Value;

        /// <summary>
        /// Value class of the node.
        /// </summary>
        public BitArray? ValueClass;

        /// <summary>
        /// Optional left child of the node.
        /// </summary>
        public TreeNode? Left;

        /// <summary>
        /// Optional right child of the node.
        /// </summary>
        public TreeNode? Right;

        /// <summary>
        /// True if this node is nullable.
        /// </summary>
        public bool Nullable;

        /// <summary>
        /// True if this node is a start state.
        /// </summary>
        public bool Starts;

        /// <summary>
        /// Nodes that can be reached from this node by following
        /// nullable (epsilon) transitions.
        /// </summary>
        public HashSet<TreeNode>? FirstPos;

        /// <summary>
        /// Nodes at the end of this node by following nullable (epsilon) transitions.
        /// </summary>
        public HashSet<TreeNode>? LastPos;

        /// <summary>
        /// Nodes that can be reached from this node.
        /// </summary>
        public HashSet<TreeNode>? FollowPos;

        /// <summary>
        /// Get the set of characters that can be used to transition
        /// out of this node.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetTransitionCharacters()
        {
            if (Type == NodeType.Char)
            {
                yield return Value;
            }
            else if (Type == NodeType.CharClass)
            {
                Debug.Assert(ValueClass is not null);
                int i = 0;
                foreach (bool x in ValueClass!)
                {
                    if (x)
                        yield return (byte) i;
                    ++i;
                }
            } 
            else if (Type == NodeType.Any)
            {
                for (int i = 0; i < 0x100; ++i)
                {
                    yield return (byte) i;
                }
            }
        }

        /// <summary>
        /// Returns a string representation of this node.
        /// </summary>
        public override string ToString()
        {
            var sw = new StringWriter();
            Write(sw);
            return sw.ToString();
        }

        /// <summary>
        /// Write a string representation of this node to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        public void Write(TextWriter writer)
        {
            writer.Write("{0}", Type);
            if (Number != 0)
                writer.Write(" - {0}", Number);
            if (Starts)
                writer.Write(" - Starts");
            writer.WriteLine();
            WriteSet("    First:  ", FirstPos!, writer);
            WriteSet("    Last:   ", LastPos!, writer);
            WriteSet("    Follow: ", FollowPos!, writer);
            switch (Type)
            {
            case NodeType.Char: writer.Write("    {0} (0x{1:X2})", (char) Value, (int) Value); break;
            case NodeType.CharClass: writer.Write("[]"); break;
            case NodeType.Cat: Left!.Write(writer); Right!.Write(writer); break;
            case NodeType.Plus: Left!.Write(writer); break;
            case NodeType.Cut: Left!.Write(writer); Right!.Write(writer); break;
            }
            writer.WriteLine();
        }

        private static void WriteSet(string caption, HashSet<TreeNode> set, TextWriter sb)
        {
            sb.Write(caption);
            sb.WriteLine(string.Join(",", set.OrderBy(n => n.Number).Select(n => n.Number.ToString())));
        }
    }

    /// <summary>
    /// DFA node types.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// End of input.
        /// </summary>
        EOS = -1,

        /// <summary>
        /// A cut node.
        /// </summary>
        Cut = 1,

        /// <summary>
        /// An or node
        /// </summary>
        Or = 2,

        /// <summary>
        /// A concatenation node.
        /// </summary>
        Cat = 3,

        /// <summary>
        /// A kleene star node.
        /// </summary>
        Star = 4,

        /// <summary>
        /// A plus node; like a star, but requires at least one match.
        /// </summary>
        Plus = 5,

        /// <summary>
        /// Exact character match.
        /// </summary>
        Char = 6,

        /// <summary>
        /// Character class match.
        /// </summary>
        CharClass = 7,

        /// <summary>
        /// Matches any character.
        /// </summary>
        Any = 8,

        /// <summary>
        /// Epsilon transition.
        /// </summary>
        Epsilon = 9,
    }
}
#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// The C parser state.
    /// </summary>
    public class ParserState
    {
        private Stack<int> alignments;

        /// <summary>
        /// Constructs a <see cref="ParserState"/> instance.
        /// </summary>
        public ParserState()
        {
            Typedefs = new HashSet<string>();
            alignments = new Stack<int>();
            alignments.Push(0); // A value of 0 means 'use the platform default'.
            Typedefs.Add("size_t");
            Typedefs.Add("va_list");
        }

        /// <summary>
        /// Constructs a <see cref="ParserState"/> instance.
        /// </summary>
        /// <param name="symbolTable">Symbol table to initialize with.
        /// </param>
        public ParserState(SymbolTable symbolTable) : this()
        {
            Typedefs.UnionWith(symbolTable.PrimitiveTypes.Keys);
            Typedefs.UnionWith(symbolTable.NamedTypes.Keys);
        }

        /// <summary>
        /// The set of typedefs that are known to the parser.
        /// </summary>
        public HashSet<string> Typedefs { get; private set; }

        /// <summary>
        /// Current memory alignment.
        /// </summary>
        public int Alignment { get { return alignments.Peek(); } }

        /// <summary>
        /// Pushes the current alignment on the stack and makes another alignment current.
        /// </summary>
        /// <param name="align">New alignment to start using.
        /// </param>
        public void PushAlignment(int align)
        {
            alignments.Push(align);
        }

        /// <summary>
        /// Removes the most recent alignment from the alignment stack.
        /// </summary>
        public void PopAlignment()
        {
            alignments.Pop();
        }
    }
}
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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.CLanguage
{
    public class ParserState
    {
        private Stack<int> alignments;
        
        public ParserState()
        {
            Typedefs = new HashSet<string>();
            alignments = new Stack<int>();
            alignments.Push(8);
            Typedefs.Add("size_t");
            Typedefs.Add("va_list");
        }

        public ParserState(SymbolTable symbolTable) : this()
        {
            Typedefs.UnionWith(symbolTable.PrimitiveTypes.Keys);
            Typedefs.UnionWith(symbolTable.NamedTypes.Keys);
        }

        public HashSet<string> Typedefs { get; private set; }

        public int Alignment { get { return alignments.Peek(); } }

        public void PushAlignment(int align)
        {
            alignments.Push(align);
        }

        public void PopAlignment()
        {
            alignments.Pop();
        }
    }
}
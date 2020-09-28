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

using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Core.Output
{
    public class JsonWriter
    {
        private const int EMPTY_OBJ = 1;
        private const int OBJ = 1;
        private const int ARR = 2;
        private const int PROP = 3;

        private PrettyPrinter pp;
        private Stack<int> state;

        public JsonWriter(TextWriter w)
        {
            this.pp = new PrettyPrinter(w, 120);
            this.state = new Stack<int>();
        }

        public void WriteStartObject()
        {
            pp.PrintCharacter('{');
            pp.BeginGroup();
            pp.Indent(4);
            state.Push(EMPTY_OBJ);
        }

        public void WritePropertyName(string n)
        {
            if (state.Count == 0)
                throw new InvalidOperationException();
            if (state.Peek() == OBJ)
            {
                pp.PrintCharacter(',');
                pp.ConnectedLineBreak();
            }
            pp.PrintString(string.Format("\"{0}\":", n));
        }

        public void Write(string s)
        {
            pp.PrintString(string.Format("\"{0}\"", s));
            if (state.Count > 0 && state.Peek() == PROP)
            {
                state.Pop();
            }
        }

        public void WriteStartArray()
        {
            pp.PrintCharacter('[');
            pp.BeginGroup();
            pp.Indent(4);
            state.Push(ARR);
        }

        public void WriteEnd()
        {
            if (state.Count == 0)
                throw new InvalidOperationException();
            var st = state.Pop();
            switch (st)
            {
            case ARR: pp.PrintCharacter(']'); pp.EndGroup(); break;
            case OBJ: pp.PrintCharacter('}'); pp.EndGroup(); break;
            }
            pp.Outdent(4);
        }

        public void Flush()
        {
            pp.Flush();
        }
    }
}
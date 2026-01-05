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

using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Core.Output
{
    /// <summary>
    /// Simple JSON serializer class.
    /// </summary>
    public class JsonWriter
    {
        private const int EMPTY_OBJ = 1;
        private const int OBJ = 1;
        private const int ARR = 2;
        private const int PROP = 3;

        private readonly PrettyPrinter pp;
        private readonly Stack<int> state;

        /// <summary>
        /// Constructs an instance of <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="w"></param>
        public JsonWriter(TextWriter w)
        {
            this.pp = new PrettyPrinter(w, 120);
            this.state = new Stack<int>();
        }

        /// <summary>
        /// Writes the start of a JSON object.
        /// </summary>
        public void WriteStartObject()
        {
            pp.PrintCharacter('{');
            pp.BeginGroup();
            pp.Indent(4);
            state.Push(EMPTY_OBJ);
        }

        /// <summary>
        /// Writes a JSON property name.
        /// </summary>
        /// <param name="n">The name of the property.</param>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Writes a string
        /// </summary>
        /// <param name="s">String to write</param>
        public void Write(string s)
        {
            //$BUG: no escaping!
            pp.PrintString(string.Format("\"{0}\"", s));
            if (state.Count > 0 && state.Peek() == PROP)
            {
                state.Pop();
            }
        }

        /// <summary>
        /// Write the start of an array.
        /// </summary>
        public void WriteStartArray()
        {
            pp.PrintCharacter('[');
            pp.BeginGroup();
            pp.Indent(4);
            state.Push(ARR);
        }

        /// <summary>
        /// Writes the ending character of a JSON object or array.
        /// </summary>
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

        /// <summary>
        /// Flushes any pending characters to the output device.
        /// </summary>
        public void Flush()
        {
            pp.Flush();
        }
    }
}
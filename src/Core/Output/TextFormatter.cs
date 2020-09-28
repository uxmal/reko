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

using Reko.Core.Types;
using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Output
{
    /// <summary>
    /// Formatter that writes to a TextWriter.
    /// </summary>
    /// <remarks>
    /// Useful when writing decompiler output to a text file.
    /// </remarks>
    public class TextFormatter : Formatter
    {
        public TextFormatter(TextWriter writer)
        {
            this.TextWriter = writer;
            this.Terminator = Environment.NewLine;
        }

        public TextWriter TextWriter { get; private set; }
        public string Terminator { get; set; }

        /// <summary>
        /// Terminate a line using the terminator string.
        /// </summary>
        public override void Terminate()
        {
            TextWriter.Write(Terminator);
        }

        /// <summary>
        /// Write the string <paramref name="s"/> with no special formatting.
        /// </summary>
        /// <param name="s"></param>
        public override void Write(string s)
        {
            TextWriter.Write(s);
        }

        public override Formatter Write(char ch)
        {
            TextWriter.Write(ch);
            return this;
        }

        public override void Write(string format, params object[] arguments)
        {
            TextWriter.Write(format, arguments);
        }

        public override void WriteComment(string comment)
        {
            TextWriter.Write(comment);
        }

        public override void WriteHyperlink(string text, object href)
        {
            TextWriter.Write(text);
        }

        public override void WriteKeyword(string keyword)
        {
            TextWriter.Write(keyword);
        }

        public override void WriteLine()
        {
            TextWriter.WriteLine();
        }

        public override void WriteLine(string s)
        {
            TextWriter.WriteLine(s);
        }

        public override void WriteLine(string format, params object[] args)
        {
            TextWriter.WriteLine(format, args);
        }

        public override void WriteType(string typeName, DataType dt)
        {
            TextWriter.Write(typeName);
        }

    }
}

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

using Reko.Core.Types;
using System;
using System.IO;

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
        /// <summary>
        /// Constructs a text formatter.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        public TextFormatter(TextWriter writer)
        {
            this.TextWriter = writer;
            this.Terminator = Environment.NewLine;
        }

        /// <summary>
        /// The output sink.
        /// </summary>
        public TextWriter TextWriter { get; private set; }

        /// <summary>
        /// Line terminator.
        /// </summary>
        public string Terminator { get; set; }

        /// <summary>
        /// Signals the start of a line with an object (which is ignored)
        /// </summary>
        /// <param name="tag"></param>
        public override void Begin(object? tag)
        {
        }


        /// <inheritdoc/>
        public override void Terminate()
        {
            TextWriter.Write(Terminator);
        }

        /// <inheritdoc/>
        public override void Write(string s)
        {
            TextWriter.Write(s);
        }

        /// <inheritdoc/>
        public override Formatter Write(char ch)
        {
            TextWriter.Write(ch);
            return this;
        }

        /// <inheritdoc/>
        public override void Write(string format, params object[] arguments)
        {
            TextWriter.Write(format, arguments);
        }

        /// <inheritdoc/>
        public override void WriteComment(string comment)
        {
            TextWriter.Write(comment);
        }

        /// <inheritdoc/>
        public override void WriteHyperlink(string text, object href)
        {
            TextWriter.Write(text);
        }

        /// <inheritdoc/>
        public override void WriteKeyword(string keyword)
        {
            TextWriter.Write(keyword);
        }

        /// <inheritdoc/>
        public override void WriteLabel(string label, object block)
        {
            TextWriter.Write(label);
        }

        /// <inheritdoc/>
        public override void WriteLine()
        {
            TextWriter.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(string s)
        {
            TextWriter.WriteLine(s);
        }

        /// <inheritdoc/>
        public override void WriteLine(string format, params object[] args)
        {
            TextWriter.WriteLine(format, args);
        }

        /// <inheritdoc/>
        public override void WriteType(string typeName, DataType dt)
        {
            TextWriter.Write(typeName);
        }

    }
}

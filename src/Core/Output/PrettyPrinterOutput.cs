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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Core.Output
{
    /// <summary>
    /// Rerpresents the <see cref="PrettyPrinter"/>'s output device.
    /// </summary>
    public class PrettyPrinterOutput
    {
        private TextWriter writer;
        private int indentColumn;
        private int deviceOutputWidth;
        private int totalCharsFlushed;
        private bool emitIndentationFirst;

        /// <summary>
        /// Construct a new <see cref="PrettyPrinterOutput"/> instance.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        /// <param name="width">Device width.</param>
        public PrettyPrinterOutput(TextWriter writer, int width)
        {
            this.writer = writer;
            this.deviceOutputWidth = width;
            emitIndentationFirst = true;
        }

        /// <summary>
        /// Number of enqueued printable characters.
        /// </summary>
        public int total_pchars_enqueued { get; set; }

        /// <summary>
        /// Change the left column position.
        /// </summary>
        /// <param name="indentAmount">Amount to change.</param>
        public void Indent(int indentAmount)
        {
            indentColumn += indentAmount;
        }

        /// <summary>
        /// Returns true if the current line must be split.
        /// </summary>
        public bool MustSplitLine
        {
            get
            {
                return (total_pchars_enqueued - totalCharsFlushed) + indentColumn
                                >= deviceOutputWidth;
            }
        }

        /// <summary>
        /// Displays a printable character.
        /// </summary>
        /// <param name="c">Character to display.</param>
        public void PrintCharacter(char c)
        {
            if (emitIndentationFirst)
            {
                WriteIndentation();
                emitIndentationFirst = false;
            }
            writer.Write(c);
            ++totalCharsFlushed;
        }

        /// <summary>
        /// Ends a line.
        /// </summary>
        public void PrintLine()
        {
            WriteLine();
            emitIndentationFirst = true;
        }

        /// <summary>
        /// Emits a line break.
        /// </summary>
        public virtual void WriteLine()
        {
            writer.WriteLine();
        }

        /// <summary>
        /// Writes indentation to the output device.
        /// </summary>
        public virtual void WriteIndentation()
        {
            writer.Write(new string(' ', this.indentColumn));
        }
    }
}

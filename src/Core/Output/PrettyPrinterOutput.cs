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
using System.Text;

namespace Reko.Core.Output
{
    public class PrettyPrinterOutput
    {
        private TextWriter writer;
        private int indentColumn;
        private int deviceOutputWidth;
        private int totalCharsFlushed;
        private bool emitIndentationFirst;

        public PrettyPrinterOutput(TextWriter writer, int width)
        {
            this.writer = writer;
            this.deviceOutputWidth = width;
            emitIndentationFirst = true;
        }

        public int total_pchars_enqueued { get; set; }

        public void Indent(int indentAmount)
        {
            indentColumn += indentAmount;
        }

        public bool MustSplitLine
        {
            get
            {
                return (total_pchars_enqueued - totalCharsFlushed) + indentColumn
                                >= deviceOutputWidth;
            }
        }

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

        public void PrintLine()
        {
            WriteLine();
            emitIndentationFirst = true;
        }

        public virtual void WriteLine()
        {
            writer.WriteLine();
        }

        public virtual void WriteIndentation()
        {
            writer.Write(new string(' ', this.indentColumn));
        }
    }
}

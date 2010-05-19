using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Output
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

        public void Indent(int indentAmt)
        {
            indentColumn += indentAmt;
        }

        public void Outdent(int outdentAmt)
        {
            indentColumn -= outdentAmt;
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

        public void PrintNewLine()
        {
            WriteNewLine();
            emitIndentationFirst = true;
        }

        public virtual void WriteNewLine()
        {
            writer.WriteLine();
        }

        public virtual void WriteIndentation()
        {
            writer.Write(new string(' ', this.indentColumn));
        }
    }
}

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

        public void Indent(int indentAmount)
        {
            indentColumn += indentAmount;
        }

        public void Outdent(int outdentAmount)
        {
            indentColumn -= outdentAmount;
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

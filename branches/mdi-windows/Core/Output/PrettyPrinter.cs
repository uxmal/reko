/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Output
{
    /// <summary>
    /// Prettyprint engine for nice formatting of long source code lines.
    /// </summary>
    /// <remarks>
    /// Based on the description given in "A New Language-Independent Prettyprinting
    /// Algorithm", by William W. Pugh and Steven J. Sinofsky TR 87-808, January 1987,
    /// Department of Computer Science, Cornell University.
    /// </remarks>
    public class PrettyPrinter
    {
        private TextWriter writer;
        private IDequeue<Token> buffer;
        private IDequeue<Break> breaks;
        private PrettyPrinterOutput output;
        private int currentGroupLevel;
        private int breakLevel;
        private int totalEnqueuedTokens;
        private int totalFlushedTokens;
        public int INDENT_WIDTH = 4;

        public PrettyPrinter(TextWriter writer, int width)
        {
            this.writer = writer;
            this.buffer = new Dequeue<Token>();
            this.breaks = new Dequeue<Break>();
            this.output = new PrettyPrinterOutput(writer, width);
            this.breakLevel = -1;
        }

        public void BeginGroup()
        {
            ++currentGroupLevel;
        }

        public void EndGroup()
        {
            --currentGroupLevel;
            if (breakLevel > currentGroupLevel)
                breakLevel = currentGroupLevel;
        }

        public void Indent()
        {
            EnqueueToken(new IndentToken(INDENT_WIDTH, output));
        }

        public void Outdent()
        {
            EnqueueToken(new OutdentToken(INDENT_WIDTH, output));
        }

        public void ForceLineBreak()
        {
            breaks.Clear();
            breakLevel = currentGroupLevel;
            EnqueueToken(new NewlineToken(output));
            PrintBuffer(buffer.Count);
        }

        public void OptionalLineBreak()
        {
            while (breaks.Count != 0 &&
                (breaks.PeekFront().level > currentGroupLevel
                || (breaks.PeekFront().level == currentGroupLevel &&
                    !breaks.PeekFront().connected)))
            {
                // discard breaks we are no longer interested in
                breaks.PopFront();
            }
            EnqueueBreak(false);
        }

        public void ConnectedLineBreak()
        {
            if (breakLevel < currentGroupLevel)
            {
                while (breaks.Count != 0 &&
                    breaks.PeekFront().level >= currentGroupLevel)
                {
                    // discard breaks we are no longer interested in.
                    breaks.PopFront();
                }
                EnqueueToken(new MarkerToken(currentGroupLevel, output));
                EnqueueBreak(true);
            }
            else
            {
                // take an immediate break, break_level == current_level
                breaks.Clear();
                EnqueueToken(new NewlineToken(output));
                PrintBuffer(buffer.Count);
            }
        }

        public void PrintCharacter(char ch)
        {
            if (output.MustSplitLine)  // must split line
            {
                if (breaks.Count != 0)
                {
                    // split line at break.
                    var temp = breaks.PopBack();
                    breakLevel = temp.level;
                    PrintBuffer(temp.chars_enqueued - totalFlushedTokens);
                    if (!temp.connected)
                    {
                        output.PrintNewLine();
                    }
                    breakLevel = Math.Min(breakLevel, currentGroupLevel);
                }
                else
                {
                    // no breaks to make! Oh noes.
                    throw new NotImplementedException();
                }
            }
            EnqueueToken(new StringToken(ch, output));
            ++output.total_pchars_enqueued;
        }

        public void Flush()
        {
            PrintBuffer(buffer.Count);
        }

        private void PrintBuffer(int k)
        {
            for (int i = 0; i < k; ++i)
            {
                var token = buffer.PopFront();
                ++totalFlushedTokens;
                token.Print(breakLevel);
            }
        }

        private void EnqueueToken(Token token)
        {
            buffer.PushBack(token);
            ++totalEnqueuedTokens;
        }

        private void EnqueueBreak(bool connectedBreak)
        {
            breaks.PushFront(new Break(totalEnqueuedTokens, currentGroupLevel, connectedBreak));
        }


        private abstract class Token
        {
            public char type { get; private set; }
            public int value { get; private set; }
            public PrettyPrinterOutput Output { get; private set; }

            protected Token(PrettyPrinterOutput output)
            {
                this.Output = output;
            }

            public abstract void Print(int break_level);
        }

        private class MarkerToken : Token
        {
            private int groupingLevel;
            public MarkerToken(int groupingLevel, PrettyPrinterOutput output)
                : base(output)
            {
                this.groupingLevel = groupingLevel;
            }

            public override void Print(int break_level)
            {
                if (value <= break_level)
                    Output.PrintNewLine();
            }
        }

        private class IndentToken : Token
        {
            private int indentAmt;

            public IndentToken(int amount, PrettyPrinterOutput output)
                : base(output)
            {
                indentAmt = amount;
            }

            public override void Print(int break_level)
            {
                Output.Indent(indentAmt);
            }
        }

        private class OutdentToken : Token
        {
            private int outdentAmt;

            public OutdentToken(int amount, PrettyPrinterOutput output)
                : base(output)
            {
                outdentAmt = amount;
            }

            public override void Print(int break_level)
            {
                Output.Indent(outdentAmt);
            }
        }

        private class NewlineToken : Token
        {
            public NewlineToken(PrettyPrinterOutput output) : base(output)
            {
            }

            public override void Print(int break_level)
            {
                Output.PrintNewLine();
            }
        }
        
        private class StringToken : Token
        {
            private char c;

            public StringToken(char c, PrettyPrinterOutput output) : base(output)
            {
                this.c = c;
            }

            public override void Print(int break_level)
            {
                Output.Print_Character(c);
            }
        }

        public class Break
        {
            public int chars_enqueued { get; private set; }
            public int level { get; private set; }
            public bool connected { get; private set; }

            public Break(int count, int level, bool connected)
            {
                this.chars_enqueued = count;
                this.level = level;
                this.connected = connected;
            }
        }

    }

    public class PrettyPrinterOutput
    {
        private TextWriter writer;

        public PrettyPrinterOutput(TextWriter writer, int width)
        {
            this.writer = writer;
            this.device_output_width = width;
        }

        public int device_left_margin { get; set; }
        public int device_output_width { get; private set; }
        public int total_pchars_enqueued { get; set; }
        public int total_pchars_flushed { get; set; }

        public void Indent(int indentAmt)
        {
            device_left_margin += indentAmt;
        }

        public void Outdent(int outdentAmt)
        {
            device_left_margin -= outdentAmt;
        }

        
        public bool MustSplitLine
        {
            get
            {
                return (total_pchars_enqueued - total_pchars_flushed) + device_left_margin
                                >= device_output_width;
            }
        }

        public void Print_Character(char c)
        {
            writer.Write(c);
            ++total_pchars_flushed;
        }

        public void PrintNewLine()
        {
            writer.WriteLine();
        }
    }
}

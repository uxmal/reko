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

using Reko.Core.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Core.Output
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
        private IDequeue<Token> buffer;     // Pending tokens
        private IDequeue<Break> breaks;     // Conditional breaks 
        private PrettyPrinterOutput output;
        private int currentGroupLevel;
        private int breakLevel;
        private int totalEnqueuedTokens;
        private int totalFlushedTokens;

        public PrettyPrinter(TextWriter writer, int width)
        {
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

        public void Indent(int indentWidth)
        {
            EnqueueToken(new IndentToken(indentWidth, output));
        }

        public void Outdent(int indentWidth)
        {
            EnqueueToken(new IndentToken(-indentWidth, output));
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
            // Discard breaks we are no longer interested in

            while (breaks.Count != 0 &&
                (breaks.PeekFront().Level > currentGroupLevel
                || (breaks.PeekFront().Level == currentGroupLevel &&
                    !breaks.PeekFront().IsConnected)))
            {
                breaks.PopFront();
            }
            EnqueueBreak(false);
        }

        public void ConnectedLineBreak()
        {
            if (breakLevel < currentGroupLevel)
            {
                // Discard breaks we are no longer interested in.

                while (breaks.Count != 0 &&
                    breaks.PeekFront().Level >= currentGroupLevel)
                {
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

        public void PrintString(string s)
        {
            foreach (var c in s)
                PrintCharacter(c);
        }

        public void PrintCharacter(char ch)
        {
            if (output.MustSplitLine)  // must split line
            {
                if (breaks.Count != 0)
                {
                    // split line at break.
                    var br = breaks.PopBack();
                    breakLevel = br.Level;
                    PrintBuffer(br.TokensEnqueued - totalFlushedTokens);
                    if (!br.IsConnected)
                    {
                        output.PrintLine();
                    }
                    breakLevel = Math.Min(breakLevel, currentGroupLevel);
                }
                else
                {
                    // no breaks to make! Oh noes. Just overflow in this case.
                }
            }
            EnqueueToken(new StringToken(ch, output));
            ++output.total_pchars_enqueued;
        }

        public void Flush()
        {
            PrintBuffer(buffer.Count);
        }

        /// <summary>
        /// Send the <paramref name="k"/> first tokens in the buffer to
        /// the output.
        /// </summary>
        /// <param name="k">Number of tokens to emit.</param>
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
            protected Token(PrettyPrinterOutput output)
            {
                this.Output = output;
            }

            public PrettyPrinterOutput Output { get; private set; }

            public abstract void Print(int break_level);
        }

        private class MarkerToken : Token
        {
            private readonly int groupingLevel;

            public MarkerToken(int groupingLevel, PrettyPrinterOutput output)
                : base(output)
            {
                this.groupingLevel = groupingLevel;
            }

            public override void Print(int break_level)
            {
                if (groupingLevel <= break_level)
                    Output.PrintLine();
            }
        }

        private class IndentToken : Token
        {
            private readonly int indentAmt;

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

        private class NewlineToken : Token
        {
            public NewlineToken(PrettyPrinterOutput output) : base(output)
            {
            }

            public override void Print(int break_level)
            {
                Output.PrintLine();
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
                Output.PrintCharacter(c);
            }
        }

        public class Break
        {
            public int TokensEnqueued { get; private set; }
            public int Level { get; private set; }
            public bool IsConnected { get; private set; }

            public Break(int count, int level, bool connected)
            {
                this.TokensEnqueued = count;
                this.Level = level;
                this.IsConnected = connected;
            }
        }
    }
}

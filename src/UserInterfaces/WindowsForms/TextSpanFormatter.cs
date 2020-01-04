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

using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using Reko.UserInterfaces.WindowsForms.Controls;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Implements a formatter that renders text into
    /// a TextViewModel that can be used with a TextView.
    /// </summary>
    public class TextSpanFormatter : Formatter
    {
        private List<List<TextSpan>> textLines;
        private List<TextSpan> currentLine;
        private FixedTextSpan currentSpan;
        
        public TextSpanFormatter()
        {
            this.textLines = new List<List<TextSpan>>();
        }

        public TextSpan[][] GetLines()
        {
            return textLines.Select(l => l.ToArray()).ToArray();
        }

        public TextViewModel GetModel()
        {
            return new TextSpanModel(textLines.Select(l => l.ToArray())
                .ToArray());
        }

        public override void Terminate()
        {
            currentSpan = null;
            currentLine = null;
        }

        public override void Write(string s)
        {
            EnsureSpan().Text.Append(s);
        }

        public override Formatter Write(char ch)
        {
            EnsureSpan().Text.Append(ch);
            return this;
        }

        public override void Write(string format, params object[] arguments)
        {
            EnsureSpan().Text.AppendFormat(format, arguments);
        }

        public override void WriteComment(string comment)
        {
            currentSpan = null;
            var span = EnsureSpan();
            span.Style = "cmt";
            span.Text.Append(comment);
            currentSpan = null;
        }

        public override void WriteHyperlink(string text, object href)
        {
            currentSpan = null;
            var span = EnsureSpan();
            span.Style = "link";
            span.Text.Append(text);
            span.Tag = href;
            currentSpan = null;
        }

        public override void WriteKeyword(string keyword)
        {
            currentSpan = null;
            var span = EnsureSpan();
            span.Style = "code-kw";
            span.Text.Append(keyword);
            currentSpan = null;
        }

        public override void WriteType(string typeName, DataType dt)
        {
            currentSpan = null;
            var span = EnsureSpan();
            span.Style = "type";
            span.Text.Append(typeName);
            span.Tag = dt;
            currentSpan = null;
        }

        public override void WriteLine()
        {
            currentSpan = null;
            currentLine = null;
        }

        public override void WriteLine(string s)
        {
            EnsureSpan().Text.Append(s);
            currentSpan = null;
            currentLine = null;
        }

        public override void WriteLine(string format, params object[] arguments)
        {
            EnsureSpan().Text.AppendFormat(format, arguments);
            currentSpan = null;
            currentLine = null;
        }

        private FixedTextSpan EnsureSpan()
        {
            if (currentLine == null)
            {
                currentLine = new List<TextSpan>();
                this.textLines.Add(currentLine);
            }
            if (currentSpan == null)
            {
                currentSpan = new FixedTextSpan();
                this.currentLine.Add(currentSpan);
            }
            return currentSpan;
        }

        private class TextSpanModel : TextViewModel
        {
            private TextSpan[][] lines;
            private int position;

            public TextSpanModel(TextSpan[][] lines)
            {
                this.lines = lines;
            }

            public int LineCount { get { return lines.Length; } }

            public int ComparePositions(object a, object b)
            {
                return ((int)a).CompareTo((int)b);
            }

            public object CurrentPosition { get { return position; } }

            public object StartPosition { get { return 0; } }

            public object EndPosition { get { return lines.Length; } }

            public int MoveToLine(object position, int offset)
            {
                int orig = (int)position;
                this.position = orig + offset;
                if (this.position < 0)
                    this.position = 0;
                if (this.position >= lines.Length)
                    this.position = lines.Length;
                return this.position - orig;
            }

            public LineSpan[] GetLineSpans(int count)
            {
                int p = (int)position;
                int c = Math.Min(count, lines.Length - p);
                if (c <= 0)
                    return new LineSpan[0];
                var spans = new LineSpan[c];
                for (int i = 0; i < c; ++i)
                {
                    spans[i] = new LineSpan(p+i, lines[p+i]);
                }
                position = p + c;
                return spans;
            }

            public Tuple<int, int> GetPositionAsFraction()
            {
                return Tuple.Create((int)position, lines.Length);
            }

            public void SetPositionAsFraction(int numer, int denom)
            {
                position = (int) (Math.BigMul(numer, lines.Length) / denom);
            }
        }

        /// <summary>
        /// Simple span containing a string/text.
        /// </summary>
        private class FixedTextSpan : TextSpan
        {
            public StringBuilder Text;

            public FixedTextSpan()
            {
                this.Text = new StringBuilder();
            }

            public override string GetText()
            {
                return Text.ToString();
            }
        }
    }
}

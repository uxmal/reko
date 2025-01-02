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

using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Gui.TextViewing
{
    /// <summary>
    /// An implementation of <see cref="Formatter"/> formatter that renders
    /// text into a <see cref="ITextViewModel"/> that can be used with a TextView.
    /// </summary>
    public sealed class TextSpanFormatter : Formatter
    {
        private readonly TextSpanFactory factory;
        private readonly List<(object?, List<ITextSpan>)> textLines;
        private List<ITextSpan>? currentLine;
        private object? currentLineTag;
        private StringBuilder? currentSpan;
        
        public TextSpanFormatter(TextSpanFactory factory)
        {
            Debug.Assert(factory is not null);
            this.factory = factory;
            this.textLines = new List<(object?, List<ITextSpan>)>();
            this.currentLineTag = null;
        }

        public LineSpan[] GetLines()
        {
            return textLines.Select((l, i) => new LineSpan(i, l.Item1, l.Item2.ToArray()))
                .ToArray();
        }

        public ITextViewModel GetModel()
        {
            return new TextSpanModel(GetLines());
        }

        public override void Begin(object? tag)
        {
            this.currentLineTag = tag;
        }

        public override void Terminate()
        {
            FinishSpan();
            currentLine = null;
            currentLineTag = null;
        }

        public override void Write(string s)
        {
            EnsureSpan().Append(s);
        }

        public override Formatter Write(char ch)
        {
            EnsureSpan().Append(ch);
            return this;
        }

        public override void Write(string format, params object[] arguments)
        {
            EnsureSpan().AppendFormat(format, arguments);
        }

        public override void WriteComment(string comment)
        {
            FinishSpan();
            var span = EnsureSpan();
            span.Append(comment);
            FinishSpan("cmt", null);
        }

        public override void WriteHyperlink(string text, object href)
        {
            FinishSpan();
            var span = EnsureSpan();
            span.Append(text);
            FinishSpan("link", href);
        }

        public override void WriteLabel(string label, object block)
        {
            FinishSpan();
            var span = EnsureSpan();
            span.Append(label);
            FinishSpan("label", block);
        }

        public override void WriteKeyword(string keyword)
        {
            FinishSpan();
            var span = EnsureSpan();
            span.Append(keyword);
            FinishSpan("code-kw", null);
        }

        public override void WriteType(string typeName, DataType dt)
        {
            FinishSpan();
            var span = EnsureSpan();
            span.Append(typeName);
            FinishSpan("type", dt);
        }

        public override void WriteLine()
        {
            FinishSpan();
            currentLine = null;
        }

        public override void WriteLine(string s)
        {
            EnsureSpan().Append(s);
            FinishSpan();
            currentLine = null;
        }

        public override void WriteLine(string format, params object[] arguments)
        {
            EnsureSpan().AppendFormat(format, arguments);
            FinishSpan();
            currentLine = null;
        }

        private void FinishSpan()
        {
            if (currentLine is null)
                return;
            var text = currentSpan is null ? "" : currentSpan.ToString();
            var span = factory.CreateTextSpan(text, null);
            this.currentLine.Add(span);
            this.currentSpan = null;
        }

        private void FinishSpan(string style, object? tag)
        {
            Debug.Assert(currentLine is not null);
            var text = currentSpan is null ? "" : currentSpan.ToString();
            var span = factory.CreateTextSpan(text, null);
            span.Style = style;
            span.Tag = tag;
            this.currentLine.Add(span);
            this.currentSpan = null;
        }

        private StringBuilder EnsureSpan()
        {
            if (currentLine is null)
            {
                currentLine = new List<ITextSpan>();
                this.textLines.Add((currentLineTag, currentLine));
            }
            if (currentSpan is null)
            {
                currentSpan = new StringBuilder();
            }
            return currentSpan;
        }

        private class TextSpanModel : ITextViewModel
        {
            private readonly LineSpan[] lines;
            private int position;

            public TextSpanModel(LineSpan[] lines)
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
                    return Array.Empty<LineSpan>();
                var spans = new LineSpan[c];
                for (int i = 0; i < c; ++i)
                {
                    spans[i] = lines[p+i];
                }
                position = p + c;
                return spans;
            }

            public (int, int) GetPositionAsFraction()
            {
                return ((int)position, lines.Length);
            }

            public void SetPositionAsFraction(int numer, int denom)
            {
                position = (int) (Math.BigMul(numer, lines.Length) / denom);
            }
        }
    }
}

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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Maintains the layout of the client area of a TextView.
    /// </summary>
    public class TextViewLayout
    {
        private TextViewModel model;
        private SortedList<float, LayoutLine> visibleLines;
        private Font defaultFont;

        public TextViewLayout(TextViewModel model, Font defaultFont)
        {
            this.model = model;
            this.visibleLines = new SortedList<float, LayoutLine>();
            this.defaultFont = defaultFont;
        }

        private TextViewLayout(TextViewModel model, Font defaultFont, SortedList<float, LayoutLine> visibleLines)
        {
            this.model = model;
            this.defaultFont = defaultFont;
            this.visibleLines = visibleLines;
        }

        public IList<LayoutLine> LayoutLines { get { return visibleLines.Values; } }

        /// <summary>
        /// Generates a TextViewLayout from all the lines in the model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="g"></param>
        /// <param name="defaultFont"></param>
        /// <param name="styleStack"></param>
        /// <returns></returns>
        public static TextViewLayout AllLines(TextViewModel model, Graphics g, Font defaultFont, StyleStack styleStack)
        {
            model.MoveToLine(model.StartPosition, 0);
            var rcLine = new RectangleF();
            var builder = new Builder(model, g, styleStack, defaultFont);
            for (;;)
            {
                var lines = model.GetLineSpans(1);
                if (lines.Length == 0)
                    break;
                builder.AddLayoutLine(lines[0], ref rcLine);
            }
            var layout = builder.Build();
            return layout;
        }


        public static TextViewLayout VisibleLines(TextViewModel model, Size size, Graphics g, Font defaultFont, StyleStack styleStack)
        {
            var szClient = new SizeF(size);
            var rcLine = new RectangleF(0, 0, szClient.Width, 0);

            var builder = new Builder(model, g, styleStack, defaultFont);
            while (rcLine.Top < szClient.Height)
            {
                var lines = model.GetLineSpans(1);
                if (lines.Length == 0)
                    break;
                builder.AddLayoutLine(lines[0], ref rcLine);
            }
            return builder.Build();
        }

        public SizeF CalculateExtent()
        {
            if (visibleLines.Count == 0)
                return new SizeF(0, 0);
            var width = visibleLines.Values.Max(l => l.Extent.Width);
            var height = visibleLines.Values.Select(l => l.Extent.Bottom).LastOrDefault();
            return new SizeF(width, height);
        }

        public TextPointer ClientToLogicalPosition(Graphics g, Point pt, StyleStack styleStack)
        {
            foreach (var line in this.visibleLines.Values)
            {
                if (line.Extent.Top <= pt.Y && pt.Y < line.Extent.Bottom)
                {
                    return FindSpanPosition(g, pt, line, styleStack);
                }
            }
            return new TextPointer(model.EndPosition, 0, 0);
        }

        public int ComparePositions(TextPointer a, TextPointer b)
        {
            var d = model.ComparePositions(a.Line, b.Line);
            if (d != 0)
                return d;
            d = a.Span.CompareTo(b.Span);
            if (d != 0)
                return d;
            return a.Character.CompareTo(b.Character);
        }

        private TextPointer FindSpanPosition(Graphics g, Point ptClient, LayoutLine line, StyleStack styleStack)
        {
            int iSpan = 0;
            foreach (var span in line.Spans)
            {
                if (span.ContentExtent.Contains(ptClient))
                {
                    int iChar = GetCharPosition(g, ptClient, span, styleStack);
                    return new TextPointer(line.Position, iSpan, iChar);
                }
                ++iSpan;
            }
            return new TextPointer(line.Position, iSpan, 0);
        }

        private int GetCharPosition(Graphics g, Point ptClient, LayoutSpan span, StyleStack styleStack)
        {
            var x = ptClient.X - span.ContentExtent.Left;
            var textStub = span.Text;
            int iLow = 0;
            int iHigh = textStub.Length;
            styleStack.PushStyle(span.Style);
            var font = styleStack.GetFont(defaultFont);
            var sz = MeasureText(g, textStub, font);
            float xLow = 0;
            float xHigh = sz.Width;
            while (iLow < iHigh - 1)
            {
                int iMid = iLow + (iHigh - iLow) / 2;
                textStub = span.Text.Substring(0, iMid);
                sz = MeasureText(g, textStub, font);
                if (x < sz.Width)
                {
                    iHigh = iMid;
                    xHigh = sz.Width;
                }
                else
                {
                    iLow = iMid;
                    xLow = sz.Width;
                }
            }
            styleStack.PopStyle();
            var cx = xHigh - xLow;
            if (x - xLow > cx)
                return iHigh;
            else
                return iLow;
        }

        public Size MeasureText(Graphics g, string text, Font font)
        {
            var sz = TextRenderer.MeasureText(
                g, text, font, new Size(0, 0),
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            return sz;
        }

        public RectangleF LogicalPositionToClient(Graphics g, TextPointer pos, StyleStack styleStack)
        {
            foreach (var line in this.visibleLines.Values)
            {
                if (line.Position == pos.Line)
                {
                    return SpanPositionToClient(g, pos, line, styleStack);
                }
            }
            return new RectangleF(new PointF(0, 0), CalculateExtent());
        }

        private RectangleF SpanPositionToClient(Graphics g, TextPointer pos, LayoutLine line, StyleStack styleStack)
        {
            var iSpan = pos.Span;
            if (iSpan < 0 || iSpan >= line.Spans.Length)
                return line.Extent;
            var span = line.Spans[iSpan];
            return CharPositionToClient(g, pos, span, styleStack);
        }

        private RectangleF CharPositionToClient(Graphics g, TextPointer pos, LayoutSpan span, StyleStack styleStack)
        {
            var iChar = pos.Character;

            styleStack.PushStyle(span.Style);
            var font = styleStack.GetFont(defaultFont);

            var textStub = span.Text.Substring(0, iChar);
            var sz = MeasureText(g, textStub, font);
            var x = sz.Width + span.ContentExtent.Left;
            var width = 1;

            styleStack.PopStyle();

            return new RectangleF(x, span.ContentExtent.Top, width, span.ContentExtent.Height);
        }


        private class Builder
        {
            private Graphics g;
            private TextViewModel model;
            private StyleStack styleStack;
            private Font defaultFont;
            private SortedList<float, LayoutLine> visibleLines;

            public Builder(TextViewModel model, Graphics g, StyleStack styleStack, Font defaultFont)
            {
                this.model = model;
                this.g = g;
                this.styleStack = styleStack;
                this.defaultFont = defaultFont;
                this.visibleLines = new SortedList<float, LayoutLine>();
            }

            public void AddLayoutLine(LineSpan line, ref RectangleF rcLine /* put in state */)
            {
                float cyLine = MeasureLineHeight(line);
                rcLine.Height = cyLine;
                var spans = ComputeSpanLayouts(line.TextSpans, rcLine);
                var ll = new LayoutLine(line.Position)
                {
                    Extent = LineExtent(rcLine, spans),
                    Spans = spans,
                };
                this.visibleLines.Add(rcLine.Top, ll);
                rcLine.Offset(0, cyLine);
            }

            private RectangleF LineExtent(RectangleF rcLine, LayoutSpan[] spans)
            {
                var r = rcLine.Right;
                if (spans.Length > 0)
                {
                    r = Math.Max(r, spans[spans.Length - 1].ContentExtent.Right);
                }
                return new RectangleF(rcLine.X, rcLine.Y, r - rcLine.X, rcLine.Height);
            }

            private float MeasureLineHeight(LineSpan line)
            {
                float height = 0.0F;
                foreach (var span in line.TextSpans)
                {
                    styleStack.PushStyle(span.Style);
                    var font = styleStack.GetFont(defaultFont);
                    height = Math.Max(
                        height,
                        font.Height + styleStack.GetNumber(s => s.PaddingTop) +
                        styleStack.GetNumber(s => s.PaddingBottom));
                    styleStack.PopStyle();
                }
                return height;
            }

            /// <summary>
            /// Computes the layout for a line of spans.
            /// </summary>
            /// <param name="spans"></param>
            /// <param name="rcLine"></param>
            /// <param name="g"></param>
            /// <returns></returns>
            private LayoutSpan[] ComputeSpanLayouts(IEnumerable<TextSpan> spans, RectangleF rcLine)
            {
                var spanLayouts = new List<LayoutSpan>();
                var pt = new PointF(rcLine.Left, rcLine.Top);
                foreach (var span in spans)
                {
                    styleStack.PushStyle(span.Style);
                    var text = span.GetText();
                    var font = styleStack.GetFont(defaultFont);
                    var szText = GetSize(span, text, font, g);
                    var rc = new RectangleF(pt, szText);
                    var rcPadded = rc;
                    styleStack.PadRectangle(ref rc, ref rcPadded);
                    spanLayouts.Add(new LayoutSpan
                    {
                        ContentExtent = rc,
                        PaddedExtent = rcPadded,
                        Style = span.Style,
                        Text = text,
                        ContextMenuID = span.ContextMenuID,
                        Tag = span.Tag,
                    });
                    pt.X = pt.X + rcPadded.Width;
                    styleStack.PopStyle();
                }
                return spanLayouts.ToArray();
            }

            /// <summary>
            /// Computes the size of a text span.
            /// </summary>
            /// <remarks>
            /// The span is first asked to measure itself, then the current style is 
            /// allowed to override the size.
            /// </remarks>
            /// <param name="span"></param>
            /// <param name="text"></param>
            /// <param name="font"></param>
            /// <param name="g"></param>
            /// <returns></returns>
            private SizeF GetSize(TextSpan span, string text, Font font, Graphics g)
            {
                var size = span.GetSize(text, font, g);
                int? width = styleStack.GetWidth();
                if (width.HasValue)
                {
                    size.Width = width.Value;
                }
                return size;
            }

            public TextViewLayout Build()
            {
                return new TextViewLayout(model, defaultFont, visibleLines);
            }
        }
    }

    /// <summary>
    /// Line of spans.
    /// </summary>
    public class LayoutLine
    {
        public LayoutLine(object Position) { this.Position = Position; }
        public object Position;
        public RectangleF Extent;
        public LayoutSpan[] Spans;
    }

    /// <summary>
    /// Horizontal span of text
    /// </summary>
    public class LayoutSpan
    {
        public RectangleF ContentExtent;
        public RectangleF PaddedExtent;
        public string Text;
        public string Style;
        public object Tag;
        public int ContextMenuID;
    }
}

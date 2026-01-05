#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Avalonia;
using Avalonia.Media;
using Reko.Gui.TextViewing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Reko.UserInterfaces.AvaloniaUI.Controls
{
    /// <summary>
    /// Maintains the layout of the client area of a TextView.
    /// </summary>
    public class TextViewLayout
    {
        private ITextViewModel model;
        private SortedList<double, LayoutLine> visibleLines;
        private Typeface defaultFont;
        private double defaultFontSize;

        public TextViewLayout(ITextViewModel model, Typeface defaultFont, double defaultFontSize)
        {
            this.model = model;
            this.visibleLines = new SortedList<double, LayoutLine>();
            this.defaultFont = defaultFont;
            this.defaultFontSize = defaultFontSize;
        }

        private TextViewLayout(ITextViewModel model, Typeface defaultFont, double defaultFontSize,  SortedList<double, LayoutLine> visibleLines)
        {
            this.model = model;
            this.defaultFont = defaultFont;
            this.defaultFontSize = defaultFontSize;
            this.visibleLines = visibleLines;
        }

        public IList<LayoutLine> LayoutLines =>  visibleLines.Values;

        /// <summary>
        /// Generates a TextViewLayout from all the lines in the model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="g"></param>
        /// <param name="defaultFont"></param>
        /// <param name="styleStack"></param>
        /// <returns></returns>
        public static TextViewLayout AllLines(ITextViewModel model, DrawingContext g, Typeface defaultFont, double defaultFontSize, StyleStack styleStack)
        {
            model.MoveToLine(model.StartPosition, 0);
            var rcLine = new Rect();
            var builder = new Builder(model, styleStack, defaultFont, defaultFontSize);
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


        public static TextViewLayout VisibleLines(ITextViewModel model, Size size, double defaultFontSize, Typeface defaultFont, StyleStack styleStack)
        {
            var szClient = new Size(size.Width, size.Height);
            var rcLine = new Rect(0, 0, szClient.Width, 0);

            var builder = new Builder(model, styleStack, defaultFont, defaultFontSize);
            while (rcLine.Top < szClient.Height)
            {
                var lines = model.GetLineSpans(1);
                if (lines.Length == 0)
                    break;
                builder.AddLayoutLine(lines[0], ref rcLine);
            }
            return builder.Build();
        }

        public Size CalculateExtent()
        {
            if (visibleLines.Count == 0)
                return new Size(0, 0);
            var width = visibleLines.Values.Max(l => l.Extent.Width);
            var height = visibleLines.Values.Select(l => l.Extent.Bottom).LastOrDefault();
            return new Size(width, height);
        }

        public TextPointer ClientToLogicalPosition(Point pt, StyleStack styleStack)
        {
            foreach (var line in this.visibleLines.Values)
            {
                if (line.Extent.Top <= pt.Y && pt.Y < line.Extent.Bottom)
                {
                    return FindSpanPosition(pt, line, styleStack);
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

        private TextPointer FindSpanPosition(Point ptClient, LayoutLine line, StyleStack styleStack)
        {
            int iSpan = 0;
            foreach (var span in line.Spans)
            {
                if (span.ContentExtent.Contains(ptClient))
                {
                    int iChar = GetCharPosition(ptClient, span, styleStack);
                    return new TextPointer(line.Position, iSpan, iChar);
                }
                ++iSpan;
            }
            return new TextPointer(line.Position, iSpan, 0);
        }

        private int GetCharPosition(Point ptClient, LayoutSpan span, StyleStack styleStack)
        {
            var x = ptClient.X - span.ContentExtent.Left;
            var textStub = span.Text;
            int iLow = 0;
            int iHigh = textStub.Length;
            styleStack.PushStyle(span.Style);
            var font = styleStack.GetFont(defaultFont);
            var sz = MeasureText(textStub, font);
            double xLow = 0;
            double xHigh = sz.Width;
            while (iLow < iHigh - 1)
            {
                int iMid = iLow + (iHigh - iLow) / 2;
                textStub = span.Text.Substring(0, iMid);
                sz = MeasureText(textStub, font);
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

        public Size MeasureText(string text, Typeface font)
        {
            var ft = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                font,
                10.0, // emsize
                null);
            var sz = new Size(ft.Width, ft.Height);
            return sz;
        }

        public Rect LogicalPositionToClient(TextPointer pos, StyleStack styleStack)
        {
            foreach (var line in this.visibleLines.Values)
            {
                if (line.Position == pos.Line)
                {
                    return SpanPositionToClient(pos, line, styleStack);
                }
            }
            return new Rect(new Point(0, 0), CalculateExtent());
        }

        private Rect SpanPositionToClient(TextPointer pos, LayoutLine line, StyleStack styleStack)
        {
            var iSpan = pos.Span;
            if (iSpan < 0 || iSpan >= line.Spans.Length)
                return line.Extent;
            var span = line.Spans[iSpan];
            return CharPositionToClient(pos, span, styleStack);
        }

        private Rect CharPositionToClient(TextPointer pos, LayoutSpan span, StyleStack styleStack)
        {
            var iChar = pos.Character;

            styleStack.PushStyle(span.Style);
            var font = styleStack.GetFont(defaultFont);

            var textStub = span.Text.Substring(0, iChar);
            var sz = MeasureText(textStub, font);
            var x = sz.Width + span.ContentExtent.Left;
            var width = 1;

            styleStack.PopStyle();

            return new Rect(x, span.ContentExtent.Top, width, span.ContentExtent.Height);
        }


        private class Builder
        {
            private ITextViewModel model;
            private StyleStack styleStack;
            private Typeface defaultFont;
            private double defaultFontSize;
            private SortedList<double, LayoutLine> visibleLines;

            public Builder(ITextViewModel model, StyleStack styleStack, Typeface defaultFont, double defaultFontSize)
            {
                this.model = model;
                this.styleStack = styleStack;
                this.defaultFont = defaultFont;
                this.defaultFontSize = defaultFontSize;
                this.visibleLines = new SortedList<double, LayoutLine>();
            }

            public void AddLayoutLine(LineSpan line, ref Rect rcLine /* put in state */)
            {
                double cyLine = MeasureLineHeight(line);
                rcLine = rcLine.WithHeight(cyLine);
                var spans = ComputeSpanLayouts(line.TextSpans, rcLine);
                var ll = new LayoutLine(line.Position, line.Tag, line.Style)
                {
                    Extent = LineExtent(rcLine, spans),
                    Spans = spans,
                };
                try
                {
                    this.visibleLines.Add(rcLine.Top, ll);
                } catch
                {
                    MeasureLineHeight(line);
                }
                rcLine = rcLine.Translate(new(0, cyLine));
            }

            private Rect LineExtent(Rect rcLine, LayoutSpan[] spans)
            {
                var r = rcLine.Right;
                if (spans.Length > 0)
                {
                    r = Math.Max(r, spans[spans.Length - 1].ContentExtent.Right);
                }
                return new Rect(rcLine.X, rcLine.Y, r - rcLine.X, rcLine.Height);
            }

            private double MeasureLineHeight(LineSpan line)
            {
                double height = 0.0;
                foreach (var span in line.TextSpans)
                {
                    styleStack.PushStyle(span.Style);
                    var font = styleStack.GetFont(defaultFont);
                    var fontSize = styleStack.GetFontSize(defaultFontSize);
                    height = Math.Max(
                        height,
                        fontSize + styleStack.GetNumber(s => s.PaddingTop) +
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
            private LayoutSpan[] ComputeSpanLayouts(IEnumerable<ITextSpan> spans, in Rect rcLine)
            {
                var spanLayouts = new List<LayoutSpan>();
                var pt = new Point(rcLine.Left, rcLine.Top);
                foreach (TextSpan span in spans)
                {
                    styleStack.PushStyle(span.Style);
                    var text = span.GetText();
                    var font = styleStack.GetFont(defaultFont);
                    var fontSize = styleStack.GetFontSize(defaultFontSize);
                    var szText = GetSize(span, text, font, fontSize);
                    var rc = new Rect(pt, szText);
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
                    pt = pt.WithX(pt.X + rcPadded.Width);
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
            private Size GetSize(TextSpan span, string text, Typeface font, double fontSize)
            {
                var size = span.GetSize(text, font, fontSize);
                int? width = styleStack.GetWidth();
                if (width.HasValue)
                {
                    size = size.WithWidth(width.Value);
                }
                return size;
            }

            public TextViewLayout Build()
            {
                return new TextViewLayout(model, defaultFont, defaultFontSize, visibleLines);
            }
        }
    }

    /// <summary>
    /// Line of spans.
    /// </summary>
    public class LayoutLine
    {
        public LayoutLine(object Position, object tag, string style) {
            this.Position = Position;
            this.Tag = tag;
            this.Style = style;
        }

        public object Position;
        public object Tag;                  // extra object for this line.
        public string Style;
        public Rect Extent;
        public LayoutSpan[] Spans;
    }

    /// <summary>
    /// Horizontal span of text
    /// </summary>
    public class LayoutSpan
    {
        public Rect ContentExtent;
        public Rect PaddedExtent;
        public string Text;
        public string Style;
        public object? Tag {
            get { return tag; }
            set
            {
                if (value is Reko.Core.Address)
                    value.ToString();
                if (value is Reko.Core.Block)
                    value.ToString();
                tag = value;
            }
        }
        private object? tag;

        public int ContextMenuID;
    }
}

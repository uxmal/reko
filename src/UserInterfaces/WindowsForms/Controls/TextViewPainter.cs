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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Paints the client area of a TextView.
    /// </summary>
    public class TextViewPainter
    {
        private TextViewLayout outer;
        private Graphics graphics;
        private TextPointer selStart;
        private TextPointer selEnd;
        private RectangleF rcContent;
        private RectangleF rcTotal;
        private LayoutSpan span;
        private StyleStack styleStack;
        private Color fg;
        private SolidBrush bg;
        private Font font;
        private SizeF extent;
        //$REVIEW: put these in the stylestack?
        private Color defaultFgColor;
        private Color defaultBgColor;
        private Font defaultFont;
        private bool useGdiPlus;
        private LayoutLine line;

        public TextViewPainter(TextViewLayout outer, Graphics g, Color fgColor, Color bgColor, Font defaultFont, StyleStack styleStack)
        {
            this.outer = outer;
            this.graphics = g;
            this.defaultFgColor = fgColor;
            this.defaultBgColor = bgColor;
            this.defaultFont = defaultFont;
            this.styleStack = styleStack;
            this.useGdiPlus = false;
        }

        public void SetSelection(TextPointer start, TextPointer end)
        {
            this.selStart = start;
            this.selEnd = end;
        }

        // Use the more accurate GDI text renderer, but can't participate in 
        // GDI+ scaling transforms.
        public void PaintGdi()
        {
            this.useGdiPlus = false;
            extent = outer.CalculateExtent();
            foreach (var line in outer.LayoutLines)
            {
                PaintLine(line);
            }
            PaintUnusedBackground();
        }

        // Use the GDI+ text renderer, which scales correctly but doesn't
        // measure text accurately.
        public void PaintGdiPlus()
        {
            this.useGdiPlus = true;
            extent = outer.CalculateExtent();
            foreach (var line in outer.LayoutLines)
            {
                PaintLine(line);
            }
            PaintUnusedBackground();
        }

        private void PaintLine(LayoutLine line)
        {
            this.line = line;
            this.styleStack.PushStyle(line.Style);

            // Paint the last piece of the line
            RectangleF rcTrailer = line.Extent;
            float xMax = 0;
            if (line.Spans.Length > 0)
            {
                xMax = line.Spans[line.Spans.Length - 1].ContentExtent.Right;
            }
            var cx = extent.Width - xMax;
            if (cx > 0)
            {
                rcTrailer.X = xMax;
                rcTrailer.Width = cx;
                graphics.FillRectangle(
                    styleStack.GetBackground(defaultBgColor),
                    rcTrailer);
            }

            for (int iSpan = 0; iSpan < line.Spans.Length; ++iSpan)
            {
                this.span = line.Spans[iSpan];
                this.styleStack.PushStyle(span.Style);
                var pos = new TextPointer(line.Position, iSpan, 0);

                var insideSelection =
                    outer.ComparePositions(selStart, pos) <= 0 &&
                    outer.ComparePositions(pos, selEnd) < 0;

                var hasTag = line.Tag is not null;
                this.fg = styleStack.GetForegroundColor(defaultFgColor);
                this.bg = styleStack.GetBackground(defaultBgColor);
                this.font = styleStack.GetFont(defaultFont);

                this.rcContent = span.ContentExtent;
                this.rcTotal = span.PaddedExtent;
                if (!insideSelection)
                {
                    if (selStart.Line == line.Position && selStart.Span == iSpan)
                    {
                        // Selection starts inside the current span. Write
                        // any unselected text first.
                        if (selStart.Character > 0)
                        {
                            DrawTextSegment(0, selStart.Character, false);
                        }
                        if (selEnd.Line == line.Position && selEnd.Span == iSpan)
                        {
                            // Selection ends inside the current span. Write
                            // selected text.
                            DrawTextSegment(selStart.Character, selEnd.Character - selStart.Character, true);
                            DrawTrailingTextSegment(selEnd.Character, false);
                        }
                        else
                        {
                            // Select all the way to the end of the span.
                            DrawTrailingTextSegment(selStart.Character, true);
                        }
                    }
                    else
                    {
                        // Not in selection at all.
                        DrawText(span.Text, false);
                    }
                }
                else
                {
                    // Inside selection. Does it end?
                    if (selEnd.Line == line.Position && selEnd.Span == iSpan)
                    {
                        // Selection ends inside the current span. Write
                        // selected text.
                        DrawTextSegment(0, selEnd.Character, true);
                        DrawTrailingTextSegment(selEnd.Character, false);
                    }
                    else
                    {
                        DrawText(span.Text, true);
                    }
                }

#if DEBUG
                var text = span.Text;
                if (line.Position == selStart.Line &&
                    iSpan == selStart.Span)
                {
                    var textFrag = text.Substring(0, selStart.Character);
                    var sz = outer.MeasureText(graphics, textFrag, font);
                    graphics.FillRectangle(
                        Brushes.Red,
                        span.ContentExtent.Left + sz.Width, line.Extent.Top,
                        1, line.Extent.Height);
                }
                if (line.Position == selEnd.Line &&
                    iSpan == selEnd.Span)
                {
                    var textFrag = text.Substring(0, selEnd.Character);
                    var sz = outer.MeasureText(graphics, textFrag, font);
                    graphics.FillRectangle(
                        Brushes.Blue,
                        span.ContentExtent.Left + sz.Width, line.Extent.Top,
                        1, line.Extent.Height);
                }
#endif
                styleStack.PopStyle();
            }
            styleStack.PopStyle();
        }

        private void PaintUnusedBackground()
        {
            using (var brBg = new SolidBrush(defaultBgColor))
            {
                var rc = graphics.ClipBounds;
                graphics.FillRectangle(brBg, 0, extent.Height, rc.Width, rc.Bottom - extent.Height);
            }
        }

        private void DrawTextSegment(int iStart, int iEnd, bool selected)
        {
            var textStub = span.Text.Substring(iStart, iEnd);
            var sz = outer.MeasureText(graphics, textStub, font);
            var oldWidth = rcContent.Width;
            rcContent.Width = sz.Width;
            rcTotal.Width = sz.Width;
            DrawText(textStub, selected);
            rcContent.Width = oldWidth - sz.Width;
            rcTotal.Width = oldWidth - sz.Width;
        }

        private void DrawTrailingTextSegment(int iStart, bool selected)
        {
            var textStub = span.Text.Substring(iStart);
            DrawText(textStub, selected);
        }

        /// <summary>
        /// Draws the entire string inside the current rectangle, then advances it.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="selected"></param>
        private void DrawText(string text, bool selected)
        {
            graphics.FillRectangle(
                selected ? SystemBrushes.Highlight : bg,
                rcTotal);

            if (rcTotal.Bottom < line.Extent.Bottom)
            {
                graphics.FillRectangle(
                    bg,
                    rcTotal.Left, rcTotal.Bottom,
                    rcTotal.Width, line.Extent.Bottom - rcTotal.Bottom);
            }

            Color color = selected ? SystemColors.HighlightText : fg;
            if (useGdiPlus)
            {
                var brush = new SolidBrush(color);
                graphics.DrawString(
                    text,
                    this.font,
                    brush,
                    rcContent.Left,
                    rcContent.Top,
                    StringFormat.GenericTypographic);
                brush.Dispose();
            }
            else
            {
                var pt = new Point((int)rcContent.X, (int)rcContent.Y);
                TextRenderer.DrawText(
                    this.graphics,
                    text,
                    this.font,
                    pt,
                    color,
                    TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            }
            rcContent.X += rcContent.Width;
            rcTotal.X += rcContent.Width;
        }
    }
}

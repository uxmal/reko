#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Controls
{
    public partial class TextView
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Services == null)
                return;
            GetStyleStack().PushStyle(StyleClass);
            var painter = new Painter(this, e.Graphics, styleStack);
            painter.Paint();
            GetStyleStack().PopStyle();
        }

        private class Painter
        {
            private TextView outer;
            private Graphics graphics;
            private TextPointer selStart;
            private TextPointer selEnd;
            private RectangleF rcText;
            private LayoutSpan span;
            private StyleStack styleStack;
            private Color fg;
            private SolidBrush bg;
            private Font font;

            public Painter(TextView outer, Graphics g, StyleStack styleStack)
            {
                this.outer = outer;
                this.graphics = g;
                this.styleStack = styleStack;

                selStart = outer.GetStartSelection();
                selEnd = outer.GetEndSelection();
            }

            public void Paint()
            {
                Debug.Print("Selection: {0} - {1}", selStart, selEnd);
                foreach (var line in outer.visibleLines.Values)
                {
                    PaintLine(line);
                }
            }

            private void PaintLine(LayoutLine line)
            {
                for (int iSpan = 0; iSpan < line.Spans.Length; ++iSpan)
                {
                    this.span = line.Spans[iSpan];
                    if (!string.IsNullOrEmpty(span.Style))
                        this.styleStack.PushStyle(span.Style);
                    var pos = new TextPointer { Line = line.Position, Span = iSpan, Character = 0 };

                    var insideSelection =
                        outer.ComparePositions(selStart, pos) <= 0 &&
                        outer.ComparePositions(pos, selEnd) < 0;

                    this.fg = styleStack.GetForegroundColor(outer);
                    this.bg = styleStack.GetBackground(outer);
                    this.font = styleStack.GetFont(outer);

                    this.rcText = span.Extent;
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
                                if (selEnd.Character < span.Text.Length)
                                {
                                    // If there is trailing unselected text, display that.
                                    DrawTrailingTextSegment(selEnd.Character, false);
                                }
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
                            span.Extent.Left + sz.Width, line.Extent.Top,
                            1, line.Extent.Height);
                    }
                    if (line.Position == selEnd.Line &&
                        iSpan == selEnd.Span)
                    {
                        var textFrag = text.Substring(0, selEnd.Character);
                        var sz = outer.MeasureText(graphics, textFrag, font);
                        graphics.FillRectangle(
                            Brushes.Blue,
                            span.Extent.Left + sz.Width, line.Extent.Top,
                            1, line.Extent.Height);
                    }
#endif
                    if (!string.IsNullOrEmpty(span.Style))
                        styleStack.PopStyle();
                }
            }

            private void DrawTextSegment(int iStart, int iEnd, bool selected)
            {
                var textStub = span.Text.Substring(iStart, iEnd);
                var sz = outer.MeasureText(graphics, textStub, font);
                rcText.Width = sz.Width;
                DrawText(textStub, selected);
            }

            private void DrawTrailingTextSegment(int iStart, bool selected)
            {
                var textStub = span.Text.Substring(iStart);
                rcText.Width = span.Extent.Width - (rcText.X - span.Extent.Left);
                DrawText(textStub, selected);
            }

            private void DrawText(string text, bool selected)
            {
                graphics.FillRectangle(
                    selected ? SystemBrushes.Highlight : bg,
                    rcText);
                var pt = new Point((int)rcText.X, (int)rcText.Y);
                TextRenderer.DrawText(
                    this.graphics,
                    text,
                    this.font,
                    pt,
                    selected ? SystemColors.HighlightText : fg,
                    TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                rcText.X += rcText.Width;
            }
        }
    }
}
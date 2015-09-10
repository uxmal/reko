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
            Position selStart;
            Position selEnd;
            var painter = new Painter(this, e.Graphics);
            painter.Paint();
        }

        private class Painter
        {
            TextView outer;
            Graphics graphics;
            Position selStart;
            Position selEnd;
            public Painter(TextView outer, Graphics g)
            {
                this.outer = outer;
                this.graphics = g;

                if (outer.ComparePositions(outer.cursorPos, outer.anchorPos) < 0)
                {
                    selStart = outer.cursorPos;
                    selEnd = outer.anchorPos;
                }
                else
                {
                    selStart = outer.anchorPos;
                    selEnd = outer.cursorPos;
                }
            }

            public void Paint()
            {
                foreach (var line in outer.visibleLines.Values)
                {
                    PaintLine(line);
                }
            }


            private void PaintLine(LayoutLine line)
            {
                for (int iSpan = 0; iSpan < line.Spans.Length; ++iSpan)
                {
                    var span = line.Spans[iSpan];
                    var text = span.Text;
                    var font = outer.GetFont(span.Style);
                    var fg = outer.GetForegroundColor(span.Style);
                    var bg = outer.GetBackground(span.Style);
                    graphics.FillRectangle(bg, span.Extent);
                    var ptF = span.Extent.Location;
                    var pt = new Point((int)ptF.X, (int)ptF.Y);
                    TextRenderer.DrawText(graphics, text, font, pt, fg, TextFormatFlags.NoPadding);
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
                }
            }
        }
    }
}
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

using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public partial class ImageMapView
    {
        public class Painter
        {
            private const int CxMinSelection = 3;
            private static readonly Color FocusedSelectionBgColor = Color.FromArgb(0xFF, 0xFF, 0x30);
            private static readonly Color SelectionBgColor = Color.FromArgb(0x80, 0x80, 0x30);

            private readonly SegmentMap segmentMap;
            private readonly ImageMap imageMap;
            private readonly long granularity;
            private readonly Address addrMin;
            private readonly Address addrMax;
            public Rectangle rcClient;
            public Rectangle rcBody;
            public List<SegmentLayout> segLayouts;
            private Brush brCode;
            private Brush brBack;
            private Brush brData;

            public Painter(ImageMapView mapView)
            {
                this.imageMap = mapView.ImageMap;
                this.segmentMap = mapView.SegmentMap;
                this.granularity = mapView.granularity;
                var b = mapView.SelectedRange.Begin;
                var e = mapView.SelectedRange.End;
                this.addrMin = Address.Min(b, e);
                this.addrMax = Address.Max(b, e);
                segLayouts = new List<SegmentLayout>();
                long x = 0;
                if (imageMap is not null && granularity > 0)
                {
                    foreach (var segment in segmentMap.Segments.Values)
                    {
                        long cx = (segment.Size + granularity - 1) / granularity;
                        segLayouts.Add(new SegmentLayout
                        {
                            Segment = segment,
                            X = x,
                            CxWidth = cx
                        });
                        x += cx + CxSegmentBorder;
                    }
                }
                this.rcClient = mapView.ClientRectangle;
                rcBody = new Rectangle(
                    CxScroll, 0,
                    rcClient.Width - 2 * CxScroll,
                    rcClient.Height - CySelection);
                Extent = (int)x;
            }

            public int Extent { get; private set; }

            public void Paint(Graphics g, ImageMapView mapView)
            {
                this.brBack = new SolidBrush(mapView.BackColor);
                this.brCode = new SolidBrush(Color.Pink);
                this.brData = new SolidBrush(Color.LightBlue);
                try
                {
                    RenderScrollControls(g, mapView);
                    RenderBody(g, mapView.cxOffset, mapView.Focused);
                }
                finally
                {
                    brData.Dispose();
                    brCode.Dispose();
                    brBack.Dispose();
                }
            }

            public void RenderBody(Graphics g, long cxOffset, bool focused)
            {
                using Font fontSeg = new Font("Arial", 7);
                Rectangle rcPaint = rcBody;
                rcPaint.Width = 0;
                Brush brNew = null;
                long? xSelectionBegin = default;
                long? xSelectionEnd = default;
                var transparentOverlay = Color.FromArgb(0x60, SystemColors.Highlight);
                using Brush brOverlay = new SolidBrush(transparentOverlay);
                //Debug.Print("== RenderBody ==");
                foreach (var sl in segLayouts)
                {
                    if (sl.Segment.IsInRange(addrMin))
                        xSelectionBegin = sl.AddressToX(addrMin, granularity);
                    if (sl.Segment.IsInRange(addrMax))
                        xSelectionEnd = sl.AddressToX(addrMax, granularity);
                    if (sl.X - cxOffset < rcBody.Width && sl.X + sl.CxWidth - cxOffset >= 0)
                    {
                        //Debug.Print("---- Segment {0} ----", sl.Segment.Name);

                        int xMin = (int)Math.Max(sl.X - cxOffset, 0);
                        int xMax = (int)Math.Min(sl.X + sl.CxWidth - cxOffset, rcBody.Width);
                        long cbOffset = (xMin - (sl.X - cxOffset)) * granularity;
                        rcPaint.X = xMin + CxScroll;
                        Brush brOld = null;
                        for (int x = xMin; x < xMax; ++x, cbOffset += granularity)
                        {
                            brNew = GetColorForOffset(sl.Segment, cbOffset);
                            if (brNew != brOld)
                            {
                                if (brOld is not null)
                                {
                                    rcPaint.Width = x + CxScroll - rcPaint.X;
                                    g.FillRectangle(brOld, rcPaint);
                                    rcPaint.X = x + CxScroll;
                                }
                                brOld = brNew;
                            }
                        }
                        if (brNew is not null)
                        {
                            rcPaint.Width = xMax + CxScroll - rcPaint.X;
                            g.FillRectangle(brNew, rcPaint);
                        }
                        RenderSelectionOverlay(g, xSelectionBegin, xSelectionEnd, xMin, xMax, rcBody.Y, rcBody.Height, brOverlay);
                        RenderSegmentName(g, sl, xMin, xMax, fontSeg);
                        RenderSegmentSeparator(g, CxScroll + sl.X + sl.CxWidth - cxOffset);
                    }
                    if (xSelectionBegin.HasValue && xSelectionEnd.HasValue)
                    {
                        RenderSelectionBar(g, xSelectionBegin.Value, xSelectionEnd.Value, focused);
                    }
                }
            }

            private void RenderSelectionOverlay(
                Graphics g,
                long? xSelectionBegin,
                long? xSelectionEnd,
                int xMin, int xMax,
                int y, int height,
                Brush brOverlay)
            {
                if (!xSelectionBegin.HasValue || !xSelectionEnd.HasValue)
                    return;
                var xBegin = (int) xSelectionBegin.Value;
                var xEnd = (int) xSelectionEnd.Value;
                trace.Verbose($"[{xBegin}-{xEnd}): min: {xMin}, max: {xMax}");
                if (xBegin >= xMax || xEnd < xMin)
                    return;
                xBegin = Math.Max(xMin, xBegin);
                xEnd = Math.Min(xMax, xEnd);
                (xBegin, xEnd) = NormalizeSelection(xBegin, xEnd);
                trace.Verbose($"Painting overlay: ({xBegin}, {xEnd-xBegin}, {y}, {height})");
                g.FillRectangle(brOverlay, CxScroll + xBegin, y, xEnd - xBegin, height);
            }

            private void RenderSegmentName(Graphics g, SegmentLayout sl, int xMin, int xMax, Font font)
            {
                var size = g.MeasureString(sl.Segment.Name, font);
                var rcText = new RectangleF(CxScroll + xMin + 1, rcBody.Bottom - (int)size.Height, size.Width, size.Height);
                g.DrawString(sl.Segment.Name, font, SystemBrushes.ControlText, rcText);
            }

            private void RenderSegmentSeparator(Graphics g, long x)
            {
                var rc = new Rectangle((int)x, rcBody.Top, CxSegmentBorder, rcBody.Height);
                g.FillRectangle(SystemBrushes.ControlText, rc);
                //Debug.Print("Separator: {0}", rc);
            }

            private void RenderSelectionBar(Graphics g, long xSelectionBegin, long xSelectionEnd, bool focused)
            {
                var yTop = 1 + rcClient.Height - CySelection;
                var cyHeight = CySelection - 1;
                using Brush brBg = new SolidBrush(focused ? FocusedSelectionBgColor : SelectionBgColor);
                g.FillRectangle(brBg, new Rectangle(0, yTop, rcClient.Width, cyHeight));

                var (xSelBegin, xSelEnd) = NormalizeSelection((int) xSelectionBegin, (int) xSelectionEnd);
                var cxSelection = xSelEnd - xSelBegin;
                var brSel = focused ? SystemBrushes.Highlight : SystemBrushes.InactiveCaption;
                g.FillRectangle(brSel, CxScroll + xSelBegin, yTop, cxSelection, cyHeight);
            }

            private void RenderScrollControls(Graphics g, ImageMapView mapView)
            {
                int cbTotal = 0;
                var baseState = imageMap is not null && cbTotal > granularity * rcClient.Width
                    ? ButtonState.Flat
                    : ButtonState.Flat | ButtonState.Inactive;
                ControlPaint.DrawScrollButton(g,
                    0, (rcClient.Height - CyScroll) / 2,
                    CxScroll,
                    CyScroll,
                    ScrollButton.Left,
                    (mapView.scrollButton == ScrollButton.Left ? ButtonState.Pushed : ButtonState.Normal) | baseState);
                ControlPaint.DrawScrollButton(g,
                    rcClient.Width - CxScroll,
                    (rcClient.Height - CyScroll) / 2,
                    CxScroll,
                    CyScroll,
                    ScrollButton.Right,
                    (mapView.scrollButton == ScrollButton.Left ? ButtonState.Pushed : ButtonState.Normal) | baseState);
            }

            /// <summary>
            /// Returns the layout of the segment that contains the given
            /// address <paramref name="addr"/>. A missing address yields
            /// a null value.
            /// </summary>
            /// <param name="addr"></param>
            /// <returns></returns>
            public SegmentLayout GetSegment(Address addr)
            {
                return segLayouts.FirstOrDefault(s => s.Segment.IsInRange(addr));
            }

            /// <summary>
            /// Normalize the selection so that it is at least <see cref="CxMinSelection"/> pixels
            /// wide.
            /// </summary>
            /// <param name="xBegin">Start X coordinate of selection.</param>
            /// <param name="xEnd">End X coordinate of selection.</param>
            /// <returns>A tuple consisting of <paramref name="xBegin"/> and <paramref name="xEnd"/>,
            /// possibly adjusted so that the difference between the values is at least <see cref="CxMinSelection"/>.
            /// </returns>
            private (int, int) NormalizeSelection(int xBegin, int xEnd)
            {
                if (xEnd - xBegin < CxMinSelection)
                {
                    xBegin -= CxMinSelection / 2;
                    xEnd = xBegin + CxMinSelection;
                }
                return (xBegin, xEnd);
            }

            private Brush GetColorForOffset(ImageSegment seg, long cbOffset)
            {
                ImageMapItem item;
                var lin = seg.Address.ToLinear() + (uint)cbOffset;
                if (!seg.IsInRange(lin))
                    return brBack;
                var address = segmentMap.MapLinearAddressToAddress(lin);
                if (!imageMap.TryFindItem(address, out item))
                    return brBack;
                if (item is ImageMapVectorTable)
                    return brData;
                if (item.DataType is UnknownType)
                {
                    if (item.DataType.Size > 0)
                        return brData;
                    else
                        return brBack;
                }
                if (item is ImageMapBlock)
                    return brCode;
                else
                    return brData;
            }
        }
    }
}

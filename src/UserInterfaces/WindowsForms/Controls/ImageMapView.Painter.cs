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

using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public partial class ImageMapView
    {
        public class Painter
        {
            private SegmentMap segmentMap;
            private ImageMap imageMap;
            public Rectangle rcClient;
            public Rectangle rcBody;
            public List<SegmentLayout> segLayouts;
            private long granularity;
            private Brush brCode;
            private Brush brBack;
            private Brush brData;

            public Painter(ImageMapView mapView)
            {
                this.imageMap = mapView.ImageMap;
                this.segmentMap = mapView.SegmentMap;
                this.granularity = mapView.granularity;

                segLayouts = new List<SegmentLayout>();
                long x = 0;
                long cx = 0;
                if (imageMap != null && granularity > 0)
                {
                    foreach (var segment in segmentMap.Segments.Values)
                    {
                        cx = (segment.Size + granularity - 1) / granularity;
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
                    RenderSelectionBar(g, mapView.Focused);
                    RenderScrollControls(g, mapView);
                    RenderBody(g, mapView.cxOffset);
                }
                finally
                {
                    brData.Dispose();
                    brCode.Dispose();
                    brBack.Dispose();
                }
            }

            public void RenderBody(Graphics g, long cxOffset)
            {
                Font fontSeg = new Font("Arial", 7);
                Rectangle rcPaint = rcBody;
                rcPaint.Width = 0;
                Brush brNew = null;
                //Debug.Print("== RenderBody ==");
                foreach (var sl in segLayouts)
                {
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
                                if (brOld != null)
                                {
                                    rcPaint.Width = x + CxScroll - rcPaint.X;
                                    g.FillRectangle(brOld, rcPaint);
                                    //Debug.Print("Paint: {0} {1}", rcPaint, brOld);
                                    brOld = brNew;
                                    rcPaint.X = x + CxScroll;
                                }
                                else
                                {
                                    brOld = brNew;
                                }
                            }
                        }
                        if (brNew != null)
                        {
                            rcPaint.Width = xMax + CxScroll - rcPaint.X;
                            g.FillRectangle(brNew, rcPaint);
                        }

                        RenderSegmentName(g, sl, xMin, xMax, fontSeg);
                        RenderSegmentSeparator(g, CxScroll + sl.X + sl.CxWidth - cxOffset);
                    }
                }
                fontSeg.Dispose();
                //Debug.Print("== RenderBody ==");
            }

            private void RenderSegmentName(Graphics g, SegmentLayout sl, int xMin, int xMax, Font font)
            {
                var size = g.MeasureString(sl.Segment.Name, font);
                var rcText = new RectangleF(CxScroll + xMin + 1, rcBody.Bottom - (int)size.Height, size.Width, size.Height);
                g.DrawString(sl.Segment.Name, font, SystemBrushes.ControlText, rcText);
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
                if (addr == null)
                    return null;
                return segLayouts.FirstOrDefault(s => s.Segment.IsInRange(addr));
            }

            private void RenderSegmentSeparator(Graphics g, long x)
            {
                var rc = new Rectangle((int)x, rcBody.Top, CxSegmentBorder, rcBody.Height);
                g.FillRectangle(Brushes.Black, rc);
                //Debug.Print("Separator: {0}", rc);
            }

            private void RenderSelectionBar(Graphics g, bool focused)
            {
                Brush br = new SolidBrush(focused ? Color.FromArgb(0xFF, 0xFF, 0x30) : Color.FromArgb(0x80, 0x80, 0x30));
                g.FillRectangle(br, new Rectangle(0, 1 + rcClient.Height - CySelection,  rcClient.Width, CySelection - 1));
                br.Dispose();
            }

            private void RenderScrollControls(Graphics g, ImageMapView mapView)
            {
                int cbTotal = 0;
                var baseState = imageMap != null && cbTotal > granularity * rcClient.Width
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
                    return brBack;
                if (item is ImageMapBlock)
                    return brCode;
                else
                    return brData;
            }
        }
    }
}

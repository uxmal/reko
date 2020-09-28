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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public partial class ImageMapView : Control
    {
        private const float ZoomOutFactor = 1.25F;
        private const float ZoomInFactor = 4.0F / 5.0F;
        private const int CySelection = 3;
        private const int CxScroll = 16;            // horizontal space for scrollers
        private const int CxSegmentBorder = 1;      // border between segments.
        private const int CyScroll = 16;
        private const int ScrollStep = 8;

        public ImageMapView()
        {
            InitializeComponent();
            scrollTimer.Tick += scrollTimer_Tick;
            xLastMouseUp = CxScroll;
        }

        public class SegmentLayout
        {
            public ImageSegment Segment;
            public long X;
            public long CxWidth;
        }

        public long Granularity { get { return granularity; } set { BoundGranularity(value); BoundOffset(cxOffset); OnGranularityChanged(); } }
        public event EventHandler GranularityChanged;
        private long granularity;

        [Browsable(false)]
        public ImageMap ImageMap {
            get { return imageMap; } 
            set { 
                if (imageMap != null)
                    imageMap.MapChanged -= imageMap_MapChanged;
                imageMap = value; 
                if (imageMap != null)
                    imageMap.MapChanged += imageMap_MapChanged;
                OnImageMapChanged(); 
            } 
        }
        public event EventHandler ImageMapChanged;
        public ImageMap imageMap;

        [Browsable(false)]
        public SegmentMap SegmentMap
        {
            get { return segmentMap; }
            set
            {
                if (segmentMap != null)
                    segmentMap.MapChanged -= imageMap_MapChanged;
                segmentMap = value;
                if (segmentMap != null)
                    segmentMap.MapChanged += imageMap_MapChanged;
                OnSegmentMapChanged();
            }
        }
        public event EventHandler SegmentMapChanged;
        private SegmentMap segmentMap;


        [Browsable(false)]
        public Address SelectedAddress { get { return selectedAddress; } set { selectedAddress = value; SelectedAddressChanged.Fire(this); } }
        public event EventHandler SelectedAddressChanged;
        private Address selectedAddress;

        public long Offset { get { return cxOffset; } set { BoundOffset(value); OnOffsetChanged(); } }
        public event EventHandler OffsetChanged;
        private long cxOffset;

        private ScrollButton scrollButton;      // If we're scrolling the scrollbuttons.
        private int xLastMouseUp;
        private Painter painter;

        protected override void OnPaint(PaintEventArgs pe)
        {
            this.painter = CalculateLayout();
            painter.Paint(pe.Graphics, this);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        public Painter CalculateLayout()
        {
            return new Painter(this);
        }

        long AddressableExtent(ImageSegment seg)
        {
            var addrMin = Address.Max(seg.Address, seg.MemoryArea.BaseAddress);
            var addrMax = Address.Min(seg.Address + seg.ContentSize, seg.MemoryArea.EndAddress);
            return addrMax - addrMin;
        }

        private void BoundGranularity(long value)
        {
            granularity = Math.Max(1L, value);
            if (ImageMap == null || SegmentMap == null)
            {
                return;
            }

            int cxAvailable = ClientSize.Width - 2 * CxScroll;
            int cxConstant = 0;             // pixels always needed
            int cxConstantPerSegment = CxSegmentBorder; // constant pixel overhead per segment
            int cSeg = SegmentMap.Segments.Count;
            long cbTotal = SegmentMap.Segments.Values.Sum(s => s.Size);
            granularity = (long)Math.Ceiling(
               cbTotal /
               (double)(cxAvailable - cxConstant - cxConstantPerSegment * cSeg));
            granularity = Math.Min(value, granularity);
        }

        private void BoundOffset(long value)
        {
            cxOffset = value;
            if (painter != null)
            {
                cxOffset = Math.Min(cxOffset, painter.Extent);
            }
            cxOffset = Math.Max(0, cxOffset);
        }

        /// <summary>
        /// Zoom in or out by a factor of <paramref name="factor"/>. Try to 
        /// keep the last selected position visible in the same position if
        /// at all position.
        /// </summary>
        /// <param name="factor"></param>
        private void Zoom(float factor)
        {
            if (imageMap == null)
                return;
            var addr = MapClientPositionToAddress(xLastMouseUp);
            var oldGranularity = Granularity;
            var newGranularity = (long)Math.Ceiling(Granularity * factor);
            BoundGranularity(newGranularity);

            // We want addr to appear in the same position as it did last time, if possible.
            this.painter = CalculateLayout();
            var iSeg = painter.GetSegment(addr);
            if (iSeg == null)
                return;

            long cxInsideSeg = (addr - iSeg.Segment.Address) / granularity;
            cxOffset = iSeg.X + cxInsideSeg - (xLastMouseUp - CxScroll);
            BoundOffset(cxOffset);
            Invalidate();
        }

        private void StartScrolling(ScrollButton button)
        {
            this.scrollTimer.Interval = 10; // msec
            this.scrollTimer.Start();
            this.scrollButton = button;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            if (!Capture)
                Capture = true;
            if (0 <= e.X && e.X < CxScroll)
            {
                Offset -= ScrollStep * Granularity;
                StartScrolling(ScrollButton.Left);
            }
            else if (Width - CxScroll <= e.X && e.X < Width)
            {
                Offset += ScrollStep *Granularity;
                StartScrolling(ScrollButton.Right);
            }
            else
            {
                // remember anchorDown, and if moved > 3 pixels, s
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            scrollTimer.Stop();
            if (Capture)
                Capture = false;
            xLastMouseUp = e.X;
            var addr = MapClientPositionToAddress(e.X);
            if (addr != null)
            {
                SelectedAddress = addr;
            }
            base.OnMouseUp(e);
        }

        private Address MapClientPositionToAddress(int x)
        {
            if (imageMap == null || painter == null)
                return null;

            x -= (CxScroll - (int) cxOffset);          // bias past the scroller button.
            foreach (var sl in painter.segLayouts)
            {
                if (sl.X <= x && x < sl.X + sl.CxWidth)
                {
                    return sl.Segment.Address + (x - sl.X) * granularity;
                }
            }
            return null;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Debug.Print("KeyDown: {0}", e.KeyData);
            switch (e.KeyData)
            {
            case Keys.Add:
                Zoom(ZoomInFactor);
                e.Handled = true;
                break;
            case Keys.Subtract:
                Zoom(ZoomOutFactor);
                e.Handled = true;
                break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
            case '+': 
                Zoom(ZoomInFactor);
                e.Handled = true;
                break;
            case '-':
                Zoom(ZoomOutFactor);
                e.Handled = true;
                break;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            BoundGranularity(granularity);
            CalculateLayout();
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected virtual void OnGranularityChanged()
        {
            CalculateLayout();
            Invalidate();
            GranularityChanged.Fire(this);
        }

        protected virtual void OnImageMapChanged()
        {
            BoundGranularity(granularity);
            Invalidate();
            ImageMapChanged.Fire(this);
        }

        protected virtual void OnSegmentMapChanged()
        {
            BoundGranularity(granularity);
            Invalidate();
            SegmentMapChanged.Fire(this);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Invalidate();
            base.OnLostFocus(e);
        }

        protected virtual void OnOffsetChanged()
        {
            Invalidate();
            OffsetChanged.Fire(this);
        }

        void scrollTimer_Tick(object sender, EventArgs e)
        {
            switch (scrollButton)
            { 
            case ScrollButton.Left: Offset -=  ScrollStep * Granularity; break;
            case ScrollButton.Right: Offset += ScrollStep * Granularity; break;
            }
        }

        void imageMap_MapChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Action(Invalidate));
            else
                Invalidate();
        }
    }
}

using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Controls
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
        private readonly List<SegmentLayout> segLayouts;

        public ImageMapView()
        {
            InitializeComponent();
            scrollTimer.Tick += scrollTimer_Tick;
            xLastMouseUp = CxScroll;
            segLayouts = new List<SegmentLayout>();
        }

        public class SegmentLayout
        {
            public ImageSegment Segment;
            public long X;
            public long Width;
        }

        /*
         *  n = number of segments
         *  c = constant width
         *  b = per-segment constant width
         *  cb[i] = per-segment size in bytes
         *  Width = c + sum(b + cb[i] * scale)
         *  Width - c - n * b = scale * sum(cb[i])
         *  scale = ceil((width - c - n * b) / sum(cb[i]))
         */
        public long Granularity { get { return granularity; } set { BoundGranularity(value); BoundOffset(offset); OnGranularityChanged(); } }
        public event EventHandler GranularityChanged;
        private long granularity;

        [Browsable(false)]
        private MemoryArea Image { get { return image; } set { image = value; OnImageChanged(); } }
        public MemoryArea image;

        [Browsable(false)]
        public ImageMap ImageMap {
            get { return imageMap; } 
            set { 
                if (imageMap != null)
                    imageMap.MapChanged -= imageMap_MapChanged;
                imageMap = value; 
                if (imageMap == null)
                {
                    this.cbTotal = 0L;
                }
                if (imageMap != null)
                    imageMap.MapChanged += imageMap_MapChanged;
                OnImageMapChanged(); 
            } 
        }
        public event EventHandler ImageMapChanged;
        public ImageMap imageMap;

        [Browsable(false)]
        public Address SelectedAddress { get { return selectedAddress; } set { selectedAddress = value; SelectedAddressChanged.Fire(this); } }
        public event EventHandler SelectedAddressChanged;
        private Address selectedAddress;

        public long Offset { get { return offset; } set { BoundOffset(value); OnOffsetChanged(); } }
        public event EventHandler OffsetChanged;
        private long offset;

        private Brush brCode;
        private Brush brBack;
        private Brush brData;

        private ScrollButton scrollButton;      // If we're scrolling the scrollbuttons.
        private int xLastMouseUp;
        private long cbTotal;

        protected override void OnPaint(PaintEventArgs pe)
        {
            this.brBack = new SolidBrush(BackColor);
            this.brCode = new SolidBrush(Color.Pink);
            this.brData = new SolidBrush(Color.LightBlue);
            try
            {
                Rectangle rcBody = CalculateLayout();
                RenderSelectionBar(pe.Graphics);
                RenderScrollControls(pe.Graphics, rcBody);
                RenderBody(pe.Graphics, rcBody);
            }
            finally
            {
                brData.Dispose();
                brCode.Dispose();
                brBack.Dispose();
            }
        }

        private void RenderSelectionBar(Graphics g)
        {
            Brush br = new SolidBrush(Focused ? Color.FromArgb(0xFF, 0xFF, 0x30) : Color.FromArgb(0x80, 0x80, 0x30));
            g.FillRectangle(br, new Rectangle(0, 1 + Height - CySelection, Width, CySelection - 1));
            br.Dispose();
        }

        private void RenderScrollControls(Graphics g, Rectangle rcBody)
        {
            var baseState = imageMap != null && cbTotal > granularity * Width 
                ? ButtonState.Flat
                : ButtonState.Flat | ButtonState.Inactive;
            ControlPaint.DrawScrollButton(g,
                0, (Height - CyScroll) / 2, 
                CxScroll, 
                CyScroll, 
                ScrollButton.Left,
                (scrollButton == ScrollButton.Left ? ButtonState.Pushed : ButtonState.Normal) | baseState); 
            ControlPaint.DrawScrollButton(g,
                Width - CxScroll,
                (Height - CyScroll) / 2, 
                CxScroll, 
                CyScroll,
                ScrollButton.Right, 
                (scrollButton == ScrollButton.Left ? ButtonState.Pushed : ButtonState.Normal) | baseState);
        }

        private void RenderBody(Graphics g, Rectangle rcBody)
        {
            Rectangle rcPaint = rcBody;
            rcPaint.Width = 0;
            Brush brNew = null;
            foreach (var sl in this.segLayouts)
            {
                Brush brOld = brBack;
                var segOffset = sl.Segment.Address.ToLinear() - imageMap.BaseAddress.ToLinear();
                if ((ulong)offset <= segOffset + sl.Segment.Size && segOffset < (ulong)(offset + rcBody.Width * granularity))
                {
                    long cbOffset = offset;
                    var cxOffset = (int)(cbOffset / granularity) + rcBody.Left;
                    var cxEnd = cxOffset + (int)sl.Width / granularity;
                    for (int x = cxOffset; x < cxEnd; ++x, cbOffset += granularity)
                    {
                        brNew = GetColorForOffset(cbOffset);
                        if (brNew != brOld)
                        {
                            rcPaint.Width = x - rcPaint.X;
                            g.FillRectangle(brOld, rcPaint);
                            brOld = brNew;
                            rcPaint.X = x;
                        }
                    }
                    if (brNew != null)
                    {
                        rcPaint.Width = (int) cxEnd - rcPaint.X;
                        g.FillRectangle(brNew, rcPaint);
                    }

                }
            }
            //for (int x = rcBody.X; x < rcBody.Right; ++x, cbOffset += granularity)
            //{
            //    brNew = GetColorForOffset(cbOffset);
            //    if (brNew != brOld)
            //    {
            //        rcPaint.Width = x - rcPaint.X;
            //        g.FillRectangle(brOld, rcPaint);
            //        brOld = brNew;
            //        rcPaint.X = x;
            //    }
            //}
            //if (brNew != null)
            //{
            //    rcPaint.Width = rcBody.Right - rcPaint.X;
            //    g.FillRectangle(brNew, rcPaint);
            //}
        }

        private Brush GetColorForOffset(long cbOffset)
        {
            ImageMapItem item;
            if (imageMap == null ||image == null)
                return brBack;
            var lin = (image.BaseAddress + cbOffset).ToLinear();
            if (!image.IsValidLinearAddress(lin))
                return brBack;
            var address = imageMap.MapLinearAddressToAddress(lin);
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

        private Rectangle CalculateLayout()
        {
            this.segLayouts.Clear();
            if (imageMap != null && image != null && granularity > 0)
            {
                long x = 0;
                long cx = 0;
                foreach (var segment in imageMap.Segments.Values)
                {
                    cx = (segment.Size + granularity - 1) / granularity;
                    segLayouts.Add(new SegmentLayout { Segment = segment, X = x, Width = cx });
                    x += cx + CxSegmentBorder;
                }
            }
            var rc = new Rectangle(
                CxScroll, 0,
                Width - 2 * CxScroll,
                Height - CySelection);
            return rc;
        }

        private void BoundGranularity(long value)
        {
            granularity = Math.Max(1L, value);
            if (image != null)
            {
                granularity = Math.Min(
                    granularity,
                    (long)Math.Ceiling((double)image.Bytes.Length / (double)Width));
            }
        }

        private void BoundOffset(long value)
        {
            offset = value;
            if (image != null)
            {
                offset = Math.Min(
                    offset,
                    image.Bytes.Length - Width * granularity);
            }
            offset = Math.Max(0, offset);
        }

        private void Zoom(float factor)
        {
            var oldGranularity = Granularity;
            var newGranularity = (int)Math.Ceiling(Granularity * factor);
            offset = offset + (oldGranularity - newGranularity) * (xLastMouseUp - CxScroll);
            Granularity = newGranularity;
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
            SelectedAddress = MapClientPositionToAddress(e.X);
            base.OnMouseUp(e);
        }

        private Address MapClientPositionToAddress(int x)
        {
            if (image == null || imageMap == null)
                return null;
            return  imageMap.MapLinearAddressToAddress( image.BaseAddress.ToLinear() + (ulong)((x - CxScroll) * granularity + offset));
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

        protected virtual void OnImageChanged()
        {
            CalculateLayout();
            Invalidate();
        }

        protected virtual void OnImageMapChanged()
        {
            CalculateLayout();
            Invalidate();
            ImageMapChanged.Fire(this);
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
                Invoke(new Action(Invalidate));
            else
                Invalidate();
        }
    }
}

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

        public class ControlLayout
        {
            public Rectangle rcBody;
            public List<SegmentLayout> segLayouts;
            public int cxExtent;
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

        public long Offset { get { return cxOffset; } set { BoundOffset(value); OnOffsetChanged(); } }
        public event EventHandler OffsetChanged;
        private long cxOffset;

        private Brush brCode;
        private Brush brBack;
        private Brush brData;

        private ScrollButton scrollButton;      // If we're scrolling the scrollbuttons.
        private int xLastMouseUp;
        private long cbTotal;
        private ControlLayout layout;

        protected override void OnPaint(PaintEventArgs pe)
        {
            this.brBack = new SolidBrush(BackColor);
            this.brCode = new SolidBrush(Color.Pink);
            this.brData = new SolidBrush(Color.LightBlue);
            try
            {
                this.layout = CalculateLayout();
                RenderSelectionBar(pe.Graphics);
                RenderScrollControls(pe.Graphics);
                RenderBody(pe.Graphics, this.layout);
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

        private void RenderScrollControls(Graphics g)
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

        private void RenderBody(Graphics g, ControlLayout layout)
        {
            Rectangle rcPaint = layout.rcBody;
            rcPaint.Width = 0;
            Brush brNew = null;
            foreach (var sl in layout.segLayouts)
            {
                Brush brOld = brBack;
                if (sl.X - cxOffset < layout.rcBody.Width && sl.X + sl.CxWidth - cxOffset >= 0)
                {
                    int xMin = (int)Math.Max(sl.X - cxOffset, 0);
                    int xMax = (int)Math.Min(sl.X + sl.CxWidth - cxOffset, layout.rcBody.Width);
                    long cbOffset = (xMin - (sl.X - cxOffset)) * granularity;
                    for (int x = xMin; x < xMax; ++x, cbOffset += granularity)
                    {
                        brNew = GetColorForOffset(sl.Segment, cbOffset);
                        if (brNew != brOld)
                        {
                            rcPaint.Width = x + CxScroll - rcPaint.X;
                            g.FillRectangle(brOld, rcPaint);
                            brOld = brNew;
                            rcPaint.X = x + CxScroll;
                        }
                    }
                    if (brNew != null)
                    {
                        rcPaint.Width = xMax + CxScroll - rcPaint.X;
                        g.FillRectangle(brNew, rcPaint);
                    }
                }
            }
        }

        private Brush GetColorForOffset(ImageSegment seg, long cbOffset)
        {
            ImageMapItem item;
            var lin = seg.Address.ToLinear() + (uint) cbOffset;
            if (!seg.IsInRange(lin)) 
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

        public ControlLayout CalculateLayout()
        {
            var segLayouts = new List<SegmentLayout>();
            long x = 0;
            long cx = 0;
            if (imageMap != null && granularity > 0)
            {
                foreach (var segment in imageMap.Segments.Values)
                {
                    cx = (segment.Size + granularity - 1) / granularity;
                    segLayouts.Add(new SegmentLayout { Segment = segment, X = x, CxWidth = cx });
                    x += cx + CxSegmentBorder;
                }
            }
            return new ControlLayout
            {
                rcBody = new Rectangle(
                    CxScroll, 0,
                    Width - 2 * CxScroll,
                    Height - CySelection),
                segLayouts = segLayouts,
                cxExtent = (int) x,
            };
        }

        long AddressableExtent(ImageSegment seg)
        {
            var addrMin = Address.Max(seg.Address, seg.MemoryArea.BaseAddress);
            var addrMax = Address.Min(seg.Address + seg.ContentSize, seg.MemoryArea.BaseAddress + seg.MemoryArea.Length);
            return addrMax - addrMin;
        }

        private void BoundGranularity(long value)
        {
            granularity = Math.Max(1L, value);
            if (ImageMap == null)
            {
                return;
            }

            int cxAvailable = ClientSize.Width - 2 * CxScroll;
            int cxConstant = 0;             // pixels always needed
            int cxConstantPerSegment = CxSegmentBorder; // constant pixel overhead per segment
            int cSeg = ImageMap.Segments.Count;
            long cbTotal = ImageMap.Segments.Values
                .Sum(s => s.Size);
            granularity = (long)Math.Ceiling(
               cbTotal /
               (double)(cxAvailable - cxConstant - cxConstantPerSegment * cSeg));
            granularity = Math.Min(value, granularity);
        }

        private void BoundOffset(long value)
        {
            cxOffset = value;
            if (layout != null)
            {
                var lastSeg = imageMap.Segments.Values.Last();
                cxOffset = Math.Min(cxOffset, layout.cxExtent);
            }
            cxOffset = Math.Max(0, cxOffset);
        }

        private void Zoom(float factor)
        {
            var oldGranularity = Granularity;
            var newGranularity = (int)Math.Ceiling(Granularity * factor);
            cxOffset = cxOffset + (oldGranularity - newGranularity) * (xLastMouseUp - CxScroll);
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
            var addr = MapClientPositionToAddress(e.X);
            if (addr != null)
            {
                SelectedAddress = addr;
            }
            base.OnMouseUp(e);
        }

        private Address MapClientPositionToAddress(int x)
        {
            if (imageMap == null || layout == null)
                return null;

            x -= (CxScroll + (int) cxOffset);          // bias past the scroller button.
            foreach (var sl in layout.segLayouts)
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

using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Controls
{
    public partial class ImageMapView : Control
    {
        private const float ZoomOutFactor = 1.25F;
        private const float ZoomInFactor = 4.0F / 5.0F;
        private const int CySelection = 3;
        private const int CxScroll = 16;
        private const int CyScroll = 16;
        private const int ScrollStep = 8;

        public ImageMapView()
        {
            InitializeComponent();
            scrollTimer.Tick += scrollTimer_Tick;
            xLastMouseUp = CxScroll;
        }

        public int Granularity { get { return granularity; } set { BoundGranularity(value); BoundOffset(offset); OnGranularityChanged(); } }
        public event EventHandler GranularityChanged;
        private int granularity;

        [Browsable(false)]
        public LoadedImage Image { get { return image; } set { image = value; OnImageChanged(); } }
        public event EventHandler ImageChanged;
        public LoadedImage image;

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
        public Address SelectedAddress { get { return selectedAddress; } set { selectedAddress = value; SelectedAddressChanged.Fire(this); } }
        public event EventHandler SelectedAddressChanged;
        private Address selectedAddress;

        public int Offset { get { return offset; } set { BoundOffset(value); OnOffsetChanged(); } }
        public event EventHandler OffsetChanged;
        private int offset;

        private Brush brCode;
        private Brush brBack;
        private Brush brData;

        private ScrollButton scrollButton;      // If we're scrolling the scrollbuttons.
        private int xLastMouseUp;

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
            var baseState = image != null && image.Bytes.Length > granularity * Width 
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

        private void RenderRightcontrol(Graphics g, Rectangle rcBody)
        {
        }

        private void RenderBody(Graphics g, Rectangle rcBody)
        {
            int cbOffset = offset;
            Brush brOld = brBack;
            Rectangle rcPaint = rcBody;
            rcPaint.Width = 0;
            Brush brNew = null;
            for (int x = rcBody.X; x < rcBody.Right; ++x, cbOffset += granularity)
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
                rcPaint.Width = rcBody.Right - rcPaint.X;
                g.FillRectangle(brNew, rcPaint);
            }
            //g.DrawRectangle(Pens.Red, rcBody);
        }

        private Brush GetColorForOffset(int cbOffset)
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
            if (item.DataType is UnknownType)
                return brBack;
            if (item is ImageMapBlock)
                return brCode;
            else
                return brData;
        }

        private Rectangle CalculateLayout()
        {
            var rc = new Rectangle(
                CxScroll, 0,
                Width-2 * CxScroll,
                Height-CySelection);
            return rc;
        }

        private void BoundGranularity(int value)
        {
            granularity = Math.Max(1, value);
            if (image != null)
            {
                granularity = Math.Min(
                    granularity,
                    (int)Math.Ceiling((double)image.Bytes.Length / (double)Width));
            }
        }

        private void BoundOffset(int value)
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
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected virtual void OnGranularityChanged()
        {
            Invalidate();
            GranularityChanged.Fire(this);
        }

        protected virtual void OnImageChanged()
        {
            Invalidate();
            ImageChanged.Fire(this);
        }

        protected virtual void OnImageMapChanged()
        {
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
            Invalidate();
        }
    }
}

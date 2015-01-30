using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Controls
{
    public partial class ImageMapView : Control
    {
        private const float ZoomOutFactor = 1.5F;
        private const float ZoomInFactor = 2.0F / 3.0F;
        private const int CySelection = 3;
        private const int CxScroll = 16;
        private const int CyScroll = 16;

        public ImageMapView()
        {
            InitializeComponent();
        }

        public int Granularity { 
            get { return granularity; }
            set
            {
                granularity = value;
                BoundGranularity();
                OnGranularityChanged();
            }
        }
        public event EventHandler GranularityChanged;
        private int granularity;

        [Browsable(false)]
        public LoadedImage Image { get { return image; } set { image = value; OnImageChanged(); } }
        public event EventHandler ImageChanged;
        public LoadedImage image;

        [Browsable(false)]
        public ImageMap ImageMap { get { return imageMap; } set { imageMap = value; OnImageMapChanged(); } }
        public event EventHandler ImageMapChanged;
        public ImageMap imageMap;

        public int Offset { get { return offset; } set { offset = value; OnOffsetChanged(); } }
        public event EventHandler OffsetChanged;
        private int offset;


        private Brush brMemory;
        private Brush brBack;

        protected override void OnPaint(PaintEventArgs pe)
        {
            this.brBack = new SolidBrush(BackColor);
            this.brMemory = new SolidBrush(Color.Pink);
            try
            {
                Rectangle rcBody = CalculateLayout();
                RenderSelectionBar(pe.Graphics);
                RenderScrollControls(pe.Graphics, rcBody);
                RenderBody(pe.Graphics, rcBody);
            }
            finally
            {
                brMemory.Dispose();
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
            ControlPaint.DrawScrollButton(g,
                0, (Height - CyScroll) / 2, CxScroll, CyScroll, ScrollButton.Left, ButtonState.Normal | ButtonState.Flat); 
            ControlPaint.DrawScrollButton(g,
              Width - CxScroll, (Height - CyScroll) / 2, CxScroll, CyScroll, ScrollButton.Right, ButtonState.Normal | ButtonState.Flat);
                 
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
            g.DrawRectangle(Pens.Red, rcBody);
        }

        private Brush GetColorForOffset(int cbOffset)
        {
            ImageMapItem item;

            if (imageMap == null ||
                image == null ||
                !imageMap.TryFindItem(image.BaseAddress + cbOffset, out item))
                return brBack;
            if (item.DataType is UnknownType)
                return brBack;
            return brMemory;
        }

        private Rectangle CalculateLayout()
        {
            var rc = new Rectangle(
                CxScroll, 0,
                Width-2 * CxScroll,
                Height-CySelection);
            return rc;
        }


        private void BoundGranularity()
        {
            granularity = Math.Max(1, granularity);
            if (image != null)
            {
                granularity = Math.Min(
                    granularity,
                    (int)Math.Ceiling((double)image.Bytes.Length / (double)Width));
            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Focus();
            base.OnMouseClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyData)
            {
            case Keys.Add:
                Granularity = (int) Math.Ceiling(Granularity *  ZoomInFactor);
                break;
            case Keys.Subtract:
                Granularity = (int) Math.Ceiling(Granularity * ZoomOutFactor);
                break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            BoundGranularity();
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
    }
}

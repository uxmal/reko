#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Controls
{
    public partial class TextView : Control
    {
        public event EventHandler ModelChanged;
        public event EventHandler<EditorNavigationArgs> Navigate;

        private Brush fgBrush;
        private Brush bgBrush;
        private StringFormat stringFormat;

        public TextView()
        {
            InitializeComponent();

            this.model = new EmptyEditorModel();
            this.styles = new Dictionary<string, EditorStyle>();
            this.stringFormat = StringFormat.GenericTypographic;
            this.ModelChanged += model_ModelChanged;
            this.vScroll.ValueChanged += vScroll_ValueChanged;
        }

        public new Size ClientSize
        {
            get
            {
                return new Size(
                    base.ClientSize.Width - vScroll.Width,
                    base.ClientSize.Height);
            }
        }

        public new Rectangle ClientRectangle
        {
            get { return new Rectangle(new Point(0, 0), ClientSize); }
        }

        private Font GetFont(string styleSelector)
        {
            EditorStyle style;
            if (!string.IsNullOrEmpty(styleSelector) &&
                Styles.TryGetValue(styleSelector, out style) && 
                style.Font != null)
                return style.Font;
            return this.Font;
        }

        private Brush GetForeground(string styleSelector)
        {
            EditorStyle style = GetStyle(styleSelector);
            if (style != null && style.Foreground != null)
                return style.Foreground;
            return CacheBrush(ref fgBrush, new SolidBrush(ForeColor));
        }

        private Brush GetBackground(string styleSelector)
        {
            EditorStyle style = GetStyle(styleSelector);
            if (style != null && style.Background != null)
                return style.Background;
            return CacheBrush(ref bgBrush, new SolidBrush(BackColor));
        }

        private EditorStyle GetStyle(string styleSelector)
        {
            EditorStyle style;
            if (!string.IsNullOrEmpty(styleSelector) &&
                Styles.TryGetValue(styleSelector, out style))
                return style;
            return null;
        }

        private Cursor GetCursor(string styleSelector)
        {
            EditorStyle style = GetStyle(styleSelector);
            if (style != null && style.Cursor != null)
                return style.Cursor;
            return Cursors.Default;
        }

        private Brush CacheBrush(ref Brush brInstance, Brush brNew)
        {
            if (brInstance != null)
                brInstance.Dispose();
            brInstance = brNew;
            return brNew;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (fgBrush != null) fgBrush.Dispose();
                if (bgBrush != null) bgBrush.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var span = GetSpan(e.Location);
            if (span != null)
            {
                this.Cursor = GetCursor(span.Style);
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Capture = true;
            Focus();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (Capture)
            {
                var span = GetSpan(e.Location);
                if (span != null && span.Tag != null)
                {
                    Navigate.Fire(this, new EditorNavigationArgs(span.Tag));
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            foreach (var line in visibleLines.Values)
            {
                PaintLine(line, e.Graphics);
            }
        }

        private class LineLayout
        {
            public RectangleF Extent;
            public SpanLayout[] Spans;
        }

        private class SpanLayout
        {
            public RectangleF Extent;
            public string Text;
            public string Style;
            public object Tag;
        }

        protected void ComputeLayout(Graphics g)
        {
            float cyLine = GetLineHeight();
            this.visibleLines = new SortedList<float, LineLayout>();
            SizeF szClient = new SizeF(ClientSize);
            var rcLine = new RectangleF(0, 0, szClient.Width, cyLine);
            
            int iLine = vScroll.Value;
            while (rcLine.Top < szClient.Height && 
                   iLine < model.LineCount)
            {
                var line = model.GetLineSpans(iLine);
                var ll = new LineLayout { 
                    Extent = rcLine,
                    Spans = ComputeSpanLayouts(line, rcLine, g)
                };
                this.visibleLines.Add(rcLine.Top, ll);
                ++iLine;
                rcLine.Offset(0, cyLine);
            }
        }

        private SpanLayout[] ComputeSpanLayouts(IEnumerable<TextSpan> spans, RectangleF rcLine, Graphics g)
        {
            var spanLayouts = new List<SpanLayout>();
            var pt = new PointF(rcLine.Left, rcLine.Top);
            foreach (var span in spans)
            {
                var text = span.GetText();
                var font = GetFont(span.Style);
                var szText = span.GetSize(text, font, g);
                var rc = new RectangleF(pt, szText);
                spanLayouts.Add(new SpanLayout
                {
                    Extent = rc,
                    Style = span.Style,
                    Text = text,
                    Tag = span.Tag,
                });
                pt.X = pt.X + szText.Width;
            }
            return spanLayouts.ToArray();
        }

        protected override void OnResize(EventArgs e)
        {
            int visibleWholeLines = (int) Math.Floor(Height / GetLineHeight());
            OnModelChanged();
            base.OnResize(e);
        }

        private SpanLayout GetSpan(Point pt)
        {
            foreach (var line in this.visibleLines.Values)
            {
                if (line.Extent.Contains(pt))
                    return FindSpan(pt, line);
            }
            return null;
        }

        private SpanLayout FindSpan(Point ptMouse, LineLayout line)
        {
            foreach (var span in line.Spans)
            {
                if (span.Extent.Contains(ptMouse))
                    return span;
            }
            return null;
        }

        private void PaintLine(LineLayout line, Graphics g)
        {
            foreach (var span in line.Spans)
            {
                var text = span.Text;

                var font = GetFont(span.Style);
                var fg = GetForeground(span.Style);
                var bg = GetBackground(span.Style);
                g.FillRectangle(bg, span.Extent);
                g.DrawString(text, font, fg, span.Extent, stringFormat);
            }
        }

        public TextViewModel Model { get { return model; } set { this.model = value; vScroll.Value = 0; OnModelChanged(); ModelChanged.Fire(this); } }
        private TextViewModel model;
        private void OnModelChanged()
        {
            int visibleLines = GetFullyVisibleLines();
            vScroll.Minimum = 0;
            vScroll.Maximum = Math.Max(model.LineCount - 1, 0);
            vScroll.LargeChange = Math.Max(visibleLines - 1, 0);
            vScroll.Enabled = visibleLines < model.LineCount;

            var g = CreateGraphics();
            ComputeLayout(g);
            g.Dispose();

            Invalidate();
        }

        private int GetVisibleLines()
        {
            float lines = ClientSize.Height / GetLineHeight();
            return (int) Math.Ceiling(lines);
        }

        private int GetFullyVisibleLines()
        {
            float lines = ClientSize.Height / GetLineHeight();
            return (int) Math.Floor(lines);
        }

        public Dictionary<string, EditorStyle> Styles { get { return styles; } }
        private Dictionary<string, EditorStyle> styles;
        private SortedList<float, LineLayout> visibleLines;

        private float GetLineHeight()
        {
            return this.Font.Height;
        }

        void model_ModelChanged(object sender, EventArgs e)
        {
            OnModelChanged();
        }

        void vScroll_ValueChanged(object sender, EventArgs e)
        {
            var g = CreateGraphics();
            ComputeLayout(g);
            g.Dispose();
            Invalidate();
        }
    }

    public class EditorStyle
    {
        public Font Font { get; set; }
        public Brush Foreground { get; set; }
        public Brush Background { get; set; }
        public Cursor Cursor { get; set; }
    }

    public class EditorNavigationArgs : EventArgs
    {
        public EditorNavigationArgs(object navigationDestination)
        {
            this.Destination = navigationDestination;
        }
        
        public object Destination { get; private set; }
    }
}

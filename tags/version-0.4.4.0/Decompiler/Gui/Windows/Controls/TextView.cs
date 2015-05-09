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
    /// <summary>
    /// A Windows Forms control that renders textual data. The textual data
    /// is fetched from a <seealso cref="TextViewModel"/>. 
    /// </summary>
    public partial class TextView : Control
    {
        // ModelChanged is fired whenever the Model property is set.
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

        public IServiceProvider Services { get; set; }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Shift|Keys.F10))
            {
                e.Handled = true;
                ContextMenu.Show(this, new Point(0, 0));
                return;
            }
            base.OnKeyDown(e);
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

        /// <summary>
        /// Computes the size of a text span.
        /// </summary>
        /// <remarks>
        /// The span is first asked to measure itself, then the current style is 
        /// allowed to override the size.
        /// </remarks>
        /// <param name="span"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        private SizeF GetSize(TextSpan span, string text, Font font, Graphics g)
        {
            var size = span.GetSize(text, font, g);
            EditorStyle style = GetStyle(span.Style);
            if (style != null && style.Width.HasValue)
            {
                size.Width = style.Width.Value;
            }
            return size;
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

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            model.MoveTo(model.CurrentPosition, (e.Delta < 0 ? 1 : -1));
            RecomputeLayout();
            UpdateScrollbar();
            OnScroll();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            foreach (var line in visibleLines.Values)
            {
                PaintLine(line, e.Graphics);
            }
        }

        /// <summary>
        /// Line of spans.
        /// </summary>
        private class LayoutLine
        {
            public RectangleF Extent;
            public LayoutSpan[] Spans;
        }

        /// <summary>
        /// Horizontal span of text
        /// </summary>
        protected class LayoutSpan
        {
            public RectangleF Extent;
            public string Text;
            public string Style;
            public object Tag;
            public int ContextMenuID;
        }

        protected void ComputeLayout(Graphics g)
        {
            float cyLine = GetLineHeight();
            this.visibleLines = new SortedList<float, LayoutLine>();
            SizeF szClient = new SizeF(ClientSize);
            var rcLine = new RectangleF(0, 0, szClient.Width, cyLine);
            
            // Get the lines.
            int cVisibleLines = (int) Math.Ceiling(szClient.Height / cyLine);
            var lines = model != null ? model.GetLineSpans(cVisibleLines) : new TextSpan[0][];
            int iLine = 0;
            while (rcLine.Top < szClient.Height && 
                   iLine < lines.Length)
            {
                var line = lines[iLine];
                var ll = new LayoutLine { 
                    Extent = rcLine,
                    Spans = ComputeSpanLayouts(line, rcLine, g)
                };
                this.visibleLines.Add(rcLine.Top, ll);
                ++iLine;
                rcLine.Offset(0, cyLine);
            }
        }

        /// <summary>
        /// Computes the layout for a line of spans.
        /// </summary>
        /// <param name="spans"></param>
        /// <param name="rcLine"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        private LayoutSpan[] ComputeSpanLayouts(IEnumerable<TextSpan> spans, RectangleF rcLine, Graphics g)
        {
            var spanLayouts = new List<LayoutSpan>();
            var pt = new PointF(rcLine.Left, rcLine.Top);
            foreach (var span in spans)
            {
                var text = span.GetText();
                var font = GetFont(span.Style);
                var szText = GetSize(span, text, font, g);
                var rc = new RectangleF(pt, szText);
                spanLayouts.Add(new LayoutSpan
                {
                    Extent = rc,
                    Style = span.Style,
                    Text = text,
                    ContextMenuID = span.ContextMenuID,
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

        /// <summary>
        /// Returns the span located at the point <paramref name="pt"/>.
        /// </summary>
        /// <param name="pt">Location specified in client coordinates.</param>
        /// <returns></returns>
        protected LayoutSpan GetSpan(Point pt)
        {
            foreach (var line in this.visibleLines.Values)
            {
                if (line.Extent.Contains(pt))
                    return FindSpan(pt, line);
            }
            return null;
        }

        private LayoutSpan FindSpan(Point ptMouse, LayoutLine line)
        {
            foreach (var span in line.Spans)
            {
                if (span.Extent.Contains(ptMouse))
                    return span;
            }
            return null;
        }

        private void PaintLine(LayoutLine line, Graphics g)
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

        /// <summary>
        /// The Model provides text spans that the TextView uses to render itself.
        /// </summary>
        public TextViewModel Model { get { return model; } set { this.model = value; vScroll.Value = 0; OnModelChanged(); ModelChanged.Fire(this); } }
        private TextViewModel model;
        protected virtual void OnModelChanged()
        {
            int visibleLines = GetFullyVisibleLines();
            vScroll.Minimum = 0;
            if (model != null)
            {
                vScroll.Maximum = Math.Max(model.LineCount - 1, 0);
                vScroll.LargeChange = Math.Max(visibleLines - 1, 0);
                vScroll.Enabled = visibleLines < model.LineCount;
            }
            else
            {
                vScroll.Enabled = false;
            }
            var g = CreateGraphics();
            ComputeLayout(g);
            g.Dispose();

            Invalidate();
           
        }

        /// <summary>
        /// Returns the number of visible lines, including any partially visible ones.
        /// </summary>
        /// <returns></returns>
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

        private float GetLineHeight()
        {
            return this.Font.Height;
        }

        public Dictionary<string, EditorStyle> Styles { get { return styles; } }
        private Dictionary<string, EditorStyle> styles;
        private SortedList<float, LayoutLine> visibleLines;
        private bool ignoreScroll;

        void model_ModelChanged(object sender, EventArgs e)
        {
            OnModelChanged();
        }

        void vScroll_ValueChanged(object sender, EventArgs e)
        {
            if (ignoreScroll)
                return;
            model.SetPositionAsFraction(vScroll.Value, vScroll.Maximum);
            RecomputeLayout();
            OnScroll();
            Invalidate();
        }

        /// <summary>
        /// Called when the view is scrolled. 
        /// </summary>
        protected virtual void OnScroll()
        {
        }

        protected void RecomputeLayout()
        {
            var g = CreateGraphics();
            ComputeLayout(g);
            g.Dispose();
        }

        internal void UpdateScrollbar()
        {
            var frac = model.GetPositionAsFraction();
            this.ignoreScroll = true;
            vScroll.Value = (int)(Math.BigMul(frac.Item1, model.LineCount) / frac.Item2);
            this.ignoreScroll = false;
        }
    }

    public class EditorStyle
    {
        public Font Font { get; set; }
        public Brush Foreground { get; set; }
        public Brush Background { get; set; }
        public Cursor Cursor { get; set; }
        public int? Width { get; set; } // If set, the width is fixed at a certain size.
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

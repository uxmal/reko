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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Reko.Gui.Windows.Controls
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
        public event EventHandler SelectionChanged; // Fired whenever the selection changes.

        private Brush fgBrush;
        private Brush bgBrush;
        private StringFormat stringFormat;
        private SortedList<float, LayoutLine> visibleLines;
        private bool ignoreScroll;
        internal TextPointer cursorPos;
        internal TextPointer anchorPos;

        public TextView()
        {
            InitializeComponent();

            this.Selection = new TextSelection(this);
            this.model = new EmptyEditorModel();
            this.stringFormat = StringFormat.GenericTypographic;
            this.vScroll.ValueChanged += vScroll_ValueChanged;
        }

        public IServiceProvider Services { get; set; }

        public TextSelection Selection { get; private set; }

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

        /// <summary>
        /// The ClientSize is the client area minus the space taken up by
        /// the scrollbars.
        /// </summary>
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
            UiStyle style = GetStyle(span.Style);
            if (style != null && style.Width.HasValue)
            {
                size.Width = style.Width.Value;
            }
            return size;
        }

        private UiStyle GetStyle(string styleSelector)
        {
            UiStyle style;
            if (!string.IsNullOrEmpty(styleSelector) && Services != null)
            {
                var uipSvc = Services.RequireService<IUiPreferencesService>();
                uipSvc.Styles.TryGetValue(styleSelector, out style);
                return style;
            }
            return null;
        }

        private Cursor GetCursor(string styleSelector)
        {
            UiStyle style = GetStyle(styleSelector);
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

        private void SetCaret()
        {
        }

        private void ClearCaret()
        {
        }

        protected override void OnLostFocus(EventArgs e)
        {
            ClearCaret();
            base.OnLostFocus(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Capture = true;
            var pos = ClientToLogicalPosition(e.Location);
            if (!IsSelectionEmpty() &&
                IsInsideSelection(pos))
            {
                // if down-click is done on a selection, the user may want 
                // to drag it.
                dragging = true;
            }
            else if (ComparePositions(pos, cursorPos) != 0)
            {
                // User clicked somewhere other than the current cursor
                // position, so we need to move it.
                dragging = false;
                this.cursorPos = pos;
                if ((Control.ModifierKeys & Keys.Shift) == 0)
                    this.anchorPos = pos;
                Focus();
                SetCaret();
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Capture && !dragging)
            {
                var pos = ClientToLogicalPosition(e.Location);
                if (ComparePositions(cursorPos, pos) != 0)
                {
                    this.cursorPos = pos;
                    Invalidate();
                }
            }
            else
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
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (Capture)
            {
                var pos = ClientToLogicalPosition(e.Location);
                if (dragging)
                {
                    cursorPos = anchorPos = pos;
                    var span = GetSpan(e.Location);
                    if (span != null && span.Tag != null)
                    {
                        Navigate.Fire(this, new EditorNavigationArgs(span.Tag));
                    }
                    Invalidate();
                }
                else
                {
                    if (IsSelectionEmpty())
                    {
                        var span = GetSpan(e.Location);
                        if (span != null && span.Tag != null)
                        {
                            Navigate.Fire(this, new EditorNavigationArgs(span.Tag));
                        }
                    }
                    SelectionChanged.Fire(this);
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            model.MoveToLine(model.CurrentPosition, (e.Delta < 0 ? 1 : -1));
            RecomputeLayout();
            UpdateScrollbar();
            OnScroll();
            Invalidate();
        }

        /// <summary>
        /// Line of spans.
        /// </summary>
        private class LayoutLine
        {
            public LayoutLine(object Position) { this.Position = Position; }
            public object Position;
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

        private int ComparePositions(TextPointer a, TextPointer b)
        {
            var d = model.ComparePositions(a.Line, b.Line);
            if (d != 0)
                return d;
            d = a.Span.CompareTo(b.Span);
            if (d != 0)
                return d;
            return a.Character.CompareTo(b.Character);
        }

        internal TextPointer GetStartSelection()
        {
            if (ComparePositions(cursorPos, anchorPos) <= 0)
                return cursorPos;
            else
                return anchorPos;
        }

        internal TextPointer GetEndSelection()
        {
            if (ComparePositions(cursorPos, anchorPos) > 0)
                return cursorPos;
            else
                return anchorPos;
        }

        internal bool IsSelectionEmpty() {
            return ComparePositions(cursorPos, anchorPos) == 0;
        }
    
        internal bool IsInsideSelection(TextPointer pos)
        {
            return
                ComparePositions(GetStartSelection(), pos) <= 0 &&
                ComparePositions(pos, GetEndSelection()) < 0;
        }

        /// <summary>
        /// Computes the layout of all visible text spans and stores them the 
        /// member variable 'visibleLines'. This includes a final partial item on the end.
        /// </summary>
        /// <param name="g"></param>
        protected void ComputeLayout(Graphics g)
        {
            float cyLine = GetLineHeight();
            this.visibleLines = new SortedList<float, LayoutLine>();
            SizeF szClient = new SizeF(ClientSize);
            var rcLine = new RectangleF(0, 0, szClient.Width, cyLine);
            
            // Get the lines.
            int cVisibleLines = (int) Math.Ceiling(szClient.Height / cyLine);
            var lines = model != null ? model.GetLineSpans(cVisibleLines) : new LineSpan[0];
            int iLine = 0;
            while (rcLine.Top < szClient.Height && 
                   iLine < lines.Length)
            {
                var line = lines[iLine];
                var ll = new LayoutLine(line.Position) { 
                    Extent = rcLine,
                    Spans = ComputeSpanLayouts(line.TextSpans, rcLine, g)
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
            ChangeLayout();
            base.OnResize(e);
        }

        private TextPointer ClientToLogicalPosition(Point pt)
        {
            foreach (var line in this.visibleLines.Values)
            {
                if (line.Extent.Top <= pt.Y && pt.Y < line.Extent.Bottom)
                {
                    return FindSpanPosition(pt, line);
                }
            }
            return new TextPointer { Line = Model.EndPosition, Span = 0, Character = 0 };
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

        private LayoutSpan FindSpan(Point ptClient, LayoutLine line)
        {
            foreach (var span in line.Spans)
            {
                if (span.Extent.Contains(ptClient))
                    return span;
            }
            return null;
        }

        private TextPointer FindSpanPosition(Point ptClient, LayoutLine line)
        {
            int iSpan = 0;
            foreach (var span in line.Spans)
            {
                if (span.Extent.Contains(ptClient))
                {
                    int iChar = GetCharPosition(ptClient, span);
                    return new TextPointer
                    {
                        Line = line.Position,
                        Span = iSpan,
                        Character = iChar
                    };
                }
                ++iSpan;
            }
            return new TextPointer { Line = line.Position, Span = iSpan, Character = 0 };
        }

        private int GetCharPosition(Point ptClient, LayoutSpan span)
        {
            var x = ptClient.X - span.Extent.Left;
            var g = CreateGraphics();
            var textStub = span.Text;
            int iLow = 0;
            int iHigh = textStub.Length;
            var font = GetFont(span.Style);
            var sz = MeasureText(g, textStub, font);
            float xLow = 0;
            float xHigh = sz.Width;
            while (iLow < iHigh-1)
            {
                int iMid = iLow + (iHigh - iLow) / 2;
                textStub = span.Text.Substring(0, iMid);
                sz = MeasureText(g, textStub, font);
                if (x < sz.Width)
                {
                    iHigh = iMid;
                    xHigh = sz.Width;
                }
                else
                {
                    iLow = iMid;
                    xLow = sz.Width;
                }
            }
            var cx = xHigh - xLow;
            if (x - xLow > cx)
                return iHigh;
            else
                return iLow;
        }

        private Size MeasureText(Graphics g, string text, Font font)
        {
            var sz = TextRenderer.MeasureText(
                g, text, font, new Size(0, 0), 
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            return sz;
        }

        /// <summary>
        /// The Model provides text spans that the TextView uses to render itself.
        /// </summary>
        public TextViewModel Model { get { return model; } set { this.model = value; OnModelChanged(EventArgs.Empty); }  }
        private TextViewModel model;
        private bool dragging;
        protected virtual void OnModelChanged(EventArgs e)
        {
            this.cursorPos = new TextPointer
            {
                Line = model.StartPosition,
                Span = 0,
                Character = 0
            };
            this.anchorPos = cursorPos;
            vScroll.Value = 0;
            ChangeLayout();
            ModelChanged.Fire(this);
        }

        /// <summary>
        /// Recomputes the spans and scrollbars of the TextView.
        /// </summary>
        protected void ChangeLayout()
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
        /// Given a point in client coordinates, locate the tag associated
        /// with the clicked span -- if there is one.
        /// </summary>
        /// <param name="ptClient"></param>
        /// <returns></returns>
        public object GetTagFromPoint(Point ptClient)
        {
            var span = GetSpan(ptClient);
            if (span == null)
                return null;
            return span.Tag;
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

        
        void model_ModelChanged(object sender, EventArgs e)
        {
            ChangeLayout();
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

        internal void SaveSelectionToStream(Stream stream, string cfFormat)
        {
            var modelPos = model.CurrentPosition;
            try
            {
                var writer = new StreamWriter(stream, Encoding.Unicode);
                var start = GetStartSelection();
                var end = GetEndSelection();
                if (ComparePositions(start, end) == 0)
                    return;
                model.MoveToLine(start.Line, 0);
                var spans = model.GetLineSpans(1);
                var line = start.Line;
                int iSpan = start.Span;
                int iChar = start.Character;
                for (;;)
                {
                    var span = spans[0].TextSpans[iSpan];
                    if (model.ComparePositions(spans[0].Position, end.Line) == 0 &&
                        iSpan == end.Span)
                    {
                        writer.Write(span.GetText().Substring(iChar, end.Character-iChar));
                        writer.Flush();
                        return;
                    }
                    else {
                        writer.Write(span.GetText().Substring(iChar));
                    }
                    ++iSpan;
                    iChar = 0;
                    if (iSpan >= spans[0].TextSpans.Length)
                    {
                        writer.WriteLine();
                        model.MoveToLine(line, 1);
                        spans = model.GetLineSpans(1);
                        if (spans.Length == 0)
                        {
                            writer.Flush();
                            return;
                        }
                        line = spans[0].Position;
                        iSpan = 0;
                    }
                }
            }
            finally
            {
                model.MoveToLine(modelPos, 0);
            }
        }

        internal void UpdateScrollbar()
        {
            var frac = model.GetPositionAsFraction();
            this.ignoreScroll = true;
            vScroll.Value = (int)(Math.BigMul(frac.Item1, model.LineCount) / frac.Item2);
            this.ignoreScroll = false;
        }
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

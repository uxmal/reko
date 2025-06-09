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
using Reko.Core.Services;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
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
        public event EventHandler VScrollValueChanged;
        public event EventHandler<SpanEventArgs> SpanEnter;
        public event EventHandler<SpanEventArgs> SpanLeave;

        private StringFormat stringFormat;
        private TextViewLayout layout;
        private bool ignoreScroll;
        internal TextPointer cursorPos;
        internal TextPointer anchorPos;
        private StyleStack styleStack;
        private bool dragging;
        private LayoutSpan spanHover;       // The span over which the mouse is hovering.

        public TextView()
        {
            InitializeComponent();

            base.DoubleBuffered = true;
            this.Selection = new TextSelection(this);
            this.model = new EmptyEditorModel();
            this.stringFormat = StringFormat.GenericTypographic;
            this.layout = new TextViewLayout(model, this.Font);
            this.vScroll.ValueChanged += vScroll_ValueChanged;
        }

        public IServiceProvider Services { get { return services; } set { services = value; OnServicesChanged(); } }
        private IServiceProvider services;
        protected virtual void OnServicesChanged()
        {
            ChangeLayout();
        }

        public TextSelection Selection { get; private set; }

        /// <summary>
        /// Performs the same function as the HTML "class" attribute.
        /// </summary>
        public string StyleClass { get { return styleClass; } set { styleClass = value; StyleClassChanged?.Invoke(this, EventArgs.Empty); } }
        public event EventHandler StyleClassChanged;
        private string styleClass;

        /// <summary>
        /// Exposes the vertical scrollbar.
        /// </summary>
        public VScrollBar VScrollBar { get { return vScroll; } }

        /// <summary>
        /// The ClientSize is the client area minus the space taken up by
        /// the scrollbars.
        /// </summary>
        public new Size ClientSize
        {
            get
            {
                int cxScroll = vScroll.Visible ? vScroll.Width : 0;
                return new Size(
                    base.ClientSize.Width - cxScroll,
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
            int? width = styleStack.GetWidth();
            if (width.HasValue)
            {
                size.Width = width.Value;
            }
            return size;
        }

        private StyleStack GetStyleStack()
        {
            if (styleStack is null)
                styleStack = new StyleStack(Services.RequireService<IUiPreferencesService>());
            return styleStack;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (styleStack is not null) styleStack.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SetCaret()
        {
            //$TODO: want a cross-platform caret
        }

        private void ClearCaret()
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Shift | Keys.F10))
            {
                e.Handled = true;
                ContextMenuStrip.Show(this, new Point(0, 0));
                return;
            }
            base.OnKeyDown(e);
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
            else if (layout.ComparePositions(pos, cursorPos) != 0)
            {
                Focus();

                // User clicked somewhere other than the current cursor
                // position, so we need to move it.
                dragging = false;
                this.cursorPos = pos;
                if ((Control.ModifierKeys & Keys.Shift) == 0)
                    this.anchorPos = pos;
                SetCaret();
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Capture && !dragging)
            {
                // We're extending the selection
                var pos = ClientToLogicalPosition(e.Location);
                if (layout.ComparePositions(cursorPos, pos) != 0)
                {
                    this.cursorPos = pos;
                    Invalidate();
                }
            }
            else
            {
                // Not captured, so rat is just floating over us.
                // Show the right cursor.
                var span = GetSpan(e.Location);
                if (span is not null)
                {
                    GetStyleStack().PushStyle(StyleClass);
                    styleStack.PushStyle(span.Style);
                    this.Cursor = styleStack.GetCursor(this);
                    styleStack.PopStyle();
                    styleStack.PopStyle();
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
                if (span != spanHover)
                {
                    if (spanHover is not null)
                    {
                        SpanLeave?.Invoke(this, new SpanEventArgs(spanHover));
                    }
                    spanHover = span;
                    if (span is not null)
                    {
                        SpanEnter?.Invoke(this, new SpanEventArgs(span));
                    }
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
                    if (span is not null && span.Tag is not null)
                    {
                        Navigate?.Invoke(this, new EditorNavigationArgs(span.Tag));
                    }
                    Invalidate();
                }
                else
                {
                    if (IsSelectionEmpty())
                    {
                        var span = GetSpan(e.Location);
                        if (span is not null && span.Tag is not null)
                        {
                            Navigate?.Invoke(this, new EditorNavigationArgs(span.Tag));
                        }
                    }
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
                Capture = false;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var wheelChange = 3;
            var newValue = vScroll.Value +
                (e.Delta < 0 ? wheelChange : -wheelChange);
            vScroll.Value = Math.Max(
                Math.Min(newValue, vScroll.Maximum),
                vScroll.Minimum);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Services is null)
            {
                Debug.Print("TextView.OnPaint: Services property must be set");
                return;
            }
            GetStyleStack().PushStyle(StyleClass);
            var painter = new TextViewPainter(layout, e.Graphics, ForeColor, BackColor, Font, styleStack);
            painter.SetSelection(GetStartSelection(), GetEndSelection());

            painter.PaintGdi();
            GetStyleStack().PopStyle();
        }

        // Disable background painting to avoid the horrible flicker.
        // We draw our own background anyway.
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        public TextPointer GetStartSelection()
        {
            if (layout.ComparePositions(cursorPos, anchorPos) <= 0)
                return cursorPos;
            else
                return anchorPos;
        }

        public TextPointer GetEndSelection()
        {
            if (layout.ComparePositions(cursorPos, anchorPos) > 0)
                return cursorPos;
            else
                return anchorPos;
        }

        public bool IsSelectionEmpty()
        {
            return  layout.ComparePositions(cursorPos, anchorPos) == 0;
        }

        internal bool IsInsideSelection(TextPointer pos)
        {
            return
                layout.ComparePositions(GetStartSelection(), pos) <= 0 &&
                layout.ComparePositions(pos, GetEndSelection()) < 0;
        }

        public void ClearSelection()
        {
            anchorPos = cursorPos;
            ChangeLayout();
            Invalidate();
        }

        public void SelectAll()
        {
            anchorPos = new TextPointer(model.StartPosition, 0, 0);
            cursorPos = new TextPointer(model.EndPosition, 0, 0);
            Invalidate();
        }

        /// <summary>
        /// Computes the layout of all visible text spans and stores them the 
        /// member variable 'visibleLines'. This includes a final partial item on the end.
        /// </summary>
        /// <param name="g"></param>
        protected void ComputeLayout(Graphics g)
        {
            var m = model ?? new EmptyEditorModel();
            var oldPos = m.CurrentPosition;
            GetStyleStack().PushStyle(StyleClass);
            this.layout = TextViewLayout.VisibleLines(m, ClientSize, g, Font, GetStyleStack());
            GetStyleStack().PopStyle();
            m.MoveToLine(oldPos, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            ChangeLayout();
            base.OnResize(e);
        }

        private TextPointer ClientToLogicalPosition(Point pt)
        {
            using (var g = CreateGraphics())
            {
                GetStyleStack().PushStyle(StyleClass);
                var ptr = layout.ClientToLogicalPosition(g, pt, styleStack);
                styleStack.PopStyle();
                return ptr;
            }
        }

        private RectangleF LogicalPositionToClient(TextPointer pos)
        {
            using (var g = CreateGraphics())
            {
                GetStyleStack().PushStyle(StyleClass);
                var ptr = layout.LogicalPositionToClient(g, pos, styleStack);
                styleStack.PopStyle();
                return ptr;
            }
        }

        public Point GetAnchorTopPoint()
        {
            var rect = LogicalPositionToClient(anchorPos);
            return new Point((int)rect.Left, (int)rect.Top);
        }

        public Point GetAnchorMiddlePoint()
        {
            var rect = LogicalPositionToClient(anchorPos);
            return new Point((int)rect.Left, (int)((rect.Top + rect.Bottom) / 2));
        }

        /// <summary>
        /// Returns the span located at the point <paramref name="pt"/>.
        /// </summary>
        /// <param name="pt">Location specified in client coordinates.</param>
        /// <returns>The span containing the given point.</returns>
        protected LayoutSpan GetSpan(Point pt)
        {
            foreach (var line in this.layout.LayoutLines)
            {
                if (line.Extent.Contains(pt))
                    return FindSpan(pt, line);
            }
            return null;
        }

        /// <summary>
        /// Returns the LayoutLine containing the point <paramref name="pt"/>.
        /// </summary>
        /// <param name="pt">Location specified in client coordinates.</param>
        /// <returns>The span containing the given point.</returns>
        protected LayoutLine GetLine(Point pt)
        {
            foreach (var line in this.layout.LayoutLines)
            {
                if (line.Extent.Contains(pt))
                    return line;
            }
            return null;
        }

        private LayoutSpan FindSpan(Point ptClient, LayoutLine line)
        {
            foreach (var span in line.Spans)
            {
                if (span.ContentExtent.Contains(ptClient))
                    return span;
            }
            return null;
        }

        /// <summary>
        /// The Model provides text spans that the TextView uses to render itself.
        /// </summary>
        public ITextViewModel Model { get { return model; } set { this.model = value; OnModelChanged(EventArgs.Empty); } }
        private ITextViewModel model;
        protected virtual void OnModelChanged(EventArgs e)
        {
            this.cursorPos = new TextPointer(model.CurrentPosition, 0, 0);
            this.anchorPos = cursorPos;
            ChangeLayout();
            UpdateScrollbar();
            ModelChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Recomputes the spans and scrollbars of the TextView.
        /// </summary>
        protected void ChangeLayout()
        {
            if (Services is null)
                return;

            // Need to recompute the layout first so we can count
            // the fully visible lines.
            var g = CreateGraphics();
            ComputeLayout(g);
            g.Dispose();

            int visibleLines = GetFullyVisibleLines();
            vScroll.Minimum = 0;
            if (model is not null)
            {
                vScroll.Maximum = Math.Max(model.LineCount - 1, 0);
                vScroll.LargeChange = Math.Max(visibleLines - 1, 0);
                vScroll.SmallChange = 1;
                vScroll.Enabled = visibleLines < model.LineCount;
            }
            else
            {
                vScroll.Enabled = false;
            }
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
            return span?.Tag;
        }

        public object GetLineTagFromPoint(Point ptClient)
        {
            var line = GetLine(ptClient);
            return line?.Tag;
        }

        private int GetFullyVisibleLines()
        {
            int cLines = layout.LayoutLines.Count;
            if (cLines == 0)
                return 0;
            if (layout.LayoutLines[cLines - 1].Extent.Bottom > ClientRectangle.Bottom)
                return cLines - 1;
            else
                return cLines;
        }

        void vScroll_ValueChanged(object sender, EventArgs e)
        {
            if (ignoreScroll)
                return;
            model.SetPositionAsFraction(vScroll.Value, vScroll.Maximum);
            RecomputeLayout();
            OnScroll();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            RecomputeLayout();
            base.OnFontChanged(e);
        }

        /// <summary>
        /// Called when the view is scrolled. 
        /// </summary>
        protected virtual void OnScroll()
        {
            VScrollValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RecomputeLayout()
        {
            Invalidate();
            if (services is null)
                return;
            var g = CreateGraphics();
            ComputeLayout(g);
            g.Dispose();
        }

        internal void SaveSelectionToStream(Stream stream)
        {
            var modelPos = model.CurrentPosition;
            try
            {
                var writer = new StreamWriter(stream, new UnicodeEncoding(false, false));
                var start = GetStartSelection();
                var end = GetEndSelection();
                if (layout.ComparePositions(start, end) == 0)
                    return;
                model.MoveToLine(start.Line, 0);
                var spans = model.GetLineSpans(1);
                var line = start.Line;
                int iSpan = start.Span;
                int iChar = start.Character;
                for (;;)
                {
                    var span = (iSpan < spans[0].TextSpans.Length) ?
                        spans[0].TextSpans[iSpan] : null;
                    if (span is not null)
                    {
                        if (model.ComparePositions(spans[0].Position, end.Line) == 0 &&
                            iSpan == end.Span)
                        {
                            writer.Write(span.GetText().Substring(iChar, end.Character - iChar));
                            writer.Flush();
                            return;
                        }
                        else {
                            writer.Write(span.GetText().Substring(iChar));
                        }
                    }
                    ++iSpan;
                    iChar = 0;
                    if (iSpan >= spans[0].TextSpans.Length)
                    {
                        writer.WriteLine();
                        if (model.ComparePositions(
                            spans[0].Position, end.Line) >= 0)
                        {
                            writer.Flush();
                            return;
                        }
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
            if (frac.Item2 != 0)
                vScroll.Value = (int)(Math.BigMul(frac.Item1, vScroll.Maximum) / frac.Item2);
            this.ignoreScroll = false;
        }

        public void InvalidateModel()
        {
            RecomputeLayout();
            UpdateScrollbar();
            Invalidate();
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

    public class SpanEventArgs : EventArgs
    {
        public SpanEventArgs(LayoutSpan span)
        {
            this.Span = span;
        }

        public LayoutSpan Span { get; private set; }
    }
}

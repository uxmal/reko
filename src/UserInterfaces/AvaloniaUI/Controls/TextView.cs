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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Reko.UserInterfaces.AvaloniaUI.Controls
{
    /// <summary>
    /// An Avalonia control that renders textual data. The textual data
    /// is fetched from a <seealso cref="ITextViewModel"/>. 
    /// </summary>
    public partial class TextView : Control, ILogicalScrollable
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(TextView), "Memory control UI");

        public static readonly StyledProperty<IBrush?> BackgroundProperty =
            Border.BackgroundProperty.AddOwner<TextView>();
        public static readonly StyledProperty<IBrush?> SelectionBrushProperty =
            TextBox.SelectionBrushProperty.AddOwner<HexViewControl>();
        public static readonly StyledProperty<IBrush?> SelectionForegroundBrushProperty =
            TextBox.SelectionForegroundBrushProperty.AddOwner<HexViewControl>();
        public static readonly DirectProperty<TextView, IServiceProvider?> ServicesProperty =
            AvaloniaProperty.RegisterDirect<TextView, IServiceProvider?>(
                nameof(Services),
                o => o.Services,
                (o, v) => o.Services = v);
        public static readonly DirectProperty<TextView, ITextViewModel?> ModelProperty =
            AvaloniaProperty.RegisterDirect<TextView, ITextViewModel?>(
                nameof(Services),
                o => o.Model,
                (o, v) => o.Model = v);


        // ModelChanged is fired whenever the Model property is set.
        public event EventHandler ModelChanged;
        public event EventHandler<EditorNavigationArgs> Navigate;
        public event EventHandler SelectionChanged; // Fired whenever the selection changes.
        public event EventHandler VScrollValueChanged;
        public event EventHandler<SpanEventArgs> SpanEnter;
        public event EventHandler<SpanEventArgs> SpanLeave;

        private TextViewLayout? layout;
        private bool ignoreScroll;
        internal TextPointer cursorPos;
        internal TextPointer anchorPos;
        private StyleStack styleStack;
        private bool captured;
        private bool dragging;
        private LayoutSpan spanHover;       // The span over which the mouse is hovering.
        private FontFamily fontFamily;
        private Size cellSize;
        private double fontSize;
        private IBrush? foreground;
        private Typeface typeface;

        private EventHandler? scrollInvalidated;
        private bool canHorizontallyScroll;
        private bool canVerticallyScroll;
        private Size extent;
        private Size scrollSize;
        private Size pageScrollSize;
        private Size viewport;
        private Vector offset;

        static TextView()
        {
            AffectsRender<TextView>(
                //$TODO: CaretBrushProperty,
                SelectionBrushProperty,
                SelectionForegroundBrushProperty,
                TextElement.ForegroundProperty);
        }

        public TextView()
        {
            this.Selection = new TextSelection(this);
            this.model = new EmptyEditorModel();
        }

        event EventHandler? ILogicalScrollable.ScrollInvalidated
        {
            add => this.scrollInvalidated += value;
            remove => this.scrollInvalidated -= value;
        }

        public IServiceProvider? Services { get { return services; } set { services = value; OnServicesChanged(); } }
        private IServiceProvider? services;
        protected virtual void OnServicesChanged()
        {
            ChangeLayout();
        }

        /// <summary>
        /// Gets or sets a brush used to paint the control's background.
        /// </summary>
        public IBrush? Background
        {
            get => GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public TextSelection Selection { get; private set; }


        /// <summary>
        /// The brush used to paint the background of selected text.
        /// </summary>
        public IBrush? SelectionBrush
        {
            get => GetValue(SelectionBrushProperty);
            set => SetValue(SelectionBrushProperty, value);
        }

     
        /// <summary>
        /// The brush used to pain the glyphs of the selected text.
        /// </summary>
        public IBrush? SelectionForegroundBrush
        {
            get => GetValue(SelectionForegroundBrushProperty);
            set => SetValue(SelectionForegroundBrushProperty, value);
        }

        /// <summary>
        /// Performs the same function as the HTML "class" attribute.
        /// </summary>
        public string StyleClass { 
            get { return styleClass; }
            set { styleClass = value; StyleClassChanged?.Invoke(this, EventArgs.Empty); } }
        public event EventHandler StyleClassChanged;
        private string styleClass;


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
        private Size GetSize(TextSpan span, string text, Typeface font, double fontSize, DrawingContext g)
        {
            var size = span.GetSize(text, font, fontSize);
            int? width = styleStack.GetWidth();
            if (width.HasValue)
            {
                size = new Size(width.Value, size.Height);
            }
            return size;
        }

        private StyleStack GetStyleStack()
        {
            if (styleStack is null)
                styleStack = new StyleStack(Services.RequireService<IUiPreferencesService>(),
                    new System.Collections.Generic.Dictionary<string, AvaloniaProperty>());
                    //this.foreground,
                    //this.Background);
            return styleStack;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            //if (styleStack is not null)
            //    styleStack.Dispose();
            base.OnDetachedFromVisualTree(e);
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
            trace.Verbose("K: {0}, M: {1}", e.Key, e.KeyModifiers);
            if (e.Key == Key.F10 && e.KeyModifiers == KeyModifiers.Shift)
            {
                e.Handled = true;
                //ContextMenuStrip.Show(this, new Point(0, 0));
                return;
            }
            base.OnKeyDown(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            ClearCaret();
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            this.captured = true;
            e.Pointer.Capture(this);
            var pos = ClientToLogicalPosition(e.GetPosition(this));
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
                if ((e.KeyModifiers & KeyModifiers.Shift) == 0)
                    this.anchorPos = pos;
                SetCaret();
                InvalidateVisual();
            }
            base.OnPointerPressed(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            var location = e.GetCurrentPoint(this).Position;
            if (this.captured && !dragging)
            {
                // We're extending the selection
                var pos = ClientToLogicalPosition(location);
                if (layout.ComparePositions(cursorPos, pos) != 0)
                {
                    this.cursorPos = pos;
                    InvalidateVisual();
                }
            }
            else
            {
                // Not captured, so rat is just floating over us.
                // Show the right cursor.
                var span = GetSpan(location);
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
                    this.Cursor = Cursor.Default;
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
            base.OnPointerMoved(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            var location = e.GetCurrentPoint(this).Position;
            if (this.captured)
            {
                this.captured = false;
                var pos = ClientToLogicalPosition(location);
                if (dragging)
                {
                    cursorPos = anchorPos = pos;
                    var span = GetSpan(location);
                    if (span is not null && span.Tag is not null)
                    {
                        Navigate?.Invoke(this, new EditorNavigationArgs(span.Tag));
                    }
                    InvalidateVisual();
                }
                else
                {
                    if (IsSelectionEmpty())
                    {
                        var span = GetSpan(location);
                        if (span is not null && span.Tag is not null)
                        {
                            Navigate?.Invoke(this, new EditorNavigationArgs(span.Tag));
                        }
                    }
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
                e.Pointer.Capture(null);
            }
            base.OnPointerReleased(e);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            //var wheelChange = 3;
            //var newValue = vScroll.Value +
            //    (e.Delta < 0 ? wheelChange : -wheelChange);
            //vScroll.Value = Math.Max(
            //    Math.Min(newValue, vScroll.Maximum),
            //    vScroll.Minimum);
        }

        public override void Render(DrawingContext context)
        {
            if (Services is null)
            {
                Debug.Print("TextView.OnPaint: Services property must be set");
                return;
            }
            GetStyleStack().PushStyle(StyleClass);
            var fgSel = SelectionForegroundBrush ?? foreground;
            var painter = new TextViewPainter(layout, context, foreground, Background, fgSel, SelectionBrush, typeface, styleStack);
            painter.SetSelection(GetStartSelection(), GetEndSelection());

            painter.Paint();
            GetStyleStack().PopStyle();
        }

        public TextPointer GetStartSelection()
        {
            if (layout is null)
                return anchorPos;
            if (layout.ComparePositions(cursorPos, anchorPos) <= 0)
                return cursorPos;
            else
                return anchorPos;
        }

        public TextPointer GetEndSelection()
        {
            if (layout is null)
                return anchorPos;
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
            InvalidateVisual();
        }

        public void SelectAll()
        {
            anchorPos = new TextPointer(model.StartPosition, 0, 0);
            cursorPos = new TextPointer(model.EndPosition, 0, 0);
            InvalidateVisual();
        }

        /// <summary>
        /// Computes the layout of all visible text spans and stores them the 
        /// member variable 'visibleLines'. This includes a final partial item on the end.
        /// </summary>
        protected void ComputeLayout()
        {
            if (Services is null)
                return;

            Invalidate();

            var m = model ?? new EmptyEditorModel();
            var oldPos = m.CurrentPosition;
            GetStyleStack().PushStyle(StyleClass);
            this.layout = TextViewLayout.VisibleLines(m, Bounds.Size, this.fontSize, new Typeface(this.fontFamily), GetStyleStack());
            GetStyleStack().PopStyle();
            m.MoveToLine(oldPos, 0);
        }

        private void Invalidate()
        {
           this.fontFamily = TextElement.GetFontFamily(this);
           this.fontSize = TextElement.GetFontSize(this);
           this.foreground = TextElement.GetForeground(this);

            this.typeface = new Typeface(fontFamily);
            var m = CreateFormattedText("M");
            this.cellSize = new Size(Math.Floor(m.Width), Math.Ceiling(m.Height));
        }

        private FormattedText CreateFormattedText(string text)
        {
            return new FormattedText(text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                foreground);
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            ChangeLayout();
            base.OnSizeChanged(e);
        }

        private TextPointer ClientToLogicalPosition(Point pt)
        {
            GetStyleStack().PushStyle(StyleClass);
            var ptr = layout.ClientToLogicalPosition(pt, styleStack);
            styleStack.PopStyle();
            return ptr;
        }

        private Rect LogicalPositionToClient(TextPointer pos)
        {
            GetStyleStack().PushStyle(StyleClass);
            var ptr = layout.LogicalPositionToClient(pos, styleStack);
            styleStack.PopStyle();
            return ptr;
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
        protected LayoutSpan? GetSpan(Point pt)
        {
            if (this.layout is null)
                return null;
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
        protected LayoutLine? GetLine(Point pt)
        {
            if (this.layout is null)
                return null;
            foreach (var line in this.layout.LayoutLines)
            {
                if (line.Extent.Contains(pt))
                    return line;
            }
            return null;
        }

        private LayoutSpan? FindSpan(Point ptClient, LayoutLine line)
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
        public ITextViewModel Model
        {
            get { return model; } 
            set { 
                this.model = value ?? new EmptyEditorModel();
                OnModelChanged(EventArgs.Empty);
            }
        }
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
            fontFamily = TextElement.GetFontFamily(this);
            fontSize = TextElement.GetFontSize(this);
            foreground = TextElement.GetForeground(this);

            // Need to recompute the layout first so we can count
            // the fully visible lines.
            ComputeLayout();

            int visibleLines = GetFullyVisibleLines();
            
            //vScroll.Minimum = 0;
            //if (model is not null)
            //{
            //    vScroll.Maximum = Math.Max(model.LineCount - 1, 0);
            //    vScroll.LargeChange = Math.Max(visibleLines - 1, 0);
            //    vScroll.SmallChange = 1;
            //    vScroll.Enabled = visibleLines < model.LineCount;
            //}
            //else
            //{
            //    vScroll.Enabled = false;
            //}
            InvalidateVisual();
        }

        /// <summary>
        /// Given a point in client coordinates, locate the tag associated
        /// with the clicked span -- if there is one.
        /// </summary>
        /// <param name="ptClient"></param>
        /// <returns></returns>
        public object? GetTagFromPoint(Point ptClient)
        {
            var span = GetSpan(ptClient);
            return span?.Tag;
        }

        public object? GetLineTagFromPoint(Point ptClient)
        {
            var line = GetLine(ptClient);
            return line?.Tag;
        }

        private int GetFullyVisibleLines()
        {
            if (layout is null)
                return 0;
            int cLines = layout.LayoutLines.Count;
            if (cLines == 0)
                return 0;
            if (layout.LayoutLines[^1].Extent.Bottom > Bounds.Bottom)
                return cLines - 1;
            else
                return cLines;
        }

        void vScroll_ValueChanged(object sender, EventArgs e)
        {
            if (ignoreScroll)
                return;
            //$TODO:
            //model.SetPositionAsFraction(vScroll.Value, vScroll.Maximum);
            RecomputeLayout();
            OnScroll();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == BoundsProperty)
            {
                InvalidateScrollable();
            }

            if (change.Property == TextElement.FontFamilyProperty
                || change.Property == TextElement.FontSizeProperty
                || change.Property == TextElement.ForegroundProperty)
            {
                RecomputeLayout();
                InvalidateScrollable();
            }
        }

        private void InvalidateScrollable() {

            var lines = model?.LineCount ?? 0;
            var width = Bounds.Width;
            var height = Bounds.Height;

            this.viewport = new Size(width, Math.Ceiling(height / this.cellSize.Height));
            this.scrollSize = new Size(1, 1);
            this.pageScrollSize = new Size(width, Math.Floor(viewport.Height / this.cellSize.Height));
            this.extent = new Size(width, lines);
            trace.Verbose("TVC: lines: {0}: viewport {1}, extent: {2}", lines, viewport, extent);
            var scrollable = (ILogicalScrollable)this;
            scrollable.RaiseScrollInvalidated(EventArgs.Empty);

            InvalidateVisual();
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
            InvalidateVisual();
            if (Services is null)
                return;
            ComputeLayout();
        }

        internal void SaveSelectionToStream(Stream stream)
        {
            var modelPos = model.CurrentPosition;
            try
            {
                var writer = new StreamWriter(stream, new UnicodeEncoding(false, false));
                var start = GetStartSelection();
                var end = GetEndSelection();
                if (layout is null || layout.ComparePositions(start, end) == 0)
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
            //$TODO
            //var frac = model.GetPositionAsFraction();
            //this.ignoreScroll = true;
            //if (frac.Item2 != 0)
            //    vScroll.Value = (int)(Math.BigMul(frac.Item1, vScroll.Maximum) / frac.Item2);
            //this.ignoreScroll = false;
        }

        public void InvalidateModel()
        {
            RecomputeLayout();
            UpdateScrollbar();
            InvalidateVisual();
        }



        bool ILogicalScrollable.CanHorizontallyScroll
        {
            get => canHorizontallyScroll;
            set
            {
                canHorizontallyScroll = value;
                InvalidateMeasure();
            }
        }

        bool ILogicalScrollable.CanVerticallyScroll {
            get => this.canVerticallyScroll;
            set
            {
                this.canVerticallyScroll = value;
                InvalidateMeasure();
            }
        }

        bool ILogicalScrollable.IsLogicalScrollEnabled => true;

        Size ILogicalScrollable.ScrollSize => this.scrollSize;

        Size ILogicalScrollable.PageScrollSize => this.pageScrollSize;

        Size IScrollable.Extent => this.extent;

        Size IScrollable.Viewport => viewport;

        Vector IScrollable.Offset {
            get => this.offset;
            set
            {
                if (ignoreScroll)
                {
                    return;
                }
                ignoreScroll = true;
                offset = ClampOffset(value);
                model.SetPositionAsFraction((int)offset.Y, (int)this.extent.Height);
                InvalidateScrollable();
                RecomputeLayout();
                OnScroll();
                ignoreScroll = false;
            }
        }

        private Vector ClampOffset(Vector value)
        {
            var scrollable = (ILogicalScrollable)this;
            var maxX = Math.Max(scrollable.Extent.Width - scrollable.Viewport.Width, 0);
            var maxY = Math.Max(scrollable.Extent.Height - scrollable.Viewport.Height, 0);
            return new Vector(Clamp(value.X, 0, maxX), Clamp(value.Y, 0, maxY));
            static double Clamp(double val, double min, double max) => val < min ? min : val > max ? max : val;
        }



        bool ILogicalScrollable.BringIntoView(Control target, Rect targetRect)
        {
            return false;
        }

        Control? ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control? from)
        {
            return null;
        }

        void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
        {
            scrollInvalidated?.Invoke(this, e);
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

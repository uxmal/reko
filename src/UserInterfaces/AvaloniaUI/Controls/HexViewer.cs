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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Types;

namespace Reko.UserInterfaces.AvaloniaUI.Controls;

/// <summary>
/// Displays the contents of memory and allows selection of memory ranges. 
/// </summary>
/// <remarks>
/// Memory that has been identified is colored.
/// <para>
/// A memory cell is displayed with a one-pixel border on all sides to
/// help present it with selections.
/// </para>
/// </remarks>
public class HexViewControl : Control, ILogicalScrollable
{
    private static readonly TraceSwitch trace = new TraceSwitch(nameof(HexViewControl), "Memory control UI")
    { Level = TraceLevel.Verbose };

    private static readonly TraceSwitch tracePaint = new TraceSwitch(nameof(MemoryControlPainter), "Memory control painting operations");

    public event EventHandler<EventArgs>? SelectionChanged;

    public static readonly StyledProperty<int> ToBaseProperty =
        AvaloniaProperty.Register<HexViewControl, int>(nameof(ToBase), defaultValue: 16);

    public static readonly StyledProperty<int> BytesWidthProperty =
        AvaloniaProperty.Register<HexViewControl, int>(nameof(BytesWidth), defaultValue: 8);

    public static readonly StyledProperty<IBrush?> CodeBackgroundProperty =
        AvaloniaProperty.Register<HexViewControl, IBrush?>(nameof(CodeBackground));

    public static readonly StyledProperty<IBrush?> DataBackgroundProperty =
        AvaloniaProperty.Register<HexViewControl, IBrush?>(nameof(DataBackground));

    public static readonly DirectProperty<HexViewControl, Address?> AnchorAddressProperty =
        AvaloniaProperty.RegisterDirect<HexViewControl, Address?>(
            nameof(AnchorAddress),
            o => o.AnchorAddress,
            (o, v) => o.AnchorAddress = v);


    public static readonly DirectProperty<HexViewControl, IProcessorArchitecture?> ArchitectureProperty =
        AvaloniaProperty.RegisterDirect<HexViewControl, IProcessorArchitecture?>(
            nameof(Architecture),
            o => o.Architecture,
            (o, v) => o.Architecture = v);

    public static readonly DirectProperty<HexViewControl, Encoding> EncodingProperty =
        AvaloniaProperty.RegisterDirect<HexViewControl, Encoding>(
            nameof(Encoding),
            o => o.Encoding,
            (o, v) => o.Encoding = v);

    public static readonly DirectProperty<HexViewControl, ImageMap?> ImageMapProperty =
        AvaloniaProperty.RegisterDirect<HexViewControl, ImageMap?>(
            nameof(ImageMap),
            o => o.ImageMap,
            (o, v) => o.ImageMap = v);

    public static readonly DirectProperty<HexViewControl, MemoryArea?> MemoryAreaProperty =
        AvaloniaProperty.RegisterDirect<HexViewControl, MemoryArea?>(
            nameof(MemoryArea),
            o => o.MemoryArea,
            (o, v) => o.MemoryArea = v);

    public static readonly DirectProperty<HexViewControl, SegmentMap?> SegmentMapProperty =
        AvaloniaProperty.RegisterDirect<HexViewControl, SegmentMap?>(
            nameof(SegmentMap),
            o => o.SegmentMap,
            (o, v) => o.SegmentMap = v);

    public static readonly DirectProperty<HexViewControl, Address?> SelectedAddressProperty =
        AvaloniaProperty.RegisterDirect<HexViewControl, Address?>(
            nameof(SelectedAddress),
            o => o.SelectedAddress,
            (o, v) => o.SelectedAddress = v);

    public static readonly DirectProperty<HexViewControl, Address?> TopAddressProperty =
        AvaloniaProperty.RegisterDirect<HexViewControl, Address?>(
            nameof(TopAddress),
            o => o.SelectedAddress,
            (o, v) => o.TopAddress = v);

    /// <summary>
    /// Defines the <see cref="Background"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<HexViewControl>();

    public static readonly StyledProperty<IBrush?> SelectionBrushProperty =
        TextBox.SelectionBrushProperty.AddOwner<HexViewControl>();

    public static readonly StyledProperty<IBrush?> SelectionForegroundBrushProperty =
        TextBox.SelectionForegroundBrushProperty.AddOwner<HexViewControl>();

    static HexViewControl()
    {
        FocusableProperty.OverrideDefaultValue<HexViewControl>(true);
        AffectsRender<HexViewControl>(SelectedAddressProperty);
        AffectsRender<HexViewControl>(AnchorAddressProperty);
        AffectsRender<HexViewControl>(TopAddressProperty);
        AffectsRender<HexViewControl>(ArchitectureProperty);
        AffectsRender<HexViewControl>(ImageMapProperty);
    }

    private readonly MemoryControlPainter mcp;
    private volatile bool _updating;
    private Size _extent;
    private Size _viewport;
    private Vector _offset;
    private bool _canHorizontallyScroll;
    private bool _canVerticallyScroll;
    private EventHandler? _scrollInvalidated;
    private Typeface _typeface;
    private FontFamily? _fontFamily;
    private double _fontSize;
    private IBrush _foreground = default!;
    private Size _scrollSize = new(1, 1);
    private Size _pageScrollSize = new(10, 10);

    private uint wordSize;
    private Address? addrMin;
    private ulong memSize;

    private int cRows;              // total number of rows.
    private int yTopRow;            // index of topmost visible row
    private int cyPage;             // number of rows / page.
    private Size cellSize;          // size of cell in pixels.
    private Point ptDown;            // point at which mouse was clicked.

    private bool isMouseClicked;
    private bool isDragging;
    private bool isTextSideSelected;

    public HexViewControl()
    {
        this.mcp = new MemoryControlPainter(this);
        this.encoding = Encoding.ASCII;
        this.cbRow = 16;
        this.wordSize = 1;
    }

    public int ToBase
    {
        get => GetValue(ToBaseProperty);
        set => SetValue(ToBaseProperty, value);
    }

    [Browsable(false)]
    public IServiceProvider? Services
    {
        get { return services; }
        set { this.services = value; }
    }
    private IServiceProvider? services;

    public int BytesWidth
    {
        get => GetValue(BytesWidthProperty);
        set => SetValue(BytesWidthProperty, value);
    }

    public IBrush? CodeBackground
    {
        get => GetValue(CodeBackgroundProperty);
        set => SetValue(CodeBackgroundProperty, value);
    }

    public IBrush? DataBackground
    {
        get => GetValue(DataBackgroundProperty);
        set => SetValue(DataBackgroundProperty, value);
    }

    public IProcessorArchitecture? Architecture
    {
        get => this.arch;
        set => this.SetAndRaise(ArchitectureProperty, ref this.arch, value);
    }
    private IProcessorArchitecture? arch;

    /// <summary>
    /// Gets or sets a brush used to paint the control's background.
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public IBrush? SelectionBrush
    {
        get => GetValue(SelectionBrushProperty);
        set => SetValue(SelectionForegroundBrushProperty, value);
    }

    public IBrush? SelectionForegroundBrush
    {
        get => GetValue(SelectionForegroundBrushProperty);
        set => SetValue(SelectionForegroundBrushProperty, value);
    }

    [Browsable(false)]
    public bool IsTextSideSelected
    {
        get => this.isTextSideSelected;
        set
        {
            if (this.isTextSideSelected == value)
                return;
            this.isTextSideSelected = value;
            InvalidateVisual();
        }
    }

    public Address? AnchorAddress
    {
        get => this.addrAnchor;
        set
        {
            if (this.addrAnchor == value)
                return;
            this.addrAnchor = value;
            var seg = GetSegmentFromAddress(value);
            if (seg is not null)
            {
                ChangeMemoryArea(seg);
            }
            InvalidateVisual();
        }
    }
    private Address? addrAnchor;


    [Browsable(false)]
    public Encoding Encoding
    {
        get => this.encoding;
        set => this.SetAndRaise(EncodingProperty, ref this.encoding, value);
    }
    private Encoding encoding;

    public MemoryArea? MemoryArea
    {
        get => this.mem;
        set
        {
            if (this.SetAndRaise(MemoryAreaProperty, ref this.mem, value))
            {
                OnMemoryAreaChanged();
            }
        }
    }
    private MemoryArea? mem;

    public ImageMap? ImageMap
    {
        get { return imageMap; }
        set { this.SetAndRaise(ImageMapProperty, ref imageMap, value); }
    }
    private ImageMap? imageMap;

    public IDictionary<Address, Procedure> Procedures { get; set; } = new Dictionary<Address, Procedure>();    //$DEBUG

    public SegmentMap? SegmentMap
    {
        get => this.segmentMap;
        set
        {
            if (this.segmentMap == value)
                return;
            if (segmentMap is not null)
                segmentMap.MapChanged -= imageMap_MapChanged;
            segmentMap = value;
            if (segmentMap is not null)
                segmentMap.MapChanged += imageMap_MapChanged;
            InvalidateVisual();
            this.SetAndRaise(SegmentMapProperty, ref segmentMap, value);
        }
    }
    private SegmentMap? segmentMap;

    public Address? SelectedAddress
    {
        get { return addrSelected; }
        set
        {
            if (!this.SetAndRaise(SelectedAddressProperty, ref addrSelected, value))
                return;
            var seg = GetSegmentFromAddress(value);
            if (seg is not null)
            {
                ChangeMemoryArea(seg);
            }
            InvalidateVisual();
        }
    }
    private Address? addrSelected;

    public Address? TopAddress
    {
        get { return addrTopVisible; }
        set
        {
            if (!this.SetAndRaise(TopAddressProperty, ref addrTopVisible, value))
                return;
            trace.Inform($"TopAddress: new value {TopAddress}");
            if (value is not null)
                value -= (int) (value.Value.ToLinear() % cbRow);
            addrTopVisible = value;
            InvalidateScrollable();
            InvalidateVisual();
        }
    }
    private Address? addrTopVisible;     // address of topmost visible row.

    /// <summary>
    /// Returns the selection as an address range. Note that the range is 
    /// a closed interval in the address space.
    /// </summary>
    public AddressRange GetAddressRange()
    {
        if (SelectedAddress is null || addrAnchor is null)
        {
            return AddressRange.Empty;
        }
        else
        {
            if (SelectedAddress <= addrAnchor)
            {
                return new AddressRange(SelectedAddress.Value, addrAnchor.Value);
            }
            else
            {
                return new AddressRange(addrAnchor.Value, SelectedAddress.Value);
            }
        }
    }

    Size IScrollable.Extent => _extent;

    Vector IScrollable.Offset
    {
        get => _offset;
        set
        {
            if (_updating || addrMin is null)
            {
                return;
            }
            _updating = true;
            _offset = CoerceOffset(value);
            addrTopVisible = addrMin + cbRow * (uint) _offset.Y;
            InvalidateScrollable();
            _updating = false;
        }
    }

    Size IScrollable.Viewport => _viewport;

    bool ILogicalScrollable.CanHorizontallyScroll
    {
        get => _canHorizontallyScroll;
        set
        {
            _canHorizontallyScroll = value;
            InvalidateMeasure();
        }
    }

    bool ILogicalScrollable.CanVerticallyScroll
    {
        get => _canVerticallyScroll;
        set
        {
            _canVerticallyScroll = value;
            InvalidateMeasure();
        }
    }

    bool ILogicalScrollable.IsLogicalScrollEnabled => true;

    event EventHandler? ILogicalScrollable.ScrollInvalidated
    {
        add => _scrollInvalidated += value;
        remove => _scrollInvalidated -= value;
    }

    Size ILogicalScrollable.ScrollSize => _scrollSize;

    Size ILogicalScrollable.PageScrollSize => _pageScrollSize;

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
        _scrollInvalidated?.Invoke(this, e);
    }

    private Vector CoerceOffset(Vector value)
    {
        var scrollable = (ILogicalScrollable) this;
        var maxX = Math.Max(scrollable.Extent.Width - scrollable.Viewport.Width, 0);
        var maxY = Math.Max(scrollable.Extent.Height - scrollable.Viewport.Height, 0);
        return new Vector(Clamp(value.X, 0, maxX), Clamp(value.Y, 0, maxY));
        static double Clamp(double val, double min, double max) => val < min ? min : val > max ? max : val;
    }

    private FormattedText CreateFormattedText(string text)
    {
        return new FormattedText(text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            _typeface,
            _fontSize,
            _foreground);
    }

    private FormattedText CreateFormattedText(string text, IBrush foreground)
    {
        return new FormattedText(text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            _typeface,
            _fontSize,
            foreground);
    }

    protected override void OnLoaded(RoutedEventArgs routedEventArgs)
    {
        base.OnLoaded(routedEventArgs);

        Invalidate();
        InvalidateScrollable();
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
            Invalidate();
            InvalidateScrollable();
        }

        if (change.Property == ToBaseProperty)
        {
            InvalidateVisual();
        }

        if (change.Property == BytesWidthProperty)
        {
            InvalidateScrollable();
        }
    }

    private void Invalidate()
    {
        _fontFamily = TextElement.GetFontFamily(this);
        _fontSize = TextElement.GetFontSize(this);
        _foreground = TextElement.GetForeground(this)!;

        _typeface = new Typeface(_fontFamily);
        var m = CreateFormattedText("M");
        this.cellSize = new Size(Math.Floor(m.Width), Math.Ceiling(m.Height));
    }

    /// <summary>
    /// Recompute logical scroll properties.
    /// </summary>
    public void InvalidateScrollable()
    {
        if (this is not ILogicalScrollable scrollable)
        {
            return;
        }

        var lines = ComputeTotalLines(MemoryArea);
        var width = Bounds.Width;
        var height = Bounds.Height;

        _scrollSize = new Size(1, 1);
        _pageScrollSize = new Size(_viewport.Width, Math.Floor(_viewport.Height / this.cellSize.Height));
        _extent = new Size(width, lines);
        _viewport = new Size(width, height / this.cellSize.Height);
        trace.Verbose("HVC: lines: {0}: viewport {1}", lines, _viewport);
        scrollable.RaiseScrollInvalidated(EventArgs.Empty);

        InvalidateVisual();
    }

    public uint BytesPerRow
    {
        get { return cbRow; }
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("BytesPerRow must be positive.");
            InvalidateScrollable();
            cbRow = value;
        }
    }
    private uint cbRow;

    private Size CellSize
    {
        get { return cellSize; }
    }

    const int UnitsPerLine = 16;

    private int ComputeTotalLines(MemoryArea? mem)
    {
        if (mem is null)
            return 0;
        long lines = (mem.Length + UnitsPerLine - 1) / UnitsPerLine;
        return (int) lines;
    }

    public Point AddressToPoint(Address addr)
    {
        if (TopAddress is null)
            return new Point();
        int cbOffset = (int) (addr - TopAddress);
        int yRow = cbOffset / (int) cbRow;
        if (yTopRow <= yRow && yRow < yTopRow + cyPage)
        {
            return new Point(60, yRow * cellSize.Height);
        }
        else
            return new Point();
    }

    private bool IsVisibleAddress(Address addr)
    {
        if (TopAddress is null)
            return false;
        int cbOffset = (int) (addr - TopAddress);
        int yRow = cbOffset / (int) cbRow;
        return (yTopRow <= yRow && yRow < yTopRow + _viewport.Height);
    }

    private void MoveSelection(int offset, KeyModifiers modifiers)
    {
        var addrTop = TopAddress;
        if (SelectedAddress is null || segmentMap is null || mem is null || addrTop is null)
            return;
        ulong linAddr = (ulong) ((long) SelectedAddress.Value.ToLinear() + offset);
        if (!mem.IsValidLinearAddress(linAddr))
            return;
        Address addr = segmentMap.MapLinearAddressToAddress(linAddr);
        if (!IsVisibleAddress(SelectedAddress.Value))
        {
            Address newTopAddress = addrTop.Value + offset;
            if (mem.IsValidAddress(newTopAddress))
            {
                TopAddress = newTopAddress;
            }
        }
        if ((modifiers & KeyModifiers.Shift) != KeyModifiers.Shift)
        {
            addrAnchor = addr;
        }
        this.SelectedAddress = addr;
    }


    private void OnMemoryAreaChanged()
    {
        if (mem is null)
        {
            TopAddress = null;
            addrMin = null;
            memSize = 0;
        }
        else
        {
            TopAddress = mem.BaseAddress;
            addrMin = mem.BaseAddress;
            this.addrMin = mem.BaseAddress;
            this.memSize = (ulong) mem.Length;
            TopAddress = addrMin;
            cbRow = 16;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        trace.Verbose("K: {0}, M: {1}", e.Key, e.KeyModifiers);
        switch (e.Key)
        {
        case Key.Down:
            MoveSelection((int) cbRow, e.KeyModifiers);
            break;
        case Key.Up:
            MoveSelection(-(int) cbRow, e.KeyModifiers);
            break;
        case Key.Left:
            MoveSelection(-(int) wordSize, e.KeyModifiers);
            break;
        case Key.Right:
            MoveSelection((int) wordSize, e.KeyModifiers);
            break;
        case Key.F10:
            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                var addrRange = GetAddressRange();
                if (this.ContextMenu is not null && addrRange.IsValid)
                {
                    trace.Error("//$NYI: show menu this.ContextMenu.Show(this, AddressToPoint(addrRange.Begin)");
                }
            }
            break;
        default:
            base.OnKeyDown(e);
            return;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        Focus();
        if (mem is null)
            return;
        this.isMouseClicked = true;
        ptDown = e.GetCurrentPoint(this).Position;
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            AffectSelection(ptDown, MouseButton.Left, e.KeyModifiers);
        base.OnPointerPressed(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (!isMouseClicked)
            return;
        var pt = e.GetCurrentPoint(this).Position;
        if (Math.Abs(ptDown.X - pt.X) < 3 && Math.Abs(ptDown.Y - pt.Y) < 3)
            return;
        isDragging = true;
        e.Pointer.Capture(this);
        AffectSelection(pt, MouseButton.Left, e.KeyModifiers);
        base.OnPointerMoved(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (isDragging)
        {
            e.Pointer.Capture(null);
        }
        isMouseClicked = false;
        if (mem is not null)
        {
            AffectSelection(e.GetCurrentPoint(this).Position, e.InitialPressMouseButton, e.KeyModifiers);
        }
        isDragging = false;
        base.OnPointerReleased(e);
    }


    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        if (mem is null)
            return;
        Debug.Assert(TopAddress is not null);
        var newTopAddress = TopAddress.Value + (int) (e.Delta.Y > 0 ? -cbRow : cbRow);
        if (mem.IsValidAddress(newTopAddress))
        {
            TopAddress = newTopAddress;
        }
    }

    private void AffectSelection(Point pt, MouseButton mouseButton, KeyModifiers keyModifiers)
    {
        var (addrClicked, textSide) = mcp.PaintWindow(null!, cellSize, new Point(pt.X, pt.Y), false);
        trace.Verbose($"AffectSelection: {addrClicked}");
        if (ShouldChangeSelection(mouseButton, addrClicked, textSide))
        {
            if (!isDragging || addrClicked is not null)
                this.SelectedAddress = addrClicked;
            if ((keyModifiers & KeyModifiers.Shift) != KeyModifiers.Shift &&
                !isDragging)
            {
                addrAnchor = addrClicked;
            }
            this.IsTextSideSelected = textSide;
            trace.Inform("AffectSelection: {0}-{1}", addrAnchor!, SelectedAddress!);
            InvalidateVisual();
        }
    }

    private bool ShouldChangeSelection(MouseButton mouseButton, Address? addrClicked, bool textSide)
    {
        if (mouseButton == MouseButton.Left)
            return true;
        if (addrClicked is null)
            return true;
        if (IsTextSideSelected != textSide)
            return true;

        return !IsAddressInSelection(addrClicked.Value);
    }

    private bool IsAddressInSelection(Address addr)
    {
        if (addrAnchor is null || SelectedAddress is null)
            return false;
        var linAddr = addr.ToLinear();
        var linAnch = addrAnchor.Value.ToLinear();
        var linSel = SelectedAddress.Value.ToLinear();
        if (linAnch <= linSel)
        {
            return linAnch <= linAddr && linAddr <= linSel;
        }
        else
        {
            return linSel <= linAddr && linAddr <= linAnch;
        }
    }

    public override void Render(DrawingContext context)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        base.Render(context);
        if (mem is null || segmentMap is null || imageMap is null || arch is null ||
            MemoryArea is null || BytesWidth <= 0)
        {
            context.DrawRectangle(Brushes.Transparent, null, Bounds);
            return;
        }

        var toBase = ToBase;
        var bytesWidth = BytesWidth;

        var rek = new MemoryFormatter(PrimitiveType.Byte, 1, bytesWidth, 2, 1);
        var startOffset = (long) Math.Ceiling(_offset.Y / cellSize.Height) * bytesWidth;
        var linesToRender = (int) Math.Ceiling(_viewport.Height / cellSize.Height);
        var rdr = this.MemoryArea.CreateLeReader(startOffset);
        mcp.PaintWindow(context, cellSize, Bounds.TopLeft, true);
        sw.Stop();
        Debug.WriteLine($"Rendered hex view control in {sw.ElapsedMilliseconds} msec");
    }

    private void ChangeMemoryArea(ImageSegment seg)
    {
        mem = seg.MemoryArea;
        if (mem is not null)
        {
            this.addrMin = Address.Max(mem.BaseAddress, seg.Address);
            this.memSize = Math.Min((ulong) mem.Length, seg.Size);
            TopAddress = addrMin;
            InvalidateScrollable();
        }
        else
        {
            InvalidateScrollable();
            Invalidate();
        }
    }


    private ImageSegment? GetSegmentFromAddress(Address? addr)
    {
        if (ImageMap is null || addr is null || SegmentMap is null)
            return null;
        if (!SegmentMap.TryFindSegment(addr.Value, out ImageSegment? seg))
            return null;
        return seg;
    }


    public uint WordSize
    {
        get { return wordSize; }
        set
        {
            wordSize = value;
            InvalidateScrollable();
            Invalidate();
        }
    }

    private void imageMap_MapChanged(object? sender, EventArgs e)
    {
        InvalidateVisual();
    }

    public class MemoryControlPainter
    {
        private readonly HexViewControl ctrl;
        private Size cellSize;
        private BrushTheme? codeTheme;
        private BrushTheme? dataTheme;
        private BrushTheme? defaultTheme;
        private BrushTheme? selectTheme;
        private BrushTheme? secondarySelectTheme;
        private Brush secondarySelBrush;
        private Geometry arrowUl;
        private Geometry arrowLl;

        public MemoryControlPainter(HexViewControl ctrl)
        {
            this.ctrl = ctrl;
            //$TODO: derive brush from SystemColors.HighLight using HSB model
            this.secondarySelBrush = new SolidColorBrush(
                Color.FromArgb(0xFF, 0xD0, 0xD0, 0xFF));
            arrowUl = Geometry.Parse("M 0,0 l 4,0 l -4,4 Z");
            arrowLl = Geometry.Parse("M 0,0 l 4,0 l -4,-4 Z");
        }

        /// <summary>
        /// Paints the control's window area. Strategy is to find the spans that make up
        /// the whole segment, and paint them one at a time.
        /// </summary>
        /// <param name="g"></param>
        public (Address?, bool) PaintWindow(DrawingContext g, Size cellSize, Point ptAddr, bool render)
        {
            this.cellSize = cellSize;
            GetColorPreferences();

            if (ctrl.arch is null || ctrl.imageMap is null || ctrl.addrMin is null)
                return (null, ctrl.IsTextSideSelected);
            // Enumerate all segments visible on screen.

            var addrMin = ctrl.addrMin.Value;
            ulong laEnd = addrMin.ToLinear() + ctrl.memSize;
            if (ctrl.TopAddress!.Value.ToLinear() >= laEnd)
                return (null, ctrl.IsTextSideSelected);
            var addrStart = Address.Max(ctrl.TopAddress.Value, ctrl.mem!.BaseAddress);
            EndianImageReader rdr = ctrl.arch.CreateImageReader(ctrl.mem, addrStart);
            Rect rcClient = ctrl.Bounds;
            Rect rc = rcClient.WithHeight(cellSize.Height);
            Size cell = ctrl.cellSize;

            ulong laSegEnd = 0;
            while (rc.Top < rcClient.Height && (laEnd == 0 || rdr.Address.ToLinear() < laEnd))
            {
                if (ctrl.SegmentMap.TryFindSegment(ctrl.TopAddress.Value, out ImageSegment? seg))
                {
                    if (rdr.Address.ToLinear() >= laSegEnd)
                    {
                        laSegEnd = seg.Address.ToLinear() + seg.Size;
                        rdr = ctrl.arch.CreateImageReader(ctrl.mem, seg.Address + (rdr.Address - seg.Address));
                    }
                }
                var formatter = ctrl.mem.Formatter;
                var linePainter = new LinePainter(this, g, rc, ptAddr, render);
                var (addr, textSide) = linePainter.Paint(formatter, rdr, ctrl.Encoding);
                if (!render && addr is not null)
                    return (addr, textSide);
                rc = rc.WithY(rc.Y + ctrl.cellSize.Height);
            }
            var dyLeft = rcClient.Bottom - rc.Top;
            if (dyLeft > 0 && render)
            {
                var theme = GetBrushTheme(null, false);
                g.FillRectangle(theme.Background, new Rect(rcClient.Left, rc.Top, rcClient.Width, dyLeft));
            }
            return (null, ctrl.IsTextSideSelected);
        }

        private void GetColorPreferences()
        {
            //var uiPrefs = this.ctrl.Services.RequireService<IUiPreferencesService>();
            //var wind = uiPrefs.Styles[UiStyles.MemoryWindow];
            //var code = uiPrefs.Styles[UiStyles.MemoryCode];
            //var data = uiPrefs.Styles[UiStyles.MemoryData];
            //var heur = uiPrefs.Styles[UiStyles.MemoryHeuristic];
            var wind = new BrushTheme
            {
                Background = ctrl.Background ?? Brushes.Transparent,
                Foreground = ctrl._foreground
            };
            var data = new BrushTheme
            {
                Background = SolidColorBrush.Parse("#C0C0FF"),
                Foreground = ctrl._foreground
            };
            var heur = new BrushTheme
            {
                Background = SolidColorBrush.Parse("#FFE0E0"),
                Foreground = ctrl._foreground
            };
            codeTheme = new BrushTheme { Background = ctrl.CodeBackground!, Foreground = ctrl._foreground, StartMarker = Brushes.Red };
            dataTheme = new BrushTheme { Background = ctrl.DataBackground!, Foreground = ctrl._foreground, StartMarker = Brushes.Blue };
            defaultTheme = new BrushTheme { Background = wind.Background ?? ctrl.Background, Foreground = wind.Foreground ?? ctrl._foreground };
            selectTheme = new BrushTheme { Background = ctrl.SelectionBrush, Foreground = ctrl.SelectionForegroundBrush };
            secondarySelectTheme = new BrushTheme { Background = secondarySelBrush, Foreground = ctrl._foreground };
        }

        /// <summary>
        /// Paints a line of the memory control, starting with the address. 
        /// </summary>
        /// <remarks>

        private class LinePainter : IMemoryFormatterOutput
        {
            private readonly MemoryControlPainter mcp;
            private readonly DrawingContext g;
            private readonly Point ptAddr;
            private readonly bool render;
            private readonly ulong linearSelected;
            private readonly ulong linearAnchor;
            private readonly ulong linearBeginSelection;
            private readonly ulong linearEndSelection;
            private Rect rc;
            private Address? addrHit;
            private bool textSide;
            private int prepadding;

            public LinePainter(MemoryControlPainter mcp, DrawingContext g, Rect rc, Point ptAddr, bool render)
            {
                this.mcp = mcp;
                this.g = g;
                this.rc = rc;
                this.ptAddr = ptAddr;
                this.render = render;
                this.linearSelected = mcp.ctrl.SelectedAddress is not null ? mcp.ctrl.SelectedAddress.Value.ToLinear() : ~0UL;
                this.linearAnchor = mcp.ctrl.addrAnchor is not null ? mcp.ctrl.addrAnchor.Value.ToLinear() : ~0UL;
                this.linearBeginSelection = Math.Min(linearSelected, linearAnchor);
                this.linearEndSelection = Math.Max(linearSelected, linearAnchor);

                tracePaint.Verbose($"MemoryControl..ctor: {rc}");
            }

            public (Address?, bool) Paint(MemoryFormatter fmt, EndianImageReader rdr, Encoding enc)
            {
                fmt.RenderLine(rdr, enc, this);
                return (addrHit, textSide);
            }

            public void BeginLine()
            {
                rc = rc.WithX(0);
                addrHit = null;
                textSide = false;
                prepadding = 1;
            }

            public void EndLine(Constant[] bytes)
            {
                if (!render)
                    return;
                tracePaint.Verbose($"MemoryControl.EndLine {rc}");
                g.FillRectangle(mcp.defaultTheme.Background, rc);
            }

            public void RenderAddress(Address addr)
            {
                if (addrHit is not null)
                    return;
                string s = addr.ToString();
                var tAddr = mcp.ctrl.CreateFormattedText(s);
                double cx = tAddr.Width;
                var rcAddr = DeriveSubRectangle(cx);
                if (!render)
                {
                    if (rcAddr.Contains(mcp.ctrl.ptDown))
                    {
                        this.addrHit = addr;
                        return;
                    }
                }
                else
                {
                    g.FillRectangle(mcp.defaultTheme.Background, rcAddr);
                    var text = mcp.ctrl.CreateFormattedText(s);
                    g.DrawText(text, rcAddr.TopLeft);
                }
                cx += mcp.cellSize.Width / 2;
                AdvanceRc(cx);
            }

            public void RenderFillerSpan(int nChunks, int cellsPerChunk)
            {
                if (addrHit is not null)
                    return;

                double cx = mcp.cellSize.Width * (1 + cellsPerChunk) * nChunks;
                if (cx <= 0)
                    return;
                var rcFiller = DeriveSubRectangle(cx);
                if (render)
                {
                    var theme = mcp.GetBrushTheme(null, false);
                    g.FillRectangle(theme.Background, rcFiller);
                }
                AdvanceRc(cx);
            }

            public void RenderUnit(Address addUnit, string s)
            {
                if (addrHit is not null)
                    return;

                double cx = mcp.cellSize.Width * (s.Length + 1);
                var rcUnit = DeriveSubRectangle(cx);
                if (!render)
                {
                    if (rcUnit.Contains(ptAddr))
                    {
                        this.addrHit = addUnit;
                        return;
                    }
                }
                else
                {
                    DoRenderUnit(addUnit, s, rcUnit);
                }
                AdvanceRc(cx);
            }

            public void RenderUnitAsText(Address addrUnit, string sUnit)
            {
                if (addrHit is not null)
                    return;
                var cx = sUnit.Length * mcp.cellSize.Width;
                var rcUnit = DeriveSubRectangle(cx);
                if (!render)
                {
                    if (rcUnit.Contains(ptAddr))
                    {
                        this.addrHit = addrUnit;
                        this.textSide = true;
                        return;
                    }
                }
                else
                {
                    DoRenderUnitAsText(addrUnit, sUnit, rcUnit);
                }
                AdvanceRc(cx);
            }

            public void RenderTextFillerSpan(int padding)
            {
                if (addrHit is not null)
                    return;
                var cx = (padding + prepadding) * mcp.cellSize.Width;
                var rcPadding = DeriveSubRectangle(cx);
                if (render)
                {
                    g.FillRectangle(mcp.defaultTheme.Background, rcPadding);
                }
                prepadding = 0;
                AdvanceRc(cx);
            }

            private void AdvanceRc(double cx)
            {
                this.rc = new Rect(Math.Floor(rc.Left + cx), rc.Top, Math.Floor(rc.Width - cx), rc.Height);
                tracePaint.Verbose($"rc now: {rc}");
            }

            private Rect DeriveSubRectangle(double cx)
            {
                return new Rect(rc.Left, rc.Top, cx, rc.Height);
            }

            private void DoRenderUnit(Address addUnit, string s, in Rect rcUnit)
            {
                ImageMapItem? item = null;
                if (mcp.ctrl.ImageMap is not null)
                    mcp.ctrl.ImageMap.TryFindItem(addUnit, out item);
                bool isProcEntry = mcp.ctrl.Procedures?.ContainsKey(addUnit) ?? false;
                bool isSelected = linearBeginSelection <= addUnit.ToLinear() && addUnit.ToLinear() <= linearEndSelection;
                bool isCursor = addUnit.ToLinear() == linearSelected;

                var theme = mcp.GetBrushTheme(item, isSelected);
                g.FillRectangle(theme.Background, rcUnit);
                if (!isSelected && theme.StartMarker is not null)
                {
                    var x = rc.X;
                    var y = rc.Y;
                    if (item is not null &&
                    addUnit.ToLinear() == item.Address.ToLinear())
                    {
                        mcp.arrowUl.Transform = new TranslateTransform(x, y);
                        g.DrawGeometry(theme.StartMarker, null, mcp.arrowUl);
                    }
                    y = rc.Bottom;
                    if (isProcEntry)
                    {
                        mcp.arrowLl.Transform = new TranslateTransform(x, y);
                        g.DrawGeometry(theme.StartMarker, null, mcp.arrowLl);
                    }
                }
                var text = mcp.ctrl.CreateFormattedText(s);
                g.DrawText(text, new Point(rc.Left + mcp.cellSize.Width / 2, rc.Top));
                if (isCursor && !mcp.ctrl.IsTextSideSelected)
                {
                    DrawFocusRectangle(g, rc);
                }
            }

            private void DrawFocusRectangle(DrawingContext g, Rect rc)
            {
                tracePaint.Error("//$NYI: DrawFocusRectangle");
            }

            private void DoRenderUnitAsText(Address addrUnit, string sUnit, in Rect rcUnit)
            {
                bool isSelected = linearBeginSelection <= addrUnit.ToLinear() && addrUnit.ToLinear() <= linearEndSelection;
                var theme = mcp.GetTextSideBrushTheme(isSelected);
                g.FillRectangle(theme.Background!, rcUnit);
                var text = mcp.ctrl.CreateFormattedText(sUnit, theme.Foreground!);
                g.DrawText(text, rcUnit.TopLeft);
            }
        }

        private BrushTheme GetBrushTheme(ImageMapItem? item, bool selected)
        {
            if (item is null)
                return defaultTheme;
            if (selected)
            {
                if (ctrl.IsTextSideSelected)
                    return this.secondarySelectTheme;
                else
                    return selectTheme;
            }
            if (item is ImageMapBlock)
                return codeTheme;
            if (item.DataType is not null &&
                (item.DataType is not UnknownType ut ||
                 ut.Size > 0))
                return dataTheme;
            if (item is ImageMapVectorTable)
                return dataTheme;
            return defaultTheme;
        }

        private BrushTheme GetTextSideBrushTheme(bool isSelected)
        {
            if (isSelected)
            {
                if (ctrl.IsTextSideSelected)
                    return this.selectTheme;
                else
                    return this.secondarySelectTheme;
            }
            else
            {
                return this.defaultTheme;
            }
        }

        private class BrushTheme
        {
            public IBrush Foreground = default!;
            public IBrush Background = default!;
            public IBrush StartMarker = default!;
        }
    }


}

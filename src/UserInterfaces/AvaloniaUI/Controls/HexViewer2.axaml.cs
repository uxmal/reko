using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Reko.UserInterfaces.AvaloniaUI.Controls
{
    public partial class HexViewer2 : UserControl // , ILogicalScrollable
    {
        public HexViewer2()
        {
            this.InitializeComponent();

            this.spans = new List<HexViewer.ClientSpan>();
            this.lines = new List<RenderedLine>();
            this.cbLine = 16;
            this.cbCellStride = 1;
            this.endianness = EndianServices.Little;

            var rnd = new Random(0x4711);
            var mem = new ByteMemoryArea(Address.Ptr32(0), new byte[20000]);
            rnd.NextBytes(mem.Bytes);

            this.ClearVisualElements();
            this.GenerateVisualElements();
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        private static readonly TraceSwitch trace = new TraceSwitch(nameof(HexViewer2), "")
        {
            Level = TraceLevel.Verbose
        };


        public static DirectProperty<HexViewer2, MemoryArea?> MemoryAreaProperty =
            AvaloniaProperty.RegisterDirect<HexViewer2, MemoryArea?>(
                nameof(MemoryArea),
                o => o.MemoryArea,
                (o, v) => o.MemoryArea = v);

        public static DirectProperty<HexViewer2, EndianServices> EndiannessProperty =
            AvaloniaProperty.RegisterDirect<HexViewer2, EndianServices>(
                nameof(Endianness),
                o => o.Endianness,
                (o, v) => o.Endianness = v);

        public static DirectProperty<HexViewer2, Address?> TopAddressProperty =
            AvaloniaProperty.RegisterDirect<HexViewer2, Address?>(
                nameof(TopAddress),
                o => o.TopAddress,
                (o, v) => o.TopAddress = v);

        private MemoryArea? mem;
        private List<RenderedLine> lines;
        private List<HexViewer.ClientSpan> spans;
        private Address? addrTop;
        private EndianServices endianness;
        private int cbLine;
        private int cbCellStride;

        private class RenderedLine
        {
            public double yTop;
            public HexViewer.TextSpan[] spans = default!;
        }



        public EndianServices Endianness
        {
            get { return this.endianness; }
            set { this.SetAndRaise(EndiannessProperty, ref endianness, value); }
        }

        public MemoryArea? MemoryArea
        {
            get { return this.mem; }
            set
            {
                if (this.SetAndRaise(MemoryAreaProperty, ref this.mem, value))
                    OnMemoryAreaChanged();
            }
        }

        public Address? TopAddress
        {
            get { return this.addrTop; }
            set { SetAndRaise(TopAddressProperty, ref this.addrTop, value); }
        }

#if LOG
        bool ILogicalScrollable.CanHorizontallyScroll { get; set; }
        bool ILogicalScrollable.CanVerticallyScroll { get => vScroll; set { this.vScroll = value; } }

        bool ILogicalScrollable.IsLogicalScrollEnabled => true;

        Size ILogicalScrollable.ScrollSize => new Size(100, 1);

        Size ILogicalScrollable.PageScrollSize => new Size(100, 30);

        Size IScrollable.Extent => new Size(100, 300);

        Vector IScrollable.Offset { get; set; }

        Size IScrollable.Viewport => new Size(100, 30);
#endif

        protected override Size MeasureOverride(Size availableSize)
        {
            trace.Verbose($"{nameof(HexViewer2)}.{nameof(MeasureOverride)}: {availableSize}");
            if (mem is null)
                return new Size(0, 0);
            var rc = MeasureSingleLetter();
            long nLines = (mem.Length + cbLine - 1) / cbLine;
            return new Size(rc.Width * 100, rc.Height * nLines);
        }

        public override void Render(DrawingContext context)
        {
            var x = this.Content;
            var bounds = Bounds;
            var client = new Rect(0, 0, bounds.Width - 50, bounds.Height - 30);

            var bg = Background;
            if (bg is not null)
            {
                context.FillRectangle(bg, client);
            }

            var typeface = new Typeface(FontFamily);
            var txt = new FormattedText(
                "AB",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                FontSize,
                Foreground);


            context.DrawText(txt, new Point(0, 0));

            foreach (var span in spans)
            {
                span.Render(this, context, typeface, FontSize);
            }
        }

        /*
        protected override Size MeasureCore(Size availableSize)
        {
            if (mem is null)
                return new Size(0, 0);
            var rc = MeasureSingleLetter();
            long nLines = (mem.Length + cbLine - 1) / cbLine;
            return new Size(rc.Width * 100, rc.Height * nLines);
        }
        */
        /*
        protected override Size MeasureCore(Size availableSize)
        {
            var typeface = new Typeface(FontFamily);
            var txt = new FormattedText(
                "X",
                typeface,
                FontSize,
                TextAlignment.Center,
                TextWrapping.NoWrap,
                Size.Infinity);
            // We want 16* that formatted text, plus 15 paddings bet
            var b = txt.Bounds;
            var c = txt.Constraint;
            var wantedWidth = 16 * b.Width + 15 * b.Width;
            var wantedHeight = b.Height * Math.Ceiling(((ByteMemoryArea)mem).Bytes.Length / 16.0);
            return new Size(wantedWidth, wantedHeight);
        }
        */
        protected override void ArrangeCore(Rect finalRect)
        {
            base.ArrangeCore(finalRect);
            ClearVisualElements();
            GenerateVisualElements();
        }

        private void ClearVisualElements()
        {
            this.spans = new List<HexViewer.ClientSpan>();
            this.lines = new List<RenderedLine>();
        }

        private void GenerateVisualElements()
        {
            var mem = this.mem;
            if (mem is null)
                return;
            Rect bounds = MeasureSingleLetter();
            double dyLine = bounds.Height;
            double dxChar = bounds.Width;
            if (dyLine <= 0)
                return;
            double controlHeight = this.Bounds.Height;
            int nLines = (int)Math.Ceiling(controlHeight / dyLine);
            double yTopLine = 0;
            for (int i = 0; i < nLines; ++i, yTopLine += dyLine)
            {
                var line = RenderLine(i, yTopLine, dxChar, dyLine);
                this.lines.Add(line);
            }
            //spans.Add(new ClientSpan(new Rect(10, 10, 400, 300)));
            //spans.Add(new TextSpan("Hello Reko!", new Rect(20, 20, 100, 200)));
        }

        private Rect MeasureSingleLetter()
        {
            var txt = new FormattedText(
                "X",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily),
                FontSize,
                null);
            var bounds = new Rect(0, 0, txt.Width, txt.Height);
            return bounds;
        }

        private RenderedLine RenderLine(int i, double yTopLine, double dxChar, double dyLine)
        {
            var spans = new HexViewer.TextSpan[16];
            for (int b = 0; b < 16; ++b)
            {
                var offset = i * 16 + b;
                var sByte = this.mem!.TryReadByte(offset, out byte by)
                    ? by.ToString("X2") : "  ";

                var span = new HexViewer.TextSpan(sByte, new Rect(b * dxChar * 3, yTopLine, 3 * dxChar, dyLine));
                spans[b] = span;
                this.spans.Add(span);
            }
            return new RenderedLine
            {
                yTop = yTopLine,
                spans = spans
            };
        }

        private void GenerateLine(int offset, TextLayout layout)
        {
            //var l = new TextLine();
            Debug.Assert(mem is not null);
            var rdr = endianness.CreateImageReader(mem, offset);
            for (int i = 0; i < cbLine; i += cbCellStride)
            {
                if (rdr.TryReadByte(out var b))
                {
                    //layout.TextLines[0].
                }
            }
        }

        private void OnMemoryAreaChanged()
        {
            ClearVisualElements();
            var mem = this.MemoryArea;
            if (mem is not null)
            {
                GenerateVisualElements();
                this.TopAddress = mem.BaseAddress;
            }
        }

        //public event EventHandler? ScrollInvalidated;


#if LOGICAL_SCROLLABLE
        bool ILogicalScrollable.BringIntoView(IControl target, Rect targetRect)
        {
            throw new NotImplementedException();
        }

        IControl ILogicalScrollable.GetControlInDirection(NavigationDirection direction, IControl from)
        {
            throw new NotImplementedException();
        }

        void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
        {
            throw new NotImplementedException();
        }
#endif
    }
}

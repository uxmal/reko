using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.UserInterfaces.AvaloniaUI.Controls
{
    public partial class HexViewer : UserControl
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(HexViewer), "")
        {
            Level = TraceLevel.Verbose
        };


        public static DirectProperty<HexViewer, MemoryArea?> MemoryAreaProperty =
            AvaloniaProperty.RegisterDirect<HexViewer, MemoryArea?>(
                nameof(MemoryArea),
                o => o.MemoryArea,
                (o, v) => o.MemoryArea = v);

        public static DirectProperty<HexViewer, EndianServices> EndiannessProperty =
            AvaloniaProperty.RegisterDirect<HexViewer, EndianServices>(
                nameof(Endianness),
                o => o.Endianness,
                (o, v) => o.Endianness = v);

        public static DirectProperty<HexViewer, Address?> TopAddressProperty =
            AvaloniaProperty.RegisterDirect<HexViewer, Address?>(
                nameof(TopAddress),
                o => o.TopAddress,
                (o, v) => o.TopAddress = v);

        private MemoryArea? mem;
        private List<RenderedLine> lines;
        private List<ClientSpan> spans;
        private Address? addrTop;
        private EndianServices endianness;
        private int cbLine;
        private int cbCellStride;

        public HexViewer()
        {
            this.spans = new List<ClientSpan>();
            this.lines = new List<RenderedLine>();
            this.cbLine = 16;
            this.cbCellStride = 1;
            this.endianness = EndianServices.Little;

            var rnd = new Random(0x4711);
            var mem = new ByteMemoryArea(Address.Ptr32(0), new byte[20000]);
            rnd.NextBytes(mem.Bytes);

            this.InitializeComponent();
            this.ClearVisualElements();
            this.GenerateVisualElements();
        }


        private void InitializeComponent()
        {
        }

       

        private class RenderedLine
        {
            public double yTop;
            public TextSpan[] spans = default!;
        }

        

        public EndianServices Endianness
        {
            get { return this.endianness; }
            set { this.SetAndRaise(EndiannessProperty, ref endianness, value); }
        }

        public MemoryArea? MemoryArea
        {
            get { return this.mem; }
            set { if (this.SetAndRaise(MemoryAreaProperty, ref this.mem, value))
                    OnMemoryAreaChanged();
            }
        }

        public Address? TopAddress
        {
            get { return this.addrTop; }
            set { SetAndRaise(TopAddressProperty, ref this.addrTop, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            trace.Verbose($"{nameof(HexViewer)}.{nameof(MeasureOverride)}: {availableSize}");
            return base.MeasureOverride(availableSize);
        }

        public override void Render(DrawingContext context)
        {
            var x = this.Content;
            var bounds = Bounds;
            var client = new Rect(0, 0, bounds.Width-50, bounds.Height-30);

            var bg = Background;
            if (bg is not null)
            {
                context.FillRectangle(bg, client);
            }

            var typeface = new Typeface(FontFamily);
            var txt = new FormattedText(
                "AB",
                typeface,
                FontSize,
                TextAlignment.Center,
                TextWrapping.NoWrap,
                Size.Infinity);


            context.DrawText(Foreground, new Point(0, 0), txt);

            foreach (var span in spans)
            {
                span.Render(this, context, typeface, FontSize);
            }
        }

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
            var wantedHeight = b.Height * Math.Ceiling(((ByteMemoryArea)mem!).Bytes.Length / 16.0);
            return new Size(wantedWidth, wantedHeight);
        }

        protected override void ArrangeCore(Rect finalRect)
        {
            base.ArrangeCore(finalRect);
            ClearVisualElements();
            GenerateVisualElements();
        }

        private void ClearVisualElements()
        {
            this.spans = new List<ClientSpan>();
            this.lines = new List<RenderedLine>();
        }

        private void GenerateVisualElements()
        {
            var mem = this.mem;
            if (mem is null)
                return;
            var txt = new FormattedText(
                "X",
                new Typeface(FontFamily),
                FontSize,
                TextAlignment.Center,
                TextWrapping.NoWrap,
                Size.Infinity);
            double dyLine = txt.Bounds.Height;
            double dxChar = txt.Bounds.Width;
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

        private RenderedLine RenderLine(int i, double yTopLine, double dxChar, double dyLine)
        {
            var spans = new TextSpan[16];
            double x = 0;
            for (int b = 0; b < 16; ++b)
            {
                var offset = i * 16 + b;
                var sByte = this.mem!.TryReadByte(offset, out byte by)
                    ? by.ToString("X2") : "  ";

                var span = new TextSpan(sByte, new Rect(b*dxChar*3, yTopLine, 3 * dxChar, dyLine));
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

    }
}

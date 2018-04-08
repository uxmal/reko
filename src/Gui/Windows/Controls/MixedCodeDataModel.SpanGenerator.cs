#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Reko.Gui.Windows.Controls
{
   public partial class MixedCodeDataModel
   {
        private bool TryReadComment(Address addrCur, out LineSpan line)
        {
            if (comments.TryGetValue(addrCur, out var commentLines) &&
                commentOffset < commentLines.Length)
            {
                line = new LineSpan(
                    addrCur,
                    new MemoryTextSpan(
                        $"; {commentLines[commentOffset]}",
                        UiStyles.MemoryWindow));
                this.commentOffset++;
                return true;
            }
            line = default(LineSpan);
            return false;
        }

        public LineSpan[] GetLineSpans(int count)
        {
            addrCur = SanitizeAddress(addrCur);

            var spans = new List<LineSpan>();
            program.SegmentMap.TryFindSegment(addrCur, out var seg);
            program.ImageMap.TryFindItem(addrCur, out var item);

            SpanGenerator sp = CreateSpanifier(item, addrCur);
            while (count != 0 && seg != null && item != null)
            {
                if (TryReadComment(addrCur, out var commentLine))
                {
                    spans.Add(commentLine);
                    --count;
                    continue;
                }
                this.commentOffset = 0;
                bool memValid = true;
                if (!item.IsInRange(addrCur))
                {
                    memValid = program.ImageMap.TryFindItem(addrCur, out item)
                        && addrCur < item.EndAddress;
                    if (memValid)
                        sp = CreateSpanifier(item, addrCur);
                }
                memValid &= seg.MemoryArea.IsValidAddress(addrCur);

                if (memValid)
                {
                    var tuple = sp.GenerateSpan();
                    if (tuple != null)
                    {
                        addrCur = tuple.Item1;
                        spans.Add(tuple.Item2);
                        --count;
                    }
                    else
                    {
                        sp = null;
                    }
                }
                if (sp == null || !memValid)
                {
                    if (!memValid)
                    {
                        // Find next segment.
                        if (program.SegmentMap.Segments.TryGetUpperBoundKey(addrCur, out Address addrSeg))
                        {
                            program.SegmentMap.TryFindSegment(addrSeg, out seg);
                            program.ImageMap.TryFindItem(addrSeg, out item);
                            addrCur = addrSeg;
                        }
                        else
                        {
                            seg = null;
                            item = null;
                            addrCur = addrEnd;
                            break;
                        }
                    }
                    sp = CreateSpanifier(item, addrCur);
                }
            }
            addrCur = SanitizeAddress(addrCur);
            var aSpans = spans.ToArray();
            return aSpans;
        }

        private SpanGenerator CreateSpanifier(ImageMapItem item, Address addr)
        {
            SpanGenerator sp;
            if (item is ImageMapBlock b)
            {
                sp = new AsmSpanifyer(program, instructions[b], addr);
            }
            else
            {
                sp = new MemSpanifyer(program, item, addr);
            }
            return sp;
        }

        public abstract class SpanGenerator
        {
            public abstract Tuple<Address, LineSpan> GenerateSpan();

            public void DecorateLastLine(LineSpan line)
            {
                for (int i = 0; i < line.TextSpans.Length; ++i)
                {
                    var span = line.TextSpans[i];
                    if (span.Style == null)
                        span.Style = "lastLine";
                    else
                        span.Style = span.Style + " lastLine";
                }
            }
        }

        public class AsmSpanifyer : SpanGenerator
        {
            private Program program;
            private MachineInstruction[] instrs;
            private int offset;

            public AsmSpanifyer(Program program, MachineInstruction[] instrs, Address addr)
            {
                this.instrs = instrs;
                this.offset = FindIndexOfInstructionAddress(instrs, addr);
                this.program = program;
            }

            public override Tuple<Address, LineSpan> GenerateSpan()
            {
                if (offset >= instrs.Length || offset < 0)
                    return null;
                var instr = instrs[offset];
                ++offset;
                var asmLine = DisassemblyTextModel.RenderAsmLine(program, instr, MachineInstructionWriterOptions.ResolvePcRelativeAddress);
                if (offset == instrs.Length)
                {
                    DecorateLastLine(asmLine);
                }
                return Tuple.Create(instr.Address + instr.Length, asmLine);
            }
        }

        public class MemSpanifyer : SpanGenerator
        {
            private Program program;
            public Address addr;
            public ImageMapItem item;

            public MemSpanifyer(Program program, ImageMapItem item, Address addr)
            {
                this.program = program;
                this.item = item;
                this.addr = addr;
            }

            public override Tuple<Address, LineSpan> GenerateSpan()
            {
                var line = new List<TextSpan>
                {
                    new AddressSpan(addr.ToString(), addr, UiStyles.MemoryWindow)
                };

                var addrStart = Align(addr, BytesPerLine);
                var addrEnd = Address.Min(addrStart + BytesPerLine, item.Address + item.Size);
                var linStart = addrStart.ToLinear();
                var linEnd = addrEnd.ToLinear();
                var cbFiller = addr.ToLinear() - linStart;
                var cbBytes = linEnd - addr.ToLinear();
                var cbPadding = BytesPerLine - (cbFiller + cbBytes);

                var abCode = new List<byte>();

                // Do any filler first

                if (cbFiller > 0)
                {
                    line.Add(new MemoryTextSpan(new string(' ', 3 * (int)cbFiller), UiStyles.MemoryWindow));
                }

                var rdr = program.CreateImageReader(addr);
                while (rdr.Address.ToLinear() < linEnd)
                {
                    if (rdr.IsValid)
                    {
                        var addr1 = rdr.Address;
                        byte b = rdr.ReadByte();
                        line.Add(new MemoryTextSpan(addr1, string.Format(" {0:X2}", b), UiStyles.MemoryWindow));
                        //$BUG: should use platform.Encoding
                        abCode.Add(b);
                    }
                    else
                    {
                        cbPadding = linEnd - rdr.Address.ToLinear();
                        addrEnd = rdr.Address;
                        break;
                    }
                }

                // Do any padding after.

                if (cbPadding > 0)
                {
                    line.Add(new MemoryTextSpan(new string(' ', 3 * (int)cbPadding), UiStyles.MemoryWindow));
                }

                // Now display the code bytes.
                string sBytes = RenderBytesAsText(abCode.ToArray());
                line.Add(new MemoryTextSpan(" ", UiStyles.MemoryWindow));
                line.Add(new MemoryTextSpan(sBytes, UiStyles.MemoryWindow));

                var linePos = this.addr;
                this.addr = addrEnd;
                var memLine = new LineSpan(linePos, line.ToArray());
                if (rdr.Address >= item.EndAddress)
                {
                    DecorateLastLine(memLine);
                }
                return Tuple.Create(addrEnd, memLine);
            }

            private string RenderBytesAsText(byte[] abCode)
            {
                var chars = program.TextEncoding.GetChars(abCode.ToArray());
                for (int i = 0; i < chars.Length; ++i)
                {
                    char ch = chars[i];
                    if (char.IsControl(ch) || char.IsSurrogate(ch) || (0xE000 <= ch && ch <= 0xE0FF))
                        chars[i] = '.';
                }

                return new string(chars);
            }
        }

        //$PERF: could benefit from a binary search, but basic blocks
        // are so small it may not make a difference.
        public static int FindIndexOfInstructionAddress(MachineInstruction[] instrs, Address addr)
        {
            return Array.FindIndex(
                instrs,
                i => i.Contains(addr));
        }

        /// <summary>
        /// An segment of memory
        /// </summary>
        public class MemoryTextSpan : TextSpan
        {
            private string text;

            public Address Address { get; private set; }

            public MemoryTextSpan(string text, string style)
            {
                this.text = text;
                base.Style = style;
            }

            public MemoryTextSpan(Address address, string text, string style) : this(text, style)
            {
                this.Tag = this;
                this.Address = address;
            }

            public override string GetText()
            {
                return text;
            }

            public override SizeF GetSize(string text, Font font, Graphics g)
            {
                SizeF sz = base.GetSize(text, font, g);
                return sz;
            }
        }

        /// <summary>
        /// An inert text span is not clickable nor has a context menu.
        /// </summary>
        public class InertTextSpan : TextSpan
        {
            private string text;

            public InertTextSpan(string text, string style)
            {
                this.text = text;
                base.Style = style;
            }

            public override string GetText()
            {
                return text;
            }

            public override SizeF GetSize(string text, Font font, Graphics g)
            {
                SizeF sz = base.GetSize(text, font, g);
                return sz;
            }
        }

    }
}

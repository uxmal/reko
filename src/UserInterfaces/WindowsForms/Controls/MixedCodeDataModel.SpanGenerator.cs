#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
   public partial class MixedCodeDataModel
   {
        private bool TryReadComment(out LineSpan line)
        {
            if (comments.TryGetValue(curPos.Address, out var commentLines) &&
                curPos.Offset < commentLines.Length)
            {
                line = new LineSpan(
                    curPos,
                    new MemoryTextSpan(
                        $"; {commentLines[curPos.Offset]}",
                        UiStyles.CodeComment));
                curPos = Pos(curPos.Address, curPos.Offset + 1);
                return true;
            }
            line = default(LineSpan);
            return false;
        }

        public LineSpan[] GetLineSpans(int count)
        {
            curPos = SanitizePosition(curPos);

            var spans = new List<LineSpan>();
            program.SegmentMap.TryFindSegment(curPos.Address, out var seg);
            program.ImageMap.TryFindItem(curPos.Address, out var item);

            SpanGenerator sp = CreateSpanifier(item, curPos);
            while (count != 0 && seg != null && item != null)
            {
                if (TryReadComment(out var commentLine))
                {
                    spans.Add(commentLine);
                    --count;
                    continue;
                }
                bool memValid = true;
                if (!item.IsInRange(curPos.Address))
                {
                    memValid = program.ImageMap.TryFindItem(
                        curPos.Address, out item)
                        && curPos.Address < item.EndAddress;
                    if (memValid)
                        sp = CreateSpanifier(item, curPos);
                }
                memValid &= seg.MemoryArea.IsValidAddress(curPos.Address);

                if (memValid)
                {
                    var tuple = sp.GenerateSpan();
                    if (tuple != null)
                    {
                        curPos = tuple.Item1;
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
                        if (program.SegmentMap.Segments.TryGetUpperBoundKey(
                            curPos.Address, out var addrSeg))
                        {
                            program.SegmentMap.TryFindSegment(addrSeg, out seg);
                            program.ImageMap.TryFindItem(addrSeg, out item);
                            curPos = Pos(addrSeg);
                        }
                        else
                        {
                            seg = null;
                            item = null;
                            curPos = endPos;
                            break;
                        }
                    }
                    sp = CreateSpanifier(item, curPos);
                }
            }
            curPos = SanitizePosition(curPos);
            var aSpans = spans.ToArray();
            return aSpans;
        }

        private SpanGenerator CreateSpanifier(
            ImageMapItem item,
            ModelPosition pos)
        {
            SpanGenerator sp;
            if (item is ImageMapBlock b && b.Block.Procedure != null)
            {
                sp = new AsmSpanifyer(program, b.Block.Procedure.Architecture, instructions[b], pos);
            }
            else
            {
                sp = new MemSpanifyer(program, item, pos);
            }
            return sp;
        }

        private abstract class SpanGenerator
        {
            public abstract Tuple<ModelPosition, LineSpan> GenerateSpan();

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

        private class AsmSpanifyer : SpanGenerator
        {
            private Program program;
            private IProcessorArchitecture arch;
            private MachineInstruction[] instrs;
            private int offset;
            private ModelPosition position;

            public AsmSpanifyer(
                Program program,
                IProcessorArchitecture arch,
                MachineInstruction[] instrs,
                ModelPosition pos)
            {
                this.instrs = instrs;
                this.arch = arch;
                var addr = pos.Address;
                this.offset = FindIndexOfInstructionAddress(instrs, addr);
                this.position = pos;
                this.program = program;
            }

            public override Tuple<ModelPosition, LineSpan> GenerateSpan()
            {
                if (offset >= instrs.Length || offset < 0)
                    return null;
                var instr = instrs[offset];
                ++offset;
                var asmLine = DisassemblyTextModel.RenderAsmLine(
                    position,
                    program,
                    arch,
                    instr,
                    MachineInstructionWriterOptions.ResolvePcRelativeAddress);
                if (offset == instrs.Length)
                {
                    DecorateLastLine(asmLine);
                }
                this.position = Pos(instr.Address + instr.Length);
                return Tuple.Create(position, asmLine);
            }
        }

        private class MemSpanifyer : SpanGenerator
        {
            private Program program;
            private ModelPosition position;
            public ImageMapItem item;

            public MemSpanifyer(
                Program program,
                ImageMapItem item,
                ModelPosition pos)
            {
                this.program = program;
                this.item = item;
                this.position = pos;
            }

            public override Tuple<ModelPosition, LineSpan> GenerateSpan()
            {
                var addr = this.position.Address;
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

                var rdr = program.CreateImageReader(program.Architecture, addr);
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

                var memLine = new LineSpan(position, line.ToArray());
                this.position = Pos(addrEnd);
                if (rdr.Address >= item.EndAddress)
                {
                    DecorateLastLine(memLine);
                }
                return Tuple.Create(position, memLine);
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

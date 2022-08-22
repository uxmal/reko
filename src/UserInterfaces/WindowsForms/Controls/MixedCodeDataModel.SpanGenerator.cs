#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
                    curPos.Address,
                    new MemoryTextSpan(
                        $"; {commentLines[curPos.Offset]}",
                        UiStyles.CodeComment));
                curPos = Pos(curPos.Address, curPos.Offset + 1);
                return true;
            }
            line = default;
            return false;
        }

        public LineSpan[] GetLineSpans(int count)
        {
            curPos = SanitizePosition(curPos);

            var spans = new List<LineSpan>();
            program.SegmentMap.TryFindSegment(curPos.Address, out var seg);
            program.ImageMap.TryFindItem(curPos.Address, out var item);

            SpanGenerator sp = CreateSpanifier(item, seg.MemoryArea, curPos);
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
                        sp = CreateSpanifier(item, seg.MemoryArea, curPos);
                }
                memValid &= seg.MemoryArea.IsValidAddress(curPos.Address);

                if (memValid)
                {
                    var tuple = sp.GenerateSpan();
                    if (tuple != null)
                    {
                        curPos = tuple.Value.Item1;
                        spans.Add(tuple.Value.Item2);
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
                    sp = CreateSpanifier(item, seg.MemoryArea, curPos);
                }
            }
            curPos = SanitizePosition(curPos);
            var aSpans = spans.ToArray();
            return aSpans;
        }

        private SpanGenerator CreateSpanifier(
            ImageMapItem item,
            MemoryArea mem,
            ModelPosition pos)
        {
            SpanGenerator sp;
            if (item is ImageMapBlock b && b.Block.Procedure != null)
            {
                sp = new AsmSpanifyer(
                    program, 
                    b.Block.Procedure.Architecture,
                    instructions[b], 
                    pos,
                    this.selSvc?.SelectedAddress);
            }
            else
            {
                sp = new MemSpanifyer(program, mem, item, pos);
            }
            return sp;
        }

        private abstract class SpanGenerator
        {
            public abstract (ModelPosition, LineSpan)? GenerateSpan();

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
            private readonly Program program;
            private readonly IProcessorArchitecture arch;
            private readonly MachineInstruction[] instrs;
            private int offset;
            private ModelPosition position;
            private readonly Address addrSelected;

            public AsmSpanifyer(
                Program program,
                IProcessorArchitecture arch,
                MachineInstruction[] instrs,
                ModelPosition pos,
                Address addrSelected)
            {
                this.instrs = instrs;
                this.arch = arch;
                var addr = pos.Address;
                this.offset = FindIndexOfInstructionAddress(instrs, addr);
                this.position = pos;
                this.program = program;
                this.addrSelected = addrSelected;
            }

            public override (ModelPosition, LineSpan)? GenerateSpan()
            {
                if (offset >= instrs.Length || offset < 0)
                    return null;
                var instr = instrs[offset];
                ++offset;
                var options = new MachineInstructionRendererOptions(
                    flags: MachineInstructionRendererFlags.ResolvePcRelativeAddress);
                var asmLine = DisassemblyTextModel.RenderAsmLine(
                    position,
                    program,
                    arch,
                    instr,
                    options);
                if (instr.Address == addrSelected)
                    asmLine.Style = "mirrored";
                if (offset == instrs.Length)
                {
                    DecorateLastLine(asmLine);
                }
                this.position = Pos(instr.Address + instr.Length);
                return (position, asmLine);
            }
        }

        private class MemSpanifyer : SpanGenerator, IMemoryFormatterOutput
        {
            private readonly Program program;
            private readonly MemoryArea mem;
            private readonly ImageMapItem item;
            private readonly List<TextSpan> line;
            private ModelPosition position;

            public MemSpanifyer(
                Program program,
                MemoryArea mem,
                ImageMapItem item,
                ModelPosition pos)
            {
                this.program = program;
                this.mem = mem;
                this.item = item;
                this.position = pos;
                this.line = new List<TextSpan>();
            }

            public override (ModelPosition, LineSpan)? GenerateSpan()
            {
                Debug.Assert(line.Count == 0);
                var addr = this.position.Address;
                var rdr = program.CreateImageReader(program.Architecture, addr, item.EndAddress);
                mem.Formatter.RenderLine(rdr, program.TextEncoding, this);
                var memLine = new LineSpan(position, addr, line.ToArray());
                line.Clear();

                this.position = Pos(rdr.Address);
                if (rdr.Address >= item.EndAddress)
                {
                    DecorateLastLine(memLine);
                }
                return (position, memLine);
            }

            public void BeginLine()
            {
            }

            public void RenderAddress(Address addr)
            {
                line.Add(new AddressSpan(addr.ToString(), addr, UiStyles.MemoryWindow));
            }

            public void RenderUnit(Address addr, string sUnit)
            {
                line.Add(new MemoryTextSpan(" ", UiStyles.MemoryWindow));
                line.Add(new MemoryTextSpan(addr, sUnit, UiStyles.MemoryWindow));
            }

            public void RenderFillerSpan(int nChunks, int nCellsPerChunk)
            {
                var nCells = (1 + nCellsPerChunk) * nChunks;
                line.Add(new MemoryTextSpan(new string(' ', nCells), UiStyles.MemoryWindow));
            }

            public void RenderUnitsAsText(int prePadding, string sBytes, int postPadding)
            {
                var sbText = new StringBuilder();
                sbText.Append(' ', prePadding + 1);
                sbText.Append(sBytes);
                sbText.Append(' ', postPadding);
                line.Add(new MemoryTextSpan(sbText.ToString(), UiStyles.MemoryWindow));
            }

            public void EndLine(Constant[] bytes)
            {
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
            private readonly string text;

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
            private readonly string text;

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

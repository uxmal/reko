#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Implemented the TextViewModel interface to support
    /// presentation of disassembled instructions as lines of text.
    /// </summary>
    /// <remarks>
    /// The big challenge here is that we don't want to go rushing away
    /// and disassemble the whole binary. It's costly, and unnecessary since
    /// users typically only navigate a page at a time. Instead we estimate the
    /// number of lines the disassembly would have had (overestimating is safe)
    /// and then render accordingly.
    /// </remarks>
    public class DisassemblyTextModel : TextViewModel
    {
        private readonly Program program;
        private readonly IProcessorArchitecture arch;
        private readonly MemoryArea mem;
        private readonly Address addrStart;
        private readonly long offsetStart;
        private readonly long offsetEnd;
        private long offset;

        public DisassemblyTextModel(Program program, IProcessorArchitecture arch, ImageSegment segment)
        {
            this.program = program ?? throw new ArgumentNullException(nameof(program));
            if (segment is null)
                throw new ArgumentNullException(nameof(segment));
            if (segment.MemoryArea == null)
                throw new ArgumentException(nameof(segment), "ImageSegment must have a valid memory area.");
            this.arch = arch;
            this.mem = segment.MemoryArea;

            this.addrStart = Address.Max(segment.Address, mem.BaseAddress);
            this.offsetStart = 0;
            this.offset = offsetStart;
            this.offsetEnd = Math.Max(
                0,
                Math.Min(
                    segment.Address - addrStart + segment.Size,
                    mem.BaseAddress - addrStart + mem.Length));
        }

        public object StartPosition => offsetStart;
        public object CurrentPosition => offset;
        public object EndPosition => offsetEnd;
        public int LineCount => GetPositionEstimate(offsetEnd - offsetStart);
        public bool ShowPcRelative { get; set; }

        public int ComparePositions(object a, object b)
        {
            return ((long)a).CompareTo((long)b);
        }

        public LineSpan[] GetLineSpans(int count)
        {
            var lines = new List<LineSpan>();
            try
            {
                var arch = CurrentArchitecture();
                if (arch is not null)
                {
                    var addr = Align(arch, addrStart + offset, arch.CodeMemoryGranularity);
                    if (program.SegmentMap.TryFindSegment(addr, out ImageSegment seg) &&
                        seg.MemoryArea != null &&
                        seg.MemoryArea.IsValidAddress(addr))
                    {
                        var options = new MachineInstructionRendererOptions(
                            flags: ShowPcRelative
                                ? MachineInstructionRendererFlags.None
                                : MachineInstructionRendererFlags.ResolvePcRelativeAddress);
                        arch = GetArchitectureForAddress(addr);
                        var cellBitSize = seg.MemoryArea.CellBitSize;
                        var rdr = arch.CreateImageReader(seg.MemoryArea, addr);
                        var dasm = arch.CreateDisassembler(rdr).GetEnumerator();
                        while (count != 0 && dasm.MoveNext())
                        {
                            var instr = dasm.Current;
                            var asmLine = RenderAsmLine(
                                    instr.Address - addrStart, program, arch, instr, options);
                            lines.Add(asmLine);
                            --count;
                            offset += instr.Length;
                        }
                    }
                }
            } 
            catch (Exception e)
            {
                Debug.Print("Exception when rendering lines. {0}", e.Message);
                return Array.Empty<LineSpan>();
            }
            return lines.ToArray();
        }

        private IProcessorArchitecture GetArchitectureForAddress(Address addr)
        {
            if (this.arch is not null)
                return this.arch;
            IProcessorArchitecture arch = null;
            // Try to find a basic block at this address and use its architecture.
            if (program.ImageMap.TryFindItem(addr, out var item) &&
                item is ImageMapBlock imb &&
                imb.Block != null &&
                imb.Block.Procedure != null)
            {
                arch = imb.Block.Procedure.Architecture;
            }
            return arch ?? program.Architecture;
        }

        public static LineSpan RenderAsmLine(
            object position,
            Program program,
            IProcessorArchitecture arch,
            MachineInstruction instr,
            MachineInstructionRendererOptions options)
        {
            var line = new List<TextSpan>();
            var addr = instr.Address;
            line.Add(new AddressSpan(addr.ToString() + " ", addr, "link"));
            var rdr = program.CreateImageReader(arch, instr.Address);
            var bytes = arch.RenderInstructionOpcode(instr, rdr);
            line.Add(new InstructionTextSpan(instr, bytes, "dasm-bytes"));
            var dfmt = new DisassemblyFormatter(program, arch, instr, line);
            instr.Render(dfmt, options);
            dfmt.NewLine();
            return new LineSpan(position, addr, line.ToArray());
        }

        private Address Align(IProcessorArchitecture arch, Address addr, int bitGranularity)
        {
            uint addrAlign = (uint)Math.Max(arch.InstructionBitSize / bitGranularity, 1);
            ulong linear = addr.ToLinear();
            var rem = linear % addrAlign;
            return addr - (int)rem;
        }

        public int MoveToAddress(Address addr)
        {
            var offset = addr - addrStart;
            return MoveToLine(offset, 0);
        }

        public int MoveToLine(object basePosition, int posOffset)
        {
            var offsetInitial = (long) basePosition;
            var offset = offsetInitial + posOffset;
            if (offset < offsetStart)
                offset = offsetStart;

            if (offsetEnd != 0 && offset >= offsetEnd)
                offset = offsetEnd-1;
            this.offset = offset;
            return (int)(offset - offsetInitial);
        }

        public (int, int) GetPositionAsFraction()
        {
            return ((int)(offset - offsetStart), (int)mem.Length);
        }

        public void SetPositionAsFraction(int numerator, int denominator)
        {
            if (denominator <= 0)
                throw new ArgumentOutOfRangeException(nameof(denominator));
            if (numerator < 0 || numerator > denominator)
                throw new ArgumentOutOfRangeException(nameof(numerator));
            long offset = offsetStart + Math.BigMul(numerator, (int)mem.Length) / denominator;
            offset = Math.Max(offsetStart, offset);
            offset = Math.Min(offsetEnd - 1, offset);
            offset = Math.Max(0, offset);
            this.offset = offset;
        }

        /// <summary>
        /// Guesses at a scrollbar position by dividing the byte offset by the 
        /// instruction size. This will possibly overestimate the position.
        /// </summary>
        /// <param name="byteOffset"></param>
        /// <returns></returns>
        private int GetPositionEstimate(long byteOffset)
        {
            if (offsetEnd == 0)
                byteOffset = Math.Abs(byteOffset);
            int bitSize;
            int unitSize;
            var arch = CurrentArchitecture();
            if (arch is not null)
            {
                bitSize = program.Architecture.InstructionBitSize;
                unitSize = program.Architecture.MemoryGranularity;
            }
            else
            {
                bitSize = 8;
                unitSize = 8;
            }
            return (int) (unitSize * byteOffset / bitSize);
        }

        private IProcessorArchitecture CurrentArchitecture()
        {
            return this.arch ?? program.Architecture;
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
        }

        public class InstructionTextSpan : TextSpan
        {
            private string text;

            public InstructionTextSpan(MachineInstruction instr, string text, string style)
            {
                this.Tag = instr;
                this.text = text;
                this.Style = style;
            }

            public override string GetText()
            {
                return text;
            }
        }

        public class AddressTextSpan : TextSpan
        {
            private readonly string txtAddress;

            public AddressTextSpan(Address address, string addrAsText)
            {
                this.Tag = address;
                this.txtAddress = addrAsText;
                this.Style = "dasm-addrText";
            }

            public override string GetText()
            {
                return txtAddress;
            }
        }

        public class ProcedureTextSpan : TextSpan
        {
            private ProcedureBase proc;

            public ProcedureTextSpan(ProcedureBase proc, Address addr)
            {
                this.proc = proc;
                this.Tag = addr;
                this.Style = "dasm-addrText";
            }

            public override string GetText()
            {
                return proc.Name;
            }
        }
    }
}

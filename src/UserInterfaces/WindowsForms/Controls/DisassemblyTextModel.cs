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
using Reko.Core.Machine;
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
    /// and disassemble the whole binary. It's costly, and unnecessesary since
    /// users typically only navigate a page at a time. Instead we estimate the
    /// number of lines the disassembly would have had (overestimating is safe)
    /// and then render accordingly.
    /// </remarks>
    public class DisassemblyTextModel : TextViewModel
    {
        private Program program;
        private MemoryArea mem;
        private Address addrStart;
        private Address addrEnd;
        private Address position;

        public DisassemblyTextModel(Program program, ImageSegment segment)
        {
            this.program = program ?? throw new ArgumentNullException("program");
            if (segment == null)
                throw new ArgumentNullException("segment");
            if (segment.MemoryArea == null)
                throw new ArgumentException("segment", "ImageSegment must have a valid memory area.");
            this.mem = segment.MemoryArea;

            this.addrStart = Address.Max(segment.Address, mem.BaseAddress);
            this.position = addrStart;
            this.addrEnd = Address.Min(segment.Address + segment.Size, mem.EndAddress);
        }

        public object StartPosition { get { return addrStart; } }
        public object CurrentPosition { get { return position; } }
        public object EndPosition { get { return addrEnd; } }
        public int LineCount { get { return GetPositionEstimate(addrEnd - addrStart); } }
        public bool ShowPcRelative { get; set; }

        public int ComparePositions(object a, object b)
        {
            return ((Address)a).CompareTo((Address)b);
        }

        public LineSpan[] GetLineSpans(int count)
        {
            var lines = new List<LineSpan>();
            if (program.Architecture != null)
            {
                var addr = Align(position);
                if (program.SegmentMap.TryFindSegment(addr, out ImageSegment seg) &&
                    seg.MemoryArea != null &&
                    seg.MemoryArea.IsValidAddress(addr))
                {
                    var options = ShowPcRelative
                        ? MachineInstructionWriterOptions.None
                        : MachineInstructionWriterOptions.ResolvePcRelativeAddress;

                    var arch = GetArchitectureForAddress(addr);
                    var dasm = program.CreateDisassembler(arch, Align(position)).GetEnumerator();
                    while (count != 0 && dasm.MoveNext())
                    {
                        var instr = dasm.Current;
                        lines.Add(
                            RenderAsmLine(
                                instr.Address, program, arch, instr, options));
                        --count;
                        position += instr.Length;
                    }
                }
            }
            return lines.ToArray();
        }

        private IProcessorArchitecture GetArchitectureForAddress(Address addr)
        {
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
            MachineInstructionWriterOptions options)
        {
            var line = new List<TextSpan>();
            var addr = instr.Address;
            line.Add(new AddressSpan(addr.ToString() + " ", addr, "link"));
            line.Add(new InstructionTextSpan(instr, BuildBytes(program, arch, instr), "dasm-bytes"));
            var dfmt = new DisassemblyFormatter(program, arch, instr, line);
            dfmt.Address = instr.Address;
            instr.Render(dfmt, options);
            dfmt.NewLine();
            return new LineSpan(position, line.ToArray());
        }

        private Address Align(Address addr)
        {
            uint byteAlign = (uint)program.Architecture.InstructionBitSize / 8u;
            ulong linear = addr.ToLinear();
            var rem = linear % byteAlign;
            return addr - (int)rem;
        }

        private static string BuildBytes(Program program, IProcessorArchitecture arch, MachineInstruction instr)
        {
            //$REVIEW: these computations will be done a lot, but we need some place to store 
            // them.
            var bitSize = arch.InstructionBitSize;
            var byteSize = (bitSize + 7) / 8;
            var instrByteFormat = $"{{0:X{byteSize * 2}}} ";      // 2 characters for each byte
            var instrByteSize = PrimitiveType.CreateWord(bitSize);

            var sb = new StringBuilder();
            var rdr = program.CreateImageReader(arch, instr.Address);
            for (int i = 0; i < instr.Length; i += byteSize)
            {
                var v = rdr.Read(instrByteSize);
                sb.AppendFormat(instrByteFormat, v.ToUInt64());
            }
            return sb.ToString();
        }

        public int MoveToLine(object basePosition, int offset)
        {
            var addrInitial = (Address)basePosition;
            var addr = addrInitial;
            if (addr < addrStart)
                addr = addrStart;
            if (addrEnd.ToLinear() != 0 && addr >= addrEnd)
                addr = addrEnd-1;
            this.position = addr;
            return (int)(addr - addrInitial);
        }

        public Tuple<int, int> GetPositionAsFraction()
        {
            return Tuple.Create((int)(position - addrStart), (int)mem.Length);
        }

        public void SetPositionAsFraction(int numerator, int denominator)
        {
            if (denominator <= 0)
                throw new ArgumentException("denominator");
            if (numerator < 0 || numerator > denominator)
                throw new ArgumentException("numerator");
            long offset = Math.BigMul(numerator, (int)mem.Length) / denominator;
            if (offset < 0)
                offset = 0;
            var addr = addrStart + offset;
            if (addrEnd.ToLinear() != 0 && addr >= addrEnd)
                addr = addrEnd-1;
            this.position = addr;
        }

        /// <summary>
        /// Guesses at a scrollbar position by dividing the byte offset by the 
        /// instruction size. This will possibly overestimate the position.
        /// </summary>
        /// <param name="byteOffset"></param>
        /// <returns></returns>
        private int GetPositionEstimate(long byteOffset)
        {
            if (addrEnd.ToLinear() == 0)
                byteOffset = Math.Abs(byteOffset);
            int bitSize = program.Architecture != null
                ? program.Architecture.InstructionBitSize
                : 8;
            return (int)(8 * byteOffset / bitSize);
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
            private string txtAddress;

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

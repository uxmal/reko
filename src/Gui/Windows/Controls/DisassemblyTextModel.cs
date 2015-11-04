#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Gui.Windows.Controls
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
        private Address position;

        public DisassemblyTextModel(Program program)
        {
            if (program == null)
                throw new ArgumentNullException("program");
            this.program = program;
            this.position = program.Image.BaseAddress;
        }

        public object StartPosition { get { return program.Image.BaseAddress; } }
        public object CurrentPosition { get { return position; } }
        public object EndPosition { get { return program.ImageMap.MapLinearAddressToAddress((ulong)((long)program.Image.BaseAddress.ToLinear() + program.Image.Bytes.LongLength)); } }
        public int LineCount { get { return GetPositionEstimate(program.Image.Bytes.Length); } }

        public int ComparePositions(object a, object b)
        {
            return ((Address)a).CompareTo((Address)b);
        }

        public LineSpan[] GetLineSpans(int count)
        {
            var lines = new List<LineSpan>();
            if (program.Architecture != null)
            {
                var dasm = program.CreateDisassembler(Align(position)).GetEnumerator();
                while (count != 0 && dasm.MoveNext())
                {
                    var line = new List<TextSpan>();
                    var instr = dasm.Current;
                    var addr = instr.Address;
                    line.Add(new AddressSpan(addr.ToString() + " ", addr, "link"));
                    line.Add(new InstructionTextSpan(instr, BuildBytes(instr), "dasm-bytes"));
                    var dfmt = new DisassemblyFormatter(program, instr, line);
                    instr.Render(dfmt);
                    dfmt.NewLine();
                    lines.Add(new LineSpan(addr, line.ToArray()));
                    --count;
                    position = addr + instr.Length;
                }

            }
            return lines.ToArray();
        }

        private Address Align(Address addr)
        {
            uint byteAlign = (uint)program.Architecture.InstructionBitSize / 8u;
            ulong linear = addr.ToLinear();
            var rem = linear % byteAlign;
            return addr - (int) rem;
        }

        private string BuildBytes(MachineInstruction instr)
        {
            var sb = new StringBuilder();
            var rdr = program.CreateImageReader(instr.Address);
            for (int i = 0; i < instr.Length; ++i)
            {
                sb.AppendFormat("{0:X2} ", rdr.ReadByte());
            }
            return sb.ToString();
        }

        public void MoveToLine(object basePosition, int offset)
        {
            var addr = (Address)basePosition;
            var image = program.Image;
            if (addr < image.BaseAddress)
                addr = image.BaseAddress;
            var addrEnd = program.ImageMap.MapLinearAddressToAddress(
                image.BaseAddress.ToLinear() + (ulong)image.Length - 1);
            if (addr > addrEnd)
                addr = addrEnd;
            this.position = addr;
        }

        public Tuple<int, int> GetPositionAsFraction()
        {
            var image = program.Image;
            return Tuple.Create((int)(position - image.BaseAddress), (int)image.Length);
        }

        public void SetPositionAsFraction(int numerator, int denominator)
        {
            if (denominator <= 0)
                throw new ArgumentException("denominator");
            if (numerator < 0 || numerator > denominator)
                throw new ArgumentException("numerator");
            var image = program.Image;
            long offset = Math.BigMul(numerator, (int)image.Length) / denominator;
            if (offset < 0)
                offset = 0;
            else if (offset > image.Bytes.Length)
                offset = image.Bytes.Length;

            this.position = program.ImageMap.MapLinearAddressToAddress(image.BaseAddress.ToLinear() + (uint) offset);
        }

        /// <summary>
        /// Guesses at a scrollbar position by dividing the byte offset by the instruction size.
        /// </summary>
        /// <param name="byteOffset"></param>
        /// <returns></returns>
        private int GetPositionEstimate(int byteOffset)
        {
            int bitSize = program.Architecture != null
                ? program.Architecture.InstructionBitSize
                : 8;
            return 8 * byteOffset / bitSize;
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
                base.Style = style ;
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

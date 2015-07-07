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

using Decompiler.Core;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Windows.Controls
{
    /// <summary>
    /// Implemented the TextViewModel interface to support
    /// presentation of disassembled instructions as lines of text.
    /// </summary>
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

        public TextSpan[][] GetLineSpans(int count)
        {
            var lines = new List<TextSpan[]>();
            if (program.Architecture != null)
            {
                var dasm = program.CreateDisassembler(position).GetEnumerator();
                while (count != 0 && dasm.MoveNext())
                {
                    var line = new List<TextSpan>();
                    var addr = dasm.Current.Address;
                    line.Add(new AddressSpan(addr.ToString() + " ", addr, "addr"));
                    line.Add(new InertTextSpan(BuildBytes(dasm.Current), "bytes"));
                    var dfmt = new DisassemblyFormatter(program, line);
                    dasm.Current.Render(dfmt);
                    dfmt.NewLine();
                    lines.Add(line.ToArray());
                    --count;
                }
            }
            return lines.ToArray();
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

        public void MoveTo(object basePosition, int offset)
        {
            var addr = (Address)basePosition;
            var image = program.Image;
            addr = addr + offset;
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

        public class AddressTextSpan : TextSpan
        {
            private string txtAddress;

            public AddressTextSpan(Address address, string addrAsText)
            {
                this.Tag = address;
                this.txtAddress = addrAsText;
                this.Style = "addrText";
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
                this.Style = "addrText";
            }

            public override string GetText()
            {
                return proc.Name;
            }
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
        private IProcessorArchitecture arch;
        private LoadedImage image;
        private Address position;

        public DisassemblyTextModel(IProcessorArchitecture arch, LoadedImage image)
        {
            this.arch = arch;
            this.image = image;
            this.position = image.BaseAddress;
        }

        public object StartPosition { get { return this.image.BaseAddress; } }
        public object CurrentPosition { get { return position; } }
        public object EndPosition { get { return this.image.BaseAddress + this.image.Bytes.Length; } }
        public int LineCount { get { return GetPositionEstimate(image.Bytes.Length); } }

        public TextSpan[][] GetLineSpans(int count)
        {
            var lines = new List<TextSpan[]>();
            var dasm = arch.CreateDisassembler(
                arch.CreateImageReader(image, position)).GetEnumerator();
            while (count != 0 && dasm.MoveNext())
            {
                var line = new List<TextSpan>();
                var addr = dasm.Current.Address;
                line.Add(new InertTextSpan(addr.ToString() + " ", "addr"));
                line.Add(new InertTextSpan(BuildBytes(dasm.Current), "bytes"));
                var dfmt = new DisassemblyFormatter(line);
                dasm.Current.Render(dfmt);
                dfmt.NewLine();
                lines.Add(line.ToArray());
                --count;
            }
            return lines.ToArray();
        }

        private string BuildBytes(MachineInstruction instr)
        {
            var sb = new StringBuilder();
            var rdr = arch.CreateImageReader(image, instr.Address);
            for (int i = 0; i < instr.Length; ++i)
            {
                sb.AppendFormat("{0:X2} ", rdr.ReadByte());
            }
            return sb.ToString();
        }

        public void MoveTo(object basePosition, int offset)
        {
            var addr = (Address)basePosition;
            addr = addr + offset;
            if (addr < image.BaseAddress)
                addr = image.BaseAddress;
            var addrEnd = image.BaseAddress + image.Bytes.Length;
            if (addr > addrEnd)
                addr = addrEnd;
            this.position = addr;
        }

        public Tuple<int, int> GetPositionAsFraction()
        {
            return Tuple.Create(position - image.BaseAddress, image.Bytes.Length);
        }

        public void SetPositionAsFraction(int numerator, int denominator)
        {
            if (denominator <= 0)
                throw new ArgumentException("denominator");
            if (numerator < 0 || numerator > denominator)
                throw new ArgumentException("numerator");
            long offset = Math.BigMul(numerator, image.Bytes.Length) / denominator;
            if (offset < 0)
                offset = 0;
            else if (offset > image.Bytes.Length)
                offset = image.Bytes.Length;
            this.position = image.BaseAddress + (int)offset;
        }

        /// <summary>
        /// Guesses at a scrollbar position by dividing the byte offset by the instruction size.
        /// </summary>
        /// <param name="byteOffset"></param>
        /// <returns></returns>
        private int GetPositionEstimate(int byteOffset)
        {
            return 8 * byteOffset / arch.InstructionBitSize;
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
            private Address addr;
            private string txtAddress;

            public AddressTextSpan(Address address, string addrAsText)
            {
                this.addr = address;
                this.txtAddress = addrAsText;
            }

            public override string GetText()
            {
                return txtAddress;
            }
        }
    }
}

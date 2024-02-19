#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Gui.TextViewing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class WindowsFormsTextSpanFactory : TextSpanFactory
    {
        public override ITextSpan CreateAddressSpan(string sAddress, Address address, string style)
        {
            return new AddressSpan(sAddress, address, style);
        }

        public override ITextSpan CreateAddressTextSpan(Address address, string formattedAddress)
        {
            return new AddressTextSpan(address, formattedAddress);
        }

        public override ITextSpan CreateEmptyTextSpan()
        {
            return new EmptyTextSpan();
        }

        public override ITextSpan CreateInstructionTextSpan(MachineInstruction instr, string bytes, string style)
        {
            return new InstructionTextSpan(instr, bytes, style);
        }

        public override ITextSpan CreateMemoryTextSpan(string text, string style)
        {
            return new MemoryTextSpan(text, style);
        }

        public override ITextSpan CreateMemoryTextSpan(Address addr, string text, string style)
        {
            return new MemoryTextSpan(addr, text, style);
        }

        public override ITextSpan CreateProcedureTextSpan(ProcedureBase proc, Address addr)
        {
            return new ProcedureTextSpan(proc, addr);
        }

        public override AbstractTextSpanFormatter CreateTextSpanFormatter()
        {
            return new TextSpanFormatter();
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

        public class AddressSpan : TextSpan
        {
            private string formattedAddress;

            public AddressSpan(string formattedAddress, Address addr, string style)
            {
                this.formattedAddress = formattedAddress;
                this.Tag = addr;
                base.Style = style;
            }

            public override string GetText()
            {
                return formattedAddress;
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


        public class EmptyTextSpan : TextSpan
        {
            public override string GetText() => "";
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
        }
    }
}

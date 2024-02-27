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

using Avalonia;
using Avalonia.Media;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Gui.TextViewing;
using Reko.UserInterfaces.AvaloniaUI.Controls;
using System;
using System.Globalization;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    public class AvaloniaTextSpanFactory : TextSpanFactory
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
            throw new NotImplementedException();
        }

        public override ITextSpan CreateInstructionTextSpan(MachineInstruction instr, string bytes, string style)
        {
            return new InstructionTextSpan(instr, bytes, style);
        }

        public override ITextSpan CreateMemoryTextSpan(string text, string style)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateMemoryTextSpan(Address addr, string text, string style)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateProcedureTextSpan(ProcedureBase proc, Address addr)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateTextSpan(string text, string? style)
        {
            return new InertTextSpan(text, style);
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

            public override Size GetSize(string text, Typeface font, double emSize)
            {
                Size sz = base.GetSize(text, font, emSize);
                return sz;
            }
        }
    }
}

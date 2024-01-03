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
using Reko.Core.Collections;
using Reko.Core.Machine;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using System.Drawing;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class MixedCodeDataModel : AbstractMixedCodeDataModel
    {
        public MixedCodeDataModel(
            Program program,
            ImageMap imageMap,
            ISelectedAddressService selSvc)
            : base(program,imageMap , selSvc)
        {
        }

        public MixedCodeDataModel(MixedCodeDataModel that)
            : base(that)
        {
        }

        public override AbstractMixedCodeDataModel Clone()
        {
            return new MixedCodeDataModel(this);
        }

        protected override LineSpan RenderAssemblerLine(object position, Program program, IProcessorArchitecture arch, MachineInstruction instr, MachineInstructionRendererOptions options)
        {
            return DisassemblyTextModel.RenderAsmLine(position, program, arch, instr, options);
        }

        protected override ITextSpan CreateAddressSpan(string formattedAddress, Address addr, string style)
        {
            return new AddressSpan(formattedAddress, addr, style);
        }

        protected override ITextSpan CreateMemoryTextSpan(string text, string style)
        {
            return new MemoryTextSpan(text, style);
        }

        protected override ITextSpan CreateMemoryTextSpan(Address addr, string text, string style)
        {
            return new MemoryTextSpan(addr, text, style);
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

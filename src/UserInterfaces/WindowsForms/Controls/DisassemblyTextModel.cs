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
using Reko.Gui.TextViewing;
using System.Collections.Generic;
using System.Configuration;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class DisassemblyTextModel : AbstractDisassemblyTextModel
    {
        public DisassemblyTextModel(Program program, IProcessorArchitecture arch, ImageSegment segment)
            : base(program, arch, segment)
        {
        }

        protected override LineSpan RenderAssemblerLine(object position, Program program, IProcessorArchitecture arch, MachineInstruction instr, MachineInstructionRendererOptions options)
        {
            return RenderAsmLine(position, program, arch, instr, options);
        }

        public static LineSpan RenderAsmLine(
            object position,
            Program program,
            IProcessorArchitecture arch,
            MachineInstruction instr,
            MachineInstructionRendererOptions options)
        {
            var line = new List<ITextSpan>();
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

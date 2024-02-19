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
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Gui.TextViewing;
using System.Collections.Generic;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class DisassemblyTextModel : AbstractDisassemblyTextModel
    {
        private readonly WindowsFormsTextSpanFactory factory;

        public DisassemblyTextModel(Program program, IProcessorArchitecture arch, ImageSegment segment)
            : base(program, arch, segment)
        {
            this.factory = new WindowsFormsTextSpanFactory();
        }

        protected override LineSpan RenderAssemblerLine(object position, Program program, IProcessorArchitecture arch, MachineInstruction instr, MachineInstructionRendererOptions options)
        {
            return RenderAsmLine(position, factory, program, arch, instr, options);
        }

        public static LineSpan RenderAsmLine(
            object position,
            TextSpanFactory factory,
            Program program,
            IProcessorArchitecture arch,
            MachineInstruction instr,
            MachineInstructionRendererOptions options)
        {
            var line = new List<ITextSpan>();
            var addr = instr.Address;
            line.Add(factory.CreateAddressSpan(addr.ToString() + " ", addr, "link"));
            if (program.TryCreateImageReader(arch, instr.Address, out var rdr))
            {
                var bytes = arch.RenderInstructionOpcode(instr, rdr);
                line.Add(factory.CreateInstructionTextSpan(instr, bytes, "dasm-bytes"));
                var dfmt = new DisassemblyFormatter(factory, program, arch, instr, line);
                instr.Render(dfmt, options);
                dfmt.NewLine();
            }
            return new LineSpan(position, addr, line.ToArray());
        }
    }
}

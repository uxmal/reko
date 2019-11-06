#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core.Machine;
using System;

namespace Reko.Arch.OpenRISC
{
    public class OpenRISCInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int OpcodeAsInteger => (int) Mnemonic;

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            RenderMnemonic(writer);
            if (Operands.Length == 0)
                return;
            writer.Tab();
            RenderOperand(Operands[0], writer, options);
            if (Operands.Length == 1)
                return;
            writer.WriteString(",");
            RenderOperand(Operands[1], writer, options);
            if (Operands.Length == 2)
                return;
            writer.WriteString(",");
            RenderOperand(Operands[2], writer, options);
        }

        private void RenderMnemonic(MachineInstructionWriter writer)
        {
            var sMnemonic = this.Mnemonic.ToString()
                .Replace('_', '.');
            writer.WriteOpcode(sMnemonic);
        }

        private void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            op.Write(writer, options);
        }
    }
}
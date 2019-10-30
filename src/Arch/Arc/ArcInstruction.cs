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
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Arc
{
    public class ArcInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public MachineOperand[] Operands { get; internal set; }

        public override int OpcodeAsInteger => (int) Mnemonic;

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            RenderMnemonic(writer);
            if (Operands.Length == 0)
                return;
            writer.Tab();
            Render(Operands[0]);
            if (Operands.Length == 1)
                return;
            writer.WriteChar(',');
            Render(Operands[1]);
            if (Operands.Length == 2)
                return;
            writer.WriteChar(',');
            Render(Operands[2]);
        }

        private void RenderMnemonic(MachineInstructionWriter writer)
        {
            writer.WriteString(Mnemonic.ToString());
        }

        private void Render(MachineOperand machineOperand)
        {
            throw new NotImplementedException();
        }
    }
}

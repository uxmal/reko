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

namespace Reko.Arch.Cray
{
    public class CrayInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public MachineOperand[] Operands { get; set; }
        public override int OpcodeAsInteger => (int)Mnemonic;

        public override MachineOperand GetOperand(int i)
        {
            if (0 <= i && i < Operands.Length)
                return Operands[i];
            return null;
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (Mnemonic)
            {
            case Mnemonic._and:
                Render3("&", writer);
                return;
            case Mnemonic._fmul:
                Render3("*F", writer);
                return;
            case Mnemonic._mov:
                RenderOperand(Operands[0], writer);
                writer.Tab();
                RenderOperand(Operands[1], writer);
                if (Operands.Length == 2)
                    return;
                writer.WriteString(",");
                RenderOperand(Operands[2], writer);
                return;
            }
            writer.WriteOpcode(this.Mnemonic.ToString());
            if (Operands.Length == 0)
                return;
            writer.Tab();
            RenderOperand(Operands[0], writer);
            if (Operands.Length == 1)
                return;
            writer.WriteChar(',');
            RenderOperand(Operands[1], writer);
            if (Operands.Length == 2)
                return;
            writer.WriteChar(',');
            RenderOperand(Operands[2], writer);
        }

        private void Render3(string infix, MachineInstructionWriter writer)
        {
            RenderOperand(Operands[0], writer);
            writer.Tab();
            RenderOperand(Operands[1], writer);
            writer.WriteString(infix);
            RenderOperand(Operands[2], writer);
        }

        private void RenderOperand(MachineOperand op, MachineInstructionWriter writer)
        {
            writer.WriteString(op.ToString());
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using System;
using Reko.Core.Machine;

namespace Reko.Arch.Xtensa
{
    public class XtensaInstruction : MachineInstruction
    {
        public override InstructionClass InstructionClass
        {
            get { return InstructionClass.Linear; }
        }

        public override bool IsValid
        {
            get { return Opcode != Opcodes.invalid;  }
        }

        public Opcodes Opcode { get; set; }

        public override int OpcodeAsInteger
        {
            get { return (int)Opcode; }
        }

        public MachineOperand[] Operands { get; internal set; }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            var sep = "";
            if (this.Operands != null)
            {
                foreach (var op in this.Operands)
                {
                    writer.Write(sep);
                    op.Write(false, writer);
                    sep = ",";
                }
            }
        }
    }
}
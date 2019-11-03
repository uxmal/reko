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

using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;
using System;

namespace Reko.Arch.Mips
{
    public class MipsInstruction : MachineInstruction
    {
        public Opcode opcode;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;
        public MachineOperand op4;

        public override int OpcodeAsInteger { get { return (int)opcode; } }

        public override MachineOperand GetOperand(int i)
        {
            switch (i)
            {
            case 0: return op1;
            case 1: return op2;
            case 2: return op3;
            case 3: return op4;
            default: return null;
            }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            WriteMnemonic(writer);

            if (op1 == null)
                return;
            writer.Tab();
            op1.Write(writer, options);
            if (op2 == null)
                return;
            writer.WriteChar(',');
            op2.Write(writer, options);
            if (op3 == null)
                return;
            writer.WriteChar(',');
            op3.Write(writer, options);
            if (op4 == null)
                return;
            writer.WriteChar(',');
            op4.Write(writer, options);
        }

        private void WriteMnemonic(MachineInstructionWriter writer)
        {
            var name = this.opcode.ToString().Replace('_', '.');
            writer.WriteOpcode(name);
        }
    }
}

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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp11
{
    public class Pdp11Instruction : MachineInstruction
    {
        public Opcode Opcode;
        public PrimitiveType DataWidth;
        public MachineOperand op1;
        public MachineOperand op2;

        public override int OpcodeAsInteger => (int)Opcode;

        public override MachineOperand GetOperand(int i)
        {
            if (i == 0)
                return op1;
            else if (i == 1)
                return op2;
            else
                return null;
        }


        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 != null)
            {
                writer.Tab();
                OpToString(op1, options, writer);
                if (op2 != null)
                {
                    writer.WriteString(",");
                    OpToString(op2, options, writer);
                }
            }
        }

        private void OpToString(
            MachineOperand op,
            MachineInstructionWriterOptions options,
            MachineInstructionWriter writer)
        {
            if (op is ImmediateOperand)
            {
                writer.WriteString("#" + op.ToString());
            }
            else
            {
                op.Write(writer, options);
            }
        }
    }
}

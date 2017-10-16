#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Types;
using System.Text;

namespace Reko.Arch.MSP430
{
    public class Msp430Instruction : MachineInstruction
    {
        public Opcode opcode;
        public PrimitiveType dataWidth;
        public MachineOperand op1;
        public MachineOperand op2;

        public override InstructionClass InstructionClass
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsValid
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int OpcodeAsInteger
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var sb = new StringBuilder(opcode.ToString());
            if (dataWidth != null)
            {
                sb.AppendFormat(".{0}", dataWidth.BitSize == 8 ? "b" : "w");
            }
            writer.WriteOpcode(sb.ToString());
            if (op1 != null)
            {
                writer.Tab();
                op1.Write(writer, options);
                if (op2 != null)
                {
                    writer.Write(",");
                    op2.Write(writer, options);
                }
            }
        }
    }
}
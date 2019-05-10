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

namespace Reko.Arch.Msp430
{
    public class Msp430Instruction : MachineInstruction
    {
        public Opcode opcode;
        public PrimitiveType dataWidth;
        public MachineOperand op1;
        public MachineOperand op2;
        public int repeatImm;
        public RegisterStorage repeatReg;

        public override int OpcodeAsInteger => (int) opcode;

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (repeatReg != null)
            {
                writer.WriteOpcode("rpt");
                writer.WriteString(" ");
                writer.WriteString(repeatReg.Name);
                writer.WriteString(" ");
            } else if (repeatImm > 1)
            {
                writer.WriteOpcode("rpt");
                writer.WriteString(" #");
                writer.WriteString(repeatImm.ToString());
                writer.WriteString(" ");
            }
            var sb = new StringBuilder(opcode.ToString());
            if (dataWidth != null)
            {
                sb.AppendFormat(".{0}", dataWidth.BitSize == 8 
                    ? "b" 
                    : dataWidth.BitSize == 16
                        ? "w"
                        : "a" );
            }
            writer.WriteOpcode(sb.ToString());
            if (op1 != null)
            {
                writer.Tab();
                Write(op1, writer, options);
                if (op2 != null)
                {
                    writer.WriteString(",");
                    Write(op2, writer, options);
                }
            }
        }

        private void Write(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (op is AddressOperand && (base.InstructionClass & InstrClass.Transfer) == 0)
            {
                writer.WriteString("&");
            }
            if (op is ImmediateOperand && opcode != Opcode.call)
            {
                writer.WriteString("#");
            }
            op.Write(writer, options);
        }
    }
}
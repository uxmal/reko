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

namespace Reko.Arch.Tms7000
{
    public class Tms7000Instruction : MachineInstruction
    {
        public Opcode Opcode;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;

        public override int OpcodeAsInteger => (int) Opcode;

        public override MachineOperand GetOperand(int i)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(this.Opcode.ToString());
            if (op1 == null)
                return;
            writer.Tab();
            RenderOperand(op1, writer, options);
            if (op2 == null)
                return;
            writer.WriteChar(',');
            RenderOperand(op2, writer, options);
        }

        private void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
            case ImmediateOperand imm:
                writer.WriteString(ImmediateOperand.FormatUnsignedValue(imm.Value, ">{1}"));
                break;
            case AddressOperand addr:
                writer.WriteAddress("@" + addr.Address, addr.Address);
                break;
            default:
                op.Write(writer, options);
                break;
            }
        }
    }
}
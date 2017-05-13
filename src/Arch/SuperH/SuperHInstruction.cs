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

namespace Reko.Arch.SuperH
{
    public class SuperHInstruction : MachineInstruction
    {
        public override InstructionClass InstructionClass
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsValid
        {
            get { return Opcode == Opcode.invalid; }
        }

        public Opcode Opcode { get; set; }

        public MachineOperand op1 { get; set; }
        public MachineOperand op2 { get; set; }

        public override int OpcodeAsInteger
        {
            get { throw new NotImplementedException(); }
        }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            if (op1 == null)
                return;
            writer.Tab();
            Render(op1, writer, options);
            if (op2 == null)
                return;
            writer.Write(',');
            Render(op2, writer, options);
        }

        private void Render(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                writer.Write(regOp.Register.Name);
                return;
            }
            var immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                writer.Write('#');
                immOp.Write(writer, options);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
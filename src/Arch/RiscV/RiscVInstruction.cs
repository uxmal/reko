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

namespace Reko.Arch.RiscV
{
    public class RiscVInstruction : MachineInstruction
    {
        internal Opcode opcode;
        internal MachineOperand op1;
        internal MachineOperand op2;
        internal MachineOperand op3;

        public override InstructionClass InstructionClass { get { throw new NotImplementedException(); } }

        public override bool IsValid { get { throw new NotImplementedException(); } }

        public override int OpcodeAsInteger { get { throw new NotImplementedException(); } }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer)
        {
            writer.Write(opcode.ToString());
            if (op1 == null)
                return;
            writer.Tab();
            WriteOp(op1, writer);
            if (op2 == null)
                return;
            writer.Write(',');
            WriteOp(op2, writer);
            if (op3 == null)
                return;
            writer.Write(',');
            WriteOp(op3, writer);
        }

        private void WriteOp(MachineOperand op, MachineInstructionWriter writer)
        {
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                writer.Write(rop.Register.Name);
                return;
            }
            var immop = op as ImmediateOperand;
            if (immop != null)
            {
                writer.Write(immop.Value.ToString());
                return;
            }
            throw new NotImplementedException();
        }
    }
}
#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.Arm
{
    public class Arm32InstructionNew : MachineInstruction
    {
        public Opcode opcode { get; set; }
        public ArmCondition condition { get; set; }
        public MachineOperand op1 { get; set; }
        public MachineOperand op2 { get; set; }
        public MachineOperand op3 { get; set; }
        public MachineOperand op4 { get; set; }

        public override InstructionClass InstructionClass
        {
            get
            {
                if (!iclasses.TryGetValue(opcode, out var iclass))
                    iclass = InstructionClass.Linear;
                if (condition != ArmCondition.AL)
                    iclass |= InstructionClass.Conditional;
                return iclass;
            }
        }

        public override bool IsValid
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override int OpcodeAsInteger
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override MachineOperand GetOperand(int i)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(opcode.ToString());
            if (op1 == null) return;
            writer.Tab();
            RenderOperand(op1, writer);

            if (op2 == null) return;
            writer.WriteChar(',');
            RenderOperand(op2, writer);

            if (op3 == null) return;
            writer.WriteChar(',');
            RenderOperand(op3, writer);

            if (op4 == null) return;
            writer.WriteChar(',');
            RenderOperand(op4, writer);
        }

        private void RenderOperand(MachineOperand op, MachineInstructionWriter writer)
        {
            switch (op)
            {
            case RegisterOperand rop:
                writer.WriteString(rop.Register.Name);
                break;
            case ImmediateOperand imm:
                writer.WriteFormat($"#{imm.Value.ToInt32():X}");
                break;
            case MemoryOperand mem:
                writer.WriteChar('[');
                writer.WriteString(mem.BaseRegister.Name);
                if (mem.Offset != null && !(mem.Offset.IsIntegerZero))
                {
                    writer.WriteChar(',');
                    writer.WriteChar('#');
                    writer.WriteUInt32(mem.Offset.ToUInt32());
                }
                writer.WriteChar(']');
                break;
            default:
                throw new NotImplementedException(op.GetType().Name);
            }
        }

        private static Dictionary<Opcode, InstructionClass> iclasses = new Dictionary<Opcode, InstructionClass>
        {
            { Opcode.hlt, InstructionClass.System },
        };
    }
}
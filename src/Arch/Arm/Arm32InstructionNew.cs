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
        #region Specially rendered opcodes
        private static readonly Dictionary<Opcode, string> opcodes = new Dictionary<Opcode, string>
        {
            { Opcode.pop_w, "pop.w" },
            { Opcode.push_w, "push.w" }
        };
        #endregion

        public Arm32InstructionNew()
        {
            this.condition = ArmCondition.AL;
        }

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
            if (!opcodes.TryGetValue(opcode, out var sOpcode))
            {
                sOpcode = opcode.ToString();
            }
            var sUpdate = UpdateFlags ? "s" : "";
            var sCond = condition == ArmCondition.AL ? "" : condition.ToString().ToLowerInvariant();
            writer.WriteOpcode($"{sOpcode}{sUpdate}{sCond}");
            if (op1 != null)
            {
                writer.Tab();
                RenderOperand(op1, writer, options);

                if (op2 != null)
                {
                    writer.WriteChar(',');
                    RenderOperand(op2, writer, options);

                    if (op3 != null)
                    {
                        writer.WriteChar(',');
                        RenderOperand(op3, writer, options);

                        if (op4 != null)
                        {
                            writer.WriteChar(',');
                            RenderOperand(op4, writer, options);
                        }
                    }
                }
            }
            if (ShiftType != Opcode.Invalid)
            {
                writer.WriteChar(',');
                writer.WriteOpcode(ShiftType.ToString());
                writer.WriteChar(' ');
                RenderOperand(ShiftValue, writer, options);
            }
        }

        private void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
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
            case AddressOperand aop:
                writer.WriteAddress($"${aop.Address}", aop.Address);
                break;
            default:
                op.Write(writer, options);
                break;
            }
        }

        private static Dictionary<Opcode, InstructionClass> iclasses = new Dictionary<Opcode, InstructionClass>
        {
            { Opcode.hlt, InstructionClass.System },
        };

        public bool Writeback;
        public bool UpdateFlags;
        public Opcode ShiftType;
        public ImmediateOperand ShiftValue;
        public ArmVectorData vector_data;
        public int vector_size;
        public byte itmask;
    }
}
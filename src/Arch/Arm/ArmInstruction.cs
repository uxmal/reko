#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.IO;
using System.Text;

namespace Reko.Arch.Arm
{
    using Gee.External.Capstone.Arm;
    using CapstoneArmInstruction = Gee.External.Capstone.Instruction<Gee.External.Capstone.Arm.ArmInstruction, Gee.External.Capstone.Arm.ArmRegister, Gee.External.Capstone.Arm.ArmInstructionGroup, Gee.External.Capstone.Arm.ArmInstructionDetail>;
    using Opcode = Gee.External.Capstone.Arm.ArmInstruction;

    public class ArmInstruction : MachineInstruction 
    {
        private CapstoneArmInstruction instruction;

        public ArmInstruction(CapstoneArmInstruction instruction)
        {
            this.instruction = instruction;
            this.Address = Address.Ptr32((uint)instruction.Address);
        }

        public CapstoneArmInstruction Internal { get { return instruction; } }

        public override int OpcodeAsInteger {
            get { return (int) instruction.Id; }
        }

        public override void Render(MachineInstructionWriter writer)
        {
            writer.WriteOpcode(Internal.Mnemonic);
            var ops = Internal.ArchitectureDetail.Operands;
            if (ops.Length < 1)
                return;
            writer.Tab();
            Write(ops[0], writer);
            if (ops.Length < 2)
                return;
            writer.Write(",");
            Write(ops[1], writer);
            if (ops.Length < 3)
                return;
            writer.Write(",");
            Write(ops[2], writer);
            if (ops.Length < 4)
                return;
            writer.Write(",");
            Write(ops[3], writer);
        }

        public void Write(ArmInstructionOperand op, MachineInstructionWriter writer)
        {
            switch (op.Type)
            {
            case ArmInstructionOperandType.Immediate:
                if (Internal.Id == Opcode.B ||
                    Internal.Id == Opcode.BL ||
                    Internal.Id == Opcode.BLX)
                {
                    writer.Write("$");
                    writer.WriteAddress(
                        string.Format("{0:X8}", op.ImmediateValue.Value),
                        Address.Ptr32((uint)op.ImmediateValue.Value));
                    break;
                }
                writer.Write("#");
                WriteImmediateValue(op.ImmediateValue.Value, writer);
                break;
            case ArmInstructionOperandType.Register:
                if (op.IsSubtracted)
                    writer.Write('-');
                writer.Write(A32Registers.RegisterByCapstoneID[op.RegisterValue.Value].Name);
                WriteShift(op, writer);
                break;
            case ArmInstructionOperandType.Memory:
                WriteMemoryOperand(op, writer);
                break;
            case ArmInstructionOperandType.SetEnd:
                writer.Write(op.SetEndValue.ToString().ToLowerInvariant());
                break;
            default:
                throw new NotImplementedException(op.Type.ToString());
            }
        }

        private void WriteShift(ArmInstructionOperand op, MachineInstructionWriter writer)
        {
            switch (op.Shifter.Type)
            {
            case ArmShifterType.ASR: WriteImmShift("asr", op.Shifter.Value, writer); break;
            case ArmShifterType.LSL: WriteImmShift("lsl", op.Shifter.Value, writer); break;
            case ArmShifterType.LSR: WriteImmShift("lsr", op.Shifter.Value, writer); break;
            case ArmShifterType.ROR: WriteImmShift("ror", op.Shifter.Value, writer); break;
            case ArmShifterType.RRX: writer.Write(",rrx"); break;
            case ArmShifterType.ASR_REG: WriteRegShift("asr", op.Shifter.Value, writer); break;
            case ArmShifterType.LSL_REG: WriteRegShift("lsl", op.Shifter.Value, writer); break;
            case ArmShifterType.LSR_REG: WriteRegShift("lsr", op.Shifter.Value, writer); break;
            case ArmShifterType.ROR_REG: WriteRegShift("ror", op.Shifter.Value, writer); break;
            case ArmShifterType.RRX_REG: WriteRegShift("rrx", op.Shifter.Value, writer); break;
            case ArmShifterType.Invalid: break;
            }
        }

        private void WriteMemoryOperand(ArmInstructionOperand op, MachineInstructionWriter writer)
        {
            writer.Write('[');
            writer.Write(A32Registers.RegisterByCapstoneID[op.MemoryValue.BaseRegister].Name);
            int displacement = op.MemoryValue.Displacement;
            if (displacement != 0)
            {
                if (true) // preincInternal.ArchitectureDetail)
                {
                    writer.Write(",");
                    if (displacement < 0)
                    {
                        displacement = -displacement;
                        writer.Write("-");
                    }
                    writer.Write("#");
                    WriteImmediateValue(displacement, writer);
                    writer.Write("]");
                    if (Internal.ArchitectureDetail.WriteBack)
                        writer.Write("!");
                }
                else
                {
                    writer.Write("],");
                    if (displacement < 0)
                    {
                        displacement = -displacement;
                        writer.Write("-");
                    }
                    WriteImmediateValue(displacement, writer);
                }
            }
            else
            {
                if (op.MemoryValue.IndexRegister != ArmRegister.Invalid)
                {
                    writer.Write(",");
                    // NOTE: capstone.NET seems to reverse the sense of this scale parameter.
                    if (op.MemoryValue.IndexRegisterScale > 0)
                        writer.Write("-");
                    writer.Write(A32Registers.RegisterByCapstoneID[op.MemoryValue.IndexRegister].Name);
                }
                if (op.Shifter.Type != ArmShifterType.Invalid)
                {
                    WriteShift(op, writer);
                }
                writer.Write(']');
                if (Internal.ArchitectureDetail.WriteBack && IsLastOperand(op))
                    writer.Write("!");
            
            }
        }

        /// <summary>
        /// Returns true if <paramref name="op"/> is the last operand of the instruction.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool IsLastOperand(ArmInstructionOperand op)
        {
            var ops = Internal.ArchitectureDetail.Operands;
            return op == ops[ops.Length-1];
        }

        private void WriteImmShift(string op, int value, MachineInstructionWriter writer)
        {
            writer.Write(",");
            writer.WriteOpcode(op);
            writer.Write(" #");
            WriteImmediateValue(value, writer);
        }

        private void WriteRegShift(string op, int value, MachineInstructionWriter writer)
        {
            writer.Write(",");
            writer.WriteOpcode(op);
            writer.Write(' ');
            writer.Write(A32Registers.RegisterByCapstoneID[(ArmRegister)value].Name);
        }

        private static void WriteImmediateValue(int imm8, MachineInstructionWriter writer)
        {
            if (imm8 > 256 && ((imm8 & (imm8 - 1)) == 0))
            {
                /* only one bit set, and that later than bit 8.
                 * Represent as 1<<... .
                 */
                writer.Write("1<<");
                {
                    uint n = 0;
                    while ((imm8 & 15) == 0)
                    {
                        n += 4; imm8 = imm8 >> 4;
                    }
                    // Now imm8 is 1, 2, 4 or 8. 
                    n += (uint)((0x30002010 >> (int)(4 * (imm8 - 1))) & 15);
                    writer.Write(n);
                }
            }
            else
            {
                var fmt = (-9 <= imm8 && imm8 <= 9) ? "{0}{1}" : "&{0}{1:X}";
                var sign = "";
                if (((int)imm8) < 0 && ((int)imm8) > -100)
                {
                    imm8 = -imm8;
                    sign = "-";
                }
                writer.Write(fmt, sign, imm8);
            }
        }
    }
}

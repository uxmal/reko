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

using Gee.External.Capstone;
using Gee.External.Capstone.Arm64;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    using Core.NativeInterface;
    using System.Runtime.InteropServices;
    using CapstoneArmInstruction = Instruction<Arm64Instruction, Arm64Register, Arm64InstructionGroup, Arm64InstructionDetail>;

    [Obsolete]
    public class AArch64Instruction : MachineInstruction
    {
        private static Dictionary<Arm64Instruction, InstructionClass> classOf;

        private Instruction<Arm64Instruction, Arm64Register, Arm64InstructionGroup, Arm64InstructionDetail> instruction;

        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;

        public AArch64Instruction(Instruction<Arm64Instruction, Arm64Register, Arm64InstructionGroup, Arm64InstructionDetail> instruction)
        {
            this.instruction = instruction;
            this.Address = Address.Ptr64((ulong)instruction.Address);
        }

        internal CapstoneArmInstruction Internal { get { return instruction; } }

        public override InstructionClass InstructionClass
        {
            get
            {
                InstructionClass ct;
                if (!classOf.TryGetValue(instruction.Id, out ct))
                {
                    ct = InstructionClass.Linear;
                }
                return ct;
            }
        }

        public override bool IsValid { get { return InstructionClass != InstructionClass.Invalid; } }

        public override int OpcodeAsInteger { get { return 0; } }

        public override MachineOperand GetOperand(int i)
        {
            return null;
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(instruction.Mnemonic);
            var ops = Internal.ArchitectureDetail.Operands;
            if (ops.Length < 1)
                return;
            writer.Tab();
            Write(ops[0], writer);
            if (ops.Length < 2)
                return;
            writer.WriteString(",");
            Write(ops[1], writer);
            if (ops.Length < 3)
                return;
            writer.WriteString(",");
            Write(ops[2], writer);
            if (ops.Length < 4)
                return;
            writer.WriteString(",");
            Write(ops[3], writer);
        }

        private void Write(Arm64InstructionOperand op, MachineInstructionWriter writer)
        {
            switch (op.Type)
            {
            case Arm64InstructionOperandType.Register:
                writer.WriteString(A64Registers.RegisterByCapstoneID[op.RegisterValue.Value].Name);
                return;
            case Arm64InstructionOperandType.Immediate:
                if (IsJump())
                {
                    writer.WriteFormat("${0:X16}", op.ImmediateValue.Value);
                }
                else
                {
                    WriteImmediateValue(op.ImmediateValue.Value, writer);
                    if (op.Shifter != null)
                    {
                        this.WriteShift(op, writer);
                    }
                }
                return;
            }
            throw new NotImplementedException(op.Type.ToString());
        }

        private bool IsJump()
        {
            return false;
        }

        private void WriteShift(Arm64InstructionOperand op, MachineInstructionWriter writer)
        {
            switch (op.Shifter.Type)
            {
            case Arm64ShifterType.ASR: WriteImmShift("asr", op.Shifter.Value, writer); break;
            case Arm64ShifterType.LSL: WriteImmShift("lsl", op.Shifter.Value, writer); break;
            case Arm64ShifterType.LSR: WriteImmShift("lsr", op.Shifter.Value, writer); break;
            case Arm64ShifterType.MSL: WriteImmShift("msl", op.Shifter.Value, writer); break;
            case Arm64ShifterType.ROR: WriteImmShift("ror", op.Shifter.Value, writer); break;
            case Arm64ShifterType.Invalid: break;
            }
        }

        private static void WriteImmediateValue(long imm, MachineInstructionWriter writer)
        {
            writer.WriteString("#");
            if (imm > 256 && ((imm & (imm - 1)) == 0))
            {
                /* only one bit set, and that later than bit 8.
                 * Represent as 1<<... .
                 */
                writer.WriteString("1<<");
                uint n = 0;
                while ((imm & 0xF) == 0)
                {
                    n += 4; imm = imm >> 4;
                }
                // Now imm8 is 1, 2, 4 or 8. 
                n += (uint)((0x30002010 >> (int)(4 * (imm - 1))) & 15);
                writer.WriteUInt32(n);
            }
            else
            {
                if (imm < 0 && imm > -100)
                {
                    writer.WriteChar('-'); imm = -imm;
                }
                writer.WriteFormat("&{0:X}", imm);
            }
        }

        private void WriteImmShift(string op, int value, MachineInstructionWriter writer)
        {
            writer.WriteString(",");
            writer.WriteOpcode(op);
            WriteImmediateValue(value, writer);
        }
     
    }

    public class Arm64Instruction : MachineInstruction
    {
        private INativeInstruction nInstr;
        private NativeInstructionInfo info;

        public Arm64Instruction(INativeInstruction nInstr)
        {
            this.nInstr = nInstr;
            nInstr.GetInfo(out info);
            this.Address = Address.Ptr32((uint)info.LinearAddress);
            this.Length = (int)info.Length;
        }

        //$REVIEW: is this really needed? nInstr is a ComInstance object,
        // provided by the CLR, and probably has its own finalizer.
        ~Arm64Instruction()
        {
            Marshal.ReleaseComObject(nInstr);
            nInstr = null;
        }

        public override InstructionClass InstructionClass
        {
            get { return (InstructionClass)info.InstructionClass; }
        }

        public override bool IsValid
        {
            get { return this.InstructionClass != InstructionClass.Invalid; }
        }

        public override int OpcodeAsInteger
        {
            get { return info.Opcode; }
        }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotSupportedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            nInstr.Render(writer, options);
        }
    }

}


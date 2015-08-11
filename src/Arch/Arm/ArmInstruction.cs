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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Arch.Arm
{
    using CapstoneArmInstruction = Gee.External.Capstone.Instruction<Gee.External.Capstone.Arm.ArmInstruction, Gee.External.Capstone.Arm.ArmRegister, Gee.External.Capstone.Arm.ArmInstructionGroup, Gee.External.Capstone.Arm.ArmInstructionDetail>;

    public class ArmInstruction : MachineInstruction 
    {
        private CapstoneArmInstruction instruction;

        public ArmInstruction(CapstoneArmInstruction instruction)
        {
            this.instruction = instruction;
        }

        public override int OpcodeAsInteger {
            get { return (int) instruction.Id; }
        }

        public override void Render(MachineInstructionWriter writer) {
            base.Render(writer);
        }

        public CapstoneArmInstruction Internal { get { return instruction; } }
    }

    public class ArmInstruction2 : MachineInstruction
    {
        public Opcode2 Opcode;
        public OpFlags OpFlags;
        public Condition Cond;
        public bool Update;
        public MachineOperand Dst;
        public MachineOperand Src1;
        public MachineOperand Src2;
        public MachineOperand Src3;

        public override int OpcodeAsInteger { get { return (int)Opcode; } }

        public override void Render(MachineInstructionWriter writer)
        {
            writer.WriteOpcode(string.Format("{0}{1}{2}",
                Opcode,
                Cond != Condition.al ? Cond.ToString() : "",
                OpFlags != OpFlags.None ? OpFlags.ToString().ToLower() : ""));
            if (Dst != null)
            {
                writer.Tab();
                Write(Dst, writer);
                if (Update) writer.Write("!");
                if (Src1 != null)
                {
                    writer.Write(",");
                    Write(Src1, writer);
                    if (Src2 != null)
                    {
                        writer.Write(",");
                        Write(Src2, writer);
                        if (Src3 != null)
                        {
                            writer.Write(",");
                            Write(Src3, writer);
                        }
                    }
                }
            }
        }

        public void Write(MachineOperand op, MachineInstructionWriter writer)
        {
            var imm = op as ImmediateOperand;
            if (imm != null)
            {
                writer.Write("#");
                int imm8 = imm.Value.ToInt32();
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
                    writer.Write(fmt,sign,imm8);
                }
                return;
            }
            var adr = op as AddressOperand;
            if (adr != null)
            {
                adr.Write(false, writer);
                return;
            }
            var mem = op as ArmMemoryOperand;
            if (mem != null)
            {
                mem.Write(false, writer);
                return;
            }
            var sh = op as ShiftOperand;
            if (sh != null)
            {
                sh.Write(false, writer);
                return;
            }
            if (op == null)
                writer.Write("<null>");
            else 
                op.Write(false, writer);
        }
    }
}

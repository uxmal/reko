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
using System.Text;

namespace Reko.Arch.Arm.AArch32
{
    public class AArch32Instruction : MachineInstruction
    {
        #region Special cases

        // Specially rendered opcodes
        private static readonly Dictionary<Opcode, string> opcodes = new Dictionary<Opcode, string>
        {
            { Opcode.pop_w, "pop.w" },
            { Opcode.push_w, "push.w" }
        };

        // Block data transfer opcodes that affect the rendering of the first operand.
        private static readonly HashSet<Opcode> blockDataXferOpcodes = new HashSet<Opcode>
        {
            Opcode.ldm, Opcode.ldmda, Opcode.ldmdb, Opcode.ldmib,
            Opcode.stm, Opcode.stmda, Opcode.stmdb, Opcode.stmib,
            Opcode.vldmia, Opcode.vldmdb, Opcode.vstmia, Opcode.vstmdb
        };
        #endregion

        public AArch32Instruction()
        {
            this.condition = ArmCondition.AL;
        }

        public Opcode opcode { get; set; }
        public ArmCondition condition { get; set; }
        public MachineOperand[] ops { get; set; }
        /*
        public MachineOperand op1 => ops[0];
        public MachineOperand op2 => ops[1];
        public MachineOperand op3 => ops[2];
        public MachineOperand op4 => ops[3];
        */

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
            if (opcode == Opcode.it)
            {
                var itOpcode = RenderIt();
                writer.WriteOpcode(itOpcode);
                writer.Tab();
                writer.WriteString(condition.ToString().ToLowerInvariant());
                return;
            }
            RenderMnemonic(writer);
            if (ops.Length > 0)
            {
                writer.Tab();
                RenderOperand(ops[0], writer, options);
                if (this.Writeback && blockDataXferOpcodes.Contains(opcode))
                {
                    writer.WriteChar('!');
                }
                for (int iOp = 1; iOp < ops.Length; ++iOp)
                {
                    writer.WriteChar(',');
                    RenderOperand(ops[iOp], writer, options);
                }
            }
            if (ShiftType != Opcode.Invalid)
            {
                writer.WriteChar(',');
                writer.WriteOpcode(ShiftType.ToString());
                if (ShiftType != Opcode.rrx)
                {
                    writer.WriteChar(' ');
                    RenderOperand(ShiftValue, writer, options);
                }
            }
        }

        private void RenderMnemonic(MachineInstructionWriter writer)
        {
            var sb = new StringBuilder();
            if (!opcodes.TryGetValue(opcode, out string sOpcode))
            {
                sOpcode = opcode.ToString();
            }
            var sUpdate = SetFlags ? "s" : "";
            var sCond = condition == ArmCondition.AL ? "" : condition.ToString().ToLowerInvariant();
            sb.Append(sOpcode);
            sb.Append(sUpdate);
            sb.Append(sCond);
            if (this.vector_data != ArmVectorData.INVALID)
            {
                sb.AppendFormat(".{0}", this.vector_data.ToString().ToLowerInvariant());
            }
            writer.WriteOpcode(sb.ToString());
        }

        private string RenderIt()
        {
            var sb = new StringBuilder();
            sb.Append("it");
            int mask = this.itmask;
            var bit = (~(int)this.condition & 1) << 3;

            while ((mask & 0xF) != 8)
            {
                sb.Append(((mask ^ bit) & 0x8) != 0 ? 't' : 'e');
                mask <<= 1;
            }
            return sb.ToString();
        }

        private void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
            case ImmediateOperand imm:
                int v = imm.Value.ToInt32();
                if (0 <= v && v <= 9)
                    writer.WriteFormat($"#{imm.Value.ToInt32()}");
                else 
                    writer.WriteFormat($"#&{imm.Value.ToUInt64():X}");
                break;
            case AddressOperand aop:
                writer.WriteAddress($"${aop.Address}", aop.Address);
                break;
            case MemoryOperand mem:
                writer.WriteChar('[');
                writer.WriteString(mem.BaseRegister.Name);
                if (this.Writeback && !mem.PreIndex)
                {
                    // Post-indexed
                    writer.WriteString("],");
                    if (mem.Offset != null && !mem.Offset.IsIntegerZero)
                    {
                        if (!mem.Add)
                            writer.WriteChar('-');
                        writer.WriteString("#&");
                        writer.WriteString(mem.Offset.ToUInt32().ToString("X"));
                    }
                    else if (mem.Index != null)
                    {
                        if (!mem.Add)
                            writer.WriteChar('-');
                        writer.WriteString(mem.Index.Name);
                    }
                }
                else
                {
                    if (mem.Offset != null && !mem.Offset.IsIntegerZero)
                    {
                        writer.WriteString(",");
                        if (!mem.Add)
                            writer.WriteChar('-');
                        writer.WriteString("#&");
                        writer.WriteString(mem.Offset.ToUInt32().ToString("X"));
                    }
                    else if (mem.Index != null)
                    {
                        writer.WriteChar(',');
                        if (!mem.Add)
                            writer.WriteChar('-');
                        writer.WriteString(mem.Index.Name);
                        if (mem.ShiftType != Opcode.Invalid)
                        {
                            writer.WriteChar(',');
                            writer.WriteString(mem.ShiftType.ToString().ToLowerInvariant());
                            if (this.ShiftType != Opcode.rrx)
                            {
                                writer.WriteFormat(" #{0}", mem.Shift);
                            }
                        }
                    }
                    writer.WriteChar(']');
                    if (this.Writeback)
                    {
                        writer.WriteChar('!');
                    }
                }
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
        public bool SetFlags;
        public Opcode ShiftType;
        public MachineOperand ShiftValue;
        public ArmVectorData vector_data;
        public int vector_size;
        public byte itmask;
    }
}
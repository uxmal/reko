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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm.AArch32
{
    public abstract class AArch32Instruction : MachineInstruction
    {
        #region Special cases

        // Specially rendered opcodes
        private static readonly Dictionary<Opcode, string> opcodes = new Dictionary<Opcode, string>
        {
        };

        // Block data transfer opcodes that affect the rendering of the first operand.
        private static readonly HashSet<Opcode> blockDataXferOpcodes = new HashSet<Opcode>
        {
            Opcode.ldm, Opcode.ldmda, Opcode.ldmdb, Opcode.ldmib,
            Opcode.stm, Opcode.stmda, Opcode.stmdb, Opcode.stmib,
            Opcode.vldmia, Opcode.vldmdb, Opcode.vstmia, Opcode.vstmdb
        };

        // Instruction aliases render instructions differently under special conditions.
        private class ArmAlias
        {
            public readonly Func<AArch32Instruction, bool> Matches;  // predicate for the alias
            public readonly string sOpcode;
            public readonly Func<AArch32Instruction, (MachineOperand[], bool)> NewOperands;   // Mutated operands

            public ArmAlias(
                Func<AArch32Instruction, bool> match,
                string sOpcode, 
                Func<AArch32Instruction, (MachineOperand[], bool)> newOperands)
            {
                this.Matches = match;
                this.sOpcode = sOpcode;
                this.NewOperands = newOperands;
            }

            public static (MachineOperand[], bool) Shift(AArch32Instruction i)
            {
                return (i.ops.Skip(1).ToArray(), false);
            }

            public static (MachineOperand[], bool) FirstOp(AArch32Instruction i)
            {
                return (i.ops.Take(1).ToArray(), false);
            }
        }

        private static readonly Dictionary<Opcode, ArmAlias> aliases = new Dictionary<Opcode, ArmAlias>
        {
            { Opcode.ldm, new ArmAlias(i => i.Writeback && i.IsStackPointer(0), "pop", ArmAlias.Shift) },
            { Opcode.ldr, new ArmAlias(i => i.IsSinglePop(), "pop", ArmAlias.FirstOp) },
            { Opcode.stmdb, new ArmAlias(i => i.Writeback && i.IsStackPointer(0), "push", ArmAlias.Shift) },
            { Opcode.str, new ArmAlias(i => i.IsSinglePush(), "push", ArmAlias.FirstOp) },
        };
        #endregion

        public AArch32Instruction()
        {
            this.condition = ArmCondition.AL;
        }

        public Opcode opcode { get; set; }
        public ArmCondition condition { get; set; }
        public MachineOperand[] ops { get; set; }

        public override int OpcodeAsInteger => (int) opcode;

        /// <summary>
        /// PC-relative addressing has an extra offset.This varies
        /// between the T32 and the A32 instruction sets.
        /// </summary>
        public abstract Address ComputePcRelativeAddress(MemoryOperand mem);


        public override MachineOperand GetOperand(int i)
        {
            if (0 <= i && i < ops.Length)
                return ops[i];
            else
                return null;
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
            var (ops, writeback) = RenderMnemonic(writer);
            if (ops.Length > 0)
            {
                writer.Tab();
                RenderOperand(ops[0], writer, options);
                if (writeback &&
                    blockDataXferOpcodes.Contains(opcode) &&
                    ops[0] is RegisterOperand)
                {
                    writer.WriteChar('!');
                }
                for (int iOp = 1; iOp < ops.Length; ++iOp)
                {
                    writer.WriteChar(',');
                    RenderOperand(ops[iOp], writer, options);
                }
            }
            if (vector_index.HasValue)
            {
                writer.WriteFormat("[{0}]", vector_index.Value);
            }
            if (ShiftType != Opcode.Invalid)
            {
                if (ShiftType != Opcode.lsl ||
                    !(ShiftValue is ImmediateOperand imm) ||
                    !imm.Value.IsZero)
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
            if (UserStmLdm)
            {
                writer.WriteChar('^');
            }
        }

        public bool IsStackPointer(int iOp)
        {
            return ops[iOp] is RegisterOperand r && r.Register == Registers.sp;
        }

        public bool IsSinglePop()
        {
            return ops[1] is MemoryOperand mem &&
                mem.BaseRegister == Registers.sp &&
                this.Writeback && 
                !mem.PreIndex &&
                mem.Offset != null &&
                mem.Add &&
                mem.Offset.ToInt32() == ops[0].Width.Size;
        }

        public bool IsSinglePush()
        {
            return ops[1] is MemoryOperand mem &&
                mem.BaseRegister == Registers.sp &&
                this.Writeback &&
                mem.PreIndex &&
                mem.Offset != null &&
                !mem.Add &&
                mem.Offset.ToInt32() == ops[0].Width.Size;
        }

        private (MachineOperand[], bool) RenderMnemonic(MachineInstructionWriter writer)
        {
            var sb = new StringBuilder();
            string sOpcode;
            var ops = (this.ops, this.Writeback);
            if (aliases.TryGetValue(opcode, out ArmAlias armAlias) &&
                armAlias.Matches(this))
            {
                sOpcode = armAlias.sOpcode;
                ops = armAlias.NewOperands(this);
            }
            else if (!opcodes.TryGetValue(opcode, out sOpcode))
            {
                sOpcode = opcode.ToString();
            }
            var sUpdate = SetFlags ? "s" : "";
            var sCond = condition == ArmCondition.AL ? "" : condition.ToString().ToLowerInvariant();
            sb.Append(sOpcode);
            if (opcode != Opcode.Invalid)
            {
                sb.Append(sUpdate);
                sb.Append(sCond);
                if (this.vector_data != ArmVectorData.INVALID)
                {
                    var s = this.vector_data.ToString().ToLowerInvariant();
                    if (s.Length == 6)
                    {
                        s = s.Substring(0, 3) + "." + s.Substring(3);
                    }
                    sb.AppendFormat(".{0}", s);
                }
                else if (Wide)
                {
                    sb.Append(".w");
                }
            }
            writer.WriteOpcode(sb.ToString());
            return ops;
        }

        private string RenderIt()
        {
            var sb = new StringBuilder();
            ;  sb.Append("it");
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
                if (imm.Value.IsReal)
                {
                    writer.WriteFormat("#{0}", imm.Value.ToString());
                }
                else
                {
                    int v = imm.Value.ToInt32();
                    if (0 <= v && v <= 9)
                        writer.WriteFormat($"#{imm.Value.ToInt32()}");
                    else
                        writer.WriteFormat($"#&{imm.Value.ToUInt64():X}");
                }
                break;
            case AddressOperand aop:
                writer.WriteAddress($"${aop.Address}", aop.Address);
                break;
            case MemoryOperand mem:
                if (mem.BaseRegister == Registers.pc)
                {
                    RenderPcRelativeAddressAnnotation(mem, writer, options);
                }
                else
                {
                    RenderMemoryOperand(mem, writer);
                }
                break;
            default:
                op.Write(writer, options);
                break;
            }
        }

        private void RenderMemoryOperand(MemoryOperand mem, MachineInstructionWriter writer)
        {
            writer.WriteChar('[');
            writer.WriteString(mem.BaseRegister.Name);
            if (this.Writeback && !mem.PreIndex)
            {
                // Post-indexed
                if (mem.Alignment != 0)
                {
                    writer.WriteFormat(":{0}", mem.Alignment);
                }
                writer.WriteString("]");
                if (mem.Offset != null && !mem.Offset.IsIntegerZero)
                {
                    writer.WriteChar(',');
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
                if (mem.Alignment != 0)
                {
                    writer.WriteFormat(":{0}", mem.Alignment);
                }
                writer.WriteChar(']');
                if (this.Writeback)
                {
                    writer.WriteChar('!');
                }
            }
        }

        private void RenderPcRelativeAddressAnnotation(MemoryOperand mem, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var addr = ComputePcRelativeAddress(mem);
            if (mem.Index == null &&
                (options & MachineInstructionWriterOptions.ResolvePcRelativeAddress) != 0)
            {
                writer.WriteChar('[');
                writer.WriteAddress(addr.ToString(), addr);
                writer.WriteChar(']');

                var sr = new StringRenderer();
                RenderMemoryOperand(mem, sr);
                var str = sr.ToString();
                writer.AddAnnotation(str);
            }
            else
            {
                RenderMemoryOperand(mem, writer);
                writer.AddAnnotation(addr.ToString());
            }
        }

        public bool Writeback;
        public bool SetFlags;
        public bool UserStmLdm;
        public bool Wide;               // (Thumb only) wide form of instruction.
        public Opcode ShiftType;
        public MachineOperand ShiftValue;
        public ArmVectorData vector_data;
        public int vector_size;         // only valid if vector_data is valid
        public int? vector_index;
        public byte itmask;
    }

    public class A32Instruction : AArch32Instruction
    {
        public override Address ComputePcRelativeAddress(MemoryOperand mem)
        {
            int offset = 8;     // PC-relative addressing has a hidden 8-byte offset.
            if (mem.Offset != null)
                offset += mem.Offset.ToInt32();
            var addr = this.Address + offset;
            return addr;
        }
    }

    public class T32Instruction : AArch32Instruction
    {
        public override Address ComputePcRelativeAddress(MemoryOperand mem)
        {
            int offset = 2;
            if (mem.Offset != null)
                offset += mem.Offset.ToInt32();
            var addr = (this.Address + offset).Align(4);
            return addr;
        }
    }
}
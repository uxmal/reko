#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Expressions;
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

        // Specially rendered mnemonics
        private static readonly Dictionary<Mnemonic, string> mnemonics = new Dictionary<Mnemonic, string>
        {
        };

        // Block data transfer mnemonics that affect the rendering of the first operand.
        private static readonly HashSet<Mnemonic> blockDataXferMnemonics = new HashSet<Mnemonic>
        {
            Mnemonic.ldm, Mnemonic.ldmda, Mnemonic.ldmdb, Mnemonic.ldmib,
            Mnemonic.stm, Mnemonic.stmda, Mnemonic.stmdb, Mnemonic.stmib,
            Mnemonic.vldmia, Mnemonic.vldmdb, Mnemonic.vstmia, Mnemonic.vstmdb
        };

        // Instruction aliases render instructions differently under special conditions.
        private class ArmAlias
        {
            public readonly Func<AArch32Instruction, bool> Matches;  // predicate for the alias
            public readonly string sMnemonic;
            public readonly Func<AArch32Instruction, (MachineOperand[], bool)> NewOperands;   // Mutated operands

            public ArmAlias(
                Func<AArch32Instruction, bool> match,
                string sOpcode, 
                Func<AArch32Instruction, (MachineOperand[], bool)> newOperands)
            {
                this.Matches = match;
                this.sMnemonic = sOpcode;
                this.NewOperands = newOperands;
            }

            public static (MachineOperand[], bool) Shift(AArch32Instruction i)
            {
                return (i.Operands.Skip(1).ToArray(), false);
            }

            public static (MachineOperand[], bool) FirstOp(AArch32Instruction i)
            {
                return (i.Operands.Take(1).ToArray(), false);
            }
        }

        private static readonly Dictionary<Mnemonic, ArmAlias> aliases = new Dictionary<Mnemonic, ArmAlias>
        {
            { Mnemonic.ldm, new ArmAlias(i => i.Writeback && i.IsStackPointer(0), "pop", ArmAlias.Shift) },
            { Mnemonic.ldr, new ArmAlias(i => i.IsSinglePop(), "pop", ArmAlias.FirstOp) },
            { Mnemonic.stmdb, new ArmAlias(i => i.Writeback && i.IsStackPointer(0), "push", ArmAlias.Shift) },
            { Mnemonic.str, new ArmAlias(i => i.IsSinglePush(), "push", ArmAlias.FirstOp) },
        };
        #endregion

        public AArch32Instruction()
        {
            this.Condition = ArmCondition.AL;
        }

        public Mnemonic Mnemonic { get; set; }
        public ArmCondition Condition { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;
        public override string MnemonicAsString => Mnemonic.ToString();

        /// <summary>
        /// PC-relative addressing has an extra offset.This varies
        /// between the T32 and the A32 instruction sets.
        /// </summary>
        public abstract Address ComputePcRelativeAddress(MemoryOperand mem);


        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Mnemonic == Mnemonic.it)
            {
                var itMnemonic = RenderIt();
                renderer.WriteMnemonic(itMnemonic);
                renderer.Tab();
                renderer.WriteString(Condition.ToString().ToLowerInvariant());
                return;
            }
            var (ops, writeback) = RenderMnemonic(renderer);
            if (ops.Length > 0)
            {
                renderer.Tab();
                RenderOperand(ops[0], renderer, options);
                if (writeback &&
                    blockDataXferMnemonics.Contains(Mnemonic) &&
                    ops[0] is RegisterStorage)
                {
                    renderer.WriteChar('!');
                }
                for (int iOp = 1; iOp < ops.Length; ++iOp)
                {
                    renderer.WriteChar(',');
                    RenderOperand(ops[iOp], renderer, options);
                }
            }
    
            if (ShiftType != Mnemonic.Invalid)
            {
                if (ShiftType != Mnemonic.lsl ||
                    ShiftValue is not Constant imm ||
                    !imm.IsZero)
                {
                    renderer.WriteChar(',');
                    renderer.WriteMnemonic(ShiftType.ToString());
                    if (ShiftType != Mnemonic.rrx)
                    {
                        renderer.WriteChar(' ');
                        RenderOperand(ShiftValue!, renderer, options);
                    }
                }
            }
            if (UserStmLdm)
            {
                renderer.WriteChar('^');
            }
        }

        public bool IsStackPointer(int iOp)
        {
            return Operands[iOp] is RegisterStorage r && r == Registers.sp;
        }

        public bool IsSinglePop()
        {
            return Operands[1] is MemoryOperand mem &&
                mem.BaseRegister == Registers.sp &&
                this.Writeback && 
                !mem.PreIndex &&
                mem.Offset is not null &&
                mem.Add &&
                mem.Offset.ToInt32() == Operands[0].DataType.Size;
        }

        public bool IsSinglePush()
        {
            return Operands[1] is MemoryOperand mem &&
                mem.BaseRegister == Registers.sp &&
                this.Writeback &&
                mem.PreIndex &&
                mem.Offset is not null &&
                !mem.Add &&
                mem.Offset.ToInt32() == Operands[0].DataType.Size;
        }

        private (MachineOperand[], bool writeback) RenderMnemonic(MachineInstructionRenderer renderer)
        {
            var sb = new StringBuilder();
            string? sMnemonic;
            var ops = (this.Operands, this.Writeback);
            if (aliases.TryGetValue(Mnemonic, out ArmAlias? armAlias) &&
                armAlias.Matches(this))
            {
                sMnemonic = armAlias.sMnemonic;
                ops = armAlias.NewOperands(this);
            }
            else if (!mnemonics.TryGetValue(Mnemonic, out sMnemonic))
            {
                sMnemonic = Mnemonic.ToString();
            }
            var sUpdate = SetFlags ? "s" : "";
            var sCond = Condition == ArmCondition.AL ? "" : Condition.ToString().ToLowerInvariant();
            sb.Append(sMnemonic);
            if (Mnemonic != Mnemonic.Invalid)
            {
                sb.Append(sUpdate);
                sb.Append(sCond);
                if (this.vector_data != ArmVectorData.INVALID)
                {
                    var s = this.vector_data.ToString().ToLowerInvariant();
                    if (s.Length == 6)
                    {
                        s = s[..3] + "." + s.Substring(3);
                    }
                    sb.AppendFormat(".{0}", s);
                }
                else if (Wide)
                {
                    sb.Append(".w");
                }
            }
            renderer.WriteMnemonic(sb.ToString());
            return ops;
        }

        private string RenderIt()
        {
            var sb = new StringBuilder();
            sb.Append("it");
            int mask = this.itmask;
            var bit = (~(int)this.Condition & 1) << 3;

            while ((mask & 0xF) != 8)
            {
                sb.Append(((mask ^ bit) & 0x8) != 0 ? 't' : 'e');
                mask <<= 1;
            }
            return sb.ToString();
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (op)
            {
            case Constant imm:
                if (imm.IsReal)
                {
                    renderer.WriteFormat("#{0}", imm.ToString());
                }
                else
                {
                    int v = imm.ToInt32();
                    if (0 <= v && v <= 9)
                        renderer.WriteFormat($"#{imm.ToInt32()}");
                    else
                        renderer.WriteFormat($"#&{imm.ToUInt64():X}");
                }
                break;
            case Address aop:
                renderer.WriteAddress($"${aop}", aop);
                break;
            case MemoryOperand mem:
                if (mem.BaseRegister == Registers.pc)
                {
                    RenderPcRelativeAddressAnnotation(mem, renderer, options);
                }
                else
                {
                    RenderMemoryOperand(mem, renderer);
                }
                break;
            default:
                op.Render(renderer, options);
                break;
            }
        }

        private void RenderMemoryOperand(MemoryOperand mem, MachineInstructionRenderer renderer)
        {
            renderer.WriteChar('[');
            renderer.WriteString(mem.BaseRegister!.Name);
            if (this.Writeback && !mem.PreIndex)
            {
                // Post-indexed
                if (mem.Alignment != 0)
                {
                    renderer.WriteFormat(":{0}", mem.Alignment);
                }
                renderer.WriteString("]");
                if (mem.Offset is not null && !mem.Offset.IsIntegerZero)
                {
                    renderer.WriteChar(',');
                    if (!mem.Add)
                        renderer.WriteChar('-');
                    renderer.WriteString("#&");
                    renderer.WriteString(mem.Offset.ToUInt32().ToString("X"));
                }
                else if (mem.Index is not null)
                {
                    renderer.WriteChar(',');
                    if (!mem.Add)
                        renderer.WriteChar('-');
                    renderer.WriteString(mem.Index.Name);
                }
            }
            else
            {
                if (mem.Offset is not null && !mem.Offset.IsIntegerZero)
                {
                    renderer.WriteString(",");
                    if (!mem.Add)
                        renderer.WriteChar('-');
                    renderer.WriteString("#&");
                    renderer.WriteString(mem.Offset.ToUInt32().ToString("X"));
                }
                else if (mem.Index is not null)
                {
                    renderer.WriteChar(',');
                    if (!mem.Add)
                        renderer.WriteChar('-');
                    renderer.WriteString(mem.Index.Name);
                    if (mem.ShiftType != Mnemonic.Invalid)
                    {
                        renderer.WriteChar(',');
                        renderer.WriteString(mem.ShiftType.ToString().ToLowerInvariant());
                        if (this.ShiftType != Mnemonic.rrx)
                        {
                            renderer.WriteFormat(" #{0}", mem.Shift);
                        }
                    }
                }
                if (mem.Alignment != 0)
                {
                    renderer.WriteFormat(":{0}", mem.Alignment);
                }
                renderer.WriteChar(']');
                if (this.Writeback)
                {
                    renderer.WriteChar('!');
                }
            }
        }

        private void RenderPcRelativeAddressAnnotation(MemoryOperand mem, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var addr = ComputePcRelativeAddress(mem);
            if (mem.Index is null &&
                (options.Flags & MachineInstructionRendererFlags.ResolvePcRelativeAddress) != 0)
            {
                renderer.WriteChar('[');
                renderer.WriteAddress(addr.ToString(), addr);
                renderer.WriteChar(']');

                var sr = new StringRenderer();
                RenderMemoryOperand(mem, sr);
                var str = sr.ToString();
                renderer.AddAnnotation(str);
            }
            else
            {
                RenderMemoryOperand(mem, renderer);
                renderer.AddAnnotation(addr.ToString());
            }
        }

        public bool Writeback;
        public bool SetFlags;
        public bool UserStmLdm;
        public bool Wide;               // (Thumb only) wide form of instruction.
        public Mnemonic ShiftType;
        public MachineOperand? ShiftValue;
        public ArmVectorData vector_data;
        public int vector_size;         // only valid if vector_data is valid
        public byte itmask;
    }

    public class A32Instruction : AArch32Instruction
    {
        public override Address ComputePcRelativeAddress(MemoryOperand mem)
        {
            int offset = 8;     // PC-relative addressing has a hidden 8-byte offset.
            if (mem.Offset is not null)
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
            if (mem.Offset is not null)
                offset += mem.Offset.ToInt32();
            var addr = (this.Address + offset).Align(4);
            return addr;
        }
    }
}
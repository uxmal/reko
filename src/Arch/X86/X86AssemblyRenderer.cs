#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Reko.Arch.X86
{
    public abstract class X86AssemblyRenderer
    {
        private static readonly char[] floatSpecials = new char[] { '.', 'e', 'E' };

        public static X86AssemblyRenderer Intel { get; } = new IntelAssemblyRenderer();
        public static X86AssemblyRenderer Nasm { get; } = new NasmAssemblyRenderer();


        public string FormatValue(Constant c, bool forceSignForSignedIntegers = true)
        {
            var pt = (PrimitiveType) c.DataType;
            if (pt.Domain == Domain.SignedInt)
            {
                return FormatSignedValue(c.ToInt64(), forceSignForSignedIntegers);
            }
            else if (pt.Domain == Domain.Real)
            {
                var str = c.ToReal64().ToString("G", CultureInfo.InvariantCulture);
                if (str.IndexOfAny(floatSpecials) < 0)
                {
                    return str + ".0";
                }
                return str;
            }
            else
                return FormatUnsignedValue(c.ToUInt64());
        }

        public void Render(X86Instruction instr, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (instr.repPrefix == 3)
            {
                writer.WriteMnemonic("rep");
                writer.WriteChar(' ');
            }
            else if (instr.repPrefix == 2)
            {
                writer.WriteMnemonic("repne");
                writer.WriteChar(' ');
            }

            var s = new StringBuilder(instr.Mnemonic.ToString());
            switch (instr.Mnemonic)
            {
            case Mnemonic.ins:
            case Mnemonic.outs:
            case Mnemonic.movs:
            case Mnemonic.cmps:
            case Mnemonic.stos:
            case Mnemonic.lods:
            case Mnemonic.scas:
                switch (instr.dataWidth.Size)
                {
                case 1: s.Append('b'); break;
                case 2: s.Append('w'); break;
                case 4: s.Append('d'); break;
                case 8: s.Append('q'); break;
                default: throw new ArgumentOutOfRangeException();
                }
                break;
            }
            writer.WriteMnemonic(s.ToString());

            var flags = options.Flags;
            if (NeedsExplicitMemorySize(instr))
            {
                flags |= MachineInstructionWriterFlags.ExplicitOperandSize;
            }
            if (instr.Operands.Length > 0)
            {
                writer.Tab();
                RenderOperand(instr.Operands[0], instr, writer, flags);
                for (int i = 1; i < instr.Operands.Length; ++i)
                {
                    writer.WriteString(options.OperandSeparator ?? ",");
                    RenderOperand(instr.Operands[i], instr, writer, flags);
                }
            }
        }

        protected virtual void RenderOperand(MachineOperand operand, X86Instruction instr, MachineInstructionWriter writer, MachineInstructionWriterFlags flags)
        {
            switch (operand)
            {
            case RegisterOperand reg:
                writer.WriteString(reg.Register.Name);
                break;
            case ImmediateOperand imm:
                RenderImmediate(imm, instr, writer);
                break;
            case MemoryOperand memOp:
                if (memOp.Base == Registers.rip)
                {
                    var addr = instr.Address + instr.Length + memOp.Offset!.ToInt32();
                    if ((flags & MachineInstructionWriterFlags.ResolvePcRelativeAddress) != 0)
                    {
                        writer.WriteString("[");
                        writer.WriteAddress(addr.ToString(), addr);
                        writer.WriteString("]");
                        writer.AddAnnotation(memOp.ToString());
                    }
                    else
                    {
                        RenderMemory(memOp, writer, flags);
                        writer.AddAnnotation(addr.ToString());
                    }
                }
                else
                {
                    RenderMemory(memOp, writer, flags);
                }
                break;
            case AddressOperand addrOp:
                if (addrOp.Address.Selector.HasValue)
                {
                    writer.WriteString("far");
                    writer.WriteString(" ");
                    writer.WriteString(FormatUnsignedValue(addrOp.Address.Selector.Value, "{0:X4}"));
                    writer.WriteChar(':');
                    writer.WriteString(FormatUnsignedValue(addrOp.Address.Offset, "{0:X4}"));
                }
                else
                {
                    writer.WriteAddress(FormatUnsignedValue(addrOp.Address.ToLinear(), "{0:X4}"), addrOp.Address);
                }
                break;
            case FpuOperand fpu:
                writer.WriteFormat("st({0})", fpu.StNumber);
                break;

            default: throw new NotImplementedException(operand.GetType().Name);
            }
        }

        protected void RenderImmediate(
            ImmediateOperand imm,
            X86Instruction instr,
            MachineInstructionWriter writer)
        {
            if (imm.Value.DataType is PrimitiveType pt)
            {
                if (pt.Domain == Domain.Pointer)
                    writer.WriteAddress(FormatValue(imm.Value), Address.FromConstant(imm.Value));
                else if (pt.Domain == Domain.Offset)
                    writer.WriteString(FormatUnsignedValue(imm.Value.ToUInt64(), "{0:X4}"));
                else
                    writer.WriteString(FormatValue(imm.Value));
            }
            else
            {
                var s = FormatValue(imm.Value);
                if (imm.Value.DataType is Pointer)
                    writer.WriteAddress(s, Address.FromConstant(imm.Value));
                else
                    writer.WriteString(s);
            }
        }

        protected void RenderMemory(
            MemoryOperand mem,
            MachineInstructionWriter writer,
            MachineInstructionWriterFlags flags)
        {
            if ((flags & MachineInstructionWriterFlags.ExplicitOperandSize) != 0)
            {
                var s = ExplicitOperandPrefix(mem.Width);
                writer.WriteString(s);
            }

            if (mem.SegOverride != RegisterStorage.None)
            {
                writer.WriteString(mem.SegOverride.ToString());
                writer.WriteString(":");
            }
            writer.WriteString("[");
            if (mem.Base != RegisterStorage.None)
            {
                writer.WriteString(mem.Base.ToString());
            }
            else
            {
                var s = FormatUnsignedValue(mem.Offset!.ToUInt64(), "{0:X4}");
                writer.WriteAddress(s, Address.FromConstant(mem.Offset!));
            }

            if (mem.Index != RegisterStorage.None)
            {
                writer.WriteString("+");
                writer.WriteString(mem.Index.ToString());
                if (mem.Scale > 1)
                {
                    writer.WriteString("*");
                    writer.WriteUInt32(mem.Scale);
                }
            }
            if (mem.Base != RegisterStorage.None && mem.Offset != null && mem.Offset.IsValid)
            {
                if (mem.Offset.DataType == PrimitiveType.Byte || mem.Offset.DataType == PrimitiveType.SByte)
                {
                    writer.WriteString(FormatSignedValue(mem.Offset.ToInt64(), true));
                }
                else
                {
                    var off = mem.Offset.ToInt32();
                    if (off == Int32.MinValue)
                    {
                        writer.WriteString("-80000000h");
                    }
                    else
                    {
                        var absOff = Math.Abs(off);
                        if (mem.Offset.DataType.Size > 2 && off < 0 && absOff < 0x10000)
                        {
                            // Special case for negative 32-bit offsets whose 
                            // absolute value < 0x10000 (GitHub issue #252)
                            writer.WriteString("-");
                            writer.WriteFormat(FormatUnsignedValue((ulong)absOff));
                        }
                        else
                        {
                            writer.WriteString("+");
                            writer.WriteString(FormatUnsignedValue(mem.Offset.ToUInt64()));
                        }
                    }
                }
            }
            writer.WriteString("]");
        }

        protected abstract string FormatSignedValue(long n, bool forceSign);

        protected abstract string FormatUnsignedValue(ulong n, string? format = null);

        protected abstract string ExplicitOperandPrefix(PrimitiveType width);

        private bool NeedsExplicitMemorySize(X86Instruction instr)
        {
            switch (instr.Mnemonic)
            {
            case Mnemonic.movsx:
            case Mnemonic.movzx:
            case Mnemonic.movsxd:
                return true;
            case Mnemonic.lea:
            case Mnemonic.lds:
            case Mnemonic.les:
            case Mnemonic.lfs:
            case Mnemonic.lgs:
            case Mnemonic.lss:
                return false;
            default:
                var ops = instr.Operands;
                if (ops.Length >= 2 && ops[0].Width.Size != ops[1].Width.Size)
                    return true;
                return
                     (ops.Length < 1 || !HasImplicitWidth(ops[0])) &&
                     (ops.Length < 2 || !HasImplicitWidth(ops[1])) &&
                     (ops.Length < 3 || !HasImplicitWidth(ops[2]));
            }
        }

        private bool HasImplicitWidth(MachineOperand op)
        {
            return op is RegisterOperand || op is AddressOperand || op is FpuOperand;
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

        public static X86AssemblyRenderer Att { get; } = new AttAssemblyRenderer();
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

        public void Render(X86Instruction instr, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderPrefix(instr, renderer);

            var s = new StringBuilder();
            RenderMnemonic(instr, s);
            renderer.WriteMnemonic(s.ToString());

            if (instr.Operands.Length > 0)
            {
                renderer.Tab();
                RenderOperands(instr, options, renderer);
            }
        }

        protected virtual void RenderMnemonic(X86Instruction instr, StringBuilder s)
        {
            s.Append(instr.MnemonicAsString);
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
        }

        protected virtual void RenderOperands(X86Instruction instr, MachineInstructionRendererOptions options, MachineInstructionRenderer renderer)
        {
            var flags = options.Flags;
            RenderOperand(instr, 0, renderer, options);
            if (instr.OpMask != 0)
            {
                renderer.WriteString("{k");
                renderer.WriteUInt32(instr.OpMask);
                renderer.WriteChar('}');
            }
            if (instr.MergingMode != 0)
            {
                renderer.WriteString("{z}");
            }
            for (int i = 1; i < instr.Operands.Length; ++i)
            {
                renderer.WriteString(options.OperandSeparator ?? ",");
                RenderOperand(instr, i, renderer, options);
            }
        }

        protected void RenderPrefix(X86Instruction instr, MachineInstructionRenderer renderer)
        {
            //$TODO: make 'lock' a prefix
            if (instr.repPrefix == 3)
            {
                renderer.WriteMnemonic("rep");
                renderer.WriteChar(' ');
            }
            else if (instr.repPrefix == 2)
            {
                renderer.WriteMnemonic("repne");
                renderer.WriteChar(' ');
            }
        }

        protected virtual void RenderOperand(
            X86Instruction instr,
            int iop,
            MachineInstructionRenderer renderer,
            MachineInstructionRendererOptions options)
        {
            var operand = instr.Operands[iop];
            switch (operand)
            {
            case RegisterStorage reg:
                RenderRegister(reg.Name, renderer);
                break;
            case ImmediateOperand imm:
                RenderImmediate(imm, instr, renderer);
                break;
            case MemoryOperand memOp:
                var flags = options.Flags;
                if (NeedsExplicitMemorySize(instr))
                {
                    flags |= MachineInstructionRendererFlags.ExplicitOperandSize;
                }

                if (memOp.Base == Registers.rip)
                {
                    var addr = instr.Address + instr.Length + memOp.Offset!.ToInt32();
                    if ((flags & MachineInstructionRendererFlags.ResolvePcRelativeAddress) != 0)
                    {
                        renderer.WriteString("[");
                        renderer.WriteAddress(addr.ToString(), addr);
                        renderer.WriteString("]");
                        renderer.AddAnnotation(memOp.ToString());
                    }
                    else
                    {
                        RenderMemory(memOp, renderer, flags);
                        renderer.AddAnnotation(addr.ToString());
                    }
                }
                else
                {
                    RenderMemory(memOp, renderer, flags);
                }
                if (instr.Broadcast)
                {
                    renderer.WriteString("{1to");
                    var xmmSize = instr.Operands[1].Width.BitSize;
                    renderer.WriteUInt32((uint) (xmmSize / memOp.Width.BitSize));
                    renderer.WriteChar('}');
                }
                break;
            case AddressOperand addrOp:
                if (addrOp.Address.Selector.HasValue)
                {
                    renderer.WriteString("far");
                    renderer.WriteString(" ");
                    renderer.WriteString(FormatUnsignedValue(addrOp.Address.Selector.Value, "{0:X4}"));
                    renderer.WriteChar(':');
                    renderer.WriteString(FormatUnsignedValue(addrOp.Address.Offset, "{0:X4}"));
                }
                else
                {
                    renderer.WriteAddress(FormatUnsignedValue(addrOp.Address.ToLinear(), "{0:X4}"), addrOp.Address);
                }
                break;
            case FpuOperand fpu:
                renderer.WriteFormat("st({0})", fpu.StNumber);
                break;

            default: throw new NotImplementedException(operand.GetType().Name);
            }
        }

        internal void RenderSymbolReference(Address address, string name, long offset, MachineInstructionRenderer renderer)
        {
            throw new NotImplementedException();
        }

        protected virtual void RenderRegister(string regName, MachineInstructionRenderer renderer)
        {
            renderer.WriteString(regName);
        }

        protected void RenderImmediate(
            ImmediateOperand imm,
            X86Instruction instr,
            MachineInstructionRenderer renderer)
        {
            var pt = imm.Value.DataType;
            if (pt.Domain == Domain.Offset)
                renderer.WriteString(FormatUnsignedValue(imm.Value.ToUInt64(), "{0:X4}"));
            else
            {
                var s = FormatValue(imm.Value);
                if (pt.Domain == Domain.Pointer)
                    renderer.WriteAddress(s, Address.FromConstant(imm.Value));
                else
                    renderer.WriteString(s);
            }
        }

        protected void RenderMemory(
            MemoryOperand mem,
            MachineInstructionRenderer renderer,
            MachineInstructionRendererFlags flags)
        {
            if ((flags & MachineInstructionRendererFlags.ExplicitOperandSize) != 0)
            {
                var s = ExplicitOperandPrefix(mem.Width);
                renderer.WriteString(s);
            }

            if (mem.SegOverride != RegisterStorage.None)
            {
                RenderRegister(mem.SegOverride.Name, renderer);
                renderer.WriteString(":");
            }
            renderer.WriteString("[");
            if (mem.Base != RegisterStorage.None)
            {
                RenderRegister(mem.Base.Name, renderer);
            }
            else
            {
                var s = FormatUnsignedValue(mem.Offset!.ToUInt64(), "{0:X4}");
                renderer.WriteAddress(s, Address.FromConstant(mem.Offset!));
            }

            if (mem.Index != RegisterStorage.None)
            {
                renderer.WriteString("+");
                RenderRegister(mem.Index.Name, renderer);
                if (mem.Scale > 1)
                {
                    renderer.WriteString("*");
                    renderer.WriteUInt32(mem.Scale);
                }
            }
            if (mem.Base != RegisterStorage.None && mem.Offset != null && mem.Offset.IsValid)
            {
                if (mem.Offset.DataType == PrimitiveType.Byte || mem.Offset.DataType == PrimitiveType.SByte)
                {
                    renderer.WriteString(FormatSignedValue(mem.Offset.ToInt64(), true));
                }
                else
                {
                    var off = mem.Offset.ToInt32();
                    if (off == Int32.MinValue)
                    {
                        renderer.WriteString("-80000000h");
                    }
                    else
                    {
                        var absOff = Math.Abs(off);
                        if (mem.Offset.DataType.Size > 2 && off < 0 && absOff < 0x10000)
                        {
                            // Special case for negative 32-bit offsets whose 
                            // absolute value < 0x10000 (GitHub issue #252)
                            renderer.WriteString("-");
                            renderer.WriteFormat(FormatUnsignedValue((ulong)absOff));
                        }
                        else
                        {
                            renderer.WriteString("+");
                            renderer.WriteString(FormatUnsignedValue(mem.Offset.ToUInt64()));
                        }
                    }
                }
            }
            renderer.WriteString("]");
        }

        protected abstract string FormatSignedValue(long n, bool forceSign);

        protected abstract string FormatUnsignedValue(ulong n, string? format = null);

        protected abstract string ExplicitOperandPrefix(DataType width);

        protected bool NeedsExplicitMemorySize(X86Instruction instr)
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
                if (ops.Length == 0)
                    return false;
                if (ops.Length >= 2 && ops[0].Width.Size != ops[1].Width.Size)
                    return true;
                if (ops.Length >= 3 && ops[2] is MemoryOperand mop && ops[0].Width.Size != mop.Width.Size)
                    return true;
                return
                         (ops.Length < 1 || !HasImplicitWidth(ops[0])) &&
                     (ops.Length < 2 || !HasImplicitWidth(ops[1])) &&
                     (ops.Length < 3 || !HasImplicitWidth(ops[2]));
            }
        }

        protected bool HasImplicitWidth(MachineOperand op)
        {
            return op is RegisterStorage || op is AddressOperand || op is FpuOperand;
        }
    }
}

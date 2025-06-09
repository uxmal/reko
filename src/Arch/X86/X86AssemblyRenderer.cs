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
using Reko.Core.Types;
using System;
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
            if (IsStringInstruction(instr))
            {
                RenderStringInstruction(instr, renderer, options);
                return;
            }

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
            if (IsStringInstruction(instr))
            switch (instr.DataWidth.Size)
            {
            case 1: break;
            case 2: s.Append('w'); break;
            case 4: s.Append('d'); break;
            case 8: s.Append('q'); break;
            default: throw new ArgumentOutOfRangeException($"Unrecognized operand size {instr.DataWidth.Size}.");
            }
        }

        private static bool IsStringInstruction(X86Instruction instr)
        {
            switch (instr.Mnemonic)
            {
            case Mnemonic.ins:
            case Mnemonic.insb:
            case Mnemonic.outs:
            case Mnemonic.outsb:
            case Mnemonic.movs:
            case Mnemonic.movsb:
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
            case Mnemonic.stos:
            case Mnemonic.stosb:
            case Mnemonic.lods:
            case Mnemonic.lodsb:
            case Mnemonic.scas:
            case Mnemonic.scasb:
                return true;
            default:
                return false;
            }
        }

        protected virtual void RenderStringInstruction(
            X86Instruction instr, 
            MachineInstructionRenderer renderer,
            MachineInstructionRendererOptions options)
        {
            static bool HasSegmentOverride(X86Instruction instr, int i)
            {
                if (instr.Operands.Length <= i)
                    return false;
                return instr.Operands[i] is MemoryOperand mem &&
                    mem.SegOverride != RegisterStorage.None;
            }

            var s = new StringBuilder();
            RenderMnemonic(instr, s);
            renderer.WriteMnemonic(s.ToString());

            // Only render the memory operands if they are needed.
            if (!HasSegmentOverride(instr, 0) && !HasSegmentOverride(instr, 1))
                return;
            renderer.Tab();
            RenderOperands(instr, options, renderer);
        }

        protected virtual void RenderOperands(X86Instruction instr, MachineInstructionRendererOptions options, MachineInstructionRenderer renderer)
        {
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

        protected static void RenderPrefix(X86Instruction instr, MachineInstructionRenderer renderer)
        {
            //$TODO: make 'lock' a prefix
            if (instr.RepPrefix == 3)
            {
                renderer.WriteMnemonic("rep");
                renderer.WriteChar(' ');
            }
            else if (instr.RepPrefix == 2)
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
            case Constant imm:
                RenderImmediate(imm, renderer);
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
                    var xmmSize = instr.Operands[iop-1].DataType.BitSize;
                    renderer.WriteUInt32((uint) (xmmSize / memOp.DataType.BitSize));
                    renderer.WriteChar('}');
                }
                break;
            case Address addrOp:
                if (addrOp.Selector.HasValue)
                {
                    renderer.WriteString("far");
                    renderer.WriteString(" ");
                    renderer.WriteString(FormatUnsignedValue(addrOp.Selector.Value, "{0:X4}"));
                    renderer.WriteChar(':');
                    renderer.WriteString(FormatUnsignedValue(addrOp.Offset, "{0:X4}"));
                }
                else
                {
                    renderer.WriteAddress(FormatUnsignedValue(addrOp.ToLinear(), "{0:X4}"), addrOp);
                }
                break;
            case FpuOperand fpu:
                renderer.WriteFormat("st({0})", fpu.StNumber);
                break;
            case SaeOperand sae:
                sae.Render(renderer, options);
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
            Constant imm,
            MachineInstructionRenderer renderer)
        {
            var pt = imm.DataType;
            if (pt.Domain == Domain.Offset)
                renderer.WriteString(FormatUnsignedValue(imm.ToUInt64(), "{0:X4}"));
            else
            {
                var s = FormatValue(imm);
                if (pt.Domain == Domain.Pointer)
                    renderer.WriteAddress(s, Address.FromConstant(imm));
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
                var s = ExplicitOperandPrefix(mem.DataType);
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
            if (mem.Base != RegisterStorage.None && mem.Offset is not null && mem.Offset.IsValid)
            {
                if (mem.Offset.DataType == PrimitiveType.Byte || mem.Offset.DataType.Domain == Domain.SignedInt)
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

        protected static bool NeedsExplicitMemorySize(X86Instruction instr)
        {
            switch (instr.Mnemonic)
            {
            case Mnemonic.movsx:
            case Mnemonic.movzx:
            case Mnemonic.movsxd:
                return true;
            case Mnemonic.fxrstor:
            case Mnemonic.fxsave:
            case Mnemonic.lea:
            case Mnemonic.lds:
            case Mnemonic.les:
            case Mnemonic.lfs:
            case Mnemonic.lgdt:
            case Mnemonic.lidt:
            case Mnemonic.lgs:
            case Mnemonic.lss:
            case Mnemonic.sgdt:
            case Mnemonic.sidt:
            case Mnemonic.xrstor:
            case Mnemonic.xrstor64:
            case Mnemonic.xsave:
            case Mnemonic.xsave64:
            case Mnemonic.xsaveopt64:
                return false;
            default:
                var ops = instr.Operands;
                if (ops.Length == 0)
                    return false;
                if (ops.Length >= 2 && ops[0].DataType.Size != ops[1].DataType.Size)
                    return true;
                if (ops.Length >= 3 && ops[2] is MemoryOperand mop && ops[0].DataType.Size != mop.DataType.Size)
                    return true;
                return
                         (ops.Length < 1 || !HasImplicitWidth(ops[0])) &&
                     (ops.Length < 2 || !HasImplicitWidth(ops[1])) &&
                     (ops.Length < 3 || !HasImplicitWidth(ops[2]));
            }
        }

        protected static bool HasImplicitWidth(MachineOperand op)
        {
            return op is RegisterStorage || op is Address || op is FpuOperand;
        }
    }
}

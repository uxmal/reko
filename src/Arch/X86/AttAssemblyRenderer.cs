#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Renders X86 instructions using AT&T syntax.
    /// </summary>
    public class AttAssemblyRenderer : X86AssemblyRenderer
    {
        protected override void RenderMnemonic(X86Instruction instr, StringBuilder sb)
        {
            switch (instr.Mnemonic)
            {
            case Mnemonic.retf:
                sb.Append("lret");
                return;
            case Mnemonic.jmp:
            case Mnemonic.call:
                if (IsFar(instr.Operands[0]))
                {
                    sb.Append("l");
                    sb.Append(instr.MnemonicAsString);
                    return;
                }
                else
                {
                    sb.Append(instr.MnemonicAsString);
                }
                break;
            case Mnemonic.bound:
            case Mnemonic.enter:
                sb.Append(instr.MnemonicAsString);
                return;
            case Mnemonic.cbw:
                sb.Append("cbtw");
                return;
            case Mnemonic.cdq:
                sb.Append("cltd");
                return;
            case Mnemonic.cdqe:
                sb.Append("cltq");
                return;
            case Mnemonic.cqo:
                sb.Append("cqto");
                return;
            case Mnemonic.cwd:
                sb.Append("cwtd");
                return;
            case Mnemonic.cwde:
                sb.Append("cwtl");
                return;
            case Mnemonic.cmps:
            case Mnemonic.ins:
            case Mnemonic.lods:
            case Mnemonic.movs:
            case Mnemonic.outs:
            case Mnemonic.stos:
                sb.Append(instr.MnemonicAsString);
                sb.Append(MnemonicSuffix(instr.dataWidth));
                return;

            case Mnemonic.movsx:
                sb.Append("movs");
                sb.Append(MnemonicSuffix(instr.Operands[1]));
                sb.Append(MnemonicSuffix(instr.Operands[0]));
                return;
            case Mnemonic.movzx:
                sb.Append("movz");
                sb.Append(MnemonicSuffix(instr.Operands[1]));
                sb.Append(MnemonicSuffix(instr.Operands[0]));
                return;
            default:
                sb.Append(instr.MnemonicAsString);
                break;
            }

            if (NeedsExplicitMemorySize(instr))
            {
                char suffix = MnemonicSuffix(instr.Operands[0]);
                sb.Append(suffix);
            }
        }

        private char MnemonicSuffix(MachineOperand op) => MnemonicSuffix(op.Width);

        private char MnemonicSuffix(DataType dt)
        {
            switch (dt.BitSize)
            {
            case 8: return 'b';
            case 16: return 'w';
            case 32: return 'l';
            case 64: return 'q';
            case 80: return 't';
            default: return '?';
            }
        }

        private static bool IsFar(MachineOperand op)
        {
            return op.Width.Domain == Domain.SegPointer;
        }

        protected override string ExplicitOperandPrefix(DataType width)
        {
            throw new NotImplementedException();
        }

        protected override string FormatSignedValue(long n, bool forceSign)
        {
            string fmt;
            if (n < 0)
            {
                n = -n;
                fmt = "-0x{0:X}";
            }
            else if (forceSign)
            {
                fmt = "+0x{0:X}";
            }
            else
            {
                fmt = "0x{0:X}";
            }
            return string.Format(fmt, n);
        }

        protected override string FormatUnsignedValue(ulong n, string? format)
        {
            return "0x" + string.Format(format ?? "{0:X}", n);
        }

        protected override void RenderOperand(X86Instruction instr, int iop, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            MachineOperand operand = instr.Operands[iop];
            switch (operand)
            {
            case RegisterStorage reg:
                if (instr.Mnemonic == Mnemonic.call || instr.Mnemonic== Mnemonic.jmp)
                {
                    renderer.WriteChar('*');
                }
                this.RenderRegister(reg.Name, renderer);
                break;
            case ImmediateOperand imm:
                renderer.WriteChar('$');
                RenderImmediate(imm, renderer);
                break;
            case MemoryOperand mem:
                if (mem.Width.Domain == Domain.SegPointer)
                {
                    renderer.WriteChar('*');
                }
                if (mem.SegOverride != RegisterStorage.None)
                {
                    RenderRegister(mem.SegOverride.Name, renderer);
                    renderer.WriteChar(':');
                }
                if (mem.Offset is not null && !mem.Offset.IsZero)
                {
                    string sOffset;
                    if (mem.Base == RegisterStorage.None && mem.Index == RegisterStorage.None)
                    {
                        sOffset = FormatUnsignedValue(mem.Offset.ToUInt64(), "{0:X}");
                    }
                    else
                    {
                        sOffset = FormatSignedValue(mem.Offset.ToInt64(), false);
                    }
                    renderer.WriteString(sOffset);
                }

                if (mem.Base != RegisterStorage.None)
                {
                    renderer.WriteChar('(');
                    RenderRegister(mem.Base.Name, renderer);
                }

                if (mem.Index != RegisterStorage.None)
                {
                    if (mem.Base == RegisterStorage.None)
                    {
                        renderer.WriteChar('(');
                    }
                    renderer.WriteChar(',');
                    RenderRegister(mem.Index.Name, renderer);
                    if (mem.Scale != 0)
                    {
                        renderer.WriteFormat(",{0}", mem.Scale);
                    }
                }
                renderer.WriteChar(')');
                break;

            case AddressOperand aop:
                var addr = aop.Address;
                if (options.SymbolResolver(addr, out string? name, out long symOffset))
                {
                    base.RenderSymbolReference(addr, name!, symOffset, renderer);
                }
                else if (addr.Selector.HasValue)
                {
                    switch (addr.DataType.BitSize)
                    {
                    case 32:
                        renderer.WriteFormat("$0x{0:X}, $0x{1:X}", 
                            addr.Selector.Value,
                            addr.Offset & 0xFFFF);
                        break;
                    case 48:
                        renderer.WriteFormat("$0x{0:X}, $0x{1:X}",
                            addr.Selector.Value,
                            addr.Offset);
                        break;
                    }
                }
                else
                {
                    renderer.WriteAddress(FormatUnsignedValue(aop.Address.Offset, "${0}"), addr);
                }
                break;
            default: throw new NotImplementedException($"Not implemeted operand type {operand.GetType().Name}");
            }
        }

        protected override void RenderRegister(string regName, MachineInstructionRenderer renderer)
        {
            renderer.WriteChar('%');
            base.RenderRegister(regName, renderer);
        }

        protected override void RenderOperands(X86Instruction instr, MachineInstructionRendererOptions options, MachineInstructionRenderer renderer)
        {
            if (instr.Mnemonic == Mnemonic.bound || instr.Mnemonic == Mnemonic.enter)
            {
                base.RenderOperands(instr, options, renderer);
            }
            else
            {
                int i = instr.Operands.Length - 1;
                RenderOperand(instr, i, renderer, options);
                for (--i; i >= 0; --i)
                {
                    renderer.WriteString(options.OperandSeparator ?? ",");
                    RenderOperand(instr, i, renderer, options);
                    if (i == 0 && instr.OpMask != 0)
                    {
                        renderer.WriteString("{k");
                        renderer.WriteUInt32(instr.OpMask);
                        renderer.WriteChar('}');
                    }
                }
            }
        }
    }
}

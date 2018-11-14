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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public partial class VaxDisassembler : DisassemblerBase<VaxInstruction>
    {
        private VaxArchitecture arch;
        private EndianImageReader rdr;

        public VaxDisassembler(VaxArchitecture arch, EndianImageReader imageReader)
        {
            this.arch = arch;
            this.rdr = imageReader;
        }

        public override VaxInstruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadByte(out byte op))
                return null;
            VaxInstruction instr;
            try
            {
                instr = oneByteInstructions[op].Decode(this);
            }
            catch
            {
                instr = new VaxInstruction {
                    Opcode = Opcode.Invalid,
                    IClass = InstrClass.Invalid,
                    Operands = new MachineOperand[0] };
            }
            if (instr == null)
                return null;
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        private VaxInstruction DecodeOperands(Opcode opcode, InstrClass iclass, string format)
        {
            var ops = new List<MachineOperand>();
            MachineOperand op;
            PrimitiveType width;
            int i = 0;
            while (i != format.Length)
            {
                switch (format[i++])
                {
                case ',':
                    continue;
                case 'a':
                    if (!TryDecodeOperand(Width(format[i++]), out op))
                        return null;
                    break;
                case 'w':
                case 'm':
                    if (!TryDecodeOperand(Width(format[i++]), out op))
                        return null;
                    if (op is ImmediateOperand)
                        op = null;    // Can't modify a constant! 
                    break;
                case 'r':
                case 'v':
                    if (!TryDecodeOperand(Width(format[i++]), out op))
                        return null;
                    break;
                case 'b':
                    width = Width(format[i++]);
                    long jOffset = rdr.ReadLeSigned(width);
                    ulong uAddr = (uint)((long)rdr.Address.Offset + jOffset);
                    op = AddressOperand.Ptr32((uint)uAddr);
                    break;
                default:
                    throw new NotImplementedException(
                        string.Format(
                            "Access type {0} not implemented.", format[i - 1]));
                }
                if (op == null)
                {
                    return new VaxInstruction
                    {
                        Opcode = Opcode.Invalid,
                        IClass = InstrClass.Invalid,
                        Operands = new MachineOperand[0],
                    };
                }
                ops.Add(op);
            }
            return new VaxInstruction
            {
                Opcode = opcode,
                IClass = iclass,
                Operands = ops.ToArray()
            };
        }

        private PrimitiveType Width(char w)
        {
            switch (w)
            {
            case 'b': return PrimitiveType.Byte;
            case 'w': return PrimitiveType.Word16;
            case 'l': return PrimitiveType.Word32;
            case 'f': return PrimitiveType.Real32;  //$TODO: this is not IEEE
            case 'd': return PrimitiveType.Real64;  //$TODO: this is not IEEE
            case 'g': return PrimitiveType.Real64;  //$TODO: this is not IEEE
            case 'h': return PrimitiveType.Real128;  //$TODO: this is not IEEE
            case 'q': return PrimitiveType.Word64;
            default:
                throw new NotImplementedException(
                    string.Format(
                        "Data width '{0}' not implemented.",
                        w));
            }
        }

        private bool TryDecodeOperand(PrimitiveType width, out MachineOperand op)
        {
            op = null;
            if (!rdr.TryReadByte(out byte bSpecifier))
            {
                return false;
            }
            var reg = arch.GetRegister(bSpecifier & 0xF);
            switch (bSpecifier >> 4)
            {
            case 0: // Literal mode
            case 1:
            case 2:
            case 3:
                op = LiteralOperand(width, bSpecifier);
                break;
            case 4: // Index mode
                op = IndexOperand(width, reg);
                break;
            case 5: // Register mode
                op = new RegisterOperand(reg);
                break;
            case 6: // Register deferred
                op = new MemoryOperand(width)
                {
                    Base = reg
                };
                break;
            case 7: // Autodecrement mode
                op = new MemoryOperand(width)
                {
                    Base = reg,
                    AutoDecrement = true,
                };
                break;
            case 8: // Autoincrement mode
                if (reg.Number == 0x0F)
                {
                    op = ImmediateOperand(width);
                }
                else
                {
                    op = new MemoryOperand(width)
                    {
                        Base = reg,
                        AutoIncrement = true,
                    };
                }
                break;
            case 9: // Deferred Autoincrement mode
                op = new MemoryOperand(width)
                {
                    Base = reg,
                    AutoIncrement = true,
                    Deferred = true,
                };
                break;
            case 0xA: // Displacement mode
            case 0xD:
                if (!rdr.TryReadByte(out byte b))
                    return false;
                op = DisplacementOperand(width, reg, Constant.SByte((sbyte)b), bSpecifier);
                break;
            case 0xB:
            case 0xE:
                if (!rdr.TryReadUInt16(out ushort w))
                    return false;
                op = DisplacementOperand(width, reg, Constant.Int16((short)w), bSpecifier);
                break;
            case 0xC:
            case 0xF:
                if (!rdr.TryReadUInt32(out uint dw))
                    return false;
                op = DisplacementOperand(width, reg, Constant.Word32(dw), bSpecifier);
                break;
            default:
                throw new InvalidCastException("Impossiburu!");
            }
            return true;
        }

        private MachineOperand ImmediateOperand(PrimitiveType width)
        {
            if (!rdr.TryRead(width, out Constant imm))
                return null;
            return new ImmediateOperand(imm);
        }

        private MachineOperand IndexOperand(PrimitiveType width, RegisterStorage reg)
        {
            if (!TryDecodeOperand(width, out MachineOperand op))
                return null;
            if (!(op is MemoryOperand aOp))
                return null;
            aOp.Index = reg;
            return aOp;
        }

        private MachineOperand DisplacementOperand(PrimitiveType width, RegisterStorage reg, Constant c, byte bSpecifier)
        {
            if (reg.Number == 15)
            {
                return AddressOperand.Ptr32(
                    (uint)((int)rdr.Address.ToLinear() + c.ToInt32()));
            }
            return new MemoryOperand(width)
            {
                Base = reg,
                Offset = c,
                Deferred = (bSpecifier >> 4) > 0xC
            };
        }

        public static MachineOperand LiteralOperand(PrimitiveType width, byte b)
        {
            Constant c;
            if (width.Domain == Domain.Real)
            {
                float exp = 1 << ((b >> 3) & 7);
                float frac = 8 | (b & 7);
                if (width.BitSize == 32)
                {
                    c = Constant.Real32(frac * exp / 16.0F);
                }
                else if (width.BitSize == 64)
                {
                    c = Constant.Real64(frac * exp / 16.0F);
                }
                else
                    throw new NotImplementedException();
            }
            else
            {
                c = Constant.Create(width, b & 0x3F);
            }
            return new ImmediateOperand(c);
        }

        private RegisterStorage GetReg(int v)
        {
            throw new NotImplementedException();
        }

        public class Decoder
        {
            private readonly Opcode op;
            private readonly InstrClass iclass;
            private readonly string format;

            public Decoder(Opcode op, string format, InstrClass iclass = InstrClass.Linear)
            {
                this.op = op;
                this.iclass = iclass;
                this.format = format;
            }

            public Decoder(Opcode op, int args)
            {
                this.op = op;
                this.format = "";
            }

            public virtual VaxInstruction Decode(VaxDisassembler dasm)
            {
                return dasm.DecodeOperands(op, iclass, format);
            }
        }
    }
}

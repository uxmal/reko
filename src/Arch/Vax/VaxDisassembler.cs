#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
        private ImageReader rdr;

        public VaxDisassembler(VaxArchitecture arch, ImageReader imageReader)
        {
            this.arch = arch;
            this.rdr = imageReader;
        }

        public override VaxInstruction DisassembleInstruction()
        {
            byte op;
            var addr = rdr.Address;
            if (!rdr.TryReadByte(out op))
                return null;
            VaxInstruction instr = oneByteInstructions[op].Decode(this);
            if (instr == null)
                return null;
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        private VaxInstruction DecodeOperands(Opcode opcode, string format)
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
                case 'r':
                case 'w':
                    if (!TryDecodeOperand(Width(format[i++]), out op))
                        return null;
                    break;
                case 'b':
                    width = Width(format[i++]);
                    long jOffset = rdr.ReadLeSigned(width);
                    ulong uAddr = (uint)((long)rdr.Address.Offset + jOffset);
                    op = AddressOperand.Ptr32((uint)uAddr);
                    break;
                default: throw new NotImplementedException(
                    string.Format(
                        "Access type {0} not implemented.", format[i - 1]));
                }
                ops.Add(op);
            }
            return new VaxInstruction
            {
                Opcode = opcode,
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
            byte b;
            byte bSpecifier;
            ushort w;
            uint dw;
            if (!rdr.TryReadByte(out bSpecifier))
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
            case 5: // Register mode
                op = new RegisterOperand(reg);
                break;
            case 7: // Autodecrement mode
                op = new MemoryOperand(width)
                {
                    Base = reg,
                    AutoDecrement = true,
                };
                break;
            case 8: // Autoincrement mode
                op = new MemoryOperand(width)
                {
                    Base = reg,
                    AutoIncrement = true,
                };
                break;
            case 9: // Deferred Autoincrement mode
                op = new MemoryOperand(width)
                {
                    Base = reg,
                    AutoIncrement = true,
                    Deferred = true,
                };
                break;
            case 10: // Displacement mode
            case 11:
                if (!rdr.TryReadByte(out b))
                    return false;
                op = DisplacementOperand(width, reg, Constant.SByte((sbyte)b), bSpecifier);
                break;
            case 12:
            case 13:
                if (!rdr.TryReadUInt16(out w))
                    return false;
                op = DisplacementOperand(width, reg, Constant.Int16((short)w), bSpecifier);
                break;
            case 14:
            case 15:
                if (!rdr.TryReadUInt32(out dw))
                    return false;
                op = DisplacementOperand(width, reg, Constant.Word32(dw), bSpecifier);
                break;
            default:
                throw new NotImplementedException(
                    string.Format(
                        "Unimplemented addressing mode {0:X2}", (bSpecifier >> 4)));
            }
            return true;
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
                Deferred = (bSpecifier & 1) == 1
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

        public class OpRec
        {
            private Opcode op;
            private string format;

            public OpRec(Opcode op, string format)
            {
                this.op = op;
                this.format = format;
            }

            public OpRec(Opcode op, int args)
            {

            }
            public virtual VaxInstruction Decode(VaxDisassembler dasm)
            {
                return dasm.DecodeOperands(op, format);
            }
        }

        
    }
}

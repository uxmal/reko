#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Threading.Tasks;
using Reko.Core.Types;

namespace Reko.Arch.SuperH
{
    public class SuperHDisassembler : DisassemblerBase<SuperHInstruction>
    {
        private EndianImageReader rdr;

        public SuperHDisassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override SuperHInstruction DisassembleInstruction()
        {
            ushort uInstr;
            var addr = rdr.Address;
            if (!rdr.TryReadUInt16(out uInstr))
                return null;
            var instr = oprecs[uInstr >> 12].Decode(this, uInstr);
            instr.Address = addr;
            instr.Length = 2;
            return instr;
        }

        private SuperHInstruction Decode(ushort uInstr, Opcode opcode, string format)
        {
            var ops = new MachineOperand[2];
            int iop = 0;
            for (int i = 0; i < format.Length; ++i)
            {
                switch (format[i])
                {
                case ',':
                    continue;
                case 'r':
                    ops[iop] = Register(format[++i], uInstr);
                    break;
                case 'I':
                    ops[iop] = ImmediateOperand.Byte((byte)uInstr);
                    break;
                case 'G':
                    ops[iop] = MemoryOperand.GbrIndexedIndirect(GetWidth(format[++i]));
                    break;
                case 'R':
                    ops[iop] = new RegisterOperand(Registers.gpregs[HexDigit(format[++i]).Value]);
                    break;
                case 'j':
                    ops[iop] = AddressOperand.Create(rdr.Address + (2 + 2 * (sbyte)uInstr));
                    break;
                case 'J':
                    int offset = ((int)uInstr << 20) >> 19;
                    ops[iop] = AddressOperand.Create(rdr.Address + (2 + offset));
                    break;

                default: throw new NotImplementedException(string.Format("SuperHDisassembler.Decode({0})", format[i]));
                }
                ++iop;
            }
            return new SuperHInstruction
            {
                Opcode = opcode,
                op1 = ops[0],
                op2 = ops[1],
            };
        }

        private PrimitiveType GetWidth(char w)
        {
            if (w == 'b')
                return PrimitiveType.Byte;
            else if (w == 'w')
                return PrimitiveType.Word16;
            else if (w == 'l')
                return PrimitiveType.Word32;
            else 
                throw new NotImplementedException();
        }

        public static uint? HexDigit(char digit)
        {
            switch (digit)
            {
            case '0': case '1': case '2': case '3': case '4': 
            case '5': case '6': case '7': case '8': case '9':
                return (uint) (digit - '0');
            case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                return (uint) ((digit - 'A') + 10);
            case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
                return (uint) ((digit - 'a') + 10);
            default:
                return null;
            }
        }
        private RegisterOperand Register(char r, ushort uInstr)
        {
            var reg = (uInstr >> 4 * ('3' - r)) & 0xF;
            return new RegisterOperand(Registers.gpregs[reg]);
        }

        private abstract class OprecBase
        {
            public abstract SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr);
        }

        private class Oprec : OprecBase
        {
            private Opcode opcode;
            private string format;

            public Oprec(Opcode op, string format)
            {
                this.opcode = op;
                this.format = format;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                return dasm.Decode(uInstr, opcode, format);
            }
        }

        private class Oprec4Bits : OprecBase
        {
            private int shift;
            private OprecBase[] oprecs;

            public Oprec4Bits(int shift, OprecBase[] oprecs)
            {
                this.shift = shift;
                this.oprecs = oprecs;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                return oprecs[(uInstr >> shift) & 0xF].Decode(dasm, uInstr);
            }
        }

        private class Oprec8Bits : OprecBase
        {
            private int shift;
            private Dictionary<int, OprecBase> oprecs;

            public Oprec8Bits(int shift, Dictionary<int, OprecBase> oprecs)
            {
                this.shift = shift;
                this.oprecs = oprecs;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                OprecBase or;
                if (!oprecs.TryGetValue((uInstr >> shift) & 0xFF, out or))
                    return dasm.Decode(uInstr, Opcode.invalid, "");
                return or.Decode(dasm, uInstr);
            }
        }

        private static OprecBase[] oprecs = new OprecBase[]
        {
            new Oprec8Bits(0, new Dictionary<int, OprecBase>
            {
                { 0x23, new Oprec(Opcode.braf, "r1") },
                { 0x3B, new Oprec(Opcode.brk, "") },
            }),
            new Oprec(Opcode.invalid, ""),
            new Oprec4Bits(0, new OprecBase[]
            {
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.and, "r2,r1"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
            }),
            new Oprec4Bits(0, new OprecBase[]
            {
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.add, "r2,r1"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.addc, "r2,r1"),
                new Oprec(Opcode.addv, "r2,r1"),
            }),

            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.add, "I,r1"),

            // 08
            new Oprec4Bits(8, new OprecBase[] {
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.bf, "j"),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.bf_s, "j"),
            }),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.bra, "J"),
            new Oprec(Opcode.bsr, "J"),

            // 0C
            new Oprec4Bits(8, new OprecBase[]
            {
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.and, "I,R0"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.and_b, "I,Gb"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
            }),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),


        };
    }
}

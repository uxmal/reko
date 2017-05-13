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


        private class OprecLow4bits : OprecBase
        {
            private OprecBase[] oprecs;

            public OprecLow4bits(OprecBase[] oprecs)
            {
                this.oprecs = oprecs;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                return oprecs[uInstr & 0xF].Decode(dasm, uInstr);
            }
        }


        private static OprecBase[] oprecs = new OprecBase[]
        {
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new OprecLow4bits(new OprecBase[]
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
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
            }),

            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.add, "I,r1"),

            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),

            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),
            new Oprec(Opcode.invalid, ""),


        };
    }
}

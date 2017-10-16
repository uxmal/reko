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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.MSP430
{
    public class Msp430Disassembler : DisassemblerBase<Msp430Instruction>
    {
        private EndianImageReader rdr;
        private Msp430Architecture arch;

        public Msp430Disassembler(Msp430Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Msp430Instruction DisassembleInstruction()
        {
            ushort uInstr;
            var addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out uInstr))
                return null;
            var instr = s_decoders[uInstr >> 12].Decode(this, uInstr);
            if (instr != null)
            {
                instr.Address = addr;
                instr.Length = (int)(rdr.Address - addr);
            }
            return instr;
        }

        private Msp430Instruction Decode(uint uInstr, Opcode opcode, string fmt)
        {
            PrimitiveType dataWidth = null;
            int i = 0;
            if (fmt[i] == 'w')      // use width bit
            {
                dataWidth = (uInstr & 0x40) != 0 ? PrimitiveType.Byte : PrimitiveType.Word16;
                ++i;
            }
            MachineOperand op1 = null;
            MachineOperand op2 = null;
            if (fmt[i] == 'J')
            {
                int offset = (short) (uInstr << 6) >> 5;
                op1 = AddressOperand.Create(rdr.Address + offset);
            }
            else if (fmt[i] == 'S')
            {
                var aS = (uInstr >> 4) & 0x03;
                var iReg = (uInstr >> 8) & 0x0F;
                if (iReg == 2)
                {
                    throw new NotImplementedException();
                }
                else if (iReg == 3)
                {
                    throw new NotImplementedException();

                }
                else
                {
                    var reg = Registers.GpRegisters[iReg];
                    switch (aS)
                    {
                    case 0: op1 = new RegisterOperand(reg); break;
                    case 1:
                        op1 = Indexed(reg, dataWidth);
                        if (op1 == null)
                            return null;
                        break;
                    case 2:
                        op1 = new MemoryOperand(dataWidth)
                        {
                            Base = reg
                        };
                        break;
                    default:
                        throw new NotImplementedException();
                    }
                }
                ++i;
            }
            if (fmt[i] == 'D')
            {
                var aD = (uInstr >> 7) & 0x01;
                var iReg = uInstr & 0x0F;
                if (iReg == 2 || iReg == 3)
                {
                    return new Msp430Instruction { opcode = Opcode.invalid };
                }
                var reg = Registers.GpRegisters[iReg];
                if (aD == 0)
                {
                    op2 = new RegisterOperand(reg);
                }
                else
                {
                    op2 = Indexed(reg, dataWidth);
                    if (op2 == null)
                        return null;
                }
            }
            return new Msp430Instruction
            {
                opcode = opcode,
                dataWidth = dataWidth,
                op1 = op1,
                op2 = op2,
            };
        }

        private MemoryOperand Indexed(RegisterStorage reg, PrimitiveType dataWidth)
        {
            short offset;
            if (!rdr.TryReadLeInt16(out offset))
                return null;
            return new MemoryOperand(dataWidth)
            {
                Base = reg,
                Offset = offset
            };
        }

        private abstract class OpRecBase
        {
            public abstract Msp430Instruction Decode(Msp430Disassembler dasm, ushort uInstr);
        }

        private class OpRec : OpRecBase
        {
            private string fmt;
            private Opcode opcode;

            public OpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override Msp430Instruction Decode(Msp430Disassembler dasm, ushort uInstr)
            {
                return dasm.Decode(uInstr, opcode, fmt);
            }
        }

        private class JmpOpRec : OpRecBase
        {
            public override Msp430Instruction Decode(Msp430Disassembler dasm, ushort uInstr)
            {
                return dasm.Decode(uInstr, jmps[(uInstr >> 10) & 7], "J");
            }
        }

        private static OpRecBase[] s_decoders = new OpRecBase[16]
        {
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new JmpOpRec(),
            new JmpOpRec(),

            new OpRec(Opcode.mov, "wSD"),
            new OpRec(Opcode.add, "wSD"),
            new OpRec(Opcode.addc, "wSD"),
            new OpRec(Opcode.subc, "wSD"),

            new OpRec(Opcode.sub, "wSD"),
            new OpRec(Opcode.cmp, "wSD"),
            new OpRec(Opcode.dadd, "wSD"),
            new OpRec(Opcode.bit, "wSD"),

            new OpRec(Opcode.bic, "wSD"),
            new OpRec(Opcode.bis, "wSD"),
            new OpRec(Opcode.xor, "wSD"),
            new OpRec(Opcode.and, "wSD"),
        };

        private static Opcode[] jmps = new Opcode[8]
        {
            Opcode.jnz,
            Opcode.jz,
            Opcode.jnc,
            Opcode.jc,
            Opcode.jn,
            Opcode.jge,
            Opcode.jl,
            Opcode.jmp,
        };
    }
}

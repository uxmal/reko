#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    public class Pdp11Disassembler : IDisassembler
    {
        private ImageReader rdr;
        private Pdp11Architecture arch;
        private PrimitiveType dataWidth;

        public Pdp11Disassembler(ImageReader rdr, Pdp11Architecture arch)
        {
            this.rdr = rdr;
            this.arch = arch;
        }

        public Address Address
        {
            get { return rdr.Address; }
        }

        public MachineInstruction DisassembleInstruction()
        {
            ushort opcode = rdr.ReadLeUInt16();
            dataWidth = DataWidthFromSizeBit(opcode & 0x8000u);
            switch ((opcode >> 0x0C) & 7)
            {
            case 0: return NonDoubleOperandInstruction(opcode);
            case 1:
                return new Pdp11Instruction
                {
                    Opcode = Opcodes.mov,
                    DataWidth = dataWidth,
                    op1 = DecodeOperand(opcode),
                    op2 = DecodeOperand(opcode >> 6)
                };
            case 2:
                return new Pdp11Instruction
                {
                    Opcode = Opcodes.cmp,
                    DataWidth = dataWidth,
                    op1 = DecodeOperand(opcode),
                    op2 = DecodeOperand(opcode >> 6),
                };
            case 3:
                return new Pdp11Instruction
                {
                    Opcode = Opcodes.bit,
                    DataWidth = dataWidth,
                    op1 = DecodeOperand(opcode),
                    op2 = DecodeOperand(opcode >> 6),
                };
            case 4:
                return new Pdp11Instruction
                {
                    Opcode = Opcodes.bic,
                    DataWidth = dataWidth,
                    op1 = DecodeOperand(opcode),
                    op2 = DecodeOperand(opcode >> 6),
                };
            case 5:
                return new Pdp11Instruction
                {
                    Opcode = Opcodes.bis,
                    DataWidth = dataWidth,
                    op1 = DecodeOperand(opcode),
                    op2 = DecodeOperand(opcode >> 6),
                };
            case 6:
                return new Pdp11Instruction
                {
                    Opcode = (opcode & 0x8000u) != 0 ? Opcodes.sub : Opcodes.add,
                    DataWidth = dataWidth,
                    op1 = DecodeOperand(opcode),
                    op2 = DecodeOperand(opcode >> 6),
                };
            case 7:
                switch ((opcode >> 0x09) & 7)
                {
                case 0:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcodes.mul,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                case 1:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcodes.div,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                case 4:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcodes.xor,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                }
                throw new NotSupportedException();
            default:
                throw new NotSupportedException();
            }
            throw new NotImplementedException();
        }

        private PrimitiveType DataWidthFromSizeBit(uint p)
        {
            return p != 0 ? PrimitiveType.Byte : PrimitiveType.Word16;
        }

        private MachineInstruction NonDoubleOperandInstruction(ushort opcode)
        {
            var dataWidth = DataWidthFromSizeBit(opcode & 0x8000u);
            var op = DecodeOperand(opcode);
            Opcodes oc = Opcodes.illegal;
            switch ((opcode >> 6) & 0x3FF)
            {
            case 0x003:
                oc = Opcodes.swab;
                dataWidth = PrimitiveType.Byte;
                break;

            case 0x203:
                return BranchInstruction(opcode);
            case 0x028:
            case 0x228:
                oc = Opcodes.clr;
                break;
            case 0x029:
            case 0x229:
                oc = Opcodes.com;
                break;
            case 0x02A:
            case 0x22A:
                oc = Opcodes.inc;
                break;
            case 0x02B:
            case 0x22B:
                oc = Opcodes.dec;
                break;
            case 0x02C:
            case 0x22C:
                oc = Opcodes.neg;
                break;
            case 0x02D:
            case 0x22D:
                oc = Opcodes.adc;
                break;
            case 0x02E:
            case 0x22E:
                oc = Opcodes.sbc;
                break;
            case 0x02F:
            case 0x22F:
                oc = Opcodes.tst;
                break;
            case 0x030:
            case 0x230:
                oc = Opcodes.ror;
                break;
            case 0x031:
            case 0x231:
                oc = Opcodes.rol;
                break;
            case 0x032:
            case 0x232:
                oc = Opcodes.asr;
                break;
            case 0x033:
            case 0x233:
                oc = Opcodes.asl;
                break;
            case 0x034:
                oc = Opcodes.mark;
                break;
            case 0x234:
                oc = Opcodes.mtps;
                break;
            case 0x035:
                oc = Opcodes.mfpi;
                break;
            case 0x235:
                oc = Opcodes.mfpd;
                break;
            case 0x036:
                oc = Opcodes.mtpi;
                break;
            case 0x236:
                oc = Opcodes.mtpd;
                break;
            case 0x037:
                oc = Opcodes.sxt;
                break;
            case 0x237:
                oc = Opcodes.mfps;
                break;
            }
            return new Pdp11Instruction
            {
                Opcode = oc,
                DataWidth = dataWidth,
                op1 = op,
            };
        }

        private MachineInstruction BranchInstruction(ushort opcode)
        {
            var oc = Opcodes.illegal;
            switch ((opcode >> 8) | (opcode >> 15))
            {
            case 1: oc = Opcodes.br; break;
            case 2: oc = Opcodes.bne; break;
            case 3: oc = Opcodes.beq; break;
            case 4: oc = Opcodes.bge; break;
            case 5: oc = Opcodes.blt; break;
            case 6: oc = Opcodes.bgt; break;
            case 7: oc = Opcodes.ble; break;

            case 8: oc = Opcodes.bpl; break;
            case 9: oc = Opcodes.bmi; break;
            case 0xA: oc = Opcodes.bhi; break;
            case 0xB: oc = Opcodes.blos; break;
            case 0xC: oc = Opcodes.bvc; break;
            case 0xD: oc = Opcodes.bvs; break;
            case 0xE: oc = Opcodes.bcc; break;
            case 0xF: oc = Opcodes.bcs; break;
            }
            return new Pdp11Instruction
            {
                Opcode = oc,
                DataWidth = PrimitiveType.Word16,
                op1 = new AddressOperand(rdr.Address + (sbyte)(opcode & 0xFF)),
            };
        }

        private MachineOperand DecodeOperand(int operandBits)
        {
            var reg = arch.GetRegister(operandBits & 7);
            if (reg == Registers.pc)
            {
                throw new NotImplementedException();
            }
            else
            {
                switch ((operandBits >> 3) & 7)
                {
                case 0: return new RegisterOperand(reg);    //   Reg           Direct addressing of the register
                case 1: return new MemoryOperand(AddressMode.RegDef, dataWidth, reg);      //   Reg Def       Contents of Reg is the address
                case 2: return new MemoryOperand(AddressMode.AutoIncr, dataWidth, reg);   //   AutoIncr      Contents of Reg is the address, then Reg incremented
                //case 3:    //   AutoIncrDef   Content of Reg is addr of addr, then Reg Incremented
                //case 4:    //   AutoDecr      Reg is decremented then contents is address
                //case 5:    //   AutoDecrDef   Reg is decremented then contents is addr of addr
                //case 6: return new MemoryOperand(reg, rdr.ReadLeUInt16());   //   Index         Contents of Reg + Following word is address
                //case 7:   //   IndexDef      Contents of Reg + Following word is addr of addr
                default: throw new NotSupportedException(string.Format("Address mode {0} not supported.", (operandBits >> 3) & 7));
                }
            }
        }

        private static OpRec[] oprecs = new OpRec[] 
        { 
            null,

        };
    }
    public class OpRec { }

}

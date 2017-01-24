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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.Pdp11
{
    public class Pdp11Disassembler : DisassemblerBase<Pdp11Instruction>
    {
        private Pdp11Architecture arch;
        private ImageReader rdr;
        private Pdp11Instruction instrCur;
        private PrimitiveType dataWidth;

        public Pdp11Disassembler(ImageReader rdr, Pdp11Architecture arch)
        {
            this.rdr = rdr;
            this.arch = arch;
        }

        public override Pdp11Instruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;
            try
            {
                instrCur = Disassemble();
            } catch (AddressCorrelatedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addr, "{0}", ex.Message);
            }
            instrCur.Address = addr;
            instrCur.Length = (int)(rdr.Address - addr);
            return instrCur;
        }

        private Pdp11Instruction DecodeOperands(ushort wOpcode, Opcode opcode, string fmt)
        {
            List<MachineOperand> ops = new List<MachineOperand>(2);
            int i = 0;
            dataWidth = PrimitiveType.Word16;
            switch (fmt[i])
            {
            case 'b': dataWidth = PrimitiveType.Byte; i += 2; break;
            case 'w': dataWidth = PrimitiveType.Word16; i += 2; break;
            }
            while (i != fmt.Length)
            {
                if (fmt[i] == ',')
                    ++i;
                switch (fmt[i++])
                {
                case 'E': ops.Add(DecodeOperand(wOpcode)); break;
                case 'e': ops.Add(DecodeOperand(wOpcode >> 6)); break;
                default: throw new NotImplementedException();
                }
            }
            var instr = new Pdp11Instruction
            {
                Opcode = opcode,
                DataWidth = dataWidth,
                op1 = ops.Count > 0 ? ops[0] : null,
                op2 = ops.Count > 1 ? ops[1] : null,
            };
            return instr;
        }

        abstract class OpRec
        {
            public abstract Pdp11Instruction Decode(ushort opcode, Pdp11Disassembler dasm);
        }

        class FormatOpRec : OpRec
        {
            private string fmt;
            private Opcode opcode;

            public FormatOpRec(string fmt, Opcode op)
            {
                this.fmt = fmt;
                this.opcode = op;
            }

            public override Pdp11Instruction Decode(ushort opcode, Pdp11Disassembler dasm)
            {
                return dasm.DecodeOperands(opcode, this.opcode, fmt);
            }
        }

        class FnOpRec : OpRec
        {
            private Func<ushort, Pdp11Disassembler, Pdp11Instruction> fn;

            public FnOpRec(Func<ushort, Pdp11Disassembler, Pdp11Instruction> fn)
            {
                this.fn = fn;
            }

            public override Pdp11Instruction Decode(ushort opcode, Pdp11Disassembler dasm)
            {
                return fn(opcode, dasm);
            }
        }

        private static OpRec[] decoders;

        static Pdp11Disassembler()
        {
            decoders = new OpRec[] {
                null,
                new FormatOpRec("w:e,E", Opcode.mov),
                new FormatOpRec("e,E", Opcode.cmp),
                new FormatOpRec("e,E", Opcode.bit),
                new FormatOpRec("e,E", Opcode.bic),
                new FormatOpRec("e,E", Opcode.bis),
                new FormatOpRec("w:e,E", Opcode.add),
                null,

                null,
                new FormatOpRec("b:e,E", Opcode.movb),
                new FormatOpRec("e,E", Opcode.cmp),
                new FormatOpRec("e,E", Opcode.bit),
                new FormatOpRec("e,E", Opcode.bic),
                new FormatOpRec("e,E", Opcode.bis),
                new FormatOpRec("w:e,E", Opcode.sub),
                null,
            };
        }

        private Pdp11Instruction Disassemble()
        {
            ushort opcode = rdr.ReadLeUInt16();
            dataWidth = DataWidthFromSizeBit(opcode & 0x8000u);
            var decoder = decoders[(opcode >> 0x0C) & 0x00F];
            if (decoder != null)
                return decoder.Decode(opcode, this);

            switch ((opcode >> 0x0C) & 0x007)
            {
            case 0: return NonDoubleOperandInstruction(opcode);
            case 7:
                switch ((opcode >> 0x09) & 7)
                {
                case 0:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcode.mul,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                case 1:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcode.div,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                case 2:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcode.ash,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                case 3:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcode.ashc,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                case 4:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcode.xor,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                case 5:
                    return FpuArithmetic(opcode);
                case 7:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcode.sob,
                        DataWidth = dataWidth,
                        op1 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                        op2 = Imm6(opcode),
                    };
                }
                throw new NotSupportedException();
            default:
                throw new NotSupportedException();
            }
            throw new NotImplementedException();
        }

        private MachineOperand Imm6(ushort opcode)
        {
            var offset = (opcode & 0x3F) << 1;
            return new AddressOperand(rdr.Address - offset);
        }

        private Pdp11Instruction FpuArithmetic(ushort opcode)
        {
            return new Pdp11Instruction { Opcode = Opcode.nop };
        }

        private PrimitiveType DataWidthFromSizeBit(uint p)
        {
            return p != 0 ? PrimitiveType.Byte : PrimitiveType.Word16;
        }

        private Pdp11Instruction NonDoubleOperandInstruction(ushort opcode)
        {
            switch ((opcode >> 8))
            {
            case 0x01: return BranchInstruction(opcode, Opcode.br);
            case 0x02: return BranchInstruction(opcode, Opcode.bne);
            case 0x03: return BranchInstruction(opcode, Opcode.beq);
            case 0x04: return BranchInstruction(opcode, Opcode.bge);
            case 0x05: return BranchInstruction(opcode, Opcode.blt);
            case 0x06: return BranchInstruction(opcode, Opcode.bgt);
            case 0x07: return BranchInstruction(opcode, Opcode.ble);
            case 0x80: return BranchInstruction(opcode, Opcode.bpl);
            case 0x81: return BranchInstruction(opcode, Opcode.bmi);
            case 0x82: return BranchInstruction(opcode, Opcode.bhi);
            case 0x83: return BranchInstruction(opcode, Opcode.blos);
            case 0x84: return BranchInstruction(opcode, Opcode.bvc);
            case 0x85: return BranchInstruction(opcode, Opcode.bvs);
            case 0x86: return BranchInstruction(opcode, Opcode.bcc);
            case 0x87: return BranchInstruction(opcode, Opcode.bcs);
            }


            var dataWidth = DataWidthFromSizeBit(opcode & 0x8000u);
            MachineOperand op = null;
            Opcode oc = Opcode.illegal;
            switch ((opcode >> 6) & 0x3FF)
            {
            case 0x000:
                switch (opcode & 0x3F)
                {
                case 0x00: op = null; oc = Opcode.halt; break;
                case 0x01: op = null; oc = Opcode.wait; break;
                case 0x02: op = null; oc = Opcode.rti; break;
                case 0x03: op = null; oc = Opcode.bpt; break;
                case 0x04: op = null; oc = Opcode.iot; break;
                case 0x05: op = null; oc = Opcode.reset; break;
                case 0x06: op = null; oc = Opcode.rtt; break;
                case 0x07: op = null; oc = Opcode.illegal; break;
                }
                break;
            case 0x001: op = DecodeOperand(opcode); oc = Opcode.jmp; break;
            case 0x002:
                switch (opcode & 0x38)
                {
                case 0: op = DecodeOperand(opcode & 7); oc = Opcode.rts; break;
                case 3: op = DecodeOperand(opcode); oc = Opcode.spl; break;
                case 0x20:
                case 0x30:
                    return DecodeCondCode(opcode);
                }
                break;
            case 0x003:
                oc = Opcode.swab; op = DecodeOperand(opcode);
                dataWidth = PrimitiveType.Byte;
                break;
            case 0x020:
            case 0x021:
            case 0x022:
            case 0x023:
            case 0x024:
            case 0x025:
            case 0x026:
            case 0x027:
                return new Pdp11Instruction
                {
                    Opcode = Opcode.jsr,
                    op1 = Reg(opcode >> 6),
                    op2 = DecodeOperand(opcode),
                    DataWidth = PrimitiveType.Word16
                };
            case 0x220:
            case 0x221:
            case 0x222:
            case 0x223:
                oc = Opcode.emt;
                op = new ImmediateOperand(Constant.Byte((byte)opcode));
                break;
            case 0x224:
            case 0x225:
            case 0x226:
            case 0x227:
                oc = Opcode.trap;
                op = new ImmediateOperand(Constant.Byte((byte)opcode));
                break;
            case 0x028:
            case 0x228:
                oc = dataWidth.Size == 1 ? Opcode.clrb : Opcode.clr; op = DecodeOperand(opcode);
                break;
            case 0x029:
            case 0x229:
                oc = Opcode.com; op = DecodeOperand(opcode);
                break;
            case 0x02A:
            case 0x22A:
                oc = Opcode.inc; op = DecodeOperand(opcode);
                break;
            case 0x02B:
            case 0x22B:
                oc = Opcode.dec; op = DecodeOperand(opcode);
                break;
            case 0x02C:
            case 0x22C:
                oc = Opcode.neg; op = DecodeOperand(opcode);
                break;
            case 0x02D:
            case 0x22D:
                oc = Opcode.adc; op = DecodeOperand(opcode);
                break;
            case 0x02E:
            case 0x22E:
                oc = Opcode.sbc; op = DecodeOperand(opcode);
                break;
            case 0x02F:
            case 0x22F:
                oc = Opcode.tst; op = DecodeOperand(opcode);
                break;
            case 0x030:
            case 0x230:
                oc = Opcode.ror; op = DecodeOperand(opcode);
                break;
            case 0x031:
            case 0x231:
                oc = Opcode.rol; op = DecodeOperand(opcode);
                break;
            case 0x032:
            case 0x232:
                oc = Opcode.asr; op = DecodeOperand(opcode);
                break;
            case 0x033:
            case 0x233:
                oc = Opcode.asl; op = DecodeOperand(opcode);
                break;
            case 0x034:
                oc = Opcode.mark; op = DecodeOperand(opcode);
                break;
            case 0x234:
                oc = Opcode.mtps; op = DecodeOperand(opcode);
                break;
            case 0x035:
                oc = Opcode.mfpi; op = DecodeOperand(opcode);
                break;
            case 0x235:
                oc = Opcode.mfpd; op = DecodeOperand(opcode);
                break;
            case 0x036:
                oc = Opcode.mtpi; op = DecodeOperand(opcode);
                break;
            case 0x236:
                oc = Opcode.mtpd; op = DecodeOperand(opcode);
                break;
            case 0x037:
                oc = Opcode.sxt; op = DecodeOperand(opcode);
                break;
            case 0x237:
                oc = Opcode.mfps; op = DecodeOperand(opcode);
                break;
            }
            return new Pdp11Instruction
            {
                Opcode = oc,
                DataWidth = dataWidth,
                op1 = op,
            };
        }

        private Pdp11Instruction DecodeCondCode(ushort opcode)
        {
            if ((opcode & 0x1F) == 0)
            {
                return new Pdp11Instruction
                {
                    Opcode = Opcode.nop,
                };
            } 
            return new Pdp11Instruction
            {
                Opcode = ((opcode & 0x10) != 0) ? Opcode.setflags : Opcode.clrflags,
                DataWidth = dataWidth,
                op1 = new ImmediateOperand(Constant.Byte((byte)(opcode&0xF))),
            };
        }

        private MachineOperand Reg(int bits)
        {
            return new RegisterOperand(arch.GetRegister(bits & 7));
        }

        private MachineOperand Imm3(ushort opcode)
        {
            throw new NotImplementedException();
        }

        private Pdp11Instruction BranchInstruction(ushort opcode, Opcode oc)
        {
            return new Pdp11Instruction
            {
                Opcode = oc,
                DataWidth = PrimitiveType.Word16,
                op1 = new AddressOperand(rdr.Address + 2 * (sbyte)(opcode & 0xFF)),
            };
        }

        private MachineOperand DecodeOperand(int operandBits)
        {
            var reg = arch.GetRegister(operandBits & 7);
            //Debug.Print("operandBits {0:X} {1:X} ", (operandBits >> 3) & 7, operandBits & 7);
            if (reg == Registers.pc)
            {
                switch ((operandBits >> 3) & 7)
                {
                case 0: return new RegisterOperand(reg);
                case 1: return new MemoryOperand(AddressMode.RegDef, dataWidth, reg);
                case 2: return new ImmediateOperand(Constant.Word16(rdr.ReadLeUInt16()));
                case 3: return new MemoryOperand(rdr.ReadLeUInt16(), dataWidth);
                case 6:
                    return new MemoryOperand(AddressMode.Indexed, dataWidth, reg)
                    {
                        EffectiveAddress = rdr.ReadLeUInt16()
                    };
                // PC relative
                case 7:
                    return new MemoryOperand(AddressMode.IndexedDef, dataWidth, reg)
                    {
                        EffectiveAddress = rdr.ReadLeUInt16()
                    };
                }
                throw new NotImplementedException();
            }
            else
            {
                switch ((operandBits >> 3) & 7)
                {
                case 0: return new RegisterOperand(reg);                                 //   Reg           Direct addressing of the register
                case 1: return new MemoryOperand(AddressMode.RegDef, dataWidth, reg);      //   Reg Def       Contents of Reg is the address
                case 2: return new MemoryOperand(AddressMode.AutoIncr, dataWidth, reg);   //   AutoIncr      Contents of Reg is the address, then Reg incremented
                case 3: return new MemoryOperand(AddressMode.AutoIncrDef, dataWidth, reg);    //   AutoIncrDef   Content of Reg is addr of addr, then Reg Incremented
                case 4: return new MemoryOperand(AddressMode.AutoDecr, dataWidth, reg);   //   AutoDecr      Reg incremented, then contents of Reg is the address
                case 5: return new MemoryOperand(AddressMode.AutoDecrDef, dataWidth, reg);    //   AutoDecrDef   Reg is decremented then contents is addr of addr
                case 6: return new MemoryOperand(AddressMode.Indexed, dataWidth, reg)
                        {
                            EffectiveAddress = rdr.ReadLeUInt16()
                        };
                //case 6: return new MemoryOperand(reg, rdr.ReadLeUInt16());   //   Index         Contents of Reg + Following word is address
                case 7: return new MemoryOperand(AddressMode.IndexedDef, dataWidth, reg)
                        {
                            EffectiveAddress = rdr.ReadLeUInt16()
                        };
                //   IndexDef      Contents of Reg + Following word is addr of addr
                default: throw new NotSupportedException(string.Format("Address mode {0} not supported.", (operandBits >> 3) & 7));
                }
            }
        }
    }
}

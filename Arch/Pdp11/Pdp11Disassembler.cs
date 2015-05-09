#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Arch.Pdp11
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
            instrCur = Disassemble();
            instrCur.Address = addr;
            instrCur.Length = (int)(rdr.Address - addr);
            return instrCur;
        }

        private Pdp11Instruction DecodeOperands(ushort wOpcode, Opcodes opcode, string fmt)
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
            private Opcodes opcode;

            public FormatOpRec(string fmt, Opcodes op)
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
                new FormatOpRec("w:e,E", Opcodes.mov),
                new FormatOpRec("E,e", Opcodes.cmp),
                new FormatOpRec("E,e", Opcodes.bit),
                new FormatOpRec("E,e", Opcodes.bic),
                new FormatOpRec("E,e", Opcodes.bis),
                new FormatOpRec("w:E,e", Opcodes.add),
                null,

                null,
                new FormatOpRec("b:e,E", Opcodes.movb),
                new FormatOpRec("E,e", Opcodes.cmp),
                new FormatOpRec("E,e", Opcodes.bit),
                new FormatOpRec("E,e", Opcodes.bic),
                new FormatOpRec("E,e", Opcodes.bis),
                new FormatOpRec("w:E,e", Opcodes.sub),
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
                case 2:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcodes.ash,
                        DataWidth = dataWidth,
                        op1 = DecodeOperand(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
                    };
                case 3:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcodes.ashc,
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
                case 5:
                    return FpuArithmetic(opcode);
                case 7:
                    dataWidth = PrimitiveType.Word16;
                    return new Pdp11Instruction
                    {
                        Opcode = Opcodes.sob,
                        DataWidth = dataWidth,
                        op1 = Imm6(opcode),
                        op2 = new RegisterOperand(arch.GetRegister((opcode >> 6) & 7)),
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
            throw new NotImplementedException();
        }

        private Pdp11Instruction FpuArithmetic(ushort opcode)
        {
            throw new NotImplementedException();
        }

        private PrimitiveType DataWidthFromSizeBit(uint p)
        {
            return p != 0 ? PrimitiveType.Byte : PrimitiveType.Word16;
        }

        private Pdp11Instruction NonDoubleOperandInstruction(ushort opcode)
        {
            switch ((opcode >> 8))
            {
            case 0x01:
            case 0x02:
            case 0x03:
            case 0x04:
            case 0x05:
            case 0x06:
            case 0x07:
            case 0x80:
            case 0x81:
            case 0x82:
            case 0x83:
            case 0x84:
            case 0x85:
            case 0x86:
            case 0x87:
                return BranchInstruction(opcode);
            }


            var dataWidth = DataWidthFromSizeBit(opcode & 0x8000u);
            var op = DecodeOperand(opcode);
            Opcodes oc = Opcodes.illegal;
            switch ((opcode >> 6) & 0x3FF)
            {
            case 0x000:
                switch (opcode & 0x3F)
                {
                case 0x00: op= null;oc = Opcodes.halt; break;
                case 0x01: op= null;oc = Opcodes.wait; break;
                case 0x02: op= null;oc = Opcodes.rti; break;
                case 0x03: op= null;oc = Opcodes.bpt; break;
                case 0x04: op= null;oc = Opcodes.iot; break;
                case 0x05: op= null;oc = Opcodes.reset; break;
                case 0x06: op = null; oc = Opcodes.rtt; break;
                case 0x07: op = null;  oc = Opcodes.illegal; break;
                }
                break;
            case 0x001: oc = Opcodes.jmp; break;
            case 0x002:
                switch (opcode & 0x38)
                {
                case 0: op = DecodeOperand(opcode & 7); oc = Opcodes.rts; break;
                case 3: op = Imm3(opcode); oc = Opcodes.spl; break;
                case 4:
                case 5:
                case 6:
                case 7:
                    throw new NotImplementedException("Cond codes");
                }
                break;
            case 0x003:
                oc = Opcodes.swab;
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
                return new Pdp11Instruction { Opcode = Opcodes.jsr, op1= Reg(opcode >> 6), op2 = op };
            case 0x220:
            case 0x221:
            case 0x222:
            case 0x223:
                oc = Opcodes.emt; op = null; break;
            case 0x224:
            case 0x225:
            case 0x226:
            case 0x227:
                oc = Opcodes.trap; op = null; break;

            case 0x028:
            case 0x228:
                oc = dataWidth.Size == 1 ? Opcodes.clrb : Opcodes.clr;
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

        private MachineOperand Reg(int bits)
        {
            return new RegisterOperand(arch.GetRegister(bits & 7));
        }

        private MachineOperand Imm3(ushort opcode)
        {
            throw new NotImplementedException();
        }

        private Pdp11Instruction BranchInstruction(ushort opcode)
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
            Debug.Print("operandBits {0:X} {1:X} ", (operandBits >> 3) & 7, operandBits & 7);
            if (reg == Registers.pc)
            {
                switch ((operandBits >> 3) & 7)
                {
                case 0: return new RegisterOperand(reg);
                case 1: return new MemoryOperand(AddressMode.RegDef, dataWidth, reg); 
                case 2: return new ImmediateOperand(Constant.Word16(rdr.ReadLeUInt16()));
                case 3: return new MemoryOperand(rdr.ReadLeUInt16(), dataWidth); 

                    // PC relative

                }
                throw new NotImplementedException();
            }
            else
            {
                switch ((operandBits >> 3) & 7)
                {
                case 0: return new RegisterOperand(reg);    //   Reg           Direct addressing of the register
                case 1: return new MemoryOperand(AddressMode.RegDef, dataWidth, reg);      //   Reg Def       Contents of Reg is the address
                case 2: return new MemoryOperand(AddressMode.AutoIncr, dataWidth, reg);   //   AutoIncr      Contents of Reg is the address, then Reg incremented
                case 4: return new MemoryOperand(AddressMode.AutoDecr, dataWidth, reg);   //   AutoDecr      Reg incremented, then contents of Reg is the address
                //case 3:    //   AutoIncrDef   Content of Reg is addr of addr, then Reg Incremented
                //case 4:    //   AutoDecr      Reg is decremented then contents is address
                //case 5:    //   AutoDecrDef   Reg is decremented then contents is addr of addr
                case 6: return new MemoryOperand(AddressMode.Indexed, dataWidth, reg)
                        {
                            EffectiveAddress = rdr.ReadLeUInt16()
                        };
                //case 6: return new MemoryOperand(reg, rdr.ReadLeUInt16());   //   Index         Contents of Reg + Following word is address
                //case 7:   //   IndexDef      Contents of Reg + Following word is addr of addr
                default: throw new NotSupportedException(string.Format("Address mode {0} not supported.", (operandBits >> 3) & 7));
                }
            }
        }
    }
}

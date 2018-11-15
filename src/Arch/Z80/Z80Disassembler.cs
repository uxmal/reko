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

namespace Reko.Arch.Z80
{
    /// <summary>
    /// Disassembles both 8080 and Z80 instructions, with respective syntax.
    /// </summary>
    public class Z80Disassembler : DisassemblerBase<Z80Instruction>
    {
        private EndianImageReader rdr;
        private RegisterStorage IndexRegister;
        private Z80Instruction instr;

        public Z80Disassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override Z80Instruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;
            this.instr = new Z80Instruction
            {
                Address = addr,
            };
            if (!rdr.TryReadByte(out byte op))
                return Invalid(addr);
            
            var opRef = oprecs[op];
            this.IndexRegister = null;
            instr = opRef.Decode(this, op, "");
            if (instr == null)
                return Invalid(addr);
            instr.Address = addr;
            instr.IClass |= op == 0 ? InstrClass.Zero : 0;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        private Z80Instruction Invalid(Address addr)
        {
            return new Z80Instruction
            {
                Address = addr,
                Code = Opcode.illegal,
            };
        }

        public Z80Instruction DecodeOperands(Opcode opcode, InstrClass iclass, byte op, string fmt)
        {
            var ops = new MachineOperand[2];
            var iOp = 0;
            ushort us;
            for (int i = 0; i < fmt.Length; ++i)
            {
                if (fmt[i] == ',')
                    ++i;
                switch (fmt[i++])
                {
                default:
                    throw new NotSupportedException(string.Format("Unknown format specifier {0}.", fmt[i - 1]));
                case 'a':       // the 'A' register
                    ops[iOp++] = new RegisterOperand(Registers.a);
                    break;
                case 'A':       // Absolute memory address.
                    if (!rdr.TryReadLeUInt16(out us))
                        return null;
                    ops[iOp++] = AddressOperand.Ptr16(us);
                    break;
                case 'R':       // register encoded in bits 0..2 of op.
                    ops[iOp++] = new RegisterOperand(ByteRegister(op));
                    break;
                case 'r':        // register encoded in bits 3..5 of op
                    ops[iOp++] = new RegisterOperand(ByteRegister(op >> 3));
                    break;
                case 'M':       // memory fetch from HL (or IX,IY)
                    ops[iOp++] = MemOperand(fmt[i++]);
                    break;
                case 'B':       // memory access using BC 
                    ops[iOp++] = new MemoryOperand(Registers.bc, PrimitiveType.Byte);
                    break;
                case 'D':       // memory access using DE 
                    ops[iOp++] = new MemoryOperand(Registers.de, PrimitiveType.Byte);
                    break;
                case 'H':       // memory access using HL
                    ops[iOp++] = new MemoryOperand(IndexRegister != null ? IndexRegister : Registers.hl, PrimitiveType.Byte);
                    break;
                case 'S':       // memory access using SP
                    ops[iOp++] = new MemoryOperand(Registers.sp, OperandSize(fmt[i++]));
                    break;
                case 'O':       // direct operand
                    ops[iOp++] = DirectOperand(PrimitiveType.Word16, OperandSize(fmt[i++]));
                    break;
                case 'o':       // direct operand (byte sized)
                    ops[iOp++] = DirectOperand(PrimitiveType.Byte, OperandSize(fmt[i++]));
                    break;
                case 'I':
                    Constant imm;
                    if (!rdr.TryReadLe(OperandSize(fmt[i++]), out imm))
                        return null;
                    ops[iOp++] = new ImmediateOperand(imm);
                    break;
                case 'W':
                    ops[iOp++] = new RegisterOperand(WordRegister(fmt[i++]));
                    break;
                case 'C':
                    ops[iOp++] = new ConditionOperand(ConditionCode(op >> 3));
                    break;
                case 'Q':
                    ops[iOp++] = new ConditionOperand(ConditionCode((op >> 3) & 3));
                    break;
                case 'J':       // Relative jump
                    var width = OperandSize(fmt[i++]);
                    int ipOffset = (int) rdr.ReadLeSigned(width);
                    ops[iOp++] = AddressOperand.Ptr16((ushort)(rdr.Address.ToUInt16() + ipOffset));
                    break;
                case 'x':       // 2-digit Inline hexadecimal byte
                    int val = (Hex(fmt[i++]) << 4);
                    val |= Hex(fmt[i++]);
                    ops[iOp++] = new ImmediateOperand(Constant.Byte((byte) val));
                    break;
                case '[':
                    ++i;
                    ops[iOp++] = new MemoryOperand(Registers.c, PrimitiveType.Byte);
                    break;
                case 'L':
                    ops[iOp++] = new RegisterOperand(LiteralRegister(fmt[i++]));
                    break;
                }
            }
            instr.Code = opcode;
            instr.IClass = iclass;
            instr.Length = (int)(rdr.Address - instr.Address);
            instr.Op1 = ops[0];
            instr.Op2 = ops[1];
            return instr;
        }

        private MemoryOperand MemOperand(char chWidth)
        {
            RegisterStorage baseReg = Registers.hl;
            sbyte offset = 0;
            if (IndexRegister != null)
            {
                baseReg = IndexRegister;
                offset = (sbyte) rdr.ReadByte();
            }
            var pop = new MemoryOperand(baseReg, offset, OperandSize(chWidth));
            return pop;
        }

        private MachineOperand DirectOperand(PrimitiveType addrSize, PrimitiveType dataSize)
        {
            return new MemoryOperand(rdr.ReadLe(addrSize), dataSize);
        }

        private static int Hex(char c)
        {
            if ('0' <= c && c <= '9')
                return c - '0';
            if ('a' <= c && c <= 'f')
                return 10 + c - 'a';
            if ('A' <= c && c <= 'F')
                return 10 + c - 'A';
            throw new FormatException();
        }

        private CondCode ConditionCode(int bits)
        {
            switch (bits & 7)
            {
            default: throw new NotImplementedException();
            case 0: return CondCode.nz;
            case 1: return CondCode.z;
            case 2: return CondCode.nc;
            case 3: return CondCode.c;
            case 4: return CondCode.po;
            case 5: return CondCode.pe;
            case 6: return CondCode.p;
            case 7: return CondCode.m;
            }
        }

        private RegisterStorage ByteRegister(int bits)
        {
            switch (bits & 0x7)
            {
            default: throw new NotImplementedException(string.Format("Unknown Z80 register {0}.", bits & 7));
            case 0: return Registers.b;
            case 1: return Registers.c;
            case 2: return Registers.d;
            case 3: return Registers.e;
            case 4: return Registers.h;
            case 5: return Registers.l;
            case 7: return Registers.a;
            }
        }

        private RegisterStorage LiteralRegister(char ch)
        {
            switch (ch)
            {
            case 'r': return Registers.r;
            case 'i': return Registers.i;
            default: throw new NotImplementedException(string.Format("Unknown register {0}.", ch));
            }
        }

        private RegisterStorage WordRegister(char encoding)
        {
            switch (encoding)
            {
            case 'B': case 'b': return Registers.bc;
            case 'D': case 'd': return Registers.de;
            case 'H': case 'h': return IndexRegister != null ? IndexRegister : Registers.hl;
            case 'S': case 's': return Registers.sp;
            case 'A': case 'a': return Registers.af;
            default: throw new NotImplementedException(string.Format("Unknown word register specifier {0}", encoding));
            }
        }

        private PrimitiveType OperandSize(char c)
        {
            switch (c)
            {
            default: throw new NotSupportedException(string.Format("Unsupported operand size {0}.", c));
            case 'b': return PrimitiveType.Byte;
            case 'w': return PrimitiveType.Word16;
            }
        }

        private abstract class OpRec
        {
            public abstract Z80Instruction Decode(Z80Disassembler disasm, byte op, string opFormat);
        }

        private class SingleByteOpRec : OpRec
        {
            public readonly Opcode i8080Opcode;
            public readonly Opcode Z80Opcode;
            public readonly InstrClass IClass;
            public readonly string Format;

            public SingleByteOpRec(Opcode i8080, Opcode z80, string format, InstrClass iclass = InstrClass.Linear)
            {
                this.i8080Opcode = i8080;
                this.Z80Opcode = z80;
                this.IClass = iclass;
                this.Format = format;
            }

            public override Z80Instruction Decode(Z80Disassembler disasm, byte op, string opFormat)
            {
                return disasm.DecodeOperands(Z80Opcode, IClass, op, Format);
            }
        }

        private class IndexPrefixOpRec : OpRec
        {
            private RegisterStorage IndexRegister;

            public IndexPrefixOpRec(RegisterStorage idxReg)
            {
                this.IndexRegister = idxReg;
            }

            public override Z80Instruction Decode(Z80Disassembler disasm, byte op, string opFormat)
            {
                disasm.IndexRegister = this.IndexRegister;
                op = disasm.rdr.ReadByte();
                if (op == 0xCB)
                {
                    var offset = disasm.rdr.ReadSByte();
                    op = disasm.rdr.ReadByte();
                    switch (op >> 6)
                    {
                    default: throw new NotImplementedException();
                    case 1:
                        return new Z80Instruction
                        {
                            Code = Opcode.bit,
                            Op1 = new ImmediateOperand(Constant.Byte((byte) ((op >> 3) & 0x07))),
                            Op2 = new MemoryOperand(IndexRegister, offset, PrimitiveType.Byte)
                        };
                    case 2:
                        return new Z80Instruction
                        {
                            Code = Opcode.res,
                            Op1 = new ImmediateOperand(Constant.Byte((byte)((op >> 3) & 0x07))),
                            Op2 = new MemoryOperand(IndexRegister, offset, PrimitiveType.Byte)
                        };
                    case 3:
                        return new Z80Instruction
                        {
                            Code = Opcode.set,
                            Op1 = new ImmediateOperand(Constant.Byte((byte)((op >> 3) & 0x07))),
                            Op2 = new MemoryOperand(IndexRegister, offset, PrimitiveType.Byte)
                        };
                    }
                }
                else
                {
                    return oprecs[op].Decode(disasm, op, opFormat);
                }
            }
        }

        private class CbPrefixOpRec : OpRec
        {
            static readonly Opcode[] cbOpcodes = new Opcode[] {
                Opcode.rlc,
                Opcode.rrc,
                Opcode.rl,
                Opcode.rr,
                Opcode.sla,
                Opcode.sra,
                Opcode.swap,
                Opcode.srl,
            };

            static readonly string[] cbFormats = new string[] {
                "R", "R", "R", "R", "R", "R", "Mb", "R", 
            };

            public override Z80Instruction Decode(Z80Disassembler disasm, byte op, string opFormat)
            {
                op = disasm.rdr.ReadByte();
                Opcode code;
                MachineOperand Op2;
                switch (op >> 6)
                {
                default: throw new NotImplementedException();
                case 0:
                    return disasm.DecodeOperands(
                        cbOpcodes[(op >> 3) & 0x07],
                        InstrClass.Linear,
                        op,
                        cbFormats[op & 0x07]);
                case 1:
                    code = Opcode.bit;
                    break;
                case 2:
                    code = Opcode.res;
                    break;
                case 3:
                    code = Opcode.set;
                    break;
                }
                if ((op & 7) == 6)
                {
                    Op2 = disasm.MemOperand('b');
                }
                else
                {
                    Op2 = new RegisterOperand(disasm.ByteRegister(op));
                }
                return new Z80Instruction
                {
                    Code = code,
                    Op1 = new ImmediateOperand(Constant.Byte((byte)((op >> 3) & 0x07))),
                    Op2 = Op2,
                };
            }
        }

        private class EdPrefixOpRec : OpRec
        {
            public override Z80Instruction Decode(Z80Disassembler disasm, byte op, string opFormat)
            {
                op = disasm.rdr.ReadByte();
                OpRec oprec = null;
                if (0x40 <= op && op < 0x80)
                    oprec = edOprecs[op-0x40];
                else if (0xA0 <= op && op < 0xC0)
                    oprec = edOprecs[op - 0x60];
                else 
                    return new Z80Instruction { Code = Opcode.illegal };
                return oprec.Decode(disasm, op, ((SingleByteOpRec)oprec).Format);
            }
        }

        /// <summary>
        /// References:
        /// http://wikiti.brandonw.net/index.php?title=Z80_Instruction_Set
        /// http://www.zophar.net/fileuploads/2/10807fvllz/z80-1.txt
        /// </summary>
        private static readonly OpRec [] oprecs = new OpRec[] 
        {
            // 00
            new SingleByteOpRec(Opcode.nop, Opcode.nop, "", InstrClass.Zero|InstrClass.Linear|InstrClass.Padding),
            new SingleByteOpRec(Opcode.lxi, Opcode.ld, "Wb,Iw"),
            new SingleByteOpRec(Opcode.stax, Opcode.ld, "B,a"),
            new SingleByteOpRec(Opcode.inx, Opcode.inc, "Wb"),
            new SingleByteOpRec(Opcode.inr, Opcode.inc, "r"),
            new SingleByteOpRec(Opcode.dcr, Opcode.dec, "r"),
            new SingleByteOpRec(Opcode.mvi, Opcode.ld, "r,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rlca, ""),

            new SingleByteOpRec(Opcode.illegal, Opcode.ex_af, ""),
            new SingleByteOpRec(Opcode.dad, Opcode.add, "Wh,Wb"),
            new SingleByteOpRec(Opcode.ldax, Opcode.ld, "a,B"),
            new SingleByteOpRec(Opcode.dcx, Opcode.dec, "Wb"),
            new SingleByteOpRec(Opcode.inr, Opcode.inc, "r"),
            new SingleByteOpRec(Opcode.dcr, Opcode.dec, "r"),
            new SingleByteOpRec(Opcode.mvi, Opcode.ld, "r,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rrca, ""),

            // 10
            new SingleByteOpRec(Opcode.illegal, Opcode.djnz, "Jb", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.lxi, Opcode.ld, "Wd,Iw"),
            new SingleByteOpRec(Opcode.stax, Opcode.ld, "D,a"),
            new SingleByteOpRec(Opcode.inx, Opcode.inc, "Wd"),
            new SingleByteOpRec(Opcode.inr, Opcode.inc, "r"),
            new SingleByteOpRec(Opcode.dcr, Opcode.dec, "r"),
            new SingleByteOpRec(Opcode.mvi, Opcode.ld, "r,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rla, ""),

            new SingleByteOpRec(Opcode.illegal, Opcode.jr, "Jb", InstrClass.Transfer),
            new SingleByteOpRec(Opcode.dad, Opcode.add, "Wh,Wd"),
            new SingleByteOpRec(Opcode.ldax, Opcode.ld, "a,D"),
            new SingleByteOpRec(Opcode.dcx, Opcode.dec, "Wd"),
            new SingleByteOpRec(Opcode.inr, Opcode.inc, "r"),
            new SingleByteOpRec(Opcode.dcr, Opcode.dec, "r"),
            new SingleByteOpRec(Opcode.mvi, Opcode.ld, "r,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rra, ""),

            // 20
            new SingleByteOpRec(Opcode.illegal, Opcode.jr, "Q,Jb", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.lxi, Opcode.ld, "Wh,Iw"),
            new SingleByteOpRec(Opcode.shld, Opcode.ld, "Ow,Wh"),
            new SingleByteOpRec(Opcode.inx, Opcode.inc, "Wh"),
            new SingleByteOpRec(Opcode.inr, Opcode.inc, "r"),
            new SingleByteOpRec(Opcode.dcr, Opcode.dec, "r"),
            new SingleByteOpRec(Opcode.mvi, Opcode.ld, "r,Ib"),
            new SingleByteOpRec(Opcode.daa, Opcode.daa, ""),

            new SingleByteOpRec(Opcode.illegal, Opcode.jr, "Q,Jb", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.dad, Opcode.add, "Wh,Wh"),
            new SingleByteOpRec(Opcode.lhld, Opcode.ld, "Wh,Ow"),
            new SingleByteOpRec(Opcode.dcx, Opcode.dec, "Wh"),
            new SingleByteOpRec(Opcode.inr, Opcode.inc, "r"),
            new SingleByteOpRec(Opcode.dcr, Opcode.dec, "r"),
            new SingleByteOpRec(Opcode.mvi, Opcode.ld, "r,Ib"),
            new SingleByteOpRec(Opcode.cma, Opcode.cpl, ""),

            // 30
            new SingleByteOpRec(Opcode.illegal, Opcode.jr, "Q,Jb", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.lxi, Opcode.ld, "Ws,Iw"),
            new SingleByteOpRec(Opcode.sta, Opcode.ld, "Ob,a"),
            new SingleByteOpRec(Opcode.inx, Opcode.inc, "Ws"),
            new SingleByteOpRec(Opcode.inr, Opcode.inc, "Hb"),
            new SingleByteOpRec(Opcode.dcr, Opcode.dec, "Hb"),
            new SingleByteOpRec(Opcode.mvi, Opcode.ld, "Mb,Ib"),
            new SingleByteOpRec(Opcode.stc, Opcode.scf, ""),

            new SingleByteOpRec(Opcode.illegal, Opcode.jr, "Q,Jb"),
            new SingleByteOpRec(Opcode.dad, Opcode.add, "Wh,Ws"),
            new SingleByteOpRec(Opcode.lda, Opcode.ld, "a,Ob"),
            new SingleByteOpRec(Opcode.dcx, Opcode.dec, "Ws"),
            new SingleByteOpRec(Opcode.inr, Opcode.inc, "r"),
            new SingleByteOpRec(Opcode.dcr, Opcode.dec, "r"),
            new SingleByteOpRec(Opcode.mvi, Opcode.ld, "r,Ib"),
            new SingleByteOpRec(Opcode.cmc, Opcode.ccf, ""),

            // 40
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,Mb"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),

            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,Mb"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),

            // 50
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,Mb"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),

            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,Mb"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),

            // 60
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,Mb"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),

            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,Mb"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),

            // 70
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "Mb,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "Mb,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "Mb,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "Mb,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "Mb,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "Mb,R"),
            new SingleByteOpRec(Opcode.hlt, Opcode.hlt, "", InstrClass.Terminates),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "Mb,R"),

            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,Mb"),
            new SingleByteOpRec(Opcode.mov, Opcode.ld, "r,R"),

            // 80
            new SingleByteOpRec(Opcode.add, Opcode.add, "a,R"),
            new SingleByteOpRec(Opcode.add, Opcode.add, "a,R"),
            new SingleByteOpRec(Opcode.add, Opcode.add, "a,R"),
            new SingleByteOpRec(Opcode.add, Opcode.add, "a,R"),
            new SingleByteOpRec(Opcode.add, Opcode.add, "a,R"),
            new SingleByteOpRec(Opcode.add, Opcode.add, "a,R"),
            new SingleByteOpRec(Opcode.add, Opcode.add, "a,Mb"),
            new SingleByteOpRec(Opcode.add, Opcode.add, "a,R"),

            new SingleByteOpRec(Opcode.adc, Opcode.adc, "a,R"),
            new SingleByteOpRec(Opcode.adc, Opcode.adc, "a,R"),
            new SingleByteOpRec(Opcode.adc, Opcode.adc, "a,R"),
            new SingleByteOpRec(Opcode.adc, Opcode.adc, "a,R"),
            new SingleByteOpRec(Opcode.adc, Opcode.adc, "a,R"),
            new SingleByteOpRec(Opcode.adc, Opcode.adc, "a,R"),
            new SingleByteOpRec(Opcode.adc, Opcode.adc, "a,Mb"),
            new SingleByteOpRec(Opcode.adc, Opcode.adc, "a,R"),

            // 90
            new SingleByteOpRec(Opcode.sub, Opcode.sub, "a,R"),
            new SingleByteOpRec(Opcode.sub, Opcode.sub, "a,R"),
            new SingleByteOpRec(Opcode.sub, Opcode.sub, "a,R"),
            new SingleByteOpRec(Opcode.sub, Opcode.sub, "a,R"),
            new SingleByteOpRec(Opcode.sub, Opcode.sub, "a,R"),
            new SingleByteOpRec(Opcode.sub, Opcode.sub, "a,R"),
            new SingleByteOpRec(Opcode.sub, Opcode.sub, "a,Mb"),
            new SingleByteOpRec(Opcode.sub, Opcode.sub, "a,R"),

            new SingleByteOpRec(Opcode.sbb, Opcode.sbc, "a,R"),
            new SingleByteOpRec(Opcode.sbb, Opcode.sbc, "a,R"),
            new SingleByteOpRec(Opcode.sbb, Opcode.sbc, "a,R"),
            new SingleByteOpRec(Opcode.sbb, Opcode.sbc, "a,R"),
            new SingleByteOpRec(Opcode.sbb, Opcode.sbc, "a,R"),
            new SingleByteOpRec(Opcode.sbb, Opcode.sbc, "a,R"),
            new SingleByteOpRec(Opcode.sbb, Opcode.sbc, "a,Mb"),
            new SingleByteOpRec(Opcode.sbb, Opcode.sbc, "a,R"),

            // A0
            new SingleByteOpRec(Opcode.ana, Opcode.and, "a,R"),
            new SingleByteOpRec(Opcode.ana, Opcode.and, "a,R"),
            new SingleByteOpRec(Opcode.ana, Opcode.and, "a,R"),
            new SingleByteOpRec(Opcode.ana, Opcode.and, "a,R"),
            new SingleByteOpRec(Opcode.ana, Opcode.and, "a,R"),
            new SingleByteOpRec(Opcode.ana, Opcode.and, "a,R"),
            new SingleByteOpRec(Opcode.ana, Opcode.and, "a,Mb"),
            new SingleByteOpRec(Opcode.ana, Opcode.and, "a,R"),

            new SingleByteOpRec(Opcode.xra, Opcode.xor, "a,R"),
            new SingleByteOpRec(Opcode.xra, Opcode.xor, "a,R"),
            new SingleByteOpRec(Opcode.xra, Opcode.xor, "a,R"),
            new SingleByteOpRec(Opcode.xra, Opcode.xor, "a,R"),
            new SingleByteOpRec(Opcode.xra, Opcode.xor, "a,R"),
            new SingleByteOpRec(Opcode.xra, Opcode.xor, "a,R"),
            new SingleByteOpRec(Opcode.xra, Opcode.xor, "a,Mb"),
            new SingleByteOpRec(Opcode.xra, Opcode.xor, "a,R"),

            // B0
            new SingleByteOpRec(Opcode.ora, Opcode.or, "a,R"),
            new SingleByteOpRec(Opcode.ora, Opcode.or, "a,R"),
            new SingleByteOpRec(Opcode.ora, Opcode.or, "a,R"),
            new SingleByteOpRec(Opcode.ora, Opcode.or, "a,R"),
            new SingleByteOpRec(Opcode.ora, Opcode.or, "a,R"),
            new SingleByteOpRec(Opcode.ora, Opcode.or, "a,R"),
            new SingleByteOpRec(Opcode.ora, Opcode.or, "a,Mb"),
            new SingleByteOpRec(Opcode.ora, Opcode.or, "a,R"),

            new SingleByteOpRec(Opcode.cmp, Opcode.cp, "a,R"),
            new SingleByteOpRec(Opcode.cmp, Opcode.cp, "a,R"),
            new SingleByteOpRec(Opcode.cmp, Opcode.cp, "a,R"),
            new SingleByteOpRec(Opcode.cmp, Opcode.cp, "a,R"),
            new SingleByteOpRec(Opcode.cmp, Opcode.cp, "a,R"),
            new SingleByteOpRec(Opcode.cmp, Opcode.cp, "a,R"),
            new SingleByteOpRec(Opcode.cmp, Opcode.cp, "a,Mb"),
            new SingleByteOpRec(Opcode.cmp, Opcode.cp, "a,R"),

            // C0
            new SingleByteOpRec(Opcode.illegal, Opcode.ret, "C", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.pop, "Wb"),
            new SingleByteOpRec(Opcode.jnz, Opcode.jp, "C,A", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.jmp, Opcode.jp, "A", InstrClass.Transfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.call, "C,A", InstrClass.ConditionalTransfer|InstrClass.Call),
            new SingleByteOpRec(Opcode.illegal, Opcode.push, "Wb"),
            new SingleByteOpRec(Opcode.adi, Opcode.add, "a,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rst, "x00", InstrClass.Transfer|InstrClass.Call),

            new SingleByteOpRec(Opcode.illegal, Opcode.ret, "C", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.ret, "", InstrClass.Transfer),
            new SingleByteOpRec(Opcode.jz, Opcode.jp, "C,A"),
            new CbPrefixOpRec(),
            new SingleByteOpRec(Opcode.illegal, Opcode.call, "C,A", InstrClass.ConditionalTransfer|InstrClass.Call),
            new SingleByteOpRec(Opcode.illegal, Opcode.call, "A", InstrClass.Transfer|InstrClass.Call),
            new SingleByteOpRec(Opcode.aci, Opcode.adc, "a,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rst, "x08", InstrClass.Transfer|InstrClass.Call),

            // D0
            new SingleByteOpRec(Opcode.illegal, Opcode.ret, "C", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.pop, "Wd"),
            new SingleByteOpRec(Opcode.jnc, Opcode.jp, "C,A", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.@out, "ob,a"),
            new SingleByteOpRec(Opcode.illegal, Opcode.call, "C,A", InstrClass.ConditionalTransfer|InstrClass.Call),
            new SingleByteOpRec(Opcode.illegal, Opcode.push, "Wd"),
            new SingleByteOpRec(Opcode.sui, Opcode.sub, "a,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rst, "x10", InstrClass.Transfer|InstrClass.Call),

            new SingleByteOpRec(Opcode.illegal, Opcode.ret, "C", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.exx, ""),
            new SingleByteOpRec(Opcode.jc, Opcode.jp, "C,A", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.@in, "a,ob"),
            new SingleByteOpRec(Opcode.illegal, Opcode.call, "C,A", InstrClass.ConditionalTransfer|InstrClass.Call),
            new IndexPrefixOpRec(Registers.ix),
            new SingleByteOpRec(Opcode.sbi, Opcode.sbc, "a,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rst, "x18", InstrClass.Transfer|InstrClass.Call),

            // E0
            new SingleByteOpRec(Opcode.illegal, Opcode.ret, "C", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.pop, "Wh"),
            new SingleByteOpRec(Opcode.jpo, Opcode.jp, "C,A", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.ex, "Sw,Wh"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.push, "Wh"),
            new SingleByteOpRec(Opcode.illegal, Opcode.add, "a,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rst, "x20", InstrClass.Transfer|InstrClass.Call),

            new SingleByteOpRec(Opcode.illegal, Opcode.add, "Ws,D"),
            new SingleByteOpRec(Opcode.pchl, Opcode.jp, "Mw", InstrClass.Transfer),
            new SingleByteOpRec(Opcode.jpe, Opcode.jp, "C,A", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.illegal, Opcode.ex, "Wd,Wh"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal, ""),
            new EdPrefixOpRec(),
            new SingleByteOpRec(Opcode.illegal, Opcode.xor, "a,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rst, "x28", InstrClass.Transfer|InstrClass.Call),

            // F0
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.pop, "Wa"),
            new SingleByteOpRec(Opcode.jp, Opcode.jp, "C,A", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.di, Opcode.di, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.push, "Wa"),
            new SingleByteOpRec(Opcode.illegal, Opcode.or, "a,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rst, "x30", InstrClass.Transfer|InstrClass.Call),

            new SingleByteOpRec(Opcode.illegal, Opcode.illegal, ""),
            new SingleByteOpRec(Opcode.sphl, Opcode.ld, "Ws,Wh"),
            new SingleByteOpRec(Opcode.jm, Opcode.jp, "C,A", InstrClass.ConditionalTransfer),
            new SingleByteOpRec(Opcode.ei, Opcode.ei, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal, ""),
            new IndexPrefixOpRec(Registers.iy),
            new SingleByteOpRec(Opcode.illegal, Opcode.cp, "a,Ib"),
            new SingleByteOpRec(Opcode.illegal, Opcode.rst, "x38", InstrClass.Transfer|InstrClass.Call),
        };

        private static readonly OpRec[] edOprecs = new OpRec[] 
        {
            // 40
            new SingleByteOpRec(Opcode.illegal, Opcode.@in, "r,[c"), 
            new SingleByteOpRec(Opcode.illegal, Opcode.@out, "[c,r"), 
            new SingleByteOpRec(Opcode.illegal, Opcode.sbc,  "Wh,Wb"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Ow,Wb"),
            new SingleByteOpRec(Opcode.illegal, Opcode.neg,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.retn,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.im,  "x00"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Li,a"),

            new SingleByteOpRec(Opcode.illegal, Opcode.@in,  "r,[c"),
            new SingleByteOpRec(Opcode.illegal, Opcode.@out,  "[c,r"),
            new SingleByteOpRec(Opcode.illegal, Opcode.adc,  "Wh,Wb"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Wb,Ow"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.reti,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Lr,a"),
            
            // 50
            new SingleByteOpRec(Opcode.illegal, Opcode.@in, "r[c"), 
            new SingleByteOpRec(Opcode.illegal, Opcode.@out, "[c,r"), 
            new SingleByteOpRec(Opcode.illegal, Opcode.sbc,  "Wh,Wd"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Ow,Wd"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.im,  "x01"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "a,Li"),

            new SingleByteOpRec(Opcode.illegal, Opcode.@in,  "r,[c"),
            new SingleByteOpRec(Opcode.illegal, Opcode.@out,  "[c,r"),
            new SingleByteOpRec(Opcode.illegal, Opcode.adc,  "Wh,Wd"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Wd,Ow"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.im,  "x02"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "a,Lr"),

            // 60
            new SingleByteOpRec(Opcode.illegal, Opcode.@in, "r,[c"), 
            new SingleByteOpRec(Opcode.illegal, Opcode.@out, "[c,r"), 
            new SingleByteOpRec(Opcode.illegal, Opcode.sbc,  "Wh,Wh"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Ow,Wh"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.rrd,  ""),

            new SingleByteOpRec(Opcode.illegal, Opcode.@in,  "r,[c"),
            new SingleByteOpRec(Opcode.illegal, Opcode.@out,  "[c,r"),
            new SingleByteOpRec(Opcode.illegal, Opcode.adc,  "Wh,Wh"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Wh,Ow"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.rld,  ""),
            
            // 70
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.sbc,  "Wh,Ws"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Ow,Ws"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),

            new SingleByteOpRec(Opcode.illegal, Opcode.@in,  "r,[c"),
            new SingleByteOpRec(Opcode.illegal, Opcode.@out,  "[c,r"),
            new SingleByteOpRec(Opcode.illegal, Opcode.adc,  "Wh,Ws"),
            new SingleByteOpRec(Opcode.illegal, Opcode.ld,  "Ws,Ow"),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
 
            // A0
            new SingleByteOpRec(Opcode.illegal, Opcode.ldi, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.cpi , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.ini  , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.outi , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),

            new SingleByteOpRec(Opcode.illegal, Opcode.ldd, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.cpd , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.ind  , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.outd , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),

            // B0
            new SingleByteOpRec(Opcode.illegal, Opcode.ldir, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.cpir , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.inir  , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.otir , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),

            new SingleByteOpRec(Opcode.illegal, Opcode.lddr, ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.cpdr , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.indr  , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.otdr , ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
            new SingleByteOpRec(Opcode.illegal, Opcode.illegal,  ""),
        };
#if NEVER

---		LD	BC,(word)	ED4Bword	BC <- (word)
---		LD	DE,(word)	ED5Bword	DE <- (word)
---		LD	HL,(word)	ED6Bword	HL <- (word)
---		LD	SP,(word)	ED7Bword	SP <- (word)

---		LD	(word),BC	ED43word	(word) <- BC
---		LD	(word),DE	ED53word	(word) <- DE
---		LD	(word),HL	ED6Bword	(word) <- HL
---		LD	(word),SP	ED73word	(word) <- SP

Register Exchange Instructions

XCHG		EX	DE,HL		EB		HL <-> DE
XTHL		EX	(SP),HL		E3	    H <-> (SP+1); L <-> (SP)
---		EX	(SP),IX		DDE3	 IXh <-> (SP+1); IXl <-> (SP)
---		EX	(SP),IY		FDE3	 IYh <-> (SP+1); IYl <-> (SP)
---		EX	AF,AF'		08		AF <-> AF'
---		EXX			D9	BC/DE/HL <-> BC'/DE'/HL'


Double Word Add With Carry-In Instructions

---		ADC	HL,BC		ED4A		HL <- HL + BC + Carry
---		ADC	HL,DE		ED5A		HL <- HL + DE + Carry
---		ADC	HL,HL		ED6A		HL <- HL + HL + Carry
---		ADC	HL,SP		ED7A		HL <- HL + SP + Carry


Double Word Subtract With Borrow-In Instructions

---		SBC	HL,BC		ED42		HL <- HL - BC - Carry
---		SBC	HL,DE		ED52		HL <- HL - DE - Carry
---		SBC	HL,HL		ED62		HL <- HL - HL - Carry
---		SBC	HL,SP		ED72		HL <- HL - SP - Carry


Control Instructions

---		IM	0		ED46		---
---		IM	1		ED56		---
---		IM	2		ED5E		---
---		LD	I,A		ED47		Interrupt Page <- A
---		LD	A,I		ED57		A <- Interrupt Page
---		LD	R,A		ED4F		Refresh Register <- A
---		LD	A,R		ED5F		A <- Refresh Register

Increment Byte Instructions

INR	A	INC	A		3C		A <- A + 1
INR	B	INC	B		04		B <- B + 1
INR	C	INC	C		0C		C <- C + 1
INR	D	INC	D		14		D <- D + 1
INR	E	INC	E		1C		E <- E + 1
INR	H	INC	H		24		H <- H + 1
INR	L	INC	L		2C		L <- L + 1
INR	M	INC	(HL)		34		(HL) <- (HL) + 1
---		INC	(IX+index)	DD34index (IX+index) <- (IX+index) + 1
---		INC	(IY+index)	FD34index (IY+index) <- (IY+index) + 1


Decrement Byte Instructions

DCR	A	DEC	A		3D		A <- A - 1
DCR	B	DEC	B		05		B <- B - 1
DCR	C	DEC	C		0D		C <- C - 1
DCR	D	DEC	D		15		D <- D - 1
DCR	E	DEC	E		1D		E <- E - 1
DCR	H	DEC	H		25		H <- H - 1
DCR	L	DEC	L		2D		L <- L - 1
DCR	M	DEC	(HL)		35		(HL) <- (HL) - 1
---		DEC	(IX+index)	DD35index (IX+index) <- (IX+index) - 1
---		DEC	(IY+index)	FD35index (IY+index) <- (IY+index) - 1


Increment Register Pair Instructions

INX	B	INC	BC		03		BC <- BC + 1
INX	D	INC	DE		13		DE <- DE + 1
INX	H	INC	HL		23		HL <- HL + 1
INX	SP	INC	SP		33		SP <- SP + 1
---		INC	IX		DD23		IX <- IX + 1
---		INC	IY		FD23		IY <- IY + 1


8080/Z80  Instruction  Set					Page 8

Decrement Register Pair Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

DCX	B	DEC	BC		0B		BC <- BC - 1
DCX	D	DEC	DE		1B		DE <- DE - 1
DCX	H	DEC	HL		2B		HL <- HL - 1
DCX	SP	DEC	SP		3B		SP <- SP - 1
---		DEC	IX		DD2B		IX <- IX - 1
---		DEC	IY		FD2B		IY <- IY - 1


Special Accumulator and Flag Instructions

DAA		DAA			27		---
CMA		CPL			2F		A <- NOT A
STC		SCF			37		CF (Carry Flag) <- 1
CMC		CCF			3F		CF (Carry Flag) <-
							NOT CF
---		NEG			ED44		A <- 0-A

Rotate Instructions

RLC		RLCA			07		---
RRC		RRCA			0F		---
RAL		RLA			17		---
RAR		RRA			1F		---
---		RLD			ED6F		---
---		RRD			ED67		---
---		RLC	A		CB07		---
---		RLC	B		CB00		---
---		RLC	C		CB01		---
---		RLC	D		CB02		---
---		RLC	E		CB03		---
---		RLC	H		CB04		---
---		RLC	L		CB05		---
---		RLC	(HL)		CB06		---
---		RLC	(IX+index)	DDCBindex06	---
---		RLC	(IY+index)	FDCBindex06	---

---		RL	A		CB17		---
---		RL	B		CB10		---
---		RL	C		CB11		---
---		RL	D		CB12		---
---		RL	E		CB13		---
---		RL	H		CB14		---
---		RL	L		CB15		---
---		RL	(HL)		CB16		---
---		RL	(IX+index)	DDCBindex16	---
---		RL	(IY+index)	FDCBindex16	---


8080/Z80  Instruction  Set					Page 9

Rotate Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		RRC	A		CB0F		---
---		RRC	B		CB08		---
---		RRC	C		CB09		---
---		RRC	D		CB0A		---
---		RRC	E		CB0B		---
---		RRC	H		CB0C		---
---		RRC	L		CB0D		---
---		RRC	(HL)		CB0E		---
---		RRC	(IX+index)	DDCBindex0E	---
---		RRC	(IY+index)	FDCBindex0E	---

---		RL	A		CB1F		---
---		RL	B		CB18		---
---		RL	C		CB19		---
---		RL	D		CB1A		---
---		RL	E		CB1B		---
---		RL	H		CB1C		---
---		RL	L		CB1D		---
---		RL	(HL)		CB1E		---
---		RL	(IX+index)	DDCBindex1E	---
---		RL	(IY+index)	FDCBindex1E	---


8080/Z80  Instruction  Set					Page 10

Logical Byte Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

ANA	A	AND	A		A7		A <- A AND A
ANA	B	AND	B		A0		A <- A AND B
ANA	C	AND	C		A1		A <- A AND C
ANA	D	AND	D		A2		A <- A AND D
ANA	E	AND	E		A3		A <- A AND E
ANA	H	AND	H		A4		A <- A AND H
ANA	L	AND	L		A5		A <- A AND L
ANA	M	AND	(HL)		A6		A <- A AND (HL)
---		AND	(IX+index)	DDA6index	A <- A AND (IX+index)
---		AND	(IY+index)	FDA6index	A <- A AND (IY+index)
ANI	byte	AND	byte		E6byte		A <- A AND byte

XRA	A	XOR	A		AF		A <- A XOR A
XRA	B	XOR	B		A8		A <- A XOR B
XRA	C	XOR	C		A9		A <- A XOR C
XRA	D	XOR	D		AA		A <- A XOR D
XRA	E	XOR	E		AB		A <- A XOR E
XRA	H	XOR	H		AC		A <- A XOR H
XRA	L	XOR	L		AD		A <- A XOR L
XRA	M	XOR	(HL)		AE		A <- A XOR (HL)
---		XOR	(IX+index)	DDAEindex	A <- A XOR (IX+index)
---		XOR	(IY+index)	FDAEindex	A <- A XOR (IY+index)
XRI	byte	XOR	byte		EEbyte		A <- A XOR byte

ORA	A	OR	A		B7		A <- A OR A
ORA	B	OR	B		B0		A <- A OR B
ORA	C	OR	C		B1		A <- A OR C
ORA	D	OR	D		B2		A <- A OR D
ORA	E	OR	E		B3		A <- A OR E
ORA	H	OR	H		B4		A <- A OR H
ORA	L	OR	L		B5		A <- A OR L
ORA	M	OR	(HL)		B6		A <- A OR (HL)
---		OR	(IX+index)	DDB6index	A <- A OR (IX+index)
---		OR	(IY+index)	FDB6index	A <- A OR (IY+index)
ORI	byte	OR	byte		F6byte		A <- A OR byte

CMP	A	CP	A		BF		A - A
CMP	B	CP	B		B8		A - B
CMP	C	CP	C		B9		A - C
CMP	D	CP	D		BA		A - D
CMP	E	CP	E		BB		A - E
CMP	H	CP	H		BC		A - H
CMP	L	CP	L		BD		A - L
CMP	M	CP	(HL)		BE		A - (HL)
---		CP	(IX+index)	DDBEindex	A - (IX+index)
---		CP	(IY+index)	FDBEindex	A - (IY+index)
CPI	byte	CP	byte		FEbyte		A - byte
---		CPI			EDA1	A - (HL);HL <- HL+1;BC <- BC-1
---		CPIR			EDB1	A - (HL);HL <- HL+1;BC <- BC-1
---		CPD			EDA9	A - (HL);HL <- HL-1;BC <- BC-1
---		CPDR			EDB9	A - (HL);HL <- HL-1;BC <- BC-1


8080/Z80  Instruction  Set					Page 11

Branch Control/Program Counter Load Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

JMP	address	JP	address		C3address	PC <- address
JNZ	address	JP	NZ,address	C2address   	If NZ, PC <- address
JZ	address	JP	Z,address	CAaddress	If  Z, PC <- address
JNC	address	JP	NC,address	D2address	If NC, PC <- address
JC	address	JP	C,address	DAaddress	If  C, PC <- address
JPO	address	JP	PO,address	E2address	If PO, PC <- address
JPE	address	JP	PE,address	EAaddress	If PE, PC <- address
JP	address	JP	P,address	F2address	If  P, PC <- address
JM	address	JP	M,address	FAaddress	If  M, PC <- address
PCHL		JP	(HL)		E9		PC <- HL
---		JP	(IX)		DDE9		PC <- IX
---		JP	(IY)		FDE9		PC <- IY
---		JR	index		18index		PC <- PC + index
---		JR	NZ,index	20index	      If NZ, PC <- PC + index
---		JR	Z,index		28index	      If  Z, PC <- PC + index
---		JR	NC,index	30index	      If NC, PC <- PC + index
---		JR	C,index		38index	      If  C, PC <- PC + index
---		DJNZ	index		10index	    B <- B - 1;
						    If B > 0, PC <- PC + index

CALL	address	CALL	address		CDaddress  (SP-1) <- PCh;(SP-2) <- PCl
						   SP <- SP - 2;PX <- address
CNZ	address	CALL	NZ,address	C4address	If NZ, CALL address
CZ	address	CALL	Z,address	CCaddress	If  Z, CALL address
CNC	address	CALL	NC,address	D4address	If NC, CALL address
CC	address	CALL	C,address	DCaddress	If  C, CALL address
CPO	address	CALL	PO,address	E4address	If PO, CALL address
CPE	address	CALL	PE,address	ECaddress	If PE, CALL address
CP	address	CALL	P,address	F4address	If  P, CALL address
CM	address	CALL	M,address	FCaddress	If  M, CALL address


RET		RET			C9	    PCl <- (SP);PCh <- (SP+1)
							SP <- SP + 2
RNZ		RET	NZ		C0		If NZ, RET
RZ		RET	Z		C8		If  Z, RET
RNC		RET	NC		D0		If NC, RET
RC		RET	C		D8		If  C, RET
RPO		RET	PO		E0		If PO, RET
RPE		RET	PE		E8		If PE, RET
RP		RET	P		F0		If  P, RET
RM		RET	M		F8		If  M, RET
---		RETI			ED4D		Return from Interrupt
---		RETN			ED45		IFF1 <- IFF2;RETI


RST	0	RST	0		C7		CALL	0
RST	1	RST	8		CF		CALL	8
RST	2	RST	10H		D7		CALL	10H
RST	3	RST	18H		DF		CALL	18H
RST	4	RST	20H		E7		CALL	20H
RST	5	RST	28H		EF		CALL	28H
RST	6	RST	30H		F7		CALL	30H
RST	7	RST	38H		FF		CALL	38H


8080/Z80  Instruction  Set					Page 12

Stack Operation Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

PUSH	B	PUSH	BC		C5	(SP-2) <- C; (SP-1) <- B;
						SP <- SP - 2
PUSH	D	PUSH	DE		D5	(SP-2) <- E; (SP-1) <- D;
						SP <- SP - 2
PUSH	H	PUSH	HL		E5	(SP-2) <- L; (SP-1) <- H;
						SP <- SP - 2
PUSH	PSW	PUSH	AF		F5	(SP-2) <- Flags; (SP-1) <- A;
						SP <- SP - 2
---		PUSH	IX		DDE5	(SP-2) <- IXl; (SP-1) <- IXh
						SP <- SP - 2
---		PUSH	IY		FDE5	(SP-2) <- IYl; (SP-1) <- IYh
						SP <- SP - 2

POP	B	POP	BC		C1	B <- (SP+1); C <- (SP);
						SP <- SP + 2
POP	D	POP	DE		D1	D <- (SP+1); E <- (SP);
						SP <- SP + 2
POP	H	POP	HL		E1	H <- (SP+1); L <- (SP);
						SP <- SP + 2
POP	PSW	POP	AF		F1	A <- (SP+1); Flags <- (SP);
						SP <- SP + 2
---		POP	IX		DDE1	IXh <- (SP+1); IXl <- (SP);
						SP <- SP + 2
---		POP	IY		FDE1	IYh <- (SP+1); IYl <- (SP);


Input/Output Instructions

IN	byte	IN	A,(byte)	DBbyte		A <- [byte]
---		IN	A,(C)		ED78		A <- [C]
---		IN	B,(C)		ED40		B <- [C]
---		IN	C,(C)		ED48		C <- [C]
---		IN	D,(C)		ED50		D <- [C]
---		IN	E,(C)		ED58		E <- [C]
---		IN	H,(C)		ED60		H <- [C]
---		IN	L,(C)		ED68		L <- [C]
---		INI			EDA2  (HL) <- [C];B <- B-1;HL <- HL+1
---		INIR			EDB2  (HL) <- [C];B <- B-1;HL <- HL+1
---		IND			EDAA  (HL) <- [C];B <- B-1;HL <- HL-1
---		INDR			EDBA  (HL) <- [C];B <- B-1;HL <- HL-1

OUT	byte	OUT	(byte),A	D320		[byte] <- A
---		OUT	(C),A		ED79		[C] <- A
---		OUT	(C),B		ED41		[C] <- B
---		OUT	(C),C		ED49		[C] <- C
---		OUT	(C),D		ED51		[C] <- D
---		OUT	(C),E		ED59		[C] <- E
---		OUT	(C),H		ED61		[C] <- H
---		OUT	(C),L		ED69		[C] <- L
---		OUTI			EDA3  [C] <- (HL);B <- B-1;HL <- HL+1
---		OTIR			EDB3  [C] <- (HL);B <- B-1;HL <- HL+1
---		OUTD			EDAB  [C] <- (HL);B <- B-1;HL <- HL-1
---		OTDR			EDBB  [C] <- (HL);B <- B-1;HL <- HL-1


8080/Z80  Instruction  Set					Page 13

Data Transfer Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		LDI			EDA0	(DE) <- (HL);HL <- HL+1
						DE <- DE+1; BC <- BC-1
---		LDIR			EDB0	(DE) <- (HL);HL <- HL+1
						DE <- DE+1; BC <- BC-1
---		LDD			EDA8	(DE) <- (HL);HL <- HL-1
						DE <- DE-1; BC <- BC-1
---		LDDR			EDB8	(DE) <- (HL);HL <- HL-1
						DE <- DE-1; BC <- BC-1

Bit Manipulation Instructions (Z80 Only)

---		BIT	0,A		CB47		Z flag <- NOT 0b
---		BIT	0,B		CB40		Z flag <- NOT 0b
---		BIT	0,C		CB41		Z flag <- NOT 0b
---		BIT	0,D		CB42		Z flag <- NOT 0b
---		BIT	0,E		CB43		Z flag <- NOT 0b
---		BIT	0,H		CB44		Z flag <- NOT 0b
---		BIT	0,L		CB45		Z flag <- NOT 0b
---		BIT	0,(HL)		CB46		Z flag <- NOT 0b
---		BIT	0,(IX+index)	DDCBindex46	Z flag <- NOT 0b
---		BIT	0,(IY+index)	FDCBindex46	Z flag <- NOT 0b

---		BIT	1,A		CB4F		Z flag <- NOT 1b
---		BIT	1,B		CB48		Z flag <- NOT 1b
---		BIT	1,C		CB49		Z flag <- NOT 1b
---		BIT	1,D		CB4A		Z flag <- NOT 1b
---		BIT	1,E		CB4B		Z flag <- NOT 1b
---		BIT	1,H		CB4C		Z flag <- NOT 1b
---		BIT	1,L		CB4D		Z flag <- NOT 1b
---		BIT	1,(HL)		CB4E		Z flag <- NOT 1b
---		BIT	1,(IX+index)	DDCBindex4E	Z flag <- NOT 1b
---		BIT	1,(IY+index)	FDCBindex4E	Z flag <- NOT 1b

---		BIT	2,A		CB57		Z flag <- NOT 2b
---		BIT	2,B		CB50		Z flag <- NOT 2b
---		BIT	2,C		CB51		Z flag <- NOT 2b
---		BIT	2,D		CB52		Z flag <- NOT 2b
---		BIT	2,E		CB53		Z flag <- NOT 2b
---		BIT	2,H		CB54		Z flag <- NOT 2b
---		BIT	2,L		CB55		Z flag <- NOT 2b
---		BIT	2,(HL)		CB56		Z flag <- NOT 2b
---		BIT	2,(IX+index)	DDCBindex56	Z flag <- NOT 2b
---		BIT	2,(IY+index)	FDCBindex56	Z flag <- NOT 2b

---		BIT	3,A		CB5F		Z flag <- NOT 3b
---		BIT	3,B		CB58		Z flag <- NOT 3b
---		BIT	3,C		CB59		Z flag <- NOT 3b
---		BIT	3,D		CB5A		Z flag <- NOT 3b
---		BIT	3,E		CB5B		Z flag <- NOT 3b
---		BIT	3,H		CB5C		Z flag <- NOT 3b
---		BIT	3,L		CB5D		Z flag <- NOT 3b
---		BIT	3,(HL)		CB5E		Z flag <- NOT 3b
---		BIT	3,(IX+index)	DDCBindex5E	Z flag <- NOT 3b
---		BIT	3,(IY+index)	FDCBindex5E	Z flag <- NOT 3b



8080/Z80  Instruction  Set					Page 14

Bit Manipulation Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		BIT	4,A		CB67		Z flag <- NOT 4b
---		BIT	4,B		CB60		Z flag <- NOT 4b
---		BIT	4,C		CB61		Z flag <- NOT 4b
---		BIT	4,D		CB62		Z flag <- NOT 4b
---		BIT	4,E		CB63		Z flag <- NOT 4b
---		BIT	4,H		CB64		Z flag <- NOT 4b
---		BIT	4,L		CB65		Z flag <- NOT 4b
---		BIT	4,(HL)		CB66		Z flag <- NOT 4b
---		BIT	4,(IX+index)	DDCBindex66	Z flag <- NOT 4b
---		BIT	4,(IY+index)	FDCBindex66	Z flag <- NOT 4b

---		BIT	5,A		CB6F		Z flag <- NOT 5b
---		BIT	5,B		CB68		Z flag <- NOT 5b
---		BIT	5,C		CB69		Z flag <- NOT 5b
---		BIT	5,D		CB6A		Z flag <- NOT 5b
---		BIT	5,E		CB6B		Z flag <- NOT 5b
---		BIT	5,H		CB6C		Z flag <- NOT 5b
---		BIT	5,L		CB6D		Z flag <- NOT 5b
---		BIT	5,(HL)		CB6E		Z flag <- NOT 5b
---		BIT	5,(IX+index)	DDCBindex6E	Z flag <- NOT 5b
---		BIT	5,(IY+index)	FDCBindex6E	Z flag <- NOT 5b

---		BIT	6,A		CB77		Z flag <- NOT 6b
---		BIT	6,B		CB70		Z flag <- NOT 6b
---		BIT	6,C		CB71		Z flag <- NOT 6b
---		BIT	6,D		CB72		Z flag <- NOT 6b
---		BIT	6,E		CB73		Z flag <- NOT 6b
---		BIT	6,H		CB74		Z flag <- NOT 6b
---		BIT	6,L		CB75		Z flag <- NOT 6b
---		BIT	6,(HL)		CB76		Z flag <- NOT 6b
---		BIT	6,(IX+index)	DDCBindex76	Z flag <- NOT 6b
---		BIT	6,(IY+index)	FDCBindex76	Z flag <- NOT 6b

---		BIT	7,A		CB7F		Z flag <- NOT 7b
---		BIT	7,B		CB78		Z flag <- NOT 7b
---		BIT	7,C		CB79		Z flag <- NOT 7b
---		BIT	7,D		CB7A		Z flag <- NOT 7b
---		BIT	7,E		CB7B		Z flag <- NOT 7b
---		BIT	7,H		CB7C		Z flag <- NOT 7b
---		BIT	7,L		CB7D		Z flag <- NOT 7b
---		BIT	7,(HL)		CB7E		Z flag <- NOT 7b
---		BIT	7,(IX+index)	DDCBindex7E	Z flag <- NOT 7b
---		BIT	7,(IY+index)	FDCBindex7E	Z flag <- NOT 7b


---		RES	0,A		CB87		0b <- 0
---		RES	0,B		CB80		0b <- 0
---		RES	0,C		CB81		0b <- 0
---		RES	0,D		CB82		0b <- 0
---		RES	0,E		CB83		0b <- 0
---		RES	0,H		CB84		0b <- 0
---		RES	0,L		CB85		0b <- 0
---		RES	0,(HL)		CB86		0b <- 0
---		RES	0,(IX+index)	DDCBindex86	0b <- 0
---		RES	0,(IY+index)	FDCBindex86	0b <- 0



8080/Z80  Instruction  Set					Page 15

Bit Manipulation Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		RES	1,A		CB8F		1b <- 0
---		RES	1,B		CB88		1b <- 0
---		RES	1,C		CB89		1b <- 0
---		RES	1,D		CB8A		1b <- 0
---		RES	1,E		CB8B		1b <- 0
---		RES	1,H		CB8C		1b <- 0
---		RES	1,L		CB8D		1b <- 0
---		RES	1,(HL)		CB8E		1b <- 0
---		RES	1,(IX+index)	DDCBindex8E	1b <- 0
---		RES	1,(IY+index)	FDCBindex8E	1b <- 0

---		RES	2,A		CB97		2b <- 0
---		RES	2,B		CB90		2b <- 0
---		RES	2,C		CB91		2b <- 0
---		RES	2,D		CB92		2b <- 0
---		RES	2,E		CB93		2b <- 0
---		RES	2,H		CB94		2b <- 0
---		RES	2,L		CB95		2b <- 0
---		RES	2,(HL)		CB96		2b <- 0
---		RES	2,(IX+index)	DDCBindex96	2b <- 0
---		RES	2,(IY+index)	FDCBindex96	2b <- 0

---		RES	3,A		CB9F		3b <- 0
---		RES	3,B		CB98		3b <- 0
---		RES	3,C		CB99		3b <- 0
---		RES	3,D		CB9A		3b <- 0
---		RES	3,E		CB9B		3b <- 0
---		RES	3,H		CB9C		3b <- 0
---		RES	3,L		CB9D		3b <- 0
---		RES	3,(HL)		CB9E		3b <- 0
---		RES	3,(IX+index)	DDCBindex9E	3b <- 0
---		RES	3,(IY+index)	FDCBindex9E	3b <- 0

---		RES	4,A		CBA7		4b <- 0
---		RES	4,B		CBA0		4b <- 0
---		RES	4,C		CBA1		4b <- 0
---		RES	4,D		CBA2		4b <- 0
---		RES	4,E		CBA3		4b <- 0
---		RES	4,H		CBA4		4b <- 0
---		RES	4,L		CBA5		4b <- 0
---		RES	4,(HL)		CBA6		4b <- 0
---		RES	4,(IX+index)	DDCBindexA6	4b <- 0
---		RES	4,(IY+index)	FDCBindexA6	4b <- 0

---		RES	5,A		CBAF		5b <- 0
---		RES	5,B		CBA8		5b <- 0
---		RES	5,C		CBA9		5b <- 0
---		RES	5,D		CBAA		5b <- 0
---		RES	5,E		CBAB		5b <- 0
---		RES	5,H		CBAC		5b <- 0
---		RES	5,L		CBAD		5b <- 0
---		RES	5,(HL)		CBAE		5b <- 0
---		RES	5,(IX+index)	DDCBindexAE	5b <- 0
---		RES	5,(IY+index)	FDCBindexAE	5b <- 0



8080/Z80  Instruction  Set					Page 16

Bit Manipulation Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		RES	6,A		CBB7		6b <- 0
---		RES	6,B		CBB0		6b <- 0
---		RES	6,C		CBB1		6b <- 0
---		RES	6,D		CBB2		6b <- 0
---		RES	6,E		CBB3		6b <- 0
---		RES	6,H		CBB4		6b <- 0
---		RES	6,L		CBB5		6b <- 0
---		RES	6,(HL)		CBB6		6b <- 0
---		RES	6,(IX+index)	DDCBindexB6	6b <- 0
---		RES	6,(IY+index)	FDCBindexB6	6b <- 0

---		RES	7,A		CBBF		7b <- 0
---		RES	7,B		CBB8		7b <- 0
---		RES	7,C		CBB9		7b <- 0
---		RES	7,D		CBBA		7b <- 0
---		RES	7,E		CBBB		7b <- 0
---		RES	7,H		CBBC		7b <- 0
---		RES	7,L		CBBD		7b <- 0
---		RES	7,(HL)		CBBE		7b <- 0
---		RES	7,(IX+index)	DDCBindexBE	7b <- 0
---		RES	7,(IY+index)	FDCBindexBE	7b <- 0

---		SET	0,A		CBC7		0b <- 1
---		SET	0,B		CBC0		0b <- 1
---		SET	0,C		CBC1		0b <- 1
---		SET	0,D		CBC2		0b <- 1
---		SET	0,E		CBC3		0b <- 1
---		SET	0,H		CBC4		0b <- 1
---		SET	0,L		CBC5		0b <- 1
---		SET	0,(HL)		CBC6		0b <- 1
---		SET	0,(IX+index)	DDCBindexC6	0b <- 1
---		SET	0,(IY+index)	FDCBindexC6	0b <- 1

---		SET	1,A		CBCF		1b <- 1
---		SET	1,B		CBC8		1b <- 1
---		SET	1,C		CBC9		1b <- 1
---		SET	1,D		CBCA		1b <- 1
---		SET	1,E		CBCB		1b <- 1
---		SET	1,H		CBCC		1b <- 1
---		SET	1,L		CBCD		1b <- 1
---		SET	1,(HL)		CBCE		1b <- 1
---		SET	1,(IX+index)	DDCBindexCE	1b <- 1
---		SET	1,(IY+index)	FDCBindexCE	1b <- 1

---		SET	2,A		CBD7		2b <- 1
---		SET	2,B		CBD0		2b <- 1
---		SET	2,C		CBD1		2b <- 1
---		SET	2,D		CBD2		2b <- 1
---		SET	2,E		CBD3		2b <- 1
---		SET	2,H		CBD4		2b <- 1
---		SET	2,L		CBD5		2b <- 1
---		SET	2,(HL)		CBD6		2b <- 1
---		SET	2,(IX+index)	DDCBindexD6	2b <- 1
---		SET	2,(IY+index)	FDCBindexD6	2b <- 1



8080/Z80  Instruction  Set					Page 17

Bit Manipulation Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		SET	3,A		CBDF		3b <- 1
---		SET	3,B		CBD8		3b <- 1
---		SET	3,C		CBD9		3b <- 1
---		SET	3,D		CBDA		3b <- 1
---		SET	3,E		CBDB		3b <- 1
---		SET	3,H		CBDC		3b <- 1
---		SET	3,L		CBDD		3b <- 1
---		SET	3,(HL)		CBDE		3b <- 1
---		SET	3,(IX+index)	DDCBindexDE	3b <- 1
---		SET	3,(IY+index)	FDCBindexDE	3b <- 1

---		SET	4,A		CBE7		4b <- 1
---		SET	4,B		CBE0		4b <- 1
---		SET	4,C		CBE1		4b <- 1
---		SET	4,D		CBE2		4b <- 1
---		SET	4,E		CBE3		4b <- 1
---		SET	4,H		CBE4		4b <- 1
---		SET	4,L		CBE5		4b <- 1
---		SET	4,(HL)		CBE6		4b <- 1
---		SET	4,(IX+index)	DDCBindexE6	4b <- 1
---		SET	4,(IY+index)	FDCBindexE6	4b <- 1

---		SET	5,A		CBEF		5b <- 1
---		SET	5,B		CBE8		5b <- 1
---		SET	5,C		CBE9		5b <- 1
---		SET	5,D		CBEA		5b <- 1
---		SET	5,E		CBEB		5b <- 1
---		SET	5,H		CBEC		5b <- 1
---		SET	5,L		CBED		5b <- 1
---		SET	5,(HL)		CBEE		5b <- 1
---		SET	5,(IX+index)	DDCBindexEE	5b <- 1
---		SET	5,(IY+index)	FDCBindexEE	5b <- 1

---		SET	6,A		CBF7		6b <- 1
---		SET	6,B		CBF0		6b <- 1
---		SET	6,C		CBF1		6b <- 1
---		SET	6,D		CBF2		6b <- 1
---		SET	6,E		CBF3		6b <- 1
---		SET	6,H		CBF4		6b <- 1
---		SET	6,L		CBF5		6b <- 1
---		SET	6,(HL)		CBF6		6b <- 1
---		SET	6,(IX+index)	DDCBindexF6	6b <- 1
---		SET	6,(IY+index)	FDCBindexF6	6b <- 1

---		SET	7,A		CBFF		7b <- 1
---		SET	7,B		CBF8		7b <- 1
---		SET	7,C		CBF9		7b <- 1
---		SET	7,D		CBFA		7b <- 1
---		SET	7,E		CBFB		7b <- 1
---		SET	7,H		CBFC		7b <- 1
---		SET	7,L		CBFD		7b <- 1
---		SET	7,(HL)		CBFE		7b <- 1
---		SET	7,(IX+index)	DDCBindexFE	7b <- 1
---		SET	7,(IY+index)	FDCBindexFE	7b <- 1



8080/Z80  Instruction  Set					Page 18

Bit Shift Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		SLA	A		CB27		---
---		SLA	B		CB20		---
---		SLA	C		CB21		---
---		SLA	D		CB22		---
---		SLA	E		CB23		---
---		SLA	H		CB24		---
---		SLA	L		CB25		---
---		SLA	(HL)		CB26		---
---		SLA	(IX+index)	DDCBindex26	---
---		SLA	(IY+index)	FDCBindex26	---

---		SRA	A		CB2F		---
---		SRA	B		CB28		---
---		SRA	C		CB29		---
---		SRA	D		CB2A		---
---		SRA	E		CB2B		---
---		SRA	H		CB2C		---
---		SRA	L		CB2D		---
---		SRA	(HL)		CB2E		---
---		SRA	(IX+index)	DDCBindex2E	---
---		SRA	(IY+index)	FDCBindex2E	---

---		SRL	A		CB3F		---
---		SRL	B		CB38		---
---		SRL	C		CB39		---
---		SRL	D		CB3A		---
---		SRL	E		CB3B		---
---		SRL	H		CB3C		---
---		SRL	L		CB3D		---
---		SRL	(HL)		CB3E		---
---		SRL	(IX+index)	DDCBindex3E	---
---		SRL	(IY+index)	FDCBindex3E	---
#endif
    }
}

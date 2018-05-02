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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
    public partial class Tlcs90Disassembler : DisassemblerBase<Tlcs90Instruction>
    {
        private EndianImageReader rdr;
        private Tlcs90Architecture arch;
        private PrimitiveType dataWidth;
        private RegisterOperand byteReg;
        private RegisterOperand wordReg;
        private int backPatchOp;

        public Tlcs90Disassembler(Tlcs90Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Tlcs90Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte b))
                return null;
            this.dataWidth = null;
            this.byteReg = null;
            this.wordReg = null;
            this.backPatchOp = -1;

            var instr = Oprecs[b].Decode(b, this);
            if (instr == null)
                instr = new Tlcs90Instruction { Opcode = Opcode.invalid };
            var len = rdr.Address - addr;
            instr.Address = addr;
            instr.Length = (int) len;
            return instr;
        }

        private Tlcs90Instruction DecodeOperands(byte b, Opcode opcode, string format)
        {
            int op = 0;
            var ops = new MachineOperand[2];
            Constant c;
            PrimitiveType size;

            for (int i = 0; i < format.Length; ++i)
            {
                switch (format[i])
                {
                case ',':
                    continue;
                case 'a':
                    this.dataWidth = PrimitiveType.Byte;
                    ops[op] = new RegisterOperand(Registers.a);
                    break;
                case '@':
                    this.dataWidth = PrimitiveType.Word16;
                    ops[op] = new RegisterOperand(Registers.af_);
                    break;
                case 'A':
                    this.dataWidth = PrimitiveType.Word16;
                    ops[op] = new RegisterOperand(Registers.af);
                    break;
                case 'B':
                    this.dataWidth = PrimitiveType.Word16;
                    ops[op] = new RegisterOperand(Registers.bc);
                    break;
                case 'c':
                    ops[op] = new ConditionOperand((CondCode)(b & 0xF));
                    break;
                case 'D':
                    this.dataWidth = PrimitiveType.Word16;
                    ops[op] = new RegisterOperand(Registers.de);
                    break;
                case 'I':
                    // Immediate value
                    this.dataWidth = GetSize(format[++i]);
                    if (!rdr.TryReadLe(this.dataWidth, out c))
                        return null;
                    ops[op] = new ImmediateOperand(c);
                    break;
                case 'i': // immediate value from opcode bits
                    ops[op] = ImmediateOperand.Byte((byte)(b & 0x7));
                    break;
                case 'g':
                    Debug.Assert(byteReg != null);
                    ops[op] = this.byteReg;
                    break;
                case 'G':
                    Debug.Assert(wordReg != null);
                    ops[op] = this.wordReg;
                    break;
                case 'H':
                    ops[op] = new RegisterOperand(Registers.hl);
                    break;
                case 'J':
                    // Absolute jump.
                    size = GetSize(format[++i]);
                    if (!rdr.TryReadLe(size, out c))
                        return null;
                    ops[op] = AddressOperand.Ptr16(c.ToUInt16());
                    break;
                case 'j':
                    // relative jump
                    size = GetSize(format[++i]);
                    Address dest;
                    if (size.Size == 1)
                    {
                        if (!rdr.TryReadByte(out b))
                            return null;
                        dest = rdr.Address + (sbyte)b;
                    }
                    else
                    {
                        if (!rdr.TryReadLeInt16(out short off))
                            return null;
                        dest = rdr.Address + off;
                    }
                    ops[op] = AddressOperand.Create(dest);
                    break;
                case 'M':
                    this.dataWidth = GetSize(format[++i]);
                    ushort absAddr;
                    if (!rdr.TryReadLeUInt16(out absAddr))
                        return null;
                    ops[op] = MemoryOperand.Absolute(this.dataWidth, absAddr);
                    break;
                case 'm':
                    size = GetSize(format[++i]);
                    byte pageAddr;
                    if (!rdr.TryReadByte(out pageAddr))
                        return null;
                    ops[op] = MemoryOperand.Absolute(size, (ushort)(0xFF00|pageAddr));
                    break;
                case 'S':
                    this.dataWidth = PrimitiveType.Word16;
                    ops[op] = new RegisterOperand(Registers.sp);
                    break;
                case 'X':
                    this.dataWidth = PrimitiveType.Word16;
                    ops[op] = new RegisterOperand(Registers.ix);
                    break;
                case 'Y':
                    this.dataWidth = PrimitiveType.Word16;
                    ops[op] = new RegisterOperand(Registers.iy);
                    break;
                case 'r':
                    // Register encoded in low 3 bits of b.
                    ops[op] = new RegisterOperand(Registers.byteRegs[b & 7]);
                    dataWidth = PrimitiveType.Byte;
                    break;
                case 'x':
                    this.backPatchOp = op;
                    break;
                default: throw new NotImplementedException(string.Format("Encoding '{0}' not implemented yet.", format[i]));
                }
                ++op;
            }
            return new Tlcs90Instruction
            {
                Opcode = opcode,
                op1 = op > 0 ? ops[0] : null,
                op2 = op > 1 ? ops[1] : null,
            };
        }

        private PrimitiveType GetSize(char v)
        {
            if (v == 'b')
                return PrimitiveType.Byte;
            else if (v == 'w')
                return PrimitiveType.Word16;
            throw new NotImplementedException();
        }

        private abstract class OpRecBase
        {
            public abstract Tlcs90Instruction Decode(byte b, Tlcs90Disassembler dasm);
        }

        private class OpRec : OpRecBase
        {
            private Opcode opcode;
            private string format;

            public OpRec(Opcode opcode, string format)
            {
                this.opcode = opcode;
                this.format = format;
            }

            public override Tlcs90Instruction Decode(byte b, Tlcs90Disassembler dasm)
            {
                return dasm.DecodeOperands(b, opcode, format);
            }
        }

        private class RegOpRec : OpRecBase
        {
            private RegisterStorage regByte;
            private RegisterStorage regWord;

            public RegOpRec(RegisterStorage regByte, RegisterStorage regWord)
            {
                this.regByte = regByte;
                this.regWord = regWord;
            }

            public override Tlcs90Instruction Decode(byte b, Tlcs90Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out b))
                    return null;
                dasm.byteReg = new RegisterOperand(regByte);
                if (regWord != null)
                    dasm.wordReg = new RegisterOperand(regWord);
                return regEncodings[b].Decode(b, dasm);
            }
        }

        private class DstOpRec : OpRecBase
        {
            private string format;

            public DstOpRec(string format)
            {
                this.format = format;
            }

            public override Tlcs90Instruction Decode(byte b, Tlcs90Disassembler dasm)
            {
                RegisterStorage baseReg = null;
                RegisterStorage idxReg = null;
                ushort? absAddr = null;
                Constant offset = null;
                switch (format[0])
                {
                case 'M':
                    ushort a;
                    if (!dasm.rdr.TryReadLeUInt16(out a))
                        return null;
                    absAddr = a;
                    break;
                case 'm':
                    byte bb;
                    if (!dasm.rdr.TryReadByte(out bb))
                        return null;
                    absAddr = (ushort)(0xFF00 | bb);
                    break;
                case 'B': baseReg = Registers.bc; break;
                case 'D': baseReg = Registers.de; break;
                case 'H': baseReg = Registers.hl; break;
                case 'X': baseReg = Registers.ix; break;
                case 'Y': baseReg = Registers.iy; break;
                case 'S': baseReg = Registers.sp; break;
                case 'E':
                    switch (format[1])
                    {
                    case 'S': baseReg = Registers.sp;  break;
                    case 'X': baseReg = Registers.ix;  break;
                    case 'Y': baseReg = Registers.ix;  break;
                    case 'H': baseReg = Registers.hl; idxReg = Registers.a;  break;
                    default: throw new NotImplementedException(string.Format("Tlcs-90: dst {0}", format));
                    }
                    if (idxReg == null)
                    {
                        if (!dasm.rdr.TryReadByte(out b))
                            return null;
                        offset = Constant.SByte((sbyte)b);
                    }
                    break;
                default: throw new NotImplementedException(string.Format("Tlcs-90: dst {0}", format));
                }
                if (!dasm.rdr.TryReadByte(out b))
                    return null;
                var instr = dstEncodings[b].Decode(b, dasm);
                if (instr == null)
                    return null;

                var operand = new MemoryOperand(dasm.dataWidth)
                {
                    Base = baseReg,
                    Offset = absAddr.HasValue
                        ? Constant.UInt16(absAddr.Value)
                        : offset,
                };
                if (dasm.backPatchOp == 0)
                {
                    instr.op1 = operand;
                    if (instr.op2 != null)
                    {
                        instr.op1.Width = instr.op2.Width;
                    }
                }
                else if (dasm.backPatchOp == 1)
                {
                    if ((instr.Opcode == Opcode.jp || instr.Opcode == Opcode.call)
                        &&
                        operand.Base == null &&
                        operand.Index == null &&
                        operand.Offset != null)
                    {
                        // JP cc,(XXXX) should be JP cc,XXXX
                        instr.op2 = AddressOperand.Ptr16(operand.Offset.ToUInt16());
                        instr.op2.Width = PrimitiveType.Ptr16;
                    }
                    else
                    {
                        instr.op2 = operand;
                        instr.op2.Width = instr.op1.Width;
                    }
                }
                else
                    return null;
                return instr;
            }
        }

        private class SrcOpRec : OpRecBase
        {
            private string format;

            public SrcOpRec(string format)
            {
                this.format = format;
            }

            public override Tlcs90Instruction Decode(byte b, Tlcs90Disassembler dasm)
            {
                Tlcs90Instruction instr;
                Constant offset = null;
                RegisterStorage baseReg = null;
                RegisterStorage idxReg = null;

                switch (format[0])
                {
                case 'E':
                    switch (format[1])
                    {
                    case 'S': baseReg = Registers.sp; break;
                    case 'X': baseReg = Registers.ix; break;
                    case 'Y': baseReg = Registers.iy; break;
                    case 'H': baseReg = Registers.hl; idxReg = Registers.a;  break;
                    default: throw new NotImplementedException(string.Format("Tlcs-90: src {0}", format));
                    };
                    if (idxReg == null)
                    {
                        if (!dasm.rdr.TryReadByte(out b))
                            return null;
                        offset = Constant.SByte((sbyte)b);
                    }
                    break;
                case 'B': baseReg = Registers.bc; break;
                case 'D': baseReg = Registers.de; break;
                case 'H': baseReg = Registers.hl; break;
                case 'S': baseReg = Registers.sp; break;
                case 'X': baseReg = Registers.ix; break;
                case 'Y': baseReg = Registers.iy; break;
                case 'M':
                    ushort us;
                    if (!dasm.rdr.TryReadLeUInt16(out us))
                        return null;
                    offset = Constant.UInt16(us);
                    break;
                case 'm':
                    byte pageAddr;
                    if (!dasm.rdr.TryReadByte(out pageAddr))
                        return null;
                    offset = Constant.UInt16((ushort)(0xFF00 | pageAddr));
                    break;
                default: throw new NotImplementedException(string.Format("Tlcs-90: src {0}", format));
                }

                if (!dasm.rdr.TryReadByte(out b))
                    return null;
                instr = srcEncodings[b].Decode(b, dasm);
                if (instr == null)
                    return null;

                var operand = new MemoryOperand(dasm.dataWidth)
                {
                    Base = baseReg,
                    Index = idxReg,
                    Offset = offset
                };

                if (dasm.backPatchOp == 0)
                {
                    instr.op1 = operand;
                    if (instr.op2 != null)
                    {
                        operand.Width = instr.op2.Width;
                    }
                }
                else if (dasm.backPatchOp == 1)
                {
                    instr.op2 = operand;
                    operand.Width = instr.op1.Width;
                }
                else
                    return null;
                return instr;
            }
        }

        private static OpRecBase[] oprecs = new OpRecBase[]
        {
            // 00
            new OpRec(Opcode.nop, ""),
            new OpRec(Opcode.halt, ""),
            new OpRec(Opcode.di, ""),
            new OpRec(Opcode.ei, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.incx, "mw"),

            new OpRec(Opcode.ex, "D,H"),
            new OpRec(Opcode.ex, "A,@"),
            new OpRec(Opcode.exx, ""),
            new OpRec(Opcode.daa, "a"),

            new OpRec(Opcode.rcf, ""),
            new OpRec(Opcode.scf, ""),
            new OpRec(Opcode.ccf, ""),
            new OpRec(Opcode.decx, "mb"),

            // 10
            new OpRec(Opcode.cpl, "a"),
            new OpRec(Opcode.neg, "a"),
            new OpRec(Opcode.mul, "H,Ib"),
            new OpRec(Opcode.div, "H,Ib"),

            new OpRec(Opcode.add, "X,Iw"),
            new OpRec(Opcode.add, "Y,Iw"),
            new OpRec(Opcode.add, "S,Iw"),
            new OpRec(Opcode.ldar, "H,jw"),

            new OpRec(Opcode.djnz, "jb"),
            new OpRec(Opcode.djnz, "B,jb"),
            new OpRec(Opcode.jp, "Jw"),
            new OpRec(Opcode.jr, "jw"),

            new OpRec(Opcode.call, "Jw"),
            new OpRec(Opcode.callr, "jw"),
            new OpRec(Opcode.ret, ""),
            new OpRec(Opcode.reti, ""), 

            // 20
            new OpRec(Opcode.ld, "a,r"),
            new OpRec(Opcode.ld, "a,r"),
            new OpRec(Opcode.ld, "a,r"),
            new OpRec(Opcode.ld, "a,r"),

            new OpRec(Opcode.ld, "a,r"),
            new OpRec(Opcode.ld, "a,r"),
            new OpRec(Opcode.ld, "a,r"),
            new OpRec(Opcode.ld, "a,mb"),

            new OpRec(Opcode.ld, "r,a"),
            new OpRec(Opcode.ld, "r,a"),
            new OpRec(Opcode.ld, "r,a"),
            new OpRec(Opcode.ld, "r,a"),

            new OpRec(Opcode.ld, "r,a"),
            new OpRec(Opcode.ld, "r,a"),
            new OpRec(Opcode.ld, "r,a"),
            new OpRec(Opcode.ld, "mb,a"),

            // 30
            new OpRec(Opcode.ld, "r,Ib"),
            new OpRec(Opcode.ld, "r,Ib"),
            new OpRec(Opcode.ld, "r,Ib"),
            new OpRec(Opcode.ld, "r,Ib"),

            new OpRec(Opcode.ld, "r,Ib"),
            new OpRec(Opcode.ld, "r,Ib"),
            new OpRec(Opcode.ld, "r,Ib"),
            new OpRec(Opcode.ld, "mb,Ib"),

            new OpRec(Opcode.ld, "B,Iw"),
            new OpRec(Opcode.ld, "D,Iw"),
            new OpRec(Opcode.ld, "H,Iw"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.ld, "X,Iw"),
            new OpRec(Opcode.ld, "Y,Iw"),
            new OpRec(Opcode.ld, "S,Iw"),
            new OpRec(Opcode.ldw, "mw,Iw"),

            // 40
            new OpRec(Opcode.ld, "H,B"),
            new OpRec(Opcode.ld, "H,D"),
            new OpRec(Opcode.ld, "H,H"),    // lolwut
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.ld, "H,X"),
            new OpRec(Opcode.ld, "H,Y"),
            new OpRec(Opcode.ld, "H,S"),
            new OpRec(Opcode.ld, "H,mw"),

            new OpRec(Opcode.ld, "B,H"),
            new OpRec(Opcode.ld, "D,H"),
            new OpRec(Opcode.ld, "H,H"),    // lolwut
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.ld, "X,H"),
            new OpRec(Opcode.ld, "Y,H"),
            new OpRec(Opcode.ld, "S,H"),
            new OpRec(Opcode.ld, "mw,H"),

            // 50
            new OpRec(Opcode.push, "B"),
            new OpRec(Opcode.push, "D"),
            new OpRec(Opcode.push, "H"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.push, "X"),
            new OpRec(Opcode.push, "Y"),
            new OpRec(Opcode.push, "A"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.pop, "B"),
            new OpRec(Opcode.pop, "D"),
            new OpRec(Opcode.pop, "H"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.pop, "X"),
            new OpRec(Opcode.pop, "Y"),
            new OpRec(Opcode.pop, "A"),
            new OpRec(Opcode.invalid, ""),

            // 60
            new OpRec(Opcode.add, "a,mb"),
            new OpRec(Opcode.adc, "a,mb"),
            new OpRec(Opcode.sub, "a,mb"),
            new OpRec(Opcode.sbc, "a,mb"),

            new OpRec(Opcode.and, "a,mb"),
            new OpRec(Opcode.xor, "a,mb"),
            new OpRec(Opcode.or,  "a,mb"),
            new OpRec(Opcode.cp,  "a,mb"),

            new OpRec(Opcode.add, "a,Ib"),
            new OpRec(Opcode.adc, "a,Ib"),
            new OpRec(Opcode.sub, "a,Ib"),
            new OpRec(Opcode.sbc, "a,Ib"),

            new OpRec(Opcode.and, "a,Ib"),
            new OpRec(Opcode.xor, "a,Ib"),
            new OpRec(Opcode.or,  "a,Ib"),
            new OpRec(Opcode.cp,  "a,Ib"),

            // 70
            new OpRec(Opcode.add, "H,Mw"),
            new OpRec(Opcode.adc, "H,Mw"),
            new OpRec(Opcode.sub, "H,Mw"),
            new OpRec(Opcode.sbc, "H,Mw"),

            new OpRec(Opcode.and, "H,Mw"),
            new OpRec(Opcode.xor, "H,Mw"),
            new OpRec(Opcode.or,  "H,Mw"),
            new OpRec(Opcode.cp,  "H,Mw"),

            new OpRec(Opcode.add, "H,Iw"),
            new OpRec(Opcode.adc, "H,Iw"),
            new OpRec(Opcode.sub, "H,Iw"),
            new OpRec(Opcode.sbc, "H,Iw"),

            new OpRec(Opcode.and, "H,Iw"),
            new OpRec(Opcode.xor, "H,Iw"),
            new OpRec(Opcode.or,  "H,Iw"),
            new OpRec(Opcode.cp,  "H,Iw"),

            // 80
            new OpRec(Opcode.inc, "r"),
            new OpRec(Opcode.inc, "r"),
            new OpRec(Opcode.inc, "r"),
            new OpRec(Opcode.inc, "r"),

            new OpRec(Opcode.inc, "r"),
            new OpRec(Opcode.inc, "r"),
            new OpRec(Opcode.inc, "r"),
            new OpRec(Opcode.inc, "mb"),

            new OpRec(Opcode.dec, "r"),
            new OpRec(Opcode.dec, "r"),
            new OpRec(Opcode.dec, "r"),
            new OpRec(Opcode.dec, "r"),

            new OpRec(Opcode.dec, "r"),
            new OpRec(Opcode.dec, "r"),
            new OpRec(Opcode.dec, "r"),
            new OpRec(Opcode.dec, "mb"),

            // 90
            new OpRec(Opcode.inc, "B"),
            new OpRec(Opcode.inc, "D"),
            new OpRec(Opcode.inc, "H"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.inc, "X"),
            new OpRec(Opcode.inc, "Y"),
            new OpRec(Opcode.inc, "A"),
            new OpRec(Opcode.incw, "mw"),

            new OpRec(Opcode.dec, "B"),
            new OpRec(Opcode.dec, "D"),
            new OpRec(Opcode.dec, "H"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.dec, "X"),
            new OpRec(Opcode.dec, "Y"),
            new OpRec(Opcode.dec, "A"),
            new OpRec(Opcode.decw, "mw"),

            // A0
            new OpRec(Opcode.rrc, ""),
            new OpRec(Opcode.rrc, ""),
            new OpRec(Opcode.rl, ""),
            new OpRec(Opcode.rr, ""),

            new OpRec(Opcode.sla, ""),
            new OpRec(Opcode.sra, ""),
            new OpRec(Opcode.sll, ""),
            new OpRec(Opcode.srl, ""),

            new OpRec(Opcode.bit, "i,mb"),
            new OpRec(Opcode.bit, "i,mb"),
            new OpRec(Opcode.bit, "i,mb"),
            new OpRec(Opcode.bit, "i,mb"),

            new OpRec(Opcode.bit, "i,mb"),
            new OpRec(Opcode.bit, "i,mb"),
            new OpRec(Opcode.bit, "i,mb"),
            new OpRec(Opcode.bit, "i,mb"),

            // B0
            new OpRec(Opcode.res, "i,mb"),
            new OpRec(Opcode.res, "i,mb"),
            new OpRec(Opcode.res, "i,mb"),
            new OpRec(Opcode.res, "i,mb"),

            new OpRec(Opcode.res, "i,mb"),
            new OpRec(Opcode.res, "i,mb"),
            new OpRec(Opcode.res, "i,mb"),
            new OpRec(Opcode.res, "i,mb"),

            new OpRec(Opcode.set, "i,mb"),
            new OpRec(Opcode.set, "i,mb"),
            new OpRec(Opcode.set, "i,mb"),
            new OpRec(Opcode.set, "i,mb"),

            new OpRec(Opcode.set, "i,mb"),
            new OpRec(Opcode.set, "i,mb"),
            new OpRec(Opcode.set, "i,mb"),
            new OpRec(Opcode.set, "i,mb"),

            // C0
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),

            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),

            new OpRec(Opcode.jr, "jb"),
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),

            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),
            new OpRec(Opcode.jr, "c,jb"),

            // D0
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // E0
            new SrcOpRec("B"),
            new SrcOpRec("D"),
            new SrcOpRec("H"),
            new SrcOpRec("M"),

            new SrcOpRec("X"),
            new SrcOpRec("Y"),
            new SrcOpRec("S"),
            new SrcOpRec("m"),

            new DstOpRec("B"),
            new DstOpRec("D"),
            new DstOpRec("H"),
            new DstOpRec("M"),

            new DstOpRec("X"),
            new DstOpRec("Y"),
            new DstOpRec("S"),
            new DstOpRec("m"),

            // F0
            new SrcOpRec("EX"),
            new SrcOpRec("EY"),
            new SrcOpRec("ES"),
            new SrcOpRec("EH"),

            new DstOpRec("EX"),
            new DstOpRec("EY"),
            new DstOpRec("ES"),
            new DstOpRec("EH"),

            new RegOpRec(Registers.b, Registers.bc),
            new RegOpRec(Registers.c, Registers.de),
            new RegOpRec(Registers.d, Registers.hl),
            new RegOpRec(Registers.e, null),

            new RegOpRec(Registers.h, Registers.ix),
            new RegOpRec(Registers.l, Registers.iy),
            new RegOpRec(Registers.a, Registers.sp),
            new OpRec(Opcode.swi, ""), 
        };
        private Address addr;

        private static OpRecBase[] Oprecs
        {
            get
            {
                return oprecs;
            }

            set
            {
                oprecs = value;
            }
        }
    }
}

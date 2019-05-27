#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
        private readonly EndianImageReader rdr;
        private readonly Tlcs90Architecture arch;
        private readonly List<MachineOperand> ops;
        private PrimitiveType dataWidth;
        private RegisterOperand byteReg;
        private RegisterOperand wordReg;
        private int backPatchOp;

        public Tlcs90Disassembler(Tlcs90Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
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
            this.ops.Clear();

            var instr = Oprecs[b].Decode(b, this);
            if (instr == null)
                instr = new Tlcs90Instruction { Opcode = Opcode.invalid, InstructionClass = InstrClass.Invalid };
            var len = rdr.Address - addr;
            instr.Address = addr;
            instr.Length = (int) len;
            return instr;
        }

        #region Mutators

        private static bool a(uint b, Tlcs90Disassembler dasm) {
            dasm.dataWidth = PrimitiveType.Byte;
            dasm.ops.Add(new RegisterOperand(Registers.a));
            return true;
        }

        // '@'
        private static bool af_(uint b, Tlcs90Disassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Word16;
            dasm.ops.Add(new RegisterOperand(Registers.af_));
            return true;
        }

            private static bool A(uint b, Tlcs90Disassembler dasm) {
                    dasm.dataWidth = PrimitiveType.Word16;
                    dasm.ops.Add(new RegisterOperand(Registers.af));
                    return true;
                }

            private static bool B(uint b, Tlcs90Disassembler dasm) {
                    dasm.dataWidth = PrimitiveType.Word16;
                    dasm.ops.Add(new RegisterOperand(Registers.bc));
                    return true;
                }

            private static bool c(uint b, Tlcs90Disassembler dasm) {
                    dasm.ops.Add(new ConditionOperand((CondCode)(b & 0xF)));
                    return true;
                }

            private static bool D(uint b, Tlcs90Disassembler dasm) {
                    dasm.dataWidth = PrimitiveType.Word16;
                    dasm.ops.Add(new RegisterOperand(Registers.de));
                    return true;
                }

        private static Mutator<Tlcs90Disassembler> I(PrimitiveType size)
        {
            return (b, dasm) => {
                // Immediate value
                dasm.dataWidth = size;
                if (!dasm.rdr.TryReadLe(dasm.dataWidth, out var c))
                    return false;
                dasm.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }
        private static readonly Mutator<Tlcs90Disassembler> Ib = I(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs90Disassembler> Iw = I(PrimitiveType.Word16);

            private static bool i(uint b, Tlcs90Disassembler dasm) { // immediate value from opcode bits
                    dasm.ops.Add(ImmediateOperand.Byte((byte)(b & 0x7)));
                    return true;
                }

            private static bool g(uint b, Tlcs90Disassembler dasm) {
                    Debug.Assert(dasm.byteReg != null);
                    dasm.ops.Add(dasm.byteReg);
                    return true;
                }

            private static bool G(uint b, Tlcs90Disassembler dasm) {
                    Debug.Assert(dasm.wordReg != null);
                    dasm.ops.Add(dasm.wordReg);
                    return true;
                }

            private static bool H(uint b, Tlcs90Disassembler dasm)
        {
                    dasm.ops.Add(new RegisterOperand(Registers.hl));
                    return true;
                }

        // Absolute jump.
        private static Mutator<Tlcs90Disassembler> J(PrimitiveType size)
        {
            return (b, dasm) =>
            {
                if (!dasm.rdr.TryReadLe(size, out var c))
                    return false;
                dasm.ops.Add(AddressOperand.Ptr16(c.ToUInt16()));
                return true;
            };
        }
        private static Mutator<Tlcs90Disassembler> Jb = J(PrimitiveType.Byte);
        private static Mutator<Tlcs90Disassembler> Jw = J(PrimitiveType.Word16);

        // relative jump
        private static bool jb(uint u, Tlcs90Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var dest = dasm.rdr.Address + (sbyte) b;
            dasm.ops.Add(AddressOperand.Create(dest));
            return true;
        }

        private static bool jw(uint b, Tlcs90Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeInt16(out short off))
                return false;
            var dest = dasm.rdr.Address + off;
            dasm.ops.Add(AddressOperand.Create(dest));
            return true;
        }

        private static Mutator<Tlcs90Disassembler> M(PrimitiveType size)
        {
            return (u, d) =>
            {
                d.dataWidth = size;
                if (!d.rdr.TryReadLeUInt16(out ushort absAddr))
                    return false;
                d.ops.Add(MemoryOperand.Absolute(d.dataWidth, absAddr));
                return true;
            };
        }
        private static readonly Mutator<Tlcs90Disassembler> Mb = M(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs90Disassembler> Mw = M(PrimitiveType.Word16);

        private static Mutator<Tlcs90Disassembler> m(PrimitiveType size)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadByte(out byte pageAddr))
                    return false;
                d.ops.Add(MemoryOperand.Absolute(size, (ushort) (0xFF00 | pageAddr)));
                return true;
            };
        }
        private static readonly Mutator<Tlcs90Disassembler> mb = m(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs90Disassembler> mw = m(PrimitiveType.Word16);

        private static bool S(uint b, Tlcs90Disassembler dasm) {
            dasm.dataWidth = PrimitiveType.Word16;
            dasm.ops.Add(new RegisterOperand(Registers.sp));
            return true;
        }

        private static bool X(uint b, Tlcs90Disassembler dasm) {
            dasm.dataWidth = PrimitiveType.Word16;
            dasm.ops.Add(new RegisterOperand(Registers.ix));
            return true;
        }

        private static bool Y(uint b, Tlcs90Disassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Word16;
            dasm.ops.Add(new RegisterOperand(Registers.iy));
            return true;
        }

        private static bool r(uint b, Tlcs90Disassembler dasm)
        {
            // Register encoded in low 3 bits of b.
            dasm.dataWidth = PrimitiveType.Byte;
            dasm.ops.Add(new RegisterOperand(Registers.byteRegs[b & 7]));
            return true;
        }

        private static bool x(uint b, Tlcs90Disassembler dasm) {
            dasm.backPatchOp = dasm.ops.Count;
            return true;
        }

        #endregion

        private PrimitiveType GetSize(char v)
        {
            if (v == 'b')
                return PrimitiveType.Byte;
            else if (v == 'w')
                return PrimitiveType.Word16;
            throw new NotImplementedException();
        }

        private abstract class Decoder
        {
            public abstract Tlcs90Instruction Decode(byte b, Tlcs90Disassembler dasm);
        }

        private class InstrDecoder : Decoder
        {
            private Opcode opcode;
            private InstrClass iclass;
            private Mutator<Tlcs90Disassembler>[] mutators;

            public InstrDecoder(Opcode opcode, InstrClass iclass, Mutator<Tlcs90Disassembler> [] mutators)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public override Tlcs90Instruction Decode(byte b, Tlcs90Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(b, dasm))
                        return new Tlcs90Instruction
                        {
                            Opcode = Opcode.invalid,
                            InstructionClass = InstrClass.Invalid
                        };
                }
                return new Tlcs90Instruction
                {
                    Opcode = opcode,
                    InstructionClass = iclass,
                    op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                    op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                };
            }
        }

        private class RegOpRec : Decoder
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

        private class DstOpRec : Decoder
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
                    instr.op2 = instr.op1;
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

        private class SrcOpRec : Decoder
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
                    instr.op2 = instr.op1;
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

        private class InvalidDecoder : Decoder
        {
            public override Tlcs90Instruction Decode(byte b, Tlcs90Disassembler dasm)
            {
                return new Tlcs90Instruction
                {
                    Opcode = Opcode.invalid,
                    InstructionClass = InstrClass.Invalid
                };
            }
        }

        private static InstrDecoder Instr(Opcode opcode, params Mutator<Tlcs90Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, mutators);
        }

        private static InstrDecoder Instr(Opcode opcode, InstrClass iclass, params Mutator<Tlcs90Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, mutators);
        }

        private static Decoder invalid = new InvalidDecoder();

        private static Decoder[] decoders = new Decoder[256]
        {
            // 00
            Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding|InstrClass.Zero),
            Instr(Opcode.halt, InstrClass.Terminates),
            Instr(Opcode.di),
            Instr(Opcode.ei),

            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.incx, mw),

            Instr(Opcode.ex, D,H),
            Instr(Opcode.ex, A,af_),
            Instr(Opcode.exx),
            Instr(Opcode.daa, a),

            Instr(Opcode.rcf),
            Instr(Opcode.scf),
            Instr(Opcode.ccf),
            Instr(Opcode.decx, mb),

            // 10
            Instr(Opcode.cpl, a),
            Instr(Opcode.neg, a),
            Instr(Opcode.mul, H,Ib),
            Instr(Opcode.div, H,Ib),

            Instr(Opcode.add, X,Iw),
            Instr(Opcode.add, Y,Iw),
            Instr(Opcode.add, S,Iw),
            Instr(Opcode.ldar, H,jw),

            Instr(Opcode.djnz, InstrClass.ConditionalTransfer, jb ),
            Instr(Opcode.djnz, InstrClass.ConditionalTransfer, B,jb),
            Instr(Opcode.jp, InstrClass.Transfer, Jw),
            Instr(Opcode.jr, InstrClass.Transfer, jw),

            Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Jw),
            Instr(Opcode.callr, InstrClass.Transfer|InstrClass.Call, jw),
            Instr(Opcode.ret, InstrClass.Transfer),
            Instr(Opcode.reti, InstrClass.Transfer), 

            // 20
            Instr(Opcode.ld, a,r),
            Instr(Opcode.ld, a,r),
            Instr(Opcode.ld, a,r),
            Instr(Opcode.ld, a,r),

            Instr(Opcode.ld, a,r),
            Instr(Opcode.ld, a,r),
            Instr(Opcode.ld, a,r),
            Instr(Opcode.ld, a,mb),

            Instr(Opcode.ld, r,a),
            Instr(Opcode.ld, r,a),
            Instr(Opcode.ld, r,a),
            Instr(Opcode.ld, r,a),

            Instr(Opcode.ld, r,a),
            Instr(Opcode.ld, r,a),
            Instr(Opcode.ld, r,a),
            Instr(Opcode.ld, mb,a),

            // 30
            Instr(Opcode.ld, r,Ib),
            Instr(Opcode.ld, r,Ib),
            Instr(Opcode.ld, r,Ib),
            Instr(Opcode.ld, r,Ib),

            Instr(Opcode.ld, r,Ib),
            Instr(Opcode.ld, r,Ib),
            Instr(Opcode.ld, r,Ib),
            Instr(Opcode.ld, mb,Ib),

            Instr(Opcode.ld, B,Iw),
            Instr(Opcode.ld, D,Iw),
            Instr(Opcode.ld, H,Iw),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.ld, X,Iw),
            Instr(Opcode.ld, Y,Iw),
            Instr(Opcode.ld, S,Iw),
            Instr(Opcode.ldw, mw,Iw),

            // 40
            Instr(Opcode.ld, H,B),
            Instr(Opcode.ld, H,D),
            Instr(Opcode.ld, H,H),    // lolwut
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.ld, H,X),
            Instr(Opcode.ld, H,Y),
            Instr(Opcode.ld, H,S),
            Instr(Opcode.ld, H,mw),

            Instr(Opcode.ld, B,H),
            Instr(Opcode.ld, D,H),
            Instr(Opcode.ld, H,H),    // lolwut
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.ld, X,H),
            Instr(Opcode.ld, Y,H),
            Instr(Opcode.ld, S,H),
            Instr(Opcode.ld, mw,H),

            // 50
            Instr(Opcode.push, B),
            Instr(Opcode.push, D),
            Instr(Opcode.push, H),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.push, X),
            Instr(Opcode.push, Y),
            Instr(Opcode.push, A),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.pop, B),
            Instr(Opcode.pop, D),
            Instr(Opcode.pop, H),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.pop, X),
            Instr(Opcode.pop, Y),
            Instr(Opcode.pop, A),
            Instr(Opcode.invalid, InstrClass.Invalid),

            // 60
            Instr(Opcode.add, a,mb),
            Instr(Opcode.adc, a,mb),
            Instr(Opcode.sub, a,mb),
            Instr(Opcode.sbc, a,mb),

            Instr(Opcode.and, a,mb),
            Instr(Opcode.xor, a,mb),
            Instr(Opcode.or,  a,mb),
            Instr(Opcode.cp,  a,mb),

            Instr(Opcode.add, a,Ib),
            Instr(Opcode.adc, a,Ib),
            Instr(Opcode.sub, a,Ib),
            Instr(Opcode.sbc, a,Ib),

            Instr(Opcode.and, a,Ib),
            Instr(Opcode.xor, a,Ib),
            Instr(Opcode.or,  a,Ib),
            Instr(Opcode.cp,  a,Ib),

            // 70
            Instr(Opcode.add, H,Mw),
            Instr(Opcode.adc, H,Mw),
            Instr(Opcode.sub, H,Mw),
            Instr(Opcode.sbc, H,Mw),

            Instr(Opcode.and, H,Mw),
            Instr(Opcode.xor, H,Mw),
            Instr(Opcode.or,  H,Mw),
            Instr(Opcode.cp,  H,Mw),

            Instr(Opcode.add, H,Iw),
            Instr(Opcode.adc, H,Iw),
            Instr(Opcode.sub, H,Iw),
            Instr(Opcode.sbc, H,Iw),

            Instr(Opcode.and, H,Iw),
            Instr(Opcode.xor, H,Iw),
            Instr(Opcode.or,  H,Iw),
            Instr(Opcode.cp,  H,Iw),

            // 80
            Instr(Opcode.inc, r),
            Instr(Opcode.inc, r),
            Instr(Opcode.inc, r),
            Instr(Opcode.inc, r),

            Instr(Opcode.inc, r),
            Instr(Opcode.inc, r),
            Instr(Opcode.inc, r),
            Instr(Opcode.inc, mb),

            Instr(Opcode.dec, r),
            Instr(Opcode.dec, r),
            Instr(Opcode.dec, r),
            Instr(Opcode.dec, r),

            Instr(Opcode.dec, r),
            Instr(Opcode.dec, r),
            Instr(Opcode.dec, r),
            Instr(Opcode.dec, mb),

            // 90
            Instr(Opcode.inc, B),
            Instr(Opcode.inc, D),
            Instr(Opcode.inc, H),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.inc, X),
            Instr(Opcode.inc, Y),
            Instr(Opcode.inc, A),
            Instr(Opcode.incw, mw),

            Instr(Opcode.dec, B),
            Instr(Opcode.dec, D),
            Instr(Opcode.dec, H),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.dec, X),
            Instr(Opcode.dec, Y),
            Instr(Opcode.dec, A),
            Instr(Opcode.decw, mw),

            // A0
            Instr(Opcode.rrc),
            Instr(Opcode.rrc),
            Instr(Opcode.rl),
            Instr(Opcode.rr),

            Instr(Opcode.sla),
            Instr(Opcode.sra),
            Instr(Opcode.sll),
            Instr(Opcode.srl),

            Instr(Opcode.bit, i,mb),
            Instr(Opcode.bit, i,mb),
            Instr(Opcode.bit, i,mb),
            Instr(Opcode.bit, i,mb),

            Instr(Opcode.bit, i,mb),
            Instr(Opcode.bit, i,mb),
            Instr(Opcode.bit, i,mb),
            Instr(Opcode.bit, i,mb),

            // B0
            Instr(Opcode.res, i,mb),
            Instr(Opcode.res, i,mb),
            Instr(Opcode.res, i,mb),
            Instr(Opcode.res, i,mb),

            Instr(Opcode.res, i,mb),
            Instr(Opcode.res, i,mb),
            Instr(Opcode.res, i,mb),
            Instr(Opcode.res, i,mb),

            Instr(Opcode.set, i,mb),
            Instr(Opcode.set, i,mb),
            Instr(Opcode.set, i,mb),
            Instr(Opcode.set, i,mb),

            Instr(Opcode.set, i,mb),
            Instr(Opcode.set, i,mb),
            Instr(Opcode.set, i,mb),
            Instr(Opcode.set, i,mb),

            // C0
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),

            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),

            Instr(Opcode.jr, InstrClass.Transfer, jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),

            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Opcode.jr, InstrClass.ConditionalTransfer, c,jb),

            // D0
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),

            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),
            Instr(Opcode.invalid, InstrClass.Invalid),

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
            Instr(Opcode.swi, InstrClass.Transfer|InstrClass.Call), 
        };
        private Address addr;

        private static Decoder[] Oprecs
        {
            get
            {
                return decoders;
            }

            set
            {
                decoders = value;
            }
        }
    }
}

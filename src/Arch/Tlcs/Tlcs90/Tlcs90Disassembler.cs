#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
    using Decoder = Decoder<Tlcs90Disassembler, Mnemonic, Tlcs90Instruction>;

    public partial class Tlcs90Disassembler : DisassemblerBase<Tlcs90Instruction, Mnemonic>
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

            var instr = Decoders[b].Decode(b, this);
            if (instr == null)
                instr = CreateInvalidInstruction();
            var len = rdr.Address - addr;
            instr.Address = addr;
            instr.Length = (int) len;
            return instr;
        }

        public override Tlcs90Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Tlcs90Instruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = this.ops.ToArray()
            };
        }

        public override Tlcs90Instruction CreateInvalidInstruction()
        {
            return new Tlcs90Instruction
            {
                Mnemonic = Mnemonic.invalid,
                InstructionClass = InstrClass.Invalid,
                Operands = MachineInstruction.NoOperands
            };
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

        private class RegDecoder : Decoder
        {
            private readonly RegisterStorage regByte;
            private readonly RegisterStorage regWord;

            public RegDecoder(RegisterStorage regByte, RegisterStorage regWord)
            {
                this.regByte = regByte;
                this.regWord = regWord;
            }

            public override Tlcs90Instruction Decode(uint bPrev, Tlcs90Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte b))
                    return null;
                dasm.byteReg = new RegisterOperand(regByte);
                if (regWord != null)
                    dasm.wordReg = new RegisterOperand(regWord);
                return regEncodings[b].Decode(b, dasm);
            }
        }

        private class DstDecoder : Decoder
        {
            private string format;

            public DstDecoder(string format)
            {
                this.format = format;
            }

            public override Tlcs90Instruction Decode(uint bPrev, Tlcs90Disassembler dasm)
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
                        if (!dasm.rdr.TryReadByte(out byte bOff))
                            return null;
                        offset = Constant.SByte((sbyte)bOff);
                    }
                    break;
                default: throw new NotImplementedException(string.Format("Tlcs-90: dst {0}", format));
                }
                if (!dasm.rdr.TryReadByte(out byte b))
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
                    if (instr.Operands.Length == 0)
                    {
                        instr.Operands = new MachineOperand[] { operand };
                    }
                    else
                    {
                        instr.Operands = new MachineOperand[] { operand, instr.Operands[0] };
                    }
                    if (instr.Operands.Length == 2)
                    {
                        instr.Operands[0].Width = instr.Operands[1].Width;
                    }
                }
                else if (dasm.backPatchOp == 1)
                {
                    if ((instr.Mnemonic == Mnemonic.jp || instr.Mnemonic == Mnemonic.call)
                        &&
                        operand.Base == null &&
                        operand.Index == null &&
                        operand.Offset != null)
                    {
                        // JP cc,(XXXX) should be JP cc,XXXX
                        var op = AddressOperand.Ptr16(operand.Offset.ToUInt16());
                        op.Width = PrimitiveType.Ptr16;
                        instr.Operands = new MachineOperand[] { instr.Operands[0], op };
                    }
                    else
                    {
                        instr.Operands = new MachineOperand[] {
                            instr.Operands[0],
                            operand
                        };
                        instr.Operands[1].Width = instr.Operands[0].Width;
                    }
                }
                else
                    return null;
                return instr;
            }
        }

        private class SrcDecoder : Decoder
        {
            private string format;

            public SrcDecoder(string format)
            {
                this.format = format;
            }

            public override Tlcs90Instruction Decode(uint bPrev, Tlcs90Disassembler dasm)
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
                        if (!dasm.rdr.TryReadByte(out byte bOff))
                            return null;
                        offset = Constant.SByte((sbyte)bOff);
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

                if (!dasm.rdr.TryReadByte(out byte b))
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
                    if (instr.Operands.Length == 1)
                    {
                        instr.Operands = new MachineOperand[] { operand, instr.Operands[0] };
                    }
                    else
                    {
                        instr.Operands = new MachineOperand[] { operand };
                    }
                    if (instr.Operands.Length >= 2)
                    {
                        operand.Width = instr.Operands[1].Width;
                    }
                }
                else if (dasm.backPatchOp == 1)
                {
                    if (operand != null)
                    {
                        instr.Operands = new MachineOperand[] { instr.Operands[0], operand };
                    }
                    operand.Width = instr.Operands[0].Width;
                }
                else
                    return null;
                return instr;
            }
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Tlcs90Disassembler>[] mutators)
        {
            return new InstrDecoder<Tlcs90Disassembler, Mnemonic, Tlcs90Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Tlcs90Disassembler>[] mutators)
        {
            return new InstrDecoder<Tlcs90Disassembler, Mnemonic, Tlcs90Instruction>(iclass, mnemonic, mutators);
        }

        private static Decoder invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

        private static Decoder[] decoders = new Decoder[256]
        {
            // 00
            Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding|InstrClass.Zero),
            Instr(Mnemonic.halt, InstrClass.Terminates),
            Instr(Mnemonic.di),
            Instr(Mnemonic.ei),

            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.incx, mw),

            Instr(Mnemonic.ex, D,H),
            Instr(Mnemonic.ex, A,af_),
            Instr(Mnemonic.exx),
            Instr(Mnemonic.daa, a),

            Instr(Mnemonic.rcf),
            Instr(Mnemonic.scf),
            Instr(Mnemonic.ccf),
            Instr(Mnemonic.decx, mb),

            // 10
            Instr(Mnemonic.cpl, a),
            Instr(Mnemonic.neg, a),
            Instr(Mnemonic.mul, H,Ib),
            Instr(Mnemonic.div, H,Ib),

            Instr(Mnemonic.add, X,Iw),
            Instr(Mnemonic.add, Y,Iw),
            Instr(Mnemonic.add, S,Iw),
            Instr(Mnemonic.ldar, H,jw),

            Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, jb ),
            Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, B,jb),
            Instr(Mnemonic.jp, InstrClass.Transfer, Jw),
            Instr(Mnemonic.jr, InstrClass.Transfer, jw),

            Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Jw),
            Instr(Mnemonic.callr, InstrClass.Transfer|InstrClass.Call, jw),
            Instr(Mnemonic.ret, InstrClass.Transfer),
            Instr(Mnemonic.reti, InstrClass.Transfer), 

            // 20
            Instr(Mnemonic.ld, a,r),
            Instr(Mnemonic.ld, a,r),
            Instr(Mnemonic.ld, a,r),
            Instr(Mnemonic.ld, a,r),

            Instr(Mnemonic.ld, a,r),
            Instr(Mnemonic.ld, a,r),
            Instr(Mnemonic.ld, a,r),
            Instr(Mnemonic.ld, a,mb),

            Instr(Mnemonic.ld, r,a),
            Instr(Mnemonic.ld, r,a),
            Instr(Mnemonic.ld, r,a),
            Instr(Mnemonic.ld, r,a),

            Instr(Mnemonic.ld, r,a),
            Instr(Mnemonic.ld, r,a),
            Instr(Mnemonic.ld, r,a),
            Instr(Mnemonic.ld, mb,a),

            // 30
            Instr(Mnemonic.ld, r,Ib),
            Instr(Mnemonic.ld, r,Ib),
            Instr(Mnemonic.ld, r,Ib),
            Instr(Mnemonic.ld, r,Ib),

            Instr(Mnemonic.ld, r,Ib),
            Instr(Mnemonic.ld, r,Ib),
            Instr(Mnemonic.ld, r,Ib),
            Instr(Mnemonic.ld, mb,Ib),

            Instr(Mnemonic.ld, B,Iw),
            Instr(Mnemonic.ld, D,Iw),
            Instr(Mnemonic.ld, H,Iw),
            invalid,

            Instr(Mnemonic.ld, X,Iw),
            Instr(Mnemonic.ld, Y,Iw),
            Instr(Mnemonic.ld, S,Iw),
            Instr(Mnemonic.ldw, mw,Iw),

            // 40
            Instr(Mnemonic.ld, H,B),
            Instr(Mnemonic.ld, H,D),
            Instr(Mnemonic.ld, H,H),    // lolwut
            invalid,

            Instr(Mnemonic.ld, H,X),
            Instr(Mnemonic.ld, H,Y),
            Instr(Mnemonic.ld, H,S),
            Instr(Mnemonic.ld, H,mw),

            Instr(Mnemonic.ld, B,H),
            Instr(Mnemonic.ld, D,H),
            Instr(Mnemonic.ld, H,H),    // lolwut
            invalid,

            Instr(Mnemonic.ld, X,H),
            Instr(Mnemonic.ld, Y,H),
            Instr(Mnemonic.ld, S,H),
            Instr(Mnemonic.ld, mw,H),

            // 50
            Instr(Mnemonic.push, B),
            Instr(Mnemonic.push, D),
            Instr(Mnemonic.push, H),
            invalid,

            Instr(Mnemonic.push, X),
            Instr(Mnemonic.push, Y),
            Instr(Mnemonic.push, A),
            invalid,

            Instr(Mnemonic.pop, B),
            Instr(Mnemonic.pop, D),
            Instr(Mnemonic.pop, H),
            invalid,

            Instr(Mnemonic.pop, X),
            Instr(Mnemonic.pop, Y),
            Instr(Mnemonic.pop, A),
            invalid,

            // 60
            Instr(Mnemonic.add, a,mb),
            Instr(Mnemonic.adc, a,mb),
            Instr(Mnemonic.sub, a,mb),
            Instr(Mnemonic.sbc, a,mb),

            Instr(Mnemonic.and, a,mb),
            Instr(Mnemonic.xor, a,mb),
            Instr(Mnemonic.or,  a,mb),
            Instr(Mnemonic.cp,  a,mb),

            Instr(Mnemonic.add, a,Ib),
            Instr(Mnemonic.adc, a,Ib),
            Instr(Mnemonic.sub, a,Ib),
            Instr(Mnemonic.sbc, a,Ib),

            Instr(Mnemonic.and, a,Ib),
            Instr(Mnemonic.xor, a,Ib),
            Instr(Mnemonic.or,  a,Ib),
            Instr(Mnemonic.cp,  a,Ib),

            // 70
            Instr(Mnemonic.add, H,Mw),
            Instr(Mnemonic.adc, H,Mw),
            Instr(Mnemonic.sub, H,Mw),
            Instr(Mnemonic.sbc, H,Mw),

            Instr(Mnemonic.and, H,Mw),
            Instr(Mnemonic.xor, H,Mw),
            Instr(Mnemonic.or,  H,Mw),
            Instr(Mnemonic.cp,  H,Mw),

            Instr(Mnemonic.add, H,Iw),
            Instr(Mnemonic.adc, H,Iw),
            Instr(Mnemonic.sub, H,Iw),
            Instr(Mnemonic.sbc, H,Iw),

            Instr(Mnemonic.and, H,Iw),
            Instr(Mnemonic.xor, H,Iw),
            Instr(Mnemonic.or,  H,Iw),
            Instr(Mnemonic.cp,  H,Iw),

            // 80
            Instr(Mnemonic.inc, r),
            Instr(Mnemonic.inc, r),
            Instr(Mnemonic.inc, r),
            Instr(Mnemonic.inc, r),

            Instr(Mnemonic.inc, r),
            Instr(Mnemonic.inc, r),
            Instr(Mnemonic.inc, r),
            Instr(Mnemonic.inc, mb),

            Instr(Mnemonic.dec, r),
            Instr(Mnemonic.dec, r),
            Instr(Mnemonic.dec, r),
            Instr(Mnemonic.dec, r),

            Instr(Mnemonic.dec, r),
            Instr(Mnemonic.dec, r),
            Instr(Mnemonic.dec, r),
            Instr(Mnemonic.dec, mb),

            // 90
            Instr(Mnemonic.inc, B),
            Instr(Mnemonic.inc, D),
            Instr(Mnemonic.inc, H),
            invalid,

            Instr(Mnemonic.inc, X),
            Instr(Mnemonic.inc, Y),
            Instr(Mnemonic.inc, A),
            Instr(Mnemonic.incw, mw),

            Instr(Mnemonic.dec, B),
            Instr(Mnemonic.dec, D),
            Instr(Mnemonic.dec, H),
            invalid,

            Instr(Mnemonic.dec, X),
            Instr(Mnemonic.dec, Y),
            Instr(Mnemonic.dec, A),
            Instr(Mnemonic.decw, mw),

            // A0
            Instr(Mnemonic.rrc),
            Instr(Mnemonic.rrc),
            Instr(Mnemonic.rl),
            Instr(Mnemonic.rr),

            Instr(Mnemonic.sla),
            Instr(Mnemonic.sra),
            Instr(Mnemonic.sll),
            Instr(Mnemonic.srl),

            Instr(Mnemonic.bit, i,mb),
            Instr(Mnemonic.bit, i,mb),
            Instr(Mnemonic.bit, i,mb),
            Instr(Mnemonic.bit, i,mb),

            Instr(Mnemonic.bit, i,mb),
            Instr(Mnemonic.bit, i,mb),
            Instr(Mnemonic.bit, i,mb),
            Instr(Mnemonic.bit, i,mb),

            // B0
            Instr(Mnemonic.res, i,mb),
            Instr(Mnemonic.res, i,mb),
            Instr(Mnemonic.res, i,mb),
            Instr(Mnemonic.res, i,mb),

            Instr(Mnemonic.res, i,mb),
            Instr(Mnemonic.res, i,mb),
            Instr(Mnemonic.res, i,mb),
            Instr(Mnemonic.res, i,mb),

            Instr(Mnemonic.set, i,mb),
            Instr(Mnemonic.set, i,mb),
            Instr(Mnemonic.set, i,mb),
            Instr(Mnemonic.set, i,mb),

            Instr(Mnemonic.set, i,mb),
            Instr(Mnemonic.set, i,mb),
            Instr(Mnemonic.set, i,mb),
            Instr(Mnemonic.set, i,mb),

            // C0
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),

            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),

            Instr(Mnemonic.jr, InstrClass.Transfer, jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),

            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),
            Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, c,jb),

            // D0
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            // E0
            new SrcDecoder("B"),
            new SrcDecoder("D"),
            new SrcDecoder("H"),
            new SrcDecoder("M"),

            new SrcDecoder("X"),
            new SrcDecoder("Y"),
            new SrcDecoder("S"),
            new SrcDecoder("m"),

            new DstDecoder("B"),
            new DstDecoder("D"),
            new DstDecoder("H"),
            new DstDecoder("M"),

            new DstDecoder("X"),
            new DstDecoder("Y"),
            new DstDecoder("S"),
            new DstDecoder("m"),

            // F0
            new SrcDecoder("EX"),
            new SrcDecoder("EY"),
            new SrcDecoder("ES"),
            new SrcDecoder("EH"),

            new DstDecoder("EX"),
            new DstDecoder("EY"),
            new DstDecoder("ES"),
            new DstDecoder("EH"),

            new RegDecoder(Registers.b, Registers.bc),
            new RegDecoder(Registers.c, Registers.de),
            new RegDecoder(Registers.d, Registers.hl),
            new RegDecoder(Registers.e, null),

            new RegDecoder(Registers.h, Registers.ix),
            new RegDecoder(Registers.l, Registers.iy),
            new RegDecoder(Registers.a, Registers.sp),
            Instr(Mnemonic.swi, InstrClass.Transfer|InstrClass.Call), 
        };
        private Address addr;

        private static Decoder[] Decoders
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

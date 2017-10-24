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
using System.Linq;
using System.Text;
using Opcode = Reko.Arch.Tlcs.Tlcs900.Tlcs900Opcode;

namespace Reko.Arch.Tlcs.Tlcs900
{
    /// <summary>
    /// Disassembler for the 32-bit Toshiba TLCS-900 processor.
    /// </summary>
    public partial class Tlcs900Disassembler : DisassemblerBase<Tlcs900Instruction>
    {
        private Tlcs900Architecture arch;
        private ImageReader rdr;
        private Address addr;

        private MachineOperand opSrc;
        private MachineOperand opDst;
        private char opSize;

        public Tlcs900Disassembler(Tlcs900Architecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Tlcs900Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            byte b;
            if (!rdr.TryReadByte(out b))
                return null;
            this.opSrc = null;
            this.opDst = null;
            this.opSize = '\0';
            var instr = opRecs[b].Decode(b, this);
            if (instr == null)
            {
                instr = new Tlcs900Instruction { Address = this.addr, Opcode = Opcode.invalid };
            }
            instr.Length = (int)(rdr.Address - instr.Address);
            return instr;
        }

        private Tlcs900Instruction Decode(byte b, Opcode opcode, string fmt)
        {
            var instr = new Tlcs900Instruction
            {
                Opcode = opcode,
                Address = this.addr,
            };
            var ops = new List<MachineOperand>();
            MachineOperand op = null;
            for (int i = 0; i< fmt.Length; ++i)
            {
                switch (fmt[i++])
                {
                case ',': continue;
                case 'C': // condition code
                    var cc = (CondCode)(b & 0xF);
                    if (cc == CondCode.T)
                        continue;
                    op = new ConditionOperand(cc);
                    break;
                case 'A':
                    op = new RegisterOperand(Tlcs900Registers.a);
                    break;
                case '3': // Immediate encoded in low 3 bits
                    var c = Constant.Create(Size(fmt[1]), imm3Const[b & 7]);
                    SetSize(fmt[1]);
                    op = new ImmediateOperand(c);
                    break;
                case 'I': // immediate
                    op = Immediate(fmt[i++]);
                    break;
                case 'j': // Relative jump
                    switch (fmt[i++])
                    {
                    case 'b':
                        byte o8;
                        if (!rdr.TryReadByte(out o8))
                            op = null;
                        else
                            op = AddressOperand.Create(rdr.Address + (sbyte)o8);
                        break;
                    case 'w':
                        short o16;
                        if (!rdr.TryReadLeInt16(out o16))
                            op = null;
                        else
                            op = AddressOperand.Create(rdr.Address + o16);
                        break;
                    }
                    break;
                case 'J': // Absolute jump
                    switch (fmt[i++])
                    {
                    case 'w': op = AbsoluteDestination(2); break;
                    case 'l': op = AbsoluteDestination(3); break;
                    default: op = null; break;
                    }
                    break;
                case 'R':   // 16 bit register encoded in lsb
                    op = new RegisterOperand( Reg(fmt[i++], b & 0x7));
                    break;
                case 'S': // status/flag register
                    op = StatusRegister(fmt[i++]);
                    break;
                }
                if (op == null)
                    return Decode(b, Opcode.invalid, "");
                ops.Add(op);
            }
            if (ops.Count > 0)
            {
                instr.op1 = ops[0];
                if (ops.Count > 1)
                {
                    instr.op2 = ops[1];
                    if (ops.Count > 2)
                    {
                        instr.op3 = ops[2];
                    }
                }
            }
            return instr;
        }

        private MachineOperand DecodeOperand(byte b, string fmt)
        {
            MachineOperand op;
            byte r;
            Constant c;
            byte o8;
            int incCode;
            switch (fmt[0])
            {
            case '+': // Predecrement
                if (!rdr.TryReadByte(out r))
                    return null;
                incCode = r & 3;
                if (incCode >= incDecSize.Length)
                    return null;
                op = MemoryOperand.PostIncrement(Size(fmt[1]), incDecSize[r & 3], Reg('x', (r >> 2) & 0x3F));
                SetSize(fmt[1]);
                return op;
            case '-':
                if (!rdr.TryReadByte(out r))
                    return null;
                incCode = r & 3;
                if (incCode >= incDecSize.Length)
                    return null;
                op = MemoryOperand.PreDecrement(Size(fmt[1]), incDecSize[r & 3], Reg('x', (r >> 2) & 0x3F));
                SetSize(fmt[1]);
                return op;
            case '3': // Immediate encoded in low 3 bits
                c = Constant.Create(Size(fmt[1]), b & 7);
                SetSize(fmt[1]);
                return new ImmediateOperand(c);
            case '#': // Immediate encoded in low 3 bits, with 8 encoded as 0
                c = Constant.Create(Size(fmt[1]), imm3Const[b & 7]);
                SetSize(fmt[1]);
                return new ImmediateOperand(c);

            case 'A': // A register
                op = new RegisterOperand(Tlcs900Registers.a);
                return op;
            case 'C': // condition code
                op = new ConditionOperand((CondCode)(b & 0xF));
                return op;
            case 'I': // immediate
                op = Immediate(fmt[1]);
                return op;
            case 'j': // Relative jump
                switch (fmt[1])
                {
                case 'b':
                    if (!rdr.TryReadByte(out o8))
                        return  null;
                    else
                        return AddressOperand.Create(rdr.Address + (sbyte)o8);
                case 'w':
                    short o16;
                    if (!rdr.TryReadLeInt16(out o16))
                        return null;
                    else
                        return AddressOperand.Create(rdr.Address + o16);
                }
                return null;
            case 'r': // Register
            case 'R':
                //$TODO: 'r' may encode other registers. manual is dense
                op = new RegisterOperand(Reg(fmt[1], b & 0x7));
                SetSize(fmt[1]);
                return op;
            case 'M': // Register indirect
                op = MemoryOperand.Indirect(Size(fmt[1]), Reg('x', b & 7));
                SetSize(fmt[1]);
                return op;
            case 'N': // indexed (8-bit offset)
                if (!rdr.TryReadByte(out o8))
                    return null;
                op = MemoryOperand.Indexed8(Size(fmt[1]), Reg('x', b & 7), (sbyte)o8);
                SetSize(fmt[1]);
                return op;
            case 'm': // various mem formats
                byte m;
                if (!rdr.TryReadByte(out m))
                    return null;
                switch (m & 3)
                {
                case 0: // Register indirect
                    op = MemoryOperand.Indirect(Size(fmt[1]), Reg('x', (m >> 2) & 0x3F));
                    break;
                case 1: // indexed (16-bit offset)
                    short o16;
                    if (!rdr.TryReadLeInt16(out o16))
                        return null;
                    op = MemoryOperand.Indexed16(Size(fmt[1]), Reg('x', (m >> 2) & 0x3F), o16);
                    SetSize(fmt[1]);
                    return op;
                case 3:
                    if (m != 3 && m != 7)
                        return null;
                    byte rBase;
                    if (!rdr.TryReadByte(out rBase))
                        return null;
                    byte rIdx;
                    if (!rdr.TryReadByte(out rIdx))
                        return null;
                    var regBase = Reg('x', rBase);
                    var regIdx = Reg(m == 3 ? 'b' : 'w', rIdx);
                    op = MemoryOperand.RegisterIndexed(Size(fmt[1]), regBase, regIdx);
                    SetSize(fmt[1]);
                    return op;
                default:
                    throw new FormatException(string.Format(
                        "Unknown format {0} decoding bytes {1:X2}{2:X2}.",
                            fmt[0], (int)b, (int)m));
                }
                SetSize(fmt[1]);
                return op;
            case 'O': return Absolute(1, fmt[1]);
            case 'P': return Absolute(2, fmt[1]);
            case 'Q': return Absolute(3, fmt[1]);
            default: throw new FormatException(
                string.Format(
                    "Unknown format {0} decoding byte {1:X2}.", fmt[0], (int)b));
            }
        }

        private RegisterStorage Reg(char size, int regNum)
        {
            int r = regNum & 7;
            if (size == 'z')
            {
                size = this.opSize;
            }
            switch (size)
            {
            case 'x': return Tlcs900Registers.regs[r];
            case 'w': return Tlcs900Registers.regs[8 + r];
            case 'b': return Tlcs900Registers.regs[16 + r];
            default: throw new FormatException();
            }
        }

        private PrimitiveType Size(char size)
        {
            if (size == 'z')
            {
                size = this.opSize;
            }
            switch (size)
            {
            
            case 'x': this.opSize = size; return PrimitiveType.Word32;
            case 'w': this.opSize = size; return PrimitiveType.Word16;
            case 'b': this.opSize = size; return PrimitiveType.Byte;
            case '?':
                // Don't know the size yet, second operand will 
                // provide size.
                this.opSize = size; return null;
            default: throw new FormatException();
            }
        }

        private void SetSize(char size)
        {
            if (size != 'z')
                this.opSize = size;
        }

        private MachineOperand Immediate(char size)
        {
            if (size == 'z')
            {
                size = this.opSize;
            }
            switch (size)
            {
            case 'b':
                byte b;
                if (!rdr.TryReadByte(out b))
                    return null;
                return ImmediateOperand.Byte(b);
            case 'w':
                ushort w;
                if (!rdr.TryReadLeUInt16(out w))
                    return null;
                return ImmediateOperand.Word16(w);
            case 'x':
                uint u;
                if (!rdr.TryReadLeUInt32(out u))
                    return null;
                return ImmediateOperand.Word32(u);
            }
            return null;
        }

        private MachineOperand Absolute(int addrBytes, char size)
        {
            uint uAddr = 0;
            int sh = 0;
            while (--addrBytes >= 0)
            {
                byte b;
                if (!rdr.TryReadByte(out b))
                    return null;
                uAddr |= (uint)b << sh;
                sh += 8;
            }
            SetSize(size);
            return MemoryOperand.Absolute(Size(size), uAddr);
        }

        private MachineOperand AbsoluteDestination(int addrBytes)
        {
            uint uAddr = 0;
            int sh = 0;
            while (--addrBytes >= 0)
            {
                byte b;
                if (!rdr.TryReadByte(out b))
                    return null;
                uAddr |= (uint)b << sh;
                sh += 8;
            }
            return AddressOperand.Ptr32(uAddr);
        }

        private MachineOperand StatusRegister(char size)
        {
            switch (size)
            {
            case 'b': return new RegisterOperand( Tlcs900Registers.f);
            case 'w': return new RegisterOperand(Tlcs900Registers.sr);
            default: return null;
            }
        }

        private RegisterOperand ExtraRegister(byte b, string width)
        {
            switch (b)
            {
            case 0x31: return new RegisterOperand(Tlcs900Registers.w);
            case 0xE6: return new RegisterOperand(Tlcs900Registers.bc);
            }
            return null;
        }


        public abstract class OpRecBase
        {
            public abstract Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm);
        }

        public class OpRec : OpRecBase
        {
            private Opcode opcode;
            private string fmt;

            public OpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                return dasm.Decode(b, opcode, fmt);
            }
        }

        private class RegOpRec : OpRecBase
        {
            private string fmt;

            public RegOpRec(string fmt)
            {
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opSrc = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opSrc == null || !dasm.rdr.TryReadByte(out b))
                    return dasm.Decode(b, Opcode.invalid, "");
                return regOpRecs[b].Decode(b, dasm);
            }
        }

        private class ExtraRegOprec :OpRecBase
        {
            private string width;

            public ExtraRegOprec(string width)
            {
                this.width = width;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out b))
                    return null;
                dasm.opSize = width[0];
                dasm.opSrc = dasm.ExtraRegister(b, width);
                if (dasm.opSrc == null)
                    return null;
                if (!dasm.rdr.TryReadByte(out b))
                    return null;
                return regOpRecs[b].Decode(b, dasm);
            }
        }

        private class MemOpRec : OpRecBase
        {
            private string fmt;

            public MemOpRec(string fmt)
            {
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opSrc = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opSrc == null || !dasm.rdr.TryReadByte(out b))
                    return dasm.Decode(b, Opcode.invalid, "");
                return memOpRecs[b].Decode(b, dasm);
            }
        }

        private class DstOpRec : OpRecBase
        {
            private string fmt;

            public DstOpRec(string fmt)
            {
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opSrc = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opSrc == null || !dasm.rdr.TryReadByte(out b))
                    return dasm.Decode(b, Opcode.invalid, "");
                var instr = dstOpRecs[b].Decode(b, dasm);
                if (instr.op1 != null && instr.op2 != null)
                {
                    instr.op1.Width = instr.op2.Width;
                }
                if (instr.op2 != null && instr.op2.Width == null)
                {
                    //$HACK to get conditional calls/jumps to work
                    instr.op2.Width = PrimitiveType.Word32;
                }
                return instr;
            }
        }

        private class SecondOpRec : OpRecBase
        {
            private Opcode opcode;
            private string fmt;

            public SecondOpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                if (this.fmt.Length == 0)
                {
                    return new Tlcs900Instruction
                    {
                        Opcode = this.opcode,
                        Address = dasm.addr,
                        op1 = dasm.opSrc
                    };
                }

                if (this.fmt[0] == 'Z')
                {
                    // Override the size of opSrc
                    dasm.opSrc.Width = dasm.Size(fmt[1]);
                    return new Tlcs900Instruction
                    {
                        Opcode = this.opcode,
                        Address = dasm.addr,
                        op1 = dasm.opSrc,
                    };
                }
                else
                {
                    dasm.opDst = dasm.DecodeOperand(b, this.fmt);
                    if (dasm.opDst == null)
                        return dasm.Decode(b, Opcode.invalid, "");
                    return new Tlcs900Instruction
                    {
                        Opcode = this.opcode,
                        Address = dasm.addr,
                        op1 = dasm.opDst,
                        op2 = dasm.opSrc,
                    };
                }
            }
        }

        // Inverts the order of the decoded operands
        private class InvOpRec : OpRecBase
        {
            private Opcode opcode;
            private string fmt;

            public InvOpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opDst = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opDst == null)
                    return dasm.Decode(b, Opcode.invalid, "");
                return new Tlcs900Instruction
                {
                    Opcode = this.opcode,
                    Address = dasm.addr,
                    op1 = dasm.opSrc,
                    op2 = dasm.opDst,
                };
            }
        }

        private class LdirOpRec : OpRecBase
        {
            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                if (dasm.opSize == 'w')
                    return dasm.Decode(b, Opcode.ldirw, "");
                else 
                    return dasm.Decode(b, Opcode.ldir, "");
            }
        }

        private static int[] imm3Const = new int[8]
        {
            8, 1, 2, 3, 4, 5, 6, 7,
        };

        private static int[] incDecSize = new int[3]
        {
            1, 2, 4
        };

        private static OpRecBase[] opRecs = {
            // 00
            new OpRec(Opcode.nop, ""),    
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.push, "Sw"),
            new OpRec(Opcode.pop, "Sw"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.halt, ""),
            new OpRec(Opcode.ei, "Ib"),
            new OpRec(Opcode.reti, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.push, "Ib"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.push, "Iw"),

            new OpRec(Opcode.incf, ""),
            new OpRec(Opcode.decf, ""),
            new OpRec(Opcode.ret, ""),
            new OpRec(Opcode.retd, "Iw"),
            // 10
            new OpRec(Opcode.rcf, ""),
            new OpRec(Opcode.scf, ""),
            new OpRec(Opcode.ccf, ""),
            new OpRec(Opcode.zcf, ""),

            new OpRec(Opcode.push, "A"),
            new OpRec(Opcode.pop, "A"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.ldf, "Ib"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.jp, "Jw"),
            new OpRec(Opcode.jp, "Jl"),

            new OpRec(Opcode.call, "Jw"),
            new OpRec(Opcode.call, "Jl"),
            new OpRec(Opcode.calr, "jw"),
            new OpRec(Opcode.invalid, ""),
            // 20
            new OpRec(Opcode.ld, "Rb,Ib"),
            new OpRec(Opcode.ld, "Rb,Ib"),
            new OpRec(Opcode.ld, "Rb,Ib"),
            new OpRec(Opcode.ld, "Rb,Ib"),

            new OpRec(Opcode.ld, "Rb,Ib"),
            new OpRec(Opcode.ld, "Rb,Ib"),
            new OpRec(Opcode.ld, "Rb,Ib"),
            new OpRec(Opcode.ld, "Rb,Ib"),

            new OpRec(Opcode.push, "Rw"),
            new OpRec(Opcode.push, "Rw"),
            new OpRec(Opcode.push, "Rw"),
            new OpRec(Opcode.push, "Rw"),
                                        
            new OpRec(Opcode.push, "Rw"),
            new OpRec(Opcode.push, "Rw"),
            new OpRec(Opcode.push, "Rw"),
            new OpRec(Opcode.push, "Rw"),
            // 30
            new OpRec(Opcode.ld, "Rw,Iw"),
            new OpRec(Opcode.ld, "Rw,Iw"),
            new OpRec(Opcode.ld, "Rw,Iw"),
            new OpRec(Opcode.ld, "Rw,Iw"),

            new OpRec(Opcode.ld, "Rw,Iw"),
            new OpRec(Opcode.ld, "Rw,Iw"),
            new OpRec(Opcode.ld, "Rw,Iw"),
            new OpRec(Opcode.ld, "Rw,Iw"),

            new OpRec(Opcode.push, "Rx"),
            new OpRec(Opcode.push, "Rx"),
            new OpRec(Opcode.push, "Rx"),
            new OpRec(Opcode.push, "Rx"),

            new OpRec(Opcode.push, "Rx"),
            new OpRec(Opcode.push, "Rx"),
            new OpRec(Opcode.push, "Rx"),
            new OpRec(Opcode.push, "Rx"),
            // 40
            new OpRec(Opcode.ld, "Rx,Ix"),
            new OpRec(Opcode.ld, "Rx,Ix"),
            new OpRec(Opcode.ld, "Rx,Ix"),
            new OpRec(Opcode.ld, "Rx,Ix"),

            new OpRec(Opcode.ld, "Rx,Ix"),
            new OpRec(Opcode.ld, "Rx,Ix"),
            new OpRec(Opcode.ld, "Rx,Ix"),
            new OpRec(Opcode.ld, "Rx,Ix"),

            new OpRec(Opcode.pop, "Rw"),
            new OpRec(Opcode.pop, "Rw"),
            new OpRec(Opcode.pop, "Rw"),
            new OpRec(Opcode.pop, "Rw"),

            new OpRec(Opcode.pop, "Rw"),
            new OpRec(Opcode.pop, "Rw"),
            new OpRec(Opcode.pop, "Rw"),
            new OpRec(Opcode.pop, "Rw"),
            // 50
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.pop, "Rx"),
            new OpRec(Opcode.pop, "Rx"),
            new OpRec(Opcode.pop, "Rx"),
            new OpRec(Opcode.pop, "Rx"),

            new OpRec(Opcode.pop, "Rx"),
            new OpRec(Opcode.pop, "Rx"),
            new OpRec(Opcode.pop, "Rx"),
            new OpRec(Opcode.pop, "Rx"),
            // 60
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),

            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),

            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),

            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            new OpRec(Opcode.jr, "C,jb"),
            // 70
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),

            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),

            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),

            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            new OpRec(Opcode.jr, "C,jw"),
            // 80
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),

            new MemOpRec("Mb"),
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),

            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            new MemOpRec("Nb"),

            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            // 90
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),

            new MemOpRec("Mw"),
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),

            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            new MemOpRec("Nw"),

            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            // A0
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),

            new MemOpRec("Mx"),
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),

            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            new MemOpRec("Nx"),

            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            // B0
            new DstOpRec("M?"),
            new DstOpRec("M?"),
            new DstOpRec("M?"),
            new DstOpRec("M?"),

            new DstOpRec("M?"),
            new DstOpRec("M?"),
            new DstOpRec("M?"),
            new DstOpRec("M?"),

            new DstOpRec("N?"),
            new DstOpRec("N?"),
            new DstOpRec("N?"),
            new DstOpRec("N?"),

            new DstOpRec("N?"),
            new DstOpRec("N?"),
            new DstOpRec("N?"),
            new DstOpRec("N?"),
            // C0
            new MemOpRec("Ob"),
            new MemOpRec("Pb"),
            new MemOpRec("Qb"),
            new MemOpRec("mb"),

            new MemOpRec("-b"),
            new MemOpRec("+b"),
            new OpRec(Opcode.invalid, ""),
            new ExtraRegOprec("b"),

            new RegOpRec("rb"),
            new RegOpRec("rb"),
            new RegOpRec("rb"),
            new RegOpRec("rb"),

            new RegOpRec("rb"),
            new RegOpRec("rb"),
            new RegOpRec("rb"),
            new RegOpRec("rb"),
            // D0
            new MemOpRec("Ow"),
            new MemOpRec("Pw"),
            new MemOpRec("Qw"),
            new MemOpRec("mw"),

            new MemOpRec("-w"),
            new MemOpRec("+w"),
            new OpRec(Opcode.invalid, ""),
            new ExtraRegOprec("w"),

            new RegOpRec("rw"),
            new RegOpRec("rw"),
            new RegOpRec("rw"),
            new RegOpRec("rw"),

            new RegOpRec("rw"),
            new RegOpRec("rw"),
            new RegOpRec("rw"),
            new RegOpRec("rw"),
            // E0
            new MemOpRec("Ox"),
            new MemOpRec("Px"),
            new MemOpRec("Qx"),
            new MemOpRec("mx"),

            new MemOpRec("-x"),
            new MemOpRec("+x"),
            new OpRec(Opcode.invalid, ""),
            new ExtraRegOprec("l"),

            new RegOpRec("rx"),
            new RegOpRec("rx"),
            new RegOpRec("rx"),
            new RegOpRec("rx"),

            new RegOpRec("rx"),
            new RegOpRec("rx"),
            new RegOpRec("rx"),
            new RegOpRec("rx"),

            // F0
            new DstOpRec("O?"),
            new DstOpRec("P?"),
            new DstOpRec("Q?"),
            new DstOpRec("m?"),

            new DstOpRec("-?"),
            new DstOpRec("+?"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.swi, "3b"),
            new OpRec(Opcode.swi, "3b"),
            new OpRec(Opcode.swi, "3b"),
            new OpRec(Opcode.swi, "3b"),

            new OpRec(Opcode.swi, "3b"),
            new OpRec(Opcode.swi, "3b"),
            new OpRec(Opcode.swi, "3b"),
            new OpRec(Opcode.swi, "3b"),
        };
    }
}

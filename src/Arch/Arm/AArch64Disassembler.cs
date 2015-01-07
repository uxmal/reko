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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class AArch64Disassembler2 : DisassemblerBase<AArch64Instruction>
    {
        internal ImageReader rdr;
        internal AArch64Instruction instr;

        public AArch64Disassembler2(ImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override AArch64Instruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;
            uint opcode = rdr.ReadLeUInt32();
            instr = oprecs[(opcode >> 25) & 0xF].Decode(this, opcode);
            instr.Address = addr;
            instr.Length = rdr.Address - addr;
            return instr;
        }

        private abstract class OpDecoder2
        {
            public abstract AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr);
        }

        private class AAOprec2 : OpDecoder2
        {
            private AA64Opcode opcode;
            protected string format;

            public AAOprec2(AA64Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.format = fmt;
            }

            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                return Decode(dasm, instr, format);
            }

            public AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr, string fmt)
            {
                AA64Opcode opcode = this.opcode;
                List<MachineOperand> ops = new List<MachineOperand>();
                MachineOperand op = null;
                int i = 0;
                int off;
                while (i < fmt.Length)
                {
                    switch (fmt[i++])
                    {
                    default: throw new InvalidOperationException(string.Format("Bad format character {0}.", fmt[i - 1]));
                    case ',':
                        continue;
                    case 'B':   // Logical Bitmask
                        op = LogicalBitmask(instr, fmt[i++] == 'x');
                        break;
                    case 'H':   // 16-bit Immediate constant
                        off = GetOffset(fmt, ref i);
                        op = ArmImmediateOperand.Word32(GetImm(instr, off, 16));
                        break;
                    case 'I':   // 12-bit Immediate constant
                        off = GetOffset(fmt, ref i);
                        op = ArmImmediateOperand.Word32(GetImm(instr, off, 12));
                        break;
                    case 'J':   // long relative branch
                        int offset = (((int) instr) << 6) >> 4;
                        op = new AddressOperand(dasm.rdr.Address + offset);
                        break;
                    case 'W':   // (32-bit) W register operand
                        op = GetWReg(instr, GetOffset(fmt, ref i));
                        break;
                    case 'X':   // (64-bit) X register operand 
                        op = GetXReg(instr, GetOffset(fmt, ref i));
                        break;
                    case 's':   // Shift operand by 12
                        off = GetOffset(fmt, ref i);
                        op = ops[ops.Count - 1];
                        ops.RemoveAt(ops.Count - 1);
                        uint shiftCode = GetImm(instr, off, 2);
                        switch (shiftCode)
                        {
                        case 0: break;
                        case 1:
                            op = new ShiftOperand(op, Opcode.lsl, 12); break;
                        default: throw new FormatException("Reserved value for shift code.");
                        }
                        break;
                    }
                    ops.Add(op);
                }
                return AArch64Instruction.Create(opcode, ops);
            }

            private int GetOffset(string fmt, ref int i)
            {
                int offset = 0;
                while (i < fmt.Length)
                {
                    char ch = fmt[i];
                    if (!Char.IsDigit(ch))
                        return offset;
                    offset = offset * 10 + (ch - '0');
                    ++i;
                }
                return offset;
            }

            private uint GetImm(uint instr, int off, int length)
            {
                return (instr >> off) & ((1u << length) - 1u);
            }

            private MachineOperand GetXReg(uint instr, int regPos)
            {
                int reg = (int) ((instr >> regPos) & 0x1F);
                return new RegisterOperand(A64Registers.GetXReg(reg));
            }

            private MachineOperand GetWReg(uint instr, int regPos)
            {
                int reg = (int) ((instr >> regPos) & 0x1F);
                return new RegisterOperand(A64Registers.GetWReg(reg));
            }

            // https://sourceware.org/git/gitweb.cgi?p=binutils-gdb.git;a=blob;f=opcodes/aarch64-dis.c;h=c11f78f78ab8569db6ebf5295d8047aeeee338bd;hb=master
            // line 694

            /* Decode logical immediate for e.g. ORR <Wd|WSP>, <Wn>, #<imm>.  */

            ArmImmediateOperand LogicalBitmask(uint value, bool is64)
            {
                ulong imm, mask;
                uint N, R, S;
                uint simd_size;

                // value = extract_fields (code, 0, 3, FLD_N, FLD_immr, FLD_imms);
                //sf = aarch64_get_qualifier_esize (inst->operands[0].qualifier) != 4;
                // value is N:immr:imms.
                S = (value >> 10) & 0x3F;
                R = (value >> 16) & 0x3F;
                N = (value >> 22) & 0x01;

                if (!is64 && N == 1)
                    throw new InvalidOperationException();

                // The immediate value is S+1 bits to 1, left rotated by SIMDsize - R
                // (in other words, right rotated by R), then replicated.
                if (N != 0)
                {
                    simd_size = 64;
                    mask = 0xFFFFFFFFFFFFFFFFul;
                }
                else
                {
                    switch (S)
                    {
                    case 0x00:
                    case 0x01:
                    case 0x02:
                    case 0x03:
                    case 0x04:
                    case 0x05:
                    case 0x06:
                    case 0x07:
                    case 0x08:
                    case 0x09:
                    case 0x0A:
                    case 0x0B:
                    case 0x0C:
                    case 0x0D:
                    case 0x0E:
                    case 0x0F:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1A:
                    case 0x1B:
                    case 0x1C:
                    case 0x1D:
                    case 0x1E:
                    case 0x1F:
                        /* 0xxxxx */
                        simd_size = 32;
                        break;
                    case 0x20:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 0x28:
                    case 0x29:
                    case 0x2A:
                    case 0x2B:
                    case 0x2C:
                    case 0x2D:
                    case 0x2E:
                    case 0x2F:
                        /* 10xxxx */
                        simd_size = 16; S &= 0xf; break;
                    case 0x30:
                    case 0x31:
                    case 0x32:
                    case 0x33:
                    case 0x34:
                    case 0x35:
                    case 0x36:
                    case 0x37:
                        /* 110xxx */
                        simd_size = 8; S &= 0x7; break;
                    case 0x38:
                    case 0x39:
                    case 0x3A:
                    case 0x3B:
                        /* 1110xx */
                        simd_size = 4; S &= 0x3; break;
                    case 0x3C:
                    case 0x3D:
                    case 0x3E:
                    case 0x3F:
                        /* 11110x */
                        simd_size = 2; S &= 0x1; break;
                    default: throw new InvalidOperationException();
                    }
                    mask = ((1ul << (int) simd_size) - 1);
                    /* Top bits are IGNORED.  */
                    R &= simd_size - 1;
                }
                /* NOTE: if S = simd_size - 1 we get 0xf..f which is rejected.  */
                if (S == simd_size - 1)
                    throw new InvalidOperationException();
                /* S+1 consecutive bits to 1.  */
                /* NOTE: S can't be 63 due to detection above.  */
                imm = (1ul << (int) (S + 1)) - 1;
                /* Rotate to the left by simd_size - R.  */
                if (R != 0)
                    imm = ((imm << (int) (simd_size - R)) & mask) | (imm >> (int) R);
                /* Replicate the value according to SIMD size.  */
                switch (simd_size)
                {
                case 2: imm = (imm << 2) | imm; goto case 4;
                case 4: imm = (imm << 4) | imm; goto case 8;
                case 8: imm = (imm << 8) | imm; goto case 16;
                case 16: imm = (imm << 16) | imm; goto case 32;
                case 32: imm = (imm << 32) | imm; break;
                case 64: break;
                default: throw new InvalidOperationException();
                }

                return is64
                    ? ArmImmediateOperand.Word64(imm)
                    : ArmImmediateOperand.Word32((int) imm);
            }
        }

        private class RootDecoder : OpDecoder2
        {
            private OpDecoder2[] opdecs;

            public RootDecoder(OpDecoder2[] opdecs)
            {
                this.opdecs = opdecs;
            }

            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                return opdecs[(instr >> 24) & 0x0F].Decode(dasm, instr);
            }
        }

        private class DataIDecoder : OpDecoder2
        {
            private OpDecoder2[] opdecs;

            public DataIDecoder(OpDecoder2[] opdecs)
            {
                this.opdecs = opdecs;
            }

            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                return opdecs[(instr >> 23) & 0x07].Decode(dasm, instr);
            }
        }

        private static OpDecoder2[] oprecs;
        private static OpDecoder2[] opBesRecs;              // Branch; Exception and system instructions

        private class MaskDecoder : OpDecoder2
        {
            int shift;
            int mask; 
            OpDecoder2[] oprecs;

            public MaskDecoder(int shift, int mask, OpDecoder2[] oprecs)
            {
                this.shift= shift;
                this.mask = mask;
                this.oprecs = oprecs;
            }

            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint opcode)
            {
                return oprecs[(opcode >> shift) & mask].Decode(dasm, opcode);
            }
        }

        class AddSubIDecoder : OpDecoder2
        {
            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                throw new NotImplementedException();
            }
        }

        class BitFieldDecoder : OpDecoder2
        {
            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                throw new NotImplementedException();
            }
        }

        class ExtractDecoder : OpDecoder2
        {
            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                throw new NotImplementedException();
            }
        }

        class LogicalIDecoder : OpDecoder2
        {
            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                throw new NotImplementedException();
            }
        }

        class MoveWideIDecoder : OpDecoder2
        {
            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                throw new NotImplementedException();
            }
        }

        class ShiftOpRec : OpDecoder2
        {
            AA64Opcode opcode;
            string format;

            public ShiftOpRec(AA64Opcode opcode, string format)
            {
                this.opcode = opcode;
                this.format = format;
            }

            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint opcode)
            {
                throw new NotImplementedException();
            }
        }

        private class DataImmediateOpRec : OpDecoder2
        {
            private OpDecoder2[] oprecs;

            public DataImmediateOpRec(OpDecoder2[] oprecs)
            {
                this.oprecs = oprecs;
            }

            public override AArch64Instruction Decode(AArch64Disassembler2 dasm, uint instr)
            {
                return oprecs[(instr >> 23) & 7].Decode(dasm, instr);
            }
        }

        static AArch64Disassembler2()
        {
            var opPcRelData = new MaskDecoder(31, 1,
                new OpDecoder2[]
                {
                    new AAOprec2(AA64Opcode.adr, "r0,I5"),
                    new AAOprec2(AA64Opcode.adrp, "r0,I5"),
                });
            var opAddSubI = new MaskDecoder(29, 0x7,
                new OpDecoder2[] 
                {
                    new ShiftOpRec(AA64Opcode.add, "W0,W5,A"),
                    new ShiftOpRec(AA64Opcode.adds, "W0,W5,A"),
                    new ShiftOpRec(AA64Opcode.sub, "W0,W5,A"),
                    new ShiftOpRec(AA64Opcode.subs, "W0,W5,A"),
                    new ShiftOpRec(AA64Opcode.add, "X0,X5,A"),
                    new ShiftOpRec(AA64Opcode.adds, "X0,X5,A"),
                    new ShiftOpRec(AA64Opcode.sub, "X0,X5,A"),
                    new ShiftOpRec(AA64Opcode.subs, "X0,X5,A"),
                });

            var opDataIRecs = new MaskDecoder(23, 7,
                new OpDecoder2 []
                {
                    opPcRelData,
                    opPcRelData,
                    opAddSubI,
                    opAddSubI,
                    new LogicalIDecoder(),
                    new MoveWideIDecoder(),
                    new BitFieldDecoder(),
                    new ExtractDecoder(),
                });

            opBesRecs = new OpDecoder2[] { };

            oprecs = new OpDecoder2[] {
                // 00
                null,   // Unallocated
                null, 
                null,
                null, 
      
                null, //opLdStRecs,             // Load and Store instructions
                null, //opDataRegRecs,          // Data processing - register
                null, //opLdStRecs,             // Load and Store instructions
                null, //opSimdFpRecs0,          // Data processSIMD and FP  

                opDataIRecs,            // Data processing - immediate
                opDataIRecs,            // Data processing - immediate
                null, //opBesRecs,              // Branch, Exception and system instructions
                null, //opBesRecs,              // Branch, Exception and system instructions
                null, 
                null, //opLdStRecs,             // Load and Store instructions
                null, //opDataRegRecs,          // Data processing - register
                null, //opLdStRecs,             // Load and Store instructions
                null, //opSimdFpRecs1,          // Data processSIMD and FP  
            };
        }
    }

    public class AArch64Disassembler : DisassemblerBase<AArch64Instruction>
    {
        internal ImageReader rdr;
        internal AArch64Instruction instr;

        public AArch64Disassembler(ImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override AArch64Instruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;
            uint opcode = rdr.ReadLeUInt32();
            instr = oprecs[(opcode >> 24) & 0xFF].Decode(this, opcode);
            instr.Address = addr;
            instr.Length = rdr.Address - addr;
            return instr;
        }

        private class RootDecoder : OpDecoder
        {
            private OpDecoder[] opdecs;

            public RootDecoder(OpDecoder[] opdecs)
            {
                this.opdecs = opdecs;
            }

            public override AArch64Instruction Decode(AArch64Disassembler dasm, uint instr)
            {
                return opdecs[(instr >> 28) & 0x0F].Decode(dasm, instr);
            }
        }

        static OpDecoder[] oprecs02 = 
        {
            null,
            new AAOprec(AA64Opcode.b, "J"), 
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            new AAOprec(AA64Opcode.bl, "J"), 
            null,
            null, 

            null,
            null,
            null,
            null, 
        };

        static OpDecoder [] oprecs03 = 
        {
            null,
            new AAOprec(AA64Opcode.b, "J"), 
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            new AAOprec(AA64Opcode.bl, "J"), 
            null,
            null, 

            null,
            null,
            null,
            null, 
        };

        static OpDecoder[] oprecs = new OpDecoder[] {
            // 00
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            // 10
            null,
            new AAOprec(AA64Opcode.add, "W0,W5,I10s22"), 
            new AAOprec(AA64Opcode.and, "W0,W5,Bw"),
            null, 
      
            new AAOprec(AA64Opcode.b, "J"),
            new AAOprec(AA64Opcode.b, "J"),
            new AAOprec(AA64Opcode.b, "J"),
            new AAOprec(AA64Opcode.b, "J"),

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // 20
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // 30
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // 40
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            new AAOprec(AA64Opcode.br, "X5"),
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // 50
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            new AAOprec(AA64Opcode.br, "X5"),
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // 60
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // 70
            null,
            new AAOprec(AA64Opcode.subs,  "W0,W5,I10s22"), 
            null,
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // 80
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // 90
            null,
            null, 
            new AAOprec(AA64Opcode.and, "X0,X5,Bx"),
            null, 
      
            new AAOprec(AA64Opcode.bl, "J"),
            new AAOprec(AA64Opcode.bl, "J"),
            new AAOprec(AA64Opcode.bl, "J"),
            new AAOprec(AA64Opcode.bl, "J"),

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // A0
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // B0
            null,
            new AAOprec(AA64Opcode.adds, "X0,X5,I10s22"), 
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // C0
            null,
            null, 
            null,
            null, 
      
            null,
            new AAOprec(AA64Opcode.br, "X5"),
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // D0
            null,
            new AAOprec(AA64Opcode.sub, "X0,X5,I10s22"), 
            null,
            null, 
      
            null,
            null,
            new AAOprec(AA64Opcode.br, "X5"),
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // E0
            null,
            null, 
            null,
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

             // F0
            null,
            null, 
            new LogMovImmOprec(AA64Opcode.ands, "X0,X5,Bx", AA64Opcode.movk, "X0,H5"),
            null, 
      
            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 

            null,
            null,
            null,
            null, 
        };

        internal ArmImmediateOperand Word64(ulong imm)
        {
            return ArmImmediateOperand.Word64(imm);
        }

        internal ArmImmediateOperand Word32(int imm)
        {
            return ArmImmediateOperand.Word32(imm);
        }
    }
    /*
Ronald Maas (rmaas.delete@this.wiwo.nl) on August 25, 2012 6:59 pm wrote:
> Recently someone posted a cross-compiled version of the ARM 64-bit binutils on the 
> web (Google for aarch64-cross.tar.gz). Just out of curiosity, I used this tool to try 
> to understand more about the instrucion format used for ARM 64-bit instructions.
> 
> Note that I do not have access to official ARM 64-bit documentation on instruction formats, 
> ARM 64-bit hardware or simulators, so I was not able to verify anything described here. 
> I do hope it is accurate enough for anyone interested to give some prelimirary insight, 
> until ARM releases the full documentation set of their new ARM 64-bit architecture.
> 
> Here the list of ARM 64-bits opcodes as far as I understand these 
> based on the output of the ARM 64-bit assembler and objdump.
> 
> 
> 
> Conditional Branch
> ------------------
> r0110100 bbbbbbbb bbbbbbbb bbbnnnnn     cbz Rn, label                   r:{0=W,1=X}
> r0110101 bbbbbbbb bbbbbbbb bbbnnnnn     cbnz Rn, label                  r:{0=W,1=X}
> r0110110 iiiiibbb bbbbbbbb bbbnnnnn     tbz Rn, #uimm6, label           r:{0=W,1=X}
> r0110111 iiiiibbb bbbbbbbb bbbnnnnn     tbnz Rn, #uimm6, label          r:{0=W,1=X}
> 01010100 bbbbbbbb bbbbbbbb bbb0cccc     b.cond label                    cccc:{0000=eq,0001=ne,0010=hs,...}
> 
> Unconditional Branch
> --------------------
> 000101bb bbbbbbbb bbbbbbbb bbbbbbbb     b label
> 100101bb bbbbbbbb bbbbbbbb bbbbbbbb     bl label
> 11010110 00011111 000000nn nnn00000     br Xn
> 11010110 00111111 000000nn nnn00000     blr Xn
> 11010110 01011111 000000nn nnn00000     ret Xn
> 
> Load/Store Single Register
> --------------------------
> 00011000 bbbbbbbb bbbbbbbb bbbnnnnn     ldr Wn, label                   pc+(b<<2)
> 00111000 000iiiii iiiippbb bbbnnnnn     strb Wn, [Xb, #imm9]        
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 00111000 001mmmmm ssss10bb bbbnnnnn     strb Wn, [Xb, Rm 
> {,shift|ext}]  ssss:{0100=uxtw,0110=no shift,1100=sxtw}
> 00111001 00iiiiii iiiiiibb bbbnnnnn     strb Wn, [Xb, #uimm12]          unsigned, scaled
> 00111000 010iiiii iiiippbb bbbnnnnn     ldrb Wn, [Xb, #imm9]        
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 00111000 011mmmmm ssss10bb bbbnnnnn     ldrb Wn, [Xb, Rm 
> {,shift|ext}]  ssss:{0100=uxtw,0110=no shift,1100=sxtw}
> 00111001 01iiiiii iiiiiibb bbbnnnnn     ldrb Wn, [Xb, #uimm12]          unsigned, scaled
> 00111000 100iiiii iiiippbb bbbnnnnn     ldrsb Xn, [Xb, #imm9]       
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 00111000 101mmmmm ssss10bb bbbnnnnn     ldrsb Xn, [Xb, Rm 
> {,shift|ext}] ssss:{0100=uxtw,0110=no shift,1100=sxtw}
> 00111001 10iiiiii iiiiiibb bbbnnnnn     ldrsb Xn, [Xb, #uimm12]         unsigned, scaled
> 00111000 110iiiii iiiippbb bbbnnnnn     ldrsb Wn, [Xb, #imm9]       
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 00111000 111mmmmm ssss10bb bbbnnnnn     ldrsb Wn, [Xb, Rm 
> {,shift|ext}] ssss:{0100=uxtw,0110=no shift,1100=sxtw}
> 00111001 11iiiiii iiiiiibb bbbnnnnn     ldrsb Wn, [Xb, #uimm12]         unsigned, scaled
> 01011000 bbbbbbbb bbbbbbbb bbbnnnnn     ldr Xn, label                   pc+(b<<2)
> 01111000 000iiiii iiiippbb bbbnnnnn     strh Wn, [Xb, #imm9]        
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 01111000 001mmmmm ssss10bb bbbnnnnn     strh Wn, [Xb, Rm {,shift|ext}]  ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 01111001 00iiiiii iiiiiibb bbbnnnnn     strh Wn, [Xb, #uimm12]          unsigned, scaled
> 01111000 010iiiii iiiippbb bbbnnnnn     ldrh Wn, [Xb, #imm9]        
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 01111000 011mmmmm ssss10bb bbbnnnnn     ldrh Wn, [Xb, Rm {,shift|ext}]  ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 01111001 01iiiiii iiiiiibb bbbnnnnn     ldrh Wn, [Xb, #uimm12]          unsigned, scaled
> 01111000 100iiiii iiiippbb bbbnnnnn     ldrsh Xn, [Xb, #imm9]       
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 01111000 101mmmmm ssss10bb bbbnnnnn     ldrsh Xn, [Xb, Rm {,shift|ext}] ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 01111001 10iiiiii iiiiiibb bbbnnnnn     ldrsh Xn, [Xb, #uimm12]         unsigned, scaled
> 01111000 110iiiii iiiippbb bbbnnnnn     ldrsh Wn, [Xb, #imm9]       
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 01111000 111mmmmm ssss10bb bbbnnnnn     ldrsh Wn, [Xb, Rm {,shift|ext}] ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 01111001 11iiiiii iiiiiibb bbbnnnnn     ldrsh Wn, [Xb, #uimm12]         unsigned, scaled
> 10011000 bbbbbbbb bbbbbbbb bbbnnnnn     ldrsw Xn, label                 pc+(b<<2)
> 10111000 000iiiii iiiippbb bbbnnnnn     str Wn, [Xb, #imm9]         
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 10111000 001mmmmm ssss10bb bbbnnnnn     str Wn, [Xb, Rm {,shift|ext}]   ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 10111001 00iiiiii iiiiiibb bbbnnnnn     str Wn, [Xb, #uimm12]           unsigned, scaled
> 10111000 010iiiii iiiippbb bbbnnnnn     ldr Wn, [Xb, #imm9]         
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 10111000 011mmmmm ssss10bb bbbnnnnn     ldr Wn, [Xb, Rm {,shift|ext}]   ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 10111001 01iiiiii iiiiiibb bbbnnnnn     ldr Wn, [Xb, #uimm12]           unsigned, scaled
> 10111000 100iiiii iiiippbb bbbnnnnn     ldrsw Xn, [Xb, #imm9]       
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 10111000 101mmmmm ssss10bb bbbnnnnn     ldrsw Xn, [Xb, Rm {,shift|ext}] ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 10111001 10iiiiii iiiiiibb bbbnnnnn     ldrsw Xn, [Xb, #uimm12]         unsigned, scaled
> 11111000 000iiiii iiiippbb bbbnnnnn     str Xn, [Xb, #imm9]         
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 11111000 001mmmmm ssss10bb bbbnnnnn     str Xn, [Xb, Rm {,shift|ext}]   ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 11111001 00iiiiii iiiiiibb bbbnnnnn     str Xn, [Xb, #uimm12]           unsigned, scaled
> 11111000 010iiiii iiiippbb bbbnnnnn     ldr Xn, [Xb, #imm9]         
>     signed, unscaled, pp:{00=offset,01=post-indexed,11=pre-indexed}
> 11111000 011mmmmm ssss10bb bbbnnnnn     ldr Xn, [Xb, Rm {,shift|ext}]   ssss:{0100=uxtw,0101=uxtw 
> #imm,0110=no shift,0111=lsl #imm,1100=sxtw,1101=sxtw #imm}
> 11111001 01iiiiii iiiiiibb bbbnnnnn     ldr Xn, [Xb, #uimm12]           unsigned, scaled
> 
> Load/Store Pair
> ---------------
> 0010100p p0iiiiii immmmmbb bbbnnnnn     stp Wn, Wm, [Xb, #imm7]     
>     signed, scaled, pp:{01=post-indexed,10=offset, 11=pre-indexed}
> 0010100p p1iiiiii immmmmbb bbbnnnnn     ldp Wn, Wm, [Xb, #imm7]     
>     signed, scaled, pp:{01=post-indexed,10=offset, 11=pre-indexed}
> 0110100p p1iiiiii immmmmbb bbbnnnnn     ldpsw Xn, Xm, [Xb, #imm7]   
>     signed, scaled, pp:{01=post-indexed,10=offset, 11=pre-indexed}
> 1010100p p0iiiiii immmmmbb bbbnnnnn     stp Xn, Xm, [Xb, #imm7]     
>     signed, scaled, pp:{01=post-indexed,10=offset, 11=pre-indexed}
> 1010100p p1iiiiii immmmmbb bbbnnnnn     ldp Xn, Xm, [Xb, #imm7]     
>     signed, scaled, pp:{01=post-indexed,10=offset, 11=pre-indexed}
> 
> Data Processing (Immediate)
> ---------------------------
> 00010000 bbbbbbbb bbbbbbbb bbbddddd     adr Rd, label                   pc+(b<<2)
> 10010000 bbbbbbbb bbbbbbbb bbbddddd     adrp Rd, label                  pc+(b<<2)
> r0010001 0siiiiii iiiiiinn nnnddddd     add Rd, Rn, #uimm12 {,shift}    r:{0=W,1=X}, s:{0=no shift,1=lsl 12}
> r0010010 0?iiiiii iiiiiinn nnnddddd     and Rd, Rn, #bimm32|64          r:{0=W,1=X}
> r0010010 1ssiiiii iiiiiiii iiiddddd     movn Rd, #uimm16 {,shift} 
>       r:{0=W,1=X}, ss:{00=no shift,01=lsl 16,10=lsl 32,11=lsl 48}
> r0010011 0?xxxxxx yyyyyynn nnnddddd     ubfm Rd, Rn, #x, #y             r:{0=W,1=X}
> r0010011 1?xxxxxx yyyyyynn nnnddddd     extr Rd, Rn, #x, #y             r:{0=W,1=X}
> r0110001 0siiiiii iiiiiinn nnnddddd     adds Rd, Rn, #uimm12 {,shift}   r:{0=W,1=X}, s:{0=no shift,1=lsl 12}
> r0110010 0?iiiiii iiiiiinn nnnddddd     orr Rd, Rn, #bimm32|64          r:{0=W,1=X}
> r0110011 0?xxxxxx yyyyyynn nnnddddd     bfm Rd, Rn, #x, #y              r:{0=W,1=X}
> r1010001 0siiiiii iiiiiinn nnnddddd     sub Rd, Rn, #uimm12 {,shift}    r:{0=W,1=X}, s:{0=no shift,1=lsl 12}
> r1010010 0?iiiiii iiiiiinn nnnddddd     eor Rd, Rn, #bimm32|64          r:{0=W,1=X}
> r1010010 1ssiiiii iiiiiiii iiiddddd     movz Rd, #uimm16 {,shift} 
>       r:{0=W,1=X}, ss:{00=no shift,01=lsl 16,10=lsl 32,11=lsl 48}
> r1010011 0?xxxxxx yyyyyynn nnnddddd     ubfm Rd, Rn, #x, #y             r:{0=W,1=X}
> r1110001 0siiiiii iiiiiinn nnnddddd     subs Rd, Rn, #uimm12 {,shift}   r:{0=W,1=X}, s:{0=no shift,1=lsl 12}
> r1110010 0?iiiiii iiiiiinn nnnddddd     ands Rd, Rn, #bimm32|64         r:{0=W,1=X}
> r1110010 1ssiiiii iiiiiiii iiiddddd     movk Rd, #uimm16 {,shift} 
>       r:{0=W,1=X}, ss:{00=no shift,01=lsl 16,10=lsl 32,11=lsl 48}
> 
> Data Processing (Register)
> --------------------------
> r0001010 ll0mmmmm ssssssnn nnnddddd     and Rd, Rn, Rm {,lshift}        r:{0=W,1=X}, 
> ll:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6,11=ror #uimm6}
> r0001010 ll1mmmmm ssssssnn nnnddddd     bic Rd, Rn, Rm {,lshift}        r:{0=W,1=X}, 
> ll:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6,11=ror #uimm6}
> r0001011 aa0mmmmm ssssssnn nnnddddd     add Rd, Rn, Rm {,ashift}  
>       r:{0=W,1=X}, aa:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6}
> r0001011 001mmmmm eeesssnn nnnddddd     add Rd, Rn, Wm, extend {#imm3}  r:{0=W,1=X}, 
> eee:{000=uxtb,001=uxth,010=uxtw,011=uxtx,100=sxtb,101=sxth,110=sxtw,111=sxtx}
> r0011010 110mmmmm 001000nn nnnddddd     lslv Rd, Rn, Rm                 r:{0=W,1=X}
> r0011010 110mmmmm 001001nn nnnddddd     lsrv Rd, Rn, Rm                 r:{0=W,1=X}
> r0011010 110mmmmm 001010nn nnnddddd     asrv Rd, Rn, Rm                 r:{0=W,1=X}
> r0011010 110mmmmm 001011nn nnnddddd     rorv Rd, Rn, Rm                 r:{0=W,1=X}
> r0011011 000mmmmm 0aaaaann nnnddddd     madd Rd, Rn, Rm, Ra             r:{0=W,1=X}
> r0011011 000mmmmm 1aaaaann nnnddddd     msub Rd, Rn, Rm, Ra             r:{0=W,1=X}
> 10011011 001mmmmm 0aaaaann nnnddddd     smaddl Xd, Wn, Wm, Xa
> 10011011 001mmmmm 1aaaaann nnnddddd     smsubl Xd, Wn, Wm, Xa
> 10011011 101mmmmm 0aaaaann nnnddddd     umaddl Xd, Wn, Wm, Xa
> 10011011 101mmmmm 1aaaaann nnnddddd     umsubl Xd, Wn, Wm, Xa
> r0101010 ll0mmmmm ssssssnn nnnddddd     orr Rd, Rn, Rm {,lshift}        r:{0=W,1=X}, 
> ll:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6,11=ror #uimm6}
> r0101010 ll1mmmmm ssssssnn nnnddddd     orn Rd, Rn, Rm {,lshift}        r:{0=W,1=X}, 
> ll:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6,11=ror #uimm6}
> r0101011 aa0mmmmm ssssssnn nnnddddd     adds Rd, Rn, Rm {,ashift} 
>       r:{0=W,1=X}, aa:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6}
> r0101011 001mmmmm eeesssnn nnnddddd     adds Rd, Rn, Wm, extend {#imm3} r:{0=W,1=X}, 
> eee:{000=uxtb,001=uxth,010=uxtw,011=uxtx,100=sxtb,101=sxth,110=sxtw,111=sxtx}
> r1001010 ll0mmmmm ssssssnn nnnddddd     eor Rd, Rn, Rm {,lshift}        r:{0=W,1=X}, 
> ll:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6,11=ror #uimm6}
> r1001010 ll1mmmmm ssssssnn nnnddddd     eon Rd, Rn, Rm {,lshift}        r:{0=W,1=X}, 
> ll:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6,11=ror #uimm6}
> r1001011 aa0mmmmm ssssssnn nnnddddd     sub Rd, Rn, Rm {,ashift}  
>       r:{0=W,1=X}, aa:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6}
> r1001011 001mmmmm eeesssnn nnnddddd     sub Rd, Rn, Wm, extend {#imm3}  r:{0=W,1=X}, 
> eee:{000=uxtb,001=uxth,010=uxtw,011=uxtx,100=sxtb,101=sxth,110=sxtw,111=sxtx}
> r1011010 11000000 000000nn nnnddddd     rbit Rd, Rn                     r:{0=W,1=X}
> r1011010 11000000 000001nn nnnddddd     rev16 Rd, Rn                    r:{0=W,1=X}
> 01011010 11000000 000010nn nnnddddd     rev Wd, Wn
> 11011010 11000000 000010nn nnnddddd     rev32 Xd, Xn
> 11011010 11000000 000011nn nnnddddd     rev Xd, Xn
> r1011010 11000000 000100nn nnnddddd     clz Rd, Rn                      r:{0=W,1=X}
> r1011010 11000000 000101nn nnnddddd     cls Rd, Rn                      r:{0=W,1=X}
> r1101010 ll0mmmmm ssssssnn nnnddddd     ands Rd, Rn, Rm {,lshift}       r:{0=W,1=X}, 
> ll:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6,11=ror #uimm6}
> r1101010 ll1mmmmm ssssssnn nnnddddd     bics Rd, Rn, Rm {,lshift}       r:{0=W,1=X}, 
> ll:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6,11=ror #uimm6}
> r1101011 aa0mmmmm ssssssnn nnnddddd     subs Rd, Rn, Rm {,ashift} 
>       r:{0=W,1=X}, aa:{00=lsl #uimm6,01=lsr #uimm6,10=asr #uimm6}
> r1101011 001mmmmm eeesssnn nnnddddd     subs Rd, Rn, Wm, extend {#imm3} r:{0=W,1=X}, 
> eee:{000=uxtb,001=uxth,010=uxtw,011=uxtx,100=sxtb,101=sxth,110=sxtw,111=sxtx}
> 
> 
> Data Processing (Conditional)
> -----------------------------
> r0011010 000mmmmm 000000nn nnnddddd     adc Rd, Rn, Rm                  r:{0=W,1=X}
> r0011010 100mmmmm cccc00nn nnnddddd     csel Rd, Rn, Rm, cond 
>           r:{0=W,1=X}, cccc:{0000=eq,0001=ne,0010=hs,...}
> r0011010 100mmmmm cccc01nn nnnddddd     csinc Rd, Rn, Rm, cond 
>          r:{0=W,1=X}, cccc:{0000=eq,0001=ne,0010=hs,...}
> r0111010 000mmmmm 000000nn nnnddddd     adcs Rd, Rn, Rm                 r:{0=W,1=X}
> r0111010 010mmmmm cccc00nn nnn0yyyy     ccmn Rn, Rm, #y     
>             r:{0=W,1=X}, cccc:{0000=eq,0001=ne,0010=hs,...}
> r0111010 010xxxxx cccc10nn nnn0yyyy     ccmn Rn, #x, #y     
>             r:{0=W,1=X}, cccc:{0000=eq,0001=ne,0010=hs,...}
> r1011010 000mmmmm 000000nn nnnddddd     sbc Rd, Rn, Rm                  r:{0=W,1=X}
> r1011010 100mmmmm cccc00nn nnnddddd     csinv Rd, Rn, Rm, cond 
>          r:{0=W,1=X}, cccc:{0000=eq,0001=ne,0010=hs,...}
> r1011010 100mmmmm cccc01nn nnnddddd     csneg Rd, Rn, Rm, cond 
>          r:{0=W,1=X}, cccc:{0000=eq,0001=ne,0010=hs,...}
> r1111010 000mmmmm 000000nn nnnddddd     sbcs Rd, Rn, Rm                 r:{0=W,1=X}
> r1111010 010mmmmm cccc00nn nnn0yyyy     ccmp Rn, Rm, #y     
>             r:{0=W,1=X}, cccc:{0000=eq,0001=ne,0010=hs,...}
> r1111010 010xxxxx cccc10nn nnn0yyyy     ccmp Rn, #x, #y     
>             r:{0=W,1=X}, cccc:{0000=eq,0001=ne,0010=hs,...}
> 


     * */
    /*
     * 
xx01 1110 xx1x xxx0 1011 10nn nnnd dddd  -  abs Sd Sn
xx00 1110 xx1x xxx0 1011 10nn nnnd dddd  -  abs Vd Vn
x001 1010 000m mmmm xxxx 00nn nnnd dddd  -  adc Rd Rn Rm
x011 1010 000m mmmm xxxx 00nn nnnd dddd  -  adcs Rd Rn Rm
x100 1110 xx1m mmmm 0100 00nn nnnd dddd  -  addhn2 Vd Vn Vm
x000 1110 xx1m mmmm 0100 00nn nnnd dddd  -  addhn Vd Vn Vm
xxx1 1110 xx11 xxx1 1011 10nn nnnd dddd  -  addp Sd Vn
xxx0 1110 xx1m mmmm 1011 11nn nnnd dddd  -  addp Vd Vn Vm
x000 1011 xx0x xxxx xxxx xxnn nnnd dddd  -  add Rd Rn Rm_SFT
x00x 0001 SSii iiii iiii iinn nnnd dddd  -  add Rd_SP Rn_SP AIMM
x000 1011 0x1x xxxx xxxx xxnn nnnd dddd  -  add Rd_SP Rn_SP Rm_EXT
x101 1110 xx1m mmmm x000 01nn nnnd dddd  -  add Sd Sn Sm
x010 1011 xx0x xxxx xxxx xxnn nnnd dddd  -  adds Rd Rn Rm_SFT
x01x 0001 SSii iiii iiii iinn nnnd dddd  -  adds Rd Rn_SP AIMM
x010 1011 0x1x xxxx xxxx xxnn nnnd dddd  -  adds Rd Rn_SP Rm_EXT
xx00 1110 xx1m mmmm 1000 01nn nnnd dddd  -  add Vd Vn Vm
xxx0 1110 xx11 xxx1 1011 10nn nnnd dddd  -  addv Fd Vn
1iix 0000 iiii iiii iiii iiii iiid dddd  -  adrp Rd ADDR_ADRP
0iix 0000 iiii iiii iiii iiii iiid dddd  -  adr Rd ADDR_PCREL21
xxx0 1110 xx1x 1xxx 0101 10nn nnnd dddd  -  aesd Vd Vn
xxx0 1110 xx1x 1xx0 0100 10nn nnnd dddd  -  aese Vd Vn
xxx0 1110 xx1x 1xx0 0111 10nn nnnd dddd  -  aesimc Vd Vn
xxx0 1110 xx1x 1xx0 0110 10nn nnnd dddd  -  aesmc Vd Vn
x000 1010 xx0x xxxx xxxx xxnn nnnd dddd  -  and Rd Rn Rm_SFT
x00x 0010 0Nii iiii iiii iinn nnnd dddd  -  and Rd_SP Rn LIMM
x11x 0010 0Nii iiii iiii iinn nnnd dddd  -  ands Rd Rn LIMM
x110 1010 xx0x xxxx xxxx xxnn nnnd dddd  -  ands Rd Rn Rm_SFT
xx00 1110 001m mmmm 0001 11nn nnnd dddd  -  and Vd Vn Vm
xxx1 1010 1x0m mmmm xx1x 10nn nnnd dddd  -  asrv Rd Rn Rm
000x 01ii iiii iiii iiii iiii iiii iiii  -  b ADDR_PCREL26 
010x 0100 iiii iiii iiii iiii iiix xxxx  -  b.c ADDR_PCREL19 
xx1x 0011 0xii iiii iiii iinn nnnd dddd  -  bfm Rd Rn IMMR IMMS   
x00x 1010 xx1x xxxx xxxx xxnn nnnd dddd  -  bic Rd Rn Rm_SFT
x11x 1010 xx1x xxxx xxxx xxnn nnnd dddd  -  bics Rd Rn Rm_SFT
xx10 1111 xxxx xxxx 0xx1 x1xx xxxd dddd  -  bic Vd SIMD_IMM_SFT
xx10 1111 xxxx xxxx 10x1 01xx xxxd dddd  -  bic Vd SIMD_IMM_SFT
xx00 1110 011m mmmm 0001 11nn nnnd dddd  -  bic Vd Vn Vm
xx10 1110 111m mmmm 0001 11nn nnnd dddd  -  bif Vd Vn Vm
xx10 1110 101m mmmm 0001 11nn nnnd dddd  -  bit Vd Vn Vm
100x 01ii iiii iiii iiii iiii iiii iiii  -  bl ADDR_PCREL26 
x10x 0110 0x1x xxxx xxxx xxnn nnnx xxxx  -  blr Rn 
110x 0100 xx1i iiii iiii iiii iiix xx00  -  brk EXCEPTION 
x10x 0110 000x xxxx xxxx xxnn nnnx xxxx  -  br Rn 
xx10 1110 011m mmmm 0001 11nn nnnd dddd  -  bsl Vd Vn Vm
xx1x 0101 iiii iiii iiii iiii iiit tttt  -  cbnz Rt ADDR_PCREL19
xx1x 0100 iiii iiii iiii iiii iiit tttt  -  cbz Rt ADDR_PCREL19
x0x1 1010 0x0i iiii xxxx 10nn nnnx cccc  -  ccmn Rn CCMP_IMM NZCV COND   
x0x1 1010 010m mmmm xxxx 00nn nnnx cccc  -  ccmn Rn Rm NZCV COND   
x1x1 1010 0x0i iiii xxxx 10nn nnnx cccc  -  ccmp Rn CCMP_IMM NZCV COND   
x1x1 1010 010m mmmm xxxx 00nn nnnx cccc  -  ccmp Rn Rm NZCV COND   
x10x 01x1 xxx0 0xxx xxx1 mmmm 010x xxxx  -  clrex UIMM4 
xxx1 1010 x10x xxxx xxx1 01nn nnnd dddd  -  cls Rd Rn
xx00 1110 xx1x 0xx0 0100 10nn nnnd dddd  -  cls Vd Vn
xxx1 1010 110x xxxx xxx1 00nn nnnd dddd  -  clz Rd Rn
xx10 1110 xx1x 0xx0 0100 10nn nnnd dddd  -  clz Vd Vn
xx01 1110 xx1x xxx0 1001 10nn nnnd dddd  -  cmeq Sd Sn IMM0
xx11 1110 xx1m mmmm 1000 11nn nnnd dddd  -  cmeq Sd Sn Sm
xx00 1110 xx1x xxx0 1001 10nn nnnd dddd  -  cmeq Vd Vn IMM0
xx10 1110 xx1m mmmm 1000 11nn nnnd dddd  -  cmeq Vd Vn Vm
xx11 1110 xx1x xxxx 1000 10nn nnnd dddd  -  cmge Sd Sn IMM0
x101 1110 xx1m mmmm x011 11nn nnnd dddd  -  cmge Sd Sn Sm
xx10 1110 xx1x xxx0 1000 10nn nnnd dddd  -  cmge Vd Vn IMM0
xx00 1110 xx1m mmmm 0011 11nn nnnd dddd  -  cmge Vd Vn Vm
x101 1110 xx1x xxxx 1000 10nn nnnd dddd  -  cmgt Sd Sn IMM0
x101 1110 xx1m mmmm 0x11 01nn nnnd dddd  -  cmgt Sd Sn Sm
xx00 1110 xx1x xxx0 1000 10nn nnnd dddd  -  cmgt Vd Vn IMM0
xx00 1110 xx1m mmmm 0011 01nn nnnd dddd  -  cmgt Vd Vn Vm
xx11 1110 xx1m mmmm 0x11 01nn nnnd dddd  -  cmhi Sd Sn Sm
xx10 1110 xx1m mmmm 0011 01nn nnnd dddd  -  cmhi Vd Vn Vm
xx11 1110 xx1m mmmm xx11 11nn nnnd dddd  -  cmhs Sd Sn Sm
xx10 1110 xx1m mmmm 0011 11nn nnnd dddd  -  cmhs Vd Vn Vm
xx11 1110 xx1x xxx0 1001 10nn nnnd dddd  -  cmle Sd Sn IMM0
xx10 1110 xx1x xxx0 1001 10nn nnnd dddd  -  cmle Vd Vn IMM0
xxx1 1110 xx10 xxx0 1010 10nn nnnd dddd  -  cmlt Sd Sn IMM0
xxx0 1110 xx10 xxx0 1010 10nn nnnd dddd  -  cmlt Vd Vn IMM0
x101 1110 xx1m mmmm 1000 11nn nnnd dddd  -  cmtst Sd Sn Sm
xx00 1110 xx1m mmmm 1000 11nn nnnd dddd  -  cmtst Vd Vn Vm
xx00 1110 xx1x 0xxx 0101 10nn nnnd dddd  -  cnt Vd Vn
x0x1 1010 100m mmmm xxxx 00nn nnnd dddd  -  csel Rd Rn Rm COND   
x0x1 1010 x00m mmmm xxxx 01nn nnnd dddd  -  csinc Rd Rn Rm COND   
x1x1 1010 100m mmmm xxxx 00nn nnnd dddd  -  csinv Rd Rn Rm COND   
x1x1 1010 x00m mmmm xxxx 01nn nnnd dddd  -  csneg Rd Rn Rm COND   
110x 0100 xx1i iiii iiii iiii iiix xx01  -  dcps1 EXCEPTION 
110x 0100 xx1i iiii iiii iiii iiix xx10  -  dcps2 EXCEPTION 
110x 0100 xx1i iiii iiii iiii iiix xx11  -  dcps3 EXCEPTION 
x10x 01x1 xxx0 0xxx xxx1 xxxx xx1x xxxx  -  dmb BARRIER 
x10x 0110 1x1x xxxx xxxx xxxx xxxx xxxx  -  drps  
x10x 01x1 xxx0 0xxx xxx1 xxxx x00x xxxx  -  dsb BARRIER 
x1x1 1110 xx0x xxxx xxxx x1nn nnnd dddd  -  dup Sd En
xx00 1110 xx0x xxxx xxxx 01nn nnnd dddd  -  dup Vd En
xx00 1110 xx0x xxxx xx00 11nn nnnd dddd  -  dup Vd Rn
x10x 1010 xx1x xxxx xxxx xxnn nnnd dddd  -  eon Rd Rn Rm_SFT
x100 1010 xx0x xxxx xxxx xxnn nnnd dddd  -  eor Rd Rn Rm_SFT
x10x 0010 0Nii iiii iiii iinn nnnd dddd  -  eor Rd_SP Rn LIMM
xx10 1110 001m mmmm 0001 11nn nnnd dddd  -  eor Vd Vn Vm
x10x 0110 100x xxxx xxxx xxxx xxxx xxxx  -  eret  
xxxx 0011 1xxm mmmm iiii iinn nnnd dddd  -  extr Rd Rn Rm IMMS   
xx10 1110 xx0m mmmm xiii i0nn nnnd dddd  -  ext Vd Vn Vm IDX   
xx11 1110 xx1m mmmm 1x01 01nn nnnd dddd  -  fabd Sd Sn Sm
xx10 1110 1x1m mmmm 1101 01nn nnnd dddd  -  fabd Vd Vn Vm
xxx1 1110 xx1x x000 1100 00nn nnnd dddd  -  fabs Fd Fn
xx0x 1110 xx10 xxx0 1111 10nn nnnd dddd  -  fabs Vd Vn
xx11 1110 0x1m mmmm x110 11nn nnnd dddd  -  facge Sd Sn Sm
xxx0 1110 0x1m mmmm 1110 11nn nnnd dddd  -  facge Vd Vn Vm
xx11 1110 1x1m mmmm x110 11nn nnnd dddd  -  facgt Sd Sn Sm
xxx0 1110 1x1m mmmm 1110 11nn nnnd dddd  -  facgt Vd Vn Vm
x001 1110 xx1m mmmm 0010 10nn nnnd dddd  -  fadd Fd Fn Fm
xxxx 1110 xx11 xxx0 1101 10nn nnnd dddd  -  faddp Sd Vn
xx10 1110 0x1m mmmm 1101 01nn nnnd dddd  -  faddp Vd Vn Vm
xx00 1110 0x1m mmmm 1101 01nn nnnd dddd  -  fadd Vd Vn Vm
x001 1110 xx1m mmmm xxxx 01nn nnn1 cccc  -  fccmpe Fn Fm NZCV COND   
x001 1110 xx1m mmmm xxxx 01nn nnn0 cccc  -  fccmp Fn Fm NZCV COND   
xx01 1110 xx10 xxx0 1101 10nn nnnd dddd  -  fcmeq Sd Sn IMM0
x101 1110 xx1m mmmm xx10 01nn nnnd dddd  -  fcmeq Sd Sn Sm
xx00 1110 xx10 xxx0 1101 10nn nnnd dddd  -  fcmeq Vd Vn IMM0
xx00 1110 0x1m mmmm 1110 01nn nnnd dddd  -  fcmeq Vd Vn Vm
xx11 1110 xx10 xxx0 1100 10nn nnnd dddd  -  fcmge Sd Sn IMM0
xx11 1110 0x1m mmmm xx10 01nn nnnd dddd  -  fcmge Sd Sn Sm
xx10 1110 xx10 xxx0 1100 10nn nnnd dddd  -  fcmge Vd Vn IMM0
xx10 1110 0x1m mmmm 1110 01nn nnnd dddd  -  fcmge Vd Vn Vm
xx01 1110 xx10 xxx0 1100 10nn nnnd dddd  -  fcmgt Sd Sn IMM0
xx11 1110 1x1m mmmm xx10 01nn nnnd dddd  -  fcmgt Sd Sn Sm
xx00 1110 xx10 xxx0 1100 10nn nnnd dddd  -  fcmgt Vd Vn IMM0
xxx0 1110 1x1m mmmm 1110 01nn nnnd dddd  -  fcmgt Vd Vn Vm
xx11 1110 xx10 xxx0 1101 10nn nnnd dddd  -  fcmle Sd Sn IMM0
xx10 1110 xx10 xxx0 1101 10nn nnnd dddd  -  fcmle Vd Vn IMM0
xxx1 1110 xx1x xxxx 1110 10nn nnnd dddd  -  fcmlt Sd Sn IMM0
xxx0 1110 xx1x xxxx 1110 10nn nnnd dddd  -  fcmlt Vd Vn IMM0
xxx1 1110 xx1m mmmm 0010 00nn nnn1 0xxx  -  fcmpe Fn Fm
xxx1 1110 xx1x xxxx 0010 00nn nnn1 1xxx  -  fcmpe Fn FPIMM0
xxx1 1110 xx1m mmmm 0010 00nn nnn0 0xxx  -  fcmp Fn Fm
xxx1 1110 xx1x xxxx 0010 00nn nnn0 1xxx  -  fcmp Fn FPIMM0
x001 1110 xx1m mmmm xxxx 11nn nnnd dddd  -  fcsel Fd Fn Fm COND   
xxx1 1110 xx1x x100 0000 00nn nnnd dddd  -  fcvtas Rd Fn
xx01 1110 0x1x xxx1 1100 10nn nnnd dddd  -  fcvtas Sd Sn
xx00 1110 0x1x xxx1 1100 10nn nnnd dddd  -  fcvtas Vd Vn
xxx1 1110 xx1x x101 0000 00nn nnnd dddd  -  fcvtau Rd Fn
xx11 1110 0x1x xxx1 1100 10nn nnnd dddd  -  fcvtau Sd Sn
xx10 1110 0x1x xxx1 1100 10nn nnnd dddd  -  fcvtau Vd Vn
xxx1 1110 xx1x x01x x100 00nn nnnd dddd  -  fcvt Fd Fn
x1x0 1110 xx1x xxx1 0111 10nn nnnd dddd  -  fcvtl2 Vd Vn
x0x0 1110 xx1x xxx1 0111 10nn nnnd dddd  -  fcvtl Vd Vn
xxx1 1110 xx11 0000 0000 00nn nnnd dddd  -  fcvtms Rd Fn
xx01 1110 0x10 xxx1 1011 10nn nnnd dddd  -  fcvtms Sd Sn
xx00 1110 0x10 xxx1 1011 10nn nnnd dddd  -  fcvtms Vd Vn
xxx1 1110 xx11 0001 0000 00nn nnnd dddd  -  fcvtmu Rd Fn
xx11 1110 0x10 xxx1 1011 10nn nnnd dddd  -  fcvtmu Sd Sn
xx10 1110 0x10 xxx1 1011 10nn nnnd dddd  -  fcvtmu Vd Vn
x100 1110 xx1x xxx1 0110 10nn nnnd dddd  -  fcvtn2 Vd Vn
xxx1 1110 xx10 0000 0000 00nn nnnd dddd  -  fcvtns Rd Fn
xx01 1110 0x10 xxx1 1010 10nn nnnd dddd  -  fcvtns Sd Sn
xx00 1110 0x10 xxx1 1010 10nn nnnd dddd  -  fcvtns Vd Vn
xxx1 1110 xx10 0001 0000 00nn nnnd dddd  -  fcvtnu Rd Fn
xx11 1110 0x10 xxx1 1010 10nn nnnd dddd  -  fcvtnu Sd Sn
xx10 1110 0x10 xxx1 1010 10nn nnnd dddd  -  fcvtnu Vd Vn
x000 1110 xx1x xxx1 0110 10nn nnnd dddd  -  fcvtn Vd Vn
xxx1 1110 xx10 1000 0000 00nn nnnd dddd  -  fcvtps Rd Fn
xx01 1110 1x10 xxx1 1010 10nn nnnd dddd  -  fcvtps Sd Sn
xx00 1110 1x10 xxx1 1010 10nn nnnd dddd  -  fcvtps Vd Vn
xxx1 1110 xx10 1001 0000 00nn nnnd dddd  -  fcvtpu Rd Fn
xx11 1110 1x10 xxx1 1010 10nn nnnd dddd  -  fcvtpu Sd Sn
xx10 1110 1x10 xxx1 1010 10nn nnnd dddd  -  fcvtpu Vd Vn
x110 1110 xx1x xxx1 0110 10nn nnnd dddd  -  fcvtxn2 Vd Vn
xx11 1110 xx1x xxxx 0110 10nn nnnd dddd  -  fcvtxn Sd Sn
x010 1110 xx1x xxx1 0110 10nn nnnd dddd  -  fcvtxn Vd Vn
xxx1 1110 xx11 1000 0000 00nn nnnd dddd  -  fcvtzs Rd Fn
x0x1 1110 xx0x xx00 SSSS SSnn nnnd dddd  -  fcvtzs Rd Fn FBITS
xx01 1110 1x10 xxx1 1011 10nn nnnd dddd  -  fcvtzs Sd Sn
x101 1111 xxxx xxxx 1x1x 11nn nnnd dddd  -  fcvtzs Sd Sn IMM_VLSR
xx00 1110 1x10 xxx1 1011 10nn nnnd dddd  -  fcvtzs Vd Vn
xx00 1111 xxxx xxxx 1x11 11nn nnnd dddd  -  fcvtzs Vd Vn IMM_VLSR
xxx1 1110 xx11 1001 0000 00nn nnnd dddd  -  fcvtzu Rd Fn
x0x1 1110 xx0x xx01 SSSS SSnn nnnd dddd  -  fcvtzu Rd Fn FBITS
xx11 1110 1x10 xxx1 1011 10nn nnnd dddd  -  fcvtzu Sd Sn
xx11 1111 xxxx xxxx 1x11 11nn nnnd dddd  -  fcvtzu Sd Sn IMM_VLSR
xx10 1110 1x10 xxx1 1011 10nn nnnd dddd  -  fcvtzu Vd Vn
xx10 1111 xxxx xxxx 1x11 11nn nnnd dddd  -  fcvtzu Vd Vn IMM_VLSR
x0x1 1110 xx1m mmmm 0001 10nn nnnd dddd  -  fdiv Fd Fn Fm
xx10 1110 0x1m mmmm 1111 11nn nnnd dddd  -  fdiv Vd Vn Vm
x001 1111 xx0m mmmm 0aaa aann nnnd dddd  -  fmadd Fd Fn Fm Fa   
x001 1110 xx1m mmmm 0100 10nn nnnd dddd  -  fmax Fd Fn Fm
xx01 1110 xx1m mmmm 0110 10nn nnnd dddd  -  fmaxnm Fd Fn Fm
xxx1 1110 0x11 xxx0 1100 10nn nnnd dddd  -  fmaxnmp Sd Vn
xx10 1110 0x1m mmmm 1100 01nn nnnd dddd  -  fmaxnmp Vd Vn Vm
xx00 1110 0x1m mmmm 1100 01nn nnnd dddd  -  fmaxnm Vd Vn Vm
xxx0 1110 0x11 xxx0 1100 10nn nnnd dddd  -  fmaxnmv Fd Vn
xxx1 1110 0x11 xxx0 1111 10nn nnnd dddd  -  fmaxp Sd Vn
xx10 1110 0x1m mmmm 1111 01nn nnnd dddd  -  fmaxp Vd Vn Vm
xx00 1110 0x1m mmmm 1111 01nn nnnd dddd  -  fmax Vd Vn Vm
xxx0 1110 0x11 xxx0 1111 10nn nnnd dddd  -  fmaxv Fd Vn
xxx1 1110 xx1m mmmm 0101 10nn nnnd dddd  -  fmin Fd Fn Fm
x001 1110 xx1m mmmm 0111 10nn nnnd dddd  -  fminnm Fd Fn Fm
xxx1 1110 1x11 xxx0 1100 10nn nnnd dddd  -  fminnmp Sd Vn
xx10 1110 1x1m mmmm 1100 01nn nnnd dddd  -  fminnmp Vd Vn Vm
xx00 1110 1x1m mmmm 1100 01nn nnnd dddd  -  fminnm Vd Vn Vm
xxx0 1110 1x11 xxx0 1100 10nn nnnd dddd  -  fminnmv Fd Vn
xxx1 1110 1x11 xxx0 1111 10nn nnnd dddd  -  fminp Sd Vn
xx10 1110 1x1m mmmm 1111 01nn nnnd dddd  -  fminp Vd Vn Vm
xx00 1110 1x1m mmmm 1111 01nn nnnd dddd  -  fmin Vd Vn Vm
xxx0 1110 1x11 xxx0 1111 10nn nnnd dddd  -  fminv Fd Vn
x101 1111 xxxm mmmm 000x x0nn nnnd dddd  -  fmla Sd Sn Em
xxx0 1111 xxxm mmmm 0001 x0nn nnnd dddd  -  fmla Vd Vn Em
xxx0 1110 0x1m mmmm 1100 11nn nnnd dddd  -  fmla Vd Vn Vm
x101 1111 xxxm mmmm 010x x0nn nnnd dddd  -  fmls Sd Sn Em
xxx0 1111 xxxm mmmm 0101 x0nn nnnd dddd  -  fmls Vd Vn Em
xxx0 1110 1x1m mmmm 1100 11nn nnnd dddd  -  fmls Vd Vn Vm
xxx1 1110 xx1x x000 0100 00nn nnnd dddd  -  fmov Fd Fn
x0x1 1110 xx1i iiii iii1 00xx xxxd dddd  -  fmov Fd FPIMM
xxx1 1110 xx1x 0111 0000 00nn nnnd dddd  -  fmov Fd Rn
xxx1 1110 xx1x 0110 0000 00nn nnnd dddd  -  fmov Rd Fn
xxx1 1110 xx1x 1110 0000 00nn nnnd dddd  -  fmov Rd VnD1
xxx1 1110 xx1x 1111 0000 00nn nnnd dddd  -  fmov VdD1 Rn
xx00 1111 xxxx xxxx 1111 01xx xxxd dddd  -  fmov Vd SIMD_FPIMM
xx10 1111 xxxx xxxx 1111 01xx xxxd dddd  -  fmov Vd SIMD_FPIMM
x001 1111 xx0m mmmm 1aaa aann nnnd dddd  -  fmsub Fd Fn Fm Fa   
x0x1 1110 xx1m mmmm 0000 10nn nnnd dddd  -  fmul Fd Fn Fm
x101 1111 xxxm mmmm 1001 x0nn nnnd dddd  -  fmul Sd Sn Em
xx00 1111 xxxm mmmm 1001 x0nn nnnd dddd  -  fmul Vd Vn Em
xx10 1110 xx1m mmmm 1101 11nn nnnd dddd  -  fmul Vd Vn Vm
xx11 1111 xxxm mmmm 1xxx x0nn nnnd dddd  -  fmulx Sd Sn Em
x101 1110 xx1m mmmm 1x01 11nn nnnd dddd  -  fmulx Sd Sn Sm
xx10 1111 xxxm mmmm 1001 x0nn nnnd dddd  -  fmulx Vd Vn Em
xx00 1110 xx1m mmmm 1101 11nn nnnd dddd  -  fmulx Vd Vn Vm
xxx1 1110 xx1x x001 0100 00nn nnnd dddd  -  fneg Fd Fn
xx1x 1110 xx10 xxx0 1111 10nn nnnd dddd  -  fneg Vd Vn
x001 1111 xx1m mmmm 0aaa aann nnnd dddd  -  fnmadd Fd Fn Fm Fa   
x001 1111 xx1m mmmm 1aaa aann nnnd dddd  -  fnmsub Fd Fn Fm Fa   
x001 1110 xx1m mmmm 1000 10nn nnnd dddd  -  fnmul Fd Fn Fm
xx01 1110 1x1x xxx1 1101 10nn nnnd dddd  -  frecpe Sd Sn
xx00 1110 1x1x xxx1 1101 10nn nnnd dddd  -  frecpe Vd Vn
x101 1110 0x1m mmmm x111 11nn nnnd dddd  -  frecps Sd Sn Sm
xx00 1110 0x1m mmmm 1111 11nn nnnd dddd  -  frecps Vd Vn Vm
xxx1 1110 xx1x xxx1 1111 10nn nnnd dddd  -  frecpx Sd Sn
xxx1 1110 xx1x x110 0100 00nn nnnd dddd  -  frinta Fd Fn
xx10 1110 0x1x xxx1 1000 10nn nnnd dddd  -  frinta Vd Vn
xxx1 1110 xx1x x11x 1100 00nn nnnd dddd  -  frinti Fd Fn
xx1x 1110 1x1x xxx1 1001 10nn nnnd dddd  -  frinti Vd Vn
xxx1 1110 xx1x x101 0100 00nn nnnd dddd  -  frintm Fd Fn
xx0x 1110 0x1x xxx1 1001 10nn nnnd dddd  -  frintm Vd Vn
xxx1 1110 xx1x x100 0100 00nn nnnd dddd  -  frintn Fd Fn
xx00 1110 0x1x xxx1 1000 10nn nnnd dddd  -  frintn Vd Vn
xxx1 1110 xx1x x100 1100 00nn nnnd dddd  -  frintp Fd Fn
xxx0 1110 1x1x xxx1 1000 10nn nnnd dddd  -  frintp Vd Vn
xxx1 1110 xx1x x111 0100 00nn nnnd dddd  -  frintx Fd Fn
xx1x 1110 0x1x xxx1 1001 10nn nnnd dddd  -  frintx Vd Vn
xxx1 1110 xx1x x101 1100 00nn nnnd dddd  -  frintz Fd Fn
xx0x 1110 1x1x xxx1 1001 10nn nnnd dddd  -  frintz Vd Vn
xx11 1110 1x1x xxx1 1101 10nn nnnd dddd  -  frsqrte Sd Sn
xx10 1110 1x1x xxx1 1101 10nn nnnd dddd  -  frsqrte Vd Vn
x101 1110 1x1m mmmm x111 11nn nnnd dddd  -  frsqrts Sd Sn Sm
xxx0 1110 1x1m mmmm 1111 11nn nnnd dddd  -  frsqrts Vd Vn Vm
xxx1 1110 xx1x x001 1100 00nn nnnd dddd  -  fsqrt Fd Fn
xxx0 1110 xx1x xxx1 1111 10nn nnnd dddd  -  fsqrt Vd Vn
x001 1110 xx1m mmmm 0011 10nn nnnd dddd  -  fsub Fd Fn Fm
xx00 1110 1x1m mmmm 1101 01nn nnnd dddd  -  fsub Vd Vn Vm
x10x 01x1 xxx0 0xxx xx10 mmmm ooox xxxx  -  hint UIMM7 
110x 0100 xx0i iiii iiii iiii iiix xx00  -  hlt EXCEPTION 
110x 0100 xx0i iiii iiii iiii iiix xx10  -  hvc EXCEPTION 
xx10 1110 xx0x xxxx xxxx x1nn nnnd dddd  -  ins Ed En
xx00 1110 xx0x xxxx xx01 11nn nnnd dddd  -  ins Ed Rn
x10x 01x1 xxx0 0xxx xxx1 xxxx 110x xxxx  -  isb BARRIER_ISB 
xx00 1101 110x xxxx xx0x xxxx xxxx xxxx  -  ld1 LEt SIMD_ADDR_POST
xx00 1101 010x xxxx xx0x xxxx xxxx xxxx  -  ld1 LEt SIMD_ADDR_SIMPLE
xx00 110x 111x xxxx xx0x xxxx xxxx xxxx  -  ld2 LEt SIMD_ADDR_POST
xx00 1101 011x xxxx xx0x xxxx xxxx xxxx  -  ld2 LEt SIMD_ADDR_SIMPLE
xx00 1101 110x xxxx xx1x xxxx xxxx xxxx  -  ld3 LEt SIMD_ADDR_POST
xx00 1101 010x xxxx xx1x xxxx xxxx xxxx  -  ld3 LEt SIMD_ADDR_SIMPLE
xx00 110x 111x xxxx xx1x xxxx xxxx xxxx  -  ld4 LEt SIMD_ADDR_POST
xx00 1101 011x xxxx xx1x xxxx xxxx xxxx  -  ld4 LEt SIMD_ADDR_SIMPLE
xx00 1100 110x xxxx xxxx xxxx xxxx xxxx  -  ld4 LVt SIMD_ADDR_POST
xx00 1100 01xx xxxx xxxx xxxx xxxx xxxx  -  ld4 LVt SIMD_ADDR_SIMPLE
0000 100x 11xx xxxx xxxx xxxx xxxt tttt  -  ldarb Rt ADDR_SIMPLE
0100 100x 11xx xxxx xxxx xxxx xxxt tttt  -  ldarh Rt ADDR_SIMPLE
1x00 100x 11xx xxxx xxxx xxxx xxxt tttt  -  ldar Rt ADDR_SIMPLE
xx00 100x 011x xxxx 1ttt ttxx xxxt tttt  -  ldaxp Rt Rt2 ADDR_SIMPLE
0000 100x 010x xxxx 1xxx xxxx xxxt tttt  -  ldaxrb Rt ADDR_SIMPLE
0100 100x 010x xxxx 1xxx xxxx xxxt tttt  -  ldaxrh Rt ADDR_SIMPLE
1x00 100x 010x xxxx 1xxx xxxx xxxt tttt  -  ldaxr Rt ADDR_SIMPLE
xx10 110I 01ii iiii ittt ttxx xxxt tttt  -  ldnp Ft Ft2 ADDR_SIMM7
x010 100I 01ii iiii ittt ttxx xxxt tttt  -  ldnp Rt Rt2 ADDR_SIMM7
xx10 110I 01ii iiii ittt ttxx xxxt tttt  -  ldp Ft Ft2 ADDR_SIMM7
xx10 110I 11ii iiii ittt ttxx xxxt tttt  -  ldp Ft Ft2 ADDR_SIMM7
x010 100I 11ii iiii ittt ttxx xxxt tttt  -  ldp Rt Rt2 ADDR_SIMM7
x110 100I 01ii iiii ittt ttxx xxxt tttt  -  ldpsw Rt Rt2 ADDR_SIMM7
x110 100I 11ii iiii ittt ttxx xxxt tttt  -  ldpsw Rt Rt2 ADDR_SIMM7
0011 1000 011x xxxx xxxx 10xx xxxt tttt  -  ldrb Rt ADDR_REGOFF
0011 1000 01xi iiii iiii I1xx xxxt tttt  -  ldrb Rt ADDR_SIMM9
00x1 1001 01ii iiii iiii iinn nnnt tttt  -  ldrb Rt ADDR_UIMM12
xx01 1100 iiii iiii iiii iiii iiit tttt  -  ldr Ft ADDR_PCREL19
xx11 1100 x1xx xxxx xxxx 10xx xxxt tttt  -  ldr Ft ADDR_REGOFF
xx11 1100 x1xi iiii iiii I1xx xxxt tttt  -  ldr Ft ADDR_SIMM9
xxx1 1101 x1ii iiii iiii iinn nnnt tttt  -  ldr Ft ADDR_UIMM12
0111 1000 011x xxxx xxxx 10xx xxxt tttt  -  ldrh Rt ADDR_REGOFF
0111 1000 01xi iiii iiii I1xx xxxt tttt  -  ldrh Rt ADDR_SIMM9
01x1 1001 01ii iiii iiii iinn nnnt tttt  -  ldrh Rt ADDR_UIMM12
0x01 1000 iiii iiii iiii iiii iiit tttt  -  ldr Rt ADDR_PCREL19
1x11 1000 011x xxxx xxxx 10xx xxxt tttt  -  ldr Rt ADDR_REGOFF
1x11 1000 01xi iiii iiii I1xx xxxt tttt  -  ldr Rt ADDR_SIMM9
1xx1 1001 01ii iiii iiii iinn nnnt tttt  -  ldr Rt ADDR_UIMM12
0011 1000 1x1x xxxx xxxx 10xx xxxt tttt  -  ldrsb Rt ADDR_REGOFF
0011 1000 1xxi iiii iiii I1xx xxxt tttt  -  ldrsb Rt ADDR_SIMM9
00x1 1001 1xii iiii iiii iinn nnnt tttt  -  ldrsb Rt ADDR_UIMM12
0111 1000 1x1x xxxx xxxx 10xx xxxt tttt  -  ldrsh Rt ADDR_REGOFF
x111 1000 1xxi iiii iiii I1xx xxxt tttt  -  ldrsh Rt ADDR_SIMM9
01x1 1001 1xii iiii iiii iinn nnnt tttt  -  ldrsh Rt ADDR_UIMM12
1001 1000 iiii iiii iiii iiii iiit tttt  -  ldrsw Rt ADDR_PCREL19
1011 1000 1x1x xxxx xxxx 10xx xxxt tttt  -  ldrsw Rt ADDR_REGOFF
1011 1000 1xxi iiii iiii I1xx xxxt tttt  -  ldrsw Rt ADDR_SIMM9
10x1 1001 1xii iiii iiii iinn nnnt tttt  -  ldrsw Rt ADDR_UIMM12
0011 1000 010i iiii iiii I0xx xxxt tttt  -  ldtrb Rt ADDR_SIMM9
0111 1000 010i iiii iiii I0xx xxxt tttt  -  ldtrh Rt ADDR_SIMM9
1x11 1000 010i iiii iiii I0xx xxxt tttt  -  ldtr Rt ADDR_SIMM9
0011 1000 1x0i iiii iiii I0xx xxxt tttt  -  ldtrsb Rt ADDR_SIMM9
x111 1000 1x0i iiii iiii I0xx xxxt tttt  -  ldtrsh Rt ADDR_SIMM9
1011 1000 1x0i iiii iiii I0xx xxxt tttt  -  ldtrsw Rt ADDR_SIMM9
0011 1000 01xi iiii iiii I0xx xxxt tttt  -  ldurb Rt ADDR_SIMM9
xx11 1100 x1xi iiii iiii I0xx xxxt tttt  -  ldur Ft ADDR_SIMM9
0111 1000 01xi iiii iiii I0xx xxxt tttt  -  ldurh Rt ADDR_SIMM9
1x11 1000 01xi iiii iiii I0xx xxxt tttt  -  ldur Rt ADDR_SIMM9
0011 1000 1xxi iiii iiii I0xx xxxt tttt  -  ldursb Rt ADDR_SIMM9
0111 1000 1xxi iiii iiii I0xx xxxt tttt  -  ldursh Rt ADDR_SIMM9
1011 1000 1xxi iiii iiii I0xx xxxt tttt  -  ldursw Rt ADDR_SIMM9
xx00 100x 011x xxxx 0ttt ttxx xxxt tttt  -  ldxp Rt Rt2 ADDR_SIMPLE
0000 100x 010x xxxx 0xxx xxxx xxxt tttt  -  ldxrb Rt ADDR_SIMPLE
0100 100x 010x xxxx 0xxx xxxx xxxt tttt  -  ldxrh Rt ADDR_SIMPLE
1x00 100x 010x xxxx 0xxx xxxx xxxt tttt  -  ldxr Rt ADDR_SIMPLE
xxx1 1010 110m mmmm xx10 00nn nnnd dddd  -  lslv Rd Rn Rm
xxx1 1010 x10m mmmm xx10 01nn nnnd dddd  -  lsrv Rd Rn Rm
xxx1 1011 x00m mmmm 0aaa aann nnnd dddd  -  madd Rd Rn Rm Ra   
xxx0 1111 xxxm mmmm 0000 x0nn nnnd dddd  -  mla Vd Vn Em
xx00 1110 xx1m mmmm 1001 01nn nnnd dddd  -  mla Vd Vn Vm
xxx0 1111 xxxm mmmm 0100 x0nn nnnd dddd  -  mls Vd Vn Em
xx10 1110 xx1m mmmm 1001 01nn nnnd dddd  -  mls Vd Vn Vm
xx10 1111 xxxx xxxx 1110 01xx xxxd dddd  -  movi Sd SIMD_IMM
xx00 1111 xxxx xxxx 1110 01xx xxxd dddd  -  movi Vd SIMD_IMM
xx00 1111 xxxx xxxx 0xx0 x1xx xxxd dddd  -  movi Vd SIMD_IMM_SFT
xx00 1111 xxxx xxxx 10x0 01xx xxxd dddd  -  movi Vd SIMD_IMM_SFT
xx00 1111 xxxx xxxx 110x 01xx xxxd dddd  -  movi Vd SIMD_IMM_SFT
xx1x 0010 1xxi iiii iiii iiii iiid dddd  -  movk Rd HALF
x00x 0010 1xxi iiii iiii iiii iiid dddd  -  movn Rd HALF
x10x 0010 1xxi iiii iiii iiii iiid dddd  -  movz Rd HALF
x10x 01x1 xx11 xxxx xxxx xxxx xxxt tttt  -  mrs Rt SYSREG
x10x 01x1 xxx0 0xxx xx00 mmmm xxxx xxxx  -  msr PSTATEFIELD UIMM4
x10x 01x1 xx01 xxxx xxxx xxxx xxxt tttt  -  msr SYSREG Rt
xxx1 1011 xx0m mmmm 1aaa aann nnnd dddd  -  msub Rd Rn Rm Ra   
xxx0 1111 xxxm mmmm 1000 x0nn nnnd dddd  -  mul Vd Vn Em
xx00 1110 xx1m mmmm 1001 11nn nnnd dddd  -  mul Vd Vn Vm
xx10 1111 xxxx xxxx 0xx0 x1xx xxxd dddd  -  mvni Vd SIMD_IMM_SFT
xx10 1111 xxxx xxxx 10x0 01xx xxxd dddd  -  mvni Vd SIMD_IMM_SFT
xx10 1111 xxxx xxxx 110x 01xx xxxd dddd  -  mvni Vd SIMD_IMM_SFT
xx11 1110 xx1x xxx0 1011 10nn nnnd dddd  -  neg Sd Sn
xx10 1110 xx1x xxx0 1011 10nn nnnd dddd  -  neg Vd Vn
xx10 1110 x01x 0xxx 0101 10nn nnnd dddd  -  not Vd Vn
x01x 1010 xx1x xxxx xxxx xxnn nnnd dddd  -  orn Rd Rn Rm_SFT
xx00 1110 111m mmmm 0001 11nn nnnd dddd  -  orn Vd Vn Vm
x010 1010 xx0x xxxx xxxx xxnn nnnd dddd  -  orr Rd Rn Rm_SFT
x01x 0010 0Nii iiii iiii iinn nnnd dddd  -  orr Rd_SP Rn LIMM
xx00 1111 xxxx xxxx 0xx1 x1xx xxxd dddd  -  orr Vd SIMD_IMM_SFT
xx00 1111 xxxx xxxx 10x1 01xx xxxd dddd  -  orr Vd SIMD_IMM_SFT
xx00 1110 101m mmmm 0001 11nn nnnd dddd  -  orr Vd Vn Vm
x1xx 1110 x01m mmmm 1110 00nn nnnd dddd  -  pmull2 Vd Vn Vm
x1xx 1110 x11m mmmm 1110 00nn nnnd dddd  -  pmull2 Vd Vn Vm
x0xx 1110 x01m mmmm 1110 00nn nnnd dddd  -  pmull Vd Vn Vm
x0xx 1110 x11m mmmm 1110 00nn nnnd dddd  -  pmull Vd Vn Vm
xx10 1110 xx1m mmmm 1001 11nn nnnd dddd  -  pmul Vd Vn Vm
1101 1000 iiii iiii iiii iiii iiix xxxx  -  prfm PRFOP ADDR_PCREL19
1111 1000 1x1x xxxx xxxx 10xx xxxx xxxx  -  prfm PRFOP ADDR_REGOFF
11x1 1001 1xii iiii iiii iinn nnnx xxxx  -  prfm PRFOP ADDR_UIMM12
1111 1000 1xxi iiii iiii I0xx xxxx xxxx  -  prfum PRFOP ADDR_SIMM9
x110 1110 xx1m mmmm 0100 00nn nnnd dddd  -  raddhn2 Vd Vn Vm
x010 1110 xx1m mmmm 0100 00nn nnnd dddd  -  raddhn Vd Vn Vm
xxx1 1010 110x xxxx xx00 00nn nnnd dddd  -  rbit Rd Rn
xx10 1110 x11x 0xxx 0101 10nn nnnd dddd  -  rbit Vd Vn
x10x 0110 x10x xxxx xxxx xxnn nnnx xxxx  -  ret Rn 
xxx1 1010 x10x xxxx xx00 01nn nnnd dddd  -  rev16 Rd Rn
xxx0 1110 xx1x xxxx 0001 10nn nnnd dddd  -  rev16 Vd Vn
11x1 1010 1x0x xxxx xx0x 10nn nnnd dddd  -  rev32 Rd Rn
xx10 1110 xx1x xxxx 0000 10nn nnnd dddd  -  rev32 Vd Vn
xx00 1110 xx1x xxxx 0000 10nn nnnd dddd  -  rev64 Vd Vn
01x1 1010 1x0x xxxx xx0x 10nn nnnd dddd  -  rev Rd Rn
x1x1 1010 xx0x xxxx xx0x 11nn nnnd dddd  -  rev Rd Rn
xxx1 1010 xx0m mmmm xx1x 11nn nnnd dddd  -  rorv Rd Rn Rm
x100 1111 xxxx xxxx 1xx0 11nn nnnd dddd  -  rshrn2 Vd Vn IMM_VLSR
x000 1111 xxxx xxxx 1xx0 11nn nnnd dddd  -  rshrn Vd Vn IMM_VLSR
x11x 1110 xx1m mmmm 0110 00nn nnnd dddd  -  rsubhn2 Vd Vn Vm
x01x 1110 xx1m mmmm 0110 00nn nnnd dddd  -  rsubhn Vd Vn Vm
x100 1110 xx1m mmmm 0101 00nn nnnd dddd  -  sabal2 Vd Vn Vm
x000 1110 xx1m mmmm 0101 00nn nnnd dddd  -  sabal Vd Vn Vm
xx00 1110 xx1m mmmm 0111 11nn nnnd dddd  -  saba Vd Vn Vm
x100 1110 xx1m mmmm x111 00nn nnnd dddd  -  sabdl2 Vd Vn Vm
x000 1110 xx1m mmmm x111 00nn nnnd dddd  -  sabdl Vd Vn Vm
xx00 1110 xx1m mmmm 0111 01nn nnnd dddd  -  sabd Vd Vn Vm
xx00 1110 xx1x 0xx0 0110 10nn nnnd dddd  -  sadalp Vd Vn
x100 1110 xx1m mmmm 0000 00nn nnnd dddd  -  saddl2 Vd Vn Vm
xx00 1110 xx1x xxx0 0010 10nn nnnd dddd  -  saddlp Vd Vn
x000 1110 xx1m mmmm 0000 00nn nnnd dddd  -  saddl Vd Vn Vm
xx00 1110 xx11 xxx0 0011 10nn nnnd dddd  -  saddlv Fd Vn
x100 1110 xx1m mmmm 0001 00nn nnnd dddd  -  saddw2 Vd Vn Vm
x000 1110 xx1m mmmm 0001 00nn nnnd dddd  -  saddw Vd Vn Vm
x101 1010 000m mmmm xxxx 00nn nnnd dddd  -  sbc Rd Rn Rm
x111 1010 000m mmmm xxxx 00nn nnnd dddd  -  sbcs Rd Rn Rm
x00x 0011 0xii iiii iiii iinn nnnd dddd  -  sbfm Rd Rn IMMR IMMS   
xxx1 1110 xx1x x010 0000 00nn nnnd dddd  -  scvtf Fd Rn
x0x1 1110 xx0x xx10 SSSS SSnn nnnd dddd  -  scvtf Fd Rn FBITS
xx01 1110 0x1x xxx1 1101 10nn nnnd dddd  -  scvtf Sd Sn
x101 1111 xxxx xxxx 1xx0 01nn nnnd dddd  -  scvtf Sd Sn IMM_VLSR
xx00 1110 0x1x xxx1 1101 10nn nnnd dddd  -  scvtf Vd Vn
x0x1 1010 xx0m mmmm xx0x 11nn nnnd dddd  -  sdiv Rd Rn Rm
x1x1 1110 xx0m mmmm x000 x0nn nnnd dddd  -  sha1c Fd Fn Vm
x1x1 1110 xx1x xxxx 0000 10nn nnnd dddd  -  sha1h Fd Fn
x1x1 1110 xx0m mmmm x010 x0nn nnnd dddd  -  sha1m Fd Fn Vm
x1x1 1110 xx0m mmmm x001 x0nn nnnd dddd  -  sha1p Fd Fn Vm
x1x1 1110 xx0m mmmm xx11 x0nn nnnd dddd  -  sha1su0 Vd Vn Vm
x1x1 1110 xx1x xxxx 0001 10nn nnnd dddd  -  sha1su1 Vd Vn
x1x1 1110 xx0m mmmm x101 x0nn nnnd dddd  -  sha256h2 Fd Fn Vm
x1x1 1110 xx0m mmmm x100 x0nn nnnd dddd  -  sha256h Fd Fn Vm
x101 1110 xx1x xxxx 0010 10nn nnnd dddd  -  sha256su0 Vd Vn
x1x1 1110 xx0m mmmm x110 x0nn nnnd dddd  -  sha256su1 Vd Vn Vm
xx00 1110 xx1m mmmm 0000 01nn nnnd dddd  -  shadd Vd Vn Vm
x1x0 1110 xx1x xxx1 0011 10nn nnnd dddd  -  shll2 Vd Vn SHLL_IMM
x0x0 1110 xx1x xxx1 0011 10nn nnnd dddd  -  shll Vd Vn SHLL_IMM
x101 1111 xxxx xxxx 0101 x1nn nnnd dddd  -  shl Sd Sn IMM_VLSL
xx00 1110 xx1m mmmm 0010 01nn nnnd dddd  -  shsub Vd Vn Vm
xx11 1111 xxxx xxxx 0101 xxnn nnnd dddd  -  sli Sd Sn IMM_VLSL
xxx1 1011 0x1m mmmm 0aaa aann nnnd dddd  -  smaddl Rd Rn Rm Ra   
xx00 1110 xx1m mmmm 1010 01nn nnnd dddd  -  smaxp Vd Vn Vm
xx00 1110 xx1m mmmm 0110 01nn nnnd dddd  -  smax Vd Vn Vm
xx0x 1110 xx11 xxx0 1010 10nn nnnd dddd  -  smaxv Fd Vn
110x 0100 xx0i iiii iiii iiii iiix xx11  -  smc EXCEPTION 
xx00 1110 xx1m mmmm 1010 11nn nnnd dddd  -  sminp Vd Vn Vm
xx00 1110 xx1m mmmm 0110 11nn nnnd dddd  -  smin Vd Vn Vm
xx0x 1110 xx11 xxx1 1010 10nn nnnd dddd  -  sminv Fd Vn
x100 1111 xxxm mmmm 0010 x0nn nnnd dddd  -  smlal2 Vd Vn Em
x10x 1110 xx1m mmmm 1000 00nn nnnd dddd  -  smlal2 Vd Vn Vm
x000 1111 xxxm mmmm 0010 x0nn nnnd dddd  -  smlal Vd Vn Em
x00x 1110 xx1m mmmm 1000 00nn nnnd dddd  -  smlal Vd Vn Vm
x100 1111 xxxm mmmm 0110 x0nn nnnd dddd  -  smlsl2 Vd Vn Em
x10x 1110 xx1m mmmm 1010 00nn nnnd dddd  -  smlsl2 Vd Vn Vm
x000 1111 xxxm mmmm 0110 x0nn nnnd dddd  -  smlsl Vd Vn Em
x00x 1110 xx1m mmmm 1010 00nn nnnd dddd  -  smlsl Vd Vn Vm
xx00 1110 xx0x xxxx xx10 11nn nnnd dddd  -  smov Rd En
xxx1 1011 0x1m mmmm 1aaa aann nnnd dddd  -  smsubl Rd Rn Rm Ra   
xxx1 1011 010m mmmm 0xxx xxnn nnnd dddd  -  smulh Rd Rn Rm
x100 1111 xxxm mmmm 1x10 x0nn nnnd dddd  -  smull2 Vd Vn Em
x100 1110 xx1m mmmm 1100 00nn nnnd dddd  -  smull2 Vd Vn Vm
x000 1111 xxxm mmmm 1x10 x0nn nnnd dddd  -  smull Vd Vn Em
x000 1110 xx1m mmmm 1100 00nn nnnd dddd  -  smull Vd Vn Vm
x101 1110 xx1x xxxx 0111 10nn nnnd dddd  -  sqabs Sd Sn
xx00 1110 xx1x 0xx0 0111 10nn nnnd dddd  -  sqabs Vd Vn
x101 1110 xx1m mmmm 0000 11nn nnnd dddd  -  sqadd Sd Sn Sm
xx00 1110 xx1m mmmm 0000 11nn nnnd dddd  -  sqadd Vd Vn Vm
x1x0 1111 xxxm mmmm 0011 x0nn nnnd dddd  -  sqdmlal2 Vd Vn Em
x1x0 1110 xx1m mmmm 1001 00nn nnnd dddd  -  sqdmlal2 Vd Vn Vm
x101 1111 xxxm mmmm 001x x0nn nnnd dddd  -  sqdmlal Sd Sn Em
x1x1 1110 xx1m mmmm x001 00nn nnnd dddd  -  sqdmlal Sd Sn Sm
x0x0 1111 xxxm mmmm 0011 x0nn nnnd dddd  -  sqdmlal Vd Vn Em
x0x0 1110 xx1m mmmm 1001 00nn nnnd dddd  -  sqdmlal Vd Vn Vm
x1x0 1111 xxxm mmmm 0111 x0nn nnnd dddd  -  sqdmlsl2 Vd Vn Em
x1x0 1110 xx1m mmmm 1011 00nn nnnd dddd  -  sqdmlsl2 Vd Vn Vm
x101 1111 xxxm mmmm 011x x0nn nnnd dddd  -  sqdmlsl Sd Sn Em
x1x1 1110 xx1m mmmm xx11 00nn nnnd dddd  -  sqdmlsl Sd Sn Sm
x0x0 1111 xxxm mmmm 0111 x0nn nnnd dddd  -  sqdmlsl Vd Vn Em
x0x0 1110 xx1m mmmm 1011 00nn nnnd dddd  -  sqdmlsl Vd Vn Vm
x101 1111 xxxm mmmm 1xx0 x0nn nnnd dddd  -  sqdmulh Sd Sn Em
x101 1110 xx1m mmmm 1x11 01nn nnnd dddd  -  sqdmulh Sd Sn Sm
xxx0 1111 xxxm mmmm 1100 x0nn nnnd dddd  -  sqdmulh Vd Vn Em
xx00 1110 xx1m mmmm 1011 01nn nnnd dddd  -  sqdmulh Vd Vn Vm
x1x0 1111 xxxm mmmm 1x11 x0nn nnnd dddd  -  sqdmull2 Vd Vn Em
x1x0 1110 xx1m mmmm 1101 00nn nnnd dddd  -  sqdmull2 Vd Vn Vm
x101 1111 xxxm mmmm 1x11 x0nn nnnd dddd  -  sqdmull Sd Sn Em
x1x1 1110 xx1m mmmm x101 00nn nnnd dddd  -  sqdmull Sd Sn Sm
x0x0 1111 xxxm mmmm 1x11 x0nn nnnd dddd  -  sqdmull Vd Vn Em
x0x0 1110 xx1m mmmm 1101 00nn nnnd dddd  -  sqdmull Vd Vn Vm
xx11 1110 xx1x xxxx 0111 10nn nnnd dddd  -  sqneg Sd Sn
xx10 1110 xx1x 0xx0 0111 10nn nnnd dddd  -  sqneg Vd Vn
x101 1111 xxxm mmmm 1101 x0nn nnnd dddd  -  sqrdmulh Sd Sn Em
xx11 1110 xx1m mmmm 1x11 01nn nnnd dddd  -  sqrdmulh Sd Sn Sm
xxx0 1111 xxxm mmmm 1101 x0nn nnnd dddd  -  sqrdmulh Vd Vn Em
xx10 1110 xx1m mmmm 1011 01nn nnnd dddd  -  sqrdmulh Vd Vn Vm
x101 1110 xx1m mmmm 0x01 11nn nnnd dddd  -  sqrshl Sd Sn Sm
xx00 1110 xx1m mmmm 0101 11nn nnnd dddd  -  sqrshl Vd Vn Vm
x100 1111 xxxx xxxx 1x01 11nn nnnd dddd  -  sqrshrn2 Vd Vn IMM_VLSR
x101 1111 xxxx xxxx 1x0x 11nn nnnd dddd  -  sqrshrn Sd Sn IMM_VLSR
x000 1111 xxxx xxxx 1x01 11nn nnnd dddd  -  sqrshrn Vd Vn IMM_VLSR
x110 1111 xxxx xxxx 1xx0 11nn nnnd dddd  -  sqrshrun2 Vd Vn IMM_VLSR
xx11 1111 xxxx xxxx 1xx0 11nn nnnd dddd  -  sqrshrun Sd Sn IMM_VLSR
x010 1111 xxxx xxxx 1xx0 11nn nnnd dddd  -  sqrshrun Vd Vn IMM_VLSR
x101 1111 xxxx xxxx 0111 x1nn nnnd dddd  -  sqshl Sd Sn IMM_VLSL
x101 1110 xx1m mmmm x100 11nn nnnd dddd  -  sqshl Sd Sn Sm
xx11 1111 xxxx xxxx 0110 xxnn nnnd dddd  -  sqshlu Sd Sn IMM_VLSL
xx00 1110 xx1m mmmm 0100 11nn nnnd dddd  -  sqshl Vd Vn Vm
x101 1111 xxxx xxxx 1xx1 01nn nnnd dddd  -  sqshrn Sd Sn IMM_VLSR
xx11 1111 xxxx xxxx 1x00 01nn nnnd dddd  -  sqshrun Sd Sn IMM_VLSR
x101 1110 xx1m mmmm xx10 11nn nnnd dddd  -  sqsub Sd Sn Sm
xx00 1110 xx1m mmmm 0010 11nn nnnd dddd  -  sqsub Vd Vn Vm
x100 1110 xx1x xxx1 0100 10nn nnnd dddd  -  sqxtn2 Vd Vn
x101 1110 xx1x xxxx 0100 10nn nnnd dddd  -  sqxtn Sd Sn
x000 1110 xx1x xxx1 0100 10nn nnnd dddd  -  sqxtn Vd Vn
x110 1110 xx1x xxx1 0010 10nn nnnd dddd  -  sqxtun2 Vd Vn
xx11 1110 xx1x xxxx 0010 10nn nnnd dddd  -  sqxtun Sd Sn
x010 1110 xx1x xxx1 0010 10nn nnnd dddd  -  sqxtun Vd Vn
xx00 1110 xx1m mmmm 0001 01nn nnnd dddd  -  srhadd Vd Vn Vm
xx11 1111 xxxx xxxx 0100 xxnn nnnd dddd  -  sri Sd Sn IMM_VLSR
x101 1110 xx1m mmmm xx01 01nn nnnd dddd  -  srshl Sd Sn Sm
xx00 1110 xx1m mmmm 0101 01nn nnnd dddd  -  srshl Vd Vn Vm
x101 1111 xxxx xxxx 0x10 x1nn nnnd dddd  -  srshr Sd Sn IMM_VLSR
x101 1111 xxxx xxxx 0011 x1nn nnnd dddd  -  srsra Sd Sn IMM_VLSR
x101 1110 xx1m mmmm x100 01nn nnnd dddd  -  sshl Sd Sn Sm
xx00 1110 xx1m mmmm 0100 01nn nnnd dddd  -  sshl Vd Vn Vm
x101 1111 xxxx xxxx 0x00 x1nn nnnd dddd  -  sshr Sd Sn IMM_VLSR
x101 1111 xxxx xxxx 0001 x1nn nnnd dddd  -  ssra Sd Sn IMM_VLSR
x100 1110 xx1m mmmm 0010 00nn nnnd dddd  -  ssubl2 Vd Vn Vm
x000 1110 xx1m mmmm 0010 00nn nnnd dddd  -  ssubl Vd Vn Vm
x100 1110 xx1m mmmm 0011 00nn nnnd dddd  -  ssubw2 Vd Vn Vm
x000 1110 xx1m mmmm 0011 00nn nnnd dddd  -  ssubw Vd Vn Vm
xx00 1101 100x xxxx xx0x xxxx xxxx xxxx  -  st1 LEt SIMD_ADDR_POST
xx00 1101 000x xxxx xx0x xxxx xxxx xxxx  -  st1 LEt SIMD_ADDR_SIMPLE
xx00 110x 101x xxxx xx0x xxxx xxxx xxxx  -  st2 LEt SIMD_ADDR_POST
xx00 1101 001x xxxx xx0x xxxx xxxx xxxx  -  st2 LEt SIMD_ADDR_SIMPLE
xx00 1101 100x xxxx xx1x xxxx xxxx xxxx  -  st3 LEt SIMD_ADDR_POST
xx00 1101 000x xxxx xx1x xxxx xxxx xxxx  -  st3 LEt SIMD_ADDR_SIMPLE
xx00 110x 101x xxxx xx1x xxxx xxxx xxxx  -  st4 LEt SIMD_ADDR_POST
xx00 1101 001x xxxx xx1x xxxx xxxx xxxx  -  st4 LEt SIMD_ADDR_SIMPLE
xx00 1100 100x xxxx xxxx xxxx xxxx xxxx  -  st4 LVt SIMD_ADDR_POST
xx00 1100 00xx xxxx xxxx xxxx xxxx xxxx  -  st4 LVt SIMD_ADDR_SIMPLE
0000 100x 10xx xxxx xxxx xxxx xxxt tttt  -  stlrb Rt ADDR_SIMPLE
0100 100x 10xx xxxx xxxx xxxx xxxt tttt  -  stlrh Rt ADDR_SIMPLE
1x00 100x 10xx xxxx xxxx xxxx xxxt tttt  -  stlr Rt ADDR_SIMPLE
xx00 100x 001s ssss 1ttt ttxx xxxt tttt  -  stlxp Rs Rt Rt2 ADDR_SIMPLE   
0000 100x 000s ssss 1xxx xxxx xxxt tttt  -  stlxrb Rs Rt ADDR_SIMPLE
0100 100x 000s ssss 1xxx xxxx xxxt tttt  -  stlxrh Rs Rt ADDR_SIMPLE
1x00 100x 000s ssss 1xxx xxxx xxxt tttt  -  stlxr Rs Rt ADDR_SIMPLE
xx10 110I 00ii iiii ittt ttxx xxxt tttt  -  stnp Ft Ft2 ADDR_SIMM7
xx10 100I 00ii iiii ittt ttxx xxxt tttt  -  stnp Rt Rt2 ADDR_SIMM7
xx10 110I 00ii iiii ittt ttxx xxxt tttt  -  stp Ft Ft2 ADDR_SIMM7
xx10 110I 10ii iiii ittt ttxx xxxt tttt  -  stp Ft Ft2 ADDR_SIMM7
xx10 100I 10ii iiii ittt ttxx xxxt tttt  -  stp Rt Rt2 ADDR_SIMM7
0011 1000 001x xxxx xxxx 10xx xxxt tttt  -  strb Rt ADDR_REGOFF
0011 1000 00xi iiii iiii I1xx xxxt tttt  -  strb Rt ADDR_SIMM9
00x1 1001 00ii iiii iiii iinn nnnt tttt  -  strb Rt ADDR_UIMM12
xx11 1100 x0xx xxxx xxxx 10xx xxxt tttt  -  str Ft ADDR_REGOFF
xx11 1100 x0xi iiii iiii I1xx xxxt tttt  -  str Ft ADDR_SIMM9
xxx1 1101 x0ii iiii iiii iinn nnnt tttt  -  str Ft ADDR_UIMM12
0111 1000 001x xxxx xxxx 10xx xxxt tttt  -  strh Rt ADDR_REGOFF
0111 1000 00xi iiii iiii I1xx xxxt tttt  -  strh Rt ADDR_SIMM9
01x1 1001 00ii iiii iiii iinn nnnt tttt  -  strh Rt ADDR_UIMM12
1x11 1000 001x xxxx xxxx 10xx xxxt tttt  -  str Rt ADDR_REGOFF
1x11 1000 00xi iiii iiii I1xx xxxt tttt  -  str Rt ADDR_SIMM9
1xx1 1001 00ii iiii iiii iinn nnnt tttt  -  str Rt ADDR_UIMM12
0011 1000 000i iiii iiii I0xx xxxt tttt  -  sttrb Rt ADDR_SIMM9
0111 1000 000i iiii iiii I0xx xxxt tttt  -  sttrh Rt ADDR_SIMM9
1x11 1000 000i iiii iiii I0xx xxxt tttt  -  sttr Rt ADDR_SIMM9
0011 1000 00xi iiii iiii I0xx xxxt tttt  -  sturb Rt ADDR_SIMM9
xx11 1100 x0xi iiii iiii I0xx xxxt tttt  -  stur Ft ADDR_SIMM9
0111 1000 00xi iiii iiii I0xx xxxt tttt  -  sturh Rt ADDR_SIMM9
1x11 1000 00xi iiii iiii I0xx xxxt tttt  -  stur Rt ADDR_SIMM9
xx00 100x 001s ssss 0ttt ttxx xxxt tttt  -  stxp Rs Rt Rt2 ADDR_SIMPLE   
0000 100x 000s ssss 0xxx xxxx xxxt tttt  -  stxrb Rs Rt ADDR_SIMPLE
0100 100x 000s ssss 0xxx xxxx xxxt tttt  -  stxrh Rs Rt ADDR_SIMPLE
1x00 100x 000s ssss 0xxx xxxx xxxt tttt  -  stxr Rs Rt ADDR_SIMPLE
x10x 1110 xx1m mmmm 0110 00nn nnnd dddd  -  subhn2 Vd Vn Vm
x00x 1110 xx1m mmmm 0110 00nn nnnd dddd  -  subhn Vd Vn Vm
x100 1011 xx0x xxxx xxxx xxnn nnnd dddd  -  sub Rd Rn Rm_SFT
x10x 0001 SSii iiii iiii iinn nnnd dddd  -  sub Rd_SP Rn_SP AIMM
x100 1011 0x1x xxxx xxxx xxnn nnnd dddd  -  sub Rd_SP Rn_SP Rm_EXT
xx11 1110 xx1m mmmm x000 01nn nnnd dddd  -  sub Sd Sn Sm
x110 1011 xx0x xxxx xxxx xxnn nnnd dddd  -  subs Rd Rn Rm_SFT
x11x 0001 SSii iiii iiii iinn nnnd dddd  -  subs Rd Rn_SP AIMM
x110 1011 0x1x xxxx xxxx xxnn nnnd dddd  -  subs Rd Rn_SP Rm_EXT
xx10 1110 xx1m mmmm 1000 01nn nnnd dddd  -  sub Vd Vn Vm
x101 1110 xx1x xxxx 0011 10nn nnnd dddd  -  suqadd Sd Sn
xx00 1110 xx10 xxx0 0011 10nn nnnd dddd  -  suqadd Vd Vn
110x 0100 xx0i iiii iiii iiii iiix xx01  -  svc EXCEPTION 
x10x 01x1 xx10 1ooo nnnn mmmm ooot tttt  -  sysl Rt UIMM3_OP1 Cn Cm UIMM3_OP2  
x10x 01x1 xx00 1ooo nnnn mmmm ooot tttt  -  sys UIMM3_OP1 Cn Cm UIMM3_OP2 Rt  
xx00 1110 xx0m mmmm xxx0 00nn nnnd dddd  -  tbl Vd LVn Vm
bx1x 0111 bbbb biii iiii iiii iiit tttt  -  tbnz Rt BIT_NUM ADDR_PCREL14
xx00 1110 xx0m mmmm xxx1 00nn nnnd dddd  -  tbx Vd LVn Vm
bx1x 0110 bbbb biii iiii iiii iiit tttt  -  tbz Rt BIT_NUM ADDR_PCREL14
xx00 1110 xx0m mmmm x0x0 10nn nnnd dddd  -  trn1 Vd Vn Vm
xx00 1110 xx0m mmmm x1x0 10nn nnnd dddd  -  trn2 Vd Vn Vm
x110 1110 xx1m mmmm 0101 00nn nnnd dddd  -  uabal2 Vd Vn Vm
x010 1110 xx1m mmmm 0101 00nn nnnd dddd  -  uabal Vd Vn Vm
xx10 1110 xx1m mmmm 0111 11nn nnnd dddd  -  uaba Vd Vn Vm
x110 1110 xx1m mmmm x111 00nn nnnd dddd  -  uabdl2 Vd Vn Vm
x010 1110 xx1m mmmm x111 00nn nnnd dddd  -  uabdl Vd Vn Vm
xx10 1110 xx1m mmmm 0111 01nn nnnd dddd  -  uabd Vd Vn Vm
xx10 1110 xx1x 0xx0 0110 10nn nnnd dddd  -  uadalp Vd Vn
x110 1110 xx1m mmmm 0000 00nn nnnd dddd  -  uaddl2 Vd Vn Vm
xx10 1110 xx1x xxx0 0010 10nn nnnd dddd  -  uaddlp Vd Vn
x010 1110 xx1m mmmm 0000 00nn nnnd dddd  -  uaddl Vd Vn Vm
xx10 1110 xx11 xxx0 0011 10nn nnnd dddd  -  uaddlv Fd Vn
x110 1110 xx1m mmmm 0001 00nn nnnd dddd  -  uaddw2 Vd Vn Vm
x010 1110 xx1m mmmm 0001 00nn nnnd dddd  -  uaddw Vd Vn Vm
x10x 0011 0xii iiii iiii iinn nnnd dddd  -  ubfm Rd Rn IMMR IMMS   
xxx1 1110 xx1x x011 0000 00nn nnnd dddd  -  ucvtf Fd Rn
x0x1 1110 xx0x xx11 SSSS SSnn nnnd dddd  -  ucvtf Fd Rn FBITS
xx11 1110 0x1x xxx1 1101 10nn nnnd dddd  -  ucvtf Sd Sn
xx11 1111 xxxx xxxx 1x10 01nn nnnd dddd  -  ucvtf Sd Sn IMM_VLSR
xx10 1110 0x1x xxx1 1101 10nn nnnd dddd  -  ucvtf Vd Vn
x0x1 1010 1x0m mmmm xx0x 10nn nnnd dddd  -  udiv Rd Rn Rm
xx10 1110 xx1m mmmm 0000 01nn nnnd dddd  -  uhadd Vd Vn Vm
xx10 1110 xx1m mmmm 0010 01nn nnnd dddd  -  uhsub Vd Vn Vm
xxxx 1011 1x1m mmmm 0aaa aann nnnd dddd  -  umaddl Rd Rn Rm Ra   
xx10 1110 xx1m mmmm 1010 01nn nnnd dddd  -  umaxp Vd Vn Vm
xx10 1110 xx1m mmmm 0110 01nn nnnd dddd  -  umax Vd Vn Vm
xx1x 1110 xx11 xxx0 1010 10nn nnnd dddd  -  umaxv Fd Vn
xx10 1110 xx1m mmmm 1010 11nn nnnd dddd  -  uminp Vd Vn Vm
xx10 1110 xx1m mmmm 0110 11nn nnnd dddd  -  umin Vd Vn Vm
xx1x 1110 xx11 xxx1 1010 10nn nnnd dddd  -  uminv Fd Vn
x110 1111 xxxm mmmm 0010 x0nn nnnd dddd  -  umlal2 Vd Vn Em
x11x 1110 xx1m mmmm 1000 00nn nnnd dddd  -  umlal2 Vd Vn Vm
x010 1111 xxxm mmmm 0010 x0nn nnnd dddd  -  umlal Vd Vn Em
x01x 1110 xx1m mmmm 1000 00nn nnnd dddd  -  umlal Vd Vn Vm
x110 1111 xxxm mmmm 0110 x0nn nnnd dddd  -  umlsl2 Vd Vn Em
x11x 1110 xx1m mmmm 1010 00nn nnnd dddd  -  umlsl2 Vd Vn Vm
x010 1111 xxxm mmmm 0110 x0nn nnnd dddd  -  umlsl Vd Vn Em
x01x 1110 xx1m mmmm 1010 00nn nnnd dddd  -  umlsl Vd Vn Vm
xx00 1110 xx0x xxxx xx11 11nn nnnd dddd  -  umov Rd En
xxxx 1011 1x1m mmmm 1aaa aann nnnd dddd  -  umsubl Rd Rn Rm Ra   
xxx1 1011 110m mmmm 0xxx xxnn nnnd dddd  -  umulh Rd Rn Rm
x110 1111 xxxm mmmm 1x10 x0nn nnnd dddd  -  umull2 Vd Vn Em
x110 1110 xx1m mmmm 1100 00nn nnnd dddd  -  umull2 Vd Vn Vm
x010 1111 xxxm mmmm 1x10 x0nn nnnd dddd  -  umull Vd Vn Em
x010 1110 xx1m mmmm 1100 00nn nnnd dddd  -  umull Vd Vn Vm
xx11 1110 xx1m mmmm 0000 11nn nnnd dddd  -  uqadd Sd Sn Sm
xx10 1110 xx1m mmmm 0000 11nn nnnd dddd  -  uqadd Vd Vn Vm
xx11 1110 xx1m mmmm xx01 11nn nnnd dddd  -  uqrshl Sd Sn Sm
xx10 1110 xx1m mmmm 0101 11nn nnnd dddd  -  uqrshl Vd Vn Vm
x110 1111 xxxx xxxx 1x01 11nn nnnd dddd  -  uqrshrn2 Vd Vn IMM_VLSR
xx11 1111 xxxx xxxx 1x01 11nn nnnd dddd  -  uqrshrn Sd Sn IMM_VLSR
x010 1111 xxxx xxxx 1x01 11nn nnnd dddd  -  uqrshrn Vd Vn IMM_VLSR
xx11 1111 xxxx xxxx 0111 xxnn nnnd dddd  -  uqshl Sd Sn IMM_VLSL
xx11 1110 xx1m mmmm x100 11nn nnnd dddd  -  uqshl Sd Sn Sm
xx10 1110 xx1m mmmm 0100 11nn nnnd dddd  -  uqshl Vd Vn Vm
xx11 1111 xxxx xxxx 1xx1 01nn nnnd dddd  -  uqshrn Sd Sn IMM_VLSR
xx11 1110 xx1m mmmm x010 11nn nnnd dddd  -  uqsub Sd Sn Sm
xx10 1110 xx1m mmmm 0010 11nn nnnd dddd  -  uqsub Vd Vn Vm
x110 1110 xx1x xxx1 0100 10nn nnnd dddd  -  uqxtn2 Vd Vn
xx11 1110 xx1x xxxx 0100 10nn nnnd dddd  -  uqxtn Sd Sn
x010 1110 xx1x xxx1 0100 10nn nnnd dddd  -  uqxtn Vd Vn
xx0x 1110 1x1x xxx1 1100 10nn nnnd dddd  -  urecpe Vd Vn
xx10 1110 xx1m mmmm 0001 01nn nnnd dddd  -  urhadd Vd Vn Vm
xx11 1110 xx1m mmmm 0x01 01nn nnnd dddd  -  urshl Sd Sn Sm
xx10 1110 xx1m mmmm 0101 01nn nnnd dddd  -  urshl Vd Vn Vm
xx11 1111 xxxx xxxx 0010 xxnn nnnd dddd  -  urshr Sd Sn IMM_VLSR
xx1x 1110 1x1x xxx1 1100 10nn nnnd dddd  -  ursqrte Vd Vn
xx11 1111 xxxx xxxx 0011 xxnn nnnd dddd  -  ursra Sd Sn IMM_VLSR
xx11 1110 xx1m mmmm x100 01nn nnnd dddd  -  ushl Sd Sn Sm
xx10 1110 xx1m mmmm 0100 01nn nnnd dddd  -  ushl Vd Vn Vm
xx11 1111 xxxx xxxx 0000 xxnn nnnd dddd  -  ushr Sd Sn IMM_VLSR
xx11 1110 xx1x xxxx 0011 10nn nnnd dddd  -  usqadd Sd Sn
xx10 1110 xx10 xxx0 0011 10nn nnnd dddd  -  usqadd Vd Vn
xx11 1111 xxxx xxxx 0001 xxnn nnnd dddd  -  usra Sd Sn IMM_VLSR
x110 1110 xx1m mmmm 0010 00nn nnnd dddd  -  usubl2 Vd Vn Vm
x010 1110 xx1m mmmm 0010 00nn nnnd dddd  -  usubl Vd Vn Vm
x110 1110 xx1m mmmm 0011 00nn nnnd dddd  -  usubw2 Vd Vn Vm
x010 1110 xx1m mmmm 0011 00nn nnnd dddd  -  usubw Vd Vn Vm
xx00 1110 xx0m mmmm x001 10nn nnnd dddd  -  uzp1 Vd Vn Vm
xx00 1110 xx0m mmmm x101 10nn nnnd dddd  -  uzp2 Vd Vn Vm
x100 1110 xx1x xxx1 0010 10nn nnnd dddd  -  xtn2 Vd Vn
x000 1110 xx1x xxx1 0010 10nn nnnd dddd  -  xtn Vd Vn
xx00 1110 xx0m mmmm x011 10nn nnnd dddd  -  zip1 Vd Vn Vm
xx00 1110 xx0m mmmm x111 10nn nnnd dddd  -  zip2 Vd Vn Vm
*/
}

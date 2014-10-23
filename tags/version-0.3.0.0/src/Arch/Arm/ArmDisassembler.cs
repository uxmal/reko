#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using System.IO;
using System.Text;

/* disarm -- a simple disassembler for ARM instructions
 * (c) 2000 Gareth McCaughan
 *
 * This file may be distributed and used freely provided:
 * 1. You do not distribute any version that lacks this
 *    copyright notice (exactly as it appears here, extending
 *    from the start to the end of the C-language comment
 *    containing these words)); and,
 * 2. If you distribute any modified version, its source
 *    contains a clear description of the ways in which
 *    it differs from the original version, and a clear
 *    indication that the changes are not mine.
 * There is no restriction on your permission to use and
 * distribute object code or executable code derived from
 * this.
 *
 * The original version of this file (or perhaps a later
 * version by the original author) may or may not be
 * available at http://web.ukonline.co.uk/g.mccaughan/g/software.html .
 *
 * Share and enjoy!    -- g
 */

namespace Decompiler.Arch.Arm
{
    using word = UInt32;
    using address = UInt32;
    using addrdiff = UInt32;

    public class ArmDisassembler2 : DisassemblerBase<ArmInstruction>
    {
        private ArmProcessorArchitecture arch;
        private ImageReader rdr;
        private ArmInstruction arm;
        private uint addr;
        private uint instr;

        private static Opcode[] dataprocessingOps = new Opcode[] {
            Opcode.and, Opcode.eor, Opcode.sub, Opcode.rsb, 
            Opcode.add, Opcode.adc, Opcode.sbc, Opcode.rsc,
            Opcode.tst, Opcode.teq, Opcode.cmp, Opcode.cmn,
            Opcode.orr, Opcode.mov, Opcode.bic, Opcode.mvn,
        };
        private static Opcode[] shiftTypes = new Opcode[] { 
            Opcode.lsl, Opcode.lsr, Opcode.asr, Opcode.ror 
        };

        public class DisOptions
        {
        }

        // Some important single-bit fields. 

        const int Sbit = (1 << 20);	// set condition codes (data processing) 
        const int Lbit = (1 << 20);	// load, not store (data transfer) 
        const int Wbit = (1 << 21);	// writeback (data transfer) 
        const int Bbit = (1 << 22);	// single byte (data transfer, SWP) 
        const int Ubit = (1 << 23);	// up, not down (data transfer) 
        const int Pbit = (1 << 24);	// pre-, not post-, indexed (data transfer) 
        const int Ibit = (1 << 25);	// non-immediate (data transfer) 

        // immediate (data processing) 
        const int SPSRbit = (1 << 22);	// SPSR, not CPSR (MRS, MSR) 

        // Some important 4-bit fields. 

        private int RD(int x) { return ((x) << 12); }	// destination register 
        private uint RN(uint x) { return ((x) << 16); }	// operand/base register 
        private int CP(int x) { return ((x) << 8); }	// coprocessor number 
        private int RDbits() { return RD(15); }
        private uint RNbits() { return RN(15); }
        private int CPbits() { return CP(15); }
        private bool RD_is(int x) { return ((instr & RDbits()) == RD(x)); }
        private bool RN_is(uint x) { return ((instr & RNbits()) == RN(x)); }
        private bool CP_is(int x) { return ((instr & CPbits()) == CP(x)); }

        public ArmDisassembler2(ArmProcessorArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override ArmInstruction Current { get { return arm; } }

        public override bool MoveNext()
        {
            if (!rdr.IsValid)
                return false;

            arm = new ArmInstruction
            {
                Address = rdr.Address,
                Length = 4,
            };

            addr = arm.Address.Linear;
            this.Disassemble(rdr.ReadLeUInt32(), new DisOptions());
            return true;
        }

        public ArmInstruction Disassemble(word instr, DisOptions opts)
        {
            this.instr = instr;
            DumpBits(instr);
            arm.Cond = ConditionField();
            if (arm.Cond == Condition.nv)
                return DecodeUnconditional();
            switch ((instr >> 24) & 0xF)
            {
            case 0x0:
                if ((instr & 0x0FE000F0) == 0x00000090)
                {
                    arm.Opcode = Opcode.mul;
                    if (Bit_S_Set(instr))
                        arm.OpFlags = OpFlags.S;
                    DecodeOperands(instr, PrimitiveType.Word32, "4,0,2");
                }
                else if ((instr & 0x0FE000F0) == 0x00200090)
                {
                    arm.Opcode = Opcode.mla;
                    if (Bit_S_Set(instr))
                        arm.OpFlags = OpFlags.S;
                    DecodeOperands(instr, PrimitiveType.Word32, "4,0,2,3");
                }
                else
                {
                    DecodeAluInstruction();
                }
                break;
            case 0x1:
                if ((instr & 0x0FB00FF0) == 0x01000090)
                {
                    arm.Opcode = BitN_Set(instr, 22) ? Opcode.swpb : Opcode.swp;
                    DecodeOperands(
                        instr, 
                        BitN_Set(instr, 22) ? PrimitiveType.Byte : PrimitiveType.Word32, 
                        "3,0,I4");
                }
                else
                {
                    DecodeAluInstruction();
                }
                break;
            case 0x2: case 0x3:
                DecodeAluInstruction();
                break;
            case 0x4: 
            case 0x5:
            case 0x6:
            case 0x7:
                DecodeSingleDataTransfer();
                break;
            case 0x8:
            case 0x9:
                DecodeLdmStm();
                break;
            case 0xA:
                arm.Opcode = Opcode.b;
                DecodeOperands(instr, null, "&");
                break;
            case 0xB:
                arm.Opcode = Opcode.bl;
                DecodeOperands(instr, null, "&");
                break;
            case 0xC: case 0x0D: case 0x0E: case 0x0F:
                DecodeSvcAndCoproc();
                break;
            }
            return arm;
        }

        private void DecodeAluInstruction()
        {
            uint encodedOp = (instr >> 21) & 0xF;
            arm.Opcode = dataprocessingOps[encodedOp];
            if (Bit_S_Set(instr))
                arm.OpFlags = OpFlags.S;
            switch (encodedOp)
            {
            case 0: case 1: case 2: case 3:
            case 4: case 5: case 6: case 7:
            case 0xC: case 0xE:
                DecodeOperands(instr, PrimitiveType.Word32, "3,4,*");
                break;
            case 8: case 9: case 0xA: case 0xB:
                DecodeOperands(instr, PrimitiveType.Word32, "4,*");
                arm.OpFlags = OpFlags.None;
                break;
            case 0xD: case 0xF:
                DecodeOperands(instr,PrimitiveType.Word32, "3,*");
                break;
            }
        }

        private void DecodeSingleDataTransfer()
        {
            PrimitiveType width = null;
            switch ((instr >> 21) & 2 | (instr >> 20) & 1)
            {
            case 0: arm.Opcode = Opcode.str; width = PrimitiveType.Word32; break;
            case 1: arm.Opcode = Opcode.ldr; width = PrimitiveType.Word32; break;
            case 2: arm.Opcode = Opcode.strb;width = PrimitiveType.Byte; break;
            case 3: arm.Opcode = Opcode.ldrb;width = PrimitiveType.Byte; break;
            }
            DecodeOperands(instr, width, "3,/");
        }

        private void DecodeLdmStm()
        {
            arm.Opcode = Bit_20_Set(instr) ? Opcode.ldm : Opcode.stm;
            if (RN_is(13))
            {
                // r13, so treat as stack 
                int x = (int) (instr & (3 << 23)) >> 22;
                if ((instr & Lbit) != 0)
                    x ^= 6;
                arm.OpFlags = new OpFlags[] { 
                    OpFlags.ED,
                    OpFlags.EA,
                    OpFlags.FD,
                    OpFlags.FA
                }[x >> 1];
            }
            else
            {
                // not r13, so don't treat as stack 
                arm.OpFlags =  ((instr & Ubit) != 0) 
                    ? ((instr & Pbit) != 0) ? OpFlags.IB : OpFlags.IA
                    : ((instr & Pbit) != 0) ? OpFlags.DB : OpFlags.DA;
            }
            arm.Update = BitN_Set(instr, 21);
            DecodeOperands(instr, PrimitiveType.Word32, "4,%");
        }

        private void DecodeSvcAndCoproc()
        {
            switch ((instr >> 24) & 0x3)
            {
            case 0:
            case 1:
            case 2:
                IllegalInstruction(instr);
                break;
            case 3:
                arm.Opcode = Opcode.svc;
                DecodeOperands(instr, null, "$");
                break;
            }
        }

        private ArmInstruction DecodeUnconditional()
        {
            arm.Cond = Condition.al;
            switch ((instr >> 24) & 0xF)
            {
            case 0: case 1: case 2: case 3:
            case 4: case 5: case 6: case 7:
                return DecodeMiscSimd();
            case 8: case 9:
                // SRS / RFE 
            case 0xA: case 0xB:
                // Branch with link and exchange
                arm.Opcode = Opcode.blx;
                DecodeOperands(instr, null,"x");
                return arm;
            case 0xC: case 0xD: 
                // Stored load coprocessor
            case 0xE:
                // Coprocessor data operations.
            case 0xF:
            default:
                return IllegalInstruction(instr);
            }
        }

        private ArmInstruction DecodeMiscSimd()
        {
            switch ((instr >> 20) & 0x7F)
            {
            case 0x10:
                if (instr == 0xF1010000)
                {
                    arm.Opcode = Opcode.setendle;
                    return arm;
                }
                else if (instr == 0xF1010200)
                {
                    arm.Opcode = Opcode.setendbe;
                    return arm;
                }
                return IllegalInstruction(instr);
            }
            return IllegalInstruction(instr);
        }


        private ArmInstruction IllegalInstruction(uint instr)
        {
            arm.Opcode = Opcode.illegal;
            arm.Cond = Condition.al;
            return arm;
        }

        private static bool Bit_S_Set(word instr)
        {
            return ((instr >> 20) & 1) != 0;
        }

        private static bool Bit_20_Set(word instr) { return ((instr >> 20) & 1) != 0; }
        private static bool BitN_Set(word instr, int n) { return ((instr >> n) & 1) != 0; }

        private Condition ConditionField()
        {
            return (Condition)(instr >> 28);
        }

        [Conditional("DEBUG")]
        public static void DumpBits(word instr)
        {
            string [] b = new string[16] {
                "0000", "0001", "0010", "0011", 
                "0100", "0101", "0110", "0111", 
                "1000", "1001", "1010", "1011", 
                "1100", "1101", "1110", "1111",
            };
            Debug.Print("{0} {1} {2} {3} {4} {5} {6} {7}",
                b[(instr >> 28) & 15], b[(instr >> 24) & 15], b[(instr >> 20) & 15], b[(instr >> 16) & 15],
                b[(instr >> 12) & 15], b[(instr >> 8) & 15], b[(instr >> 4) & 15], b[(instr) & 15]);
        }

        private void DecodeOperands(word instr, PrimitiveType width, string format)
        {
            var ops = new MachineOperand[4];
            int iOp = 0;
            int offset;
            uint dstAddr;
            for (int i = 0; i < format.Length; ++i)
            {
                char ch = format[i];
                switch (ch)
                {
                default:
                    throw new NotImplementedException(string.Format("Unknown format character '{0}'.", ch));
                case '0': case '1': case '2': case '3': case '4':
                    ops[iOp] = new RegisterOperand(A32Registers.GpRegs[(instr >> ((ch - '0') << 2)) & 0xF]);
                    break;
                case '*':
                    ops[iOp] = DecodeImmediateOperand(instr);
                    break;
                case '&':   // Offset for b and bl instructions.
                    offset = ((int)instr << 8) >> 6;
                    dstAddr = (uint)(addr + 8 + offset);
                    ops[iOp] = new AddressOperand(new Address(dstAddr));
                    break;
                case 'x':   // Offset for blx instructions
                    offset = ((int) instr << 8) >> 6;
                    offset |= (int)((instr >> 23) & 2);
                    dstAddr = (uint) (addr + 8 + offset);
                    ops[iOp] = new AddressOperand(new Address(dstAddr));
                    break;
                case '/':   // Format for ldr, str
                    ops[iOp] = DecodeIndirectOperand(width, instr);
                    break;
                case 'I':   // Indirect register, nibble in following character.
                    ops[iOp] = new ArmMemoryOperand(width, A32Registers.GpRegs[(instr >> ((format[++i] - '0') << 2)) & 0xF]);
                    break;
                case ',':
                    ++iOp;
                    break;
                case '%': // Register range
                    ops[iOp] = new RegisterRangeOperand(instr & 0xFFFF);
                    break;
                case '$':
                    ops[iOp] = new ImmediateOperand(Constant.Word32(instr & 0x00FFFFFF));
                    break;
                }
            }
            if (ops[0] != null)
            {
                arm.Dst = ops[0];
                if (ops[1] != null)
                {
                    arm.Src1 = ops[1];
                    if (ops[2] != null)
                    {
                        arm.Src2 = ops[2];
                        if (ops[3] != null)
                        {
                            arm.Src3 = ops[3];
                        }
                    }
                }
            }
        }

        private PrimitiveType GetWidth(char w)
        {
            switch (w)
            {
            case 'b': return PrimitiveType.Byte;
            case 'h': return PrimitiveType.Word16;
            case 'w': return PrimitiveType.Word32;
            default: throw new NotImplementedException();
            }
        }

        private MachineOperand DecodeImmediateOperand(word instr)
        {
            if (((instr >> 25) & 1) == 0)
            {
                return DecodeShiftOperand(instr);
            }
            else
            {
                // Immediate value in bits 0..7, rotate amount in 8..11
                uint imm8 = instr & 0xFF;
                int rotAmt = (int)((instr >> 7) & 0x1E);
                return ArmImmediateOperand.Word32((imm8 >> rotAmt) | (imm8 << (32 - rotAmt)));
            }
        }

        private MachineOperand DecodeShiftOperand(word instr)
        {
            // Register (with shift) in 0..11
            var shiftType = shiftTypes[(instr >> 5) & 3];
            var reg = get_regop(instr & 0xF);
            if ((instr & (1 << 4)) == 0)
            {
                // Shift by immediate amount in 7..11
                int immShift = (int)((instr >> 7) & 0x1F);
                if (immShift == 0)
                {
                    switch (shiftType)
                    {
                    case Opcode.lsl:
                        // Special case: use Rm directly.
                        return reg;
                    case Opcode.lsr:
                    case Opcode.asr:
                        // Special case: lsr,asr #0 is actually lsr,asr #32
                        immShift = 32;
                        break;
                    case Opcode.ror:
                        // Special case: ror 0 is actually rrx 1
                        shiftType = Opcode.rrx;
                        immShift = 1;
                        break;
                    }
                }
                return new ShiftOperand(reg, shiftType, immShift);
            }
            else
            {
                // Shift by register in 8..11
                return new ShiftOperand(reg, shiftType, get_regop(instr >> 8));
            }
        }

        private ArmMemoryOperand DecodeIndirectOperand(PrimitiveType width, uint instr)
        {
            var rn = A32Registers.GpRegs[(instr >> 16) & 0xF];
            MachineOperand offset;
            if (BitN_Set(instr, 25))
            {
                // Offset is shifted register
                offset = DecodeShiftOperand(instr);
            }
            else
            {
                // Offset is 12-bit immediate value-
                var o = instr & 0xFFF;
                offset = o != 0 ? ArmImmediateOperand.Word32(o) : null;
            }
            if (((instr >> 24) & 1) == 0)
            {
                // Post index
                return new ArmMemoryOperand(width, rn, offset)
                {
                    Preindexed = false,
                    Subtract = !BitN_Set(instr, 23),
                };
            }
            else
            {
                // Pre index
                var op = new ArmMemoryOperand(width, rn, offset)
                {
                    Preindexed = true,
                    Writeback = BitN_Set(instr, 21),
                    Subtract = !BitN_Set(instr, 23),
                };
                return op;
            }
        }

        private static RegisterOperand get_regop(addrdiff bits)
        {
            return new RegisterOperand(A32Registers.GpRegs[bits&0xF]);
        }
    }

    public class ArmDisassembler3 : DisassemblerBase<ArmInstruction>
    {

        /* (*This* comment is NOT part of the notice mentioned in the
         * distribution conditions above.)
         *
         * The bulk of this code was ripped brutally from the middle
         * of a much more interesting piece of software whose purpose
         * is to disassemble object files in the format known as AOF;
         * it's quite clever at spotting blocks of non-code embedded
         * in code, identifying labels, and so on.
         *
         * This program, on the other hand, is very much simpler.
         * It simply disassembles one instruction at a time. Some
         * traces of the original purpose can be seen here and there.
         * You might want to make this do a two-phase disassembly,
         * adding labels etc the second time around. I've made this
         * work by loading the whole file into memory first, partly
         * because that makes a two-pass approach easier.
         *
         * One word of warning: I believe that the syntax this program
         * uses for the MSR instruction is now obsolete.
         *
         * Usage:
         *   disarm <filename> <base-address>
         * will disassemble every word in <filename>.
         *
         * <base-address> should be something understood by strtol.
         * So you can get hex (which is probably what you want)
         * by prefixing "0x".
         *
         * The -r option will byte-reverse each word before it's
         * disassembled.
         *
         * The code is rather unmaintainable. I'm sorry.
         *
         * Changes since original release:
         *   ????-??-?? v0.00 Initial release.
         *   2007-09-02 v0.11 Change %X to %lX in a format string.
         *                    (Thanks to Vincent Zweije for reporting this.)
         */

        private ArmProcessorArchitecture arch;
        private ImageReader rdr;
        private ArmInstruction arm;
        private eTargetType poss_tt;
        private OpFlags flag;
        private uint addr;

        public ArmDisassembler3(ArmProcessorArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override ArmInstruction Current { get { return arm; } }

        public override bool MoveNext()
        {
            if (!rdr.IsValid)
                return false;
            var a = rdr.Address;
            addr = a.Linear;
            this.Disassemble(rdr.ReadLeUInt32(), new DisOptions());
            arm.Address = a;
            arm.Length = rdr.Address - a;
            return true;
        }

        public enum eTargetType
        {
            target_None,		// instruction doesn't refer to an address 
            target_Data,		// instruction refers to address of data 
            target_FloatS,	    // instruction refers to address of single-float 
            target_FloatD,	    // instruction refers to address of double-float 
            target_FloatE,	    // blah blah extended-float 
            target_FloatP,	    // blah blah packed decimal float 
            target_Code,		// instruction refers to address of code 
            target_Unknown	    // instruction refers to address of *something* 
        }

        public class Instruction
        {
            public StringBuilder text = new StringBuilder();	// the disassembled instruction 
            public bool undefined;	// non-0 iff it's an undefined instr 
            public bool badbits;		// non-0 iff something reserved has the wrong value 
            public bool oddbits;		// non-0 iff something unspecified isn't 0 
            public bool is_SWI;		// non-0 iff it's a SWI 
            public word swinum;		// only set for SWIs 
            public address target;	// address instr refers to 
            public eTargetType target_type;	// and what we expect to be there 
            public int offset;		// offset from register in LDR or STR or similar 
            public int addrstart;	// start of address part of instruction, or 0 
        }

        [Flags]
        public enum disopt : uint
        {
            SWInames = 1,	    // use names, not &nnnn 
            FIXS = 4,	        // bogus FIX syntax for ObjAsm 
            ReverseBytes = 8,	// byte-reverse words first 
        }

        public class DisOptions
        {
            public disopt flags;
        }

        const bool INSTR_grok_v4 = true;

        /* Preprocessor defs you can give to affect this stuff:
         * INSTR_grok_v4   understand ARMv4 instructions (halfword & sign-ext LDR/STR)
         * INSTR_new_msr   be prepared to produce new MSR syntax if asked
         * The first of these is supported; the second isn't.
         */

        // Some important single-bit fields. 

        const int Sbit = (1 << 20);	// set condition codes (data processing) 
        const int Lbit = (1 << 20);	// load, not store (data transfer) 
        const int Wbit = (1 << 21);	// writeback (data transfer) 
        const int Bbit = (1 << 22);	// single byte (data transfer, SWP) 
        const int Ubit = (1 << 23);	// up, not down (data transfer) 
        const int Pbit = (1 << 24);	// pre-, not post-, indexed (data transfer) 
        const int Ibit = (1 << 25);	// non-immediate (data transfer) 

        // immediate (data processing) 
        const int SPSRbit = (1 << 22);	// SPSR, not CPSR (MRS, MSR) 

        // Some important 4-bit fields. 

        private int RD(int x) { return ((x) << 12); }	// destination register 
        private uint RN(uint x) { return ((x) << 16); }	// operand/base register 
        private int CP(int x) { return ((x) << 8); }	// coprocessor number 
        private int RDbits() { return RD(15); }
        private uint RNbits() { return RN(15); }
        private int CPbits() { return CP(15); }
        private bool RD_is(int x) { return ((instr & RDbits()) == RD(x)); }
        private bool RN_is(uint x) { return ((instr & RNbits()) == RN(x)); }
        private bool CP_is(int x) { return ((instr & CPbits()) == CP(x)); }

        /* A slightly efficient way of telling whether two bits are the same
         * or not. It's assumed that a<b.
         */
        private bool BitsDiffer(int a, int b) { return (((instr >> a) ^ (instr >> b)) & 1) != 0; }

        //extern void swiname(word, char *, size_t);

        /* op = append(op,ip) === op += sprintf(op,"%s",ip),
         * except that it's faster.
         */
        static void append(StringBuilder op, string ip)
        {
            op.Append(ip);
        }

        /* op = hex8(op,w) === op += sprintf(op,"&%08lX",w), but faster.
         */
        static void hex8(StringBuilder op, word w)
        {
            int i;
            op.Append('&');
            for (i = 28; i >= 0; i -= 4) op.Append("0123456789ABCDEF"[(int)((w >> i) & 15)]);
        }

        /* op = reg(op,'x',n) === op += sprintf(op,"x%lu",n&15).
         */
        static void reg(StringBuilder op, char c, word n)
        {
            op.Append(c);
            op.Append(n & 15);
        }

        /* op = num(op,n) appends n in decimal or &n in hex
         * depending on whether n<100. It's assumed that n>=0.
         */
        static void num(StringBuilder op, word w)
        {
            if (w >= 100)
            {
                int i;
                word t;
                op.Append('&');
                for (i = 28; (t = (w >> i) & 15) == 0; i -= 4) ;
                for (; i >= 0; i -= 4) op.Append("0123456789ABCDEF"[(int)((w >> i) & 15)]);
            }
            else
            {
                // divide by 10. You can prove this works by exhaustive search. :-) 
                word t = w - (w >> 2); t = (t + (t >> 4)) >> 3;
                {
                    word u = w - 10 * t;
                    if (u == 10) { u = 0; ++t; }
                    if (t != 0) op.Append((char)(t + '0'));
                    op.Append((char)(u + '0'));
                }
            }
        }

        /* instr_disassemble
         * Disassemble a single instruction.
         *
         * args:   instr   a single ARM instruction
         *         addr    the address it's presumed to have come from
         *         opts    cosmetic preferences for our output
         *
         * reqs:   opts must be filled in right. In particular, it must contain
         *         a list of register names.
         *
         * return: a pointer to a structure containing the disassembled instruction
         *         and some other information about it.
         *
         * This is basically a replacement for the SWI Debugger_Disassemble,
         * but it has the following advantages:
         *
         *   + it's 3-4 times as fast
         *   + it's better at identifying undefined instructions,
         *     and instructions not invariant under { disassemble; ObjAsm; }
         *   + it provides some other useful information as well
         *   + its output syntax is the same as ObjAsm's input syntax
         *     (where possible)
         *   + it doesn't disassemble FIX incorrectly unless you ask it to
         *   + it's more configurable in some respects
         *
         * It also has the following disadvantages:
         *
         *   - it increases the size of ObjDism
         *   - it doesn't provide so many `helpful' usage comments etc
         *   - it's less configurable in some respects
         *   - it doesn't (yet) know about ARMv4 instructions
         *
         * This function proceeds in two phases. The first is very simple:
         * it works out what sort of instruction it's looking at and sets up
         * three strings:
         *   - |mnemonic|  (the basic mnemonic: LDR or whatever)
         *   - |flagchars| (things to go after the cond code: B or whatever)
         *   - |format|    (a string describing how to display the instruction)
         * The second phase consists of interpreting |format|, character by
         * character. Some characters (e.g., letters) just mean `append this
         * character to the output string'; some mean more complicated things
         * like `append the name of the register whose number is in bits 12..15'
         * or, worse, `append a description of the <op2> field'.
         *
         * I'm afraid the magic characters in |format| are rather arbitrary.
         * One criterion in choosing them was that they should form a contiguous
         * subrange of the character set! Sorry.
         *
         * Things I still want to do:
         *
         *   - more configurability?
         *   - make it much faster, if possible
         *   - make it much smaller, if possible
         *
         * Format characters:
         *
         *   \01..\05 copro register number from nybble (\001 == nybble 0, sorry)
         *   $        SWI number
         *   %        register set for LDM/STM (takes note of bit 22 for ^)
         *   &        address for B/BL
         *   '        ! if bit 21 set, else nothing (mnemonic: half a !)
         *   (        #regs for SFM (bits 22,15 = fpn, assumed already tweaked)
         *   )        copro opcode in bits 20..23 (for CDP)
         *   *        op2 (takes note of bottom 12 bits, and bit 25)
         *   +        FP register or immediate value: bits 0..3
         *   ,        comma or comma-space
         *   -        copro extra info in bits 5..7 preceded by , omitted if 0
         *   .        address in ADR instruction
         *   /        address for LDR/STR (takes note of bit 23 & reg in bits 16..19)
         *   0..4     register number from nybble
         *   5..9     FP register number from nybble
         *   :        copro opcode in bits 21..23 (for MRC/MCR)
         *   ;        copro number in bits 8..11
         *
         * NB that / takes note of bit 22, too, and does its own ! when
         * appropriate.
         *
         * On typical instructions this seems to take about 100us on my ARM6;
         * that's about 3000 cycles, which seems grossly excessive. I'm not
         * sure where all those cycles are being spent. Perhaps it's possible
         * to make it much, much faster. Most of this time is spent on phase 2.
         */
        StringBuilder flagchars;
        StringBuilder flagp;
        Instruction result;
        word fpn;
        word instr;
        Opcode mnemonic;
        PrimitiveType width;

        public ArmInstruction Disassemble(word instr) { return Disassemble(instr, new DisOptions()); }

        public ArmInstruction Disassemble(word instr, DisOptions opts)
        {
            this.instr = instr;
            this.mnemonic = Opcode.illegal;
            this.flagp = new StringBuilder();
            this.flagchars = flagp;

            this.fpn = (word)0;
            this.poss_tt = eTargetType.target_None;
            int is_v4 = 0;
            string format = "";
            this.width = PrimitiveType.Word32;

            // PHASE 0. Set up default values for |result|. 

            fpn = ((instr >> 15) & 1) + ((instr >> 21) & 2);

            result = new Instruction();
            result.undefined =
                result.badbits =
                result.oddbits =
                result.is_SWI = false;
            result.target_type = eTargetType.target_None;
            result.offset = (int)-0x80000000;
            result.addrstart = 0;

            // PHASE 1. Decode and classify instruction. 

            switch ((instr >> 24) & 15)
            {
            case 0:
                // multiply or data processing, or LDRH etc 
                if ((instr & (15 << 4)) != (9 << 4))
                    goto lMaybeLDRHetc;
                // multiply 
                if ((instr & (1 << 23)) != 0)
                {
                    // long multiply 
                    width = PrimitiveType.Word64;
                    mnemonic = new Opcode[] { Opcode.umull, Opcode.umlal, Opcode.smull, Opcode.smlal }[(instr >> 21) & 3];
                    format = "3,4,0,2";
                }
                else
                {
                    if ((instr & (1 << 22)) != 0)
                        return IllegalInstruction();	// "class C" 
                    // short multiply 
                    if ((instr & (1 << 21)) != 0)
                    {
                        mnemonic = Opcode.mla;
                        format = "4,0,2,3";
                    }
                    else
                    {
                        mnemonic = Opcode.mul;
                        format = "4,0,2";
                    }
                }
                if ((instr & Sbit) != 0)
                {
                    this.flag = OpFlags.S;
                    flagp.Append('S');
                }
                break;
            case 1:
            case 3:
                // SWP or MRS/MSR or data processing 
                if ((instr & 0x02B00FF0) == 0x00000090)
                {
                    // SWP 
                    mnemonic = Opcode.swp;
                    format = "3,0,[4]";
                    if ((instr & Bbit) != 0)
                    {
                        width = PrimitiveType.Byte;
                        flagp.Append('B');
                    }
                    break;
                }
                else if ((instr & 0x02BF0FFF) == 0x000F0000)
                {
                    // MRS 
                    mnemonic = Opcode.mrs;
                    format = ((instr & SPSRbit) != 0) ? "3,SPSR" : "3,CPSR";
                    break;
                }
                else if ((instr & 0x02BFFFF0) == 0x0029F000)
                {
                    // MSR psr<P=0/1...>,Rs 
                    mnemonic = Opcode.msr;
                    format = ((instr & SPSRbit) != 0) ? "SPSR,0" : "CPSR,0";
                    break;
                }
                else if ((instr & 0x00BFF000) == 0x0028F000)
                {
                    // MSR {C,S}PSR_flag,op2 
                    mnemonic = Opcode.msr;
                    format = ((instr & SPSRbit) != 0) ? "SPSR_flg,*" : "CPSR_flg,*";
                    if ((instr & Ibit) == 0 && ((instr & (15 << 4)) != 0))
                        goto lMaybeLDRHetc;
                    break;
                }
            // fall through here 
            lMaybeLDRHetc:
                if ((instr & (14 << 24)) == 0
                    && 
                    ((instr & (9 << 4)) == (9 << 4)))
                {
                    // Might well be LDRH or similar. 
                    if ((instr & (Wbit + Pbit)) == Wbit)
                        return IllegalInstruction();	// "class E", case 1 
                    if ((instr & (Lbit + (1 << 6))) == (1 << 6))
                        return IllegalInstruction();	// STRSH etc 
                    mnemonic = ((instr & Lbit) >> 18) != 0 ? Opcode.ldr : Opcode.str;
                    switch ((instr & (3 << 5)) >> 5)
                    {
                    case 0: width = PrimitiveType.Byte; break;
                    case 1: width = PrimitiveType.Word16; break;
                    case 2: flag = OpFlags.S; width = PrimitiveType.Byte; break;
                    case 3: flag = OpFlags.S; width = PrimitiveType.Word16; break;
                    }
                    format = "3,/";
                    // aargh: 
                    if ((instr & (1 << 22)) == 0)
                        instr |= Ibit;
                    is_v4 = 1;
                    break;
                }
                goto case 2;
            case 2:
                // data processing 
                {
                    word op21 = (instr >> 21) & 15;
                    if ((op21 == 2 || op21 == 4)			// ADD or SUB 
                        && ((instr & (RNbits() + Ibit + Sbit)) == RN(15) + Ibit)	// imm, no S 
                        && ((instr & (30 << 7)) == 0 || (instr & 3) != 0))
                    {		// normal rot 
                        // ADD ...,pc,#... or SUB ...,pc,#...: turn into ADR 
                        mnemonic = Opcode.adr;
                        format = "3,.";
                        if ((instr & (30 << 7)) != 0 && (instr & 3) == 0) 
                            result.oddbits = true;
                        break;
                    }
                    mnemonic = aluOps[op21 >> 19];
                    // Rd needed for all but TST,TEQ,CMP,CMN (8..11) 
                    // Rn needed for all but MOV,MVN (13,15) 
                    if (op21 < (8 << 21))
                    {
                        format = "3,4,*";
                    }
                    else if (op21 < (12 << 21))
                    {
                        format = "4,*";
                        if ((instr & RDbits()) != 0)
                        {
                            if (((instr & Sbit) != 0) && RD_is(15))
                            {
                                flag = OpFlags.P;
                                flagp.Append('P');
                            }
                            else
                                result.oddbits = true;
                        }
                        if ((instr & Sbit) == 0)
                            return IllegalInstruction();	// CMP etc, no S bit 
                    }
                    else if ((op21 & (1 << 21)) != 0)
                    {
                        format = "3,*";
                        if ((instr & RNbits()) != 0) 
                            result.oddbits = true;
                    }
                    else
                    {
                        format = "3,4,*";
                    }
                    if ((instr & Sbit) != 0 && (op21 < (8 << 21) || op21 >= (12 << 21)))
                    {
                        flag = OpFlags.S;
                        flagp.Append('S');
                    }
                }
                break;
            case 4:
            case 5:
            case 6:
            case 7:
                // undefined or STR/LDR 
                if (((instr & Ibit) != 0) && (instr & (1 << 4)) != 0)
                    return IllegalInstruction();	// "class A" 
                mnemonic = (instr & Lbit)!=0 ? Opcode.ldr : Opcode.str;
                format = "3,/";
                if ((instr & Bbit) != 0)
                {
                    width = PrimitiveType.Byte;
                    flagp.Append('B');
                }
                if ((instr & (Wbit + Pbit)) == Wbit)
                {
                    flag |= OpFlags.T;
                    flagp.Append('T');
                }
                poss_tt = eTargetType.target_Data;
                break;
            case 8:
            case 9:
                // STM/LDM 
                mnemonic = (instr & Lbit) != 0 ? Opcode.ldm : Opcode.stm;
                if (RN_is(13))
                {
                    // r13, so treat as stack 
                    int x = (int)(instr & (3 << 23)) >> 22;
                    if ((instr & Lbit) != 0)
                        x ^= 6;
                    flagp.Append("EDEAFDFA".Substring(x, 2));
                }
                else
                {
                    // not r13, so don't treat as stack 
                    flagp.Append(((instr & Ubit) != 0) ? 'I' : 'D');
                    flagp.Append(((instr & Pbit) != 0) ? 'B' : 'A');
                }
                format = "4',%";
                break;
            case 10:
            case 11:
                // B or BL 
                mnemonic = (instr & (1 << 24)) != 0 ? Opcode.bl : Opcode.b;
                format = "&";
                break;
            case 12:
            case 13:
                // STC or LDC 
                if (CP_is(1))
                {
                    // copro 1: FPU. This is STF or LDF. 
                    mnemonic = ((instr & Lbit) >> 18) != 0 ? Opcode.ldf : Opcode.stf;
                    format = "8,/";
                    flag = FpPrecisionOpFlag(fpn);
                    poss_tt = (eTargetType)((int)eTargetType.target_FloatS + fpn);
                }
                else if (CP_is(2))
                {
                    // copro 2: this is LFM or SFM. 
                    mnemonic = ((instr & Lbit) >> 18) != 0 ? Opcode.lfm : Opcode.sfm;
                    if (fpn == 0) fpn = 4;
                    if (RN_is(13) && BitsDiffer(23, 24))
                    {
                        if ((instr & 0xFF) != fpn)
                        {
                            // not r13 or U=P or wrong offset, so don't treat as stack 
                            format = "8,(,/";
                            poss_tt = eTargetType.target_FloatE;
                        }
                        else
                        {
                            // r13 and U!=P, so treat as stack 
                            if (BitsDiffer(20, 24))
                            {
                                // L != P, so FD 
                                flag = OpFlags.FD;
                                flagp.Append('F'); 
                                flagp.Append('D');
                            }
                            else
                            {
                                flag = OpFlags.EA;
                                // L == P, so EA 
                                flagp.Append('E');
                                flagp.Append('A');
                            }
                            format = "8,(,[4]'";
                        }
                    }
                    else
                    {
                        // not r13 or U=P or wrong offset, so don't treat as stack 
                        format = "8,(,/";
                        poss_tt = eTargetType.target_FloatE;
                    }
                }
                else
                {
                    // some other copro number: STC or LDC. 
                    mnemonic = (instr & Lbit) != 0 ? Opcode.ldc : Opcode.stc;
                    format = ";,\004,/";
                    if ((instr & (1 << 22)) != 0)
                    {
                        flag = OpFlags.L;
                        flagp.Append('L');
                    }
                    poss_tt = eTargetType.target_Unknown;
                }
                break;
            case 14:
                // CDP or MRC/MCR 
                if ((instr & (1 << 4)) != 0)
                {
                    // MRC/MCR. 
                    if (CP_is(1))
                    {
                        // copro 1: FPU. 
                        if ((instr & Lbit) != 0 && RD_is(15))
                        {
                            // MCR in FPU with Rd=r15: comparison (ugh) 
                            if ((instr & (1 << 23)) == 0)
                                return IllegalInstruction();	// unused operation 
                            mnemonic = new Opcode[] { Opcode.cmf, Opcode.cnf, Opcode.cmfe, Opcode.cnfe }[(instr & (3 << 21)) >> 21];
                            format = "9,+";
                            if ((instr & ((1 << 19) + (7 << 5))) != 0)
                                result.badbits = true;	// size,rmode reseved 
                        }
                        else
                        {
                            // normal FPU MCR/MRC 
                            word op20 = instr & (15 << 20);
                            if (op20 >= 6 << 20)
                                return IllegalInstruction();
                            mnemonic = new Opcode[] { Opcode.flt, Opcode.fix, Opcode.wfs, Opcode.rfs, Opcode.wfc, Opcode.rfc }[op20 >> 18];
                            if (op20 == 0)
                            {
                                // FLT instruction 
                                format = "9,3";
                                {
                                    flag = FpPrecisionOpFlag(((instr >> 7) & 1) + ((instr >> 18) & 2));
                                    if (flag == OpFlags.P)
                                        return IllegalInstruction();
                                    flag |= FpRoundingMode((instr & (3 << 5)) >> 5);
                                }
                                if ((instr & 15) != 0) result.oddbits = true;	// Fm and const flag unused 
                            }
                            else
                            {
                                // not FLT instruction 
                                if ((instr & ((1 << 7) + (1 << 19))) != 0)
                                    result.badbits = true;	// size bits reserved 
                                if (op20 == 1 << 20)
                                {
                                    // FIX instruction 
                                    format = "3,+";
                                    if ((opts.flags & disopt.FIXS) != 0)
                                    {
                                        flagp.Append("SDEP"[(int)(((instr >> 7) & 1) + ((instr >> 18) & 2))]);
                                    }
                                    flagp.Append("\0PMZ"[(int)((instr & (3 << 5)) >> 5)]);
                                    if ((instr & (7 << 15)) != 0) result.oddbits = true;	// Fn unused 
                                    if ((instr & (1 << 3)) != 0) result.badbits = true;	// no immediate consts 
                                }
                                else
                                {
                                    // neither FLT nor FIX 
                                    format = "3";
                                    if ((instr & (3 << 5)) != 0) result.badbits = true;	// rmode reserved 
                                    if ((instr & (15 + (7 << 15))) != 0) result.oddbits = true;// iFm, Fn unused 
                                }
                            }
                        }
                    }
                    else
                    {
                        // some other copro number. Not FPU. 
                        /* NB that ObjAsm documentation gets MCR and MRC the wrong way round!
                         */
                        mnemonic = new Opcode[] { Opcode.mcr, Opcode.mrc }[(instr & Lbit) >> 18];
                        format = ";,:,3,\005,\001-";
                    }
                }
                else
                {
                    // CDP. 
                    if (CP_is(1))
                    {
                        // copro 1: FPU. 
                        mnemonic = fpuOps[
                            ((instr & (15 << 20)) >> 18)	// opcode   -> bits 5432 
                             + ((instr & (1 << 15)) >> 9)];	// monadicP -> bit 6 
                                                    format = (instr & (1 << 15)) != 0 ? "8,+" : "8,9,+";
                        flagp.Append("SDE*"[(int)(((instr >> 7) & 1) + ((instr >> 18) & 2))]);
                        flagp.Append("\0PMZ"[(int)((instr & (3 << 5)) >> 5)]);
                        // NB that foregoing relies on this being the last flag! 
                        if (mnemonic == Opcode.illegal || flagchars[0] == '*')
                            return IllegalInstruction();
                    }
                    else
                    {
                        // some other copro number. Not FPU. 
                        mnemonic = Opcode.cdp;
                        format = ";,),\004,\005,\001-";
                    }
                }
                break;
            case 15:
                // SWI 
                mnemonic = Opcode.swi;
                format = "$";
                break;
            }

            var op = result.text;

            this.arm = new ArmInstruction();
            arm.Opcode = this.mnemonic;
            arm.Cond = (Condition)(instr >> 28);
            arm.OpFlags = flag;
            var operands = new List<MachineOperand>();
            if (!BuildOperands(format.GetEnumerator(), opts, is_v4, operands, op))
                return IllegalInstruction();
            if (operands.Count > 0)
            {
                arm.Dst = operands[0];
                if (operands.Count > 1)
                {
                    arm.Src1 = operands[1];
                    if (operands.Count > 2)
                    {
                        arm.Src2 = operands[2];
                        if (operands.Count > 3)
                        {
                            arm.Src3 = operands[3];
                        }
                    }
                }
            }
            return arm;
        }

        private  OpFlags FpPrecisionOpFlag(word fpn)
        {
            switch (fpn)
            {
            case 0: width = PrimitiveType.Real32; return OpFlags.S;
            case 1: width = PrimitiveType.Real64; return OpFlags.D;
            case 2: width = PrimitiveType.Real80; return OpFlags.E;
            case 3: width = PrimitiveType.Real32; return OpFlags.P;
            }
            return 0;
        }

        private OpFlags FpRoundingMode(word w)
        {
            switch (w)
            {
            case 0: return 0;
            case 1: return OpFlags.Pl;
            case 2: return OpFlags.Mi;
            case 3: return OpFlags.Zr;
            }
            return 0;
        }
        /// <summary>
        /// Adds operands to the <paramref name="operands"/> list, basing the operand types on the provided format characters
        /// in the <paramref name="f"/> stream.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="opts"></param>
        /// <param name="is_v4"></param>
        /// <param name="operands"></param>
        /// <param name="op"></param>
        /// <returns>Returns false if an invalid operand is discovered, otherwise true.</returns>
        private bool BuildOperands(IEnumerator<char> f, DisOptions opts, int is_v4, List<MachineOperand> operands, StringBuilder op)
        {
            char c;

            string[] regnames = reg_names;
            while (f.MoveNext())
            {
                c = f.Current;
                switch (c)
                {
                case '$':   // Immediate operand.
                    operands.Add(new ImmediateOperand(Constant.Word32(instr & 0x00FFFFFF)));
                    result.is_SWI = true;
                    result.swinum = instr & 0x00FFFFFF;
                    result.addrstart = op.Length;
                    //if ((oflags&disopt.SWInames)!=0) {
                    //  swiname(result.swinum, op, 128-(op-result.text));
                    //  op += strlen(op);
                    //}
                    //else
                    op.AppendFormat("&{0:X}", result.swinum);
                    break;
                case '%':   // Register range
                    operands.Add(new RegisterRangeOperand(instr & 0xFFFF));
                    break;
                case '&':
                    RenderAddress(operands, op);
                    break;
                case '\'':
                    LPling(instr, op);
                    break;
                case '(':
                    op.Append((char)('0' + fpn));
                    break;
                case ')':
                    op.Append((instr >> 20) & 15);
                    break;
                case '[':
                    var nestedOperands = new List<MachineOperand>();
                    BuildOperands(f, opts, is_v4, nestedOperands, op);
                    Debug.Assert(nestedOperands.Count == 1);
                    operands.Add(new ArmMemoryOperand(width, ((RegisterOperand) nestedOperands[0]).Register));
                    break;
                case ']':
                    return true;
                case '*':
                case '.':
                    if (!AddSecondOperand(c, operands, op))
                        return false;
                    break;
                case '+':
                    operands.Add(FpRegisterOrConstant(instr, operands, op));
                    break;
                case ',':
                    break;
                case '-':
                    if (!AddCoprocessorExtraInfo(operands))
                        return false;
                    break;
                case '/':
                    result.addrstart = op.Length;
                    op.Append('[');
                    var rn = GpReg((instr & RNbits()) >> 16);
                    append(op, reg_names[(instr & RNbits()) >> 16]);
                    if ((instr & Pbit) == 0)
                        op.Append(']');
                    // For following, NB that bit 25 is always 0 for LDC, SFM etc 
                    if ((instr & Ibit) != 0)
                    {
                        // shifted offset 
                        if ((instr & Ubit) == 0) 
                            op.Append('-');
                        /* We're going to transfer to '*', basically. The stupid
                         * thing is that the meaning of bit 25 is reversed there;
                         * I don't know why the designers of the ARM did that.
                         */
                        instr ^= Ibit;
                        if ((instr & (1 << 4)) != 0)
                        {
                            if (is_v4 != 0 && (instr & (15 << 8)) == 0)
                            {
                                f = ((instr & Pbit) != 0 ? "0]" : "0").GetEnumerator();
                                break;
                            }
                        }
                        /* Need a ] iff it was pre-indexed; and an optional ! iff
                         * it's pre-indexed *or* a copro instruction,
                         * except that FPU operations don't need the !. Bletch.
                         */
                        if ((instr & Pbit) != 0) 
                        {
                            f = "*]'".GetEnumerator(); 
                        }
                        else if ((instr & (1 << 27)) != 0)
                        {
                            if (CP_is(1) || CP_is(2))
                            {
                                if ((instr & Wbit) == 0)
                                    return false;
                                f = "*".GetEnumerator(); 
                            }
                            else 
                            {
                                f = "*'".GetEnumerator(); 
                            }
                        }
                        else 
                        {
                            f = "*".GetEnumerator();
                        }
                    }
                    else
                    {
                        // immediate offset 
                        word offset;
                        if ((instr & (1 << 27)) != 0)
                        {
                            // LDF or LFM or similar 
                            offset = (instr & 255) << 2;
                        }
                        else if (is_v4 != 0)
                        {
                            offset = (instr & 15) + ((instr & (15 << 8)) >> 4);
                        }
                        else
                        {
                            // LDR or STR 
                            offset = instr & 0xFFF;
                        }
                        op.Append('#');
                        if ((instr & Ubit) == 0)
                        {
                            if (offset != 0) op.Append('-');
                            else result.oddbits = true;
                            result.offset = -(int)offset;
                        }
                        else 
                            result.offset = (int)offset;
                        num(op, offset);
                        if (RN_is(15) && (instr & Pbit) != 0)
                        {
                            // Immediate, pre-indexed and PC-relative. Set target. 
                            result.target_type = poss_tt;
                            result.target = (instr & Ubit) != 0 
                                ? addr + 8 + offset
                                : addr + 8 - offset;
                            if ((instr & Wbit) == 0)
                            {
                                // no writeback, either. Use friendly form. 
                                operands.Add(new AddressOperand(new Address(result.target)));
                                break;
                            }
                        }
                        if ((instr & Pbit) != 0)
                        {
                            op.Append(']');
                            LPling(instr, op);
                        }
                        else if ((instr & (1 << 27)) != 0)
                        {
                            if (CP_is(1) || CP_is(2))
                            {
                                if ((instr & Wbit) == 0)
                                    return false;
                            }
                            else
                                LPling(instr, op);
                        }
                    }
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                    operands.Add(new RegisterOperand(A32Registers.GpRegs[(instr >> (4 * (c - '0'))) & 15]));
                    break;
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    operands.Add(new RegisterOperand(A32Registers.FpRegs[(instr >> (4 * (c - '5'))) & 7]));
                    break;
                case ':':
                    operands.Add(ImmediateOperand.Byte((byte)((instr >> 21) & 7)));
                    break;
                case ';':
                    reg(op, 'p', instr >> 8);
                    break;
                default:
                    if (c <= 5)
                        reg(op, 'c', instr >> (4 * (c - 1)));
                    else
                        op.Append(c);
                    break;
                }
            }
            return true;
        }

        private static RegisterOperand GpReg(uint gpReg)
        {
            return new RegisterOperand(A32Registers.GpRegs[gpReg]);
        }

        private bool AddCoprocessorExtraInfo(List<MachineOperand> operands)
        {
            word w = instr & (7 << 5);
            if (w != 0)
            {
                operands.Add(new ImmediateOperand(Constant.Byte((byte)(w >> 5))));
                return true;
            }
            else
                return false;
        }

        private MachineOperand FpRegisterOrConstant(word instr, List<MachineOperand> operands, StringBuilder op)
        {
            word w = instr & 7;
            if ((instr & (1 << 3)) != 0)
            {
                op.Append('#');
                if (w < 6)
                {
                    op.Append((char)('0' + w));
                    return new ImmediateOperand(Constant.Real64(w));
                }
                else
                {
                    append(op, w == 6 ? "0.5" : "10");
                    return new ImmediateOperand(Constant.Real64(w == 6 ? 0.5 : 10.0));
                }
            }
            else
            {
                op.Append('f');
                op.Append((char)('0' + (instr & 7)));
                return new RegisterOperand(A32Registers.FpRegs[instr & 7]);
            } 
        }

        private static void LPling(word instr, StringBuilder op)
        {
            if ((instr & Wbit) != 0) 
                op.Append('!');
        }

        private void RenderAddress(List<MachineOperand> operands, StringBuilder op)
        {
            uint target = (addr + 8 + ((instr << 8) >> 6)) & 0x03FFFFFCu;
            operands.Add(new AddressOperand(new Address(target)));
            result.addrstart = op.Length;
            hex8(op, target);
            result.target_type = eTargetType.target_Code;
            result.target = target;
        }

        private bool AddSecondOperand(char c, List<MachineOperand> operands, StringBuilder op)
        {
            if ((instr & Ibit) != 0)
            {
                // immediate constant 
                uint imm8 = (instr & 255);
                int rot = (int)((instr >> 7) & 0x1E);
                if (rot != 0 && (imm8 & 3) == 0 && c == '*')
                {
                    // Funny immediate const. Guaranteed not '.', btw 
                    op.AppendFormat("#&{0:X2},", imm8 & 0xFF);
                    operands.Add(new ImmediateOperand(Constant.Word32(imm8 & 0xFF)));
                    return true;
                }
                else
                {
                    imm8 = (imm8 >> rot) | (imm8 << (32 - rot));
                    if (c == '*')
                    {
                        op.Append('#');
                        operands.Add(ImmediateOperand.Word32((int)imm8));
                        return true;
                    }
                    else
                    {
                        address a = addr + 8;
                        if ((instr & (1 << 22)) != 0)
                            a -= imm8;
                        else
                            a += imm8;
                        result.addrstart = op.Length;
                        hex8(op, a);
                        result.target = a; result.target_type = eTargetType.target_Unknown;
                        operands.Add(ImmediateOperand.Word32((int)imm8));
                        return true;
                    }
                }
            }
            else
            {
                // rotated register 
                Opcode rot = new Opcode[] { Opcode.lsl, Opcode.lsr, Opcode.asr, Opcode.ror }[(instr & (3 << 5)) >> 5];
                var operand = new RegisterOperand(A32Registers.GpRegs[instr & 0x0F]);
                append(op, reg_names[instr & 15]);
                if ((instr & (1 << 4)) != 0)
                {
                    // register rotation 
                    if ((instr & (1 << 7)) != 0)
                        return false;

                    // yield operator
                    op.Append(',');
                    append(op, rot.ToString()); op.Append(' ');
                    append(op, reg_names[(instr & (15 << 8)) >> 8]);
                    operands.Add(operand);
                    operands.Add(new ShiftOperand(rot, new RegisterOperand(A32Registers.GpRegs[(instr & (15 << 8)) >> 8])));
                    return true;
                }
                else
                {
                    // constant rotation 
                    word n = instr & (31 << 7);
                    if (n == 0)
                    {
                        if ((instr & (3 << 5)) == 0)
                        {
                            operands.Add(new RegisterOperand(A32Registers.GpRegs[instr & 0x0F]));
                            return true;
                        }
                        else if ((instr & (3 << 5)) == (3 << 5))
                        {
                            operands.Add(operand);
                            append(op, "," + Opcode.rrx);
                            operands.Add(new ShiftOperand(Opcode.rrx, PrimitiveType.Word32));
                            return true;
                        }
                        else
                            n = 32 << 7;
                    }
                    op.Append(','); 
                    append(op, rot.ToString());
                    append(op, " #");
                    num(op, n >> 7);
                    operands.Add(operand);
                    operands.Add(new ShiftOperand(rot, ImmediateOperand.Byte((byte)(n >> 7))));
                    return true;
                }
            }
        }

        private ArmInstruction IllegalInstruction()
        {
            result.text.Append("Undefined instruction");
            result.undefined = true;
            return new ArmInstruction
            {
                Opcode = Opcode.illegal
            };
        }

        static Opcode[] aluOps = new Opcode[] { 
            Opcode.and, Opcode.eor, Opcode.sub, Opcode.rsb, Opcode.add, Opcode.adc, Opcode.sbc, Opcode.rsc,
            Opcode.tst, Opcode.teq, Opcode.cmp, Opcode.cmn, Opcode.orr, Opcode.mov, Opcode.bic, Opcode.mvn
        };

        Opcode[] fpuOps = new Opcode[] 
        { 
            // dyadics: 
            Opcode.adf, Opcode.muf, Opcode.suf,Opcode.rsf,
            Opcode.dvf, Opcode.rdf, Opcode.pow, Opcode.rpw,
            Opcode.rmf, Opcode.fml, Opcode.fdv, Opcode.frd,
            Opcode.pol, Opcode.illegal, Opcode.illegal, Opcode.illegal,
            // monadics: 
            Opcode.mvf, Opcode.mnf, Opcode.abs, Opcode.rnd,
            Opcode.sqt, Opcode.log, Opcode.lgn, Opcode.exp,
            Opcode.sin, Opcode.cos, Opcode.tan, Opcode.asn,
            Opcode.acs, Opcode.atn, Opcode.urd, Opcode.nrm
        };


        static string[] reg_names = new string[] {
              "r0", "r1", "r2", "r3", "r4", "r5", "r6", "r7",
              "r8", "r9", "r10", "r11", "ip", "sp", "lr", "pc"
        };

        void swiname(word w, string s, int sz) { return; }
    }
}

#region OpCode map
/*
http://imrannazar.com/ARM-Opcode-Map
 * <!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
 <head>
  <title>Imran Nazar: ARM Opcode Map</title>
  <meta http-equiv="description" content="There are many places in which you can look if you want an explanation of the instructions available on the ARM series of processor cores. However, there is no overview of the instruction set in the form of a table or map; not even the official ARM instruction reference provides this anywhere in its 811 pages. I produced an opcode map in 2006, in an attempt to rectify this problem: it provides a mapping of all the instructions and addressing modes for cores up to ARM version 4, with version 5 extensions highlighted in blue and the DSP extensions in green.">
  <link href="/css/index.css" type="text/css" rel="stylesheet">
  <link href="/css/content.css" type="text/css" rel="stylesheet">
  <link rel='stylesheet' type='text/css' href='/content/css/opcode-map.css'>
  <link rel="alternate" type="application/rss+xml"  href="/rss.xml" title="ImranNazar.com">
  <style type="text/css">
#head h1 { filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=crop,src='http://imrannazar.com/content/titles/opcode-map.png'); }
#head > h1 { background: url(http://imrannazar.com/content/titles/opcode-map.png) no-repeat top left; }
  </style>
 </head>
 <body>
  <div id="wrapper">
   <div id="head">
    <h1><a href="http://imrannazar.com/">Imran Nazar</a>: ARM Opcode Map</h1>
    <a id="navlink" href="#nav">Skip to navigation</a>
   </div>
   <div id="container">
    <div id="content">
  <p>The following is a full opcode map of instructions for the ARM7 and ARM9 series of CPU cores. Instructions added for ARM9 are highlighted in blue, and instructions specific to the M-extension are shown in green. The Thumb instruction set is also included, in Table 2.</p>
  <div id="opcodemaps">
  <table>
   <caption>Table 1. ARM Opcode Map.</caption>
   <thead>
    <tr>
     <th rowspan='2'>Bits<br>27-20</th>
     <th colspan='16'>Bits 7-4</th>
    </tr>
    <tr>
     <th>0</th><th>1</th><th>2</th><th>3</th>
     <th>4</th><th>5</th><th>6</th><th>7</th>
     <th>8</th><th>9</th><th>A</th><th>B</th>
     <th>C</th><th>D</th><th>E</th><th>F</th>
    </tr>
   </thead>
   <tbody>
<tr>
 <td class='bit'>00</td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical And'>AND</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical And'>AND</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical And'>AND</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical And'>AND</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Multiply registers'>MUL</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical And'>AND</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical And'>AND</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, post-decrement'>ptrm</span></td>
</tr>
<tr>
 <td class='bit'>01</td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Multiply registers, setting flags'>MULS</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
</tr>
<tr>
 <td class='bit'>02</td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Multiply and accumulate registers'>MLA</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, post-decrement'>ptrm</span></td>
</tr>
<tr>
 <td class='bit'>03</td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Multiply and accumulate registers, setting flags'>MLAS</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
</tr>
<tr>
 <td class='bit'>04</td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>05</td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>06</td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>07</td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>08</td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Unsigned long multiply (32x32 to 64)'>UMULL</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, post-increment'>ptrp</span></td>
</tr>
<tr>
 <td class='bit'>09</td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Unsigned long multiply, setting flags'>UMULLS</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, post-increment'>ptrp</span></td>
</tr>
<tr>
 <td class='bit'>0A</td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Unsigned long multiply and accumulate'>UMLAL</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, post-increment'>ptrp</span></td>
</tr>
<tr>
 <td class='bit'>0B</td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Unsigned long multiply and accumulate, setting flags'>UMLALS</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, post-increment'>ptrp</span></td>
</tr>
<tr>
 <td class='bit'>0C</td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Signed long multiply (32x32 to 64)'>SMULL</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>0D</td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Signed long multiply, setting flags'>SMULLS</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>0E</td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Signed long multiply and accumulate'>SMLAL</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>0F</td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Signed long multiply and accumulate, setting flags'>SMLALS</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>10</td>
 <td><span title='Move status word to register'>MRS</span> <span title='Register, CPSR'>rc</span></td>
 <td class='und' colspan='4'></td>
 <td class='edsp'><span title='Saturated add'>QADD</span></td>
 <td class='und' colspan='2'></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with bottom-half of second, and accumulate'>SMLABB</span></td>
 <td><span title='Swap registers with memory word'>SWP</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with bottom-half of second, and accumulate'>SMLATB</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Negative register offset'>ofrm</span></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with top-half of second, and accumulate'>SMLABT</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Negative register offset'>ofrm</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with top-half of second, and accumulate'>SMLATT</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Negative register offset'>ofrm</span></td>
</tr>
<tr>
 <td class='bit'>11</td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Negative register offset'>ofrm</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Negative register offset'>ofrm</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Negative register offset'>ofrm</span></td>
</tr>
<tr>
 <td class='bit'>12</td>
 <td><span title='Move value to status word'>MSR</span> <span title='Register, CPSR'>rc</span></td>
 <td><span title='Branch and switch execution modes'>BX</span></td>
 <td class='und'></td>
 <td class='arm9'><span title='Branch, link and switch execution modes '>BLX</span> <span title='Register offset'>reg</span></td>
 <td class='und'></td>
 <td class='edsp'><span title='Saturated subtract'>QSUB</span></td>
 <td class='und'></td>
 <td class='arm9'><span title='Software breakpoint'>BKPT</span></td>
 <td class='edsp'><span title='Signed multiply first operand with bottom-half of second operand, keeping top 32 bits, and accumulate'>SMLAWB</span></td>
 <td class='und'></td>
 <td class='edsp'><span title='Signed multiply first operand with bottom-half of second operand, keeping top 32 bits'>SMULWB</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, pre-decrement'>prrm</span></td>
 <td class='edsp'><span title='Signed multiply first operand with top-half of second operand, keeping top 32 bits, and accumulate'>SMLAWT</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, pre-decrement'>prrm</span></td>
 <td class='edsp'><span title='Signed multiply first operand with top-half of second operand, keeping top 32 bits'>SMULWT</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, pre-decrement'>prrm</span></td>
</tr>
<tr>
 <td class='bit'>13</td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, pre-decrement'>prrm</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, pre-decrement'>prrm</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, pre-decrement'>prrm</span></td>
</tr>
<tr>
 <td class='bit'>14</td>
 <td><span title='Move status word to register'>MRS</span> <span title='Register, SPSR'>rs</span></td>
 <td class='und' colspan='4'></td>
 <td class='edsp'><span title='Saturated add with doubling of second operand'>QDADD</span></td>
 <td class='und' colspan='2'></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with bottom-half of second, and 64-bit accumulate'>SMLALBB</span></td>
 <td><span title='Swap registers with memory byte'>SWPB</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with bottom-half of second, and 64-bit accumulate'>SMLALTB</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Negative immediate offset'>ofim</span></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with top-half of second, and 64-bit accumulate'>SMLALBT</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Negative immediate offset'>ofim</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with top-half of second, and 64-bit accumulate'>SMLALTT</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>15</td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Negative immediate offset'>ofim</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Negative immediate offset'>ofim</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>16</td>
 <td><span title='Move value to status word'>MSR</span> <span title='Register, SPSR'>rs</span></td>
 <td class='arm9'><span title='Count leading zeros in register'>CLZ</span></td>
 <td class='und' colspan='3'></td>
 <td class='edsp'><span title='Saturated subtract with doubling of second operand'>QDSUB</span></td>
 <td class='und' colspan='2'></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with bottom-half of second'>SMULBB</span></td>
 <td class='und'></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with bottom-half of second'>SMULTB</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with top-half of second'>SMULBT</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with top-half of second'>SMULTT</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>17</td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>18</td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Positive register offset'>ofrp</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Positive register offset'>ofrp</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Positive register offset'>ofrp</span></td>
</tr>
<tr>
 <td class='bit'>19</td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Positive register offset'>ofrp</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Positive register offset'>ofrp</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Positive register offset'>ofrp</span></td>
</tr>
<tr>
 <td class='bit'>1A</td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, pre-increment'>prrp</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, pre-increment'>prrp</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, pre-increment'>prrp</span></td>
</tr>
<tr>
 <td class='bit'>1B</td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, pre-increment'>prrp</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, pre-increment'>prrp</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, pre-increment'>prrp</span></td>
</tr>
<tr>
 <td class='bit'>1C</td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Positive immediate offset'>ofip</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Positive immediate offset'>ofip</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>1D</td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Positive immediate offset'>ofip</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Positive immediate offset'>ofip</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>1E</td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, pre-increment'>prip</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, pre-increment'>prip</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>1F</td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, pre-increment'>prip</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, pre-increment'>prip</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>20</td>
 <td colspan='16'><span title='Logical And'>AND</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>21</td>
 <td colspan='16'><span title='Logical And, setting flags'>ANDS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>22</td>
 <td colspan='16'><span title='Logical Exclusive-or'>EOR</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>23</td>
 <td colspan='16'><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>24</td>
 <td colspan='16'><span title='Subtract from register'>SUB</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>25</td>
 <td colspan='16'><span title='Subtract, setting flags'>SUBS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>26</td>
 <td colspan='16'><span title='Subtract register from value'>RSB</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>27</td>
 <td colspan='16'><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>28</td>
 <td colspan='16'><span title='Add to register'>ADD</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>29</td>
 <td colspan='16'><span title='Add to register, setting flags'>ADDS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2A</td>
 <td colspan='16'><span title='Add to register with carry'>ADC</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2B</td>
 <td colspan='16'><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2C</td>
 <td colspan='16'><span title='Subtract from register with borrow'>SBC</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2D</td>
 <td colspan='16'><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2E</td>
 <td colspan='16'><span title='Subtract register from value with borrow'>RSC</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2F</td>
 <td colspan='16'><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>30</td>
 <td class='und' colspan='16'></td>
</tr>
<tr>
 <td class='bit'>31</td>
 <td colspan='16'><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>32</td>
 <td colspan='16'><span title='Move value to status word'>MSR</span> <span title='Immediate, CPSR'>ic</span></td>
</tr>
<tr>
 <td class='bit'>33</td>
 <td colspan='16'><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>34</td>
 <td class='und' colspan='16'></td>
</tr>
<tr>
 <td class='bit'>35</td>
 <td colspan='16'><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>36</td>
 <td colspan='16'><span title='Move value to status word'>MSR</span> <span title='Immediate, SPSR'>is</span></td>
</tr>
<tr>
 <td class='bit'>37</td>
 <td colspan='16'><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>38</td>
 <td colspan='16'><span title='Logical Or'>ORR</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>39</td>
 <td colspan='16'><span title='Logical Or, setting flags'>ORRS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3A</td>
 <td colspan='16'><span title='Move value to a register'>MOV</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3B</td>
 <td colspan='16'><span title='Move value to a register, setting flags'>MOVS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3C</td>
 <td colspan='16'><span title='Clear bits in register (NAND)'>BIC</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3D</td>
 <td colspan='16'><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3E</td>
 <td colspan='16'><span title='Move negation of value to a register'>MVN</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3F</td>
 <td colspan='16'><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>40</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>41</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>42</td>
 <td colspan='16'><span title='Store word from user-mode register'>STRT</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>43</td>
 <td colspan='16'><span title='Load word into user-mode register'>LDRT</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>44</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>45</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>46</td>
 <td colspan='16'><span title='Store byte from user-mode register'>STRBT</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>47</td>
 <td colspan='16'><span title='Load byte into user-mode register'>LDRBT</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>48</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>49</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4A</td>
 <td colspan='16'><span title='Store word from user-mode register'>STRT</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4B</td>
 <td colspan='16'><span title='Load word into user-mode register'>LDRT</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4C</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4D</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4E</td>
 <td colspan='16'><span title='Store byte from user-mode register'>STRBT</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4F</td>
 <td colspan='16'><span title='Load byte into user-mode register'>LDRBT</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>50</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>51</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>52</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>53</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>54</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>55</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>56</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>57</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>58</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>59</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>5A</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>5B</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>5C</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>5D</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>5E</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>5F</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>60</td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>61</td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>62</td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>63</td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>64</td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>65</td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>66</td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>67</td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>68</td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>69</td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6A</td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6B</td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6C</td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6D</td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6E</td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6F</td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>70</td>
 <td><span title='Store word'>STR</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>71</td>
 <td><span title='Load word'>LDR</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>72</td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>73</td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>74</td>
 <td><span title='Store byte'>STRB</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>75</td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>76</td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>77</td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>78</td>
 <td><span title='Store word'>STR</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>79</td>
 <td><span title='Load word'>LDR</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7A</td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7B</td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7C</td>
 <td><span title='Store byte'>STRB</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7D</td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7E</td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7F</td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>80</td>
 <td colspan='16'><span title='Store multiple words, decrement after'>STMDA</span></td>
</tr>
<tr>
 <td class='bit'>81</td>
 <td colspan='16'><span title='Load multiple words, decrement after'>LDMDA</span></td>
</tr>
<tr>
 <td class='bit'>82</td>
 <td colspan='16'><span title='Store multiple words, decrement after'>STMDA</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>83</td>
 <td colspan='16'><span title='Load multiple words, decrement after'>LDMDA</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>84</td>
 <td colspan='16'><span title='Store multiple words, decrement after'>STMDA</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>85</td>
 <td colspan='16'><span title='Load multiple words, decrement after'>LDMDA</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>86</td>
 <td colspan='16'><span title='Store multiple words, decrement after'>STMDA</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>87</td>
 <td colspan='16'><span title='Load multiple words, decrement after'>LDMDA</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>88</td>
 <td colspan='16'><span title='Store multiple words, increment after'>STMIA</span></td>
</tr>
<tr>
 <td class='bit'>89</td>
 <td colspan='16'><span title='Load multiple words, increment after'>LDMIA</span></td>
</tr>
<tr>
 <td class='bit'>8A</td>
 <td colspan='16'><span title='Store multiple words, increment after'>STMIA</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>8B</td>
 <td colspan='16'><span title='Load multiple words, increment after'>LDMIA</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>8C</td>
 <td colspan='16'><span title='Store multiple words, increment after'>STMIA</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>8D</td>
 <td colspan='16'><span title='Load multiple words, increment after'>LDMIA</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>8E</td>
 <td colspan='16'><span title='Store multiple words, increment after'>STMIA</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>8F</td>
 <td colspan='16'><span title='Load multiple words, increment after'>LDMIA</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>90</td>
 <td colspan='16'><span title='Store multiple words, decrement before'>STMDB</span></td>
</tr>
<tr>
 <td class='bit'>91</td>
 <td colspan='16'><span title='Load multiple words, decrement before'>LDMDB</span></td>
</tr>
<tr>
 <td class='bit'>92</td>
 <td colspan='16'><span title='Store multiple words, decrement before'>STMDB</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>93</td>
 <td colspan='16'><span title='Load multiple words, decrement before'>LDMDB</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>94</td>
 <td colspan='16'><span title='Store multiple words, decrement before'>STMDB</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>95</td>
 <td colspan='16'><span title='Load multiple words, decrement before'>LDMDB</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>96</td>
 <td colspan='16'><span title='Store multiple words, decrement before'>STMDB</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>97</td>
 <td colspan='16'><span title='Load multiple words, decrement before'>LDMDB</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>98</td>
 <td colspan='16'><span title='Store multiple words, increment before'>STMIB</span></td>
</tr>
<tr>
 <td class='bit'>99</td>
 <td colspan='16'><span title='Load multiple words, increment before'>LDMIB</span></td>
</tr>
<tr>
 <td class='bit'>9A</td>
 <td colspan='16'><span title='Store multiple words, increment before'>STMIB</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>9B</td>
 <td colspan='16'><span title='Load multiple words, increment before'>LDMIB</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>9C</td>
 <td colspan='16'><span title='Store multiple words, increment before'>STMIB</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>9D</td>
 <td colspan='16'><span title='Load multiple words, increment before'>LDMIB</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>9E</td>
 <td colspan='16'><span title='Store multiple words, increment before'>STMIB</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>9F</td>
 <td colspan='16'><span title='Load multiple words, increment before'>LDMIB</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>A0</td>
 <td rowspan='16' colspan='16'><span title='Branch'>B</span></td>
</tr>
<tr>
 <td class='bit'>A1</td>
</tr>
<tr>
 <td class='bit'>A2</td>
</tr>
<tr>
 <td class='bit'>A3</td>
</tr>
<tr>
 <td class='bit'>A4</td>
</tr>
<tr>
 <td class='bit'>A5</td>
</tr>
<tr>
 <td class='bit'>A6</td>
</tr>
<tr>
 <td class='bit'>A7</td>
</tr>
<tr>
 <td class='bit'>A8</td>
</tr>
<tr>
 <td class='bit'>A9</td>
</tr>
<tr>
 <td class='bit'>AA</td>
</tr>
<tr>
 <td class='bit'>AB</td>
</tr>
<tr>
 <td class='bit'>AC</td>
</tr>
<tr>
 <td class='bit'>AD</td>
</tr>
<tr>
 <td class='bit'>AE</td>
</tr>
<tr>
 <td class='bit'>AF</td>
</tr>
<tr>
 <td class='bit'>B0</td>
 <td rowspan='16' colspan='16'><span title='Branch and link'>BL</span></td>
</tr>
<tr>
 <td class='bit'>B1</td>
</tr>
<tr>
 <td class='bit'>B2</td>
</tr>
<tr>
 <td class='bit'>B3</td>
</tr>
<tr>
 <td class='bit'>B4</td>
</tr>
<tr>
 <td class='bit'>B5</td>
</tr>
<tr>
 <td class='bit'>B6</td>
</tr>
<tr>
 <td class='bit'>B7</td>
</tr>
<tr>
 <td class='bit'>B8</td>
</tr>
<tr>
 <td class='bit'>B9</td>
</tr>
<tr>
 <td class='bit'>BA</td>
</tr>
<tr>
 <td class='bit'>BB</td>
</tr>
<tr>
 <td class='bit'>BC</td>
</tr>
<tr>
 <td class='bit'>BD</td>
</tr>
<tr>
 <td class='bit'>BE</td>
</tr>
<tr>
 <td class='bit'>BF</td>
</tr>
<tr>
 <td class='bit'>C0</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Negative offset'>ofm</span></td>
</tr>
<tr>
 <td class='bit'>C1</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Negative offset'>ofm</span></td>
</tr>
<tr>
 <td class='bit'>C2</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Pre-decrement'>prm</span></td>
</tr>
<tr>
 <td class='bit'>C3</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Pre-decrement'>prm</span></td>
</tr>
<tr>
 <td class='bit'>C4</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Negative offset'>ofm</span></td>
</tr>
<tr>
 <td class='bit'>C5</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Negative offset'>ofm</span></td>
</tr>
<tr>
 <td class='bit'>C6</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Pre-decrement'>prm</span></td>
</tr>
<tr>
 <td class='bit'>C7</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Pre-decrement'>prm</span></td>
</tr>
<tr>
 <td class='bit'>C8</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Positive offset'>ofp</span></td>
</tr>
<tr>
 <td class='bit'>C9</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Positive offset'>ofp</span></td>
</tr>
<tr>
 <td class='bit'>CA</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Pre-increment'>prp</span></td>
</tr>
<tr>
 <td class='bit'>CB</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Pre-increment'>prp</span></td>
</tr>
<tr>
 <td class='bit'>CC</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Positive offset'>ofp</span></td>
</tr>
<tr>
 <td class='bit'>CD</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Positive offset'>ofp</span></td>
</tr>
<tr>
 <td class='bit'>CE</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Pre-increment'>prp</span></td>
</tr>
<tr>
 <td class='bit'>CF</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Pre-increment'>prp</span></td>
</tr>
<tr>
 <td class='bit'>D0</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Unindexed, bits 7-0 available for copro use'>unm</span></td>
</tr>
<tr>
 <td class='bit'>D1</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Unindexed, bits 7-0 available for copro use'>unm</span></td>
</tr>
<tr>
 <td class='bit'>D2</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Post-decrement'>ptm</span></td>
</tr>
<tr>
 <td class='bit'>D3</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Post-decrement'>ptm</span></td>
</tr>
<tr>
 <td class='bit'>D4</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Unindexed, bits 7-0 available for copro use'>unm</span></td>
</tr>
<tr>
 <td class='bit'>D5</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Unindexed, bits 7-0 available for copro use'>unm</span></td>
</tr>
<tr>
 <td class='bit'>D6</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Post-decrement'>ptm</span></td>
</tr>
<tr>
 <td class='bit'>D7</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Post-decrement'>ptm</span></td>
</tr>
<tr>
 <td class='bit'>D8</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Unindexed, bits 7-0 available for copro use'>unp</span></td>
</tr>
<tr>
 <td class='bit'>D9</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Unindexed, bits 7-0 available for copro use'>unp</span></td>
</tr>
<tr>
 <td class='bit'>DA</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Post-increment'>ptp</span></td>
</tr>
<tr>
 <td class='bit'>DB</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Post-increment'>ptp</span></td>
</tr>
<tr>
 <td class='bit'>DC</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Unindexed, bits 7-0 available for copro use'>unp</span></td>
</tr>
<tr>
 <td class='bit'>DD</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Unindexed, bits 7-0 available for copro use'>unp</span></td>
</tr>
<tr>
 <td class='bit'>DE</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Post-increment'>ptp</span></td>
</tr>
<tr>
 <td class='bit'>DF</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Post-increment'>ptp</span></td>
</tr>
<tr>
 <td class='bit'>E0</td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E1</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>E2</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E3</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>E4</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E5</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>E6</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E7</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>E8</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E9</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>EA</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>EB</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>EC</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>ED</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>EE</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>EF</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>F0</td>
 <td rowspan='16' colspan='16'><span title='Software interrupt (enter supervisor mode)'>SWI</span></td>
</tr>
<tr>
 <td class='bit'>F1</td>
</tr>
<tr>
 <td class='bit'>F2</td>
</tr>
<tr>
 <td class='bit'>F3</td>
</tr>
<tr>
 <td class='bit'>F4</td>
</tr>
<tr>
 <td class='bit'>F5</td>
</tr>
<tr>
 <td class='bit'>F6</td>
</tr>
<tr>
 <td class='bit'>F7</td>
</tr>
<tr>
 <td class='bit'>F8</td>
</tr>
<tr>
 <td class='bit'>F9</td>
</tr>
<tr>
 <td class='bit'>FA</td>
</tr>
<tr>
 <td class='bit'>FB</td>
</tr>
<tr>
 <td class='bit'>FC</td>
</tr>
<tr>
 <td class='bit'>FD</td>
</tr>
<tr>
 <td class='bit'>FE</td>
</tr>
<tr>
 <td class='bit'>FF</td>
</tr>
   </tbody>
  </table>
  <table>
   <caption>Table 2. Thumb Opcode Map.</caption>
   <thead>
    <tr>
     <th rowspan='2'>Bits<br>15-12</th>
     <th colspan='16'>Bits 11-8</th>
    </tr>
    <tr>
     <th>0</th><th>1</th><th>2</th><th>3</th>
     <th>4</th><th>5</th><th>6</th><th>7</th>
     <th>8</th><th>9</th><th>A</th><th>B</th>
     <th>C</th><th>D</th><th>E</th><th>F</th>
    </tr>
   </thead>
   <tbody>
<tr>
 <td class='bit'>0</td>
 <td colspan='8'><span title='Logical shift-left register'>LSL</span> <span title='Immediate value'>imm</span></td>
 <td colspan='8'><span title='Logical shift-right register'>LSR</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>1</td>
 <td colspan='8'><span title='Arithmetic shift-right register'>ASR</span> <span title='Immediate value'>imm</span></td>
 <td colspan='2'><span title='Add to register'>ADD</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Subtract from register'>SUB</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Add to register'>ADD</span> <span title='3-bit immediate offset'>imm3</span></td>
 <td colspan='2'><span title='Subtract from register'>SUB</span> <span title='3-bit immediate offset'>imm3</span></td>
</tr>
<tr>
 <td class='bit'>2</td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r0'>i8r0</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r1'>i8r1</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r2'>i8r2</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r3'>i8r3</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r4'>i8r4</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r5'>i8r5</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r6'>i8r6</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r7'>i8r7</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r0'>i8r0</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r1'>i8r1</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r2'>i8r2</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r3'>i8r3</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r4'>i8r4</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r5'>i8r5</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r6'>i8r6</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r7'>i8r7</span></td>
</tr>
<tr>
 <td class='bit'>3</td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r0'>i8r0</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r1'>i8r1</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r2'>i8r2</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r3'>i8r3</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r4'>i8r4</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r5'>i8r5</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r6'>i8r6</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r7'>i8r7</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r0'>i8r0</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r1'>i8r1</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r2'>i8r2</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r3'>i8r3</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r4'>i8r4</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r5'>i8r5</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r6'>i8r6</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r7'>i8r7</span></td>
</tr>
<tr>
 <td class='bit'>4</td>
 <td><span title='Thumb data processing'>DP</span> <span title='Instruction group 1'>g1</span></td>
 <td><span title='Thumb data processing'>DP</span> <span title='Instruction group 2'>g2</span></td>
 <td><span title='Thumb data processing'>DP</span> <span title='Instruction group 3'>g3</span></td>
 <td><span title='Thumb data processing'>DP</span> <span title='Instruction group 4'>g4</span></td>
 <td><span title='Add registers, select from all 16'>ADDH</span></td>
 <td><span title='Compare registers, select from all 16'>CMPH</span></td>
 <td><span title='Move to a register, select from all 16'>MOVH</span></td>
 <td><span title='Branch and switch execution modes'>BX</span> <span title='Register offset'>reg</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r0'>r0</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r1'>r1</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r2'>r2</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r3'>r3</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r4'>r4</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r5'>r5</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r6'>r6</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r7'>r7</span></td>
</tr>
<tr>
 <td class='bit'>5</td>
 <td colspan='2'><span title='Store word'>STR</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Store halfword'>STRH</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Store byte'>STRB</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load signed byte'>LDRSB</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load word'>LDR</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load halfword'>LDRH</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load byte'>LDRB</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load signed halfword'>LDRSH</span> <span title='Register offset'>reg</span></td>
</tr>
<tr>
 <td class='bit'>6</td>
 <td colspan='8'><span title='Store word'>STR</span> <span title='5-bit immediate offset'>imm5</span></td>
 <td colspan='8'><span title='Load word'>LDR</span> <span title='5-bit immediate offset'>imm5</span></td>
</tr>
<tr>
 <td class='bit'>7</td>
 <td colspan='8'><span title='Store byte'>STRB</span> <span title='5-bit immediate offset'>imm5</span></td>
 <td colspan='8'><span title='Load byte'>LDRB</span> <span title='5-bit immediate offset'>imm5</span></td>
</tr>
<tr>
 <td class='bit'>8</td>
 <td colspan='8'><span title='Store halfword'>STRH</span> <span title='5-bit immediate offset'>imm5</span></td>
 <td colspan='8'><span title='Load halfword'>LDRH</span> <span title='5-bit immediate offset'>imm5</span></td>
</tr>
<tr>
 <td class='bit'>9</td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r0'>r0</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r1'>r1</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r2'>r2</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r3'>r3</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r4'>r4</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r5'>r5</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r6'>r6</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r7'>r7</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r0'>r0</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r1'>r1</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r2'>r2</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r3'>r3</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r4'>r4</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r5'>r5</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r6'>r6</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r7'>r7</span></td>
</tr>
<tr>
 <td class='bit'>A</td>
 <td><span title=''>ADDPC</span> <span title='Using r0'>r0</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r1'>r1</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r2'>r2</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r3'>r3</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r4'>r4</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r5'>r5</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r6'>r6</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r7'>r7</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r0'>r0</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r1'>r1</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r2'>r2</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r3'>r3</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r4'>r4</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r5'>r5</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r6'>r6</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r7'>r7</span></td>
</tr>
<tr>
 <td class='bit'>B</td>
 <td><span title=''>ADDSP</span> <span title='7-bit immediate offset'>imm7</span></td>
 <td class='und' colspan='3'></td>
 <td><span title='Store multiple words to memory (STMDB equivalent)'>PUSH</span></td>
 <td><span title='Store multiple words to memory (STMDB equivalent)'>PUSH</span> <span title='Include r14'>lr</span></td>
 <td class='und' colspan='6'></td>
 <td><span title='Load multiple words from memory (LDMIA equivalent)'>POP</span></td>
 <td><span title='Load multiple words from memory (LDMIA equivalent)'>POP</span> <span title='Include r15'>pc</span></td>
 <td class='arm9'><span title='Software breakpoint'>BKPT</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>C</td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r0'>r0</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r1'>r1</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r2'>r2</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r3'>r3</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r4'>r4</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r5'>r5</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r6'>r6</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r7'>r7</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r0'>r0</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r1'>r1</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r2'>r2</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r3'>r3</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r4'>r4</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r5'>r5</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r6'>r6</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r7'>r7</span></td>
</tr>
<tr>
 <td class='bit'>D</td>
 <td><span title='Branch if zero flag set'>BEQ</span></td>
 <td><span title='Branch if zero flag clear'>BNE</span></td>
 <td><span title='Branch if carry flag set'>BCS</span></td>
 <td><span title='Branch if carry flag clear'>BCC</span></td>
 <td><span title='Branch if negative flag set'>BMI</span></td>
 <td><span title='Branch if negative flag clear'>BPL</span></td>
 <td><span title='Branch if overflow flag set'>BVS</span></td>
 <td><span title='Branch if overflow flag clear'>BVC</span></td>
 <td><span title='Branch if higher (unsigned)'>BHI</span></td>
 <td><span title='Branch if lower or the same (unsigned)'>BLS</span></td>
 <td><span title='Branch if greater than or equal to'>BGE</span></td>
 <td><span title='Branch if less than'>BLT</span></td>
 <td><span title='Branch if greater than'>BGT</span></td>
 <td><span title='Branch if less than or equal to'>BLE</span></td>
 <td class='und'></td>
 <td><span title='Software interrupt (enter supervisor mode)'>SWI</span></td>
</tr>
<tr>
 <td class='bit'>E</td>
 <td colspan='8'><span title='Branch'>B</span></td>
 <td class='arm9' colspan='8'><span title='Branch, link and switch execution modes '>BLX</span> <span title='Two-instruction branch, low 11 bits of offset'>off</span></td>
</tr>
<tr>
 <td class='bit'>F</td>
 <td colspan='8'><span title='Branch and link'>BL</span> <span title='Two-instruction branch, high 11 bits of offset'>setup</span></td>
 <td colspan='8'><span title='Branch and link'>BL</span> <span title='Two-instruction branch, low 11 bits of offset'>off</span></td>
</tr>
   </tbody>
  </table>
  <table>
   <caption>Table 2A. Thumb Opcode Map - Register/Register Data Processing.</caption>
   <thead>
    <tr>
     <th rowspan='2'>Bits<br>9-8</th>
     <th colspan='4'>Bits 7-6</th>
    </tr>
    <tr>
     <th>0</th><th>1</th><th>2</th><th>3</th>
    </tr>
   </thead>
   <tbody>
<tr>
 <td class='bit'>0</td>
 <td><span title='Logical And'>AND</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span></td>
 <td><span title='Logical Left-shift'>LSL</span></td>
 <td><span title='Logical Right-shift'>LSR</span></td>
</tr>
<tr>
 <td class='bit'>1</td>
 <td><span title='Arithmetic Right-shift'>ASR</span></td>
 <td><span title='Add'>ADD</span></td>
 <td><span title='Subtract'>SUB</span></td>
 <td><span title='Rotate right'>ROR</span></td>
</tr>
<tr>
 <td class='bit'>2</td>
 <td><span title='Test Bits (Logical And)'>TST</span></td>
 <td><span title='Negate (Subtract from zero)'>NEG</span></td>
 <td><span title='Compare (Subtract)'>CMP</span></td>
 <td><span title='Compare negative (Add)'>CMN</span></td>
</tr>
<tr>
 <td class='bit'>3</td>
 <td><span title='Logical Or'>ORR</span></td>
 <td><span title='Multiply'>MUL</span></td>
 <td><span title='Bit Clear (NAND)'>BIC</span></td>
 <td><span title='Move negative (NOT)'>MVN</span></td>
</tr>
   </tbody>
  </table>
  </div>
<p><em>Article dated: 4th Oct 2007</em></p>    
    </div>
   </div>
   <div id="foot">
    <div class="inner">
     Operated by Imran Nazar Ltd, registered in the UK (#07698370). Content copyright Imran Nazar, 2005-2011.<br>
     Design and imagery copyright Imran Nazar, 2008-2011; "Parchment" used by license from <a href="http://sxc.hu">sxc</a>.
    </div>
   </div>
   <ul id="nav">
    <li  id="nav_Articles"><a href="/Articles">Articles</a></li>
    <li class="hilite" id="nav_Programming"><a href="/Programming">Programming</a></li>
    <li  id="nav_Fiction"><a href="/Fiction">Fiction</a></li>
    <li  id="nav_Contact"><a href="/Contact">Get in touch</a></li>
   </ul>
   <a href="/rss.xml" id="rss">Get the RSS feed</a>
   <div id="ads">
<script type="text/javascript"><!--


*/
#endregion

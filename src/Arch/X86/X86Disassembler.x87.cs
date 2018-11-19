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
using System;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private bool IsEmulated8087Vector(byte vectorNumber)
        {
            return (0x34 <= vectorNumber && vectorNumber <= 0x3E);
        }

        private X86Instruction RewriteEmulated8087Instruction(byte vectorNumber)
        {
            switch (vectorNumber)
            {
            case 0x34: return Patchx87Instruction(0xD8);
            case 0x35: return Patchx87Instruction(0xD9);
            case 0x36: return Patchx87Instruction(0xDA);
            case 0x37: return Patchx87Instruction(0xDB);
            case 0x38: return Patchx87Instruction(0xDC);
            case 0x39: return Patchx87Instruction(0xDD);
            case 0x3A: return Patchx87Instruction(0xDE);
            case 0x3B: return Patchx87Instruction(0xDF);
            case 0x3C: return Patchx87InstructionSegPrefix();
            case 0x3D: return Patchx87Instruction(0x90);
            case 0x3E: return Emitx87BorlandShortcut();
            }
            throw new InvalidOperationException();
        }

        private X86Instruction Patchx87Instruction(byte op)
        {
            long off = rdr.Offset - 2;
            // On a real 8086, the NOP was a FWAIT, but
            // we violate this so that the resulting 
            // disassembled code is actually legible.
            rdr.Bytes[off] = 0x90;      // NOP
            rdr.Bytes[off + 1] = op;
            rdr.Offset = off;
            return DisassembleInstruction();
        }

        private static byte[] patchx87prefixes = new byte[]{
            0x3E, // DS
            0x36, // SS
            0x2E, // CS
            0x26, // ES
        };  

        private X86Instruction Patchx87InstructionSegPrefix()
        {
            var modifiedEscOp = rdr.Bytes[rdr.Offset];
            long off = rdr.Offset - 2;
            rdr.Bytes[off] = 0x90;      // NOP
            // Segment override is encoded as the top two bits 
            // of modifiedEscOp.
            rdr.Bytes[off + 1] = patchx87prefixes[modifiedEscOp >> 6];
            rdr.Bytes[off + 2] = (byte)(modifiedEscOp | 0xC0);
            rdr.Offset = off;
            return DisassembleInstruction();
        }

        /// <summary>
        /// Borland used INT 3C followed by two bytes to implement
        /// "short cuts". None of these correspond to a real x87
        /// instruction, so we have to simulate them.
        /// </summary>
        private X86Instruction Emitx87BorlandShortcut()
        {
            byte b1 = rdr.Bytes[rdr.Offset];
            rdr.Offset += 2;    // Skip the two trailing bytes.
            switch (b1)
            {
            case 0xDC:
            // load 8086 stack with 8087 registers; overwrites the 10 * N bytes
            // at the top of the stack prior to the INT 3E with the 8087
            // register contents
            case 0xDE:
            // load 8087 registers from top of 8086 stack; ST0 is furthest
            // from  top of 8086 stack
            case 0xE0:
            // round TOS and R1 to single precision, compare, pop twice
            // returns AX = 8087 status word, FLAGS = 8087 condition bits
            case 0xE2:
            // round TOS and R1 to double precision, compare, pop twice
            // returns AX = 8087 status word, FLAGS = 8087 condition bits
            // Note: buggy in TPas5.5, because it sets the 8087 precision
            // control field to the undocumented value 01h; this results in
            // actually rounding to single precision
            case 0xE4:
            //  compare TOS/R1 with two POP's
            //  returns FLAGS = 8087 condition bits
            case 0xE6:
            // compare TOS/R1 with POP
            // returns FLAGS = 8087 condition bits
            case 0xE8:
            // FTST(check TOS value)
            // returns FLAGS = 8087 condition bits
            case 0xEA:
            // FXAM(check TOS value)
            // returns AX = 8087 status word
            case 0xEC:  //  sine(ST0)
            default:
                throw new NotImplementedException();

            //  EEh cosine(ST0)
            //  F0h tangent(ST0)
            //  F2h arctangent(ST0)
            case 0xF4:
                //  F4h ST0 = ln(ST0)
                return new X86Instruction(Opcode.BOR_ln, InstrClass.Linear, dataWidth, addressWidth);
            // F6h    ST0 = log2(ST0)
            //  F8h ST0 = log10(ST0)
            case 0xFA:
                // FAh    ST0 = e** ST0
                return new X86Instruction(Opcode.BOR_exp, InstrClass.Linear, dataWidth, addressWidth);
                //  FCh ST0 = 2 * *ST0
                // FEh    ST0 = 10**ST0
            }
        }

        private static Decoder[] CreateFpuOprecs()
        {
            return new Decoder[]  
			{
				// D8 /////////////////////////

				new InstructionDecoder(Opcode.fadd, "Mf"),
				new InstructionDecoder(Opcode.fmul, "Mf"),
				new InstructionDecoder(Opcode.fcom, "Mf"),
				new InstructionDecoder(Opcode.fcomp, "Mf"),
				new InstructionDecoder(Opcode.fsub,  "Mf"),
				new InstructionDecoder(Opcode.fsubr, "Mf"),
				new InstructionDecoder(Opcode.fdiv,  "Mf"),
				new InstructionDecoder(Opcode.fdivr, "Mf"),
				// D8 C0
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),

				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				// D8 D0		
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
						
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				// D8 E0
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
						
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				// D8 F0
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
						
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				
				// D9 ////////////////////////////////
				
				new InstructionDecoder(Opcode.fld, "Mf"),
				new InstructionDecoder(Opcode.illegal, ""),
				new InstructionDecoder(Opcode.fst, "Mf"),
				new InstructionDecoder(Opcode.fstp, "Mf"),
				new InstructionDecoder(Opcode.fldenv, "Mw"),
				new InstructionDecoder(Opcode.fldcw, "Mw"),
				new InstructionDecoder(Opcode.fstenv, "Mw"),
				new InstructionDecoder(Opcode.fstcw, "Mw"),
						
				// D9 C0
				new InstructionDecoder(Opcode.fld, "F"),
				new InstructionDecoder(Opcode.fld, "F"),
				new InstructionDecoder(Opcode.fld, "F"),
				new InstructionDecoder(Opcode.fld, "F"),
				new InstructionDecoder(Opcode.fld, "F"),
				new InstructionDecoder(Opcode.fld, "F"),
				new InstructionDecoder(Opcode.fld, "F"),
				new InstructionDecoder(Opcode.fld, "F"),
						
				new InstructionDecoder(Opcode.fxch, "f,F"),
				new InstructionDecoder(Opcode.fxch, "f,F"),
				new InstructionDecoder(Opcode.fxch, "f,F"),
				new InstructionDecoder(Opcode.fxch, "f,F"),
				new InstructionDecoder(Opcode.fxch, "f,F"),
				new InstructionDecoder(Opcode.fxch, "f,F"),
				new InstructionDecoder(Opcode.fxch, "f,F"),
				new InstructionDecoder(Opcode.fxch, "f,F"),
						
				// D9 D0
				new InstructionDecoder(Opcode.fnop, ""),
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				// E0
				new InstructionDecoder(Opcode.fchs),
				new InstructionDecoder(Opcode.fabs, ""),
				s_nyi,
				s_nyi,
				new InstructionDecoder(Opcode.ftst),
				new InstructionDecoder(Opcode.fxam),
				s_nyi,
				s_nyi,
						
				new InstructionDecoder(Opcode.fld1),
				new InstructionDecoder(Opcode.fldl2t, ""),
				new InstructionDecoder(Opcode.fldl2e, ""),
				new InstructionDecoder(Opcode.fldpi),
				new InstructionDecoder(Opcode.fldlg2, ""),
				new InstructionDecoder(Opcode.fldln2),
				new InstructionDecoder(Opcode.fldz),
				s_nyi,
						
				// D9 F0
				new InstructionDecoder(Opcode.f2xm1, "F,f"),
				new InstructionDecoder(Opcode.fyl2x, "F,f"),
				new InstructionDecoder(Opcode.fptan, "F,f"),
				new InstructionDecoder(Opcode.fpatan, "F,f"),
				new InstructionDecoder(Opcode.fxtract, "f"),
				new InstructionDecoder(Opcode.fprem1, "F,f"),
				new InstructionDecoder(Opcode.fdecstp, "F,f"),
				new InstructionDecoder(Opcode.fincstp, "F,f"),
						
				new InstructionDecoder(Opcode.fprem, "F,f"),
				new InstructionDecoder(Opcode.fyl2xp1, "F,f"),
				new InstructionDecoder(Opcode.fsqrt),
				new InstructionDecoder(Opcode.fsincos),
				new InstructionDecoder(Opcode.frndint),
				new InstructionDecoder(Opcode.fscale, "F,f"),
				new InstructionDecoder(Opcode.fsin),
				new InstructionDecoder(Opcode.fcos),
				
				// DA //////////////
				
				new InstructionDecoder(Opcode.fiadd, "Md"),
				new InstructionDecoder(Opcode.fimul, "Md"),
				new InstructionDecoder(Opcode.ficom, "Md"),
				new InstructionDecoder(Opcode.ficomp, "Md"),
				new InstructionDecoder(Opcode.fisub, "Md"),
				new InstructionDecoder(Opcode.fisubr, "Md"),
				new InstructionDecoder(Opcode.fidiv, "Md"),
				new InstructionDecoder(Opcode.fidivr, "Md"),
				
				// DA C0 

				new InstructionDecoder(Opcode.fcmovb, "f,F"), 
				new InstructionDecoder(Opcode.fcmovb, "f,F"), 
				new InstructionDecoder(Opcode.fcmovb, "f,F"), 
				new InstructionDecoder(Opcode.fcmovb, "f,F"), 
				new InstructionDecoder(Opcode.fcmovb, "f,F"), 
				new InstructionDecoder(Opcode.fcmovb, "f,F"), 
				new InstructionDecoder(Opcode.fcmovb, "f,F"), 
				new InstructionDecoder(Opcode.fcmovb, "f,F"),

                new InstructionDecoder(Opcode.fcmove, "f,F"),
                new InstructionDecoder(Opcode.fcmove, "f,F"),
                new InstructionDecoder(Opcode.fcmove, "f,F"),
                new InstructionDecoder(Opcode.fcmove, "f,F"),
                new InstructionDecoder(Opcode.fcmove, "f,F"),
                new InstructionDecoder(Opcode.fcmove, "f,F"),
                new InstructionDecoder(Opcode.fcmove, "f,F"),
                new InstructionDecoder(Opcode.fcmove, "f,F"),

                // DA D0
                new InstructionDecoder(Opcode.fcmovbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovbe, "f,F"),

                new InstructionDecoder(Opcode.fcmovu, "f,F"),
                new InstructionDecoder(Opcode.fcmovu, "f,F"),
                new InstructionDecoder(Opcode.fcmovu, "f,F"),
                new InstructionDecoder(Opcode.fcmovu, "f,F"),
                new InstructionDecoder(Opcode.fcmovu, "f,F"),
                new InstructionDecoder(Opcode.fcmovu, "f,F"),
                new InstructionDecoder(Opcode.fcmovu, "f,F"),
                new InstructionDecoder(Opcode.fcmovu, "f,F"),

                // DA E0
                s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 

				s_nyi, 
				new InstructionDecoder(Opcode.fucompp), 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 

				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 

				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 

				// DB ///////////////////////////
				
				new InstructionDecoder(Opcode.fild, "Md"),
				new InstructionDecoder(Opcode.fisttp, "Md"),
				new InstructionDecoder(Opcode.fist, "Md"),
				new InstructionDecoder(Opcode.fistp, "Md"),
				s_nyi,
				new InstructionDecoder(Opcode.fld, "Mh"),
				s_nyi,
				new InstructionDecoder(Opcode.fstp, "Mh"),
						
				// DB C0, Conditional moves.

				new InstructionDecoder(Opcode.fcmovnb, "f,F"),
                new InstructionDecoder(Opcode.fcmovnb, "f,F"),
                new InstructionDecoder(Opcode.fcmovnb, "f,F"),
                new InstructionDecoder(Opcode.fcmovnb, "f,F"),
                new InstructionDecoder(Opcode.fcmovnb, "f,F"),
                new InstructionDecoder(Opcode.fcmovnb, "f,F"),
                new InstructionDecoder(Opcode.fcmovnb, "f,F"),
                new InstructionDecoder(Opcode.fcmovnb, "f,F"),

                new InstructionDecoder(Opcode.fcmovne, "f,F"),
                new InstructionDecoder(Opcode.fcmovne, "f,F"),
                new InstructionDecoder(Opcode.fcmovne, "f,F"),
                new InstructionDecoder(Opcode.fcmovne, "f,F"),
                new InstructionDecoder(Opcode.fcmovne, "f,F"),
                new InstructionDecoder(Opcode.fcmovne, "f,F"),
                new InstructionDecoder(Opcode.fcmovne, "f,F"),
                new InstructionDecoder(Opcode.fcmovne, "f,F"),

                new InstructionDecoder(Opcode.fcmovnbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovnbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovnbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovnbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovnbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovnbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovnbe, "f,F"),
                new InstructionDecoder(Opcode.fcmovnbe, "f,F"),

                new InstructionDecoder(Opcode.fcmovnu, "f,F"),
                new InstructionDecoder(Opcode.fcmovnu, "f,F"),
                new InstructionDecoder(Opcode.fcmovnu, "f,F"),
                new InstructionDecoder(Opcode.fcmovnu, "f,F"),
                new InstructionDecoder(Opcode.fcmovnu, "f,F"),
                new InstructionDecoder(Opcode.fcmovnu, "f,F"),
                new InstructionDecoder(Opcode.fcmovnu, "f,F"),
                new InstructionDecoder(Opcode.fcmovnu, "f,F"),

                // DB E0
				s_nyi, 
				s_nyi, 
				new InstructionDecoder(Opcode.fclex), 
				new InstructionDecoder(Opcode.fninit), 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 

				new InstructionDecoder(Opcode.fucomi, "f,F"), 
				new InstructionDecoder(Opcode.fucomi, "f,F"), 
				new InstructionDecoder(Opcode.fucomi, "f,F"), 
				new InstructionDecoder(Opcode.fucomi, "f,F"), 
				new InstructionDecoder(Opcode.fucomi, "f,F"), 
				new InstructionDecoder(Opcode.fucomi, "f,F"), 
				new InstructionDecoder(Opcode.fucomi, "f,F"), 
				new InstructionDecoder(Opcode.fucomi, "f,F"), 

                // DB F0
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 

				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
				s_nyi, 
					
				// DC ////////////////////

				new InstructionDecoder(Opcode.fadd, "Mg"),
				new InstructionDecoder(Opcode.fmul, "Mg"),
				new InstructionDecoder(Opcode.fcom, "Mg"),
				new InstructionDecoder(Opcode.fcomp, "Mg"),
				new InstructionDecoder(Opcode.fsub, "Mg"),
				new InstructionDecoder(Opcode.fsubr, "Mg"),
				new InstructionDecoder(Opcode.fdiv, "Mg"),
				new InstructionDecoder(Opcode.fdivr, "Mg"),

                // DC C0
						
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
				new InstructionDecoder(Opcode.fadd, "f,F"),
						
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
				new InstructionDecoder(Opcode.fmul, "f,F"),
						
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
				new InstructionDecoder(Opcode.fcom, "f,F"),
						
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
				new InstructionDecoder(Opcode.fcomp, "f,F"),
						
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
				new InstructionDecoder(Opcode.fsub, "f,F"),
						
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
				new InstructionDecoder(Opcode.fsubr, "f,F"),
						
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
				new InstructionDecoder(Opcode.fdiv, "f,F"),
						
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),
				new InstructionDecoder(Opcode.fdivr, "f,F"),

				// DD ////////////////

				new InstructionDecoder(Opcode.fld, "Mg"),
				new InstructionDecoder(Opcode.fisttp, "Mq"),
				new InstructionDecoder(Opcode.fst, "Mg"),
				new InstructionDecoder(Opcode.fstp, "Mg"),
				new InstructionDecoder(Opcode.frstor, "Mw"),
				s_nyi,
				new InstructionDecoder(Opcode.fsave, "Mw"),
				new InstructionDecoder(Opcode.fstsw, "Mw"),
						
				// DD C0

				new InstructionDecoder(Opcode.ffree, "F"),
				new InstructionDecoder(Opcode.ffree, "F"),
				new InstructionDecoder(Opcode.ffree, "F"),
				new InstructionDecoder(Opcode.ffree, "F"),
				new InstructionDecoder(Opcode.ffree, "F"),
				new InstructionDecoder(Opcode.ffree, "F"),
				new InstructionDecoder(Opcode.ffree, "F"),
				new InstructionDecoder(Opcode.ffree, "F"),
						
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// DD D0
				new InstructionDecoder(Opcode.fst, "F"),
				new InstructionDecoder(Opcode.fst, "F"),
				new InstructionDecoder(Opcode.fst, "F"),
				new InstructionDecoder(Opcode.fst, "F"),
				new InstructionDecoder(Opcode.fst, "F"),
				new InstructionDecoder(Opcode.fst, "F"),
				new InstructionDecoder(Opcode.fst, "F"),
				new InstructionDecoder(Opcode.fst, "F"),
						
				new InstructionDecoder(Opcode.fstp, "F"),
				new InstructionDecoder(Opcode.fstp, "F"),
				new InstructionDecoder(Opcode.fstp, "F"),
				new InstructionDecoder(Opcode.fstp, "F"),
				new InstructionDecoder(Opcode.fstp, "F"),
				new InstructionDecoder(Opcode.fstp, "F"),
				new InstructionDecoder(Opcode.fstp, "F"),
				new InstructionDecoder(Opcode.fstp, "F"),
						
				// DD E0
				new InstructionDecoder(Opcode.fucom, "F,f"),
				new InstructionDecoder(Opcode.fucom, "F,f"),
				new InstructionDecoder(Opcode.fucom, "F,f"),
				new InstructionDecoder(Opcode.fucom, "F,f"),
				new InstructionDecoder(Opcode.fucom, "F,f"),
				new InstructionDecoder(Opcode.fucom, "F,f"),
				new InstructionDecoder(Opcode.fucom, "F,f"),
				new InstructionDecoder(Opcode.fucom, "F,f"),
						
				new InstructionDecoder(Opcode.fucomp, "F"),
				new InstructionDecoder(Opcode.fucomp, "F"),
				new InstructionDecoder(Opcode.fucomp, "F"),
				new InstructionDecoder(Opcode.fucomp, "F"),
				new InstructionDecoder(Opcode.fucomp, "F"),
				new InstructionDecoder(Opcode.fucomp, "F"),
				new InstructionDecoder(Opcode.fucomp, "F"),
				new InstructionDecoder(Opcode.fucomp, "F"),
						
				// DD F0
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				// DE //////////////////////////

				new InstructionDecoder(Opcode.fiadd, "Mw"),
				new InstructionDecoder(Opcode.fimul, "Mw"),
				new InstructionDecoder(Opcode.ficom, "Mw"),
				new InstructionDecoder(Opcode.ficomp, "Mw"),
				new InstructionDecoder(Opcode.fisub, "Mw"),
				new InstructionDecoder(Opcode.fisubr, "Mw"),
				new InstructionDecoder(Opcode.fidiv, "Mw"),
				new InstructionDecoder(Opcode.fidivr, "Mw"),
				
                // DE C0
                new InstructionDecoder(Opcode.faddp, "F,f"),
				new InstructionDecoder(Opcode.faddp, "F,f"),
				new InstructionDecoder(Opcode.faddp, "F,f"),
				new InstructionDecoder(Opcode.faddp, "F,f"),
				new InstructionDecoder(Opcode.faddp, "F,f"),
				new InstructionDecoder(Opcode.faddp, "F,f"),
				new InstructionDecoder(Opcode.faddp, "F,f"),
				new InstructionDecoder(Opcode.faddp, "F,f"),
						
				new InstructionDecoder(Opcode.fmulp, "F,f"),
				new InstructionDecoder(Opcode.fmulp, "F,f"),
				new InstructionDecoder(Opcode.fmulp, "F,f"),
				new InstructionDecoder(Opcode.fmulp, "F,f"),
				new InstructionDecoder(Opcode.fmulp, "F,f"),
				new InstructionDecoder(Opcode.fmulp, "F,f"),
				new InstructionDecoder(Opcode.fmulp, "F,f"),
				new InstructionDecoder(Opcode.fmulp, "F,f"),
						
                // DE D0
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				s_nyi,
				new InstructionDecoder(Opcode.fcompp),
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

				// DE E0	
				new InstructionDecoder(Opcode.fsubrp, "F,f"),
				new InstructionDecoder(Opcode.fsubrp, "F,f"),
				new InstructionDecoder(Opcode.fsubrp, "F,f"),
				new InstructionDecoder(Opcode.fsubrp, "F,f"),
				new InstructionDecoder(Opcode.fsubrp, "F,f"),
				new InstructionDecoder(Opcode.fsubrp, "F,f"),
				new InstructionDecoder(Opcode.fsubrp, "F,f"),
				new InstructionDecoder(Opcode.fsubrp, "F,f"),
						
				new InstructionDecoder(Opcode.fsubp, "F,f"),
				new InstructionDecoder(Opcode.fsubp, "F,f"),
				new InstructionDecoder(Opcode.fsubp, "F,f"),
				new InstructionDecoder(Opcode.fsubp, "F,f"),
				new InstructionDecoder(Opcode.fsubp, "F,f"),
				new InstructionDecoder(Opcode.fsubp, "F,f"),
				new InstructionDecoder(Opcode.fsubp, "F,f"),
				new InstructionDecoder(Opcode.fsubp, "F,f"),

				// DE F0
				new InstructionDecoder(Opcode.fdivrp, "F,f"),
				new InstructionDecoder(Opcode.fdivrp, "F,f"),
				new InstructionDecoder(Opcode.fdivrp, "F,f"),
				new InstructionDecoder(Opcode.fdivrp, "F,f"),
				new InstructionDecoder(Opcode.fdivrp, "F,f"),
				new InstructionDecoder(Opcode.fdivrp, "F,f"),
				new InstructionDecoder(Opcode.fdivrp, "F,f"),
				new InstructionDecoder(Opcode.fdivrp, "F,f"),
						
				new InstructionDecoder(Opcode.fdivp, "F,f"),
				new InstructionDecoder(Opcode.fdivp, "F,f"),
				new InstructionDecoder(Opcode.fdivp, "F,f"),
				new InstructionDecoder(Opcode.fdivp, "F,f"),
				new InstructionDecoder(Opcode.fdivp, "F,f"),
				new InstructionDecoder(Opcode.fdivp, "F,f"),
				new InstructionDecoder(Opcode.fdivp, "F,f"),
				new InstructionDecoder(Opcode.fdivp, "F,f"),
				
				// DF //////////////////////

				new InstructionDecoder(Opcode.fild, "Mw"),
				new InstructionDecoder(Opcode.fisttp, "Mw"),
				new InstructionDecoder(Opcode.fist, "Mw"),
				new InstructionDecoder(Opcode.fistp, "Mw"),
				new InstructionDecoder(Opcode.fbld, "MB"),
				new InstructionDecoder(Opcode.fild, "Mq"),
				new InstructionDecoder(Opcode.fbstp, "MB"),
				new InstructionDecoder(Opcode.fistp, "Mq"),

				// DF C0
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				// DF D0
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
						
				// DF E0
				new InstructionDecoder(Opcode.fstsw, "aw"),
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,

                new InstructionDecoder(Opcode.fucomip, "f,F"),
                new InstructionDecoder(Opcode.fucomip, "f,F"),
                new InstructionDecoder(Opcode.fucomip, "f,F"),
                new InstructionDecoder(Opcode.fucomip, "f,F"),
                new InstructionDecoder(Opcode.fucomip, "f,F"),
                new InstructionDecoder(Opcode.fucomip, "f,F"),
                new InstructionDecoder(Opcode.fucomip, "f,F"),
                new InstructionDecoder(Opcode.fucomip, "f,F"),

				// DF F0
                new InstructionDecoder(Opcode.fcomip, "f,F"),
                new InstructionDecoder(Opcode.fcomip, "f,F"),
                new InstructionDecoder(Opcode.fcomip, "f,F"),
                new InstructionDecoder(Opcode.fcomip, "f,F"),
                new InstructionDecoder(Opcode.fcomip, "f,F"),
                new InstructionDecoder(Opcode.fcomip, "f,F"),
                new InstructionDecoder(Opcode.fcomip, "f,F"),
                new InstructionDecoder(Opcode.fcomip, "f,F"),

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
            };
        }
    }
}

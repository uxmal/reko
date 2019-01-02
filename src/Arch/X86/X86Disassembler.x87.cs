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

        private static OpRec[] CreateFpuOprecs()
        {
            return new OpRec[]  
			{
				// D8 /////////////////////////

				new SingleByteOpRec(Opcode.fadd, "Mf"),
				new SingleByteOpRec(Opcode.fmul, "Mf"),
				new SingleByteOpRec(Opcode.fcom, "Mf"),
				new SingleByteOpRec(Opcode.fcomp, "Mf"),
				new SingleByteOpRec(Opcode.fsub,  "Mf"),
				new SingleByteOpRec(Opcode.fsubr, "Mf"),
				new SingleByteOpRec(Opcode.fdiv,  "Mf"),
				new SingleByteOpRec(Opcode.fdivr, "Mf"),
				// D8 C0
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),

				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				// D8 D0		
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
						
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				// D8 E0
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
						
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				// D8 F0
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
						
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				
				// D9 ////////////////////////////////
				
				new SingleByteOpRec(Opcode.fld, "Mf"),
				new SingleByteOpRec(Opcode.illegal, ""),
				new SingleByteOpRec(Opcode.fst, "Mf"),
				new SingleByteOpRec(Opcode.fstp, "Mf"),
				new SingleByteOpRec(Opcode.fldenv, "Mw"),
				new SingleByteOpRec(Opcode.fldcw, "Mw"),
				new SingleByteOpRec(Opcode.fstenv, "Mw"),
				new SingleByteOpRec(Opcode.fstcw, "Mw"),
						
				// D9 C0
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
						
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
						
				// D9 D0
				new SingleByteOpRec(Opcode.fnop, ""),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				// E0
				new SingleByteOpRec(Opcode.fchs),
				new SingleByteOpRec(Opcode.fabs, ""),
				s_invalid,
				s_invalid,
				new SingleByteOpRec(Opcode.ftst),
				new SingleByteOpRec(Opcode.fxam),
				s_invalid,
				s_invalid,
						
				new SingleByteOpRec(Opcode.fld1),
				new SingleByteOpRec(Opcode.fldl2t, ""),
				new SingleByteOpRec(Opcode.fldl2e, ""),
				new SingleByteOpRec(Opcode.fldpi),
				new SingleByteOpRec(Opcode.fldlg2, ""),
				new SingleByteOpRec(Opcode.fldln2),
				new SingleByteOpRec(Opcode.fldz),
				s_invalid,
						
				// D9 F0
				new SingleByteOpRec(Opcode.f2xm1, "F,f"),
				new SingleByteOpRec(Opcode.fyl2x, "F,f"),
				new SingleByteOpRec(Opcode.fptan, "F,f"),
				new SingleByteOpRec(Opcode.fpatan, "F,f"),
				new SingleByteOpRec(Opcode.fxtract, "f"),
				new SingleByteOpRec(Opcode.fprem1, "F,f"),
				new SingleByteOpRec(Opcode.fdecstp, "F,f"),
				new SingleByteOpRec(Opcode.fincstp, "F,f"),
						
				new SingleByteOpRec(Opcode.fprem, "F,f"),
				new SingleByteOpRec(Opcode.fyl2xp1, "F,f"),
				new SingleByteOpRec(Opcode.fsqrt),
				new SingleByteOpRec(Opcode.fsincos),
				new SingleByteOpRec(Opcode.frndint),
				new SingleByteOpRec(Opcode.fscale, "F,f"),
				new SingleByteOpRec(Opcode.fsin),
				new SingleByteOpRec(Opcode.fcos),
				
				// DA //////////////
				
				new SingleByteOpRec(Opcode.fiadd, "Md"),
				new SingleByteOpRec(Opcode.fimul, "Md"),
				new SingleByteOpRec(Opcode.ficom, "Md"),
				new SingleByteOpRec(Opcode.ficomp, "Md"),
				new SingleByteOpRec(Opcode.fisub, "Md"),
				new SingleByteOpRec(Opcode.fisubr, "Md"),
				new SingleByteOpRec(Opcode.fidiv, "Md"),
				new SingleByteOpRec(Opcode.fidivr, "Md"),
				
				// DA C0 

				new SingleByteOpRec(Opcode.fcmovb, "f,F"), 
				new SingleByteOpRec(Opcode.fcmovb, "f,F"), 
				new SingleByteOpRec(Opcode.fcmovb, "f,F"), 
				new SingleByteOpRec(Opcode.fcmovb, "f,F"), 
				new SingleByteOpRec(Opcode.fcmovb, "f,F"), 
				new SingleByteOpRec(Opcode.fcmovb, "f,F"), 
				new SingleByteOpRec(Opcode.fcmovb, "f,F"), 
				new SingleByteOpRec(Opcode.fcmovb, "f,F"),

                new SingleByteOpRec(Opcode.fcmove, "f,F"),
                new SingleByteOpRec(Opcode.fcmove, "f,F"),
                new SingleByteOpRec(Opcode.fcmove, "f,F"),
                new SingleByteOpRec(Opcode.fcmove, "f,F"),
                new SingleByteOpRec(Opcode.fcmove, "f,F"),
                new SingleByteOpRec(Opcode.fcmove, "f,F"),
                new SingleByteOpRec(Opcode.fcmove, "f,F"),
                new SingleByteOpRec(Opcode.fcmove, "f,F"),

                // DA D0
                new SingleByteOpRec(Opcode.fcmovbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovbe, "f,F"),

                new SingleByteOpRec(Opcode.fcmovu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovu, "f,F"),

                // DA E0
                s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 

				s_invalid, 
				new SingleByteOpRec(Opcode.fucompp), 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 

				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 

				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 

				// DB ///////////////////////////
				
				new SingleByteOpRec(Opcode.fild, "Md"),
				new SingleByteOpRec(Opcode.fisttp, "Md"),
				new SingleByteOpRec(Opcode.fist, "Md"),
				new SingleByteOpRec(Opcode.fistp, "Md"),
				s_invalid,
				new SingleByteOpRec(Opcode.fld, "Mh"),
				s_invalid,
				new SingleByteOpRec(Opcode.fstp, "Mh"),
						
				// DB C0, Conditional moves.

				new SingleByteOpRec(Opcode.fcmovnb, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnb, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnb, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnb, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnb, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnb, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnb, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnb, "f,F"),

                new SingleByteOpRec(Opcode.fcmovne, "f,F"),
                new SingleByteOpRec(Opcode.fcmovne, "f,F"),
                new SingleByteOpRec(Opcode.fcmovne, "f,F"),
                new SingleByteOpRec(Opcode.fcmovne, "f,F"),
                new SingleByteOpRec(Opcode.fcmovne, "f,F"),
                new SingleByteOpRec(Opcode.fcmovne, "f,F"),
                new SingleByteOpRec(Opcode.fcmovne, "f,F"),
                new SingleByteOpRec(Opcode.fcmovne, "f,F"),

                new SingleByteOpRec(Opcode.fcmovnbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnbe, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnbe, "f,F"),

                new SingleByteOpRec(Opcode.fcmovnu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnu, "f,F"),
                new SingleByteOpRec(Opcode.fcmovnu, "f,F"),

                // DB E0
				s_invalid, 
				s_invalid, 
				new SingleByteOpRec(Opcode.fclex), 
				new SingleByteOpRec(Opcode.fninit), 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 

				new SingleByteOpRec(Opcode.fucomi, "f,F"), 
				new SingleByteOpRec(Opcode.fucomi, "f,F"), 
				new SingleByteOpRec(Opcode.fucomi, "f,F"), 
				new SingleByteOpRec(Opcode.fucomi, "f,F"), 
				new SingleByteOpRec(Opcode.fucomi, "f,F"), 
				new SingleByteOpRec(Opcode.fucomi, "f,F"), 
				new SingleByteOpRec(Opcode.fucomi, "f,F"), 
				new SingleByteOpRec(Opcode.fucomi, "f,F"), 

                // DB F0
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 

				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
					
				// DC ////////////////////

				new SingleByteOpRec(Opcode.fadd, "Mg"),
				new SingleByteOpRec(Opcode.fmul, "Mg"),
				new SingleByteOpRec(Opcode.fcom, "Mg"),
				new SingleByteOpRec(Opcode.fcomp, "Mg"),
				new SingleByteOpRec(Opcode.fsub, "Mg"),
				new SingleByteOpRec(Opcode.fsubr, "Mg"),
				new SingleByteOpRec(Opcode.fdiv, "Mg"),
				new SingleByteOpRec(Opcode.fdivr, "Mg"),

                // DC C0
						
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
						
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
						
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
				new SingleByteOpRec(Opcode.fcom, "f,F"),
						
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
						
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
						
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
						
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
						
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),
				new SingleByteOpRec(Opcode.fdivr, "f,F"),

				// DD ////////////////

				new SingleByteOpRec(Opcode.fld, "Mg"),
				new SingleByteOpRec(Opcode.fisttp, "Mq"),
				new SingleByteOpRec(Opcode.fst, "Mg"),
				new SingleByteOpRec(Opcode.fstp, "Mg"),
				new SingleByteOpRec(Opcode.frstor, "Mw"),
				s_invalid,
				new SingleByteOpRec(Opcode.fsave, "Mw"),
				new SingleByteOpRec(Opcode.fstsw, "Mw"),
						
				// DD C0

				new SingleByteOpRec(Opcode.ffree, "F"),
				new SingleByteOpRec(Opcode.ffree, "F"),
				new SingleByteOpRec(Opcode.ffree, "F"),
				new SingleByteOpRec(Opcode.ffree, "F"),
				new SingleByteOpRec(Opcode.ffree, "F"),
				new SingleByteOpRec(Opcode.ffree, "F"),
				new SingleByteOpRec(Opcode.ffree, "F"),
				new SingleByteOpRec(Opcode.ffree, "F"),
						
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// DD D0
				new SingleByteOpRec(Opcode.fst, "F"),
				new SingleByteOpRec(Opcode.fst, "F"),
				new SingleByteOpRec(Opcode.fst, "F"),
				new SingleByteOpRec(Opcode.fst, "F"),
				new SingleByteOpRec(Opcode.fst, "F"),
				new SingleByteOpRec(Opcode.fst, "F"),
				new SingleByteOpRec(Opcode.fst, "F"),
				new SingleByteOpRec(Opcode.fst, "F"),
						
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
						
				// DD E0
				new SingleByteOpRec(Opcode.fucom, "F,f"),
				new SingleByteOpRec(Opcode.fucom, "F,f"),
				new SingleByteOpRec(Opcode.fucom, "F,f"),
				new SingleByteOpRec(Opcode.fucom, "F,f"),
				new SingleByteOpRec(Opcode.fucom, "F,f"),
				new SingleByteOpRec(Opcode.fucom, "F,f"),
				new SingleByteOpRec(Opcode.fucom, "F,f"),
				new SingleByteOpRec(Opcode.fucom, "F,f"),
						
				new SingleByteOpRec(Opcode.fucomp, "F"),
				new SingleByteOpRec(Opcode.fucomp, "F"),
				new SingleByteOpRec(Opcode.fucomp, "F"),
				new SingleByteOpRec(Opcode.fucomp, "F"),
				new SingleByteOpRec(Opcode.fucomp, "F"),
				new SingleByteOpRec(Opcode.fucomp, "F"),
				new SingleByteOpRec(Opcode.fucomp, "F"),
				new SingleByteOpRec(Opcode.fucomp, "F"),
						
				// DD F0
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				// DE //////////////////////////

				new SingleByteOpRec(Opcode.fiadd, "Mw"),
				new SingleByteOpRec(Opcode.fimul, "Mw"),
				new SingleByteOpRec(Opcode.ficom, "Mw"),
				new SingleByteOpRec(Opcode.ficomp, "Mw"),
				new SingleByteOpRec(Opcode.fisub, "Mw"),
				new SingleByteOpRec(Opcode.fisubr, "Mw"),
				new SingleByteOpRec(Opcode.fidiv, "Mw"),
				new SingleByteOpRec(Opcode.fidivr, "Mw"),
				
                // DE C0
                new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
						
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
						
                // DE D0
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				s_invalid,
				new SingleByteOpRec(Opcode.fcompp),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// DE E0	
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
						
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),

				// DE F0
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
						
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				
				// DF //////////////////////

				new SingleByteOpRec(Opcode.fild, "Mw"),
				new SingleByteOpRec(Opcode.fisttp, "Mw"),
				new SingleByteOpRec(Opcode.fist, "Mw"),
				new SingleByteOpRec(Opcode.fistp, "Mw"),
				new SingleByteOpRec(Opcode.fbld, "MB"),
				new SingleByteOpRec(Opcode.fild, "Mq"),
				new SingleByteOpRec(Opcode.fbstp, "MB"),
				new SingleByteOpRec(Opcode.fistp, "Mq"),

				// DF C0
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				// DF D0
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
						
				// DF E0
				new SingleByteOpRec(Opcode.fstsw, "aw"),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

                new SingleByteOpRec(Opcode.fucomip, "f,F"),
                new SingleByteOpRec(Opcode.fucomip, "f,F"),
                new SingleByteOpRec(Opcode.fucomip, "f,F"),
                new SingleByteOpRec(Opcode.fucomip, "f,F"),
                new SingleByteOpRec(Opcode.fucomip, "f,F"),
                new SingleByteOpRec(Opcode.fucomip, "f,F"),
                new SingleByteOpRec(Opcode.fucomip, "f,F"),
                new SingleByteOpRec(Opcode.fucomip, "f,F"),

				// DF F0
                new SingleByteOpRec(Opcode.fcomip, "f,F"),
                new SingleByteOpRec(Opcode.fcomip, "f,F"),
                new SingleByteOpRec(Opcode.fcomip, "f,F"),
                new SingleByteOpRec(Opcode.fcomip, "f,F"),
                new SingleByteOpRec(Opcode.fcomip, "f,F"),
                new SingleByteOpRec(Opcode.fcomip, "f,F"),
                new SingleByteOpRec(Opcode.fcomip, "f,F"),
                new SingleByteOpRec(Opcode.fcomip, "f,F"),

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
            };
        }
    }
}

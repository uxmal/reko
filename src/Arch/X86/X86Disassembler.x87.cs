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
            //  EEh cosine(ST0)
            //  F0h tangent(ST0)
            //  F2h arctangent(ST0)
            default:
                return this.NotYetImplemented("Emulated x87");

            case 0xF4:
                //  F4h ST0 = ln(ST0)
                return new X86Instruction(Opcode.BOR_ln, InstrClass.Linear, dataWidth, addressWidth);
            //  F6h ST0 = log2(ST0)
            //  F8h ST0 = log10(ST0)
            case 0xFA:
                // FAh    ST0 = e** ST0
                return new X86Instruction(Opcode.BOR_exp, InstrClass.Linear, dataWidth, addressWidth);
                // FCh    ST0 = 2 * *ST0
                // FEh    ST0 = 10**ST0
            }
        }

        private static Decoder[] CreateFpuOprecs()
        {
            return new Decoder[]  
			{
				// D8 /////////////////////////

				Instr(Opcode.fadd, Mf),
				Instr(Opcode.fmul, Mf),
				Instr(Opcode.fcom, Mf),
				Instr(Opcode.fcomp, Mf),
				Instr(Opcode.fsub,  Mf),
				Instr(Opcode.fsubr, Mf),
				Instr(Opcode.fdiv,  Mf),
				Instr(Opcode.fdivr, Mf),
				// D8 C0
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),

				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				// D8 D0		
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
						
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				// D8 E0
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
						
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				// D8 F0
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
						
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				
				// D9 ////////////////////////////////
				
				Instr(Opcode.fld, Mf),
				s_invalid,
				Instr(Opcode.fst, Mf),
				Instr(Opcode.fstp, Mf),
				Instr(Opcode.fldenv, Mw),
				Instr(Opcode.fldcw, Mw),
				Instr(Opcode.fstenv, Mw),
				Instr(Opcode.fstcw, Mw),
						
				// D9 C0
				Instr(Opcode.fld, F),
				Instr(Opcode.fld, F),
				Instr(Opcode.fld, F),
				Instr(Opcode.fld, F),
				Instr(Opcode.fld, F),
				Instr(Opcode.fld, F),
				Instr(Opcode.fld, F),
				Instr(Opcode.fld, F),
						
				Instr(Opcode.fxch, f,F),
				Instr(Opcode.fxch, f,F),
				Instr(Opcode.fxch, f,F),
				Instr(Opcode.fxch, f,F),
				Instr(Opcode.fxch, f,F),
				Instr(Opcode.fxch, f,F),
				Instr(Opcode.fxch, f,F),
				Instr(Opcode.fxch, f,F),
						
				// D9 D0
				Instr(Opcode.fnop),
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
				Instr(Opcode.fchs),
				Instr(Opcode.fabs),
				s_invalid,
				s_invalid,
				Instr(Opcode.ftst),
				Instr(Opcode.fxam),
				s_invalid,
				s_invalid,
						
				Instr(Opcode.fld1),
				Instr(Opcode.fldl2t),
				Instr(Opcode.fldl2e),
				Instr(Opcode.fldpi),
				Instr(Opcode.fldlg2),
				Instr(Opcode.fldln2),
				Instr(Opcode.fldz),
				s_invalid,
						
				// D9 F0
				Instr(Opcode.f2xm1, F,f),
				Instr(Opcode.fyl2x, F,f),
				Instr(Opcode.fptan, F,f),
				Instr(Opcode.fpatan, F,f),
				Instr(Opcode.fxtract, f),
				Instr(Opcode.fprem1, F,f),
				Instr(Opcode.fdecstp, F,f),
				Instr(Opcode.fincstp, F,f),
						
				Instr(Opcode.fprem, F,f),
				Instr(Opcode.fyl2xp1, F,f),
				Instr(Opcode.fsqrt),
				Instr(Opcode.fsincos),
				Instr(Opcode.frndint),
				Instr(Opcode.fscale, F,f),
				Instr(Opcode.fsin),
				Instr(Opcode.fcos),
				
				// DA //////////////
				
				Instr(Opcode.fiadd, Md),
				Instr(Opcode.fimul, Md),
				Instr(Opcode.ficom, Md),
				Instr(Opcode.ficomp, Md),
				Instr(Opcode.fisub, Md),
				Instr(Opcode.fisubr, Md),
				Instr(Opcode.fidiv, Md),
				Instr(Opcode.fidivr, Md),
				
				// DA C0 

				Instr(Opcode.fcmovb, f,F), 
				Instr(Opcode.fcmovb, f,F), 
				Instr(Opcode.fcmovb, f,F), 
				Instr(Opcode.fcmovb, f,F), 
				Instr(Opcode.fcmovb, f,F), 
				Instr(Opcode.fcmovb, f,F), 
				Instr(Opcode.fcmovb, f,F), 
				Instr(Opcode.fcmovb, f,F),

                Instr(Opcode.fcmove, f,F),
                Instr(Opcode.fcmove, f,F),
                Instr(Opcode.fcmove, f,F),
                Instr(Opcode.fcmove, f,F),
                Instr(Opcode.fcmove, f,F),
                Instr(Opcode.fcmove, f,F),
                Instr(Opcode.fcmove, f,F),
                Instr(Opcode.fcmove, f,F),

                // DA D0
                Instr(Opcode.fcmovbe, f,F),
                Instr(Opcode.fcmovbe, f,F),
                Instr(Opcode.fcmovbe, f,F),
                Instr(Opcode.fcmovbe, f,F),
                Instr(Opcode.fcmovbe, f,F),
                Instr(Opcode.fcmovbe, f,F),
                Instr(Opcode.fcmovbe, f,F),
                Instr(Opcode.fcmovbe, f,F),

                Instr(Opcode.fcmovu, f,F),
                Instr(Opcode.fcmovu, f,F),
                Instr(Opcode.fcmovu, f,F),
                Instr(Opcode.fcmovu, f,F),
                Instr(Opcode.fcmovu, f,F),
                Instr(Opcode.fcmovu, f,F),
                Instr(Opcode.fcmovu, f,F),
                Instr(Opcode.fcmovu, f,F),

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
				Instr(Opcode.fucompp), 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
                // DA F0
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
				
				Instr(Opcode.fild, Md),
				Instr(Opcode.fisttp, Md),
				Instr(Opcode.fist, Md),
				Instr(Opcode.fistp, Md),
				s_invalid,
				Instr(Opcode.fld, Mh),
				s_invalid,
				Instr(Opcode.fstp, Mh),
						
				// DB C0, Conditional moves.

				Instr(Opcode.fcmovnb, f,F),
                Instr(Opcode.fcmovnb, f,F),
                Instr(Opcode.fcmovnb, f,F),
                Instr(Opcode.fcmovnb, f,F),
                Instr(Opcode.fcmovnb, f,F),
                Instr(Opcode.fcmovnb, f,F),
                Instr(Opcode.fcmovnb, f,F),
                Instr(Opcode.fcmovnb, f,F),

                Instr(Opcode.fcmovne, f,F),
                Instr(Opcode.fcmovne, f,F),
                Instr(Opcode.fcmovne, f,F),
                Instr(Opcode.fcmovne, f,F),
                Instr(Opcode.fcmovne, f,F),
                Instr(Opcode.fcmovne, f,F),
                Instr(Opcode.fcmovne, f,F),
                Instr(Opcode.fcmovne, f,F),

                Instr(Opcode.fcmovnbe, f,F),
                Instr(Opcode.fcmovnbe, f,F),
                Instr(Opcode.fcmovnbe, f,F),
                Instr(Opcode.fcmovnbe, f,F),
                Instr(Opcode.fcmovnbe, f,F),
                Instr(Opcode.fcmovnbe, f,F),
                Instr(Opcode.fcmovnbe, f,F),
                Instr(Opcode.fcmovnbe, f,F),

                Instr(Opcode.fcmovnu, f,F),
                Instr(Opcode.fcmovnu, f,F),
                Instr(Opcode.fcmovnu, f,F),
                Instr(Opcode.fcmovnu, f,F),
                Instr(Opcode.fcmovnu, f,F),
                Instr(Opcode.fcmovnu, f,F),
                Instr(Opcode.fcmovnu, f,F),
                Instr(Opcode.fcmovnu, f,F),

                // DB E0
                Instr(Opcode.fneni),
                Instr(Opcode.fndisi),
                Instr(Opcode.fclex), 
				Instr(Opcode.fninit),

                Instr(Opcode.fnsetpm),
                Instr(Opcode.frstpm), 
				s_invalid, 
				s_invalid, 

				Instr(Opcode.fucomi, f,F), 
				Instr(Opcode.fucomi, f,F), 
				Instr(Opcode.fucomi, f,F), 
				Instr(Opcode.fucomi, f,F), 
				Instr(Opcode.fucomi, f,F), 
				Instr(Opcode.fucomi, f,F), 
				Instr(Opcode.fucomi, f,F), 
				Instr(Opcode.fucomi, f,F), 

                // DB F0
				Instr(Opcode.fcomi, f,F),
                Instr(Opcode.fcomi, f,F),
                Instr(Opcode.fcomi, f,F),
                Instr(Opcode.fcomi, f,F),
                Instr(Opcode.fcomi, f,F),
                Instr(Opcode.fcomi, f,F),
                Instr(Opcode.fcomi, f,F),
                Instr(Opcode.fcomi, f,F),

                s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
					
				// DC ////////////////////

				Instr(Opcode.fadd, Mg),
				Instr(Opcode.fmul, Mg),
				Instr(Opcode.fcom, Mg),
				Instr(Opcode.fcomp, Mg),
				Instr(Opcode.fsub, Mg),
				Instr(Opcode.fsubr, Mg),
				Instr(Opcode.fdiv, Mg),
				Instr(Opcode.fdivr, Mg),

                // DC C0
						
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
				Instr(Opcode.fadd, f,F),
						
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
				Instr(Opcode.fmul, f,F),
						
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
				Instr(Opcode.fcom, f,F),
						
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
				Instr(Opcode.fcomp, f,F),
						
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
				Instr(Opcode.fsub, f,F),
						
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
				Instr(Opcode.fsubr, f,F),
						
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
				Instr(Opcode.fdiv, f,F),
						
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),
				Instr(Opcode.fdivr, f,F),

				// DD ////////////////

				Instr(Opcode.fld, Mg),
				Instr(Opcode.fisttp, Mq),
				Instr(Opcode.fst, Mg),
				Instr(Opcode.fstp, Mg),
				Instr(Opcode.frstor, Mw),
				s_invalid,
				Instr(Opcode.fsave, Mw),
				Instr(Opcode.fstsw, Mw),
						
				// DD C0

				Instr(Opcode.ffree, F),
				Instr(Opcode.ffree, F),
				Instr(Opcode.ffree, F),
				Instr(Opcode.ffree, F),
				Instr(Opcode.ffree, F),
				Instr(Opcode.ffree, F),
				Instr(Opcode.ffree, F),
				Instr(Opcode.ffree, F),
						
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// DD D0
				Instr(Opcode.fst, F),
				Instr(Opcode.fst, F),
				Instr(Opcode.fst, F),
				Instr(Opcode.fst, F),
				Instr(Opcode.fst, F),
				Instr(Opcode.fst, F),
				Instr(Opcode.fst, F),
				Instr(Opcode.fst, F),
						
				Instr(Opcode.fstp, F),
				Instr(Opcode.fstp, F),
				Instr(Opcode.fstp, F),
				Instr(Opcode.fstp, F),
				Instr(Opcode.fstp, F),
				Instr(Opcode.fstp, F),
				Instr(Opcode.fstp, F),
				Instr(Opcode.fstp, F),
						
				// DD E0
				Instr(Opcode.fucom, F,f),
				Instr(Opcode.fucom, F,f),
				Instr(Opcode.fucom, F,f),
				Instr(Opcode.fucom, F,f),
				Instr(Opcode.fucom, F,f),
				Instr(Opcode.fucom, F,f),
				Instr(Opcode.fucom, F,f),
				Instr(Opcode.fucom, F,f),
						
				Instr(Opcode.fucomp, F),
				Instr(Opcode.fucomp, F),
				Instr(Opcode.fucomp, F),
				Instr(Opcode.fucomp, F),
				Instr(Opcode.fucomp, F),
				Instr(Opcode.fucomp, F),
				Instr(Opcode.fucomp, F),
				Instr(Opcode.fucomp, F),
						
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

				Instr(Opcode.fiadd, Mw),
				Instr(Opcode.fimul, Mw),
				Instr(Opcode.ficom, Mw),
				Instr(Opcode.ficomp, Mw),
				Instr(Opcode.fisub, Mw),
				Instr(Opcode.fisubr, Mw),
				Instr(Opcode.fidiv, Mw),
				Instr(Opcode.fidivr, Mw),
				
                // DE C0
                Instr(Opcode.faddp, F,f),
				Instr(Opcode.faddp, F,f),
				Instr(Opcode.faddp, F,f),
				Instr(Opcode.faddp, F,f),
				Instr(Opcode.faddp, F,f),
				Instr(Opcode.faddp, F,f),
				Instr(Opcode.faddp, F,f),
				Instr(Opcode.faddp, F,f),
						
				Instr(Opcode.fmulp, F,f),
				Instr(Opcode.fmulp, F,f),
				Instr(Opcode.fmulp, F,f),
				Instr(Opcode.fmulp, F,f),
				Instr(Opcode.fmulp, F,f),
				Instr(Opcode.fmulp, F,f),
				Instr(Opcode.fmulp, F,f),
				Instr(Opcode.fmulp, F,f),
						
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
				Instr(Opcode.fcompp),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// DE E0	
				Instr(Opcode.fsubrp, F,f),
				Instr(Opcode.fsubrp, F,f),
				Instr(Opcode.fsubrp, F,f),
				Instr(Opcode.fsubrp, F,f),
				Instr(Opcode.fsubrp, F,f),
				Instr(Opcode.fsubrp, F,f),
				Instr(Opcode.fsubrp, F,f),
				Instr(Opcode.fsubrp, F,f),
						
				Instr(Opcode.fsubp, F,f),
				Instr(Opcode.fsubp, F,f),
				Instr(Opcode.fsubp, F,f),
				Instr(Opcode.fsubp, F,f),
				Instr(Opcode.fsubp, F,f),
				Instr(Opcode.fsubp, F,f),
				Instr(Opcode.fsubp, F,f),
				Instr(Opcode.fsubp, F,f),

				// DE F0
				Instr(Opcode.fdivrp, F,f),
				Instr(Opcode.fdivrp, F,f),
				Instr(Opcode.fdivrp, F,f),
				Instr(Opcode.fdivrp, F,f),
				Instr(Opcode.fdivrp, F,f),
				Instr(Opcode.fdivrp, F,f),
				Instr(Opcode.fdivrp, F,f),
				Instr(Opcode.fdivrp, F,f),
						
				Instr(Opcode.fdivp, F,f),
				Instr(Opcode.fdivp, F,f),
				Instr(Opcode.fdivp, F,f),
				Instr(Opcode.fdivp, F,f),
				Instr(Opcode.fdivp, F,f),
				Instr(Opcode.fdivp, F,f),
				Instr(Opcode.fdivp, F,f),
				Instr(Opcode.fdivp, F,f),
				
				// DF //////////////////////

				Instr(Opcode.fild, Mw),
				Instr(Opcode.fisttp, Mw),
				Instr(Opcode.fist, Mw),
				Instr(Opcode.fistp, Mw),
				Instr(Opcode.fbld, MB),
				Instr(Opcode.fild, Mq),
				Instr(Opcode.fbstp, MB),
				Instr(Opcode.fistp, Mq),

				// DF C0
				Instr(Opcode.ffreep, F),
				Instr(Opcode.ffreep, F),
				Instr(Opcode.ffreep, F),
				Instr(Opcode.ffreep, F),
				Instr(Opcode.ffreep, F),
				Instr(Opcode.ffreep, F),
				Instr(Opcode.ffreep, F),
				Instr(Opcode.ffreep, F),
						
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
				Instr(Opcode.fstsw, aw),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

                Instr(Opcode.fucomip, f,F),
                Instr(Opcode.fucomip, f,F),
                Instr(Opcode.fucomip, f,F),
                Instr(Opcode.fucomip, f,F),
                Instr(Opcode.fucomip, f,F),
                Instr(Opcode.fucomip, f,F),
                Instr(Opcode.fucomip, f,F),
                Instr(Opcode.fucomip, f,F),

				// DF F0
                Instr(Opcode.fcomip, f,F),
                Instr(Opcode.fcomip, f,F),
                Instr(Opcode.fcomip, f,F),
                Instr(Opcode.fcomip, f,F),
                Instr(Opcode.fcomip, f,F),
                Instr(Opcode.fcomip, f,F),
                Instr(Opcode.fcomip, f,F),
                Instr(Opcode.fcomip, f,F),

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

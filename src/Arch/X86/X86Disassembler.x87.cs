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
            //$TODO: check for nulls.
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
                this.NotYetImplemented("Emulated x87");
                return null;
            case 0xF4:
                //  F4h ST0 = ln(ST0)
                return new X86Instruction(Mnemonic.BOR_ln, InstrClass.Linear, this.decodingContext.dataWidth, this.decodingContext.addressWidth);
            //  F6h ST0 = log2(ST0)
            //  F8h ST0 = log10(ST0)
            case 0xFA:
                // FAh    ST0 = e** ST0
                return new X86Instruction(Mnemonic.BOR_exp, InstrClass.Linear, this.decodingContext.dataWidth, this.decodingContext.addressWidth);
                // FCh    ST0 = 2 * *ST0
                // FEh    ST0 = 10**ST0
            }
        }

        private static Decoder[] CreateFpuDecoders()
        {
            return new Decoder[]  
			{
				// D8 /////////////////////////

				Instr(Mnemonic.fadd, Mf),
				Instr(Mnemonic.fmul, Mf),
				Instr(Mnemonic.fcom, Mf),
				Instr(Mnemonic.fcomp, Mf),
				Instr(Mnemonic.fsub,  Mf),
				Instr(Mnemonic.fsubr, Mf),
				Instr(Mnemonic.fdiv,  Mf),
				Instr(Mnemonic.fdivr, Mf),
				// D8 C0
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),

				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				// D8 D0		
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
						
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				// D8 E0
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
						
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				// D8 F0
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
						
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				
				// D9 ////////////////////////////////
				
				Instr(Mnemonic.fld, Mf),
				s_invalid,
				Instr(Mnemonic.fst, Mf),
				Instr(Mnemonic.fstp, Mf),
				Instr(Mnemonic.fldenv, Mw),
				Instr(Mnemonic.fldcw, Mw),
				Instr(Mnemonic.fstenv, Mw),
				Instr(Mnemonic.fstcw, Mw),
						
				// D9 C0
				Instr(Mnemonic.fld, F),
				Instr(Mnemonic.fld, F),
				Instr(Mnemonic.fld, F),
				Instr(Mnemonic.fld, F),
				Instr(Mnemonic.fld, F),
				Instr(Mnemonic.fld, F),
				Instr(Mnemonic.fld, F),
				Instr(Mnemonic.fld, F),
						
				Instr(Mnemonic.fxch, f,F),
				Instr(Mnemonic.fxch, f,F),
				Instr(Mnemonic.fxch, f,F),
				Instr(Mnemonic.fxch, f,F),
				Instr(Mnemonic.fxch, f,F),
				Instr(Mnemonic.fxch, f,F),
				Instr(Mnemonic.fxch, f,F),
				Instr(Mnemonic.fxch, f,F),
						
				// D9 D0
				Instr(Mnemonic.fnop),
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
				Instr(Mnemonic.fchs),
				Instr(Mnemonic.fabs),
				s_invalid,
				s_invalid,
				Instr(Mnemonic.ftst),
				Instr(Mnemonic.fxam),
				s_invalid,
				s_invalid,
						
				Instr(Mnemonic.fld1),
				Instr(Mnemonic.fldl2t),
				Instr(Mnemonic.fldl2e),
				Instr(Mnemonic.fldpi),
				Instr(Mnemonic.fldlg2),
				Instr(Mnemonic.fldln2),
				Instr(Mnemonic.fldz),
				s_invalid,
						
				// D9 F0
				Instr(Mnemonic.f2xm1, F,f),
				Instr(Mnemonic.fyl2x, F,f),
				Instr(Mnemonic.fptan, F,f),
				Instr(Mnemonic.fpatan, F,f),
				Instr(Mnemonic.fxtract, f),
				Instr(Mnemonic.fprem1, F,f),
				Instr(Mnemonic.fdecstp, F,f),
				Instr(Mnemonic.fincstp, F,f),
						
				Instr(Mnemonic.fprem, F,f),
				Instr(Mnemonic.fyl2xp1, F,f),
				Instr(Mnemonic.fsqrt),
				Instr(Mnemonic.fsincos),
				Instr(Mnemonic.frndint),
				Instr(Mnemonic.fscale, F,f),
				Instr(Mnemonic.fsin),
				Instr(Mnemonic.fcos),
				
				// DA //////////////
				
				Instr(Mnemonic.fiadd, Md),
				Instr(Mnemonic.fimul, Md),
				Instr(Mnemonic.ficom, Md),
				Instr(Mnemonic.ficomp, Md),
				Instr(Mnemonic.fisub, Md),
				Instr(Mnemonic.fisubr, Md),
				Instr(Mnemonic.fidiv, Md),
				Instr(Mnemonic.fidivr, Md),
				
				// DA C0 

				Instr(Mnemonic.fcmovb, f,F), 
				Instr(Mnemonic.fcmovb, f,F), 
				Instr(Mnemonic.fcmovb, f,F), 
				Instr(Mnemonic.fcmovb, f,F), 
				Instr(Mnemonic.fcmovb, f,F), 
				Instr(Mnemonic.fcmovb, f,F), 
				Instr(Mnemonic.fcmovb, f,F), 
				Instr(Mnemonic.fcmovb, f,F),

                Instr(Mnemonic.fcmove, f,F),
                Instr(Mnemonic.fcmove, f,F),
                Instr(Mnemonic.fcmove, f,F),
                Instr(Mnemonic.fcmove, f,F),
                Instr(Mnemonic.fcmove, f,F),
                Instr(Mnemonic.fcmove, f,F),
                Instr(Mnemonic.fcmove, f,F),
                Instr(Mnemonic.fcmove, f,F),

                // DA D0
                Instr(Mnemonic.fcmovbe, f,F),
                Instr(Mnemonic.fcmovbe, f,F),
                Instr(Mnemonic.fcmovbe, f,F),
                Instr(Mnemonic.fcmovbe, f,F),
                Instr(Mnemonic.fcmovbe, f,F),
                Instr(Mnemonic.fcmovbe, f,F),
                Instr(Mnemonic.fcmovbe, f,F),
                Instr(Mnemonic.fcmovbe, f,F),

                Instr(Mnemonic.fcmovu, f,F),
                Instr(Mnemonic.fcmovu, f,F),
                Instr(Mnemonic.fcmovu, f,F),
                Instr(Mnemonic.fcmovu, f,F),
                Instr(Mnemonic.fcmovu, f,F),
                Instr(Mnemonic.fcmovu, f,F),
                Instr(Mnemonic.fcmovu, f,F),
                Instr(Mnemonic.fcmovu, f,F),

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
				Instr(Mnemonic.fucompp), 
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
				
				Instr(Mnemonic.fild, Md),
				Instr(Mnemonic.fisttp, Md),
				Instr(Mnemonic.fist, Md),
				Instr(Mnemonic.fistp, Md),
				s_invalid,
				Instr(Mnemonic.fld, Mh),
				s_invalid,
				Instr(Mnemonic.fstp, Mh),
						
				// DB C0, Conditional moves.

				Instr(Mnemonic.fcmovnb, f,F),
                Instr(Mnemonic.fcmovnb, f,F),
                Instr(Mnemonic.fcmovnb, f,F),
                Instr(Mnemonic.fcmovnb, f,F),
                Instr(Mnemonic.fcmovnb, f,F),
                Instr(Mnemonic.fcmovnb, f,F),
                Instr(Mnemonic.fcmovnb, f,F),
                Instr(Mnemonic.fcmovnb, f,F),

                Instr(Mnemonic.fcmovne, f,F),
                Instr(Mnemonic.fcmovne, f,F),
                Instr(Mnemonic.fcmovne, f,F),
                Instr(Mnemonic.fcmovne, f,F),
                Instr(Mnemonic.fcmovne, f,F),
                Instr(Mnemonic.fcmovne, f,F),
                Instr(Mnemonic.fcmovne, f,F),
                Instr(Mnemonic.fcmovne, f,F),

                Instr(Mnemonic.fcmovnbe, f,F),
                Instr(Mnemonic.fcmovnbe, f,F),
                Instr(Mnemonic.fcmovnbe, f,F),
                Instr(Mnemonic.fcmovnbe, f,F),
                Instr(Mnemonic.fcmovnbe, f,F),
                Instr(Mnemonic.fcmovnbe, f,F),
                Instr(Mnemonic.fcmovnbe, f,F),
                Instr(Mnemonic.fcmovnbe, f,F),

                Instr(Mnemonic.fcmovnu, f,F),
                Instr(Mnemonic.fcmovnu, f,F),
                Instr(Mnemonic.fcmovnu, f,F),
                Instr(Mnemonic.fcmovnu, f,F),
                Instr(Mnemonic.fcmovnu, f,F),
                Instr(Mnemonic.fcmovnu, f,F),
                Instr(Mnemonic.fcmovnu, f,F),
                Instr(Mnemonic.fcmovnu, f,F),

                // DB E0
                Instr(Mnemonic.fneni),
                Instr(Mnemonic.fndisi),
                Instr(Mnemonic.fclex), 
				Instr(Mnemonic.fninit),

                Instr(Mnemonic.fnsetpm),
                Instr(Mnemonic.frstpm), 
				s_invalid, 
				s_invalid, 

				Instr(Mnemonic.fucomi, f,F), 
				Instr(Mnemonic.fucomi, f,F), 
				Instr(Mnemonic.fucomi, f,F), 
				Instr(Mnemonic.fucomi, f,F), 
				Instr(Mnemonic.fucomi, f,F), 
				Instr(Mnemonic.fucomi, f,F), 
				Instr(Mnemonic.fucomi, f,F), 
				Instr(Mnemonic.fucomi, f,F), 

                // DB F0
				Instr(Mnemonic.fcomi, f,F),
                Instr(Mnemonic.fcomi, f,F),
                Instr(Mnemonic.fcomi, f,F),
                Instr(Mnemonic.fcomi, f,F),
                Instr(Mnemonic.fcomi, f,F),
                Instr(Mnemonic.fcomi, f,F),
                Instr(Mnemonic.fcomi, f,F),
                Instr(Mnemonic.fcomi, f,F),

                s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
				s_invalid, 
					
				// DC ////////////////////

				Instr(Mnemonic.fadd, Mg),
				Instr(Mnemonic.fmul, Mg),
				Instr(Mnemonic.fcom, Mg),
				Instr(Mnemonic.fcomp, Mg),
				Instr(Mnemonic.fsub, Mg),
				Instr(Mnemonic.fsubr, Mg),
				Instr(Mnemonic.fdiv, Mg),
				Instr(Mnemonic.fdivr, Mg),

                // DC C0
						
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
				Instr(Mnemonic.fadd, f,F),
						
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
				Instr(Mnemonic.fmul, f,F),
						
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
				Instr(Mnemonic.fcom, f,F),
						
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
				Instr(Mnemonic.fcomp, f,F),
						
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
				Instr(Mnemonic.fsub, f,F),
						
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
				Instr(Mnemonic.fsubr, f,F),
						
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
				Instr(Mnemonic.fdiv, f,F),
						
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),
				Instr(Mnemonic.fdivr, f,F),

				// DD ////////////////

				Instr(Mnemonic.fld, Mg),
				Instr(Mnemonic.fisttp, Mq),
				Instr(Mnemonic.fst, Mg),
				Instr(Mnemonic.fstp, Mg),
				Instr(Mnemonic.frstor, Mw),
				s_invalid,
				Instr(Mnemonic.fsave, Mw),
				Instr(Mnemonic.fstsw, Mw),
						
				// DD C0

				Instr(Mnemonic.ffree, F),
				Instr(Mnemonic.ffree, F),
				Instr(Mnemonic.ffree, F),
				Instr(Mnemonic.ffree, F),
				Instr(Mnemonic.ffree, F),
				Instr(Mnemonic.ffree, F),
				Instr(Mnemonic.ffree, F),
				Instr(Mnemonic.ffree, F),
						
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// DD D0
				Instr(Mnemonic.fst, F),
				Instr(Mnemonic.fst, F),
				Instr(Mnemonic.fst, F),
				Instr(Mnemonic.fst, F),
				Instr(Mnemonic.fst, F),
				Instr(Mnemonic.fst, F),
				Instr(Mnemonic.fst, F),
				Instr(Mnemonic.fst, F),
						
				Instr(Mnemonic.fstp, F),
				Instr(Mnemonic.fstp, F),
				Instr(Mnemonic.fstp, F),
				Instr(Mnemonic.fstp, F),
				Instr(Mnemonic.fstp, F),
				Instr(Mnemonic.fstp, F),
				Instr(Mnemonic.fstp, F),
				Instr(Mnemonic.fstp, F),
						
				// DD E0
				Instr(Mnemonic.fucom, F,f),
				Instr(Mnemonic.fucom, F,f),
				Instr(Mnemonic.fucom, F,f),
				Instr(Mnemonic.fucom, F,f),
				Instr(Mnemonic.fucom, F,f),
				Instr(Mnemonic.fucom, F,f),
				Instr(Mnemonic.fucom, F,f),
				Instr(Mnemonic.fucom, F,f),
						
				Instr(Mnemonic.fucomp, F),
				Instr(Mnemonic.fucomp, F),
				Instr(Mnemonic.fucomp, F),
				Instr(Mnemonic.fucomp, F),
				Instr(Mnemonic.fucomp, F),
				Instr(Mnemonic.fucomp, F),
				Instr(Mnemonic.fucomp, F),
				Instr(Mnemonic.fucomp, F),
						
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

				Instr(Mnemonic.fiadd, Mw),
				Instr(Mnemonic.fimul, Mw),
				Instr(Mnemonic.ficom, Mw),
				Instr(Mnemonic.ficomp, Mw),
				Instr(Mnemonic.fisub, Mw),
				Instr(Mnemonic.fisubr, Mw),
				Instr(Mnemonic.fidiv, Mw),
				Instr(Mnemonic.fidivr, Mw),
				
                // DE C0
                Instr(Mnemonic.faddp, F,f),
				Instr(Mnemonic.faddp, F,f),
				Instr(Mnemonic.faddp, F,f),
				Instr(Mnemonic.faddp, F,f),
				Instr(Mnemonic.faddp, F,f),
				Instr(Mnemonic.faddp, F,f),
				Instr(Mnemonic.faddp, F,f),
				Instr(Mnemonic.faddp, F,f),
						
				Instr(Mnemonic.fmulp, F,f),
				Instr(Mnemonic.fmulp, F,f),
				Instr(Mnemonic.fmulp, F,f),
				Instr(Mnemonic.fmulp, F,f),
				Instr(Mnemonic.fmulp, F,f),
				Instr(Mnemonic.fmulp, F,f),
				Instr(Mnemonic.fmulp, F,f),
				Instr(Mnemonic.fmulp, F,f),
						
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
				Instr(Mnemonic.fcompp),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

				// DE E0	
				Instr(Mnemonic.fsubrp, F,f),
				Instr(Mnemonic.fsubrp, F,f),
				Instr(Mnemonic.fsubrp, F,f),
				Instr(Mnemonic.fsubrp, F,f),
				Instr(Mnemonic.fsubrp, F,f),
				Instr(Mnemonic.fsubrp, F,f),
				Instr(Mnemonic.fsubrp, F,f),
				Instr(Mnemonic.fsubrp, F,f),
						
				Instr(Mnemonic.fsubp, F,f),
				Instr(Mnemonic.fsubp, F,f),
				Instr(Mnemonic.fsubp, F,f),
				Instr(Mnemonic.fsubp, F,f),
				Instr(Mnemonic.fsubp, F,f),
				Instr(Mnemonic.fsubp, F,f),
				Instr(Mnemonic.fsubp, F,f),
				Instr(Mnemonic.fsubp, F,f),

				// DE F0
				Instr(Mnemonic.fdivrp, F,f),
				Instr(Mnemonic.fdivrp, F,f),
				Instr(Mnemonic.fdivrp, F,f),
				Instr(Mnemonic.fdivrp, F,f),
				Instr(Mnemonic.fdivrp, F,f),
				Instr(Mnemonic.fdivrp, F,f),
				Instr(Mnemonic.fdivrp, F,f),
				Instr(Mnemonic.fdivrp, F,f),
						
				Instr(Mnemonic.fdivp, F,f),
				Instr(Mnemonic.fdivp, F,f),
				Instr(Mnemonic.fdivp, F,f),
				Instr(Mnemonic.fdivp, F,f),
				Instr(Mnemonic.fdivp, F,f),
				Instr(Mnemonic.fdivp, F,f),
				Instr(Mnemonic.fdivp, F,f),
				Instr(Mnemonic.fdivp, F,f),
				
				// DF //////////////////////

				Instr(Mnemonic.fild, Mw),
				Instr(Mnemonic.fisttp, Mw),
				Instr(Mnemonic.fist, Mw),
				Instr(Mnemonic.fistp, Mw),
				Instr(Mnemonic.fbld, MB),
				Instr(Mnemonic.fild, Mq),
				Instr(Mnemonic.fbstp, MB),
				Instr(Mnemonic.fistp, Mq),

				// DF C0
				Instr(Mnemonic.ffreep, F),
				Instr(Mnemonic.ffreep, F),
				Instr(Mnemonic.ffreep, F),
				Instr(Mnemonic.ffreep, F),
				Instr(Mnemonic.ffreep, F),
				Instr(Mnemonic.ffreep, F),
				Instr(Mnemonic.ffreep, F),
				Instr(Mnemonic.ffreep, F),
						
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
				Instr(Mnemonic.fstsw, aw),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,

                Instr(Mnemonic.fucomip, f,F),
                Instr(Mnemonic.fucomip, f,F),
                Instr(Mnemonic.fucomip, f,F),
                Instr(Mnemonic.fucomip, f,F),
                Instr(Mnemonic.fucomip, f,F),
                Instr(Mnemonic.fucomip, f,F),
                Instr(Mnemonic.fucomip, f,F),
                Instr(Mnemonic.fucomip, f,F),

				// DF F0
                Instr(Mnemonic.fcomip, f,F),
                Instr(Mnemonic.fcomip, f,F),
                Instr(Mnemonic.fcomip, f,F),
                Instr(Mnemonic.fcomip, f,F),
                Instr(Mnemonic.fcomip, f,F),
                Instr(Mnemonic.fcomip, f,F),
                Instr(Mnemonic.fcomip, f,F),
                Instr(Mnemonic.fcomip, f,F),

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

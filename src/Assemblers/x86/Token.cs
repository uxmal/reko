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

using System;

namespace Reko.Assemblers.x86
{
	public enum Token
	{
		ERR, EOFile, EOL, 
		ID, COLON, COMMA, BRA, KET, PLUS, MINUS, TIMES,
		INTEGER, REGISTER, STRINGLITERAL,
		LPAREN, RPAREN, EQUALS,

		PROC, ENDP, i386, i386p, i86, TEXT, DATA,
		STRUCT, DB, DW, DD, ENDS, FAR, SHORT, LONG, 
		BYTE, WORD, DWORD, QWORD, PTR, DUP,
		OFFSET, IMPORT, TITLE, INCLUDE, INCLUDELIB, 
		IF, ELSE, ENDIF,
		MODEL, SEGMENT, GROUP,  ASSUME, PUBLIC, EXTRN,
		COMM,

		MOV, MOVSX, MOVZX, LEA, LDS, LES, LGS, LFS, LSS, XCHG,
		ADD, SUB, CMP, XOR, OR, AND, ADC, SBB, TEST,
		MUL, IMUL, DIV, IDIV,
		INC, DEC, NEG, NOT,
		SHR, SHL, SAR, SHRD, SHLD,

		PUSH, POP,
		CALL, RET, RETF, INT,
		ENTER, LEAVE,
		JMP,
		JZ, JNZ, JA, JBE, JC, JCXZ, JNC, JNS, JL, JLE, JG, JGE, JS, JO, JNO, JPO, JPE, LOOP, LOOPE, LOOPNE,
		SETNZ, SETZ,
		IN, OUT,			
		CMPSB, CMPSW, CMPSD,
        MOVSB, MOVSD, MOVSW,
        LODSB, LODSW, LODSD,
        SCASB, SCASW, SCASD,
        STOSB, STOSW, STOSD,
        REP,
		RCL, RCR, ROL, ROR,
		CWD, CDQ,
		STC, CLC, CMC, STI, CLI,
		BT, BSR,

		ST, FADD, FADDP, FCOM, FCOMP, FCOMPP, FSUB, FSUBR, FSUBP, FSUBRP,
		FMUL, FMULP, FDIV, FDIVR, FDIVP, FDIVRP,
		FLD, FLD1, FLDZ, FILD, FISTP, FST, FSTSW, FIST,  FSTP, 
	}
}

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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.Assemblers.x86
{
	/// <summary>
	/// Performs lexical analysis of x86 assembler, MASM-style.
	/// </summary>
	public class Lexer
	{
		private TextReader rdr;
		private Token tok;
		private int lineNumber;
		private StringBuilder sb;
		private RegisterStorage reg;
		private int integer;

        private static SortedList<string, Token> keywords = new SortedList<string, Token>(StringComparer.InvariantCultureIgnoreCase);
		private const string IdentifierCharacters = "._$@?";

		public Lexer(TextReader rdr)
		{
			this.sb = new StringBuilder();
			this.lineNumber = 1;
			this.rdr = rdr;
			this.tok = Token.EOFile;
		}

		static Lexer()
		{
			keywords[".386"] = Token.i386p;
			keywords[".386p"] = Token.i386p;
			keywords[".data"] = Token.DATA;
			keywords[".i386"] = Token.i386;
			keywords[".i86"] = Token.i86; 
			keywords[".import"] = Token.IMPORT;
			keywords[".model"]  = Token.MODEL;
			keywords[".text"] = Token.TEXT;
			keywords["assume"] = Token.ASSUME;
			keywords["comm"] = Token.COMM;
			keywords["db"] = Token.DB;
			keywords["dd"] = Token.DD;
			keywords["dup"] = Token.DUP;
			keywords["dw"] = Token.DW;
			keywords["else"] = Token.ELSE;
			keywords["endif"] = Token.ENDIF;
			keywords["endp"] = Token.ENDP;
			keywords["ends"] = Token.ENDS;
			keywords["extrn"] = Token.EXTRN;
            keywords["far"] = Token.FAR;
			keywords["group"] = Token.GROUP;
			keywords["if"] = Token.IF;
			keywords["include"] = Token.INCLUDE;
			keywords["includelib"] = Token.INCLUDELIB;
			keywords["proc"] = Token.PROC;
			keywords["public"] = Token.PUBLIC;
			keywords["segment"] = Token.SEGMENT;
			keywords["short"] = Token.SHORT;

			keywords["adc"] = Token.ADC;
			keywords["add"] = Token.ADD;
			keywords["and"] = Token.AND;
			keywords["bsr"] = Token.BSR;
			keywords["bt"] = Token.BT;
			keywords["byte"] = Token.BYTE;
			keywords["call"] = Token.CALL;
			keywords["cdq"] = Token.CDQ;
			keywords["clc"] = Token.CLC;
			keywords["cmc"] = Token.CMC;
			keywords["cmp"] = Token.CMP;
			keywords["cwd"] = Token.CWD;
			keywords["db"] = Token.DB;
			keywords["dec"] = Token.DEC;
			keywords["div"] = Token.DIV;
			keywords["dw"] = Token.DW;
			keywords["dword"] = Token.DWORD;
			keywords["enter"] = Token.ENTER;

			keywords["fadd"] = Token.FADD;
			keywords["faddp"] = Token.FADDP;
			keywords["fcom"] = Token.FCOM;
            keywords["fcomp"] = Token.FCOMP;
            keywords["fcompp"] = Token.FCOMPP;
			keywords["fdiv"] = Token.FDIV;
			keywords["fdivp"] = Token.FDIVP;
			keywords["fdivr"] = Token.FDIVR;
			keywords["fdivrp"] = Token.FDIVRP;
			keywords["fild"] = Token.FILD;
			keywords["fistp"] = Token.FISTP;
			keywords["fld"] = Token.FLD;
			keywords["fld1"] = Token.FLD1;
			keywords["fldz"] = Token.FLDZ;
			keywords["fmul"] = Token.FMUL;
			keywords["fmulp"] = Token.FMULP;
			keywords["fst"] = Token.FST;
			keywords["fstsw"] = Token.FSTSW;
			keywords["fstp"] = Token.FSTP;
			keywords["fsub"] = Token.FSUB;
			keywords["fsubp"] = Token.FSUBP;
			keywords["fsubr"] = Token.FSUBR;
			keywords["fsubrp"] = Token.FSUBRP;

			keywords["idiv"] = Token.IDIV;
			keywords["imul"] = Token.IMUL;
			keywords["in"] = Token.IN;
			keywords["int"] = Token.INT;
			keywords["inc"] = Token.INC;
			keywords["ja"] = Token.JA;
			keywords["jae"] = Token.JNC;
			keywords["jb"] = Token.JC;
			keywords["jbe"] = Token.JBE;
			keywords["jc"] = Token.JC;
			keywords["jcxz"] = Token.JCXZ;
			keywords["je"] = Token.JZ;
			keywords["jg"] = Token.JG;
			keywords["jge"] = Token.JGE;
			keywords["jl"] = Token.JL;
			keywords["jle"] = Token.JLE;
			keywords["jmp"] = Token.JMP;
			keywords["jnc"] = Token.JNC;
			keywords["jne"] = Token.JNZ;
			keywords["jno"] = Token.JNO;
            keywords["jnp"] = Token.JPO;
            keywords["jns"] = Token.JNS;
			keywords["jnz"] = Token.JNZ;
            keywords["jp"] = Token.JPE;
            keywords["jpe"] = Token.JPE;
            keywords["jpo"] = Token.JPO;
			keywords["jo"] = Token.JO;
			keywords["js"] = Token.JS;
			keywords["jz"] = Token.JZ;
			keywords["lds"] = Token.LDS;
			keywords["lea"] = Token.LEA;
			keywords["leave"] = Token.LEAVE;
			keywords["les"] = Token.LES;
			keywords["lfs"] = Token.LFS;
			keywords["lgs"] = Token.LGS;
            keywords["loop"] = Token.LOOP;
			keywords["loope"] = Token.LOOPE;
			keywords["loopne"] = Token.LOOPNE;
			keywords["long"] = Token.LONG;
			keywords["lss"] = Token.LSS;

			keywords["mov"] = Token.MOV;
			keywords["movsx"] = Token.MOVSX;
			keywords["movzx"] = Token.MOVZX;
			keywords["mul"] = Token.MUL;
			keywords["neg"] = Token.NEG;
			keywords["not"] = Token.NOT;
			keywords["offset"] = Token.OFFSET;
			keywords["or"] = Token.OR;
			keywords["out"] = Token.OUT;
			keywords["pop"] = Token.POP;
			keywords["ptr"] = Token.PTR;
			keywords["push"] = Token.PUSH;
			keywords["qword"] = Token.QWORD;
			keywords["rcl"] = Token.RCL;
			keywords["rcr"] = Token.RCR;
			keywords["rep"] = Token.REP;
            keywords["ret"] = Token.RET;
            keywords["retf"] = Token.RETF;
			keywords["rol"] = Token.ROL;
			keywords["ror"] = Token.ROR;
			keywords["sar"] = Token.SAR;
			keywords["sbb"] = Token.SBB;
            keywords["setnz"] = Token.SETNZ;
			keywords["setz"] = Token.SETZ;
			keywords["shl"] = Token.SHL;
			keywords["shld"] = Token.SHLD;
			keywords["shr"] = Token.SHR;
			keywords["shrd"] = Token.SHRD;
			keywords["st"] = Token.ST;
			keywords["stc"] = Token.STC;
            keywords["struct"] = Token.STRUCT;
			keywords["sub"] = Token.SUB;
			keywords["test"] = Token.TEST;
			keywords["title"] = Token.TITLE;
			keywords["word"] = Token.WORD;
            keywords["xchg"] = Token.XCHG;
			keywords["xor"] = Token.XOR;

            keywords["lodsb"] = Token.LODSB;
            keywords["lodsw"] = Token.LODSW;
            keywords["lodsd"] = Token.LODSD;
            keywords["movsb"] = Token.MOVSB;
            keywords["movsw"] = Token.MOVSW;
            keywords["movsd"] = Token.MOVSD;
            keywords["stosb"] = Token.STOSB;
            keywords["stosw"] = Token.STOSW;
            keywords["stosd"] = Token.STOSD;
            keywords["scasb"] = Token.SCASB;
            keywords["scasw"] = Token.SCASW;
            keywords["scasd"] = Token.SCASD;
            keywords["cmpsb"] = Token.CMPSB;
            keywords["cmpsw"] = Token.CMPSW;
            keywords["cmpsd"] = Token.CMPSD;
        }

		private Token ClassifySymbol()
		{
			string s = sb.ToString();
            Token tok;
            if (keywords.TryGetValue(s, out tok))
                return tok;
			RegisterStorage reg = Registers.GetRegister(s);
			if (reg != RegisterStorage.None)
			{
				this.reg = reg;
				return Token.REGISTER;
			}
			return Token.ID;
		}

		public void DiscardToken()
		{
			tok = Token.EOFile;
		}

		private int EatWS()
		{
			int ch = rdr.Read();
			while (ch != -1 && (ch == ' ' || ch == '\t'))
				ch = rdr.Read();
			if (ch == ';')
			{
				while (ch != -1 && ch != '\n')
					ch = rdr.Read();
			}
			return ch;
		}

		public Token FetchToken()
		{
			int ch;

			ch = EatWS();
			if (ch == -1)
				return Token.EOFile;

			switch (ch)
			{
			case '\r':
				ch = rdr.Peek();
				if (ch == '\n')
					rdr.Read();
				++lineNumber;
				return Token.EOL;
			case '\n':
				++lineNumber;
				return Token.EOL;
			case ',': return Token.COMMA;
			case ':': return Token.COLON;
			case '+': return Token.PLUS;
			case '-': return Token.MINUS;
			case '[': return Token.BRA;
			case ']': return Token.KET;
			case '(': return Token.LPAREN;
			case ')': return Token.RPAREN;
			case '*': return Token.TIMES;
			case '=': return Token.EQUALS;
				//	case '\"': return DQUOTE;
			case '\'':
				sb = new StringBuilder();
				for (;;)
				{
					ch = rdr.Read();
					if (ch == -1 || ch == '\'')
						return Token.STRINGLITERAL;
					sb.Append((char)ch);
				}
			default:
				if (char.IsDigit((char)ch))
				{
					integer = 0;
					sb = new StringBuilder();
					sb.Append((char) ch);
					if (ch == '0')
					{
						ch = rdr.Peek();
						if (ch == -1)
							return Token.INTEGER;

						if (ch == 'x' || ch == 'X')
						{
							rdr.Read();
							ch = rdr.Peek();
							while ("0123456789abcdefABCDEF".IndexOf((char) ch) >= 0)
							{
								sb.Append((char) ch);
								rdr.Read();
								ch = rdr.Peek();
							}
							integer = Convert.ToInt32(sb.ToString(), 16);
							return Token.INTEGER;
						}
						else if (Char.IsDigit((char)ch))
						{
							sb.Append((char)ch);
							rdr.Read();
						}
						else
						{
							integer = Convert.ToInt32(sb.ToString(), 16);
							return Token.INTEGER;
						}
					}

					ch = rdr.Peek();
					while (Char.IsDigit((char)ch))
					{
						sb.Append((char)ch);
						rdr.Read();
						ch = rdr.Peek();
					}
					integer = Convert.ToInt32(sb.ToString(), 10);
					return Token.INTEGER;
				}
				if (Char.IsLetter((char)ch) || IdentifierCharacters.IndexOf((char)ch) >= 0)
				{
					sb = new StringBuilder();
					sb.Append((char) ch);
					ch = rdr.Peek();
					if (ch != -1)
					{
						while (Char.IsLetterOrDigit((char)ch) || IdentifierCharacters.IndexOf((char)ch) >= 0)
						{
							sb.Append((char) ch);
							rdr.Read();
							ch = rdr.Peek();
						}
					}
					return ClassifySymbol();
				}
				return Token.ERR;
			}
		}

		public Token GetToken()
		{
			if (tok != Token.EOFile)
			{
				Token tokT = tok;
				tok = Token.EOFile;
				return tokT;
			}
			return FetchToken();
		}

		public int Integer
		{
			get { return integer; }
		}

		public int LineNumber
		{
			get { return lineNumber; }
		}

		public Token PeekToken()
		{
			if (tok != Token.EOFile)
				return tok;

			tok = FetchToken();
			return tok;
		}

		public RegisterStorage Register
		{
			get { return reg; }
		}

		public void SkipUntil(Token t)
		{
			Token tok = PeekToken();
			while (tok != t && tok != Token.EOFile)
			{
				DiscardToken();
				tok = PeekToken();
			}
		}

		public string StringLiteral
		{
			get { return sb.ToString(); }
		}

		private void UngetToken(Token tok)
		{
			Debug.Assert(tok == Token.EOFile);
			this.tok = tok;
		}
	}
}
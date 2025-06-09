#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Assemblers;
using Reko.Core.Loading;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.Arch.X86.Assembler
{
    /// <summary>
    /// A crude MASM-style assembler for x86 opcodes.
    /// </summary>
    public class X86TextAssembler : IAssembler
	{
        private readonly IntelArchitecture arch;
        private readonly List<ImageSymbol> entryPoints;
#nullable disable
        private Lexer lexer;
		private Address addrBase;
		private Address addrStart;
        private X86Assembler asm;
#nullable enable

        public X86TextAssembler(IntelArchitecture arch)
		{
            this.entryPoints = new List<ImageSymbol>();
            this.ImageSymbols = new List<ImageSymbol>();
            this.arch = arch;
        }

        public Program Assemble(Address addr, string filename, TextReader rdr)
        {
            addrBase = addr;
            lexer = new Lexer(rdr);

            asm = new X86Assembler(this.arch, addrBase, entryPoints);

            // Assemblers are strongly line-oriented.

            while (lexer.PeekToken() != Token.EOFile)
            {
                try {
                    ProcessLine();
                }
                catch (Exception ex)
                {
                    Debug.Print("Error on line {0}: {1}", lexer.LineNumber, ex.Message);
                    throw;
                }
            }

            asm.ReportUnresolvedSymbols();
            addrStart = addrBase;
            return asm.GetImage();
        }

		public Program AssembleFragment(Address addr, string fragment)
		{
			return Assemble(addr, "", new StringReader(fragment));
		}

        public int AssembleAt(Program program, Address addr, TextReader rdr)
        {
            addrBase = addr;
            lexer = new Lexer(rdr);

            asm = new X86Assembler(program, addrBase);
            var initialPos = asm.CurrentPosition;

            // Assemblers are strongly line-oriented.

            while (lexer.PeekToken() != Token.EOFile)
            {
                try
                {
                    ProcessLine();
                }
                catch (Exception ex)
                {
                    Debug.Print("Error on line {0}: {1}", lexer.LineNumber, ex.Message);
                    break;
                }
            }
            asm.ReportUnresolvedSymbols();
            var bytesWritten = asm.CurrentPosition - initialPos;
            return bytesWritten;
        }

        public int AssembleFragmentAt(Program program, Address addr, string asmFragment)
        {
            return AssembleAt(program, addr, new StringReader(asmFragment));
        }

        public IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public ICollection<ImageSymbol> EntryPoints
        {
            get { return entryPoints; }
        }

        public ICollection<ImageSymbol> ImageSymbols { get; private set; }

        public Address StartAddress
        {
            get { return addrStart; }
        }

        public Dictionary<Address, ImportReference> ImportReferences
        {
            get { return asm.ImportReferences; }
        }

		private bool Error(string pstr)
		{
			throw new ApplicationException(string.Format("line {0}: error: {1}", lexer.LineNumber, pstr));
		}

		private void Expect(Token tok)
		{
			if (lexer.GetToken() != tok)
				Error("expected '" + tok + "'");
		}

		private void Expect(Token tok, string msg)
		{
			if (lexer.GetToken() != tok)
				Error(msg);
		}

		public ParsedOperand []? ParseOperandList(int count)
		{
			return ParseOperandList(count, count);
		}

		public ParsedOperand []? ParseOperandList(int min, int max)
		{
            OperandParser opp = asm.CreateOperandParser(lexer);
            List<ParsedOperand> ops = new List<ParsedOperand>();
			asm.SegmentOverride = RegisterStorage.None;
			asm.AddressWidth = asm.SegmentAddressWidth;
			if (lexer.PeekToken() != Token.EOL)
			{
                var op = opp.ParseOperand();
                if (op is not null)
                    ops.Add(op);
				if (opp.SegmentOverride != RegisterStorage.None)
					asm.SegmentOverride = opp.SegmentOverride;
				if (opp.AddressWidth is not null)
					asm.AddressWidth = (PrimitiveType)opp.AddressWidth;

				while (lexer.PeekToken() == Token.COMMA)
				{
					lexer.DiscardToken();
					opp.DataWidth = ops[0].Operand.DataType;
                    op = opp.ParseOperand();
                    if (op is not null)
                        ops.Add(op);
					if (opp.SegmentOverride != RegisterStorage.None)
					{
						if (asm.SegmentOverride != RegisterStorage.None)
							Error("Can't have two segment overrides in one instruction");
						asm.SegmentOverride = opp.SegmentOverride;
					}
					if (opp.AddressWidth is not null)
						asm.AddressWidth = (PrimitiveType)opp.AddressWidth;
				}
			}

			if (min <= ops.Count && ops.Count <= max)
			{
				ParsedOperand [] o = new ParsedOperand[ops.Count];
				ops.CopyTo(o);
				return o;
			}
			else
			{
				if (min == max)
					Error(string.Format("Instruction expects {0} operand(s).", min));
				else
					Error(string.Format("Instruction expects between {0} and {1} operand(s).", min, max));
				return null;
			}
		}

		public void ProcessAssume()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessBinop(int binop)
		{
			ParsedOperand []? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessBinop(binop, ops);
        }
		
		private void ProcessBitOp()
		{
			ParsedOperand []? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessBitOp(ops);
		}

		private void ProcessBitScan(byte opCode)
		{
			ParsedOperand []? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessBitScan(opCode, ops[0], ops[1]);
		}

		private void ProcessBranch(int cc)
		{
            string destination;

			Token token = lexer.GetToken();
			switch (token)
			{
			case Token.SHORT:
				Expect(Token.ID);
				goto case Token.ID;
			case Token.ID:
                destination = lexer.StringLiteral;
                asm.ProcessShortBranch(cc, destination);
				break;				
			case Token.LONG:
				Expect(Token.ID);
                destination = lexer.StringLiteral;
                asm.ProcessLongBranch(cc, destination);
				break;
			}
		}

		private void ProcessLoop(int opcode)
		{
			Expect(Token.ID);
            string destination = lexer.StringLiteral;

            asm.ProcessLoop(opcode, destination);
		}


		private void ProcessCallJmp(bool isCall)
		{
            // call lbl = 0xE8
            // call [indir] = 0x02
            // call far lbl = 0x9A
            // call far [indir] = 0x03

            // jmp lbl = 0xE9
            // jmp [indir] = 0x04
            // jmp far lbl = 0x9A
            // jmp far [indir] = 0x03

            bool far = false;
            Token token = lexer.PeekToken();
            if (token == Token.FAR)
            {
                lexer.DiscardToken();
                far = true;
			    token = lexer.PeekToken();
            }
            switch (token)
			{
			default:
				Error("Unexpected token: " + token);
				break;
			case Token.BRA:
			case Token.DWORD:
			case Token.WORD:
			case Token.REGISTER:
                int indirect = isCall
                    ? (far ? 0x03 : 0x02)
                    : (far ? 0x05 : 0x04);
				var ops = ParseOperandList(1);
                if (ops is null)
                    return;
                asm.ProcessCallJmp(far, indirect, ops[0]);
				break;
			case Token.ID:
                var str = lexer.StringLiteral;
                lexer.DiscardToken();
                if (lexer.PeekToken() == Token.BRA)
                {
                    var opp = asm.CreateOperandParser(lexer);
                    var target = opp.ParseIdOperand(str);
                    if (target is null)
                        return;
                    int indir = isCall
                        ? (far ? 0x03 : 0x02)
                        : (far ? 0x05 : 0x04);
                    asm.ProcessCallJmp(far, indir, target);
                }
                else
                {
                    int direct = isCall
                        ? (far ? 0x9A : 0xE8)
                        : (far ? 0xEA : 0xE9);
                    asm.ProcessCallJmp(far, direct, str);
                }
				break;
			}
		}

		public void ProcessComm()
		{
			Expect(Token.ID);
            string sym = lexer.StringLiteral;
			Expect(Token.COLON);
			Token t = lexer.GetToken();
			switch (t)
			{
			case Token.DWORD:
                asm.ProcessComm(sym);
				break;
			default:
				Error("Unexpected token: " + t);
				break;
			}
		}

		private void ProcessCwd(PrimitiveType width)
		{
            asm.ProcessCwd(width);
		}

		private void ProcessDB()
		{
			for (;;)
			{
				Token tok = lexer.GetToken();
				switch (tok)
				{
				case Token.INTEGER:
                    int by = lexer.Integer;
                    if (lexer.PeekToken() == Token.DUP)
                    {
                        lexer.DiscardToken();
                        Expect(Token.INTEGER, "expected count to follow 'dup'");
                        asm.DbDup(by, lexer.Integer);
                    }
                    else
                    {
                        asm.Db(by);
                    }
					break;
				case Token.STRINGLITERAL:
					asm.Dstring(lexer.StringLiteral);
					break;
				default:
					Error("unexpected tokens following 'db'");
					break;
				}

				tok = lexer.PeekToken();
				if (tok == Token.EOFile || tok == Token.EOL)
					return;

				Expect(Token.COMMA, "expected ','");
			}
		}
        private void ProcessTest()
        {
            ParsedOperand[]? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessTest(ops);
        }

		public void ProcessTitle()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessWords(PrimitiveType width)
		{
			for (;;)
			{
				Token tok = lexer.GetToken();
				switch (tok)
				{
				case Token.OFFSET:
					Expect(Token.ID);
					goto case Token.ID;
				case Token.INTEGER:
					asm.DefineWord(width, lexer.Integer);
					break;
				case Token.ID:
				{
                    string symbolText = lexer.StringLiteral;
                    asm.DefineWord(width, symbolText);
					break;
				}
				default:
					throw new ApplicationException("unexpected token: " + tok);
				}
				
				tok = lexer.PeekToken();
				if (tok == Token.EOFile || tok == Token.EOL)
					return;
				Expect(Token.COMMA);
			}
		}

		private void ProcessEndp()
		{
			// TODO: keep track of procedures in a stack.
			Expect(Token.EOL);
		}

		public void ProcessEnds()
		{
			lexer.SkipUntil(Token.EOL);
		}
		
		private void ProcessEnter()
		{
			int cbStack = 0;
			int nLevel = 0;
			if (lexer.PeekToken() == Token.INTEGER)
			{
				lexer.DiscardToken();
				cbStack = lexer.Integer;
				if (lexer.PeekToken() == Token.COMMA)
				{
					lexer.DiscardToken();
					Expect(Token.INTEGER);
					nLevel = lexer.Integer;
				}
			}

            asm.Enter(cbStack, nLevel);
		}

		private void ProcessIncDec(bool fDec)
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.ProcessIncDec(fDec, ops[0]);
        }

		public void ProcessInclude()
		{
			lexer.SkipUntil(Token.EOL);
		}

		public void ProcessIf()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessDiv(int operation)
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.ProcessDiv(operation, ops[0]);
		}

		private void ProcessExtrn()
		{
			Expect(Token.ID);
            string externSymbol = lexer.StringLiteral;
            asm.Extern(externSymbol, asm.SegmentAddressWidth);     //$REVIEW: doesn't take into account NEAR, FAR for real mode.
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessFild()
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.Fild(ops[0]);
		}

		private void ProcessFistp()
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.Fistp(ops[0]);
		}

		/// <summary>
		/// Handles the FPU operations that are similar in structure: FADD, FMUL, FCOM, FCOMP, FSUB, FDIV, FSUBR, FDIVR
		/// </summary>
		/// <param name="opcodeFreg"></param>
		/// <param name="opcodeMem"></param>
		/// <param name="reg"></param>
		/// <param name="fixedOrder">true for FSUB... and FDIV... since order is important</param>
		private void ProcessFpuCommon(int opcodeFreg, int opcodeMem, int fpuOperation, bool isPop, bool fixedOrder)
		{
			ParsedOperand []? ops = ParseOperandList(0,2);
            if (ops is null)
                return;
            asm.ProcessFpuCommon(opcodeFreg, opcodeMem, fpuOperation, isPop, fixedOrder, ops);
        }

		private void ProcessFst(bool pop)
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.ProcessFst(pop, ops[0]);
		}

		private void ProcessImport(string s)
		{
			Expect(Token.STRINGLITERAL);
			string fnName = lexer.StringLiteral;
			Expect(Token.COMMA);
			Expect(Token.STRINGLITERAL);
			string dllName = lexer.StringLiteral.ToLower();

            asm.Import(s, fnName, dllName);
		}

		private void ProcessImul()
		{
			ParsedOperand []? ops = ParseOperandList(1, 3);
            if (ops is null)
                return;
            asm.ProcessImul(ops);
        }

		private void ProcessInOut(bool fOut)
		{
			ParsedOperand []? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessInOut(fOut, ops);
        }

		private void ProcessInt()
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.ProcessInt(ops[0]);
		}

		private void ProcessLabel()
		{
			string s = lexer.StringLiteral;

			Token tok = lexer.PeekToken();
			switch (tok)
			{
			default: 
				Error("token '" + s + "' unexpected");
				return;
			case Token.DB:
				asm.Label(s);
				lexer.DiscardToken();
				ProcessDB();
				break;
			case Token.DW:
				asm.Label(s);
				lexer.DiscardToken();
				ProcessWords(PrimitiveType.Word16);
				break;
			case Token.DD:
				asm.Label(s);
				lexer.DiscardToken();
				ProcessWords(PrimitiveType.Word32);
				break;

			case Token.ENDP:
				lexer.DiscardToken();
				ProcessEndp();
				break;
			case Token.ENDS:
				lexer.DiscardToken();
				ProcessEnds();
				break;
			case Token.EQUALS:
				lexer.DiscardToken();
				Expect(Token.INTEGER);
                asm.Equ(s, lexer.Integer);
				break;
			case Token.PROC:
				lexer.SkipUntil(Token.EOL);		// TODO: keep track of procedures on a stack?
				Expect(Token.EOL);
                asm.Proc(s);
				break;
			case Token.COLON:
				asm.Label(s);
				lexer.DiscardToken();
				ProcessLine();
				break;
			case Token.EOL:
				asm.Label(s);
				break;

			case Token.GROUP:
				asm.Label(s);
				lexer.SkipUntil(Token.EOL);
				lexer.DiscardToken();
				break;
			case Token.IMPORT:
				lexer.DiscardToken();
				ProcessImport(s);
				break;
			case Token.SEGMENT:
				ProcessSegment();
				lexer.DiscardToken();
				break;
			}
		}

		private void ProcessLea()
		{
			ParsedOperand []? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.Lea(ops[0], ops[1]);
		}

		private void ProcessLeave()
		{
            asm.Leave();
		}

		private void ProcessLine()
		{
			asm.AddressWidth = null;
			Token tok = lexer.GetToken();
			switch (tok)
			{
			default:
				Error("unexpected token: " + tok.ToString());
				return;
			case Token.EOL:
				return;
				/* puppen: q11111111111` */
			case Token.i86:
                asm.i86();
				lexer.SkipUntil(Token.EOL);
				break;
			case Token.i386:
			case Token.i386p:
                asm.i386();
				lexer.SkipUntil(Token.EOL);
				break;
			case Token.ASSUME:
				ProcessAssume();
				break;
			case Token.COMM:
				ProcessComm();
				break;
			case Token.EXTRN:
				ProcessExtrn();
				break;
			case Token.IF:
				ProcessIf();
				break;
			case Token.INCLUDELIB:
				ProcessInclude();
				break;
			case Token.ENDIF:
				lexer.SkipUntil(Token.EOL);
				break;
			case Token.PUBLIC:
				ProcessPublic();
				break;

			case Token.ADD:
				ProcessBinop(0x00);
				break;
			case Token.ADC:
				ProcessBinop(0x02);
				break;
			case Token.AND:
				ProcessBinop(0x04);
				break;
			case Token.BSR:
				ProcessBitScan(0xBD);
				break;
			case Token.BT:
				ProcessBitOp();
				break;
			case Token.CALL:
				ProcessCallJmp(true);
				break;
			case Token.CDQ:
				asm.ProcessCwd(PrimitiveType.Word32);
				break;
			case Token.CLC:
                asm.Clc();
				break;
			case Token.CMC:
                asm.Cmc();
				break;
			case Token.CWD:
				asm.ProcessCwd(PrimitiveType.Word16);
				break;
			case Token.DEC:
				ProcessIncDec(true);
				break;
			case Token.DIV:
				ProcessDiv(0x06);
				break;
			case Token.DB:
				ProcessDB();
				break;
			case Token.DD:
				ProcessWords(PrimitiveType.Word32);
				break;
			case Token.DW:
				ProcessWords(PrimitiveType.Word16);
				break;
			case Token.ENDP:
				ProcessEndp();
				return;
			case Token.ENDS:
				ProcessEnds();
				return;
			case Token.ENTER:
				ProcessEnter();
				break;

			case Token.FADD:
				ProcessFpuCommon(0xD8, 0xD8, 0, false, false);
				break;
			case Token.FADDP:
				ProcessFpuCommon(0xDE, 0xD8, 0, true, false);
				break;
			case Token.FCOM:
				ProcessFpuCommon(0xD8, 0xD8, 2, false, false);
				break;
			case Token.FCOMP:
				ProcessFpuCommon(0xD8, 0xD8, 3, true, false);
				break;
            case Token.FCOMPP:
                asm.Fcompp();
                break;
			case Token.FDIV:
				ProcessFpuCommon(0xD8, 0xD8, 6, false, true);
				break;
			case Token.FDIVP:
				ProcessFpuCommon(0xDE, 0xD8, 6, true, true);
				break;
			case Token.FDIVRP:
				ProcessFpuCommon(0xDE, 0xD9, 7, true, true);
				break;
			case Token.FDIVR:
				ProcessFpuCommon(0xD8, 0xD9, 7, false, true);
				break;
			case Token.FILD:
				ProcessFild();
				break;
			case Token.FISTP:
				ProcessFistp();
				break;
			case Token.FLD:
				ProcessFpuCommon(0xD9, 0xD9, 0, false, false);
				break;
			case Token.FLD1:
                asm.Fld1();
				break;
			case Token.FLDZ:
                asm.Fldz();
				break;
			case Token.FMUL:
				ProcessFpuCommon(0xD8, 0xD8, 1, false, false);
				break;
			case Token.FMULP:
				ProcessFpuCommon(0xDE, 0xD8, 1, true, false);
				break;
			case Token.FST:
				ProcessFst(false);
				break;
			case Token.FSTP:
				ProcessFst(true);
				break;
			case Token.FSTSW:
				ProcessFstsw();
				break;
			case Token.FSUB:
				ProcessFpuCommon(0xD8, 0xD8, 4, false, true);
				break;
			case Token.FSUBP:
				ProcessFpuCommon(0xDE, 0xD8, 4, true, true);
				break;
			case Token.FSUBR:
				ProcessFpuCommon(0xD8, 0xD8, 5, false, true);
				break;
			case Token.FSUBRP:
				ProcessFpuCommon(0xDE, 0xD8, 5, true, true);
				break;

			case Token.ID:
				ProcessLabel(); 
				return;
			case Token.ELSE:
				ProcessIf();
				break;
			case Token.IDIV:
				ProcessDiv(0x07);
				break;
			case Token.IMUL:
				ProcessImul();
				break;
			case Token.IN:
				ProcessInOut(false);
				break;
			case Token.INC: 
				ProcessIncDec(false);
				break;
			case Token.INCLUDE:
				ProcessInclude();
				break;
			case Token.INT:
				ProcessInt();
				break;
			case Token.JA:
				ProcessBranch(0x07);
				break;
			case Token.JBE:
				ProcessBranch(0x06);
				break;
			case Token.JC:
				ProcessBranch(0x02);
				break;
			case Token.JCXZ:
				Expect(Token.ID);
                asm.Jcxz(lexer.StringLiteral);
				break;
			case Token.JG:
				ProcessBranch(0x0F);
				break;
			case Token.JGE:
				ProcessBranch(0x0D);
				break;
			case Token.JL:
				ProcessBranch(0x0C);
				break;
			case Token.JLE:
				ProcessBranch(0x0E);
				break;
			case Token.JMP:
				ProcessCallJmp(false);
				break;
			case Token.JNB:
			case Token.JNC:
                ProcessBranch(0x03);
				break;
			case Token.JNS:
				ProcessBranch(0x09);
				break;
			case Token.JNO:
				ProcessBranch(0x01);
				break;
			case Token.JNZ:
				ProcessBranch(0x05);
				break;
			case Token.JO:
				ProcessBranch(0x00);
				break;
            case Token.JPE:
                ProcessBranch(0x0A);
                break;
            case Token.JPO:
                ProcessBranch(0x0B);
                break;
			case Token.JS:
				ProcessBranch(0x08);
				break;
			case Token.JZ:
				ProcessBranch(0x04);
				break;
			case Token.LDS:
				ProcessLxs(-1, 0xC5);
				break;
			case Token.LEA:
				ProcessLea();
				break;
			case Token.LES:
				ProcessLxs(-1, 0xC4);
				break;
			case Token.LEAVE:
				ProcessLeave();
				break;
			case Token.LOOP:
				ProcessLoop(2);
				break;
			case Token.LOOPE:
				ProcessLoop(1);
				break;
			case Token.LOOPNE:
				ProcessLoop(0);
				break;
			case Token.LSS:
				ProcessLxs(0x0F, 0xB2);
				break;
			case Token.MODEL:
				ProcessModel();
				break;
			case Token.MOV:
				ProcessMov();
				break;
			case Token.MOVSX:
				ProcessMovx(0xBE);
				break;
			case Token.MOVZX:
				ProcessMovx(0xB6);
				break;
			case Token.SETNZ:
				ProcessSetCc(0x05);
				break;
			case Token.SETZ:
				ProcessSetCc(0x04);
				break;
			case Token.STC:
                asm.Stc();
				break;
			case Token.STOSB:
                asm.Stosb();
				break;
            case Token.STOSW:
                asm.Stosw();
				break;
            case Token.STOSD:
                asm.Stosd();
                break;
			case Token.LODSB:
                asm.Lodsb();
				break;
			case Token.LODSW:
                asm.Lodsw();
				break;
			case Token.LODSD:
                asm.Lodsd();
				break;
            case Token.SCASB:
                asm.Scasb();
                break;
            case Token.SCASW:
                asm.Scasw();
                break;
            case Token.SCASD:
                asm.Scasd();
                break;
            case Token.MOVSB:
                asm.Movsb();
                break;
            case Token.MOVSW:
                asm.Movsw();
                break;
            case Token.MOVSD:
                asm.Movsd();
                break;
            case Token.CMPSB:
                asm.Cmpsb();
                break;
            case Token.CMPSW:
                asm.Cmpsw();
                break;
            case Token.CMPSD:
                asm.Cmpsd();
                break;
            case Token.MUL:
				ProcessMul();
				break;
			case Token.NEG:
				ProcessUnary(0x03);
				break;
			case Token.NOT:
				ProcessUnary(0x02);
				break;
			case Token.OUT:
				ProcessInOut(true);
				break;
			case Token.POP:
				ProcessPushPop(true);
				break;
			case Token.PUSH:
				ProcessPushPop(false);
				break;
			case Token.RCL:
				ProcessShiftRotation(2);
				break;
			case Token.RCR:
				ProcessShiftRotation(3);
				break;
			case Token.REP:
                asm.Rep();
				ProcessLine();
				return;
            case Token.RET:
                ProcessRet(false);
                break;
            case Token.RETF:
                ProcessRet(true);
                break;

			case Token.ROL:
				ProcessShiftRotation(0x00);
				break;
			case Token.ROR:
				ProcessShiftRotation(0x01);
				break;

			case Token.OR:
				ProcessBinop(0x01);
				break;
			case Token.SAR:
				ProcessShiftRotation(0x07);
				break;
			case Token.SBB:
				ProcessBinop(0x03);
				break;
			case Token.SHL:
				ProcessShiftRotation(0x04);
				break;
			case Token.SHLD:
				ProcessDoubleShift(0x00);
				break;
			case Token.SHR:
				ProcessShiftRotation(0x05);
				break;
			case Token.SHRD:
				ProcessDoubleShift(0x04);
				break;
			case Token.SUB:
				ProcessBinop(0x05);
				break;
			case Token.TEST:
				ProcessTest();
				break;
			case Token.TITLE:
				ProcessTitle();
				break;
            case Token.XCHG:
                ProcessXchg();
                break;
            case Token.XOR:
				ProcessBinop(0x06);
				break;
			case Token.CMP:
				ProcessBinop(0x07);
				break;
			}
            if (lexer.PeekToken() != Token.EOFile)
            {
                Expect(Token.EOL);
            }
		}

		private void ProcessLxs(int prefix, int b)
		{
			ParsedOperand []? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessLxs(prefix, b, ops);
        }

		public void ProcessModel()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessMov()
		{
            var ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessMov(ops);
        }

		private void ProcessMovx(int opcode)
		{
			ParsedOperand []? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessMovx(opcode, ops);
		}

		private void ProcessMul()
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.Mul(ops[0]);
		}

		private void ProcessUnary(int operation)
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.ProcessUnary(operation, ops[0]);
		}

		public void ProcessPublic()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessPushPop(bool fPop)
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.ProcessPushPop(fPop, ops[0]);
        }

		private void ProcessRet(bool far)
		{
            int n = 0;
			if (lexer.PeekToken() == Token.INTEGER)
			{
				n = lexer.Integer;
				lexer.DiscardToken();
			}
            if (far)
            {
                asm.Retf(n);
            }
            else
            {
                asm.Ret(n);
            }
        }

		public void ProcessSegment()
		{
			lexer.SkipUntil(Token.EOL);
		}

        private void ProcessSetCc(byte bits)
        {
            ParsedOperand[]? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.ProcessSetCc(bits, ops[0]);
        }

		private void ProcessShiftRotation(byte bits)
		{
			ParsedOperand []? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.ProcessShiftRotation(bits, ops[0], ops[1]);
        }

		private void ProcessDoubleShift(byte bits)
		{
			ParsedOperand[]? ops = ParseOperandList(3);
            if (ops is null)
                return;
            asm.ProcessDoubleShift(bits, ops[0], ops[1], ops[2]);
		}

		private void ProcessFstsw()
		{
			ParsedOperand []? ops = ParseOperandList(1);
            if (ops is null)
                return;
            asm.Fstsw(ops[0]);
        }

        private void ProcessXchg()
        {
            ParsedOperand[]? ops = ParseOperandList(2);
            if (ops is null)
                return;
            asm.Xchg(ops);
        }
    }
}

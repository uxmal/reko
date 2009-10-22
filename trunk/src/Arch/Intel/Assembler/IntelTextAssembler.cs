/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Decompiler.Arch.Intel.Assembler
{
	/// <summary>
	/// A crude MASM-style assembler for x86 opcodes.
	/// </summary>
	public class IntelTextAssembler : Decompiler.Core.Assembler
	{
		private Lexer lexer;
		private Emitter emitter;
		private Symbol m_symOrigin;
		private Address addrBase;
		private Address addrStart;
		private SortedDictionary<string, SignatureLibrary> importLibraries;
		private Program prog;
		private ModRmBuilder modRm;
		private List<EntryPoint> entryPoints;
        private IntelAssembler asm;

		public IntelTextAssembler()
		{
			importLibraries = new SortedDictionary<string, SignatureLibrary>();
			m_symOrigin = null;
		}

		public override ProgramImage Assemble(Program prog, Address addr, string file, List<EntryPoint> entryPoints)
		{
			this.prog = prog;
			this.entryPoints = entryPoints;
			using (StreamReader rdr = new StreamReader(file))
			{
				addrBase = addr;
				return Assemble(rdr);
			}
		}

		public override ProgramImage AssembleFragment(Program prog, Address addr, string fragment)
		{
			this.prog = prog;
			addrBase = addr;
			return Assemble(new StringReader(fragment));
		}

		private ProgramImage Assemble(TextReader rdr)
		{
			lexer = new Lexer(rdr);
			emitter = new Emitter();

			// Default assembler is real-mode.

			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            asm = new IntelAssembler(prog, prog.Architecture.WordWidth, addrBase, emitter, entryPoints);

			// Assemblers are strongly line-oriented.

			while (lexer.PeekToken() != Token.EOFile)
			{
				ProcessLine();
			}
			
			Symbol [] syms = asm.SymbolTable.GetUndefinedSymbols();
			if (syms.Length > 0)
			{
				ReportUnresolvedSymbols(syms);
			}
			if (m_symOrigin != null && m_symOrigin.fResolved)
			{
				addrStart = new Address(addrBase.Selector, (ushort) m_symOrigin.offset);
			}
			else
			{
				addrStart = addrBase;
			}
			return new ProgramImage(addrBase, emitter.Bytes);
		}
        [Obsolete]
        private void DefineSymbol(string pstr)
        {
            asm.DefineSymbol(pstr);
        }

        private void EmitOffset(Constant v)
		{
			if (v == null)
				return;
			emitter.EmitInteger((PrimitiveType)v.DataType, v.ToInt32());
		}

		private void EmitModRM(int reg, ParsedOperand op)
		{
            asm.EmitModRM(reg, op);
		}

		private void EmitModRM(int reg, ParsedOperand op, byte b)
		{
            asm.EmitModRM(reg, op, b);
		}


		private void EmitModRM(int reg, MemoryOperand memOp, Symbol sym)
		{
            asm.EmitModRM(reg, memOp, sym);
		}

		private void EmitModRM(int reg, MemoryOperand memOp, byte b, Symbol sym)
		{
            asm.EmitModRM(reg, memOp, b, sym);
		}

        [Obsolete]
		private void EmitRelativeTarget(string target, PrimitiveType offsetSize)
		{
			int offBytes = (int) offsetSize.Size;
			switch (offBytes)
			{
			case 1: emitter.EmitByte(-(emitter.Length + 1)); break;
			case 2: emitter.EmitWord(-(emitter.Length + 2)); break;
			case 4: emitter.EmitDword((uint)-(emitter.Length + 4)); break;
			}
			ReferToSymbol(asm.SymbolTable.CreateSymbol(lexer.StringLiteral),
				emitter.Length - offBytes, offsetSize);
		}

		private PrimitiveType EnsureValidOperandSize(ParsedOperand op)
		{
            return asm.EnsureValidOperandSize(op);
		}

        [Obsolete]
		private PrimitiveType EnsureValidOperandSizes(ParsedOperand [] ops, int count)
		{
            return asm.EnsureValidOperandSizes(ops, count);
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

        [Obsolete]
		private bool IsSignedByte(int n) 
		{
			return -128 <= n && n < 128; 
		}


		public ParsedOperand [] ParseOperandList(int count)
		{
			return ParseOperandList(count, count);
		}

		public ParsedOperand [] ParseOperandList(int min, int max)
		{
            OperandParser opp = asm.CreateOperandParser(lexer);
            List<ParsedOperand> ops = new List<ParsedOperand>();
			emitter.SegmentOverride = MachineRegister.None;
			emitter.AddressWidth = emitter.SegmentAddressWidth;
			if (lexer.PeekToken() != Token.EOL)
			{
				ops.Add(opp.ParseOperand());
				if (opp.SegmentOverride != MachineRegister.None)
					emitter.SegmentOverride = opp.SegmentOverride;
				if (opp.AddressWidth != null)
					emitter.AddressWidth = opp.AddressWidth;

				while (lexer.PeekToken() == Token.COMMA)
				{
					lexer.DiscardToken();
					opp.DataWidth = ops[0].Operand.Width;
					ops.Add(opp.ParseOperand());
					if (opp.SegmentOverride != MachineRegister.None)
					{
						if (emitter.SegmentOverride != MachineRegister.None)
							Error("Can't have two segment overrides in one instruction");
						emitter.SegmentOverride = opp.SegmentOverride;
					}
					if (opp.AddressWidth != null)
						emitter.AddressWidth = opp.AddressWidth;
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
					Error(string.Format("Instruction expects {0} operand(s)", min));
				else
					Error(string.Format("Instruction expects between {0} and {1} operand(s)", min, max));
				return null;
			}
		}

        [Obsolete]
		private int IsAccumulator(MachineRegister reg)
		{
			return (reg == Registers.eax || reg == Registers.ax || reg == Registers.al) ? 1 : 0;
		}

		private int IsWordWidth(MachineOperand op)
		{
			return IsWordWidth(op.Width);
		}

		private int IsWordWidth(PrimitiveType width)
		{
			if (width == null)
				Error("Operand width is undefined");
			if (width.Size == 1)
				return 0;
			return 1;
		}

		public void ProcessAssume()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessBinop(int binop)
		{
			ParsedOperand [] ops = ParseOperandList(2);
            asm.ProcessBinop(binop, ops);
        }
		
		private void ProcessBitOp()
		{
			ParsedOperand [] ops = ParseOperandList(2);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			ImmediateOperand imm2 = ops[1].Operand as ImmediateOperand;
			if (imm2 != null)
			{
				emitter.EmitOpcode(0x0F, dataWidth);
				emitter.EmitByte(0xBA);
				EmitModRM(0x04, ops[0]);
				emitter.EmitByte(imm2.Value.ToInt32());
			}
			else
			{
				emitter.EmitOpcode(0x0F, dataWidth);
				emitter.EmitByte(0xA3);
				EmitModRM(RegisterEncoding(((RegisterOperand) ops[1].Operand).Register), ops[0]);
			}
		}

		private void ProcessBitScan(byte opCode)
		{
			ParsedOperand [] ops = ParseOperandList(2);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[1]);
			RegisterOperand regDst = ops[0].Operand as RegisterOperand;
			if (regDst == null)
				Error("First operand of bit scan instruction must be a register");
			emitter.EmitOpcode(0x0F, dataWidth);
			emitter.EmitByte(opCode);
			EmitModRM(RegisterEncoding(regDst.Register), ops[1]); 
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
			emitter.EmitOpcode(0xE0 | opcode, null);
			emitter.EmitByte(-(emitter.Length + 1));
			ReferToSymbol(asm.SymbolTable.CreateSymbol(lexer.StringLiteral),
				emitter.Length - 1, PrimitiveType.Byte);
		}

		private void ProcessCallJmp(int direct, int indirect)
		{
			//$NYI: No CALLF/JMPF support
			Token token = lexer.PeekToken();
			switch (token)
			{
			default:
				Error("Unexpected token: " + token);
				break;
			case Token.BRA:
			case Token.DWORD:
			case Token.WORD:
			case Token.REGISTER:
			{
				// Indirect jump.
				ParsedOperand [] ops = ParseOperandList(1);
				emitter.EmitOpcode(0xFF, emitter.SegmentDataWidth);
				EmitModRM(indirect, ops[0]);
				break;
			}
			case Token.ID:
				lexer.DiscardToken();
                asm.ProcessCallJmp(direct, lexer.StringLiteral);
				break;
			}
		}

		public void ProcessComm()
		{
			Expect(Token.ID);
			DefineSymbol(lexer.StringLiteral);
			Expect(Token.COLON);
			Token t = lexer.GetToken();
			switch (t)
			{
			case Token.DWORD:
				emitter.EmitDword(0);
				break;
			default:
				Error("Unexpected token: " + t);
				break;
			}
		}

		private void ProcessCwd(PrimitiveType width)
		{
			emitter.EmitOpcode(0x99, width);
		}

		private void ProcessDB()
		{
			for (;;)
			{
				Token tok = lexer.GetToken();
				switch (tok)
				{
				case Token.INTEGER:
					emitter.EmitByte(lexer.Integer);
					if (lexer.PeekToken() == Token.DUP)
					{
						lexer.DiscardToken();
						Expect(Token.INTEGER, "expected count to follow 'dup'");
						emitter.EmitBytes(0, lexer.Integer);
					}
					break;
				case Token.STRINGLITERAL:
					emitter.EmitString(lexer.StringLiteral);
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
            ParsedOperand[] ops = ParseOperandList(2);
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
					emitter.EmitInteger(width, lexer.Integer);
					break;
				case Token.ID:
				{
					Symbol sym = asm.SymbolTable.CreateSymbol(lexer.StringLiteral);
					emitter.EmitInteger(width, (int)addrBase.Offset);
					ReferToSymbol(sym, emitter.Length - (int) width.Size, emitter.SegmentAddressWidth);
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
			emitter.EmitOpcode(0xC8, null);
			emitter.EmitWord(cbStack);
			emitter.EmitByte(nLevel);
		}

		private void ProcessIncDec(bool fDec)
		{
			ParsedOperand [] ops = ParseOperandList(1);
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
			ParsedOperand [] ops = ParseOperandList(1);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			emitter.EmitOpcode(0xF6|IsWordWidth(dataWidth), dataWidth);
			EmitModRM(operation, ops[0]);
		}

		private void ProcessExtrn()
		{
			Expect(Token.ID);
			DefineSymbol(lexer.StringLiteral);
			AddImport(lexer.StringLiteral, null);
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessFild()
		{
			ParsedOperand [] ops = ParseOperandList(1);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			int opCode;
			int reg;
			switch (dataWidth.Size)
			{
			case 2: opCode = 0xDF; reg = 0x00; break;
			case 4: opCode = 0xDD; reg = 0x00; break;
			case 8: opCode = 0xDF; reg = 0x05; break;
			default: Error(string.Format("Instruction doesn't support {0}-byte operands", dataWidth.Size)); return;
			}
			emitter.EmitOpcode(opCode, dataWidth);
			EmitModRM(reg, ops[0]);
		}

		private void ProcessFistp()
		{
			ParsedOperand [] ops = ParseOperandList(1);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			int opCode;
			int reg;
			switch (dataWidth.Size)
			{
			case 2: opCode = 0xDF; reg = 0x03; break;
			case 4: opCode = 0xDB; reg = 0x03; break;
			case 8: opCode = 0xDF; reg = 0x07; break;
			default: Error(string.Format("Instruction doesn't support {0}-byte operands", dataWidth.Size)); return;
			}
			emitter.EmitOpcode(opCode, null);
			EmitModRM(reg, ops[0]);
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
			ParsedOperand [] ops = ParseOperandList(0,2);
			if (ops.Length == 0)
			{
				ops = new ParsedOperand[] { new ParsedOperand(new FpuOperand(0)), new ParsedOperand(new FpuOperand(1)) };
			}
			else if (ops.Length == 1 && ops[0].Operand is FpuOperand)
			{
				ops = new ParsedOperand[] { new ParsedOperand(new FpuOperand(0)), ops[0] };
			}

			FpuOperand fop1 = ops[0].Operand as FpuOperand;
			FpuOperand fop2 = ops.Length > 1 ? ops[1].Operand as FpuOperand : null;
			MemoryOperand mop = ops[0].Operand as MemoryOperand;
			if (mop == null && ops.Length > 1)
				mop = ops[1].Operand as MemoryOperand;
			if (mop != null)
			{
				emitter.EmitOpcode(opcodeMem|(mop.Width == PrimitiveType.Word64?4:0), null);
				EmitModRM(fpuOperation, mop, null);
				return;
			}
			if (isPop)
			{
				if (fop1 == null)
					Error("First operand must be of type ST(n)");
				emitter.EmitOpcode(opcodeFreg, null);
				if (fixedOrder)
					fpuOperation ^= 1;
				if (fop1.StNumber == 0)
					EmitModRM(fpuOperation, new ParsedOperand(fop2, null));
				else
					EmitModRM(fpuOperation, new ParsedOperand(fop1, null));
				return;
			}
			if (fop1 != null)
			{
				fop2 = (FpuOperand) ops[1].Operand;
				if (fop1.StNumber != 0)
				{
					if (fop2.StNumber != 0)
						Error("at least one of the floating point stack arguments must be ST(0)");
					if (fixedOrder)
						fpuOperation ^= 1;
					fop2 = fop1;
				}
				emitter.EmitOpcode(opcodeFreg, null);
				EmitModRM(fpuOperation, new ParsedOperand(fop2, null));
				return;
			}
			throw new NotImplementedException("NYI");
		}


		private void ProcessFst(bool pop)
		{
			ParsedOperand [] ops = ParseOperandList(1);
			FpuOperand fop = ops[0].Operand as FpuOperand;
			MemoryOperand mop = ops[0].Operand as MemoryOperand;
			int regBits = pop ? 3 : 2;
			if (mop != null)
			{
				switch (mop.Width.Size)
				{
				case 4: emitter.EmitOpcode(0xD9, null); break;
				case 8: emitter.EmitOpcode(0xDD, null); break;
				default: Error("Unexpected operator width"); break;
				}
				EmitModRM(regBits, ops[0]);
			}
			else if (fop != null)
			{
				emitter.EmitOpcode(0xDD, null);
				EmitModRM(regBits, new ParsedOperand(fop, null));
			}
			else
				Error("Unexpected operator type");
		}

		private void ProcessImport()
		{
			Expect(Token.STRINGLITERAL);
			string fnName = lexer.StringLiteral;
			Expect(Token.COMMA);
			Expect(Token.STRINGLITERAL);
			string dll = lexer.StringLiteral.ToLower();


			SignatureLibrary lib;
            if (!importLibraries.TryGetValue(dll, out lib))
			{
				lib = new SignatureLibrary(prog.Architecture);
				lib.Load(Path.ChangeExtension(dll, ".xml"));
				importLibraries[dll] = lib;
			}
			AddImport(fnName, lib.Lookup(fnName));
		}

		public void AddImport(string fnName, ProcedureSignature sig)
		{
			uint u = (uint) (addrBase.Linear + emitter.Position);
			prog.ImportThunks.Add(u, new PseudoProcedure(fnName, sig));
			emitter.EmitDword(0);
		}

		private void ProcessImul()
		{
			ParsedOperand [] ops = ParseOperandList(1, 3);
            asm.ProcessImul(ops);
        }


		private void ProcessInOut(bool fOut)
		{
			ParsedOperand [] ops = ParseOperandList(2);
			ParsedOperand opPort;
			ParsedOperand opData;

			if (fOut)
			{
				opPort = ops[0];
				opData = ops[1];
			}
			else
			{
				opData = ops[0];
				opPort = ops[1];
			}

			RegisterOperand regOpData = opData.Operand as RegisterOperand;
			if (regOpData == null || IsAccumulator(regOpData.Register) == 0)
				throw new ApplicationException("invalid register for in or out instruction");

			int opcode = IsWordWidth(regOpData) | (fOut ? 0xE6 : 0xE4);

			RegisterOperand regOpPort = opPort.Operand as RegisterOperand;
			if (regOpPort != null)
			{
				if (regOpPort.Register == Registers.dx || regOpPort.Register == Registers.edx)
				{
					emitter.EmitOpcode(8|opcode, regOpPort.Width);
				}
				else
					throw new ApplicationException("port must be specified with 'immediate', dx, or edx register");
				return;
			}

			ImmediateOperand immOp = opPort.Operand as ImmediateOperand;
			if (immOp != null)
			{
				if (immOp.Value.ToUInt32() > 0xFF)
				{
					throw new ApplicationException("port number must be between 0 and 255");
				}
				else
				{
					emitter.EmitOpcode(opcode, regOpPort.Width);
					emitter.EmitByte(immOp.Value.ToInt32());
				}
			}
			else
			{
				throw new ApplicationException("port must be specified with 'immediate', dx, or edx register");
			}
		}

		private void ProcessInt()
		{
			ParsedOperand [] ops = ParseOperandList(1);
			ImmediateOperand op = (ImmediateOperand) ops[0].Operand;
			emitter.EmitOpcode(0xCD, null);
			emitter.EmitByte(op.Value.ToInt32());
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
				DefineSymbol(s);
				lexer.DiscardToken();
				ProcessDB();
				break;
			case Token.DW:
				DefineSymbol(s);
				lexer.DiscardToken();
				ProcessWords(PrimitiveType.Word16);
				break;
			case Token.DD:
				DefineSymbol(s);
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
				asm.SymbolTable.Equates[s] = lexer.Integer;
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
				DefineSymbol(s);
				break;

			case Token.GROUP:
				DefineSymbol(s);
				lexer.SkipUntil(Token.EOL);
				lexer.DiscardToken();
				break;
			case Token.IMPORT:
				DefineSymbol(s);
				lexer.DiscardToken();
				ProcessImport();
				break;
			case Token.SEGMENT:
				ProcessSegment();
				lexer.DiscardToken();
				break;
			}
		}


		private void ProcessLea()
		{
			ParsedOperand [] ops = ParseOperandList(2);
			RegisterOperand ropLhs = (RegisterOperand) ops[0].Operand;
			emitter.EmitOpcode(0x8D, ropLhs.Width);
			EmitModRM(RegisterEncoding(ropLhs.Register), ops[1]);
		}

		private void ProcessLeave()
		{
			emitter.EmitOpcode(0xC9, null);
		}

		private void ProcessLine()
		{
			emitter.AddressWidth = null;
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
			{
                asm.i86();
				lexer.SkipUntil(Token.EOL);
				break;
			}
			case Token.i386:
			case Token.i386p:
				prog.Architecture = new IntelArchitecture(ProcessorMode.ProtectedFlat);
				asm.SetDefaultWordWidth(PrimitiveType.Word32);
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
				ProcessCallJmp(0xE8, 0x02);
				break;
			case Token.CDQ:
				ProcessCwd(PrimitiveType.Word32);
				break;
			case Token.CLC:
				emitter.EmitOpcode(0xF8, null);
				break;
			case Token.CMC:
				emitter.EmitOpcode(0xF5, null);
				break;
			case Token.CWD:
				ProcessCwd(PrimitiveType.Word16);
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
				emitter.EmitByte(0xD9);
				emitter.EmitByte(0xE8);
				break;
			case Token.FLDZ:
				emitter.EmitOpcode(0xD9, null);
				emitter.EmitByte(0xEE);
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
				emitter.EmitOpcode(0xE3, null);
				Expect(Token.ID);
				EmitRelativeTarget(lexer.StringLiteral, PrimitiveType.Byte);
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
				ProcessCallJmp(0xE9, 0x04);
				break;
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
			case Token.MOVSB:
				ProcessStringInstruction(0xA4, PrimitiveType.Byte);
				break;
			case Token.MOVSW:
				ProcessStringInstruction(0xA4, PrimitiveType.Word16);
				break;
			case Token.MOVSX:
				ProcessMovx(0xBE);
				break;
			case Token.MOVZX:
				ProcessMovx(0xB6);
				break;
			case Token.SCASB:
				ProcessStringInstruction(0xAE, PrimitiveType.Byte);
				break;
			case Token.SCASW:
				ProcessStringInstruction(0xAE, PrimitiveType.Word16);
				break;
			case Token.SETNZ:
				ProcessSetCc(0x05);
				break;
			case Token.SETZ:
				ProcessSetCc(0x04);
				break;
			case Token.STC:
				emitter.EmitOpcode(0xF9, null);
				break;
			case Token.STOSB:
				ProcessStringInstruction(0xAA, PrimitiveType.Byte);
				break;
			case Token.STOSW:
				ProcessStringInstruction(0xAA, PrimitiveType.Word16);
				break;
			case Token.LODSB:
				ProcessStringInstruction(0xAC, PrimitiveType.Byte);
				break;
			case Token.LODSW:
				ProcessStringInstruction(0xAC, PrimitiveType.Word16);
				break;
			case Token.LODSD:
				ProcessStringInstruction(0xAC, PrimitiveType.Word32);
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
				emitter.EmitByte(0xF3);
				ProcessLine();
				return;
			case Token.RET:
				ProcessRet();
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
			case Token.XOR:
				ProcessBinop(0x06);
				break;
			case Token.CMP:
				ProcessBinop(0x07);
				break;
			}
			Expect(Token.EOL);
		}

		private void ProcessLxs(int prefix, int b)
		{
			ParsedOperand [] ops = ParseOperandList(2);
			RegisterOperand opDst = (RegisterOperand) ops[0].Operand;

			if (prefix > 0)
			{
				emitter.EmitOpcode(prefix, opDst.Width);
				emitter.EmitByte(b);
			}
			else
				emitter.EmitOpcode(b, emitter.SegmentDataWidth);
			EmitModRM(RegisterEncoding(opDst.Register), ops[1]);
		}

		public void ProcessModel()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessMov()
		{
            asm.ProcessMov(ParseOperandList(2));
        }


		private void ProcessMovx(int opcode)
		{
			ParsedOperand [] ops = ParseOperandList(2);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[1]);

			RegisterOperand regDst = ops[0].Operand as RegisterOperand;
			if (regDst == null)
				Error("First operand must be a register");
			emitter.EmitOpcode(0x0F, regDst.Width);
			emitter.EmitByte(opcode | IsWordWidth(dataWidth));
			EmitModRM(RegisterEncoding(regDst.Register), ops[1]);
		}

		private void ProcessMul()
		{
			ParsedOperand [] ops = ParseOperandList(1);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			// Single operand doesn't accept immediate values.
			if (ops[0].Operand is ImmediateOperand)
				Error("Immediate operand not allowed for single-argument multiplication");

			emitter.EmitOpcode(0xF6 | IsWordWidth(dataWidth), dataWidth);
			EmitModRM(4, ops[0]);
		}

		private void ProcessUnary(int operation)
		{
			ParsedOperand [] ops = ParseOperandList(1);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			emitter.EmitOpcode(0xF6 | IsWordWidth(dataWidth), dataWidth);
			EmitModRM(operation, ops[0]);
		}

		public void ProcessPublic()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessPushPop(bool fPop)
		{
			ParsedOperand [] ops = ParseOperandList(1);
            asm.ProcessPushPop(fPop, ops[0]);

        }



		private void ProcessRet()
		{
			//$BUGBUG: far / near
			if (lexer.PeekToken() == Token.INTEGER)
			{
				int n = lexer.Integer;
				lexer.DiscardToken();
				if (n != 0)
				{
                    asm.Ret(n);
				}
				else
				{
                    asm.Ret();
				}
			}
			else
			{
                asm.Ret();
			}
		}

		public void ProcessSegment()
		{
			lexer.SkipUntil(Token.EOL);
		}

		private void ProcessSetCc(byte bits)
		{
			ParsedOperand [] ops = ParseOperandList(1);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			if (dataWidth != PrimitiveType.Byte)
				Error("Instruction takes only a byte operand");
			emitter.EmitOpcode(0x0F, dataWidth);
			emitter.EmitByte(0x90 | (bits & 0xF));
			EmitModRM(0, ops[0]);
		}

		private void ProcessShiftRotation(byte bits)
		{
			ParsedOperand [] ops = ParseOperandList(2);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			
			ImmediateOperand immOp = ops[1].Operand as ImmediateOperand;
			if (immOp != null)
			{
				int imm = immOp.Value.ToInt32();
				if (imm == 1)
				{
					emitter.EmitOpcode(0xD0 | IsWordWidth(dataWidth), dataWidth);
					EmitModRM(bits, ops[0]);
				}
				else
				{
					emitter.EmitOpcode(0xC0 | IsWordWidth(dataWidth), dataWidth);
					EmitModRM(bits, ops[0], (byte) immOp.Value.ToInt32());
				}
				return;
			}

			RegisterOperand regOp = ops[1].Operand as RegisterOperand;
			if (regOp != null && regOp.Register == Registers.cl)
			{
				emitter.EmitOpcode(0xD2 | IsWordWidth(dataWidth), dataWidth);
				EmitModRM(bits, ops[0]);
				return;
			}

			throw new ApplicationException("Shift/rotate instructions must be followed by a constant or CL");
		}

		private void ProcessDoubleShift(byte bits)
		{
			ParsedOperand [] ops = ParseOperandList(3);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);

			RegisterOperand regSrc = ops[1].Operand as RegisterOperand;
			if (regSrc == null)
				Error("Second operand of SHLD/SHRD must be a register");

			ImmediateOperand immShift = ops[2].Operand as ImmediateOperand;
			RegisterOperand regShift = ops[2].Operand as RegisterOperand;
			if (regShift != null && regShift.Register == Registers.cl)
			{
				bits |= 0x01;
			}
			else if (immShift == null)
			{
				Error("SHLD/SHRD instruction must be followed by a constant or CL");
			}

			emitter.EmitOpcode(0x0F, dataWidth);
			emitter.EmitByte(0xA4|bits);
			EmitModRM(RegisterEncoding(regSrc.Register), ops[0]);
			if (immShift != null)
				emitter.EmitByte((byte)immShift.Value.ToUInt32());
		}

		private void ProcessFstsw()
		{
			ParsedOperand [] ops = ParseOperandList(1);
			PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
			RegisterOperand regOp = ops[0].Operand as RegisterOperand;
			if (regOp != null)
			{
				if (regOp.Register != Registers.ax)
					Error("Register operand must be AX");
				emitter.EmitOpcode(0xDF, dataWidth);
				emitter.EmitByte(0xE0);
			}
			MemoryOperand mop = ops[0].Operand as MemoryOperand;
			if (mop != null)
			{
				if (dataWidth != PrimitiveType.Word16)
					Error("Destination must be two bytes");
				emitter.EmitOpcode(0xDD, dataWidth);
				EmitModRM(0x07, ops[0]);
			}
		}

		private void ProcessStringInstruction(byte opcode, PrimitiveType width)
		{
			emitter.EmitOpcode(opcode|IsWordWidth(width), width);
		}

        [Obsolete]
		public static byte RegisterEncoding(byte b)
		{
			return IntelAssembler.RegisterEncoding(b);
		}

        [Obsolete]
		public static byte RegisterEncoding(MachineRegister reg)
		{
            return IntelAssembler.RegisterEncoding(reg);
		}

		public void ReportUnresolvedSymbols(Symbol [] s)
		{
			StringWriter writer = new StringWriter();
			writer.WriteLine("The following symbols were undefined:");
			for (int i = 0; i < s.Length; ++i)
			{
				writer.WriteLine("  {0}", s[i].ToString());
			}
			throw new ApplicationException(writer.ToString());
		}

        [Obsolete]
        public void ResolveSymbol(Symbol psym)
        {
            asm.ResolveSymbol(psym);
        }


		private void ReferToSymbol(Symbol psym, int off, DataType width)
		{
			if (psym.fResolved)
			{
				emitter.Patch(off, psym.offset, width);
			}
			else
			{
				// Add forward references to the backpatch list.
				psym.AddForwardReference(off, width);
			}
		}

		public override Address StartAddress
		{
			get { return addrStart; }
		}


		private void modRm_Error(object sender, ErrorEventArgs args)
		{
			args.LineNumber = lexer.LineNumber;
			Error(args.Message);
		}
	}
}

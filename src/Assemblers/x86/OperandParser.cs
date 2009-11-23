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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Assemblers.x86
{
	/// <summary>
	/// This class is in charge of parsing an operand or several operands and 
	/// building instances of the ParsedOperand class.
	/// </summary>
	public class OperandParser
	{
		public event ErrorEventHandler Error;

		private Lexer lexer;
		private PrimitiveType addrWidth;
		private Symbol sym;
		private SymbolTable symtab; 
		private Address addrBase;
		private PrimitiveType defaultWordWidth;
		private PrimitiveType defaultAddressWidth;
		private MachineRegister segOverride;
		private int totalInt;

		public OperandParser(Lexer lexer, SymbolTable symtab, Address addrBase, PrimitiveType defaultWordWidth, PrimitiveType defaultAddressWidth)
		{
			this.lexer = lexer;
			this.symtab = symtab;
			this.addrBase = addrBase;
			this.defaultWordWidth = defaultWordWidth;
			this.defaultAddressWidth = defaultAddressWidth;
		}

		public PrimitiveType AddressWidth
		{
			get { return addrWidth; }
		}

		public PrimitiveType DataWidth
		{
			get { return defaultWordWidth; }
			set { defaultWordWidth = value; }
		}

		private void Expect(Token tok)
		{
			if (lexer.GetToken() != tok)
			{
				OnError(string.Format("Expected token: '{0}'", tok));
			}
		}

		private void Expect(Token tok, string name)
		{
			if (lexer.GetToken() != tok)
			{
				OnError(string.Format("Expected token: '{0}'", name));
			}
		}

		private FpuOperand ParseFpuOperand()
		{
			if (lexer.PeekToken() == Token.LPAREN)
			{
				Expect(Token.LPAREN);
				Expect(Token.INTEGER, "integer");
				if (0 <= lexer.Integer && lexer.Integer < 8)
				{
					int f = lexer.Integer;
					Expect(Token.RPAREN);
					return new FpuOperand(f);
				}
				throw new ApplicationException("FPU register must be between 0 and 7");
			}
			else
				return new FpuOperand(0);
		}

		private void ParseMemoryFactor(MemoryOperand memOp)
		{
			Token token = lexer.GetToken();
			MachineRegister reg = MachineRegister.None;
			switch (token)
			{
			default:
				OnError("unexpected token: " + token);
				return;
			case Token.INTEGER:
				totalInt += lexer.Integer;
				break;
			case Token.REGISTER:
			{
				reg = lexer.Register;
 				PrimitiveType width = reg.DataType;
 				if (addrWidth == null)
 					addrWidth = width;
 				else if (addrWidth != width)
 					throw new ApplicationException("Conflicting address widths");
 				break;
			}
			case Token.ID:
			{
				int v;
                if (symtab.Equates.TryGetValue(lexer.StringLiteral.ToLower(), out v))
				{
                    totalInt += v;
				}
				else
				{
					sym = symtab.CreateSymbol(lexer.StringLiteral);
					totalInt += unchecked((int) addrBase.Offset);
				}
				break;
			}
			}

			if (lexer.PeekToken() == Token.TIMES)
			{
				if (reg == MachineRegister.None)
					throw new ApplicationException("Scale factor must be preceded by a register");
				lexer.GetToken();
				if (memOp.Index != MachineRegister.None)
					throw new ApplicationException("Scale can only be used once in an addressing form");
				Expect(Token.INTEGER, "Expected an integer scale");
				if (lexer.Integer != 1 && lexer.Integer != 2 && lexer.Integer != 4 && lexer.Integer != 8)
					throw new ApplicationException("Only scales 1, 2, 4, and 8 are supported");
				memOp.Scale = (byte) lexer.Integer;
				memOp.Index = reg;
			}
			else if (reg != MachineRegister.None)
			{
				if (memOp.Base == MachineRegister.None)
					memOp.Base = reg;
				else if (memOp.Index == MachineRegister.None)
				{
					memOp.Index = reg;
					memOp.Scale = 1;
				}
				else
					throw new ApplicationException("Can't have more than two registers in an addressing form");
			}
		}

		private ParsedOperand ParseMemoryOperand(MachineRegister segOver)
		{
			MemoryOperand memOp = new MemoryOperand(null);
			memOp.SegOverride = segOver;
			this.segOverride = segOver;

			ParseMemoryFactor(memOp);
			for (;;)
			{
				Token token = lexer.GetToken();
				switch (token)
				{
				default: 
					OnError("Unexpected token: " + token);
					return null;
				case Token.KET:
					if (totalInt != 0 || sym != null)
					{
						if (addrWidth == null || sym != null)
							memOp.Offset = new Constant(defaultAddressWidth, totalInt);
						else
							memOp.Offset = IntelAssembler.IntegralConstant(totalInt, addrWidth);
					}
					return new ParsedOperand(memOp, sym);
				case Token.PLUS:
					break;
				case Token.MINUS:
					Expect(Token.INTEGER);
					totalInt -= lexer.Integer;
					continue;
				case Token.ID:
					break;
				}
				ParseMemoryFactor(memOp);
			} 
		}


		public ParsedOperand ParseOperand()
		{
			sym = null;
			totalInt = 0;
			segOverride = MachineRegister.None;
			Token token = lexer.GetToken();
			switch (token)
			{
			default:
				OnError(string.Format("Unexpected token '{0}'", token));
				return null;
			case Token.BRA:
				return ParseMemoryOperand(MachineRegister.None);
			case Token.MINUS:
				Expect(Token.INTEGER);
				totalInt -= lexer.Integer;
				goto IntegerCommon;
			case Token.INTEGER:
				totalInt += lexer.Integer;
				IntegerCommon:
				if (lexer.PeekToken() == Token.BRA)
				{
					Expect(Token.BRA);
					return ParseMemoryOperand(MachineRegister.None);
				}
				else
				{
					return new ParsedOperand(new ImmediateOperand(IntelAssembler.IntegralConstant(totalInt, defaultWordWidth)));
				}
			case Token.REGISTER:
			{
				MachineRegister reg = lexer.Register;
				switch (lexer.PeekToken())
				{
				case Token.COLON:		// Segment override of the form "es:" usually precedes a memory operand.
					if (!(reg is SegmentRegister))
						throw new ApplicationException(reg.ToString() + " is not a segment register.");
					Expect(Token.COLON);			// Discard ':'
					Expect(Token.BRA);
					return ParseMemoryOperand(reg);
				default:
					return new ParsedOperand(new RegisterOperand(reg));
				}
			}
			case Token.OFFSET:
				Expect(Token.ID);
				return new ParsedOperand(
					new ImmediateOperand(new Constant(defaultWordWidth, addrBase.Offset)),
					symtab.CreateSymbol(lexer.StringLiteral));
				
			case Token.ID:
			{
                int v;
                if (symtab.Equates.TryGetValue(lexer.StringLiteral.ToLower(), out v))
                {
					totalInt += lexer.Integer;
					goto IntegerCommon;
				}
				return new ParsedOperand(
							   new MemoryOperand(addrWidth, new Constant(defaultWordWidth, addrBase.Offset)),
							   symtab.CreateSymbol(lexer.StringLiteral));
			}
			case Token.WORD:
				return ParsePtrOperand(PrimitiveType.Word16);
			case Token.BYTE:
				return ParsePtrOperand(PrimitiveType.Byte);
			case Token.DWORD:
				return ParsePtrOperand(PrimitiveType.Word32);
			case Token.QWORD:
				return ParsePtrOperand(PrimitiveType.Word64);
			case Token.ST:
				return new ParsedOperand(ParseFpuOperand(), null);
			}
		}


		private void OnError(string msg )
		{
			if (Error != null)
			{
				Error(this, new ErrorEventArgs(msg));
			}
		}
	
		private ParsedOperand ParsePtrOperand(PrimitiveType width)
		{
			Expect(Token.PTR);
			ParsedOperand op = ParseOperand();
			op.Operand.Width = width;
			return op;
		}

		public MachineRegister SegmentOverride
		{
			get { return segOverride; }
		}
	}
}

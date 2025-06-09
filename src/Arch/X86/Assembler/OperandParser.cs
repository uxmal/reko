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
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;

namespace Reko.Arch.X86.Assembler
{
	/// <summary>
	/// This class is in charge of parsing an operand or several operands and 
	/// building instances of the ParsedOperand class.
	/// </summary>
	public class OperandParser
	{
		public event ErrorEventHandler? Error;

		private readonly Lexer lexer;
		private DataType? addrWidth;
		private Symbol? sym;
		private SymbolTable symtab; 
		private readonly Address addrBase;
		private DataType defaultWordWidth;
		private PrimitiveType defaultAddressWidth;
		private RegisterStorage segOverride;
		private int totalInt;

		public OperandParser(Lexer lexer, SymbolTable symtab, Address addrBase, PrimitiveType defaultWordWidth, PrimitiveType defaultAddressWidth)
		{
			this.lexer = lexer;
			this.symtab = symtab;
			this.addrBase = addrBase;
			this.defaultWordWidth = defaultWordWidth;
			this.defaultAddressWidth = defaultAddressWidth;
            this.segOverride = RegisterStorage.None;
		}

		public DataType? AddressWidth
		{
			get { return addrWidth; }
		}

		public DataType DataWidth
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
			RegisterStorage reg = RegisterStorage.None;
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
				reg = lexer.Register!;
 				DataType width = reg.DataType;
 				if (addrWidth is null)
 					addrWidth = width;
 				else if (addrWidth != width)
 					throw new ApplicationException("Conflicting address widths");
 				break;
			}
			case Token.ID:
			{
                if (symtab.Equates.TryGetValue(lexer.StringLiteral.ToLower(), out int v))
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
				if (reg == RegisterStorage.None)
					throw new ApplicationException("Scale factor must be preceded by a register");
				lexer.GetToken();
				if (memOp.Index != RegisterStorage.None)
					throw new ApplicationException("Scale can only be used once in an addressing form");
				Expect(Token.INTEGER, "Expected an integer scale");
				if (lexer.Integer != 1 && lexer.Integer != 2 && lexer.Integer != 4 && lexer.Integer != 8)
					throw new ApplicationException("Only scales 1, 2, 4, and 8 are supported");
				memOp.Scale = (byte) lexer.Integer;
				memOp.Index = reg;
			}
			else if (reg != RegisterStorage.None)
			{
				if (memOp.Base == RegisterStorage.None)
					memOp.Base = reg;
				else if (memOp.Index == RegisterStorage.None)
				{
					memOp.Index = reg;
					memOp.Scale = 1;
				}
				else
					throw new ApplicationException("Can't have more than two registers in an addressing form");
			}
		}

		private ParsedOperand? ParseMemoryOperand(RegisterStorage segOver)
		{
            MemoryOperand memOp = new MemoryOperand(null!); // null will be replaced later.
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
					if (totalInt != 0 || sym is not null)
					{
						if (addrWidth is null || sym is not null)
							memOp.Offset = Constant.Create(defaultAddressWidth, totalInt);
						else
							memOp.Offset = X86Assembler.IntegralConstant(totalInt, addrWidth);
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

		private ParsedOperand? IntegerCommon() {
			if (lexer.PeekToken() == Token.BRA)
			{
				Expect(Token.BRA);
				return ParseMemoryOperand(RegisterStorage.None);
			}
			else
			{
				return new ParsedOperand(X86Assembler.IntegralConstant(totalInt, defaultWordWidth));
			}
		}

		public ParsedOperand? ParseOperand()
		{
			sym = null;
			totalInt = 0;
			segOverride = RegisterStorage.None;
			Token token = lexer.GetToken();
			switch (token)
			{
			default:
				OnError(string.Format("Unexpected token '{0}'", token));
				return null;
			case Token.BRA:
				return ParseMemoryOperand(RegisterStorage.None);
			case Token.MINUS:
				Expect( Token.INTEGER );
				totalInt -= lexer.Integer;
				return IntegerCommon();
			case Token.INTEGER:
				totalInt += lexer.Integer;
				return IntegerCommon();
			case Token.REGISTER:
			{
				RegisterStorage reg = lexer.Register!;
				switch (lexer.PeekToken())
				{
				case Token.COLON:		// Segment override of the form "es:" usually precedes a memory operand.
					if (!X86Assembler.IsSegmentRegister(reg))
						throw new ApplicationException(reg.ToString() + " is not a segment register.");
					Expect(Token.COLON);			// Discard ':'
					Expect(Token.BRA);
					return ParseMemoryOperand(reg);
				default:
					return new ParsedOperand(reg);
				}
			}
			case Token.OFFSET:
				Expect(Token.ID);
				return new ParsedOperand(
					Constant.Create(defaultWordWidth, addrBase.Offset),
					symtab.CreateSymbol(lexer.StringLiteral));
				
			case Token.ID:
                return ParseIdOperand(lexer.StringLiteral);
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
            case Token.STRINGLITERAL:
                if (this.DataWidth is null || string.IsNullOrEmpty(lexer.StringLiteral))
                    OnError("Unexpected string literal");
                if (this.DataWidth!.BitSize == 8)
                {
                    totalInt = (byte) lexer.StringLiteral[0];
                    return IntegerCommon();
                }
                OnError("Unexpected string literal");
                return null;
			}
		}

        public ParsedOperand? ParseIdOperand(string symStr)
        {
            if (symtab.Equates.TryGetValue(symStr.ToLower(), out int v))
            {
                totalInt += lexer.Integer;
                return IntegerCommon();
            }
            if (lexer.PeekToken() == Token.BRA)
            {
                lexer.GetToken();
                var memOp = ParseMemoryOperand(RegisterStorage.None);
                if (memOp is null)
                    return null;
                memOp.Symbol = symtab.CreateSymbol(symStr);
                return memOp;
            }
            return new ParsedOperand(
                           new MemoryOperand(addrWidth!, Constant.Create(defaultWordWidth, addrBase.Offset)),
                           symtab.CreateSymbol(symStr));
        }

        private void OnError(string msg)
		{
            Error?.Invoke(this, new ErrorEventArgs(msg));
        }
	
		private ParsedOperand? ParsePtrOperand(PrimitiveType width)
		{
			Expect(Token.PTR);
			ParsedOperand? op = ParseOperand();
            if (op is null)
                return null;
			op.Operand.DataType = width;
			return op;
		}

		public RegisterStorage SegmentOverride
		{
			get { return segOverride; }
		}
	}
}

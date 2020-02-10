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

using Reko.Arch.Pdp11;
using Reko.Assemblers.Pdp11;
using Reko.Core.Assemblers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Assemblers.Pdp11
{
    public class OperandParser
    {
        private Pdp11Architecture arch;
        private Lexer lexer;
        private Pdp11Assembler asm;

        public OperandParser(Pdp11Architecture arch, Lexer lexer, Pdp11Assembler asm)
        {
            this.arch = arch;
            this.lexer = lexer;
            this.asm = asm;
        }

        public ParsedOperand ParseOperand()
        {
            var tok = lexer.Get();
            switch (tok.Type)
            {
            case TokenType.Register:
                return new ParsedOperand
                {
                    Type = AddressMode.Register,
                    Register = arch.GetRegister((string) tok.Value)
                };
            case TokenType.Hash:
                tok = lexer.Get();
                switch (tok.Type)
                {
                case TokenType.Id:
                    return new ParsedOperand
                    {
                        Type = AddressMode.Immediate,
                        Symbol = asm.Symtab.CreateSymbol((string) tok.Value)
                    };
                default: 
                    lexer.Unexpected(tok); 
                    return null;
                }
            case TokenType.At:
                var op = ParseOperand();
                switch (op.Type)
                {
                case AddressMode.Register: op.Type = AddressMode.RegDef; break;
                case AddressMode.RegDef:
                case AddressMode.AutoIncrDef:
                case AddressMode.AutoDecrDef:
                case AddressMode.IndexedDef:
                    throw new FormatException("Can't re-defer.");
                case AddressMode.AutoIncr: op.Type = AddressMode.AutoIncrDef; break;
                case AddressMode.AutoDecr: op.Type = AddressMode.AutoDecrDef; break;
                case AddressMode.Indexed: op.Type = AddressMode.IndexedDef; break;
                }
                return op;
            case TokenType.Minus:
                lexer.Expect(TokenType.LParen);
                var regStr = (string) lexer.Expect(TokenType.Register);
                lexer.Expect(TokenType.RParen);
                return new ParsedOperand
                {
                    Type = AddressMode.AutoDecr,
                    Register = arch.GetRegister(regStr),
                };
            case TokenType.LParen:
                regStr = (string) lexer.Expect(TokenType.Register);
                lexer.Expect(TokenType.RParen);
                if (lexer.PeekAndDiscard(TokenType.Plus))
                {
                    return new ParsedOperand
                    {
                        Type = AddressMode.AutoIncr,
                        Register = arch.GetRegister(regStr)
                    };
                }
                else
                {
                    return new ParsedOperand
                    {
                        Type = AddressMode.RegDef,
                        Register = arch.GetRegister(regStr)
                    };
                }
            default:
                return new ParsedOperand
                {
                    Type = AddressMode.Absolute,
                    Symbol = asm.Symtab.CreateSymbol((string) tok.Value)
                };
            }
        }
    }
}

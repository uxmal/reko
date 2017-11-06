#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Pascal
{
    public class PascalParser
    {
        private PascalLexer lexer;

        public PascalParser(PascalLexer lexer)
        {
            this.lexer = lexer;
        }

        public void Parse()
        {
            while (lexer.Peek().Type != TokenType.EOF)
            {
                var tok = lexer.Read();
                switch (tok.Type)
                {
                case TokenType.Unit:
                    ParseUnit();
                    break;
                default:
                    Unexpected(lexer.Peek());
                    break;

                }
            }
        }

        private void ParseUnit()
        {
            var name = Expect<string>(TokenType.Id);
            Debug.Print($"UNIT {name}");
            Expect(TokenType.Semi);
            Expect(TokenType.Interface);
            if (PeekAndDiscard(TokenType.Const))
            {
                ParseConstSection();
            }
            if (PeekAndDiscard(TokenType.Type))
            {
                ParseTypeDefinitions();
            }
            Expect(TokenType.End);
            Expect(TokenType.Dot);
        }
 

        private void ParseConstSection()
        {
            while (lexer.Peek().Type == TokenType.Id)
            {
                var id = Expect<string>(TokenType.Id);
                Expect(TokenType.Eq);
                var exp = ParseExp();
                Expect(TokenType.Semi);
                Debug.Print("const {0} = {1};", id, exp);
            }
        }

        private void ParseTypeDefinitions()
        {
            while (lexer.Peek().Type == TokenType.Id)
            {
                var id = Expect<string>(TokenType.Id);
                Expect(TokenType.Eq);
                var type = ParseType();
                Expect(TokenType.Semi);
                Debug.Print("{0} = {1}", id, type);
            }
        }

        private PascalType ParseType()
        {
            switch (lexer.Peek().Type)
            {
            case TokenType.Byte:
                lexer.Read();
                return new Primitive { type = Serialization.PrimitiveType_v1.Char8() };
            case TokenType.Integer:
                lexer.Read();
                return new Primitive { type = Serialization.PrimitiveType_v1.Int16() };
            case TokenType.Longint:
                lexer.Read();
                return new Primitive { type = Serialization.PrimitiveType_v1.Int32() };
            case TokenType.Id:
                var id = Expect<string>(TokenType.Id);
                return new TypeReference(id);
            case TokenType.Ptr:
                lexer.Read();
                var type = ParseType();
                return new Pointer(type);
            case TokenType.Packed:
                lexer.Read();
                if (PeekAndDiscard(TokenType.Record))
                {
                    var prec = ParseRecord();
                    prec.Packed = true;
                    return prec;
                }
                else if (PeekAndDiscard(TokenType.Array))
                {
                    var parr = ParseArray();
                    parr.Packed = true;
                    return parr;
                }
                else
                    goto default;
            case TokenType.Array:
                lexer.Read();
                var arr = ParseArray();
                arr.Packed = true;
                return arr;
            case TokenType.Record:
                lexer.Read();
                var rec = ParseRecord();
                return rec;
            default:
                Unexpected(lexer.Peek());
                return null;
            }
        }

        public Record ParseRecord()
        {
            var fields = new List<Field>();
            while (!PeekAndDiscard(TokenType.End))
            {
                if (PeekAndDiscard(TokenType.Case))
                {
                    // Always last.
                    ParseVariantRecord();
                }
                else
                {
                    var fname = Expect<string>(TokenType.Id);
                    Expect(TokenType.Colon);
                    var type = ParseType();
                    Expect(TokenType.Semi);
                    fields.Add(new Field(fname, type));
                    Debug.Print("   {0}:{1}", fname, type);
                }
            }
            return new Record
            {
                Fields = fields,
            };
        }

        private void ParseVariantRecord()
        {
            //$TODO: too lazy to do this now, just eat all tokens until the end.
            //$BUG Won't work if nested structures occur....
            while (lexer.Peek().Type != TokenType.End)
            {
                lexer.Read();
            }
        }

        private Pascal.Array ParseArray()
        {
            Expect(TokenType.LBracket);
            var lo = ParseExp();
            Expect(TokenType.DotDot);
            var hi = ParseExp();
            Expect(TokenType.RBracket);
            Expect(TokenType.Of);
            var type = ParseType();
            Debug.Print("{0}[{1}..{2}]", type, lo, hi);
            return new Array { ElementType = type, Low = lo, High = hi };
        }

        private Exp ParseExp()
        {
            switch (lexer.Peek().Type)
            {
            case TokenType.Number:
                return new Number(Expect<long>(TokenType.Number));
            case TokenType.String:
                return new StringLiteral(Expect<string>(TokenType.String));
            default:
                Unexpected(lexer.Peek());
                return null;
            }
        }

        private bool PeekAndDiscard(TokenType type)
        {
            if (lexer.Peek().Type == type)
            {
                lexer.Read();
                return true;
            }
            return false;
        }

        private T Expect<T>(TokenType expectedType)
        {
            var tok = lexer.Read();
            if (tok.Type != expectedType)
                throw new ApplicationException(string.Format("Expected token {0} but read {1}.", expectedType, tok));
            return (T)tok.Value;
        }

        private void Expect(TokenType expectedType)
        {
            var tok = lexer.Read();
            if (tok.Type != expectedType)
                throw new ApplicationException(string.Format("Expected token {0} but read {1}.", expectedType, tok));
        }

        private void Unexpected(Token token)
        {
            throw new ApplicationException(string.Format("Token {0} was not expected.", token));
        }
    }
}

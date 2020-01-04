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

        public List<Declaration> Parse()
        {
            var decls = new List<Declaration>();
            while (lexer.Peek().Type != TokenType.EOF)
            {
                var tok = lexer.Read();
                switch (tok.Type)
                {
                case TokenType.Unit:
                    var d = ParseUnit();
                    decls.AddRange(d);
                    break;
                default:
                    Unexpected(lexer.Peek());
                    return null;
                }
            }
            return decls;
        }

        public List<Declaration> ParseUnit()
        {
            var decls = new List<Declaration>();
            var name = Expect<string>(TokenType.Id);
            Expect(TokenType.Semi);
            Expect(TokenType.Interface);
            for (;;)
            {
                switch (lexer.Peek().Type)
                {
                case TokenType.Const:
                    lexer.Read();
                    var consts = ParseConstSection();
                    decls.AddRange(consts);
                    break;
                case TokenType.Type:
                    lexer.Read();
                    var typedefs = ParseTypeDefinitions();
                    decls.AddRange(typedefs);
                    break;
                case TokenType.Var:
                    lexer.Read();
                    ParseVarDefinitions();
                    //$TODO: dropping these on the floor for now.
                    break;
                case TokenType.Function:
                    lexer.Read();
                    var func = ParseFunction();
                    decls.Add(func);
                    break;
                case TokenType.Procedure:
                    lexer.Read();
                    var proc = ParseProcedure();
                    decls.Add(proc);
                    break;
                default:
                    Expect(TokenType.End);
                    Expect(TokenType.Dot);
                    return decls;
                }
            }
        }

        private CallableDeclaration ParseFunction()
        {
            var name = Expect<string>(TokenType.Id);
            var pps = ParseParameters();
            Expect(TokenType.Colon);
            var type = ParseType();
            Expect(TokenType.Semi);

            var body = ParseSubroutine();
            return new CallableDeclaration
            {
                Name = name,
                Parameters = pps,
                ReturnType = type,
                Body = body
            };
        }

 
        private CallableDeclaration ParseProcedure()
        {
            var name = Expect<string>(TokenType.Id);
            var pps = ParseParameters();
            Expect(TokenType.Semi);

            var body = ParseSubroutine();
            return new CallableDeclaration
            {
                Name = name,
                Parameters = pps,
                Body = body,
            };
        }

        private List<ParameterDeclaration> ParseParameters()
        {
            var pps = new List<ParameterDeclaration>();
            if (PeekAndDiscard(TokenType.LParen))
            {
                var pp = ParseParameterDeclaration();
                pps.Add(pp);
                while (PeekAndDiscard(TokenType.Semi))
                {
                    pp = ParseParameterDeclaration();
                    pps.Add(pp);
                }
                Expect(TokenType.RParen);
            }
            return pps;
        }

        private ParameterDeclaration ParseParameterDeclaration()
        {
            bool byReference = PeekAndDiscard(TokenType.Var);
            var name = Expect<string>(TokenType.Id);
            if (string.Compare(name, "opt", true) == 0)
            {
                byReference |= PeekAndDiscard(TokenType.Var);
                name = Expect<string>(TokenType.Id);
                if (lexer.Peek().Type != TokenType.Colon)
                {
                    return new ParameterDeclaration
                    {
                        ByReference = byReference,
                        ParameterNames = new List<string> { name },
                        Type = null,
                    };
                }
            }
            var names = new List<string> { name };
            while (PeekAndDiscard(TokenType.Comma))
            {
                name = Expect<string>(TokenType.Id);
                names.Add(name);
            }
            Expect(TokenType.Colon);
            var type = ParseType();
            return new ParameterDeclaration
            {
                ByReference = byReference,
                ParameterNames = names,
                Type = type,
            };
        }

        //$REVIEW: very MPW / Mac specific.
        private Block ParseSubroutine()
        {
            if (PeekAndDiscard(TokenType.Inline))
            {
                var opcodes = new List<Exp>();
                var num = this.ParseExp();
                opcodes.Add(num);
                while (PeekAndDiscard(TokenType.Comma))
                {
                    num = ParseExp();
                    opcodes.Add(num);
                }
                Expect(TokenType.Semi);
                return new InlineMachineCode
                {
                    Opcodes = opcodes
                };
            }
            else if (lexer.Peek().Type == TokenType.Id &&
                     string.Compare((string)lexer.Peek().Value, "builtin", true) == 0)
            {
                lexer.Read();
                Expect(TokenType.Semi);
                return null;
            }
            else
            { 
                return null;
            }
        }

        private List<Declaration> ParseConstSection()
        {
            var consts = new List<Declaration>();
            while (lexer.Peek().Type == TokenType.Id)
            {
                var name = Expect<string>(TokenType.Id);
                Expect(TokenType.Eq);
                var exp = ParseExp();
                consts.Add(new ConstantDeclaration(name, exp));
                Expect(TokenType.Semi);
            }
            return consts;
        }

        private List<Declaration> ParseTypeDefinitions()
        {
            var typedefs = new List<Declaration>();
            while (lexer.Peek().Type == TokenType.Id)
            {
                var name = Expect<string>(TokenType.Id);
                Expect(TokenType.Eq);
                var type = ParseType();
                typedefs.Add(new TypeDeclaration(name, type));
                Expect(TokenType.Semi);
            }
            return typedefs;
        }

        private void ParseVarDefinitions()
        {
            while (lexer.Peek().Type == TokenType.Id)
            {
                var id = Expect<string>(TokenType.Id);
                Expect(TokenType.Colon);
                var type = ParseType();
                Expect(TokenType.Semi);
            }
        }

        public PascalType ParseType()
        {
            switch (lexer.Peek().Type)
            {
            case TokenType.Boolean:
                lexer.Read();
                return new Primitive { Type = Serialization.PrimitiveType_v1.Bool() };
            case TokenType.Char:
                lexer.Read();
                return Primitive.Char();
            case TokenType.Integer:
                lexer.Read();
                return new Primitive { Type = Serialization.PrimitiveType_v1.Int16() };
            case TokenType.Longint:
                lexer.Read();
                return new Primitive { Type = Serialization.PrimitiveType_v1.Int32() };
            case TokenType.Extended:    //$REFACTOR: Mac MPW specific.
                lexer.Read();
                return new Primitive { Type = Serialization.PrimitiveType_v1.Real80() };
            case TokenType.Id:
                var id = Expect<string>(TokenType.Id);
                return new TypeReference(id);
            case TokenType.Ptr:
                lexer.Read();
                var type = ParseType();
                return new Pointer(type);
            case TokenType.File:
                lexer.Read();
                return new File();
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
                arr.Packed = false;
                return arr;
            case TokenType.String:
                lexer.Read();
                var str = ParseString();
                return str;
            case TokenType.Set:
                lexer.Read();
                var set = ParseSet();
                return set;
            case TokenType.Record:
                lexer.Read();
                var rec = ParseRecord();
                return rec;
            case TokenType.Univ:
                lexer.Read();   //$TODO: what does 'univ' mean?
                return ParseType();
            case TokenType.LParen:
                lexer.Read();
                return ParseEnum();
            case TokenType.Minus:
            case TokenType.Number:
                return ParseRange();
            default:
                Unexpected(lexer.Peek());
                return null;
            }
        }

        public Record ParseRecord()
        {
            var (fields, variantPart) = ParseFieldList();
            Expect(TokenType.End);
            return new Record
            {
                Fields = fields,
                VariantPart = variantPart
            };
        }

        private (List<Field>, VariantPart) ParseFieldList()
        {
            var fields = new List<Field>();
            while (lexer.Peek().Type != TokenType.End)
            {
                if (PeekAndDiscard(TokenType.Case))
                {
                    // Always last.
                    var vp = ParseVariantPart();
                    return (fields, vp);
                }
                else
                {
                    var fnames = new List<string>();
                    do
                    {
                        var fname = Expect<string>(TokenType.Id);
                        fnames.Add(fname);
                    } while (PeekAndDiscard(TokenType.Comma));
                    Expect(TokenType.Colon);
                    var type = ParseType();
                    fields.Add(new Field(fnames, type));
                    if (!PeekAndDiscard(TokenType.Semi))
                        break;
                    var next = lexer.Peek().Type;
                    if (next != TokenType.Id && next != TokenType.Case)
                        break;
                }
            }
            return (fields, null);
        }

        private VariantPart ParseVariantPart()
        {
            string variantID = null;
            PascalType ordinalType;
            if (lexer.Peek().Type == TokenType.Id)
            {
                variantID = (string) lexer.Read().Value;
                if (PeekAndDiscard(TokenType.Colon))
                {
                    ordinalType = ParseType();
                }
                else
                { 
                    ordinalType = new TypeReference(variantID);
                    variantID = null;
                }
            }
            else
            {
                ordinalType = ParseType();
            }
            Expect(TokenType.Of);
            List<Variant> variants = new List<Variant>();
            do
            {
                var variant = ParseVariant();
                variants.Add(variant);
            } while (PeekAndDiscard(TokenType.Semi) &&
                lexer.Peek().Type != TokenType.End);
            return new VariantPart
            {
                VariantTag = variantID,
                TagType = ordinalType,
                Variants = variants
            };
        }

        private Variant ParseVariant()
        {
            var tagValues = new List<Pascal.Exp>();
            do
            {
                var c = ParseExp();
                tagValues.Add(c);
            } while (PeekAndDiscard(TokenType.Comma));
            Expect(TokenType.Colon);
            Expect(TokenType.LParen);
            var (fields, vp) = ParseFieldList();
            Expect(TokenType.RParen);
            return new Variant
            {
                TagValues = tagValues,
                Fields = fields,
                VariantPart = vp,
            };
        }

        private Pascal.Array ParseArray()
        {
            Expect(TokenType.LBracket);
            var dim = ParseArrayDimension();
            var dims = new List<ArrayDimension> { dim };
            while (PeekAndDiscard(TokenType.Comma))
            {
                 dim = ParseArrayDimension();
                 dims .Add( dim );
            }
            Expect(TokenType.RBracket);
            Expect(TokenType.Of);
            var type = ParseType();
            var arr = new Array { ElementType = type, Dimensions = dims };
            return arr;
        }

        private SetType ParseSet()
        {
            Expect(TokenType.Of);
            if (PeekAndDiscard(TokenType.LParen))
            {
                var name = Expect<string>(TokenType.Id);
                var names = new List<string> { name };
                while (PeekAndDiscard(TokenType.Comma))
                {
                    name = Expect<string>(TokenType.Id);
                    names.Add(name);
                }
                Expect(TokenType.RParen);
               return new SetType { Names = names };
            }
            else
            {
                var enumName = Expect<string>(TokenType.Id);
                return new Pascal.SetType { EnumName = enumName };
            }
        }

        private StringType ParseString()
        {
            Exp exp = null;
            if (PeekAndDiscard(TokenType.LBracket))
            {
                exp = ParseExp();
                Expect(TokenType.RBracket);
            }
            return new StringType { Size = exp };
        }

        private ArrayDimension ParseArrayDimension()
        {
            Exp lo = ParseExp();
            Exp hi = null;
            if (PeekAndDiscard(TokenType.DotDot))
            {
                hi = ParseExp();
            }
            return new ArrayDimension { Low = lo, High = hi };
        }

        private EnumType ParseEnum()
        {
            var name = Expect<string>(TokenType.Id);
            var names = new List<string> { name };
            while (PeekAndDiscard(TokenType.Comma))
            {
                name = Expect<string>(TokenType.Id);
                names.Add(name);
            }
            Expect(TokenType.RParen);
            return new EnumType { Names = names };
        }

        private RangeType ParseRange()
        {
            var lo = ParseExp();
            Expect(TokenType.DotDot);
            var hi = ParseExp();
            return new RangeType
            {
                Low = lo,
                High = hi,
            };
        }

        private Exp ParseExp()
        {
            var term = ParseFactor();
            while (PeekAndDiscard(TokenType.Minus))
            {
                var term2 = ParseFactor();
                term = new BinExp(TokenType.Minus, term, term2);
            }
            return term;
        }

        private Exp ParseFactor()
        {
            if (PeekAndDiscard(TokenType.Minus))
            {
                var factor = ParseFactor();
                var term = new UnaryExp(TokenType.Minus, factor);
                return term;
            }
            return ParseAtom();
        }

        private Exp ParseAtom()
        {
            switch (lexer.Peek().Type)
            {
            case TokenType.False:
                lexer.Read();
                return new BooleanLiteral(false);
            case TokenType.True:
                lexer.Read();
                return new BooleanLiteral(true);
            case TokenType.Number:
                return new NumericLiteral(Expect<long>(TokenType.Number));
            case TokenType.StringLiteral:
                return new StringLiteral(Expect<string>(TokenType.StringLiteral));
            case TokenType.RealLiteral:
                return new RealLiteral(Expect<double>(TokenType.RealLiteral));
            case TokenType.Id:
                return new Id(Expect<string>(TokenType.Id));
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

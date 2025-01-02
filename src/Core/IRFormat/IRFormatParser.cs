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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Reko.Core.IRFormat
{
    public class IRFormatParser
    {
        private readonly IRFormatLexer lex;
        private Program program;
        private Token? token;
        private IRProcedureBuilder? m;

        public IRFormatParser(TextReader rdr)
        {
            this.lex = new IRFormatLexer(rdr);
            this.program = new Program();
            this.m = null;
        }

        public Program Parse()
        {
            this.program = new Program();

            //ParseTarget();

            while (!PeekToken(IRTokenType.EOF))
            {
                ParseProgramItem();
            }
            return program;
        }

        public void ParseProgramItem()
        {
            switch (ReadToken().Type)
            {
            case IRTokenType.Define:
                ParseProcedureDefinition();
                break;
            }
        }

        public void ParseProcedureDefinition()
        {
            //var name = Expect(IRTokenType.ID).Value;
            //var m = new IRProcedureBuilder(name, program.Platform);

            //if (PeekAndDiscard(IRTokenType.Addr))
            //{
            //    lex.SkipSpaces();
            //    var sAddr = lex.GetSpaceDelimitedString();
            //    Address addr;
            //    if (!program.Architecture.TryParseAddress(sAddr, out addr))
            //        throw new FormatException("Invalid address line", lex.LineNumber);
            //    program.Procedures.Add(addr, )
            //}
        }


        public Instruction ParseStatement()
        {
            if (m is null)
                throw new InvalidOperationException();
            return ParseStatement(m);
        }

        public Instruction ParseStatement(IRProcedureBuilder m)
        {
            this.m = m;
            var token = ReadToken();
            switch (token.Type)
            {
            case IRTokenType.ID:
                var tokenNext = PeekToken();
                switch (tokenNext.Type)
                {
                case IRTokenType.LogicalNot:
                    DiscardToken();
                    Expression e = ParseExpression();
                    var id = EnsureId(token);
                    return m.Assign(id, e);
                case IRTokenType.LBRACKET:
                    DiscardToken();
                    Expression ea = ParseExpression();
                    Expect(IRTokenType.COLON);
                    DataType dt = ParseType();
                    Expect(IRTokenType.RBRACKET);
                    Expect(IRTokenType.LogicalNot);
                    e = ParseExpression();
                    return m.Store(EnsureId(token), dt, ea, e);
                case IRTokenType.COLON:
                    DiscardToken();
                    m.Label(token.ValueAs<string>()!);
                    return ParseStatement();
                }
                throw Unexpected(tokenNext);
            }
            throw new NotImplementedException($"Token: {token.Type}");
        }

        private static Identifier EnsureId(Token token)
        {
            Debug.Assert(token.Type == IRTokenType.ID);
            Debug.Assert(token.Value is not null);
            var id = Identifier.CreateTemporary((string) token.Value, new UnknownType());
            return id;
        }

        private Expression ParseExpression()
        {
            return ParseLogicalOr();
        }

        private Expression ParseLogicalOr()
        {
            Debug.Assert(m is not null);
            var e = ParseLogicalAnd();
            for (; ;)
            {
                var token = PeekToken();
                if (token.Type != IRTokenType.LogicalOr)
                    return e;
                DiscardToken();
                var e2 = ParseLogicalAnd();
                e = m.Cor(e, e2);
            }
        }

        private Expression ParseLogicalAnd()
        {
            Debug.Assert(m is not null);
            var e = ParseBitwiseOr();
            for (; ; )
            {
                var token = PeekToken();
                if (token.Type != IRTokenType.LogicalOr)
                    return e;
                DiscardToken();
                var e2 = ParseBitwiseOr();
                e = m.Cand(e, e2);
            }
        }

        private Expression ParseBitwiseOr()
        {
            Debug.Assert(m is not null);
            var e = ParseBitwiseXor();
            for (; ; )
            {
                var token = PeekToken();
                if (token.Type != IRTokenType.LogicalOr)
                    return e;
                DiscardToken();
                var e2 = ParseBitwiseXor();
                e = m.Or(e, e2);
            }
        }

        private Expression ParseBitwiseXor()
        {
            Debug.Assert(m is not null);
            var e = ParseBitwiseAnd();
            for (; ; )
            {
                var token = PeekToken();
                if (token.Type != IRTokenType.LogicalOr)
                    return e;
                DiscardToken();
                var e2 = ParseBitwiseAnd();
                e = m.Xor(e, e2);
            }
        }

        private Expression ParseBitwiseAnd()
        {
            Debug.Assert(m is not null);
            var e = ParseEquality();
            for (; ; )
            {
                var token = PeekToken();
                if (token.Type != IRTokenType.LogicalOr)
                    return e;
                DiscardToken();
                var e2 = ParseEquality();
                e = m.Or(e, e2);
            }
        }


        private Expression ParseEquality()
        {
            Debug.Assert(m is not null);
            var e = ParseInequality();
            for (; ; )
            {
                var token = PeekToken();
                var opr = token.Type switch
                {
                    IRTokenType.EQ => Operator.Eq,
                    IRTokenType.NE => Operator.Ne,
                    _ => null
                };
                if (opr is null)
                    return e;
                DiscardToken();
                var e2 = ParseInequality();
                e = m.Bin(opr, e, e2);
            }
        }

        private Expression ParseInequality()
        {
            Debug.Assert(m is not null);
            var e = ParseShift();
            for (; ; )
            {
                var token = PeekToken();
                var opr = token.Type switch
                {
                    IRTokenType.LT => Operator.Lt,
                    IRTokenType.LE => Operator.Le,
                    IRTokenType.GE => Operator.Ge,
                    IRTokenType.GT => Operator.Gt,
                    IRTokenType.ULT => Operator.Ult,
                    IRTokenType.ULE => Operator.Ule,
                    IRTokenType.UGE => Operator.Uge,
                    IRTokenType.UGT => Operator.Ugt,
                    _ => null
                };
                if (opr is null)
                    return e;
                DiscardToken();
                var e2 = ParseShift();
                e = m.Bin(opr, e, e2);
            }
        }
        private Expression ParseShift()
        {
            Debug.Assert(m is not null);
            var e = ParseSum();
            for (; ; )
            {
                var token = PeekToken();
                BinaryOperator? op = token.Type switch
                {
                    IRTokenType.SHL => Operator.Shl,
                    IRTokenType.Goto => Operator.Shr,
                    IRTokenType.SAR => Operator.Sar,
                    _ => null,
                };
                if (op is null)
                    return e;
                DiscardToken();
                var sh = ParseSum();
                e = m.Bin(op, e, sh);
            }
        }

        private Expression ParseSum()
        {
            Debug.Assert(m is not null);
            var e = ParseTerm();
            for (; ; )
            {
                if (PeekAndDiscard(IRTokenType.PLUS))
                {
                    var e2 = ParseTerm();
                    e = m.IAdd(e, e2);
                }
                else if (PeekAndDiscard(IRTokenType.MINUS))
                {
                    var e2 = ParseTerm();
                    e = m.ISub(e, e2);
                }
                else
                    break;
            }
            return e;
        }

        private Expression ParseTerm()
        {
            Debug.Assert(m is not null);
            var factor = ParsePrefixUnary();
            for (; ;)
            {
                PrimitiveType? dt;
                Expression f2;
                var token = PeekToken();
                switch (token.Type)
                {
                default: return factor;
                case IRTokenType.MUL:
                    DiscardToken();
                    dt = token.ValueAs<PrimitiveType>();
                    f2 = ParsePrefixUnary();
                    if (dt is not null)
                    {
                        factor = m.IMul(dt, factor, f2);
                    }
                    else
                    {
                        factor = m.IMul(factor, f2);
                    }
                    break;
                }
            }
        }

        private Expression ParsePrefixUnary()
        {
            Debug.Assert(m is not null);
            var token = PeekToken();
            switch (token.Type)
            {
            case IRTokenType.MINUS:
                DiscardToken();
                var e = ParseAtom();
                return m.Unary(Operator.Neg, e);
            case IRTokenType.LogicalNot:
                DiscardToken();
                e = ParseAtom();
                return m.Unary(Operator.Not, e);
            }
            return ParseAtom();
        }

        private Expression ParseAtom()
        {
            var token = ReadToken();
            switch (token.Type)
            {
            case IRTokenType.ID:
                var id = EnsureId(token);
                return id;
            case IRTokenType.CONST:
                Debug.Assert(token.Value is not null);
                var c = (Constant) token.Value;
                return c;
            }
            throw Unexpected(token);
        }

        private DataType ParseType()
        {
            var id = Expect(IRTokenType.ID).ValueAs<string>()!;
            if (id.StartsWith("word"))
                return PrimitiveType.CreateWord(NumericSuffix(id));
                    throw new NotImplementedException($"Unimplemented type {id}");
        }

        private int NumericSuffix(string s)
        {
            int n = 0;
            int scale = 1;
            for (int i = s.Length-1; i >= 0; --i, scale *= 10)
            {
                char c = s[i];
                int digit = c - '0';
                if ((uint) digit >= 10)
                    break;
                n = n + digit * scale;
            }
            return n;
        }


        // target x86-Win32

        // void foo()
        // define foo

        // Frame
        // def eax:word32 reg(eax 0)
        // id = <expr>
        // call/ret/sideffect/phi/if
        // label
        // backpatch labels.

        private Token Expect(IRTokenType type)
        {
            var token = ReadToken();
            if (token.Type != type)
            {
                throw new ParserException($"Expected {type} but found {token}.");
            }
            return token;
        }

        private Token PeekToken()
        {
            if (!this.token.HasValue)
            {
                this.token = lex.Read();
            }
            return this.token.Value;
        }

        private bool PeekToken(IRTokenType type)
        {
            if (!this.token.HasValue)
            {
                this.token = lex.Read();
            }
            return this.token.Value.Type == type;
        }

        private bool PeekAndDiscard(IRTokenType type)
        {
            if (!PeekToken(type))
            {
                return false;
            }
            DiscardToken();
            return true;
        }

        private Token ReadToken()
        {
            if (this.token.HasValue)
            {
                var t = this.token.Value;
                this.token = null;
                return t;
            }
            return lex.Read();
        }


        private void DiscardToken()
        {
            if (this.token.HasValue)
            {
                this.token = null;
            }
        }


        private Exception Unexpected(in Token token)
        {
            throw new NotImplementedException($"Token: {token.Type}");
        }
    }
}

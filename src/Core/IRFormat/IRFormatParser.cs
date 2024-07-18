#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var token = ReadToken();
            switch (token.Type)
            {
            case IRTokenType.ID:
                var tokenNext = PeekToken();
                switch (tokenNext.Type)
                {
                case IRTokenType.ASSIGN:
                    DiscardToken();
                    Expression e = ParseExpression();
                    var id = EnsureId(token);
                    return m.Assign(id, e);
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
            return ParseAtom();
        }

        private Expression ParseAtom()
        {
            var token = ReadToken();
            switch (token.Type)
            {
            case IRTokenType.CONST:
                Debug.Assert(token.Value is not null);
                var c = (Constant) token.Value;
                return c;
            }
            throw Unexpected(token);
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
            throw new NotImplementedException();
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

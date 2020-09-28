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

namespace Reko.Core.CLanguage
{
    /// <summary>
    /// Lexer that deals with #pragma, #line and other directives that can happen at any time.
    /// </summary>
    public class CDirectiveLexer
    {
        private CLexer lexer;
        private ParserState state;
        private CToken tokenPrev;

        public CDirectiveLexer(ParserState state, CLexer lexer)
        {
            this.state = state;
            this.lexer = lexer;
            this.tokenPrev = new CToken(    CTokenType.EOF);
        }

        public int LineNumber { get { return lexer.LineNumber; } }

        private enum State
        {
            Start = 0,
            Strings = 1,
        }

        public CToken Read()
        {
            if (tokenPrev.Type != CTokenType.EOF)
            {
                var t = tokenPrev;
                tokenPrev = new CToken(CTokenType.EOF);
                return t;
            }

            var token = lexer.Read();
            string lastString = null;
            var state = State.Start;
            StringBuilder sb = null;
            for (; ; )
            {
                switch (state)
                {
                case State.Start:
                    if (token.Type == CTokenType.LineDirective)
                    {
                        Expect(CTokenType.NumericLiteral);
                        Expect(CTokenType.StringLiteral);
                        token = lexer.Read();
                    }
                    else if (token.Type == CTokenType.PragmaDirective)
                    {
                        token = ReadPragma((string) lexer.Read().Value);
                    }
                    else if (token.Type == CTokenType.__Pragma)
                    {
                        Expect(CTokenType.LParen);
                        token = ReadPragma((string) lexer.Read().Value);
                        token = lexer.Read();
                    }
                    else if (token.Type == CTokenType.StringLiteral)
                    {
                        state = State.Strings;
                        lastString = (string) token.Value;
                    }
                    else
                    {
                        return token;
                    }
                    break;
                case State.Strings:
                    if (token.Type == CTokenType.StringLiteral)
                    {
                        if (lastString != null)
                        {
                            sb = new StringBuilder(lastString);
                            lastString = null;
                        }
                        else
                        {
                            sb.Append(token.Value);
                        }
                        token = lexer.Read();
                    }
                    else
                    {
                        tokenPrev = token;
                        if (lastString != null)
                        {
                            var tok = new CToken(CTokenType.StringLiteral, lastString);
                            lastString = null;
                            return tok;
                        }
                        else
                        {
                            return new CToken(CTokenType.StringLiteral, sb.ToString());
                        }
                    }
                    break;
                }
            }
        }

        public virtual CToken ReadPragma(string pragma)
        {
            if (pragma == "once")
            {
                return lexer.Read();
            }
            else if (pragma == "warning")
            {
                Expect(CTokenType.LParen);
                var value = (string) Expect(CTokenType.Id);
                if (value == "disable")
                {
                    Expect(CTokenType.Colon);
                    Expect(CTokenType.NumericLiteral);
                    var token = lexer.Read();
                    while (token.Type == CTokenType.NumericLiteral)
                    {
                        token = lexer.Read();
                    }
                    Expect(CTokenType.RParen, token);
                    return lexer.Read();
                }
                else if (value == "push" || value == "pop")
                {
                }
                Expect(CTokenType.RParen);
                return lexer.Read();
            }
            else if (pragma == "prefast")
            {
                Expect(CTokenType.LParen);
                for (;;)
                {
                    var token = lexer.Read();
                    if (token.Type == CTokenType.RParen)
                        break;
                }
                return lexer.Read();
            }
            else if (pragma == "intrinsic" || pragma == "function")
            {
                Expect(CTokenType.LParen);
                Expect(CTokenType.Id);
                Expect(CTokenType.RParen);
                return lexer.Read();
            }
            else if (pragma == "pack")
            {
                Expect(CTokenType.LParen);
                var token = lexer.Read();
                switch (token.Type)
                {
                case CTokenType.NumericLiteral:
                    this.state.PushAlignment((int) token.Value);
                    Expect(CTokenType.RParen);
                    break;
                case CTokenType.RParen:
                    this.state.PopAlignment();
                    break;
                case CTokenType.Id:
                    var verb = (string) token.Value;
                    if (verb == "push")
                    {
                        Expect(CTokenType.Comma);
                        this.state.PushAlignment((int) Expect(CTokenType.NumericLiteral));
                        Expect(CTokenType.RParen);
                    }
                    else if (verb == "pop")
                    {
                        this.state.PopAlignment();
                        Expect(CTokenType.RParen);
                    }
                    else
                        throw new FormatException(string.Format("Unknown verb {0}.", verb));
                    break;
                default:
                    throw new FormatException(string.Format("Unexpected token {0}.", token.Type));
                }
                return lexer.Read();
            }
            else if (pragma == "deprecated")
            {
                Expect(CTokenType.LParen);
                Expect(CTokenType.Id);
                Expect(CTokenType.RParen);
                return lexer.Read();
            }
            else if (pragma == "comment")
            {
                Expect(CTokenType.LParen);
                Expect(CTokenType.Id);
                Expect(CTokenType.Comma);
                Expect(CTokenType.StringLiteral);
                Expect(CTokenType.RParen);
                return lexer.Read();
            }
            else if (pragma == "region" || pragma == "endregion")
            {
                lexer.SkipToNextLine();
                return lexer.Read();
            }
            else 
            {
                throw new FormatException(string.Format("Unknown #pragma {0}.", pragma));
            }
        }


        private object Expect(CTokenType expected)
        {
            var token = lexer.Read();
            return Expect(expected, token);
        }

        private object Expect(CTokenType expected, CToken actualToken)
        {
            if (actualToken.Type != expected)
                throw new FormatException(string.Format("Expected '{0}' but got '{1}'.", expected, actualToken.Type));
            return actualToken.Value;
        }

        private void Expect(CTokenType expected, object expectedValue)
        {
            var token = lexer.Read();
            if (token.Type != expected)
                throw new FormatException(string.Format("Expected '{0}' but got '{1}'.", expected, token.Type));
            if (!expectedValue.Equals(token.Value))
                throw new FormatException(string.Format("Expected '{0}' but got '{1}.", expectedValue, token.Value));
        }
    }
}

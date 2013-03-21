#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

namespace Decompiler.Tools.C2Xml
{
    /// <summary>
    /// Lexer that deals with #pragma, #line and other directives that can happen at any time.
    /// </summary>
    public class CDirectiveLexer
    {
        private CLexer lexer;
        private ParserState state;

        public CDirectiveLexer(ParserState state, CLexer lexer)
        {
            this.state = state;
            this.lexer = lexer;
        }

        public int LineNumber { get { return lexer.LineNumber; } }

        public CToken Read()
        {
            CToken token = lexer.Read();
            for (; ; )
            {
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
                else
                {
                    return token;
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
                var verb = (string) Expect(CTokenType.Id);
                if (verb == "push")
                {
                    Expect(CTokenType.Comma);
                    int align = (int) Expect(CTokenType.NumericLiteral);
                    this.state.PushAlignment(align);
                }
                else if (verb == "pop")
                {
                    this.state.PopAlignment();
                }
                else
                    throw new FormatException();
                Expect(CTokenType.RParen);
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

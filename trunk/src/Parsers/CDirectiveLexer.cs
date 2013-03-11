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
using System.Linq;
using System.Text;

namespace Decompiler.Parsers
{
    /// <summary>
    /// Lexer that deals with #pragma, #line and other directives that can happen at any time.
    /// </summary>
    public class CDirectiveLexer
    {
        private CLexer lexer;

        public CDirectiveLexer(CLexer lexer)
        {
            this.lexer = lexer;
        }

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
                    ReadPragma((string) lexer.Read().Value);
                    token = lexer.Read();
                }
                else 
                {
                    return token;
                }
            }
        }

        public virtual void ReadPragma(string pragma)
        {
            if (pragma == "once")
            {
                return;
            }
            else if (pragma == "warning")
            {
                Expect(CTokenType.LParen);
                var value = (string) Expect(CTokenType.Id);
                if (value == "disable")
                {
                    Expect(CTokenType.Colon);
                    Expect(CTokenType.NumericLiteral);
                }
                else if (value == "push" || value == "pop")
                {
                }
                Expect(CTokenType.RParen);
            }
            else
            {
                throw new FormatException(string.Format("Unknown #pragma {0}.", pragma));
            }
        }


        private object Expect(CTokenType expected)
        {
            var token = lexer.Read();
            if (token.Type != expected)
                throw new FormatException(string.Format("Expected '{0}' but got '{1}'.", expected, token.Type));
            return token.Value;
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

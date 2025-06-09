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

using NUnit.Framework;
using Reko.Core.Hll.Pascal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Hll.Pascal
{
    [TestFixture]
    public class PascalLexerTests
    {
        private Token T(TokenType tt)
        {
            return new Token(tt);
        }

        private Token T(TokenType tt, object value)
        {
            return new Token(tt, value);
        }

        private void RunTest(string src, params Token[] expectedTokens)
        {
            var lex = new PascalLexer(new StringReader(src), true);
            foreach (var exp in expectedTokens)
            {
                var token = lex.Read();
                Assert.AreEqual(exp.Type, token.Type);
                if (exp.Value is null)
                {
                    Assert.IsNull(token.Value);
                }
                else
                {
                    Assert.AreEqual(exp.Value, token.Value);
                }
            }
            Assert.AreEqual(TokenType.EOF, lex.Read().Type);
        }

        [Test]
        public void PLex_NestedComments()
        {
            var src = " { this { is a { nested } } comment } CoNsT";
            RunTest(src, T(TokenType.Const));
        }


        [Test]
        public void PLex_Number()
        {
            var src = "42";
            RunTest(src, T(TokenType.Number, 42));
        }
    
        [Test]
        public void PLex_NegativeNumber()
        {
            var src = "-13";
            RunTest(src, T(TokenType.Number, -13));
        }


        [Test]
        public void PLex_AlternateComment()
        {
            var src = "(* -- alternative *)\r\nProcEdUrE";
            RunTest(src, T(TokenType.Procedure));
        }

        [Test]
        public void PLex_Boolean()
        {
            var src = "BooleaN";
            RunTest(src, T(TokenType.Boolean));
        }

        [Test]
        public void PLex_DoubleQuoteString()
        {
            var src = "\"string\"";
            RunTest(src, T(TokenType.StringLiteral, "string"));
        }

        [Test]
        public void PLex_FloatWithExponent()
        {
            var culture = CultureInfo.CurrentCulture;
            try
            {
                // Github #1050: tokenizer is sensitive to locale.
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("sv");
                RunTest("2e10", T(TokenType.RealLiteral, 2e10));
                RunTest("2.5e10", T(TokenType.RealLiteral, 2.5e10));
                RunTest("2e+10", T(TokenType.RealLiteral, 2e10));
                RunTest("2.5e+10", T(TokenType.RealLiteral, 2.5e10));
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
            }
        }
    }
}

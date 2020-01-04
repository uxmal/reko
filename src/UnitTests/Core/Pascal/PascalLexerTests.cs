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

using NUnit.Framework;
using Reko.Core.Pascal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Pascal
{
    [TestFixture]
    public class PascalLexerTests
    {
        private Token T(TokenType tt)
        {
            return new Token { Type = tt };
        }

        private Token T(TokenType tt, object value)
        {
            return new Token { Type = tt, Value = value };
        }

        private void RunTest(string src, params Token[] expectedTokens)
        {
            var lex = new PascalLexer(new StringReader(src));
            foreach (var exp in expectedTokens)
            {
                var token = lex.Read();
                Assert.AreEqual(exp.Type, token.Type);
                if (exp.Value == null)
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
    }
}

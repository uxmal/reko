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
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Reko.UnitTests.Core.CLanguage
{
    [TestFixture]
    public class LookAheadLexerTests
    {
        private LookAheadLexer lexer;
        private ParserState parserState;

        [SetUp]
        public void Setup()
        {
            parserState = new ParserState();
        }
        private void Lexer(string input)
        {
            var dlexer = new CDirectiveLexer(
                parserState, 
                new CLexer(new StringReader(input)));
            lexer = new LookAheadLexer(dlexer);
        }

        private void Expect(CTokenType expectedType, string expectedValue)
        {
            var token = lexer.Read();
            Assert.AreEqual(expectedType, token.Type);
            Assert.AreEqual(expectedValue, token.Value);
        }

        private void Peek(int n, CTokenType expectedType, string expectedValue)
        {
            var token = lexer.Peek(n);
            Assert.AreEqual(expectedType, token.Type);
            Assert.AreEqual(expectedValue, token.Value);
        }

        [Test]
        public void LookAheadLexer_Create()
        {
            Lexer("a ");
            Expect(CTokenType.Id, "a");
        }

        [Test]
        public void LookAheadLexer_Peek()
        {
            Lexer("a ");
            Peek(0, CTokenType.Id, "a");
            Expect(CTokenType.Id, "a");
        }

        [Test]
        public void LookAheadLexer_Peek2()
        {
            Lexer("a b");
            Peek(1, CTokenType.Id, "b");
            Expect(CTokenType.Id, "a");
            Expect(CTokenType.Id, "b");
        }

        [Test]
        public void LookAheadLexer_Interleaved()
        {
            Lexer(" a b  c d");
            Peek(2, CTokenType.Id, "c");
            Expect(CTokenType.Id, "a");
            Peek(2, CTokenType.Id, "d");
            Expect(CTokenType.Id, "b");
        }

        [Test]
        public void LookAheadLexer_PeenThenRead()
        { 
            Lexer(" a b  c d");
            Peek(1, CTokenType.Id, "b");
            Expect(CTokenType.Id, "a");
            Expect(CTokenType.Id, "b");
            Expect(CTokenType.Id, "c");
        }

    }
}

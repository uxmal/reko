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
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.CLanguage
{
    [TestFixture]
    public class CDirectiveLexerTests
    {
        private CDirectiveLexer lexer;
        private ParserState state;

        [SetUp]
        public void Setup()
        {
            this.state = new ParserState();
        }

        private void Lex(string text)
        {
            lexer = new CDirectiveLexer(state, new CLexer(new StringReader(text)));
        }

        private void Expect(CTokenType expectedType)
        {
            var token = lexer.Read();
            Assert.AreEqual(expectedType, token.Type);
        }
        private void Expect(CTokenType expectedType, object expectedValue)
        {
            var token = lexer.Read();
            Assert.AreEqual(expectedType, token.Type);
            Assert.AreEqual(expectedValue, token.Value);
        }

        [Test]
        public void CDirectiveLexer_NotTerminated_ReturnEof()
        {
            Lex("#line 1\n \"foo.h\"");
            Expect(CTokenType.EOF);
        }

        [Test]
        public void CDirectiveLexer_LineDirective_ReturnFollowingToken()
        {
            Lex("#line 1\n \"foo.h\"\na");
            Expect(CTokenType.Id, "a");
        }

        [Test]
        public void CDirectiveLexer_Pack_Push()
        {
            Assert.AreEqual(8, state.Alignment);
            Lex("#pragma pack(push,4)");
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
            Assert.AreEqual(4, state.Alignment);
        }

        [Test]
        public void CDirectiveLexer_Pack_Push_Pop()
        {
            Assert.AreEqual(8, state.Alignment);
            Lex("#pragma pack(push,4)\nx\n#pragma pack(pop)");
            Assert.AreEqual(CTokenType.Id, lexer.Read().Type);
            Assert.AreEqual(4, state.Alignment);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
            Assert.AreEqual(8, state.Alignment);
        }

        [Test]
        public void CDirectiveLexer_Pragma_Keywords()
        {
            Lex("__pragma(warning(push)) __pragma(warning(disable : 4548)) x");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_Pragma_deprecated()
        {
            Lex("#pragma deprecated (Foo) x");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_Pragma_comment_lib()
        {
            Lex("#pragma comment(lib, \"foo.lib\")\nx");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_Pragma_region()
        {
            Lex("#pragma region stuf stuff stuff\r\nx");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_Concatenate_string_literals()
        {
            Lex("\"foo\"    \"bar\"");
            Assert.AreEqual("foobar", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_NewPackSyntax()
        {
            Lex(
                "#pragma pack(4)\r\n" +
                "#pragma pack()\r\nx");
            Assert.AreEqual("x", lexer.Read().Value);
        }
    }
}

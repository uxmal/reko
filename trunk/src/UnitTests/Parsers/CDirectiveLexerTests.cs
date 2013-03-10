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

using Decompiler.Parsers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Parsers
{
    [TestFixture]
    public class CDirectiveLexerTests
    {
        private CDirectiveLexer lexer;

        private void Lex(string text)
        {
            lexer = new CDirectiveLexer(new CLexer(new StringReader(text)));
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
    }
}

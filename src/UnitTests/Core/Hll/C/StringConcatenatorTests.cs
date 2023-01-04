#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Hll.C;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Hll.C
{
    [TestFixture]
    public class StringConcatenatorTests
    {
        private StringConcatenator sc;

        [SetUp]
        public void Setup()
        {
            this.sc = null!;
        }

        private void Lex(string source)
        {
            this.sc = new StringConcatenator(
                new CDirectiveLexer(
                    new ParserState(),
                    new CLexer(
                        new StringReader(source),
                        CLexer.StdKeywords)));
        }

        [Test]
        public void SC_interleaved_define()
        {
            Lex(
@" ""first+""
" + @"#define SECOND ""second""
 SECOND");
            Assert.AreEqual("first+second", sc.Read().Value!);
        }

        [Test]
        public void SC_concat_2()
        {
            Lex("\"foo\"    \"bar\"");
            Assert.AreEqual("foobar", sc.Read().Value);
        }

        [Test]
        public void SC_concat_3()
        {
            Lex(@" ""a"" ""b"" ""c""");
            Assert.AreEqual("abc", sc.Read().Value!);
        }

        [Test]
        public void SC_string_literals_separated_by_newline()
        {
            Lex("\"foo\" \r   \"bar\"");
            Assert.AreEqual("foobar", sc.Read().Value);
        }
    }
}

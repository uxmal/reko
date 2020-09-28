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
using Reko.ImageLoaders.LLVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.Llvm
{
    [TestFixture]
    public class LlvmLexerTests
    {
        private LLVMLexer lex;

        private void CreateLexer(string str)
        {
            this.lex = new LLVMLexer(new StringReader(str));
        }

        [Test]
        public void LLLex_EOF()
        {
            CreateLexer("");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.EOF, tok.Type);
        }

        [Test]
        public void LLLex_Comment()
        {
            CreateLexer("; hello\r\n");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.Comment, tok.Type);
            Assert.AreEqual(" hello", tok.Value);
        }

        [Test]
        public void LLLex_reserved()
        {
            CreateLexer("  source_filename");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.source_filename, tok.Type);
        }

        [Test]
        public void LLLex_string()
        {
            CreateLexer("\"foo.cpp\"");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.String, tok.Type);
            Assert.AreEqual("foo.cpp", tok.Value);
        }

        [Test]
        public void LLLex_local_id()
        {
            CreateLexer("%0");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.LocalId, tok.Type);
            Assert.AreEqual("0", tok.Value);
        }

        [Test]
        public void LLLex_global_id()
        {
            CreateLexer("@foo");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.GlobalId, tok.Type);
            Assert.AreEqual("foo", tok.Value);
        }

        [Test]
        public void LLLex_equal_sign()
        {
            CreateLexer("   = ");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.EQ, tok.Type);
        }

        [Test]
        public void LLLex_char_array()
        {
            CreateLexer("c\"zero\\00\"");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.CharArray, tok.Type);
            Assert.AreEqual("zero\\00", tok.Value);
        }

        [Test]
        public void LLLex_comma()
        {
            CreateLexer(",");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.COMMA, tok.Type);
        }

        [Test]
        public void LLLex_hash()
        {
            CreateLexer("#");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.HASH, tok.Type);
        }

        [Test]
        public void LLLex_number_0()
        {
            CreateLexer("0");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.Integer, tok.Type);
            Assert.AreEqual("0", tok.Value);
        }

        [Test]
        public void LLLex_number_0xAA()
        {
            CreateLexer("0xAA");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.HexInteger, tok.Type);
            Assert.AreEqual("AA", tok.Value);
        }

        [Test]
        public void LLLex_number_ellipsis()
        {
            CreateLexer("...");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.ELLIPSIS, tok.Type);
        }

        [Test]
        public void LLLex_negative_integer()
        {
            CreateLexer("-108");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.Integer, tok.Type);
            Assert.AreEqual("-108", tok.Value);
        }

        [Test]
        public void LLLex_x86_80_literal()
        {
            CreateLexer("0xK3fff8000000000000001");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.X86_fp80_Literal, tok.Type);
            Assert.AreEqual("3fff8000000000000001", tok.Value);
        }

        [Test]
        public void LLLex_double_literal()
        {
            CreateLexer("5.000000e-01");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.DoubleLiteral, tok.Type);
            Assert.AreEqual("5.000000e-01", tok.Value);
        }

        [Test]
        public void LLLex_double_literal2()
        {
            CreateLexer("5.0e+01");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.DoubleLiteral, tok.Type);
            Assert.AreEqual("5.0e+01", tok.Value);
        }
    }
}

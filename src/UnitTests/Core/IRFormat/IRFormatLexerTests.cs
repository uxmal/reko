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
using Reko.Core.Expressions;
using Reko.Core.IRFormat;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.IRFormat
{
    [TestFixture]
    public class IRFormatLexerTests
    {
        [Test]
        public void Irfl_Constant_word()
        {
            var lexer = new IRFormatLexer(new StringReader("3<32>"));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.CONST, token.Type);
            var c = (Constant) token.Value;
            Assert.IsTrue(c.DataType.IsWord);
            Assert.AreEqual(PrimitiveType.Word32, c.DataType);
            Assert.AreEqual(3, c.ToInt32());
        }

        [Test]
        public void Irfl_Constant_int16()
        {
            var lexer = new IRFormatLexer(new StringReader("4<i16>"));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.CONST, token.Type);
            var c = (Constant) token.Value;
            Assert.IsFalse(c.DataType.IsWord);
            Assert.AreEqual(PrimitiveType.Int16, c.DataType);
            Assert.AreEqual(4, c.ToInt16());
        }

        [Test]
        public void Irfl_Identifier()
        {
            var lexer = new IRFormatLexer(new StringReader("eax "));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.ID, token.Type);
            Assert.AreEqual("eax", token.Value.ToString());
        }

        [Test]
        public void Irfl_Identifier_numeric_suffix()
        {
            var lexer = new IRFormatLexer(new StringReader("r2 "));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.ID, token.Type);
            Assert.AreEqual("r2", token.Value.ToString());
        }

        [Test]
        public void Irfl_smul()
        {
            var lexer = new IRFormatLexer(new StringReader(" *s "));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.SMUL, token.Type);
            Assert.IsNull(token.Value);
        }

        [Test]
        public void Irfl_smul_extending()
        {
            var lexer = new IRFormatLexer(new StringReader(" *s32 "));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.SMUL, token.Type);
            Assert.AreEqual(PrimitiveType.Int32, token.Value);
        }

        [Test]
        public void Irfl_plus()
        {
            var lexer = new IRFormatLexer(new StringReader(" + "));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.PLUS, token.Type);
        }

        [Test]
        public void Irfl_assign()
        {
            var lexer = new IRFormatLexer(new StringReader("="));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.LogicalNot, token.Type);
        }

        [Test]
        public void Irfl_lt()
        {
            var lexer = new IRFormatLexer(new StringReader("<"));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.LT, token.Type);
        }

        [Test]
        public void Irfl_le()
        {
            var lexer = new IRFormatLexer(new StringReader("<="));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.LE, token.Type);
        }

        [Test]
        public void Irfl_shl()
        {
            var lexer = new IRFormatLexer(new StringReader("<<"));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.SHL, token.Type);
        }

        [Test]
        public void Irfl_gt()
        {
            var lexer = new IRFormatLexer(new StringReader(">"));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.GT, token.Type);
        }

        [Test]
        public void Irfl_ge()
        {
            var lexer = new IRFormatLexer(new StringReader(">="));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.GE, token.Type);
        }

        [Test]
        public void Irfl_sar()
        {
            var lexer = new IRFormatLexer(new StringReader(">>"));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.SAR, token.Type);
        }

        [Test]
        public void Irfl_shr()
        {
            var lexer = new IRFormatLexer(new StringReader(">>u"));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.Goto, token.Type);
        }

        [Test]
        public void Irfl_goto()
        {
            var lexer = new IRFormatLexer(new StringReader("goto"));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.Goto, token.Type);
        }

        [Test]
        public void Irfl_equals()
        {
            var lexer = new IRFormatLexer(new StringReader("=="));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.EQ, token.Type);
        }


        [Test]
        public void Irfl_notequals()
        {
            var lexer = new IRFormatLexer(new StringReader("!="));

            var token = lexer.Read();

            Assert.AreEqual(IRTokenType.NE, token.Type);
        }
    }
}

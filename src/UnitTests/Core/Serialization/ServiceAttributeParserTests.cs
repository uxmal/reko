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
using System.Text;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ServiceAttributeParserTests
    {
        private ServiceAttributeParser sap;

        private void Given_Parser(string cText)
        {
            var state = new ParserState();
            var lexer = new CLexer(new StringReader(cText), CLexer.MsvcKeywords);
            var cparser = new CParser(state, lexer);
            var attr = cparser.Parse_AttributeSpecifier();
            this.sap = new ServiceAttributeParser(attr);
        }

        [Test]
        public void Sap_Parse_Vector()
        {
            Given_Parser("[[reko::service(vector=0x21)]]");

            var result = sap.Parse();

            Assert.AreEqual("21", result.Vector);
        }

        [Test]
        public void Sap_Parse_Registers()
        {
            Given_Parser("[[reko::service(vector=0x80, regs={al:0x42,ah:0x43})]]");

            var result = sap.Parse();

            Assert.AreEqual("80", result.Vector);
            var regs = result.RegisterValues!;
            Assert.AreEqual(2, regs.Length);
            Assert.AreEqual("al", regs[0].Register);
            Assert.AreEqual("42", regs[0].Value);
            Assert.AreEqual("ah", regs[1].Register);
            Assert.AreEqual("43", regs[1].Value);

        }


    }
}

#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.ImageLoaders.Elf.Relocators;
using System.IO;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class CharacteristicsParserTests
    {
        private CharacteristicsParser chp;

        private void Given_Parser(string cText)
        {
            var state = new ParserState();
            var lexer = new CLexer(new StringReader(cText), CLexer.MsvcKeywords);
            var cparser = new CParser(state, lexer);
            var attr = cparser.Parse_AttributeSpecifier();
            this.chp = new CharacteristicsParser(attr);
        }

        [Test]
        public void Chp_Empty()
        {
            Given_Parser("[[reko::characteristics()]]");

            var result = chp.Parse();

            Assert.IsTrue(result.IsDefaultCharactaristics);
        }

        [Test]
        public void Chp_Parse_Registers()
        {
            Given_Parser("[[reko::characteristics({terminates:true})]]");

            var result = chp.Parse();

            Assert.IsTrue(result.Terminates);
        }

        [Test]
        public void Chp_Parse_alloca()
        {
            Given_Parser("[[reko::characteristics({alloca:true})]]");

            var result = chp.Parse();

            Assert.IsTrue(result.IsAlloca);
        }

        [Test]
        public void Chp_Parse_varargs_class()
        {
            Given_Parser("[[reko::characteristics({varargs:\"My.Custom.Class\"})]]");

            var result = chp.Parse();

            Assert.AreEqual("My.Custom.Class", result.VarargsParserClass);
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Types;
using Reko.Libraries.Python;

namespace Reko.UnitTests.Core.Analysis
{
    [TestFixture]
    public class PyArgFormatParserTests
    {
        private PyArgFormatParser parser;
        private Program program;

        [SetUp]
        public void Setup()
        {
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            arch.Setup(a => a.WordWidth).Returns(PrimitiveType.Word32);
            platform.Setup(p => p.Architecture).Returns(arch.Object);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Short)).Returns(2);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Long)).Returns(4);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.LongLong)).Returns(8);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Float)).Returns(4);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Double)).Returns(8);
            platform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            this.program = new Program { Platform = platform.Object };
        }

        private void ParseChar32(string formatString)
        {
            this.parser = new PyArgFormatParser(
                program,
                Address.Ptr32(0x123400),
                formatString,
                null);
            parser.Parse();
        }

        [Test]
        public void PAFP_Empty()
        {
            ParseChar32("");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PAFP_NoFormat()
        {
            ParseChar32(":, \t");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PAFP_Integer()
        {
            ParseChar32("i:integer");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 int32)", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PAFP_UInteger()
        {
            ParseChar32("I");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 uint32)", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PAFP_Byte()
        {
            ParseChar32("b|B");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 char)", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("(ptr32 char)", parser.ArgumentTypes[1].ToString());
        }

        [Test]
        public void PAFP_Short()
        {
            ParseChar32("H|h");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 uint16)", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("(ptr32 int16)", parser.ArgumentTypes[1].ToString());
        }

        [Test]
        public void PAFP_Float()
        {
            ParseChar32("f");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 real32)", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PAFP_Double()
        {
            ParseChar32("d");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 real64)", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PAFP_Char()
        {
            ParseChar32("c;char");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 char)", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PAFP_TwoFormats()
        {
            ParseChar32("cs");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 char)", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("(ptr32 (ptr32 char))", parser.ArgumentTypes[1].ToString());

        }

        [Test]
        public void PAFP_PyObject()
        {
            ParseChar32("O");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 (ptr32 PyObject))", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PAFP_PyObjectConverter()
        {
            ParseChar32("O&");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 code)", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("(ptr32 void)", parser.ArgumentTypes[1].ToString());
        }

        [Test]
        public void PAFP_longlong()
        {
            ParseChar32("(LK)");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 int64)", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("(ptr32 uint64)", parser.ArgumentTypes[1].ToString());
        }

        [Test]
        public void PAFP_list()
        {
            ParseChar32("((d)(dd)(ddd))");
            Assert.AreEqual(6, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PAFP_StringAndLength()
        {
            ParseChar32("s#");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 (ptr32 char))", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("(ptr32 int32)", parser.ArgumentTypes[1].ToString());
        }
    }
}

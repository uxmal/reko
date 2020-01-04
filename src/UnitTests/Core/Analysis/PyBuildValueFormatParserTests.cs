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
    public class PyBuildValueFormatParserTests
    {
        private PyBuildValueFormatParser parser;
        private Program program;

        [SetUp]
        public void Setup()
        {
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            arch.Setup(a => a.WordWidth).Returns(PrimitiveType.Word32);
            platform.Setup(p => p.Architecture).Returns(arch.Object);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Long)).Returns(4);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.LongLong)).Returns(8);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Double)).Returns(8);
            platform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            this.program = new Program { Platform = platform.Object };
        }

        private void ParseChar32(string formatString)
        {
            this.parser = new PyBuildValueFormatParser(
                program,
                Address.Ptr32(0x123400),
                formatString,
                null);
            parser.Parse();
        }

        [Test]
        public void PBFP_Empty()
        {
            ParseChar32("");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PBFP_NoFormat()
        {
            ParseChar32(":, \t");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PBFP_Integer()
        {
            ParseChar32("i:");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int32", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PBFP_Float()
        {
            ParseChar32("f");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("real64", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PBFP_Double()
        {
            ParseChar32("d");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("real64", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PBFP_Char()
        {
            ParseChar32("c");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int32", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PBFP_TwoFormats()
        {
            ParseChar32("cs");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("int32", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("(ptr32 char)", parser.ArgumentTypes[1].ToString());

        }

        [Test]
        public void PBFP_PyObject()
        {
            ParseChar32("O");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 PyObject)", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PBFP_PyObjectConverter()
        {
            ParseChar32("O&");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 code)", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("(ptr32 void)", parser.ArgumentTypes[1].ToString());
        }

        [Test]
        public void PBFP_longlong()
        {
            ParseChar32("(LK)");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("int64", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("uint64", parser.ArgumentTypes[1].ToString());
        }

        [Test]
        public void PBFP_list()
        {
            ParseChar32("[(d){d:d}[d,d,d]]");
            Assert.AreEqual(6, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PBFP_StringAndLength()
        {
            ParseChar32("s#");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 char)", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("int32", parser.ArgumentTypes[1].ToString());
        }
    }
}

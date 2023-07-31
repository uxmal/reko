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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Hll.C;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Libraries.Libc;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Core.Analysis
{
    [TestFixture]
    public class PrintfFormatParserTests
    {
        private PrintfFormatParser parser;
        private Mock<IEventListener> eventListener;
        private ServiceContainer sc;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            this.eventListener = new Mock<IEventListener>();
            this.sc.AddService(typeof(IEventListener), this.eventListener.Object);
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            arch.Setup(a => a.WordWidth).Returns(PrimitiveType.Word32);
            platform.Setup(p => p.Architecture).Returns(arch.Object);
            platform.Setup(p => p.GetBitSizeFromCBasicType(CBasicType.Int)).Returns(32);
            platform.Setup(p => p.GetBitSizeFromCBasicType(CBasicType.Long)).Returns(32);
            platform.Setup(p => p.GetBitSizeFromCBasicType(CBasicType.Double)).Returns(64);
            platform.Setup(p => p.GetBitSizeFromCBasicType(CBasicType.LongDouble)).Returns(80);
            platform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            this.program = new Program { Platform = platform.Object };
        }

        private void ParseChar32(string formatString)
        {
            this.parser = new PrintfFormatParser(program, Address.Ptr32(0x123400), formatString, sc);
            parser.Parse();
        }

        [Test]
        public void PFP_Empty()
        {
            ParseChar32("");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PFP_NoFormat()
        {
            ParseChar32("Hello world");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PFP_LiteralPercent()
        {
            ParseChar32("H%%ello world");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void PFP_32_Decimal()
        {
            ParseChar32("Total: %d");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int32", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PFP_32_Char()
        {
            ParseChar32("'%c'");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("char", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PFP_32_TwoFormats()
        {
            ParseChar32("%c%x");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("char", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("uint32", parser.ArgumentTypes[1].ToString());

        }

        [Test]
        public void PFP_32_Short()
        {
            ParseChar32("%hd");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int16", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PFP_32_I64_is_unknown_Microsoft_extension()
        {
            ParseChar32("%I64x");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("<unknown>", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PFP_32_Invalid_S()
        {
            eventListener.Setup(e => e.CreateAddressNavigator(
                It.IsAny<Program>(), It.IsAny<Address>())).Verifiable();
            eventListener.Setup(e => e.Warn(
                It.IsAny<ICodeLocation>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()));
            ParseChar32("%S");
            eventListener.VerifyAll();
        }

        [Test]
        public void PFP_32_longlong()
        {
            ParseChar32("%lli");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int64", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PFP_32_Pointer()
        {
            ParseChar32("%08p");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("ptr32", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PFP_32_String()
        {
            ParseChar32("%08s");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 char)", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PFP_32_wchar_t()
        {
            ParseChar32("%lc");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("wchar_t", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void PFP_long_double()
        {
            ParseChar32("%Lg");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("real80", parser.ArgumentTypes[0].ToString());
        }
    }
}

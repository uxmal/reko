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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class MsPrintfFormatParserTests
    {
        private MsPrintfFormatParser parser;
        private ServiceContainer sc;
        private Mock<DecompilerEventListener> eventListener;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            this.eventListener = new Mock<DecompilerEventListener>();
            this.sc.AddService(typeof(DecompilerEventListener), this.eventListener.Object);
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            arch.Setup(a => a.WordWidth).Returns(PrimitiveType.Word32);
            platform.Setup(p => p.Architecture).Returns(arch.Object);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Long)).Returns(4);
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Double)).Returns(8);
            platform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            this.program = new Program { Platform = platform.Object };
        }

        private void ParseChar32(string formatString)
        {
            this.parser = new MsPrintfFormatParser(program, Address.Ptr32(0x123400), formatString, sc);
            parser.Parse();
        }

        [Test]
        public void MSPFP_32_I64()
        {
            ParseChar32("%I64x");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("uint64", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void MSPFP_S_widechar_string()
        {
            ParseChar32("%S");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("(ptr32 wchar_t)", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void MSPFP_S_precision()
        {
            ParseChar32("%.13d");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int32", parser.ArgumentTypes[0].ToString());
        }

    }
}

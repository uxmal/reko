#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Windows;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class MsPrintfFormatParserTests
    {
        private MsPrintfFormatParser parser;
        private MockRepository mr;
        private ServiceContainer sc;
        private DecompilerEventListener eventListener;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.sc = new ServiceContainer();
            this.eventListener = mr.Stub<DecompilerEventListener>();
            this.sc.AddService(typeof(DecompilerEventListener), this.eventListener);
            var arch = mr.Stub<IProcessorArchitecture>();
            var platform = mr.Stub<IPlatform>();
            arch.Stub(a => a.WordWidth).Return(PrimitiveType.Word32);
            platform.Stub(p => p.Architecture).Return(arch);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Long)).Return(4);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Double)).Return(8);
            platform.Stub(p => p.PointerType).Return(PrimitiveType.Ptr32);
            this.program = new Program { Platform = platform };
        }

        private void ParseChar32(string formatString)
        {
            mr.ReplayAll();

            this.parser = new MsPrintfFormatParser(program, Address.Ptr32(0x123400), formatString, sc);
            parser.Parse();

            mr.VerifyAll();
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

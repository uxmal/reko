#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class CHeaderLoaderTests
    {
        private byte[] Given_File(string filecontents)
        {
            return Encoding.UTF8.GetBytes(filecontents);
        }

        [Test]
        public void Chl_Address_Attribute()
        {
            var file = Given_File(
@"[[reko::address(""0123:4567"")]] int foo(char * bar);
");
            var sc = new ServiceContainer();
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            arch.Setup(a => a.MemoryGranularity).Returns(8);
            platform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            platform.Setup(p => p.GetBitSizeFromCBasicType(CBasicType.Int)).Returns(32);
            platform.Setup(p => p.Architecture).Returns(arch.Object);
            platform.Setup(p => p.CreateCParser(It.IsAny<TextReader>(), It.IsAny<ParserState>()))
                .Returns(new Func<TextReader, ParserState, CParser>((r, s) =>
                {
                    var lex = new CLexer(r, CLexer.StdKeywords);
                    return new CParser(s ?? new ParserState(), lex);
                }));
            var addr = Address.SegPtr(0123, 4567);
            platform.Setup(p => p.TryParseAddress("0123:4567", out addr)).Returns(true);

            var chl = new CHeaderLoader(sc, new RekoUri("file:foo.inc"), file);
            var typelib = chl.Load(platform.Object, new TypeLibrary());

            Assert.IsTrue(typelib.Procedures.TryGetValue(addr, out var proc));
            Assert.AreEqual(proc.Name, "foo");
            Assert.AreEqual(proc.Signature.ToString(), "(fn void ())");
        }
    }
}

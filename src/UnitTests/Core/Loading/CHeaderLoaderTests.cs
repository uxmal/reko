#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Hll.C;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Loading
{
    [TestFixture]
    public class CHeaderLoaderTests
    {
        private Mock<IPlatform> platform = default!;
        private TestCallingConvention testCc;

        [SetUp]
        public void Setup()
        {
            this.platform = default!;
            this.testCc = new TestCallingConvention();
        }

        private class TestCallingConvention : CallingConvention
        {
            public void Generate(ICallingConventionEmitter ccr, int retAddressOnStack, DataType dtRet, DataType dtThis, List<DataType> dtParams)
            {
                ccr.LowLevelDetails(4, 4);
                int i = 0;
                foreach (var dtParam in dtParams)
                {
                    var reg = new RegisterStorage($"r{i}", i, 0, PrimitiveType.CreateWord(dtParam.BitSize));
                    ccr.RegParam(reg);
                }
                if (dtRet is not null)
                {
                    ccr.RegReturn(new RegisterStorage($"r{i}", i, 0, PrimitiveType.CreateWord(dtRet.BitSize)));
                }
            }

            public bool IsArgument(Storage stg)
            {
                throw new NotImplementedException();
            }

            public bool IsOutArgument(Storage stg)
            {
                throw new NotImplementedException();
            }
        }

        private byte[] Given_File(string filecontents)
        {
            return Encoding.UTF8.GetBytes(filecontents);
        }

        private void Given_Platform()
        {
            var arch = new Mock<IProcessorArchitecture>();
            this.platform = new Mock<IPlatform>();
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
        }

        [Test]
        public void Chl_Address_Attribute()
        {
            Given_Platform();
            var file = Given_File(
@"[[reko::address(""0123:4567"")]] int foo(char * bar);
");
            var sc = new ServiceContainer();
            var addr = Address.SegPtr(0123, 4567);
            platform.Setup(p => p.TryParseAddress("0123:4567", out addr)).Returns(true);

            var chl = new CHeaderLoader(sc, ImageLocation.FromUri("file:foo.inc"), file);
            var typelib = chl.Load(platform.Object, new TypeLibrary());

            Assert.IsTrue(typelib.Procedures.TryGetValue(addr, out var proc));
            Assert.AreEqual(proc.Name, "foo");
            Assert.AreEqual(proc.Signature.ToString(), "(fn void ())");
        }

        [Test]
        public void Chl_noreturn_attribute()
        {
            Given_Platform();
            var file = Given_File(
@"[[noreturn]] void exit(int);
");
            var sc = new ServiceContainer();
            var chl = new CHeaderLoader(sc, ImageLocation.FromUri("file:foo.inc"), file);
            var typelib = chl.Load(platform.Object, new TypeLibrary());

            Assert.IsTrue(typelib.Characteristics.TryGetValue("exit", out var chr));
            Assert.IsTrue(chr.Terminates);
        }

        [Test]
        public void Chl_function_typedef()
        {
            Given_Platform();
            platform.Setup(p => p.GetCallingConvention(It.IsAny<string>())).Returns(testCc);
            var file = Given_File(@"
typedef int fn(int);
int caller(fn thing_to_call);
");
            var sc = new ServiceContainer();
            var chl = new CHeaderLoader(sc, ImageLocation.FromUri("file:foo.inc"), file);
            var typelib = chl.Load(platform.Object, new TypeLibrary());

            var caller = typelib.Signatures["caller"];
            Assert.AreEqual("(fn int32 ((ptr32 (fn int32 (int32)))))", typelib.Signatures["caller"].ToString());
        }

        [Test]
        public void Chl_reko_annotation()
        {
            Given_Platform();
            var addr = Address.Ptr32(0x023423ED);
            platform.Setup(p => p.TryParseAddress("023423ED", out addr)).Returns(true);

            var file = Given_File(@"
[[reko::annotation(""023423ED"", ""The following code formats my hard drive on Fridays"")]]
");
            var sc = new ServiceContainer();
            var chl = new CHeaderLoader(sc, ImageLocation.FromUri("file:foo.inc"), file);
            var typelib = chl.Load(platform.Object, new TypeLibrary());

            Assert.AreEqual(1, typelib.Annotations.Count);
            var a = typelib.Annotations.Values.First();
            Assert.AreEqual(0x023423ED, a.Address.ToLinear());
            Assert.AreEqual(
                "The following code formats my hard drive on Fridays", 
                a.Text);
        }
    }
}

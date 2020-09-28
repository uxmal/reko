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
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Scanning;
using System;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class GlobalDataWorkItemTests
    {
        private Mock<IScannerQueue> scanner;
        private Mock<IProcessorArchitecture> arch;
        private Mock<IPlatform> platform;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.scanner = new Mock<IScannerQueue>();
            this.arch = new Mock<IProcessorArchitecture>();
            this.platform = new Mock<IPlatform>();
            arch.Setup(a => a.Name).Returns("FakeArch");
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<MemoryArea>(),
                It.IsAny<Address>()))
                .Returns((MemoryArea i, Address a) => new LeImageReader(i, a));
            platform.Setup(p => p.Architecture).Returns(arch.Object);
            scanner.Setup(s => s.Error(
                It.IsAny<Address>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()))
                .Callback((Address a, string s, object[] args) => { Assert.Fail(string.Format("{0}: {1}", a, string.Format(s, args))); });
        }

        private void Given_Program(Address address, byte[] bytes)
        {
            var mem = new MemoryArea(address, bytes);
            var segmentMap = new SegmentMap(address, new ImageSegment(".text", mem, AccessMode.ReadExecute));
            var imageMap = segmentMap.CreateImageMap();
            this.program = new Program
            {
                Architecture = arch.Object,
                SegmentMap = segmentMap,
                ImageMap = imageMap,
                Platform = platform.Object
            };
        }


        [Test]
        public void Gdwi_GlobalData()
        {
            var bytes = new byte[] {
                0x48, 0x00, 0x21, 0x43,  0x00, 0x00, 0x00, 0x01,  0x53, 0x00, 0x21, 0x43,
                0x28, 0x00, 0x21, 0x43,  0x00, 0x00, 0x00, 0x02,  0x63, 0x00, 0x21, 0x43,
                0x38, 0x00, 0x21, 0x43,  0x00, 0x00, 0x00, 0x03,  0x73, 0x00, 0x21, 0x43,
                0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
            };
            Given_Program(Address.Ptr32(0x43210000), bytes);
            var project = new Project { Programs = { program } };

            var sSig2 = new SerializedSignature()
            {
                ReturnValue = new Argument_v1(null, new PrimitiveType_v1(Domain.Character, 1), null, false),
            };
            var ft1 = FunctionType.Func(
                new Identifier("", PrimitiveType.Int32, null));
            var ft2 = FunctionType.Func(
                new Identifier("", PrimitiveType.Char, null));
            var str = new StructureType();
            var fields = new StructureField[] {
                new StructureField(0, new Pointer(ft1, 32), "A"),
                new StructureField(4, PrimitiveType.Int32, "B"),
                new StructureField(8, new Pointer(ft2, 32), "C"),
            };
            str.Fields.AddRange(fields);
            var elementType = new TypeReference("test", str);
            var arrayType = new ArrayType(elementType, 3);

            Expect_ScannerGlobalData(0x43210048, ft1);
            Expect_ScannerGlobalData(0x43210053, ft2);
            Expect_ScannerGlobalData(0x43210028, ft1);
            Expect_ScannerGlobalData(0x43210063, ft2);
            Expect_ScannerGlobalData(0x43210038, ft1);
            Expect_ScannerGlobalData(0x43210073, ft2);

            var gdwi = new GlobalDataWorkItem(scanner.Object, program, program.ImageMap.BaseAddress, arrayType, null);
            gdwi.Process();

            scanner.Verify();
        }

        private void Expect_ScannerGlobalData(uint addrExp, DataType dtExp)
        {
            scanner.Setup(s => s.EnqueueUserGlobalData(
                Address.Ptr32(addrExp),
                dtExp,
                It.IsAny<string>()))
                .Verifiable();
        }

        [Test]
        public void Gdwi_GlobalDataRecursiveStructs()
        {
            var bytes = new byte[] {
                0x17, 0x00, 0x21, 0x43, 0x00, 0x00, 0x21, 0x43,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };
            Given_Program(Address.Ptr32(0x43210000), bytes);

            var ft = FunctionType.Func(
                new Identifier("", PrimitiveType.Real32, null),
                new Identifier[0]);
            var str = new StructureType("str", 0);
            var fields = new StructureField[] {
                new StructureField(0, new Pointer(ft,  32), "func"),
                new StructureField(4, new Pointer(str, 32), "next"),
            };
            str.Fields.AddRange(fields);
            Expect_ScannerGlobalData(0x43210017, ft);
            Expect_ScannerGlobalData(0x43210000, str);

            var gdwi = new GlobalDataWorkItem(scanner.Object, program, program.ImageMap.BaseAddress, str, null);
            gdwi.Process();

            scanner.Verify();
        }

        [Test(Description = "Scanner should be able to handle structures with padding 'holes'")]
        public void Gdwi_StructWithPadding()
        {
            var bytes = new byte[]
            {
                0x03, 0x00,             // Type field (halfword)
                0x00, 0x00,             // ...alignment padding

                0x08, 0x0, 0x21, 0x43,  // pointer to function

                0xC3,                   // function code.
            };
            Given_Program(Address.Ptr32(0x43210000), bytes);

            var ft = FunctionType.Func(
                new Identifier("", PrimitiveType.Real32, null),
                new Identifier[0]);
            var str = new StructureType();
            str.Fields.AddRange(new StructureField[]
            {
                new StructureField(0, PrimitiveType.Word16, "typeField"),
                // two-byte gap here.
                new StructureField(4, new Pointer(ft, 32), "pfn")
            });
            Expect_ScannerGlobalData(0x43210008, ft);

            var gdwi = new GlobalDataWorkItem(scanner.Object, program, program.ImageMap.BaseAddress, str, null);
            gdwi.Process();

            scanner.Verify();
        }

        [Test]
        public void Gdwi_FunctionType()
        {
            var addr = Address.Ptr32(0x12340000);
            Given_Program(addr, new byte[4]);
            var ft = FunctionType.Func(
               new Identifier("", PrimitiveType.Real32, null),
               new Identifier[0]);
            scanner.Setup(s => s.EnqueueUserProcedure(
                It.IsNotNull<IProcessorArchitecture>(),
                addr,
                It.IsNotNull<FunctionType>(),
                It.IsAny<string>()))
                .Verifiable();

            var gdwi = new GlobalDataWorkItem(scanner.Object, program, program.ImageMap.BaseAddress, ft, null);
            gdwi.Process();

            scanner.Verify();
        }
    }
}

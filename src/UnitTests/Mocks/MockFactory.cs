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

using Reko.Core;
using Reko.Arch.X86;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using Moq;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Utility class to simplify common unit test setup tasks.
    /// </summary>
    public class CommonMockFactory
    {
        private Mock<IPlatform> mockPlatform;
        private TypeLibrary platformMetadata;
        private Mock<ILoader> mockLoader;
        private ICollection<Program> programs;

        public CommonMockFactory(object ignore = null)
        {
            this.platformMetadata = new TypeLibrary();
            this.programs = new List<Program>();
        }

        /// <summary>
        /// Create a deserializer that doesn't depend on TypeLibrary.
        /// </summary>
        /// <returns></returns>
        public ISerializedTypeVisitor<DataType> CreateDeserializer(int ptrBitSize)
        {
            return new FakeTypeDeserializer(ptrBitSize);
        }

        public Mock<IPlatform> CreateMockPlatform()
        {
            if (this.mockPlatform != null)
                return this.mockPlatform;

            var mockPlatform = new Mock<IPlatform>();

            mockPlatform.Setup(p => p.Name).Returns("TestPlatform");
            mockPlatform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Char)).Returns(1);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Short)).Returns(2);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Int)).Returns(4);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Long)).Returns(4);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.LongLong)).Returns(8);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Float)).Returns(4);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Double)).Returns(8);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.LongDouble)).Returns(8);
            mockPlatform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Int64)).Returns(8);
            mockPlatform.Setup(p => p.CreateMetadata()).Returns(() => this.platformMetadata.Clone());
            var arch = new X86ArchitectureFlat32("x86-protected-32");
            mockPlatform.Setup(p => p.Architecture).Returns(arch);
            mockPlatform.Setup(p => p.DefaultCallingConvention).Returns("__cdecl");
            var ccStdcall = new X86CallingConvention(4, 4, 4, false, false);
            var ccCdecl = new X86CallingConvention(4, 4, 4, true, false);
            mockPlatform.Setup(p => p.GetCallingConvention(null)).Returns(ccCdecl);
            mockPlatform.Setup(p => p.GetCallingConvention("__stdcall")).Returns(ccStdcall);
            mockPlatform.Setup(p => p.SaveUserOptions()).Returns((Dictionary<string,object>) null);

            this.mockPlatform = mockPlatform;
            return mockPlatform;
        }

        public void Given_PlatformTypes(Dictionary<string, DataType> types)
        {
            this.platformMetadata = new TypeLibrary(
                types, new Dictionary<string, FunctionType>(), new Dictionary<string, DataType>()
            );
        }

        public ILoader CreateLoader()
        {
            if (this.mockLoader != null)
                return this.mockLoader.Object;

            this.mockLoader = new Mock<ILoader>();

            var program = CreateProgram();
            var mem = new MemoryArea(Address.Ptr32(0x10000000), new byte[1000]);
            program.SegmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment(".text", mem, AccessMode.ReadExecute));
            program.ImageMap = program.SegmentMap.CreateImageMap();
            mockLoader.Setup(
                l => l.LoadExecutable(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<Address>())
            ).Returns(program);

            mockLoader.Setup(
                l => l.LoadImageBytes(It.IsAny<string>(), It.IsAny<int>())
            ).Returns(new byte[1000]);

            return this.mockLoader.Object;
        }

        public void CreateLoadMetadataStub(
            string metafileName, 
            IPlatform platform,
            TypeLibrary loaderMetadata)
        {
            mockLoader.Setup(l => l.LoadMetadata(
                metafileName,
                platform,
                It.IsNotNull<TypeLibrary>()
            )).Returns((string f, IPlatform p, TypeLibrary tl) =>
                {
                    foreach (var module in loaderMetadata.Modules)
                        tl.Modules.Add(module);

                    foreach (var sig in loaderMetadata.Signatures)
                        tl.Signatures.Add(sig);

                    foreach (var type in loaderMetadata.Types)
                        tl.Types.Add(type);

                    return tl;
                });
        }

        public Program CreateProgram()
        {
            var platform = CreateMockPlatform();

            var program = new Program {
                Architecture = platform.Object.Architecture,
                Platform = platform.Object,
            };

            programs.Add(program);

            return program;
        }

        public void Given_UserDefinedMetafile(
            string moduleName,
            Dictionary<string, DataType> types,
            Dictionary<string, FunctionType> signatures,
            Dictionary<string, DataType> globals,
            ModuleDescriptor module)
        {
            if (types == null)
                types = new Dictionary<string, DataType>();
            if (signatures == null)
                signatures = new Dictionary<string, FunctionType>();
            if (globals == null)
                globals = new Dictionary<string, DataType>();
            var loaderMetadata = new TypeLibrary(types, signatures, globals);
            if (module != null)
                loaderMetadata.Modules.Add(moduleName, module);

            var loader = CreateLoader();

            var metafileName = moduleName + ".xml";

            CreateLoadMetadataStub(metafileName, mockPlatform.Object, loaderMetadata);
        }
    }
}

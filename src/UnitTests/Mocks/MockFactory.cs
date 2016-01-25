#region License
/* 
 * Copyright (C) 1999-2016 Pavel Tomin.
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
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using Rhino.Mocks;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Utility class to simplify common unit test setup tasks.
    /// </summary>
    public class MockFactory
    {
        private MockRepository mr;
        private IPlatform platform;
        private TypeLibrary platformMetadata;
        private ILoader loader;
        private ICollection<Program> programs;

        public MockFactory(MockRepository mr)
        {
            this.mr = mr;
            this.platformMetadata = new TypeLibrary();
            this.programs = new List<Program>();
        }

        /// <summary>
        /// Create a deserializer that doesn't depend on TypeLibrary.
        /// </summary>
        /// <returns></returns>
        public ISerializedTypeVisitor<DataType> CreateDeserializer()
        {
            var deserializer = mr.Stub<ISerializedTypeVisitor<DataType>>();
            deserializer.Stub(d => d.VisitPrimitive(null))
                .IgnoreArguments()
                .Do(new Func<PrimitiveType_v1, PrimitiveType>(
                    p => PrimitiveType.Create(p.Domain, p.ByteSize)));
            deserializer.Stub(d => d.VisitTypeReference(null))
                .IgnoreArguments()
                .Do(new Func<SerializedTypeReference, TypeReference>(
                    t => new TypeReference(t.TypeName, null)));

            return deserializer;
        }

        public IPlatform CreatePlatform()
        {
            if (platform != null)
                return platform;

            platform = mr.Stub<IPlatform>();

            platform.Stub(p => p.PointerType).Return(PrimitiveType.Pointer32);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Char)).Return(1);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Short)).Return(2);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Int)).Return(4);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Long)).Return(4);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.LongLong)).Return(8);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Float)).Return(4);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Double)).Return(8);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.LongDouble)).Return(8);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Int64)).Return(8);
            platform.Stub(p => p.CreateMetadata()).Do(new Func<TypeLibrary>(() => this.platformMetadata));
            var arch = mr.Stub<IProcessorArchitecture>();
            platform.Stub(p => p.Architecture).Return(arch);
            var ser = mr.Stub<ProcedureSerializer>(arch, null, "cdecl");
            platform.Stub(s => s.CreateProcedureSerializer(null, null)).IgnoreArguments().Return(ser);
            ser.Stub(s => s.Deserialize(
                Arg<SerializedSignature>.Is.NotNull,
                Arg<Frame>.Is.NotNull)).Return(new ProcedureSignature());
            platform.Stub(p => p.SaveUserOptions()).Return(null);

            platform.Replay();

            return platform;
        }

        public void Given_PlatformTypes(Dictionary<string, DataType> types)
        {
            this.platformMetadata = new TypeLibrary(types, null);
        }

        public ILoader CreateLoader()
        {
            if (loader != null)
                return loader;

            loader = mr.Stub<ILoader>();

            return loader;
        }

        private void CreateLoadMetadataStub(
            string metafileName, IPlatform platform, TypeLibrary loaderMetadata
        )
        {
            loader.Stub(l => l.LoadMetadata(
                Arg<string>.Is.Equal(metafileName),
                Arg<IPlatform>.Is.Equal(platform),
                Arg<TypeLibrary>.Is.NotNull
            )).Do(new Func<string, IPlatform, TypeLibrary, TypeLibrary>((f, p, tl) =>
                {
                    foreach (var module in loaderMetadata.Modules)
                        tl.Modules.Add(module);

                    foreach(var sig in loaderMetadata.Signatures)
                        tl.Signatures.Add(sig);

                    foreach (var type in loaderMetadata.Types)
                        tl.Types.Add(type);

                    return tl;
                }
            ));

            loader.Replay();
        }

        public Program CreateProgram()
        {
            var platform = CreatePlatform();

            var program = new Program {
                Architecture = platform.Architecture,
                Platform = platform,
            };

            programs.Add(program);

            return program;
        }

        public void Given_UserDefinedMetafile(
            string moduleName, Dictionary<string, DataType> types,
            Dictionary<string, ProcedureSignature> signatures,
            ModuleDescriptor module)
        {
            if (types == null)
                types = new Dictionary<string, DataType>();
            if (signatures == null)
                signatures = new Dictionary<string, ProcedureSignature>();
            var loaderMetadata = new TypeLibrary(types, signatures);
            if (module != null)
                loaderMetadata.Modules.Add(moduleName, module);

            var loader = CreateLoader();

            var metafileName = moduleName+".xml";

            CreateLoadMetadataStub(metafileName, platform, loaderMetadata);

            foreach(var program in programs)
            {
                program.LoadMetadataFile(loader, metafileName);
            }
        }
    }
}

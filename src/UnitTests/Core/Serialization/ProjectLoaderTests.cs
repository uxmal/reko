#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class ProjectLoaderTests
    {
        private MockRepository mr;
        private MockFactory mockFactory;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.mockFactory = new MockFactory(mr);
            this.sc = new ServiceContainer();
        }

        [Test(Description = "If the project file just has a single metadata file, we don't know what the platform is; so ask the user.")]
        public void Prld_LoadMetadata_NoPlatform_ShouldQuery()
        {
            var ldr = mr.Stub<ILoader>();
            var oracle = mr.StrictMock<IOracleService>();
            var arch = mr.Stub<IProcessorArchitecture>();
            var platform = mockFactory.CreatePlatform();
            var typeLib = new TypeLibrary();
            ldr.Stub(l => l.LoadMetadata(Arg<string>.Is.NotNull, Arg<IPlatform>.Is.Equal(platform), Arg<TypeLibrary>.Is.NotNull)).Return(typeLib);
            oracle.Expect(o => o.QueryPlatform(Arg<string>.Is.NotNull)).Return(platform);
            sc.AddService<IOracleService>(oracle);
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr);
            prld.LoadProject(new Project_v2
            {
                Inputs = {
                        new MetadataFile_v2 {
                            Filename = "foo",
                        }
                    }
            });
            mr.VerifyAll();
        }

        [Test]
        public void Prld_LoadMetadata_SingleBinary_ShouldNotQuery()
        {
            var ldr = mr.Stub<ILoader>();
            var oracle = mr.StrictMock<IOracleService>();
            var arch = mr.Stub<IProcessorArchitecture>();
            var platform = mockFactory.CreatePlatform();
            var typelibrary = new TypeLibrary();
            Given_Binary(ldr, platform);
            ldr.Stub(l => l.LoadMetadata(Arg<string>.Is.NotNull, Arg<IPlatform>.Is.Same(platform), Arg<TypeLibrary>.Is.NotNull)).Return(typelibrary);
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr);
            prld.LoadProject(new Project_v2
            {
                Inputs = {
                        new DecompilerInput_v2 {
                            Filename = "foo.exe",
                        },
                        new MetadataFile_v2 {
                            Filename = "foo",
                        }
                    }
            });
            mr.VerifyAll();
        }

        private void Given_Binary(ILoader ldr, IPlatform platform)
        {
            ldr.Stub(l => l.LoadImageBytes(
                Arg<string>.Is.Anything,
                Arg<int>.Is.Anything)).Return(new byte[100]);
            ldr.Stub(l => l.LoadExecutable(
                Arg<string>.Is.Anything,
                Arg<byte[]>.Is.Anything,
                Arg<Address>.Is.Anything)).Return(new Program { Platform = platform });
        }

        public class TestPlatform : DefaultPlatform
        {
            public Dictionary<string, object> Test_Options;
            public TestPlatform() : base(null, null) { }
            public override void LoadUserOptions(Dictionary<string, object> options) { Test_Options = options; }
        }

        [Test]
        public void Prld_PlatformOptions_Scalar()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v3"">
  <input>
    <user>
      <platform>
        <item key=""Name"">Bob</item>
        <item key=""Name2"">Sue</item>
      </platform>
    </user>
  </input>
</project>";
            var ldr = mr.Stub<ILoader>();
            var platform = new TestPlatform();
            Given_Binary(ldr, platform);
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr);
            prld.LoadProject("/foo/bar", new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            Assert.AreEqual(2, platform.Test_Options.Count);
            Assert.AreEqual("Bob", platform.Test_Options["Name"]);
            Assert.AreEqual("Sue", platform.Test_Options["Name2"]);
        }

        [Test]
        public void Prld_PlatformOptions_List()
        {
            var sExp =
    @"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v3"">
  <input>
    <user>
      <platform>
        <list key=""Names"">
          <item>Adam</item>
          <item>Bob</item>
          <item>Cecil</item>
        </list>
        <item key=""Name2"">Sue</item>
      </platform>
    </user>
  </input>
</project>";
            var ldr = mr.Stub<ILoader>();
            var platform = new TestPlatform();
            Given_Binary(ldr, platform);
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr);
            prld.LoadProject("/ff/b/foo.proj", new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            var list = (IList)platform.Test_Options["Names"];
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Prld_PlatformOptions_Dictionary()
        {
            var sproject =
    @"<?xml version=""1.0"" encoding=""utf-8""?>
<project xmlns=""http://schemata.jklnet.org/Reko/v3"">
  <input>
    <user>
      <platform>
        <dict key=""Names"">
          <item key=""Adam"">30</item>
          <item key=""Bob"">10</item>
          <item key=""Cecil"">120</item>
        </dict>
        <item key=""Name2"">Sue</item>
      </platform>
    </user>
  </input>
</project>";
            var ldr = mr.Stub<ILoader>();
            var platform = new TestPlatform();
            Given_Binary(ldr, platform);
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr);
            prld.LoadProject("c:\\foo\\bar.proj", new MemoryStream(Encoding.UTF8.GetBytes(sproject)));

            var list = (IDictionary)platform.Test_Options["Names"];
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Prld_MakePathsAbsolute()
        {
            var sProject = new Project_v3
            {
                Inputs =
                {
                    new DecompilerInput_v3
                    {
                        Filename = "foo.exe",
                    }
                }
            };

            var ldr = mr.Stub<ILoader>();
            ldr.Stub(l => l.LoadExecutable(null, null, null)).IgnoreArguments().Return(new Program());
            ldr.Stub(l => l.LoadImageBytes(null, 0)).IgnoreArguments().Return(new byte[1000]);
            mr.ReplayAll();

            var prld = new ProjectLoader(sc, ldr);
            var project = prld.LoadProject(@"c:/users/bob/projects/foo.project", sProject);
            Assert.AreEqual(@"c:\users\bob\projects\foo.exe", project.Programs[0].Filename);
        }

        [Test]
        public void Prld_LoadUserDefinedMetadata()
        {
            var sProject = new Project_v3
            {
                Inputs =
                {
                    new DecompilerInput_v3
                    {
                        Filename = "foo.exe",
                    },
                    new MetadataFile_v3 {
                        Filename = "meta1.xml",
                    },
                    new MetadataFile_v3 {
                        Filename = "meta2.xml",
                    },
                }
            };

            var types1 = new Dictionary<string, DataType>()
            {
                {"USRTYPE1", PrimitiveType.Word16}
            };
            var types2 = new Dictionary<string, DataType>()
            {
                {"USRTYPE2", PrimitiveType.Word32}
            };

            var ldr = mockFactory.CreateLoader();
            var platform = mockFactory.CreatePlatform();

            mockFactory.CreateLoadMetadataStub(
                "c:/meta1.xml",
                platform,
                new TypeLibrary(
                    types1, new Dictionary<string, ProcedureSignature>()
                )
            );
            mockFactory.CreateLoadMetadataStub(
                "c:/meta2.xml",
                platform,
                new TypeLibrary(
                    types2, new Dictionary<string, ProcedureSignature>()
                )
            );

            var prld = new ProjectLoader(sc, ldr);
            var project = prld.LoadProject("c:/foo.project", sProject);
            Assert.AreEqual(2, project.Programs[0].Metadata.Types.Count);
            Assert.AreEqual(
                "word16",
                project.Programs[0].Metadata.Types["USRTYPE1"].ToString()
            );
            Assert.AreEqual(
                "word32",
                project.Programs[0].Metadata.Types["USRTYPE2"].ToString()
            );
        }
    }
}

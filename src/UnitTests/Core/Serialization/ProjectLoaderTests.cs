/* 
 * Copyright (C) 1999-2008 John Källén.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
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
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.sc = new ServiceContainer();
        }

        [Test(Description = "If the project file just has a single metadata file, we don't know what the platform is; so ask the user.")]
        public void Prld_LoadMetadata_NoPlatform_ShouldQuery()
        {
            var ldr = mr.Stub<ILoader>();
            var oracle = mr.StrictMock<IOracleService>();
            var arch = mr.Stub<IProcessorArchitecture>();
            var platform = mr.Stub<Platform>(this.sc, arch);
            var typeLib = new TypeLibrary();
            ldr.Stub(l => l.LoadMetadata(Arg<string>.Is.NotNull, Arg<Platform>.Is.Equal(platform))).Return(typeLib);
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
            var platform = mr.Stub<Platform>(this.sc, arch);
            var typelibrary = new TypeLibrary();
            Given_Binary(ldr, platform);
            ldr.Stub(l => l.LoadMetadata(Arg<string>.Is.NotNull, Arg<Platform>.Is.Same(platform))).Return(typelibrary);
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

        private void Given_Binary(ILoader ldr, Platform platform)
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
            prld.LoadProject(new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

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
            prld.LoadProject(new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            var list = (IList)platform.Test_Options["Names"];
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Prld_PlatformOptions_Dictionary()
        {
            var sExp =
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
            prld.LoadProject(new MemoryStream(Encoding.UTF8.GetBytes(sExp)));

            var list = (IDictionary)platform.Test_Options["Names"];
            Assert.AreEqual(3, list.Count);
        }
    }
}

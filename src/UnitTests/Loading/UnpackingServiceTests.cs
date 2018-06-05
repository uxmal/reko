#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Configuration;
using Reko.Loading;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Reko.Core.Services;
using System.IO;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class UnpackingServiceTests
    {

        public class TestImageLoader : ImageLoader
        {
            public TestImageLoader(IServiceProvider services, string filename, byte[] bytes) : base(services, filename, bytes)
            {
            }

            public override Address PreferredBaseAddress
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public override Program Load(Address addrLoad)
            {
                throw new NotImplementedException();
            }

            public override RelocationResults Relocate(Program program, Address addrLoad)
            {
                throw new NotImplementedException();
            }
        }

        private MockRepository mr;
        private ServiceContainer sc;
        private IConfigurationService cfgSvc;
        private IFileSystemService fsSvc;
        private IDiagnosticsService diagSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            cfgSvc = mr.Stub<IConfigurationService>();
            fsSvc = mr.Stub<IFileSystemService>();
            diagSvc = mr.Stub<IDiagnosticsService>();
            sc.AddService<IFileSystemService>(fsSvc);
            sc.AddService<IDiagnosticsService>(diagSvc);
        }

        void Given_File(string name, byte[] content)
        {
            fsSvc.Stub(f => f.FileExists(name)).Return(true);
            fsSvc.Stub(f => f.CreateFileStream(name, FileMode.Open, FileAccess.Read))
                .Return(new MemoryStream(content));
        }


        void Given_NoFile(string name)
        {
            fsSvc.Stub(f => f.FileExists(name)).Return(false);
        }

        [Test]
        public void Upsvc_Find_Match()
        {
            var image = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x12, 0x34, 0x00, 0x00 };

            Given_File("foo.exe.sufa-raw.ubj", new byte[] { 0x5B, 0x24, 0x6C, 0x23, 0x69, 0x00 });
            var le = mr.Stub<LoaderConfiguration>();
            le.Label = "LoaderKey";
            le.TypeName = typeof(TestImageLoader).AssemblyQualifiedName;
            cfgSvc.Stub(c => c.GetImageLoader("LoaderKey")).Return(le);
            sc.AddService(typeof(IConfigurationService), cfgSvc);
            mr.ReplayAll();

            var upSvc = new UnpackingService(sc);
            upSvc.Signatures.Add(new ImageSignature
            {
                Name = "LoaderKey",
                EntryPointPattern = "1234",
            });
            var loader = upSvc.FindUnpackerBySignature("foo.exe", image, 4);
            Assert.IsInstanceOf<TestImageLoader>(loader);
        }

        [Test]
        public void Upsvc_Match()
        {
            var image = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x00 };
            var sig = new ImageSignature
            {
                EntryPointPattern = "1234"
            };
            var upsvc = new UnpackingService(sc);

            Assert.IsTrue(upsvc.Matches(sig, image, 4));
        }

        [Test]
        public void Upsvc_Match_Wildcard()
        {
            var image = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x00 };
            var sig = new ImageSignature
            {
                EntryPointPattern = "??34"
            };
            var upsvc = new UnpackingService(sc);

            Assert.IsTrue(upsvc.Matches(sig, image, 4));
        }

        [Test(Description = "Verifies that the suffix array for the raw image is loaded if it exists on disk.")]
        public void Upsvc_LoadSuffixArray_LoadSuffixArrayIfpresent()
        {
            Given_File("foo.exe.sufa-raw.ubj", new byte[] { 0x5B, 0x24, 0x6C, 0x23, 0x69, 0x00 });
            mr.ReplayAll();

            var upsvc = new UnpackingService(sc);
            upsvc.FindUnpackerBySignature("foo.exe", new byte[0x1000], 0x0100);
        }

        [Ignore("Disabled until new suffix array generation algorithm")]
        [Test(Description = "Verifies that the suffix array for the raw image is loaded if it exists on disk.")]
        public void Upsvc_LoadSuffixArray_CreateSuffixArrayIfpresent()
        {
            Given_NoFile("foo.exe.sufa-raw.ubj"); // , new byte[] { 0x5B, 0x24, 0x6C, 0x23, 0x69, 0x00 });
            var stm = new MemoryStream();
            fsSvc.Expect(f => f.CreateFileStream("foo.exe.sufa-raw.ubj", FileMode.Create, FileAccess.Write))
                .Return(stm);
            mr.ReplayAll();

            var upsvc = new UnpackingService(sc);
            upsvc.FindUnpackerBySignature("foo.exe", new byte[0x4], 0);
            Assert.AreEqual(new byte[] {
                0x5B, 0x24, 0x6C, 0x23, 0x69, 0x04,
                      0x00, 0x00, 0x00, 0x003,
                      0x00, 0x00, 0x00, 0x002,
                      0x00, 0x00, 0x00, 0x001,
                      0x00, 0x00, 0x00, 0x000,
                },
                stm.ToArray());
        }
    }
}

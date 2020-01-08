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
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Loading;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class UnpackingServiceTests
    {

        public class TestImageLoader : ImageLoader
        {
            public TestImageLoader(ImageLoader loader)
                : base(loader.Services, loader.Filename, loader.RawImage)
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

        private ServiceContainer sc;
        private Mock<IConfigurationService> cfgSvc;
        private Mock<IFileSystemService> fsSvc;
        private Mock<IDiagnosticsService> diagSvc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            cfgSvc = new Mock<IConfigurationService>();
            fsSvc = new Mock<IFileSystemService>();
            diagSvc = new Mock<IDiagnosticsService>();
            sc.AddService<IFileSystemService>(fsSvc.Object);
            sc.AddService<IDiagnosticsService>(diagSvc.Object);
        }

        void Given_File(string name, byte[] content)
        {
            fsSvc.Setup(f => f.FileExists(name)).Returns(true);
            fsSvc.Setup(f => f.CreateFileStream(name, FileMode.Open, FileAccess.Read))
                .Returns(new MemoryStream(content));
        }


        void Given_NoFile(string name)
        {
            fsSvc.Setup(f => f.FileExists(name)).Returns(false);
        }

        [Test]
        public void Upsvc_Find_Match()
        {
            var image = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x12, 0x34, 0x00, 0x00 };

            Given_File("foo.exe.sufa-raw.ubj", new byte[] { 0x5B, 0x24, 0x6C, 0x23, 0x69, 0x00 });
            var le = new Mock<LoaderDefinition>();
            le.SetupAllProperties();
            le.Object.Label = "LoaderKey";
            le.Object.TypeName = typeof(TestImageLoader).AssemblyQualifiedName;
            cfgSvc.Setup(c => c.GetImageLoader("LoaderKey")).Returns(le.Object);
            sc.AddService<IConfigurationService>(cfgSvc.Object);

            var upSvc = new UnpackingService(sc);
            upSvc.Signatures.Add(new ImageSignature
            {
                Name = "LoaderKey",
                EntryPointPattern = "1234",
            });
            var loader = upSvc.FindUnpackerBySignature(new NullImageLoader(sc, "foo.exe", image), 4);
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

            var upsvc = new UnpackingService(sc);
            upsvc.FindUnpackerBySignature(new NullImageLoader(sc, "foo.exe", new byte[0x1000]), 0x0100);
        }

        [Ignore("Disabled until new suffix array generation algorithm")]
        [Test(Description = "Verifies that the suffix array for the raw image is loaded if it exists on disk.")]
        public void Upsvc_LoadSuffixArray_CreateSuffixArrayIfpresent()
        {
            Given_NoFile("foo.exe.sufa-raw.ubj"); // , new byte[] { 0x5B, 0x24, 0x6C, 0x23, 0x69, 0x00 });
            var stm = new MemoryStream();
            fsSvc.Setup(f => f.CreateFileStream("foo.exe.sufa-raw.ubj", FileMode.Create, FileAccess.Write))
                .Returns(stm);

            var upsvc = new UnpackingService(sc);
            upsvc.FindUnpackerBySignature(new NullImageLoader(sc, "foo.exe", new byte[0x4]), 0);
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

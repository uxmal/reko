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
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Loading;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Decompiler.Loading
{
    [TestFixture]
	public class LoaderTests
	{
        private IServiceContainer sc;
        private FakeDecompilerEventListener eventListener;
        private Mock<IConfigurationService> cfgSvc;
        private List<SignatureFileDefinition> signatureFiles;
        private Mock<IProcessorArchitecture> x86arch;
        private Mock<IPlatform> msdosPlatform;
        private byte[] testImage;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            eventListener = new FakeDecompilerEventListener();
            cfgSvc = new Mock<IConfigurationService>();
            signatureFiles = new List<SignatureFileDefinition>();
            sc.AddService<IEventListener>(eventListener);
            sc.AddService<IDecompilerEventListener>(eventListener);
            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
            cfgSvc.Setup(d => d.GetSignatureFiles()).Returns(signatureFiles);
        }

        [Test]
        public void Ldr_Match()
        {
            Assert.IsTrue(Loader.ImageHasMagicNumber(new byte[] { 0x47, 0x11 }, "4711", 0));
        }

        [Test(Description="Unless otherwise specified, loading unknown file formats returns a Blob.")]
        public void Ldr_UnknownImageType()
        {
            cfgSvc.Setup(d => d.GetImageLoaders()).Returns(new List<LoaderDefinition>());
            cfgSvc.Setup(d => d.GetRawFile(It.IsAny<string>())).Returns((RawFileDefinition)null);
            var testImage = new byte[] { 42, 42, 42, 42, };
            var ldr = new Mock<Loader>(sc);

            Blob blob = (Blob) ldr.Object.LoadBinaryImage(ImageLocation.FromUri(""), testImage, null, null);

            Assert.IsNotNull(blob);
        }

        [Test(Description = "Use default settings when loading unknown file formats.")]
        public void Ldr_UnknownImageType_DefaultSpecified()
        {
            Given_MsDosRawFileFormat();
            cfgSvc.Setup(d => d.GetImageLoaders()).Returns(new List<LoaderDefinition>());
            x86arch.Setup(x => x.PostprocessProgram(It.IsAny<Program>()));
            var testImage = new byte[] { 42, 42, 42, 42, };
            var ldr = new Mock<Loader>(sc);

            ldr.Object.DefaultToFormat = "ms-dos-com";
            Program program = (Program) ldr.Object.LoadBinaryImage(ImageLocation.FromUri(""), testImage, null, null);

            Assert.IsNull(eventListener.LastDiagnostic);
            Assert.AreEqual("0C00:0100", program.ImageMap.BaseAddress.ToString());
            Assert.AreSame(x86arch.Object, program.Architecture);
            Assert.AreSame(msdosPlatform.Object, program.Platform);
        }

        private void Given_MsDosRawFileFormat()
        {
            this.x86arch = new Mock<IProcessorArchitecture>();
            this.x86arch.Setup(a => a.Name).Returns("x86-real-16");
            var env = new Mock<PlatformDefinition>();
            this.msdosPlatform = new Mock<IPlatform>();
            var map = new SegmentMap(Address.SegPtr(0x0C00, 0));
            var state = new FakeProcessorState(x86arch.Object);
            var rawFile = new RawFileDefinition
            {
                BaseAddress = "0C00:0100",
                Environment = "ms-dos",
                Architecture = "x86-real-16",
            };
            rawFile.EntryPoint.Address = null;
            rawFile.EntryPoint.Name = "Start_Here";
            cfgSvc.Setup(d => d.GetRawFile("ms-dos-com")).Returns(rawFile);
            cfgSvc.Setup(d => d.GetArchitecture("x86-real-16")).Returns(x86arch.Object);
            cfgSvc.Setup(d => d.GetEnvironment("ms-dos")).Returns(env.Object);
            env.Setup(e => e.Load(It.IsAny<IServiceProvider>(), It.IsAny<IProcessorArchitecture>()))
                .Returns(msdosPlatform.Object);

            var addr = Address.SegPtr(0x0C00, 0x0100);
            x86arch.Setup(a => a.TryParseAddress(
                "0C00:0100",
                out addr))
                .Returns(true);
            x86arch.Setup(a => a.CreateProcessorState()).Returns(state);
        }

        [Test]
        public void Ldr_AtOffset()
        {
            cfgSvc.Setup(d => d.GetImageLoaders()).Returns(new List<LoaderDefinition>
            {
                new LoaderDefinition {
                    Offset = 0x0002,
                    MagicNumber = "A0A0",
                    TypeName = typeof(TestImageLoader).AssemblyQualifiedName,
                }
            });
            Given_Image();

            var ldr = new Mock<Loader>(sc);
            ldr.Setup(l => l.LoadFileBytes("")).Returns(testImage);

            var imgLoader = ldr.Object.FindImageLoader<ImageLoader>(ImageLocation.FromUri(""), testImage);

            Assert.IsInstanceOf<TestImageLoader>(imgLoader);
        }

        private void Given_Image()
        {
            this.testImage = new byte[] { 0x2A, 0x2A, 0xA0, 0xA0 };
        }

        private class FakeImageLoader  : ProgramImageLoader
        {
            public FakeImageLoader(IServiceProvider services, ImageLocation imageUri, byte[]imgRaw) :
                base(services, imageUri, imgRaw)
            {

            }

            public override Address PreferredBaseAddress {get; set; }

            public override Program LoadProgram(Address addrLoad)
            {
                return new Program();
            }
        }

        private void Given_ImageLoader()
        {
            var ldrs = new List<LoaderDefinition>{
                new LoaderDefinition {
                    MagicNumber = "2A2A",
                    TypeName = typeof(FakeImageLoader).AssemblyQualifiedName,
                }
            };
            cfgSvc.Setup(c => c.GetImageLoaders()).Returns(ldrs).Verifiable();
        }

        [Test]
        public void Ldr_CreateDefaultImageLoader_GivenDefault()
        {
            Given_MsDosRawFileFormat();
            var ldr = new Mock<Loader>(sc);

            ldr.Object.DefaultToFormat = "ms-dos-com";
            var imgLoader = ldr.Object.CreateDefaultImageLoader(ImageLocation.FromUri("file:foo.com"), new byte[30]);
            var program = imgLoader.Load(null);
        }

        public class TestImageLoader : ProgramImageLoader
        {
            public TestImageLoader(IServiceProvider services, ImageLocation imageUri, byte[] imgRaw) : base(services, imageUri, imgRaw)
            {
            }

            public override Address PreferredBaseAddress
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public override Program LoadProgram(Address addrLoad)
            {
                throw new NotImplementedException();
            }
        }

        [Test(Description = "Validate that entry points are added to the Program instance when loading a 'raw' file.")]
        public void Ldr_RawFileEntryPoint()
        {
            var arch = new Mock<IProcessorArchitecture>();
            var openv = new Mock<PlatformDefinition>();
            cfgSvc.Setup(s => s.GetArchitecture("mmix", It.IsAny<Dictionary<string,object>>())).Returns(arch.Object);
            cfgSvc.Setup(s => s.GetEnvironment(It.IsAny<string>())).Returns(openv.Object);
            cfgSvc.Setup(s => s.GetImageLoader("zlorgo")).Returns(new LoaderDefinition {
                TypeName = $"{typeof(NullImageLoader).FullName},{typeof(NullImageLoader).Assembly.FullName}",
            });
            openv.Setup(o => o.Load(
                It.IsAny<IServiceProvider>(),
                It.IsAny<IProcessorArchitecture>()))
                .Returns(new DefaultPlatform(sc, arch.Object));
            arch.Setup(a => a.LoadUserOptions(It.IsAny<Dictionary<string, object>>()));
            arch.Setup(a => a.Name).Returns("mmix");
            arch.Setup(a => a.PostprocessProgram(It.IsAny<Program>()));
            var addr = Address.Ptr32(0x00123500);
            arch.Setup(a => a.TryParseAddress(
                "00123500",
                out addr))
                .Returns(true);

            var ldr = new Loader(sc);
            var program = ldr.LoadRawImage(Array.Empty<byte>(), Address.Ptr32(0x00123400), new LoadDetails
            {
                Location = ImageLocation.FromUri("file:foo.bin"),
                ArchitectureName = "mmix",
                EntryPoint = new EntryPointDefinition {  Address = "00123500" },
                LoaderName = "zlorgo",
            });

            Assert.AreEqual(1, program.EntryPoints.Count);
            Assert.AreEqual(SymbolType.Procedure, program.EntryPoints[Address.Ptr32(0x00123500)].Type);
        }

        class FakeArchiveLoader : ImageLoader
        {
            public FakeArchiveLoader(IServiceProvider services, ImageLocation uri, byte[] bytes) :
                base(services, uri, bytes)
            {
            }

            public override ILoadedImage Load(Address addrLoad)
            {
                var blob = new Blob(base.ImageLocation, new byte[234]);
                var archiveFile = new Mock<ArchivedFile>();
                archiveFile.Setup(a => a.LoadImage(
                    It.IsAny<IServiceProvider>(),
                    null))
                    .Returns(blob);
                var archive = new Mock<IArchive>();
                return archive.Object;
            }
        }

        [Test]
        public void Ldr_LoadArchivedBlob()
        {
            var ldrDef = new LoaderDefinition
            {
                Extension = ".arch",
                TypeName = typeof(FakeArchiveLoader).FullName,
            };
            cfgSvc.Setup(c => c.GetImageLoaders())
                .Returns(new List<LoaderDefinition> { ldrDef });

            var fsSvc = new Mock<IFileSystemService>();
            sc.AddService(fsSvc.Object);

            var ldr = new Loader(sc);
            var image = ldr.Load(ImageLocation.FromUri("file:archive.arch#inside/path"));

            Assert.IsNotNull(image);
            Assert.IsAssignableFrom<Blob>(image);
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Loading;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Loading
{
	[TestFixture]
	public class LoaderTests
	{
        private MockRepository mr;
        private IServiceContainer sc;
        private FakeDecompilerEventListener eventListener;
        private IConfigurationService cfgSvc;
        private List<SignatureFile> signatureFiles;
        private IProcessorArchitecture x86arch;
        private IPlatform msdosPlatform;
        private byte[] testImage;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            eventListener = new FakeDecompilerEventListener();
            cfgSvc = mr.Stub<IConfigurationService>();
            signatureFiles = new List<SignatureFile>();
            sc.AddService<DecompilerEventListener>(eventListener);
            sc.AddService<IConfigurationService>(cfgSvc);
            cfgSvc.Stub(d => d.GetSignatureFiles()).Return(signatureFiles);
        }

        [Test]
        public void Ldr_Match()
        {
            mr.ReplayAll();

            Loader ldr = new Loader(sc);
            Assert.IsTrue(ldr.ImageHasMagicNumber(new byte[] { 0x47, 0x11 }, "4711", "0"));

            mr.VerifyAll();
        }

        [Test(Description="Unless otherwise specified, fail loading unknown file formats.")]
        public void Ldr_UnknownImageType()
        {
            cfgSvc.Stub(d => d.GetImageLoaders()).Return(new List<LoaderConfiguration>());
            cfgSvc.Stub(d => d.GetRawFile(null)).IgnoreArguments().Return(null);
            var testImage = new byte[] { 42, 42, 42, 42, };
            mr.ReplayAll();
            Loader ldr = mr.PartialMock<Loader>(sc);
            ldr.Replay();

            Program prog = ldr.LoadExecutable("", testImage,  null, null);

            Assert.AreEqual("WarningDiagnostic -  - The format of the file is unknown." , eventListener.LastDiagnostic);
            Assert.AreEqual(0, prog.ImageMap.BaseAddress.Offset);
            Assert.IsNull(prog.Architecture);
            Assert.IsAssignableFrom<DefaultPlatform>(prog.Platform);
            mr.VerifyAll();
        }

        [Test(Description = "Use default settings when loading unknown file formats.")]
        public void Ldr_UnknownImageType_DefaultSpecified()
        {
            Given_MsDosRawFileFormat();
            cfgSvc.Stub(d => d.GetImageLoaders()).Return(new List<LoaderConfiguration>());
            x86arch.Stub(x => x.PostprocessProgram(null)).IgnoreArguments();
            var testImage = new byte[] { 42, 42, 42, 42, };
            mr.ReplayAll();
            Loader ldr = mr.PartialMock<Loader>(sc);
            ldr.Replay();

            ldr.DefaultToFormat = "ms-dos-com";
            Program program = ldr.LoadExecutable("", testImage, null, null);

            Assert.IsNull(eventListener.LastDiagnostic);
            Assert.AreEqual("0C00:0100", program.ImageMap.BaseAddress.ToString());
            Assert.AreSame(x86arch, program.Architecture);
            Assert.AreSame(msdosPlatform, program.Platform);
            mr.VerifyAll();
        }

        private void Given_MsDosRawFileFormat()
        {
            this.x86arch = mr.Stub<IProcessorArchitecture>();
            var env = mr.Stub<OperatingEnvironment>();
            this.msdosPlatform = mr.Stub<IPlatform>();
            var map = new SegmentMap(Address.SegPtr(0x0C00, 0));
            var state = new FakeProcessorState(x86arch);
            var rawFile = new RawFileElementImpl
            {
                BaseAddress = "0C00:0100",
                Environment = "ms-dos",
                Architecture = "x86-real-16",
            };
            rawFile.EntryPoint.Address = null;
            rawFile.EntryPoint.Name = "Start_Here";
            cfgSvc.Stub(d => d.GetRawFile("ms-dos-com")).Return(rawFile);
            cfgSvc.Stub(d => d.GetArchitecture("x86-real-16")).Return(x86arch);
            cfgSvc.Stub(d => d.GetEnvironment("ms-dos")).Return(env);
            env.Stub(e => e.Load(null, null)).IgnoreArguments().Return(msdosPlatform);
            x86arch.Stub(a => a.TryParseAddress(
                Arg<string>.Is.Equal("0C00:0100"),
                out Arg<Address>.Out(Address.SegPtr(0x0C00, 0x0100)).Dummy))
                .Return(true);
            x86arch.Stub(a => a.CreateProcessorState()).Return(state);
        }

        [Test]
        public void Ldr_AtOffset()
        {
            cfgSvc.Stub(d => d.GetImageLoaders()).Return(new List<LoaderConfiguration>
            {
                new LoaderElementImpl {
                    Offset = "0002",
                    MagicNumber = "A0A0",
                    TypeName = typeof(TestImageLoader).AssemblyQualifiedName,
                }
            });
            Given_Image();
            mr.ReplayAll();

            Loader ldr = mr.PartialMock<Loader>(sc);
            ldr.Stub(l => l.LoadImageBytes("", 0)).Return(testImage);
            mr.ReplayAll();

            var imgLoader = ldr.FindImageLoader<ImageLoader>("", testImage, () => null);

            Assert.IsInstanceOf<TestImageLoader>(imgLoader);
            mr.VerifyAll();
        }

        private void Given_Image()
        {
            this.testImage = new byte[] { 0x2A, 0x2A, 0xA0, 0xA0 };
        }

        private class FakeImageLoader  : ImageLoader
        {
            public FakeImageLoader(IServiceProvider services, string filename, byte[]imgRaw) :
                base(services, filename, imgRaw)
            {

            }

            public override Address PreferredBaseAddress {get; set; }

            public override Program Load(Address addrLoad)
            {
                return new Program();
            }

            public override RelocationResults Relocate(Program program, Address addrLoad)
            {
                return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
            }
        }

        private void Given_ImageLoader()
        {
            var ldrs = new List<LoaderConfiguration>{
                new LoaderElementImpl {
                    MagicNumber = "2A2A",
                    TypeName = typeof(FakeImageLoader).AssemblyQualifiedName,
                }
            };
            cfgSvc.Expect(c => c.GetImageLoaders()).Return(ldrs);
        }

        [Test]
        public void Ldr_CreateDefaultImageLoader_GivenDefault()
        {
            Given_MsDosRawFileFormat();
            mr.ReplayAll();
            var ldr = mr.PartialMock<Loader>(sc);
            ldr.Replay();

            ldr.DefaultToFormat = "ms-dos-com";
            var imgLoader = ldr.CreateDefaultImageLoader("foo.com", new byte[30]);
            var program = imgLoader.Load(null);
        }

        public class TestImageLoader : ImageLoader
        {
            public TestImageLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
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

        [Test(Description = "Validate that entry points are added to the Program instance when loading a 'raw' file.")]
        public void Ldr_RawFileEntryPoint()
        {
            var arch = mr.Stub<IProcessorArchitecture>();
            var openv = mr.Stub<OperatingEnvironment>();
            cfgSvc.Stub(s => s.GetArchitecture("mmix")).Return(arch);
            cfgSvc.Stub(s => s.GetEnvironment(null)).Return(openv);
            cfgSvc.Stub(s => s.GetImageLoader("zlorgo")).Return(new LoaderElementImpl {
                TypeName = typeof(NullImageLoader).FullName,
            });
            openv.Stub(o => o.Load(null, null)).IgnoreArguments().Return(new DefaultPlatform(sc, arch));
            arch.Stub(a => a.LoadUserOptions(null)).IgnoreArguments();
            arch.Stub(a => a.Name).Return("mmix");
            arch.Stub(a => a.PostprocessProgram(null)).IgnoreArguments();
            arch.Stub(a => a.TryParseAddress(
                Arg<string>.Is.Equal("00123500"),
                out Arg<Address>.Out(Address.Ptr32(0x00123500)).Dummy)).Return(true);
            mr.ReplayAll();

            var ldr = new Loader(sc);
            var program = ldr.LoadRawImage("foo.bin", new byte[0], Address.Ptr32(0x00123400), new LoadDetails
            {
                ArchitectureName = "mmix",
                EntryPoint = new EntryPointElement {  Address = "00123500" },
                LoaderName = "zlorgo",
            });

            Assert.AreEqual(1, program.EntryPoints.Count);
            Assert.AreEqual(SymbolType.Procedure, program.EntryPoints[Address.Ptr32(0x00123500)].Type);
            mr.VerifyAll();
        }
	}
}

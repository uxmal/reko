#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Configuration;
using Decompiler.Core.Services;
using Decompiler.Loading;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;

namespace Decompiler.UnitTests.Loading
{
	[TestFixture]
	public class LoaderTests
	{
        private MockRepository mr;
        private IServiceContainer sc;
        private FakeDecompilerEventListener eventListener;
        private IConfigurationService dcSvc;
        private List<SignatureFileElement> signatureFiles;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            eventListener = new FakeDecompilerEventListener();
            dcSvc = mr.Stub<IConfigurationService>();
            signatureFiles = new List<SignatureFileElement>();
            sc.AddService<DecompilerEventListener>(eventListener);
            sc.AddService<IConfigurationService>(dcSvc);
            dcSvc.Stub(d => d.GetSignatureFiles()).Return(signatureFiles);
        }

        [Test]
        public void LoaderMatch()
        {
            mr.ReplayAll();

            Loader ldr = new Loader(sc);
            Assert.IsTrue(ldr.ImageHasMagicNumber(new byte[] { 0x47, 0x11 }, "4711", "0"));

            mr.VerifyAll();
        }

        [Test]
        public void LoaderUnknownImageType()
        {
            dcSvc.Stub(d => d.GetImageLoaders()).Return(new ArrayList());

            var testImage = new byte[] { 42, 42, 42, 42, };
            mr.ReplayAll();
            Loader ldr = mr.PartialMock<Loader>(sc);
            ldr.Replay();

            Program prog = ldr.LoadExecutable("", testImage, null);

            Assert.AreEqual("ErrorDiagnostic -  - The format of the file is unknown." , eventListener.LastDiagnostic);
            Assert.AreEqual(0, prog.Image.BaseAddress.Offset);
            Assert.IsNull(prog.Architecture);
            Assert.IsAssignableFrom<DefaultPlatform>(prog.Platform);
            mr.VerifyAll();
        }

        [Test]
        public void LoaderAtOffset()
        {
            dcSvc.Stub(d => d.GetImageLoaders()).Return(new ArrayList
            {
                new LoaderElementImpl {
                    Offset = "0002",
                    MagicNumber = "A0A0",
                    TypeName = typeof(TestImageLoader).AssemblyQualifiedName,
                }
            });
            var testImage = new byte[] { 42, 42, 0xA0, 0xA0 };
            mr.ReplayAll();
            Loader ldr = mr.PartialMock<Loader>(sc);
            ldr.Stub(l => l.LoadImageBytes("", 0)).Return(testImage);
            mr.ReplayAll();

            var imgLoader = ldr.FindImageLoader<ImageLoader>("", testImage, () => null);

            Assert.IsInstanceOf<TestImageLoader>(imgLoader);
            mr.VerifyAll();
        }

        public class TestImageLoader : ImageLoader
        {
            public TestImageLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
            {
            }

            public override Address PreferredBaseAddress
            {
                get { throw new NotImplementedException(); }
            }

            public override Program Load(Address addrLoad)
            {
                throw new NotImplementedException();
            }

            public override RelocationResults Relocate(Address addrLoad)
            {
                throw new NotImplementedException();
            }
        }
	}
}

#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Services;
using Decompiler.Loading;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
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
        FakeDecompilerEventListener eventListener;
        private IDecompilerConfigurationService dcSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            eventListener = new FakeDecompilerEventListener();
            dcSvc = mr.Stub<IDecompilerConfigurationService>();
            sc.AddService(typeof(DecompilerEventListener), eventListener);
        }

        [Test]
        public void LoaderMatch()
        {
            mr.ReplayAll();

            TestLoader ldr = new TestLoader(dcSvc, sc);
            Assert.IsTrue(ldr.ImageHasMagicNumber(new byte[] { 0x47, 0x11 }, "4711", "0"));

            mr.VerifyAll();
        }

        [Test]
        public void LoaderUnknownImageType()
        {
            dcSvc.Stub(d => d.GetImageLoaders()).Return(new ArrayList());
            mr.ReplayAll();

            TestLoader ldr = new TestLoader(dcSvc, sc);
            ldr.Test_Image = new byte[] { 42, 42, 42, 42, };
            Program prog = ldr.Load(ldr.Test_Image, null);

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
            mr.ReplayAll();

            TestLoader ldr = new TestLoader(dcSvc, sc);
            ldr.Test_Image = new byte[] { 42, 42, 0xA0, 0xA0 };
            var imgLoader = ldr.FindImageLoader(ldr.Test_Image);

            Assert.IsInstanceOf<TestImageLoader>(imgLoader);
            mr.VerifyAll();
        }

		private class TestLoader : Loader
		{
			public TestLoader(IDecompilerConfigurationService config, IServiceProvider services)
                : base(config, services)
			{
			}

			private byte [] image;

			public byte [] Test_Image
			{
				get { return image; }
				set { image = value; }
			}

			public override byte[] LoadImageBytes(string fileName, int offset)
			{
				return image;
			}
		}

        public class TestImageLoader : ImageLoader
        {
            public TestImageLoader(IServiceProvider services, byte[] imgRaw) : base(services, imgRaw)
            {
            }

            public override Address PreferredBaseAddress
            {
                get { throw new NotImplementedException(); }
            }

            public override LoaderResults Load(Address addrLoad)
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

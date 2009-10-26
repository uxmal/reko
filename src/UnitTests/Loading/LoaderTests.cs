/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Loading;
using Decompiler.Scanning;
using NUnit.Framework;
using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Decompiler.UnitTests.Loading
{
	[TestFixture]
	public class LoaderTests
	{
		[Test]
		public void LoadProjectFileNoBom()
		{
			byte [] image = new UTF8Encoding(false).GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?><project xmlns=\"http://schemata.jklnet.org/Decompiler\">" +
				"<input><filename>foo.bar</filename></input></project>");
            TestLoader ldr = new TestLoader(new Program(), new FakeDecompilerHost());
			ldr.Image = image;
			DecompilerProject project = ldr.Load(null);
			Assert.AreEqual("foo.bar", project.Input.Filename);
		}

        [Test]
        public void Match()
        {
            TestLoader ldr = new TestLoader(new Program(), new FakeDecompilerHost());
            Assert.IsTrue(ldr.ImageBeginsWithMagicNumber(new byte[] { 0x47, 0x11 }, "4711"));
        }

        [Test]
        public void LoadUnknownImageType()
        {
            FakeDecompilerHost host = new FakeDecompilerHost();
            Program prog = new Program();
            TestLoader ldr = new TestLoader(prog, host);
            ldr.Image = new byte[] { 42, 42, 42, 42, };
            DecompilerProject project = ldr.Load(null);
            Assert.AreEqual("Warning - 00000000: The format of the file test.bin is unknown; you will need to specify it manually." , host.LastDiagnostic);
            Assert.AreEqual(0, ldr.Program.Image.BaseAddress.Offset);
            Assert.IsNull(ldr.Program.Architecture);
            Assert.IsNull(ldr.Program.Platform);

        }

		private class TestLoader : Loader
		{
			public TestLoader(Program prog, DecompilerHost host)
				: base("test.bin", prog, host)
			{
			}

			private byte [] image;

			public byte [] Image
			{
				get { return image; }
				set { image = value; }
			}

			public override byte[] LoadImageBytes(string fileName, int offset)
			{
				return image;
			}

		}
	}
}

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
			TestLoader ldr = new TestLoader(new Program());
			ldr.Image = image;
			DecompilerProject project = ldr.Load(null);
			Assert.AreEqual("foo.bar", project.Input.Filename);
		}

		private class TestLoader : Loader
		{
			public TestLoader(Program prog)
				: base("", prog)
			{
			}

			private bool allowLoadExecutable;
			private byte [] image;

			public bool AllowLoadExecutable
			{
				get { return allowLoadExecutable; }
				set { allowLoadExecutable = value; }
			}

			public byte [] Image
			{
				get { return image; }
				set { image = value; }
			}

			public override byte[] LoadImageBytes(string fileName, int offset)
			{
				return image;
			}

			public override void LoadExecutable(string pstrFileName, Address addrLoad)
			{
				if (allowLoadExecutable)
					base.LoadExecutable(pstrFileName, addrLoad);
			}

		}
	}
}

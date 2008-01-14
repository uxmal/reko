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

using Decompiler.Core;
using Decompiler.Loading;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Loading
{
	[TestFixture]
	public class PkLiteUnpackerTests
	{
		[Test]
		public void PkLiteLoad()
		{
			Program prog = new Program();
			Loader l = new Loader(prog);
			
			prog.Image = new ProgramImage(new Address(0xC00, 0), l.LoadImageBytes(FileUnitTester.MapTestPath("binaries/life.exe"), 0));
			ExeImageLoader exe = new ExeImageLoader(prog, prog.Image.Bytes);
			PkLiteUnpacker ldr = new PkLiteUnpacker(exe, prog.Image.Bytes);
			ProgramImage img = ldr.Load(new Address(0xC00, 0));
			Assert.AreEqual(0x19EC0, img.Bytes.Length);
			ldr.Relocate(new Address(0xC00, 0), new ArrayList());
		}
	}
}

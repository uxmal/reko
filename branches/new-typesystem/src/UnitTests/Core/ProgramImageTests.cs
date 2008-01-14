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
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ProgramImageTests
	{
		[Test]
		public void PriReadLiterals()
		{
			byte [] bytes = new byte [] { 0x01, 0x00, 0xFE, 0x80, 
											0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x3F,
											0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F,
											0xF1, 0x68, 0xE3, 0x88, 0xB5, 0xF8, 0xE4, 0x3E,
											0x27, 0x10, 0x10, 0x10, 0x10, 0x10, 0x80, 0x3F,
											0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x49, 0x40,
			};
			ProgramImage img = new ProgramImage(new Address(0xC00, 0), bytes);
			Assert.AreEqual(-0x7F01FFFF, img.ReadInt(0));
			Assert.AreEqual(0.5, (double) img.ReadDouble(0x04).Value, 0.00001);
			Assert.AreEqual(1.0, (double) img.ReadDouble(0x0C).Value, 0.00001);
			Assert.AreEqual(1e-5, (double) img.ReadDouble(0x14).Value, 1e-10);
			Assert.AreEqual(0.007843137254902, (double) img.ReadDouble(0x1C).Value, 1e-8);
			Assert.AreEqual(51.0, (double) img.ReadDouble(0x24).Value, 0.00001);
		}

		//$TODO: little endian / big-endian reader.

		[Test]
		public void UShortFixup()
		{
			byte [] bytes = new byte[] { 0x01, 0x02, 0x03 };

			ProgramImage img = new ProgramImage(new Address(0x0C00, 0), bytes);
			ushort newSeg = img.FixupUShort(1, 0x4444);
			Assert.AreEqual(0x4746, newSeg);
		}

		[Test]
		public void MapTests()
		{
			ProgramImage img = new ProgramImage(new Address(0x1231000), new byte [256]);
			Assert.IsNotNull(img.Map);
			Assert.AreEqual(1, img.Map.Items.Count);
		}
	}
}
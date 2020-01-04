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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Text;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class ProgramImageTests
	{
		[Test]
		public void PriReadLiterals()
		{
			var bytes = new byte [] { 
                0x01, 0x00, 0xFE, 0x80, 
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x3F,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F,
				0xF1, 0x68, 0xE3, 0x88, 0xB5, 0xF8, 0xE4, 0x3E,
				0x27, 0x10, 0x10, 0x10, 0x10, 0x10, 0x80, 0x3F,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x49, 0x40,
			};
			var img = new MemoryArea(Address.SegPtr(0xC00, 0), bytes);
			Assert.AreEqual(-0x7F01FFFF, img.ReadLeInt32(0));
			Assert.AreEqual(0.5, img.ReadLeDouble(0x04).ToDouble(), 0.00001);
            Assert.AreEqual(1.0, img.ReadLeDouble(0x0C).ToDouble(), 0.00001);
            Assert.AreEqual(1e-5, img.ReadLeDouble(0x14).ToDouble(), 1e-10);
			Assert.AreEqual(0.007843137254902, img.ReadLeDouble(0x1C).ToDouble(), 1e-8);
			Assert.AreEqual(51.0, img.ReadLeDouble(0x24).ToDouble(), 0.00001);
		}

		[Test]
		public void UShortFixup()
		{
			var bytes = new byte[] { 0x01, 0x02, 0x03 };
			var img = new MemoryArea(Address.SegPtr(0x0C00, 0), bytes);
			ushort newSeg = img.FixupLeUInt16(1, 0x4444);
			Assert.AreEqual(0x4746, newSeg);
		}

		[Test]
		public void ReadLeUShort()
		{
			MemoryArea img = new MemoryArea(Address.Ptr32(0x10000), new byte[] {
				0x78, 0x56, 0x34, 0x12 });
			Constant c = img.ReadLe(2, PrimitiveType.Word16);
			Assert.AreSame(PrimitiveType.Word16, c.DataType);
			Assert.AreEqual("0x1234", c.ToString());
		}

		[Test]
		public void ReadLeUInt32()
		{
			MemoryArea img = new MemoryArea(Address.Ptr32(0x10000), new byte[] {
				0x78, 0x56, 0x34, 0x12 });
			Constant c = img.ReadLe(0, PrimitiveType.Word32);
			Assert.AreSame(PrimitiveType.Word32, c.DataType);
			Assert.AreEqual("0x12345678", c.ToString());
		}

		[Test]
		public void ReadLeNegativeInt()
		{
			MemoryArea img = new MemoryArea(Address.Ptr32(0x10000), new byte[] {
				0xFE, 0xFF, 0xFF, 0xFF });
			Constant c = img.ReadLe(0, PrimitiveType.Int32);
			Assert.AreSame(PrimitiveType.Int32, c.DataType);
			Assert.AreEqual("-2", c.ToString());
		}
	}
}
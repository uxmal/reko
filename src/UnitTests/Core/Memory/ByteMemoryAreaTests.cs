#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Types;

namespace Reko.UnitTests.Core.Memory
{
    [TestFixture]
	public class ByteMemoryAreaTests
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
			var img = new ByteMemoryArea(Address.SegPtr(0xC00, 0), bytes);
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
			var img = new ByteMemoryArea(Address.SegPtr(0x0C00, 0), bytes);
			ushort newSeg = img.FixupLeUInt16(1, 0x4444);
			Assert.AreEqual(0x4746, newSeg);
		}

		[Test]
		public void TryReadLeUShort()
		{
			ByteMemoryArea img = new ByteMemoryArea(Address.Ptr32(0x10000), new byte[] {
				0x78, 0x56, 0x34, 0x12 });
			Assert.IsTrue(img.TryReadLe(2, PrimitiveType.Word16, out Constant c));
			Assert.AreSame(PrimitiveType.Word16, c.DataType);
			Assert.AreEqual("0x1234<16>", c.ToString());
		}

		[Test]
		public void TryReadLeUInt32()
		{
			ByteMemoryArea img = new ByteMemoryArea(Address.Ptr32(0x10000), new byte[] {
				0x78, 0x56, 0x34, 0x12 });
			Assert.IsTrue(img.TryReadLe(0, PrimitiveType.Word32, out Constant c));
			Assert.AreSame(PrimitiveType.Word32, c.DataType);
			Assert.AreEqual("0x12345678<32>", c.ToString());
		}

		[Test]
		public void TryReadLeNegativeInt()
		{
			ByteMemoryArea img = new ByteMemoryArea(Address.Ptr32(0x10000), new byte[] {
				0xFE, 0xFF, 0xFF, 0xFF });
			Assert.IsTrue(img.TryReadLe(0, PrimitiveType.Int32, out Constant c));
			Assert.AreSame(PrimitiveType.Int32, c.DataType);
			Assert.AreEqual("-2<i32>", c.ToString());
		}
	}
}
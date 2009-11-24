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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class StructureTypeTests
	{
		[Test]
		public void SimplifySimpleStruct()
		{
			StructureType s = new StructureType(null, 0, new StructureField(0, PrimitiveType.Int32));
			DataType dt = s.Simplify();
			Assert.AreEqual("int32", dt.ToString());
		}

	
		[Test]
		public void SimplifyNonSimpleStruct()
		{
			StructureType s = new StructureType(null, 0, new StructureField(4, PrimitiveType.Int32));
			DataType dt = s.Simplify();
			Assert.AreEqual("(struct (4 int32 dw0004))", dt.ToString());
		}

		[Test]
		public void StructureSizeInHex()
		{
			StructureType s = new StructureType(null, 0x42);
			Assert.AreEqual("(struct 42)", s.ToString());
		}

		[Test]
		public void DontSimplifySegmentStruct()
		{
			StructureType s = new StructureType(null, 0, new StructureField(0, PrimitiveType.Int32));
			s.IsSegment = true;
			DataType dt = s.Simplify();
			Assert.AreEqual("(segment (0 int32 dw0000))", dt.ToString());
		}

        [Test]
        public void CreateField()
        {
            int off = StructureField.ToOffset(new Constant(PrimitiveType.Word16, 4));
            Assert.AreEqual(4, off);
        }

        [Test]
        public void CreateFieldWithLargeOffset()
        {
            ushort s = 0xC004;
            int off = StructureField.ToOffset(new Constant(PrimitiveType.Word16, s));
            Assert.AreEqual(0xC004, off);
        }
	}
}

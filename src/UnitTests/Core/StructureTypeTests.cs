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

using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class StructureTypeTests
	{
		[Test]
		public void SimplifySimpleStruct()
		{
            StructureType s = new StructureType(null, 0) { Fields = { { 0, PrimitiveType.Int32 } } };
			DataType dt = s.Simplify();
			Assert.AreEqual("int32", dt.ToString());
		}
	
		[Test]
		public void SimplifyNonSimpleStruct()
		{
            StructureType s = new StructureType(null, 0) { Fields = { { 4, PrimitiveType.Int32 } } };
			DataType dt = s.Simplify();
			Assert.AreEqual("(struct (4 int32 dw0004))", dt.ToString());
		}

		[Test]
		public void StructureSizeInHex()
		{
			StructureType s = new StructureType(null, 0x42);
			Assert.AreEqual("(struct 0042)", s.ToString());
		}

		[Test]
		public void DontSimplifySegmentStruct()
		{
			StructureType s = new StructureType(null, 0) { Fields = { { 0, PrimitiveType.Int32 } } };
			s.IsSegment = true;
			DataType dt = s.Simplify();
			Assert.AreEqual("(segment (0 int32 dw0000))", dt.ToString());
		}

        [Test]
        public void CreateField()
        {
            int off = StructureField.ToOffset(Constant.Word16(4));
            Assert.AreEqual(4, off);
        }

        [Test]
        public void CreateFieldWithLargeOffset()
        {
            ushort s = 0xC004;
            int off = StructureField.ToOffset(Constant.Word16(s));
            Assert.AreEqual(0xC004, off);
        }

        [Test]
        public void CloneSelfReferencingStructure()
        {
            var str = new StructureType("str", 12, true);
            str.Fields.Add(0, PrimitiveType.Int32, "i");
            str.Fields.Add(4, new Pointer(str, 32), "ptr");
            str.Fields.Add(8, PrimitiveType.Real32, "f");

            var clonedStr = (StructureType)str.Clone();
            var field = clonedStr.Fields.AtOffset(4);
            var nestedStr = ((Pointer)field.DataType).Pointee;
            Assert.AreNotSame(clonedStr, str);
            Assert.AreSame(clonedStr, nestedStr);
            Assert.AreEqual(
                "(struct \"str\" 000C (0 int32 i) (4 (ptr32 (struct \"str\" 000C)) ptr) (8 real32 f))",
                clonedStr.ToString());
        }
    }
}

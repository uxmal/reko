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
using Reko.Core.Types;
using Reko.Typing;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Typing
{
	[TestFixture]
	public class TypeTransformArrayTests
	{
		private TypeFactory factory;
		private TypeStore store;
		private TypeTransformer trans;

		[Test]
		public void MergeOffsetStructures()
		{
			StructureType s1 = new StructureType(null, 20);
			s1.Fields.Add(0, PrimitiveType.Int32);
			StructureType s2 = new StructureType(null, 20);
			s2.Fields.Add(0, PrimitiveType.Real32);
			DataType dt = trans.MergeOffsetStructures(s1, 4, s2, 8);
			Assert.AreEqual("(struct 0014 (0 int32 dw0000) (4 real32 r0004))", dt.ToString());
		}

		[Test]
		public void MergeStaggeredArrays()
		{
			StructureType s = BuildStaggeredArrays();
			trans.MergeStaggeredArrays(s);
			Assert.AreEqual("(struct (4 (arr (struct 0014 (0 int32 dw0000) (4 real64 r0004) (8 byte b0008))) a0004))", s.ToString());
		}

		[Test]
		public void MergeStaggeredArrays2()
		{
			StructureType s = new StructureType(null, 0);
			AddArrayField(s, 0, 8, PrimitiveType.Int32);
			AddArrayField(s, 4, 8, PrimitiveType.Int32);
			trans.MergeStaggeredArrays(s);
			Assert.AreEqual("(struct (0 (arr (struct 0008 (0 int32 dw0000) (4 int32 dw0004))) a0000))", s.ToString());
		}

		[Test]
		public void MergeDistinctArrays()
		{
			StructureType s = BuildDistinctArrays();
			trans.MergeStaggeredArrays(s);
			Assert.AreEqual("(struct (0 (arr (struct 0014 (0 int32 dw0000) (4 int32 dw0004))) a0000) (28 (arr (struct 0014 (0 real32 r0000) (4 real32 r0004))) a0028))", s.ToString());
		}

		private StructureType BuildStaggeredArrays()
		{
			StructureType s = new StructureType(null, 0);
			s.Fields.Add(4, new ArrayType(new StructureType(null, 20) { Fields = { { 0, PrimitiveType.Int32 } } }, 0));
			s.Fields.Add(8, new ArrayType(new StructureType(null, 20) { Fields = { { 0, PrimitiveType.Real64} } }, 0));
			s.Fields.Add(12,new ArrayType(new StructureType(null, 20) { Fields = { { 0, PrimitiveType.Byte } } }, 0));
			return s;
		}

		private StructureType BuildDistinctArrays()
		{
			StructureType s = new StructureType(null, 0);
			AddArrayField(s, 0, 20, PrimitiveType.Int32);
			AddArrayField(s, 4, 20, PrimitiveType.Int32);
			AddArrayField(s, 40, 20, PrimitiveType.Real32);
			AddArrayField(s, 44, 20, PrimitiveType.Real32);
			return s;
		}

		private void AddArrayField(StructureType s, int off, int elemSize, DataType elemType)
		{
            s.Fields.Add(off, new ArrayType(new StructureType(null, elemSize) { Fields = { { 0, elemType } } }, 0));
		}

		[SetUp]
		public void Setup()
		{
			factory = new TypeFactory();
			store = new TypeStore();
			trans = new TypeTransformer(factory, store, null);
		}
	}
}

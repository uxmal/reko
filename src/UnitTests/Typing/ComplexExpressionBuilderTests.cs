/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class ComplexExpressionBuilderTests
	{
		private TypeStore store;
		private TypeFactory factory;
		private Pointer ptrPoint;
		private Pointer ptrUnion;
        private Pointer ptrInt;
        private Pointer ptrWord;

        [SetUp]
		public void Setup()
		{
			store = new TypeStore();
			factory = new TypeFactory();
			StructureType point = new StructureType("Point", 0);
			point.Fields.Add(0, PrimitiveType.Word32, null);
			point.Fields.Add(4, PrimitiveType.Word32, null);
			TypeVariable tvPoint = store.EnsureExpressionTypeVariable(factory, null);
			EquivalenceClass eq = new EquivalenceClass(tvPoint);
			tvPoint.DataType = eq;
			eq.DataType = point;
			ptrPoint = new Pointer(eq, 4);

			UnionType u = new UnionType("RealInt", null);
			u.Alternatives.Add(new UnionAlternative("w", PrimitiveType.Word32));
			u.Alternatives.Add(new UnionAlternative("r", PrimitiveType.Real32));
			TypeVariable tvUnion = store.EnsureExpressionTypeVariable(factory, null);
			eq = new EquivalenceClass(tvUnion);
			tvUnion.DataType = eq;
			eq.DataType = u;
			ptrUnion = new Pointer(eq, 4);

            ptrInt = new Pointer(PrimitiveType.Int32, 4);
            ptrWord = new Pointer(PrimitiveType.Word32, 4);

		}

		[Test]
		public void BuildPrimitive()
		{
			Identifier id = new Identifier("id", 3, PrimitiveType.Word32, null);
            ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(PrimitiveType.Word32, PrimitiveType.Word32, PrimitiveType.Word32, null, id, null, 0);
			Assert.AreEqual("id", ceb.BuildComplex().ToString());
		}

		[Test]
		public void BuildPointer()
		{
			Identifier ptr = new Identifier("ptr", 3, PrimitiveType.Word32, null);
			store.EnsureExpressionTypeVariable(factory, ptr);
			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(ptrInt, ptrPoint, new Pointer(PrimitiveType.Word32, 4), null, ptr, null, 0);
			Assert.AreEqual("&ptr->dw0000", ceb.BuildComplex().ToString());
		}

		[Test]
		public void BuildPointerFetch()
		{
			Identifier ptr = new Identifier("ptr", 3, PrimitiveType.Word32, null);
            ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(ptrInt, ptrPoint, new Pointer(PrimitiveType.Word32, 4), null, ptr, null, 0);
            ceb.Dereferenced = true;
			Assert.AreEqual("ptr->dw0000", ceb.BuildComplex().ToString());
		}

		[Test]
		public void BuildUnionFetch()
		{
			Identifier ptr = new Identifier("ptr", 3, PrimitiveType.Word32, null);
			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                ptrWord,
				ptrUnion,
				new Pointer(PrimitiveType.Real32, 4),
				null, ptr, null, 0);
			ceb.Dereferenced = true;
			Assert.AreEqual("ptr->r", ceb.BuildComplex().ToString());
		}

		[Test]
		public void BuildByteArrayFetch()
		{
            Identifier globals = new Identifier("globals", 3, PrimitiveType.Word32, null);
            Identifier i = new Identifier("i", 4, PrimitiveType.Word32, null);
            ProcedureMock m = new ProcedureMock();
            DataType arrayOfBytes = new ArrayType(PrimitiveType.Byte, 0);
            StructureType str = Struct(
                Fld(0x01000, arrayOfBytes));
            CreateTv(globals, Ptr32(str), Ptr32(PrimitiveType.Byte));
            CreateTv(i, PrimitiveType.Int32, PrimitiveType.Word32);
            ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                PrimitiveType.Byte,
                globals.TypeVariable.DataType,
                globals.TypeVariable.OriginalDataType,
                null,
                globals, i, 0x1000);
            ceb.Dereferenced = true;
            Assert.AreEqual("globals->a1000[i]", ceb.BuildComplex().ToString());
		}

        private StructureType Struct(params StructureField [] fields)
        {
            StructureType str = new StructureType();
            foreach (StructureField f in fields)
            {
                str.Fields.Add(f);
            }
            return str;
        }

        private StructureType Segment(params StructureField[] fields)
        {
            StructureType str = new StructureType();
            str.IsSegment = true;
            foreach (StructureField f in fields)
            {
                str.Fields.Add(f);
            }
            return str;
        }

        private StructureField Fld(int offset, DataType dt)
        {
            return new StructureField(offset, dt);
        }

        [Test]
        public void BuildMemberAccessFetch()
        {
            Identifier ds = new Identifier("ds", 3, PrimitiveType.SegmentSelector, null);
            Identifier bx = new Identifier("bx", 3, PrimitiveType.Word16, null);
            SegmentedAccess sa = new SegmentedAccess(null, ds, bx, PrimitiveType.Word16);
            TypeVariable tvDs = CreateTv(ds, Ptr16(Segment()), ds.DataType); 
            TypeVariable tvBx = CreateTv(bx, MemPtr(Segment(), PrimitiveType.Word16), MemPtr(new TypeVariable(43), PrimitiveType.Word16));
            ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                new Pointer(PrimitiveType.Word16, 2),
                tvBx.Class.DataType, tvBx.OriginalDataType, ds, bx, null, 0);
            ceb.Dereferenced = true;
            Assert.AreEqual("ds->*bx", ceb.BuildComplex().ToString());
        }

        private Pointer Ptr32(DataType dataType)
        {
            return new Pointer(dataType, 4);
        }

        private Pointer Ptr16(DataType dataType)
        {
            return new Pointer(dataType, 2);
        }

        private MemberPointer MemPtr(DataType baseType, DataType fieldType)
        {
            return new MemberPointer(
                new Pointer(baseType, 2),
                fieldType, 2);
        }

        private TypeVariable CreateTv(Expression e, DataType dt, DataType dtOrig)
        {
            TypeVariable tv = store.EnsureExpressionTypeVariable(factory, e);
            tv.DataType = dt;
            tv.OriginalDataType = dtOrig;
            e.TypeVariable = tv;
            if (dt.IsComplex)
            {
                tv.Class = new EquivalenceClass(tv);
                tv.Class.DataType = dt;
            }
            return tv;
        }

	}
}

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
using Reko.Typing;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Typing
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
        private Identifier globals;
        private ExpressionEmitter m;

        [SetUp]
		public void Setup()
		{
			store = new TypeStore();
			factory = new TypeFactory();
            globals = new Identifier("globals", PrimitiveType.Word32, null);
            StructureType point = new StructureType("Point", 0)
            {
                Fields = {
                    { 0, PrimitiveType.Word32, null },
			        { 4, PrimitiveType.Word32, null }
                }
            };
			TypeVariable tvPoint = store.CreateTypeVariable(factory);
            EquivalenceClass eq = new EquivalenceClass(tvPoint)
            {
                DataType = point
            };
			tvPoint.DataType = eq;
			ptrPoint = new Pointer(eq, 32);

            UnionType u = new UnionType("RealInt", null)
            {
                Alternatives = {
                    new UnionAlternative("w", PrimitiveType.Word32, 0),
			        new UnionAlternative("r", PrimitiveType.Real32, 1),
                }
            };
			TypeVariable tvUnion = store.CreateTypeVariable(factory);
            eq = new EquivalenceClass(tvUnion)
            {
                DataType = u
            };
			tvUnion.DataType = eq;
			ptrUnion = new Pointer(eq, 32);

            ptrInt = new Pointer(PrimitiveType.Int32, 32);
            ptrWord = new Pointer(PrimitiveType.Word32, 32);
            m = new ExpressionEmitter();
		}

        private Pointer Ptr32(DataType dataType)
        {
            return new Pointer(dataType, 32);
        }

        private Pointer Ptr16(DataType dataType)
        {
            return new Pointer(dataType, 16);
        }

        private MemberPointer MemPtr(DataType baseType, DataType fieldType)
        {
            return new MemberPointer(
                new Pointer(baseType, 16),
                fieldType, 2);
        }

        private StructureType Struct(params StructureField[] fields)
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

        private static ComplexExpressionBuilder CreateBuilder(
            DataType dtField,
            Expression basePtr,
            Expression complex,
            Expression index = null,
            int offset = 0)
        {
            return new ComplexExpressionBuilder(dtField, basePtr, complex, index, offset);
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

		[Test]
        public void CEB_BuildPrimitive()
		{
			var id = new Identifier("id", PrimitiveType.Word32, null);
            var ceb = new ComplexExpressionBuilder(PrimitiveType.Word32, null, id, null, 0);
			Assert.AreEqual("id", ceb.BuildComplex(false).ToString());
		}

		[Test]
        public void CEB_BuildPointer()
		{
			var ptr = new Identifier("ptr", PrimitiveType.Word32, null);
            CreateTv(ptr, ptrPoint, Ptr32(PrimitiveType.Word32));
			var ceb = new ComplexExpressionBuilder(PrimitiveType.Word32, null, ptr, null, 0);
			Assert.AreEqual("&ptr->dw0000", ceb.BuildComplex(false).ToString());
		}

		[Test]
        public void CEB_BuildPointerFetch()
		{
			var ptr = new Identifier("ptr", PrimitiveType.Word32, null);
            CreateTv(ptr, ptrPoint, Ptr32(PrimitiveType.Word32));
            var ceb = new ComplexExpressionBuilder(PrimitiveType.Word32, null, ptr, null, 0);
			Assert.AreEqual("ptr->dw0000", ceb.BuildComplex(true).ToString());
		}

        [Test]
        public void CEB_BuildAddrOfPointer()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var addrOf = m.AddrOf(PrimitiveType.Ptr32, id);
            CreateTv(addrOf, Ptr32(Ptr32(PrimitiveType.Int32)), Ptr32(PrimitiveType.Word32));
            var ceb = CreateBuilder(PrimitiveType.Word32, null, addrOf);
            Assert.AreEqual("&id", ceb.BuildComplex(false).ToString());
        }

        [Test]
        public void CEB_BuildAddrOfPointerFetch()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var addrOf = m.AddrOf(PrimitiveType.Ptr32, id);
            CreateTv(addrOf, Ptr32(Ptr32(PrimitiveType.Int32)), Ptr32(PrimitiveType.Word32));
            var ceb = CreateBuilder(PrimitiveType.Word32, null, addrOf);
            Assert.AreEqual("id", ceb.BuildComplex(true).ToString());
        }

        [Test]
        public void CEB_BuildPointerToVoid()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            CreateTv(id, Ptr32(VoidType.Instance), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, 4);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(char *) id + 4", e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToUnknown()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var index = new Identifier("index", PrimitiveType.Word32, null);
            CreateTv(id, Ptr32(new UnknownType()), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, index, 0);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(char *) id + index", e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToCode()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var indexId = new Identifier("index", PrimitiveType.Word32, null);
            var index = m.IMul(indexId, 16);
            CreateTv(id, Ptr32(new CodeType()), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, index, -4);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual(
                "(char *) id + (index * 0x00000010 - 4)",
                e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToPointerToInteger()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var index = new Identifier("index", PrimitiveType.Word32, null);
            CreateTv(id, Ptr32(Ptr32(PrimitiveType.Int32)), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, index, -8);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(char *) id + (index - 8)", e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToStruct_MiddleOfTheField()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var str = Struct(Fld(4, PrimitiveType.Int32));
            CreateTv(id, Ptr32(str), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, 6);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(char *) &id->dw0004 + 2", e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToStruct_EndOfTheField()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var str = Struct(Fld(4, PrimitiveType.Int32));
            CreateTv(id, Ptr32(str), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, 8);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("&id->dw0004 + 1", e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToNestedStruct()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var nestedStr = Struct(
                Fld(0, PrimitiveType.Int32),
                Fld(4, PrimitiveType.Real32));
            var str = Struct(Fld(4, nestedStr));
            CreateTv(id, Ptr32(str), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, 8);
            var e = ceb.BuildComplex(true);
            Assert.AreEqual("id->t0004.r0004", e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToInteger()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            CreateTv(id, Ptr32(PrimitiveType.Int32), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, 6);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(char *) id + 6", e.ToString());
        }

        [Test]
        public void CEB_BuildUnionWithOffset()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            CreateTv(id, ptrUnion.Pointee, PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, 2);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(word32) id.w + 2", e.ToString());
        }

        [Test]
        public void CEB_BuildUnionWithNegativeOffset()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            CreateTv(id, ptrUnion.Pointee, PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, -2);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(word32) id.w - 2", e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToStruct_NoFieldAtGivenOffset()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            var str = Struct(Fld(8, PrimitiveType.Int32));
            CreateTv(id, Ptr32(str), PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, 4);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(char *) id + 4", e.ToString());
        }

        [Test]
        public void CEB_BuildPointerToUnion_NotFoundAlternative()
        {
            var id = new Identifier("id", PrimitiveType.Word32, null);
            CreateTv(id, ptrUnion, PrimitiveType.Real64);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, id, null, 2);
            var e = ceb.BuildComplex(false);
            Assert.AreEqual("(char *) id + 2", e.ToString());
        }

        [Test]
        public void CEB_BuildUnionFetch()
        {
            var ptr = new Identifier("ptr", PrimitiveType.Word32, null);
            CreateTv(ptr, ptrUnion, Ptr32(PrimitiveType.Real32));
            var ceb = new ComplexExpressionBuilder(PrimitiveType.Real32, null, ptr, null, 0);
            Assert.AreEqual("ptr->r", ceb.BuildComplex(true).ToString());
        }

        [Test]
		public void CEB_BuildByteArrayFetch()
		{
            var i = new Identifier("i", PrimitiveType.Word32, null);
            DataType arrayOfBytes = new ArrayType(PrimitiveType.Byte, 0);
            StructureType str = Struct(
                Fld(0x800, arrayOfBytes));
            str.Size = 0x2000;
            CreateTv(globals, Ptr32(str), Ptr32(PrimitiveType.Byte));
            CreateTv(i, PrimitiveType.Int32, PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Byte, null, globals, i, 0x800);
            Assert.AreEqual("globals->a0800[i]", ceb.BuildComplex(true).ToString());
		}

        [Test]
        public void CEB_BuildByteArrayFetch_Nondereferenced()
        {
            var i = new Identifier("i", PrimitiveType.Word32, null);
            DataType arrayOfBytes = new ArrayType(PrimitiveType.Byte, 0);
            StructureType str = Struct(
                Fld(0x01000, arrayOfBytes));
            str.Size = 0x2000;
            CreateTv(globals, Ptr32(str), Ptr32(PrimitiveType.Byte));
            CreateTv(i, PrimitiveType.Int32, PrimitiveType.Word32);
            var ceb = CreateBuilder(PrimitiveType.Byte, null, globals, i, 0x1000);
            Assert.AreEqual("globals->a1000 + i", ceb.BuildComplex(false).ToString());
        }

        [Test]
        public void CEB_SegmentedArray()
        {
            var m = new Reko.UnitTests.Mocks.ProcedureBuilder("CEB_SegmentedArray");
            var aw = new ArrayType(PrimitiveType.Int16, 0);
            var ds = m.Temp(PrimitiveType.SegmentSelector, "ds");
            var bx = m.Temp(PrimitiveType.Word16, "bx");
            var acc =
                m.Array(
                    PrimitiveType.Word16,
                    m.Seq(
                        ds,
                        m.Word16(0x5388)),
                    m.IMul(bx, 2));

            var seg = Segment();
            CreateTv(globals, Ptr32(factory.CreateStructureType()), Ptr32(factory.CreateStructureType()));
            CreateTv(ds, Ptr16(seg), Ptr16(factory.CreateStructureType()));
            var ceb = CreateBuilder(PrimitiveType.Int16, ds, bx);
        }

        [Test]
        public void CEB_BuildMemberAccessFetch()
        {
            var ds = new Identifier("ds", PrimitiveType.SegmentSelector, null);
            var bx = new Identifier("bx", PrimitiveType.Word16, null);
            CreateTv(ds, Ptr16(Segment()), ds.DataType);
            CreateTv(bx, MemPtr(Segment(), PrimitiveType.Word16), MemPtr(new TypeVariable(43), PrimitiveType.Word16));
            var ceb = CreateBuilder(null, ds, bx);
            Assert.AreEqual("ds->*bx", ceb.BuildComplex(true).ToString());
        }

        [Test]
        public void CEB_MemberOffset()
        {
            var dtPseg = Ptr16(Segment());
            var ds = new Identifier("ds", dtPseg, null);
            var bx = new Identifier("bx", PrimitiveType.Word16, null);
            CreateTv(ds, dtPseg, ds.DataType);
            CreateTv(bx, MemPtr(dtPseg, PrimitiveType.Real32), MemPtr(new TypeVariable(43), PrimitiveType.Real32));
            var ceb = CreateBuilder(null, ds, bx);
            Assert.AreEqual("&(ds->*bx)", ceb.BuildComplex(false).ToString());
        }

        [Test]
        public void CEB_ArrayOfStructs()
        {
            var array = new ArrayType(new StructureType(8)
            {
                Fields =
                {
                    new StructureField(0, PrimitiveType.Word32),
                    new StructureField(4, PrimitiveType.Real32),
                }
            }, 0);
            var a = new Identifier("a", Ptr32(array), null);
            var i = new Identifier("i", PrimitiveType.Int32, null);
            CreateTv(a, array, array);
            var ceb = CreateBuilder(PrimitiveType.Word32, null, a, m.SMul(i, 8));
            Assert.AreEqual("a[i].dw0000", ceb.BuildComplex(true).ToString());
        }
    }
}

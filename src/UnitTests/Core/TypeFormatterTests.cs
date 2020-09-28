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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.IO;
using Reko.Core.Expressions;
using System.Collections.Generic;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class TypeFormatterTests
	{
        private readonly string nl = Environment.NewLine;

        private StringWriter sw;
        private TypeFormatter tyfo;
        private TypeReferenceFormatter tyreffo;
        private Mock<IProcessorArchitecture> arch;

        [SetUp]
        public void SetUp()
        {
            sw = new StringWriter();
            var tf = new TextFormatter(sw) { Indentation = 0 };
            tyfo = new TypeFormatter(tf);
            tf = new TextFormatter(sw) { Indentation = 0 };
            tyreffo = new TypeReferenceFormatter(tf);
            arch = new Mock<IProcessorArchitecture>();
        }
        
        [Test]
		public void TyfoInt()
		{
			DataType dt = PrimitiveType.Int32;
			tyreffo.WriteDeclaration(dt, "test");
			Assert.AreEqual("int32 test", sw.ToString());
		}

		[Test]
		public void TyfoBareInt()
		{
			DataType dt = PrimitiveType.Int32;
			tyreffo.WriteTypeReference(dt);
			Assert.AreEqual("int32", sw.ToString());
		}

		[Test]
		public void TyfoPtrReal()
		{
			DataType dt = new Pointer(PrimitiveType.Real32, 32);
			tyreffo.WriteDeclaration(dt, "test");
			Assert.AreEqual("real32 * test", sw.ToString());
		}

		[Test]
		public void TyfoBarePtrReal()
		{
			DataType dt = new Pointer(PrimitiveType.Real32, 32);
            tyreffo.WriteTypeReference(dt);
			Assert.AreEqual("real32 *", sw.ToString());
		}

		[Test]
		public void TyfoMem()
		{
			StructureType m = new StructureType( "foo", 0);
			m.Fields.Add(4, PrimitiveType.UInt16);
			m.Fields.Add(8, new Pointer(PrimitiveType.UInt32, 32));
			tyfo.Write(m, "bar");
			Assert.AreEqual(
@"struct foo {
	uint16 w0004;	// 4
	uint32 * ptr0008;	// 8
} bar",
				sw.ToString());
		}

		[Test]
		public void TyfoUnion()
		{
			UnionType u = new UnionType( "foo", null);
			u.Alternatives.Add(PrimitiveType.Real32);
			u.Alternatives.Add(PrimitiveType.Int32);
			tyfo.Write(u, "bar");
			Assert.AreEqual(
@"union foo {
	int32 u1;
	real32 u0;
} bar", 
				sw.ToString());
		}

		[Test]
		public void TyfoSelfref()
		{
			StructureType s = new StructureType("link", 0);
			s.Fields.Add(0, PrimitiveType.Int32, "data");
			s.Fields.Add(4, new Pointer(s, 32), "next");
			tyfo.Write(s, "list");
			Assert.AreEqual(
@"struct link {
	int32 data;	// 0
	struct link * next;	// 4
} list",
				sw.ToString());
		}

		[Test]
		public void TyfoCycle()
		{
			StructureType a = new StructureType("a", 0);
			StructureType b = new StructureType("b", 0);
			a.Fields.Add(0, new Pointer(b, 32), "pb");
			b.Fields.Add(0, new Pointer(a, 32), "pa");
			tyfo.Write(a, null);
			Assert.AreEqual(
@"struct b;
struct a {
	struct b * pb;	// 0
}",
			sw.ToString());
		}

		[Test]
		public void TyfoFn()
		{
			FunctionType fn = FunctionType.Func(
                new Identifier("", PrimitiveType.Int32, null), 
				new Identifier[] { new Identifier("", PrimitiveType.Word32, null) });
			tyreffo.WriteDeclaration(fn, "fn");
			Assert.AreEqual("int32 fn(word32)", 
				sw.ToString());
		}

		[Test]
		public void TyfoPfn()
		{
			FunctionType fn = FunctionType.Action(
				new Identifier[] { new Identifier("", PrimitiveType.Word32, null)});
			Pointer pfn = new Pointer(fn, 32);
			tyreffo.WriteDeclaration(pfn, "pfn");
			Assert.AreEqual("void (* pfn)(word32)", 
				sw.ToString());
		}

		[Test]
		public void TyfoMembptr()
		{
            var s = new StructureType("s", 0);
			MemberPointer mp = new MemberPointer(new Pointer(s, 32), PrimitiveType.Int32, 2);
			tyreffo.WriteDeclaration(mp, "mp");
			Assert.AreEqual("int32 s::* mp", sw.ToString());
		}

		[Test]
		public void TyfoManyArgs()
		{
            FunctionType fn = FunctionType.Action(
                    new Identifier("", PrimitiveType.Ptr32,  null),
                new Identifier("", PrimitiveType.Int64 , null));
			tyreffo.WriteDeclaration(fn, "fn");
			Assert.AreEqual("void fn(ptr32, int64)", sw.ToString());
		}

        [Test]
        public void TypeReference()
        {
            tyreffo = new TypeReferenceFormatter(new TextFormatter(sw));
            EquivalenceClass b = new EquivalenceClass(new TypeVariable(1));
            b.DataType = new StructureType("b", 0) { Fields = { { 4, PrimitiveType.Word32 } } };

            tyfo.Write(new Pointer(b, 16), "pb");
            Assert.AreEqual("b * pb", sw.ToString());
        }

        [Test]
        public void TyfoMemberPointerMembers()
        {
            StructureType seg = new StructureType("seg", 0);

            StructureType s = new StructureType("s", 0);
            s.Fields.Add(
                42,
                new MemberPointer(seg, PrimitiveType.Word32, 2));
            tyfo.Write(s, "meeble");
            string sExp = 
@"struct s {
	word32 seg::* ptr002A;	// 2A
} meeble";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoMemberPointerCycle()
        {
            var seg = new StructureType("seg", 100);

            var a = new StructureType("a", 0);
            var b = new StructureType("b", 0);

            a.Fields.Add(0, new MemberPointer(seg, b, 2));
            b.Fields.Add(0, new MemberPointer(seg, a, 2));

            tyfo.WriteTypes(new DataType[] { a, b });

            string sExp =
                "struct b;" + nl +
                "struct a {" + nl +
                "\tstruct b seg::* ptr0000;\t// 0" + nl +
                "};" + nl +
                nl +
                "struct b {" + nl +
                "\tstruct a seg::* ptr0000;\t// 0" + nl +
                "};" + nl + nl;

            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoArray()
        {
            ArrayType arr = new ArrayType(PrimitiveType.Int32, 10);
            tyreffo.WriteDeclaration(arr, "a");

            string sExp = "int32 a[10]";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoArrayArray()
        {
            ArrayType arr = new ArrayType(PrimitiveType.Int32, 10);
            ArrayType arr2 = new ArrayType(arr, 4);
            tyreffo.WriteDeclaration(arr2, "a");

            string sExp = "int32 a[4][10]";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoPtrPtr()
        {
            var ptr = new Pointer(PrimitiveType.Int32, 32);
            var ptr2 = new Pointer(ptr, 32);
            tyreffo.WriteDeclaration(ptr2, "ppi");

            string sExp = "int32 ** ppi";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoStructOfArray()
        {
            var str = new StructureType("str1", 0)
            {
                Fields = {
                    { 0, new ArrayType(PrimitiveType.Char, 10) }
                }
            };
            tyfo.Write(str, "meeble");
            var sExp =
                "struct str1 {" + nl +
                "\tchar a0000[10];\t// 0" + nl +
                "} meeble";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoPtrToTypeReference()
        {
            var typeReference = new TypeReference("testDataType", PrimitiveType.Int32);
            var ptr = new Pointer(typeReference, 32);
            tyfo.Write(ptr, "var");

            string sExp = "testDataType * var";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoClass_Simple()
        {
            var ct = new ClassType();
            ct.Name = "TestClass";
            ct.Fields.Add(new ClassField
            {
                Protection = ClassProtection.Private,
                Offset = 4,
                Name = "m_n0004",
                DataType = PrimitiveType.Int32
            });
            ct.Fields.Add(new ClassField
            {
                Protection = ClassProtection.Private,
                Offset = 8,
                Name = "m_ptr0008",
                DataType = new Pointer(ct, 32),
            });

            ct.Methods.Add(new ClassMethod
            {
                Protection = ClassProtection.Public,
                Attribute = ClassMemberAttribute.Virtual,
                Procedure = new Procedure(arch.Object, "do_something", Address.Ptr32(0x00123400), null),
                Name = "do_something",
            });
            tyfo.Write(ct, null);

            var sExp =
            #region Expected
@"class TestClass {
public:
	do_something();
private:
	int32 m_n0004;	// 4
	TestClass * m_ptr0008;	// 8
}";
            #endregion
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoEnum()
        {
            var e = new EnumType("myEnum");
            e.Members = new SortedList<string, long>
            {
                { "FALSE", 0 },
                { "TRUE", 1 },
                { "FILE_NOT_FOUND", 2 }
            };

            tyfo.Write(e, null);
            var sExp =
@"enum myEnum {
	FALSE = 0x0,
	TRUE = 0x1,
	FILE_NOT_FOUND = 0x2,
}";
            #region
            Assert.AreEqual(sExp, sw.ToString());

            #endregion
        }
    }
}

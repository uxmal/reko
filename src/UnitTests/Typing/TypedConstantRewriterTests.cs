#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Typing;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.UnitTests.Typing
{
    [TestFixture]
	public class TypedConstantRewriterTests
	{
        private TypeStore store;
		private TypeFactory factory;
		private TypedConstantRewriter tcr;
		private Identifier globals;
        private Program program;
        private ByteMemoryArea bmem;

        [SetUp]
		public void Setup()
		{
            bmem = new ByteMemoryArea(Address.Ptr32(0x00100000), new byte[1024]);
            var arch = new FakeArchitecture(new ServiceContainer());
            this.program = new Program
            {
                Architecture = arch,
                SegmentMap = new SegmentMap(
                    bmem.BaseAddress,  
                    new ImageSegment(".text", bmem, AccessMode.ReadWriteExecute)),
                Platform = new DefaultPlatform(null, arch),
            };
            store = program.TypeStore;
            factory = program.TypeFactory;
            globals = program.Globals;
			store.EnsureExpressionTypeVariable(factory, 0, globals);

			StructureType s = new StructureType(null, 0);
			s.Fields.Add(0x00100000, PrimitiveType.Word32, null);

			TypeVariable tvGlobals = store.EnsureExpressionTypeVariable(factory, 0, globals);
			EquivalenceClass eqGlobals = new EquivalenceClass(tvGlobals);
			eqGlobals.DataType = s;
            var globalsPtr = new Pointer(eqGlobals, 32);
            globals.TypeVariable.DataType = globalsPtr;
            globals.TypeVariable.OriginalDataType = globalsPtr;
            globals.DataType = globalsPtr;
		}

        private void Given_TypedConstantRewriter()
        {
            tcr = new TypedConstantRewriter(program, new FakeDecompilerEventListener());
        }

        private void Given_Global(uint address, DataType dt)
        {
            var str = globals.DataType.ResolveAs<Pointer>().Pointee.ResolveAs<StructureType>();
            str.Fields.Add((int)address, dt);
        }

        private void Given_Segment(ushort selector, string name)
        {
            var seg = new ImageSegment(name, new ByteMemoryArea(Address.SegPtr(selector, 0), new byte[100]), AccessMode.ReadWriteExecute);
            seg.Identifier = new Identifier(name, PrimitiveType.SegmentSelector, RegisterStorage.None);
            program.SegmentMap.AddSegment(seg);

            var tv = store.EnsureExpressionTypeVariable(factory, 0, seg.Identifier);
            var dt = new StructureType
            {
                IsSegment = true,
            };
            tv.Class.DataType = dt;
            tv.DataType = new Pointer(tv.Class, 16);
            tv.OriginalDataType = PrimitiveType.SegmentSelector;
        }

        private Constant Given_Constant(int n)
        {
            var c = Constant.Word32(n);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            return c;
        }

        private void Given_DataType(Constant c, DataType dt)
        {
            c.TypeVariable.DataType = dt;
            c.TypeVariable.OriginalDataType = dt;
        }

        private void Given_String_At(string str, uint addr)
        {
            var w = new LeImageWriter(
                bmem.Bytes, addr - (uint) bmem.BaseAddress.ToLinear());
            w.WriteString(str, Encoding.ASCII);
        }

        private void Given_UInt64_At(ulong bits, uint addr)
        {
            var w = new LeImageWriter(
                bmem.Bytes, addr - (uint) bmem.BaseAddress.ToLinear());
            w.WriteLeUInt64(bits);
        }

        private void Given_UInt32_At(uint bits, uint addr)
        {
            var w = new LeImageWriter(
                bmem.Bytes, addr - (uint) bmem.BaseAddress.ToLinear());
            w.WriteLeUInt32(bits);
        }

        private void Given_Readonly_Segment()
        {
            foreach (var seg in program.SegmentMap.Segments.Values)
            {
                seg.Access = AccessMode.Read;
            }
        }

        private void Given_Writeable_Segment()
        {
            foreach (var seg in program.SegmentMap.Segments.Values)
            {
                seg.Access = AccessMode.ReadWrite;
            }
        }

        private Expression RewritePointer(Address addr)
        {
            return tcr.Rewrite(addr, null, false);
        }

        private Expression RewritePointer(Constant c)
        {
            return tcr.Rewrite(c, null, false);
        }

        private Expression RewriteDereferenced(Constant c)
        {
            return tcr.Rewrite(c, null, true);
        }

        [Test]
		public void Tcr_RewriteWord32()
		{
            Given_TypedConstantRewriter();
			Constant c = Constant.Word32(0x0131230);
			store.EnsureExpressionTypeVariable(factory, 0, c);
			c.TypeVariable.DataType = PrimitiveType.Word32;
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			Expression e = RewritePointer(c);
			Assert.AreEqual("0x131230<32>" , e.ToString());
		}

		[Test]
        public void Tcr_RewriterRealBitpattern()
		{
            Given_TypedConstantRewriter();
            Constant c = Constant.Word32(0x3F800000);
			store.EnsureExpressionTypeVariable(factory, 0, c);
			c.TypeVariable.DataType = PrimitiveType.Real32;
			c.TypeVariable.OriginalDataType = c.DataType;
			Expression e = RewritePointer(c);
			Assert.AreEqual("1.0F", e.ToString());
		}

		[Test]
        public void Tcr_RewritePointer()
		{
            Given_TypedConstantRewriter();
            Constant c = Constant.Word32(0x00100000);
			store.EnsureExpressionTypeVariable(factory, 0, c);
			c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 32);
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			Expression e = RewritePointer(c);
			Assert.AreEqual("&g_dw100000", e.ToString());
		}

        [Test]
        public void Tcr_RewriteNullPointer()
        {
            Given_TypedConstantRewriter();
            Constant c = Constant.Word32(0x00000000);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 32);
            c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
            Expression e = RewritePointer(c);
            Assert.AreEqual("00000000", e.ToString());
        }

        [Test]
        public void Tcr_OffImagePointer()
        {
            Given_TypedConstantRewriter();
            Constant c = Constant.Word32(0xFFFFFFFF);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 32);
            c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
            Expression e = RewritePointer(c);
            Assert.AreEqual("(word32 *) 0xFFFFFFFF<32>", e.ToString());
        }

        [Test]
        public void Tcr_RewritePointerToStructField()
        {
            Given_TypedConstantRewriter();
            var str = new StructureType
            {
                Fields =
                {
                    { 0, PrimitiveType.Int32 },
                    { 4, PrimitiveType.Real32 },
                },
            };
            Given_Global(0x00100100, str);
            var c = Constant.Word32(0x00100104);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 32);
            c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
            var e = RewritePointer(c);
            Assert.AreEqual("&g_t100100.r0004", e.ToString());
        }

        [Test]
        public void Tcr_RewriteDereferencedFirstStructField()
        {
            Given_TypedConstantRewriter();
            var str = new StructureType
            {
                Fields =
                {
                    { 0, PrimitiveType.Int32 },
                    { 4, PrimitiveType.Real32 },
                },
            };
            Given_Global(0x00100100, str);
            var c = Constant.Word32(0x00100100);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 32);
            c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
            var e = RewriteDereferenced(c);
            Assert.AreEqual("g_t100100.dw0000", e.ToString());
        }

        [Test(Description ="If we have a char * to read-only memory, treat it as a C string")]
        public void Tcr_ReadOnly_Char_Pointer_YieldsStringConstant()
        {
            //$REVIEW: this is highly platform dependent. Some platforms have 
            // strings terminated by the high bit of the last character set to
            // 1, others have length prefixed strings (looking at you, Turbo Pascal and
            // MacOS classic).
            Given_TypedConstantRewriter();
            Given_String_At("Hello", 0x00100000);
            Given_Readonly_Segment();
            var c = Constant.Word32(0x00100000);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            var charPtr = new Pointer(PrimitiveType.Char, 32);
            c.TypeVariable.DataType = charPtr;
            c.TypeVariable.OriginalDataType = charPtr;
            var e = RewritePointer(c);
            Assert.AreEqual("Hello", e.ToString());
            Assert.AreEqual(
                "(struct (100000 (str char) str100000))",
                ((Pointer)program.Globals.DataType).Pointee.ResolveAs<StructureType>().ToString());
        }

        [Test(Description = "If we have a (array char)* to read-only memory, treat it as a C string")]
        public void Tcr_ReadOnly_ArrayChar_Pointer_YieldsStringConstant()
        {
            //$REVIEW: this is highly platform dependent. Some platforms have 
            // strings terminated by the high bit of the last character set to
            // 1, others have length prefixed strings (looking at you, Turbo Pascal and
            // MacOS classic).
            Given_TypedConstantRewriter();
            Given_String_At("Hello", 0x00100000);
            Given_Readonly_Segment();
            var c = Constant.Word32(0x00100000);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            var arrayCharPtr = new Pointer(new ArrayType(PrimitiveType.Char, 32), 6);
            c.TypeVariable.DataType = arrayCharPtr;
            c.TypeVariable.OriginalDataType = arrayCharPtr;
            var e = RewritePointer(c);
            Assert.AreEqual("Hello", e.ToString());
            Assert.AreEqual(
                "(struct (100000 (str char) str100000))",
                ((Pointer)program.Globals.DataType).Pointee.ResolveAs<StructureType>().ToString());
        }


        [Test]
        public void Tcr_Writable_Char_Pointer_YieldsCharacterReference()
        {
            Given_TypedConstantRewriter();
            Given_String_At("Hello", 0x00100000);
            Given_Writeable_Segment();
            var c = Constant.Word32(0x00100000);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            var charPtr = new Pointer(PrimitiveType.Char, 32);
            c.TypeVariable.DataType = charPtr;
            c.TypeVariable.OriginalDataType = charPtr;
            var e = RewritePointer(c);
            Assert.AreEqual("&g_dw100000", e.ToString());
        }

        [Test(Description="Pointers to the end of arrays are well-defined.")]
        public void Tcr_ArrayEnd()
        {
            Given_TypedConstantRewriter();
            Given_Global(0x00000000, new ArrayType(PrimitiveType.Real32, 16));
            Given_Global(0x00000040, PrimitiveType.Word16);
            var c = Constant.Word32(0x00100040);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Real32, 32);
            c.TypeVariable.OriginalDataType = new Pointer(PrimitiveType.Real32, 32);

            var e = RewritePointer(c);
            Assert.AreEqual("&g_r100040", e.ToString());
        }

        [Test(Description = "Segmented pointers need to be properly handled")]
        public void Tcr_SegPtr()
        {
            Given_Segment(0xC00, "seg0C00");
            Given_TypedConstantRewriter();

            var c = Address.SegPtr(0xC00, 0x0124);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Char, 32);
            c.TypeVariable.OriginalDataType = new Pointer(PrimitiveType.Char, 32);

            var e = RewritePointer(c);
            Assert.AreEqual("&seg0C00->b0124", e.ToString());
        }

        [Test]
        public void Tcr_TypelessPointer()
        {
            Given_Segment(0xC00, "seg0C00");
            Given_TypedConstantRewriter();
            var c = Address.SegPtr(0xC00, 0x200);
            store.EnsureExpressionTypeVariable(factory, 0, c);
            c.TypeVariable.DataType = PrimitiveType.SegPtr32;
            c.TypeVariable.OriginalDataType = PrimitiveType.SegPtr32;

            var e = RewritePointer(c);
            Assert.AreEqual("&seg0C00->t0200", e.ToString());
        }

        [Test(Description = "Don't rewrite pointer to double constant at read-only memory")]
        public void Tcr_Real64_ReadOnly_Pointer()
        {
            Given_TypedConstantRewriter();
            Given_UInt64_At(0x4029800000000000, 0x00100000); // 12.75
            Given_Readonly_Segment();
            var c = Given_Constant(0x00100000);
            Given_DataType(c, new Pointer(PrimitiveType.Real64, 32));

            var e = RewritePointer(c);

            Assert.AreEqual("&g_dw100000", e.ToString());
        }

        [Test(Description = "Rewrite double constant at read-only memory")]
        public void Tcr_Real64_ReadOnly_Dereferenced()
        {
            Given_TypedConstantRewriter();
            Given_UInt64_At(0x4029800000000000, 0x00100000); // 12.75
            Given_Readonly_Segment();
            var c = Given_Constant(0x00100000);
            Given_DataType(c, new Pointer(PrimitiveType.Real64, 32));

            var e = RewriteDereferenced(c);

            Assert.AreEqual("12.75", e.ToString());
        }

        [Test(Description = "Don't rewrite double constant at writeable memory")]
        public void Tcr_Real64_Writeable_Dereferenced()
        {
            Given_TypedConstantRewriter();
            Given_UInt64_At(0x4029800000000000, 0x00100000); // 12.75
            Given_Writeable_Segment();
            var c = Given_Constant(0x00100000);
            Given_DataType(c, new Pointer(PrimitiveType.Real64, 32));

            var e = RewriteDereferenced(c);

            Assert.AreEqual("g_dw100000", e.ToString());
        }

        [Test(Description = "Rewrite float constant at read-only memory")]
        public void Tcr_Real32_ReadOnly_Dereferenced()
        {
            Given_TypedConstantRewriter();
            Given_UInt32_At(0x414C0000, 0x00100000); // 12.75
            Given_Readonly_Segment();
            var c = Given_Constant(0x00100000);
            Given_DataType(c, new Pointer(PrimitiveType.Real32, 32));

            var e = RewriteDereferenced(c);

            Assert.AreEqual("12.75F", e.ToString());
        }
    }
}

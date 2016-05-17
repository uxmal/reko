#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
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
        private MemoryArea mem;

        [SetUp]
		public void Setup()
		{
            mem = new MemoryArea(Address.Ptr32(0x00100000), new byte[1024]);
            var arch = new FakeArchitecture();
            this.program = new Program
            {
                Architecture = arch,
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,  
                    new ImageSegment(".text", mem, AccessMode.ReadWriteExecute)),
                Platform = new DefaultPlatform(null, arch),
            };
            store = program.TypeStore;
            factory = program.TypeFactory;
            globals = program.Globals;
			store.EnsureExpressionTypeVariable(factory, globals);

			StructureType s = new StructureType(null, 0);
			s.Fields.Add(0x00100000, PrimitiveType.Word32, null);

			TypeVariable tvGlobals = store.EnsureExpressionTypeVariable(factory, globals);
			EquivalenceClass eqGlobals = new EquivalenceClass(tvGlobals);
			eqGlobals.DataType = s;
			globals.TypeVariable.DataType = new Pointer(eqGlobals, 4);
			globals.DataType = globals.TypeVariable.DataType;

            tcr = new TypedConstantRewriter(program);
		}

        private void Given_Global(uint address, DataType dt)
        {
            var str = globals.DataType.ResolveAs<Pointer>().Pointee.ResolveAs<StructureType>();
            str.Fields.Add((int)(address - 0x00100000u), dt);
        }

		[Test]
		public void Tcr_RewriteWord32()
		{
			Constant c = Constant.Word32(0x0131230);
			store.EnsureExpressionTypeVariable(factory, c);
			c.TypeVariable.DataType = PrimitiveType.Word32;
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("0x00131230" , e.ToString());
		}

		[Test]
        public void Tcr_RewriterRealBitpattern()
		{
			Constant c = Constant.Word32(0x3F800000);
			store.EnsureExpressionTypeVariable(factory, c);
			c.TypeVariable.DataType = PrimitiveType.Real32;
			c.TypeVariable.OriginalDataType = c.DataType;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("1.0F", e.ToString());
		}

		[Test]
        public void Tcr_RewritePointer()
		{
			Constant c = Constant.Word32(0x00100000);
			store.EnsureExpressionTypeVariable(factory, c);
			c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 4);
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("&globals->dw100000", e.ToString());
		}

        [Test]
        public void Tcr_RewriteNullPointer()
        {
            Constant c = Constant.Word32(0x00000000);
            store.EnsureExpressionTypeVariable(factory, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 4);
            c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
            Expression e = tcr.Rewrite(c, false);
            Assert.AreEqual("00000000", e.ToString());
        }

        [Test]
        public void Tcr_OffImagePointer()
        {
            Constant c = Constant.Word32(0xFFFFFFFF);
            store.EnsureExpressionTypeVariable(factory, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 4);
            c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
            Expression e = tcr.Rewrite(c, false);
            Assert.AreEqual("(word32 *) 0xFFFFFFFF", e.ToString());
        }

        private void Given_String(string str, uint addr)
        {
            var w = new LeImageWriter(mem.Bytes, addr - (uint)mem.BaseAddress.ToLinear());
            w.WriteString(str, Encoding.ASCII);
        }

        private void Given_Readonly_Segment(string segName, uint address, uint size)
        {
            var seg = program.SegmentMap.AddSegment(
                Address.Ptr32(address),
                segName, AccessMode.Read, size);
            seg.Access = AccessMode.Read;
        }

        private void Given_Writeable_Segment(string segName, uint address, uint size)
        {
            var seg = program.SegmentMap.AddSegment(
                Address.Ptr32(address),
                segName, AccessMode.ReadWrite, size);
            seg.Access = AccessMode.ReadWrite;
        }

        [Test(Description ="If we have a char * to read-only memory, treat it as a C string")]
        public void Tcr_ReadOnly_Char_Pointer_YieldsStringConstant()
        {
            //$REVIEW: this is highly platform dependent. Some platforms have 
            // strings terminated by the high bit of the last character set to
            // 1, others have length prefixed strings (looking at you, Turbo Pascal and
            // MacOS classic).
            Given_String("Hello", 0x00100000);
            Given_Readonly_Segment(".rdata", 0x00100000, 0x20);
            var c = Constant.Word32(0x00100000);
            store.EnsureExpressionTypeVariable(factory, c);
            var charPtr = new Pointer(PrimitiveType.Char, 4);
            c.TypeVariable.DataType = charPtr;
            c.TypeVariable.OriginalDataType = charPtr;
            var e = tcr.Rewrite(c, false);
            Assert.AreEqual("Hello", e.ToString());
            Assert.AreEqual(
                "(struct (100000 (str char) str100000))",
                ((Pointer)program.Globals.DataType).Pointee.ResolveAs<StructureType>().ToString());
        }

        [Test]
        public void Tcr_Writable_Char_Pointer_YieldsCharacterReference()
        {
            Given_String("Hello", 0x00100000);
            Given_Writeable_Segment(".rdata", 0x00100000, 0x20);
            var c = Constant.Word32(0x00100000);
            store.EnsureExpressionTypeVariable(factory, c);
            var charPtr = new Pointer(PrimitiveType.Char, 4);
            c.TypeVariable.DataType = charPtr;
            c.TypeVariable.OriginalDataType = charPtr;
            var e = tcr.Rewrite(c, false);
            Assert.AreEqual("&globals->dw100000", e.ToString());
        }

        [Test(Description="Pointers to the end of arrays are well-defined.")]
        public void Tcr_ArrayEnd()
        {
            Given_Global(0x00100000, new ArrayType(PrimitiveType.Real32, 16));
            Given_Global(0x00100040, PrimitiveType.Word16);
            var c = Constant.Word32(0x00100040);
            store.EnsureExpressionTypeVariable(factory, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Real32, 4);
            c.TypeVariable.OriginalDataType = new Pointer(PrimitiveType.Real32, 4);

            var e = tcr.Rewrite(c, false);
            Assert.AreEqual("&globals->r100040", e.ToString());
        }
    }
}

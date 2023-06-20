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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using NUnit.Framework;
using System;
using Reko.UnitTests.Mocks;
using Reko.Core.Memory;

namespace Reko.UnitTests.Decompiler.Typing
{
	[TestFixture]
	public class EquivalenceClassBuilderTests
	{
		private TypeFactory factory;
		private TypeStore store;
		private EquivalenceClassBuilder eqb;
        private FakeDecompilerEventListener listener;

        [SetUp]
		public void Setup()
		{
			factory = new TypeFactory();
			store = new TypeStore();
            listener = new FakeDecompilerEventListener();
			eqb = new EquivalenceClassBuilder(factory, store, listener);
		}

        private TypeVariable TypeVar(Expression e) => store.GetTypeVariable(e);

		[Test]
		public void EqbSimpleEquivalence()
		{
			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store, listener);
			Identifier src = new Identifier("src", PrimitiveType.Word32, null);
			Identifier dst = new Identifier("dst", PrimitiveType.Word32, null);
			Assignment ass = new Assignment(dst, src);
			ass.Accept(eqb);

			Assert.IsNotNull(src);
			Assert.IsNotNull(dst);
			Assert.AreEqual(1, TypeVar(src).Number, "src type number");
			Assert.AreEqual(2, TypeVar(dst).Number, "dst type number");
			Assert.AreEqual(TypeVar(src).Class, TypeVar(dst).Class);
		}

		[Test]
		public void EqbArrayAccess()
		{
            ArrayAccess e = new ArrayAccess(PrimitiveType.Real32, new Identifier("a", PrimitiveType.Ptr32, null), new Identifier("i", PrimitiveType.Int32, null));
			e.Accept(eqb);
			Assert.AreEqual("T_3", TypeVar(e).ToString());
			Assert.AreEqual("T_1", TypeVar(e.Array).ToString());
			Assert.AreEqual("T_2", TypeVar(e.Index).ToString());
		}

		[Test]
		public void EqbSegmentedAccess()
		{
			Identifier ds = new Identifier("ds", PrimitiveType.SegmentSelector, null);
			Identifier bx = new Identifier("bx", PrimitiveType.Word16, null);
			MemoryAccess mps = new MemoryAccess(
                MemoryIdentifier.GlobalMemory,
                SegmentedPointer.Create(ds, bx),
                PrimitiveType.Word32);
			mps.Accept(eqb);
			Assert.AreEqual("T_4", TypeVar(mps).Name);
		}

        [Test]
        public void EqbSegmentConstants()
        {
            Constant seg1 = Constant.Create(PrimitiveType.SegmentSelector, 0x1234);
            Constant seg2 = Constant.Create(PrimitiveType.SegmentSelector, 0x1234);

            seg1.Accept(eqb);
            seg2.Accept(eqb);
            Assert.AreSame(TypeVar(seg1), TypeVar(seg2));
        }

        [Test(Description = "Fixes a regression test that failed when the new type system cut over.")]
        public void EqbProcedureSignature()
        {
            var sig = FunctionType.Action(
                new Identifier("dwArg00", PrimitiveType.Word32, new StackStorage(0, PrimitiveType.Word32)));
            eqb.EnsureSignatureTypeVariables(sig);
            Assert.IsNotNull(store.GetTypeVariable(sig.Parameters[0]));
        }

        [Test(Description = "Expressions referring to type references should map to the same TypeVariable.")]
        public void EqbTypeReference()
        {
            var a = new Identifier("a", new TypeReference("INT", PrimitiveType.Int32), new TemporaryStorage("a", 43, PrimitiveType.Int32));
            var b = new Identifier("b", new TypeReference("INT", PrimitiveType.Int32), new TemporaryStorage("b", 44, PrimitiveType.Int32));
            a.Accept(eqb);
            b.Accept(eqb);
            Assert.AreSame(TypeVar(a).Class, TypeVar(b).Class);
        }

        [Test]
        public void EqSegmentConstantsWithAllocatedSegment()
        {
            var tmp = new TemporaryStorage("seg1234", 0, PrimitiveType.SegmentSelector);
            var segment = new ImageSegment(
                    "seg1234",
                    new ByteMemoryArea(Address.SegPtr(0x1234, 0), new byte[100]),
                    AccessMode.ReadWriteExecute)
            {
                Identifier = new Identifier(tmp.Name, PrimitiveType.SegmentSelector, tmp)
            };
            eqb.EnsureSegmentTypeVariables(new[] { segment });
            Constant seg1 = Constant.Create(PrimitiveType.SegmentSelector, 0x1234);
            seg1.Accept(eqb);
            var tvSeg1 = store.GetTypeVariable(seg1);
            var tvSegment = store.GetTypeVariable(segment.Identifier);
            Assert.AreSame(tvSeg1.Class, tvSegment.Class);
        }
    }
}

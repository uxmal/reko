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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Typing;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class EquivalenceClassBuilderTests
	{
		private TypeFactory factory;
		private TypeStore store;
		private EquivalenceClassBuilder eqb;

		[Test]
		public void SimpleEquivalence()
		{
			TypeFactory factory = new TypeFactory();
			TypeStore store = new TypeStore();
			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store);
			Identifier id1 = new Identifier("id2", 1, PrimitiveType.Word32, null);
			Identifier id2 = new Identifier("id2", 2, PrimitiveType.Word32, null);
			Assignment ass = new Assignment(id1, id2);
			ass.Accept(eqb);

			Assert.IsNotNull(id1);
			Assert.IsNotNull(id2);
			Assert.AreEqual(2, id1.TypeVariable.Number, "id1 type number");
			Assert.AreEqual(2, id1.TypeVariable.Number, "id2 type number");
			Assert.AreEqual(id1.TypeVariable.Class, id2.TypeVariable.Class);
		}

		[Test]
		public void ArrayAccess()
		{
			ArrayAccess e = new ArrayAccess(PrimitiveType.Real32, new Identifier("a", 1, null, null), new Identifier("i", 1, null, null));
			e.Accept(eqb);
			Assert.AreEqual("T_3", e.TypeVariable.ToString());
			Assert.AreEqual("T_1", e.Array.TypeVariable.ToString());
			Assert.AreEqual("T_2", e.Index.TypeVariable.ToString());
		}

		[Test]
		public void SegmentedAccess()
		{
			Identifier ds = new Identifier("ds", 1, PrimitiveType.SegmentSelector, null);
			Identifier bx = new Identifier("bx", 2, PrimitiveType.Word16, null);
			SegmentedAccess mps = new SegmentedAccess(MemoryIdentifier.GlobalMemory, ds, bx, PrimitiveType.Word32);
			mps.Accept(eqb);
			Assert.AreEqual("T_3", mps.TypeVariable.Name);
		}

        [Test]
        public void SegmentConstants()
        {
            Constant seg1 = new Constant(PrimitiveType.SegmentSelector, 0x1234);
            Constant seg2 = new Constant(PrimitiveType.SegmentSelector, 0x1234);

            seg1.Accept(eqb);
            seg2.Accept(eqb);
            Assert.IsNotNull(seg1.TypeVariable);
            Assert.AreSame(seg1.TypeVariable, seg2.TypeVariable);
        }

		[SetUp]
		public void Setup()
		{
			factory = new TypeFactory();
			store = new TypeStore();
			eqb = new EquivalenceClassBuilder(factory, store);
		}
	}
}

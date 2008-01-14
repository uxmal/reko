/* 
 * Copyright (C) 1999-2008 John Källén.
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
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TypedConstantRewriterTests
	{
		private TypeStore store;
		private TypeFactory factory;
		private TypedConstantRewriter tcr;
		private Identifier globals;

		[SetUp]
		public void Setup()
		{
			store = new TypeStore();
			factory = new TypeFactory();
			globals = new Identifier("globals", 0, PrimitiveType.Pointer, null);
			store.EnsureTypeVariable(factory, globals);

			StructureType s = new StructureType(null, 0);
			s.Fields.Add(0x00100000, PrimitiveType.Word32, null);

			TypeVariable tvGlobals = store.EnsureTypeVariable(factory, globals);
			EquivalenceClass eqGlobals = new EquivalenceClass(tvGlobals);
			eqGlobals.DataType = s;
			globals.TypeVariable.DataType = new Pointer(eqGlobals, 4);
			globals.DataType = globals.TypeVariable.DataType;

			tcr = new TypedConstantRewriter(store, globals);
		}

		[Test]
		public void RewriteWord32()
		{
			Constant c = new Constant(PrimitiveType.Word32, 0x0131230);
			store.EnsureTypeVariable(factory, c);
			c.TypeVariable.DataType = PrimitiveType.Word32;
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("0x00131230" , e.ToString());
		}

		[Test]
		public void RewriterRealBitpattern()
		{
			Constant c = new Constant(PrimitiveType.Word32, 0x3F800000);
			store.EnsureTypeVariable(factory, c);
			c.TypeVariable.DataType = PrimitiveType.Real32;
			c.TypeVariable.OriginalDataType = c.DataType;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("1F", e.ToString());
		}

		[Test]
		public void RewritePointer()
		{
			Constant c = new Constant(PrimitiveType.Word32, 0x00100000);
			store.EnsureTypeVariable(factory, c);
			c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 4);
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("&globals->dw100000", e.ToString());

		}
	}
}

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
	public class ComplexExpressionBuilderTests
	{
		private TypeStore store;
		private TypeFactory factory;
		private Pointer ptrPoint;
		private Pointer ptrUnion;

		public ComplexExpressionBuilderTests()
		{
			store = new TypeStore();
			factory = new TypeFactory();
			StructureType point = new StructureType("Point", 0);
			point.Fields.Add(0, PrimitiveType.Word32, null);
			point.Fields.Add(4, PrimitiveType.Word32, null);
			TypeVariable tvPoint = store.EnsureTypeVariable(factory, null);
			EquivalenceClass eq = new EquivalenceClass(tvPoint);
			tvPoint.DataType = eq;
			eq.DataType = point;
			ptrPoint = new Pointer(eq, 4);

			UnionType u = new UnionType("RealInt", null);
			u.Alternatives.Add(new UnionAlternative("w", PrimitiveType.Word32));
			u.Alternatives.Add(new UnionAlternative("r", PrimitiveType.Real32));
			TypeVariable tvUnion = store.EnsureTypeVariable(factory, null);
			eq = new EquivalenceClass(tvUnion);
			tvUnion.DataType = eq;
			eq.DataType = u;
			ptrUnion = new Pointer(eq, 4);

		}

		[Test]
		public void BuildPrimitive()
		{
			Identifier id = new Identifier("id", 3, PrimitiveType.Word32, null);
			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(PrimitiveType.Word32, PrimitiveType.Word32, id, 0);
			Assert.AreEqual("id", ceb.BuildComplex().ToString());
		}

		[Test]
		public void BuildPointer()
		{
			Identifier ptr = new Identifier("ptr", 3, PrimitiveType.Word32, null);
			store.EnsureTypeVariable(factory, ptr);
			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(ptrPoint, new Pointer(PrimitiveType.Word32, 4), ptr, 0);
			Assert.AreEqual("&ptr->dw0000", ceb.BuildComplex().ToString());
		}

		[Test]
		public void BuildPointerFetch()
		{
			Identifier ptr = new Identifier("ptr", 3, PrimitiveType.Word32, null);
			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(ptrPoint, new Pointer(PrimitiveType.Word32, 4), ptr, 0);
			ceb.Dereferenced = true;
			Assert.AreEqual("ptr->dw0000", ceb.BuildComplex().ToString());
		}

		[Test]
		public void BuildUnionFetch()
		{
			Identifier ptr = new Identifier("ptr", 3, PrimitiveType.Word32, null);
			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
				ptrUnion,
				new Pointer(PrimitiveType.Real32, 4),
				ptr, 0);
			ceb.Dereferenced = true;
			Assert.AreEqual("ptr->r", ceb.BuildComplex().ToString());
		}

		[Test]
		public void BuildArrayFetch()
		{
		}
	}
}

/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Typing;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TypedMemoryExpressionRewriterTests
	{
		private TypeStore store;
		private TypeFactory  factory;
		private StructureType point;


		[SetUp]
		public void Setup()
		{
			store = new TypeStore();
			factory = new TypeFactory();
			point = new StructureType(null, 0);
			point.Fields.Add(0, PrimitiveType.Word32, null);
			point.Fields.Add(4, PrimitiveType.Word32, null);

		}
		[Test]
		public void PointerToSingleItem()
		{
			Identifier ptr = new Identifier("ptr", 1, PrimitiveType.Word32, null);
			TypeVariable tv = store.EnsureExpressionTypeVariable(factory, ptr);
			tv.OriginalDataType = new Pointer(point, 4);
			EquivalenceClass eq = new EquivalenceClass(tv);
			eq.DataType = point;
			tv.DataType = new Pointer(eq, 4);

			TypedMemoryExpressionRewriter tmer = new TypedMemoryExpressionRewriter(store, null);
            MemoryAccess access = new MemoryAccess(ptr, PrimitiveType.Word32);
            TypeVariable tvAccess = store.EnsureExpressionTypeVariable(factory, access);
            tvAccess.DataType = PrimitiveType.Word32;
            Expression e = tmer.Rewrite(access);
			Assert.AreEqual("ptr->dw0000", e.ToString());
		}

		[Test]
		public void PointerToSecondItemOfPoint()
		{
			Identifier ptr = new Identifier("ptr", 1, PrimitiveType.Word32, null);
			store.EnsureExpressionTypeVariable(factory, ptr);
			EquivalenceClass eqPtr = new EquivalenceClass(ptr.TypeVariable);
			eqPtr.DataType = point;
			ptr.TypeVariable.OriginalDataType = new Pointer(point, 4);
			ptr.TypeVariable.DataType = new Pointer(eqPtr, 4);

			Constant c = Constant.Word32(4);
			store.EnsureExpressionTypeVariable(factory, c);
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			c.TypeVariable.DataType = PrimitiveType.Word32;
			BinaryExpression bin = new BinaryExpression(BinaryOperator.add, PrimitiveType.Word32, ptr, c);
			store.EnsureExpressionTypeVariable(factory, bin, null);
			bin.TypeVariable.DataType = bin.DataType;
			bin.TypeVariable.OriginalDataType = bin.DataType;

			TypedMemoryExpressionRewriter tmer = new TypedMemoryExpressionRewriter(store, null);
			Expression e = bin.Accept(tmer);
			Assert.AreEqual("ptr->dw0004", e.ToString());
		}
	}
}

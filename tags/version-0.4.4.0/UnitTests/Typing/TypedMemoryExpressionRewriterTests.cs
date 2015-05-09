#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Typing;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using Decompiler.Core;
using Decompiler.UnitTests.Mocks;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TypedMemoryExpressionRewriterTests
	{
        private Program program;
		private TypeStore store;
		private TypeFactory  factory;
		private StructureType point;

		[SetUp]
		public void Setup()
		{
            var image = new LoadedImage(Address.Ptr32(0x00400000), new byte[1024]);
            var arch = new FakeArchitecture();
            program = new Program
            {
                Architecture = arch,
                Image = image,
                ImageMap = image.CreateImageMap(),
                Platform = new DefaultPlatform(null, arch)
            };
            store = program.TypeStore;
            factory = program.TypeFactory;
			point = new StructureType(null, 0);
			point.Fields.Add(0, PrimitiveType.Word32, null);
			point.Fields.Add(4, PrimitiveType.Word32, null);
		}

        private Expression Wrap(Expression e)
        {
            store.EnsureExpressionTypeVariable(factory, e);
            e.TypeVariable.DataType = e.DataType;
            e.TypeVariable.OriginalDataType = e.DataType;
            return e;
        }

		[Test]
		public void Tmer_PointerToSingleItem()
		{
			var ptr = new Identifier("ptr", PrimitiveType.Word32, null);
			var tv = store.EnsureExpressionTypeVariable(factory, ptr);
			tv.OriginalDataType = new Pointer(point, 4);
			var eq = new EquivalenceClass(tv);
			eq.DataType = point;
			tv.DataType = new Pointer(eq, 4);

			TypedExpressionRewriter tmer = new TypedExpressionRewriter(program);
            var access = Wrap(new MemoryAccess(ptr, PrimitiveType.Word32));
            TypeVariable tvAccess = access.TypeVariable;
            tvAccess.DataType = PrimitiveType.Word32;
            Expression e = access.Accept(tmer);
			Assert.AreEqual("ptr->dw0000", e.ToString());
		}

		[Test]
		public void Tmer_PointerToSecondItemOfPoint()
		{
			Identifier ptr = new Identifier("ptr", PrimitiveType.Word32, null);
			store.EnsureExpressionTypeVariable(factory, ptr);
			EquivalenceClass eqPtr = new EquivalenceClass(ptr.TypeVariable);
			eqPtr.DataType = point;
			ptr.TypeVariable.OriginalDataType = new Pointer(point, 4);
			ptr.TypeVariable.DataType = new Pointer(eqPtr, 4);

			var c = Wrap(Constant.Word32(4));
			var bin = Wrap(new BinaryExpression(BinaryOperator.IAdd, PrimitiveType.Word32, ptr, c));
            var mem = Wrap(new MemoryAccess(bin, PrimitiveType.Word32));
			var tmer = new TypedExpressionRewriter(program);
			Expression e = mem.Accept(tmer);
			Assert.AreEqual("ptr->dw0004", e.ToString());
		}
	}
}

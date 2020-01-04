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

using Reko.Typing;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using Reko.Core;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Typing
{
	[TestFixture]
	public class TypedMemoryExpressionRewriterTests
	{
        private Program program;
		private TypeStore store;
		private TypeFactory  factory;
		private StructureType point;
        private ProcedureBuilder m;

		[SetUp]
		public void Setup()
		{
            var mem = new MemoryArea(Address.Ptr32(0x00400000), new byte[1024]);
            var arch = new FakeArchitecture();
            program = new Program
            {
                Architecture = arch,
                SegmentMap = new SegmentMap(
                    mem.BaseAddress, 
                    new ImageSegment(".text", mem, AccessMode.ReadWriteExecute)),
                Platform = new DefaultPlatform(null, arch)
            };
            store = program.TypeStore;
            factory = program.TypeFactory;
			point = new StructureType(null, 0);
			point.Fields.Add(0, PrimitiveType.Word32, null);
			point.Fields.Add(4, PrimitiveType.Word32, null);
            m = new ProcedureBuilder();
		}

        private Expression CreateTv(Expression e)
        {
            store.EnsureExpressionTypeVariable(factory, e);
            e.TypeVariable.DataType = e.DataType;
            e.TypeVariable.OriginalDataType = e.DataType;
            return e;
        }

        private TypeVariable CreateTv(Expression e, DataType dt, DataType dtOrig)
        {
            store.EnsureExpressionTypeVariable(factory, e);
            e.TypeVariable.DataType = dt;
            e.TypeVariable.OriginalDataType = dtOrig;
            return e.TypeVariable;
        }

		[Test]
		public void Tmer_PointerToSingleItem()
		{
            var ptr = new Identifier("ptr", PrimitiveType.Word32, null);
            CreateTv(ptr, new Pointer(point, 32), new Pointer(point, 32));
            var tmer = new TypedExpressionRewriter(program, null);
            var access = CreateTv(m.Mem32(m.IAdd(ptr, 0)));
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
			ptr.TypeVariable.OriginalDataType = new Pointer(point, 32);
			ptr.TypeVariable.DataType = new Pointer(eqPtr, 32);

			var c = CreateTv(Constant.Word32(4));
			var bin = CreateTv(new BinaryExpression(BinaryOperator.IAdd, PrimitiveType.Word32, ptr, c));
            var mem = CreateTv(new MemoryAccess(bin, PrimitiveType.Word32));
			var tmer = new TypedExpressionRewriter(program, null);
			Expression e = mem.Accept(tmer);
			Assert.AreEqual("ptr->dw0004", e.ToString());
		}
	}
}

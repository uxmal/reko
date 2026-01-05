#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Core;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class CriticalInstructionTests
	{
        private Identifier foo = new Identifier("foo", PrimitiveType.Word32, null);

		[Test]
		public void ConstantTest()
		{
			Assert.IsFalse(CriticalInstruction.IsCritical(Constant.Word32(1)));
		}

		[Test]
		public void Crit_Application_Idempotent()
		{
			Assert.IsTrue(CriticalInstruction.IsCritical(new Application(new Identifier("foo", PrimitiveType.Word32, null), PrimitiveType.Bool)));
		}

		[Test]
		public void LoadTest()
		{
			Assert.IsFalse(CriticalInstruction.IsCritical(new MemoryAccess(MemoryStorage.GlobalMemory, Constant.Word32(1), PrimitiveType.Byte)));
		}

		[Test]
		public void StoreTest()
		{
			Assert.IsTrue(CriticalInstruction.IsCritical(new Store(new MemoryAccess(MemoryStorage.GlobalMemory, foo, PrimitiveType.Byte), Constant.Word32(3))));
		}

		[Test]
		public void DereferenceTest()
		{
			Assert.IsTrue(CriticalInstruction.IsCritical(new Reko.Core.Expressions.Dereference(PrimitiveType.Ptr32, Id32("foo"))));
		}

        [Test]
        public void CommentTest()
        {
            Assert.IsTrue(CriticalInstruction.IsCritical(new CodeComment("Comment")));
        }

        [Test]
		public void Crit_BinOpTestTrue()
		{
			Assert.IsTrue(CriticalInstruction.IsCritical(new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, 
				new Application(
                    new ProcedureConstant(PrimitiveType.Ptr32, new IntrinsicProcedure("effect", true, PrimitiveType.Int32, 0)),
                    PrimitiveType.Word32),
				Constant.Word32(1))));
		}

		[Test]
		public void BinOpTestFalse()
		{
			Assert.IsFalse(CriticalInstruction.IsCritical(new BinaryExpression(
                Operator.IAdd, 
                PrimitiveType.Word32, 
                new Identifier("id", PrimitiveType.Word32, null), 
                Constant.Word32(3))));
		}

		[Test]
		public void TestReturn()
		{
			Assert.IsTrue(CriticalInstruction.IsCritical(new ReturnInstruction(null)));
		}

		[Test]
		public void TestBranch()
		{
			Assert.IsTrue(CriticalInstruction.IsCritical(new Branch(null, null)));
		}

		[Test]
		public void TestCallInstruction()
		{
            var someFn = new ExternalProcedure("someFn", new FunctionType());
			Assert.IsTrue(CriticalInstruction.IsCritical(
                new CallInstruction(
                    new ProcedureConstant(PrimitiveType.Ptr32, someFn), 
                    new CallSite(0, 0))));
		}

		[Test]
		public void TestUse()
		{
			Assert.IsTrue(CriticalInstruction.IsCritical(new UseInstruction(null, null)));
		}

		[Test]
		public void TestSideEffect()
		{
			Assert.IsTrue(CriticalInstruction.IsCritical(new SideEffect(null)));
		}

		[Test]
		public void TestAssignFalse()
		{
			Assert.IsFalse(CriticalInstruction.IsCritical(new Assignment(Id32("ax"), Constant.Word32(0))));
		}

        private Identifier Id16(string name)
        {
            return new Identifier(
                name,
                PrimitiveType.Word16,
                new TemporaryStorage(
                    name, 0, PrimitiveType.Word16));
        }

        private Identifier Id32(string name)
        {
            return new Identifier(
                name,
                PrimitiveType.Word32,
                new TemporaryStorage(
                    name, 0, PrimitiveType.Word32));
        }
	}
}

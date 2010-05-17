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

using Decompiler.Analysis;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class CriticalInstructionTests
	{
		private CriticalInstruction ci;
		private Identifier foo = new Identifier("foo", 1, null, null);

		[SetUp]
		public void SetUp()
		{
			ci = new CriticalInstruction();
		}

		[Test]
		public void ConstantTest()
		{
			Assert.IsFalse(ci.IsCritical(Constant.Word32(1)));
		}

		[Test]
		public void ApplicationTest()
		{
			Assert.IsTrue(ci.IsCritical(new Application(new Identifier("foo", 0, null, null), null)));
		}

		[Test]
		public void LoadTest()
		{
			Assert.IsFalse(ci.IsCritical(new MemoryAccess(MemoryIdentifier.GlobalMemory, Constant.Word32(1), PrimitiveType.Byte)));
		}

		[Test]
		public void StoreTest()
		{
			Assert.IsTrue(ci.IsCritical(new Store(new MemoryAccess(MemoryIdentifier.GlobalMemory, foo, PrimitiveType.Byte), Constant.Word32(3))));
		}

		[Test]
		public void DereferenceTest()
		{
			Assert.IsTrue(ci.IsCritical(new Decompiler.Core.Code.Dereference(null, null)));
		}

		[Test]
		public void BinOpTestTrue()
		{
			Assert.IsTrue(ci.IsCritical(new BinaryExpression(null, null, 
				new Application(null, null),
				Constant.Word32(1))));
		}

		[Test]
		public void BinOpTestFalse()
		{
			Assert.IsFalse(ci.IsCritical(new BinaryExpression(null, null, new Identifier(null, 0, null, null), Constant.Word32(3))));
		}

		[Test]
		public void TestReturn()
		{
			Assert.IsTrue(ci.IsCritical(new ReturnInstruction(null)));
		}

		[Test]
		public void TestBranch()
		{
			Assert.IsTrue(ci.IsCritical(new Branch(null, null)));
		}

		[Test]
		public void TestCallInstruction()
		{
			Assert.IsTrue(ci.IsCritical(new CallInstruction(new ProcedureConstant(null, null), new CallSite(0, 0))));
		}

		[Test]
		public void TestIndirectCall()
		{
			Assert.IsTrue(ci.IsCritical(new IndirectCall(null, new CallSite(0, 0))));
		}

		[Test]
		public void TestUse()
		{
			Assert.IsTrue(ci.IsCritical(new UseInstruction(null, null)));
		}

		[Test]
		public void TestSideEffect()
		{
			Assert.IsTrue(ci.IsCritical(new SideEffect(null)));
		}

		[Test]
		public void TestAssignFalse()
		{
			Assert.IsFalse(ci.IsCritical(new Assignment(new Identifier(null, 0, null, null), Constant.Word32(0))));
		}

		[Test]
		public void TestDbp()
		{
			Assert.IsFalse(ci.IsCritical(new DepositBits(new Identifier("eax", 0, null, null), new Identifier("ax", 0, null, null), 0, 16)));
		}
	}

}

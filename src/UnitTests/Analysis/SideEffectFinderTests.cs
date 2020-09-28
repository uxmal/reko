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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class SideEffectFinderTests
	{
		private SideEffectFinder sef;

		[SetUp]
		public void Setup()
		{
			sef = new SideEffectFinder();
		}

		[Test]
		public void SefConflict()
		{
			Assert.IsFalse(sef.Conflict(SideEffectFlags.None, SideEffectFlags.None), "None / None");
			Assert.IsFalse(sef.Conflict(SideEffectFlags.None, SideEffectFlags.Load), "None / Load");
			Assert.IsFalse(sef.Conflict(SideEffectFlags.Load, SideEffectFlags.None), "Load / None");
			Assert.IsFalse(sef.Conflict(SideEffectFlags.Load, SideEffectFlags.Load), "Load / Load");
			Assert.IsTrue(sef.Conflict(SideEffectFlags.Load, SideEffectFlags.Store), "Load / Store");
			Assert.IsTrue(sef.Conflict(SideEffectFlags.Load, SideEffectFlags.Application), "Load / Application");
		}

		[Test]
		public void SefConstant()
		{
			var c = Constant.Byte(3);
			Assert.IsFalse(sef.HasSideEffect(c));
		}

		[Test]
		public void SefApplication()
		{
			var p = new Procedure(new FakeArchitecture(), "foo", Address.Ptr32(0x00123400), null);
			var a = new Application(
				new ProcedureConstant(PrimitiveType.Ptr32, p), 
				PrimitiveType.Word32,
				Constant.Word32(3));
			Assert.AreEqual("foo(0x00000003)", a.ToString());
			Assert.IsTrue(sef.HasSideEffect(a));
		}

		[Test]
		public void SefAreConstrained()
		{
			Procedure proc = new ConstrainedMock().Procedure;
            Statement def = proc.EntryBlock.Succ[0].Statements[0];
            Statement use = proc.EntryBlock.Succ[0].Statements[2];
			Assert.AreEqual("id = bar(3)", def.Instruction.ToString());
			Assert.AreEqual("Mem0[0x10000304:word32] = id", use.Instruction.ToString());
			Assert.AreEqual(SideEffectFlags.Application, sef.FindSideEffect(def.Instruction));
            Assert.AreEqual(SideEffectFlags.Load | SideEffectFlags.Store, sef.FindSideEffect(proc.EntryBlock.Succ[0].Statements[1].Instruction));
			Assert.AreEqual(SideEffectFlags.Load|SideEffectFlags.Store, sef.FindSideEffect(use.Instruction));
			Assert.IsTrue(sef.AreConstrained(def, use));
		}

		[Test]
		public void SefAreUnconstrained()
		{
			Procedure proc = new UnconstrainedMock().Procedure;
			Statement def = proc.EntryBlock.Succ[0].Statements[0];
            Statement use = proc.EntryBlock.Succ[0].Statements[2];

			Assert.AreEqual("id = Mem0[0x01000000:word32]", def.Instruction.ToString());
			Assert.AreEqual("Mem0[0x10000008:word32] = id", use.Instruction.ToString());
			Assert.AreEqual(SideEffectFlags.Load, sef.FindSideEffect(def.Instruction));
            Assert.AreEqual(SideEffectFlags.Load, sef.FindSideEffect(proc.EntryBlock.Succ[0].Statements[1].Instruction));
			Assert.AreEqual(SideEffectFlags.Store|SideEffectFlags.Load, sef.FindSideEffect(use.Instruction));
			Assert.IsFalse(sef.AreConstrained(def, use));
		}

		public class ConstrainedMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				var id = Local32("id");
				Assign(id, this.Fn("bar", Int32(3)));
				MStore(Word32(0x10000300), Int32(0));
				MStore(Word32(0x10000304), id);
			}
		}

		public class UnconstrainedMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				var id = Local32("id");
				var ix = Local32("ix");
				LoadId(id, Word32(0x1000000));
				LoadId(ix, Word32(0x1000004));
				MStore(Word32(0x10000008), id);
			}
		}
	}
}

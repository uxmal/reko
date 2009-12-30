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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class LivenessTests
	{
		private ProcedureMock m;
		private FakeLiveness l;

		[Test]
		public void Assignment()
		{
			Identifier a = m.Local32("a");
			Identifier b = m.Local32("b");
			Statement stm = m.Assign(a, b);

			l.Use(a);
			stm.Instruction.Accept(l);
			Assert.IsTrue(l.IsLive(b), "b should be live");
			Assert.IsFalse(l.IsLive(a), "a shouldn't be live");
		}

		[Test]
		public void FnWithOutparameter()
		{
			Identifier a = m.Local32("a");
			
			l.Use(a);
			Statement stm = m.SideEffect(m.Fn("foo", m.AddrOf(a)));
			stm.Instruction.Accept(l);
			Assert.IsFalse(l.IsLive(a), "a shouldn't be live");
		}

		[Test]
		public void FnWithInOutParameter()
		{
			Identifier a = m.Local32("a");
			
			l.Use(a);
			Statement stm = m.SideEffect(m.Fn("foo", a, m.AddrOf(a)));
			stm.Instruction.Accept(l);
			Assert.IsTrue(l.IsLive(a), "a should be live");
		}

		[SetUp]
		public void Setup()
		{
			m = new ProcedureMock();
			l = new FakeLiveness();
		}
	}

	public class FakeLiveness : Liveness
	{
		private HashedSet<Identifier> h = new HashedSet<Identifier>();

		public override void Def(Identifier id)
		{
			h.Remove(id);
		}

		public bool IsLive(Identifier id)
		{
			return h.Contains(id);
		}

		public override void Use(Identifier id)
		{
            h.Add(id);
		}

	}
}

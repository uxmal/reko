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
using Decompiler.Core.Lib;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ProcedureTests
	{
		public ProcedureTests()
		{
		}

		[Test]
		public void CreateProcedure()
		{
			Procedure proc = Procedure.Create(new Address(0xBAFE, 0x0123), null);
			Assert.IsTrue(proc.EntryBlock != null);
			Assert.IsTrue(proc.ExitBlock != null);
		}

		[Test]
		public void ProcToString()
		{
			Procedure proc1 = Procedure.Create(new Address(0x0F00, 0x0BA9), null);
			Assert.AreEqual("fn0F00_0BA9", proc1.Name);
			Assert.AreEqual(proc1.Name, proc1.ToString());
			Procedure proc2 = Procedure.Create(new Address(0x0F000BA9), null);
			Assert.AreEqual("fn0F000BA9", proc2.Name);
			Assert.AreEqual(proc2.Name, proc2.ToString());
			Procedure proc3 = new Procedure("foo", null);
			Assert.AreEqual("foo", proc3.Name);
			Assert.AreEqual(proc3.Name, proc3.ToString());
		}


		[Test]
		public void ProcCharacteristicIsAlloca()
		{
			Procedure proc = new Procedure("foo", null);
			Assert.IsFalse(proc.Characteristics.IsAlloca);
		}

	}
}

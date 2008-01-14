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

using System;
using Decompiler.Analysis;
using Decompiler.Analysis.Simplification;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;

namespace Decompiler.UnitTests.Analysis.Simplification
{
	[TestFixture]
	public class Add_id_c_id_RuleTest
	{
		/// <summary>
		/// tests (+ (* id c) id)
		/// </summary>
		[Test]
		public void Test1()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier id = m.Local32("id");
			Identifier x = m.Local32("x");
			Statement stm = m.Assign(x, m.Add(m.Muls(id, 4), id));
		}
	}
}

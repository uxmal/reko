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

using System;
using Reko.Analysis;
using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Operators;
using Reko.UnitTests.Mocks;
using NUnit.Framework;

namespace Reko.UnitTests.Evaluation
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
			var m = new ProcedureBuilder();
			var id = m.Local32("id");
			var x = m.Local32("x");
			var stm = m.Assign(x, m.IAdd(m.SMul(id, 4), id));
		}
	}
}

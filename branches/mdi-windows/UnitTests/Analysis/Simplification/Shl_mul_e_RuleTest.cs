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
using Decompiler.Analysis.Simplification;
using Decompiler.Core; 
using Decompiler.Core.Code; 
using Decompiler.UnitTests.Mocks;
using System;
using NUnit.Framework;

namespace Decompiler.UnitTests.Analysis.Simplification
{
	[TestFixture]
	public class Shl_mul_e_RuleTest
	{
		private Identifier id;
		private ProcedureMock m;
		private Identifier x;
		private SsaIdentifierCollection ssaIds;

		/// <summary>
		/// (shl (* e c1) c2) => (* e (c1 shl c2))
		/// </summary>
		[Test]
		public void Test1()
		{
			BinaryExpression b = m.Shl(m.Muls(id, 3), 2);
			Assignment ass = new Assignment(x, b);
			Statement stm = new Statement(ass, null);
			ssaIds[id].Uses.Add(stm);
			ssaIds[id].Uses.Add(stm);

			Shl_mul_e_Rule  rule = new Shl_mul_e_Rule(ssaIds);
			Assert.IsTrue(rule.Match(b));
			ass.Src = rule.Transform(stm);
			Assert.AreEqual("x = id *s 12", ass.ToString());
		}
			
		
		[SetUp]
		public void SetUp()
		{
			m = new ProcedureMock();
			id = m.Local32("id");
			x = m.Local32("x");
			ssaIds = new SsaIdentifierCollection();
			foreach (Identifier i in m.Procedure.Frame.Identifiers)
			{
				ssaIds.Add(i, null, null, false);
			}
		}
	}
}

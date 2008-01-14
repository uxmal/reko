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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class ExpressionSimplifierTests
	{
		private Hashtable table;
		private ExpressionSimplifier simplifier;
		private Identifier foo;
		private Identifier bar;

		[Test]
		public void ExsConstants()
		{
			BuildExpressionSimplifier();
			Expression expr = new BinaryExpression(Operator.add, PrimitiveType.Word32, 
				Constant.Word32(1), Constant.Word32(2));
			Constant c = (Constant) simplifier.Simplify(expr);

			Assert.AreEqual(3, Convert.ToInt32(c.Value));
		}

		private void BuildExpressionSimplifier()
		{
			SsaIdentifierCollection ssaIds = BuildSsaIdentifiers();
			table = new Hashtable();
			simplifier = new ExpressionSimplifier(new ValueNumbering(ssaIds), table);
		}

		private SsaIdentifierCollection BuildSsaIdentifiers()
		{
			MachineRegister mrFoo = new MachineRegister("foo", 1, PrimitiveType.Word32);
			MachineRegister mrBar = new MachineRegister("bar", 2, PrimitiveType.Word32);
			foo = new Identifier(mrFoo.Name, 1, mrFoo.DataType, new RegisterStorage(mrFoo));
			bar = new Identifier(mrBar.Name, 2, mrBar.DataType, new RegisterStorage(mrBar));

			SsaIdentifierCollection coll = new SsaIdentifierCollection();
			
			coll.Add(foo, new Statement(new Assignment(foo, Constant.Word32(1)), null));
			return coll;
		}
	}
}
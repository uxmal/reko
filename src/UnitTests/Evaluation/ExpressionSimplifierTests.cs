#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Evaluation
{
	[TestFixture]
	public class ExpressionSimplifierTests
	{
		private Dictionary<Expression, Expression> table;
		private ExpressionSimplifier simplifier;
		private Identifier foo;
		private Identifier bar;

		[Test]
		public void ExsConstants()
		{
			BuildExpressionSimplifier();
			Expression expr = new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, 
				Constant.Word32(1), Constant.Word32(2));
			Constant c = (Constant) expr.Accept(simplifier);

			Assert.AreEqual(3, c.ToInt32());
		}

        [Test]
        public void Exs_OrWithSelf()
        {
            BuildExpressionSimplifier();
            var expr = new BinaryExpression(Operator.Or, foo.DataType, foo, foo);
            var result = expr.Accept(simplifier);
            Assert.AreSame(foo, result);
        }

        [Test]
        public void ExsAddPositiveConstantToNegative()
        {
            BuildExpressionSimplifier();
            var expr = new BinaryExpression(
                Operator.IAdd,
                foo.DataType,
                new BinaryExpression(
                    Operator.ISub,
                    foo.DataType,
                    foo,
                    Constant.Word32(4)),
                Constant.Word32(1));
            var result = expr.Accept(simplifier);
            Assert.AreEqual("foo_0 - 0x00000003", result.ToString());
        }

		private void BuildExpressionSimplifier()
		{
			SsaIdentifierCollection ssaIds = BuildSsaIdentifiers();
			table = new Dictionary<Expression,Expression>();
            simplifier = new ExpressionSimplifier(new SsaEvaluationContext(null, ssaIds));
		}

		private SsaIdentifierCollection BuildSsaIdentifiers()
		{
			var mrFoo = new RegisterStorage("foo", 1, 0, PrimitiveType.Word32);
			var mrBar = new RegisterStorage("bar", 2, 1, PrimitiveType.Word32);
			foo = new Identifier(mrFoo.Name, mrFoo.DataType, mrFoo);
			bar = new Identifier(mrBar.Name, mrBar.DataType, mrBar);

			var coll = new SsaIdentifierCollection();
            var src = Constant.Word32(1);
            foo = coll.Add(foo, new Statement(0, new Assignment(foo, src), null), src, false).Identifier;
			return coll;
		}
	}
}
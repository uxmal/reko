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

using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models the common occurrence of CPU condition codes. The return
    /// value is the condition codes that were set when the Expression 
    /// was evaluated.
    /// </summary>
    /// <remarks>
    /// For instance, the x86 instruction 
    /// <code>
    ///     add eax,ebx
    /// </code>
    /// should be translated to:
    /// <code>
    ///     eax = eax + ebx
    ///     SZCO = COND(eax)
    /// </code>
    /// which models the fact that the SZCO condition codes are set by the
    /// ADD instruction that produced eax.
    /// <para>
    /// ConditionOf instances only exist early in the decompilation process,
    /// and are replaced with boolean conditions as the decompiler starts 
    /// building expressions. Any surviving ConditionOf instances at the end
    /// of the decompilation are considered bugs.</para>
    /// </remarks>
	public class ConditionOf : Expression
	{
		public ConditionOf(Expression ex) : base(PrimitiveType.Byte)
		{
			Expression = ex;
		}

        public Expression Expression { get; set; }

        public override IEnumerable<Expression> Children
        {
            get { yield return Expression; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitConditionOf(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitConditionOf(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitConditionOf(this);
		}

		public override Expression CloneExpression()
		{
			return new ConditionOf(Expression.CloneExpression());
		}
	}
}

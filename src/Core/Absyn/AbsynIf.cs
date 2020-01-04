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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Reko.Core.Absyn
{
	/// <summary>
	/// Abstract syntax for a traditional 'if' statement.
	/// </summary>
	public class AbsynIf : AbsynStatement
	{
        private Expression condition;
        private List<AbsynStatement> then;
        private List<AbsynStatement> @else;

        public Expression Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        public List<AbsynStatement> Then
        {
            get { return then; }
        }

        public List<AbsynStatement> Else
        {
            get { return @else; }
        }

        public AbsynIf()
            : this(null, new List<AbsynStatement>(), new List<AbsynStatement>())
        {
        }

        public AbsynIf(Expression e, List<AbsynStatement> then) : this(e, then, new List<AbsynStatement>())
		{
		}

		public AbsynIf(Expression e, List<AbsynStatement> then, List<AbsynStatement> els)
		{
			this.Condition = e;
			this.then = then;
			this.@else = els;
		}

		public override void Accept(IAbsynVisitor v)
		{
			v.VisitIf(this);
		}

        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitIf(this); 
        }

        public void InvertCondition()
        {
            List<AbsynStatement> t = then;
            then = @else;
            @else = t;
            Condition = Condition.Invert();
        }
    }
}

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

using Decompiler.Core.Code;
using System;

namespace Decompiler.Core.Absyn
{
	/// <summary>
	/// Abstract syntax for a traditional 'if' statement.
	/// </summary>
	public class AbsynIf : AbsynStatement
	{
        private Expression condition;
        private AbsynStatementList then;
        private AbsynStatementList @else;

        public Expression Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        public AbsynStatementList Then
        {
            get { return then; }
        }

        public AbsynStatementList Else
        {
            get { return @else; }
        }

        public AbsynIf(Expression e, AbsynStatementList then) : this(e, then, null)
		{
		}

		public AbsynIf(Expression e, AbsynStatementList then, AbsynStatementList els)
		{
			this.Condition = e;
			this.then = then;
			this.@else = els;
		}

		public override void Accept(IAbsynVisitor v)
		{
			v.VisitIf(this);
		}

        public void InvertCondition()
        {
            AbsynStatementList t = then;
            then = @else;
            @else = t;
            Condition = Condition.Invert();
        }
    }
}

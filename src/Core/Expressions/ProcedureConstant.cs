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
	public class ProcedureConstant : Expression
	{
		public ProcedureConstant(DataType ptrType, ProcedureBase proc) : base(ptrType)
		{
            this.Procedure = proc;
		}

        public ProcedureBase Procedure { get; private set; }

        public override IEnumerable<Expression> Children
        {
            get { yield break; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitProcedureConstant(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitProcedureConstant(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitProcedureConstant(this);
		}

		public override Expression CloneExpression()
		{
            return this;
		}
	}
}

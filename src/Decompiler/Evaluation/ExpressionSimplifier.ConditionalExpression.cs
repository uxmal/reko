#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitConditionalExpression(ConditionalExpression c)
        {
            var (cond, cChanged) = c.Condition.Accept(this);
            var (t, tChanged) = c.ThenExp.Accept(this);
            var (f, fChanged) = c.FalseExp.Accept(this);
            if (cond is Constant cCond && cCond.DataType == PrimitiveType.Bool)
            {
                //$TODO: side effects
                if (cCond.IsZero)
                    return (f, true);
                else
                    return (t, true);
            }
            bool changed = (cChanged | tChanged | fChanged);
            if (changed)
                c = new ConditionalExpression(c.DataType, cond, t, f);
            return (c, changed);
        }
    }
}

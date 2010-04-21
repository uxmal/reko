/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Code
{
    /// <summary>
    /// Represents testing the expression to see if the condition code is true, and generating a
    /// boolean value.
    /// </summary>
    /// <remarks>
    /// This is a very low-level expression modelling the condition codes of certain processor
    /// architectures, and should be rewritten by the later decompiler stages to a boolean expression.
    /// </remarks>
    public class TestCondition : Expression
    {
        private ConditionCode cc;
        private Expression expr;

        public TestCondition(ConditionCode cc, Expression expr)
            : base(PrimitiveType.Bool)
        {
            this.cc = cc;
            this.expr = expr;
        }

        public override Expression Accept(IExpressionTransformer xform)
        {
            return xform.TransformTestCondition(this);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitTestCondition(this);
        }

        public override Expression CloneExpression()
        {
            throw new NotImplementedException();
        }

        public ConditionCode ConditionCode
        {
            get { return cc; }
            set { cc = value; }
        }

        public Expression Expression
        {
            get { return expr; }
            set { expr = value; }
        }

        public override Expression Invert()
        {
            ConditionCode cc;
            switch (this.ConditionCode)
            {
            case ConditionCode.EQ: cc = ConditionCode.NE; break;
            case ConditionCode.NE: cc = ConditionCode.EQ; break;
            default: throw new NotImplementedException("Invert of Test(" + ConditionCode + ") not implemented");
            }
            return new TestCondition(cc, Expression);
        }


    }
}

#region License
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
#endregion

using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Core.Output;
using System;
using System.IO;

namespace Decompiler.Core.Expressions
{
    /// <summary>
    /// Base class for all decompiled expressions.
    /// </summary>
    public abstract class Expression
    {
        private TypeVariable typeVariable;			// index to high-level type of this expression. //$REVIEW: only used for typing, perhaps move it to hashtable?

        public Expression(DataType dataType)
        {
            this.DataType = dataType;
        }

        // Data type of this expression.
        public DataType DataType { get; set; }
        public TypeVariable TypeVariable { get; set; }

        public abstract Expression Accept(IExpressionTransformer xform);
        public abstract void Accept(IExpressionVisitor visit);
        public abstract T Accept<T>(ExpressionVisitor<T> visitor);
        public abstract Expression CloneExpression();
        
        public virtual Expression Invert()
        {
            throw new NotSupportedException(string.Format("Expression of type {0} doesn't support Invert.", GetType().Name));
        }

        public override string ToString()
        {
            var sw = new StringWriter();
            var fmt = new CodeFormatter(new Formatter(sw));
            fmt.WriteExpression(this);
            return sw.ToString();
        }

        public static string OperatorToString(Operator op)
        {
            return op.ToString();
        }
    }
}

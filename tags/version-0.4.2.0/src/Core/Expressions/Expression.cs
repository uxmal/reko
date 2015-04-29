#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
        public Expression(DataType dataType)
        {
            this.DataType = dataType;
        }

        /// <summary>
        /// Data type of this expression.
        /// </summary>
        public DataType DataType { get { return dt; } set { if (value == null) new ArgumentNullException(); dt = value; } } 
        private DataType dt;

        /// <summary>
        /// Type variable for the expression.
        /// </summary>
        /// <remarks>
        ///$REVIEW: TypeVariable is only used during type inference. It might be better to store it in
        /// the TypeStore.
        public TypeVariable TypeVariable { get; set; } 		// index to high-level type of this expression.

        /// <summary>
        /// Returns true if the expression evaluates to a constant zero.
        /// </summary>
        public virtual bool IsZero { get { return false; } }

        // Visitor methods that must be implemented by concrete derived classes.
        public abstract void Accept(IExpressionVisitor visitor);
        public abstract T Accept<T>(ExpressionVisitor<T> visitor);
        public abstract T Accept<T,C>(ExpressionVisitor<T, C> visitor, C context);
        public abstract Expression CloneExpression();
        
        public bool As<T>(out T value) where T : Expression
        {
            value = this as T;
            return value != null;
        }

        /// <summary>
        /// Applies logical (not-bitwise) negation to the expression.
        /// </summary>
        /// <returns></returns>
        public virtual Expression Invert()
        {
            throw new NotSupportedException(string.Format("Expression of type {0} doesn't support Invert.", GetType().Name));
        }

        public override string ToString()
        {
            var sw = new StringWriter();
            var fmt = new CodeFormatter(new TextFormatter(sw));
            fmt.WriteExpression(this);
            return sw.ToString();
        }
    }
}

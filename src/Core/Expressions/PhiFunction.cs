#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Expressions
{
	/// <summary>
	/// Representation of an SSA phi-function.
	/// </summary>
	public class PhiFunction : AbstractExpression
	{
        /// <summary>
        /// Constructs a phi function.
        /// </summary>
        /// <param name="joinType">The datatype of the phi function.</param>
        /// <param name="arguments">The arguments of the phi function.</param>
		public PhiFunction(DataType joinType, params PhiArgument[] arguments) : base(joinType)
		{
			this.Arguments = arguments;
		}

        /// <summary>
        /// The arguments of the phi function.
        /// </summary>
        public PhiArgument[] Arguments { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { return Arguments.Select(de => de.Value); }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitPhiFunction(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitPhiFunction(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor v)
		{
			v.VisitPhiFunction(this);
		}

        /// <inheritdoc/>
        public override Expression CloneExpression()
        {
            var args = Arguments
                .Select(a => new PhiArgument(a.Block, a.Value.CloneExpression()))
                .ToArray();
            return new PhiFunction(DataType, args);
        }
	}

    /// <summary>
    /// The phi function arguments need to keep track of both the SSA identifier 
    /// and the predecessor block from which the identifier came.
    /// </summary>
    public readonly struct PhiArgument
    {
        /// <summary>
        /// Constructs a phi function argument.
        /// </summary>
        /// <param name="block">The <see cref="Block"/> from which the value comes.</param>
        /// <param name="value">The value.</param>
        public PhiArgument(Block block, Expression value)
        {
            this.Value = value;
            this.Block = block;
        }

        /// <summary>
        /// Deconstructs the <see cref="PhiArgument"/> into its constituent parts.
        /// </summary>
        /// <param name="block">The <see cref="Block">basic block</see> from which
        /// this argument came.</param>
        /// <param name="value">The value of this argument.</param>
        public void Deconstruct(out Block block, out Expression value)
        {
            block = this.Block;
            value = this.Value;
        }

        /// <summary>
        /// The <see cref="Block">basic block</see> from which
        /// this argument came.
        /// </summary>
        public Block Block { get; }

        /// <summary>
        /// The value of this argument.
        /// </summary>
        public Expression Value { get; }

    }
}
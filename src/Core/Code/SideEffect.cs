#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Core.Code
{
	/// <summary>
	/// Represents an expression used only for its side effects. Typically, the expression is
	/// a function application which performs I/O or invokes system services.
	/// </summary>
	public class SideEffect : Instruction
	{
        /// <summary>
        /// Creates an instance of the <see cref="SideEffect"/> class.
        /// </summary>
        /// <param name="expr">Expression which is evaluated for its side effect.</param>
		public SideEffect(Expression expr)
		{
			this.Expression = expr;
		}

        /// <summary>
        /// Expression evaluated for its side effect.
        /// </summary>
		public Expression Expression { get; set; }

        /// <inheritdoc/>
		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformSideEffect(this);
		}

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitSideEffect(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitSideEffect(this, ctx);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
		{
			v.VisitSideEffect(this);
		}

        /// <inheritdoc/>
		public override bool IsControlFlow => false;
	}
}

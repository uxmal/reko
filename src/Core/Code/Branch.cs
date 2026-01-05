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

namespace Reko.Core.Code
{
	/// <summary>
	/// Models a typical "if"-statement.
	/// </summary>
	public class Branch : Instruction
	{
        /// <summary>
        /// Construct a <see cref="Branch"/> instance.
        /// </summary>
        /// <param name="cond">The branch predicate.</param>
        /// <param name="target">Destination block if predicate is true.</param>
        public Branch(Expression cond, Block target)
        {
            this.Condition = cond;
            this.Target = target;
        }

        /// <summary>
        /// The condition of the branch.
        /// </summary>
        public Expression Condition { get; set; }

        /// <inheritdoc/>
        public override bool IsControlFlow => true;

        /// <summary>
        /// The target of the branch (if <see cref="Condition"/> evalutes to true).
        /// </summary>
        public Block Target { get; set; }

        /// <inheritdoc/>
		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformBranch(this);
		}

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitBranch(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitBranch(this, ctx);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
		{
			v.VisitBranch(this);
		}
    }
}
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
    /// This instruction models unconditional jumps.
    /// </summary>
    public class GotoInstruction : Instruction
    {
        /// <summary>
        /// Use this constructor to create an unconditional transfer instruction.
        /// </summary>
        /// <param name="target">The destination, either as a linear address
        /// or as an expression.
        /// </param>
        public GotoInstruction(Expression target)
        {
            this.Target = target;
            this.Condition = null;
        }

        /// <inheritdoc/>
        public override Instruction Accept(InstructionTransformer xform)
        {
            return xform.TransformGotoInstruction(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitGotoInstruction(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitGotoInstruction(this, ctx);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
        {
            v.VisitGotoInstruction(this);
        }

        /// <inheritdoc/>
        public override bool IsControlFlow => true;

        /// <inheritdoc/>
        public Expression? Condition { get; set; }

        /// <summary>
        /// The target of the goto instruction. Either a Constant, in which case it should 
        /// be an address, or 
        /// </summary>
        public Expression Target { get; set; }
    }
}

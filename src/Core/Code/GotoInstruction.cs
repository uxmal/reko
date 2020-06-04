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

using Reko.Core.Expressions;
using System;

namespace Reko.Core.Code
{
    /// <summary>
    /// This instruction models unconditional jumps.
    /// </summary>
    public class GotoInstruction : Instruction
    {
        private Expression condition;

        /// <summary>
        /// Use this constructor to create an unconditional transfer instruction.
        /// </summary>
        /// <param name="target">The destination, either as a linear address or as an expression.</param>
        public GotoInstruction(Expression target)
        {
            this.Target = target;
            this.condition = Constant.Invalid;
        }

        public override Instruction Accept(InstructionTransformer xform)
        {
            return xform.TransformGotoInstruction(this);
        }

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitGotoInstruction(this);
        }

        public override void Accept(InstructionVisitor v)
        {
            v.VisitGotoInstruction(this);
        }

        public bool IsConditional { get { return Condition != Constant.Invalid; } }

        public override bool IsControlFlow { get { return true; } }

        public Expression Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        /// <summary>
        /// The target of the goto instruction. Either a Constant, in which case it should 
        /// be an address, or 
        /// </summary>
        public Expression Target { get; set; }
    }
}

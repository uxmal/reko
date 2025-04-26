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

using System;
using Reko.Core.Expressions;

namespace Reko.Core.Code
{
    /// <summary>
	/// An Assignment copies data from <see cref="Src" /> to <see cref="Dst" />.
    /// </summary>
    public class Assignment : Instruction
	{
        /// <summary>
        /// Constructs an assignment instruction.
        /// </summary>
        /// <param name="dst">The destination of the assignment.</param>
        /// <param name="src">The source of the assignment.</param>
        public Assignment(Identifier dst, Expression src)
        {
            this.Dst = dst ?? throw new ArgumentNullException(nameof(dst));
            this.Src = src ?? throw new ArgumentNullException(nameof(src));
        }

        /// <summary>
        /// The destination of the assignment. This is an identifier.
        /// </summary>
        public Identifier Dst { get; set; }

        /// <summary>
        /// The source of the assignment.
        /// </summary>
        public Expression Src { get; set; }

        /// <inheritdoc/>
        public override bool IsControlFlow => false;
        
        /// <inheritdoc/>
        public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformAssignment(this);
		}

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitAssignment(this, ctx);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
		{
			v.VisitAssignment(this);
		}
	}

	/// <summary>
	/// A Store copies data from Src to the memory referred to by the expression in Dst.
	/// </summary>
	public class Store : Instruction
	{
        /// <summary>
        /// Constructs a store instruction.
        /// </summary>
        /// <param name="dst">The destination of the store instruction.</param>
        /// <param name="src">The source of the store instruction.</param>
        /// <exception cref="ArgumentNullException"></exception>
		public Store(Expression dst, Expression src)
		{
            Dst = dst ?? throw new ArgumentNullException(nameof(dst));
			Src = src ?? throw new ArgumentNullException(nameof(src));
        }

        /// <summary>
        /// The destination of the store instruction.
        /// </summary>
        public Expression Dst { get; set; }

        /// <summary>
        /// The source of the store instruction.
        /// </summary>
        public Expression Src { get; set; }

        /// <inheritdoc/>
        public override bool IsControlFlow => false;
        
        /// <inheritdoc/>
        public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformStore(this);
		}

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitStore(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitStore(this, ctx);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
		{
			v.VisitStore(this);
		}
    }

    /// <summary>
    /// Represents an assignment due to alias expansion. It exists to denote
    /// assignments that don't correspond to actual code present in the original
    /// binary.
    /// </summary>
    public class AliasAssignment : Assignment
    {
        /// <summary>
        /// Constructs an instance of <see cref="AliasAssignment"/>.
        /// </summary>
        /// <param name="idDst">Destination of the assignment.</param>
        /// <param name="expSrc">Source of the assignment.</param>
        public AliasAssignment(Identifier idDst, Expression expSrc) : base(idDst, expSrc)
        {
        }
    }
}

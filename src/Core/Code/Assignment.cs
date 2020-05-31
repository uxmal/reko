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

using System;
using Reko.Core.Expressions;

namespace Reko.Core.Code
{
	/// <summary>
	/// An Assignment copies data from <paramref name="Src" /> to <paramref name="Dst" />.
	/// </summary>
	public class Assignment : Instruction
	{
        public Assignment(Identifier dst, Expression src)
        {
            this.Dst = dst ?? throw new ArgumentNullException(nameof(dst));
            this.Src = src ?? throw new ArgumentNullException(nameof(src));
        }

        public Identifier Dst { get; set; }
        public Expression Src { get; set; }
        public virtual bool IsAlias { get { return false; } }
        public override bool IsControlFlow { get { return false; } }
        
        public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformAssignment(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }

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
		public Store(Expression dst, Expression src)
		{
            Dst = dst ?? throw new ArgumentNullException(nameof(dst));
			Src = src ?? throw new ArgumentNullException(nameof(src));
        }

        public Expression Dst { get; set; }

        public Expression Src { get; set; }

        public override bool IsControlFlow { get { return false; } }
        
        public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformStore(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitStore(this);
        }

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
        public AliasAssignment(Identifier idDst, Expression expSrc) : base(idDst, expSrc)
        {
        }

        public override bool IsAlias
        {
            get { return true; }
        }
    }
}

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
	/// Represents an expression used only for its side effects. Typically, the expression is
	/// a function application which performs I/O or invokes system services.
	/// </summary>
	public class SideEffect : Instruction
	{
		public SideEffect(Expression expr)
		{
			this.Expression = expr;
		}

		public Expression Expression { get; set; }

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformSideEffect(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitSideEffect(this);
        }

		public override void Accept(InstructionVisitor v)
		{
			v.VisitSideEffect(this);
		}

		public override bool IsControlFlow { get { return false; } }
	}
}

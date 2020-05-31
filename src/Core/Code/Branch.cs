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
using System.IO;

namespace Reko.Core.Code
{
	/// <summary>
	/// Models a typical "if"-statement.
	/// </summary>
	public class Branch : Instruction
	{
        public Branch(Expression cond, Block target)
        {
            this.Condition = cond;
            this.Target = target;
        }

        public Expression Condition { get; set; }
        public override bool IsControlFlow { get { return true; } }

        public Block Target { get; set; }

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformBranch(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitBranch(this);
        }

		public override void Accept(InstructionVisitor v)
		{
			v.VisitBranch(this);
		}
    }
}
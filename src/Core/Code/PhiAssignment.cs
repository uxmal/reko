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
using System.IO;

namespace Reko.Core.Code
{
	/// <summary>
	/// Represents an assignment of an identifier from an SSA phi expression.
	/// </summary>
	public class PhiAssignment : Instruction
	{
		public Identifier Dst;
		public PhiFunction Src;

		public PhiAssignment(Identifier d, params PhiArgument[] args)
		{
			Dst = d;
            var phi = new PhiFunction(d.DataType, args);
			Src = phi;
		}

		public PhiAssignment(Identifier d, PhiFunction p)
		{
			Dst = d;
			Src = p;
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformPhiAssignment(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitPhiAssignment(this);
        }

        public override void Accept(InstructionVisitor v)
		{
			v.VisitPhiAssignment(this);
		}

		public override bool IsControlFlow
		{
			get { return false; }
		}
	}
}

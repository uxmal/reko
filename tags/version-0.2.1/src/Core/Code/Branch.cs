/* 
 * Copyright (C) 1999-2010 John Källén.
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

using System;
using System.IO;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Represents a typical "if"-statement.
	/// </summary>
	public class Branch : Instruction
	{
		private Expression cond;
        private Block target;

        public Branch(Expression cond, Block target)
        {
            this.cond = cond;
            this.target = target;
        }

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformBranch(this);
		}

		public override void Accept(InstructionVisitor v)
		{
			v.VisitBranch(this);
		}

        public Block Target
        {
            get { return target; }
        }

		public override bool IsControlFlow
		{
			get { return true; }
		}

		public Expression Condition
		{
			get { return cond; }
			set { cond = value; }
		}
	}

	public enum ConditionCode
	{
		None,
		UGT,	// Unsigned >
		ULE,	// Unsigned <=
		ULT,	// Unsigned <
		GT,		// >
		GE,		// >=
		LT,		// <
		LE,		// <=
		UGE,	// Unsigned >=
		NO,		// No overflow
		NS,		// >= 0
		NE,		// != 
		OV,		// Overflow
		SG,		// < 0
		EQ,		// ==	
        PE,     // Parity even
        PO,     // parity odd
	}
}
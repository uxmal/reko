/* 
 * Copyright (C) 1999-2007 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using System;
using System.Collections;

namespace Decompiler.Analysis.Typing
{
	/// <summary>
	/// Analyzes a procedure and generates constraints
	/// </summary>
	public class ConstraintGenerator : InstructionVisitor
	{
		private Hashtable constraints;
		private int ty;

		public ConstraintGenerator()
		{
			constraints = new Hashtable();
		}

		private void Add(Constraint c)
		{
			constraints[c] = c;
		}

		public void GenerateConstraints(Procedure proc)
		{
			foreach (Block b in proc.RpoBlocks)
			{
				foreach (Statement stm in b.Statements)
				{
					stm.Instruction.Accept(this);
				}
			}
		}

		public override Instruction VisitAssignment(Assignment ass)
		{
			ty = 1;
			ass.Src.Accept(this);
			int tySrc = ty;
			ass.Dst.Accept(this);
			int tyDst = ty;
//			Add(new AssignmentCompatibleConstraint(t1, t2));
			return ass;
		}
	}
}

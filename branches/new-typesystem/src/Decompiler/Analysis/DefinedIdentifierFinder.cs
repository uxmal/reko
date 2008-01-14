/* 
 * Copyright (C) 1999-2008 John Källén.
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
using Decompiler.Core.Operators;
using System;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Locates the variables defined in this block by examining each
	/// statement to find variables in L-value positions.
	/// In addition, the set deadIn for each block is calculated.
	/// These are all the variables that are known to be dead on
	/// entry to the function. Dead variables won't need phi code!
	/// </summary>
	public abstract class DefinedIdentifierFinder : InstructionVisitorBase
	{
		private Procedure proc;
		private Block block;
		private Statement stmCur;

		public DefinedIdentifierFinder(Procedure proc)
		{
			this.proc = proc;
		}

		public abstract void Def(Identifier id);

		public void LocateDefs(Block b)
		{
			block = b;

			for (int i = block.Statements.Count - 1; i >= 0; --i)
			{
				stmCur = (Statement) block.Statements[i];
				stmCur.Instruction.Accept(this);
			}
		}

		public override void VisitAssignment(Assignment ass)
		{
			Identifier id = ass.Dst as Identifier;
			if (id != null)
			{
				Def(id);
			}
			else if (ass.Dst != null)
			{
				ass.Dst.Accept(this);
			}
			ass.Src.Accept(this);
		}

		public override void VisitApplication(Application app)
		{
			app.Procedure.Accept(this);
			foreach (Expression exp in app.Arguments)
			{
				UnaryExpression u = exp as UnaryExpression;
				if (u != null && u.op == Operator.addrOf)
				{
					Identifier id = u.Expression as Identifier;
					if (id != null)
						Def(id);
					else
						u.Expression.Accept(this);
				}
				else
				{
					exp.Accept(this);
				}
			}
		}
	}
}
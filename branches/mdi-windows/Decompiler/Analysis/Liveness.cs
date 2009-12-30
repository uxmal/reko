/* 
 * Copyright (C) 1999-2009 John Källén.
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
	/// Core engine that does liveness analysis on a statement, with a utility function for a basic block
	/// </summary>
	public class Liveness : InstructionVisitorBase
	{
		public virtual void Def(Identifier id)
		{
		}

		public virtual void Use(Identifier id)
		{
		}

		#region InstructionVisitor methods ////////////////////////////////////////////////////////////

		public override void VisitAssignment(Assignment a)
		{
			Identifier id = a.Dst as Identifier;
			if (id != null)
				Def(id);
			else if (a.Dst != null)
				a.Dst.Accept(this);

			a.Src.Accept(this);
		}

		public override void VisitApplication(Application appl)
		{
			appl.Procedure.Accept(this);
			for (int i = appl.Arguments.Length -1; i >= 0; --i)
			{
				UnaryExpression u = appl.Arguments[i] as UnaryExpression;
				if (u != null && u.op == Operator.addrOf)
				{
					Identifier id = (Identifier) u.Expression;
					Def(id);
				}
				else
				{
					appl.Arguments[i].Accept(this);
				}
			}
		}

		public override void VisitIdentifier(Identifier id)
		{
			Use(id);
		}

		public override void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
		}

		#endregion // Visitor Methods //////////////////////////////////////

	}
}

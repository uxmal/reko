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

namespace Decompiler.Analysis
{
	/// <summary>
	/// Deletes chains of variable copies, starting with the tail.
	/// </summary>
	public class CopyChain : InstructionVisitorBase
	{
		private SsaState ssa;
		private WorkList wl;

		public CopyChain(SsaState ssa)
		{
			this.ssa = ssa;
			wl = new WorkList();
		}

		public int CountUnaliasedUses(SsaIdentifier sid)
		{
			int c = 0;
			foreach (Statement stm in sid.uses)
			{
				if (!(stm.Instruction is AliasAssignment))
					++c;
			}
			return c;
		}

		public void Kill(Statement stm)
		{
			stm.Instruction.Accept(this);
			ssa.DeleteStatement(stm);
			while (!wl.IsEmpty)
			{
				SsaIdentifier info = (SsaIdentifier) wl.GetWorkItem();
				if (info.def != null)
				{
					info.def.Instruction.Accept(this);
					ssa.DeleteStatement(info.def);
				}
			}
		}

		public void MarkAsDead(Identifier id)
		{
			if (id == null)
				return;
			SsaIdentifier info = ssa.Identifiers[id];
			if (CountUnaliasedUses(info) <= 1)
			{
				wl.Add(info);
			}
		}

		public void Delete()
		{
			while (!wl.IsEmpty)
			{
				Identifier id = (Identifier) wl.GetWorkItem();
				SsaIdentifier info = ssa.Identifiers[id];
				info.def.Instruction.Accept(this);
				ssa.DeleteStatement(info.def);
			}
		}

		#region InstructionVisitorBase ////////////////

		public override void VisitAssignment(Assignment a)
		{
			Identifier src = a.Src as Identifier;
			if (src != null)
			{
				SsaIdentifier info = ssa.Identifiers[src];
				if (info.idOrig.Storage is StackLocalStorage)
				{
					wl.Add(info);
				}
				else
				{
					MarkAsDead(src);
				}
			}
		}

		public override void VisitPhiFunction(PhiFunction phi)
		{
			for (int i = 0; i < phi.Arguments.Length; ++i)
			{
				MarkAsDead(phi.Arguments[i] as Identifier);
			}
		}


		public override void VisitUseInstruction(UseInstruction u)
		{
			Identifier id = (Identifier) u.Expression;
			MarkAsDead(id);
		}

		#endregion
	}
}

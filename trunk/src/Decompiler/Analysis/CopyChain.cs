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

namespace Decompiler.Analysis
{
	public class CopyChainFinder : InstructionVisitorBase
	{
		private Procedure proc;
		private Statement stmCur;
		private Hashtable chains;

		public CopyChainFinder(Procedure proc)
		{
			this.proc = proc;
			chains = new Hashtable();
		}

		public Hashtable Chains
		{
			get { return chains; }
		}

		public void Process(Statement stm)
		{
			stmCur = stm;
			stm.Instruction.Accept(this);
		}

		public void CopyIdentifier(Identifier dst, Identifier src)
		{
			ArrayList chain = (ArrayList) chains[src];
			if (chain == null)
			{
				chain = new ArrayList();
			}
			chains[dst] = chain;
			chain.Add(stmCur);
		}

		public void FindCopyChains()
		{
			Hashtable visited = new Hashtable();
			WorkList wl = new WorkList();
			wl.Add(proc.EntryBlock);
			while (!wl.IsEmpty)
			{
				Block b = (Block) wl.GetWorkItem();
				if (!visited.Contains(b))
				{
					visited[b] = b;
					foreach (Statement stm in b.Statements)
					{
						Process(stm);
					}
					foreach (Block s in b.Succ)
					{
						wl.Add(s);
					}
				}
			}
		}

		public void TrashIdentifier(Identifier dst)
		{
			chains[dst] = "Trash";
		}

		public override void VisitAssignment(Assignment a)
		{
			Identifier src = a.Src as Identifier;
			if (src != null)
				CopyIdentifier(a.Dst, src);
			else
				TrashIdentifier(a.Dst);
		}

	}

	/// <summary>
	/// Deletes chains of variable copies, starting with the tail.
	/// </summary>
	[Obsolete("Use new CopyChain class, which has no SSA requirement.")]
	public class CopyChain2 : InstructionVisitorBase
	{
		private SsaState ssa;
		private WorkList wl;

		public CopyChain2(SsaState ssa)
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

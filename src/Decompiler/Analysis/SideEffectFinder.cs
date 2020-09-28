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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using System;

namespace Reko.Analysis
{
	public class SideEffectFinder : InstructionVisitorBase
	{
		private SideEffectFlags flags;

		public SideEffectFinder()
		{
		}

		public bool AreConstrained(Statement def, Statement use)
		{
			SideEffectFlags defFlags = FindSideEffect(def.Instruction);
			int iUse = use.Block.Statements.IndexOf(use);
			for (int i = def.Block.Statements.IndexOf(def) + 1; i < def.Block.Statements.Count; ++i)
			{
				if (i == iUse)
					return false;

				if (Conflict(defFlags, FindSideEffect(def.Block.Statements[i].Instruction)))
					return true;
			}
			return true;
		}

		public bool Conflict(SideEffectFlags defFlags, SideEffectFlags curFlags)
		{
			if (defFlags == SideEffectFlags.None || curFlags == SideEffectFlags.None)
				return false;
			if (defFlags == SideEffectFlags.Load && curFlags == SideEffectFlags.Load)
				return false;
			return true;
		}

		public SideEffectFlags FindSideEffect(Instruction instr)
		{
			flags = SideEffectFlags.None;
			instr.Accept(this);
			return flags;
		}

		public SideEffectFlags FindSideEffect(Expression e)
		{
			flags = SideEffectFlags.None;
			e.Accept(this);
			return flags;
		}

		public bool HasSideEffect(Instruction instr)
		{
			return FindSideEffect(instr) != SideEffectFlags.None;
		}

		public bool HasSideEffect(Expression e)
		{
			return FindSideEffect(e) != SideEffectFlags.None;
		}

		public override void VisitApplication(Application appl)
		{
			base.VisitApplication(appl);
			flags |= SideEffectFlags.Application;
		}

		public override void VisitBranch(Branch b)
		{
			base.VisitBranch(b);
			flags |= SideEffectFlags.SideEffect;
		}

		public override void VisitMemoryAccess(MemoryAccess access)
		{
			base.VisitMemoryAccess(access);
			flags |= SideEffectFlags.Load;
		}

		public override void VisitSegmentedAccess(SegmentedAccess access)
		{
			base.VisitSegmentedAccess(access);
			flags |= SideEffectFlags.Load;
		}


		public override void VisitStore(Store store)
		{
			store.Dst.Accept(this);
			store.Src.Accept(this);
			flags |= SideEffectFlags.Store;
		}
	}

	[Flags]
	public enum SideEffectFlags
	{
		None = 0,
		Load = 1,
		Store = 2,
		Application = 4,
		SideEffect = 8,
	}

}

#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Diagnostics;

namespace Reko.Analysis
{
    /// <summary>
    /// Analysis class that finds side effects in instructions and expressions.
    /// </summary>
	public class SideEffectFinder : InstructionVisitorBase
	{
		private SideEffectFlags flags;


        /// <summary>
        /// Determines whether the <paramref name="def"/> and <paramref name="use"/> statements are constrained by side effects,
        /// making movement impossible between them.
        /// </summary>
        /// <param name="def">Defining statement.</param>
        /// <param name="use">Using statement.</param>
        /// <returns>True of the statements cannot be moved because of 
        /// side effects; otherwise false.
        /// </returns>
		public bool AreConstrained(Statement def, Statement use)
		{
            Debug.Assert(def.Block == use.Block, "Def and use must be in the same block.");
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


        /// <summary>
        /// Determinses whether the given side effect flags <paramref name="defFlags"/>
        /// constitute a conflict.
        /// </summary>
        /// <param name="defFlags">Side effects seen</param>
        /// <param name="curFlags">Side effects to compare to.</param>
        /// <returns></returns>
		public bool Conflict(SideEffectFlags defFlags, SideEffectFlags curFlags)
		{
			if (defFlags == SideEffectFlags.None || curFlags == SideEffectFlags.None)
				return false;
			if (defFlags == SideEffectFlags.Load && curFlags == SideEffectFlags.Load)
				return false;
			return true;
		}

        /// <summary>
        /// Given an <see cref="Instruction"/>, finds its side effects.
        /// </summary>
        /// <param name="instr">Instruction to analyze.</param>
        /// <returns>The side effects of the instruction.</returns>
		public SideEffectFlags FindSideEffect(Instruction instr)
		{
			flags = SideEffectFlags.None;
			instr.Accept(this);
			return flags;
		}

        /// <summary>
        /// Given an <see cref="Expression"/>, finds its side effects.
        /// </summary>
        /// <param name="e"><see cref="Expression"/> to analyze.</param>
        /// <returns>The side effects of the expression.</returns>
		public SideEffectFlags FindSideEffect(Expression e)
		{
			flags = SideEffectFlags.None;
			e.Accept(this);
			return flags;
		}

        /// <summary>
        /// Returns true if the given expression <paramref name="e"/> has side effects.
        /// </summary>
        /// <param name="e"><see cref="Expression"/> to test.</param>
        /// <returns>True if there are side effects in the expression; otherwise
        /// false.
        /// </returns>
		public bool HasSideEffect(Expression e)
		{
			return FindSideEffect(e) != SideEffectFlags.None;
		}

        /// <inheritdoc/>
		public override void VisitApplication(Application appl)
		{
			base.VisitApplication(appl);
            //$TODO: calling intrinsics with no side effects should
            // not be considered side-effecting.
            flags |= SideEffectFlags.Application;
		}

        /// <inheritdoc/>
		public override void VisitBranch(Branch b)
		{
			base.VisitBranch(b);
			flags |= SideEffectFlags.SideEffect;
		}

        /// <inheritdoc/>
		public override void VisitMemoryAccess(MemoryAccess access)
		{
			base.VisitMemoryAccess(access);
			flags |= SideEffectFlags.Load;
		}

        /// <inheritdoc/>
		public override void VisitStore(Store store)
		{
			store.Dst.Accept(this);
			store.Src.Accept(this);
			flags |= SideEffectFlags.Store;
		}
	}

    /// <summary>
    /// Describes the kind of side effects that an instruction or expression may have.
    /// </summary>
	[Flags]
	public enum SideEffectFlags
	{
        /// <summary>
        /// No side effects.
        /// </summary>
		None = 0,

        /// <summary>
        /// A memory load operation occurs.
        /// </summary>
		Load = 1,

        /// <summary>
        /// A memory store operation occurs.
        /// </summary>
        Store = 2,

        /// <summary>
        /// A call to a procedure occurs.
        /// </summary>
		Application = 4,

        /// <summary>
        /// A call to an intrinsic function that has side effects occurs.
        /// </summary>
		SideEffect = 8,
	}

}

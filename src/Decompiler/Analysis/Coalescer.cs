#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Analysis;

/// <summary>
/// Builds expression trees out of identifier assignment statements and 
/// moves assignment statements (def's) as close to their 
/// uses in this block as possible. 
/// </summary>
/// <remarks>
/// This transformation is quite destabilizing and ineffective if long 
/// chains of expressions are still in their 3-address format. Before
/// coalescing, call <see cref="ValuePropagator"/> to perform constant
/// propagation and other transformations that make the expression trees
/// smaller.
/// </remarks>
public class Coalescer : IAnalysis<SsaState>
{
    private static readonly TraceSwitch trace = new(nameof(Coalescer), "Traces the progress of identifier coalescing");

    public Coalescer(AnalysisContext context)
    {
    }

    public string Id => "coa";

    public string Description => "Builds expression trees by joining using expressions with definitions";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        var worker = new Worker(ssa);
        bool changed = worker.Transform();
        return (ssa, changed);
    }
    private class Worker: InstructionTransformer
	{
		private readonly SsaState ssa;
		private readonly SideEffectFinder sef;
        private readonly Dictionary<Statement, List<SsaIdentifier>> defsByStatement;
        private bool changed;
        private bool coalesced;

        public Worker(SsaState ssa)
		{
			this.ssa = ssa;
			this.sef = new SideEffectFinder();
            this.defsByStatement = new Dictionary<Statement, List<SsaIdentifier>>();
            foreach (SsaIdentifier sid in ssa.Identifiers)
            {
                if (sid.DefStatement is not null)
                    SetDefStatement(sid.DefStatement, sid);
            }
		}

        private void SetDefStatement(Statement stm, SsaIdentifier sid)
        {
            if (defsByStatement.TryGetValue(sid.DefStatement, out var sids))
            {
                sids.Remove(sid);
            }
            if (!defsByStatement.TryGetValue(stm, out sids))
            {
                sids = new List<SsaIdentifier>();
                defsByStatement.Add(stm, sids);
            }
            sids.Add(sid);
        }

        private static bool IsAssignmentToInvalidConstant(Statement stm)
        {
            if (stm.Instruction is not Assignment ass)
                return false;
            return ass.Src is InvalidConstant;
        }

        /// <summary>
        /// Returns true if the identifer <paramref name="sid"/>, which is defined in <paramref name="def"/>, can safely
        /// be coalesced into <paramref name="use"/>.
        /// </summary>
        /// <param name="sid">identifier common to <paramref name="def"/> and <paramref name="use"/>.</param>
        /// <param name="def">Statement that defines <paramref name="sid"/>. </param>
        /// <param name="use">Statement that uses <paramref name="sid"/>. </param>
        /// <returns></returns>
		public bool CanCoalesce(SsaIdentifier sid, Statement def, Statement use)
		{
			if (sid.Uses.Count != 1)
				return false;
			System.Diagnostics.Debug.Assert(sid.Uses[0] == use);
			if (use.Instruction is PhiAssignment)
				return false;
			if (use.Instruction is UseInstruction)
				return false;
            // A correctly decompiled program shouldn't contain any 
            // `Constant.Invalid`, but Reko uses `Constant.Invalid` to mark
            // places where it has noticed something is wrong. In such cases
            // we avoid coalescing to make it easier to troubleshoot what the
            // problem is in the generated code
            if (IsAssignmentToInvalidConstant(def))
                return false;

            //$PERFORMANCE: this loop might be slow and should be improved if possible.
            if (defsByStatement.TryGetValue(def, out var sids))
            {
                foreach (SsaIdentifier sidOther in sids)
                {
                    if (sidOther != sid && sidOther.IsSideEffect)
                    {
                        if (sidOther.Uses.Contains(use))
                            return false;
                    }
                }
            }
			return true;
		}

		/// <summary>
		/// Coalesces the single use and the single definition of an identifier.
		/// </summary>
		/// <param name="sid"></param>
		/// <param name="defExpr"></param>
		/// <param name="def"></param>
		/// <param name="use"></param>
		/// <returns></returns>
		public bool CoalesceStatements(SsaIdentifier sid, Expression defExpr, Statement def, Statement use)
		{
            PreCoalesceDump(sid, def, use);
            use.Instruction.Accept(new IdentifierReplacer(ssa.Identifiers, use, sid.Identifier, defExpr, false));

			if (defsByStatement.TryGetValue(def, out var sids))
			{
				foreach (SsaIdentifier s in sids)
				{
					if (s != sid)
					{
						s.DefStatement = use;
						SetDefStatement(use, s);
					}
				}
			}
			ssa.DeleteStatement(def);
			PostCoalesceDump(use);
			return true;
		}

        [Conditional("DEBUG")]
        private static void PreCoalesceDump(SsaIdentifier sid, Statement def, Statement use)
        {
            if (trace.TraceInfo)
            {
                Debug.WriteLineIf(trace.TraceInfo, "Coalescing on " + sid.Identifier.ToString());
                Debug.Indent();
                Debug.WriteLineIf(trace.TraceInfo, def.Instruction.ToString());
                Debug.WriteLineIf(trace.TraceInfo, use.Instruction.ToString());
                Debug.Unindent();
            }
        }

        [Conditional("DEBUG")]
        private static void PostCoalesceDump(Statement use)
        {
            if (trace.TraceInfo)
            {
                Debug.WriteLineIf(trace.TraceInfo, "  ; coalesced to");
                Debug.Indent();
                Debug.WriteLineIf(trace.TraceInfo, use.Instruction.ToString());
                Debug.Unindent();
            }
        }

		private static bool MoveAssignment(int initPos, int newPos, Block block)
		{
			if (initPos + 1 == newPos)
				return false;
			block.Statements.Insert(newPos, block.Statements[initPos]);
			block.Statements.RemoveAt(initPos);
			return true;
		}

		public void Process(Block block)
		{
			do
			{
				coalesced = false;

				var visited = new HashSet<Identifier>();
				for (int i = 0; i < block.Statements.Count; ++i)
				{
					Statement stmDef = block.Statements[i];
					if (stmDef.Instruction is Assignment ass && !visited.Contains(ass.Dst))
					{
						visited.Add(ass.Dst);
						SsaIdentifier sidDef = ssa.Identifiers[ass.Dst];
						if (TryMoveAssignment(stmDef, sidDef, ass.Src, block, i))
						{
							--i;
						}
					}
				}
                changed |= coalesced;
			} while (coalesced);
		}

		public bool Transform()
		{
            this.changed = false;
			foreach (Block b in ssa.Procedure.ControlGraph.Blocks)
			{
				Process(b);
			}
            return changed;
		}

		/// <summary>
		/// Tries to move the assigment as far down the block as is possible.
		/// </summary>
		/// <param name="ass"></param>
		/// <param name="block"></param>
		/// <param name="i"></param>
		/// <returns>true if a change was made</returns>
		public bool TryMoveAssignment(Statement stmDef, SsaIdentifier sidDef, Expression defExpr, Block block, int initialPosition)
		{
			SideEffectFlags flagsDef = sef.FindSideEffect(stmDef.Instruction);
			for (int i = initialPosition + 1; i < block.Statements.Count; ++i)
			{
				Statement stm = block.Statements[i];
				if (sidDef.Uses.Contains(stm))
				{
					if (CanCoalesce(sidDef, stmDef, stm))
					{
						coalesced = true;
						return CoalesceStatements(sidDef, defExpr, stmDef, stm);
					}
					else
					{
						return MoveAssignment(initialPosition, i, block);
					}
				}
				if (stm.Instruction.IsControlFlow)
				{
					return MoveAssignment(initialPosition, i, block);
				}

				SideEffectFlags flagsStm = sef.FindSideEffect(stm.Instruction);
				if (sef.Conflict(flagsDef, flagsStm))
				{
					return MoveAssignment(initialPosition, i, block);
				}
			}
			return MoveAssignment(initialPosition, block.Statements.Count, block);
		}
    }
}

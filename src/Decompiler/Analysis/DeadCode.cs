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
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Analysis
{
    /// <summary>
    /// Performs dead code elimination. Statements that define an identifier
    /// that has no uses are removed, unless they are marked as critical.
    /// Critical statemets are always retained, even if they define dead 
    /// identifiers.
    /// </summary>
    public class DeadCode : InstructionVisitorBase
	{
		private static readonly TraceSwitch trace = new(nameof(DeadCode), "Traces dead code elimination");
        
        private readonly SsaState ssa;
		private readonly WorkList<SsaIdentifier> liveIds;

		private DeadCode(SsaState ssa) 
		{
			this.ssa = ssa;
            this.liveIds = new WorkList<SsaIdentifier>();
        }

		/// <summary>
		/// Cleanup statements of the type eax = Foo(); where eax is dead (has no uses),
		/// turning them into side-effect functions calls like Foo();
		/// </summary>
		public void AdjustApplicationsWithDeadReturnValues()
		{
			foreach (Block b in ssa.Procedure.ControlGraph.Blocks)
			{
				for (int iStm = 0; iStm < b.Statements.Count; ++iStm)
				{
					Statement stm = b.Statements[iStm];
					if (stm.Instruction is Assignment ass)
					{
						if (ass.Dst is Identifier id && ContainedApplication(ass.Src) is Application app)
						{
							if (ssa.Identifiers[id].Uses.Count == 0)
							{
								stm.Instruction = new SideEffect(app);
								ssa.Identifiers[id].DefStatement = null!;
							}
						}
					}
				}
			}
		}

        /// <summary>
        /// Digs out an <see cref="Application"/> that may be masked by a 
        /// chain of dead expressions.
        /// expressions.
        /// </summary>
        private static Application? ContainedApplication(Expression exp)
        {
            for (; ;)
            {
                switch (exp)
                {
                case Application app: return app;
                case Conversion convert: return ContainedApplication(convert.Expression);
                case Cast cast: return ContainedApplication(cast.Expression);
                case Slice slice: return ContainedApplication(slice.Expression);
                case UnaryExpression u: return ContainedApplication(u.Expression);
                case Dereference deref: return ContainedApplication(deref.Expression);
                case MkSequence seq:
                    Application? appl = null;
                    foreach (var elem in seq.Expressions)
                    {
                        var a = ContainedApplication(elem);
                        if (a is not null)
                        {
                            if (appl is null)
                                appl = a;
                            else
                                return null;
                        }
                    }
                    return appl;
                default:
                    return null;
                }
            }
        }

        /// <summary>
        /// Remove dead "def variables in a call instruction".
        /// </summary>
        /// <param name="call"></param>
        public void AdjustCallWithDeadDefinitions(CallInstruction call)
        {
            call.Definitions.RemoveWhere(def =>
            {
                var id =(Identifier)def.Expression;
                var sid = ssa.Identifiers[id];
                if (sid.Uses.Count == 0)
                {
                    sid.DefStatement = null!;
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public static void Eliminate(SsaState ssa)
		{
			new DeadCode(ssa).Eliminate();
		}

		private void Eliminate()
		{
			var marks = new HashSet<Statement>();

			// Initially, just mark those statements that contain critical statements.
			// These are calls to other functions, functions (which have side effects) and use statements.
			// Critical instructions must never be considered dead.

            foreach (var stm in ssa.Procedure.Statements)
            {
                if (CriticalInstruction.IsCritical(stm.Instruction))
                {
                    trace.Inform("Critical: {0}", stm.Instruction);
                    marks.Add(stm);
                    stm.Instruction.Accept(this);		// mark all used identifiers as live.
                }
            }
			
			// Each identifier is live, so its defining statement is also live.

			while (liveIds.TryGetWorkItem(out SsaIdentifier? sid))
			{
				Statement? def = sid.DefStatement;
				if (def is not null)
				{
					if (!marks.Contains(def))
					{
						trace.Inform("Marked: {0}", def.Instruction);
                        marks.Add(def);
						sid.DefStatement?.Instruction.Accept(this);
					}
				}
			}

			// We have now marked all the useful instructions in the code. Any non-marked
            // instruction is now useless and should be deleted. Now remove all uses; 
            // we just proved that no-one uses the non-marked instructions.
            foreach (var sid in ssa.Identifiers)
            {
                sid.Uses.RemoveAll(u => !marks.Contains(u));
                if (sid.DefStatement is not null && !marks.Contains(sid.DefStatement))
                    sid.DefStatement = null!;
            }
			foreach (Block b in ssa.Procedure.ControlGraph.Blocks)
			{
                int iTo = 0;
				for (int iStm = 0; iStm < b.Statements.Count; ++iStm)
				{
					Statement stm = b.Statements[iStm];
                    if (stm.Instruction is CallInstruction call)
                    {
                        AdjustCallWithDeadDefinitions(call);
                    }
                    if (marks.Contains(stm))
					{
                        b.Statements[iTo] = stm;
                        ++iTo;
					}
                    else
                    {
                        trace.Inform("Deleting: {0}", stm.Instruction);
				}
			}
                b.Statements.RemoveRange(iTo, b.Statements.Count - iTo);
			}

			AdjustApplicationsWithDeadReturnValues();
		}

		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
		}

        public override void VisitCallInstruction(CallInstruction ci)
        {
            base.VisitCallInstruction(ci);
            foreach (var use in ci.Uses)
            {
                use.Expression.Accept(this);
            }
        }

        public override void VisitIdentifier(Identifier id)
		{
			SsaIdentifier sid = ssa.Identifiers[id];
			if (sid.DefStatement is not null)
				liveIds.Add(sid);
		}

		public override void VisitStore(Store store)
		{
            if (store.Dst is not Identifier idDst)
            {
                store.Dst.Accept(this);
            }
            store.Src.Accept(this);
		}
	}
}

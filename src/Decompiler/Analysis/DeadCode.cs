#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
		private SsaState ssa;
		private WorkList<SsaIdentifier> liveIds;
		private CriticalInstruction critical;

		private static TraceSwitch trace = new TraceSwitch("DeadCode", "Traces dead code elimination");

		private DeadCode(SsaState ssa) 
		{
			this.ssa = ssa;
			this.critical = new CriticalInstruction();
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
					Assignment ass = stm.Instruction as Assignment;
					if (ass != null)
					{
						Identifier id = ass.Dst as Identifier;
						if (id != null && ass.Src is Application)
						{
							if (ssa.Identifiers[id].Uses.Count == 0)
							{
								stm.Instruction = new SideEffect(ass.Src);
								ssa.Identifiers[id].DefStatement = null;
							}
						}
					}
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
                    sid.DefExpression = null;
                    sid.DefStatement = null;
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
			liveIds = new WorkList<SsaIdentifier>();
			HashSet<Statement> marks = new HashSet<Statement>();

			// Initially, just mark those statements that contain critical statements.
			// These are calls to other functions, functions (which have side effects) and use statements.
			// Critical instructions must never be considered dead.

            foreach (var stm in ssa.Procedure.Statements)
            {
                if (critical.IsCritical(stm.Instruction))
                {
                    if (trace.TraceInfo) Debug.WriteLineIf(trace.TraceInfo, string.Format("Critical: {0}", stm.Instruction));
                    marks.Add(stm);
                    stm.Instruction.Accept(this);		// mark all used identifiers as live.
                }
            }
			
			// Each identifier is live, so its defining statement is also live.

            SsaIdentifier sid;
			while (liveIds.GetWorkItem(out sid))
			{
				Statement def = sid.DefStatement;
				if (def != null)
				{
					if (!marks.Contains(def))
					{
						if (trace.TraceInfo) Debug.WriteLine(string.Format("Marked: {0}", def.Instruction));
                        marks.Add(def);
						sid.DefStatement.Instruction.Accept(this);
					}
				}
			}

			// We have now marked all the useful instructions in the code. Any non-marked
			// instruction is now useless and should be deleted.

			foreach (Block b in ssa.Procedure.ControlGraph.Blocks)
			{
				for (int iStm = 0; iStm < b.Statements.Count; ++iStm)
				{
					Statement stm = b.Statements[iStm];
                    if (stm.Instruction is CallInstruction call)
                    {
                        AdjustCallWithDeadDefinitions(call);
                    }
					if (!marks.Contains(stm))
					{
						if (trace.TraceInfo) Debug.WriteLineIf(trace.TraceInfo, string.Format("Deleting: {0}", stm.Instruction));
						ssa.DeleteStatement(stm);
						--iStm;
					}
				}
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
			if (sid.DefStatement != null)
				liveIds.Add(sid);
		}

		public override void VisitStore(Store store)
		{
            var idDst = store.Dst as Identifier;
            if (idDst == null || (!(idDst.Storage is OutArgumentStorage)))
            {
                store.Dst.Accept(this);
            }
			store.Src.Accept(this);
		}
	}
}

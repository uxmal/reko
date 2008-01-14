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
using System;
using System.Collections;
using System.Diagnostics;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Performs dead code elimination. An identifier that has no uses are removed, along
	/// with the statement that defined it.
	/// </summary>
	public class DeadCode : InstructionVisitorBase
	{
		private Procedure proc;
		private SsaState ssa;
		private WorkList liveIds;
		private CriticalInstruction critical;

		private static TraceSwitch trace = new TraceSwitch("DeadCode", "Traces dead code elimination");

		private DeadCode(Procedure proc, SsaState ssa) 
		{
			this.proc = proc;
			this.ssa = ssa;
			this.critical = new CriticalInstruction();
		}


		/// <summary>
		/// Cleanup statements of the type eax = Foo(); where eax is dead (has no uses),
		/// turning them into side-effect functions calls like Foo();
		/// </summary>
		public void AdjustApplicationsWithDeadReturnValues()
		{
			foreach (Block b in proc.RpoBlocks)
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
							if (ssa.Identifiers[id].uses.Count == 0)
							{
								stm.Instruction = new SideEffect(ass.Src);
								ssa.Identifiers[id].def = null;
							}
						}
					}
				}
			}
		}

		public static void Eliminate(Procedure proc, SsaState ssa)
		{
			new DeadCode(proc, ssa).Eliminate();
		}

		private void Eliminate()
		{
			liveIds = new WorkList();
			Hashtable marks = new Hashtable();

			// Initially, just mark those statements that contain critical statements.
			// These are calls to other functions, functions (which have side effects) and use statements.
			// Critical instructions must never be considered dead.

			foreach (Block b in proc.RpoBlocks)
			{
				foreach (Statement stm in b.Statements)
				{
					if (critical.IsCritical(stm.Instruction))
					{
						if (trace.TraceInfo) Debug.WriteLineIf(trace.TraceInfo, string.Format("Critical: {0}", stm.Instruction));
						marks[stm] = stm;
						stm.Instruction.Accept(this);		// mark all used identifiers as live.
					}
				}
			}
			
			// Each identifier is live, so its defining statement is also live.

			while (!liveIds.IsEmpty)
			{
				SsaIdentifier sid = NextWorkItem(liveIds);
				Statement def = sid.def;
				if (def != null)
				{
					if (!marks.Contains(def))
					{
						if (trace.TraceInfo) Debug.WriteLine(string.Format("Marked: {0}", def.Instruction));
						marks[sid.def] = def;
						sid.def.Instruction.Accept(this);
					}
				}
			}

			// We have now marked all the useful instructions in the code. Any non-marked
			// instruction is now useless and should be deleted.

			foreach (Block b in proc.RpoBlocks)
			{
				for (int iStm = 0; iStm < b.Statements.Count; ++iStm)
				{
					Statement stm = b.Statements[iStm];
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



		private SsaIdentifier NextWorkItem(WorkList wl)
		{
			return (SsaIdentifier) liveIds.GetWorkItem();
		}

		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
		}

		public override void VisitIdentifier(Identifier id)
		{
			SsaIdentifier sid = ssa.Identifiers[id];
			if (sid.def != null)
				liveIds.Add(sid);
		}

		public override void VisitStore(Store store)
		{
			store.Dst.Accept(this);
			store.Src.Accept(this);
		}
	}
}

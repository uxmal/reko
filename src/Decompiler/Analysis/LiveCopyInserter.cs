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

namespace Decompiler.Analysis
{
	public class LiveCopyInserter
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;
		private SsaLivenessAnalysis sla;
		private DominatorGraph doms;

		public LiveCopyInserter(Procedure proc, SsaIdentifierCollection ssaIds)
		{
			this.proc = proc;
			this.ssaIds = ssaIds;
			this.sla = new SsaLivenessAnalysis(proc, ssaIds);
			this.doms = new DominatorGraph(proc);
		}

		public int IndexOfInsertedCopy(Block b)
		{
			int i = b.Statements.Count;
			if (i > 0)
			{
				if (b.Statements[i-1].Instruction.IsControlFlow)
					--i;
			}
			return i;
		}

		public Identifier InsertAssignmentNewId(Identifier idOld, Block b, int i)
		{
			Statement stm = new Statement(null, b);
			SsaIdentifier sidNew = ssaIds.Add(ssaIds[idOld].idOrig, stm);
			stm.Instruction = new Assignment(sidNew.id, idOld);
			b.Statements.Insert(i, stm);
			return sidNew.id;
		}

		public Identifier InsertAssignment(Identifier idDst, Identifier idSrc, Block b, int i)
		{
			b.Statements.Insert(i, new Assignment(idDst, idSrc));
			return idDst;
		}

		public bool IsLiveOut(Identifier id, Statement stm)
		{
			return sla.IsLiveOut(id, stm);
		}

		public bool IsLiveAtCopyPoint(Identifier id, Block b)
		{
			if (b.Statements.Count == 0)
				return sla.IsLiveOut(id, b);
			int i = IndexOfInsertedCopy(b);
			if (i >= b.Statements.Count)
				return sla.IsLiveOut(id, b.Statements[i-1]);
			else
				return sla.IsLiveIn(id, b.Statements[i]);
		}

		public void RenameDominatedIdentifiers(SsaIdentifier sidOld, SsaIdentifier sidNew)
		{
			DominatedUseRenamer dur = new DominatedUseRenamer(doms);
			dur.Transform(sidOld, sidNew);
		}

		public void Transform()
		{
			for (int i = 0; i < ssaIds.Count; ++i)
			{
				SsaIdentifier sid = ssaIds[i];
				if (sid.def == null || sid.uses.Count == 0)
					continue;
				PhiAssignment ass = sid.def.Instruction as PhiAssignment;
				if (ass != null)
				{
					Transform(sid.def, ass);
				}
			}
		}

		public void Transform(Statement stm, PhiAssignment phi)
		{
			Identifier idDst = (Identifier) phi.Dst;
			for (int i = 0; i < phi.Src.Arguments.Length; ++i)
			{
				Identifier id = (Identifier) phi.Src.Arguments[i];
				Block pred = stm.block.Pred[i];
				if (idDst != id)
				{
					if (IsLiveAtCopyPoint(idDst, pred))
					{
						int idx = IndexOfInsertedCopy(pred);
						Identifier idNew = InsertAssignmentNewId(idDst, pred, idx);
						RenameDominatedIdentifiers(ssaIds[idDst], ssaIds[idNew]);
					}
					else if (IsLiveOut(id, stm))
					{
						phi.Src.Arguments[i] = idDst;
						int idx = IndexOfInsertedCopy(pred);
						Identifier idNew = InsertAssignment(idDst, id, pred, idx);
					}
				}
			}
		}


		public class DominatedUseRenamer : InstructionTransformer
		{
			private DominatorGraph domGraph;
			private SsaIdentifier sidOld; 
			private SsaIdentifier sidNew;
			private Statement stmCur;

			public DominatedUseRenamer(DominatorGraph domGraph)
			{
				this.domGraph = domGraph;
			}

			public void Transform(SsaIdentifier sidOld, SsaIdentifier sidNew)
			{
				this.sidOld = sidOld;
				this.sidNew = sidNew;

				foreach (Statement stm in sidOld.uses)
				{
					stmCur = stm;
					if (domGraph.DominatesStrictly(sidOld.def, stm))
					{
						stm.Instruction = stm.Instruction.Accept(this);
					}
				}
			}

			public override Expression TransformIdentifier(Identifier id)
			{
				return (id == sidOld.id) ? sidNew.id : id;
			}


			public int Index(Statement stm)
			{
				return stm.block.Statements.IndexOf(stm);
			}
		}

	}
}


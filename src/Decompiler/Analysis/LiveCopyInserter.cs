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
using Reko.Core.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
	public class LiveCopyInserter
	{
		private readonly SsaIdentifierCollection ssaIds;
		private readonly SsaLivenessAnalysis sla;
		private readonly BlockDominatorGraph doms;

		public LiveCopyInserter(SsaState ssa)
		{
			this.ssaIds = ssa.Identifiers;
			this.sla = new SsaLivenessAnalysis(ssa);
			this.doms = ssa.Procedure.CreateBlockDominatorGraph();
		}

		public static int IndexOfInsertedCopy(Block b)
		{
			int i = b.Statements.Count;
			if (i > 0)
			{
				if (b.Statements[i-1].Instruction.IsControlFlow)
					--i;
			}
			return i;
		}

        //$REFACTOR: this should go to SsaState.
		public Identifier InsertAssignmentNewId(Identifier idOld, Block b, int i)
		{
            var stm = new Statement(
                b.Address,
                null!,
                b);
            SsaIdentifier sidNew = ssaIds.Add((Identifier)ssaIds[idOld].OriginalIdentifier, stm, false);
			stm.Instruction = new Assignment(sidNew.Identifier, idOld);
			b.Statements.Insert(i, stm);
			return sidNew.Identifier;
		}

		public static Identifier InsertAssignment(Identifier idDst, Identifier idSrc, Block b, int i)
		{
            b.Statements.Insert(
                i,
                b.Address,
                new Assignment(idDst, idSrc));
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
			var dur = new DominatedUseRenamer(doms);
			dur.Transform(sidOld, sidNew);
		}

		public void Transform()
		{
			foreach (var sid in ssaIds.ToArray())
			{
				if (sid.DefStatement is null || sid.Uses.Count == 0)
					continue;
                if (sid.DefStatement.Instruction is PhiAssignment ass)
                {
                    Transform(sid.DefStatement, ass);
                }
            }
		}

		public void Transform(Statement stm, PhiAssignment phi)
		{
			Identifier idDst = phi.Dst;
			for (int i = 0; i < phi.Src.Arguments.Length; ++i)
			{
                Block pred = phi.Src.Arguments[i].Block;
                if (phi.Src.Arguments[i].Value is Identifier id &&
                    id.Storage is not MemoryStorage &&
                    idDst != id)
				{
					if (IsLiveAtCopyPoint(idDst, pred))
					{
						int idx = IndexOfInsertedCopy(pred);
						Identifier idNew = InsertAssignmentNewId(idDst, pred, idx);
						RenameDominatedIdentifiers(ssaIds[idDst], ssaIds[idNew]);
					}
					else if (IsLiveOut(id, stm))
					{
                        phi.Src.Arguments[i] = new PhiArgument(pred, idDst);
                        int idx = IndexOfInsertedCopy(pred);
						InsertAssignment(idDst, id, pred, idx);
					}
                }
            }
		}


		public class DominatedUseRenamer : InstructionTransformer
		{
			private readonly BlockDominatorGraph domGraph;
			private SsaIdentifier? sidOld; 
			private SsaIdentifier? sidNew;

			public DominatedUseRenamer(BlockDominatorGraph domGraph)
			{
				this.domGraph = domGraph;
			}

			public void Transform(SsaIdentifier sidOld, SsaIdentifier sidNew)
			{
				this.sidOld = sidOld;
				this.sidNew = sidNew;

				foreach (Statement stm in sidOld.Uses)
				{
					if (domGraph.DominatesStrictly(sidOld.DefStatement, stm))
					{
						stm.Instruction = stm.Instruction.Accept(this);
					}
				}
			}

			public override Expression VisitIdentifier(Identifier id)
			{
				return (id == sidOld!.Identifier) ? sidNew!.Identifier : id;
			}


			public static int Index(Statement stm)
			{
				return stm.Block.Statements.IndexOf(stm);
			}
		}

	}
}


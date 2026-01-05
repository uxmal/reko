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
using Reko.Core.Output;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Reko.Core.Analysis;

namespace Reko.Analysis
{
    /// <summary>
    /// Performs liveness analysis on SSA identifiers in a procedure.
    /// </summary>
    /// <remarks>
    /// This class is used to determine which SSA identifiers are live at
    /// each statement in the procedure, and which identifiers are live at
    /// the end of each block.
    /// </remarks>
	public class SsaLivenessAnalysis : InstructionVisitorBase
	{
        private class Record
        {
            public Record()
            {
                this.LiveIn = new List<SsaIdentifier>();
                this.LiveOut = new List<SsaIdentifier>();
            }

            public List<SsaIdentifier> LiveIn { get; }
            public List<SsaIdentifier> LiveOut { get; }
        }

		private readonly SsaIdentifierCollection ssaIds;
        private readonly HashSet<Block> visited;
		private readonly Dictionary<Statement, List<Identifier>> defined;		// maps statement to -> List of identifiers
		private readonly InterferenceGraph interference;
        private readonly Dictionary<Block, Record> records;

        /// <summary>
        /// Constructs an instance of <see cref="SsaLivenessAnalysis"/>
        /// </summary>
        /// <param name="ssa"><see cref="SsaState"/> of the procedure being 
        /// analyzed.
        /// </param>
		public SsaLivenessAnalysis(SsaState ssa)
		{
			this.ssaIds = ssa.Identifiers;
            this.visited = new HashSet<Block>();
            this.records = new Dictionary<Block, Record>();
			this.defined = new Dictionary<Statement,List<Identifier>>();
            this.interference = new InterferenceGraph();

            BuildRecords(ssa.Procedure.ControlGraph.Blocks);
			BuildDefinedMap(ssaIds);
			BuildInterferenceGraph(ssaIds);
		}

		private void BuildRecords(IEnumerable<Block> blocks)
		{
            foreach (Block b in blocks)
            {
                records.Add(b, new Record());
			}
		}

		private void BuildDefinedMap(SsaIdentifierCollection ssaIds)
		{
			foreach (SsaIdentifier ssa in ssaIds)
			{
				if (ssa.Uses.Count > 0 && ssa.DefStatement is not null)
				{
                    if (!defined.TryGetValue(ssa.DefStatement, out List<Identifier>? al))
					{
						al = new List<Identifier>();
						defined.Add(ssa.DefStatement, al);
					}
					al.Add(ssa.Identifier);
				}
			}
		}

		private void BuildInterferenceGraph(SsaIdentifierCollection ssaIds)
		{
			foreach (SsaIdentifier v in ssaIds)
			{
                visited.Clear();
				foreach (Statement s in v.Uses)
				{
					PhiFunction? phi = GetPhiFunction(s);
					if (phi is not null)
					{
                        var p = phi.Arguments.First(e => e.Value == v.Identifier).Block;
						LiveOutAtBlock(p, v);
					}
					else
					{
						int i = s.Block.Statements.IndexOf(s);
						LiveInAtStatement(s.Block, i, v);
					}
				}
			}
		}

		private List<Identifier> IdentifiersDefinedAtStatement(Statement stm)
		{
            if (stm is null) return [];
			return defined[stm];
		}

        /// <summary>
        /// Determines whether <paramref name="sid"/> is defined at statement
        /// <paramref name="stm"/>.
        /// </summary>
        /// <param name="sid"><see cref="SsaIdentifier"/> to test.</param>
        /// <param name="stm"><see cref="Statement"/> to test.</param>
        /// <returns>
        /// True if the SSA identifier is defined at the given statement;
        /// otherwise false.
        /// </returns>
		public bool IsDefinedAtStatement(SsaIdentifier sid, Statement? stm)
		{
			return (sid.DefStatement == stm);
		}

        /// <summary>
        /// Determine whether the identifier <paramref name="id"/> is live-in
        /// right before the execution of the statement <paramref name="stm"/>.
        /// </summary>
        /// <param name="id">Identifier to test for liveness.</param>
        /// <param name="stm">Statement in reference to which the liveness test is made.</param>
        /// <returns>True if <paramref name="id"/> is live-in to <paramref name="stm"/>;
        /// otherwise false.
        /// </returns>
		public bool IsLiveIn(Identifier id, Statement stm)
		{
			bool live = records[stm.Block].LiveOut.Contains(ssaIds[id]);
			for (int i = stm.Block.Statements.Count - 1; i >= 0; --i)
			{
				Statement s = stm.Block.Statements[i];
				if (ssaIds[id].DefStatement == s)
					return false;

				if (s.Instruction is not PhiAssignment && ssaIds[id].Uses.Contains(s))
					live = true;

				if (s == stm)
					break;
			}
			return live;
		}

        /// <summary>
        /// Determines whether <paramref name="id"/> is live-out of the
        /// basic block <paramref name="block"/>.
        /// </summary>
        /// <param name="id">Identifier whose liveness is tested.</param>
        /// <param name="block"><see cref="Block">Basic block</see> being tested.</param>
        /// <returns>Returns true if <paramref name="id"/> is live-out of the 
        /// block <paramref name="block"/>; otherwise false.</returns>
		public bool IsLiveOut(Identifier id, Block block)
		{
			return records[block].LiveOut.Contains(ssaIds[id]);
		}

        /// <summary>
        /// Determines whether <paramref name="id"/> is live-out of the
        /// statement <paramref name="stm"/>.
        /// </summary>
        /// <param name="id">Identifier whose liveness is tested.</param>
        /// <param name="stm"><see cref="Statement"/> being tested.</param>
        /// <returns>Returns true if <paramref name="id"/> is live-out of the 
        /// statement <paramref name="stm"/>; otherwise false.</returns>
		public bool IsLiveOut(Identifier id, Statement stm)
		{
			bool live = records[stm.Block].LiveOut.Contains(ssaIds[id]);
			for (int i = stm.Block.Statements.Count - 1; i >= 0; --i)
			{
				Statement s = stm.Block.Statements[i];
				if (s == stm)
					return live;

				if (ssaIds[id].DefStatement == s)
					return false;

				if (s.Instruction is not PhiAssignment && ssaIds[id].Uses.Contains(s))
					live = true;
			}
			return live;
		}

		private void LiveOutAtBlock(Block block, SsaIdentifier sid)
		{
			Set(records[block].LiveOut, sid);
			if (!visited.Contains(block))
			{
				visited.Add(block);
                var s = (block.Statements.Count > 0) ? block.Statements[^1] : null;
				if (!IsDefinedAtStatement(sid, s))
					LiveInAtStatement(block, block.Statements.Count - 1, sid);
			}
		}

		private void LiveInAtStatement(Block b, int i, SsaIdentifier v)
		{
			// v is live-in at s.

			while (i > 0)
			{
				--i;
				Statement s = b.Statements[i];
				if (IsDefinedAtStatement(v, s))
					return;
			}
		
			// v is live-in at the header of this block!
			Set(records[b].LiveIn, v);
			foreach (Block p in b.Pred)
			{
				LiveOutAtBlock(p, v);
			}
		}

        /// <summary>
        /// Writes a textual representation of the procedure <paramref name="proc"/>,
        /// decorated with the liveness of SSA identifiers,
        /// to the given <see cref="TextWriter"/> <paramref name="writer"/>.
        /// </summary>
        /// <param name="proc">The procedure to write.</param>
        /// <param name="writer"><see cref="TextWriter"/> to write to.</param>
		public void Write(Procedure proc, TextWriter writer)
		{
            proc.Write(false, new SsaBlockDecorator(records), writer);
        }

        private class SsaBlockDecorator : BlockDecorator
        {
            private readonly Dictionary<Block, Record> records;

            public SsaBlockDecorator(Dictionary<Block, Record> records)
            {
                this.records = records;
            }

            public override void BeforeBlock(Block block, List<string> lines)
            {
                lines.Add("liveIn: " + string.Join(" ", records[block].LiveIn.Select(v => v.Identifier.Name)));
            }

            public override void AfterBlock(Block block, List<string> lines)
            {
                lines.Add("liveOut: " + string.Join(" ", records[block].LiveOut.Select(v => v.Identifier.Name)));
            }
		}

        /// <summary>
        /// Returns true if <paramref name="sid"/> is also live in before 
        /// executing s.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="sid"></param>
        /// <returns></returns>

        public bool LiveOutAtStatement(Statement s, SsaIdentifier sid)
        {
            // v is live-out at s.

            List<Identifier> ids = this.IdentifiersDefinedAtStatement(s);
            foreach (Identifier id in ids)
            {
                if (id != sid.Identifier)
                    interference.Add(id, sid.Identifier);
            }
            return (sid.DefStatement != s);
        }

		private static PhiFunction? GetPhiFunction(Statement stm)
		{
            return (stm.Instruction is PhiAssignment ass)
                ? ass.Src
                : null;
		}

		private static void Set(List<SsaIdentifier> s, SsaIdentifier v)
		{
			if (!s.Contains(v))
				s.Add(v);
		}
	}

    /// <summary>
    /// Alternate implementation of SSA liveness analysis.
    /// </summary>
    public class SsaLivenessAnalysis2 : InstructionVisitorBase
	{
		private readonly SsaIdentifierCollection ssa;
		private readonly Dictionary<Block,Block> visitedBlocks;
		private readonly InterferenceGraph interference;

        /// <summary>
        /// Constructs an instance of <see cref="SsaLivenessAnalysis2"/>.
        /// </summary>
        /// <param name="ssa"><see cref="SsaState"/> whose liveness is being analyzed.</param>
		public SsaLivenessAnalysis2(SsaState ssa)
		{
			this.ssa = ssa.Identifiers;
            visitedBlocks = new Dictionary<Block, Block>();
			interference = new InterferenceGraph();
		}

        /// <summary>
        /// Performs liveness analysis on the SSA identifiers in the SSA state.
        /// </summary>
		public void Analyze()
		{
			foreach (SsaIdentifier sid in ssa)
			{
				foreach (Statement use in sid.Uses)
				{
					Block? p = PrecedingPhiBlock(sid.Identifier, use);
					if (p is not null)
					{
						LiveOutAtBlock(p, sid);
					}
					else
					{
						LiveInAtStatement(use.Block, use.Block.Statements.IndexOf(use), sid);
					}
				}
			}
		}

        /// <summary>
        /// The interference graph that was built during the liveness analysis.
        /// </summary>
		public InterferenceGraph InterferenceGraph
		{
			get { return interference; }
		}

		private void LiveOutAtBlock(Block n, SsaIdentifier sid)
		{
			if (!visitedBlocks.ContainsKey(n))
			{
				visitedBlocks[n] = n;
				LiveOutAtStatement(n, n.Statements.Count-1, sid);
			}
		}

		private void LiveInAtStatement(Block block, int iStm, SsaIdentifier sid)
		{
			if (iStm <= 0)
			{
				foreach (Block p in block.Pred)
				{
					LiveOutAtBlock(p, sid);
				}
			}
			else
			{
				LiveOutAtStatement(block, --iStm, sid);
			}
		}

		private void LiveOutAtStatement(Block block, int iStm, SsaIdentifier sid)
		{
            Dictionary<SsaIdentifier, SsaIdentifier> W = iStm >= 0 
				? VariablesDefinedByStatement(block.Statements[iStm])
				: new Dictionary<SsaIdentifier,SsaIdentifier>();

			foreach (SsaIdentifier w in W.Values)
			{
				if (w != sid)
					interference.Add(w.Identifier, sid.Identifier);
			}
			if (!W.ContainsKey(sid))
				LiveInAtStatement(block, iStm, sid);
		}

		private Dictionary<SsaIdentifier,SsaIdentifier> VariablesDefinedByStatement(Statement stm)
		{
			var W = new Dictionary<SsaIdentifier,SsaIdentifier>();
			foreach (SsaIdentifier sid in ssa)
			{
				if (sid.DefStatement == stm)
					W[sid] = sid;
			}
			return W;
		}

		private static Block? PrecedingPhiBlock(Identifier u, Statement stm)
		{
            if (stm.Instruction is not PhiAssignment phi)
                return null;
            foreach (var arg in phi.Src.Arguments)
			{
                if (u == arg.Value)
                    return arg.Block;
			}
			return null;
		}
	}
}

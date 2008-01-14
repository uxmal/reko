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
using Decompiler.Core.Lib;
using System;
using System.IO;
using System.Collections;

namespace Decompiler.Analysis
{
	public class SsaLivenessAnalysis : InstructionVisitorBase
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;
		private BitSet visited;
		private Hashtable defined;		// maps statement to -> ArrayList of identifiers
		private InterferenceGraph interference; 
		private ArrayList [] liveInBlocks;
		private ArrayList [] liveOutBlocks;

		public SsaLivenessAnalysis(Procedure proc, SsaIdentifierCollection ssaIds)
		{
			this.proc = proc;
			this.ssaIds = ssaIds;
			visited = new BitSet(proc.RpoBlocks.Count);
			this.liveInBlocks = BuildLiveSet(proc.RpoBlocks.Count);
			this.liveOutBlocks = BuildLiveSet(proc.RpoBlocks.Count);
			BuildDefinedMap(ssaIds);
			BuildInterferenceGraph(ssaIds);
		}

		private ArrayList [] BuildLiveSet(int size)
		{
			ArrayList [] arr = new ArrayList[size];
			for (int i = 0; i < size; ++i)
			{
				arr[i] = new ArrayList();
			}
			return arr;
		}

		public void BuildDefinedMap(SsaIdentifierCollection ssaIds)
		{
			defined = new Hashtable();
			foreach (SsaIdentifier ssa in ssaIds)
			{
				if (ssa.uses.Count > 0 && ssa.def != null)
				{
					ArrayList al = (ArrayList) defined[ssa.def];
					if (al == null)
					{
						al = new ArrayList();
						defined.Add(ssa.def, al);
					}
					al.Add(ssa.id);
				}
			}
		}

		public void BuildInterferenceGraph(SsaIdentifierCollection ssaIds)
		{
			interference = new InterferenceGraph();
			foreach (SsaIdentifier v in ssaIds)
			{
				visited.SetAll(false);
				foreach (Statement s in v.uses)
				{
					PhiFunction phi = GetPhiFunction(s);
					if (phi != null)
					{
						int i = Array.IndexOf(phi.Arguments, v.id);
						Block p = s.block.Pred[i];
						LiveOutAtBlock(p, v);
					}
					else
					{
						int i = s.block.Statements.IndexOf(s);
						LiveInAtStatement(s.block, i, v);
					}
				}
			}
		}

		public ArrayList IdentifiersDefinedAtStatement(Statement stm)
		{
			if (stm == null) return null;
			return (ArrayList) defined[stm];
		}

		public InterferenceGraph InterferenceGraph
		{
			get { return interference; }
		}

		public bool IsDefinedAtStatement(SsaIdentifier v, Statement stm)
		{
			return (v.def == stm);
		}

		public bool IsFirstStatementInBlock(Statement stm, Block block)
		{
			return block.Statements.IndexOf(stm) == 0;
		}

		public bool IsLiveIn(Identifier id, Statement stm)
		{
			bool live = liveOutBlocks[stm.block.RpoNumber].Contains(ssaIds[id]);
			for (int i = stm.block.Statements.Count - 1; i >= 0; --i)
			{
				Statement s = stm.block.Statements[i];
				if (ssaIds[id].def == s)
					return false;

				if (!(s.Instruction is PhiAssignment) && ssaIds[id].uses.Contains(s))
					live = true;

				if (s == stm)
					break;
			}
			return live;
		}

		public bool IsLiveOut(Identifier id, Block b)
		{
			return liveOutBlocks[b.RpoNumber].Contains(ssaIds[id]);
		}

		public bool IsLiveOut(Identifier id, Statement stm)
		{
			bool live = liveOutBlocks[stm.block.RpoNumber].Contains(ssaIds[id]);
			for (int i = stm.block.Statements.Count - 1; i >= 0; --i)
			{
				Statement s = stm.block.Statements[i];
				if (s == stm)
					return live;

				if (ssaIds[id].def == s)
					return false;

				if (!(s.Instruction is PhiAssignment) && ssaIds[id].uses.Contains(s))
					live = true;
			}
			return live;
		}

		public void LiveOutAtBlock(Block b, SsaIdentifier v)
		{
			Set(liveOutBlocks[b.RpoNumber], v);
			if (!visited[b.RpoNumber])
			{
				visited[b.RpoNumber] = true;
				Statement s = b.Statements.Last;
				if (!IsDefinedAtStatement(v, s))
					LiveInAtStatement(b, b.Statements.Count - 1, v);
			}
		}

		public void LiveInAtStatement(Block b, int i, SsaIdentifier v)
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
			Set(liveInBlocks[b.RpoNumber], v);
			foreach (Block p in b.Pred)
			{
				LiveOutAtBlock(p, v);
			}
		}

		public void Write(Procedure proc, TextWriter writer)
		{
			for (int i = 0; i < proc.RpoBlocks.Count; ++i)
			{
				Block b = proc.RpoBlocks[i];
				writer.Write("liveIn: ");
				foreach (SsaIdentifier v in liveInBlocks[b.RpoNumber])
				{
					writer.Write(" {0}", v.id.Name);
				}
				writer.WriteLine();

				b.Write(writer);

				writer.Write("liveOut:");
				foreach (SsaIdentifier v in liveOutBlocks[b.RpoNumber])
				{
					writer.Write(" {0}", v.id.Name);
				}
				writer.WriteLine();
				writer.WriteLine();
			}
		}


		// Returns true if v is also live in before executing s.

		public bool LiveOutAtStatement(Statement s, SsaIdentifier v)
		{
			// v is live-out at s.

			ArrayList ids = this.IdentifiersDefinedAtStatement(s);
			if (ids != null)
			{
				foreach (Identifier id in ids)
				{
					if (id != v.id)
						interference.Add(id, v.id);
				}
			}
			return (v.def != s);
		}

		public PhiFunction GetPhiFunction(Statement stm)
		{
			PhiAssignment ass = stm.Instruction as PhiAssignment;
			if (ass == null)
				return null;
			return ass.Src;
		}

		private void Set(ArrayList s, SsaIdentifier v)
		{
			if (!s.Contains(v))
				s.Add(v);
		}
	}

	public class SsaLivenessAnalysis2 : InstructionVisitorBase
	{
		private SsaIdentifierCollection ssa;
		private Hashtable visitedBlocks;
		private InterferenceGraph interference;

		public SsaLivenessAnalysis2(Procedure proc, SsaIdentifierCollection ssa)
		{
			this.ssa = ssa;
			visitedBlocks = new Hashtable();
			interference = new InterferenceGraph();

		}
		public void Analyze()
		{
			foreach (SsaIdentifier sid in ssa)
			{
				WorkList stms = new WorkList();
				foreach (Statement use in sid.uses)
				{
					Block p = PrecedingPhiBlock(sid.id, use);
					if (p != null)
					{
						LiveOutAtBlock(p, sid);
					}
					else
					{

						LiveInAtStatement(use.block, use.block.Statements.IndexOf(use), sid);
					}
				}
			}
		}

		public InterferenceGraph InterferenceGraph
		{
			get { return interference; }
		}

		private void LiveOutAtBlock(Block n, SsaIdentifier sid)
		{
			if (!visitedBlocks.Contains(n))
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

		public void LiveOutAtStatement(Block block, int iStm, SsaIdentifier sid)
		{
			Hashtable W = iStm >= 0 
				? VariablesDefinedByStatement(block.Statements[iStm])
				: new Hashtable();

			foreach (SsaIdentifier w in W.Values)
			{
				if (w != sid)
					interference.Add(w.id, sid.id);

			}
			if (!W.Contains(sid))
				LiveInAtStatement(block, iStm, sid);
		}

		private Hashtable VariablesDefinedByStatement(Statement stm)
		{
			Hashtable W = new Hashtable();
			foreach (SsaIdentifier sid in ssa)
			{
				if (sid.def == stm)
					W[sid] = sid;
			}
			return W;
		}

		private Block PrecedingPhiBlock(Identifier u, Statement stm)
		{
			PhiAssignment phi = stm.Instruction as PhiAssignment;
			if (phi == null)
				return null;
			for (int i = 0; i < phi.Src.Arguments.Length; ++i)
			{
				if (u == phi.Src.Arguments[i])
					return stm.block.Pred[i];
			}
			return null;
		}
	}
}

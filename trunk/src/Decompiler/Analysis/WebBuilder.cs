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
using System.IO;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Builds webs out of the unions of phi functions. Each
	/// web will correspond to a local variable in the finished 
	/// decompilation. After this
	/// </summary>
	public class WebBuilder
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;
		private SsaLivenessAnalysis sla;
		private DominatorGraph doms;
		private InductionVariableCollection ivs;
		private Web [] webOf;
		private ArrayList webs = new ArrayList();
		private Statement stmCur;
		
		public WebBuilder(Procedure proc, SsaIdentifierCollection ssaIds, InductionVariableCollection ivs)
		{
			this.proc = proc;
			this.ssaIds = ssaIds;
			this.ivs = ivs;
			this.sla = new SsaLivenessAnalysis(proc, ssaIds);
			this.doms = new DominatorGraph(proc);
		}


		private void BuildWebOf()
		{
			this.webOf = new Web[ssaIds.Count];
			foreach (SsaIdentifier sid in ssaIds)
			{
				Web w = new Web();
				w.Add(sid);
				webOf[sid.id.Number] = w;
				webs.Add(w);
			}
		}

		public void InsertDeclarations()
		{
			DeclarationInserter deci = new DeclarationInserter(ssaIds, doms);
			foreach (Web web in this.webs)
			{
				if (!(web.id is MemoryIdentifier))
					deci.InsertDeclaration(web);
			}
		}

		public void Transform()
		{
			new LiveCopyInserter(proc, ssaIds).Transform();
			BuildWebOf();

			foreach (SsaIdentifier id in ssaIds)
			{
				if (id.def != null && !(id.id is MemoryIdentifier))
					VisitStatement(id.def);
			}

			InsertDeclarations();

			WebReplacer replacer = new WebReplacer(this);
			foreach (Block bl in proc.RpoBlocks)
			{
				for (int i = bl.Statements.Count - 1; i >= 0; --i)
				{
					Statement stm = bl.Statements[i];
					stm.Instruction = stm.Instruction.Accept(replacer);
					if (stm.Instruction == null)
					{
						bl.Statements.RemoveAt(i);
					}
				}
			}

			foreach (Web w in webs)
			{
				if (w.InductionVariable != null)
				{
					ivs.Add(proc, w.id, w.InductionVariable);
				}
			}
		}

		private void Merge(Web a, Web b)
		{
			Web c = new Web();
			foreach (SsaIdentifier sid in a.Members)
			{
				c.Add(sid);
				webOf[sid.id.Number] = c;
				foreach (Statement u in a.uses)
					if (!c.uses.Contains(u))
						c.uses.Add(u);
			}
			foreach (SsaIdentifier sid in b.Members)
			{
				c.Add(sid);
				webOf[sid.id.Number] = c;
				foreach (Statement u in b.uses)
					if (!c.uses.Contains(u))
						c.uses.Add(u);
			}
			webs.Remove(a);
			webs.Remove(b);
			webs.Add(c);
		}

		public void VisitPhiAssignment(PhiAssignment p)
		{
			Identifier idDst = (Identifier) p.Dst;
			PhiFunction phi = p.Src;
			for (int i = 0; i < phi.Arguments.Length; ++i)
			{
				Identifier id = (Identifier) phi.Arguments[i];
				Block pred = stmCur.block.Pred[i];
				if (id != idDst)
					Merge(webOf[idDst.Number], webOf[id.Number]);
			}
		}


		public void VisitStatement(Statement stm)
		{
			stmCur = stm;
			PhiAssignment phi = stm.Instruction as PhiAssignment;
			if (phi != null)
				VisitPhiAssignment(phi);
		}

		public Web WebOf(Identifier id)
		{
			return webOf[id.Number];
		}

		public void Write(TextWriter writer)
		{
			foreach (SsaIdentifier sid in ssaIds)
			{
				WebOf(sid.id).Write(writer);
			}
		}

		private class WebReplacer : InstructionTransformer
		{
			private WebBuilder bld;

			public WebReplacer(WebBuilder bld)
			{
				this.bld = bld;
			}

			public override Expression TransformIdentifier(Identifier id)
			{
				return bld.webOf[id.Number].id;
			}

			public override Instruction TransformAssignment(Assignment a)
			{
				a.Dst = (Identifier) a.Dst.Accept(this);
				a.Src = a.Src.Accept(this);
				Identifier idDst = a.Dst as Identifier;
				Identifier idSrc = a.Src as Identifier;
				if (idDst != null && idSrc == idDst)
					return null;
				else 
					return a;
			}

		}

	}
}

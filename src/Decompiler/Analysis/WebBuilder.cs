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
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Analysis;

/// <summary>
/// Builds webs out of the unions of phi functions. Each
/// web will correspond to a local variable in the finished 
/// decompilation. 
/// </summary>
/// <remarks>
/// After this pass, the code is no longer in SSA
/// form, so all analyses should be done prior to applying this stage.
/// </remarks>
public class WebBuilder
{
    private readonly AnalysisContext context;
    private readonly Dictionary<Identifier, LinearInductionVariable> ivs;

    public WebBuilder(
        AnalysisContext context,
        Dictionary<Identifier, LinearInductionVariable> ivs)
    {
        this.context = context;
        this.ivs = ivs;
    }

    public string Id => "webb";

    public string Description => "Joins connected SSA 'net', forming webs";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        var program = context.Program;
        var worker = new Worker(program, ssa, ivs, context.EventListener);
        worker.Transform();
        return (ssa, true);
    }

    public class Worker
	{
        private readonly IReadOnlyProgram program;
		private readonly SsaState ssa;
		private readonly SsaIdentifierCollection ssaIds;
        private readonly IEventListener listener;
		private readonly BlockDominatorGraph doms;
		private readonly Dictionary<Identifier,LinearInductionVariable> ivs;
		private readonly Dictionary<Identifier, Web> webOf;
		private readonly List<Web> webs;

        public Worker(
            IReadOnlyProgram program, 
            SsaState ssa, 
            Dictionary<Identifier,LinearInductionVariable> ivs, 
            IEventListener listener)
        {
            this.program = program;
            this.ssa = ssa;
            this.ssaIds = ssa.Identifiers;
            this.ivs = ivs;
            this.listener = listener;
            this.doms = ssa.Procedure.CreateBlockDominatorGraph();
            this.webs = new List<Web>();
            this.webOf = new Dictionary<Identifier, Web>();
        }

        private void BuildWebOf()
		{
			foreach (SsaIdentifier sid in ssaIds)
			{
				var w = new Web();
				w.Add(sid);
				webOf[sid.Identifier] = w;
				webs.Add(w);
			}
		}

		public void Transform()
		{
            try
            {
                new LiveCopyInserter(ssa).Transform();
                BuildWebOf();

                foreach (SsaIdentifier id in ssaIds)
                {
                    if (id.DefStatement is not null && id.Identifier.Storage is not MemoryStorage)
                        VisitStatement(id.DefStatement);
                }

                // InsertDeclarations();

                var replacer = new WebReplacer(this);
                foreach (Block bl in ssa.Procedure.ControlGraph.Blocks)
                {
                    for (int i = bl.Statements.Count - 1; i >= 0; --i)
                    {
                        Statement stm = bl.Statements[i];
                        stm.Instruction = stm.Instruction.Accept(replacer);
                        if (stm.Instruction is null)
                        {
                            bl.Statements.RemoveAt(i);
                        }
                    }
                }

                foreach (Web w in webs)
                {
                    if (w.InductionVariable is not null)
                    {
                        ivs.Add(w.Identifier!, w.InductionVariable );
                    }
                }
            }
            catch (Exception ex)
            {
                listener.Error(
                    listener.CreateProcedureNavigator(program, ssa.Procedure),
                    ex,
                    "An error occurred while renaming variables.");
            }
		}

		private void Merge(Web a, Web b)
		{
			var c = new Web();
			foreach (SsaIdentifier sid in a.Members)
			{
				c.Add(sid);
				webOf[sid.Identifier] = c;
				foreach (Statement u in a.Uses)
					if (!c.Uses.Contains(u))
						c.Uses.Add(u);
			}
			foreach (SsaIdentifier sid in b.Members)
			{
				c.Add(sid);
				webOf[sid.Identifier] = c;
				foreach (Statement u in b.Uses)
					if (!c.Uses.Contains(u))
						c.Uses.Add(u);
			}
			webs.Remove(a);
			webs.Remove(b);
			webs.Add(c);
		}

		public void VisitPhiAssignment(PhiAssignment p)
		{
			Identifier idDst = p.Dst;
			PhiFunction phi = p.Src;
			foreach (var de in phi.Arguments)
			{
                if (de.Value is Identifier id && id != idDst)
                    Merge(webOf[idDst], webOf[id]);
            }
		}

		public void VisitStatement(Statement stm)
		{
            if (stm.Instruction is PhiAssignment phi)
                VisitPhiAssignment(phi);
        }

		public Web WebOf(Identifier id)
		{
			return webOf[id];
		}

		public void Write(TextWriter writer)
		{
			foreach (SsaIdentifier sid in ssaIds)
			{
				WebOf(sid.Identifier).Write(writer);
			}
		}

		private class WebReplacer : InstructionTransformer
		{
			private readonly Worker bld;

			public WebReplacer(Worker bld)
			{
				this.bld = bld;
			}

			public override Expression VisitIdentifier(Identifier id)
			{
				return bld.webOf[id].Identifier!;
			}

			public override Instruction TransformAssignment(Assignment a)
			{
				a.Dst = (Identifier) a.Dst.Accept(this);
				a.Src = a.Src.Accept(this);
                Identifier idDst = a.Dst;
                if (a.Src is Identifier idSrc && idSrc == idDst)
                    return null!;
                else
                    return a;
            }
		}

        public class OutDefinitionFinder : InstructionVisitorBase
        {
            private readonly Identifier id;
            private bool usedAsOutArgument;

            public static bool Find(IEnumerable<SsaIdentifier> sids)
            {
                foreach (var sid in sids)
                {
                    if (sid.DefStatement is not null)
                    {
                        var odf = new OutDefinitionFinder(sid.Identifier);
                        sid.DefStatement.Instruction.Accept(odf);
                        if (odf.usedAsOutArgument)
                            return true;
                    }
                }
                return false;
            }

            public OutDefinitionFinder(Identifier id)
            {
                this.id = id;
            }

            public override void VisitOutArgument(OutArgument outArg)
            {
                usedAsOutArgument |= (outArg.Expression is Identifier id && this.id == id);
            }
        }
	}
}

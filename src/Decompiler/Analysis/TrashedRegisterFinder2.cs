#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// This class is used to find registers that are modified or preserved by
    /// a procedure.
    /// </summary>
    public class TrashedRegisterFinder2 
    {
        private IProcessorArchitecture arch;
        private ProgramDataFlow progFlow;
        private SsaTransform ssa;
        private DecompilerEventListener decompilerEventListener;
        private ProcedureFlow2 flow;
        private ISet<SsaTransform> sccGroup;
        private Dictionary<Procedure, HashSet<Storage>> assumedPreserved;

        public enum Effect
        {
            Preserved,
            Trashed,
            Constant,
            Copy,
        }

        public TrashedRegisterFinder2(
            IProcessorArchitecture arch,
            ProgramDataFlow flow,
            IEnumerable<SsaTransform> sccGroup,
            DecompilerEventListener listener)
        {
            this.arch = arch;
            this.progFlow = flow;
            this.sccGroup = sccGroup.ToHashSet();
            this.assumedPreserved = sccGroup.ToDictionary(k => k.Procedure, v => new HashSet<Storage>());
            this.decompilerEventListener = listener;
        }

        public ProcedureFlow2 Compute(SsaTransform ssa)
        {
            this.ssa = ssa;
            this.flow = new ProcedureFlow2();
            this.progFlow.ProcedureFlows2.Add(ssa.Procedure, flow);

            foreach (var sid in ssa.Procedure.ExitBlock.Statements
                .Select(s => s.Instruction as UseInstruction)
                .Where(u => u != null)
                .Select(u => ssa.SsaState.Identifiers[(Identifier) u.Expression]))
            {
                CategorizeIdentifier(sid, new HashSet<PhiAssignment>());
            }
            return flow;
        }

        private Tuple<Effect, Expression> CategorizeIdentifier(SsaIdentifier sid, ISet<PhiAssignment> activePhis)
        {
            var c = GetIdentifierCategory(sid, activePhis);
            switch (c.Item1)
            {
            case Effect.Preserved:
                flow.Preserved.Add(sid.OriginalIdentifier.Storage);
                break;
            case Effect.Trashed:
                flow.Trashed.Add(sid.OriginalIdentifier.Storage);
                break;
            case Effect.Constant:
                flow.Constants.Add(sid.OriginalIdentifier.Storage, (Constant) c.Item2);
                flow.Trashed.Add(sid.OriginalIdentifier.Storage);
                break;
            }
            return c;
        }

        private Tuple<Effect, Expression> GetIdentifierCategory(SsaIdentifier sid, ISet<PhiAssignment> activePhis)
        {
            var sidOrig = sid;
            while (true)
            {
                var defInstr = sid.DefStatement.Instruction;
                if (defInstr is DefInstruction &&
                    sid.DefStatement.Block == ssa.Procedure.EntryBlock)
                {
                    // Reaching definition was a DefInstruction;
                    return (sid == sidOrig) ? Preserve() : Trash();
                }
                Assignment ass;
                if (defInstr.As(out ass))
                {
                    var c = VisitAssignment(ass, sid);
                    if (c.Item1 != Effect.Copy)
                        return c;
                    sid = ssa.SsaState.Identifiers[(Identifier)c.Item2];
                }
                CallInstruction call;
                if (defInstr.As(out call))
                {
                    return VisitCall(call, sid);
                }
                PhiAssignment phi;
                if (defInstr.As(out phi))
                {
                    Effect eff = Effect.Preserved;
                    activePhis.Add(phi);
                    foreach (var id in phi.Src.Arguments.OfType<Identifier>())
                    {
                        var c = GetIdentifierCategory(ssa.SsaState.Identifiers[id], activePhis);
                        if (c.Item1 != Effect.Preserved)
                            eff = Effect.Trashed;
                    }
                    activePhis.Remove(phi);
                    return new Tuple<Effect, Expression>(eff, null);
                }
            }
        }

        private static Tuple<Effect, Expression> Trash()
        {
            return new Tuple<Effect, Expression>(Effect.Trashed, null);
        }

        private static Tuple<Effect, Expression> Preserve()
        {
            return new Tuple<Effect, Expression>(Effect.Preserved, null);
        }

        private Tuple<Effect, Expression> VisitAssignment(Assignment ass, SsaIdentifier sid)
        {
            if ((ass.Src == sid.OriginalIdentifier)
                ||
               (sid.OriginalIdentifier.Storage == arch.StackRegister &&
                ass.Src == ssa.Procedure.Frame.FramePointer))
            {
                return new Tuple<Effect, Expression>(Effect.Preserved, null);
            }
            Constant c;
            if (ass.Src.As(out c))
            {
                return new Tuple<Effect, Expression>(Effect.Constant, c);
            }
            Identifier id;
            if (ass.Src.As(out id))
            {
                return new Tuple<Effect, Expression>(Effect.Copy, id);
            }
            return new Tuple<Effect, Expression>(Effect.Trashed, id);
        }

        public Tuple<Effect, Expression> VisitCall(CallInstruction call, SsaIdentifier sid)
        {
            if (!call.Definitions.Any(d => d.Expression == sid.Identifier))
                throw new InvalidOperationException("Call should have defined identifier.");
            ProcedureConstant callee;
            if (!call.Callee.As(out callee))
            {
                throw new NotImplementedException("Indirect calls not handled yet.");
            }

            // Call trashes this identifier. If it's not in our SCC group we
            // know it to be trashed for sure.
            if (!sccGroup.Any(s => s.Procedure == callee.Procedure ))
            {
                return new Tuple<Effect, Expression>(Effect.Trashed, null);
            }

            // Assume that it preserves it.

            throw new NotImplementedException();
            // assumedPreserved[proc].Add(sid.OriginalIdentifier.Storage);
        }
    }
}

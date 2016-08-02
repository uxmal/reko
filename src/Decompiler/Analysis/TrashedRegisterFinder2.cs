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
    /// <remarks>
    /// Assumes that value propagation has been carried out beforehand. 
    /// This means that </remarks>
    public class TrashedRegisterFinder2 
    {
        private IProcessorArchitecture arch;
        private ProgramDataFlow progFlow;
        private SsaState ssa;
        private DecompilerEventListener decompilerEventListener;
        private ProcedureFlow2 flow;
        private ISet<SsaTransform2> sccGroup;
        private Dictionary<Procedure, HashSet<Storage>> assumedPreserved;
        private ExpressionValueComparer cmp;
        private HashSet<PhiAssignment> activePhis;

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
            IEnumerable<SsaTransform2> sccGroup,
            DecompilerEventListener listener)
        {
            this.arch = arch;
            this.progFlow = flow;
            this.sccGroup = sccGroup.ToHashSet();
            this.assumedPreserved = sccGroup.ToDictionary(k => k.SsaState.Procedure, v => new HashSet<Storage>());
            this.decompilerEventListener = listener;
            this.cmp = new ExpressionValueComparer();
        }

        public ProcedureFlow2 Compute(SsaState ssa)
        {
            this.ssa = ssa;
            this.flow = new ProcedureFlow2();
            this.progFlow.ProcedureFlows2.Add(ssa.Procedure, flow);
            this.activePhis = new HashSet<PhiAssignment>();
            foreach (var sid in ssa.Procedure.ExitBlock.Statements
                .Select(s => s.Instruction as UseInstruction)
                .Where(u => u != null && u.Expression is Identifier)    //$TODO: fix is Identifier
                .Select(u=>ssa.Identifiers[(Identifier) u.Expression]))
            {
                CategorizeIdentifier(sid);
            }
            return flow;
        }

        private void CategorizeIdentifier(SsaIdentifier sid)
        {
            var e = GetReachingExpression(sid, activePhis);
            Identifier id;
            Constant c;
            if (e.As(out id))
            {
                if (id == sid.OriginalIdentifier
                    ||
                    id == ssa.Procedure.Frame.FramePointer)
                {
                    flow.Preserved.Add(sid.OriginalIdentifier.Storage);
                    return;
                }
                // Fall through to trash case.
            }
            else if (e.As(out c) && c.IsValid)
            {
                flow.Constants.Add(sid.OriginalIdentifier.Storage, c);
                // Fall through to trash case.
            }
            flow.Trashed.Add(sid.OriginalIdentifier.Storage);
        }

        /// <summary>
        /// Looks "backwards" for the uniquely defining expression.
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="activePhis"></param>
        /// <returns></returns>
        private Expression GetReachingExpression(SsaIdentifier sid, ISet<PhiAssignment> activePhis)
        {
            var sidOrig = sid;
            var defInstr = sid.DefStatement.Instruction;
            if (defInstr is DefInstruction &&
                sid.DefStatement.Block == ssa.Procedure.EntryBlock)
            {
                // Reaching definition was a DefInstruction;
                return sid.Identifier;
            }
            Assignment ass;
            if (defInstr.As(out ass))
            {
                var src = VisitAssignment(ass, sid);
                return src;
            }
            CallInstruction call;
            if (defInstr.As(out call))
            {
                return VisitCall(call, sid);
            }
            PhiAssignment phi;
            if (defInstr.As(out phi))
            {
                Expression value = null;
                activePhis.Add(phi);
                foreach (var id in phi.Src.Arguments.OfType<Identifier>())
                {
                    var c = GetReachingExpression(ssa.Identifiers[id], activePhis);
                    if (value == null)
                    {
                        value = c;
                    }
                    else if (c == Constant.Invalid || !cmp.Equals(value, c))
                    {
                        value = Constant.Invalid;
                    }
                }
                activePhis.Remove(phi);
                return value;
            }
            return Constant.Invalid;
        }

        private static Tuple<Effect, Expression> Trash()
        {
            return new Tuple<Effect, Expression>(Effect.Trashed, null);
        }

        private static Tuple<Effect, Expression> Preserve()
        {
            return new Tuple<Effect, Expression>(Effect.Preserved, null);
        }

        private Expression VisitAssignment(Assignment ass, SsaIdentifier sid)
        {
            Constant c;
            if (ass.Src.As(out c))
            {
                return c;
            }
            Identifier idCopy;
            if (ass.Src.As(out idCopy))
            {
                sid = ssa.Identifiers[idCopy];
                return GetReachingExpression(sid, activePhis);
            }
            return Constant.Invalid;
        }

        public Expression VisitCall(CallInstruction call, SsaIdentifier sid)
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
            if (!sccGroup.Any(s => s.SsaState.Procedure == callee.Procedure))
            {
                return Constant.Invalid;
            }

            // Are we already assuming that sid is preserved? If so, continue.
            HashSet<Storage> preserved;
            var stg = sid.Identifier.Storage;
            if (this.assumedPreserved.TryGetValue(ssa.Procedure, out preserved) &&
                preserved.Contains(sid.Identifier.Storage))
            {
                var sidBeforeCall = GetIdentifierFor(stg, call.Uses);
                var exBeforeCall = GetReachingExpression(sidBeforeCall,
                    new HashSet<PhiAssignment>());
                return exBeforeCall;
            }

            // Assume that it preserves it.
            preserved.Add(stg);
            var sidUse = GetIdentifierFor(stg, call.Uses);
            var ex = GetReachingExpression(sidUse, new HashSet<PhiAssignment>());
            preserved.Remove(stg);
            return ex;
        }

        /// <summary>
        /// Find a use that uses the specified storage location <paramref name="stg"/>.
        /// </summary>
        /// <param name="stg"></param>
        /// <param name="uses"></param>
        /// <returns></returns>
        private SsaIdentifier GetIdentifierFor(Storage stg, HashSet<UseInstruction> uses)
        {
            foreach (var use in uses)
            {
                Identifier id;
                if (!use.Expression.As(out id))
                    continue;
                if (id.Storage == stg)
                    return ssa.Identifiers[id];
            }
            return null;
        }
    }
}

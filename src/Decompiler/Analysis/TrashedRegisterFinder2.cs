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
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private DecompilerEventListener listener;
        private ProcedureFlow flow;
        private ISet<SsaTransform> sccGroup;
        private Dictionary<Procedure, HashSet<Storage>> assumedPreserved;
        private ExpressionValueComparer cmp;
        private ExpressionEmitter m;
        private HashSet<PhiAssignment> activePhis;
        private ExpressionSimplifier simpl;

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
            this.assumedPreserved = sccGroup.ToDictionary(k => k.SsaState.Procedure, v => new HashSet<Storage>());
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
        }

        /// <summary>
        /// Given an SsaState of a Procedure, computes the summary data flow
        /// information for the procedure.
        /// </summary>
        /// <remarks>
        /// Expects that the SSA transform has injected UseInstructions in 
        /// the exit block.
        /// </remarks>
        /// <param name="ssa"></param>
        /// <returns></returns>
        public ProcedureFlow Compute(SsaState ssa)
        {
            this.ssa = ssa;
            this.simpl = new ExpressionSimplifier(
                new SsaEvaluationContext(arch, ssa.Identifiers),
                listener);
            this.flow = this.progFlow.ProcedureFlows[ssa.Procedure];

            if (ssa.Procedure.ExitBlock.Pred.Count == 0)
            {
                // We have proved that no code reaches the exit block; this
                // procedure is "no-return". The net effect is callers see 
                // no trashed registers because control never returns to 
                // them.
                this.flow.TerminatesProcess = true;
            }
            else if (ssa.Procedure.Signature.ParametersValid)
            {
                //$REVIEW: do we need this? if a procedure has a signature,
                // we will always trust that rather than the flow.
                var sig = ssa.Procedure.Signature;
                if (!sig.HasVoidReturn)
                {
                    this.flow.Trashed.Add(sig.ReturnValue.Storage);
                }
                foreach (var stg in sig.Parameters
                    .Select(p => p.Storage)
                    .OfType<OutArgumentStorage>())
                {
                    this.flow.Trashed.Add(stg.OriginalIdentifier.Storage);
                }
            }
            else
            {
                this.activePhis = new HashSet<PhiAssignment>();
                var sids = GetSsaIdentifiersInExitBlock(ssa);
                foreach (var sid in sids)
                {
                    CategorizeIdentifier(sid);
                }
            }
            return this.flow;
        }

        /// <summary>
        /// Get all SSA ids used in the exit block.
        /// </summary>
        /// <param name="ssa"></param>
        /// <returns></returns>
        private static List<SsaIdentifier> GetSsaIdentifiersInExitBlock(SsaState ssa)
        {
            return ssa.Procedure.ExitBlock.Statements
                .Select(s => s.Instruction as UseInstruction)
                .Where(u => u != null)
                .SelectMany(u =>
                    ExpressionIdentifierUseFinder.Find(ssa.Identifiers, u.Expression)
                    .Select(id => ssa.Identifiers[id]))
                .ToList();
        }

        private void CategorizeIdentifier(SsaIdentifier sid)
        {
            var e = GetReachingExpression(sid, activePhis);
            Identifier id;
            Constant c;
            if (e.Item1.As(out id))
            {
                if (id == sid.OriginalIdentifier ||
                    id == ssa.Procedure.Frame.FramePointer)
                {
                    flow.Preserved.Add(sid.OriginalIdentifier.Storage);
                    return;
                }
                // Fall through to trash case.
            }
            else if (e.Item1.As(out c) && c.IsValid)
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
        private Tuple<Expression,SsaIdentifier> GetReachingExpression(SsaIdentifier sid, ISet<PhiAssignment> activePhis)
        {
            var sidOrig = sid;
            if (sid.DefStatement == null)
            {
                return new Tuple<Expression, SsaIdentifier>(Constant.Invalid, sid);
            }
            var defInstr = sid.DefStatement.Instruction;
            if (defInstr is DefInstruction &&
                sid.DefStatement.Block == ssa.Procedure.EntryBlock)
            {
                // Reaching definition was a DefInstruction;
                return new Tuple<Expression, SsaIdentifier>(sid.Identifier, sid);
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
                return VisitPhi(activePhis, phi, sid);
            }
            return new Tuple<Expression, SsaIdentifier>(Constant.Invalid, sid);
        }

        private Tuple<Expression, SsaIdentifier> VisitPhi(
            ISet<PhiAssignment> activePhis,
            PhiAssignment phi,
            SsaIdentifier sid)
        {
            var inv = new Tuple<Expression, SsaIdentifier>(Constant.Invalid, sid);

            Tuple<Expression, SsaIdentifier> value = null;
            if (activePhis.Contains(phi))
            {
                return new Tuple<Expression,SsaIdentifier>(sid.Identifier, sid);
            }
            activePhis.Add(phi);
            foreach (var id in phi.Src.Arguments.OfType<Identifier>())
            {
                var c = GetReachingExpression(ssa.Identifiers[id], activePhis);
                if (value == null)
                {
                    value = c;
                }
                else if (c.Item1 != sid.Identifier && (c.Item1 == Constant.Invalid || !cmp.Equals(value.Item1, c.Item1)))
                {
                    value = inv;
                }
            }
            activePhis.Remove(phi);
            return value;
        }

        private static Tuple<Effect, Expression> Trash()
        {
            return new Tuple<Effect, Expression>(Effect.Trashed, null);
        }

        private static Tuple<Effect, Expression> Preserve()
        {
            return new Tuple<Effect, Expression>(Effect.Preserved, null);
        }                                             

        public Tuple<Expression,SsaIdentifier> VisitAssignment(Assignment ass, SsaIdentifier sid)
        {
            Constant c;
            if (ass.Src.As(out c))
            {
                return new Tuple<Expression,SsaIdentifier>(c, sid);
            }
            Identifier idCopy;
            if (ass.Src.As(out idCopy))
            {
                sid = ssa.Identifiers[idCopy];
                return GetReachingExpression(sid, activePhis);
            }
            BinaryExpression bin;
            if (ass.Src.As(out bin) && 
                bin.Left.As(out idCopy) &&
                bin.Right.As(out c))
            {
                if (bin.Operator == Operator.IAdd)
                {
                    var sidCopy = ssa.Identifiers[idCopy];
                    var next = GetReachingExpression(sidCopy, activePhis);
                    if (next.Item1 != Constant.Invalid && next.Item2 != sidCopy)
                    {
                        
                    }
                }
            }
            return  new Tuple<Expression, SsaIdentifier>(Constant.Invalid, sid);
        }

        public Tuple<Expression, SsaIdentifier> VisitCall(CallInstruction call, SsaIdentifier sid)
        {
            if (!call.Definitions.Any(d => d.Expression == sid.Identifier))
                throw new InvalidOperationException("Call should have defined identifier.");
            var inv = new Tuple<Expression, SsaIdentifier>(Constant.Invalid, sid);
            ProcedureConstant callee;
            if (!call.Callee.As(out callee))
            {
                // Indirect function call, we don't know whether it's valid across the call.
                return inv;
            }

            // Call defined this identifier. If the callee is not in our SCC group we
            // can rely on the procedureflow information.
            if (!sccGroup.Any(s => s.SsaState.Procedure == callee.Procedure))
            {
                return inv;
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

            // Assume that it preserves it; skip across the call to
            // use the reaching expression.
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
        private SsaIdentifier GetIdentifierFor(Storage stg, HashSet<CallBinding> uses)
        {
            foreach (var use in uses)
            {
                var ids = ExpressionIdentifierUseFinder.Find(ssa.Identifiers, use.Expression);
                foreach (var id in ids)
                {
                    if (id.Storage == stg)
                        return ssa.Identifiers[id];
                }
            }
            return null;
        }

        // for u in uses:
        //   e = u.Expression
        //   s = def(e)
        //   if s is ass
        //      if s.dst is (+,Q,c)
        //          e = (e - c).simplify
        //          e = Q
        //      if s.dst is (-,Q,c):
        //          e = (e + c).simplify
        //          e = Q
        //      if s.dst is ID
        //          e = SRC
        //   if s is call:
        //  
        
        // recursiveQ.clear
        // Q = all_blocks
        // while Q:
        //      b = pop(Q)
        //      state = states[b]
        //      for s in b.statements
        //          if (ass)
        //              state[ass.Dst] = eval(ass.Src, state)
        //          else if (side,branch) 
        //              eval(x.Expression, state)


            
    }
}

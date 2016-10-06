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
        private DecompilerEventListener decompilerEventListener;
        private ProcedureFlow flow;
        private ISet<SsaTransform> sccGroup;
        private Dictionary<Procedure, HashSet<Storage>> assumedPreserved;
        private ExpressionValueComparer cmp;
        private ExpressionSimplifier simp;
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
            this.decompilerEventListener = listener;
            this.cmp = new ExpressionValueComparer();
            this.simpl = new Evaluation.ExpressionSimplifier(new SsaEvaluationContext(arch, ssa.Identifiers));
            foreach (var sst in sccGroup)
            {
                var proc = sst.SsaState.Procedure;
                flow.ProcedureFlows.Add(proc, new ProcedureFlow(proc));
            }
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
            this.flow = this.progFlow.ProcedureFlows[ssa.Procedure];
            if (ssa.Procedure.Signature.ParametersValid)
            {
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
            var inv = new Tuple<Expression, SsaIdentifier>(Constant.Invalid, sid);

            PhiAssignment phi;
            if (defInstr.As(out phi))
            {
                Tuple<Expression,SsaIdentifier> value = null;
                if (activePhis.Contains(phi))
                    return inv;
                activePhis.Add(phi);
                foreach (var id in phi.Src.Arguments.OfType<Identifier>())
                {
                    var c = GetReachingExpression(ssa.Identifiers[id], activePhis);
                    if (value == null)
                    {
                        value = c;
                    }
                    else if (c.Item1 == Constant.Invalid || !cmp.Equals(value.Item1, c.Item1))
                    {
                        value = inv;
                    }
                }
                activePhis.Remove(phi);
                return value;
            }
            return inv;
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
                return inv;
            }

            // Call trashes this identifier. If it's not in our SCC group we
            // know it to be trashed for sure.
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
        private SsaIdentifier GetIdentifierFor(Storage stg, HashSet<CallBinding> uses)
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
    }
}

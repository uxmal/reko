#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Performs propagation by replacing occurences of expressions with 
    /// simpler expressions if these are beneficial. Constants are folded,
    /// and so on.
    /// </summary>
    /// <remarks>
    /// This is a useful transform that doesn't cause too many problems for
    /// later transforms. Calling it will flush out lots of dead expressions
    /// that can be removed with DeadCode.Eliminate()
    /// </remarks>
    public class ValuePropagator : InstructionVisitor<Instruction>
    {
        private static TraceSwitch trace = new TraceSwitch("ValuePropagation", "Traces value propagation");

        private readonly IProcessorArchitecture arch;
        private readonly SsaState ssa;
        private readonly CallGraph callGraph;
        private readonly ExpressionSimplifier eval;
        private readonly SsaEvaluationContext evalCtx;
        private readonly SsaMutator ssam;
        private readonly DecompilerEventListener eventListener;
        private Statement stmCur;

        public ValuePropagator(
            SegmentMap segmentMap,
            SsaState ssa,
            CallGraph callGraph,
            IDynamicLinker dynamicLinker,
            DecompilerEventListener eventListener)
        {
            this.ssa = ssa;
            this.callGraph = callGraph;
            this.arch = ssa.Procedure.Architecture;
            this.eventListener = eventListener;
            this.ssam = new SsaMutator(ssa);
            this.evalCtx = new SsaEvaluationContext(arch, ssa.Identifiers, dynamicLinker);
            this.eval = new ExpressionSimplifier(segmentMap, evalCtx, eventListener);
        }

        public bool Changed { get { return eval.Changed; } set { eval.Changed = value; } }

        public void Transform()
        {
            do
            {
                //$PERFORMANCE: consider changing this to a work list, where 
                // every time we process the 
                Changed = false;
                foreach (Statement stm in ssa.Procedure.Statements.ToArray())
                {
                    if (eventListener.IsCanceled())
                        return;
                    this.stmCur = stm;
                    Transform(stm);
                }
            } while (Changed);
        }

        public void Transform(Statement stm)
        {
            evalCtx.Statement = stm;
            if (trace.TraceVerbose) Debug.WriteLine(string.Format("From: {0}", stm.Instruction.ToString()));
            stm.Instruction = stm.Instruction.Accept(this);
            if (trace.TraceVerbose) Debug.WriteLine(string.Format("  To: {0}", stm.Instruction.ToString()));
        }

        #region InstructionVisitor<Instruction> Members

        public Instruction VisitAssignment(Assignment a)
        {
            a.Src = a.Src.Accept(eval);
            ssa.Identifiers[a.Dst].DefExpression = a.Src;
            return a;
        }

        public Instruction VisitBranch(Branch b)
        {
            b.Condition = b.Condition.Accept(eval);
            return b;
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            var oldCallee = ci.Callee;
            ci.Callee = ci.Callee.Accept(eval);
            if (ci.Callee is ProcedureConstant pc)
            {
                if (pc.Procedure.Signature.ParametersValid)
                {
                    var sig = pc.Procedure.Signature;
                    var chr = pc.Procedure.Characteristics;
                    RewriteCall(stmCur, ci, sig, chr);
                    return stmCur.Instruction;
                }
                if (oldCallee != pc && pc.Procedure is Procedure procCallee)
                {
                    // This was an indirect call, but is now a direct call.
                    // Make sure the call graph knows about the link between
                    // this statement and the callee.
                    callGraph.AddEdge(stmCur, procCallee);
                }
            }
            foreach (var use in ci.Uses)
            {
                use.Expression = use.Expression.Accept(eval);
            }
            foreach (var def in ci.Definitions
                .Where(d => !( d.Expression is Identifier)))
            {
                def.Expression = def.Expression.Accept(eval);
            }
            return ci;
        }

        public Instruction VisitComment(CodeComment comment)
        {
            return comment;
        }

        public Instruction VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
                decl.Expression = decl.Expression.Accept(eval);
            return decl;
        }

        public Instruction VisitDefInstruction(DefInstruction def)
        {
            return def;
        }

        public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            return gotoInstruction;
        }

        public Instruction VisitPhiAssignment(PhiAssignment phi)
        {
            var src = phi.Src.Accept(eval);
            if (src is PhiFunction f)
                return new PhiAssignment(phi.Dst, f);
            else
                return new Assignment(phi.Dst, src);
        }

        public Instruction VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
                ret.Expression = ret.Expression.Accept(eval);
            return ret;
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            var exp = side.Expression.Accept(eval);
            return new SideEffect(exp);
        }

        public Instruction VisitStore(Store store)
        {
            store.Src = store.Src.Accept(eval);
            var idDst = store.Dst as Identifier;
            if (idDst == null || (!(idDst.Storage is OutArgumentStorage)))
            {
                store.Dst = store.Dst.Accept(eval);
            }
            return store;
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            var exp = si.Expression.Accept(eval);
            return new SwitchInstruction(exp, si.Targets);
        }

        public Instruction VisitUseInstruction(UseInstruction u)
        {
            return u;
        }

        #endregion

        private void RewriteCall(
            Statement stm,
            CallInstruction ci,
            FunctionType sig,
            ProcedureCharacteristics chr)
        {
            ssam.AdjustRegisterAfterCall(
                stm,
                ci,
                this.arch.StackRegister,
                sig.StackDelta - ci.CallSite.SizeOfReturnAddressOnStack);
            ssam.AdjustRegisterAfterCall(
                stm,
                ci,
                this.arch.FpuStackRegister,
                -sig.FpuStackDelta);
            var ab = new CallApplicationBuilder(this.ssa, stm, ci, ci.Callee);
            stm.Instruction = ab.CreateInstruction(sig, chr);
            ssam.AdjustSsa(stm, ci);
        }
    }
}

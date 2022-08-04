#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
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
    public class ValuePropagator : InstructionVisitor<(Instruction, bool)>
    {
        private static readonly TraceSwitch trace = new TraceSwitch("ValuePropagation", "Traces value propagation");

        private readonly IProcessorArchitecture arch;
        private readonly SsaState ssa;
        private readonly CallGraph callGraph;
        private readonly IDynamicLinker dynamicLinker;
        private readonly ExpressionSimplifier eval;
        private readonly SsaEvaluationContext evalCtx;
        private readonly SsaMutator ssam;
        private readonly DecompilerEventListener eventListener;
        private Statement? stmCur;      //$REFACTOR: try to make this a context paramter.

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
            this.dynamicLinker = dynamicLinker;
            this.eventListener = eventListener;
            this.ssam = new SsaMutator(ssa);
            this.evalCtx = new SsaEvaluationContext(arch, ssa.Identifiers, dynamicLinker);
            this.eval = new ExpressionSimplifier(segmentMap, evalCtx, eventListener);
        }


        public void Transform()
        {
            bool changed;
            do
            {
                //$PERFORMANCE: consider changing this to a work list, where 
                // every time we process the 
                changed = false;
                foreach (Statement stm in ssa.Procedure.Statements.ToArray())
                {
                    if (eventListener.IsCanceled())
                        return;
                    this.stmCur = stm;
                    changed |= Transform(stm);
                }
            } while (changed);
        }

        public bool Transform(Statement stm)
        {
            bool changed;
            evalCtx.Statement = stm;
            trace.Verbose("From: {0}", stm.Instruction.ToString());
            (stm.Instruction, changed) = stm.Instruction.Accept(this);
            trace.Verbose("  To: {0}", stm.Instruction.ToString());
            return changed;
        }

        #region InstructionVisitor<Instruction> Members

        public (Instruction, bool) VisitAssignment(Assignment a)
        {
            bool changed;
            (a.Src, changed) = a.Src.Accept(eval);
            var (src, changed2) = ReplaceIndirectCallToImport(a.Src);
            a.Src = src;
            ssa.Identifiers[a.Dst].DefExpression = a.Src;
            return (a, changed|changed2);
        }

        public (Instruction, bool) VisitBranch(Branch b)
        {
            bool changed;
            (b.Condition, changed) = b.Condition.Accept(eval);
            return (b, changed);
        }

        public (Instruction, bool) VisitCallInstruction(CallInstruction ci)
        {
            var stmCur = this.stmCur!;
            var oldCallee = ci.Callee;
            bool changed;
            (ci.Callee, changed) = ci.Callee.Accept(eval);
            if (ci.Callee is ProcedureConstant pc)
            {
                if (pc.Procedure.Signature.ParametersValid)
                {
                    var sig = pc.Procedure.Signature;
                    var chr = pc.Procedure.Characteristics;
                    RewriteCall(stmCur, ci, sig, chr);
                    return (stmCur.Instruction, true);
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
                var (e, c) = use.Expression.Accept(eval);
                use.Expression = e;
                changed |= c;
            }
            foreach (var def in ci.Definitions
                .Where(d => d.Expression is not Identifier))
            {
                var (e, c) = def.Expression.Accept(eval);
                def.Expression = e;
                changed |= c;
            }
            return (ci, changed);
        }

        public (Instruction, bool) VisitComment(CodeComment comment)
        {
            return (comment, false);
        }

        public (Instruction, bool) VisitDeclaration(Declaration decl)
        {
            bool changed = false;
            if (decl.Expression != null)
                (decl.Expression, changed) = decl.Expression.Accept(eval);
            return (decl, changed);
        }

        public (Instruction, bool) VisitDefInstruction(DefInstruction def)
        {
            return (def, false);
        }

        public (Instruction, bool) VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            return (gotoInstruction, false);
        }

        public (Instruction, bool) VisitPhiAssignment(PhiAssignment phi)
        {
            var (src, changed) = phi.Src.Accept(eval);
            if (src is PhiFunction f)
                return (new PhiAssignment(phi.Dst, f), changed);
            else
                return (new Assignment(phi.Dst, src), changed);
        }

        public (Instruction, bool) VisitReturnInstruction(ReturnInstruction ret)
        {
            bool changed = false;
            if (ret.Expression != null)
                (ret.Expression, changed) = ret.Expression.Accept(eval);
            return (ret, changed);
        }

        public (Instruction, bool) VisitSideEffect(SideEffect side)
        {
            var (exp, changed) = side.Expression.Accept(eval);
            return (new SideEffect(exp), changed);
        }

        public (Instruction, bool) VisitStore(Store store)
        {
            bool srcChanged;
            bool dstChanged = false;
            (store.Src, srcChanged) = store.Src.Accept(eval);
            if (store.Dst is not Identifier idDst || (idDst.Storage is not OutArgumentStorage))
            {
                (store.Dst, dstChanged) = store.Dst.Accept(eval);
            }
            return (store, srcChanged|dstChanged);
        }

        public (Instruction, bool) VisitSwitchInstruction(SwitchInstruction si)
        {
            var (exp, changed) = si.Expression.Accept(eval);
            return (new SwitchInstruction(exp, si.Targets), changed);
        }

        public (Instruction, bool) VisitUseInstruction(UseInstruction u)
        {
            return (u, false);
        }

        #endregion

        private void RewriteCall(
            Statement stm,
            CallInstruction ci,
            FunctionType sig,
            ProcedureCharacteristics? chr)
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
            ssa.RemoveUses(stm);
            var ab = new CallApplicationBuilder(this.ssa, stm, ci, ci.Callee, true);
            stm.Instruction = ab.CreateInstruction(sig, chr);
            ssam.AdjustSsa(stm, ci);
        }

        private (Expression, bool) ReplaceIndirectCallToImport(Expression e)
        {
            if (dynamicLinker != null && 
                e is MemoryAccess mem && 
                mem.EffectiveAddress is Constant c)
            {
                var stm = evalCtx.Statement!;
                var pc = dynamicLinker.ResolveToImportedValue(stm, c);
                if (pc is not null)
                {
                    Debug.Print("Const: {0} was replaced with ", c, pc);
                    return (pc, true);
                }
            }
            return (e, false);
        }
    }
}

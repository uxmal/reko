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
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Scanning;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis;

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
public class ValuePropagator : IAnalysis<SsaState>
{
    private static readonly TraceSwitch trace = new(nameof(ValuePropagator), "Traces value propagation");

    private readonly AnalysisContext context;

    public ValuePropagator(AnalysisContext context)
    {
        this.context = context;
    }

    public string Id => "vp";

    public string Description => "Propagates values from definitions to uses";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        var worker = new Worker(context, ssa);
        var changed = worker.Transform();
        return (ssa, changed);
    }

    private class Worker : InstructionVisitor<(Instruction, bool)>
    {
        private readonly IReadOnlyProgram program;
        private readonly IProcessorArchitecture arch;
        private readonly SsaState ssa;
        private readonly IReadOnlyCallGraph callGraph;
        private readonly IDynamicLinker dynamicLinker;
        private readonly ExpressionSimplifier eval;
        private readonly SsaEvaluationContext evalCtx;
        private readonly SsaMutator ssam;
        private readonly IEventListener eventListener;
        private readonly VarargsFormatScanner va;
        private Statement stmCur;      //$REFACTOR: try to make this a context paramter.
        private bool changed;

        public Worker(
            AnalysisContext context,
            SsaState ssa)
        {
            this.program = context.Program;
            this.ssa = ssa;
            this.callGraph = program.CallGraph;
            this.arch = ssa.Procedure.Architecture;
            this.dynamicLinker = context.DynamicLinker;
            this.eventListener = context.EventListener;
            this.ssam = new SsaMutator(ssa);
            this.evalCtx = new SsaEvaluationContext(arch, ssa.Identifiers, dynamicLinker);
            this.eval = new ExpressionSimplifier(program.Memory, evalCtx, eventListener);
            var ctx = new SsaEvaluationContext(arch, ssa.Identifiers, dynamicLinker);
            this.va = new VarargsFormatScanner(program, arch, ctx, context.Services, context.EventListener);
            this.stmCur = default!;
        }

        public bool Transform()
        {
            this.changed = false;
            const int MaxIterations = 10_000;

            bool changed;
            int iterations = 0;
            do
            {
                //$PERFORMANCE: consider changing this to a work list, where 
                // every time we process an assignment of the form a = XXX
                // and XXX changes, add all uses of a to the work list.
                changed = false;
                if (++iterations >= MaxIterations)
                {
                    eventListener.Warn(
                        eventListener.CreateProcedureNavigator(program, ssa.Procedure),
                        "Stopping value propagation after iterating {0} times.", iterations);
                    return this.changed;
                }
                foreach (Statement stm in ssa.Procedure.Statements.ToArray())
                {
                    if (eventListener.IsCanceled())
                        return this.changed;
                    this.stmCur = stm;
                    var c = Transform(stm);
                    changed |= c;
                    this.changed |= c;
                }
            } while (changed);
            return this.changed;
        }

        public bool Transform(Statement stm)
        {
            bool changed;
            evalCtx.Statement = stm;
            trace.Verbose("VP: From: {0}", stm.Instruction.ToString());
            (stm.Instruction, changed) = stm.Instruction.Accept(this);
            trace.Verbose("VP:   To: {0}", stm.Instruction.ToString());
            return changed;
        }

        #region InstructionVisitor<Instruction> Members

        public (Instruction, bool) VisitAssignment(Assignment a)
        {
            var (src1, changed) = a.Src.Accept(eval);
            var (src, changed2) = ReplaceIndirectCallToImport(src1);
            if (changed|changed2)
            {
                evalCtx.RemoveExpressionUse(a.Src);
                evalCtx.UseExpression(src);
            }
            a.Src = src;
            return (a, changed|changed2);
        }

        public (Instruction, bool) VisitBranch(Branch b)
        {
            var (cond, changed) = b.Condition.Accept(eval);
            if (changed)
            {
                evalCtx.RemoveExpressionUse(b.Condition);
                evalCtx.UseExpression(cond);
                b.Condition = cond;
            }
            return (b, changed);
        }

        public (Instruction, bool) VisitCallInstruction(CallInstruction ci)
        {
            var stmCur = this.stmCur;
            var oldCallee = ci.Callee;
            bool changed;
            (ci.Callee, changed) = ci.Callee.Accept(eval);
            if (changed)
            {
                evalCtx.RemoveExpressionUse(oldCallee);
                evalCtx.UseExpression(ci.Callee);
            }
            if (ci.Callee is ProcedureConstant pc)
            {
                var sig = pc.Procedure.Signature;
                if (sig.ParametersValid)
                {
                    var chr = pc.Procedure.Characteristics;
                    changed = RewriteCall(stmCur, ci, sig, chr);
                    return (stmCur.Instruction, changed);
                }
                if (oldCallee != pc && pc.Procedure is Procedure procCallee)
                {
                    // This was an indirect call, but is now a direct call.
                    // Make sure the call graph knows about the link between
                    // this statement and the callee.
                    callGraph.AddEdge(stmCur, procCallee);
                }
            } else if (ci.Callee is Identifier id &&
                id == ssa.Procedure.Frame.Continuation)
            {
                return ReplaceCallToContinuationWithReturn();
            }
            foreach (var use in ci.Uses)
            {
                var (e, c) = use.Expression.Accept(eval);
                if (c)
                {
                    evalCtx.RemoveExpressionUse(use.Expression);
                    evalCtx.UseExpression(e);
                }
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

        private (Instruction, bool) ReplaceCallToContinuationWithReturn()
        {
            var stm = stmCur;
            var block = stm.Block;
            var iCall = block.Statements.IndexOf(stm);
            for (int i = block.Statements.Count - 1; i >= iCall; --i)
            {
                ssa.DeleteStatement(block.Statements[i]);
            }
            var ret = new ReturnInstruction();
            block.Statements.Add(stm.Address, ret);
            return (ret, true);
        }

        public (Instruction, bool) VisitComment(CodeComment comment)
        {
            return (comment, false);
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
            if (!changed)
                return (phi, false);
            evalCtx.RemoveExpressionUse(phi.Src);
            if (src is PhiFunction f)
            {
                evalCtx.UseExpression(f);
                return (new PhiAssignment(phi.Dst, f), changed);
            }
            else
            {
                evalCtx.UseExpression(src);
                return (new Assignment(phi.Dst, src), changed);
            }
        }

        public (Instruction, bool) VisitReturnInstruction(ReturnInstruction ret)
        {
            bool changed = false;
            if (ret.Expression is not null)
            {
                Expression e;
                (e, changed) = ret.Expression.Accept(eval);
                if (changed)
                {
                    evalCtx.RemoveExpressionUse(ret.Expression);
                    evalCtx.UseExpression(e);
                    ret.Expression = e;
                }
            }
            return (ret, changed);
        }

        public (Instruction, bool) VisitSideEffect(SideEffect side)
        {
            var (exp, changed) = side.Expression.Accept(eval);
            if (changed)
            {
                evalCtx.RemoveExpressionUse(side.Expression);
                evalCtx.UseExpression(exp);
            }
            return (new SideEffect(exp), changed);
        }

        public (Instruction, bool) VisitStore(Store store)
        {
            bool dstChanged = false;
            var (src, srcChanged) = store.Src.Accept(eval);
            if (srcChanged)
            {
                evalCtx.RemoveExpressionUse(store.Src);
                evalCtx.UseExpression(src);
                store.Src = src;
            }
            if (store.Dst is MemoryAccess mem)
            {
                Expression ea;
                (ea, dstChanged) = mem.EffectiveAddress.Accept(eval);
                if (dstChanged)
                {
                    evalCtx.RemoveExpressionUse(mem.EffectiveAddress);
                    evalCtx.UseExpression(ea);
                    store.Dst = new MemoryAccess(mem.MemoryId, ea, mem.DataType);
                }
            }
            return (store, srcChanged|dstChanged);
        }

        public (Instruction, bool) VisitSwitchInstruction(SwitchInstruction si)
        {
            var (exp, changed) = si.Expression.Accept(eval);
            if (changed)
            {
                evalCtx.RemoveExpressionUse(si.Expression);
                evalCtx.UseExpression(exp);
            }
            return (new SwitchInstruction(exp, si.Targets), changed);
        }

        public (Instruction, bool) VisitUseInstruction(UseInstruction u)
        {
            return (u, false);
        }

        #endregion

        private bool RewriteCall(
            Statement stm,
            CallInstruction ci,
            FunctionType sig,
            ProcedureCharacteristics? chr)
        {
            if (VarargsFormatScanner.IsVariadicParserKnown(sig, chr))
            {
                bool changed = false;
                //$REFACTOR: doing this in VisitCallInstruction already, consider moving those 
                // loops over ci.Uses before special cases.
                foreach (var use in ci.Uses)
                {
                    var (e, c) = use.Expression.Accept(eval);
                    if (c)
                    {
                        evalCtx.RemoveExpressionUse(use.Expression);
                        evalCtx.UseExpression(e);
                        use.Expression = e;
                    }
                    changed |= c;
                }
                var abb = new CallApplicationBuilder(this.ssa, stm, ci, true);
                if (!va.TryScan(stmCur.Address, ci.Callee, sig, chr, abb, out var vaResult))
                    return changed;
                //$TODO: we found a string, record it in globals.
                // We can't do it immediately because we're inside a SCC. So hang 
                // onto the string information and merge it in as a final pass.
                stm.Instruction = va.BuildInstruction(ci.Callee, sig, vaResult.Signature, chr, abb);
                ssam.AdjustSsa(stm, ci);
                return true;
            }
            ssam.AdjustRegisterAfterCall(
                stm,
                ci,
                this.arch.StackRegister,
                sig.StackDelta - ci.CallSite.SizeOfReturnAddressOnStack);
            var fpuStackReg = this.arch.FpuStackRegister;
            if (fpuStackReg is not null)
            {
                ssam.AdjustRegisterAfterCall(
                    stm,
                    ci,
                    fpuStackReg,
                    -sig.FpuStackDelta);
            }
            ssa.RemoveUses(stm);
            var ab = new CallApplicationBuilder(this.ssa, stm, ci, true);
            if (va.TryScan(stmCur.Address, ci.Callee, sig, chr, ab, out var result))
            {
                //$TODO: we found a string, record it in globals.
                // We can't do it immediately because we're inside a SCC. So hang 
                // onto the string information and merge it in as a final pass.
                stm.Instruction = va.BuildInstruction(ci.Callee, sig, result.Signature, chr, ab);
            }
            else
            { 
                stm.Instruction = ab.CreateInstruction(ci.Callee, sig, chr);
            }
            ssam.AdjustSsa(stm, ci);
            return true;
        }

        private (Expression, bool) ReplaceIndirectCallToImport(Expression e)
        {
            if (dynamicLinker is not null && 
                e is MemoryAccess mem && 
                mem.EffectiveAddress is Constant c)
            {
                var stm = evalCtx.Statement!;
                var pc = dynamicLinker.ResolveToImportedValue(stm, c);
                if (pc is not null)
                {
                    trace.Verbose("VP: Const: {0} was replaced with ", c, pc);
                    return (pc, true);
                }
            }
            return (e, false);
        }
    }
}

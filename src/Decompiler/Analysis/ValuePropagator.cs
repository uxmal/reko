#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Graphs;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Scanning;
using Reko.Services;
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
    public class ValuePropagator : InstructionVisitor<(Instruction, bool)>
    {
        private static readonly TraceSwitch trace = new(nameof(ValuePropagator), "Traces value propagation");

        private readonly IReadOnlyProgram program;
        private readonly IProcessorArchitecture arch;
        private readonly SsaState ssa;
        private readonly IReadOnlyCallGraph callGraph;
        private readonly IDynamicLinker dynamicLinker;
        private readonly ExpressionSimplifier eval;
        private readonly SsaEvaluationContext evalCtx;
        private readonly SsaMutator ssam;
        private readonly IDecompilerEventListener eventListener;
        private readonly Scanning.VarargsFormatScanner va;
        private Statement stmCur;      //$REFACTOR: try to make this a context paramter.

        public ValuePropagator(
            IReadOnlyProgram program,
            SsaState ssa,
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
        {
            this.program = program;
            this.ssa = ssa;
            this.callGraph = program.CallGraph;
            this.arch = ssa.Procedure.Architecture;
            this.dynamicLinker = dynamicLinker;
            this.eventListener = services.RequireService<IDecompilerEventListener>();
            this.ssam = new SsaMutator(ssa);
            this.evalCtx = new SsaEvaluationContext(arch, ssa.Identifiers, dynamicLinker);
            this.eval = new ExpressionSimplifier(program.SegmentMap, evalCtx, eventListener);
            var ctx = new SsaEvaluationContext(arch, ssa.Identifiers, dynamicLinker);
            this.va = new Scanning.VarargsFormatScanner(program, arch, ctx, services);
            this.stmCur = default!;
        }

        public void Transform()
        {
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
                    return;
                }
                foreach (Statement stm in ssa.Procedure.Statements.ToArray())
                {
                    if (eventListener.IsCanceled())
                        return;
                    this.stmCur = stm;
                    var c = Transform(stm);
                    changed |= c;
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
            var stmCur = this.stmCur;
            var oldCallee = ci.Callee;
            bool changed;
            (ci.Callee, changed) = ci.Callee.Accept(eval);
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

        /*
        public (Instruction, bool) VisitDeclaration(Declaration decl)
        {
            bool changed = false;
            if (decl.Expression != null)
                (decl.Expression, changed) = decl.Expression.Accept(eval);
            return (decl, changed);
        }
        */

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
                    use.Expression = e;
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

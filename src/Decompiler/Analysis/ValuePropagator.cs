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

using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Performs propagation by replacing occurences of expressions with simpler expressions if these are beneficial. 
    /// Constants are folded, and so on.
    /// </summary>
    /// <remarks>
    /// This is a useful transform that doesn't cause too many problems for later transforms. Calling it will flush out
    /// lots of dead expressions that can be removed with DeadCode.Eliminate()
    /// </remarks>
    public class ValuePropagator : InstructionVisitor<Instruction>
    {
        private static TraceSwitch trace = new TraceSwitch("ValuePropagation", "Traces value propagation");

        private IProcessorArchitecture arch;
        private SsaState ssa;
        private ExpressionSimplifier eval;
        private SsaEvaluationContext evalCtx;
        private SsaIdentifierTransformer ssaIdTransformer;
        private DecompilerEventListener eventListener;

        public ValuePropagator(
            IProcessorArchitecture arch,
            SegmentMap segmentMap,
            SsaState ssa,
            DecompilerEventListener eventListener)
        {
            this.arch = arch;
            this.ssa = ssa;
            this.eventListener = eventListener;
            this.ssaIdTransformer = new SsaIdentifierTransformer(ssa);
            this.evalCtx = new SsaEvaluationContext(arch, ssa.Identifiers);
            this.eval = new ExpressionSimplifier(segmentMap, evalCtx, eventListener);
        }

        public bool Changed { get { return eval.Changed; } set { eval.Changed = value; } }

        public void Transform()
        {
            do
            {
                Changed = false;
                foreach (Statement stm in ssa.Procedure.Statements)
                {
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
            ci.Callee = ci.Callee.Accept(eval);
            if (ci.Callee is ProcedureConstant pc &&
                pc.Procedure.Signature != null &&
                pc.Procedure.Signature.ParametersValid)
            {
                var ab = new ApplicationBuilder(
                      arch, ssa.Procedure.Frame, ci.CallSite,
                      ci.Callee, pc.Procedure.Signature, false);
                evalCtx.Statement.Instruction = ab.CreateInstruction();
                ssaIdTransformer.Transform(evalCtx.Statement, ci);
                return evalCtx.Statement.Instruction;
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
            side.Expression = side.Expression.Accept(eval);
            return side;
        }

        public Instruction VisitStore(Store store)
        {
            store.Src = store.Src.Accept(eval);
            store.Dst = store.Dst.Accept(eval);
            return store;
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression = si.Expression.Accept(eval);
            return si;
        }

        public Instruction VisitUseInstruction(UseInstruction u)
        {
            return u;
        }

        #endregion
    }
}

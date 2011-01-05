#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Evaluation;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;

namespace Decompiler.Analysis
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
        private SsaIdentifierCollection ssaIds;
        private ExpressionSimplifier eval;
        private SsaEvaluationContext evalCtx;
        private Procedure proc;
        private Statement stm;
        private bool changed;

        private static TraceSwitch trace = new TraceSwitch("ValuePropagation", "Traces value propagation");

        public ValuePropagator(SsaIdentifierCollection ssaIds, Procedure proc)
        {
            this.ssaIds = ssaIds;
            this.proc = proc;
            this.evalCtx = new SsaEvaluationContext(ssaIds);
            this.eval = new ExpressionSimplifier(evalCtx);
        }

        public bool Changed { get { return eval.Changed; } set { eval.Changed = value; } }

        public void Transform()
        {
            do
            {
                Changed = false;
                foreach (Block block in proc.RpoBlocks)
                {
                    for (int i = 0; i < block.Statements.Count; ++i)
                    {
                        Transform(block.Statements[i]);
                    }
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
            return a;
        }

        public Instruction VisitBranch(Branch b)
        {
            b.Condition = b.Condition.Accept(eval);
            return b;
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            return ci;
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
            return phi;
        }

        public Instruction VisitIndirectCall(IndirectCall ic)
        {
            ic.Callee = ic.Callee.Accept(eval);
            return ic;
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
            u.Expression = u.Expression.Accept(eval);
            return u;
        }

        #endregion
    }
}

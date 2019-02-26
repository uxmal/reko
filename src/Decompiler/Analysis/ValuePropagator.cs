#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
        private readonly DecompilerEventListener eventListener;
        private Statement stmCur;

        public ValuePropagator(
            SegmentMap segmentMap,
            SsaState ssa,
            CallGraph callGraph,
            IImportResolver importResolver,
            DecompilerEventListener eventListener)
        {
            this.ssa = ssa;
            this.callGraph = callGraph;
            this.arch = ssa.Procedure.Architecture;
            this.eventListener = eventListener;
            this.evalCtx = new SsaEvaluationContext(arch, ssa.Identifiers, importResolver);
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
                    var ab = new CallApplicationBuilder(this.ssa, this.stmCur, ci, ci.Callee);
                    evalCtx.Statement.Instruction = ab.CreateInstruction(pc.Procedure.Signature, pc.Procedure.Characteristics);
                    AdjustSsa(ssa, evalCtx.Statement, ci);
                    return evalCtx.Statement.Instruction;
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

        private void AdjustSsa(SsaState ssa, Statement stm, CallInstruction call)
        {
            ssa.ReplaceDefinitions(stm, null);
            ssa.RemoveUses(stm);
            ssa.AddDefinitions(stm);
            ssa.AddUses(stm);
            var ssam = new SsaMutator(ssa);
            ssam.DefineUninitializedIdentifiers(stm, call);
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
            var idDst = store.Dst as Identifier;
            if (idDst == null || (!(idDst.Storage is OutArgumentStorage)))
            {
                store.Dst = store.Dst.Accept(eval);
            }
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

        private class CallInstructionBinder : IStorageBinder
        {
            private CallInstruction ci;

            public CallInstructionBinder(CallInstruction ci)
            {
                this.ci = ci;
            }

            public Identifier CreateTemporary(DataType dt)
            {
                throw new NotImplementedException();
            }

            public Identifier CreateTemporary(string name, DataType dt)
            {
                throw new NotImplementedException();
            }

            public Identifier EnsureFlagGroup(FlagGroupStorage grf)
            {
                throw new NotImplementedException();
            }

            public Identifier EnsureFlagGroup(RegisterStorage flagRegister, uint flagGroupBits, string name, DataType dataType)
            {
                throw new NotImplementedException();
            }

            public Identifier EnsureFpuStackVariable(int v, DataType dataType)
            {
                throw new NotImplementedException();
            }

            public Identifier EnsureIdentifier(Storage stg)
            {
                // If we can't find the storage, we're in trouble.
                // it could mean that no statement in this fn defined it,
                // so we probably should create it as a live-in identifier.
                return ci.Uses
                    .Select(u => u.Expression)
                    .OfType<Identifier>()
                    .Where(i => i.Storage == stg)
                    .First();
            }

            public Identifier EnsureOutArgument(Identifier idOrig, DataType outArgumentPointer)
            {
                throw new NotImplementedException();
            }

            public Identifier EnsureRegister(RegisterStorage reg)
            {
                return ci.Uses
                   .Select(u => u.Expression)
                   .OfType<Identifier>()
                   .Where(i => i.Storage.Name == reg.Name)
                   .First();
            }

            public Identifier EnsureSequence(DataType dt, Storage head, Storage tail)
            {
                throw new NotImplementedException();
            }

            public Identifier EnsureSequence(DataType dt, string name, Storage head, Storage tail)
            {
                throw new NotImplementedException();
            }

            public Identifier EnsureStackVariable(int v, DataType dataType)
            {
                throw new NotImplementedException();
            }
        }

    }
}

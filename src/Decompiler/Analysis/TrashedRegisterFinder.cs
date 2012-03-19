#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using Decompiler.Typing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Uses an interprocedural reaching definition analysis to detect which 
	/// registers are modified by the procedures, which registers are constant at block
    /// exits, and which registers have their values preserved. A useful side effect is the
    /// tentative typing of procedure frames and global function pointers.<para>
    /// The results of the analysis are stored in the ProgramDataFlow.</para>
	/// </summary>
    public class TrashedRegisterFinder : InstructionVisitorBase
    {
        private Program prog;
        private IEnumerable<Procedure> procedures;
        private ProgramDataFlow flow;
        private BlockFlow bf;
        private WorkList<Block> worklist;
        private SymbolicEvaluator se;
        private SymbolicEvaluationContext ctx;
        private DecompilerEventListener eventListener;
        private ExpressionValueComparer ecomp;

        public TrashedRegisterFinder(
            Program prog,
            IEnumerable<Procedure> procedures,
            ProgramDataFlow flow,
            DecompilerEventListener eventListener)
        {
            this.prog = prog;
            this.procedures = procedures;
            this.flow = flow;
            this.eventListener = eventListener;
            this.worklist = new WorkList<Block>();
            this.ecomp = new ExpressionValueComparer();
        }

        public Dictionary<RegisterStorage, Expression> RegisterSymbolicValues { get { return ctx.RegisterState; } }
        public IDictionary<int, Expression> StackSymbolicValues { get { return ctx.StackState; } } 
        public uint TrashedFlags {get { return ctx.TrashedFlags; } }

        public void CompleteWork()
        {
            foreach (Procedure proc in procedures)
            {
                var pf = flow[proc];
                foreach (int r in pf.TrashedRegisters)
                {
                    prog.Architecture.GetRegister(r).SetAliases(pf.TrashedRegisters, true);
                }
            }
        }

        public void Compute()
        {
            FillWorklist();
            ProcessWorkList();
            CompleteWork();
        }

        public void RewriteBasicBlocks()
        {
            foreach (var proc in procedures)
            {
                foreach (var block in proc.ControlGraph.Blocks)
                {
                    RewriteBlock(block);
                }
            }
        }

        public void ProcessWorkList()
        {

            int initial = worklist.Count;
            Block block;
            while (worklist.GetWorkItem(out block))
            {
                var e = eventListener;
                if (e != null)
                    eventListener.ShowProgress(string.Format("Blocks left: {0}", worklist.Count), initial - worklist.Count, initial);
                ProcessBlock(block);
            }
        }

        public void FillWorklist()
        {
            foreach (Procedure proc in procedures)
            {
                foreach (Block block in proc.ControlGraph.Blocks)
                {
                    worklist.Add(block);
                }
            }
        }

        public bool IsTrashed(Storage storage)
        {
            var reg = storage as RegisterStorage;
            if (reg == null)
                throw new NotImplementedException();

            Expression regVal;
            if (!ctx.RegisterState.TryGetValue(reg, out regVal))
                return false;
            var id = regVal as Identifier;
            if (id == null)
                return true;
            return id.Storage == reg;
        }

        public void ProcessBlock(Block block)
        {
            StartProcessingBlock(block);
            block.Statements.ForEach(stm => stm.Instruction.Accept(this));
            
            if (block == block.Procedure.ExitBlock)
            {
                PropagateToProcedureSummary(block.Procedure);
            }
            else
            {
                block.Succ.ForEach(s => PropagateToSuccessorBlock(s));
            }
        }

        public void RewriteBlock(Block block)
        {
            StartProcessingBlock(block);
            var updater = new Updater(se);
            block.Statements.ForEach(stm => stm.Instruction.Accept(updater));
        }

        public void StartProcessingBlock(Block block)
        {
            bf = flow[block];
            EnsureEvaluationContext(bf);
            if (block.Procedure.EntryBlock == block)
            {
                var sp = block.Procedure.Frame.EnsureRegister(prog.Architecture.StackRegister);
                bf.SymbolicIn[sp.Storage] = block.Procedure.Frame.FramePointer;
            }
            ctx.TrashedFlags = bf.grfTrashedIn;
        }

        public void EnsureEvaluationContext(BlockFlow bf)
        {
            if (bf.SymbolicAuxIn == null)
            {
                bf.SymbolicAuxIn = new SymbolicEvaluationContext(prog.Architecture);
            }
            this.ctx = bf.SymbolicAuxIn;
            this.se = new SymbolicEvaluator(new TrashedExpressionSimplifier(this, ctx), ctx);
        }

        public void PropagateToProcedureSummary(Procedure proc)
        {
            bool changed = false;
            var pf = flow[proc];
            var tr = prog.Architecture.CreateRegisterBitset();
            var pr = prog.Architecture.CreateRegisterBitset();
            if (pf.TerminatesProcess)
            {
                if (!pf.TrashedRegisters.IsEmpty)
                {
                    changed = true;
                    pf.TrashedRegisters.SetAll(false);
                }
                if (pf.grfTrashed != 0)
                {
                    changed = true;
                    pf.grfTrashed = 0;
                }
                if (pf.ConstantRegisters.Count > 0)
                {
                    pf.ConstantRegisters.Clear();
                    changed = true;
                }
            }
            else
            {
                var cmp = new ExpressionValueComparer();
                foreach (KeyValuePair<RegisterStorage, Expression> de in ctx.RegisterState)
                {
                    var idValue = de.Value as Identifier;
                    if (idValue != null)
                    {
                        if (de.Key != idValue.Storage)
                        {
                            tr[de.Key.Register.Number] = true;
                        }
                        else
                        {
                            pr[de.Key.Register.Number] = true;
                        }
                    }
                    else
                    {
                        tr[de.Key.Register.Number] = true;
                        var c = de.Value as Constant;
                        if (c != null)
                        {
                            if (c.IsValid)
                            {
                                Constant cOld;
                                if (!pf.ConstantRegisters.TryGetValue(de.Key, out cOld) || !cmp.Equals(cOld, c))
                                {
                                    changed = true;
                                    pf.ConstantRegisters[de.Key] = c;
                                }
                            }
                            else
                                pf.ConstantRegisters.Remove(de.Key);
                        }
                    }

                }

                if (!(tr & ~pf.TrashedRegisters).IsEmpty)
                {
                    pf.TrashedRegisters |= tr;
                    changed = true;
                }
                if (!(pr & ~pf.PreservedRegisters).IsEmpty)
                {
                    pf.PreservedRegisters |= pr;
                    changed = true;
                }
                uint grfNew = pf.grfTrashed | ctx.TrashedFlags;
                if (grfNew != pf.grfTrashed)
                {
                    pf.grfTrashed = grfNew;
                    changed = true;
                }
            }

            if (changed)
            {
                foreach (Statement stm in prog.CallGraph.CallerStatements(proc))
                {
                    worklist.Add(stm.Block);
                }
            }
        }

        public void PropagateToSuccessorBlock(Block s)
        {
            bool changed = false;
            BlockFlow succFlow = flow[s];
            var successorState = succFlow.SymbolicIn;
            var ctxSucc = succFlow.SymbolicAuxIn;

            Dump(ctx.RegisterState);
            Dump(ctx.StackState);

            foreach (var de in ctx.RegisterState)
            {
                Expression oldValue;
                if (!ctxSucc.RegisterState.TryGetValue(de.Key, out oldValue))
                {
                    ctxSucc.RegisterState[de.Key] = de.Value;
                    changed = true;
                }
                else if (!ecomp.Equals(oldValue, de.Value) && oldValue != Constant.Invalid)
                {
                    ctxSucc.RegisterState[de.Key] = Constant.Invalid;
                    changed = true;
                }

                Expression oldValue2;
                if (!successorState.TryGetValue(de.Key, out oldValue2))
                {
                    successorState[de.Key] = de.Value;
                    changed = true;
                }
                else if (oldValue2 != de.Value && oldValue2 != Constant.Invalid)
                {
                    successorState[de.Key] = Constant.Invalid;
                    changed = true;
                }
            }

            foreach (var de in ctx.StackState)
            {
                Expression oldValue;
                if (!ctxSucc.StackState.TryGetValue(de.Key, out oldValue))
                {
                    ctxSucc.StackState[de.Key] = de.Value;
                    changed = true;
                }
                else if (!ecomp.Equals(oldValue, de.Value) && oldValue != Constant.Invalid)
                {
                    ctxSucc.StackState[de.Key] = Constant.Invalid;
                    changed = true;
                }
            }

            uint grfNew = succFlow.grfTrashedIn | ctx.TrashedFlags;
            if (grfNew != succFlow.grfTrashedIn)
            {
                succFlow.grfTrashedIn = grfNew;
                changed = true;
            }
            if (changed)
            {
                worklist.Add(s);
            }
        }

        [Conditional("DEBUG")]
        private void Dump(Map<int, Expression> map)
        {
            var sort = new SortedList<string, string>();
            foreach (var de in map)
                sort.Add(de.Key.ToString(), de.Value.ToString());
            foreach (var de in sort)
                Debug.Write(string.Format("{0}:{1} ", de.Key, de.Value));
            Debug.WriteLine("");
        }

        [Conditional("DEBUG")]
        private void Dump(Dictionary<RegisterStorage, Expression> dictionary)
        {
            var sort = new SortedList<string, string>();
            foreach (var de in dictionary)
                sort.Add(de.Key.ToString(), de.Value.ToString());
            foreach (var de in sort)
                Debug.Write(string.Format("{0}:{1} ", de.Key, de.Value));
            Debug.WriteLine("");
        }

        public override void VisitAssignment(Assignment a)
        {
            se.VisitAssignment(a);
        }

        public override void VisitSideEffect(SideEffect side)
        {
            se.VisitSideEffect(side);
        }

        public override void VisitStore(Store store)
        {
            se.VisitStore(store);
        }

        public override void VisitIndirectCall(IndirectCall ic)
        {
        }

        public override void VisitCallInstruction(CallInstruction ci)
        {
            se.VisitCallInstruction(ci);
            if (ProcedureTerminates(ci.Callee))
            {
                // A terminating procedure has no trashed registers because caller will never see those effects!
                ctx.RegisterState.Clear();
                ctx.TrashedFlags = 0;
                return;
            }

            var callee = ci.Callee as Procedure;
            if (callee != null)
            {
                ProcedureFlow pf = flow[callee];
                foreach (int r in pf.TrashedRegisters)
                {
                    var reg = new RegisterStorage(prog.Architecture.GetRegister(r));
                    Constant c;
                    if (!pf.ConstantRegisters.TryGetValue(reg, out c))
                    {
                        c = Constant.Invalid;
                    }
                    ctx.RegisterState[reg] = c;
                }
                ctx.TrashedFlags |= pf.grfTrashed;
            }
            else
            {
                //$TODO: get trash information from signature?
            }
        }

        private bool ProcedureTerminates(ProcedureBase proc)
        {
            if (proc.Characteristics.Terminates)
                return true;
            var p = proc as Procedure;
            return (p != null && flow[p].TerminatesProcess);
        }

        public class TrashedExpressionSimplifier : ExpressionSimplifier
        {
            private TrashedRegisterFinder trf;
            private SymbolicEvaluationContext ctx;

            public TrashedExpressionSimplifier(TrashedRegisterFinder trf, SymbolicEvaluationContext ctx)
                : base(ctx)
            {
                this.trf = trf;
                this.ctx = ctx;
            }

            public override Expression VisitApplication(Application appl)
            {
                var e = base.VisitApplication(appl);
                var pc = appl.Procedure as ProcedureConstant;
                if (pc != null && trf.ProcedureTerminates(pc.Procedure))
                {
                    ctx.TrashedFlags = 0;
                    ctx.RegisterState.Clear();
                    return appl;
                }
                foreach (var u in appl.Arguments.OfType<UnaryExpression>())
                {
                    if (u.op == UnaryOperator.AddrOf)
                    {
                        Identifier id = u.Expression as Identifier;
                        if (id != null)
                        {
                            ctx.SetValue(id, Constant.Invalid);
                        }
                    }
                }
                return e;
            }
        }

        public class Updater : InstructionVisitor<Instruction>, ExpressionVisitor<Expression>
        {
            private SymbolicEvaluator se;
            
            public Updater(SymbolicEvaluator se)
            {
                this.se = se;
            }

            public Instruction VisitAssignment(Assignment a)
            {
                a.Accept(se);
                se.VisitAssignment(a);
                return a;
            }

            public Instruction VisitBranch(Branch b)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitCallInstruction(CallInstruction ci)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitDeclaration(Declaration decl)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitDefInstruction(DefInstruction def)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitPhiAssignment(PhiAssignment phi)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitIndirectCall(IndirectCall ic)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitReturnInstruction(ReturnInstruction ret)
            {
                if (ret.Expression != null)
                    ret.Expression.Accept(this);
                return ret;
            }

            public Instruction VisitSideEffect(SideEffect side)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitStore(Store store)
            {
                se.VisitStore(store);
                return store;
            }

            public Instruction VisitSwitchInstruction(SwitchInstruction si)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitUseInstruction(UseInstruction u)
            {
                throw new NotImplementedException();
            }

            public Expression VisitAddress(Address addr)
            {
                throw new NotImplementedException();
            }

            public Expression VisitApplication(Application appl)
            {
                throw new NotImplementedException();
            }

            public Expression VisitArrayAccess(ArrayAccess acc)
            {
                throw new NotImplementedException();
            }

            public Expression VisitBinaryExpression(BinaryExpression binExp)
            {
                throw new NotImplementedException();
            }

            public Expression VisitCast(Cast cast)
            {
                throw new NotImplementedException();
            }

            public Expression VisitConditionOf(ConditionOf cof)
            {
                throw new NotImplementedException();
            }

            public Expression VisitConstant(Constant c)
            {
                throw new NotImplementedException();
            }

            public Expression VisitDepositBits(DepositBits d)
            {
                throw new NotImplementedException();
            }

            public Expression VisitDereference(Dereference deref)
            {
                throw new NotImplementedException();
            }

            public Expression VisitFieldAccess(FieldAccess acc)
            {
                throw new NotImplementedException();
            }

            public Expression VisitIdentifier(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
            {
                throw new NotImplementedException();
            }

            public Expression VisitMemoryAccess(MemoryAccess access)
            {
                throw new NotImplementedException();
            }

            public Expression VisitMkSequence(MkSequence seq)
            {
                throw new NotImplementedException();
            }

            public Expression VisitPhiFunction(PhiFunction phi)
            {
                throw new NotImplementedException();
            }

            public Expression VisitPointerAddition(PointerAddition pa)
            {
                throw new NotImplementedException();
            }

            public Expression VisitProcedureConstant(ProcedureConstant pc)
            {
                throw new NotImplementedException();
            }

            public Expression VisitScopeResolution(ScopeResolution scopeResolution)
            {
                throw new NotImplementedException();
            }

            public Expression VisitSegmentedAccess(SegmentedAccess access)
            {
                throw new NotImplementedException();
            }

            public Expression VisitSlice(Slice slice)
            {
                throw new NotImplementedException();
            }

            public Expression VisitTestCondition(TestCondition tc)
            {
                throw new NotImplementedException();
            }

            public Expression VisitUnaryExpression(UnaryExpression unary)
            {
                throw new NotImplementedException();
            }
        }
    }
}

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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
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
    /// exits, and which registers have their values preserved. 
	/// </summary>
    public class TrashedRegisterFinder : InstructionVisitorBase
    {
        private Program prog;
        private ProgramDataFlow flow;
        private WorkList<Block> worklist;
        private TrashStorageHelper tsh;
        private SymbolicEvaluator se;
        private SymbolicEvaluationContext ctx;
        private DecompilerEventListener eventListener;
        private ExpressionValueComparer ecomp;

        public TrashedRegisterFinder(
            Program prog,
            ProgramDataFlow flow,
            DecompilerEventListener eventListener)
        {
            this.prog = prog;
            this.flow = flow;
            this.eventListener = eventListener;
            this.tsh = new TrashStorageHelper(null);
            this.worklist = new WorkList<Block>();
            this.ecomp = new ExpressionValueComparer();
        }

        public Dictionary<RegisterStorage, Expression> RegisterSymbolicValues { get { return ctx.RegisterState; } }
        public IDictionary<int, Expression> StackSymbolicValues { get { return ctx.StackState; } } 
        public uint TrashedFlags {get { return ctx.TrashedFlags; } }


        public void CompleteWork()
        {
            foreach (Procedure proc in prog.Procedures.Values)
            {
                ProcedureFlow pf = flow[proc];
                foreach (int r in pf.TrashedRegisters)
                {
                    prog.Architecture.GetRegister(r).SetAliases(pf.TrashedRegisters, true);
                }
            }
        }

        public void Compute()
        {
            FillWorklist();
            int initial = worklist.Count;
            Block block;
            while (worklist.GetWorkItem(out block))
            {
                var e = eventListener;
                if (e != null)
                    eventListener.ShowProgress(string.Format("Blocks left: {0}", worklist.Count), initial - worklist.Count, initial);

                ProcessBlock(block);
            }
            CompleteWork();
        }

        public void FillWorklist()
        {
            foreach (Procedure proc in prog.Procedures.Values)
            {
                foreach (Block block in proc.ControlGraph.Nodes)
                {
                    worklist.Add(block);
                }
            }
        }

        public bool IsTrashed(Storage storage)
        {
            var reg = storage as RegisterStorage;
            if (reg != null)
            {
                Expression regVal;
                if (!ctx.RegisterState.TryGetValue(reg, out regVal))
                    return false;
                var id = regVal as Identifier;
                if (id == null)
                    return true;
                return id.Storage == reg;
            }
            throw new NotImplementedException();
        }

        public void ProcessBlock(Block block)
        {
            var bf = flow[block];
            EnsureEvaluationContext(bf);
            tsh = CreateTrashStorageHelper(bf);
            block.Statements.ForEach(stm => stm.Instruction.Accept(this));
            
            if (block == block.Procedure.ExitBlock)
            {
                PropagateToProcedureSummary(block.Procedure);
            }
            else
            {
                foreach (Block s in block.Procedure.ControlGraph.Successors(block))
                {
                    PropagateToSuccessorBlock(s);
                }
            }
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

        private TrashStorageHelper CreateTrashStorageHelper(BlockFlow bf)
        {
            return new TrashStorageHelper(new Dictionary<Storage, Storage>(bf.TrashedIn), bf.grfTrashedIn, null);
        }

        public void PropagateToProcedureSummary(Procedure proc)
        {
            bool changed = false;
            ProcedureFlow pf = flow[proc];
            BitSet tr = prog.Architecture.CreateRegisterBitset();
            BitSet pr = prog.Architecture.CreateRegisterBitset();
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
            }
            else
            {
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
            BlockFlow sf = flow[s];
            var successorState = sf.SymbolicIn;
            var ctxSucc = sf.SymbolicAuxIn;
            foreach (KeyValuePair<RegisterStorage, Expression> de in ctx.RegisterState)
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

            uint grfNew = sf.grfTrashedIn | tsh.TrashedFlags;
            if (grfNew != sf.grfTrashedIn)
            {
                sf.grfTrashedIn = grfNew;
                changed = true;
            }
            if (changed)
            {
                worklist.Add(s);
            }
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

        public override void VisitCallInstruction(CallInstruction ci)
        {
            se.VisitCallInstruction(ci);
            if (ProcedureTerminates(ci.Callee))
            {
                // A terminating procedure has no trashed registers because caller will never see those effects!
                tsh.TrashedFlags = 0;
                ctx.RegisterState.Clear();
                return;
            }

            var callee = ci.Callee as Procedure;
            if (callee == null)
                return;             //$TODO: get trash information from signature?
            ProcedureFlow pf = flow[callee];
            foreach (int r in pf.TrashedRegisters)
            {
                var reg = new RegisterStorage(prog.Architecture.GetRegister(r));
                ctx.RegisterState[reg] = Constant.Invalid;
            }
            tsh.TrashedFlags |= pf.grfTrashed;
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
                if (pc != null)
                {
                    if (trf.ProcedureTerminates(pc.Procedure))
                    {
                        ctx.TrashedFlags = 0;
                        ctx.RegisterState.Clear();
                        return appl;
                    }
                }
                for (int i = 0; i < appl.Arguments.Length; ++i)
                {
                    UnaryExpression u = appl.Arguments[i] as UnaryExpression;
                    if (u != null && u.op == UnaryOperator.AddrOf)
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
    }
}

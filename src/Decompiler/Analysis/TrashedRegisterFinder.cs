 #region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Typing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Analysis
{
	/// <summary>
	/// Uses an interprocedural reaching definition analysis to detect which 
	/// registers are modified by the procedures, which registers are constant
    /// at block exits, and which registers have their values preserved.
    /// <para>
    /// The results of the analysis are stored in the ProgramDataFlow.</para>
	/// </summary>
    //$TODO: bugger. Can use a queue here, but must use depth-first search.
    public class TrashedRegisterFinder : InstructionVisitor<Instruction>
    {
        private Program program;
        private IEnumerable<Procedure> procedures;
        private ProgramDataFlow flow;
        private BlockFlow bf;
        private WorkList<Block> worklist;
        private HashSet<Block> visited;
        private SymbolicEvaluator se;
        private SymbolicEvaluationContext ctx;
        private DecompilerEventListener eventListener;
        private ExpressionValueComparer ecomp;

        public TrashedRegisterFinder(
            Program program,
            IEnumerable<Procedure> procedures,
            ProgramDataFlow flow,
            DecompilerEventListener eventListener)
        {
            this.program = program;
            this.procedures = procedures;
            this.flow = flow;
            this.eventListener = eventListener ?? NullDecompilerEventListener.Instance;
            this.worklist = new WorkList<Block>();
            this.visited = new HashSet<Block>();
            this.ecomp = new ExpressionValueComparer();
        }

        public Dictionary<Storage, Expression> RegisterSymbolicValues { get { return ctx.RegisterState; } }
        public IDictionary<int, Expression> StackSymbolicValues { get { return ctx.StackState; } } 
        public uint TrashedFlags {get { return ctx.TrashedFlags; } }

        public void CompleteWork()
        {
            foreach (Procedure proc in procedures)
            {
                if (eventListener.IsCanceled())
                    break;
                var pf = flow[proc];
                foreach (var reg in pf.TrashedRegisters.ToList())
                {
                    pf.TrashedRegisters.UnionWith(program.Architecture.GetAliases(reg));
                }
            }
        }

        public void Compute()
        {
            FillWorklist();
            ProcessWorkList();
            CompleteWork();
        }

        public void ProcessWorkList()
        {
            int initial = worklist.Count;
            Block block;
            var e = eventListener;
            while (worklist.GetWorkItem(out block))
            {
                if (e.IsCanceled())
                    break;
                eventListener.ShowStatus(string.Format("Blocks left: {0}", worklist.Count));
                ProcessBlock(block);
            }
        }

        public void FillWorklist()
        {
            foreach (Procedure proc in procedures)
            {
                worklist.Add(proc.EntryBlock);
                SetInitialValueOfStackPointer(proc);
            }
        }

        public void RewriteBasicBlocks()
        {
            foreach (var proc in procedures)
            {
                SetInitialValueOfStackPointer(proc);
                var blocks = new DfsIterator<Block>(proc.ControlGraph).PreOrder().ToList();
                foreach (var block in blocks)
                {
                    if (eventListener.IsCanceled())
                        return;
                    RewriteBlock(block);
                }
            }
        }

        /// <summary>
        /// Sets the initial value of the processor's stack pointer register to 
        /// the virtual register 'fp':
        ///     sp = fp
        /// The intent of this is to propagate fp into all expressions using sp,
        /// so that, for instance, the x86 sequence
        ///     push ebp
        ///     mov ebp,esp
        ///     mov eax,[ebp+8]
        /// can be translated to
        ///     esp = fp
        ///     Mem[fp - 4] = esp ; original sp
        ///     ebp = fp - 4
        ///     eax = Mem[fp + 4]
        /// </summary>
        /// <param name="proc"></param>
        private void SetInitialValueOfStackPointer(Procedure proc)
        {
            flow[proc.EntryBlock].SymbolicIn.SetValue(
                proc.Frame.EnsureRegister(program.Architecture.StackRegister),
                proc.Frame.FramePointer);
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
            visited.Add(block);
            StartProcessingBlock(block);
            foreach (var stm in block.Statements)
            {
                try
                {
                    stm.Instruction.Accept(this);
                }
                catch (Exception ex)
                {
                    eventListener.Error(
                        eventListener.CreateStatementNavigator(program, stm),
                        ex,
                        "Error while analyzing trashed registers.");
                }
            }
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
            var propagator = new ExpressionPropagator(program.Platform, se.Simplifier, ctx, flow);
            foreach (Statement stm in block.Statements)
            {
                if (eventListener.IsCanceled())
                    return;
                try
                {
                    Instruction instr = stm.Instruction.Accept(propagator);
#if _DEBUG
                    string sInstr = stm.Instruction.ToString();
                    string sInstrNew = instr.ToString();
                    if (sInstr != sInstrNew)
                    {
                        Debug.Print("Changed: ");
                        Debug.Print("\t{0}", sInstr);
                        Debug.Print("\t{0}", sInstrNew);
                    }
#endif
                    stm.Instruction = instr;
                }
                catch (Exception ex)
                {
                    var location = eventListener.CreateStatementNavigator(program, stm);
                    eventListener.Error(
                        location,
                        ex,
                        "An error occurred while processing instruction at address {0:X}.",
                            program.SegmentMap.MapLinearAddressToAddress(stm.LinearAddress));
                }
            }
        }

        public void StartProcessingBlock(Block block)
        {
            bf = flow[block];
            EnsureEvaluationContext(bf);
            if (block.Procedure.EntryBlock == block)
            {
                var sp = block.Procedure.Frame.EnsureRegister(program.Architecture.StackRegister);
                bf.SymbolicIn.RegisterState[sp.Storage] = block.Procedure.Frame.FramePointer;
            }
            ctx.TrashedFlags = bf.grfTrashedIn;
        }

        public void EnsureEvaluationContext(BlockFlow bf)
        {
            this.ctx = bf.SymbolicIn.Clone();
            var tes = new TrashedExpressionSimplifier(this, ctx);
            this.se = new SymbolicEvaluator(tes, ctx);
        }

        public void PropagateToProcedureSummary(Procedure proc)
        {
            var prop = new TrashedRegisterSummarizer(program.Architecture, proc, flow[proc], ctx);
            bool changed = prop.PropagateToProcedureSummary();
            if (changed)
            {
                foreach (Statement stm in program.CallGraph.CallerStatements(proc))
                {
                    if (visited.Contains(stm.Block))
                        worklist.Add(stm.Block);
                }
            }
        }

        public void PropagateToSuccessorBlock(Block s)
        {
            BlockFlow succFlow = flow[s];
            bool changed = MergeDataFlow(succFlow);
            if (changed)
            {
                worklist.Add(s);
            }
        }

        public bool MergeDataFlow(BlockFlow succFlow)
        {
            var ctxSucc = succFlow.SymbolicIn;
            bool changed = false;
            foreach (var de in ctx.RegisterState)
            {
                Expression oldValue;
                if (!ctxSucc.RegisterState.TryGetValue(de.Key, out oldValue))
                {
                    ctxSucc.RegisterState[de.Key] = de.Value;
                    changed = true;
                }
                else if (oldValue != Constant.Invalid && !ecomp.Equals(oldValue, de.Value))
                {
                    ctxSucc.RegisterState[de.Key] = Constant.Invalid;
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
            return changed;
        }

        [Conditional("DEBUG")]
        private void Dump(SortedList<int, Expression> map)
        {
            var sort = new SortedList<string, string>();
            foreach (var de in map)
                sort.Add(de.Key.ToString(), de.Value.ToString());
            foreach (var de in sort)
                Debug.Write(string.Format("{0}:{1} ", de.Key, de.Value));
            Debug.WriteLine("");
        }

        [Conditional("DEBUG")]
        private void Dump(Dictionary<Storage, Expression> dictionary)
        {
            var sort = new SortedList<string, string>();
            foreach (var de in dictionary)
                sort[de.Key.ToString()] = de.Value.ToString();
            foreach (var de in sort)
                Debug.Write(string.Format("{0}:{1} ", de.Key, de.Value));
            Debug.WriteLine("");
        }

        public Instruction VisitAssignment(Assignment ass)
        {
            return se.VisitAssignment(ass);
        }

        public Instruction VisitBranch(Branch br)
        {
            return se.VisitBranch(br);
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            se.VisitCallInstruction(ci);
            if (ProcedureTerminates(ci.Callee))
            {
                // A terminating procedure has no trashed registers because caller will never see those effects!
                ctx.RegisterState.Clear();
                ctx.TrashedFlags = 0;
                return ci;
            }

            var pc = ci.Callee as ProcedureConstant;
            if (pc != null)
            {
                var callee = pc.Procedure as Procedure;
                if (callee != null)
                {
                    ctx.UpdateRegistersTrashedByProcedure(flow[callee]);
                    return ci;
                }
            }
            // Hell node: will want to assume that registers which aren't
            // guaranteed to be preserved by the ABI are trashed.
            foreach (var r in ctx.RegisterState.Keys.ToList())
            {
                foreach (var reg in program.Platform.CreateTrashedRegisters())
                {
                    //$PERF: not happy about the O(n^2) algorithm,
                    // but this is better in the analysis-development 
                    // branch.
                    if (r.Domain == reg.Domain)
                    {
                        ctx.RegisterState[r] = Constant.Invalid;
                    }
                }
            }
            //$TODO: get trash information from signature?
            return ci;
        }

        public Instruction VisitDeclaration(Declaration decl)
        {
            return se.VisitDeclaration(decl);
        }

        public Instruction VisitDefInstruction(DefInstruction def)
        {
            return se.VisitDefInstruction(def);
        }

        public Instruction VisitGotoInstruction(GotoInstruction g)
        {
            return se.VisitGotoInstruction(g);
        }

        public Instruction VisitPhiAssignment(PhiAssignment phi)
        {
            return se.VisitPhiAssignment(phi);
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            return se.VisitSideEffect(side);
        }

        public Instruction VisitReturnInstruction(ReturnInstruction r)
        {
            return se.VisitReturnInstruction(r);
        }

        public Instruction VisitStore(Store store)
        {
            return se.VisitStore(store);
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction sw)
        {
            return se.VisitSwitchInstruction(sw);
        }

        public Instruction VisitUseInstruction(UseInstruction u)
        {
            throw new NotSupportedException();
        }

        private bool ProcedureTerminates(Expression expr)
        {
            var pc = expr as ProcedureConstant;
            if (pc == null)
                return false;
            var proc = pc.Procedure;
            if (proc.Characteristics != null && proc.Characteristics.Terminates)
                return true;
            var p = proc as Procedure;
            return (p != null && flow[p].TerminatesProcess);
        }

        public class TrashedExpressionSimplifier : ExpressionSimplifier
        {
            private TrashedRegisterFinder trf;
            private SymbolicEvaluationContext ctx;

            public TrashedExpressionSimplifier(TrashedRegisterFinder trf, SymbolicEvaluationContext ctx)
                : base(ctx, trf.eventListener)
            {
                this.trf = trf;
                this.ctx = ctx;
            }

            public override Expression VisitApplication(Application appl)
            {
                var e = base.VisitApplication(appl);
                if (appl.Procedure != null && trf.ProcedureTerminates(appl.Procedure))
                {
                    ctx.TrashedFlags = 0;
                    ctx.RegisterState.Clear();
                    return appl;
                }
                foreach (var u in appl.Arguments.OfType<UnaryExpression>())
                {
                    if (u.Operator == UnaryOperator.AddrOf)
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

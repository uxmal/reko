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
using Reko.Core.Services;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    public class TrashedRegisterFinder3 : InstructionVisitor<Instruction>
    {
        private IProcessorArchitecture arch;
        private ProgramDataFlow flow;
        private Dictionary<Procedure, HashSet<Storage>> assumedPreserved;
        private ExpressionValueComparer cmp;
        private CallGraph callGraph;
        private DecompilerEventListener listener;
        private HashSet<SsaTransform> sccGroup;
        private WorkStack<Block> worklist;
        private Dictionary<Procedure, ProcedureFlow> procCtx;
        private Dictionary<Block, Context> blockCtx;
        private Context ctx;
        private bool propagateToCallers;

        public TrashedRegisterFinder3(
            IProcessorArchitecture arch,
            ProgramDataFlow flow,
            CallGraph callGraph,
            IEnumerable<SsaTransform> sccGroup,
            DecompilerEventListener listener)
        {
            this.arch = arch;
            this.flow = flow;
            this.sccGroup = sccGroup.ToHashSet();
            this.callGraph = callGraph;
            this.assumedPreserved = sccGroup.ToDictionary(k => k.SsaState.Procedure, v => new HashSet<Storage>());
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
            this.worklist = new WorkStack<Block>();
        }

        public void Compute()
        {
            CreateState();
            worklist.AddRange(sccGroup.Select(s => s.SsaState.Procedure.EntryBlock));
            this.propagateToCallers = false;
            Block block;
            while (worklist.GetWorkItem(out block))
            {
                ProcessBlock(block);
            }
        }

        private void CreateState()
        {
            this.blockCtx = new Dictionary<Block, Context>();
            this.procCtx = new Dictionary<Procedure, ProcedureFlow>();
            foreach (var sst in sccGroup)
            {
                var proc = sst.SsaState.Procedure;
                var procFlow = this.flow[proc];
                procCtx.Add(proc, procFlow);
                var idState = new Dictionary<Identifier, Expression>();
                foreach (var block in sst.SsaState.Procedure.ControlGraph.Blocks)
                {
                    blockCtx[block] = new Context(idState, procFlow);
                }
            }
        }

        private void ProcessBlock(Block block)
        {
            this.ctx = blockCtx[block];
            
            foreach (var stm in block.Statements)
            {
                stm.Instruction.Accept(this);
            }
            if (block == block.Procedure.ExitBlock)
            {
                UpdateProcedureSummary(this.ctx, block);
            }
            else
            {
                UpdateProcedureSuccessors(block);
            }
        }

        private void UpdateProcedureSummary(Context ctx, Block block)
        {
            if (!propagateToCallers)
                return;
            foreach (var de in ctx.IdState)
            {
                throw new NotImplementedException();

            }
        }

        private void UpdateProcedureSuccessors(Block block)
        {
            var bf = blockCtx[block];
            foreach (var s in block.Succ)
            {
                var changed = blockCtx[s].MergeWith(bf);
                if (changed)
                    worklist.Add(s);
            }
        }

        public Instruction VisitAssignment(Assignment ass)
        {
            var eval = new ExpressionSimplifier(ctx, listener);
            var value = ass.Src.Accept(eval);
            ctx.SetValue(ass.Dst, value);
            return ass;
        }

        public Instruction VisitBranch(Branch branch)
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

        public Instruction VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
            {
                var eval = new ExpressionSimplifier(ctx, listener);
                ret.Expression.Accept(eval);
            }
            return ret;
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitStore(Store store)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitUseInstruction(UseInstruction use)
        {
            // We're in the exit block now, so collect all identifiers
            // that are registes into the procedureflow of the 
            // current procedure.
            var id = use.Expression as Identifier;
            if (id != null && id.Storage is RegisterStorage)
            {
                var value = ctx.GetValue(id);
                if (value == Constant.Invalid)
                {
                    ctx.ProcFlow.Trashed.Add(id.Storage);
                }
                else
                {
                    var c = value as Constant;
                    if (c != null)
                    {
                        ctx.ProcFlow.Constants[id.Storage] = c;
                    }
                }
            }
            return use;
        }

        public class Context : EvaluationContext
        {
            public readonly Dictionary<Identifier, Expression> IdState;
            public readonly ProcedureFlow ProcFlow;
            public readonly Dictionary<int, Expression> StackState;

            public Context(Dictionary<Identifier, Expression> idState, ProcedureFlow procFlow)
            {
                this.IdState = idState;
                this.ProcFlow = procFlow;
                this.StackState = new Dictionary<int, Expression>();
            }

            public bool MergeWith(Context ctxOther)
            {
                return true;
            }

            public Expression GetDefiningExpression(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(SegmentedAccess access)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(Application appl)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(MemoryAccess access)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(Identifier id)
            {
                return IdState[id];
            }

            public bool IsUsedInPhi(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression MakeSegmentedAddress(Constant c1, Constant c2)
            {
                throw new NotImplementedException();
            }

            public void RemoveExpressionUse(Expression expr)
            {
                throw new NotImplementedException();
            }

            public void RemoveIdentifierUse(Identifier id)
            {
                throw new NotImplementedException();
            }

            public void SetValue(Identifier id, Expression value)
            {
                Expression oldValue;
                if (!IdState.TryGetValue(id, out oldValue))
                {
                    IdState.Add(id, value);
                }
                else
                    throw new NotImplementedException();
            }

            public void SetValueEa(Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void SetValueEa(Expression basePointer, Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void UseExpression(Expression expr)
            {
                throw new NotImplementedException();
            }
        }
    }
}

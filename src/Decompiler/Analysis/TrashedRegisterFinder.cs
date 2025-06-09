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
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Lib;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis
{
    public class TrashedRegisterFinder : InstructionVisitor<bool>
    {
        private static readonly TraceSwitch trace = new(nameof(TrashedRegisterFinder), "Trashed value propagation") { Level = TraceLevel.Error };

        private readonly IReadOnlyProgram program;
        private readonly IMemory memory;
        private readonly ProgramDataFlow flow;
        private readonly HashSet<SsaTransform> sccGroup;
        private readonly IReadOnlyCallGraph callGraph;
        private readonly IEventListener listener;
        private readonly WorkStack<Block> worklist;
        private readonly Dictionary<Procedure, SsaState> ssas;
        private readonly ExpressionValueComparer cmp;
        //$REFACTOR the following expressions to get rid of the nullable '?'s.
        private Block? block;
        private Dictionary<Procedure, ProcedureFlow>? procCtx;
        private Dictionary<Block, Context>? blockCtx;
        private Context? ctx;       //$R
        private ExpressionSimplifier eval;
        private IProcessorArchitecture arch;
        private bool propagateToCallers;
        private bool selfRecursiveCalls;

        public TrashedRegisterFinder(
            IReadOnlyProgram program,
            ProgramDataFlow flow,
            IEnumerable<SsaTransform> sccGroup,
            IEventListener listener)
        {
            this.program = program;
            this.arch = program.Architecture;
            this.memory = program.Memory;
            this.flow = flow;
            this.sccGroup = sccGroup.ToHashSet();
            this.callGraph = program.CallGraph;
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
            this.worklist = new WorkStack<Block>();
            this.ssas = sccGroup.ToDictionary(s => s.SsaState.Procedure, s => s.SsaState);
            this.program = program;
            this.eval = default!;
        }

        /// <summary>
        /// Computes the trashed registers for all the procedures in the 
        /// SCC group.
        /// </summary>
        /// <remarks>
        /// To deal with recursive functions -- including deeply nested
        /// mutually recursive functions, we first compute what registers
        /// are trashed when recursion is disregarded. If there are no
        /// recursive calls, we are done and leave early. 
        /// If there are recursive calls, we make one unwarranted but
        /// highly likely assumption: for each involved procedure, 
        /// the stack pointer will have the same value in the exit block 
        /// after traversing both non-recursive and recursive paths
        /// of the program.
        /// </remarks>
        public void Compute()
        {
            CreateState();
            this.propagateToCallers = false;

            while (worklist.TryGetWorkItem(out Block? block))
            {
                arch = block.Procedure.Architecture;
                ProcessBlock(block);
            }
            
            if (!selfRecursiveCalls)
            {
                return;
            }

            // We make a big, but very reasonable assumption here: if a procedure
            // has a recursive branch and a non-recursive branch, the stack pointer
            // will have the same value at the point where the branches join.
            // It certainly possible for an assembly language programmer to construct
            // a program where procedures deliberately put the stack in imbalance
            // after calling a procedure, but using such a procedure is very difficult
            // to do as you must somehow understand how the procedure changes the
            // stack pointers depending on ... anything! 
            // It seems safe to assume that all branches leading to the exit block
            // have the same stack pointer value.
            
            var savedSps = CollectStackPointers(flow, arch.StackRegister);
            //$REVIEW: Ew. This hardwires a dependency on x87 in common code.
            // We need a general mechanism for dealing with "stack pointers"
            // that abstracts over platform integer stack pointers and the
            // x87 FPU stack pointer.
            var savedTops = new Dictionary<Procedure, int?>();
            if (arch.TryGetRegister("Top", out var top))
            {
                savedTops = CollectStackPointers(flow, top);
            }

            CreateState();

            ApplyStackPointers(savedSps, flow);
            ApplyStackPointers(savedTops, flow);

            BypassRegisterOffsets(savedSps, arch.StackRegister);

            this.propagateToCallers = true;
            while (worklist.TryGetWorkItem(out block))
            {
                ProcessBlock(block);
            }
        }

        private static Dictionary<Procedure, int?> CollectStackPointers(
            ProgramDataFlow flow,
            Storage? stackRegister)
        {
            if (stackRegister is null)
                return [];
            return flow.ProcedureFlows.ToDictionary(
                de => de.Key,
                de =>
                {
                    if (de.Value.Trashed.Contains(stackRegister))
                        return (int?)null;
                    if (de.Value.Preserved.Contains(stackRegister))
                        return 0;
                    //$TODO: x86 RET N instructions.
                    return 0;
                });
        }

        private void ApplyStackPointers(
            Dictionary<Procedure, int?> savedSps,
            ProgramDataFlow flow)
        {
            foreach (var de in savedSps)
            {
                var p = flow[de.Key];
                if (de.Value.HasValue && de.Value.Value == 0)
                {
                    //$TODO: x86 RET-N instructions unbalance the stack
                    // register.
                    p.Trashed.Remove(arch.StackRegister);
                    p.Preserved.Add(arch.StackRegister);
                }
            }
        }

        private void BypassRegisterOffsets(Dictionary<Procedure, int?> savedSps, RegisterStorage register)
        {
            foreach (var ssa in this.ssas.Values)
            {
                var callStms = ssa.Procedure.Statements
                    .Where(stm => stm.Instruction is CallInstruction)
                    .ToList();
                var ssam = new SsaMutator(ssa);
                foreach (var stm in callStms)
                {
                    var call = (CallInstruction)stm.Instruction;
                    if ((call.Callee as ProcedureConstant)?.Procedure is not Procedure proc)
                        continue;
                    if (savedSps.TryGetValue(proc, out var delta) &&
                        delta.HasValue)
                    {
                        ssam.AdjustRegisterAfterCall(stm, call, register, delta.Value);
                    }
                }
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
                procFlow.Trashed.Clear();
                procFlow.Preserved.Clear();
                procFlow.Constants.Clear();

                procCtx.Add(proc, procFlow);
                var idState = new Dictionary<Identifier, (Expression,BitRange)>();
                //$REVIEW: this assumes the existence of a frame pointer.
                Identifier? fp;
                if (sst.SsaState.Identifiers.TryGetValue(proc.Frame.FramePointer, out var sidFp))
                {
                    fp = sidFp.Identifier;
                }
                else
                {
                    fp = null;
                }
                var block = proc.EntryBlock;
                blockCtx.Add(block, new Context(sst.SsaState, fp!, idState, procFlow));

                if (proc.Signature.ParametersValid)
                {
                    ApplySignature(proc.Signature, flow[proc]);
                }
                else
                {
                    worklist.Add(proc.EntryBlock);
                }
                procFlow.TerminatesProcess = proc.ExitBlock.Pred.Count == 0;
            }
        }

        private static void ApplySignature(FunctionType sig, ProcedureFlow procFlow)
        {
            //$REVIEW: do we need this? if a procedure has a signature,
            // we will always trust that rather than the flow.
            foreach (var id in sig.Outputs)
            {
                if (id.DataType.Domain != Domain.None)
                    procFlow.Trashed.Add(sig.Outputs[0].Storage);
            }
        }

        private void ProcessBlock(Block block)
        {
            this.ctx = blockCtx![block];
            this.eval = new ExpressionSimplifier(memory, ctx, listener);
            this.block = block;
            this.ctx.IsDirty = false;
            foreach (var stm in block.Statements)
            {
                try
                {
                    if (!stm.Instruction.Accept(this))
                        return;
                }
                catch (Exception ex)
                {
                    listener.Error(
                        listener.CreateStatementNavigator(program, stm),
                        ex,
                        "An error occurred when finding the trashed registers in {0}", stm);
                    block.Procedure.Write(false, Console.Out);
                    block.Procedure.Dump(true);
                }
            }
            if (block == block.Procedure.ExitBlock)
            {
                UpdateProcedureSummary(this.ctx, block);
            }
            else
            {
                UpateBlockSuccessors(block);
            }
        }

        private void UpdateProcedureSummary(Context ctx, Block block)
        {
            if (!propagateToCallers || !ctx.IsDirty)
                return;

           // Propagate defined registers to calling procedures.
            var callingBlocks = callGraph
                .FindCallerStatements(block.Procedure)
                .Cast<Statement>()
                .Select(s => s.Block)
                .Where(b => ssas.ContainsKey(b.Procedure));
            foreach (var caller in callingBlocks)
            {
                if (!blockCtx!.TryGetValue(caller, out var succCtx))
                {
                    var ssaCaller = ssas[caller.Procedure];
                    var fpCaller = ssaCaller.Identifiers[caller.Procedure.Frame.FramePointer].Identifier;
                    var idsCaller = blockCtx[caller.Procedure.EntryBlock].IdState;
                    var clone = new Context(
                        ssaCaller,
                        fpCaller,
                        idsCaller,
                        procCtx![caller.Procedure]);
                    blockCtx.Add(caller, clone);
                }
                else
                {
                    succCtx.MergeWith(ctx);
                }
                worklist.Add(caller);
            }
        }

        private void UpateBlockSuccessors(Block block)
        {
            var bf = blockCtx![block];
            foreach (var s in block.Succ)
            {
                if (!blockCtx.TryGetValue(s, out var succCtx))
                {
                    var ctxClone = ctx!.Clone();
                    blockCtx.Add(s, ctxClone);
                    worklist.Add(s);
                }
                else if (bf.IsDirty)
                {
                    succCtx.MergeWith(bf);
                    worklist.Add(s);
                }
            }
        }

        public bool VisitAssignment(Assignment ass)
        {
            var (value, _) = ass.Src.Accept(eval);
            trace.Verbose("{0} = [{1}]", ass.Dst, value);
            ctx!.SetValue(ass.Dst, value);

            return true;
        }

        public bool VisitBranch(Branch branch)
        {
            branch.Condition.Accept(eval);
            return true;
        }

        public bool VisitCallInstruction(CallInstruction ci)
        {
            var ctx = this.ctx!;
            if (ci.Callee is not ProcedureConstant pc)
            {
                foreach (var d in ci.Definitions)
                {
                    var invalid = InvalidConstant.Create(d.Expression.DataType);
                    ctx.SetValue((Identifier) d.Expression, invalid);
                    trace.Verbose("  {0} = [{1}]", d.Expression, invalid);
                }
                return true;
            }

            if (pc.Procedure is ExternalProcedure ep)
            {
                // External procedures with signatures will have generated
                // an Application earlier; without signatures they will have
                // generated hell nodes. In either case we can't propagate 
                // anything, so we leave early.
                return true;
            }
            if (pc.Procedure is not Procedure callee)
                return true;

            if (sccGroup.Any(s => s.SsaState.Procedure == callee))
            {
                // We're calling a function in the recursion group. If 
                // we are not in propagate to callers mode, simply stop 
                // at this block.
                if (!propagateToCallers)
                {
                    selfRecursiveCalls = true;
                    return false;
                }
                // Otherwise, use the flow information collected by
                // the previous pass.
            }

            var flow = this.flow[callee];
            foreach (var d in ci.Definitions)
            {
                if (flow.Trashed.Contains(d.Storage))
                {
                    var invalid = InvalidConstant.Create(d.Expression.DataType);
                    ctx.SetValue((Identifier) d.Expression, invalid);
                    trace.Verbose("  {0} = [{1}]", d.Expression, invalid);
                }
                if (flow.Preserved.Contains(d.Storage))
                {
                    var before = ci.Uses
                        .Where(u => u.Storage == d.Storage)
                        .Select(u =>
                        {
                            var (e, _) = u.Expression.Accept(eval);
                            return e;
                        })
                        .SingleOrDefault();
                    ctx.SetValue((Identifier)d.Expression, before!);
                    trace.Verbose("  {0} = [{1}]", d.Expression, before!);
                }
            }
            return true;
        }

        public bool VisitComment(CodeComment comment)
        {
            return true;
        }

        public bool VisitDefInstruction(DefInstruction def)
        {
            var id = def.Identifier;
            ctx!.SetValue(id, id, BitRange.Empty);
            return true;
        }

        public bool VisitGotoInstruction(GotoInstruction g)
        {
            if (g.Condition is not null)
            {
                g.Condition.Accept(eval);
            }
            g.Target.Accept(eval);
            return true;
        }

        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            Expression? total = null;
            var ctx = this.ctx!;
            foreach (var de in phi.Src.Arguments)
            {
                var p = de.Block;
                var phiarg = (Identifier) de.Value;
                // If phiarg hasn't been evaluated yet, it will have
                // the value null after ctx.GetValue below. If not, we 
                // use that value and hope all of the phi args have
                // the same value.
                var value = ctx.GetValue(phiarg);
                if (total is null)
                {
                    total = value;
                }
                else if (value is not null && !cmp.Equals(value, total))
                {
                    total = InvalidConstant.Create(phiarg.DataType);
                    break;
                }
            }
            if (total is not null)
            {
                ctx.SetValue(phi.Dst, total);
            }
            trace.Verbose("{0} = φ[{1}]", phi.Dst, total!);
            return true;
        }

        public bool VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression is not null)
            {
                ret.Expression.Accept(eval);
            }
            return true;
        }

        public bool VisitSideEffect(SideEffect side)
        {
            side.Expression.Accept(eval);
            return true;
        }

        public bool VisitStore(Store store)
        {
            var (value, _) = store.Src.Accept(eval);
            if (store.Dst is MemoryAccess mem)
            {
                ctx!.SetValueEa(mem.EffectiveAddress, value);
            }
            return true;
        }

        public bool VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression.Accept(eval);
            return true;
        }

        /// <summary>
        /// We're in the exit block now, so collect all identifiers
        /// that are registers into the procedureflow of the 
        /// current procedure.
        /// </summary>
        public bool VisitUseInstruction(UseInstruction use)
        {
            if (use.Expression is not Identifier id)
                return true;
            var sid = ssas[block!.Procedure].Identifiers[id];
            var ctx = this.ctx!;
            switch (id.Storage)
            {
            case RegisterStorage _:
                {
                    var value = ctx.GetValue(id);
                    var range = ctx.GetBitRange(id);
                    var stg = arch.GetRegister(id.Storage.Domain, range) ?? id.Storage;
                    if (value is InvalidConstant)
                    {
                        ctx.ProcFlow.Trashed.Add(stg);
                        ctx.ProcFlow.Preserved.Remove(stg);
                        ctx.ProcFlow.Constants.Remove(stg);
                        return true;
                    }
                    if (value is Constant c)
                    {
                        ctx.ProcFlow.Constants[stg] = c;
                        ctx.ProcFlow.Preserved.Remove(stg);
                        ctx.ProcFlow.Trashed.Add(stg);
                        return true;
                    }
                    if (value is Identifier idV)
                    {
                        if (id.Storage == arch.StackRegister)
                        {
                            if (idV == ctx.FramePointer)
                            {
                                // Special case: if we deduce that the CPU stack
                                // register is equal to the pseudo-register FP
                                // (frame pointer), we make note that the stack
                                // register is preserved.
                                ctx.ProcFlow.Preserved.Add(arch.StackRegister);
                                return true;
                            }
                        }
                        else if (idV.Storage == id.Storage)
                        {
                            var sidV = ssas[block!.Procedure].Identifiers[idV];
                            if (sidV.DefStatement?.Instruction is DefInstruction)
                            {
                                ctx.ProcFlow.Preserved.Add(stg);
                                return true;
                            }
                        }
                    }
                    ctx.ProcFlow.Trashed.Add(stg);
                    ctx.ProcFlow.Preserved.Remove(stg);
                    ctx.ProcFlow.Constants.Remove(stg);
                }
                break;
            case FlagGroupStorage grfStorage:
                {
                    var value = ctx.GetValue(id);
                    if (value is Identifier idV && idV == sid.OriginalIdentifier)
                    {
                        ctx.ProcFlow.PreservedFlags[grfStorage.FlagRegister] =
                            ctx.ProcFlow.PreservedFlags.Get(grfStorage.FlagRegister) | grfStorage.FlagGroupBits;
                        ctx.ProcFlow.grfTrashed[grfStorage.FlagRegister] =
                            ctx.ProcFlow.grfTrashed.Get(grfStorage.FlagRegister) & ~grfStorage.FlagGroupBits;
                    }
                    else
                    {
                        ctx.ProcFlow.grfTrashed[grfStorage.FlagRegister] =
                            ctx.ProcFlow.grfTrashed.Get(grfStorage.FlagRegister) | grfStorage.FlagGroupBits;
                        ctx.ProcFlow.PreservedFlags[grfStorage.FlagRegister] =
                            ctx.ProcFlow.PreservedFlags.Get(grfStorage.FlagRegister) & ~grfStorage.FlagGroupBits;
                    }
                    return true;
                }
            case FpuStackStorage fpuStg:
                {
                    var value = ctx.GetValue(id);
                    if (value is Identifier idV && idV == sid.OriginalIdentifier)
                    {
                        ctx.ProcFlow.Preserved.Add(fpuStg);
                    }
                    else
                    {
                        ctx.ProcFlow.Trashed.Add(fpuStg);
                    }
                    return true;
                }
            }
            return true;
        }

        [Conditional("DEBUG")]
        public void Dump(Block block)
        {
            var b = block ?? this.block!;
            foreach (var de in blockCtx![b].IdState.OrderBy(i => i.Key.Name))
            {
                trace.Verbose("{0}: [{1}]", de.Key, de.Value);
            }
            foreach (var de in blockCtx[b].StackState.OrderBy(i => i.Key))
            {
                trace.Verbose("fp {0} {1}: [{2}]", de.Key >= 0 ? "+" : "-", Math.Abs(de.Key), de.Value);
            }
        }

        public class Context : EvaluationContext
        {
            public readonly Identifier FramePointer;
            public readonly Dictionary<Identifier, (Expression, BitRange)> IdState;
            public readonly Dictionary<int, Expression> StackState;
            public ProcedureFlow ProcFlow;
            private readonly SsaState ssa;
            private readonly ExpressionValueComparer cmp;

            public Context(
                SsaState ssa,
                Identifier fp,
                Dictionary<Identifier, (Expression, BitRange)> idState,
                ProcedureFlow procFlow)
                : this(
                      ssa,
                      fp,
                      idState,
                      procFlow, 
                      new Dictionary<int,Expression>(),
                      new ExpressionValueComparer())
            {
            }

            private Context(
                SsaState ssa,
                Identifier fp,
                Dictionary<Identifier, (Expression, BitRange)> idState,
                ProcedureFlow procFlow,
                Dictionary<int, Expression> stack,
                ExpressionValueComparer cmp)
            {
                this.ssa = ssa;
                this.FramePointer = fp;
                this.IdState = idState;
                this.ProcFlow = procFlow;
                this.StackState = stack;
                var arch = ssa.Procedure.Architecture;
                this.Endianness = arch.Endianness;
                this.MemoryGranularity = arch.MemoryGranularity;
                this.cmp = cmp;
            }

            public EndianServices Endianness { get; }
            public int MemoryGranularity { get; }

            public bool IsDirty { get; set; }

            public Context Clone()
            {
                return new Context(ssa, this.FramePointer, this.IdState, this.ProcFlow, new Dictionary<int, Expression>(StackState), cmp);
            }

            /// <summary>
            /// Merge <paramref name="ctxOther"/> into this context.
            /// </summary>
            /// <param name="ctxOther"></param>
            /// <returns>True if a change resulted, otherwise false.</returns>
            public bool MergeWith(Context ctxOther)
            {
                bool changed = false;
                foreach (var de in ctxOther.StackState)
                {
                    if (!this.StackState.TryGetValue(de.Key, out var oldValue))
                    {
                        changed = true;
                        this.StackState.Add(de.Key, de.Value);
                    }
                    else if (oldValue is not InvalidConstant && !cmp.Equals(oldValue, de.Value))
                    {
                        changed = true;
                        this.StackState[de.Key] = InvalidConstant.Create(oldValue.DataType);
                    }
                }
                return changed;
            }

            public Expression? GetDefiningExpression(Identifier id)
            {
                return null;
            }

            public Expression GetValue(Application appl)
            {
                var args = appl.Arguments;
                for (int i = 0; i < args.Length; ++i)
                {
                    if (args[i] is not OutArgument outArg)
                        continue;
                    if (outArg.Expression is Identifier outId)
                    {
                        SetValue(outId, InvalidConstant.Create(outId.DataType));
                    }
                }
                return InvalidConstant.Create(appl.DataType);
            }

            public Expression GetValue(MemoryAccess access, IMemory memory)
            {
                var offset = this.GetFrameOffset(access.EffectiveAddress);
                if (offset.HasValue && StackState.TryGetValue(offset.Value, out Expression? value))
                {
                    if (value.DataType is StructureType str)
                    {
                        var ptrStr = new Pointer(str, access.EffectiveAddress.DataType.BitSize);
                        value = new MemoryAccess(
                            access.MemoryId,
                            new UnaryExpression(Operator.AddrOf, ptrStr, value),
                            access.DataType);
                    }
                    return value;
                }
                return InvalidConstant.Create(access.DataType);
            }

            public Expression? GetValue(Identifier id)
            {
                if (id.Storage is StackStorage stack)
                    return StackState!.Get(stack.StackOffset);
                if (!IdState.TryGetValue(id, out (Expression, BitRange) value))
                    return null;
                else if (value.Item1 is { })
                    return MaybeSlice(value.Item1, id.DataType);
                else
                    return null;
            }

            private static Expression MaybeSlice(Expression exp, DataType dt)
            {
                var wantedBitsize = dt.BitSize;
                if (exp.DataType.BitSize > wantedBitsize)
                {
                    exp = new Slice(dt, exp, 0);
                }
                return exp;
            }

            public BitRange GetBitRange(Identifier id)
            {
                if (!IdState.TryGetValue(id, out (Expression, BitRange) value))
                    return BitRange.Empty;
                else
                    return value.Item2;
            }


            public bool IsUsedInPhi(Identifier id)
            {
                var src = ssa.Identifiers[id].DefStatement;
                if (src is null)
                    return false;
                if (src.Instruction is not Assignment assSrc)
                    return false;
                return ExpressionIdentifierUseFinder.Find(assSrc.Src)
                    .Select(c => ssa.Identifiers[c].DefStatement)
                    .Where(d => d is not null)
                    .Select(ph => ph!.Instruction as PhiAssignment)
                    .Where(ph => ph is not null)
                    .Any();
            }

            public Expression MakeSegmentedAddress(Constant c1, Constant c2)
            {
                return ssa.Procedure.Architecture.MakeSegmentedAddress(c1, c2);
            }

            public Constant ReinterpretAsFloat(Constant rawBits)
            {
                return ssa.Procedure.Architecture.ReinterpretAsFloat(rawBits);
            }
            
            public void RemoveExpressionUse(Expression expr)
            {
            }

            public void SetValue(Identifier id, Expression value)
            {
                SetValue(id, value, id.Storage.GetBitRange());
            }

            public void SetValue(Identifier id, Expression value, BitRange range)
            {
                if (id.Storage is StackStorage stack)
                {
                    if (!StackState.TryGetValue(stack.StackOffset, out Expression? oldValue))
                    {
                        trace.Verbose("Trf: Stack Offset {0:X4} now has value {1}", stack.StackOffset, value);
                        IsDirty = true;
                        StackState.Add(stack.StackOffset, value);
                    }
                    else if (oldValue is not InvalidConstant &&
                        (oldValue.DataType.BitSize != value.DataType.BitSize || !cmp.Equals(oldValue, value)))
                    {
                        trace.Verbose("Trf: Stack Offset {0:X4} now has value {1}, was {2}", stack.StackOffset, value, oldValue);
                        IsDirty = true;
                        StackState[stack.StackOffset] = InvalidConstant.Create(id.DataType);
                    }
                }
                else
                {
                    if (!IdState.TryGetValue(id, out (Expression, BitRange) oldValue))
                    {
                        trace.Verbose("Trf: id {0} now has value {1}", id, value);
                        IsDirty = true;
                        IdState.Add(id, (value, range));
                    }
                    else if (oldValue.Item1 is not InvalidConstant && !cmp.Equals(oldValue.Item1, value))
                    {
                        trace.Verbose("Trf: id {0} now has value {1}, was {2}", id, value, oldValue);
                        IsDirty = true;
                        IdState[id] = ((Expression)InvalidConstant.Create(id.DataType), range);
                    }
                }
            }

            public void SetValueEa(Expression effectiveAddress, Expression value)
            {
                var offset = GetFrameOffset(effectiveAddress);
                if (!offset.HasValue)
                    return;

                if (!StackState.TryGetValue(offset.Value, out Expression? oldValue))
                {
                    IsDirty = true;
                    trace.Verbose("Trf: Stack Offset {0:X4} now has value {1}", offset.Value, value);
                    StackState.Add(offset.Value, value);
                }
                else if (!cmp.Equals(oldValue, value) && oldValue is not InvalidConstant)
                {
                    IsDirty = true;
                    trace.Verbose("Trf: Stack Offset {0:X4} now has value {1}, was {2}", offset.Value, value, oldValue);
                    //$BUG: need the data width here.
                    StackState[offset.Value] = InvalidConstant.Create(PrimitiveType.Word32);
                }
            }

            private int? GetFrameOffset(Expression effectiveAddress)
            {
                if (effectiveAddress is SegmentedPointer segptr)
                    effectiveAddress = segptr.Offset;
                if (effectiveAddress is not BinaryExpression ea || ea.Left != FramePointer)
                    return null;
                if (ea.Right is not Constant o)
                    return null;
                if (ea.Operator.Type == OperatorType.IAdd)
                {
                    return o.ToInt32();
                }
                else if (ea.Operator.Type == OperatorType.ISub)
                {
                    return -o.ToInt32();
                }
                return null;
            }

            public void SetValueEa(Expression basePointer, Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void UseExpression(Expression expr)
            {
            }
        }
    }
}

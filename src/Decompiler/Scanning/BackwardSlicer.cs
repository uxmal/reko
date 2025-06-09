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
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// Traces a backward slice in a (potentially partial)
    /// control flow graph, in order to discover the entries of an indirect
    /// call or jump. 
    /// </summary>
    /// <remarks>
    /// The strategy used is start at the indirect jump and discover what registers
    /// are live, then proceed to the statements preceding the jump to see how
    /// those registers are computed and to see whether the registers are either
    /// compared with a constant or mask away the high bits. The first analysis
    /// gradually builds up a jump table expression, stored in the
    /// <see cref="JumpTableFormat" /> property, while the second one
    /// determines how many entries there are in the jump table, stored in the
    /// <see cref="JumpTableIndexInterval" /> property.
    /// </remarks>
    public class BackwardSlicer
    {
        public static readonly TraceSwitch trace = new TraceSwitch(nameof(BackwardSlicer), "Traces the backward slicer") { Level = TraceLevel.Warning };

        internal IBackWalkHost<RtlBlock, RtlInstruction> host;
        private readonly RtlBlock rtlBlock;
        private readonly ProcessorState processorState;
        private readonly WorkList<SliceState> worklist;
        private readonly HashSet<RtlBlock> visited;
        private readonly ExpressionValueComparer cmp;
        private readonly ExpressionSimplifier simp;
        private SliceState? state;

        public BackwardSlicer(IBackWalkHost<RtlBlock, RtlInstruction> host, RtlBlock rtlBlock, ProcessorState state)
        {
            this.host = host;
            this.rtlBlock = rtlBlock;
            this.processorState = state;
            this.worklist = new WorkList<SliceState>();
            this.visited = new HashSet<RtlBlock>();
            this.cmp = new ExpressionValueComparer();
            this.simp = new ExpressionSimplifier(host.Program.Memory, new EvalCtx(state.Endianness, state.MemoryGranularity), NullDecompilerEventListener.Instance);
        }

        /// <summary>
        /// Keeps track of what expressions are live during the analysis.
        /// </summary>
        public Dictionary<Expression, BackwardSlicerContext> Live { get { return state!.Live; } }

        /// <summary>
        /// The expression that computes the destination addresses.
        /// </summary>
        public Expression? JumpTableFormat { get { return state!.JumpTableFormat; } } 

        /// <summary>
        /// The expression used as the index in a jump/call table
        /// </summary>
        public Expression? JumpTableIndex { get { return state!.JumpTableIndex; } }

        /// <summary>
        /// The set of values the index expression may have, expressed as strided interval.
        /// </summary>
        public StridedInterval JumpTableIndexInterval => state!.JumpTableIndexInterval;

        public Expression? JumpTableIndexToUse { get { return state!.JumpTableIndexToUse; } }

        public Address? GuardInstrAddress => state?.GuardInstrAddress; 


        public TableExtent? DiscoverTableExtent(Address addrSwitch, RtlTransfer xfer, IEventListener listener)
        {
            if (!Start(rtlBlock, host.BlockInstructionCount(rtlBlock) - 1, xfer.Target))
            {
                // No registers were found, so we can't trace back. 
                return null;
            }
            while (Step())
                ;

            var jumpExpr = this.JumpTableFormat;
            var interval = this.JumpTableIndexInterval;
            var index = this.JumpTableIndexToUse;
            var ctx = new Dictionary<Expression, ValueSet>(new ExpressionValueComparer());
            if (index is null)
            {
                // Weren't able to find the index register,
                // try finding it by blind pattern matching.
                index = FindIndexWithPatternMatch(this.JumpTableFormat);
                if (index is null)
                {
                    // This is likely an indirect call like a C++
                    // vtable dispatch. Since these are common, we don't 
                    // spam the user with warnings.
                    return null;
                }

                // We have a jump table, and we've guessed the index expression.
                // At this point we've given up on knowing the exact size 
                // of the table, but we do know that it must be at least
                // more than one entry. The safest assumption is that it
                // has two entries.
                listener.Warn(
                    listener.CreateAddressNavigator(host.Program, addrSwitch),
                    "Unable to determine size of call or jump table; there may be more than 2 entries.");
                ctx.Add(index, new IntervalValueSet(index.DataType, StridedInterval.Create(1, 0, 1)));
            }
            else if (interval.IsEmpty)
            {
                return null;
            }
            else if (interval.High == Int64.MaxValue)
            {
                // We have no reasonable upper bound. We make the arbitrary
                // assumption that the jump table has 2 items; it wouldn't 
                // make sense to be indexing otherwise.
                listener.Warn(
                    listener.CreateAddressNavigator(host.Program, addrSwitch),
                    "Unable to determine the upper bound of an indirect call or jump; there may be more than 2 entries.");
                var vs = new IntervalValueSet(
                    this.JumpTableIndex!.DataType,
                    StridedInterval.Create(1, interval.Low, interval.Low + 1));
                ctx.Add(this.JumpTableIndexToUse!, vs);
            }
            else 
            {
                var idx = this.JumpTableIndex!;
                if (idx is Slice slice)
                {
                    idx = slice.Expression;
                }
                ctx.Add(idx, new IntervalValueSet(this.JumpTableIndex!.DataType, interval));
            }
            var vse = new ValueSetEvaluator(host.Architecture, host.Program.Memory, ctx, this.processorState);
            var (values, accesses) = vse.Evaluate(jumpExpr!);
            var vector = values.Values
                .Select(ForceToAddress)
                .TakeWhile(IsValidJumpTarget)
                .Select(a => a!.Value)
                .Take(2000)             // Arbitrary limit
                .ToList()!;
            if (vector.Count == 0)
                return null;
            return new TableExtent
            {
                Targets = vector,
                Accesses = accesses,
                Index = index,
                GuardInstrAddress = this.GuardInstrAddress,
            };
        }

        private bool IsValidJumpTarget(Address? addr)
        {
            if (addr is null)
                return false;
            return host.Program.Memory.IsExecutableAddress(addr.Value);
        }

        private Address? ForceToAddress(Expression arg)
        {
            switch (arg)
            {
            case InvalidConstant:
                return null;
            case Address addr:
                return addr;
            case Constant c:
                if (c.DataType.Size < host.Architecture.PointerType.Size &&
                    host.Architecture.PointerType == PrimitiveType.SegPtr32)
                {
                    var sel = rtlBlock.Address.Selector!.Value;
                    return host.Architecture.MakeSegmentedAddress(Constant.Word16(sel), c);
                }
                return host.Architecture.MakeAddressFromConstant(c, true);
            case MkSequence seq:
                if (seq.Expressions.Length == 2 &&
                    seq.Expressions[0] is Constant hd && seq.Expressions[1] is Constant tl)
                {
                    return host.Architecture.MakeSegmentedAddress(hd, tl);
                }
                break;
            }
            return null;
        }


        /// <summary>
        /// Start a slice by examining any variables in the indirect jump
        /// <paramref name="indirectJump"/>, then start tracing instructions
        /// backwards beginning at instruction <paramref name="iInstr"/> in <paramref name="block"/>.
        /// </summary>
        /// <remarks>
        /// Any expressions discovered in this step become the "roots"
        /// of the backward slice. These roots are kept in the `Live` collection.
        /// </remarks>
        /// <param name="block">Basic block of instructions.</param>
        /// <param name="iInstr">Index into the instructions in <paramref name="block"/>.</param>
        /// <param name="indirectJump">Expression containing the target of the indirect call or jump.</param>
        /// <returns>If backward slicing should continue.</returns>
        public bool Start(RtlBlock block, int iInstr, Expression indirectJump)
        {
            this.state = new SliceState(this, block, iInstr);
            visited.Add(block);
            if (state!.Start(indirectJump))
            {
                worklist.Add(state);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Advance the backward slicer (paroxically by moving backwards in the 
        /// RTL instruction stream).
        /// </summary>
        /// <returns>True if progress was made, false if there was no work left to do.</returns>
        public bool Step()
        {
            // Try finding a state that isn't at the beginning of its basic block.
            SliceState? state;
            for (; ; )
            {
                if (!worklist.TryGetWorkItem(out state))
                    return false;
                this.state = state;
                if (!state.IsInBeginningOfBlock())
                    break;

                trace.Verbose("Reached beginning of block {0}", state!.block.Address);
                var preds = host.GetPredecessors(state!.block);
                if (preds.Count == 0)
                {
                    trace.Verbose("  No predecessors found for block {0}", state.block.Address);
                    trace.Verbose("  index: {0} ({1})", this.JumpTableIndex!, this.JumpTableIndexInterval);
                    trace.Verbose("  expr:  {0}", this.JumpTableFormat!);
                    return true;
                }
                foreach (var pred in preds)
                {
                    if (!visited.Contains(pred))
                    {
                        visited.Add(pred);
                        var stms = host.GetBlockInstructions(pred).ToArray();
                        //$TODO: the hack below works around the fact that some
                        // Code instructions don't exist in "raw" RTL. We are 
                        // checking for a magic 1-length array of nulls as a 
                        // sentinel.
                        if (stms.Length == 1 && stms[0].Item2 is null)
                            break;
                        SliceState pstate = state.CreateNew(pred,  state.block.Address);
                        worklist.Add(pstate);
                        trace.Verbose("  Added block {0} to worklist", pred.Address);
                    }
                }
            }
            if (state.Step())
            {
                worklist.Add(state);
                return true;
            }
            else
            {
                return worklist.Count > 0;
            }
        }

        internal bool AreEqual(Expression a, Expression b)
        {
            return cmp.Equals(a, b);
        }

        internal Expression Simplify(Expression e)
        {
            var (ee, _) = e.Accept(simp);
            return ee;
        }

        /// <summary>
        /// Finds an index candidate by pattern matching an indirect 
        /// jump expression from <paramref name="jumpTableFormat"/>. 
        /// This is last resort measure, used to extract some form of index.
        /// </summary>
        /// <remarks>
        /// If the jump table format expression is like:
        /// Mem[{index} * scale + {table_base}]
        /// or 
        /// Mem[index << scale + table_base]
        /// we assume `index` is an index into the jump table. 
        /// </remarks>
        /// <returns>The `index` expression if detected, otherwise null.</returns>
        public static Expression? FindIndexWithPatternMatch(Expression? jumpTableFormat)
        {
            if (jumpTableFormat is not MemoryAccess mem)
                return null;
            if (mem.EffectiveAddress is not BinaryExpression sum ||
                sum.Operator.Type != OperatorType.IAdd)
                return null;
            if (sum.Right is not Constant)
                return null;
            if (sum.Left is not BinaryExpression mul)
                return null;
            if (mul.Right is not Constant cScale)
                return null;
            int scale;
            switch (mul.Operator)
            {
            case SMulOperator _:
            case UMulOperator _:
            case IMulOperator _:
                scale = cScale.ToInt32();
                break;
            case ShlOperator _:
                scale = 1 << cScale.ToInt32();
                break;
            default:
                return null;
            }
            return mul.Left;
        }

        class EvalCtx : EvaluationContext
        {
            public EvalCtx(EndianServices e, int memoryGranularity)
            {
                this.Endianness = e;
                this.MemoryGranularity = memoryGranularity;
            }

            public EndianServices Endianness { get; }

            public int MemoryGranularity { get; }

            public Expression GetDefiningExpression(Identifier id)
            {
                return id;
            }

            public Expression GetValue(Identifier id)
            {
                return id;
            }

            public Expression GetValue(MemoryAccess access, IMemory memory)
            {
                return access;
            }

            public Expression GetValue(Application appl)
            {
                return appl;
            }

            public bool IsUsedInPhi(Identifier id)
            {
                return false;
            }

            public Expression MakeSegmentedAddress(Constant c1, Constant c2)
            {
                return Address.SegPtr(
                    c1.ToUInt16(),
                    c2.ToUInt16());
            }

            public Constant ReinterpretAsFloat(Constant rawBits)
            {
                return InvalidConstant.Create(rawBits.DataType);
            }

            public void RemoveExpressionUse(Expression expr)
            {
            }

            public void SetValue(Identifier id, Expression value)
            {
            }

            public void SetValueEa(Expression ea, Expression value)
            {
            }

            public void SetValueEa(Expression basePointer, Expression ea, Expression value)
            {
            }

            public void UseExpression(Expression expr)
            {
            }
        }

        public RtlInstructionVisitor<RtlInstruction> CreateBlockConstantPropagator()
        {
            return new BlockConstantPropagator(host.Program.SegmentMap, NullDecompilerEventListener.Instance);
        }
    }

    [Flags]
    public enum ContextType
    {
        None = 0,
        Jumptable = 1,
        Condition = 2,
    }

    public class BackwardSlicerContext : IComparable<BackwardSlicerContext>
    {
        public ContextType Type;
        public BitRange BitRange;   // Indicates the range of bits which are live.

        public BackwardSlicerContext(ContextType type, BitRange range)
        {
            Type = type;
            BitRange = range;
        }

        /// <summary>
        /// The expression is being used in a jump table context.
        /// </summary>
        /// <param name="bitRange"></param>
        /// <returns></returns>
        public static BackwardSlicerContext Jump(BitRange bitRange)
        {
            return new BackwardSlicerContext(ContextType.Jumptable, bitRange);
        }

        /// <summary>
        /// The expression is being used in a conditional context.
        /// </summary>
        /// <param name="bitRange"></param>
        /// <returns></returns>
        public static BackwardSlicerContext Cond(BitRange bitRange)
        {
            return new BackwardSlicerContext(ContextType.Condition, bitRange);
        }

        public int CompareTo(BackwardSlicerContext? that)
        {
            if (that is null)
                return 1;
            return this.BitRange.CompareTo(that.BitRange);
        }

        public BackwardSlicerContext Merge(BackwardSlicerContext that)
        {
            var type = this.Type | that.Type;
            var range = this.BitRange | that.BitRange;
            return new BackwardSlicerContext(type, range);
        }

        public override string ToString()
        {
            return $"({this.Type},{this.BitRange})";
        }
    }

    /// <summary>
    /// An instance of the backwards-tracing "feelers" that tries to locate
    /// instructions that modify or use registers that are relevant to 
    /// the indirect jump whose table extent we're trying to discover.
    /// </summary>
    /// <remarks>
    /// The visitor methods return a <see cref="SlicerResult" /> if they've
    /// discovered something interesting. If the SlicerResult is null, it 
    /// means the visited instructiondoesn't affect the outcome of the jump
    /// table and can be ignored.
    /// </remarks>
    public class SliceState :
        RtlInstructionVisitor<SlicerResult?>,
        ExpressionVisitor<SlicerResult?, BackwardSlicerContext>
    {
        private readonly BackwardSlicer slicer;
        public RtlBlock block;
        public int iInstr;              // The current instruction
        public (Address, RtlInstruction)[] instrs; // The instructions of this block
        public Address? addrSucc;        // the block from which we traced.
        public ConditionCode ccNext;    // The condition code that is used in a branch.
        public Expression? assignLhs;    // current LHS
        public bool invertCondition;
        public Dictionary<Expression, BackwardSlicerContext> Live;
        private int blockCount;

        public SliceState(BackwardSlicer slicer, RtlBlock block, int iInstr)
        {
            this.slicer = slicer;
            this.block = block;

            var constPropagator = slicer.CreateBlockConstantPropagator();

            (Address, RtlInstruction) Propagate((Address, RtlInstruction) stm)
            {
                var (addr, instr) = stm;
                var instrNew = instr.Accept(constPropagator);
                return (addr, instrNew);
            }
            this.instrs = slicer.host
                .GetBlockInstructions(block)
                .Select(instr => Propagate(instr!))
                .ToArray();

            this.iInstr = iInstr;
            this.Live = new Dictionary<Expression, BackwardSlicerContext>();
            DumpBlock(BackwardSlicer.trace.TraceVerbose);
        }

        // The RTL expression in the executable that computes the destination addresses.
        public Expression? JumpTableFormat { get; private set; }

        // An expression that tests the index 
        public Expression? JumpTableIndex { get; private set; }
        // An expression that tests the index that should be used in the resulting source code.
        public Expression? JumpTableIndexToUse { get; private set; }
        // the 'stride' of the jump/call table.
        public StridedInterval JumpTableIndexInterval { get; private set; }
        // the address of the instruction at which the range check happens.
        public Address? GuardInstrAddress { get; private set; }

        /// <summary>
        /// Start the analysis with the expression in the indirect jump.
        /// Detect any live expressions, and place them in the `Live`
        /// collection.
        /// </summary>
        /// <param name="indirectJump"></param>
        /// <returns></returns>
        public bool Start(Expression indirectJump)
        {
            BackwardSlicer.trace.Inform("Bwslc: == Starting at instruction call/goto {0} ======", indirectJump);
            var sr = indirectJump.Accept(this, BackwardSlicerContext.Jump(RangeOf(indirectJump)));
            JumpTableFormat ??= indirectJump;

            this.Live = sr!.LiveExprs;
            if (!sr.LiveExprs.Keys.OfType<Identifier>().Any())
            {
                // Couldn't find any indirect registers, so there is no work to do.
                BackwardSlicer.trace.Warn("Bwslc: No indirect registers in {0}?", indirectJump);
                return false;
            }
            BackwardSlicer.trace.Verbose("  live: {0}", DumpLive(this.Live));
            return true;
        }

        public bool Step()
        {
            var (addr, instr) = this.instrs[this.iInstr];
            BackwardSlicer.trace.Inform("Bwslc: Stepping to instruction {0}", instr);
            var sr = instr.Accept(this);
            --this.iInstr;
            if (sr is null)
            {
                // Instruction had no effect on live registers.
                return true;
            }
            foreach (var de in sr.LiveExprs)
            {
                if (this.Live.ContainsKey(de.Key))
                {
                    this.Live[de.Key] = this.Live[de.Key].Merge(de.Value);
                }
                else
                {
                    this.Live[de.Key] = de.Value;
                }
            }
            if (sr.Stop)
            {
                this.GuardInstrAddress = addr;
                var jtFormat = slicer.Simplify(this.JumpTableFormat!);
                BackwardSlicer.trace.Verbose("  Was asked to stop, stopping.");
                BackwardSlicer.trace.Verbose("  index: {0} ({1})", this.JumpTableIndex!, this.JumpTableIndexInterval);
                BackwardSlicer.trace.Verbose("  expr:  {0}", jtFormat);
                BackwardSlicer.trace.Verbose("  addr:  {0} (of range check instr)", this.GuardInstrAddress);
                return false;
            }
            if (this.Live.Count == 0)
            {
                BackwardSlicer.trace.Verbose("  No more live expressions, stopping.");
                BackwardSlicer.trace.Verbose("  index: {0} ({1})", this.JumpTableIndex!, this.JumpTableIndexInterval);
                BackwardSlicer.trace.Verbose("  expr:  {0}", this.JumpTableFormat!);
                return false;
            }
            BackwardSlicer.trace.Verbose("  live: {0}", DumpLive(this.Live));
            return true;
        }

        public bool IsInBeginningOfBlock()
        {
            return iInstr < 0;
        }

        private static StorageDomain DomainOf(Expression? e)
        {
            if (e is Identifier id)
            {
                return id.Storage.Domain;
            }
            return StorageDomain.Memory;
        }

        private StridedInterval MakeInterval_IAdd(bool invert, Constant? right)
        {
            if (right is null)
                return StridedInterval.Empty;
            var cc = this.ccNext;
            if (this.invertCondition)
                invert = !invert;
            if (invert)
                cc = cc.Invert();
            switch (cc)
            {
            default:
                return StridedInterval.Empty;
            case ConditionCode.UGE:
                long max = ~right.ToInt64();
                if (max < 0)
                    return StridedInterval.Empty;
                return StridedInterval.Create(1, 0, max);
            }
        }

        private StridedInterval MakeInterval_ISub(bool invert, Constant? right)
        {
            if (right is null || right.IsZero)
                return StridedInterval.Empty;
            var cc = this.ccNext;
            if (this.invertCondition)
                invert = !invert;
            if (invert)
                cc = cc.Invert();
            long hi;
            switch (cc)
            {
            // NOTE: GE and GT should really be modeled with the semi-open range
            // [right,inf) and the open range (right,inf), respectively. See comment
            // for LE/LT.
            case ConditionCode.GE: return StridedInterval.Create(1, right.ToInt64(), long.MaxValue);
            case ConditionCode.GT: return StridedInterval.Create(1, right.ToInt64() + 1, long.MaxValue);
            // NOTE: LE and LT should really be modeled with the semi-open range
            // (inf,right] and the open range (inf,right). However, typically compilers
            // make the mistake and use LE/LT for boundary checking in indirect transfers.
            case ConditionCode.LE:
                hi = right.ToInt64();
                if (hi < 0)
                    return StridedInterval.Empty;
                return StridedInterval.Create(1, 0, right.ToInt64());
            case ConditionCode.LT: return StridedInterval.Create(1, 0, right.ToInt64() - 1);
            case ConditionCode.ULE: return StridedInterval.Create(1, 0, (long) right.ToUInt64());
            case ConditionCode.ULT: return StridedInterval.Create(1, 0, (long) right.ToUInt64() - 1);
            case ConditionCode.UGE: return StridedInterval.Create(1, (long)right.ToUInt64(), long.MaxValue);
            case ConditionCode.UGT: return StridedInterval.Create(1, (long)right.ToUInt64() + 1, long.MaxValue);
            default:
                return StridedInterval.Empty;
            }
        }

        private StridedInterval MakeInterval_And(Expression left, Constant? right)
        {
            if (right is null)
                return StridedInterval.Empty;
            long n = right.ToInt64();
            if (Bits.IsEvenPowerOfTwo(n + 1))
            {
                // n is a mask (0000...00111..111)
                return StridedInterval.Create(1, 0, n);
            }
            else
            {
                return StridedInterval.Empty;
            }
        }

        public SlicerResult VisitAddress(Address addr, BackwardSlicerContext ctx)
        {
            return new SlicerResult
            {
                SrcExpr = addr,
            };
        }

        public SlicerResult VisitApplication(Application appl, BackwardSlicerContext ctx)
        {
            return new SlicerResult
            {
                SrcExpr = appl,
            };
        }

        public SlicerResult VisitArrayAccess(ArrayAccess acc, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult? VisitAssignment(RtlAssignment ass)
        {
            if (ass.Dst is not Identifier id)
            {
                // Ignore writes to memory.
                return null;
            }
            // Check if we're assigning the condition variable with the index variable.
            if (Live.TryGetValue(id, out var dstCtx) &&
                Live.TryGetValue(ass.Src, out var srcCtx))
            {
                if (dstCtx.Type == ContextType.Condition && srcCtx.Type == ContextType.Jumptable)
                {
                    this.JumpTableIndex = ass.Src;
                    return new SlicerResult
                    {
                        LiveExprs = Live,
                        Stop = true,
                    };
                }
                if (dstCtx.Type == ContextType.Jumptable && srcCtx.Type == ContextType.Condition)
                {
                    this.JumpTableIndex = id;
                    return new SlicerResult
                    {
                        LiveExprs = Live,
                        Stop = true,
                    };
                }
            }
            this.assignLhs = ass.Dst;
            // var killedRegs = Live.Where(de => de.Key is Identifier i && id.Storage.Covers(i.Storage)).ToList();
            var killedRegs = Live.Where(de => de.Key is Identifier i && i.Storage.Domain == id.Storage.Domain).ToList();
            if (killedRegs.Count == 0)
            {
                // This assignment doesn't affect the end result.
                return null;
            }
            foreach (var killedReg in killedRegs)
            {
                this.Live.Remove(killedReg.Key);
            }
            this.assignLhs = killedRegs[0].Key;
            var se = ass.Src.Accept(this, killedRegs[0].Value);
            if (se is null)
                return se;
            if (se.SrcExpr is not null)
            {
                var expReplacement = MaybeSlice(se.SrcExpr, assignLhs.DataType);
                var newJt = ExpressionReplacer.Replace(assignLhs, expReplacement, JumpTableFormat!);
                this.JumpTableFormat = slicer.Simplify(newJt);
            }
            BackwardSlicer.trace.Verbose("  expr:  {0}", this.JumpTableFormat!);
            this.assignLhs = null;
            return se;
        }

        public SlicerResult? VisitBinaryExpression(BinaryExpression binExp, BackwardSlicerContext ctx)
        {
            var opType = binExp.Operator.Type;
            if (opType == OperatorType.Eq || opType == OperatorType.Ne)
            {
                // Equality comparisons cannot contribute to determining the size
                // of the jump table; stop processing this instruction.
                return null;
            }

            if ((opType == OperatorType.Xor || opType == OperatorType.ISub) &&
                this.slicer.AreEqual(binExp.Left, binExp.Right))
            {
                // XOR r,r (or SUB r,r) clears a register. Is it part of a live register?
                var regDst = this.assignLhs as Identifier;
                var regHi = binExp.Left as Identifier;
                if (regHi is not null && regDst is not null &&
                    DomainOf(regDst) == regHi.Storage.Domain &&
                    regDst.Storage.OffsetOf(regHi.Storage) == 8)
                {
                    // The 8086 didn't have a MOVZX instruction, so clearing the high byte of a
                    // register BX was done by issuing XOR BH,BH
                    var seXor = new SlicerResult
                    {
                        SrcExpr = new Conversion(new Slice(PrimitiveType.Byte, this.assignLhs!, 0), PrimitiveType.Byte, regDst.DataType),
                        LiveExprs = new Dictionary<Expression, BackwardSlicerContext>
                        {
                            {
                                this.assignLhs!,
                                BackwardSlicerContext.Jump(new BitRange(0, 8))
                            }
                        }
                    };
                    return seXor;
                }
            }
            if (binExp.Operator is ConditionalOperator)
                ctx.BitRange = RangeOf(binExp.Left);
            var seLeft = binExp.Left.Accept(this, ctx);
            var seRight = binExp.Right.Accept(this, ctx);
            if (seLeft is null && seRight is null)
                return null;
            //$TODO: addOrSub
            switch (opType)
            {
            case OperatorType.ISub:
            case OperatorType.IAdd:
                if (this.Live is null || (ctx.Type & ContextType.Condition) == 0)
                    break;
                var domLeft = DomainOf(seLeft!.SrcExpr);
                if (Live.Count == 0)
                {
                    // We have no live variables, which means this subtraction instruction
                    // is both computing the jumptable index and also performing the 
                    // comparison.
                    this.JumpTableIndex = assignLhs;
                    this.JumpTableIndexToUse = assignLhs;
                    this.JumpTableIndexInterval = MakeInterval_ISub(false, binExp.Right as Constant);
                    BackwardSlicer.trace.Verbose("  Found range of {0}: {1}", assignLhs!, JumpTableIndexInterval);
                    return new SlicerResult
                    {
                        SrcExpr = null,     // the jump table expression already has the correct shape.
                        Stop = true
                    };
                }
                foreach (var live in Live)
                {
                    if (live.Value.Type != ContextType.Jumptable)
                        continue;
                    if (IsBoundaryCheck(binExp.Left, domLeft, live.Key))
                    {
                        return FoundBoundaryCheck(binExp, false, live.Key);
                    }
                }
                if (IsBoundaryCheck(binExp.Left, domLeft, this.assignLhs!))
                {
                    return FoundBoundaryCheck(binExp, false, this.assignLhs!);
                }

                // Some architectures invert their subtract operators...
                var domRight = DomainOf(seRight!.SrcExpr);
                foreach (var live in Live)
                {
                    if (live.Value.Type != ContextType.Jumptable)
                        continue;
                    if (IsBoundaryCheck(binExp.Right, domRight, live.Key))
                    {
                        return FoundBoundaryCheck(binExp, true, live.Key);
                    }
                }
                if (IsBoundaryCheck(binExp.Right, domRight, this.assignLhs!))
                {
                    return FoundBoundaryCheck(binExp, true, assignLhs!);
                }
                break;
            case OperatorType.And:
                this.JumpTableIndex = binExp.Left;
                this.JumpTableIndexToUse = binExp.Left;
                this.JumpTableIndexInterval = MakeInterval_And(binExp.Left, binExp.Right as Constant);
                return new SlicerResult
                {
                    SrcExpr = binExp,
                    Stop = true,
                };
            case OperatorType.Ugt:
                if (this.invertCondition)
                {
                    return FoundBoundaryCheck(binExp, ConditionCode.UGT);
                }
                break;
            case OperatorType.Uge:
                if (this.invertCondition)
                {
                    return FoundBoundaryCheck(binExp, ConditionCode.UGE);
                }
                break;
            case OperatorType.Ult:
                if (!this.invertCondition)
                {
                    return FoundBoundaryCheck(binExp, ConditionCode.ULT);
                }
                break;
            case OperatorType.Ule:
                if (!this.invertCondition)
                {
                    return FoundBoundaryCheck(binExp, ConditionCode.ULE);
                }
                break;

            case OperatorType.Lt:
                if (!this.invertCondition)
                {
                    return FoundBoundaryCheck(binExp, ConditionCode.LT);
                }
                break;
            case OperatorType.Le:
                if (!this.invertCondition)
                {
                    return FoundBoundaryCheck(binExp, ConditionCode.LE);
                }
                break;
            }
            IEnumerable<KeyValuePair<Expression,BackwardSlicerContext>> liveExpr = seLeft!.LiveExprs;
            if (seRight is not null)
                liveExpr = liveExpr.Concat(seRight.LiveExprs);
            var se = new SlicerResult
            {
                LiveExprs = liveExpr
                    .GroupBy(e => e.Key)
                    .ToDictionary(k => k.Key, v => v.Max(vv => vv.Value)!),
                SrcExpr = binExp,
            };
            return se;
        }

        private SlicerResult FoundBoundaryCheck(BinaryExpression binExp, ConditionCode cc)
        {
            this.JumpTableIndex = binExp.Left;
            this.JumpTableIndexToUse = binExp.Left;
            this.ccNext = cc;
            this.JumpTableIndexInterval = MakeInterval_ISub(false, binExp.Right as Constant);
            BackwardSlicer.trace.Verbose("  Found range of {0}: {1}", binExp.Left, JumpTableIndexInterval);
            return new SlicerResult
            {
                SrcExpr = binExp,
                Stop = true
            };
        }

        private bool IsBoundaryCheck(Expression eLeft, StorageDomain domLeft, Expression liveKey)
        {
            if (domLeft != StorageDomain.Memory)
                return DomainOf(liveKey) == domLeft;
            else
                return this.slicer.AreEqual(liveKey, eLeft);
        }

        private SlicerResult FoundBoundaryCheck(BinaryExpression binExp, bool commute, Expression liveKey)
        {
            var eIndex = commute ? binExp.Right : binExp.Left;
            var eLimit = commute ? binExp.Left : binExp.Right;
            var interval = binExp.Operator.Type == OperatorType.ISub
                ? MakeInterval_ISub(commute, eLimit as Constant)
                : MakeInterval_IAdd(commute, eLimit as Constant);
            this.JumpTableIndex = liveKey;
            this.JumpTableIndexToUse = eIndex;
            this.JumpTableIndexInterval = interval;
            BackwardSlicer.trace.Verbose("  Found range of {0}: {1}", liveKey, JumpTableIndexInterval);
            bool stop = Live.ContainsKey(liveKey);
            return new SlicerResult
            {
                SrcExpr = binExp,
                LiveExprs = new() { { liveKey, BackwardSlicerContext.Cond(RangeOf(liveKey)) } },
                Stop = stop
            };
        }

        public SlicerResult? VisitBranch(RtlBranch branch)
        {
            if (branch.Target is not Address addrTarget)
                throw new NotImplementedException();    //$REVIEW: do we ever see this?
            if (this.addrSucc! != addrTarget)
            {
                this.invertCondition = !this.invertCondition;
            }
            var se = branch.Condition.Accept(this, BackwardSlicerContext.Cond(new BitRange(0, 0)));
            return se;
        }

        public SlicerResult VisitCall(RtlCall call)
        {
            return new SlicerResult
            {
                LiveExprs = new Dictionary<Expression, BackwardSlicerContext>(),
                Stop = true,
            };
        }

        public SlicerResult? VisitCast(Cast cast, BackwardSlicerContext ctx)
        {
            var range = new BitRange(0, (short)cast.DataType.BitSize);
            return cast.Expression.Accept(this, new BackwardSlicerContext(ctx.Type, range));
        }

        public SlicerResult? VisitConditionalExpression(ConditionalExpression c, BackwardSlicerContext ctx)
        {
            return null;
        }

        public SlicerResult? VisitConditionOf(ConditionOf cof, BackwardSlicerContext ctx)
        {
            var se = cof.Expression.Accept(this, BackwardSlicerContext.Cond(RangeOf(cof.Expression)));
            if (se is not null && !se.Stop)
            {
                se.SrcExpr = cof;
                this.JumpTableIndex = cof.Expression;
                this.JumpTableIndexToUse = cof.Expression;
            }
            return se;
        }

        public SlicerResult VisitConstant(Constant c, BackwardSlicerContext ctx)
        {
            return new SlicerResult
            {
                LiveExprs = new Dictionary<Expression, BackwardSlicerContext>(),
                SrcExpr = c,
            };
        }

        public SlicerResult? VisitConversion(Conversion conversion, BackwardSlicerContext ctx)
        {
            var range = new BitRange(0, (short) conversion.DataType.BitSize);
            var se = conversion.Expression.Accept(this, new BackwardSlicerContext(ctx.Type, range));
            if (se is not null && se.SrcExpr is not null)
            {
                se.SrcExpr = new Conversion(se.SrcExpr, se.SrcExpr.DataType, conversion.DataType);
            }
            return se;
        }

        public SlicerResult VisitDereference(Dereference deref, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }


        public SlicerResult VisitFieldAccess(FieldAccess acc, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult? VisitGoto(RtlGoto go)
        {
            var sr = go.Target.Accept(this, BackwardSlicerContext.Jump(RangeOf(go.Target)));
            if (JumpTableFormat is null)
            {
                JumpTableFormat = go.Target;
            }
            return sr;
        }

        private static BitRange RangeOf(Expression expr)
        {
            return new BitRange(0, (short)expr.DataType.BitSize);
        }

        private static Expression MaybeSlice(Expression expr, DataType dt)
        {
            if (expr.DataType.BitSize > dt.BitSize)
            {
                return new Slice(dt, expr, 0);
            }
            return expr;
        }

        private static Expression MaybeSlice(Expression expr, BitRange bitRange)
        {
            int bitSize = bitRange.Extent;
            if (expr.DataType.BitSize > bitSize || bitRange.Lsb > 0)
            {
                return new Slice(PrimitiveType.CreateWord(bitSize), expr, bitRange.Msb);
            }
            return expr;
        }

        public SlicerResult VisitIdentifier(Identifier id, BackwardSlicerContext ctx)
        {
            var sr = new SlicerResult
            {
                LiveExprs = { { id, ctx } },
                SrcExpr = id,
            };
            return sr;
        }

        public SlicerResult VisitIf(RtlIf rtlIf)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitInvalid(RtlInvalid invalid)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitMemberPointerSelector(MemberPointerSelector mps, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitMemoryAccess(MemoryAccess access, BackwardSlicerContext ctx)
        {
            var rangeEa = new BitRange(0, (short)access.EffectiveAddress.DataType.BitSize);
            var srEa = access.EffectiveAddress.Accept(this, new BackwardSlicerContext(ctx.Type, rangeEa))!; //$BUG: dangerous!
            srEa.LiveExprs[access] = new BackwardSlicerContext(ctx.Type, ctx.BitRange);
            return new SlicerResult
            {
                LiveExprs = srEa.LiveExprs,
                SrcExpr = srEa.SrcExpr is not null ? new MemoryAccess(srEa.SrcExpr, access.DataType) : null,
                Stop = srEa.Stop
            };
        }

        public SlicerResult VisitMkSequence(MkSequence seq, BackwardSlicerContext ctx)
        {
            var srExprs = seq.Expressions
                .Select(e => e.Accept(this, ctx)!)
                .ToArray();
            var srLast = srExprs[^1]!;
            if (RangeOf(srLast.SrcExpr!) == ctx.BitRange)
            {
                return new SlicerResult
                {
                    LiveExprs = srLast.LiveExprs,
                    SrcExpr = srLast.SrcExpr,
                    Stop = srLast.Stop
                };
            }
            else 
            {
                var liveExprs = FuseLiveExpr(srExprs);
                return new SlicerResult
                {
                    LiveExprs = liveExprs,
                    SrcExpr = new MkSequence(seq.DataType, srExprs.Select(s => s!.SrcExpr).ToArray()!),
                    Stop = srExprs.Any(s => s!.Stop)
                };
            }
        }

        private Dictionary<Expression, BackwardSlicerContext> FuseLiveExpr(SlicerResult[] srExprs)
        {
            var result = new Dictionary<Expression, BackwardSlicerContext>();
            foreach (var sr in srExprs)
            {
                foreach (var (k, v) in sr.LiveExprs)
                {
                    result[k] = v;
                }
            }
            return result;
        }

        public SlicerResult? VisitNop(RtlNop rtlNop)
        {
            return null;
        }

        public SlicerResult VisitOutArgument(OutArgument outArgument, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitPhiFunction(PhiFunction phi, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitPointerAddition(PointerAddition pa, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitProcedureConstant(ProcedureConstant pc, BackwardSlicerContext ctx)
        {
            return new SlicerResult
            {
                LiveExprs = new Dictionary<Expression, BackwardSlicerContext>(),
                SrcExpr = pc,
            };
        }

        public SlicerResult VisitReturn(RtlReturn ret)
        {
            return new SlicerResult
            {
                Stop = true
            };
        }

        public SlicerResult VisitScopeResolution(ScopeResolution scopeResolution, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult? VisitSegmentedAddress(SegmentedPointer address, BackwardSlicerContext ctx)
        {
            return address.Offset.Accept(this, ctx);
        }

        public SlicerResult VisitSideEffect(RtlSideEffect side)
        {
            return new SlicerResult
            {
                Stop = true
            };
        }

        public SlicerResult? VisitSlice(Slice slice, BackwardSlicerContext ctx)
        {
            var range = new BitRange(
                (short)slice.Offset,
                (short) (slice.DataType.BitSize + slice.Offset));
            return slice.Expression.Accept(this, new BackwardSlicerContext(ctx.Type, range));
        }

        public SlicerResult VisitStringConstant(StringConstant str, BackwardSlicerContext ctx)
        {
            return new SlicerResult
            {
                LiveExprs = new Dictionary<Expression, BackwardSlicerContext>(),
                SrcExpr = str,
            };
        }

        public SlicerResult? VisitSwitch(RtlSwitch rtlSwitch)
        {
            return null;
        }

        public SlicerResult? VisitTestCondition(TestCondition tc, BackwardSlicerContext ctx)
        {
            var se = tc.Expression.Accept(this, BackwardSlicerContext.Cond(RangeOf(tc.Expression)));
            this.ccNext = tc.ConditionCode;
            return se;
        }

        public SlicerResult? VisitUnaryExpression(UnaryExpression unary, BackwardSlicerContext ctx)
        {
            if (unary.Operator.Type == OperatorType.Not)
                ctx.BitRange = RangeOf(unary.Expression);
            var sr = unary.Expression.Accept(this, ctx);
            if (unary.Operator.Type == OperatorType.Not)
            {
                this.invertCondition = !invertCondition;
            }
            return sr;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}: ", iInstr);
            if (0 <= iInstr && iInstr < instrs.Length)
            {
                sb.Append(instrs[iInstr].ToString());
            }
            else
            {
                sb.Append("<no instruction>");
            }
            sb.AppendFormat(", live: {0}", DumpLive(slicer.Live));
            return sb.ToString();
        }

        private static string DumpLive(Dictionary<Expression, BackwardSlicerContext> live)
        {
            return string.Format("{{ {0} }}",
                string.Join(
                    ",",
                    live
                        .OrderBy(l => l.Key.ToString())
                        .Select(l => $"{{ {l.Key}, {l.Value.Type} {l.Value.BitRange} }}")));
        }

        [Conditional("DEBUG")]
        private void DumpBlock(bool dump)
        {
            if (!BackwardSlicer.trace.TraceVerbose)
                return;
            var sw = new StringWriter();
            foreach (var (_, instr) in instrs)
            {
                sw.Write("    ");
                instr.Write(sw);
                sw.WriteLine();
            }
            Debug.Write(sw.ToString());
        }

        public SliceState CreateNew(RtlBlock block, Address addrSucc)
        {
            var state = new SliceState(this.slicer, block, 0)
            {
                JumpTableFormat = this.JumpTableFormat,
                JumpTableIndex = this.JumpTableIndex,
                JumpTableIndexInterval = this.JumpTableIndexInterval,
                Live = new Dictionary<Expression, BackwardSlicerContext>(this.Live, this.Live.Comparer),
                ccNext = this.ccNext,
                invertCondition = this.invertCondition,
                addrSucc = addrSucc,
                blockCount = blockCount + 1
            };
            state.iInstr = state.instrs.Length - 1;
            return state;
        }

        public SlicerResult VisitMicroGoto(RtlMicroGoto uGoto)
        {
            throw new NotImplementedException();
        }
    }

    public class SlicerResult
    {
        // Live storages are involved in the computation of the jump destinations.
        public Dictionary<Expression, BackwardSlicerContext> LiveExprs = new();
        public Expression? SrcExpr;
        public bool Stop;       // Set to true if the analysis should stop.
    }

    public class TableExtent
    {
        public List<Address>? Targets;
        public Dictionary<Address, DataType>? Accesses;
        public Expression? Index;
        public Address? GuardInstrAddress;
    }
}
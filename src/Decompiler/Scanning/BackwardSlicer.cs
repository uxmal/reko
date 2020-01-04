#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
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
    /// `JumpTableFormat` property, while the second one
    /// determines how many entries there are in the jump table, stored in the
    /// `JumpTableIndexInterval` property.
    /// </remarks>
    public class BackwardSlicer
    {
        internal static TraceSwitch trace = new TraceSwitch("BackwardSlicer", "Traces the backward slicer") { Level = TraceLevel.Warning };

        internal IBackWalkHost<RtlBlock, RtlInstruction> host;
        private RtlBlock rtlBlock;
        private ProcessorState processorState;
        private SliceState state;
        private WorkList<SliceState> worklist;
        private HashSet<RtlBlock> visited;
        private ExpressionValueComparer cmp;
        private ExpressionSimplifier simp;

        public BackwardSlicer(IBackWalkHost<RtlBlock, RtlInstruction> host, RtlBlock rtlBlock, ProcessorState state)
        {
            this.host = host;
            this.rtlBlock = rtlBlock;
            this.processorState = state;
            this.worklist = new WorkList<SliceState>();
            this.visited = new HashSet<RtlBlock>();
            this.cmp = new ExpressionValueComparer();
            this.simp = new ExpressionSimplifier(host.SegmentMap, new EvalCtx(), null);
        }

        /// <summary>
        /// Keeps track of what expressions are live during the analysis.
        /// </summary>
        public Dictionary<Expression, BackwardSlicerContext> Live { get { return state.Live; } }

        // The expression that computes the destination addresses.
        public Expression JumpTableFormat { get { return state.JumpTableFormat; } } 

        // The expression used as the index in a jump/call table
        public Expression JumpTableIndex { get { return state.JumpTableIndex; } }

        // The set of values the index expression may have, expressed as strided interval.
        public StridedInterval JumpTableIndexInterval { get { return state.JumpTableIndexInterval; } }    
        public Expression JumpTableIndexToUse { get { return state.JumpTableIndexToUse; } }

        public TableExtent DiscoverTableExtent(Address addrSwitch, RtlTransfer xfer, DecompilerEventListener listener)
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
            if (index == null)
            {
                // Weren't able to find the index register,
                // try finding it by blind pattern matching.
                index = this.FindIndexWithPatternMatch(this.JumpTableFormat);
                if (index == null)
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
                    this.JumpTableIndex.DataType,
                    StridedInterval.Create(1, interval.Low, interval.Low + 1));
                ctx.Add(this.JumpTableIndexToUse, vs);
            }
            else 
            {
                ctx.Add(this.JumpTableIndex, new IntervalValueSet(this.JumpTableIndex.DataType, interval));
            }
            var vse = new ValueSetEvaluator(host.Architecture, host.SegmentMap, ctx, this.processorState);
            var (values, accesses) = vse.Evaluate(jumpExpr);
            var vector = values.Values
                .TakeWhile(c => c != Constant.Invalid)
                .Take(2000)             // Arbitrary limit
                .Select(ForceToAddress)
                .TakeWhile(a => a != null)
                .ToList();
            if (vector.Count == 0)
                return null;
            return new TableExtent
            {
                Targets = vector,
                Accesses = accesses,
                Index = index,
            };
        }

        private Address ForceToAddress(Expression arg)
        {
            if (arg is Address addr)
                return addr;
            if (arg is Constant c)
            {
                if (c.DataType.Size < host.Architecture.PointerType.Size &&
                    host.Architecture.PointerType == PrimitiveType.SegPtr32)
                {
                    var sel = rtlBlock.Address.Selector.Value;
                    return host.Architecture.MakeSegmentedAddress(Constant.Word16(sel), c);
                }
                return host.Architecture.MakeAddressFromConstant(c, true);
            }
            if (arg is MkSequence seq)
            {
                if (seq.Expressions.Length == 2 &&
                    seq.Expressions[0] is Constant hd && seq.Expressions[1] is Constant tl)
                {
                    return host.Architecture.MakeSegmentedAddress(hd, tl);
                }
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
            if (state.Start(indirectJump))
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
            SliceState state;
            for (; ; )
            {
                if (!worklist.GetWorkItem(out state))
                    return false;
                this.state = state;
                if (!state.IsInBeginningOfBlock())
                    break;

                DebugEx.Verbose(trace, "Reached beginning of block {0}", state.block.Address);
                var preds = host.GetPredecessors(state.block);
                if (preds.Count == 0)
                {
                    DebugEx.Verbose(trace, "  No predecessors found for block {0}", state.block.Address);
                    DebugEx.Verbose(trace, "  index: {0} ({1})", this.JumpTableIndex, this.JumpTableIndexInterval);
                    DebugEx.Verbose(trace, "  expr:  {0}", this.JumpTableFormat);
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
                        if (stms.Length == 1 && stms[0] == null)
                            break;
                        SliceState pstate = state.CreateNew(pred,  state.block.Address);
                        worklist.Add(pstate);
                        DebugEx.Verbose(trace, "  Added block {0} to worklist", pred.Address);
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
            return e.Accept(simp);
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
        public Expression FindIndexWithPatternMatch(Expression jumpTableFormat)
        {
            if (!(jumpTableFormat is MemoryAccess mem))
                return null;
            if (!(mem.EffectiveAddress is BinaryExpression sum) ||
                sum.Operator != Operator.IAdd)
                return null;
            if (!(sum.Right is Constant cTable))
                return null;
            if (!(sum.Left is BinaryExpression mul))
                return null;
            if (!(mul.Right is Constant cScale))
                return null;
            int scale = 0;
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
            public Expression GetDefiningExpression(Identifier id)
            {
                return id;
            }

            public List<Statement> GetDefiningStatementClosure(Identifier id)
            {
                return new List<Statement>();
            }

            public Expression GetValue(Identifier id)
            {
                return id;
            }

            public Expression GetValue(MemoryAccess access, SegmentMap segmentMap)
            {
                return access;
            }

            public Expression GetValue(SegmentedAccess access, SegmentMap segmentMap)
            {
                return access;
            }

            public Expression GetValue(Application appl)
            {
                return appl;
            }

            public bool IsFramePointer(Expression e)
            {
                return false;
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

            public void RemoveExpressionUse(Expression expr)
            {
            }

            public void RemoveIdentifierUse(Identifier id)
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

        public int CompareTo(BackwardSlicerContext that)
        {
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
    /// The visitor methods return a SlicerResult if they've discovered something
    /// interesting. If the SlicerResult is null, it means the visited instruction
    /// doesn't affect the outcome of the jump table and can be ignored.
    /// </remarks>
    public class SliceState :
        RtlInstructionVisitor<SlicerResult>,
        ExpressionVisitor<SlicerResult, BackwardSlicerContext>
    {
        private BackwardSlicer slicer;
        public RtlBlock block;
        public int iInstr;              // The current instruction
        public RtlInstruction[] instrs; // The instructions of this block
        public Address addrSucc;        // the block from which we traced.
        public ConditionCode ccNext;    // The condition code that is used in a branch.
        public Expression assignLhs;    // current LHS
        public bool invertCondition;
        public Dictionary<Expression, BackwardSlicerContext> Live;
        private int blockCount;

        public SliceState(BackwardSlicer slicer, RtlBlock block, int iInstr)
        {
            this.slicer = slicer;
            this.block = block;
            this.instrs = slicer.host.GetBlockInstructions(block).ToArray();
            this.iInstr = iInstr;
            DumpBlock(BackwardSlicer.trace.TraceVerbose);
        }

        // The RTL expression in the executable that computes the destination addresses.
        public Expression JumpTableFormat { get; private set; }

        // An expression that tests the index 
        public Expression JumpTableIndex { get; private set; }
        // An expression that tests the index that should be used in the resulting source code.
        public Expression JumpTableIndexToUse { get; private set; }
        // the 'stride' of the jump/call table.
        public StridedInterval JumpTableIndexInterval { get; private set; }

        /// <summary>
        /// Start the analysis with the expression in the indirect jump.
        /// Detect any live expressions, and place them in the `Live`
        /// collection.
        /// </summary>
        /// <param name="indirectJump"></param>
        /// <returns></returns>
        public bool Start(Expression indirectJump)
        {
            var sr = indirectJump.Accept(this, BackwardSlicerContext.Jump(RangeOf(indirectJump)));
            if (JumpTableFormat == null)
            {
                JumpTableFormat = indirectJump;
            }

            this.Live = sr.LiveExprs;
            if (!sr.LiveExprs.Keys.OfType<Identifier>().Any())
            {
                // Couldn't find any indirect registers, so there is no work to do.
                DebugEx.Warn(BackwardSlicer.trace, "Bwslc: No indirect registers?");
                return false;
            }
            DebugEx.Verbose(BackwardSlicer.trace, "  live: {0}", DumpLive(this.Live));
            return true;
        }

        public bool Step()
        {
            var instr = this.instrs[this.iInstr];
            DebugEx.Inform(BackwardSlicer.trace, "Bwslc: Stepping to instruction {0}", instr);
            var sr = instr.Accept(this);
            --this.iInstr;
            if (sr == null)
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
                DebugEx.Verbose(BackwardSlicer.trace, "  Was asked to stop, stopping.");
                DebugEx.Verbose(BackwardSlicer.trace, "  index: {0} ({1})", this.JumpTableIndex, this.JumpTableIndexInterval);
                DebugEx.Verbose(BackwardSlicer.trace, "  expr:  {0}", this.JumpTableFormat);
                return false;
            }
            if (this.Live.Count == 0)
            {
                DebugEx.Verbose(BackwardSlicer.trace, "  No more live expressions, stopping.");
                DebugEx.Verbose(BackwardSlicer.trace, "  index: {0} ({1})", this.JumpTableIndex, this.JumpTableIndexInterval);
                DebugEx.Verbose(BackwardSlicer.trace, "  expr:  {0}", this.JumpTableFormat);
                return false;
            }
            DebugEx.Verbose(BackwardSlicer.trace, "  live: {0}", this.DumpLive(this.Live));
            return true;
        }

        public bool IsInBeginningOfBlock()
        {
            return iInstr < 0;
        }

        private StorageDomain DomainOf(Expression e)
        {
            if (e is Identifier id)
            {
                return id.Storage.Domain;
            }
            return StorageDomain.Memory;
        }

        private StridedInterval MakeInterval_ISub(Expression left, Constant right)
        {
            if (right == null)
                return StridedInterval.Empty;
            var cc = this.ccNext;
            if (this.invertCondition)
                cc = cc.Invert();
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
            case ConditionCode.LE: return StridedInterval.Create(1, 0, right.ToInt64());
            case ConditionCode.LT: return StridedInterval.Create(1, 0, right.ToInt64() - 1);
            case ConditionCode.ULE: return StridedInterval.Create(1, 0, (long) right.ToUInt64());
            case ConditionCode.ULT: return StridedInterval.Create(1, 0, (long) right.ToUInt64() - 1);
            case ConditionCode.UGE: return StridedInterval.Create(1, (long)right.ToUInt64(), long.MaxValue);
            case ConditionCode.UGT: return StridedInterval.Create(1, (long)right.ToUInt64() + 1, long.MaxValue);
            default:
                return StridedInterval.Empty;
            }
        }

        private StridedInterval MakeInterval_And(Expression left, Constant right)
        {
            if (right == null)
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

        public SlicerResult VisitAssignment(RtlAssignment ass)
        {
            if (!(ass.Dst is Identifier id))
            {
                // Ignore writes to memory.
                return null;
            }
            this.assignLhs = ass.Dst;
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
            if (se == null)
                return se;
            if (se.SrcExpr != null)
            {
            var newJt = ExpressionReplacer.Replace(assignLhs, se.SrcExpr, JumpTableFormat);
            this.JumpTableFormat = slicer.Simplify(newJt);
            }
            DebugEx.Verbose(BackwardSlicer.trace, "  expr:  {0}", this.JumpTableFormat);
            this.assignLhs = null;
            return se;
        }

        public SlicerResult VisitBinaryExpression(BinaryExpression binExp, BackwardSlicerContext ctx)
        {
            if (binExp.Operator == Operator.Eq || binExp.Operator == Operator.Ne)
            {
                // Equality comparisons cannot contribute to determining the size
                // of the jump table; stop processing this instruction.
                return null;
            }

            if ((binExp.Operator == Operator.Xor || binExp.Operator == Operator.ISub) &&
                this.slicer.AreEqual(binExp.Left, binExp.Right))
            {
                // XOR r,r (or SUB r,r) clears a register. Is it part of a live register?
                var regDst = this.assignLhs as Identifier;
                var regHi = binExp.Left as Identifier;
                if (regHi != null && regDst != null &&
                    DomainOf(regDst) == regHi.Storage.Domain &&
                    regDst.Storage.OffsetOf(regHi.Storage) == 8)
                {
                    // The 8086 didn't have a MOVZX instruction, so clearing the high byte of a
                    // register BX was done by issuing XOR BH,BH
                    var seXor = new SlicerResult
                    {
                        SrcExpr = new Cast(regDst.DataType, new Cast(PrimitiveType.Byte, this.assignLhs)),
                        LiveExprs = new Dictionary<Expression, BackwardSlicerContext>
                        {
                            {
                                this.assignLhs,
                                BackwardSlicerContext.Jump(new BitRange(0, 8))
                            }
                        }
                    };
                    return seXor;
                }
            }

            var seLeft = binExp.Left.Accept(this, ctx);
            var seRight = binExp.Right.Accept(this, ctx);
            if (seLeft == null && seRight == null)
                return null;
            if (binExp.Operator == Operator.ISub && this.Live != null && (ctx.Type & ContextType.Condition) != 0)
            {
                var domLeft = DomainOf(seLeft.SrcExpr);

                if (Live.Count > 0)
                {
                foreach (var live in Live)
                {
                    if (live.Value.Type != ContextType.Jumptable)
                        continue;
                    if ((domLeft != StorageDomain.Memory && DomainOf(live.Key) == domLeft)
                        ||
                        (this.slicer.AreEqual(live.Key, binExp.Left)))
                    {
                        //$TODO: if jmptableindex and jmptableindextouse not same, inject a statement.
                        this.JumpTableIndex = live.Key;
                        this.JumpTableIndexToUse = binExp.Left;
                        this.JumpTableIndexInterval = MakeInterval_ISub(live.Key, binExp.Right as Constant);
                        DebugEx.Verbose(BackwardSlicer.trace, "  Found range of {0}: {1}", live, JumpTableIndexInterval);
                        return new SlicerResult
                        {
                            SrcExpr = binExp,
                            Stop = true
                        };
                    }
                }
            }
                else
                {
                    // We have no live variables, which means this subtraction instruction
                    // is both computing the jumptable index and also performing the 
                    // comparison.
                    this.JumpTableIndex = assignLhs;
                    this.JumpTableIndexToUse = assignLhs;
                    this.JumpTableIndexInterval = MakeInterval_ISub(assignLhs, binExp.Right as Constant);
                    DebugEx.Verbose(BackwardSlicer.trace, "  Found range of {0}: {1}", assignLhs, JumpTableIndexInterval);
                    return new SlicerResult
                    {
                        SrcExpr = null,     // the jump table expression already has the correct shape.
                        Stop = true
                    };
                }
            }
            else if (binExp.Operator == Operator.And)
            {
                this.JumpTableIndex = binExp.Left;
                this.JumpTableIndexToUse = binExp.Left;
                this.JumpTableIndexInterval = MakeInterval_And(binExp.Left, binExp.Right as Constant);
                return new SlicerResult
                {
                    SrcExpr = binExp,
                    Stop = true,
                };
            }
            IEnumerable<KeyValuePair<Expression,BackwardSlicerContext>> liveExpr = seLeft.LiveExprs;
            if (seRight != null)
                liveExpr = liveExpr.Concat(seRight.LiveExprs);
            var se = new SlicerResult
            {
                LiveExprs = liveExpr
                    .GroupBy(e => e.Key)
                    .ToDictionary(k => k.Key, v => v.Max(vv => vv.Value)),
                SrcExpr = binExp,
            };
            return se;
        }

        public SlicerResult VisitBranch(RtlBranch branch)
        {
            var se = branch.Condition.Accept(this, BackwardSlicerContext.Cond(new BitRange(0, 0)));
            var addrTarget = branch.Target as Address;
            if (addrTarget == null)
                throw new NotImplementedException();    //$REVIEW: do we ever see this?
            if (this.addrSucc != addrTarget)
            {
                this.invertCondition = true;
            }
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

        public SlicerResult VisitCast(Cast cast, BackwardSlicerContext ctx)
        {
            var range = new BitRange(0, (short)cast.DataType.BitSize);
            return cast.Expression.Accept(this, new BackwardSlicerContext(ctx.Type, range));
        }

        public SlicerResult VisitConditionalExpression(ConditionalExpression c, BackwardSlicerContext ctx)
        {
            return null;
        }

        public SlicerResult VisitConditionOf(ConditionOf cof, BackwardSlicerContext ctx)
        {
            var se = cof.Expression.Accept(this, BackwardSlicerContext.Cond(RangeOf(cof.Expression)));
            if (se != null && !se.Stop)
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

        public SlicerResult VisitDepositBits(DepositBits d, BackwardSlicerContext ctx)
        {
            var srSrc = d.Source.Accept(this, ctx);
            var brBits = RangeOf(d.InsertedBits);
            var srBits = d.InsertedBits.Accept(this, new BackwardSlicerContext(ctx.Type, brBits));
            if (brBits == ctx.BitRange)
            {
                return new SlicerResult
                {
                    SrcExpr = d.InsertedBits,
                    LiveExprs = srBits.LiveExprs
                };
            }
            else
            {
                return new SlicerResult
                {
                    SrcExpr = new DepositBits(d.Source, d.InsertedBits, d.BitPosition),
                    LiveExprs = srSrc.LiveExprs.Concat(srBits.LiveExprs)
                        .GroupBy(e => e.Key)
                        .ToDictionary(k => k.Key, v => v.Max(vv => vv.Value)),
                };
            }
        }

        public SlicerResult VisitDereference(Dereference deref, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }


        public SlicerResult VisitFieldAccess(FieldAccess acc, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitGoto(RtlGoto go)
        {
            var sr = go.Target.Accept(this, BackwardSlicerContext.Cond(RangeOf(go.Target)));
            if (JumpTableFormat == null)
            {
                JumpTableFormat = go.Target;
            }
            return sr;
        }

        private BitRange RangeOf(Expression expr)
        {
            return new BitRange(0, (short)expr.DataType.BitSize);
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
            var srEa = access.EffectiveAddress.Accept(this, new BackwardSlicerContext(ctx.Type, rangeEa));
            srEa.LiveExprs[access] = new BackwardSlicerContext(ctx.Type, ctx.BitRange);
            return new SlicerResult
            {
                LiveExprs = srEa.LiveExprs,
                SrcExpr = srEa.SrcExpr != null ? new MemoryAccess(srEa.SrcExpr, access.DataType) : null,
                Stop = srEa.Stop
            };
        }

        public SlicerResult VisitMkSequence(MkSequence seq, BackwardSlicerContext ctx)
        {
            var srExprs = seq.Expressions
                .Select(e => e.Accept(this, ctx))
                .ToArray();
            return new SlicerResult
            {
                LiveExprs = srExprs[1].LiveExprs,
                SrcExpr = new MkSequence(seq.DataType, srExprs.Select(s => s.SrcExpr).ToArray()),
                Stop = srExprs.Any(s => s.Stop)
            };
        }

        public SlicerResult VisitNop(RtlNop rtlNop)
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
            throw new NotImplementedException();
        }

        public SlicerResult VisitScopeResolution(ScopeResolution scopeResolution, BackwardSlicerContext ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitSegmentedAccess(SegmentedAccess access, BackwardSlicerContext ctx)
        {
            var sr = access.EffectiveAddress.Accept(this, ctx);
            return sr;
        }

        public SlicerResult VisitSideEffect(RtlSideEffect side)
        {
            return new SlicerResult
            {
                Stop = true
            };
        }

        public SlicerResult VisitSlice(Slice slice, BackwardSlicerContext ctx)
        {
            var range = new BitRange(
                (short)slice.Offset,
                (short) (slice.DataType.BitSize + slice.Offset));
            return slice.Expression.Accept(this, new BackwardSlicerContext(ctx.Type, range));
        }

        public SlicerResult VisitTestCondition(TestCondition tc, BackwardSlicerContext ctx)
        {
            var se = tc.Expression.Accept(this, BackwardSlicerContext.Cond(RangeOf(tc.Expression)));
            this.ccNext = tc.ConditionCode;
            return se;
        }

        public SlicerResult VisitUnaryExpression(UnaryExpression unary, BackwardSlicerContext ctx)
        {
            var sr = unary.Expression.Accept(this, ctx);
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

        private string DumpLive(Dictionary<Expression, BackwardSlicerContext> live)
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
            foreach (var i in instrs)
            {
                sw.Write("    ");
                i.Write(sw);
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
    }

    public class SlicerResult
    {
        // Live storages are involved in the computation of the jump destinations.
        public Dictionary<Expression, BackwardSlicerContext> LiveExprs = new Dictionary<Expression, BackwardSlicerContext>();
        public Expression SrcExpr;
        public bool Stop;       // Set to true if the analysis should stop.
    }

    public class TableExtent
    {
        public List<Address> Targets;
        public Dictionary<Address, DataType> Accesses;
        public Expression Index;
    }
}
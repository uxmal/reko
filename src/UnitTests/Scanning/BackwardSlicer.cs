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
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System.Collections.Generic;
using System;
using Reko.Core.Types;
using Reko.Scanning;
using System.Linq;
using System.Diagnostics;
using Reko.Core.Operators;

namespace Reko.UnitTests.Scanning
{
    public class BackwardSlicer : RtlInstructionVisitor<SlicerResult>, ExpressionVisitor<SlicerResult, BitRange>
    {
        private RtlBlock block;
        private int iInstr;
        private IBackWalkHost<RtlBlock, RtlInstruction> host;
        private List<RtlInstruction> instrs;
        private Address addrSucc;       // the block from which we traced.
        private ConditionCode ccNext;   // The condition code that is used in a branch.
        private ExpressionValueComparer cmp;
        private Expression assignLhs;   // current LHS
        private bool invertCondition;

        public BackwardSlicer(RtlBlock block, IBackWalkHost<RtlBlock, RtlInstruction> host)
        {
            this.block = block;
            this.host = host;
            var b = block;
            this.instrs = FlattenInstructions(b);
            this.Roots = new Dictionary<Expression, BitRange>();
            this.cmp = new ExpressionValueComparer();
        }

        public Dictionary<Expression, BitRange> Roots { get; private set; }
        public Dictionary<Expression, BitRange> Live { get; private set; }
        public Expression JumpTableFormat { get; private set; }  // an expression that computes the destination addresses.

        public Expression JumpTableIndex { get; private set; }    // an expression that tests the index 
        public StridedInterval JumpTableIndexInterval { get; private set; }    // an expression that tests the index 

        private static List<RtlInstruction> FlattenInstructions(RtlBlock b)
        {
            return b.Instructions.SelectMany(rtlc => rtlc.Instructions).ToList();
        }

        public bool Start()
        {
            this.Roots.Clear();

            this.iInstr = instrs.Count - 1;
            var sr = instrs[iInstr].Accept(this);
            if (sr.LiveExprs.Count == 0)
            {
                Debug.Print("No indirect registers?");
                return false;
            }
            this.Roots = new Dictionary<Expression, BitRange>(sr.LiveExprs);
            this.Live = sr.LiveExprs;
            return true;
        }

        public bool Step()
        {
            --iInstr;
            while (iInstr < 0)
            {
                var pred = host.GetSinglePredecessor(block);    //$TODO: do all predecessors, add to queue.
                if (pred == null)
                    return false;
                this.addrSucc = block.Address;
                block = pred;
                instrs = FlattenInstructions(block);
                iInstr = instrs.Count - 1;
            }
            var sr = instrs[iInstr].Accept(this);
            if (sr == null)
                return false;
            foreach (var de in sr.LiveExprs)
            {
                this.Live[de.Key] = de.Value;
            }
            return true;
        }

        private StridedInterval MakeInterval_ISub(Expression left, Constant right)
        {
            if (right == null)
                return StridedInterval.Empty;
            var cc = ccNext;
            if (invertCondition)
                cc = cc.Invert();
            switch (cc)
            {
            case ConditionCode.ULE: return StridedInterval.Create(1, 0, right.ToInt64());
            default: throw new NotImplementedException($"Unimplemented condition code {cc}.");
            }
        }

        public SlicerResult VisitAddress(Address addr, BitRange ctx)
        {
            return new SlicerResult
            {
                Addresses = { addr },
            };
        }

        public SlicerResult VisitApplication(Application appl, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitArrayAccess(ArrayAccess acc, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitAssignment(RtlAssignment ass)
        {
            var id = ass.Dst as Identifier;
            if (id != null)
            {
                bool wasLive = Live.Remove(id); 
                if (!wasLive)
                {
                    // This assignment doesn't affect the end result.
                    return new SlicerResult();   
                }
                //$TODO: create edges in graph. storages....
            }
            this.assignLhs = ass.Dst;
            var se = ass.Src.Accept(
                this,
                new BitRange(
                    (short)id.Storage.BitAddress,
                    (short)(id.Storage.BitAddress + id.Storage.BitSize)));
            this.JumpTableFormat = ExpressionReplacer.Replace(ass.Dst, ass.Src, JumpTableFormat);
            this.assignLhs = null;
            return se;
        }

        public SlicerResult VisitBinaryExpression(BinaryExpression binExp, BitRange ctx)
        {
            var seLeft = binExp.Left.Accept(this, ctx);
            var seRight = binExp.Right.Accept(this, ctx);
            var se = new SlicerResult
            {
                Addresses = seLeft.Addresses.Concat(seRight.Addresses).ToHashSet(),
                LiveExprs = seLeft.LiveExprs.Concat(seRight.LiveExprs)
                    .ToDictionary(k => k.Key, v => v.Value)
            };
            return se;
        }

        public SlicerResult VisitBranch(RtlBranch branch)
        {
            var se = branch.Condition.Accept(this, new BitRange(0, 0));
            var addrTarget = branch.Target as Address;
            if (addrTarget == null)
                throw new NotImplementedException();    //#REVIEW: do we ever see this?
            if (addrSucc != addrTarget)
            {
                this.invertCondition = true;
            }
            return se;
        }

        public SlicerResult VisitCall(RtlCall call)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitCast(Cast cast, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitConditionalExpression(ConditionalExpression c, BitRange context)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitConditionOf(ConditionOf cof, BitRange ctx)
        {
            var bin = cof.Expression as BinaryExpression;
            if (bin != null)
            {
                if (bin.Operator == Operator.ISub)
                {
                    foreach (var live in Live.Keys)
                    {
                        if (cmp.Equals(live, bin.Left))
                        {
                            if (cmp.Equals(assignLhs, this.JumpTableIndex))
                            {
                                this.JumpTableIndexInterval = MakeInterval_ISub(bin.Left, bin.Right as Constant);
                                return null;
                            }
                        }
                    }
                }
                else
                    throw new NotImplementedException();
            }
            var se = cof.Expression.Accept(this, RangeOf(cof.Expression.DataType));
            this.JumpTableIndex = cof.Expression;
            return se;
        }
      

        public SlicerResult VisitConstant(Constant c, BitRange ctx)
        {
            var addr = host.MakeAddressFromConstant(c);
            var addresses = new HashSet<Address>();
            if (host.IsValidAddress(addr))
            {
                addresses.Add(addr);
            }
            return new SlicerResult
            {
                Addresses = addresses,
                LiveExprs = new Dictionary<Expression, BitRange>(),
            };
        }

        public SlicerResult VisitDepositBits(DepositBits d, BitRange ctx)
        {
            throw new NotImplementedException();
        }


        public SlicerResult VisitDereference(Dereference deref, BitRange ctx)
        {
            throw new NotImplementedException();
        }


        public SlicerResult VisitFieldAccess(FieldAccess acc, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitGoto(RtlGoto go)
        {
            var sr = go.Target.Accept(this, RangeOf(go.Target.DataType));
            if (JumpTableFormat == null)
            {
                JumpTableFormat = go.Target;
            }
            return sr;
        }

        private BitRange RangeOf(DataType dt)
        {
            return new BitRange(0, (short)dt.BitSize);
        }

        public SlicerResult VisitIdentifier(Identifier id, BitRange ctx)
        {
            var sr = new SlicerResult
            {
                LiveExprs = { { id, ctx } }
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

        public SlicerResult VisitMemberPointerSelector(MemberPointerSelector mps, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitMemoryAccess(MemoryAccess access, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitMkSequence(MkSequence seq, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitNop(RtlNop rtlNop)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitOutArgument(OutArgument outArgument, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitPhiFunction(PhiFunction phi, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitPointerAddition(PointerAddition pa, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitProcedureConstant(ProcedureConstant pc, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitReturn(RtlReturn ret)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitScopeResolution(ScopeResolution scopeResolution, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitSegmentedAccess(SegmentedAccess access, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitSideEffect(RtlSideEffect side)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitSlice(Slice slice, BitRange ctx)
        {
            throw new NotImplementedException();
        }

        public SlicerResult VisitTestCondition(TestCondition tc, BitRange ctx)
        {
            var se = tc.Expression.Accept(this, RangeOf(tc.Expression.DataType));
            this.ccNext = tc.ConditionCode;
            this.JumpTableIndex = tc.Expression;
            return se;
        }

        public SlicerResult VisitUnaryExpression(UnaryExpression unary, BitRange ctx)
        {
            throw new NotImplementedException();
        }
    }

    public struct BitRange
    {
        public readonly short begin;
        public readonly short end;

        public BitRange(short begin, short end)
        {
            this.begin = begin;
            this.end = end;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is BitRange)
            {
                var that = (BitRange)obj;
                return this.begin == that.begin && this.end == that.end;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return begin.GetHashCode() ^ end.GetHashCode() * 5;
        }

        public static bool operator == (BitRange a, BitRange b)
        {
            return a.begin == b.begin && a.end == b.end;
        }

        public static bool operator !=(BitRange a, BitRange b)
        {
            return a.begin != b.begin || a.end != b.end;
                
        }

        public override string ToString()
        {
            return $"[{begin}-{end})";
        }
    }

    public class SlicerResult
    {
        // Live storages are involved in the computation of the jump destinations.
        public Dictionary<Expression, BitRange> LiveExprs = new Dictionary<Expression, BitRange>();

        public HashSet<Address> Addresses = new HashSet<Address>();
    }
}
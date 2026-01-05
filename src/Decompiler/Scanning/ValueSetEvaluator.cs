#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// Evaluates IR expressions using value sets. The result of evaluating 
    /// an expression is another value set.
    /// </summary>
    /// <remarks>
    /// As a side effect, this evaluator tracks memory accesses and generates
    /// type information about them.
    /// </remarks>
    public class ValueSetEvaluator : ExpressionVisitor<ValueSet, BitRange>
    {
        private const int MaxTransferTableEntries = 2000;

        private readonly IProcessorArchitecture arch;
        private readonly IMemory memory;
        private readonly ExpressionEmitter m;
        private readonly Dictionary<Expression, ValueSet> context;
        private readonly ProcessorState? state;
        private readonly ExpressionValueComparer cmp;
        private readonly Dictionary<Address, DataType> memAccesses;

        /// <summary>
        /// Constructs a new <see cref="ValueSetEvaluator"/> instance.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture" /> to use.</param>
        /// <param name="memory"><see cref="IMemory"/> instance.</param>
        /// <param name="m">Expression emitter.</param> 
        /// <param name="context">Previously evaluated value sets.</param>
        /// <param name="state">Processor state used for evaluation.</param>
        public ValueSetEvaluator(
            IProcessorArchitecture arch,
            IMemory memory,
            ExpressionEmitter m,
            Dictionary<Expression, ValueSet> context,
            ProcessorState? state = null)
        {
            Debug.Assert(memory is not null);
            this.arch = arch;
            this.memory = memory;
            this.m = m;
            this.context = context;
            this.state = state;
            this.cmp = new ExpressionValueComparer();
            this.memAccesses = [];
        }

        /// <summary>
        /// Evaluates the given expression and returns a value set.
        /// </summary>
        /// <param name="expr">Expression to evaluate.</param>
        /// <returns>A tuple consisting of the resulting value set
        /// and a dictionary of all the memory accesses done during
        /// the evaluation.
        /// </returns>
        public (ValueSet, Dictionary<Address,DataType>) Evaluate(Expression expr)
        {
            var bitrange = new BitRange(0, (short)expr.DataType.BitSize);
            var values = expr.Accept(this, bitrange);
            return (values, this.memAccesses);
        }

        /// <inheritdoc/>
        public ValueSet VisitAddress(Address addr, BitRange bitRange)
        {
            return new ConcreteValueSet(addr.DataType, addr.ToConstant());
        }

        /// <inheritdoc/>
        public ValueSet VisitApplication(Application appl, BitRange bitRange)
        {
            return IntervalValueSet.Any;
        }

        /// <inheritdoc/>
        public ValueSet VisitArrayAccess(ArrayAccess acc, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueSet VisitBinaryExpression(BinaryExpression binExp, BitRange bitRange)
        {
            var cLeft = binExp.Left as Constant;
            var cRight = binExp.Right as Constant;
            //$TODO: it would be great if Address were simply a Constant.
            // but we have segmented addresses which need special treatment
            // everywhere.
            if (binExp.Left is Address aLeft)
                cLeft = aLeft.ToConstant();
            if (binExp.Right is Address aRight)
                cRight = aRight.ToConstant();

            if (cLeft is not null && cRight is not null)
            {
                return new IntervalValueSet(
                    cLeft.DataType,
                    StridedInterval.Constant(
                        binExp.Operator.ApplyConstants(cLeft.DataType, cLeft, cRight)));
            }

            if (cLeft is null && cRight is not null)
            {
                var left = binExp.Left.Accept(this, bitRange);
                switch (binExp.Operator.Type)
                {
                case OperatorType.IAdd: return left.Add(cRight);
                case OperatorType.And: return left.And(cRight);
                case OperatorType.Shl: return left.Shl(cRight);
                case OperatorType.IMul: return left.IMul(cRight);
                case OperatorType.ISub:return left.Sub(cRight);
                }
            }
            if (cRight is null && cLeft is not null)
            {
                var right = binExp.Right.Accept(this, bitRange);
                switch (binExp.Operator.Type)
                {
                case OperatorType.IAdd:
                    return right.Add(cLeft);
                case OperatorType.And:
                    return right.And(cLeft);
                }
            }
            if (binExp.Operator.Type == OperatorType.IAdd)
            {
                if (cmp.Equals(binExp.Left, binExp.Right))
                {
                    var left = binExp.Left.Accept(this, bitRange);
                    return left.Shl(Constant.Int32(1));
                }
            }
            return IntervalValueSet.Any;
        }

        /// <inheritdoc/>
        public ValueSet VisitCast(Cast cast, BitRange bitRange)
        {
            if (this.context.TryGetValue(cast, out ValueSet? vs))
                return vs;
            var bitRangeNarrow = new BitRange(0, (short)cast.DataType.BitSize);
            vs = cast.Expression.Accept(this, bitRangeNarrow);
            if (cast.DataType.BitSize == cast.Expression.DataType.BitSize)
            {
                // no-op!
                return vs;
            }
            if (cast.DataType.BitSize < cast.Expression.DataType.BitSize)
            {
                return vs.Truncate(cast.DataType);
            }
            if (cast.DataType.Domain == Domain.SignedInt)
            {
                return vs.SignExtend(cast.DataType);
            }
            return vs.ZeroExtend(cast.DataType);
        }

        /// <inheritdoc/>
        public ValueSet VisitConditionalExpression(ConditionalExpression cond, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueSet VisitConditionOf(ConditionOf cof, BitRange bitRange)
        {
            return ValueSet.Any;
        }

        /// <inheritdoc/>
        public ValueSet VisitConstant(Constant c, BitRange bitRange)
        {
            return new ConcreteValueSet(c.DataType, c);
        }

        /// <inheritdoc/>
        public ValueSet VisitConversion(Conversion conversion, BitRange bitRange)
        {
            if (this.context.TryGetValue(conversion, out ValueSet? vs))
                return vs;
            var bitRangeNarrow = new BitRange(0, (short) conversion.DataType.BitSize);
            vs = conversion.Expression.Accept(this, bitRangeNarrow);
            if (conversion.DataType.BitSize == conversion.Expression.DataType.BitSize)
            {
                // no-op!
                return vs;
            }
            if (conversion.DataType.BitSize < conversion.Expression.DataType.BitSize)
            {
                return vs.Truncate(conversion.DataType);
            }
            if (conversion.DataType.Domain == Domain.SignedInt)
            {
                return vs.SignExtend(conversion.DataType);
            }
            return vs.ZeroExtend(conversion.DataType);
        }

        /// <inheritdoc/>
        public ValueSet VisitDereference(Dereference deref, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueSet VisitFieldAccess(FieldAccess acc, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueSet VisitIdentifier(Identifier id, BitRange bitRange)
        {
            if (context.TryGetValue(id, out ValueSet? vs))
                return vs;
            if (state is not null && state.GetValue(id) is Constant c && c.IsValid)
                return new ConcreteValueSet(c.DataType, c);
            return new IntervalValueSet(id.DataType, StridedInterval.Empty);
        }

        /// <inheritdoc/>
        public ValueSet VisitMemberPointerSelector(MemberPointerSelector mps, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluating a memory access forces the creation of a 
        /// Concrete value set.
        /// </summary>
        /// <param name="access">Memory access.</param>
        /// <param name="bitRange">Bit range of the effective address of the memory access.</param>
        /// <returns></returns>
        public ValueSet VisitMemoryAccess(MemoryAccess access, BitRange bitRange)
        {
            if (context.TryGetValue(access, out ValueSet? value))
            {
                return value;
            }
            var eaRange = new BitRange(0, (short)access.EffectiveAddress.DataType.BitSize);
            var vs = access.EffectiveAddress.Accept(this, eaRange);
            return new ConcreteValueSet(
                access.DataType,
                vs.Values
                    .Take(MaxTransferTableEntries)
                    .Select(v => ReadValue(access.DataType, v))
                    .ToArray());
        }

        private Expression ReadValue(DataType dt, Expression eAddr)
        {
            if (eAddr is not Address addr)
            {
                if (eAddr is not Constant cAddr)
                    throw new NotImplementedException($"Don't know how to read from {eAddr}");
                addr = arch.MakeAddressFromConstant(cAddr, false);
            }
            if (!arch.TryCreateImageReader(memory, addr, out var rdr))
                return InvalidConstant.Create(dt);
            memAccesses[addr] = dt;
            if (dt == PrimitiveType.SegPtr32)
            {
                var addrRead = arch.ReadCodeAddress(dt.Size, rdr, null);
                if (addrRead is not null)
                {
                    return addrRead;
                }
                //$REVIEW we want a warning here. OR the caller.
                return InvalidConstant.Create(dt);
            }
            else
            {
                if (!rdr.TryRead((PrimitiveType) dt, out var c))
                    return InvalidConstant.Create(dt);
                else
                    return c;
            }
        }

        /// <inheritdoc/>
        public ValueSet VisitMkSequence(MkSequence seq, BitRange bitRange)
        {
            if (bitRange.Lsb > 0)
            {
                // We seldom encounter this. If we do, we write the code accordingly.
                return ValueSet.Any;
            }
            var valuesets = new List<ValueSet>();
            int nTotalBits = 0;
            for (int i = seq.Expressions.Length-1; i >= 0 && nTotalBits < bitRange.Msb; --i)
            {
                var elem = seq.Expressions[i];
                var elemRange = new BitRange(0, Math.Min((short) elem.DataType.BitSize, (short) (bitRange.Msb - nTotalBits)));
                var vs = elem.Accept(this, elemRange);
                valuesets.Add(vs);
                nTotalBits += elem.DataType.BitSize;
            }
            if (valuesets.Count == 1)
                return valuesets[0];

            valuesets.Reverse();
            var consts = new Expression[valuesets.Count];
            for (int i = 0; i < valuesets.Count - 1; ++i)
            {
                var va = valuesets[i];
                if (va == ValueSet.Any)
                    return va;
                var aVa = va.Values.ToArray();
                if (aVa.Length != 1)
                    return ValueSet.Any;
                if (aVa[0] is Constant c)
                    consts[i] = c;
                else
                    return ValueSet.Any;
            }

            var vsTail = valuesets[^1];
            return new ConcreteValueSet(
                vsTail.DataType,
                vsTail.Values
                      .Select(v => MakeSequence(seq.DataType, consts, v))
                      .ToArray());
        }

        private Expression MakeSequence(DataType dataType, Expression [] exps, Expression off)
        {
            if (exps.Length == 2 &&
                exps[0] is Constant cSeg &&
                cSeg.DataType == PrimitiveType.SegmentSelector)
            {
                // Special case for segmented pointers.
                //$TODO: we have SegmentedAddress, let's use that.
                return arch.MakeSegmentedAddress(cSeg, (Constant) off); 
            }
            exps[^1] = (Constant) off;
            return m.Seq(dataType, exps);
        }

        /// <inheritdoc/>
        public ValueSet VisitOutArgument(OutArgument outArgument, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueSet VisitPhiFunction(PhiFunction phi, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueSet VisitPointerAddition(PointerAddition pa, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueSet VisitProcedureConstant(ProcedureConstant pc, BitRange bitRange)
        {
            Address addr;
            switch (pc.Procedure)
            {
            case Procedure proc:
                addr = proc.EntryAddress;
                var cAddr = addr.ToConstant();
                return new ConcreteValueSet(pc.DataType, cAddr);
            default:
                return ValueSet.Any;
            }
        }

        /// <inheritdoc/>
        public ValueSet VisitScopeResolution(ScopeResolution scopeResolution, BitRange bitRange)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueSet VisitSegmentedAddress(SegmentedPointer address, BitRange bitRange)
        {
            Expression MakeAddress(DataType dataType, Constant selector, Expression offset)
            {
                var cOffset = (Constant) offset;
                return arch.MakeSegmentedAddress(selector, cOffset);
            }

            if (context.TryGetValue(address, out ValueSet? value))
            {
                return value;
            }
            var vs = address.Offset.Accept(this, bitRange);

            var vaSeg = address.BasePointer.Accept(this, bitRange);
            if (vaSeg == ValueSet.Any)
                return vaSeg;
            var segs = vaSeg.Values.ToArray();
            if (segs.Length != 1)
                return ValueSet.Any;
            if (segs[0] is not Constant cSeg)
                return ValueSet.Any;

            var vsOff = address.Offset.Accept(this, bitRange);
            return new ConcreteValueSet(
                vsOff.DataType,
                vsOff.Values
                    .Select(v => MakeAddress(address.DataType, cSeg, v))
                    .ToArray());
        }

        /// <inheritdoc/>
        public ValueSet VisitSlice(Slice slice, BitRange bitRange)
        {
            if (slice.Offset != 0)
            {
                // This rarely occurs in real-world code. We punt the implementation to when
                // it becomes necessary.
                return ValueSet.Any;
            }
            var bitRangeNarrow = new BitRange((short)slice.Offset, (short) (slice.Offset + slice.DataType.BitSize));
            ValueSet vs = slice.Expression.Accept(this, bitRangeNarrow);

            if (slice.DataType.BitSize == slice.Expression.DataType.BitSize)
            {
                // no-op!
                return vs;
            }
            if (slice.DataType.BitSize < slice.Expression.DataType.BitSize)
            {
                return vs.Truncate(slice.DataType);
            }
            if (slice.DataType.Domain == Domain.SignedInt)
            {
                return vs.SignExtend(slice.DataType);
            }
            return vs.ZeroExtend(slice.DataType);
        }

        /// <inheritdoc/>
        public ValueSet VisitStringConstant(StringConstant str, BitRange bitRange)
        {
            return new ConcreteValueSet(str.DataType, str);
        }

        /// <inheritdoc/>
        public ValueSet VisitTestCondition(TestCondition tc, BitRange bitRange)
        {
            return ValueSet.Any;
        }

        /// <inheritdoc/>
        public ValueSet VisitUnaryExpression(UnaryExpression unary, BitRange bitRange)
        {
            throw new NotImplementedException();
        }
    }
}

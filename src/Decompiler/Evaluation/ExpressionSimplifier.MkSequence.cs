using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitMkSequence(MkSequence seq)
        {
            bool changed = false;
            var newSeq = seq.Expressions.Select(e =>
            {
                var (eNew, eChanged) = e.Accept(this);
                if (eNew is InvalidConstant)
                    eNew = e;
                changed |= eChanged;
                return eNew;
            }).ToArray();
            
            // SEQ(SEQ(a, b), c) = SEQ(a, b, c)
            (newSeq, changed) = FlattenNestedSequences(newSeq, changed);

            if (newSeq.Length == 2)
            {
                // Special case for the frequent case of segment:offset or 
                // two concatenated bit vectors.
                if (newSeq[0] is Constant c1 && newSeq[1] is Constant c2)
                {
                    DataType tHead = c1.DataType;
                    DataType tTail = c2.DataType;
                    PrimitiveType t;
                    if (tHead.Domain == Domain.Selector)            //$REVIEW: seems to require Address, SegmentedAddress?
                    {
                        //$TODO: now that there is a SegmentPointer class available,
                        // consider removing this special case code, since it already
                        // is implemented for SegmentPointer
                        t = PrimitiveType.Create(Domain.SegPointer, tHead.BitSize + tTail.BitSize);
                        return (ctx.MakeSegmentedAddress(c1, c2), true);
                    }
                    else if (tTail.Domain != Domain.Real)
                    {
                        t = PrimitiveType.Create(tHead.Domain, tHead.BitSize + tTail.BitSize);
                        if (tHead.BitSize + tTail.BitSize <= 64)
                            return (Constant.Create(t, (c1.ToUInt64() << tTail.BitSize) | c2.ToUInt64()), true);
                        else
                            return (Constant.Create(t, (c1.ToBigInteger() << tTail.BitSize) | c2.ToBigInteger()), true);
                    }
                }
            }
            else if (newSeq.All(e => e is Constant))
            {
                BigInteger value = BigInteger.Zero;
                for (int i = 0; i < newSeq.Length; ++i)
                {
                    var c = (Constant) newSeq[i];
                    value = (value << c.DataType.BitSize) | c.ToBigInteger();
                }
                return (Constant.Create(seq.DataType, value), true);
            }

            if (!newSeq[^1].DataType.IsReal && newSeq.Take(newSeq.Length - 1).All(e => e.IsZero))
            {
                var tail = newSeq.Last();
                // leading zeros imply a conversion to unsigned.
                return (new Conversion(
                    tail,
                    PrimitiveType.Create(Domain.UnsignedInt, tail.DataType.BitSize),
                    PrimitiveType.Create(Domain.UnsignedInt, seq.DataType.BitSize)),
                    true);
            }
            var mul = FuseSequenceOfSlicedMultiplications(seq.DataType, newSeq);
            if (mul is not null)
                return (mul, true);
            var log = FuseLogicalOperations(seq.DataType, newSeq);
            if (log is not null)
                return (log, true);
            var neg = SignExtendedNegation(seq);
            if (neg is not null)
                return (neg, true);
            var mem = FuseAdjacentMemoryAccesses(seq.DataType, newSeq);
            if (mem != null)
                return (mem, true);
            var slices = FuseAdjacentSlices(seq.DataType, newSeq);
            if (slices != null)
                return (slices, true);
            return (new MkSequence(seq.DataType, newSeq), changed);
        }

        private (Expression[] newSeq, bool changed) FlattenNestedSequences(
            Expression[] sequence,
            bool changed)
        {
            var result = new List<Expression>();
            foreach (var exp in sequence)
            {
                if (exp is MkSequence seq)
                {
                    result.AddRange(seq.Expressions);
                    changed = true;
                }
                else
                {
                    result.Add(exp);
                }
            }
            return (result.ToArray(), changed);
        }

        /// <summary>
        /// The expression SEQ(-0<16> - (x != 0<16>), -x) appears a lot in
        /// 16-bit x86 binaries. It amounts to a zero extension of an unsigned 
        /// integer (like size_t) followed by a negation.
        /// </summary>
        private Expression? SignExtendedNegation(MkSequence seq)
        {
            if (seq.Expressions.Length < 2)
                return null;
            var eLast = seq.Expressions[^1];
            if (eLast is UnaryExpression un && un.Operator.Type == OperatorType.Neg)
            {
                var eFirst = seq.Expressions[^2];
                var negated = un.Expression;
                if (eFirst is BinaryExpression bin &&
                    bin.Operator.Type == OperatorType.ISub &&
                    (bin.Left.IsZero ||
                     bin.Left is UnaryExpression un2 && un2.Operator.Type == OperatorType.Neg && un2.Expression.IsZero) &&
                    bin.Right is BinaryExpression binRight &&
                    binRight.Left == negated &&
                    binRight.Right.IsZero)
                {
                    var bitsize = eFirst.DataType.BitSize + eLast.DataType.BitSize;
                    var unsignedInt = PrimitiveType.Create(Domain.UnsignedInt, eLast.DataType.BitSize);
                    var signedInt = PrimitiveType.Create(Domain.SignedInt, bitsize);
                    return m.Neg(m.Convert(negated, unsignedInt, signedInt));
                }
            }
            return null;
        }

        private Expression? FuseAdjacentMemoryAccesses(DataType dt, Expression[] elems)
        {
            if (elems[0] is not MemoryAccess access)
                return null;

            var (seg, ea, offset) = access.Unpack();
            var offsetFused = offset;
            for (int i = 1; i < elems.Length; ++i)
            {
                if (elems[i] is not MemoryAccess accNew)
                    return null;
                var (segNew, eaNew, offNew) = accNew.Unpack();
                if (cmp.Equals(seg, segNew) &&
                    cmp.Equals(ea, eaNew) &&
                    ctx.Endianness.OffsetsAdjacent(offNew, offset, accNew.DataType.Size))
                {
                    offsetFused = Math.Min(offsetFused, offNew);
                }
                else
                    return null;
            }
            Expression fusedEa;
            if (ea is null)
            {
                var dtEa = (access.EffectiveAddress is SegmentedPointer segptr)
                    ? segptr.Offset.DataType
                    : access.EffectiveAddress.DataType;
                fusedEa = Constant.Create(dtEa, (ulong) offsetFused);
            }
            else
            {
                fusedEa = m.AddSubSignedInt(ea, offsetFused);
            }

            var result = (seg is null)
                ? m.Mem(access.MemoryId, dt, fusedEa)
                : m.SegMem(access.MemoryId, dt, seg, fusedEa);

            foreach (var e in elems)
                ctx.RemoveExpressionUse(e);
            ctx.UseExpression(result);
            return result;
        }

        private Expression? FuseAdjacentSlices(DataType dataType, Expression[] elems)
        {
            var fused = new List<Expression> { AsSlice(elems[0]) ?? elems[0] };
            bool changed = false;
            for (int i = 1; i < elems.Length; ++i)
            {
                Slice? slNext = AsSlice(elems[i]);
                if (fused[^1] is Slice slPrev && slNext != null &&
                    cmp.Equals(slPrev.Expression, slNext.Expression) &&
                    slPrev.Offset == slNext.Offset + slNext.DataType.BitSize)
                {
                    // Fuse the two consecutive slices. 
                    var newSlice = new Slice(
                        PrimitiveType.CreateWord(slPrev.DataType.BitSize + slNext.DataType.BitSize),
                        slNext.Expression,
                        slNext.Offset);
                    (fused[^1], _) = newSlice.Accept(this);
                    changed = true;
                }
                else
                {
                    fused.Add(elems[i]);
                }
            }
            if (changed)
            {
                foreach (var e in elems)
                    ctx.RemoveExpressionUse(e);
                foreach (var f in fused)
                    ctx.UseExpression(f);
                if (fused.Count == 1)
                    return fused[0];
                else
                    return new MkSequence(dataType, fused.ToArray());
            }
            return null;
        }

        private Expression? FuseSequenceOfSlicedMultiplications(DataType dtSeq, Expression[] seq)
        {
            BinaryExpression? mul = null;

            bool AccumulateMul(BinaryExpression bin)
            {
                if (mul is null)
                    mul = bin;
                else if (!cmp.Equals(mul.Left, bin.Left) ||
                        !cmp.Equals(mul.Right, bin.Right))
                    return false;
                else
                    mul = bin;
                return true;
            }
            int bits = 0;
            for (int i = seq.Length - 1; i >= 0; --i)
            {
                switch (seq[i])
                {
                case Slice slice when
                    slice.Expression is BinaryExpression bin &&
                    bin.Operator is IMulOperator:
                    {
                        if (!AccumulateMul(bin))
                            return null;
                        bits += slice.DataType.BitSize;
                        break;
                    }
                case BinaryExpression bin when
                    bin.Operator is IMulOperator:
                    {
                        if (!AccumulateMul(bin))
                            return null;
                        bits += bin.DataType.BitSize;
                        break;
                    }
                default:
                    return null;
                }
            }
            if (mul is null)
                return null;
            foreach (var e in seq)
                ctx.RemoveExpressionUse(e);
            ctx.UseExpression(mul);
            if (dtSeq.BitSize < mul.DataType.BitSize)
            {
                return m.Slice(mul, dtSeq, 0);
            }
            return mul;
        }

        private Expression? FuseSequenceOfSlicedMultiplications(Expression[] seq)
        {
            BinaryExpression? mul = null;
            for (int i = seq.Length - 1; i >= 0; --i)
            {
                if (seq[i] is Slice slice &&
                    slice.Expression is BinaryExpression bin &&
                    bin.Operator is IMulOperator)
                {
                    if (mul is null)
                        mul = bin;
                    else if (!cmp.Equals(mul.Left, bin.Left) ||
                            !cmp.Equals(mul.Right, bin.Right))
                        return null;
                    else
                        mul = bin;
                }
            }
            if (mul is null)
                return null;
            foreach (var e in seq)
                ctx.RemoveExpressionUse(e);
            ctx.UseExpression(mul);
            return mul;
        }


        private Slice? AsSlice(Expression? e)
        {
            if (e is Identifier id)
            {
                e = ctx.GetDefiningExpression(id);
            }
            if (e is Cast c)
            {
                return new Slice(c.DataType, c.Expression, 0);
            }
            else
            {
                return e as Slice;
            }
        }

        public Expression? FuseLogicalOperations(DataType dt, Expression[] exps)
        {
            var bins = new BinaryExpression[exps.Length];
            BinaryOperator? opPrev = null;
            for (int i = 0; i < exps.Length; ++i)
            {
                if (exps[i] is BinaryExpression bin)
                {
                    var op = bin.Operator;
                    if ((op.Type == OperatorType.And || op.Type == OperatorType.Or || op.Type == OperatorType.Xor) &&
                        (opPrev is null || opPrev == op))
                    {
                        bins[i] = bin;
                        opPrev = (BinaryOperator) op;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            var lefts = new Expression[bins.Length];
            var rights = new Expression[bins.Length];
            for (int i = 0; i < bins.Length; ++i)
            {
                lefts[i] = bins[i].Left;
                rights[i] = bins[i].Right;
            }
            var (left, _) = new MkSequence(dt, lefts).Accept(this);
            var (right, _) = new MkSequence(dt, rights).Accept(this);
            return new BinaryExpression(opPrev!, dt, left, right);
        }

    }
}

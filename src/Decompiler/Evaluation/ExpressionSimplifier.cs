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
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Evaluation
{
    /// <summary>
    /// Partially evaluates expressions, using an <see cref="EvaluationContext"/> to obtain the values
    /// of identifiers and optionally modifies the expression being evaluated.
    /// </summary>
    public partial class ExpressionSimplifier : ExpressionVisitor<(Expression, bool)>
    {
        private static readonly IdConstant idConst;
        private static readonly ComparisonConstOnLeft constOnLeft;
        private static readonly AddTwoIdsRule add2ids;
        private static readonly Add_mul_id_c_id_Rule addMici;
        private static readonly ConstConstBin_Rule constConstBin;
        private static readonly IdBinIdc_Rule idBinIdc;
        private static readonly SliceConstant_Rule sliceConst;
        private static readonly SliceMem_Rule sliceMem;
        private static readonly SliceSegmentedPointer_Rule sliceSegPtr;
        private static readonly SliceShift sliceShift;
        private static readonly Shl_add_Rule shAdd;
        private static readonly Shl_mul_e_Rule shMul;
        private static readonly ShiftShift_c_c_Rule shiftShift;
        private static readonly NegSub_Rule negSub;
        private static readonly CompSub_Rule compSub;
        private static readonly BinOpWithSelf_Rule binopWithSelf;
        private static readonly ConstDivisionImplementedByMultiplication constDiv;
        private static readonly ConvertConvertRule convertConvertRule;
        private static readonly DistributedCastRule distributedCast;
        private static readonly DistributedConversionRule distributedConvert;
        private static readonly DistributedSliceRule distributedSlice;
        private static readonly MkSeqFromSlices_Rule mkSeqFromSlicesRule;
        private static readonly SliceConvert sliceConvert;
        private static readonly LogicalNotFollowedByNegRule logicalNotFollowedByNeg;
        private static readonly LogicalNotFromArithmeticSequenceRule logicalNotFromBorrow;
        private static readonly UnaryNegEqZeroRule unaryNegEqZero;
        private static readonly ScaledIndexRule scaledIndexRule;

        private readonly IMemory memory;
        private readonly EvaluationContext ctx;
        private readonly ExpressionValueComparer cmp;
        private readonly ExpressionEmitter m;
        private readonly Unifier unifier;
        private readonly IEventListener listener;

        /// <summary>
        /// Creates an <see cref="ExpressionSimplifier"/> that uses the given
        /// <paramref name="memory"/> and <paramref name="ctx"/> to evaluate expressions.
        /// </summary>
        /// <param name="memory"><see cref="IMemory"/> instance modeling program memory.</param>
        /// <param name="ctx"><see cref="EvaluationContext"/> providing whole-program context.</param>
        /// <param name="listener"><see cref="IEventListener"/> used to report diagnostic messages.</param>
        public ExpressionSimplifier(IMemory memory, EvaluationContext ctx, IEventListener listener)
            : this(memory, ctx, new Unifier(), listener)
        {
            // Creating the unifier is slow, so we provide a constructor
            // where the unifier is passed in.
        }

        /// <summary>
        /// Creates an <see cref="ExpressionSimplifier"/> that uses the given
        /// <paramref name="memory"/> and <paramref name="ctx"/> to evaluate expressions.
        /// </summary>
        /// <param name="memory"><see cref="IMemory"/> instance modeling program memory.</param>
        /// <param name="ctx"><see cref="EvaluationContext"/> providing whole-program context.</param>
        /// <param name="unifier"><see cref="Unifier"/> to be used when coalescing an identifier with its 
        /// defining expression.</param>
        /// <param name="listener"><see cref="IEventListener"/> used to report diagnostic messages.</param>
        public ExpressionSimplifier(
            IMemory memory,
            EvaluationContext ctx,
            Unifier unifier,
            IEventListener listener)
        {
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            this.ctx = ctx;
            this.unifier = unifier;
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
            this.m = new ExpressionEmitter();
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitAddress(Address addr)
        {
            return (addr, false);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitCast(Cast cast)
        {
            var (e, changed) = cast.Expression.Accept(this);
            if (changed)
                cast = m.Cast(cast.DataType, e);
            return (cast, changed);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitConditionOf(ConditionOf c)
        {
            Expression e = c;
            bool changed = false;
            // It's unsafe to simplify 'cond(x - y)'. This operation actually
            // requires flags of expression, not expression itself. So
            // ConditionCodeEliminator should do its work before. For instance
            // we have cond(x - y) where x = y + 1 and can't simplify it to
            // cond((y + 1) - y) => cond(1). 'y + 1 > y' can be used as test
            // for overflow.
            if (c.Expression is BinaryExpression bin &&
                bin.Operator.Type == OperatorType.ISub)
            {
                var (left, changedLeft) = bin.Left.Accept(this);
                var (right, changedRight) = bin.Right.Accept(this);
                e = m.Bin(
                    bin.Operator, bin.DataType, left, right);
                changed = changedLeft || changedRight;
            }
/*
            else if (c.Expression is Identifier id)
            {
                //$TODO: having "sea of nodes" representation
                // and global value numbering, would make 
                // this unnecessary.
                var (ee, cc) = c.Expression.Accept(this);
                if (ee is Identifier)
                {
                    e = ee;
                    changed = cc;
                }
            }
*/
            else if (c.Expression is not Identifier id)
            {
                (e, changed) = c.Expression.Accept(this);
            }
            //$REVIEW: if e == 0, then Z flags could be set to 1. But that's architecture specific, so
            // we leave that as an exercise to re reader
            if (changed)
                c = m.Cond(c.DataType, e);
            return (c, changed);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitConstant(Constant c)
        {
            return (c, false);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitDereference(Dereference deref)
        {
            var (e, changed) = deref.Expression.Accept(this);
            if (changed)
                deref = new Dereference(deref.DataType, e);
            return (deref, changed);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitFieldAccess(FieldAccess acc)
        {
            var (e, changed) = acc.Structure.Accept(this);
            if (changed)
                acc = m.Field(acc.DataType, e, acc.Field);
            return (acc, changed);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            //$TODO: this has been disabled.
            //if (mpsRule.Match(mps))
            //{
            //    var (e, _) = mpsRule.Transform().Accept(this);
            //    return 
            //}
            var basePtr = mps.BasePointer.Accept(this);
            var memberPtr = mps.MemberPointer.Accept(this);
            var changed = basePtr.Item2 | memberPtr.Item2;
            if (changed)
                mps = new MemberPointerSelector(mps.DataType, basePtr.Item1, memberPtr.Item1);
            return (mps, changed);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitOutArgument(OutArgument outArg)
        {
            if (outArg.Expression is not Identifier)
            {
                var (exp, change) = outArg.Expression.Accept(this);
                if (change)
                {
                    return (m.Out(outArg.DataType, exp), true);
                }
            }
            return (outArg, false);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitPointerAddition(PointerAddition pa)
        {
            return (pa, false);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitProcedureConstant(ProcedureConstant pc)
        {
            return (pc, false);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitScopeResolution(ScopeResolution sc)
        {
            return (sc, false);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitStringConstant(StringConstant str)
        {
            return (str, false);
        }

        /// <inheritdoc/>
        public virtual (Expression, bool) VisitTestCondition(TestCondition tc)
        {
            return (tc, false);
            
            var (e, changed) = tc.Expression.Accept(this);
            if (e is BinaryExpression {
                Operator: { Type: OperatorType.Or }, 
                Right: Constant { DataType: { Domain: Domain.Boolean } } } bin)
            {
                e = bin.Left;
                changed = true;
            }
            if (changed)
                tc = m.Test(tc.ConditionCode, e);
            return (tc, changed);
        }

        /// <inheritdoc/>
        public static bool IsSequence(
            EvaluationContext ctx, 
            Expression e, 
            [MaybeNullWhen(false)] out MkSequence sequence)
        {
            MkSequence? s;
            if (e is Identifier id)
            {
                s = ctx.GetDefiningExpression(id) as MkSequence;
            }
            else
            {
                s = e as MkSequence;
            }
            sequence = s;
            return s is not null;
        }

        /// <inheritdoc/>
        public Expression? SliceSequence(MkSequence seq, DataType dtSlice, int sliceOffset)
        {
            var bitsUsed = dtSlice.BitSize;
            int bitoffset = 0;
            for (int i = seq.Expressions.Length - 1; i >= 0; --i)
            {
                var elem = seq.Expressions[i];
                var bitsElem = elem.DataType.BitSize;
                var offset = sliceOffset - bitoffset;
                if (0 <= offset && offset + bitsUsed <= bitsElem)
                {
                    var eNew = offset == 0 && bitsUsed == bitsElem
                        ? elem
                        : m.Slice(elem, dtSlice, offset);
                    return eNew;
                }
                bitoffset += bitsElem;
            }
            return null;
        }

        static ExpressionSimplifier()
        {
            idConst = new IdConstant();
            constOnLeft = new ComparisonConstOnLeft();
            add2ids = new AddTwoIdsRule();
            addMici = new Add_mul_id_c_id_Rule();
            idBinIdc = new IdBinIdc_Rule();
            sliceConst = new SliceConstant_Rule();
            sliceMem = new SliceMem_Rule();
            sliceSegPtr = new SliceSegmentedPointer_Rule();
            negSub = new NegSub_Rule();
            compSub = new();
            constConstBin = new ConstConstBin_Rule();
            shAdd = new Shl_add_Rule();
            shMul = new Shl_mul_e_Rule();
            shiftShift = new ShiftShift_c_c_Rule();
            sliceShift = new SliceShift();
            binopWithSelf = new BinOpWithSelf_Rule();
            constDiv = new ConstDivisionImplementedByMultiplication();
            convertConvertRule = new ConvertConvertRule();
            distributedConvert = new DistributedConversionRule();
            distributedCast = new DistributedCastRule();
            distributedSlice = new DistributedSliceRule();
            mkSeqFromSlicesRule = new MkSeqFromSlices_Rule();
            sliceConvert = new SliceConvert();
            logicalNotFollowedByNeg = new LogicalNotFollowedByNegRule();
            logicalNotFromBorrow = new LogicalNotFromArithmeticSequenceRule();
            unaryNegEqZero = new UnaryNegEqZeroRule();
            scaledIndexRule = new ScaledIndexRule();
        }
    }
}

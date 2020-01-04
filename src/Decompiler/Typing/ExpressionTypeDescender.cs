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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Typing
{
    /// <summary>
    /// Pushes type information down from the root of an expression to its leaves.
    /// </summary>
    /// <remarks>
    ///    root
    ///  ↓ /  \ ↓
    /// leaf  leaf
    /// </remarks>
    public class ExpressionTypeDescender : ExpressionVisitor<bool, TypeVariable>
    {
        // Matches the effective address of Mem[p + c] where c is a constant.
        private ExpressionMatcher fieldAccessPattern = new ExpressionMatcher(
            new BinaryExpression(
                Operator.IAdd,
                ExpressionMatcher.AnyDataType(null),
                ExpressionMatcher.AnyExpression("p"),
                ExpressionMatcher.AnyConstant("c")));
        private ExpressionMatcher segFieldAccessPattern = new ExpressionMatcher(
            new MkSequence(
                ExpressionMatcher.AnyDataType(null),
                ExpressionMatcher.AnyExpression("p"),
                ExpressionMatcher.AnyConstant("c")));

        private IPlatform platform;
        private TypeStore store;
        private TypeFactory factory;
        private Unifier unifier;
        private Identifier globals;
        private Dictionary<Identifier,LinearInductionVariable> ivs;

        public ExpressionTypeDescender(Program program, TypeStore store, TypeFactory factory)
        {
            this.platform = program.Platform;
            this.globals = program.Globals;
            this.ivs = program.InductionVariables;
            this.store = store;
            this.factory = factory;
            this.unifier = new DataTypeBuilderUnifier(factory, store);
        }

        public bool VisitAddress(Address addr, TypeVariable tv)
        {
            MeetDataType(addr, tv.DataType);
            return false;
        }

        public bool VisitApplication(Application appl, TypeVariable tv)
        {
            MeetDataType(appl, appl.TypeVariable.DataType);

            appl.Procedure.Accept(this, appl.Procedure.TypeVariable);
            TypeVariable[] paramTypes = new TypeVariable[appl.Arguments.Length];
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                appl.Arguments[i].Accept(this, appl.Arguments[i].TypeVariable);
                paramTypes[i] = appl.Arguments[i].TypeVariable;
            }
            FunctionTrait(appl.Procedure, appl.Procedure.DataType.BitSize, appl.TypeVariable, paramTypes);
            BindActualTypesToFormalTypes(appl);
            return false;
        }

        private FunctionType MatchFunctionPointer(DataType dt)
        {
            var ptr = dt as Pointer;
            if (ptr == null)
                return null;
            return ptr.Pointee as FunctionType;
        }

        private FunctionType ExtractSignature(Expression proc)
        {
            if (proc is ProcedureConstant pc)
                return pc.Procedure.Signature;
            return MatchFunctionPointer(proc.TypeVariable.DataType);
        }

        private void BindActualTypesToFormalTypes(Application appl)
        {
            var sig = ExtractSignature(appl.Procedure);
            if (sig == null)
                return;
            if (appl.Arguments.Length != sig.Parameters.Length)
                throw new InvalidOperationException(
                    string.Format("Call to {0} had {1} arguments instead of the expected {2}.",
                    appl.Procedure, appl.Arguments.Length, sig.Parameters.Length));
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                 MeetDataType(appl.Arguments[i], sig.Parameters[i].DataType);
                sig.Parameters[i].Accept(this, sig.Parameters[i].TypeVariable);
            }
        }

        public void FunctionTrait(Expression function, int funcPtrBitSize, TypeVariable ret, params TypeVariable[] actuals)
        {
            Identifier[] parameters = actuals
                .Select(a => new Identifier("", a, null))
                .ToArray();
            var fn = factory.CreateFunctionType(
                new Identifier("", ret, null),
                parameters);
            var pfn = factory.CreatePointer(fn, funcPtrBitSize);
            MeetDataType(function, pfn);
        }

        public bool VisitArrayAccess(ArrayAccess acc, TypeVariable tv)
        {
            MeetDataType(acc, acc.DataType);
            Expression arr;
            int offset;
            if (fieldAccessPattern.Match(acc.Array))
            {
                arr = fieldAccessPattern.CapturedExpression("p");
                offset = OffsetOf((Constant)fieldAccessPattern.CapturedExpression("c"));
            }
            else if (segFieldAccessPattern.Match(acc.Array))
            {
                arr = segFieldAccessPattern.CapturedExpression("p");
                offset = OffsetOf((Constant)segFieldAccessPattern.CapturedExpression("c"));
            }
            else
            {
                arr = acc.Array;
                offset = 0;
            }
            int stride = 1;
            if (acc.Index is BinaryExpression bIndex && (bIndex.Operator == Operator.IMul || bIndex.Operator == Operator.SMul || bIndex.Operator == Operator.UMul))
            {
                if (bIndex.Right is Constant c)
                {
                    stride = c.ToInt32();
                }
            }
            var tvElement = ArrayField(null, arr, arr.DataType.BitSize, offset, stride, 0, acc.TypeVariable);

            MeetDataType(acc.Array, factory.CreatePointer(tvElement, acc.Array.DataType.BitSize));
            acc.Array.Accept(this, acc.Array.TypeVariable);
            acc.Index.Accept(this, acc.Index.TypeVariable);
            return false;
        }

        /// <summary>
        /// Assert that there is an array field at offset <paramref name="offset"/>
        /// of the structure pointed at by <paramref name="expStruct"/>.
        /// </summary>
        /// <param name="expBase"></param>
        /// <param name="expStruct"></param>
        /// <param name="structPtrSize"></param>
        /// <param name="offset"></param>
        /// <param name="elementSize"></param>
        /// <param name="length"></param>
        /// <param name="tvField"></param>
        /// <returns>A type variable for the array type of the field.</returns>
        private TypeVariable ArrayField(Expression expBase, Expression expStruct, int structPtrBitSize, int offset, int elementSize, int length, TypeVariable tvField)
        {
            var dtElement = factory.CreateStructureType(null, elementSize);
            dtElement.Fields.Add(0, tvField);
            var tvElement = store.CreateTypeVariable(factory);
            tvElement.DataType = dtElement;
            tvElement.OriginalDataType = dtElement;

            DataType dtArray = factory.CreateArrayType(tvElement, length);
            MemoryAccessCommon(expBase, expStruct, offset, dtArray, structPtrBitSize);
            return tvElement;
        }

        public DataType MemoryAccessCommon(Expression eBase, Expression eStructPtr, int offset, DataType dtField, int structPtrBitSize)
        {
            var s = factory.CreateStructureType(null, 0);
            var field = new StructureField(offset, dtField);
            s.Fields.Add(field);

            var pointer = eBase != null && eBase != globals
                ? (DataType)factory.CreateMemberPointer(eBase.TypeVariable, s, structPtrBitSize / DataType.BitsPerByte)
                : (DataType)factory.CreatePointer(s, structPtrBitSize);
            return MeetDataType(eStructPtr, pointer);
        }

        public bool VisitBinaryExpression(BinaryExpression binExp, TypeVariable tv)
        {
            var eLeft = binExp.Left;
            var eRight= binExp.Right;
            if (binExp.Operator == Operator.IAdd)
            {
                var dt = PushAddendDataType(binExp.TypeVariable.DataType, eRight.TypeVariable.DataType);
                if (dt != null)
                    MeetDataType(eLeft, dt);
                dt = PushAddendDataType(binExp.TypeVariable.DataType, eLeft.TypeVariable.DataType);
                if (dt != null)
                    MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.ISub)
            {
                var dt = PushMinuendDataType(binExp.TypeVariable.DataType, eRight.TypeVariable.DataType);
                MeetDataType(eLeft, dt);
                dt = PushSubtrahendDataType(binExp.TypeVariable.DataType, eLeft.TypeVariable.DataType);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.And || binExp.Operator == Operator.Or)
            {
                //$REVIEW: need a push-logical-Data type to push [[a & 3]] = char into its left and right halves.
                var dt = PrimitiveType.CreateWord(tv.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
                MeetDataType(eLeft, dt);
                MeetDataType(eRight, dt);
            }
            else if (
                binExp.Operator == Operator.IMul ||
                binExp.Operator == Operator.IMod)
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).BitSize).MaskDomain(Domain.Boolean | Domain.Integer );
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).BitSize).MaskDomain(Domain.Boolean | Domain.Integer);
                MeetDataType(eRight, dt);
            }
            else if (
                binExp.Operator == Operator.SMul ||
                binExp.Operator == Operator.SDiv)
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).BitSize).MaskDomain(Domain.Boolean | Domain.SignedInt);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).BitSize).MaskDomain(Domain.Boolean | Domain.SignedInt);
                MeetDataType(eRight, dt);
            }
            else if (
                binExp.Operator == Operator.UMul ||
                binExp.Operator == Operator.UDiv)
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).BitSize).MaskDomain(Domain.Boolean | Domain.UnsignedInt);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).BitSize).MaskDomain(Domain.Boolean | Domain.UnsignedInt);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.FAdd ||
                    binExp.Operator == Operator.FSub ||
                    binExp.Operator == Operator.FMul ||
                    binExp.Operator == Operator.FDiv)
            {
                var dt = PrimitiveType.Create(Domain.Real, eLeft.DataType.BitSize);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Real, eRight.DataType.BitSize);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator is SignedIntOperator)
            {
                var dt = PrimitiveType.CreateWord(eRight.TypeVariable.DataType.BitSize).MaskDomain(Domain.SignedInt | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(eRight.TypeVariable.DataType.BitSize).MaskDomain(Domain.SignedInt | Domain.Character);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator is UnsignedIntOperator)
            {
                var dt = PrimitiveType.CreateWord(eRight.TypeVariable.DataType.BitSize).MaskDomain(Domain.Pointer| Domain.UnsignedInt | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(eRight.TypeVariable.DataType.BitSize).MaskDomain(Domain.Pointer | Domain.UnsignedInt|Domain.Character);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.Eq || binExp.Operator == Operator.Ne||
                binExp.Operator == Operator.Xor || binExp.Operator == Operator.Cand ||
                binExp.Operator == Operator.Cor)
            {
                // Not much can be deduced here, except that the operands should have the same size. Earlier passes
                // already did that work, so just continue with the operands.
            } 
            else if (binExp.Operator is RealConditionalOperator)
            {
                // We know leaves must be floats
                var dt = PrimitiveType.Create(Domain.Real, eLeft.DataType.BitSize);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Real, eLeft.DataType.BitSize);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.Shl)
            {
                var dt = PrimitiveType.CreateWord(tv.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Integer, DataTypeOf(eRight).BitSize);
            }
            else if (binExp.Operator == Operator.Shr)
            {
                var dt = PrimitiveType.CreateWord(tv.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.UnsignedInt| Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Integer, DataTypeOf(eRight).BitSize);
            }
            else if (binExp.Operator == Operator.Sar)
            {
                var dt = PrimitiveType.CreateWord(tv.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.SignedInt | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Integer, DataTypeOf(eRight).BitSize);
            }
            else
                throw new TypeInferenceException($"Unhandled binary operator {binExp.Operator} in expression {binExp}.");
            eLeft.Accept(this, eLeft.TypeVariable);
            eRight.Accept(this, eRight.TypeVariable);
            return false;
        }

        private DataType PushAddendDataType(DataType dtSum, DataType dtOther)
        {
            var ptSum = dtSum as PrimitiveType;
            var ptOther = dtOther as PrimitiveType;
            if (ptSum != null && ptSum.Domain == Domain.Pointer || dtSum is Pointer)
            {
                if (ptOther != null && ptOther.Domain == Domain.Pointer || dtOther is Pointer)
                {
                    return PrimitiveType.Create(Domain.SignedInt, dtSum.BitSize);
                }
                if (ptOther != null && (ptOther.Domain & Domain.Integer) != 0)
                {
                    return PrimitiveType.Create(Domain.Pointer, dtSum.BitSize);
                }
            }
            if (dtSum is MemberPointer mpSum)
            {
                if (dtOther is MemberPointer)
                    return PrimitiveType.Create(Domain.SignedInt, dtOther.BitSize);
                if (ptOther != null && (ptOther.Domain & Domain.Integer) != 0)
                {
                    return factory.CreateMemberPointer(mpSum.BasePointer, factory.CreateUnknown(), mpSum.Size);
                }
            }
            if (ptSum != null && ptSum.IsIntegral)
            {
                // With integral types, type information flows only from leaves
                // to root.
                return null;
            }
            if (ptSum != null && ptSum.Domain == Domain.Pointer || dtSum is Pointer)
            {
                return PrimitiveType.Create(Domain.SignedInt, dtSum.BitSize);
            }
            return dtSum;
        }

        private DataType PushMinuendDataType(DataType dtDiff, DataType dtSub)
        {
            var ptDiff = dtDiff as PrimitiveType;
            var ptSub = dtSub.ResolveAs<PrimitiveType>();
            if (dtDiff is Pointer || ptDiff != null && ptDiff.Domain == Domain.Pointer)
            {
                if (ptSub != null && (ptSub.Domain & Domain.Integer) != 0)
                    return PrimitiveType.Create(Domain.Pointer, dtDiff.BitSize);
                throw new TypeInferenceException($"Not handling {dtDiff} and {dtSub} yet.");
            }
            if (dtDiff is MemberPointer || ptDiff != null && ptDiff.Domain == Domain.Offset)
            {
                if (ptSub != null && (ptSub.Domain & Domain.Integer) != 0)
                    return dtDiff;
                throw new TypeInferenceException($"Not handling {dtDiff} and {dtSub} yet.");
            }
            return dtDiff;
        }

        private DataType PushSubtrahendDataType(DataType dtDiff, DataType dtMin)
        {
            var ptDiff = dtDiff as PrimitiveType;
            var ptMin = dtMin as PrimitiveType;
            if (dtDiff is Pointer || ptDiff != null && ptDiff.Domain == Domain.Pointer)
            {
                if (dtMin is Pointer || ptMin != null && ptMin.Domain == Domain.Pointer)
                    return PrimitiveType.Create(Domain.Integer, dtDiff.BitSize);
                throw new TypeInferenceException(string.Format("Not handling {0} and {1} yet", dtDiff, dtMin));
            }
            if (dtDiff is MemberPointer || ptDiff != null && ptDiff.Domain == Domain.Offset)
            {
                if (dtMin is MemberPointer || ptMin != null && ptMin.Domain == Domain.Offset)
                    return PrimitiveType.Create(Domain.Integer, dtDiff.BitSize);
                throw new TypeInferenceException(string.Format("Not handling {0} and {1} yet", dtDiff, dtMin));
            }
            return dtMin;
        }

        public DataType MeetDataType(Expression exp, DataType dt)
        {
            return MeetDataType(exp.TypeVariable, dt);
        }

        public DataType MeetDataType(TypeVariable tvExp, DataType dt)
        { 
            if (dt == PrimitiveType.SegmentSelector)
            {
                var seg = factory.CreateStructureType(null, 0);
                seg.IsSegment = true;
                var ptr = factory.CreatePointer(seg, dt.BitSize);
                dt = ptr;
            } 
            tvExp.DataType = unifier.Unify(tvExp.DataType, dt);
            tvExp.OriginalDataType = unifier.Unify(tvExp.OriginalDataType, dt);
            return tvExp.DataType;
        }

        public bool VisitCast(Cast cast, TypeVariable tv)
        {
            MeetDataType(cast, cast.DataType);
            cast.Expression.Accept(this, cast.Expression.TypeVariable);
            return false;
        }

        public bool VisitConditionalExpression(ConditionalExpression c, TypeVariable tv)
        {
            MeetDataType(c.Condition, PrimitiveType.Bool);
            c.Condition.Accept(this, c.Condition.TypeVariable);
            c.ThenExp.Accept(this, c.TypeVariable);
            c.FalseExp.Accept(this, c.TypeVariable);
            return false;
        }

        public bool VisitConditionOf(ConditionOf cof, TypeVariable tv)
        {
            MeetDataType(cof, cof.DataType);
            cof.Expression.Accept(this, cof.Expression.TypeVariable);
            return false;
        }

        public bool VisitConstant(Constant c, TypeVariable tv)
        {
            MeetDataType(c, tv.DataType);
            if (c.DataType == PrimitiveType.SegmentSelector)
            {
                //$TODO: instead of pushing it into globals, it should 
                // allocate special types for each segment. This can be 
                // done at load time.
                MemoryAccessCommon(
                    null,
                    globals, 
                    c.ToInt32() * 0x10,   //$REVIEW Platform-dependent: only valid for x86 real mode.
                    c.TypeVariable,
                    platform.PointerType.BitSize);
            }
            return false;
        }

        public bool VisitDepositBits(DepositBits d, TypeVariable tv)
        {
            MeetDataType(d, tv.DataType);
            d.Source.Accept(this, d.Source.TypeVariable);
            d.InsertedBits.Accept(this, d.InsertedBits.TypeVariable);
            return false;
        }

        public bool VisitDereference(Dereference deref, TypeVariable tv)
        {
            //$BUG: push (ptr (typeof(deref)
            deref.Expression.Accept(this, deref.Expression.TypeVariable);
            return false;
        }

        public bool VisitFieldAccess(FieldAccess acc, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitIdentifier(Identifier id, TypeVariable tv)
        {
            return false;
        }

        public bool VisitMemberPointerSelector(MemberPointerSelector mps, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitMemoryAccess(MemoryAccess access, TypeVariable tv)
        {
            return VisitMemoryAccess(null, access.TypeVariable, access.EffectiveAddress, globals);
        }

        private bool VisitMemoryAccess(Expression basePointer, TypeVariable tvAccess, Expression effectiveAddress, Expression globals)
        {
            MeetDataType(tvAccess, tvAccess.DataType);
            int eaBitSize = effectiveAddress.TypeVariable.DataType.BitSize;
            Expression p;
            int offset;
            if (fieldAccessPattern.Match(effectiveAddress))
            {
                // Mem[p + c]
                p = fieldAccessPattern.CapturedExpression("p");
                offset = OffsetOf((Constant) fieldAccessPattern.CapturedExpression("c"));
                var iv = GetInductionVariable(p);
                if (iv != null)
                {
                    VisitInductionVariable(globals, (Identifier) p, iv, offset, tvAccess);
                }
                MemoryAccessCommon(basePointer, p, offset, tvAccess, eaBitSize);
            }
            else if (effectiveAddress is Constant)
            {
                // Mem[c]
                var c = effectiveAddress as Constant;
                p = effectiveAddress;
                offset = 0;
                //$BUG: offsets should be long for 64-bit architectures.
                MemoryAccessCommon(null, globals, OffsetOf(c), tvAccess, eaBitSize);
            }
            else if (effectiveAddress is Address addr && !addr.Selector.HasValue)
            {
                // Mem[addr]
                //$TODO: what to do about segmented addresses?
                p = effectiveAddress;
                offset = 0;
                //$BUG: offsets should be long for 64-bit architectures.
                MemoryAccessCommon(null, globals, (int) addr.ToLinear(), tvAccess, eaBitSize);
            }
            else if (IsArrayAccess(effectiveAddress))
            {
                // Mem[p + i] where i is integer type.
                var binEa = (BinaryExpression)effectiveAddress;

                // First do the array index.
                binEa.Right.Accept(this, binEa.Right.TypeVariable);

                var tvElement = ArrayField(basePointer, binEa.Left, binEa.DataType.BitSize, 0, 1, 0, tvAccess);
                var dtArray = factory.CreateArrayType(tvElement, 0);
                MemoryAccessCommon(basePointer, binEa.Left, 0, dtArray, eaBitSize);

                var tvArray = store.CreateTypeVariable(factory);
                tvArray.DataType = dtArray;
                tvArray.OriginalDataType = dtArray;
                VisitMemoryAccess(basePointer, tvArray, binEa.Left, globals);

                MemoryAccessCommon(basePointer, effectiveAddress, 0, tvAccess, eaBitSize);
                effectiveAddress.Accept(this, effectiveAddress.TypeVariable);
                return false;
            }
            else
            {
                // Mem[anything]
                p = effectiveAddress;
                offset = 0;
            }
            MemoryAccessCommon(basePointer, p, offset, tvAccess, eaBitSize);
            p.Accept(this, p.TypeVariable);
            return false;
        }

        private bool IsArrayAccess(Expression effectiveAddress)
        {
            if (!(effectiveAddress is BinaryExpression binEa) ||
                binEa.Operator != Operator.IAdd)
                return false;
            if (!(binEa.Right.TypeVariable.DataType is PrimitiveType ptRight) ||
                !ptRight.IsIntegral)
                return false;
            return true;
        }

        public LinearInductionVariable GetInductionVariable(Expression e)
		{
			if (!(e is Identifier id))
                return null;
            if (!ivs.TryGetValue(id, out var iv))
                return null;
            return iv;
		}

        /// <summary>
        /// Handle an expression of type 'id + offset', where id is a LinearInductionVariable.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="iv"></param>
        /// <param name="offset"></param>
        public void VisitInductionVariable(Expression eBase, Identifier id, LinearInductionVariable iv, int offset, TypeVariable tvField)
        {
            int delta = iv.Delta.ToInt32();
            var stride = Math.Abs(delta);
            int init;
            if (delta < 0)
            {
                // induction variable is decremented, so the actual array begins at ivFinal - delta.
                if (iv.Final != null)
                {
                    init = iv.Final.ToInt32() - delta;
                    if (iv.IsSigned)
                    {
                        ArrayField(null, eBase, id.DataType.BitSize, init + offset, stride, iv.IterationCount, tvField);
                    }
                    else
                    {
                        ArrayField(null, eBase, id.DataType.BitSize, init + offset, stride, iv.IterationCount, tvField);
                    }
                }
            }
            else
            {
                if (iv.Initial != null)
                {
                    init = iv.Initial.ToInt32();
                    if (iv.IsSigned)
                    {
                        ArrayField(null, eBase, id.DataType.BitSize, init + offset, stride, iv.IterationCount, tvField);
                    }
                    else
                    {
                        ArrayField(null, eBase, id.DataType.BitSize, init + offset, stride, iv.IterationCount, tvField);
                    }
                }
            }
            if (iv.IsSigned)
            {
                if (offset != 0)
                {
                    SetSize(eBase, id, stride);
                    MemoryAccessCommon(eBase, id, offset, tvField.DataType, platform.PointerType.BitSize);
                }
            }
            else
            {
                SetSize(eBase, id, stride);
                MemoryAccessCommon(eBase, id, offset, tvField.DataType, platform.PointerType.BitSize);
            }
        }

        public DataType SetSize(Expression eBase, Expression tStruct, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size must be positive");
            var s = factory.CreateStructureType(null, size);
            var ptr = eBase != null && eBase != globals
                ? (DataType) factory.CreateMemberPointer(eBase.TypeVariable, s, platform.FramePointerType.Size)
                : (DataType) factory.CreatePointer(s, platform.PointerType.BitSize);
            return MeetDataType(tStruct, ptr);
        }

        private int OffsetOf(Constant c)
        {
            var pt = c.DataType as PrimitiveType;
            if ((pt.Domain & Domain.Integer) == Domain.SignedInt)
                return c.ToInt32();
            else
                return (int) c.ToUInt32();
        }

        private Pointer PointerTo(DataType dt)
        {
            return new Pointer(dt, platform.PointerType.BitSize);
        }

        private MemberPointer MemberPointerTo(DataType baseType, DataType fieldType, int size)
        {
            return new MemberPointer(baseType, fieldType, size);
        }

        private DataType DataTypeOf(Expression e)
        {
             return e.TypeVariable.DataType;
        }

        private bool IsSelector(Expression e)
        {
            return
                DataTypeOf(e) is PrimitiveType pt &&
                pt.Domain == Domain.Selector;
        }


        public bool VisitMkSequence(MkSequence seq, TypeVariable tv)
        {
            if (seq.Expressions.Length == 2 && tv.DataType.IsPointer)
            {
                if (IsSelector(seq.Expressions[0]) || DataTypeOf(seq.Expressions[0]) is Pointer)
                {
                    var seg = seq.Expressions[0];
                    var off = seq.Expressions[1];
                    MeetDataType(seg, new Pointer(new StructureType { IsSegment = true }, DataTypeOf(seg).BitSize));
                    if (DataTypeOf(seq) is Pointer ptr)
                    {
                        MeetDataType(off, MemberPointerTo(seg.TypeVariable, ptr.Pointee, DataTypeOf(off).Size));
                    }
                    seg.Accept(this, seg.TypeVariable);
                    off.Accept(this, off.TypeVariable);
                    return false;
                }
            }
            if (tv.DataType is PrimitiveType pt && pt.IsIntegral)
            {
                MeetDataType(seq.Expressions[0], PrimitiveType.Create(pt.Domain, seq.Expressions[0].DataType.BitSize));
                foreach (var e in seq.Expressions.Skip(1))
                {
                    MeetDataType(e, PrimitiveType.Create(Domain.UnsignedInt, e.DataType.BitSize));
                }
            }
            foreach (var e in seq.Expressions)
            {
                e.Accept(this, e.TypeVariable);
            }
            return false;
        }

        private bool NYI(Expression e, TypeVariable tv)
        {
            throw new NotImplementedException(string.Format("Haven't implemented pushing {0} ({1}) into {2} yet.", tv, tv.DataType, e));
        }

        public bool VisitOutArgument(OutArgument outArgument, TypeVariable tv)
        {
            outArgument.Expression.Accept(this, outArgument.Expression.TypeVariable);
            return false;
        }

        public bool VisitPhiFunction(PhiFunction phi, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitPointerAddition(PointerAddition pa, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitProcedureConstant(ProcedureConstant pc, TypeVariable tv)
        {
            //throw new NotImplementedException();
            return false;
        }

        public bool VisitScopeResolution(ScopeResolution scopeResolution, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitSegmentedAccess(SegmentedAccess access, TypeVariable tv)
        {
            var seg = factory.CreateStructureType(null, 0);
            seg.IsSegment = true;
            MeetDataType(access.BasePointer, factory.CreatePointer(seg, access.BasePointer.DataType.BitSize));
            access.BasePointer.Accept(this, access.BasePointer.TypeVariable);

            return VisitMemoryAccess(access.BasePointer, access.TypeVariable, access.EffectiveAddress, access.BasePointer);
        }

        public bool VisitSlice(Slice slice, TypeVariable tv)
        {
            slice.Expression.Accept(this, slice.Expression.TypeVariable);
            return false;
        }

        public bool VisitTestCondition(TestCondition tc, TypeVariable tv)
        {
            MeetDataType(tc, tc.DataType);
            tc.Expression.Accept(this, tc.Expression.TypeVariable);
            return false;
        }

        public bool VisitUnaryExpression(UnaryExpression unary, TypeVariable tv)
        {
            unary.Expression.Accept(this, unary.Expression.TypeVariable);
            return false;
        }
    }
}

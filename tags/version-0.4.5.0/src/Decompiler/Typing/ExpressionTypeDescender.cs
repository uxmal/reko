#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Typing
{
    public class ExpressionTypeDescender : ExpressionVisitor<bool, TypeVariable>
    {
        // Matches the effective address of Mem[p + c] where c is a constant.
        private ExpressionMatcher fieldAccessPattern = new ExpressionMatcher(
            new BinaryExpression(
                Operator.IAdd,
                ExpressionMatcher.AnyDataType(null),
                ExpressionMatcher.AnyExpression("p"),
                ExpressionMatcher.AnyConstant("c")));

        private Platform platform;
        private TypeStore store;
        private TypeFactory factory;
        private Unifier unifier;
        private Identifier globals;
        private Dictionary<Identifier,LinearInductionVariable> ivs;

        public ExpressionTypeDescender(Program prog, TypeStore store, TypeFactory factory)
        {
            this.platform = prog.Platform;
            this.globals = prog.Globals;
            this.ivs = prog.InductionVariables;
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
            FunctionTrait(appl.Procedure, appl.Procedure.DataType.Size, appl.TypeVariable, paramTypes);

            BindActualTypesToFormalTypes(appl);
            return false;
        }

        private void BindActualTypesToFormalTypes(Application appl)
        {
            var pc = appl.Procedure as ProcedureConstant;
            if (pc == null)
                throw new NotImplementedException("The type inference of indirect calls are not implmented yet.");
            if (pc.Procedure.Signature == null)
                return;

            var sig = pc.Procedure.Signature;
            if (appl.Arguments.Length != sig.Parameters.Length)
                throw new InvalidOperationException(
                    string.Format("Call to {0} had {1} arguments instead of the expected {2}.",
                    pc.Procedure.Name, appl.Arguments.Length, sig.Parameters.Length));
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                MeetDataType(appl.Arguments[i], sig.Parameters[i].DataType);
                sig.Parameters[i].Accept(this, sig.Parameters[i].TypeVariable);
            }
        }

        public void FunctionTrait(Expression function, int funcPtrSize, TypeVariable ret, params TypeVariable[] actuals)
        {
            DataType[] adt = new DataType[actuals.Length];
            actuals.CopyTo(adt, 0);
            var fn = factory.CreateFunctionType(null, ret, adt, null);
            var pfn = factory.CreatePointer(fn, funcPtrSize);
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
            else
            {
                arr = acc.Array;
                offset = 0;
            }
            BinaryExpression b = acc.Index as BinaryExpression;
            int stride = 1;
            if (b != null && (b.Operator == Operator.IMul || b.Operator == Operator.SMul || b.Operator == Operator.UMul))
            {
                Constant c = b.Right as Constant;
                if (c != null)
                {
                    stride = c.ToInt32();
                }
            }
            ArrayField(null, arr, arr.DataType.Size, offset, stride, 0, acc);
            acc.Array.Accept(this, acc.Array.TypeVariable);
            acc.Index.Accept(this, acc.Index.TypeVariable);
            return false;
        }

        void ArrayField(Expression expBase, Expression expStruct, int structPtrSize, int offset, int elementSize, int length, Expression expField)
        {
            var element = factory.CreateStructureType(null, elementSize);
            element.Fields.Add(0, expField.TypeVariable);
            var tvElement = store.CreateTypeVariable(factory);
            tvElement.OriginalDataType = element;

            DataType dtArray = factory.CreateArrayType(tvElement, length);
            MemoryAccessCommon(expBase, expStruct, offset, dtArray, structPtrSize);
        }

        public DataType MemoryAccessCommon(Expression tBase, Expression tStruct, int offset, DataType tField, int structPtrSize)
        {
            var s = factory.CreateStructureType(null, 0);
            var field = new StructureField(offset, tField);
            s.Fields.Add(field);

            var pointer = tBase != null && tBase != globals
                ? (DataType) factory.CreateMemberPointer(tBase.TypeVariable, s, structPtrSize)
                : (DataType) factory.CreatePointer(s, structPtrSize);
            return MeetDataType(tStruct, pointer);
        }

        public bool VisitBinaryExpression(BinaryExpression binExp, TypeVariable tv)
        {
            Debug.Print("Pushing {0} ({1}) into {2}", tv, tv.DataType, binExp);
            var eLeft = binExp.Left;
            var eRight= binExp.Right;
            if (binExp.Operator == Operator.IAdd)
            {
                var dt = PushAddendDataType(binExp.TypeVariable.DataType, eRight.TypeVariable.DataType);
                MeetDataType(eLeft, dt);
                dt = PushAddendDataType(binExp.TypeVariable.DataType, eLeft.TypeVariable.DataType);
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
                var dt = PrimitiveType.CreateWord(tv.DataType.Size).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
                MeetDataType(eLeft, dt);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.IMul)
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).Size).MaskDomain(Domain.Boolean | Domain.Integer );
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).Size).MaskDomain(Domain.Boolean | Domain.Integer);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.SMul)
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).Size).MaskDomain(Domain.Boolean | Domain.SignedInt);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).Size).MaskDomain(Domain.Boolean | Domain.SignedInt);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.UMul)
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).Size).MaskDomain(Domain.Boolean | Domain.UnsignedInt);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).Size).MaskDomain(Domain.Boolean | Domain.UnsignedInt);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.FAdd ||
                binExp.Operator == Operator.FMul)
            {
                var dt = PrimitiveType.Create(Domain.Real, tv.DataType.Size);
                MeetDataType(eLeft, dt);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator is SignedIntOperator)
            {
                var dt = PrimitiveType.CreateWord(eRight.TypeVariable.DataType.Size).MaskDomain(Domain.SignedInt | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(eRight.TypeVariable.DataType.Size).MaskDomain(Domain.SignedInt | Domain.Character);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator is UnsignedIntOperator)
            {
                var dt = PrimitiveType.CreateWord(eRight.TypeVariable.DataType.Size).MaskDomain(Domain.UnsignedInt | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(eRight.TypeVariable.DataType.Size).MaskDomain(Domain.UnsignedInt|Domain.Character);
                MeetDataType(eRight, dt);
            }
            else if (binExp.Operator == Operator.Eq || binExp.Operator == Operator.Ne)
            {
                // Not much can be deduced here, except that the operands should have the same size. Earlier passes
                // already did that work, so just continue with the operands.
            }
            else if (binExp.Operator == Operator.Shl)
            {
                var dt = PrimitiveType.CreateWord(tv.DataType.Size).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Integer, DataTypeOf(eRight).Size);
            }
            else if (binExp.Operator == Operator.Shr)
            {
                var dt = PrimitiveType.CreateWord(tv.DataType.Size).MaskDomain(Domain.Boolean | Domain.UnsignedInt| Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Integer, DataTypeOf(eRight).Size);
            }
            else
                throw new NotImplementedException(string.Format("Unhandled binary operator {0} in expression {1}.", binExp.Operator, binExp));
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
                    return PrimitiveType.Create(Domain.SignedInt, dtSum.Size);
                }
                if (ptOther != null && (ptOther.Domain & Domain.Integer) != 0)
                    return PrimitiveType.Create(Domain.Pointer, dtSum.Size);
            }
            if (dtSum is MemberPointer)
            {
                var mpSum  = dtSum as MemberPointer;
                if (dtOther is MemberPointer)
                    return PrimitiveType.Create(Domain.SignedInt, dtOther.Size);
                if (ptOther != null && (ptOther.Domain & Domain.Integer) != 0)
                    return factory.CreateMemberPointer(mpSum.BasePointer, factory.CreateUnknown(), mpSum.Size);
            }
            if (ptSum != null && ptSum.IsIntegral)
            {
                if (ptOther != null && ptOther.Domain == Domain.Pointer || dtOther is Pointer)
                    return factory.CreateUnionType(null, null, new List<DataType> {dtSum, dtOther});
            }
            if (ptSum != null && ptSum.Domain == Domain.Pointer || dtSum is Pointer)
            {
                return PrimitiveType.Create(Domain.SignedInt, dtSum.Size);
            }
            return dtSum;
        }

        private DataType PushMinuendDataType(DataType dtDiff, DataType dtSub)
        {
            var ptDiff = dtDiff as PrimitiveType;
            var ptSub = dtSub as PrimitiveType;
            if (dtDiff is Pointer || ptDiff != null && ptDiff.Domain == Domain.Pointer)
            {
                throw new NotImplementedException(string.Format("Not handling {0} and {1} yet", dtDiff, dtSub));
            }
            return dtDiff;
        }

        private DataType PushSubtrahendDataType(DataType dtDiff, DataType dtMin)
        {
            var ptDiff = dtDiff as PrimitiveType;
            var ptSub = dtMin as PrimitiveType;
            if (dtDiff is Pointer || ptDiff != null && ptDiff.Domain == Domain.Pointer)
            {
                throw new NotImplementedException(string.Format("Not handling {0} and {1} yet", dtDiff, dtMin));
            }
            return dtMin;
        }

        public DataType MeetDataType(TypeVariable tv, DataType dt)
        {
            if (dt == PrimitiveType.SegmentSelector)
            {
                var seg = factory.CreateStructureType(null, 0);
                seg.IsSegment = true;
                var ptr = factory.CreatePointer(seg, dt.Size);
                dt = ptr;
            }
            tv.DataType = unifier.Unify(tv.DataType, dt);
            tv.OriginalDataType = unifier.Unify(tv.OriginalDataType, dt);
            return tv.DataType;
        }

        public DataType MeetDataType(Expression e, DataType dt)
        {
            return MeetDataType(e.TypeVariable, dt);
        }

        public bool VisitCast(Cast cast, TypeVariable tv)
        {
            MeetDataType(cast, cast.DataType);
            cast.Expression.Accept(this, cast.Expression.TypeVariable);
            return false;
        }

        public bool VisitConditionOf(ConditionOf cof, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitConstant(Constant c, TypeVariable tv)
        {
            MeetDataType(c, tv.DataType);
            if (c.DataType == PrimitiveType.SegmentSelector)
            {
                //$REVIEW: instead of pushing it into globals, it should allocate special types for
                // each segment. This can be done at start time 
                MemoryAccessCommon(
                    null,
                    globals, 
                    c.ToInt32() * 0x10,   //$REVIEW Platform-dependent: only valid for x86 real mode.
                    c.TypeVariable,
                    platform.PointerType.Size);
            }
            return false;
        }

        public bool VisitDepositBits(DepositBits d, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitDereference(Dereference deref, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitFieldAccess(FieldAccess acc, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitIdentifier(Identifier id, TypeVariable tv)
        {
            MeetDataType(id, tv.DataType);
            return false;
        }

        public bool VisitMemberPointerSelector(MemberPointerSelector mps, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitMemoryAccess(MemoryAccess access, TypeVariable tv)
        {
            return VisitMemoryAccess(null, access, access.EffectiveAddress, globals);
        }

        private bool VisitMemoryAccess(Expression basePointer, Expression access, Expression effectiveAddress, Expression globals)
        {
            var tv = access.TypeVariable;
            MeetDataType(access, tv.DataType);
            int eaSize = effectiveAddress.TypeVariable.DataType.Size;
            Debug.Print("Pushing {0} into {1}", tv, access);
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
                    VisitInductionVariable(globals, (Identifier) p, iv, offset, access);
                }
            }
            else if (effectiveAddress is Constant)
            {
                // Mem[c]
                var c = effectiveAddress as Constant;
                p = effectiveAddress;
                offset = 0;
                MemoryAccessCommon(null, globals, OffsetOf(c), access.TypeVariable, eaSize);
            }
            else if (IsArrayAccess(effectiveAddress))
            {
                // Mem[p + i] where i is integer type.
                var binEa = (BinaryExpression)effectiveAddress;
                ArrayField(null, binEa.Left, binEa.DataType.Size, 0, 1, 0, access);
                p = effectiveAddress;
                offset = 0;

            }
            else
            {
                // Mem[anything]
                p = effectiveAddress;
                offset = 0;
            }
            MemoryAccessCommon(basePointer, p, offset, access.TypeVariable, eaSize);
            p.Accept(this, p.TypeVariable);
            return false;
        }

        private bool IsArrayAccess(Expression effectiveAddress)
        {
            var binEa = effectiveAddress as BinaryExpression;
            if (binEa == null || binEa.Operator != Operator.IAdd)
                return false;
            var ptRight = binEa.Right.DataType as PrimitiveType;
            if (ptRight == null || !ptRight.IsIntegral)
                return false;
            return true;
        }

        private bool IsByteArrayAccess(Expression effectiveAddress)
        {
            throw new NotImplementedException();
        }

        public LinearInductionVariable GetInductionVariable(Expression e)
		{
			var id = e as Identifier;
			if (id == null) return null;
            LinearInductionVariable iv;
            if (!ivs.TryGetValue(id, out iv)) return null;
            return iv;
		}

        /// <summary>
        /// Handle an expression of type 'id + offset', where id is a LinearInductionVariable.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="iv"></param>
        /// <param name="offset"></param>
        public void VisitInductionVariable(Expression eBase, Identifier id, LinearInductionVariable iv, int offset, Expression eField)
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
                        ArrayField(null, eBase, id.DataType.Size, init + offset, stride, iv.IterationCount, eField);
                    }
                    else
                    {
                        ArrayField(null, eBase, id.DataType.Size, init + offset, stride, iv.IterationCount, eField);
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
                        ArrayField(null, eBase, id.DataType.Size, init + offset, stride, iv.IterationCount, eField);
                    }
                    else
                    {
                        ArrayField(null, eBase, id.DataType.Size, init + offset, stride, iv.IterationCount, eField);
                    }
                }
            }
            if (iv.IsSigned)
            {
                if (offset != 0)
                {
                    SetSize(eBase, id, stride);
                    MemoryAccessCommon(eBase, id, offset, DataTypeOf(eField), platform.PointerType.Size);
                }
            }
            else
            {
                SetSize(eBase, id, stride);
                MemoryAccessCommon(eBase, id, offset, DataTypeOf(eField), platform.PointerType.Size);
            }
        }

        public DataType SetSize(Expression eBase, Expression tStruct, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size must be positive");
            var s = factory.CreateStructureType(null, size);
            var ptr = eBase != null && eBase != globals
                ? (DataType) factory.CreateMemberPointer(eBase.TypeVariable, s, platform.FramePointerType.Size)
                : (DataType) factory.CreatePointer(s, platform.PointerType.Size);
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
            return new Pointer(dt, platform.PointerType.Size);
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
            var pt = DataTypeOf(e) as PrimitiveType;
            return pt != null && pt.Domain == Domain.Selector;
        }

        private bool IsPointer(DataType dt)
        {
            if (dt is Pointer)
                return true;
            var pt = dt as PrimitiveType;
            return pt != null && pt.Domain == Domain.Pointer;
        }

        public bool VisitMkSequence(MkSequence seq, TypeVariable tv)
        {
            if (IsPointer(tv.DataType))
            {
                if (IsSelector(seq.Head) || DataTypeOf(seq.Head) is Pointer)
                {
                    MeetDataType(seq.Head, new Pointer(new StructureType { IsSegment = true }, DataTypeOf(seq.Head).Size));
                    var ptr = DataTypeOf(seq) as Pointer;
                    if (ptr != null)
                    {
                        MeetDataType(seq.Tail, MemberPointerTo(seq.Head.TypeVariable, ptr.Pointee, DataTypeOf(seq.Tail).Size));
                    }
                    seq.Head.Accept(this, seq.Head.TypeVariable);
                    seq.Tail.Accept(this, seq.Tail.TypeVariable);
                    return false;
                }
            }
            return NYI(seq, tv);
        }

        private bool NYI(Expression e, TypeVariable tv)
        {
            throw new NotImplementedException(string.Format("Haven't implemented pushing {0} ({1}) into {2} yet.", tv, tv.DataType, e));
        }

        public bool VisitOutArgument(OutArgument outArgument, TypeVariable tv)
        {
            throw new NotImplementedException();
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
            MeetDataType(access.BasePointer, factory.CreatePointer(seg, access.BasePointer.DataType.Size));
            access.BasePointer.Accept(this, access.BasePointer.TypeVariable);

            return VisitMemoryAccess(access.BasePointer, access, access.EffectiveAddress, access.BasePointer);
            //MeetDataType(access, tv.DataType);
            //int eaSize = access.TypeVariable.DataType.Size;
            //if (fieldAccessPattern.Match(access.EffectiveAddress))
            //{
            //    // Mem[seg:p + c]
            //    var p = fieldAccessPattern.CapturedExpression("p");
            //    var c = ((Constant) fieldAccessPattern.CapturedExpression("c")).ToInt32();
            //    MemoryAccessCommon(access.BasePointer, p, c, access.TypeVariable, eaSize);
            //    p.Accept(this, p.TypeVariable);
            //}
            //else if (access.EffectiveAddress is Constant)
            //{
            //    // Mem[seg:c]
            //    var c = access.EffectiveAddress as Constant;
            //    MemoryAccessCommon(access.BasePointer, c, 0, access.TypeVariable, eaSize);
            //    MemoryAccessCommon(null, access.BasePointer, c.ToInt32(), access.TypeVariable, eaSize);
            //    c.Accept(this, c.TypeVariable);
            //}
            //else 
            //{
            //    // Mem[seg:anything]
            //    MemoryAccessCommon(access.BasePointer, access.EffectiveAddress, 0, access.TypeVariable, eaSize);
            //}
            //return false;
        }

        public bool VisitSlice(Slice slice, TypeVariable tv)
        {
            slice.Expression.Accept(this, slice.Expression.TypeVariable);
            return false;
        }

        public bool VisitTestCondition(TestCondition tc, TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public bool VisitUnaryExpression(UnaryExpression unary, TypeVariable tv)
        {
            throw new NotImplementedException();
        }
    }
}

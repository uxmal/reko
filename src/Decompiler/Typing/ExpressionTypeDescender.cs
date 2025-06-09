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
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
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
        private static readonly ExpressionMatcher fieldAccessPattern = new(
            new BinaryExpression(
                Operator.IAdd,
                ExpressionMatcher.AnyDataType(null),
                ExpressionMatcher.AnyExpression("p"),
                ExpressionMatcher.AnyConstant("c")));
        private static readonly ExpressionMatcher segFieldAccessPattern = new(
            new MkSequence(
                ExpressionMatcher.AnyDataType(null),
                ExpressionMatcher.AnyExpression("p"),
                ExpressionMatcher.AnyConstant("c")));

        protected readonly IPlatform platform;
        protected readonly TypeStore store;
        protected readonly TypeFactory factory;
        protected readonly Unifier unifier;
        protected readonly ArrayExpressionMatcher aem;
        protected readonly Identifier globals;
        protected readonly IReadOnlyDictionary<Identifier,LinearInductionVariable> ivs;

        public ExpressionTypeDescender(IReadOnlyProgram program, TypeStore store, TypeFactory factory)
        {
            this.platform = program.Platform;
            this.globals = program.Globals;
            this.ivs = program.InductionVariables;
            this.store = store;
            this.factory = factory;
            this.unifier = new DataTypeBuilderUnifier(factory, store);
            this.aem = new ArrayExpressionMatcher(program.Platform.PointerType);
        }

        protected virtual TypeVariable TypeVar(Expression exp)
            => store.GetTypeVariable(exp);
        
        public bool VisitAddress(Address addr, TypeVariable tv)
        {
            MeetDataType(addr, tv.DataType);
            return false;
        }

        public bool VisitApplication(Application appl, TypeVariable tv)
        {
            MeetDataType(appl, TypeVar(appl).DataType);

            appl.Procedure.Accept(this, TypeVar(appl.Procedure));
            TypeVariable[] paramTypes = new TypeVariable[appl.Arguments.Length];
            int i;
            for (i = 0; i < appl.Arguments.Length; ++i)
            {
                appl.Arguments[i].Accept(this, TypeVar(appl.Arguments[i]));
                paramTypes[i] = TypeVar(appl.Arguments[i]);
            }
            FunctionTrait(appl.Procedure, appl.Procedure.DataType.BitSize, TypeVar(appl), paramTypes);
            BindActualTypesToFormalTypes(appl);
            return false;
        }

        private static FunctionType? MatchFunctionPointer(DataType dt)
        {
            if (dt is Pointer ptr)
                return ptr.Pointee as FunctionType;
            else
                return null;
        }

        private FunctionType? ExtractSignature(Expression proc)
        {
            if (proc is ProcedureConstant pc)
                return pc.Signature;
            return MatchFunctionPointer(TypeVar(proc).DataType);
        }

        private void BindActualTypesToFormalTypes(Application appl)
        {
            var sig = ExtractSignature(appl.Procedure);
            if (sig is null || !sig.ParametersValid)
                return;
            var inputs = sig.Parameters!;
            var outputs = sig.Outputs;
            ValidateArgumentCounts(appl, sig);
            int i;
            for (i = 0; i < appl.Arguments.Length; ++i)
            {
                if (i < inputs.Length)
                {
                    MeetDataType(appl.Arguments[i], inputs[i].DataType);
                }
                else
                {
                    var iOut = i - inputs.Length + 1;
                    if (iOut < outputs.Length)
                    {
                        MeetDataType(appl.Arguments[i], outputs[iOut].DataType);
                    }
                }
            }
        }

        private static void ValidateArgumentCounts(Application appl, FunctionType sig)
        {
            if (sig.IsVariadic)
                return;
            var cParameters = sig.Outputs!.Length;
            if (cParameters > 0)
            {
                // The first output parameter is the return value.
                --cParameters;
            }
            cParameters += sig.Parameters!.Length;
            if (appl.Arguments.Length != cParameters)
                throw new InvalidOperationException(
                    string.Format("Call to {0} had {1} arguments instead of the expected {2}.",
                    appl.Procedure, appl.Arguments.Length, cParameters));
        }

        public void FunctionTrait(Expression function, int funcPtrBitSize, TypeVariable ret, params TypeVariable[] actuals)
        {
            if (function is ProcedureConstant pc &&
                pc.Procedure is ExternalProcedure ep &&
                ep.Characteristics is not null &&
                ep.Characteristics.Allocator)
            {
                // Allocation sites mustn't be tied to other allocation sites. Don't mutate
                // the existing signature. 
                //$TODO: In fact, no user- or environment-provided function types should
                // ever merge new FunctionTypes into the existing signature; the existing 
                // signature should be treated as correct.
                return;
            }

            Identifier[] parameters = actuals
                .Select(a => new Identifier("", a, null!))
                .ToArray();
            var fn = factory.CreateFunctionType(
                new Identifier("", ret, null!),
                parameters);
            var pfn = factory.CreatePointer(fn, funcPtrBitSize);
            MeetDataType(function, pfn);
        }

        public bool VisitArrayAccess(ArrayAccess acc, TypeVariable tv)
        {
            MeetDataType(acc, acc.DataType);
            Expression arr;
            int offset;
            var m = fieldAccessPattern.Match(acc.Array);
            if (m.Success)
            {
                arr = m.CapturedExpression("p")!;
                offset = OffsetOf((Constant) m.CapturedExpression("c")!);
            }
            else
            {
                m = segFieldAccessPattern.Match(acc.Array);
                if (m.Success)
                {
                    arr = m.CapturedExpression("p")!;
                    offset = OffsetOf((Constant) m.CapturedExpression("c")!);
                }
                else
                {
                    arr = acc.Array;
                    offset = 0;
                }
            }
            int stride = 1;
            if (acc.Index is BinaryExpression bIndex && (bIndex.Operator.Type == OperatorType.IMul || bIndex.Operator.Type == OperatorType.SMul || bIndex.Operator.Type == OperatorType.UMul))
            {
                if (bIndex.Right is Constant c)
                {
                    stride = c.ToInt32();
                }
            }
            var tvArray = ArrayField(null, arr, arr.DataType.BitSize, offset, stride, 0, TypeVar(acc));

            MeetDataType(acc.Array, factory.CreatePointer(tvArray, acc.Array.DataType.BitSize));
            acc.Array.Accept(this, TypeVar(acc.Array));
            acc.Index.Accept(this, TypeVar(acc.Index));
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
        private TypeVariable ArrayField(
            Expression? expBase, 
            Expression expStruct,
            int structPtrBitSize, 
            int offset, 
            int elementSize, 
            int length, 
            TypeVariable tvField)
        {
            var dtElement = factory.CreateStructureType(null, elementSize);
            if (
                tvField.DataType is StructureType strField &&
                // Structures with different sizes are not compatible
                (strField.Size == 0 || strField.Size == elementSize))
            {
                foreach (var f in strField.Fields)
                {
                    dtElement.Fields.Add(f);
                }
            }
            else
            {
                dtElement.Fields.Add(0, tvField);
            }
            var tvElement = store.CreateTypeVariable(factory);
            tvElement.DataType = dtElement;
            tvElement.OriginalDataType = dtElement;

            var tvArray = store.CreateTypeVariable(factory);
            tvArray.DataType = tvArray.OriginalDataType =
                factory.CreateArrayType(tvElement, length);
            StructField(expBase, expStruct, offset, tvArray, structPtrBitSize);
            return tvArray;
        }

        /// <summary>
        /// Assert that there is a structure field at offset <paramref name="offset" />
        /// of <paramref name="eStructPtr"/>, which is treated as a pointer to the struct.
        /// </summary>
        /// <param name="eBase">Optional base pointer used in segmented addressing.</param>
        /// <param name="eStructPtr">Expression that is a pointer to a structure.</param>
        /// <param name="offset">An offset within that structure.</param>
        /// <param name="dtField">The data type of the field being accessed.</param>
        /// <param name="structPtrBitSize">Side of eStructPtr in bits.</param>
        /// <returns>The union of <paramref name="eStructPtr" /> with a pointer to a structure
        /// containing a field at offset <paramref name="offset" />.
        /// </returns>
        public DataType StructField(Expression? eBase, Expression eStructPtr, int offset, DataType dtField, int structPtrBitSize)
        {
            // Do not unify with user-defined fields
            if (IsUserDefinedField(eStructPtr, offset))
                return TypeVar(eStructPtr).DataType;
            var s = factory.CreateStructureType(null, 0);
            var field = new StructureField(offset, dtField);
            s.Fields.Add(field);

            var pointer = eBase is not null && eBase != globals
                ? (DataType)factory.CreateMemberPointer(TypeVar(eBase), s, structPtrBitSize)
                : (DataType)factory.CreatePointer(s, structPtrBitSize);
            return MeetDataType(eStructPtr, pointer);
        }

        private bool IsUserDefinedField(Expression eStructPtr, int offset)
        {
            var tv = TypeVar(eStructPtr);
            if (tv.DataType is null)
                return false;
            var ptr = tv.DataType.ResolveAs<Pointer>();
            if (ptr is null)
                return false;
            var str = ptr.Pointee.ResolveAs<StructureType>();
            if (str is null)
                return false;
            if (!str.UserDefined)
                return false;
            var field = str.Fields.AtOffset(offset);
            return field is not null;
        }

        public bool VisitBinaryExpression(BinaryExpression binExp, TypeVariable tv)
        {
            var eLeft = binExp.Left;
            var eRight = binExp.Right;
            var tvBin = TypeVar(binExp);
            var tvLeft = TypeVar(eLeft);
            var tvRight = TypeVar(eRight);
            switch (binExp.Operator.Type)
            {
            case OperatorType.IAdd:
            {
                PushArrayElement(binExp);
                var dt = PushAddendDataType(tvBin.DataType, tvRight.DataType);
                if (dt is not null)
                    MeetDataType(eLeft, dt);
                dt = PushAddendDataType(tvBin.DataType, tvLeft.DataType);
                if (dt is not null)
                    MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.ISub:
            case OperatorType.USub:
            {
                var dt = PushMinuendDataType(tvBin.DataType, tvRight.DataType);
                MeetDataType(eLeft, dt);
                dt = PushSubtrahendDataType(tvBin.DataType, tvLeft.DataType);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.And:
            case OperatorType.Or:
            {
                //$REVIEW: need a push-logical-Data type to push [[a & 3]] = char into its left and right halves.
                var dt = PrimitiveType.CreateWord(tv.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
                MeetDataType(eLeft, dt);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.IMul:
            case OperatorType.IMod:
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).BitSize).MaskDomain(Domain.Boolean | Domain.Integer );
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).BitSize).MaskDomain(Domain.Boolean | Domain.Integer);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.SMul:
            case OperatorType.SDiv:
            case OperatorType.SMod:
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).BitSize).MaskDomain(Domain.Boolean | Domain.SignedInt);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).BitSize).MaskDomain(Domain.Boolean | Domain.SignedInt);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.UMul:
            case OperatorType.UDiv:
            case OperatorType.UMod:
            {
                var dt = PrimitiveType.CreateWord(DataTypeOf(eLeft).BitSize).MaskDomain(Domain.Boolean | Domain.UnsignedInt);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(DataTypeOf(eRight).BitSize).MaskDomain(Domain.Boolean | Domain.UnsignedInt);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.FAdd:
            case OperatorType.FSub:
            case OperatorType.FMul:
            case OperatorType.FDiv:
            case OperatorType.FMod:
                {
                var dt = PrimitiveType.Create(Domain.Real, eLeft.DataType.BitSize);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Real, eRight.DataType.BitSize);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.Lt:
            case OperatorType.Le:
            case OperatorType.Ge:
            case OperatorType.Gt:
            {
                var dt = PrimitiveType.CreateWord(tvRight.DataType.BitSize).MaskDomain(Domain.SignedInt | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(tvRight.DataType.BitSize).MaskDomain(Domain.SignedInt | Domain.Character);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.Ult:
            case OperatorType.Ule:
            case OperatorType.Uge:
            case OperatorType.Ugt:
            {
                var dt = PrimitiveType.CreateWord(tvRight.DataType.BitSize).MaskDomain(Domain.Pointer| Domain.UnsignedInt | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.CreateWord(tvRight.DataType.BitSize).MaskDomain(Domain.Pointer | Domain.UnsignedInt|Domain.Character);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.Eq:
            case OperatorType.Ne:
            case OperatorType.Xor:
            case OperatorType.Cand:
            case OperatorType.Cor:
            {
                // Not much can be deduced here, except that the operands should have the same size. Earlier passes
                // already did that work, so just continue with the operands.
                break;
            }
            case OperatorType.Feq:
            case OperatorType.Fne:
            case OperatorType.Flt:
            case OperatorType.Fle:
            case OperatorType.Fge:
            case OperatorType.Fgt:
            {
                // We know leaves must be floats
                var dt = PrimitiveType.Create(Domain.Real, eLeft.DataType.BitSize);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Real, eLeft.DataType.BitSize);
                MeetDataType(eRight, dt);
                break;
            }
            case OperatorType.Shl:
            {
                var dt = PrimitiveType.CreateWord(tv.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.Integer | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Integer, DataTypeOf(eRight).BitSize);
                break;
            }
            case OperatorType.Shr:
            {
                var dt = PrimitiveType.CreateWord(tv.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.UnsignedInt| Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Integer, DataTypeOf(eRight).BitSize);
                break;
            }
            case OperatorType.Sar:
            {
                var dt = PrimitiveType.CreateWord(tv.DataType.BitSize).MaskDomain(Domain.Boolean | Domain.SignedInt | Domain.Character);
                MeetDataType(eLeft, dt);
                dt = PrimitiveType.Create(Domain.Integer, DataTypeOf(eRight).BitSize);
                break;
            }
            default:
                throw new TypeInferenceException($"Unhandled binary operator {binExp.Operator} in expression {binExp}.");
            }
            eLeft.Accept(this, tvLeft);
            eRight.Accept(this, tvRight);
            return false;
        }

        private DataType? PushAddendDataType(DataType dtSum, DataType dtOther)
        {
            if (dtSum.Domain == Domain.Pointer)
            {
                if (dtOther.Domain == Domain.Pointer)
                {
                    return PrimitiveType.Create(Domain.SignedInt, dtSum.BitSize);
                }
                if ((dtOther.Domain & Domain.Integer) != 0)
                {
                    return PrimitiveType.Create(Domain.Pointer, dtSum.BitSize);
                }
            }
            if (dtSum is MemberPointer mpSum)
            {
                if (dtOther.Domain is Domain.Offset)
                    return PrimitiveType.Create(Domain.SignedInt, dtOther.BitSize);
                if ((dtOther.Domain & Domain.Integer) != 0)
                {
                    return factory.CreateMemberPointer(mpSum.BasePointer, factory.CreateUnknown(), mpSum.BitSize);
                }
            }
            if (dtSum.IsIntegral)
            {
                // With integral types, type information flows only from leaves
                // to root.
                return null;
            }
            if (dtSum.Domain == Domain.Pointer)
            {
                return PrimitiveType.Create(Domain.SignedInt, dtSum.BitSize);
            }
            return dtSum;
        }

        private void PushArrayElement(BinaryExpression binExp)
        {
            var eLeft = binExp.Left;
            var tvBin = TypeVar(binExp);
            if (tvBin.DataType is not Pointer binPtr)
                return;
            if (!aem.Match(binExp))
                return;
            if (aem.ArrayPointer == eLeft)
            {
                if (binPtr.Pointee is not TypeVariable tvField)
                {
                    tvField = store.CreateTypeVariable(factory);
                    tvField.DataType = binPtr.Pointee;
                    tvField.OriginalDataType = binPtr.Pointee;
                }
                ArrayField(
                    null, eLeft, eLeft.DataType.BitSize, 0,
                    OffsetOf(aem.ElementSize!), 0, tvField);
            }
        }

        private static DataType PushMinuendDataType(DataType dtDiff, DataType dtSub)
        {
            if (dtDiff.Domain == Domain.Pointer)
            {
                if ((dtSub.Domain & Domain.Integer) != 0)
                    return PrimitiveType.Create(Domain.Pointer, dtDiff.BitSize);
            }
            if (dtDiff.Domain == Domain.Offset)
            {
                if ((dtSub.Domain & Domain.Integer) != 0)
                    return dtDiff;
            }
            return dtDiff;
        }

        private static DataType PushSubtrahendDataType(DataType dtDiff, DataType dtMin)
        {
            if (dtDiff.Domain == Domain.Pointer)
            {
                if (dtMin.Domain == Domain.Pointer)
                    return PrimitiveType.Create(Domain.Integer, dtDiff.BitSize);
            }
            if (dtDiff.Domain == Domain.Offset)
            {
                if (dtMin.Domain == Domain.Offset)
                    return PrimitiveType.Create(Domain.Integer, dtDiff.BitSize);
            }
            return dtMin;
        }

        public DataType MeetDataType(Expression exp, DataType dt)
        {
            var tv = TypeVar(exp);
            if (exp is Conversion || exp is Cast)
                return tv.DataType;
            return MeetDataType(tv, dt);
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
            tvExp.DataType = unifier.Unify(tvExp.DataType, dt)!;
            var u = unifier.Unify(tvExp.OriginalDataType, dt)!;
            tvExp.OriginalDataType = u;
            return tvExp.DataType;
        }

        public bool VisitCast(Cast cast, TypeVariable? tv)
        {
            MeetDataType(cast, cast.DataType);
            cast.Expression.Accept(this, TypeVar(cast.Expression));
            return false;
        }

        public bool VisitConditionalExpression(ConditionalExpression c, TypeVariable tv)
        {
            MeetDataType(c.Condition, PrimitiveType.Bool);
            c.Condition.Accept(this, TypeVar(c.Condition));
            c.ThenExp.Accept(this, TypeVar(c));
            c.FalseExp.Accept(this, TypeVar(c));
            return false;
        }

        public bool VisitConditionOf(ConditionOf cof, TypeVariable tv)
        {
            MeetDataType(cof, cof.DataType);
            cof.Expression.Accept(this, TypeVar(cof.Expression));
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
                StructField(
                    null,
                    globals, 
                    c.ToInt32() * 0x10,   //$REVIEW Platform-dependent: only valid for x86 real mode.
                    TypeVar(c),
                    platform.PointerType.BitSize);
            }
            return false;
        }

        public bool VisitConversion(Conversion conversion, TypeVariable tv)
        {
     //       MeetDataType(conversion, conversion.DataType);
            MeetDataType(conversion.Expression, conversion.SourceDataType);
            conversion.Expression.Accept(this, TypeVar(conversion.Expression));
            return false;
        }

        public bool VisitDereference(Dereference deref, TypeVariable tv)
        {
            //$BUG: push (ptr (typeof(deref)
            deref.Expression.Accept(this, TypeVar(deref.Expression));
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
            if (access.EffectiveAddress is SegmentedPointer segptr)
            {
                var seg = factory.CreateStructureType(null, 0);
                seg.IsSegment = true;
                MeetDataType(segptr.BasePointer, factory.CreatePointer(seg, segptr.BasePointer.DataType.BitSize));
                segptr.BasePointer.Accept(this, TypeVar(segptr.BasePointer));
                return VisitMemoryAccess(segptr.BasePointer, TypeVar(access), segptr.Offset, segptr.BasePointer);
            }
            else
                return VisitMemoryAccess(null, TypeVar(access), access.EffectiveAddress, globals);
        }

        private bool VisitMemoryAccess(Expression? basePointer, TypeVariable tvAccess, Expression effectiveAddress, Expression globals)
        {
            MeetDataType(tvAccess, tvAccess.DataType);
            int eaBitSize = TypeVar(effectiveAddress).DataType.BitSize;
            Expression p;
            int offset;
            var match = fieldAccessPattern.Match(effectiveAddress);
            if (match.Success)
            {
                // Mem[p + c]
                p = match.CapturedExpression("p")!;
                var c = ToConstant(match.CapturedExpression("c")!)!;
                offset = OffsetOf(c);
                if (p is Conversion cvt && cvt.SourceDataType.BitSize < cvt.DataType.BitSize)
                {
                    // p some convert-extended thing and cannot be a pointer; c therefore must be treated as a
                    // pointer and p is the index.
                    // First do the array index.
                    p.Accept(this, TypeVar(p));

                    // Now treat c as an array pointer.
                    var cbElement = tvAccess.DataType.Size;
                    var tvArray = ArrayField(basePointer, c, c.DataType.BitSize, 0, cbElement, 0, tvAccess);
                    StructField(basePointer, c, 0, tvArray, eaBitSize);
                    return false;
                }
                else if (p is Cast cast && cast.Expression.DataType.BitSize < cast.DataType.BitSize)
                {
                    p.Accept(this, TypeVar(p));

                    var cbElement = tvAccess.DataType.Size;
                    var tvArray = ArrayField(basePointer, c, c.DataType.BitSize, 0, cbElement, 0, tvAccess);
                    StructField(basePointer, c, 0, tvArray, eaBitSize);
                    return false;
                }
                else
                {
                    var iv = GetInductionVariable(p);
                    if (iv is not null)
                    {
                        VisitInductionVariable(globals, (Identifier) p, iv, offset, tvAccess);
                    }
                    StructField(basePointer, p, offset, tvAccess, eaBitSize);
                }
            }
            else if (effectiveAddress is Constant c)
            {
                // Mem[c]
                p = effectiveAddress;
                offset = 0;
                //$BUG: offsets should be long for 64-bit architectures.
                StructField(null, globals, OffsetOf(c), tvAccess, eaBitSize);
            }
            else if (effectiveAddress is Address addr && !addr.Selector.HasValue)
            {
                // Mem[addr]
                //$TODO: what to do about segmented addresses?
                p = effectiveAddress;
                offset = 0;
                //$BUG: offsets should be long for 64-bit architectures.
                StructField(null, globals, (int) addr.ToLinear(), tvAccess, eaBitSize);
            }
            else if (IsArrayAccess(effectiveAddress))
            {
                // Mem[p + i] where i is integer type.
                var binEa = (BinaryExpression) effectiveAddress;
                VisitPossibleArrayAccess(basePointer, tvAccess, binEa.Left, binEa.Right, globals, eaBitSize);
                StructField(basePointer, effectiveAddress, 0, tvAccess, eaBitSize);
                effectiveAddress.Accept(this, TypeVar(effectiveAddress));
                return false;
            }
            else
            {
                // Mem[anything]
                p = effectiveAddress;
                offset = 0;
            }
            StructField(basePointer, p, offset, tvAccess, eaBitSize);
            p.Accept(this, TypeVar(p));
            return false;
        }

        private static Constant? ToConstant(Expression expression)
        {
            if (expression is Constant c)
                return c;
            if (expression is Address addr)
                return addr.ToConstant();
            return null;
        }

        private void VisitPossibleArrayAccess(Expression? basePointer, TypeVariable tvAccess, Expression left, Expression right, Expression globals, int eaBitSize)
        {
            // First do the array index.
            right.Accept(this, TypeVar(right));

            var cbElement = tvAccess.DataType.Size;
            var tvArray = ArrayField(basePointer, left, left.DataType.BitSize, 0, cbElement, 0, tvAccess);

            StructField(basePointer, left, 0, tvArray, eaBitSize);

            if (left is not Identifier)
            {
                VisitMemoryAccess(basePointer, tvArray, left, globals);
            }
        }

        /// <summary>
        /// Returns true if the <paramref name="effectiveAddress"/> is an addition
        /// with an integral right-hand-side.
        /// </summary>
        private bool IsArrayAccess(Expression effectiveAddress)
        {
            if (effectiveAddress is not BinaryExpression binEa ||
                binEa.Operator.Type != OperatorType.IAdd)
                return false;
            return TypeVar(binEa.Right).DataType.IsIntegral;
        }

        public LinearInductionVariable? GetInductionVariable(Expression e)
		{
			if (e is Identifier id &&
                ivs.TryGetValue(id, out var iv))
                return iv;
            else
                return null;
		}

        /// <summary>
        /// Handle an expression of type 'id + offset', where id is a LinearInductionVariable.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="iv"></param>
        /// <param name="offset"></param>
        public void VisitInductionVariable(Expression eBase, Identifier id, LinearInductionVariable iv, int offset, TypeVariable tvField)
        {
            int delta = iv.Delta!.ToInt32();
            var stride = Math.Abs(delta);
            int init;
            if (delta < 0)
            {
                // induction variable is decremented, so the actual array begins at ivFinal - delta.
                if (iv.Final is not null)
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
                if (iv.Initial is not null)
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
                    StructField(eBase, id, offset, tvField.DataType, platform.PointerType.BitSize);
                }
            }
            else
            {
                SetSize(eBase, id, stride);
                StructField(eBase, id, offset, tvField.DataType, platform.PointerType.BitSize);
            }
        }

        public DataType SetSize(Expression eBase, Expression tStruct, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size must be positive");
            var s = factory.CreateStructureType(null, size);
            var ptr = eBase is not null && eBase != globals
                ? (DataType) factory.CreateMemberPointer(TypeVar(eBase), s, platform.FramePointerType.BitSize)
                : (DataType) factory.CreatePointer(s, platform.PointerType.BitSize);
            return MeetDataType(tStruct, ptr);
        }

        private static int OffsetOf(Constant c)
        {
            if ((c.DataType.Domain & Domain.Integer) == Domain.SignedInt)
                return c.ToInt32();
            else
                return (int) c.ToUInt32();
        }

        private static MemberPointer MemberPointerTo(DataType baseType, DataType fieldType, int bitSize)
        {
            return new MemberPointer(baseType, fieldType, bitSize);
        }

        private DataType DataTypeOf(Expression e)
        {
             return TypeVar(e).DataType;
        }

        private bool IsSelector(Expression e)
        {
            return DataTypeOf(e).Domain == Domain.Selector;
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
                    var dtSeq = DataTypeOf(seq);
                    if (dtSeq is Pointer ptr)
                    {
                        MeetDataType(off, MemberPointerTo(TypeVar(seg), ptr.Pointee, DataTypeOf(off).BitSize));
                    } else if (dtSeq.IsPointer)
                    {
                        MeetDataType(off, MemberPointerTo(TypeVar(seg), new UnknownType(), DataTypeOf(off).BitSize));
                    }
                    seg.Accept(this, TypeVar(seg));
                    off.Accept(this, TypeVar(off));
                    return false;
                }
            }
            if (tv.DataType.IsIntegral)
            {
                MeetDataType(seq.Expressions[0], PrimitiveType.Create(tv.DataType.Domain, seq.Expressions[0].DataType.BitSize));
                foreach (var e in seq.Expressions.Skip(1))
                {
                    MeetDataType(e, PrimitiveType.CreateWord(e.DataType.BitSize));
                }
            }
            foreach (var e in seq.Expressions)
            {
                e.Accept(this, TypeVar(e));
            }
            return false;
        }

        private bool NYI(Expression e, TypeVariable tv)
        {
            throw new NotImplementedException(string.Format("Haven't implemented pushing {0} ({1}) into {2} yet.", tv, tv.DataType, e));
        }

        public bool VisitOutArgument(OutArgument outArgument, TypeVariable tv)
        {
            outArgument.Expression.Accept(this, TypeVar(outArgument.Expression));
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

        public bool VisitSegmentedAddress(SegmentedPointer address, TypeVariable tv)
        {
            var basePtr = address.BasePointer;
            var off = address.Offset;
            var seg = factory.CreateStructureType(null, 0);
            seg.IsSegment = true;
            MeetDataType(address.BasePointer, factory.CreatePointer(seg, basePtr.DataType.BitSize));
            basePtr.Accept(this, TypeVar(address.BasePointer));
            var dtSegPtr = DataTypeOf(address);
            if (dtSegPtr is Pointer ptr)
            {
                MeetDataType(off, MemberPointerTo(TypeVar(basePtr), ptr.Pointee, DataTypeOf(off).BitSize));
            }
            else if (dtSegPtr.IsPointer)
            {
                MeetDataType(off, MemberPointerTo(TypeVar(basePtr), new UnknownType(), DataTypeOf(off).BitSize));
            }
            basePtr.Accept(this, TypeVar(basePtr));
            off.Accept(this, TypeVar(off));
            return false;
        }

        public bool VisitSlice(Slice slice, TypeVariable tv)
        {
            slice.Expression.Accept(this, TypeVar(slice.Expression));
            return false;
        }

        public bool VisitStringConstant(StringConstant str, TypeVariable tv)
        {
            MeetDataType(str, tv.DataType);
            return false;
        }

        public bool VisitTestCondition(TestCondition tc, TypeVariable tv)
        {
            MeetDataType(tc, tc.DataType);
            tc.Expression.Accept(this, TypeVar(tc.Expression));
            return false;
        }

        public bool VisitUnaryExpression(UnaryExpression unary, TypeVariable tv)
        {
            unary.Expression.Accept(this, TypeVar(unary.Expression));
            return false;
        }
    }
}

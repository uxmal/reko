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
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Typing
{
    /// <summary>
    /// Given an expression whose type is "complex" (e.g. pointer
    /// or structure types), and optional index expresssion and/or 
    /// integer offset, creates a C-like expression of the type:
    ///     - array index reference (e.g. a[i])
    ///     - structure field reference (e.g. ptr->x)
    ///     - union alternative reference (e.g. u->r)
    ///     - member pointer dereference (e.g. ptr->*foo)
    ///     - simple pointer dereference (e.g. *ptr)
    /// </summary>
    /// <remarks>
    /// It is assumed that the constituent expressions have already
    /// been converted; this class is not intended to execute
    /// recursively.
    /// </remarks>
    public class ComplexExpressionBuilder : IDataTypeVisitor<Expression>
    {
        private readonly IReadOnlyProgram program;
        private readonly ITypeStore store;
        private readonly ArrayExpressionMatcher aem;
        private Expression? expComplex;         // The expression we wish to convert to high-level code.
        private Expression? index;              // Optional index expression (like ptr + i). Should never be a constant (see "offset" member variable)
        private readonly Expression? basePtr;   // Non-null if x86-style base segment present.
        private DataType? dtComplex;            // DataType inferred by reko
        private DataType? dtComplexOrig;        // DataType of only this expression.
        private int offset;                     // constant offset from expComplex.
        private DataType? enclosingPtr;
        private DataType? dtResult;         // DataType of resulting expression. Defined if expComplex was dereferenced (Mem0[expComplex])
        private bool dereferenceGenerated;  // True if a dereferencing expression has been emitted (field access or the like.
        private int depth;

        public ComplexExpressionBuilder(
            IReadOnlyProgram program,
            ITypeStore store,
            Expression? basePtr,
            Expression complex,
            Expression? index,
            int offset)
        {
            this.program = program;
            this.store = store;
            this.basePtr = basePtr;
            this.expComplex = complex;
            this.index = index;
            this.offset = offset;
            this.aem = new(program.Platform.PointerType);
        }

        private bool Dereferenced => dtResult is not null;

        /// <summary>
        /// Build the complex expression.
        /// </summary>
        /// <param name="dtResult">DataType of resulting expression.
        /// Defined if this is being executed
        /// in the context of a MemAccess or SegmentedMemAccess.</param>
        /// <returns>The rewritten expression.</returns>
        public Expression BuildComplex(DataType? dtResult)
        {
            depth = 0; //$DEBUG;
            this.enclosingPtr = null;
            if (store.TryGetTypeVariable(expComplex!, out var tvComplex))
            {
                this.dtComplex = tvComplex.DataType;
                this.dtComplexOrig = tvComplex.OriginalDataType;
            }
            else
            {
                this.dtComplex = expComplex!.DataType;
                this.dtComplexOrig = expComplex.DataType;
            }
            var dtComplex = this.dtComplex;
            this.dtResult = dtResult;
            var exp = this.dtComplex.Accept(this);
            if (!Dereferenced && dereferenceGenerated)
            {
                var ptr = new Pointer(exp.DataType, dtComplex.BitSize);
                exp = new UnaryExpression(Operator.AddrOf, ptr, exp);
            }
            if (Dereferenced && !dereferenceGenerated)
            {
                exp = CreateDereference(dtComplex, exp);
            }
            return exp;
        }

        /// <summary>
        /// Return fallback expression. This is something like
        ///    (char *)exp + offset + index
        /// It used when we could not build appropriate C-like expression.
        /// e.g. if data type of expression is (int *) and offset is 2 we
        /// could not generate array access as size of (int *) is 4 (on x86).
        /// Returning just expression will cause to lose offset. So best way is
        /// returning '(char *)expression + offset'
        /// </summary>
        private Expression FallbackExpression()
        {
            if (offset == 0 && index is null)
                return expComplex!;
            var e = CreateAddressOf(expComplex!);
            DataType dt;
            if (enclosingPtr is not null)
            {
                dt = new Pointer(PrimitiveType.Char, enclosingPtr.BitSize);
                e = new Cast(dt, e);
            }
            else if (e.DataType.Size != 0)
            {
                dt = PrimitiveType.CreateWord(e.DataType.BitSize);
                e = new Cast(dt, e);
            }
            var eOffset = CreateOffsetExpression(offset, index);
            var op = Operator.IAdd;
            if (eOffset is Constant cOffset && cOffset.IsNegative)
            {
                op = Operator.ISub;
                eOffset = cOffset.Negate();
            }
            return new BinaryExpression(op, e.DataType, e, eOffset);
        }

        private Expression CreateAddressOf(Expression e)
        {
            if (!dereferenceGenerated)
                return e;

            dereferenceGenerated = false;
            return new UnaryExpression(Operator.AddrOf, dtComplex!, e);
        }

        public Expression VisitArray(ArrayType at)
        {
            if (offset == 0 && index is null && !Dereferenced)
                return expComplex!;
            int i = (int)(offset / at.ElementType.Size);
            int r = (int)(offset % at.ElementType.Size);
            if (TryScaleDownIndex(index, at.ElementType.Size, out var idx))
            {
                index = null;       // we've consumed the index.
            }
            dtComplex = at.ElementType;
            dtComplexOrig = at.ElementType;
            this.expComplex!.DataType = at;
            expComplex = CreateArrayAccess(at.ElementType, at, i, idx);
            offset = r;
            return dtComplex.Accept(this);
        }

        public Expression VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public Expression VisitCode(CodeType c)
        {
            return FallbackExpression();
        }

        public Expression VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public Expression VisitEquivalenceClass(EquivalenceClass eq)
        {
            this.dtComplex = eq.DataType;
            return this.dtComplex.Accept(this);
        }

        public Expression VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public Expression VisitMemberPointer(MemberPointer memptr)
        {
            if (enclosingPtr is not null)
            {
                return expComplex!;
            }
            var pointee = memptr.Pointee;
            var origMemptr = dtComplexOrig!.ResolveAs<MemberPointer>();
            if (origMemptr is not null)
            {
                pointee = origMemptr.Pointee;
            }
            return RewritePointer(memptr, memptr.Pointee, pointee);
        }

        public Expression VisitPointer(Pointer ptr)
        {
            if (enclosingPtr is not null)
            {
                return FallbackExpression();
            }
            var pointee = ptr.Pointee;
            var origPtr = dtComplexOrig!.ResolveAs<Pointer>();
            if (origPtr is not null)
            {
                pointee = origPtr.Pointee;
            }
            return RewritePointer(ptr, ptr.Pointee, pointee);
        }

        private Expression RewritePointer(DataType ptr, DataType dtPointee, DataType dtOrigPointee)
        {
            if (++depth > 20)
            {
                Debug.Print("*** Quitting; determine cause of recursion"); //$DEBUG
                return expComplex!;
            }
            enclosingPtr = ptr;
            this.dtComplex = dtPointee;
            this.dtComplexOrig = dtOrigPointee;
            return dtComplex.Accept(this);
        }

        public Expression VisitPrimitive(PrimitiveType pt)
        {
            if (enclosingPtr is null)
            {
                // We're not in a pointer context.
                expComplex!.DataType = dtComplex!;
                return FallbackExpression();
            }
            if (offset == 0 || pt.Size > 0 && offset % pt.Size == 0)
            {
                if (offset == 0 && index is null)
                {
                    if (Dereferenced)
                    {
                        if (!dereferenceGenerated)
                        {
                            dereferenceGenerated = true;
                            return CreateDereference(pt, expComplex!);
                        }
                        else
                        {
                            return expComplex!;
                        }
                    }
                    else
                    {
                        return CreateUnreferenced(pt, expComplex!);
                    }
                }
                else if (TryScaleDownIndex(index, pt.Size, out var idx))
                {
                    return CreateArrayAccess(pt, enclosingPtr, offset / pt.Size, idx);
                }
            }
            return FallbackExpression();
        }

        public Expression VisitReference(ReferenceTo refTo)
        {
            return refTo.Referent.Accept(this);
        }

        public Expression VisitString(StringType str)
        {
            return expComplex!;
        }

        public Expression VisitStructure(StructureType str)
        {
            if (++depth > 20)
            {
                Debug.Print("*** recursion too deep, quitting. Determine error then remove this"); //$DEBUG
                return expComplex!;
            }
            if (enclosingPtr is not null)
            {
                int strSize = str.GetInferredSize();
                if (str.Size > 0 // We know the size of the struct, for sure.
                    && (offset >= strSize && offset % strSize == 0 && index is null))
                {
                    var exp = CreateArrayAccess(str, enclosingPtr, offset / strSize, index);
                    index = null;
                    --depth;
                    return exp;
                }
                else if (
                    index is not null && offset == 0 &&
                    TryScaleDownIndex(index, strSize, out var idx))
                {
                    index = null;
                    var exp = CreateArrayAccess(str, enclosingPtr, 0, idx);
                    --depth;
                    return exp;
                }
            }
            StructureField? field = str.Fields.LowerBound(this.offset);
            if (field is null)
                return FallbackExpression();

            dtComplex = field.DataType;
            dtComplexOrig = field.DataType.ResolveAs<DataType>();
            this.expComplex = CreateFieldAccess(str, field.DataType, expComplex!, field);
            offset -= field.Offset;
            var e = dtComplex.Accept(this);
            --depth;
            return e;
        }

        public Expression VisitTypeReference(TypeReference typeref)
        {
            return typeref.Referent.Accept(this);
        }

        public Expression VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public Expression VisitUnion(UnionType ut)
        {
            UnionAlternative? alt = ut.FindAlternative(dtComplexOrig!);
            if (alt is null)
            {
                alt = UnionAlternativeChooser.Choose(
                    ut, dtResult, enclosingPtr is not null, offset);
            }
            if (alt is null)
            {
                Debug.Print("Unable to find {0} in {1} (Offset {2}).", dtComplexOrig, ut, offset);          //$diagnostic service
                return FallbackExpression();
            }

            dtComplex = alt.DataType;
            dtComplexOrig = alt.DataType;
            if (ut.PreferredType is not null)
            {
                expComplex = new Cast(ut.PreferredType, expComplex!);
            }
            else
            {
                expComplex = CreateFieldAccess(ut, alt.DataType, expComplex!, alt);
            }
            return dtComplex.Accept(this);
        }

        public Expression VisitUnknownType(UnknownType ut)
        {
            return FallbackExpression();
        }

        public Expression VisitVoidType(VoidType voidType)
        {
            return FallbackExpression();
        }

        private Expression CreateArrayAccess(DataType dtPointee, DataType dtPointer, int offset, Expression? arrayIndex)
        {
            if (offset == 0 && arrayIndex is null && !Dereferenced)
                return expComplex!;
            var e = CreateAddressOf(expComplex!);
            arrayIndex = CreateOffsetExpression(offset, arrayIndex);
            if (Dereferenced)
            {
                dereferenceGenerated = true;
                return new ArrayAccess(dtPointee, e, arrayIndex);
            }
            else
            {
                // Could generate &a[index] here, but 
                // a + index is more idiomatic C/C++
                dereferenceGenerated = false;
                return new BinaryExpression(Operator.IAdd, dtPointer, e, arrayIndex);
            }
        }

        private static Expression CreateOffsetExpression(int offset, Expression? index)
        {
            if (index is null)
                return Constant.Int32(offset); //$REVIEW: forcing 32-bit ints;
            if (offset == 0)
                return index;
            var op = offset < 0 ? Operator.ISub : Operator.IAdd;
            offset = Math.Abs(offset);
            var cOffset = Constant.Int32(offset); //$REVIEW: forcing 32-bit ints
            return new BinaryExpression(op, index.DataType, index, cOffset);
        }

        private Expression CreateDereference(DataType dt, Expression e)
        {
            this.dereferenceGenerated = true;
            if (basePtr is not null)
                return new MemberPointerSelector(dt, new Dereference(dt, basePtr), e);
            if (e is UnaryExpression unary && unary.Operator.Type == OperatorType.AddrOf)
                return unary.Expression;
            else if (e is not null)
                return new Dereference(dt, e);
            else
                return new ScopeResolution(dt);
        }

        private Expression CreateUnreferenced(DataType dt, Expression e)
        {
            if (basePtr is not null)
            {
                var mps = new MemberPointerSelector(dt, new Dereference(dt, basePtr), e);
                if (dt is ArrayType)
                {
                    return mps;
                }
                return new UnaryExpression(
                    Operator.AddrOf,
                    new Pointer(dt, program.Platform.PointerType.BitSize),
                    mps);
            }
            else if (e is not null)
            {
                return e;
            }
            else
                throw new NotImplementedException();
        }

        private Expression CreateFieldAccess(DataType dtStructure, DataType dtField, Expression exp, Field field)
        {
            if (exp == program.Globals)
            {
                var name = GlobalFieldName(field);
                var globalVar = Identifier.Global(name, field.DataType);
                if (dtField.ResolveAs<ArrayType>() is null)
                {
                    dereferenceGenerated = true;
                }
                return globalVar;
            }
            else
            {
                if (enclosingPtr is not null && !dereferenceGenerated)
                {
                    dereferenceGenerated = true;
                    exp = CreateDereference(dtStructure, exp);
                    if (dtField.ResolveAs<ArrayType>() is not null)
                    {
                        dereferenceGenerated = false;
                    }
                }
                var fa = new FieldAccess(dtField, exp, field);
                return fa;
            }
        }

        private string GlobalFieldName(Field field)
        {
            if (field is StructureField strField)
                return program.NamingPolicy.GlobalName(strField);
            else
                return field.Name;
        }

        private bool TryScaleDownIndex(
            Expression? exp, int elementSize, out Expression? index)
        {
            if (exp is null || elementSize <= 1)
            {
                index = exp;
                return true;
            }
            if (exp is BinaryExpression bin &&
                aem.MatchMul(bin) &&
                aem.ElementSize!.ToInt32() % elementSize == 0)
            {
                // Expression is of the form (* x c) where c is a multiple of elementSize.

                var scale = aem.ElementSize!.ToInt32() / elementSize;
                if (scale == 1)
                {
                    index = aem.Index;
                    return true;
                }
                index = new BinaryExpression(
                    Operator.IMul,
                    bin.DataType,
                    aem.Index!,
                    Constant.Int32(scale));
                return true;
            }
            else
            {
                index = null;
                return false;
            }
        }
    }
}

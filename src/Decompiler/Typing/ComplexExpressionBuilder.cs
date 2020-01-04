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

using System;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Operators;
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
        private Expression expComplex;      // The expression we wish to convert to high-level code.
        private Expression index;           // Optional index expression (like ptr + i). Should never be a constant (see "offset" member variable)
        private Expression basePtr;         // Non-null if x86-style base segment present.
        private DataType dtComplex;         // DataType inferred by reko
        private DataType dtComplexOrig;     // DataType of only this expression.
        private int offset;                 // constant offset from expComplex.
        private DataType enclosingPtr;
        private bool dereferenced;          // True if expComplex was dereferenced (Mem0[expComplex])
        private bool dereferenceGenerated;       // True if a dereferencing expression has been emitted (field access or the like.
        private int depth;

        public ComplexExpressionBuilder(
            DataType dtResult,
            Expression basePtr,
            Expression complex,
            Expression index,
            int offset)
        {
            this.basePtr = basePtr;
            this.expComplex = complex;
            this.index = index;
            this.offset = offset;
        }

        /// <summary>
        /// Build the complex expression.
        /// </summary>
        /// <param name="dereferenced">True if this is being executed
        /// in the context of a MemAccess or SegmentedMemAccess.</param>
        /// <returns>The rewritten expression.</returns>
        public Expression BuildComplex(bool dereferenced)
        {
            depth = 0; //$DEBUG;
            this.enclosingPtr = null;
            if (expComplex.TypeVariable != null)
            {
                this.dtComplex = expComplex.TypeVariable.DataType;
                this.dtComplexOrig = expComplex.TypeVariable.OriginalDataType;
            }
            else
            {
                this.dtComplex = expComplex.DataType;
                this.dtComplexOrig = expComplex.DataType;
            }
            var dtComplex = this.dtComplex;
            this.dereferenced = dereferenced;
            var exp = this.dtComplex.Accept(this);
            if (!dereferenced && dereferenceGenerated)
            {
                exp = new UnaryExpression(Operator.AddrOf, dtComplex, exp);
            }
            if (dereferenced && !dereferenceGenerated)
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
            if (offset == 0 && index == null)
                return expComplex;
            var e = CreateAddressOf(expComplex);
            DataType dt;
            if (enclosingPtr != null)
                dt = new Pointer(PrimitiveType.Char, enclosingPtr.BitSize);
            else
                dt = PrimitiveType.CreateWord(e.DataType.BitSize);
            e = new Cast(dt, e);
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
            return new UnaryExpression(Operator.AddrOf, dtComplex, e);
        }

        public Expression VisitArray(ArrayType at)
        {
            int i = (int)(offset / at.ElementType.Size);
            int r = (int)(offset % at.ElementType.Size);
            index = ScaleDownIndex(index, at.ElementType.Size);
            dtComplex = at.ElementType;
            dtComplexOrig = at.ElementType;
            this.expComplex.DataType = at;
            expComplex = CreateArrayAccess(at.ElementType, at, i, index);
            index = null;       // we've consumed the index.
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
            if (enclosingPtr != null)
            {
                return expComplex;
            }
            var pointee = memptr.Pointee;
            var origMemptr = dtComplexOrig.ResolveAs<MemberPointer>();
            if (origMemptr != null)
            {
                pointee = origMemptr.Pointee;
            }
            return RewritePointer(memptr, memptr.Pointee, pointee);
        }

        public Expression VisitPointer(Pointer ptr)
        {
            if (enclosingPtr != null)
            {
                return FallbackExpression();
            }
            var pointee = ptr.Pointee;
            var origPtr = dtComplexOrig.ResolveAs<Pointer>();
            if (origPtr != null)
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
                return expComplex;
            }
            enclosingPtr = ptr;
            this.dtComplex = dtPointee;
            this.dtComplexOrig = dtOrigPointee;
            return dtComplex.Accept(this);
        }

        public Expression VisitPrimitive(PrimitiveType pt)
        {
            if (enclosingPtr == null)
            {
                // We're not in a pointer context.
                expComplex.DataType = dtComplex;
                return FallbackExpression();
            }
            if (offset == 0 || pt.Size > 0 && offset % pt.Size == 0)
            {
                if (offset == 0 && index == null)
                {
                    if (dereferenced)
                    {
                        if (!dereferenceGenerated)
                        {
                            dereferenceGenerated = true;
                            return CreateDereference(pt, expComplex);
                        }
                        else
                        {
                            return expComplex;
                        }
                    }
                    else
                    {
                        return CreateUnreferenced(pt, expComplex);
                    }
                }
                else
                {
                    return CreateArrayAccess(pt, enclosingPtr, offset / pt.Size, index);
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
            return expComplex;
        }

        public Expression VisitStructure(StructureType str)
        {
            if (++depth > 20)
            {
                Debug.Print("*** recursion too deep, quitting. Determine error then remove this"); //$DEBUG
                return expComplex;
            }
            if (enclosingPtr != null)
            {
                int strSize = str.GetInferredSize();
                if (str.Size > 0 // We know the size of the struct, for sure.
                    && (offset >= strSize && offset % strSize == 0 && index == null))
                {
                    var exp = CreateArrayAccess(str, enclosingPtr, offset / strSize, index);
                    index = null;
                    --depth;
                    return exp;
                }
                else if (index != null && offset == 0)
                {
                    var idx = this.ScaleDownIndex(index, strSize);
                    index = null;
                    var exp = CreateArrayAccess(str, enclosingPtr, 0, idx);
                    --depth;
                    return exp;
                }
            }
            StructureField field = str.Fields.LowerBound(this.offset);
            if (field == null)
                return FallbackExpression();

            dtComplex = field.DataType;
            dtComplexOrig = field.DataType.ResolveAs<DataType>();
            this.expComplex = CreateFieldAccess(str, field.DataType, expComplex, field);
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
            UnionAlternative alt = ut.FindAlternative(dtComplexOrig);
            if (alt == null)
            {
                Debug.Print("Unable to find {0} in {1} (offset {2}).", dtComplexOrig, ut, offset);          //$diagnostic service
                return FallbackExpression();
            }

            dtComplex = alt.DataType;
            dtComplexOrig = alt.DataType;
            if (ut.PreferredType != null)
            {
                expComplex = new Cast(ut.PreferredType, expComplex);
            }
            else
            {
                expComplex = CreateFieldAccess(ut, alt.DataType, expComplex, alt);
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

        private Expression CreateArrayAccess(DataType dtPointee, DataType dtPointer, int offset, Expression arrayIndex)
        {
            if (offset == 0 && arrayIndex == null && !dereferenced)
                return expComplex;
            var e = CreateAddressOf(expComplex);
            arrayIndex = CreateOffsetExpression(offset, arrayIndex);
            if (dereferenced)
            {
                enclosingPtr = null;
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

        Expression CreateOffsetExpression(int offset, Expression index)
        {
            if (index == null)
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
            var unary = e as UnaryExpression;
            if (basePtr != null)
                return new MemberPointerSelector(dt, new Dereference(dt, basePtr), e);
            else if (unary != null && unary.Operator == Operator.AddrOf)
                return unary.Expression;
            else if (e != null)
                return new Dereference(dt, e);
            else
                return new ScopeResolution(dt);
        }

        private Expression CreateUnreferenced(DataType dt, Expression e)
        {
            if (basePtr != null)
            {
                var mps = new MemberPointerSelector(dt, new Dereference(dt, basePtr), e);
                if (dt is ArrayType)
                {
                    return mps;
                }
                return new UnaryExpression(
                    Operator.AddrOf,
                    new Pointer(dt, 32),         //$BUG: hardwired '4'.
                    mps);
            }
            else if (e != null)
            {
                return e;
            }
            else
                throw new NotImplementedException();
        }

        private Expression CreateFieldAccess(DataType dtStructure, DataType dtField, Expression exp, Field field)
        {
            if (enclosingPtr != null && !dereferenceGenerated)
            {
                dereferenceGenerated = true;
                exp = CreateDereference(dtStructure, exp);
                if (dtField.ResolveAs<ArrayType>() != null)
                {
                    dereferenceGenerated = false;
                }
            }
            var fa = new FieldAccess(dtField, exp, field);
            return fa;
        }

        private Expression ScaleDownIndex(Expression exp, int elementSize)
        {
            if (exp == null || elementSize <= 1)
                return exp;
            if (!(exp is BinaryExpression bin) ||
                (bin.Operator != Operator.IMul && bin.Operator != Operator.UMul && bin.Operator != Operator.SMul) ||
               !(bin.Right is Constant cRight) ||
                cRight.ToInt32() % elementSize != 0)
            {
                return new BinaryExpression(
                    Operator.SDiv,
                    exp.DataType,
                    exp,
                    Constant.Int32(elementSize));
            }

            // Expression is of the form (* x c) where c is a multuple of elementSize.

            var scale = cRight.ToInt32() / elementSize;
            if (scale == 1)
                return bin.Left;
            return new BinaryExpression(
                bin.Operator,
                bin.DataType,
                bin.Left,
                Constant.Int32(scale));
        }
    }
}

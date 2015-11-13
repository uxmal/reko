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

using System;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Operators;

namespace Reko.Typing
{
    public class ComplexExpressionBuilder2 : IDataTypeVisitor<Expression>
    {
        private Expression basePtr;
        private Expression complex;
        private DataType dtComplex;
        private DataType dtComplexOrig;
        private Expression index;
        private int offset;
        private bool dereferenced;
        private DataType enclosingPtr;
        private bool wasDereferenced;

        public ComplexExpressionBuilder2(
            DataType dtResult,
            Expression basePtr,
            Expression complex, 
            Expression index,
            int offset)
        {
            this.basePtr = basePtr;
            this.complex = complex;
            this.index = index;
            this.offset = offset;
        }

        public Expression BuildComplex(bool dereferenced)
        {
            this.enclosingPtr = null;
            if (complex.TypeVariable != null)
            {
                this.dtComplex = complex.TypeVariable.DataType;
                this.dtComplexOrig = complex.TypeVariable.OriginalDataType;
            }
            else
            {
                this.dtComplex = complex.DataType;
                this.dtComplexOrig = complex.DataType;
            }
            var dtComplex = this.dtComplex;
            this.dereferenced = dereferenced;
            var exp = this.dtComplex.Accept(this);
            if (!dereferenced && wasDereferenced)
            {
                exp = new UnaryExpression(Operator.AddrOf, dtComplex, exp);
            }
            if (dereferenced && !wasDereferenced)
            {
                exp = CreateDereference(dtComplex, exp);
            }
            return exp;
        }

        public Expression VisitArray(ArrayType at)
        {
            int i = (int)(offset / at.ElementType.Size);
            int r = (int)(offset % at.ElementType.Size);
            index = ScaleDownIndex(index, at.ElementType.Size);
            dtComplex = at.ElementType;
            dtComplexOrig = at.ElementType;
            this.complex.DataType = at;
            complex = CreateArrayAccess(at.ElementType, at, i, index);
            offset = r;
            return dtComplex.Accept(this);
        }

        public Expression VisitCode(CodeType c)
        {
            throw new NotImplementedException();
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
                return complex;
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
                return complex;
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
                complex.DataType = dtComplex;
                return complex;
            }
            if (offset == 0)
            {
                if (dereferenced && !wasDereferenced)
                {
                    wasDereferenced = true;
                    return CreateDereference(pt, complex);
                }
                else
                {
                    return CreateUnreferenced(pt, complex);
                }
            }
            throw new NotImplementedException();
        }

        public Expression VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

        public Expression VisitStructure(StructureType str)
        {
            StructureField field = str.Fields.LowerBound(this.offset);
            if (field == null)
                throw new TypeInferenceException("Expected structure type {0} to have a field at offset {1} ({1:X}).", str.Name, offset);

            dtComplex = field.DataType;
            dtComplexOrig = field.DataType;
            this.complex = CreateFieldAccess(str, field.DataType, complex, field.Name);
            offset -= field.Offset;
            return dtComplex.Accept(this);
        }

        public Expression VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public Expression VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public Expression VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public Expression VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public Expression VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }

        private Expression CreateArrayAccess(DataType dtPointee, DataType dtPointer, int offset, Expression arrayIndex)
        {
            if (offset == 0 && arrayIndex == null && !dereferenced)
                return complex;
            arrayIndex = CreateArrayIndexExpression(offset, arrayIndex);
            if (dereferenced)
            {
                enclosingPtr = null;
                wasDereferenced = true;
                return new ArrayAccess(dtPointee, complex, arrayIndex);
            }
            else
            {
                wasDereferenced = false;
                return new BinaryExpression(Operator.IAdd, dtPointer, complex, arrayIndex);
            }
        }

        Expression CreateArrayIndexExpression(int offset, Expression arrayIndex)
        {
            BinaryOperator op = offset < 0 ? Operator.ISub : Operator.IAdd;
            offset = Math.Abs(offset);
            Constant cOffset = Constant.Int32(offset); //$REVIEW: forcing 32-bit ints
            if (arrayIndex != null)
            {
                if (offset != 0)
                {
                    return new BinaryExpression(op, arrayIndex.DataType, arrayIndex, cOffset);
                }
            }
            else
            {
                return cOffset;
            }
            return arrayIndex;
        }

        private Expression CreateDereference(DataType dt, Expression e)
        {
            this.wasDereferenced = true;
            if (basePtr != null)
                return new MemberPointerSelector(dt, new Dereference(dt, basePtr), e);
            else if (e != null)
                return new Dereference(dt, e);
            else
                return new ScopeResolution(dt);
        }

        private Expression CreateUnreferenced(DataType dt, Expression e)
        {
            if (basePtr!= null)
            {
                var mps = new MemberPointerSelector(dt, new Dereference(dt, basePtr), e);
                if (dt is ArrayType)
                {
                    return mps;
                }
                return new UnaryExpression(
                    Operator.AddrOf,
                    new Pointer(dt, 4),         //$BUG: hardwired '4'.
                    mps);
            }
            else if (e != null)
            {
                return e;
            }
            else
                throw new NotImplementedException();
        }


        private Expression CreateFieldAccess(StructureType dtStructure, DataType dtField, Expression exp, string name)
        {
            if (enclosingPtr != null)
            {
                wasDereferenced = true;
                exp = new Dereference(dtStructure, exp);
            }
            var fa = new FieldAccess(dtField, exp, name);
            return fa;
        }

        private Expression ScaleDownIndex(Expression exp, int elementSize)
        {
            if (exp == null)
                return null;
            BinaryExpression bin = exp as BinaryExpression;
            Constant cRight;
            if (!exp.As(out bin) || 
                (bin.Operator != Operator.IMul && bin.Operator != Operator.UMul && bin.Operator != Operator.SMul) ||
                !bin.Right.As(out cRight) ||
                cRight.ToInt32() % elementSize != 0)
            {
                return new BinaryExpression(
                    Operator.SDiv,
                    exp.DataType,
                    exp,
                    Constant.Int32(elementSize));
            }
            
            // Expression is of the form (* x c) where c is a multuple of elementSize.

            var index = cRight.ToInt32() / elementSize;
            if (index == 1)
                return bin.Left;
            return new BinaryExpression(
                bin.Operator,
                bin.DataType,
                bin.Left,
                Constant.Int32(index));
        }

      
    }
}
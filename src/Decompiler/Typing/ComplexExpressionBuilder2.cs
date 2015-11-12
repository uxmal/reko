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

namespace Reko.Typing
{
    public class ComplexExpressionBuilder2 : IDataTypeVisitor<Expression>
    {
        private Expression complex;
        private DataType dtComplex;
        private DataType dtComplexOrig;
        private Expression other;
        private int offset;
        private bool dereferenced;
        private bool seenPtr;

        public ComplexExpressionBuilder2()
        {
            seenPtr = false;
        }

        public Expression Rewrite(Expression complex, Expression other, bool dereferenced)
        {
            this.complex = complex;
            this.other = other;
            Constant cOther;
            if (other.As(out cOther))
            {
                offset = cOther.ToInt32();
                this.other = null;
            }
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
            this.dereferenced = dereferenced;
            return this.dtComplex.Accept(this);
        }

        public Expression VisitArray(ArrayType at)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Expression VisitPointer(Pointer ptr)
        {
            return RewritePointer(ptr);
        }

        private Expression RewritePointer(Pointer ptr)
        {
            if (seenPtr)
            {
                if (dereferenced)
                {
                    return new Dereference(ptr.Pointee, complex);
                }
                else
                {
                    return complex;
                }
            }
            seenPtr = true;
            this.dtComplex = ptr.Pointee;
            this.dtComplexOrig = dtComplexOrig.ResolveAs<Pointer>().Pointee;

            return dtComplex.Accept(this);
        }

        public Expression VisitPrimitive(PrimitiveType pt)
        {
            if (!seenPtr)
            {
                // We're not in a pointer context.
                complex.DataType = dtComplex;
                return complex;
            }
            if (other != null)
                throw new NotImplementedException();    //$TODO arrays.
            if (offset == 0)
            {
                if (dereferenced)
                {
                    return new Dereference(pt, complex);
                }
                else
                {
                    complex.DataType = pt;
                    return complex;
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

        private Expression CreateFieldAccess(StructureType dtStructure, DataType dtField , Expression exp, string name)
        {
            if (dereferenced)
            {
                dereferenced = false;
                exp = new Dereference(dtStructure, exp);
            }
            var fa = new FieldAccess(dtField, exp, name);
            return fa;
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
    }
}
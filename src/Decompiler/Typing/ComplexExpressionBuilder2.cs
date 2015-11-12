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
            this.dtComplex = complex.TypeVariable.DataType;
            this.dtComplexOrig = complex.TypeVariable.OriginalDataType;
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
            throw new NotImplementedException();
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
                complex.DataType = dtComplex;
                return complex;
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
            Constant offset;
            if (!other.As(out offset))
                throw new NotImplementedException();    //$TODO arrays.
            var nOffset = offset.ToInt32();
            if (nOffset == 0)
            {
                if (dereferenced)
                {
                    return new Dereference(dtComplex, complex);
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
            throw new NotImplementedException();
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
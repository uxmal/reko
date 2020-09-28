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

namespace Reko.Core.Types
{
	/// <summary>
	/// Visitor methods for types.
	/// </summary>
    public interface IDataTypeVisitor<T>
    {
        T VisitArray(ArrayType at);
        T VisitClass(ClassType ct);
        T VisitCode(CodeType c);
        T VisitEnum(EnumType e);
        T VisitEquivalenceClass(EquivalenceClass eq);
        T VisitFunctionType(FunctionType ft);
        T VisitPrimitive(PrimitiveType pt);
        T VisitMemberPointer(MemberPointer memptr);
        T VisitPointer(Pointer ptr);
        T VisitReference(ReferenceTo refTo);
        T VisitString(StringType str);
        T VisitStructure(StructureType str);
        T VisitTypeReference(TypeReference typeref);
        T VisitTypeVariable(TypeVariable tv);
        T VisitUnion(UnionType ut);
        T VisitUnknownType(UnknownType ut);
        T VisitVoidType(VoidType voidType);
    }

    public interface IDataTypeVisitor
    {
        void VisitArray(ArrayType at);
        void VisitClass(ClassType ct);
        void VisitCode(CodeType c);
        void VisitEnum(EnumType e);
        void VisitEquivalenceClass(EquivalenceClass eq);
        void VisitFunctionType(FunctionType ft);
        void VisitPrimitive(PrimitiveType pt);
        void VisitMemberPointer(MemberPointer memptr);
        void VisitPointer(Pointer ptr);
        void VisitReference(ReferenceTo refTo);

        void VisitString(StringType str);
        void VisitStructure(StructureType str);
        void VisitTypeReference(TypeReference typeref);
        void VisitTypeVariable(TypeVariable tv);
        void VisitUnion(UnionType ut);
        void VisitUnknownType(UnknownType ut);
        void VisitVoidType(VoidType voidType);
    }
}

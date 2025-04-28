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

using System;

namespace Reko.Core.Types
{
	/// <summary>
	/// Visitor methods for types.
	/// </summary>
    public interface IDataTypeVisitor<T>
    {
        /// <summary>
        /// Called when visiting an array.
        /// </summary>
        /// <param name="at">Array type.</param>
        /// <returns>The result of the visit to the array.</returns>
        T VisitArray(ArrayType at);

        /// <summary>
        /// Called when visiting a class.
        /// </summary>
        /// <param name="ct">Class type.</param>
        /// <returns>The result of the visit to the class.</returns>
        T VisitClass(ClassType ct);

        /// <summary>
        /// Called when visiting an code.
        /// </summary>
        /// <param name="c">Code.</param>
        /// <returns>The result of the visit to the code.</returns>
        T VisitCode(CodeType c);

        /// <summary>
        /// Called when visiting an enumerated type.
        /// </summary>
        /// <param name="e">Enumerated type.</param>
        /// <returns>The result of the visit to the enumerated type.</returns>
        T VisitEnum(EnumType e);

        /// <summary>
        /// Called when visiting an equivalence class.
        /// </summary>
        /// <param name="eq">Equivalence class.</param>
        /// <returns>The result of the visit to the equivalence class.</returns>
        T VisitEquivalenceClass(EquivalenceClass eq);

        /// <summary>
        /// Called when visiting a function type.
        /// </summary>
        /// <param name="ft">Function type.</param>
        /// <returns>The result of the visit to the function type.</returns>
        T VisitFunctionType(FunctionType ft);

        /// <summary>
        /// Called when visiting a primitive type.
        /// </summary>
        /// <param name="pt">Primitive type.</param>
        /// <returns>The result of the visit to the primitive type.</returns>
        T VisitPrimitive(PrimitiveType pt);

        /// <summary>
        /// Called when visiting a member pointer.
        /// </summary>
        /// <param name="memptr">Member pointer.</param>
        /// <returns>The result of the visit to the member pointer.</returns>
        T VisitMemberPointer(MemberPointer memptr);

        /// <summary>
        /// Called when visiting a pointer.
        /// </summary>
        /// <param name="ptr">Pointer.</param>
        /// <returns>The result of the visit to the pointer.</returns>
        T VisitPointer(Pointer ptr);

        /// <summary>
        /// Called when visiting a reference.
        /// </summary>
        /// <param name="refTo">Reference.</param>
        /// <returns>The result of the visit to the reference.</returns>
        T VisitReference(ReferenceTo refTo);

        /// <summary>
        /// Called when visiting a string type.
        /// </summary>
        /// <param name="str">String.</param>
        /// <returns>The result of the visit to the string type.</returns>
        T VisitString(StringType str);

        /// <summary>
        /// Called when visiting a structure.
        /// </summary>
        /// <param name="str">Structure.</param>
        /// <returns>The result of the visit to the structure.</returns>
        T VisitStructure(StructureType str);

        /// <summary>
        /// Called when visiting a type reference.
        /// </summary>
        /// <param name="typeref">Type reference.</param>
        /// <returns>The result of the visit to the type reference.</returns>
        T VisitTypeReference(TypeReference typeref);

        /// <summary>
        /// Called when visiting a type variable.
        /// </summary>
        /// <param name="tv">Type variable </param>
        /// <returns>The result of the visit to the type variable.</returns>
        T VisitTypeVariable(TypeVariable tv);

        /// <summary>
        /// Called when visiting a union.
        /// </summary>
        /// <param name="ut">Union.</param>
        /// <returns>The result of the visit to the union.</returns>
        T VisitUnion(UnionType ut);

        /// <summary>
        /// Called when visiting an unknown type.
        /// </summary>
        /// <param name="ut">Unknown type.</param>
        /// <returns>The result of the visit to the unknown type.</returns>
        T VisitUnknownType(UnknownType ut);

        /// <summary>
        /// Called when visiting the void type.
        /// </summary>
        /// <param name="voidType">Unknown type.</param>
        /// <returns>The result of the visit to the void type.</returns>
        T VisitVoidType(VoidType voidType);
    }

    /// <summary>
    /// Visitor methods for types.
    /// </summary>
    public interface IDataTypeVisitor
    {
        /// <summary>
        /// Called when visiting an array.
        /// </summary>
        /// <param name="at">Array type.</param>
        /// <returns>The result of the visit to the array.</returns>
        void VisitArray(ArrayType at);

        /// <summary>
        /// Called when visiting a class.
        /// </summary>
        /// <param name="ct">Class type.</param>
        /// <returns>The result of the visit to the class.</returns>
        void VisitClass(ClassType ct);

        /// <summary>
        /// Called when visiting an code.
        /// </summary>
        /// <param name="c">Code.</param>
        /// <returns>The result of the visit to the code.</returns>
        void VisitCode(CodeType c);

        /// <summary>
        /// Called when visiting an enumerated type.
        /// </summary>
        /// <param name="e">Enumerated type.</param>
        /// <returns>The result of the visit to the enumerated type.</returns>
        void VisitEnum(EnumType e);

        /// <summary>
        /// Called when visiting an equivalence class.
        /// </summary>
        /// <param name="eq">Equivalence class.</param>
        /// <returns>The result of the visit to the equivalence class.</returns>
        void VisitEquivalenceClass(EquivalenceClass eq);

        /// <summary>
        /// Called when visiting a function type.
        /// </summary>
        /// <param name="ft">Function type.</param>
        /// <returns>The result of the visit to the function type.</returns>
        void VisitFunctionType(FunctionType ft);

        /// <summary>
        /// Called when visiting a primitive type.
        /// </summary>
        /// <param name="pt">Primitive type.</param>
        /// <returns>The result of the visit to the primitive type.</returns>
        void VisitPrimitive(PrimitiveType pt);

        /// <summary>
        /// Called when visiting a member pointer.
        /// </summary>
        /// <param name="memptr">Member pointer.</param>
        /// <returns>The result of the visit to the member pointer.</returns>
        void VisitMemberPointer(MemberPointer memptr);

        /// <summary>
        /// Called when visiting a pointer.
        /// </summary>
        /// <param name="ptr">Pointer.</param>
        /// <returns>The result of the visit to the pointer.</returns>
        void VisitPointer(Pointer ptr);

        /// <summary>
        /// Called when visiting a reference.
        /// </summary>
        /// <param name="refTo">Reference.</param>
        /// <returns>The result of the visit to the reference.</returns>
        void VisitReference(ReferenceTo refTo);

        /// <summary>
        /// Called when visiting a string type.
        /// </summary>
        /// <param name="str">String.</param>
        /// <returns>The result of the visit to the string type.</returns>
        void VisitString(StringType str);

        /// <summary>
        /// Called when visiting a structure.
        /// </summary>
        /// <param name="str">Structure.</param>
        /// <returns>The result of the visit to the structure.</returns>
        void VisitStructure(StructureType str);

        /// <summary>
        /// Called when visiting a type reference.
        /// </summary>
        /// <param name="typeref">Type reference.</param>
        /// <returns>The result of the visit to the type reference.</returns>
        void VisitTypeReference(TypeReference typeref);

        /// <summary>
        /// Called when visiting a type variable.
        /// </summary>
        /// <param name="tv">Type variable </param>
        /// <returns>The result of the visit to the type variable.</returns>
        void VisitTypeVariable(TypeVariable tv);

        /// <summary>
        /// Called when visiting a union.
        /// </summary>
        /// <param name="ut">Union.</param>
        /// <returns>The result of the visit to the union.</returns>
        void VisitUnion(UnionType ut);

        /// <summary>
        /// Called when visiting an unknown type.
        /// </summary>
        /// <param name="ut">Unknown type.</param>
        /// <returns>The result of the visit to the unknown type.</returns>
        void VisitUnknownType(UnknownType ut);

        /// <summary>
        /// Called when visiting the void type.
        /// </summary>
        /// <param name="voidType">Unknown type.</param>
        /// <returns>The result of the visit to the void type.</returns>
        void VisitVoidType(VoidType voidType);
    }
}

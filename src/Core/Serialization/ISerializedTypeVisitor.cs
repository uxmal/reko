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

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Visitor interface for visiting serialized types.
    /// </summary>
    /// <typeparam name="T">Type of values returned from visitor.</typeparam>
    public interface ISerializedTypeVisitor<T>
    {
        /// <summary>
        /// Called when visiting a <see cref="PrimitiveType_v1"/>.
        /// </summary>
        /// <param name="primitive">Visited primitive type.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitPrimitive(PrimitiveType_v1 primitive);

        /// <summary>
        /// Called when visiting a <see cref="PointerType_v1"/>.
        /// </summary>
        /// <param name="pointer">Visited pointer.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitPointer(PointerType_v1 pointer);

        /// <summary>
        /// Called when visiting a <see cref="ReferenceType_v1"/>.
        /// </summary>
        /// <param name="reference">Visited type reference.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitReference(ReferenceType_v1 reference);

        /// <summary>
        /// Called when visiting a <see cref="CodeType_v1"/>.
        /// </summary>
        /// <param name="code">Visited code type.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitCode(CodeType_v1 code);

        /// <summary>
        /// Called when visiting a <see cref="MemberPointer_v1"/>.
        /// </summary>
        /// <param name="memptr">Visited member type.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitMemberPointer(MemberPointer_v1 memptr);

        /// <summary>
        /// Called when visiting an <see cref="ArrayType_v1"/>.
        /// </summary>
        /// <param name="array">Visited array.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitArray(ArrayType_v1 array);

        /// <summary>
        /// Called when visiting a function's <see cref="SerializedSignature"/>.
        /// </summary>
        /// <param name="signature">Visited function signature.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitSignature(SerializedSignature signature);

        /// <summary>
        /// Called when visiting a <see cref="StructType_v1"/>.
        /// </summary>
        /// <param name="structure">Visited structure type.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitStructure(StructType_v1 structure);

        /// <summary>
        /// Called when visiting a <see cref="SerializedTypedef"/>.
        /// </summary>
        /// <param name="typedef">Visited type definition.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitTypedef(SerializedTypedef typedef);

        /// <summary>
        /// Called when visiting a <see cref="TypeReference_v1"/>.
        /// </summary>
        /// <param name="typeReference">Visited type reference.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitTypeReference(TypeReference_v1 typeReference);

        /// <summary>
        /// Called when visiting a <see cref="UnionType_v1"/>.
        /// </summary>
        /// <param name="union">Visited type reference.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitUnion(UnionType_v1 union);

        /// <summary>
        /// Called when visiting a <see cref="SerializedEnumType"/>.
        /// </summary>
        /// <param name="serializedEnumType">Visited enumeration type.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitEnum(SerializedEnumType serializedEnumType);

        /// <summary>
        /// Called when visiting a <see cref="SerializedTemplate"/>.
        /// </summary>
        /// <param name="serializedTemplate">Visited template.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitTemplate(SerializedTemplate serializedTemplate);

        /// <summary>
        /// Called when visiting a <see cref="VoidType_v1"/>.
        /// </summary>
        /// <param name="serializedVoidType">Visited void type.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitVoidType(VoidType_v1 serializedVoidType);

        /// <summary>
        /// Called when visiting a <see cref="StringType_v2"/>.
        /// </summary>
        /// <param name="str">Visited string type.</param>
        /// <returns>Value returned by the visitor.</returns>
        T VisitString(StringType_v2 str);
    }
}

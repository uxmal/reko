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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Serialization
{
    public interface ISerializedTypeVisitor<T>
    {
        T VisitPrimitive(PrimitiveType_v1 primitive);
        T VisitPointer(PointerType_v1 pointer);
        T VisitReference(ReferenceType_v1 referenceType_v1);
        T VisitCode(CodeType_v1 code);
        T VisitMemberPointer(MemberPointer_v1 memptr);
        T VisitArray(ArrayType_v1 array);
        T VisitSignature(SerializedSignature signature);
        T VisitStructure(StructType_v1 structure);
        T VisitTypedef(SerializedTypedef typedef);
        T VisitTypeReference(TypeReference_v1 typeReference);
        T VisitUnion(UnionType_v1 union);
        T VisitEnum(SerializedEnumType serializedEnumType);
        T VisitTemplate(SerializedTemplate serializedTemplate);
        T VisitVoidType(VoidType_v1 serializedVoidType);
        T VisitString(StringType_v2 str);
    }
}

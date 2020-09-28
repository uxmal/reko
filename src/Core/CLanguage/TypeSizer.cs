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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.CLanguage
{
    /// <summary>
    /// Returns the size of a type in bytes.
    /// </summary>
    public class TypeSizer : ISerializedTypeVisitor<int>
    {
        private readonly IPlatform platform;
        private readonly IDictionary<string, SerializedType> typedefs;
        private readonly Dictionary<SerializedTaggedType, int> tagSizes;

        public TypeSizer(IPlatform platform, IDictionary<string, SerializedType> typedefs)
        {
            this.platform = platform;
            this.typedefs = typedefs;
            this.tagSizes = new Dictionary<SerializedTaggedType, int>(new SerializedTypeComparer());
        }

        private int Align(int size)
        {
            return size;
        }

        public int VisitCode(CodeType_v1 code)
        {
            throw new NotImplementedException();
        }

        public int VisitPrimitive(PrimitiveType_v1 primitive)
        {
            return primitive.ByteSize;
        }

        public int VisitPointer(PointerType_v1 pointer)
        {
            return platform.PointerType.Size;
        }

        public int VisitReference(ReferenceType_v1 pointer)
        {
            return platform.PointerType.Size;
        }

        public int VisitMemberPointer(MemberPointer_v1 memptr)
        {
            return platform.FramePointerType.Size;
        }

        public int VisitArray(ArrayType_v1 array)
        {
            return Align(array.ElementType.Accept(this)) * array.Length;
        }

        public int VisitEnum(SerializedEnumType e)
        {
            //$BUGBUG: at most sizeof int according to the C lang def, but varies widely among compilers.
            return 4;
        }

        public int VisitSignature(SerializedSignature signature)
        {
            return 0;
        }

        public int VisitString(StringType_v2 str)
        {
            return platform.PointerType.Size;
        }

        public int VisitStructure(StructType_v1 structure)
        {
            var size = 0;
            if (structure.Fields == null)
            {
                this.tagSizes.TryGetValue(structure, out size);
                return size;
            }
            foreach (var field in structure.Fields)
            {
                field.Offset = size;
                size += field.Type.Accept(this);
            }
            structure.ByteSize = size;
            return size;
        }

        public int VisitTypedef(SerializedTypedef typedef)
        {
            //int size = typedef.DataType.Accept(this);
            //namedTypeSizes[typedef.Name] = size;
            //return size;

            int size = typedef.DataType.Accept(this);
            return size;
        }

        public int VisitTypeReference(TypeReference_v1 typeReference)
        {
            return typedefs[typeReference.TypeName].Accept(this);
        }

        public int VisitUnion(UnionType_v1 union)
        {
            if (union.Alternatives == null)
                return tagSizes[union];
            var size = 0;
            foreach (var field in union.Alternatives)
            {
                size = Math.Max(size, field.Type.Accept(this));
            }
            union.ByteSize = size;
            return size;
        }

        public int VisitVoidType(VoidType_v1 voidType)
        {
            return 0;
        }

        public int VisitTemplate(SerializedTemplate template)
        {
            throw new NotImplementedException();
        }

        public void SetSize(SerializedTaggedType str)
        {
            var size = str.Accept(this);
            tagSizes[str] = size;
        }
    }
}

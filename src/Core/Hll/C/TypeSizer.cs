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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Returns the size and alignment requirement of a type in bytes.
    /// </summary>
    public class TypeSizer : ISerializedTypeVisitor<(int, int)>
    {
        private readonly IPlatform platform;
        private readonly IDictionary<string, SerializedType> typedefs;
        private readonly Dictionary<SerializedTaggedType, (int,int)> tagSizes;
        private int structureAlignment;

        /// <summary>
        /// Constructs a <see cref="TypeSizer"/> instance.
        /// </summary>
        /// <param name="platform">Current <see cref="IPlatform"/>.</param>
        /// <param name="typedefs">Known type definitions.</param>
        public TypeSizer(IPlatform platform, IDictionary<string, SerializedType> typedefs)
        {
            this.platform = platform;
            this.typedefs = typedefs;
            this.tagSizes = new Dictionary<SerializedTaggedType, (int,int)>(new SerializedTypeComparer());
            this.structureAlignment = platform.StructureMemberAlignment;
        }

        private int Align(int size)
        {
            return size;
        }

        /// <inheritdoc/>
        public (int, int) VisitCode(CodeType_v1 code)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public (int, int) VisitPrimitive(PrimitiveType_v1 primitive)
        {
            var size = primitive.ByteSize;
            return (size, size);
        }

        /// <inheritdoc/>
        public (int, int) VisitPointer(PointerType_v1 pointer)
        {
            var ptrSize = platform.PointerType.Size;
            return (ptrSize, ptrSize);
        }

        /// <inheritdoc/>
        public (int, int) VisitReference(ReferenceType_v1 pointer)
        {
            var refSize = platform.PointerType.Size;
            return (refSize, refSize);
        }

        /// <inheritdoc/>
        public (int, int) VisitMemberPointer(MemberPointer_v1 memptr)
        {
            var mpSize = platform.FramePointerType.Size;
            return (mpSize, mpSize);
        }

        /// <inheritdoc/>
        public (int, int) VisitArray(ArrayType_v1 array)
        {
            var (elemSize, elemAlign) = array.ElementType!.Accept(this);
            return (Align(elemSize) * array.Length, elemAlign);
        }

        /// <inheritdoc/>
        public (int, int) VisitEnum(SerializedEnumType e)
        {
            //$BUGBUG: at most sizeof int according to the C lang def, but varies widely among compilers.
            return (4, 4);
        }

        /// <inheritdoc/>
        public (int, int) VisitSignature(SerializedSignature signature)
        {
            return (0, 1);
        }

        /// <inheritdoc/>
        public (int, int) VisitString(StringType_v2 str)
        {
            var pstrSize = platform.PointerType.Size;
            return (pstrSize, pstrSize);
        }

        /// <inheritdoc/>
        public (int, int) VisitStructure(StructType_v1 structure)
        {
            var size = 0;
            var alignment = 1;
            if (structure.Fields is null)
            {
                this.tagSizes.TryGetValue(structure, out var sizeAlign);
                return sizeAlign;
            }
            foreach (var field in structure.Fields)
            {
                var (fieldSize, fieldAlignment) = field.Type!.Accept(this);
                field.Offset = AlignFieldOffset(size, fieldAlignment);
                size = field.Offset + fieldSize;
                alignment = Math.Max(alignment, fieldAlignment);
            }
            structure.ByteSize = size;
            return (size, alignment);
        }

        private int AlignFieldOffset(int offset, int preferredAlignment)
        {
            int alignment;
            if (preferredAlignment < this.structureAlignment)
            {
                var floor = 1 << System.Numerics.BitOperations.Log2((uint) preferredAlignment);
                if (floor < preferredAlignment)
                    floor <<= 1;
                alignment = floor;
            }
            else
            {
                alignment = this.structureAlignment;
            }
            return alignment * ((offset + (alignment - 1)) / alignment);
        }

        /// <inheritdoc/>
        public (int, int) VisitTypedef(SerializedTypedef typedef)
        {
            //int size = typedef.DataType.Accept(this);
            //namedTypeSizes[typedef.Name] = size;
            //return size;

            var size = typedef.DataType!.Accept(this);
            return size;
        }

        /// <inheritdoc/>
        public (int, int) VisitTypeReference(TypeReference_v1 typeReference)
        {
            if (!typedefs.TryGetValue(typeReference.TypeName!, out var dataType))
            {
                Debug.WriteLine("Unable to determine size of {0}", typeReference.TypeName!);
                return (4, 4);
            }
            return dataType.Accept(this);
        }

        /// <inheritdoc/>
        public (int, int) VisitUnion(UnionType_v1 union)
        {
            if (union.Alternatives is null)
                return tagSizes[union];
            var size = 0;
            var alignment = 1;
            foreach (var field in union.Alternatives)
            {
                var (fieldSize, fieldAlignment) = field.Type!.Accept(this);
                size = Math.Max(size, fieldSize);
                alignment = Math.Max(alignment, fieldAlignment);
            }
            union.ByteSize = size;
            return (size, alignment);
        }

        /// <inheritdoc/>
        public (int, int) VisitVoidType(VoidType_v1 voidType)
        {
            return (0,1);
        }

        /// <inheritdoc/>
        public (int, int) VisitTemplate(SerializedTemplate template)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Force a size calculation for a tagged type.
        /// </summary>
        /// <param name="str">Tagged type.</param>
        /// <param name="structureAlignment">Alignment of structure members.
        /// </param>
        public void SetSize(SerializedTaggedType str, int structureAlignment)
        {
            this.structureAlignment = structureAlignment;
            var size = str.Accept(this);
            tagSizes[str] = size;
        }
    }
}

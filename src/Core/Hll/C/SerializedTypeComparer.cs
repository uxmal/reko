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

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Compares two <see cref="SerializedType"/> instances for equality.
    /// </summary>
    public class SerializedTypeComparer :
            IEqualityComparer<SerializedType>,
            ISerializedTypeVisitor<bool>
    {
        private SerializedType? y;

        /// <inheritdoc/>
        public bool Equals(SerializedType? x, SerializedType? y)
        {
            if (x is null)
                return y is null;
            if (y is null)
                return false;
            if (x.GetType() != y.GetType())
                return false;
            this.y = y;
            return x.Accept(this);
        }

        /// <inheritdoc/>
        public int GetHashCode(SerializedType? obj)
        {
            if (obj is null)
                return 0;
            int hash = obj.GetType().GetHashCode() * 11;
            if (obj is PrimitiveType_v1 prim)
                return hash ^ ((int) prim.Domain << 8) ^ prim.ByteSize;
            if (obj is PointerType_v1 ptr)
                return hash ^ GetHashCode(ptr.DataType);
            if (obj is ArrayType_v1 arr)
                return hash ^ GetHashCode(arr.ElementType);
            if (obj is StructType_v1 str)
                return hash ^ (str.Name is not null ? str.Name.GetHashCode() : 0);
            if (obj is UnionType_v1 uni)
                return hash ^ (uni.Name is not null ? uni.Name.GetHashCode() : 0);

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitCode(CodeType_v1 code)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitPrimitive(PrimitiveType_v1 pX)
        {
            var pY = (PrimitiveType_v1) y!;
            return pX.Domain == pY.Domain && pX.ByteSize == pY.ByteSize;
        }

        /// <inheritdoc/>
        public bool VisitVoidType(VoidType_v1 vX)
        {
            var vY = (VoidType_v1) y!;
            return vX == vY && vX is not null;
        }

        /// <inheritdoc/>
        public bool VisitPointer(PointerType_v1 pX)
        {
            y = ((PointerType_v1) y!).DataType!;
            return pX.DataType!.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitReference(ReferenceType_v1 rX)
        {
            y = ((ReferenceType_v1)y!).Referent!;
            return rX.Referent!.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitMemberPointer(MemberPointer_v1 mpX)
        {
            var mpY = (MemberPointer_v1) y!;
            
            y = mpY.DeclaringClass;
            if (!mpX.DeclaringClass!.Accept(this))
                return false;
            y = mpY.MemberType;
            return mpX.MemberType!.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitArray(ArrayType_v1 aX)
        {
            var aY = ((ArrayType_v1) y!);
            if (aX.Length != aY.Length)
                return false;
            y = aY.ElementType;
            return aX.ElementType!.Accept(this);
        }

        /// <inheritdoc/>
        public bool VisitEnum(SerializedEnumType e)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitSignature(SerializedSignature signature)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitStructure(StructType_v1 sX)
        {
            var sY = (StructType_v1) y!;
            return sX.Name == sY.Name && sX.Name is not null;
        }

        /// <inheritdoc/>
        public bool VisitString(StringType_v2 typedef)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitTypedef(SerializedTypedef typedef)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitTemplate(SerializedTemplate template)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitTypeReference(TypeReference_v1 typeReference)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitUnion(UnionType_v1 uX)
        {
            var uY = (UnionType_v1) y!;
            return uX.Name == uY.Name && uX.Name is not null;
        }
    }
}

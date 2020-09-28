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
    public class SerializedTypeComparer :
            IEqualityComparer<SerializedType>,
            ISerializedTypeVisitor<bool>
    {
        private SerializedType y;

        public bool Equals(SerializedType x, SerializedType y)
        {
            if (x.GetType() != y.GetType())
                return false;
            this.y = y;
            return x.Accept(this);
        }

        public int GetHashCode(SerializedType obj)
        {
            int hash = obj.GetType().GetHashCode() * 11;
            var prim = obj as PrimitiveType_v1;
            if (prim != null)
                return hash ^ ((int) prim.Domain << 8) ^ prim.ByteSize;
            var ptr = obj as PointerType_v1;
            if (ptr != null)
                return hash ^ GetHashCode(ptr.DataType);
            var arr = obj as ArrayType_v1;
            if (arr != null)
                return hash ^ GetHashCode(arr.ElementType);
            var str = obj as StructType_v1;
            if (str != null)
                return hash ^ (str.Name != null ? str.Name.GetHashCode() : 0);
            var uni = obj as UnionType_v1;
            if (uni != null)
                return hash ^ (uni.Name != null ? uni.Name.GetHashCode() : 0);

            throw new NotImplementedException();
        }

        public bool VisitCode(CodeType_v1 code)
        {
            throw new NotImplementedException();
        }

        public bool VisitPrimitive(PrimitiveType_v1 pX)
        {
            var pY = (PrimitiveType_v1) y;
            return pX.Domain == pY.Domain && pX.ByteSize == pY.ByteSize;
        }

        public bool VisitVoidType(VoidType_v1 vX)
        {
            var vY = (VoidType_v1) y;
            return vX == vY && vX != null;
        }

        public bool VisitPointer(PointerType_v1 pX)
        {
            y = ((PointerType_v1) y).DataType;
            return pX.DataType.Accept(this);
        }

        public bool VisitReference(ReferenceType_v1 rX)
        {
            y = ((ReferenceType_v1)y).Referent;
            return rX.Referent.Accept(this);
        }

        public bool VisitMemberPointer(MemberPointer_v1 mpX)
        {
            var mpY = (MemberPointer_v1) y;
            
            y = mpY.DeclaringClass;
            if (!mpX.DeclaringClass.Accept(this))
                return false;
            y = mpY.MemberType;
            return mpX.MemberType.Accept(this);
        }

        public bool VisitArray(ArrayType_v1 aX)
        {
            var aY = ((ArrayType_v1) y);
            if (aX.Length != aY.Length)
                return false;
            y = aY.ElementType;
            return aX.ElementType.Accept(this);
        }

        public bool VisitEnum(SerializedEnumType e)
        {
            throw new NotImplementedException();
        }

        public bool VisitSignature(SerializedSignature signature)
        {
            throw new NotImplementedException();
        }

        public bool VisitStructure(StructType_v1 sX)
        {
            var sY = (StructType_v1) y;
            return sX.Name == sY.Name && sX.Name != null;
        }

        public bool VisitString(StringType_v2 typedef)
        {
            throw new NotImplementedException();
        }

        public bool VisitTypedef(SerializedTypedef typedef)
        {
            throw new NotImplementedException();
        }

        public bool VisitTemplate(SerializedTemplate template)
        {
            throw new NotImplementedException();
        }

        public bool VisitTypeReference(TypeReference_v1 typeReference)
        {
            throw new NotImplementedException();
        }

        public bool VisitUnion(UnionType_v1 uX)
        {
            var uY = (UnionType_v1) y;
            return uX.Name == uY.Name && uX.Name != null;
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Tools.C2Xml
{
    public class ParserState
    {
        private Stack<int> alignments;
        
        public ParserState()
        {
            Typedefs = new HashSet<string>();
            StructsSeen = new Dictionary<string, SerializedStructType>();
            UnionsSeen = new Dictionary<string, SerializedUnionType>();
            EnumsSeen = new Dictionary<string, SerializedEnumType>();
            Constants = new Dictionary<string, int>();
            alignments = new Stack<int>();
            alignments.Push(8);
            Typedefs.Add("size_t");
            Typedefs.Add("va_list");
        }

        public HashSet<string> Typedefs { get; private set; }
        public Dictionary<string, SerializedStructType> StructsSeen { get; private set; }
        public Dictionary<string, SerializedUnionType> UnionsSeen { get; private set; }
        public Dictionary<string, SerializedEnumType> EnumsSeen { get; private set; }
        public Dictionary<string, int> Constants { get; private set; }

        public int Alignment { get { return alignments.Peek(); } }

        public void PushAlignment(int align)
        {
            alignments.Push(align);
        }

        public void PopAlignment()
        {
            alignments.Pop();
        }

        private class STComparer : IEqualityComparer<SerializedType>, 
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
                int hash = obj.GetHashCode() * 11;
                var prim = obj as SerializedPrimitiveType;
                if (prim != null)
                    return hash  ^ ((int) prim.Domain << 8) ^ prim.ByteSize;
                var ptr = obj as SerializedPointerType;
                if (ptr != null)
                    return hash  ^ GetHashCode(ptr.DataType);
                var arr = obj as SerializedArrayType;
                if (arr != null)
                    return hash  ^ GetHashCode(arr.ElementType);
                var str = obj as SerializedStructType;
                if (str != null)
                    return hash  ^ str.Name.GetHashCode();
                var uni = obj as SerializedUnionType;
                if (uni != null)
                    return hash ^ uni.Name.GetHashCode();

                throw new NotImplementedException();
            }

            public bool VisitPrimitive(SerializedPrimitiveType pX)
            {
                var pY = (SerializedPrimitiveType) y;
                return pX.Domain == pY.Domain && pX.ByteSize == pY.ByteSize;
            }

            public bool VisitPointer(SerializedPointerType pX)
            {
                y = ((SerializedPointerType) y).DataType;
                return pX.DataType.Accept(this);
            }

            public bool VisitArray(SerializedArrayType aX)
            {
                var aY = ((SerializedArrayType) y);
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

            public bool VisitStructure(SerializedStructType sX)
            {
                var sY = (SerializedStructType) y;
                return sX.Name == sY.Name && sX.Name != null;
            }

            public bool VisitTypedef(SerializedTypedef typedef)
            {
                throw new NotImplementedException();
            }

            public bool VisitTypeReference(SerializedTypeReference typeReference)
            {
                throw new NotImplementedException();
            }

            public bool VisitUnion(SerializedUnionType uX)
            {
                var uY = (SerializedUnionType) y;
                return uX.Name == uY.Name && uX.Name != null;
            }
        }
    }
}

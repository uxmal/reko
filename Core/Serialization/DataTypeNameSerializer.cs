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

using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Serialization
{
    public class DataTypeSerializer : IDataTypeVisitor<SerializedType>
    {
        public SerializedType VisitArray(ArrayType at)
        {
            var et = at.ElementType.Accept(this);
            return new ArrayType_v1 { ElementType = et, Length = at.Length };
        }
         
        public SerializedType VisitCode(CodeType c)
        {
            return new CodeType_v1();
        }

        public SerializedType VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitPrimitive(PrimitiveType pt)
        {
            return new PrimitiveType_v1
            {
                Domain = pt.Domain,
                ByteSize = pt.Size,
            };
        }

        public SerializedType VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitPointer(Pointer ptr)
        {
            return new PointerType_v1
            {
                DataType = ptr.Pointee.Accept(this),
                PointerSize = ptr.Size
            };
        }

        public SerializedType VisitString(StringType str)
        {
            return new StringType_v2 { 
                Termination = StringType_v2.ZeroTermination,    //$TODO: hardwired
                CharType = str.ElementType.Accept(this)
            };
        }

        public SerializedType VisitStructure(StructureType str)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitVoidType(VoidType ut)
        {
            throw new NotImplementedException();
        }
    }
}

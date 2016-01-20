#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Serialization
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
            var structure = new SerializedStructType {
                Name = str.Name,
                ByteSize = str.Size
            };

            if (str.Fields != null)
            {
                var fields = str.Fields.Select(f => new StructField_v1(f.Offset, f.Name, f.DataType.Accept(this)));
                structure.Fields = fields.ToArray();
            }
            return structure;
        }

        public SerializedType VisitTypeReference(TypeReference typeref)
        {
            return new SerializedTypeReference(typeref.Name);
        }

        public SerializedType VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitUnion(UnionType ut)
        {
            var union = new UnionType_v1
            {
                Name = ut.Name,
            };

            if (ut.Alternatives != null)
            {
                var alts = ut.Alternatives.Select(
                    a => new SerializedUnionAlternative(a.Value.Name, a.Value.DataType.Accept(this))
                );
                union.Alternatives = alts.ToArray();
            }
            return union;
        }

        public SerializedType VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitVoidType(VoidType ut)
        {
            return new VoidType_v1();
        }
    }
}

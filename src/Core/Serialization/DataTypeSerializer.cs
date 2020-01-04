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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Serialization
{
    public class DataTypeSerializer : IDataTypeVisitor<SerializedType>
    {
        private HashSet<string> structs = new HashSet<string>();
        private HashSet<string> unions = new HashSet<string>();

        public SerializedType VisitArray(ArrayType at)
        {
            var et = at.ElementType.Accept(this);
            return new ArrayType_v1 { ElementType = et, Length = at.Length };
        }

        public SerializedType VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitCode(CodeType c)
        {
            return new CodeType_v1();
        }

        public SerializedType VisitEnum(EnumType e)
        {
            var members = e.Members?.Select(
                    m => new SerializedEnumValue { Name = m.Key, Value = (int)m.Value })
                .ToArray();
            return new SerializedEnumType
            {
                Name = e.Name,
                Values = members,
            };
        }

        public SerializedType VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitFunctionType(FunctionType ft)
        {
            Argument_v1 ret = null;
            if (!ft.HasVoidReturn)
            {
                ret = SerializeArgument(null, null, ft.ReturnValue.DataType);
            }
            Argument_v1[] parms;
            if (ft.Parameters != null)
            {
                parms = new Argument_v1[ft.Parameters.Length];
                for (int i = 0; i < ft.Parameters.Length; ++i)
                {
                    parms[i] = SerializeArgument(ft.Parameters[i].Name, null, ft.Parameters[i].DataType);
                }
            }
            else
            {
                parms = new Argument_v1[0];
            }
            return new SerializedSignature
            {
                Arguments = parms,
                ReturnValue = ret
            };
        }

        private Argument_v1 SerializeArgument(string name, Storage stg, DataType dt)
        {
            return new Argument_v1
            {
                Name = name,
                //    Kind = arg.Storage.Serialize(),
                OutParameter = stg is OutArgumentStorage,
                Type = dt.Accept(this)
            };
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

        public SerializedType VisitReference(ReferenceTo refTo)
        {
            return new ReferenceType_v1
            {
                Referent = refTo.Referent.Accept(this),
                Size = refTo.Size,
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
            var sStr = new StructType_v1
            {
                Name = str.Name,
                ByteSize = str.Size,
                ForceStructure = str.ForceStructure,
            };

            // If this is a forward reference with 0 fields or 
            // we've already serialized the structure, emit
            // a struct reference.
            if (str.Fields.Count == 0 ||
                structs.Contains(str.Name))
            {
                return sStr;
            }

            structs.Add(str.Name);
            var fields = str.Fields.Select(f => new StructField_v1(f.Offset, f.Name, f.DataType.Accept(this)));
            sStr.Fields = fields.ToArray();
            return sStr;
        }

        public SerializedType VisitTypeReference(TypeReference typeref)
        {
            return new TypeReference_v1(typeref.Name);
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

            // If this is a forward reference with 0 alternatives or
            // we've already serialized the union, emit a union
            // reference.
            if (ut.Alternatives.Count == 0 ||
                unions.Contains(ut.Name))
            {
                return union;
            }

            unions.Add(ut.Name);
            var alts = ut.Alternatives.Select(
                    a => new UnionAlternative_v1(a.Value.Name, a.Value.DataType.Accept(this))
            );
            union.Alternatives = alts.ToArray();
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

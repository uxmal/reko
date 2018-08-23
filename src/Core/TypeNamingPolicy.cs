#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Threading.Tasks;

namespace Reko.Core
{
    public class TypeNamingPolicy
    {
        private PrefixPolicy prefixPolicy = new PrefixPolicy();

        public string ShortPrefix(DataType dt)
        {
            return dt.Accept(new PrefixPolicy());
        }

        private class PrefixPolicy : IDataTypeVisitor<string>
        {
            public string VisitArray(ArrayType at)
            {
                throw new NotImplementedException();
            }

            public string VisitClass(ClassType ct)
            {
                throw new NotImplementedException();
            }

            public string VisitCode(CodeType c)
            {
                throw new NotImplementedException();
            }

            public string VisitEnum(EnumType e)
            {
                throw new NotImplementedException();
            }

            public string VisitEquivalenceClass(EquivalenceClass eq)
            {
                throw new NotImplementedException();
            }

            public string VisitFunctionType(FunctionType ft)
            {
                throw new NotImplementedException();
            }

            public string VisitMemberPointer(MemberPointer memptr)
            {
                throw new NotImplementedException();
            }

            public string VisitPointer(Pointer ptr)
            {
                throw new NotImplementedException();
            }

            public string VisitPrimitive(PrimitiveType pt)
            {
                switch (pt.Domain)
                {
                case Domain.None:
                    return "v";
                case Domain.Boolean:
                    return "f";
                case Domain.Real:
                    return "r";
                case Domain.Pointer:
                case Domain.SegPointer:
                    return "ptr";
                case Domain.Selector:
                    return "pseg";
                default:
                    switch (pt.BitSize)
                    {
                    case 8: return "b";
                    case 16: return "w";
                    case 32: return "dw";
                    case 64: return "qw";
                    case 128: return "ow";
                    case 256: return "hw";
                    default: return "n";
                    }
                }
            }

            public string VisitQualifiedType(QualifiedType qt)
            {
                throw new NotImplementedException();
            }

            public string VisitReference(ReferenceTo refTo)
            {
                throw new NotImplementedException();
            }

            public string VisitString(StringType str)
            {
                throw new NotImplementedException();
            }

            public string VisitStructure(StructureType str)
            {
                throw new NotImplementedException();
            }

            public string VisitTypeReference(TypeReference typeref)
            {
                throw new NotImplementedException();
            }

            public string VisitTypeVariable(TypeVariable tv)
            {
                throw new NotImplementedException();
            }

            public string VisitUnion(UnionType ut)
            {
                throw new NotImplementedException();
            }

            public string VisitUnknownType(UnknownType ut)
            {
                throw new NotImplementedException();
            }

            public string VisitVoidType(VoidType voidType)
            {
                throw new NotImplementedException();
            }
        }

        public string GetStructureFieldName(StructureField field, string userGivenName)
        {
            if (!string.IsNullOrEmpty(userGivenName))
                return userGivenName;
            var prefix = field.DataType.Accept(this.prefixPolicy);
            return $"{prefix}{field.Offset:X4}";
        }

        public string GetUnionAlternativeName(UnionAlternative alt, string userGivenName)
        {
            if (!string.IsNullOrEmpty(userGivenName))
                return userGivenName;
            return $"u{alt.Index}";
        }
    }
}

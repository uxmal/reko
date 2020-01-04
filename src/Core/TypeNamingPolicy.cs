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
        private readonly PrefixPolicy prefixPolicy = new PrefixPolicy();

        public virtual string ShortPrefix(DataType dt)
        {
            return dt.Accept(prefixPolicy);
        }

        private class PrefixPolicy : IDataTypeVisitor<string>
        {
            private const string DefaultPrefix = "t";

            public string VisitArray(ArrayType at)
            {
                return "a";
            }

            public string VisitClass(ClassType ct)
            {
                return DefaultPrefix;
            }

            public string VisitCode(CodeType c)
            {
                return DefaultPrefix;
            }

            public string VisitEnum(EnumType e)
            {
                return DefaultPrefix;
            }

            public string VisitEquivalenceClass(EquivalenceClass eq)
            {
                return DefaultPrefix;
            }

            public string VisitFunctionType(FunctionType ft)
            {
                return DefaultPrefix;
            }

            public string VisitMemberPointer(MemberPointer memptr)
            {
                return "ptr";
            }

            public string VisitPointer(Pointer ptr)
            {
                return "ptr";
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

            public string VisitReference(ReferenceTo refTo)
            {
                return DefaultPrefix;
            }

            public string VisitString(StringType str)
            {
                return "str";
            }

            public string VisitStructure(StructureType str)
            {
                return DefaultPrefix;
            }

            public string VisitTypeReference(TypeReference typeref)
            {
                return DefaultPrefix;
            }

            public string VisitTypeVariable(TypeVariable tv)
            {
                return DefaultPrefix;
            }

            public string VisitUnion(UnionType ut)
            {
                return "u";
            }

            public string VisitUnknownType(UnknownType ut)
            {
                return DefaultPrefix;
            }

            public string VisitVoidType(VoidType voidType)
            {
                return "v";
            }
        }

        public virtual string StructureFieldName(StructureField field, string userGivenName)
        {
            if (!string.IsNullOrEmpty(userGivenName))
                return userGivenName;
            var prefix = field.DataType.Accept(this.prefixPolicy);
            return $"{prefix}{field.Offset:X4}";
        }

        public virtual string UnionAlternativeName(UnionAlternative alt, string userGivenName)
        {
            if (!string.IsNullOrEmpty(userGivenName))
                return userGivenName;
            return $"u{alt.Index}";
        }
    }
}

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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests
{
    class TestSerializedTypeRenderer : ISerializedTypeVisitor<StringBuilder>
    {
        private StringBuilder sb;
        private string name;
        private string modifier;

        public TestSerializedTypeRenderer(StringBuilder sb)
        {
            this.sb = sb;
        }

        internal string Render(string modifier, string scope, string name, SerializedType sp)
        {
            this.modifier = modifier;
            this.name = name;
            if (scope is not null)
                this.name = scope + "::" + name;
            if (sp is not null)
                sp.Accept(this);
            else
                sb.Append(this.name);
            return sb.ToString();
        }

        public StringBuilder VisitPrimitive(PrimitiveType_v1 primitive)
        {
            WriteQualifier(primitive.Qualifier, true);
            switch (primitive.Domain)
            {
            case Domain.None:
                sb.Append("void");
                break;
            case Domain.Boolean:
                sb.Append("bool");
                break;
            case Domain.SignedInt:
                switch (primitive.ByteSize)
                {
                case 1: sb.Append("int8_t"); break;
                case 2: sb.Append("int16_t"); break;
                case 4: sb.Append("int32_t"); break;
                case 8: sb.Append("__int64"); break;
                default: throw new NotImplementedException();
                }
                break;
            case Domain.UnsignedInt:
                switch (primitive.ByteSize)
                {
                case 2: sb.Append("uint16_t"); break;
                case 4: sb.Append("uint32_t"); break;
                default: throw new NotImplementedException();
                }
                break;
            case Domain.Character:
                switch (primitive.ByteSize)
                {
                case 1: sb.Append("char"); break;
                case 2: sb.Append("wchar_t"); break;
                }
                break;
            case Domain.Character | Domain.UnsignedInt:
                switch (primitive.ByteSize)
                {
                case 1: sb.Append("char"); break;
                default: throw new NotImplementedException();
                }
                break;
            default:
                throw new NotSupportedException(string.Format("Domain {0} is not supported.", primitive.Domain));
            }
            if (name is not null)
                sb.AppendFormat(" {0}", name);
            return sb;
        }

        public StringBuilder VisitPointer(PointerType_v1 pointer)
        {
            var n = name;
            name = null;
            pointer.DataType.Accept(this);
            sb.AppendFormat(" *");
            WriteQualifier(pointer.Qualifier, false);
            name = n;
            if (name is not null)
                sb.AppendFormat(" {0}", name);
            return sb;
        }

        private void WriteQualifier(Qualifier q, bool padAfter)
        {
            if (padAfter)
            {
                if ((q & Qualifier.Const) != 0)
                    sb.Append("const ");
                if ((q & Qualifier.Volatile) != 0)
                    sb.Append("volatile ");
                if ((q & Qualifier.Restricted) != 0)
                    sb.Append("restricted ");
            }
            else
            {
                if ((q & Qualifier.Const) != 0)
                    sb.Append(" const");
                if ((q & Qualifier.Volatile) != 0)
                    sb.Append(" volatile");
                if ((q & Qualifier.Restricted) != 0)
                    sb.Append(" restricted");
            }
        }

        //switch (qt.Qualifier)
        //{
        //case Qualifier.Const: sb.Append("const "); break;
        //case Qualifier.Volatile: sb.Append("volatile "); break;
        //case Qualifier.Restricted: sb.Append("restrict "); break;
        //}

        public StringBuilder VisitReference(ReferenceType_v1 reference)
        {
            var n = name;
            name = null;
            reference.Referent.Accept(this);
            sb.AppendFormat(" ^");
            name = n;
            if (name is not null)
                sb.AppendFormat(" {0}", name);
            return sb;
        }

        public StringBuilder VisitMemberPointer(MemberPointer_v1 memptr)
        {
            var n = name;
            memptr.DeclaringClass.Accept(this);
            sb.Append("::*");
            sb.Append(n);
            return sb;
        }

        public StringBuilder VisitArray(ArrayType_v1 array)
        {
            throw new NotImplementedException();
        }

        public StringBuilder VisitCode(CodeType_v1 array)
        {
            throw new NotImplementedException();
        }

        public StringBuilder VisitSignature(SerializedSignature signature)
        {
            if (!string.IsNullOrEmpty(signature.Convention))
                sb.AppendFormat("{0} ", signature.Convention);
            if (!string.IsNullOrEmpty(modifier))
                sb.AppendFormat("{0}: ", modifier);
            if (signature.ReturnValue is not null && signature.ReturnValue.Type is not null)
            {
                signature.ReturnValue.Type.Accept(this);
            }
            else
            {
                sb.Append(name);
            }
            sb.Append("(");
            string sep = "";
            foreach (var arg in signature.Arguments)
            {
                sb.Append(sep);
                sep = ", ";
                this.name = arg.Name;
                arg.Type.Accept(this);
            }
            sb.Append(")");
            return sb;
        }

        public StringBuilder VisitString(StringType_v2 str)
        {
            throw new NotImplementedException();
        }

        public StringBuilder VisitStructure(StructType_v1 structure)
        {
            sb.Append(structure.Name);
            return sb;
        }

        public StringBuilder VisitTypedef(SerializedTypedef typedef)
        {
            throw new NotImplementedException();
        }

        public StringBuilder VisitTypeReference(TypeReference_v1 typeReference)
        {
            WriteQualifier(typeReference.Qualifier, true);
            sb.Append(typeReference.TypeName);
            if (name is not null)
                sb.AppendFormat(" {0}", name);
            if (typeReference.TypeArguments is not null && typeReference.TypeArguments.Length > 0)
            {
                sb.Append("<");
                var sep = "";
                foreach (var tyArg in typeReference.TypeArguments)
                {
                    sb.Append(sep);
                    tyArg.Accept(this);
                    sep = ",";
                }
                sb.Append(">");
            }
            return sb;
        }

        public StringBuilder VisitUnion(UnionType_v1 union)
        {
            throw new NotImplementedException();
        }

        public StringBuilder VisitEnum(SerializedEnumType serializedEnumType)
        {
            sb.AppendFormat("enum {0}", serializedEnumType.Name);
            return sb;
        }

        public StringBuilder VisitTemplate(SerializedTemplate template)
        {
            var n = name;
            sb.Append(template.Name);
            sb.Append("<");
            var sep = "";
            foreach (var typeArg in template.TypeArguments)
            {
                sb.Append(sep);
                typeArg.Accept(this);
            }
            sb.Append(">");

            name = n;
            if (name is not null)
                sb.AppendFormat(" {0}", name);
            return sb;
        }
        public StringBuilder VisitVoidType(VoidType_v1 serializedVoidType)
        {
            sb.Append("void");
            if (name is not null)
                sb.AppendFormat(" {0}", name);
            return sb;
        }
    }
}

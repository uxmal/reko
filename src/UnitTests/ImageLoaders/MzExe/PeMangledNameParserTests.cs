#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Types;
using Decompiler.ImageLoaders.MzExe;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.ImageLoaders.MzExe
{
    [TestFixture]
    public class PeMangledNameParserTests
    {
        private void RunTest(string expected, string parse)
        {
            var p = new PeMangledNameParser(parse);
            var sp = p.Parse();
            var sb = new StringBuilder();
            Assert.AreEqual(expected, new Renderer(sb).Render(p.Modifier, p.Scope, sp));
        }

        class Renderer : ISerializedTypeVisitor<StringBuilder>
        {
            private StringBuilder sb;
            private string name;
            private string modifier;

            public Renderer(StringBuilder sb)
            {
                this.sb = sb;
            }

            internal string Render(string modifier, string scope, SerializedProcedure sp)
            {
                this.modifier = modifier;
                this.name = sp.Name;
                if (scope != null)
                    this.name = scope + "::" + sp.Name;
                sp.Signature.Accept(this);
                return sb.ToString();
            }

            public StringBuilder VisitPrimitive(SerializedPrimitiveType primitive)
            {
                switch (primitive.Domain)
                {
                case Domain.SignedInt:
                    switch (primitive.ByteSize)
                    {
                    case 4: sb.Append("int");
                        break;
                    default: throw new NotImplementedException();
                    }
                    break;
                case Domain.Character:
                    switch (primitive.ByteSize)
                    {
                    case 1: sb.Append("char"); break;
                    default: throw new NotImplementedException();
                    }
                    break;
                case Domain.Character|Domain.UnsignedInt:
                    switch (primitive.ByteSize)
                    {
                    case 1: sb.Append("char"); break;
                    default: throw new NotImplementedException();
                    }
                    break;
                default: 
                    throw new NotSupportedException(string.Format("Domain {0} is not supported.", primitive.Domain));
                }
                if (name != null)
                    sb.AppendFormat(" {0}", name);
                return sb;
            }

            public StringBuilder VisitPointer(SerializedPointerType pointer)
            {
                var n = name;
                name = null;
                pointer.DataType.Accept(this);
                sb.AppendFormat(" *");
                name = n;
                if (name != null)
                    sb.AppendFormat(" {0}", name);
                return sb;
            }

            public StringBuilder VisitArray(SerializedArrayType array)
            {
                throw new NotImplementedException();
            }

            public StringBuilder VisitSignature(SerializedSignature signature)
            {
                if (!string.IsNullOrEmpty(signature.Convention))
                    sb.AppendFormat("{0} ", signature.Convention);
                if (!string.IsNullOrEmpty(modifier))
                    sb.AppendFormat("{0}: ", modifier);
                if (signature.ReturnValue != null && signature.ReturnValue.Type != null)
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
                    this.name = arg.Name;
                    arg.Type.Accept(this);
                }
                sb.Append(")");
                return sb;
            }

            public StringBuilder VisitStructure(SerializedStructType structure)
            {
                throw new NotImplementedException();
            }

            public StringBuilder VisitTypedef(SerializedTypedef typedef)
            {
                throw new NotImplementedException();
            }

            public StringBuilder VisitTypeReference(SerializedTypeReference typeReference)
            {
                sb.Append(typeReference.TypeName);
                if (name != null)
                    sb.AppendFormat(" {0}", name);
                if (typeReference.TypeArguments != null && typeReference.TypeArguments.Length > 0)
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

            public StringBuilder VisitUnion(SerializedUnionType union)
            {
                throw new NotImplementedException();
            }

            public StringBuilder VisitEnum(SerializedEnumType serializedEnumType)
            {
                throw new NotImplementedException();
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
                if (name != null)
                    sb.AppendFormat(" {0}", name);
                return sb;
            }
            public StringBuilder VisitVoidType(SerializedVoidType serializedVoidType)
            {
                sb.Append("void");
                if (name != null)
                    sb.AppendFormat(" {0}", name);
                return sb;
            }
        }

        [Test]
        public void PMNP_Simple()
        {
            RunTest("__cdecl int foo()", "?foo@@YAHXZ");
        }

        [Test]
        public void PMNP_stdcall()
        {
            RunTest("__stdcall int foo(char)", "?foo@@YGHD@Z");
        }

        [Test]
        public void PMNP_global_var()
        {
            RunTest("__thiscall public virtual: type_info::~type_info()", "??1type_info@@UAE@XZ");
        }

        [Test]
        public void PMNP_voidfn()
        {
            RunTest("__cdecl void terminate()", "?terminate@@YAXXZ");
        }

        [Test]
        public void PMNP_ctor()
        {
            RunTest("__thiscall public: exception::exception()", "??0exception@@QAE@XZ");
        }

        [Test]
        public void PMNP_ctor2()
        {
            //$TODO: need support for full C++ type system for this. Whew.
            RunTest("__thiscall public: exception::exception(exception *)", "??0exception@@QAE@ABV0@@Z");
        }

        [Test]
        [Ignore("Too damn hard at this time; we don't have support for template classes or procedures.")]
        public void PMNP_method_in_class_in_namespace()
        {
            RunTest(
                "__thiscall public: std::basic_string<char, std::char_traits, allocator>::c_str()",
                "?c_str@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QBEPBDXZ");
        }

        [Test]
        public void PMNP_const_ptr()
        {
            RunTest(
                "__thiscall public: int foo::foox(foo *)",
                "?foox@foo@@QBEHPBV1@@Z");
        }

        [Test]
        public void PMNP_template_method()
        {
            RunTest(
                 "__thiscall public: char * str::c_str()",
                 "?c_str@?$str@DH@@QBEPADXZ");
        }

        [Test]
        public void PMNP_template_function()
        {
            RunTest(
                "__cdecl char barzoom(int)",      //$TODO: a SerializedProcedure could be templatized.
                 "??$barzoom@N@@YADH@Z");
        }
    }
}
    
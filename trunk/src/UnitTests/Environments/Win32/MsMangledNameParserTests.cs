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

using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Environments.Win32;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Environments.Win32
{
    [TestFixture]
    public class MsMangledNameParserTests
    {
        private void RunTest(string expected, string parse)
        {
            var p = new MsMangledNameParser(parse);
            var sp = p.Parse();
            var sb = new StringBuilder();
            Assert.AreEqual(expected, new Renderer(sb).Render(p.Modifier, p.Scope, sp.Name, sp.Type));
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

            internal string Render(string modifier, string scope, string name, SerializedType sp)
            {
                this.modifier = modifier;
                this.name = name;
                if (scope != null)
                    this.name = scope + "::" + name;
                sp.Accept(this);
                return sb.ToString();
            }

            public StringBuilder VisitPrimitive(PrimitiveType_v1 primitive)
            {
                switch (primitive.Domain)
                {
                case Domain.None:
                    sb.Append("void");
                    break;
                case Domain.SignedInt:
                    switch (primitive.ByteSize)
                    {
                    case 4: sb.Append("int"); break;
                    case 8: sb.Append("__int64");break;
                    default: throw new NotImplementedException();
                    }
                    break;
                case Domain.UnsignedInt:
                    switch (primitive.ByteSize)
                    {
                    case 4: sb.Append("unsigned int");
                        break;
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
                if (name != null)
                    sb.AppendFormat(" {0}", name);
                return sb;
            }

            public StringBuilder VisitPointer(PointerType_v1 pointer)
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
                if (name != null)
                    sb.AppendFormat(" {0}", name);
                return sb;
            }
            public StringBuilder VisitVoidType(VoidType_v1 serializedVoidType)
            {
                sb.Append("void");
                if (name != null)
                    sb.AppendFormat(" {0}", name);
                return sb;
            }
        }

        [Test]
        public void PMNP_Typeinfo()
        {
            RunTest("__thiscall public virtual: void type_info::~type_info()", "??1type_info@@UAE@XZ");
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
        public void PMNP_voidfn()
        {
            RunTest("__cdecl void terminate()", "?terminate@@YAXXZ");
        }

        [Test]
        public void PMNP_ctor()
        {
            RunTest("__thiscall public: void exception::exception()", "??0exception@@QAE@XZ");
        }

        [Test]
        public void PMNP_ctor2()
        {
            //$TODO: need support for full C++ type system for this, since the 'A' in 'ABV0' is a C++ reference.
            // For now, fake it with a pointer.
            RunTest("__thiscall public: void exception::exception(exception *)", "??0exception@@QAE@ABV0@@Z");
        }

        [Test]
        //$TODO: no support for C++ type system here either, the template arguments to basic_string are dropped.
        public void PMNP_method_in_class_in_namespace()
        {
            RunTest(
                "__thiscall public: char * std::basic_string::c_str()",
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

        [Test]
        public void PMNP_string_ctor()
        {
            RunTest(
                "__thiscall public: void std::basic_string::basic_string(basic_string *)",
                "??0?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QAE@ABV01@@Z");
        }

        [Test]
        public void PMNP_wchar_t()
        {
            RunTest(
                "__stdcall unsigned int HashKey(wchar_t *)",
                "??$HashKey@PB_W@@YGIPB_W@Z");
        }

        [Test]
        public void PMNP_BackRefs()
        {
            RunTest(
                "__stdcall int Foo(void *, void *, int)",
                "?Foo@@YGHPAX0H@Z");
        }

        [Test]
        public void PMNP_reg()
        {
            RunTest(
                "__thiscall public virtual: void CFile::Abort()",
            "?Abort@CFile@@UAEXXZ");
        }

        [Test]
        public void PMNP_function_ptr()
        {
            RunTest(
                "__cdecl char install(char *, __cdecl int(int *, char *) *, int)",
                "?install@@YADPADP6AHPAH0@ZJ@Z");
        }

        [Test]
        public void PMNP_numeric_template()
        {
            RunTest(
                "__thiscall public: void ATL::CSimpleStringT::~CSimpleStringT()",
                "??1?$CSimpleStringT@D$00@ATL@@QAE@XZ");
        }

        [Test]
        public void PMNP_destructor()
        {
            RunTest(
                "__thiscall public: void afx::~afx()",
                "??1afx@@QAE@XZ");
        }

        [Test]
        public void PMNP_name_with_namespace()
        {
            RunTest(
                "__thiscall public: void ATL::CSimpleStringT::CSimpleStringT(CSimpleStringT *)",
                "??0?$CSimpleStringT@_W$00@ATL@@QAE@ABV?$CSimpleStringT@_W$0A@@1@@Z");
        }

        [Test]
        public void PMNP_regression1()
        {
            RunTest(
                "__thiscall public: void AFX_MAINTAIN_STATE2::AFX_MAINTAIN_STATE2(AFX_MODULE_STATE *)",
                "??0AFX_MAINTAIN_STATE2@@QAE@PAVAFX_MODULE_STATE@@@Z");
        }

        [Test]
        public void PMNP_regression2()
        {
            RunTest(
                "__stdcall int ATL::AtlIAccessibleGetIDsOfNamesHelper(_GUID *, wchar_t * *, unsigned int, unsigned int, int *)",
                "?AtlIAccessibleGetIDsOfNamesHelper@ATL@@YGJABU_GUID@@PAPA_WIKPAJ@Z");
        }

        [Test]
        public void PMNP_regression3()
        {
            RunTest(
                "unsigned int CEditView::dwStyleDefault",
                "?dwStyleDefault@CEditView@@2KB");
        }

        [Test]
        public void PMNP_regression4()
        {
            RunTest(
                "__thiscall public: void CHtmlView::ExecWB(enum OLECMDID, enum OLECMDEXECOPT, tagVARIANT *, tagVARIANT *)",
                "?ExecWB@CHtmlView@@QAEXW4OLECMDID@@W4OLECMDEXECOPT@@PAUtagVARIANT@@2@Z ");
        }

        [Test]
        public void PMNP_Ellipses()
        {
            RunTest(
                "__cdecl void AfxTrace(char *, void ...)",
                "?AfxTrace@@YAXPBDZZ");
        }

        [Test]
        public void PMNP_int64()
        {
            RunTest(
                "__stdcall void DDV_MinMaxLongLong(CDataExchange *, __int64, __int64, __int64)",
                "?DDV_MinMaxLongLong@@YGXPAVCDataExchange@@_J11@Z");
        }

        [Test]
        public void PMNP_protected_static_field()
        { 
            RunTest(
                "unsigned int COleDropTarget::nScrollDelay",
                "?nScrollDelay@COleDropTarget@@1IA");
        }

        [Test]
        public void PMNP_regression5()
        {
            RunTest(
                "__thiscall public: int COleFrameHook::NotifyAllInPlace(int, COleFrameHook::*)",
                "?NotifyAllInPlace@COleFrameHook@@QAEHHP81@AEHH@Z@Z");
        }
        [Test]
        public void PMNP_regression6()
        {
            RunTest(
                "__thiscall public virtual: CStringT COleStreamFile::GetStorageName()", 
                "?GetStorageName@COleStreamFile@@UBE?BV?$CStringT@DV?$StrTraitMFC_DLL@DV?$ChTraitsCRT@D@ATL@@@@@ATL@@XZ");
        }

        [Test]
        [Ignore]
        public void PMNP_regression7()
        {
            RunTest(
                "__thiscall public: int CDHtmlControlSink::InvokeFromFuncInfo(CDHtmlSinkHandler::*, void)", 
                "?InvokeFromFuncInfo@CDHtmlControlSink@@QAEJP8CDHtmlSinkHandler@@AGXXZAAU_ATL_FUNC_INFO@ATL@@PAUtagDISPPARAMS@@PAUtagVARIANT@@@Z");
        }

        [Test]
        public void PMNP_regression8()
        {
            RunTest(
                "__thiscall public: CNoTrackObject * CProcessLocalObject::GetData(__stdcall public: CNoTrackObject *() *)",
                "?GetData@CProcessLocalObject@@QAEPAVCNoTrackObject@@P6GPAV2@XZ@Z");
        }

        [Test]
        public void PMNP_regression9()
        {
            RunTest(
                "__cdecl public: void CLASS_DESCRIPTOR::CLASS_DESCRIPTOR()",
                "??0CLASS_DESCRIPTOR@@QEAA@XZ");
        }
    }
}

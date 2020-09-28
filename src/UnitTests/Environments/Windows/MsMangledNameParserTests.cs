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
using Reko.Core.Types;
using Reko.Environments.Windows;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class MsMangledNameParserTests
    {
        private void RunTest(string expected, string parse)
        {
            var p = new MsMangledNameParser(parse);
            var sp = p.Parse();
            var sb = new StringBuilder();
            Assert.AreEqual(expected, new TestSerializedTypeRenderer(sb).Render(p.Modifier, p.Scope, sp.Item1, sp.Item2));
        }

        [Test]
        public void PMNP_Typeinfo()
        {
            RunTest("__thiscall public virtual: void type_info::~type_info()", "??1type_info@@UAE@XZ");
        }

        [Test]
        public void PMNP_Simple()
        {
            RunTest("__cdecl int32_t foo()", "?foo@@YAHXZ");
        }

        [Test]
        public void PMNP_stdcall()
        {
            RunTest("__stdcall int32_t foo(char)", "?foo@@YGHD@Z");
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
            RunTest(
                "__thiscall public: void exception::exception(const exception *)",
                "??0exception@@QAE@ABV0@@Z");
        }

        [Test]
        //$TODO: no support for C++ type system here either, the template arguments to basic_string are dropped.
        public void PMNP_method_in_class_in_namespace()
        {
            RunTest(
                "__thiscall public: const char * std::basic_string::c_str()",
                "?c_str@?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QBEPBDXZ");
        }

        [Test]
        public void PMNP_const_ptr()
        {
            RunTest(
                "__thiscall public: int32_t foo::foox(const foo *)",
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
                "__cdecl char barzoom(int32_t)",      //$TODO: a SerializedProcedure could be templatized.
                 "??$barzoom@N@@YADH@Z");
        }

        [Test]
        public void PMNP_string_ctor()
        {
            RunTest(
                "__thiscall public: void std::basic_string::basic_string(const basic_string *)",
                "??0?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QAE@ABV01@@Z");
        }

        [Test]
        public void PMNP_wchar_t()
        {
            RunTest(
                "__stdcall uint32_t HashKey(const wchar_t *)",
                "??$HashKey@PB_W@@YGIPB_W@Z");
        }

        [Test]
        public void PMNP_BackRefs()
        {
            RunTest(
                "__stdcall int32_t Foo(void *, void *, int32_t)",
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
                "__cdecl char install(char *, __cdecl int32_t(int32_t *, char *) *, int32_t)",
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
                "__thiscall public: void ATL::CSimpleStringT::CSimpleStringT(const CSimpleStringT *)",
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
                "__stdcall int32_t ATL::AtlIAccessibleGetIDsOfNamesHelper(const _GUID *, wchar_t * *, uint32_t, uint32_t, int32_t *)",
                "?AtlIAccessibleGetIDsOfNamesHelper@ATL@@YGJABU_GUID@@PAPA_WIKPAJ@Z");
        }

        [Test]
        public void PMNP_regression3()
        {
            RunTest(
                "uint32_t CEditView::dwStyleDefault",
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
                "__cdecl void AfxTrace(const char *, void ...)",
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
                "uint32_t COleDropTarget::nScrollDelay",
                "?nScrollDelay@COleDropTarget@@1IA");
        }

        [Test]
        public void PMNP_regression5()
        {
            RunTest(
                "__thiscall public: int32_t COleFrameHook::NotifyAllInPlace(int32_t, COleFrameHook::*)",
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
        [Ignore("")]
        public void PMNP_regression7()
        {
            RunTest(
                "__thiscall public: int32_t CDHtmlControlSink::InvokeFromFuncInfo(CDHtmlSinkHandler::*, void)", 
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

        [Test]
        public void PMNP_regression10()
        {
            RunTest(
                "__thiscall private: char A::get(int32_t, int32_t)",
                "?get@A@@AAEEHH@Z");
        }

        [Test]
        public void PMNP_regression11()
        {
            RunTest(
                "__stdcall void CopyElements(COleVariant *, const COleVariant *, int32_t)",
                "??$CopyElements@VCOleVariant@@@@YGXPAVCOleVariant@@PBV0@H@Z");
        }


        [Test]
        public void PMNP_regression12()
        {
            RunTest(
                "std::exception::`vftable'",
                "??_7exception@std@@6B@");
        }

        [Test]
        public void PMNP_regression13()
        {
            RunTest(
                "nothrow_t std::nothrow",
                "?nothrow@std@@3Unothrow_t@1@B");
        }

        [Test]
        public void PMNP_regression14()
        {
            RunTest(
                "__thiscall public: QString * QT::QString::operator =(QString *)",
                "??4QString@QT@@QAEAAV01@$$QAV01@@Z");
        }

        [Test]
        [Ignore("Issue #484")]
        public void PMNP_nested_template()
        {
            RunTest(
                "@@@",
                "?GetPropertiesPriv@CPropertySet@@AAEPAV?$hash_map@V?$basic_string@DV?$char_traits@D@_STL@@V?$allocator@D@2@@_STL@@VCValue@@VLECSimpleStringHash@@U?$equal_to@V?$basic_string@DV?$char_traits@D@_STL@@V?$allocator@D@2@@_STL@@@2@V?$allocator@U?$pair@$$CBV?$basic_string@DV?$char_traits@D@_STL@@V?$allocator@D@2@@_STL@@VCValue@@@_STL@@@2@@_STL@@XZ");
        }

        [Test]
        public void PMNP_const_char_ptr()
        {
            RunTest(
                "__cdecl int32_t foo(char * const)",
                "?foo@@YAHQAD@Z");
        }

        [Test]
        public void PMNP_const_char_const_ptr()
        {
            RunTest(
                "__cdecl int32_t foo(const char * const)",
                "?foo@@YAHQBD@Z");
        }
    }
}

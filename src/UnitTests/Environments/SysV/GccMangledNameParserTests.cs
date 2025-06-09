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

using NUnit.Framework;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.SysV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.SysV
{
    // http://mentorembedded.github.io/cxx-abi/abi.html#mangling
    // http://mentorembedded.github.io/cxx-abi/abi-examples.html#mangling
    [TestFixture]
    public class GccMangledNameParserTests
    {
        private void RunTest(string expected, string parse)
        {
            var p = new GccMangledNameParser(parse, 4);
            var sp = p.Parse();
            var sb = new StringBuilder();
            Assert.AreEqual(expected, new Renderer(sb).Render(p.Modifier, p.Scope, sp.Name, sp.Type));
        }

        private void RunTest(string test)
        {
            var p = new GccMangledNameParser(test, 4);
            var sp = p.Parse();
            var sb = new StringBuilder();
            Debug.Print(new Renderer(sb).Render(p.Modifier, p.Scope, sp.Name, sp.Type));
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
                WriteQualifier(primitive.Qualifier);
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
                    case 4: sb.Append("int"); break;
                    case 8: sb.Append("__int64"); break;
                    default: throw new NotImplementedException();
                    }
                    break;
                case Domain.UnsignedInt:
                    switch (primitive.ByteSize)
                    {
                    case 2: sb.Append("unsigned short"); break;
                    case 4: sb.Append("unsigned int"); break;
                    case 8: sb.Append("unsigned long long"); break;
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
                WriteQualifier(pointer.Qualifier);
                name = n;
                if (name is not null)
                    sb.AppendFormat(" {0}", name);
                return sb;
            }

            public StringBuilder WriteQualifier(Qualifier q)
            {
                if ((q & Qualifier.Const) != 0)
                {
                    sb.Append(" const");
                }
                if ((q & Qualifier.Volatile) != 0)
                {
                    sb.AppendFormat(" volatile");
                }
                if ((q & Qualifier.Restricted) != 0)
                {
                    sb.AppendFormat(" restricted");
                }
                return sb;
            }

            public StringBuilder VisitReference(ReferenceType_v1 reference)
            {
                var n = name;
                name = null;
                reference.Referent.Accept(this);
                sb.AppendFormat(" &");
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
                throw new NotImplementedException();
            }

            public StringBuilder VisitTypedef(SerializedTypedef typedef)
            {
                throw new NotImplementedException();
            }

            public StringBuilder VisitTypeReference(TypeReference_v1 typeReference)
            {
                if (typeReference.Scope is not null)
                {
                    sb.Append(string.Join("::", typeReference.Scope));
                    sb.Append("::");
                }
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
                WriteQualifier(typeReference.Qualifier);
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

        [Test]
        public void Gmnp_Regression1()
        {
            RunTest(
                "Example1Function(int, int *, bool, bool, bool *)",
                "_Z16Example1FunctioniPibbPb");
        }

        [Test]
        public void Gmnp_NamedArg()
        {
            RunTest(
                "foo(bar)",
                "_Z3foo3bar");
        }

        [Test]
        public void Gmnp_StaticFunction()
        {
            RunTest(
                "foo(bar)",
                "_ZL3foo3bar");
        }

        [Test]
        public void Gmnp_Regression2()
        {
            RunTest(
                "std::ostream::operator<<(int)",
                "_ZNSolsEi");
        }

        [Test]
        public void Gmnp_std()
        {
            RunTest(
                "std::_Rb_tree_increment(std::_Rb_tree_node_base *)",
                "_ZSt18_Rb_tree_incrementPSt18_Rb_tree_node_base");
        }

        [Test]
        public void Gmnp_std_string()
        {
            RunTest(
                "std::string::assign(std::string const &)",
                "_ZNSs6assignERKSs");
        }

        [Test]
        public void Gmnp_destructor()
        {
            RunTest(
                "std::string::~string()",
                "_ZNSsD1Ev");
        }

        [Test]
        public void Gmnp_regression1()
        {
            RunTest(
                "CDSMCC_T_List_T<CDSMCC_Object>::Add(CDSMCC_Object *)",
                "_ZN15CDSMCC_T_List_TI13CDSMCC_ObjectE3AddEPS0_");
        }

        [Test]
        public void Gmnp_destructor2()
        {
            RunTest(
                "TS_Module::~TS_Module()",
                "_ZN9TS_ModuleD1Ev");
        }

        [Test]
        public void Gmnp_LongLong()
        {
            RunTest(
                "SI_DSMCCNotifyModuleTimeOut(unsigned long long, void *, void *)",
                "_Z27SI_DSMCCNotifyModuleTimeOutmPvS_");
        }

        [Test]
        public void Gmnp_FunctionParameter()
        {
            RunTest(
                "foo(int(bar *) *)",
                "_Z3fooPFiP3barE");
        }

        [Test]
        public void Gmnp_ConstPtr()
        {
            RunTest(
              "foo(bar * const)",
              "_Z3fooKP3bar");
        }

        [Test]
        [Ignore("Seems we need an LR grammar to handle this.")]
        public void Gmnp_Regression3()
        {
            RunTest(
                "HBBTV::T_List_T<HBBTV::TS_ModuleInfoExt>::GetFirst()",
                "_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE8GetFirstEv");
        }

        [Test]
        [Ignore("")]
        public void Gmnp_Regression4()
        {
            RunTest(
                "HBBTV::TS_ResolveDB::TS_ResolveDB(" +
                    "HBBTV::TS_ObjectCarousel*, void*, char*, unsigned char, unsigned int, void*, " + 
                    "HBBTV_HOA_RET_T_tag (*)(int, void*, unsigned int, unsigned char*, unsigned int, DSMCC_FILE_ACK_T_tag, unsigned char*, unsigned char*, unsigned int, unsigned int), " + 
                    "int)",
                "_ZN5HBBTV12TS_ResolveDBC1EPNS_17TS_ObjectCarouselEPvPchjS3_PF19HBBTV_HOA_RET_T_tagiS3_jPhj20DSMCC_FILE_ACK_T_tagS6_S6_jjEi");
        }

        [Test]
        [Ignore("")]
        public void Gmnp_Substitutions()
        {
            RunTest(
                "N::T<int,int>::mf T<@@@",
                "_ZN1N1TIiiE2mfES0_IddE");
        }


        [Test]
        public void GmnpRegression2()
        {
            RunTest("_ZN24CDSMCCT_Send_TaskMessage6CreateEPvjS0_S0_");
            RunTest("_ZN15CDSMCC_TSModule6DecodeEPPhj");
            RunTest("_ZN15CDSMCC_TSModule10FindObjectEP12CDSMCC_TSIor");
            RunTest("_ZN15CDSMCC_TSModuleC2EP14CDSMCC_TSGroupP22CDSMCC_TSModuleInfoExt");
            RunTest("_ZN14CDSMCC_TSGroup9AddModuleEtP15CDSMCC_TSModule");
            RunTest("_ZN14CDSMCC_TSGroup6UpdateER24CDSMCCT_Send_TaskMessageP19CDSMCC_TSDiiMessage");
            RunTest("_ZN14CDSMCC_TSGroup12DecodeModuleEPhtj");
            RunTest("_ZN14CDSMCC_TSGroup16GetTransactionIdEv");
            RunTest("_ZN14CDSMCC_TSGroupC1EP23CDSMCC_TSObjectCarouseltP19CDSMCC_TSDiiMessage");
            RunTest("_ZN16CDSMCCTS_EventDBC1Ehttj");
            RunTest("_ZN13CDSMCC_Object13GetObjectKindEv");
            RunTest("_ZN16CDSMCC_Directory11GetInitNameEv");
            RunTest("_ZN13CDSMCC_Object17GetObjectCarouselEv");
            RunTest("_ZN13CDSMCC_Object8GetGroupEv");
            RunTest("_ZN27CDSMCCT_Receive_TaskMessageC2EPPv");
            RunTest("_ZN23CDSMCC_TSDiiMessageBody6DecodeEPPh");
            RunTest("_ZN19CDSMCCTS_DsiMessage5EqualEPS_");
            RunTest("_ZN23CDSMCCTS_DsiMessageBody6DecodeEPPh");
            RunTest("_ZN27CDSMCCTS_ServiceGatewayInfoC1Ev");
            RunTest("_ZN22CDSMCC_TSModuleInfoExt12FindAssocTagEtPt");
            RunTest("_ZN18CDSMCC_TSResolveDB15UpdateEvent_DINEthR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN18CDSMCC_TSResolveDB16UnSubscribeEventEPc");
            RunTest("_ZN18CDSMCC_TSResolveDB14SubscribeEventEPc");
            RunTest("_ZN18CDSMCC_TSResolveDBC2EP23CDSMCC_TSObjectCarouselPvPchj");
            RunTest("_ZN16CDSMCC_TSOnEventC1EPct");
            RunTest("_ZN20CDSMCC_TSProfileBody6DecodeEPPh");
            RunTest("_ZN29CDSMCC_TSBiopMessageSubHeader13GetObjectInfoEPPhPt");
            RunTest("_ZN19CDSMCC_TSConnBinder12FindAssocTagEtPt");
            RunTest("_ZN23CDSMCC_TSObjectLocation12GetObjectKeyEPPhS0_");
            RunTest("_ZN23CDSMCC_TSObjectLocationC1Ev");
            RunTest("_ZN13CDSMCC_TSName7GetKindEv");
            RunTest("_ZN13CDSMCC_TSName5GetIdEv");
            RunTest("_ZN18CDSMCCTS_UnMessage6DecodeEPPh");
            RunTest("_ZN23CDSMCC_TSObjectCarousel18OnStreamDescArriveER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel19OnFileOpenReqArriveER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel13OnUnMsgArriveER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel8AddGroupEP14CDSMCC_TSGroup");
            RunTest("_ZN23CDSMCC_TSObjectCarousel16OnMhegEngineIdleER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel15OnCarouselResetEv");
            RunTest("_ZN23CDSMCC_TSObjectCarousel16OnModuleCompleteER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel17SaveLORequestInfoEjPc");
            RunTest("_ZN23CDSMCC_TSObjectCarousel13OnIndexArriveER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel8FinalizeER24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel18ResetLORequestInfoEv");
            RunTest("_ZN23CDSMCC_TSObjectCarousel15OnModuleTimeOutER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel13CheckModCacheEv");
            RunTest("_ZN23CDSMCC_TSObjectCarousel17OnSTE_UnSubscribeER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel15OnSTE_SubscribeER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN23CDSMCC_TSObjectCarousel20OnFileCloseReqArriveER27CDSMCCT_Receive_TaskMessageR24CDSMCCT_Send_TaskMessage");
            RunTest("_ZN5HBBTV10TS_EventDBC1Ehttj");
            RunTest("_ZN5HBBTV19HBBTV_OC_UnCompressEPhjj");
            RunTest("_ZN5HBBTV18T_Send_TaskMessage4SendEPvjS1_S1_");
            RunTest("_ZN5HBBTV21T_Receive_TaskMessageC1EPPv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_16TS_ModuleInfoExtEED0Ev");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_16TS_ModuleInfoExtEEC2EPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE6GoNextEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_16TS_ModuleInfoExtEE10GetContentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE10GetCurrentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE7GoFirstEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE3AddEPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE8GetCountEv");
            RunTest("_ZN5HBBTV21TS_ServiceGatewayInfo6DecodeEPPh");
            RunTest("_ZN5HBBTV15TS_CompressDesc6DecodeEPPh");
            RunTest("_ZN5HBBTV15TS_CompressDescC1Ev");
            RunTest("_ZN5HBBTV13TS_DsiMessage16GetModuleIdOfSGWERt");
            RunTest("_ZN5HBBTV13TS_DsiMessage16GetAssocTagOfDiiERt");
            RunTest("_ZN5HBBTV13TS_DsiMessage21GetTransactionIdOfDiiERj");
            RunTest("_ZN5HBBTV13TS_DsiMessage5EqualEPS0_");
            RunTest("_ZN5HBBTV17TS_DsiMessageBody6DecodeEPPh");
            RunTest("_ZN5HBBTV13TS_DiiMessage12FindAssocTagEttPt");
            RunTest("_ZN5HBBTV21TS_ServiceGatewayInfoC2Ev");
            RunTest("_ZN5HBBTV17TS_DiiMessageBody6DecodeEPPh");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_10TS_OnEventEED0Ev");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_10TS_OnEventEEC1EPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_OnEventEE6GoNextEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_10TS_OnEventEE10GetContentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_OnEventEE10GetCurrentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_OnEventEE8GetCountEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_OnEventEE3AddEPS1_");
            RunTest("_ZN5HBBTV10TS_OnEvent13SetSubscribedEh");
            RunTest("_ZN5HBBTV10TS_OnEvent12IsSubscribedEv");
            RunTest("_ZN5HBBTV10TS_OnEvent15SetEventVersionEh");
            RunTest("_ZNK5HBBTV10TS_OnEvent15GetEventVersionEv");
            RunTest("_ZNK5HBBTV10TS_OnEvent10GetEventIdEv");
            RunTest("_ZNK5HBBTV10TS_OnEvent12GetEventNameEv");
            RunTest("_ZN5HBBTV28HBBTV_OC_NotifyModuleTimeOutEmPvS0_");
            RunTest("_ZN5HBBTV12TS_ResolveDB15UpdateEvent_DINEthRNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV12TS_ResolveDB16UnSubscribeEventEPc");
            RunTest("_ZN5HBBTV12TS_ResolveDB14SubscribeEventEPc");
            RunTest("_ZN5HBBTV12TS_ResolveDB8AddEventEPct");
            RunTest("_ZN5HBBTV12TS_ResolveDB11NextResolveEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB14IsParentModuleEtth");
            RunTest("_ZN5HBBTV12TS_ResolveDB6CancelEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB5ResetEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB8DownloadEtttPNS_14DSM_DirectoryCEP23CosNaming_NameComponentj");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_10TS_EventDBEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEE6RemoveEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_8TS_GroupEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE6RemoveEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE6RemoveEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_13TS_FRQueuedDBEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEE6RemoveEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE6DeleteEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_10TS_EventDBEEC1EPS1_");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_12TS_ResolveDBEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_12TS_ResolveDBEE6RemoveEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_12TS_ResolveDBEEC2EPS1_");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_13TS_FRQueuedDBEEC1EPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEE6DeleteEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_8TS_GroupEEC1EPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE8GetFirstEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_12TS_ResolveDBEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEE3AddEPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEE6GoNextEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_10TS_EventDBEE10GetContentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEE10GetCurrentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEE8GetCountEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_EventEE8GetCountEv");
            RunTest("_ZN5HBBTV8T_List_TINS_12TS_ResolveDBEE8GoNextOfEPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_12TS_ResolveDBEE3AddEPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEE3AddEPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEE6GoNextEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_13TS_FRQueuedDBEE10GetContentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEE10GetCurrentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEE8GetCountEv");
            RunTest("_ZN5HBBTV8T_List_TINS_12TS_ResolveDBEE10GetCurrentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_12TS_ResolveDBEE8GetCountEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE3AddEPS1_");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE6GoNextEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_8TS_GroupEE10GetContentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE10GetCurrentEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE7GoFirstEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEE8GetCountEv");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_12TS_ResolveDBEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_13TS_FRQueuedDBEEC1Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_GroupEEC1Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_EventDBEEC1Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_12TS_ResolveDBEEC1Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_ModuleInfoExtEEC2Ev");
            RunTest("_ZNK5HBBTV10TS_EventDB10GetEventIdEv");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel14IsSrgInSrgCTagEv");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel18ResetServiceDomainEv");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel8SetPhaseENS_12ServicePhaseE");
            RunTest("_ZNK5HBBTV13TS_FRQueuedDB9GetHoaRevEv");
            RunTest("_ZNK5HBBTV13TS_FRQueuedDB14GetAddonCBFuncEv");
            RunTest("_ZNK5HBBTV13TS_FRQueuedDB16GetBrowserCBFuncEv");
            RunTest("_ZNK5HBBTV13TS_FRQueuedDB12GetRequestIdEv");
            RunTest("_ZNK5HBBTV13TS_FRQueuedDB15GetNeedCheckSrgEv");
            RunTest("_ZNK5HBBTV13TS_FRQueuedDB11GetPathNameEv");
            RunTest("_ZNK5HBBTV13TS_FRQueuedDB11GetSenderIdEv");
            RunTest("_ZNK5HBBTV12TS_ResolveDB9GetHoaRevEv");
            RunTest("_ZNK5HBBTV12TS_ResolveDB14GetAddonCBFuncEv");
            RunTest("_ZNK5HBBTV12TS_ResolveDB16GetBrowserCBFuncEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB13SetSTE_LivingEh");
            RunTest("_ZN5HBBTV12TS_ResolveDB12IsSTE_LivingEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB14SetModuleTimerEPv");
            RunTest("_ZN5HBBTV12TS_ResolveDB18GetWaitingModuleIDEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB14GetWaitingCTagEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB11GetSenderIdEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB11GetModuleIdEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB10GetGroupIdEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB7GetStepEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB7GetPathEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB8GetStageEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB9ResetStepEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB8SetStageEh");
            RunTest("_ZN5HBBTV12TS_ResolveDB15IsWaitingResumeEtt");
            RunTest("_ZN5HBBTV12TS_ResolveDB15IsWaitingModuleEtt");
            RunTest("_ZN5HBBTV12TS_ResolveDB14IsWaitingGroupEtt");
            RunTest("_ZN5HBBTV12TS_ResolveDB11ClearReloadEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB10NeedReloadEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB6IsDoneEv");
            RunTest("_ZN5HBBTV12TS_ResolveDB9IsLoadingEv");
            RunTest("_ZN5HBBTV8TS_Group10GetCompTagEv");
            RunTest("_ZN5HBBTV8TS_Group13GetModuleListEv");
            RunTest("_ZN5HBBTV8TS_Group6GetDiiEv");
            RunTest("_ZN5HBBTV21T_Receive_TaskMessage14ExtractOptFuncEv");
            RunTest("_ZN5HBBTV9TS_Module13GetDecodeTimeEv");
            RunTest("_ZN5HBBTV13TS_DsiMessage14GetMessageBodyEv");
            RunTest("_ZN5HBBTV13TS_DsiMessageC1Ev");
            RunTest("_ZNK5HBBTV13TS_DiiMessage14GetMessageBodyEv");
            RunTest("_ZN5HBBTV13TS_DiiMessageC1Ev");
            RunTest("_ZN5HBBTV12TS_UnMessage6DecodeEPPh");
            RunTest("_ZN5HBBTV16TS_ModuleInfoExt16GetModuleTimeoutEv");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel11IsSvcDomainE20SERVICE_DOMAIN_T_tagj");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel16GetServiceDomainEP20SERVICE_DOMAIN_T_tag");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel15OnModuleTimeOutERNS_21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel17GetModuleTimeInfoEttt");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel18OnStreamDescArriveERNS_21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel9IsLoadingEv");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel16OnModuleCompleteERNS_21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel12CheckResolveEtt");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel17OnSTE_UnSubscribeERNS_21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel15OnSTE_SubscribeERNS_21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel20OnFileCloseReqArriveERNS_21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel19OnFileOpenReqArriveERNS_21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel18DownloadMainModuleEPNS_8TS_GroupE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel15DecodeUnMessageEPh");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel13OnUnMsgArriveERNS_21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel13CheckModCacheEv");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel7DestroyEv");
            RunTest("_ZN5HBBTV17TS_ObjectCarousel6CreateE20SERVICE_DOMAIN_T_tag");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_9TS_ModuleEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_9TS_ModuleEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_9TS_ModuleEED0Ev");
            RunTest("_ZN5HBBTV8TS_Group16GetTransactionIdEv");
            RunTest("_ZN5HBBTV20TS_StreamMessageBody10GetTapListEv");
            RunTest("_ZN5HBBTV16DSM_StreamEventC23GetAssocTagOfProgramUseEv");
            RunTest("_ZN5HBBTV11DSM_StreamC23GetAssocTagOfProgramUseEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_OnEventEE6RemoveEv");
            RunTest("_ZN5HBBTV8T_List_TINS_17TS_ObjectCarouselEE6RemoveEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_OnEventEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_17TS_ObjectCarouselEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_OnEventEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_17TS_ObjectCarouselEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_OnEventEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_17TS_ObjectCarouselEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_17TS_ObjectCarouselEEC2Ev");
            RunTest("_ZN5HBBTV20TS_ObjectCarouselMgrC2Ev");
            RunTest("_ZN5HBBTV18T_Send_TaskMessageC2EPPv");
            RunTest("_ZN5HBBTV21T_Receive_TaskMessage7ReceiveEi");
            RunTest("_ZN5HBBTV21T_Receive_TaskMessage12GetMessageIdEv");
            RunTest("_Z17OnSTE_UnSubscribeRN5HBBTV21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_Z15OnSTE_SubscribeRN5HBBTV21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_Z19OnFileOpenReqArriveRN5HBBTV21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_Z20OnFileOpenReqAddListRN5HBBTV21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_Z19OnNotifyStreamEventRN5HBBTV21T_Receive_TaskMessageERNS_18T_Send_TaskMessageE");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_10TS_BindingEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_BindingEE6RemoveEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_16TS_LiteComponentEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_LiteComponentEE6RemoveEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_6TS_TapEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_6TS_TapEE6RemoveEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_16TS_NameComponentEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_NameComponentEE6RemoveEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_8TS_EventEED0Ev");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_14TS_ContentTypeEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_BindingEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_LiteComponentEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_6TS_TapEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_NameComponentEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_BindingEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_LiteComponentEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_6TS_TapEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_NameComponentEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_10TS_BindingEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_LiteComponentEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_6TS_TapEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_16TS_NameComponentEED0Ev");
            RunTest("_ZN5HBBTV18TS_ServiceLocation7GetNameEv");
            RunTest("_ZN5HBBTV18TS_ServiceLocation7GetONIdEv");
            RunTest("_ZN5HBBTV18TS_ServiceLocation7GetTSIdEv");
            RunTest("_ZN5HBBTV18TS_ServiceLocation12GetserviceIdEv");
            RunTest("_ZN5HBBTV18TS_ServiceLocation13GetCarouselIdEv");
            RunTest("_ZN5HBBTV16TS_NameComponent7GetKindEv");
            RunTest("_ZN5HBBTV23TS_BiopMessageSubHeader13GetObjectInfoEPPhPt");
            RunTest("_ZN5HBBTV6TS_Ior7GetPathEv");
            RunTest("_ZN5HBBTV6TS_Ior8GetDVBRIEP17HBBTV_DVBRI_T_tag");
            RunTest("_ZN5HBBTV6TS_Ior13GetCarouselIDEv");
            RunTest("_ZN5HBBTV13TS_ConnBinder12FindAssocTagEtPt");
            RunTest("_ZN5HBBTV7TS_Name7GetKindEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_14TS_BiopMessageEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_14TS_BiopMessageEE6RemoveEv");
            RunTest("_ZN5HBBTV12T_ListItem_TINS_11DSM_ObjectCEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_11DSM_ObjectCEE6RemoveEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_EventEE6RemoveEv");
            RunTest("_ZN5HBBTV8T_List_TINS_14TS_ContentTypeEE6RemoveEv");
            RunTest("_ZN5HBBTV8T_List_TINS_14TS_BiopMessageEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_11DSM_ObjectCEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_EventEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_14TS_ContentTypeEE6DeleteEv");
            RunTest("_ZN5HBBTV8T_List_TINS_14TS_BiopMessageEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_11DSM_ObjectCEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_EventEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_14TS_ContentTypeEE11DisposeListEv");
            RunTest("_ZN5HBBTV8T_List_TINS_14TS_BiopMessageEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_11DSM_ObjectCEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_8TS_EventEED0Ev");
            RunTest("_ZN5HBBTV8T_List_TINS_14TS_ContentTypeEED0Ev");
            RunTest("_ZN5HBBTV11DSM_ObjectC13GetObjectKindEv");
            RunTest("_ZN8TS_Group6UpdateER18T_Send_TaskMessageP13TS_DiiMessage");
            RunTest("_ZN8TS_Group12DecodeModuleEPhtj");
            RunTest("_ZN8TS_Group9AddModuleEtP9TS_Module");
            RunTest("_ZN8TS_Group16GetTransactionIdEv");
            RunTest("_ZN8TS_GroupC2EP17TS_ObjectCarouseltP13TS_DiiMessage");
            RunTest("_ZN10TS_EventDBC2Ehttj");
            RunTest("_ZN14DSM_DirectoryC11GetInitNameEv");
            RunTest("_ZN11DSM_ObjectC17GetObjectCarouselEv");
            RunTest("_ZN11DSM_ObjectC8GetGroupEv");
            RunTest("_ZN5Timer14getElapsedTimeEv");
            RunTest("_ZN15TAnimatedSpriteD2Ev");
            RunTest("_ZN6TSoundD1Ev");
            RunTest("_ZN7TShader6UnBindEv");
            RunTest("_ZN7TShaderD1Ev");
            RunTest("_ZN11TCompositor14EndStencilReadEv");
            RunTest("_ZN11TCompositor15EndStencilWriteEv");

            RunTest("_ZN13TStaticBitmapD2Ev");
            RunTest("_ZN19TGamepadInputDevice11BtnActivateEv");
            RunTest("_ZN19TGamepadInputDevice16ResetBtnActivateEv");
            RunTest("_Z15RenderLogoFramev");
            RunTest("_ZN10TLuaObjectD2Ev");
            RunTest("_ZN18TSmallAnnouncementC1ERKSs");
            RunTest("_ZNK13TStatsManager7GetStatERKSsNS_6TScopeE");
            RunTest("_ZN13TDronemanagerD2Ev");
            RunTest("_ZN7TRocket9OnWallHitEP15TPhysicalObject6b2Vec2S2_");
            RunTest("_ZN13TCannonBullet11OnPlayerHitEP15TPhysicalObject6b2Vec2S2_");
            RunTest("_ZN5THole6TLayerD2Ev");
            RunTest("_ZN8TLuaItemD1Ev");
            RunTest("_ZN7TCameraD2Ev");
            RunTest("_ZN12TBeamShooter6UpdateEv");
            RunTest("_ZN16TDiscreteShooter6UpdateEv");
            RunTest("_ZN13TNewMenuState6UpdateEv");
            RunTest("_ZN13TNewMenuState9RenderHUDEv");
            RunTest("_ZN18TModuleSelectState6UpdateEv");
            RunTest("_ZN18TModuleSelectState6RenderEv");
            RunTest("_ZN15TIntroMenuState14RenderEmissiveEv");
            RunTest("_ZN15TIntroMenuState14RenderEmissiveEv");
            RunTest("_ZN6TLevel11AddBoundaryERK15TModuleBoundary");
            RunTest("_ZN16TPlayerInventoryD2Ev");
            RunTest("_Z7InitLuav");
        }


        [Test]
        [Ignore("Need to redesign how symbolic names are parsed. They are not simply strings.")]
        public void Gmnp_Regression5()
        {
            RunTest("_ZNSt6vectorIcSaIcEE15_M_range_insertIPcEEvN9__gnu_cxx17__normal_iteratorIS3_S1_EET_S7_St20forward_iterator_tag");
            RunTest("_ZNSt6vectorIP7TShaderSaIS1_EED2Ev");
            RunTest("_ZNSt6vectorI8TVector3SaIS0_EED1Ev");
            RunTest("_ZNSt6vectorI6b2Vec2SaIS0_EE17_M_default_appendEj");
            RunTest("_ZNSt6vectorIP8TTextureSaIS1_EED1Ev");
            RunTest("_ZNSt4listIP17TPathfinderPortalSaIS1_EE6removeERKS1_");
            RunTest("_ZNSt6vectorIS_IbSaIbEESaIS1_EED2Ev");
            RunTest("_ZNSt6vectorI17TPossibleReactionSaIS0_EE13_M_insert_auxIIRKS0_EEEvN9__gnu_cxx17__normal_iteratorIPS0_S2_EEDpOT_");
            RunTest("_ZNSt6vectorIiSaIiEE14_M_fill_insertEN9__gnu_cxx17__normal_iteratorIPiS1_EEjRKi");
            RunTest("_ZNSt6vectorIiSaIiEE14_M_fill_insertEN9__gnu_cxx17__normal_iteratorIPiS1_EEjRKi");
            RunTest("_ZNSt6vectorISsSaISsEED1Ev");
            RunTest("_ZSt19__move_median_firstIN9__gnu_cxx17__normal_iteratorIPSsSt6vectorISsSaISsEEEEEvT_S7_S7_");
            RunTest("_ZNSt5dequeISsSaISsEE17_M_reallocate_mapEjb");
            RunTest("_ZNSt4listIP15TPhysicalObjectSaIS1_EE6removeERKS1_");
            RunTest("_ZNSt4listIP13TStaticBitmapSaIS1_EE6removeERKS1_");
            RunTest("_ZNSt6vectorIP9b2FixtureSaIS1_EE7reserveEj");
        }
    }
}

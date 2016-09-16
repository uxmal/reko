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

using NUnit.Framework;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.SysV;
using System;
using System.Collections.Generic;
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
                if (sp != null)
                    sp.Accept(this);
                else
                    sb.Append(this.name);
                return sb.ToString();
            }

            public StringBuilder VisitPrimitive(PrimitiveType_v1 primitive)
            {
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
                    case 4:
                        sb.Append("unsigned int");
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

        /*
 _ZN5Timer14getElapsedTimeEv
_ZN15TAnimatedSpriteD2Ev
_ZNSt6vectorIcSaIcEE15_M_range_insertIPcEEvN9__gnu_cxx17__normal_iteratorIS3_S1_EET_S7_St20forward_iterator_tag
_ZN6TSoundD1Ev
_ZN7TShader6UnBindEv
_ZNSt6vectorIP7TShaderSaIS1_EED2Ev
_ZN7TShaderD1Ev
_ZN11TCompositor14EndStencilReadEv
_ZN11TCompositor15EndStencilWriteEv
_ZNSt6vectorI8TVector3SaIS0_EED1Ev
_ZN13TStaticBitmapD2Ev
_ZN19TGamepadInputDevice11BtnActivateEv
_ZN19TGamepadInputDevice16ResetBtnActivateEv
_Z15RenderLogoFramev
_ZN10TLuaObjectD2Ev
_ZNSt6vectorI6b2Vec2SaIS0_EE17_M_default_appendEj
_ZNSt6vectorIP8TTextureSaIS1_EED1Ev
_ZN18TSmallAnnouncementC1ERKSs
_ZNK13TStatsManager7GetStatERKSsNS_6TScopeE
_ZN13TDronemanagerD2Ev
_ZNSt4listIP17TPathfinderPortalSaIS1_EE6removeERKS1_
_ZN7TRocket9OnWallHitEP15TPhysicalObject6b2Vec2S2_
_ZN13TCannonBullet11OnPlayerHitEP15TPhysicalObject6b2Vec2S2_
_ZN5THole6TLayerD2Ev
_ZNSt6vectorIS_IbSaIbEESaIS1_EED2Ev
_ZN8TLuaItemD1Ev
_ZN7TCameraD2Ev
_ZN12TBeamShooter6UpdateEv
_ZN16TDiscreteShooter6UpdateEv
_ZNSt6vectorI17TPossibleReactionSaIS0_EE13_M_insert_auxIIRKS0_EEEvN9__gnu_cxx17__normal_iteratorIPS0_S2_EEDpOT_
_ZNSt6vectorIiSaIiEE14_M_fill_insertEN9__gnu_cxx17__normal_iteratorIPiS1_EEjRKi
_ZNSt6vectorIiSaIiEE14_M_fill_insertEN9__gnu_cxx17__normal_iteratorIPiS1_EEjRKi
_ZN13TNewMenuState6UpdateEv
_ZNSt6vectorISsSaISsEED1Ev
_ZN13TNewMenuState9RenderHUDEv
_ZSt19__move_median_firstIN9__gnu_cxx17__normal_iteratorIPSsSt6vectorISsSaISsEEEEEvT_S7_S7_
_ZN18TModuleSelectState6UpdateEv
_ZN18TModuleSelectState6RenderEv
_ZN15TIntroMenuState14RenderEmissiveEv
_ZN15TIntroMenuState14RenderEmissiveEv
_ZNSt5dequeISsSaISsEE17_M_reallocate_mapEjb
_ZNSt4listIP15TPhysicalObjectSaIS1_EE6removeERKS1_
_ZNSt4listIP13TStaticBitmapSaIS1_EE6removeERKS1_
_ZN6TLevel11AddBoundaryERK15TModuleBoundary
_ZN16TPlayerInventoryD2Ev
_ZNSt6vectorIP9b2FixtureSaIS1_EE7reserveEj
_ZNSt6vectorIP9b2FixtureSaIS1_EE7reserveEj
_ZNSt6vectorIP9b2FixtureSaIS1_EE7reserveEj
_Z7InitLuav
_Z7InitLuav
*/
    }
}

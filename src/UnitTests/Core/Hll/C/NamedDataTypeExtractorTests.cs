#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Core.Hll.C
{
    [TestFixture]
    public class NamedDataTypeExtractorTests
    {
        private IPlatform platform;
        private NamedDataType nt;
        private SymbolTable symbolTable;
        private FakeArchitecture arch;

        [SetUp]
        public void Setup()
        {
            var sc = new ServiceContainer();
            this.arch = new FakeArchitecture(sc);
            this.platform = new DefaultPlatform(sc, arch);
            symbolTable = new SymbolTable(platform);
        }

        public void Given_16bitPlatform()
        {
            var platformMock = new Mock<IPlatform>();
            platformMock.Setup(p => p.GetBitSizeFromCBasicType(CBasicType.Int)).Returns(16);
            platformMock.Setup(p => p.GetBitSizeFromCBasicType(CBasicType.Long)).Returns(32);
            platformMock.Setup(p => p.Architecture).Returns(arch);
            platformMock.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            this.platform = platformMock.Object;
            symbolTable = new SymbolTable(platform);
        }

        private void Run(DeclSpec[] declSpecs, Declarator decl)
        {
            var ndte = new NamedDataTypeExtractor(platform, declSpecs, symbolTable, platform.PointerType.Size);
            this.nt = ndte.GetNameAndType(decl);
        }

        private static TypeSpec SType(CTokenType type)
        {
            return new SimpleTypeSpec { Type = type };
        }

        [Test]
        public void NamedDataTypeExtractor_Ulong()
        {
            Run(new[] { SType(CTokenType.Unsigned), SType(CTokenType.Long) },
                new IdDeclarator("Bob"));

            Assert.AreEqual("Bob", nt.Name);
            Assert.AreEqual("prim(UnsignedInt,4)", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_Bool()
        {
            Run(new[] { SType(CTokenType.Bool) },
                new IdDeclarator("Bob"));

            Assert.AreEqual("Bob", nt.Name);
            Assert.AreEqual("prim(Boolean,1)", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor__Bool()
        {
            Run(new[] { SType(CTokenType._Bool) },
                new IdDeclarator("Bob"));

            Assert.AreEqual("Bob", nt.Name);
            Assert.AreEqual("prim(Boolean,1)", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_PtrChar()
        {
            Run(new[] { SType(CTokenType.Char), },
                new PointerDeclarator
                {
                    Pointee = new IdDeclarator("Sue")
                });
            Assert.AreEqual("Sue", nt.Name);
            Assert.IsInstanceOf<PointerType_v1>(nt.DataType);
            var ptr = (PointerType_v1)nt.DataType;
            Assert.IsInstanceOf<PrimitiveType_v1>(ptr.DataType);
            var p = (PrimitiveType_v1)ptr.DataType;
            Assert.AreEqual(Domain.Character, p.Domain);
            Assert.AreEqual(1, p.ByteSize);
        }

        [Test]
        public void NamedDataTypeExtractor_Pfn()
        {
            Run(new[] { SType(CTokenType.Int) },
                new FunctionDeclarator(
                    new PointerDeclarator
                    {
                        Pointee = new IdDeclarator("fn"),
                    },
                    new List<ParamDecl>
                    {
                        new ParamDecl(
                            null,
                            new List<DeclSpec>{ SType(CTokenType.Char) },
                            new IdDeclarator("ch"),
                            false)
                    }));
            Assert.AreEqual("fn", nt.Name);
            Assert.AreEqual("ptr(fn(arg(prim(SignedInt,4)),(arg(ch,prim(Character,1)))))", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_long_double_field()
        {
            Run(new[] { SType(CTokenType.Long), SType(CTokenType.Double) },
                new IdDeclarator("foo"));
            Assert.AreEqual("prim(Real,8)", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_unsigned_short()
        {
            Run(new[] { SType(CTokenType.Unsigned), SType(CTokenType.Short) },
                new IdDeclarator("foo"));
            Assert.AreEqual("prim(UnsignedInt,2)", nt.DataType.ToString());
        }

        [Test(Description = "Verifies call convention.")]
        public void NamedDataTypeExtractor_ptrchar_stdcall_fn()
        {
            Run(new[] { SType(CTokenType.Char) },
                new PointerDeclarator()
                {
                    Pointee = new CallConventionDeclarator(
                        CTokenType.__Stdcall,
                        new FunctionDeclarator(
                            new IdDeclarator("test"),
                            new List<ParamDecl>()))
                });
            Assert.AreEqual(
                "fn(__stdcall,arg(ptr(prim(Character,1))),())",
                nt.DataType.ToString());
        }

        [Test(Description = "If no reko attributes are present, don't explicitly state the kind, but let the ABI rules decide.")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_OtherAttrs()
        {
            var ndte = new NamedDataTypeExtractor(platform, Array.Empty<DeclSpec>(), symbolTable, platform.PointerType.Size);
            var (kind, _) = ndte.GetArgumentKindFromAttributes("arg", null);
            Assert.IsNull(kind);
        }

        [Test(Description = "If no reko attributes are present, don't explicitly state the kind, but let the ABI rules decide.")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_null()
        {
            var ndte = new NamedDataTypeExtractor(platform, Array.Empty<DeclSpec>(), symbolTable, platform.PointerType.Size);
            var (kind, _) = ndte.GetArgumentKindFromAttributes(
                "arg",
                new List<CAttribute>
                {
                    new CAttribute {
                         Name = new QualifiedName("foo", "bar")
                    }
                });
            Assert.IsNull(kind);
        }

        [Test(Description = "If there is a reko::arg attribute present, use it to determine kind.")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_reko_reg()
        {
            var ndte = new NamedDataTypeExtractor(platform, Array.Empty<DeclSpec>(), symbolTable, platform.PointerType.Size);
            var (kind, _) = ndte.GetArgumentKindFromAttributes(
                "arg", 
                new List<CAttribute>
                {
                    new CAttribute {
                         Name = new QualifiedName("reko", "arg"),
                         Tokens = new List<CToken> {
                             new CToken(CTokenType.Register),
                             new CToken(CTokenType.Comma),
                             new CToken(CTokenType.StringLiteral, "D0")
                         }
                    }
                });
            Assert.IsNotNull(kind);
            var sReg = (Register_v1)kind;
            Assert.AreEqual("D0", sReg.Name);
        }

        [Test(Description = "If there is a reko::fpu attribute present, use it to determine kind.")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_reko_x87_fpu()
        {
            var ndte = new NamedDataTypeExtractor(platform, Array.Empty<DeclSpec>(), symbolTable, platform.PointerType.Size);
            var (kind, _) = ndte.GetArgumentKindFromAttributes(
                "arg",
                new List<CAttribute>
                {
                    new CAttribute {
                         Name = new QualifiedName("reko", "arg"),
                         Tokens = new List<CToken> {
                             new CToken(CTokenType.Id, "fpu")
                         }
                    }
                });
            Assert.IsNotNull(kind);
            Assert.IsInstanceOf<FpuStackVariable_v1>(kind);
        }

        [Test(Description = "Handle the presence of [[reko::arg(seq,'dx','ax')]]")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_reko_seq()
        {
            var ndte = new NamedDataTypeExtractor(platform, Array.Empty<DeclSpec>(), symbolTable, platform.PointerType.Size);
            var (kind, isOut) = ndte.GetArgumentKindFromAttributes(
                "arg",
                new List<CAttribute>
                {
                    new CAttribute {
                         Name = new QualifiedName("reko", "arg"),
                         Tokens = new List<CToken> {
                             new CToken(CTokenType.Id, "seq"),
                             new CToken(CTokenType.Comma),
                             new CToken(CTokenType.StringLiteral, "dx"),
                             new CToken(CTokenType.Comma),
                             new CToken(CTokenType.StringLiteral, "ax"),
                         }
                    }
                });
            Assert.IsNotNull(kind);
            Assert.IsInstanceOf<SerializedSequence>(kind);
            var seq = (SerializedSequence) kind;
            Assert.AreEqual("dx", seq.Registers[0].Name);
            Assert.AreEqual("ax", seq.Registers[1].Name);
        }

        [Test]
        public void NamedDataTypeExtractor_thiscall_declspec()
        {
            Run(new[] { SType(CTokenType.Char) },
                new PointerDeclarator()
                {
                    Pointee = new CallConventionDeclarator(
                        CTokenType.__Thiscall,
                        new FunctionDeclarator(
                            new IdDeclarator("test"),
                            new List<ParamDecl>()))
                });
            Assert.AreEqual(
                "fn(__thiscall,arg(ptr(prim(Character,1))),())",
                nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_reference_to_ptr()
        {
            Run(new[] { SType(CTokenType.Char) },
                new PointerDeclarator
                {
                    Pointee = new ReferenceDeclarator
                    {
                        Referent = new IdDeclarator("fooReference"),
                    }
                });
            Assert.AreEqual("ref(ptr(prim(Character,1)))",
                nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_short_unsigned_int()
        {
            Run(new[] {
                SType(CTokenType.Short),
                SType(CTokenType.Unsigned),
                SType(CTokenType.Int),
                },
                new IdDeclarator("size_t"));
            Assert.AreEqual("prim(UnsignedInt,2)", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_unsigned_short_int()
        {
            Run(new[] {
                SType(CTokenType.Unsigned),
                SType(CTokenType.Short),
                SType(CTokenType.Int),
                },
                new IdDeclarator("size_t"));
            Assert.AreEqual("prim(UnsignedInt,2)", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_unsigned_long_16bitplatform()
        {
            Given_16bitPlatform();
            Run(new[] {
                SType(CTokenType.Unsigned),
                SType(CTokenType.Long),
                },
                new IdDeclarator("DWORD"));
            Assert.AreEqual("prim(UnsignedInt,4)", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_unsigned_long_int_16bitplatform()
        {
            Given_16bitPlatform();
            Run(new[] {
                SType(CTokenType.Unsigned),
                SType(CTokenType.Long),
                SType(CTokenType.Int),
                },
                new IdDeclarator("DWORD"));
            Assert.AreEqual("prim(UnsignedInt,4)", nt.DataType.ToString());
        }

        [Test(Description = "Near pointers are always 16 bit")]
        public void Ndte_near_pointer()
        {
            Run(
                new[] { SType(CTokenType.Char), },
                new PointerDeclarator
                {
                    TypeQualifierList = new List<TypeQualifier> { new TypeQualifier { Qualifier = CTokenType._Near } },
                    Pointee = new IdDeclarator("Sue")
                });
            var ptrType = (PointerType_v1) nt.DataType;
            Assert.AreEqual("ptr(prim(Character,1))", ptrType.ToString());
            Assert.AreEqual(2, ptrType.PointerSize);
        }

        [Test(Description = "Near functions have a stack delta of 2")]
        public void Ndte_near_fn()
        {
            Given_16bitPlatform();
            Run(
                new DeclSpec[] {
                    SType(CTokenType.Void), 
                    new TypeQualifier { Qualifier = CTokenType._Near },
                    new TypeQualifier { Qualifier = CTokenType.__Cdecl },
                },
                new FunctionDeclarator(
                    new IdDeclarator("exit"),
                    new List<ParamDecl>
                    {
                        new ParamDecl(
                            null,
                            new(){ SType(CTokenType.Int) },
                            new IdDeclarator("arg"),
                            false)
                    }));
            var sig = (SerializedSignature) nt.DataType;
            Assert.AreEqual(2, sig.ReturnAddressOnStack);
        }

        [Test(Description = "Handle the presence of [[reko::arg(out,seq,'dx','ax')]]")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_reko_seq_out()
        {
            var ndte = new NamedDataTypeExtractor(platform, Array.Empty<DeclSpec>(), symbolTable, platform.PointerType.Size);
            var (kind, isOutParameter) = ndte.GetArgumentKindFromAttributes(
                "arg",
                new List<CAttribute>
                {
                    new CAttribute {
                         Name = new QualifiedName("reko", "arg"),
                         Tokens = new List<CToken> {
                             new CToken(CTokenType.Id, "out"),
                             new CToken(CTokenType.Comma),
                             new CToken(CTokenType.Id, "seq"),
                             new CToken(CTokenType.Comma),
                             new CToken(CTokenType.StringLiteral, "dx"),
                             new CToken(CTokenType.Comma),
                             new CToken(CTokenType.StringLiteral, "ax"),
                         }
                    }
                });
            Assert.IsNotNull(kind);
            Assert.IsInstanceOf<SerializedSequence>(kind);
            var seq = (SerializedSequence) kind;
            Assert.AreEqual("dx", seq.Registers[0].Name);
            Assert.AreEqual("ax", seq.Registers[1].Name);
            Assert.IsTrue(isOutParameter);
        }

        [Test]
        public void NamedDataTypeExtractor_EnumValues()
        {
            var ndte = new NamedDataTypeExtractor(platform, Array.Empty<DeclSpec>(), symbolTable, platform.PointerType.Size);
            ndte.VisitEnum(new EnumeratorTypeSpec("enumFoo", new()
            {
                new Enumerator("VAL_A", new ConstExp(42)),
                new Enumerator("VAL_B", new CBinaryExpression(
                    CTokenType.Plus,
                    new CIdentifier("VAL_A"),
                    new ConstExp(2)))
            }));
            var se = (SerializedEnumType) symbolTable.Types[^1];
            Assert.AreEqual(2, se.Values.Length);
            Assert.AreEqual(42, se.Values[0].Value);
            Assert.AreEqual(44, se.Values[1].Value);
        }
    }
}

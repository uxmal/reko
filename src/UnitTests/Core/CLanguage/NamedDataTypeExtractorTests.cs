﻿#region License
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

using Reko.Core.CLanguage;
using Reko.Core.Serialization;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.UnitTests.Mocks;

namespace Reko.Core.CLanguage.UnitTests
{
    [TestFixture]
    public class NamedDataTypeExtractorTests
    {
        private Platform platform;
        private NamedDataType nt;
        private SymbolTable symbolTable;
        private FakeArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new FakeArchitecture();
            this.platform = new DefaultPlatform(null, arch);
            symbolTable = new SymbolTable(platform);
        }

        private void Run(DeclSpec[] declSpecs, Declarator decl)
        {
            var ndte = new NamedDataTypeExtractor(platform, declSpecs, symbolTable);
            this.nt = ndte.GetNameAndType(decl);
        }

        private TypeSpec SType(CTokenType type)
        {
            return new SimpleTypeSpec { Type = type };
        }

        [Test]
        public void NamedDataTypeExtractor_Ulong()
        {
            Run(new[] { SType(CTokenType.Unsigned), SType(CTokenType.Long) },
                new IdDeclarator { Name = "Bob" });

            Assert.AreEqual("Bob", nt.Name);
            Assert.AreEqual("prim(UnsignedInt,4)", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_PtrChar()
        {
            Run(new[] { SType(CTokenType.Char), },
                new PointerDeclarator
                {
                    Pointee = new IdDeclarator { Name = "Sue" }
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
                new FunctionDeclarator
                {
                    Declarator = new PointerDeclarator
                    {
                        Pointee = new IdDeclarator { Name = "fn" },
                    },
                    Parameters = new List<ParamDecl>
                    {
                        new ParamDecl {
                            DeclSpecs = new List<DeclSpec>{ SType(CTokenType.Char) },
                            Declarator = new IdDeclarator { Name="ch" },
                        }
                    }
                });
            Assert.AreEqual("fn", nt.Name);
            Assert.AreEqual("ptr(fn(arg(prim(SignedInt,4)),(arg(ch,prim(Character,1))))", nt.DataType.ToString());
        }

        [Test]
        public void NamedDataTypeExtractor_long_double_field()
        {
            Run(new[] { SType(CTokenType.Long), SType(CTokenType.Double) },
                new IdDeclarator { Name = "foo" });
            Assert.AreEqual("prim(Real,8)", nt.DataType.ToString());
        }


        [Test]
        public void NamedDataTypeExtractor_unsigned_short()
        {
            Run(new[] { SType(CTokenType.Unsigned), SType(CTokenType.Short) },
                new IdDeclarator { Name = "foo" });
            Assert.AreEqual("prim(UnsignedInt,2)", nt.DataType.ToString());
        }

        [Test(Description = "If there are not reko attributes present, don't explicitly state the kind, but let the ABI rules decide.")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_OtherAttrs()
        {
            var ndte = new NamedDataTypeExtractor(platform, new DeclSpec[0], symbolTable);
            var kind = ndte.GetArgumentKindFromAttributes(null);
            Assert.IsNull(kind);
        }

        [Test(Description = "If there are not reko attributes present, don't explicitly state the kind, but let the ABI rules decide.")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_null()
        {
            var ndte = new NamedDataTypeExtractor(platform, new DeclSpec[0], symbolTable);
            var kind = ndte.GetArgumentKindFromAttributes(new List<CAttribute>
            {
                new CAttribute {
                     Name = new QualifiedName { Components = new[] { "foo", "bar"} }
                }
            });
            Assert.IsNull(kind);
        }

        [Test(Description = "If there is a reko::reg attribute present, us it to determine kind.")]
        public void NamedDataTypeExtractor_GetArgumentKindFromAttributes_reko_reg()
        {
            var ndte = new NamedDataTypeExtractor(platform, new DeclSpec[0], symbolTable);
            var kind = ndte.GetArgumentKindFromAttributes(new List<CAttribute>
            {
                new CAttribute {
                     Name = new QualifiedName("reko", "reg"),
                     Tokens = new List<CToken> {
                         new CToken(CTokenType.StringLiteral, "D0")
                     }
                }
            });
            Assert.IsNotNull(kind);
            var sReg = (Register_v1)kind;
            Assert.AreEqual("D0", sReg.Name);
        }
    }
}

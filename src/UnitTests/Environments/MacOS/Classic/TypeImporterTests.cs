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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Pascal;
using Reko.Core.Serialization;
using Reko.Environments.MacOS.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.MacOS.Classic
{
    [TestFixture]
    public class TypeImporterTests
    {
        private TypeLibrary typelib;
        private Mock<IPlatform> platform;
        private TypeImporter typeimporter;

        [SetUp]
        public void Setup()
        {
            this.typelib = new TypeLibrary();
            this.platform = new Mock<IPlatform>();
            platform.Setup(p => p.PointerType).Returns(Reko.Core.Types.PrimitiveType.Ptr32);
        }

        /// <summary>
        /// Illustrates a regression that is holding up the analysis-development
        /// branch.
        /// </summary>
        [Test]
        public void MacTi_PtrChain()
        {
            var decls = new List<Declaration>
            {
                new TypeDeclaration("GrafPort", new Record{
                    Fields = new List<Field>
                    {
                        new Field(new List<string> { "test" }, new Primitive { Type = PrimitiveType_v1.Int32() })
                    }
                }),
                new TypeDeclaration("DialogPtr", new TypeReference("WindowPtr")),
                new TypeDeclaration("GrafPtr", new Pointer(new TypeReference("GrafPort"))),
                new TypeDeclaration("WindowPtr", new TypeReference("GrafPtr"))
            };
            Given_TypeImporter();

            typeimporter.LoadTypes(decls);

            Assert.AreEqual(32, typelib.Types["GrafPtr"].BitSize);
            Assert.AreEqual(32, typelib.Types["WindowPtr"].BitSize);
            Assert.AreEqual(32, typelib.Types["DialogPtr"].BitSize);
        }

        private void Given_TypeImporter()
        {
            var tldser = new TypeLibraryDeserializer(platform.Object, false, typelib);
            this.typeimporter = new TypeImporter(platform.Object, tldser, new Dictionary<string, Constant>(), typelib);
        }

        [Test]
        public void MacTi_Point()
        {
            var decls = new List<Declaration>
            {
                new TypeDeclaration("Point", new Record{
                    Fields = new List<Field>
                    {
                        new Field(new List<string> { "x" }, new Primitive { Type = PrimitiveType_v1.Int16() }),
                        new Field(new List<string> { "y" }, new Primitive { Type = PrimitiveType_v1.Int16() })
                    }
                }),
            };
            Given_TypeImporter();

            typeimporter.LoadTypes(decls);

            Assert.AreEqual(32, typelib.Types["Point"].BitSize);
        }

        [Test]
        public void MacTi_VariantRecord()
        {
            var decls = new List<Declaration>
            {
                new TypeDeclaration("UnionType", new Record
                {
                    VariantPart = new VariantPart
                    {
                         TagType = Primitive.Integer(),
                         Variants = new List<Variant>
                         {
                             new Variant {
                                 TagValues = new List<Exp> { new NumericLiteral(1) },
                                 Fields = new List<Field>
                                 {
                                     new Field(new List<string> { "c" }, Primitive.Char()),
                                 },
                             },
                             new Variant {
                                 TagValues = new List<Exp> { new NumericLiteral(2) },
                                 Fields = new List<Field>
                                 {
                                     new Field(new List<string> { "i" }, Primitive.Integer()),
                                 }
                             }
                         }
                    }
                })
            };
            Given_TypeImporter();

            typeimporter.LoadTypes(decls);

            Assert.AreEqual(16, typelib.Types["UnionType"].BitSize);
        }
    }
}

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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Memory;
using Reko.ImageLoaders.MzExe.CodeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.MzExe.CodeView
{
    [TestFixture]
    public class TypeBuilderTests
    {
        private Mock<IProcessorArchitecture> arch;
        private Address addrSym;
        private List<CodeViewLoader.PublicSymbol> syms;
        private Dictionary<int, TypeDefinition> typesByIndex;

        [SetUp]
        public void Setup()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.addrSym = Address.SegPtr(0x0800, 0x0010);
            this.syms = new List<CodeViewLoader.PublicSymbol>();
            this.typesByIndex = new Dictionary<int, TypeDefinition>(); 
        }

        private void Given_Symbol(string name, int typeIndex)
        {
            syms.Add(new CodeViewLoader.PublicSymbol(addrSym, typeIndex, name));
            addrSym += 0x0005;
        }

        private void Given_Type(int typeIndex, string hexString)
        {
            var bytes = BytePattern.FromHexBytes(hexString);
            var rdr = new LeImageReader(bytes);
            this.typesByIndex.Add(typeIndex, new TypeDefinition
            {
                Leaves = new object[]
                {
                    CodeViewTypeLoader.ReadLeaf(rdr)
                }
            });
        }

        private void AssertSymbols(params string[] sExpected)
        {
            var imgSyms = TypeBuilder.Build(arch.Object, typesByIndex, syms);
            Assert.AreEqual(sExpected.Length, imgSyms.Count);
            for (int i = 0; i < sExpected.Length; ++i)
            {
                var imgSym = imgSyms[i];
                string sActual;
                if (imgSym.Signature is not null)
                {
                    sActual = $"{imgSym}: {imgSym.Signature}";
                }
                else
                {
                    sActual = imgSyms[i].ToString();
                }
                Assert.AreEqual(sExpected[i], sActual);
            }
        }

        [Test]
        public void Cvtb_NoType()
        {
            Given_Symbol("foo", 0);

            AssertSymbols("foo (0800:0010)");
        }

        [Test]
        public void Cvtb_Fn_int()
        {
            Given_Type(0x200, "75 80 81 63 00 83 0102"); // Procedure C_NEAR return:129 (2: #03C1)");
            Given_Type(0x201, "80");
            Given_Symbol("foo", 0x200);

            AssertSymbols("foo (0800:0010): fn(__cdecl,arg(prim(SignedInt,2)),())");
        }

        [Test]
        public void Cvtb_Fn_float_arg()
        {
            Given_Type(0x200, "75 80 81 63 02 83 0102");
            Given_Type(0x201, "7F 83 8200 83 8800");
            Given_Symbol("foo", 0x200);

            AssertSymbols(
                "foo (0800:0010): " +
                "fn(__cdecl,arg(prim(SignedInt,2)),(arg(prim(SignedInt,4)),arg(prim(Real,4))))");
        }

        [Test]
        public void Cvtb_Fn_Structure()
        {
            Given_Type(0x200, "75 80 83 8400 63 01 83 0102");   // arg0 = ptr to structure
            Given_Type(0x201, "7F 83 0202");                    // args: ( #0202 )
            Given_Type(0x202, "7A 73 83 0302");                 // ptr to #0203
            Given_Type(0x203, "79 08 02 83 0402 83 0502 82 06 7265636f7264"); // structure definition
            Given_Type(0x204, "7F 83 C200 83 0202");
            Given_Type(0x205, "7F 82 02 7565 88 00  82 02 696D 88 04");

            Given_Symbol("foo", 0x200);

            AssertSymbols(
                "foo (0800:0010): " +
                "fn(__cdecl,arg(prim(UnsignedInt,1)),(arg(ptr(struct(record, )))))");
        }
    }
}

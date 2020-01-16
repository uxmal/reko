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

        private void AssertSymbols(params string[] sExpected)
        {
            var imgSyms = TypeBuilder.Build(arch.Object, typesByIndex, syms);
            Assert.AreEqual(sExpected.Length, imgSyms.Count);
            for (int i = 0; i < sExpected.Length; ++i)
            {
                Assert.AreEqual(sExpected[i], imgSyms[i].ToString());
            }
        }

        [Test]
        public void Cvtb_ZeroSymbol()
        {
            Given_Symbol("foo", 0);

            AssertSymbols("foo (0800:0010)");
        }
    }
}

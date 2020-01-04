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

using NUnit.Framework;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class ImportReferenceTests
    {
        [Test]
        public void NIR_Compare()
        {
            var nir1= new NamedImportReference(Address.Ptr64(0x123400), "libc.so", "malloc", SymbolType.ExternalProcedure);
            var nir2= new NamedImportReference(Address.Ptr64(0x123400), "libc.so", "malloc", SymbolType.ExternalProcedure);
            Assert.AreEqual(0, nir1.CompareTo(nir2));
        }

        [Test]
        public void NIR_Compare_No_Module()
        {
            var nirNullModule1 = new NamedImportReference(Address.Ptr64(0x123400), "libc.so", "malloc", SymbolType.ExternalProcedure);
            var nirNullModule2 = new NamedImportReference(Address.Ptr64(0x123400), null, "malloc", SymbolType.ExternalProcedure);
            Assert.AreEqual(1, nirNullModule1.CompareTo(nirNullModule2));
        }

        [Test]
        public void NIR_Compare_No_Module_2()
        {
            var nirNullModule1 = new NamedImportReference(Address.Ptr64(0x123400), null, "malloc", SymbolType.ExternalProcedure);
            var nirNullModule2 = new NamedImportReference(Address.Ptr64(0x123400), "libc.so", "malloc", SymbolType.ExternalProcedure);
            Assert.AreEqual(-1, nirNullModule1.CompareTo(nirNullModule2));
        }

        [Test]
        public void NIR_Compare_No_Module_anywhere()
        {
            var nirNullModule1 = new NamedImportReference(Address.Ptr64(0x123400), null, "malloc", SymbolType.ExternalProcedure);
            var nirNullModule2 = new NamedImportReference(Address.Ptr64(0x123400), null, "malloc", SymbolType.ExternalProcedure);
            Assert.AreEqual(0, nirNullModule1.CompareTo(nirNullModule2));
        }
    }
}

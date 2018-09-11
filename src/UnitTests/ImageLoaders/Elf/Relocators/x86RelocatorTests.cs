#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.ImageLoaders.Elf;
using Reko.ImageLoaders.Elf.Relocators;
using Reko.Core;
using System.ComponentModel.Design;

namespace Reko.UnitTests.ImageLoaders.Elf.Relocators
{
    [TestFixture]
    public class x86RelocatorTests
    {
        private x86Relocator relocator;
        private Program program;
        private Dictionary<int, ElfSymbol> symbols;
        private ElfLoader32 loader;

        [SetUp]
        public void Setup()
        {
            this.program = new Program();
            this.symbols = new Dictionary<int, ElfSymbol>();
            var services = new ServiceContainer();
            var elfImgLoader = new ElfImageLoader(services, "foo.elf", new byte[0]);
            this.loader = new ElfLoader32();
        }

        private void Given_x86Relocator()
        {
            this.relocator = new x86Relocator(loader, new SortedList<Address, ImageSymbol>());
        }

        private ElfRelocation Given_rel(uint uAddrRelocate, int sym, i386Rt type)
        {
            var rel = new ElfRelocation
            {
                Offset = uAddrRelocate,
                Info = (uint)type,
                SymbolIndex = sym,
            };
            return rel;
        }

        private void Given_section(string name, uint uAddress)
        {
            var section = new ElfSection
            {
                Name = name,
                Address = Address.Ptr32(uAddress)
            };
        }

        private ElfSymbol Given_symbol(int index, string name, uint uAddr)
        {
            var sym = new ElfSymbol
            {
                Name = name,
                Value = uAddr
            };
            symbols.Add(index, sym);
            return sym;
        }

        [Test]
        [Ignore("Wait for it...")]
        public void ElfRel_x86_NullSymbolValue()
        {
            var sym = Given_symbol(3, "puts", 0);

            Given_section(".plt", 0x1030);

            var rel = Given_rel(0x400C, sym: 3, type: i386Rt.R_386_JMP_SLOT);

            Given_x86Relocator();

            var symNew = relocator.RelocateEntry(program, sym, null, rel);

            Assert.AreEqual((uint)symNew.Value, 0x1030u);
        }
    }
}

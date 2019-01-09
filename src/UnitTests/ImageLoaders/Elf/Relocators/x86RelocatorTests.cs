#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
            this.program.SegmentMap = new SegmentMap(Address.Ptr32(0x1000));
            this.program.Architecture = new Reko.Arch.X86.X86ArchitectureFlat32("x86-flat-32");
            this.symbols = new Dictionary<int, ElfSymbol>();
            var services = new ServiceContainer();
            var elfImgLoader = new ElfImageLoader(services, "foo.elf", new byte[0]);
            this.loader = new ElfLoader32();
            loader.Sections.Add(new ElfSection { Name = "" });   // section 0
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

        private ImageWriter Given_section(string name, uint uAddress, int len)
        {
            var addr = Address.Ptr32(uAddress);
            var mem = new MemoryArea(addr, new byte[len]);
            var writer = new LeImageWriter(mem, 0);
            var section = new ElfSection
            {
                Name = name,
                Address = Address.Ptr32(uAddress)
            };
            loader.Sections.Add(section);
            var seg = new ImageSegment(section.Name, mem, AccessMode.ReadWriteExecute);
            program.SegmentMap.AddSegment(seg);
            return writer;
        }

        private ElfSymbol Given_symbol(int index, string name, ElfSymbolType type, uint iSection, uint uAddr)
        {
            var sym = new ElfSymbol
            {
                Name = name,
                Value = uAddr,
                Type = type,
                SectionIndex = iSection,
            };
            symbols.Add(index, sym);
            return sym;
        }

        [Test]
        public void ElfRel_x86_NullSymbolValue()
        {
            var sym = Given_symbol(3, "puts", ElfSymbolType.STT_FUNC, 0, 0);

            Given_section(".plt", 0x1000, 0x100);

            Given_section(".got.plt", 0x4000, 0x100)
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00001036)  // Points 6 bytes into PLT stub at 0x1030
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00123400)
                .WriteLeUInt32(0x00123400);

            var rel = Given_rel(0x400C, sym: 3, type: i386Rt.R_386_JMP_SLOT);

            Given_x86Relocator();

            var symNew = relocator.RelocateEntry(program, sym, null, rel);

            Assert.AreEqual(0x1030u, (uint)symNew.Value);
        }
    }
}

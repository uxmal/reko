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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.ImageLoaders.Elf;
using Reko.ImageLoaders.Elf.Relocators;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.Elf.Relocators
{
    public abstract class ElfRelocatorTestBase
    {
        protected ElfRelocator relocator;
        protected Program program;
        protected Dictionary<int, ElfSymbol> symbols;
        protected ElfLoader loader;
        protected RelocationContext context;

        protected abstract Address GetLoadAddress();

        protected abstract IProcessorArchitecture GetArchitecture();

        protected abstract ElfRelocator CreateRelocator(ElfLoader loader, SortedList<Address, ImageSymbol> imageSymbols);

        protected abstract ElfLoader CreateLoader();

        [SetUp]
        public void Setup()
        {
            var segmentMap = new SegmentMap(GetLoadAddress());
            this.program = new Program();
            this.program.Memory = new ByteProgramMemory(segmentMap);
            this.program.SegmentMap = segmentMap;
            this.program.Architecture = GetArchitecture();
            this.symbols = new Dictionary<int, ElfSymbol>();
            var services = new ServiceContainer();
            var elfImgLoader = new ElfImageLoader(services, ImageLocation.FromUri("file:foo.elf"), Array.Empty<byte>());
            this.loader = CreateLoader();
            loader.BinaryImage.AddSection(new ElfSection { Name = "", VirtualAddress = Address.Ptr32(0) });   // section 0
        }

        protected void Given_Context()
        {
            this.context = new RelocationContext(
                loader,
                program,
                program.SegmentMap.BaseAddress,
                null,
                new());
        }

        protected void Given_Relocator()
        {
            this.relocator = CreateRelocator(loader, new SortedList<Address, ImageSymbol>());
        }

        protected ElfRelocation Given_rel<T>(uint uAddrRelocate, int sym, T type) 
        {
            var uType = ((IConvertible) type).ToUInt32(System.Globalization.CultureInfo.InvariantCulture);
            var rel = new ElfRelocation
            {
                Offset = uAddrRelocate,
                Info = uType,
                SymbolIndex = sym,
            };
            return rel;
        }

        protected ImageWriter Given_section(string name, uint uAddress, int len)
        {
            var addr = Address.Ptr32(uAddress);
            var mem = new ByteMemoryArea(addr, new byte[len]);
            var writer = new LeImageWriter(mem, 0);
            var section = new ElfSection
            {
                Name = name,
                VirtualAddress = Address.Ptr32(uAddress)
            };
            loader.BinaryImage.AddSection(section);
            var seg = new ImageSegment(section.Name, mem, AccessMode.ReadWriteExecute);
            program.SegmentMap.AddSegment(seg);
            return writer;
        }

        protected ElfSymbol Given_symbol(int index, string name, ElfSymbolType type, uint iSection, uint uAddr)
        {
            var sym = new ElfSymbol(name)
            {
                Value = uAddr,
                Type = type,
                SectionIndex = iSection,
            };
            symbols.Add(index, sym);
            return sym;
        }
    }
}

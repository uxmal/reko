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

using Reko.Core;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public abstract class ElfRelocator
    {
        public abstract ElfLoader Loader { get; }

        public abstract void Relocate(Program program);

        public abstract void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rela);

        public abstract string RelocationTypeToString(uint type);

        /// <summary>
        /// When an ELF binary has no symbols, Reko needs help in finding the location
        /// of the entry point because it's so early in the analysis we have no
        /// way of tracing pointers yet.
        /// </summary>
        /// <returns>The address of the main function, or null if it couldn't
        /// be found.
        /// </returns>
        /// <param name="program"></param>
        /// <param name="addrEntry"></param>
        /// <returns></returns>
        public Address FindMainFunction(Program program, Address addrEntry)
        {
            if (!program.SegmentMap.TryFindSegment(addrEntry, out var seg))
                return null;
            foreach (var sPattern in GetStartPatterns())
            {
                var dfa = Core.Dfa.Automaton.CreateFromPattern(sPattern.SearchPattern);
                var start = addrEntry - seg.MemoryArea.BaseAddress;
                var hits = dfa.GetMatches(seg.MemoryArea.Bytes, (int)start, (int)start + 300).ToList();
                if (hits.Count > 0)
                {
                    return GetMainFunctionAddress(program.Architecture, seg.MemoryArea, hits[0], sPattern);
                }
            }
            return null;
        }

        protected virtual Address GetMainFunctionAddress(IProcessorArchitecture arch, MemoryArea mem, int offset, StartPattern sPattern)
        {
            return null;
        }

        public virtual StartPattern[] GetStartPatterns()
        {
            return new StartPattern[0];
        }
    }

    public abstract class ElfRelocator32 : ElfRelocator
    {
        protected ElfLoader32 loader;
        protected SortedList<Address, ImageSymbol> imageSymbols;

        public ElfRelocator32(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) 
        {
            this.loader = loader;
            this.imageSymbols = imageSymbols;
        }

        public override ElfLoader Loader => loader;

        public override void Relocate(Program program)
        {
            DumpRel32(loader);
            DumpRela32(loader);

            // Get all relocations from PT_DYNAMIC segments first; these are the relocations actually
            // carried out by the operating system.
            var symbols = new List<ElfSymbol>();
            foreach (var ph in loader.ProgramHeaders.Where(p => p.p_type == ProgramHeaderType.PT_DYNAMIC))
            {
                var str = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_STRTAB);
                var symtab = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_SYMTAB);
                var rela = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_RELA);
                var relasz = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_RELASZ);
                var relaent = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_RELAENT);

                var rel = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_REL);
                var relsz = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_RELSZ);
                var relent = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_RELENT);

                var syment = loader.GetDynEntries(ph.p_offset).SingleOrDefault(de => de.d_tag == ElfLoader.DT_SYMENT);
                if (symtab == null || (rela == null && rel == null))
                    continue;
                if (str == null)
                    throw new BadImageFormatException("ELF dynamic segment lacks a string table.");
                if (syment == null)
                    throw new BadImageFormatException("ELF dynamic segment lacks the size of symbol table entries.");
                RelocationTable relTable;
                if (rela != null)
                {
                    if (relasz == null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of the relocation table.");
                    if (relaent == null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of relocation table entries.");
                    relTable = new RelaTable(this, rela.d_ptr, relasz.d_ptr);
                }
                else
                {
                    Debug.Assert(rel != null);
                    if (relsz == null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of the relocation table.");
                    if (relent == null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of relocation table entries.");
                    relTable = new RelTable(this, rel.d_ptr, relsz.d_ptr);
                }

                var offStrtab = loader.AddressToFileOffset(str.d_ptr);
                var offSymtab = loader.AddressToFileOffset(symtab.d_ptr);

                // Iterate through all the relocations 
                foreach (var symbol in relTable.RelocateEntries(program, offStrtab, offSymtab, syment.d_ptr))
                {
                    symbols.Add(symbol);
                }
            }

            foreach (var relSection in loader.Sections)
            {
                if (relSection.Type == SectionHeaderType.SHT_REL)
                {
                    var sectionSymbols = loader.Symbols[relSection.LinkedSection.FileOffset];
                    var referringSection = relSection.RelocatedSection;
                    var rdr = loader.CreateReader(relSection.FileOffset);
                    for (uint i = 0; i < relSection.EntryCount(); ++i)
                    {
                        var rel = loader.LoadRelEntry(rdr);
                        var sym = sectionSymbols[rel.SymbolIndex];
                        RelocateEntry(program, sym, referringSection, rel);
                    }
                }
                else if (relSection.Type == SectionHeaderType.SHT_RELA)
                {
                    var sectionSymbols = loader.Symbols[relSection.LinkedSection.FileOffset];
                    var referringSection = relSection.RelocatedSection;
                    var rdr = loader.CreateReader(relSection.FileOffset);
                    for (uint i = 0; i < relSection.EntryCount(); ++i)
                    {
                        var rela = loader.LoadRelaEntry(rdr);
                        var sym = sectionSymbols[rela.SymbolIndex];
                        RelocateEntry(program, sym, referringSection, rela);
                    }
                }
            }
        }

        private abstract class RelocationTable
        {
            protected ElfRelocator relocator;

            public RelocationTable(ElfRelocator relocator, ulong virtualAddress, ulong tableByteSize)
            {
                this.relocator = relocator;
                this.VirtualAddress = virtualAddress;
                this.TableSize = tableByteSize;
            }

            public ulong VirtualAddress { get; }
            public ulong TableSize { get; }

            public abstract List<ElfSymbol> RelocateEntries(Program program, ulong offStrtab, ulong offSymtab, ulong symEntrySize);

            public abstract void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection);

            public abstract ElfRelocation ReadRelocation(EndianImageReader rdr);
        }

        private class RelaTable : RelocationTable
        {
            public RelaTable(ElfRelocator32 relocator, ulong virtualAddress, ulong tableByteSize) : base(relocator, virtualAddress, tableByteSize)
            {
            }

            public override ElfRelocation ReadRelocation(EndianImageReader rdr)
            {
                return relocator.Loader.LoadRelaEntry(rdr);
            }

            public override List<ElfSymbol> RelocateEntries(Program program, ulong offStrtab, ulong offSymtab, ulong symEntrySize)
            {
                var offRela = relocator.Loader.AddressToFileOffset(VirtualAddress);
                var rdrRela = relocator.Loader.CreateReader(offRela);
                var offRelaEnd = (long)(offRela + TableSize);

                var symbols = new List<ElfSymbol>();
                while (rdrRela.Offset < offRelaEnd)
                {
                    var relocation = ReadRelocation(rdrRela);
                    var elfSym = relocator.Loader.EnsureSymbol(offSymtab, relocation.SymbolIndex, symEntrySize, offStrtab);
                    relocator.RelocateEntry(program, elfSym, null, relocation);
                    symbols.Add(elfSym);
                }
                return symbols;
            }

            public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection)
            {
                throw new NotImplementedException();
            }
        }

        private class RelTable : RelocationTable
        {
            public RelTable(ElfRelocator relocator, ulong virtualAddress, ulong tableByteSize) : base(relocator, virtualAddress, tableByteSize)
            {
            }

            public override ElfRelocation ReadRelocation(EndianImageReader rdr)
            {
                return relocator.Loader.LoadRelEntry(rdr);
            }

            public override List<ElfSymbol> RelocateEntries(Program program, ulong offStrtab, ulong offSymtab, ulong symEntrySize)
            {
                throw new NotImplementedException();
            }

            public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection)
            {
                throw new NotImplementedException();
            }
        }


        [Conditional("DEBUG")]
        protected void DumpRel32(ElfLoader32 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_REL))
            {
                Debug.Print("REL: offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection.Name,
                    section.RelocatedSection.Name);
                var symbols = loader.Symbols[section.LinkedSection.FileOffset];
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rel = Elf32_Rel.Read(rdr);
                    Debug.Print("  off:{0:X8} type:{1,-16} {3,3} {2}",
                                    rel.r_offset,
                                    RelocationTypeToString(rel.r_info & 0xFF),
                                    symbols[(int)(rel.r_info >> 8)].Name,
                                    (int)(rel.r_info >> 8));
                }
            }
        }

        [Conditional("DEBUG")]
        protected void DumpRela32(ElfLoader32 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                Debug.Print("RELA: offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection.Name,
                    section.RelocatedSection.Name);

                var symbols = loader.Symbols[section.LinkedSection.FileOffset];
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rela = Elf32_Rela.Read(rdr);
                    Debug.Print("  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2}",
                        rela.r_offset,
                        RelocationTypeToString(rela.r_info & 0xFF),
                        symbols[(int)(rela.r_info >> 8)].Name,
                        rela.r_addend,
                        (int)(rela.r_info >> 8));
                }
            }
        }
    }

    public abstract class ElfRelocator64 : ElfRelocator
    {
        protected ElfLoader64 loader;
        protected SortedList<Address, ImageSymbol> imageSymbols;

        public ElfRelocator64(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols)
        {
            this.loader = loader;
            this.imageSymbols = imageSymbols;
        }

        public override ElfLoader Loader => loader;

        public override void Relocate(Program program)
        {
            DumpRela64(loader);
            foreach (var relSection in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                var symbols = loader.Symbols[relSection.LinkedSection.FileOffset];
                var referringSection = relSection.RelocatedSection;
                var rdr = loader.CreateReader(relSection.FileOffset);
                for (uint i = 0; i < relSection.EntryCount(); ++i)
                {
                    var rela = loader.LoadRelaEntry(rdr);
                    var sym = symbols[rela.SymbolIndex];
                    RelocateEntry(program, sym, referringSection, rela);
                }
            }
        }

        [Conditional("DEBUG")]
        protected void DumpRela64(ElfLoader64 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                Debug.Print("RELA: offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection.Name,
                    section.RelocatedSection.Name);

                var symbols = loader.Symbols[section.LinkedSection.FileOffset];
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rela = Elf64_Rela.Read(rdr);
                    Debug.Print("  off:{0:X16} type:{1,-16} add:{3,-20} {4,3} {2}",
                        rela.r_offset,
                        RelocationTypeToString((uint)rela.r_info),
                        symbols[(int)(rela.r_info >> 32)].Name,
                        rela.r_addend,
                        (int)(rela.r_info >> 32));
                }
            }
        }
    }

    public class StartPattern
    {
        public string SearchPattern;
        public int MainAddressOffset;
    }
}

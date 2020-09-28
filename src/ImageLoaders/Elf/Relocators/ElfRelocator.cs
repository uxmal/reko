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

using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public abstract class ElfRelocator
    {
        private SortedList<Address, ImageSymbol> imageSymbols;

        public ElfRelocator(SortedList<Address, ImageSymbol> syms)
        {
            this.imageSymbols = syms;
        }

        public abstract ElfLoader Loader { get; }

        public IEnumerable<ElfSegment> EnumerateDynamicSegments()
        {
            return Loader.Segments
                .Where(p => p.p_type == ProgramHeaderType.PT_DYNAMIC);
        }

        /// <summary>
        /// Give the relocator a chance to adjust the image symbol.
        /// </summary>
        /// <remarks>
        /// This is helpful when adjust ARM Thumb symbols, which will have
        /// their least significant bit set to 1.
        /// </remarks>
        public virtual ImageSymbol AdjustImageSymbol(ImageSymbol sym)
        {
            return sym;
        }

        public abstract void Relocate(Program program);

        public abstract ElfSymbol RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rela);

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

        /// <summary>
        /// Creates a symbol that refers to the location of a PLT stub functon, based on the value
        /// found in the GOT for that PLT stub.
        /// </summary>
        /// <remarks>
        /// Some versions of GCC emit a R_386_JUMP_SLOT relocation where the symbol being referred to
        /// has a value of 0, where it normally would have been the virtual address of a PLT stub. Those
        /// versions of GCC put, in the GOT entry for the relocation, a pointer to the PLT stub + 6 bytes.
        /// We subtract those 6 bytes to obtain a pointer to the PLT stub.
        /// </remarks>
        protected ElfSymbol CreatePltStubSymbolFromRelocation(ElfSymbol sym, ulong gotEntry, int offset)
        {
            sym.Value = (ulong)((long)gotEntry - offset);   // skip past the Jmp [ebx+xxxxxxxx]
            return sym;
        }

        protected virtual Address GetMainFunctionAddress(IProcessorArchitecture arch, MemoryArea mem, int offset, StartPattern sPattern)
        {
            return null;
        }

        public virtual StartPattern[] GetStartPatterns()
        {
            return new StartPattern[0];
        }

        /// <summary>
        /// Relocates all relocations found in the DT_DYNAMIC segment.
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public List<ElfSymbol> RelocateDynamicSymbols(Program program)
        {
            var symbols = new List<ElfSymbol>();
            foreach (var dynSeg in EnumerateDynamicSegments())
            {
                DumpDynamicSegment(dynSeg);

                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_STRTAB, out var strtab);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_SYMTAB, out var symtab);

                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_RELA, out var rela);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_RELASZ, out var relasz);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_RELAENT, out var relaent);

                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_REL, out var rel);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_RELSZ, out var relsz);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_RELENT, out var relent);

                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_SYMENT, out var syment);
                if (symtab == null || (rela == null && rel == null))
                    continue;
                if (strtab == null)
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
                    relTable = new RelaTable(this, rela.UValue, relasz.UValue);
                }
                else
                {
                    Debug.Assert(rel != null);
                    if (relsz == null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of the relocation table.");
                    if (relent == null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of relocation table entries.");
                    relTable = new RelTable(this, rel.UValue, relsz.UValue);
                }

                var offStrtab = Loader.AddressToFileOffset(strtab.UValue);
                var offSymtab = Loader.AddressToFileOffset(symtab.UValue);

                LoadSymbolsFromDynamicSegment(dynSeg, symtab, syment, offStrtab, offSymtab);

                // Generate a symbol for each relocation.
                Debug.Print("Relocating entries in .dynamic:");
                foreach (var elfSym in relTable.RelocateEntries(program, offStrtab, offSymtab, syment.UValue))
                {
                    symbols.Add(elfSym);
                    var imgSym = Loader.CreateImageSymbol(elfSym, true);
                    if (imgSym == null || imgSym.Address.ToLinear() == 0)
                        continue;
                    imageSymbols[imgSym.Address] = imgSym;
                }

                // Relocate the DT_JMPREL table.
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_JMPREL, out var jmprel);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_PLTRELSZ, out var pltrelsz);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_PLTREL, out var pltrel);
                if (jmprel != null && pltrelsz != null && pltrel != null)
                {
                    if (pltrel.SValue == 7) // entries are in RELA format.
                    {
                        relTable = new RelaTable(this, jmprel.UValue, pltrelsz.UValue);
                    }
                    else if (pltrel.SValue == 0x11) // entries are in REL format
                    {
                        relTable = new RelTable(this, jmprel.UValue, pltrelsz.UValue);
                    }
                    else
                    {
                        //$REVIEW: bad elf format!
                        continue;
                    }

                    DebugEx.Inform(ElfImageLoader.trace, "Relocating entries in DT_JMPREL:");
                    foreach (var elfSym in relTable.RelocateEntries(program, offStrtab, offSymtab, syment.UValue))
                    {
                        symbols.Add(elfSym);
                        var imgSym = Loader.CreateImageSymbol(elfSym, true);
                        if (imgSym == null || imgSym.Address.ToLinear() == 0)
                            continue;
                        imageSymbols[imgSym.Address] = imgSym;
                        program.ImportReferences[imgSym.Address] =
                            new NamedImportReference(
                                imgSym.Address,
                                null,   // ELF imports don't specify which modile to take the import from
                                imgSym.Name,
                                imgSym.Type);
                    }
                }
            }
            return symbols;
        }

        private void LoadSymbolsFromDynamicSegment(ElfSegment dynSeg, ElfDynamicEntry symtab, ElfDynamicEntry syment, ulong offStrtab, ulong offSymtab)
        {
            // Sadly, the ELF format has no way to locate the end of the symbols in a DT_DYNAMIC segment.
            // We guess instead...
            var addrEnd = Loader.GuessAreaEnd(symtab.UValue, dynSeg);
            if (addrEnd != 0)
            {
                // We have found some symbols to ensure.
                DebugEx.Verbose(ElfImageLoader.trace, "== Symbols in the DT_DYNAMIC segment");
                int i = 0;
                for (ulong uSymAddr = symtab.UValue; uSymAddr < addrEnd; uSymAddr += syment.UValue)
                {
                    var elfSym = Loader.EnsureSymbol(offSymtab, i, syment.UValue, offStrtab);
                    ++i;
                    DebugEx.Verbose(ElfImageLoader.trace, "  {0:X8} {1}", elfSym.Value, elfSym.Name);
                    var imgSym = Loader.CreateImageSymbol(elfSym, true);
                    if (imgSym == null || imgSym.Address.ToLinear() == 0)
                        continue;
                    imageSymbols[imgSym.Address] = imgSym;
                }
            }
        }

        [Conditional("DEBUG")]
        protected abstract void DumpDynamicSegment(ElfSegment dynSeg);
        
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

            public List<ElfSymbol> RelocateEntries(Program program, ulong offStrtab, ulong offSymtab, ulong symEntrySize)
            {
                var offRela = relocator.Loader.AddressToFileOffset(VirtualAddress);
                var rdrRela = relocator.Loader.CreateReader(offRela);
                var offRelaEnd = (long)(offRela + TableSize);

                var symbols = new List<ElfSymbol>();
                while (rdrRela.Offset < offRelaEnd)
                {
                    var relocation = ReadRelocation(rdrRela);
                    var elfSym = relocator.Loader.EnsureSymbol(offSymtab, relocation.SymbolIndex, symEntrySize, offStrtab);
                    DebugEx.Verbose(ElfImageLoader.trace, "  {0}: symbol {1} type: {2} addend: {3:X}", relocation, elfSym, relocator.RelocationTypeToString((byte)relocation.Info), relocation.Addend);
                    relocator.RelocateEntry(program, elfSym, null, relocation);
                    symbols.Add(elfSym);
                }
                return symbols;
            }

            public abstract ElfRelocation ReadRelocation(EndianImageReader rdr);
        }

        private class RelaTable : RelocationTable
        {
            public RelaTable(ElfRelocator relocator, ulong virtualAddress, ulong tableByteSize) : base(relocator, virtualAddress, tableByteSize)
            {
            }

            public override ElfRelocation ReadRelocation(EndianImageReader rdr)
            {
                return relocator.Loader.LoadRelaEntry(rdr);
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
        }

        /// <summary>
        /// Allow processor specific location of the GOT pointers.
        /// </summary>
        /// <remarks>
        /// It may be the case that a specific ELF ABI dictates how you can
        /// recover GOT pointers safely from a binary. If there is no safe
        /// way to get GOT pointers, fallback to ElfLoader.LocateGotPointers which
        /// goes about it in a hacky way.
        /// </remarks>
        public virtual void LocateGotPointers(Program program, SortedList<Address, ImageSymbol> symbols)
        {
             Loader.LocateGotPointers(program, symbols);
        }
    }

    public abstract class ElfRelocator32 : ElfRelocator
    {
        protected ElfLoader32 loader;
        protected SortedList<Address, ImageSymbol> imageSymbols;

        public ElfRelocator32(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(imageSymbols)
        {
            this.loader = loader;
            this.imageSymbols = imageSymbols;
        }

        public override ElfLoader Loader => loader;

        protected override void DumpDynamicSegment(ElfSegment dynSeg)
        {
            var renderer = new DynamicSectionRenderer32(Loader, null, ElfMachine.EM_NONE);
            var sw = new StringWriter();
            renderer.Render(dynSeg.p_offset, new TextFormatter(sw));
            Debug.WriteLine(sw.ToString());
        }

        public override void Relocate(Program program)
        {
            // Get all relocations from PT_DYNAMIC segments first; these are the relocations actually
            // carried out by the operating system.
            var symbols = RelocateDynamicSymbols(program);
            if (symbols.Count > 0)
                return;

            DumpRel32(loader);
            DumpRela32(loader);
            foreach (var relSection in loader.Sections)
            {
                if (relSection.Type == SectionHeaderType.SHT_REL)
                {
                    loader.Symbols.TryGetValue(relSection.LinkedSection.FileOffset, out var sectionSymbols);
                    var referringSection = relSection.RelocatedSection;
                    var rdr = loader.CreateReader(relSection.FileOffset);
                    for (uint i = 0; i < relSection.EntryCount(); ++i)
                    {
                        var rel = loader.LoadRelEntry(rdr);
                        var sym = sectionSymbols?[rel.SymbolIndex];
                        RelocateEntry(program, sym, referringSection, rel);
                    }
                }
                else if (relSection.Type == SectionHeaderType.SHT_RELA)
                {
                    loader.Symbols.TryGetValue(relSection.LinkedSection.FileOffset, out var sectionSymbols);
                    var referringSection = relSection.RelocatedSection;
                    var rdr = loader.CreateReader(relSection.FileOffset);
                    for (uint i = 0; i < relSection.EntryCount(); ++i)
                    {
                        var rela = loader.LoadRelaEntry(rdr);
                        var sym = sectionSymbols?[rela.SymbolIndex];
                        RelocateEntry(program, sym, referringSection, rela);
                    }
                }
            }
        }


        [Conditional("DEBUG")]
        protected void DumpRel32(ElfLoader32 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_REL))
            {
                DebugEx.Inform(ElfImageLoader.trace, "REL: offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection.Name,
                    section.RelocatedSection.Name);
                loader.Symbols.TryGetValue(section.LinkedSection.FileOffset, out var symbols);
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rel = Elf32_Rel.Read(rdr);
                    DebugEx.Verbose(ElfImageLoader.trace,
                        "  off:{0:X8} type:{1,-16} {3,3} {2}",
                        rel.r_offset,
                        RelocationTypeToString(rel.r_info & 0xFF),
                        symbols!=null ? symbols[(int)(rel.r_info >> 8)].Name : "<nosym>",
                        (int)(rel.r_info >> 8));
                }
            }
        }

        [Conditional("DEBUG")]
        protected void DumpRela32(ElfLoader32 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                DebugEx.Inform(ElfImageLoader.trace, 
                    "RELA: offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection.Name,
                    section.RelocatedSection.Name);

                var symbols = loader.Symbols[section.LinkedSection.FileOffset];
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rela = Elf32_Rela.Read(rdr);
                    DebugEx.Verbose(ElfImageLoader.trace,
                        "  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2}",
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

        public ElfRelocator64(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(imageSymbols)
        {
            this.loader = loader;
            this.imageSymbols = imageSymbols;
        }

        public override ElfLoader Loader => loader;

        protected override void DumpDynamicSegment(ElfSegment dynSeg)
        {
            var renderer = new DynamicSectionRenderer64(Loader, null);
            var sw = new StringWriter();
            renderer.Render(dynSeg.p_offset, new TextFormatter(sw));
            Debug.WriteLine(sw.ToString());
        }


        public override void Relocate(Program program)
        {
            DumpRela64(loader);
            var syms = RelocateDynamicSymbols(program);
            if (syms != null)
                return;
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

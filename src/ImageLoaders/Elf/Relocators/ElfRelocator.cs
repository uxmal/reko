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

using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Memory;
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
        protected readonly SortedList<Address, ImageSymbol> imageSymbols;

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
        /// Give the relocator a chance to adjust an address.
        /// </summary>
        /// <remarks>
        /// This is helpful when adjust ARM Thumb symbols, which will have
        /// their least significant bit set to 1.
        /// </remarks>
        public virtual Address AdjustAddress(Address address) => address;

        /// <summary>
        /// Give the relocator a chance to adjust the image symbol.
        /// </summary>
        /// <remarks>
        /// This is helpful when adjust ARM Thumb symbols, which will have
        /// their least significant bit set to 1.
        /// </remarks>
        public virtual ImageSymbol AdjustImageSymbol(ImageSymbol sym) => sym;

        public virtual void Relocate(
            Program program,
            Address addrBase,
            Dictionary<ElfSymbol, Address> pltEntries)
        {
            // Get all relocations from PT_DYNAMIC segments first; these are the relocations actually
            // carried out by the operating system.
            var symbols = RelocateDynamicSymbols(program);
            if (symbols.Count > 0)
                return;

            foreach (var relSection in this.Loader.Sections)
            {
                if (relSection.RelocatedSection?.Address is null)
                    continue;

                if (relSection.Type == SectionHeaderType.SHT_REL)
                {
                    Loader.Symbols.TryGetValue(relSection.LinkedSection!.FileOffset, out var sectionSymbols);
                    var referringSection = relSection.RelocatedSection;
                    var ctx = new RelocationContext(
                        Loader,
                        program,
                        addrBase,
                        referringSection,
                        pltEntries);
                    var rdr = Loader.CreateReader(relSection.FileOffset);
                    for (uint i = 0; i < relSection.EntryCount(); ++i)
                    {
                        var rel = Loader.LoadRelEntry(rdr);
                        var sym = sectionSymbols![rel.SymbolIndex];
                        if (ctx.Update(rel, sym))
                            RelocateEntry(ctx, rel, sym);
                    }
                }
                else if (relSection.Type == SectionHeaderType.SHT_RELA)
                {
                    Loader.Symbols.TryGetValue(relSection.LinkedSection!.FileOffset, out var sectionSymbols);
                    var referringSection = relSection.RelocatedSection;
                    var rdr = Loader.CreateReader(relSection.FileOffset);
                    var ctx = new RelocationContext(
                        Loader,
                        program,
                        addrBase,
                        referringSection,
                        pltEntries);
                    for (uint i = 0; i < relSection.EntryCount(); ++i)
                    {
                        var rela = Loader.LoadRelaEntry(rdr);
                        var sym = sectionSymbols![rela.SymbolIndex];
                        if (ctx.Update(rela, sym))
                            RelocateEntry(ctx, rela, sym);
                    }
                }
            }
        }

        /// <summary>
        /// Perform the relocation specified by <paramref name="rela"/>, using the <paramref name="symbol"/> as a 
        /// reference.
        /// </summary>
        /// <param name="ctx"><see cref="RelocationContext"/> to use when relocating.</param>
        /// <param name="rela">The relocation information.</param>
        /// <param name="symbol">The <see cref="ElfSymbol"/> associated with this relocation.</param>
        /// <returns>The address of the entry in the GOT where the relocation was performed, and optionally
        /// a symbol for the PLT entry that refers to that GOT entry.</returns>
        public abstract (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol);

        public abstract string? RelocationTypeToString(uint type);

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
        public Address? FindMainFunction(Program program, Address addrEntry)
        {
            if (!program.SegmentMap.TryFindSegment(addrEntry, out var seg))
                return null;
            if (!(seg.MemoryArea is ByteMemoryArea mem))
                return null;    //$REVIEW: is ELF even compatible with non-byte granularity?
            foreach (var sPattern in GetStartPatterns())
            {
                if (sPattern.SearchPattern is null)
                    continue;
                var dfa = Core.Dfa.Automaton.CreateFromPattern(sPattern.SearchPattern);
                if (dfa is null)
                    continue;
                var start = (int)(addrEntry - seg.MemoryArea.BaseAddress);
                var end = Math.Min((int) start + 300, mem.Bytes.Length);
                var hits = dfa.GetMatches(mem.Bytes, start, end).ToList();
                if (hits.Count > 0)
                {
                    return GetMainFunctionAddress(program.Architecture, mem, hits[0], sPattern);
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

        protected virtual Address? GetMainFunctionAddress(IProcessorArchitecture arch, ByteMemoryArea mem, int offset, StartPattern sPattern)
        {
            return null;
        }

        public virtual StartPattern[] GetStartPatterns()
        {
            return Array.Empty<StartPattern>();
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
                if (symtab is null)
                    continue;
                if (strtab is null)
                    throw new BadImageFormatException("ELF dynamic segment lacks a string table.");
                if (syment is null)
                    throw new BadImageFormatException("ELF dynamic segment lacks the size of symbol table entries.");
                var offStrtab = Loader.AddressToFileOffset(strtab.UValue);
                var offSymtab = Loader.AddressToFileOffset(symtab.UValue);

                RelocationTable? relTable = null;
                if (rela is { })
                {
                    if (relasz is null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of the relocation table.");
                    if (relaent is null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of relocation table entries.");
                    relTable = new RelaTable(this, rela.UValue, relasz.UValue);
                }
                else if (rel is { })
                {
                    if (relsz is null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of the relocation table.");
                    if (relent is null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of relocation table entries.");
                    relTable = new RelTable(this, rel.UValue, relsz.UValue);
                }

                if (relTable is { })
                {
                    var arch = program.Architecture;
                    LoadSymbolsFromDynamicSegment(arch, dynSeg, symtab, syment, offStrtab, offSymtab);

                    // Generate a symbol for each relocation.
                    ElfImageLoader.trace.Inform("Relocating entries in .dynamic:");
                    foreach (var (_, elfSym, _) in relTable.RelocateEntries(program, offStrtab, offSymtab, syment.UValue))
                    {
                        symbols.Add(elfSym);
                        var imgSym = Loader.CreateImageSymbol(elfSym, arch, true);
                        // Symbols need to refer to the loaded image, if their value is 0,
                        // they are imported symbols.
                        if (imgSym == null || imgSym.Address!.ToLinear() == 0)
                            continue;
                        imageSymbols[imgSym.Address] = imgSym;
                    }
                }

                // Relocate the DT_JMPREL table.
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_JMPREL, out var jmprel);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_PLTRELSZ, out var pltrelsz);
                Loader.DynamicEntries.TryGetValue(ElfDynamicEntry.DT_PLTREL, out var pltrel);
                if (jmprel is { } && pltrelsz is { } && pltrel is { })
                {
                    if (pltrel.SValue == ElfDynamicEntry.DT_RELA) // entries are in RELA format.
                    {
                        relTable = new RelaTable(this, jmprel.UValue, pltrelsz.UValue);
                    }
                    else if (pltrel.SValue == ElfDynamicEntry.DT_REL) // entries are in REL format
                    {
                        relTable = new RelTable(this, jmprel.UValue, pltrelsz.UValue);
                    }
                    else
                    {
                        var listener = Loader.Services.RequireService<IEventListener>();
                        listener.Warn("Invalid value for DT_PLTREL: {0}", pltrel.UValue);
                        continue;
                    }

                    ElfImageLoader.trace.Inform("Relocating entries in DT_JMPREL:");
                    foreach (var (addrImport, elfSym, extraSym) in relTable.RelocateEntries(program, offStrtab, offSymtab, syment.UValue))
                    {
                        symbols.Add(elfSym);
                        if (extraSym is { })
                            symbols.Add(extraSym);
                        GenerateImageSymbol(program, addrImport, elfSym, extraSym);
                    }
                }
            }
            return symbols;
        }

        /// <summary>
        /// Generates a Reko <see cref="ImageSymbol"/> and records it in the <paramref name="program"/>.
        /// </summary>
        protected void GenerateImageSymbol(Program program, Address addrImport, ElfSymbol elfSym, ElfSymbol? extraSym)
        {
            var arch = program.Architecture;
            var imgSym = Loader.CreateImageSymbol(elfSym, arch, true);
            if (imgSym != null && imgSym.Address!.ToLinear() != 0)
            {
                imageSymbols[imgSym.Address] = imgSym;
            }

            if (extraSym != null)
            {
                var extraImgSym = Loader.CreateImageSymbol(extraSym, arch, true);
                if (extraImgSym != null)
                {
                    imageSymbols[extraImgSym.Address!] = extraImgSym;
                }
            }
            if (elfSym.SectionIndex == ElfSection.SHN_UNDEF && imgSym != null)
            {
                program.ImportReferences[addrImport] =
                    new NamedImportReference(
                        addrImport,
                        null,   // ELF imports don't specify which module to take the import from
                        imgSym.Name!,
                        imgSym.Type);
            }
        }

        private void LoadSymbolsFromDynamicSegment(
            IProcessorArchitecture arch,
            ElfSegment dynSeg,
            ElfDynamicEntry symtab,
            ElfDynamicEntry syment,
            ulong offStrtab,
            ulong offSymtab)
        {
            // Sadly, the ELF format has no way to locate the end of the symbols in a DT_DYNAMIC segment.
            // We guess instead...
            var addrEnd = Loader.GuessAreaEnd(symtab.UValue, dynSeg);
            if (addrEnd != 0)
            {
                // We have found some symbols to ensure.
                ElfImageLoader.trace.Verbose("== Symbols in the DT_DYNAMIC segment");
                int i = 0;
                for (ulong uSymAddr = symtab.UValue; uSymAddr < addrEnd; uSymAddr += syment.UValue)
                {
                    var elfSym = Loader.EnsureSymbol(offSymtab, i, syment.UValue, offStrtab);
                    ++i;
                    if (elfSym is null)
                        continue;
                    ElfImageLoader.trace.Verbose("  {0:X8} {1}", elfSym.Value, elfSym.Name);
                    var imgSym = Loader.CreateImageSymbol(elfSym, arch, true);
                    if (imgSym == null || imgSym.Address!.ToLinear() == 0)
                        continue;
                    imageSymbols[imgSym.Address] = imgSym;
                }
            }
        }

        public EndianImageReader CreateImageReader(Program program, Address addr)
        {
            //$BUG: this returns null. ELF relocators need rework to handle
            // bad images.
            return program.TryCreateImageReader(addr, out var rdr)
                ? rdr
                : null!;
        }

        public EndianImageReader CreateImageReader(Program program, IProcessorArchitecture arch, Address addr)
        {
            //$BUG: this returns null. ELF relocators need rework to handle
            // bad images.
            return program.TryCreateImageReader(arch, addr, out var rdr)
                ? rdr
                : null!;
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

            public List<(Address,ElfSymbol,ElfSymbol?)> RelocateEntries(Program program, ulong offStrtab, ulong offSymtab, ulong symEntrySize)
            {
                var offRela = relocator.Loader.AddressToFileOffset(VirtualAddress);
                var rdrRela = relocator.Loader.CreateReader(offRela);
                var offRelaEnd = (long)(offRela + TableSize);

                var symbols = new List<(Address, ElfSymbol, ElfSymbol?)>();
                var ctx = relocator.CreateRelocationContext(program, program.SegmentMap.BaseAddress, null, new());
                while (rdrRela.Offset < offRelaEnd)
                {
                    var relocation = ReadRelocation(rdrRela);
                    var elfSym = relocator.Loader.EnsureSymbol(offSymtab, relocation.SymbolIndex, symEntrySize, offStrtab);
                    if (elfSym is null)
                        continue;
                    if (!ctx.Update(relocation, elfSym))
                        continue;

                    ElfImageLoader.trace.Verbose("  {0}: symbol {1} type: {2} addend: {3}", relocation, elfSym, relocator.RelocationTypeToString((byte) relocation.Info) ?? "?", relocation.Addend.HasValue ? relocation.Addend.Value.ToString("X") : "-None-");
                    var (addrRelocation, newSym) = relocator.RelocateEntry(ctx, relocation, elfSym);
                    if (addrRelocation is not null)
                    {
                        symbols.Add((addrRelocation, elfSym, newSym));
                    }
                }
                return symbols;
            }

            public abstract ElfRelocation ReadRelocation(EndianImageReader rdr);
        }

        private RelocationContext CreateRelocationContext(
            Program program,
            Address addrBase,
            ElfSection? referringSection,
            Dictionary<ElfSymbol, Address> plt)
        {
            return new RelocationContext(
                Loader,
                program,
                addrBase,
                referringSection,
                plt);
        }

        private class RelaTable : RelocationTable
        {
            public RelaTable(ElfRelocator relocator, ulong virtualAddress, ulong tableByteSize) : base(relocator, virtualAddress, tableByteSize)
            {
            }

            public override ElfRelocation ReadRelocation(EndianImageReader rdr)
            {
                var entry = relocator.AdjustRelocation(relocator.Loader.LoadRelaEntry(rdr));
                return entry;
            }
        }

        private class RelTable : RelocationTable
        {
            public RelTable(ElfRelocator relocator, ulong virtualAddress, ulong tableByteSize) : base(relocator, virtualAddress, tableByteSize)
            {
            }

            public override ElfRelocation ReadRelocation(EndianImageReader rdr)
            {
                return relocator.AdjustRelocation(relocator.Loader.LoadRelEntry(rdr));
            }
        }

        /// <summary>
        /// Some ELF spec variants, like MIPS64 (bless their hearts) deviate from the
        /// "default" interpretation of ELF relocations. This hook allows the relocator
        /// to manipulate the entry.
        /// </summary>
        /// <param name="elfRelocation">Relocation to manipulate</param>
        /// <returns></returns>
        protected virtual ElfRelocation AdjustRelocation(ElfRelocation elfRelocation)
        {
            return elfRelocation;
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

        public ElfRelocator32(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(imageSymbols)
        {
            this.loader = loader;
        }

        public override ElfLoader Loader => loader;

        protected override void DumpDynamicSegment(ElfSegment dynSeg)
        {
            var renderer = new DynamicSectionRenderer32(Loader, null!, loader.Machine);
            var sw = new StringWriter();
            renderer.Render(dynSeg.p_offset, new TextFormatter(sw));
            Debug.WriteLine(sw.ToString());
        }


        [Conditional("DEBUG")]
        protected void DumpRel32(ElfLoader32 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_REL))
            {
                ElfImageLoader.trace.Inform("REL: Offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection?.Name ?? "?",
                    section.RelocatedSection?.Name ?? "?");
                loader.Symbols.TryGetValue(section.LinkedSection!.FileOffset, out var symbols);
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rel = Elf32_Rel.Read(rdr);
                    ElfImageLoader.trace.Verbose(
                        "  off:{0:X8} type:{1,-16} {3,3} {2}",
                        rel.r_offset,
                        RelocationTypeToString(rel.r_info & 0xFF) ?? "?",
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
                ElfImageLoader.trace.Inform(
                    "RELA: Offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection!.Name,
                    section.RelocatedSection!.Name);

                var symbols = loader.Symbols[section.LinkedSection.FileOffset];
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rela = Elf32_Rela.Read(rdr);
                    ElfImageLoader.trace.Verbose(
                        "  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2}",
                        rela.r_offset,
                        RelocationTypeToString(rela.r_info & 0xFF) ?? "?",
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

        public ElfRelocator64(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols) 
            : base(imageSymbols)
        {
            this.loader = loader;
        }

        public override ElfLoader Loader => loader;

        protected override void DumpDynamicSegment(ElfSegment dynSeg)
        {
            var renderer = new DynamicSectionRenderer64(Loader, null, ElfMachine.EM_NONE);
            var sw = new StringWriter();
            renderer.Render(dynSeg.p_offset, new TextFormatter(sw));
            Debug.WriteLine(sw.ToString());
        }



        [Conditional("DEBUG")]
        protected void DumpRel64(ElfLoader64 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_REL))
            {
                ElfImageLoader.trace.Inform("REL: Offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection?.Name ?? "?",
                    section.RelocatedSection?.Name ?? "?");
                loader.Symbols.TryGetValue(section.LinkedSection!.FileOffset, out var symbols);
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rel = Elf64_Rel.Read(rdr);
                    ElfImageLoader.trace.Verbose(
                        "  off:{0:X16} type:{1,-16} {3,3} {2}",
                        rel.r_offset,
                        RelocationTypeToString((uint)rel.r_info & 0xFF) ?? "?",
                        symbols != null ? symbols[(int) (rel.r_info >> 32)].Name : "<nosym>",
                        (int) (rel.r_info >> 32));
                }
            }
        }

        [Conditional("DEBUG")]
        protected void DumpRela64(ElfLoader64 loader)
        {
            foreach (var section in loader.Sections.Where(s => 
                s.Type == SectionHeaderType.SHT_RELA &&
                s.LinkedSection != null && 
                s.LinkedSection.FileOffset != 0))
            {
                Debug.Print("RELA: Offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection?.Name ?? "?",
                    section.RelocatedSection?.Name ?? "?");

                var symbols = loader.Symbols[section.LinkedSection!.FileOffset];
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
        public string? SearchPattern;
        public int MainAddressOffset;
        public int MainPcRelativeOffset;
    }
}

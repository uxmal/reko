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

using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            return Loader.BinaryImage.Segments
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
            CreateImageSymbols(program.Architecture, Loader.BinaryImage.DynamicSymbols);

            // Get all relocations from PT_DYNAMIC segments first; these are the relocations actually
            // carried out by the operating system.
            var symbols = RelocateDynamicSymbols(program);
            if (symbols.Count > 0)
                return;

            foreach (var (iSection, relocations) in Loader.BinaryImage.Relocations)
            {
                var relSection = Loader.BinaryImage.Sections[iSection];
                var ctx = CreateRelocationContext(
                    program,
                    program.SegmentMap.BaseAddress,
                    relSection,
                    new());
                foreach (var relocation in relocations)
                {
                    var elfsym = relocation.Symbol;
                    if (elfsym is null)
                        continue;
                    if (ctx.Update(relocation, elfsym))
                        RelocateEntry(ctx, relocation, elfsym);
                }
            }
        }

        private void CreateImageSymbols(IProcessorArchitecture arch, IReadOnlyDictionary<int, IBinarySymbol> dynamicSymbols)
        {
            foreach (ElfSymbol elfSym in dynamicSymbols.Values)
            {
                var imgSym = Loader.CreateImageSymbol(elfSym, arch, true);
                if (imgSym is null || imgSym.Address!.ToLinear() == 0)
                    continue;
                imageSymbols[imgSym.Address] = imgSym;
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
            if (seg.MemoryArea is not ByteMemoryArea mem)
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
            var result = new List<ElfSymbol>();
            var ctx = CreateRelocationContext(program, program.SegmentMap.BaseAddress, null, new());
            foreach (var relocation in Loader.BinaryImage.DynamicRelocations)
            {
                var elfSym = relocation.Symbol;
                if (elfSym is null)
                    continue;
                if (!ctx.Update(relocation, elfSym))
                    continue;
                ElfImageLoader.trace.Verbose("  {0}: symbol {1} type: {2} addend: {3}", relocation, elfSym, RelocationTypeToString((byte) relocation.Info) ?? "?", relocation.Addend.HasValue ? relocation.Addend.Value.ToString("X") : "-None-");
                var (addrRelocation, newSym) = RelocateEntry(ctx, relocation, elfSym);
                if (addrRelocation is not null)
                {
                    result.Add(elfSym);
                    if (newSym is not null)
                        result.Add(newSym);
                    var imgSym = Loader.CreateImageSymbol(elfSym, program.Architecture, true);
                    // Symbols need to refer to the loaded image, if their value is 0,
                    // they are imported symbols.
                    if (imgSym is null || imgSym.Address!.ToLinear() == 0)
                        continue;
                    imageSymbols[imgSym.Address] = imgSym;
                }
            }
            return result;
        }

        /// <summary>
        /// Generates a Reko <see cref="ImageSymbol"/> and records it in the <paramref name="program"/>.
        /// </summary>
        protected void GenerateImageSymbol(Program program, Address addrImport, ElfSymbol elfSym, ElfSymbol? extraSym)
        {
            var arch = program.Architecture;
            var imgSym = Loader.CreateImageSymbol(elfSym, arch, true);
            if (imgSym is not null && imgSym.Address!.ToLinear() != 0)
            {
                imageSymbols[imgSym.Address] = imgSym;
            }

            if (extraSym is not null)
            {
                var extraImgSym = Loader.CreateImageSymbol(extraSym, arch, true);
                if (extraImgSym is not null)
                {
                    imageSymbols[extraImgSym.Address!] = extraImgSym;
                }
            }
            if (elfSym.SectionIndex == ElfSection.SHN_UNDEF && imgSym is not null)
            {
                program.ImportReferences[addrImport] =
                    new NamedImportReference(
                        addrImport,
                        null,   // ELF imports don't specify which module to take the import from
                        imgSym.Name!,
                        imgSym.Type);
            }
        }

        [Conditional("DEBUG")]
        protected abstract void DumpDynamicSegment(ElfSegment dynSeg);

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
            var renderer = new DynamicSectionRenderer32(Loader, null!, loader.BinaryImage.Header.Machine);
            var sw = new StringWriter();
            renderer.Render(dynSeg.FileOffset, new TextFormatter(sw));
            Debug.WriteLine(sw.ToString());
        }


        [Conditional("DEBUG")]
        protected void DumpRel32(ElfLoader32 loader)
        {
            foreach (var section in loader.BinaryImage.Sections.Where(s => s.Type == SectionHeaderType.SHT_REL))
            {
                ElfImageLoader.trace.Inform("REL: Offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection?.Name ?? "?",
                    section.RelocatedSection?.Name ?? "?");
                loader.BinaryImage.SymbolsByFileOffset.TryGetValue(section.LinkedSection!.FileOffset, out var symbols);
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rel = Elf32_Rel.Read(rdr);
                    ElfImageLoader.trace.Verbose(
                        "  off:{0:X8} type:{1,-16} {3,3} {2}",
                        rel.r_offset,
                        RelocationTypeToString(rel.r_info & 0xFF) ?? "?",
                        symbols is not null ? symbols[(int)(rel.r_info >> 8)].Name : "<nosym>",
                        (int)(rel.r_info >> 8));
                }
            }
        }

        [Conditional("DEBUG")]
        protected void DumpRela32(ElfLoader32 loader)
        {
            foreach (var section in loader.BinaryImage.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                ElfImageLoader.trace.Inform(
                    "RELA: Offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection!.Name,
                    section.RelocatedSection!.Name);

                var symbols = loader.BinaryImage.SymbolsByFileOffset[section.LinkedSection.FileOffset];
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
            renderer.Render(dynSeg.FileOffset, new TextFormatter(sw));
            Debug.WriteLine(sw.ToString());
        }



        [Conditional("DEBUG")]
        protected void DumpRel64(ElfLoader64 loader)
        {
            foreach (var section in loader.BinaryImage.Sections.Where(s => s.Type == SectionHeaderType.SHT_REL))
            {
                ElfImageLoader.trace.Inform("REL: Offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection?.Name ?? "?",
                    section.RelocatedSection?.Name ?? "?");
                loader.BinaryImage.SymbolsByFileOffset.TryGetValue(section.LinkedSection!.FileOffset, out var symbols);
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rel = Elf64_Rel.Read(rdr);
                    ElfImageLoader.trace.Verbose(
                        "  off:{0:X16} type:{1,-16} {3,3} {2}",
                        rel.r_offset,
                        RelocationTypeToString((uint)rel.r_info & 0xFF) ?? "?",
                        symbols is not null ? symbols[(int) (rel.r_info >> 32)].Name : "<nosym>",
                        (int) (rel.r_info >> 32));
                }
            }
        }

        [Conditional("DEBUG")]
        protected void DumpRela64(ElfLoader64 loader)
        {
            foreach (var section in loader.BinaryImage.Sections.Where(s => 
                s.Type == SectionHeaderType.SHT_RELA &&
                s.LinkedSection is not null && 
                s.LinkedSection.FileOffset != 0))
            {
                Debug.Print("RELA: Offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection?.Name ?? "?",
                    section.RelocatedSection?.Name ?? "?");

                var symbols = loader.BinaryImage.SymbolsByFileOffset[section.LinkedSection!.FileOffset];
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

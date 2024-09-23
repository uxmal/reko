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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.ImageLoaders.Elf
{
    /// <summary>
    /// If we are dealing with an object file, we need to link it.
    /// </summary>
    // https://sourceware.org/gdb/onlinedocs/stabs/Stab-Section-Basics.html#Stab-Section-Basics
    public abstract class ElfObjectLinker
    {
        protected readonly IProcessorArchitecture arch;
        private readonly ElfLoader loader;
        protected byte[] rawImage;

        /// <summary>
        /// Section used to simulate the presence of external functions.
        /// </summary>
        protected readonly ElfSection rekoExtfn;
        protected readonly List<ElfSymbol> unresolvedSymbols;

        public ElfObjectLinker(ElfLoader loader, IProcessorArchitecture arch, byte[] rawImage)
        {
            this.loader = loader;
            this.arch = arch;
            this.rawImage = rawImage ?? throw new ArgumentNullException(nameof(rawImage));
            this.rekoExtfn = new ElfSection
            {
                Name = ".reko.externs",
                Number = (uint) loader.Sections.Count,
                Type = SectionHeaderType.SHT_NOBITS,
                Flags = ElfLoader.SHF_ALLOC | ElfLoader.SHF_EXECINSTR,
                FileOffset = 0,
                Size = 0,
                Alignment = 0x10,
            };
            this.unresolvedSymbols = new List<ElfSymbol>();
            this.PltEntries = new Dictionary<ElfSymbol, Address>();
        }

        public Dictionary<ElfSymbol, Address> PltEntries { get; }

        public abstract Program LinkObject(IPlatform platform, Address? addrLoad, byte[] rawImage);

        protected Address? ComputeBaseAddressFromSections(IEnumerable<ElfSection> sections)
        {
            var address = sections.Where(s => s.Address != null && (s.Flags & ElfLoader.SHF_ALLOC) != 0)
                .OrderBy(s => s.Address)
                .Select(s => s.Address)
                .FirstOrDefault();
            if (address is null)
                return loader.CreateAddress(0x1000);
            if (address.Offset == 0)
                address += 0x1000;
            return address;
        }

        /// <summary>
        /// Collects all unresolved symbols and add them as external procedures.
        /// </summary>
        /// <param name="interceptedCalls">List of intercepted calls, into which we add 
        /// the external procedures.</param>
        public void LoadExternalProcedures(Dictionary<Address, ExternalProcedure> interceptedCalls)
        {
            ElfImageLoader.trace.Verbose("== Adjusted unresolved procedures");
            foreach (var sym in unresolvedSymbols)
            {
                var addr =
                    loader.Sections[(int) sym.SectionIndex].Address! +
                    sym.Value;
                ElfImageLoader.trace.Verbose("  {0}", sym.Value);

                //$TODO: try guessing the signature based on the symbol name.
                var sig = new FunctionType();
                interceptedCalls.Add(addr, new ExternalProcedure(sym.Name, sig));
                PltEntries[sym] = addr;
            }
        }
    }

    public abstract class ElfObjectLinker<TLoader, TSection, TSegment> : ElfObjectLinker
        where TLoader : ElfLoader
    {
        protected readonly TLoader loader;

        protected ElfObjectLinker(TLoader loader, IProcessorArchitecture arch, byte[] image) 
            : base(loader, arch, image)
        {
            this.loader = loader;
        }

        public override Program LinkObject(IPlatform platform, Address? addrLoad, byte[] rawImage)
        {
            var addrBase = addrLoad is not null && addrLoad.Offset != 0
                ? addrLoad
                : ComputeBaseAddressFromSections(loader.Sections)!;
            CollectCommonSymbolsIntoSection();
            CollectUndefinedSymbolsIntoSection();
            var segments = ComputeSegmentSizes();
            var segmentMap = CreateSegments(addrBase, segments);
            var program = new Program(new ByteProgramMemory(segmentMap), platform.Architecture, platform);
            LoadExternalProcedures(program.InterceptedCalls);
            return program;
        }

        public abstract Dictionary<ElfSection, TSegment> ComputeSegmentSizes();

        /// <summary>
        /// Allocate the space required by SHN_COMMON symbols into a 
        /// synthesized section called ".reko.common", and which will
        /// be placed into its own segment later.
        /// </summary>
        public void CollectCommonSymbolsIntoSection()
        {
            var rekoCommon = new ElfSection
            {
                Name = ".reko.common",
                Number = (uint) loader.Sections.Count,
                Type = SectionHeaderType.SHT_NOBITS,
                Flags = ElfLoader.SHF_WRITE | ElfLoader.SHF_ALLOC | ElfLoader.SHF_REKOCOMMON,
                FileOffset = 0,
                Size = 0,
            };
            foreach (var sym in loader.GetAllSymbols().Where(s => s.SectionIndex == ElfSection.SHN_COMMON))
            {
                rekoCommon.Size = Align(rekoCommon.Size, sym.Value);
                sym.Value = (uint) rekoCommon.Size;
                sym.SectionIndex = (uint) loader.Sections.Count;
                rekoCommon.Size += sym.Size;
            }
            if (rekoCommon.Size > 0)
            {
                loader.Sections.Add(rekoCommon);
            }
        }

        public abstract SegmentMap CreateSegments(Address addrBase, Dictionary<ElfSection, TSegment> mpSections);

        /// <summary>
        /// Allocate an arbitrary 16 bytes for each unresolved
        /// external symbol.
        /// </summary>
        public void CollectUndefinedSymbolsIntoSection()
        {
            static bool IsUnresolved(ElfSymbol s)
            {
                return (s.SectionIndex == ElfSection.SHN_UNDEF);
            }

            foreach (var sym in loader.GetAllSymbols().Where(IsUnresolved))
            {
                rekoExtfn.Size = Align(rekoExtfn.Size, 0x10);
                sym.Value = rekoExtfn.Size;
                sym.SectionIndex = (uint) loader.Sections.Count;
                base.unresolvedSymbols.Add(sym);
                rekoExtfn.Size += 0x10;
            }
            if (rekoExtfn.Size > 0)
            {
                loader.Sections.Add(rekoExtfn);
            }
        }

        protected ulong Align(ulong p_pmemsz, ulong value)
        {
            if (value < 2)
                return p_pmemsz;
            return value * ((p_pmemsz + value - 1) / value);
        }

        protected uint Align(uint p_pmemsz, uint value)
        {
            if (value < 2)
                return p_pmemsz;
            return value * ((p_pmemsz + value - 1) / value);
        }

        protected uint SegmentAccess(ulong sectionFlags)
        {
            uint segFlags = ElfLoader.PF_R;
            if ((sectionFlags & ElfLoader.SHF_WRITE) != 0)
                segFlags |= ElfLoader.PF_W;
            if ((sectionFlags & ElfLoader.SHF_EXECINSTR) != 0)
                segFlags |= ElfLoader.PF_X;
            return segFlags;
        }
    }

    public class ElfObjectLinker64 : ElfObjectLinker<ElfLoader64, Elf64_SHdr, Elf64_PHdr>
    {

        public ElfObjectLinker64(ElfLoader64 loader, IProcessorArchitecture arch, byte[] rawImage) 
            : base(loader, arch, rawImage)
        {
            this.Segments = new List<Elf64_PHdr>();
        }

        public List<Elf64_PHdr> Segments { get; private set; }

        /// <summary>
        /// Collects all required segments from the sections and determines
        /// their total size.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<ElfSection, Elf64_PHdr> ComputeSegmentSizes()
        {
            var mpToSegment = new Dictionary<ulong, Elf64_PHdr>();
            var mpSectionToSegment = new Dictionary<ElfSection, Elf64_PHdr>();
            foreach (var section in loader.Sections
                .Where(s => (s.Flags & ElfLoader.SHF_ALLOC) != 0))
            {
                if (!mpToSegment.TryGetValue(section.Flags, out Elf64_PHdr? segment))
                {
                    segment = new Elf64_PHdr();
                    segment.p_flags = SegmentAccess(section.Flags);
                    mpToSegment.Add(section.Flags, segment);
                    Segments.Add(segment);
                }
                segment.p_pmemsz = Align(segment.p_pmemsz, (uint) section.Alignment);

                mpSectionToSegment.Add(section, segment);
                if (section.Type != SectionHeaderType.SHT_NOBITS)
                {
                    segment.p_pmemsz += (uint) section.Size;
                    segment.p_filesz += (uint) section.Size;
                }
                else
                {
                    segment.p_pmemsz += (uint) section.Size;
                    segment.p_filesz += 0;
                }
            }
            return mpSectionToSegment;
        }

        public override SegmentMap CreateSegments(Address addrBase, Dictionary<ElfSection, Elf64_PHdr> mpSections)
        {
            if (addrBase == null) throw new ArgumentNullException(nameof(addrBase));
            var addr = addrBase;
            foreach (var segment in Segments)
            {
                segment.p_paddr = (uint) addr.ToLinear();
                //$REVIEW: 4096 byte alignment should be enough for everyone - Bill Gates III
                addr = (addr + segment.p_pmemsz).Align(0x1000);
            }

            var psegAlloc = Segments.ToDictionary(k => k, v => v.p_paddr);
            var psegMem = Segments.ToDictionary(k => k, v => arch.CreateImageWriter());
            foreach (var section in loader.Sections)
            {
                if (!mpSections.TryGetValue(section, out Elf64_PHdr? segment))
                    continue;
                section.Address = Address.Ptr64(psegAlloc[segment]);
                if (section.Type != SectionHeaderType.SHT_NOBITS)
                {
                    psegMem[segment].WriteBytes(rawImage, (uint) section.FileOffset, (uint) section.Size);
                }
                else
                {
                    psegMem[segment].WriteBytes(0, (uint) section.Size);
                }
                psegAlloc[segment] += (uint) section.Size;
            }

            var mpMemoryAreas = psegMem.ToDictionary(
                k => k.Key,
                v => new ByteMemoryArea(
                    Address.Ptr64(v.Key.p_paddr),
                    v.Value.ToArray()));
            var imageMap = new SegmentMap(
                addrBase,
                mpSections
                    .Select(s => new ImageSegment(
                        s.Key.Name,
                        mpMemoryAreas[s.Value],
                        ElfLoader.AccessModeOf(s.Key.Flags)))
                    .ToArray());
            return imageMap;
        }
    }

    public class ElfObjectLinker32 : ElfObjectLinker<ElfLoader32, Elf32_SHdr, Elf32_PHdr>
    {
        public ElfObjectLinker32(ElfLoader32 loader, IProcessorArchitecture arch, byte[] rawImage)
            : base(loader, arch, rawImage)
        {
            this.Segments = new List<Elf32_PHdr>();
        }

        public List<Elf32_PHdr> Segments { get; private set; }

        /// <summary>
        /// Collects all required segments from the sections and determines
        /// their total size.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<ElfSection, Elf32_PHdr> ComputeSegmentSizes()
        {
            var mpToSegment = new Dictionary<ulong, Elf32_PHdr>();
            var mpSectionToSegment = new Dictionary<ElfSection, Elf32_PHdr>();
            foreach (var section in loader.Sections)
            {
                if (!mpToSegment.TryGetValue(section.Flags, out Elf32_PHdr? segment))
                {
                    segment = new Elf32_PHdr();
                    segment.p_flags = SegmentAccess(section.Flags);
                    mpToSegment.Add(section.Flags, segment);
                    Segments.Add(segment);
                }
                segment.p_pmemsz = Align(segment.p_pmemsz, (uint) section.Alignment);

                mpSectionToSegment.Add(section, segment);
                if (section.Type != SectionHeaderType.SHT_NOBITS)
                {
                    segment.p_pmemsz += (uint) section.Size;
                    segment.p_filesz += (uint) section.Size;
                }
                else
                {
                    segment.p_pmemsz += (uint) section.Size;
                    segment.p_filesz += 0;
                }
            }
            return mpSectionToSegment;
        }

        public override SegmentMap CreateSegments(Address addrBase, Dictionary<ElfSection, Elf32_PHdr> mpSections)
        {
            if (addrBase == null) throw new ArgumentNullException(nameof(addrBase));
            var addr = addrBase;
            foreach (var segment in Segments)
            {
                segment.p_paddr = (uint) addr.ToLinear();
                //$REVIEW: 4096 byte alignment should be enough for everyone - Bill Gates III
                addr = (addr + segment.p_pmemsz).Align(0x1000);
            }

            var psegAlloc = Segments.ToDictionary(k => k, v => v.p_paddr);
            var psegMem = Segments.ToDictionary(k => k, v => arch.CreateImageWriter());
            foreach (var section in loader.Sections)
            {
                if (!mpSections.TryGetValue(section, out Elf32_PHdr? segment))
                    continue;
                section.Address = Address.Ptr32(psegAlloc[segment]);
                if (section.Type != SectionHeaderType.SHT_NOBITS)
                {
                    psegMem[segment].WriteBytes(rawImage, (uint) section.FileOffset, (uint) section.Size);
                }
                else
                {
                    psegMem[segment].WriteBytes(0, (uint) section.Size);
                }
                psegAlloc[segment] += (uint) section.Size;
            }

            var mpMemoryAreas = psegMem.ToDictionary(
                k => k.Key,
                v => new ByteMemoryArea(
                    Address.Ptr32(v.Key.p_paddr),
                    v.Value.ToArray()));
            var imageMap = new SegmentMap(
                addrBase,
                mpSections
                    .Select(s => new ImageSegment(
                        s.Key.Name,
                        mpMemoryAreas[s.Value],
                        ElfLoader.AccessModeOf(s.Key.Flags)))
                    .ToArray());
            return imageMap;
        }
    }
}

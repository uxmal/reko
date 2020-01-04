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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    /// <summary>
    /// If we are dealing with an object file, we need to link it.
    /// </summary>
    // https://sourceware.org/gdb/onlinedocs/stabs/Stab-Section-Basics.html#Stab-Section-Basics
    public abstract class ElfObjectLinker
    {
        protected IProcessorArchitecture arch;
        private ElfLoader loader;
        protected byte[] rawImage;
        protected ElfSection rekoExtfn;

        public ElfObjectLinker(ElfLoader loader, IProcessorArchitecture arch, byte[] rawImage)
        {
            if (rawImage == null)
                throw new ArgumentNullException("rawImage");
            this.loader = loader;
            this.arch = arch;
            this.rawImage = rawImage;
        }

        public abstract Program LinkObject(IPlatform platform, Address addrLoad, byte[] rawImage);
    }

    public class ElfObjectLinker64 : ElfObjectLinker
    {
        public ElfObjectLinker64(ElfLoader64 loader, IProcessorArchitecture arch, byte[] rawImage) 
            : base(loader, arch, rawImage)
        { }

        public override Program LinkObject(IPlatform platform, Address addrLoad, byte[] rawImage)
        {
            throw new NotImplementedException();
        }
    }

    public class ElfObjectLinker32 : ElfObjectLinker
    {
        private ElfLoader32 loader;

        public ElfObjectLinker32(ElfLoader32 loader, IProcessorArchitecture arch, byte[] rawImage)
            : base(loader, arch, rawImage)
        {
            this.loader = loader;
            this.Segments = new List<Elf32_PHdr>();
        }

        public List<Elf32_PHdr> Segments { get; private set; }

        public override Program LinkObject(IPlatform platform, Address addrLoad, byte[] rawImage)
        {
            var segments = ComputeSegmentSizes();
            var imageMap = CreateSegments(addrLoad, segments);
            var program = new Program(imageMap, platform.Architecture, platform);
            LoadExternalProcedures(program.InterceptedCalls);
            return program;
        }

        /// <summary>
        /// Collects all required segments from the sections and determines
        /// their total size.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ElfSection, Elf32_PHdr> ComputeSegmentSizes()
        {
            CollectCommonSymbolsIntoSection();
            CollectUndefinedSymbolsIntoSection();

            var mpToSegment = new Dictionary<ulong, Elf32_PHdr>();
            var mpSectionToSegment = new Dictionary<ElfSection, Elf32_PHdr>();
            foreach (var section in loader.Sections
                .Where(s => (s.Flags & ElfLoader.SHF_ALLOC) != 0))
            {
                Elf32_PHdr segment;
                if (!mpToSegment.TryGetValue(section.Flags, out segment))
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

        /// <summary>
        /// Allocate the space required by SHN_COMMON symbols into a 
        /// synthesized section called ".reko.common", and which will
        /// be placed into its own segment later.
        /// </summary>
        private void CollectCommonSymbolsIntoSection()
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

        /// <summary>
        /// Allocate an arbitrary 16 bytes for each unresolved
        /// external symbol.
        /// </summary>
        private void CollectUndefinedSymbolsIntoSection()
        {
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
            foreach (var sym in loader.GetAllSymbols().Where(s =>
                s.Type == ElfSymbolType.STT_NOTYPE &&
                !string.IsNullOrEmpty(s.Name)))
            {
                rekoExtfn.Size = Align(rekoExtfn.Size, 0x10);
                sym.Value = (uint) rekoExtfn.Size;
                sym.SectionIndex = (uint) loader.Sections.Count;
                rekoExtfn.Size += 0x10;
            }
            if (rekoExtfn.Size > 0)
            {
                loader.Sections.Add(rekoExtfn);
            }
        }

        private ulong Align(ulong p_pmemsz, ulong value)
        {
            if (value < 2)
                return p_pmemsz;
            return value * ((p_pmemsz + value - 1) / value);
        }

        private uint Align(uint p_pmemsz, uint value)
        {
            if (value < 2)
                return p_pmemsz;
            return value * ((p_pmemsz + value - 1) / value);
        }

        private uint SegmentAccess(ulong sectionFlags)
        {
            uint segFlags = ElfLoader.PF_R;
            if ((sectionFlags & ElfLoader.SHF_WRITE) != 0)
                segFlags |= ElfLoader.PF_W;
            if ((sectionFlags & ElfLoader.SHF_EXECINSTR) != 0)
                segFlags |= ElfLoader.PF_X;
            return segFlags;
        }

        public SegmentMap CreateSegments(Address addrBase, Dictionary<ElfSection, Elf32_PHdr> mpSections)
        {
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
                Elf32_PHdr segment;
                if (!mpSections.TryGetValue(section, out segment))
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
                v => new MemoryArea(
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

        public void LoadExternalProcedures(Dictionary<Address, ExternalProcedure> interceptedCalls)
        {
            // Find all unresolved symbols and add them as external procedures.
            foreach (var sym in loader.GetAllSymbols().Where(IsExternalSymbol))
            {
                var addr =
                    loader.Sections[(int) sym.SectionIndex].Address +
                    sym.Value;
                //$TODO: try guessing the signature based on the symbol name.
                var sig = new FunctionType();
                interceptedCalls.Add(addr, new ExternalProcedure(sym.Name, sig));
            }
        }

        private bool IsExternalSymbol(ElfSymbol s)
        {
            return s.Type == ElfSymbolType.STT_NOTYPE &&
                !string.IsNullOrEmpty(s.Name) &&
                s.SectionIndex == this.rekoExtfn.Number;
        }
    }
}

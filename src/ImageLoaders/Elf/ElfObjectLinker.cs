#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
            return program;
        }

        /// <summary>
        /// Collects all required segments from the sections and determines
        /// their total size.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Elf32_SHdr, Elf32_PHdr> ComputeSegmentSizes()
        {
            CollectCommonSymbolsIntoSection();
            CollectUndefinedSymbolsIntoSection();

            var mpToSegment = new Dictionary<uint, Elf32_PHdr>();
            var mpSectionToSegment = new Dictionary<Elf32_SHdr, Elf32_PHdr>();
            foreach (var section in loader.SectionHeaders
                .Where(s => (s.sh_flags & ElfLoader.SHF_ALLOC) != 0))
            {
                Elf32_PHdr segment;
                if (!mpToSegment.TryGetValue(section.sh_flags, out segment))
                {
                    segment = new Elf32_PHdr();
                    segment.p_flags = SegmentAccess(section.sh_flags);
                    mpToSegment.Add(section.sh_flags, segment);
                    Segments.Add(segment);
                }
                mpSectionToSegment.Add(section, segment);
                if (section.sh_type != SectionHeaderType.SHT_NOBITS)
                {
                    segment.p_pmemsz += section.sh_size;
                    segment.p_filesz += section.sh_size;
                }
                else
                {
                    segment.p_pmemsz += section.sh_size;
                    segment.p_filesz += 0;
                }
            }
            return mpSectionToSegment;
        }

        /// <summary>
        /// Allocate the space required by SHN_COMMON symbols into a 
        /// synthesized section called ".rekocommon", and which will
        /// be placed into its own segment later.
        /// </summary>
        private void CollectCommonSymbolsIntoSection()
        {
            var rekoCommon = new Elf32_SHdr
            {
                sh_type = SectionHeaderType.SHT_NOBITS,
                sh_flags = ElfLoader.SHF_WRITE | ElfLoader.SHF_ALLOC | ElfLoader.SHF_REKOCOMMON,
                sh_offset = 0,
                sh_size = 0,
            };
            foreach (var sym in loader.GetAllSymbols().Where(s => s.SectionIndex == 0xFFF2))
            {
                rekoCommon.sh_size = Align(rekoCommon.sh_size, sym.Value);
                sym.Value = rekoCommon.sh_size;
                sym.SectionIndex = (uint) loader.SectionHeaders.Count;
                rekoCommon.sh_size += sym.Size;
            }
            if (rekoCommon.sh_size > 0)
            {
                loader.SectionHeaders.Add(rekoCommon);
            }
        }

        /// <summary>
        /// Allocate an arbitrary 16 bytes for each unresolved
        /// external symbol.
        /// </summary>
        private void CollectUndefinedSymbolsIntoSection()
        {
            var rekoExtfn = new Elf32_SHdr
            {
                sh_type = SectionHeaderType.SHT_NOBITS,
                sh_flags = ElfLoader.SHF_ALLOC | ElfLoader.SHF_EXECINSTR,
                sh_offset = 0,
                sh_size = 0,
                sh_addralign = 0x10,
            };
            foreach (var sym in loader.GetAllSymbols().Where(s => s.Type == SymbolType.STT_NOTYPE))
            {
                rekoExtfn.sh_size = Align(rekoExtfn.sh_size, 0x10);
                sym.Value = rekoExtfn.sh_size;
                sym.SectionIndex = (uint)loader.SectionHeaders.Count;
                rekoExtfn.sh_size += 0x10;
            }
            if (rekoExtfn.sh_size > 0)
            {
                loader.SectionHeaders.Add(rekoExtfn);
            }
        }

        private uint Align(uint p_pmemsz, uint value)
        {
            return value * ((p_pmemsz + value - 1) / value);
        }

        private uint SegmentAccess(uint sectionFlags)
        {
            uint segFlags = ElfLoader.PF_R;
            if ((sectionFlags & ElfLoader.SHF_WRITE) != 0)
                segFlags |= ElfLoader.PF_W;
            if ((sectionFlags & ElfLoader.SHF_EXECINSTR) != 0)
                segFlags |= ElfLoader.PF_X;
            return segFlags;
        }

        public ImageMap CreateSegments(Address addrBase, Dictionary<Elf32_SHdr,Elf32_PHdr> mpSections)
        {
            var addr = addrBase;
            foreach (var segment in Segments)
            {
                segment.p_paddr = (uint)addr.ToLinear();
                //$REVIEW: 4096 byte alignment should be enough for everyone - Bill Gates III
                addr = (addr + segment.p_pmemsz).Align(0x1000);
            }

            var psegAlloc = Segments.ToDictionary(k => k, v => v.p_paddr);
            var psegMem = Segments.ToDictionary(k => k, v => arch.CreateImageWriter());
            foreach (var section in loader.SectionHeaders)
            {
                Elf32_PHdr segment;
                if (!mpSections.TryGetValue(section, out segment))
                    continue;
                section.sh_addr = psegAlloc[segment];
                if (section.sh_type != SectionHeaderType.SHT_NOBITS)
                {
                    psegMem[segment].WriteBytes(rawImage, section.sh_offset, section.sh_size);
                }
                else
                {
                    psegMem[segment].WriteBytes(0, section.sh_size);
                }
                psegAlloc[segment] += section.sh_size;
            }

            var mpMemoryAreas = psegMem.ToDictionary(
                k => k.Key,
                v => new MemoryArea(
                    Address.Ptr32(v.Key.p_paddr),
                    v.Value.ToArray()));
            var imageMap = new ImageMap(
                addrBase,
                mpSections
                    .Select(s => new ImageSegment(
                        loader.GetSectionName(s.Key.sh_name),
                        mpMemoryAreas[s.Value],
                        ElfLoader.AccessModeOf(s.Key.sh_flags)))
                    .ToArray());
            return imageMap;
        }
    }
}

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
        private List<ElfSymbol> symbols;

        public ElfObjectLinker32(ElfLoader32 loader, IProcessorArchitecture arch, byte[] rawImage)
            : base(loader, arch, rawImage)
        {
            this.loader = loader;
            this.Segments = new List<Elf32_PHdr>();
            this.symbols = LoadSymbols();
        }

        public List<Elf32_PHdr> Segments { get; private set; }

        public override Program LinkObject(IPlatform platform, Address addrLoad, byte[] rawImage)
        {
            var segments = ComputeSegmentSizes();
            var imageMap = CreateSegments(addrLoad, segments);
            return new Program(imageMap, platform.Architecture, platform);
        }

        /// <summary>
        /// Collects all required segments from the sections and determines
        /// their total size.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Elf32_SHdr, Elf32_PHdr> ComputeSegmentSizes()
        {
            var mpToSegment = new Dictionary<uint, Elf32_PHdr>();
            var mpSectionToSegment = new Dictionary<Elf32_SHdr, Elf32_PHdr>();
            foreach (var section in loader.SectionHeaders
                .Where(s =>( s.sh_flags & ElfLoader.SHF_ALLOC) != 0))
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
                segment.p_pmemsz += section.sh_size;
                segment.p_filesz += section.sh_type != SectionHeaderType.SHT_NOBITS
                    ? section.sh_size
                    : 0;
            }

            // Collect SHN_COMMON symbols
            var rwaSegment = Segments.Single(s => s.p_flags == (ElfLoader.PF_R | ElfLoader.PF_W));
            foreach (var sym in symbols.Where(s => s.SegmentIndex == 0xFFF2))
            {
                rwaSegment.p_pmemsz = Align(rwaSegment.p_pmemsz, sym.Value);
                sym.Value = rwaSegment.p_pmemsz;
                rwaSegment.p_pmemsz += sym.Size;
            }
            return mpSectionToSegment;  
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
                psegMem[segment].WriteBytes(rawImage, section.sh_offset, section.sh_size);
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

        public List<ElfSymbol> LoadSymbols()
        {
            Elf32_SHdr section = loader.SectionHeaders.Single(s => s.sh_type == SectionHeaderType.SHT_SYMTAB);
            var strTable = loader.SectionHeaders[(int)section.sh_link];
            uint nSymbols = section.sh_size / section.sh_entsize;
            var rdr = loader.CreateReader(section.sh_offset);
            var list = new List<ElfSymbol>();
            for (int iSymbol = 0; iSymbol < nSymbols; ++iSymbol)
            {
                var iName = rdr.ReadUInt32();
                var rdrName = loader.CreateReader(strTable.sh_offset + iName);
                var name = rdrName.ReadCString(PrimitiveType.Char, Encoding.UTF8);

                var value = rdr.ReadUInt32();
                var size = rdr.ReadUInt32();
                var info = rdr.ReadByte();
                rdr.ReadByte();         // skip unused st_other
                var iSegment = rdr.ReadUInt16();

                list.Add(new ElfSymbol
                {
                    Name = name.ToString(),
                    Value = value,
                    Size = size,
                    SegmentIndex = iSegment,
                    Info = info,
                });
            }
            return list;
        }
    }
}

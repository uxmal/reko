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
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.ImageLoaders.Elf.Relocators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Elf
{
    public class ElfLoader64 : ElfLoader
    {
        private readonly byte osAbi;

        public ElfLoader64(ElfImageLoader imgLoader, Elf64_EHdr elfHeader, byte[] rawImage, byte osAbi, byte endianness)
            : base(imgLoader, elfHeader.e_machine, endianness)
        {
            this.Header64 = elfHeader;
            this.osAbi = osAbi;
            base.rawImage = rawImage;
        }

        public Elf64_EHdr Header64 { get; set; }

        public override Address DefaultAddress { get { return Address.Ptr64(0x8048000); } }

        public override bool IsExecutableFile { get { return Header64.e_type != ElfImageLoader.ET_REL; } }

        public override ulong AddressToFileOffset(ulong addr)
        {
            foreach (var ph in Segments)
            {
                if (ph.p_vaddr <= addr && addr < ph.p_vaddr + ph.p_filesz)
                    return (addr - ph.p_vaddr) + ph.p_offset;
            }
            return ~0ul;
        }

        public override Address ComputeBaseAddress(IPlatform platform)
        {
            ulong uBaseAddr = Segments
                .Where(ph => ph.p_vaddr > 0 && ph.p_filesz > 0)
                .Min(ph => ph.p_vaddr);
            return platform.MakeAddressFromLinear(uBaseAddr, true);
        }

        public override Address CreateAddress(ulong uAddr)
        {
            return Address.Ptr64(uAddr);
        }

        protected override IProcessorArchitecture CreateArchitecture(byte endianness)
        {
            string arch;
            switch (machine)
            {
            case ElfMachine.EM_MIPS:
                //$TODO: detect release 6 of the MIPS architecture. 
                // would be great to get our sweaty little hands on
                // such a binary.
                if (endianness == ELFDATA2LSB)
                {
                    arch = "mips-le-64";
                }
                else if (endianness == ELFDATA2MSB)
                {
                    arch = "mips-be-64";
                }
                else
                {
                    throw new NotSupportedException(string.Format("The MIPS architecture does not support ELF endianness value {0}", endianness));
                }
                break;
            default:
                return base.CreateArchitecture(endianness);
            }
            var cfgSvc = Services.RequireService<IConfigurationService>();
            return cfgSvc.GetArchitecture(arch);
        }

        public override ElfObjectLinker CreateLinker()
        {
            return new ElfObjectLinker64(this, Architecture, rawImage);
        }

        private ImageSegmentRenderer CreateRenderer64(ElfSection shdr)
        {
            switch (shdr.Type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer64(this, shdr);
            case SectionHeaderType.SHT_RELA:
                return new RelaSegmentRenderer64(this, shdr);
            case SectionHeaderType.SHT_SYMTAB:
            case SectionHeaderType.SHT_DYNSYM:
                return new SymtabSegmentRenderer64(this, shdr);
            default: return null;
            }
        }

        public override ElfRelocator CreateRelocator(ElfMachine machine, SortedList<Address, ImageSymbol> symbols)
        {
            switch (machine)
            {
            case ElfMachine.EM_AARCH64: return new Arm64Relocator(this, symbols);
            case ElfMachine.EM_X86_64: return new x86_64Relocator(this, symbols);
            case ElfMachine.EM_PPC64: return new PpcRelocator64(this, symbols);
            case ElfMachine.EM_MIPS: return new MipsRelocator64(this, symbols);
            case ElfMachine.EM_RISCV: return new RiscVRelocator64(this, symbols);
            case ElfMachine.EM_ALPHA: return new AlphaRelocator(this, symbols);
            case ElfMachine.EM_S390: return new zSeriesRelocator(this, symbols);
            }
            return base.CreateRelocator(machine, symbols);
        }

        public override void Dump(TextWriter writer)
        {
            writer.WriteLine("Entry: {0:X}", Header64.e_entry);
            writer.WriteLine("Sections:");
            foreach (var sh in Sections)
            {
                writer.WriteLine("{0,-18} sh_type: {1,-12} sh_flags: {2,-4} sh_addr; {3:X8} sh_offset: {4:X8} sh_size: {5:X8} sh_link: {6,-18} sh_info: {7,-18} sh_addralign: {8:X8} sh_entsize: {9:X8}",
                    sh.Name,
                    sh.Type,
                    DumpShFlags(sh.Flags),
                    sh.Address,
                    sh.FileOffset,
                    sh.Size,
                    sh.LinkedSection != null ? sh.LinkedSection.Name : "",
                    sh.RelocatedSection != null ? sh.RelocatedSection.Name : "",
                    sh.Alignment,
                    sh.EntrySize);
            }
            writer.WriteLine();
            writer.WriteLine("Program headers:");
            foreach (var ph in Segments)
            {
                writer.WriteLine("p_type:{0,-12} p_offset: {1:X8} p_vaddr:{2:X8} p_paddr:{3:X8} p_filesz:{4:X8} p_pmemsz:{5:X8} p_flags:{6} {7:X8} p_align:{8:X8}",
                    ph.p_type,
                    ph.p_offset,
                    ph.p_vaddr,
                    ph.p_paddr,
                    ph.p_filesz,
                    ph.p_pmemsz,
                    rwx(ph.p_flags & 7),
                    ph.p_flags,
                    ph.p_align);
            }
            writer.WriteLine("Base address: {0:X8}", ComputeBaseAddress(platform));
            writer.WriteLine();
            writer.WriteLine("Dependencies");
            foreach (var dep in GetDependencyList(imgLoader.RawImage))
            {
                writer.WriteLine("  {0}", dep);
            }

            writer.WriteLine();
            writer.WriteLine("Relocations");
            foreach (var sh in Sections.Where(sh => sh.Type == SectionHeaderType.SHT_RELA))
            {
                // DumpRela(sh);
            }
        }

        public override List<string> GetDependencyList(byte[] rawImage)
        {
            return Dependencies;
        }

        public override IEnumerable<ElfDynamicEntry> GetDynamicEntries(ulong offsetDynamic)
        {
            var rdr = imgLoader.CreateReader(offsetDynamic);
            for (; ; )
            {
                var dyn = new Elf64_Dyn();
                if (!rdr.TryReadInt64(out dyn.d_tag))
                    break;
                if (dyn.d_tag == ElfDynamicEntry.DT_NULL)
                    break;
                if (!rdr.TryReadInt64(out long val))
                    break;
                dyn.d_val = val;
                yield return new ElfDynamicEntry(dyn.d_tag, dyn.d_ptr); ;
            }
        }


        public override Address GetEntryPointAddress(Address addrBase)
        {
            Address addr = null;
            //$REVIEW: should really have a subclassed "Ps3ElfLoader"
            if (osAbi == ElfLoader.ELFOSABI_CELL_LV2)
            {
                // The Header64.e_entry field actually points to a 
                // "function descriptor" consisiting of two 32-bit 
                // pointers.
                var rdr = imgLoader.CreateReader(Header64.e_entry - addrBase.ToLinear());
                if (rdr.TryReadUInt32(out uint uAddr))
                    addr = Address.Ptr32(uAddr);
            }
            else
            {
                addr = Address.Ptr64(Header64.e_entry);
            }
            return addr;
        }

        internal ElfSection GetSectionInfoByAddr64(ulong r_offset)
        {
            return
                (from sh in this.Sections
                 let addr = sh.Address != null ? sh.Address.ToLinear() : 0
                 where
                    r_offset != 0 &&
                    addr <= r_offset && r_offset < addr + sh.Size
                 select sh)
                .FirstOrDefault();
        }

        protected override int GetSectionNameOffset(uint idxString)
        {
            return (int) (Sections[Header64.e_shstrndx].FileOffset + idxString);
        }

        public string GetSymbol64(int iSymbolSection, ulong symbolNo)
        {
            var symSection = Sections[iSymbolSection];
            return GetSymbol64(iSymbolSection, symbolNo);
        }

        public string GetSymbol64(ElfSection symSection, ulong symbolNo)
        {
            var strSection = symSection.LinkedSection;
            ulong offset = symSection.FileOffset + symbolNo * symSection.EntrySize;
            var rdr = imgLoader.CreateReader(offset);
            rdr.TryReadUInt64(out offset);
            return GetStrPtr(symSection.LinkedSection, (uint) offset);
        }

        public override SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred)
        {
            var segMap = AllocateMemoryAreas(
                Segments
                    .Where(p => IsLoadable(p.p_pmemsz, p.p_type))
                    .Select(p => Tuple.Create(
                        platform.MakeAddressFromLinear(p.p_vaddr, false),
                        (uint) p.p_pmemsz)));
            foreach (var ph in Segments)
            {
                DebugEx.Inform(ElfImageLoader.trace, "ph: addr {0:X8} filesize {0:X8} memsize {0:X8}", ph.p_vaddr, ph.p_filesz, ph.p_pmemsz);
                if (!IsLoadable(ph.p_pmemsz, ph.p_type))
                    continue;
                var vaddr = platform.MakeAddressFromLinear(ph.p_vaddr, false);
                segMap.TryGetLowerBound(vaddr, out var mem);
                if (ph.p_filesz > 0)
                    Array.Copy(
                        rawImage,
                        (long) ph.p_offset, mem.Bytes,
                        vaddr - mem.BaseAddress, (long) ph.p_filesz);
            }
            var segmentMap = new SegmentMap(addrPreferred);
            if (Sections.Count > 0)
            {
                foreach (var section in Sections)
                {
                    if (section.Name == null || section.Address == null)
                        continue;
                    if (segMap.TryGetLowerBound(section.Address, out var mem) &&
                        section.Address < mem.EndAddress)
                    {
                        AccessMode mode = AccessModeOf(section.Flags);
                        var seg = segmentMap.AddSegment(new ImageSegment(
                            section.Name,
                            section.Address,
                            mem, mode)
                        {
                            Size = (uint) section.Size
                        });
                        seg.Designer = CreateRenderer64(section);
                    }
                    else
                    {
                        //$TODO: warn
                    }
                }
            }
            else
            {
                // There are stripped ELF binaries with 0 sections. If we have one
                // create a pseudo-section from the segMap.
                foreach (var segment in segMap)
                {
                    var imgSegment = new ImageSegment(
                        segment.Value.BaseAddress.GenerateName("seg", ""),
                        segment.Value,
                        AccessMode.ReadExecute)        //$TODO: writeable segments.
                    {
                        Size = (uint) segment.Value.Length,
                    };
                    segmentMap.AddSegment(imgSegment);
                }
            }
            segmentMap.DumpSections();
            return segmentMap;
        }

        public override int LoadSegments()
        {
            var rdr = imgLoader.CreateReader(Header64.e_phoff);
            for (int i = 0; i < Header64.e_phnum; ++i)
            {
                var sSeg = Elf64_PHdr.Load(rdr);
                Segments.Add(new ElfSegment
                {
                    p_type = sSeg.p_type,
                    p_offset = sSeg.p_offset,
                    p_vaddr = sSeg.p_vaddr,
                    p_paddr = sSeg.p_paddr,
                    p_filesz = sSeg.p_filesz,
                    p_pmemsz = sSeg.p_pmemsz,
                    p_flags = sSeg.p_flags,
                    p_align = sSeg.p_align,
                });
            }
            return Segments.Count;
        }

        public override ElfRelocation LoadRelEntry(EndianImageReader rdr)
        {
            throw new NotImplementedException();
        }

        public override ElfRelocation LoadRelaEntry(EndianImageReader rdr)
        {
            var rela = Elf64_Rela.Read(rdr);
            return new ElfRelocation
            {
                Offset = rela.r_offset,
                Info = rela.r_info,
                Addend = rela.r_addend,
                SymbolIndex = (int) (rela.r_info >> 32),
            };
        }


        public override void LoadSectionHeaders()
        {
            // Create the sections.
            var inames = new List<uint>();
            var links = new List<uint>();
            var infos = new List<uint>();
            var rdr = imgLoader.CreateReader(Header64.e_shoff);
            for (uint i = 0; i < Header64.e_shnum; ++i)
            {
                var shdr = Elf64_SHdr.Load(rdr);
                var section = new ElfSection
                {
                    Number = i,
                    Type = shdr.sh_type,
                    Flags = shdr.sh_flags,
                    Address = shdr.sh_addr != 0
                        ? platform.MakeAddressFromLinear(shdr.sh_addr, false)
                        : null,
                    FileOffset = shdr.sh_offset,
                    Size = shdr.sh_size,
                    Alignment = shdr.sh_addralign,
                    EntrySize = shdr.sh_entsize,
                };
                Sections.Add(section);
                inames.Add(shdr.sh_name);
                links.Add(shdr.sh_link);
                infos.Add(shdr.sh_info);
            }

            // Get section names and crosslink sections.

            for (int i = 0; i < Sections.Count; ++i)
            {
                var section = Sections[i];
                section.Name = ReadSectionName(inames[i]);

                ElfSection linkSection = null;
                ElfSection relSection = null;
                switch (section.Type)
                {
                case SectionHeaderType.SHT_REL:
                case SectionHeaderType.SHT_RELA:
                    linkSection = GetSectionByIndex(links[i]);
                    relSection = GetSectionByIndex(infos[i]);
                    break;
                case SectionHeaderType.SHT_DYNAMIC:
                case SectionHeaderType.SHT_HASH:
                case SectionHeaderType.SHT_SYMTAB:
                case SectionHeaderType.SHT_DYNSYM:
                    linkSection = GetSectionByIndex(links[i]);
                    break;
                }
                section.LinkedSection = linkSection;
                section.RelocatedSection = relSection;
            }
        }

        public override ElfSymbol LoadSymbol(ulong offsetSymtab, ulong symbolIndex, ulong entrySize, ulong offsetStringTable)
        {
            var rdr = CreateReader(offsetSymtab + entrySize * symbolIndex);
            var sym = Elf64_Sym.Load(rdr);
            return new ElfSymbol
            {
                Name = RemoveModuleSuffix(ReadAsciiString(offsetStringTable + sym.st_name)),
                Type = (ElfSymbolType) (sym.st_info & 0xF),
                Bind = sym.st_info >> 4,
                SectionIndex = sym.st_shndx,
                Value = sym.st_value,
                Size = sym.st_size,
            };
        }

        public override Dictionary<int, ElfSymbol> LoadSymbolsSection(ElfSection symSection)
        {
            //Debug.Print("Symbols");
            var stringtableSection = symSection.LinkedSection;
            var rdr = CreateReader(symSection.FileOffset);
            var symbols = new Dictionary<int, ElfSymbol>();
            for (ulong i = 0; i < symSection.Size / symSection.EntrySize; ++i)
            {
                var sym = Elf64_Sym.Load(rdr);
                //Debug.Print("  {0,3} {1,-25} {2,-12} {3,6} {4,-15} {5:X8} {6,9}",
                //    i,
                //    RemoveGlibcSuffix(ReadAsciiString(stringtableSection.FileOffset + sym.st_name)),
                //    (ElfSymbolType)(sym.st_info & 0xF),
                //    sym.st_shndx,
                //    GetSectionName(sym.st_shndx),
                //    sym.st_value,
                //    sym.st_size);
                symbols.Add(
                    (int) i,
                    new ElfSymbol
                    {
                        Name = RemoveModuleSuffix(ReadAsciiString(stringtableSection.FileOffset + sym.st_name)),
                        Type = (ElfSymbolType) (sym.st_info & 0xF),
                        SectionIndex = sym.st_shndx,
                        Value = sym.st_value,
                        Size = sym.st_size,
                    });
            }
            return symbols;
        }

        public override Address ReadAddress(EndianImageReader rdr)
        {
            if (!rdr.TryReadUInt64(out ulong uAddrSym))
                return null;

            var addr = Address.Ptr64(uAddrSym);
            return addr;
        }
    }
}
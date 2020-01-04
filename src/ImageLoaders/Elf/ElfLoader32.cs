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

    public class ElfLoader32 : ElfLoader
    {
        public ElfLoader32(ElfImageLoader imgLoader, Elf32_EHdr header32, byte[] rawImage, byte endianness)
            : base(imgLoader, header32.e_machine, endianness)
        {
            this.Header = header32 ?? throw new ArgumentNullException(nameof(header32));
            this.rawImage = rawImage;
        }

        public ElfLoader32() : base() { }

        public Elf32_EHdr Header { get; private set; }
        public override Address DefaultAddress { get { return Address.Ptr32(0x8048000); } }

        public override bool IsExecutableFile { get { return Header.e_type != ElfImageLoader.ET_REL; } }

        public static int ELF32_R_SYM(int info) { return ((info) >> 8); }
        public static int ELF32_ST_BIND(int i) { return ((i) >> 4); }
        public static int ELF32_ST_TYPE(int i) { return ((i) & 0x0F); }
        public static byte ELF32_ST_INFO(int b, ElfSymbolType t) { return (byte) (((b) << 4) + ((byte) t & 0xF)); }

        public override ulong AddressToFileOffset(ulong addr)
        {
            foreach (var ph in Segments)
            {
                if (ph.p_vaddr <= addr && addr < ph.p_vaddr + ph.p_filesz)
                    return addr - ph.p_vaddr;
            }
            return ~0ul;
        }

        public override Address ComputeBaseAddress(IPlatform platform)
        {
            if (Segments.Count == 0)
                return Address.Ptr32(0);

            return Address.Ptr32(
                Segments
                .Where(ph => ph.p_filesz > 0)
                .Min(ph => (uint) ph.p_vaddr));
        }

        public override Address CreateAddress(ulong uAddr)
        {
            return Address.Ptr32((uint) uAddr);
        }

        protected override IProcessorArchitecture CreateArchitecture(byte endianness)
        {
            string arch;
            var options = new Dictionary<string, object>();
            string stackRegName = null;
            switch (machine)
            {
            case ElfMachine.EM_MIPS:
                //$TODO: detect release 6 of the MIPS architecture. 
                // would be great to get our sweaty little hands on
                // such a binary.
                var mipsFlags = (MIPSflags) Header.e_flags;
                var is64 = (mipsFlags & MIPSflags.EF_MIPS_ARCH) == MIPSflags.EF_MIPS_ARCH_64;
                if (endianness == ELFDATA2LSB)
                {
                    arch = is64 
                        ? "mips-le-64"
                        : "mips-le-32";
                }
                else if (endianness == ELFDATA2MSB)
                {
                    arch = is64
                        ? "mips-be-64"
                        : "mips-be-32";
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
            var a = cfgSvc.GetArchitecture(arch);
            a.LoadUserOptions(options);
            if (stackRegName != null)
            {
                a.StackRegister = a.GetRegister(stackRegName);
            }
            return a;

        }

        public override ElfObjectLinker CreateLinker()
        {
            return new ElfObjectLinker32(this, Architecture, rawImage);
        }

        public override ElfRelocator CreateRelocator(ElfMachine machine, SortedList<Address, ImageSymbol> imageSymbols)
        {
            switch (machine)
            {
            case ElfMachine.EM_386: return new x86Relocator(this, imageSymbols);
            case ElfMachine.EM_ARM: return new ArmRelocator(this, imageSymbols);
            case ElfMachine.EM_MIPS: return new MipsRelocator(this, imageSymbols);
            case ElfMachine.EM_NANOMIPS: return new NanoMipsRelocator(this, imageSymbols);
            case ElfMachine.EM_MSP430: return new Msp430Relocator(this, imageSymbols);
            case ElfMachine.EM_PPC: return new PpcRelocator(this, imageSymbols);
            case ElfMachine.EM_SPARC32PLUS:
            case ElfMachine.EM_SPARC: return new SparcRelocator(this, imageSymbols);
            case ElfMachine.EM_XTENSA: return new XtensaRelocator(this, imageSymbols);
            case ElfMachine.EM_68K: return new M68kRelocator(this, imageSymbols);
            case ElfMachine.EM_AVR: return new AvrRelocator(this, imageSymbols);
            case ElfMachine.EM_SH: return new SuperHRelocator(this, imageSymbols);
            case ElfMachine.EM_BLACKFIN: return new BlackfinRelocator(this, imageSymbols);
            }
            return base.CreateRelocator(machine, imageSymbols);
        }

        public ImageSegmentRenderer CreateRenderer(ElfSection shdr, ElfMachine machine)
        {
            switch (shdr.Type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer32(this, shdr, machine);
            case SectionHeaderType.SHT_REL:
                return new RelSegmentRenderer(this, shdr);
            case SectionHeaderType.SHT_RELA:
                return new RelaSegmentRenderer(this, shdr);
            case SectionHeaderType.SHT_SYMTAB:
            case SectionHeaderType.SHT_DYNSYM:
                return new SymtabSegmentRenderer32(this, shdr);
            default: return null;
            }
        }

        public override void Dump(TextWriter writer)
        {
            writer.WriteLine("Entry: {0:X}", Header.e_entry);
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
            foreach (var dep in GetDependencyList(rawImage))
            {
                writer.WriteLine("  {0}", dep);
            }

            writer.WriteLine();
            writer.WriteLine("Relocations");
            foreach (var sh in Sections.Where(sh => sh.Type == SectionHeaderType.SHT_RELA))
            {
                DumpRela(sh);
            }
        }

        private void DumpRela(ElfSection sh)
        {
            var entries = sh.Size / sh.EntrySize;
            var symtab = sh.LinkedSection;
            var rdr = imgLoader.CreateReader(sh.FileOffset);
            for (ulong i = 0; i < entries; ++i)
            {
                if (!rdr.TryReadUInt32(out uint offset))
                    return;
                if (!rdr.TryReadUInt32(out uint info))
                    return;
                if (!rdr.TryReadInt32(out int addend))
                    return;

                uint sym = info >> 8;
                string symStr = GetStrPtr(symtab, sym);
                DebugEx.Verbose(ElfImageLoader.trace, "  RELA {0:X8} {1,3} {2:X8} {3:X8} {4}", offset, info & 0xFF, sym, addend, symStr);
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
                var dyn = new Elf32_Dyn();
                if (!rdr.TryReadInt32(out dyn.d_tag))
                    break;
                if (dyn.d_tag == ElfDynamicEntry.DT_NULL)
                    break;
                if (!rdr.TryReadInt32(out int val))
                    break;
                dyn.d_val = val;
                yield return new ElfDynamicEntry(dyn.d_tag, dyn.d_ptr);
            }
        }


        public override Address GetEntryPointAddress(Address addrBase)
        {
            if (Header.e_entry == 0)
                return null;
            else
                return Address.Ptr32(Header.e_entry);
        }

        internal ElfSection GetSectionInfoByAddr(uint r_offset)
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
            return (int) (Sections[Header.e_shstrndx].FileOffset + idxString);
        }

        public string GetStrPtr(int idx, uint offset)
        {
            if (idx < 0)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentException(string.Format("GetStrPtr passed index of {0}.", idx));
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString(Sections[idx].FileOffset + offset);
        }

        public string GetSymbolName(int iSymbolSection, uint symbolNo)
        {
            var symSection = Sections[iSymbolSection];
            return GetSymbolName(symSection, symbolNo);
        }

        public string GetSymbolName(ElfSection symSection, uint symbolNo)
        {
            var strSection = symSection.LinkedSection;
            if (strSection == null)
                return string.Format("null:{0:X8}", symbolNo);
            uint offset = (uint) (symSection.FileOffset + symbolNo * symSection.EntrySize);
            var rdr = imgLoader.CreateReader(offset);
            rdr.TryReadUInt32(out offset);
            return GetStrPtr(strSection, offset);
        }

        public override SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred)
        {
            var segMap = AllocateMemoryAreas(
                Segments
                    .Where(p => IsLoadable(p.p_pmemsz, p.p_type))
                    .Select(p => Tuple.Create(
                        Address.Ptr32((uint) p.p_vaddr),
                        (uint) p.p_pmemsz)));

            foreach (var ph in Segments)
            {
                DebugEx.Inform(ElfImageLoader.trace, "ph: addr {0:X8} filesize {0:X8} memsize {0:X8}", ph.p_vaddr, ph.p_filesz, ph.p_pmemsz);
                if (!IsLoadable(ph.p_pmemsz, ph.p_type))
                    continue;
                var vaddr = Address.Ptr32((uint) ph.p_vaddr);
                segMap.TryGetLowerBound(vaddr, out var mem);
                if (ph.p_filesz > 0)
                {
                    Array.Copy(
                        rawImage,
                        (long) ph.p_offset, mem.Bytes,
                        vaddr - mem.BaseAddress, (long) ph.p_filesz);
                }
            }
            var segmentMap = new SegmentMap(addrPreferred);
            if (Sections.Count > 0)
            {
                foreach (var section in Sections)
                {
                    if (string.IsNullOrEmpty(section.Name) || section.Address == null)
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
                        seg.Designer = CreateRenderer(section, machine);
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
            var rdr = imgLoader.CreateReader(Header.e_phoff);
            for (int i = 0; i < Header.e_phnum; ++i)
            {
                var sSeg = Elf32_PHdr.Load(rdr);
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
            var rela = Elf32_Rel.Read(rdr);
            return new ElfRelocation
            {
                Offset = rela.r_offset,
                Info = rela.r_info,
                SymbolIndex = (int) (rela.r_info >> 8)
            };
        }

        public override ElfRelocation LoadRelaEntry(EndianImageReader rdr)
        {
            var rela = Elf32_Rela.Read(rdr);
            return new ElfRelocation
            {
                Offset = rela.r_offset,
                Info = rela.r_info,
                Addend = rela.r_addend,
                SymbolIndex = (int) (rela.r_info >> 8)
            };
        }

        public override void LoadSectionHeaders()
        {
            // Create the sections.
            var inames = new List<uint>();
            var links = new List<uint>();
            var infos = new List<uint>();
            var rdr = imgLoader.CreateReader(Header.e_shoff);
            for (uint i = 0; i < Header.e_shnum; ++i)
            {
                var shdr = Elf32_SHdr.Load(rdr);
                if (shdr == null)
                    break;
                var section = new ElfSection
                {
                    Number = i,
                    Type = shdr.sh_type,
                    Flags = shdr.sh_flags,
                    Address = Address.Ptr32(shdr.sh_addr),
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
            var sym = Elf32_Sym.Load(rdr);
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
            DebugEx.Inform(ElfImageLoader.trace, "== Symbols from {0} ==", symSection.Name);
            var stringtableSection = symSection.LinkedSection;
            var rdr = CreateReader(symSection.FileOffset);
            var symbols = new Dictionary<int, ElfSymbol>();
            for (ulong i = 0; i < symSection.Size / symSection.EntrySize; ++i)
            {
                var sym = Elf32_Sym.Load(rdr);
                var symName = RemoveModuleSuffix(ReadAsciiString(stringtableSection.FileOffset + sym.st_name));
                DebugEx.Verbose(ElfImageLoader.trace, "  {0,3} {1,-25} {2,-12} {3,6} {4,-15} {5:X8} {6,9}",
                    i,
                    string.IsNullOrWhiteSpace(symName) ? "<empty>" : symName,
                    (ElfSymbolType) (sym.st_info & 0xF),
                    sym.st_shndx,
                    GetSectionName(sym.st_shndx),
                    sym.st_value,
                    sym.st_size);
                symbols.Add((int) i, new ElfSymbol
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
            if (!rdr.TryReadUInt32(out uint uAddr))
                return null;
            var addr = Address.Ptr32(uAddr);
            return addr;
        }
    }
}

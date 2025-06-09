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
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.ImageLoaders.Elf.Relocators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.ImageLoaders.Elf
{
    public class ElfLoader64 : ElfLoader
    {
        private readonly byte osAbi;

        public ElfLoader64(
            IServiceProvider services,
            ElfBinaryImage elfImage,
            byte[] rawImage)
            : base(services, elfImage, rawImage)
        {
            this.osAbi = elfImage.Header.osAbi;
            base.rawImage = rawImage;
        }

        public override Address DefaultAddress => Address.Ptr64(0x8048000);

        public override ulong AddressToFileOffset(ulong addr)
        {
            foreach (ElfSegment ph in BinaryImage.Segments)
            {
                if (ph.VirtualAddress.Offset <= addr && addr < ph.VirtualAddress.Offset + ph.FileSize)
                    return (addr - ph.VirtualAddress.Offset) + ph.FileOffset;
            }
            return ~0ul;
        }

        public override Address ComputeBaseAddress(IPlatform platform)
        {
            ulong uBaseAddr = BinaryImage.Segments
                .Where(ph => ph.VirtualAddress.Offset > 0 && ph.FileSize > 0)
                .Min(ph => ph.VirtualAddress.Offset);
            return platform.MakeAddressFromLinear(uBaseAddr, true);
        }

        public override Address CreateAddress(ulong uAddr)
        {
            return Address.Ptr64(uAddr);
        }

        public override IProcessorArchitecture CreateArchitecture(ElfMachine elfMachine, EndianServices endianness)
        {
            var options = new Dictionary<string, object>();
            options[ProcessorOption.Endianness] = endianness == EndianServices.Little ? "le" : "be";
            string archName;
            switch (elfMachine)
            {
            case ElfMachine.EM_IA_64:
                archName = "ia64";
                break;
            case ElfMachine.EM_MIPS:
                //$TODO: detect release 6 of the MIPS architecture. 
                // would be great to get our sweaty little hands on
                // such a binary.
                archName = endianness == EndianServices.Little ? "mips-le-64" :  "mips-be-64";
                options[ProcessorOption.WordSize] = 64;
                break;
            case ElfMachine.EM_PARISC:
                archName = "paRisc";
                options[ProcessorOption.WordSize] = 64;
                break;
            case ElfMachine.EM_PPC64:
                archName = endianness == EndianServices.Little ? "ppc-le-64" : "ppc-be-64";
                options[ProcessorOption.WordSize] = 64;
                break;
            case ElfMachine.EM_RISCV: 
                archName = "risc-v";
                options[ProcessorOption.WordSize] = 64;
                var flags = (RiscVFlags) BinaryImage.Header.Flags;
                // According to the Risc-V ELF spec, a RV64G implementation is strongly
                // encouraged to support the LP64D ABI
                if ((flags & RiscVFlags.EF_RISCV_FLOAT_ABI_MASK) == 0)
                {
                    flags |= RiscVFlags.EF_RISCV_FLOAT_ABI_DOUBLE;
                }
                RiscVElf.SetOptions(flags, options);
                break;
            case ElfMachine.EM_S390: //$REVIEW: any pertinent differences?
                archName = "zSeries";
                options[ProcessorOption.WordSize] = 64;
                break;
            case ElfMachine.EM_BA64:
                archName = "beyond";
                options[ProcessorOption.WordSize] = 64;
                break;

            default:
                return base.CreateArchitecture(elfMachine, endianness);
            }
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture(archName, options);
            if (arch is null)
                throw new InvalidOperationException($"Unknown architecture '{archName}'.");
            return arch;
        }

        public override ElfObjectLinker CreateLinker(IProcessorArchitecture arch)
        {
            return new ElfObjectLinker64(this, arch, rawImage);
        }

        private ImageSegmentRenderer? CreateRenderer64(ElfSection shdr)
        {
            switch (shdr.Type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer64(this, shdr, BinaryImage.Header.Machine);
            case SectionHeaderType.SHT_RELA:
                return new RelaSegmentRenderer64(this, shdr);
            case SectionHeaderType.SHT_SYMTAB:
            case SectionHeaderType.SHT_DYNSYM:
                return new SymtabSegmentRenderer64(this, shdr);
            default: return null;
            }
        }

        public override ElfRelocator CreateRelocator(
            ElfMachine machine,
            IProcessorArchitecture arch,
            SortedList<Address, ImageSymbol> symbols,
            Dictionary<ElfSymbol, Address> plt)
        {
            switch (machine)
            {
            case ElfMachine.EM_AARCH64: return new Arm64Relocator(this, symbols);
            case ElfMachine.EM_X86_64: return new x86_64Relocator(this, symbols, plt);
            case ElfMachine.EM_PPC64: return new PpcRelocator64(this, symbols);
            case ElfMachine.EM_MIPS: return new MipsRelocator64(this, symbols);
            case ElfMachine.EM_RISCV: return new RiscVRelocator64(this, symbols);
            case ElfMachine.EM_ALPHA: return new AlphaRelocator(this, symbols);
            case ElfMachine.EM_S390: return new zSeriesRelocator(this, symbols);
            case ElfMachine.EM_PARISC: return new PaRiscRelocator(this, symbols);
            case ElfMachine.EM_SPARCV9: return new Sparc64Relocator(this, symbols);
            case ElfMachine.EM_IA_64: return new Ia64Relocator(this, symbols);
            }
            return base.CreateRelocator(machine, arch, symbols, plt);
        }

        public override void Dump(Address addrLoad, TextWriter writer)
        {
            writer.WriteLine("Entry: {0:X}", BinaryImage.Header.StartAddress);
            writer.WriteLine("Sections:");
            foreach (var sh in BinaryImage.Sections)
            {
                writer.WriteLine("{0,-18} sh_type: {1,-12} sh_flags: {2,-4} sh_addr: {3:X8} sh_offset: {4:X8} sh_size: {5:X8} sh_link: {6,-18} sh_info: {7,-18} sh_addralign: {8:X8} sh_entsize: {9:X8}",
                    sh.Name,
                    sh.Type,
                    DumpShFlags(sh.Flags),
                    sh.VirtualAddress,
                    sh.FileOffset,
                    sh.Size,
                    sh.LinkedSection is not null ? sh.LinkedSection.Name : "",
                    sh.RelocatedSection is not null ? sh.RelocatedSection.Name : "",
                    sh.Alignment,
                    sh.EntrySize);
            }
            writer.WriteLine();
            writer.WriteLine("Program headers:");
            foreach (var ph in BinaryImage.Segments)
            {
                writer.WriteLine("p_type:{0,-12} p_offset: {1:X8} p_vaddr:{2:X8} p_paddr:{3:X8} p_filesz:{4:X8} p_pmemsz:{5:X8} p_flags:{6} {7:X8} p_align:{8:X8}",
                    ph.p_type,
                    ph.FileOffset,
                    ph.VirtualAddress,
                    ph.PhysicalAddress,
                    ph.FileSize,
                    ph.MemorySize,
                    rwx(ph.Flags & 7),
                    ph.Flags,
                    ph.Alignment);
            }
            writer.WriteLine("Base address: {0:X8}", addrLoad);
            writer.WriteLine();
            writer.WriteLine("Dependencies");
            foreach (var dep in BinaryImage.Dependencies)
            {
                writer.WriteLine("  {0}", dep);
            }

            writer.WriteLine();
            writer.WriteLine("Relocations");
            foreach (var sh in BinaryImage.Sections.Where(sh => sh.Type == SectionHeaderType.SHT_REL))
            {
                DumpRel(sh);
            }
            foreach (var sh in BinaryImage.Sections.Where(sh => sh.Type == SectionHeaderType.SHT_RELA))
            {
                DumpRela(sh);
            }
        }

        private void DumpRel(ElfSection sh)
        {
            var entries = sh.Size / sh.EntrySize;
            var symtab = sh.LinkedSection;
            var rdr = CreateReader(sh.FileOffset);
            for (ulong i = 0; i < entries; ++i)
            {
                if (!rdr.TryReadUInt64(out ulong offset))
                    return;
                if (!rdr.TryReadUInt64(out ulong info))
                    return;

                ulong sym = info >> 8;
                string symStr = GetStrPtr(symtab!, sym);
                ElfImageLoader.trace.Verbose("  RELA {0:X16} {1,3} {2:X16} {3}", offset, info & 0xFF, sym, symStr);
            }
        }

        private void DumpRela(ElfSection sh)
        {
            var entries = sh.Size / sh.EntrySize;
            var symtab = sh.LinkedSection;
            var rdr = CreateReader(sh.FileOffset);
            for (ulong i = 0; i < entries; ++i)
            {
                if (!rdr.TryReadUInt64(out ulong offset))
                    return;
                if (!rdr.TryReadUInt64(out ulong info))
                    return;
                if (!rdr.TryReadInt64(out long addend))
                    return;

                ulong sym = info >> 8;
                string symStr = GetStrPtr(symtab!, sym);
                ElfImageLoader.trace.Verbose("  RELA {0:X16} {1,3} {2:X16} {3:X16} {4}", offset, info & 0xFF, sym, addend, symStr);
            }
        }

        public override IEnumerable<ElfDynamicEntry> GetDynamicEntries(EndianImageReader rdr)
        {
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


        public override (Address?, ProcessorState?) GetEntryPointAddress(Address addrBase, Program program)
        {
            Address? addr = null;
            ProcessorState? estate = null;
            //$REVIEW: should really have a subclassed "Ps3ElfLoader"
            if (osAbi == ElfLoader.ELFOSABI_CELL_LV2)
            {
                // The BinaryImage.Header.e_entry field actually points to a 
                // "function descriptor" consisiting of two 32-bit 
                // pointers.
                if (program.TryCreateImageReader(BinaryImage.Header.StartAddress, out var rdr) &&
                    rdr.TryReadUInt32(out uint uAddr))
                {
                    addr = Address.Ptr32(uAddr);
                }
            }
            else if (this.BinaryImage.Header.Machine == ElfMachine.EM_PPC64)
            {
                if (BinaryImage.Header.Flags == 2)
                {
                    estate = program.Architecture.CreateProcessorState();
                    var uAddr_r2 = Ppc64abiv2Setup(program, estate, BinaryImage.Header.StartAddress);
                    if (uAddr_r2 is null)
                        return default;
                    program.GlobalRegisterValue = uAddr_r2;
                    addr = BinaryImage.Header.StartAddress;
                    return (addr, estate);
                }

                // The entrypoint address actually points to a function descriptor.
                // The first piece is a 64-bit pointer to the _actual_ executable
                // code, the other is the initial value of r2.
                if (program.TryCreateImageReader(BinaryImage.Header.StartAddress, out var rdr) &&
                    rdr.TryReadUInt64(out ulong uAddrCode) &&
                    rdr.TryReadUInt64(out ulong uAddrR2))  // initial TOC value in r2
                {
                    program.GlobalRegisterValue = Constant.UInt64(uAddrR2);
                    addr = Address.Ptr64(uAddrCode);
                    estate = program.Architecture.CreateProcessorState();
                    estate.SetRegister(
                        program.Architecture.GetRegister("r2")!,
                        Constant.Create(PrimitiveType.Ptr64, uAddrR2));
                }
            }
            else if (this.IsExecutableFile)
            {
                addr = BinaryImage.Header.StartAddress;
            }
            return (addr, estate);
        }

        private Constant? Ppc64abiv2Setup(Program program, ProcessorState state, Address startAddress)
        {
            if (!program.TryCreateImageReader(startAddress, out var rdr))
                return null;
            var r12 = program.Architecture.GetRegister("r12")!;
            state.SetRegister(r12, startAddress.ToConstant());
            var lifter = program.Architecture.CreateRewriter(rdr, state, new StorageBinder(), new NullRewriterHost());
            var clusters = lifter.Take(2).ToArray();
            if (clusters[0].Instructions[0] is RtlAssignment ass1 &&
                clusters[1].Instructions[0] is RtlAssignment ass2 &&
                ass1.Dst == ass2.Dst &&
                ass1.Dst is Identifier idDst &&
                idDst.Storage is RegisterStorage reg &&
                reg.Number == 2 &&
                ass1.Src is BinaryExpression bin1 &&
                    bin1.Left is Identifier id1 &&
                    id1.Storage is RegisterStorage reg1 &&
                    reg1 == r12 &&
                    bin1.Right is Constant c1 &&
                    ass2.Src is BinaryExpression bin2 &&
                 bin2.Left == ass1.Dst &&
                 bin2.Right is Constant c2)
            {
                // Compute the TOC address from r12 value. The r2 register
                // will be offset 0x8000 from the actual beginning of the TOC.
                var uAddr_r2 = startAddress.ToLinear() + c1.ToUInt64() + c2.ToUInt64();
                return Constant.Word64(uAddr_r2);
            }
            return null;
        }

        internal ElfSection? GetSectionInfoByAddr64(ulong r_offset)
        {
            return
                (from sh in this.BinaryImage.Sections
                 let addr = sh.VirtualAddress.ToLinear()
                 where
                    r_offset != 0 &&
                    addr <= r_offset && r_offset < addr + sh.Size
                 select sh)
                .FirstOrDefault();
        }

        protected override int GetSectionNameOffset(List<ElfSection> sections, uint idxString)
        {
            return (int) (sections[BinaryImage.Header.e_shstrndx].FileOffset + idxString);
        }

        public string GetSymbol64(ElfSection symSection, ulong symbolNo)
        {
            ulong offset = symSection.FileOffset + symbolNo * symSection.EntrySize;
            var rdr = CreateReader(offset);
            rdr.TryReadUInt64(out offset);
            return GetStrPtr(symSection.LinkedSection!, (uint) offset);
        }

        public override SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred)
        {
            var segMap = AllocateMemoryAreas(
                BinaryImage.Segments
                    .Where(p => IsLoadable(p.MemorySize, p.p_type))
                    .OrderBy(p => p.VirtualAddress)
                    .Select(p => (
                        p.VirtualAddress,
                        (uint) p.MemorySize)));
            foreach (var ph in BinaryImage.Segments)
            {
                ElfImageLoader.trace.Inform("ph: addr {0:X8} filesize {0:X8} memsize {0:X8}", ph.VirtualAddress, ph.FileSize, ph.MemorySize);
                if (!IsLoadable(ph.MemorySize, ph.p_type))
                    continue;
                var vaddr = ph.VirtualAddress;
                segMap.TryGetLowerBound(vaddr, out var mem);
                if (ph.FileSize > 0)
                    Array.Copy(
                        rawImage,
                        (long) ph.FileOffset, mem.Bytes,
                        vaddr - mem.BaseAddress, (long) ph.FileSize);
            }
            var segmentMap = new SegmentMap(addrPreferred);
            if (BinaryImage.Sections.Count > 0)
            {
                foreach (var section in BinaryImage.Sections)
                {
                    if (section.Name is null || section.VirtualAddress.IsNull)
                        continue;
                    if (segMap.TryGetLowerBound(section.VirtualAddress, out var mem) &&
                        mem.IsValidAddress(section.VirtualAddress))
                    {
                        AccessMode mode = AccessModeOf(section.Flags);
                        var seg = segmentMap.AddSegment(new ImageSegment(
                            section.Name,
                            section.VirtualAddress,
                            mem, mode)
                        {
                            Size = (uint) section.Size,
                            IsBss = section.Type ==  SectionHeaderType.SHT_NOBITS, 
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

        public override IReadOnlyList<ElfSegment> LoadSegments()
        {
            var rdr = CreateReader(BinaryImage.Header.e_phoff);
            for (int i = 0; i < BinaryImage.Header.e_phnum; ++i)
            {
                var sSeg = Elf64_PHdr.Load(rdr);
                BinaryImage.AddSegment(new ElfSegment
                {
                    p_type = sSeg.p_type,
                    FileOffset = sSeg.p_offset,
                    VirtualAddress = platform!.MakeAddressFromLinear(sSeg.p_vaddr, false),
                    PhysicalAddress = platform!.MakeAddressFromLinear(sSeg.p_paddr, false),
                    FileSize = sSeg.p_filesz,
                    MemorySize = sSeg.p_pmemsz,
                    Flags = sSeg.p_flags,
                    Alignment = sSeg.p_align,
                });
            }
            return BinaryImage.Segments;
        }

        public override ElfRelocation LoadRelEntry(EndianImageReader rdr, IDictionary<int, ElfSymbol> symbols)
        {
            var rel = Elf64_Rel.Read(rdr);
            var symbolIndex = rel.SymbolIndex;
            return new ElfRelocation
            {
                Offset = rel.r_offset,
                Info = rel.r_info,
                Type = (uint) rel.r_info,
                SymbolIndex = symbolIndex,
                Symbol = symbols[symbolIndex],
            };
        }

        public override ElfRelocation LoadRelaEntry(EndianImageReader rdr, IDictionary<int, ElfSymbol> symbols)
        {
            var rela = Elf64_Rela.Read(rdr);
            int symbolIndex = (int) (rela.r_info >> 32);
            return new ElfRelocation
            {
                Offset = rela.r_offset,
                Info = rela.r_info,
                Type = (uint) rela.r_info,
                Addend = rela.r_addend,
                SymbolIndex = symbolIndex,
                Symbol = symbols[symbolIndex]
            };
        }

        public override void LoadFileHeader()
        {
            var elf = BinaryImage;
            var rdr = CreateReader(HEADER_OFFSET);

            var header64 = Elf64_EHdr.Load(rdr);
            var trace = ElfImageLoader.trace;
            trace.Verbose("== ELF header =================");
            trace.Verbose("  e_entry: {0:X16}", header64.e_entry);
            trace.Verbose("  e_phoff: {0:X16}", header64.e_phoff);
            trace.Verbose("  e_shoff: {0:X16}", header64.e_shoff);
            trace.Verbose("  e_flags: {0:X8}", header64.e_flags);
            trace.Verbose("  e_ehsize: {0}", header64.e_ehsize);
            trace.Verbose("  e_phentsize: {0}", header64.e_phentsize);
            trace.Verbose("  e_phnum: {0}", header64.e_phnum);
            trace.Verbose("  e_shentsize: {0}", header64.e_shentsize);
            trace.Verbose("  e_shnum: {0}", header64.e_shnum);
            trace.Verbose("  e_shstrndx: {0}", header64.e_shstrndx);
            elf.Header.BinaryFileType = FileTypeOf(header64.e_type);
            elf.Header.StartAddress = Address.Ptr64(header64.e_entry);
            elf.Header.Machine = (ElfMachine) header64.e_machine;
            elf.Header.e_phoff = header64.e_phoff;
            elf.Header.e_shoff = header64.e_shoff;
            elf.Header.Flags = header64.e_flags;
            elf.Header.e_phnum = header64.e_phnum;
            elf.Header.e_shnum = header64.e_shnum;
            elf.Header.e_shstrndx = header64.e_shstrndx;
            elf.Header.PointerType = PrimitiveType.Ptr64;
        }

        public override List<ElfSection> LoadSectionHeaders()
        {
            // Create the sections.
            var inames = new List<uint>();
            var links = new List<uint>();
            var infos = new List<uint>();
            var sections = new List<ElfSection>();
            var rdr = CreateReader(BinaryImage.Header.e_shoff);
            for (int i = 0; i < BinaryImage.Header.e_shnum; ++i)
            {
                var shdr = Elf64_SHdr.Load(rdr);
                var section = new ElfSection
                {
                    Index = i,
                    Type = shdr.sh_type,
                    Flags = shdr.sh_flags,
                    VirtualAddress = shdr.sh_addr != 0
                        ? platform!.MakeAddressFromLinear(shdr.sh_addr, false)
                        : default,
                    FileOffset = shdr.sh_offset,
                    FileSize = shdr.sh_type == SectionHeaderType.SHT_NOBITS
                        ? 0
                        : shdr.sh_size,
                    Size = shdr.sh_size,
                    Alignment = shdr.sh_addralign,
                    EntrySize = shdr.sh_entsize,
                };
                sections.Add(section);
                inames.Add(shdr.sh_name);
                links.Add(shdr.sh_link);
                infos.Add(shdr.sh_info);
            }

            // Get section names and crosslink sections.

            for (int i = 0; i < sections.Count; ++i)
            {
                var section = sections[i];
                section.Name = ReadSectionName(sections, inames[i]);

                ElfSection? linkSection = null;
                ElfSection? relSection = null;
                switch (section.Type)
                {
                case SectionHeaderType.SHT_REL:
                case SectionHeaderType.SHT_RELA:
                    linkSection = GetSectionByIndex(sections, links[i]);
                    relSection = GetSectionByIndex(sections, infos[i]);
                    break;
                case SectionHeaderType.SHT_DYNAMIC:
                case SectionHeaderType.SHT_HASH:
                case SectionHeaderType.SHT_SYMTAB:
                case SectionHeaderType.SHT_DYNSYM:
                    linkSection = GetSectionByIndex(sections, links[i]);
                    break;
                }
                section.LinkedSection = linkSection;
                section.RelocatedSection = relSection;
            }
            return sections;
        }

        public override ElfSymbol? LoadSymbol(ulong offsetSymtab, ulong symbolIndex, ulong entrySize, ulong offsetStringTable)
        {
            var rdr = CreateReader(offsetSymtab + entrySize * symbolIndex);
            if (!Elf64_Sym.TryLoad(rdr, out var sym))
                return null;
            var name = ReadAsciiString(offsetStringTable + sym.st_name);
            return new ElfSymbol(name)
            {
                Type = (ElfSymbolType) (sym.st_info & 0xF),
                Bind = (ElfSymbolBinding) (sym.st_info >> 4),
                SectionIndex = sym.st_shndx,
                Value = sym.st_value,
                Size = sym.st_size,
            };
        }

        public override Dictionary<int, ElfSymbol> LoadSymbolsSection(ElfSection symSection)
        {
            ElfImageLoader.trace.Inform("== Symbols from {0} ==", symSection.Name);
            var symbols = new Dictionary<int, ElfSymbol>();
            var stringtableSection = symSection.LinkedSection!;
            var rdr = CreateReader(symSection.FileOffset);
            for (ulong i = 0; i < symSection.Size / symSection.EntrySize; ++i)
            {
                if (!Elf64_Sym.TryLoad(rdr, out var sym))
                {
                    ElfImageLoader.trace.Warn("Unable to load symbol entry {0} from {1}", i, symSection.Name);
                    continue;
                }
                var name = ReadAsciiString(stringtableSection.FileOffset + sym.st_name);
                ElfImageLoader.trace.Verbose("  {0,3} {1,-25} {2,-12} {3,6} {4} {5,-15} {6:X16} {7,9}",
                    i,
                    string.IsNullOrWhiteSpace(name) ? "<empty>" : name,
                    (ElfSymbolType) (sym.st_info & 0xF),
                    sym.st_shndx,
                    GetBindingName((ElfSymbolBinding) (sym.st_info >> 4)),
                    GetSectionName(sym.st_shndx),
                    sym.st_value,
                    sym.st_size);

                var esym = new ElfSymbol(name)
                {
                    Type = (ElfSymbolType) (sym.st_info & 0xF),
                    Bind = (ElfSymbolBinding) (sym.st_info >> 4),
                    SectionIndex = sym.st_shndx,
                    Value = sym.st_value,
                    Size = sym.st_size,
                };
                symbols.Add((int) i, esym);
            }
            return symbols;
        }

        public override Address? ReadAddress(EndianImageReader rdr)
        {
            if (!rdr.TryReadUInt64(out ulong uAddrSym))
                return null;

            var addr = Address.Ptr64(uAddrSym);
            return addr;
        }
    }
}
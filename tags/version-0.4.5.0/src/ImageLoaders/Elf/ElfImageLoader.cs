#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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


// http://hitmen.c02.at/files/yapspd/psp_doc/chap26.html - PSP ELF

using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    using ADDRESS = UInt32;
    using StrIntMap = Dictionary<string, int>;
    using RelocMap = Dictionary<UInt32, string>;
    using Decompiler.Core.Configuration;
    using System.Collections;
    using Decompiler.Core.Services;

    /// <summary>
    /// Loader for ELF images.
    /// </summary>
    public class ElfImageLoader : ImageLoader
    {
        private const int ELF_MAGIC = 0x7F454C46;         // "\x7FELF"
        private const byte LITTLE_ENDIAN = 1;
        private const byte BIG_ENDIAN = 2;
        private const byte ELFCLASS32 = 1;              // 32-bit object file
        private const byte ELFCLASS64 = 2;              // 64-bit object file
        private const int HEADER_OFFSET = 0x0010;

        private const int ELFOSABI_NONE = 0x00;         // No specific ABI specified.
        private const int ELFOSABI_CELL_LV2 = 0x66;     // PS/3 has this in its files

        private const ushort EM_NONE = 0;           // No machine 
        private const ushort EM_M32 = 1;            // AT&T WE 32100 
        private const ushort EM_SPARC = 2;          // SPARC 
        private const ushort EM_386 = 3;            // Intel 80386 
        private const ushort EM_68K = 4;            // Motorola 68000 
        private const ushort EM_88K = 5;            // Motorola 88000 
        //private const ushort RESERVED 6 Reserved for future use 
        private const ushort EM_860 = 7;            // Intel 80860 
        private const ushort EM_MIPS = 8;           // MIPS I Architecture 
        private const ushort EM_S370 = 9;           // IBM System/370 Processor 
        private const ushort EM_MIPS_RS3_LE = 10;   // MIPS RS3000 Little-endian 
        //private const ushort RESERVED 11-14 Reserved for future use 
        private const ushort EM_PARISC = 15;        // Hewlett-Packard PA-RISC 
        //private const ushort RESERVED 16 Reserved for future use 
        private const ushort EM_VPP500 = 17;        // Fujitsu VPP500 
        private const ushort EM_SPARC32PLUS = 18;   // Enhanced instruction set SPARC 
        private const ushort EM_960 = 19;           // Intel 80960 
        private const ushort EM_PPC = 20;           // PowerPC 
        private const ushort EM_PPC64 = 21;         // 64-bit PowerPC 
        //private const ushort RESERVED 22-35 Reserved for future use 
        private const ushort EM_V800 = 36;          // NEC V800 
        private const ushort EM_FR20 = 37;          // Fujitsu FR20 
        private const ushort EM_RH32 = 38;          // TRW RH-32 
        private const ushort EM_RCE = 39;           // Motorola RCE 
        private const ushort EM_ARM = 40;           // Advanced RISC Machines ARM 
        private const ushort EM_ALPHA = 41;         // Digital Alpha 
        private const ushort EM_SH = 42;            // Hitachi SH 
        private const ushort EM_SPARCV9 = 43;       // SPARC Version 9 
        private const ushort EM_TRICORE = 44;       // Siemens Tricore embedded processor 
        private const ushort EM_ARC = 45;           // Argonaut RISC Core, Argonaut Technologies Inc. 
        private const ushort EM_H8_300 = 46;        // Hitachi H8/300 
        private const ushort EM_H8_300H = 47;       // Hitachi H8/300H 
        private const ushort EM_H8S = 48;           // Hitachi H8S 
        private const ushort EM_H8_500 = 49;        // Hitachi H8/500 
        private const ushort EM_IA_64 = 50;         // Intel IA-64 processor architecture 
        private const ushort EM_MIPS_X = 51;        // Stanford MIPS-X 
        private const ushort EM_COLDFIRE = 52;      // Motorola ColdFire 
        private const ushort EM_68HC12 = 53;        // Motorola M68HC12 
        private const ushort EM_MMA = 54;           // Fujitsu MMA Multimedia Accelerator 
        private const ushort EM_PCP = 55;           // Siemens PCP 
        private const ushort EM_NCPU = 56;          // Sony nCPU embedded RISC processor 
        private const ushort EM_NDR1 = 57;          // Denso NDR1 microprocessor 
        private const ushort EM_STARCORE = 58;      // Motorola Star*Core processor 
        private const ushort EM_ME16 = 59;          // Toyota ME16 processor 
        private const ushort EM_ST100 = 60;         // STMicroelectronics ST100 processor 
        private const ushort EM_TINYJ = 61;         // Advanced Logic Corp. TinyJ embedded processor family 
        private const ushort EM_X86_64 = 62;        // AMD x86-64 
        //private const ushort Reserved 63-65 Reserved for future use 
        private const ushort EM_FX66 = 66;          // Siemens FX66 microcontroller 
        private const ushort EM_ST9PLUS = 67;       // STMicroelectronics ST9+ 8/16 bit microcontroller 
        private const ushort EM_ST7 = 68;           // STMicroelectronics ST7 8-bit microcontroller 
        private const ushort EM_68HC16 = 69;        // Motorola MC68HC16 Microcontroller 
        private const ushort EM_68HC11 = 70;        // Motorola MC68HC11 Microcontroller 
        private const ushort EM_68HC08 = 71;        // Motorola MC68HC08 Microcontroller 
        private const ushort EM_68HC05 = 72;        // Motorola MC68HC05 Microcontroller 
        private const ushort EM_SVX = 73;           // Silicon Graphics SVx 
        private const ushort EM_ST19 = 74;          // STMicroelectronics ST19 8-bit microcontroller 
        private const ushort EM_VAX = 75;           // Digital VAX  
        private const ushort EM_CRIS = 76;          // Axis Communications 32-bit embedded processor 
        private const ushort EM_JAVELIN = 77;       // Infineon Technologies 32-bit embedded processor 
        private const ushort EM_FIREPATH = 78;      // Element 14 64-bit DSP Processor 
        private const ushort EM_ZSP = 79;           // LSI Logic 16-bit DSP Processor 
        private const ushort EM_MMIX = 80;          // Donald Knuth's educational 64-bit processor 
        private const ushort EM_HUANY = 81;         // Harvard University machine-independent object files 
        private const ushort EM_PRISM = 82;         // SiTera Prism 
        private const ushort EM_AARCH64 = 183;      // ARM AARCH64

        private const uint SHF_WRITE = 0x1;
        private const uint SHF_ALLOC = 0x2;
        private const uint SHF_EXECINSTR = 0x4;

        private const int ET_REL = 0x01;

        private const int R_386_COPY = 5;

        private int ELF32_R_SYM(int info) { return ((info) >> 8); }
        private int ELF32_ST_BIND(int i) { return ((i) >> 4); }
        private int ELF32_ST_TYPE(int i) { return ((i) & 0xf); }
        private int ELF32_ST_INFO(int b, int t) { return (((b) << 4) + ((t) & 0xf)); }

        const int STT_NOTYPE = 0;			// Symbol table type: none
        const int STT_FUNC = 2;				// Symbol table type: function
        const int STT_SECTION = 3;
        const int STT_FILE = 4;
        const int STB_GLOBAL = 1;
        const int STB_WEAK = 2;

        private byte fileClass;             // 0x2 = 
        private byte endianness;
        private byte fileVersion;
        private byte osAbi;
        private IProcessorArchitecture arch;
        private Platform platform;
        private Address addrPreferred;
        private LoadedImage image;
        private ImageMap imageMap;
        private Dictionary<Address, ImportReference> importReferences;
        private List<EntryPoint> entryPoints;
        private ulong m_uPltMin;
        private ulong m_uPltMax;

        public ElfImageLoader(IServiceProvider services, string filename, byte[] rawBytes)
            : base(services, filename, rawBytes)
        {
            this.entryPoints = new List<EntryPoint>();
        }

        public Elf32_EHdr Header32 { get; set; }
        public Elf64_EHdr Header64 { get; set; }
        public List<Elf32_SHdr> SectionHeaders { get; private set; }
        public List<Elf64_SHdr> SectionHeaders64 { get; private set; }
        public List<Elf32_PHdr> ProgramHeaders { get; private set; }
        public List<Elf64_PHdr> ProgramHeaders64 { get; private set; }
        public override Address PreferredBaseAddress {
            get { return addrPreferred; }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            LoadElfIdentification();
            LoadHeader();
            this.platform = CreatePlatform(osAbi);
            LoadProgramHeaderTable();

            LoadSectionHeaders();

            GetPltLimits();
            addrPreferred = ComputeBaseAddress();
            var addrMax = ComputeMaxAddress();
            Dump();
            return LoadImageBytes(addrPreferred, addrMax);
        }

   

        // Find the PLT limits. Required for IsDynamicLinkedProc(), e.g.
        private void GetPltLimits()
        {
            if (fileClass == ELFCLASS64)
            {
                var pPlt = GetSectionInfoByName64(".plt");
                if (pPlt != null)
                {
                    m_uPltMin = pPlt.sh_addr;
                    m_uPltMax = pPlt.sh_addr + pPlt.sh_size; ;
                }
            }
            else
            {
                var pPlt = GetSectionInfoByName32(".plt");
                if (pPlt != null)
                {
                    m_uPltMin = pPlt.sh_addr;
                    m_uPltMax = pPlt.sh_addr + pPlt.sh_size; ;
                }
            }
        }

        public void LoadElfIdentification()
        {
            var rdr = new BeImageReader(base.RawImage, 0);
            var elfMagic = rdr.ReadBeInt32();
            if (elfMagic != ELF_MAGIC)
                throw new BadImageFormatException("File is not in ELF format.");
            this.fileClass = rdr.ReadByte();
            this.endianness = rdr.ReadByte();
            this.fileVersion = rdr.ReadByte();
            this.osAbi = rdr.ReadByte();
        }

        private Program LoadImageBytes(Address addrPreferred, Address addrMax)
        {
            var bytes = new byte[addrMax - addrPreferred];
            var v_base = addrPreferred.ToLinear();
            this.image = new LoadedImage(addrPreferred, bytes);
            this.imageMap = image.CreateImageMap();

            if (fileClass == ELFCLASS64)
            {
                foreach (var ph in ProgramHeaders64)
                {
                    if (ph.p_vaddr > 0 && ph.p_filesz > 0)
                        Array.Copy(RawImage, (long)ph.p_offset, bytes, (long)(ph.p_vaddr - v_base), (long)ph.p_filesz);
                }
                foreach (var segment in SectionHeaders64)
                {
                    if (segment.sh_name == 0 || segment.sh_addr == 0)
                        continue;
                    AccessMode mode = AccessMode.Read;
                    if ((segment.sh_flags & SHF_WRITE) != 0)
                        mode |= AccessMode.Write;
                    if ((segment.sh_flags & SHF_EXECINSTR) != 0)
                        mode |= AccessMode.Execute;
                    var seg = imageMap.AddSegment(
                        platform.MakeAddressFromLinear(segment.sh_addr), 
                        GetSectionName(segment.sh_name),
                        mode);
                    seg.Designer = CreateRenderer64(segment);
                }
            }
            else
            {
                foreach (var ph in ProgramHeaders)
                {
                    if (ph.p_vaddr > 0 && ph.p_filesz > 0)
                        Array.Copy(RawImage, (long)ph.p_offset, bytes, (long)(ph.p_vaddr - v_base), (long)ph.p_filesz);
                }
                foreach (var segment in SectionHeaders)
                {
                    if (segment.sh_name == 0 || segment.sh_addr == 0)
                        continue;
                    AccessMode mode = AccessMode.Read;
                    if ((segment.sh_flags & SHF_WRITE) != 0)
                        mode |= AccessMode.Write;
                    if ((segment.sh_flags & SHF_EXECINSTR) != 0)
                        mode |= AccessMode.Execute;
                    var seg = imageMap.AddSegment(Address.Ptr32(segment.sh_addr), GetSectionName(segment.sh_name), mode);
                    seg.Designer = CreateRenderer(segment);
                }
                imageMap.DumpSections();
            }
            var program = new Program(
                this.image,
                this.imageMap,
                this.arch,
                this.platform);
            importReferences = program.ImportReferences;
            return program;
        }

        public Platform CreatePlatform(byte osAbi)
        {
            OperatingEnvironment env;
            Platform platform;
            var dcSvc = Services.RequireService<IConfigurationService>();
            switch (osAbi)
            {
            case ELFOSABI_NONE: // Unspecified ABI
                env = dcSvc.GetEnvironment("elf-neutral");
                platform = env.Load(Services, arch);
                return platform;
            case ELFOSABI_CELL_LV2: // PS/3
                env = dcSvc.GetEnvironment("elf-cell-lv2");
                platform = env.Load(Services, arch);
                return platform;
            default:
                throw new NotSupportedException(string.Format("Unsupported ELF ABI 0x{0:X2}.", osAbi));
            }
        }

        public IEnumerable<TypeLibrary> LoadTypeLibraries()
        {
            var dcSvc = Services.GetService<IConfigurationService>();
            if (dcSvc == null)
                return new TypeLibrary[0];
            var env = dcSvc.GetEnvironment("elf-neutral");
            if (env == null)
                return new TypeLibrary[0];
            var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();
            return ((IEnumerable)env.TypeLibraries)
                    .OfType<ITypeLibraryElement>()
                    .Where(tl => tl.Architecture == "ppc32")
                    .Select(tl => tlSvc.LoadLibrary(arch,  dcSvc.GetPath(tl.Name)))
                    .Where(tl => tl != null);
        }

        private ImageMapSegmentRenderer CreateRenderer64(Elf64_SHdr shdr)
        {
            return null;        //$NYI
        }


        private ImageMapSegmentRenderer CreateRenderer(Elf32_SHdr shdr)
        {
            switch (shdr.sh_type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer(this, shdr);
            case SectionHeaderType.SHT_DYNSYM:
                return new SymtabSegmentRenderer(this, shdr);
            case SectionHeaderType.SHT_REL:
                return new RelSegmentRenderer(this, shdr);
            case SectionHeaderType.SHT_RELA:
                return new RelaSegmentRenderer(this, shdr);
            default: return null;
            }
        }

        public void LoadProgramHeaderTable()
        {
            if (fileClass == ELFCLASS64)
            {
                this.ProgramHeaders64 = new List<Elf64_PHdr>();
                var rdr = CreateReader(Header64.e_phoff);
                for (int i = 0; i < Header64.e_phnum; ++i)
                {
                    ProgramHeaders64.Add(Elf64_PHdr.Load(rdr));
                }
            }
            else
            {
                this.ProgramHeaders = new List<Elf32_PHdr>();
                var rdr = CreateReader(Header32.e_phoff);
                for (int i = 0; i < Header32.e_phnum; ++i)
                {
                    ProgramHeaders.Add(Elf32_PHdr.Load(rdr));
                }
            }
        }

        public void LoadSectionHeaders()
        {
            if (fileClass == ELFCLASS64)
            {
                this.SectionHeaders64 = new List<Elf64_SHdr>();
                var rdr = CreateReader(Header64.e_shoff);
                for (int i = 0; i < Header64.e_shnum; ++i)
                {
                    SectionHeaders64.Add(Elf64_SHdr.Load(rdr));
                }
            }
            else
            {
                this.SectionHeaders = new List<Elf32_SHdr>();
                var rdr = CreateReader(Header32.e_shoff);
                for (int i = 0; i < Header32.e_shnum; ++i)
                {
                    SectionHeaders.Add(Elf32_SHdr.Load(rdr));
                }
            }
        }

        public void LoadHeader()
        {
            var rdr = CreateReader(HEADER_OFFSET);
            if (fileClass == ELFCLASS64)
            {
                this.Header64 = Elf64_EHdr.Load(rdr);
                arch = CreateArchitecture(Header64.e_machine);
            }
            else
            {
                this.Header32 = Elf32_EHdr.Load(rdr);
                arch = CreateArchitecture(Header32.e_machine);
            }
        }

        public Address ComputeBaseAddress()
        {
            if (fileClass == ELFCLASS64)
            {
                ulong uBaseAddr = ProgramHeaders64
                    .Where(ph => ph.p_vaddr > 0 && ph.p_filesz > 0)
                    .Min(ph => ph.p_vaddr);
                return platform.MakeAddressFromLinear(uBaseAddr);
            }
            else
            {
                return Address.Ptr32(
                    ProgramHeaders
                    .Where(ph => ph.p_vaddr > 0 && ph.p_filesz > 0)
                    .Min(ph => ph.p_vaddr));
            }
        }

        private Address ComputeMaxAddress()
        {
            if (fileClass == ELFCLASS64)
            {
                ulong uMaxAddress = 
                    ProgramHeaders64
                    .Where(ph => ph.p_vaddr > 0 && ph.p_filesz > 0)
                    .Select(ph => ph.p_vaddr + ph.p_pmemsz)
                    .Max();
                return platform.MakeAddressFromLinear(uMaxAddress);
            }
            else
            {
                return Address.Ptr32(
                    ProgramHeaders
                    .Where(ph => ph.p_vaddr > 0 && ph.p_filesz > 0)
                    .Select(ph => ph.p_vaddr + ph.p_pmemsz)
                    .Max());
            }
        }

        private IProcessorArchitecture CreateArchitecture(ushort machineType)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            string arch;
            switch (machineType)
            {
            case EM_NONE: return null; // No machine
            case EM_AARCH64: arch = "aarch64"; break;
            case EM_ARM: arch = "arm"; break;
            case EM_SPARC: arch = "sparc"; break;
            case EM_386: arch = "x86-protected-32"; break;
            case EM_X86_64: arch = "x86-protected-64"; break;
            case EM_68K: arch = "m68k"; break;
            case EM_PPC: arch = "ppc32"; break;
            case EM_PPC64: arch = "ppc64"; break;
            default:
                throw new NotSupportedException(string.Format("Processor format {0} is not supported.", machineType));
            }
            return cfgSvc.GetArchitecture(arch);
        }

        public ImageReader CreateReader(ulong fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageReader(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageReader(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public ImageReader CreateReader(uint fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageReader(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageReader(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        private ImageWriter CreateWriter(uint fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageWriter(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageWriter(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public string GetSectionName(uint idxString)
        {
            int offset;
            if (fileClass == ELFCLASS64)
            {
                offset = (int)(SectionHeaders64[Header64.e_shstrndx].sh_offset + idxString);
            }
            else
            {
                offset = (int)(SectionHeaders[Header32.e_shstrndx].sh_offset + idxString);
            }

            var i = offset;
            for (; i < RawImage.Length && RawImage[i] != 0; ++i)
                ;
            try
            { //$DEBUG }
                return Encoding.ASCII.GetString(RawImage, (int)offset, i - offset);
            } catch {
                return "<FAIL>";
            }
        }

        public void Dump()
        {
            var sw = new StringWriter();
            Dump(sw);
            Debug.Print(sw.ToString());
        }

        public void Dump(TextWriter writer)
        {
#if NOT
            writer.WriteLine("Entry: {0:X}", Header32.e_entry);
            writer.WriteLine("Sections:");
            foreach (var sh in SectionHeaders)
            {
                writer.WriteLine("{0,-18} sh_type: {1,-12} sh_flags: {2,-4} sh_addr; {3:X8} sh_offset: {4:X8} sh_size: {5:X8} sh_link: {6:X8} sh_info: {7:X8} sh_addralign: {8:X8} sh_entsize: {9:X8}",
                    GetSectionName(sh.sh_name),
                    sh.sh_type,
                    DumpShFlags(sh.sh_flags),
                    sh.sh_addr,
                    sh.sh_offset,
                    sh.sh_size,
                    sh.sh_link,
                    sh.sh_info,
                    sh.sh_addralign,
                    sh.sh_entsize);
            }
            writer.WriteLine();
            writer.WriteLine("Program headers:");
            foreach (var ph in ProgramHeaders)
            {
                writer.WriteLine("p_type:{0,-12} p_offset: {1:X8} p_vaddr:{2:X8} p_paddr:{3:X8} p_filesz:{4:X8} p_pmemsz:{5:X8} p_flags:{6:X8} p_align:{7:X8}",
                    ph.p_type,
                    ph.p_offset,
                    ph.p_vaddr,
                    ph.p_paddr,
                    ph.p_filesz,
                    ph.p_pmemsz,
                    ph.p_flags,
                    ph.p_align);
            }
            writer.WriteLine("Base address: {0:X8}", ComputeBaseAddress());
            writer.WriteLine();
            writer.WriteLine("Dependencies");
            foreach (var dep in GetDependencyList())
            {
                writer.WriteLine("  {0}", dep);
            }

            writer.WriteLine();
            writer.WriteLine("Relocations");
            foreach (var sh in SectionHeaders.Where(sh => sh.sh_type == SectionHeaderType.SHT_RELA))
            {
                DumpRela(sh);
            }
#endif
        }

        private void DumpRela(Elf32_SHdr sh)
        {
            var entries = sh.sh_size / sh.sh_entsize;
            var symtab = sh.sh_link;
            var rdr = CreateReader(sh.sh_offset);
            for (int i = 0; i < entries; ++i)
            {
                uint offset;
                if (!rdr.TryReadUInt32(out offset))
                    return;
                uint info;
                if (!rdr.TryReadUInt32(out info))
                    return;
                int addend;
                if (!rdr.TryReadInt32(out addend))
                    return;

                uint sym = info >> 8;
                string symStr = GetStrPtr((int)symtab, sym);
                Debug.Print("  RELA {0:X8} {1,3} {2:X8} {3:X8} {4}", offset, info&0xFF, sym, addend, symStr);
            }
        }

        private string DumpShFlags(uint shf)
        {
            return string.Format("{0}{1}{2}",
                ((shf & SHF_EXECINSTR) != 0) ? "x" : " ",
                ((shf & SHF_ALLOC) != 0) ? "a" : " ",
                ((shf & SHF_WRITE) != 0) ? "w" : " ");
        }

        public override RelocationResults Relocate(Address addrLoad)
        {
            if (image == null)
                throw new InvalidOperationException(); // No file loaded
            RelocationDictionary relocations = new RelocationDictionary();
            var addrEntry = GetEntryPointAddress();
            if (addrEntry != null)
            {
                var ep = new EntryPoint(addrEntry, arch.CreateProcessorState());
                entryPoints.Add(ep);
            }
            if (fileClass == ELFCLASS64)
            {
                if (Header64.e_machine == EM_PPC64)
                {
                    //$TODO
                }
                else
                    throw new NotImplementedException(string.Format("Relocations for architecture {0} not implemented.", Header64.e_machine));
            }
            else
            {
                switch (Header32.e_machine)
                {
                case EM_386: RelocateI386(); break;
                case EM_PPC: RelocatePpc32(); break;
                case EM_ARM: RelocateArm(); break;
                default: throw new NotImplementedException(string.Format("ELF relocation for {0} is not implemented yet.", arch.GetType().Name));
                }
            }
            return new RelocationResults(entryPoints, relocations);
        }

        private Address GetEntryPointAddress()
        {
            Address addr = null;
            if (fileClass == ELFCLASS64)
            {
                //$REVIEW: should really have a subclassed "Ps3ElfLoader"
                if (osAbi == ELFOSABI_CELL_LV2)
                {
                    // The Header64.e_entry field actually points to a 
                    // "function descriptor" consisiting of two 32-bit 
                    // pointers.
                    var rdr = CreateReader(Header64.e_entry-image.BaseAddress.ToLinear());
                    uint uAddr;
                    if (rdr.TryReadUInt32(out uAddr))
                        addr = Address.Ptr32(uAddr);
                }
                else
                {
                    addr = Address.Ptr64(Header64.e_entry);
                }
            }
            else
            {
                addr = Address.Ptr32(Header32.e_entry);
            }
            return addr;
        }

        private void RelocateI386()
        {
            uint nextFakeLibAddr = ~1u; // See R_386_PC32 below; -1 sometimes used for main
            for (int i = 1; i < this.SectionHeaders.Count; ++i)
            {
                var ps = SectionHeaders[i];
                if (ps.sh_type != SectionHeaderType.SHT_REL)
                    continue;
                // A section such as .rel.dyn or .rel.plt (without an addend 
                // field). Each entry has 2 words: r_offset and r_info. The 
                // r_offset is just the offset from the beginning of the 
                // section (section given by the section header's sh_info) to
                // the word to be modified. r_info has the type in the bottom
                // byte, and a symbol table index in the top 3 bytes. A symbol
                // table offset of 0 (STN_UNDEF) means use value 0. The symbol
                // table involved comes from the section header's sh_link
                // field.
                var rdrReloc = CreateReader(ps.sh_offset);
                uint size = ps.sh_size;
                // NOTE: the r_offset is different for .o files (ET_REL in the e_type header field) than for exe's
                // and shared objects!
                uint destNatOrigin = 0;
                uint destHostOrigin = 0;
                if (Header32.e_type == ET_REL)
                {
                    int destSection = (int)ps.sh_info;
                    destNatOrigin = SectionHeaders[destSection].sh_addr;
                    destHostOrigin = SectionHeaders[destSection].sh_offset;
                }
                int symSection = (int)SectionHeaders[i].sh_link; // Section index for the associated symbol table
                int strSection = (int)SectionHeaders[symSection].sh_link; // Section index for the string section assoc with this
                uint pStrSection = SectionHeaders[strSection].sh_offset;
                var symOrigin = SectionHeaders[symSection].sh_offset;
                var relocR = CreateReader(0);
                var relocW = CreateWriter(0);
                for (uint u = 0; u < size; u += 2 * sizeof(uint))
                {
                    uint r_offset = rdrReloc.ReadUInt32();
                    uint info = rdrReloc.ReadUInt32();

                    byte relType = (byte)info;
                    uint symTabIndex = info >> 8;
                    uint pRelWord; // Pointer to the word to be relocated
                    if (Header32.e_type == ET_REL)
                    {
                        pRelWord = destHostOrigin + r_offset;
                    }
                    else
                    {
                        if (r_offset == 0)
                            continue;
                        var destSec = GetSectionInfoByAddr(r_offset);
                        pRelWord = ~0u; // destSec.uHostAddr - destSec.uNativeAddr + r_offset;
                        destNatOrigin = 0;
                    }
                    uint A, S = 0, P;
                    int nsec;
                    var sym = Elf32_Sym.Load(CreateReader(symOrigin + symTabIndex * Elf32_Sym.Size));
                    switch (relType)
                    {
                    case 0: // R_386_NONE: just ignore (common)
                        break;
                    case 1: // R_386_32: S + A
                        // Read the symTabIndex'th symbol.
                        S = sym.st_value;
                        if (Header32.e_type == ET_REL)
                        {
                            nsec = sym.st_shndx;
                            if (nsec >= 0 && nsec < SectionHeaders.Count)
                                S += SectionHeaders[nsec].sh_addr;
                        }
                        A = relocR.ReadUInt32(pRelWord);
                        relocW.WriteUInt32(pRelWord, S + A);
                        break;
                    case 2: // R_386_PC32: S + A - P
                        if (ELF32_ST_TYPE(sym.st_info) == STT_SECTION)
                        {
                            nsec = sym.st_shndx;
                            if (nsec >= 0 && nsec < SectionHeaders.Count)
                                S = SectionHeaders[nsec].sh_addr;
                        }
                        else
                        {
                            S = sym.st_value;
                            if (S == 0)
                            {
                                // This means that the symbol doesn't exist in this module, and is not accessed
                                // through the PLT, i.e. it will be statically linked, e.g. strcmp. We have the
                                // name of the symbol right here in the symbol table entry, but the only way
                                // to communicate with the loader is through the target address of the call.
                                // So we use some very improbable addresses (e.g. -1, -2, etc) and give them entries
                                // in the symbol table
                                uint nameOffset = sym.st_name;
                                string pName = ReadAsciiString(image.Bytes, pStrSection + nameOffset);
                                // this is too slow, I'm just going to assume it is 0
                                //S = GetAddressByName(pName);
                                //if (S == (e_type == E_REL ? 0x8000000 : 0)) {
                                S = nextFakeLibAddr--; // Allocate a new fake address
                                AddSymbol(S, pName);
                                //}
                            }
                            else if (Header32.e_type == ET_REL)
                            {
                                nsec = sym.st_shndx;
                                if (nsec >= 0 && nsec < SectionHeaders.Count)
                                    S += SectionHeaders[nsec].sh_addr;
                            }
                        }
                        A = relocR.ReadUInt32(pRelWord);
                        P = destNatOrigin + r_offset;
                        relocW.WriteUInt32(pRelWord, S + A - P);
                        break;
                    case R_386_COPY:   // no action needed
                        break;
                    case 6: // R_386_GLOB_DAT: S
                        // S = sym.st_value;
                        // relocW.WriteUInt32(pRelWord, S);
                        break;
                    case 7:
                    case 8: // R_386_RELATIVE
                        // No need to do anything with these, if a shared object
                        var addr = Address.Ptr32(sym.st_value);
                        string symStr = ReadAsciiString(image.Bytes, pStrSection + sym.st_name);
                        if (addr.ToLinear() != 0)
                        {
                            importReferences.Add(
                                addr,
                                new NamedImportReference(addr, null, symStr));
                        }
                        break;
                    default:
                        throw new NotSupportedException("Relocation type " + (int)relType + " not handled yet");
                    }
                }
            }
        }

        /// <remarks>
        /// According to the ELF PPC32 documentation, the .rela.plt and .plt tables 
        /// should contain the same number of entries, even if the individual entry 
        /// sizes are distinct. The entries in .rela.plt refer to symbols while the
        /// entries in .plt are (writeable) pointers.  Any caller that jumps to one
        /// of pointers in the .plt table is a "trampoline", and should be replaced
        /// in the decompiled code with just a call to the symbol obtained from the
        /// .real.plt section.
        /// </remarks>
        public void RelocatePpc32()
        {
            var rela_plt = GetSectionInfoByName32(".rela.plt");
            var plt = GetSectionInfoByName32(".plt");
            var relaRdr = CreateReader(rela_plt.sh_offset);
            var pltRdr = CreateReader(plt.sh_offset);
            for (int i = 0; i < rela_plt.sh_size / rela_plt.sh_entsize; ++i)
            {
                // Read the .rela.plt entry
                uint offset;
                if (!relaRdr.TryReadUInt32(out offset))
                    return;
                uint info;
                if (!relaRdr.TryReadUInt32(out info))
                    return;
                int addend;
                if (!relaRdr.TryReadInt32(out addend))
                    return;

                // Read the .plt entry. We don't care about its contents,
                // only its address. Anyone accessing that address is
                // trying to access the symbol.

                uint thunkAddress;
                if (!pltRdr.TryReadUInt32(out thunkAddress))
                    break;

                uint sym = info >> 8;
                string symStr = GetSymbol((int)rela_plt.sh_link, (int)sym);

                var addr = Address.Ptr32(plt.sh_addr + (uint)i * 4);
                importReferences.Add(
                    addr,
                    new NamedImportReference(addr, null, symStr));
            }
        }

        public void RelocateArm()
        {
            // Add symbol info. Some symbols will be in the main table only, and others in the dynamic table only.
            // The best idea is to add symbols for all sections of the appropriate type.
            var textSeg = GetSectionInfoByName32(".text");
            foreach (var sec in SectionHeaders)
            {

                if (sec.sh_type == SectionHeaderType.SHT_SYMTAB || sec.sh_type == SectionHeaderType.SHT_DYNSYM)
                {
                    foreach (var symbol in ReadAllSymbols(sec)
                        .Where(s => (s.Info & 0xF) == STT_FUNC))
                    {
                        Debug.Print("Symbol: {0:X8} {1:X7} {2}", symbol.Value, symbol.Info, symbol.Name);
                        var addr = 
                            Address.Ptr32(symbol.Value);
                        if (symbol.Name == "main"
                            || (textSeg != null && textSeg.ContainsAddress(symbol.Value)))
                        {
                            entryPoints.Add(new EntryPoint(addr, symbol.Name, arch.CreateProcessorState()));
                        }
                        else
                        {
                            importReferences[addr] = new NamedImportReference(addr, null, symbol.Name);
                        }
                    }
                }
            }
            /* Buggy and likely wrong.
            var rel_plt = GetSectionInfoByName32(".rel.plt");
            var plt = GetSectionInfoByName32(".plt");
            var relRdr = CreateReader(rel_plt.sh_offset);
            var pltRdr = CreateReader(plt.sh_offset);
            for (int i = 0; i < rel_plt.sh_size / rel_plt.sh_entsize; ++i)
            {
                // Read .rel.plt entry
                uint offset;
                if (!relRdr.TryReadUInt32(out offset))
                    return;
                uint info;
                if (!relRdr.TryReadUInt32(out info))
                    return;

                if (info == 0)
                    continue;

                // only its address.

                uint thunkAddress;
                if (!pltRdr.TryReadUInt32(out thunkAddress))
                    break;

                uint sym = info >> 8;
                string symStr = GetSymbol((int)rel_plt.sh_link, (int)sym);

                var addr = Address.Ptr32(plt.sh_addr + (uint)i * 4);
                Debug.Print("ARM symbol: {1:X8}:{0}", symStr, addr);
                importReferences.Add(
                    addr,
                    new NamedImportReference(addr, null, symStr));
            }*/
        }

        public Elf64_SHdr GetSectionInfoByName64(string sectionName)
        {
            return
                (from sh in this.SectionHeaders64
                 let name = GetSectionName(sh.sh_name)
                 where name == sectionName
                 select sh)
                .FirstOrDefault();
        }

        private Elf32_SHdr GetSectionInfoByName32(string sectionName)
        {
            return
                (from sh in this.SectionHeaders
                 let name = GetSectionName(sh.sh_name)
                 where name == sectionName
                 select sh)
                .FirstOrDefault();
        }

        internal Elf64_SHdr GetSectionInfoByAddr64(ulong r_offset)
        {
            return
                (from sh in this.SectionHeaders64
                 where sh.sh_addr <= r_offset && r_offset < sh.sh_addr + sh.sh_size
                 select sh)
                .FirstOrDefault();
        }

        internal Elf32_SHdr GetSectionInfoByAddr(ADDRESS r_offset)
        {
            return
                (from sh in this.SectionHeaders
                 where sh.sh_addr <= r_offset && r_offset < sh.sh_addr + sh.sh_size
                 select sh)
                .FirstOrDefault();
        }

        internal string ReadAsciiString(byte [] bytes, ulong fileOffset)
        {
            int u = (int)fileOffset;
            while (bytes[u] != 0)
            {
                ++u;
            }
            return Encoding.ASCII.GetString(bytes, (int)fileOffset, u - (int)fileOffset);
        }

        // A map for extra symbols, those not in the usual Elf symbol tables

        private void AddSymbol(ADDRESS uNative, string pName)
        {
            //m_SymTab[uNative] = pName;
        }


        public string GetSymbol(int iSymbolSection, int symbolNo)
        {
            var symSection = SectionHeaders[iSymbolSection];
            var strSection = SectionHeaders[(int)symSection.sh_link];
            uint offset = symSection.sh_offset + (uint)symbolNo * symSection.sh_entsize;
            var rdr = CreateReader(offset);
            uint psym;
            rdr.TryReadUInt32(out psym);
            return GetStrPtr((int)symSection.sh_link, psym);
        }

        const int DT_NULL = 0;
        const int DT_NEEDED = 1;
        const int DT_STRTAB = 5;

        public IEnumerable<Elf64_Dyn> GetDynEntries64(ulong offset)
        {
            var rdr = CreateReader(offset);
            for (; ; )
            {
                var dyn = new Elf64_Dyn();
                if (!rdr.TryReadInt64(out dyn.d_tag))
                    break;
                if (dyn.d_tag == DT_NULL)
                    break;
                long val;
                if (!rdr.TryReadInt64(out val))
                    break;
                dyn.d_val = val;
                yield return dyn;
            }
        }

        public IEnumerable<Elf32_Dyn> GetDynEntries(uint offset)
        {
            var rdr = CreateReader(offset);
            for (;;)
            {
                var dyn = new Elf32_Dyn();
                if (!rdr.TryReadInt32(out dyn.d_tag))
                    break;
                if (dyn.d_tag == DT_NULL)
                    break;
                int val;
                if (!rdr.TryReadInt32(out val))
                    break;
                dyn.d_val = val;
                yield return dyn;
            }
        }

        /// <summary>
        /// Find the names of all shared objects this image depends on.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDependencyList()
        {
            var result = new List<string>();
            if (fileClass == ELFCLASS64)
            {
                var dynsect = GetSectionInfoByName64(".dynamic");
                if (dynsect == null)
                    return result; // no dynamic section = statically linked 

                var dynStrtab = GetDynEntries64(dynsect.sh_offset).Where(d => d.d_tag == DT_STRTAB).FirstOrDefault();
                if (dynStrtab == null)
                    return result;
                var section = GetSectionInfoByAddr64(dynStrtab.d_ptr);
                foreach (var dynEntry in GetDynEntries64(dynsect.sh_offset).Where(d => d.d_tag == DT_NEEDED))
                {
                    result.Add(ReadAsciiString(RawImage, section.sh_offset + dynEntry.d_ptr));
                }
            }
            else
            {
                var dynsect = GetSectionInfoByName32(".dynamic");
                if (dynsect == null)
                    return result; // no dynamic section = statically linked 

                var dynStrtab = GetDynEntries(dynsect.sh_offset).Where(d => d.d_tag == DT_STRTAB).FirstOrDefault();
                if (dynStrtab == null)
                    return result;
                var section = GetSectionInfoByAddr(dynStrtab.d_ptr);
                foreach (var dynEntry in GetDynEntries(dynsect.sh_offset).Where(d => d.d_tag == DT_NEEDED))
                {
                    result.Add(ReadAsciiString(RawImage, section.sh_offset + dynEntry.d_ptr));
                }
            }
            return result;
            /*
            var section = GetSectionInfoByAddr(dynStrtab);
            stringtab = NativeToHostAddress(stringtab);
            var rdr = CreateReader(dynsect.sh_offset);
            for (; ; )
            {
                Elf32_Dyn dyn = ReadDynEntry(rdr);
                if (dyn.d_tag == DT_NULL)
                    break;
                if (dyn.d_tag == DT_NEEDED)
                    for (dyn = (Elf32_Dyn*)dynsect.uHostAddr; dyn.d_tag != DT_NULL; dyn++)
                    {
                        if (dyn.d_tag == DT_NEEDED)
                        {
                            string need = (byte*)stringtab + dyn.d_un.d_val;
                            if (need != null)
                                result.Add(need);
                        }
                    }
                return result;
            }
             * */
        }

        /*==============================================================================
         * FUNCTION:	  ElfBinaryFile::GetImportStubs
         * OVERVIEW:	  Get an array of addresses of imported function stubs
         *					This function relies on the fact that the symbols are sorted by address, and that Elf PLT
         *					entries have successive addresses beginning soon after m_PltMin
         * PARAMETERS:	  numImports - reference to integer set to the number of these
         * RETURNS:		  An array of native ADDRESSes
         *============================================================================*/
        ADDRESS GetImportStubs(out int numImports)
        {
            int n = 0;
#if NYI
            ADDRESS a = m_uPltMin;
            std_map<ADDRESS, string>.iterator aa = m_SymTab.find(a);
            std_map<ADDRESS, string>.iterator ff = aa;
            bool delDummy = false;
            if (aa == m_SymTab.end())
            {
                // Need to insert a dummy entry at m_uPltMin
                delDummy = true;
                m_SymTab[a] = "";
                ff = m_SymTab.find(a);
                aa = ff;
                aa++;
            }
            while ((aa != m_SymTab.end()) && (a < m_uPltMax))
            {
                n++;
                a = aa.first;
                aa++;
            }
            // Allocate an array of ADDRESSESes
            m_pImportStubs = new ADDRESS[n];
            aa = ff; // Start at first
            a = aa.first;
            int i = 0;
            while ((aa != m_SymTab.end()) && (a < m_uPltMax))
            {
                m_pImportStubs[i++] = a;
                a = aa.first;
                aa++;
            }
            if (delDummy)
                m_SymTab.erase(ff); // Delete dummy entry
#endif
            numImports = n;
            return 0; //m_pImportStubs[];
        }

        // Add appropriate symbols to the symbol table.  secIndex is the section index of the symbol table.
        public IEnumerable<ElfSymbol> ReadAllSymbols(Elf32_SHdr pSect)
        {
            if (pSect == null)
                yield break;
            int e_type = this.Header32.e_type;
            // Calc number of symbols
            uint nSyms = pSect.sh_size / pSect.sh_entsize;
            uint offSym = pSect.sh_offset;
            //m_pSym = (Elf32_Sym*)pSect.uHostAddr; // Pointer to symbols
            uint strIdx = pSect.sh_link; // sh_link points to the string table

            var siPlt = GetSectionInfoByName32(".plt");
            ADDRESS addrPlt = siPlt!=null ? siPlt.sh_addr : 0;
            var siRelPlt = GetSectionInfoByName32(".rel.plt");
            uint sizeRelPlt = siRelPlt.sh_entsize; // Size of each entry in the .rel.plt table
            if (siRelPlt == null)
            {
                siRelPlt = GetSectionInfoByName32(".rela.plt");
                sizeRelPlt = siRelPlt.sh_entsize; // Size of each entry in the .rela.plt table is 12 bytes
            }
            ADDRESS addrRelPlt = 0;
            uint numRelPlt = 0;
            if (siRelPlt != null)
            {
                addrRelPlt = siRelPlt.sh_addr;
                numRelPlt = sizeRelPlt != 0 ? siRelPlt.sh_size / sizeRelPlt : 0u;
            }
            // Number of entries in the PLT:
            // int max_i_for_hack = siPlt ? (int)siPlt.uSectionSize / 0x10 : 0;
            // Index 0 is a dummy entry
            var symRdr = CreateReader(offSym);
            for (int i = 1; i < nSyms; i++)
            {
                uint name;
                if (!symRdr.TryReadUInt32(out name))
                    break;
                uint val;
                if (!symRdr.TryReadUInt32(out val))
                    break;
                uint size;
                if (!symRdr.TryReadUInt32(out size))
                    break;
                byte info;
                if (!symRdr.TryReadByte(out info))
                    break;
                byte other;
                if (!symRdr.TryReadByte(out other))
                    break;
                ushort shndx;
                if (!symRdr.TryReadLeUInt16(out shndx))
                    break;

                if (name == 0)
                    continue; // Weird: symbol w no name.
                if (shndx >= SectionHeaders.Count)
                {

                }
                else
                {
                    var otherSection = SectionHeaders[shndx];
                }
                string str = GetStrPtr((int)strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                // Ensure no overwriting (except functions)
#if Nilx
                if (@m_SymTab.ContainsKey(val) || ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FUNC)
                {
                    if (val == 0 && siPlt != null)
                    { //&& i < max_i_for_hack) {
                        throw new NotImplementedException();
                        // Special hack for gcc circa 3.3.3: (e.g. test/pentium/settest).  The value in the dynamic symbol table
                        // is zero!  I was assuming that index i in the dynamic symbol table would always correspond to index i
                        // in the .plt section, but for fedora2_true, this doesn't work. So we have to look in the .rel[a].plt
                        // section. Thanks, gcc!  Note that this hack can cause strange symbol names to appear
                        //val = findRelPltOffset(i, addrRelPlt, sizeRelPlt, numRelPlt, addrPlt);
                    }
                    else if (e_type == ET_REL)
                    {
                        throw new NotImplementedException();
#if NYI
                        int nsec = elfRead2(m_pSym[i].st_shndx);
                        if (nsec >= 0 && nsec < m_iNumSections)
                            val += GetSectionInfo(nsec)->uNativeAddr;
#endif
                    }

                    m_SymTab[val] = str;
                }
#endif
                //Debug.Print("  Symbol {0} ({0:X}) with address {1:X} (segment {2} {2:X}): {3}", i, val, shndx, str);
                yield return new ElfSymbol
                {
                    Name = str,
                    Value = val,
                    Info = info,
                };
            }
        }

        public string GetStrPtr(Elf32_SHdr sect, uint offset)
        {
            if (sect == null)
            {
                // Most commonly, this will be an null, because a call to GetSectionByName() failed
                throw new ArgumentException("GetStrPtr passed null section.");
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString( RawImage, sect.sh_offset + offset);
        }
        
        public string GetStrPtr(int idx, uint offset)
        {
            if (idx < 0)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentException(string.Format("GetStrPtr passed index of {0}.", idx));
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString(RawImage, SectionHeaders[idx].sh_offset + offset);
        }
    }

}
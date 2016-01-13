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

// http://hitmen.c02.at/files/yapspd/psp_doc/chap26.html - PSP ELF

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
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
        private const ushort EM_X86_64 = 62;        // AMD x86-64 architecture
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
        private const ushort EM_AVR           = 83; // Atmel AVR 8-bit microcontroller
        private const ushort EM_FR30          = 84; // Fujitsu FR30
        private const ushort EM_D10V          = 85; // Mitsubishi D10V
        private const ushort EM_D30V          = 86; // Mitsubishi D30V
        private const ushort EM_V850          = 87; // NEC v850
        private const ushort EM_M32R          = 88; // Mitsubishi M32R
        private const ushort EM_MN10300       = 89; // Matsushita MN10300
        private const ushort EM_MN10200       = 90; // Matsushita MN10200
        private const ushort EM_PJ            = 91; // picoJava
        private const ushort EM_OPENRISC      = 92; // OpenRISC 32-bit embedded processor
        private const ushort EM_ARC_COMPACT   = 93; // ARC International ARCompact processor (old
                                                    // spelling/synonym: EM_ARC_A5)
        private const ushort EM_XTENSA        = 94; // Tensilica Xtensa Architecture
        private const ushort EM_VIDEOCORE     = 95; // Alphamosaic VideoCore processor
        private const ushort EM_TMM_GPP       = 96; // Thompson Multimedia General Purpose Processor
        private const ushort EM_NS32K         = 97; // National Semiconductor 32000 series
        private const ushort EM_TPC           = 98; // Tenor Network TPC processor
        private const ushort EM_SNP1K         = 99; // Trebia SNP 1000 processor
        private const ushort EM_ST200         = 100; // STMicroelectronics (www.st.com) ST200
        private const ushort EM_IP2K          = 101; // Ubicom IP2xxx microcontroller family
        private const ushort EM_MAX           = 102; // MAX Processor
        private const ushort EM_CR            = 103; // National Semiconductor CompactRISC microprocessor
        private const ushort EM_F2MC16        = 104; // Fujitsu F2MC16
        private const ushort EM_MSP430        = 105; // Texas Instruments embedded microcontroller msp430
        private const ushort EM_BLACKFIN      = 106; // Analog Devices Blackfin (DSP) processor
        private const ushort EM_SE_C33        = 107; // S1C33 Family of Seiko Epson processors
        private const ushort EM_SEP           = 108; // Sharp embedded microprocessor
        private const ushort EM_ARCA          = 109; // Arca RISC Microprocessor
        private const ushort EM_UNICORE       = 110; // Microprocessor series from PKU-Unity Ltd. and MPRC
                                                     // of Peking University
        private const ushort EM_EXCESS        = 111; // eXcess: 16/32/64-bit configurable embedded CPU
        private const ushort EM_DXP           = 112; // Icera Semiconductor Inc. Deep Execution Processor
        private const ushort EM_ALTERA_NIOS2  = 113; // Altera Nios II soft-core processor
        private const ushort EM_CRX           = 114; // National Semiconductor CompactRISC CRX
        private const ushort EM_XGATE         = 115; // Motorola XGATE embedded processor
        private const ushort EM_C166          = 116; // Infineon C16x/XC16x processor
        private const ushort EM_M16C          = 117; // Renesas M16C series microprocessors
        private const ushort EM_DSPIC30F      = 118; // Microchip Technology dsPIC30F Digital Signal
                                                     // Controller
        private const ushort EM_CE            = 119; // Freescale Communication Engine RISC core
        private const ushort EM_M32C          = 120; // Renesas M32C series microprocessors
        private const ushort EM_TSK3000       = 131; // Altium TSK3000 core
        private const ushort EM_RS08          = 132; // Freescale RS08 embedded processor
        private const ushort EM_SHARC         = 133; // Analog Devices SHARC family of 32-bit DSP
                                                     // processors
        private const ushort EM_ECOG2         = 134; // Cyan Technology eCOG2 microprocessor
        private const ushort EM_SCORE7        = 135; // Sunplus S+core7 RISC processor
        private const ushort EM_DSP24         = 136; // New Japan Radio (NJR) 24-bit DSP Processor
        private const ushort EM_VIDEOCORE3    = 137; // Broadcom VideoCore III processor
        private const ushort EM_LATTICEMICO32 = 138; // RISC processor for Lattice FPGA architecture
        private const ushort EM_SE_C17        = 139; // Seiko Epson C17 family
        private const ushort EM_TI_C6000      = 140; // The Texas Instruments TMS320C6000 DSP family
        private const ushort EM_TI_C2000      = 141; // The Texas Instruments TMS320C2000 DSP family
        private const ushort EM_TI_C5500      = 142; // The Texas Instruments TMS320C55x DSP family
        private const ushort EM_MMDSP_PLUS    = 160; // STMicroelectronics 64bit VLIW Data Signal Processor
        private const ushort EM_CYPRESS_M8C   = 161; // Cypress M8C microprocessor
        private const ushort EM_R32C          = 162; // Renesas R32C series microprocessors
        private const ushort EM_TRIMEDIA      = 163; // NXP Semiconductors TriMedia architecture family
        private const ushort EM_HEXAGON       = 164; // Qualcomm Hexagon processor
        private const ushort EM_8051          = 165; // Intel 8051 and variants
        private const ushort EM_STXP7X        = 166; // STMicroelectronics STxP7x family of configurable
                                                     // and extensible RISC processors
        private const ushort EM_NDS32         = 167; // Andes Technology compact code size embedded RISC
                                                     // processor family
        private const ushort EM_ECOG1         = 168; // Cyan Technology eCOG1X family
        private const ushort EM_ECOG1X        = 168; // Cyan Technology eCOG1X family
        private const ushort EM_MAXQ30        = 169; // Dallas Semiconductor MAXQ30 Core Micro-controllers
        private const ushort EM_XIMO16        = 170; // New Japan Radio (NJR) 16-bit DSP Processor
        private const ushort EM_MANIK         = 171; // M2000 Reconfigurable RISC Microprocessor
        private const ushort EM_CRAYNV2       = 172; // Cray Inc. NV2 vector architecture
        private const ushort EM_RX            = 173; // Renesas RX family
        private const ushort EM_METAG         = 174; // Imagination Technologies META processor
                                                     // architecture
        private const ushort EM_MCST_ELBRUS   = 175; // MCST Elbrus general purpose hardware architecture
        private const ushort EM_ECOG16        = 176; // Cyan Technology eCOG16 family
        private const ushort EM_CR16          = 177; // National Semiconductor CompactRISC CR16 16-bit
                                                     // microprocessor
        private const ushort EM_ETPU          = 178; // Freescale Extended Time Processing Unit
        private const ushort EM_SLE9X         = 179; // Infineon Technologies SLE9X core
        private const ushort EM_L10M          = 180; // Intel L10M
        private const ushort EM_K10M          = 181; // Intel K10M
        private const ushort EM_AARCH64       = 183; // ARM AArch64
        private const ushort EM_AVR32         = 185; // Atmel Corporation 32-bit microprocessor family
        private const ushort EM_STM8          = 186; // STMicroeletronics STM8 8-bit microcontroller
        private const ushort EM_TILE64        = 187; // Tilera TILE64 multicore architecture family
        private const ushort EM_TILEPRO       = 188; // Tilera TILEPro multicore architecture family
        private const ushort EM_CUDA          = 190; // NVIDIA CUDA architecture
        private const ushort EM_TILEGX        = 191; // Tilera TILE-Gx multicore architecture family
        private const ushort EM_CLOUDSHIELD   = 192; // CloudShield architecture family
        private const ushort EM_COREA_1ST     = 193; // KIPO-KAIST Core-A 1st generation processor family
        private const ushort EM_COREA_2ND     = 194; // KIPO-KAIST Core-A 2nd generation processor family
        private const ushort EM_ARC_COMPACT2  = 195; // Synopsys ARCompact V2
        private const ushort EM_OPEN8         = 196; // Open8 8-bit RISC soft processor core
        private const ushort EM_RL78          = 197; // Renesas RL78 family
        private const ushort EM_VIDEOCORE5    = 198; // Broadcom VideoCore V processor
        private const ushort EM_78KOR         = 199; // Renesas 78KOR family
        private const ushort EM_56800EX       = 200; // Freescale 56800EX Digital Signal Controller (DSC)
        private const ushort EM_BA1           = 201; // Beyond BA1 CPU architecture
        private const ushort EM_BA2           = 202; // Beyond BA2 CPU architecture
        private const ushort EM_XCORE         = 203; // XMOS xCORE processor family
        private const ushort EM_MCHP_PIC      = 204; // Microchip 8-bit PIC(r) family
        private const ushort EM_INTEL205      = 205; // Reserved by Intel
        private const ushort EM_INTEL206      = 206; // Reserved by Intel
        private const ushort EM_INTEL207      = 207; // Reserved by Intel
        private const ushort EM_INTEL208      = 208; // Reserved by Intel
        private const ushort EM_INTEL209      = 209; // Reserved by Intel
        private const ushort EM_KM32          = 210; // KM211 KM32 32-bit processor
        private const ushort EM_KMX32         = 211; // KM211 KMX32 32-bit processor
        private const ushort EM_KMX16         = 212; // KM211 KMX16 16-bit processor
        private const ushort EM_KMX8          = 213; // KM211 KMX8 8-bit processor
        private const ushort EM_KVARC         = 214; // KM211 KVARC processor
        private const ushort EM_CDP           = 215; // Paneve CDP architecture family
        private const ushort EM_COGE          = 216; // Cognitive Smart Memory Processor
        private const ushort EM_COOL          = 217; // iCelero CoolEngine
        private const ushort EM_NORC          = 218; // Nanoradio Optimized RISC
        private const ushort EM_CSR_KALIMBA   = 219; // CSR Kalimba architecture family
        private const ushort EM_AMDGPU        = 224;  // AMD GPU architecture
  
        private const uint SHF_WRITE = 0x1;
        private const uint SHF_ALLOC = 0x2;
        private const uint SHF_EXECINSTR = 0x4;

        private const int ET_REL = 0x01;

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
        private IPlatform platform;
        private Address addrPreferred;
        private LoadedImage image;
        private ImageMap imageMap;
        private Dictionary<Address, ImportReference> importReferences;
        private ulong m_uPltMin;
        private ulong m_uPltMax;

        public ElfImageLoader(IServiceProvider services, string filename, byte[] rawBytes)
            : base(services, filename, rawBytes)
        {
        }

        public Elf32_EHdr Header32 { get; set; }
        public Elf64_EHdr Header64 { get; set; }
        public List<Elf32_SHdr> SectionHeaders { get; private set; }
        public List<Elf64_SHdr> SectionHeaders64 { get; private set; }
        public List<Elf32_PHdr> ProgramHeaders { get; private set; }
        public List<Elf64_PHdr> ProgramHeaders64 { get; private set; }

        public override Address PreferredBaseAddress { 
            get { return addrPreferred; }
            set { addrPreferred = value; }
        }

        public override Program Load(Address addrLoad)
        {
            LoadElfIdentification();
            LoadHeader();
            this.platform = CreatePlatform(osAbi);
            LoadProgramHeaderTable();

            LoadSectionHeaders();

            GetPltLimits();
            //LoadSymbols();
            addrPreferred = ComputeBaseAddress();
            var addrMax = ComputeMaxAddress();
            Dump();
            return LoadImageBytes(addrPreferred, addrMax);
        }

        private void LoadSymbols()
        {
            // Add symbol info. Some symbols will be in the main table only, and others in the dynamic table only.
            // The best idea is to add symbols for all sections of the appropriate type
            foreach (var sec in SectionHeaders)
            {
                if (sec.sh_type == SectionHeaderType.SHT_SYMTAB || sec.sh_type == SectionHeaderType.SHT_DYNSYM)
                    AddSyms(sec);
            }
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
                        mode,
                        (uint)segment.sh_size);
                    seg.Designer = CreateRenderer64(segment);
                }
            }
            else
            {
                foreach (var ph in ProgramHeaders)
                {
                    if (ph.p_vaddr > 0 && ph.p_filesz > 0)
                        Array.Copy(RawImage, (long)ph.p_offset, bytes, (long)(ph.p_vaddr - v_base), (long)ph.p_filesz);
                    Debug.Print("ph: addr {0:X8} filesize {0:X8} memsize {0:X8}", ph.p_vaddr, ph.p_filesz, ph.p_pmemsz);
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
                    var seg = imageMap.AddSegment(
                        Address.Ptr32(segment.sh_addr),
                        GetSectionName(segment.sh_name),
                        mode, 
                        segment.sh_size);
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

        public IPlatform CreatePlatform(byte osAbi)
        {
            string envName;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            switch (osAbi)
            {
            case ELFOSABI_NONE: // Unspecified ABI
                envName = "elf-neutral";
                break;
            case ELFOSABI_CELL_LV2: // PS/3
                envName = "elf-cell-lv2";
                break;
            default:
                throw new NotSupportedException(string.Format("Unsupported ELF ABI 0x{0:X2}.", osAbi));
            }
            return cfgSvc.GetEnvironment(envName).Load(Services, arch);
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
                    .Select(tl => tlSvc.LoadLibrary(platform, tl.Name))
                    .Where(tl => tl != null);
        }

        private ImageMapSegmentRenderer CreateRenderer64(Elf64_SHdr shdr)
        {
            switch (shdr.sh_type)
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

        private ImageMapSegmentRenderer CreateRenderer(Elf32_SHdr shdr)
        {
            switch (shdr.sh_type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer(this, shdr);
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
            case EM_SPARC: arch = "sparc"; break;
            case EM_386: arch = "x86-protected-32"; break;
            case EM_X86_64: arch = "x86-protected-64"; break;
            case EM_68K: arch = "m68k"; break;
            case EM_MIPS: arch = "mips-be-32"; break;
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
            return Encoding.ASCII.GetString(RawImage, (int)offset, i - offset);
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

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            if (image == null)
                throw new InvalidOperationException(); // No file loaded
            var entryPoints = new List<EntryPoint>();
            var relocations = new RelocationDictionary();
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
                else if (Header64.e_machine == EM_X86_64)
                {
                    RelocateX86_64();
                }
                else
                    throw new NotImplementedException(string.Format("Relocations for architecture {0} not implemented.", Header64.e_machine));
            }
            else
            {
                switch (Header32.e_machine)
                {
                case EM_386:
                    RelocateI386();
                    break;
                case EM_PPC:
                    RelocatePpc32();
                    break;
                case EM_MIPS:
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            return new RelocationResults(entryPoints, relocations, new List<Address>());
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
                    var rdr = CreateReader(Header64.e_entry - image.BaseAddress.ToLinear());
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
                if (ps.sh_type == SectionHeaderType.SHT_REL)
                {
                    // A section such as .rel.dyn or .rel.plt (without an addend field).
                    // Each entry has 2 words: r_offset and r_info. The r_offset is just the offset from the beginning
                    // of the section (section given by the section header's sh_info) to the word to be modified.
                    // r_info has the type in the bottom byte, and a symbol table index in the top 3 bytes.
                    // A symbol table offset of 0 (STN_UNDEF) means use value 0. The symbol table involved comes from
                    // the section header's sh_link field.
                    var pReloc = CreateReader(ps.sh_offset);
                    uint size = ps.sh_size;
                    // NOTE: the r_offset is different for .o files (ET_REL in the e_type header field) than for exe's
                    // and shared objects!
                    uint destNatOrigin = 0;
                    uint destHostOrigin = 0;
                    if (Header32.e_type == ET_REL)
                    {
                        int destSection = (int)SectionHeaders[i].sh_info;
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
                        uint r_offset = pReloc.ReadUInt32();
                        uint info = pReloc.ReadUInt32();

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
                        case 7:
                        case 8: // R_386_RELATIVE
                            break; // No need to do anything with these, if a shared object
                        default:
                            throw new NotSupportedException("Relocation type " + (int)relType + " not handled yet");
                        }
                    }
                }
            }
        }

        /// <remarks>
        /// According to the ELF PPC32 documentation, the .rela.plt and .plt tables 
        /// should contain the same number of entries, even if the individual entry 
        /// sizes are distinct. The entries in .real.plt refer to symbols while the
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
            for (int i =0; i < rela_plt.sh_size / rela_plt.sh_entsize; ++i)
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

        /// <remarks>
        /// According to the ELF PPC32 documentation, the .rela.plt and .plt tables 
        /// should contain the same number of entries, even if the individual entry 
        /// sizes are distinct. The entries in .real.plt refer to symbols while the
        /// entries in .plt are (writeable) pointers.  Any caller that jumps to one
        /// of pointers in the .plt table is a "trampoline", and should be replaced
        /// in the decompiled code with just a call to the symbol obtained from the
        /// .real.plt section.
        /// </remarks>
        public void RelocateX86_64()
        {
            var rela_plt = GetSectionInfoByName64(".rela.plt");
            var plt = GetSectionInfoByName64(".plt");
            var relaRdr = CreateReader(rela_plt.sh_offset);
            for (ulong i = 0; i < rela_plt.sh_size / rela_plt.sh_entsize; ++i)
            {
                // Read the .rela.plt entry
                ulong offset;
                if (!relaRdr.TryReadUInt64(out offset))
                    return;
                ulong info;
                if (!relaRdr.TryReadUInt64(out info))
                    return;
                long addend;
                if (!relaRdr.TryReadInt64(out addend))
                    return;

                ulong sym = info >> 32;
                string symStr = GetSymbol64((int)rela_plt.sh_link, (int)sym);

                var addr = Address.Ptr64(plt.sh_addr + (uint)(i + 1) * plt.sh_entsize);
                importReferences.Add(
                    addr,
                    new NamedImportReference(addr, null, symStr));
            }
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

        internal Elf32_SHdr GetSectionInfoByAddr(uint r_offset)
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

        private void AddSymbol(uint uNative, string pName)
        {
            //m_SymTab[uNative] = pName;
        }


        public string GetSymbol(int iSymbolSection, int symbolNo)
        {
            var symSection = SectionHeaders[iSymbolSection];
            var strSection = SectionHeaders[(int)symSection.sh_link];
            uint offset = symSection.sh_offset + (uint)symbolNo * symSection.sh_entsize;
            var rdr = CreateReader(offset);
            rdr.TryReadUInt32(out offset);
            return GetStrPtr((int)symSection.sh_link, offset);
        }

        public string GetSymbol64(int iSymbolSection, int symbolNo)
        {
            var symSection = SectionHeaders64[iSymbolSection];
            var strSection = SectionHeaders64[(int)symSection.sh_link];
            ulong offset = symSection.sh_offset + (ulong)symbolNo * symSection.sh_entsize;
            var rdr = CreateReader(offset);
            rdr.TryReadUInt64(out offset);
            return GetStrPtr64((int)symSection.sh_link, (uint)offset);
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
        uint GetImportStubs(out int numImports)
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
        private void AddSyms(Elf32_SHdr pSect)
        {
            int e_type = this.Header32.e_type;
            // Calc number of symbols
            uint nSyms = pSect.sh_size / pSect.sh_entsize;
            uint offSym = pSect.sh_offset;
            //m_pSym = (Elf32_Sym*)pSect.uHostAddr; // Pointer to symbols
            uint strIdx = pSect.sh_link; // sh_link points to the string table

            var siPlt = GetSectionInfoByName32(".plt");
            uint addrPlt = siPlt!=null ? siPlt.sh_addr : 0;
            var siRelPlt = GetSectionInfoByName32(".rel.plt");
            uint sizeRelPlt = 8; // Size of each entry in the .rel.plt table
            if (siRelPlt == null)
            {
                siRelPlt = GetSectionInfoByName32(".rela.plt");
                sizeRelPlt = 12; // Size of each entry in the .rela.plt table is 12 bytes
            }
            uint addrRelPlt = 0;
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
                if (!symRdr.TryReadUInt32(out val)) //= (ADDRESS)elfRead4((int)m_pSym[i].st_value);
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
                Debug.Print("  Symbol {0} ({0:X}) with address {1:X} (segment {2} {2:X}): {3}", i, val, shndx, str);
            }
#if NYI
            ADDRESS uMain = GetMainEntryPoint();
            if (uMain != NO_ADDRESS && m_SymTab.find(uMain) == m_SymTab.end())
            {
                // Ugh - main mustn't have the STT_FUNC attribute. Add it
                string sMain = "main";
                m_SymTab[uMain] = sMain;
            }
            return;
#endif
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

        public string GetStrPtr64(Elf64_SHdr sect, uint offset)
        {
            if (sect == null)
            {
                // Most commonly, this will be an null, because a call to GetSectionByName() failed
                throw new ArgumentException("GetStrPtr passed null section.");
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString(RawImage, sect.sh_offset + offset);
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

        public string GetStrPtr64(int idx, uint offset)
        {
            if (idx < 0)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentException(string.Format("GetStrPtr passed index of {0}.", idx));
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString(RawImage, SectionHeaders64[idx].sh_offset + offset);
        }
#if ZLON
    public class ElfObsolete
    {
        private long m_lImageSize; // Size of image in bytes
        private byte[] m_pImage; // Pointer to the loaded image
        private Elf32_Ehdr pHeader;
        private Elf32_Phdr[] m_pPhdrs; // Pointer to program header
        private Elf32_Shdr[] m_pShdrs; // Array of section header structs
        private uint stringTableOffset; // Pointer to the string section
        private bool bigEndian; // 1 = Big Endian
        private Dictionary<ADDRESS, string> m_SymTab; // Map from address to symbol name; contains symbols from the
        // various elf symbol tables, and possibly some symbols with fake
        // addresses
        private SymTab m_Reloc; // Object to store the reloc syms
        private Elf32_Rel m_pReloc; // Pointer to the relocation section
        private Elf32_Sym[] m_pSym; // Pointer to loaded symbol section
        private bool m_bAddend; // true if reloc table has addend
        private ADDRESS m_uLastAddr; // Save last address looked up
        private int m_iLastSize; // Size associated with that name
        private ADDRESS m_uPltMin; // Min address of PLT table
        private ADDRESS m_uPltMax; // Max address (1 past last) of PLT
        private List<SectionInfo> m_EntryPoint; // A list of one entry point
        private ADDRESS m_pImportStubs; // An array of import stubs
        private ADDRESS first_extern; // where the first extern will be placed
        private ADDRESS next_extern; // where the next extern will be placed
        private int[] m_sh_link; // pointer to array of sh_link values
        private int[] m_sh_info; // pointer to array of sh_info values
        private int m_iNumSections;
        private SectionInfo[] m_pSections;
        private IProcessorArchitecture arch;
        private Platform platform;

        public ElfLoader(IServiceProvider sp, byte[] rawImage)
            : base(sp, rawImage)
        {
            next_extern = 0;
            m_pImage = null;
            m_pPhdrs = null; // No program headers
            m_pShdrs = null; // No section headers
            stringTableOffset = 0; // No strings
            m_pReloc = null;
            m_pSym = null;
            m_uPltMin = 0; // No PLT limits
            m_uPltMax = 0;
            m_iLastSize = 0;
            m_pImportStubs = 0;

            m_SymTab = new Dictionary<ADDRESS, string>();
        }

        public override Address PreferredBaseAddress { get { return Address.Ptr(0x00100000); } }
        public override IProcessorArchitecture Architecture { get { return arch; } }
        public override Platform Platform { get { return platform; } }
        
        private uint elf_hash(string s)
        {
            int i = 0;
            uint h = 0;
            while (i != s.Length)
            {
                h = (h << 4) + s[i++];
                uint g = h & 0xf0000000u;
                if (g != 0)
                    h ^= g >> 24;
                h &= ~g;
            }
            return h;
        }

        public virtual Dictionary<ADDRESS, string> getSymbols()
        {
            return m_SymTab;
        }

        /// <summary>
        /// Reads the ELF header.
        /// </summary>
        /// <returns></returns>
        private Elf32_Ehdr ReadElfHeaderStart()
        {
            var rdr = new ImageReader(RawImage, 0);
            var h = new Elf32_Ehdr();

            h.e_ident = rdr.ReadBeUInt32();
            
            h.e_class = rdr.ReadByte();
            h.endianness = rdr.ReadByte();
            h.version = rdr.ReadByte();
            h.osAbi = rdr.ReadByte();

            rdr.Seek(8);             // 8 bytes of padding.

            // Now that we know the endianness, read the remaining fields in endian mode.
            rdr = CreateImageReader(h.endianness, rdr.Offset);
            h.e_type = rdr.ReadInt16();
            h.e_machine = rdr.ReadInt16();
            h.e_version = rdr.ReadInt32();
            h.e_entry = rdr.ReadUInt32();
            h.e_phoff = rdr.ReadUInt32();
            h.e_shoff = rdr.ReadUInt32();
            h.e_flags = rdr.ReadInt32();
            h.e_ehsize = rdr.ReadInt16();
            h.e_phentsize = rdr.ReadInt16();
            h.e_phnum = rdr.ReadInt16();
            h.e_shentsize = rdr.ReadInt16();
            h.e_shnum = rdr.ReadInt16();
            h.e_shstrndx = rdr.ReadInt16();

            Dump("e_type: {0}", h.e_type);
            Dump("e_machine: {0}", (MachineType) h.e_machine);
            Dump("e_version: {0}", h.e_version);
            Dump("e_entry: {0:X}", h.e_entry);
            Dump("e_phoff: {0:X}", h.e_phoff);
            Dump("e_shoff: {0:X}", h.e_shoff);
            Dump("e_flags: {0:X}", h.e_flags);
            Dump("e_ehsize: {0}", h.e_ehsize);
            Dump("e_phentsize: {0}", h.e_phentsize);
            Dump("e_phnum: {0}", h.e_phnum);
            Dump("e_shentsize: {0}", h.e_shentsize);
            Dump("e_shnum: {0}", h.e_shnum);
            Dump("e_shstrndx: {0}", h.e_shstrndx);
            
            return h;
        }

        private ImageReader CreateImageReader(uint offset)
        {
            return CreateImageReader(pHeader.endianness, offset);
        }

        private ImageReader CreateImageReader(int endianness, uint offset)
        {
            switch (endianness)
            {
            case 1: return new LeImageReader(RawImage, offset);
            case 2: return new BeImageReader(RawImage, offset);
            default: throw new NotSupportedException(string.Format("Unknown endianness {0}.", endianness));
            }
        }

        private string ReadAsciiString(uint offset)
        {
            int o = (int)offset;
            var bytes = RawImage;
            int end = o;
            for (; bytes[end] != 0; ++end)
                ;
            return Encoding.ASCII.GetString(bytes, o, end - o);
        }

        public override ProgramImage Load(Address addrLoad)
        {
            int i;

            m_lImageSize = RawImage.Length;

            m_pImage = RawImage;
            pHeader = ReadElfHeaderStart();
            arch = GetProcessorArchitecture();
            platform = GetPlatform();

            if (pHeader.e_ident != ELF_MAGIC)   
                throw new BadImageFormatException("Incorrect ELF header.");

            if (pHeader.e_phoff != 0)
                m_pPhdrs = LoadProgramHeaders(pHeader.e_phnum, pHeader.e_phoff);

            if (pHeader.e_shoff != 0)
                m_pShdrs = LoadSectionHeaders(pHeader.e_shnum, pHeader.e_shoff);

            // Set up section header string table pointer
            if (pHeader.e_shstrndx != 0)
                stringTableOffset = m_pShdrs[pHeader.e_shstrndx].sh_offset;

            i = 1; // counter - # sects. Start @ 1, total m_iNumSections

            // Number of sections
            m_iNumSections = pHeader.e_shnum;

            // Allocate room for all the Elf sections (including the silly first one)
            m_pSections = new SectionInfo[m_iNumSections];

            // Set up the m_sh_link and m_sh_info arrays
            m_sh_link = new int[m_iNumSections];
            m_sh_info = new int[m_iNumSections];

            // Number of elf sections
            bool bGotCode = false; // True when have seen a code sect

            Address arbitaryLoadAddr = Address.Ptr32(addrLoad.Linear);
            var rdr = CreateImageReader(pHeader.e_shoff);
            for (i = 0; i < m_iNumSections; i++)
            {
                var pShdr = m_pShdrs[i];
                string pName = ReadAsciiString(m_pShdrs[pHeader.e_shstrndx].sh_offset + pShdr.sh_name);
                var sect = new SectionInfo();
                m_pSections[i] = sect;
                m_pSections[i].pSectionName = pName;
                var off = pShdr.sh_offset;
                if (pShdr.sh_offset != 0)
                    sect.uHostAddr = off;
                sect.uNativeAddr = pShdr.sh_addr;
                sect.uSectionSize = pShdr.sh_size;
                if (sect.uNativeAddr == 0 && pName.StartsWith(".rel"))
                {
                    int align = pShdr.sh_addralign;
                    if (align > 1)
                    {
                        if ((arbitaryLoadAddr.Linear % align) != 0)
                            arbitaryLoadAddr += (int)(align - (arbitaryLoadAddr.Linear % align));
                    }
                    sect.uNativeAddr = arbitaryLoadAddr.Offset;
                    arbitaryLoadAddr += sect.uSectionSize;
                }
                sect.uType = pShdr.sh_type;
                m_sh_link[i] = pShdr.sh_link;
                m_sh_info[i] = pShdr.sh_info;
                sect.uSectionEntrySize = pShdr.sh_entsize;
                if (sect.uNativeAddr + sect.uSectionSize > next_extern)
                    first_extern = next_extern = sect.uNativeAddr + sect.uSectionSize;
                if ((pShdr.sh_flags & SectionFlags.SHF_WRITE) == 0)
                    sect.IsReadOnly = true;
                // Can't use the SHF_ALLOC bit to determine bss section; the bss section has SHF_ALLOC but also SHT_NOBITS.
                // (But many other sections, such as .comment, also have SHT_NOBITS). So for now, just use the name
                //      if ((elfRead4(&pShdr.sh_flags) & SHF_ALLOC) == 0)
                if (pName == ".bss")
                    sect.IsBss = true;
                if ((pShdr.sh_flags & SectionFlags.SHF_EXECINSTR) != 0)
                {
                    sect.IsCode = true;
                    bGotCode = true; // We've got to a code section
                }
                // Deciding what is data and what is not is actually quite tricky but important.
                // For example, it's crucial to flag the .exception_ranges section as data, otherwise there is a "hole" in the
                // allocation map, that means that there is more than one "delta" from a read-only section to a page, and in the
                // end using -C results in a file that looks OK but when run just says "Killed".
                // So we use the Elf designations; it seems that ALLOC.!EXEC -> data
                // But we don't want sections before the .text section, like .interp, .hash, etc etc. Hence bGotCode.
                // NOTE: this ASSUMES that sections appear in a sensible order in the input binary file:
                // junk, code, rodata, data, bss
                if (bGotCode &&
                    (pShdr.sh_flags & (SectionFlags.SHF_EXECINSTR | SectionFlags.SHF_ALLOC)) == SectionFlags.SHF_ALLOC &&
                       pShdr.sh_type != SectionType.SHT_NOBITS)
                    sect.bData = true;
                
                sect.Dump();
                Debug.WriteLine("");

            } // for each section

            // assign arbitary addresses to .rel.* sections too
            for (i = 0; i < m_iNumSections; i++)
            {
                if (m_pSections[i].uNativeAddr == 0 && m_pSections[i].pSectionName.StartsWith(".rel"))
                {
                    m_pSections[i].uNativeAddr = arbitaryLoadAddr.Offset;
                    arbitaryLoadAddr += m_pSections[i].uSectionSize;
                }
            }

            // Add symbol info. Note that some symbols will be in the main table only, and others in the dynamic table only.
            // So the best idea is to add symbols for all sections of the appropriate type
            for (i = 1; i < m_iNumSections; ++i)
            {
                var uType = m_pSections[i].uType;
                if (uType == SectionType.SHT_SYMTAB || uType == SectionType.SHT_DYNSYM)
                    AddSyms(i);
            }

            // Save the relocation to symbol table info
            SectionInfo pRel = GetSectionInfoByName(".rela.text");
            if (pRel != null)
            {
                m_bAddend = true; // Remember its a relA table
                m_pReloc =   (Elf32_Rel*)pRel.uHostAddr; // Save pointer to reloc table
            }
            else
            {
                m_bAddend = false;
                pRel = GetSectionInfoByName(".rel.text");
                if (pRel != null)
                {
                    SetRelocInfo(pRel);
                    m_pReloc = (Elf32_Rel*)pRel.uHostAddr; // Save pointer to reloc table
                }
            }

            // Find the PLT limits. Required for IsDynamicLinkedProc(), e.g.
            SectionInfo pPlt = GetSectionInfoByName(".plt");
            if (pPlt != null)
            {
                m_uPltMin = pPlt.uNativeAddr;
                m_uPltMax = pPlt.uNativeAddr + pPlt.uSectionSize;
            }
            return new ProgramImage(addrLoad, new byte[arbitaryLoadAddr - addrLoad]);
        }


        private Elf32_Shdr[] LoadSectionHeaders(int count, uint imageOffset)
        {
            var rdr = CreateImageReader(imageOffset);
            var headers =  new Elf32_Shdr[count];
            for (int i = 0; i < count; ++i)
            {
                Elf32_Shdr pShdr = new Elf32_Shdr();
                pShdr.sh_name = rdr.ReadUInt32();
                pShdr.sh_type = (SectionType) rdr.ReadUInt32();
                pShdr.sh_flags = (SectionFlags) rdr.ReadUInt32();
                pShdr.sh_addr = rdr.ReadUInt32();
                pShdr.sh_offset = rdr.ReadUInt32();
                pShdr.sh_size = rdr.ReadUInt32();
                pShdr.sh_link = rdr.ReadInt32();
                pShdr.sh_info = rdr.ReadInt32();
                pShdr.sh_addralign = rdr.ReadInt32();
                pShdr.sh_entsize = rdr.ReadUInt32();
                headers[i] = pShdr;
                //Debug.Print("Section {0}", i);
                //Dump("sh_name: {0}", pShdr.sh_name);
                //Dump("sh_type: {0}", pShdr.sh_type);
                //Dump("sh_flags: {0}", pShdr.sh_flags);
                //Dump("sh_addr: {0:X}", pShdr.sh_addr);
                //Dump("sh_offset: {0}", pShdr.sh_offset);
                //Dump("sh_size: {0}", pShdr.sh_size);
                //Dump("sh_link: {0}", pShdr.sh_link);
                //Dump("sh_info: {0}", pShdr.sh_info);
                //Dump("sh_addralign: {0}", pShdr.sh_addralign);
                //Dump("sh_entsize: {0}", pShdr.sh_entsize);
            }
            return headers;
        }

        enum ProgramHeaderType
        {
            PT_NULL = 0,
            PT_LOAD = 1,
            PT_DYNAMIC = 2,
            PT_INTERP = 3,
            PT_NOTE = 4,
            PT_SHLIB = 5,
            PT_PHDR = 6,
            PT_TLS = 7,
            PT_LOOS = 0x60000000,
            PT_HIOS = 0x6fffffff,
            PT_LOPROC = 0x70000000,
            PT_HIPROC = 0x7fffffff,
        }

        private Elf32_Phdr [] LoadProgramHeaders(int headerCount, uint imageOffset)
        {
            var rdr = CreateImageReader(imageOffset);
            var headers = new Elf32_Phdr[headerCount];
            for (int i = 0; i < headerCount; ++i)
            {
                var phdr = new Elf32_Phdr();
                phdr.p_type = rdr.ReadUInt32(); /* entry Type */
                phdr.p_offset = rdr.ReadUInt32(); /* file offset */
                phdr.p_vaddr = rdr.ReadUInt32(); /* virtual address */
                phdr.p_paddr = rdr.ReadUInt32(); /* physical address */
                phdr.p_filesz = rdr.ReadUInt32(); /* file size */
                phdr.p_memsz = rdr.ReadUInt32(); /* memory size */
                phdr.p_flags = rdr.ReadUInt32(); /* entry flags */
                phdr.p_align = rdr.ReadUInt32(); /* memory/file alignment */
                headers[i] = phdr;

                Debug.Print("Program header {0}", i);
                Dump("p_type: {0}", (ProgramHeaderType) phdr.p_type);
                Dump("p_offset: {0:X}", phdr.p_offset);
                Dump("p_vaddr: {0:X}", phdr.p_vaddr);
                Dump("p_paddr: {0:X}", phdr.p_paddr);
                Dump("p_filesz: {0:X}", phdr.p_filesz);
                Dump("p_memsz: {0:X}", phdr.p_memsz);
                Dump("p_flags: {0}", phdr.p_flags);
                Dump("p_align: {0}", phdr.p_align);
            }
            return headers;
        }

        [Conditional("DEBUG")]
        public void Dump(string caption, object value)
        {
            Debug.Print(caption, value);
        }

        private SectionInfo GetSectionInfoByName(string name)
        {
            return m_pSections.FirstOrDefault(sec => sec.pSectionName == name);
        }

        // Like a replacement for elf_strptr()
 

        // Search the .rel[a].plt section for an entry with symbol table index i.
        // If found, return the native address of the associated PLT entry.
        // A linear search will be needed. However, starting at offset i and searching backwards with wraparound should
        // typically minimise the number of entries to search
        ADDRESS findRelPltOffset(uint i, ADDRESS addrRelPlt, uint sizeRelPlt, uint numRelPlt, ADDRESS addrPlt)
        {
            uint first = i;
            if (first >= numRelPlt)
                first = numRelPlt - 1;
            uint curr = first;
            do
            {
                // Each entry is sizeRelPlt bytes, and will contain the offset, then the info (addend optionally follows)
                var pEntry = CreateImageReader(addrRelPlt + (curr * sizeRelPlt));
                pEntry.ReadInt32();
                int entry = pEntry.ReadInt32(); // Read pEntry[1]
                int sym = entry >> 8; // The symbol index is in the top 24 bits (Elf32 only)
                if (sym == i)
                {
                    // Found! Now we want the native address of the associated PLT entry.
                    // For now, assume a size of 0x10 for each PLT entry, and assume that each entry in the .rel.plt section
                    // corresponds exactly to an entry in the .plt (except there is one dummy .plt entry)
                    return addrPlt + 0x10u * (curr + 1);
                }
                if (--curr < 0)
                    curr = numRelPlt - 1;
            } while (curr != first); // Will eventually wrap around to first if not present
            return 0; // Exit if this happens
        }

 

        List<ADDRESS> GetExportedAddresses(bool funcsOnly)
        {
            List<ADDRESS> exported = new List<ADDRESS>();
#if NYI
            int i;
            int secIndex = 0;
            for (i = 1; i < m_iNumSections; ++i)
            {
                uint uType = m_pSections[i].uType;
                if (uType == SHT_SYMTAB)
                {
                    secIndex = i;
                    break;
                }
            }
            if (secIndex == 0)
                return exported;

            int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
            SectionInfo pSect = &m_pSections[secIndex];
            // Calc number of symbols
            int nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            m_pSym = (Elf32_Sym*)pSect.uHostAddr; // Pointer to symbols
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

            // Index 0 is a dummy entry
            for (int i = 1; i < nSyms; i++)
            {
                ADDRESS val = (ADDRESS)elfRead4((int*)&m_pSym[i].st_value);
                int name = elfRead4(&m_pSym[i].st_name);
                if (name == 0) /* Silly symbols with no names */ continue;
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str.Remove(pos);
                if (ELF32_ST_BIND(m_pSym[i].st_info) == STB_GLOBAL || ELF32_ST_BIND(m_pSym[i].st_info) == STB_WEAK)
                {
                    if (funcsOnly == false || ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FUNC)
                    {
                        if (e_type == E_REL)
                        {
                            int nsec = elfRead2(&m_pSym[i].st_shndx);
                            if (nsec >= 0 && nsec < m_iNumSections)
                                val += GetSectionInfo(nsec)->uNativeAddr;
                        }
                        exported.push_back(val);
                    }
                }
            }
#endif
            return exported;

        }


        // FIXME: this function is way off the rails. It seems to always overwrite the relocation entry with the 32 bit value
        // from the symbol table. Totally invalid for SPARC, and most X86 relocations!
        // So currently not called
        private void AddRelocsAsSyms(int relSecIdx)
        {
#if NYI
            SectionInfo pSect = m_pSections[relSecIdx];
            if (pSect == null) return;
            // Calc number of relocations
            int nRelocs = pSect.uSectionSize / pSect.uSectionEntrySize;
            m_pReloc = (Elf32_Rel)pSect.uHostAddr; // Pointer to symbols
            int symSecIdx = m_sh_link[relSecIdx];
            int strSecIdx = m_sh_link[symSecIdx];

            // Index 0 is a dummy entry
            for (int i = 1; i < nRelocs; i++)
            {
                ADDRESS val = (ADDRESS)elfRead4((int*)&m_pReloc[i].r_offset);
                int symIndex = elfRead4(&m_pReloc[i].r_info) >> 8;
                int flags = elfRead4(&m_pReloc[i].r_info);
                if ((flags & 0xFF) == R_386_32)
                {
                    // Lookup the value of the symbol table entry
                    ADDRESS a = elfRead4((int*)&m_pSym[symIndex].st_value);
                    if (m_pSym[symIndex].st_info & STT_SECTION)
                        a = GetSectionInfo(elfRead2(&m_pSym[symIndex].st_shndx))->uNativeAddr;
                    // Overwrite the relocation value... ?
                    writeNative4(val, a);
                    continue;
                }
                if ((flags & R_386_PC32) == 0)
                    continue;
                if (symIndex == 0) /* Silly symbols with no names */ continue;
                string str = GetStrPtr(strSecIdx, elfRead4(&m_pSym[symIndex].st_name));
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                uint pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                // Linear search!
                foreach (var it in m_SymTab)
                    if (it.Value == str)
                        break;
                if (!m_SymTab.ContainsValue(str))
                {
                // Add new extern
                    m_SymTab[next_extern] = str;
                    it = m_SymTab.find(next_extern);
                    next_extern += 4;
                }
                writeNative4(val, (*it).first - val - 4);
            }
            return;
#endif
        }

        // Note: this function overrides a simple "return 0" function in the base class (i.e. BinaryFile::SymbolByAddress())
        string SymbolByAddress(ADDRESS dwAddr)
        {
            string sym;
            if (m_SymTab.TryGetValue(dwAddr, out sym))
                return sym;
            return null;
        }

        bool ValueByName(string pName, ref SymValue pVal, bool bNoTypeOK /* = false */)
        {
            throw new NotImplementedException();
#if NYI
            int hash, numBucket, numChain, y;
            int* pBuckets;
            int* pChains; // For symbol table work
            int found;
            int [] pHash; // Pointer to hash table
            Elf32_Sym* pSym; // Pointer to the symbol table
            int iStr; // Section index of the string table
            PSectionInfo pSect;

            pSect = GetSectionInfoByName(".dynsym");
            if (pSect == 0)
            {
                // We have a file with no .dynsym section, and hence no .hash section (from my understanding - MVE).
                // It seems that the only alternative is to linearly search the symbol tables.
                // This must be one of the big reasons that linking is so slow! (at least, for statically linked files)
                // Note MVE: We can't use m_SymTab because we may need the size
                return SearchValueByName(pName, pVal);
            }
            pSym = (Elf32_Sym)pSect.uHostAddr;
            if (pSym == null) return false;
            pSect = GetSectionInfoByName(".hash");
            if (pSect == 0) return false;
            pHash = (int[])pSect.uHostAddr;
            iStr = GetSectionIndexByName(".dynstr");

            // First organise the hash table
            numBucket = elfRead4(&pHash[0]);
            numChain = elfRead4(&pHash[1]);
            pBuckets = &pHash[2];
            pChains = &pBuckets[numBucket];

            // Hash the symbol
            hash = elf_hash(pName) % numBucket;
            y = elfRead4(&pBuckets[hash]); // Look it up in the bucket list
            // Beware of symbol tables with 0 in the buckets, e.g. libstdc++.
            // In that case, set found to false.
            found = (y != 0);
            if (y)
            {
                while (strcmp(pName, GetStrPtr(iStr, elfRead4(&pSym[y].st_name))) != 0)
                {
                    y = elfRead4(&pChains[y]);
                    if (y == 0)
                    {
                        found = false;
                        break;
                    }
                }
            }
            // Beware of symbols with STT_NOTYPE, e.g. "open" in libstdc++ !
            // But sometimes "main" has the STT_NOTYPE attribute, so if bNoTypeOK is passed as true, return true
            if (found && (bNoTypeOK || (ELF32_ST_TYPE(pSym[y].st_info) != STT_NOTYPE)))
            {
                pVal.uSymAddr = elfRead4((int*)&pSym[y].st_value);
                int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
                if (e_type == E_REL)
                {
                    int nsec = elfRead2(&pSym[y].st_shndx);
                    if (nsec >= 0 && nsec < m_iNumSections)
                        pVal.uSymAddr += GetSectionInfo(nsec)->uNativeAddr;
                }
                pVal.iSymSize = elfRead4(&pSym[y].st_size);
                return true;
            }
            else
            {
                // We may as well do a linear search of the main symbol table. Some symbols (e.g. init_dummy) are
                // in the main symbol table, but not in the hash table
                return SearchValueByName(pName, pVal);
            }
#endif
        }

        // Lookup the symbol table using linear searching. See comments above for why this appears to be needed.
        bool SearchValueByName(string pName, SymValue pVal, string pSectName, string pStrName)
        {
#if NYI
            // Note: this assumes .symtab. Many files don't have this section!!!
            SectionInfo pSect, pStrSect;

            pSect = GetSectionInfoByName(pSectName);
            if (pSect == 0) return false;
            pStrSect = GetSectionInfoByName(pStrName);
            if (pStrSect == 0) return false;
            string pStr = (string)pStrSect.uHostAddr;
            // Find number of symbols
            int n = pSect.uSectionSize / pSect.uSectionEntrySize;
            Elf32_Sym* pSym = (Elf32_Sym*)pSect.uHostAddr;
            // Search all the symbols. It may be possible to start later than index 0
            for (int i = 0; i < n; i++)
            {
                int idx = elfRead4(&pSym[i].st_name);
                if (strcmp(pName, pStr + idx) == 0)
                {
                    // We have found the symbol
                    pVal.uSymAddr = elfRead4((int*)&pSym[i].st_value);
                    int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
                    if (e_type == E_REL)
                    {
                        int nsec = elfRead2(&pSym[i].st_shndx);
                        if (nsec >= 0 && nsec < m_iNumSections)
                            pVal.uSymAddr += GetSectionInfo(nsec)->uNativeAddr;
                    }
                    pVal.iSymSize = elfRead4(&pSym[i].st_size);
                    return true;
                }
            }
#endif
            return false; // Not found (this table)
        }

        // Search for the given symbol. First search .symtab (if present); if not found or the table has been stripped,
        // search .dynstr

        bool SearchValueByName(string pName, SymValue pVal)
        {
            if (SearchValueByName(pName, pVal, ".symtab", ".strtab"))
                return true;
            return SearchValueByName(pName, pVal, ".dynsym", ".dynstr");
        }

        ADDRESS GetAddressByName(string pName,
            bool bNoTypeOK /* = false */)
        {
            var Val = new SymValue ();
            bool bSuccess = ValueByName(pName, ref Val, bNoTypeOK);
            if (bSuccess)
            {
                m_iLastSize = Val.iSymSize;
                m_uLastAddr = Val.uSymAddr;
                return Val.uSymAddr;
            }
            else return NO_ADDRESS;
        }

        int GetSizeByName(string pName, bool bNoTypeOK /* = false */)
        {
            SymValue Val = new SymValue();
            bool bSuccess = ValueByName(pName, ref Val, bNoTypeOK);
            if (bSuccess)
            {
                m_iLastSize = Val.iSymSize;
                m_uLastAddr = Val.uSymAddr;
                return Val.iSymSize;
            }
            else return 0;
        }

        // Guess the size of a function by finding the next symbol after it, and subtracting the distance.
        // This function is NOT efficient; it has to compare the closeness of ALL symbols in the symbol table
        int GetDistanceByName(string sName, string pSectName)
        {
#if NYI
            int size = GetSizeByName(sName);
            if (size) return size; // No need to guess!
            // No need to guess, but if there are fillers, then subtracting labels will give a better answer for coverage
            // purposes. For example, switch_cc. But some programs (e.g. switch_ps) have the switch tables between the
            // end of _start and main! So we are better off overall not trying to guess the size of _start
            uint value = GetAddressByName(sName);
            if (value == 0) return 0; // Symbol doesn't even exist!

            SectionInfo pSect;
            pSect = GetSectionInfoByName(pSectName);
            if (pSect == 0) return 0;
            // Find number of symbols
            int n = pSect.uSectionSize / pSect.uSectionEntrySize;
            Elf32_Sym* pSym = (Elf32_Sym*)pSect.uHostAddr;
            // Search all the symbols. It may be possible to start later than index 0
            uint closest = 0xFFFFFFFF;
            int idx = -1;
            for (int i = 0; i < n; i++)
            {
                if ((pSym[i].st_value > value) && (pSym[i].st_value < closest))
                {
                    idx = i;
                    closest = pSym[i].st_value;
                }
            }
            if (idx == -1) return 0;
            // Do some checks on the symbol's value; it might be at the end of the .text section
            pSect = GetSectionInfoByName(".text");
            ADDRESS low = pSect.uNativeAddr;
            ADDRESS hi = low + pSect.uSectionSize;
            if ((value >= low) && (value < hi))
            {
                // Our symbol is in the .text section. Put a ceiling of the end of the section on closest.
                if (closest > hi) closest = hi;
            }
            return closest - value;
#endif
            return 0;
        }

        int GetDistanceByName(string sName)
        {
            int val = GetDistanceByName(sName, ".symtab");
            if (val != 0) return val;
            return GetDistanceByName(sName, ".dynsym");
        }

        bool IsDynamicLinkedProc(ADDRESS uNative)
        {
            if (uNative > unchecked((uint)-1024) && uNative != ~0U)
                return true; // Say yes for fake library functions
            if (first_extern <= uNative && uNative < next_extern)
                return true; // Yes for externs (not currently used)
            if (m_uPltMin == 0) return false;
            return (uNative >= m_uPltMin) && (uNative < m_uPltMax); // Yes if a call to the PLT (false otherwise)
        }


        //
        // GetEntryPoints()
        // Returns a list of pointers to SectionInfo structs representing entry points to the program
        // Item 0 is the main() function; items 1 and 2 are .init and .fini
        //

        List<SectionInfo> GetEntryPoints(string pEntry /* = "main" */)
        {
            SectionInfo pSect = GetSectionInfoByName(".text");
            ADDRESS uMain = GetAddressByName(pEntry, true);
            ADDRESS delta = uMain - pSect.uNativeAddr;
            pSect.uNativeAddr += delta;
            pSect.uHostAddr += delta;
            // Adjust uSectionSize so uNativeAddr + uSectionSize still is end of sect
            pSect.uSectionSize -= delta;
            m_EntryPoint.Add(pSect);
            // .init and .fini sections
            pSect = GetSectionInfoByName(".init");
            m_EntryPoint.Add(pSect);
            pSect = GetSectionInfoByName(".fini");
            m_EntryPoint.Add(pSect);
            return m_EntryPoint;
        }


        //
        // GetMainEntryPoint()
        // Returns the entry point to main (this should be a label in elf binaries generated by compilers).
        //

        ADDRESS GetMainEntryPoint()
        {
            return GetAddressByName("main", true);
        }

        ADDRESS GetEntryPoint()
        {
            return pHeader.e_entry;
        }

        // FIXME: the below assumes a fixed delta
        ADDRESS NativeToHostAddress(ADDRESS uNative)
        {
            if (m_iNumSections == 0) return 0;
            return m_pSections[1].uHostAddr - m_pSections[1].uNativeAddr + uNative;
        }

        ADDRESS GetRelocatedAddress(ADDRESS uNative)
        {
            // Not implemented yet. But we need the function to make it all link
            return 0;
        }

        IProcessorArchitecture GetProcessorArchitecture()
        {
            switch ((MachineType) pHeader.e_machine)
            {
            case MachineType.EM_386: return new IntelArchitecture(ProcessorMode.ProtectedFlat);
            case MachineType.EM_68K: return new M68kArchitecture();
            case MachineType.EM_SPARC:
            case MachineType.EM_SPARC32PLUS: return new SparcArchitecture();
            case MachineType.EM_PA_RISC:
            case MachineType.EM_PPC:
            case MachineType.EM_MIPS:
            case MachineType.EM_X86_64:
                throw new NotSupportedException(string.Format("The machine {0} is not supported yet.", (MachineType)pHeader.e_machine));
            }
            // An unknown machine type
            throw new NotSupportedException(string.Format("The machine with ELF machine ID {0:X} is not supported.", pHeader.e_machine));
        }

        enum OsAbi
        {
            ELFOSABI_NONE = 0, // No extensions or unspecified 
            ELFOSABI_HPUX = 1, // Hewlett-Packard HP-UX 
            ELFOSABI_NETBSD = 2, // NetBSD 
            ELFOSABI_GNU = 3, // GNU 
            ELFOSABI_LINUX = 3, // Linux  historical - alias for ELFOSABI_GNU 
            ELFOSABI_SOLARIS = 6, // Sun Solaris 
            ELFOSABI_AIX = 7, // AIX 
            ELFOSABI_IRIX = 8, // IRIX 
            ELFOSABI_FREEBSD = 9, // FreeBSD 
            ELFOSABI_TRU64 = 10, // Compaq TRU64 UNIX 
            ELFOSABI_MODESTO = 11, // Novell Modesto 
            ELFOSABI_OPENBSD = 12, // Open BSD 
            ELFOSABI_OPENVMS = 13, // Open VMS 
            ELFOSABI_NSK = 14, // Hewlett-Packard Non-Stop Kernel 
            ELFOSABI_AROS = 15, // Amiga Research OS 
            ELFOSABI_FENIXOS = 16, // The FenixOS highly scalable multi-core OS 
        }

        private Platform GetPlatform()
        {
            switch (pHeader.osAbi)
            {
            default:
                Console.Error.WriteLine("Unsupported ABI: {0}", (OsAbi)pHeader.osAbi);
                return new DefaultPlatform();
            }
        }

        bool isLibrary()
        {
            int type = pHeader.e_type;
            return (type == ET_DYN);
        }



 

        /*==============================================================================
         * FUNCTION:	ElfBinaryFile::GetDynamicGlobalMap
         * OVERVIEW:	Get a map from ADDRESS to string . This map contains the native addresses
         *					and symbolic names of global data items (if any) which are shared with dynamically
         *					linked libraries.
         *					Example: __iob (basis for stdout). The ADDRESS is the native address of a pointer
         *					to the real dynamic data object.
         * NOTE:		The caller should delete the returned map.
         * PARAMETERS:	None
         * RETURNS:		Pointer to a new map with the info, or 0 if none
         *============================================================================*/
        Dictionary<ADDRESS, string> GetDynamicGlobalMap()
        {
            var ret = new Dictionary<ADDRESS, string>();
#if NYI
            SectionInfo* pSect = GetSectionInfoByName(".rel.bss");
            if (pSect == 0)
                pSect = GetSectionInfoByName(".rela.bss");
            if (pSect == 0)
            {
                // This could easily mean that this file has no dynamic globals, and
                // that is fine.
                return ret;
            }
            int numEnt = pSect.uSectionSize / pSect.uSectionEntrySize;
            SectionInfo sym = GetSectionInfoByName(".dynsym");
            if (sym == 0)
            {
                Console.WriteLine("Could not find section .dynsym in source binary file");
                return ret;
            }
            Elf32_Sym pSym = (Elf32_Sym*)sym.uHostAddr;
            int idxStr = GetSectionIndexByName(".dynstr");
            if (idxStr == -1)
            {
                Console.WriteLine("Could not find section .dynstr in source binary file");
                return ret;
            }

            uint p = pSect.uHostAddr;
            for (int i = 0; i < numEnt; i++)
            {
                // The ugly p[1] below is because it p might point to an Elf32_Rela struct, or an Elf32_Rel struct
                int sym = ELF32_R_SYM(((int*)p)[1]);
                int name = pSym[sym].st_name; // Index into string table
                string s = GetStrPtr(idxStr, name);
                ADDRESS val = ((int*)p)[0];
                (*ret)[val] = s; // Add the (val, s) mapping to ret
                p += pSect.uSectionEntrySize;
            }
#endif
            return ret;
        }


        bool IsRelocationAt(ADDRESS uNative)
        {
#if NYI
            //int nextFakeLibAddr = -2;			// See R_386_PC32 below; -1 sometimes used for main
            if (m_pImage == 0) return false; // No file loaded
            int machine = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_machine);
            int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
            switch (machine)
            {
            case EM_SPARC:
                break; // Not implemented yet
            case EM_386:
                {
                    for (int i = 1; i < m_iNumSections; ++i)
                    {
                        SectionInfo ps = &m_pSections[i];
                        if (ps.uType == SHT_REL)
                        {
                            // A section such as .rel.dyn or .rel.plt (without an addend field).
                            // Each entry has 2 words: r_offet and r_info. The r_offset is just the offset from the beginning
                            // of the section (section given by the section header's sh_info) to the word to be modified.
                            // r_info has the type in the bottom byte, and a symbol table index in the top 3 bytes.
                            // A symbol table offset of 0 (STN_UNDEF) means use value 0. The symbol table involved comes from
                            // the section header's sh_link field.
                            int* pReloc = (int*)ps.uHostAddr;
                            uint size = ps.uSectionSize;
                            // NOTE: the r_offset is different for .o files (E_REL in the e_type header field) than for exe's
                            // and shared objects!
                            ADDRESS destNatOrigin = 0, destHostOrigin;
                            if (e_type == E_REL)
                            {
                                int destSection = m_sh_info[i];
                                destNatOrigin = m_pSections[destSection].uNativeAddr;
                                destHostOrigin = m_pSections[destSection].uHostAddr;
                            }
                            //int symSection = m_sh_link[i];			// Section index for the associated symbol table
                            //int strSection = m_sh_link[symSection];	// Section index for the string section assoc with this
                            //string pStrSection = (char*)m_pSections[strSection].uHostAddr;
                            //Elf32_Sym* symOrigin = (Elf32_Sym*) m_pSections[symSection].uHostAddr;
                            for (uint u = 0; u < size; u += 2 * sizeof(uint))
                            {
                                uint r_offset = elfRead4(pReloc++);
                                //unsigned info	= elfRead4(pReloc);
                                pReloc++;
                                //byte relType = (byte) info;
                                //unsigned symTabIndex = info >> 8;
                                ADDRESS pRelWord; // Pointer to the word to be relocated
                                if (e_type == E_REL)
                                    pRelWord = destNatOrigin + r_offset;
                                else
                                {
                                    if (r_offset == 0) continue;
                                    SectionInfo destSec = GetSectionInfoByAddr(r_offset);
                                    pRelWord = destSec.uNativeAddr + r_offset;
                                    destNatOrigin = 0;
                                }
                                if (uNative == pRelWord)
                                    return true;
                            }
                        }
                    }
                }
            default:
                break; // Not implemented
            }
#endif
            return false;
        }

        string getFilenameSymbolFor(string sym)
        {
#if NYI
            int i;
            int secIndex = 0;
            for (i = 1; i < m_iNumSections; ++i)
            {
                uint uType = m_pSections[i].uType;
                if (uType == SHT_SYMTAB)
                {
                    secIndex = i;
                    break;
                }
            }
            if (secIndex == 0)
                return null;

            //int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
            SectionInfo pSect = m_pSections[secIndex];
            // Calc number of symbols
            int nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            m_pSym = (Elf32_Sym*)pSect.uHostAddr; // Pointer to symbols
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

            string filename;

            // Index 0 is a dummy entry
            for (int i = 1; i < nSyms; i++)
            {
                //ADDRESS val = (ADDRESS) elfRead4((int*)&m_pSym[i].st_value);
                int name = elfRead4(&m_pSym[i].st_name);
                if (name == 0) /* Silly symbols with no names */ continue;
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                if (ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FILE)
                {
                    filename = str;
                    continue;
                }
                if (str == sym)
                {
                    if (!string.IsNullOrEmpty(filename))
                        return filename;
                    return null;
                }
            }
#endif
            return null;
        }

        private void getFunctionSymbols(SortedList<string, SortedList<ADDRESS, string>> syms_in_file) {
#if NYI
    int i;
    int secIndex = 0;
    for (i = 1; i < m_iNumSections; ++i) {
        uint uType = m_pSections[i].uType;
        if (uType == SHT_SYMTAB) {
            secIndex = i;
            break;
        }
    }
    if (secIndex == 0) {
        Console.Error.WriteLine("no symtab section? Assuming stripped, looking for dynsym.\n");

        for (i = 1; i < m_iNumSections; ++i) {
            uint uType = m_pSections[i].uType;
            if (uType == SHT_DYNSYM) {
                secIndex = i;
                break;
            }
        }

        if (secIndex == 0) {
            Console.Error.WriteLine("no dynsyms either.. guess we're out of luck.\n");
            return;
        }
    }

    int e_type = pHeader.e_type;
    SectionInfo pSect = m_pSections[secIndex];
    // Calc number of symbols
    int nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
    m_pSym = (Elf32_Sym*) pSect.uHostAddr; // Pointer to symbols
    int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

    string filename = "unknown.c";

    // Index 0 is a dummy entry
    for (int i = 1; i < nSyms; i++) {
        int name = elfRead4(&m_pSym[i].st_name);
        if (name == 0) /* Silly symbols with no names */ continue;
        string str = GetStrPtr(strIdx, name);
        // Hack off the "@@GLIBC_2.0" of Linux, if present
        uint pos;
        if ((pos = str.IndexOf("@@"))>= 0)
            str.erase(pos);
        if (ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FILE) {
            filename = str;
            continue;
        }
        if (ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FUNC) {
            ADDRESS val = (ADDRESS) elfRead4((int*) & m_pSym[i].st_value);
            if (e_type == E_REL) {
                int nsec = elfRead2(&m_pSym[i].st_shndx);
                if (nsec >= 0 && nsec < m_iNumSections)
                    val += GetSectionInfo(nsec)->uNativeAddr;
            }
            if (val == 0) {
                // ignore plt for now
            } else {
                syms_in_file[filename][val] = str;
            }
        }
    }
#endif
        }


        private void dumpSymbols()
        {
            foreach (var de in m_SymTab)
                Console.Error.WriteLine("0x{0:X} {1}        ", de.Key, de.Value);
        }

        struct SymValue
        {
            public ADDRESS uSymAddr; // Symbol native address
            public int iSymSize; // Size associated with symbol
        }

        // Internal elf info




        enum MachineType
        {
            EM_M32 = 1,
            EM_SPARC = 2,			// Sun SPARC
            EM_386 = 3,			// Intel 80386 or higher
            EM_68K = 4,			// Motorola 68000
            EM_MIPS = 8,			// MIPS
            EM_PA_RISC = 15,			// HP PA-RISC
            EM_SPARC32PLUS = 18,			// Sun SPARC 32+
            EM_PPC = 20,			// PowerPC
            EM_X86_64 = 62,
            EM_ST20 = 0xA8,			// ST20 (made up... there is no official value?)
        }

        public enum SectionType
        {
            SHT_NULL = 0,
            SHT_PROGBITS = 1,
            SHT_SYMTAB = 2,
            SHT_STRTAB = 3,
            SHT_RELA = 4,
            SHT_HASH = 5,
            SHT_DYNAMIC = 6,
            SHT_NOTE = 7,
            SHT_NOBITS = 8,
            SHT_REL = 9,
            SHT_SHLIB = 10,
            SHT_DYNSYM = 11,
            SHT_INIT_ARRAY = 14,
            SHT_FINI_ARRAY = 15,
            SHT_PREINIT_ARRAY = 16,
            SHT_GROUP = 17,
            SHT_SYMTAB_SHNDX = 18,
        }

        [Flags]
        public enum SectionFlags : uint
        {
            SHF_WRITE = 0x1,
            SHF_ALLOC = 0x2,
            SHF_EXECINSTR = 0x4,
            SHF_MERGE = 0x10,
            SHF_STRINGS = 0x20,
            SHF_INFO_LINK = 0x40,
            SHF_LINK_ORDER = 0x80,
            SHF_OS_NONCONFORMING = 0x100,
            SHF_GROUP = 0x200,
            SHF_TLS = 0x400,
            SHF_MASKOS = 0x0ff00000,
            SHF_MASKPROC = 0xf0000000u,
        }

        const int ET_NONE = 0;// No file type
        const int ET_REL = 1;// Relocatable file
        const int ET_EXEC = 2;// Executable file
        const int ET_DYN = 3; // Shared object file
        const int ET_CORE = 4; // Core dump

        const int R_386_32 = 1;
        const int R_386_PC32 = 2;

        // Program header

        public class Elf32_Phdr
        {
            public uint p_type; /* entry Type */
            public uint p_offset; /* file offset */
            public uint p_vaddr; /* virtual address */
            public uint p_paddr; /* physical address */
            public uint p_filesz; /* file size */
            public uint p_memsz; /* memory size */
            public uint p_flags; /* entry flags */
            public uint p_align; /* memory/file alignment */
        }

        // Section header

        public class Elf32_Shdr
        {
            public uint sh_name;
            public SectionType sh_type;
            public SectionFlags sh_flags;
            public uint sh_addr;
            public uint sh_offset;
            public uint sh_size;
            public int sh_link;
            public int sh_info;
            public int sh_addralign;
            public uint sh_entsize;
        }

        const int SHF_WRITE = 1;		// Writeable
        const int SHF_ALLOC = 2;		// Consumes memory in exe
        const int SHF_EXECINSTR = 4;		// Executable

        const int SHT_NOBITS = 8;		// Bss
        const int SHT_REL = 9;		// Relocation table (no addend)
        const int SHT_RELA = 4;		// Relocation table (with addend, e.g. RISC)
        const int SHT_SYMTAB = 2;		// Symbol table
        const int SHT_DYNSYM = 11;		// Dynamic symbol table

        public class Elf32_Sym
        {
            public int st_name;
            public uint st_value;
            public int st_size;
            public byte st_info;
            public byte st_other;
            public short st_shndx;
        }

        public class Elf32_Rel
        {
            public uint r_offset;
            public int r_info;
        }


 


        // Tag values
        const int DT_NULL = 0;		// Last entry in list
        const int DT_STRTAB = 5;		// String table
        const int DT_NEEDED = 1;		// A needed link-type object

        const int E_REL = 1;		// Relocatable file type

        const uint NO_ADDRESS = ~0u;

    }

    public class SymTab
    {
    }
#endif

    }

}
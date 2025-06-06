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

namespace Reko.ImageLoaders.Elf
{
    // https://fossies.org/linux/gdb/include/elf/common.h
    // https://gabi.xinuos.com/elf/a-emachine.html
    public enum ElfMachine : ushort
    {
        EM_NONE = 0,            // No machine 
        EM_M32 = 1,             // AT&T WE 32100 
        EM_SPARC = 2,           // SPARC 
        EM_386 = 3,             // Intel 80386 
        EM_68K = 4,             // Motorola 68000 
        EM_88K = 5,             // Motorola 88000 
        EM_IAMCU = 6,           // Intel MCU
        EM_860 = 7,             // Intel 80860 
        EM_MIPS = 8,            // MIPS I Architecture 
        EM_S370 = 9,            // IBM System/370 Processor 
        EM_MIPS_RS3_LE = 10,    // MIPS RS3000 Little-endian 
        EM_OLD_SPARCV9 = 11,    // Old version of Sparc v9, from before the ABI.  Deprecated.
        //RESERVED 11-14 Reserved for future use 
        EM_PARISC = 15,         // Hewlett-Packard PA-RISC 
                                //RESERVED 16 Reserved for future use 
        EM_VPP500 = 17,         // Fujitsu VPP500 
        EM_PPC_OLD = 17,        // Old version of PowerPC.  Deprecated.
        EM_SPARC32PLUS = 18,    // Enhanced instruction set SPARC 
        EM_960 = 19,            // Intel 80960 
        EM_PPC = 20,            // PowerPC 
        EM_PPC64 = 21,          // 64-bit PowerPC 
        EM_S390 = 22,           // IBM System/390
        EM_SPU = 23,            // IBM SPU/SPC
                                //RESERVED 24-35 Reserved for future use 
        EM_V800 = 36,           // NEC V800 
        EM_FR20 = 37,           // Fujitsu FR20 
        EM_RH32 = 38,           // TRW RH-32 
        EM_MCORE = 39,          // Motorola MCore. May also be taken by Fujitsu MMA
        EM_RCE = 39,            // Motorola RCE, hijacked by C-Sky
        EM_ARM = 40,            // Advanced RISC Machines ARM 
        EM_ALPHA_STD = 41,      // Digital Alpha
        EM_OLD_ALPHA = 41,      // Digital Alpha
        EM_SH = 42,             // Renesas (formerly Hitachi) SH 
        EM_SPARCV9 = 43,        // SPARC Version 9 
        EM_TRICORE = 44,        // Siemens Tricore embedded processor 
        EM_ARC = 45,            // Argonaut RISC Core, Argonaut Technologies Inc. 
        EM_H8_300 = 46,         // Hitachi H8/300 
        EM_H8_300H = 47,        // Hitachi H8/300H 
        EM_H8S = 48,            // Hitachi H8S 
        EM_H8_500 = 49,         // Hitachi H8/500 
        EM_IA_64 = 50,          // Intel IA-64 processor architecture 
        EM_MIPS_X = 51,         // Stanford MIPS-X 
        EM_COLDFIRE = 52,       // Motorola ColdFire 
        EM_68HC12 = 53,         // Motorola M68HC12 
        EM_MMA = 54,            // Fujitsu MMA Multimedia Accelerator 
        EM_PCP = 55,            // Siemens PCP 
        EM_NCPU = 56,           // Sony nCPU embedded RISC processor 
        EM_NDR1 = 57,           // Denso NDR1 microprocessor 
        EM_STARCORE = 58,       // Motorola Star*Core processor 
        EM_ME16 = 59,           // Toyota ME16 processor 
        EM_ST100 = 60,          // STMicroelectronics ST100 processor 
        EM_TINYJ = 61,          // Advanced Logic Corp. TinyJ embedded processor family 
        EM_X86_64 = 62,         // AMD x86-64 architecture
        EM_PDSP = 63,           // Sony DSP Processor
        EM_PDP10 = 64,          // Digital Equipment Corp. PDP-10
        EM_PDP11 = 65,          // Digital Equipment Corp. PDP-11
        EM_FX66 = 66,           // Siemens FX66 microcontroller 
        EM_ST9PLUS = 67,        // STMicroelectronics ST9+ 8/16 bit microcontroller 
        EM_ST7 = 68,            // STMicroelectronics ST7 8-bit microcontroller 
        EM_68HC16 = 69,         // Motorola MC68HC16 Microcontroller 
        EM_68HC11 = 70,         // Motorola MC68HC11 Microcontroller 
        EM_68HC08 = 71,         // Motorola MC68HC08 Microcontroller 
        EM_68HC05 = 72,         // Motorola MC68HC05 Microcontroller 
        EM_SVX = 73,            // Silicon Graphics SVx 
        EM_ST19 = 74,           // STMicroelectronics ST19 8-bit microcontroller 
        EM_VAX = 75,            // Digital VAX  
        EM_CRIS = 76,           // Axis Communications 32-bit embedded processor 
        EM_JAVELIN = 77,        // Infineon Technologies 32-bit embedded processor 
        EM_FIREPATH = 78,       // Element 14 64-bit DSP Processor 
        EM_ZSP = 79,            // LSI Logic 16-bit DSP Processor 
        EM_MMIX = 80,           // Donald Knuth's educational 64-bit processor 
        EM_HUANY = 81,          // Harvard University machine-independent object files 
        EM_PRISM = 82,          // SiTera Prism 
        EM_AVR = 83,            // Atmel AVR 8-bit microcontroller
        EM_FR30 = 84,           // Fujitsu FR30
        EM_D10V = 85,           // Mitsubishi D10V
        EM_D30V = 86,           // Mitsubishi D30V
        EM_V850 = 87,           // NEC v850
        EM_M32R = 88,           // Mitsubishi M32R
        EM_MN10300 = 89,        // Matsushita MN10300
        EM_MN10200 = 90,        // Matsushita MN10200
        EM_PJ = 91,             // picoJava
        EM_OPENRISC = 92,       // OpenRISC 32-bit embedded processor
        EM_OR1K = 92,           // OpenRISC 1000 32-bit embedded processor
        EM_ARC_COMPACT = 93,    // ARC International ARCompact processor (old
                                // spelling/synonym: EM_ARC_A5)
        EM_XTENSA = 94,         // Tensilica Xtensa Architecture
        EM_VIDEOCORE = 95,      // Alphamosaic VideoCore processor
        EM_TMM_GPP = 96,        // Thompson Multimedia General Purpose Processor
        EM_NS32K = 97,          // National Semiconductor 32000 series
        EM_TPC = 98,            // Tenor Network TPC processor
        EM_SNP1K = 99,          // Trebia SNP 1000 processor
        EM_ST200 = 100,         // STMicroelectronics (www.st.com) ST200
        EM_IP2K = 101,          // Ubicom IP2xxx microcontroller family
        EM_MAX = 102,           // MAX Processor
        EM_CR = 103,            // National Semiconductor CompactRISC microprocessor
        EM_F2MC16 = 104,        // Fujitsu F2MC16
        EM_MSP430 = 105,        // Texas Instruments embedded microcontroller msp430
        EM_BLACKFIN = 106,      // Analog Devices Blackfin (DSP) processor
        EM_SE_C33 = 107,        // S1C33 Family of Seiko Epson processors
        EM_SEP = 108,           // Sharp embedded microprocessor
        EM_ARCA = 109,          // Arca RISC Microprocessor
        EM_UNICORE = 110,       // Microprocessor series from PKU-Unity Ltd. and MPRC
                                // of Peking University
        EM_EXCESS = 111,        // eXcess: 16/32/64-bit configurable embedded CPU
        EM_DXP = 112,           // Icera Semiconductor Inc. Deep Execution Processor
        EM_ALTERA_NIOS2 = 113,  // Altera Nios II soft-core processor
        EM_CRX = 114,           // National Semiconductor CompactRISC CRX
        EM_XGATE = 115,         // Motorola XGATE embedded processor
        EM_C166 = 116,          // Infineon C16x/XC16x processor
        EM_M16C = 117,          // Renesas M16C series microprocessors
        EM_DSPIC30F = 118,      // Microchip Technology dsPIC30F Digital Signal
                                // Controller
        EM_CE = 119,            // Freescale Communication Engine RISC core
        EM_M32C = 120,          // Renesas M32C series microprocessors
        EM_TSK3000 = 131,       // Altium TSK3000 core
        EM_RS08 = 132,          // Freescale RS08 embedded processor
        EM_SHARC = 133,         // Analog Devices SHARC family of 32-bit DSP
                                // processors
        EM_ECOG2 = 134,         // Cyan Technology eCOG2 microprocessor
        EM_SCORE = 135,         // Sunplus Score
        EM_SCORE7 = 135,        // Sunplus S+core7 RISC processor
        EM_DSP24 = 136,         // New Japan Radio (NJR) 24-bit DSP Processor
        EM_VIDEOCORE3 = 137,    // Broadcom VideoCore III processor
        EM_LATTICEMICO32 = 138, // RISC processor for Lattice FPGA architecture
        EM_SE_C17 = 139,        // Seiko Epson C17 family
        EM_TI_C6000 = 140,      // The Texas Instruments TMS320C6000 DSP family
        EM_TI_C2000 = 141,      // The Texas Instruments TMS320C2000 DSP family
        EM_TI_C5500 = 142,      // The Texas Instruments TMS320C55x DSP family
        EM_TI_PRU = 144,        // Texas Instruments Programmable Realtime Unit

        // 145–159 Reserved for future use

        EM_MMDSP_PLUS = 160,    // STMicroelectronics 64bit VLIW Data Signal Processor
        EM_CYPRESS_M8C = 161,   // Cypress M8C microprocessor
        EM_R32C = 162,          // Renesas R32C series microprocessors
        EM_TRIMEDIA = 163,      // NXP Semiconductors TriMedia architecture family
        EM_HEXAGON = 164,       // Qualcomm Hexagon processor
        EM_QDSP6 = 164,         // QUALCOMM DSP6 Processor
        EM_8051 = 165,          // Intel 8051 and variants
        EM_STXP7X = 166,        // STMicroelectronics STxP7x family of configurable
                                // and extensible RISC processors
        EM_NDS32 = 167,         // Andes Technology compact code size embedded RISC
                                // processor family
        EM_ECOG1 = 168,         // Cyan Technology eCOG1X family
        EM_ECOG1X = 168,        // Cyan Technology eCOG1X family
        EM_MAXQ30 = 169,        // Dallas Semiconductor MAXQ30 Core Micro-controllers
        EM_XIMO16 = 170,        // New Japan Radio (NJR) 16-bit DSP Processor
        EM_MANIK = 171,         // M2000 Reconfigurable RISC Microprocessor
        EM_CRAYNV2 = 172,       // Cray Inc. NV2 vector architecture
        EM_RX = 173,            // Renesas RX family
        EM_METAG = 174,         // Imagination Technologies META processor architecture
        EM_MCST_ELBRUS = 175,   // MCST Elbrus general purpose hardware architecture
        EM_ECOG16 = 176,        // Cyan Technology eCOG16 family
        EM_CR16 = 177,          // National Semiconductor CompactRISC CR16 16-bit microprocessor
        EM_ETPU = 178,          // Freescale Extended Time Processing Unit
        EM_SLE9X = 179,         // Infineon Technologies SLE9X core
        EM_L10M = 180,          // Intel L10M
        EM_K10M = 181,          // Intel K10M
        EM_AARCH64 = 183,       // ARM AArch64

        // reserved = 184,         // Reserved for future ARM use

        EM_AVR32 = 185,         // Atmel Corporation 32-bit microprocessor family
        EM_STM8 = 186,          // STMicroeletronics STM8 8-bit microcontroller
        EM_TILE64 = 187,        // Tilera TILE64 multicore architecture family
        EM_TILEPRO = 188,       // Tilera TILEPro multicore architecture family
        EM_MICROBLAZE = 189,    // Xilinx MicroBlaze 32-bit RISC soft processor core
        EM_CUDA = 190,          // NVIDIA CUDA architecture
        EM_TILEGX = 191,        // Tilera TILE-Gx multicore architecture family
        EM_CLOUDSHIELD = 192,   // CloudShield architecture family
        EM_COREA_1ST = 193,     // KIPO-KAIST Core-A 1st generation processor family
        EM_COREA_2ND = 194,     // KIPO-KAIST Core-A 2nd generation processor family
        EM_ARC_COMPACT2 = 195,  // Synopsys ARCompact V2
        EM_OPEN8 = 196,         // Open8 8-bit RISC soft processor core
        EM_RL78 = 197,          // Renesas RL78 family
        EM_VIDEOCORE5 = 198,    // Broadcom VideoCore V processor
        EM_78KOR = 199,         // Renesas 78KOR family
        EM_56800EX = 200,       // Freescale 56800EX Digital Signal Controller (DSC)
        EM_BA1 = 201,           // Beyond BA1 CPU architecture
        EM_BA2 = 202,           // Beyond BA2 CPU architecture
        EM_XCORE = 203,         // XMOS xCORE processor family
        EM_MCHP_PIC = 204,      // Microchip 8-bit PIC(r) family
        EM_INTELGT = 205,       // Intel Graphics Technology
        EM_INTEL206 = 206,      // Reserved by Intel
        EM_INTEL207 = 207,      // Reserved by Intel
        EM_INTEL208 = 208,      // Reserved by Intel
        EM_INTEL209 = 209,      // Reserved by Intel
        EM_KM32 = 210,          // KM211 KM32 32-bit processor
        EM_KMX32 = 211,         // KM211 KMX32 32-bit processor
        EM_KMX16 = 212,         // KM211 KMX16 16-bit processor
        EM_KMX8 = 213,          // KM211 KMX8 8-bit processor
        EM_KVARC = 214,         // KM211 KVARC processor
        EM_CDP = 215,           // Paneve CDP architecture family
        EM_COGE = 216,          // Cognitive Smart Memory Processor
        EM_COOL = 217,          // Bluechip iCelero CoolEngine
        EM_NORC = 218,          // Nanoradio Optimized RISC
        EM_CSR_KALIMBA = 219,   // CSR Kalimba architecture family
        EM_Z80 = 220,           // Zilog Z80
        EM_VISIUM = 221,        // Controls and Data Services VISIUMcore processor
        EM_FT32 = 222,          // FTDI Chip FT32 high performance 32-bit RISC architecture
        EM_MOXIE = 223,         // Moxie processor family
        EM_AMDGPU = 224,        // AMD GPU architecture

        // 225–242 - Reserved for future use

        EM_RISCV = 243,         // RISC-V
        EM_LANAI = 244,         // Lanai 32-bit processor
        EM_CEVA = 245,          // CEVA Processor Architecture Family
        EM_CEVA_X2 = 246,       // CEVA X2 Processor Family
        EM_BPF = 247,           // Linux BPF – in-kernel virtual machine.
        EM_GRAPHCORE_IPU = 248, // Graphcore Intelligent Processing Unit
        EM_IMG1 = 249,          // Imagination Technologies
        EM_NFP = 250,           // Netronome Flow Processor.
        EM_VE = 251,            // NEC SX-Aurora Vector Engine
        EM_CSKY = 252,          // C-SKY processor family.
        EM_ARC_COMPACT3_64 = 253, // Synopsys ARCv2.3 64-bit
        EM_MCS6502 = 254,       // MOS Technology MCS 6502 processor
        EM_ARC_COMPACT3 = 255,  // Synopsys ARCv2.3 32-bit
        EM_KVX = 256,           // Kalray VLIW core of the MPPA processor family
        EM_65816 = 257,         // WDC 65816/65C816
        EM_LOONGARCH = 258,     // LoongArch
        EM_KF32 = 259,          // ChipON KungFu32
        EM_U16_U8CORE = 260,    // LAPIS nX-U16/U8
        EM_TACHYUM = 261,       // Tachyum
        EM_56800EF = 262,       // NXP 56800EF Digital Signal Controller (DSC)
        EM_SBF = 263,           // Solana Bytecode Format
        EM_AIENGINE = 264,      // AMD/Xilinx AIEngine architecture
        EM_SIMA_MLA = 265,      // SiMa MLA
        EM_BANG = 266,          // Cambricon BANG
        EM_LOONGGPU = 267,      // Loongson LoongGPU


        // Unofficial definitions; some manufacturers do not register thier EM_*
        // values officially.

        EM_NANOMIPS = 249,       //$REVIEW: a guess?
        EM_MORPHOS_PPC = 8692,   //$REVIEW: a guess?
        EM_AVR32a = 0x18AD,      // Atmel AVR32, according to https://sites.uclouvain.be/SystInfo/usr/include/linux/elf-em.h.html
        EM_FRV = 0x5441,         // Fujitsu FR-V
        EM_AEON = 0x8466,        // AEON?
        EM_TC32 = 0x8800,        // Telink TC32
        EM_ALPHA = 0x9026,       // Alpha (written in the absence of an ABI) 

        EM_AVR_OLD = 0x1057,        // AVR magic number.  Written in the absense of an ABI.  
        EM_MSP430_OLD = 0x1059,     // MSP430 magic number.  Written in the absense of everything.  
        EM_MT = 0x2530,             // Morpho MT.   Written in the absense of an ABI.  
        EM_CYGNUS_FR30 = 0x3330,    // FR30 magic number - no EABI available.  
        EM_WEBASSEMBLY = 0x4157,    // Unofficial value for Web Assembly binaries, as used by LLVM.  
        EM_S12Z = 0x4DEF,           // Freescale S12Z.   The Freescale toolchain generates elf files with this value.  
        EM_DLX = 0x5AA5,            // DLX magic number.  Written in the absense of an ABI.  
        EM_CYGNUS_FRV = 0x5441,     // FRV magic number - no EABI available??.  
        EM_XC16X = 0x4688,          // Infineon Technologies 16-bit microcontroller with C166-V2 core.  
        EM_CYGNUS_D10V = 0x7650,    // D10V backend magic number.  Written in the absence of an ABI.  
        EM_CYGNUS_D30V = 0x7676,    // D30V backend magic number.  Written in the absence of an ABI.  
        EM_IP2K_OLD = 0x8217,       // Ubicom IP2xxx;   Written in the absense of an ABI.  
        EM_BA = 0x8472,             // Beyond BA 32-bit architecture.
        EM_BA64 = 0x8473,           // Beyond BA 64-bit architecture.
        EM_CYGNUS_POWERPC = 0x9025, // Cygnus PowerPC ELF backend.  Written in the absence of an ABI.  
        EM_CYGNUS_M32R = 0x9041,    // Cygnus M32R ELF backend.  Written in the absence of an ABI.  
        EM_CYGNUS_V850 = 0x9080,    // V850 backend magic number.  Written in the absense of an ABI.  
        EM_S390_OLD = 0xA390,       // old S/390 backend magic number. Written in the absence of an ABI.  
        EM_XTENSA_OLD = 0xABC7,     // Old, unofficial value for Xtensa.  
        EM_XSTORMY16 = 0xAD45,
 
        EM_CYGNUS_MN10300 = 0xBEEF, // mn10200 and mn10300 backend magic numbers.  Written in the absense of an ABI.
        EM_CYGNUS_MN10200 = 0xDEAD, // mn10200 and mn10300 backend magic numbers.  Written in the absense of an ABI.

        EM_M32C_OLD = 0xFEB0,       // Renesas M32C and M16C.  
        EM_IQ2000 = 0xFEBA,         // Vitesse IQ2000.  
        EM_NIOS32 = 0xFEBB,         // NIOS magic number - no EABI available.  
        EM_CYGNUS_MEP = 0xF00D,     // Toshiba MeP
        EM_MOXIE_OLD = 0xFEED,      // Old, unofficial value for Moxie.  
        EM_MICROBLAZE_OLD = 0xBAAB, // Old MicroBlaze
        EM_ADAPTEVA_EPIPHANY = 0x1223, // Adapteva's Epiphany architecture.
 
        /* C-SKY historically used 39, the same value as MCORE, from which the
        architecture was derived.  */
        EM_CSKY_OLD   =  EM_MCORE,
    }
}
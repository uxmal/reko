#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    class Avr32Relocator : ElfRelocator32
    {
        private ElfLoader32 elfLoader;

        public Avr32Relocator(ElfLoader32 elfLoader, SortedList<Address, ImageSymbol> imageSymbols) 
            : base(elfLoader, imageSymbols)

        {
            this.elfLoader = elfLoader;
        }

        public override (Address?, ElfSymbol?) RelocateEntry(Program program, ElfSymbol symbol, ElfSection? referringSection, ElfRelocation rela)
        {
            //$TODO: implement me!
            return (null, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((Avr32Rt)type).ToString();
        }
    }

    public enum Avr32Rt
    {
        R_AVR32_NONE = 0,
        R_AVR32_32 = 1,
        R_AVR32_16 = 2,
        R_AVR32_8 = 3,
        R_AVR32_32_PCREL = 4,
        R_AVR32_16_PCREL = 5,
        R_AVR32_8_PCREL = 6,
        R_AVR32_DIFF32 = 7,
        R_AVR32_DIFF16 = 8,
        R_AVR32_DIFF8 = 9,
        R_AVR32_GOT32 = 10,
        R_AVR32_GOT16 = 11,
        R_AVR32_GOT8 = 12,
        R_AVR32_21S = 13,
        R_AVR32_16U = 14,
        R_AVR32_16S = 15,
        R_AVR32_8S = 16,
        R_AVR32_8S_EXT = 17,
        R_AVR32_22H_PCREL = 18,
        R_AVR32_18W_PCREL = 19,
        R_AVR32_16B_PCREL = 20,
        R_AVR32_16N_PCREL = 21,
        R_AVR32_14UW_PCREL = 22,
        R_AVR32_11H_PCREL = 23,
        R_AVR32_10UW_PCREL = 24,
        R_AVR32_9H_PCREL = 25,
        R_AVR32_9UW_PCREL = 26,
        R_AVR32_HI16 = 27,
        R_AVR32_LO16 = 28,
        R_AVR32_GOTPC = 29,
        R_AVR32_GOTCALL = 30,
        R_AVR32_LDA_GOT = 31,
        R_AVR32_GOT21S = 32,
        R_AVR32_GOT18SW = 33,
        R_AVR32_GOT16S = 34,
        R_AVR32_GOT7UW = 35,
        R_AVR32_32_CPENT = 36,
        R_AVR32_CPCALL = 37,
        R_AVR32_16_CP = 38,
        R_AVR32_9W_CP = 39,
        R_AVR32_RELATIVE = 40,
        R_AVR32_GLOB_DAT = 41,
        R_AVR32_JMP_SLOT = 42,
        R_AVR32_ALIGN = 43,
    }

#if NOPE
    /*

    /* AVR32 relocation numbers */
$define R_AVR32_NONE		0
$define R_AVR32_32		1
$define R_AVR32_16		2
$define R_AVR32_8		3
$define R_AVR32_32_PCREL	4
$define R_AVR32_16_PCREL	5
$define R_AVR32_8_PCREL		6
$define R_AVR32_DIFF32		7
$define R_AVR32_DIFF16		8
$define R_AVR32_DIFF8		9
$define R_AVR32_GOT32		10
$define R_AVR32_GOT16		11
$define R_AVR32_GOT8		12
$define R_AVR32_21S		13
$define R_AVR32_16U		14
$define R_AVR32_16S		15
$define R_AVR32_8S		16
$define R_AVR32_8S_EXT		17
$define R_AVR32_22H_PCREL	18
$define R_AVR32_18W_PCREL	19
$define R_AVR32_16B_PCREL	20
$define R_AVR32_16N_PCREL	21
$define R_AVR32_14UW_PCREL	22
$define R_AVR32_11H_PCREL	23
$define R_AVR32_10UW_PCREL	24
$define R_AVR32_9H_PCREL	25
$define R_AVR32_9UW_PCREL	26
$define R_AVR32_HI16		27
$define R_AVR32_LO16		28
$define R_AVR32_GOTPC		29
$define R_AVR32_GOTCALL		30
$define R_AVR32_LDA_GOT		31
$define R_AVR32_GOT21S		32
$define R_AVR32_GOT18SW		33
$define R_AVR32_GOT16S		34
$define R_AVR32_GOT7UW		35
$define R_AVR32_32_CPENT	36
$define R_AVR32_CPCALL		37
$define R_AVR32_16_CP		38
$define R_AVR32_9W_CP		39
$define R_AVR32_RELATIVE	40
$define R_AVR32_GLOB_DAT	41
$define R_AVR32_JMP_SLOT	42
$define R_AVR32_ALIGN		43

    /*
     * ELF register definitions..
     */


    typedef unsigned long elf_greg_t;

$define ELF_NGREG (sizeof (struct pt_regs) / sizeof (elf_greg_t))
    typedef elf_greg_t elf_gregset_t[ELF_NGREG];

typedef struct user_fpu_struct elf_fpregset_t;

/*
 * This is used to ensure we don't load something for the wrong architecture.
 */
$define elf_check_arch(x) ( (x)->e_machine == EM_AVR32 )

/*
 * These are used to set parameters in the core dumps.
 */
$define ELF_CLASS	ELFCLASS32
$define ELF_DATA	ELFDATA2LSB
$define ELF_DATA	ELFDATA2MSB
$define ELF_ARCH	EM_AVR32

$define ELF_EXEC_PAGESIZE	4096

/* This is the location that an ET_DYN program is loaded if exec'ed.  Typical
   use of this is to invoke "./ld.so someprog" to test out a new version of
   the loader.  We need to make sure that it is out of the way of the program
   that it will "exec", and that there is sufficient room for the brk.  */

$define ELF_ET_DYN_BASE         (TASK_SIZE / 3 * 2)


/* This yields a mask that user programs can use to figure out what
   instruction set this CPU supports.  This could be done in user space,
   but it's not easy, and we've already done it here.  */

$define ELF_HWCAP	(0)

/* This yields a string that ld.so will use to load implementation
   specific libraries for optimization.  This is more specific in
   intent than poking at uname or /proc/cpuinfo.

   For the moment, we have only optimizations for the Intel generations,
   but that could change... */

$define ELF_PLATFORM  (NULL)

GENH(R_AVR32_NONE,     0, 0, 0,  FALSE, 0, dont,     0x00000000),
 212 +
 213 +  GENH(R_AVR32_32,       0, 2, 32, FALSE, 0, dont,     0xffffffff),
 214 +  GENH(R_AVR32_16,       0, 1, 16, FALSE, 0, bitfield, 0x0000ffff),
 215 +  GENH(R_AVR32_8,        0, 0,  8, FALSE, 0, bitfield, 0x000000ff),
 216 +  GENH(R_AVR32_32_PCREL,  0, 2, 32, TRUE,  0, signed,   0xffffffff),
 217 +  GENH(R_AVR32_16_PCREL,  0, 1, 16, TRUE,  0, signed,   0x0000ffff),
 218 +  GENH(R_AVR32_8_PCREL,          0, 0,  8, TRUE,  0, signed,   0x000000ff),
 219 +
 220 +  /* Difference between two symbol (sym2 - sym1).  The reloc encodes
 221 +     the value of sym1.  The field contains the difference before any
 222 +     relaxing is done.  */
 223 +  GENH(R_AVR32_DIFF32,   0, 2, 32, FALSE, 0, dont,     0xffffffff),
 224 +  GENH(R_AVR32_DIFF16,   0, 1, 16, FALSE, 0, signed,   0x0000ffff),
 225 +  GENH(R_AVR32_DIFF8,    0, 0,  8, FALSE, 0, signed,   0x000000ff),
 226 +
 227 +  GENH(R_AVR32_GOT32,    0, 2, 32, FALSE, 0, signed,   0xffffffff),
 228 +  GENH(R_AVR32_GOT16,    0, 1, 16, FALSE, 0, signed,   0x0000ffff),
 229 +  GENH(R_AVR32_GOT8,     0, 0,  8, FALSE, 0, signed,   0x000000ff),
 230 +
 231 +  GENH(R_AVR32_21S,      0, 2, 21, FALSE, 0, signed,   0x1e10ffff),
 232 +  GENH(R_AVR32_16U,      0, 2, 16, FALSE, 0, unsigned, 0x0000ffff),
 233 +  GENH(R_AVR32_16S,      0, 2, 16, FALSE, 0, signed,   0x0000ffff),
 234 +  GENH(R_AVR32_8S,       0, 1,  8, FALSE, 4, signed,   0x00000ff0),
 235 +  GENH(R_AVR32_8S_EXT,   0, 2,  8, FALSE, 0, signed,   0x000000ff),
 236 +
 237 +  GENH(R_AVR32_22H_PCREL, 1, 2, 21, TRUE,  0, signed,  0x1e10ffff),
 238 +  GENH(R_AVR32_18W_PCREL, 2, 2, 16, TRUE,  0, signed,  0x0000ffff),
 239 +  GENH(R_AVR32_16B_PCREL, 0, 2, 16, TRUE,  0, signed,  0x0000ffff),
 240 +  GENH(R_AVR32_16N_PCREL, 0, 2, 16, TRUE,  0, signed,  0x0000ffff),
 241 +  GENH(R_AVR32_14UW_PCREL, 2, 2, 12, TRUE, 0, unsigned, 0x0000f0ff),
 242 +  GENH(R_AVR32_11H_PCREL, 1, 1, 10, TRUE,  4, signed,  0x00000ff3),
 243 +  GENH(R_AVR32_10UW_PCREL, 2, 2, 8, TRUE,  0, unsigned, 0x000000ff),
 244 +  GENH(R_AVR32_9H_PCREL,  1, 1,  8, TRUE,  4, signed,  0x00000ff0),
 245 +  GENH(R_AVR32_9UW_PCREL, 2, 1,  7, TRUE,  4, unsigned,        0x000007f0),
 246 +
 247 +  GENH(R_AVR32_HI16,    16, 2, 16, FALSE, 0, dont,     0x0000ffff),
 248 +  GENH(R_AVR32_LO16,     0, 2, 16, FALSE, 0, dont,     0x0000ffff),
 249 +
 250 +  GENH(R_AVR32_GOTPC,    0, 2, 32, FALSE, 0, dont,     0xffffffff),
 251 +  GENH(R_AVR32_GOTCALL,   2, 2, 21, FALSE, 0, signed,  0x1e10ffff),
 252 +  GENH(R_AVR32_LDA_GOT,          2, 2, 21, FALSE, 0, signed,   0x1e10ffff),
 253 +  GENH(R_AVR32_GOT21S,   0, 2, 21, FALSE, 0, signed,   0x1e10ffff),
 254 +  GENH(R_AVR32_GOT18SW,          2, 2, 16, FALSE, 0, signed,   0x0000ffff),
 255 +  GENH(R_AVR32_GOT16S,   0, 2, 16, FALSE, 0, signed,   0x0000ffff),
 256 +  GENH(R_AVR32_GOT7UW,   2, 1,  5, FALSE, 4, unsigned, 0x000001f0),
 257 +
 258 +  GENH(R_AVR32_32_CPENT,  0, 2, 32, FALSE, 0, dont,    0xffffffff),
 259 +  GENH(R_AVR32_CPCALL,   2, 2, 16, TRUE,  0, signed,   0x0000ffff),
 260 +  GENH(R_AVR32_16_CP,    0, 2, 16, TRUE,  0, signed,   0x0000ffff),
 261 +  GENH(R_AVR32_9W_CP,    2, 1,  7, TRUE,  4, unsigned, 0x000007f0),
 262 +
 263 +  GENH(R_AVR32_RELATIVE,  0, 2, 32, FALSE, 0, signed,  0xffffffff),
 264 +  GENH(R_AVR32_GLOB_DAT,  0, 2, 32, FALSE, 0, dont,    0xffffffff),
 265 +  GENH(R_AVR32_JMP_SLOT,  0, 2, 32, FALSE, 0, dont,    0xffffffff),
 266 +
 267 +  GENH(R_AVR32_ALIGN,    0, 1, 0,  FALSE, 0, unsigned, 0x00000000),
 268 +
 269 +  GENH(R_AVR32_15S,      2, 2, 15, FALSE, 0, signed,   0x00007fff),
 270 +};
#endif
}

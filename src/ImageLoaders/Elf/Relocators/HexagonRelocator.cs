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
    public class HexagonRelocator : ElfRelocator32
    {
        public HexagonRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(Program program, ElfSymbol symbol, ElfSection? referringSection, ElfRelocation rela)
        {
            return (null, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((HexagonRt) type).ToString();
        }

        public enum HexagonRt
        {
            // https://docs.huihoo.com/doxygen/linux/kernel/3.7/arch_2hexagon_2include_2asm_2elf_8h.html
            R_HEXAGON_NONE = 0,
            R_HEXAGON_B22_PCREL = 1,
            R_HEXAGON_B15_PCREL = 2,
            R_HEXAGON_B7_PCREL = 3,
            R_HEXAGON_LO16 = 4,
            R_HEXAGON_HI16 = 5,
            R_HEXAGON_32 = 6,
            R_HEXAGON_16 = 7,
            R_HEXAGON_8 = 8,
            R_HEXAGON_GPREL16_0 = 9,
            R_HEXAGON_GPREL16_1 = 10,
            R_HEXAGON_GPREL16_2 = 11,

            R_HEXAGON_GPREL16_3 = 12,

            R_HEXAGON_HL16 = 13,

            R_HEXAGON_B13_PCREL = 14,

            R_HEXAGON_B9_PCREL = 15,

            R_HEXAGON_B32_PCREL_X = 16,

            R_HEXAGON_32_6_X = 17,

            R_HEXAGON_B22_PCREL_X = 18,

            R_HEXAGON_B15_PCREL_X = 19,

            R_HEXAGON_B13_PCREL_X = 20,

            R_HEXAGON_B9_PCREL_X = 21,

            R_HEXAGON_B7_PCREL_X = 22,

            R_HEXAGON_16_X = 23,

            R_HEXAGON_12_X = 24,

            R_HEXAGON_11_X = 25,

            R_HEXAGON_10_X = 26,

            R_HEXAGON_9_X = 27,

            R_HEXAGON_8_X = 28,

            R_HEXAGON_7_X = 29,

            R_HEXAGON_6_X = 30,

            R_HEXAGON_32_PCREL = 31,

            R_HEXAGON_COPY = 32,

            R_HEXAGON_GLOB_DAT = 33,

            R_HEXAGON_JMP_SLOT = 34,

            R_HEXAGON_RELATIVE = 35,

            R_HEXAGON_PLT_B22_PCREL = 36,

            R_HEXAGON_GOTOFF_LO16 = 37,

            R_HEXAGON_GOTOFF_HI16 = 38,

            R_HEXAGON_GOTOFF_32 = 39,

            R_HEXAGON_GOT_LO16 = 40,

            R_HEXAGON_GOT_HI16 = 41,

            R_HEXAGON_GOT_32 = 42,

            R_HEXAGON_GOT_16 = 43,
        }
    }
}
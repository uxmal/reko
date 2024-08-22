#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Loading;
using System.Collections.Generic;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class Nios2Relocator : ElfRelocator32
    {
        public Nios2Relocator(ElfLoader32 elfLoader32, SortedList<Address, ImageSymbol> imageSymbols)
            : base(elfLoader32, imageSymbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var rt = (Nios2Rt) (rela.Info & 0xFF);
            return default;
        }

        public override string? RelocationTypeToString(uint type)
        {
            return ((Nios2Rt) type).ToString();
        }
    }

    public enum Nios2Rt
    {
        R_NIOS2_NONE = 0,           //  n/a None n/a n/a
        R_NIOS2_S16 = 1,            //  Yes S + A 0x003FFFC0 6
        R_NIOS2_U16 = 2,            //  Yes S + A 0x003FFFC0 6
        R_NIOS2_PCREL16 = 3,        //  Yes((S + A) – 4) – PC 0x003FFFC0 6
        R_NIOS2_CALL26 = 4,         //  Yes(S + A) >> 2 0xFFFFFFC0 6
        R_NIOS2_CALL26_NOAT = 41,   //  No(S + A) >> 2 0xFFFFFFC0 6
        R_NIOS2_IMM5 = 5,           //  Yes(S + A) & 0x1F 0x000007C0 6
        R_NIOS2_CACHE_OPX = 6,      //  Yes(S + A) & 0x1F 0x07C00000 22
        R_NIOS2_IMM6 = 7,           //  Yes(S + A) & 0x3F 0x00000FC0 6
        R_NIOS2_IMM8 = 8,           //  Yes(S + A) & 0xFF 0x00003FC0 6
        R_NIOS2_HI16 = 9,           //  No((S + A) >> 16) & 0xFFFF 0x003FFFC0 6
        R_NIOS2_LO16 = 10,          //  No(S + A) & 0xFFFF 0x003FFFC0 6
        R_NIOS2_HIADJ16 = 11,       //  No Adj(S+A) 0x003FFFC0 6
        R_NIOS2_BFD_RELOC_32 = 12,  //  No S + A 0xFFFFFFFF 0
        R_NIOS2_BFD_RELOC_16 = 13,  //  Yes(S + A) & 0xFFFF 0x0000FFFF 0
        R_NIOS2_BFD_RELOC_8 = 14,   //  Yes(S + A) & 0xFF 0x000000FF 0
        R_NIOS2_GPREL = 15,         //  No(S + A – GP) & 0xFFFF 0x003FFFC0 6
        R_NIOS2_GNU_VTINHERIT = 16, //  n/a None n/a n/a
        R_NIOS2_GNU_VTENTRY = 17,              //  n/a None n/a n/a
        R_NIOS2_UJMP = 18,          //  No((S + A) >> 16) & 0xFFFF, (S + A + 4) & 0xFFFF 0x003FFFC0 6
        R_NIOS2_CJMP = 19,          //  No((S + A) >> 16) & 0xFFFF, (S + A + 4) & 0xFFFF 0x003FFFC0 6
        R_NIOS2_CALLR = 20,         //  No((S + A) >> 16) & 0xFFFF) (S + A + 4) & 0xFFFF 0x003FFFC0 6
        R_NIOS2_ALIGN = 21,         //  n/a None n/a n/a
        R_NIOS2_GOT16 = 22,         // (42) Yes G 0x003FFFC0 6
        R_NIOS2_CALL16 = 23,        // (42) Yes G 0x003FFFC0 6
        R_NIOS2_GOTOFF_LO = 24,     // (42) No(S + A – GOT) & 0xFFFF 0x003FFFC0 6
        R_NIOS2_GOTOFF_HA = 25,     // (42) No Adj(S + A – GOT) 0x003FFFC0 6
        R_NIOS2_PCREL_LO = 26,      // (42) No(S + A – PC) & 0xFFFF 0x003FFFC0 6
        R_NIOS2_PCREL_HA = 27,      // (42) No Adj(S + A – PC) 0x003FFFC0 6
        R_NIOS2_TLS_GD16 = 28,      // (42) Yes Refer to Thread-LocalStorage section 0x003FFFC0 6
        R_NIOS2_TLS_LDM16 = 29,     // (42) Yes Refer to Thread-Local Storage section 0x003FFFC0 6
    }
}
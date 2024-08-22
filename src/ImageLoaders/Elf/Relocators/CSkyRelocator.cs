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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class CSkyRelocator : ElfRelocator32
    {
        private RelocState state;

        public CSkyRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols)
            : base(loader, imageSymbols)
        {
            this.state = new RelocState();
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            return default;
        }

        public override string? RelocationTypeToString(uint type)
        {
            return ((CSkyRt) type).ToString();
        }

        private class RelocState
        {
            public uint S { get; set; }

        }
        public enum CSkyRt
        {
            R_CKCORE_NONE = 0,                       // none none ALL

            R_CKCORE_ADDR32 = 1,                   // word32 S+A ALL

            R_CKCORE_PCREL_IMM8BY4 = 2,            // dis8          ((S+A-P)>>2)&&0xff V1.0
            R_CKCORE_PCREL_IMM11BY2 = 3,           // disp11        ((S+A-P)>>1)&0x7ff V1.0
            R_CKCORE_PCREL_IMM4BY2 = 4,            // none unsupported, deleted None
            R_CKCORE_PCREL32 = 5,                  // word32         S+A-P ??
            R_CKCORE_PCREL_JSR_IMM11BY2 = 6,       // disp11        ((S+A-P)>>1)&0x7ff V1.0
            R_CKCORE_GNU_VTINHERIT = 7,                       // - ?? ??
            R_CKCORE_GNU_VTENTRY = 8,                       // - ?? ??
            R_CKCORE_RELATIVE = 9,                 // word32        B + A               ALL
            R_CKCORE_COPY = 10,                    // none          none                ALL
            R_CKCORE_GLOB_DAT = 11,                // word32        S                   ALL
            R_CKCORE_JUMP_SLOT = 12,               // word32        S                   ALL
            R_CKCORE_GOTOFF = 13,                  // word32        S + A - GOT         V1.0
            R_CKCORE_GOTPC = 14,                   // word32 GOT+A-P V1.0
            R_CKCORE_GOT32 = 15,                   // word32 G V1.0
            R_CKCORE_PLT32 = 16,                   // word32 G V1.0
            R_CKCORE_ADDRGOT = 17,                 // word32 GOT+G V1.0 32-bit
            R_CKCORE_ADDRPLT = 18,                 // word32 GOT+G V1.0 32-bit
            R_CKCORE_PCREL_IMM26BY2 = 19,          // disp26 ((S+A–P)>>1)&0x3ffffff V2.0 32-bit
            R_CKCORE_PCREL_IMM16BY2 = 20,          // disp16 ((S+A-P)>>1)&0xffff V2.0 32-bit
            R_CKCORE_PCREL_IMM16BY4 = 21,          // disp16 ((S+A-P)>>2)&0xffff V2.0 32-bit
            R_CKCORE_PCREL_IMM10BY2 = 22,          // disp10 ((S+A-P)>>1)&0x3ff V2.0 16-bit
            R_CKCORE_PCREL_IMM10BY4 = 23,          // disp10 ((S+A-P)>>2)&0x3ff V2.0 16-bit
            R_CKCORE_ADDR_HI16 = 24,               // word_hi16 ((S+A)>>16)&0xffff V2.0 32-bit
            R_CKCORE_ADDR_LO16 = 25,               // word_lo16 (S+A)&0xffff V2.0 32-bit
            R_CKCORE_GOTPC_HI16 = 26,              // gb_disp_hi16 ((GOT+A-P)>16)&0xffff V2.0 32-bit
            R_CKCORE_GOTPC_LO16 = 27,              // gb_disp_lo16 (GOT+A-P)&0xffff V2.0 32-bit
            R_CKCORE_GOTOFF_HI16 = 28,             // gb_offset_hi16 ((S+A-GOT) >> 16) & 0xffff V2.0 32-bit
            R_CKCORE_GOTOFF_LO16 = 29,             // gb_offset_lo16 (S+A-GOT) & 0xffff V2.0 32-bit
            R_CKCORE_GOT12 = 30,                   // disp12 G V2.0 32-bit
            R_CKCORE_GOT_HI16 = 31,                // gb_got_hi16 (G >> 16) & 0xffff V2.0 32-bit
            R_CKCORE_GOT_LO16 = 32,                 // gb_got_lo16 G & 0xffff V2.0 32-bit
            R_CKCORE_PLT12 = 33,                    // disp12 G V2.0 32-bit
            R_CKCORE_PLT_HI16 = 34,                 // gb_got_hi16 (G >> 16) & 0xffff V2.0 32-bit
            R_CKCORE_PLT_LO16 = 35,                 // gb_got_lo16 G & 0xffff V2.0 32-bit
            R_CKCORE_ADDRGOT_HI16 = 36,             // gb_got_hi16 (GOT+G*4)& 0xffff V2.0 32-bit
            R_CKCORE_ADDRGOT_LO16 = 37,             // gb_got_lo16 (GOT+G*4) & 0xffff V2.0 32-bit
            R_CKCORE_ADDRPLT_HI16 = 38,             // gb_got_hi16 ((GOT+G*4) >> 16) & 0xffff V2.0 32-bit
            R_CKCORE_ADDRPLT_LO16 = 39,             // gb_got_lo16 (GOT+G*4) & 0xffff V2.0 32-bit
            R_CKCORE_PCREL_JSR_IMM26BY2 = 40,       // disp26 ((S+A–P)>>1)&0x3ffffff V2.0 32-bit
            R_CKCORE_TOFFSET_LO16 = 41,             // disp16 (S+A-BTEXT) & 0xffff V2.0 32-bit
            R_CKCORE_DOFFSET_LO16 = 42,             // disp16 (S+A-BTEXT) & 0xffff V2.0 32-bit
            R_CKCORE_PCREL_IMM18BY2 = 43,           // disp16 ((S+A–P)>>1)&0x3ffff V2.0 32-bit
            R_CKCORE_DOFFSET_IMM18ABS = 44,         // word_disp18 (S+A-BDATA)&0x3ffff V2.0 32-bit
            R_CKCORE_DOFFSET_IMM18BY2ABS = 45,      // word_disp18 ((S+A-BDATA)>>1)&0x3ffff V2.0 32-bit
            R_CKCORE_DOFFSET_IMM18BY4ABS = 46,      // word_disp18 ((S+A-BDATA)>>2)&0x3ffff V2.0 32-bit
            R_CKCORE_GOTOFF_IMM18 = 47,             // disp18 ? V2.0 32-bit
            R_CKCORE_GOT_IMM18BY4 = 48,             // word_disp18 (G >> 2) V2.0 32-bit
            R_CKCORE_PLT_IMM18BY4 = 49,             //word_disp = 18,                       // (G >> 2) V2.0 32-bit
            R_CKCORE_PCREL_IMM7BY4 = 50,            // disp7 ((S+A-P) >>2) & 0x7f V2.0 16-bit     
        }
    }
}

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

using System;
using Reko.Core;
using System.Collections.Generic;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class XtensaRelocator : ElfRelocator32
    {
        public XtensaRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var rt = (XtensaRt) (rela.Info & 0xFF);
            ElfImageLoader.trace.Inform("XTensa relocation {0}: {1}", rt, rela);
            switch (rt)
            {
            case XtensaRt.R_XTENSA_32:
                //aspace.make_data(raddr, wordsz);
                //if (value is not null) {
                //    data = aspace.get_data(raddr, wordsz);
                //    if (is_exe) {
                //        if (data != value) {
                //            log.Warn("Computed reloc value and value present in fully linked file differ: 0x%x vs 0x%x", value, data)
                //        }
                //    }
                //    else {
                //        data += value;
                //        aspace.set_data(raddr, data, wordsz);
                //        aspace.make_arg_offset(raddr, 0, data);
                //    }
                //} else {
                //    // Undefined symbol
                //    // TODO: This is more or less hacky way to do this. It would be
                //    // better to explicitly assign a symbolic alias to a value at
                //    // particular address, but so far we assume call below to do
                //    // that.
                //    aspace.make_arg_offset(raddr, 0, symname);
                //}
                break;
            case XtensaRt.R_XTENSA_SLOT0_OP:
                //if (is_exe)
                //    break;
                //var opcode = aspace.get_byte(raddr);
                //if ((opcode & 0xf) == 0x5) {
                //    // call
                //    if (value is not null) {
                //        p = raddr
                //        value -= ((p & ~0x3) + 4);
                //        Debug.Assert((value & 0x3) == 0);
                //        value = value >> 2;
                //        aspace.set_byte(p, (opcode & ~0xc0) | ((value << 6) & 0xc0));
                //        aspace.set_byte(p + 1, value >> 2);
                //        aspace.set_byte(p + 2, value >> 10);
                //    }
                //}
                //if (opcode & 0xf == 0x1) {
                //    // l32r
                //}
                break;

            default:
                break;
            }
            return (null, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((XtensaRt) type).ToString();
        }
    }

    public enum XtensaRt
    {
        R_XTENSA_NONE = 0,
        R_XTENSA_32 = 1,
        R_XTENSA_RTLD = 2,
        R_XTENSA_GLOB_DAT = 3,
        R_XTENSA_JMP_SLOT = 4,
        R_XTENSA_RELATIVE = 5,
        R_XTENSA_PLT = 6,

        R_XTENSA_OP0 = 8,
        R_XTENSA_OP1 = 9,
        R_XTENSA_OP2 = 10,

        R_XTENSA_ASM_EXPAND = 11,
        R_XTENSA_ASM_SIMPLIFY = 12,
        R_XTENSA_GNU_VTINHERIT = 15,
        R_XTENSA_GNU_VTENTRY = 16,

        R_XTENSA_DIFF8 = 17,
        R_XTENSA_DIFF16 = 18,
        R_XTENSA_DIFF32 = 19,

        R_XTENSA_SLOT0_OP = 20,
        R_XTENSA_SLOT1_OP = 21,
        R_XTENSA_SLOT2_OP = 22,
        R_XTENSA_SLOT3_OP = 23,
        R_XTENSA_SLOT4_OP = 24,
        R_XTENSA_SLOT5_OP = 25,
        R_XTENSA_SLOT6_OP = 26,
        R_XTENSA_SLOT7_OP = 27,
        R_XTENSA_SLOT8_OP = 28,
        R_XTENSA_SLOT9_OP = 29,
        R_XTENSA_SLOT10_OP = 30,
        R_XTENSA_SLOT11_OP = 31,
        R_XTENSA_SLOT12_OP = 32,
        R_XTENSA_SLOT13_OP = 33,
        R_XTENSA_SLOT14_OP = 34,

        R_XTENSA_SLOT0_ALT = 35,
        R_XTENSA_SLOT1_ALT = 36,
        R_XTENSA_SLOT2_ALT = 37,
        R_XTENSA_SLOT3_ALT = 38,
        R_XTENSA_SLOT4_ALT = 39,
        R_XTENSA_SLOT5_ALT = 40,
        R_XTENSA_SLOT6_ALT = 41,
        R_XTENSA_SLOT7_ALT = 42,
        R_XTENSA_SLOT8_ALT = 43,
        R_XTENSA_SLOT9_ALT = 44,
        R_XTENSA_SLOT10_ALT = 45,
        R_XTENSA_SLOT11_ALT = 46,
        R_XTENSA_SLOT12_ALT = 47,
        R_XTENSA_SLOT13_ALT = 48,
        R_XTENSA_SLOT14_ALT = 49,
    }
}
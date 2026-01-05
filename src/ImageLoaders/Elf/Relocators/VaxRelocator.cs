#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Diagnostics;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class VaxRelocator : ElfRelocator32
    {
        public VaxRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override void Relocate(Program program, Address addrBase, Dictionary<ElfSymbol, Address> pltEntries)
        {
            base.Relocate(program, addrBase, pltEntries);
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = Address.Ptr32((uint) ctx.P);
            var rt = (VaxRt) (rela.Info & 0xFF);
            switch (rt)
            {
            case VaxRt.R_VAX_JMP_SLOT:
                var st = ElfLoader.GetSymbolType(symbol);
                if (!st.HasValue)
                    return (null, null);
                return (addr, symbol);

            default:
                ctx.Warn(addr, $"VAX ELF relocation type {rt} not implemented yet.");
                break;
            }
            return (null, null);
        }

        public override string? RelocationTypeToString(uint type)
        {
            return ((VaxRt) type).ToString();
        }
    }

    // https://github.com/alex91ar/gdb-multiarch/blob/master/gdb-7.11/include/elf/vax.h
    public enum VaxRt
    {
        R_VAX_NONE = 0,         /* No reloc */
        R_VAX_32 = 1,           /* Direct 32 bit  */
        R_VAX_16 = 2,           /* Direct 16 bit  */
        R_VAX_8 = 3,            /* Direct 8 bit  */
        R_VAX_PC32 = 4,         /* PC relative 32 bit */
        R_VAX_PC16 = 5,         /* PC relative 16 bit */
        R_VAX_PC8 = 6,          /* PC relative 8 bit */
        R_VAX_GOT32 = 7,        /* 32 bit PC relative GOT entry */
        R_VAX_PLT32 = 13,       /* 32 bit PC relative PLT address */
        R_VAX_COPY = 19,        /* Copy symbol at runtime */
        R_VAX_GLOB_DAT = 20,    /* Create GOT entry */
        R_VAX_JMP_SLOT = 21,    /* Create PLT entry */
        R_VAX_RELATIVE = 22,    /* Adjust by program base */
        /* These are GNU extensions to enable C++ vtable garbage collection.  */
        R_VAX_GNU_VTINHERIT = 23,
        R_VAX_GNU_VTENTRY = 24,

    }
}

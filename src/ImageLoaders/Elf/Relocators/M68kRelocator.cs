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
using Reko.Core.Loading;
using System.Collections.Generic;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class M68kRelocator : ElfRelocator32
    {
        public M68kRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            return (null, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return type.ToString();
        }

        public enum M68kRt
        { 
            R_68K_NONE     = 0,              /* No reloc */
            R_68K_32       = 1,              /* Direct 32 bit  */
            R_68K_16       = 2,              /* Direct 16 bit  */
            R_68K_8        = 3,              /* Direct 8 bit  */
            R_68K_PC32     = 4,              /* PC relative 32 bit */
            R_68K_PC16     = 5,              /* PC relative 16 bit */
            R_68K_PC8      = 6,              /* PC relative 8 bit */
            R_68K_GOT32    = 7,              /* 32 bit PC relative GOT entry */
            R_68K_GOT16    = 8,              /* 16 bit PC relative GOT entry */
            R_68K_GOT8     = 9,              /* 8 bit PC relative GOT entry */
            R_68K_GOT32O   = 10,             /* 32 bit GOT offset */
            R_68K_GOT16O   = 11,             /* 16 bit GOT offset */
            R_68K_GOT8O    = 12,             /* 8 bit GOT offset */
            R_68K_PLT32    = 13,             /* 32 bit PC relative PLT address */
            R_68K_PLT16    = 14,             /* 16 bit PC relative PLT address */
            R_68K_PLT8     = 15,             /* 8 bit PC relative PLT address */
            R_68K_PLT32O   = 16,             /* 32 bit PLT offset */
            R_68K_PLT16O   = 17,             /* 16 bit PLT offset */
            R_68K_PLT8O    = 18,             /* 8 bit PLT offset */
            R_68K_COPY     = 19,             /* Copy symbol at runtime */
            R_68K_GLOB_DAT = 20,             /* Create GOT entry */
            R_68K_JMP_SLOT = 21,             /* Create PLT entry */
            R_68K_RELATIVE = 22,             /* Adjust by program base */
            R_68K_NUM      = 23,
        }
    }
}

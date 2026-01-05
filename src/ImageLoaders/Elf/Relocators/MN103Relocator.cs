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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Loading;

namespace Reko.ImageLoaders.Elf.Relocators;

public class MN103Relocator : ElfRelocator
{
    public MN103Relocator(ElfLoader32 elfLoader32, SortedList<Address, ImageSymbol> imageSymbols)
        : base(imageSymbols)
    {
        this.Loader = elfLoader32;
    }

    public override ElfLoader Loader { get; }

    public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
    {
        return default;
    }

    public override string? RelocationTypeToString(uint type)
    {
        throw new NotImplementedException();
    }

    protected override void DumpDynamicSegment(ElfSegment dynSeg)
    {
        throw new NotImplementedException();
    }
}

public enum MN103Rt
{
     // https://android.googlesource.com/kernel/mediatek/+/android-6.0.1_r0.6/arch/mn10300/include/asm/elf.h?autodive=0%2F%2F%2F%2F
     R_MN10300_NONE		=  0,	/* No reloc.  */
     R_MN10300_32		=  1,	/* Direct 32 bit.  */
     R_MN10300_16		=  2,	/* Direct 16 bit.  */
     R_MN10300_8		=  3,	/* Direct 8 bit.  */
     R_MN10300_PCREL32	=  4,	/* PC-relative 32-bit.  */
     R_MN10300_PCREL16	=  5,	/* PC-relative 16-bit signed.  */
     R_MN10300_PCREL8	=  6,	/* PC-relative 8-bit signed.  */
     R_MN10300_24		=  9,	/* Direct 24 bit.  */
     R_MN10300_RELATIVE	=  23,	/* Adjust by program base.  */
     R_MN10300_SYM_DIFF	=  33,	/* Adjustment when relaxing. */
     R_MN10300_ALIGN 	=  34,	/* Alignment requirement. */
}

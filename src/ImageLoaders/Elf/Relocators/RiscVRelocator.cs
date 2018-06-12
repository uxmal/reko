﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Linq;
using System.Text;
using Reko.Core;

namespace Reko.ImageLoaders.Elf.Relocators
{
    class RiscVRelocator
    {
    }

    public class RiscVRelocator64 : ElfRelocator64
    {
        public RiscVRelocator64(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rela)
        {
            var rt = (RiscV64Rt)(rela.Info & 0xFF);

            switch (rt)
            {
            case RiscV64Rt.R_RISCV_COPY:
                break;
            }
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((RiscV64Rt)type).ToString();
        }
    }

    public enum RiscV64Rt
    {
        R_RISCV_NONE = 0,
        R_RISCV_32 = 1,
        R_RISCV_64 = 2,
        R_RISCV_RELATIVE = 3,
        R_RISCV_COPY = 4,
        R_RISCV_JUMP_SLOT = 5,
        R_RISCV_TLS_DTPMOD32 = 6,
        R_RISCV_TLS_DTPMOD64 = 7,
        R_RISCV_TLS_DTPREL32 = 8,
        R_RISCV_TLS_DTPREL64 = 9,
        R_RISCV_TLS_TPREL32 = 1,
        R_RISCV_TLS_TPREL64 = 1,
    }
}

#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Diagnostics;
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

        public override ElfSymbol RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rela)
        {
            var rt = (RiscV64Rt)(rela.Info & 0xFF);
            ulong S = symbol.Value;
            ulong A = 0;
            ulong B = 0;

            var addr = referringSection != null
                ? referringSection.Address + rela.Offset
                : loader.CreateAddress(rela.Offset);
            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);

            switch (rt)
            {
            case RiscV64Rt.R_RISCV_COPY:
                return symbol;
            case RiscV64Rt.R_RISCV_RELATIVE: // B + A
                A = (ulong) rela.Addend;
                B = program.SegmentMap.BaseAddress.ToLinear();
                S = 0;
                break;
            case RiscV64Rt.R_RISCV_64: // S + A
                A = (ulong) rela.Addend;
                B = 0;
                break;
            case RiscV64Rt.R_RISCV_JUMP_SLOT: // S
                S = symbol.Value;
                if (S == 0)
                {
                    var gotEntry = relR.PeekLeUInt64(0);
                    return CreatePltStubSymbolFromRelocation(symbol, gotEntry, 0x0);
                }
                break;
            default:
                Debug.Print("ELF RiscV: unhandled relocation {0}: {1}", rt, rela);
                break;
            }

            var w = relR.ReadLeUInt64();
            w += ((uint) (B + S + A));
            relW.WriteLeUInt64(w);


            return symbol;
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

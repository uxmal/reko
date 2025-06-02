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
using System;
using System.Collections.Generic;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class PaRiscRelocator : ElfRelocator
    {
        private readonly ElfLoader elfLoader;
        private readonly SortedList<Address, ImageSymbol> symbols;

        public PaRiscRelocator(ElfLoader elfLoader, SortedList<Address, ImageSymbol> symbols) : base(symbols)
        {
            this.elfLoader = elfLoader;
            this.symbols = symbols;
        }

        public override ElfLoader Loader => elfLoader;

        public override void Relocate(Program program, Address addrBase, Dictionary<ElfSymbol, Address> pltEntries)
        {
            //$TODO: do something with relocations.
            return;
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            return default;
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((PaRiscRt) type).ToString();
        }

        protected override void DumpDynamicSegment(ElfSegment dynSeg)
        {
            throw new NotImplementedException();
        }
    }

    public enum PaRiscRt
    {
        R_PARISC_NONE = 0,      // none none
        R_PARISC_DIR32 = 1,      // 32-bit word symbol + addend
        R_PARISC_DIR21L = 2,      // Long Immediate (7) LR(symbol, addend)
        R_PARISC_DIR17R = 3,      // Branch External (19) RR(symbol, addend)
        R_PARISC_DIR17F = 4,      // Branch External (19) symbol + addend
        R_PARISC_DIR14R = 6,      // Load/Store (1) RR(symbol, addend)
        R_PARISC_PCREL21L = 10,      // Long Immediate (7) L(symbol – PC – 8 + addend)
        R_PARISC_PCREL17R = 11,      // Branch External (19) R(symbol – PC – 8 + addend)
        R_PARISC_PCREL17F = 12,      // Branch (20) symbol – PC – 8 + addend
        R_PARISC_PCREL17C = 13,      // Branch (20) symbol – PC – 8 + addend
        R_PARISC_PCREL14R = 14,      // Load/Store (1) R(symbol – PC – 8 + addend)
        R_PARISC_DPREL21L = 18,      // Long Immediate (7) LR(symbol – GP, addend)
        R_PARISC_DPREL14WR = 19,      // Load/Store Mod. Comp. (2) RR(symbol – GP, addend)
        R_PARISC_DPREL14DR = 20,      // Load/Store Doubleword (3) RR(symbol – GP, addend)
        R_PARISC_DPREL14R = 22,      // Load/Store (1) RR(symbol – GP, addend)
        R_PARISC_DLTREL21L = 26,      // Long Immediate (7) LR(symbol – GP, addend)
        R_PARISC_DLTREL14R = 30,      // Load/Store (1) RR(symbol – GP, addend)
        R_PARISC_DLTIND21L = 34,      // Long Immediate (7) L(ltoff(symbol + addend))
        R_PARISC_DLTIND14R = 38,      // Load/Store (1) R(ltoff(symbol + addend))
        R_PARISC_DLTIND14F = 39,      // Load/Store (1) ltoff(symbol + addend)
        R_PARISC_SETBASE = 40,      // none no relocation; base := symbol
        R_PARISC_SECREL32 = 41,      // 32-bit word symbol – SECT + addend
        R_PARISC_BASEREL21L = 42,      // Long Immediate (7) LR(symbol – base, addend)
        R_PARISC_BASEREL17R = 43,      // Branch External (19) RR(symbol – base, addend)
        R_PARISC_BASEREL14R = 46,      // Load/Store (1) RR(symbol – base, addend)
        R_PARISC_SEGBASE = 48,      // none no relocation; SB := symbol
        R_PARISC_SEGREL32 = 49,      // 32-bit word symbol – SB + addend
        R_PARISC_PLTOFF21L = 50,      // Long Immediate (7) LR(pltoff(symbol), addend)
        R_PARISC_PLTOFF14R = 54,      // Load/Store (1) RR(pltoff(symbol), addend)
        R_PARISC_PLTOFF14F = 55,      // Load/Store (1) pltoff(symbol) + addend
        R_PARISC_PLABEL32 = 65,      // 32-bit word fptr(symbol)
        R_PARISC_PCREL22C = 73,      // Branch & Link (21) symbol – PC – 8 + addend
        R_PARISC_PCREL22F = 74,      // Branch & Link (21) symbol – PC – 8 + addend
        R_PARISC_PCREL14WR = 75,      // Load/Store Mod. Comp. (2) R(symbol – PC – 8 + addend)
        R_PARISC_PCREL14DR = 76,      // Load/Store Doubleword (3) R(symbol – PC – 8 + addend)
        R_PARISC_DIR14WR = 83,      // Load/Store Mod. Comp. (2) RR(symbol, addend)
        R_PARISC_DIR14DR = 84,      // Load/Store Doubleword (3) RR(symbol, addend)
        R_PARISC_DLTREL14WR = 91,      // Load/Store Mod. Comp. (2) RR(symbol – GP, addend)
        R_PARISC_DLTREL14DR = 92,      // Load/Store Doubleword (3) RR(symbol – GP, addend)
        R_PARISC_DLTIND14WR = 99,      // Load/Store Mod. Comp. (2) R(ltoff(symbol + addend))
        R_PARISC_DLTIND14DR = 100,      // Load/Store Doubleword (3) R(ltoff(symbol + addend))
        R_PARISC_BASEREL14WR = 107,      // Load/Store Mod. Comp. (2) RR(symbol – base, addend)
        R_PARISC_BASEREL14DR = 108,      // Load/Store Doubleword (3) RR(symbol – base, addend)
        R_PARISC_PLTOFF14WR = 115,      // Load/Store Mod. Comp. (2) RR(pltoff(symbol), addend)
        R_PARISC_PLTOFF14DR = 116,      // Load/Store Doubleword (3) RR(pltoff(symbol), addend)
        R_PARISC_LORESERVE = 128,
        R_PARISC_HIRESERVE = 255,
    }

    public enum PaRiscRt2
    {

R_PARISC_NONE = 0,      // none none
R_PARISC_DIR32 = 1,      // 32-bit word symbol + addend
R_PARISC_DIR21L = 2,      // Long Immediate (7) LR(symbol, addend)
R_PARISC_DIR17R = 3,      // Branch External (19) RR(symbol, addend)
R_PARISC_DIR17F = 4,      // Branch External (19) symbol + addend
R_PARISC_DIR14R = 6,      // Load/Store (1) RR(symbol, addend)
R_PARISC_PCREL32 = 9,      // 32-bit word symbol – PC – 8 + addend
R_PARISC_PCREL21L = 10,      // Long Immediate (7) L(symbol – PC – 8 + addend)
R_PARISC_PCREL17R = 11,      // Branch External (19) R(symbol – PC – 8 + addend)
R_PARISC_PCREL17F = 12,      // Branch (20) symbol – PC – 8 + addend
R_PARISC_PCREL14R = 14,      // Load/Store (1) R(symbol – PC – 8 + addend)
R_PARISC_GPREL21L = 26,      // Long Immediate (7) LR(symbol – GP, addend)
R_PARISC_GPREL14R = 30,      // Load/Store (1) RR(symbol – GP, addend)
R_PARISC_LTOFF21L = 34,      // Long Immediate (7) L(ltoff(symbol + addend))
R_PARISC_LTOFF14R = 38,      // Load/Store (1) R(ltoff(symbol + addend))
R_PARISC_SECREL32 = 41,      // 32-bit word symbol – SECT + addend
R_PARISC_SEGBASE = 48,      // none no relocation; SB := symbol
R_PARISC_SEGREL32 = 49,      // 32-bit word symbol – SB + addend
R_PARISC_PLTOFF21L = 50,      // Long Immediate (7) LR(pltoff(symbol), addend)
R_PARISC_PLTOFF14R = 54,      // Load/Store (1) RR(pltoff(symbol), addend)
R_PARISC_LTOFF_FPTR32 = 57,      // 32-bit word ltoff(fptr(symbol+addend))
R_PARISC_LTOFF_FPTR21L = 58,      // Long Immediate (7) L(ltoff(fptr(symbol+addend)))
R_PARISC_LTOFF_FPTR14R = 62,      // Load/Store (1) R(ltoff(fptr(symbol+addend)))
R_PARISC_FPTR64 = 64,      // 64-bit doubleword fptr(symbol+addend)
R_PARISC_PCREL64 = 72,      // 64-bit doubleword symbol – PC – 8 + addend
R_PARISC_PCREL22F = 74,      // Branch & Link (21) symbol – PC – 8 + addend
R_PARISC_PCREL14WR = 75,      // Load/Store Mod. Comp. (2) R(symbol – PC – 8 + addend)
R_PARISC_PCREL14DR = 76,      // Load/Store Doubleword (3) R(symbol – PC – 8 + addend)
R_PARISC_PCREL16F = 77,      // Load/Store (1) symbol – PC – 8 + addend
R_PARISC_PCREL16WF = 78,      // Load/Store Mod. Comp. (2) symbol – PC – 8 + addend
R_PARISC_PCREL16DF = 79,      // Load/Store Doubleword (3) symbol – PC – 8 + addend
R_PARISC_DIR64 = 80,      // 64-bit doubleword symbol + addend
R_PARISC_DIR14WR = 83,      // Load/Store Mod. Comp. (2) RR(symbol, addend)
R_PARISC_DIR14DR = 84,      // Load/Store Doubleword (3) RR(symbol, addend)
R_PARISC_DIR16F = 85,      // Load/Store (1) symbol + addend
R_PARISC_DIR16WF = 86,      // Load/Store Mod. Comp. (2) symbol + addend
R_PARISC_DIR16DF = 87,      // Load/Store Doubleword (3) symbol + addend
R_PARISC_GPREL64 = 88,      // 64-bit doubleword symbol – GP + addend
R_PARISC_GPREL14WR = 91,      // Load/Store Mod. Comp. (2) RR(symbol – GP, addend)
R_PARISC_GPREL14DR = 92,      // Load/Store Doubleword (3) RR(symbol – GP, addend)
R_PARISC_GPREL16F = 93,      // Load/Store (1) symbol – GP + addend
R_PARISC_GPREL16WF = 94,      // Load/Store Mod. Comp. (2) symbol – GP + addend
R_PARISC_GPREL16DF = 95,      // Load/Store Doubleword (3) symbol – GP + addend
R_PARISC_LTOFF64 = 96,      // 64-bit doubleword ltoff(symbol + addend)
R_PARISC_LTOFF14WR = 99,      // Load/Store Mod. Comp. (2) R(ltoff(symbol + addend))
R_PARISC_LTOFF14DR = 100,      // Load/Store Doubleword (3) R(ltoff(symbol + addend))
R_PARISC_LTOFF16F = 101,      // Load/Store (1) ltoff(symbol + addend)
R_PARISC_LTOFF16WF = 102,      // Load/Store Mod. Comp. (2) ltoff(symbol + addend)
R_PARISC_LTOFF16DF = 103,      // Load/Store Doubleword (3) ltoff(symbol + addend)
R_PARISC_SECREL64 = 104,      // 64-bit doubleword symbol – SECT + addend
R_PARISC_SEGREL64 = 112,      // 64-bit doubleword symbol – SB + addend
R_PARISC_PLTOFF14WR = 115,      // Load/Store Mod. Comp. (2) RR(pltoff(symbol), addend)
R_PARISC_PLTOFF14DR = 116,      // Load/Store Doubleword (3) RR(pltoff(symbol), addend)
R_PARISC_PLTOFF16F = 117,      // Load/Store (1) pltoff(symbol) + addend
R_PARISC_PLTOFF16WF = 118,      // Load/Store Mod. Comp. (2) pltoff(symbol) + addend
R_PARISC_PLTOFF16DF = 119,      // Load/Store Doubleword (3) pltoff(symbol) + addend
R_PARISC_LTOFF_FPTR64 = 120,      // 64-bit doubleword ltoff(fptr(symbol+addend))
R_PARISC_LTOFF_FPTR14WR = 123,      // Load/Store Mod. Comp. (2) R(ltoff(fptr(symbol+addend)))
R_PARISC_LTOFF_FPTR14DR = 124,      // Load/Store Doubleword (3) R(ltoff(fptr(symbol+addend)))
R_PARISC_LTOFF_FPTR16F = 125,      // Load/Store (1) ltoff(fptr(symbol+addend))
R_PARISC_LTOFF_FPTR16WF = 126,      // Load/Store Mod. Comp. (2) ltoff(fptr(symbol+addend))
R_PARISC_LTOFF_FPTR16DF = 127,      // Load/Store Doubleword (3) ltoff(fptr(symbol+addend))
R_PARSIC_LORESERVE = 128,
R_PARSIC_HIRESERVE = 255,
    }
}

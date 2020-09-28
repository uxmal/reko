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
using System.Threading.Tasks;
using Reko.Core;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class zSeriesRelocator : ElfRelocator64
    {
        public zSeriesRelocator(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override ElfSymbol RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, ElfRelocation rela)
        {
            ulong S = (ulong)symbol.Value;
            ulong A = 0;
            ulong B = 0;
            var rt = (zSeriesRt)(rela.Info & 0xFF);
            var addr = referringSection != null
                ? referringSection.Address + rela.Offset
                : loader.CreateAddress(rela.Offset);
            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);

            switch (rt)
            {
            case zSeriesRt.R_390_RELATIVE:  // B + A
                A = (ulong)rela.Addend;
                B = program.SegmentMap.BaseAddress.ToLinear();
                S = 0;
                break;
            case zSeriesRt.R_390_GLOB_DAT:  // S + A
                A = (ulong)rela.Addend;
                break;
            case zSeriesRt.R_390_JMP_SLOT:
                if (symbol.Value == 0)
                {
                    // Broken GCC compilers generate relocations referring to symbols 
                    // whose value is 0 instead of the expected address of the PLT stub.
                    var gotEntry = relR.PeekBeUInt64(0);
                    return CreatePltStubSymbolFromRelocation(symbol, gotEntry, 0xE);
                }
                break;
            default:
                Debug.Print("Unhandled relocation {0}: {1}", rt, rela);
                break;
            }

            var w = relR.ReadBeUInt64();
            w += ((uint)(B + S + A));
            relW.WriteBeUInt64(w);

            return symbol;
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((zSeriesRt)type).ToString();
        }
    }

    public enum zSeriesRt
    {
        R_390_NONE = 0,         // none none
        R_390_8 = 1,            // byte8 S + A
        R_390_12 = 2,           // low12 S + A
        R_390_16 = 3,           // half16 S + A
        R_390_32 = 4,           // word32 S + A
        R_390_PC32 = 5,         // word32 S+A-P
        R_390_GOT12 = 6,        // low12 O + A
        R_390_GOT32 = 7,        // word32 O + A
        R_390_PLT32 = 8,        // word32 L + A
        R_390_COPY = 9,         // none (see below)
        R_390_GLOB_DAT = 10,    // quad64 S + A (see below)
        R_390_JMP_SLOT = 11,    // none (see below)
        R_390_RELATIVE = 12,    // quad64 B + A (see below)
        R_390_GOTOFF = 13,      // quad64 S+A-G
        R_390_GOTPC = 14,       // quad64 G+A-P
        R_390_GOT16 = 15,       // half16 O + A
        R_390_PC16 = 16,        // half16 S+A-P
        R_390_PC16DBL = 17,     // pc16 (S + A - P) >> 1
        R_390_PLT16DBL = 18,    // pc16 (L + A - P) >> 1
        R_390_PC32DBL = 19,     // pc32 (S + A - P) >> 1
        R_390_PLT32DBL = 20,    // pc32 (L + A - P) >> 1
        R_390_GOTPCDBL = 21,    // pc32 (G + A - P) >> 1
        R_390_64 = 22,          // quad64 S + A
        R_390_PC64 = 23,        // quad64 S+A-P
        R_390_GOT64 = 24,       // quad64 O + A
        R_390_PLT64 = 25,       // quad64 L + A
        R_390_GOTENT = 26,      // pc32 (G + O + A - P) >> 1
    }
}

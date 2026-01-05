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
    // https://docs.oracle.com/cd/E23824_01/html/819-0690/chapter6-1235.html
    public class Sparc32Relocator : ElfRelocator32
    {
        private Dictionary<Address,ImportReference> importReferences;

        public Sparc32Relocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
            importReferences = null!;
        }

        public override void Relocate(Program program, Address addrBase, Dictionary<ElfSymbol, Address> pltEntries)
        {
            this.importReferences = program.ImportReferences;
            base.Relocate(program, addrBase, pltEntries);
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            Debug.Print("  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2} {5}",
                rela.Offset,
                (SparcRt) (rela.Info & 0xFF),
                symbol.Name,
                rela.Addend,
                (int) (rela.Info >> 8),
                "section?");

            var addr = ctx.CreateAddress(ctx.P);
            var rt = (SparcRt)(rela.Info & 0xFF);
            switch (rt)
            {
            case SparcRt.R_SPARC_GLOB_DAT:
            case SparcRt.R_SPARC_JMP_SLOT:
                if (symbol.SectionIndex == 0)
                {
                    var addrPfn = Address.Ptr32((uint) rela.Offset);
                    ctx.AddImportReference(symbol, addrPfn);
                    return (addrPfn, null);
                }
                return (null, null);
            case SparcRt.R_SPARC_NONE:
                return (addr, null);
            case SparcRt.R_SPARC_HI22:  // (S + A) >> 10
                if (!ctx.TryReadUInt32(addr, out uint w))
                    return default;
                w = (w & ~0x3F_FFFFu) | (uint)((ctx.S + ctx.A) >> 10);
                ctx.WriteUInt32(addr, w);
                return (addr, null);
            case SparcRt.R_SPARC_LO10:  // (S + A) & 0x3FF
                if (!ctx.TryReadUInt32(addr, out w))
                    return default;
                w = (w & ~0x3FFu) | (uint) ((ctx.S + ctx.A) & 0x3FF);
                ctx.WriteUInt32(addr, w);
                return (addr, null);
            case SparcRt.R_SPARC_32:    // S + A
                ctx.WriteUInt32(addr, ctx.S + ctx.A);
                return (addr, null);
            case SparcRt.R_SPARC_WDISP30:   // (S + A) >> 2
                if (!ctx.TryReadUInt32(addr, out w))
                    return default;
                w = (w & ~0x3FFF_FFFFu) | (uint) ((ctx.S + ctx.A - ctx.P) >> 2);
                ctx.WriteUInt32(addr, w);
                return (addr, null);
            case SparcRt.R_SPARC_RELATIVE:  // B + A
                ctx.WriteUInt32(addr, ctx.B + ctx.A);
                return (addr, null);
            case SparcRt.R_SPARC_COPY:
                return (addr, null);
            default:
                ctx.Warn(addr, $"SPARC ELF relocation type {rt} not implemented yet.");
                return (addr, null);
            }
        }

        private string LoadString(uint symtabOffset, uint sym)
        {
            return loader.ReadAsciiString(symtabOffset + sym);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((SparcRt)type).ToString();
        }
    }

    // https://docs.oracle.com/cd/E19683-01/816-1386/6m7qcoblh/index.html
    public enum SparcRt
    {
        R_SPARC_NONE = 0,       // None         None
        R_SPARC_8 = 1,          // V-byte8      S + A
        R_SPARC_16 = 2,         // V-half16     S + A
        R_SPARC_32 = 3,         // V-word32     S + A
        R_SPARC_DISP8 = 4,      // V-byte8      S + A - P
        R_SPARC_DISP16 = 5,     // V-half16     S + A - P
        R_SPARC_DISP32 = 6,     // V-disp32     S + A - P
        R_SPARC_WDISP30 = 7,    // V-disp30     (S + A - P) >> 2
        R_SPARC_WDISP22 = 8,    // V-disp22     (S + A - P) >> 2
        R_SPARC_HI22 = 9,       // T-imm22      (S + A) >> 10
        R_SPARC_22 = 10,        // V-imm22      S + A
        R_SPARC_13 = 11,        // V-simm13     S + A
        R_SPARC_LO10 = 12,      // T-simm13     (S + A) & 0x3ff
        R_SPARC_GOT10 = 13,     // T-simm13     G & 0x3ff
        R_SPARC_GOT13 = 14,     // V-simm13     G
        R_SPARC_GOT22 = 15,     // T-simm22     G >> 10
        R_SPARC_PC10 = 16,      // T-simm13     (S + A - P) & 0x3ff
        R_SPARC_PC22 = 17,      // V-disp22     (S + A - P) >> 10
        R_SPARC_WPLT30 = 18,    // V-disp30     (L + A - P) >> 2
        R_SPARC_COPY = 19,      // None         None
        R_SPARC_GLOB_DAT = 20,  // V-word32     S + A
        R_SPARC_JMP_SLOT = 21,  // None         See R_SPARC_JMP_SLOT,
        R_SPARC_RELATIVE = 22,  // V-word32     B + A
        R_SPARC_UA32 = 23,      // V-word32     S + A
        R_SPARC_PLT32 = 24,     // V-word32     L + A
        R_SPARC_HIPLT22 = 25,   // T-imm22      (L + A) >> 10
        R_SPARC_LOPLT10 = 26,   // T-simm13     (L + A) & 0x3ff
        R_SPARC_PCPLT32 = 27,   // V-word32     L + A - P
        R_SPARC_PCPLT22 = 28,   // V-disp22     (L + A - P) >> 10
        R_SPARC_PCPLT10 = 29,   // V-simm13     (L + A - P) & 0x3ff
        R_SPARC_10 = 30,        // V-simm10     S + A
        R_SPARC_11 = 31,        // V-simm11     S + A
        R_SPARC_OLO10 = 33,     // V-simm13     ((S + A) & 0x3ff) + O
        R_SPARC_HH22 = 34,      // V-imm22      (S + A) >>	42
        R_SPARC_HM10 = 35,      // T-simm13     ((S + A) >>	32) & 0x3ff
        R_SPARC_LM22 = 36,      // T-imm22      (S + A) >>	10
        R_SPARC_PC_HH22 = 37,   // V-imm22      (S + A - P) >>	42
        R_SPARC_PC_HM10 = 38,   // T-simm13     ((S + A - P) >>	32) & 0x3ff
        R_SPARC_PC_LM22 = 39,   // T-imm22      (S + A - P) >>	10
        R_SPARC_WDISP16 = 40,   // V-d2/disp14  (S + A - P) >> 2
        R_SPARC_WDISP19 = 41,   // V-disp19     (S + A - P) >> 2
        R_SPARC_7 = 43,         // V-imm7       S + A
        R_SPARC_5 = 44,         // V-imm5       S + A
        R_SPARC_6 = 45,         // V-imm6       S + A
        R_SPARC_HIX22 = 48,     // V-imm22      ((S + A) ^ 0xffffffffffffffff) >> 10
        R_SPARC_LOX10 = 49,     // T-simm13     ((S + A) & 0x3ff) | 0x1c00
        R_SPARC_H44 = 50,       // V-imm22      (S + A) >> 22
        R_SPARC_M44 = 51,       // T-imm10      ((S + A) >> 12) & 0x3ff
        R_SPARC_L44 = 52,       // T-imm13      (S + A) & 0xfff
        R_SPARC_REGISTER = 53,  // V-word32     S + A
        R_SPARC_UA16 = 55,      // V-half16     S + A

        // On Sparc64:
        //R_SPARC_HI22 9 V-imm22 (S + A) >> 10
        //R_SPARC_GLOB_DAT 20 V-xword64 S + A
        //R_SPARC_RELATIVE 22 V-xword64 B + A
        //R_SPARC_64 32 V-xword64 S + A
        //R_SPARC_OLO10 33 V-simm13 ((S + A) & 0x3ff) + O
        //R_SPARC_DISP64 46 V-xword64 S+A-P
        //R_SPARC_PLT64 47 V-xword64 L + A
        //R_SPARC_REGISTER 53 V-xword64 S + A
        //R_SPARC_UA64 54 V-xword64 S + A
        //R_SPARC_H34 85 V-imm22
    }
}

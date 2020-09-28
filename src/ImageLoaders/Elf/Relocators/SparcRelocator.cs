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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    // https://docs.oracle.com/cd/E23824_01/html/819-0690/chapter6-1235.html
    public class SparcRelocator : ElfRelocator32
    {
        private Dictionary<Address,ImportReference> importReferences;

        public SparcRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override void Relocate(Program program)
        {
            this.importReferences=  program.ImportReferences;
            base.Relocate(program);
        }

        public override ElfSymbol RelocateEntry(Program program, ElfSymbol sym, ElfSection referringSection, ElfRelocation rela)
        {
            if (loader.Sections.Count <= sym.SectionIndex)
                return sym;
            var rt = (SparcRt)(rela.Info & 0xFF);
            if (sym.SectionIndex == 0)
            {
                if (rt == SparcRt.R_SPARC_GLOB_DAT ||
                    rt == SparcRt.R_SPARC_JMP_SLOT)
                {
                    var addrPfn = Address.Ptr32((uint)rela.Offset);
                    Debug.Print("Import reference {0} - {1}", addrPfn, sym.Name);
                    var st = ElfLoader.GetSymbolType(sym);
                    if (st.HasValue)
                    {
                        importReferences[addrPfn] = new NamedImportReference(addrPfn, null, sym.Name, st.Value);
                    }
                    return sym;
                }
            }

            var symSection = loader.Sections[(int)sym.SectionIndex];
            uint S = (uint)sym.Value + symSection.Address.ToUInt32();
            int A = 0;
            int sh = 0;
            uint mask = ~0u;
            Address addr;
            if (referringSection != null)
            {
                addr = referringSection.Address + rela.Offset;
            }
            else
            {
                addr = Address.Ptr32((uint)rela.Offset);
            }
            uint P = (uint)addr.ToLinear();
            uint PP = P;
            uint B = 0;

            Debug.Print("  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2} {5}",
                rela.Offset,
                (SparcRt)(rela.Info & 0xFF),
                sym.Name,
                rela.Addend,
                (int)(rela.Info >> 8),
                "section?");

            switch (rt)
            {
            case 0:
                return sym;
            case SparcRt.R_SPARC_HI22:
                A = (int)rela.Addend;
                sh = 10;
                P = 0;
                break;
            case SparcRt.R_SPARC_LO10:
                A = (int)rela.Addend;
                mask = 0x3FF;
                P = 0;
                break;
            case SparcRt.R_SPARC_32:
                A = (int)rela.Addend;
                mask = 0xFFFFFFFF;
                P = 0;
                break;
            case SparcRt.R_SPARC_WDISP30:
                A = (int)rela.Addend;
                P = ~P + 1;
                sh = 2;
                break;
            case SparcRt.R_SPARC_RELATIVE:
                A = (int)rela.Addend;
                B = program.SegmentMap.BaseAddress.ToUInt32();
                break;
            case SparcRt.R_SPARC_COPY:
                Debug.Print("Relocation type {0} not handled yet.", rt);
                return sym;
            default:
                throw new NotImplementedException(string.Format(
                    "SPARC ELF relocation type {0} not implemented yet.",
                    rt));
            }
            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);

            var w = relR.ReadBeUInt32();
            w += ((uint)(B + S + A + P) >> sh) & mask;
            relW.WriteBeUInt32(w);

            return sym;
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
    }
}

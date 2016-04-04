#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

namespace Reko.ImageLoaders.Elf
{
    public class SparcRelocator : ElfRelocator
    {
        private ElfLoader32 loader;

        public SparcRelocator(ElfLoader32 loader)
        {
            this.loader = loader;
        }

        public override void Relocate(Program program)
        {
            DumpRela32(loader);
            foreach (var relSection in loader.SectionHeaders.Where(s => s.sh_type == SectionHeaderType.SHT_RELA))
            {
                var symbols = LoadSymbols(relSection.sh_link);
                var referringSection = loader.SectionHeaders[(int)relSection.sh_info];
                var rdr = loader.CreateReader(relSection.sh_offset);
                for (uint i = 0; i < relSection.sh_size / relSection.sh_entsize; ++i)
                {
                    var rela = Elf32_Rela.Read(rdr);
                    var sym = symbols[(int)(rela.r_info >> 8)];
                    if (loader.SectionHeaders.Count <= sym.SegmentIndex)
                        continue;       //$DEBUG
                    if (sym.SegmentIndex == 0)
                        continue;       //$DEBUG
                    var symSection = loader.SectionHeaders[(int)sym.SegmentIndex];
                    Debug.Print("  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2} {5}",
                        rela.r_offset,
                        (SparcRt)(rela.r_info & 0xFF),
                        symbols[(int)(rela.r_info >> 8)].Name,
                        rela.r_addend,
                        (int)(rela.r_info >> 8),
                        loader.GetSectionName(symSection.sh_name));
                    uint S = sym.Value + symSection.sh_addr;
                    int A = 0;
                    int sh = 0;
                    uint mask = ~0u;
                    var addr = Address.Ptr32(referringSection.sh_addr + rela.r_offset);
                    var relR = program.CreateImageReader(addr);
                    var relW = program.CreateImageWriter(addr);

                    var rt = (SparcRt)(rela.r_info & 0xFF);
                    switch (rt)
                    {
                    case SparcRt.R_SPARC_HI22:
                        A = rela.r_addend;
                        sh = 10;
                        break;
                    case SparcRt.R_SPARC_LO10:
                        A = rela.r_addend;
                        mask = 0x3FF;
                        break;
                    //default:
                    //    throw new NotImplementedException(string.Format(
                    //        "SPARC relocation type {0} not implemented yet.",
                    //        rt));
                    }
                    var w = relR.ReadBeUInt32();
                    w += ((uint)(S + A) >> sh) & mask;
                    relW.WriteBeUInt32(w);

                }
            }
        }

        public override List<ElfSymbol> LoadSymbols(uint iSymbolSection)
        {
            return LoadSymbols32(loader, iSymbolSection);
        }

        private string LoadString(uint symtabOffset, uint sym)
        {
            return loader.ReadAsciiString(symtabOffset + sym);
        }
    }

    public enum SparcRt
    {
        R_SPARC_NONE = 0, //none none
        R_SPARC_8 = 1, //V-byte8 S + A
        R_SPARC_16 = 2, //V-half16 S + A
        R_SPARC_32 = 3, //V-word32 S + A
        R_SPARC_DISP8 = 4, //V-byte8 S + A - P
        R_SPARC_DISP16 = 5, //V-half16 S + A - P
        R_SPARC_DISP32 = 6, //V-word32 S + A - P
        R_SPARC_WDISP30 = 7, //V-disp30 ( S + A - P ) > > 2
        R_SPARC_WDISP22 = 8, //V-disp22 ( S + A - P ) > > 2
        R_SPARC_HI22 = 9, //T-imm22 ( S + A ) > > 1 0
        R_SPARC_22 = 10, // V-imm22 S + A
        R_SPARC_13 = 11, // V-simm13 S + A
        R_SPARC_LO10 = 12, // T-simm13 ( S + A ) & 0 x 3 f f
        R_SPARC_GOT10 = 13, // T-simm13 G & 0 x 3 f f
        R_SPARC_GOT13 = 14, // V-simm13 G
        R_SPARC_GOT22 = 15, // T-imm22 G > > 1 0
        R_SPARC_PC10 = 16, // T-simm13 ( S + A - P ) & 0 x 3 f f
        R_SPARC_PC22 = 17, // V-disp22 ( S + A - P ) > > 1 0
        R_SPARC_WPLT30 = 18, // V-disp30 ( L + A - P ) > > 2
        R_SPARC_COPY = 19, // none none
        R_SPARC_GLOBDAT = 20, // V-word32 S + A
        R_SPARC_JMPSLOT = 21, // none
    }
}

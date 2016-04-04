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

using System;
using System.Collections.Generic;
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

        public override void Relocate()
        {
            DumpRela32(loader);
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

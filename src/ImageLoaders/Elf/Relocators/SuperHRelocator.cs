#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
    public class SuperHRelocator : ElfRelocator32
    {
        public SuperHRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(Program program, ElfSymbol symbol, ElfSection? referringSection, ElfRelocation rela)
        {
            var rt = (SuperHrt)(byte)rela.Info;
            if (rt == SuperHrt.R_SH_GLOB_DAT ||
                rt == SuperHrt.R_SH_JMP_SLOT)
            {
                var addrPfn = Address.Ptr32((uint)rela.Offset);
                Debug.Print("Import reference {0} - {1}", addrPfn, symbol.Name);
                var st = ElfLoader.GetSymbolType(symbol);
                if (st.HasValue)
                {
                    program.ImportReferences[addrPfn] = new NamedImportReference(addrPfn, null, symbol.Name, st.Value);
                }
                return (addrPfn, null);
            }
            return (null, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((SuperHrt)type).ToString();
        }
    }

    [Flags]
    public enum SuperHFlags
    {
        EF_MODEL_MASK = 0xF,

        EF_SH1 = 1,
        EF_SH2 = 2,
        EF_SH3 = 3,
        EF_SH_DSP = 4,
        EF_SH3_DSP = 5,
        EF_SH4AL_DSP = 6,
        EF_SH3E = 8,
        EF_SH4 = 9,
        EF_SH2E = 11,
        EF_SH4A = 12,
        EF_SH2A = 13,

        EF_SH4_NOFPU = 16,
        EF_SH4A_NOFPU = 17,
        EF_SH4_NOMMU_NOFPU = 18,
        EF_SH2A_NOFPU = 19,
        EF_SH3_NOMMU = 20,

        EF_SH2A_SH4_NOFPU = 21,
        EF_SH2A_SH3_NOFPU = 22,
        EF_SH2A_SH4 = 23,
        EF_SH2A_SH3E = 24,
        EF_SH5 = 10,
    }
    public enum SuperHrt
    {
        R_SH_NONE = 0,
        R_SH_DIR32 = 1,
        R_SH_REL32 = 2,
        R_SH_DIR8WPN = 3,
        R_SH_IND12W = 4,
        R_SH_DIR8WPL = 5,
        R_SH_DIR8WPZ = 6,
        R_SH_DIR8BP = 7,
        R_SH_DIR8W = 8,
        R_SH_DIR8L = 9,
        R_SH_SWITCH16 = 25,
        R_SH_SWITCH32 = 26,
        R_SH_USES = 27,
        R_SH_COUNT = 28,
        R_SH_ALIGN = 29,
        R_SH_CODE = 30,
        R_SH_DATA = 31,
        R_SH_LABEL = 32,
        R_SH_SWITCH8 = 33,
        R_SH_GNU_VTINHERIT = 34,
        R_SH_GNU_VTENTRY = 35,
        R_SH_TLS_GD_32 = 144,
        R_SH_TLS_LD_32 = 145,
        R_SH_TLS_LDO_32 = 146,
        R_SH_TLS_IE_32 = 147,
        R_SH_TLS_LE_32 = 148,
        R_SH_TLS_DTPMOD32 = 149,
        R_SH_TLS_DTPOFF32 = 150,
        R_SH_TLS_TPOFF32 = 151,
        R_SH_GOT32 = 160,
        R_SH_PLT32 = 161,
        R_SH_COPY = 162,
        R_SH_GLOB_DAT = 163,
        R_SH_JMP_SLOT = 164,
        R_SH_RELATIVE = 165,
        R_SH_GOTOFF = 166,
        R_SH_GOTPC = 167,
    }
}

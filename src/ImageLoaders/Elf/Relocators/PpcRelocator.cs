#region License
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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class PpcRelocator : ElfRelocator32
    {
        public PpcRelocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        /// <remarks>
        /// According to the ELF PPC32 documentation, the .rela.plt and .plt tables 
        /// should contain the same number of entries, even if the individual entry 
        /// sizes are distinct. The entries in .real.plt refer to symbols while the
        /// entries in .plt are (writeable) pointers.  Any caller that jumps to one
        /// of pointers in the .plt table is a "trampoline", and should be replaced
        /// in the decompiled code with just a call to the symbol obtained from the
        /// .real.plt section.
        /// </remarks>
        public override void Relocate(Program program)
        {
            base.Relocate(program);
            var rela_plt = loader.GetSectionInfoByName(".rela.plt");
            if (rela_plt == null)
                return;
            var plt = loader.GetSectionInfoByName(".plt");
            var relaRdr = loader.CreateReader(rela_plt.FileOffset);
            var pltRdr = loader.CreateReader(plt.FileOffset);
            for (int i = 0; i < rela_plt.EntryCount(); ++i)
            {
                // Read the .rela.plt entry
                uint offset;
                if (!relaRdr.TryReadUInt32(out offset))
                    return;
                uint info;
                if (!relaRdr.TryReadUInt32(out info))
                    return;
                int addend;
                if (!relaRdr.TryReadInt32(out addend))
                    return;

                // Read the .plt entry. We don't care about its contents,
                // only its address. Anyone accessing that address is
                // trying to access the symbol.

                uint thunkAddress;
                if (!pltRdr.TryReadUInt32(out thunkAddress))
                    break;

                uint sym = info >> 8;
                string symStr = loader.GetSymbolName(rela_plt.LinkedSection, sym);

                var addr = plt.Address + (uint)i * 4;
                program.ImportReferences.Add(
                    addr,
                    new NamedImportReference(addr, null, symStr));
            }
        }

        public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, Elf32_Rela rela)
        {
            switch ((PpcRt)(rela.r_info & 0xFF))
            {
            case PpcRt.R_PPC_GLOB_DAT:
            case PpcRt.R_PPC_COPY:
            case PpcRt.R_PPC_JMP_SLOT:
                break;
            default:
                throw new NotImplementedException();
            }
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((PpcRt)type).ToString();
        }
    }

    public class PpcRelocator64 : ElfRelocator64
    {
        public PpcRelocator64(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
            this.loader = loader;
        }

        public override void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, Elf64_Rela rela)
        {
            throw new NotImplementedException();
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((PpcRt)type).ToString();
        }
    }

    public enum PpcRt
    {
        R_PPC_NONE = 0,                 // none none
        R_PPC_ADDR32 = 1,               // word32 S + A
        R_PPC_ADDR24 = 2,               // low24* (S + A) >> 2
        R_PPC_ADDR16 = 3,               // half16* S + A
        R_PPC_ADDR16_LO = 4,            // half16 #lo(S + A)
        R_PPC_ADDR16_HI = 5,            // half16 #hi(S + A)
        R_PPC_ADDR16_HA = 6,            // half16 #ha(S + A)
        R_PPC_ADDR14 = 7,               // low14* (S + A) >> 2
        R_PPC_ADDR14_BRTAKEN = 8,       // low14* (S + A) >> 2
        R_PPC_ADDR14_BRNTAKEN = 9,      // low14* (S + A) >> 2
        R_PPC_REL24 = 10,               // low24* (S + A - P) >> 2
        R_PPC_REL14 = 11,               // low14* (S + A - P) >> 2
        R_PPC_REL14_BRTAKEN = 12,       // low14* (S + A - P) >> 2
        R_PPC_REL14_BRNTAKEN = 13,      // low14* (S + A - P) >> 2
        R_PPC_GOT16 = 14,               // half16* G + A
        R_PPC_GOT16_LO = 15,            // half16 #lo(G + A)
        R_PPC_GOT16_HI = 16,            // half16 #hi(G + A)
        R_PPC_GOT16_HA = 17,            // half16 #ha(G + A)
        R_PPC_PLTREL24 = 18,            // low24* (L + A - P) >> 2
        R_PPC_COPY = 19,                // none none
        R_PPC_GLOB_DAT = 20,            // word32 S + A
        R_PPC_JMP_SLOT = 21,            // none see below
        R_PPC_RELATIVE = 22,            // word32 B + A
        R_PPC_LOCAL24PC = 23,           // low24* see below
        R_PPC_UADDR32 = 24,             // word32 S + A
        R_PPC_UADDR16 = 25,             // half16* S + A
        R_PPC_REL32 = 26,               // word32 S + A - P
        R_PPC_PLT32 = 27,               // word32 L + A
    }
}

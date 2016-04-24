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
    public abstract class ElfRelocator
    {
        public abstract void Relocate(Program program);
        public abstract string RelocationTypeToString(uint type);
    }

    public abstract class ElfRelocator32 : ElfRelocator
    {
        private ElfLoader32 loader;

        public ElfRelocator32(ElfLoader32 loader)
        {
            this.loader = loader;
        }

        public override void Relocate(Program program)
        {
            DumpRela32(loader);
            foreach (var relSection in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                var symbols = loader.Symbols[relSection.LinkedSection];
                var referringSection = relSection.RelocatedSection;
                var rdr = loader.CreateReader(relSection.FileOffset);
                for (uint i = 0; i < relSection.EntryCount(); ++i)
                {
                    var rela = Elf32_Rela.Read(rdr);
                    var sym = symbols[(int)(rela.r_info >> 8)];
                    RelocateEntry(program, sym, referringSection, rela);
                }
            }
        }

        public abstract void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, Elf32_Rela rela);

        [Conditional("DEBUG")]
        protected void DumpRela32(ElfLoader32 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                Debug.Print("RELA: offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection.Name,
                    section.RelocatedSection.Name);

                var symbols = loader.Symbols[section.LinkedSection];
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rela = Elf32_Rela.Read(rdr);
                    Debug.Print("  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2}",
                        rela.r_offset,
                        RelocationTypeToString(rela.r_info & 0xFF),
                        symbols[(int)(rela.r_info >> 8)].Name,
                        rela.r_addend,
                        (int)(rela.r_info >> 8));
                }
            }
        }
    }

    public abstract class ElfRelocator64 : ElfRelocator
    {
        ElfLoader64 loader;

        public ElfRelocator64(ElfLoader64 loader)
        {
            this.loader = loader;
        }

        public override void Relocate(Program program)
        {
            DumpRela64(loader);
            foreach (var relSection in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                var symbols = loader.Symbols[relSection.LinkedSection];
                var referringSection = relSection.RelocatedSection;
                var rdr = loader.CreateReader(relSection.FileOffset);
                for (uint i = 0; i < relSection.EntryCount(); ++i)
                {
                    var rela = Elf64_Rela.Read(rdr);
                    var sym = symbols[(int)(rela.r_info >> 32)];
                    RelocateEntry(program, sym, referringSection, rela);
                }
            }
        }

        public abstract void RelocateEntry(Program program, ElfSymbol symbol, ElfSection referringSection, Elf64_Rela rela);
    
        [Conditional("DEBUG")]
        protected void DumpRela64(ElfLoader64 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                Debug.Print("RELA: offset {0:X} symbol section {1}, relocating in section {2}",
                    section.FileOffset,
                    section.LinkedSection.Name,
                    section.RelocatedSection.Name);

                var symbols = loader.Symbols[section.LinkedSection];
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
                {
                    var rela = Elf64_Rela.Read(rdr);
                    Debug.Print("  off:{0:X16} type:{1,-16} add:{3,-20} {4,3} {2}",
                        rela.r_offset,
                        RelocationTypeToString((uint)rela.r_info),
                        symbols[(int)(rela.r_info >> 32)].Name,
                        rela.r_addend,
                        (int)(rela.r_info >> 32));
                }
            }
        }
    }
}

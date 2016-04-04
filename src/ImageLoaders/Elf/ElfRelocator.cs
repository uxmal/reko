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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public abstract class ElfRelocator
    {
        public abstract void Relocate();
        public abstract List<ElfSymbol> LoadSymbols(uint iSymbolSection);


        [Conditional("DEBUG")]
        protected void DumpRela32(ElfLoader32 loader)
        {
            foreach (var section in loader.SectionHeaders.Where(s => s.sh_type == SectionHeaderType.SHT_RELA))
            {
                Debug.Print("RELA: offset {0:X} link section {1}",
                    section.sh_offset,
                    loader.GetSectionName(loader.SectionHeaders[(int)section.sh_link].sh_name));

                var symbols = LoadSymbols(section.sh_link);
                var rdr = loader.CreateReader(section.sh_offset);
                for (uint i = 0; i < section.sh_size / section.sh_entsize; ++i)
                {
                    var rela = Elf32_Rela.Read(rdr);
                    Debug.Print("  off:{0:X8} type:{1,-16} add:{3,-20} {4,3} {2}",
                        rela.r_offset,
                        (SparcRt)(rela.r_info & 0xFF),
                        symbols[(int)(rela.r_info >> 8)].Name,
                        rela.r_addend,
                        (int)(rela.r_info >> 8));
                }
            }
        }

        protected List<ElfSymbol> LoadSymbols32(ElfLoader32 loader, uint iSymbolSection)
        {
            Debug.Print("Symbols");
            var symSection = loader.SectionHeaders[(int)iSymbolSection];
            var stringtableSection = loader.SectionHeaders[(int)symSection.sh_link];
            var rdr = loader.CreateReader(symSection.sh_offset);
            var symbols = new List<ElfSymbol>();
            for (uint i = 0; i < symSection.sh_size / symSection.sh_entsize; ++i)
            {
                var sym = Elf32_Sym.Load(rdr);
                Debug.Print("  {0,3} {1,-25} {2,-12} {3,6} {4,-15} {5:X8} {6,9}",
                    i,
                    loader.ReadAsciiString(stringtableSection.sh_offset + sym.st_name),
                    (SymbolType)(sym.st_info & 0xF),
                    sym.st_shndx,
                    SectionName(loader, sym.st_shndx),
                    sym.st_value,
                    sym.st_size);
                symbols.Add(new ElfSymbol
                {
                    Name = loader.ReadAsciiString(stringtableSection.sh_offset + sym.st_name),
                    SegmentIndex = 0
                });
            }
            return symbols;
        }

        private string SectionName(ElfLoader32 loader, ushort st_shndx)
        {
            if (st_shndx < 0xFF00)
            {
                return loader.GetSectionName(loader.SectionHeaders[(int)st_shndx].sh_name);
            }
            else
            {
                switch (st_shndx)
                {
                case 0xFFF1: return "SHN_ABS";
                case 0xFFF2: return "SHN_COMMON";
                default: return st_shndx.ToString("X4");
                }
            }
        }
    }
}

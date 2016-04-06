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
using Reko.Core;

namespace Reko.ImageLoaders.Elf
{
    public abstract class ElfRelocator
    {
        public abstract void Relocate(Program program);

        [Conditional("DEBUG")]
        protected void DumpRela32(ElfLoader32 loader)
        {
            foreach (var section in loader.Sections.Where(s => s.Type == SectionHeaderType.SHT_RELA))
            {
                Debug.Print("RELA: offset {0:X} link section {1}",
                    section.FileOffset,
                    section.LinkedSection.Name);

                var symbols = loader.Symbols[section];
                var rdr = loader.CreateReader(section.FileOffset);
                for (uint i = 0; i < section.EntryCount(); ++i)
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
    }
}

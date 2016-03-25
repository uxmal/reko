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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    /// <summary>
    /// If we are dealing with an object file, we need to link it.
    /// </summary>
    // https://sourceware.org/gdb/onlinedocs/stabs/Stab-Section-Basics.html#Stab-Section-Basics
    public abstract class ElfObjectLinker
    {
        private ElfLoader loader;

        public ElfObjectLinker(ElfLoader loader)
        {
            this.loader = loader;
        }

        public Program LinkObject(IPlatform platform, byte[] rawImage)
        {
            throw new NotImplementedException();
        }
#if NYI
        public List<ElfSymbol> LoadSymbols(Elf32_SHdr section)
        {
            var strTable = loader.SectionHeaders[(int)section.sh_link];
            uint nSymbols = section.sh_size / section.sh_entsize;
            var rdr = loader.CreateReader(section.sh_offset);
            var list = new List<ElfSymbol>();
            for (int iSymbol = 0; iSymbol < nSymbols; ++iSymbol)
            {
                var iName = rdr.ReadUInt32();
                var rdrName = loader.CreateReader(strTable.sh_offset + iName);
                var name = rdrName.ReadCString(PrimitiveType.Char, Encoding.UTF8);

                var value = rdr.ReadUInt32();
                var size = rdr.ReadUInt32();
                var info = rdr.ReadByte();
                rdr.ReadByte();         // skip unused st_other
                var iSegment = rdr.ReadByte();

                list.Add(new ElfSymbol
                {
                    Name = name.ToString(),
                    Value = value,
                    SegmentIndex = iSegment,
                    Info = info,
                });
            }
            return list;
        }
#endif
    }

    public class ElfObjectLinker64 : ElfObjectLinker
    {
        public ElfObjectLinker64(ElfLoader64 loader) : base(loader)
        { }

    }

    public class ElfObjectLinker32 : ElfObjectLinker
    {
        public ElfObjectLinker32(ElfLoader32 loader) : base(loader)
        { }

    }
}

#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class SymtabSegmentRenderer : ImageMapSegmentRenderer
    {
        private ElfImageLoader loader;
        private Elf32_SHdr shdr;

        public SymtabSegmentRenderer(ElfImageLoader loader, Elf32_SHdr shdr)
        {
            this.loader = loader;
            this.shdr = shdr;
        }

        public override void Render(ImageMapSegment segment, Program program, Formatter formatter)
        {
            var entries = shdr.sh_size / shdr.sh_entsize;
            var symtab = (int)shdr.sh_link;
            var rdr = loader.CreateReader(shdr.sh_offset);
            for (int i = 0; i < entries; ++i)
            {
                uint iName;
                if (!rdr.TryReadUInt32(out iName))
                    return;
                uint value;
                if (!rdr.TryReadUInt32(out value))
                    return;
                uint size;
                if (!rdr.TryReadUInt32(out size))
                    return;
                byte info;
                if (!rdr.TryReadByte(out info))
                    return;
                byte other;
                if (!rdr.TryReadByte(out other))
                    return;
                ushort shIndex;
                if (!rdr.TryReadUInt16(out shIndex))
                    return;
                string symStr = loader.GetStrPtr(symtab, iName);
                string segName = loader.GetSectionName(shIndex);
                formatter.Write("{0,-40} {1:X8} {2:X8} {3:X2} {4}", symStr, value, size, info & 0xFF, segName);
                formatter.WriteLine();
            }
        }

    }
}

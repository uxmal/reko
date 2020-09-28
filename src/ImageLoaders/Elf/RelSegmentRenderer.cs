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
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class RelSegmentRenderer : ImageSegmentRenderer
    {
        private ElfLoader32 loader;
        private ElfSection shdr;

        public RelSegmentRenderer(ElfLoader32 loader, ElfSection shdr)
        {
            this.loader = loader;
            this.shdr = shdr;
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            var entries = shdr.Size / shdr.EntrySize;
            var symtab = shdr.LinkedSection;
            var rdr = loader.CreateReader(shdr.FileOffset);
            for (ulong i = 0; i < entries; ++i)
            {
                uint offset;
                if (!rdr.TryReadUInt32(out offset))
                    return;
                uint info;
                if (!rdr.TryReadUInt32(out info))
                    return;

                uint sym = info >> 8;
                string symStr = loader.GetSymbolName(symtab, sym);
                formatter.Write("{0:X8} {1,3} {2:X8} {3}", offset, info & 0xFF, sym, symStr);
                formatter.WriteLine();
            }
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
    public class RelaSegmentRenderer : ImageSegmentRenderer
    {
        private ElfLoader32 loader;
        private ElfSection shdr;

        public RelaSegmentRenderer(ElfLoader32 imgLoader, ElfSection shdr)
        {
            this.loader = imgLoader;
            this.shdr = shdr;
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            var entries = shdr.Size / shdr.EntrySize;
            var symtab = shdr.LinkedSection!;
            var rdr = loader.CreateReader(shdr.FileOffset);
            for (ulong i = 0; i < entries; ++i)
            {
                if (!rdr.TryReadUInt32(out uint offset))
                    return;
                if (!rdr.TryReadUInt32(out uint info))
                    return;
                if (!rdr.TryReadInt32(out int addend))
                    return;

                uint sym = info >> 8;
                string symStr = loader.GetSymbolName(symtab, sym);
                formatter.Write("{0:X8} {1,3} {2:X8} {3:X8} {4} ({5})", offset, info & 0xFF, sym, addend, symStr, sym);
                formatter.WriteLine();
            }
        }
    }

    public class RelaSegmentRenderer64 : ImageSegmentRenderer
    {
        private ElfLoader64 loader;
        private ElfSection shdr;

        public RelaSegmentRenderer64(ElfLoader64 loader, ElfSection shdr)
        {
            this.loader = loader;
            this.shdr = shdr;
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            var entries = shdr.EntryCount();
            var symtab = shdr.LinkedSection!;
            if (symtab.FileOffset == 0)
                return;
            var rdr = loader.CreateReader(shdr.FileOffset);
            for (ulong i = 0; i < entries; ++i)
            {
                if (!rdr.TryReadUInt64(out ulong offset))
                    return;
                if (!rdr.TryReadUInt64(out ulong info))
                    return;
                if (!rdr.TryReadUInt64(out ulong addend))
                    return;

                ulong sym = info >> 32;
                string symStr = loader.GetSymbol64(symtab, sym);
                formatter.Write("{0:X8} {1,3} {2:X8} {3:X16} {4} ({5})", offset, info & 0xFFFFFFFF, sym, addend, symStr, sym);
                formatter.WriteLine();
            }
        }
    }
}

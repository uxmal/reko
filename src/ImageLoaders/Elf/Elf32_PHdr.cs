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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class Elf32_PHdr
    {
        public ProgramHeaderType p_type;
        public uint p_offset;
        public uint p_vaddr;
        public uint p_paddr;
        public uint p_filesz;
        public uint p_pmemsz;
        public uint p_flags;
        public uint p_align;

        public const int Size = 32;

        public static Elf32_PHdr Load(EndianImageReader rdr)
        {
            var hdr = new Elf32_PHdr
            {
                p_type = (ProgramHeaderType)rdr.ReadUInt32(),
                p_offset = rdr.ReadUInt32(),
                p_vaddr = rdr.ReadUInt32(),
                p_paddr = rdr.ReadUInt32(),
                p_filesz = rdr.ReadUInt32(),
                p_pmemsz = rdr.ReadUInt32(),
                p_flags = rdr.ReadUInt32(),
                p_align = rdr.ReadUInt32(),
            };
            return hdr;
        }
    }

    public class Elf64_PHdr
    {
        public ProgramHeaderType p_type;
        public uint p_flags;
        public ulong p_offset;
        public ulong p_vaddr;
        public ulong p_paddr;
        public ulong p_filesz;
        public ulong p_pmemsz;
        public ulong p_align;

        public static Elf64_PHdr Load(EndianImageReader rdr)
        {
            var hdr = new Elf64_PHdr
            {
                p_type = (ProgramHeaderType)rdr.ReadUInt32(),
                p_flags = rdr.ReadUInt32(),
                p_offset = rdr.ReadUInt64(),
                p_vaddr = rdr.ReadUInt64(),
                p_paddr = rdr.ReadUInt64(),
                p_filesz = rdr.ReadUInt64(),
                p_pmemsz = rdr.ReadUInt64(),
                p_align = rdr.ReadUInt64(),
            };
            return hdr;
        }
    }
}
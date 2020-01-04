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
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class Elf32_EHdr 
    {
        public ushort e_type;
        public ushort e_machine;
        public uint e_version;

        public uint e_entry;            // Entry address
        public uint e_phoff;            // Program header offset
        public uint e_shoff;            // Section table offset

        public uint e_flags;
        public ushort e_ehsize;
        public ushort e_phentsize;      // Program header entry size
        public ushort e_phnum;          // Number of program header entries.
        public ushort e_shentsize;      // Section header size
        public ushort e_shnum;          // Number of section header entries.
        public ushort e_shstrndx;       // section name string table index

        public static Elf32_EHdr Load(EndianImageReader rdr)
        {
            return new Elf32_EHdr
            {
                e_type = rdr.ReadUInt16(),
                e_machine = rdr.ReadUInt16(),
                e_version = rdr.ReadUInt32(),
                e_entry = rdr.ReadUInt32(),
                e_phoff = rdr.ReadUInt32(),
                e_shoff = rdr.ReadUInt32(),
                e_flags = rdr.ReadUInt32(),
                e_ehsize = rdr.ReadUInt16(),
                e_phentsize = rdr.ReadUInt16(),
                e_phnum = rdr.ReadUInt16(),
                e_shentsize = rdr.ReadUInt16(),
                e_shnum = rdr.ReadUInt16(),
                e_shstrndx = rdr.ReadUInt16(),
            };
        }
    }

    public class Elf64_EHdr
    {
        public ushort e_type;
        public ushort e_machine;
        public uint e_version;

        public ulong e_entry;            // Entry address
        public ulong e_phoff;            // Program header offset
        public ulong e_shoff;            // Section table offset

        public uint e_flags;
        public ushort e_ehsize;
        public ushort e_phentsize;      // Program header entry size
        public ushort e_phnum;          // Number of program header entries.
        public ushort e_shentsize;      // Section header size
        public ushort e_shnum;          // Number of section header entries.
        public ushort e_shstrndx;       // section name string table index

        public static Elf64_EHdr Load(EndianImageReader rdr)
        {
            return new Elf64_EHdr
            {
                e_type = rdr.ReadUInt16(),
                e_machine = rdr.ReadUInt16(),
                e_version = rdr.ReadUInt32(),

                e_entry = rdr.ReadUInt64(),
                e_phoff = rdr.ReadUInt64(),
                e_shoff = rdr.ReadUInt64(),

                e_flags = rdr.ReadUInt32(),
                e_ehsize = rdr.ReadUInt16(),
                e_phentsize = rdr.ReadUInt16(),
                e_phnum = rdr.ReadUInt16(),
                e_shentsize = rdr.ReadUInt16(),
                e_shnum = rdr.ReadUInt16(),
                e_shstrndx = rdr.ReadUInt16(),
            };
        }
    }
}

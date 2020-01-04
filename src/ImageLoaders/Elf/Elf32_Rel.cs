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
    public class Elf32_Rel
    {
        public uint r_offset;
        public uint r_info;

        public static Elf32_Rel Read(EndianImageReader rdr)
        {
            var o = rdr.ReadUInt32();
            var i = rdr.ReadUInt32();
            return new Elf32_Rel
            {
                r_offset = o,
                r_info = i,
            };
        }
    }

    public class Elf32_Rela
    {
        public uint r_offset;
        public uint r_info;
        public int r_addend;

        public int SymbolIndex => (int)(r_info >> 8);

        public static Elf32_Rela Read(EndianImageReader rdr)
        {
            var o = rdr.ReadUInt32();
            var i = rdr.ReadUInt32();
            var a = rdr.ReadInt32();
            return new Elf32_Rela
            {
                r_offset = o,
                r_info = i,
                r_addend = a
            };
        }
    }


    public class Elf64_Rela
    {
        public ulong r_offset;
        public ulong r_info;
        public long r_addend;

        public ulong SymbolIndex => r_info >> 8;

        public static Elf64_Rela Read(EndianImageReader rdr)
        {
            var o = rdr.ReadUInt64();
            var i = rdr.ReadUInt64();
            var a = rdr.ReadInt64();
            return new Elf64_Rela
            {
                r_offset = o,
                r_info = i,
                r_addend = a
            };
        }
    }
}

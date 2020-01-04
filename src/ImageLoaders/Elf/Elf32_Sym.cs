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
    public class Elf32_Sym
    {
        public const int Size = 16; // byte size in image.

        public uint st_name;
        public uint st_value;
        public uint st_size;
        public byte st_info;
        public byte st_other;
        public ushort st_shndx;

        public static Elf32_Sym Load(EndianImageReader rdr)
        {
            var sym = new Elf32_Sym();
            sym.st_name = rdr.ReadUInt32();
            sym.st_value = rdr.ReadUInt32();
            sym.st_size = rdr.ReadUInt32();
            sym.st_info = rdr.ReadByte();
            sym.st_other = rdr.ReadByte();
            sym.st_shndx = rdr.ReadUInt16();
            return sym;
        }
    }

    public class Elf64_Sym
    {
        public const int Size = 24; // byte size in image.

        public uint st_name;
        public byte st_info;
        public byte st_other;
        public ushort st_shndx;
        public ulong st_value;
        public ulong st_size;

        public static Elf64_Sym Load(EndianImageReader rdr)
        {
            var sym = new Elf64_Sym();
            sym.st_name = rdr.ReadUInt32();
            sym.st_info = rdr.ReadByte();
            sym.st_other = rdr.ReadByte();
            sym.st_shndx = rdr.ReadUInt16();
            sym.st_value = rdr.ReadUInt64();
            sym.st_size = rdr.ReadUInt64();
            return sym;
        }
    }


    public enum ElfSymbolType
    {
        STT_NOTYPE = 0,
        STT_OBJECT = 1,
        STT_FUNC = 2,
        STT_SECTION = 3,
        STT_FILE = 4,
        STT_LOPROC = 13,
        STT_HIPROC = 15,
    }
}
#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Memory;
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

        public static bool TryLoad(EndianImageReader rdr, out Elf32_Sym sym)
        {
            sym = new Elf32_Sym();
            if (
                rdr.TryReadUInt32(out sym.st_name) &&
                rdr.TryReadUInt32(out sym.st_value) &&
                rdr.TryReadUInt32(out sym.st_size) &&
                rdr.TryReadByte(out sym.st_info) &&
                rdr.TryReadByte(out sym.st_other) &&
                rdr.TryReadUInt16(out sym.st_shndx))
            {
                return true;
            }
            else
            {
                sym = null!;
                return false;
            }
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

        public static bool TryLoad(EndianImageReader rdr, out Elf64_Sym sym)
        {
            sym = new Elf64_Sym();
            if (
                rdr.TryReadUInt32(out sym.st_name) &&
                rdr.TryReadByte(out sym.st_info) &&
                rdr.TryReadByte(out sym.st_other) &&
                rdr.TryReadUInt16(out sym.st_shndx) &&
                rdr.TryReadUInt64(out sym.st_value) &&
                rdr.TryReadUInt64(out sym.st_size))
            {
                return true;
            }
            else
            {
                sym = null!;
                return false;
            }
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

    public enum ElfSymbolBinding
    {
        STB_LOCAL = 0,  // Not visible outside object file where defined
        STB_GLOBAL = 1, // Visible to all object files. Multiple definitions cause errors.
                        // Force extraction of defining object from archive file.
        STB_WEAK  = 2,  // Visible to all object files. Ignored if
                        // STB_GLOBAL with same name found. Do
                        // not force extraction of defining object from
                        // archive file. Value is 0 if undefined
        STB_GNU_UNIQUE = 10 // GNU extension. Variant of STB_GLOBAL for template static data
    }
}
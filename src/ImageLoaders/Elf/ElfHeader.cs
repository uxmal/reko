#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Types;

namespace Reko.ImageLoaders.Elf
{
    public class ElfHeader : IBinaryHeader
    {
        internal byte fileClass;
        internal byte endianness;
        internal byte fileVersion;
        internal byte osAbi;
        internal byte abiVersion;
        internal ulong e_phoff;
        internal int e_phnum;
        internal ulong e_shoff;
        internal uint e_shnum;
        internal int e_shstrndx;

        public ElfHeader()
        {
            this.Architecture = default!;
            this.BaseAddress = default!;
            this.StartAddress = default!;
            this.PointerType = PrimitiveType.Ptr32;
        }

        public string Architecture { get; set; }

        public ElfMachine Machine { get; set; }

        public BinaryFileType BinaryFileType { get; set; }

        public ulong Flags { get; set; }

        public Address BaseAddress { get; set; }

        public Address StartAddress { get; set; }

        public PrimitiveType PointerType { get; set; }
    }
}

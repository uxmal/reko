#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Collections.Generic;

namespace Reko.ImageLoaders.Elf
{
    public class ElfSegment : IBinarySegment
    {
        public ProgramHeaderType p_type;

        public ElfSegment()
        {
            this.VirtualAddress = default!;
            this.PhysicalAddress = default!;
            this.Sections = new();
        }

        uint IBinarySegment.Type => (uint) p_type;

        public AccessMode AccessMode =>
            (AccessMode) (Flags & 7);

        public ulong FileOffset { get; set; }

        public Address VirtualAddress { get; set; }

        public Address PhysicalAddress { get; set; }

        public ulong FileSize { get; set; }

        public ulong MemorySize { get; set; }

        public ulong Flags { get; set; }

        public ulong Alignment { get; set; }

        public bool IsValidAddress(ulong uAddr)
        {
            return VirtualAddress.Offset <= uAddr && uAddr < VirtualAddress.Offset + MemorySize;
        }

        IReadOnlyList<IBinarySection> IBinarySegment.Sections => this.Sections;

        public List<IBinarySection> Sections { get; }

        public override string ToString()
        {
            return $"[{VirtualAddress:X} - 0x{MemorySize:X}] - {p_type}";
        }


    }
}

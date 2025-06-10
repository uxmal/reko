#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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

namespace Reko.Scanning;

using Reko.Core;
using Reko.Core.Memory;

/// <summary>
/// A chunk represents a candidate area for scanning.
/// </summary>
/// <param name="arch"><see cref="IProcessorArchitecture"/> to use when disassembling
/// this chunk.</param>
/// <param name="address">The <see cref="Address"/> at which the chunk starts.
/// </param>
/// <param name="length">The length of the chunk in storage units.
/// </param>
public readonly struct Chunk(IProcessorArchitecture? arch, MemoryArea mem, Address address, long length)
{
    public IProcessorArchitecture? Architecture { get; } = arch;
    public MemoryArea MemoryArea { get; } = mem;
    public Address Address { get; } = address;
    public long Length { get; } = length;

    public override string ToString()
    {
        return $"(0x{Address.Offset:X}, 0x{Length:X})";
    }

    public void Deconstruct(out IProcessorArchitecture? arch, out MemoryArea mem, out Address address, out long length)
    {
        arch = Architecture;
        mem = MemoryArea;
        address = Address;
        length = Length;
    }
}

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

public readonly struct Chunk(IProcessorArchitecture? arch, Address address, long length)
{
    public IProcessorArchitecture? Architecture { get; } = arch;
    public Address Address { get; } = address;
    public long Length { get; } = length;

    public bool IsGap => Architecture is null;

    public override string ToString()
    {
        return $"(0x{Address.Offset:X}, 0x{Length:X})";
    }
}

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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Pdp.Memory;

/// <summary>
/// Data types used in the PDP architectures, which aren't multiples of 8 bits.
/// </summary>
public class PdpTypes
{
    public static PrimitiveType Word18 { get; } = PrimitiveType.CreateWord(18);
    public static PrimitiveType Word36 { get; } = PrimitiveType.CreateWord(36);
    public static PrimitiveType Int36 { get; } = PrimitiveType.Create(Domain.SignedInt, 36);
    public static PrimitiveType Word72 { get; } = PrimitiveType.CreateWord(72);

    public static PrimitiveType Ptr18 { get; } = PrimitiveType.Create(Domain.Pointer, 18);
    public static PrimitiveType Real36 { get; } = PrimitiveType.Create(Domain.Real, 36);
    public static PrimitiveType Real72 { get; } = PrimitiveType.Create(Domain.Real, 72);

    /// <summary>
    /// Parses an 18-bit PDP address from a string of octal digits.
    /// </summary>
    /// <param name="txtAddr">String to parse.</param>
    /// <param name="addr">Resulting address.</param>
    /// <returns>True if <paramref name="txtAddr"/> is a valid
    /// octal string.</returns>
    public static bool TryParseAddress(string? txtAddr, out Address addr)
    {
        if (txtAddr is null)
        {
            addr = default!;
            return false;
        }
        uint uAddr = 0;
        foreach (var c in txtAddr)
        {
            var u = (uint) (c - '0');
            if (u > 8)
            {
                addr = default!;
                return false;
            }
            uAddr = (uAddr << 3) | u;
        }
        addr = Pdp10Architecture.Ptr18(uAddr);
        return true;
    }
}

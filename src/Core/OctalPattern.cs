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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core;

/// <summary>
/// Utility class for handling octal-encoded data.
/// </summary>
public static class OctalPattern
{
    /// <summary>
    /// Converts an octal digit pattern into an array of 16-bit unsigned values by packing each group of octal digits
    /// into one word, in groups of <paramref name="digitsPerWord"/> digits.
    /// </summary>
    /// <remarks>Digits are packed from most significant to least significant position within each word. Any
    /// trailing digits that do not complete a full group are not included in the result.</remarks>
    /// <param name="octalPattern">String containing octal digits to parse. Characters outside the range '0' through '7' are ignored.</param>
    /// <param name="digitsPerWord">Number of octal digits to pack into each output word.</param>
    /// <returns>An array of packed 16-bit unsigned values created from complete digit groups in the input pattern.</returns>
    public static ushort[] ToUInt16s(string octalPattern, int digitsPerWord)
    {
        List<ushort> result = [];
        int shift = 3 * (digitsPerWord - 1);
        int w = 0;
        for (int i = 0; i < octalPattern.Length; i++)
        {
            char c = octalPattern[i];
            int digit = c - '0';
            if ((uint) digit > 7)
                continue;
            w |= (ushort) (digit << shift);
            shift -= 3;
            if (shift < 0)
            {
                result.Add((ushort)w);
                w = 0;
                shift = 3 * (digitsPerWord - 1);
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Converts an octal digit pattern into an array of 64-bit unsigned words using a fixed number of octal digits per
    /// word.
    /// </summary>
    /// <remarks>Characters outside the range '0' through '7' are ignored. Any trailing group with fewer than
    /// <paramref name="digitsPerWord"/> octal digits is not included in the result.</remarks>
    /// <param name="octalPattern">String containing the octal digit pattern to pack into words.</param>
    /// <param name="digitsPerWord">Number of octal digits packed into each output word.</param>
    /// <returns>An array of packed <see cref="ulong"/> values created from complete groups of octal digits.</returns>
    public static ulong[] ToUInt64s(string octalPattern, int digitsPerWord)
    {
        List<ulong> result = [];
        int shift = 3 * (digitsPerWord - 1);
        ulong w = 0;
        for (int i = 0; i < octalPattern.Length; i++)
        {
            char c = octalPattern[i];
            int digit = c - '0';
            if ((uint) digit > 7)
                continue;
            w |= ((ulong)digit << shift);
            shift -= 3;
            if (shift < 0)
            {
                result.Add(w);
                w = 0;
                shift = 3 * (digitsPerWord - 1);
            }
        }
        return result.ToArray();

    }
}

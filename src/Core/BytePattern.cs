#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Text;

namespace Reko.Core
{
    public static class BytePattern
    {
        /// <summary>
        /// Maps ASCII code points - '0' to the corresponding digit value.
        /// </summary>
        private static readonly byte[] hexValues = new byte[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 10, 11, 12, 13, 14, 15, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 10, 11, 12, 13, 14, 15
        };

        /// <summary>
        /// Generate an array of bytes from a string of hexadecimal digits.
        /// </summary>
        /// <param name="sBytes">String containing hexadecimal digits. Non-hexadecimal characters 
        /// are ignored.</param>
        /// <returns>The pattern as an array of bytes.
        /// </returns>
        public static byte[] FromHexBytes(string sBytes)
        {
            int shift = 4;
            int bb = 0;
            var bytes = new List<byte>();
            for (int i = 0; i < sBytes.Length; ++i)
            {
                char c = sBytes[i];
                if (TryParseHexDigit(c, out byte b))
                {
                    bb |= (b << shift);
                    shift -= 4;
                    if (shift < 0)
                    {
                        bytes.Add((byte) bb);
                        shift = 4;
                        bb = 0;
                    }
                }
            }
            return bytes.ToArray();
        }

        public static bool TryParseHexDigit(char c, out byte b)
        {
            int i = c - '0';
            if (0 <= i && i < hexValues.Length)
            {
                b = hexValues[i];
                if (b <= 15)
                    return true;
            }
            b = 0;
            return false;
        }

        public static bool IsHexDigit(char c)
        {
            int i = c - '0';
            return 0 <= i && i < hexValues.Length && hexValues[i] <= 15;
        }
    }
}

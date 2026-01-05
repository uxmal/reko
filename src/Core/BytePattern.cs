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

using System.Collections.Generic;

namespace Reko.Core
{
    /// <summary>
    /// Utility class for handling hexadecimal encodings of binary data.
    /// </summary>
    public static class BytePattern
    {
        /// <summary>
        /// Maps ASCII code points - '0' to the corresponding digit value.
        /// </summary>
        private static readonly byte[] hexValues = new byte[]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
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

        /// <summary>
        /// Parses a single hexadecimal digit and returns its value.
        /// </summary>
        /// <param name="c">Hexadecimal digit.</param>
        /// <param name="b">The numerical value of that digit.</param>
        /// <returns>True if the given digit was a valid hexadecimal character, 
        /// otherwise false.
        /// </returns>
        public static bool TryParseHexDigit(char c, out byte b)
        {
            int i = c - '0';
            if ((uint)i < (uint)hexValues.Length)
            {
                b = hexValues[i];
                if (b <= 15)
                    return true;
            }
            b = 0;
            return false;
        }

        /// <summary>
        /// Tests whether the given character is a valid hexadecimal digit.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>True if the given character is a valid hexadecimal digit, 
        /// otherwise false.
        /// </returns>
        public static bool IsHexDigit(char c)
        {
            int i = c - '0';
            return 0 <= i && i < hexValues.Length && hexValues[i] <= 15;
        }
    }
}

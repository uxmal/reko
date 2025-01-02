#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Collections;
using Reko.Core.Memory;
using System.Collections.Generic;

namespace Reko.Scanning
{
    public class PatternFinder
    {
        /// <summary>
        /// Given a <see cref="ByteMemoryArea"/> finds sequences of consecutive
        /// bytes corresponding to ASCII characters. The sequences must be at
        /// least <see cref="minStringLength"/> bytes long.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="minStringLength"></param>
        /// <returns></returns>
        public static List<(ulong uAddress, uint length)> FindAsciiStrings(ByteMemoryArea buffer, int minStringLength)
        {
            var strings = new List<(ulong, uint)>();

            var bytes = buffer.Bytes;
            bool insideString = false;
            uint iStart = 0;
            for (uint i = 0; i < bytes.Length; ++i)
            {
                var b = bytes[i];
                if (' ' <= b && b <= '~' || b == '\t' || b == '\r' || b == '\n')
                {
                    if (!insideString)
                    {
                        insideString = true;
                        iStart = i;
                    }
                }
                else
                {
                    uint strlen = i - iStart;
                    if (insideString && strlen >= minStringLength)
                    {
                        strings.Add((iStart, strlen));
                    }
                    insideString = false;
                }
            }
            if (insideString)
            {
                uint strlen = (uint) bytes.Length - iStart;
                if (strlen >= minStringLength)
                {
                    strings.Add((iStart, strlen));
                }
            }
            return strings;
        }

        public static List<ulong> FindProcedurePrologs(ByteMemoryArea buffer, ByteTrie<object> prologs)
        {
            var match = prologs.Match(buffer.Bytes, 0);
            var results = new List<ulong>();
            while (match.Success)
            {
                results.Add((ulong)match.Index);
                match = match.NextMatch(buffer.Bytes);
            }
            return results;
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Core
{
    public class PlatformHeuristics
    {
        public BytePattern[] ProcedurePrologs;
    }

    public class BytePattern
    {
        public byte[] Bytes;
        public byte[] Mask;

        public static bool TryParseHexDigit(char c, out byte b)
        {
            if ('0' <= c && c <= '9')
            {
                b = (byte)(c - '0');
                return true;
            }
            else if ('A' <= c && c <= 'F')
            {
                b = (byte)(c - 'A' + 10);
                return true;
            }
            else if ('a' <= c && c <= 'f')
            {
                b = (byte)(c - 'a' + 10);
                return true;
            }
            b = 0;
            return false;
        }
    }
}

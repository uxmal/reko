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
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.ImageLoaders.Srec
{
    public class SrecParser
    {
        private readonly StreamReader rdr;

        public SrecParser(byte[] rawImage)
        {
            this.rdr = new StreamReader(new MemoryStream(rawImage));
        }

        public SrecRecord? ParseRecord()
        {
            string? line;
            do
            {
                line = rdr.ReadLine();
                if (line is null)
                    return null;
            } while (line.Length < 3 || line[0] != 'S');

            var bytes = BytePattern.FromHexBytes(line.Substring(2));    //$PERF: use Span<T>.
            if (bytes.Length < 2)
                throw new BadImageFormatException("Bad line format.");
            int dataOffset;
            SrecType type;
            Address addr;
            var ch = line[1];
            switch (ch)
            {
            default:
                throw new BadImageFormatException($"Unexpected character '{(char) ch}' (U+{ch:X4}).");
            case '0':
                type = SrecType.Header;
                addr = ReadAddress(bytes, 2, u => Address.Ptr16((ushort) u));
                dataOffset = 2 + 1;
                break;
            case '1':
                type = SrecType.Data16;
                addr = ReadAddress(bytes, 2, u => Address.Ptr16((ushort)u));
                dataOffset = 2 + 1;
                break;
            case '2':
                type = SrecType.Data24;
                addr = ReadAddress(bytes, 3, Address.Ptr32);
                dataOffset = 3 + 1;
                break;
            case '3':
                type = SrecType.Data32;
                addr = ReadAddress(bytes, 4, Address.Ptr32);
                dataOffset = 1 + 4;
                break;
            case '8':
                return ParseStartAddress(bytes, 3, Addr32);
            case '9':
                return ParseStartAddress(bytes, 2, Addr16);
            }

            var nBytes = bytes.Length - dataOffset - 1;
            var data = new byte[nBytes];
            Array.Copy(bytes, dataOffset, data, 0, nBytes);
            return new SrecRecord
            {
                Type = type,
                Address = addr,
                Data = data,
            };
        }

        private SrecRecord ParseStartAddress(byte[] bytes, int cAddrBytes, Func<uint, Address> mkAddress)
        {
            var addr = ReadAddress(bytes, cAddrBytes, mkAddress);
            return new SrecRecord
            {
                Type = SrecType.StartAddress,
                Address = addr,
            };
        }

        private Address ReadAddress(byte[] bytes, int cBytes, Func<uint, Address> mkAddress)
        {
            uint u = 0;
            for (int i = 0; i < cBytes; ++i)
            {
                u = (u << 8) | bytes[i + 1];
            }
            return mkAddress(u);
        }

        private static Address Addr16(uint uAddr)
        {
            return Address.Ptr16((ushort) uAddr);
        }

        private static Address Addr32(uint uAddr)
        {
            return Address.Ptr32((ushort) uAddr);
        }
    }
}
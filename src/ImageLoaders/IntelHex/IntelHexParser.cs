#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
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
using System.Diagnostics;
using System.Linq;
using Reko.Core;
using Reko.Core.Memory;

namespace Reko.ImageLoaders.IntelHex
{
    public class IntelHexParser
    {

        private const int MinHexRecordSize = 11;

        /// <summary>
        /// Parse a single Intel Hex32 hexadecimal record.
        /// </summary>
        /// <param name="hexRecord">The hexadecimal record as a string.</param>
        /// <param name="lineNum">(Optional) The line number in the IHex32 binary stream.</param>
        /// <returns>
        /// An <see cref="IntelHexRecord"/> .
        /// </returns>
        /// <exception cref="IntelHex32Exception">Thrown whenever an error is found in the IHex32 record.</exception>
        public static IntelHexRecord ParseRecord(string hexRecord, int lineNum = 0)
        {

            List<byte> ParseHexData()
            {
                try
                {
                    return Enumerable.Range(1, hexRecord.Length - 1)
                        .Where(i => (i & 1) != 0)
                        .Select(i => Convert.ToByte(hexRecord.Substring(i, 2), 16))
                        .ToList();
                }
                catch (Exception ex)
                {
                    throw new IntelHexException($"Unable to parse hexedecimal numbers in Intel Hex line [{hexRecord}]", ex, lineNum);
                }
            }

            if (hexRecord is null)
                throw new IntelHexException("Intel Hex line can not be null.", lineNum);
            if (hexRecord.Length < MinHexRecordSize)
                throw new IntelHexException($"Intel Hex line [{hexRecord}] is less than {MinHexRecordSize} characters long.", lineNum);
            if (hexRecord.Length % 2 == 0)
                throw new IntelHexException($"Intel Hex line has an even number of characters [{hexRecord}].", lineNum);
            if (!hexRecord.StartsWith(":"))
                throw new IntelHexException($"Illegal Intel Hex line start character [{hexRecord}].", lineNum);

            var hexData = ParseHexData();

            if (hexData.Count != hexData[0] + 5)
                throw new IntelHexException($"Intel Hex line [{hexRecord}] does not have required record length of [{hexData[0] + 5}].", lineNum);

            if (!Enum.IsDefined(typeof(IntelHexRecordType), hexData[3]))
                throw new IntelHexException($"Intel Hex line has an invalid/unsupported record type value: [{hexData[3]}].", lineNum);

            if ((hexData.Sum(b => b) % 256) != 0)
                throw new IntelHexException($"Checksum for Intel Hex line [{hexRecord}] is incorrect.", lineNum);

            var rdr = new ByteImageReader(hexData.ToArray());
            var datasize = rdr.ReadByte();
            var newRecord = new IntelHexRecord
            {
                ByteCount = datasize,
                Address = rdr.ReadBeUInt16(),
                RecordType = (IntelHexRecordType)rdr.ReadByte(),
                Data = rdr.ReadBytes(datasize).ToList(),
                CheckSum = rdr.ReadByte()
            };
            Debug.WriteLine($"Addr: {newRecord.Address:X4} Type: {newRecord.RecordType}");
            return newRecord;
        }
    }
}

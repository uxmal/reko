#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
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

namespace Reko.ImageLoaders.IntelHex32
{
    public class IntelHex32Parser
    {
        /// <summary>
        /// Parse a single IHex32 hexadecimal record.
        /// </summary>
        /// <param name="hexRecord">The hexadecimal record as a string.</param>
        /// <param name="lineNum">(Optional) The line number in the IHex32 binary stream.</param>
        /// <returns>
        /// An <see cref="IntelHex32Record"/> .
        /// </returns>
        /// <exception cref="IntelHex32Exception">Thrown whenever an error is found in the IHex32 record.</exception>
        public static IntelHex32Record ParseRecord(string hexRecord, int lineNum = 0)
        {
            if (hexRecord == null)
                throw new IntelHex32Exception("Hex record line can not be null", lineNum);
            if (hexRecord.Length < 11)
                throw new IntelHex32Exception($"Hex record line length [{hexRecord}] is less than 11", lineNum);
            if (hexRecord.Length % 2 == 0)
                throw new IntelHex32Exception($"Hex record has an even number of characters [{hexRecord}]", lineNum);
            if (!hexRecord.StartsWith(":"))
                throw new IntelHex32Exception($"Illegal line start character [{hexRecord}]", lineNum);
            var hexData = TryParseHexData(hexRecord.Substring(1), lineNum);

            if (hexData.Count != hexData[0] + 5)
                throw new IntelHex32Exception($"Line [{hexRecord}] does not have required record length of [{hexData[0] + 5}]", lineNum);

            if (!Enum.IsDefined(typeof(IntelHex32RecordType), hexData[3]))
                throw new IntelHex32Exception($"Invalid record type value: [{hexData[3]}]", lineNum);

            var checkSum = hexData.Last();
            hexData.RemoveAt(hexData[0] + 4);

            if (!VerifyChecksum(hexData, checkSum))
                throw new IntelHex32Exception($"Checksum for line [{hexRecord}] is incorrect", lineNum);

            var dataSize = hexData[0];

            var newRecord = new IntelHex32Record
            {
                ByteCount = dataSize,
                Address = ((uint)(hexData[1] << 8) | hexData[2]),
                RecordType = (IntelHex32RecordType)hexData[3],
                Data = hexData,
                CheckSum = checkSum
            };

            hexData.RemoveRange(0, 4);
            return newRecord;
        }

        #region Helpers

        private static bool VerifyChecksum(IList<byte> checkSumData, int checkSum)
        {
            var maskedSumBytes = checkSumData.Sum(x => x) & 0xff;
            var calculatedChecksum = (byte)(256 - maskedSumBytes);

            return calculatedChecksum == checkSum;
        }

        private static List<byte> TryParseHexData(string hexData, int lineNum = 0)
        {
            try
            {
                return Enumerable.Range(0, hexData.Length)
                    .Where(i => (i & 1) == 0)
                    .Select(i => Convert.ToByte(hexData.Substring(i, 2), 16))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new IntelHex32Exception($"Unable to parse bytes for [{hexData}]", ex, lineNum);
            }
        }

        #endregion

    }

}

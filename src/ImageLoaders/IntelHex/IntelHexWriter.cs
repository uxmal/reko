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
using System.IO;
using System.Linq;

namespace Reko.ImageLoaders.IntelHex
{

    /// <summary>
    /// A writer capable of writing an Intel Hexadecimal 32-bit object format stream.
    /// </summary>
    public class IntelHexWriter : IDisposable
    {
        #region Locals

        private readonly StreamWriter streamWriter;
        private const string HexDigits = "0123456789ABCDEF";
        private const int maxSegAddr = 0x10000;
        private const int maxRecordDataSize = 255;

        #endregion

        #region Constructors

        /// <summary>
        ///     Construct instance of an <see cref="IntelHexWriter" />.
        /// </summary>
        /// <param name="str">The target stream of the hex file.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="str" /> is null.</exception>
        public IntelHexWriter(Stream str)
        {
            if (str is null)
                throw new ArgumentNullException(nameof(str));

            streamWriter = new StreamWriter(str);
        }

        #endregion

        #region API Methods

        /// <summary>
        /// Write an address record (type 02, 04 or 05) to the underlying stream
        /// </summary>
        /// <param name="addressType">The <see cref="IntelHex32AddressType" /> address record type to write to the stream</param>
        /// <param name="address">The address value to write to the stream. This is either the segment address (type 02) or the upper word of a 32 bit address (type 04 or 05).</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="addressType"/> is not a member of <see cref="IntelHex32AddressType"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="addressType"/> is an <see cref="IntelHex32AddressType.ExtendedSegmentAddress"/> and <paramref name="address"/> is > 0x10000</exception>
        public void WriteAddress(IntelHexAddressType addressType, int address)
        {
            if (!Enum.IsDefined(typeof(IntelHexAddressType), addressType))
                throw new ArgumentOutOfRangeException(nameof(addressType),
                    $"Value [{addressType}] in not a value of [{nameof(IntelHexAddressType)}]");

            if ((addressType == IntelHexAddressType.ExtendedSegmentAddress) && (address > maxSegAddr))
                throw new ArgumentOutOfRangeException(nameof(address), $"Value must be less than 0x{maxSegAddr:X}");

            var addressData = FormatAddress(addressType, address);
            WriteHexRecord((IntelHexRecordType)addressType, 0, addressData);
        }

        /// <summary>
        /// Write a block of <paramref name="data"/> at <paramref name="address"/>
        /// </summary>
        /// <param name="address">The 16-bit address to write the data.</param>
        /// <param name="data">The block data to write.</param>
        /// <remarks>
        /// <paramref name="data"/> can be, at most, 255 bytes (even if checksum used is not strong enough).
        /// </remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="data"/> size is greater than 255..</exception>
        public void WriteData(ushort address, IList<byte> data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (data.Count > maxRecordDataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Must be less than {maxRecordDataSize} bytes long.");

            WriteHexRecord(IntelHexRecordType.Data, address, data);
        }

        /// <summary>
        /// Close the file flushing pending changes to the underlying stream and writing EOF record
        /// </summary>
        public void Close()
        {
            if (!disposedValue)
            {
                streamWriter?.WriteLine(":00000001FF");
                streamWriter?.Flush();
            }
        }

        private static List<byte> FormatAddress(IntelHexAddressType addressType, int address)
        {
            var result = new List<byte>();
            var shift = (byte)(addressType == IntelHexAddressType.ExtendedSegmentAddress ? 4 : 0);
            shift = (byte)(addressType == IntelHexAddressType.ExtendedLinearAddress ? 16 : shift);

            var addressBytes = BitConverter.GetBytes(address >> shift);

            if (addressType == IntelHexAddressType.StartLinearAddress)
            {
                result.Add(addressBytes[3]);
                result.Add(addressBytes[2]);
            }

            result.Add(addressBytes[1]);
            result.Add(addressBytes[0]);

            return result;
        }

        private static byte CalculateChecksum(IList<byte> checkSumData)
        {
            var maskedSumBytes = checkSumData.Sum(x => x) & 0xff;
            return (byte)(256 - maskedSumBytes);
        }

        private static string ToHexString(IList<byte> data)
        {
            var result = new char[data.Count * 2];
            for (var i = 0; i < data.Count; i++)
            {
                byte b = data[i];
                result[2 * i] = HexDigits[b >> 4];
                result[2 * i + 1] = HexDigits[b & 0xF];
            }

            return new string(result);
        }

        private void WriteHexRecord(IntelHexRecordType recordType, ushort address, IList<byte> data)
        {
            var addresBytes = BitConverter.GetBytes(address);
            var hexRecordData = new List<byte>
            {
                (byte)data.Count,
                addresBytes[1],
                addresBytes[0],
                (byte)recordType
            };
            hexRecordData.AddRange(data);
            var checksum = CalculateChecksum(hexRecordData);
            hexRecordData.Add(checksum);

            var hexRecord = $":{ToHexString(hexRecordData)}";

            streamWriter.WriteLine(hexRecord);
        }

        #endregion

        #region IDisposable implementation

        private bool disposedValue; // To detect redundant calls

        /// <summary>
        /// Dispose the <see cref="IntelHEXWriter"/>
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                    streamWriter?.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the <see cref="IntelHEXWriter"/>
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion

    }

}

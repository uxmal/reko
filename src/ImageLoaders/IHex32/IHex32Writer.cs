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
using System.IO;
using System.Linq;

namespace Reko.ImageLoaders.IHex32
{

    /// <summary>
    /// A writer capable of writing a Intel HEX32 stream
    /// </summary>
    public class IHEX32Writer : IDisposable
    {
        #region Locals

        private readonly StreamWriter _streamWriter;
        const string HexDigits = "0123456789ABCDEF";

        #endregion

        #region Constructors

        /// <summary>
        ///     Construct instance of an <see cref="IHEX32Writer" />.
        /// </summary>
        /// <param name="str">The target stream of the hex file.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="str" /> is null.</exception>
        public IHEX32Writer(Stream str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            _streamWriter = new StreamWriter(str);
        }

        #endregion

        #region API Methods

        /// <summary>
        /// Write an address record (type 02, 04 or 05) to the underlying stream
        /// </summary>
        /// <param name="addressType">The <see cref="IHEX32AddressType" /> address record type to write to the stream</param>
        /// <param name="address">The address value to write to the stream. This is either the segment address (type 02) or the upper word of a 32 bit address (type 04 or 05).</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="addressType"/> is not a member of <see cref="IHEX32AddressType"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="addressType"/> is an <see cref="IHEX32AddressType.ExtendedSegmentAddress"/> and <paramref name="address"/> is > 0x10000</exception>
        public void WriteAddress(IHEX32AddressType addressType, int address)
        {
            if (!Enum.IsDefined(typeof(IHEX32AddressType), addressType))
                throw new ArgumentOutOfRangeException(nameof(addressType),
                    $"Value [{addressType}] in not a value of [{nameof(IHEX32AddressType)}]");

            if ((addressType == IHEX32AddressType.ExtendedSegmentAddress) && (address > 0x10000))
                throw new ArgumentOutOfRangeException(nameof(address), "Value must be less than 0x10000");

            var addressData = _formatAddress(addressType, address);

            _writeHexRecord((IHEX32RecordType)addressType, 0, addressData);
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
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Count > 0xFF)
                throw new ArgumentOutOfRangeException(nameof(data), "Must be less than 255");

            _writeHexRecord(IHEX32RecordType.Data, address, data);
        }

        /// <summary>
        /// Close the file flushing pending changes to the underlying stream and writing EOF record
        /// </summary>
        public void Close()
        {
            if (!_disposedValue)
            {
                _streamWriter?.WriteLine(":00000001FF");
                _streamWriter?.Flush();
            }
        }

        #endregion

        #region Helpers

        private static List<byte> _formatAddress(IHEX32AddressType addressType, int address)
        {
            var result = new List<byte>();
            var shift = (byte)(addressType == IHEX32AddressType.ExtendedSegmentAddress ? 4 : 0);
            shift = (byte)(addressType == IHEX32AddressType.ExtendedLinearAddress ? 16 : shift);

            var addressBytes = BitConverter.GetBytes(address >> shift);

            if (addressType == IHEX32AddressType.StartLinearAddress)
            {
                result.Add(addressBytes[3]);
                result.Add(addressBytes[2]);
            }

            result.Add(addressBytes[1]);
            result.Add(addressBytes[0]);

            return result;
        }

        private static byte _calculateCrc(IList<byte> checkSumData)
        {
            var maskedSumBytes = checkSumData.Sum(x => x) & 0xff;
            var calculatedChecksum = (byte)(256 - maskedSumBytes);

            return calculatedChecksum;
        }

        private static string _toHexString(IList<byte> data)
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
        private void _writeHexRecord(IHEX32RecordType recordType, ushort address, IList<byte> data)
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
            var checksum = _calculateCrc(hexRecordData);
            hexRecordData.Add(checksum);

            var hexRecord = $":{_toHexString(hexRecordData)}";

            _streamWriter.WriteLine(hexRecord);
        }

        #endregion

        #region IDisposable implementation

        private bool _disposedValue; // To detect redundant calls

        /// <summary>
        /// Dispose the <see cref="IntelHEXWriter"/>
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Close();
                    _streamWriter?.Dispose();
                }

                _disposedValue = true;
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

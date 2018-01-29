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
    /// A reader capable of reading a Intel HEX32 stream
    /// </summary>
    public class IHex32Reader : IDisposable
    {

        #region Locals

        private readonly StreamReader _streamReader;
        private uint _addressBase = 0;
        private int _linenum;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs instance of an <see cref="IHex32Reader" />.
        /// </summary>
        /// <param name="str">The source stream of the hex file.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="str" /> is null.</exception>
        public IHex32Reader(Stream str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            _streamReader = new StreamReader(str);
        }

        #endregion

        #region IDisposable implementation

        private bool _disposedValue; // To detect redundant calls

        /// <summary>
        /// Dispose the <see cref="IHex32Reader"/>
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _streamReader?.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the <see cref="IHex32Reader"/>
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(disposing) above.
            Dispose(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read data with address information from the stream.
        /// </summary>
        /// <param name="address">[out] The start address for the block of data.</param>
        /// <param name="data">[out] The data byte block.</param>
        /// <returns>
        /// true if there are more records available or false if end of file.
        /// </returns>
        /// <remarks>
        /// An address value may be read without corresponding data bytes. This occurs for record
        /// types 02, 04, 05.
        /// </remarks>
        public bool Read(out uint address, out byte[] data)
        {
            var result = false;
            data = null;
            address = 0;
            _linenum++;

            var hexLine = _streamReader.ReadLine();

            if (!string.IsNullOrWhiteSpace(hexLine))
            {
                var hexRecord = _parseHexRecord(hexLine);

                if (hexRecord.RecordType != IHex32RecordType.EndOfFile)
                {
                    address = _handleAddress(hexRecord);

                    if (hexRecord.RecordType == IHex32RecordType.Data)
                        data = hexRecord.Data.ToArray();

                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Helpers

        private bool _verifyChecksum(IList<byte> checkSumData, int checkSum)
        {
            var maskedSumBytes = checkSumData.Sum(x => x) & 0xff;
            var calculatedChecksum = (byte)(256 - maskedSumBytes);

            return calculatedChecksum == checkSum;
        }

        private List<byte> _tryParseData(string hexData)
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
                throw new IHex32Exception($"Unable to extract bytes for [{hexData}]", ex, _linenum);
            }
        }

        private IHex32Record _parseHexRecord(string hexRecord)
        {
            if (hexRecord == null)
                throw new IHex32Exception("Hex record line can not be null", _linenum);
            if (hexRecord.Length < 11)
                throw new IHex32Exception($"Hex record line length [{hexRecord}] is less than 11", _linenum);
            if (hexRecord.Length % 2 == 0)
                throw new IHex32Exception($"Hex record has an even number of characters [{hexRecord}]", _linenum);
            if (!hexRecord.StartsWith(":"))
                throw new IHex32Exception($"Illegal line start character [{hexRecord}]", _linenum);
            var hexData = _tryParseData(hexRecord.Substring(1));

            if (hexData.Count != hexData[0] + 5)
                throw new IHex32Exception($"Line [{hexRecord}] does not have required record length of [{hexData[0] + 5}]", _linenum);

            if (!Enum.IsDefined(typeof(IHex32RecordType), (int)hexData[3]))
                throw new IHex32Exception($"Invalid record type value: [{hexData[3]}]", _linenum);

            var checkSum = hexData.Last();
            hexData.RemoveAt(hexData[0] + 4);

            if (!_verifyChecksum(hexData, checkSum))
                throw new IHex32Exception($"Checksum for line [{hexRecord}] is incorrect", _linenum);

            var dataSize = hexData[0];

            var newRecord = new IHex32Record
            {
                ByteCount = dataSize,
                Address = ((uint)(hexData[1] << 8) | hexData[2]),
                RecordType = (IHex32RecordType)hexData[3],
                Data = hexData,
                CheckSum = checkSum
            };

            hexData.RemoveRange(0, 4);

            return newRecord;
        }

        private uint _handleAddress(IHex32Record hexRecord)
        {
            uint result = 0;
            switch (hexRecord.RecordType)
            {
                case IHex32RecordType.Data:
                    result = _addressBase + hexRecord.Address;
                    break;
                case IHex32RecordType.ExtendedSegmentAddress:
                    _addressBase = (((uint)hexRecord.Data[0] << 8) | hexRecord.Data[1]) << 4;
                    result = _addressBase;
                    break;
                case IHex32RecordType.StartSegmentAddress:
                    result = _addressBase;
                    break;
                case IHex32RecordType.ExtendedLinearAddress:
                    _addressBase = (((uint)hexRecord.Data[0] << 8) | hexRecord.Data[1]) << 16;
                    result = _addressBase;
                    break;
                case IHex32RecordType.StartLinearAddress:
                    _addressBase = (((uint)hexRecord.Data[0] << 24) | ((uint)hexRecord.Data[1] << 16) |
                                           ((uint)hexRecord.Data[2] << 8) | hexRecord.Data[3]);
                    result = _addressBase;
                    break;
                default:
                    throw new IHex32Exception($"Unknown value read for [{nameof(hexRecord.RecordType)}]", _linenum);
            }

            return result;
        }

        #endregion

    }

}

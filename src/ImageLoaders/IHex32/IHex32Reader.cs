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
using System.IO;

namespace Reko.ImageLoaders.IHex32
{

    /// <summary>
    /// A reader capable of reading a Intel HEX32 stream
    /// </summary>
    public class IHEX32Reader : IDisposable
    {

        #region Locals

        private readonly StreamReader _streamReader;
        private int _linenum;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs instance of an <see cref="IHEX32Reader" />.
        /// </summary>
        /// <param name="str">The source stream of the hex file.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="str" /> is null.</exception>
        public IHEX32Reader(Stream str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            _streamReader = new StreamReader(str);
            _linenum = 0;
        }

        #endregion

        #region IDisposable implementation

        private bool _disposedValue; // To detect redundant calls

        /// <summary>
        /// Dispose the <see cref="IHEX32Reader"/>
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
        /// Dispose the <see cref="IHEX32Reader"/>
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(disposing) above.
            Dispose(true);
        }

        #endregion

        #region Public Methods/properties

        /// <summary>
        /// Gets or sets the base address for loading the 16-bit IHex32 records.
        /// </summary>
        public uint AddressBase { get; set; } = 0;

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
            data = null;
            address = 0;

            string hexLine = null;
            while (!_streamReader.EndOfStream && string.IsNullOrWhiteSpace(hexLine))
            {
                _linenum++;
                hexLine = _streamReader.ReadLine();
            }
            if (_streamReader.EndOfStream) return false;

            var hexRecord = IHEX32Parser.ParseRecord(hexLine, _linenum);

            if (hexRecord.RecordType != IHEX32RecordType.EndOfFile)
            {
                address = _handleAddress(hexRecord);

                if (hexRecord.RecordType == IHEX32RecordType.Data)
                    data = hexRecord.Data.ToArray();

                return true;
            }

            return false;
        }

        #endregion

        #region Helpers

        private uint _handleAddress(IHex32Record hexRecord)
        {
            switch (hexRecord.RecordType)
            {
                case IHEX32RecordType.Data:
                    return AddressBase + hexRecord.Address;

                case IHEX32RecordType.ExtendedSegmentAddress:
                    AddressBase = (((uint)hexRecord.Data[0] << 8) | hexRecord.Data[1]) << 4;
                    return AddressBase;

                case IHEX32RecordType.StartSegmentAddress:
                    return AddressBase;

                case IHEX32RecordType.ExtendedLinearAddress:
                    AddressBase = (((uint)hexRecord.Data[0] << 8) | hexRecord.Data[1]) << 16;
                    return AddressBase;

                case IHEX32RecordType.StartLinearAddress:
                    AddressBase = (((uint)hexRecord.Data[0] << 24) | ((uint)hexRecord.Data[1] << 16) |
                                           ((uint)hexRecord.Data[2] << 8) | hexRecord.Data[3]);
                    return AddressBase;

                default:
                    throw new IHEX32Exception($"Unknown value read for [{nameof(hexRecord.RecordType)}]", _linenum);
            }

        }

        #endregion

    }

}

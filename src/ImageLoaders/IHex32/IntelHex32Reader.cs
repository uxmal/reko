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

namespace Reko.ImageLoaders.IntelHex32
{

    /// <summary>
    /// A reader capable of reading a Intel Hexadecimal 32-bit format object (a.k.a. IHEX32) stream
    /// </summary>
    public class IntelHex32Reader : IDisposable
    {

        #region Locals

        private readonly StreamReader streamReader;
        private int lineNum;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs instance of an <see cref="IntelHex32Reader" />.
        /// </summary>
        /// <param name="str">The source stream of the hex file.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="str" /> is null.</exception>
        public IntelHex32Reader(Stream str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            streamReader = new StreamReader(str);
            lineNum = 0;
        }

        #endregion

        #region IDisposable implementation

        private bool disposedValue; // To detect redundant calls

        /// <summary>
        /// Dispose the <see cref="IntelHex32Reader"/>
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    streamReader?.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the <see cref="IntelHex32Reader"/>
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
            while (!streamReader.EndOfStream && string.IsNullOrWhiteSpace(hexLine))
            {
                lineNum++;
                hexLine = streamReader.ReadLine();
            }
            if (streamReader.EndOfStream)
                return false;

            var hexRecord = IntelHex32Parser.ParseRecord(hexLine, lineNum);

            if (hexRecord.RecordType != IntelHex32RecordType.EndOfFile)
            {
                address = HandleAddress(hexRecord);
                if (hexRecord.RecordType == IntelHex32RecordType.Data)
                    data = hexRecord.Data.ToArray();
                return true;
            }

            return false;
        }

        #endregion

        #region Helpers

        private uint HandleAddress(IntelHex32Record hexRecord)
        {
            switch (hexRecord.RecordType)
            {
                case IntelHex32RecordType.Data:
                    return AddressBase + hexRecord.Address;

                case IntelHex32RecordType.ExtendedSegmentAddress:
                    AddressBase = (((uint)hexRecord.Data[0] << 8) | hexRecord.Data[1]) << 4;
                    return AddressBase;

                case IntelHex32RecordType.StartSegmentAddress:
                    return AddressBase;

                case IntelHex32RecordType.ExtendedLinearAddress:
                    AddressBase = (((uint)hexRecord.Data[0] << 8) | hexRecord.Data[1]) << 16;
                    return AddressBase;

                case IntelHex32RecordType.StartLinearAddress:
                    AddressBase = (((uint)hexRecord.Data[0] << 24) | ((uint)hexRecord.Data[1] << 16) |
                                           ((uint)hexRecord.Data[2] << 8) | hexRecord.Data[3]);
                    return AddressBase;

                default:
                    throw new IntelHex32Exception($"Unknown value read for [{nameof(hexRecord.RecordType)}]", lineNum);
            }

        }

        #endregion

    }

}

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
using System.IO;
using Reko.Core;

namespace Reko.ImageLoaders.IntelHex
{

    /// <summary>
    /// A reader capable of reading a Intel Hexadecimal 32-bit format object (a.k.a. IHEX32) stream.
    /// </summary>
    public class IntelHexReader : IDisposable
    {

        #region Locals

        private readonly StreamReader streamReader;
        private int lineNum;
        private bool IsSegmented = false;
        private bool IsLinear = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs instance of an <see cref="IntelHexReader" />.
        /// </summary>
        /// <param name="str">The source stream of the hex file.</param>
        /// <param name="addressBase">The default base address to use.</param>
        /// <param name="bytesPerCodeUnit">Number of bytes consumed by each memory unit.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="str" /> is null.</exception>
        public IntelHexReader(Stream str, Address addressBase)
        {
            if (str is null)
                throw new ArgumentNullException(nameof(str));
            streamReader = new StreamReader(str);
            AddressBase = addressBase;
            lineNum = 0;
        }

        #endregion

        #region IDisposable implementation

        private bool disposedValue; // To detect redundant calls

        /// <summary>
        /// Dispose the <see cref="IntelHexReader"/>
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
        /// Dispose the <see cref="IntelHexReader"/>
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(disposing) above.
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Gets or sets the base address for loading/dumping the 16-bit IHex32 records.
        /// </summary>
        public Address AddressBase { get; set; }

        /// <summary>
        /// Gets the start address (program entry point).
        /// </summary>
        public Address? StartAddress { get; private set; } = null;

        #region Methods

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
        public bool TryReadRecord(out Address address, out byte[] data)
        {
            data = null!;
            address = default;
            string? hexLine = null;
            while (!streamReader.EndOfStream && string.IsNullOrWhiteSpace(hexLine))
            {
                lineNum++;
                hexLine = streamReader.ReadLine();
            }
            if (streamReader.EndOfStream)
                return false;

            var hexRecord = IntelHexParser.ParseRecord(hexLine!, lineNum);

            if (hexRecord.RecordType != IntelHexRecordType.EndOfFile)
            {
                address = HandleAddress(hexRecord);
                if (hexRecord.RecordType == IntelHexRecordType.Data)
                    data = hexRecord.Data!.ToArray();
                return true;
            }
            return false;
        }

        private Address HandleAddress(IntelHexRecord hexRecord)
        {
            switch (hexRecord.RecordType)
            {
            case IntelHexRecordType.Data:
                return AddressBase + hexRecord.Address;

            case IntelHexRecordType.ExtendedSegmentAddress:
                if (IsLinear)
                    throw new IntelHexException($"Mixed segmented/linear address.", lineNum);
                IsSegmented = true;
                var seg = ((uint) hexRecord.Data![0] << 8) | hexRecord.Data[1];
                AddressBase = Address.SegPtr((ushort) seg, 0);
                return AddressBase;

            case IntelHexRecordType.ExtendedLinearAddress:
                if (IsSegmented)
                    throw new IntelHexException($"Mixed segmented/linear address.", lineNum);
                IsLinear = true;
                AddressBase = Address.Ptr32((((uint) hexRecord.Data![0] << 8) | hexRecord.Data[1]) << 16);
                return AddressBase;

            case IntelHexRecordType.StartSegmentAddress:
                if (IsLinear)
                    throw new IntelHexException($"Mixed segmented/linear address.", lineNum);
                IsSegmented = true;
                StartAddress = Address.SegPtr(
                    (ushort) ((hexRecord.Data![0] << 8) | hexRecord.Data[1]),
                    (ushort) ((hexRecord.Data![2] << 8) | hexRecord.Data[3]));
                return AddressBase;

            case IntelHexRecordType.StartLinearAddress:
                if (IsSegmented)
                    throw new IntelHexException($"Mixed segmented/linear address.", lineNum);
                IsLinear = true;
                StartAddress = Address.Ptr32(
                    ((uint) hexRecord.Data![0] << 24) |
                    ((uint) hexRecord.Data[1] << 16) |
                    ((uint) hexRecord.Data[2] << 8) 
                    | hexRecord.Data[3]);
                return AddressBase;

            default:
                throw new IntelHexException($"Unknown value read for [{nameof(hexRecord.RecordType)}]", lineNum);
            }
        }

        #endregion

    }

}

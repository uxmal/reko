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

namespace Reko.ImageLoaders.IHex32
{
    /// <summary>
    /// Values that represent Intel Hexadecimal record types.
    /// </summary>
    internal enum IHex32RecordType
    {
        /// <summary>
        /// Indicates the record contains data and a 16-bit loading address for the data.
        /// </summary>
        Data,
        /// <summary>
        /// Indicates the record is the marker of End-of-File.
        /// </summary>
        EndOfFile,
        /// <summary>
        /// Indicates the record data field contains a 16-bit segment base address.
        /// </summary>
        ExtendedSegmentAddress,
        /// <summary>
        /// Indicates the record specifies the initial content of the CS:IP registers.
        /// </summary>
        StartSegmentAddress,
        /// <summary>
        /// Indicates the record contains the upper 16-bit address of a linear address.
        /// </summary>
        ExtendedLinearAddress,
        /// <summary>
        /// Indicates the record contains a 32 bit start address.
        /// </summary>
        StartLinearAddress
    }

}

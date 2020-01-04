#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
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

namespace Reko.ImageLoaders.IntelHex
{
    /// <summary>
    /// Values that represent Intel Hexadecimal 32-bit record types.
    /// </summary>
    public enum IntelHexRecordType : byte
    {
        /// <summary>
        /// Indicates the record contains data and a 16-bit loading address for the data.
        /// </summary>
        Data = 0,
        /// <summary>
        /// Indicates the record is the marker of End-of-File.
        /// </summary>
        EndOfFile = 1,
        /// <summary>
        /// Indicates the record data field contains a 16-bit segment base address.
        /// </summary>
        ExtendedSegmentAddress = 2,
        /// <summary>
        /// Indicates the record specifies the initial content of the CS:IP registers.
        /// </summary>
        StartSegmentAddress = 3,
        /// <summary>
        /// Indicates the record contains the upper 16-bit address of a linear address.
        /// </summary>
        ExtendedLinearAddress = 4,
        /// <summary>
        /// Indicates the record contains a 32-bit start linear address.
        /// </summary>
        StartLinearAddress = 5
    }

}

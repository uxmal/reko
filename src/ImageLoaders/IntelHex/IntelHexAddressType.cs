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
    /// The Address type for address values written to an Intel Hexadecimal 32-bit object format stream.
    /// </summary>
    public enum IntelHexAddressType : byte
    {
        /// <summary>
        /// Indicates the record data field contains a 16-bit segment:base address
        /// </summary>
        ExtendedSegmentAddress = 2,

        /// <summary>
        /// Indicates the record contains the upper 16-bit address
        /// </summary>
        ExtendedLinearAddress = 4,

        /// <summary>
        /// Indicates the record contains a 32-bit address
        /// </summary>
        StartLinearAddress = 5
    }

}

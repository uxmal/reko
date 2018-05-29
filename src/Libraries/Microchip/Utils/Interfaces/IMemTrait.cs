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

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// This interface provides information on a memory trait (characteristics).
    /// </summary>
    public interface IMemTrait
    {
        /// <summary>
        /// Gets the size of the memory word (in bytes).
        /// </summary>
        uint WordSize { get; }

        /// <summary>
        /// Gets the memory location access size (in bytes).
        /// </summary>
        uint LocSize { get; }

        /// <summary>
        /// Gets the memory word implementation (bit mask).
        /// </summary>
        uint WordImpl { get; }

        /// <summary>
        /// Gets the initial (erased) memory word value.
        /// </summary>
        uint WordInit { get; }

        /// <summary>
        /// Gets the memory word 'safe' value. (Probably unused)
        /// </summary>
        uint WordSafe { get; }

    }

}


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
    /// This interface provides information on PIC memory address range [begin,end( , domain, sub-domain.
    /// </summary>
    public interface IMemoryAddrRange
    {

        /// <summary>
        /// Gets the beginning address of the memory range.
        /// </summary>
        uint BeginAddr { get; }

        /// <summary>
        /// Gets the ending (+1) address of the memory range.
        /// </summary>
        uint EndAddr { get; }

        /// <summary>
        /// Gets the memory domain of this memory range.
        /// </summary>
        /// <value>
        /// A value from the <see cref="MemoryDomain"/> enumeration.
        /// </value>
        MemoryDomain MemoryDomain { get; }

        /// <summary>
        /// Gets the memory sub-domain of this memory range.
        /// </summary>
        /// <value>
        /// A value from the <see cref="MemorySubDomain"/> enumeration.
        /// </value>
        MemorySubDomain MemorySubDomain { get; }

    }

}

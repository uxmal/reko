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
    /// This interface provides traits (characteristics) for a given memory domain/sub-domain.
    /// </summary>
    public interface ITrait : IMemTrait
    {
        /// <summary>
        /// Gets the memory domain.
        /// </summary>
        /// <value>
        /// A value from the enumeration <seealso cref="PICMemoryDomain"/> enumeration.
        /// </value>
        PICMemoryDomain Domain { get; }

        /// <summary>
        /// Gets the memory sub-domain.
        /// </summary>
        /// <value>
        /// A value from the enumeration <seealso cref="PICMemorySubDomain"/> enumeration.
        /// </value>
        PICMemorySubDomain SubDomain { get; }

    }

}

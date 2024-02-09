#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

namespace Reko.Core.Memory
{
    /// <summary>
    /// Abstracts the notion of 'memory' that can be written to.
    /// Clients need not need to be concerned with low-level
    /// details like segments etc.
    /// </summary>
    public interface IWriteableMemory : IMemory
    {
        /// <summary>
        /// Create a big-endian <see cref="ImageWriter"/> starting at the
        /// memory address <paramref name="addr" />.
        /// </summary>
        /// <param name="addr">Address at which to start writing.</param>
        /// <returns>A big-endian <see cref="ImageWriter"/>.</returns>
        ImageWriter CreateBeWriter(Address addr);

        /// <summary>
        /// Create a little-endian <see cref="ImageWriter"/> starting at the
        /// memory address <paramref name="addr" />.
        /// </summary>
        /// <param name="addr">Address at which to start writing.</param>
        /// <returns>A big-endian <see cref="ImageWriter"/>.</returns>
        ImageWriter CreateLeWriter(Address addr);
    }

    /// <summary>
    /// Implements a common special case of the <see cref="IWriteableMemory"/>,
    /// where individual memory units are 8-bit octets.
    /// </summary>

    public interface IByteWriteableMemory : IWriteableMemory
    {
        /// <summary>
        /// Write an octet-sized value to memory.
        /// </summary>
        /// <param name="addr">Address to which to write the value.</param>
        /// <param name="b">Value to write.</param>
        void TryWriteInt8(Address addr, sbyte b);

        /// <summary>
        /// Write an octet-sized value to memory.
        /// </summary>
        /// <param name="addr">Address to which to write the value.</param>
        /// <param name="b">Value to write.</param>
        void TryWriteUInt8(Address addr, byte b);
    }
}

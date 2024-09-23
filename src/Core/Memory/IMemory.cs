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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Memory
{
    /// <summary>
    /// Abstracts the notion of 'memory'. Clients need not need to be 
    /// concerned with low-level details like segments etc.
    /// </summary>
    public interface IMemory
    {
        SegmentMap SegmentMap { get; }

        /// <summary>
        /// Tests if the memory address is neither writeable nor
        /// executable (and readable).
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>True if the memory address can be read but cannot be
        /// written to or executed.
        /// </returns>
        bool IsReadonly(Address addr);

        /// <summary>
        /// Returns whether the given address exists in the memory.
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>True if address is mapped, false if not.</returns>
        bool IsValidAddress(Address addr);

        /// <summary>
        /// Tests if the memory address is writeable.
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>True if the memory address can written to.
        /// </returns>
        bool IsWriteable(Address addr);

        /// <summary>
        /// Tests if the memory address is executable, i.e. the
        /// CPU is allowed to execute an instruction at that address.
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>True if the memory address is executable.
        /// </returns>
        bool IsExecutableAddress(Address addr);

        /// <summary>
        /// Create a big-endian <see cref="EndianImageReader"/> spanning the
        /// memory starting at address <paramref name="addr" /> and continuing
        /// until the end of memory.
        /// </summary>
        /// <param name="addr">Address at which to start reading.</param>
        /// <param name="rdr">A big-endian <see cref="EndianImageReader"/>.</returns>
        /// <returns>True if the provided address refers to valid memory,
        /// otherwise false.</returns>
        bool TryCreateBeReader(Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Create a little-endian <see cref="EndianImageReader"/> spanning the
        /// memory starting at address <paramref name="addr" /> and continuing
        /// until the end of memory.
        /// </summary>
        /// <param name="addr">Address at which to start reading.</param>
        /// <param name="rdr">A little-endian <see cref="EndianImageReader"/>.</returns>
        /// <returns>True if the provided address refers to valid memory,
        /// otherwise false.</returns>
        bool TryCreateLeReader(Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Create a big-endian <see cref="EndianImageReader"/> spanning the
        /// memory starting at address <paramref name="addr" /> and continuing
        /// for <paramref name="cUnits"/> memory units.
        /// </summary>
        /// <param name="addr">Address at which to start reading.</param>
        /// <param name="cUnits">Number of units to read, after which no more
        /// will be read.</param>
        /// <param name="rdr">A big-endian <see cref="EndianImageReader"/>.</returns>
        /// <returns>True if the provided address refers to valid memory,
        /// otherwise false.</returns>
        bool TryCreateBeReader(Address addr, long cUnits, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Create a big-endian <see cref="EndianImageReader"/> spanning the
        /// memory starting at address <paramref name="addr" /> and continuing
        /// for <paramref name="cUnits"/> memory units.
        /// </summary>
        /// <param name="addr">Address at which to start reading.</param>
        /// <param name="cUnits">Number of units to read, after which no more
        /// will be read.</param>
        /// <param name="rdr">A little-endian <see cref="EndianImageReader"/>.</returns>
        /// <returns>True if the provided address refers to valid memory,
        /// otherwise false.</returns>
        bool TryCreateLeReader(Address addr, long cUnits, [MaybeNullWhen(false)] out EndianImageReader rdr);

        /// <summary>
        /// Attempt to read a big-endian value of type <paramref name="dt"/> from the address 
        /// <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address at which the read is to be attempted.</param>
        /// <param name="dt">Data type of the value to be read.</param>
        /// <param name="c">The returned value.</param>
        /// <returns>True if the read succeeded, false if not.</returns>
        bool TryReadBe(Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant c);

        /// <summary>
        /// Attempt to read a little-endian value of type <paramref name="dt"/> from the address 
        /// <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address at which the read is to be attempted.</param>
        /// <param name="dt">Data type of the value to be read.</param>
        /// <param name="c">The returned value.</param>
        /// <returns>True if the read succeeded, false if not.</returns>
        bool TryReadLe(Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant c);
    }

    /// <summary>
    /// Implements a common special case of the <see cref="IMemory"/>,
    /// where individual memory units are 8-bit octets.
    /// </summary>
    public interface IByteAdressableMemory : IMemory
    {
        /// <summary>
        /// Read a signed octet from address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadInt8(Address addr, out byte b);

        /// <summary>
        /// Read an unsigned octet from address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadUInt8(Address addr, out sbyte b);

        /// <summary>
        /// Read a big endian signed 16-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadBeInt16(Address addr, out short s);

        /// <summary>
        /// Read a little endian signed 16-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadLeInt16(Address addr, out short s);

        /// <summary>
        /// Read a big endian unsigned 16-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadBeUInt16(Address addr, out ushort s);

        /// <summary>
        /// Read a little endian unsigned 16-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadLeUInt16(Address addr, out ushort s);

        /// <summary>
        /// Read a big endian signed 32-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadBeInt32(Address addr, out int i);

        /// <summary>
        /// Read a little endian signed 32-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadLeInt32(Address addr, out int i);

        /// <summary>
        /// Read a big endian unsigned 32-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadBeUInt32(Address addr, out uint u);

        /// <summary>
        /// Read a little endian unsigned 32-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadLeUInt32(Address addr, out uint u);

        /// <summary>
        /// Read a big endian signed 64-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadBeInt64(Address addr, out long l);

        /// <summary>
        /// Read a little endian signed 64-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadLeInt64(Address addr, out long l);

        /// <summary>
        /// Read a big endian unsigned 64-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadBeUInt64(Address addr, out ulong ul);

        /// <summary>
        /// Read a little endian unsigned 64-bit integer from 
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to read from.</param>
        /// <param name="b">Resulting value if successful.</param>
        /// <returns>True if address was valid, false if not.
        /// </returns>
        bool TryReadLeUInt64(Address addr, out ulong ul);
    }
}

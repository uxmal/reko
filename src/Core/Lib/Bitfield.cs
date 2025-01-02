#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Core.Lib
{
    /// <summary>
    /// Represents a bitfield inside a machine code instruction.
    /// </summary>
    public readonly struct Bitfield
    {
        /// <summary>
        /// The bit position within an instruction. Bit positions are numbered
        /// little-endian fashion: position 0 is the least significant bit.
        /// </summary>
        public readonly int Position;

        /// <summary>
        /// The length of the bit field.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The mask to use when extracting a value from the bit field.
        /// </summary>
        public readonly uint Mask;

        /// <summary>
        /// Creates a <see cref="Bitfield"/> starting at bit position 
        /// <paramref name="position"/> and consisting of <paramref name="length"/>
        /// bits.
        /// </summary>
        /// <param name="position">Little endian bit position where the mask begins.</param>
        /// <param name="length">Length of the bit field in bits.</param>
        public Bitfield(int position, int length)
        {
            this.Position = position;
            this.Length = length;
            this.Mask = (uint)((1UL << length) - 1U);
        }

        /// <summary>
        /// Creates a <see cref="Bitfield"/> starting at bit position 
        /// <paramref name="position"/> and consisting of <paramref name="length"/>
        /// bits, using the provided <paramref name="mask"/> to mask out read data.
        /// </summary>
        /// <param name="position">Little endian bit position where the mask begins.</param>
        /// <param name="length">Length of the bit field in bits.</param>
        /// <param name="mask">Mask to use when reading the bit field.</param>
        public Bitfield(int position, int length, uint mask)
        {
            this.Position = position;
            this.Length = length;
            this.Mask = mask;
        }

        /// <summary>
        /// Reads a unsigned integer bit field from the given unsigned integer value 
        /// <paramref name="u"/>.
        /// </summary>
        /// <param name="u">Value from which to extract a bit field.</param>
        /// <returns>A zero-extended unsigned bitfield value.</returns>
        public uint Read(uint u)
        {
            return (u >> Position) & Mask;
        }

        /// <summary>
        /// Reads an unsigned integer bit field from the given unsigned long integer value.
        /// <paramref name="u"/>.
        /// </summary>
        /// <param name="u">Value from which to extract a bit field.</param>
        /// <returns>A zero-extended unsigned bitfield value.</returns>
        public uint Read(ulong u)
        {
            return (uint)((u >> Position) & Mask);
        }

        /// <summary>
        /// Reads a signed integer bit field from the given unsigned integer value.
        /// <paramref name="u"/>.
        /// </summary>
        /// <param name="u">Value from which to extract a bit field.</param>
        /// <returns>A sign-extended bitfield value.</returns>
        public int ReadSigned(uint u)
        {
            var v = (u >> Position) & Mask;
            var m = 1u << (Length - 1);
            var s = (v ^ m) - m;
            return (int)s;
        }

        /// <summary>
        /// Reads a signed integer bit field from the given unsigned long integer value.
        /// <paramref name="u"/>.
        /// </summary>
        /// <param name="u">Value from which to extract a bit field.</param>
        /// <returns>A sign-extended bitfield value.</returns>
        public long ReadSigned(ulong u)
        {
            var v = (u >> Position) & Mask;
            var m = 1ul << (Length - 1);
            var s = (v ^ m) - m;
            return (long) s;
        }

        /// <summary>
        /// Reads a sequence of bit field values and returns the concatenation of
        /// those values.
        /// </summary>
        /// <param name="bitfields">Sequence of bit fields to read.</param>
        /// <param name="u">Unsigned integer to read the fields from.</param>
        /// <returns>An unsigned value consisting of the bitwise concatenation of the
        /// bitfield values.
        /// </returns>
        public static uint ReadFields(Bitfield[] bitfields, uint u)
        {
            uint n = 0;
            foreach (var bitfield in bitfields)
            {
                n = n << bitfield.Length | ((u >> bitfield.Position) & bitfield.Mask);
            }
            return n;
        }

        /// <summary>
        /// Reads a sequence of bit field values and returns the concatenation of
        /// those values.
        /// </summary>
        /// <param name="bitfields">Sequence of bit fields to read.</param>
        /// <param name="u">Unsigned long integer to read the fields from.</param>
        /// <returns>An unsigned long value consisting of the bitwise concatenation of the
        /// bitfield values.
        /// </returns>
        public static ulong ReadFields(Bitfield[] bitfields, ulong u)
        {
            ulong n = 0;
            foreach (var bitfield in bitfields)
            {
                n = n << bitfield.Length | ((u >> bitfield.Position) & bitfield.Mask);
            }
            return n;
        }

        public static int ReadSignedFields(Bitfield[] fields, uint u)
        {
            int n = 0;
            int bitsTotal = 0;
            foreach (var bitfield in fields)
            {
                n = n << bitfield.Length | (int)((u >> bitfield.Position) & bitfield.Mask);
                bitsTotal += bitfield.Length;
            }
            n <<= (32 - bitsTotal);
            n >>= (32 - bitsTotal);
            return n;
        }

        /// <summary>
        /// Reads a sequence of bit field values and returns the sign
        /// extension of the concatenation of those values.
        /// </summary>
        /// <param name="bitfields">Sequence of bit fields to read.</param>
        /// <param name="u">Unsigned long integer to read the fields from.</param>
        /// <returns>An signed long value consisting of the sign extension of
        /// the bitwise concatenation of the bitfield values.
        /// </returns>
        public static long ReadSignedFields(Bitfield[] fields, ulong ul)
        {
            long n = 0;
            int bitsTotal = 0;
            foreach (var bitfield in fields)
            {
                n = n << bitfield.Length | (long) ((ul >> bitfield.Position) & bitfield.Mask);
                bitsTotal += bitfield.Length;
            }
            n <<= (64 - bitsTotal);
            n >>= (64 - bitsTotal);
            return n;
        }

        public override string ToString()
        {
            return $"[{Position}..{Position + Length})";
        }
    }
}

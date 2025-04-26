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

using System.Numerics;

namespace Reko.Core.Lib
{
    /// <summary>
    /// Bit manipulation methods.
    /// </summary>
    public static class Bits
    {
        /// <summary>
        /// Returns true if the bit at position <paramref name="pos"/> is set.
        /// </summary>
        public static bool IsBitSet(uint u, int pos)
        {
            return ((u >> pos) & 1) != 0;
        }

        /// <summary>
        /// Tests whether the bit at position <paramref name="pos"/>.
        /// </summary>
        /// <param name="u">Bit vector.</param>
        /// <param name="pos">Position to test.</param>
        /// <returns>True if <paramref name="u"/> has a bit set at 
        /// position <paramref name="pos"/>; otherwise false.
        /// </returns>
        public static bool IsBitSet(ulong u, int pos)
        {
            return ((u >> pos) & 1) != 0;
        }

        /// <summary>
        /// Returns true if exactly one bit of the word is set.
        /// </summary>
        public static bool IsSingleBitSet(uint w)
        {
            return w != 0 && (w & (w - 1)) == 0;
        }

        /// <summary>
        /// Returns true if <paramref name="n"/> is an even power of two.
        /// </summary>
        /// <param name="n">Number to test.</param>
        public static bool IsEvenPowerOfTwo(long n)
        {
            return n != 0 && (n & (n - 1)) == 0;
        }

        /// <summary>
        /// Sign-extend the <paramref name="b"/>-bit unsigned long number. 
        /// <paramref name="w"/>.
        /// </summary>
        /// <param name="w">Value containing the number to sign extend.</param>
        /// <param name="b">Number of lsb bits to extend.</param>
        /// <returns>The sign-extended number.</returns>
        public static ulong SignExtend(ulong w, int b)
        {
            // This test unfortunately introduces a jump,
            // but if we don't add it, we get incorrect
            // results.
            if (b >= 0x40)  // The size of a C# ulong
                return w;

            ulong r;      // resulting sign-extended number
            ulong m = 1LU << (b - 1); // mask can be pre-computed if b is fixed
            w &= ((1LU << b) - 1);  // (Skip this if bits in x above position b are already zero.)
            r = (w ^ m) - m;
            return r;
        }

        /// <summary>
        /// Sign-extend the <paramref name="b"/>-bit unsigned long number. 
        /// <paramref name="w"/>.
        /// </summary>
        /// <param name="w">Value containing the number to sign extend.</param>
        /// <param name="b">Number of lsb bits to extend.</param>
        /// <returns>The sign-extended number.</returns>
        public static uint SignExtend(uint w, int b)
        {
            // This test unfortunately introduces a jump,
            // but if we don't add it, we get incorrect
            // results.
            if (b >= 0x20)  // The size of a C# uint
                return w;
            uint r;      // resulting sign-extended number
            uint m = 1u << (b - 1); // mask can be pre-computed if b is fixed
            w &= ((1u << b) - 1);  // (Skip this if bits in x above position b are already zero.)
            r = (w ^ m) - m;
            return r;
        }

        /// <summary>
        /// Zero-extend the <paramref name="b"/>-bit unsigned long number. 
        /// <paramref name="w"/>.
        /// </summary>
        /// <param name="w">Value containing the number to zero extend.</param>
        /// <param name="b">Number of lsb bits to extend.</param>
        /// <returns>The zero-extended number.</returns>
        public static ulong ZeroExtend(ulong w, int b)
        {
            // This test unfortunately introduces a jump,
            // but if we don't add it, we get incorrect
            // results.
            if (b >= 0x40)  // The size of a C# ulong
                return w;
            ulong m = (1Lu << b) - 1;
            return w & m;
        }

        /// <summary>
        /// Generates a bit mask, starting at bit position 
        /// <paramref name="lsb"/> and having the length 
        /// <paramref name="bitsize"/>.
        /// </summary>
        /// <param name="lsb">Least bit position.</param>
        /// <param name="bitsize">Width of the bit mask.</param>
        /// <returns>A bit mask.</returns>
        public static ulong Mask(int lsb, int bitsize)
        {
            ulong mask = (bitsize == 0x40)
                ? ~0ul
                : ((1ul << bitsize) - 1);
            return mask << lsb;
        }

        /// <summary>
        /// Create a <see cref="BigInteger"/> mask of width <paramref name="maskWidth"/>.
        /// </summary>
        /// <param name="maskWidth">Width of the mask.</param>
        /// <returns>The mask as a <see cref="BigInteger"/>.</returns>
        public static BigInteger Mask(int maskWidth)
        {
            return (BigInteger.One << maskWidth) - BigInteger.One;
        }

        // http://stackoverflow.com/questions/11376288/fast-computing-of-log2-for-64-bit-integers
        private static readonly int[] log2_tab = {
             0,  9,  1, 10, 13, 21,  2, 29,
            11, 14, 16, 18, 22, 25,  3, 30,
             8, 12, 20, 28, 15, 17, 24,  7,
            19, 27, 23,  6, 26,  5,  4, 31
        };

        /// <summary>
        /// Computes the base-2 logarithm of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value to compute the base-2 logarithm of.</param>
        /// <returns>Base-2 logarithm.</returns>
        // http://stackoverflow.com/questions/11376288/fast-computing-of-log2-for-64-bit-integers
        public static int Log2(uint value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return log2_tab[(uint)(value * 0x07C4ACDD) >> 27];
        }

        /// <summary>
        /// Rotate the 32-bit number <paramref name="u"/> right 
        /// <paramref name="s"/> bits.
        /// </summary>
        /// <param name="u">Value to rotate.</param>
        /// <param name="s">Rotation amount.</param>
        /// <returns>Rotated value.</returns>
        public static uint RotateR32(uint u, int s)
        {
            return (u << (32 - s)) | (u >> s);
        }

        /// <summary>
        /// Rotate the 64bit number <paramref name="u"/> left 
        /// <paramref name="s"/> bits.
        /// </summary>
        /// <param name="u">Value to rotate.</param>
        /// <param name="s">Rotation amount.</param>
        /// <returns>Rotated value.</returns>
        public static ulong RotateL64(ulong u, int s)
        {
            return (u << s) | (u >> (64 - s));
        }

        /// <summary>
        /// Rotate the <paramref name="wordSize"/> least significant bits of 
        /// the unsigned value <paramref name="u"/> to the right by <paramref name="s"/>
        /// bits.
        /// </summary>
        public static ulong RotateR(int wordSize, ulong u, int s)
        {
            var mask = Bits.Mask(0, wordSize);
            u &= mask;
            ulong uNew = (u << (wordSize - s)) | (u >> s);
            return uNew & mask;
        }

        /// <summary>
        /// Counts the number of leading zero bits.
        /// </summary>
        /// <param name="wordSize">Bit size of value being examined.</param>
        /// <param name="value">Value being examined.</param>
        /// <returns>Number of leading zero bits of <paramref name="value"/>.</returns>
        public static int CountLeadingZeros(int wordSize, ulong value)
        {
            int n = wordSize;
            ulong y;
            y = value >> 32; if (y != 0) { n -= 32; value = y; }
            y = value >> 16; if (y != 0) { n -= 16; value = y; }
            y = value >> 8; if (y != 0) { n -= 8; value = y; }
            y = value >> 4; if (y != 0) { n -= 4; value = y; }
            y = value >> 2; if (y != 0) { n -= 2; value = y; }
            y = value >> 1; if (y != 0) return n - 2;
            var leading = n - (int) value;
            return leading;
        }

        /// <summary>
        /// Change the endianness of <paramref name="u"/>.
        /// </summary>
        public static uint Reverse(uint u)
        {
            var uNew =
                (u >> 24) |
                (((u >> 16) & 0xFF) << 8) |
                (((u >> 8) & 0xFF) << 16) |
                ((u & 0xFF) << 24);
            return uNew;
        }

        /// <summary>
        /// Replicates the bits in the least significant <paramref name="length"/> bits
        /// of <paramref name="pattern"/> <paramref name="times"/>.
        /// </summary>
        /// <param name="pattern">Bit pattern to replicate</param>
        /// <param name="length">Length of the bit pattern</param>
        /// <param name="times">Number of times to replicate</param>
        /// <returns>Replicated pattern.</returns>
        public static ulong Replicate64(ulong pattern, int length, int times)
        {
            ulong maskedPattern = pattern & Mask(0, length);
            ulong value = 0;
            for (int i = 0; i < times;++i)
            {
                value = (value << length) | maskedPattern;
            }
            return value;
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

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

        public static bool IsEvenPowerOfTwo(long n)
        {
            return n != 0 && (n & (n - 1)) == 0;
        }

        /// <summary>
        /// Sign-extend the <paramref name="b"/>-bit number 
        /// <paramref name="w"/>.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ulong SignExtend(ulong w, int b)
        {
            ulong r;      // resulting sign-extended number
            ulong m = 1LU << (b - 1); // mask can be pre-computed if b is fixed
            w &= ((1LU << b) - 1);  // (Skip this if bits in x above position b are already zero.)
            r = (w ^ m) - m;
            return r;
        }

        public static uint SignExtend(uint w, int b)
        {
            uint r;      // resulting sign-extended number
            uint m = 1u << (b - 1); // mask can be pre-computed if b is fixed
            w &= ((1u << b) - 1);  // (Skip this if bits in x above position b are already zero.)
            r = (w ^ m) - m;
            return r;
        }

        public static ulong ZeroExtend(ulong w, int b)
        {
            ulong m = (1Lu << b) - 1;
            return w & m;
        }

        public static ulong Mask(int lsb, int bitsize)
        {
            ulong mask = (bitsize == 0x40)
                ? ~0ul
                : ((1ul << bitsize) - 1);
            return mask << lsb;
        }

        public static BigInteger Mask(int maskWidth)
        {
            return (BigInteger.One << maskWidth) - BigInteger.One;
        }

        public static int BitCount(ulong u)
        {
            return BitOperations.PopCount(u);
        }

        // http://stackoverflow.com/questions/11376288/fast-computing-of-log2-for-64-bit-integers
        private static readonly int[] log2_tab = {
             0,  9,  1, 10, 13, 21,  2, 29,
            11, 14, 16, 18, 22, 25,  3, 30,
             8, 12, 20, 28, 15, 17, 24,  7,
            19, 27, 23,  6, 26,  5,  4, 31
        };

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

        public static uint RotateR32(uint u, int s)
        {
            return (u << (32 - s)) | (u >> s);
        }

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

        public static int CountLeadingZeros(int wordSize, ulong x)
        {
            int n = wordSize;
            ulong y;

            y = x >> 32; if (y != 0) { n -= 32; x = y; }
            y = x >> 16; if (y != 0) { n -= 16; x = y; }
            y = x >> 8; if (y != 0) { n -= 8; x = y; }
            y = x >> 4; if (y != 0) { n -= 4; x = y; }
            y = x >> 2; if (y != 0) { n -= 2; x = y; }
            y = x >> 1; if (y != 0) return n - 2;
            var leading = n - (int) x;
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

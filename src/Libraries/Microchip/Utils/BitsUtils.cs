#region License
/* 
 * Copyright (c) 2017-2025 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.gnu.org/licenses/gpl-2.0.html.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * If applicable, add the following below the header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */

#endregion

namespace Reko.Libraries.Microchip
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// This class provides extension methods to manipulate bit fields.
    /// Some of the algorithms implemented here are inspired by the book "Hacker's Delight" by Henry S. Warren Jr.
    /// </summary>
    public static partial class BitsUtils
    {
        #region Sign Extension

        /// <summary>
        /// Sign-extends the specified lowest bits field of an unsigned byte integer into a full-sized signed byte integer.
        /// </summary>
        /// <param name="val">The specified byte bit field</param>
        /// <param name="size">Number of less significant bits to take for sign extension.</param>
        /// <returns>An signed byte integer containing the signed-extended bit field.</returns>
        /// 
        public static sbyte SignExtend(this byte val, int size)
        {
            unchecked
            {
                var m = 1U << (size - 1);
                return (sbyte) ((val ^ m) - m);
            }
        }

        /// <summary>
        /// Sign-extends the specified lowest bits field of an unsigned short integer into a full-sized signed short integer.
        /// </summary>
        /// <param name="val">The specified unsigned short bit field</param>
        /// <param name="size">Number of less significant bits to take for sign extension.</param>
        /// <returns>An signed short integer containing the signed-extended bit field.</returns>
        /// 
        public static short SignExtend(this ushort val, int size)
        {
            unchecked
            {
                var m = 1U << (size - 1);
                return (short) ((val ^ m) - m);
            }
        }

        /// <summary>
        /// Sign-extends the specified lowest bits field of an unsigned integer into a full-sized signed integer.
        /// </summary>
        /// <param name="val">The specified bit field</param>
        /// <param name="size">Number of less significant bits to take for sign extension.</param>
        /// <returns>An signed integer containing the signed-extended bit field.</returns>
        /// 
        public static int SignExtend(this uint val, int size)
        {
            unchecked
            {
                var m = 1U << (size - 1);
                return (int) ((val ^ m) - m);
            }
        }

        /// <summary>
        /// Sign-extends the specified lowest bits field of a signed long integer into a full-sized signed long integer.
        /// </summary>
        /// <param name="val">The specified long bit field</param>
        /// <param name="size">Number of less significant bits to take for sign extension.</param>
        /// <returns>An signed long integer containing the signed-extended bit field.</returns>
        /// 
        public static long SignExtend(this ulong val, int size)
        {
            unchecked
            {
                var m = 1UL << (size - 1);
                return (long) ((val ^ m) - m);
            }
        }

        #endregion

        #region Bit field Extraction

        /// <summary>
        /// Extracts the bits field from the specified signed byte
        /// </summary>
        /// <param name="val">The specified signed byte.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>Right-adjusted bits field.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static sbyte Extract(this sbyte val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (sbyte) (val & ((1 << bitcount) - 1));
                return val;
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified signed short integer
        /// </summary>
        /// <param name="val">The specified signed short integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>Right-adjusted bits field.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static short Extract(this short val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (short) (val & ((1 << bitcount) - 1));
                return val;
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified signed integer
        /// </summary>
        /// <param name="val">The specified signed integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>Right-adjusted bits field.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static int Extract(this int val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (int) (val & ((1 << bitcount) - 1));
                return val;
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified signed long integer
        /// </summary>
        /// <param name="val">The specified signed long integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>Right-adjusted bits field.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static long Extract(this long val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (long) (val & ((1L << bitcount) - 1));
                return val;
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified (unsigned) byte
        /// </summary>
        /// <param name="val">The specified (unsigned) byte.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>Right-adjusted bits field.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static byte Extract(this byte val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (byte) (val & ((1 << bitcount) - 1));
            }
            return val;
        }

        /// <summary>
        /// Extracts the bits field from the specified (unsigned) byte and sign-extend
        /// </summary>
        /// <param name="val">The specified (unsigned) byte.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>A signed byte.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static sbyte ExtractSignExtend(this byte val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (byte) (val & ((1U << bitcount) - 1));
                var m = 1U << (bitcount - 1);
                return (sbyte) ((val ^ m) - m);
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified unsigned short integer
        /// </summary>
        /// <param name="val">The specified unsigned short integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>Right-adjusted bits field.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static ushort Extract(this ushort val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (ushort) (val & ((1U << bitcount) - 1));
                return val;
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified unsigned short integer and sign-extend
        /// </summary>
        /// <param name="val">The specified 16-bit unsigned integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>16-bit signed integer.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static short ExtractSignExtend(this ushort val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (ushort) (val & ((1U << bitcount) - 1));
                var m = 1U << (bitcount - 1);
                return (short) ((val ^ m) - m);
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified unsigned integer
        /// </summary>
        /// <param name="val">The specified unsigned integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>Right-adjusted bits field.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static uint Extract(this uint val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (val & ((1U << bitcount) - 1));
                return val;
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified unsigned integer and sign-extend
        /// </summary>
        /// <param name="val">The specified unsigned integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>The 32-bit signed integer.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static int ExtractSignExtend(this uint val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (val & ((1U << bitcount) - 1));
                var m = 1U << (bitcount - 1);
                return (int) ((val ^ m) - m);
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified unsigned long integer
        /// </summary>
        /// <param name="val">The specified unsigned long integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>Right-adjusted bits field.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static ulong Extract(this ulong val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (val & ((1UL << bitcount) - 1));
                return val;
            }
        }

        /// <summary>
        /// Extracts the bits field from the specified unsigned long integer and sign-extend
        /// </summary>
        /// <param name="val">The specified unsigned long integer.</param>
        /// <param name="startbit">Lowest bit to extract.</param>
        /// <param name="bitcount">Number of bits to extract.</param>
        /// <returns>The 64-bit signed integer.</returns>
        /// <remarks>No check is done on the validity of the bit start and bit count values.</remarks>
        /// 
        public static long ExtractSignExtend(this ulong val, int startbit, int bitcount)
        {
            unchecked
            {
                val >>= startbit;
                val = (val & ((1UL << bitcount) - 1));
                var m = 1UL << (bitcount - 1);
                return (long) ((val ^ m) - m);
            }
        }

        /// <summary>   Extracts the bits field from the specified byte array. </summary>
        ///
        /// <param name="val">      The specified byte array of 1, 2 or 4 bytes. </param>
        /// <param name="startbit"> Lowest bit to extract. </param>
        /// <param name="bitcount"> Number of bits to extract. </param>
        ///
        /// <returns>   Right-adjusted bits field as an unsigned 32-bit integer. </returns>
        ///
        /// <remarks>   No check is done on the validity of the bit start and bit count values. </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">If the array is not 1, 2 or 4 bytes long.
        /// </exception>
        public static uint Extract(this byte[] val, int startbit, int bitcount)
        {
            if (val is null) throw new ArgumentNullException(nameof(val), "Null byte array");
            switch (val.Length)
            {
            case 1:
                return val[0].Extract(startbit, bitcount);
            case 2:
                return BitConverter.ToUInt16(val, 0).Extract(startbit, bitcount);
            case 4:
                return BitConverter.ToUInt32(val, 0).Extract(startbit, bitcount);
            }
            throw new ArgumentOutOfRangeException(nameof(val), "Invalid byte array. Must be a 1, 2 or 4 bytes array");
        }

        /// <summary>   Extracts the bits field from the specified byte array and sign-extend. </summary>
        ///
        /// <param name="val">      The specified byte array of 1, 2 or 4 bytes. </param>
        /// <param name="startbit"> Lowest bit to extract. </param>
        /// <param name="bitcount"> Number of bits to extract. </param>
        ///
        /// <returns>   A signed 32-bit integer. </returns>
        ///
        /// <remarks>   No check is done on the validity of the bit start and bit count values. </remarks>
        ///
        /// <exception cref="ArgumentNullException">Parameter <paramref name="val"/> is null.</exception>
        /// <exception cref="ArgumentException">If the array is not 1, 2 or 4 bytes long.</exception>
        public static int ExtractSignExtend(this byte[] val, int startbit, int bitcount)
        {
            if (val is null) throw new ArgumentNullException(nameof(val), "Null byte array");
            switch (val.Length)
            {
            case 1:
                return val[0].ExtractSignExtend(startbit, bitcount);
            case 2:
                return BitConverter.ToUInt16(val, 0).ExtractSignExtend(startbit, bitcount);
            case 4:
                return BitConverter.ToUInt32(val, 0).ExtractSignExtend(startbit, bitcount);
            }
            throw new ArgumentException(nameof(val), "Invalid byte array. Must be a 1, 2 or 4 bytes array");
        }

        #endregion

        #region Bits Count

        /// <summary>
        /// Returns the number of ones bits in this unsigned integer.
        /// </summary>
        /// <param name="ui">The unsigned integer value to act on.</param>
        /// <returns>
        /// The total number of one-bits.
        /// </returns>
        public static int CountBits(this uint ui)
        {
            unchecked
            {
                ui -= ((ui >> 1) & 0x55555555u);
                ui = (ui & 0x33333333u) + ((ui >> 2) & 0x33333333u);
                ui = (ui + (ui >> 4)) & 0x0f0f0f0f;
                ui += (ui >> 8);
                ui += (ui >> 16);
                return (int) ui & 0x3f;
            }
        }

        /// <summary>
        /// Returns the number of ones bits in this integer.
        /// </summary>
        /// <param name="i">The integer value to act on.</param>
        /// <returns>
        /// The total number of one-bits.
        /// </returns>
        public static int CountBits(this int i)
            => CountBits(unchecked((uint) i));

        /// <summary>
        /// Returns the number of ones bits in this unsigned long integer.
        /// </summary>
        /// <param name="ul">The unsigned long integer to act on.</param>
        /// <returns>
        /// The total number of one-bits.
        /// </returns>
        public static int CountBits(this ulong ul)
        {
            unchecked
            {
                const ulong MASK_01010101010101010101010101010101 = 0x5555555555555555UL;
                const ulong MASK_00110011001100110011001100110011 = 0x3333333333333333UL;
                const ulong MASK_00001111000011110000111100001111 = 0x0F0F0F0F0F0F0F0FUL;
                const ulong MASK_00000000111111110000000011111111 = 0x00FF00FF00FF00FFUL;
                const ulong MASK_00000000000000001111111111111111 = 0x0000FFFF0000FFFFUL;
                const ulong MASK_11111111111111111111111111111111 = 0x00000000FFFFFFFFUL;
                ul = (ul & MASK_01010101010101010101010101010101) + ((ul >> 1) & MASK_01010101010101010101010101010101);
                ul = (ul & MASK_00110011001100110011001100110011) + ((ul >> 2) & MASK_00110011001100110011001100110011);
                ul = (ul & MASK_00001111000011110000111100001111) + ((ul >> 4) & MASK_00001111000011110000111100001111);
                ul = (ul & MASK_00000000111111110000000011111111) + ((ul >> 8) & MASK_00000000111111110000000011111111);
                ul = (ul & MASK_00000000000000001111111111111111) + ((ul >> 16) & MASK_00000000000000001111111111111111);
                ul = (ul & MASK_11111111111111111111111111111111) + ((ul >> 32) & MASK_11111111111111111111111111111111);
                return (int) ul & 0x7F;
            }
        }

        /// <summary>
        /// Returns the number of ones bits in this long integer.
        /// </summary>
        /// <param name="l">The long integer to process.</param>
        /// <returns>
        /// The total number of one-bits.
        /// </returns>
        public static int CountBits(this long l) 
            => CountBits(unchecked((ulong) l));

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order ("leftmost") one-bit in the binary representation
        /// of the specified int value.  Returns 32 if the specified value has no one-bits in its binary representation,
        /// in other words if it is equal to zero.
        /// <para>
        /// Note that this method is closely related to the logarithm base 2. For all positive int values x:
        /// <list type="bullet">
        /// <item><description>floor(log2(x)) = 31 - numberOfLeadingZeros(x)</description></item>
        /// <item><description>ceil(log2(x)) = 32 - numberOfLeadingZeros(x - 1)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="i">The value whose number of leading zeros is to be computed.</param>
        /// <returns>
        /// The number of zero bits preceding the highest-order ("leftmost") one-bit binary representation of the specified
        ///     int value, or 32 if the value is equal to zero.
        /// </returns>
        public static int NumberOfLeadingZeros(this int i)
            => NumberOfLeadingZeros(unchecked((uint) i));

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order ("leftmost") one-bit in the binary representation
        /// of the specified unsigned integer value.  Returns 32 if the specified value has no one-bits in its binary representation,
        /// in other words if it is equal to zero.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that this method is closely related to the logarithm base 2. For all positive int values x:
        /// <list type="bullet">
        /// <item><description>floor(log2(x)) = 31 - numberOfLeadingZeros(x)</description></item>
        /// <item><description>ceil(log2(x)) = 32 - numberOfLeadingZeros(x - 1)</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="ui">The value whose number of leading zeros is to be computed.</param>
        /// <returns>
        /// The number of zero bits preceding the highest-order ("leftmost") one-bit binary representation of the specified
        ///     int value, or 32 if the value is equal to zero.
        /// </returns>
        public static int NumberOfLeadingZeros(this uint ui)
        {
            unchecked
            {
                if (ui == 0)
                    return 32;
                var n = 1;
                if (ui >> 16 == 0) { n += 16; ui <<= 16; }
                if (ui >> 24 == 0) { n += 8; ui <<= 8; }
                if (ui >> 28 == 0) { n += 4; ui <<= 4; }
                if (ui >> 30 == 0) { n += 2; ui <<= 2; }
                if (ui >> 31 == 0) { n += 1; }
                return n;
            }
        }

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the binary representation of the specified int value.
        /// Returns 32 if the specified value has no one-bits in its representation, in other words if it is equal to zero.
        /// </summary>
        /// <param name="i">The value whose number of trailing zeros is to be computed.</param>
        /// <returns>
        /// The number of zero bits following the lowest-order ("rightmost") one-bit in the binary representation
        /// of the specified int value, or 32 if the value is equal to zero.
        /// </returns>
        public static int NumberOfTrailingZeros(this int i)
            => NumberOfTrailingZeros(unchecked((uint) i));

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the binary representation of the specified unsigned integer value.
        /// Returns 32 if the specified value has no one-bits in its representation, in other words if it is equal to zero.
        /// </summary>
        /// <param name="ui">The value whose number of trailing zeros is to be computed.</param>
        /// <returns>
        /// The number of zero bits following the lowest-order ("rightmost") one-bit in the binary representation
        /// of the specified unsigned integer value, or 32 if the value is equal to zero.
        /// </returns>
        public static int NumberOfTrailingZeros(this uint ui)
        {
            unchecked
            {
                if (ui == 0)
                    return 32;
                uint y;
                var n = 31;
                y = ui << 16; if (y != 0) { n -= 16; ui = y; }
                y = ui << 8; if (y != 0) { n -= 8; ui = y; }
                y = ui << 4; if (y != 0) { n -= 4; ui = y; }
                y = ui << 2; if (y != 0) { n -= 2; ui = y; }
                y = ui << 1; if (y != 0) { n -= 1; }
                return n;
            }
        }

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order ("leftmost") one-bit in the binary representation
        /// of the specified long value.  Returns 64 if the specified value has no one-bits in its binary representation,
        /// in other words if it is equal to zero.
        /// <para>
        /// Note that this method is closely related to the logarithm base 2. For all positive int values x:
        /// <list type="bullet">
        /// <item><description>floor(log2(x)) = 63 - numberOfLeadingZeros(x)</description></item>
        /// <item><description>ceil(log2(x)) = 64 - numberOfLeadingZeros(x - 1)</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="l">The value whose number of leading zeros is to be computed.</param>
        /// <returns>
        /// The number of zero bits preceding the highest-order ("leftmost") one-bit binary representation of the specified
        /// long value, or 64 if the value is equal to zero.
        /// </returns>
        public static int NumberOfLeadingZeros(this long l)
            => NumberOfLeadingZeros(unchecked((ulong) l));

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order ("leftmost") one-bit in the binary representation
        /// of the specified unsigned long value.  Returns 64 if the specified value has no one-bits in its binary representation,
        /// in other words if it is equal to zero.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that this method is closely related to the logarithm base 2. For all positive int values x:
        /// <list type="bullet">
        /// <item><description>floor(log2(x)) = 63 - numberOfLeadingZeros(x)</description></item>
        /// <item><description>ceil(log2(x)) = 64 - numberOfLeadingZeros(x - 1)</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="ul">The value whose number of leading zeros is to be computed.</param>
        /// <returns>
        /// The number of zero bits preceding the highest-order ("leftmost") one-bit binary representation of the specified
        /// unsigned long value, or 64 if the value is equal to zero.
        /// </returns>
        public static int NumberOfLeadingZeros(this ulong ul)
        {
            unchecked
            {
                if (ul == 0)
                    return 64;
                var n = 1;
                var x = (uint) (ul >> 32);
                if (x == 0) { n += 32; x = (uint) ul; }
                if (x >> 16 == 0) { n += 16; x <<= 16; }
                if (x >> 24 == 0) { n += 8; x <<= 8; }
                if (x >> 28 == 0) { n += 4; x <<= 4; }
                if (x >> 30 == 0) { n += 2; x <<= 2; }
                if (x >> 31 == 0) { n += 1; }
                return n;
            }
        }

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the binary representation of the specified long value.
        /// Returns 64 if the specified value has no one-bits in its representation, in other words if it is equal to zero.
        /// </summary>
        /// <param name="l">The value whose number of trailing zeros is to be computed.</param>
        /// <returns>
        /// The number of zero bits following the lowest-order ("rightmost") one-bit in the binary representation
        /// of the specified long value, or 64 if the value is equal to zero.
        /// </returns>
        public static int NumberOfTrailingZeros(this long l)
            => NumberOfTrailingZeros(unchecked((ulong) l));

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the binary representation of the specified unsigned long value.
        /// Returns 64 if the specified value has no one-bits in its representation, in other words if it is equal to zero.
        /// </summary>
        /// <param name="ul">The value whose number of trailing zeros is to be computed.</param>
        /// <returns>
        /// The number of zero bits following the lowest-order ("rightmost") one-bit in the binary representation
        /// of the specified unsigned long value, or 64 if the value is equal to zero.
        /// </returns>
        public static int NumberOfTrailingZeros(this ulong ul)
        {
            unchecked
            {
                uint x, y;
                if (ul == 0)
                    return 64;
                var n = 63;
                y = (uint) ul; if (y != 0) { n -= 32; x = y; } else x = (uint) (ul >> 32);
                y = x << 16; if (y != 0) { n -= 16; x = y; }
                y = x << 8; if (y != 0) { n -= 8; x = y; }
                y = x << 4; if (y != 0) { n -= 4; x = y; }
                y = x << 2; if (y != 0) { n -= 2; x = y; }
                y = x << 1; if (y != 0) { n -= 1; }
                return n;
            }
        }

        #endregion

        #region Alignment to a bit position

        /// <summary>
        /// Returns an unsigned integer value of this unsigned integer aligned to <paramref name="alignment"/> parameter.
        /// </summary>
        /// <param name="ui">The unsigned integer to act on.</param>
        /// <param name="alignment">The alignment value.</param>
        /// <returns>
        /// An unsigned integer.
        /// </returns>
        public static uint Align(this uint ui, uint alignment)
        {
            Debug.Assert(CountBits(alignment) == 1);

            var result = ui & ~(alignment - 1);
            if (result == ui)
                return result;
            return result + alignment;
        }

        /// <summary>
        /// Returns an integer value of this unsigned integer aligned to <paramref name="alignment"/> parameter.
        /// </summary>
        /// <param name="i">The integer to act on.</param>
        /// <param name="alignment">The alignment value.</param>
        /// <returns>
        /// An unsigned integer.
        /// </returns>
        public static int Align(this int i, int alignment)
        {
            Debug.Assert(i >= 0 && alignment > 0);
            Debug.Assert(CountBits(alignment) == 1);

            var result = i & ~(alignment - 1);
            if (result == i)
                return result;
            return result + alignment;
        }

        #endregion

        #region Rotate & Reverse

        /// <summary>
        /// Returns the value obtained by rotating the binary representation of the
        /// specified unsigned integer value left by the specified number of bits.  (Bits shifted out of the
        /// left hand, or high-order, side reenter on the right, or low-order.)
        /// </summary>
        /// <param name="ui">The value whose bits are to be rotated left.</param>
        /// <param name="distance">The number of bit positions to rotate left.</param>
        /// <returns>
        /// The value obtained by rotating the binary representation of the specified unsigned integer value left by the specified number of bits.
        /// </returns>
        public static uint RotateLeft(this uint ui, int distance)
            => unchecked((ui << distance) | (ui >> -distance));

        /// <summary>
        /// Returns the value obtained by rotating the binary representation of the
        /// specified int value left by the specified number of bits.  (Bits shifted out of the
        /// left hand, or high-order, side reenter on the right, or low-order.)
        /// </summary>
        /// <param name="i">The value whose bits are to be rotated left.</param>
        /// <param name="distance">The number of bit positions to rotate left.</param>
        /// <returns>
        /// The value obtained by rotating the binary representation of the specified int value left by the specified number of bits.
        /// </returns>
        public static int RotateLeft(this int i, int distance)
            => unchecked((int) RotateLeft((uint) i, distance));

        /// <summary>
        /// Returns the value obtained by rotating the binary representation of the
        /// specified unsigned long value left by the specified number of bits.  (Bits shifted out of the
        /// left hand, or high-order, side reenter on the right, or low-order.)
        /// </summary>
        /// <param name="ul">The value whose bits are to be rotated left.</param>
        /// <param name="distance">The number of bit positions to rotate left.</param>
        /// <returns>
        /// The value obtained by rotating the binary representation of the specified unsigned long value left by the specified number of bits.
        /// </returns>
        public static ulong RotateLeft(this ulong ul, int distance)
            => unchecked((ul << distance) | (ul >> -distance));

        /// <summary>
        /// Returns the value obtained by rotating the binary representation of the
        /// specified long value left by the specified number of bits.  (Bits shifted out of the
        /// left hand, or high-order, side reenter on the right, or low-order.)
        /// </summary>
        /// <param name="l">The value whose bits are to be rotated left.</param>
        /// <param name="distance">The number of bit positions to rotate left.</param>
        /// <returns>
        /// The value obtained by rotating the binary representation of the specified long value left by the specified number of bits.
        /// </returns>
        public static long RotateLeft(this long l, int distance) 
            => unchecked((long) RotateLeft((ulong) l, distance));

        /// <summary>
        /// Returns the value obtained by rotating the binary representation of the
        /// specified unsigned integer value right by the specified number of bits.  (Bits shifted out of the
        /// right hand, or low-order, side reenter on the left, or high-order.)
        /// </summary>
        /// <param name="ui">The value whose bits are to be rotated right.</param>
        /// <param name="distance">The number of bit positions to rotate right.</param>
        /// <returns>
        /// The value obtained by rotating the binary representation of the specified
        /// unsigned integer value right by the specified number of bits.
        /// </returns>
        public static uint RotateRight(this uint ui, int distance)
            => unchecked((ui >> distance) | (ui << -distance));

        /// <summary>
        /// Returns the value obtained by rotating the binary representation of the
        /// specified int value right by the specified number of bits.  (Bits shifted out of the
        /// right hand, or low-order, side reenter on the left, or high-order.)
        /// </summary>
        /// <param name="i">The value whose bits are to be rotated right.</param>
        /// <param name="distance">The number of bit positions to rotate right.</param>
        /// <returns>
        /// The value obtained by rotating the binary representation of the specified
        /// int value right by the specified number of bits.
        /// </returns>
        public static int RotateRight(this int i, int distance) 
            => unchecked((int) RotateRight((uint) i, distance));

        /// <summary>
        /// Returns the value obtained by rotating the binary representation of the
        /// specified unsigned long value right by the specified number of bits.  (Bits shifted out of the
        /// right hand, or low-order, side reenter on the left, or high-order.)
        /// </summary>
        /// <param name="ul">The value whose bits are to be rotated right.</param>
        /// <param name="distance">The number of bit positions to rotate right.</param>
        /// <returns>
        /// The value obtained by rotating the binary representation of the specified
        /// unsigned long value right by the specified number of bits.
        /// </returns>
        public static ulong RotateRight(this ulong ul, int distance)
            => unchecked((ul >> distance) | (ul << -distance));

        /// <summary>
        /// Returns the value obtained by rotating the binary representation of the
        /// specified long value right by the specified number of bits.  (Bits shifted out of the
        /// right hand, or low-order, side reenter on the left, or high-order.)
        /// </summary>
        /// <param name="l">The value whose bits are to be rotated right.</param>
        /// <param name="distance">The number of bit positions to rotate right.</param>
        /// <returns>
        /// The value obtained by rotating the binary representation of the specified
        /// long value right by the specified number of bits.
        /// </returns>
        public static long RotateRight(this long l, int distance) 
            => unchecked((long) RotateRight((ulong) l, distance));

        /// <summary>
        /// Returns the value obtained by reversing the order of the bits in binary
        /// representation of the specified unsigned integer value.
        /// </summary>
        /// <param name="ui">The value to be reversed.</param>
        /// <returns>
        /// The value obtained by reversing order of the bits in the specified unsigned integer value.
        /// </returns>
        public static uint Reverse(this uint ui)
        {
            unchecked
            {
                ui = (ui & 0x55555555) << 1 | (ui >> 1) & 0x55555555;
                ui = (ui & 0x33333333) << 2 | (ui >> 2) & 0x33333333;
                ui = (ui & 0x0f0f0f0f) << 4 | (ui >> 4) & 0x0f0f0f0f;
                ui = (ui << 24) | ((ui & 0xff00) << 8) | ((ui >> 8) & 0xff00) | (ui >> 24);
                return ui;
            }
        }

        /// <summary>
        /// Returns the value obtained by reversing the order of the bits in binary
        /// representation of the specified int value.
        /// </summary>
        /// <param name="i">The value to be reversed.</param>
        /// <returns>
        /// The value obtained by reversing order of the bits in the specified int value.
        /// </returns>
        public static int Reverse(this int i)
        {
            unchecked { return (int) Reverse(unchecked((uint) i)); }
        }

        /// <summary>
        /// Returns the value obtained by reversing the order of the bits in binary
        /// representation of the specified unsigned long value.
        /// </summary>
        /// <param name="ul">The value to be reversed.</param>
        /// <returns>
        /// The value obtained by reversing order of the bits in the specified unsigned long value.
        /// </returns>
        public static ulong Reverse(this ulong ul)
        {
            unchecked
            {
                ulong msw = Reverse((uint) ((ul >> 32) & 0xFFFFFFFF));
                ulong lsw = Reverse((uint) (ul & 0xFFFFFFFF));
                var res = lsw << 32;
                return res | msw;
            }
        }

        /// <summary>
        /// Returns the value obtained by reversing the order of the bits in binary
        /// representation of the specified long value.
        /// </summary>
        /// <param name="l">The value to be reversed.</param>
        /// <returns>
        /// The value obtained by reversing order of the bits in the specified long value.
        /// </returns>
        public static long Reverse(this long l)
        {
            unchecked { return (long) Reverse(unchecked((ulong) l)); }
        }

        /// <summary>
        /// Returns the signum function of the specified int value.  (The return value is -1 if
        /// the specified value is negative; 0 if the specified value is zero; and 1 if the specified
        /// value is positive.)
        /// </summary>
        /// <param name="i">The value whose signum is to be computed.</param>
        /// <returns>
        /// The signum function of the specified int value.
        /// </returns>
        public static int Signum(this int i) 
            => unchecked((i >> 31) | ((int) ((uint) -i >> 31)));

        /// <summary>
        /// Returns the Signum function of the specified long value.  (The return value is -1 if
        /// the specified value is negative; 0 if the specified value is zero; and 1 if the specified
        /// value is positive.)
        /// </summary>
        /// <param name="l">The value whose Signum is to be computed.</param>
        /// <returns>
        /// The Signum function of the specified long value.
        /// </returns>
        public static long Signum(this long l) 
            => unchecked((l >> 63) | ((long) ((ulong) -l >> 63)));

        #endregion

        #region String conversions

        /// <summary>
        /// This extension method converts a bit field value to an equivalent binary string.
        /// </summary>
        /// <param name="field">The field value to convert. Right justified.</param>
        /// <param name="width">The bit field width in bits.</param>
        /// <param name="sset">The 'set' (one) representation character.</param>
        /// <param name="sclr">The 'clear' (zero) representation character. Default is '-'.</param>
        /// <returns>
        /// A string representing the bit field value in base 2.
        /// </returns>
        public static string ToStringField(this int field, int width, char sset, char sclr = '-')
        {
            var sb = new StringBuilder(width);
            field &= (1 << width) - 1;
            while (width > 0)
            {
                sb.Insert(0, ((field & 1) == 0 ? sclr : sset));
                field >>= 1;
                width--;
            }
            return sb.ToString();
        }

        /// <summary>
        /// This extension method converts a String containing <paramref name="sset"/> chars to an equivalent bit field value.
        /// </summary>
        /// <param name="sField">The String to act on.</param>
        /// <param name="width">The bit field width in bits.</param>
        /// <param name="sset">The 'set' (one) representation character.</param>
        /// <returns>
        /// The given data converted to an integer representing the field value (right justified)
        /// </returns>
        public static int ToBitField(this string sField, int width, char sset)
        {
            if (string.IsNullOrEmpty(sField))
                return 0;
            var field = 0;
            var lfield = sField.Length;
            width = Math.Min(lfield, width);
            for (var i = lfield - width; i < lfield; i++)
            {
                field <<= 1;
                if (sField[i] == sset)
                    field |= 1;
            }
            return field;
        }

        #endregion

        #region Range checking

        #region IsInRange

        /// <summary>
        /// Checks whether the specified signed byte is within a specified value range.
        /// </summary>
        /// <param name="val">The signed byte to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this sbyte val, sbyte low, sbyte high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified signed short integer is within a specified value range.
        /// </summary>
        /// <param name="val">The signed short integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this short val, int low, int high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified signed integer is within a specified value range.
        /// </summary>
        /// <param name="val">The signed integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this int val, int low, int high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified signed long integer is within a specified value range.
        /// </summary>
        /// <param name="val">The signed long integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this long val, long low, long high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified (unsigned) byte is within a specified value range.
        /// </summary>
        /// <param name="val">The (unsigned) byte to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this byte val, byte low, byte high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified unsigned short integer is within a specified value range.
        /// </summary>
        /// <param name="val">The unsigned short integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this ushort val, ushort low, ushort high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified unsigned integer is within a specified value range.
        /// </summary>
        /// <param name="val">The unsigned integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this uint val, uint low, uint high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified unsigned long integer is within a specified value range.
        /// </summary>
        /// <param name="val">The unsigned long integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this ulong val, ulong low, ulong high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified float is within a specified value range.
        /// </summary>
        /// <param name="val">The float to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this float val, float low, float high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified double is within a specified value range.
        /// </summary>
        /// <param name="val">The double to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this double val, double low, double high)
            => !(val < low || val > high);

        /// <summary>
        /// Checks whether the specified decimal is within a specified value range.
        /// </summary>
        /// <param name="val">The decimal to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>Boolean true if value is in range; false otherwise.</returns>
        /// 
        public static bool IsInRange(this decimal val, decimal low, decimal high)
            => !(val < low || val > high);

        #endregion

        #region InRange

        /// <summary>
        /// Check whether the specified signed byte is within a specified value range.
        /// </summary>
        /// <param name="val">The signed byte to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The signed byte value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static sbyte InRange(this sbyte val, sbyte low, sbyte high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Signed byte value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified signed short integer is within a specified value range.
        /// </summary>
        /// <param name="val">The signed short integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The signed short integer value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static short InRange(this short val, int low, int high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Short value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified signed integer is within a specified value range.
        /// </summary>
        /// <param name="val">The signed integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The signed integer value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static int InRange(this int val, int low, int high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Integer value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified signed long integer is within a specified value range.
        /// </summary>
        /// <param name="val">The signed long integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The signed long integer value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static long InRange(this long val, long low, long high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Long integer value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified (unsigned) byte is within a specified value range.
        /// </summary>
        /// <param name="val">The (unsigned) byte to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The (unsigned) byte value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static byte InRange(this byte val, byte low, byte high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Byte value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified unsigned short integer is within a specified value range.
        /// </summary>
        /// <param name="val">The unsigned short integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The unsigned short integer value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static ushort InRange(this ushort val, ushort low, ushort high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Unsigned short integer value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified unsigned integer is within a specified value range.
        /// </summary>
        /// <param name="val">The unsigned integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The unsigned integer value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static uint InRange(this uint val, uint low, uint high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Unsigned integer value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified unsigned long integer is within a specified value range.
        /// </summary>
        /// <param name="val">The unsigned long integer to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The unsigned long integer value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static ulong InRange(this ulong val, ulong low, ulong high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Unsigned long integer value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified float is within a specified value range.
        /// </summary>
        /// <param name="val">The float to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The float value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static float InRange(this float val, float low, float high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Float value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified double is within a specified value range.
        /// </summary>
        /// <param name="val">The double to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The double value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static double InRange(this double val, double low, double high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Double value not between {low} and {high}");
        }

        /// <summary>
        /// Check whether the specified decimal is within a specified value range.
        /// </summary>
        /// <param name="val">The decimal to check.</param>
        /// <param name="low">The lowest value of the range.</param>
        /// <param name="high">The highest value of the range.</param>
        /// <returns>The decimal value, if valid.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> in case the check fails.</exception>
        /// 
        public static decimal InRange(this decimal val, decimal low, decimal high)
        {
            if (val.IsInRange(low, high))
                return val;
            throw new ArgumentOutOfRangeException(nameof(val), val, $"Decimal value not between {low} and {high}");
        }

        #endregion

        #endregion

    }

}

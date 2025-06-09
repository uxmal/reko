#region License
/* 
 *
 * Copyrighted (c) 2017-2025 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.netbeans.org/cddl.html
 * or http://www.gnu.org/licenses/gpl-2.0.html.
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
 * When distributing Covered Code, include this CDDL Header Notice in each file
 * and include the License file at http://www.netbeans.org/cddl.txt.
 * If applicable, add the following below the CDDL Header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */
#endregion

namespace Reko.Libraries.Microchip
{
    using System;
    using System.Text;
    using System.Globalization;

    using static System.Convert;
    using static System.Math;

    /// <summary>
    /// Values that represent Conversion bases.
    /// </summary>
    public enum ConvBase : int
    {

        /// <summary> Constant representing the binary conversion option. </summary>
        Binary = 2,
        /// <summary> Constant representing the octal conversion option. </summary>
        Octal = 8,
        /// <summary> Constant representing the decimal conversion option. </summary>
        Decimal = 10,
        /// <summary> Constant representing the hexadecimal conversion option. </summary>
        Hexadecimal = 16
    }

    /// <summary>
    /// Values that represent conversion suffixes.
    /// </summary>
    public enum ConvSuffix : byte
    {
        /// <summary>Use a short suffix (KB, MB, ...) </summary>
        Sht,
        /// <summary>Use a long suffix (kilobytes, megabytes, ...) </summary>
        Lng
    }

    public static partial class ConvUtils
    {
        #region Locals

        private const string defaultNullDisplayValue = "?";
        private static string nullDisplayValue = defaultNullDisplayValue;

        // The power of 1024 suffixes. Uses "usual" notation (not IEC)
        private static readonly string[] cvShtSuffix = { "", " KB", " MB", " GB", " TB", " PB", " EB", " ZB", " YB" };
        private static readonly string[] cvLngSuffix = { " byte", " kilobyte", " megabyte", " gigabyte", " terabyte", " petabyte", " exabyte", " zettabyte", " yottabyte" };
        private static readonly Func<ConvSuffix, string[]> getConvSuffix =
            ((esuf) => (esuf == ConvSuffix.Lng ? cvLngSuffix : cvShtSuffix));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ConvUtils() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the string displayed for any nullable value being null.
        /// </summary>
        public static string NullDispValue
        {
            get => nullDisplayValue ?? defaultNullDisplayValue;
            set => nullDisplayValue = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the string conversion must be made in uppercase letters.
        /// </summary>
        /// <value>
        /// True if convert to uppercase, false if not. Default is true.
        /// </value>
        public static bool ConvertUpper { get; set; } = true;

        #endregion

        #region Helpers

        private static uint toUInt32Throw(string sNumber)
        {
            var s = sNumber.Trim();
            if (s.Length == 0)
                throw new FormatException("Empty number");
            if (s.Length >= 2 && s[0] == '0')
            {
                if (s.Length >= 3)
                {
                    if (s.StartsWith("0X", StringComparison.InvariantCultureIgnoreCase))
                        return ToUInt32(s, 16);
                    if (s.StartsWith("0B", StringComparison.InvariantCultureIgnoreCase))
                        return ToUInt32(s.Substring(2), 2);
                }
                return ToUInt32(s, 8);
            }
            return ToUInt32(s, 10);
        }

        private static int toInt32Throw(string sNumber)
        {
            var s = sNumber.Trim();
            if (s.Length == 0)
                throw new FormatException("Empty number");
            if (s[0] == '+')
                return toInt32Throw(s.Substring(1));
            if (s[0] == '-')
                return -toInt32Throw(s.Substring(1));
            if (s.Length >= 2 && s[0] == '0')
            {
                if (s.Length >= 3)
                {
                    if (s.StartsWith("0X", StringComparison.InvariantCultureIgnoreCase))
                        return ToInt32(s, 16);
                    if (s.StartsWith("0B", StringComparison.InvariantCultureIgnoreCase))
                        return ToInt32(s.Substring(2), 2);
                }
                return ToInt32(s, 8);
            }
            return ToInt32(s, 10);
        }

        private static string getPrefix(ConvBase eToBase, bool withprefix)
        {
            if (withprefix)
            {
                switch (eToBase)
                {
                case ConvBase.Binary:
                    return "0b";
                case ConvBase.Octal:
                    return "0";
                case ConvBase.Hexadecimal:
                    return "0x";
                }
            }
            return "";
        }

        private static string toStr(byte iNumber, ConvBase eBase)
        {
            var s = Convert.ToString(iNumber, (int) eBase);
            return (ConvertUpper ? s.ToUpperInvariant() : s);
        }

        private static string toStr(short iNumber, ConvBase eBase)
        {
            var s = Convert.ToString(iNumber, (int) eBase);
            return (ConvertUpper ? s.ToUpperInvariant() : s);
        }

        private static string toStr(ushort iNumber, ConvBase eBase)
        {
            var s = Convert.ToString(iNumber, (int) eBase);
            return (ConvertUpper ? s.ToUpperInvariant() : s);
        }

        private static string toStr(int iNumber, ConvBase eBase)
        {
            var s = Convert.ToString(iNumber, (int) eBase);
            return (ConvertUpper ? s.ToUpperInvariant() : s);
        }

        private static string toStr(uint iNumber, ConvBase eBase)
        {
            var s = Convert.ToString(iNumber, (int) eBase);
            return (ConvertUpper ? s.ToUpperInvariant() : s);
        }

        private static string toStr(long iNumber, ConvBase eBase)
        {
            var s = Convert.ToString(iNumber, (int) eBase);
            return (ConvertUpper ? s.ToUpperInvariant() : s);
        }

        private static ulong toUInt64Throw(string s)
        {
            if (s.Length >= 2 & s[0] == '0')
            {
                if (s.Length >= 3)
                {
                    if (s.StartsWith("0X", StringComparison.InvariantCultureIgnoreCase))
                        return ToUInt64(s, 16);
                    if (s.StartsWith("0B", StringComparison.InvariantCultureIgnoreCase))
                        return ToUInt64(s.Substring(2), 2);
                }
                return ToUInt64(s, 8);
            }
            return ToUInt64(s, 10);
        }

        private static long toInt64Throw(string sNumber)
        {
            var s = sNumber.Trim();
            if (s.Length == 0)
                throw new FormatException("Empty number");
            if (s[0] == '+')
                return toInt64Throw(s.Substring(1));
            if (s[0] == '-')
                return -toInt64Throw(s.Substring(1));
            if (s.Length >= 2 && s[0] == '0')
            {
                if (s.Length >= 3)
                {
                    if (s.StartsWith("0X", StringComparison.InvariantCultureIgnoreCase))
                        return ToInt64(s, 16);
                    if (s.StartsWith("0B", StringComparison.InvariantCultureIgnoreCase))
                        return ToInt64(s.Substring(2), 2);
                }

                return ToInt64(s, 8);
            }
            return ToInt64(s, 10);
        }

        #endregion

        #region Extensions methods

        #region Boolean

        /// <summary>
        /// Converts the string representation of a boolean to an equivalent boolean.
        /// </summary>
        /// <param name="sNumber">A string that contains the boolean value for a true result.</param>
        /// <returns>Boolean value.</returns>
        /// 
        public static bool ToBooleanEx(this string sNumber)
        {
            if (sNumber is null)
                return false; ;
            var s = sNumber.Trim();
            return (s.Equals("true", StringComparison.InvariantCultureIgnoreCase) ||
                    s.Equals("y", StringComparison.InvariantCultureIgnoreCase) ||
                    s.Equals("1"));
        }

        /// <summary>
        /// Converts the string representation of a boolean to an equivalent nullable boolean.
        /// </summary>
        /// <param name="sNumber">A string that contains the boolean value for a true result.</param>
        /// <returns>Nullable boolean value. Will be null if the string is null/empty.</returns>
        /// 
        public static bool? NullableToBooleanEx(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return null;
            if (sNumber.Equals(NullDispValue))
                return null;
            return ToBooleanEx(sNumber);
        }

        #endregion

        #region Byte

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent unsigned 8-bit integer
        /// (byte).
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary
        ///                       (prefix 0b or 0B) number to convert.</param>
        /// <returns>
        /// An unsigned 8-bit integer (byte).
        /// </returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a byte.</exception>
        public static byte ToByteEx(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return 0;
            var res = toUInt32Throw(sNumber.Trim());
            if (res > byte.MaxValue)
                throw new OverflowException("Value cannot fit in a byte.");
            return (byte) res;
        }

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent nullable unsigned 8-bit integer (byte).
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.</param>
        /// <returns>A nullable unsigned 8-bit integer (byte).</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a byte.</exception>
        /// 
        public static byte? ToNullableByteEx(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return null;
            var s = sNumber.Trim();
            if (s.Equals(NullDispValue))
                return null;
            uint res;
            try
            {
                res = toUInt32Throw(s);
            }
            catch (OverflowException)
            {
                throw new OverflowException("Value cannot fit in a byte.");
            }
            catch
            {
                return null;
            }
            if (res > byte.MaxValue)
                throw new OverflowException("Value cannot fit in a byte.");
            return (byte?) res;
        }

        /// <summary>
        /// Converts the specified byte to an even value (extension).
        /// </summary>
        /// <param name="val">The byte to convert.</param>
        /// <returns>The truncated even value corresponding to this byte.</returns>
        public static byte ToEven(this byte val) => unchecked((byte) (val & ~1));

        /// <summary>
        /// Converts the specified nullable byte to an even value (extension).
        /// </summary>
        /// <param name="val">The nullable byte to convert.</param>
        /// <returns>The truncated even value corresponding to this nullable byte.</returns>
        public static byte? ToEven(this byte? val) => (val.HasValue ? unchecked((byte?) (val & ~1)) : null);

        /// <summary>
        /// Converts the specified byte to an odd value (extension).
        /// </summary>
        /// <param name="val">The byte to convert.</param>
        /// <returns>The rounded odd value corresponding to this byte.</returns>
        public static byte ToOdd(this byte val) => unchecked((byte) (val | 1));

        /// <summary>
        /// Converts the specified nullable byte to an odd value (extension).
        /// </summary>
        /// <param name="val">The nullable byte to convert.</param>
        /// <returns>The rounded odd value corresponding to this nullable byte.</returns>
        public static byte? ToOdd(this byte? val) => (val.HasValue ? unchecked((byte?) (val | 1)) : null);

        /// <summary>
        /// Converts the value of an unsigned byte to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The unsigned byte to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>.</returns>
        /// 
        public static string ToStringEx(this byte iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
            => getPrefix(eToBase, withprefix) +
               toStr(iNumber, eToBase).PadLeft((numdigits < 0 ? 0 : numdigits), '0');

        /// <summary>
        /// Converts the value of a nullable byte to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The nullable byte to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>. Returns '0' if the byte is null.</returns>
        /// 
        public static string ToStringEx(this byte? iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
            => (iNumber.HasValue ? iNumber.Value.ToStringEx(eToBase, numdigits, withprefix) : NullDispValue);

        #endregion

        #region Int16

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent signed 16-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.
        /// A negative sign can prefix this string.</param>
        /// <returns>A signed 16-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 16-bit integer.</exception>
        /// 
        public static short ToInt16Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return 0;
            var res = toInt32Throw(sNumber.Trim());
            if (res < short.MinValue || res > short.MaxValue)
                throw new OverflowException("Value cannot fit in a signed 16-bit integer.");
            return (short) res;
        }

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent nullable signed 16-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.
        /// A negative sign can prefix this string.</param>
        /// <returns>A nullable signed 16-bit integer.</returns>
        /// 
        public static short? ToNullableInt16Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return null;
            var s = sNumber.Trim();
            if (s.Equals(NullDispValue))
                return null;
            int res;
            try
            {
                res = toInt32Throw(s);
            }
            catch (OverflowException)
            {
                throw new OverflowException("Value cannot fit in a nullable 16-bit signed integer.");
            }
            catch
            {
                return null;
            }
            if (res < short.MinValue || res > short.MaxValue)
                throw new OverflowException("Value cannot fit in a nullable 16-bit signed integer.");
            return (short?) res;
        }

        /// <summary>
        /// Converts the specified short integer to an even value (extension).
        /// </summary>
        /// <param name="val">The short integer to convert.</param>
        /// <returns>The truncated even value corresponding to this short integer.</returns>
        public static short ToEven(this short val) => unchecked((short) (val & ~1));

        /// <summary>
        /// Converts the specified nullable short integer to an even value (extension).
        /// </summary>
        /// <param name="val">The nullable short integer to convert.</param>
        /// <returns>The truncated even value corresponding to this nullable short integer.</returns>
        public static short? ToEven(this short? val) => (val.HasValue ? unchecked((short?) (val & ~1)) : null);

        /// <summary>
        /// Converts the specified short integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The short integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this short integer.</returns>
        public static short ToOdd(this short val) => unchecked((short) (val | 1));

        /// <summary>
        /// Converts the specified nullable short integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The nullable short integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this nullable short integer.</returns>
        public static short? ToOdd(this short? val) => (val.HasValue ? unchecked((short?) (val | 1)) : null);

        /// <summary>
        /// Converts the value of an 16-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 16-bit integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>.</returns>
        /// 
        public static string ToStringEx(this short iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
            => getPrefix(eToBase, withprefix) +
               toStr(iNumber, eToBase).PadLeft((numdigits < 0 ? 0 : numdigits), '0');

        /// <summary>
        /// Converts the value of a nullable 16-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 16-bit nullable integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>. Returns '0' if the integer is null.</returns>
        /// 
        public static string ToStringEx(this short? iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
            => (iNumber.HasValue ? iNumber.Value.ToStringEx(eToBase, numdigits, withprefix) : NullDispValue);

        /// <summary>
        /// Converts the value of a 16-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The 16-bit integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffix (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this short iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
        {
            if (iNumber == 0)
                return "0" + getConvSuffix(eSuffix)[0];
            iDecimalPlace = Min(Max(0, iDecimalPlace), 15);
            var iAbsNumber = Abs(iNumber);
            var nPower = (int) Floor(Log(iAbsNumber, 1024));
            var dDispQuantity = Round(iAbsNumber / Pow(1024, nPower), iDecimalPlace);
            var bPlural = (eSuffix == ConvSuffix.Lng) && (dDispQuantity > 1.0);
            return (Sign(iNumber) * dDispQuantity).ToString("F" + iDecimalPlace, CultureInfo.InvariantCulture) + getConvSuffix(eSuffix)[nPower] + (bPlural ? "s" : "");
        }

        /// <summary>
        /// Converts the value of a nullable 16-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The 16-bit nullable integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KB, MB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kilobytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffix (KB, MB, GB, etc...)</returns>
        /// 
        public static string ToPower1K(this short? iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
            => (iNumber.HasValue ? ToPower1K(iNumber.Value, iDecimalPlace, eSuffix) : NullDispValue);

        #endregion

        #region UInt16

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent unsigned 16-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.</param>
        /// <returns>An unsigned 16-bit integer.</returns>
        /// <exception cref="ArgumentNullException">if the <paramref name="sNumber"/> is null.</exception>
        /// <exception cref="ArgumentException"> - <see cref="System.Convert.ToUInt32(string, int)"/></exception>
        /// <exception cref="FormatException"> - <see cref="System.Convert.ToUInt32(string, int)"/></exception>
        /// <exception cref="OverflowException"> - <see cref="System.Convert.ToUInt32(string, int)"/></exception>
        /// 
        public static ushort ToUInt16Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return 0;
            var res = toUInt32Throw(sNumber.Trim());
            if (res > ushort.MaxValue)
                throw new OverflowException("Value cannot fit in an unsigned 16-bit integer.");
            return (ushort) res;
        }

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent nullable unsigned 16-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.</param>
        /// <returns>A nullable unsigned 16-bit integer.</returns>
        /// 
        public static ushort? ToNullableUInt16Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return null;
            var s = sNumber.Trim();
            if (s.Equals(NullDispValue))
                return null;
            uint res;
            try
            {
                res = toUInt32Throw(s);
            }
            catch (OverflowException)
            {
                throw new OverflowException("Value cannot fit in a nullable 16-bit unsigned integer.");
            }
            catch
            {
                return null;
            }
            if (res > ushort.MaxValue)
                throw new OverflowException("Value cannot fit in a nullable 16-bit unsigned integer.");
            return (ushort) res;
        }

        /// <summary>
        /// Converts the specified unsigned short integer to an even value (extension).
        /// </summary>
        /// <param name="val">The unsigned short integer to convert.</param>
        /// <returns>The truncated even value corresponding to this unsigned short integer.</returns>
        public static ushort ToEven(this ushort val) => unchecked((ushort) (val & ~1));

        /// <summary>
        /// Converts the specified nullable unsigned short integer to an even value (extension).
        /// </summary>
        /// <param name="val">The nullable unsigned short integer to convert.</param>
        /// <returns>The truncated even value corresponding to this nullable unsigned short integer.</returns>
        public static ushort? ToEven(this ushort? val) => (val.HasValue ? unchecked((ushort?) (val & ~1)) : null);

        /// <summary>
        /// Converts the specified unsigned short integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The unsigned short integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this unsigned short integer.</returns>
        public static ushort ToOdd(this ushort val) => unchecked((ushort) (val | 1));

        /// <summary>
        /// Converts the specified nullable unsigned short integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The nullable unsigned short integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this nullable unsigned short integer.</returns>
        public static ushort? ToOdd(this ushort? val) => (val.HasValue ? unchecked((ushort?) (val | 1)) : null);

        /// <summary>
        /// Converts the value of an unsigned 16-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 16-bit unsigned integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>.</returns>
        /// 
        public static string ToStringEx(this ushort iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
            => getPrefix(eToBase, withprefix) +
               toStr(iNumber, eToBase).PadLeft((numdigits < 0 ? 0 : numdigits), '0');

        /// <summary>
        /// Converts the value of an unsigned nullable 16-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 16-bit nullable unsigned integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>. Returns '0' if the integer is null.</returns>
        /// 
        public static string ToStringEx(this ushort? iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
            => (iNumber.HasValue ? iNumber.Value.ToStringEx(eToBase, numdigits, withprefix) : NullDispValue);

        /// <summary>
        /// Converts the value of an unsigned 16-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The unsigned 16-bit integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffix (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this ushort iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
        {
            if (iNumber == 0)
                return "0" + getConvSuffix(eSuffix)[0];
            iDecimalPlace = Min(Max(0, iDecimalPlace), 15);
            var nPower = (int) Floor(Log(iNumber, 1024));
            var dDispQuantity = Round(iNumber / Pow(1024, nPower), iDecimalPlace);
            var bPlural = (eSuffix == ConvSuffix.Lng) && (dDispQuantity > 1.0);
            return (dDispQuantity).ToString("F" + iDecimalPlace, CultureInfo.InvariantCulture) + getConvSuffix(eSuffix)[nPower] + (bPlural ? "s" : "");
        }

        /// <summary>
        /// Converts the value of a nullable unsigned 16-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The nullable unsigned 16-bit integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffix (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this ushort? iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
            => (iNumber.HasValue ? ToPower1K(iNumber.Value, iDecimalPlace, eSuffix) : NullDispValue);

        #endregion

        #region Int32

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent signed 32-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.
        /// A negative sign can prefix this string.</param>
        /// <returns>A signed 32-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 32-bit integer.</exception>
        /// 
        public static int ToInt32Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return 0;
            return toInt32Throw(sNumber.Trim());
        }

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent nullable signed 32-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.
        /// A negative sign can prefix this string.</param>
        /// <returns>A nullable signed 32-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 32-bit integer.</exception>
        /// 
        public static int? ToNullableInt32Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return null;
            var s = sNumber.Trim();
            if (s.Equals(NullDispValue))
                return null;
            int res;
            try
            {
                res = toInt32Throw(s);
            }
            catch (OverflowException)
            {
                throw new OverflowException("Value cannot fit in a nullable 32-bit signed integer.");
            }
            catch
            {
                return null;
            }
            return res;
        }

        /// <summary>
        /// Converts the specified 32-bit signed integer to an even value (extension).
        /// </summary>
        /// <param name="val">The 32-bit signed integer to convert.</param>
        /// <returns>The truncated even value corresponding to this 32-bit signed integer.</returns>
        public static int ToEven(this int val) => unchecked((int) (val & ~1));

        /// <summary>
        /// Converts the specified nullable 32-bit signed integer to an even value (extension).
        /// </summary>
        /// <param name="val">The nullable 32-bit signed integer to convert.</param>
        /// <returns>The truncated even value corresponding to this nullable 32-bit signed integer.</returns>
        public static int? ToEven(this int? val) => (val.HasValue ? unchecked((int?) (val & ~1)) : null);

        /// <summary>
        /// Converts the specified 32-bit signed integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The 32-bit signed integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this 32-bit signed integer.</returns>
        public static int ToOdd(this int val) => unchecked((int) (val | 1));

        /// <summary>
        /// Converts the specified nullable 32-bit signed integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The nullable 32-bit signed integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this nullable 32-bit signed integer.</returns>
        public static int? ToOdd(this int? val) => (val.HasValue ? unchecked((int?) (val | 1)) : null);

        /// <summary>
        /// Converts the value of a 32-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 32-bit integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>.</returns>
        /// 
        public static string ToStringEx(this int iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
            => getPrefix(eToBase, withprefix) +
               toStr(iNumber, eToBase).PadLeft((numdigits < 0 ? 0 : numdigits), '0');

        /// <summary>
        /// Converts the value of a nullable 32-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 32-bit nullable integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>. Returns '0' if the integer is null.</returns>
        /// 
        public static string ToStringEx(this int? iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = -1, bool withprefix = false)
            => (iNumber.HasValue ? iNumber.Value.ToStringEx(eToBase, numdigits, withprefix) : NullDispValue);

        /// <summary>
        /// Converts the value of a 32-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The 32-bit integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffix (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this int iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
        {
            if (iNumber == 0)
                return "0" + getConvSuffix(eSuffix)[0];
            iDecimalPlace = Min(Max(0, iDecimalPlace), 15);
            var iAbsNumber = Abs(iNumber);
            var nPower = (int) Floor(Log(iAbsNumber, 1024));
            var dDispQuantity = Round(iAbsNumber / Pow(1024, nPower), iDecimalPlace);
            var bPlural = (eSuffix == ConvSuffix.Lng) && (dDispQuantity > 1.0);
            return (Sign(iNumber) * dDispQuantity).ToString("F" + iDecimalPlace, CultureInfo.InvariantCulture) + getConvSuffix(eSuffix)[nPower] + (bPlural ? "s" : "");
        }

        /// <summary>
        /// Converts the value of a nullable 32-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The 32-bit nullable integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffix (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this int? iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
            => (iNumber.HasValue ? ToPower1K(iNumber.Value, iDecimalPlace, eSuffix) : NullDispValue);

        #endregion

        #region UInt32

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent unsigned 32-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.</param>
        /// <returns>An unsigned 32-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 32-bit unsigned integer.</exception>
        /// 
        public static uint ToUInt32Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return 0;
            return toUInt32Throw(sNumber.Trim());
        }

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent nullable unsigned 32-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.</param>
        /// <returns>A nullable unsigned 32-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 32-bit unsigned integer.</exception>
        /// 
        public static uint? ToNullableUInt32Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return null;
            var s = sNumber.Trim();
            if (s.Equals(NullDispValue))
                return null;
            uint res;
            try
            {
                res = toUInt32Throw(s);
            }
            catch (OverflowException)
            {
                throw new OverflowException("Value cannot fit in a nullable 32-bit unsigned integer.");
            }
            catch
            {
                return null;
            }
            return (uint?) res;
        }

        #endregion

        #region Int64

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent signed 64-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.
        /// A negative sign can prefix this string.</param>
        /// <returns>A signed 64-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 64-bit integer.</exception>
        /// 
        public static long ToInt64Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return 0;
            return toInt64Throw(sNumber.Trim());
        }

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent nullable signed 64-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.
        /// A negative sign can prefix this string.</param>
        /// <returns>A nullable signed 64-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 64-bit integer.</exception>
        /// 
        public static long? ToNullableInt64Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return null;
            var s = sNumber.Trim();
            if (s.Equals(NullDispValue))
                return null;
            long res;
            try
            {
                res = toInt64Throw(s);
            }
            catch (OverflowException)
            {
                throw new OverflowException("Value cannot fit in a nullable 64-bit signed integer.");
            }
            catch
            {
                return null;
            }
            return (long?) res;
        }

        /// <summary>
        /// Converts the specified 64-bit signed integer to an even value (extension).
        /// </summary>
        /// <param name="val">The 64-bit signed integer to convert.</param>
        /// <returns>The truncated even value corresponding to this 64-bit signed integer.</returns>
        public static long ToEven(this long val) => unchecked((long) (val & ~1L));

        /// <summary>
        /// Converts the specified nullable 64-bit signed integer to an even value (extension).
        /// </summary>
        /// <param name="val">The nullable 64-bit signed integer to convert.</param>
        /// <returns>The truncated even value corresponding to this nullable 64-bit signed integer.</returns>
        public static long? ToEven(this long? val) => (val.HasValue ? unchecked((long?) (val & ~1)) : null);

        /// <summary>
        /// Converts the specified 64-bit signed integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The 64-bit signed integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this 64-bit signed integer.</returns>
        public static long ToOdd(this long val) => unchecked((long) (val | 1L));

        /// <summary>
        /// Converts the specified nullable 64-bit signed integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The nullable 64-bit signed integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this nullable 64-bit signed integer.</returns>
        public static long? ToOdd(this long? val) => (val.HasValue ? unchecked((long?) (val | 1)) : null);

        /// <summary>
        /// Converts the value of a 64-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 64-bit integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>.</returns>
        /// 
        public static string ToStringEx(this long iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
            => getPrefix(eToBase, withprefix) +
               toStr(iNumber, eToBase).PadLeft((numdigits < 0 ? 0 : numdigits), '0');

        /// <summary>
        /// Converts the value of a nullable 64-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 64-bit nullable integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>. Returns '0' if the integer is null.</returns>
        /// 
        public static string ToStringEx(this long? iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = -1, bool withprefix = true)
            => (iNumber.HasValue ? iNumber.Value.ToStringEx(eToBase, numdigits, withprefix) : NullDispValue);

        /// <summary>
        /// Converts the value of a 64-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The 64-bit integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffix (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this long iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
        {
            if (iNumber == 0)
                return "0" + getConvSuffix(eSuffix)[0];
            iDecimalPlace = Min(Max(0, iDecimalPlace), 15);
            var iAbsNumber = Abs(iNumber);
            var nPower = (int) Floor(Log(iAbsNumber, 1024));
            var dDispQuantity = Round(iAbsNumber / Pow(1024, nPower), iDecimalPlace);
            var bPlural = (eSuffix == ConvSuffix.Lng) && (dDispQuantity > 1.0);
            return (Sign(iNumber) * dDispQuantity).ToString("F" + iDecimalPlace, CultureInfo.InvariantCulture) + getConvSuffix(eSuffix)[nPower] + (bPlural ? "s" : "");
        }

        /// <summary>
        /// Converts the value of a nullable 64-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The 64-bit nullable integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffix (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this long? iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
            => (iNumber.HasValue ? ToPower1K(iNumber.Value, iDecimalPlace, eSuffix) : NullDispValue);

        #endregion

        #region UInt64

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent unsigned 64-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.</param>
        /// <returns>An unsigned 64-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 64-bit unsigned integer.</exception>
        /// 
        public static ulong ToUInt64Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return 0UL;
            return toUInt64Throw(sNumber.Trim());
        }

        /// <summary>
        /// Converts a decimal/hexadecimal/binary numeric string to an equivalent nullable unsigned 64-bit integer.
        /// </summary>
        /// <param name="sNumber">A string containing the decimal, hexadecimal (prefix 0x, or 0X) or binary (prefix 0b or 0B) number to convert.</param>
        /// <returns>A nullable unsigned 64-bit integer.</returns>
        /// <exception cref="OverflowException">- In case the value cannot fit in a 64-bit unsigned integer.</exception>
        /// 
        public static ulong? ToNullableUInt64Ex(this string sNumber)
        {
            if (string.IsNullOrWhiteSpace(sNumber))
                return null;
            var s = sNumber.Trim();
            if (s.Equals(NullDispValue))
                return null;
            ulong res;
            try
            {
                res = toUInt64Throw(s);
            }
            catch (OverflowException)
            {
                throw new OverflowException("Value cannot fit in a nullable 64-bit unsigned integer.");
            }
            catch
            {
                return null;
            }
            return (ulong?) res;
        }

        /// <summary>
        /// Converts the specified 64-bit unsigned long integer to an even value.
        /// </summary>
        /// <param name="val">The 64-bit unsigned integer to convert.</param>
        /// <returns>The truncated even value corresponding to this 64-bit unsigned integer.</returns>
        public static ulong ToEven(this ulong val) => unchecked((ulong) (val & ~1UL));

        /// <summary>
        /// Converts the specified nullable 64-bit unsigned long integer to an even value.
        /// </summary>
        /// <param name="val">The nullable 64-bit unsigned integer to convert.</param>
        /// <returns>The truncated even value corresponding to this nullable 64-bit unsigned integer.</returns>
        public static ulong? ToEven(this ulong? val) => (val.HasValue ? unchecked((ulong?) (val & ~(ulong) 1)) : null);

        /// <summary>
        /// Converts the specified 64-bit unsigned integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The 64-bit unsigned integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this 64-bit unsigned integer.</returns>
        public static ulong ToOdd(this ulong val) => unchecked((ulong) (val | 1UL));

        /// <summary>
        /// Converts the specified nullable 64-bit unsigned integer to an odd value (extension).
        /// </summary>
        /// <param name="val">The nullable 64-bit unsigned integer to convert.</param>
        /// <returns>The rounded odd value corresponding to this nullable 64-bit unsigned integer.</returns>
        public static ulong? ToOdd(this ulong? val) => (val.HasValue ? unchecked((ulong?) (val | (ulong) 1)) : null);

        /// <summary>
        /// Converts the value of an unsigned 64-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 64-bit unsigned integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>.</returns>
        /// 
        public static string ToStringEx(this ulong iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = 0, bool withprefix = true)
        {
            // Special treatment as DotNET 4.6 does not provide a conversion function for unsigned 64-bit integer but for base 10 only.

            ulong mask;
            int shift;
            switch (eToBase)
            {
            case ConvBase.Binary:
                mask = 1L;
                shift = 1;
                break;
            case ConvBase.Octal:
                mask = 7L;
                shift = 3;
                break;
            case ConvBase.Hexadecimal:
                mask = 15L;
                shift = 4;
                break;
            case ConvBase.Decimal:
            default:
                return Convert.ToString(iNumber).PadLeft((numdigits < 0 ? 0 : numdigits), '0');
            }

            var sb = new StringBuilder();
            unchecked
            {
                while (iNumber > 0)
                {
                    var c = (int) (iNumber & mask);
                    sb.Append(Convert.ToString(c, (int) eToBase));
                    iNumber >>= shift;
                }
            }

            char[] charArray;
            if (ConvertUpper)
                charArray = sb.ToString().ToUpperInvariant().ToCharArray();
            else
                charArray = sb.ToString().ToCharArray();
            Array.Reverse(charArray);
            return getPrefix(eToBase, withprefix) + new string(charArray).PadLeft((numdigits < 0 ? 0 : numdigits), '0');
        }

        /// <summary>
        /// Converts the value of an unsigned nullable 64-bit integer to its equivalent string representation in a specified base.
        /// </summary>
        /// <param name="iNumber">The 64-bit nullable unsigned integer to convert</param>
        /// <param name="eToBase">The base of the return value, which must be 2, 8, 10, or 16. Default is ConvBase.Decimal.</param>
        /// <param name="numdigits">Minimum number of digits. 0 for no minimum.</param>
        /// <param name="withprefix">Number is prefixed. Default prefix is present for binary, octal and hexadecimal.</param>
        /// <returns>The string representation of <paramref name="iNumber"/> in base <paramref name="eToBase"/>. Returns '0' if the integer is null.</returns>
        /// 
        public static string ToStringEx(this ulong? iNumber, ConvBase eToBase = ConvBase.Decimal, int numdigits = -1, bool withprefix = true)
            => (iNumber.HasValue ? iNumber.Value.ToStringEx(eToBase, numdigits, withprefix) : NullDispValue);

        /// <summary>
        /// Converts the value of an unsigned 64-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The unsigned 64-bit integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffixe (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this ulong iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
        {
            if (iNumber == 0)
                return "0" + getConvSuffix(eSuffix)[0];
            iDecimalPlace = Min(Max(0, iDecimalPlace), 15);
            var nPower = (int) Floor(Log(iNumber, 1024));
            var dDispQuantity = Round(iNumber / Pow(1024, nPower), iDecimalPlace);
            var bPlural = (eSuffix == ConvSuffix.Lng) && (dDispQuantity > 1.0);
            return (dDispQuantity).ToString("F" + iDecimalPlace, CultureInfo.InvariantCulture) + getConvSuffix(eSuffix)[nPower] + (bPlural ? "s" : "");
        }

        /// <summary>
        /// Converts the value of a nullable unsigned 64-bit integer to its equivalent string in multiple of 1024 bytes.
        /// </summary>
        /// <param name="iNumber">The nullable unsigned 64-bit integer to convert</param>
        /// <param name="iDecimalPlace">The number of decimal places for the formatted number.</param>
        /// <param name="eSuffix">Either <see cref="ConvSuffix.Sht"/> for short suffix (like KiB, MiB),
        ///                       or <see cref="ConvSuffix.Sht"/> for long suffix ('kibibytes', ...); default is short suffix.
        /// </param>
        /// <returns>The string representation of the number with the appropriate suffixe (KiB, MiB, GiB, etc...)</returns>
        /// 
        public static string ToPower1K(this ulong? iNumber, int iDecimalPlace = 0, ConvSuffix eSuffix = ConvSuffix.Sht)
            => (iNumber.HasValue ? ToPower1K(iNumber.Value, iDecimalPlace, eSuffix) : NullDispValue);

        #endregion

        #region Misc.

        /// <summary>
        /// Calculates the number of bytes corresponding to specified number of bits.
        /// (extension)
        /// </summary>
        /// <param name="nBitCount">Number of bits.</param>
        /// <returns>
        /// The number of bytes as an integer.
        /// </returns>
        public static int Bits2Bytes(this int nBitCount) => nBitCount / 8 + (nBitCount % 8 != 0 ? 1 : 0);

        #endregion

        #endregion

    }

}

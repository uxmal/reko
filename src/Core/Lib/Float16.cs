#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Lib
{
    /// <summary>
    /// Used to represent IEEE 754-2008 16-bit floating point numbers.
    /// </summary>
    public struct Float16 : IFormattable, IConvertible
    {
        private readonly ushort value;

        private const ushort SignBit = 0x8000;
        private const ushort ExponentMask = 0x1F;
        private const int ExponentBitOffset = 10;
        private const int ExponentBias = 15;

        public Float16(ushort binary16)
        {
            this.value = binary16;
        }

        public Float16(double d)
        {
            //$TODO: This is quick and dirty and doesn't handle
            // corner cases well. Feel free to improve it, but 
            // we're hoping the .NET Framework and .NET Core will

            ulong dBitPattern = (ulong) BitConverter.DoubleToInt64Bits(d);
            ulong hSign = ((dBitPattern >> 48) & SignBit);
            ulong dBiasedExponent = (dBitPattern >> 52) & 0x7FF;
            ulong dSignificand = dBitPattern & ((1L << 52) - 1);
            ulong hSignificand = dSignificand >> 42;
            int exp = (int)(dBiasedExponent - 1023) + ExponentBias;
            if (exp >= 31)
            {
                exp = 31;
                hSignificand = 0;
            }
            else if (exp <= -14)
            {
                exp = 0;
                hSignificand = 0;
            }
            value = (ushort)(hSign | ((uint)exp << ExponentBitOffset) | hSignificand);
        }

        public static Float16 operator - (Float16 a)
        {
            var negated = (ushort)(a.value ^ 0x8000);
            return new Float16(negated);
        }

        private bool IsZero()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var d = ToDouble(formatProvider);
            return d.ToString(format, formatProvider);
        }

        public double ToDouble(IFormatProvider formatProvider)
        {
            short sValue = (short)value;
            if (sValue == 0)
                return 0.0;
            if (value == SignBit)
                return -0.0;

            long dSign = ((long) sValue & SignBit) << 48;
            int hBiasedExp = (sValue >> ExponentBitOffset) & ExponentMask;
            int significand = sValue & ((1 << ExponentBitOffset) - 1);
            if (hBiasedExp == 0)
            {
                throw new NotImplementedException("Denormal");
            }
            if (hBiasedExp >= 31)
            {
                if (significand == 0)
                    return (dSign == 0) ? double.PositiveInfinity : double.NegativeInfinity;
                throw new NotImplementedException();
            }
            long dBiasedExp = (hBiasedExp - ExponentBias + 1023L) << 52;
            long dSignificand = (long)significand << 42;
            long dValue = dSign | dBiasedExp | dSignificand;
            return BitConverter.Int64BitsToDouble(dValue);
        }

        private InvalidCastException InvalidCast(Type type)
        {
            return new InvalidCastException(string.Format("Invalid cast from '{0}' to '{1}'.",
                type.Name, GetType().Name));
        }

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw InvalidCast(typeof(bool));
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw InvalidCast(typeof(char));
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw InvalidCast(typeof(sbyte));
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            if (IsZero())
                return 0;
            throw InvalidCast(typeof(byte));
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            //$TODO: rounding etc.
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw InvalidCast(typeof(DateTime));
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString("G", provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}

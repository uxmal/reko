#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System;

namespace Reko.Core.Lib
{
    /// <summary>
    /// Implements the 32-bit Microsoft Binary Format floating point number
    /// representation.
    /// </summary>
    public struct MBFFloat32 : IFormattable, IConvertible
    {
        private readonly uint value;

        private const uint SignBit = 0x80_0000;
        private const uint ExponentMask = 0xFF;
        private const int ExponentBitOffset = 24;
        private const int MantissaSize = ExponentBitOffset - 1;
        private const int ExponentBias = 128;

        /// <summary>
        /// Constructs an instance of <see cref="MBFFloat32"/> from a 32-bit word.
        /// </summary>
        /// <param name="value"></param>
        public MBFFloat32(uint value)
        {
            this.value = value;
        }

        /// <inheritdoc/>
        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ToBoolean(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public byte ToByte(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public char ToChar(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public DateTime ToDateTime(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public decimal ToDecimal(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public double ToDouble(IFormatProvider? provider)
        {
            long mant = value & 0x007FFFFF;
            var biasedExp = value >> ExponentBitOffset;
            var isNegative = (value & SignBit) != 0;
            if (biasedExp == 0x00)
                return isNegative ? -0.0F : 0.0F;
            var d = Constant.MakeReal((int)biasedExp, 0x81, mant, 23);
            if (isNegative)
                d = -d;
            return d;
        }

        /// <inheritdoc/>
        public short ToInt16(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int ToInt32(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long ToInt64(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public sbyte ToSByte(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public float ToSingle(IFormatProvider? provider)
        {
            return (float) ToDouble(provider);
        }

        /// <inheritdoc/>
        public string ToString(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object ToType(Type conversionType, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ushort ToUInt16(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public uint ToUInt32(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ulong ToUInt64(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
    }
}

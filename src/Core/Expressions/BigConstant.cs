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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Represents large constant values that will not fit in a Uint64.
    /// </summary>
    public class BigConstant : Constant
    {
        public BigConstant(DataType dt, BigInteger value) : base(dt)
        {
            this.Value = value;
        }

        public static BigConstant CreateUnsigned(DataType dt, BigInteger value)
        {
            var mask = (BigInteger.One << dt.BitSize) - 1;
            return new BigConstant(dt, value & mask);
        }

        public static new BigConstant Replicate(DataType dt, Constant valueToReplicate)
        {
            var n = valueToReplicate.ToBigInteger();
            int bits = valueToReplicate.DataType.BitSize;
            int times = dt.BitSize / bits;

            var result = BigInteger.Zero;
            for (int i = 0; i < times; ++i)
            {
                result = (result << bits) | n;
            }
            return new BigConstant(dt, result);
        }

        public override bool IsMaxUnsigned => false;    //$TODO: consider implementing this.

        public override bool IsIntegerZero => this.Value.IsZero;
        
        public override bool IsIntegerOne => this.Value.IsOne;

        public override bool IsZero => this.Value.IsZero;

        public BigInteger Value { get; }

        public override Expression CloneExpression()
        {
            return new BigConstant(this.DataType, Value);
        }

        public override Constant Complement()
        {
            var pow = (BigInteger.One << this.DataType.BitSize) - Value - BigInteger.One;
            return new BigConstant(this.DataType, pow);
        }

        protected override Constant DoSlice(DataType dt, int offset)
        {
            if (dt.BitSize <= 64)
            {
                var mask = ~0ul >> (64 - dt.BitSize);
                var n = (this.Value >> offset) & mask;
                return Constant.Create(dt, (ulong) n);
            }
            else
            {
                if (dt.Domain == Domain.SignedInt)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    var mask = (BigInteger.One << dt.BitSize) - BigInteger.One;
                    var n = (this.Value >> offset) & mask;
                    return new BigConstant(dt, n);
                }
            }
        }

        public override int GetHashOfValue()
        {
            return Value.GetHashCode();
        }

        public override object GetValue()
        {
            return Value;
        }

        public override Constant Negate()
        {
            return new BigConstant(this.DataType, -this.Value);
        }

        public override byte ToByte() => (byte) Value;

        public override short ToInt16() => (short) Value;

        public override int ToInt32() => (int) Value;

        public override long ToInt64() => (long) Value;

        public override ushort ToUInt16() => (ushort) Value;

        public override uint ToUInt32() => (uint) Value;

        public override ulong ToUInt64() => (ulong) Value;

        public override BigInteger ToBigInteger() => Value;
    }
}

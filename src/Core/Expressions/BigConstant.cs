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

using Reko.Core.Types;
using System;
using System.Numerics;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Represents large constant values that will not fit in a Uint64.
    /// </summary>
    public class BigConstant : Constant
    {
        /// <summary>
        /// Constructs a large constant.
        /// </summary>
        /// <param name="dt">Datatype of the value.</param>
        /// <param name="value">Large constant value.</param>
        public BigConstant(DataType dt, BigInteger value) : base(dt)
        {
            this.Value = value;
        }

        /// <summary>
        /// Constructs a large unsigned constant of a given size.
        /// </summary>
        /// <param name="dt">Datatype of the value.</param>
        /// <param name="value">Large constant value, to be converted to unsigned.</param>
        /// <returns>Resulting <see cref="BigConstant"/> instance.</returns>
        public static BigConstant CreateUnsigned(DataType dt, BigInteger value)
        {
            var mask = (BigInteger.One << dt.BitSize) - 1;
            return new BigConstant(dt, value & mask);
        }

        /// <summary>
        /// Replicates <paramref name="valueToReplicate"/> to fill the entire size of <paramref name="dt"/>.
        /// </summary>
        /// <param name="dt">Required datatype of the result.</param>
        /// <param name="valueToReplicate">Value to replicate.</param>
        /// <returns>Replicated value as a <see cref="BigConstant"/>.</returns>
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

        /// <inheritdoc/>
        public override bool IsMaxUnsigned => false;    //$TODO: consider implementing this.

        /// <inheritdoc/>
        public override bool IsIntegerZero => this.Value.IsZero;
        
        /// <inheritdoc/>
        public override bool IsIntegerOne => this.Value.IsOne;

        /// <inheritdoc/>
        public override bool IsZero => this.Value.IsZero;

        /// <inheritdoc/>
        public BigInteger Value { get; }

        /// <inheritdoc/>
        public override Expression CloneExpression()
        {
            return new BigConstant(this.DataType, Value);
        }

        /// <inheritdoc/>
        public override Constant Complement()
        {
            var pow = (BigInteger.One << this.DataType.BitSize) - Value - BigInteger.One;
            return new BigConstant(this.DataType, pow);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override int GetHashOfValue()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc/>
        public override object GetValue()
        {
            return Value;
        }

        /// <inheritdoc/>
        public override Constant Negate()
        {
            return new BigConstant(this.DataType, -this.Value);
        }

        /// <inheritdoc/>
        public override byte ToByte() => (byte) Value;

        /// <inheritdoc/>
        public override short ToInt16() => (short) Value;

        /// <inheritdoc/>
        public override int ToInt32() => (int) Value;

        /// <inheritdoc/>
        public override long ToInt64() => (long) Value;

        /// <inheritdoc/>
        public override ushort ToUInt16() => (ushort) Value;

        /// <inheritdoc/>
        public override uint ToUInt32() => (uint) Value;

        /// <inheritdoc/>
        public override ulong ToUInt64() => (ulong) Value;

        /// <inheritdoc/>
        public override BigInteger ToBigInteger() => Value;
    }
}

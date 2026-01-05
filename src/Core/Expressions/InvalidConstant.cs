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
using System.Collections.Generic;
using System.Numerics;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models a value that is known to not have a single constant value.
    /// </summary>
    public class InvalidConstant : Constant
    {
        private const uint pseudoValue = 0xBADDCAFE;
        private static readonly object cacheLock = new object();
        private static readonly Dictionary<DataType, InvalidConstant> cache =
            new Dictionary<DataType, InvalidConstant>();

        private InvalidConstant(DataType dt) : base(dt)
        {
        }

        /// <inheritdoc/>
        public override bool IsZero => false;

        /// <summary>
        /// Creates an <see cref="InvalidConstant"/> instance of the given size.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static InvalidConstant Create(DataType dt)
        {
            lock (cacheLock)
            {
                if (!cache.TryGetValue(dt, out var ic))
                {
                    ic = new InvalidConstant(dt);
                    cache.Add(dt, ic);
                }
                return ic;
            }
        }

        /// <inheritdoc/>
        public override bool IsMaxUnsigned => false;

        /// <inheritdoc/>
        public override bool IsValid => false;

        /// <inheritdoc/>
        public override Expression CloneExpression() => this;

        /// <inheritdoc/>
        public override Constant Complement() => this;

        /// <inheritdoc/>
        public override int GetHashOfValue()
        {
            return unchecked((int) pseudoValue) ^ DataType.GetHashCode();
        }

        /// <inheritdoc/>
        public override object GetValue() => throw new NotImplementedException();

        /// <inheritdoc/>
        public override Constant Slice(DataType dt, int offset)
        {
            return new InvalidConstant(dt);
        }

        /// <inheritdoc/>
        public override byte ToByte() => unchecked((byte) pseudoValue);

        /// <inheritdoc/>
        public override short ToInt16() => unchecked((short)pseudoValue);

        /// <inheritdoc/>
        public override int ToInt32() => unchecked((int) pseudoValue);

        /// <inheritdoc/>
        public override long ToInt64() => pseudoValue;

        /// <inheritdoc/>
        public override ushort ToUInt16() => unchecked((ushort) pseudoValue);

        /// <inheritdoc/>
        public override uint ToUInt32() => pseudoValue;

        /// <inheritdoc/>
        public override ulong ToUInt64() => pseudoValue;

        /// <inheritdoc/>
        public override BigInteger ToBigInteger() => pseudoValue;
    }
}

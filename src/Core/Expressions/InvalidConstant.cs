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

        public override bool IsZero => false;

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

        public override bool IsMaxUnsigned => false;

        public override bool IsValid => false;

        public override Expression CloneExpression() => this;

        public override Constant Complement() => this;

        public override int GetHashOfValue()
        {
            return unchecked((int) pseudoValue) ^ DataType.GetHashCode();
        }

        public override object GetValue() => throw new NotImplementedException();

        public override Constant Slice(DataType dt, int offset)
        {
            return new InvalidConstant(dt);
        }

        public override byte ToByte() => unchecked((byte) pseudoValue);

        public override short ToInt16() => unchecked((short)pseudoValue);

        public override int ToInt32() => unchecked((int) pseudoValue);

        public override long ToInt64() => pseudoValue;

        public override ushort ToUInt16() => unchecked((ushort) pseudoValue);

        public override uint ToUInt32() => pseudoValue;

        public override ulong ToUInt64() => pseudoValue;

        public override BigInteger ToBigInteger() => pseudoValue;
    }
}

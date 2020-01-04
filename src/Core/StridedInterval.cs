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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
    /// <summary>
    /// Represents a strided interval. Strided intervals are sets 
    /// of integers values starting at `low` and ending in `high` (inclusive),
    /// spaced by the `stride`.
    /// </summary>
    /// <remarks>
    /// A stride of 0 implies a constant value or singleton.
    /// A negative stride is interpreted as the empty set.
    /// </remarks>
    public struct StridedInterval
    {
        public readonly long Low;
        public readonly long High;
        public readonly int Stride;

        // The empty set.
        public readonly static StridedInterval Empty = new StridedInterval(-1, 0, 0);
        // The universal set.
        public readonly static StridedInterval All = new StridedInterval(1, long.MinValue, long.MaxValue);

        /// <summary>
        /// Creates a strided interval containing a single value <paramref name="c"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static StridedInterval Constant(Constant c)
        {
            long v = c.ToInt64();
            return new StridedInterval(0, v, v);
        }

        public static StridedInterval Create(int stride, long low, long high)
        {
            if (stride < 0)
                throw new ArgumentOutOfRangeException("stride", "Negative strides are not allowed.");
            if (low > high)
                throw new ArgumentException("Parameter 'low' mustn't be larger than 'high'.");
            return new StridedInterval(stride, low, high);
        }

        private StridedInterval(int stride, long low, long high)
        {
            this.Low = low;
            this.High = high;
            this.Stride = stride;
        }

        public bool IsEmpty
        {
            get { return Stride < 0; }
        }

        public override string ToString()
        {
            if (Stride < 0)
                return "\x27D8";    // U+27D8 LARGE UP TACK
            var low = Low < 0 ? $"-{-Low:X}" : Low.ToString("X");
            var high = High < 0 ? $"-{-High:X}" : High.ToString("X");
            return $"{Stride:X}[{low},{high}]";
        }
    }
}

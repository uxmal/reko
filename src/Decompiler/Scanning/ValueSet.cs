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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    /// <summary>
    /// Abstracts away a list of values into a single object.
    /// </summary>
    public abstract class ValueSet
    {
        public ValueSet(DataType dt)
        {
            this.DataType = dt;
        }

        public static readonly ValueSet Any = new ConcreteValueSet(new UnknownType());
        public DataType DataType { get; }

        /// <summary>
        /// Extracts the list of values, possibly concretizing them on the fly.
        /// </summary>
        public abstract IEnumerable<Expression> Values { get; }
        public abstract bool IsEmpty { get; }

        public abstract ValueSet Add(ValueSet right);
        public abstract ValueSet Add(Constant right);
        public abstract ValueSet And(Constant cRight);
        public abstract ValueSet IMul(Constant cRight);
        public abstract ValueSet Sub(Constant cRight);
        public abstract ValueSet Shl(Constant cRight);
        public abstract ValueSet SignExtend(DataType dataType);
        public abstract ValueSet Truncate(DataType dt);
        public abstract ValueSet ZeroExtend(DataType dataType);

    }

    /// <summary>
    /// Represents a value set using a strided interval.
    /// </summary>
    public class IntervalValueSet : ValueSet
    {
        public StridedInterval SI;

        public IntervalValueSet(DataType dt, StridedInterval si) : base(dt)
        {
            this.SI = si;
        }

        /// <summary>
        /// Concretizes the values represented by the strided interval.
        /// </summary>
        public override IEnumerable<Expression> Values
        {
            get
            {
                if (SI.Stride < 0)
                    yield break;
                else if (SI.Stride == 0)
                    yield return Constant.Create(DataType, SI.Low);
                else
                {
                    long v = SI.Low; 
                    while (v <= SI.High)
                    {
                        yield return Constant.Create(DataType, v);
                        if (v == SI.High)   // avoid overflow.
                            yield break;
                        v += SI.Stride;
                    }
                }
            }
        }

        public override bool IsEmpty
        {
            get { return SI.Stride < 0; }
        }

        public override ValueSet Add(ValueSet right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new value set where all values are 
        /// augmented by <paramref name="right"/>.
        /// </summary>
        /// <param name="right"></param>
        /// <returns>A new value set.</returns>
        public override ValueSet Add(Constant right)
        {
            if (SI.Stride < 0)
            {
                return new IntervalValueSet(
                    this.DataType,
                    StridedInterval.Empty);
            }
            long v = right.ToInt64();
            return new IntervalValueSet(
                this.DataType,
                StridedInterval.Create(
                    SI.Stride,
                    SI.Low + v,
                    SI.High + v));
        }

        public override ValueSet And(Constant right)
        {
            long v = right.ToInt64();
            return new IntervalValueSet(
                this.DataType,
                StridedInterval.Create(1, 0, v));
        }

        public override ValueSet IMul(Constant cRight)
        {
            if (SI.IsEmpty)
            {
                return new IntervalValueSet(
                    this.DataType,
                    StridedInterval.Empty);
            }
            long v = cRight.ToInt64();
            return new IntervalValueSet(
                this.DataType,
                StridedInterval.Create(
                    SI.Stride * (int)v,
                    SI.Low * v,
                    SI.High * v));
        }

        public override ValueSet Shl(Constant cRight)
        {
            int v = (int) cRight.ToInt64();
            return new IntervalValueSet(
                this.DataType,
                StridedInterval.Create(
                    SI.Stride << v,
                    SI.Low << v,
                    SI.High << v));
        }

        public override ValueSet SignExtend(DataType dataType)
        {
            return new IntervalValueSet(dataType, this.SI);
        }

        public override ValueSet Sub(Constant right)
        {
            if (SI.Stride < 0)
            {
                return new IntervalValueSet(
                    this.DataType,
                    StridedInterval.Empty);
            }
            long v = right.ToInt64();
            return new IntervalValueSet(
                this.DataType,
                StridedInterval.Create(
                    SI.Stride,
                    SI.Low - v,
                    SI.High - v));
        }

        /// <summary>
        /// Returns a valueset whose values are the truncation of this valueset's
        /// values.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override ValueSet Truncate(DataType dt)
        {
            if (SI.Stride < 0)
                return this;

            var mask = (1 << dt.BitSize) - 1;
            StridedInterval siNew;
            if (SI.Low == SI.High)
            {
                siNew = StridedInterval.Constant(
                    Constant.Create(dt, SI.Low & mask));
            }
            else
            {
                siNew = StridedInterval.Create(
                    SI.Stride, 0, Math.Min(mask, SI.High));
            }
            return new IntervalValueSet(dt, siNew);
        }

        public override ValueSet ZeroExtend(DataType dataType)
        {
            return new IntervalValueSet(dataType, this.SI);
        }

        public override string ToString()
        {
            return SI.ToString();
        }
    }

    /// <summary>
    /// A Concrete value set consists of a literal sequence of values.
    /// </summary>
    public class ConcreteValueSet : ValueSet
    {
        private Expression[] values;

        public ConcreteValueSet(DataType dt, params Expression[] values) : base(dt)
        {
            this.values = values;
        }

        public override IEnumerable<Expression> Values
        {
            get { return values; }
        }

        public override bool IsEmpty
        {
            get { return values.Length == 0; }
        }

        /// <summary>
        /// Perform the computation <paramref name="map"/> on each value
        /// in the value set, and wrap them in a new value set.
        /// </summary>
        private ConcreteValueSet Map(DataType dt, Func<Expression,Expression> map)
        {
            return new ConcreteValueSet(
                dt,
                values.Select(map).ToArray());
        }

        public override ValueSet Add(Constant cRight)
        {
            return Map(DataType, v => AddValue(v, cRight));
        }

        public override ValueSet Add(ValueSet right)
        {
            throw new NotImplementedException();
        }

        public override ValueSet And(Constant right)
        {
            throw new NotImplementedException();
        }

        public override ValueSet IMul(Constant cRight)
        {
            return Map(DataType, v => MulValue(v, cRight));
        }

        private Expression AddValue(Expression eLeft, Constant cRight)
        {
            if (eLeft is Constant cLeft)
                return Operator.IAdd.ApplyConstants(cLeft, cRight);
            throw new NotImplementedException();
        }

        private Expression MulValue(Expression eLeft, Constant cRight)
        {
            if (eLeft is Constant cLeft)
                return Operator.IMul.ApplyConstants(cLeft, cRight);
            throw new NotImplementedException();
        }

        public override ValueSet Shl(Constant cRight)
        {
            return Map(DataType, v => ShlValue(v, cRight));
        }

        private Expression ShlValue(Expression eLeft, Constant cRight)
        {
            if (eLeft is Constant cLeft)
                return Operator.Shl.ApplyConstants(cLeft, cRight);
            throw new NotImplementedException();
        }

        public override ValueSet SignExtend(DataType dt)
        {
            return Map(dt, v => SignExtendValue(dt, v));
        }

        private Expression SignExtendValue(DataType dt, Expression arg)
        {
            if (arg is Constant v)
            {
                int bits = this.DataType.BitSize;
                return Constant.Create(dt, Bits.SignExtend(v.ToUInt64(), bits));
            }
            throw new NotImplementedException();
        }

        public override ValueSet Sub(Constant cRight)
        {
            return Map(DataType, v => SubValue(v, cRight));
        }

        private Expression SubValue(Expression eLeft, Constant cRight)
        {
            if (eLeft is Constant cLeft)
            {
                if (cLeft.IsValid)
                    return Operator.ISub.ApplyConstants(cLeft, cRight);
                else
                    return cLeft;
            }
            throw new NotImplementedException();
        }

        public override ValueSet Truncate(DataType dt)
        {
            return Map(dt, v => TruncateValue(dt, v));
        }

        private Expression TruncateValue(DataType dt, Expression value)
        {
            if (value is Constant c)
            {
                var mask = (1L << dt.BitSize) - 1;
                return Constant.Create(dt, c.ToInt64() & mask);
            }
            throw new NotImplementedException();
        }

        public override ValueSet ZeroExtend(DataType dt)
        {
            return Map(dt, v => ZeroExtendValue(dt, v));
        }

        private Expression ZeroExtendValue(DataType dt, Expression arg)
        {
            if (arg is Constant v)
            {
                int bits = this.DataType.BitSize;
                return Constant.Create(dt, Bits.ZeroExtend(v.ToUInt64(), bits));
            }
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"[{string.Join(",", values.AsEnumerable())}]";
        }
    }
}

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
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// Abstracts away a list of values into a single object.
    /// </summary>
    public abstract class ValueSet
    {
        /// <summary>
        /// Constructs a value set.
        /// </summary>
        /// <param name="dt"><see cref="DataType"/> of the members of the value set.
        /// </param>
        public ValueSet(DataType dt)
        {
            this.DataType = dt;
        }

        /// <summary>
        /// Symbolizes any value.
        /// </summary>
        public static readonly ValueSet Any = new ConcreteValueSet(new UnknownType());
        
        /// <summary>
        /// Data type of the elements in the value set.
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// Extracts the list of values, possibly concretizing them on the fly.
        /// </summary>
        public abstract IEnumerable<Expression> Values { get; }

        /// <summary>
        /// True if the <see cref="ValueSet"/> is empty.
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Adds the specified <see cref="ValueSet"/> to the current instance and returns the result.
        /// </summary>
        /// <param name="right">The <see cref="ValueSet"/> to add to the current instance.</param>
        /// <returns>A new <see cref="ValueSet"/> representing the sum of the current instance and the specified <paramref
        /// name="right"/>.</returns>
        public abstract ValueSet Add(ValueSet right);

        /// <summary>
        /// Adds the constant <paramref name="right"/> to the current instance and returns the resulting
        /// <see cref="ValueSet"/>.
        /// </summary>
        /// <param name="right">The <see cref="ValueSet"/> to add to the current instance.</param>
        /// <returns>A new <see cref="ValueSet"/> representing the sum of the current instance and the specified <paramref
        /// name="right"/>.</returns>
        public abstract ValueSet Add(Constant right);

        /// <summary>
        /// Computes the logical-and of the constant <paramref name="cRight"/> and the current instance,
        /// and returns the resulting
        /// <see cref="ValueSet"/>.
        /// </summary>
        /// <param name="cRight">The <see cref="ValueSet"/> to add to the current
        /// instance.</param>
        /// <returns>A new <see cref="ValueSet"/> representing the sum of the current
        /// instance and the specified <paramref name="cRight"/>.</returns>
        public abstract ValueSet And(Constant cRight);

        /// <summary>
        /// Computes the integer product and of the constant <paramref name="cRight"/> and the current instance,
        /// and returns the resulting
        /// <see cref="ValueSet"/>.
        /// </summary>
        /// <param name="cRight">The <see cref="ValueSet"/> to add to the current
        /// instance.</param>
        /// <returns>A new <see cref="ValueSet"/> representing the logical and of the current
        /// instance and the specified <paramref name="cRight"/>.</returns>
        public abstract ValueSet IMul(Constant cRight);

        /// <summary>
        /// Subtracts the constant <paramref name="cRight"/> from the current instance,
        /// and returns the resulting
        /// <see cref="ValueSet"/>.
        /// </summary>
        /// <param name="cRight">The <see cref="ValueSet"/> to add to the current
        /// instance.</param>
        /// <returns>A new <see cref="ValueSet"/> representing the difference of the current
        /// instance and the specified <paramref name="cRight"/>.</returns>
        public abstract ValueSet Sub(Constant cRight);

        /// <summary>
        /// Shifts the current instance to the left by the constant <paramref name="cRight"/> 
        /// and returns the resulting <see cref="ValueSet"/>.
        /// </summary>
        /// <param name="cRight">The <see cref="ValueSet"/> to add to the current
        /// instance.</param>
        /// <returns>A new <see cref="ValueSet"/> representing the current
        /// instance shifted left by the specified <paramref name="cRight"/>.
        /// </returns>
        public abstract ValueSet Shl(Constant cRight);

        /// <summary>
        /// Extends the sign of a value to match the specified data type.
        /// </summary>
        /// <param name="dataType">The target data type to which the sign extension should be applied.</param>
        /// <returns>A <see cref="ValueSet"/> whose type is <paramref name="dataType"/>.</returns>
        public abstract ValueSet SignExtend(DataType dataType);

        /// <summary>
        /// Truncates the values in the value set to fit within the specified data type.
        /// </summary>
        /// <param name="dt">(Smaller) data type to truncate the values to.</param>
        /// <returns>A new <see cref="ValueSet"/> where all values are truncated.
        /// </returns>
        public abstract ValueSet Truncate(DataType dt);

        /// <summary>
        /// Zero extends the values to match the specified data type.
        /// </summary>
        /// <param name="dataType">The target data type to which the sign extension should be applied.</param>
        /// <returns>A <see cref="ValueSet"/> whose type is <paramref name="dataType"/>.</returns>
        public abstract ValueSet ZeroExtend(DataType dataType);

    }

    /// <summary>
    /// Represents a value set using a strided interval.
    /// </summary>
    public class IntervalValueSet : ValueSet
    {
        /// <summary>
        /// Strided interval representation.
        /// </summary>
        public StridedInterval SI { get; }

        /// <summary>
        /// Constructs a value set from a strided interval.
        /// </summary>
        /// <param name="dt">Datatype of the value set.</param>
        /// <param name="si">Strided interval.</param>
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

        /// <inheritdoc/>
        public override bool IsEmpty
        {
            get { return SI.Stride < 0; }
        }

        /// <inheritdoc/>
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
            var dt = this.DataType.BitSize >= right.DataType.BitSize
                ? this.DataType
                : right.DataType;
            return new IntervalValueSet(
                dt,
                StridedInterval.Create(
                    SI.Stride,
                    SI.Low + v,
                    SI.High + v));
        }

        /// <inheritdoc/>
        public override ValueSet And(Constant right)
        {
            long v = right.ToInt64();
            return new IntervalValueSet(
                this.DataType,
                StridedInterval.Create(1, 0, v));
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ValueSet Shl(Constant cRight)
        {
            if (SI.IsEmpty)
            {
                return new IntervalValueSet(
                    this.DataType,
                    StridedInterval.Empty);
            }
            int v = (int) cRight.ToInt64();
            return new IntervalValueSet(
                this.DataType,
                StridedInterval.Create(
                    SI.Stride << v,
                    SI.Low << v,
                    SI.High << v));
        }

        /// <inheritdoc/>
        public override ValueSet SignExtend(DataType dataType)
        {
            return new IntervalValueSet(dataType, this.SI);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ValueSet ZeroExtend(DataType dataType)
        {
            return new IntervalValueSet(dataType, this.SI);
        }

        /// <inheritdoc/>
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
        private readonly Expression[] values;

        /// <summary>
        /// Constructs a concrete value set from a sequence of values.
        /// </summary>
        /// <param name="dt">Data type of the values.</param>
        /// <param name="values">Values of the value set.</param>
        public ConcreteValueSet(DataType dt, params Expression[] values) : base(dt)
        {
            this.values = values;
        }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Values
        {
            get { return values; }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ValueSet Add(Constant cRight)
        {
            return Map(DataType, v => AddValue(v, cRight));
        }

        /// <inheritdoc/>
        public override ValueSet Add(ValueSet right)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ValueSet And(Constant right)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ValueSet IMul(Constant cRight)
        {
            return Map(DataType, v => MulValue(v, cRight));
        }

        private static Expression AddValue(Expression eLeft, Constant cRight)
        {
            if (eLeft is Constant cLeft)
                return Operator.IAdd.ApplyConstants(cLeft.DataType, cLeft, cRight);
            throw new NotImplementedException();
        }

        private static Expression MulValue(Expression eLeft, Constant cRight)
        {
            if (eLeft is Constant cLeft)
                return Operator.IMul.ApplyConstants(cLeft.DataType, cLeft, cRight);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ValueSet Shl(Constant cRight)
        {
            return Map(DataType, v => ShlValue(v, cRight));
        }

        private static Expression ShlValue(Expression eLeft, Constant cRight)
        {
            if (eLeft is Constant cLeft)
                return Operator.Shl.ApplyConstants(cLeft.DataType, cLeft, cRight);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override ValueSet Sub(Constant cRight)
        {
            return Map(DataType, v => SubValue(v, cRight));
        }

        private static Expression SubValue(Expression eLeft, Constant cRight)
        {
            if (eLeft is Constant cLeft)
            {
                if (cLeft.IsValid)
                    return Operator.ISub.ApplyConstants(cLeft.DataType, cLeft, cRight);
                else
                    return cLeft;
            }
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ValueSet Truncate(DataType dt)
        {
            return Map(dt, v => TruncateValue(dt, v));
        }

        private static Expression TruncateValue(DataType dt, Expression value)
        {
            if (value is Constant c)
            {
                var mask = (1L << dt.BitSize) - 1;
                return Constant.Create(dt, c.ToInt64() & mask);
            }
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{string.Join(",", values.AsEnumerable())}]";
        }
    }
}

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

using Reko.Core.Expressions;
using Reko.Core.Operators;
using System;
using System.Text;

namespace Reko.Core.Analysis
{
    /// <summary>
    /// Describes the initial value, stride, and final value of an induction
    /// variable.
    /// </summary>
    public class LinearInductionVariable
    {
        /// <summary>
        /// Creates an induction variable. The initial and final values define
        /// a half-open interval, i.e. the final value is not included in the 
        /// range of values the induction variable can take.
        /// </summary>
        /// <param name="initial">The initial value the variable takes.</param>
        /// <param name="delta">The amount incremented or decremented per interation.</param>
        /// <param name="final">The final value, not attained by loop since it terminated.</param>
        /// <param name="isSigned">True if the value should be treated as signed.</param>
        public LinearInductionVariable(
            Constant? initial,
            Constant? delta,
            Constant? final,
            bool isSigned)
        {
            Initial = initial;
            Delta = delta;
            Final = final;
            IsSigned = isSigned;
        }

        /// <summary>
        /// The initial value of the induction variable.
        /// </summary>
        public Constant? Initial { get; private set; }

        /// <summary>
        /// The amount incremented or decremented per interation.
        /// </summary>
        public Constant? Delta { get; private set; }

        /// <summary>
        /// The final value, not attained by loop since it terminated.
        /// </summary>
        public Constant? Final { get; private set; }

        /// <summary>
        /// True if signed compares are used for the induction variable.
        /// </summary>
        public bool IsSigned { get; private set; }    

        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                int t = a % b;
                a = b;
                b = t;
            }
            return a;
        }

        /// <summary>
        /// Returns the number of times the loop iterates, or zero if this can't be determined.
        /// </summary>
        public int IterationCount
        {
            get
            {
                if (Initial is null || Final is null || Delta is null)
                    return 0;
                return Math.Abs((Initial.ToInt32() - Final.ToInt32()) / Delta.ToInt32());
            }
        }

        /// <summary>
        /// Merges two induction variables. 
        /// </summary>
        /// <param name="liv1">An <see cref="LinearInductionVariable"/>.</param>
        /// <param name="liv2">Another <see cref="LinearInductionVariable"/>.</param>
        /// <returns>Returns a new induction variable whose stride is the 
        /// greatest common divisor (GCD) of the strides of <paramref name="liv1"/> and
        /// <paramref name="liv2"/> or null if the strides are not compatible.
        /// </returns>
        public static LinearInductionVariable? Merge(LinearInductionVariable liv1, LinearInductionVariable liv2)
        {
            if (liv1.Delta is null || liv2.Delta is null)
                return null;
            int delta1 = Convert.ToInt32(liv1.Delta.ToInt32());
            int delta2 = Convert.ToInt32(liv2.Delta.ToInt32());
            if (delta1 == 1)
                return new LinearInductionVariable(null, liv1.Delta, null, liv1.IsSigned);
            else if (delta2 == 1)
                return new LinearInductionVariable(null, liv2.Delta, null, liv2.IsSigned);
            else
            {
                int delta = Gcd(delta1, delta2);
                if (delta == 1)
                    return null;
                else
                    return new LinearInductionVariable(null, Constant.Create(liv1.Delta.DataType, delta),
                        null, liv1.IsSigned || liv2.IsSigned);
            }
        }

        /// <summary>
        /// Offset the induction variable by a constant.
        /// </summary>
        /// <param name="c">Offset to add to the induction variable.</param>
        public void AddIncrement(Constant c)
        {
            if (Initial is not null)
                Initial = Operator.IAdd.ApplyConstants(Initial.DataType, Initial, c);
            if (Final is not null)
                Final = Operator.IAdd.ApplyConstants(Final.DataType, Final, c);
        }

        /// <summary>
        /// Scale the induction variable by a constant.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>A new, scaled induction variable.</returns>
        public LinearInductionVariable Scale(Constant c)
        {
            Constant? initial = Initial;
            Constant delta = Delta!;
            Constant? final = Final;

            if (initial is not null)
                initial = Operator.SMul.ApplyConstants(initial.DataType, initial, c);
            delta = Operator.SMul.ApplyConstants(delta.DataType, delta, c);
            if (final is not null)
                final = Operator.SMul.ApplyConstants(final.DataType, final, c);
            return new LinearInductionVariable(initial, delta, final, IsSigned);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('(');
            if (Initial is not null)
            {
                sb.Append(Initial.ToString());
            }
            else
            {
                sb.Append('?');
            }
            sb.Append(' ');
            sb.Append(Delta!.ToString());
            sb.Append(' ');
            if (Final is not null)
            {
                sb.Append(Final.ToString());
            }
            else
            {
                sb.Append('?');
            }
            if (IsSigned)
            {
                sb.Append(" signed");
            }
            sb.Append(')');
            return sb.ToString();
        }
    }
}

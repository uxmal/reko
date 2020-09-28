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
using Reko.Core.Operators;
using System;
using System.Text;

namespace Reko.Core
{
	/// <summary>
	/// Describes the initial value, stride, and final value of an induction
    /// variable.
	/// </summary>
	public class LinearInductionVariable
	{
		public LinearInductionVariable(
            Constant initial, 
            Constant delta, 
            Constant final,
            bool isSigned)
		{
			this.Initial = initial;
			this.Delta = delta;
			this.Final = final;
            this.IsSigned = isSigned;
		}

        public Constant Initial { get; private set; }   // First value used by induction variable 
        public Constant Delta { get; private set; }		// Amount incremented or decremented per interation
        public Constant Final { get; private set; }     // Value not attained by loop since it terminated.
        public bool IsSigned { get; private set; }      // True if signed compares are used for the induction variable.

		public static int Gcd(int a, int b)
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
				if (Initial == null || Final == null)
					return 0;
				return Math.Abs((Initial.ToInt32() - Final.ToInt32()) / Delta.ToInt32());
			}
		}

		public static LinearInductionVariable Merge(LinearInductionVariable liv1, LinearInductionVariable liv2)
		{
			if (liv1.Delta == null || liv2.Delta == null)
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

        public void AddIncrement(Constant c)
        {
            if (Initial != null)
                Initial = Operator.IAdd.ApplyConstants(Initial, c);
            if (Final != null)
                Final = Operator.IAdd.ApplyConstants(Final, c);
        }

		public LinearInductionVariable Scale(Constant c)
		{
			Constant initial = Initial;
			Constant delta = Delta;
			Constant final = Final;

			if (initial != null)
				initial = Operator.SMul.ApplyConstants(initial, c);
			delta = Operator.SMul.ApplyConstants(delta, c);
			if (final != null)
				final = Operator.SMul.ApplyConstants(final, c);
			return new LinearInductionVariable(initial, delta, final, IsSigned);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("(");
			if (Initial != null)
			{
				sb.Append(Initial.ToString());
			}
			else
			{
				sb.Append('?');
			}
			sb.Append(' ');
			sb.Append(Delta.ToString());
			sb.Append(' ');
			if (Final != null)
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

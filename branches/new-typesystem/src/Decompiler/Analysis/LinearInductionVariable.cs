/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using System;
using System.Text;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Describes the initial value, stride, and final value of an induction variable.
	/// </summary>
	public class LinearInductionVariable
	{
		public readonly Constant Initial;		// First value used by induction variable 
		public readonly Constant Delta;			// Amount incremented or decremented per interation
		public readonly Constant Final;			// Value not attained by loop since it terminated.

		public LinearInductionVariable(Constant initial, Constant delta, Constant final)
		{
			this.Initial = initial;
			this.Delta = delta;
			this.Final = final;
		}

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
				return Math.Abs((Convert.ToInt32(Initial.Value) - Convert.ToInt32(Final.Value)) / Convert.ToInt32(Delta.Value));
			}
		}

		public static LinearInductionVariable Merge(LinearInductionVariable liv1, LinearInductionVariable liv2)
		{
			if (liv1.Delta == null || liv2.Delta == null)
				return null;
			int delta1 = Convert.ToInt32(liv1.Delta.AsInt32());
			int delta2 = Convert.ToInt32(liv2.Delta.AsInt32());
			if (delta1 == 1)
				return new LinearInductionVariable(null, liv1.Delta, null);
			else if (delta2 == 1)
				return new LinearInductionVariable(null, liv2.Delta, null);
			else
			{
				int delta = Gcd(delta1, delta2);
				if (delta == 1)
					return null;
				else
					return new LinearInductionVariable(null, new Constant(liv1.Delta.DataType, delta), null);
			}
		}

		public LinearInductionVariable Scale(Constant c)
		{
			Constant initial = Initial;
			Constant delta = Delta;
			Constant final = Final;

			if (initial != null)
				initial = Operator.muls.ApplyConstants(initial, c);
			delta = Operator.muls.ApplyConstants(delta, c);
			if (final != null)
				final = Operator.muls.ApplyConstants(final, c);
			return new LinearInductionVariable(initial, delta, final);
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
			sb.Append(')');
			return sb.ToString();
		}
	}
}

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

using System;

namespace Reko.Scanning
{
	/// <summary>
	/// </summary>
	public class Regexp
	{
		private int [] mpbytecls;
		private State [] states;
		private int [] next;
		private int [] check;

		private int pos;		// where the scanner is right now.

		internal Regexp(int [] m, State [] s, int [] n, int [] c)
		{
			mpbytecls = m;
			states = s;
			next = n;
			check = c;
		}

		/// <summary>
		/// Compiles a scanner corresponding to the regular expression.
		/// </summary>
		/// <param name="s"></param>
		/// <returns>A scanner ready to use</returns>
		public static Regexp Compile(string s)
		{
			RegexpBuilder b = new RegexpBuilder(s);
			return b.Build();
		}

		public int Position
		{
			get { return pos; }
		}

		public bool Match(byte [] arr, int position)
		{
            int p = position;
			pos = p;
			int s = 1;			// state 0 is the error state.
			int posAcc = states[1].Accepts ? 0 : -1;
			while (p < arr.Length)
			{
				if (states[s].Accepts)
					posAcc = p;
				int idx = states[s].BasePosition + mpbytecls[arr[p++]];
				if (check[idx] != s)
				{
					if (posAcc != -1)
					{
						pos = posAcc;
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					s = next[idx];
				}
			}
			pos = p;
			return states[s].Accepts || posAcc != -1;
		}

		public override string ToString()
		{
			var sb = new System.Text.StringBuilder();
			sb.Append("{ alphabet:");
			for (int i = 0; i != this.mpbytecls.Length; ++i)
			{
				if (mpbytecls[i] != 0)
					sb.AppendFormat(" {0:X2}:{1}", i, mpbytecls[i]);
			}
			sb.AppendLine("\r\nstates:");
			for (int s = 0; s != states.Length; ++s)
			{
				sb.AppendFormat("\ts{0}{1}:", s, states[s].Accepts?" (acc)":"");
				for (int n = states[s].BasePosition; n != next.Length; ++n)
				{
					if (check[n] == s)
					{
						sb.AppendFormat(" {0}->s{1}", n - states[s].BasePosition, next[n]);
					}
				}
				sb.AppendLine();
			}
			sb.Append("}");
			return sb.ToString();
		}

		internal class State
		{
			public int BasePosition;
			public bool Accepts;
		}
	}
}
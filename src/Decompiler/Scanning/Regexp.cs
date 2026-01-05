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

#nullable disable

using System;

namespace Reko.Scanning
{
    /// <summary>
    /// Represents a compiled regular expression scanner.
    /// </summary>
    public class Regexp
	{
		private readonly int [] mpbytecls;
		private readonly State [] states;
		private readonly int [] next;
		private readonly int [] check;

		private int pos;		// where the scanner is right now.

        /// <summary>
        /// Constructs a new Regexp instance.
        /// </summary>
        /// <param name="m">Maps bytes to byte classes.</param>
        /// <param name="s">The states of the regexp.</param>
        /// <param name="n">The transitions of the regexp.</param>
        /// <param name="c">Check values (for overlapping states).</param>
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
			var b = new RegexpBuilder(s);
			return b.Build();
		}

        /// <summary>
        /// Determines whether the regexp matches the byte array <paramref name="arr"/>
        /// at position <paramref name="position"/>.
        /// </summary>
        /// <param name="arr">Array to match.</param>
        /// <param name="position">Position at which to match.</param>
        /// <returns>True if there is a match at the given position; false if not.
        /// </returns>
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

        /// <inheritdoc />
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
			sb.Append('}');
			return sb.ToString();
		}

		internal class State
		{
			public int BasePosition;
			public bool Accepts;
		}
	}
}
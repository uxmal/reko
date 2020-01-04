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
using System.Collections.Generic;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// Knuth-Morris-Pratt string search algorithm.
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public class KmpStringSearch<C> : StringSearch<C> where C : IComparable<C>
    {
        private int[] failureTable;
        private C[] keyword;

        public KmpStringSearch(C[] W, bool scannedMemory, bool unscannedMemory)
            : base(W,  scannedMemory,  unscannedMemory)
        {
            this.keyword = W;
            failureTable = BuildFailureTable(W);
        }

        public override IEnumerable<int> GetMatchPositions(C [] text)
        {
            int m = 0; // the beginning of the current match in S
            int i = 0; // the position of the current character in W

            while (m + i < text.Length)
            {
                if (keyword[i].CompareTo(text[m + i]) == 0)
                {
                    ++i;
                    if (i == keyword.Length)
                        yield return m;
                }
                else
                {
                    m = m + i - failureTable[i];
                    if (failureTable[i] > -1)
                        i = failureTable[i];
                }
            }
        }

        private int[] BuildFailureTable(C[] W)
        {
            int[] T = new int[W.Length+2];
            T[0] = -1;
            T[1] = 0;
            int pos = 2;
            int cnd = 0;  // the zero-based index in W of the next character of the current candidate substring

            while (pos < W.Length)
            {
                if (W[pos - 1].CompareTo(W[cnd]) == 0)
                {
                    T[pos] = cnd + 1;
                    ++pos;
                    ++cnd;
                }
                else if (cnd > 0)
                {
                    cnd = T[cnd];
                }
                else
                {
                    T[pos] = 0;
                    ++pos;
                }
            }
            return T;
        }
    }
}

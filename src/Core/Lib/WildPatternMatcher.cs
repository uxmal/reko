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
using System.Linq;
using System.Text;

namespace Reko.Core.Lib
{
    public class WildPatternMatcher
    {

        /*
         * Pattern Matching Algorithms with Don’t Cares
        Costas S. Iliopoulos and M. Sohel Rahman
        Algorithm Design Group
        Department of Computer Science, King’s College London
        */


        /// </summary>
        /// Given a text T over the alphabet Σ and a pattern P
        /// over the alphabet Σ∪{∗}, find all the occurrences of
        /// P in T.
        /// <param name="T"></param>
        /// <param name="P"></param>
        public IEnumerable<int> FindDcp(string T, string P)
        {
            var sa = new SuffixArray<char>(T.ToCharArray());
            int[] permitted = new int[T.Length];
            Tuple<string[], int[]> pat = splitPattern(P);
            var l = pat.Item1.Length;
            var Val = new int[l];
            Val[0] = 0;
            for (int i = 1; i < l; ++i)
            {
                Val[i] = Val[i - 1] + pat.Item1[i - 1].Length + pat.Item2[i - 1];
            }
            var occ_P = new SortedSet<int>();
            for (int i = 0; i < l; ++i)
            {
                var Pi = pat.Item1[i];
                var occ_Pi = new SortedSet<int>(sa.FindOccurences(Pi.ToCharArray()));
                foreach (var r in occ_Pi)
                {
                    ++permitted[r - Val[i]];
                    if (permitted[r - Val[i]] == l)
                    {
                        occ_P.Add(r - Val[i]);
                    }
                }
            }
            return occ_P;
        }

        public IEnumerable<int> FindIdcp(string T, string P)
        {
            var sa = new SuffixArray<char>(T.ToCharArray());
            Tuple<string[], int[]> pat = splitPattern(P);
            var l = pat.Item1.Length;
            var Val = new int[l];
            Val[0] = 0;
            for (int i = 1; i < l; ++i)
            {
                Val[i] = Val[i - 1] + pat.Item1[i - 1].Length + pat.Item2[i - 1];
            }
            var occ_P = new SortedSet<int>();
            var P_1 = pat.Item1[0];
            var occ_P1 = sa.FindOccurences(P_1.ToCharArray());
            var permitted = occ_P1.ToDictionary(i => i, v => 1);
            for (int i =0; i < l; ++i)
            {
                var Pi = pat.Item1[i];
                var occ_Pi = sa.FindOccurences(Pi.ToCharArray());
                foreach (var r in occ_Pi)
                {
                    int r_val = r - Val[i];
                    int c;
                    if (permitted.TryGetValue(r_val, out c))
                    {
                        ++c;
                        permitted[r_val] = c;
                        if (c == l)
                            occ_P.Add(r_val);
                    }
                }
            }
            return occ_P;
        }

        private Tuple<string[], int[]> splitPattern(string P)
        {
            var Ps = new List<string>();
            var Pi = new StringBuilder();
            var Ks = new List<int>();
            bool inWildcards = false;
            foreach (var ch in P)
            {
                if (ch == '*')
                {
                    if (inWildcards)
                    {
                        Ks[Ks.Count - 1]++;
                    }
                    else
                    {
                        Ps.Add(Pi.ToString());
                        Pi = new StringBuilder();
                        Ks.Add(1);
                    }
                }
                else
                {
                    if (inWildcards)
                    {
                        inWildcards = false;
                    }
                    Pi.Append(ch);
                }
            }
            Ps.Add(Pi.ToString());
            return Tuple.Create(Ps.ToArray(), Ks.ToArray());
        }
    }
}

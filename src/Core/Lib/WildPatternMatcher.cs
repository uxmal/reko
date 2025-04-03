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


        /// <summary>
        /// Given a text T over the alphabet Σ and a pattern P
        /// over the alphabet Σ∪{∗}, find all the occurrences of
        /// P in T.
        /// </summary>
        /// <param name="T"></param>
        /// <param name="P"></param>
        public IEnumerable<int> FindDcp(string T, string P)
        {
            var sa = new SuffixArray<char>(T.ToCharArray());
            int textLength = T.Length;
            int[] permitted = new int[T.Length];
            (string[] patternParts, int[] dontCares) = SplitPattern(P);
            var patternLength = patternParts.Length;
            var Val = CalculateOffsets(patternParts, dontCares);
            var occ_P = new SortedSet<int>();
            for (int i = 0; i < patternLength; ++i)
            {
                var Pi = patternParts[i];
                var nextPatternWildcards = i<dontCares.Length ? dontCares[i] : 0;
                var occ_Pi = new SortedSet<int>(sa.FindOccurences(Pi.ToCharArray()));
                foreach (var pos in occ_Pi)
                {
                    int adjustedPos = pos - Val[i];
                    if (!IsValidPos(adjustedPos,Val[i], nextPatternWildcards, textLength))
                        continue;
                    ++permitted[adjustedPos];
                    if (permitted[adjustedPos] == patternLength)
                    {
                        occ_P.Add(adjustedPos);
                    }
                }
            }
            return occ_P;
        }
        /// <summary>
        /// Checks that a match at the position <paramref name="pos" /> and
        /// length <paramref name="len" /> fits in the text
        /// </summary>
        private bool IsValidPos(int pos,int len,int nextPatternWildcards,int textLength)
        {
            return pos >= 0 && pos + len + nextPatternWildcards < textLength;
        }

        public IEnumerable<int> FindIdcp(string T, string P)
        {
            var sa = new SuffixArray<char>(T.ToCharArray());
            int textLength = T.Length;
            (string[] patternParts, int[] dontCares) = SplitPattern(P);
            var patternLength = patternParts.Length;
            var Val = CalculateOffsets(patternParts, dontCares);
            var occ_P = new SortedSet<int>();
            var P_1 = patternParts[0];
            var occ_P1 = sa.FindOccurences(P_1.ToCharArray());
            var permitted = occ_P1.ToDictionary(i => i, v => 1);
            for (int i =0; i < patternLength; ++i)
            {
                var Pi = patternParts[i];
                var nextPatternWildcards = i<dontCares.Length ? dontCares[i] : 0;
                var occ_Pi = sa.FindOccurences(Pi.ToCharArray());
                foreach (var pos in occ_Pi)
                {
                    int adjustedPos = pos - Val[i];
                    if (!IsValidPos(adjustedPos,Val[i], nextPatternWildcards, textLength))
                        continue;
                    if (permitted.TryGetValue(adjustedPos, out int c))
                    {
                        ++c;
                        permitted[adjustedPos] = c;
                        if (c == patternLength)
                            occ_P.Add(adjustedPos);
                    }
                }
            }
            return occ_P;
        }

        private int[] CalculateOffsets(string[] patternParts, int[] wildcards)
        {
            int length = patternParts.Length;
            var offsets = new int[length];      
            offsets[0] = 0;
            for (int i = 1; i < length; ++i)
            {
                offsets[i] = offsets[i - 1] + patternParts[i - 1].Length + wildcards[i - 1];
            }
            return offsets;
        }

        private (string[], int[]) SplitPattern(string P)
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
                        inWildcards = true;
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
            if (Pi.Length > 0) // pattern can be followed by a wildcard, and the Pi will be empty then
            {
                Ps.Add(Pi.ToString());
            }
            return (Ps.ToArray(), Ks.ToArray());
        }
    }
}

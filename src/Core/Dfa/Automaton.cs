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

namespace Reko.Core.Dfa
{
    /// <summary>
    /// Deterministic finite automaton that matches a pattern of bytes in an array.
    /// </summary>
    public class Automaton
    {
        private int[,] transitions;
        private State[] states;

        public static Automaton CreateFromPattern(string pattern)
        {
            try
            {
                var parser = new PatternParser(pattern);
                var tree = parser.Parse();
                var builder = new DfaBuilder(tree);
                builder.ExtendWithEos();
                builder.BuildNodeSets();
                builder.BuildAutomaton(tree);
                return new Automaton(builder.States, builder.Transitions);
            }
            catch
            {
                return null;
            }
        }

        public Automaton(State[] states, int[,] transitions)
        {
            this.states = states;
            this.transitions = transitions;
        }

        public IEnumerable<int> GetMatches(byte[] bytes, int iStart)
        {
            return GetMatches(bytes, iStart, bytes.Length);
        }

        /// <summary>
        /// Returns a sequence of positions at which the automaton pattern matches.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="iStart"></param>
        /// <returns></returns>
        public IEnumerable<int> GetMatches(byte[] bytes, int iStart, int iEnd)
        {
            bool isMatching = false; // true if we have found the beginning of a pattern and are matching.
            int lastMatchPos = -1;   // Last position we found the start of a pattern.
            int lastAcceptPos = -1;  // Last position we entered an accepting state.
            int iState = 0;
            for (int i = iStart; i < iEnd; ++i)
            {
                var dst = transitions[iState,bytes[i]];
                if (dst == 0)
                {
                    if (lastMatchPos != -1 && lastAcceptPos != -1)
                    {
                        yield return lastMatchPos;
                        i = lastAcceptPos;
                    }
                    else if (isMatching)
                    {
                        --i;
                    }
                    isMatching = false;
                    lastMatchPos = -1;
                    lastAcceptPos = -1;
                }
                else
                {
                    var st = states[dst];
                    if (!isMatching)
                    {
                        lastMatchPos = i;
                        isMatching = true;
                    }
                    if (isMatching && st.Accepts)
                    {
                        lastAcceptPos = i;
                    }
                }
                iState = dst;
            }
            if (isMatching)
                yield return lastMatchPos;
        }
    }
}

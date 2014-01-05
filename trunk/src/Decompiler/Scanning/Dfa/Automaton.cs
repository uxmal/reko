#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

namespace Decompiler.Scanning.Dfa
{
    /// <summary>
    /// Deterministic finite automaton that matches a pattern of bytes in an array.
    /// </summary>
    public class Automaton
    {
        private int[,] transitions;
        private State[] states;

        public static Automaton CreateFromPatter(string pattern)
        {
            var parser = new PatternParser(pattern);
            var tree = parser.Parse();
            var builder = new DfaBuilder(tree);
            builder.ExtendWithEos();
            builder.BuildNodeSets(tree);
            builder.BuildAutomaton(tree);
            return new Automaton(builder.States, builder.Transitions);
        }

        public Automaton(State[] states, int[,] transitions)
        {
            this.states = states;
            this.transitions = transitions;
        }

        public IEnumerable<int> GetMatches(byte[] bytes, int position)
        {
            bool isMatching = false;
            int lastMatchPos = -1;
            int iState = 0;
            for (int i = position; i < bytes.Length; ++i)
            {
                var dst = transitions[iState,bytes[i]];
                if (dst < 0)
                {
                    if (lastMatchPos != -1)
                    {
                        yield return lastMatchPos;
                        lastMatchPos = -1;
                        isMatching = false;
                        --i;
                    }
                    iState = 0;
                }
                else
                {
                    iState = dst;
                    var st = states[dst];
                    if (!isMatching)
                    {
                        lastMatchPos = i;
                        isMatching = true;
                    }
                    else if (st.Starts && isMatching)
                    {
                        lastMatchPos = i;
                    }
                }
            }
            if (isMatching)
                yield return lastMatchPos;
        }
    }
}

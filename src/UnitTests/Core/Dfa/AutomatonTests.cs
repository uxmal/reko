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

using Reko.Core.Dfa;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Dfa
{
    [TestFixture]
    public class DfAutomatonTests
    {
        private Automaton automaton;

        [Test]
        public void Dfa_SingleState()
        {
            var automaton = Given_SingleStateAutomaton();

            Assert.AreEqual(
                new int[] { 1 },
                automaton.GetMatches(new byte[] {0, 2}, 0).ToArray());
        }

        [Test]
        public void Dfa_SingleState_NoMatch()
        {
            var automaton = Given_SingleStateAutomaton();

            Assert.AreEqual(
                new int[] { },
                automaton.GetMatches(new byte[] { 0, 3 }, 0).ToArray());
        }

        [Test]
        public void Dfa_SingleState_ThreeMatches()
        {
            var automaton = Given_SingleStateAutomaton();

            Assert.AreEqual(
                new int[] { 0, 1, 2 },
                automaton.GetMatches(new byte[] { 2, 2, 2 }, 0).ToArray());
        }

        [Test]
        public void Dfa_Longmatch()
        {
            var automaton = Given_LongMatchAutomaton();
            Assert.AreEqual(
                new int[] { 3 },
                automaton.GetMatches(new byte[] { 3, 3, 3, 2, 2, 0, 1, 3 }, 0).ToArray());
        }

        private static Automaton Given_LongMatchAutomaton()
        {
            var automaton = Automaton.CreateFromPattern("02+");
            return automaton;
        }

        private static Automaton Given_SingleStateAutomaton()
        {
            var automaton = Automaton.CreateFromPattern("02");
            return automaton;
        }

        [Test]
        public void Dfa_TwoBytes()
        {
             Given_TextAutomaton("55 8B");
             Assert.AreEqual(
                new int[] { 3 },
                automaton.GetMatches(new byte[] { 0x00, 0x00, 0x00, 0x55, 0x8B, 0x00, 0x00}, 0).ToArray());
        }

        [Test]
        public void Dfa_TwoOccurrences()
        {
            Given_TextAutomaton("55 8B");
            Assert.AreEqual(
                new int[] { 3, 9 },
                automaton.GetMatches(new byte[] { 0x00, 0x00, 0x00, 0x55, 0x8B, 0x00, 0x00, 0x00, 0x00, 0x55, 0x8B }, 0).ToArray());
        }

        [Test]
        public void Dfa_TwoOccurrences_Wildcards()
        {
            Given_TextAutomaton("55+8B");
            Assert.AreEqual(
                new int[] { 2, 8 },
                automaton.GetMatches(new byte[] { 0x00, 0x00, 0x55, 0x55, 0x8B, 0x00, 0x00, 0x00, 0x55, 0x55, 0x8B }, 0).ToArray());
        }

        [Test]
        public void Dfa_Regressions()
        {
            Given_TextAutomaton("55 8B");
            Assert.AreEqual(
                new int[] { 8 },
                automaton.GetMatches(new byte[] { 0x00, 0x00, 0x55, 0x55, 0x00, 0x00, 0x00, 0x55, 0x55, 0x8B }, 0).ToArray());
        }

        [Test]
        public void Dfa_MatchAny()
        {
            Given_TextAutomaton("??");
            Assert.AreEqual(
                new int[] { 0 },
                automaton.GetMatches(new byte[] { 0x1 }, 0).ToArray());
        }

        [Test]
        public void Dfa_MatchAny2()
        {
            Given_TextAutomaton("55 ??");
            Assert.AreEqual(
                new int[] { 2 },
                automaton.GetMatches(new byte[] { 0x11, 0x22, 0x55, 0x41 }, 0).ToArray());
        }

        private void Given_TextAutomaton(string pattern)
        {
            this.automaton = Automaton.CreateFromPattern(pattern);
        }
    }
}

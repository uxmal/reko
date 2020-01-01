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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Dfa
{
    [TestFixture]
    public class DfaBuilderTests
    {
        [Test]
        public void Dfab_Simple()
        {
            var dfab = new DfaBuilder("af");
            dfab.ExtendWithEos();
            dfab.BuildNodeSets();
            dfab.BuildAutomaton();
            Assert.AreEqual(2, dfab.States.Length, "Should have 2 states.");
            Assert.AreEqual(1, dfab.Transitions[0, 0xAF]);
        }

        [Test]
        public void Dfab_Plus()
        {
            var dfab = new DfaBuilder("CC+");
            dfab.ExtendWithEos();
            dfab.BuildNodeSets();
            dfab.BuildAutomaton();
            Debug.WriteLine(dfab.ParseTree.ToString());
            Assert.AreEqual(2, dfab.States.Length, "Should have 2 states.");
            Assert.AreEqual(1, dfab.Transitions[0, 0xCC]);
            Assert.AreEqual(1, dfab.Transitions[1, 0xCC]);
        }

        [Test]
        public void Dfab_PlusCat()
        {
            var dfab = new DfaBuilder("CC+55");
            dfab.ExtendWithEos();
            dfab.BuildNodeSets();
            dfab.BuildAutomaton();
            Debug.WriteLine(dfab.ParseTree.ToString());
            Assert.AreEqual(3, dfab.States.Length, "Should have 3 states.");
            Assert.AreEqual(1, dfab.Transitions[0, 0xCC]);
            Assert.AreEqual(1, dfab.Transitions[1, 0xCC]);
            Assert.AreEqual(2, dfab.Transitions[1, 0x55]);
        }

        [Test]
        public void Dfab_StartsWith()
        {
            var dfab = new DfaBuilder("BB][CC");
            dfab.ExtendWithEos();
            dfab.BuildNodeSets();
            dfab.BuildAutomaton();
            //Debug.WriteLine(dfab.ParseTree.ToString());
            Assert.IsTrue(dfab.States[2].Starts);
        }

        [Test]
        public void Dfab_MatchAny()
        {
            var dfab = new DfaBuilder("??");
            dfab.ExtendWithEos();
            dfab.BuildNodeSets();
            dfab.BuildAutomaton();
            Assert.AreEqual(2, dfab.States.Length, "Should have 2 states");
            Assert.AreEqual(1, dfab.Transitions[0, 0xAF]);
        }
    }
}

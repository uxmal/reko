/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Lib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Core.Lib
{
    [TestFixture]
    public class DominatorGraphTests
    {
        DirectedGraphImpl<string> graph;
        private DominatorGraph<string> pdg;

        [SetUp]
        public void Setup()
        {
            graph = new DirectedGraphImpl<string>();
        }
        [Test]
        public void SingleItem()
        {
            graph.AddNode("a");
            CompileTest(graph, "a");

            Assert.AreEqual(null, pdg.ImmediateDominator("a"));
        }

        [Test]
        public void TwoItems()
        {
            graph.AddNode("b");
            graph.AddNode("a");
            graph.AddEdge("b", "a");

            CompileTest(graph, "b");
            Assert.AreEqual("b", pdg.ImmediateDominator("a"));
            Assert.IsNull(pdg.ImmediateDominator("b"));

        }

        [Test]
        public void Diamond()
        {
            graph.AddNode("a");
            graph.AddNode("b");
            graph.AddNode("c");
            graph.AddNode("d");
            graph.AddEdge("b", "a");
            graph.AddEdge("d", "b");
            graph.AddEdge("c", "a");
            graph.AddEdge("d", "c");

            CompileTest(graph, "d");
            Assert.AreEqual("d", pdg.ImmediateDominator("a"));
            Assert.AreEqual("d", pdg.ImmediateDominator("b"));
            Assert.AreEqual("d", pdg.ImmediateDominator("c"));
            Assert.IsNull(pdg.ImmediateDominator("d"));
        }

        [Test]
        public void PreLoop()
        {
            graph.AddEdge("b", "a");
            graph.AddEdge("a", "b");
            graph.AddEdge("c", "a");

            CompileTest(graph, "c");
            Assert.AreEqual("c", pdg.ImmediateDominator("a"));
            Assert.AreEqual("a", pdg.ImmediateDominator("b"));
            Assert.IsNull(pdg.ImmediateDominator("c"));
        }

        [Test]
        public void InfiniteLoop()
        {
            graph.AddNode("a");
            graph.AddNode("b");
            graph.AddNode("c");
            graph.AddEdge("a", "b");
            graph.AddEdge("b", "a");

            CompileTest(graph, "c");

            Assert.AreEqual("a", pdg.ImmediateDominator("b"));
            Assert.AreEqual("b", pdg.ImmediateDominator("a"));
        }

        private void CompileTest(DirectedGraphImpl<string> e, string entry)
        {
            pdg = new DominatorGraph<string>(e, entry);
        }
    }
}

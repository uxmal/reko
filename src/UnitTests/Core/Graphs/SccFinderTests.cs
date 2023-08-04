using NUnit.Framework;
using Reko.Analysis;
using Reko.Core.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Graphs
{
    [TestFixture]
    internal class SccFinderTests
    {
        [Test]
        public void Sccf_CyclicGraph()
        {
            var g = new AdjacencyListGraph<string>(new Dictionary<string, List<string>>
            {
                { "main", new() { "fact"} },
                { "fact", new() { "fact"} }
            });

            var cond = SccFinder.Condense(g);
            var gc = cond.Graph;
            Assert.AreEqual(1, gc.Predecessors(0).Count);
            Assert.AreEqual(0, gc.Predecessors(1).Count);
            Assert.AreEqual(0, gc.Successors(0).Count);
            Assert.AreEqual(1, gc.Successors(1).Count);
            cond.ToString();
        }
    }
}

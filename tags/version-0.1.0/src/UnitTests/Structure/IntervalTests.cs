using Decompiler.Core;
using Decompiler.Structure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class IntervalTests
    {
        private Procedure proc;

        [SetUp]
        public void Setup()
        {
            proc = new Procedure("test", null);
        }

        [Test]
        public void CreateWithCfgNode()
        {
            StructureNode node = CreateNode("foo", 1);
            Interval i = new Interval(1, node);
//            Assert.AreSame(node, i.Header);
            Assert.AreEqual(1, i.Nodes.Count);
        }

        [Test]
        public void CreateWithIntervalNode()
        {
            StructureNode node1 = CreateNode("node1", 1);
            StructureNode node2 = CreateNode("node2", 2);
            Interval i1 = new Interval(1, node1);
            Interval i2 = new Interval(2, node2);
            StructureNode node3 = CreateNode("node3", 3);
            i2.AddNode(node3);

            Interval i = new Interval(3, i1);
            i.AddNode(i2);
            Assert.AreEqual(2, i.Nodes.Count);
        }

        private StructureNode CreateNode(string name, int id)
        {
            Block b = new Block(null, name);
            StructureNode node = new StructureNode(b, id);
            return node;
        }
    }
}

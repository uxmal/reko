using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Structure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class StructureNodeTests
    {
        Procedure proc;
        int nodeCount;

        [SetUp]
        public void Setup()
        {
            proc = new Procedure("Test", new Frame(PrimitiveType.Word32));
            nodeCount = 1;
        }

        [Test]
        public void SetLoopStamps()
        {
            StructureNode n1 = NewNode("n1");
            StructureNode n2 = NewNode("n2");
            StructureNode n3 = NewNode("n3");
            n1.AddEdgeTo(n2);
            n2.AddEdgeTo(n3);
            n3.AddEdgeTo(n2);
            int time = 1;
            List<StructureNode> postOrder = new List<StructureNode>();
            n1.SetLoopStamps(ref time, postOrder);
            Assert.AreEqual(0, n1.InEdges.Count);
            Assert.AreEqual(2, n2.InEdges.Count);
            Assert.IsTrue(n2.InEdges.Contains(n1));
            Assert.IsTrue(n2.InEdges.Contains(n3));
            Assert.AreEqual(1, n3.InEdges.Count);
            Assert.IsTrue(n3.InEdges.Contains(n2));
            Assert.AreSame(n3, postOrder[0]);
            Assert.AreSame(n2, postOrder[1]);
            Assert.AreSame(n1, postOrder[2]);

        }

        private StructureNode NewNode(string name)
        {
            return new StructureNode(proc.AddBlock(name), nodeCount++);
        }
    }
}

using Decompiler.Core;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class SccLoopFinderTests
    {
        [Test]
        public void FindNoLoopInInterval()
        {
            ProcedureMock m = new ProcedureMock();
            m.Return();
            m.Procedure.RenumberBlocks();

            StructureNode node = new StructureNode(m.Procedure.RpoBlocks[1], 3);
            node.Order = 0;
            IntNode interval = new IntNode(1, node);

            bool [] nodesInInterval = new bool[30];
            interval.FindNodesInInt(nodesInInterval, 0);
            SccLoopFinder finder = new SccLoopFinder(interval, nodesInInterval);
            IList<StructureNode> loopNodes = finder.FindLoop();

            Assert.AreEqual(0, loopNodes.Count);
        }

        [Test]
        public void FindSelfLoopInInterval()
        {
            ProcedureStructure proc = CompileTest(delegate(ProcedureMock m)
            {
                m.Label("lupe");
                m.BranchIf(m.LocalBool("f"), "lupe");
                m.Return();
            });

            SccLoopFinder finder = CreateSccLoopFinder(proc, proc.DerivedGraphs[0].Intervals[1], 0);
            IList<StructureNode> loopNodes = finder.FindLoop();
            Assert.AreEqual(1, loopNodes.Count);
            Assert.AreEqual("lupe", loopNodes[0].Block.Name);
        }

        private SccLoopFinder CreateSccLoopFinder(ProcedureStructure proc, IntNode intNode, int graphLevel)
        {
            bool [] nodes = new bool[proc.Ordering.Count];
            intNode.FindNodesInInt(nodes, graphLevel);
            return new SccLoopFinder(intNode, nodes);
        }


        [Test]
        public void FindMultiNodeLoop()
        {
            ProcedureStructure proc = CompileTest(delegate(ProcedureMock m)
            {
                m.Label("lupe");
                m.BranchIf(m.LocalBool("a"), "branch_true");
                m.SideEffect(m.Fn("foo"));
                m.Jump("join");
                m.Label("branch_true");
                m.SideEffect(m.Fn("bar"));
                m.Label("join");
                m.BranchIf(m.LocalBool("b"), "lupe");
                m.Return();
            });

            SccLoopFinder finder = CreateSccLoopFinder(proc, proc.DerivedGraphs[0].Intervals[1], 0);
            IList<StructureNode> loopNodes = finder.FindLoop();
            Assert.AreEqual(4, loopNodes.Count);
            Assert.AreEqual("branch_true", loopNodes[0].Block.Name);
        }

        private ProcedureStructure CompileTest(ProcGenerator g)
        {
            ProcedureMock m = new ProcedureMock();
            g(m);
            return CompileTest(m);
        }

        private ProcedureStructure CompileTest(ProcedureMock m)
        {
            m.Procedure.RenumberBlocks();
            ProcedureStructureBuilder g = new ProcedureStructureBuilder();
            Dictionary<Block, StructureNode> nodes = new Dictionary<Block, StructureNode>();
            g.BuildNodes(m.Procedure, nodes);
            g.DefineEdges(m.Procedure, nodes);
            ProcedureStructure proc = g.DefineCfgs(m.Procedure, nodes);
            g.SetTimeStamps(proc);

            DerivedSequenceBuilder gr = new DerivedSequenceBuilder();
            gr.BuildDerivedSequence(proc);
            return proc;
        }
    }
}

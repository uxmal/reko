using Decompiler.Core;
using Decompiler.Core.Lib;
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

            HashSet<StructureNode> nodesInInterval = interval.FindIntervalNodes(0);
            SccLoopFinder finder = new SccLoopFinder(interval, nodesInInterval);
            HashSet<StructureNode> loopNodes = finder.FindLoop();
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
            HashSet<StructureNode> loopNodes = finder.FindLoop();
            Assert.AreEqual(1, loopNodes.Count);
            Assert.AreEqual("lupe", loopNodes.ToArray()[0].Block.Name);
        }

        private SccLoopFinder CreateSccLoopFinder(ProcedureStructure proc, IntNode intNode, int graphLevel)
        {
            HashSet<StructureNode> nodesInInterval = intNode.FindIntervalNodes(graphLevel);
            return new SccLoopFinder(intNode, nodesInInterval);
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
            HashSet<StructureNode> loopNodes = finder.FindLoop();
            Assert.AreEqual(4, loopNodes.Count);
            Assert.IsTrue(loopNodes.Contains(proc.Nodes[2]));
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
            ProcedureStructureBuilder g = new ProcedureStructureBuilder(m.Procedure);
            g.BuildNodes();
            g.DefineEdges();
            ProcedureStructure proc = g.CreateProcedureStructure();
            g.SetTimeStamps();

            DerivedSequenceBuilder gr = new DerivedSequenceBuilder();
            gr.BuildDerivedSequence(proc);
            return proc;
        }
    }
}

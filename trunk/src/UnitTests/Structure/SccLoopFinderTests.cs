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
    public class SccLoopFinderTests : StructureTestBase
    {
        [Test]
        public void FindNoLoopInInterval()
        {
            ProcedureMock m = new ProcedureMock();
            m.Return();
            m.Procedure.RenumberBlocks();

            StructureNode node = new StructureNode(m.Procedure.RpoBlocks[1], 3);
            node.Order = 0;
            Interval interval = new Interval(1, node);

            HashedSet<StructureNode> nodesInInterval = interval.FindIntervalNodes(0);
            SccLoopFinder finder = new SccLoopFinder(interval, nodesInInterval);
            HashedSet<StructureNode> loopNodes = finder.FindLoop();
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
            HashedSet<StructureNode> loopNodes = finder.FindLoop();
            Assert.AreEqual(1, loopNodes.Count);
            Assert.AreEqual("lupe", loopNodes.ToArray()[0].Block.Name);
        }

        private SccLoopFinder CreateSccLoopFinder(ProcedureStructure proc, Interval intNode, int graphLevel)
        {
            HashedSet<StructureNode> nodesInInterval = intNode.FindIntervalNodes(graphLevel);
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
            HashedSet<StructureNode> loopNodes = finder.FindLoop();
            Assert.AreEqual(4, loopNodes.Count);
            Assert.IsTrue(loopNodes.Contains(proc.Nodes[2]));
        }

        [Test]
        public void Reg00013()
        {
            ProcedureStructure proc = CompileTest("Fragments/regressions/r00013.asm");
            proc.DumpDerivedSequence(Console.Out);
            for (int j = 0; j < proc.DerivedGraphs.Count; ++j)
            {
                for (int i = 0; i < proc.DerivedGraphs[j].Intervals.Count; ++i)
                {
                    Interval interval = proc.DerivedGraphs[j].Intervals[i];
                    SccLoopFinder finder = CreateSccLoopFinder(proc, interval, j);
                    HashedSet<StructureNode> loopNodes = finder.FindLoop();
                    StructureNode[] items = loopNodes.ToArray();
                    Array.Sort<StructureNode>(items, delegate(StructureNode a, StructureNode b) { return string.Compare(a.Name, b.Name); });
                    foreach (StructureNode sn in items)
                    {
                        Console.Out.Write(sn.Name + " ");
                    }
                    Console.Out.WriteLine();
                }
            }
        }

        private ProcedureStructure CompileTest(ProcGenerator g)
        {
            ProcedureMock m = new ProcedureMock();
            g(m);
            return CompileTest(m.Procedure);
        }

        private ProcedureStructure CompileTest(Procedure proc)
        {
            proc.RenumberBlocks();
            ProcedureStructureBuilder g = new ProcedureStructureBuilder(proc);
            g.BuildNodes();
            g.DefineEdges();
            ProcedureStructure ps = g.CreateProcedureStructure();
            g.SetTimeStamps();

            DerivedSequenceBuilder gr = new DerivedSequenceBuilder(ps);
            return ps;
        }

        private ProcedureStructure CompileTest(string asmfile)
        {
            RewriteProgram(asmfile, new Address(0x0C00, 0));
            return CompileTest(prog.Procedures.Values[0]);
        }

    }
}

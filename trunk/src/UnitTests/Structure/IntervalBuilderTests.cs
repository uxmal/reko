using Decompiler.Core;
using Decompiler.UnitTests.Mocks;
using Decompiler.Structure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Structure
{

    [TestFixture]
    public class IntervalBuilderTests
    {

        private DerivedGraph g;
        private ProcedureStructure ph;

        [Test]
        public void SimpleGraph()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Assign(m.Local16("a"), m.Word16(42));
                m.Return();
            });

            Assert.AreEqual(1, g.Intervals.Count);
        }

        [Test]
        public void IfThen()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.BranchIf(m.LocalBool("f"), "join");
                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.Label("join");
                m.Return();
            });
            Assert.AreEqual(1, g.Intervals.Count);
        }

        [Test]
        public void Loop()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("loop");
                m.BranchIf(m.LocalBool("f"), "done");

                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.Jump("loop");

                m.Label("done");
                m.Return();
            });
            Assert.AreEqual(2, g.Intervals.Count, "Expected 2 intervals");
            Assert.AreEqual("Interval 0: [ProcedureMock_entry]", g.Intervals[0].ToString());
            Assert.AreEqual("Interval 1: [done,l1,loop,ProcedureMock_exit]", g.Intervals[1].ToString());
        }

        [Test]
        public void TwoLoops()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("loop1");
                m.BranchIf(m.LocalBool("f"), "loop2");

                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.Jump("loop1");

                m.Label("loop2");
                m.BranchIf(m.LocalBool("f"), "done");

                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.Jump("loop2");

                m.Label("done");
                m.Return();
            });
            Assert.AreEqual(3, g.Intervals.Count);
            Assert.AreEqual("Interval 0: [ProcedureMock_entry]", g.Intervals[0].ToString());
            Assert.AreEqual("Interval 1: [l1,loop1]", g.Intervals[1].ToString());
            Assert.AreEqual("Interval 2: [done,l2,loop2,ProcedureMock_exit]", g.Intervals[2].ToString());
        }

        [Test]
        public void Switch()
        {
            RunTest(new MockSwitch());
            Assert.AreEqual("Interval 0: [case0,case1,case2,case3,default,done,l1,l2,MockSwitch_entry,MockSwitch_exit]", g.Intervals[0].ToString());
            Assert.AreEqual(1, g.Intervals.Count);
        }

        protected virtual void RunTest(ProcGenerator pg)
        {
            ProcedureMock pm = new ProcedureMock();
            pg(pm);
            pm.Procedure.RenumberBlocks();
            RunTest(pm.Procedure);
        }

        protected virtual void RunTest(ProcedureMock m)
        {
            RunTest(m.Procedure);
        }

        protected virtual void RunTest(Procedure proc)
        {
            ProcedureStructureBuilder graphs = new ProcedureStructureBuilder();
            Dictionary<Block, StructureNode> nodes = new Dictionary<Block, StructureNode>();
            graphs.BuildNodes(proc, nodes);
            graphs.DefineEdges(proc, nodes);
            ph = graphs.DefineCfgs(proc, nodes);
            graphs.SetTimeStamps(ph);

            IntervalBuilder ib = new IntervalBuilder();
            g = new DerivedGraph();
            g.cfg = ph.EntryNode;
            ib.BuildIntervals(g);
        }
    }
}

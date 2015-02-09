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
    public class IntervalBuilderTests : StructureTestBase
    {
        private ProcedureStructure ph;
        private List<Interval> intervals;

        [Test]
        public void IvbSimpleGraph()
        {
            RunTest(delegate(ProcedureBuilder m)
            {
                m.Assign(m.Local16("a"), m.Word16(42));
                m.Return();
            });

            Assert.AreEqual(1, intervals.Count);
        }

        [Test]
        public void IvbIfThen()
        {
            RunTest(delegate(ProcedureBuilder m)
            {
                m.BranchIf(m.LocalBool("f"), "join");
                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.Label("join");
                m.Return();
            });
            Assert.AreEqual(1, intervals.Count);
        }

        [Test]
        public void IvbLoop()
        {
            RunTest(delegate(ProcedureBuilder m)
            {
                m.Label("loop");
                m.BranchIf(m.LocalBool("f"), "done");

                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.Jump("loop");

                m.Label("done");
                m.Return();
            });
            Assert.AreEqual(2, intervals.Count, "Expected 2 intervals");
            Assert.AreEqual("Interval 0: [ProcedureBuilder_entry]", intervals[0].ToString());
            Assert.AreEqual("Interval 1: [done,l1,loop,ProcedureBuilder_exit]", intervals[1].ToString());
        }

        [Test]
        public void IvbTwoLoops()
        {
            RunTest(delegate(ProcedureBuilder m)
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
            Assert.AreEqual(3, intervals.Count);
            Assert.AreEqual("Interval 0: [ProcedureBuilder_entry]", intervals[0].ToString());
            Assert.AreEqual("Interval 1: [l1,loop1]", intervals[1].ToString());
            Assert.AreEqual("Interval 2: [done,l2,loop2,ProcedureBuilder_exit]", intervals[2].ToString());
        }

        [Test]
        public void Switch()
        {
            RunTest(new MockSwitch());
            Assert.AreEqual(1, intervals.Count);
            Assert.AreEqual("Interval 0: [case0,case1,case2,case3,default,done,l1,l2,MockSwitch_entry,MockSwitch_exit]", intervals[0].ToString());
        }

        protected virtual void RunTest(Action<ProcedureBuilder> pg)
        {
            ProcedureBuilder pm = new ProcedureBuilder();
            pg(pm);
            RunTest(pm.Procedure);
        }

        protected virtual void RunTest(ProcedureBuilder m)
        {
            RunTest(m.Procedure);
        }

        protected virtual void RunTest(Procedure proc)
        {
            ProcedureStructureBuilder graphs = new ProcedureStructureBuilder(proc);
            graphs.BuildNodes();
            graphs.DefineEdges();
            ph = graphs.CreateProcedureStructure();
            graphs.SetTimeStamps();

            IntervalBuilder ib = new IntervalBuilder();
            intervals = ib.BuildIntervals(new StructureGraphAdapter(ph.Nodes), ph.EntryNode);
        }

    }
}

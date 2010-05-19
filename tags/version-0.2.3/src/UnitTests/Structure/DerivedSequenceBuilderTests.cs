using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class DerivedSequenceBuilderTests
    {
        private ProcedureStructure proc;

        [Test]
        public void LoopTestdead()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("loop");
                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.BranchIf(m.LocalBool("f"), "loop");
                m.Return();
            });
            Assert.AreEqual(4, proc.DerivedGraphs[0].Graph.Nodes.Count);
            Assert.AreEqual(2, proc.DerivedGraphs[1].Graph.Nodes.Count);
            Assert.AreEqual(1, proc.DerivedGraphs[2].Graph.Nodes.Count);
        }


        [Test]
        public void BuildSingleNodeGraph()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.Return();
            });

            DerivedSequenceBuilder seq = new DerivedSequenceBuilder(proc);
            Assert.AreEqual(3, proc.Nodes.Count);
            Assert.AreEqual(2, proc.DerivedGraphs.Count);

            Assert.AreEqual(3, proc.DerivedGraphs[0].Graph.Nodes.Count);
            Assert.AreEqual(1, proc.DerivedGraphs[0].Intervals.Count);
            Assert.AreEqual(1, proc.DerivedGraphs[1].Graph.Nodes.Count);
            Assert.AreEqual(1, proc.DerivedGraphs[1].Intervals.Count);
        }

        [Test]
        public void LoopTest()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.Label("loop");
                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.BranchIf(m.LocalBool("f"), "loop");
                m.Return();
            });

            DerivedSequenceBuilder seq = new DerivedSequenceBuilder(proc);
            proc.Dump();

            Assert.AreEqual(4, proc.DerivedGraphs[0].Graph.Nodes.Count);
            Assert.AreEqual(2, proc.DerivedGraphs[0].Intervals.Count);
            Assert.AreEqual(2, proc.DerivedGraphs[1].Graph.Nodes.Count);
            Assert.AreEqual(1, proc.DerivedGraphs[1].Intervals.Count);
            Assert.AreEqual(1, proc.DerivedGraphs[2].Graph.Nodes.Count);
            Assert.AreEqual(1, proc.DerivedGraphs[2].Intervals.Count);
        }

        [Test]
        public void NonReducible()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.BranchIf(m.Fn("foo"), "right");

                m.Label("left");
                m.SideEffect(m.Fn("Left"));
                m.Jump("right");

                m.Label("right");
                m.SideEffect(m.Fn("Right"));
                m.BranchIf(m.Fn("exitTest"), "left");

                m.Return();
            });
            DerivedSequenceBuilder seq = new DerivedSequenceBuilder(proc);
            Assert.AreEqual(2, proc.DerivedGraphs.Count, "Expected 2 graphs");
            Assert.AreEqual(6, proc.DerivedGraphs[0].Graph.Nodes.Count);
            Assert.AreEqual(3, proc.DerivedGraphs[1].Graph.Nodes.Count, "Expected 3 nodes in top level graph");
        }


        private string DumpBoolArray(bool[] blocks)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < blocks.Length; ++i)
                sb.Append(blocks[i] ? '1' : '0');
            return sb.ToString();
        }

        protected virtual void RunTest(Action<ProcedureMock> pg)
        {
            ProcedureMock pm = new ProcedureMock();
            pg(pm);
            RunTest(pm);
        }

        protected virtual void RunTest(ProcedureMock pm)
        {
            CompileTest(pm);
            DerivedSequenceBuilder gr = new DerivedSequenceBuilder(proc);
        }


        protected virtual void CompileTest(Action<ProcedureMock> pg)
        {
            ProcedureMock pm = new ProcedureMock();
            pg(pm);
            CompileTest(pm);
        }

        private void CompileTest(ProcedureMock pm)
        {
            pm.Procedure.RenumberBlocks();
            ProcedureStructureBuilder g = new ProcedureStructureBuilder(pm.Procedure);
            Dictionary<Block, StructureNode> nodes = new Dictionary<Block, StructureNode>();
            g.BuildNodes();
            g.DefineEdges();
            proc = g.CreateProcedureStructure();
            g.SetTimeStamps();

        }

        private void Dump(List<DerivedGraph> gr, TextWriter writer)
        {
            int gNum = 0;
            foreach (DerivedGraph g in gr)
            {
                writer.Write("Graph #");
                writer.Write(gNum++);
                writer.WriteLine(":");
                foreach (Interval i in g.Intervals)
                {
                    i.Write(writer);
                    writer.WriteLine();
                }
            }
        }
    }
}

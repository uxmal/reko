using Decompiler.Core;
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
        public void LoopTest()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("loop");
                m.SideEffect(m.Fn("foo", m.Int32(4)));
                m.BranchIf(m.LocalBool("f"), "loop");
                m.Return();
            });
            Assert.AreEqual(4, proc.DerivedGraphs[0].Count);
            Assert.AreEqual(2, proc.DerivedGraphs[1].Count);
            Assert.AreEqual(1, proc.DerivedGraphs[2].Count);
        }

        [Test]
        public void NonReducible()
        {
            RunTest(delegate(ProcedureMock m)
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
            Assert.AreEqual(2, proc.DerivedGraphs.Count, "Expected 2 graphs");
            Assert.AreEqual(6, proc.DerivedGraphs[0].Count);
            Assert.AreEqual(3, proc.DerivedGraphs[1].Count, "Expected 3 nodes in top level graph");
        }

        [Test]
        public void NestedWhileLoops()
        {
            RunTest(new MockNestedWhileLoops());
            Dump(proc.DerivedGraphs, Console.Out);
            Assert.AreEqual(9, proc.DerivedGraphs[0].Count);
            Assert.AreEqual(3, proc.DerivedGraphs[1].Count);
            Assert.AreEqual(2, proc.DerivedGraphs[2].Count);
            Assert.AreEqual(1, proc.DerivedGraphs[3].Count);
            IntNode i = proc.DerivedGraphs[1].Intervals[1];
            Assert.AreEqual(4, i.Ident());
            bool [] blocks = new bool[proc.Nodes.Count];
            i.FindNodesInInt(blocks, 1);
            Assert.AreEqual("111111100", DumpBoolArray(blocks));
        }

        private string DumpBoolArray(bool[] blocks)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < blocks.Length; ++i)
                sb.Append(blocks[i] ? '1' : '0');
            return sb.ToString();
        }

        protected virtual void RunTest(ProcGenerator pg)
        {
            ProcedureMock pm = new ProcedureMock();
            pg(pm);
            RunTest(pm);
        }

        protected virtual void RunTest(ProcedureMock pm)
        {
            ProcedureStructureBuilder g = new ProcedureStructureBuilder();
            pm.Procedure.RenumberBlocks();
            Dictionary<Block,StructureNode> nodes = new Dictionary<Block,StructureNode>();
            g.BuildNodes(pm.Procedure, nodes);
            g.DefineEdges(pm.Procedure, nodes);
            proc = g.DefineCfgs(pm.Procedure, nodes);
            g.SetTimeStamps(proc);

            DerivedSequenceBuilder gr = new DerivedSequenceBuilder();
            gr.BuildDerivedSequence(proc);
        }

        private void Dump(List<DerivedGraph> gr, TextWriter writer)
        {
            int gNum = 0;
            foreach (DerivedGraph g in gr)
            {
                writer.Write("Graph #");
                writer.Write(gNum++);
                writer.WriteLine(":");
                foreach (IntNode i in g.Intervals)
                {
                    i.Write(writer);
                    writer.WriteLine();
                }
            }
        }
    }
}

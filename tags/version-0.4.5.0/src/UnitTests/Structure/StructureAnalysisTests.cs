using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class StructureAnalysisTests
    {
        [Test]
        public void StrAn_UnstructuredJumpOutOfLoop()
        {
            string sExp =
@"Node 1: Block: ProcedureBuilder_entry
    Order: 6
    Interval: 0
    iPDom: loopheader
    Structure type:
    Unstructured type: Structured
    Succ: loopheader
Node 2: Block: loopheader
    Order: 5
    Interval: 1
    iPDom: unstructuredexit
    Structure type: LoopCond
    Loop header:loopheader
    Latch: l2
    Cond follow: unstructuredexit
    Unstructured type: JumpInOutLoop
l1,done
Node 4: Block: l1
    Order: 3
    Structure type: Cond
    Loop header:loopheader
    Cond follow: unstructuredexit
    Unstructured type: JumpInOutLoop
l2,unstructuredexit
Node 7: Block: l2
    Order: 0
    Structure type: Seq
    Loop header:loopheader
    Unstructured type: Structured
loopheader
Node 5: Block: unstructuredexit
    Order: 2
    Structure type: Seq
    Unstructured type: Structured
ProcedureBuilder_exit
Node 6: Block: ProcedureBuilder_exit
    Order: 1
    Structure type: Seq
    Unstructured type: Structured

Node 3: Block: done
    Order: 4
    Structure type: Seq
    Unstructured type: Structured
unstructuredexit

";
            RunTest(delegate(ProcedureBuilder m)
            {
                m.Label("loopheader");
                m.BranchIf(m.Fn("foo"), "done");

                    m.SideEffect(m.Fn("bar"));
                    m.BranchIf(m.Fn("foo"), "unstructuredexit");
                    m.SideEffect(m.Fn("bar"));
                    m.Jump("loopheader");

                m.Label("done");
                m.SideEffect(m.Fn("bar"));

                m.Label("unstructuredexit");
                m.SideEffect(m.Fn("bar"));
                m.Return();
            }, sExp);


        }

        [Test]
        public void InfiniteLoop()
        {
            StructureAnalysis sa = CompileTest(new MockInfiniteLoop());
            Assert.AreEqual("Infinite", sa.ProcedureStructure.Nodes[1].Name);
            Assert.IsNotNull(sa.ProcedureStructure.Nodes[1].Loop, "Should be part of loop.");
            Assert.IsTrue(sa.ProcedureStructure.Nodes[1].Loop is TestlessLoop);
        }

        private void RunTest(Action<ProcedureBuilder> gen, string sExp)
        {
            ProcedureBuilder mock = new ProcedureBuilder();
            gen(mock);
            StructureAnalysis sa = CompileTest(mock);
            string s = Dump(sa.ProcedureStructure);
            Console.WriteLine(s);
            Assert.AreEqual(sExp, s);
        }

        private static StructureAnalysis CompileTest(ProcedureBuilder mock)
        {
            var sa = new StructureAnalysis(mock.Procedure);
            sa.BuildProcedureStructure();
            sa.FindStructures();
            return sa;
        }

        private string Dump(ProcedureStructure str)
        {
            StringWriter sw = new StringWriter();
            str.Write(sw);
            return sw.ToString();
        }

    }
}

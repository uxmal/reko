using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace Reko.ScannerV2.UnitTests
{
    [TestFixture]
    public class RecursiveScannerTests : AbstractScannerTests
    {
        [SetUp]
        public void Setup()
        {
            base.Setup(4096, 8);
        }

        private void RunTest(string sExpected)
        {
            var scanner = new RecursiveScanner(program, new Mock<DecompilerEventListener>().Object);
            var cfg = scanner.ScanProgram();
            scanner.RegisterPredecessors();
            var sw = new StringWriter();
            DumpCfg(cfg, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        private void DumpCfg(Cfg cfg, TextWriter w)
        {
            var g = new CfgGraph(cfg);
            foreach (var proc in cfg.Procedures.Values.OrderBy(p => p.Address))
            {
                w.WriteLine();
                w.WriteLine("define {0}", proc.Name);
                var it = new DfsIterator<Block>(g);
                foreach (var block in it.PreOrder(cfg.Blocks[proc.Address]).OrderBy(b => b.Name))
                {
                    DumpBlock(block, g, w);
                }
            }
        }

        [Test]
        public void RecScan_Return()
        {
            Given_EntryPoint(0x001000);
            Given_Trace(new RtlTrace(0x001000)
            {
                m => m.Return(0,0)
            });
            var sExpected =
            #region Expected
@"
define fn00001000
l00001000:
    // pred:
    return (0,0)
    // succ:
";
            #endregion
            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Assignment()
        {
            Given_EntryPoint(0x001000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r2, m.Word32(42)),
                m => m.Assign(r1, r2),
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
@"
define fn00001000
l00001000:
    // pred:
    r2 = 0x2A<32>
    r1 = r2
    return (0,0)
    // succ:
";
            #endregion
            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Branch()
        {
            Given_EntryPoint(0x001000);
            Given_Trace(new RtlTrace(0x1000)
            {  
                m => m.Assign(r2, m.ISub(r2, 1)),
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x1010)),
                m => m.Assign(r1, 0),
                m => m.Return(0, 0)
            });
            Given_Trace(new RtlTrace(0x1010)
            {
                m => m.Assign(r1, 1),
                m => m.Return(0, 0),
            });

            var sExpected =
            #region Expected
@"
define fn00001000
l00001000:
    // pred:
    r2 = r2 - 1<32>
    if (r2 != 0<32>) branch 00001010
    // succ: l00001008 l00001010
l00001008:
    // pred: l00001000
    r1 = 0<32>
    return (0,0)
    // succ:
l00001010:
    // pred: l00001000
    r1 = 1<32>
    return (0,0)
    // succ:
";
            #endregion
            RunTest(sExpected);
        }

        [Test]
        public void RecScan_BackBranch()
        {
            Given_EntryPoint(0x001000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r2, 10),
                m => m.Assign(r2, m.ISub(r2, 1)),
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x1004)),
                m => m.Assign(r1, 0),
                m => m.Return(0, 0),
            });
            Given_Trace(new RtlTrace(0x1004)
            {
                m => m.Assign(r2, m.ISub(r2, 1)),
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x1004)),
                m => m.Return(0, 0),
            });

            var sExpected =
            #region Expected
@"
define fn00001000
l00001000:
    // pred:
    r2 = 0xA<32>
    // succ: l00001004
l00001004:
    // pred: l00001000 l00001004
    r2 = r2 - 1<32>
    if (r2 != 0<32>) branch 00001004
    // succ: l0000100C l00001004
l0000100C:
    // pred: l00001004
    r1 = 0<32>
    return (0,0)
    // succ:
";
            #endregion
            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Call()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, 3),
                m => m.Call(Address.Ptr32(0x1020), 0),
            });
            Given_Trace(new RtlTrace(0x1008)
            {
                m => m.Assign(m.Mem32(m.Word32(0x3000)), r1),
                m => m.Return(0,0)
            });

            Given_Trace(new RtlTrace(0x1020)
            {
                m => m.Assign(r1, m.SMul(r1, r1)),
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    // pred:
    r1 = 3<32>
    call 00001020 (0)
    // succ: l00001008
l00001008:
    // pred: l00001000
    Mem0[0x3000<32>:word32] = r1
    return (0,0)
    // succ:

define fn00001020
l00001020:
    // pred:
    r1 = r1 *s r1
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Goto()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, 0),
                m => m.Assign(r2, 20),
                m => m.Goto(Address.Ptr32(0x1014)),
            });
            Given_Trace(new RtlTrace(0x100C)
            {
                m => m.Assign(r1, r2),
                m => m.Assign(r2, m.ISub(r2, 1)),
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x100C)),
                m => m.Return(0, 0)
            });
            Given_Trace(new RtlTrace(0x1014)
            { 
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x100C)),
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    // pred:
    r1 = 0<32>
    r2 = 0x14<32>
    goto 00001014
    // succ: l00001014
l0000100C:
    // pred: l00001014
    r1 = r2
    r2 = r2 - 1<32>
    // succ: l00001014
l00001014:
    // pred: l00001000 l0000100C
    if (r2 != 0<32>) branch 0000100C
    // succ: l00001018 l0000100C
l00001018:
    // pred: l00001014
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_DelaySlots()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, 0),
                m => m.GotoD(Address.Ptr32(0x1014)),
                m => m.Assign(r2, m.Mem32(m.Word32(0x00123400))),
            });
            Given_Trace(new RtlTrace(0x100C)
            {
                m => m.Assign(r1, r2),
                m => m.Assign(r2, m.ISub(r2, 1)),
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x100C), InstrClass.ConditionalTransfer|InstrClass.Delay ),
                m => m.Nop(),
                m => m.ReturnD(0, 0),
                m => m.Nop()
            });
            Given_Trace(new RtlTrace(0x1014)
            {
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x100C), InstrClass.ConditionalTransfer|InstrClass.Delay ),
                m => m.Nop(),
                m => m.ReturnD(0, 0),
                m => m.Nop()
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    // pred:
    r1 = 0<32>
    r2 = Mem0[0x123400<32>:word32]
    goto 00001014
    // succ: l00001014
l0000100C:
    // pred: l00001014
    r1 = r2
    r2 = r2 - 1<32>
    // succ: l00001014
l00001014:
    // pred: l00001000 l0000100C
    if (r2 != 0<32>) branch 0000100C
    // succ: l0000101C l0000100C
l0000101C:
    // pred: l00001014
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_MultipleBackEdges()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, r2),
                m => m.Assign(r2, m.IAdd(r2, 1)),
                m => m.Assign(r1, m.Mem32(m.Word32(0x00123400))),
                m => m.Branch(m.Eq0(r1), Address.Ptr32(0x1004)),
                m => m.Assign(r2, m.Mem32(r1)),
                m => m.Branch(m.Eq0(r2), Address.Ptr32(0x1008)),
                m => m.Return(0, 0)
            });
            Given_Trace(new RtlTrace(0x1004)
            {
                m => m.Assign(r2, m.IAdd(r2, 1)),
                m => m.Assign(r1, m.Mem32(m.Word32(0x00123400))),
                m => m.Branch(m.Eq0(r1), Address.Ptr32(0x1004))
            });
            Given_Trace(new RtlTrace(0x1008)
            {
                m => m.Assign(r1, m.Mem32(m.Word32(0x00123400))),
                m => m.Branch(m.Eq0(r1), Address.Ptr32(0x1004))
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    // pred:
    r1 = r2
    // succ: l00001004
l00001004:
    // pred: l00001000 l00001008
    r2 = r2 + 1<32>
    // succ: l00001008
l00001008:
    // pred: l00001004 l00001010
    r1 = Mem0[0x123400<32>:word32]
    if (r1 == 0<32>) branch 00001004
    // succ: l00001010 l00001004
l00001010:
    // pred: l00001008
    r2 = Mem0[r1:word32]
    if (r2 == 0<32>) branch 00001008
    // succ: l00001018 l00001008
l00001018:
    // pred: l00001010
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_NestedCalls()
        {
            Given_EntryPoint(0x1000);
            Given_EntryPoint(0x1040);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Call(Address.Ptr32(0x1030), 0),
            });
            Given_Trace(new RtlTrace(0x1004)
            {
                m => m.Return(0,0)
            });
            Given_Trace(new RtlTrace(0x1030)
            {
                m => m.Return(0, 0)
            });
            Given_Trace(new RtlTrace(0x1040)
            {
                m => m.Call(Address.Ptr32(0x1000), 0),
            });
            Given_Trace(new RtlTrace(0x1044)
            {
                m => m.Return(0,0)
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    // pred:
    call 00001030 (0)
    // succ: l00001004
l00001004:
    // pred: l00001000
    return (0,0)
    // succ:

define fn00001030
l00001030:
    // pred:
    return (0,0)
    // succ:

define fn00001040
l00001040:
    // pred:
    call 00001000 (0)
    // succ: l00001044
l00001044:
    // pred: l00001040
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Jump_to_invalid_address()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m=> m.Goto(Address.Ptr32(0x6000))
            });
            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000: // (INVALID)
    // pred:
    <invalid>
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_ConditionalMove()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, m.Mem32(m.Ptr32(0x3000))),
                m =>
                {
                    m.BranchInMiddleOfInstruction(m.Eq0(r1), Address.Ptr32(0x1008), InstrClass.ConditionalTransfer);
                    m.Assign(r2, m.SDiv(r2, r1));
                },
                m => m.Assign(m.Mem32(m.Ptr32(0x4000)), r2),
                m => m.Return(0,0)
            });

            var sExpected =
            #region Expected
@"
define fn00001000
l00001000:
    // pred:
    r1 = Mem0[0x00003000<p32>:word32]
    if (r1 == 0<32>) branch 00001008
    r2 = r2 / r1
    // succ: l00001008
l00001008:
    // pred: l00001000
    Mem0[0x00004000<p32>:word32] = r2
    return (0,0)
    // succ:
";
            #endregion
            RunTest(sExpected);
        }

        [Test]
        public void RecScan_JumpIntoOtherInstruction()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                { 3, m => m.Assign(r1, 4) },
                { 3, m => m.Assign(m.Mem32(r2), r1) },
                { 4, m => m.Branch(m.Ne0(m.Mem32(m.IAdd(r2, 4))), Address.Ptr32(0x1002)) },
                { 4, m => m.Return(0, 0) }
            });
            Given_Trace(new RtlTrace(0x1002)
            {
                { 4, m => m.Assign(m.Mem32(r2), m.ISub(r1, 1)) },
                { 4, m => m.Branch(m.Ne0(m.Mem32(m.IAdd(r2, 4))), Address.Ptr32(0x1002)) },
                { 4, m => m.Return(0, 0) }
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    // pred:
    r1 = 4<32>
    Mem0[r2:word32] = r1
    // succ: l00001006
l00001002:
    // pred: l00001006
    Mem0[r2:word32] = r1 - 1<32>
    // succ: l00001006
l00001006:
    // pred: l00001000 l00001002
    if (Mem0[r2 + 4<32>:word32] != 0<32>) branch 00001002
    // succ: l0000100A l00001002
l0000100A:
    // pred: l00001006
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }
    }
}

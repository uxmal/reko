#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Scanning;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Scanning
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
            var dynamicLinker = new Mock<IDynamicLinker>().Object;
            var listener = new Mock<IDecompilerEventListener>().Object;
            var scanner = new RecursiveScanner(program, ProvenanceType.Scanning, dynamicLinker, listener, new ServiceContainer());
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

        private void DumpCfg(ScanResultsV2 cfg, TextWriter w)
        {
            var g = new CfgGraph(cfg);
            foreach (var proc in cfg.Procedures.Values.OrderBy(p => p.Address))
            {
                w.WriteLine();
                w.Write("define {0}", proc.Name);
                if (cfg.TrampolineStubStarts.TryGetValue(proc.Address, out var trampoline))
                {
                    w.Write(" stub to '{0}'", trampoline.Procedure.Name);
                }
                w.WriteLine();
                w.WriteLine("    provenance: {0}", proc.Provenance);
                var it = new DfsIterator<RtlBlock>(g);
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
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
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
    provenance: ImageEntrypoint
l00001000: // l:12; ft:0000100C
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
    provenance: ImageEntrypoint
l00001000: // l:8; ft:00001008
    // pred:
    r2 = r2 - 1<32>
    if (r2 != 0<32>) branch 00001010
    // succ: l00001008 l00001010
l00001008: // l:8; ft:00001010
    // pred: l00001000
    r1 = 0<32>
    return (0,0)
    // succ:
l00001010: // l:8; ft:00001018
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
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    r2 = 0xA<32>
    // succ: l00001004
l00001004: // l:8; ft:0000100C
    // pred: l00001000 l00001004
    r2 = r2 - 1<32>
    if (r2 != 0<32>) branch 00001004
    // succ: l0000100C l00001004
l0000100C: // l:8; ft:00001014
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
    provenance: ImageEntrypoint
l00001000: // l:8; ft:00001008
    // pred:
    r1 = 3<32>
    call 00001020 (0)
    // succ: l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001000
    Mem0[0x3000<32>:word32] = r1
    return (0,0)
    // succ:

define fn00001020
    provenance: Scanning
l00001020: // l:8; ft:00001028
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
    provenance: ImageEntrypoint
l00001000: // l:12; ft:0000100C
    // pred:
    r1 = 0<32>
    r2 = 0x14<32>
    goto 00001014
    // succ: l00001014
l0000100C: // l:8; ft:00001014
    // pred: l00001014
    r1 = r2
    r2 = r2 - 1<32>
    // succ: l00001014
l00001014: // l:4; ft:00001018
    // pred: l00001000 l0000100C
    if (r2 != 0<32>) branch 0000100C
    // succ: l00001018 l0000100C
l00001018: // l:4; ft:0000101C
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
    provenance: ImageEntrypoint
l00001000: // l:8; ft:0000100C
    // pred:
    r1 = 0<32>
    r2 = Mem0[0x123400<32>:word32]
    goto 00001014
    // succ: l00001014
l0000100C: // l:8; ft:00001014
    // pred: l00001014
    r1 = r2
    r2 = r2 - 1<32>
    // succ: l00001014
l00001014: // l:4; ft:0000101C
    // pred: l00001000 l0000100C
    if (r2 != 0<32>) branch 0000100C
    // succ: l0000101C l0000100C
l0000101C: // l:4; ft:00001024
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
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    r1 = r2
    // succ: l00001004
l00001004: // l:4; ft:00001008
    // pred: l00001000 l00001008
    r2 = r2 + 1<32>
    // succ: l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001004 l00001010
    r1 = Mem0[0x123400<32>:word32]
    if (r1 == 0<32>) branch 00001004
    // succ: l00001010 l00001004
l00001010: // l:8; ft:00001018
    // pred: l00001008
    r2 = Mem0[r1:word32]
    if (r2 == 0<32>) branch 00001008
    // succ: l00001018 l00001008
l00001018: // l:4; ft:0000101C
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
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    call 00001030 (0)
    // succ: l00001004
l00001004: // l:4; ft:00001008
    // pred: l00001000
    return (0,0)
    // succ:

define fn00001030
    provenance: Scanning
l00001030: // l:4; ft:00001034
    // pred:
    return (0,0)
    // succ:

define fn00001040
    provenance: ImageEntrypoint
l00001040: // l:4; ft:00001044
    // pred:
    call 00001000 (0)
    // succ: l00001044
l00001044: // l:4; ft:00001048
    // pred: l00001040
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Jump_to_non_executable_address()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m=> m.Goto(Address.Ptr32(0x3000))
            });
            var sExpected =
            #region Expected
                @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004 (INVALID)
    // pred:
    <invalid>
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test(Description = "Jumps to invalid addresses generate stub calls.")]
        public void RecScan_JumpToExternalAddress()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Goto(Address.Ptr32(0x4711_4711))
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    call fn47114711 (0)
    return (0,0)
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
            Given_Trace(new RtlTrace(0x1008)
            {
                m => m.Assign(m.Mem32(m.Ptr32(0x4000)), r2),
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
@"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:8; ft:00001008
    // pred:
    r1 = Mem0[0x00003000<p32>:word32]
    if (r1 == 0<32>) branch 00001008
    r2 = r2 / r1
    // succ: l00001008 l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001000 l00001000
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
                { 4, m => m.Branch(m.Ne0(m.Mem32(r2, 4)), Address.Ptr32(0x1002)) },
                { 4, m => m.Return(0, 0) }
            });
            Given_Trace(new RtlTrace(0x1002)
            {
                { 4, m => m.Assign(m.Mem32(r2), m.ISub(r1, 1)) },
                { 4, m => m.Branch(m.Ne0(m.Mem32(r2, 4)), Address.Ptr32(0x1002)) },
                { 4, m => m.Return(0, 0) }
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:6; ft:00001006
    // pred:
    r1 = 4<32>
    Mem0[r2:word32] = r1
    // succ: l00001006
l00001002: // l:4; ft:00001006
    // pred: l00001006
    Mem0[r2:word32] = r1 - 1<32>
    // succ: l00001006
l00001006: // l:4; ft:0000100A
    // pred: l00001000 l00001002
    if (Mem0[r2 + 4<32>:word32] != 0<32>) branch 00001002
    // succ: l0000100A l00001002
l0000100A: // l:4; ft:0000100E
    // pred: l00001006
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Switch()
        {
            Given_EntryPoint(0x1000);
            Given_JumpTable(0x1020,
                0x1030, 0x1038, 0x1030, 0x1038);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(C, m.Cond(m.ISub(r1, 3))),
                m => m.Branch(m.Test(ConditionCode.UGT, C), Address.Ptr32(0x1040)),
                m => m.Goto(m.Mem32(m.IAdd(m.IMul(r1, 4), 0x1020)))
            });
            Given_Trace(new RtlTrace(0x1030)
            {
                m => m.Assign(r2, 1),
                m => m.Goto(Address.Ptr32(0x1040))
            });
            Given_Trace(new RtlTrace(0x1038)
            {
                m => m.Assign(r2, 1),
                m => m.Assign(r1, -1),
                m => m.Return(0, 0)
            });
            Given_Trace(new RtlTrace(0x1040)
            {
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:8; ft:00001008
    // pred:
    C = cond(r1 - 3<32>)
    if (Test(UGT,C)) branch 00001040
    // succ: l00001008 l00001040
l00001008: // l:4; ft:0000100C
    // pred: l00001000
    switch (r1) {
        00001030,
        00001038,
        00001030,
        00001038
    }
    // succ: l00001030 l00001038 l00001030 l00001038
l00001030: // l:8; ft:00001038
    // pred: l00001008 l00001008
    r2 = 1<32>
    goto 00001040
    // succ: l00001040
l00001038: // l:8; ft:00001040
    // pred: l00001008 l00001008
    r2 = 1<32>
    r1 = 0xFFFFFFFF<32>
    // succ: l00001040
l00001040: // l:4; ft:00001044
    // pred: l00001000 l00001030 l00001038
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Padding()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r2, 1),
                m => m.Nop(),
                m => m.Nop(),
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    r2 = 1<32>
    // succ: l00001004
l00001004: // l:8; ft:0000100C
    // pred: l00001000
    nop
    nop
    // succ: l0000100C
l0000100C: // l:4; ft:00001010
    // pred: l00001004
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Padding_DelaySlot()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r2, 1),
                m => m.Nop(),
                m => m.Nop(),
                m => m.ReturnD(0, 0),
                m => m.Nop(),
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    r2 = 1<32>
    // succ: l00001004
l00001004: // l:8; ft:0000100C
    // pred: l00001000
    nop
    nop
    // succ: l0000100C
l0000100C: // l:4; ft:00001014
    // pred: l00001004
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Call_DelaySlot()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r2, m.Mem32(m.Word32(0x00123400))),
                m => m.CallD(r2, 0),
                m => m.Assign(r2, 42)
            });

            var sExpected =
            #region Expected
@"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:8; ft:0000100C
    // pred:
    r2 = Mem0[0x123400<32>:word32]
    v0 = r2
    r2 = 0x2A<32>
    call v0 (0)
    // succ: l0000100C
l0000100C: // l:1; ft:0000100D (INVALID)
    // pred: l00001000
    <invalid>
    // succ:
";
            #endregion
            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Call_Trampoline()
        {
            Given_EntryPoint(0x1000);
            Given_TrampolineAt(0x1020, 0x1020, "malloc");
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r2, m.Mem32(m.Word32(0x00123400))),
                m => m.Call(Address.Ptr32(0x1020), 0),
            });
            Given_Trace(new RtlTrace(0x1008)
            {
                m => m.Return(0, 0)
            });
            Given_Trace(new RtlTrace(0x1020)
            {
                m => m.Goto(m.Mem32(m.Word32(0x3000)))
            });

            var expected =
            #region Expected
@"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:8; ft:00001008
    // pred:
    r2 = Mem0[0x123400<32>:word32]
    call 00001020 (0)
    // succ: l00001008
l00001008: // l:4; ft:0000100C
    // pred: l00001000
    return (0,0)
    // succ:

define fn00001020 stub to 'malloc'
    provenance: Scanning
l00001020: // l:4; ft:00001024
    // pred:
    goto Mem0[0x3000<32>:word32]
    // succ:
";
            #endregion

            RunTest(expected);
        }

        [Test]
        public void RecScan_TwoCalls_SameProc()
        {
            Given_EntryPoint(0x1000);
            Given_TrampolineAt(0x1020, 0x1020, "malloc");
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r2, m.Mem32(m.Word32(0x00123400))),
                m => m.Call(Address.Ptr32(0x1020), 0),
            });
            Given_Trace(new RtlTrace(0x1008)
            {
                m => m.Assign(r2, 0x42),
                m => m.Call(Address.Ptr32(0x1020), 0),
            });
            Given_Trace(new RtlTrace(0x1010)
            {
                m => m.Return(0, 0)
            });
            Given_Trace(new RtlTrace(0x1020)
            {
                m => m.Goto(m.Mem32(m.Word32(0x3000)))
            });

            var sExpected =
            #region Expected
@"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:8; ft:00001008
    // pred:
    r2 = Mem0[0x123400<32>:word32]
    call 00001020 (0)
    // succ: l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001000
    r2 = 0x42<32>
    call 00001020 (0)
    // succ: l00001010
l00001010: // l:4; ft:00001014
    // pred: l00001008
    return (0,0)
    // succ:

define fn00001020 stub to 'malloc'
    provenance: Scanning
l00001020: // l:4; ft:00001024
    // pred:
    goto Mem0[0x3000<32>:word32]
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_ImageSymbol()
        {
            Given_EntryPoint(0x1000);
            Given_ImageSymbol_Proc(0x1100, "bob");
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Return(0, 0)
            });
            Given_Trace(new RtlTrace(0x1100)
            {
                m => m.Assign(r2, 0),
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
@"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    return (0,0)
    // succ:

define bob
    provenance: Image
l00001100: // l:8; ft:00001108
    // pred:
    r2 = 0<32>
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Unlikely()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000, InstrClass.Unlikely)
            {
                m => m.Assign(r2, m.IAdd(r2, r2)),
                m => m.Return(0, 0)
            });
            program.User.Heuristics.Add(ScannerHeuristics.Unlikely);

            var sExpected =
            #region Expected
@"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004 (INVALID)
    // pred:
    <invalid>
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_UserProcedure_Overlapping_EntryPoint()
        {
            Given_EntryPoint(0x1000);
            Given_UserProcedure(0x1000, "main", "int main(int c, char **v)");
            Given_Trace(new RtlTrace(0x1000, InstrClass.Linear)
            {
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
                @"
define main
    provenance: UserInput
l00001000: // l:4; ft:00001004
    // pred:
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Endless_procedure_cycle()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Call(Address.Ptr32(0x1040), 0),
            });
            Given_Trace(new RtlTrace(0x1004)
            {
                m => m.Return(0, 0)
            });

            Given_Trace(new RtlTrace(0x1040)
            {
                m => m.Call(Address.Ptr32(0x1050), 0)
            });
            Given_Trace(new RtlTrace(0x1044)
            { 
                m => m.Return(0, 0)
            });

            Given_Trace(new RtlTrace(0x1050)
            {
                m => m.Call(Address.Ptr32(0x1040), 0)
            });
            Given_Trace(new RtlTrace(0x1054)
            { 
                m => m.Return(0, 0)
            });

            var sExpected =
@"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    call 00001040 (0)
    // succ:

define fn00001040
    provenance: Scanning
l00001040: // l:4; ft:00001044
    // pred:
    call 00001050 (0)
    // succ:

define fn00001050
    provenance: Scanning
l00001050: // l:4; ft:00001054
    // pred:
    call 00001040 (0)
    // succ:
";
            RunTest(sExpected);
        }

        [Test(Description = "Handle sub-instruction cfg resulting from 'scasxx' instructions")]
        public void RecScan_scasw()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, m.Mem32(m.Word32(0x00123000))),
                m =>
                {
                    m.BranchInMiddleOfInstruction(m.Eq0(r1), Address.Ptr32(0x1008), InstrClass.ConditionalTransfer);
                    m.BranchInMiddleOfInstruction(m.Eq(r2, m.Mem32(r1)), Address.Ptr32(0x1008), InstrClass.ConditionalTransfer);
                    m.Assign(r1, m.ISub(r1, 4));
                    m.Goto(Address.Ptr32(0x1004));
                }
            });
            Given_Trace(new RtlTrace(0x1004)
            {
                 m =>
                {
                    m.BranchInMiddleOfInstruction(m.Eq0(r1), Address.Ptr32(0x1008), InstrClass.ConditionalTransfer);
                    m.BranchInMiddleOfInstruction(m.Eq(r2, m.Mem32(r1)), Address.Ptr32(0x1008), InstrClass.ConditionalTransfer);
                    m.Assign(r1, m.ISub(r1, 4));
                    m.Goto(Address.Ptr32(0x1004));
                }
            });
            Given_Trace(new RtlTrace(0x1008)
            {
                m => m.Assign(m.Mem32(m.Word32(0x00123400)), r1),
                m => m.Return(0, 0)
            });

            var sExpected = @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:4; ft:00001004
    // pred:
    r1 = Mem0[0x123000<32>:word32]
    // succ: l00001004
l00001004: // l:4; ft:00001008
    // pred: l00001000 l00001004
    if (r1 == 0<32>) branch 00001008
    if (r2 == Mem0[r1:word32]) branch 00001008
    r1 = r1 - 4<32>
    goto 00001004
    // succ: l00001004 l00001008 l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001004 l00001004
    Mem0[0x123400<32>:word32] = r1
    return (0,0)
    // succ:
";

            RunTest(sExpected);
        }

        [Test(Description = "Handle Z80- and ARM-style conditional calls")]
        public void RecScan_ConditionalCall()
        {
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r2, m.Mem32(m.Word32(0x00123000))),
                m =>
                {
                    m.BranchInMiddleOfInstruction(m.Eq0(r1), Address.Ptr32(0x1008), InstrClass.ConditionalTransfer);
                    m.Call(Address.Ptr32(0x1010), 0);
                }
            });
            Given_Trace(new RtlTrace(0x1008)
            {
                m => m.Assign(m.Mem32(m.Word32(0x00123400)), r1),
                m => m.Return(0, 0)
            });

            Given_Trace(new RtlTrace(0x1010)
            {
                m => m.Return(0, 0)
            });

            var sExpected = @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:8; ft:00001008
    // pred:
    r2 = Mem0[0x123000<32>:word32]
    if (r1 == 0<32>) branch 00001008
    call 00001010 (0)
    // succ: l00001008 l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001000 l00001000
    Mem0[0x123400<32>:word32] = r1
    return (0,0)
    // succ:

define fn00001010
    provenance: Scanning
l00001010: // l:4; ft:00001014
    // pred:
    return (0,0)
    // succ:
";

            RunTest(sExpected);
        }

        [Test]
        public void RecScan_NestedTailCalls()
        {
            // Based on the following fragment:
            // 9810 8A C6    mov al, dh
            // 9812 E8 02 00 call 9817h
            // 9815 8A C2    mov al, dl

            // 9817 D4 10    aam 10h
            // 9819 86 E0    xchg al,ah
            // 981B E8 02 00 call 9820h
            // 981E 86 E0    xchg al,ah

            // 9820 04 90    add al,90h
            // 9822 27 daa
            // 9823 14 40    adc al,40h
            // 9825 27       daa
            // 9826 AA       stosb
            // 9827 C3       ret
            Given_EntryPoint(0x1000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, r2),
                m => m.Call(Address.Ptr32(0x1008),0)
            });
            Given_Trace(new RtlTrace(0x1008)
            {
                m => m.Assign(r2, r1),
                m => m.Call(Address.Ptr32(0x1010), 0)
            });
            Given_Trace(new RtlTrace(0x1010)
            {
                m => m.Assign(r2, m.IAdd(r1, r2)),
                m => m.Assign(r2, m.IAdd(r2, 0x40)),
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
    provenance: ImageEntrypoint
l00001000: // l:8; ft:00001008
    // pred:
    r1 = r2
    call 00001008 (0)
    // succ: l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001000
    r2 = r1
    call 00001010 (0)
    // succ: l00001010
l00001010: // l:12; ft:0000101C
    // pred: l00001008
    r2 = r1 + r2
    r2 = r2 + 0x40<32>
    return (0,0)
    // succ:

define fn00001008
    provenance: Scanning
l00001008: // l:8; ft:00001010
    // pred: l00001000
    r2 = r1
    call 00001010 (0)
    // succ: l00001010
l00001010: // l:12; ft:0000101C
    // pred: l00001008
    r2 = r1 + r2
    r2 = r2 + 0x40<32>
    return (0,0)
    // succ:

define fn00001010
    provenance: Scanning
l00001010: // l:12; ft:0000101C
    // pred: l00001008
    r2 = r1 + r2
    r2 = r2 + 0x40<32>
    return (0,0)
    // succ:
";
            #endregion
            RunTest(sExpected);
        }
    }
}

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2.UnitTests
{
    [TestFixture]
    public class RecursiveScannerTests
    {
        private Mock<IProcessorArchitecture> arch = default!;
        private Program program = default!;
        private Identifier r1 = Identifier.Create(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
        private Identifier r2 = Identifier.Create(new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));

        [SetUp]
        public void Setup()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("FakeCpu");
            arch.Setup(a => a.CreateProcessorState())
                .Returns(new Func<ProcessorState>(() => new FakeProcessorState(arch.Object)));
            arch.Setup(a => a.CreateImageReader(
                It.IsNotNull<MemoryArea>(),
                It.IsNotNull<Address>()))
                .Returns(new Func<MemoryArea, Address, EndianImageReader>((mm, aa) =>
                    mm.CreateLeReader(aa)));

            var segmentMap = new SegmentMap(
                new ImageSegment(
                    ".text",
                    new ByteMemoryArea(Address.Ptr32(0x1000), new byte[4096]),
                    AccessMode.ReadExecute),
                new ImageSegment(
                    ".data",
                    new ByteMemoryArea(Address.Ptr32(0x3000), new byte[4096]),
                    AccessMode.ReadWrite));

            this.program = new Program
            {
                SegmentMap = segmentMap,
                Architecture = arch.Object,
            };
        }

        private void Given_EntryPoint(uint uAddr)
        {
            var addr = Address.Ptr32(uAddr);
            var sym = ImageSymbol.Procedure(arch.Object, addr);
            this.program.EntryPoints.Add(addr, sym);
        }

        private void Given_Trace(RtlTrace trace)
        {
            arch.Setup(a => a.CreateRewriter(
                It.Is<EndianImageReader>(r => r.Address == trace.StartAddress),
                It.IsNotNull<ProcessorState>(),
                It.IsNotNull<IStorageBinder>(),
                It.IsNotNull<IRewriterHost>()))
                .Returns(trace);
        }

        private void RunTest(string sExpected)
        {
            var scanner = new RecursiveScanner(program);
            var cfg = scanner.ScanProgram();
            var sw = new StringWriter();
            DumpCfg(cfg, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        private class CfgGraph : DirectedGraph<Block>
        {
            private Cfg cfg;

            public CfgGraph(Cfg cfg)
            {
                this.cfg = cfg;
            }

            public ICollection<Block> Nodes => cfg.Blocks.Values;

            public void AddEdge(Block nodeFrom, Block nodeTo)
            {
                throw new NotImplementedException();
            }

            public bool ContainsEdge(Block nodeFrom, Block nodeTo)
            {
                throw new NotImplementedException();
            }

            public ICollection<Block> Predecessors(Block node)
            {
                throw new NotImplementedException();
            }

            public void RemoveEdge(Block nodeFrom, Block nodeTo)
            {
                throw new NotImplementedException();
            }

            public ICollection<Block> Successors(Block node)
            {
                return cfg.Successors.TryGetValue(node.Address, out var edges)
                    ? edges
                        .Select(e => Get(e))
                        .ToArray()
                    : Array.Empty<Block>();
            }

            private Block Get(Edge e)
            {
                var b = cfg.Blocks[e.To];
                return b;
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
                foreach(var block in it.PreOrder(cfg.Blocks[proc.Address]).OrderBy(b => b.Id))
                {
                    w.WriteLine("{0}:", block.Id);
                    foreach (var (_, instr) in block.Instructions)
                    {
                        w.WriteLine("    {0}", instr);
                    }
                    w.Write("    succ:");
                    foreach (var s in g.Successors(block))
                    {
                        w.Write(" {0}", s.Id);
                    }
                    w.WriteLine();
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
    return (0,0)
    succ:
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
    r2 = 0x2A<32>
    r1 = r2
    return (0,0)
    succ:
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
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x1010))
            });
            Given_Trace(new RtlTrace(0x1008)
            {
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
    r2 = r2 - 1<32>
    if (r2 != 0<32>) branch 00001010
    succ: l00001008 l00001010
l00001008:
    r1 = 0<32>
    return (0,0)
    succ:
l00001010:
    r1 = 1<32>
    return (0,0)
    succ:
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
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x1004))
            });
            Given_Trace(new RtlTrace(0x1004)
            {
                m => m.Assign(r2, m.ISub(r2, 1)),
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x1004))
            });
            Given_Trace(new RtlTrace(0x100C)
            { 
                m => m.Assign(r1, 0),
                m => m.Return(0, 0),
            });

            var sExpected =
            #region Expected
@"
define fn00001000
l00001000:
    r2 = 0xA<32>
    succ: l00001004
l00001004:
    r2 = r2 - 1<32>
    if (r2 != 0<32>) branch 00001004
    succ: l0000100C l00001004
l0000100C:
    r1 = 0<32>
    return (0,0)
    succ:
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
    r1 = 3<32>
    call 00001020 (0)
    succ: l00001008
l00001008:
    Mem0[0x3000<32>:word32] = r1
    return (0,0)
    succ:

define fn00001020
l00001020:
    r1 = r1 *s r1
    return (0,0)
    succ:
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
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x100C))
            });
            Given_Trace(new RtlTrace(0x1014)
            { 
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x100C))
            });
            Given_Trace(new RtlTrace(0x1018)
            {
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    r1 = 0<32>
    r2 = 0x14<32>
    goto 00001014
    succ: l00001014
l0000100C:
    r1 = r2
    r2 = r2 - 1<32>
    succ: l00001014
l00001014:
    if (r2 != 0<32>) branch 0000100C
    succ: l00001018 l0000100C
l00001018:
    return (0,0)
    succ:
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
                m => m.Nop()
            });
            Given_Trace(new RtlTrace(0x1014)
            {
                m => m.Branch(m.Ne0(r2), Address.Ptr32(0x100C), InstrClass.ConditionalTransfer|InstrClass.Delay ),
                m => m.Nop()
            });
            Given_Trace(new RtlTrace(0x101C)
            {
                m => m.ReturnD(0, 0),
                m => m.Nop()
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    r1 = 0<32>
    r2 = Mem0[0x123400<32>:word32]
    goto 00001014
    succ: l00001014
l0000100C:
    r1 = r2
    r2 = r2 - 1<32>
    succ: l00001014
l00001014:
    if (r2 != 0<32>) branch 0000100C
    succ: l0000101C l0000100C
l0000101C:
    return (0,0)
    succ:
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
                m => m.Branch(m.Eq0(r1), Address.Ptr32(0x1004))
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
            Given_Trace(new RtlTrace(0x1010)
            {
                m => m.Assign(r2, m.Mem32(r1)),
                m => m.Branch(m.Eq0(r2), Address.Ptr32(0x1008))
            });
            Given_Trace(new RtlTrace(0x1018)
            {
                m => m.Return(0, 0)
            });

            var sExpected =
            #region Expected
                @"
define fn00001000
l00001000:
    r1 = r2
    succ: l00001004
l00001004:
    r2 = r2 + 1<32>
    succ: l00001008
l00001008:
    r1 = Mem0[0x123400<32>:word32]
    if (r1 == 0<32>) branch 00001004
    succ: l00001010 l00001004
l00001010:
    r2 = Mem0[r1:word32]
    if (r2 == 0<32>) branch 00001008
    succ: l00001018 l00001008
l00001018:
    return (0,0)
    succ:
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
    call 00001030 (0)
    succ: l00001004
l00001004:
    return (0,0)
    succ:

define fn00001030
l00001030:
    return (0,0)
    succ:

define fn00001040
l00001040:
    call 00001000 (0)
    succ: l00001044
l00001044:
    return (0,0)
    succ:
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
l00001000:
    <invalid>
    succ:
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
    r1 = Mem0[0x00003000<p32>:word32]
    if (r1 == 0<32>) branch 00001008
    r2 = r2 / r1
    succ: l00001008
l00001008:
    Mem0[0x00004000<p32>:word32] = r2
    return (0,0)
    succ:
";
            #endregion
            RunTest(sExpected);
        }
    }
}

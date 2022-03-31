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
        private Dictionary<Address, RtlTrace> traces = default!;
        private Mock<IProcessorArchitecture> arch = default!;
        private Program program = default!;
        private Identifier r1 = Identifier.Create(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
        private Identifier r2 = Identifier.Create(new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));

        [SetUp]
        public void Setup()
        {
            this.traces = new Dictionary<Address, RtlTrace>();
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

        private void Given_Trace(uint uAddr, Action<RtlEmitter> action)
        {
            RtlTrace trace = new RtlTrace(uAddr) { action };
            traces.Add(trace.StartAddress, trace);
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
                return cfg.Edges.TryGetValue(node.Address, out var edges)
                    ? edges
                        .Select(e => cfg.Blocks[e.To])
                        .ToArray()
                    : Array.Empty<Block>();
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
                foreach(var block in it.PreOrder().OrderBy(b => b.Id))
                {
                    w.WriteLine("{0}:", block.Id);
                    foreach (var (_, instr) in block.Instructions)
                    {
                        w.WriteLine("    {0}", instr);
                    }
                    w.Write("    succ:");
                    foreach (var s in g.Successors(block))
                    {
                        w.Write(" {0}", block.Id);
                    }
                    w.WriteLine();
                }
            }
        }

        [Test]
        public void RecScan_Return()
        {
            Given_EntryPoint(0x001000);
            Given_Trace(0x001000, m => m.Return(0,0));

            var sExpected =
            #region Expected
@"
define fn00001000
l00001000:
    return
    succ:
";
            #endregion
            RunTest(sExpected);
        }

        [Test]
        public void RecScan_Assignment()
        {
            Given_EntryPoint(0x001000);
            Given_Trace(0x1000, m =>
            {
                m.Assign(r2, m.Word32(42));
                m.Assign(r1, r2);
                m.Return(0, 0);
            });

            var sExpected =
            #region Expected
@"
define fn00001000
l00001000:
    r2 = 0x2A<32>
    r1 = r2
    return
    succ:
";
            #endregion
            RunTest(sExpected);
        }

    }
}

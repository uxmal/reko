using Moq;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.ScannerV2.UnitTests
{
    public class AbstractScannerTests
    {
        protected Mock<IProcessorArchitecture> arch = default!;
        protected Program program = default!;
        protected Identifier r1 = Identifier.Create(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
        protected Identifier r2 = Identifier.Create(new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));

        protected void Setup(int textSize, int instrBitSize)
        {
            this.arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("FakeCpu");
            arch.Setup(a => a.InstructionBitSize).Returns(instrBitSize);
            arch.Setup(a => a.MemoryGranularity).Returns(8);
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
                    new ByteMemoryArea(Address.Ptr32(0x1000), new byte[textSize]),
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

        protected void DumpBlock(Block block, CfgGraph g, TextWriter w)
        {
            w.WriteLine("{0}:{1}", block.Name, block.IsValid ? "" : " // (INVALID)");
            w.Write("    // pred:");
            foreach (var s in g.Predecessors(block))
            {
                w.Write(" {0}", s.Name);
            }
            w.WriteLine();

            foreach (var cluster in block.Instructions)
            {
                foreach (var instr in cluster.Instructions)
                {
                    w.WriteLine("    {0}", instr);
                }
            }
            w.Write("    // succ:");
            foreach (var s in g.Successors(block))
            {
                w.Write(" {0}", s.Name);
            }
            w.WriteLine();
        }


        protected void Given_EntryPoint(uint uAddr)
        {
            var addr = Address.Ptr32(uAddr);
            var sym = ImageSymbol.Procedure(arch.Object, addr);
            this.program.EntryPoints.Add(addr, sym);
        }

        protected void Given_Trace(RtlTrace trace)
        {
            arch.Setup(a => a.CreateRewriter(
                It.Is<EndianImageReader>(r => r.Address == trace.StartAddress),
                It.IsNotNull<ProcessorState>(),
                It.IsNotNull<IStorageBinder>(),
                It.IsNotNull<IRewriterHost>()))
                .Returns(trace);
        }

        protected class CfgGraph : DirectedGraph<Block>
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
                return cfg.Predecessors.TryGetValue(node.Address, out var edges)
                    ? edges
                        .Select(e => cfg.Blocks[e.From])
                        .ToArray()
                    : Array.Empty<Block>();
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
    }
}
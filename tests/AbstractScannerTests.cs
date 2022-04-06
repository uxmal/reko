using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RtlBlock = Reko.Scanning.RtlBlock;

namespace Reko.ScannerV2.UnitTests
{
    public class AbstractScannerTests
    {
        protected Mock<IProcessorArchitecture> arch = default!;
        protected Program program = default!;
        protected Identifier r1;
        protected Identifier r2;
        protected RegisterStorage sr;
        protected Identifier C;

        public AbstractScannerTests()
        {
            this.r1 = Identifier.Create(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
            this.r2 = Identifier.Create(new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));
            this.sr = new RegisterStorage("SR", 42, 0, PrimitiveType.Word32);
            this.C = Identifier.Create(new FlagGroupStorage(sr, 1, "C", PrimitiveType.Bool));
        }

        protected void Setup(int textSize, int instrBitSize)
        {
            this.arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("FakeCpu");
            arch.Setup(a => a.InstructionBitSize).Returns(instrBitSize);
            arch.Setup(a => a.MemoryGranularity).Returns(8);
            arch.Setup(a => a.Endianness).Returns(EndianServices.Little);
            arch.Setup(a => a.PointerType).Returns(PrimitiveType.Ptr32);
            arch.Setup(a => a.CreateProcessorState())
                .Returns(new Func<ProcessorState>(() => new FakeProcessorState(arch.Object)));
            arch.Setup(a => a.CreateImageReader(
                It.IsNotNull<MemoryArea>(),
                It.IsNotNull<Address>()))
                .Returns(new Func<MemoryArea, Address, EndianImageReader>((mm, aa) =>
                    mm.CreateLeReader(aa)));
            arch.Setup(a => a.MakeAddressFromConstant(
                It.IsNotNull<Constant>(),
                It.IsAny<bool>()))
                .Returns(new Func<Constant, bool, Address>((c, a) =>
                    Address.Ptr32(c.ToUInt32())));

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

        protected void DumpBlock(RtlBlock block, CfgGraph g, TextWriter w)
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

        protected void Given_JumpTable(uint uAddrTable, params uint[] entries)
        {
            var addrTable = Address.Ptr32(uAddrTable);
            if (!program.SegmentMap.TryFindSegment(addrTable, out var segment))
                Assert.Fail($"No segment available for {uAddrTable:X8}");
            var mem = segment.MemoryArea;
            var writer = program.Architecture.Endianness.CreateImageWriter(mem, addrTable);
            foreach (var uEntry in entries)
            {
                writer.WriteUInt32(uEntry);
            }
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

        protected class CfgGraph : DirectedGraph<RtlBlock>
        {
            private Cfg cfg;

            public CfgGraph(Cfg cfg)
            {
                this.cfg = cfg;
            }

            public ICollection<RtlBlock> Nodes => cfg.Blocks.Values;

            public void AddEdge(RtlBlock nodeFrom, RtlBlock nodeTo)
            {
                throw new NotImplementedException();
            }

            public bool ContainsEdge(RtlBlock nodeFrom, RtlBlock nodeTo)
            {
                throw new NotImplementedException();
            }

            public ICollection<RtlBlock> Predecessors(RtlBlock node)
            {
                return cfg.Predecessors.TryGetValue(node.Address, out var edges)
                    ? edges
                        .Select(e => cfg.Blocks[e.From])
                        .ToArray()
                    : Array.Empty<RtlBlock>();
            }

            public void RemoveEdge(RtlBlock nodeFrom, RtlBlock nodeTo)
            {
                throw new NotImplementedException();
            }

            public ICollection<RtlBlock> Successors(RtlBlock node)
            {
                return cfg.Successors.TryGetValue(node.Address, out var edges)
                    ? edges
                        .Select(e => Get(e))
                        .ToArray()
                    : Array.Empty<RtlBlock>();
            }

            private RtlBlock Get(Edge e)
            {
                var b = cfg.Blocks[e.To];
                return b;
            }
        }
    }
}
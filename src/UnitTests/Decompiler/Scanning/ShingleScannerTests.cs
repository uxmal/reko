#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class ShingleScannerTests : AbstractScannerTests
    {
        private static string nl = Environment.NewLine;

        private ScanResultsV2 cfg = default!;
        private ServiceContainer sc = default!;
        private ShingleScanner scanner = default!;
        private Dictionary<uint, RtlInstructionCluster> clusters = default!;
        private RelocationDictionary rd;
        private readonly Identifier id;
        private readonly IDecompilerEventListener listener;

        public ShingleScannerTests()
        {
            this.id = Identifier.Create(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
            this.listener = new FakeDecompilerEventListener();
        }

        [SetUp]
        public void Setup()
        {
            base.Setup(0x10, 16); // text segment of 0x10 bytes, with 16-bit instruction granularity.
            this.cfg = new ScanResultsV2();
            this.sc = new ServiceContainer();
            this.clusters = new Dictionary<uint, RtlInstructionCluster>();
        }

        private void Given_Block(uint uAddr, int length)
        {
            var addr = Address.Ptr32(uAddr);
            var addrFallThrough = addr + length;
            var id = $"l{addr:X4}";
            var instrs = new List<RtlInstructionCluster>
            {
                new RtlInstructionCluster(addr, length,
                    new RtlAssignment(r2, r1))
            };
            var block = new RtlBlock(program.Architecture, addr, id, length, addrFallThrough, ProvenanceType.None, instrs);
            cfg.Blocks.TryAdd(addr, block);
        }

        private void Given_RecursiveProc(uint uAddr)
        {
            var proc = new Proc(
                Address.Ptr32(uAddr),
                ProvenanceType.Scanning,
                this.program.Architecture,
                $"fn{uAddr:X4}");
            var added = cfg.Procedures.TryAdd(Address.Ptr32(uAddr), proc);
            Assert.IsTrue(added);
        }


        private List<ChunkWorker> When_MakeScanChunks()
        {
            var dynLinker = new Mock<IDynamicLinker>();
            var listener = new Mock<IDecompilerEventListener>();
            var scanner = new ShingleScanner(program, cfg, dynLinker.Object, listener.Object, new ServiceContainer());
            var gaps = scanner.FindUnscannedExecutableGaps();
            var chunks = gaps.Select(g => scanner.MakeChunkWorker(g.Item1, g.Item2));
            return chunks.ToList();
        }

        private void RunTest(string sExpected)
        {
            var dynLinker = new Mock<IDynamicLinker>();
            var listener = new Mock<IDecompilerEventListener>();
            var scanner = new ShingleScanner(program, cfg, dynLinker.Object, listener.Object, new ServiceContainer());
            var cfgNew = scanner.ScanProgram();
            scanner.RegisterPredecessors();
            var sw = new StringWriter();
            DumpCfg(cfgNew, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        private void DumpCfg(ScanResultsV2 cfg, StringWriter sw)
        {
            var g = new CfgGraph(cfg);
            var cfgs = cfg.Blocks.Values.OrderBy(b => b.Address);
            sw.WriteLine();
            foreach (var block in cfgs)
            {
                DumpBlock(block, g, sw);
            }
        }

        private void Given_x86_Image(params byte[] bytes)
        {
            var image = new ByteMemoryArea(
                Address.Ptr32(0x10000),
                bytes);
            this.rd = image.Relocations;
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            CreateProgram(image, arch);
        }

        private void Given_Image(IProcessorArchitecture arch, params byte[] bytes)
        {
            var image = new ByteMemoryArea(
                Address.Ptr32(0x10000),
                bytes);
            this.rd = image.Relocations;
            CreateProgram(image, arch);
        }

        private void Given_x86_Image(Action<X86Assembler> asm)
        {
            var addrBase = Address.Ptr32(0x100000);
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            var entry = ImageSymbol.Procedure(arch, addrBase);
            var m = new X86Assembler(arch, addrBase, new List<ImageSymbol> { entry });
            asm(m);
            this.program = m.GetImage();
            this.program.Platform = new DefaultPlatform(null, arch);
        }

        private void CreateProgram(ByteMemoryArea bmem, IProcessorArchitecture arch)
        {
            var segmentMap = new SegmentMap(bmem.BaseAddress);
            var seg = segmentMap.AddSegment(new ImageSegment(
                ".text",
                bmem,
                AccessMode.ReadExecute)
            {
                Size = (uint) bmem.Bytes.Length
            });
            seg.Access = AccessMode.ReadExecute;
            var platform = new DefaultPlatform(null, arch);
            program = new Program(
                segmentMap,
                arch,
                platform);
        }

        private void Given_ExecutableSegment(string name, uint uAddr, uint len)
        {
            var addr = Address.Ptr32(uAddr);
            var mem = new ByteMemoryArea(addr, new byte[len]);
            program.SegmentMap.AddSegment(mem, name, AccessMode.ReadExecute);
        }

        private void Given_CodeBlock(IProcessorArchitecture arch, uint uAddr, int len)
        {
            var addr = Address.Ptr32(uAddr);
            var id = program.NamingPolicy.BlockName(addr);
            cfg.Blocks.TryAdd(addr, new RtlBlock(arch, addr, id, len, addr + len,
                ProvenanceType.None,
                new List<RtlInstructionCluster>()));
        }

        private void Given_UnknownBlock(uint uAddr, uint len)
        {
            var addr = Address.Ptr32(uAddr);
            var item = new ImageMapItem(addr) { Size = len };
            program.ImageMap.AddItem(addr, item);
        }

        private void Given_DataBlock(uint uAddr, uint len)
        {
            var addr = Address.Ptr32(uAddr);
            var item = new ImageMapItem(addr)
            {
                Size = len,
                DataType = new ArrayType(PrimitiveType.Byte, 0)
            };
            program.ImageMap.AddItem(addr, item);
        }

        private void Lin(uint uAddr, int len, int next)
        {
            var addr = Address.Ptr32((uint) uAddr);
            clusters.Add(uAddr, new RtlInstructionCluster(addr, len,
                new RtlAssignment(id, Constant.Word32(uAddr)))
                {
                    Class = InstrClass.Linear
                });
        }

        private void Call(uint uAddr, int len, int next, int uAddrDst)
        {
            var addr = Address.Ptr32((uint) uAddr);
            clusters.Add(
                uAddr,
                new RtlInstructionCluster(addr, len,
                    new RtlCall(
                        Address.Ptr32((uint)uAddrDst),
                        0,
                        InstrClass.Transfer|InstrClass.Call))
            {
                Class = InstrClass.Transfer|InstrClass.Call
            });
        }

        private void Bra(uint uAddr, int len, int a, uint b)
        {
            var addr = Address.Ptr32((uint) uAddr);
            clusters.Add(
                uAddr,
                new RtlInstructionCluster(addr, len,
                    new RtlBranch(
                        id,
                        Address.Ptr32(b),
                        InstrClass.ConditionalTransfer))
                {
                    Class = InstrClass.ConditionalTransfer
                });
        }

        /// <summary>
        /// Direct jump
        /// </summary>
        private void Jmp(uint uAddr, int len, uint uAddrDst)
        {
            var addr = Address.Ptr32(uAddr);
            var addrDst = Address.Ptr32(uAddrDst);
            clusters.Add(uAddr, new RtlInstructionCluster(addr, len,
                new RtlGoto(addrDst, InstrClass.Transfer))
            {
                Class = InstrClass.Transfer
            });
        }

        /// <summary>
        /// Indirect jump
        /// </summary>
        private void Jmpi(uint uAddr, int len, uint uAddrNext)
        {
            var addr = Address.Ptr32((uint) uAddr);
            clusters.Add(uAddr, new RtlInstructionCluster(addr, len,
                new RtlGoto(id, InstrClass.Transfer | InstrClass.Indirect))
                {
                    Class = InstrClass.Transfer | InstrClass.Indirect
                });
        }

        private void Bad(uint uAddr, int len)
        {
            var addr = Address.Ptr32((uint) uAddr);
            clusters.Add(
                uAddr,
                new RtlInstructionCluster(addr, len,
                    new RtlInvalid())
                {
                    Class = InstrClass.Invalid
                });
        }

        private void End(uint uAddr, int len)
        {
            var addr = Address.Ptr32((uint) uAddr);
            clusters.Add(
                uAddr,
                new RtlInstructionCluster(addr, len,
                    new RtlReturn(0, 0, InstrClass.Transfer|InstrClass.Return))
                {
                    Class = InstrClass.Transfer | InstrClass.Return
                });
        }

        private void Pad(uint uAddr, int len, int next)
        {
            var addr = Address.Ptr32((uint) uAddr);
            clusters.Add(
                uAddr,
                new RtlInstructionCluster(addr, len,
                    new RtlNop())
                {
                    Class = InstrClass.Linear | InstrClass.Padding,
                });
        }

        private void Given_OverlappingLinearTraces()
        {
            Lin(0x100, 2, 0x102);
            Lin(0x101, 2, 0x103);
            Lin(0x102, 2, 0x104);
            Bad(0x103, 2);
            End(0x104, 2);
        }

        private class TestRewriter : IEnumerable<RtlInstructionCluster>
        {
            private readonly ShingleScannerTests outer;
            private Address addr;
            private readonly Address addrMax;

            public TestRewriter(ShingleScannerTests outer, Address addr)
            {
                this.outer = outer;
                this.addr = addr;
                this.addrMax = outer.clusters.OrderByDescending(c => c.Key)
                    .Select(c => c.Value.Address + c.Value.Length)
                    .First();
            }

            public IEnumerator<RtlInstructionCluster> GetEnumerator()
            {
                while (addr < addrMax)
                {
                    if (!outer.clusters.TryGetValue(addr.ToUInt32(), out var cluster))
                    {
                        cluster = new RtlInstructionCluster(addr, 1, new RtlInvalid())
                        {
                            Class = InstrClass.Invalid
                        };
                    }
                    yield return cluster;
                    addr += cluster.Length;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }

        private void CreateX86Scanner()
        {
            var dynLinker = new Mock<IDynamicLinker>();
            this.scanner = new ShingleScanner(program, cfg, dynLinker.Object, listener, new ServiceContainer());
        }

        private void CreateScanner(uint uAddr)
        {
            this.program = new Program();
            this.program.Platform = base.platform.Object;
            var mem = new ByteMemoryArea(Address.Ptr32(uAddr), new byte[256]);
            this.program.SegmentMap = new SegmentMap(new ImageSegment(
                ".text",
                mem,
                AccessMode.ReadExecute));
            var arch = new Mock<IProcessorArchitecture>();
            var dynamicLinker = new Mock<IDynamicLinker>();

            arch.Setup(a => a.Name).Returns("A");
            arch.Setup(a => a.Endianness).Returns(EndianServices.Little);
            arch.Setup(a => a.InstructionBitSize).Returns(8);
            arch.Setup(a => a.MemoryGranularity).Returns(8);
            arch.Setup(a => a.CodeMemoryGranularity).Returns(8);
            arch.Setup(a => a.CreateProcessorState()).Returns(new Func<ProcessorState>(() =>
                new DefaultProcessorState(arch.Object)));
            arch.Setup(a => a.CreateImageReader(
                It.IsNotNull<MemoryArea>(),
                It.IsNotNull<Address>())).Returns(new Func<MemoryArea, Address, EndianImageReader>((m, a) =>
                    new LeImageReader((ByteMemoryArea) m, a)));
            arch.Setup(a => a.CreateRewriter(
                It.IsNotNull<EndianImageReader>(),
                It.IsNotNull<ProcessorState>(),
                It.IsNotNull<IStorageBinder>(),
                It.IsNotNull<IRewriterHost>())).Returns(
                new Func<EndianImageReader, ProcessorState, IStorageBinder, IRewriterHost, IEnumerable<RtlInstructionCluster>>(
                    (r, s, b, h) => new TestRewriter(this, r.Address)));
            this.program.Architecture = arch.Object;
            this.scanner = new ShingleScanner(program, cfg, dynamicLinker.Object, listener, new ServiceContainer());
        }

        [Test]
        public void ShScanner_MakeChunks_NoCfg()
        {
            var chunks = When_MakeScanChunks();
            Assert.AreEqual(chunks.Count, 1);
            Assert.AreEqual(0x1000ul, chunks[0].Address.ToLinear());
            Assert.AreEqual(0x10, chunks[0].Length);
        }

        [Test]
        public void ShScanner_MakeChunks_SingleBlock_AtStart()
        {
            Given_Block(0x1000, 0x7);
            var chunks = When_MakeScanChunks();
            Assert.AreEqual(chunks.Count, 1);
            Assert.AreEqual(0x1008ul, chunks[0].Address.ToLinear());
            Assert.AreEqual(8, chunks[0].Length);
        }

        [Test]
        public void ShScanner_MakeChunks_SingleBlock_AtEnd()
        {
            Given_Block(0x1007, 0x10);
            var chunks = When_MakeScanChunks();
            Assert.AreEqual(chunks.Count, 1);
            Assert.AreEqual(0x1000ul, chunks[0].Address.ToLinear());
            Assert.AreEqual(7, chunks[0].Length);
        }

        [Test]
        public void ShScanner_MakeChunks_SingleBlock_InMiddle()
        {
            Given_Block(0x1007, 4);
            var chunks = When_MakeScanChunks();
            Assert.AreEqual(chunks.Count, 2);
            Assert.AreEqual(0x1000ul, chunks[0].Address.ToLinear());
            Assert.AreEqual(7, chunks[0].Length);
            Assert.AreEqual(0x100Cul, chunks[1].Address.ToLinear());
            Assert.AreEqual(4, chunks[1].Length);
        }

        [Test]
        public void ShScanner_LinearBlock()
        {
            // No previous discoveries in the block, all instructions 4 bytes.
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Assign(r1, m.IAdd(r2, 3)),
                m => m.Assign(r2, m.Mem32(r1)),
                m => m.Assign(r1, m.Mem32(r2)),
                m => m.Return(0, 0)
            });

            string sExpected =
            #region Expected
                @"
l00001000: // l:16; ft:00001010
    // pred:
    r1 = r2 + 3<32>
    r2 = Mem0[r1:word32]
    r1 = Mem0[r2:word32]
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void ShScanner_Branch()
        {
            // No previous discoveries in the block, all instructions 4 bytes.
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Branch(m.Eq0(r1), Address.Ptr32(0x1008)),
                m => m.Assign(r2, m.UDiv(Constant.UInt32(1000), r1)),
                m => m.Assign(r1, m.Mem32(r2)),
                m => m.Return(0, 0)
            });

            string sExpected =
            #region Expected
@"
l00001000: // l:4; ft:00001004
    // pred:
    if (r1 == 0<32>) branch 00001008
    // succ: l00001004 l00001008
l00001004: // l:4; ft:00001008
    // pred: l00001000
    r2 = 0x3E8<u32> /u r1
    // succ: l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001000 l00001004
    r1 = Mem0[r2:word32]
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void ShScanner_Mips_call()
        {
            // The call address is built dynamically.
            Given_Trace(new RtlTrace(0x1000)
            {
                m => m.Branch(m.Eq0(r1), Address.Ptr32(0x1008)),
                m => m.Assign(r2, m.UDiv(Constant.UInt32(1000), r1)),
                m => m.Assign(r1, m.Mem32(r2)),
                m => m.Return(0, 0)
            });

            string sExpected =
            #region Expected
@"
l00001000: // l:4; ft:00001004
    // pred:
    if (r1 == 0<32>) branch 00001008
    // succ: l00001004 l00001008
l00001004: // l:4; ft:00001008
    // pred: l00001000
    r2 = 0x3E8<u32> /u r1
    // succ: l00001008
l00001008: // l:8; ft:00001010
    // pred: l00001000 l00001004
    r1 = Mem0[r2:word32]
    return (0,0)
    // succ:
";
            #endregion

            RunTest(sExpected);
        }

        [Test]
        public void Shsc_Blocks()
        {
            Given_OverlappingLinearTraces();

            CreateScanner(0x100);
            var sr = scanner.ScanProgram();

            Assert.AreEqual(1, sr.Blocks.Count);
        }

        [Test]
        public void Shsc_InvalidBlocks()
        {
            Given_OverlappingLinearTraces();

            CreateScanner(0x100);
            var sr = scanner.ScanProgram();

            var sExp =
            #region Expected
@"
00000100-00000106 (6): End
";
            #endregion

            AssertBlocks(sExp, sr.Blocks);
        }

        [Test]
        public void Shsc_ShingledBlocks()
        {
            Lin(0x0001B0D7, 2, 0x0001B0D9);
            Lin(0x0001B0D8, 6, 0x0001B0DE);
            Lin(0x0001B0D9, 1, 0x0001B0DA);
            Lin(0x0001B0DA, 1, 0x0001B0DB);
            Bra(0x0001B0DB, 2, 0x0001B0DD, 0x0001B0DE);

            Lin(0x0001B0DC, 2, 0x0001B0DE);
            Lin(0x0001B0DD, 1, 0x0001B0DE);

            End(0x0001B0DE, 2);

            CreateScanner(0x0001B0D7);
            var blocks = scanner.ScanProgram().Blocks;
            var sExp =
            #region Expected
@"
0001B0D7-0001B0DD (6): Bra 0001B0DD, 0001B0DE
0001B0D8-0001B0DE (6): Lin 0001B0DE
0001B0DC-0001B0DE (2): Lin 0001B0DE
0001B0DD-0001B0DE (1): Lin 0001B0DE
0001B0DE-0001B0E0 (2): End
";
            #endregion
            AssertBlocks(sExp, blocks);
        }

        private void AssertBlocks(string sExp, IDictionary<Address, RtlBlock> blocks)
        {
            var sw = new StringWriter();
            sw.WriteLine();
            this.scanner.DumpBlocks(cfg, blocks, sw.WriteLine);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.WriteLine("* Failed AssertBlocks ***");
                Console.Write(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void Shsc_Overlapping()
        {
            // At offset 0, we have 0x33, 0xC0, garbage.
            // At offset 1, we have rol al,0x90, ret.
            Lin(0x00010000, 2, 0x00010002);
            Lin(0x00010001, 3, 0x00010004);
            Bad(0x00010002, 3);
            End(0x00010004, 1);
            CreateScanner(0x00010000);
            var blocks = scanner.ScanProgram();
            var sExp =
            #region Expected
@"
00010001-00010005 (4): End
";
            #endregion
            AssertBlocks(sExp, cfg.Blocks);
        }

        [Test]
        public void Shsc_Regression_0001()
        {
            Given_x86_Image(
                0x55,                               // 0000: push ebp
                0x8B, 0xEC,                         // 0001: mov ebp,esp
                0x81, 0xEC, 0x68, 0x01, 0x00, 0x00, // 0003: sub esp,168h
                0x53,                               // 0009: push ebx
                0x56,                               // 000A: push esi
                0x57,                               // 000B: push edi
                0x8D, 0xBD, 0x98, 0xFE, 0xFF, 0xFF, // 000C: lea edi,[ebp-168h]
                0xB9, 0x5A, 0x00, 0x00, 0x00,       // 0012: mov ecx,5ah
                0xC3,                               // 0017: ret
                0xC3,
                0xC3);
            CreateX86Scanner();
            var seg = program.SegmentMap.Segments.Values.First();
            cfg = scanner.ScanProgram();
            var sExp = @"
00010000-00010003 (3): Lin 00010003
00010002-00010003 (1): Lin 00010003
00010003-00010009 (6): Lin 00010009
00010004-0001000A (6): Lin 0001000A
00010006-00010008 (2): Lin 00010008
00010007-00010009 (2): Zer 00010009
00010008-0001000B (3): Zer 0001000B
00010009-0001000A (1): Lin 0001000A
0001000A-0001000B (1): Lin 0001000B
0001000B-00010012 (7): Lin 00010012
0001000D-00010012 (5): Lin 00010012
00010012-00010017 (5): Lin 00010017
00010013-00010014 (1): Lin 00010014
00010014-00010018 (4): Zer 00010018
00010015-00010017 (2): Zer 00010017
00010017-00010018 (1): End
00010018-00010019 (1): End
00010019-0001001A (1): End
";
            AssertBlocks(sExp, cfg.Blocks);
        }

        [Test]
        public void Shsc_Terminate()
        {
            Given_x86_Image(m =>
            {
                m.Hlt();
                m.Mov(m.eax, 0);
                m.Ret();
            });
            CreateX86Scanner();

            var sr = scanner.ScanProgram();

            var sExp =
            #region Expected
@"
00100000-00100001 (1): Trm
00100001-00100006 (5): Lin 00100006
00100002-00100006 (4): Zer 00100006
00100006-00100007 (1): End
";
            #endregion
            AssertBlocks(sExp, sr.Blocks);
        }

        [Test(Description = "Stop tracing invalid blocks at call boundaries")]
        public void Shsc_Call_TerminatesBlock()
        {
            Lin(0x1000, 3, 0x1003);
            Lin(0x1003, 2, 0x1005);
            Call(0x1005, 5, 0x100A, 0x1010);
            End(0x100A, 1);

            CreateScanner(0x1000);
            var blocks = scanner.ScanProgram();

            var sExp =
            #region Expected
@"
00001000-0000100A (10): Cal 0000100A
0000100A-0000100B (1): End
";
            #endregion
            AssertBlocks(sExp, blocks.Blocks);
        }

        [Test(Description = "Stop tracing invalid blocks at call boundaries")]
        public void Shsc_CallThen_Invalid()
        {
            Lin(0x1000, 3, 0x1003);
            Lin(0x1003, 2, 0x1005);
            Call(0x1005, 5, 0x100A, 0x1010);
            Bad(0x100A, 1);

            CreateScanner(0x1000);
            var cfg = scanner.ScanProgram();

            var sExp =
            #region Expected
@"
00001000-0000100A (10): Cal
";
            #endregion
            AssertBlocks(sExp, cfg.Blocks);
        }

        [Test(Description = "Don't make blocks containing a possible call target")]
        public void Shsc_CallTargetInBlock()
        {
            Lin(0x1000, 3, 0x1003);
            Lin(0x1003, 5, 0x1008);
            Lin(0x1008, 4, 0x100C);
            End(0x100C, 4);

            CreateScanner(0x1000);
            cfg.SpeculativeProcedures.TryAdd(Address.Ptr32(0x1008), 2);
            cfg = scanner.ScanProgram();
            Assert.IsTrue(cfg.Blocks.ContainsKey(Address.Ptr32(0x1008)));
        }

        [Test]
        public void Shsc_PaddingBlocks()
        {
            Lin(0x1000, 4, 0x1004);
            End(0x1004, 4);
            Pad(0x1008, 4, 0x100C);
            Pad(0x100C, 4, 0x1010);
            Lin(0x1010, 4, 0x1014);
            End(0x1014, 4);

            CreateScanner(0x1000);
            var blocks = scanner.ScanProgram();
            var sExp =
            #region Expected
@"
00001000-00001008 (8): End
00001008-00001010 (8): Pad 00001010
00001010-00001018 (8): End
";
            #endregion

            AssertBlocks(sExp, blocks.Blocks);
        }

        [Test]
        public void Shsc_FindUnscannedRanges_AUA()
        {
            var A = new Mock<IProcessorArchitecture>();
            A.Setup(a => a.Name).Returns("A");
            Given_Image(A.Object, new byte[0x100]);
            Given_CodeBlock(A.Object, 0x1000, 20);
            Given_UnknownBlock(0x1020, 20);
            Given_CodeBlock(A.Object, 0x1040, 0xC0);
            CreateScanner(0x1000);

            var ranges = scanner.MakeScanChunks().ToArray();

            Assert.AreEqual(1, ranges.Length);
            Assert.AreEqual(A.Object.Name, ranges[0].Architecture.Name);
        }

        [Test]
        public void Shsc_FindUnscannedRanges_UA()
        {
            var A = new Mock<IProcessorArchitecture>();
            A.Setup(a => a.Name).Returns("A");
            Given_Image(A.Object, new byte[100]);
            Given_UnknownBlock(0x1020, 20);
            Given_CodeBlock(A.Object, 0x1040, 0xC0);
            CreateScanner(0x1000);

            var ranges = scanner.MakeScanChunks().ToArray();

            Assert.AreEqual(1, ranges.Length);
            Assert.AreEqual(A.Object.Name, ranges[0].Architecture.Name);
        }

        [Test]
        [Ignore("Think about how to merge ImageMap with ScanResultsV2")]
        public void Shsc_FindUnscannedRanges_BUB()
        {
            var A = new Mock<IProcessorArchitecture>();
            var B = new Mock<IProcessorArchitecture>();
            A.Setup(a => a.Name).Returns("A");
            B.Setup(a => a.Name).Returns("B");

            Given_Image(A.Object, new byte[100]);
            Given_CodeBlock(B.Object, 0x1000, 20);
            Given_UnknownBlock(0x1020, 20);
            Given_CodeBlock(B.Object, 0x1040, 0xC0);
            CreateScanner(0x1000);

            var ranges = scanner.MakeScanChunks().ToArray();

            Assert.AreEqual(1, ranges.Length);
            Assert.AreEqual(B.Object.Name, ranges[0].Architecture.Name);
        }

        [Test]
        [Ignore("Think about how to merge ImageMap with ScanResultsV2")]
        public void Shsc_FindUnscannedRanges_AUBB()
        {
            var A = new Mock<IProcessorArchitecture>();
            var B = new Mock<IProcessorArchitecture>();
            A.Setup(a => a.Name).Returns("A");
            B.Setup(a => a.Name).Returns("B");

            Given_Image(A.Object, new byte[100]);
            Given_CodeBlock(A.Object, 0x1000, 20);
            Given_UnknownBlock(0x1020, 20);
            Given_CodeBlock(B.Object, 0x1040, 0xC0);
            CreateScanner(0x1000);

            var ranges = scanner.MakeScanChunks().ToArray();

            Assert.AreEqual(1, ranges.Length);
            Assert.AreEqual(B.Object.Name, ranges[0].Architecture.Name);
        }

        [Test(Description = "Stop tracing invalid blocks at call boundaries")]
        public void Shsc_CallThenInvalid()
        {
            Call(0x1000, 4, 0x1004, 0x1010);
            Bra(0x1004, 4, 0x1008, 0x100C);
            Bad(0x1008, 4);
            Bad(0x100C, 4);

            Lin(0x1010, 4, 0x1014);
            Bad(0x1014, 4);

            CreateScanner(0x1000);
            var cfg = scanner.ScanProgram();

            var sExp =
            #region Expected
@"
00001000-00001004 (4): Cal
";
            #endregion
            AssertBlocks(sExp, cfg.Blocks);
        }

        [Test]
        public void Shsc_MergingBlocks()
        {
            Lin(0x1000, 2, 0x1002);
            Lin(0x1001, 1, 0x1002);
            End(0x1002, 1);

            CreateScanner(0x1000);
            var sr = scanner.ScanProgram();

            var sExp =
            #region Expected
@"
00001000-00001002 (2): Lin 00001002
00001001-00001002 (1): Lin 00001002
00001002-00001003 (1): End
";
            #endregion
            AssertBlocks(sExp, sr.Blocks);
        }

        [Test]
        public void Shsc_FallIntoPltStub()
        {
            Given_TrampolineAt(0x1002, 0x1004, "imported_proc");

            Lin(0x1000, 2, 0x1002); // This statements falls through into...
            Lin(0x1002, 2, 0x1004); // ...the beginning of the stub.
            Jmpi(0x1004, 4, 0x1008);

            CreateScanner(0x1000);
            var sr = scanner.ScanProgram();

            var sExp =
            #region Expected
                @"
00001000-00001002 (2): Lin 00001002
00001002-00001008 (6): End
";
            #endregion
            AssertBlocks(sExp, sr.Blocks);
        }

        [Test]
        public void Shsc_InfiniteLoop()
        {
            Lin(0x1000, 1, 0x1001);
            Lin(0x1001, 1, 0x1002);
            Lin(0x1002, 2, 0x1004);
            Jmp(0x1004, 2, 0x1004);
            End(0x1006, 1);

            Lin(0x1003, 2, 0x1005);
            Lin(0x1005, 1, 0x1006);

            CreateScanner(0x1000);
            var sr = scanner.ScanProgram();

            var sExp =
            #region Expected
    @"
00001000-00001004 (4): Lin 00001004
00001003-00001006 (3): Lin 00001006
00001004-00001006 (2): End 00001004
00001006-00001007 (1): End
";
            #endregion
            AssertBlocks(sExp, sr.Blocks);
        }

        [Test]
        public void Shsc_Call_RecursiveProcedure()
        {
            CreateScanner(0x1000);
            Given_RecursiveProc(0x2004);
            Given_ExecutableSegment(".text2", 0x2000, 0x100);

            // If shingle scan a call to a procedure that was previously
            // found by recursive calls, we're OK.
            Lin(0x1000, 2, 0x1002);
            Call(0x1002, 2, 0x1004, 0x2000);    // 0x2000 is a valid call target.
            End(0x1004, 2);

            var sr = scanner.ScanProgram();

            var sExp =
            #region Expected
                @"
00001000-00001004 (4): Cal 00001004
00001004-00001006 (2): End
";
            #endregion
            AssertBlocks(sExp, sr.Blocks);
        }

        [Test]
        public void Shsc_Call_Hyperspace()
        {
            // If shingle scan a call to non-executable space,
            // we should mark the block as invalid.
            Lin(0x1000, 2, 0x1002);
            Call(0x1002, 2, 0x1004, 0x2000);    // 0x2000 is an invalid call target.
            End(0x1004, 2);

            CreateScanner(0x1000);

            var sr = scanner.ScanProgram();

            var sExp =
            #region Expected
                @"
00001004-00001006 (2): End
";
            #endregion
            AssertBlocks(sExp, sr.Blocks);
        }
    }
}
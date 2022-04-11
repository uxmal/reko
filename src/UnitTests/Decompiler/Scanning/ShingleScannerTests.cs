#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class ShingleScannerTests : AbstractScannerTests
    {
        private ScanResultsV2 cfg = default!;

        [SetUp]
        public void Setup()
        {
            base.Setup(0x10, 16); // block of 0x10 bytes, with 16-bit instruction granularity.
            this.cfg = new ScanResultsV2();
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
            var block = new RtlBlock(program.Architecture, addr, id, length, addrFallThrough, instrs);
            cfg.Blocks.TryAdd(addr, block);
        }

        private List<ChunkWorker> When_MakeScanChunks()
        {
            var scanner = new ShingleScanner(program, cfg, new Mock<DecompilerEventListener>().Object);
            var chunks = scanner.MakeScanChunks();
            return chunks;
        }

        private void RunTest(string sExpected)
        {
            var scanner = new ShingleScanner(program, cfg, new Mock<DecompilerEventListener>().Object);
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
    }
}

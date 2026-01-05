#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
 *
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Graphs;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HeuristicProcedure = Reko.Scanning.HeuristicProcedure;
using RtlBlock = Reko.Scanning.RtlBlock;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class BlockConflictResolverTests : HeuristicTestBase
    {
        private HeuristicProcedure proc = default!;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        private void AssertConflicts(string sExp, IEnumerable<(RtlBlock, RtlBlock)> conflicts)
        {
            var sActual = conflicts
                .OrderBy(c => c.Item1.Address.ToLinear())
                .ThenBy(c => c.Item2.Address.ToLinear())
                .Aggregate(
                    new StringBuilder(),
                    (s, c) => s.AppendFormat("({0}-{1})", c.Item1.Address, c.Item2.Address).AppendLine())
                .ToString();
            if (sExp != sActual)
            {
                Debug.Print(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private ScanResultsV2 CreateScanResults(DiGraph<RtlBlock> icfg)
        {
            var sr = new ScanResultsV2();
            foreach (var node in icfg.Nodes)
            {
                sr.Blocks.TryAdd(node.Address, node);
                foreach (var s in icfg.Successors(node))
                {
                    sr.ICFG.AddEdge(node.Address, s.Address);
                }
            }
            return sr;
        }

        private void When_DisassembleProcedure()
        {
            var hsc = new Reko.Scanning.HeuristicScanner(program, host.Object, eventListener.Object);
            var mem = program.SegmentMap.Segments.Values.First().MemoryArea;
            //$BUG: danger of overflow
            this.proc = hsc.DisassembleProcedure(
                mem.BaseAddress,
                mem.BaseAddress + mem.Length);
        }

        private RtlBlock Given_Block(uint uAddr)
        {
            var b = RtlBlock.CreateEmpty(null!, Address.Ptr32(uAddr), $"l{uAddr:X8}");
            return b;
        }

        private void Given_Instrs(RtlBlock block, Action<RtlEmitter> b)
        {
            var instrs = new List<RtlInstruction>();
            var trace = new RtlEmitter(instrs);
            b(trace);
            var rtlc = InstrClass.Linear;
            if (instrs.Count == 1)
            {
                rtlc = instrs.First().Class;
            }
            block.Instructions.Add(
                new RtlInstructionCluster(
                    block.Address + (block.Instructions.Count * 4),
                    4,
                    instrs.ToArray())
                {
                    Class = rtlc,
                });
        }

        private void ResolveBlockConflicts(ScanResultsV2 sr)
        {
            var hps = new BlockConflictResolver(null, sr, null, null);
            hps.ResolveBlockConflicts(Array.Empty<Address>());
        }

        [Test]
        public void HPSC_Conflicts_1()
        {
            Given_Image32(
                0x0010000,
                "55 89 e5 c3");
            Given_x86_32();
            Given_RewriterHost();

            When_DisassembleProcedure();
            var conflicts = BlockConflictResolver.BuildConflictGraph(proc.Cfg.Nodes);
            var sExp =
@"(00010001-00010002)
(00010002-00010003)
";
            AssertConflicts(sExp, conflicts);
        }

        [Test]
        public void HPSC_Conflicts_2()
        {
            Given_Image32(
                0x0010000,
                "55 E8 00 00 00 38 c3");
            Given_x86_32();
            Given_RewriterHost();

            When_DisassembleProcedure();
            var conflicts = BlockConflictResolver.BuildConflictGraph(proc.Cfg.Nodes);
            var sExp =
@"(00010001-00010002)
(00010001-00010003)
(00010001-00010004)
(00010001-00010005)
(00010002-00010003)
(00010003-00010004)
(00010004-00010005)
(00010005-00010006)
";
            AssertConflicts(sExp, conflicts);
        }

        [Test]
        public void HPSC_DiscardNodes()
        {
            Given_Image32(
                0x0010000,
                "55 8B EC a1 32 12 1a 12 5D c3");
            Given_x86_32();
            Given_RewriterHost();
            Given_NoImportedProcedures();

            When_DisassembleProcedure();
            var sr = CreateScanResults(proc.Cfg);
            var hps = new BlockConflictResolver(program, sr, proc.IsValidAddress, host.Object);
            hps.ResolveBlockConflicts(new[] { proc.BeginAddress });

            var sExp =
            #region Expected
@"l00010000:  // pred:
    push ebp
l00010001:  // pred: l00010000
    mov ebp,esp
l00010003:  // pred: l00010001
    mov eax,[121A1232h]
l00010008:  // pred: l00010003
    pop ebp
l00010009:  // pred: l00010008
    ret
";
            #endregion

            AssertBlocks(sExp, sr);
        }

        [Test(Description =
            "If caller block was discarded, call target address should " +
            "be removed from directly called addresses list")]
        public void HPSC_ResolveBlockConflicts_RemoveDirectlyCalledAddress()
        {
            var calledAddr = Address.Ptr32(0x5678);
            var wrongBlock = Given_Block(0x1234);
            Given_Instrs(wrongBlock, m =>
            {
                m.Call(calledAddr, 4);
            });
            var firstBlock = Given_Block(0x1235);
            Given_Instrs(firstBlock, m =>
            {
                m.Assign(m.Mem32(m.Word32(0x2222)), 0);
            });
            var lastBlock = Given_Block(0x1240);
            Given_Instrs(lastBlock, m =>
            {
                m.Return(4, 0);
            });
            var cfg = new DiGraph<RtlBlock>();
            cfg.AddNode(wrongBlock);
            cfg.AddNode(firstBlock);
            cfg.AddNode(lastBlock);
            cfg.AddEdge(firstBlock, lastBlock);
            var sr = CreateScanResults(cfg);
            sr.SpeculativeProcedures.TryAdd(calledAddr, 1);

            ResolveBlockConflicts(sr);

            Assert.AreEqual(0, sr.SpeculativeProcedures.Count);
        }

        [Test]
        [Ignore("This code will be obsolete soon")]
        public void HPSC_TrickyProc()
        {
            Given_Image32(0x0010000, TrickyProc);
            program.SegmentMap.AddSegment(new ImageSegment(
                "code",
                new ByteMemoryArea(Address.Ptr32(0x11750000), new byte[100]),
                AccessMode.ReadExecute));
            Given_x86_32();
            Given_RewriterHost();
            Given_NoImportedProcedures();

            When_DisassembleProcedure();
            var sr = CreateScanResults(proc.Cfg);
            var hps = new BlockConflictResolver(program, sr, proc.IsValidAddress, host.Object);
            hps.ResolveBlockConflicts(new[] { proc.BeginAddress });

            var sExp =
            #region Expected
 @"l00010000:  // pred:
    push ebp
l00010001:  // pred: l00010000
    mov ebp,esp
l00010003:  // pred: l00010001
    call 11750008
l0001000A:  // pred:
    cmp al,00
l0001000C:  // pred: l0001000A
    jnz 00010014
l0001000E:  // pred: l0001000C
    mov al,00
l00010010:  // pred: l0001000E
    jmp 00010019
l00010014:  // pred: l0001000C
    mov eax,[01740000]
l00010019:  // pred: l00010010 l00010014
    mov esp,ebp
l0001001B:  // pred: l00010019
    pop ebp
l0001001C:  // pred: l0001001B
    ret 
";
            #endregion
            AssertBlocks(sExp, sr);
        }
    }
}

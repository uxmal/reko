#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class BackwardSlicerTests
    {
        private StorageBinder binder;
        private IProcessorArchitecture arch;
        private ServiceContainer sc;
        private FakeArchitecture fakeArch;
        private RtlBackwalkHost host;
        private Program program;
        private DirectedGraph<RtlBlock> graph;
        private ProcessorState processorState;
        private PrimitiveType word24;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            fakeArch = new FakeArchitecture(sc);
            arch = fakeArch;
            var segmentMap = new SegmentMap(
                    Address.Ptr32(0x00120000),
                    new ImageSegment(
                        ".text",
                        new ByteMemoryArea(Address.Ptr32(0x00120000), new byte[0x10000]),
                        AccessMode.ReadExecute));
            program = new Program
            {
                Architecture = arch,
                SegmentMap = segmentMap,
                Memory = new ByteProgramMemory(segmentMap),
            };
            binder = new StorageBinder();
            graph = new DiGraph<RtlBlock>();
            host = new RtlBackwalkHost(program, graph);
            processorState = arch.CreateProcessorState();
            word24 = PrimitiveType.CreateWord(24);

        }

        private Identifier Reg(string name)
        {
            var reg = arch.GetRegister(name);
            return binder.EnsureRegister(reg);
        }


        private Identifier Cc(string name)
        {
            var cc = arch.GetFlagGroup(name);
            return binder.EnsureFlagGroup(cc);
        }

        private RtlBlock Given_Block(ulong uAddr)
        {
            var b = RtlBlock.CreateEmpty(arch, Address.Ptr64(uAddr), $"l{uAddr:X16}");
            return b;
        }

        private RtlBlock Given_Block(uint uAddr)
        {
            var b = RtlBlock.CreateEmpty(arch, Address.Ptr32(uAddr), $"l{uAddr:X8}");
            return b;
        }

        private RtlBlock Given_Block(ushort uSeg, ushort uOffset)
        {
            var b = RtlBlock.CreateEmpty(arch, Address.SegPtr(uSeg, uOffset), $"l{uSeg:X4}_{uOffset:X4}");
            return b;
        }

        private void Given_Instrs(RtlBlock block, params Action<RtlEmitter>[] generators)
        {
            var instrs = new List<RtlInstruction>();
            var trace = new RtlEmitter(instrs);
            foreach (var gen in generators)
            {
                gen(trace);
                block.Instructions.Add(
                    new RtlInstructionCluster(
                        block.Address + block.Instructions.Count * 4,
                        4,
                        instrs.ToArray()));
                instrs.Clear();
            }
        }

        private Expression Target(RtlBlock block)
        {
            return ((RtlGoto) block.Instructions.Last().Instructions.Last()).Target;
        }

        [Test]
        public void Bwslc_DetectRegister()
        {
            var r1 = Reg("r1");
            var b = Given_Block(0x10);
            Given_Instrs(b, m => m.Goto(r1));

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsTrue(bwslc.Start(b, 0, Target(b)));
            Assert.AreEqual(new BitRange(0, 32), bwslc.Live[r1].BitRange);
        }

        [Test]
        public void Bwslc_DetectNoRegister()
        {
            var r1 = Reg("r1");
            var b = Given_Block(0x10);
            Given_Instrs(b, m => m.Goto(Address.Ptr32(0x00123400)));

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsFalse(bwslc.Start(b, 0, Target(b)));
            Assert.AreEqual(0, bwslc.Live.Count);
        }

        [Test]
        public void Bwslc_SeedSlicer()
        {
            var r1 = Reg("r1");
            var b = Given_Block(0x10);
            Given_Instrs(b, m => m.Goto(r1));

            var bwslc = new BackwardSlicer(host, b, processorState);
            var result = bwslc.Start(b, 0, Target(b));
            Assert.IsTrue(result);
        }

        [Test]
        public void Bwslc_DetectAddition()
        {
            var r1 = Reg("r1");
            var b = Given_Block(0x100);
            Given_Instrs(b, m => m.Goto(m.IAdd(r1, 0x00123400)));

            var bwslc = new BackwardSlicer(host, b, processorState);
            var start = bwslc.Start(b, 0, Target(b));
            Assert.IsTrue(start);
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r1", bwslc.Live.First().Key.ToString());
        }

        [Test]
        public void Bwslc_KillLiveness()
        {
            var r1 = Reg("r1");
            var r2 = Reg("r2");
            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Assign(r1, m.Shl(r2, 2)); });
            Given_Instrs(b, m => { m.Goto(m.IAdd(r1, 0x00123400)); });

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsTrue(bwslc.Start(b, 0, Target(b)));
            Assert.IsTrue(bwslc.Step());
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r2", bwslc.Live.First().Key.ToString());
        }

        [Test(Description = "Trace across a jump")]
        public void Bwslc_AcrossJump()
        {
            var r1 = Reg("r1");
            var r2 = Reg("r2");

            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Assign(r1, m.Shl(r2, 2)); });
            Given_Instrs(b, m => { m.Goto(Address.Ptr32(0x200)); });

            var b2 = Given_Block(0x200);
            Given_Instrs(b2, m => { m.Goto(m.IAdd(r1, 0x00123400)); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, -1, Target(b2)));  // indirect jump
            Assert.IsTrue(bwslc.Step());    // direct jump
            Assert.IsTrue(bwslc.Step());    // shift left
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r2", bwslc.Live.First().Key.ToString());
        }

        [Test(Description = "Trace across a branch where the branch was taken.")]
        public void Bwslc_BranchTaken()
        {
            var r1 = Reg("r1");
            var r2 = Reg("r2");
            var cz = Cc("CZ");

            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.ULE, cz), Address.Ptr32(0x200), InstrClass.ConditionalTransfer); });

            var b2 = Given_Block(0x200);
            Given_Instrs(b2, m => { m.Assign(r1, m.Shl(r2, 2)); });
            Given_Instrs(b2, m => { m.Goto(m.IAdd(r1, 0x00123400)); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 0, Target(b2)));  // indirect jump
            Assert.IsTrue(bwslc.Step());                    // shift left
            Assert.IsTrue(bwslc.Step());                    // branch

            Assert.AreEqual("CZ,r2",
                string.Join(",", bwslc.Live.Select(l => l.Key.ToString()).OrderBy(n => n)));
        }

        [Test(Description = "Trace until the comparison that gates the jump is encountered.")]
        public void Bwslc_RangeCheck()
        {
            var r1 = Reg("r1");
            var r2 = Reg("r2");
            var cz = Cc("CZ");

            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Assign(cz, m.Cond(cz.DataType, m.ISub(r2, 4))); });
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.ULE, cz), Address.Ptr32(0x200), InstrClass.ConditionalTransfer); });

            var b2 = Given_Block(0x200);
            Given_Instrs(b2, m => { m.Assign(r1, m.Shl(r2, 2)); });
            Given_Instrs(b2, m => { m.Goto(m.IAdd(r1, 0x00123400)); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 0, Target(b2)));   // indirect jump
            Assert.IsTrue(bwslc.Step());    // shift left
            Assert.IsTrue(bwslc.Step());    // branch
            Assert.IsFalse(bwslc.Step());   // test
            Assert.AreEqual("r2",
                string.Join(",", bwslc.Live.Select(l => l.Key.ToString()).OrderBy(n => n)));
            Assert.AreEqual("(r2 << 2<8>) + 0x123400<32>", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,4]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Bwslc_BoundIndexWithAnd()
        {
            var r1 = Reg("r1");
            var r2 = Reg("r2");
            var cz = Cc("CZ");

            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Assign(r1, m.And(r1, 7)); m.Assign(cz, m.Cond(cz.DataType, r1)); });
            Given_Instrs(b, m => { m.Goto(m.Mem32(m.IAdd(m.Word32(0x00123400), m.IMul(r1, 4)))); });

            graph.Nodes.Add(b);

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsTrue(bwslc.Start(b, 1, Target(b)));   // indirect jump
            Assert.IsTrue(bwslc.Step());    // assign flags
            Assert.IsFalse(bwslc.Step());    // and
            Assert.AreEqual("Mem0[(r1 & 7<32>) * 4<32> + 0x123400<32>:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,7]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Bwslc_x86_RegisterHack()
        {
            // In old x86 binaries we see this mechanism
            // for zero extending a register.
            arch = new Reko.Arch.X86.X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            var bl = binder.EnsureRegister(arch.GetRegister("bl"));
            var bh = binder.EnsureRegister(arch.GetRegister("bh"));
            var bx = binder.EnsureRegister(arch.GetRegister("bx"));
            var si = binder.EnsureRegister(arch.GetRegister("si"));
            var SCZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SCZO"));
            var SZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SZO"));
            var c = binder.EnsureFlagGroup(arch.GetFlagGroup("C"));

            var b = Given_Block(0x0100);
            Given_Instrs(b, m =>
            {
                m.Assign(bl, m.Mem8(si));
            });
            Given_Instrs(b, m =>
            {
                m.Assign(SCZO, m.Cond(SCZO.DataType, m.ISub(bl, 2)));
            });
            Given_Instrs(b, m =>
            {
                m.Branch(m.Test(ConditionCode.UGT, SCZO), Address.Ptr16(0x120), InstrClass.ConditionalTransfer);
            });

            var b2 = Given_Block(0x200);
            Given_Instrs(b2, m =>
            {
                m.Assign(bh, m.Xor(bh, bh));
                m.Assign(SCZO, m.Cond(SCZO.DataType, bh));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(bx, m.IAdd(bx, bx));
                m.Assign(SCZO, m.Cond(SCZO.DataType, bx));
            });
            Given_Instrs(b2, m =>
            {
                m.Goto(m.Mem16(m.IAdd(bx, 0x8400)));
            });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsTrue(bwslc.Start(b2, 3, Target(b2)));   // indirect jump
            Assert.IsTrue(bwslc.Step());    // assign flags
            Assert.IsTrue(bwslc.Step());    // add bx,bx
            Assert.IsTrue(bwslc.Step());    // assign flags
            Assert.IsTrue(bwslc.Step());    // xor high-byte of bx
            Assert.IsTrue(bwslc.Step());    // branch.
            Assert.IsFalse(bwslc.Step());    // cmp.

            Assert.AreEqual("Mem0[CONVERT(SLICE(bx, byte, 0), byte, word16) * 2<16> + 0x8400<16>:word16]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,2]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test(Description = "test case discovered by @smx-smx. The rep movsd should " +
                            "have no effect on the indirect jump.")]
        public void Bwslc_RepMovsd()
        {
            // Original i386 code:
            // shr ecx,02
            // and edx,03
            // cmp ecx,08
            // jc 00002000
            // rep movsd 
            // jmp dword ptr[007862E8 + edx * 4]

            arch = new Reko.Arch.X86.X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            var ecx = binder.EnsureRegister(arch.GetRegister("ecx"));
            var edx = binder.EnsureRegister(arch.GetRegister("edx"));
            var esi = binder.EnsureRegister(arch.GetRegister("esi"));
            var edi = binder.EnsureRegister(arch.GetRegister("edi"));
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup("C"));
            var SZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SZO"));
            var SCZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SCZO"));
            var tmp = binder.CreateTemporary(ecx.DataType);

            var b = Given_Block(0x1000);
            Given_Instrs(b, m => { m.Assign(ecx, m.Shr(ecx, 2)); m.Assign(SCZO, m.Cond(SCZO.DataType, ecx)); });
            Given_Instrs(b, m => { m.Assign(edx, m.And(edx, 3)); m.Assign(SZO, m.Cond(SZO.DataType, edx)); m.Assign(C, m.False()); });
            Given_Instrs(b, m => { m.Assign(SCZO, m.Cond(SCZO.DataType, m.ISub(ecx, 8))); });
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.ULT, C), Address.Ptr32(0x2000), InstrClass.ConditionalTransfer); });

            var b2 = Given_Block(0x1008);
            Given_Instrs(b2, m =>
            {
                m.BranchInMiddleOfInstruction(m.Eq0(ecx), Address.Ptr32(0x1010), InstrClass.ConditionalTransfer);
                m.Assign(tmp, m.Mem32(esi));
                m.Assign(m.Mem32(edi), tmp);
                m.Assign(esi, m.IAdd(esi, 4));
                m.Assign(edi, m.IAdd(edi, 4));
                m.Assign(ecx, m.ISub(ecx, 1));
                m.Goto(Address.Ptr32(0x1008));
            });

            var b3 = Given_Block(0x1010);
            Given_Instrs(b3, m =>
            {
                m.Goto(m.Mem32(m.IAdd(m.IMul(edx, 4), 0x00123400)));
            });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.Nodes.Add(b3);
            graph.AddEdge(b, b2);
            graph.AddEdge(b2, b3);
            graph.AddEdge(b2, b2);

            var bwslc = new BackwardSlicer(host, b3, processorState);
            Assert.IsTrue(bwslc.Start(b3, -1, Target(b3)));   // indirect jump
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.False(bwslc.Step());     // edx &= 3
            Assert.AreEqual("Mem0[(edx & 3<32>) * 4<32> + 0x123400<32>:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,3]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_SegmentedLoad()
        {
            arch = new Reko.Arch.X86.X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            var cx = binder.EnsureRegister(arch.GetRegister("cx"));
            var bx = binder.EnsureRegister(arch.GetRegister("bx"));
            var ds = binder.EnsureRegister(arch.GetRegister("ds"));
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup("C"));
            var SZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SZO"));
            var SCZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SCZO"));

            var b = Given_Block(0x0C00, 0x0100);
            Given_Instrs(b, m => { m.Assign(SCZO, m.Cond(SCZO.DataType, m.ISub(bx, 15))); });
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.UGT, C), Address.SegPtr(0xC00, 0x200), InstrClass.ConditionalTransfer); });

            var b2 = Given_Block(0x0C00, 0x0108);
            Given_Instrs(b2, m => { m.Assign(bx, m.IAdd(bx, bx)); m.Assign(SCZO, m.Cond(SCZO.DataType, bx)); });
            Given_Instrs(b2, m => { m.Goto(m.SegMem(PrimitiveType.Ptr32, ds, m.IAdd(bx, 34))); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            graph.Nodes.Add(b);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 0, Target(b2)));   // indirect jump
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsFalse(bwslc.Step());

            Assert.AreEqual("Mem0[ds:bx * 2<16> + 0x22<16>:ptr32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,F]", bwslc.JumpTableIndexInterval.ToString());

        }

        [Test]
        public void Bwslc_ClearingBits()
        {
            arch = new Reko.Arch.X86.X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            var eax = binder.EnsureRegister(arch.GetRegister("eax"));
            var edx = binder.EnsureRegister(arch.GetRegister("edx"));
            var dl = binder.EnsureRegister(arch.GetRegister("dl"));
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup("C"));
            var SZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SZO"));
            var SCZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SCZO"));

            var b = Given_Block(0x001000000);
            Given_Instrs(b, m => { m.Assign(SCZO, m.Cond(SCZO.DataType, m.ISub(eax, 3))); });
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.UGT, C), Address.Ptr32(0x00100010), InstrClass.ConditionalTransfer); });

            var b2 = Given_Block(0x001000008);
            Given_Instrs(b2, m => { m.Assign(edx, m.Xor(edx, edx)); m.Assign(SZO, m.Cond(SZO.DataType, edx)); m.Assign(C, m.False()); });
            Given_Instrs(b2, m => { m.Assign(dl, m.Mem8(m.IAdd(eax, 0x00123500))); });
            Given_Instrs(b2, m => { m.Goto(m.Mem32(m.IAdd(m.IMul(edx, 4), 0x00123400))); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            graph.Nodes.Add(b);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 3, Target(b2)));  // indirect jump
            Assert.IsTrue(bwslc.Step());                    // dl = ...
            Assert.IsTrue(bwslc.Step());                    // edx = 0
            Assert.IsTrue(bwslc.Step());                    // branch ...
            Assert.IsTrue(bwslc.Step());                    // SZCO = cond(eax - 3)
            Assert.IsTrue(bwslc.Step());
            Assert.IsFalse(bwslc.Step());

            Assert.AreEqual("Mem0[Mem0[eax + 0x123500<32>:byte] *32 4<32> + 0x123400<32>:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,3]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_SimplifySum()
        {
            var r1 = Reg("r1");
            var b = Given_Block(0x100);
            Given_Instrs(b, m => m.Assign(r1, m.IAdd(r1, r1)));
            Given_Instrs(b, m => m.Goto(m.IAdd(r1, 0x00123400)));

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsTrue(bwslc.Start(b, 0, Target(b)));
            Assert.IsTrue(bwslc.Step());
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r1", bwslc.Live.First().Key.ToString());
            Assert.AreEqual("r1 * 2<32> + 0x123400<32>", bwslc.JumpTableFormat.ToString());
        }

        [Test]
        public void Bwslc_SimplifySumAndProduct()
        {
            var r1 = Reg("r1");
            var b = Given_Block(0x100);
            Given_Instrs(b, m => m.Assign(r1, m.IAdd(r1, r1)));
            Given_Instrs(b, m => m.Assign(r1, m.IAdd(r1, r1)));
            Given_Instrs(b, m => m.Goto(m.IAdd(r1, 0x00123400)));

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsTrue(bwslc.Start(b, 1, Target(b)));
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r1", bwslc.Live.First().Key.ToString());
            Assert.AreEqual("r1 * 4<32> + 0x123400<32>", bwslc.JumpTableFormat.ToString());
        }

        [Test]
        public void Bwslc_Dpbs()
        {
            // This test is derived from a m68k binary which originally looked like this:

            //  cmpi.b #$17,d0
            //  bhi $0010F010
            //  
            //  moveq #$00,d1
            //  move.b d0,d1
            //  add.w d1,d1
            //  move.w (06,pc,d1),d1
            //  jmp.l (pc,d1)

            // The code introduces a lot of DPB's, which must be dealt with appropriately.

            var W8 = PrimitiveType.Byte;
            var W16 = PrimitiveType.Word16;
            var W32 = PrimitiveType.Word32;
            var I16 = PrimitiveType.Int16;
            var I32 = PrimitiveType.Int32;
            arch = new Reko.Arch.Motorola.M68kArchitecture(sc, "m68k", new Dictionary<string, object>());
            var d0 = Reg("d0");
            var d1 = Reg("d1");
            var v2 = binder.CreateTemporary("v2", W8);
            var v3 = binder.CreateTemporary("v3", W8);
            var v4 = binder.CreateTemporary("v4", W16);
            var v5 = binder.CreateTemporary("v5", PrimitiveType.Word16);
            var CVZNX = Cc("CVZNX");
            var CVZN = Cc("CVZN");
            var CZ = Cc("CZ");

            var b = Given_Block(0x00100000);
            Given_Instrs(b, m =>
            {
                m.Assign(v2, m.ISub(m.Slice(d0, PrimitiveType.Byte), 0x17));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v2));
            });
            Given_Instrs(b, m =>
            {
                m.Branch(m.Test(ConditionCode.UGT, CZ), Address.Ptr32(0x00100040), InstrClass.ConditionalTransfer);
            });

            var b2 = Given_Block(0x00100008);
            Given_Instrs(b2, m =>
            {
                m.Assign(d1, m.Word32(0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, d1));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v3, m.Slice(d0, v3.DataType));
                m.Assign(d1, m.Dpb(d1, v3, 0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v3));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v4, m.IAdd(m.Slice(d1, v4.DataType), m.Slice(d1, v4.DataType)));
                m.Assign(d1, m.Dpb(d1, v4, 0));
                m.Assign(CVZNX, m.Cond(CVZNX.DataType, v4));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v5, m.Mem16(m.IAdd(m.Word32(0x0010EC32), m.Convert(m.Slice(d1, W16), W16, W32))));
                m.Assign(d1, m.Dpb(d1, v5, 0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v5));
            });
            Given_Instrs(b2, m =>
            {
                m.Goto(m.IAdd(m.Word32(0x0010EC30), m.Convert(m.Slice(d1, I16), I16, I32)));
            });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 10, Target(b2)));
            while (bwslc.Step())
                ;
            Assert.AreEqual(2, bwslc.Live.Count);
            Console.WriteLine(bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("CONVERT(Mem0[CONVERT(CONVERT(SLICE(d0, byte, 0), uint8, word16) * 2<16>, word16, word32) + 0x10EC32<32>:word16], int16, int32) + 0x10EC30<32>", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("d0", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,17]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_Slices()
        {
            // This test is derived from a m68k binary which originally looked like this:

            //  cmpi.b #$17,d0
            //  bhi $0010F010
            //  
            //  moveq #$00,d1
            //  move.b d0,d1
            //  add.w d1,d1
            //  move.w (06,pc,d1),d1
            //  jmp.l (pc,d1)

            // The code introduces a lot of SLICEs, which must be dealt with appropriately.

            var W8 = PrimitiveType.Byte;
            var W16 = PrimitiveType.Word16;
            var W32 = PrimitiveType.Word32;
            var I16 = PrimitiveType.Int16;
            var I32 = PrimitiveType.Int32;
            arch = new Reko.Arch.Motorola.M68kArchitecture(sc, "m68k", new Dictionary<string, object>());
            var d0 = Reg("d0");
            var d1 = Reg("d1");
            var v2 = binder.CreateTemporary("v2", W8);
            var v3 = binder.CreateTemporary("v3", W8);
            var v4 = binder.CreateTemporary("v4", W16);
            var v5 = binder.CreateTemporary("v5", PrimitiveType.Word16);
            var CVZNX = Cc("CVZNX");
            var CVZN = Cc("CVZN");
            var CZ = Cc("CZ");

            var b = Given_Block(0x00100000);
            Given_Instrs(b, m =>
            {
                m.Assign(v2, m.ISub(m.Slice(d0, PrimitiveType.Byte), 0x17));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v2));
            });
            Given_Instrs(b, m =>
            {
                m.Branch(m.Test(ConditionCode.UGT, CZ), Address.Ptr32(0x00100040), InstrClass.ConditionalTransfer);
            });

            var b2 = Given_Block(0x00100008);
            Given_Instrs(b2, m =>
            {
                m.Assign(d1, m.Word32(0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, d1));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v3, m.Slice(d0, v3.DataType));
                m.Assign(d1, m.Dpb(d1, v3, 0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v3));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v4, m.IAdd(m.Slice(d1, v4.DataType), m.Slice(d1, v4.DataType)));
                m.Assign(d1, m.Dpb(d1, v4, 0));
                m.Assign(CVZNX, m.Cond(CVZNX.DataType, v4));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v5, m.Mem16(m.IAdd(m.Word32(0x0010EC32), m.Convert(m.Slice(d1, W16), W16, W32))));
                m.Assign(d1, m.Dpb(d1, v5, 0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v5));
            });
            Given_Instrs(b2, m =>
            {
                m.Goto(m.IAdd(m.Word32(0x0010EC30), m.Convert(m.Slice(d1, I16), I16, I32)));
            });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 10, Target(b2)));
            while (bwslc.Step())
                ;
            Assert.AreEqual(2, bwslc.Live.Count);
            Assert.AreEqual("CONVERT(Mem0[CONVERT(CONVERT(SLICE(d0, byte, 0), uint8, word16) * 2<16>, word16, word32) + 0x10EC32<32>:word16], int16, int32) + 0x10EC30<32>", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("d0", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,17]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_DetectUsingExpression()
        {
            var r1 = Reg("r1");
            var r2 = Reg("r2");
            var b = Given_Block(0x10);
            Given_Instrs(b, m => m.Assign(r1, m.Mem32(m.IAdd(r2, 8))));
            Given_Instrs(b, m => m.Goto(r1));

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsTrue(bwslc.Start(b, 0, Target(b)));
            Assert.AreEqual(new BitRange(0, 32), bwslc.Live[r1].BitRange);
            Assert.IsTrue(bwslc.Step());
            Assert.AreEqual(2, bwslc.Live.Count);
            Assert.AreEqual("r2", bwslc.Live.First().Key.ToString());
            Assert.AreEqual(new BitRange(0, 32), bwslc.Live.First().Value.BitRange);
        }


        [Test(Description = "Handle m68k-style sign extensions.")]
        [Category(Categories.UnitTests)]
        [Ignore(Categories.FailedTests)]
        public void Bwslc_SignExtension()
        {
            var CVZNX = Cc("CVZNX");
            var CVZN = Cc("CVZN");
            var VZN = Cc("VZN");
            var r1 = Reg("r1");
            var v80 = binder.CreateTemporary("v80", PrimitiveType.Word32);
            var v82 = binder.CreateTemporary("v82", PrimitiveType.Word32);

            var b1 = Given_Block(0x1000);
            Given_Instrs(b1, m =>
            {
                m.Assign(v80, m.ISub(r1, 0x28));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v80));
                m.Branch(m.Test(ConditionCode.GT, VZN), Address.Ptr16(0x1020), InstrClass.ConditionalTransfer);
            });

            var b2 = Given_Block(0x1010);
            Given_Instrs(b2, m =>
            {
                m.Assign(r1, m.IAdd(r1, r1));
                m.Assign(CVZNX, m.Cond(CVZNX.DataType, r1));
                m.Assign(v82, m.Mem16(m.IAdd(m.Word32(0x001066A4), r1)));
                m.Assign(r1, m.Dpb(r1, v82, 0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v82));
                m.Goto(
                    m.IAdd(
                        m.Word32(0x001066A2),
                        m.Convert(m.Slice(r1, PrimitiveType.Int16), PrimitiveType.Int16, PrimitiveType.Int32)),
                    InstrClass.Transfer);
            });

            //m.Label("default_case");
            //m.Return();

            graph.Nodes.Add(b1);
            graph.Nodes.Add(b2);
            graph.AddEdge(b1, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 5, Target(b2)));
            while (bwslc.Step())
                ;

            Assert.AreEqual(2, bwslc.Live.Count);
            Assert.AreEqual("(int32) (int16) DPB(r1 * 0x00000002<32>, Mem0[r1 * 0x00000002<32> + 0x001066A4<32>:word16], 0) + 0x001066A2<32>", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("v80", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,17]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_Issue_691()
        {
            arch = new Reko.Arch.Motorola.M68kArchitecture(sc, "m68k", new Dictionary<string, object>());
            var d0 = Reg("d0");
            var CVZN = Cc("CVZN");
            var C = Cc("C");
            var v3 = binder.CreateTemporary(PrimitiveType.Word16);
            var v16 = binder.CreateTemporary(PrimitiveType.Word16);
            var v17 = binder.CreateTemporary(PrimitiveType.Word16);
            var b1 = Given_Block(0xA860);
            Given_Instrs(b1, m =>
            {
                m.Assign(v3, m.ISub(m.Slice(d0, PrimitiveType.Word16), m.Word16(0x20)));
                m.Assign(d0, m.Dpb(d0, v3, 0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v3));
                m.Branch(m.Test(ConditionCode.UGE, C), Address.Ptr32(0xA900), InstrClass.ConditionalTransfer);
            });

            var bRet = Given_Block(0xA870);
            Given_Instrs(bRet, m =>
            {
                m.Return(0, 0);
            });

            var b2 = Given_Block(0xA900);
            Given_Instrs(b2, m =>
            {
                m.Assign(v16, m.IAdd(m.Slice(d0, PrimitiveType.Word16), m.Slice(d0, PrimitiveType.Word16)));
                m.Assign(d0, m.Dpb(d0, v16, 0));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v16));
                m.Assign(v17, m.IAdd(m.Slice(d0, PrimitiveType.Word16), m.Slice(d0, PrimitiveType.Word16)));
                m.Assign(CVZN, m.Cond(CVZN.DataType, v17));
                m.Assign(d0, m.Dpb(d0, v17, 0));
                m.Goto(m.IAdd(m.Word32(0x0000A8B4), m.Convert(
                    m.Slice(d0, PrimitiveType.Int16),
                    PrimitiveType.Int16, PrimitiveType.Int32)));
            });

            graph.Nodes.Add(b1);
            graph.Nodes.Add(bRet);
            graph.Nodes.Add(b2);
            graph.AddEdge(b1, bRet);
            graph.AddEdge(b1, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 6, Target(b2)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("CONVERT(v3 * 4<16>, int16, int32) + 0xA8B4<32>", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("v3", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[20,7FFFFFFFFFFFFFFF]", bwslc.JumpTableIndexInterval.ToString());
        }

        /// <summary>
        /// Handle 8051 switch statements.
        /// </summary>
        [Test]
        public void Bwslc_Issue_826()
        {
            var A = binder.EnsureRegister(RegisterStorage.Reg8("A", 0));
            var R7 = binder.EnsureRegister(RegisterStorage.Reg8("R7", 7));
            var DPTR = binder.EnsureRegister(RegisterStorage.Reg16("DPTR", 8));
            var C = Cc("C");
            var b0082 = Given_Block(0x0082);
            Given_Instrs(b0082, m =>
            {
                m.Assign(A, m.Mem(A.DataType, DPTR));
                m.Assign(R7, A);
                m.Assign(A, m.IAdd(A, 0xFC));   // A >= 4 will cause a carry.
                m.Assign(C, m.Cond(C.DataType, A));
                m.Branch(m.Test(ConditionCode.ULT, C), Address.Ptr16(0x00C0));
            });

            var b0088 = Given_Block(0x0088);
            Given_Instrs(b0088, m =>
            {
                m.Assign(A, R7);
                m.Assign(A, m.IAdd(A, R7));
                m.Assign(DPTR, m.Word16(0x008E));
                m.Goto(m.IAdd(DPTR, m.ExtendZ(A, PrimitiveType.UInt16)));
            });
            graph.Nodes.Add(b0082);
            graph.Nodes.Add(b0088);
            graph.AddEdge(b0082, b0088);

            var bwslc = new BackwardSlicer(host, b0088, processorState);
            Assert.IsTrue(bwslc.Start(b0088, 3, Target(b0088)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("CONVERT(R7 * 2<8>, byte, word16) + 0x8E<16>", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("R7", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,3]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_Arm()
        {
            // Follow the ARM pattern:
            //  cmp r1,#4
            //  ldrCC pc,[pc,r1,lsl #2]
            //  goto other
            var C = Cc("C");
            var b1000 = Given_Block(0x1000);
            var r1 = binder.EnsureRegister(RegisterStorage.Reg32("r1", 1));
            Given_Instrs(b1000, m =>
            {
                m.Assign(C, m.Cond(C.DataType, m.ISub(r1, 4)));
                m.Branch(m.Test(ConditionCode.UGT, C), Address.Ptr32(0x1004));
                m.Goto(m.Mem32(m.IAdd(m.Ptr32(0x1020), m.IMul(r1, 4))));
            });
            var bwslc = new BackwardSlicer(host, b1000, processorState);
            Assert.IsTrue(bwslc.Start(b1000, 2, Target(b1000)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("Mem0[0x00001020<p32> + r1 * 4<32>:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("r1", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,4]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_X86_Swap()
        {
            // Problematic xchg instruction 
            //    mov ax,[bp + 6h]
            //    cmp ax,9h
            //    jbe 0648h
            //l0FDC_0645:
            //    jmp 0883h
            //l0FDC_0648:
            //    add ax, ax
            // xchg bx, ax
            //    jmp word ptr cs:[bx+86Fh]
            var ax = binder.EnsureRegister(RegisterStorage.Reg16("ax", 0));
            var bx = binder.EnsureRegister(RegisterStorage.Reg16("bx", 3));
            var bp = binder.EnsureRegister(RegisterStorage.Reg16("bp", 5));
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var flags = RegisterStorage.Reg16("FLAGS", 6);
            var SCZ = binder.EnsureFlagGroup(new FlagGroupStorage(flags, 0x7, "SCZ"));
            var CZ = binder.EnsureFlagGroup(new FlagGroupStorage(flags, 1, "CZ"));

            var b1000 = Given_Block(0x1000);
            var b1010 = Given_Block(0x1010);
            var b1020 = Given_Block(0x1020);
            var b1030 = Given_Block(0x1030);
            Given_Instrs(b1000, m =>
            {
                m.Assign(ax, m.Mem16(m.IAdd(bp, 6)));
            });
            Given_Instrs(b1000, m =>
            {
                m.Assign(SCZ, m.Cond(SCZ.DataType, m.ISub(ax, 9)));
                m.Branch(m.Test(ConditionCode.ULE, CZ), b1020.Address);
            });
            Given_Instrs(b1010, m =>
            {
                m.Goto(b1030.Address);
            });
            Given_Instrs(b1020, m =>
            {
                m.Assign(ax, m.IAdd(ax, ax));
                m.Assign(SCZ, m.Cond(SCZ.DataType, ax));
                m.Assign(tmp, bx);
                m.Assign(bx, ax);
                m.Assign(ax, tmp);
                m.Goto(m.Mem16(m.IAdd(bx, 0x400)));
            });
            graph.Nodes.Add(b1000);
            graph.Nodes.Add(b1010);
            graph.Nodes.Add(b1020);
            graph.AddEdge(b1000, b1010);
            graph.AddEdge(b1000, b1020);

            var bwslc = new BackwardSlicer(host, b1000, processorState);
            Assert.IsTrue(bwslc.Start(b1020, 5, Target(b1020)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("Mem0[ax * 2<16> + 0x400<16>:word16]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("ax", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,9]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("00001004", bwslc.GuardInstrAddress.ToString());
        }

        [Test(Description = "MIPS switches are guarded by have explicit comparisons")]
        public void Bwslc_MipsBranch()
        {
            /*
            00404786 F3FC andi r7,r7,000000FF
            00404788 C8EC 57C7 bgeiuc r7,0000000A,00404752
            0040478C 04C0 DBB0 addiupc r6,00006DD8
            00404790 537F lwxs r7,r7(r6)
            00404792 D8E0 jrc r7
*/

            var r6 = binder.EnsureRegister(RegisterStorage.Reg32("r6", 6));
            var r7 = binder.EnsureRegister(RegisterStorage.Reg32("r7", 7));

            var l1000 = Given_Block(0x1000);
            var l1008 = Given_Block(0x1008);
            var l1020 = Given_Block(0x1020);
            Given_Instrs(l1000,
                m => m.Assign(r7, m.And(r7, 0xFF)),
                m => m.Branch(m.Uge(r7, 10), l1020.Address));
            Given_Instrs(l1008,
                m => m.Assign(r6, m.Word32(0x00123400)),
                m => m.Assign(r7, m.Mem32(m.IAdd(r6, m.IMul(r7, 4)))),
                m => m.Goto(r7));
            graph.Nodes.Add(l1000);
            graph.Nodes.Add(l1008);
            graph.Nodes.Add(l1020);
            graph.AddEdge(l1000, l1008);
            graph.AddEdge(l1000, l1020);

            var bwslc = new BackwardSlicer(host, l1000, processorState);
            Assert.IsTrue(bwslc.Start(l1008, 2, Target(l1008)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("Mem0[r7 * 4<32> + 0x123400<32>:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("r7", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,9]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("00001004", bwslc.GuardInstrAddress.ToString());
        }

        [Test]
        public void Bwslc_AeonSingleBitTest()
        {
            // Aeon has a single condition code bit:

            // bn.sfgtui	r7,0x27
            // bn.bnf?	skip
            // 
            // default_case:
            //    ...
            // 
            // skip:
            // bn.slli?	r7,r7,0x2
            // bg.movhi	r6,0xaddr@hi
            // bg.addi	r6,r6,addr@lo
            // bt.add?	r7,r6
            // bn.lwz	r7,(r7)
            // bt.jr	r7

            var b1000 = this.Given_Block(0x1000);
            var b100C = this.Given_Block(0x100C);
            var b1010 = this.Given_Block(0x1010);
            var r6 = binder.EnsureRegister(RegisterStorage.Reg32("r6", 6));
            var r7 = binder.EnsureRegister(RegisterStorage.Reg32("r7", 7));
            var tmp = binder.CreateTemporary(PrimitiveType.UInt16);
            var sr = binder.EnsureRegister(RegisterStorage.Sysreg("sr", 0, PrimitiveType.Word32));
            var f = binder.EnsureFlagGroup(new FlagGroupStorage((RegisterStorage) sr.Storage, 1 << 9, "f"));

            Given_Instrs(b1000, m =>
                {
                    m.Assign(tmp, m.Slice(r7, PrimitiveType.UInt16));
                    m.Assign(r7, m.Convert(tmp, PrimitiveType.UInt16, PrimitiveType.UInt32));
                },
                m => m.Assign(f, m.Ugt(r7, m.Word32(0x4u))),
                m => m.Branch(m.Not(f), b1010.Address));

            Given_Instrs(b100C, m =>
            {
                m.Return(0, 0);
            });

            Given_Instrs(b1010,
                m => m.Assign(r7, m.Shl(r7, 2)),
                m => m.Assign(r6, m.Word32(0x390000)),
                m => m.Assign(r6, m.Word32(0x38B42C)),
                m => m.Assign(r7, m.IAdd(r7, r6)),
                m => m.Assign(r7, m.Mem32(r7)),
                m => m.Goto(r7));

            graph.Nodes.Add(b1000);
            graph.Nodes.Add(b100C);
            graph.Nodes.Add(b1010);
            graph.AddEdge(b1000, b100C);
            graph.AddEdge(b1000, b1010);

            var bwslc = new BackwardSlicer(host, b1000, processorState);
            Assert.IsTrue(bwslc.Start(b1010, 5, Target(b1010)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("Mem0[(r7 << 2<8>) + 0x38B42C<32>:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("r7", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,4]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("00001004", bwslc.GuardInstrAddress.ToString());
        }

        [Test]
        public void Bwslc_SignedLeComparison()
        {
            /*
            guard:
                bg.lwz	r7,0x34C(r10)
                bg.bltsi?	r7,0x6,_switch

            default:
                ...
            _switch:
                bn.slli?	r7,r7,0x2
                bg.movhi	r6,0x391A42@hi
                bg.addi	r6,r6,0x391A42@lo
                bt.add?	r7,r6
                bn.lwz	r7,(r7)
                bt.jr	r7
            */
            var r6 = binder.EnsureRegister(RegisterStorage.Reg32("r6", 6));
            var r7 = binder.EnsureRegister(RegisterStorage.Reg32("r7", 7));
            var r10 = binder.EnsureRegister(RegisterStorage.Reg32("r10", 10));

            var l1000 = Given_Block(0x1000);
            var l1008 = Given_Block(0x1008);
            var l1010 = Given_Block(0x1010);
            graph.Nodes.Add(l1000);
            graph.Nodes.Add(l1008);
            graph.Nodes.Add(l1010);
            graph.AddEdge(l1000, l1008);
            graph.AddEdge(l1000, l1010);

            Given_Instrs(l1000,
                m => m.Assign(r7, m.Mem32(m.AddSubSignedInt(r10, 844))),
                m => m.Branch(m.Le(r7, 6), l1010.Address));
            Given_Instrs(l1010,
                m => m.Assign(r7, m.Shl(r7, 2)),
                m => m.Assign(r6, m.Word32(0x391AA0)),
                m => m.Assign(r7, m.IAdd(r7, r6)),
                m => m.Assign(r7, m.Mem32(r7)),
                m => m.Goto(r7));

            var bwslc = new BackwardSlicer(host, l1000, processorState);
            Assert.IsTrue(bwslc.Start(l1010, 4, Target(l1010)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("Mem0[(r7 << 2<8>) + 0x391AA0<32>:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("r7", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,6]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("00001004", bwslc.GuardInstrAddress.ToString());
        }

        [Test]
        public void Bwslc_issue_958()
        {
            // A RiscV switch statement:
            //      c.li a5,00000012
            //      bleu a5,s7,000000002309B2C2
            //      jal zero,000000002309CE8C
            //
            //      lui a4,000230CB
            //      slli a5,s7,00000002
            //      addi a4,a4,-00000744
            //      c.add a5,a4
            //      c.lw a5,0(a5)
            //      c.jr a5
            var a4 = binder.EnsureRegister(RegisterStorage.Reg32("a4", 0x0E));
            var a5 = binder.EnsureRegister(RegisterStorage.Reg32("a5", 0x0F));
            var s7 = binder.EnsureRegister(RegisterStorage.Reg32("s7", 0x17));

            var l1000 = Given_Block(0x1000);
            var l1008 = Given_Block(0x1008);
            var l100C = Given_Block(0x100C);
            var l1010 = Given_Block(0x1010);
            graph.Nodes.Add(l1000);
            graph.Nodes.Add(l1008);
            graph.Nodes.Add(l1010);
            graph.AddEdge(l1000, l1008);
            graph.AddEdge(l1000, l1010);

            Given_Instrs(l1000,
                m => m.Assign(a5, 4),
                m => m.Branch(m.Ule(a5, s7), l100C.Address));
            Given_Instrs(l1008,
                m => m.Return(0, 0));
            Given_Instrs(l1010,
                m => m.Assign(a4, m.Word32(0x2000)),
                m => m.Assign(a5, m.Shl(s7, 2)),
                m => m.Assign(a4, m.IAddS(a4, -0x800)),
                m => m.Assign(a5, m.IAdd(a5, a4)),
                m => m.Assign(a5, m.Mem32(a5)),
                m => m.Goto(a5));

            var bwslc = new BackwardSlicer(host, l1000, processorState);
            Assert.IsTrue(bwslc.Start(l1010, 4, Target(l1010)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("Mem0[(s7 << 2<8>) + 0x1800<32>:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("s7", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,3]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("00001004", bwslc.GuardInstrAddress.ToString());
        }

        [Test]
        public void Bwslc_Regression_00001()
        {
            // A switch statement from PE/m68k/hello

            //00001736 E788 lsl.l #$03,d0
            //00001738 D0AE FFF8 add.l -$0008(a6),d0
            //0000173C 1033 0800 move.b(a3, d0),d0
            //00001740 E800 asr.b #$04,d0
            //00001742 49C0 extb.l d0
            //00001744 2D40 FFF8 move.l d0,-$0008(a6)
            //00001748 7407 moveq #$07,d2
            //0000174A B480 cmp.l d0, d2
            //0000174C 6500 04D6 bcs $00001C24

            //00001750 303B 0206 move.w($08, pc, d0.w * 2),d0
            //00001754 4EFB 0002 jmp.l($04, pc, d0.w)

            var b1736 = Given_Block(0x1736);
            var b1C24 = Given_Block(0x1C24);
            var b1750 = Given_Block(0x1750);
            graph.Nodes.Add(b1736);
            graph.Nodes.Add(b1C24);
            graph.Nodes.Add(b1750);
            graph.AddEdge(b1736, b1750);
            graph.AddEdge(b1736, b1C24);

            var d0 = binder.EnsureRegister(RegisterStorage.Reg32("d0", 0));
            var d2 = binder.EnsureRegister(RegisterStorage.Reg32("d2", 2));
            var a3 = binder.EnsureRegister(RegisterStorage.Reg32("a3", 11));
            var a6 = binder.EnsureRegister(RegisterStorage.Reg32("a6", 14));
            var cr = RegisterStorage.Reg32("cr", 0x42);
            var CVZNX = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x1F, "CVZNX"));
            var CVZN = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x1E, "CVZN"));
            var ZN = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x6, "ZN"));
            var C = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x10, "C"));
            var V = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x08, "V"));
            var v35 = binder.CreateTemporary("v35", PrimitiveType.Byte);
            var v36 = binder.CreateTemporary("v36", word24);
            var v37 = binder.CreateTemporary("v37", PrimitiveType.Byte);
            var v38 = binder.CreateTemporary("v38", v36.DataType);
            var v40 = binder.CreateTemporary("v40", PrimitiveType.Word32);
            var v41 = binder.CreateTemporary("v41", PrimitiveType.Word16);
            var v42 = binder.CreateTemporary("v42", PrimitiveType.Word16);
            Given_Instrs(b1736,
                m =>
                {
                    m.Assign(d0, m.Shl(d0, 3));
                    m.Assign(CVZNX, m.Cond(CVZNX.DataType, d0));
                },
                m =>
                {
                    m.Assign(d0, m.IAdd(d0, m.Mem32(m.IAdd(a6, m.Int32(-8)))));
                    m.Assign(CVZNX, m.Cond(CVZNX.DataType, d0));
                },
                m =>
                {
                    m.Assign(v35, m.Mem8(m.IAdd(a3, d0)));
                    m.Assign(v36, m.Slice(d0, v36.DataType, 8));
                    m.Assign(d0, m.Seq(v36, v35));
                    m.Assign(ZN, m.Cond(ZN.DataType, v35));
                    m.Assign(C, m.False());
                    m.Assign(V, m.False());
                },
                m =>
                {
                    m.Assign(v37, m.Shr(m.Slice(d0, v37.DataType, 0), 0x04));
                    m.Assign(v38, m.Slice(d0, v38.DataType, 8));
                    m.Assign(d0, m.Seq(v38, v37));
                    m.Assign(CVZNX, m.Cond(CVZNX.DataType, v37));
                },
                m =>
                {
                    m.Assign(d0, m.Convert(m.Slice(d0, PrimitiveType.Int8, 0), PrimitiveType.Int8, PrimitiveType.Int32));
                    m.Assign(ZN, m.Cond(ZN.DataType, d0));
                },
                m =>
                {
                    m.Assign(m.Mem32(m.IAdd(a6, m.Int32(-8))), d0);
                    m.Assign(ZN, m.Cond(ZN.DataType, d0));
                    m.Assign(C, m.False());
                    m.Assign(V, m.False());
                },
                m =>
                {
                    m.Assign(d2, 7);
                    m.Assign(ZN, m.Cond(ZN.DataType, d2));
                    m.Assign(C, m.False()); ;
                    m.Assign(V, m.False()); ;
                },
                m =>
                {

                    m.Assign(v40, m.ISub(d2, d0));
                    m.Assign(CVZN, m.Cond(CVZN.DataType, v40));
                },
                m =>
                {
                    m.Branch(m.Test(ConditionCode.ULT, C), b1C24.Address);
                });

            Given_Instrs(b1750,
                m =>
                {

                    m.Assign(v41, m.Mem16(m.IAdd(Address.Ptr32(0x00001758), m.IMul(m.Convert(m.Slice(d0, PrimitiveType.Int16, 0), PrimitiveType.Int16, PrimitiveType.Int32), 2))));
                    m.Assign(v42, m.Slice(d0, v42.DataType, 16));
                    m.Assign(d0, m.Seq(v42, v41));
                    m.Assign(ZN, m.Cond(ZN.DataType, v41));
                    m.Assign(C, m.False());
                    m.Assign(V, m.False());
                },
                m =>
                {
                    m.Goto(m.IAdd(Address.Ptr32(0x000017580), m.Convert(m.Slice(d0, PrimitiveType.Int16, 0), PrimitiveType.Int16, PrimitiveType.Int32)));
                });

            var bwslc = new BackwardSlicer(host, b1750, processorState);
            Assert.IsTrue(bwslc.Start(b1750, 4, Target(b1750)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("0x00017580<p32> + CONVERT(Mem0[0x00001758<p32> + CONVERT(SLICE(d0, int16, 0), int16, int32) * 2<i32>:word16], int16, int32)", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("d0", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,6]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("00001752", bwslc.GuardInstrAddress.ToString());
        }

        [Test]
        public void Bwslc_Regression_00002()
        {
            /*
             * 
00101408 1028 0001 move.b $0001(a0),d0
0010140C 0400 0041 subi.b #$41,d0
00101410 6B00 0170 bmi $00101582
00101414 0C00 0017 cmpi.b #$17,d0
00101418 6E00 0168 bgt $00101582
0010141C 7200 moveq #$00,d1
0010141E 1200 move.b d0,d1
00101420 D241 add.w d1,d1
00101422 323B 1006 move.w ($08,pc,d1.w),d1
00101426 4EFB 1000 jmp.l ($02,pc,d1.w)


                v69 = Mem0[a0 + 1:byte]
                v70 = SLICE(d0, word24, 8)
                d0 = SEQ(v70, v69)
                ZN = cond(v69)
                C = false
                V = false
                v71 = SLICE(d0, byte, 0) - 0x41
                v72 = SLICE(d0, word24, 8)
                d0 = SEQ(v72, v71)
                CVZNX = cond(v71)
                branch Test(LT,N) l00101582
            l00101414:
                v73 = SLICE(d0, byte, 0) - 0x17
                CVZN = cond(v73)
                branch Test(GT,VZN) l00101582
            l0010141C:
                d1 = 0
                ZN = cond(d1)
                C = false
                V = false
                v75 = SLICE(d0, byte, 0)
                v76 = SLICE(d1, word24, 8)
                d1 = SEQ(v76, v75)
                ZN = cond(v75)
                C = false
                V = false
                v77 = SLICE(d1, word16, 0) + SLICE(d1, word16, 0)
                v78 = SLICE(d1, word16, 16)
                d1 = SEQ(v78, v77)
                CVZNX = cond(v77)
                v79 = Mem0[0x0010142A<p32> + CONVERT(SLICE(d1, int16, 0), int16, int32):word16]
                v80 = SLICE(d1, word16, 16)
                d1 = SEQ(v80, v79)
                ZN = cond(v79)
                C = false
                V = false
                call 0x00101428<p32> + CONVERT(SLICE(d1, int16, 0), int16, int32)
            */

            var b101408 = Given_Block(0x00101408);
            var b101414 = Given_Block(0x00101414);
            var b10141C = Given_Block(0x0010141C);
            var b101582 = Given_Block(0x00101582);
            graph.Nodes.Add(b101408);
            graph.Nodes.Add(b101414);
            graph.Nodes.Add(b10141C);
            graph.Nodes.Add(b101582);
            graph.AddEdge(b101408, b101414);
            graph.AddEdge(b101408, b101582);
            graph.AddEdge(b101414, b10141C);
            graph.AddEdge(b101414, b101582);

            var v69 = binder.CreateTemporary(PrimitiveType.Byte);
            var v70 = binder.CreateTemporary(word24);
            var v71 = binder.CreateTemporary(PrimitiveType.Byte);
            var v72 = binder.CreateTemporary(word24);
            var v73 = binder.CreateTemporary(PrimitiveType.Byte);
            var v75 = binder.CreateTemporary(PrimitiveType.Byte);
            var v76 = binder.CreateTemporary(word24);
            var v77 = binder.CreateTemporary(PrimitiveType.Word16);
            var v78 = binder.CreateTemporary(PrimitiveType.Word16);
            var v79 = binder.CreateTemporary(PrimitiveType.Word16);
            var v80 = binder.CreateTemporary(PrimitiveType.Word16);
            var a0 = binder.EnsureRegister(RegisterStorage.Reg32("a0", 8));
            var d0 = binder.EnsureRegister(RegisterStorage.Reg32("d0", 0));
            var d1 = binder.EnsureRegister(RegisterStorage.Reg32("d1", 1));
            var cr = RegisterStorage.Reg32("cr", 0x42);
            var CVZNX = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x1F, "CVZNX"));
            var CVZN = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x1E, "CVZN"));
            var VZN = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0xE, "VZN"));
            var ZN = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x6, "ZN"));
            var N = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x4, "N"));
            var C = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x10, "C"));
            var V = binder.EnsureFlagGroup(new FlagGroupStorage(cr, 0x08, "V"));
            var word16 = PrimitiveType.Word16;
            var int16 = PrimitiveType.Int16;
            var int32 = PrimitiveType.Int32;

            Given_Instrs(b101408,
                m =>
                {
                    m.Assign(v69, m.Mem8(m.IAdd(a0, 1)));
                    m.Assign(v70, m.Slice(d0, word24, 8));
                    m.Assign(v71, m.ISub(m.Slice(d0, PrimitiveType.Byte, 0), 0x41));
                    m.Assign(v72, m.Slice(d0, word24, 8));
                    m.Assign(d0, m.Seq(v72, v71));
                    m.Assign(CVZNX, m.Cond(CVZNX.DataType, v71));
                },
                m => m.Branch(m.Test(ConditionCode.LT, N), b101582.Address)
                );
            Given_Instrs(b101414,
                m =>
                {
                    m.Assign(v73, m.ISub(m.Slice(d0, PrimitiveType.Byte, 0), 0x17));
                    m.Assign(CVZN, m.Cond(CVZN.DataType, v73));
                },
                m => m.Branch(m.Test(ConditionCode.GT, VZN), b101582.Address));

            Given_Instrs(b10141C,
                    m =>
                    {
                        m.Assign(d1, 0);
                        m.Assign(ZN, m.Cond( ZN.DataType, d1));
                        m.Assign(C, m.False());
                        m.Assign(V, m.False());
                    },
                    m =>
                    {
                        m.Assign(v75, m.Slice(d0, PrimitiveType.Byte, 0));
                        m.Assign(v76, m.Slice(d1, word24, 8));
                        m.Assign(d1, m.Seq(v76, v75));
                        m.Assign(ZN, m.Cond(ZN.DataType, v75));
                        m.Assign(C, m.False());
                        m.Assign(V, m.False());
                    },
                    m =>
                    {
                        m.Assign(v77, m.IAdd(m.Slice(d1, word16, 0), m.Slice(d1, word16, 0)));
                        m.Assign(v78, m.Slice(d1, word16, 16));
                        m.Assign(d1, m.Seq(v78, v77));
                        m.Assign(CVZNX, m.Cond(CVZNX.DataType, v77));
                    },
                    m =>
                    {
                        m.Assign(v79, m.Mem16(m.IAdd(Address.Ptr32(0x0010142A), m.Convert(m.Slice(d1, int16, 0), int16, int32))));
                        m.Assign(v80, m.Slice(d1, word16, 16));
                        m.Assign(d1, m.Seq(v80, v79));
                        m.Assign(ZN, m.Cond(ZN.DataType, v79));
                        m.Assign(C, m.False());
                        m.Assign(V, m.False());
                    },
                    m => m.Goto(m.IAdd(Address.Ptr32(0x0101428), m.Convert(m.Slice(d1, int16, 0), int16, int32))));

            var bwslc = new BackwardSlicer(host, b10141C, processorState);
            Assert.IsTrue(bwslc.Start(b10141C, 20, Target(b10141C)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("0x00101428<p32> + CONVERT(Mem0[0x0010142A<p32> + CONVERT(CONVERT(SLICE(d0, byte, 0), uint8, word16) * 2<16>, int16, int32):word16], int16, int32)", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("d0", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("1[0,17]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("00101414", bwslc.GuardInstrAddress.ToString());
        }
        /*
0FFE9EF4 E8 E7 FF FF bg.lhz? r7,-0x2(r7)
0FFE9EF8 1D 9C E1 bn.addi r12,r28,-0x1F
0FFE9EFB 0C A3 02 bn.lwz r5,(r3)
0FFE9EFE 50 80 2B bn.ori r4,r0,0x2B
0FFE9F01 0C C1 04 bn.sw 0x4(r1),r6
0FFE9F04 0C E1 00 bn.sw (r1),r7
0FFE9F07 84 A8 bt.jalr? r5
0FFE9F09 0C E1 02 bn.lwz r7,(r1)
0FFE9F0C 0C C1 06 bn.lwz r6,0x4(r1)
0FFE9F0F E7 FF FF 23 bg.j 0FFE9EA0
0FFE9F13 9C E1 bt.addi? r7,0x1
0FFE9F15 0C A3 02 bn.lwz r5,(r3)
0FFE9F18 50 80 2D bn.ori r4,r0,0x2D
0FFE9F1B 0C C1 04 bn.sw 0x4(r1),r6
0FFE9F1E 0C E1 00 bn.sw (r1),r7
0FFE9F21 84 A8 bt.jalr? r5
0FFE9F23 0C C1 06 bn.lwz r6,0x4(r1)
0FFE9F26 0C E1 02 bn.lwz r7,(r1)
0FFE9F29 E7 FF FE EF bg.j 0FFE9EA0
0FFE9F2D 88 6B bt.mov? r3,r11
0FFE9F2F 0C EB 02 bn.lwz r7,(r11)
0FFE9F32 50 80 2D bn.ori r4,r0,0x2D
0FFE9F35 84 E8 bt.jalr? r7
0FFE9F37 E7 FF FE 99 bg.j 0FFE9E83
0FFE9F3B 0C E3 02 bn.lwz r7,(r3)
0FFE9F3E 50 80 2D bn.ori r4,r0,0x2D
0FFE9F41 84 E8 bt.jalr? r7
        */

        [Test]
        public void Bwslc_issue_1338()
        {
            /*
            00000000004011F7 8B 55 F8 mov edx,[rbp-8h]
            00000000004011FA 48 89 D0 mov rax,rdx
            00000000004011FD 48 83 E8 02 sub rax,2h
            0000000000401201 77 2B ja 40122Eh

            0000000000401203 48 8D 0D FA 0D 00 00 lea rcx,[0000000000402004]                                                           ; [rip+00000DFA]
            000000000040120A 48 63 04 91 movsxd rax,dword ptr [rcx+rdx*4]
            000000000040120E 48 01 C8 add rax,rcx
            0000000000401211 FF E0 jmp rax

            00000000004011F7 edx = Mem0[rbp - 8<i64>:word32]
            00000000004011F7 rdx = CONVERT(edx, word32, uint64)
            00000000004011FA rax = rdx
            00000000004011FD rax = rax - 2<64>
            00000000004011FD SCZO = cond(rax)
            0000000000401201 if (Test(UGT,CZ)) branch 000000000040122E
                             
            0000000000401203 rcx = 0000000000402004
            000000000040120A rax = CONVERT(Mem0[rcx + rdx * 4<64>:word32], word32, int64)
            000000000040120E rax = rax + rcx
            000000000040120E SCZO = cond(rax)
            0000000000401211 goto rax


             */
            var b4011F7 = Given_Block(0x4011F7);
            var b401203 = Given_Block(0x401203);
            var b40122E = Given_Block(0x40122E);

            graph.Nodes.Add(b4011F7);
            graph.Nodes.Add(b401203);
            graph.Nodes.Add(b40122E);
            graph.AddEdge(b4011F7, b401203);
            graph.AddEdge(b4011F7, b40122E);

            var rax = binder.EnsureRegister(RegisterStorage.Reg64("rax", 0));
            var rcx = binder.EnsureRegister(RegisterStorage.Reg64("rcx", 1));
            var rdx = binder.EnsureRegister(RegisterStorage.Reg64("rdx", 2));
            var rbp = binder.EnsureRegister(RegisterStorage.Reg64("rbp", 5));
            var edx = binder.EnsureRegister(RegisterStorage.Reg32("edx", 2));
            var flags = RegisterStorage.Reg32("eflags", 0x42);
            var SCZO = binder.EnsureFlagGroup(new FlagGroupStorage(flags, 0xF, "SZCO"));
            var CZ = binder.EnsureFlagGroup(new FlagGroupStorage(flags, 0x6, "CZ"));
            var word32 = PrimitiveType.Word32;
            var uint64 = PrimitiveType.UInt64;
            var int64 = PrimitiveType.Int64;
            Given_Instrs(b4011F7,
                m =>
                {
                    m.Assign(edx, m.Mem32(m.ISub(rbp, 8)));
                    m.Assign(rdx, m.Convert(edx, word32, uint64));
                },
                m => m.Assign(rax, rdx),
                m =>
                {
                    m.Assign(rax, m.ISub(rax, 2));
                    m.Assign(SCZO, m.Cond(SCZO.DataType, rax));
                },
                m => m.Branch(m.Test(ConditionCode.UGT, CZ), b40122E.Address)
                );
            Given_Instrs(b401203,
                m => m.Assign(rcx, m.Word64(0x0000000000402004)),
                m => m.Assign(rax, m.Convert(m.Mem32(m.IAdd(rcx, m.IMul(rdx, 4))), word32, int64)),
                m =>
                {
                    m.Assign(rax, m.IAdd(rax, rcx));
                    m.Assign(SCZO, m.Cond(SCZO.DataType, rax));
                },
                m => m.Goto(rax));

            var bwslc = new BackwardSlicer(host, b401203, processorState);
            Assert.IsTrue(bwslc.Start(b401203, 4, Target(b401203)));
            while (bwslc.Step())
                ;
            Assert.AreEqual(
                "CONVERT(Mem0[rdx * 4<64> + 0x402004<64>:word32], word32, int64) + 0x402004<64>",
                bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("rdx", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("rax", bwslc.JumpTableIndexToUse.ToString());
            Assert.AreEqual("1[0,2]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("004011FB", bwslc.GuardInstrAddress.ToString());
        }
        /*

    [Test]
    public void Bwslc_RiscV_64bit()
    {
         * 
0000000000015214 F0EF DDDF jal getopt_long
0000000000015218 0713 FFF0 li a4,-0x1
000000000001521C 0793 0005 mv a5,a0
0000000000015220 0463 12E5 beq a0,a4,0x0000000000015348

0000000000015224 0713 0760 li a4,0x76
0000000000015228 6CE3 FCA7 bltu a4,a0,0x0000000000015200

000000000001522C 9793 0207 slli a5,a5,0x20
0000000000015230 0737 0001 lui a4,0x10
0000000000015234 D793 01E7 srli a5,a5,0x1E
0000000000015238 0713 2A07 addi a4,a4,0x2A0
000000000001523C 87B3 00E7 add a5,a5,a4
0000000000015240 A783 0007 lw a5,(a5)
0000000000015244 8067 0007 jalr zero,a5,0x0
*
call get_optt
a4 = -1
a5 = a0
branch a0 == a4 l0000000000015348
l0000000000015224:
a4 = 118
branch a4 <u a0 l0000000000015200
l000000000001522C:
a5 = a5 << 0x20
a4 = 0x00010000
a5 = a5 >>u 0x1E
a4 = a4 + 672
a5 = a5 + a4
a5 = CONVERT(Mem0[a5:int32], int32, int64)
call a5 (retsize: 0;)

           var l15214 = Given_Block(0x0000000000015214ul);
            var l15224 = Given_Block(0x0000000000015224ul);
            var l1522C = Given_Block(0x000000000001522Cul);
            var bye = Given_Block(0x0000000000015200ul);

            graph.Nodes.Add(l15214);
            graph.Nodes.Add(l15224);
            graph.Nodes.Add(l1522C);
            graph.Nodes.Add(bye);

            graph.AddEdge(l15214, l15224);
            graph.AddEdge(l15214, bye);
            graph.AddEdge(l15224, l1522C);
            graph.AddEdge(l15224, bye);

            
            var a0 = binder.EnsureRegister(RegisterStorage.Reg64("a0", 0));
            var a4 = binder.EnsureRegister(RegisterStorage.Reg64("a4", 4));
            var a5 = binder.EnsureRegister(RegisterStorage.Reg64("a5", 5));


            Given_Instrs(l15214,
                m => m.Call(Address.Ptr64(0x424242), 0),
                m => m.Assign(a4, -1),
                m => m.Assign(a5, a0),
                m => m.Branch(m.Eq(a0, a4), bye.Address));
            Given_Instrs(l15224,
                m => m.Assign(a4, 0x5),
                m => m.Branch(m.Ult(a4, a0), bye.Address));
            Given_Instrs(l1522C,
                m => m.Assign(a5, m.Shl(a5, 0x20)),
                m => m.Assign(a4, 0x00010000),
                m => m.Assign(a5, m.Shr(a5, 0x1E)),
                m => m.Assign(a4, m.IAdd(a4, 672)),
                m => m.Assign(a5, m.IAdd(a5, a4)),
                m => m.Assign(a5, m.Convert(
                    m.Mem(
                    PrimitiveType.Int32, a5),
                    PrimitiveType.Int32,
                    PrimitiveType.Int64)),
                m => m.Goto(a5));

            var bwslc = new BackwardSlicer(host, l1522C, processorState);
            Assert.IsTrue(bwslc.Start(l1522C, 4, Target(l1522C)));
            while (bwslc.Step())
                ;
            Assert.AreEqual(
                "(a5 << 0x20<8> >>u 0x1E<8>) + 0x102A0<64>",
                bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("a0", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("a0", bwslc.JumpTableIndexToUse.ToString());
            Assert.AreEqual("1[0,2]", bwslc.JumpTableIndexInterval.ToString());
            Assert.AreEqual("004011FB", bwslc.GuardInstrAddress.ToString());
        }
    */
    }
    // Test cases
    // A one-level jump table from MySQL. JTT represents the jump table.
    // mov ebp,[rsp + 0xf8]         : 0 ≤ rdx==[rsp+0xf8]==ebp≤ 5
    // cmp ebp, 5                   : 0 ≤%ebp≤ 5 
    // ja 43a4ab 
    // lea rax,[0x525e8f + rip]             : rax = 0x9602a0, rcx = 0x43a116
    // lea rcx,[rip -0x302]                 : JTT = 0x43a116 + [0x9602a0 + rdx×8]       : rax, [rsp 
    // movsx rdx,dword ptr [rsp + 0xF8]     : rdx==[rsp+0xf8]                           : raxl, [rsp + f8], rcx 
    // add rcx, [rax + rdx*8]               : JTT = rcx + [rax + rdx×8]                 : rax, rdx, rcx
    // jmp rcx                              : JTT = rcx                                 : ecx

    // cmp eax,0xa9                         :  0 ≤ eax  ≤ 0xa9                          : eax [0, 0xAA)
    // ja 0x41677e                          :                                           : ecx, eax, CZ
    // movzx ecx,byte ptr [0x416bd4 + eax]  : ecx = [0x416bd4 + eax]                    : eax
    //                                        JTT = [0x416bc0 + [0x416bd4 + eax] × 4]
    // jmp [0x416bc0 + ecx *4]              : JTT = [0x416bc0+ecx×4]                    : ecx


    // A one-level jump table.
    // The input upper bound to this jump table must be inferred.
    // In addition, the input is right shifted to get the index into the table

    // movzx eax,byte ptr [edi]     : 0 ≤ eax ≤ 255                                     : rax [0, 255]
    // shr al,4                     : rax = rax >> 4                                    : al ~ rax
    //                              : JTT = [0x495e30 + (rax >> 4)×8]                   :
    // jmpq [0x495e30 + rax * 8]    : JTT = [0x495e30 + rax×8]                          : rax



    // Unoptimized x86 code
    // cmp dword ptr [ebp + 8], 0xA : 0 <= [ebp + 8] <= 0xA                     : [ebp + 8] [0, 0x0A]
    // ja default                   :                                           : [ebp + 8], CZ
    // movzx edx, byte ptr[ebp + 8] : edx = ZEX([(ebp + 8)], 8)                 : [ebp + 8]
    //                              : JTT = [0x023450 + ZEX([ebp + 8)], 8) * 4] :
    // jmp [0x00234500<32> + edx * 4]   : JTT = [0x0023450 + edx * 4]               : edx

    // M68k relative jump code.
    //  corresponds to
    // cmpi.l #$00000028,d1         : 0 <= d1 <= 0x28
    // bgt $00106C66
    // add.l d1,d1                  : d1 = d1 * 2
    //                                JTT = 0x0010000 + SEXT:([0x10006 + d1*2],16)
    // move.w (06,pc,d1),d1         : JTT = 0x0010000 + SEXT:([0x10006 + d1],16)
    // jmp.l (pc,d1.w)              : JTT = 0x0010000 + SEXT(d1, 16)

    //  m.Assign(d1, m.IAdd(d1, d1));
    //  m.Assign(CVZNX, m.Cond(CVZNX.DataType, d1));
    //  m.Assign(v82,m.LoadW(m.IAdd(m.Word32(0x001066A4), d1)));
    //  m.Assign(d1, m.Dpb(d1, v82, 0));
    //  m.Assign(CVZN, m.Cond(CVZN.DataType, v82));
    //  var block = m.CurrentBlock;
    //  var xfer = new RtlGoto(
    //      m.IAdd(
    //          m.Word32(0x001066A2), 
    //          m.Cast(PrimitiveType.Int32, m.Cast(PrimitiveType.Int16, d1))),
    //      InstrClass.Transfer);

    // cmp [ebp-66],1D
    // mov edx,[ebp-66]
    // movzx eax,byte ptr [edx + 0x10000]
    // jmp [eax + 0x12000]
}

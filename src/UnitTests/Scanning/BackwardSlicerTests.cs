#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Rtl;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.UnitTests.Mocks;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class BackwardSlicerTests
    {
        private StorageBinder binder;
        private IProcessorArchitecture arch;
        private FakeArchitecture fakeArch;
        private RtlBackwalkHost host;
        private Program program;
        private DirectedGraph<RtlBlock> graph;
        private ProcessorState processorState;

        [SetUp]
        public void Setup()
        {
            fakeArch = new FakeArchitecture();
            arch = fakeArch;
            program = new Program {
                Architecture = arch,
                SegmentMap = new SegmentMap(
                    Address.Ptr32(0x00120000),
                    new ImageSegment(
                        ".text",
                        new MemoryArea(Address.Ptr32(0x00120000), new byte[0x10000]),
                        AccessMode.ReadExecute))
            };
            binder = new StorageBinder();
            graph = new DiGraph<RtlBlock>();
            host = new RtlBackwalkHost(program, graph);
            processorState = arch.CreateProcessorState();
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

        private RtlBlock Given_Block(uint uAddr)
        {
            var b = new RtlBlock(Address.Ptr32(uAddr), $"l{uAddr:X8}");
            return b;
        }

        private RtlBlock Given_Block(ushort uSeg, ushort uOffset)
        {
            var b = new RtlBlock(Address.SegPtr(uSeg, uOffset), $"l{uSeg:X4}_{uOffset:X4}");
            return b;
        }

        private void Given_Instrs(RtlBlock block, Action<RtlEmitter> b)
        {
            var instrs = new List<RtlInstruction>();
            var trace = new RtlEmitter(instrs);
            b(trace);
            block.Instructions.Add(
                new RtlInstructionCluster(
                    block.Address + block.Instructions.Count * 4,
                    4,
                    instrs.ToArray()));
        }

        private Expression Target(RtlBlock block)
        {
            return ((RtlGoto)block.Instructions.Last().Instructions.Last()).Target;
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
            Given_Instrs(b, m => { m.Assign(cz, m.Cond(m.ISub(r2, 4))); });
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
            Assert.AreEqual("(r2 << 0x02) + 0x00123400", bwslc.JumpTableFormat.ToString());
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
            Given_Instrs(b, m => { m.Assign(r1, m.And(r1, 7)); m.Assign(cz, m.Cond(r1)); });
            Given_Instrs(b, m => { m.Goto(m.Mem32(m.IAdd(m.Word32(0x00123400), m.IMul(r1, 4)))); });

            graph.Nodes.Add(b);

            var bwslc = new BackwardSlicer(host, b, processorState);
            Assert.IsTrue(bwslc.Start(b, 1, Target(b)));   // indirect jump
            Assert.IsTrue(bwslc.Step());    // assign flags
            Assert.IsFalse(bwslc.Step());    // and
            Assert.AreEqual("Mem0[(r1 & 0x00000007) * 0x00000004 + 0x00123400:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,7]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Bwslc_x86_RegisterHack()
        {
            // In old x86 binaries we see this mechanism
            // for zero extending a register.
            arch = new Reko.Arch.X86.X86ArchitectureReal("x86-real-16");
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
                m.Assign(SCZO, m.Cond(m.ISub(bl, 2)));
            });
            Given_Instrs(b, m => {
                m.Branch(new TestCondition(ConditionCode.UGT, SCZO), Address.Ptr16(0x120), InstrClass.ConditionalTransfer);
            });

            var b2 = Given_Block(0x200);
            Given_Instrs(b2, m =>
            {
                m.Assign(bh, m.Xor(bh, bh));
                m.Assign(SCZO, new ConditionOf(bh));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(bx, m.IAdd(bx, bx));
                m.Assign(SCZO, new ConditionOf(bx));
            });
            Given_Instrs(b2, m => {
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

            Assert.AreEqual("Mem0[(word16) (byte) bx * 0x0002 + 0x8400:word16]", bwslc.JumpTableFormat.ToString());
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

            arch = new Reko.Arch.X86.X86ArchitectureReal("x86-real-16");
            var ecx = binder.EnsureRegister(arch.GetRegister("ecx"));
            var edx = binder.EnsureRegister(arch.GetRegister("edx"));
            var esi = binder.EnsureRegister(arch.GetRegister("esi"));
            var edi = binder.EnsureRegister(arch.GetRegister("edi"));
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup("C"));
            var SZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SZO"));
            var SCZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SCZO"));
            var tmp = binder.CreateTemporary(ecx.DataType);

            var b = Given_Block(0x1000);
            Given_Instrs(b, m => { m.Assign(ecx, m.Shr(ecx, 2)); m.Assign(SCZO, m.Cond(ecx)); });
            Given_Instrs(b, m => { m.Assign(edx, m.And(edx, 3)); m.Assign(SZO, m.Cond(edx)); m.Assign(C, Constant.False()); });
            Given_Instrs(b, m => { m.Assign(SCZO, m.Cond(m.ISub(ecx, 8))); });
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.ULT, C), Address.Ptr32(0x2000), InstrClass.ConditionalTransfer); });

            var b2 = Given_Block(0x1008);
            Given_Instrs(b2, m => {
                m.BranchInMiddleOfInstruction(m.Eq0(ecx), Address.Ptr32(0x1010), InstrClass.ConditionalTransfer);
                m.Assign(tmp, m.Mem32(esi));
                m.Assign(m.Mem32(edi), tmp);
                m.Assign(esi, m.IAdd(esi, 4));
                m.Assign(edi, m.IAdd(edi, 4));
                m.Assign(ecx, m.ISub(ecx, 1));
                m.Goto(Address.Ptr32(0x1008));
            });

            var b3 = Given_Block(0x1010);
            Given_Instrs(b3, m => {
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
            Assert.AreEqual("Mem0[(edx & 0x00000003) * 0x00000004 + 0x00123400:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,3]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_SegmentedLoad()
        {
            arch = new Reko.Arch.X86.X86ArchitectureReal("x86-real-16");
            var cx = binder.EnsureRegister(arch.GetRegister("cx"));
            var bx = binder.EnsureRegister(arch.GetRegister("bx"));
            var ds = binder.EnsureRegister(arch.GetRegister("ds"));
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup("C"));
            var SZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SZO"));
            var SCZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SCZO"));

            var b = Given_Block(0x0C00, 0x0100);
            Given_Instrs(b, m => { m.Assign(SCZO, m.Cond(m.ISub(bx, 15))); });
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.UGT, C), Address.SegPtr(0xC00, 0x200), InstrClass.ConditionalTransfer); });

            var b2 = Given_Block(0x0C00, 0x0108);
            Given_Instrs(b2, m => { m.Assign(bx, m.IAdd(bx, bx)); m.Assign(SCZO, m.Cond(bx)); });
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

            Assert.AreEqual("Mem0[ds:bx * 0x0002 + 0x0022:ptr32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,F]", bwslc.JumpTableIndexInterval.ToString());

        }

        [Test]
        public void Bwslc_ClearingBits()
        {
            arch = new Reko.Arch.X86.X86ArchitectureReal("x86-real-16");
            var eax = binder.EnsureRegister(arch.GetRegister("eax"));
            var edx = binder.EnsureRegister(arch.GetRegister("edx"));
            var dl = binder.EnsureRegister(arch.GetRegister("dl"));
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup("C"));
            var SZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SZO"));
            var SCZO = binder.EnsureFlagGroup(arch.GetFlagGroup("SCZO"));

            var b = Given_Block(0x001000000);
            Given_Instrs(b, m => { m.Assign(SCZO, m.Cond(m.ISub(eax, 3))); });
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.UGT, C), Address.Ptr32(0x00100010), InstrClass.ConditionalTransfer); });

            var b2 = Given_Block(0x001000008);
            Given_Instrs(b2, m => { m.Assign(edx, m.Xor(edx, edx)); m.Assign(SZO, m.Cond(edx)); m.Assign(C, Constant.False()); });
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

            Assert.AreEqual("Mem0[Mem0[eax + 0x00123500:byte] * 0x00000004 + 0x00123400:word32]", bwslc.JumpTableFormat.ToString());
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
            Assert.AreEqual("r1 * 0x00000002 + 0x00123400", bwslc.JumpTableFormat.ToString());
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
            Assert.AreEqual("r1 * 0x00000004 + 0x00123400", bwslc.JumpTableFormat.ToString());
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
            arch = new Reko.Arch.M68k.M68kArchitecture("m68k");
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
                m.Assign(v2, m.ISub(m.Cast(PrimitiveType.Byte, d0), 0x17));
                m.Assign(CVZN, m.Cond(v2));
            });
            Given_Instrs(b, m =>
            {
                m.Branch(m.Test(ConditionCode.UGT, CZ), Address.Ptr32(0x00100040), InstrClass.ConditionalTransfer);
            });

            var b2 = Given_Block(0x00100008);
            Given_Instrs(b2, m => {
                m.Assign(d1, m.Word32(0));
                m.Assign(CVZN, m.Cond(d1));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v3, m.Cast(v3.DataType, d0));
                m.Assign(d1, m.Dpb(d1, v3, 0));
                m.Assign(CVZN, m.Cond(v3));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v4, m.IAdd(m.Cast(v4.DataType, d1), m.Cast(v4.DataType, d1)));
                m.Assign(d1, m.Dpb(d1, v4, 0));
                m.Assign(CVZNX, m.Cond(v4));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v5, m.Mem16(m.IAdd(m.Word32(0x0010EC32), m.Cast(W32, m.Cast(W16, d1)))));
                m.Assign(d1, m.Dpb(d1, v5, 0));
                m.Assign(CVZN, m.Cond(v5));
            });
            Given_Instrs(b2, m=> {
                m.Goto(m.IAdd(m.Word32(0x0010EC30), m.Cast(I32, m.Cast(I16, d1))));
            });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 10, Target(b2)));
            while (bwslc.Step())
                ;
            Assert.AreEqual(2, bwslc.Live.Count);
            Assert.AreEqual("(int32) (int16) Mem0[(word32) (word16) (d0 * 0x00000002) + 0x0010EC32:word16] + 0x0010EC30", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("d0", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("(byte) d0", bwslc.JumpTableIndexToUse.ToString(), "Expression to use when indexing");
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
            arch = new Reko.Arch.M68k.M68kArchitecture("m68k");
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
                m.Assign(v2, m.ISub(m.Slice(PrimitiveType.Byte, d0, 0), 0x17));
                m.Assign(CVZN, m.Cond(v2));
            });
            Given_Instrs(b, m =>
            {
                m.Branch(m.Test(ConditionCode.UGT, CZ), Address.Ptr32(0x00100040), InstrClass.ConditionalTransfer);
            });

            var b2 = Given_Block(0x00100008);
            Given_Instrs(b2, m => {
                m.Assign(d1, m.Word32(0));
                m.Assign(CVZN, m.Cond(d1));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v3, m.Slice(v3.DataType, d0, 0));
                m.Assign(d1, m.Dpb(d1, v3, 0));
                m.Assign(CVZN, m.Cond(v3));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v4, m.IAdd(m.Slice(v4.DataType, d1, 0), m.Slice(v4.DataType, d1, 0)));
                m.Assign(d1, m.Dpb(d1, v4, 0));
                m.Assign(CVZNX, m.Cond(v4));
            });
            Given_Instrs(b2, m =>
            {
                m.Assign(v5, m.Mem16(m.IAdd(m.Word32(0x0010EC32), m.Cast(W32, m.Slice(W16, d1, 0)))));
                m.Assign(d1, m.Dpb(d1, v5, 0));
                m.Assign(CVZN, m.Cond(v5));
            });
            Given_Instrs(b2, m => {
                m.Goto(m.IAdd(m.Word32(0x0010EC30), m.Cast(I32, m.Slice(I16, d1, 0))));
            });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host, b2, processorState);
            Assert.IsTrue(bwslc.Start(b2, 10, Target(b2)));
            while (bwslc.Step())
                ;
            Assert.AreEqual(2, bwslc.Live.Count);
            Assert.AreEqual("(int32) Mem0[(word32) SLICE(d0 * 0x00000002, word16, 0) + 0x0010EC32:word16] + 0x0010EC30", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("d0", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("SLICE(d0, byte, 0)", bwslc.JumpTableIndexToUse.ToString(), "Expression to use when indexing");
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
                m.Assign(CVZN, m.Cond(v80));
                m.Branch(m.Test(ConditionCode.GT, VZN), Address.Ptr16(0x1020), InstrClass.ConditionalTransfer);
            });

            var b2 = Given_Block(0x1010);
            Given_Instrs(b2, m =>
            {
                m.Assign(r1, m.IAdd(r1, r1));
                m.Assign(CVZNX, m.Cond(r1));
                m.Assign(v82, m.Mem16(m.IAdd(m.Word32(0x001066A4), r1)));
                m.Assign(r1, m.Dpb(r1, v82, 0));
                m.Assign(CVZN, m.Cond(v82));
                m.Goto(
                    m.IAdd(
                        m.Word32(0x001066A2),
                        m.Cast(PrimitiveType.Int32, m.Cast(PrimitiveType.Int16, r1))),
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
            Assert.AreEqual("(int32) (int16) DPB(r1 * 0x00000002, Mem0[r1 * 0x00000002 + 0x001066A4:word16], 0) + 0x001066A2", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("v80", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("v80", bwslc.JumpTableIndexToUse.ToString(), "Expression to use when indexing");
            Assert.AreEqual("1[0,17]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_Issue_691()
        {
            arch = new Reko.Arch.M68k.M68kArchitecture("m68k");
            var d0 = Reg("d0");
            var CVZN = Cc("CVZN");
            var C = Cc("C");
            var v3 = binder.CreateTemporary(PrimitiveType.Word16);
            var v16 = binder.CreateTemporary(PrimitiveType.Word16);
            var v17 = binder.CreateTemporary(PrimitiveType.Word16);
            var b1 = Given_Block(0xA860);
            Given_Instrs(b1, m =>
            {
                m.Assign(v3, m.ISub(m.Cast(PrimitiveType.Word16, d0), m.Word16(0x20)));
                m.Assign(d0, m.Dpb(d0, v3, 0));
                m.Assign(CVZN, m.Cond(v3));
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
                m.Assign(v16, m.IAdd(m.Cast(PrimitiveType.Word16, d0), m.Cast(PrimitiveType.Word16, d0)));
                m.Assign(d0, m.Dpb(d0, v16, 0));
                m.Assign(CVZN, m.Cond(v16));
                m.Assign(v17, m.IAdd(m.Cast(PrimitiveType.Word16, d0), m.Cast(PrimitiveType.Word16, d0)));
                m.Assign(CVZN, m.Cond(v17));
                m.Assign(d0, m.Dpb(d0, v17, 0));
                m.Goto(m.IAdd(m.Word32(0x0000A8B4), m.Cast(PrimitiveType.Int32, m.Cast(PrimitiveType.Int16, d0))));
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
            Assert.AreEqual("(int32) (int16) (word16) ((word16) (v3 * 0x00000002) * 0x00000002) + 0x0000A8B4", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("v3", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("v3", bwslc.JumpTableIndexToUse.ToString(), "Expression to use when indexing");
            Assert.AreEqual("1[20,7FFFFFFFFFFFFFFF]", bwslc.JumpTableIndexInterval.ToString());
        }

        /// <summary>
        /// Handle 8051 switch statements.
        /// </summary>
        [Test]
        public void Bwslc_Issue_826()
        {
            var A = binder.EnsureRegister(new RegisterStorage("A", 0, 0, PrimitiveType.Byte));
            var R7 = binder.EnsureRegister(new RegisterStorage("R7", 7, 0, PrimitiveType.Byte));
            var DPTR = binder.EnsureRegister(new RegisterStorage("DPTR", 8, 0, PrimitiveType.Word16));
            var C = Cc("C");
            var b0082 = Given_Block(0x0082);
            Given_Instrs(b0082, m =>
            {
                m.Assign(A, m.Mem(A.DataType, DPTR));
                m.Assign(R7, A);
                m.Assign(A, m.IAdd(A, 0xFC));   // A >= 4 will cause a carry.
                m.Assign(C, m.Cond(A));
                m.Branch(m.Test(ConditionCode.ULT, C), Address.Ptr16(0x00C0));
            });

            var b0088 = Given_Block(0x0088);
            Given_Instrs(b0088, m =>
            {
                m.Assign(A, R7);
                m.Assign(A, m.IAdd(A, R7));
                m.Assign(DPTR, m.Word16(0x008E));
                m.Goto(m.IAdd(DPTR, m.Cast(PrimitiveType.UInt16, A)));
            });
            graph.Nodes.Add(b0082);
            graph.Nodes.Add(b0088);
            graph.AddEdge(b0082, b0088);

            var bwslc = new BackwardSlicer(host, b0088, processorState);
            Assert.IsTrue(bwslc.Start(b0088, 3, Target(b0088)));
            while (bwslc.Step())
                ;
            Assert.AreEqual("(uint16) (R7 * 0x02) + 0x008E", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("A", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("A", bwslc.JumpTableIndexToUse.ToString(), "Expression to use when indexing");
            Assert.AreEqual("1[0,3]", bwslc.JumpTableIndexInterval.ToString());
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
        // jmp [0x00234500 + edx * 4]   : JTT = [0x0023450 + edx * 4]               : edx

        // M68k relative jump code.
        //  corresponds to
        // cmpi.l #$00000028,d1         : 0 <= d1 <= 0x28
        // bgt $00106C66
        // add.l d1,d1                  : d1 = d1 * 2
        //                                JTT = 0x0010000 + SEXT:([0x10006 + d1*2],16)
        // move.w (06,pc,d1),d1         : JTT = 0x0010000 + SEXT:([0x10006 + d1],16)
        // jmp.l (pc,d1.w)              : JTT = 0x0010000 + SEXT(d1, 16)

        //  m.Assign(d1, m.IAdd(d1, d1));
        //  m.Assign(CVZNX, m.Cond(d1));
        //  m.Assign(v82,m.LoadW(m.IAdd(m.Word32(0x001066A4), d1)));
        //  m.Assign(d1, m.Dpb(d1, v82, 0));
        //  m.Assign(CVZN, m.Cond(v82));
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
}

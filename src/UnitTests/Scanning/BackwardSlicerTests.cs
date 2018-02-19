#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        private RtlBackwalkHost host;
        private Program program;
        private DirectedGraph<RtlBlock> graph;

        [SetUp]
        public void Setup()
        {
            arch = new FakeArchitecture();
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
        }

        private Identifier Reg(int rn)
        {
            var reg = arch.GetRegister(rn);
            return binder.EnsureRegister(reg);
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
            var r1 = Reg(1);
            var b = Given_Block(0x10);
            Given_Instrs(b, m => m.Goto(r1));

            var bwslc = new BackwardSlicer(host);
            Assert.IsTrue(bwslc.Start(b, 0, Target(b)));
            Assert.AreEqual(bwslc.Live[r1], new BitRange(0, 32));
        }

        [Test]
        public void Bwslc_DetectNoRegister()
        {
            var r1 = Reg(1);
            var b = Given_Block(0x10);
            Given_Instrs(b, m => m.Goto(Address.Ptr32(0x00123400)));

            var bwslc = new BackwardSlicer(host);
            Assert.IsFalse(bwslc.Start(b, 0, Target(b)));
            Assert.AreEqual(0, bwslc.Live.Count);
        }

        [Test]
        public void Bwslc_SeedSlicer()
        {
            var r1 = Reg(1);
            var b = Given_Block(0x10);
            Given_Instrs(b, m => m.Goto(r1));

            var bwslc = new BackwardSlicer(host);
            var result = bwslc.Start(b, 0, Target(b));
            Assert.IsTrue(result);
        }

        [Test]
        public void Bwslc_DetectAddition()
        {
            var r1 = Reg(1);
            var b = Given_Block(0x100);
            Given_Instrs(b, m => m.Goto(m.IAdd(r1, 0x00123400)));

            var bwslc = new BackwardSlicer(host);
            var start = bwslc.Start(b, 0, Target(b));
            Assert.IsTrue(start);
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r1", bwslc.Live.First().Key.ToString());
        }

        [Test]
        public void Bwslc_KillLiveness()
        {
            var r1 = Reg(1);
            var r2 = Reg(2);
            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Assign(r1, m.Shl(r2, 2)); });
            Given_Instrs(b, m => { m.Goto(m.IAdd(r1, 0x00123400)); });

            var bwslc = new BackwardSlicer(host);
            Assert.IsTrue(bwslc.Start(b, 0, Target(b)));
            Assert.IsTrue(bwslc.Step());
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r2", bwslc.Live.First().Key.ToString());
        }

        [Test(Description = "Trace across a jump")]
        public void Bwslc_AcrossJump()
        {
            var r1 = Reg(1);
            var r2 = Reg(2);

            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Assign(r1, m.Shl(r2, 2)); });
            Given_Instrs(b, m => { m.Goto(Address.Ptr32(0x200)); });

            var b2 = Given_Block(0x200);
            Given_Instrs(b2, m => { m.Goto(m.IAdd(r1, 0x00123400)); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host);
            Assert.IsTrue(bwslc.Start(b2, -1, Target(b2)));  // indirect jump
            Assert.IsTrue(bwslc.Step());    // direct jump
            Assert.IsTrue(bwslc.Step());    // shift left
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r2", bwslc.Live.First().Key.ToString());
        }

        [Test(Description = "Trace across a branch where the branch was taken.")]
        public void Bwslc_BranchTaken()
        {
            var r1 = Reg(1);
            var r2 = Reg(2);
            var cz = Cc("CZ");

            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.ULE, cz), Address.Ptr32(0x200), RtlClass.ConditionalTransfer); });

            var b2 = Given_Block(0x200);
            Given_Instrs(b2, m => { m.Assign(r1, m.Shl(r2, 2)); });
            Given_Instrs(b2, m => { m.Goto(m.IAdd(r1, 0x00123400)); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host);
            var start = bwslc.Start(b2, 0, Target(b2)); // indirect jump
            bwslc.Step();                               // shift left
            var step = bwslc.Step();                    // branch

            Assert.IsTrue(start);
            Assert.IsTrue(step);
            Assert.AreEqual(2, bwslc.Live.Count);
            Assert.AreEqual("CZ,r2", 
                string.Join(",", bwslc.Live.Select(l => l.Key.ToString()).OrderBy(n => n)));
        }

        [Test(Description = "Trace until the comparison that gates the jump is encountered.")]
        public void Bwslc_RangeCheck()
        {
            var r1 = Reg(1);
            var r2 = Reg(2);
            var cz = Cc("CZ");

            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Assign(cz, m.Cond(m.ISub(r2, 4))); });
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.ULE, cz), Address.Ptr32(0x200), RtlClass.ConditionalTransfer); });

            var b2 = Given_Block(0x200);
            Given_Instrs(b2, m => { m.Assign(r1, m.Shl(r2, 2)); });
            Given_Instrs(b2, m => { m.Goto(m.IAdd(r1, 0x00123400)); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            var bwslc = new BackwardSlicer(host);
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
            var r1 = Reg(1);
            var r2 = Reg(2);
            var cz = Cc("CZ");

            var b = Given_Block(0x100);
            Given_Instrs(b, m => { m.Assign(r1, m.And(r1, 7)); m.Assign(cz, m.Cond(r1)); });
            Given_Instrs(b, m => { m.Goto(m.Mem32(m.IAdd(m.Word32(0x00123400), m.IMul(r1, 4)))); });

            graph.Nodes.Add(b);

            var bwslc = new BackwardSlicer(host);
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
                m.Branch(new TestCondition(ConditionCode.UGT, SCZO), Address.Ptr16(0x120), RtlClass.ConditionalTransfer);
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

            var bwslc = new BackwardSlicer(host);
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
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.ULT, C), Address.Ptr32(0x2000), RtlClass.ConditionalTransfer); });

            var b2 = Given_Block(0x1008);
            Given_Instrs(b2, m => {
                m.BranchInMiddleOfInstruction(m.Eq0(ecx), Address.Ptr32(0x1010), RtlClass.ConditionalTransfer);
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

            var bwslc = new BackwardSlicer(host);
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
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.UGT, C), Address.SegPtr(0xC00, 0x200), RtlClass.ConditionalTransfer); });

            var b2 = Given_Block(0x0C00, 0x0108);
            Given_Instrs(b2, m => { m.Assign(bx, m.IAdd(bx, bx)); m.Assign(SCZO, m.Cond(bx)); });
            Given_Instrs(b2, m => { m.Goto(m.SegMem(PrimitiveType.Ptr32, ds, m.IAdd(bx, 34))); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            graph.Nodes.Add(b);

            var bwslc = new BackwardSlicer(host);
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
            Given_Instrs(b, m => { m.Branch(m.Test(ConditionCode.UGT, C), Address.Ptr32(0x00100010), RtlClass.ConditionalTransfer); });

            var b2 = Given_Block(0x001000008);
            Given_Instrs(b2, m => { m.Assign(edx, m.Xor(edx, edx)); m.Assign(SZO, m.Cond(edx)); m.Assign(C, Constant.False()); });
            Given_Instrs(b2, m => { m.Assign(dl, m.Mem8(m.IAdd(eax, 0x00123500))); });
            Given_Instrs(b2, m => { m.Goto(m.Mem32(m.IAdd(m.IMul(edx, 4), 0x00123400))); });

            graph.Nodes.Add(b);
            graph.Nodes.Add(b2);
            graph.AddEdge(b, b2);

            graph.Nodes.Add(b);

            var bwslc = new BackwardSlicer(host);
            Assert.IsTrue(bwslc.Start(b2, 3, Target(b2)));   // indirect jump
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsTrue(bwslc.Step());
            Assert.IsFalse(bwslc.Step());

            Assert.AreEqual("Mem0[Mem0[eax + 0x00123500:byte] * 0x00000004 + 0x00123400:word32]", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("1[0,3]", bwslc.JumpTableIndexInterval.ToString());
        }

        [Test]
        public void Bwslc_SimplifySum()
        {
            var r1 = Reg(1);
            var b = Given_Block(0x100);
            Given_Instrs(b, m => m.Assign(r1, m.IAdd(r1, r1)));
            Given_Instrs(b, m => m.Goto(m.IAdd(r1, 0x00123400)));

            var bwslc = new BackwardSlicer(host);
            Assert.IsTrue(bwslc.Start(b, 0, Target(b)));
            Assert.IsTrue(bwslc.Step());
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("r1", bwslc.Live.First().Key.ToString());
            Assert.AreEqual("r1 * 0x00000002 + 0x00123400", bwslc.JumpTableFormat.ToString());
        }

        [Test]
        public void Bwslc_SimplifySumAndProduct()
        {
            var r1 = Reg(1);
            var b = Given_Block(0x100);
            Given_Instrs(b, m => m.Assign(r1, m.IAdd(r1, r1)));
            Given_Instrs(b, m => m.Assign(r1, m.IAdd(r1, r1)));
            Given_Instrs(b, m => m.Goto(m.IAdd(r1, 0x00123400)));

            var bwslc = new BackwardSlicer(host);
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
                m.Branch(m.Test(ConditionCode.UGT, CZ), Address.Ptr32(0x00100040), RtlClass.ConditionalTransfer);
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

            var bwslc = new BackwardSlicer(host);
            Assert.IsTrue(bwslc.Start(b2, 10, Target(b2)));
            while (bwslc.Step())
                ;
            Assert.AreEqual(1, bwslc.Live.Count);
            Assert.AreEqual("(int32) (int16) Mem0[(word32) (word16) ((word32) d0 * 0x00000002) + 0x0010EC32:word16] + 0x0010EC30", bwslc.JumpTableFormat.ToString());
            Assert.AreEqual("v2", bwslc.JumpTableIndex.ToString());
            Assert.AreEqual("r1", bwslc.JumpTableIndexToUse.ToString(), "Register to use when indexing");
            Assert.AreEqual("r1", bwslc.JumpTableIndexInterval.ToString());
        }
    }
}

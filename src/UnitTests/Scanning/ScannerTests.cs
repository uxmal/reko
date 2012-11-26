#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Scanning;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class ScannerTests
    {
        ArchitectureMock arch;
        Program prog;
        TestScanner scan;

        public class TestScanner : Scanner
        {
            public TestScanner(Program prog)
                : base(prog, null, new FakeDecompilerEventListener())
            {
            }

            public BlockWorkitem Test_LastBlockWorkitem { get; private set; } 

            public override BlockWorkitem CreateBlockWorkItem(Address addrStart, Procedure proc, ProcessorState state)
            {
                Test_LastBlockWorkitem = base.CreateBlockWorkItem(addrStart, proc, state);
                return Test_LastBlockWorkitem;
            }
        }

        [SetUp]
        public void Setup()
        {
            arch = new ArchitectureMock();
        }

        private ProcedureSignature CreateSignature(string ret, params string[] args)
        {
            var retReg = new Identifier(ret, 0, PrimitiveType.Word32, new RegisterStorage(ret, 0, PrimitiveType.Word32));
            var argIds = new List<Identifier>();
            foreach (var arg in args)
            {
                argIds.Add(new Identifier(arg, argIds.Count + 1, PrimitiveType.Word32,
                    new RegisterStorage(ret, argIds.Count + 1, PrimitiveType.Word32)));
            }
            return new ProcedureSignature(retReg, argIds.ToArray());
        }

        private void BuildX86RealTest(Action<IntelAssembler> test)
        {
            prog = new Program();
            var addr = new Address(0xC00, 0);
            var m = new IntelAssembler(new IntelArchitecture(ProcessorMode.Real), addr, new List<EntryPoint>());
            test(m);
            prog.Image = m.GetImage();
            prog.Architecture = m.Architecture;
            prog.Platform = new FakePlatform();
            scan = new TestScanner(prog);
            EntryPoint ep = new EntryPoint(addr, arch.CreateProcessorState());
            scan.EnqueueEntryPoint(ep);
        }

        [Test]
        public void AddEntryPoint()
        {
            arch.DisassemblyStream = new MachineInstruction[] {
                new FakeInstruction(Operation.Add,
                    new RegisterOperand(arch.GetRegister(1)), 
                    ImmediateOperand.Word32(1))
            };
            arch.Test_SetRewriterForAddress(new Address(0x12314), new FakeRewriter(new RtlInstructionCluster[] {
                new RtlInstructionCluster(new Address(0x12314), 1, 
                new RtlReturn(4, 0)),
            }));
            var prog = new Program
            {
                Architecture = arch,
                Image = new ProgramImage(new Address(0x12314), new byte[1]),
                Platform = new FakePlatform()
            };
            var sc = new Scanner(prog, null, new FakeDecompilerEventListener());
            sc.EnqueueEntryPoint(
                new EntryPoint(
                    new Address(0x12314),
                    arch.CreateProcessorState()));
            sc.ProcessQueue();

            Assert.AreEqual(1, sc.Procedures.Count);
            Assert.AreEqual(0x12314, sc.Procedures.Keys[0].Offset);
            Assert.IsTrue(sc.CallGraph.EntryPoints.Contains(sc.Procedures.Values[0]));
        }

        [Test]
        public void AddBlock()
        {
            var sc = CreateScanner(0x0100, 10);
            var block = sc.AddBlock(new Address(0x102), new Procedure("bob", null), "l0102");
            Assert.IsNotNull(sc.FindExactBlock(new Address(0x0102)));
        }

        private TestScanner CreateScanner(uint startAddress, int imageSize)
        {
            if (prog == null)
            {
                prog = new Program();
            }
            prog.Architecture = arch;
            prog.Platform = new FakePlatform();
            prog.Image = new ProgramImage(new Address(startAddress), new byte[imageSize]);
            return new TestScanner(prog);
        }

        [Test]
        public void SplitBlock()
        {
            scan = CreateScanner(0x100, 0x100);
            var proc = new Procedure("foo", arch.CreateFrame());
            Enqueue(new Address(0x101), proc);
            Enqueue(new Address(0x106), proc);
            Enqueue(new Address(0x104), proc);

            Assert.AreEqual("l00000101", scan.FindContainingBlock(new Address(0x103)).Name);
            Assert.AreEqual("l00000104", scan.FindContainingBlock(new Address(0x105)).Name);
            Assert.AreEqual("l00000106", scan.FindContainingBlock(new Address(0x106)).Name);
        }

        private void Enqueue(Address addr, Procedure proc)
        {
            arch.Test_SetRewriterForAddress(addr, new FakeRewriter(new RtlInstructionCluster(addr, 4,
                new RtlAssignment(new MemoryAccess(new Constant(0x3000), PrimitiveType.Word32), Constant.Word32(42)))));

            scan.EnqueueJumpTarget(addr, proc, arch.CreateProcessorState());
        }

        [Test]
        public void BuildExpr()
        {
            Regexp re;
            re = Regexp.Compile("11.22");
            Debug.WriteLine(re);
            re = Regexp.Compile("34+32+33");
            Debug.WriteLine(re);
            re = Regexp.Compile(".*11221122");
            Debug.WriteLine(re);
            re = Regexp.Compile("11(22|23)*44");
            Assert.IsTrue(re.Match(new Byte[] { 0x11, 0x22, 0x22, 0x23, 0x44 }, 0));
            re = Regexp.Compile("(B8|B9)*0204");
            Assert.IsTrue(re.Match(new Byte[] { 0xB8, 0x02, 0x04 }, 0));
            re = Regexp.Compile("C390*");
            Assert.IsTrue(re.Match(new Byte[] { 0xC3, 0x90, 0x90, 0x90, 0xB8 }, 0));
        }

        [Test]
        public void MatchTest()
        {
            byte[] data = new byte[] {
										   0x30, 0x34, 0x32, 0x12, 0x55, 0xC3, 0xB8, 0x34, 0x00 
									   };

            Regexp re = Regexp.Compile(".*55C3");
            Assert.IsTrue(re.Match(data, 0), "Should have matched");
        }


        [Test]
        public void CallGraphTree()
        {
            Program prog = new Program();
            var addr = new Address(0xC00, 0);
            var m = new IntelAssembler(new IntelArchitecture(ProcessorMode.Real), addr, new List<EntryPoint>());
            m.i86();

            m.Proc("main");
            m.Call("baz");
            m.Ret();
            m.Endp("main");

            m.Proc("foo");
            m.Ret();
            m.Endp("foo");

            m.Proc("bar");
            m.Ret();
            m.Endp("bar");

            m.Proc("baz");
            m.Call("foo");
            m.Call("bar");
            m.Jmp("foo");
            m.Endp("baz");


            prog.Image = m.GetImage();
            prog.Architecture = m.Architecture;
            prog.Platform = new FakePlatform();
            var scan = new Scanner(prog, new Dictionary<Address, ProcedureSignature>(), new FakeDecompilerEventListener());
            EntryPoint ep = new EntryPoint(addr, arch.CreateProcessorState());
            scan.EnqueueEntryPoint(ep);
            scan.ProcessQueue();

            Assert.AreEqual(4, prog.Procedures.Count);
        }

        [Test]
        public void RepeatUntilBlock()
        {
            BuildX86RealTest(delegate (IntelAssembler m)
            {
                m.i86();
                m.Mov(m.ax, 0);         // To ensure we end up with a split block.
                m.Label("lupe");
                m.Mov(m.MemB(Registers.si, 0), 0);
                m.Inc(m.si);
                m.Dec(m.cx);
                m.Jnz("lupe");
                m.Ret();
            });
            scan.ProcessQueue();
            Assert.AreEqual(1, scan.Procedures.Count);
            var sExp = @"// fn0C00_0000
void fn0C00_0000()
fn0C00_0000_entry:
	// succ:  l0C00_0000
l0C00_0000:
	ax = 0x0000
	// succ:  l0C00_0003
l0C00_0003:
	Mem0[ds:si + 0x0000:byte] = 0x00
	si = si + 0x0001
	SZO = cond(si)
	cx = cx - 0x0001
	SZO = cond(cx)
	branch Test(NE,Z) l0C00_0003
	// succ:  l0C00_000B l0C00_0003
l0C00_000B:
	return
	// succ:  fn0C00_0000_exit
fn0C00_0000_exit:
";
            var sw = new StringWriter();
            scan.Procedures.Values[0].Write(false, sw);
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        [Category("FPU")]
        [Ignore("Get back to work on FPU later")]
        public void EnqueueingProcedureShouldResetItsFpuStack()
        {
            var scan = CreateScanner(0x100000, 0x1000);
            var rtls = new List<RtlInstructionCluster>();
            arch.Test_SetRewriterForAddress(new Address(0x100100), new FakeRewriter(
                new RtlInstructionCluster(new Address(0x100100), 4,
                    new RtlReturn(4, 0))));
            
            var st = (X86State) arch.CreateProcessorState();
            st.GrowFpuStack(new Address(0x100000));
            scan.ScanProcedure(new Address(0x100100),  null, st);
            var stNew = (X86State)scan.Test_LastBlockWorkitem.Context;
            Assert.IsNotNull(stNew);
            Assert.AreNotSame(st, stNew);
            Assert.AreEqual(1, st.FpuStackItems);
            Assert.AreEqual(0, stNew.FpuStackItems);
        }

        [Test]
        public void ScanImportedProcedure()
        {
            prog = new Program();
            prog.ImportThunks.Add(0x2000, new PseudoProcedure(
                "grox", CreateSignature("ax", "bx")));
            var scan = CreateScanner(0x1000, 0x200);
            var proc = scan.ScanProcedure(new Address(0x2000), "fn000020", arch.CreateProcessorState());
            Assert.AreEqual("grox", proc.Name);
            Assert.AreEqual("ax", proc.Signature.ReturnValue.Name);
            Assert.AreEqual("bx", proc.Signature.FormalArguments[0].Name);
        }



        //        [Test(Description="When entrypoints are added they should end up in the top-level scanner queue")]
        //public void EntryPointsAddedToScanQueue()
        //{
        //    scan.EnqueueEntryPoint(new EntryPoint(new Address(0x3123), new IntelState()));
        //    Assert.AreEqual(1, scan.Queue.Count);
        //    Assert.AreEqual(0, scan.Stack.Count);
        //}

        //[Test(Description="Pulling a queue item should create a ProcedureScanner and push it on the stack.")]
        //public void DequeueingItemShouldPutProcedureScannerOnStack()
        //{
        //    scan.EnqueueEntryPoint(new EntryPoint(new Address(0x3123), new IntelState()));
        //    scan.ProcessQueueItem();
        //    Assert.AreEqual(1, scan.Stack.Count);
        //}

        //        [Test(Description="Dequeueing a procedure item should put a block in the scanner queue")] 
        //public void DequeueingItemShouldPutBlockworkitemOnProcedureScannerQueue()
        //{
        //    scan.EnqueueEntryPoint(new EntryPoint(new Address(0x3123), new IntelState()));
        //    scan.ProcessQueueItem();
        //    Assert.AreEqual(1, scan.Stack.Peek().Queue.Count);
        //}

    }
}
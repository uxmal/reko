#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Arch.Intel;
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
    public class Scanner2Tests
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

        private void BuildX86RealTest(Action<IntelAssembler> test)
        {
            prog = new Program();
            var emitter = new IntelEmitter();
            var addr = new Address(0xC00, 0);
            var m = new IntelAssembler(new IntelArchitecture(ProcessorMode.Real), addr, emitter, new List<EntryPoint>());
            test(m);
            prog.Image = new ProgramImage(addr, emitter.Bytes);
            prog.Architecture = m.Architecture;
            prog.Platform = new FakePlatform();
            scan = new TestScanner(prog);
            EntryPoint ep = new EntryPoint(addr, new IntelState());
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
            arch.InstructionStream = new RtlInstruction[] { 
                new RtlReturn(new Address(0x12314), 1, 4, 0)
            };
            var sc = new Scanner(arch, new ProgramImage(new Address(0x12314), new byte[1]), new FakePlatform(), null, new FakeDecompilerEventListener());
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
            prog = new Program();
            prog.Architecture = arch;
            prog.Platform = new FakePlatform();
            prog.Image = new ProgramImage(new Address(startAddress), new byte[imageSize]);
            return new TestScanner(prog);
        }

        [Test]
        public void SplitBlock()
        {
            var sc = CreateScanner(0x100, 10);
            var proc = new Procedure("foo", arch.CreateFrame());
            var b101 = sc.EnqueueJumpTarget(new Address(0x101), proc, null);
            b101.Statements.Add(0x101,new DefInstruction(null));
            var b106 = sc.EnqueueJumpTarget(new Address(0x106), proc, null);
            b106.Statements.Add(0x106,new DefInstruction(null));
            var b104 = sc.EnqueueJumpTarget(new Address(0x104), proc, null);
            b104.Statements.Add(0x104, new DefInstruction(null));
            Assert.IsTrue(proc.ControlGraph.Nodes.Contains(b101));
            Assert.IsTrue(proc.ControlGraph.Nodes.Contains(b106));
            Assert.IsTrue(proc.ControlGraph.Nodes.Contains(b104));
            Assert.AreEqual("l00000101", sc.FindContainingBlock(new Address(0x103)).Name);
            Assert.AreEqual("l00000104", sc.FindContainingBlock(new Address(0x105)).Name);
            Assert.AreEqual("l00000106", sc.FindContainingBlock(new Address(0x106)).Name);
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
            var emitter = new IntelEmitter();
            var addr = new Address(0xC00, 0);
            var m = new IntelAssembler(new IntelArchitecture(ProcessorMode.Real), addr, emitter, new List<EntryPoint>());
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


            prog.Image = new ProgramImage(addr, emitter.Bytes);
            var scan = new Scanner(m.Architecture, prog.Image, new FakePlatform(), new Dictionary<Address, ProcedureSignature>(), new FakeDecompilerEventListener());
            EntryPoint ep = new EntryPoint(addr, new IntelState());
            scan.EnqueueEntryPoint(ep);
            scan.ProcessQueue();

            Assert.AreEqual(4, scan.Procedures.Count);
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
l0C00_0000:
	ax = 0x0000
l0C00_0003:
	store(Mem0[ds:si + 0x0000:byte]) = 0x00
	si = si + 0x0001
	SZO = cond(si)
	cx = cx - 0x0001
	SZO = cond(cx)
	branch Test(NE,Z) l0C00_0003
l0C00_000B:
	return
fn0C00_0000_exit:
";
            var sw = new StringWriter();
            scan.Procedures.Values[0].Write(false, sw);
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void EnqueueingProcedureShouldResetItsFpuStack()
        {
            var scan = CreateScanner(0x100000, 0x1000);
            IntelState st = new IntelState();
            st.GrowFpuStack(new Address(0x100000));
            scan.EnqueueProcedure(null, new Address(0x200000), null, st);
            var stNew = (IntelState)scan.Test_LastBlockWorkitem.State;
            Assert.IsNotNull(stNew);
            Assert.AreNotSame(st, stNew);
            Assert.AreEqual(1, st.FpuStackItems);
            Assert.AreEqual(0, stNew.FpuStackItems);
        }
    }

    [TestFixture]
    public class ScannerOldTests
    {
        private Program prog;
        private TestScanner scanner;

        public ScannerOldTests()
        {
        }

        private void SetupMockCodeWalker()
        {
            prog = new Program();
            prog.Architecture = new ArchitectureMock();
            prog.Image = new ProgramImage(new Address(0x1000), new byte[0x4000]);
            scanner = new TestScanner(prog);
            scanner.MockCodeWalker = new MockCodeWalker(new Address(0x1000));
            scanner.EnqueueEntryPoint(new EntryPoint(new Address(0x1000), prog.Architecture.CreateProcessorState()));
        }



        /// <summary>
        /// Avoid promoting stumps that contain short sequences of code.
        /// </summary>
        [Test]
        public void DontPromoteStumps()
        {
            Program prog = BuildTest("Fragments/multiple/jumpintoproc2.asm");
            ScannerOld scan = new ScannerOld(prog, null);
            scan.EnqueueProcedure(null, prog.Image.BaseAddress, null, prog.Architecture.CreateProcessorState());
            Assert.IsTrue(scan.ProcessItem());
            Assert.IsTrue(scan.ProcessItem());
            Assert.IsTrue(scan.ProcessItem());
            Assert.IsTrue(scan.ProcessItem());
            //			Assert.IsTrue(scan.ProcessItem());
            DumpImageMap(prog.Image.Map);
        }

        [Test]
        public void ScanInterprocedureJump()
        {
            Program prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            Assembler asm = new IntelTextAssembler();
            asm.Assemble(new Address(0xC00, 0x0000), FileUnitTester.MapTestPath("Fragments/multiple/jumpintoproc.asm"));
            prog.Image = asm.Image;
            ScannerOld scan = new ScannerOld(prog, null);
            scan.EnqueueEntryPoint(new EntryPoint(asm.StartAddress, new IntelState()));
            scan.ProcessQueue();
            using (FileUnitTester fut = new FileUnitTester("Scanning/ScanInterprocedureJump.txt"))
            {
                Dumper dumper = prog.Architecture.CreateDumper();
                dumper.Dump(prog, prog.Image.Map, fut.TextWriter);
                foreach (KeyValuePair<Address, Procedure> de in prog.Procedures)
                {
                    fut.TextWriter.WriteLine("{0} (@ {1})", de.Value.Name, de.Key);
                }
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void ScanSimple()
        {
            SetupMockCodeWalker();

            scanner.MockCodeWalker.AddReturn(new Address(0x1004));
            scanner.ProcessQueue();

            Assert.AreEqual(1, prog.Procedures.Count);

        }

        [Test]
        public void ScanTwoProcedures()
        {
            SetupMockCodeWalker();

            scanner.MockCodeWalker.AddCall(new Address(0x1001), new Address(0x2000), new FakeProcessorState());
            scanner.MockCodeWalker.AddReturn(new Address(0x1002));

            scanner.MockCodeWalker.AddReturn(new Address(0x2002));

            scanner.ProcessQueue();

            Assert.AreEqual(2, prog.Procedures.Count);
        }

        [Test]
        public void ScanProcJumpingIntoOther()
        {
            SetupMockCodeWalker();
            scanner.MockCodeWalker.AddCall(new Address(0x1001), new Address(0x1100));
            scanner.MockCodeWalker.AddReturn(new Address(0x1002));

            scanner.MockCodeWalker.AddJump(new Address(0x1102), new Address(0x1103));
            scanner.MockCodeWalker.AddReturn(new Address(0x1110));

            scanner.ProcessQueue();
            scanner.EnqueueProcedure(null, new Address(0x2000), null, new FakeProcessorState());
            scanner.MockCodeWalker.AddCall(new Address(0x2001), new Address(0x1101));	// calls into middle of procedure already scanned.
            scanner.MockCodeWalker.AddReturn(new Address(0x2004));

            scanner.ProcessQueue();

            Assert.AreEqual(3, prog.Procedures.Count);
            Procedure p2000 = prog.Procedures[new Address(0x2000)];
            Procedure p1100 = prog.Procedures[new Address(0x1100)];
            Procedure p1101 = prog.Procedures[new Address(0x1101)];
            Assert.IsNotNull(p2000);
            Assert.IsNotNull(p1100);
            Assert.IsNotNull(p1101);
            ImageMapBlock b1100 = GetBlockAt(0x1100);
            ImageMapBlock b1101 = GetBlockAt(0x1101);
            ImageMapBlock b1103 = GetBlockAt(0x1103);
            Assert.AreSame(p1100, b1100.Procedure);
            Assert.AreSame(p1101, b1101.Procedure);
            Assert.AreSame(p1101, b1103.Procedure);
        }



        private ImageMapBlock GetBlockAt(uint a)
        {
            ImageMapItem item;
            prog.Image.Map.TryFindItemExact(new Address(a), out item);
            return (ImageMapBlock)item;
        }

        [Test]
        [Ignore("Need to implement this feature")]
        public void ObeyDontDecompileUserProcedure()
        {
            //$REVIEW: a directive that introduces a procedure signature, but inhibits its decompilation.
        }

        private Program BuildTest(string srcFile)
        {
            Program prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            Assembler asm = new IntelTextAssembler();
            asm.Assemble(new Address(0x0C00, 0x0000), FileUnitTester.MapTestPath(srcFile));
            prog.Image = asm.Image;
            return prog;
        }

        private void DumpImageMap(ImageMap map)
        {
            foreach (ImageMapItem item in map.Items.Values)
            {
                Console.WriteLine(item);
            }
        }

        private class TestScanner : ScannerOld
        {
            private MockCodeWalker mcw;

            public TestScanner(Program prog)
                : base(prog, null)
            {
            }

            public override CodeWalker CreateCodeWalker(Address addr, ProcessorState state)
            {
                if (mcw != null)
                {
                    mcw.SetWalkAddress(addr);
                    return mcw;
                }
                else
                    return base.CreateCodeWalker(addr, state);
            }

            public MockCodeWalker MockCodeWalker
            {
                get { return mcw; }
                set { mcw = value; }
            }
        }
    }
}
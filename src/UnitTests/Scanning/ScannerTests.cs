#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Rhino.Mocks;
using Decompiler.UnitTests.Scanning.Fragments;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class ScannerTests
    {
        private MockRepository mr;
        IProcessorArchitecture arch;
        FakeArchitecture fakeArch;
        Program program;
        TestScanner scan;
        Identifier reg1;
        private IImportResolver importResolver;
        IDictionary<Address, ProcedureSignature> callSigs;

        public class TestScanner : Scanner
        {
            public TestScanner(Program prog, IDictionary<Address, ProcedureSignature> callSigs, IImportResolver importResolver)
                : base(prog, callSigs, importResolver, new FakeDecompilerEventListener())
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
            mr = new MockRepository();
            fakeArch = new FakeArchitecture();
            importResolver = mr.StrictMock<IImportResolver>();
            callSigs = new Dictionary<Address, ProcedureSignature>();
            arch = fakeArch;
            var r1 = arch.GetRegister(1);
            reg1 = new Identifier(r1.Name, PrimitiveType.Word32, r1);
        }

        private ProcedureSignature CreateSignature(string ret, params string[] args)
        {
            var retReg = new Identifier(ret, PrimitiveType.Word32, new RegisterStorage(ret, 0, PrimitiveType.Word32));
            var argIds = new List<Identifier>();
            foreach (var arg in args)
            {
                argIds.Add(new Identifier(arg, PrimitiveType.Word32,
                    new RegisterStorage(ret, argIds.Count + 1, PrimitiveType.Word32)));
            }
            return new ProcedureSignature(retReg, argIds.ToArray());
        }

        private void BuildX86RealTest(Action<X86Assembler> test)
        {
            var addr = Address.SegPtr(0x0C00, 0);
            var m = new X86Assembler(new IntelArchitecture(ProcessorMode.Real), addr, new List<EntryPoint>());
            test(m);
            var lr = m.GetImage();
            program = new Program(
                lr.Image,
                lr.ImageMap,
                lr.Architecture,
                new FakePlatform(null, arch));
            scan = CreateScanner(program);
            EntryPoint ep = new EntryPoint(addr, program.Architecture.CreateProcessorState());
            scan.EnqueueEntryPoint(ep);
        }

        private void AssertProgram(string sExpected, Program prog)
        {
            var sw = new StringWriter();
            foreach (var proc in prog.Procedures.Values)
            {
                proc.Write(false, false, sw);
                sw.WriteLine();
            }
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        [Test]
        public void Scanner_AddEntryPoint()
        {
            Given_Trace(new RtlTrace(0x12314) 
            {
                m => { m.Return(4, 0); }
            });
            Given_Program(Address.Ptr32(0x12314));
            var project = new Project { Programs = { program } };
            
            var sc = new Scanner(
                program,
                null,
                new ImportResolver(project),
                new FakeDecompilerEventListener());
            sc.EnqueueEntryPoint(
                new EntryPoint(
                    Address.Ptr32(0x12314),
                    arch.CreateProcessorState()));
            sc.ScanImage();

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0x12314, program.Procedures.Keys[0].Offset);
            Assert.IsTrue(program.CallGraph.EntryPoints.Contains(program.Procedures.Values[0]));
        }

        private void Given_Program(Address address)
        {
            var image = new LoadedImage(address, new byte[1]);
            var imageMap = image.CreateImageMap();
            this.program = new Program
            {
                Architecture = arch,
                Image = image,
                ImageMap = imageMap,
                Platform = new FakePlatform(null, arch)
            };
        }

        private void Given_Trace( RtlTrace trace)
        {
            fakeArch.Test_AddTrace(trace);
        }

        [Test]
        public void AddBlock()
        {
            var sc = CreateScanner(0x0100, 10);
            var block = sc.AddBlock(Address.Ptr32(0x102), new Procedure("bob", null), "l0102");
            Assert.IsNotNull(sc.FindExactBlock(Address.Ptr32(0x0102)));
        }

        private TestScanner CreateScanner(uint startAddress, int imageSize)
        {
            var image = new LoadedImage(Address.Ptr32(startAddress), new byte[imageSize]);
            program = new Program(
                image,
                image.CreateImageMap(),
                arch,
                new FakePlatform(null, arch));
            return new TestScanner(
                program,
                callSigs,
                importResolver);
        }

        private TestScanner CreateScanner(Program prog)
        {
            this.program = prog;
            return new TestScanner(
                prog, 
                callSigs, 
                importResolver);
        }

        private TestScanner CreateScanner(Program prog, uint startAddress, int imageSize)
        {
            this.program = prog;
            prog.Architecture = arch;
            prog.Platform = new FakePlatform(null, arch);
            prog.Image = new LoadedImage(Address.Ptr32(startAddress), new byte[imageSize]);
            prog.ImageMap = prog.Image.CreateImageMap();
            return new TestScanner(prog, callSigs, importResolver);
        }

        [Test]
        public void Scanner_SplitBlock()
        {
            scan = CreateScanner(0x100, 0x100);
            var proc = new Procedure("foo", arch.CreateFrame());
            Enqueue(Address.Ptr32(0x101), proc);
            Enqueue(Address.Ptr32(0x106), proc);
            Enqueue(Address.Ptr32(0x104), proc);

            Assert.AreEqual("l00000101", scan.FindContainingBlock(Address.Ptr32(0x103)).Name);
            Assert.AreEqual("l00000104", scan.FindContainingBlock(Address.Ptr32(0x105)).Name);
            Assert.AreEqual("l00000106", scan.FindContainingBlock(Address.Ptr32(0x106)).Name);
        }

        private void Enqueue(Address addr, Procedure proc)
        {
            fakeArch.Test_AddTrace(new RtlTrace(addr.ToUInt32())
            {
                m => {
                    m.Assign(m.LoadDw(m.Word32(0x3000)), m.Word32(42));
                }
            });
            scan.EnqueueJumpTarget(addr, addr, proc, arch.CreateProcessorState());
        }

        [Test]
        public void Scanner_BuildExpr()
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
        public void Scanner_MatchTest()
        {
            byte[] data = new byte[] {
										   0x30, 0x34, 0x32, 0x12, 0x55, 0xC3, 0xB8, 0x34, 0x00 
									   };

            Regexp re = Regexp.Compile(".*55C3");
            Assert.IsTrue(re.Match(data, 0), "Should have matched");
        }


        [Test]
        public void Scanner_CallGraphTree()
        {
            Program prog = new Program();
            var addr = Address.SegPtr(0xC00, 0);
            var m = new X86Assembler(new IntelArchitecture(ProcessorMode.Real), addr, new List<EntryPoint>());
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

            var lr = m.GetImage();
            prog.Image = lr.Image;
            prog.ImageMap = lr.ImageMap;
            prog.Architecture = lr.Architecture;
            prog.Platform = new FakePlatform(null, arch);
            var proj = new Project { Programs = { prog } };
            var scan = new Scanner(prog, new Dictionary<Address, ProcedureSignature>(), new ImportResolver(proj), new FakeDecompilerEventListener());
            EntryPoint ep = new EntryPoint(addr, prog.Architecture.CreateProcessorState());
            scan.EnqueueEntryPoint(ep);
            scan.ScanImage();

            Assert.AreEqual(4, prog.Procedures.Count);
        }

        [Test]
        public void Scanner_RepeatUntilBlock()
        {
            BuildX86RealTest(delegate(X86Assembler m)
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
            scan.ScanImage();
            Assert.AreEqual(1, program.Procedures.Count);
            var sExp = @"// fn0C00_0000
// Return size: 2
void fn0C00_0000()
fn0C00_0000_entry:
	// succ:  l0C00_0000
l0C00_0000:
	sp = fp
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
            program.Procedures.Values[0].Write(false, sw);
            Assert.AreEqual(sExp, sw.ToString());
        }

        private void Given_x86_Flat32()
        {
            arch = new X86ArchitectureFlat32();
        }

        [Test]
        public void Scanner_ImportedProcedure()
        {
            program = new Program();
            program.ImportReferences.Add(
                Address.Ptr32(0x2000),
                new NamedImportReference(
                    Address.Ptr32(0x2000),
                    "module",
                    "grox"));
            Given_Trace(new RtlTrace(0x2000) {
                    m => m.SideEffect(m.Word32(0x1234))
            });
            var groxSig = CreateSignature("ax", "bx");
            var grox = new ExternalProcedure("grox", groxSig);

            importResolver.Stub(i => i.ResolveProcedure(
                Arg<string>.Is.Equal("module"),
                Arg<string>.Is.Equal("grox"),
                Arg<Platform>.Is.NotNull)).Return(grox);
            mr.ReplayAll();

            var scan = CreateScanner(program, 0x1000, 0x2000);
            var proc = scan.ScanProcedure(Address.Ptr32(0x2000), "fn000020", arch.CreateProcessorState());
            Assert.AreEqual("grox", proc.Name);
            Assert.AreEqual("ax", proc.Signature.ReturnValue.Name);
            Assert.AreEqual("bx", proc.Signature.Parameters[0].Name);
        }

        [Test]
        public void Scanner_IsLinearReturning_EmptyBlock()
        {
            var scanner = CreateScanner(0x1000, 0x1000);
            var proc = new Procedure("fn1000", arch.CreateFrame());
            var block = new Block(proc, "l1000");
            Assert.IsFalse(scanner.IsLinearReturning(block));
        }

        [Test]
        public void Scanner_IsLinearReturn_EndsWithReturn()
        {
            var scanner = CreateScanner(0x1000, 0x1000);
            var m = new ProcedureBuilder(arch, "fn1000");
            m.Return();

            var block = m.Procedure.ControlGraph.Blocks[2];
            Assert.IsTrue(scanner.IsLinearReturning(block));
        }

        [Test]
        public void Scanner_IsLinearReturn_DoesntEndWithReturn()
        {
            var scanner = CreateScanner(0x1000, 0x1000);
            var m = new ProcedureBuilder(arch, "fn1000");
            m.Assign(m.Register("ax"), m.Register("bx"));

            var block = m.Block;
            Assert.IsFalse(scanner.IsLinearReturning(block));
        }

        [Test]
        public void Scanner_Interprocedural_CloneBlocks()
        {
            var scan = CreateScanner(0x1000, 0x2000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => { m.Assign(reg1, m.Word32(0)); },
                m => { m.Assign(m.LoadDw(m.Word32(0x1800)), reg1); },
                m => { m.Return(0, 0); }
            });
            Given_Trace(new RtlTrace(0x1004)
            {
                m => { m.Assign(m.LoadDw(m.Word32(0x1800)), reg1); },
                m => { m.Return(0, 0); }
            });
            Given_Trace(new RtlTrace(0x1100)
            {
                m => { m.Assign(reg1, m.Word32(1)); },
                m => { m.Goto(Address.Ptr32(0x1004)); },
            });

            scan.EnqueueEntryPoint(new EntryPoint(Address.Ptr32(0x1000), arch.CreateProcessorState()));
            scan.EnqueueEntryPoint(new EntryPoint(Address.Ptr32(0x1100), arch.CreateProcessorState()));
            scan.ScanImage();

            var sExp =
@"// fn00001000
// Return size: 0
void fn00001000()
fn00001000_entry:
l00001000:
	r63 = fp
	r1 = 0x00000000
l00001004:
	Mem0[0x00001800:word32] = r1
	return
fn00001000_exit:

// fn00001100
// Return size: 0
void fn00001100()
fn00001100_entry:
	goto l00001100
l00001004_in_fn00001100:
	Mem0[0x00001800:word32] = r1
	return
l00001100:
	r63 = fp
	r1 = 0x00000001
	goto l00001004_in_fn00001100
fn00001100_exit:

";
            AssertProgram(sExp, program);
        }

        [Test]
        public void Scanner_Trampoline()
        {
            var scan = CreateScanner(0x1000, 0x2000);
            var platform = mr.Stub<Platform>(null, program.Architecture);
            platform.Stub(p => p.GetTrampolineDestination(null, null))
                .IgnoreArguments()
                .Return(new ExternalProcedure("bar", new ProcedureSignature()));
            platform.Stub(p => p.LookupProcedureByName("foo.dll", "bar")).Return(
                new ExternalProcedure("bar", new ProcedureSignature()));
            program.Platform = platform;
            Given_Trace(new RtlTrace(0x1000)
            {
                m => { m.Goto(m.LoadDw(m.Word32(0x2000))); }
            });
            program.ImportReferences.Add(
                Address.Ptr32(0x2000),
                new NamedImportReference(Address.Ptr32(0x2000), "foo.dll", "bar"));
            mr.ReplayAll();

            var proc = scan.ScanProcedure(Address.Ptr32(0x1000), "fn1000", arch.CreateProcessorState());

            Assert.AreEqual("bar", proc.Name);
        }

        [Test]
        public void Scanner_EvenOdd()
        {
            var scan = CreateScanner(0x1000, 0x2000);
            var platform = mr.Stub<Platform>(null, program.Architecture);
            program.Platform = platform;
            fakeArch.Test_AddTraces(RtlEvenOdd.Create(fakeArch));

            var proc = scan.ScanProcedure(
                Address.Ptr32(0x1000),
                "fn1000",
                arch.CreateProcessorState());
            var sExp = @"// fn1000
// Return size: 4
void fn1000()
fn1000_entry:
l00001000:
	r63 = fp
	r1 = 0x00000003
	r63 = r63 - 0x00000004
	Mem0[r63:word32] = r1
	call fn00001200 (retsize: 4;)
	r63 = r63 + 0x00000008
	r1 = 0x00000003
	r63 = r63 - 0x00000004
	Mem0[r63:word32] = r1
	call fn00001100 (retsize: 4;)
	r63 = r63 + 0x00000008
	return
fn1000_exit:

// fn00001100
// Return size: 0
void fn00001100()
fn00001100_entry:
l00001100:
	r63 = fp
	r1 = Mem0[r63 + 0x00000004:word32]
	branch r1 == 0x00000000 l00001120
	goto l00001108
l00001100:
l00001108:
	r1 = Mem0[r63 + 0x00000004:word32]
	r1 = r1 - 0x00000001
	Mem0[r63 + 0x00000004:word32] = r1
l00001114_thunk_fn00001200:
	call fn00001200 (retsize: 0;)
	return
l00001120:
	r1 = 0x00000000
	return
fn00001100_exit:

// fn00001200
// Return size: 0
void fn00001200()
fn00001200_entry:
l00001200:
	r63 = fp
	r1 = Mem0[r63 + 0x00000004:word32]
	branch r1 == 0x00000000 l00001220
l00001208:
	r1 = Mem0[r63 + 0x00000004:word32]
	r1 = r1 - 0x00000001
	Mem0[r63 + 0x00000004:word32] = r1
l00001214_thunk_fn00001100:
	call fn00001100 (retsize: 0;)
	return
l00001220:
	r1 = 0x00000001
	return
fn00001200_exit:

";
            AssertProgram(sExp, program);
        }

        [Test]
        public void Scanner_ResolveInterceptedCall()
        {
            var scanner = CreateScanner(0x1000, 0x2000);
            var addrEmulated = Address.Ptr32(0x5000);
            var addrThunk = Address.Ptr32(0x1800);
            program.Image.WriteLeUInt32(addrThunk, addrEmulated.ToUInt32());
            program.InterceptedCalls.Add(addrEmulated, new ExternalProcedure("Foo", null));
            var ep = scanner.GetInterceptedCall(addrThunk);
            Assert.AreEqual("Foo", ep.Name);
        }
    }
}
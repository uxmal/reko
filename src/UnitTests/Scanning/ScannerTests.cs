#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using Reko.UnitTests.Scanning.Fragments;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class ScannerTests
    {
        private IProcessorArchitecture arch;
        private FakeArchitecture fakeArch;
        private Program program;
        private TestScanner scan;
        private Identifier reg1;
        private Mock<IDynamicLinker> dynamicLinker;
        private IDictionary<Address, FunctionType> callSigs;
        private ServiceContainer sc;
        private Project project;
        private ByteMemoryArea bmem;
        private DecompilerEventListener eventListener;

        public class TestScanner : Scanner
        {
            public TestScanner(Program program, TypeLibrary metadata, IDynamicLinker dynamicLinker,
                IServiceProvider services)
                : base(program, metadata, dynamicLinker, services)
            {
            }

            public BlockWorkitem Test_LastBlockWorkitem { get; private set; }
            public ProcessorState Test_State { get; private set; }

            public override BlockWorkitem CreateBlockWorkItem(Address addrStart, Procedure proc, ProcessorState state)
            {
                Test_State = state;
                Test_LastBlockWorkitem = base.CreateBlockWorkItem(addrStart, proc, state);
                return Test_LastBlockWorkitem;
            }
        }

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            fakeArch = new FakeArchitecture(sc);
            dynamicLinker = new Mock<IDynamicLinker>();
            callSigs = new Dictionary<Address, FunctionType>();
            this.eventListener = new FakeDecompilerEventListener();
            arch = fakeArch;
            var r1 = fakeArch.GetRegister(1);
            reg1 = new Identifier(r1.Name, PrimitiveType.Word32, r1);
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            sc.AddService<DecompilerEventListener>(eventListener);
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
        }

        private FunctionType CreateSignature(string ret, params string[] args)
        {
            var retReg = new Identifier(ret, PrimitiveType.Word32, new RegisterStorage(ret, 0, 0, PrimitiveType.Word32));
            var argIds = new List<Identifier>();
            foreach (var arg in args)
            {
                argIds.Add(new Identifier(arg, PrimitiveType.Word32,
                    new RegisterStorage(ret, argIds.Count + 1, 0, PrimitiveType.Word32)));
            }
            return FunctionType.Func(retReg, argIds.ToArray());
        }

        private void BuildX86RealTest(Action<X86Assembler> test)
        {
            var addr = Address.SegPtr(0x0C00, 0);
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            var m = new X86Assembler(arch, addr, new List<ImageSymbol>());
            test(m);
            this.program = m.GetImage();
            this.program.Platform = new MsdosPlatform(sc, arch);
            this.scan = this.CreateScanner(this.program);
            var sym = ImageSymbol.Procedure(arch, addr);
            scan.EnqueueImageSymbol(sym, true);
        }

        private void AssertProgram(
            string sExpected, bool showEdges, bool lowLevelInfo,
            Program program)
        {
            var sw = new StringWriter();
            foreach (var proc in program.Procedures.Values)
            {
                proc.Write(false, showEdges, lowLevelInfo, sw);
                sw.WriteLine();
            }
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        private void AssertProgram(string sExpected, Program prog)
        {
            AssertProgram(sExpected, false, false, prog);
        }

        private SerializedType Char()
        {
            return new PrimitiveType_v1 { Domain = Domain.Character, ByteSize = 1 };
        }

        private SerializedType Int32()
        {
            return new PrimitiveType_v1 { Domain = Domain.Integer, ByteSize = 4 };
        }

        private SerializedType Real32()
        {
            return new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 4 };
        }

        [Test]
        public void Scanner_AddEntryPoint()
        {
            Given_Trace(new RtlTrace(0x12314) 
            {
                m => { m.Return(4, 0); }
            });
            Given_Program(Address.Ptr32(0x12314), new byte[1]);
            var project = new Project { Programs = { program } };

            var sc = new Scanner(
                this.program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, eventListener),
                this.sc);
            sc.EnqueueImageSymbol(ImageSymbol.Procedure(arch, Address.Ptr32(0x12314)), true);
            sc.ScanImage();

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0x12314, program.Procedures.Keys[0].Offset);
            Assert.IsTrue(program.CallGraph.EntryPoints.Contains(program.Procedures.Values[0]));
        }

        private void Given_Program(Address address, byte[] bytes)
        {
            var mem = new ByteMemoryArea(address, bytes);
            var segmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("proggie", mem, AccessMode.ReadExecute));
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            var platform = new FakePlatform(sc, arch);
            platform.Test_DefaultCallingConvention = "__cdecl";
            this.program = new Program
            {
                Architecture = arch,
                SegmentMap = segmentMap,
                Platform = platform
            };
            platform.Test_GetCallingConvention = (ccName) => {
                return new X86CallingConvention(4, 4, 4, true, false);
            };
        }

        private void Given_Project()
        {
            Assert.IsNotNull(program, "You must first call Given_Program or set the 'program' field.");
            this.project = new Project { Programs = { program } };
        }

        private void Given_Trace(RtlTrace trace)
        {
            fakeArch.Test_AddTrace(trace);
        }

        [Test]
        public void AddBlock()
        {
            var sc = CreateScanner(0x0100, 10);
            sc.AddBlock(Address.Ptr32(0x102), new Procedure(sc.Program.Architecture, "bob", Address.Ptr32(0x100), null), "l0102");
            Assert.IsNotNull(sc.FindExactBlock(Address.Ptr32(0x0102)));
        }

        private TestScanner CreateScanner(uint startAddress, int imageSize)
        {
            bmem = new ByteMemoryArea(Address.Ptr32(startAddress), new byte[imageSize]);
            program = new Program
            {
                SegmentMap = new SegmentMap(bmem.BaseAddress,
                    new ImageSegment("progseg", bmem, AccessMode.ReadExecute)),
                Architecture = arch,
                Platform = new FakePlatform(null, arch)
            };
            return new TestScanner(
                program,
                new TypeLibrary(),
                dynamicLinker.Object,
                sc);
        }

        private TestScanner CreateScanner(Program program)
        {
            this.program = program;
            return new TestScanner(
                program, 
                new TypeLibrary(),
                dynamicLinker.Object,
                sc);
        }

        private TestScanner CreateScanner(Program program, uint startAddress, int imageSize)
        {
            this.program = program;
            program.Architecture = arch;
            program.Platform = new FakePlatform(null, arch);
            this.bmem = new ByteMemoryArea(Address.Ptr32(startAddress), new byte[imageSize]);
            program.SegmentMap = new SegmentMap(
                bmem.BaseAddress,
                new ImageSegment("progseg", this.bmem, AccessMode.ReadExecute));
            return new TestScanner(program, project.LoadedMetadata, dynamicLinker.Object, sc);
        }

        private DataScanner CreateDataScanner(Program program)
        {
            this.program = program;
            var sr = new ScanResults()
            {
                KnownProcedures = new HashSet<Address>(),
            };
            return new DataScanner(program, sr,  eventListener);
        }

        [Test]
        public void Scanner_SplitBlock()
        {
            scan = CreateScanner(0x100, 0x100);
            var proc = new Procedure(program.Architecture, "foo", Address.Ptr32(0x00000100), arch.CreateFrame());
            Enqueue(Address.Ptr32(0x101), proc);
            Enqueue(Address.Ptr32(0x106), proc);
            Enqueue(Address.Ptr32(0x104), proc);

            Assert.AreEqual("l00000101", scan.FindContainingBlock(Address.Ptr32(0x103)).DisplayName);
            Assert.AreEqual("l00000104", scan.FindContainingBlock(Address.Ptr32(0x105)).DisplayName);
            Assert.AreEqual("l00000106", scan.FindContainingBlock(Address.Ptr32(0x106)).DisplayName);
        }

        private void Enqueue(Address addr, Procedure proc)
        {
            fakeArch.Test_AddTrace(new RtlTrace(addr.ToUInt32())
            {
                m => {
                    m.Assign(m.Mem32(m.Word32(0x3000)), m.Word32(42));
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
            var arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            program = new Program();
            program.Architecture = arch;
            var addr = Address.SegPtr(0xC00, 0);
            var m = new X86Assembler(arch, addr, new List<ImageSymbol>());
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

            program = m.GetImage();
            program.Platform = new FakePlatform(null, arch);
            Given_Project();

            var scan = new Scanner(
                program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, eventListener),
                sc);
            var sym = ImageSymbol.Procedure(arch, addr);
            scan.EnqueueImageSymbol(sym, true);
            scan.ScanImage();

            Assert.AreEqual(4, program.Procedures.Count);
        }

        [Test]
        public void Scanner_RepeatUntilBlock()
        {
            BuildX86RealTest(m =>
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
define fn0C00_0000
fn0C00_0000_entry:
	sp = fp
	Top = 0<i8>
	// succ:  l0C00_0000
l0C00_0000:
	ax = 0<16>
	// succ:  l0C00_0003
l0C00_0003:
	Mem0[ds:si + 0<16>:byte] = 0<8>
	si = si + 1<16>
	SZO = cond(si)
	cx = cx - 1<16>
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
            arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32", new Dictionary<string, object>());
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
                    "grox",
                    SymbolType.ExternalProcedure));
            Given_Trace(new RtlTrace(0x2000) {
                    m => m.SideEffect(m.Word32(0x1234))
            });
            var groxSig = CreateSignature("ax", "bx");
            var grox = new ExternalProcedure("grox", groxSig);

            dynamicLinker.Setup(i => i.ResolveProcedure(
                "module",
                "grox",
                It.IsNotNull<IPlatform>())).Returns(grox);

            var scan = CreateScanner(program, 0x1000, 0x2000);
            var proc = scan.ScanProcedure(arch, Address.Ptr32(0x2000), "fn000020", arch.CreateProcessorState());
            Assert.AreEqual("grox", proc.Name);
            Assert.AreEqual("ax", proc.Signature.ReturnValue.Name);
            Assert.AreEqual("bx", proc.Signature.Parameters[0].Name);
        }

        [Test]
        public void Scanner_IsLinearReturning_EmptyBlock()
        {
            var scanner = CreateScanner(0x1000, 0x1000);
            var addr = Address.Ptr32(0x00001000);
            var proc = new Procedure(program.Architecture, "fn1000", addr, arch.CreateFrame());
            var block = new Block(proc, addr, "l1000");
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
            m.Assign(m.Register("r0"), m.Register("r1"));

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
                m => { m.Assign(m.Mem32(m.Ptr32(0x1800)), reg1); },
                m => { m.Return(0, 0); }
            });
            Given_Trace(new RtlTrace(0x1004)
            {
                m => { m.Assign(m.Mem32(m.Ptr32(0x1800)), reg1); },
                m => { m.Return(0, 0); }
            });
            Given_Trace(new RtlTrace(0x1100)
            {
                m => { m.Assign(reg1, m.Word32(1)); },
                m => { m.Goto(Address.Ptr32(0x1004)); },
            });
            fakeArch.Test_IgnoreAllUnkownTraces();

            scan.EnqueueImageSymbol(ImageSymbol.Procedure(arch, Address.Ptr32(0x1000), state: arch.CreateProcessorState()), true);
            scan.EnqueueImageSymbol(ImageSymbol.Procedure(arch, Address.Ptr32(0x1100), state: arch.CreateProcessorState()), true);
            scan.ScanImage();

            var sExp =
@"// fn00001000
// Return size: 0
define fn00001000
fn00001000_entry:
	r63 = fp
l00001000:
	r1 = 0<32>
l00001004:
	Mem0[0x00001800<p32>:word32] = r1
	return
fn00001000_exit:

// fn00001100
// Return size: 0
define fn00001100
fn00001100_entry:
	r63 = fp
	goto l00001100
l00001004_in_fn00001100:
	Mem0[0x00001800<p32>:word32] = r1
	return
l00001100:
	r1 = 1<32>
	goto l00001004_in_fn00001100
fn00001100_exit:

";
            AssertProgram(sExp, program);
        }

        [Test]
        public void Scanner_CommonJump()
        {
            var scan = CreateScanner(0x1000, 0x2000);
            Given_Trace(new RtlTrace(0x1000)
            {
                // 0x1000<16>:
                m => { m.Assign(reg1, m.Word32(0)); },
                // 0x1004<16>:
                m => { m.Goto(Address.Ptr32(0x1100)); }
            });
            Given_Trace(new RtlTrace(0x1100)
            {
                // 0x1100<16>:
                m => { m.Assign(reg1, m.Word32(1)); },
                // 0x1104<16>:
                m => { m.Goto(Address.Ptr32(0x1004)); },
            });
            fakeArch.Test_IgnoreAllUnkownTraces();

            scan.EnqueueImageSymbol(
                ImageSymbol.Procedure(arch, Address.Ptr32(0x1000), state: arch.CreateProcessorState()),
                true);
            scan.ScanImage();

            var sExp =
@"// fn00001000
// Return size: 0
define fn00001000
fn00001000_entry:
	r63 = fp
	// succ:  l00001000
l00001000:
	r1 = 0<32>
	// succ:  l00001004
l00001004:
	// succ:  l00001100
l00001100:
	r1 = 1<32>
	goto l00001004
	// succ:  l00001004
fn00001000_exit:

";
            AssertProgram(sExp, true, false, program);
        }

        [Test]
        public void Scanner_JumpToUserProcedureEntry()
        {
            var scan = CreateScanner(0x1000, 0x2000);
            Given_Trace(new RtlTrace(0x1000)
            {
                // 0x1000:
                m => { m.Goto(Address.Ptr32(0x1100)); }
            });
            Given_Trace(new RtlTrace(0x1100)
            {
                // 0x1100:
                m => { m.Assign(reg1, m.Word32(1)); },
                // 0x1104:
                m => { m.Return(4, 8); },
            });
            var address = Address.Ptr32(0x1100);
            var userProcedure = new UserProcedure(address, "user_proc");
            scan.EnqueueUserProcedure(arch, userProcedure);
            scan.EnqueueImageSymbol(
                ImageSymbol.Procedure(
                    arch,
                    Address.Ptr32(0x1000),
                    state: arch.CreateProcessorState()),
                true);

            scan.ScanImage();

            var sExp =
            #region Expected
@"// fn00001000
// Return size: 4
define fn00001000
// stackDelta: 12; fpuStackDelta: 0; fpuMaxParam: -1

fn00001000_entry:
	r63 = fp
l00001000:
l00001000_thunk_user_proc:
	call user_proc (retsize: 0;)
	return
fn00001000_exit:

// user_proc
// Return size: 4
define user_proc
// stackDelta: 12; fpuStackDelta: 0; fpuMaxParam: -1

user_proc_entry:
	r63 = fp
l00001100:
	r1 = 1<32>
	return
user_proc_exit:

";
            #endregion
            AssertProgram(sExp, false, true, program);
        }

        [Test]
        public void Scanner_Trampoline()
        {
            var scan = CreateScanner(0x1000, 0x2000);
            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.GetTrampolineDestination(
                It.IsAny<Address>(),
                It.IsAny<IEnumerable<RtlInstruction>>(),
                It.IsAny<IRewriterHost>()))
                .Returns(new ExternalProcedure("bar", new FunctionType()));
            platform.Setup(p => p.LookupProcedureByName("foo.dll", "bar")).Returns(
                new ExternalProcedure("bar", new FunctionType()));
            program.Platform = platform.Object;
            Given_Trace(new RtlTrace(0x1000)
            {
                m => { m.Goto(m.Mem32(m.Word32(0x2000))); }
            });
            program.ImportReferences.Add(
                Address.Ptr32(0x2000),
                new NamedImportReference(Address.Ptr32(0x2000), "foo.dll", "bar", SymbolType.ExternalProcedure));

            var proc = scan.ScanProcedure(arch, Address.Ptr32(0x1000), "fn1000", arch.CreateProcessorState());

            Assert.AreEqual("bar", proc.Name);
        }

        [Test]
        public void Scanner_NoDecompiledProcedure()
        {
            Given_Program(Address.Ptr32(0x1000), new byte[0x2000]);
            var address = Address.Ptr32(0x2000);
            program.User.Procedures.Add(
                address,
                new UserProcedure(address, "ndProc")
                {
                    CSignature = "int ndProc(double dVal)",
                    Decompile = false,
                }
            );

            var sc = CreateScanner(program);
            var proc = sc.ScanProcedure(arch, Address.Ptr32(0x2000), "fn000020", arch.CreateProcessorState());
            Assert.AreEqual("ndProc", proc.Name);
            Assert.AreEqual("int32", proc.Signature.ReturnValue.DataType.ToString());
            Assert.AreEqual("eax", proc.Signature.ReturnValue.Storage.Name);
            Assert.AreEqual(1, proc.Signature.Parameters.Length);
            Assert.AreEqual("real64", proc.Signature.Parameters[0].DataType.ToString());
            Assert.AreEqual("dVal", proc.Signature.Parameters[0].Name);
        }

        [Test]
        public void Scanner_NoDecompiledProcedure_NullSignature()
        {
            Given_Program(Address.Ptr32(0x1000), new byte[0x2000]);
            var address = Address.Ptr32(0x2000);
            program.User.Procedures.Add(
                address,
                new UserProcedure(address, NamingPolicy.Instance.ProcedureName(address))
                {
                    CSignature = null,
                    Decompile = false,
                }
            );

            var sc = CreateScanner(program);
            var proc = sc.ScanProcedure(
                arch,
                Address.Ptr32(0x2000),
                "fn000020", arch.CreateProcessorState());
            Assert.False(proc.Signature.ParametersValid);
        }

        [Test]
        public void Scanner_EnqueueUserProcedure()
        {
            Given_Program(Address.Ptr32(0x1000), new byte[0x2000]);

            var sc = CreateScanner(program);
            sc.EnqueueUserProcedure(
                program.Architecture,
                new UserProcedure(Address.Ptr32(0x1000), "proc1")
                {
                    Decompile = false
                });
            sc.EnqueueUserProcedure(
                program.Architecture,
                new UserProcedure(Address.Ptr32(0x1020), "proc2")
                );
            sc.ScanImage();
            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual("proc2", program.Procedures.Values[0].Name);
        }

        [Test]
        public void Scanner_NoDecompiledEntryPoint()
        {
            Given_Program(Address.Ptr32(0x12314), new byte[20]);
            var address = Address.Ptr32(0x12314);
            program.User.Procedures.Add(
                address,
                new UserProcedure(address, NamingPolicy.Instance.ProcedureName(address))
                {
                    Decompile = false,
                }
            );

            var sc = CreateScanner(program);
            sc.EnqueueImageSymbol(
                ImageSymbol.Procedure(program.Architecture, Address.Ptr32(0x12314)),
                true);
            sc.EnqueueImageSymbol(
                ImageSymbol.Procedure(program.Architecture, Address.Ptr32(0x12324)),
                true);
            sc.ScanImage();

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0x12324, program.Procedures.Keys[0].Offset);
        }

        [Test]
        public void Scanner_NoDecompiledProcedureFromUserGlobal()
        {
            Given_Program(Address.Ptr32(0x12314), new byte[20]);
            fakeArch.Test_IgnoreAllUnkownTraces();
            var address = Address.Ptr32(0x12314);
            program.User.Procedures.Add(
                address,
                new UserProcedure(address, NamingPolicy.Instance.ProcedureName(address))
                {
                    Decompile = false,
                }
            );

            var sc = CreateScanner(program);
            sc.EnqueueUserProcedure(
                arch,
                Address.Ptr32(0x12314),
                FunctionType.Action(),
                null);
            sc.EnqueueUserProcedure(
                arch,
                Address.Ptr32(0x12324),
                FunctionType.Action(),
                null);

            sc.ScanImage();

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0x12324, program.Procedures.Keys[0].Offset);
        }

        [Test]
        public void Scanner_ScanData_NoDecompiledProcedureFromUserGlobal()
        {
            Given_Program(Address.Ptr32(0x12314), new byte[20]);
            var address = Address.Ptr32(0x12314);
            program.User.Procedures.Add(
                address,
                new UserProcedure(address, NamingPolicy.Instance.ProcedureName(address))
                {
                    Decompile = false,
                }
            );

            var sc = CreateDataScanner(program);
            sc.EnqueueUserProcedure(
                program.Architecture,
                Address.Ptr32(0x12314),
                FunctionType.Action(),
                null);
            sc.EnqueueUserProcedure(
                program.Architecture,
                Address.Ptr32(0x12324),
                FunctionType.Action(),
                null);
            sc.ProcessQueue();

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(0x12324, program.Procedures.Keys[0].Offset);
        }

        [Test]
        public void Scanner_EvenOdd()
        {
            var scan = CreateScanner(0x1000, 0x2000);
            var platform = new DefaultPlatform(null, program.Architecture);
            program.Platform = platform;
            fakeArch.Test_AddTraces(RtlEvenOdd.Create(fakeArch));

            scan.ScanProcedure(
                program.Architecture, 
                Address.Ptr32(0x1000),
                "fn1000",
                arch.CreateProcessorState());
            var sExp = @"// fn1000
// Return size: 4
define fn1000
fn1000_entry:
	r63 = fp
l00001000:
	r1 = 3<32>
	r63 = r63 - 4<32>
	Mem0[r63:word32] = r1
	call fn00001200 (retsize: 4;)
	r63 = r63 + 4<32>
	r1 = 3<32>
	r63 = r63 - 4<32>
	Mem0[r63:word32] = r1
	call fn00001100 (retsize: 4;)
	r63 = r63 + 4<32>
	return
fn1000_exit:

// fn00001100
// Return size: 4
define fn00001100
fn00001100_entry:
	r63 = fp
l00001100:
	r1 = Mem0[r63 + 4<32>:word32]
	branch r1 == 0<32> l00001120
	goto l00001108
l00001100:
l00001108:
	r1 = Mem0[r63 + 4<32>:word32]
	r1 = r1 - 1<32>
	Mem0[r63 + 4<32>:word32] = r1
	goto 0x00001200<p32>
l00001114_thunk_fn00001200:
	call fn00001200 (retsize: 0;)
	return
l00001120:
	r1 = 0<32>
	return
fn00001100_exit:

// fn00001200
// Return size: 4
define fn00001200
fn00001200_entry:
	r63 = fp
l00001200:
	r1 = Mem0[r63 + 4<32>:word32]
	branch r1 == 0<32> l00001220
l00001208:
	r1 = Mem0[r63 + 4<32>:word32]
	r1 = r1 - 1<32>
	Mem0[r63 + 4<32>:word32] = r1
	goto 0x00001100<p32>
l00001214_thunk_fn00001100:
	call fn00001100 (retsize: 0;)
	return
l00001220:
	r1 = 1<32>
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
            bmem.WriteLeUInt32(addrThunk, addrEmulated.ToUInt32());
            program.InterceptedCalls.Add(
                addrEmulated,
                new ExternalProcedure("Foo", new FunctionType()));
            var ep = scanner.GetInterceptedCall(program.Architecture, addrThunk);
            Assert.AreEqual("Foo", ep.Name);
        }

        [Test]
        public void Scanner_ScanProcedure_AssumeRegisterValues()
        {
            var scanner = CreateScanner(0x1000, 0x2000);

            var address = Address.Ptr32(0x1000);
            program.User.Procedures.Add(Address.Ptr32(0x1000), new UserProcedure(address, NamingPolicy.Instance.ProcedureName(address))
            {
                Assume = {
                     new RegisterValue_v2 { Register="r1", Value="0DC0" }
                }
            });
            Given_Trace(new RtlTrace(0x1000)
            {
                m => { m.Return(0, 0); }
            });
            var proc = (Procedure) scanner.ScanProcedure(
                arch,
                Address.Ptr32(0x1000), 
                "fnFoo",
                arch.CreateProcessorState());

            var r1 = proc.Frame.EnsureIdentifier(arch.GetRegister("r1"));
            Assert.AreEqual("0xDC0<32>", scanner.Test_State.GetValue(r1).ToString());
        }

        [Test(Description = "EntryPoints with no discernible type should not crash")]
        public void Scanner_Regression_1()
        {
            var scanner = CreateScanner(0x1000, 0x2000);
            Given_Trace(new RtlTrace(0x1000)
            {
                m => { m.Return(0, 0); }
            });

            scanner.ScanImageSymbol(
                ImageSymbol.Procedure(arch, Address.Ptr32(0x1000), "test", state: arch.CreateProcessorState()),
                true);

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual("test", program.Procedures[Address.Ptr32(0x1000)].Name);
        }

        [Test]
        public void Scanner_GlobalData()
        {
            var bytes = new byte[] {
                0x48, 0x00, 0x21, 0x43, 0x00, 0x00, 0x00, 0x01, 0x53, 0x00, 0x21, 0x43,
                0x28, 0x00, 0x21, 0x43, 0x00, 0x00, 0x00, 0x02, 0x63, 0x00, 0x21, 0x43,
                0x38, 0x00, 0x21, 0x43, 0x00, 0x00, 0x00, 0x03, 0x73, 0x00, 0x21, 0x43,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };
            Given_Program(Address.Ptr32(0x43210000), bytes);
            var project = new Project { Programs = { program } };

            var sc = new Scanner(
                this.program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, eventListener),
                this.sc
            );

            var ft1 = Given_Serialized_Signature(new SerializedSignature
            {
                Convention = "__cdecl",
                ReturnValue = new Argument_v1 { Type = Int32() },
            });
            var ft2 = Given_Serialized_Signature(new SerializedSignature
            {
                ReturnValue = new Argument_v1 { Type = Char() }
            });
            var str = new StructureType();
            var fields = new StructureField[] {
                new StructureField(0, new Pointer(ft1, 32), "A"),
                new StructureField(4, PrimitiveType.Int32, "B"),
                new StructureField(8, new Pointer(ft2, 32), "C"),
            };
            str.Fields.AddRange(fields);
            var elementType = new TypeReference("test", str);
            var arrayType = new ArrayType(elementType, 3);

            sc.EnqueueUserGlobalData(Address.Ptr32(0x43210000), arrayType, null);
            sc.ScanImage();

            var sExpSig1 =
@"Register ui32 sig1()
// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1
";
            var sExpSig2 =
@"Register char sig2()
// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(6, program.Procedures.Count);
            Assert.AreEqual(sExpSig1, program.Procedures[Address.Ptr32(0x43210028)].Signature.ToString("sig1", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(sExpSig1, program.Procedures[Address.Ptr32(0x43210038)].Signature.ToString("sig1", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(sExpSig1, program.Procedures[Address.Ptr32(0x43210048)].Signature.ToString("sig1", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(sExpSig2, program.Procedures[Address.Ptr32(0x43210053)].Signature.ToString("sig2", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(sExpSig2, program.Procedures[Address.Ptr32(0x43210063)].Signature.ToString("sig2", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(sExpSig2, program.Procedures[Address.Ptr32(0x43210073)].Signature.ToString("sig2", FunctionType.EmitFlags.AllDetails));
        }

        private DataType Given_Serialized_Signature(SerializedSignature sSignature)
        {
            var tldeser = new TypeLibraryDeserializer(
                   program.Platform,
                   true,
                   new TypeLibrary());
            return sSignature.Accept(tldeser);
        }

        [Test]
        public void Scanner_GlobalDataRecursiveStructs()
        {
            var bytes = new byte[] {
                0x17, 0x00, 0x21, 0x43, 0x00, 0x00, 0x21, 0x43,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };
            Given_Program(Address.Ptr32(0x43210000), bytes);
            Given_Project();

            var sc = new Scanner(
                this.program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, eventListener),
                this.sc
            );

            var ft = Given_Serialized_Signature(new SerializedSignature
            { 
                 ReturnValue = new Argument_v1 { Type = Real32() },
            });
            var str = new StructureType();
            var fields = new StructureField[] {
                new StructureField(0, new Pointer(ft,  32), "func"),
                new StructureField(4, new Pointer(str, 32), "next"),
            };
            str.Fields.AddRange(fields);

            sc.EnqueueUserGlobalData(Address.Ptr32(0x43210000), str, null);
            sc.ScanImage();

            var sExpSig =
@"FpuStack real32 fn43210017()
// stackDelta: 4; fpuStackDelta: 1; fpuMaxParam: -1
";
            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual(sExpSig, program.Procedures[Address.Ptr32(0x43210017)].Signature.ToString("fn43210017", FunctionType.EmitFlags.AllDetails));
        }

        [Test(Description = "Scanner should be able to handle structures with padding 'holes'")]
        public void Scanner_GlobalData_StructWithPadding()
        {
            var bytes = new byte[]
            {
                0x03, 0x00,             // Type field (halfword)
                0x00, 0x00,             // ...alignment padding

                0x08, 0x0, 0x21, 0x43,  // pointer to function

                0xC3,                   // function code.
            };
            Given_Program(Address.Ptr32(0x43210000), bytes);
            Given_Project();

            var ft = FunctionType.Func(
                new Identifier("", PrimitiveType.Real32, null),
                new Identifier[0]);
            var str = new StructureType();
            str.Fields.AddRange(new StructureField[]
            {
                new StructureField(0, PrimitiveType.Word16, "typeField"),
                // two-byte gap here.
                new StructureField(4, new Pointer(ft, 32), "pfn")
            });

            var scanner = new Scanner(
                this.program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, eventListener),
                this.sc);
            scanner.EnqueueUserGlobalData(Address.Ptr32(0x43210000), str, null);
            scanner.ScanImage();

            Assert.AreEqual(1, program.Procedures.Count, "Scanner should have detected the pointer to function correctly.");
            Assert.AreEqual(Address.Ptr32(0x43210008), program.Procedures.Keys.First());
        }

        [Test(Description ="User-supplied signatures should be respected")]
        public void Scanner_UserProcedure_GenerateSignature()
        {
            Given_Program(Address.Ptr32(0x00100000), new byte[100]);
            Given_Project();
            var address = Address.Ptr32(0x00100010);
            program.User.Procedures.Add(
                address,
                new UserProcedure(address, NamingPolicy.Instance.ProcedureName(address))
                {
                    CSignature = "int foo(char * a, float b)"
                });

            var scanner = new Scanner(
                this.program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, eventListener),
                this.sc);
            var proc = scanner.ScanProcedure(
                program.Architecture,
                Address.Ptr32(0x00100010),
                null,
                fakeArch.CreateProcessorState());

            Assert.AreEqual("foo", proc.Name);
            Assert.AreEqual("Register int32 foo(Stack (ptr32 char) a, Stack real32 b)", proc.Signature.ToString(proc.Name));
        }

        [Test(Description = "Should discover pointer to function and record it in ScanResults.")]
        public void Scanner_ScanData_ImageSymbols()
        {
            Given_Program(Address.Ptr32(0x00100000), new byte[] {
                0x42, 0x42, 0x42, 0x42,
                0x10, 0x00, 0x10, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0xC3, 0x00, 0x00, 0x00,
            });
            Given_Project();
            var dt = new StructureType
            {
                Fields =
                {
                    { 0,  PrimitiveType.Int32, "data" },
                    { 4,  new Pointer(FunctionType.Action(
                            new Identifier("arg", PrimitiveType.Int32, null)),
                            32)
                    }
                }
            };
            var sym = ImageSymbol.DataObject(arch, Address.Ptr32(0x00100000), "data_blob", dt);
            program.ImageSymbols.Add(sym.Address, sym);

            var scanner = new Scanner(program, project.LoadedMetadata, dynamicLinker.Object, sc);
            var sr = scanner.ScanDataItems();
            Assert.AreEqual(1, sr.KnownProcedures.Count);
            Assert.AreEqual("00100010", sr.KnownProcedures.First().ToString());
        }

        [Test(Description = "Procedures in metadata should be respected")]
        public void Scanner_Metadata_procedure()
        {
            Given_Program(Address.Ptr32(0x00100000), new byte[100]);
            Given_Project();
            var address = Address.Ptr32(0x00100010);
            project.LoadedMetadata.Procedures.Add(address, ("foo", FunctionType.Action()));

            var scanner = new Scanner(
                this.program,
                project.LoadedMetadata,
                new DynamicLinker(project, program, eventListener),
                this.sc);
            scanner.ScanImage();

            Assert.AreEqual(1, program.Procedures.Count);
            Assert.AreEqual("(fn void ())", program.Procedures.Values.First().Signature.ToString());
        }
    }
}

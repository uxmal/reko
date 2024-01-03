#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Reko.Core.Memory;
using Reko.Services;
using Reko.Core.Loading;
using Reko.Core.Analysis;
using Reko.Core.Intrinsics;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class BlockWorkitemTests
    {
        private Mock<IScannerServices> scanner;
        private Mock<IProcessorArchitecture> arch;
        private Program program;
        private Procedure proc;
        private Block block;
        private RtlTrace trace;
        private Identifier r0;
        private Identifier r1;
        private Identifier r2;
        private Identifier sp;
        private Identifier grf;

        [SetUp]
        public void Setup()
        {
            program = new Program();
            trace = new RtlTrace(0x00100000);
            r0 = new Identifier("r0", PrimitiveType.Word32, RegisterStorage.Reg32("r0", 0));
            r1 = new Identifier("r1", PrimitiveType.Word32, RegisterStorage.Reg32("r1", 1));
            r2 = new Identifier("r2", PrimitiveType.Word32, RegisterStorage.Reg32("r2", 2));
            sp = new Identifier("sp", PrimitiveType.Word32, RegisterStorage.Reg32("sp", 15));
            var sc = new ServiceContainer();
            var listener = new Mock<IDecompilerEventListener>();
            scanner = new Mock<IScannerServices>();
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("FakeArch");
            proc = new Procedure(arch.Object, "testProc", Address.Ptr32(0x00100000), new Frame(arch.Object, PrimitiveType.Word32));
            block = proc.AddBlock(proc.EntryAddress, "l00100000");
            grf = proc.Frame.EnsureFlagGroup(Registers.eflags, 3, "SCZ", PrimitiveType.Byte);
            program.Architecture = arch.Object;
            program.SegmentMap = new SegmentMap(
                Address.Ptr32(0x00100000),
                new ImageSegment(
                    ".text",
                    new ByteMemoryArea(Address.Ptr32(0x00100000), new byte[0x20000]),
                    AccessMode.ReadExecute));
            program.Platform = new DefaultPlatform(sc, arch.Object);
            arch.Setup(a => a.StackRegister).Returns((RegisterStorage)sp.Storage);
            arch.Setup(s => s.PointerType).Returns(PrimitiveType.Ptr32);
            arch.Setup(s => s.CreateFrameApplicationBuilder(
                It.IsAny<IStorageBinder>(),
                It.IsAny<CallSite>()))
                .Returns((IStorageBinder frame, CallSite site) =>
                    new FrameApplicationBuilder(arch.Object, frame, site));
            scanner.Setup(s => s.Services).Returns(sc);
            sc.AddService<IEventListener>(listener.Object);
            sc.AddService<IDecompilerEventListener>(listener.Object);
        }

        private BlockWorkitem CreateWorkItem(Address addr)
        {
            return CreateWorkItem(addr, new FakeProcessorState(arch.Object));
        }

        private BlockWorkitem CreateWorkItem(Address addr, ProcessorState state)
        {
            return new BlockWorkitem(
                scanner.Object,
                program,
                program.Architecture,
                state,
                addr);
        }

        private FunctionType CreateSignature(RegisterStorage ret, params RegisterStorage[] args)
        {
            var retReg = proc.Frame.EnsureRegister(ret);
            var argIds = new List<Identifier>();
            foreach (var arg in args)
            {
                argIds.Add(proc.Frame.EnsureRegister(arg));
            }
            return new FunctionType(retReg, argIds.ToArray());
        }

        private void Given_Segment(string segName, uint addr, int size, AccessMode mode)
        {
            program.SegmentMap.AddSegment(new ImageSegment(segName, new ByteMemoryArea(Address.Ptr32(addr), new byte[size]), mode));
        }

        private bool StashArg(ref ProcessorState state, ProcessorState value)
        {
            state = value;
            return true;
        }

        private void Given_TrashedRegisters(params Identifier[] regs)
        {
            program.Platform = new FakePlatform(null, arch.Object)
            {
                Test_CreateTrashedRegisters =
                    () => regs
                        .Select(id => (RegisterStorage)id.Storage)
                        .ToHashSet()
            };
        }

        private void Given_NoImportedProcedure()
        {
            scanner.Setup(s => s.GetImportedProcedure(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<Address>()))
                .Returns((ExternalProcedure)null);
        }

        private void Given_NoInlinedCall()
        {
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<ByteMemoryArea>(),
                It.IsAny<Address>()))
                .Returns(new LeImageReader(new ByteMemoryArea(Address.Ptr32(0),Array.Empty<byte>()), 0));
            arch.Setup(a => a.InlineCall(
                It.IsAny<Address>(),
                It.IsAny<Address>(),
                It.IsAny<EndianImageReader>(),
                It.IsAny<IStorageBinder>()))
                .Returns((List<RtlInstruction>)null);
        }

        private void Given_NoTrampoline()
        {
            scanner.Setup(s => s.GetTrampoline(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>()))
                .Returns((ProcedureBase)null);
        }

        private void Given_SimpleTrace(IEnumerable<RtlInstructionCluster> trace)
        {
            scanner.Setup(s => s.GetTrace(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<ProcessorState>(),
                It.IsAny<IStorageBinder>()))
                .Returns(trace);
        }

        private Address Given_Trace(uint uAddr, Action<RtlEmitter> generator)
        {
            var addr = Address.Ptr32(uAddr);
            trace.Add(generator);
            scanner.Setup(s => s.GetTrace(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<ProcessorState>(),
                It.IsAny<IStorageBinder>()))
                .Returns(trace);
            scanner.Setup(s => s.FindContainingBlock(It.IsAny<Address>()))
                .Returns(block);
            return addr;
        }

        /// <summary>
        /// Generate a CodePatch that ends in a Transfer instruction.
        /// </summary>
        private void Given_TransferPatch(uint uAddr, int length, Action<RtlEmitter> generateCode)
        {
            var addr = Address.Ptr32(uAddr);
            var m = new RtlEmitter(new List<RtlInstruction>());
            generateCode(m);
            var cluster = m.MakeCluster(addr, length, InstrClass.Transfer);
            program.User.Patches.Add(addr, new CodePatch(cluster));
        }


        private void Given_ExpectedBranchTarget(uint uAddrDst, string blockName)
        {
            var block = proc.AddBlock(Address.Ptr32(uAddrDst), blockName);
            scanner.Setup(x => x.EnqueueJumpTarget(
                It.IsNotNull<Address>(),
                It.Is<Address>(a => a.ToUInt32() == uAddrDst),
                block.Procedure,
                It.IsAny<ProcessorState>()))
                .Returns(block);
        }

        private void AssertBlockCode(string expected, Block block)
        {
            var actual = Environment.NewLine;
            foreach (var stm in block.Statements)
            {
                actual += stm.Instruction.ToString() + Environment.NewLine;
            }
            if (expected != actual)
            {
                Debug.Print(actual);
                Assert.AreEqual(expected, actual);
            }
        }

        private void AssertProcedureCode(string expected, Procedure proc)
        {
            var sw = new StringWriter();
            proc.WriteBody(false, sw);
            if (expected != sw.ToString())
            {
                Debug.WriteLine(sw.ToString());
                Assert.AreEqual(expected, sw.ToString());
            }
        }

        [Test]
        public void Bwi_RewriteReturn()
        {
            trace.Add(m => { m.Return(4, 0); });
            trace.Add(m => { m.SideEffect(m.Fn(m.Int32(0x49242))); });

            scanner.Setup(x => x.FindContainingBlock(
                It.IsAny<Address>())).Returns(block);
            //scanner.Setup(x => x.TerminateBlock(null, null)).IgnoreArguments();
            scanner.Setup(x => x.SetProcedureReturnAddressBytes(
                It.IsNotNull<Procedure>(),
                4,
                It.Is<Address>(a => a.ToLinear() == 0x00100000)));
            Given_SimpleTrace(trace);

            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.IsTrue(proc.ControlGraph.ContainsEdge(block, proc.ExitBlock), "Expected return to add an edge to the Exit block");
        }

        [Test]
        public void Bwi_StopOnGoto()
        {
            trace.Add(m =>
            {
                m.Assign(r0, m.Word32(3));
                m.Goto(Address.Ptr32(0x104000));
            });

            Block next = block.Procedure.AddBlock(Address.Ptr32(0x10004), "next");
            arch.Setup(x => x.PointerType).Returns(PrimitiveType.Ptr32);
            arch.Setup(x => x.CreateRewriter(
                It.IsAny<EndianImageReader>(),
                It.IsAny<ProcessorState>(),
                It.IsAny<IStorageBinder>(),
                It.IsAny<IRewriterHost>())).Returns(trace);
            scanner.Setup(x => x.FindContainingBlock(
                It.IsAny<Address>())).Returns(block);
            scanner.Setup(x => x.EnqueueJumpTarget(
                It.IsNotNull<Address>(),
                It.IsAny<Address>(),
                block.Procedure,
                It.IsAny<ProcessorState>()))
                .Returns(next)
                .Verifiable();
            //scanner.Setup(x => x.TerminateBlock(null, null)).IgnoreArguments();
            Given_SimpleTrace(trace);
            Given_NoImportedProcedure();
            Given_NoTrampoline();

            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();
            Assert.AreEqual(2, block.Statements.Count);
            Assert.AreEqual("r0 = 3<32>", block.Statements[0].ToString());
            Assert.AreEqual("goto 0x00104000<p32>", block.Statements[1].ToString());
            Assert.AreEqual(1, proc.ControlGraph.Successors(block).Count);
            var items = new List<Block>(proc.ControlGraph.Successors(block));
            Assert.AreSame(next, items[0]);

            scanner.Verify();
        }

        [Test]
        public void Bwi_HandleBranch()
        {
            trace.Add(m =>
                m.Branch(r1, Address.Ptr32(0x00104000), InstrClass.ConditionalTransfer));
            trace.Add(m =>
                m.Assign(r1, r2));
            var blockElse = new Block(proc, Address.Ptr32(0x00104010), "else");
            var blockThen = new Block(proc, Address.Ptr32(0x00104020), "then");
            ProcessorState s1 = null;
            ProcessorState s2 = null;
                arch.Setup(a => a.FramePointerType).Returns(PrimitiveType.Ptr32);
                arch.Setup(x => x.CreateRewriter(
                    It.IsAny<EndianImageReader>(),
                    It.IsAny<ProcessorState>(),
                    It.IsAny<IStorageBinder>(),
                    It.IsAny<IRewriterHost>())).Returns(trace);
                scanner.Setup(x => x.FindContainingBlock(
                    It.IsAny<Address>())).Returns(block);
            scanner.Setup(x => x.EnqueueJumpTarget(
                It.IsNotNull<Address>(),
                It.Is<Address>(arg => arg.Offset == 0x00100004),
                block.Procedure,
                It.Is<ProcessorState>(arg => StashArg(ref s1, arg))))
                .Returns(blockElse)
                .Verifiable();
            scanner.Setup(x => x.EnqueueJumpTarget(
                    It.IsNotNull<Address>(),
                    It.Is<Address>(arg => arg.Offset == 0x00104000),
                    block.Procedure,
                    It.Is<ProcessorState>(arg => StashArg(ref s2, arg))))
                    .Returns(blockThen)
                .Verifiable();
            Given_SimpleTrace(trace);
            
            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();
            Assert.AreEqual(1, block.Statements.Count, "Expected a branch statement in the block");
            Assert.AreNotSame(s1, s2);
            Assert.IsNotNull(s1);
            Assert.IsNotNull(s2);

            scanner.Verify();
        }

        [Test]
        public void Bwi_CallInstructionShouldAddNodeToCallgraph()
        {
            trace.Add(m => { m.Call(Address.Ptr32(0x102000), 4); });
            trace.Add(m => { m.Assign(m.Word32(0x4000), m.Word32(0)); });
            trace.Add(m => { m.Return(4, 0); });

            arch.Setup(x => x.CreateRewriter(
                It.IsAny<EndianImageReader>(),
                It.IsAny<ProcessorState>(),
                It.IsAny<IStorageBinder>(),
                It.IsAny<IRewriterHost>())).Returns(trace);
            arch.Setup(x => x.PointerType).Returns(PrimitiveType.Ptr32);
                Given_NoInlinedCall();
            Given_NoImportedProcedure();
                scanner.Setup(x => x.FindContainingBlock(
                    It.IsAny<Address>())).Returns(block);
            scanner.Setup(x => x.ScanProcedure(
                It.IsNotNull<IProcessorArchitecture>(),
                It.Is<Address>(arg => arg.Offset == 0x102000),
                null,
                It.IsAny<ProcessorState>()))
                        .Returns(new Procedure(
                            program.Architecture,
                            "fn102000",
                            Address.Ptr32(0x00102000),
                            new Frame(program.Architecture, PrimitiveType.Word32)))
                        .Verifiable();
                //scanner.Setup(x => x.TerminateBlock(null, null)).IgnoreArguments();
                scanner.Setup(x => x.SetProcedureReturnAddressBytes(
                    It.IsNotNull<Procedure>(),
                    4,
                    It.IsNotNull<Address>()));
                Given_SimpleTrace(trace);
            
            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();

            var callees = new List<Procedure>(program.CallGraph.Callees(block.Procedure));
            Assert.AreEqual(1, callees.Count);
            Assert.AreEqual("fn102000", callees[0].Name);
            scanner.Verify();
        }

        [Test]
        public void Bwi_CallingAllocaWithConstant()
        {
            var sc = new ServiceContainer();
            program.Architecture = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            program.Platform = new DefaultPlatform(sc, program.Architecture);
            var sig = CreateSignature(Registers.esp, Registers.eax);
            var alloca = new ExternalProcedure("alloca", sig)
            {
                Characteristics = new ProcedureCharacteristics
                {
                IsAlloca = true
                }
            };

            scanner.Setup(x => x.FindContainingBlock(
                It.IsAny<Address>())).Returns(block);
            scanner.Setup(x => x.GetImportedProcedure(
                It.IsNotNull<IProcessorArchitecture>(),
                It.Is<Address>(a => a.ToLinear() == 0x102000u),
                It.IsNotNull<Address>())).Returns(alloca);
            Given_SimpleTrace(trace);
            trace.Add(m => m.Call(Address.Ptr32(0x102000), 4));
            var state = new FakeProcessorState(program.Architecture);
            state.SetRegister(Registers.eax, Constant.Word32(0x0400));
            var wi = CreateWorkItem(Address.Ptr32(0x1000), state);
            wi.Process();

            scanner.Verify();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("esp = esp - 0x400<32>", block.Statements[^1].ToString());
        }

        [Test]
        public void Bwi_CallingAllocaWithNonConstant()
        {
            program.Platform = new DefaultPlatform(new ServiceContainer(), arch.Object);

            var sig = CreateSignature(Registers.esp, Registers.eax);
            var alloca = new ExternalProcedure("alloca", sig, new ProcedureCharacteristics
            {
                IsAlloca = true
            });

            trace.Add(m => m.Call(Address.Ptr32(0x102000), 4));

            scanner.Setup(x => x.FindContainingBlock(
                    It.IsAny<Address>())).Returns(block);
            scanner.Setup(x => x.GetImportedProcedure(
                    It.IsNotNull<IProcessorArchitecture>(),
                    Address.Ptr32(0x102000u),
                    It.IsNotNull<Address>())).Returns(alloca);
            Given_SimpleTrace(trace);

            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();

            scanner.Verify();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("esp = alloca(eax)", block.Statements[^1].ToString());
        }

        [Test]
        public void Bwi_CallTerminatingProcedure_StopScanning()
        {
            var frame = new Frame(program.Architecture, PrimitiveType.Ptr32);
            proc = Procedure.Create(program.Architecture, "proc", Address.Ptr32(0x102000), frame);
            var terminator = Procedure.Create(program.Architecture, "terminator", Address.Ptr32(0x0001000), program.Architecture.CreateFrame());
            terminator.Characteristics = new ProcedureCharacteristics {
                Terminates = true,
            };
            block = proc.AddBlock(proc.EntryAddress, "the_block");
            arch.Setup(a => a.PointerType).Returns(PrimitiveType.Word32);
            scanner.Setup(s => s.FindContainingBlock(It.IsAny<Address>())).Returns(block);
            Given_NoImportedProcedure();
            Given_NoInlinedCall();
            scanner.Setup(s => s.ScanProcedure(
                It.IsNotNull<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<string>(),
                It.IsAny<ProcessorState>()))
                .Returns(terminator)
                .Verifiable();
            scanner.Setup(s => s.TerminateBlock(It.IsNotNull<Block>(), It.IsNotNull<Address>())).Verifiable();
            arch.Setup(a => a.FramePointerType).Returns(PrimitiveType.Ptr32);
            Given_SimpleTrace(trace);

            trace.Add(m => m.Call(Address.Ptr32(0x00102000), 4));
            trace.Add(m => m.SideEffect(new ProcedureConstant(VoidType.Instance, new IntrinsicProcedure("shouldnt_decompile_this", true, VoidType.Instance, 0))));

            var wi = CreateWorkItem(Address.Ptr32(0x2000));
            wi.Process();

            Assert.AreEqual(1, block.Statements.Count, "Should only have rewritten the Call to 'terminator'");
            scanner.Verify();
        }

        [Test]
        public void Bwi_CallProcedureWithSignature()
        {
            var frame = new Frame(program.Architecture, PrimitiveType.Ptr32);
            var proc2 = new Procedure(program.Architecture, "fn2000", Address.Ptr32(0x2000), frame);
            var sig = FunctionType.Func(
                proc2.Frame.EnsureRegister(RegisterStorage.Reg32("r1", 1)),
                proc2.Frame.EnsureRegister(RegisterStorage.Reg32("r2", 2)),
                proc2.Frame.EnsureRegister(RegisterStorage.Reg32("r3", 3)));
            proc2.Signature = sig;
            var block2 = new Block(proc, Address.Ptr32(0x0010008), "l00100008");
            var block3 = new Block(proc, Address.Ptr32(0x0010004), "l00100004");
            arch.Setup(a => a.PointerType).Returns(PrimitiveType.Ptr32);
            scanner.Setup(s => s.FindContainingBlock(Address.Ptr32(0x00100000))).Returns(block);
            scanner.Setup(s => s.FindContainingBlock(Address.Ptr32(0x00100004))).Returns(block2);
            Given_NoImportedProcedure();
            Given_NoInlinedCall();
            scanner.Setup(s => s.EnqueueJumpTarget(
                It.IsNotNull<Address>(),
                It.Is<Address>(a => a.ToLinear() == 0x00100004),
                It.IsNotNull<Procedure>(),
                It.IsNotNull<ProcessorState>()))
                .Returns(block3);
            scanner.Setup(s => s.ScanProcedure(
                It.IsNotNull<IProcessorArchitecture>(),
                Address.Ptr32(0x102000),
                null,
                It.IsNotNull<ProcessorState>())).Returns(proc2);

            trace.Add(m => m.Call(Address.Ptr32(0x102000), 0));
            trace.Add(m => m.Return(0, 0));
            Given_SimpleTrace(trace);

            var wi = CreateWorkItem(Address.Ptr32(0x00100000));
            wi.Process();

            scanner.Verify();
            var sw = new StringWriter();
            proc.Write(false, sw);
            var sExp = @"// testProc
// Return size: 0
define testProc
testProc_entry:
l00100000:
	r1 = fn2000(r2, r3)
	goto l00100004
	// succ:  l00100004
testProc_exit:
";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void Bwi_IndirectCallMatchedByPlatform()
        {
            var platform =new Mock<IPlatform>();
            var reg0 = proc.Frame.EnsureRegister(new RegisterStorage("r0", 0, 0, PrimitiveType.Ptr32));
            var reg1 = proc.Frame.EnsureRegister(new RegisterStorage("r1", 1, 0, PrimitiveType.Ptr32));
            var sysSvc = new SystemService {
                Name = "SysSvc",
                Signature = FunctionType.Action(reg1),
                Characteristics = new ProcedureCharacteristics()
            };
            platform.Setup(p => p.FindService(
                It.IsAny<RtlInstruction>(),
                It.IsAny<ProcessorState>(),
                It.IsAny<SegmentMap>()))
                .Returns(sysSvc)
                .Verifiable();
            platform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            platform.Setup(p => p.ResolveIndirectCall(It.IsAny<RtlCall>()))
                .Returns((Address)null);
            platform.Setup(p => p.TrashedRegisters)
                .Returns(new HashSet<RegisterStorage>());
            program.Platform = platform.Object;
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Returns(block);
            Given_SimpleTrace(trace);

            trace.Add(m => m.Call(m.Mem32(m.IAdd(reg0, -32)), 4));
            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual("SysSvc(r1)", block.Statements[0].ToString());
            platform.Verify();
        }

        [Test]
        public void Bwi_IndirectJump()
        {
            var platform =new Mock<IPlatform>();
            var sp = proc.Frame.EnsureRegister(new RegisterStorage("sp", 14, 0, PrimitiveType.Ptr32));
            platform.Setup(p => p.FindService(
                It.IsAny<RtlInstruction>(),
                It.IsAny<ProcessorState>(),
                It.IsAny<SegmentMap>())).Returns((SystemService)null);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Returns(block);
            Given_SimpleTrace(trace);
            //scanner.Setup(s => s.TerminateBlock(null, null)).IgnoreArguments();

            trace.Add(m => m.Goto(m.Mem32(sp)));
            var wi = CreateWorkItem(Address.Ptr32(0x0100000));
            wi.Process();

            Assert.AreEqual("call Mem0[sp:word32] (retsize: 0;)", block.Statements[0].ToString());
            Assert.AreEqual("return", block.Statements[1].ToString());
            platform.Verify();
        }

        [Test]
        public void Bwi_Goto_DelaySlot()
        {
            var l00100008 = new Block(proc, Address.Ptr32(0x00100008), "l00100008");
            var l00100100 = new Block(proc, Address.Ptr32(0x00101000), "l00101000");
            Given_NoImportedProcedure();
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100008))).Returns(l00100008);
            Given_SimpleTrace(trace);
            //scanner.Setup(s => s.TerminateBlock(null, null)).IgnoreArguments();
            scanner.Setup(s => s.EnqueueJumpTarget(
                It.IsAny<Address>(),
                It.IsAny<Address>(),
                It.IsAny<Procedure>(),
                It.IsAny<ProcessorState>()))
                .Returns(l00100100);
            Given_NoTrampoline();

            trace.Add(m => m.GotoD(Address.Ptr32(0x0100100)));
            trace.Add(m => m.Assign(r0, r1));
            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual(2, block.Statements.Count);
            Assert.AreEqual("r0 = r1", block.Statements[0].ToString());
            Assert.AreEqual("goto 0x00100100<p32>", block.Statements[1].ToString());

            Assert.AreEqual("l00101000", block.Succ[0].DisplayName);
            scanner.Verify();
        }

        [Test]
        public void Bwi_Branch_DelaySlot()
        {
            var l00100008 = new Block(proc, Address.Ptr32(0x00100008), "l00100008");
            var l00100100 = new Block(proc, Address.Ptr32(0x00101000), "l00101000");
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100008))).Returns(l00100008);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x10000C))).Returns((Block)null);
            Given_SimpleTrace(trace);
            scanner.Setup(s => s.EnqueueJumpTarget(
                Address.Ptr32(0x00100004),
                Address.Ptr32(0x00101000),
                proc,
                It.IsNotNull<ProcessorState>())).Returns(l00100100);
            scanner.Setup(s => s.EnqueueJumpTarget(
                Address.Ptr32(0x00100004),
                Address.Ptr32(0x00100008),
                proc,
                It.IsNotNull<ProcessorState>())).Returns(l00100008);

            trace.Add(m => m.Branch(r1, Address.Ptr32(0x101000), InstrClass.ConditionalTransfer | InstrClass.Delay));
            trace.Add(m => m.Assign(r0, r1));   // 100004
            trace.Add(m => m.Assign(r2, r1));   // 100008

            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual("branch r1 l00100004_ds_t", block.Statements[0].ToString());
            var blFalse = block.ElseBlock;
            var blTrue = block.ThenBlock;
            Assert.AreEqual("l00100004_ds_f", blFalse.DisplayName);     // delay-slot-false
            Assert.AreEqual(1, blFalse.Statements.Count);
            Assert.AreEqual("r0 = r1", blFalse.Statements[0].ToString());
            Assert.AreEqual(1, blFalse.Succ.Count);
            Assert.AreEqual("l00100008", blFalse.Succ[0].DisplayName);

            Assert.AreEqual("l00100004_ds_t", blTrue.DisplayName);      // delay-slot-true
            Assert.AreEqual(1, blTrue.Statements.Count);
            Assert.AreEqual("r0 = r1", blTrue.Statements[0].ToString());
            Assert.AreEqual(1, blTrue.Succ.Count);
            Assert.AreEqual("l00101000", blTrue.Succ[0].DisplayName);
        }

        [Test]
        public void Bwi_Call_DelaySlot()
        {
            var l00100008 = new Block(proc, Address.Ptr32(0x00100008), "l00100008");
            var l00100100 = new Block(proc, Address.Ptr32(0x00101000), "l00101000");
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100008))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x10000C))).Returns((Block)null);
            Given_SimpleTrace(trace);
            //scanner.Setup(s => s.TerminateBlock(null, null)).IgnoreArguments();
            Given_NoImportedProcedure();
            Given_NoInlinedCall();
            scanner.Setup(s => s.ScanProcedure(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<string>(),
                It.IsAny<ProcessorState>())).Returns(proc);
            scanner.Setup(s => s.EnqueueJumpTarget(
                It.IsAny<Address>(),
                It.IsAny<Address>(),
                It.IsAny<Procedure>(),
                It.IsAny<ProcessorState>()))
                .Returns(l00100100);
            arch.Setup(a => a.PointerType).Returns(PrimitiveType.Ptr32);

            trace.Add(m => m.CallD(Address.Ptr32(0x0100100), 0));
            trace.Add(m => m.Assign(r0, r1));
            trace.Add(m => m.Assign(r1, r2));
            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual(3, block.Statements.Count);
            Assert.AreEqual("r0 = r1", block.Statements[0].ToString());
            Assert.AreEqual("call testProc (retsize: 0;)", block.Statements[1].ToString());
            Assert.AreEqual("r1 = r2", block.Statements[2].ToString());
            scanner.Verify();
        }

        [Test]
        public void Bwi_Return_DelaySlot()
        {
            var l00100008 = new Block(proc, Address.Ptr32(0x00100008), "l00100008");
            var l00100100 = new Block(proc, Address.Ptr32(0x00101000), "l00101000");
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Returns(block);
            Given_SimpleTrace(trace);
            //scanner.Setup(s => s.TerminateBlock(null, null)).IgnoreArguments();
            scanner.Setup(s => s.EnqueueJumpTarget(
                It.IsAny<Address>(),
                It.IsAny<Address>(),
                It.IsAny<Procedure>(),
                It.IsAny<ProcessorState>()))
                .Returns(l00100100);
            scanner.Setup(s => s.SetProcedureReturnAddressBytes(proc, 0, Address.Ptr32(0x100000)));

            trace.Add(m => m.ReturnD(0, 0));
            trace.Add(m => m.Assign(r0, r1));
            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual(2, block.Statements.Count);
            Assert.AreEqual("r0 = r1", block.Statements[0].ToString());
            Assert.AreEqual("return", block.Statements[1].ToString());
        }

        [Test(Description = "Test for when a delay slot is anulled (SPARC)")]
        public void Bwi_Branch_DelaySlotAnulled()
        {
            var l00100008 = new Block(proc, Address.Ptr32(0x00100008), "l00100008");
            var l00100100 = new Block(proc, Address.Ptr32(0x00101000), "l00101000");
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Returns(block);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100008))).Returns(l00100008);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x10000C))).Returns((Block)null);
            Given_SimpleTrace(trace);
            scanner.Setup(s => s.EnqueueJumpTarget(
                Address.Ptr32(0x00100004),
                Address.Ptr32(0x00101000),
                proc,
                It.IsNotNull<ProcessorState>())).Returns(l00100100);
            scanner.Setup(s => s.EnqueueJumpTarget(
                Address.Ptr32(0x00100004),
                Address.Ptr32(0x00100008),
                proc,
                It.IsNotNull<ProcessorState>())).Returns(l00100008);
            trace.Add(m => m.Branch(r1, Address.Ptr32(0x101000), InstrClass.ConditionalTransfer | InstrClass.Delay | InstrClass.Annul));
            trace.Add(m => m.Assign(r0, r1));   // 100004
            trace.Add(m => m.Assign(r2, r1));   // 100008

            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual("branch r1 l00100004_ds_t", block.Statements[0].ToString());
            var blFalse = block.ElseBlock;
            var blTrue = block.ThenBlock;
            Assert.AreEqual("l00100008", blFalse.DisplayName);     // delay-slot was anulled.
            Assert.AreEqual(1, blFalse.Statements.Count);
            Assert.AreEqual("r2 = r1", blFalse.Statements[0].ToString());

            Assert.AreEqual("l00100004_ds_t", blTrue.DisplayName);      // delay-slot-true
            Assert.AreEqual(1, blTrue.Statements.Count);
            Assert.AreEqual("r0 = r1", blTrue.Statements[0].ToString());
            Assert.AreEqual(1, blTrue.Succ.Count);
            Assert.AreEqual("l00101000", blTrue.Succ[0].DisplayName);
        }


        [Test(Description = "Test for infinite loops with delay slots")]
        public void Bwi_Branch_InfiniteLoop_DelaySlot()
        {
            var l00100000 = new Block(proc, Address.Ptr32(0x00100008), "l0010000");
            var l00100004 = new Block(proc, Address.Ptr32(0x00101000), "l00100004");
            Given_NoImportedProcedure();
            Given_SimpleTrace(trace);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Returns(l00100000);
            scanner.Setup(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Returns(l00100000);
            scanner.Setup(s => s.EnqueueJumpTarget(
                Address.Ptr32(0x00100004),
                Address.Ptr32(0x00100004),
                proc,
                It.IsNotNull<ProcessorState>())).Returns(l00100004);
            scanner.Setup(s => s.TerminateBlock(
                l00100000,
                Address.Ptr32(0x0010000C)));
            Given_NoTrampoline();

            trace.Add(m => m.Assign(r0, r1));   // 100000
            trace.Add(m => m.GotoD(Address.Ptr32(0x00100004)));   // 100004
            trace.Add(m => m.Nop());            // 100008 (delay slot)

            var wi = CreateWorkItem(Address.Ptr32(0x00100000));
            wi.Process();

            //mr.Verify();
        }

        [Test(Description = "User-defined procedures with signatures should generate applications immediately")]
        public void Bwi_Call_UserProcedure_With_Signature()
        {
            var addrCall = Address.Ptr32(0x00100000);
            var addrCallee = Address.Ptr32(0x00102000);
            var l00100000 = new Block(proc, addrCall, "l00100000");
            var procCallee = new Procedure(program.Architecture, null, addrCallee, program.Architecture.CreateFrame())
            {
                Name = "testFn",
                Signature = FunctionType.Func(
                    new Identifier("", PrimitiveType.Int32, r0.Storage),
                        new Identifier("str", new Pointer(PrimitiveType.Char, 32), r0.Storage),
                    new Identifier("f", PrimitiveType.Real32, r1.Storage))
            };
            Given_SimpleTrace(trace);
            scanner.Setup(f => f.FindContainingBlock(It.IsAny<Address>())).Returns(l00100000);
            Given_NoImportedProcedure();
            Given_NoInlinedCall();
            scanner.Setup(f => f.ScanProcedure(
                It.IsNotNull<IProcessorArchitecture>(),
                addrCallee,
                It.IsAny<string>(),
                It.IsAny<ProcessorState>())).Returns(procCallee);
            //scanner.Setup(f => f.SetProcedureReturnAddressBytes(null, 0, null)).IgnoreArguments();
            //scanner.Setup(f => f.TerminateBlock(null, null)).IgnoreArguments();

            trace.Add(m => m.Call(addrCallee, 4));
            trace.Add(m => m.Return(4, 0));

            program.User.Procedures.Add(
                addrCallee,
                new UserProcedure(addrCallee, NamingPolicy.Instance.ProcedureName(addrCallee))
                {
                    CSignature = "int testFn(char * str, float f)"
                });

            var wi = CreateWorkItem(addrCall);
            wi.Process();

            Assert.AreEqual("r0 = testFn(r0, r1)", l00100000.Statements[0].ToString());
        }

        [Test(Description = "Create two edges even if they both point to the same destination")]
        public void BwiBranchToSame()
        {
            var addrStart = Address.Ptr32(0x00100000);
            var addrNext = Address.Ptr32(0x00100004);
            var blockOther = new Block(proc, addrNext, "other");

            scanner.Setup(s => s.FindContainingBlock(addrStart)).Returns(block);
            scanner.Setup(s => s.FindContainingBlock(addrNext)).Returns(blockOther);
            Given_SimpleTrace(trace);
            scanner.Setup(s => s.EnqueueJumpTarget(
                addrStart,
                addrNext,
                proc,
                It.IsAny<ProcessorState>())).Returns(blockOther);
            trace.Add(m => m.Branch(m.Mem8(m.Word32(0x12340)), addrNext, InstrClass.ConditionalTransfer));

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual(2, block.Succ.Count);
            Assert.AreSame(blockOther, block.Succ[0]);
            Assert.AreSame(blockOther, block.Succ[1]);
        }

        [Test(Description = "Tests the implementation of #25; user specified register values at a specific address in the program")]
        public void BwiUserSpecifiedRegisterValues()
        {
            var addrStart = Address.Ptr32(0x00100000);
            program.User.RegisterValues[addrStart+4] = new List<UserRegisterValue>
            {
                new UserRegisterValue(r1.Storage, Constant.Word32(0x4711)),
                new UserRegisterValue(r2.Storage, Constant.Word32(0x1147)),
            };
            trace.Add(m => { m.Assign(r1, m.Mem32(m.Ptr32(0x112200))); });
            trace.Add(m => { m.Assign(m.Mem32(m.Ptr32(0x112204)), r1); });
            scanner.Setup(s => s.FindContainingBlock(It.IsAny<Address>())).Returns(block);
            Given_SimpleTrace(trace);
            arch.Setup(s => s.GetRegister("r1")).Returns((RegisterStorage)r1.Storage);
            arch.Setup(s => s.GetRegister("r2")).Returns((RegisterStorage)r2.Storage);
            arch.Setup(s => s.MakeAddressFromConstant(It.IsAny<Constant>(), It.IsAny<bool>()))
                .Returns((Constant c, bool b) => Address.Ptr32(c.ToUInt32()));
            Constant co;
            arch.Setup(s => s.TryRead(
                It.IsAny<ByteMemoryArea>(),
                It.IsAny<Address>(),
                It.IsAny<PrimitiveType>(),
                out co)).Returns(false);

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual("r1 = Mem0[0x00112200<p32>:word32]", block.Statements[0].Instruction.ToString());
            Assert.AreEqual("r1 = 0x4711<32>", block.Statements[1].Instruction.ToString());
            Assert.AreEqual("r2 = 0x1147<32>", block.Statements[2].Instruction.ToString());
            Assert.AreEqual("Mem0[0x00112204<p32>:word32] = r1", block.Statements[3].Instruction.ToString());
        }

        [Test(Description = "If we fall into another procedure (that may not yet have been processed), we should generate an call-ret sequence")]
        public void BwiFallIntoOtherProcedure()
        {
            var addrStart = Address.Ptr32(0x00100000);
            var blockCallRet = new Block(proc, addrStart, "callRetStub");
            trace.Add(m => { m.Assign(m.Mem32(m.Ptr32(0x00123400)), m.Word32(1)); });
            Given_SimpleTrace(trace);
            scanner.Setup(s => s.FindContainingBlock(It.IsAny<Address>())).Returns(block);
            scanner.Setup(s => s.CreateCallRetThunk(
                It.IsAny<Address>(),
                It.IsAny<Procedure>(),
                It.IsAny<ProcessorState>(),
                It.IsAny<Procedure>()))
                .Returns(blockCallRet);
            var frame = program.Architecture.CreateFrame();
            program.Procedures.Add(addrStart + 4, Procedure.Create(program.Architecture, addrStart + 4, frame));

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual("Mem0[0x00123400<p32>:word32] = 1<32>", block.Statements[0].Instruction.ToString());
            scanner.Verify();
        }

        [Test(Description = "Jumping to an address off the address space should warn and emit ExternalProcedure")]
        public void BwiJumpExternalProcedure()
        {
            var addrStart = Address.Ptr32(0x00100000);
            var blockCallRet = new Block(proc, addrStart, "jmpOut");
            trace.Add(m => { m.Goto(Address.Ptr32(0x00123400)); });
            Given_NoImportedProcedure();
            //scanner.Setup(x => x.TerminateBlock(null, null)).IgnoreArguments();
            Given_SimpleTrace(trace);
            scanner.Setup(s => s.FindContainingBlock(It.IsAny<Address>())).Returns(block);
            scanner.Setup(s => s.Warn(
                It.IsAny<Address>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()));
            var frame = program.Architecture.CreateFrame();
            program.Procedures.Add(addrStart + 4, Procedure.Create(program.Architecture, addrStart + 4, frame));

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual("call fn00123400 (retsize: 0;)", block.Statements[0].Instruction.ToString());
            Assert.AreEqual("return", block.Statements[1].Instruction.ToString());
            scanner.Verify();
        }

        [Test(Description = "Read constants from read-only memory")]
        public void BwiReadConstants()
        {
            var addrStart = Address.Ptr32(0x00100000);
            var blockCallRet = new Block(proc, addrStart, "jmpOut");
            trace.Add(m =>
            {
                m.Assign(r1, 4);
                m.Assign(r1, m.Or(r1, 0x00100000));
                m.Assign(r2, m.Mem32(r1));
                m.Call(r2, 0);
                m.Return(0, 0);
            });

            Given_Segment(".text2", 0x00123000, 0x500, AccessMode.ReadExecute);
            scanner.Setup(s => s.FindContainingBlock(addrStart)).Returns(block);
            Given_SimpleTrace(trace);
            arch.Setup(a => a.MakeAddressFromConstant(
                It.Is<Constant>(c => c.ToUInt32() == 0x00100004),
                It.IsAny<bool>())).Returns(Address.Ptr32(0x00100004));
            var addr = Constant.Word32(0x00123400);
            arch.Setup(a => a.TryRead(
                It.IsNotNull<ByteMemoryArea>(),
                Address.Ptr32(0x00100004),
                PrimitiveType.Word32,
                out addr)).Returns(true);
            arch.Setup(a => a.MakeAddressFromConstant(
                It.Is<Constant>(c => c.ToUInt32() == 0x00123400),
                It.IsAny<bool>())).Returns(Address.Ptr32(0x00123400));
            Given_NoInlinedCall();
            scanner.Setup(s => s.SetProcedureReturnAddressBytes(
                proc,
                0,
                addrStart));
            scanner.Setup(s => s.TerminateBlock(
                block,
                addrStart + 4));
            scanner.Setup(f => f.GetImportedProcedure(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<Address>())).Returns((ExternalProcedure) null);
            scanner.Setup(s => s.ScanProcedure(
                It.IsNotNull<IProcessorArchitecture>(),
                Address.Ptr32(0x00123400),
                null,
                It.IsNotNull<ProcessorState>()))
                .Returns(new ExternalProcedure("fn00123400", new FunctionType()))
                .Verifiable();
            program.Procedures.Add(addrStart + 4, Procedure.Create(arch.Object, addrStart + 4, new Frame(arch.Object, PrimitiveType.Ptr32)));

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual("call fn00123400 (retsize: 0;)", block.Statements[3].Instruction.ToString());
            scanner.Verify();
    }

        [Test]
        public void BwiTrashRegisterAfterCall()
        {
            Given_TrashedRegisters(r1);
            var addrStart = Given_Trace(0x00100000, m =>
            {
                m.Assign(r1, 0xBAD);
                m.Call(r2, 4);
                m.Call(r1, 4);
            });

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            var expected =
@"
r1 = 0xBAD<32>
call r2 (retsize: 4;)
call r1 (retsize: 4;)
";
            AssertBlockCode(expected, block);
    }

        [Test]
        public void BwiUsePatch()
        {
            var addrStart = Address.Ptr32(0x00100000);

            trace.Add(m => m.Assign(r0, m.Mem8(m.Word16(0x0042))));
            trace.Add(m => m.Assign(r1, 42));
            Given_SimpleTrace(trace);
            Given_TransferPatch(addrStart.ToUInt32() + 4, 8, m => {
                m.BranchInMiddleOfInstruction(m.Eq(r0, 1), Address.Ptr32(0x00100100), InstrClass.ConditionalTransfer);
                m.BranchInMiddleOfInstruction(m.Eq(r0, 2), Address.Ptr32(0x00100200), InstrClass.ConditionalTransfer);
                m.BranchInMiddleOfInstruction(m.Eq(r0, 3), Address.Ptr32(0x00100300), InstrClass.ConditionalTransfer);
                m.Goto(Address.Ptr32(0x00100400));
            });
            Given_ExpectedBranchTarget(0x00100100, "case1");
            Given_ExpectedBranchTarget(0x00100200, "case2");
            Given_ExpectedBranchTarget(0x00100300, "case3");
            Given_ExpectedBranchTarget(0x00100400, "default");

            scanner.Setup(s => s.FindContainingBlock(addrStart)).Returns(block);
            scanner.Setup(s => s.FindContainingBlock(addrStart + 4)).Returns(block);

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            var expected =
@"testProc_entry:
case1:
case2:
case3:
default:
l00100000:
	r0 = Mem0[0x42<16>:byte]
	branch r0 == 1<32> case1
l00100004_1:
	branch r0 == 2<32> case2
l00100004_2:
	branch r0 == 3<32> case3
l00100004_3:
	goto 0x00100400<p32>
	goto default
testProc_exit:
";
            AssertProcedureCode(expected, block.Procedure);
        }

        [Test]
        public void Bwi_Syscall_ReturnAddressAdjustment()
        {
            var addrStart = Address.Ptr32(0x00100000);

            var platform = new Mock<IPlatform>(MockBehavior.Strict);
            platform.Setup(p => p.FindService(
                5,
                It.IsAny<ProcessorState>(),
                It.IsAny<SegmentMap>())).Returns(new SystemService
                {
                    Name = "Service5",
                    Signature = FunctionType.Action(),
                    Characteristics = new ProcedureCharacteristics ()
                    {
                        ReturnAddressAdjustment = 8,
                    },
                });
            platform.Setup(p => p.PointerType).Returns(PrimitiveType.Ptr32);
            platform.Setup(p => p.TrashedRegisters).Returns(new HashSet<RegisterStorage>());
            program.Platform = platform.Object;
            Given_Trace(0x00100000, m => m.SideEffect(m.Fn(CommonOps.Syscall_1, Constant.Byte(5))));
            Given_Trace(0x00100008, m => m.Return(0, 0));
            Given_ExpectedBranchTarget(0x00100008, "l00100008");

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            var sExpected =
@"testProc_entry:
l00100000:
	Service5()
	goto 0x00100008<p32>
l00100008:
testProc_exit:
";
            AssertProcedureCode(sExpected, block.Procedure);
        }
    }
}

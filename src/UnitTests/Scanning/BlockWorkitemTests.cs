#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ComponentModel.Design;
using Reko.Core.Services;
using System;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class BlockWorkitemTests
    {
        private MockRepository mr;
        private IScanner scanner;
        private IProcessorArchitecture arch;
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
            mr = new MockRepository();
            program = new Program();
            proc = new Procedure("testProc", new Frame(PrimitiveType.Word32));
            block = proc.AddBlock("l00100000");
            trace = new RtlTrace(0x00100000);
            r0 = new Identifier("r0", PrimitiveType.Word32, new RegisterStorage("r0", 0, 0, PrimitiveType.Word32));
            r1 = new Identifier("r1", PrimitiveType.Word32, new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
            r2 = new Identifier("r2", PrimitiveType.Word32, new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));
            sp = new Identifier("sp", PrimitiveType.Word32, new RegisterStorage("sp", 15, 0, PrimitiveType.Word32));
            grf = proc.Frame.EnsureFlagGroup(Registers.eflags, 3, "SCZ", PrimitiveType.Byte);
            var sc = new ServiceContainer();
            var listener = mr.Stub<DecompilerEventListener>();
            scanner = mr.StrictMock<IScanner>();
            arch = mr.Stub<IProcessorArchitecture>();
            program.Architecture = arch;
            program.SegmentMap = new SegmentMap(
                Address.Ptr32(0x00100000),
                new ImageSegment(
                    ".text",
                    new MemoryArea(Address.Ptr32(0x00100000), new byte[0x20000]),
                    AccessMode.ReadExecute));
            arch.Replay();
            program.Platform = new DefaultPlatform(null, arch);
            arch.BackToRecord();
            arch.StackRegister = (RegisterStorage)sp.Storage;
            arch.Stub(s => s.PointerType).Return(PrimitiveType.Ptr32);
            scanner.Stub(s => s.Services).Return(sc);
            sc.AddService<DecompilerEventListener>(listener);
        }

        private BlockWorkitem CreateWorkItem(Address addr)
        {
            return CreateWorkItem(addr, new FakeProcessorState(arch));
        }

        private BlockWorkitem CreateWorkItem(Address addr, ProcessorState state)
        {
            return new BlockWorkitem(
                scanner,
                program,
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
            program.SegmentMap.AddSegment(new ImageSegment(segName, new MemoryArea(Address.Ptr32(addr), new byte[size]), mode));
        }

        private bool StashArg(ref ProcessorState state, ProcessorState value)
        {
            state = value;
            return true;
        }

        [Test]
        public void Bwi_RewriteReturn()
        {
            trace.Add(m => { m.Return(4, 0); });
            trace.Add(m => { m.Fn(m.Int32(0x49242)); });

            using (mr.Record())
            {
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Stub(x => x.TerminateBlock(null, null)).IgnoreArguments();
                scanner.Stub(x => x.SetProcedureReturnAddressBytes(
                    Arg<Procedure>.Is.NotNull,
                    Arg<int>.Is.Equal(4),
                    Arg<Address>.Matches(a => a.ToLinear() == 0x00100000)));
                scanner.Stub(x => x.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            }

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

            Block next = block.Procedure.AddBlock("next");
            using (mr.Record())
            {
                arch.Stub(x => x.PointerType).Return(PrimitiveType.Ptr32);
                arch.Stub(x => x.CreateRewriter(
                    Arg<EndianImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<IStorageBinder>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(trace);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Is.NotNull,
                    Arg<Address>.Is.Anything,
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Is.Anything)).Return(next);
                scanner.Stub(x => x.TerminateBlock(null, null)).IgnoreArguments();
                scanner.Stub(x => x.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
                scanner.Stub(f => f.GetImportedProcedure(null, null)).IgnoreArguments().Return(null);
                scanner.Stub(s => s.GetTrampoline(null)).IgnoreArguments().Return(null);
            }

            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();
            Assert.AreEqual(2, block.Statements.Count);
            Assert.AreEqual("r0 = 0x00000003", block.Statements[0].ToString());
            Assert.AreEqual("goto 0x00104000", block.Statements[1].ToString());
            Assert.AreEqual(1, proc.ControlGraph.Successors(block).Count);
            var items = new List<Block>(proc.ControlGraph.Successors(block));
            Assert.AreSame(next, items[0]);
            mr.VerifyAll();
        }

        [Test]
        public void Bwi_HandleBranch()
        {
            trace.Add(m =>
                m.Branch(r1, Address.Ptr32(0x00104000), RtlClass.ConditionalTransfer));
            trace.Add(m =>
                m.Assign(r1, r2));
            var blockElse = new Block(proc, "else");
            var blockThen = new Block(proc, "then");
            ProcessorState s1 = null;
            ProcessorState s2 = null;
            using (mr.Record())
            {
                arch.Stub(a => a.FramePointerType).Return(PrimitiveType.Ptr32);
                arch.Stub(x => x.CreateRewriter(
                    Arg<EndianImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<IStorageBinder>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(trace);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Is.NotNull,
                    Arg<Address>.Matches(arg => arg.Offset == 0x00100004),
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Matches(arg => StashArg(ref s1, arg)))).Return(blockElse);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Is.NotNull,
                    Arg<Address>.Matches(arg => arg.Offset == 0x00104000),
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Matches(arg => StashArg(ref s2, arg)))).Return(blockThen);
                scanner.Stub(x => x.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            }
            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();
            Assert.AreEqual(1, block.Statements.Count, "Expected a branch statement in the block");
            Assert.AreNotSame(s1, s2);
            Assert.IsNotNull(s1);
            Assert.IsNotNull(s2);

            mr.VerifyAll();
        }

        [Test]
        public void Bwi_CallInstructionShouldAddNodeToCallgraph()
        {
            trace.Add(m => { m.Call(Address.Ptr32(0x102000), 4); });
            trace.Add(m => { m.Assign(m.Word32(0x4000), m.Word32(0)); });
            trace.Add(m => { m.Return(4, 0); });

            using (mr.Record())
            {
                arch.Stub(x => x.CreateRewriter(
                    Arg<EndianImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<IStorageBinder>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(trace);
                arch.Stub(x => x.PointerType).Return(PrimitiveType.Ptr32);
                scanner.Stub(x => x.GetImportedProcedure(null, null)).IgnoreArguments().Return(null);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.ScanProcedure(
                    Arg<Address>.Matches(arg => arg.Offset == 0x102000),
                    Arg<string>.Is.Null,
                    Arg<ProcessorState>.Is.Anything))
                        .Return(new Procedure("fn102000", new Frame(PrimitiveType.Word32)));
                scanner.Stub(x => x.TerminateBlock(null, null)).IgnoreArguments();
                scanner.Stub(x => x.SetProcedureReturnAddressBytes(
                    Arg<Procedure>.Is.NotNull,
                    Arg<int>.Is.Equal(4),
                    Arg<Address>.Is.NotNull));
                scanner.Stub(x => x.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            }
            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();
            var callees = new List<Procedure>(program.CallGraph.Callees(block.Procedure));
            Assert.AreEqual(1, callees.Count);
            Assert.AreEqual("fn102000", callees[0].Name);
        }

        [Test]
        public void Bwi_CallingAllocaWithConstant()
        {
            program.Architecture = new X86ArchitectureFlat32("x86-protected-32");
            program.Platform = new DefaultPlatform(null, program.Architecture);
            var sig = CreateSignature(Registers.esp, Registers.eax);
            var alloca = new ExternalProcedure("alloca", sig)
            {
                Characteristics = new ProcedureCharacteristics
                {
                    IsAlloca = true
                }
            };

            using (mr.Record())
            {
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.GetImportedProcedure(
                    Arg<Address>.Matches(a => a.ToLinear() == 0x102000u),
                    Arg<Address>.Is.NotNull)).Return(alloca);
                scanner.Stub(x => x.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            }
            trace.Add(m => m.Call(Address.Ptr32(0x102000), 4));
            var state = new FakeProcessorState(program.Architecture);
            state.SetRegister(Registers.eax, Constant.Word32(0x0400));
            var wi = CreateWorkItem(Address.Ptr32(0x1000), state);
            wi.Process();

            mr.VerifyAll();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("esp = esp - 0x00000400", block.Statements.Last.ToString());
        }

        [Test]
        public void Bwi_CallingAllocaWithNonConstant()
        {
            arch = mr.Stub<IProcessorArchitecture>();
            arch = new X86ArchitectureFlat32("x86-protected-32");
            program.Platform = new DefaultPlatform(null, arch);

            var sig = CreateSignature(Registers.esp, Registers.eax);
            var alloca = new ExternalProcedure("alloca", sig, new ProcedureCharacteristics
            {
                IsAlloca = true
            });

            trace.Add(m => m.Call(Address.Ptr32(0x102000), 4));

            using (mr.Record())
            {
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.GetImportedProcedure(
                    Arg<Address>.Is.Equal(Address.Ptr32(0x102000u)),
                    Arg<Address>.Is.NotNull)).Return(alloca);
                scanner.Stub(x => x.GetTrace(null, null, null)).IgnoreArguments().Return(trace);

            }
            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();
            mr.VerifyAll();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("esp = alloca(eax)", block.Statements.Last.ToString());
        }

        [Test]
        public void Bwi_CallTerminatingProcedure_StopScanning()
        {
            proc = Procedure.Create("proc", Address.Ptr32(0x102000), new Frame(PrimitiveType.Ptr32));
            var terminator = Procedure.Create("terminator", Address.Ptr32(0x0001000), new Frame(PrimitiveType.Ptr32));
            terminator.Characteristics = new ProcedureCharacteristics {
                Terminates = true,
            };
            block = proc.AddBlock("the_block");
            arch.Stub(a => a.PointerType).Return(PrimitiveType.Word32);
            scanner.Stub(s => s.FindContainingBlock(Arg<Address>.Is.Anything)).Return(block);
            scanner.Stub(s => s.GetImportedProcedure(Arg<Address>.Is.Anything, Arg<Address>.Is.NotNull)).Return(null);
            scanner.Expect(s => s.ScanProcedure(
                Arg<Address>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ProcessorState>.Is.Anything))
                .Return(terminator);
            scanner.Expect(s => s.TerminateBlock(Arg<Block>.Is.NotNull, Arg<Address>.Is.NotNull));
            arch.Stub(a => a.FramePointerType).Return(PrimitiveType.Ptr32);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            mr.ReplayAll();

            trace.Add(m => m.Call(Address.Ptr32(0x00102000), 4));
            trace.Add(m => m.SideEffect(new ProcedureConstant(VoidType.Instance, new PseudoProcedure("shouldnt_decompile_this", VoidType.Instance, 0))));

            var wi = CreateWorkItem(Address.Ptr32(0x2000));
            wi.Process();

            Assert.AreEqual(1, block.Statements.Count, "Should only have rewritten the Call to 'terminator'");
            mr.VerifyAll();
        }

        [Test]
        public void Bwi_CallProcedureWithSignature()
        {
            var proc2 = new Procedure("fn2000", new Frame(PrimitiveType.Ptr32));
            var sig = new FunctionType(
                proc2.Frame.EnsureRegister(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32)),
                new[] {
                    proc2.Frame.EnsureRegister(new RegisterStorage("r2", 2, 0, PrimitiveType.Word32)),
                    proc2.Frame.EnsureRegister(new RegisterStorage("r3", 3, 0, PrimitiveType.Word32))
                });
            proc2.Signature = sig;
            var block2 = new Block(proc, "l00100008");
            var block3 = new Block(proc, "l00100004");
            arch.Stub(a => a.PointerType).Return(PrimitiveType.Ptr32);
            scanner.Expect(s => s.FindContainingBlock(Address.Ptr32(0x00001000))).IgnoreArguments().Return(block).Repeat.Times(2);
            scanner.Expect(s => s.FindContainingBlock(Address.Ptr32(0x00001004))).IgnoreArguments().Return(block2); // .Repeat.Times(2);
            scanner.Expect(s => s.GetImportedProcedure(null, null)).IgnoreArguments().Return(null);
            scanner.Expect(s => s.EnqueueJumpTarget(
                Arg<Address>.Is.NotNull,
                Arg<Address>.Matches(a => a.ToLinear() == 0x00100004),
                Arg<Procedure>.Is.NotNull,
                Arg<ProcessorState>.Is.NotNull))
                .Return(block3);
            scanner.Expect(s => s.ScanProcedure(
                Arg<Address>.Is.Equal(Address.Ptr32(0x102000)),
                Arg<string>.Is.Null,
                Arg<ProcessorState>.Is.NotNull)).Return(proc2);
            scanner.Expect(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            mr.ReplayAll();

            trace.Add(m => m.Call(Address.Ptr32(0x102000), 0));
            trace.Add(m => m.Return(0, 0));
            var wi = CreateWorkItem(Address.Ptr32(0x1000));
            wi.Process();

            mr.VerifyAll();
            var sw = new StringWriter();
            proc.Write(false, sw);
            var sExp = @"// testProc
// Return size: 0
void testProc()
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
            var platform = mr.StrictMock<IPlatform>();
            var reg0 = proc.Frame.EnsureRegister(new RegisterStorage("r0", 0, 0, PrimitiveType.Ptr32));
            var reg1 = proc.Frame.EnsureRegister(new RegisterStorage("r1", 1, 0, PrimitiveType.Ptr32));
            var sysSvc = new SystemService {
                Name = "SysSvc",
                Signature = FunctionType.Action(new[] { reg1 }),
                Characteristics = new ProcedureCharacteristics()
            };
            platform.Expect(p => p.FindService(null, null)).IgnoreArguments().Return(sysSvc);
            platform.Stub(p => p.PointerType).Return(PrimitiveType.Ptr32);
            platform.Stub(p => p.ResolveIndirectCall(null)).IgnoreArguments().Return(null);
            program.Platform = platform;
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Return(block);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            mr.ReplayAll();

            trace.Add(m => m.Call(m.Mem32(m.IAdd(reg0, -32)), 4));
            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual("SysSvc(r1)", block.Statements[0].ToString());
            mr.VerifyAll();
        }

        [Test]
        public void Bwi_IndirectJump()
        {
            var platform = mr.StrictMock<IPlatform>();
            var sp = proc.Frame.EnsureRegister(new RegisterStorage("sp", 14, 0, PrimitiveType.Ptr32));
            platform.Expect(p => p.FindService(null, null)).IgnoreArguments().Return(null);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Return(block);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.TerminateBlock(null, null)).IgnoreArguments();
            mr.ReplayAll();

            trace.Add(m => m.Goto(m.Mem32(sp)));
            var wi = CreateWorkItem(Address.Ptr32(0x0100000));
            wi.Process();

            Assert.AreEqual("call Mem0[sp:word32] (retsize: 4;)", block.Statements[0].ToString());
            Assert.AreEqual("return", block.Statements[1].ToString());
        }

        [Test]
        public void Bwi_Goto_DelaySlot()
        {
            var l00100008 = new Block(proc, "l00100008");
            var l00100100 = new Block(proc, "l00101000");
            scanner.Stub(f => f.GetImportedProcedure(null, null)).IgnoreArguments().Return(null);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100008))).Return(l00100008);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.TerminateBlock(null, null)).IgnoreArguments();
            scanner.Stub(s => s.EnqueueJumpTarget(null, null, null, null))
                .IgnoreArguments()
                .Return(l00100100);
            scanner.Stub(s => s.GetTrampoline(null)).IgnoreArguments().Return(null);
            mr.ReplayAll();

            trace.Add(m => m.GotoD(Address.Ptr32(0x0100100)));
            trace.Add(m => m.Assign(r0, r1));
            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual(2, block.Statements.Count);
            Assert.AreEqual("r0 = r1", block.Statements[0].ToString());
            Assert.AreEqual("goto 0x00100100", block.Statements[1].ToString());

            Assert.AreEqual("l00101000", block.Succ[0].Name);
            mr.VerifyAll();
        }

        [Test]
        public void Bwi_Branch_DelaySlot()
        {
            var l00100008 = new Block(proc, "l00100008");
            var l00100100 = new Block(proc, "l00101000");
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100008))).Return(l00100008);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x10000C))).Return(null);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.EnqueueJumpTarget(
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100004)),
                Arg<Address>.Is.Equal(Address.Ptr32(0x00101000)),
                Arg<Procedure>.Is.Equal(proc),
                Arg<ProcessorState>.Is.NotNull)).Return(l00100100);
            scanner.Stub(s => s.EnqueueJumpTarget(
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100004)),
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100008)),
                Arg<Procedure>.Is.Equal(proc),
                Arg<ProcessorState>.Is.NotNull)).Return(l00100008);
            mr.ReplayAll();

            trace.Add(m => m.Branch(r1, Address.Ptr32(0x101000), RtlClass.ConditionalTransfer | RtlClass.Delay));
            trace.Add(m => m.Assign(r0, r1));   // 100004
            trace.Add(m => m.Assign(r2, r1));   // 100008

            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual("branch r1 l00100000_ds_t", block.Statements[0].ToString());
            var blFalse = block.ElseBlock;
            var blTrue = block.ThenBlock;
            Assert.AreEqual("l00100000_ds_f", blFalse.Name);     // delay-slot-false
            Assert.AreEqual(1, blFalse.Statements.Count);
            Assert.AreEqual("r0 = r1", blFalse.Statements[0].ToString());
            Assert.AreEqual(1, blFalse.Succ.Count);
            Assert.AreEqual("l00100008", blFalse.Succ[0].Name);

            Assert.AreEqual("l00100000_ds_t", blTrue.Name);      // delay-slot-true
            Assert.AreEqual(1, blTrue.Statements.Count);
            Assert.AreEqual("r0 = r1", blTrue.Statements[0].ToString());
            Assert.AreEqual(1, blTrue.Succ.Count);
            Assert.AreEqual("l00101000", blTrue.Succ[0].Name);
        }

        [Test]
        public void Bwi_Call_DelaySlot()
        {
            var l00100008 = new Block(proc, "l00100008");
            var l00100100 = new Block(proc, "l00101000");
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100008))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x10000C))).Return(null);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.TerminateBlock(null, null)).IgnoreArguments();
            scanner.Stub(s => s.GetImportedProcedure(null, null)).IgnoreArguments().Return(null);
            scanner.Stub(s => s.ScanProcedure(null, null, null)).IgnoreArguments().Return(proc);
            scanner.Stub(s => s.EnqueueJumpTarget(null, null, null, null))
                .IgnoreArguments()
                .Return(l00100100);
            arch.Stub(a => a.PointerType).Return(PrimitiveType.Ptr32);
            mr.ReplayAll();

            trace.Add(m => m.CallD(Address.Ptr32(0x0100100), 0));
            trace.Add(m => m.Assign(r0, r1));
            trace.Add(m => m.Assign(r1, r2));
            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual(3, block.Statements.Count);
            Assert.AreEqual("r0 = r1", block.Statements[0].ToString());
            Assert.AreEqual("call testProc (retsize: 0;)", block.Statements[1].ToString());
            Assert.AreEqual("r1 = r2", block.Statements[2].ToString());

            mr.VerifyAll();
        }

        [Test]
        public void Bwi_Return_DelaySlot()
        {
            var l00100008 = new Block(proc, "l00100008");
            var l00100100 = new Block(proc, "l00101000");
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Return(block);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.TerminateBlock(null, null)).IgnoreArguments();
            scanner.Stub(s => s.EnqueueJumpTarget(null, null, null, null))
                .IgnoreArguments()
                .Return(l00100100);
            scanner.Stub(s => s.SetProcedureReturnAddressBytes(proc, 0, Address.Ptr32(0x100000)));
            mr.ReplayAll();

            trace.Add(m => m.ReturnD(0, 0));
            trace.Add(m => m.Assign(r0, r1));
            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual(2, block.Statements.Count);
            Assert.AreEqual("r0 = r1", block.Statements[0].ToString());
            Assert.AreEqual("return", block.Statements[1].ToString());
            mr.VerifyAll();
        }

        [Test(Description = "Test for when a delay slot is anulled (SPARC)")]
        public void Bwi_Branch_DelaySlotAnulled()
        {
            var l00100008 = new Block(proc, "l00100008");
            var l00100100 = new Block(proc, "l00101000");
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Return(block);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100008))).Return(l00100008);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x10000C))).Return(null);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.EnqueueJumpTarget(
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100004)),
                Arg<Address>.Is.Equal(Address.Ptr32(0x00101000)),
                Arg<Procedure>.Is.Equal(proc),
                Arg<ProcessorState>.Is.NotNull)).Return(l00100100);
            scanner.Stub(s => s.EnqueueJumpTarget(
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100004)),
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100008)),
                Arg<Procedure>.Is.Equal(proc),
                Arg<ProcessorState>.Is.NotNull)).Return(l00100008);
            mr.ReplayAll();

            trace.Add(m => m.Branch(r1, Address.Ptr32(0x101000), RtlClass.ConditionalTransfer | RtlClass.Delay | RtlClass.Annul));
            trace.Add(m => m.Assign(r0, r1));   // 100004
            trace.Add(m => m.Assign(r2, r1));   // 100008

            var wi = CreateWorkItem(Address.Ptr32(0x100000));
            wi.Process();

            Assert.AreEqual("branch r1 l00100000_ds_t", block.Statements[0].ToString());
            var blFalse = block.ElseBlock;
            var blTrue = block.ThenBlock;
            Assert.AreEqual("l00100008", blFalse.Name);     // delay-slot was anulled.
            Assert.AreEqual(1, blFalse.Statements.Count);
            Assert.AreEqual("r2 = r1", blFalse.Statements[0].ToString());

            Assert.AreEqual("l00100000_ds_t", blTrue.Name);      // delay-slot-true
            Assert.AreEqual(1, blTrue.Statements.Count);
            Assert.AreEqual("r0 = r1", blTrue.Statements[0].ToString());
            Assert.AreEqual(1, blTrue.Succ.Count);
            Assert.AreEqual("l00101000", blTrue.Succ[0].Name);
        }


        [Test(Description = "Test for infinite loops with delay slots")]
        public void Bwi_Branch_InfiniteLoop_DelaySlot()
        {
            var l00100000 = new Block(proc, "l0010000");
            var l00100004 = new Block(proc, "l00100004");
            scanner.Stub(f => f.GetImportedProcedure(null, null)).IgnoreArguments().Return(null);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100000))).Return(l00100000);
            scanner.Stub(f => f.FindContainingBlock(Address.Ptr32(0x100004))).Return(l00100000);
            scanner.Stub(s => s.EnqueueJumpTarget(
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100004)),
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100004)),
                Arg<Procedure>.Is.Equal(proc),
                Arg<ProcessorState>.Is.NotNull)).Return(l00100004);
            scanner.Stub(s => s.TerminateBlock(
                Arg<Block>.Is.Equal(l00100000),
                Arg<Address>.Is.Equal(Address.Ptr32(0x0010000C))));
            scanner.Stub(s => s.GetTrampoline(null)).IgnoreArguments().Return(null);
            mr.ReplayAll();

            trace.Add(m => m.Assign(r0, r1));   // 100000
            trace.Add(m => m.GotoD(Address.Ptr32(0x00100004)));   // 100004
            trace.Add(m => m.Nop());            // 100008 (delay slot)

            var wi = CreateWorkItem(Address.Ptr32(0x00100000));
            wi.Process();

            mr.VerifyAll();
        }

        [Test(Description = "User-defined procedures with signatures should generate applications immediately")]
        public void Bwi_Call_UserProcedure_With_Signature()
        {
            var addrCall = Address.Ptr32(0x00100000);
            var addrCallee = Address.Ptr32(0x00102000);
            var l00100000 = new Block(proc, "l00100000");
            var procCallee = new Procedure(null, new Frame(PrimitiveType.Ptr32))
            {
                Name = "testFn",
                Signature = new FunctionType(
                    new Identifier("", PrimitiveType.Int32, r0.Storage),
                    new[] {
                        new Identifier("str", new Pointer(PrimitiveType.Char, 4), r0.Storage),
                        new Identifier("f", PrimitiveType.Real32, r1.Storage)
                    })
            };
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(f => f.FindContainingBlock(null)).IgnoreArguments().Return(l00100000);
            scanner.Stub(f => f.GetImportedProcedure(addrCallee, addrCall)).Return(null);
            scanner.Stub(f => f.ScanProcedure(
                Arg<Address>.Is.Equal(addrCallee),
                Arg<string>.Is.Anything,
                Arg<ProcessorState>.Is.Anything)).Return(procCallee);
            scanner.Stub(f => f.SetProcedureReturnAddressBytes(null, 0, null)).IgnoreArguments();
            scanner.Stub(f => f.TerminateBlock(null, null)).IgnoreArguments();
            mr.ReplayAll();

            trace.Add(m => m.Call(addrCallee, 4));
            trace.Add(m => m.Return(4, 0));

            program.User.Procedures.Add(
                addrCallee,
                new Procedure_v1
                {
                    CSignature = "int testFn(char * str, float f)"
                });

            var wi = CreateWorkItem(addrCall);
            wi.Process();
            Assert.AreEqual("r0 = testFn(r0, r1)", l00100000.Statements[0].ToString());

            mr.VerifyAll();
        }

        [Test(Description = "Create two edges even if they both point to the same destination")]
        public void BwiBranchToSame()
        {
            var addrStart = Address.Ptr32(0x00100000);
            var addrNext = Address.Ptr32(0x00100004);
            var blockOther = new Block(proc, "other");

            scanner.Stub(s => s.FindContainingBlock(addrStart)).Return(block);
            scanner.Stub(s => s.FindContainingBlock(addrNext)).Return(blockOther);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.EnqueueJumpTarget(
                Arg<Address>.Is.Equal(addrStart),
                Arg<Address>.Is.Equal(addrNext),
                Arg<Procedure>.Is.Same(proc),
                Arg<ProcessorState>.Is.Anything)).Return(blockOther);
            mr.ReplayAll();

            trace.Add(m => m.Branch(m.Mem8(m.Word32(0x12340)), addrNext, RtlClass.ConditionalTransfer));

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual(2, block.Succ.Count);
            Assert.AreSame(blockOther, block.Succ[0]);
            Assert.AreSame(blockOther, block.Succ[1]);
            mr.VerifyAll();
        }

        [Test(Description = "Tests the implementation of #25; user specified register values at a specific address in the program")]
        public void BwiUserSpecifiedRegisterValues()
        {
            var addrStart = Address.Ptr32(0x00100000);
            program.User.RegisterValues[addrStart+4] = new List<UserRegisterValue>
            {
                new UserRegisterValue { Register = (RegisterStorage)r1.Storage, Value= Constant.Word32(0x4711) },
                new UserRegisterValue { Register = (RegisterStorage)r2.Storage, Value= Constant.Word32(0x1147) },
            };
            trace.Add(m => { m.Assign(r1, m.Mem32(m.Word32(0x112200))); });
            trace.Add(m => { m.Assign(m.Mem32(m.Word32(0x112204)), r1); });
            scanner.Stub(s => s.FindContainingBlock(null)).IgnoreArguments().Return(block);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            arch.Stub(s => s.GetRegister("r1")).Return((RegisterStorage)r1.Storage);
            arch.Stub(s => s.GetRegister("r2")).Return((RegisterStorage)r2.Storage);
            arch.Stub(s => s.MakeAddressFromConstant(null)).IgnoreArguments()
                .Do(new Func<Constant, Address>(c => Address.Ptr32(c.ToUInt32())));
            Constant co;
            arch.Stub(s => s.TryRead(null, null, null, out co)).IgnoreArguments().Return(false);
            mr.ReplayAll();

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual("r1 = Mem0[0x00112200:word32]", block.Statements[0].Instruction.ToString());
            Assert.AreEqual("r1 = 0x00004711", block.Statements[1].Instruction.ToString());
            Assert.AreEqual("r2 = 0x00001147", block.Statements[2].Instruction.ToString());
            Assert.AreEqual("Mem0[0x00112204:word32] = r1", block.Statements[3].Instruction.ToString());
        }

        [Test(Description = "If we fall into another procedure (that may not yet have been processed), we should generate an call-ret sequence")]
        public void BwiFallIntoOtherProcedure()
        {
            var addrStart = Address.Ptr32(0x00100000);
            var blockCallRet = new Block(proc, "callRetStub");
            trace.Add(m => { m.Assign(m.Mem32(m.Word32(0x00123400)), m.Word32(1)); });
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.FindContainingBlock(addrStart)).IgnoreArguments().Return(block);
            scanner.Expect(s => s.CreateCallRetThunk(null, null, null)).IgnoreArguments().Return(blockCallRet);
            program.Procedures.Add(addrStart + 4, Procedure.Create(addrStart + 4, new Frame(PrimitiveType.Ptr32)));
            mr.ReplayAll();

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual("Mem0[0x00123400:word32] = 0x00000001", block.Statements[0].Instruction.ToString());
            mr.VerifyAll();
        }

        [Test(Description = "Jumping to an address off the address space should warn and emit ExternalProcedure")]
        public void BwiJumpExternalProcedure()
        {
            var addrStart = Address.Ptr32(0x00100000);
            var blockCallRet = new Block(proc, "jmpOut");
            trace.Add(m => { m.Goto(Address.Ptr32(0x00123400)); });
            scanner.Stub(f => f.GetImportedProcedure(null, null)).IgnoreArguments().Return(null);
            scanner.Stub(x => x.TerminateBlock(null, null)).IgnoreArguments();
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            scanner.Stub(s => s.FindContainingBlock(addrStart)).IgnoreArguments().Return(block);
            scanner.Expect(s => s.Warn(null, null, null)).IgnoreArguments();
            program.Procedures.Add(addrStart + 4, Procedure.Create(addrStart + 4, new Frame(PrimitiveType.Ptr32)));
            mr.ReplayAll();

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual("call fn00123400 (retsize: 4;)", block.Statements[0].Instruction.ToString());
            Assert.AreEqual("return", block.Statements[1].Instruction.ToString());
            mr.VerifyAll();
        }

        [Test(Description = "Read constants from read-only memory")]
        public void BwiReadConstants()
        {
            var addrStart = Address.Ptr32(0x00100000);
            var blockCallRet = new Block(proc, "jmpOut");
            trace.Add(m => {
                m.Assign(r1, 4); 
                m.Assign(r1, m.Or(r1, 0x00100000)); 
                m.Assign(r2, m.Mem32(r1));
                m.Call(r2, 0); 
                m.Return(0, 0);
            });

            Given_Segment(".text2", 0x00123000, 0x500, AccessMode.ReadExecute);
            scanner.Stub(s => s.FindContainingBlock(addrStart)).IgnoreArguments().Return(block);
            scanner.Stub(s => s.GetTrace(null, null, null)).IgnoreArguments().Return(trace);
            arch.Stub(a => a.MakeAddressFromConstant(
                Arg<Constant>.Matches(c => c.ToUInt32() == 0x00100004))).Return(Address.Ptr32(0x00100004));
            arch.Stub(a => a.TryRead(
                Arg<MemoryArea>.Is.NotNull,
                Arg<Address>.Is.Equal(Address.Ptr32(0x00100004)),
                Arg<PrimitiveType>.Is.Equal(PrimitiveType.Word32),
                out Arg<Constant>.Out(Constant.Word32(0x00123400)).Dummy)).Return(true);
            arch.Stub(a => a.MakeAddressFromConstant(
                Arg<Constant>.Matches(c => c.ToUInt32() == 0x00123400))).Return(Address.Ptr32(0x00123400));
            scanner.Stub(s => s.SetProcedureReturnAddressBytes(
                Arg<Procedure>.Is.Same(proc),
                Arg<int>.Is.Equal(0),
                Arg<Address>.Is.Equal(addrStart)));
            scanner.Stub(s => s.TerminateBlock(
                Arg<Block>.Is.Same(block),
                Arg<Address>.Is.Equal(addrStart + 4)));
            scanner.Stub(f => f.GetImportedProcedure(null, null)).IgnoreArguments().Return(null);
            scanner.Expect(s => s.ScanProcedure(
                Arg<Address>.Is.Equal(Address.Ptr32(0x00123400)),
                Arg<string>.Is.Null,
                Arg<ProcessorState>.Is.NotNull)).
                Return(new ExternalProcedure("fn00123400", new FunctionType()));
            program.Procedures.Add(addrStart + 4, Procedure.Create(addrStart + 4, new Frame(PrimitiveType.Ptr32)));
            mr.ReplayAll();

            var wi = CreateWorkItem(addrStart);
            wi.Process();

            Assert.AreEqual("call fn00123400 (retsize: 0;)", block.Statements[3].Instruction.ToString());
            mr.VerifyAll();
        }
    }
}

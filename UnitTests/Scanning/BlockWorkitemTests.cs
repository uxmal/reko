#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class BlockWorkitemTests
    {
        private MockRepository repository;
        private IScanner scanner;
        private IProcessorArchitecture arch;
        private Program prog;
        private Procedure proc;
        private Block block;
        private RtlTrace trace;
        private Identifier r0;
        private Identifier r1;
        private Identifier r2;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            prog = new Program();
            proc = new Procedure("testProc", new Frame(null));
            block = new Block(proc, "test");
            trace = new RtlTrace(0x1000);
            r0 = new Identifier("r0", 0, PrimitiveType.Word32, new RegisterStorage("r0", 0, PrimitiveType.Word32));
            r1 = new Identifier("r1", 0, PrimitiveType.Word32, new RegisterStorage("r1", 0, PrimitiveType.Word32));
            r2 = new Identifier("r2", 0, PrimitiveType.Word32, new RegisterStorage("r2", 0, PrimitiveType.Word32));

            scanner = repository.DynamicMock<IScanner>();
            arch = repository.DynamicMock<IProcessorArchitecture>();
        }

        private BlockWorkitem CreateWorkItem(Address addr, ProcessorState state)
        {
            return new BlockWorkitem(
                scanner, 
                trace,
                state,
                proc.Frame, 
                addr);
        }

        private ProcedureSignature CreateSignature(RegisterStorage  ret, params RegisterStorage[] args)
        {
            var retReg = proc.Frame.EnsureRegister(ret);
            var argIds = new List<Identifier>();
            foreach (var arg in args)
            {
                argIds.Add(proc.Frame.EnsureRegister(arg));
            }
            return new ProcedureSignature(retReg, argIds.ToArray());
        }

        [Test]
        public void RewriteReturn()
        {
            trace.Add(m => { m.Return(4, 0); });
            trace.Add(m => { m.Fn(m.Int32(0x49242)); });

            using (repository.Record())
            {
                scanner.Stub(x => x.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
            }

            var wi = CreateWorkItem(new Address(0x1000), new FakeProcessorState(arch));
            wi.Process();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.IsTrue(proc.ControlGraph.ContainsEdge(block, proc.ExitBlock), "Expected return to add an edge to the Exit block");
        }


        [Test]
        public void StopOnGoto()
        {
            trace.Add(m =>
            {
                m.Assign(r0, m.Word32(3));
                m.Goto(new Address(0x4000));
            });

            Block next = new Block(block.Procedure, "next");
            using (repository.Record())
            {
                arch.Stub(x => x.CreateRewriter(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(trace);
                scanner.Stub(s => s.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Is.Anything,
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Is.Anything)).Return(next);
            }

            var wi = CreateWorkItem(new Address(0x1000), new FakeProcessorState(arch));
            wi.Process();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("r0 = 0x00000003", block.Statements[0].ToString());
            Assert.AreEqual(1, proc.ControlGraph.Successors(block).Count);
            var items = new List<Block>(proc.ControlGraph.Successors(block));
            Assert.AreSame(next, items[0]);
            repository.VerifyAll();
        }

        private bool StashArg(ref ProcessorState state, ProcessorState value)
        {
            state = value;
            return true;
        }

        [Test]
        public void HandleBranch()
        {
            trace.Add(m => { m.Branch(r1, new Address(0x4000)); });
            trace.Add(m => { m.Assign(r1, r2); });
            var blockElse = new Block(proc, "else");
            var blockThen = new Block(proc, "then");
            ProcessorState s1 = null;
            ProcessorState s2 = null;
            using (repository.Record())
            {
                arch.Stub(a => a.FramePointerType).Return(PrimitiveType.Pointer32);
                arch.Stub(x => x.CreateRewriter(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(trace);
                scanner.Stub(x => x.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(arg => arg.Offset == 0x1004),
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Matches(arg => StashArg(ref s1, arg)))).Return(blockElse); 
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(arg => arg.Offset == 0x4000),
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Matches(arg => StashArg(ref s2, arg)))).Return(blockThen);
            }
            var wi = CreateWorkItem(new Address(0x1000), new FakeProcessorState(arch));
            wi.Process();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreNotSame(s1, s2);
            Assert.IsNotNull(s1);
            Assert.IsNotNull(s2);

            repository.VerifyAll();
        }

        [Test]
        public void CallInstructionShouldAddNodeToCallgraph()
        {
            trace.Add(m => { m.Call(new Address(0x1200), 4); });
            trace.Add(m => { m.Assign(m.Word32(0x4000), m.Word32(0)); });
            trace.Add(m => { m.Return(4, 0); });

            var cg = new CallGraph();
            using (repository.Record())
            {
                arch.Stub(x => x.CreateRewriter(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(trace);
                scanner.Stub(x => x.CallGraph).Return(cg);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.ScanProcedure(
                    Arg<Address>.Matches(arg => arg.Offset == 0x1200),
                    Arg<string>.Is.Null,
                    Arg<ProcessorState>.Is.Anything))
                        .Return(new Procedure("fn1200", new Frame(null)));
                scanner.Stub(x=> x.Architecture).Return(arch);
            }
            var wi = CreateWorkItem(new Address(0x1000), new FakeProcessorState(arch));
            wi.Process();
            var callees = new List<Procedure>(cg.Callees(block.Procedure));
            Assert.AreEqual(1, callees.Count);
            Assert.AreEqual("fn1200", callees[0].Name);
        }

        [Test]
        public void CallingAllocaWithConstant()
        {
            arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            var sig = CreateSignature(Registers.esp, Registers.eax);
            var alloca = new PseudoProcedure("alloca", sig);
            alloca.Characteristics = new ProcedureCharacteristics
            {
                IsAlloca = true
            };

            trace.Add(m => { m.Call(new Address(0x2000), 4); });
            using (repository.Record())
            {
                scanner.Stub(x => x.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.GetImportedProcedure(
                    Arg<uint>.Is.Equal(0x2000u))).Return(alloca);
                    
            }
            var state = new FakeProcessorState(arch);
            state.SetRegister(Registers.eax, Constant.Word32(0x0400));
            var wi = CreateWorkItem(new Address(0x1000), state);
            wi.Process();
            repository.VerifyAll();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("esp = esp - 0x00000400", block.Statements.Last.ToString());
        }

        [Test]
        public void CallingAllocaWithNonConstant()
        {
            arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            var sig = CreateSignature(Registers.esp, Registers.eax);
            var alloca = new PseudoProcedure("alloca", sig);
            alloca.Characteristics = new ProcedureCharacteristics
            {
                IsAlloca = true
            };

            trace.Add(m => { m.Call(new Address(0x2000), 4); });

            using (repository.Record())
            {
                scanner.Stub(x => x.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.GetImportedProcedure(
                    Arg<uint>.Is.Equal(0x2000u))).Return(alloca);
                    
            }
            var wi = CreateWorkItem(new Address(0x1000), new FakeProcessorState(arch));
            wi.Process();
            repository.VerifyAll();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("esp = alloca(eax)", block.Statements.Last.ToString());
        }

        [Test]
        public void CallTerminatingProcedure_StopScanning()
        {
            var proc = Procedure.Create("proc", new Address(0x002000), new Frame(PrimitiveType.Pointer32));
            var terminator = Procedure.Create("terminator", new Address(0x0001000), new Frame(PrimitiveType.Pointer32));
            terminator.Characteristics = new ProcedureCharacteristics {
                Terminates = true,
             };
            var block = new Block(proc, "the_block");
            var callGraph = new CallGraph();
            scanner = repository.StrictMock<IScanner>();
            scanner.Stub(s => s.Architecture).Return(arch);
            scanner.Stub(s => s.FindContainingBlock(Arg<Address>.Is.Anything)).Return(block);
            scanner.Stub(s => s.GetCallSignatureAtAddress(Arg<Address>.Is.Anything)).Return(null);
            scanner.Stub(s => s.GetImportedProcedure(Arg<uint>.Is.Anything)).Return(null);
            scanner.Stub(s => s.CallGraph).Return(callGraph);
            scanner.Expect(s => s.ScanProcedure(
                Arg<Address>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<ProcessorState>.Is.Anything))
                .Return(terminator);
            arch.Stub(a => a.FramePointerType).Return(PrimitiveType.Pointer32);
            repository.ReplayAll();

            trace.Add(m => { m.Call(new Address(0x0001000), 4); });
            trace.Add(m => { m.SideEffect(new ProcedureConstant(PrimitiveType.Void, new PseudoProcedure("shouldnt_decompile_this", PrimitiveType.Void, 0))); });

            var wi = CreateWorkItem(new Address(0x2000), new FakeProcessorState(arch));
            wi.Process();

            Assert.AreEqual(1, block.Statements.Count, "Should only have rewritten the Call to 'terminator'");
            repository.VerifyAll();
        }
    }
}

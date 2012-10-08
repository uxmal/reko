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
        private Rewriter rewriter;
        private Program prog;
        private Procedure proc;
        private Block block;
        private RtlStatementStream m;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            prog = new Program();
            proc = new Procedure("testProc", new Frame(null));
            block = new Block(proc, "test");
            m = new RtlStatementStream(0x1000, block);

            scanner = repository.DynamicMock<IScanner>();
            arch = repository.DynamicMock<IProcessorArchitecture>();
            rewriter = repository.Stub<Rewriter>();
        }

        private BlockWorkitem CreateWorkItem(Address addr)
        {
            return new BlockWorkitem(
                scanner, 
                rewriter, 
                new ScannerEvaluationContext(
                    arch, 
                    new FakeProcessorState(arch)),
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
            m.Return();
            m.Fn(m.Int32(0x49242));

            using (repository.Record())
            {
                scanner.Stub(x => x.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                rewriter.Stub(x => x.GetEnumerator()).Return(m.GetRewrittenInstructions());
            }

            var wi = CreateWorkItem(new Address(0x1000));
            wi.Process();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.IsTrue(proc.ControlGraph.ContainsEdge(block, proc.ExitBlock), "Expected return to add an edge to the Exit block");
        }


        [Test]
        public void StopOnGoto()
        {
            m.Assign(m.Register(0), 3);
            m.Goto(0x4000);

            Block next = new Block(block.Procedure, "next");
            using (repository.Record())
            {
                arch.Stub(x => x.CreateRewriter(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(rewriter);
                rewriter.Stub(x => x.GetEnumerator()).Return(m.GetRewrittenInstructions());
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Is.Anything,
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ScannerEvaluationContext>.Is.Anything)).Return(next);
            }

            var wi = CreateWorkItem(new Address(0x1000));
            wi.Process();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("r0 = 0x00000003", block.Statements[0].ToString());
            Assert.AreEqual(1, proc.ControlGraph.Successors(block).Count);
            var items = new List<Block>(proc.ControlGraph.Successors(block));
            Assert.AreSame(next, items[0]);
            repository.VerifyAll();
        }

        private bool StashArg(ref ScannerEvaluationContext state, ScannerEvaluationContext value)
        {
            state = value;
            return true;
        }

        [Test]
        public void HandleBranch()
        {
            m.Branch(m.Register(1), new Address(0x4000));
            m.Assign(m.Register(1), m.Register(2));
            var blockElse = new Block(proc, "else");
            var blockThen = new Block(proc, "then");
            ScannerEvaluationContext s1 = null;
            ScannerEvaluationContext s2 = null;
            using (repository.Record())
            {
                arch.Stub(x => x.CreateRewriter(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(rewriter);
                rewriter.Stub(x => x.GetEnumerator()).Return(m.GetRewrittenInstructions());
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(arg => arg.Offset == 0x1004),
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ScannerEvaluationContext>.Matches(arg => StashArg(ref s1, arg)))).Return(blockElse); 
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(arg => arg.Offset == 0x4000),
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ScannerEvaluationContext>.Matches(arg => StashArg(ref s2, arg)))).Return(blockThen);
            }
            var wi = CreateWorkItem(new Address(0x1000));
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
            m.Call(new Address(0x1200));
            m.Assign(m.Word32(0x4000), m.Word32(0));
            m.Return();

            var cg = new CallGraph();
            using (repository.Record())
            {
                arch.Stub(x => x.CreateRewriter(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost>.Is.Anything)).Return(rewriter);
                StubRewriterStream();
                scanner.Stub(x => x.CallGraph).Return(cg);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.ScanProcedure(
                    Arg<Address>.Matches(arg => arg.Offset == 0x1200),
                    Arg<string>.Is.Null,
                    Arg<ScannerEvaluationContext>.Is.Anything))
                        .Return(new Procedure("fn1200", new Frame(null)));
                scanner.Stub(x=> x.Architecture).Return(arch);
            }
            var wi = CreateWorkItem(new Address(0x1000));
            wi.Process();
            var callees = new List<Procedure>(cg.Callees(block.Procedure));
            Assert.AreEqual(1, callees.Count);
            Assert.AreEqual("fn1200", callees[0].Name);
        }

        private void StubRewriterStream()
        {
            rewriter.Stub(x => x.GetEnumerator()).Return(m.GetRewrittenInstructions());
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

            m.Call(new Address(0x2000));
            using (repository.Record())
            {
                StubRewriterStream();
                scanner.Stub(x => x.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.GetImportedProcedure(
                    Arg<uint>.Is.Equal(0x2000u))).Return(alloca);
                    
            }
            var wi = CreateWorkItem(new Address(0x1000));
            wi.Context.State.SetRegister(Registers.eax, Constant.Word32(0x0400));
            wi.Process();
            repository.VerifyAll();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("esp = esp - 0x00000400", wi.Block.Statements.Last.ToString());
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

            m.Call(new Address(0x2000));
            using (repository.Record())
            {
                StubRewriterStream();
                scanner.Stub(x => x.Architecture).Return(arch);
                scanner.Stub(x => x.FindContainingBlock(
                    Arg<Address>.Is.Anything)).Return(block);
                scanner.Expect(x => x.GetImportedProcedure(
                    Arg<uint>.Is.Equal(0x2000u))).Return(alloca);
                    
            }
            var wi = CreateWorkItem(new Address(0x1000));
            wi.Process();
            repository.VerifyAll();
            Assert.AreEqual(1, block.Statements.Count);
            Assert.AreEqual("esp = alloca(eax)", wi.Block.Statements.Last.ToString());
        }
    }
}

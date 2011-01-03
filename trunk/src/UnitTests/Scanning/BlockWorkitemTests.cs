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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Rtl;
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
        private Rewriter2 rewriter;
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
            rewriter = repository.Stub<Rewriter2>();
        }

        private BlockWorkitem CreateWorkItem(Address addr)
        {
            return new BlockWorkitem(scanner, rewriter, new FakeProcessorState(), proc.Frame, addr);
        }

        [Test]
        public void RewriteReturn()
        {
            m.Return();
            m.Fn(m.Int32(0x49242));

            using (repository.Record())
            {
                scanner.Stub(x => x.Architecture).Return(arch);
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
                arch.Stub(x => x.CreateRewriter2(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost2>.Is.Anything)).Return(rewriter);
                rewriter.Stub(x => x.GetEnumerator()).Return(m.GetRewrittenInstructions());
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Is.Anything,
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Is.Anything)).Return(next);
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

        private bool StashArg(ref ProcessorState state, ProcessorState value)
        {
            state = value;
            return true;
        }

        [Test]
        public void HandleBranch()
        {
            m.Branch(m.Register(1), new Address(0x4000));
            m.Assign(m.Register(1), m.Register(2));

            ProcessorState s1 = null;
            ProcessorState s2 = null;
            using (repository.Record())
            {
                arch.Stub(x => x.CreateRewriter2(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost2>.Is.Anything)).Return(rewriter);
                rewriter.Stub(x => x.GetEnumerator()).Return(m.GetRewrittenInstructions());
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(arg => arg.Offset == 0x1004),
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Matches(arg => StashArg(ref s1, arg)))).Return(null); 
                scanner.Expect(x => x.EnqueueJumpTarget(
                    Arg<Address>.Matches(arg => arg.Offset == 0x4000),
                    Arg<Procedure>.Is.Same(block.Procedure),
                    Arg<ProcessorState>.Matches(arg => StashArg(ref s2, arg)))).Return(null);
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
                arch.Stub(x => x.CreateRewriter2(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost2>.Is.Anything)).Return(rewriter);
                rewriter.Stub(x => x.GetEnumerator()).Return(m.GetRewrittenInstructions());
                scanner.Stub(x => x.CallGraph).Return(cg);
                scanner.Expect(x => x.EnqueueProcedure(
                    Arg<WorkItem>.Is.Anything,
                    Arg<Address>.Matches(arg => arg.Offset == 0x1200),
                    Arg<string>.Is.Null,
                    Arg<ProcessorState>.Is.Anything))
                        .Return(new Procedure("fn1200", new Frame(null)));
            }
            var wi = CreateWorkItem(new Address(0x1000));
            wi.Process();
            var callees = new List<Procedure>(cg.Callees(block.Procedure));
            Assert.AreEqual(1, callees.Count);
            Assert.AreEqual("fn1200", callees[0].Name);
        }

        [Test]
        public void RewriteSideEffect()
        {
            m.SideEffect(m.Register(1));
            Assert.Fail();
        }

        [Test]
        [Ignore("Need split to work in scanner first")]
        public void SplitBlock()
        {
            m.Assign(m.Register(0), 0);
            m.Add(m.Register(0), 1);
            m.Branch(m.Lt(m.Register(0), 10), new Address(0x1004));
            m.Return();

            using (repository.Record())
            {
                arch.Stub(x => x.CreateRewriter2(
                    Arg<ImageReader>.Is.Anything,
                    Arg<ProcessorState>.Is.Anything,
                    Arg<Frame>.Is.Anything,
                    Arg<IRewriterHost2>.Is.Anything)).Return(rewriter);
                rewriter.Stub(x => x.GetEnumerator()).Return(m.GetRewrittenInstructions());
            }
            var wi = CreateWorkItem(new Address(0x1000));
            wi.Process();
        }

    }

}

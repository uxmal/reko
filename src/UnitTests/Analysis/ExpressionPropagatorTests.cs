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
using Reko.Analysis;
using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class ExpressionPropagatorTests
    {
        private FakeDecompilerEventListener listener;

        [SetUp]
        public void Setup()
        {
            this.listener = new FakeDecompilerEventListener();
        }
        [Test]
        public void EP_TestCondition()
        {
            var p = new ProgramBuilder();
            p.Add("main", (m) =>
            {
                m.Label("foo");
                m.BranchCc(ConditionCode.EQ, "foo");
                m.Return();
            });

            var proc = p.BuildProgram().Procedures.Values.First();
            var arch = new X86ArchitectureFlat32();
            var ctx = new SymbolicEvaluationContext(arch, proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx, listener);
            var ep = new ExpressionPropagator(null, simplifier, ctx, new ProgramDataFlow());

            var newInstr = proc.EntryBlock.Succ[0].Statements[0].Instruction.Accept(ep);
            Assert.AreEqual("branch Test(EQ,Z) foo", newInstr.ToString());
        }

        [Test]
        public void EP_ConditionOf()
        {
            var p = new ProgramBuilder();
            var proc = p.Add("main", (m) =>
            {
                var szo = m.Frame.EnsureFlagGroup(Registers.eflags, 0x7, "SZO", PrimitiveType.Byte);
                var ebx = m.Frame.EnsureRegister(new RegisterStorage("ebx", 3, 0, PrimitiveType.Word32));
                var v4 = m.Frame.CreateTemporary(PrimitiveType.Word16);

                m.Assign(v4, m.IAdd(m.LoadW(ebx), 1));
                m.Store(ebx, v4);
                m.Assign(szo, m.Cond(v4));
                m.Return();
            });

            var arch = new X86ArchitectureFlat32();
            var platform = new FakePlatform(null, arch);
            var ctx = new SymbolicEvaluationContext(arch, proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx, listener);
            var ep = new ExpressionPropagator(platform, simplifier, ctx, new ProgramDataFlow());

            var newInstr = proc.EntryBlock.Succ[0].Statements[2].Instruction.Accept(ep);
            Assert.AreEqual("SZO = cond(v4)", newInstr.ToString());
        }

        [Test]
        public void EP_Application()
        {
            var p = new ProgramBuilder();
            var proc = p.Add("main", (m) =>
            {
                var r1 = m.Frame.EnsureRegister(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));

                m.Assign(r1, m.Word32(0x42));
                m.SideEffect(m.Fn("foo", r1));
                m.Return();
            });

            var arch = new FakeArchitecture();
            var ctx = new SymbolicEvaluationContext(arch, proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx, listener);
            var ep = new ExpressionPropagator(null, simplifier, ctx, new ProgramDataFlow());

            var stms = proc.EntryBlock.Succ[0].Statements;
            stms[0].Instruction.Accept(ep);
            var newInstr = stms[1].Instruction.Accept(ep);
            Assert.AreEqual("foo(0x00000042)", newInstr.ToString());
        }

        [Test]
        public void EP_IndirectCall()
        {
            var arch = new FakeArchitecture();
            var p = new ProgramBuilder(arch);
            var proc = p.Add("main", (m) =>
            {
                var r1 = m.Register("r1");

                m.Assign(r1, m.Word32(0x42));
                m.Call(r1, 4);
                m.Return();
            });

            var platform = new FakePlatform(null, arch)
            {
                Test_CreateTrashedRegisters = () => new HashSet<RegisterStorage>()
            };
            var ctx = new SymbolicEvaluationContext(arch, proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx, listener);
            var ep = new ExpressionPropagator(platform, simplifier, ctx, new ProgramDataFlow());

            ctx.RegisterState[arch.StackRegister] = proc.Frame.FramePointer;
            var stms = proc.EntryBlock.Succ[0].Statements;
            stms[0].Instruction.Accept(ep);
            var newInstr = stms[1].Instruction.Accept(ep);
            Assert.AreEqual("call 0x00000042 (retsize: 4; depth: 4)", newInstr.ToString());
        }

        [Test]
        public void EP_StackReference()
        {
            var arch = new FakeArchitecture();
            var platform = new FakePlatform(null, arch);
            var p = new ProgramBuilder(arch);
            var proc = p.Add("main", (m) =>
            {
                var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var r1 = m.Register(1);
                m.Assign(sp, m.ISub(sp, 4));
                m.Assign(r1, m.LoadDw(m.IAdd(sp, 8)));
                m.Return();
            });

            var ctx = new SymbolicEvaluationContext(arch, proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx, listener);
            var ep = new ExpressionPropagator(platform, simplifier, ctx, new ProgramDataFlow());

            ctx.RegisterState[arch.StackRegister] = proc.Frame.FramePointer;

            var stms = proc.EntryBlock.Succ[0].Statements;
            var newInstr = stms[0].Instruction.Accept(ep);
            Assert.AreEqual("r63 = fp - 0x00000004", newInstr.ToString());
            newInstr = stms[1].Instruction.Accept(ep);
            Assert.AreEqual("r1 = dwArg04", newInstr.ToString());
        }

        [Test]
        public void EP_AddrOf()
        {
            var arch = new FakeArchitecture();
            var platform = new FakePlatform(null, arch);
            var p = new ProgramBuilder(arch);
            Identifier r2 = null, r3 = null;
            var proc = p.Add("main", (m) =>
            {
                r2 = m.Register("r2");
                r3 = m.Register("r3");
                m.Assign(r2, 0x1234);                       // after which R2 has a definite value
                m.SideEffect(m.Fn("Foo", m.Out(PrimitiveType.Pointer32, r2)));    // Can't promise R2 is preserved after call, so should be invalid.
                m.Assign(r3, r2);
            });

            var ctx = new SymbolicEvaluationContext(arch, proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx, listener);
            var ep = new ExpressionPropagator(platform, simplifier, ctx, new ProgramDataFlow());

            ctx.RegisterState[arch.StackRegister] = proc.Frame.FramePointer;

            var stms = proc.EntryBlock.Succ[0].Statements;
            stms[0].Instruction.Accept(ep);
            Assert.AreEqual("0x00001234", ctx.GetValue(r2).ToString());
            var instr2 = stms[1].Instruction.Accept(ep);
            Assert.AreEqual("Foo(out r2)", instr2.ToString());
            Assert.AreEqual("<invalid>", ctx.GetValue(r2).ToString());
            var instr3 = stms[2].Instruction.Accept(ep);
            Assert.AreEqual("r3 = r2", instr3.ToString());
            Assert.AreEqual("<invalid>", ctx.GetValue(r2).ToString());
            Assert.AreEqual("<invalid>", ctx.GetValue(r3).ToString());
        }

        [Test]
        public void EP_LValue()
        {
            var arch = new FakeArchitecture();
            var platform = new FakePlatform(null, arch);
            var p = new ProgramBuilder(arch);
            Identifier r2 = null;
            Identifier sp = null;
            var proc = p.Add("main", (m) =>
            {
                r2 = m.Register("r2");
                sp = m.Frame.EnsureRegister(arch.StackRegister);
                m.Store(m.ISub(sp, 12), m.ISub(sp, 16));
                m.Store(m.ISub(sp, 12), m.Word32(2));
            });

            var ctx = new SymbolicEvaluationContext (arch, proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx, listener);
            var ep = new ExpressionPropagator(platform, simplifier,ctx, new ProgramDataFlow());

            ctx.RegisterState[arch.StackRegister]= proc.Frame.FramePointer;

            var stms = proc.EntryBlock.Succ[0].Statements;
            var instr1 = stms[0].Instruction.Accept(ep);
            Assert.AreEqual("dwLoc0C = fp - 0x00000010", instr1.ToString());
            var instr2 = stms[1].Instruction.Accept(ep);
            Assert.AreEqual("dwLoc0C = 0x00000002", instr2.ToString());
        }
    }
}

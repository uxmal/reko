#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    [TestFixture]
    public class TrashedRegisterSummarizerTests
    {
        private FakeArchitecture arch;
        private Procedure proc;
        private ProcedureFlow flow;
        private SymbolicEvaluationContext ctx;
        private TrashedRegisterSummarizer trs;

        [SetUp]
        public void Setup()
        {
            arch = new FakeArchitecture();
            proc = new Procedure("Test", new Frame(arch.FramePointerType));
            flow = new ProcedureFlow(proc, arch);
            ctx = new SymbolicEvaluationContext(arch, proc.Frame);
            trs = new TrashedRegisterSummarizer(arch, proc, flow, ctx);
        }

        [Test]
        public void Simple()
        {
            var reg = arch.GetRegister(1);
            ctx.RegisterState[reg] = Constant.Word32(1);
            trs.PropagateToProcedureSummary();

            Assert.AreEqual(" r1", flow.EmitRegisters(arch, "", flow.TrashedRegisters));
            Assert.AreEqual("0x00000001", flow.ConstantRegisters[reg].ToString());
        }

        [Test]
        public void Constants()
        {
            var reg = arch.GetRegister(1);
            ctx.RegisterState[reg] = Constant.Word32(1);
            trs.PropagateToProcedureSummary();
            ctx.RegisterState[reg] = Constant.Word32(2);
            trs.PropagateToProcedureSummary();

            Assert.AreEqual("<invalid>", flow.ConstantRegisters[reg].ToString());
        }

        [Test]
        public void Trs_s()
        {
            var r4 = arch.GetRegister(4);
            var id = proc.Frame.EnsureRegister(r4);
            ctx.RegisterState[r4] = new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, id, Constant.Word32(1));
            trs.PropagateToProcedureSummary();
            ctx.RegisterState[r4] = Constant.Word32(3);
            trs.PropagateToProcedureSummary();

            Assert.AreEqual("<invalid>", flow.ConstantRegisters[r4].ToString());
        }

        [Test]
        public void Trs_t()
        {
            var r4 = arch.GetRegister(4);
            var id = proc.Frame.EnsureRegister(r4);
            ctx.RegisterState[r4] = Constant.Word32(3);
            trs.PropagateToProcedureSummary();
            ctx.RegisterState[r4] = new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, id, Constant.Word32(1));
            trs.PropagateToProcedureSummary();

            Assert.AreEqual("<invalid>", flow.ConstantRegisters[r4].ToString());
        }

        [Test]
        public void EqualConstants()
        {
            var r4 = arch.GetRegister(4);
            var id = proc.Frame.EnsureRegister(r4);
            ctx.RegisterState[r4] = Constant.Word32(3);
            trs.PropagateToProcedureSummary();
            ctx.RegisterState[r4] = Constant.Word32(3);
            trs.PropagateToProcedureSummary();

            Assert.AreEqual("0x00000003", flow.ConstantRegisters[r4].ToString());
        }
    }
}

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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Analysis
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
            proc = new Procedure(arch, "Test", new Frame(arch.FramePointerType));
            flow = new ProcedureFlow(proc);
            ctx = new SymbolicEvaluationContext(arch, proc.Frame);
            trs = new TrashedRegisterSummarizer(proc, flow, ctx);
        }

        [Test]
        public void Simple()
        {
            var reg = arch.GetRegister(1);
            ctx.RegisterState[reg] = Constant.Word32(1);
            trs.PropagateToProcedureSummary();

            Assert.AreEqual(" r1", flow.EmitRegisters(arch, "", flow.Trashed));
            Assert.AreEqual("0x00000001", flow.Constants[reg].ToString());
        }

        [Test]
        public void Constants()
        {
            var reg = arch.GetRegister(1);
            ctx.RegisterState[reg] = Constant.Word32(1);
            trs.PropagateToProcedureSummary();
            ctx.RegisterState[reg] = Constant.Word32(2);
            trs.PropagateToProcedureSummary();

            Assert.AreEqual("<invalid>", flow.Constants[reg].ToString());
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

            Assert.AreEqual("<invalid>", flow.Constants[r4].ToString());
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

            Assert.AreEqual("<invalid>", flow.Constants[r4].ToString());
        }

        [Test]
        public void EqualConstants()
        {
            var r4 = arch.GetRegister(4);
            proc.Frame.EnsureRegister(r4);
            ctx.RegisterState[r4] = Constant.Word32(3);
            trs.PropagateToProcedureSummary();
            ctx.RegisterState[r4] = Constant.Word32(3);
            trs.PropagateToProcedureSummary();

            Assert.AreEqual("0x00000003", flow.Constants[r4].ToString());
        }

        [Test]
        [Ignore("This may go away with SSA2 implemented.")]
        public void TrsMergeSubregisterWithRegister()
        {
            var cl = Reko.Arch.X86.Registers.cl;
            var cx = Reko.Arch.X86.Registers.cx;
            ctx.RegisterState[cl] = Constant.Zero(cl.DataType);
            trs.PropagateToProcedureSummary();
            ctx.RegisterState[cx] = Constant.Invalid;
            trs.PropagateToProcedureSummary();

            Assert.AreEqual("cl:<invalid>, cx:<invalid>", Dump(flow.Constants));
        }

        private string Dump(Dictionary<Storage, Constant> dictionary)
        {
            return string.Join(
                ", ",
                dictionary
                    .OrderBy(d => d.Key.ToString())
                    .Select(d => string.Format("{0}:{1}", d.Key, d.Value)));
        }
    }
}

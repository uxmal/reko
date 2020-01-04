#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Evaluation
{
    [TestFixture]
    public class MkSeqFromSlices_RuleTest
    {
        private SsaEvaluationContext ctx;
        private SsaState ssa;

        private void BuildTest(Action<ProcedureBuilder> bld)
        {
            var m = new ProcedureBuilder();
            bld(m);
            var proc = m.Procedure;
            var flow = new ProgramDataFlow();
            var program = new Program()
            {
                Architecture = m.Architecture,
            };
            var sst = new SsaTransform(
                program,
                proc,
                new HashSet<Procedure>(),
                null,
                flow);
            this.ssa = sst.Transform();
            this.ctx = new SsaEvaluationContext(m.Architecture, ssa.Identifiers,  null);
        }

        [Test]
        public void MkSeqFs_RemoveRedundantSlices()
        {
            BuildTest(m =>
            {
                var dw = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var t1 = m.Temp(PrimitiveType.Word16, "t1");
                var t2 = m.Temp(PrimitiveType.Word16, "t2");

                m.Assign(t1 , m.Slice((PrimitiveType)t1.DataType, dw, 16));
                m.Assign(t2, m.Cast(t2.DataType, dw));
                m.Assign(r2, m.Seq(t1, t2));
                m.Return();

            });

            var rule = new MkSeqFromSlices_Rule(ctx);
            ctx.Statement = ssa.Procedure.EntryBlock.Succ[0].Statements[2];
            var ass = (Assignment)ctx.Statement.Instruction;
            var mkseq = (MkSequence)ass.Src;
            Assert.IsTrue(rule.Match(mkseq));
            ass.Src = rule.Transform();
            Assert.AreEqual("r1", ass.Src.ToString());
        }
    }
}

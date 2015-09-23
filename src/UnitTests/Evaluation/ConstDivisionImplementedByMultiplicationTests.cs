using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Evaluation
{
    [TestFixture]
    public class ConstDivisionImplementedByMultiplicationTests
    {
        private ProcedureBuilder m;

        [SetUp]
        public void SetUp()
        {
            m = new ProcedureBuilder();
        }

        [Test]
        [Ignore("Not appropriate -- needs to be implemented as an SSA transform")]
        public void Cdiv_3()
        {
            /*
            ﻿  eax = ~0x33333332
    edx_eax = esi *u eax
    edx = edx >>u 0x03
*/
            var c = Constant.UInt32(0x55555555);
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");
            var r2_r1 = m.Frame.EnsureSequence(r2, r1, PrimitiveType.Word64);

            var ass = m.Assign(r2_r1, m.UMul(r1, c));

            var proc = m.Procedure;
            var ssa = new SsaTransform(null, proc, proc.CreateBlockDominatorGraph()).Transform();
            var ctx = new SsaEvaluationContext(ssa.Identifiers);
            var rule = new ConstDivisionImplementedByMultiplication(ctx);

            //ctx.Statement = proc.EntryBlock.Succ[0].Statements[0];
            //Assert.IsTrue(rule.Match((BinaryExpression) ass.Src));
            //ass.Src = rule.Transform();
            Assert.AreEqual("x = id /u 3", ass.ToString());
        }
    }
}

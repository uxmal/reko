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

using Reko.Analysis;
using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions; 
using Reko.UnitTests.Mocks;
using System;
using NUnit.Framework;

namespace Reko.UnitTests.Evaluation
{

    [TestFixture]
    public class Add_mul_id_c_id_RuleTest
    {
        private ProcedureBuilder m;
        private Identifier id;
        private Identifier x;
        private SsaIdentifierCollection ssaIds;
        private SsaEvaluationContext ctx;

        /// <summary>
        /// (+ (* id c) id) => (* id (+ c 1))
        /// </summary>
        [Test]
        public void Test1()
        {
            BinaryExpression b = m.IAdd(m.SMul(id, 4), id);
            Assignment ass = new Assignment(x, b);
            Statement stm = new Statement(0, ass, null);
            ssaIds[id].Uses.Add(stm);
            ssaIds[id].Uses.Add(stm);

            ctx.Statement = stm;
            Add_mul_id_c_id_Rule rule = new Add_mul_id_c_id_Rule(ctx);
            Assert.IsTrue(rule.Match(b));
            Assert.AreEqual(2, ssaIds[id].Uses.Count);
            ass.Src = rule.Transform();
            Assert.AreEqual("x = id *s 0x00000005", ass.ToString());
            Assert.AreEqual(1, ssaIds[id].Uses.Count);
        }

        /// <summary>
        /// (+ (* c id) id) => (* id (+ c 1))
        /// </summary>
        [Test]
        public void Test2()
        {
            BinaryExpression b = m.IAdd(id, m.UMul(id, 5));
            Assignment ass = new Assignment(x, b);
            var rule = new Add_mul_id_c_id_Rule(new SsaEvaluationContext(null, ssaIds, null));
            Assert.IsTrue(rule.Match(b));
            ass.Src = rule.Transform();
            Assert.AreEqual("x = id *u 0x00000006", ass.ToString());
        }

        [SetUp]
        public void SetUp()
        {
            m = new ProcedureBuilder();
            id = m.Local32("id");
            x = m.Local32("x");
            ssaIds = new SsaIdentifierCollection();
            foreach (Identifier i in m.Frame.Identifiers)
            {
                ssaIds.Add(i, null, null, false);
            }
            ctx = new SsaEvaluationContext(null, ssaIds, null);
        }
    }
}
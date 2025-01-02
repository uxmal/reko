#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Moq;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Reko.Evaluation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Decompiler.Evaluation
{
    [TestFixture]
    public class SubstitutorTests
    {
        private ProcedureBuilder m;
        private Mock<EvaluationContext> ctx;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
            ctx = new Mock<EvaluationContext>();
        }

        [TearDown]
        public void TearDown()
        {
            ctx.VerifyAll();
        }

        [Test]
        public void Const()
        {
            var subst = new Substitutor(ctx.Object);
            var c = m.Int32(4);
            var c2 = c.Accept(subst);
            Assert.AreSame(c, c2);
        }

        [Test]
        public void Identifier_GetValue()
        {
            var id = m.Register("r3");
            var invalid = InvalidConstant.Create(id.DataType);
            ctx.Setup(c => c.GetValue(id)).Returns(invalid);

            var subst = new Substitutor(ctx.Object);
            var cInvalid = id.Accept(subst);
            Assert.AreSame(invalid, cInvalid);
        }

        [Test]
        public void Bin_LeftInvalid()
        {
            var id = m.Register("r3");
            var e = m.IAdd(id, m.Word32(12));
            var invalid = InvalidConstant.Create(id.DataType);
            ctx.Setup(c => c.GetValue(id)).Returns(invalid);

            var subst = new Substitutor(ctx.Object);
            var cInvalid = e.Accept(subst);

            Assert.AreSame(invalid, cInvalid);
        }

        [Test]
        public void Bin_BothValid()
        {
            var id = m.Register("r3");
            var e = m.IAdd(id, id);
            ctx.Setup(c => c.GetValue(id)).Returns(id);

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.AreEqual("r3 + r3", e2.ToString());
        }

        [Test]
        public void Unary_Valid()
        {
            var id = m.Register("r3");
            var e = m.Not(id);
            ctx.Setup(c => c.GetValue(id)).Returns(id);

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.AreEqual("!r3", e2.ToString());
        }

        [Test]
        public void Unary_Invalid()
        {
            var id = m.Register("r3");
            var e = m.Not(id);
            var invalid = InvalidConstant.Create(id.DataType);
            ctx.Setup(c => c.GetValue(id)).Returns(invalid);

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.AreSame(invalid, e2);
        }

        [Test]
        public void Mem_Valid()
        {
            var id = m.Register("r3");
            var e = m.Mem16(id);
            ctx.Setup(c => c.GetValue(id)).Returns(id);

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.AreEqual("Mem0[r3:word16]", e2.ToString());
        }

        [Test]
        public void Mem_Invalid()
        {
            var id = m.Register("r3");
            var e = m.Mem16(id);
            var invalid = InvalidConstant.Create(id.DataType);
            ctx.Setup(c => c.GetValue(id)).Returns(invalid);

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.IsTrue(e2 is InvalidConstant);
        }

        [Test]
        public void SegMem_Valid()
        {
            var es = m.Frame.CreateTemporary("es", PrimitiveType.Word16);
            var bx = m.Frame.CreateTemporary("bx", PrimitiveType.Word16);
            var e = m.SegMem16(es, bx);
            ctx.Setup(c => c.GetValue(es)).Returns(es);
            ctx.Setup(c => c.GetValue(bx)).Returns(bx);

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.AreEqual("Mem0[es:bx:word16]", e2.ToString());
        }

        [Test]
        public void SegMem_Invalid()
        {
            var es = m.Frame.CreateTemporary("es", PrimitiveType.Word16);
            var bx = m.Frame.CreateTemporary("bx", PrimitiveType.Word16);
            var e = m.SegMem16(es, bx);
            var invalid = InvalidConstant.Create(bx.DataType);
            ctx.Setup(c => c.GetValue(es)).Returns(es);
            ctx.Setup(c => c.GetValue(bx)).Returns(invalid);

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.AreSame(invalid, e2);
        }

        [Test]
        public void ConditionOf_Valid()
        {
            var id = m.Register("r3");
            var e = m.Cond(id);
            ctx.Setup(c => c.GetValue(id)).Returns(id);

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.AreEqual("cond(r3)", e2.ToString());
        }

        [Test]
        public void ConditionOf_Invalid()
        {
            var id = m.Register("r3");
            var e = m.Cond(id);
            ctx.Setup(c => c.GetValue(id)).Returns(InvalidConstant.Create(id.DataType));

            var subst = new Substitutor(ctx.Object);
            var e2 = e.Accept(subst);

            Assert.IsTrue(e2 is InvalidConstant);
        }
    }
}

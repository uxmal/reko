#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Analysis;
using Reko.Evaluation;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Decompiler.Evaluation
{
	[TestFixture]
	public class IdConstantTests
	{
		private ProcedureBuilder m;
        private SsaIdentifierCollection ssa;
        private FakeDecompilerEventListener listener;

        [SetUp]
		public void Setup()
		{
			m = new ProcedureBuilder();
            ssa = new SsaIdentifierCollection();
            listener = new FakeDecompilerEventListener();
		}

        private void AssertTypedExpression(string sExp, Expression e)
        {
            Assert.AreEqual(sExp, $"{e}<{e.DataType}>");
        }

        [Test]
		public void Idc_ConstantPropagate()
		{
			Identifier ds = m.Frame.EnsureRegister(Registers.ds);
            var c = Constant.Word16(0x1234);
            m.Assign(ds, c);
			m.SideEffect(ds);
            var def = m.Block.Statements[0];
            var use = m.Block.Statements[1];
			SsaIdentifier sid_ds = ssa.Add(ds, def, false);
            var ass = (Assignment)def.Instruction;
            ass.Dst = sid_ds.Identifier;
            ((SideEffect)use.Instruction).Expression = sid_ds.Identifier;
			sid_ds.Uses.Add(use);

			IdConstant ic = new IdConstant();
            var e = ic.Match(sid_ds.Identifier, new SsaEvaluationContext(null, ssa, null), new Unifier(null, null), listener);
            Assert.That(e, Is.Not.Null);
			Assert.AreEqual("selector", e.DataType.ToString());
		}

        [Test]
        public void Idc_ConstantReferenceInt()
        {
            var dword = new TypeReference("DWORD", PrimitiveType.Int32);
            Identifier edx = new Identifier("edx", dword, Registers.edx);

            var ctx = new Mock<EvaluationContext>();
            ctx.Setup(c => c.GetValue(edx)).Returns(Constant.Int32(321));

            IdConstant ic = new IdConstant();
            
            Expression e = ic.Match(edx, ctx.Object, new Unifier(null, null), listener);
            Assert.That(e, Is.Not.Null);
            Assert.AreEqual("321<i32>", e.ToString());
            Assert.AreEqual("DWORD", e.DataType.ToString());
        }

        [Test]
        public void Idc_ConstantReferencePointerToInt()
        {
            var intptr = new TypeReference("INTPTR", new Pointer(PrimitiveType.Int32, 32));
            Identifier edx = new Identifier("edx", intptr, Registers.edx);

            var ctx = new Mock<EvaluationContext>();
            ctx.Setup(c => c.GetValue(edx)).Returns(Constant.Int32(0x567));

            IdConstant ic = new IdConstant(); 
            var e = ic.Match(edx, ctx.Object, new Unifier(null, null), listener);
            Assert.That(e, Is.Not.Null);
            Assert.AreEqual("00000567", e.ToString());
            Assert.AreEqual("(ptr32 int32)", e.DataType.ToString());
        }

        [Test]
        public void Idc_ConstantAddress()
        {
            var intptr = new TypeReference("INTPTR", new Pointer(PrimitiveType.Int32, 32));
            Identifier edx = new Identifier("edx", intptr, Registers.edx);

            var ctx = new Mock<EvaluationContext>();
            ctx.Setup(c => c.GetValue(edx)).Returns(Address.Ptr32(0x00123400));

            IdConstant ic = new IdConstant();
            var e = ic.Match(edx, ctx.Object, new Unifier(null, null), listener);
            Assert.That(e, Is.Not.Null);
            Assert.AreEqual("00123400", e.ToString());
            Assert.AreEqual("(ptr32 int32)", e.DataType.ToString());
        }

        [Test]
        public void Idc_FloatingPointConstants()
        {
            Identifier edx = new Identifier("edx", PrimitiveType.Real32, Registers.edx);

            var ctx = new Mock<EvaluationContext>();
            ctx.Setup(c => c.GetValue(edx)).Returns(Constant.Word32(0x3E400000));

            IdConstant ic = new IdConstant();
            var e = ic.Match(edx, ctx.Object, new Unifier(null, null), listener);
            Assert.That(e, Is.Not.Null);
            AssertTypedExpression("0.1875F<real32>", e);
        }

        [Test]
        public void Idc_TypeReferenceToReal64()
        {
            var r64 = new Identifier(
                "r64",
                new TypeReference("DOUBLE", PrimitiveType.Real64),
                Registers.rdx);
            var ctx = new Mock<EvaluationContext>();
            var c = Constant.Word64(0x4028000000000000); // 12.0
            ctx.Setup(c => c.GetValue(r64)).Returns(c);

            IdConstant ic = new IdConstant();
            var e = ic.Match(r64, ctx.Object, new Unifier(null, null), listener);
            Assert.That(e, Is.Not.Null);
            AssertTypedExpression("12.0<DOUBLE>", e);
        }
    }
}

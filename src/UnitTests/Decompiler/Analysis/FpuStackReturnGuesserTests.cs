#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class FpuStackReturnGuesserTests
    {
        private SsaProcedureBuilder m;
        private Identifier ST;
        private RegisterStorage Top;

        [SetUp]
        public void Setup()
        {
            m = new SsaProcedureBuilder();
            var stStg = new MemoryStorage("FakeST", StorageDomain.Register + 400);
            ST = new Identifier(stStg.Name, stStg.DataType, stStg);
            ((FakeArchitecture) m.Architecture).FpuStackBase = ST;
            Top = m.Architecture.FpuStackRegister;
        }

        private Expression MemST(Expression ea)
        {
            return m.Mem(ST, PrimitiveType.Real64, ea);
        }

        private void RunFpuStackReturnGuesser()
        {
            var context = new AnalysisContext(
                new Program(), m.Procedure, null, null, new FakeDecompilerEventListener());
            var fpuGuesser = new FpuStackReturnGuesser(context);
            fpuGuesser.Transform(m.Ssa);
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        private void AssertProcedureCode(string expected)
        {
            ProcedureCodeVerifier.AssertCode(m.Ssa.Procedure, expected);
        }

        [Test(Description = "Assume FPU stack delta is 0 if there is not FPU stack uses after call")]
        [Category(Categories.UnitTests)]
        public void FPUG_NoFpuUses()
        {
            var top_1 = m.Reg("FakeTop_1", Top);
            var top_2 = m.Reg("FakeTop_2", Top);
            var f = m.Reg32("f");
            var uses = new Identifier[] { top_1 };
            var defines = new Identifier[] { top_2 };
            m.Call(f, 4, uses, defines);

            RunFpuStackReturnGuesser();

            var expected =
@"
call f (retsize: 4;)
	uses: Top:FakeTop_1
FakeTop_2 = FakeTop_1
";
            AssertProcedureCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void FPUG_ExtractReturnFromFpuStack()
        {
            var top_1 = m.Reg("FakeTop_1", Top);
            var top_2 = m.Reg("FakeTop_2", Top);
            var top_3 = m.Reg("FakeTop_3", Top);
            var a = m.Reg64("a");
            var f = m.Reg32("f");
            var uses = new Identifier[] { top_1 };
            var defines = new Identifier[] { top_2 };
            m.Call(f, 4, uses, defines);
            m.Assign(a, MemST(top_2));
            m.Assign(top_3, m.IAddS(top_2, 1));

            RunFpuStackReturnGuesser();

            var expected =
@"
call f (retsize: 4;)
	uses: Top:FakeTop_1
	defs: FPU -1:rRet0
FakeST8[FakeTop_1 - 1<8>:real64] = rRet0
FakeTop_2 = FakeTop_1 - 1<i8>
a = FakeST[FakeTop_2:real64]
FakeTop_3 = FakeTop_2 + 1<i8>
";
            AssertProcedureCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void FPUG_NegateFpuStackReturn()
        {
            var top_1 = m.Reg("FakeTop_1", Top);
            var top_2 = m.Reg("FakeTop_2", Top);
            var top_3 = m.Reg("FakeTop_3", Top);
            var a = m.Reg64("a");
            var f = m.Reg32("f");
            var uses = new Identifier[] { top_1 };
            var defines = new Identifier[] { top_2 };
            m.Call(f, 4, uses, defines);
            m.Store(MemST(top_2), m.Neg(MemST(top_2)));

            RunFpuStackReturnGuesser();

            var expected =
@"
call f (retsize: 4;)
	uses: Top:FakeTop_1
	defs: FPU -1:rRet0
FakeST9[FakeTop_1 - 1<8>:real64] = rRet0
FakeTop_2 = FakeTop_1 - 1<i8>
FakeST[FakeTop_2:real64] = -FakeST[FakeTop_2:real64]
";
            AssertProcedureCode(expected);
        }
    }
}

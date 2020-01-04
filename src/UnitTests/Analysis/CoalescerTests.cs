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

using Reko.Core;
using Reko.Analysis;
using Reko.Core.Code;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using Reko.Core.Expressions;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class CoalescerTests : AnalysisTestBase
    {
        private SsaProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new SsaProcedureBuilder();
        }

        public void RunCoalescer()
        {
            var co = new Coalescer(m.Ssa);
            co.Transform();
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        private void AssertProcedureCode(string expected)
        {
            ProcedureCodeVerifier.AssertCode(m.Ssa.Procedure, expected);
        }

        protected override void RunTest(Program program, TextWriter fut)
        {
            IDynamicLinker dynamicLinker = null;
            var listener = new FakeDecompilerEventListener();
            DataFlowAnalysis dfa = new DataFlowAnalysis(program, dynamicLinker, listener);
            var ssts = dfa.UntangleProcedures();

            foreach (Procedure proc in program.Procedures.Values)
            {
                var sst = ssts.Single(s => s.SsaState.Procedure == proc);
                SsaState ssa = sst.SsaState;

                ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa, program.Platform);
                cce.Transform();
                DeadCode.Eliminate(ssa);

                ValuePropagator vp = new ValuePropagator(program.SegmentMap, ssa, program.CallGraph, dynamicLinker, listener);
                vp.Transform();
                DeadCode.Eliminate(ssa);
                Coalescer co = new Coalescer(ssa);
                co.Transform();

                ssa.Write(fut);
                proc.Write(false, fut);
                fut.WriteLine();

                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });
            }
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void Coa3Converge()
        {
            RunFileTest_x86_real("Fragments/3converge.asm", "Analysis/Coa3Converge.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaAsciiHex()
        {
            RunFileTest_x86_real("Fragments/ascii_hex.asm", "Analysis/CoaAsciiHex.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaDataConstraint()
        {
            RunFileTest_x86_real("Fragments/data_constraint.asm", "Analysis/CoaDataConstraint.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaMoveChain()
        {
            RunFileTest_x86_real("Fragments/move_sequence.asm", "Analysis/CoaMoveChain.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaFactorialReg()
        {
            RunFileTest_x86_real("Fragments/factorial_reg.asm", "Analysis/CoaFactorialReg.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaMemoryTest()
        {
            RunFileTest_x86_real("Fragments/simple_memoperations.asm", "Analysis/CoaMemoryTest.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaSmallLoop()
        {
            RunFileTest_x86_real("Fragments/small_loop.asm", "Analysis/CoaSmallLoop.txt");
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        public void CoaAddSubCarries()
        {
            RunFileTest_x86_real("Fragments/addsubcarries.asm", "Analysis/CoaAddSubCarries.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaConditionals()
        {
            RunFileTest_x86_real("Fragments/multiple/conditionals.asm", "Analysis/CoaConditionals.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaSliceReturn()
        {
            RunFileTest_x86_real("Fragments/multiple/slicereturn.asm", "Analysis/CoaSliceReturn.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaReg00002()
        {
            RunFileTest_x86_real("Fragments/regression00002.asm", "Analysis/CoaReg00002.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaWhileGoto()
        {
            RunFileTest_x86_real("Fragments/while_goto.asm", "Analysis/CoaWhileGoto.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CoaSideEffectCalls()
        {
            RunFileTest_x86_real("Fragments/multiple/sideeffectcalls.asm", "Analysis/CoaSideEffectCalls.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CoaCallUses()
        {
            var r2 = m.Reg32("r2");
            var r3 = m.Reg32("r3");
            var r4 = m.Reg32("r4");
            m.Assign(r4, m.Fn(r2));
            var uses = new Identifier[] { r2, r3, r4 };
            var defines = new Identifier[] { };
            m.Call(r3, 4, uses, defines);

            RunCoalescer();

            var expected =
@"
call r3 (retsize: 4;)
	uses: r2:r2,r3:r3,r4:r2()
";
            AssertProcedureCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CoaCallCallee()
        {
            var r2 = m.Reg32("r2");
            var r3 = m.Reg32("r3");
            m.Assign(r3, m.Fn(r2));
            var uses = new Identifier[] { r2 };
            var defines = new Identifier[] { };
            m.Call(m.IAdd(r3, 4), 4, uses, defines);

            RunCoalescer();

            var expected =
@"
call r2() + 0x00000004 (retsize: 4;)
	uses: r2:r2
";
            AssertProcedureCode(expected);
        }

        [Test(Description = "Avoid coalescing of invalid constant")]
        [Category(Categories.UnitTests)]
        public void CoaDoNotCoalesceInvalidConstant()
        {
            var a = m.Reg32("a");
            var b = m.Reg32("b");
            m.Assign(a, Constant.Invalid);
            m.Assign(b, m.IAdd(a, 4));

            RunCoalescer();

            var expected =
@"
a = <invalid>
b = a + 0x00000004
";
            AssertProcedureCode(expected);
        }

        [Test(Description = "Coalescense should work across a comment.")]
        [Category(Categories.UnitTests)]
        public void CoaAcrossComment()
        {
            var a = m.Reg32("a");
            var b = m.Reg32("b");
            m.Assign(a, m.Mem32(m.Word32(0x00123400)));
            m.Comment("This is a comment");
            m.Assign(b, m.Mem32(a));

            RunCoalescer();

            var sExp =
@"
// This is a comment
b = Mem3[Mem2[0x00123400:word32]:word32]
";
            AssertProcedureCode(sExp);
        }

        [Test]
        public void CoaIdentifier()
        {
            var a = m.Reg32("a");
            var b = m.Reg32("b");
            var c = m.Reg32("c");
            m.Assign(a, b);
            m.Assign(c, m.IAddS(a, 1));

            RunCoalescer();

            var sExp =
@"
c = b + 1
";
            AssertProcedureCode(sExp);
        }
    }
}

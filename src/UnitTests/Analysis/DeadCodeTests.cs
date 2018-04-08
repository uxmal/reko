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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Analysis;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;


namespace Reko.UnitTests.Analysis
{
	/// <summary>
	/// Tests to make sure DeadCodeElimination works.
	/// </summary>
	[TestFixture]
	public class DeadCodeTests : AnalysisTestBase
	{
        private SsaProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new SsaProcedureBuilder();
        }

        public void EliminateDeadCode()
        {
            DeadCode.Eliminate(m.Ssa.Procedure, m.Ssa);
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        private void AssertProcedureCode(string expected)
        {
            ProcedureCodeVerifier.AssertCode(m.Ssa.Procedure, expected);
        }

        [Test]
		public void DeadPushPop()
		{
			RunFileTest("Fragments/pushpop.asm", "Analysis/DeadPushPop.txt");
		}

		[Test]
		public void DeadFactorialReg()
		{
			RunFileTest("Fragments/factorial_reg.asm", "Analysis/DeadFactorialReg.txt");
		}

		[Test]
		public void DeadFactorial()
		{
			RunFileTest("Fragments/factorial.asm", "Analysis/DeadFactorial.txt");
		}

		[Test]
		public void Dead3Converge()
		{
			RunFileTest("Fragments/3converge.asm", "Analysis/Dead3Converge.txt");
		}

		[Test]
		public void DeadCmpMock()
		{
			RunFileTest(new CmpMock(), "Analysis/DeadCmpMock.txt");
		}

		[Test]
		public void DeadFnReturn()
		{
			ProcedureBuilder m = new ProcedureBuilder("foo");
			Identifier unused = m.Local32("unused");
			m.Assign(unused, m.Fn("foo", Constant.Word32(1)));
			m.Return();
			RunFileTest(m, "Analysis/DeadFnReturn.txt");
		}

        [Test(Description = "Comment should not be removed as dead code")]
        public void DeadComment()
        {
            var dead = m.Reg16("dead");
            m.Comment("This is a comment");
            m.Assign(dead, m.Word16(0xDEAD));

            EliminateDeadCode();

            var sExp =
@"
// This is a comment
";
            AssertProcedureCode(sExp);
        }

        protected override void RunTest(Program program, TextWriter writer)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(program, null,  new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in program.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, program.Architecture);
				alias.Transform();
				SsaTransform sst = new SsaTransform(
                    dfa.ProgramDataFlow,
                    proc,
                    null,
                    proc.CreateBlockDominatorGraph(),
                    program.Platform.CreateImplicitArgumentRegisters());
				SsaState ssa = sst.SsaState;
				ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa, program.Platform);
				cce.Transform();

				DeadCode.Eliminate(proc, ssa);
				ssa.Write(writer);
				proc.Write(false, writer);
			}
		}
 	}
}

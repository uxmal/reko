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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Diagnostics;

namespace Reko.UnitTests.Analysis
{
    /// <summary>
    /// Tests to make sure DeadCodeElimination works.
    /// </summary>
    [TestFixture]
	public class DeadCodeTests : AnalysisTestBase
	{
        private ProgramDataFlow programDataFlow;

        [SetUp]
        public void Setup()
        {
            this.programDataFlow = new ProgramDataFlow();
        }

        protected void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var m = new ProcedureBuilder();
            builder(m);

            var program = new Program()
            {
                Architecture = m.Architecture,
            };
            var sst = new SsaTransform(
                program,
                m.Procedure,
                new HashSet<Procedure>(),
                null,
                programDataFlow);
            sst.Transform();

            DeadCode.Eliminate(sst.SsaState);
            var sw = new StringWriter();
            sst.SsaState.Procedure.Write(false, sw);
            if (sw.ToString() != sExp)
            {
                Debug.WriteLine(sw.ToString());
                Assert.AreEqual(sExp, sw.ToString());
            }
        }

		protected override void RunTest(Program program, TextWriter writer)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(program, null,  new FakeDecompilerEventListener());
			var ssts = dfa.UntangleProcedures();
			foreach (var sst in ssts)
			{
				SsaState ssa = sst.SsaState;
				ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa, program.Platform);
				cce.Transform();

				DeadCode.Eliminate(ssa);
				ssa.Write(writer);
				ssa.Procedure.Write(false, writer);
			}
		}

        private Procedure Given_Procedure_With_Flow(ProcedureBuilder m, string name, Storage[] uses, Storage[] defs)
        {
            var sig = new FunctionType();
            var proc = new Procedure(m.Architecture, name, m.Architecture.CreateFrame());
            var flow = new ProcedureFlow(proc);
            flow.BitsUsed = uses.ToDictionary(u => u, u => new BitRange(0, (int)u.BitSize / 8));
            flow.Trashed = defs.ToHashSet();
            this.programDataFlow[proc] = flow;
            return proc;
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void DeadPushPop()
		{
			RunFileTest_x86_real("Fragments/pushpop.asm", "Analysis/DeadPushPop.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DeadFactorialReg()
		{
			RunFileTest_x86_real("Fragments/factorial_reg.asm", "Analysis/DeadFactorialReg.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DeadFactorial()
		{
			RunFileTest_x86_real("Fragments/factorial.asm", "Analysis/DeadFactorial.txt");
		}

		[Test]
		public void Dead3Converge()
		{
			RunFileTest_x86_real("Fragments/3converge.asm", "Analysis/Dead3Converge.txt");
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
            m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
			m.Assign(unused, m.Fn("foo", Constant.Word32(1)));
			m.Return();
			RunFileTest(m, "Analysis/DeadFnReturn.txt");
		}

        [Test(Description = "If a call defines a dead variable, remove it from the call instruction")]
        public void DeadCallDefinition()
        {
         var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r1
	// succ:  l1
l1:
	call foo (retsize: 4;)
		uses: r1:r1
		defs: r1:r1_2
	Mem4[0x00123400:word32] = r1_2
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var _r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);
                var _r2 = new RegisterStorage("r2", 2, 0, PrimitiveType.Word32);
                var foo = Given_Procedure_With_Flow(m,
                    "foo",
                    new Storage[] { _r1 }, 
                    new Storage[] { _r1, _r2 });

                var r1 = m.Frame.EnsureRegister(_r1);
                var r2 = m.Frame.EnsureRegister(_r2);
                var call = m.Call(foo, 4);
                m.MStore(m.Word32(0x123400), r1);
                m.Return();
            });
        }
    }
}

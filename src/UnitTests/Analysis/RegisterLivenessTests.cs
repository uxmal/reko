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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    /// <summary>
    /// Used to test register liveness across the whole program.
    /// This shows the result of analysis just before the call rewriting
    /// takes place.
    /// </summary>
    [TestFixture]
	public class RegisterLivenessTests : AnalysisTestBase
	{
        private SortedList<Address, Procedure_v1> userSigs;
        private Mock<IDynamicLinker> dynamicLinker;

        [SetUp]
        public void Setup()
        {
            userSigs = new SortedList<Address, Procedure_v1>();
            this.dynamicLinker = new Mock<IDynamicLinker>();
        }

		protected override void RunTest(Program program, TextWriter writer)
		{
			var eventListener = new FakeDecompilerEventListener();
			var dfa = new DataFlowAnalysis(
                program, 
                dynamicLinker.Object, 
                eventListener);
            program.User.Procedures = userSigs;
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures(eventListener);

            IntraBlockDeadRegisters.Apply(program, eventListener);

            var ssts = dfa.RewriteProceduresToSsa();

            // Discover ssaId's that are live out at each call site.
            // Delete all others.
            var uvr = new UnusedOutValuesRemover(
                program,
                ssts.Select(sst => sst.SsaState),
                dfa.ProgramDataFlow,
                dynamicLinker.Object,
                eventListener);
            uvr.Transform();
            DumpProcedureFlows(program, dfa, writer);
		}

        private void Given_Signature(Address addr, string procName, SerializedSignature ssig)
        {
            userSigs[addr] = new Procedure_v1
            {
                 Address = addr.ToString(),
                 Name = procName,
                 Signature = ssig,
            };
        }

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RlDataConstraint()
		{
			RunFileTest_x86_real("Fragments/data_constraint.asm", "Analysis/RlDataConstraint.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RlOneProcedure()
        {
			RunFileTest_x86_real("Fragments/one_procedure.asm", "Analysis/RlOneProcedure.txt");
		}

		/// <summary>
		/// Test that self-recursive functions are handled correctly.
		/// </summary>
		[Test]
        [Ignore(Categories.FailedTests)]
        public void RlFactorialReg()
		{
			RunFileTest_x86_real("Fragments/factorial_reg.asm", "Analysis/RlFactorialReg.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlFactorial()
		{
			RunFileTest_x86_real("Fragments/factorial.asm", "Analysis/RlFactorial.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlCalleeSave()
		{
			RunFileTest_x86_real("Fragments/callee_save.asm", "Analysis/RlCalleeSave.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlDeepNest()
		{
			RunFileTest_x86_real("Fragments/deep_nest.asm", "Analysis/RlDeepNest.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlSequence()
		{
			RunFileTest_x86_real("Fragments/sequence_calls_reg.asm", "Analysis/RlSequence.txt");
		}

		[Test]
		public void RlMiniFloats()
		{
			RunFileTest_x86_real("Fragments/mini_msfloats_regs.asm", "Analysis/RlMiniFloats.txt");
		}

		[Test]
		[Ignore("Won't pass until ProcedureSignatures for call tables and call pointers are implemented")]
		public void RlCallTables()
		{
			RunFileTest_x86_real("Fragments/multiple/calltables.asm", "Analysis/RlCallTables.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RlConditionals()
        {
			RunFileTest_x86_real("Fragments/multiple/conditionals.asm", "Analysis/RlConditionals.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RlFpuOps()
        {
			RunFileTest_x86_real("Fragments/fpuops.asm", "Analysis/RlFpuOps.txt");
		}

		[Test]
		public void RlIpLiveness()
		{
			RunFileTest_x86_real("Fragments/multiple/ipliveness.asm", "Analysis/RlIpLiveness.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void RlMutual()
		{
			RunFileTest_x86_real("Fragments/multiple/mutual.asm", "Analysis/RlMutual.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlStackVariables()
		{
			RunFileTest_x86_real("Fragments/stackvars.asm", "Analysis/RlStackVariables.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlProcIsolation()
		{
			RunFileTest_x86_real("Fragments/multiple/procisolation.asm", "Analysis/RlProcIsolation.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlLeakyLiveness()
		{
			RunFileTest_x86_real("Fragments/multiple/leaky_liveness.asm", "Analysis/RlLeakyLiveness.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlStringInstructions()
		{
			RunFileTest_x86_real("Fragments/stringinstr.asm", "Analysis/RlStringInstructions.txt");
		}

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlTermination()
        {
            Given_Signature(
                Address.SegPtr(0xC00, 0x15),
                "maybeterminate",
                new SerializedSignature
                {
                    ReturnValue = new Argument_v1 { Kind = new Register_v1("ax") },
                    Arguments = new[]
                    {
                        new Argument_v1 { Kind = new Register_v1("ax") },
                        new Argument_v1 { Kind = new Register_v1("bx") }
                    }
                });
			RunFileTest_x86_real("Fragments/multiple/termination.asm", "Analysis/RlTermination.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void RlLivenessAfterCall()
 		{
			RunFileTest_x86_real("Fragments/multiple/livenessaftercall.asm", "Analysis/RlLivenessAfterCall.txt");
		}

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlPushedRegisters()
        {
            RunFileTest_x86_real("Fragments/multiple/pushed_registers.asm", "Analysis/RlPushedRegisters.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlReg00005()
        {
            RunFileTest_x86_real("Fragments/regressions/r00005.asm", "Analysis/RlReg00005.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlReg00007()
        {
            RunFileTest_x86_real("Fragments/regressions/r00007.asm", "Analysis/RlReg00007.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlReg00010()
        {
            RunFileTest_x86_real("Fragments/regressions/r00010.asm", "Analysis/RlReg00010.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlReg00015()
        {
            RunFileTest_x86_real("Fragments/regressions/r00015.asm", "Analysis/RlReg00015.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlPushPop()
        {
            RunFileTest_x86_real("Fragments/pushpop.asm", "Analysis/RlPushPop.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlChainTest()
        {
            RunFileTest_x86_real("Fragments/multiple/chaincalls.asm", "Analysis/RlChainTest.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlSliceReturn()
        {
            RunFileTest_x86_real("Fragments/multiple/slicereturn.asm", "Analysis/RlSliceReturn.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void RlRecurseWithPushes()
        {
            RunFileTest_x86_real("Fragments/multiple/recurse_with_pushes.asm", "Analysis/RlRecurseWithPushes.txt");
        }
    }
}

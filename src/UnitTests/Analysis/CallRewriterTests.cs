#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using Reko.Core.Types;
using System.Collections.Generic;
using Reko.Core.Expressions;
using Reko.Arch.X86;
using Reko.Core.Code;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class CallRewriterTests : AnalysisTestBase
	{
		private DataFlowAnalysis dfa;
        private Program program;
        private CallRewriter crw;
        private Procedure proc;
        private ProcedureFlow flow;

        [SetUp]
        public void Setup()
        {
            program = new Program();
            program.Architecture = new X86ArchitectureFlat32();
            program.Platform = new DefaultPlatform(null, program.Architecture);
            crw = new CallRewriter(program.Platform, new ProgramDataFlow(), new FakeDecompilerEventListener());
            proc = new Procedure("foo", program.Architecture.CreateFrame());
            flow = new ProcedureFlow(proc);
        }

        protected override void RunTest(Program program, TextWriter writer)
		{
            var eventListener = new FakeDecompilerEventListener();

            dfa = new DataFlowAnalysis(program, null, eventListener);
            var ssts = dfa.RewriteProceduresToSsa();

            // Discover ssaId's that are live out at each call site.
            // Delete all others.
            var uvr = new UnusedOutValuesRemover(program, ssts, dfa.ProgramDataFlow, eventListener);
            uvr.Transform();

            // At this point, the exit blocks contain only live out registers.
            // We can create signatures from that.
            CallRewriter.Rewrite(program.Platform, ssts, dfa.ProgramDataFlow, eventListener);
            foreach (var proc in program.Procedures.Values)
			{
				ProcedureFlow flow = dfa.ProgramDataFlow[proc];
				proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.ArgumentKind, new TextFormatter(writer));
				writer.WriteLine();
				flow.Emit(program.Architecture, writer);
				proc.Write(true, writer);
				writer.Flush();
			}
		}


        private void Given_ExitBlockStatement(Identifier id)
        {
            proc.ExitBlock.Statements.Add(0x10020, new UseInstruction(id));
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CrwAsciiHex()
		{
			RunFileTest_x86_real("Fragments/ascii_hex.asm", "Analysis/CrwAsciiHex.txt");
		}

		[Test]
		public void CrwNoCalls()
		{
			RunFileTest_x86_real("Fragments/diamond.asm", "Analysis/CrwNoCalls.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CrwEvenOdd()
		{
			RunFileTest_x86_real("Fragments/multiple/even_odd.asm", "Analysis/CrwEvenOdd.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CrwFactorial()
		{
			RunFileTest_x86_real("Fragments/factorial.asm", "Analysis/CrwFactorial.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CrwFactorialReg()
		{
			RunFileTest_x86_real("Fragments/factorial_reg.asm", "Analysis/CrwFactorialReg.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CrwLeakyLiveness()
		{
			RunFileTest_x86_real("Fragments/multiple/leaky_liveness.asm", "Analysis/CrwLeakyLiveness.txt");
		}

		[Test]
		public void CrwManyStackArgs()
		{
			RunFileTest_x86_real("Fragments/multiple/many_stack_args.asm", "Analysis/CrwManyStackArgs.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CrwStackVariables()
		{
			RunFileTest_x86_real("Fragments/stackvars.asm", "Analysis/CrwStackVariables.txt");
		}

		[Test]
		[Ignore("Won't pass until ProcedureSignatures for call tables and call pointers are implemented")]
		public void CrwCallTables()
		{
			RunFileTest_x86_real("Fragments/multiple/calltables.asm", "Analysis/CrwCallTables.txt");
		}

		[Test]
		public void CrwFpuArgs()
		{
			RunFileTest_x86_real("Fragments/multiple/fpuArgs.asm", "Analysis/CrwFpuArgs.txt");
		}

		[Test]
		public void CrwFpuOps()
		{
			RunFileTest_x86_real("Fragments/fpuops.asm", "Analysis/CrwFpuOps.txt");
		}

		[Test]
		public void CrwIpLiveness()
		{
			RunFileTest_x86_real("Fragments/multiple/ipliveness.asm", "Analysis/CrwIpLiveness.txt");
		}

		[Test]
		public void CrwVoidFunctions()
		{
			RunFileTest_x86_real("Fragments/multiple/voidfunctions.asm", "Analysis/CrwVoidFunctions.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CrwMutual()
		{
			RunFileTest_x86_real("Fragments/multiple/mutual.asm", "Analysis/CrwMutual.txt");
		}

		[Test]
        [Ignore("scanning-development")]
        public void CrwMemPreserve()
		{
			RunFileTest_x86_real("Fragments/multiple/mempreserve.asm", "Analysis/CrwMemPreserve.xml", "Analysis/CrwMemPreserve.txt");
		}

		[Test]
		public void CrwSliceReturn()
		{
			RunFileTest_x86_real("Fragments/multiple/slicereturn.asm", "Analysis/CrwSliceReturn.txt");
		}

		[Test]
		public void CrwProcIsolation()
		{
			RunFileTest_x86_real("Fragments/multiple/procisolation.asm", "Analysis/CrwProcIsolation.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CrwFibonacci()
		{
			RunFileTest_x86_32("Fragments/multiple/fibonacci.asm", "Analysis/CrwFibonacci.txt");
		}

        [Test]
        public void CrwRegisterArgument()
        {
            flow.BitsUsed.Add(Registers.eax, 32);
            crw.EnsureSignature(proc, flow);
            Assert.AreEqual("void foo(Register word32 eax)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        public void CrwRegisterOutArgument()
        {
            Given_ExitBlockStatement(new Identifier("eax", PrimitiveType.Word32, Registers.eax));
            Given_ExitBlockStatement(new Identifier("ebx", PrimitiveType.Word32, Registers.ebx));
            flow.LiveOut.Add(Registers.eax);        // becomes the return value.
            flow.LiveOut.Add(Registers.ebx);
            crw.EnsureSignature(proc, flow);
            Assert.AreEqual("Register word32 foo(Register out ptr32 ebxOut)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        public void CrwFpuArgument()
        {
            proc.Frame.EnsureFpuStackVariable(1, PrimitiveType.Real80);
            crw.EnsureSignature(proc, flow);
            Assert.AreEqual("void foo(FpuStack real80 rArg1)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        public void CrwFpuOutArgument()
        {
            flow.LiveOut.Add(Registers.eax);
            Given_ExitBlockStatement(new Identifier("eax", PrimitiveType.Word32, Registers.eax));
            Given_ExitBlockStatement(new Identifier("st0", PrimitiveType.Word32, new FpuStackStorage(0, PrimitiveType.Real80)));
            Given_ExitBlockStatement(new Identifier("st2", PrimitiveType.Word32, new FpuStackStorage(1, PrimitiveType.Real80)));

            proc.Frame.EnsureFpuStackVariable(0, PrimitiveType.Real80);
            proc.Frame.EnsureFpuStackVariable(1, PrimitiveType.Real80);
            proc.Signature.FpuStackDelta = 1;
            crw.EnsureSignature(proc, flow);
            Assert.AreEqual("Register word32 foo(FpuStack real80 rArg0, FpuStack real80 rArg1, FpuStack out ptr32 rArg0Out)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        public void NarrowedStackArgument()
        {
            var arg = proc.Frame.EnsureStackArgument(4, PrimitiveType.Word32);
            flow.StackArguments[arg] = 16;
            crw.EnsureSignature(proc, flow);
            Assert.AreEqual("void foo(Stack uipr16 dwArg04)", proc.Signature.ToString(proc.Name));
        }

        // Ensure that UseInstructions for "out" parameters are generated even when a signature is pre-specified.
        [Test]
        public void GenerateUseInstructionsForSpecifiedSignature()
        {
            Procedure proc = new Procedure("foo", program.Architecture.CreateFrame());
            proc.Signature = FunctionType.Func(
                new Identifier("eax", PrimitiveType.Word32, Registers.eax),
                new Identifier[] {
                new Identifier("ecx", PrimitiveType.Word32, Registers.ecx),
                new Identifier("edxOut", PrimitiveType.Word32,
                                    new OutArgumentStorage(proc.Frame.EnsureRegister(Registers.edx)))});
            crw.EnsureSignature(proc, new ProcedureFlow(proc));
            crw.AddUseInstructionsForOutArguments(proc);
            Assert.AreEqual(1, proc.ExitBlock.Statements.Count);
            Assert.AreEqual("use edx (=> edxOut)", proc.ExitBlock.Statements[0].Instruction.ToString());
        }

        [Test]
        public void GcrStackArguments()
        {
            Frame f = program.Architecture.CreateFrame();
            f.ReturnAddressKnown = true;
            f.ReturnAddressSize = PrimitiveType.Word16.Size;

            f.EnsureStackVariable(Constant.Word16(8), 2, PrimitiveType.Word16);
            f.EnsureStackVariable(Constant.Word16(6), 2, PrimitiveType.Word16);
            f.EnsureStackVariable(Constant.Word16(0x0E), 2, PrimitiveType.Word32);

            CallRewriter gcr = new CallRewriter(null, null, new FakeDecompilerEventListener());
            using (FileUnitTester fut = new FileUnitTester("Analysis/GcrStackParameters.txt"))
            {
                foreach (KeyValuePair<int, Identifier> de in gcr.GetSortedStackArguments(f))
                {
                    fut.TextWriter.Write("{0:X4} ", de.Key);
                    de.Value.Write(true, fut.TextWriter);
                    fut.TextWriter.WriteLine();
                }
                fut.AssertFilesEqual();
            }
        }

        private class NestedProgram
        {
            public static Program Build()
            {
                ProgramBuilder m = new ProgramBuilder();
                m.Add(new MainFn());
                m.Add(new Leaf());
                return m.BuildProgram();
            }

            public class MainFn : ProcedureBuilder
            {
                private FakeArchitecture arch = new FakeArchitecture();

                protected override void BuildBody()
                {
                    base.Call("Leaf", 4);
                    Store(Int32(0x320123), base.Register(0));
                }
            }

            public class Leaf : ProcedureBuilder
            {
                protected override void BuildBody()
                {
                    Assign(Register(0), Int32(3));
                    Return();
                }
            }
        }
    }
}


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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class CallRewriterTests : AnalysisTestBase
    {
        private DataFlowAnalysis dfa;
        private Program program;
        private CallRewriter crw;
        private Procedure proc;
        private SsaState ssa;
        private ProcedureFlow flow;
        ProgramBuilder pb;
        private List<SsaState> ssaStates;
        private FakeDecompilerEventListener eventListener;

        [SetUp]
        public void Setup()
        {
            program = new Program();
            program.Architecture = new X86ArchitectureFlat32("x86-protected-32");
            program.Platform = new DefaultPlatform(null, program.Architecture);
            crw = new CallRewriter(program.Platform, new ProgramDataFlow(), new FakeDecompilerEventListener());
            proc = new Procedure(program.Architecture, "foo", Address.Ptr32(0x00123400), program.Architecture.CreateFrame());
            flow = new ProcedureFlow(proc);
            ssa = new SsaState(proc);
            pb = new ProgramBuilder();
            ssaStates = new List<SsaState>();
            eventListener = new FakeDecompilerEventListener();

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
                    Store(Int32(0x320123), base.Register("r0"));
                }
            }

            public class Leaf : ProcedureBuilder
            {
                protected override void BuildBody()
                {
                    Assign(Register("r0"), Int32(3));
                    Return();
                }
            }
        }

        protected override void RunTest(Program program, TextWriter writer)
        {
            var dynamicLinker = new Mock<IDynamicLinker>();

            dfa = new DataFlowAnalysis(program, dynamicLinker.Object, eventListener);
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

            foreach (var p in program.Procedures.Values)
            {
                p.Dump(true);
                Debug.Print("====");
            }

            // At this point, the exit blocks contain only live out registers.
            // We can create signatures from that.
            CallRewriter.Rewrite(program.Platform, ssts, dfa.ProgramDataFlow, eventListener);
            foreach (var proc in program.Procedures.Values)
            {
                var flow = dfa.ProgramDataFlow[proc];
                proc.Signature.Emit(proc.Name, FunctionType.EmitFlags.ArgumentKind, new TextFormatter(writer));
                writer.WriteLine();
                flow.Emit(program.Architecture, writer);
                proc.Write(true, writer);
                writer.Flush();
            }
            ssts.ForEach(sst => sst.SsaState.Validate(s => Assert.Fail(s)));
        }

        private void Given_ExitBlockStatement(Identifier id)
        {
            proc.ExitBlock.Statements.Add(0x10020, new UseInstruction(id));
        }

        private SsaState Given_Procedure(
            string name,
            Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder(name);
            builder(m);
            pb.Add(m);
            ssaStates.Add(m.Ssa);
            return m.Ssa;
        }

        private void Given_Signature(string name, FunctionType signature)
        {
            foreach (var ssa in ssaStates)
            {
                if (ssa.Procedure.Name == name)
                {
                    ssa.Procedure.Signature = signature;
                }
            }
        }

        private CallRewriter When_CallRewriterCreated()
        {
            var program = pb.BuildProgram();
            var flow = new ProgramDataFlow(program);
            var crw = new CallRewriter(
                program.Platform,
                flow,
                new FakeDecompilerEventListener());
            return crw;
        }

        private void When_RewriteCalls(SsaState ssa)
        {
            var crw = When_CallRewriterCreated();
            crw.RewriteCalls(ssa);
            ssa.Validate(s => Assert.Fail(s));
        }

        private void When_RewriteReturns(SsaState ssa)
        {
            var crw = When_CallRewriterCreated();
            crw.RewriteReturns(ssa);
            ssa.Validate(s => Assert.Fail(s));
        }

        private void AssertExpected(string sExp, SsaState ssa)
        {
            var sw = new StringWriter();
            ssa.Write(sw);
            ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void AssertProcedureCode(string expected, SsaState ssa)
        {
            var writer = new StringWriter();
            ssa.Procedure.WriteBody(false, writer);
            var actual = writer.ToString();
            if (actual != expected)
            {
                Console.WriteLine(actual);
            }
            Assert.AreEqual(expected, actual);
        }

        private void AddUserProc(Program program, Address address, string name, SerializedSignature sSig)
        {
            program.User.Procedures.Add(
                address,
                new Procedure_v1
                {
                    Name = name,
                    Address = address.ToString(),
                    Decompile = true,
                    Signature = sSig,
                });
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwAsciiHex()
        {
            RunFileTest_x86_real("Fragments/ascii_hex.asm", "Analysis/CrwAsciiHex.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwNoCalls()
        {
            RunFileTest_x86_real("Fragments/diamond.asm", "Analysis/CrwNoCalls.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwEvenOdd()
        {
            RunFileTest_x86_real("Fragments/multiple/even_odd.asm", "Analysis/CrwEvenOdd.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwFactorial()
        {
            RunFileTest_x86_real("Fragments/factorial.asm", "Analysis/CrwFactorial.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwFactorialReg()
        {
            RunFileTest_x86_real("Fragments/factorial_reg.asm", "Analysis/CrwFactorialReg.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwLeakyLiveness()
        {
            RunFileTest_x86_real("Fragments/multiple/leaky_liveness.asm", "Analysis/CrwLeakyLiveness.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwManyStackArgs()
        {
            RunFileTest_x86_real("Fragments/multiple/many_stack_args.asm", "Analysis/CrwManyStackArgs.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
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
        [Category(Categories.IntegrationTests)]
        public void CrwFpuArgs()
        {
            RunFileTest_x86_real("Fragments/multiple/fpuArgs.asm", "Analysis/CrwFpuArgs.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwFpuOps()
        {
            RunFileTest_x86_real("Fragments/fpuops.asm", "Analysis/CrwFpuOps.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwIpLiveness()
        {
            RunFileTest_x86_real("Fragments/multiple/ipliveness.asm", "Analysis/CrwIpLiveness.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwVoidFunctions()
        {
            RunFileTest_x86_real("Fragments/multiple/voidfunctions.asm", "Analysis/CrwVoidFunctions.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwMutual()
        {
            RunFileTest_x86_real("Fragments/multiple/mutual.asm", "Analysis/CrwMutual.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwMemPreserve()
        {
            Program program = RewriteMsdosAssembler("Fragments/multiple/mempreserve.asm", p =>
            {
                var ax = new Argument_v1 { Kind = new Register_v1 { Name = "ax" } };
                AddUserProc(p, Address.SegPtr(0x0C00, 0x0000), "main", null);
                AddUserProc(p, Address.SegPtr(0x0C00, 0x0017), "memfoo", new SerializedSignature { Arguments = new Argument_v1[0], ReturnValue = ax });
                AddUserProc(p, Address.SegPtr(0x0C00, 0x0040), "membar", new SerializedSignature { Arguments = new Argument_v1[1] { ax }, ReturnValue = ax });
            });
            SaveRunOutput(program, RunTest, "Analysis/CrwMemPreserve.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwSliceReturn()
        {
            RunFileTest_x86_real("Fragments/multiple/slicereturn.asm", "Analysis/CrwSliceReturn.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwProcIsolation()
        {
            RunFileTest_x86_real("Fragments/multiple/procisolation.asm", "Analysis/CrwProcIsolation.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwFibonacci()
        {
            RunFileTest_x86_32("Fragments/multiple/fibonacci.asm", "Analysis/CrwFibonacci.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwRegisterArgument()
        {
            flow.BitsUsed.Add(Registers.eax, new BitRange(0, 32));
            crw.EnsureSignature(ssa, proc.Frame, flow);
            Assert.AreEqual("void foo(Register word32 eax)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwRegisterOutArgument()
        {
            Given_ExitBlockStatement(new Identifier("eax", PrimitiveType.Word32, Registers.eax));
            Given_ExitBlockStatement(new Identifier("ebx", PrimitiveType.Word32, Registers.ebx));
            flow.BitsLiveOut.Add(Registers.eax, BitRange.Empty);        // becomes the return value.
            flow.BitsLiveOut.Add(Registers.ebx, BitRange.Empty);
            crw.EnsureSignature(ssa, proc.Frame, flow);
            Assert.AreEqual("Register word32 foo(Register out ptr32 ebxOut)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwFpuArgument()
        {
            flow.BitsUsed.Add(new FpuStackStorage(0, PrimitiveType.Real80), new BitRange(0, 80));
            crw.EnsureSignature(ssa, proc.Frame, flow);
            Assert.AreEqual("void foo(FpuStack real80 rArg0)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwFpuOutArgument()
        {
            flow.BitsUsed.Add(new FpuStackStorage(0, PrimitiveType.Real80), new BitRange(0, 80));
            flow.BitsLiveOut.Add(Registers.eax, BitRange.Empty);
            flow.BitsLiveOut.Add(new FpuStackStorage(0, PrimitiveType.Real80), BitRange.Empty);
            flow.BitsLiveOut.Add(new FpuStackStorage(1, PrimitiveType.Real80), BitRange.Empty);

            crw.EnsureSignature(ssa, proc.Frame, flow);
            Assert.AreEqual("Register word32 foo(FpuStack real80 rArg0, FpuStack out ptr32 rArg0Out, FpuStack out ptr32 rArg1Out)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        public void GcrNarrowedStackArgument()
        {
            var arg = proc.Frame.EnsureStackArgument(4, PrimitiveType.Word32);
            flow.BitsUsed[arg.Storage] = new BitRange(0, 16);
            crw.EnsureSignature(ssa, proc.Frame, flow);
            Assert.AreEqual("void foo(Stack word16 wArg04)", proc.Signature.ToString(proc.Name));
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void GcrStackArguments()
        {
            Frame f = program.Architecture.CreateFrame();
            f.ReturnAddressKnown = true;
            f.ReturnAddressSize = PrimitiveType.Word16.Size;

            var uses = new List<KeyValuePair<Storage, BitRange>>
            {
                new KeyValuePair<Storage,BitRange>(new StackArgumentStorage(8, PrimitiveType.Word16), new BitRange(0, 16)),
                new KeyValuePair<Storage,BitRange>(new StackArgumentStorage(6, PrimitiveType.Word16), new BitRange(0, 16)),
                new KeyValuePair<Storage,BitRange>(new StackArgumentStorage(0xE, PrimitiveType.Word32), new BitRange(0, 32))
            };

            CallRewriter gcr = new CallRewriter(null, null, new FakeDecompilerEventListener());
            using (FileUnitTester fut = new FileUnitTester("Analysis/GcrStackParameters.txt"))
            {
                foreach ((int, Identifier) de in gcr.GetSortedStackArguments(f, uses))
                {
                    fut.TextWriter.Write("{0:X4} ", de.Item1);
                    de.Item2.Write(true, fut.TextWriter);
                    fut.TextWriter.WriteLine();
                }
                fut.AssertFilesEqual();
            }
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwSinglePredecessorToExitBlock()
        {
            var m = new ProcedureBuilder("CrwSinglePredecessorToExitBlock");
            var eax = m.Frame.EnsureRegister(Registers.eax);
            m.Assign(eax, m.Mem32(eax));
            m.Return();

            var flow = new ProgramDataFlow(program);
            var sst = new SsaTransform(program, m.Procedure, new HashSet<Procedure>(), null, new ProgramDataFlow());
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.SsaState.Procedure.Signature = FunctionType.Func(
                new Identifier("", PrimitiveType.Word32, Registers.eax),
                new Identifier("eax", PrimitiveType.Word32, Registers.eax));

            var crw = new CallRewriter(this.platform, flow, new FakeDecompilerEventListener());
            crw.RewriteReturns(sst.SsaState);
            crw.RemoveStatementsFromExitBlock(sst.SsaState);

            var sExp =
            #region Expected 
@"eax:eax
    def:  def eax
    uses: eax_3 = Mem0[eax:word32]
Mem0:Mem
    def:  def Mem0
    uses: eax_3 = Mem0[eax:word32]
eax_3: orig: eax
    def:  eax_3 = Mem0[eax:word32]
    uses: return eax_3
// CrwSinglePredecessorToExitBlock
// Return size: 0
word32 CrwSinglePredecessorToExitBlock(word32 eax)
CrwSinglePredecessorToExitBlock_entry:
	def eax
	def Mem0
	// succ:  l1
l1:
	eax_3 = Mem0[eax:word32]
	return eax_3
	// succ:  CrwSinglePredecessorToExitBlock_exit
CrwSinglePredecessorToExitBlock_exit:
";
            #endregion
            AssertExpected(sExp, sst.SsaState);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwManyPredecessorsToExitBlock()
        {
            var m = new ProcedureBuilder("CrwManyPredecessorsToExitBlock");
            var eax = m.Frame.EnsureRegister(Registers.eax);

            m.BranchIf(m.Ge0(eax), "m2Ge");

            m.Label("m1Lt");
            m.Assign(eax, -1);
            m.Return();

            m.Label("m2Ge");
            m.BranchIf(m.Ne0(eax), "m4Gt");

            m.Label("m3Eq");
            m.Return();

            m.Label("m4Gt");
            m.Assign(eax, 1);
            m.Return();

            var flow = new ProgramDataFlow(program);
            var sst = new SsaTransform(program, m.Procedure, new HashSet<Procedure>(), null, new ProgramDataFlow());
            sst.Transform();
            sst.AddUsesToExitBlock();
            sst.SsaState.Procedure.Signature = FunctionType.Func(
                new Identifier("", PrimitiveType.Word32, Registers.eax),
                new Identifier("eax", PrimitiveType.Word32, Registers.eax));

            var crw = new CallRewriter(this.platform, flow, new FakeDecompilerEventListener());
            crw.RewriteReturns(sst.SsaState);
            crw.RemoveStatementsFromExitBlock(sst.SsaState);

            var sExp =
            #region Expected 
@"eax:eax
    def:  def eax
    uses: branch eax >= 0x00000000 m2Ge
          branch eax != 0x00000000 m4Gt
          return eax
eax_2: orig: eax
    def:  eax_2 = 0x00000001
    uses: return eax_2
eax_3: orig: eax
    def:  eax_3 = 0xFFFFFFFF
    uses: return eax_3
eax_4: orig: eax
// CrwManyPredecessorsToExitBlock
// Return size: 0
word32 CrwManyPredecessorsToExitBlock(word32 eax)
CrwManyPredecessorsToExitBlock_entry:
	def eax
	// succ:  l1
l1:
	branch eax >= 0x00000000 m2Ge
	// succ:  m1Lt m2Ge
m1Lt:
	eax_3 = 0xFFFFFFFF
	return eax_3
	// succ:  CrwManyPredecessorsToExitBlock_exit
m2Ge:
	branch eax != 0x00000000 m4Gt
	// succ:  m3Eq m4Gt
m3Eq:
	return eax
	// succ:  CrwManyPredecessorsToExitBlock_exit
m4Gt:
	eax_2 = 0x00000001
	return eax_2
	// succ:  CrwManyPredecessorsToExitBlock_exit
CrwManyPredecessorsToExitBlock_exit:
";
            #endregion
            AssertExpected(sExp, sst.SsaState);
        }

        [Test(Description = "Pops three values off FPU stack and places one back.")]
        [Category(Categories.UnitTests)]
        public void CrwFpuMultiplyAdd()
        {
            var dt = PrimitiveType.Real64;
            var arch = new FakeArchitecture();
            var ST = arch.FpuStackBase;
            var _top = arch.FpuStackRegister;
            var pb = new ProgramBuilder(arch);
            pb.Add("main", m =>
            {
                var Top = m.Frame.EnsureRegister(arch.FpuStackRegister);
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(Top, 0);
                m.Assign(Top, m.ISub(Top, 1));
                m.MStore(ST, Top, Constant.Real64(3.0));
                m.Assign(Top, m.ISub(Top, 1));
                m.MStore(ST, Top, Constant.Real64(4.0));
                m.Assign(Top, m.ISub(Top, 1));
                m.MStore(ST, Top, Constant.Real64(5.0));
                // At this point there are 3 values on the FPU stack
                m.Call("FpuMultiplyAdd", 0);
                m.MStore(m.Word32(0x00123400), m.Mem(ST, dt, Top));
                m.Assign(Top, m.IAdd(Top, 1));
                m.Return();
            });
            pb.Add("FpuMultiplyAdd", m =>
            {
                var Top = m.Frame.EnsureRegister(_top);

                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(Top, 0);
                m.MStore(ST, m.IAdd(Top, 1), m.FMul(
                    m.Mem(ST, dt, m.IAdd(Top, 1)),
                    m.Mem(ST, dt, Top)));
                m.Assign(Top, m.IAdd(Top, 1));
                m.MStore(ST, m.IAdd(Top, 1), m.FAdd(
                    m.Mem(ST, dt, m.IAdd(Top, 1)),
                    m.Mem(ST, dt, Top)));
                m.Assign(Top, m.IAdd(Top, 1));

                // At this point two values have been popped from the
                // FPU stack, leaving one.
                m.Return();
            });

            var sExp =
            #region Expected
@"void main()
// MayUse: 
// LiveOut:
// Trashed: FPU -1 FPU -2 FPU -3 Top
// Preserved: r63
// main
// Return size: 0
// Mem0:Mem
// fp:fp
// Top:Top
// r63:r63
// rRet0:rRet0
// rLoc1:FPU -1
// rLoc2:FPU -2
// rLoc3:FPU -3
// return address size: 0
void main()
main_entry:
	// succ:  l1
l1:
	rRet0_10 = FpuMultiplyAdd(5.0, 4.0, 3.0)
	Mem13[0x00123400:real64] = rRet0_10
	return
	// succ:  main_exit
main_exit:
FpuStack real64 FpuMultiplyAdd(FpuStack real64 rArg0, FpuStack real64 rArg1, FpuStack real64 rArg2)
// MayUse:  FPU +0:[0..63] FPU +1:[0..63] FPU +2:[0..63]
// LiveOut: FPU +2
// Trashed: FPU +1 FPU +2 Top
// Preserved: r63
// FpuMultiplyAdd
// Return size: 0
// Mem0:Mem
// fp:fp
// Top:Top
// r63:r63
// rArg1:FPU +1
// rArg0:FPU +0
// rArg2:FPU +2
// return address size: 0
real64 FpuMultiplyAdd(real64 rArg0, real64 rArg1, real64 rArg2)
FpuMultiplyAdd_entry:
	def rArg1
	def rArg0
	def rArg2
	// succ:  l1
l1:
	rArg1_11 = rArg1 * rArg0
	rArg2_13 = rArg2 + rArg1_11
	return rArg2_13
	// succ:  FpuMultiplyAdd_exit
FpuMultiplyAdd_exit:
";
            #endregion
            RunStringTest(sExp, pb.BuildProgram());
        }

        [Test(Description = "Handle procedure with out parameters")]
        [Category(Categories.UnitTests)]
        public void CrwOutParameter()
        {
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Call("fnOutParam", 0);
                m.MStore(m.Word32(0x00123400), r1);
                m.MStore(m.Word32(0x00123404), r2);
                m.Return();
            });
            pb.Add("fnOutParam", m =>
            {
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");

                m.Label("m0");
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.BranchIf(m.Eq0(r1), "m2");

                m.Label("m1");
                m.Assign(r1, m.IAdd(r1, 3));
                m.Assign(r2, m.ISub(r2, 3));
                m.Return();

                m.Label("m2");
                m.Assign(r1, 0);
                m.Assign(r2, 0);
                m.Return();
            });

            var sExp =
            #region Expected
@"void main(Register word32 r1, Register word32 r2)
// MayUse:  r1:[0..31] r2:[0..31]
// LiveOut:
// Trashed: r1 r2
// Preserved: r63
// main
// Return size: 0
// Mem0:Mem
// fp:fp
// r1:r1
// r2:r2
// r63:r63
// return address size: 0
void main(word32 r1, word32 r2)
main_entry:
	def r1
	def r2
	// succ:  l1
l1:
	r1_5 = fnOutParam(r1, r2, out r2_6)
	Mem7[0x00123400:word32] = r1_5
	Mem8[0x00123404:word32] = r2_6
	return
	// succ:  main_exit
main_exit:
Register word32 fnOutParam(Register word32 r1, Register word32 r2, Register out ptr32 r2Out)
// MayUse:  r1:[0..31] r2:[0..31]
// LiveOut: r1 r2
// Trashed: r1 r2
// Preserved: r63
// fnOutParam
// Return size: 0
// Mem0:Mem
// fp:fp
// r1:r1
// r2:r2
// r63:r63
// r2Out:Out:r2
// return address size: 0
word32 fnOutParam(word32 r1, word32 r2, ptr32 & r2Out)
fnOutParam_entry:
	def r1
	def r2
	// succ:  m0
m0:
	branch r1 == 0x00000000 m2
	// succ:  m1 m2
m1:
	r1_6 = r1 + 0x00000003
	r2_8 = r2 - 0x00000003
	r2Out = r2_8
	return r1_6
	// succ:  fnOutParam_exit
m2:
	r1_4 = 0x00000000
	r2_5 = 0x00000000
	r2Out = r2_5
	return r1_4
	// succ:  fnOutParam_exit
fnOutParam_exit:
";
            #endregion
            RunStringTest(sExp, pb.BuildProgram());
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CrwParameters()
        {
            RunFileTest_x86_real("Fragments/multiple/outparameters.asm", "Analysis/CrwParameters.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwStackArgumentNotFound()
        {
            var ssa = Given_Procedure("main", m =>
            {
                m.Label("body");
                m.Call("fn", 4, new Identifier[] { }, new Identifier[] { });
                m.Return();
            });
            Given_Procedure("fn", m => { });
            Given_Signature(
                "fn",
                FunctionType.Action(
                    new Identifier(
                        "arg04",
                        PrimitiveType.Word32,
                        new StackArgumentStorage(
                            4, PrimitiveType.Word32))));

            When_RewriteCalls(ssa);

            var sExp =
            #region Expected
@"main_entry:
body:
	// Failed to bind call argument.
	// Please report this issue at https://github.com/uxmal/reko
	stackArg4 = <invalid>
	fn(stackArg4)
	return
main_exit:
";
            #endregion
            AssertProcedureCode(sExp, ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwRegisterArgumentNotFound()
        {
            var ret = new RegisterStorage("ret", 1, 0, PrimitiveType.Word32);
            var arg = new RegisterStorage("arg", 2, 0, PrimitiveType.Word32);
            var ssa = Given_Procedure("main", m =>
            {
                m.Label("body");
                m.Call("fn", 4, new Identifier[] { }, new Identifier[] { });
                m.Return();
            });
            Given_Procedure("fn", m => { });
            Given_Signature(
                "fn",
                FunctionType.Func(
                    new Identifier(
                        "ret",
                        PrimitiveType.Word32,
                        ret),
                    new Identifier(
                        "arg",
                        PrimitiveType.Word32,
                        arg)));

            When_RewriteCalls(ssa);

            var sExp =
            #region Expected
@"main_entry:
	def arg
body:
	fn(arg)
	return
main_exit:
";
            #endregion
            AssertProcedureCode(sExp, ssa);
        }

        [Test(Description = "Ignore FPU return if it was not found in call definitions")]
        [Category(Categories.UnitTests)]
        public void CrwFPUReturnNotFound()
        {
            var ret = new FpuStackStorage(0, PrimitiveType.Real64);
            var ssa = Given_Procedure("main", m =>
            {
                m.Label("body");
                m.Call("fn", 4, new Identifier[] { }, new Identifier[] { });
                m.Return();
            });
            Given_Procedure("fn", m => { });
            Given_Signature(
                "fn",
                FunctionType.Func(
                    new Identifier(
                        "ret",
                        ret.DataType,
                        ret)));

            When_RewriteCalls(ssa);

            var sExp =
            #region Expected
@"main_entry:
body:
	fn()
	return
main_exit:
";
            #endregion
            AssertProcedureCode(sExp, ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwExcessRegisterUse()
        {
            var ret = new RegisterStorage("ret", 1, 0, PrimitiveType.Word32);
            var ssa = Given_Procedure("main", m =>
            {
                var a = m.Reg32("a");
                m.Label("body");
                m.Call("fn", 4, new Identifier[] { a }, new Identifier[] { });
                m.Return();
            });
            Given_Procedure("fn", m => { });
            Given_Signature("fn", FunctionType.Action());

            When_RewriteCalls(ssa);

            var sExp =
            #region Expected
@"main_entry:
body:
	fn()
	return
main_exit:
";
            #endregion
            AssertProcedureCode(sExp, ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwExcessRegisterDefinition()
        {
            var ret = new RegisterStorage("ret", 1, 0, PrimitiveType.Word32);
            var ssa = Given_Procedure("main", m =>
            {
                var a = m.Reg32("a");
                m.Label("body");
                m.Call("fn", 4, new Identifier[] { }, new Identifier[] { a });
                m.Return();
            });
            Given_Procedure("fn", m => { });
            Given_Signature("fn", FunctionType.Action());

            When_RewriteCalls(ssa);

            var sExp =
            #region Expected
@"main_entry:
body:
	fn()
	a = <invalid>
	return
main_exit:
";
            #endregion
            AssertProcedureCode(sExp, ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwReturnRegisterNotFound()
        {
            var ret = new RegisterStorage("ret", 1, 0, PrimitiveType.Word32);
            var rOut = new RegisterStorage("out", 1, 0, PrimitiveType.Word32);
            var ssa = Given_Procedure("main", m =>
            {
                m.Label("body");
                m.Return();
            });
            Given_Signature(
                "main",
                FunctionType.Func(
                    new Identifier(
                        "ret",
                        PrimitiveType.Word32,
                        ret),
                    new Identifier(
                        "out",
                        PrimitiveType.Word32,
                        new OutArgumentStorage(
                            ssa.Procedure.Frame.EnsureRegister(rOut)))));

            When_RewriteReturns(ssa);

            var sExp =
            #region Expected
@"main_entry:
body:
	out = <invalid>
	return <invalid>
main_exit:
";
            #endregion
            AssertProcedureCode(sExp, ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwSliceReturnRegister()
        {
            var ret = new RegisterStorage("ret", 1, 0, PrimitiveType.Word32);
            var reth = new RegisterStorage("reth", 1, 16, PrimitiveType.Word16);
            var ssa = Given_Procedure("main", m =>
            {
                var result = m.Reg("ret", ret);
                m.Label("body");
                m.Assign(result, 0x12345678);
                m.AddUseToExitBlock(result);
                m.Return();
            });
            Given_Signature(
                "main",
                FunctionType.Func(
                    new Identifier(
                        "reth",
                        PrimitiveType.Word16,
                        reth)));

            When_RewriteReturns(ssa);

            var sExp =
            #region Expected
@"main_entry:
body:
	ret = 0x12345678
	return SLICE(ret, word16, 16)
main_exit:
	use ret
";
            #endregion
            AssertProcedureCode(sExp, ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwMakeSig_Sequence_InArg()
        {
            flow.BitsUsed.Add(new SequenceStorage(Registers.edx, Registers.eax), new BitRange(0, 64));
            var crw = new CallRewriter(program.Platform, new ProgramDataFlow(), eventListener);
            var sig = crw.MakeSignature(new SsaState(proc), proc.Frame, flow);
            Assert.AreEqual("(fn void (word64))", sig.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CrwIndirectCall()
        {
            var ssa = Given_Procedure("main", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var r2_1 = m.Reg32("r2_1", 2);
                var r3 = m.Reg32("r3", 3);
                m.Label("body");
                m.Call(r1, 0,
                    new[] { r2, r3 },
                    new[] { r2_1 });
                m.MStore(m.Word32(0x00123400), r2_1);
                m.Return();
            });

            When_RewriteCalls(ssa);

            var sExp =
            #region Expected
@"main_entry:
body:
	call r1 (retsize: 0;)
		uses: r2:r2,r3:r3
		defs: r2_1:r2_1
	Mem4[0x00123400:word32] = r2_1
	return
main_exit:
";
            #endregion
            AssertProcedureCode(sExp, ssa);
        }
    }
}

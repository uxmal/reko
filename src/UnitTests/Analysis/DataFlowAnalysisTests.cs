#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Analysis;
using Reko.UnitTests.Mocks;
using Reko.UnitTests.TestCode;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Rhino.Mocks;
using Reko.Core.Types;
using Reko.Core.Expressions;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class DataFlowAnalysisTests : AnalysisTestBase
	{
		private DataFlowAnalysis dfa;
        private MockRepository mr;
        private string CSignature;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            this.CSignature = null;
            this.dfa = null;
            base.platform = null;
        }

        protected override void RunTest(Program program, TextWriter writer)
		{
            SetCSignatures(program);
            IImportResolver importResolver = mr.Stub<IImportResolver>();
            mr.ReplayAll();
			dfa = new DataFlowAnalysis(program, importResolver, new FakeDecompilerEventListener());
			dfa.AnalyzeProgram();
			foreach (Procedure proc in program.Procedures.Values)
			{
				ProcedureFlow flow = dfa.ProgramDataFlow[proc];
				writer.Write("// ");
                var sig = flow.Signature ?? proc.Signature;
                sig.Emit(proc.Name, FunctionType.EmitFlags.ArgumentKind | FunctionType.EmitFlags.LowLevelInfo, writer);
                flow.Emit(program.Architecture, writer);
				proc.Write(false, writer);
				writer.WriteLine();
			}
		}

        private void SetCSignatures(Program program)
        {
            foreach (var addr in program.Procedures.Keys)
            {
                program.User.Procedures.Add(
                    addr,
                    new Procedure_v1
                    {
                        CSignature = this.CSignature
                    });
            }
        }

        protected void Given_CSignature(string CSignature)
        {
            this.CSignature = CSignature;
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaAsciiHex()
		{
			RunFileTest_x86_real("Fragments/ascii_hex.asm", "Analysis/DfaAsciiHex.txt");
		}

		[Test]
        [Ignore("Stack arrays are not supported yet")]
		public void DfaAutoArray32()
		{
			RunFileTest_x86_32("Fragments/autoarray32.asm", "Analysis/DfaAutoArray32.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaFactorial()
		{
			RunFileTest_x86_real("Fragments/factorial.asm", "Analysis/DfaFactorial.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaFactorialReg()
		{
			RunFileTest_x86_real("Fragments/factorial_reg.asm", "Analysis/DfaFactorialReg.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaFibonacci()
		{
			RunFileTest_x86_32("Fragments/multiple/fibonacci.asm", "Analysis/DfaFibonacci.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void DfaFpuOps()
		{
			RunFileTest_x86_real("Fragments/fpuops.asm", "Analysis/DfaFpuOps.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaMutualTest()
		{
			RunFileTest_x86_real("Fragments/multiple/mutual.asm", "Analysis/DfaMutualTest.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void DfaChainTest()
		{
			RunFileTest_x86_real("Fragments/multiple/chaincalls.asm", "Analysis/DfaChainTest.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void DfaGlobalHandle()
		{
            Given_FakeWin32Platform(mr);
            this.platform.Stub(p => p.ResolveImportByName(null, null)).IgnoreArguments().Return(null);
            this.platform.Stub(p => p.DataTypeFromImportName(null)).IgnoreArguments().Return(null);
            mr.ReplayAll();
            RunFileTest_x86_32("Fragments/import32/GlobalHandle.asm", "Analysis/DfaGlobalHandle.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void DfaMoveChain()
		{
			RunFileTest_x86_real("Fragments/move_sequence.asm", "Analysis/DfaMoveChain.txt");
		}

		[Test]
 
        [Category(Categories.AnalysisDevelopment)]
        public void DfaNegsNots()
		{
			RunFileTest_x86_real("Fragments/negsnots.asm", "Analysis/DfaNegsNots.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void DfaPreservedAlias()
		{
			RunFileTest_x86_real("Fragments/multiple/preserved_alias.asm", "Analysis/DfaPreservedAlias.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void DfaReadFile()
        {
			RunFileTest_x86_real("Fragments/multiple/read_file.asm", "Analysis/DfaReadFile.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void DfaStackPointerMessing()
        {
			RunFileTest_x86_real("Fragments/multiple/stackpointermessing.asm", "Analysis/DfaStackPointerMessing.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void DfaStringInstructions()
		{
			RunFileTest_x86_real("Fragments/stringinstr.asm", "Analysis/DfaStringInstructions.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
        public void DfaSuccessiveDecs()
        {
			RunFileTest_x86_real("Fragments/multiple/successivedecs.asm", "Analysis/DfaSuccessiveDecs.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void DfaWhileBigHead()
		{
			RunFileTest_x86_real("Fragments/while_bighead.asm", "Analysis/DfaWhileBigHead.txt");
		}

		[Test]
		public void DfaWhileGoto()
		{
			RunFileTest_x86_real("Fragments/while_goto.asm", "Analysis/DfaWhileGoto.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaRecurseWithPushes()
		{
			RunFileTest_x86_real("Fragments/multiple/recurse_with_pushes.asm", "Analysis/DfaRecurseWithPushes.txt");
		}

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaReg00007()
        {
            RunFileTest_x86_real("Fragments/regressions/r00007.asm", "Analysis/DfaReg00007.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void DfaReg00009()
		{
			RunFileTest_x86_real("Fragments/regressions/r00009.asm", "Analysis/DfaReg00009.txt");
		}

		[Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaReg00010()
		{
			RunFileTest_x86_real("Fragments/regressions/r00010.asm", "Analysis/DfaReg00010.txt");
		}

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.IntegrationTests)]
        public void DfaReg00011()
        {
            RunFileTest_x86_real("Fragments/regressions/r00011.asm", "Analysis/DfaReg00011.txt");
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.IntegrationTests)]
        public void DfaReg00015()
        {
            RunFileTest_x86_real("Fragments/regressions/r00015.asm", "Analysis/DfaReg00015.txt");
        }

        [Test]
        public void DfaFstsw()
        {
           var program = RewriteCodeFragment(@"
                fcomp   dword ptr [bx]
                fstsw   ax
                test    ah,0x41
                jpo     done
                mov     word ptr [si],4
done:   
                ret
");
           SaveRunOutput(program, RunTest, "Analysis/DfaFstsw.txt");
        }

        [Test]
        public void DfaManyIncrements()
        {
            RunFileTest(new ManyIncrements(), "Analysis/DfaManyIncrements.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void DfaReg00001()
        {
            var program = RewriteCodeFragment32(UnitTests.Fragments.Regressions.Reg00001.Text);
            SaveRunOutput(program, RunTest, "Analysis/DfaReg00001.txt");
        }

        [Test]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaReg00282()
        {
            RunFileTest_x86_real("Fragments/regressions/r00282.asm", "Analysis/DfaReg00282.txt");
        }

        [Test]
        [Category(Categories.AnalysisDevelopment)]
        public void DfaReg00316()
        {
            Given_CSignature("long r316(long a)");
            RunFileTest_x86_32("Fragments/regressions/r00316.asm", "Analysis/DfaReg00316.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void DfaCastCast()
        {
            var m = new ProcedureBuilder();
            var r1 = m.Register(1);
            var r2 = m.Register(2);
            m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
            r1.DataType = PrimitiveType.Real32;
            r2.DataType = PrimitiveType.Real32;
            m.Assign(r2, m.Cast(PrimitiveType.Real64, r1));
            m.Store(m.Word32(0x123408), m.Cast(PrimitiveType.Real32, r2));
            m.Return();

            RunFileTest(m, "Analysis/DfaCastCast.txt");
        }

        [Test]
        [Ignore("Fixing this resolves #318")]
        public void Dfa_318_IncrementedSegmentedPointerOffset()
        {
            var sExp =
            #region Expected
@"// void ProcedureBuilder(Register word16 cx, Register word16 ds)
// stackDelta: 0; fpuStackDelta: 0; fpuMaxParam: -1
// MayUse:  cx ds
// LiveOut:
// Trashed: SC bx cx es
// Preserved: r63
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder(word16 cx, word16 ds)
ProcedureBuilder_entry:
	// succ:  l1
l1:
	segptr32 es_bx_2 = Mem0[ds:0x0100:word32]
	// succ:  mHead
mHead:
	Mem8[es_bx_2:byte] = 0x00
	es_bx_4 = es_bx_4 + 0x0001
	cx = cx - 0x0001
	branch cx != 0x0000 mHead
	// succ:  mReturn mHead
mReturn:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunStringTest(sExp, m =>
            {
                var ds = m.Reg16("ds", 8);
                var es = m.Reg16("es", 9);
                var cx = m.Reg16("cx", 1);
                var bx = m.Reg16("bx", 3);
                var es_bx = m.Frame.EnsureSequence(es.Storage, bx.Storage, PrimitiveType.SegPtr32);
                var SZ = m.Flags("SZ");
                var Z = m.Flags("Z");

                m.Assign(es_bx, m.SegMem(PrimitiveType.Word32, ds, m.Word16(0x100)));

                m.Label("mHead");
                m.SegStore(es, bx, m.Byte(0));
                m.Assign(bx, m.IAdd(bx, 1));
                m.Assign(cx, m.ISub(cx, 1));
                m.Assign(SZ, m.Cond(cx));
                m.BranchIf(m.Test(ConditionCode.NE, Z), "mHead");

                m.Label("mReturn");
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void DfaUnsignedDiv()
        {
            var m = new ProcedureBuilder();
            var r1 = m.Register(1);
            var r2 = m.Register(2);
            var r2_r1 = m.Frame.EnsureSequence(r2.Storage, r1.Storage, PrimitiveType.Word64);
            var tmp = m.Frame.CreateTemporary(r2_r1.DataType);

            m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
            m.Assign(r1, m.LoadDw(m.Word32(0x123400)));
            m.Assign(r2_r1, m.Seq(m.Word32(0), r1));
            m.Assign(tmp, r2_r1);
            m.Assign(r1, m.UDiv(tmp, m.Word32(42)));
            m.Store(m.Word32(0x123404), r1);
            m.Return();

            RunFileTest(m, "Analysis/DfaUnsignedDiv.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void DfaFpuStackReturn()
        {
            RunFileTest_x86_real("Fragments/fpustackreturn.asm", "Analysis/DfaFpuStackReturn.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void DfaJumpIntoProc3()
        {
            RunFileTest_x86_32("Fragments/multiple/jumpintoproc3.asm", "Analysis/DfaJumpIntoProc3.txt");
        }
    }
}

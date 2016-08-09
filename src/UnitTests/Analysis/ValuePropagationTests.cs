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
using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Core.Machine;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Rhino.Mocks;
using System.Diagnostics;
using System.Collections.Generic;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class ValuePropagationTests : AnalysisTestBase
	{
		SsaIdentifierCollection ssaIds;
        private MockRepository mr;
        private IProcessorArchitecture arch;
        private IImportResolver importResolver;

        [SetUp]
		public void Setup()
		{
			ssaIds = new SsaIdentifierCollection();
            mr = new MockRepository();
            arch = mr.Stub<IProcessorArchitecture>();
            importResolver = mr.Stub<IImportResolver>();
		}

        private Identifier Reg32(string name)
        {
            var mr = new RegisterStorage(name, ssaIds.Count, 0, PrimitiveType.Word32);
            Identifier id = new Identifier(mr.Name, mr.DataType, mr);
            SsaIdentifier sid = new SsaIdentifier(id, id, null, null, false);
            ssaIds.Add(id, sid);
            return sid.Identifier;
        }

        private Identifier Reg16(string name)
        {
            var mr = new RegisterStorage(name, ssaIds.Count, 0, PrimitiveType.Word16);
            Identifier id = new Identifier(mr.Name, mr.DataType, mr);
            SsaIdentifier sid = new SsaIdentifier(id, id, null, null, false);
            ssaIds.Add(id, sid);
            return sid.Identifier;
        }


        private Identifier Reg8(string name)
        {
            var mr = new RegisterStorage(name, ssaIds.Count, 0, PrimitiveType.Byte);
            Identifier id = new Identifier(mr.Name, mr.DataType, mr);
            SsaIdentifier sid = new SsaIdentifier(id, id, null, null, false);
            ssaIds.Add(id, sid);
            return sid.Identifier;
        }

        private ExternalProcedure CreateExternalProcedure(string name, Identifier ret, params Identifier[] parameters)
        {
            var ep = new ExternalProcedure(name, new FunctionType(null, ret, parameters));
            ep.Signature.ReturnAddressOnStack = 4;
            return ep;
        }

        private Identifier RegArg(int n, string name)
        {
            return new Identifier(
                name,
                PrimitiveType.Word32,
                new RegisterStorage(name, n, 0, PrimitiveType.Word32));
        }

        private Identifier StackArg(int offset)
        {
            return new Identifier(
                string.Format("arg{0:X2}", offset),
                PrimitiveType.Word32,
                new StackArgumentStorage(offset, PrimitiveType.Word32));
        }

		protected override void RunTest(Program program, TextWriter writer)
		{
			var dfa = new DataFlowAnalysis(program, null, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in program.Procedures.Values)
			{
				writer.WriteLine("= {0} ========================", proc.Name);
                SsaTransform2 sst = new SsaTransform2(program.Architecture, proc, null, dfa.ProgramDataFlow.ToDataFlow2());
                sst.Transform();
				SsaState ssa = sst.SsaState;
                var cce = new ConditionCodeEliminator(ssa, program.Platform);
                cce.Transform();
				ssa.Write(writer);
				proc.Write(false, writer);
				writer.WriteLine();

                ValuePropagator vp = new ValuePropagator(program.Architecture, ssa);
				vp.Transform();

				ssa.Write(writer);
				proc.Write(false, writer);
			}
		}

        private void AssertStringsEqual(string sExp, SsaState ssa)
        {
            var sw = new StringWriter();
            ssa.Write(sw);
            ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.Print("{0}", sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

		[Test]
		public void VpChainTest()
		{
			RunFileTest_x86_real("Fragments/multiple/chaincalls.asm", "Analysis/VpChainTest.txt");
		}

		[Test]
		public void VpConstPropagation()
		{
			RunFileTest_x86_real("Fragments/constpropagation.asm", "Analysis/VpConstPropagation.txt");
		}

		[Test]
		public void VpGlobalHandle()
		{
            Given_FakeWin32Platform(mr);
            this.platform.Stub(p => p.LookupGlobalByName(null, null)).IgnoreArguments().Return(null);
            mr.ReplayAll();
			RunFileTest32("Fragments/import32/GlobalHandle.asm", "Analysis/VpGlobalHandle.txt");
		}

		[Test]
		public void VpNegsNots()
		{
			RunFileTest_x86_real("Fragments/negsnots.asm", "Analysis/VpNegsNots.txt");
		}

		[Test]
		public void VpNestedRepeats()
		{
			RunFileTest_x86_real("Fragments/nested_repeats.asm", "Analysis/VpNestedRepeats.txt");
		}

		[Test]
		public void VpStringInstructions()
		{
			RunFileTest_x86_real("Fragments/stringinstr.asm", "Analysis/VpStringInstructions.txt");
		}

		[Test]
		public void VpSuccessiveDecs()
		{
			RunFileTest_x86_real("Fragments/multiple/successivedecs.asm", "Analysis/VpSuccessiveDecs.txt");
		}

		[Test]
		public void VpWhileGoto()
		{
			RunFileTest_x86_real("Fragments/while_goto.asm", "Analysis/VpWhileGoto.txt");
		}

        [Test][Ignore("Very complex interaction with SSA")]
        [Category("investigation")]
        public void VpReg00011()
        {
            RunFileTest_x86_real("Fragments/regressions/r00011.asm", "Analysis/VpReg00011.txt");
        }

        [Test]
        public void VpLoop()
        {
            var b = new ProgramBuilder();
            b.Add("main", m =>
            {
                var r = m.Reg32("r0", 0);
                var zf = m.Flags("Z");
                m.Label("l0000");
                m.Store(r, 0);
                m.Assign(r, m.ISub(r, 4));
                m.Assign(m.Flags("Z"), m.Cond(r));
                m.BranchCc(ConditionCode.NE, "l0000");

                m.Label("l0001");
                m.Assign(r, 42);

                m.Label("l0002");
                m.Store(r, 12);
                m.Assign(r, m.ISub(r, 4));
                m.BranchIf(m.Eq0(r), "l0002");

                m.Return();
            });
            RunFileTest(b.BuildProgram(), "Analysis/VpLoop.txt");
        }

		[Test]
		public void VpDbp()
		{
			Procedure proc = new DpbMock().Procedure;
			var gr = proc.CreateBlockDominatorGraph();
			SsaTransform sst = new SsaTransform(new ProgramDataFlow(), proc,  null, gr,
                new HashSet<RegisterStorage>());
			SsaState ssa = sst.SsaState;

            ssa.DebugDump(true);

            ValuePropagator vp = new ValuePropagator(arch, ssa);
			vp.Transform();

			using (FileUnitTester fut = new FileUnitTester("Analysis/VpDbp.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void VpEquality()
		{
			Identifier foo = Reg32("foo");

            var vp = CreatePropagatorWithDummyStatement();
			BinaryExpression expr = 
				new BinaryExpression(Operator.Eq, PrimitiveType.Bool, 
				new BinaryExpression(Operator.ISub, PrimitiveType.Word32, foo,
				Constant.Word32(1)),
				Constant.Word32(0));
			Assert.AreEqual("foo - 0x00000001 == 0x00000000", expr.ToString());

			Expression simpler = vp.VisitBinaryExpression(expr);
			Assert.AreEqual("foo == 0x00000001", simpler.ToString());
		}

        private ExpressionSimplifier CreatePropagatorWithDummyStatement()
        {
            var ctx = new SsaEvaluationContext(arch, ssaIds);
            ctx.Statement = new Statement(0, new SideEffect(Constant.Word32(32)), null);
            return new ExpressionSimplifier(ctx);
        }

		[Test]
		public void VpAddZero()
		{
			Identifier r = Reg32("r");
			Identifier s = Reg32("s");

            var sub = new BinaryExpression(Operator.ISub, PrimitiveType.Word32, new MemoryAccess(MemoryIdentifier.GlobalMemory, r, PrimitiveType.Word32), Constant.Word32(0));
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
			var exp = sub.Accept(vp);
			Assert.AreEqual("Mem0[r:word32]", exp.ToString());
		}

		[Test]
		public void VpEquality2()
		{
            // Makes sure that 
            // y = x - 2
            // if (y == 0) ...
            // doesn't get munged into
            // y = x - 2
            // if (x == 2)

            ProcedureBuilder m = new ProcedureBuilder();
			Identifier x = m.Reg32("x", 0);
			Identifier y = m.Reg32("y", 1);
            m.Assign(x, m.LoadDw(Constant.Word32(0x1000300)));
            m.Assign(y, m.ISub(x, 2));
			m.BranchIf(m.Eq(y, 0), "test");
            m.Return();
            m.Label("test");
            m.Return();
            var importResolver = mr.Stub<IImportResolver>();
            importResolver.Replay();
            var sst = new SsaTransform2(m.Architecture, m.Procedure, importResolver, new DataFlow2());
            sst.Transform();

            var vp = new ValuePropagator(arch, sst.SsaState);
            var stm = m.Procedure.EntryBlock.Succ[0].Statements.Last;
			vp.Transform(stm);
			Assert.AreEqual("branch x_2 == 0x00000002 test", stm.Instruction.ToString());
		}

		[Test]
		public void VpCopyPropagate()
		{
            var m = new ProcedureBuilder();
			Identifier x = m.Reg32("x", 0);
			Identifier y = m.Reg32("y", 1);
			Identifier z = m.Reg32("z", 2);
			Identifier w = m.Reg32("w", 3);
            m.Assign(x, m.LoadDw(m.Word32(0x10004000)));
            m.Assign(y, x);
            m.Assign(z, m.IAdd(y, 2));
            m.Assign(w, y);

            var importResolver = mr.Stub<IImportResolver>();
            importResolver.Replay();

            var sst = new SsaTransform2(m.Architecture, m.Procedure, importResolver, new DataFlow2());
            sst.Transform();
            ssaIds = sst.SsaState.Identifiers;
            var stms = m.Procedure.EntryBlock.Succ[0].Statements;
			Assert.AreEqual("x_2 = Mem0[0x10004000:word32]", stms[0].ToString());
			Assert.AreEqual("y_3 = x_2", stms[1].ToString());
			Assert.AreEqual("z_4 = y_3 + 0x00000002", stms[2].ToString());
			Assert.AreEqual("w_5 = y_3", stms[3].ToString());
            //Debug.Print("{0}", string.Join(", ", ssaIds.Single(i => i.Identifier.Name == "x_1").Uses));
            //Debug.Print("{0}", string.Join(", ", ssaIds.Single(i => i.Identifier.Name == "y_2").Uses));
            //Debug.Print("{0}", string.Join(", ", ssaIds.Single(i => i.Identifier.Name == "z_3").Uses));
            //Debug.Print("{0}", string.Join(", ", ssaIds.Single(i => i.Identifier.Name == "w_4").Uses));


            ValuePropagator vp = new ValuePropagator(arch, sst.SsaState);
            stms.ForEach(s => vp.Transform(s));

			Assert.AreEqual("x_2 = Mem0[0x10004000:word32]", stms[0].ToString());
			Assert.AreEqual("y_3 = x_2", stms[1].ToString());
			Assert.AreEqual("z_4 = x_2 + 0x00000002", stms[2].ToString());
			Assert.AreEqual("w_5 = x_2", stms[3].ToString());

            Assert.AreEqual(3, ssaIds.Single(i => i.Identifier.Name == "x_2").Uses.Count);
			Assert.AreEqual(0, ssaIds.Single(i => i.Identifier.Name == "y_3").Uses.Count);
		}

		[Test]
		public void VpSliceConstant()
		{
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
            Expression c = new Slice(PrimitiveType.Byte, Constant.Word32(0x10FF), 0).Accept(vp);
			Assert.AreEqual("0xFF", c.ToString());
		}

		[Test]
		public void VpNegSub()
		{
			Identifier x = Reg32("x");
			Identifier y = Reg32("y");
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
			Expression e = vp.VisitUnaryExpression(
				new UnaryExpression(Operator.Neg, PrimitiveType.Word32, new BinaryExpression(
				Operator.ISub, PrimitiveType.Word32, x, y)));
			Assert.AreEqual("y - x", e.ToString());
		}

		/// <summary>
		/// (<< (+ (* id c1) id) c2))
		/// </summary>
		[Test] 
		public void VpMulAddShift()
		{
			Identifier id = Reg32("id");
			Identifier x =  Reg32("x");
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
			PrimitiveType t = PrimitiveType.Int32;
			BinaryExpression b = new BinaryExpression(Operator.Shl, t, 
				new BinaryExpression(Operator.IAdd, t, 
					new BinaryExpression(Operator.SMul, t, id, Constant.Create(t, 4)),
					id),
				Constant.Create(t, 2));
			Expression e = vp.VisitBinaryExpression(b);
			Assert.AreEqual("id *s 20", e.ToString());
		}

		[Test]
		public void VpShiftShift()
		{
			Identifier id = Reg32("id");
			ProcedureBuilder m = new ProcedureBuilder();
			Expression e = m.Shl(m.Shl(id, 1), 4);
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
			e = e.Accept(vp);
			Assert.AreEqual("id << 0x05", e.ToString());
		}

		[Test]
		public void VpShiftSum()
		{
			Constant c = Constant.Word32(1);
			ProcedureBuilder m = new ProcedureBuilder();
			Expression e = m.Shl(1, m.ISub(Constant.Byte(32), 1));
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
			e = e.Accept(vp);
			Assert.AreEqual("0x80000000", e.ToString());
		}

		[Test]
		public void VpSequenceOfConstants()
		{
			Constant pre = Constant.Word16(0x0001);
			Constant fix = Constant.Word16(0x0002);
			Expression e = new MkSequence(PrimitiveType.Word32, pre, fix);
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
			e = e.Accept(vp);
			Assert.AreEqual("0x00010002", e.ToString());
		}

        [Test]
        public void SliceShift()
        {
            Constant eight = Constant.Word16(8);
            Constant ate = Constant.Word32(8);
            Identifier C = Reg8("C");
            Identifier ax = Reg16("ax");
            Expression e = new Slice(PrimitiveType.Byte, new BinaryExpression(Operator.Shl, PrimitiveType.Word16, C, eight), 8);
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
            e = e.Accept(vp);
            Assert.AreEqual("C", e.ToString());
        }

        [Test]
        public void VpMkSequenceToAddress()
        {
            Constant seg = Constant.Create(PrimitiveType.SegmentSelector, 0x4711);
            Constant off = Constant.Word16(0x4111);
            arch.Expect(a => a.MakeSegmentedAddress(seg, off)).Return(Address.SegPtr(0x4711, 0x4111));
            mr.ReplayAll();

            Expression e = new MkSequence(PrimitiveType.Word32, seg, off);
            var vp = new ExpressionSimplifier(new SsaEvaluationContext(arch, ssaIds));
            e = e.Accept(vp);
            Assert.IsInstanceOf(typeof(Address), e);
            Assert.AreEqual("4711:4111", e.ToString());

            mr.VerifyAll();
        }

        [Test]
        [Ignore("Making this pass breaks a lot of older unit tests. Re-enable once transition to new ProcedureFlow is complete.")]
        public void VpPhiWithConstants()
        {
            Constant c1 = Constant.Word16(0x4711);
            Constant c2 = Constant.Word16(0x4711);
            Identifier r1 = Reg16("r1");
            Identifier r2 = Reg16("r2");
            Identifier r3 = Reg16("r3");
            var stm1 = new Statement(1, new Assignment(r1, c1), null);
            var stm2 = new Statement(2, new Assignment(r2, c2), null);
            ssaIds[r1].DefStatement = stm1;
            ssaIds[r2].DefStatement = stm2;
            var vp = new ValuePropagator(arch, null);
            Instruction instr = new PhiAssignment(r3, new PhiFunction(r1.DataType, r1, r2));
            instr = instr.Accept(vp);
            Assert.AreEqual("r3 = 0x4711", instr.ToString());
        }

		private class DpbMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				var dl = LocalByte("dl");
				var dx = Local16("dx");
				var edx = Local32("edx");

				Assign(edx, Int32(0x0AAA00AA));
				Assign(edx, Dpb(edx, Int8(0x55), 8));
				Store(Int32(0x1000000), edx);

				Assign(edx, Int32(0));
                Assign(edx, Dpb(edx, dl, 0));
				Return(edx);
			}
		}

        [Test]
        public void VpDbpDbp()
        {
            var m = new ProcedureBuilder();
            var d1 = m.Reg32("d32",0);
            var a1 = m.Reg32("a32",1);
            var tmp = m.Frame.CreateTemporary(PrimitiveType.Word16);

            m.Assign(d1, m.Dpb(d1, m.LoadW(a1), 0));
            m.Assign(d1, m.Dpb(d1, m.LoadW(m.IAdd(a1, 4)), 0));

			Procedure proc = m.Procedure;
			var gr = proc.CreateBlockDominatorGraph();
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();
			var sst = new SsaTransform(new ProgramDataFlow(), proc, importResolver, gr, new HashSet<RegisterStorage>());
			var ssa = sst.SsaState;

			var vp = new ValuePropagator(arch, ssa);
			vp.Transform();

			using (FileUnitTester fut = new FileUnitTester("Analysis/VpDpbDpb.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
				fut.AssertFilesEqual();
			}
		}

        private SsaState RunTest(ProcedureBuilder m)
        {
            var proc = m.Procedure;
            var gr = proc.CreateBlockDominatorGraph();
            var sst = new SsaTransform(new ProgramDataFlow(), proc, importResolver, gr, new HashSet<RegisterStorage>());
            var ssa = sst.SsaState;

            var vp = new ValuePropagator(arch, ssa);
            vp.Transform();
            return ssa;
        }

        [Test(Description = "Casting a DPB should result in the deposited bits.")]
        public void VpLoadDpb()
        {
            var m = new ProcedureBuilder();
            var a2 = m.Reg32("a2", 10);
            var d3 = m.Reg32("d3", 3);
            var tmp = m.Temp(PrimitiveType.Byte, "tmp");

            m.Assign(tmp, m.LoadB(a2));
            m.Assign(d3, m.Dpb(d3, tmp, 0));
            m.Store(m.IAdd(a2, 4), m.Cast(PrimitiveType.Byte, d3));

            SsaState ssa = RunTest(m);

            var sExp =
            #region Expected
@"a2:a2
    def:  def a2
    uses: tmp_3 = Mem0[a2:byte]
          Mem6[a2 + 0x00000004:byte] = tmp_3
Mem0:Global memory
    def:  def Mem0
    uses: tmp_3 = Mem0[a2:byte]
tmp_3: orig: tmp
    def:  tmp_3 = Mem0[a2:byte]
    uses: d3_5 = DPB(d3, tmp_3, 0)
          Mem6[a2 + 0x00000004:byte] = tmp_3
d3:d3
    def:  def d3
    uses: d3_5 = DPB(d3, tmp_3, 0)
d3_5: orig: d3
    def:  d3_5 = DPB(d3, tmp_3, 0)
Mem6: orig: Mem0
    def:  Mem6[a2 + 0x00000004:byte] = tmp_3
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def a2
	def Mem0
	def d3
	// succ:  l1
l1:
	tmp_3 = Mem0[a2:byte]
	d3_5 = DPB(d3, tmp_3, 0)
	Mem6[a2 + 0x00000004:byte] = tmp_3
ProcedureBuilder_exit:
";
            #endregion

            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void VpLoadDpbSmallerCast()
        {
            var m = new ProcedureBuilder();
            var a2 = m.Reg32("a2", 10);
            var d3 = m.Reg32("d3", 3);
            var tmp = m.Temp(PrimitiveType.Word16, "tmp");

            m.Assign(tmp, m.LoadW(a2));
            m.Assign(d3, m.Dpb(d3, tmp, 0));
            m.Store(m.IAdd(a2, 4), m.Cast(PrimitiveType.Byte, d3));

            SsaState ssa = RunTest(m);

            var sExp =
            #region Expected
@"a2:a2
    def:  def a2
    uses: tmp_3 = Mem0[a2:word16]
          Mem6[a2 + 0x00000004:byte] = (byte) tmp_3
Mem0:Global memory
    def:  def Mem0
    uses: tmp_3 = Mem0[a2:word16]
tmp_3: orig: tmp
    def:  tmp_3 = Mem0[a2:word16]
    uses: d3_5 = DPB(d3, tmp_3, 0)
          Mem6[a2 + 0x00000004:byte] = (byte) tmp_3
d3:d3
    def:  def d3
    uses: d3_5 = DPB(d3, tmp_3, 0)
d3_5: orig: d3
    def:  d3_5 = DPB(d3, tmp_3, 0)
Mem6: orig: Mem0
    def:  Mem6[a2 + 0x00000004:byte] = (byte) tmp_3
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def a2
	def Mem0
	def d3
	// succ:  l1
l1:
	tmp_3 = Mem0[a2:word16]
	d3_5 = DPB(d3, tmp_3, 0)
	Mem6[a2 + 0x00000004:byte] = (byte) tmp_3
ProcedureBuilder_exit:
";
            #endregion

            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void VpCastRealConstant()
        {
            var m = new ProcedureBuilder();
            var r1 = m.Reg32("r1", 1);

            m.Assign(r1, m.Cast(PrimitiveType.Real32, ConstantReal.Real64(1)));

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r1_1: orig: r1
    def:  r1_1 = 1.0F
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	// succ:  l1
l1:
	r1_1 = 1.0F
ProcedureBuilder_exit:
";
            #endregion

            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void VpUndoUnnecessarySlicingOfSegmentPointer()
        {
            var m = new ProcedureBuilder();
            var es = m.Reg16("es", 1);
            var bx = m.Reg16("bx", 3);
            var es_bx = m.Frame.EnsureSequence(es.Storage, bx.Storage, PrimitiveType.Word32);

            m.Assign(es_bx, m.SegMem(PrimitiveType.Word32, es, bx));
            m.Assign(es, m.Slice(PrimitiveType.Word16, es_bx, 16));
            m.Assign(bx, m.Cast(PrimitiveType.Word16, es_bx));
            m.SegStore(es, m.IAdd(bx, 4), m.Byte(3));

            var ssa = RunTest(m);

            var sExp =
            #region Expected
@"es:es
    def:  def es
    uses: es_bx_4 = Mem0[es:bx:word32]
bx:bx
    def:  def bx
    uses: es_bx_4 = Mem0[es:bx:word32]
Mem0:Global memory
    def:  def Mem0
    uses: es_bx_4 = Mem0[es:bx:word32]
es_bx_4: orig: es_bx
    def:  es_bx_4 = Mem0[es:bx:word32]
    uses: es_5 = SLICE(es_bx_4, word16, 16)
          bx_6 = (word16) es_bx_4
          Mem0[es_bx_4 + 0x0004:byte] = 0x03
es_5: orig: es
    def:  es_5 = SLICE(es_bx_4, word16, 16)
bx_6: orig: bx
    def:  bx_6 = (word16) es_bx_4
Mem7: orig: Mem0
    def:  Mem0[es_bx_4 + 0x0004:byte] = 0x03
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def es
	def bx
	def Mem0
	// succ:  l1
l1:
	es_bx_4 = Mem0[es:bx:word32]
	es_5 = SLICE(es_bx_4, word16, 16)
	bx_6 = (word16) es_bx_4
	Mem0[es_bx_4 + 0x0004:byte] = 0x03
ProcedureBuilder_exit:
";
            #endregion

            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void VpMulBy6()
        {
            var m = new ProcedureBuilder();
            var r1 = m.Reg16("r1", 1);
            var r2 = m.Reg16("r1", 2);

            m.Assign(r2, r1);                 // r1
            m.Assign(r1, m.Shl(r1, 1));       // r1 * 2
            m.Assign(r1, m.IAdd(r1, r2));     // r1 * 3
            m.Assign(r1, m.Shl(r1, 1));       // r1 * 6

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r1:r1
    def:  def r1
    uses: r1_2 = r1
          r1_3 = r1 << 0x01
          r1_4 = r1 * 0x0003
          r1_5 = r1 * 0x0006
r1_2: orig: r1
    def:  r1_2 = r1
r1_3: orig: r1
    def:  r1_3 = r1 << 0x01
r1_4: orig: r1
    def:  r1_4 = r1 * 0x0003
r1_5: orig: r1
    def:  r1_5 = r1 * 0x0006
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r1
	// succ:  l1
l1:
	r1_2 = r1
	r1_3 = r1 << 0x01
	r1_4 = r1 * 0x0003
	r1_5 = r1 * 0x0006
ProcedureBuilder_exit:
";
            #endregion

            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void VpIndirectCall()
        {
            var callee = CreateExternalProcedure("foo", RegArg(1, "r1"), StackArg(4), StackArg(8));
            var pc = new ProcedureConstant(PrimitiveType.Pointer32, callee);

            var m = new ProcedureBuilder();
            var r1 = m.Reg32("r1", 1);
            var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
            m.Assign(r1, pc);
            m.Assign(sp, m.ISub(sp, 4));
            m.Store(sp, 3);
            m.Assign(sp, m.ISub(sp, 4));
            m.Store(sp, m.LoadW(m.Word32(0x1231230)));
            m.Call(r1, 4);
            m.Return();

            arch.Stub(a => a.CreateStackAccess(null, 0, null))
                .IgnoreArguments()
                .Do(new Func<Frame, int, DataType, Expression>((f, off, dt) => m.Load(dt, m.IAdd(sp, off))));
            mr.ReplayAll();

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r1_1: orig: r1
    def:  r1_1 = foo
r63:r63
    def:  def r63
    uses: r63_3 = r63 - 0x00000004
          Mem4[r63 - 0x00000004:word32] = 0x00000003
          r63_5 = r63 - 0x00000008
          Mem6[r63 - 0x00000008:word16] = Mem4[0x01231230:word16]
r63_3: orig: r63
    def:  r63_3 = r63 - 0x00000004
Mem4: orig: Mem0
    def:  Mem4[r63 - 0x00000004:word32] = 0x00000003
    uses: Mem6[r63 - 0x00000008:word16] = Mem4[0x01231230:word16]
r63_5: orig: r63
    def:  r63_5 = r63 - 0x00000008
Mem6: orig: Mem0
    def:  Mem6[r63 - 0x00000008:word16] = Mem4[0x01231230:word16]
r1_7: orig: r1
    def:  r1_7 = foo(Mem0[r63:word32], Mem0[r63 + 0x00000004:word32])
r63_8: orig: r63
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r63
	// succ:  l1
l1:
	r1_1 = foo
	r63_3 = r63 - 0x00000004
	Mem4[r63 - 0x00000004:word32] = 0x00000003
	r63_5 = r63 - 0x00000008
	Mem6[r63 - 0x00000008:word16] = Mem4[0x01231230:word16]
	r1_7 = foo(Mem0[r63:word32], Mem0[r63 + 0x00000004:word32])
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion
                AssertStringsEqual(sExp, ssa);
        }
    }
}

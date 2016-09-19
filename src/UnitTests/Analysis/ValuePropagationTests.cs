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
            var ep = new ExternalProcedure(name, new FunctionType(ret, parameters));
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

        protected override void RunTest(Program prog, TextWriter writer)
		{
			var dfa = new DataFlowAnalysis(prog, importResolver, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				writer.WriteLine("= {0} ========================", proc.Name);
				var gr = proc.CreateBlockDominatorGraph();
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
                SsaTransform sst = new SsaTransform(dfa.ProgramDataFlow, proc, importResolver, gr,
                    new HashSet<RegisterStorage>());
				SsaState ssa = sst.SsaState;
                var cce = new ConditionCodeEliminator(ssa, prog.Platform);
                cce.Transform();
				ssa.Write(writer);
				proc.Write(false, writer);
				writer.WriteLine();

				ValuePropagator vp = new ValuePropagator(prog.Architecture, ssa);
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
			RunFileTest("Fragments/multiple/chaincalls.asm", "Analysis/VpChainTest.txt");
		}

		[Test]
		public void VpConstPropagation()
		{
			RunFileTest("Fragments/constpropagation.asm", "Analysis/VpConstPropagation.txt");
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
			RunFileTest("Fragments/negsnots.asm", "Analysis/VpNegsNots.txt");
		}

		[Test]
		public void VpNestedRepeats()
		{
			RunFileTest("Fragments/nested_repeats.asm", "Analysis/VpNestedRepeats.txt");
		}

		[Test]
		public void VpStringInstructions()
		{
			RunFileTest("Fragments/stringinstr.asm", "Analysis/VpStringInstructions.txt");
		}

		[Test]
		public void VpSuccessiveDecs()
		{
			RunFileTest("Fragments/multiple/successivedecs.asm", "Analysis/VpSuccessiveDecs.txt");
		}

		[Test]
		public void VpWhileGoto()
		{
			RunFileTest("Fragments/while_goto.asm", "Analysis/VpWhileGoto.txt");
		}

        [Test]
        public void VpReg00011()
        {
            RunFileTest("Fragments/regressions/r00011.asm", "Analysis/VpReg00011.txt");
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
                m.Assign(zf, m.Cond(r));
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
            var ssa = new SsaState(m.Procedure, null);
            this.ssaIds = ssa.Identifiers;
            Identifier x = Reg32("x");
			Identifier y = Reg32("y");
            var stmX = m.Assign(x, m.LoadDw(Constant.Word32(0x1000300)));
			ssaIds[x].DefStatement = m.Block.Statements.Last;
            var stmY = m.Assign(y, m.ISub(x, 2));
			ssaIds[y].DefStatement = m.Block.Statements.Last;
			var stm = m.BranchIf(m.Eq(y, 0), "test");
			Assert.AreEqual("x = Mem0[0x01000300:word32]", stmX.ToString());
			Assert.AreEqual("y = x - 0x00000002", stmY.ToString());
			Assert.AreEqual("branch y == 0x00000000 test", stm.ToString());

			var vp = new ValuePropagator(arch, ssa);
			vp.Transform(stm);
			Assert.AreEqual("branch x == 0x00000002 test", stm.Instruction.ToString());
		}

		[Test]
		public void VpCopyPropagate()
		{
            var ssa = new SsaState(new Procedure("foo", new Frame(PrimitiveType.Pointer32)), null);
            ssaIds = ssa.Identifiers;
			Identifier x = Reg32("x");
			Identifier y = Reg32("y");
			Identifier z = Reg32("z");
			Identifier w = Reg32("w");
			Statement stmX = new Statement(0, new Assignment(x, new MemoryAccess(MemoryIdentifier.GlobalMemory, Constant.Word32(0x10004000), PrimitiveType.Word32)), null);
			Statement stmY = new Statement(1, new Assignment(y, x), null);
			Statement stmZ = new Statement(2, new Assignment(z, new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, y, Constant.Word32(2))), null);
			Statement stmW = new Statement(3, new Assignment(w, y), null);
			ssaIds[x].DefStatement = stmX;
			ssaIds[y].DefStatement = stmY;
			ssaIds[z].DefStatement = stmZ;
			ssaIds[w].DefStatement = stmW;
			ssaIds[x].Uses.Add(stmY);
			ssaIds[y].Uses.Add(stmZ);
			ssaIds[y].Uses.Add(stmW);
			Assert.AreEqual("x = Mem0[0x10004000:word32]", stmX.Instruction.ToString());
			Assert.AreEqual("y = x", stmY.Instruction.ToString());
			Assert.AreEqual("z = y + 0x00000002", stmZ.Instruction.ToString());
			Assert.AreEqual("w = y", stmW.Instruction.ToString());

			ValuePropagator vp = new ValuePropagator(arch, ssa);
			vp.Transform(stmX);
			vp.Transform(stmY);
			vp.Transform(stmZ);
			vp.Transform(stmW);

			Assert.AreEqual("x = Mem0[0x10004000:word32]", stmX.Instruction.ToString());
			Assert.AreEqual("y = x", stmY.Instruction.ToString());
			Assert.AreEqual("z = x + 0x00000002", stmZ.Instruction.ToString());
			Assert.AreEqual("w = x", stmW.Instruction.ToString());
			Assert.AreEqual(3, ssaIds[x].Uses.Count);
			Assert.AreEqual(0, ssaIds[y].Uses.Count);
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
            Identifier C = Reg8("C");
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
				Local16("dx");
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
    uses: tmp_2 = Mem0[a2:byte]
          Mem5[a2 + 0x00000004:byte] = tmp_2
Mem0:Global memory
    def:  def Mem0
    uses: tmp_2 = Mem0[a2:byte]
tmp_2: orig: tmp
    def:  tmp_2 = Mem0[a2:byte]
    uses: d3_4 = DPB(d3, tmp_2, 0)
          Mem5[a2 + 0x00000004:byte] = tmp_2
d3:d3
    def:  def d3
    uses: d3_4 = DPB(d3, tmp_2, 0)
d3_4: orig: d3
    def:  d3_4 = DPB(d3, tmp_2, 0)
Mem5: orig: Mem0
    def:  Mem5[a2 + 0x00000004:byte] = tmp_2
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def a2
	def Mem0
	def d3
	// succ:  l1
l1:
	tmp_2 = Mem0[a2:byte]
	d3_4 = DPB(d3, tmp_2, 0)
	Mem5[a2 + 0x00000004:byte] = tmp_2
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
    uses: tmp_2 = Mem0[a2:word16]
          Mem5[a2 + 0x00000004:byte] = (byte) tmp_2
Mem0:Global memory
    def:  def Mem0
    uses: tmp_2 = Mem0[a2:word16]
tmp_2: orig: tmp
    def:  tmp_2 = Mem0[a2:word16]
    uses: d3_4 = DPB(d3, tmp_2, 0)
          Mem5[a2 + 0x00000004:byte] = (byte) tmp_2
d3:d3
    def:  def d3
    uses: d3_4 = DPB(d3, tmp_2, 0)
d3_4: orig: d3
    def:  d3_4 = DPB(d3, tmp_2, 0)
Mem5: orig: Mem0
    def:  Mem5[a2 + 0x00000004:byte] = (byte) tmp_2
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def a2
	def Mem0
	def d3
	// succ:  l1
l1:
	tmp_2 = Mem0[a2:word16]
	d3_4 = DPB(d3, tmp_2, 0)
	Mem5[a2 + 0x00000004:byte] = (byte) tmp_2
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
@"r1_0: orig: r1
    def:  r1_0 = 1.0F
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	// succ:  l1
l1:
	r1_0 = 1.0F
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
    uses: es_bx_3 = Mem0[es:bx:word32]
bx:bx
    def:  def bx
    uses: es_bx_3 = Mem0[es:bx:word32]
Mem0:Global memory
    def:  def Mem0
    uses: es_bx_3 = Mem0[es:bx:word32]
es_bx_3: orig: es_bx
    def:  es_bx_3 = Mem0[es:bx:word32]
    uses: es_4 = SLICE(es_bx_3, word16, 16)
          bx_5 = (word16) es_bx_3
          Mem0[es_bx_3 + 0x0004:byte] = 0x03
es_4: orig: es
    def:  es_4 = SLICE(es_bx_3, word16, 16)
bx_5: orig: bx
    def:  bx_5 = (word16) es_bx_3
Mem6: orig: Mem0
    def:  Mem0[es_bx_3 + 0x0004:byte] = 0x03
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def es
	def bx
	def Mem0
	// succ:  l1
l1:
	es_bx_3 = Mem0[es:bx:word32]
	es_4 = SLICE(es_bx_3, word16, 16)
	bx_5 = (word16) es_bx_3
	Mem0[es_bx_3 + 0x0004:byte] = 0x03
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
    uses: r1_1 = r1
          r1_2 = r1 << 0x01
          r1_3 = r1 * 0x0003
          r1_4 = r1 * 0x0006
r1_1: orig: r1
    def:  r1_1 = r1
r1_2: orig: r1
    def:  r1_2 = r1 << 0x01
r1_3: orig: r1
    def:  r1_3 = r1 * 0x0003
r1_4: orig: r1
    def:  r1_4 = r1 * 0x0006
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r1
	// succ:  l1
l1:
	r1_1 = r1
	r1_2 = r1 << 0x01
	r1_3 = r1 * 0x0003
	r1_4 = r1 * 0x0006
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
@"r1_0: orig: r1
    def:  r1_0 = foo
r63:r63
    def:  def r63
    uses: r63_2 = r63 - 0x00000004
          Mem3[r63 - 0x00000004:word32] = 0x00000003
          r63_4 = r63 - 0x00000008
          r63_4 = r63 - 0x00000008
          Mem5[r63 - 0x00000008:word16] = Mem3[0x01231230:word16]
r63_2: orig: r63
    def:  r63_2 = r63 - 0x00000004
Mem3: orig: Mem0
    def:  Mem3[r63 - 0x00000004:word32] = 0x00000003
    uses: Mem5[r63 - 0x00000008:word16] = Mem3[0x01231230:word16]
r63_4: orig: r63
    def:  r63_4 = r63 - 0x00000008
Mem5: orig: Mem0
    def:  Mem5[r63 - 0x00000008:word16] = Mem3[0x01231230:word16]
r1_6: orig: r1
    def:  r1_6 = foo(Mem0[r63:word32], Mem0[r63 + 0x00000004:word32])
r63_7: orig: r63
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r63
	// succ:  l1
l1:
	r1_0 = foo
	r63_2 = r63 - 0x00000004
	Mem3[r63 - 0x00000004:word32] = 0x00000003
	r63_4 = r63 - 0x00000008
	Mem5[r63 - 0x00000008:word16] = Mem3[0x01231230:word16]
	r1_6 = foo(Mem0[r63:word32], Mem0[r63 + 0x00000004:word32])
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion
                AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void VpCastCast()
        {
            var m = new ProcedureBuilder();
            m.Store(
                m.Word32(0x1234000),
                m.Cast(
                    PrimitiveType.Real32,
                    m.Cast(
                        PrimitiveType.Real64, 
                        m.Load(PrimitiveType.Real32, m.Word32(0x123400)))));
            m.Return();
            mr.ReplayAll();

            RunFileTest(m, "Analysis/VpCastCast.txt");
        }
    }
}

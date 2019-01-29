#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Moq;
using System.Diagnostics;
using System.Collections.Generic;
using Reko.UnitTests.Fragments;
using Reko.Core.Rtl;
using Reko.Core.Serialization;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class ValuePropagationTests : AnalysisTestBase
	{
        private Mock<IProcessorArchitecture> arch;
        private Mock<IImportResolver> importResolver;
        private FakeDecompilerEventListener listener;
        private SsaProcedureBuilder m;
        private SegmentMap segmentMap;

        [SetUp]
		public void Setup()
		{
            arch = new Mock<IProcessorArchitecture>();
            importResolver = new Mock<IImportResolver>();
            listener = new FakeDecompilerEventListener();
            m = new SsaProcedureBuilder();
            segmentMap = new SegmentMap(Address.Ptr32(0));
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

        protected override void RunTest(Program program, TextWriter writer)
		{
			var dfa = new DataFlowAnalysis(program, importResolver.Object, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in program.Procedures.Values)
			{
				writer.WriteLine("= {0} ========================", proc.Name);
				var gr = proc.CreateBlockDominatorGraph();
				Aliases alias = new Aliases(proc);
				alias.Transform();
                SsaTransform sst = new SsaTransform(dfa.ProgramDataFlow, proc, importResolver.Object, gr,
                    new HashSet<RegisterStorage>());
				SsaState ssa = sst.SsaState;
                var cce = new ConditionCodeEliminator(ssa, program.Platform);
                cce.Transform();
				ssa.Write(writer);
				proc.Write(false, writer);
				writer.WriteLine();

				ValuePropagator vp = new ValuePropagator(program.SegmentMap, ssa, importResolver.Object, listener);
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

        private void RunValuePropagator()
        {
            var vp = new ValuePropagator(segmentMap, m.Ssa, importResolver.Object, listener);
            vp.Transform();
            m.Ssa.Validate(s => Assert.Fail(s));
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
            Given_FakeWin32Platform();
            this.platformMock.Setup(p => p.ResolveImportByName(It.IsAny<string>(), It.IsAny<string>())).Returns((Expression) null);
            this.platformMock.Setup(p => p.DataTypeFromImportName(It.IsAny<string>())).Returns((Tuple<string, SerializedType, SerializedType>) null);
            this.platformMock.Setup(p => p.ResolveIndirectCall(It.IsAny<RtlCall>())).Returns((Address) null);
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
                m.MStore(r, m.Word32(0));
                m.Assign(r, m.ISub(r, 4));
                m.Assign(zf, m.Cond(r));
                m.BranchCc(ConditionCode.NE, "l0000");

                m.Label("l0001");
                m.Assign(r, 42);

                m.Label("l0002");
                m.MStore(r, m.Word32(12));
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

			ValuePropagator vp = new ValuePropagator(segmentMap, ssa, importResolver.Object, listener);
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
			Identifier foo = m.Reg32("foo");

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
            var ctx = new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, importResolver.Object);
            ctx.Statement = new Statement(0, new SideEffect(Constant.Word32(32)), null);
            return new ExpressionSimplifier(segmentMap, ctx, listener);
        }

		[Test]
		public void VpAddZero()
		{
			Identifier r = m.Reg32("r");

            var sub = new BinaryExpression(Operator.ISub, PrimitiveType.Word32, new MemoryAccess(MemoryIdentifier.GlobalMemory, r, PrimitiveType.Word32), Constant.Word32(0));
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, importResolver.Object), listener);
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

            var x = m.Reg32("x");
			var y = m.Reg32("y");
            var stmX = m.Assign(x, m.Mem32(Constant.Word32(0x1000300)));
            var stmY = m.Assign(y, m.ISub(x, 2));
			var stm = m.BranchIf(m.Eq(y, 0), "test");
			Assert.AreEqual("x = Mem2[0x01000300:word32]", stmX.ToString());
			Assert.AreEqual("y = x - 0x00000002", stmY.ToString());
			Assert.AreEqual("branch y == 0x00000000 test", stm.ToString());

			var vp = new ValuePropagator(segmentMap, m.Ssa, importResolver.Object, listener);
			vp.Transform(stm);
			Assert.AreEqual("branch x == 0x00000002 test", stm.Instruction.ToString());
		}

		[Test]
		public void VpCopyPropagate()
		{
			var x = m.Reg32("x");
            var y = m.Reg32("y");
            var z = m.Reg32("z");
            var w = m.Reg32("w");
			m.Assign(x, m.Mem32(Constant.Word32(0x10004000)));
            var stmX = m.Block.Statements.Last();
            m.Assign(y, x);
            var stmY = m.Block.Statements.Last();
            m.Assign(z, m.IAdd(y, Constant.Word32(2)));
            var stmZ = m.Block.Statements.Last();
            m.Assign(w, y);
            var stmW = m.Block.Statements.Last();
            Assert.AreEqual("x = Mem4[0x10004000:word32]", stmX.Instruction.ToString());
			Assert.AreEqual("y = x", stmY.Instruction.ToString());
			Assert.AreEqual("z = y + 0x00000002", stmZ.Instruction.ToString());
			Assert.AreEqual("w = y", stmW.Instruction.ToString());

			var vp = new ValuePropagator(segmentMap, m.Ssa, importResolver.Object, listener);
			vp.Transform(stmX);
			vp.Transform(stmY);
			vp.Transform(stmZ);
			vp.Transform(stmW);

			Assert.AreEqual("x = Mem4[0x10004000:word32]", stmX.Instruction.ToString());
			Assert.AreEqual("y = x", stmY.Instruction.ToString());
			Assert.AreEqual("z = x + 0x00000002", stmZ.Instruction.ToString());
			Assert.AreEqual("w = x", stmW.Instruction.ToString());
			Assert.AreEqual(3, m.Ssa.Identifiers[x].Uses.Count);
			Assert.AreEqual(0, m.Ssa.Identifiers[y].Uses.Count);
		}

		[Test]
		public void VpSliceConstant()
		{
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, null, importResolver.Object), listener);
            Expression c = new Slice(PrimitiveType.Byte, Constant.Word32(0x10FF), 0).Accept(vp);
			Assert.AreEqual("0xFF", c.ToString());
		}

		[Test]
		public void VpNegSub()
		{
			Identifier x = m.Reg32("x");
			Identifier y = m.Reg32("y");
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, importResolver.Object), listener);
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
			Identifier id = m.Reg32("id");
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, importResolver.Object), listener);
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
			Identifier id = m.Reg32("id");
			Expression e = m.Shl(m.Shl(id, 1), 4);
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, importResolver.Object), listener);
			e = e.Accept(vp);
			Assert.AreEqual("id << 0x05", e.ToString());
		}

		[Test]
		public void VpShiftSum()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Expression e = m.Shl(1, m.ISub(Constant.Byte(32), 1));
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, null, importResolver.Object), listener);
			e = e.Accept(vp);
			Assert.AreEqual("0x80000000", e.ToString());
		}

		[Test]
		public void VpSequenceOfConstants()
		{
			Constant pre = Constant.Word16(0x0001);
			Constant fix = Constant.Word16(0x0002);
			Expression e = new MkSequence(PrimitiveType.Word32, pre, fix);
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, null, importResolver.Object), listener);
			e = e.Accept(vp);
			Assert.AreEqual("0x00010002", e.ToString());
		}

        [Test]
        public void SliceShift()
        {
            Constant eight = Constant.Word16(8);
            Identifier C = m.Reg8("C");
            Expression e = new Slice(PrimitiveType.Byte, new BinaryExpression(Operator.Shl, PrimitiveType.Word16, C, eight), 8);
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, importResolver.Object), listener);
            e = e.Accept(vp);
            Assert.AreEqual("C", e.ToString());
        }

        [Test]
        public void VpMkSequenceToAddress()
        {
            Constant seg = Constant.Create(PrimitiveType.SegmentSelector, 0x4711);
            Constant off = Constant.Word16(0x4111);
            arch.Setup(a => a.MakeSegmentedAddress(seg, off)).Returns(Address.SegPtr(0x4711, 0x4111));

            Expression e = new MkSequence(PrimitiveType.Word32, seg, off);
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch.Object, null, importResolver.Object), listener);
            e = e.Accept(vp);
            Assert.IsInstanceOf(typeof(Address), e);
            Assert.AreEqual("4711:4111", e.ToString());
        }

        [Test]
        public void VpPhiWithConstants()
        {
            var c1 = Constant.Word16(0x4711);
            var c2 = Constant.Word16(0x4711);
            var r1 = m.Reg16("r1");
            var r2 = m.Reg16("r2");
            var r3 = m.Reg16("r3");
            m.Assign(r1, c1);
            m.Assign(r2, c2);
            var phiStm = m.Phi(r3, (r1, "block1"), (r2, "block2"));
            RunValuePropagator();
            Assert.AreEqual("r3 = 0x4711", phiStm.Instruction.ToString());
        }

        [Test(Description =
            "if x = phi(a_1, a_2, ... a_n) and all phi arguments after " +
            "value propagation are equal to <exp> or x where <exp> is some  " +
            "expression then replace phi assignment with x = <exp>)")]
        public void VpPhiLoops()
        {
            var fp = m.Reg16("fp");
            var a = m.Reg16("a");
            var b = m.Reg16("b");
            var c = m.Reg16("c");
            var d = m.Reg16("d");
            var x = m.Reg16("x");
            var y = m.Reg16("y");
            var z = m.Reg16("z");
            m.Assign(y, m.IAdd(x, 4));
            m.Assign(z, m.ISub(x, 8));
            m.Assign(a, m.ISub(fp, 12));
            m.Assign(b, m.ISub(fp, 12));
            m.Assign(c, m.ISub(y, 4));
            m.Assign(d, m.IAdd(z, 8));
            var phiStm = m.Phi(x, 
                (a, "blockA"),
                (b, "blockB"),
                (c, "blockC"),
                (d, "blockD"));
            RunValuePropagator();
            Assert.AreEqual("x = fp - 0x000C", phiStm.Instruction.ToString());
        }

        [Test(Description =
            "if x = phi(a_1, a_2, ... a_n) and all phi arguments after " +
            "value propagation are equal to <exp> or x where <exp> is some  " +
            "expression then replace phi assignment with x = <exp>)")]
        public void VpPhiLoopsSimplifyArgs()
        {
            var sp = m.Reg16("sp");
            var sp_1 = m.Reg16("sp_1");
            var sp_2 = m.Reg16("sp_2");
            var a = m.Reg16("a");
            var b = m.Reg16("b");
            var c = m.Reg16("c");
            var d = m.Reg16("d");
            var v = m.Reg16("v");
            var w = m.Reg16("w");
            var x = m.Reg16("x");
            var y = m.Reg16("y");
            var z = m.Reg16("z");
            m.Phi(sp, (sp_1, "block1"), (sp_2, "block2"));
            m.Assign(v, m.ISub(sp, 4));
            m.Assign(w, m.ISub(sp, 8));
            m.Assign(y, m.IAdd(x, 4));
            m.Assign(z, m.ISub(x, 8));
            m.Assign(a, m.ISub(v, 8));
            m.Assign(b, m.ISub(w, 4));
            m.Assign(c, m.ISub(y, 4));
            m.Assign(d, m.IAdd(z, 8));
            var phiStm = m.Phi(x, 
                (a, "blockA"),
                (b, "blockB"),
                (c, "blockC"),
                (d, "blockD"));
            RunValuePropagator();
            Assert.AreEqual("x = sp - 0x000C", phiStm.Instruction.ToString());
        }

        [Test]
        public void VpUndoSlicingOfSegmentPointerCheckUses()
        {
            var es = m.Reg16("es");
            var es_2 = m.Reg16("es_2");
            var bx = m.Reg16("bx");
            var bx_3 = m.Reg16("bx_3");
            var bx_4 = m.Reg16("bx_4");
            var es_bx_1 = m.Reg32("es_bx_1");

            m.SStore(es, m.IAdd(bx, 4), m.Byte(3));
            m.Assign(es_bx_1, m.SegMem(PrimitiveType.Word32, es, bx));
            m.Assign(es_2, m.Slice(PrimitiveType.Word16, es_bx_1, 16));
            m.Assign(bx_3, m.Cast(PrimitiveType.Word16, es_bx_1));
            var instr = m.Assign(bx_4, m.SegMem(PrimitiveType.Word16, es_2, m.IAdd(bx_3, 4)));
            RunValuePropagator();
            Assert.AreEqual("bx_4 = Mem8[es_bx_1 + 0x0004:word16]", instr.ToString());
        }


        private class DpbMock : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				var dl = LocalByte("dl");
				Local16("dx");
				var edx = Local32("edx");

				Assign(edx, Word32(0x0AAA00AA));
				Assign(edx, Dpb(edx, Byte(0x55), 8));
				MStore(Word32(0x1000000), edx);

				Assign(edx, Word32(0));
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

            m.Assign(d1, m.Dpb(d1, m.Mem16(a1), 0));
            m.Assign(d1, m.Dpb(d1, m.Mem16(m.IAdd(a1, 4)), 0));

			Procedure proc = m.Procedure;
			var gr = proc.CreateBlockDominatorGraph();
            var importResolver = new Mock<IImportResolver>().Object;
			var sst = new SsaTransform(new ProgramDataFlow(), proc, importResolver, gr, new HashSet<RegisterStorage>());
			var ssa = sst.SsaState;

			var vp = new ValuePropagator(segmentMap, ssa, importResolver, listener);
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
            var sst = new SsaTransform(new ProgramDataFlow(), proc, importResolver.Object, gr, new HashSet<RegisterStorage>());
            var ssa = sst.SsaState;

            var segmentMap = new SegmentMap(Address.Ptr32(0));
            var vp = new ValuePropagator(segmentMap, ssa, importResolver.Object, listener);
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

            m.Assign(tmp, m.Mem8(a2));
            m.Assign(d3, m.Dpb(d3, tmp, 0));
            m.MStore(m.IAdd(a2, 4), m.Cast(PrimitiveType.Byte, d3));

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

            m.Assign(tmp, m.Mem16(a2));
            m.Assign(d3, m.Dpb(d3, tmp, 0));
            m.MStore(m.IAdd(a2, 4), m.Cast(PrimitiveType.Byte, d3));

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
            m.SStore(es, m.IAdd(bx, 4), m.Byte(3));

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
          Mem6[es_bx_3 + 0x0004:byte] = 0x03
es_4: orig: es
    def:  es_4 = SLICE(es_bx_3, word16, 16)
bx_5: orig: bx
    def:  bx_5 = (word16) es_bx_3
Mem6: orig: Mem0
    def:  Mem6[es_bx_3 + 0x0004:byte] = 0x03
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
	Mem6[es_bx_3 + 0x0004:byte] = 0x03
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
            var pc = new ProcedureConstant(PrimitiveType.Ptr32, callee);

            var m = new ProcedureBuilder();
            var r1 = m.Reg32("r1", 1);
            var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
            m.Assign(r1, pc);
            m.Assign(sp, m.ISub(sp, 4));
            m.MStore(sp, m.Word32(3));
            m.Assign(sp, m.ISub(sp, 4));
            m.MStore(sp, m.Mem16(m.Word32(0x1231230)));
            m.Call(r1, 4);
            m.Return();

            arch.Setup(a => a.CreateStackAccess(
                It.IsAny<IStorageBinder>(),
                It.IsAny<int>(),
                It.IsAny<DataType>()))
                .Returns((IStorageBinder f, int off, DataType dt) => m.Mem(dt, m.IAdd(sp, off)));

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
          Mem5[r63 - 0x00000008:word16] = Mem3[0x01231230:word16]
          r1_6 = foo(Mem8[r63 - 0x00000008:word32], Mem9[r63 - 0x00000004:word32])
          r1_6 = foo(Mem8[r63 - 0x00000008:word32], Mem9[r63 - 0x00000004:word32])
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
    def:  r1_6 = foo(Mem8[r63 - 0x00000008:word32], Mem9[r63 - 0x00000004:word32])
r63_7: orig: r63
Mem8: orig: Mem0
    uses: r1_6 = foo(Mem8[r63 - 0x00000008:word32], Mem9[r63 - 0x00000004:word32])
Mem9: orig: Mem0
    uses: r1_6 = foo(Mem8[r63 - 0x00000008:word32], Mem9[r63 - 0x00000004:word32])
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
	r1_6 = foo(Mem8[r63 - 0x00000008:word32], Mem9[r63 - 0x00000004:word32])
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
            m.MStore(
                m.Word32(0x1234000),
                m.Cast(
                    PrimitiveType.Real32,
                    m.Cast(
                        PrimitiveType.Real64, 
                        m.Mem(PrimitiveType.Real32, m.Word32(0x123400)))));
            m.Return();

            RunFileTest(m, "Analysis/VpCastCast.txt");
        }

        [Test(Description = "m68k floating-point comparison")]
        public void VpFCmp()
        {
            var m = new FCmpFragment();

            RunFileTest(m, "Analysis/VpFCmp.txt");
        }

        [Test(Description = "Should be able to simplify address +/- constant")]
        public void VpAddress32Const()
        {
            var m = new ProcedureBuilder("VpAddress32Const");
            var r1 = m.Reg32("r1", 1);
            m.Assign(r1, Address.Ptr32(0x00123400));
            m.Assign(r1, m.Mem(r1.DataType, m.IAdd(r1, 0x56)));
            m.Return();

            RunFileTest(m, "Analysis/VpAddress32Const.txt");
        }

        [Test]
        public void VpUndoSlicingOfSegmentPointerCheckUses_NoOffset()
        {
            var es = m.Reg16("es");
            var es_2 = m.Reg16("es_2");
            var bx = m.Reg16("bx");
            var bx_3 = m.Reg16("bx_3");
            var bx_4 = m.Reg16("bx_4");
            var es_bx_1 = m.Reg32("es_bx_1");

            m.SStore(es, m.IAdd(bx, 4), m.Byte(3));
            m.Assign(es_bx_1, m.SegMem(PrimitiveType.Word32, es, bx));
            m.Assign(es_2, m.Slice(PrimitiveType.Word16, es_bx_1, 16));
            m.Assign(bx_3, m.Cast(PrimitiveType.Word16, es_bx_1));
            var instr = m.Assign(bx_4, m.SegMem(PrimitiveType.Word16, es_2, bx_3));
            RunValuePropagator();
            Assert.AreEqual("bx_4 = Mem8[es_bx_1:word16]", instr.ToString());
        }
    }
}

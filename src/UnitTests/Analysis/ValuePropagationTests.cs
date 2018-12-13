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
using Reko.UnitTests.Fragments;
using Reko.Core.Lib;

namespace Reko.UnitTests.Analysis
{
    /// <summary>
    /// Tests for intra-procedural value propagation. 
    /// </summary>
    /// <remarks>
    /// Because of the intra procedural nature of ValuePropagation,
    /// it is pointless to test it on fragments consisting of multiple
    /// procedures. In addition, the dependency on 
    /// </remarks>
	[TestFixture]
	public class ValuePropagationTests : AnalysisTestBase
	{
        private MockRepository mr;
        private IProcessorArchitecture arch;
        private Program program;
        private IImportResolver importResolver;
        private FakeDecompilerEventListener listener;
        private SsaProcedureBuilder m;
        private SegmentMap segmentMap;

        [SetUp]
		public void Setup()
		{
            mr = new MockRepository();
            arch = mr.Stub<IProcessorArchitecture>();
            importResolver = mr.Stub<IImportResolver>();
            listener = new FakeDecompilerEventListener();
            m = new SsaProcedureBuilder();
            segmentMap = new SegmentMap(Address.Ptr32(0));
            program = new Program()
            {
                Architecture = arch,
                SegmentMap = segmentMap,
            };
        }

        private Identifier Reg32(string name)
        {
            var mr = new RegisterStorage(name, m.Ssa.Identifiers.Count, 0, PrimitiveType.Word32);
            Identifier id = new Identifier(mr.Name, mr.DataType, mr);
            SsaIdentifier sid = new SsaIdentifier(id, id, null, null, false);
            m.Ssa.Identifiers.Add(id, sid);
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

        protected override void RunTest(Program program, TextWriter writer)
		{
			var dfa = new DataFlowAnalysis(program, importResolver, new FakeDecompilerEventListener());
			foreach (Procedure proc in ProceduresInSccOrder(program))
			{
				writer.WriteLine("= {0} ========================", proc.Name);
                SsaTransform sst = new SsaTransform(
                    program, 
                    proc, 
                    new HashSet<Procedure>(),
                    importResolver, 
                    dfa.ProgramDataFlow);
                sst.Transform();
				SsaState ssa = sst.SsaState;
                var cce = new ConditionCodeEliminator(ssa, program.Platform);
                cce.Transform();
				ssa.Write(writer);
				proc.Write(false, writer);
				writer.WriteLine();

				ValuePropagator vp = new ValuePropagator(program.SegmentMap, ssa, importResolver, listener);
				vp.Transform();
                sst.RenameFrameAccesses = true;
                sst.Transform();

				ssa.Write(writer);
				proc.Write(false, writer);
			}
		}

        private List<Procedure> ProceduresInSccOrder(Program program)
        {
            var list = new List<Procedure>();
            void CollectScc(IList<Procedure> sccProcs)
            {
                list.AddRange(sccProcs);
            }
            var sscf = new SccFinder<Procedure>(new ProcedureGraph(program), CollectScc);
            foreach (var procedure in program.Procedures.Values)
            {
                sscf.Find(procedure);
            }
            return list;
        }

    private SsaState RunTest(ProcedureBuilder m)
        {
            var proc = m.Procedure;
            var sst = new SsaTransform(
                program,
                proc,
                new HashSet<Procedure>(),
                importResolver,
                new ProgramDataFlow());
            var ssa = sst.SsaState;
            sst.Transform();

            var vp = new ValuePropagator(segmentMap, ssa, importResolver, listener);
            vp.Transform();
            return ssa;
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
            var vp = new ValuePropagator(segmentMap, m.Ssa, importResolver, listener);
            vp.Transform();
            m.Ssa.Validate(s => Assert.Fail(s));
        }

		[Test]
        [Category(Categories.IntegrationTests)]
		public void VpConstPropagation()
		{
			RunFileTest_x86_real("Fragments/constpropagation.asm", "Analysis/VpConstPropagation.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void VpGlobalHandle()
		{
            Given_FakeWin32Platform(mr);
            this.platform.Stub(p => p.ResolveImportByName(null, null)).IgnoreArguments().Return(null);
            this.platform.Stub(p => p.DataTypeFromImportName(null)).IgnoreArguments().Return(null);
            this.platform.Stub(p => p.ResolveIndirectCall(null)).IgnoreArguments().Return(null);
            mr.ReplayAll();
			RunFileTest_x86_32("Fragments/import32/GlobalHandle.asm", "Analysis/VpGlobalHandle.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void VpNegsNots()
		{
			RunFileTest_x86_real("Fragments/negsnots.asm", "Analysis/VpNegsNots.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void VpNestedRepeats()
		{
			RunFileTest_x86_real("Fragments/nested_repeats.asm", "Analysis/VpNestedRepeats.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void VpWhileGoto()
		{
			RunFileTest_x86_real("Fragments/while_goto.asm", "Analysis/VpWhileGoto.txt");
		}

        [Test]
        [Category(Categories.IntegrationTests)]
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
                m.BranchIf(m.Test(ConditionCode.NE, zf), "l0000");

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
        [Category(Categories.IntegrationTests)]
		public void VpDbp()
		{
			Procedure proc = new DpbFragment().Procedure;
            var sst = new SsaTransform(
                program,
                proc,
                new HashSet<Procedure>(),
                importResolver, 
                new ProgramDataFlow());
            sst.Transform();
            SsaState ssa = sst.SsaState;

			ValuePropagator vp = new ValuePropagator(segmentMap, ssa, importResolver, listener);
			vp.Transform();

			using (FileUnitTester fut = new FileUnitTester("Analysis/VpDbp.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
				fut.AssertFilesEqual();
			}
		}

		[Test]
        [Category(Categories.UnitTests)]
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
            var ctx = new SsaEvaluationContext(arch, m.Ssa.Identifiers, importResolver);
            ctx.Statement = new Statement(0, new SideEffect(Constant.Word32(32)), null);
            return new ExpressionSimplifier(segmentMap, ctx, listener);
        }

		[Test]
        [Category(Categories.UnitTests)]
		public void VpAddZero()
		{
			Identifier r = m.Reg32("r");

            var sub = new BinaryExpression(Operator.ISub, PrimitiveType.Word32, new MemoryAccess(MemoryIdentifier.GlobalMemory, r, PrimitiveType.Word32), Constant.Word32(0));
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, m.Ssa.Identifiers, importResolver), listener);
			var exp = sub.Accept(vp);
			Assert.AreEqual("Mem0[r:word32]", exp.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
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
            m.Assign(x, m.Mem32(Constant.Word32(0x1000300)));
            m.Assign(y, m.ISub(x, 2));
			m.BranchIf(m.Eq(y, 0), "test");
            m.Return();
            m.Label("test");
            m.Return();
            var importResolver = mr.Stub<IImportResolver>();
            importResolver.Replay();
            var sst = new SsaTransform(
                program, 
                m.Procedure, 
                new HashSet<Procedure>(),
                importResolver, 
                new ProgramDataFlow());
            sst.Transform();

            var vp = new ValuePropagator(segmentMap, sst.SsaState, importResolver, listener);
            var stm = m.Procedure.EntryBlock.Succ[0].Statements.Last;
			vp.Transform(stm);
			Assert.AreEqual("branch x_2 == 0x00000002 test", stm.Instruction.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpCopyPropagate()
		{
			Identifier x_2 = Reg32("x_2");
			Identifier y_3 = Reg32("y_3");
			Identifier z_4 = Reg32("z_4");
			Identifier w_5 = Reg32("w_5");
            m.Assign(x_2, m.Mem32(m.Word32(0x10004000)));
            m.Assign(y_3, x_2);
            m.Assign(z_4, m.IAdd(y_3, 2));
            m.Assign(w_5, y_3);

            var importResolver = mr.Stub<IImportResolver>();
            importResolver.Replay();

            var stms = m.Procedure.EntryBlock.Succ[0].Statements;
			Assert.AreEqual("x_2 = Mem4[0x10004000:word32]", stms[0].ToString());
			Assert.AreEqual("y_3 = x_2", stms[1].ToString());
			Assert.AreEqual("z_4 = y_3 + 0x00000002", stms[2].ToString());
			Assert.AreEqual("w_5 = y_3", stms[3].ToString());
            m.Ssa.Dump(true);

            RunValuePropagator();

			Assert.AreEqual("x_2 = Mem4[0x10004000:word32]", stms[0].ToString());
			Assert.AreEqual("y_3 = x_2", stms[1].ToString());
			Assert.AreEqual("z_4 = x_2 + 0x00000002", stms[2].ToString());
			Assert.AreEqual("w_5 = x_2", stms[3].ToString());

            m.Ssa.Dump(true);

			Assert.AreEqual(0, m.Ssa.Identifiers.Single(i => i.Identifier.Name == "y_3").Uses.Count);
            Assert.AreEqual(3, m.Ssa.Identifiers.Single(i => i.Identifier.Name == "x_2").Uses.Count);
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpSliceConstant()
		{
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, null, importResolver), listener);
            Expression c = new Slice(PrimitiveType.Byte, Constant.Word32(0x10FF), 0).Accept(vp);
			Assert.AreEqual("0xFF", c.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpNegSub()
		{
			Identifier x = m.Reg32("x");
			Identifier y = m.Reg32("y");
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, m.Ssa.Identifiers, importResolver), listener);
			Expression e = vp.VisitUnaryExpression(
				new UnaryExpression(Operator.Neg, PrimitiveType.Word32, new BinaryExpression(
				Operator.ISub, PrimitiveType.Word32, x, y)));
			Assert.AreEqual("y - x", e.ToString());
		}

		/// <summary>
		/// (<< (+ (* id c1) id) c2))
		/// </summary>
		[Test] 
        [Category(Categories.UnitTests)]
		public void VpMulAddShift()
		{
			Identifier id = m.Reg32("id");
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, m.Ssa.Identifiers, importResolver), listener);
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
        [Category(Categories.UnitTests)]
		public void VpShiftShift()
		{
			Identifier id = m.Reg32("id");
			Expression e = m.Shl(m.Shl(id, 1), 4);
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, m.Ssa.Identifiers, importResolver), listener);
			e = e.Accept(vp);
			Assert.AreEqual("id << 0x05", e.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpShiftSum()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Expression e = m.Shl(1, m.ISub(Constant.Byte(32), 1));
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, null, importResolver), listener);
			e = e.Accept(vp);
			Assert.AreEqual("0x80000000", e.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpSequenceOfConstants()
		{
			Constant pre = Constant.Word16(0x0001);
			Constant fix = Constant.Word16(0x0002);
			Expression e = new MkSequence(PrimitiveType.Word32, pre, fix);
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, null, importResolver), listener);
			e = e.Accept(vp);
			Assert.AreEqual("0x00010002", e.ToString());
		}

        [Test]
        [Category(Categories.UnitTests)]
        public void SliceShift()
        {
            Constant eight = Constant.Word16(8);
            Identifier C = m.Reg8("C");
            Expression e = new Slice(PrimitiveType.Byte, new BinaryExpression(Operator.Shl, PrimitiveType.Word16, C, eight), 8);
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, m.Ssa.Identifiers, importResolver), listener);
            e = e.Accept(vp);
            Assert.AreEqual("C", e.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void VpMkSequenceToAddress()
        {
            Constant seg = Constant.Create(PrimitiveType.SegmentSelector, 0x4711);
            Constant off = Constant.Word16(0x4111);
            arch.Expect(a => a.MakeSegmentedAddress(seg, off)).Return(Address.SegPtr(0x4711, 0x4111));
            mr.ReplayAll();

            Expression e = new MkSequence(PrimitiveType.Word32, seg, off);
            var vp = new ExpressionSimplifier(segmentMap, new SsaEvaluationContext(arch, null, importResolver), listener);
            e = e.Accept(vp);
            Assert.IsInstanceOf(typeof(Address), e);
            Assert.AreEqual("4711:4111", e.ToString());

            mr.VerifyAll();
        }

        [Test]
        [Category(Categories.UnitTests)]
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
        [Category(Categories.UnitTests)]
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
        [Category(Categories.IntegrationTests)]
        public void VpDbpDbp()
        {
            var m = new ProcedureBuilder();
            var d1 = m.Reg32("d32",0);
            var a1 = m.Reg32("a32",1);

            m.Assign(d1, m.Dpb(d1, m.Mem16(a1), 0));
            m.Assign(d1, m.Dpb(d1, m.Mem16(m.IAdd(a1, 4)), 0));

			Procedure proc = m.Procedure;
			var gr = proc.CreateBlockDominatorGraph();
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();
			var sst = new SsaTransform(
                program,
                proc, 
                new HashSet<Procedure>(),
                importResolver,
                new ProgramDataFlow());
            sst.Transform();
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

        [Test(Description = "Casting a DPB should result in the deposited bits.")]
        [Category(Categories.UnitTests)]
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
    uses: tmp_3 = Mem0[a2:byte]
          Mem6[a2 + 0x00000004:byte] = tmp_3
Mem0:Mem
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
define ProcedureBuilder
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
        [Category(Categories.UnitTests)]
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
    uses: tmp_3 = Mem0[a2:word16]
          Mem6[a2 + 0x00000004:byte] = (byte) tmp_3
Mem0:Mem
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
define ProcedureBuilder
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
        [Category(Categories.UnitTests)]
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
define ProcedureBuilder
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
        [Category(Categories.UnitTests)]
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
    uses: es_bx_4 = Mem0[es:bx:word32]
bx:bx
    def:  def bx
    uses: es_bx_4 = Mem0[es:bx:word32]
Mem0:Mem
    def:  def Mem0
    uses: es_bx_4 = Mem0[es:bx:word32]
es_bx_4: orig: es_bx
    def:  es_bx_4 = Mem0[es:bx:word32]
    uses: es_5 = SLICE(es_bx_4, word16, 16) (alias)
          bx_6 = (word16) es_bx_4 (alias)
          es_7 = SLICE(es_bx_4, word16, 16)
          bx_8 = (word16) es_bx_4
          Mem9[es_bx_4 + 0x0004:byte] = 0x03
es_5: orig: es
    def:  es_5 = SLICE(es_bx_4, word16, 16) (alias)
bx_6: orig: bx
    def:  bx_6 = (word16) es_bx_4 (alias)
es_7: orig: es
    def:  es_7 = SLICE(es_bx_4, word16, 16)
bx_8: orig: bx
    def:  bx_8 = (word16) es_bx_4
Mem9: orig: Mem0
    def:  Mem9[es_bx_4 + 0x0004:byte] = 0x03
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def es
	def bx
	def Mem0
	// succ:  l1
l1:
	es_bx_4 = Mem0[es:bx:word32]
	es_5 = SLICE(es_bx_4, word16, 16) (alias)
	bx_6 = (word16) es_bx_4 (alias)
	es_7 = SLICE(es_bx_4, word16, 16)
	bx_8 = (word16) es_bx_4
	Mem9[es_bx_4 + 0x0004:byte] = 0x03
ProcedureBuilder_exit:
";
            #endregion

            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
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
define ProcedureBuilder
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
        [Category(Categories.UnitTests)]
        public void VpIndirectCall()
        {
            var callee = CreateExternalProcedure("foo", RegArg(1, "r1"), StackArg(4), StackArg(8));
            var pc = new ProcedureConstant(PrimitiveType.Ptr32, callee);

            var m = new ProcedureBuilder();
            var r1 = m.Reg32("r1", 1);
            var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
            m.Assign(sp, m.Frame.FramePointer);
            m.Assign(r1, pc);
            m.Assign(sp, m.ISub(sp, 4));
            m.MStore(sp, m.Word32(3));
            m.Assign(sp, m.ISub(sp, 4));
            m.MStore(sp, m.Mem16(m.Word32(0x1231230)));
            m.Call(r1, 4);
            m.Return();

            arch.Stub(a => a.CreateStackAccess(null, 0, null))
                .IgnoreArguments()
                .Do(new Func<IStorageBinder, int, DataType, Expression>((f, off, dt) => m.Mem(dt, m.IAdd(f.EnsureRegister((RegisterStorage)sp.Storage), off))));
            arch.Stub(s => s.CreateFrameApplicationBuilder(null, null, null)).IgnoreArguments().Do(
                new Func<IStorageBinder, CallSite, Expression, FrameApplicationBuilder>(
                (frame, site, c) => new FrameApplicationBuilder(arch, frame, site, c, false)));

            mr.ReplayAll();

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_2 = fp
          r63_4 = fp - 0x00000004
          Mem5[fp - 0x00000004:word32] = 0x00000003
          r63_6 = fp - 0x00000008
          Mem7[fp - 0x00000008:word16] = Mem5[0x01231230:word16]
          r1_8 = foo(Mem7[fp - 0x00000008:word32], Mem7[fp - 0x00000004:word32])
          r1_8 = foo(Mem7[fp - 0x00000008:word32], Mem7[fp - 0x00000004:word32])
r63_2: orig: r63
    def:  r63_2 = fp
r1_3: orig: r1
    def:  r1_3 = foo
r63_4: orig: r63
    def:  r63_4 = fp - 0x00000004
Mem5: orig: Mem0
    def:  Mem5[fp - 0x00000004:word32] = 0x00000003
    uses: Mem7[fp - 0x00000008:word16] = Mem5[0x01231230:word16]
r63_6: orig: r63
    def:  r63_6 = fp - 0x00000008
Mem7: orig: Mem0
    def:  Mem7[fp - 0x00000008:word16] = Mem5[0x01231230:word16]
    uses: r1_8 = foo(Mem7[fp - 0x00000008:word32], Mem7[fp - 0x00000004:word32])
          r1_8 = foo(Mem7[fp - 0x00000008:word32], Mem7[fp - 0x00000004:word32])
r1_8: orig: r1
    def:  r1_8 = foo(Mem7[fp - 0x00000008:word32], Mem7[fp - 0x00000004:word32])
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def fp
	// succ:  l1
l1:
	r63_2 = fp
	r1_3 = foo
	r63_4 = fp - 0x00000004
	Mem5[fp - 0x00000004:word32] = 0x00000003
	r63_6 = fp - 0x00000008
	Mem7[fp - 0x00000008:word16] = Mem5[0x01231230:word16]
	r1_8 = foo(Mem7[fp - 0x00000008:word32], Mem7[fp - 0x00000004:word32])
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion
                AssertStringsEqual(sExp, ssa);
        }

        [Test]
        [Category(Categories.IntegrationTests)]
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
            mr.ReplayAll();

            Assert.IsNotNull(importResolver);
            RunFileTest(m, "Analysis/VpCastCast.txt");
        }

        [Test(Description = "m68k floating-point comparison")]
        [Category(Categories.IntegrationTests)]
        public void VpFCmp()
        {
            var m = new FCmpFragment();
            mr.ReplayAll();

            RunFileTest(m, "Analysis/VpFCmp.txt");
        }

        [Test(Description = "Should be able to simplify address +/- constant")]
        [Category(Categories.IntegrationTests)]
        public void VpAddress32Const()
        {
            var m = new ProcedureBuilder("VpAddress32Const");
            var r1 = m.Reg32("r1", 1);
            m.Assign(r1, Address.Ptr32(0x00123400));
            m.Assign(r1, m.Mem(r1.DataType, m.IAdd(r1, 0x56)));
            m.Return();

            mr.ReplayAll();
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

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.IntegrationTests)]
        public void VpConstantHighByte()
        {
            var pb = new ProgramBuilder();
            pb.Add("sum", m =>
            {
                var _ax = new RegisterStorage("ax", 0, 0, PrimitiveType.Word16);
                var _al = new RegisterStorage("al", 0, 0, PrimitiveType.Byte);
                var _ah = new RegisterStorage("ah", 0, 8, PrimitiveType.Byte);
                var _dx = new RegisterStorage("dx", 2, 0, PrimitiveType.Word16);
                var _si = new RegisterStorage("si", 6, 0, PrimitiveType.Word16);
                var ax = m.Frame.EnsureRegister(_ax);
                var ah = m.Frame.EnsureRegister(_ah);
                var al = m.Frame.EnsureRegister(_al);
                var dx = m.Frame.EnsureRegister(_dx);
                var si = m.Frame.EnsureRegister(_si);

                m.Assign(ah, 0);

                m.Label("m0");
                m.BranchIf(m.Eq0(m.Mem8(si)), "m3done");

                m.Label("m1");
                m.Assign(al, m.Mem8(si));
                m.Assign(dx, m.IAdd(dx, ax));
                m.Goto("m0");

                m.Label("m3done");
                m.Return();
            });
            RunFileTest(pb.BuildProgram(), "Analysis/VpConstantHighByte.txt");
        }
    }
}

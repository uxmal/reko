#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Services;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Analysis
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
        private Mock<IProcessorArchitecture> arch;
        private Mock<IDynamicLinker> dynamicLinker;
        private Program program;
        private FakeDecompilerEventListener listener;
        private SsaProcedureBuilder m;

        [SetUp]
		public void Setup()
		{
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("FakeArch");
            dynamicLinker = new Mock<IDynamicLinker>();
            listener = new FakeDecompilerEventListener();
            m = new SsaProcedureBuilder();
            var segmentMap = new SegmentMap(Address.Ptr32(0));
            program = new Program()
            {
                Architecture = arch.Object,
                Platform = new FakePlatform(sc, arch.Object),
                Memory = new ByteProgramMemory(segmentMap),
                SegmentMap = segmentMap,
            };
            sc = new ServiceContainer();
            sc.AddService<IDecompilerEventListener>(listener);
            sc.AddService<IPluginLoaderService>(new PluginLoaderService());
        }

        private ExternalProcedure CreateExternalProcedure(
            string name,
            int stackDelta,
            Identifier ret,
            params Identifier[] parameters)
        {
            var ep = new ExternalProcedure(name, new FunctionType(parameters, [ret]));
            ep.Signature.ReturnAddressOnStack = 4;
            ep.Signature.StackDelta = stackDelta;
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
                new StackStorage(offset, PrimitiveType.Word32));
        }

        protected override void RunTest(Program program, TextWriter writer)
		{
            var dfa = new DataFlowAnalysis(
                program,
                dynamicLinker.Object,
                sc);
			foreach (Procedure proc in ProceduresInSccOrder(program))
            {
                writer.WriteLine("= {0} ========================", proc.Name);
                SsaTransform sst = new SsaTransform(
                    program,
                    proc,
                    new HashSet<Procedure>(),
                    dynamicLinker.Object,
                    dfa.ProgramDataFlow);
                sst.Transform();
                SsaState ssa = sst.SsaState;
                var ctx = CreateContext(program, proc);
                var cce = new ConditionCodeEliminator(ctx);
                cce.Transform(ssa);
                ssa.Write(writer);
                proc.Write(false, writer);
                writer.WriteLine();

                var vp = new ValuePropagator(ctx);
                vp.Transform(ssa);
                sst.RenameFrameAccesses = true;
                sst.Transform();

                ssa.Write(writer);
                proc.Write(false, writer);
            }
        }

        private AnalysisContext CreateContext(Program program, Procedure proc)
        {
            return new AnalysisContext(program, proc, dynamicLinker.Object, sc, listener);
        }

        private List<Procedure> ProceduresInSccOrder(Program program)
        {
            var list = new List<Procedure>();
            var sscs = SccFinder.FindAll(new ProcedureGraph(program));
            foreach (var scc in sscs)
            {
                list.AddRange(scc);
            }
            return list;
        }

        private SsaState RunTest(ProcedureBuilder m)
        {
            var proc = m.Procedure;
            var context = new AnalysisContext(
                program,
                proc,
                dynamicLinker.Object,
                sc,
                listener);

            var sst = new SsaTransform(
                program,
                proc,
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                new ProgramDataFlow());
            var ssa = sst.SsaState;
            sst.Transform();

            var vp = new ValuePropagator(context);
            vp.Transform(ssa);
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
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void AssertLastStatement(string sExp)
        {
            var lastStatement = m.Ssa.Procedure.Statements.Last();
            Assert.AreEqual(sExp, lastStatement.ToString());
        }

        private void RunValuePropagator()
        {
            var context = CreateContext(program, m.Ssa.Procedure);
            var vp = new ValuePropagator(context);
            vp.Transform(m.Ssa);
            m.Ssa.Validate(s => { m.Ssa.Dump(true); Assert.Fail(s); });
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
            Given_FakeWin32Platform();
            //this.platformMock.Setup(p => p.ResolveImportByName(null, null)).IgnoreArguments().Return(null);
            //this.platformMock.Setup(p => p.DataTypeFromImportName(null)).IgnoreArguments().Return(null);
            //this.platformMock.Setup(p => p.ResolveIndirectCall(null)).IgnoreArguments().Return(null);
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
                m.Assign(zf, m.Cond(zf.DataType, r));
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
                dynamicLinker.Object, 
                new ProgramDataFlow());
            sst.Transform();
            SsaState ssa = sst.SsaState;

            var context = CreateContext(program, proc);
            ValuePropagator vp = new ValuePropagator(context);
			vp.Transform(ssa);

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
				m.Bin(Operator.Eq, PrimitiveType.Bool, 
				m.Bin(Operator.ISub, PrimitiveType.Word32, foo,
				m.Word32(1)),
				m.Word32(0));
			Assert.AreEqual("foo - 1<32> == 0<32>", expr.ToString());

			var (simpler, _) = vp.VisitBinaryExpression(expr);
			Assert.AreEqual("foo == 1<32>", simpler.ToString());
		}

        private ExpressionSimplifier CreatePropagatorWithDummyStatement()
        {
            var ctx = new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, dynamicLinker.Object);
            ctx.Statement = new Statement(Address.Ptr32(0), new SideEffect(m.Word32(32)), null);
            return new ExpressionSimplifier(program.Memory, ctx, listener);
        }

		[Test]
        [Category(Categories.UnitTests)]
		public void VpAddZero()
		{
			Identifier r = m.Reg32("r");

            var sub = m.Bin(Operator.ISub, PrimitiveType.Word32, new MemoryAccess(MemoryStorage.GlobalMemory, r, PrimitiveType.Word32), m.Word32(0));
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, dynamicLinker.Object), listener);
			var exp = sub.Accept(vp).Item1;
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

            var x = m.Reg32("x");
			var y = m.Reg32("y");
            var stmX = m.Assign(x, m.Mem32(m.Word32(0x1000300)));
            var stmY = m.Assign(y, m.ISub(x, 2));
			var stm = m.BranchIf(m.Eq(y, 0), "test");
			Assert.AreEqual("x = Mem2[0x1000300<32>:word32]", stmX.ToString());
			Assert.AreEqual("y = x - 2<32>", stmY.ToString());
			Assert.AreEqual("branch y == 0<32> test", stm.ToString());

            RunValuePropagator();

			Assert.AreEqual("branch x == 2<32> test", stm.Instruction.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpCopyPropagate()
		{
			var x = m.Reg32("x");
            var y = m.Reg32("y");
            var z = m.Reg32("z");
            var w = m.Reg32("w");
            var stmX = m.Assign(x, m.Mem32(m.Word32(0x10004000)));
            var stmY = m.Assign(y, x);
            var stmZ = m.Assign(z, m.IAdd(y, m.Word32(2)));
            var stmW = m.Assign(w, y);
            Assert.AreEqual("x = Mem4[0x10004000<32>:word32]", stmX.ToString());
			Assert.AreEqual("y = x", stmY.ToString());
			Assert.AreEqual("z = y + 2<32>", stmZ.ToString());
			Assert.AreEqual("w = y", stmW.ToString());

            RunValuePropagator();

			Assert.AreEqual("x = Mem4[0x10004000<32>:word32]", stmX.ToString());
			Assert.AreEqual("y = x", stmY.ToString());
			Assert.AreEqual("z = x + 2<32>", stmZ.ToString());
			Assert.AreEqual("w = x", stmW.ToString());
			Assert.AreEqual(3, m.Ssa.Identifiers[x].Uses.Count);
			Assert.AreEqual(0, m.Ssa.Identifiers[y].Uses.Count);
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpSliceConstant()
		{
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, null, dynamicLinker.Object), listener);
            var (c, _) = m.Slice(m.Word32(0x10FF), PrimitiveType.Byte).Accept(vp);
			Assert.AreEqual("0xFF<8>", c.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpNegSub()
		{
			Identifier x = m.Reg32("x");
			Identifier y = m.Reg32("y");
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, dynamicLinker.Object), listener);
			var (e, _) = vp.VisitUnaryExpression(
				new UnaryExpression(Operator.Neg, PrimitiveType.Word32, m.Bin(
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
			Identifier id = this.m.Reg32("id");
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, this.m.Ssa.Identifiers, dynamicLinker.Object), listener);
			PrimitiveType t = PrimitiveType.Int32;
            var m = new ExpressionEmitter();
			BinaryExpression b = m.Bin(Operator.Shl, t, 
				m.Bin(Operator.IAdd, t, 
					m.Bin(Operator.SMul, t, id, m.Const(t, 4)),
					id),
				m.Const(t, 2));
			var (e, _) = vp.VisitBinaryExpression(b);
			Assert.AreEqual("id *s 20<i32>", e.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpShiftShift()
		{
			Identifier id = m.Reg32("id");
			Expression e = m.Shl(m.Shl(id, 1), 4);
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, dynamicLinker.Object), listener);
			(e, _) = e.Accept(vp);
			Assert.AreEqual("id << 5<8>", e.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpShiftSum()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Expression e = m.Shl(1, m.ISub(m.Byte(32), 1));
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, null, dynamicLinker.Object), listener);
			(e, _) = e.Accept(vp);
			Assert.AreEqual("0x80000000<32>", e.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void VpSequenceOfConstants()
		{
			Constant pre = m.Word16(0x0001);
			Constant fix = m.Word16(0x0002);
			Expression e = m.Seq(PrimitiveType.Word32, pre, fix);
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, null, dynamicLinker.Object), listener);
			(e, _) = e.Accept(vp);
			Assert.AreEqual("0x10002<32>", e.ToString());
		}

        [Test]
        [Category(Categories.UnitTests)]
        public void VpSliceShift()
        {
            Constant eight = m.Word16(8);
            Identifier C = m.Reg8("C");
            Expression e = m.Slice(m.Bin(Operator.Shl, PrimitiveType.Word16, C, eight), PrimitiveType.Byte, 8);
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, m.Ssa.Identifiers, dynamicLinker.Object), listener);
            (e, _) = e.Accept(vp);
            Assert.AreEqual("C", e.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void VpMkSequenceToAddress()
        {
            Constant seg = m.Const(PrimitiveType.SegmentSelector, 0x4711);
            Constant off = m.Word16(0x4111);
            arch.Setup(a => a.MakeSegmentedAddress(seg, off))
                .Returns(Address.SegPtr(0x4711, 0x4111))
                .Verifiable();

            Expression e = m.Seq(PrimitiveType.Word32, seg, off);
            var vp = new ExpressionSimplifier(program.Memory, new SsaEvaluationContext(arch.Object, null, dynamicLinker.Object), listener);
            (e, _) = e.Accept(vp);
            Assert.IsInstanceOf(typeof(Address), e);
            Assert.AreEqual("4711:4111", e.ToString());

            arch.Verify();
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void VpPhiWithConstants()
        {
            var c1 = m.Word16(0x4711);
            var c2 = m.Word16(0x4711);
            var r1 = m.Reg16("r1");
            var r2 = m.Reg16("r2");
            var r3 = m.Reg16("r3");
            m.Assign(r1, c1);
            m.Assign(r2, c2);
            var phiStm = m.Phi(r3, (r1, "block1"), (r2, "block2"));
            RunValuePropagator();
            Assert.AreEqual("r3 = 0x4711<16>", phiStm.Instruction.ToString());
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
            Assert.AreEqual("x = fp - 0xC<16>", phiStm.Instruction.ToString());
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
            Assert.AreEqual("x = sp - 0xC<16>", phiStm.Instruction.ToString());
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
            m.Assign(es_2, m.Slice(es_bx_1, PrimitiveType.Word16, 16));
            m.Assign(bx_3, m.Slice(es_bx_1, PrimitiveType.Word16, 0));
            var instr = m.Assign(bx_4, m.SegMem(PrimitiveType.Word16, es_2, m.IAdd(bx_3, 4)));
            RunValuePropagator();
            Assert.AreEqual("bx_4 = Mem8[es_bx_1 + 4<16>:word16]", instr.ToString());
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
            var dynamicLinker = new Mock<IDynamicLinker>();
			var sst = new SsaTransform(
                program,
                proc, 
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                new ProgramDataFlow());
            sst.Transform();
			var ssa = sst.SsaState;

            var context = CreateContext(program, proc);
            var vp = new ValuePropagator(context);
			vp.Transform(ssa);

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
            m.MStore(m.IAdd(a2, 4), m.Slice(d3, PrimitiveType.Byte));

            SsaState ssa = RunTest(m);

            var sExp =
            #region Expected
@"a2:a2
    def:  def a2
    uses: tmp_3 = Mem0[a2:byte]
          Mem6[a2 + 4<32>:byte] = tmp_3
Mem0:Mem
    def:  def Mem0
    uses: tmp_3 = Mem0[a2:byte]
tmp_3: orig: tmp
    def:  tmp_3 = Mem0[a2:byte]
    uses: d3_5 = SEQ(SLICE(d3, word24, 8), tmp_3)
          Mem6[a2 + 4<32>:byte] = tmp_3
d3:d3
    def:  def d3
    uses: d3_5 = SEQ(SLICE(d3, word24, 8), tmp_3)
d3_5: orig: d3
    def:  d3_5 = SEQ(SLICE(d3, word24, 8), tmp_3)
Mem6: orig: Mem0
    def:  Mem6[a2 + 4<32>:byte] = tmp_3
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
	d3_5 = SEQ(SLICE(d3, word24, 8), tmp_3)
	Mem6[a2 + 4<32>:byte] = tmp_3
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
            m.MStore(m.IAdd(a2, 4), m.Slice(d3, PrimitiveType.Byte));

            SsaState ssa = RunTest(m);

            var sExp =
            #region Expected
@"a2:a2
    def:  def a2
    uses: tmp_3 = Mem0[a2:word16]
          Mem6[a2 + 4<32>:byte] = SLICE(tmp_3, byte, 0)
Mem0:Mem
    def:  def Mem0
    uses: tmp_3 = Mem0[a2:word16]
tmp_3: orig: tmp
    def:  tmp_3 = Mem0[a2:word16]
    uses: d3_5 = SEQ(SLICE(d3, word16, 16), tmp_3)
          Mem6[a2 + 4<32>:byte] = SLICE(tmp_3, byte, 0)
d3:d3
    def:  def d3
    uses: d3_5 = SEQ(SLICE(d3, word16, 16), tmp_3)
d3_5: orig: d3
    def:  d3_5 = SEQ(SLICE(d3, word16, 16), tmp_3)
Mem6: orig: Mem0
    def:  Mem6[a2 + 4<32>:byte] = SLICE(tmp_3, byte, 0)
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
	d3_5 = SEQ(SLICE(d3, word16, 16), tmp_3)
	Mem6[a2 + 4<32>:byte] = SLICE(tmp_3, byte, 0)
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

            m.Assign(r1, m.Convert(Constant.Real64(1), PrimitiveType.Real64, PrimitiveType.Real32));

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
            var es_bx = m.Frame.EnsureSequence(PrimitiveType.Word32, es.Storage, bx.Storage);

            m.Assign(es_bx, m.SegMem(PrimitiveType.Word32, es, bx));
            m.Assign(es, m.Slice(es_bx, PrimitiveType.Word16, 16));
            m.Assign(bx, m.Slice(es_bx, PrimitiveType.Word16, 0));
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
          bx_6 = SLICE(es_bx_4, word16, 0) (alias)
          es_7 = SLICE(es_bx_4, word16, 16)
          bx_8 = SLICE(es_bx_4, word16, 0)
          Mem9[es_bx_4 + 4<16>:byte] = 3<8>
es_5: orig: es
    def:  es_5 = SLICE(es_bx_4, word16, 16) (alias)
bx_6: orig: bx
    def:  bx_6 = SLICE(es_bx_4, word16, 0) (alias)
es_7: orig: es
    def:  es_7 = SLICE(es_bx_4, word16, 16)
bx_8: orig: bx
    def:  bx_8 = SLICE(es_bx_4, word16, 0)
Mem9: orig: Mem0
    def:  Mem9[es_bx_4 + 4<16>:byte] = 3<8>
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
	bx_6 = SLICE(es_bx_4, word16, 0) (alias)
	es_7 = SLICE(es_bx_4, word16, 16)
	bx_8 = SLICE(es_bx_4, word16, 0)
	Mem9[es_bx_4 + 4<16>:byte] = 3<8>
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
          r1_3 = r1 << 1<8>
          r1_4 = r1 * 3<16>
          r1_5 = r1 * 6<16>
r1_2: orig: r1
    def:  r1_2 = r1
r1_3: orig: r1
    def:  r1_3 = r1 << 1<8>
r1_4: orig: r1
    def:  r1_4 = r1 * 3<16>
r1_5: orig: r1
    def:  r1_5 = r1 * 6<16>
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r1
	// succ:  l1
l1:
	r1_2 = r1
	r1_3 = r1 << 1<8>
	r1_4 = r1 * 3<16>
	r1_5 = r1 * 6<16>
ProcedureBuilder_exit:
";
            #endregion

            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void VpIndirectCall()
        {
            var callee = CreateExternalProcedure(
                "foo",
                12,
                RegArg(1, "r1"),
                StackArg(4),
                StackArg(8));
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

            arch.Setup(a => a.CreateStackAccess(
                It.IsAny<IStorageBinder>(),
                It.IsAny<int>(),
                It.IsAny<DataType>()))
                .Returns((IStorageBinder f, int off, DataType dt) => 
                    m.Mem(dt, m.IAdd(f.EnsureRegister((RegisterStorage)sp.Storage), off)));
            arch.Setup(s => s.CreateFrameApplicationBuilder(
                It.IsAny<IStorageBinder>(),
                It.IsAny<CallSite>()))
                .Returns((IStorageBinder binder, CallSite site) =>
                    new FrameApplicationBuilder(arch.Object, binder, site));

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_2 = fp
          r63_4 = fp - 4<32>
          Mem5[fp - 4<32>:word32] = 3<32>
          r63_6 = fp - 8<32>
          Mem7[fp - 8<32>:word16] = Mem5[0x1231230<32>:word16]
          r1_8 = foo(Mem7[fp - 8<32>:word32], Mem7[fp - 4<32>:word32])
          r1_8 = foo(Mem7[fp - 8<32>:word32], Mem7[fp - 4<32>:word32])
r63_2: orig: r63
    def:  r63_2 = fp
r1_3: orig: r1
    def:  r1_3 = foo
r63_4: orig: r63
    def:  r63_4 = fp - 4<32>
Mem5: orig: Mem0
    def:  Mem5[fp - 4<32>:word32] = 3<32>
    uses: Mem7[fp - 8<32>:word16] = Mem5[0x1231230<32>:word16]
r63_6: orig: r63
    def:  r63_6 = fp - 8<32>
Mem7: orig: Mem0
    def:  Mem7[fp - 8<32>:word16] = Mem5[0x1231230<32>:word16]
    uses: r1_8 = foo(Mem7[fp - 8<32>:word32], Mem7[fp - 4<32>:word32])
          r1_8 = foo(Mem7[fp - 8<32>:word32], Mem7[fp - 4<32>:word32])
r1_8: orig: r1
    def:  r1_8 = foo(Mem7[fp - 8<32>:word32], Mem7[fp - 4<32>:word32])
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def fp
	// succ:  l1
l1:
	r63_2 = fp
	r1_3 = foo
	r63_4 = fp - 4<32>
	Mem5[fp - 4<32>:word32] = 3<32>
	r63_6 = fp - 8<32>
	Mem7[fp - 8<32>:word16] = Mem5[0x1231230<32>:word16]
	r1_8 = foo(Mem7[fp - 8<32>:word32], Mem7[fp - 4<32>:word32])
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
                m.Convert(
                    m.Convert(
                        m.Mem(PrimitiveType.Real32, m.Word32(0x123400)),
                        PrimitiveType.Real32,
                        PrimitiveType.Real64),
                    PrimitiveType.Real64,
                    PrimitiveType.Real32));
            m.Return();

            Assert.IsNotNull(dynamicLinker);
            RunFileTest(m, "Analysis/VpCastCast.txt");
        }

        [Test(Description = "m68k floating-point comparison")]
        [Category(Categories.IntegrationTests)]
        public void VpFCmp()
        {
            var m = new FCmpFragment();

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
            m.Assign(es_2, m.Slice(es_bx_1, PrimitiveType.Word16, 16));
            m.Assign(bx_3, m.Slice(es_bx_1, PrimitiveType.Word16, 0));
            var instr = m.Assign(bx_4, m.SegMem(PrimitiveType.Word16, es_2, bx_3));
            RunValuePropagator();
            Assert.AreEqual("bx_4 = Mem8[es_bx_1:word16]", instr.ToString());
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void VpConstantHighByte()
        {
            var pb = new ProgramBuilder();
            pb.Add("sum", m =>
            {
                var _ax = RegisterStorage.Reg16("ax", 0);
                var _al = RegisterStorage.Reg8("al", 0);
                var _ah = RegisterStorage.Reg8("ah", 0, 8);
                var _dx = RegisterStorage.Reg16("dx", 2);
                var _si = RegisterStorage.Reg16("si", 6);
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

        // This code breaks in ValuePropagator.VisitCallInstruction when calling  
        // ssaIdTransformer.Transform(...)
        [Test]
        public void VpReusedRegistersAtCall()
        {
            var sExp =
            #region Expected
@"r2_1: orig: r2
    def:  r2_1 = Mem4[0x220200<32>:word32]
    uses: r4_1 = r2_1
          callee(r2_1)
r2_2: orig: r2
    def:  r2_2 = callee
r4_1: orig: r4
    def:  r4_1 = r2_1
r25_1: orig: r25
    def:  r25_1 = callee
Mem4: orig: Mem0
    uses: r2_1 = Mem4[0x220200<32>:word32]
Mem5: orig: Mem0
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r2_1 = Mem4[0x220200<32>:word32]
	r4_1 = r2_1
	r2_2 = callee
	r25_1 = callee
	callee(r2_1)
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            var uAddrGotSlot = m.Word32(0x0040000);
            var reg2 = RegisterStorage.Reg32("r2", 2);
            var reg4 = RegisterStorage.Reg32("r4", 4);
            var reg25 = RegisterStorage.Reg32("r25", 25);
            var r2_1 = m.Reg("r2_1", reg2);
            var r2_2 = m.Reg("r2_2", reg2);
            var r4_1 = m.Reg("r4_1", reg4);
            var r25_1 = m.Reg("r25_1", reg25);
            m.Assign(r2_1, m.Mem32(m.Word32(0x0220200)));    // fetch a string pointer perhaps
            m.Assign(r4_1, r2_1);
            m.Assign(r2_2, m.Mem32(uAddrGotSlot)); // pointer to a function in a lookup table.
            m.Assign(r25_1, r2_2);
            var stmCall = m.Call(r25_1, 0,
                new Identifier[] { r4_1, r2_2 },
                new Identifier[] { });
            m.Return();

            // Initially we don't know what r2_2 is pointing to.
            var context = CreateContext(program, m.Ssa.Procedure);
            var vp = new ValuePropagator(context);
            vp.Transform(m.Ssa);

            // Later, Reko discovers information about the pointer in 0x400000<32>!

            var sigCallee = FunctionType.Action(
                    new Identifier("r4", PrimitiveType.Word64, reg4));
            var callee = new ProcedureConstant(
                PrimitiveType.Ptr32,
                new ExternalProcedure("callee", sigCallee));
            // Add our new found knowledge to the import resolver.
            dynamicLinker.Setup(i => i.ResolveToImportedValue(
                It.IsNotNull<Statement>(),
                uAddrGotSlot)).
                Returns(callee);

            // Run Value propagation again with the newly gathered information.

            vp.Transform(m.Ssa);

            m.Ssa.Validate(s => { Console.WriteLine(m.Ssa); Assert.Fail(s); });
            AssertStringsEqual(sExp, m.Ssa);
        }

        // Unit test for Github #773
        [Test]
        [Category(Categories.UnitTests)]
        public void VpDpbPhi()
        {
            var d3 = m.Reg32("d3", 3);
            var wLoc02_2 = m.Local16("wLoc02_2");
            var d3_3 = m.Reg32("d3_3", 3);
            var d3_4 = m.Reg32("d3_4", 3);
            var d3_5 = m.Reg32("d3_5", 3);
            var d3_6 = m.Reg32("d3_6", 3);

            m.Assign(wLoc02_2, m.Slice(d3, PrimitiveType.Word16));
            m.Label("m1");
            m.Assign(d3_3, m.Dpb(d3, m.Word16(3), 0));
            m.Goto("m3");
            m.Label("m2");
            m.Assign(d3_4, m.Dpb(d3, m.Word16(4), 0));
            m.Label("m3");
            m.Phi(d3_5, (d3_3, "m1"), (d3_4, "m2"));
            m.Assign(d3_6, m.Dpb(d3_5, wLoc02_2, 0));

            RunValuePropagator();

            var sExp =
            #region Expected
@"d3: orig: d3
    uses: wLoc02_2 = SLICE(d3, word16, 0)
          d3_3 = SEQ(SLICE(d3, word16, 16), 3<16>)
          d3_4 = SEQ(SLICE(d3, word16, 16), 4<16>)
wLoc02_2: orig: wLoc04
    def:  wLoc02_2 = SLICE(d3, word16, 0)
    uses: d3_6 = SEQ(SLICE(d3_5, word16, 16), wLoc02_2)
d3_3: orig: d3_3
    def:  d3_3 = SEQ(SLICE(d3, word16, 16), 3<16>)
    uses: d3_5 = PHI((d3_3, m1), (d3_4, m2))
d3_4: orig: d3_4
    def:  d3_4 = SEQ(SLICE(d3, word16, 16), 4<16>)
    uses: d3_5 = PHI((d3_3, m1), (d3_4, m2))
d3_5: orig: d3_5
    def:  d3_5 = PHI((d3_3, m1), (d3_4, m2))
    uses: d3_6 = SEQ(SLICE(d3_5, word16, 16), wLoc02_2)
d3_6: orig: d3_6
    def:  d3_6 = SEQ(SLICE(d3_5, word16, 16), wLoc02_2)
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	wLoc02_2 = SLICE(d3, word16, 0)
	// succ:  m1
m1:
	d3_3 = SEQ(SLICE(d3, word16, 16), 3<16>)
	goto m3
	// succ:  m3
m2:
	d3_4 = SEQ(SLICE(d3, word16, 16), 4<16>)
	// succ:  m3
m3:
	d3_5 = PHI((d3_3, m1), (d3_4, m2))
	d3_6 = SEQ(SLICE(d3_5, word16, 16), wLoc02_2)
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

        [Test]
        public void VpSliceSeq()
        {
            var t1 = m.Temp(PrimitiveType.Word16, "t1");
            var t2 = m.Temp(PrimitiveType.Word32, "t2");
            var t3 = m.Temp(PrimitiveType.Word16, "t3");

            m.Assign(t2, m.Seq(t1, m.Mem16(m.Word32(0x00123400))));
            m.Assign(t3, m.Slice(t2, PrimitiveType.Word16, 16));

            RunValuePropagator();

            var sExp =
            #region Expected
@"t1: orig: t1
    uses: t2 = SEQ(t1, Mem3[0x123400<32>:word16])
          t3 = t1
t2: orig: t2
    def:  t2 = SEQ(t1, Mem3[0x123400<32>:word16])
t3: orig: t3
    def:  t3 = t1
Mem3: orig: Mem0
    uses: t2 = SEQ(t1, Mem3[0x123400<32>:word16])
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	t2 = SEQ(t1, Mem3[0x123400<32>:word16])
	t3 = t1
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

        [Test]
        public void VpSliceSeq_SameInstruction()
        {
            var t1 = m.Temp(PrimitiveType.Word16, "t1");
            var t2 = m.Temp(PrimitiveType.Word16, "t2");

            var instr = m.Assign(t2, m.Slice(
                m.Seq(t1, m.Mem16(m.Word32(0x00123400))),
                PrimitiveType.Word16,
                16));

            RunValuePropagator();

            Assert.AreEqual("t2 = t1", instr.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void VpSliceConst()
        {
            var t1 = m.Temp(PrimitiveType.Word64, "t1");
            var t2 = m.Temp(PrimitiveType.Word32, "t2");

            m.Assign(t1, m.Word64(2));
            m.Assign(t2, m.Slice(t1, PrimitiveType.Word32));

            RunValuePropagator();

            var sExp =
            #region Expected
@"t1: orig: t1
    def:  t1 = 2<64>
t2: orig: t2
    def:  t2 = 2<32>
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	t1 = 2<64>
	t2 = 2<32>
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

        /// <summary>
        /// This reassembles 64-bit constants. It is based on actual PowerPC code.
        /// </summary>
        [Test]
        public void Vp64BitConstant()
        {
            var r9_1 = m.Reg32("r9_1", 9);
            var r10_1 = m.Reg64("r10_1", 10);
            var r10_2 = m.Reg64("r10_2", 10);
            var r11_1 = m.Reg64("r11_1", 11);
            var r11_2 = m.Reg64("r11_2", 11);
            var r11_3 = m.Reg64("r11_3", 11);
            var r4_1 = m.Reg64("r4_1", 4);
            var r19 = m.Reg64("r19", 19);
            var r30 = m.Reg64("r30", 30);
            var v30 = m.Temp(PrimitiveType.Word32, "v30");

            m.AddDefToEntryBlock(r19);
            m.AddDefToEntryBlock(r30);
            m.Assign(r11_1, m.Word64(0x91690000));
            m.Assign(r4_1, m.Convert(m.Mem32(m.IAdd(r19, 28)), PrimitiveType.Word32, PrimitiveType.Word64));
            m.Assign(r10_1, m.Word64(0x42420000));
            m.Assign(r11_2, m.Or(r11_1, 0x1448));
            m.Assign(r10_2, m.Or(r10_1, 0x8DA6));
            m.Assign(r9_1, m.And(r30, m.Word64(0xFFFFFFFF)));
            m.Assign(v30, m.Slice(r11_2, PrimitiveType.Word32));
            m.Assign(r11_3, m.Seq(m.Slice(r10_2, PrimitiveType.Word32), v30));

            RunValuePropagator();

            var sExp =
            #region Expected
@"r9_1: orig: r9_1
    def:  r9_1 = r30 & 0xFFFFFFFF<64>
r10_1: orig: r10_1
    def:  r10_1 = 0x42420000<64>
r10_2: orig: r10_2
    def:  r10_2 = 0x42428DA6<64>
r11_1: orig: r11_1
    def:  r11_1 = 0x91690000<64>
r11_2: orig: r11_2
    def:  r11_2 = 0x91691448<64>
r11_3: orig: r11_3
    def:  r11_3 = 0x42428DA691691448<64>
r4_1: orig: r4_1
    def:  r4_1 = CONVERT(Mem10[r19 + 0x1C<64>:word32], word32, word64)
r19: orig: r19
    def:  def r19
    uses: r4_1 = CONVERT(Mem10[r19 + 0x1C<64>:word32], word32, word64)
r30: orig: r30
    def:  def r30
    uses: r9_1 = r30 & 0xFFFFFFFF<64>
v30: orig: v30
    def:  v30 = 0x91691448<32>
Mem10: orig: Mem0
    uses: r4_1 = CONVERT(Mem10[r19 + 0x1C<64>:word32], word32, word64)
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	def r19
	def r30
	// succ:  l1
l1:
	r11_1 = 0x91690000<64>
	r4_1 = CONVERT(Mem10[r19 + 0x1C<64>:word32], word32, word64)
	r10_1 = 0x42420000<64>
	r11_2 = 0x91691448<64>
	r10_2 = 0x42428DA6<64>
	r9_1 = r30 & 0xFFFFFFFF<64>
	v30 = 0x91691448<32>
	r11_3 = 0x42428DA691691448<64>
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

        [Test]
        public void VpGitHub942()
        {
            var rax_6 = m.Reg64("rax_6");
            var rbx_7 = m.Reg64("rbx_7");
            var ax_8 = m.Reg16("ax_8");
            var rax_10 = m.Reg64("rax_10");
            var rax_48_16_9 = m.Temp(PrimitiveType.CreateWord(48), "rax_48_16_9");
            m.Assign(rax_6, m.Word64(0x1A2A3A4A5A6A7A8A));
            m.Assign(rax_48_16_9, m.Slice(rax_6, rax_48_16_9.DataType, 16));
            m.Assign(rbx_7, m.Word64(0x1B2B3B4B5B6B7B8B));
            m.Assign(ax_8, m.Slice(rbx_7, ax_8.DataType));
            m.Assign(rax_10, m.Seq(rax_48_16_9, ax_8));
            m.MStore(m.Word32(0x123400), rax_10);

            RunValuePropagator();
            var sExp =
            #region Expected
@"rax_6: orig: rax_6
    def:  rax_6 = 0x1A2A3A4A5A6A7A8A<64>
rbx_7: orig: rbx_7
    def:  rbx_7 = 0x1B2B3B4B5B6B7B8B<64>
ax_8: orig: ax_8
    def:  ax_8 = 0x7B8B<16>
rax_10: orig: rax_10
    def:  rax_10 = 0x1A2A3A4A5A6A7B8B<64>
rax_48_16_9: orig: rax_48_16_9
    def:  rax_48_16_9 = 0x1A2A3A4A5A6A<48>
Mem5: orig: Mem0
    def:  Mem5[0x123400<32>:word64] = 0x1A2A3A4A5A6A7B8B<64>
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	rax_6 = 0x1A2A3A4A5A6A7A8A<64>
	rax_48_16_9 = 0x1A2A3A4A5A6A<48>
	rbx_7 = 0x1B2B3B4B5B6B7B8B<64>
	ax_8 = 0x7B8B<16>
	rax_10 = 0x1A2A3A4A5A6A7B8B<64>
	Mem5[0x123400<32>:word64] = 0x1A2A3A4A5A6A7B8B<64>
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

        [Test]
        public void VpGithub1074()
        {
            var x1 = m.Reg64("x1");
            var x2 = m.Reg64("x2");
            var x3 = m.Reg64("x3");
            m.Assign(x1, m.Mem64(m.Word64(0x00123400)));
            m.Assign(x2, m.Shl(x1, 3));
            m.Assign(x3, m.ISub(x2, x1));
            m.MStore(m.Word64(0x00123408), x3);

            RunValuePropagator();
            var sExp =
            #region Expected
@"x1: orig: x1
    def:  x1 = Mem3[0x123400<64>:word64]
    uses: x2 = x1 << 3<8>
          x3 = x1 * 7<64>
          Mem4[0x123408<64>:word64] = x1 * 7<64>
x2: orig: x2
    def:  x2 = x1 << 3<8>
x3: orig: x3
    def:  x3 = x1 * 7<64>
Mem3: orig: Mem0
    uses: x1 = Mem3[0x123400<64>:word64]
Mem4: orig: Mem0
    def:  Mem4[0x123408<64>:word64] = x1 * 7<64>
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	x1 = Mem3[0x123400<64>:word64]
	x2 = x1 << 3<8>
	x3 = x1 * 7<64>
	Mem4[0x123408<64>:word64] = x1 * 7<64>
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void VpTypeReferenceToReal64()
        {
            var c = m.Word64(0x4028000000000000); // 12.0
            var doubleRef = new TypeReference("DOUBLE", PrimitiveType.Real64);
            var v1 = m.Temp(doubleRef, "v1");
            var v2 = m.Temp(doubleRef, "v2");
            m.Assign(v1, c);
            m.Assign(v2, v1);

            RunValuePropagator();

            AssertLastStatement("v2 = 12.0");
        }

        [Test]
        public void VpSetCarryFlagBeforeAdc()
        {
            var v1 = m.Reg64("v1");
            var v1b = m.Reg8("v1b");
            var v1h = m.Temp(PrimitiveType.CreateWord(56), "v1h");
            var v2b = m.Reg8("v2b");
            var flags = RegisterStorage.Reg32("flags", 42);
            var C = m.Flags("S", new FlagGroupStorage(flags, 1, "S"));

            m.Assign(v1, m.Word64(0x57DF836069B622E7));
            m.Alias(v1b, m.Slice(v1, PrimitiveType.Byte));
            m.Alias(v1h, m.Slice(v1, v1h.DataType, 8));
            m.Assign(C, 1);
            m.Assign(v2b, m.IAdd(m.IAdd(v1b, m.Byte(0x19)), m.Slice(C, v1b.DataType)));

            RunValuePropagator();

            AssertLastStatement("v2b = 1<8>");
        }

        [Test]
        public void VpRegression1()
        {
            string sExpected =
            #region Expected
@"r2: orig: r2
    uses: r2_1 = r2 & 1<32>
          r2_2 = r2 & 1<32>
          Mem4[r4:word32] = r2 & 1<32>
r2_1: orig: r2_1
    def:  r2_1 = r2 & 1<32>
r2_2: orig: r2_2
    def:  r2_2 = r2 & 1<32>
r4: orig: r4
    uses: Mem4[r4:word32] = r2 & 1<32>
Mem4: orig: Mem0
    def:  Mem4[r4:word32] = r2 & 1<32>
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r2_1 = r2 & 1<32>
	r2_2 = r2 & 1<32>
	Mem4[r4:word32] = r2 & 1<32>
SsaProcedureBuilder_exit:
";
            #endregion

            var r2 = m.Reg32("r2");
            var r2_1 = m.Reg32("r2_1");
            var r2_2 = m.Reg32("r2_2");
            var r4 = m.Reg32("r4");

            m.Assign(r2_1, m.And(r2, 1));
            m.Assign(r2_2, m.And(r2_1, 0xFFFF));
            m.MStore(r4, r2_2);

            RunValuePropagator();

            AssertStringsEqual(sExpected, m.Ssa);
        }

        [Test]
        public void VpUnsignedSub()
        {
            string sExpected =
            #region Expected
@"r2: orig: r2
    uses: r2_1 = r2 -u 1<32>
          Mem2[0x123400<32>:word32] = r2 <=u 1<32> ? 1<32> : 0<32>
r2_1: orig: r2_1
    def:  r2_1 = r2 -u 1<32>
Mem2: orig: Mem0
    def:  Mem2[0x123400<32>:word32] = r2 <=u 1<32> ? 1<32> : 0<32>
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r2_1 = r2 -u 1<32>
	Mem2[0x123400<32>:word32] = r2 <=u 1<32> ? 1<32> : 0<32>
SsaProcedureBuilder_exit:
";
            #endregion

            var r2 = m.Reg32("r2");
            var r2_1 = m.Reg32("r2_1");

            m.Assign(r2_1, m.USub(r2, m.Word32(1)));
            m.MStore(m.Word32(0x00123400),
                m.Conditional(
                    PrimitiveType.Word32,
                    m.Le0(r2_1),
                    m.Word32(1),
                    m.Word32(0)));

            RunValuePropagator();

            AssertStringsEqual(sExpected, m.Ssa);
        }

        [Test]
        public void VpCallContinuation()
        {
            string sExpected =
            #region Expected
@"r2: orig: r2
    def:  r2 = %continuation
%continuation:%continuation
    uses: r2 = %continuation
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r2 = %continuation
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            var r2 = m.Reg32("r2", 1);
            var sidCont = m.Ssa.Identifiers.Add(m.Frame.Continuation, null, false);
            m.Assign(r2, sidCont.Identifier);
            m.Call(r2, 0);
            m.Return();

            RunValuePropagator();

            AssertStringsEqual(sExpected, m.Ssa);
        }

        // When slicing zero extensions, we know that the slice is a zero.
        [Test]
        public void VpSliceOfZeroExtension()
        {
            var sExpected =
            #region Expected
@"ds: orig: ds
    uses: bl_8 = Mem10[ds:bx_7 + 0x2605<16>:byte]
          Mem11[ds:0x1234<16>:bool] = bx_9 <=u 0x17<16>
al_1: orig: al_1
    uses: ax_2 = CONVERT(al_1, uint8, uint16)
          bl_3 = al_1
          dl_6 = al_1
          bx_7 = CONVERT(al_1 - 0x20<8>, uint8, uint16)
ax_2: orig: ax_2
    def:  ax_2 = CONVERT(al_1, uint8, uint16)
    uses: dx_5 = ax_2
bl_3: orig: bl_3
    def:  bl_3 = al_1
bh_4: orig: bh_4
    def:  bh_4 = 0<8>
dx_5: orig: dx_5
    def:  dx_5 = ax_2
dl_6: orig: dl_6
    def:  dl_6 = al_1
bx_7: orig: bx_7
    def:  bx_7 = CONVERT(al_1 - 0x20<8>, uint8, uint16)
    uses: bl_8 = Mem10[ds:bx_7 + 0x2605<16>:byte]
bl_8: orig: bl_8
    def:  bl_8 = Mem10[ds:bx_7 + 0x2605<16>:byte]
    uses: bx_9 = CONVERT(bl_8, uint8, uint16)
bx_9: orig: bx_9
    def:  bx_9 = CONVERT(bl_8, uint8, uint16)
    uses: Mem11[ds:0x1234<16>:bool] = bx_9 <=u 0x17<16>
Mem10: orig: Mem0
    uses: bl_8 = Mem10[ds:bx_7 + 0x2605<16>:byte]
Mem11: orig: Mem0
    def:  Mem11[ds:0x1234<16>:bool] = bx_9 <=u 0x17<16>
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	ax_2 = CONVERT(al_1, uint8, uint16)
	bl_3 = al_1
	bh_4 = 0<8>
	dx_5 = ax_2
	dl_6 = al_1
	bx_7 = CONVERT(al_1 - 0x20<8>, uint8, uint16)
	bl_8 = Mem10[ds:bx_7 + 0x2605<16>:byte]
	bx_9 = CONVERT(bl_8, uint8, uint16)
	Mem11[ds:0x1234<16>:bool] = bx_9 <=u 0x17<16>
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            var ds = m.Temp16("ds");
            var al_1 = m.Temp8("al_1");
            var ax_2 = m.Temp16("ax_2");
            var bl_3 = m.Temp8("bl_3");
            var bh_4 = m.Temp8("bh_4");
            var dx_5 = m.Temp16("dx_5");
            var dl_6 = m.Temp8("dl_6");
            var bx_7 = m.Temp16("bx_7");
            var bl_8 = m.Temp8("bl_8");
            var bx_9 = m.Temp16("bx_9");

            /*
                ax_2 = CONVERT(al_1, uint8, uint16) (alias)
                bl_3 = SLICE(ax_2, byte, 0) (alias)
                bh_4 = SLICE(ax_2, byte, 8) (alias)
                ah_1224 = 0<8>
                dx = ax_2
                dl_6 = SLICE(ax_2, byte, 0) (alias)
                bx = SEQ(bh_4, bl_3 - 0x20<8>) (alias)
                cx = SEQ(ch_1220, 0<8>) (alias)
                branch bl_3 >=u 0x80<8> l0800_98FE
            l0800_98EB:
                bl_8 = Mem77[ds:bx + 0x2605<16>:byte]
                bx = SEQ(bh_4, bl_8) (alias)
                cx = SEQ(ch_1220, 0<8>) (alias)
                branch bx <=u 0x17<16> l0800_98F7
            */
            var uint8 = PrimitiveType.UInt8;
            var uint16 = PrimitiveType.UInt16;
            var @byte = PrimitiveType.Byte;
            m.Assign(ax_2, m.Convert(al_1, uint8, uint16));
            m.Assign(bl_3, m.Slice(ax_2, @byte, 0));
            m.Assign(bh_4, m.Slice(ax_2, @byte, 8));
            m.Assign(dx_5, ax_2);
            m.Assign(dl_6, m.Slice(ax_2, @byte, 0));
            m.Assign(bx_7, m.Seq(bh_4, m.ISub(bl_3, 0x20)));
            m.Assign(bl_8, m.Mem8(m.SegPtr(ds, m.IAdd(bx_7, 0x2605))));
            m.Assign(bx_9, m.Seq(bh_4, bl_8));
            m.MStore(m.SegPtr(ds, m.Word16(0x1234)), m.Ule(bx_9, 0x17));
            m.Return();

            RunValuePropagator();

            AssertStringsEqual(sExpected, m.Ssa);
        }
    }
}

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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using Reko.Typing;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Reko.UnitTests.Decompiler.Typing
{
    [TestFixture]
	public class TraitCollectorTests : TypingTestBase
	{
		private TraitCollector coll;
        private TestTraitHandler handler;
		private ExpressionNormalizer en;
		private EquivalenceClassBuilder eqb;
		private readonly string nl;

		public TraitCollectorTests()
		{
			nl = Environment.NewLine;
		}


		[Test]
        [Category(Categories.IntegrationTests)]
        public void TrcoFloatingPoint()
		{
			RunTest16("Fragments/fpuops.asm", "Typing/TrcoFloatingPoint.txt");
		}

		[Test]
		public void TrcoLength()
		{
			RunTest16("Fragments/type/listlength.asm", "Typing/TrcoLength.txt");
		}

		[Test]
		public void TrcoNestedStructs()
		{
			RunTest16("Fragments/type/nestedstructs.asm", "Typing/TrcoNestedStructs.txt");
		}

		[Test]
		public void TrcoSimpleLinearCode()
		{
			RunTest16("Fragments/simple_memoperations.asm", "Typing/TrcoSimpleLinearCode.txt");
		}

		[Test]
		public void TrcoUnknown()
		{
			RunTest16("Fragments/type/unknown.asm", "Typing/TrcoUnknown.txt");
		}

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TrcoReals()
        {
            RunTest16("Fragments/fpuops.asm", "Typing/TrcoReals.txt");
        }

		[Test]
		public void TrcoMemAccesses()
		{
			RunTest16("Fragments/multiple/memaccesses.asm", "Typing/TrcoMemAccesses.txt");
		}

		[Test]
		public void TrcoCmpMock()
		{
			ProgramBuilder mock = new ProgramBuilder();
			mock.Add(new CmpMock());
			Program program = mock.BuildProgram();
            coll = CreateCollector(program);
            eqb.Build(program);
			coll.CollectProgramTraits(program);

			Verify(program, "Typing/TrcoCmpMock.txt");
		}

		[Test]
		public void TrcoStaggeredArraysMock()
		{
			ProgramBuilder mock = new ProgramBuilder();
			mock.Add(new StaggeredArraysFragment());
			Program program = mock.BuildProgram();
            coll = CreateCollector(program);

			en.Transform(program);
			eqb.Build(program);
			coll.CollectProgramTraits(program);

			Verify(program, "Typing/TrcoStaggeredArraysMock.txt");
		}

		[Test]
		public void TrcoArrayExpression()
		{
			var b = new Identifier("base", PrimitiveType.Word32, null);
			var i = new Identifier("idx", PrimitiveType.Word32, null);
			var s = Constant.Word32(4);

			ProcedureBuilder m = new ProcedureBuilder();

			// e ::= Mem[(b+0x1003000)+(i*s):word16]
			Expression e = m.Mem(
				PrimitiveType.Word16,
				m.IAdd(m.IAdd(b, Constant.Word32(0x10030000)),
				m.SMul(i, s)));
            coll = CreateCollector();
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoArrayExpression.txt");
		}

		[Test]
		public void TrcoInductionVariable()
		{
			Identifier i = new Identifier("i", PrimitiveType.Word32, null);
			MemoryAccess load = new MemoryAccess(MemoryStorage.GlobalMemory, i, PrimitiveType.Int32);
			Identifier i2 = new Identifier("i2", PrimitiveType.Word32, null);
			MemoryAccess ld2 = new MemoryAccess(MemoryStorage.GlobalMemory, i2, PrimitiveType.Int32);

			LinearInductionVariable iv = new LinearInductionVariable(
				Constant.Word32(0), 
				Constant.Word32(1),
				Constant.Word32(10),
                false);
			LinearInductionVariable iv2 = new LinearInductionVariable(
				Constant.Word32(0x0010000),
				Constant.Word32(4),
				Constant.Word32(0x0010040),
                false);

            Program program = CreateProgram();
			program.InductionVariables.Add(i, iv);
			program.InductionVariables.Add(i2, iv2);

            coll = CreateCollector(program);
			program.Globals.Accept(eqb);
			load.Accept(eqb);
			ld2.Accept(eqb);
			program.Globals.Accept(coll);
			load.Accept(coll);
			ld2.Accept(coll);
			Verify(null, "Typing/TrcoInductionVariable.txt");
		}

		[Test]
		public void TrcoGlobalArray()
		{
            Program program = CreateProgram();
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier i = m.Local32("i");
            Expression ea = m.IAdd(program.Globals, m.IAdd(m.Shl(i, 2), 0x3000));
            Expression e = m.Mem(PrimitiveType.Int32, ea);

            coll = CreateCollector(program);
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoGlobalArray.txt");
		}

		[Test]
		public void TrcoMemberPointer()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			MemberPointerSelector mps = m.MembPtr16(ds, m.IAdd(bx, 4));
			Expression e = m.Mem(PrimitiveType.Byte, mps);

            coll = CreateCollector();
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoMemberPointer.txt");
		}

		[Test]
		public void TrcoSegmentedAccess()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			Expression e = m.SegMem(PrimitiveType.Word16, ds, m.IAdd(bx, 4));

            coll = CreateCollector();
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegmentedAccess.txt");
		}

		[Test]
		public void TrcoSegmentedDirectAddress()
		{
            Program program = CreateProgram();
			program.TypeStore.EnsureExpressionTypeVariable(program.TypeFactory, null, program.Globals);

            ProcedureBuilder m = new ProcedureBuilder();
            Identifier ds = m.Local16("ds");
			Expression e = m.SegMem(PrimitiveType.Byte, ds, m.Word16(0x0200));

            coll = CreateCollector(program);
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegmentedDirectAddress.txt");
		}

        [Test]
        public void TrcoMultiplication()
        {
            var program = CreateProgram();
            var m = new ProcedureBuilder();
            var id = m.Local32("id");
            var e = m.IMul(id, id);

            coll = CreateCollector(program);
            e = e.Accept(en);
            e.Accept(eqb);
            e.Accept(coll);
            Verify(null, "Typing/TrcoMultiplication.txt");
        }

        private static Program CreateProgram()
        {
            var arch = new FakeArchitecture(new ServiceContainer());

            return new Program
            {
                Architecture = arch,
                Platform = new DefaultPlatform(arch.Services, arch),
            };
        }

		[Test]
		public void TrcoPtrPtrInt()
		{
			ProgramBuilder p = new ProgramBuilder();
			p.Add(new PtrPtrIntMock());
			RunTest(p.BuildProgram(), "Typing/TrcoPtrPtrInt.txt");
		}

		[Test]
		public void TrcoFnPointerMock()
		{
			ProgramBuilder p = new ProgramBuilder();
			p.Add(new FnPointerFragment());
			RunTest(p.BuildProgram(), "Typing/TrcoFnPointerMock.txt");
		}

		[Test]
		public void TrcoSegmentedMemoryPointer()
		{
			ProgramBuilder p = new ProgramBuilder();
			p.Add(new SegmentedMemoryPointerMock());
			RunTest(p.BuildProgram(), "Typing/TrcoSegmentedMemoryPointer.txt");
		}

		[Test]
		public void TrcoReg00008()
		{
			RunTest16("Fragments/regressions/r00008.asm", "Typing/TrcoReg00008.txt");
		}

        [Test]
		public void TrcoIntelIndexedAddressingMode()
		{
			ProgramBuilder m = new ProgramBuilder();
			m.Add(new IntelIndexedAddressingMode());
			Program program = m.BuildProgram();
            var sc = new ServiceContainer();
            var listener = new FakeDecompilerEventListener();
            sc.AddService<IEventListener>(listener);
            sc.AddService<IDecompilerEventListener>(listener);
            DataFlowAnalysis dfa = new DataFlowAnalysis(program, null, sc);
			dfa.AnalyzeProgram();
			RunTest(program, "Typing/TrcoIntelIndexedAddressingMode.txt");
		}

		[Test]
		public void TrcoTreeFind()
		{
			ProgramBuilder m = new ProgramBuilder();
			m.Add(new TreeFindMock());
			Program program = m.BuildProgram();
            var sc = new ServiceContainer();
            var listener = new FakeDecompilerEventListener();
            sc.AddService<IEventListener>(listener);
            sc.AddService<IDecompilerEventListener>(listener);
            DataFlowAnalysis dfa = new DataFlowAnalysis(program, null, sc);
			dfa.AnalyzeProgram();
			RunTest(program, "Typing/TrcoTreeFind.txt");
		}

		[Test]
		public void TrcoSegmentedDoubleReference()
		{
			ProgramBuilder m = new ProgramBuilder();
			m.Add(new SegmentedDoubleReferenceMock());
			RunTest(m.BuildProgram(), "Typing/TrcoSegmentedDoubleReference.txt");
		}

        [Test]
        public void TrcoSegmentedPointer()
        {
            var m = new ProgramBuilder();
            m.Add(new SegmentedPointerProc());
            RunTest(m.BuildProgram(), "Typing/TrcoSegmentedPointer.txt");
        }

        [Test]
        public void TrcoCallTable()
        {
            var pb = new ProgramBuilder();
            pb.Add(new IndirectCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TrcoCallTable.txt");
        }

        [Test]
        public void TrcoSegmentedCall()
        {
            var pb = new ProgramBuilder();
            pb.Add(new SegmentedCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TrcoSegmentedCall.txt");
        }

		[Test]
		public void TrcoIcall()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier pfn = m.Local32("pfn");
			Expression l = m.Mem(PrimitiveType.Word32, pfn);
			CallInstruction icall = new CallInstruction(l, new CallSite(0, 0));

            coll = CreateCollector();
			icall.Accept(eqb);
			icall.Accept(coll);
			StringWriter sw = new StringWriter();
			handler.Traits.Write(sw);
            string exp =
                "T_1 (in pfn : word32)" + nl +
                "\ttrait_primitive(word32)" + nl +
                "\ttrait_mem(T_2, 0)" + nl +
                "T_2 (in Mem0[pfn:word32] : word32)" + nl +
                "\ttrait_primitive((ptr32 code))" + nl +
                "\ttrait_primitive(word32)" + nl;
			Assert.AreEqual(exp, sw.ToString());
		}

		[Test]
		public void TrcoSegMem()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			Expression e = m.SegMem16(ds, m.Word16(0xC002U));

            coll = CreateCollector();
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegMem.txt");
		}

		[Test]
		public void TrcoUnsignedCompare()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			Expression e = m.Uge(ds, m.Word16(0x0800));

            coll = CreateCollector();
			e.Accept(eqb);
			e.Accept(coll);
			StringWriter sb = new StringWriter();
			handler.Traits.Write(sb);
			string exp = 
				"T_1 (in ds : word16)" + nl +
				"\ttrait_primitive(word16)" + nl +
				"\ttrait_equal(T_2)" + nl +
				"\ttrait_primitive(cupos16)" + nl +
				"T_2 (in 0x800<16> : word16)" + nl +
				"\ttrait_primitive(word16)" + nl +
				"\ttrait_primitive(cupos16)" + nl +
				"T_3 (in ds >=u 0x800<16> : bool)" + nl +
				"\ttrait_primitive(bool)" + nl;
			Assert.AreEqual(exp, sb.ToString());
		}

        [Test]
        public void TrcoArrayAccess()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            Identifier bx = m.Local16("bx");
            Expression e = m.Array(PrimitiveType.Word32, m.Seq(ds, m.Word16(0x300)), m.IMul(bx, 8));
            coll = CreateCollector();
            e.Accept(eqb);
            e.Accept(coll);
            StringWriter sb = new StringWriter();
            handler.Traits.Write(sb);
            string sExp =
                "T_1 (in ds : selector)" + nl +
                "\ttrait_primitive(selector)" + nl +
                "\ttrait_mem_array(300, 8, 0, T_7)" + nl + 
                "T_2 (in 0x300<16> : word16)" + nl +
                "	trait_primitive(word16)" + nl +
                "T_3 (in SEQ(ds, 0x300<16>) : ptr32)" + nl +
                "	trait_primitive(ptr32)" + nl +
                "T_4 (in bx : word16)" + nl +
                "	trait_primitive(word16)" + nl +
                "	trait_primitive(ui16)" + nl +
                "T_5 (in 8<16> : word16)" + nl +
                "	trait_primitive(word16)" + nl +
                "	trait_primitive(ui16)" + nl +
                "T_6 (in bx * 8<16> : word16)" + nl +
                "	trait_primitive(ui16)" + nl +
                "T_7 (in SEQ(ds, 0x300<16>)[bx * 8<16>] : word32)" + nl +
                "	trait_primitive(word32)" + nl;
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void TrcoDbp()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier a = m.Local32("a");
            Identifier b = m.LocalByte("b");
            var s = m.Assign(a, m.Dpb(a, b, 0));
            coll = CreateCollector();
            s.Accept(eqb);
            s.Accept(coll);
            StringWriter sb = new StringWriter();
            handler.Traits.Write(sb);
            string exp =
@"T_1 (in a : word32)
	trait_primitive(word32)
	trait_primitive(word32)
	trait_primitive(word32)
	trait_equal(T_4)
T_2 (in SLICE(a, word24, 8) : word24)
	trait_primitive(word24)
T_3 (in b : byte)
	trait_primitive(byte)
T_4 (in SEQ(SLICE(a, word24, 8), b) : word32)
	trait_primitive(word32)
";
            Assert.AreEqual(exp, sb.ToString());
        }

        private TraitCollector CreateCollector()
        {
            return CreateCollector(CreateProgram());
        }

        private TraitCollector CreateCollector(Program program)
        {
            en = new ExpressionNormalizer(program.Architecture.PointerType);
            eqb = new EquivalenceClassBuilder(program.TypeFactory, program.TypeStore, new FakeDecompilerEventListener());
            handler = new TestTraitHandler(program.TypeStore);
            return new TraitCollector(program.TypeFactory, program.TypeStore, handler, program);
        }

        private ITraitHandler CreateHandler(TypeStore store)
        {
            return new TestTraitHandler(store);
        }

        protected override void RunTest(Program program, string outFile)
		{
            coll = CreateCollector(program);
            en.Transform(program);
            eqb.Build(program);
			coll.CollectProgramTraits(program);

			using (FileUnitTester fut = new FileUnitTester(outFile))
			{
				foreach (Procedure proc in program.Procedures.Values)
				{
					proc.Write(false, fut.TextWriter);
					fut.TextWriter.WriteLine();
				}
				handler.Traits.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		private void Verify(Program program, string outputFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFilename))
			{
				if (program is not null)
				{
					foreach (Procedure proc in program.Procedures.Values)
					{
						proc.Write(false, fut.TextWriter);
						fut.TextWriter.WriteLine();
					}
				}
				handler.Traits.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}

	public class IntelIndexedAddressingMode : ProcedureBuilder
	{
		protected override void BuildBody()
		{
            Identifier ds = Local16("ds");
			Identifier es = Local16("es");
			Identifier ax = Local16("ax");
			Identifier bx = Local16("bx");
			Identifier si = Local16("si");
            Assign(Frame.EnsureRegister(Architecture.StackRegister), Frame.FramePointer);
			Assign(es, SegMem(PrimitiveType.Word16, ds, Word16(0x7070)));
			Assign(ax, 0x4A);
			Assign(si, SMul(ax, SegMem(PrimitiveType.Word16, ds, Word16(0x1C0A))));
            Assign(bx, SegMem(PrimitiveType.Word16, ds, Word16(0x0CA4)));
			SStore(ds, IAdd(IAdd(bx, 10), si), Byte(0xF8));
			Return();
		}
	}

    public class TestTraitHandler : ITraitHandler
    {
        private readonly ITypeStore store;

        public TestTraitHandler(TypeStore store)
        {
            this.store = store;
            this.Traits = new TraitMapping(store);
        }

        public TraitMapping Traits { get; private set; }

        #region ITraitHandler Members

        public void ArrayTrait(TypeVariable tArray, int elementSize, int length)
        {
            Traits.AddTrait(tArray, new TraitArray(elementSize, length));
        }

        public void BuildEquivalenceClassDataTypes()
        {
        }

        private TypeVariable TypeVar(Expression e) => store.GetTypeVariable(e);

        public DataType DataTypeTrait(Expression exp, DataType p)
        {
            if (p is null)
                return null;
            Traits.AddTrait(TypeVar(exp), new TraitDataType(p));
            return p;
        }

        public DataType EqualTrait(Expression t1, Expression t2)
        {
            if (t1 is not null && t2 is not null)
                Traits.AddTrait(TypeVar(t1), new TraitEqual(TypeVar(t2)));
            return null;
        }

        public DataType FunctionTrait(Expression function, int funcPtrSize, TypeVariable ret, params TypeVariable[] actuals)
        {
            return Traits.AddTrait(TypeVar(function), new TraitFunc(TypeVar(function), funcPtrSize, ret, actuals));
        }

        public DataType MemAccessArrayTrait(Expression tBase, Expression tStruct, int structPtrSize, int offset, int elementSize, int length, Expression tAccess)
        {
            return Traits.AddTrait(TypeVar(tStruct), new TraitMemArray(tBase is not null ? TypeVar(tBase) : null, structPtrSize, offset, elementSize, length, TypeVar(tAccess)));
        }

        public DataType MemAccessTrait(Expression tBase, Expression tStruct, int structPtrSize, Expression tField, int offset)
        {
            return Traits.AddTrait(TypeVar(tStruct), new TraitMem(tBase is not null ? TypeVar(tBase) : null, structPtrSize, TypeVar(tField), offset));
        }

        public DataType MemFieldTrait(Expression tBase, Expression tStruct, Expression tField, int offset)
        {
            return Traits.AddTrait(TypeVar(tStruct), new TraitMem(tBase is not null ? TypeVar(tBase) : null, 0, TypeVar(tField), offset));
        }

        public DataType MemSizeTrait(Expression tBase, Expression tStruct, int size)
        {
            return Traits.AddTrait(TypeVar(tStruct), new TraitMemSize(size));
        }

        public DataType PointerTrait(Expression tPtr, int ptrSize, Expression tPointee)
        {
            return Traits.AddTrait(TypeVar(tPtr), new TraitPointer(TypeVar(tPointee)));
        }

        #endregion
    }
}

/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Analysis;
using Decompiler.Arch.Intel;
using Decompiler.Scanning;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TraitCollectorTests : TypingTestBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private MockTraitHandler handler;
		private TraitCollector coll;
		private ArrayExpressionNormalizer aen;
		private EquivalenceClassBuilder eqb;
		private readonly string nl;

		public TraitCollectorTests()
		{
			nl = Environment.NewLine;
		}

		[Test]
		public void TrcoFactorial()
		{
			RunTest("Fragments/factorial.asm", "Typing/TrcoFactorial.txt");
		}

		[Test]
		public void TrcoFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Typing/TrcoFactorialReg.txt");
		}

		[Test]
		public void TrcoFloatingPoint()
		{
			RunTest("Fragments/fpuops.asm", "Typing/TrcoFloatingPoint.txt");
		}

		[Test]
		public void TrcoLength()
		{
			RunTest("Fragments/Type/listlength.asm", "Typing/TrcoLength.txt");
		}

		[Test]
		public void TrcoNestedStructs()
		{
			RunTest("Fragments/type/nestedstructs.asm", "Typing/TrcoNestedStructs.txt");
		}

		[Test]
		public void TrcoSimpleLinearCode()
		{
			RunTest("Fragments/simple_memoperations.asm", "Typing/TrcoSimpleLinearCode.txt");
		}

		[Test]
		public void TrcoUnknown()
		{
			RunTest("Fragments/Type/unknown.asm", "Typing/TrcoUnknown.txt");
		}

		[Test]
		public void TrcoMemAccesses()
		{
			RunTest("fragments/multiple/memaccesses.asm", "Typing/TrcoMemAccesses.txt");
		}


		[Test]
		public void TrcoCmpMock()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new CmpMock());
			Program prog = mock.BuildProgram();
			eqb.Build(prog);
			coll = new TraitCollector(factory, store, handler, prog.Globals, ivs);
			coll.CollectProgramTraits(prog);

			Verify(prog, "Typing/TrcoCmpMock.txt");
		}

		[Test]
		public void TrcoStaggeredArraysMock()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new StaggeredArraysMock());
			Program prog = mock.BuildProgram();
			aen.Transform(prog);
			eqb.Build(prog);
			coll = new TraitCollector(factory, store, handler, prog.Globals, ivs);
			coll.CollectProgramTraits(prog);

			Verify(prog, "Typing/TrcoStaggeredArraysMock.txt");
		}

		[Test]
		public void TrcoArrayExpression()
		{
			coll = new TraitCollector(factory, store, handler, null, ivs);
			Identifier b = new Identifier("base", 0, PrimitiveType.Word32, null);
			Identifier i = new Identifier("idx", 1, PrimitiveType.Word32, null);
			Constant s = Constant.Word32(4);

			ProcedureMock m = new ProcedureMock();

			// e ::= Mem[(b+0x1003000)+(i*s):word16]
			Expression e = m.Load(
				PrimitiveType.Word16,
				m.Add(m.Add(b, Constant.Word32(0x10030000)),
				m.Muls(i, s)));
			coll.Procedure = new Procedure("foo", null);
			e = e.Accept(aen);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoArrayExpression.txt");
		}

		[Test]
		public void TrcoInductionVariable()
		{
			Identifier globals = new Identifier("globals", 0, PrimitiveType.Pointer, null);

			coll = new TraitCollector(factory, store, handler, globals, ivs);
		
			Identifier i = new Identifier("i", 0, PrimitiveType.Word32, null);
			MemoryAccess load = new MemoryAccess(MemoryIdentifier.GlobalMemory, i, PrimitiveType.Int32);
			Identifier i2 = new Identifier("i2", 1, PrimitiveType.Word32, null);
			MemoryAccess ld2 = new MemoryAccess(MemoryIdentifier.GlobalMemory, i2, PrimitiveType.Int32);
			Procedure proc = new Procedure("foo", null);
			coll.Procedure = proc;

			LinearInductionVariable iv = new LinearInductionVariable(
				Constant.Word32(0), 
				Constant.Word32(1),
				Constant.Word32(10));
			LinearInductionVariable iv2 = new LinearInductionVariable(
				Constant.Word32(0x0010000),
				Constant.Word32(4),
				Constant.Word32(0x0010040));

			ivs.Add(proc, i, iv);
			ivs.Add(proc, i2, iv2);

			
			globals.Accept(eqb);
			load.Accept(eqb);
			ld2.Accept(eqb);
			globals.Accept(coll);
			load.Accept(coll);
			ld2.Accept(coll);
			Verify(null, "Typing/TrcoInductionVariable.txt");
		}

		[Test]
		public void TrcoGlobalArray()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier globals = m.Local32("globals");
			Identifier i = m.Local32("i");
			Expression ea = m.Add(globals, m.Add(m.Shl(i, 2), 0x3000));
			Expression e = m.Load(PrimitiveType.Int32, ea);
			coll = new TraitCollector(factory, store, handler, globals, ivs);
			coll.Procedure = new Procedure("foo", null);
			e = e.Accept(aen);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoGlobalArray.txt");
		}

		[Test]
		public void TrcoMemberPointer()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier globals = m.Local32("globals");
			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			Identifier ax = m.Local16("ax");
			MemberPointerSelector mps = m.MembPtr(ds, m.Add(bx, 4));
			Expression e = m.Load(PrimitiveType.Byte, mps);

			coll = new TraitCollector(factory, store, handler, globals, ivs);
			coll.Procedure = new Procedure("foo", null);
			e = e.Accept(aen);
			e.Accept(eqb);
			e.Accept(coll);
			Assert.IsNotNull(mps.Ptr.TypeVariable, "Base pointer should have type variable");
			Verify(null, "Typing/TrcoMemberPointer.txt");
		}

		[Test]
		public void TrcoSegmentedAccess()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier globals = m.Local32("globals");
			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			Identifier ax = m.Local16("ax");
			Expression e = m.SegMem(PrimitiveType.Word16, ds, m.Add(bx, 4));

			coll = new TraitCollector(factory, store, handler, globals, ivs);
			coll.Procedure = new Procedure("foo", null);
			e = e.Accept(aen);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegmentedAccess.txt");
		}

		[Test]
		public void TrcoSegmentedDirectAddress()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier globals = m.Local32("globals");
			store.EnsureTypeVariable(factory, globals);

			Identifier ds = m.Local16("ds");
			Expression e = m.SegMem(PrimitiveType.Byte, ds, m.Int16(0x0200));
			
			coll = new TraitCollector(factory, store, handler, globals, ivs);
			coll.Procedure = new Procedure("foo", null);
			e = e.Accept(aen);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegmentedDirectAddress.txt");
		}

		[Test]
		public void TrcoPtrPtrInt()
		{
			ProgramMock p = new ProgramMock();
			p.Add(new PtrPtrIntMock());
			RunTest(p.BuildProgram(), "Typing/TrcoPtrPtrInt.txt");
		}

		[Test]
		public void TrcoFnPointerMock()
		{
			ProgramMock p = new ProgramMock();
			p.Add(new FnPointerMock());
			RunTest(p.BuildProgram(), "Typing/TrcoFnPointerMock.txt");
		}

		[Test]
		public void TrcoSegmentedMemoryPointer()
		{
			ProgramMock p = new ProgramMock();
			p.Add(new SegmentedMemoryPointerMock());
			RunTest(p.BuildProgram(), "Typing/TrcoSegmentedMemoryPointer.txt");
		}

		[Test]
		public void TrcoReg00008()
		{
			RunTest("fragments/regressions/r00008.asm", "Typing/TrcoReg00008.txt");
		}

		[Test]
		public void TrcoIntelIndexedAddressingMode()
		{
			ProgramMock m = new ProgramMock();
			m.Add(new IntelIndexedAddressingMode());
			prog = m.BuildProgram();
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerHost());
			dfa.AnalyzeProgram();
			RunTest(prog, "Typing/TrcoIntelIndexedAddressingMode.txt");
		}

		[Test]
		public void TrcoTreeFind()
		{
			ProgramMock m = new ProgramMock();
			m.Add(new TreeFindMock());
			prog = m.BuildProgram();
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerHost());
			dfa.AnalyzeProgram();
			RunTest(prog, "Typing/TrcoTreeFind.txt");
		}

		[Test]
		public void TrcoSegmentedDoubleReference()
		{
			ProgramMock m = new ProgramMock();
			m.Add(new SegmentedDoubleReferenceMock());
			RunTest(m.BuildProgram(), "Typing/TrcoSegmentedDoubleReference.txt");
		}

		[Test]
		public void TrcoIcall()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier pfn = m.Local32("pfn");
			Expression l = m.Load(PrimitiveType.Word32, pfn);
			IndirectCall icall = new IndirectCall(l, 0, 0);
			
			coll = new TraitCollector(factory, store, handler, null, ivs);
			icall.Accept(eqb);
			icall.Accept(coll);
			StringWriter sw = new StringWriter();
			handler.Traits.Write(sw);
			string exp =
				"T_1 (in pfn)" + nl +
				"\ttrait_primitive(word32)" + nl +
				"\ttrait_mem(T_2, 0)" + nl + 
				"T_2 (in Mem0[pfn:word32])" + nl +
				"\ttrait_primitive(word32)" + nl +
				"\ttrait_func( -> )" + nl;
			Assert.AreEqual(exp, sw.ToString());
		}

		[Test]
		public void TrcoSegMem()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier ds = m.Local16("ds");
			Expression e = m.SegMemW(ds, m.Word16(0xC002U));
			
			coll = new TraitCollector(factory, store, handler, null, null);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegMem.txt");
		}

		[Test]
		public void TrcoUnsignedCompare()
		{
			ProcedureMock m = new ProcedureMock();
			Identifier ds = m.Local16("ds");
			Expression e = m.Uge(ds, m.Word16(0x0800));

			coll = new TraitCollector(factory, store, handler, null, null);
			e.Accept(eqb);
			e.Accept(coll);
			StringWriter sb = new StringWriter();
			handler.Traits.Write(sb);
			Console.WriteLine(sb);
			string exp = 
				"T_1 (in ds)" + nl +
				"\ttrait_primitive(word16)" + nl +
				"\ttrait_equal(T_2)" + nl +
				"\ttrait_primitive(ups16)" + nl +
				"T_2 (in 0x0800)" + nl +
				"\ttrait_primitive(word16)" + nl +
				"\ttrait_primitive(ups16)" + nl +
				"T_3 (in ds >=u 0x0800)" + nl +
				"\ttrait_primitive(bool)" + nl;
			Assert.AreEqual(exp, sb.ToString());
		}

		[SetUp]
		public void SetUp()
		{
			factory = new TypeFactory();
			store = new TypeStore();
			handler = new MockTraitHandler();
			ivs = new InductionVariableCollection();
			aen = new ArrayExpressionNormalizer();
			eqb = new EquivalenceClassBuilder(factory, new TypeStore());
		}

		private void RunTest(Program prog, string outFile)
		{
			eqb.Build(prog);
			coll = new TraitCollector(factory, store, handler, prog.Globals, ivs);
			coll.CollectProgramTraits(prog);

			using (FileUnitTester fut = new FileUnitTester(outFile))
			{
				foreach (Procedure proc in prog.Procedures.Values)
				{
					proc.Write(false, fut.TextWriter);
					fut.TextWriter.WriteLine();
				}
				handler.Traits.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		private void RunTest(string sourceFile, string outFile)
		{
			RewriteFile(sourceFile);
			Assert.IsNotNull(prog);
			RunTest(prog, outFile);
		}

		private void Verify(Program prog, string outputFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFilename))
			{
				if (prog != null)
				{
					foreach (Procedure proc in prog.Procedures.Values)
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

	public class IntelIndexedAddressingMode : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier es = Local16("es");
			Identifier ax = Local16("ax");
			Identifier bx = Local16("bx");
			Identifier si = Local16("si");
			Assign(es, Load(PrimitiveType.Word16, Int16(0x7070)));
			Assign(ax, 0x4A);
			Assign(si, Muls(ax, Load(PrimitiveType.Word16, Int16(0x1C0A))));
			Load(bx, Int16(0x0CA4));
			Store(Add(Add(bx, 10), si), Int8(0xF8));
			Return();
		}
	}

}

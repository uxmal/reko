/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class DerivedPointerAnalysisTests
	{
		private TypeStore store;
		private TypeFactory factory;

		[Test]
		public void CpfSimple()
		{
			Program prog = new Program();
            prog.Architecture = new ArchitectureMock();

			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store);
			DataTypeBuilder dtb = new DataTypeBuilder(factory, store, prog.Architecture);

			Constant c = Constant.Word32(0x10000000);
			MemoryAccess mem = new MemoryAccess(c, PrimitiveType.Real32);

			prog.Globals.Accept(eqb);
			mem.Accept(eqb);

			TraitCollector tc = new TraitCollector(factory, store, dtb, prog);
			prog.Globals.Accept(tc);
			mem.Accept(tc);
			dtb.BuildEquivalenceClassDataTypes();

			DerivedPointerAnalysis cf = new DerivedPointerAnalysis(factory, store, dtb, prog.Architecture);
			mem.Accept(cf);

			Verify(null, "Typing/CpfSimple.txt");
		}

		[Test]
		public void DpaGlobalVariables()
		{
			ProgramMock prog = new ProgramMock();
			prog.Add(new GlobalVariablesMock());
			RunTest(prog.BuildProgram(), "Typing/DpaGlobalVariables.txt");
		}

		[Test]
		public void DpaConstantPointer()
		{
			ProgramMock prog = new ProgramMock();
			ProcedureMock m = new ProcedureMock();
			Identifier r1 = m.Register(1);
			m.Assign(r1, 0x123130);
			m.Store(r1, m.Int32(0x42));
			prog.Add(m);

			RunTest(prog.BuildProgram(), "Typing/DpaConstantPointer.txt");
		}

		[Test]
		public void DpaConstantMemberPointer()
		{
			ProgramMock prog = new ProgramMock();
			ProcedureMock m = new ProcedureMock();
			Identifier ds = m.Local16("ds");
			ds.DataType = PrimitiveType.SegmentSelector;
			Identifier bx = m.Local16("bx");

			m.Assign(bx, 0x1234);
			m.Store(m.SegMemW(ds, bx), m.Int16(0x0042));
			prog.Add(m);

			RunTest(prog.BuildProgram(), "Typing/DpaConstantMemberPointer.txt");
		}

		private void RunTest(Program prog, string outputFile)
		{
			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store);
			DataTypeBuilder dtb = new DataTypeBuilder(factory, store, prog.Architecture);
			eqb.Build(prog);
            TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
			trco.CollectProgramTraits(prog);
			dtb.BuildEquivalenceClassDataTypes();

			DerivedPointerAnalysis dpa = new DerivedPointerAnalysis(factory, store, dtb, prog.Architecture);
			dpa.FollowConstantPointers(prog);

			Verify(null, outputFile);
		}

		[SetUp]
		public void Setup()
		{
			store = new TypeStore();
			factory = new TypeFactory();
		}

		private void Verify(Program prog, string outputFile)
		{
			store.CopyClassDataTypesToTypeVariables();
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				if (prog != null)
				{
					foreach (Procedure proc in prog.Procedures.Values)
					{
						proc.Write(false, fut.TextWriter);
						fut.TextWriter.WriteLine();
					}
				}
				store.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}

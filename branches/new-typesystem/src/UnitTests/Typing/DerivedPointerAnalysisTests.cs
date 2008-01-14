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

			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store);
			DataTypeBuilder dtb = new DataTypeBuilder(factory, store);

			Identifier globals = new Identifier("globals", 0, PrimitiveType.Pointer, null);
			Constant c = new Constant(PrimitiveType.Word32,0x10000000);
			MemoryAccess mem = new MemoryAccess(c, PrimitiveType.Real32);

			globals.Accept(eqb);
			mem.Accept(eqb);

			TraitCollector tc = new TraitCollector(factory, store, dtb, globals, new InductionVariableCollection());
			tc.Procedure = new Procedure("foo", null);
			globals.Accept(tc);
			mem.Accept(tc);
			dtb.BuildEquivalenceClassDataTypes();

			DerivedPointerAnalysis cf = new DerivedPointerAnalysis(factory, store, dtb);
			mem.Accept(cf);

			Verify("Typing/CpfSimple.txt");
		}

		[Test]
		public void DpaGlobalVariables()
		{
			ProgramMock prog = new ProgramMock();
			prog.Add(new GlobalVariablesMock());
			RunTest(prog.BuildProgram(), "Typing/DpaGlobalVariables.txt");
		}

		private void RunTest(Program prog, string outputFile)
		{
			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store);
			DataTypeBuilder dtb = new DataTypeBuilder(factory, store);
			eqb.Build(prog);
			TraitCollector trco = new TraitCollector(factory, store, dtb, prog.Globals, new InductionVariableCollection());
			trco.CollectProgramTraits(prog);
			dtb.BuildEquivalenceClassDataTypes();

			DerivedPointerAnalysis dpa = new DerivedPointerAnalysis(factory, store, dtb);
			dpa.FollowConstantPointers(prog);

			Verify(outputFile);
		}

		[SetUp]
		public void Setup()
		{
			store = new TypeStore();
			factory = new TypeFactory();
		}

		private void Verify(string outputFile)
		{
			store.CopyClassDataTypesToTypeVariables();
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				store.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}

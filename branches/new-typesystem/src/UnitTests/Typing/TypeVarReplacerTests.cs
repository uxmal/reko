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
	public class TypeVarReplacerTests : TypingTestBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private EquivalenceClassBuilder eqb;
		private DataTypeBuilder dtb;
		private TraitCollector trco;

		[Test]
		public void TvrReplaceInMem()
		{
			Identifier id1 = new Identifier("pptr", 1, PrimitiveType.Word32, null);
			Identifier id2 = new Identifier("ptr", 2, PrimitiveType.Word32, null);
			Identifier id3 = new Identifier("v", 3, PrimitiveType.Word32, null);
			Assignment ass1 = new Assignment(id2, MemLoad(id1, 0, PrimitiveType.Word32));
			Assignment ass2 = new Assignment(id3, MemLoad(id2, 0, PrimitiveType.Word32));
			eqb.VisitAssignment(ass1);
			eqb.VisitAssignment(ass2);

			trco = new TraitCollector(factory, store, dtb, new Identifier("globals", 0, PrimitiveType.Pointer, new MemoryStorage()), ivs);
			trco.Procedure = new Procedure("foo", null);
			trco.VisitAssignment(ass1);
			trco.VisitAssignment(ass2);
			dtb.BuildEquivalenceClassDataTypes();

			TypeVariableReplacer tvr = new TypeVariableReplacer(store);
			tvr.ReplaceTypeVariables();
			Verify("Typing/TvrReplaceInMem.txt");
		}

		private void RunTest(Program prog, string outputFilename)
		{
			eqb.Build(prog);
			trco = new TraitCollector(factory, store, dtb, prog.Globals, ivs);
			trco.CollectProgramTraits(prog);
			dtb.BuildEquivalenceClassDataTypes();

			store.CopyClassDataTypesToTypeVariables();
			TypeVariableReplacer tvr = new TypeVariableReplacer(store);
			tvr.ReplaceTypeVariables();

			Verify(outputFilename);
		}



		[SetUp]
		public void Setup()
		{
			factory = new TypeFactory();
			store = new TypeStore();
			ivs = new InductionVariableCollection();
			eqb = new EquivalenceClassBuilder(factory, store);
			dtb = new DataTypeBuilder(factory, store);
		}

		private void Verify(string outputFilename)
		{
			store.CopyClassDataTypesToTypeVariables();
			using (FileUnitTester fut = new FileUnitTester(outputFilename))
			{
				store.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}

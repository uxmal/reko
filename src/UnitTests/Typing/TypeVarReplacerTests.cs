#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Typing
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
			var id1 = new Identifier("pptr", PrimitiveType.Word32, null);
			var id2 = new Identifier("ptr", PrimitiveType.Word32, null);
			var id3 = new Identifier("v", PrimitiveType.Word32, null);
			var ass1 = new Assignment(id2, MemLoad(id1, 0, PrimitiveType.Word32));
			var ass2 = new Assignment(id3, MemLoad(id2, 0, PrimitiveType.Word32));
			eqb.VisitAssignment(ass1);
			eqb.VisitAssignment(ass2);

            var program = new Program();
            program.Architecture = new FakeArchitecture();
            program.Platform = new DefaultPlatform(null, program.Architecture);
            trco = new TraitCollector(factory, store, dtb, program);
			trco.VisitAssignment(ass1);
			trco.VisitAssignment(ass2);
			dtb.BuildEquivalenceClassDataTypes();

			var tvr = new TypeVariableReplacer(store);
			tvr.ReplaceTypeVariables();
			Verify("Typing/TvrReplaceInMem.txt");
		}

		protected override void RunTest(Program program, string outputFilename)
		{
			eqb.Build(program);
			trco = new TraitCollector(factory, store, dtb, program);
			trco.CollectProgramTraits(program);
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
			eqb = new EquivalenceClassBuilder(factory, store, new FakeDecompilerEventListener());
            var platform = new DefaultPlatform(null, new FakeArchitecture());
			dtb = new DataTypeBuilder(factory, store, platform);
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

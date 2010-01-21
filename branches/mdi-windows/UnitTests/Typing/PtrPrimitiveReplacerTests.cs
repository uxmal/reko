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
using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using System;
using NUnit.Framework;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class PtrPrimitiveReplacerTests
	{
		private TypeStore store;

		[Test]
		public void PprReplaceInts()
		{
			TypeFactory factory = new TypeFactory();
			store = new TypeStore();
			TypeVariable tv1 = store.EnsureExpressionTypeVariable(factory, null);
			TypeVariable tv2 = store.EnsureExpressionTypeVariable(factory, null);
			Assert.IsNotNull(tv1.Class, "Expected store.EnsureTypeVariable to create equivalence class");
			Assert.IsNotNull(tv2.Class, "Expected store.EnsureTypeVariable to create equivalence class");
			tv1.Class.DataType = PrimitiveType.Word32;
			tv2.Class.DataType = PrimitiveType.Word16;

			TypeVariable tv3 = store.EnsureExpressionTypeVariable(factory, null);
			Assert.IsNotNull(tv3.Class, "Expected store.EnsureTypeVariable to create equivalence class");

			StructureType mem = factory.CreateStructureType(null, 0);
			mem.Fields.Add(new StructureField(0, tv1));
			mem.Fields.Add(new StructureField(4, tv2));
			tv3.Class.DataType = factory.CreatePointer(mem, 4);

			store.CopyClassDataTypesToTypeVariables();
			TypeVariableReplacer tvr = new TypeVariableReplacer(store);
			tvr.ReplaceTypeVariables();

			PtrPrimitiveReplacer ppr = new PtrPrimitiveReplacer(factory, store);

			ppr.ReplaceAll();

			Verify(null, "Typing/PprReplaceInts.txt");
		}

		[Test]
		public void PprPtrPtrInt()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new PtrPtrIntMock());
			RunTest(mock.BuildProgram(), "Typing/PprPtrPtrInt.txt");
		}

		[Test]
		public void PprUnionIntReal()
		{
			ProgramMock mock = new ProgramMock();
			mock.Add(new UnionIntRealMock());
			RunTest(mock.BuildProgram(), "Typing/PprUnionIntReal.txt");
		}

		[Test]
		public void PprMemberVars()
		{
			ProgramMock mock = new ProgramMock();
			ProcedureMock p = new ProcedureMock();
			Identifier cs = p.Frame.EnsureRegister(Registers.cs);
			p.Store(p.SegMemW(cs, p.Word32(0x0001)), new Constant(PrimitiveType.SegmentSelector, 0x0800));
			p.Procedure.RenumberBlocks();
			mock.Add(p);
			RunTest(mock.BuildProgram(), "Typing/PprMemberVars.txt");
		}

        [Test]
        public void PprMemberPointers()
        {
            ProgramMock mock = new ProgramMock();
            ProcedureMock m = new ProcedureMock();
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            m.SegStoreW(ds, m.Word32(7000), m.SegMemW(ds, m.SegMemW(ds, m.Word32(0x5321))));
            mock.Add(m);
            RunTest(mock.BuildProgram(), "typing/PprMemberPointers.txt");
        }

		private void RunTest(Program prog, string outputFilename)
		{
			TypeFactory factory = new TypeFactory();
			store = new TypeStore();
			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store);
			eqb.Build(prog);
			DataTypeBuilder dtb = new DataTypeBuilder(factory, store, prog.Architecture);
			TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
			trco.CollectProgramTraits(prog);
			dtb.BuildEquivalenceClassDataTypes();

			store.CopyClassDataTypesToTypeVariables();
			TypeVariableReplacer tvr = new TypeVariableReplacer(store);
			tvr.ReplaceTypeVariables();

			PtrPrimitiveReplacer ppr = new PtrPrimitiveReplacer(factory, store);
			ppr.ReplaceAll();

			Verify(prog, outputFilename);
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
				store.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}

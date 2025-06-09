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

using Reko.Analysis;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using System;
using NUnit.Framework;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Decompiler.Typing
{
	[TestFixture]
	public class PtrPrimitiveReplacerTests : TypingTestBase
	{
		private TypeStore store;

        protected override void RunTest(Program program, string outputFilename)
		{
            var listener = new FakeDecompilerEventListener();
			TypeFactory factory = new TypeFactory();
			store = new TypeStore();
			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store, listener);
			eqb.Build(program);
			DataTypeBuilder dtb = new DataTypeBuilder(factory, store, program.Platform);
			TraitCollector trco = new TraitCollector(factory, store, dtb, program);
			trco.CollectProgramTraits(program);
			dtb.BuildEquivalenceClassDataTypes();

			store.CopyClassDataTypesToTypeVariables();
			TypeVariableReplacer tvr = new TypeVariableReplacer(store);
			tvr.ReplaceTypeVariables();

			PtrPrimitiveReplacer ppr = new PtrPrimitiveReplacer(factory, store, program, listener);
			ppr.ReplaceAll();

			Verify(program, outputFilename);
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
				store.Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void PprReplaceInts()
		{
            var arch = new FakeArchitecture(new ServiceContainer()); 
            var program = new Program { Architecture = arch, Platform = new DefaultPlatform(null, arch) };

			TypeFactory factory = new TypeFactory();
			store = new TypeStore();
			TypeVariable tv1 = store.CreateTypeVariable(factory);
            TypeVariable tv2 = store.CreateTypeVariable(factory);
			Assert.IsNotNull(tv1.Class, "Expected store.EnsureTypeVariable to create equivalence class");
			Assert.IsNotNull(tv2.Class, "Expected store.EnsureTypeVariable to create equivalence class");
			tv1.Class.DataType = PrimitiveType.Word32;
			tv2.Class.DataType = PrimitiveType.Word16;
            store.SetTypeVariable(program.Globals, store.CreateTypeVariable(factory));
            program.Globals.DataType = factory.CreateStructureType();

            TypeVariable tv3 = store.CreateTypeVariable(factory);
			Assert.IsNotNull(tv3.Class, "Expected store.EnsureTypeVariable to create equivalence class");

			StructureType mem = factory.CreateStructureType(null, 0);
			mem.Fields.Add(0, tv1);
			mem.Fields.Add(4, tv2);
			tv3.Class.DataType = factory.CreatePointer(mem, 32);

			store.CopyClassDataTypesToTypeVariables();
			TypeVariableReplacer tvr = new TypeVariableReplacer(store);
			tvr.ReplaceTypeVariables();

            var ppr = new PtrPrimitiveReplacer(factory, store, program, new FakeDecompilerEventListener());
			ppr.ReplaceAll();

			Verify(null, "Typing/PprReplaceInts.txt");
		}

		[Test]
		public void PprPtrPtrInt()
		{
			ProgramBuilder mock = new ProgramBuilder();
			mock.Add(new PtrPtrIntMock());
			RunTest(mock.BuildProgram(), "Typing/PprPtrPtrInt.txt");
		}

		[Test]
		public void PprUnionIntReal()
		{
			ProgramBuilder mock = new ProgramBuilder();
			mock.Add(new UnionIntRealMock());
			RunTest(mock.BuildProgram(), "Typing/PprUnionIntReal.txt");
		}

		[Test]
		public void PprMemberVars()
		{
			ProgramBuilder mock = new ProgramBuilder();
			ProcedureBuilder p = new ProcedureBuilder();
			Identifier cs = p.Frame.EnsureRegister(Registers.cs);
			p.Store(p.SegMem16(cs, p.Word32(0x0001)), Constant.Create(PrimitiveType.SegmentSelector, 0x0800));
			mock.Add(p);
			RunTest(mock.BuildProgram(), "Typing/PprMemberVars.txt");
		}

        [Test]
        public void PprMemberPointers()
        {
            ProgramBuilder mock = new ProgramBuilder();
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            m.SStore(ds, m.Word32(7000), m.SegMem16(ds, m.SegMem16(ds, m.Word32(0x5321))));
            mock.Add(m);
            RunTest(mock.BuildProgram(), "Typing/PprMemberPointers.txt");
        }

        [Test]
        public void PprRecursiveStructs()
        {
            StructureType s1 = new StructureType(null, 0, true);
            StructureType s2 = new StructureType(null, 0, true);
            s1.Fields.Add(0, new Pointer(s2, 32));
            s2.Fields.Add(0, new Pointer(s1, 32));

            var program = new Program();
            var factory = new TypeFactory();
            var store = new TypeStore();

            var ppr = new PtrPrimitiveReplacer(factory, store, program, new FakeDecompilerEventListener());

            var sExp = "(struct (0 (ptr32 (struct (0 (ptr32 (struct)) ptr0000))) ptr0000))";

            Assert.AreEqual(sExp, ppr.Replace(s1).ToString());
            Assert.AreEqual(sExp, ppr.Replace(s2).ToString());
        }
	}
}

#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Typing
{
	[TestFixture]
	public class ConstantPointerAnalysisTests
	{
		private TypeStore store;
		private TypeFactory factory;

        [SetUp]
        public void Setup()
        {
            store = new TypeStore();
            factory = new TypeFactory();
        }

        private void RunTest(Program prog, string outputFile)
        {
            EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store);
            DataTypeBuilder dtb = new DataTypeBuilder(factory, store, prog.Platform);
            eqb.Build(prog);
            TraitCollector trco = new TraitCollector(factory, store, dtb, prog);
            trco.CollectProgramTraits(prog);
            dtb.BuildEquivalenceClassDataTypes();
            var tv = new TypeVariableReplacer(store);
            tv.ReplaceTypeVariables();
            store.CopyClassDataTypesToTypeVariables();
            var ppr = new PtrPrimitiveReplacer(factory, store, prog);
            ppr.ReplaceAll();

            var cpa = new ConstantPointerAnalysis(factory, store, prog);
            cpa.FollowConstantPointers();

            Verify(null, outputFile);
        }

		[Test]
		public void CpaSimple()
		{
			var prog = new ProgramBuilder();
            prog.Add("test", m=>
               {
                   var r1 = m.Register(1);
                   m.Assign(r1, m.Load(PrimitiveType.Real32, m.Word32(0x10000000)));
               });
			RunTest(prog.BuildProgram(), "Typing/CpaSimple.txt");
		}

		[Test]
		public void CpaGlobalVariables()
		{
			ProgramBuilder prog = new ProgramBuilder();
			prog.Add(new GlobalVariablesMock());
			RunTest(prog.BuildProgram(), "Typing/CpaGlobalVariables.txt");
		}

		[Test]
		public void CpaConstantPointer()
		{
			ProgramBuilder prog = new ProgramBuilder();
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier r1 = m.Register(1);
			m.Assign(r1, 0x123130);
			m.Store(r1, m.Int32(0x42));
			prog.Add(m);

			RunTest(prog.BuildProgram(), "Typing/CpaConstantPointer.txt");
		}

		[Test]
		public void CpaConstantMemberPointer()
		{
			ProgramBuilder prog = new ProgramBuilder();
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			ds.DataType = PrimitiveType.SegmentSelector;
			Identifier bx = m.Local16("bx");

			m.Assign(bx, 0x1234);
			m.Store(m.SegMemW(ds, bx), m.Int16(0x0042));
			prog.Add(m);

			RunTest(prog.BuildProgram(), "Typing/CpaConstantMemberPointer.txt");
		}


		private void Verify(Program prog, string outputFile)
		{
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

        [Test(Description = "If the data type at a particular offset is compatible with an array, it's considered part of the array.")]
        public void CpaPointerToArray()
        {
            var cpa = new ConstantPointerAnalysis(factory, store, new Program());
            var isInside = cpa.IsInsideArray(
                new StructureType { Fields = { new StructureField(300, new ArrayType(PrimitiveType.Int32, 0)) } },
                304,
                PrimitiveType.Int32);
            Assert.IsTrue(isInside, "Since the array has no specified size, offset 304 should be inside the array.");
        }

    }
}

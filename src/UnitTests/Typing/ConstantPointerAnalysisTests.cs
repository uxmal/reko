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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;

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

        private void RunTest(Program program, string outputFile)
        {
            var listener = new FakeDecompilerEventListener();
            EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store, listener);
            DataTypeBuilder dtb = new DataTypeBuilder(factory, store, program.Platform);
            eqb.Build(program);
            TraitCollector trco = new TraitCollector(factory, store, dtb, program);
            trco.CollectProgramTraits(program);
            dtb.BuildEquivalenceClassDataTypes();
            var tv = new TypeVariableReplacer(store);
            tv.ReplaceTypeVariables();
            store.CopyClassDataTypesToTypeVariables();
            var ppr = new PtrPrimitiveReplacer(factory, store, program);
            ppr.ReplaceAll(listener);

            var cpa = new ConstantPointerAnalysis(factory, store, program);
            cpa.FollowConstantPointers();

            Verify(null, outputFile);
        }

		[Test]
		public void CpaSimple()
		{
			var program = new ProgramBuilder();
            program.Add("test", m =>
            {
                var r1 = m.Register("r1");
                m.Assign(r1, m.Mem(PrimitiveType.Real32, m.Word32(0x10000000)));
            });
			RunTest(program.BuildProgram(), "Typing/CpaSimple.txt");
		}

		[Test]
		public void CpaGlobalVariables()
		{
			ProgramBuilder program = new ProgramBuilder();
			program.Add(new GlobalVariablesMock());
			RunTest(program.BuildProgram(), "Typing/CpaGlobalVariables.txt");
		}

		[Test]
		public void CpaConstantPointer()
		{
			ProgramBuilder program = new ProgramBuilder();
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier r1 = m.Register("r1");
			m.Assign(r1, 0x123130);
			m.MStore(r1, m.Word32(0x42));
			program.Add(m);

			RunTest(program.BuildProgram(), "Typing/CpaConstantPointer.txt");
		}

		[Test]
		public void CpaConstantMemberPointer()
		{
			ProgramBuilder program = new ProgramBuilder();
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			ds.DataType = PrimitiveType.SegmentSelector;
			Identifier bx = m.Local16("bx");

			m.Assign(bx, 0x1234);
			m.Store(m.SegMem16(ds, bx), m.Word16(0x0042));
			program.Add(m);

			RunTest(program.BuildProgram(), "Typing/CpaConstantMemberPointer.txt");
		}


		private void Verify(Program program, string outputFile)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				if (program != null)
				{
					foreach (Procedure proc in program.Procedures.Values)
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

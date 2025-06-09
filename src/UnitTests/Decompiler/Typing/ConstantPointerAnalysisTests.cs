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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using System;

namespace Reko.UnitTests.Decompiler.Typing
{
    [TestFixture]
	public class ConstantPointerAnalysisTests
	{
		private TypeStore store;
		private TypeFactory factory;
        private ProgramBuilder pb;
        private Program program;

        [SetUp]
        public void Setup()
        {
            store = new TypeStore();
            factory = new TypeFactory();
            pb = new ProgramBuilder();
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
            var ppr = new PtrPrimitiveReplacer(factory, store, program, listener);
            ppr.ReplaceAll();

            var cpa = new ConstantPointerAnalysis(factory, store, program);
            cpa.FollowConstantPointers();

            Verify(null, outputFile);
        }

        private Expression EnsureTypeVariable(Expression e, DataType dt)
        {
            var tv = store.EnsureExpressionTypeVariable(factory, null, e);
            tv.DataType = dt;
            return e;
        }

        private Expression Ptr32ToInt32(ProcedureBuilder m, uint ptr)
        {
            var dt = new Pointer(PrimitiveType.Int32, 32);
            return EnsureTypeVariable(m.Word32(ptr), dt);
        }

        private Expression Word32(ProcedureBuilder m, uint n)
        {
            return EnsureTypeVariable(m.Word32(n), PrimitiveType.Word32);
        }

        private Procedure Given_Procedure(string name, Action<ProcedureBuilder> builder)
        {
            return pb.Add(name, builder);
        }

        private void Given_Program(StructureFieldCollection globalVariables)
        {
            this.program = pb.BuildProgram();
            var tvGlobals = store.CreateTypeVariable(factory);
            store.SetTypeVariable(program.Globals, tvGlobals);
            var globalStructure = new StructureType();
            globalStructure.Fields.AddRange(globalVariables);
            tvGlobals.Class.DataType = globalStructure;
        }

        private void When_RunConstantPointerAnalysis()
        {
            var cpa = new ConstantPointerAnalysis(factory, store, program);
            cpa.FollowConstantPointers();
        }

        private void AssertGlobalVariables(string expected)
        {
            var eqGlobals = store.GetTypeVariable(program.Globals).Class;
            var strGlobals = eqGlobals.ResolveAs<StructureType>();
            var actual = strGlobals.ToString();
            if (actual != expected)
            {
                Console.WriteLine(actual);
            }
            Assert.AreEqual(expected, actual);
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

        [Test(Description = "If the data type at a particular Offset is compatible with an array, it's considered part of the array.")]
        public void CpaPointerToArray()
        {
            var cpa = new ConstantPointerAnalysis(factory, store, new Program());
            var isInside = cpa.IsInsideArray(
                new StructureType { Fields = { new StructureField(300, new ArrayType(PrimitiveType.Int32, 0)) } },
                304,
                PrimitiveType.Int32);
            Assert.IsTrue(isInside, "Since the array has no specified size, Offset 304 should be inside the array.");
        }

        [Test(Description = "If the data pointer is inside structure, it should be added to globals.")]
        public void CpaPointerToGlobalStructureField()
        {
            Given_Procedure("proc", m =>
            {
                m.MStore(Ptr32ToInt32(m, 0x123130), Word32(m, 1));
                m.MStore(Ptr32ToInt32(m, 0x123134), Word32(m, 2));
                m.MStore(Ptr32ToInt32(m, 0x123138), Word32(m, 3));
            });
            var strFieldType = new StructureType
            {
                Fields =
                {
                    { 0, PrimitiveType.Int32 },
                    { 4, PrimitiveType.Real32 },
                    { 8, PrimitiveType.Int32 },
                }
            };
            var globalVariables = new StructureFieldCollection
            {
                { 0x123130, strFieldType }
            };
            Given_Program(globalVariables);

            When_RunConstantPointerAnalysis();

            var expected =
                "(struct (123130 (struct" +
                " (0 int32 dw0000)" +
                " (4 real32 r0004)" +
                " (8 int32 dw0008)) t123130))";
            AssertGlobalVariables(expected);
        }
    }
}

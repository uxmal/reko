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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Decompiler.Typing
{
    [TestFixture]
    public class TypedExpressionRewriterTests : TypingTestBase
    {
        private TypedExpressionRewriter ter;
        private ExpressionNormalizer aen;
        private EquivalenceClassBuilder eqb;
        private TraitCollector coll;
        private DataTypeBuilder dtb;
        private TypeVariableReplacer tvr;
        private TypeTransformer trans;
        private ComplexTypeNamer ctn;
        private List<StructureField> userDefinedGlobals;
        private Dictionary<Address, ImageSegment> imageSegments;

        [SetUp]
        public void Setup()
        {
            userDefinedGlobals = new List<StructureField>();
            imageSegments = new Dictionary<Address, ImageSegment>();
        }

        protected override void RunTest(Program program, string outputFile)
        {
            var eventListener = new FakeDecompilerEventListener();
            using (FileUnitTester fut = new FileUnitTester(outputFile))
            {
                fut.TextWriter.WriteLine("// Before ///////");
                DumpProgram(program, fut.TextWriter);

                SetupPreStages(program);
                aen.Transform(program);
                eqb.Build(program);
                var coll = new TypeCollector(program.TypeFactory, program.TypeStore, program, eventListener);
                coll.CollectTypes();
                program.TypeStore.BuildEquivalenceClassDataTypes(program.TypeFactory, eventListener);
                tvr.ReplaceTypeVariables();
                trans.Transform();
                ctn.RenameAllTypes(program.TypeStore);
                ter = new TypedExpressionRewriter(program, program.TypeStore, eventListener);
                try
                {
                    ter.RewriteProgram(program);
                }
                catch (Exception ex)
                {
                    fut.TextWriter.WriteLine("** Exception **");
                    fut.TextWriter.WriteLine(ex);
                }
                finally
                {
                    fut.TextWriter.WriteLine("// After ///////");
                    DumpProgAndStore(program, fut);
                }
            }
        }

        protected void RunStringTest(Action<ProcedureBuilder> pb, string expectedOutput)
        {
            var pm = CreateProgramBuilder(0x1000, 0x1000);
            pm.Add("test", pb);
            pm.BuildProgram();
            RunStringTest(pm.Program, expectedOutput);
        }

        protected void RunStringTest(Program program, string expectedOutput)
        {
            var sw = new StringWriter();
            sw.WriteLine("// Before ///////");
            DumpProgram(program, sw);

            var eventListener = new FakeDecompilerEventListener();
            SetupPreStages(program);
            aen.Transform(program);
            eqb.Build(program);
            var coll = new TypeCollector(program.TypeFactory, program.TypeStore, program, eventListener);
            coll.CollectTypes();
            program.TypeStore.Dump();

            program.TypeStore.BuildEquivalenceClassDataTypes(program.TypeFactory, eventListener);
                program.TypeStore.Dump();
            tvr.ReplaceTypeVariables();
            trans.Transform();
            ctn.RenameAllTypes(program.TypeStore);

            var ter = new TypedExpressionRewriter(program, program.TypeStore, eventListener);
            try
            {
                ter.RewriteProgram(program);
            }
            catch (Exception ex)
            {
                sw.WriteLine("** Exception **");
                sw.WriteLine(ex);
                throw;
            }
            finally
            {
                sw.WriteLine("// After ///////");
                DumpProgram(program, sw);
            }
            var sActual = sw.ToString();
            if (expectedOutput != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(expectedOutput, sActual);
            }
        }

        private ProgramBuilder CreateProgramBuilder(uint linearAddress, int size)
        {
            return new ProgramBuilder(
                new ByteMemoryArea(Address.Ptr32(linearAddress), new byte[size]));
        }

        private void DumpProgram(Program program, TextWriter tw)
        {
            foreach (Procedure proc in program.Procedures.Values)
            {
                proc.Write(false, tw);
                tw.WriteLine();
            }
        }

        private void DumpProgAndStore(Program program, FileUnitTester fut)
        {
            foreach (Procedure proc in program.Procedures.Values)
            {
                proc.Write(false, fut.TextWriter);
                fut.TextWriter.WriteLine();
            }

            program.TypeStore.Write(false, fut.TextWriter);
            fut.AssertFilesEqual();
        }

        public void SetupPreStages(Program program)
        {
            var listener = new FakeDecompilerEventListener();
            foreach (var f in userDefinedGlobals)
            {
                program.GlobalFields.Fields.Add(f);
            }
            foreach (var s in imageSegments.Values)
            {
                program.SegmentMap.Segments.Add(s.Address, s);
            }
            aen = new ExpressionNormalizer(program.Platform.PointerType);
            eqb = new EquivalenceClassBuilder(program.TypeFactory, program.TypeStore, listener);
            dtb = new DataTypeBuilder(program.TypeFactory, program.TypeStore, program.Platform, listener);
            tvr = new TypeVariableReplacer(program.TypeStore);
            trans = new TypeTransformer(program.TypeFactory, program.TypeStore, program);
            ctn = new ComplexTypeNamer();
        }

        private void SetType(Expression e, DataType t)
        {
            e.DataType = t;
        }

        private void Given_GlobalVariable(uint linAddress, string name, DataType dt)
        {
            userDefinedGlobals.Add(new StructureField((int)linAddress, dt, name));
        }

        private void Given_ReadonlyMemory(uint uAddr, uint length)
        {

        }

        private ExternalProcedure Given_Procedure(string name, params DataType [] argTypes)
        {
            var sig = FunctionType.Action(
                argTypes.Select((argType, i) => new Identifier(
                        "arg" + i, 
                        argType,
                        new StackStorage(i * 4, argType)))
                    .ToArray());
            return new ExternalProcedure(name, sig);
        }

        [Test]
        public void TerComplex()
        {
            Program program = new Program();
            program.SegmentMap = new SegmentMap(Address.Ptr32(0x0010000));
            program.Architecture = new FakeArchitecture(new ServiceContainer());
            program.Platform = new DefaultPlatform(null, program.Architecture);
            SetupPreStages(program);
            Identifier id = new Identifier("v0", PrimitiveType.Word32, null);
            Expression cmp = MemLoad(id, 4, PrimitiveType.Word32);

            program.Globals.Accept(eqb);
            cmp.Accept(aen);
            cmp.Accept(eqb);
            coll = new TraitCollector(program.TypeFactory, program.TypeStore, dtb, program);
            cmp.Accept(coll);
            dtb.BuildEquivalenceClassDataTypes();

            tvr.ReplaceTypeVariables();
            trans.Transform();
            ctn.RenameAllTypes(program.TypeStore);

            ter = new TypedExpressionRewriter(program, program.TypeStore, null);
            cmp = cmp.Accept(ter);
            Assert.AreEqual("v0->dw0004", cmp.ToString());
        }

        [Test]
        public void TerPtrPtrInt()
        {
            var mock = CreateProgramBuilder(0x0010000, 0x1000);
            mock.Add(new PtrPtrIntMock());
            RunTest(mock.BuildProgram(), "Typing/TerPtrPtrInt.txt");
        }

        [Test]
        public void TerUnionIntReal()
        {
            var pb = CreateProgramBuilder(0x10000, 0x1000);
            pb.Add(new UnionIntRealMock());
            RunTest(pb.BuildProgram(), "Typing/TerUnionIntReal.txt");
        }

        [Test]
        public void TerConstantUnion()
        {
            var pb = CreateProgramBuilder(0x10000, 0x1000);
            pb.Add(new ConstantUnionMock());
            RunTest(pb.BuildProgram(), "Typing/TerConstantUnion.txt");
        }

        [Test]
        public void TerConstants()
        {
            var arch = new FakeArchitecture(new ServiceContainer());
            var segmentMap = new SegmentMap(Address.Ptr32(0x10000));
            Program program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                new DefaultPlatform(null, arch));
            SetupPreStages(program);
            Constant r = Constant.Real32(3.0F);
            Constant i = Constant.Int32(1);
            Identifier x = new Identifier("x", PrimitiveType.Word32, null);
            Assignment ass = new Assignment(x, r);
            TypeVariable tvR = program.TypeFactory.CreateTypeVariable();
            TypeVariable tvI = program.TypeFactory.CreateTypeVariable();
            TypeVariable tvX = program.TypeFactory.CreateTypeVariable();

            program.TypeStore.SetTypeVariable(r, tvR);
            program.TypeStore.SetTypeVariable(i, tvI);
            program.TypeStore.SetTypeVariable(x, tvX);

            program.TypeStore.TypeVariables.AddRange(new TypeVariable[] { tvR, tvI, tvX });
            UnionType u = program.TypeFactory.CreateUnionType(null, null, new DataType[] { r.DataType, i.DataType });
            tvR.OriginalDataType = r.DataType;
            tvI.OriginalDataType = i.DataType;
            tvX.OriginalDataType = x.DataType;
            tvR.DataType = u;
            tvI.DataType = u;
            tvX.DataType = u;
            ctn.RenameAllTypes(program.TypeStore);
            var ter = new TypedExpressionRewriter(program, program.TypeStore, null);
            Instruction instr = ter.TransformAssignment(ass);
            Assert.AreEqual("x.u0 = 3.0F", instr.ToString());
        }

        [Test]
        public void TerVector()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(new VectorFragment());
            RunTest(pb.BuildProgram(), "Typing/TerVector.txt");
        }

        [Test]
        public void TerGlobalVariables()
        {
            ProgramBuilder pb = CreateProgramBuilder(0x10000000, 0x1000);
            pb.Add(new GlobalVariablesMock());
            RunTest(pb.BuildProgram(), "Typing/TerGlobalVariables.txt");
        }

        [Test]
        public void TerSegmentedMemoryPointer()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(new SegmentedMemoryPointerMock());
            RunTest(pb.BuildProgram(), "Typing/TerSegmentedMemoryPointer.txt");
        }

        [Test]
        public void TerSegMemPtr2()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(new SegmentedMemoryPointerMock2());
            RunTest(pb.BuildProgram(), "Typing/TerSegMemPtr2.txt");
        }

        [Test]
        public void TerSegMem3()
        {
            ProgramBuilder mock = new ProgramBuilder();
            mock.Add(new SegMem3Mock());
            RunTest(mock.BuildProgram(), "Typing/TerSegMem3.txt");
        }

        [Test]
        public void TerArrayConstantPointers()
        {
            var pb = CreateProgramBuilder(0x00123000,4000);
            pb.Add("Fn", m =>
            {
                Identifier a = m.Local32("a");
                Identifier i = m.Local32("i");
                m.Assign(a, 0x00123456);		// array pointer
                m.MStore(m.IAdd(a, m.IMul(i, 8)), m.Int32(42));
            });
            RunTest(pb.BuildProgram(), "Typing/TerArrayConstantPointers.txt");
        }

        [Test]
        public void TerReg00008()
        {
            RunTest16("Fragments/regressions/r00008.asm", "Typing/TerReg00008.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void TerReg00011()
        {
            RunTest16("Fragments/regressions/r00011.asm", "Typing/TerReg00011.txt");
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        public void TerReg00012()
        {
            RunTest16("Fragments/regressions/r00012.asm", "Typing/TerReg00012.txt");
        }

        [Test]
        public void TerAddNonConstantToPointer()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add("proc1", m =>
            {
                Identifier i = m.Local16("i");
                Identifier p = m.Local16("p");

                m.MStore(p, m.Word16(4));
                m.MStore(m.IAdd(p, 4), m.Word16(4));
                m.Assign(p, m.IAdd(p, i));
            });
            RunTest(pb.BuildProgram(), "Typing/TerAddNonConstantToPointer.txt");
        }

        [Test]
        public void TerSignedCompare()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add("proc1", m =>
            {
                Identifier ds = m.Local16("ds");
                ds.DataType = PrimitiveType.SegmentSelector;
                Identifier ds2 = m.Local16("ds2");
                ds2.DataType = PrimitiveType.SegmentSelector;
                m.Assign(ds2, ds);
                m.Store(
                    m.SegMem(PrimitiveType.Bool, ds, m.Word16(0x5400)),
                    m.Lt(m.SegMem16(ds, m.Word16(0x5404)), m.Word16(20)));
                m.Store(m.SegMem16(ds2, m.Word16(0x5404)), m.Word16(0));
            });
            RunTest(pb.BuildProgram(), "Typing/TerSignedCompare.txt");
        }

        [Test]
        public void TerDereferenceSignedCompare()
        {
            ProgramBuilder pb = CreateProgramBuilder(0x5000, 0x1000);
            pb.Add("proc1", m =>
            {
                Identifier ds = m.Local16("ds");
                ds.DataType = PrimitiveType.SegmentSelector;
                Identifier ds2 = m.Local16("ds2");
                ds2.DataType = PrimitiveType.SegmentSelector;
                m.Assign(ds2, ds);
                m.Store(
                    m.SegMem(PrimitiveType.Bool, ds, m.Word16(0x5400)),
                    m.Lt(
                        m.SegMem16(ds, m.IAdd(m.SegMem16(ds, m.Word16(0x5404)), 4)),
                        m.Word16(20)));
                m.SStore(ds2, m.IAdd(m.SegMem16(ds2, m.Word16(0x5404)), 4), m.Word16(0));
                m.Return();
            });
            RunTest(pb.BuildProgram(), "Typing/TerDereferenceSignedCompare.txt");
        }

        [Test]
        public void TerFlatDereferenceSignedCompare()
        {
            ProgramBuilder pb = CreateProgramBuilder(0x5400, 0x1000);
            pb.Add("proc1", m =>
            {
                Identifier ds = m.Local32("ds");
                Identifier ds2 = m.Local32("ds2");
                m.Assign(ds2, ds);
                m.MStore(
                    m.IAdd(ds, m.Word32(0x5400)),
                    m.Lt(
                        m.Mem16(m.IAdd(m.Mem32(m.IAdd(ds, m.Word32(0x5404))), 4)),
                        m.Word16(20)));
                m.MStore(m.IAdd(m.Mem32(m.IAdd(ds2, m.Word32(0x5404))), 4), m.Word16(0));
            });
            RunTest(pb.BuildProgram(), "Typing/TerFlatDereferenceSignedCompare.txt");
        }

        [Test]
        public void TerComparison()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add("proc1", m =>
            {
                Identifier p = m.Local32("p");
                Expression fetch = m.Mem(new Pointer(new StructureType("foo", 8), 32), m.IAdd(p, 4));
                m.Assign(m.LocalBool("f"), m.Lt(fetch, m.Word32(0x00001028)));
            });
            RunTest(pb.BuildProgram(), "Typing/TerComparison.txt");
        }

        [Test]
        public void TerUnionConstants()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add("proc1", m =>
            {
                Identifier bx = m.Local16("bx");
                m.Assign(bx, m.Shr(bx, 2));     // makes bx unsigned uint16
                m.Assign(m.LocalBool("f"), m.Lt(bx, 4));    // makes bx also signed; assembler bug, but forces a union.
                m.Assign(bx, m.Word16(4));          // what type should 4 have?
            });
            RunTest(pb.BuildProgram(), "Typing/TerUnionConstants.txt");
        }

        [Test]
        public void TerOffsetInArrayLoop()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add("proc1", m =>
            {
                var ds = m.Local16("ds");
                var cx = m.Local16("cx");
                var di = m.Local16("di");
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(di, 0);
                m.Label("lupe");
                m.SStore(ds, m.IAdd(di, 0x5388), m.Word16(0));
                m.Assign(di, m.IAdd(di, 2));
                m.Assign(cx, m.ISub(cx, 1));
                m.BranchIf(m.Ne(cx, 0), "lupe");
                m.Return();
            });
            RunTest(pb, "Typing/TerOffsetInArrayLoop.txt");
        }

        [Test]
        public void TerSegmentedLoadLoad()
        {
            ProgramBuilder pm = new ProgramBuilder();
            pm.Add("proc1", m =>
            {
                var ds = m.Local(PrimitiveType.SegmentSelector, "ds");
                var bx = m.Local(PrimitiveType.Word16, "bx");
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.SStore(ds, m.Word16(0x300), m.SegMem16(ds, m.SegMem16(ds, bx)));
                m.Return();
            });
            RunTest(pm, "Typing/TerSegmentedLoadLoad.txt");
        }

        [Test]
        public void TerIntelIndexedAddressingMode()
        {
            ProgramBuilder m = new ProgramBuilder();
            m.Add(new IntelIndexedAddressingMode());
            RunTest(m.BuildProgram(), "Typing/TerIntelIndexedAddressingMode.txt");
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        public void TerReg00016()
        {
            RunHexTest("fragments/regressions/r00016.dchex", "Typing/TerReg00016.txt");
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        public void TerReg00017()
        {
            RunTest32("Fragments/regressions/r00017.asm", "Typing/TerReg00017.txt");
        }

        [Test]
        public void TerCallTable()
        {
            var pb = new ProgramBuilder();
            pb.Add(new IndirectCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TerCallTable.txt");
        }

        [Test]
        public void TerSegmentedCall()
        {
            var pb = new ProgramBuilder();
            pb.Add(new SegmentedCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TerSegmentedCall.txt");
        }

        [Test]
        public void TerPointerChain()
        {
            var pb = new ProgramBuilder();
            pb.Add(new PointerChainFragment());
            RunTest(pb.BuildProgram(), "Typing/TerPointerChain.txt");
        }

        [Test]
        public void TerStaggeredArrays()
        {
            ProgramBuilder pb = new ProgramBuilder();
            pb.Add(new StaggeredArraysFragment());
            RunTest(pb.BuildProgram(), "Typing/TerStaggeredArrays.txt");
        }

        [Test]
        public void TerArrayLoopMock()
        {
            var pb = CreateProgramBuilder(0x04000000, 0x9000);
            pb.Add(new ArrayLoopMock());
            RunTest(pb, "Typing/TerArrayLoopMock.txt");
        }

        [Test]
        public void TerArrayExpression()
        {
            var m = new ProgramBuilder();
            m.Add(new ArrayExpressionFragment());
            RunTest(m.BuildProgram(), "Typing/TerArrayExpression.txt");
        }

        [Test]
        public void TerDeclaration()
        {
            ProgramBuilder pm = new ProgramBuilder();
            pm.Add("proc1", m =>
            {
                var ax = m.Reg16("ax", 0);
                var rand = new ExternalProcedure(
                    "rand",
                    FunctionType.Create(
                        new Identifier("ax", PrimitiveType.Int16, ax.Storage),
                        Array.Empty<Identifier>()));
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(ax, m.Fn(rand));
                m.MStore(m.Word16(0x1300), ax);
                m.Return();
            });
            RunTest(pm, "Typing/TerDeclaration.txt");
        }

        [Test]
        public void TerShortArray()
        {
            var pm = CreateProgramBuilder(0x00001000, 0x1000);
            pm.Add("proc1", m =>
                {
                    var ebp = m.Reg32("ebp", 4);
                    var ecx = m.Reg32("ecx", 1);
                    var eax = m.Reg32("eax", 0);

                    m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                    m.Assign(ebp, m.ISub(m.Frame.FramePointer, 4));
                    m.Assign(eax, m.Mem32(m.IAdd(ebp, 0x0C)));
                    m.Assign(ecx, m.Mem32(m.IAdd(ebp, 0x08)));
                    m.Assign(eax, m.Convert(m.Mem(PrimitiveType.Int16,
                        m.IAdd(ecx, m.IMul(eax, 2))),
                        PrimitiveType.Int16,
                        PrimitiveType.Int32));
                    m.MStore(m.Word32(0x1234), eax);
                });
            RunTest(pm, "Typing/TerShortArray.txt");
        }

        [Test]
        public void TerArray()
        {
            var pm = CreateProgramBuilder(0x00F000, 0x2000);
            pm.Add("proc1", m =>
            {
                var eax = m.Reg32("eax", 0);
                var ecx = m.Reg32("ecx", 1);
                var eax_2 = m.Reg32("eax_2", 0);

                // eax_2 = (int32) ecx[eax];
                m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
                m.Assign(
                    eax_2,
                    m.Convert(
                    m.Mem(PrimitiveType.Int16,
                        m.IAdd(
                            ecx,
                            m.IAdd(
                                m.IMul(eax, 2),
                                100))),
                    PrimitiveType.Int16,
                    PrimitiveType.Int32));
                m.MStore(m.Word32(0x010000), eax_2);
            });
            RunTest(pm, "Typing/TerArray.txt");
        }

        [Test]
        public void Ter2Integer()
        {
            var pm = CreateProgramBuilder(0x1000, 0x1000);
            pm.Add("proc1", m =>
            {
                var eax = m.Reg32("eax", 0);
                m.MStore(m.Ptr32(0x01000), eax);
            });
            var sExp =
            #region Expected String
@"// Before ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	Mem0[0x00001000<p32>:word32] = eax
proc1_exit:

// After ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	g_dw1000 = eax
proc1_exit:

";
            #endregion
            RunStringTest(pm.BuildProgram(), sExp);
        }

        [Test]
        public void Ter2PtrToInt16()
        {
            var pm = CreateProgramBuilder(0x1000, 0x1000);
            pm.Add("proc1", m =>
            {
                var eax = m.Reg32("eax", 0);
                m.MStore(m.Word32(0x01000), m.Mem16(eax));
            });
            var sExp =
            #region Expected String
@"// Before ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	Mem0[0x1000<32>:word16] = Mem0[eax:word16]
proc1_exit:

// After ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	g_w1000 = *eax
proc1_exit:

";
            #endregion
            RunStringTest(pm.BuildProgram(), sExp);
        }

        [Test]
        public void Ter2PtrToStruct()
        {
            var pm = CreateProgramBuilder(0x1000, 0x1000);
            pm.Add("proc1", m =>
            {
                var eax = m.Reg32("eax", 0);
                m.MStore(m.Ptr32(0x01000), m.Mem16(eax));
                m.MStore(m.Ptr32(0x01002), m.Mem16(m.IAdd(eax, 2)));
            });
            var sExp =
            #region Expected String
@"// Before ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	Mem0[0x00001000<p32>:word16] = Mem0[eax:word16]
	Mem0[0x00001002<p32>:word16] = Mem0[eax + 2<32>:word16]
proc1_exit:

// After ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	g_w1000 = eax->w0000
	g_w1002 = eax->w0002
proc1_exit:

";
            #endregion
            RunStringTest(pm.BuildProgram(), sExp);
        }

        [Test]
        public void Ter2ThreeStarProgrammer()
        {
            var pm = CreateProgramBuilder(0x1000, 0x1000);
            pm.Add("proc1", m =>
            {
                var eax1 = m.Reg32("eax1", 0);
                var eax2 = m.Reg32("eax2", 0);
                var eax3 = m.Reg32("eax3", 0);
                m.Assign(eax2, m.Mem32(eax1));
                m.Assign(eax3, m.Mem32(eax2));
                m.MStore(m.Ptr32(0x01004), m.Mem(PrimitiveType.Real32, eax3));
            });
            var sExp =
            #region Expected String
@"// Before ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	eax2 = Mem0[eax1:word32]
	eax3 = Mem0[eax2:word32]
	Mem0[0x00001004<p32>:real32] = Mem0[eax3:real32]
proc1_exit:

// After ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	eax2 = *eax1
	eax3 = *eax2
	g_r1004 = *eax3
proc1_exit:

";
            #endregion
            RunStringTest(pm.BuildProgram(), sExp);
        }

        [Test]
        public void Ter2LinkedList()
        {
            var pm = CreateProgramBuilder(0x1000, 0x1000);
            pm.Add("proc1", m =>
            {
                var r1 = m.Reg32("r1", 1);
                m.Assign(r1, m.Mem32(r1));
                m.MStore(m.Ptr32(0x01004), m.Mem(
                    PrimitiveType.Char,
                    m.IAdd(
                        m.Mem32(
                            m.Mem32(r1)),
                        4)));
            });
            var sExp =
            #region Expected String
@"// Before ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	r1 = Mem0[r1:word32]
	Mem0[0x00001004<p32>:char] = Mem0[Mem0[Mem0[r1:word32]:word32] + 4<32>:char]
proc1_exit:

// After ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	r1 = r1->ptr0000
	g_b1004 = r1->ptr0000->ptr0000->b0004
proc1_exit:

";
            #endregion
            RunStringTest(pm.BuildProgram(), sExp);
        }

        [Test]
        public void Ter2AddrOfLinkedList()
        {
            var pm = CreateProgramBuilder(0x1000, 0x1000);
            pm.Add("proc1", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(r1, m.Mem32(r1));
                m.MStore(m.Word32(0x01004), m.Mem(
                    PrimitiveType.Char,
                    m.IAdd(
                        m.Mem32(
                            m.Mem32(r1)),
                        4)));
                m.Assign(r2, m.IAdd(r1, 4));
            });
            var sExp =
            #region Expected String
@"// Before ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	r1 = Mem0[r1:word32]
	Mem0[0x1004<32>:char] = Mem0[Mem0[Mem0[r1:word32]:word32] + 4<32>:char]
	r2 = r1 + 4<32>
proc1_exit:

// After ///////
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	r1 = r1->ptr0000
	g_b1004 = r1->ptr0000->ptr0000->b0004
	r2 = &r1->b0004
proc1_exit:

";
            #endregion
            RunStringTest(pm.BuildProgram(), sExp);
        }

        [Test]
        public void TerStruct()
        {
            var sExp =
            #region Expected
@"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	eax = Mem0[0x1200<32>:word32]
	Mem0[eax:word32] = eax
	Mem0[eax + 4<32>:word32] = eax
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	eax = g_ptr1200
	eax->ptr0000 = eax
	eax->ptr0004 = eax
test_exit:

";
            #endregion

            RunStringTest(m =>
            {
                var eax = m.Reg32("eax", 0);
                m.Assign(eax, m.Mem32(m.Word32(0x1200)));
                m.MStore(eax, eax);
                m.MStore(m.IAdd(eax, 4), eax);
            },sExp);
        }

        /*
        [Test]
        public void TerDeclaration2()
        {
            var sExp = @"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	word32 foo = 1<32>
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	word32 foo = 1<32>
test_exit:

";
            RunStringTest(m =>
            {
                m.Declare(PrimitiveType.Word32, "foo", m.Word32(1));
            }, sExp);
        }
        */

        [Test(Description = "Tests that user-provided global names are used in the output")]
        public void TerNamedGlobal()
        {
            var sExp =
            #region Expected
@"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	func(0x1000<32>)
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	func(arrayBlobs)
test_exit:

";
            #endregion

            var sBlob = new StructureType("blob_t", 16);
            var func = Given_Procedure("func", new Pointer(sBlob, 32));
            Given_GlobalVariable(
                0x0001000,
                "arrayBlobs",
                new ArrayType(
                    new TypeReference(sBlob), 5));
            RunStringTest(m =>
            {
                m.SideEffect(m.Fn(func, m.Word32(0x1000)));
            }, sExp);
        }

        [Test(Description = "Rewrite constants with segment selector type ")]
        public void TerSelector()
        {
            var sExp =
            #region Expected

@"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	ds = 0x1234<16>
	Mem0[ds:0x10<16>:word32] = 0x10004<32>
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	ds = seg1234
	ds->dw0010 = 0x10004<32>
test_exit:

"
;
            #endregion

            var seg = new ImageSegment(
                "1234",
                new ByteMemoryArea(Address.SegPtr(0x1234, 0), new byte[0x100]),
                AccessMode.ReadWriteExecute);
            seg.Identifier = Identifier.CreateTemporary("seg1234", PrimitiveType.SegmentSelector);
            imageSegments.Add(seg.Address, seg);
            RunStringTest(m =>
            {
                var ds = m.Frame.CreateTemporary("ds", PrimitiveType.SegmentSelector);
                m.Assign(ds, Constant.Create(ds.DataType, 0x1234));
                m.SStore(ds, m.Word16(0x10), m.Word32(0x010004));
            }, sExp);
        }

        [Test]
        public void TerNestedStructsPtr()
        {
            var pm = CreateProgramBuilder(0x1000, 0x1000);
            pm.Add("proc1", m =>
            {
                var eax = m.Reg32("eax", 0);
                var ecx = m.Reg32("ecx", 1);
                var strInner = new StructureType("strInner", 8, true)
                {
                    Fields = {
                        { 0, PrimitiveType.Real32, "innerAttr00" },
                        { 4, PrimitiveType.Int32, "innerAttr04" },
                    }
                };
                var str = new StructureType("str", 8, true)
                {
                    Fields = {
                        { 0, new Pointer(strInner, 32), "strAttr00" },
                        { 4, PrimitiveType.Int32, "strAttr04" },
                    }
                };
                var v = m.Frame.EnsureStackArgument(4, new Pointer(str, 32));
                m.Assign(eax, m.Mem(PrimitiveType.Word32, v));
                m.Assign(ecx, m.Mem(PrimitiveType.Word32, eax));
            });
            RunTest(pm.BuildProgram(), "Typing/TerNestedStructsPtr.txt");
        }

        [Test]
        public void TerAddressOf()
        {
            var pb = new ProgramBuilder();
            pb.Add("AddressOf", m =>
            {
                var foo = Identifier.Global("foo", new UnknownType());
                var r1 = m.Reg32("r1", 1);
                m.Assign(r1, m.AddrOf(PrimitiveType.Ptr32, foo));
                m.MStore(r1, m.Word16(0x1234));
                m.MStore(m.IAdd(r1, 4), m.Byte(0x0A));
                m.Return();
            });
            RunTest(pb.BuildProgram(), "Typing/TerAddressOf.txt");
        }

        [Test]
        public void TerTypedAddressOf()
        {
            var pb = new ProgramBuilder();
            pb.Add("TypedAddressOf", m =>
            {
                var str = new TypeReference("foo", new StructureType("foo", 0)
                {
                    Fields = {
                        { 0, PrimitiveType.Int16, "word00" },
                        { 4, PrimitiveType.Byte, "byte004"}
                    }
                });
                var foo = Identifier.Global("foo", str);
                var r1 = m.Reg32("r1", 1);
                m.Assign(r1, m.AddrOf(PrimitiveType.Ptr32, foo));
                m.MStore(r1, m.Word16(0x1234));
                m.MStore(m.IAdd(r1, 4), m.Byte(0x0A));
                m.Return();
            });
            RunTest(pb.BuildProgram(), "Typing/TerTypedAddressOf.txt");
        }

        [Test(Description = "@smx-smx discovered that 64-bit ELF binaries always have an issue with a variable" + 
            " being used both as a signed and an unsigned integer.")]
        public void TerUnsignedSigned()
        {
            var pb = new ProgramBuilder();
            pb.Add("register_tm_clones", m =>
            {
                var rax = Identifier.Create(RegisterStorage.Reg64("rax", 0));
                var rsp = Identifier.Create(RegisterStorage.Reg64("rsp", 4));
                var rbp = Identifier.Create(RegisterStorage.Reg64("rbp", 5));
                var rsi = Identifier.Create(RegisterStorage.Reg64("rsi", 6));
                var rdi = Identifier.Create(RegisterStorage.Reg64("rdi", 7));
                var rflags = RegisterStorage.Reg64("rflags", 42);
                var SCZO =Identifier.Create(new FlagGroupStorage(rflags, 0xF, "SZCO"));
                var Z = Identifier.Create(new FlagGroupStorage(rflags, 0x2, "Z"));

                //m.Assign(rsp, m.Frame.FramePointer);
                m.Assign(rdi, 0x0000000000201028);
                m.Assign(rsi, 0x0000000000201028);
                //m.Assign(rsp, m.ISub(rsp, 0x0000000000000008<64>));

    
                m.Assign(rsi, m.ISub(rsi, rdi));
                m.BranchIf(m.Eq0(m.IAdd(m.Sar(rsi, 0x0000000000000003),
                    m.Sar(m.Shr(m.Sar(rsi, 0x0000000000000003), 0x3F), 1))), "mHyperSpace");

                m.Label("mHello");
                m.MStore(m.Word64(42), rsi);
                m.Label("mHyperspace");

                m.Return();
            });
            RunTest(pb.BuildProgram(), "Typing/TerUnsignedSigned.txt");
        }

        [Test]
        public void TerAddress()
        {
            var pb = new ProgramBuilder();
            pb.Add("fn", m =>
            {
                m.MStore(Address.Ptr32(0x001028),
                    m.Convert(
                        m.Mem(PrimitiveType.Real64, Address.Ptr32(0x001020)),
                        PrimitiveType.Real64,
                        PrimitiveType.Real32));
                m.Return();
            });
            var program = pb.BuildProgram();
            RunTest(program, "Typing/" + nameof(TerAddress) + ".txt");
        }

        [Test]
        public void TerSliceToCast()
        {
            var sExp =
            #region Expected
@"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	eax = Mem0[0x1200<32>:word32]
	ax_1 = SLICE(eax, word16, 0)
	return
	// succ:  test_exit
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	eax = g_dw1200
	ax_1 = (word16) eax
	return
	// succ:  test_exit
test_exit:

";
            #endregion

            RunStringTest(m =>
            {
                var eax = m.Reg32("eax", 0);
                var ax_1 = m.Reg16("ax_1", 0);
                m.Assign(eax, m.Mem32(m.Word32(0x1200)));
                m.Assign(ax_1, m.Slice(eax, PrimitiveType.Word16));
                m.Return();
            }, sExp);
        }

        [Test]
        public void TerArrayAssignment()
        {
            var sExp =
            #region Expected
@"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l000000000040EC30
l000000000040EC30:
	rbx_18 = rdx - 1<64>
	branch rdx == 0<64> l000000000040EC69
	// succ:  l000000000040EC40 l000000000040EC69
l000000000040EC40:
	rax_22 = 0x10000040<64>
	// succ:  l000000000040EC50
l000000000040EC50:
	Mem0[rdi + rbx_18:byte] = CONVERT(Mem0[Mem0[rax_22:word64] + CONVERT(CONVERT(Mem0[rsi + rbx_18:byte], byte, word32), word32, uint64) * 4<64>:word32], word32, byte)
	rbx_18 = rbx_18 - 1<64>
	branch rbx_18 != 0xFFFFFFFFFFFFFFFF<64> l000000000040EC50
	// succ:  l000000000040EC69 l000000000040EC50
l000000000040EC69:
	return
	// succ:  test_exit
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l000000000040EC30
l000000000040EC30:
	rbx_18 = rdx - 1<64>
	branch rdx == 0<64> l000000000040EC69
	// succ:  l000000000040EC40 l000000000040EC69
l000000000040EC40:
	rax_22 = (word32 (**)[]) 0x10000040<64>
	// succ:  l000000000040EC50
l000000000040EC50:
	rdi[rbx_18] = (byte) *((char *) *rax_22 + (uint64) ((word32) (*((word64) rsi + rbx_18))) * 4<64>)
	rbx_18 = rbx_18 - 1<64>
	branch rbx_18 != 0xFFFFFFFFFFFFFFFF<64> l000000000040EC50
	// succ:  l000000000040EC69 l000000000040EC50
l000000000040EC69:
	return
	// succ:  test_exit
test_exit:

";
            #endregion

            // fn000000000040EC30 //////////
            RunStringTest(m =>
            {
                Identifier rbx_18 = m.Local(PrimitiveType.Word64, "rbx_18");
                Identifier rdx = m.Local(PrimitiveType.Word64, "rdx");
                Identifier rax_22 = m.Local(PrimitiveType.Word64, "rax_22");
                Identifier rsi = m.Local(PrimitiveType.Word64, "rsi");
                Identifier rdi = m.Local(PrimitiveType.Word64, "rdi");

                m.Label("l000000000040EC30");
                m.Assign(rbx_18, m.ISub(rdx, Constant.Create(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), 0x1)));
                m.BranchIf(m.Eq(rdx, Constant.Create(PrimitiveType.Create(Domain.Integer | Domain.Real | Domain.Pointer, 64), 0x0)), "l000000000040EC69");

                m.Label("l000000000040EC40");
                m.Assign(rax_22, m.Word64(0x10000040));

                m.Label("l000000000040EC50");
                m.MStore(m.IAdd(rdi, rbx_18), m.Convert(
                    m.Mem32(m.IAdd(m.Mem64(rax_22), m.IMul(m.Convert(
                        m.Convert(
                            m.Mem8(m.IAdd(rsi, rbx_18)),
                            PrimitiveType.Byte,
                            PrimitiveType.Word32),
                        PrimitiveType.Word32,
                        PrimitiveType.UInt64),
                    m.Const(PrimitiveType.Word64, 0x4)))),
                    PrimitiveType.Word32,
                    PrimitiveType.Byte));
                m.Assign(rbx_18, m.ISub(rbx_18, m.Const(PrimitiveType.Word64, 0x1)));
                m.BranchIf(m.Ne(rbx_18, m.Const(PrimitiveType.Word64, 0xFFFFFFFFFFFFFFFF)), "l000000000040EC50");

                m.Label("l000000000040EC69");
                m.Return();
            }, sExp);
        }

        [Test]
        public void TerGithubIssue879()
        {
            var sExp =
            #region Expected
                @"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	Mem0[ds:0x118<16>:byte] = Mem0[ds:CONVERT(CONVERT(Mem0[ds:0x94<16>:byte], byte, uint8), uint8, word16) + 0x95<16>:byte]
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	ds->b0118 = ds->a0095[(word16) (uint8) ds->b0094]
test_exit:

";
            #endregion

            RunStringTest(m =>
            {
                Identifier ds = m.Reg16("ds", 42);

                m.SStore(ds, m.Word16(0x118),
                    m.SegMem8(
                        ds,
                        m.IAdd(
                            m.Convert(m.Convert(
                                m.SegMem8(ds, m.Word16(0x94)),
                                PrimitiveType.Byte,
                                PrimitiveType.UInt8),
                            PrimitiveType.UInt8,
                            PrimitiveType.Word16),
                            m.Word16(0x95))));
            }, sExp);

        }

        [Test]
        public void TerConvertToCast()
        {
            var sExp =
            #region Expected
@"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	Mem0[ptr:real32] = CONVERT(n, int16, real32)
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	*ptr = (real32) n
test_exit:

";
            #endregion

            RunStringTest(m =>
            {
                Identifier n = m.Reg16("n", 42);
                Identifier ptr = m.Reg32("ptr", 43);
                m.MStore(ptr, m.Convert(n, PrimitiveType.Int16, PrimitiveType.Real32));
            }, sExp);
        }

        [Test]
        public void TerDerivedArray()
        {
            var sExp =
            #region Expected
@"// Before ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	Mem0[a5 + -2696<i32> + d3 * 4<i32>:word32] = 0<32>
	d0_1 = Mem0[a5 + -2696<i32> + d3 * 4<i32>:word32]
	Mem0[a2 + 8<i32>:word32] = d0_1
	return
	// succ:  test_exit
test_exit:

// After ///////
// test
// Return size: 0
define test
test_entry:
	// succ:  l1
l1:
	a5->aFFFFF578[d3] = 0<32>
	d0_1 = a5->aFFFFF578[d3]
	a2->dw0008 = d0_1
	return
	// succ:  test_exit
test_exit:

";
            #endregion

            RunStringTest(m =>
            {
                Identifier a2 = m.Local(PrimitiveType.Word32, "a2");
                Identifier a5 = m.Local(PrimitiveType.Word32, "a5");
                Identifier d0_1 = m.Local(PrimitiveType.Word32, "d0_1");
                Identifier d3 = m.Local(PrimitiveType.Word32, "d3");

                m.MStore(m.IAdd(m.IAdd(a5, m.Int32(-2696)), m.IMul(d3, m.Int32(4))), m.Word32(0x0));
                m.Assign(d0_1, m.Mem32(m.IAdd(m.IAdd(a5, m.Int32(-2696)), m.IMul(d3, m.Int32(4)))));
                m.MStore(m.IAdd(a2, m.Int32(8)), d0_1);
                m.Return();
            }, sExp);
        }

        [Test]
        public void TerPassARefToFunction()
        {
            var sExp =
@"// Before ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r1 = Mem0[r0 + 0<32>:word32]
	r1 = Mem0[r0 + r1 * 4<32>:word32]
main_exit:

// After ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r1 = r0[0<i32>]
	r1 = r0[r1]
main_exit:

";
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r0 = m.Register("r0");
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var r3 = m.Register("r3");
                m.Assign(r1, m.Mem32(m.IAdd(r0, 0)));
                m.Assign(r1, m.Mem32(m.IAdd(r0, m.IMul(r1, 4))));
            });
            RunStringTest(pb.BuildProgram(), sExp);
        }

        [Test]
        public void TerCallSegmentedPtr()
        {
            var sExp =
            #region Expected
@"// Before ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	call 0xC00<16>:bx + 4<16> (retsize: 4;)
main_exit:

// After ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	(0xC00<16>->*((char *) bx + 4<i32>))()
main_exit:

";
            #endregion
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var bx = m.Register(RegisterStorage.Reg16("bx", 3));
                m.Call(m.SegPtr(m.Word16(0xC00), m.IAdd(bx, 4)), 4);
            });
            RunStringTest(pb.BuildProgram(), sExp);
        }

        [Test]
        public void TerFetchReadonlyCharPtr()
        {
            var sExp =
@"// Before ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	return Mem0[0x2000<32>:(ptr32 char)]
	// succ:  main_exit
main_exit:

// After ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	return ""Hello""
	// succ:  main_exit
main_exit:

";
            var pb = new ProgramBuilder();
            var mem = new ByteMemoryArea(Address.Ptr32(0x2000), new byte[0x100]);
            mem.WriteLeUInt32(0, 0x2008);
            mem.WriteBytes(Encoding.UTF8.GetBytes("Hello"), 8, 5);
            imageSegments.Add(mem.BaseAddress, new ImageSegment(".rodata", mem, AccessMode.Read));
            pb.Add("main", m =>
            {
                var ptr = new Pointer(PrimitiveType.Char, 32);
                m.Return(m.Mem(ptr, m.Word32(0x2000)));
            });
            RunStringTest(pb.BuildProgram(), sExp);
        }

        [Test]
        public void TerMultiDimensionalArray()
        {
            var sExp =
            #region Expected
@"// Before ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r0 = Mem0[a + r1 * 0x20<32> + r2 * 4<32>:word32]
main_exit:

// After ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r0 = (a + r1)->a0000[r2]
main_exit:

";
            #endregion
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r0 = m.Register("r0");
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var a = m.Temp(PrimitiveType.Word32, "a");
                m.Assign(
                    r0,
                    m.Mem32(m.IAdd(m.IAdd(a, m.IMul(r1, 32)), m.IMul(r2, 4)))
                );
            });
            RunStringTest(pb.BuildProgram(), sExp);
        }

        [Test]
        public void TerMultiDimensionalArray_UserDefined()
        {
            var sExp =
            #region Expected
@"// Before ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r0 = Mem0[a + r1 * 0x20<32> + r2 * 4<32>:word32]
main_exit:

// After ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r0 = (a + r1)->array[r2]
main_exit:

";
            #endregion
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r0 = m.Register("r0");
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var str = new StructureType("str", 32, true)
                {
                    Fields = {
                        {0, new ArrayType(PrimitiveType.Int32, 8), "array"}
                    }
                };
                var a = m.Temp(new Pointer(str, 32), "a");
                m.Assign(
                    r0,
                    m.Mem32(m.IAdd(m.IAdd(a, m.IMul(r1, 32)), m.IMul(r2, 4)))
                );
            });
            RunStringTest(pb.BuildProgram(), sExp);
        }

        [Test]
        public void TerMultiDimensionalArray_TwoStatements()
        {
            var sExp =
            #region Expected
@"// Before ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r0 = a + r1 * 0x20<32>
	Mem0[r0 + r2 * 8<32>:word32] = 1<32>
	Mem0[r0 + (r2 * 8<32> + 4<32>):word32] = 2<32>
main_exit:

// After ///////
// main
// Return size: 0
define main
main_entry:
	// succ:  l1
l1:
	r0 = a + r1 * 0x20<32>
	r0[r2].dw0000 = 1<32>
	r0[r2].dw0004 = 2<32>
main_exit:

";
            #endregion
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r0 = m.Register("r0");
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var a = m.Temp(PrimitiveType.Word32, "a");
                m.Assign(
                    r0,
                    m.IAdd(a, m.IMul(r1, 32))
                );
                m.MStore(m.IAdd(r0, m.IMul(r2, 8)), m.Word32(1));
                m.MStore(m.IAdd(r0, m.IAdd(m.IMul(r2, 8), 4)), m.Word32(2));
            });
            RunStringTest(pb.BuildProgram(), sExp);
        }
    }
}

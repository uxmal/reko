#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Arch.X86.Analysis;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.ImageLoaders.OdbgScript;
using Reko.Services;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
	public class ConditionCodeEliminatorTests : AnalysisTestBase
	{
		private SsaIdentifierCollection ssaIds;
        private RegisterStorage freg;
        private SsaState ssaState;
        private ProcedureBuilder m;
        private ConditionCodeEliminator cce;
        private SegmentMap segmentMap;

        [SetUp]
		public void Setup()
		{
            m = new ProcedureBuilder();
            ssaState = new SsaState(m.Procedure);
            ssaIds = ssaState.Identifiers;
            freg = RegisterStorage.Reg32("flags", 32);
            segmentMap = new SegmentMap(Address.Ptr32(0));
		}

        private void Given_ConditionCodeEliminator()
        {
            var program = new Program
            {
                Platform = new DefaultPlatform(null, new FakeArchitecture(new ServiceContainer()))
            };
            var context = CreateAnalysisContext(program, ssaState.Procedure);
            cce = new ConditionCodeEliminator(context);
        }

        protected Program CompileTest(Action<ProcedureBuilder> m)
        {
            var mock = new ProcedureBuilder();
            m(mock);
            var pmock = new ProgramBuilder();
            pmock.Add(mock);
            return pmock.BuildProgram();
        }

        private void RunTest(ProgramBuilder p, string output)
        {
            SaveRunOutput(p.BuildProgram(), RunTest, output);
        }

        private Identifier Reg32(string name)
        {
            var mr = new RegisterStorage(name, ssaIds.Count, 0, PrimitiveType.Word32);
            var id = new Identifier(name, PrimitiveType.Word32, mr);
            return ssaIds.Add(id, null, false).Identifier;
        }

        private Identifier FlagGroup(string name)
        {
            Identifier id = new Identifier(
                name,
                PrimitiveType.Word32,
                new FlagGroupStorage(freg, 1U, "C"));
            return ssaIds.Add(id, null, false).Identifier;
        }

        private Expression RorC(Expression expr, Expression count, Expression carry)
        {
            return m.Fn(CommonOps.RorC.MakeInstance(expr.DataType, count.DataType), expr, count, carry);
        }

        private void RunSsaTest(string sExpected, Action<SsaProcedureBuilder> generateCode)
        {
            var ssapb = new SsaProcedureBuilder();
            generateCode(ssapb);
            var program = new Program
            {
                Platform = platform,
            };
            var context = CreateAnalysisContext(program, ssapb.Procedure);
            var cce = new ConditionCodeEliminator(context);
            cce.Transform(ssapb.Ssa);
            var writer = new StringWriter();
            ssapb.Ssa.Procedure.WriteBody(true, writer);
            var sActual = writer.ToString();
            if (sActual != sExpected)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
            ssapb.Ssa.Validate(s => { ssapb.Ssa.Dump(true); Assert.Fail(s); });
        }

        private static AnalysisContext CreateAnalysisContext(Program program, Procedure proc)
        {
            return new AnalysisContext(
                program,
                proc,
                null,
                null,
                new FakeDecompilerEventListener());
        }

        protected override void RunTest(Program program, TextWriter writer)
        {
            var dynamicLinker = new Mock<IDynamicLinker>().Object;
            var listener = new FakeDecompilerEventListener();
            var sc = new ServiceContainer();
            sc.AddService<IEventListener>(listener);
            sc.AddService<IDecompilerEventListener>(listener);
            var dfa = new DataFlowAnalysis(program, dynamicLinker, sc);
            foreach (var proc in program.Procedures.Values)
            {
                var sst = new SsaTransform(
                    program,
                    proc,
                    new HashSet<Procedure>(),
                    dynamicLinker, 
                    new ProgramDataFlow());
                var ssa = sst.Transform();

                var context = new AnalysisContext(
                    program,
                    proc,
                    dynamicLinker,
                    sc,
                    listener);

                var larw = new LongAddRewriter(context);
                larw.Transform(ssa);

                // This is x86-specific, but harmless when run on
                // non-x86 programs.
                var fstsw = new FstswAnalysis(program, listener);
                (ssa, _) = fstsw.Transform(ssa);

                var cce = new ConditionCodeEliminator(context);
                cce.Transform(ssa);
                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });

                var vp = new ValuePropagator(context);
                vp.Transform(ssa);
                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });

                sst.RenameFrameAccesses = true;
                sst.Transform();

                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });

                // We don't add uses to exit block on purpose. We
                // are not testing interprocedural effects here.
                DeadCode.Eliminate(ssa);

                ssa.Procedure.Write(false, writer);
                writer.WriteLine();

                ssa.Validate(s => { ssa.Write(Console.Out); ssa.Dump(true); Assert.Fail(s); });
            }
        }

		[Test]
        [Category(Categories.IntegrationTests)]
        public void CceAddSubCarries()
		{
			RunFileTest_x86_real("Fragments/addsubcarries.asm", "Analysis/CceAddSubCarries.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void CceAdcMock()
        {
			RunFileTest(new AdcMock(), "Analysis/CceAdcMock.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void CceCmpMock()
        {
			RunFileTest(new CmpMock(), "Analysis/CceCmpMock.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void CceFrame32()
        {
			RunFileTest_x86_32("Fragments/multiple/frame32.asm", "Analysis/CceFrame32.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void CceWhileLoop()
        {
			RunFileTest_x86_real("Fragments/while_loop.asm", "Analysis/CceWhileLoop.txt");
		}

        [Test]
        public void CceFstswTestAx()
        {
            Program program = RewriteCodeFragment(@"
                fcomp   dword ptr [bx]
                fstsw  ax
                test    ah,0x41
                jz      done
                xor     ecx,ecx
 done:
                ret
");
            SaveRunOutput(program, RunTest, "Analysis/CceFstswTestAx.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        [Ignore("Wait until we see this in real code? If we do, we have to move the logic for FPUF into an architecture specific branch.")]
        public void CceFstswTestAxWithConstantBl()
        {
            Program program = RewriteCodeFragment(@"
                mov     bx,1
                fcomp   dword ptr[si]
                fstsw   ax
                test    bl,ah
                jz      done
                mov     byte ptr [0x0300<16>],1
done:
                ret
");
            SaveRunOutput(program, RunTest, "Analysis/CceFstswTestAxWithConstantBl.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceFstswEq()
        {
            var program = RewriteCodeFragment(@"
    fld	QWORD PTR [si]
	fldz
	fcompp
	fstsw	ax
	test	ah, 68					; 00000044H
	jpe	done
	mov	word ptr[di], 1
    ret
done:
    mov word ptr[di], 0
    ret
");
            SaveRunOutput(program, RunTest, "Analysis/CceFstswEq.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceFstswNe()
        {
            var program = RewriteCodeFragment(@"
	fld	QWORD PTR [si]
	fldz
	fcompp
	fstsw	ax
	test	ah, 68					; 00000044H
	jnp	done
	mov	word ptr [di], 1
	ret	
done:
	mov	word ptr [di], 0
	ret	
");
            SaveRunOutput(program, RunTest,"Analysis/CceFstswNe.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceFstswGe()
        {
            var program = RewriteCodeFragment(@"

; 18   : 	return x >= 0;

	fldz
	fcomp	qword ptr [si]
	fstsw	ax
	test	ah, 65					; 00000041H
	jp	done
	mov	word ptr[di], 1
	ret	
done:
	mov	word ptr[di], 0
    ret
");
            SaveRunOutput(program, RunTest, "Analysis/CceFstswGe.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceFstswGt()
        {
            var program = RewriteCodeFragment(@"
; 13   : 	return x > 0;

	fldz
	fcomp	qword ptr [si]
	fstsw	ax
	test	ah, 5
	jp	done
	mov	word ptr[di], 1
	ret	
done:
	mov	word ptr[di], 0
    ret
");
            SaveRunOutput(program, RunTest,"Analysis/CceFstswGt.txt");

         }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceFstswLe()
        {
            var program = RewriteCodeFragment(@"

; 8    : 	return x <= 0;

	fldz
	fcomp	qword ptr [si]
	fstsw	ax
	test	ah, 1
	jne	done
	mov	word ptr[di], 1
	ret	
done:
	mov	word ptr[di], 0
    ret
");
            SaveRunOutput(program, RunTest, "Analysis/CceFstswLe.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceFstswLt()
        {
            var program = RewriteCodeFragment(@"
; 3    : 	return x < 0;

	fldz
	fcomp	qword ptr [si]
	fstsw	ax
	test	ah, 65					; 00000041H
	jne	done
	mov	word ptr[di], 1
	ret	
done:
	mov	word ptr[di], 0
    ret
");
            SaveRunOutput(program, RunTest, "Analysis/CceFstswLt.txt");
        }

		[Test]
        [Category(Categories.UnitTests)]
		public void CceEqId()
		{
			Identifier r = Reg32("r");
			Identifier z = FlagGroup("z");  // is a condition code.
            Identifier y = FlagGroup("y");  // is a condition code.

            m.Assign(z, new ConditionOf(r));
            ssaIds[z].DefStatement = m.Block.Statements[^1];
            m.Assign(y, z);
            ssaIds[y].DefStatement = m.Block.Statements[^1];
			ssaIds[z].Uses.Add(m.Block.Statements[^1]);
			var stmBr = m.BranchIf(m.Test(ConditionCode.EQ, y), "foo");
            ssaIds[y].Uses.Add(stmBr);

            Given_ConditionCodeEliminator();
			cce.Transform(ssaState);
			Assert.AreEqual("branch r == 0<32> foo", stmBr.Instruction.ToString());
		}

        [Test]
        [Category(Categories.UnitTests)]
        public void CceSetnz()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r
	// succ:  l1
l1:
	Mem4[0x123400<32>:word32] = r != 0<32>
ProcedureBuilder_exit:

";
            #endregion

            RunStringTest(sExp, m =>
            {
                Identifier r = Reg32("r");
                Identifier Z = FlagGroup("Z");
                Identifier f = Reg32("f");

                m.Assign(Z, m.Cond(m.ISub(r, 0)));
                m.Assign(f, m.Test(ConditionCode.NE, Z));
                m.MStore(m.Word32(0x123400), f);
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
		public void Cce_SignedIntComparisonFromConditionCode()
        {
            Given_ConditionCodeEliminator();
            var bin = m.ISub(new Identifier("a", PrimitiveType.Word16, null), new Identifier("b", PrimitiveType.Word16, null));
            var worker = cce.CreateWorker(ssaState);
            var b = (BinaryExpression) worker.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
            Assert.AreEqual("a < b", b.ToString());
            Assert.AreEqual("LtOperator", b.Operator.GetType().Name);
        }


        [Test]
        [Category(Categories.UnitTests)]
		public void Cce_RealComparisonFromConditionCode()
		{
            Given_ConditionCodeEliminator();
			var bin = m.FSub(new Identifier("a", PrimitiveType.Real64, null), new Identifier("b", PrimitiveType.Real64, null));
            var worker = cce.CreateWorker(ssaState);
			var b = (BinaryExpression) worker.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
			Assert.AreEqual("a < b", b.ToString());
			Assert.AreEqual("RltOperator", b.Operator.GetType().Name);
		}

        [Test]
        [Category(Categories.UnitTests)]
        public void Cce_TypeReferenceComparisonFromConditionCode()
        {
            Given_ConditionCodeEliminator();
            var w16 = new TypeReference("W16", PrimitiveType.Word16);
            var bin = m.IAdd(
                new Identifier("a", w16, null),
                new Identifier("b", w16, null));
            var worker = cce.CreateWorker(ssaState);
            var b = (BinaryExpression)worker.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
            Assert.AreEqual("a + b < 0<16>", b.ToString());
            Assert.AreEqual("LtOperator", b.Operator.GetType().Name);
        }

        private Identifier MockReg(ProcedureBuilder m, int i)
        {
            return m.Frame.EnsureRegister(FakeArchitecture.GetMachineRegister(i));
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceAddAdcPattern()
        {
            var p = new ProgramBuilder(new FakeArchitecture(new ServiceContainer()));
            p.Add("main", (m) =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var r3 = m.Reg32("r3", 3);
                var r4 = m.Reg32("r4", 4);
                var flags = RegisterStorage.Reg32("flags", 0x0A);
                var SCZ = m.Frame.EnsureFlagGroup(flags, 0x7, "SZC");
                var C = m.Frame.EnsureFlagGroup(flags, 0x4, "C");

                m.Assign(r1, m.IAdd(r1, r2));
                m.Assign(SCZ, m.Cond(r1));
                m.Assign(r3, m.IAdd(m.IAdd(r3, r4), C));
                m.MStore(m.Word32(0x0444400), r1);
                m.MStore(m.Word32(0x0444404), r3);
                m.Return();
            });
            RunTest(p, "Analysis/CceAddAdcPattern.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceShrRcrPattern()
        {
            var p = new ProgramBuilder(new FakeArchitecture(new ServiceContainer()));
            p.Add("main", (m) =>
            {
                var C = m.Flags("C");
                var r1 = MockReg(m, 1);
                var r2 = MockReg(m, 2);

                m.Assign(r1, m.Shr(r1, 1));
                m.Assign(C, m.Cond(r1));
                m.Assign(r2, m.Fn(
                    CommonOps.RorC.MakeInstance(r2.DataType, PrimitiveType.Byte),
                    r2, Constant.Byte(1), C));
                m.Assign(C, m.Cond(r2));
                m.MStore(m.Word32(0x3000), r2);
                m.MStore(m.Word32(0x3004), r1);
                m.Return();
            });
            RunTest(p, "Analysis/CceShrRcrPattern.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceShlRclPattern()
        {
            var p = new ProgramBuilder();
            p.Add("main", (m) =>
            {
                var C = m.Flags("C");
                var r1 = MockReg(m, 1);
                var r2 = MockReg(m, 2);

                m.Assign(r1, m.Shl(r1, 1));
                m.Assign(C, m.Cond(r1));
                m.Assign(r2, m.Fn(
                    CommonOps.RolC.MakeInstance(r2.DataType, PrimitiveType.Byte),
                    r2, Constant.Byte(1), C));
                m.Assign(C, m.Cond(r2));
                m.MStore(m.Word32(0x3000), r1);
                m.MStore(m.Word32(0x3004), r2);
                m.Return();
            });
            RunTest(p, "Analysis/CceShlRclPattern.txt");
        }

        [Test]
        [Ignore("//$TODO: This is difficult code, so we leave it for later")]
        [Category(Categories.IntegrationTests)]
        public void CceIsqrt()
        {
            RunFileTest_x86_real("Fragments/isqrt.asm", "Analysis/CceIsqrt.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void CceFCmp()
        {
            var p = new ProgramBuilder();
            p.Add(new FCmpFragment());
            RunTest(p, "Analysis/CceFCmp.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CceUnsignedRange()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r2
	// succ:  l1
l1:
	branch r2 >u 7<32> || r2 <u 2<32> mElse
	// succ:  mDo mElse
mDo:
	Mem5[0x00123400<p32>:word32] = r2
	// succ:  mElse
mElse:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion

            RunStringTest(sExp, m =>
            {
                var r1 = MockReg(m, 1);
                var r2 = MockReg(m, 2);
                var SCZ = m.Flags("SCZ");
                var CZ = m.Flags("CZ");

                m.Assign(r1, m.ISub(r2, 2));
                m.Assign(SCZ, m.Cond(m.ISub(r1, 5)));
                m.BranchIf(m.Test(ConditionCode.UGT, CZ), "mElse");

                m.Label("mDo");
                m.MStore(m.Ptr32(0x00123400), r2);

                m.Label("mElse");
                m.Return();
            });
        }

        [Test(Description = "Handle x86-style test/jbe sequence")]
        [Category(Categories.UnitTests)]
        public void CceTestBe()
        {
            var SZO = m.Flags("SZO");
            var C = m.Flags("C");
            var CZ = m.Flags("CZ");
            var r1 = m.Reg32("r1", 1);

            m.Assign(SZO, m.Cond(m.And(r1, r1)));
            m.Assign(C, false);
            var block = m.Block;
            m.BranchIf(m.Test(ConditionCode.ULE, CZ), "yay");
            m.Label("nay");
            m.Return(m.Word32(0));
            m.Label("yay");
            m.Return(m.Word32(1));

            var program = new Program {
                Architecture = m.Architecture,
                SegmentMap = segmentMap,
                Memory = new ByteProgramMemory(segmentMap),
                Platform = new DefaultPlatform(sc, m.Architecture)
            };

            var context = CreateAnalysisContext(program, m.Procedure);
            
            var ssa = new SsaTransform(
                program,
                m.Procedure,
                new HashSet<Procedure> { m.Procedure }, 
                context.DynamicLinker, 
                new ProgramDataFlow());
            this.ssaState = ssa.Transform();
            var vp = new ValuePropagator(context);
            vp.Transform(ssaState);
            cce = new ConditionCodeEliminator(context);

            cce.Transform(ssaState);

            Assert.AreEqual("branch r1 <=u 0<32> yay", block.Statements[^1].Instruction.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CceUInt64()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def rax
	// succ:  l1
l1:
	branch rax >u 0x1FFFFFFFFFFFFFFF<64> || rax <u 1<64> mElse
	// succ:  mDo mElse
mDo:
	Mem5[0x00123400<p32>:word64] = 0x1FFFFFFFFFFFFFFE<64>
	// succ:  mElse
mElse:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion

            RunStringTest(sExp, m =>
            {
                var rdx = m.Reg64("rdx", 2);
                var rax = m.Reg64("rax", 0);
                var CZ = m.Flags("CZ");

                m.Assign(rdx, m.ISub(rax, 1));
                m.Assign(rax, Constant.Word64(0x1FFFFFFFFFFFFFFE));
                m.Assign(CZ, m.Cond(m.ISub(rdx, rax)));
                m.BranchIf(m.Test(ConditionCode.UGT, CZ), "mElse");

                m.Label("mDo");
                m.MStore(m.Ptr32(0x00123400), rax);

                m.Label("mElse");
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CceRorcWithIntermediateCopy()
        {
            var sExp =
            #region Expected
@"// RorChainFragment
// Return size: 0
define RorChainFragment
RorChainFragment_entry:
	def h
	def l
	def c
	// succ:  l1
l1:
	// succ:  m1Loop
m1Loop:
	c_16 = PHI((c, l1), (c_17, m1Loop))
	l_11 = PHI((l, l1), (l_15, m1Loop))
	h_3 = PHI((h, l1), (h_10, m1Loop))
	v12_25 = SEQ(h_3, l_11) >>u 1<8>
	a_8 = SLICE(v12_25, byte, 8)
	h_10 = a_8
	a_13 = SLICE(v12_25, byte, 0)
	l_15 = a_13
	c_17 = c_16 - 1<8>
	branch c_17 != 0<8> m1Loop
	// succ:  m2Done m1Loop
m2Done:
	Mem20[0x1000<32>:byte] = a_13
	Mem21[0x1001<32>:byte] = a_8
	return
	// succ:  RorChainFragment_exit
RorChainFragment_exit:

";
            #endregion
            RunStringTest(sExp, new RorChainFragment());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CceUnorderedComparison()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def rArg0
	def rArg1
	// succ:  l1
l1:
	branch !isunordered(rArg0, rArg1) m3Done
	// succ:  m1isNan m3Done
m1isNan:
	return 0<32>
	// succ:  ProcedureBuilder_exit
m3Done:
	return 1<32>
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion
            RunStringTest(sExp, m =>
            {
                var r0 = m.Reg32("r0", 0);
                var f0 = m.Frame.EnsureFpuStackVariable(0, PrimitiveType.Real80);
                var f1 = m.Frame.EnsureFpuStackVariable(1, PrimitiveType.Real80);
                var C = m.Flags("C");

                m.Assign(C, m.Cond(m.FSub(f0, f1)));
                m.BranchIf(m.Test(ConditionCode.NOT_NAN, C), "m3Done");
                m.Label("m1isNan");
                m.Assign(r0, 0);
                m.Return(r0);
                m.Label("m3Done");
                m.Assign(r0, 1);
                m.Return(r0);
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CceMultibitCcFromPhiNode()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r0
	def r2
	// succ:  l1
l1:
	branch r0 <= r2 m1
	// succ:  m0 m1
m0:
	r0_5 = r0 + r2
	v14_19 = r0_5 == 0<32>
	v11_16 = r0_5 <=u 0<32>
	v8_13 = r0_5 >u 0<32>
	goto m2
	// succ:  m2
m1:
	r0_3 = r2 - r0
	v15_20 = r0_3 == 0<32>
	v12_17 = r0_3 <=u 0<32>
	v9_14 = r0_3 >u 0<32>
	// succ:  m2
m2:
	v10_15 = PHI((v11_16, m0), (v12_17, m1))
	v7_12 = PHI((v8_13, m0), (v9_14, m1))
	v13_18 = PHI((v14_19, m0), (v15_20, m1))
	Mem8[0x123400<32>:int8] = CONVERT(v7_12, bool, int8)
	Mem9[0x123402<32>:int8] = CONVERT(v10_15, bool, int8)
	Mem11[0x123404<32>:int8] = CONVERT(v13_18, bool, int8)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion
            RunStringTest(sExp, m =>
            {
                var r0 = m.Reg32("r0", 0);
                var r2 = m.Reg32("r2", 2);
                var CZ = m.Flags("CZ");
                var Z = m.Flags("Z");
                m.BranchIf(m.Le(r0, r2), "m1");

                m.Label("m0");
                m.Assign(r0, m.IAdd(r0, r2));
                m.Assign(CZ, m.Cond(r0));
                m.Goto("m2");

                m.Label("m1");
                m.Assign(r0, m.ISub(r2, r0));
                m.Assign(CZ, m.Cond(r0));

                m.Label("m2");
                //m.Assign(tmp, m.Convert(m.Test(ConditionCode.UGT, CZ), PrimitiveType.Bool, PrimitiveType.SByte));
                m.MStore(m.Word32(0x00123400), m.Convert(m.Test(ConditionCode.UGT, CZ), PrimitiveType.Bool, PrimitiveType.SByte));
                m.MStore(m.Word32(0x00123402), m.Convert(m.Test(ConditionCode.ULE, CZ), PrimitiveType.Bool, PrimitiveType.SByte));
                m.MStore(m.Word32(0x00123404), m.Convert(m.Test(ConditionCode.EQ, Z), PrimitiveType.Bool, PrimitiveType.SByte));
                m.Return();
            });
        }

        [Test]
        public void CceShlRcl_Through_Alias()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r1
	def r0
	// succ:  l1
l1:
	v10_13 = SEQ(r0, r1) << 1<8>
	r1_2 = SLICE(v10_13, word16, 0)
	r0_7 = SLICE(v10_13, word16, 16)
	Mem9[0x1234<16>:word16] = r0_7
	Mem10[0x1236<16>:word16] = r1_2
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion
            RunStringTest(sExp, m =>
            {
                var r1 = m.Reg16("r1", 1);
                var r0 = m.Reg16("r0", 0);
                var psw = RegisterStorage.Reg16("psw", 2);
                var C = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 1, "C"));
                var NZVC = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0xF, "NZVC"));
                var tmp = m.Frame.CreateTemporary("tmp", PrimitiveType.Word16);
                m.Assign(r1, m.Shl(r1, m.Int16(1)));
                m.Assign(NZVC, m.Cond(r1));
                m.Assign(tmp, r0);
                m.Assign(r0, m.Fn(
                    CommonOps.RolC.MakeInstance(r0.DataType, PrimitiveType.Int16), 
                    r0, m.Int16(1), C));
                m.Assign(C, m.Ne0(m.And(tmp, m.Word16(0x8000))));
                m.MStore(m.Word16(0x1234), r0);
                m.MStore(m.Word16(0x1236), r1);
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void CceRorcViaAliases()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def h
	def l
	def b
	def c
	// succ:  l1
l1:
	v12_18 = SEQ(SEQ(SEQ(h, l), b), c) >>u 1<8>
	v11_17 = SLICE(v12_18, uint24, 8)
	v10_16 = SLICE(v11_17, uint16, 8)
	h_1 = SLICE(v10_16, byte, 8)
	SZC_1 = cond(h_1)
	C_1 = SZC_1 & 1<32> (alias)
	l_1 = SLICE(v10_16, byte, 0)
	SZC_2 = cond(l_1)
	C_2 = SZC_2 & 1<32> (alias)
	b = SLICE(v11_17, byte, 0)
	SZC_3 = cond(b)
	C_3 = SZC_3 & 1<32> (alias)
	c_1 = SLICE(v12_18, byte, 0)
	SZC_4 = cond(c_1)
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion
            RunSsaTest(sExp, m =>
            {
                var h = m.Reg8("h");
                var h_1 = m.Reg8("h_1", 1);
                var l = m.Reg8("l", 2);
                var l_1 = m.Reg8("l_1", 2);
                var b = m.Reg8("b", 3);
                var b_1 = m.Reg8("b", 3);
                var c = m.Reg8("c", 4);
                var c_1 = m.Reg8("c_1", 4);
                var flags = RegisterStorage.Reg32("flags", 42);
                var szc = new FlagGroupStorage(flags, 7, "SZC");
                var cy = new FlagGroupStorage(flags, 1, "C");
                var SZC_1 = m.Flags("SZC_1", szc);
                var SZC_2 = m.Flags("SZC_2", szc);
                var SZC_3 = m.Flags("SZC_3", szc);
                var SZC_4 = m.Flags("SZC_4", szc);
                var C_1 = m.Flags("C_1", cy);
                var C_2 = m.Flags("C_2", cy);
                var C_3 = m.Flags("C_3", cy);
                var Bool = PrimitiveType.Bool;

                m.AddDefToEntryBlock(h);
                m.AddDefToEntryBlock(l);
                m.AddDefToEntryBlock(b);
                m.AddDefToEntryBlock(c);

                m.Assign(h_1, m.Shr(h, 1));
                m.Assign(SZC_1, m.Cond(h_1));
                m.Alias(C_1, m.And(SZC_1, 1));

                m.Assign(l_1, RorC(l, m.Byte(1), C_1));
                m.Assign(SZC_2, m.Cond(l_1));
                m.Alias(C_2, m.And(SZC_2, 1));

                m.Assign(b_1, RorC(b, m.Byte(1), C_2));
                m.Assign(SZC_3, m.Cond(b_1));
                m.Alias(C_3, m.And(SZC_3, 1));

                m.Assign(c_1, RorC(c, m.Byte(1), C_3));
                m.Assign(SZC_4, m.Cond(c_1));

                m.Return();

                //h_4 = h_3 >>u 1
                //SZXC_5 = cond(h_4)
                //C_7 = SLICE(SZXC_5, bool, 0) (alias)
                //l_8 = __rcr(l_6, 0x01, C_7)
                //SZXC_9 = cond(l_8)
                //C_11 = SLICE(SZXC_9, bool, 0) (alias)
                //b_12 = __rcr(b_10, 0x01, C_11)
                //SZXC_13 = cond(b_12)
                //C_15 = SLICE(SZXC_13, bool, 0) (alias)
                //c_16 = __rcr(c_14, 0x01, C_15)
                //SZXC_17 = cond(c_16)
            });
        }

        [Test]
        public void CceAdd16to32()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def fp
	def ax_1
	def dx_2
	// succ:  l1
l1:
	ax_3 = ax_1 + Mem12[fp + 2<32>:word16]
	SCZO_4 = cond(ax_3)
	C_5 = SLICE(SCZO_4, bool, 1) (alias)
	dx_6 = dx_2 + CONVERT(ax_3 <u 0<16>, bool, word16)
	ax_7 = ax_3 + Mem13[fp + 6<32>:word16]
	SCZO_8 = cond(ax_7)
	C_9 = SLICE(SCZO_8, bool, 1)
	dx_10 = dx_6 + Mem14[fp + 8<32>:word16] + CONVERT(ax_7 <u 0<16>, bool, word16)
	SCZO_11 = cond(dx_10)
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            RunSsaTest(sExp, m =>
            {
                var flags = RegisterStorage.Reg32("flags", 42);
                var sczo = new FlagGroupStorage(flags, 0xF, "SZCO");
                var cy = new FlagGroupStorage(flags, 1, "C");
                var fp = m.FramePointer();
                var ax_1 = m.Reg16("ax_1");
                var dx_2 = m.Reg16("dx_2");
                var ax_3 = m.Reg16("ax_3");
                var SCZO_4 = m.Flags("SCZO_4", sczo);
                var C_5 = m.Flags("C_5", cy);
                var dx_6 = m.Reg16("dx_6");
                var ax_7 = m.Reg16("ax_7");
                var SCZO_8 = m.Flags("SCZO_8", sczo);
                var C_9 = m.Flags("C_9", cy);
                var dx_10 = m.Reg16("dx_10");
                var SCZO_11 = m.Flags("SCZO_11", sczo);

                m.AddDefToEntryBlock(fp);
                m.AddDefToEntryBlock(ax_1);
                m.AddDefToEntryBlock(dx_2);
                m.Assign(ax_3, m.IAdd(ax_1, m.Mem16(m.IAdd(fp, 2))));
                m.Assign(SCZO_4, m.Cond(ax_3));
                m.Alias(C_5, m.Slice(SCZO_4, PrimitiveType.Bool, 1));
                m.Assign(dx_6, m.IAdd(dx_2, C_5));

                m.Assign(ax_7, m.IAdd(ax_3, m.Mem16(m.IAdd(fp, 6))));
                m.Assign(SCZO_8, m.Cond(ax_7));
                m.Assign(C_9, m.Slice(SCZO_8, PrimitiveType.Bool, 1));
                m.Assign(dx_10, m.IAdd(m.IAdd(dx_6, m.Mem16(m.IAdd(fp, 8))), C_9));
                m.Assign(SCZO_11, m.Cond(dx_10));
                m.Return();
            });
        }

        [Test]
        public void CceLongShiftInLoop()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def cx
	def ax
	def dx
	// succ:  l1
l1:
	branch cx == 0<16> m1Done
	// succ:  m0Loop m1Done
m0Loop:
	cx_10 = PHI((cx, l1), (cx_11, m0Loop))
	dx_5 = PHI((dx, l1), (dx_8, m0Loop))
	ax_2 = PHI((ax, l1), (ax_3, m0Loop))
	v11_20 = SEQ(dx_5, ax_2) << 1<8>
	ax_3 = SLICE(v11_20, word16, 0)
	dx_8 = SLICE(v11_20, word16, 16)
	cx_11 = cx_10 - 1<16>
	branch cx_11 != 0<16> m0Loop
	// succ:  m1Done m0Loop
m1Done:
	dx_15 = PHI((dx, l1), (dx_8, m0Loop))
	ax_12 = PHI((ax, l1), (ax_3, m0Loop))
	Mem14[0x123400<32>:word16] = ax_12
	Mem17[0x123402<32>:word16] = dx_15
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion
            RunStringTest(sExp, m =>
            {
                var ax = m.Reg16("ax", 0);
                var cx = m.Reg16("cx", 1);
                var dx = m.Reg16("dx", 2);
                var psw = RegisterStorage.Reg16("psw", 2);
                var CF = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 1, "C"));
                var SCZO = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0xF, "SCZO"));
                var tmp = m.Frame.CreateTemporary(PrimitiveType.Bool);

                m.BranchIf(m.Eq0(cx), "m1Done");

                m.Label("m0Loop");
                m.Assign(ax, m.Shl(ax, 1));
                m.Assign(SCZO, m.Cond(ax));
                m.Assign(tmp, m.Ne0(m.And(dx, 0x8000)));
                m.Assign(dx, m.Fn(
                    CommonOps.RolC.MakeInstance(dx.DataType, PrimitiveType.Byte),
                    dx, Constant.Byte(1), CF));
                m.Assign(CF, tmp);
                m.Assign(cx, m.ISub(cx, 1));
                m.BranchIf(m.Ne0(cx), "m0Loop");

                m.Label("m1Done");
                m.MStore(m.Word32(0x00123400), ax);
                m.MStore(m.Word32(0x00123402), dx);
                m.Return();

                m.Use(SCZO);
            });
        }

        [Test]
        public void CceForceFlagAlive()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def dx
	def ax
	// succ:  l1
l1:
	v8_9 = SEQ(dx, ax) >>u 1<16>
	dx_2 = SLICE(v8_9, word16, 16)
	ax_6 = SLICE(v8_9, word16, 0)
	Mem7[0x1234<16>:word16] = ax_6
	Mem8[0x1236<16>:word16] = dx_2
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion

            RunStringTest(sExp, m =>
            {
                var ax = m.Reg16("ax", 0);
                var dx = m.Reg16("dx", 2);
                var psw = RegisterStorage.Reg16("psw", 2);
                var CF = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 1, "C"));
                var SF = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 8, "S"));
                var SCZO = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0xF, "SCZO"));

                m.Assign(dx, m.Sar(dx, 1));
                m.Assign(SCZO, m.Cond(dx));
                m.Assign(ax, RorC(ax, m.Word16(1), CF));
                m.MStore(m.Word16(0x1234), ax);
                m.MStore(m.Word16(0x1236), dx);
                m.Return();

                m.Use(ax);
                m.Use(dx);
                m.Use(SF);
            });
        }

        [Test]
        public void CceGithub1168()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r2
	def Mem0
	def ctr
	// succ:  m1
m1:
	r2_2 = r2 & 0x7F<32>
	v7_20 = (r2 & 0x7F<32>) == 0<32>
	// succ:  m2C
m2C:
	ctr_12 = PHI((ctr, m1), (ctr_12, m2C), (ctr_13, m80))
	v6_19 = PHI((v7_20, m1), (v6_19, m2C), (v8_21, m80))
	r2_4 = PHI((r2_2, m1), (r2_5, m2C), (r2_14, m80))
	r2_5 = r2_4 >>u 1<8>
	branch r2_5 == 0<32> m2C
	// succ:  m3 m2C
m3:
	branch v6_19 m80
	// succ:  m4 m80
m4:
	r2_8 = Mem0[0x123400<32>:word32]
	r2_9 = r2_8 & 0x7F<32>
	v9_22 = (r2_8 & 0x7F<32>) == 0<32>
	// succ:  m80
m80:
	v8_21 = PHI((v6_19, m3), (v9_22, m4))
	r2_14 = PHI((r2_5, m3), (r2_9, m4))
	ctr_13 = ctr_12 - 1<i32>
	branch ctr_13 != 0<32> m2C
	// succ:  m9 m2C
m9:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion

            RunStringTest(sExp, m =>
            {
                var r2 = m.Reg32("r2", 2);
                var ctr = m.Reg32("ctr", 12);
                var psw = RegisterStorage.Reg16("psw", 2);
                var SCZO = m.Frame.EnsureFlagGroup(psw, 0xF, "SCZO");

                m.Label("m1");
                m.Assign(r2, m.And(r2, 0x7F));
                m.Assign(SCZO, m.Cond(r2));

                m.Label("m2C");
                m.Assign(r2, m.Shr(r2, 1));
                m.BranchIf(m.Eq0(r2), "m2C");

                m.Label("m3");
                m.BranchIf(m.Test(ConditionCode.EQ, SCZO), "m80");

                m.Label("m4");
                m.Assign(r2, m.Mem32(m.Word32(0x00123400)));
                m.Assign(r2, m.And(r2, 0x7F));
                m.Assign(SCZO, m.Cond(r2));

                m.Label("m80");
                m.Assign(ctr, m.ISubS(ctr, 1));
                m.BranchIf(m.Ne0(ctr), "m2C");
                m.Label("m9");
                m.Return();
            });
        }

        [Test]
        [Ignore("This type of code doesn't seem to be generated anymore")]
        public void CceShrConvert()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r2
	// succ:  l1
l1:
	r3_4 = (r2 & 1<32>) != 0<32> ? 1<32> : 0<32>
	return r3_4
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion

            RunStringTest(sExp, m =>
            {
                var r2 = m.Reg32("r2", 2);
                var r3 = m.Reg32("r3", 3);
                var psw = RegisterStorage.Reg32("psw", 4);
                var C = m.Frame.EnsureFlagGroup(m.Architecture.CarryFlag);

                m.Assign(r2, m.Shr(r2, 1));
                m.Assign(C, m.Cond(r2));
                m.Assign(r3, C);
                m.Return(r3);
            });
        }

        [Test]
        public void CceRegression00939()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def eax
	// succ:  foo
foo:
	branch eax <= 0<32> foo
	// succ:  l1 foo
l1:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion
            RunStringTest(sExp, m =>
            {
                var eax = m.Reg32("eax", 1);
                var psw = RegisterStorage.Reg32("psw", 4);
                var SZ = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0x0C, "SZ"));
                var SZO = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0x0E, "SZO"));
                var O = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0x02, "O"));
                var C = m.Frame.EnsureFlagGroup(m.Architecture.CarryFlag);
                m.Label("foo");
                m.Assign(eax, m.Or(eax, eax));
                m.Assign(SZ, m.Cond(eax));
                m.Assign(O, 0);
                m.Assign(C, 0);
                m.BranchIf(m.Test(ConditionCode.LE, SZO), "foo");
                m.Return();
            });
        }

        [Test(Description = "Treat condition codes as simple bit flags if they're not the result of Cond pseudo-expressions")]
        public void CceSingleBitTest_noCond_true()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	// succ:  foo
foo:
	eax_1 = 0x123400<32>()
	branch (eax_1 & 1<32>) != 0<32> foo
	// succ:  l1 foo
l1:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion
            RunStringTest(sExp, m =>
            {
                var eax = m.Reg32("eax", 1);
                var psw = RegisterStorage.Reg32("psw", 4);
                var C = m.Frame.EnsureFlagGroup(m.Architecture.CarryFlag);
                var tmp = m.Frame.CreateTemporary(PrimitiveType.Bool);
                m.Label("foo");
                m.Assign(eax, m.Fn(m.Word32(0x00123400)));
                m.Assign(C, m.Ne0(m.And(eax, 1)));
                m.Assign(eax, m.Fn(CommonOps.Ror, eax, m.Byte(1)));
                m.BranchIf(m.Test(ConditionCode.ULT, C), "foo");
                m.Return();
            });
        }

        [Test(Description = "Treat condition codes as simple bit flags if they're not the result of Cond pseudo-expressions")]
        public void CceSingleBitTest_noCond_false()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	// succ:  foo
foo:
	eax_1 = 0x123400<32>()
	branch (eax_1 & 1<32>) == 0<32> foo
	// succ:  l1 foo
l1:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion
            RunStringTest(sExp, m =>
            {
                var eax = m.Reg32("eax", 1);
                var psw = RegisterStorage.Reg32("psw", 4);
                var C = m.Frame.EnsureFlagGroup(m.Architecture.CarryFlag);
                var tmp = m.Frame.CreateTemporary(PrimitiveType.Bool);
                m.Label("foo");
                m.Assign(eax, m.Fn(m.Word32(0x00123400)));
                m.Assign(C, m.Ne0(m.And(eax, 1)));
                m.Assign(eax, m.Fn(CommonOps.Ror, eax, m.Byte(1)));
                m.BranchIf(m.Test(ConditionCode.UGE, C), "foo");
                m.Return();
            });
        }

        [Test]
        public void CceRegression_Avr32()
        {
            var sExpected =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r9
	def r7
	// succ:  l1
l1:
	r9_5 = CONVERT(r9 >=u r7, bool, word32)
	Mem6[0x123400<32>:word32] = r9_5
ProcedureBuilder_exit:

";
            #endregion

            RunStringTest(sExpected, m =>
            {
                var psw = RegisterStorage.Reg32("psw", 42);
                var VNZC = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0xF, "VNZC"));
                var C = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 1, "C"));
                var r7 = m.Reg32("r7");
                var r9 = m.Reg32("r9");

                m.Assign(VNZC, m.Cond(m.ISub(r9, r7)));
                m.Assign(
                    r9,
                    m.Convert(
                        m.Test(ConditionCode.UGE, C),
                        PrimitiveType.Bool,
                        PrimitiveType.Word32));
                m.MStore(m.Word32(0x123400), r9);
            });
        }
    }
}

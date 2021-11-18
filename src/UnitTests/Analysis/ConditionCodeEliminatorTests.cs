#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.Design;
using Reko.Core.Services;

namespace Reko.UnitTests.Analysis
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
            freg = new RegisterStorage("flags", 32, 0, PrimitiveType.Word32);
            segmentMap = new SegmentMap(Address.Ptr32(0));
		}

        private void Given_ConditionCodeEliminator()
        {
            var program = new Program
            {
                Platform = new DefaultPlatform(null, new FakeArchitecture(new ServiceContainer()))
            };
            cce = new ConditionCodeEliminator(program, ssaState, new FakeDecompilerEventListener());
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
            return ssaIds.Add(id, null, null, false).Identifier;
        }

        private Identifier FlagGroup(string name)
        {
            Identifier id = new Identifier(
                name,
                PrimitiveType.Word32,
                new FlagGroupStorage(freg, 1U, "C", PrimitiveType.Byte));
            return ssaIds.Add(id, null, null, false).Identifier;
        }

        protected override void RunTest(Program program, TextWriter writer)
        {
            var dynamicLinker = new Mock<IDynamicLinker>().Object;
            var listener = new FakeDecompilerEventListener();
            var sc = new ServiceContainer();
            sc.AddService<DecompilerEventListener>(listener);
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

                var larw = new LongAddRewriter(ssa, listener);
                larw.Transform();

                var cce = new ConditionCodeEliminator(program, ssa, listener);
                cce.Transform();
                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });

                var vp = new ValuePropagator(program.SegmentMap, ssa, program.CallGraph, dynamicLinker, listener);
                vp.Transform();
                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });

                sst.RenameFrameAccesses = true;
                sst.Transform();

                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });

                // We don't add uses to exit block on purpose. We
                // are not testing interprocedural effects here.
                DeadCode.Eliminate(ssa);

                ssa.Procedure.Write(false, writer);
                writer.WriteLine();

                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });
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
            ssaIds[z].DefStatement = m.Block.Statements.Last;
            m.Assign(y, z);
            ssaIds[y].DefStatement = m.Block.Statements.Last;
			ssaIds[z].Uses.Add(m.Block.Statements.Last);
			var stmBr = m.BranchIf(m.Test(ConditionCode.EQ, y), "foo");
            ssaIds[y].Uses.Add(stmBr);

            Given_ConditionCodeEliminator();
			cce.Transform();
			Assert.AreEqual("branch r == 0<32> foo", stmBr.Instruction.ToString());
		}

		[Test]
        [Category(Categories.UnitTests)]
		public void CceSetnz()
		{
			Identifier r = Reg32("r");
			Identifier Z = FlagGroup("Z");
			Identifier f = Reg32("f");

			Statement stmZ = new Statement(0, m.Assign(Z, m.Cond(m.ISub(r, 0))), null);
			ssaIds[Z].DefStatement = stmZ;
			Statement stmF = new Statement(0, m.Assign(f, m.Test(ConditionCode.NE, Z)), null);
			ssaIds[f].DefStatement = stmF;
			ssaIds[Z].Uses.Add(stmF);

            Given_ConditionCodeEliminator();
			cce.Transform();
			Assert.AreEqual("f = r != 0<32>", stmF.Instruction.ToString());
		}

        [Test]
        [Category(Categories.UnitTests)]
		public void Cce_SignedIntComparisonFromConditionCode()
        {
            Given_ConditionCodeEliminator();
            var bin = m.ISub(new Identifier("a", PrimitiveType.Word16, null), new Identifier("b", PrimitiveType.Word16, null));
            var b = (BinaryExpression)cce.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
            Assert.AreEqual("a < b", b.ToString());
            Assert.AreEqual("LtOperator", b.Operator.GetType().Name);
        }


        [Test]
        [Category(Categories.UnitTests)]
		public void Cce_RealComparisonFromConditionCode()
		{
            Given_ConditionCodeEliminator();
			var bin = m.FSub(new Identifier("a", PrimitiveType.Real64, null), new Identifier("b", PrimitiveType.Real64, null));
			BinaryExpression b = (BinaryExpression) cce.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
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
            var b = (BinaryExpression)cce.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
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
                var flags = new RegisterStorage("flags", 0x0A, 0, PrimitiveType.Word32);
                var SCZ = m.Frame.EnsureFlagGroup(flags, 0x7, "SZC", PrimitiveType.Byte);
                var C = m.Frame.EnsureFlagGroup(flags, 0x4, "C", PrimitiveType.Byte);

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
                    new IntrinsicProcedure(IntrinsicProcedure.RorC, false, r2.DataType, 2),
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
                    new IntrinsicProcedure(IntrinsicProcedure.RolC, false, r2.DataType, 2),
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

            var ssa = new SsaTransform(
                new Program { Architecture = m.Architecture }, 
                m.Procedure,
                new HashSet<Procedure> { m.Procedure }, 
                null, 
                new ProgramDataFlow());
            this.ssaState = ssa.Transform();
            var vp = new ValuePropagator(segmentMap, ssaState, new CallGraph(), null, new FakeDecompilerEventListener());
            vp.Transform();
            Given_ConditionCodeEliminator();
            cce.Transform();

            Assert.AreEqual("branch r1 <=u 0<32> yay", block.Statements.Last.Instruction.ToString());
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
	h_10 = a_8
	v13_27 = SEQ(h_3, l_11) >>u 1<8>
	a_8 = SLICE(v13_27, byte, 8)
	a_13 = SLICE(v13_27, byte, 0)
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
	v12_18 = r0_5 == 0<32>
	v9_15 = r0_5 <=u 0<32>
	v6_12 = r0_5 >u 0<32>
	goto m2
	// succ:  m2
m1:
	r0_3 = r2 - r0
	v13_19 = r0_3 == 0<32>
	v10_16 = r0_3 <=u 0<32>
	v7_13 = r0_3 >u 0<32>
	// succ:  m2
m2:
	v11_17 = PHI((v9_15, m0), (v10_16, m1))
	v8_14 = PHI((v6_12, m0), (v7_13, m1))
	v14_20 = PHI((v12_18, m0), (v13_19, m1))
	Mem8[0x123400<32>:int8] = CONVERT(v8_14, bool, int8)
	Mem9[0x123402<32>:int8] = CONVERT(v11_17, bool, int8)
	Mem11[0x123404<32>:int8] = CONVERT(v14_20, bool, int8)
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
	v9_13 = SEQ(r0, r1) << 1<8>
	r1_2 = SLICE(v9_13, word16, 0)
	r0_7 = SLICE(v9_13, word16, 16)
	Mem9[0x1234<16>:word16] = r0_7
	Mem10[0x1236<16>:word16] = r1_2
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            #endregion
            RunStringTest(sExp, m => {
                var RolC = new ProcedureConstant(PrimitiveType.Ptr32, new IntrinsicProcedure(
                    IntrinsicProcedure.RolC, false, PrimitiveType.Word16, 3));
                var r1 = m.Reg16("r1", 1);
                var r0 = m.Reg16("r0", 0);
                var psw = new RegisterStorage("psw", 2, 0, PrimitiveType.Word16);
                var C = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 1, "C", PrimitiveType.Bool));
                var NZVC = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0xF, "NZVC", PrimitiveType.Word16));
                var tmp = m.Frame.CreateTemporary("tmp", PrimitiveType.Word16);
                m.Assign(r1, m.Shl(r1 , m.Int16(1)));
                m.Assign(NZVC, m.Cond(r1));
                m.Assign(tmp, r0);
                m.Assign(r0, m.Fn(RolC, r0, m.Int16(1), C));
                m.Assign(C, m.Ne0(m.And(tmp, m.Word16(0x8000))));
                m.MStore(m.Word16(0x1234), r0);
                m.MStore(m.Word16(0x1236), r1);
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
	v10_20 = SEQ(dx_5, ax_2) << 1<8>
	ax_3 = SLICE(v10_20, word16, 0)
	dx_8 = SLICE(v10_20, word16, 16)
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
                static Expression RolC(ExpressionEmitter m, Expression e, Expression sh, Expression c)
                {
                    var sig = FunctionType.Func(
                        new Identifier("", e.DataType, null!),
                        new Identifier("e", e.DataType, null!),
                        new Identifier("sh", e.DataType, null!),
                        new Identifier("c", e.DataType, null!));
                    return m.Fn(new IntrinsicProcedure(IntrinsicProcedure.RolC, false, sig), e, sh, c);
                }

                var ax = m.Reg16("ax", 0);
                var cx = m.Reg16("cx", 1);
                var dx = m.Reg16("dx", 2);
                var psw = new RegisterStorage("psw", 2, 0, PrimitiveType.Word16);
                var CF = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 1, "C", PrimitiveType.Bool));
                var SCZO = m.Frame.EnsureFlagGroup(new FlagGroupStorage(psw, 0xF, "SCZO", PrimitiveType.Word16));
                var tmp = m.Frame.CreateTemporary(PrimitiveType.Bool);

                m.BranchIf(m.Eq0(cx), "m1Done");

                m.Label("m0Loop");
                m.Assign(ax, m.Shl(ax, 1));
                m.Assign(SCZO, m.Cond(ax));
                m.Assign(tmp, m.Ne0(m.And(dx, 0x8000)));
                m.Assign(dx, RolC(m, dx, Constant.Byte(1), CF));
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

    }
}

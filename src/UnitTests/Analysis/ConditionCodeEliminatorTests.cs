#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.UnitTests.Fragments;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        [SetUp]
		public void Setup()
		{
            m = new ProcedureBuilder();
            ssaState = new SsaState(m.Procedure, null);
            ssaIds = ssaState.Identifiers;
            freg = new RegisterStorage("flags", 32, 0, PrimitiveType.Word32);
		}

        private void Given_ConditionCodeEliminator()
        {
            cce = new ConditionCodeEliminator(ssaState, new DefaultPlatform(null, new FakeArchitecture()));
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
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            var listener = new FakeDecompilerEventListener();
            DataFlowAnalysis dfa = new DataFlowAnalysis(program, importResolver, listener);
            foreach (var proc in program.Procedures.Values)
            {
                var sst = new SsaTransform(
                    program,
                    proc,
                    new HashSet<Procedure>(),
                    importResolver, 
                    new ProgramDataFlow());
                var ssa = sst.Transform();
                var larw = new LongAddRewriter2(program.Architecture, ssa);
                larw.Transform();

                var cce = new ConditionCodeEliminator(ssa, program.Platform);
                cce.Transform();

                var vp = new ValuePropagator(program.Architecture, ssa, listener);
                vp.Transform();

                sst.RenameFrameAccesses = true;
                sst.Transform();

                // We don't add uses to exit block on purpose. We
                // are not testing interprocedural effects here.
                DeadCode.Eliminate(ssa);

                ssa.Write(writer);
                ssa.Procedure.Write(false, writer);
                writer.WriteLine();
            }
        }

		[Test]
		public void CceAsciiHex()
		{
			RunFileTest_x86_real("Fragments/ascii_hex.asm", "Analysis/CceAsciiHex.txt");
		}

		[Test]
        //[Ignore("scanning-development")]
        public void CceAddSubCarries()
		{
			RunFileTest_x86_real("Fragments/addsubcarries.asm", "Analysis/CceAddSubCarries.txt");
		}

		[Test]
		public void CceAdcMock()
		{
			RunFileTest(new AdcMock(), "Analysis/CceAdcMock.txt");
		}

		[Test]
		public void CceCmpMock()
		{
			RunFileTest(new CmpMock(), "Analysis/CceCmpMock.txt");
		}

		[Test]
		public void CceFrame32()
        {
			RunFileTest_x86_32("Fragments/multiple/frame32.asm", "Analysis/CceFrame32.txt");
		}

		[Test]
		public void CceWhileLoop()
		{
			RunFileTest_x86_real("Fragments/while_loop.asm", "Analysis/CceWhileLoop.txt");
		}

		[Test]
        [Ignore("suffers from infinite recursion")]
        public void CceReg00005()
		{
			RunFileTest_x86_real("Fragments/regressions/r00005.asm", "Analysis/CceReg00005.txt");
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
        [Ignore("Wait until we see this in real code? If we do, we have to move the logic for FPUF into an architecture specific branch.")]
        public void CceFstswTestAxWithConstantBl()
        {
            Program program = RewriteCodeFragment(@"
                mov     bx,1
                fcomp   dword ptr[si]
                fstsw   ax
                test    bl,ah
                jz      done
                mov     byte ptr [0x0300],1
done:
                ret
");
            SaveRunOutput(program, RunTest, "Analysis/CceFstswTestAxWithConstantBl.txt");
        }

        [Test]
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
			Assert.AreEqual("branch r == 0x00000000 foo", stmBr.Instruction.ToString());
		}

		[Test]
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
			Assert.AreEqual("f = r != 0x00000000", stmF.Instruction.ToString());
		}

        [Test]
		public void Cce_SignedIntComparisonFromConditionCode()
        {
            Given_ConditionCodeEliminator();
            var bin = m.ISub(new Identifier("a", PrimitiveType.Word16, null), new Identifier("b", PrimitiveType.Word16, null));
            var b = (BinaryExpression)cce.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
            Assert.AreEqual("a < b", b.ToString());
            Assert.AreEqual("LtOperator", b.Operator.GetType().Name);
        }


        [Test]
		public void Cce_RealComparisonFromConditionCode()
		{
            Given_ConditionCodeEliminator();
			var bin = m.FSub(new Identifier("a", PrimitiveType.Real64, null), new Identifier("b", PrimitiveType.Real64, null));
			BinaryExpression b = (BinaryExpression) cce.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
			Assert.AreEqual("a < b", b.ToString());
			Assert.AreEqual("RltOperator", b.Operator.GetType().Name);
		}

        [Test]
        public void Cce_TypeReferenceComparisonFromConditionCode()
        {
            Given_ConditionCodeEliminator();
            var w16 = new TypeReference("W16", PrimitiveType.Word16);
            var bin = m.IAdd(
                new Identifier("a", w16, null),
                new Identifier("b", w16, null));
            var b = (BinaryExpression)cce.ComparisonFromConditionCode(ConditionCode.LT, bin, false);
            Assert.AreEqual("a + b < 0x0000", b.ToString());
            Assert.AreEqual("LtOperator", b.Operator.GetType().Name);
        }

        private Identifier MockReg(ProcedureBuilder m, int i)
        {
            return m.Frame.EnsureRegister(FakeArchitecture.GetMachineRegister(i));
        }

        [Test]
        public void CceAddAdcPattern()
        {
            var p = new ProgramBuilder(new FakeArchitecture());
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
                m.Store(m.Word32(0x0444400), r1);
                m.Store(m.Word32(0x0444404), r3);
                m.Return();
            });
            RunTest(p, "Analysis/CceAddAdcPattern.txt");
        }

        [Test]
        public void CceShrRcrPattern()
        {
            var p = new ProgramBuilder(new FakeArchitecture());
            p.Add("main", (m) =>
            {
                var C = m.Flags("C");
                var r1 = MockReg(m, 1);
                var r2 = MockReg(m, 2);

                m.Assign(r1, m.Shr(r1, 1));
                m.Assign(C, m.Cond(r1));
                m.Assign(r2, m.Fn(
                    new PseudoProcedure(PseudoProcedure.RorC, r2.DataType, 2),
                    r2, Constant.Byte(1), C));
                m.Assign(C, m.Cond(r2));
                m.Store(m.Word32(0x3000), r2);
                m.Store(m.Word32(0x3004), r1);
                m.Return();
            });
            RunTest(p, "Analysis/CceShrRcrPattern.txt");
        }

        [Test]
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
                    new PseudoProcedure(PseudoProcedure.RolC, r2.DataType, 2),
                    r2, Constant.Byte(1), C));
                m.Assign(C, m.Cond(r2));
                m.Store(m.Word32(0x3000), r1);
                m.Store(m.Word32(0x3004), r2);
                m.Return();
            });
            RunTest(p, "Analysis/CceShlRclPattern.txt");
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        [Category(Categories.AnalysisDevelopment)]
        public void CceIsqrt()
        {
            RunFileTest_x86_real("Fragments/isqrt.asm", "Analysis/CceIsqrt.txt");
        }

        [Test]
        public void CceFCmp()
        {
            var p = new ProgramBuilder();
            p.Add(new FCmpFragment());
            RunTest(p, "Analysis/CceFCmp.txt");
        }

        [Test]
        public void CceUnsignedRange()
        {
            var sExp =
            #region Expected
@"r2:r2
    def:  def r2
    uses: Mem5[0x00123400:word32] = r2
          branch r2 >u 0x00000007 || r2 <u 0x00000002 mElse
          branch r2 >u 0x00000007 || r2 <u 0x00000002 mElse
r1_2: orig: r1
SCZ_3: orig: SCZ
CZ_4: orig: CZ
Mem5: orig: Mem0
    def:  Mem5[0x00123400:word32] = r2
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r2
	// succ:  l1
l1:
	branch r2 >u 0x00000007 || r2 <u 0x00000002 mElse
	// succ:  mDo mElse
mDo:
	Mem5[0x00123400:word32] = r2
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
                m.Store(m.Word32(0x00123400), r2);

                m.Label("mElse");
                m.Return();
            });
        }

        [Test(Description = "Handle x86-style test/jbe sequence")]
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

            var ssa = new SsaTransform(new Program { Architecture = m.Architecture }, m.Procedure, new HashSet<Procedure> { m.Procedure }, null, new ProgramDataFlow());
            this.ssaState = ssa.Transform();
            var vp = new ValuePropagator(m.Architecture, ssaState, new FakeDecompilerEventListener());
            vp.Transform();
            Given_ConditionCodeEliminator();
            cce.Transform();

            Assert.AreEqual("branch r1 <=u 0x00000000 yay", block.Statements.Last.Instruction.ToString());
        }

        [Test]
        public void CceUInt64()
        {
            var sExp =
            #region Expected
@"rax:rax
    def:  def rax
    uses: branch rax >u 0x1FFFFFFFFFFFFFFF || rax <u 0x0000000000000001 mElse
          branch rax >u 0x1FFFFFFFFFFFFFFF || rax <u 0x0000000000000001 mElse
rdx_2: orig: rdx
rax_3: orig: rax
CZ_4: orig: CZ
Mem5: orig: Mem0
    def:  Mem5[0x00123400:word64] = 0x1FFFFFFFFFFFFFFE
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def rax
	// succ:  l1
l1:
	branch rax >u 0x1FFFFFFFFFFFFFFF || rax <u 0x0000000000000001 mElse
	// succ:  mDo mElse
mDo:
	Mem5[0x00123400:word64] = 0x1FFFFFFFFFFFFFFE
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
                m.Store(m.Word32(0x00123400), rax);

                m.Label("mElse");
                m.Return();
            });
        }
    }
}

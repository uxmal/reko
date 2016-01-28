#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    /// <summary>
    /// These tests are making sure that we can re-run SsaTransform
    /// on a procedure that already has been transformed once.
    /// </summary>
    [TestFixture]
    [Category("UnitTests")]
    public class SsaTransformTests
    {
        private ProgramBuilder pb;
        private ProgramDataFlow programFlow;
        private DataFlow2 programFlow2;
        private bool addUseInstructions;

        [SetUp]
        public void Setup()
        {
            this.pb = new ProgramBuilder();
            this.programFlow = new ProgramDataFlow();
            this.programFlow2 = new DataFlow2();
            this.addUseInstructions = false;
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder(this.pb.Program.Architecture);
            builder(pb);
            var proc = pb.Procedure;

            var ssa = new SsaTransform2(this.pb.Program.Architecture, proc, programFlow2);
            ssa.AddUseInstructions = addUseInstructions;
            ssa.Transform();

            var writer = new StringWriter();
            proc.Write(false, writer);
            var sActual = writer.ToString();
            if (sActual != sExp)
                Debug.Print(sActual);
            Assert.AreEqual(sExp, sActual);
        }

        private void RunTest_FrameAccesses(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder(this.pb.Program.Architecture);
            builder(pb);
            var proc = pb.Procedure;

            // Perform initial transformation.
            var sst = new SsaTransform2(this.pb.Program.Architecture, proc, programFlow2);
            sst.AddUseInstructions = false;
            sst.Transform();

            // Propagate values and simplify the results.
            // We hope the the sequence
            //   esp = fp - 4
            //   mov [esp-4],eax
            // will become
            //   esp_2 = fp - 4
            //   mov [fp - 8],eax

            sst.SsaState.DebugDump(true);

            var vp = new ValuePropagator(this.pb.Program.Architecture, sst.SsaState);
            vp.Transform();

            sst.RenameFrameAccesses = true;
            sst.AddUseInstructions = true;
            sst.Transform();

            var writer = new StringWriter();
            proc.Write(false, writer);
            var sActual = writer.ToString();
            if (sActual != sExp)
                Debug.Print(sActual);
            Assert.AreEqual(sExp, sActual);
        }

        [Test]
        public void SsaSimple()
        {
            var sExp =
@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r1
	def r2
	// succ:  l1
l1:
	r1_2 = r1 + r2
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use r1_2
";
            addUseInstructions = true;
            RunTest(sExp, m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                m.Assign(r1, m.IAdd(r1, r2));
                m.Return();
            });
        }

        [Test]
        public void SsaStackLocals()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def fp
	def r1
	def r2
	// succ:  l1
l1:
	r63_1 = fp
	r63_2 = fp - 0x00000004
	dwLoc04_12 = r1
	r63_5 = fp - 0x00000008
	dwLoc08_13 = r2
	r1_8 = dwLoc04_12
	r2_9 = dwLoc08_13
	r1_10 = r1_8 + r2_9
	Mem14[0x00010008:word32] = r1_10
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use dwLoc04_12
	use dwLoc08_13
	use r1_10
	use r2_9
	use r63_5
";
            addUseInstructions = true;
            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r1);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r2);
                m.Assign(r1, m.LoadDw(m.IAdd(sp, 4)));
                m.Assign(r2, m.LoadDw(sp));
                m.Assign(r1, m.IAdd(r1, r2));
                m.Store(m.Word32(0x010008), r1);
                m.Return();
            });
        }

        [Test]
        public void SsaDiamond()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def fp
	def bp
	def wArg04
	goto l1
	// succ:  l1
done:
	r1_19 = PHI(r1_8, r1_7)
	bp_11 = dwLoc04_14
	r63_13 = fp
	return
	// succ:  ProcedureBuilder_exit
ge3:
	r1_7 = 0x00000001
	goto done
	// succ:  done
l1:
	r63_1 = fp
	r63_2 = fp - 0x00000004
	dwLoc04_14 = bp
	bp_5 = fp - 0x00000004
	CZS_6 = cond(wArg04 - 0x0003)
	branch Test(GE,CZS_6) ge3
	// succ:  l2 ge3
l2:
	r1_8 = 0x00000000
	goto done
	// succ:  done
ProcedureBuilder_exit:
	use bp_11
	use CZS_6
	use dwLoc04_14
	use r1_19
	use r63_13
";
            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var cr = m.Frame.EnsureFlagGroup(flags, 0x3, "CZS", PrimitiveType.Byte);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, bp);
                m.Assign(bp, sp);
                m.Assign(cr, m.Cond( m.ISub(m.LoadW(m.IAdd(bp, 8)), 0x3)));
                m.BranchIf(m.Test(ConditionCode.GE, cr), "ge3");

                m.Assign(r1, 0);
                m.Goto("done");

                m.Label("ge3");
                m.Assign(r1, 1);

                m.Label("done");
                m.Assign(bp, m.LoadDw(sp));
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
        }

        [Test]
        public void SsarDiamondFrame()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def fp
	def bp
	def wArg04
	def r1
	goto l1
	// succ:  l1
done:
	wArg04_24 = PHI(wArg04_18, wArg04_17)
	r1_22 = PHI(r1, r1_8)
	Mem11 = PHI(Mem9, Mem7)
	bp_12 = dwLoc04_15
	r63_14 = fp
	return
	// succ:  ProcedureBuilder_exit
ge3:
	wArg04_17 = -3
	r1_8 = 0x00000001
	goto done
	// succ:  done
l1:
	r63_1 = fp
	r63_2 = fp - 0x00000004
	dwLoc04_15 = bp
	bp_5 = fp - 0x00000004
	CZS_6 = wArg04 - 0x0003
	branch Test(GE,CZS_6) ge3
	// succ:  l2 ge3
l2:
	wArg04_18 = 0x0003
	goto done
	// succ:  done
ProcedureBuilder_exit:
	use bp_12
	use CZS_6
	use dwLoc04_15
	use r1_22
	use r63_14
	use wArg04_24
";
            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var cr = m.Frame.EnsureFlagGroup(flags, 0x3, "CZS", PrimitiveType.Byte);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, bp);
                m.Assign(bp, sp);
                m.Assign(cr, m.ISub(m.LoadW(m.IAdd(bp, 8)), 0x3));
                m.BranchIf(m.Test(ConditionCode.GE, cr), "ge3");

                m.Store(m.IAdd(bp, 8), m.Word16(3));
                m.Goto("done");

                m.Label("ge3");
                m.Store(m.IAdd(bp, 8), Constant.Int16(-3));
                m.Assign(r1, 1);

                m.Label("done");
                m.Assign(bp, m.LoadDw(sp));
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
        }

        [Test]
        public void SsaSubroutine()
        {
            Storage r1_ = null;
            // Simulate the creation of a subroutine.
            var procSub = this.pb.Add("Adder", m =>
            {
                r1_ = m.Register(1).Storage;
            });

            var procSubFlow = new ProcedureFlow2 { Trashed = { r1_ } };
            programFlow2.ProcedureFlows.Add(procSub, procSubFlow);

            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	// succ:  l1
l1:
	r1_0 = 0x00000003
	r2_1 = 0x00000004
	call Adder (retsize: 4;)
		defs: r1_2
	Mem3[0x00012300:word32] = r1_2
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use r1_2
	use r2_1
";
            addUseInstructions = true;
            RunTest(sExp, m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                m.Assign(r1, 3);
                m.Assign(r2, 4);
                m.Call(procSub, 4);
                m.Store(m.Word32(0x012300), r1);
                m.Return();
            });
        }

        [Test]
        public void SsarLocaldiamond()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def fp
	def bp
	def r1
	def wArg04
	goto l1
	// succ:  l1
done:
	dwLoc0C_23 = PHI(dwLoc0C_22, dwLoc0C_21)
	Mem12 = PHI(Mem10, Mem9)
	r1_13 = dwLoc0C_23
	bp_16 = dwLoc04_18
	r63_17 = fp
	return
	// succ:  ProcedureBuilder_exit
ge3:
	dwLoc0C_21 = r1
	goto done
	// succ:  done
l1:
	r63_1 = fp
	r63_2 = fp - 0x00000004
	dwLoc04_18 = bp
	bp_5 = fp - 0x00000004
	dwLoc0C_19 = 0x00000000
	CZS_7 = wArg04 - 0x0003
	branch Test(GE,CZS_7) ge3
	// succ:  l2 ge3
l2:
	dwLoc0C_22 = r1
	goto done
	// succ:  done
ProcedureBuilder_exit:
	use bp_16
	use CZS_7
	use dwLoc04_18
	use dwLoc0C_23
	use r1_13
	use r63_17
";
            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var cr = m.Frame.EnsureFlagGroup(flags, 0x3, "CZS", PrimitiveType.Byte);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, bp);
                m.Assign(bp, sp);
                m.Store(m.IAdd(bp, -8), m.Word32(0));
                m.Assign(cr, m.ISub(m.LoadW(m.IAdd(bp, 8)), 0x3));
                m.BranchIf(m.Test(ConditionCode.GE, cr), "ge3");

                m.Store(m.IAdd(bp, -8), r1);
                m.Goto("done");

                m.Label("ge3");
                m.Store(m.IAdd(bp, -8), r1);

                m.Label("done");
                m.Assign(r1, m.LoadDw(m.IAdd(bp, -8)));
                m.Assign(bp, m.LoadDw(sp));
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
        }

        [Test]
        public void SsaFlagRegisters()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def esi
	// succ:  l1
l1:
	SZ_1 = cond(esi & esi)
	C_2 = false
	al_3 = Test(ULE,C_2 | SZ_1)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest(sExp, m =>
            {
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var sz = m.Frame.EnsureFlagGroup(flags, 6, "SZ", PrimitiveType.Byte);
                var cz = m.Frame.EnsureFlagGroup(flags, 3, "CZ", PrimitiveType.Byte);
                var c = m.Frame.EnsureFlagGroup(flags, 1, "C", PrimitiveType.Bool);
                var al = m.Reg8("al", 0);
                var esi = m.Reg32("esi", 6);
                m.Assign(sz, m.Cond(m.And(esi, esi)));
                m.Assign(c, Constant.False());
                m.Assign(al, m.Test(ConditionCode.ULE, cz));
                m.Return();
            });
        }

        [Test]
        public void SsaHellNode()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r1
	def r2
	def r3
	// succ:  l1
l1:
	branch r1 true
	// succ:  l2 true
l2:
	r2_1 = 0x00000010
	// succ:  true
true:
	r2_4 = PHI(r2, r2_1)
	call r3 (retsize: 4;)
		uses: r1,r2_4,r3
		defs: r1_3,r2_6,r3_9
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use r1_3
	use r2_6
	use r3_9
";
            RunTest_FrameAccesses(sExp, m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var r3 = m.Register("r3");
                m.BranchIf(r1, "true");
                m.Assign(r2, m.Int32(16));
                m.Label("true");
                m.Call(r3, 4);
                m.Return();
            });
        }

        [Test(Description = "Self-recursive functions that pass parameters on stack should work.")]
        public void SsaHellNode_WithStackArgs()
        {
            var sExp =
            #region Expected
               @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def fp
	def Mem0
	def r3
	def dwArg04
	def dwLoc04
	// succ:  l1
l1:
	r63_1 = fp
	r4_3 = dwArg04
	branch r4_3 == 0x00000000 m1Base
	// succ:  m0Induction m1Base
m0Induction:
	r4_5 = Mem0[r4_3 + 0x00000004:word32]
	r63_6 = fp - 0x00000004
	dwLoc04_15 = r4_5
	call ProcedureBuilder (retsize: 0;)
		uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6
		defs: r3_10,r4_11,r63_8
	r4_12 = r4_11 + 0x00000001
	r63_13 = r63_8 + 0x00000004
	goto m2Done
	// succ:  m2Done
m1Base:
	r4_4 = 0x00000000
	// succ:  m2Done
m2Done:
	r63_20 = PHI(r63_13, r63_1)
	r4_19 = PHI(r4_12, r4_4)
	r3_18 = PHI(r3_10, r3)
	dwLoc04_16 = PHI(dwLoc04_15, dwLoc04)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use dwLoc04_16
	use r3_18
	use r4_19
	use r63_20
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r3 = m.Reg32("r3", 3);
                var r4 = m.Reg32("r4", 4);
                m.Assign(sp, m.Frame.FramePointer);   // Establish frame.
                m.Assign(r4, m.LoadDw(m.IAdd(sp, 4)));
                m.BranchIf(m.Eq0(r4), "m1Base");

                m.Label("m0Induction");
                m.Assign(r4, m.LoadDw(m.IAdd(r4, 4)));
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r4);
                m.Call(m.Procedure, 0);
                m.Assign(r4, m.IAdd(r4, 1));
                m.Assign(sp, m.IAdd(sp, 4));

                m.Goto("m2Done");

                m.Label("m1Base");
                m.Assign(r4, 0);

                m.Label("m2Done");
                m.Return();
            });
        }

        [Test]
        public void SsaSimple_DefUse()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	// succ:  l1
l1:
	a_0 = 0x00000003
	b_1 = a_0
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest(sExp, m =>
            {
                var regA = new RegisterStorage("a", 0, 0, PrimitiveType.Word32);
                var regB = new RegisterStorage("b", 1, 0, PrimitiveType.Word32);
                var a = m.Frame.EnsureRegister(regA);
                var b = m.Frame.EnsureRegister(regB);
                m.Assign(a, 3);
                m.Assign(b, a);
                m.Return();
            });
        }

        [Test]
        public void SsaUseUndefined()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def a
	// succ:  l1
l1:
	Mem1[0x00123400:word32] = a
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            this.addUseInstructions = true;
            RunTest(sExp, m =>
            {
                var a = m.Reg32("a", 0);
                m.Store(m.Word32(0x123400), a);
                m.Return();
            });
        }

        [Test]
        public void SsaIfThen()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def a
	def b
	// succ:  l1
l1:
	branch a == 0x00000000 m_2
	// succ:  m_1 m_2
m_1:
	b_1 = 0xFFFFFFFF
	// succ:  m_2
m_2:
	b_2 = PHI(b, b_1)
	return b_2
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest(sExp, m =>
            {
                var regA = new RegisterStorage("a", 0, 0, PrimitiveType.Word32);
                var regB = new RegisterStorage("b", 1, 0, PrimitiveType.Word32);
                var a = m.Frame.EnsureRegister(regA);
                var b = m.Frame.EnsureRegister(regB);
                m.BranchIf(m.Eq0(a), "m_2");
                m.Label("m_1");
                m.Assign(b, -1);
                m.Label("m_2");
                m.Return(b);
            });
        }


        [Test]
        public void SsaRegisterAlias()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def eax
	def Mem0
	// succ:  l1
l1:
	eax_2 = Mem0[eax:word32]
	ah_3 = SLICE(eax_2, byte, 8) (alias)
	Mem4[0x00001234:byte] = ah_3
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest(sExp, m =>
            {
                var regEax = new RegisterStorage("eax", 0, 0, PrimitiveType.Word32);
                var regAh = new RegisterStorage("ah", 0, 8, PrimitiveType.Byte);
                var eax = m.Frame.EnsureRegister(regEax);
                var ah = m.Frame.EnsureRegister(regAh);
                m.Assign(eax, m.LoadDw(eax));
                m.Store(m.Word32(0x1234), ah);
                m.Return();
            });
        }

        [Test]
        public void SsaAliasedPhi()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def Mem0
	// succ:  l1
l1:
	ecx_1 = Mem0[0x00542300:word32]
	branch Mem0[0x00010042:bool] mBranch2
	// succ:  mBranch1 mBranch2
mBranch1:
	cl_3 = 0x2A
	ecx_4 = DPB(ecx_1, cl_3, 0) (alias)
	goto mCommon
	// succ:  mCommon
mBranch2:
	ecx_2 = 0x00000020
	// succ:  mCommon
mCommon:
	ecx_5 = PHI(ecx_4, ecx_2)
	Mem6[0x00010232:word32] = ecx_5
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var ecx = m.Frame.EnsureRegister(new RegisterStorage("ecx", 1, 0, PrimitiveType.Word32));
                var cl = m.Frame.EnsureRegister(new RegisterStorage("cl", 1, 0, PrimitiveType.Byte));

                m.Assign(ecx, m.LoadDw(m.Word32(0x542300)));
                m.BranchIf(m.Load(PrimitiveType.Bool, m.Word32(0x10042)), "mBranch2");

                m.Label("mBranch1");
                m.Assign(cl, 42);
                m.Goto("mCommon");

                m.Label("mBranch2");
                m.Assign(ecx, 32);

                m.Label("mCommon");
                m.Store(m.Word32(0x10232), ecx);
                m.Return();
            });
        }

        [Test(Description = "Multiple assignments in the same block")]
        public void SsaManyAssignments()
        {
            var sExp =
            #region Expected@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def Mem0
	// succ:  l1
l1:
	eax_1 = Mem0[0x00543200:word32]
	edx_2 = Mem0[0x00543208:word32]
	eax_3 = eax_1 + edx_2
	Mem4[0x00642300:word32] = eax_3
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var edx = m.Frame.EnsureRegister(new RegisterStorage("edx", 2, 0, PrimitiveType.Word32));
                var eax = m.Frame.EnsureRegister(new RegisterStorage("eax", 0, 0, PrimitiveType.Word32));

                m.Assign(eax, m.LoadDw(m.Word32(0x543200)));
                m.Assign(edx, m.LoadDw(m.Word32(0x543208)));
                m.Assign(eax, m.IAdd(eax, edx));
                m.Store(m.Word32(0x642300), eax);
                m.Return();
            });
        }

        [Test(Description = "Multiple assignments in the same block with aliases")]
        public void SsaManyAssignmentsWithAliases()
        {
            var sExp =
            #region Expected@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def Mem0
	// succ:  l1
l1:
	edx_1 = Mem0[0x00543200:word32]
	dl_2 = (byte) edx_1 (alias)
	Mem3[0x00642300:byte] = dl_2
	edx_4 = Mem3[0x00543208:word32]
	dl_5 = (byte) edx_4 (alias)
	Mem6[0x00642308:byte] = dl_5
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var edx = m.Frame.EnsureRegister(new RegisterStorage("edx", 2, 0, PrimitiveType.Word32));
                var dl = m.Frame.EnsureRegister(new RegisterStorage("dl", 2, 0, PrimitiveType.Byte));

                m.Assign(edx, m.LoadDw(m.Word32(0x543200)));
                m.Store(m.Word32(0x642300), dl);
                m.Assign(edx, m.LoadDw(m.Word32(0x543208)));
                m.Store(m.Word32(0x642308), dl);
                m.Return();
            });
        }

        [Test]
        public void SsaOutArgs()
        {
            var sExp =
                #region Expected
@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def ebx
	// succ:  l1
l1:
	C_2 = os_service(ebx, out ebx_1)
	Mem3[0x00123400:word32] = ebx_1
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var ebx = m.Reg32("ebx", 2);
                var C = m.Flags("C");
                var func = new ExternalProcedure("os_service", new ProcedureSignature(C, ebx));

                m.Assign(C, m.Fn(func, ebx, m.Out(ebx.DataType, ebx)));
                m.Store(m.Word32(0x123400), ebx);
                m.Return();
            });
        }

        [Test]
        public void SsaSequence()
        {
            var sExp =
            #region Expected
                @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def es
	def bx
	def Mem0
	// succ:  l1
l1:
	es_bx_3 = Mem0[es:bx:word32]
	es_4 = SLICE(es_bx_3, word16, 16) (alias)
	bx_5 = (word16) es_bx_3 (alias)
	bx_6 = Mem0[es_4:bx_5 + 0x0010:word32]
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use bx_6
	use es_4
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var es = m.Reg16("es", 10);
                var bx = m.Reg16("bx", 3);
                var es_bx = m.Frame.EnsureSequence(es, bx, PrimitiveType.SegPtr32);

                m.Assign(es_bx, m.SegMem(PrimitiveType.Word32, es, bx));
                m.Assign(bx, m.SegMem(PrimitiveType.Word32, es, m.IAdd(bx,16)));
                m.Return();
            });
        }

        [Test(Description = "Two uses of an aliased register shoudn't crate two alias assignments")]
        public void SsaDoubleAlias()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def eax
	def Mem0
	// succ:  l1
l1:
	eax_2 = Mem0[eax:word32]
	al_3 = (byte) eax_2 (alias)
	Mem4[0x00123100:byte] = al_3
	Mem5[0x00123108:byte] = al_3
ProcedureBuilder_exit:
";          
                #endregion

            RunTest(sExp, m =>
            {
                var eax = m.Frame.EnsureRegister(new RegisterStorage("eax", 0, 0, PrimitiveType.Word32));
                var al = m.Frame.EnsureRegister(new RegisterStorage("al", 0, 0, PrimitiveType.Byte));

                m.Assign(eax, m.LoadDw(eax));
                m.Store(m.Word32(0x123100), al);            // store the low-order byte
                m.Store(m.Word32(0x123108), al);            // ...twice.
            });
        }

        [Test(Description = "Make sure SSA state behaves correctly in presence of loops.")]
        public void SsaWhileLoop()
        {
            var sExp =
            #region Expected
                @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def ebx
	def Mem0
	// succ:  l1
l1:
	eax_0 = 0x00000000
	goto l3Head
	// succ:  l3Head
l2Body:
	eax_5 = eax_3 + Mem4[ebx_1:word32]
	ebx_6 = Mem4[ebx_1 + 0x00000004:word32]
	// succ:  l3Head
l3Head:
	eax_3 = PHI(eax_0, eax_5)
	ebx_1 = PHI(ebx, ebx_6)
	SCZ_2 = cond(ebx_1 - 0x00000000)
	branch Test(NE,SCZ_2) l2Body
	// succ:  l4Exit l2Body
l4Exit:
	return eax_3
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var eax = m.Reg32("eax", 0);
                var ebx = m.Reg32("ebx", 3);
                var SCZ = m.Flags("SCZ");
                var Z = m.Flags("Z");

                m.Assign(eax, 0);
                m.Goto("l3Head");

                m.Label("l2Body");
                m.Assign(eax, m.IAdd(eax, m.LoadDw(ebx)));
                m.Assign(ebx, m.LoadDw(m.IAdd(ebx, 4)));

                m.Label("l3Head");
                m.Assign(SCZ, m.Cond(m.ISub(ebx, 0)));
                m.BranchIf(m.Test(ConditionCode.NE, Z), "l2Body");

                m.Label("l4Exit");
                m.Return(eax);      // forces liveness of eax.
            });
        }

        [Test]
        public void SsaAliasedSequence()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def Mem0
	// succ:  m0
m0:
	cx_1 = Mem0[0x1234:word16]
	Mem6[0x00001236:word16] = cx_1
	es_cx_3 = Mem2[0x00001238:word32]
	cl_4 = 0x2D
	es_cx_5 = DPB(es_cx_3, 0x2D, 0) (alias)
	cx_7 = (word16) es_cx_5 (alias)
	cx_8 = DPB(cx, cl_4, 0) (alias)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use cl_4
	use cx_8
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var es = m.Reg16("es", 2);
                var cx = m.Reg16("cx", 1);
                var cl = m.Reg8("cl", 1);
                var es_cx = m.Frame.EnsureSequence(es, cx, PrimitiveType.SegPtr32);

                m.Label("m0");
                m.Assign(cx, m.LoadW(m.Word16(0x1234)));
                m.Store(m.Word32(0x1236), cx);
                m.Assign(es_cx, m.LoadDw(m.Word32(0x1238)));
                m.Assign(cl, m.Byte(45));
                m.Return();
            });
        }

        [Test(Description = "A variable carried around uselessly in a loop.")]
        public void SsaLoopCarriedVariable()
        {
            var sExp =
            #region Expected
                @"@@@";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);

                m.Label("m0");
                m.BranchIf(m.Eq0(r1), "m3done");

                m.Label("m1notdone");
                m.Assign(r1, m.IAdd(r1, m.LoadDw(r2)));
                m.Goto("m0");

                m.Label("m3done");
                m.Return();
            });
        }
    }
}

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
using Rhino.Mocks;

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
        private Dictionary<Address, ImportReference> importReferences;
        private DataFlow2 programFlow2;
        private bool addUseInstructions;
        private IImportResolver importResolver;

        [SetUp]
        public void Setup()
        {
            this.pb = new ProgramBuilder();
            this.importReferences = new Dictionary<Address, ImportReference>();
            this.programFlow2 = new DataFlow2();
            this.addUseInstructions = false;
            this.importResolver = MockRepository.GenerateStub<IImportResolver>();
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder(this.pb.Program.Architecture);
            builder(pb);
            var proc = pb.Procedure;
            var dg = new DominatorGraph<Block>(proc.ControlGraph, proc.EntryBlock);
            var project = new Project
            {
                Programs = { this.pb.Program }
            };
            this.pb.Program.Platform = new FakePlatform(null, new FakeArchitecture());
            this.pb.Program.ImageMap = new ImageMap(
                Address.Ptr32(0x0000),
                new ImageSegment(
                    ".text",
                    Address.Ptr32(0), 
                    0x40000,
                    AccessMode.ReadWriteExecute));
            this.importResolver.Replay();

            var sst = new SsaTransform2(this.pb.Program.Architecture, proc, importResolver, programFlow2);
            sst.AddUseInstructions = addUseInstructions;
            sst.Transform();

            var writer = new StringWriter();
            sst.SsaState.Write(writer);
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
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();

            // Perform initial transformation.
            var sst = new SsaTransform2(this.pb.Program.Architecture, proc, importResolver, programFlow2);
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
            sst.SsaState.Write(writer);
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
            #region Expected
@"r1:r1
    def:  def r1
    uses: r1_2 = r1 + r2
r2:r2
    def:  def r2
    uses: r1_2 = r1 + r2
r1_2: orig: r1
    def:  r1_2 = r1 + r2
    uses: use r1_2
// ProcedureBuilder
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
            #endregion

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
            var sExp = 
@"fp:fp
    def:  def fp
    uses: r63_1 = fp
          r63_2 = fp - 0x00000004
          r63_5 = fp - 0x00000008
r63_1: orig: r63
    def:  r63_1 = fp
r63_2: orig: r63
    def:  r63_2 = fp - 0x00000004
r1:r1
    def:  def r1
    uses: dwLoc04_12 = r1
Mem4: orig: Mem0
    def:  dwLoc04_12 = r1
r63_5: orig: r63
    def:  r63_5 = fp - 0x00000008
    uses: use r63_5
r2:r2
    def:  def r2
    uses: dwLoc08_13 = r2
Mem7: orig: Mem0
    def:  dwLoc08_13 = r2
    uses: r1_8 = dwLoc04_12
          r2_9 = dwLoc08_13
r1_8: orig: r1
    def:  r1_8 = dwLoc04_12
    uses: r1_10 = r1_8 + r2_9
r2_9: orig: r2
    def:  r2_9 = dwLoc08_13
    uses: r1_10 = r1_8 + r2_9
          use r2_9
r1_10: orig: r1
    def:  r1_10 = r1_8 + r2_9
    uses: Mem14[0x00010008:word32] = r1_10
          use r1_10
Mem11: orig: Mem0
    def:  Mem14[0x00010008:word32] = r1_10
dwLoc04_12: orig: dwLoc04
    def:  dwLoc04_12 = r1
    uses: r1_8 = dwLoc04_12
          use dwLoc04_12
dwLoc08_13: orig: dwLoc08
    def:  dwLoc08_13 = r2
    uses: r2_9 = dwLoc08_13
          use dwLoc08_13
Mem14: orig: Mem11
    def:  Mem14[0x00010008:word32] = r1_10
// ProcedureBuilder
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
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_1 = fp
          r63_2 = fp - 0x00000004
          bp_5 = fp - 0x00000004
          r63_13 = fp
r63_1: orig: r63
    def:  r63_1 = fp
r63_2: orig: r63
    def:  r63_2 = fp - 0x00000004
bp:bp
    def:  def bp
    uses: dwLoc04_14 = bp
Mem4: orig: Mem0
    def:  dwLoc04_14 = bp
    uses: CZS_6 = cond(wArg04 - 0x0003)
          bp_11 = dwLoc04_14
bp_5: orig: bp
    def:  bp_5 = fp - 0x00000004
CZS_6: orig: CZS
    def:  CZS_6 = cond(wArg04 - 0x0003)
    uses: branch Test(GE,CZS_6) ge3
          use CZS_6
r1_7: orig: r1
    def:  r1_7 = 0x00000001
    uses: r1_19 = PHI(r1_8, r1_7)
r1_8: orig: r1
    def:  r1_8 = 0x00000000
    uses: r1_19 = PHI(r1_8, r1_7)
bp_11: orig: bp
    def:  bp_11 = dwLoc04_14
    uses: use bp_11
r63_13: orig: r63
    def:  r63_13 = fp
    uses: use r63_13
dwLoc04_14: orig: dwLoc04
    def:  dwLoc04_14 = bp
    uses: bp_11 = dwLoc04_14
          use dwLoc04_14
wArg04:Stack +0004
    def:  def wArg04
    uses: CZS_6 = cond(wArg04 - 0x0003)
r1_19: orig: r1
    def:  r1_19 = PHI(r1_8, r1_7)
    uses: use r1_19
// ProcedureBuilder
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
            #endregion

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
        public void SsaDiamondFrame()
        {
            var sExp = @"fp:fp
    def:  def fp
    uses: r63_1 = fp
          r63_2 = fp - 0x00000004
          bp_5 = fp - 0x00000004
          r63_14 = fp
r63_1: orig: r63
    def:  r63_1 = fp
r63_2: orig: r63
    def:  r63_2 = fp - 0x00000004
bp:bp
    def:  def bp
    uses: dwLoc04_15 = bp
Mem4: orig: Mem0
    def:  dwLoc04_15 = bp
    uses: CZS_6 = wArg04 - 0x0003
bp_5: orig: bp
    def:  bp_5 = fp - 0x00000004
CZS_6: orig: CZS
    def:  CZS_6 = wArg04 - 0x0003
    uses: branch Test(GE,CZS_6) ge3
          use CZS_6
Mem7: orig: Mem0
    def:  wArg04_17 = -3
    uses: Mem11 = PHI(Mem9, Mem7)
r1_8: orig: r1
    def:  r1_8 = 0x00000001
    uses: r1_22 = PHI(r1, r1_8)
Mem9: orig: Mem0
    def:  wArg04_18 = 0x0003
    uses: Mem11 = PHI(Mem9, Mem7)
Mem11: orig: Mem0
    def:  Mem11 = PHI(Mem9, Mem7)
    uses: bp_12 = dwLoc04_15
bp_12: orig: bp
    def:  bp_12 = dwLoc04_15
    uses: use bp_12
r63_14: orig: r63
    def:  r63_14 = fp
    uses: use r63_14
dwLoc04_15: orig: dwLoc04
    def:  dwLoc04_15 = bp
    uses: bp_12 = dwLoc04_15
          use dwLoc04_15
wArg04:Stack +0004
    def:  def wArg04
    uses: CZS_6 = wArg04 - 0x0003
wArg04_17: orig: wArg04
    def:  wArg04_17 = -3
    uses: wArg04_24 = PHI(wArg04_18, wArg04_17)
wArg04_18: orig: wArg04
    def:  wArg04_18 = 0x0003
    uses: wArg04_24 = PHI(wArg04_18, wArg04_17)
r1_22: orig: r1
    def:  r1_22 = PHI(r1, r1_8)
    uses: use r1_22
r1:r1
    def:  def r1
    uses: r1_22 = PHI(r1, r1_8)
wArg04_24: orig: wArg04
    def:  wArg04_24 = PHI(wArg04_18, wArg04_17)
    uses: use wArg04_24
// ProcedureBuilder
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

            var sExp =
            #region Expected
@"r1_0: orig: r1
    def:  r1_0 = 0x00000003
r2_1: orig: r2
    def:  r2_1 = 0x00000004
    uses: use r2_1
r1_2: orig: r1
    def:  call Adder (retsize: 4;)	defs: r1_2
    uses: Mem3[0x00012300:word32] = r1_2
          use r1_2
Mem3: orig: Mem0
    def:  Mem3[0x00012300:word32] = r1_2
// ProcedureBuilder
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
            #endregion

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
        public void SsaLocaldiamond()
        {
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_1 = fp
          r63_2 = fp - 0x00000004
          bp_5 = fp - 0x00000004
          r63_17 = fp
r63_1: orig: r63
    def:  r63_1 = fp
r63_2: orig: r63
    def:  r63_2 = fp - 0x00000004
bp:bp
    def:  def bp
    uses: dwLoc04_18 = bp
Mem4: orig: Mem0
    def:  dwLoc04_18 = bp
bp_5: orig: bp
    def:  bp_5 = fp - 0x00000004
Mem6: orig: Mem0
    def:  dwLoc0C_19 = 0x00000000
    uses: CZS_7 = wArg04 - 0x0003
CZS_7: orig: CZS
    def:  CZS_7 = wArg04 - 0x0003
    uses: branch Test(GE,CZS_7) ge3
          use CZS_7
r1:r1
    def:  def r1
    uses: dwLoc0C_21 = r1
          dwLoc0C_22 = r1
Mem9: orig: Mem0
    def:  dwLoc0C_21 = r1
    uses: Mem12 = PHI(Mem10, Mem9)
Mem10: orig: Mem0
    def:  dwLoc0C_22 = r1
    uses: Mem12 = PHI(Mem10, Mem9)
Mem12: orig: Mem0
    def:  Mem12 = PHI(Mem10, Mem9)
    uses: r1_13 = dwLoc0C_23
          bp_16 = dwLoc04_18
r1_13: orig: r1
    def:  r1_13 = dwLoc0C_23
    uses: use r1_13
bp_16: orig: bp
    def:  bp_16 = dwLoc04_18
    uses: use bp_16
r63_17: orig: r63
    def:  r63_17 = fp
    uses: use r63_17
dwLoc04_18: orig: dwLoc04
    def:  dwLoc04_18 = bp
    uses: bp_16 = dwLoc04_18
          use dwLoc04_18
dwLoc0C_19: orig: dwLoc0C
    def:  dwLoc0C_19 = 0x00000000
wArg04:Stack +0004
    def:  def wArg04
    uses: CZS_7 = wArg04 - 0x0003
dwLoc0C_21: orig: dwLoc0C
    def:  dwLoc0C_21 = r1
    uses: dwLoc0C_23 = PHI(dwLoc0C_22, dwLoc0C_21)
dwLoc0C_22: orig: dwLoc0C
    def:  dwLoc0C_22 = r1
    uses: dwLoc0C_23 = PHI(dwLoc0C_22, dwLoc0C_21)
dwLoc0C_23: orig: dwLoc0C
    def:  dwLoc0C_23 = PHI(dwLoc0C_22, dwLoc0C_21)
    uses: r1_13 = dwLoc0C_23
          use dwLoc0C_23
// ProcedureBuilder
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
            #endregion

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

            var sExp =
            #region Expected
@"esi:esi
    def:  def esi
    uses: SZ_1 = cond(esi & esi)
          SZ_1 = cond(esi & esi)
SZ_1: orig: SZ
    def:  SZ_1 = cond(esi & esi)
    uses: al_3 = Test(ULE,C_2 | SZ_1)
C_2: orig: C
    def:  C_2 = false
    uses: al_3 = Test(ULE,C_2 | SZ_1)
al_3: orig: al
    def:  al_3 = Test(ULE,C_2 | SZ_1)
// ProcedureBuilder
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
            #endregion

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
            var sExp =
            #region Expected
@"r1:r1
    def:  def r1
    uses: branch r1 true
          call r3 (retsize: 4;)	uses: r1,r2_6,r3	defs: r1_5,r2_8,r3_9
r2_1: orig: r2
    def:  r2_1 = 0x00000010
    uses: r2_6 = PHI(r2, r2_1)
r1_5: orig: r1
    def:  call r3 (retsize: 4;)	uses: r1,r2_6,r3	defs: r1_5,r2_8,r3_9
    uses: use r1_5
r3:r3
    def:  def r3
    uses: call r3 (retsize: 4;)	uses: r1,r2_6,r3	defs: r1_5,r2_8,r3_9
          call r3 (retsize: 4;)	uses: r1,r2_6,r3	defs: r1_5,r2_8,r3_9
r2_6: orig: r2
    def:  r2_6 = PHI(r2, r2_1)
    uses: call r3 (retsize: 4;)	uses: r1,r2_6,r3	defs: r1_5,r2_8,r3_9
r2:r2
    def:  def r2
    uses: r2_6 = PHI(r2, r2_1)
r2_8: orig: r2
    def:  call r3 (retsize: 4;)	uses: r1,r2_6,r3	defs: r1_5,r2_8,r3_9
    uses: use r2_8
r3_9: orig: r3
    def:  call r3 (retsize: 4;)	uses: r1,r2_6,r3	defs: r1_5,r2_8,r3_9
    uses: use r3_9
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r1
	def r3
	def r2
	// succ:  l1
l1:
	branch r1 true
	// succ:  l2 true
l2:
	r2_1 = 0x00000010
	// succ:  true
true:
	r2_6 = PHI(r2, r2_1)
	call r3 (retsize: 4;)
		uses: r1,r2_6,r3
		defs: r1_5,r2_8,r3_9
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use r1_5
	use r2_8
	use r3_9
";
            #endregion

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
@"fp:fp
    def:  def fp
    uses: r63_1 = fp
          r63_6 = fp - 0x00000004
r63_1: orig: r63
    def:  r63_1 = fp
    uses: r63_20 = PHI(r63_13, r63_1)
Mem0:Global memory
    def:  def Mem0
    uses: r4_3 = dwArg04
          r4_5 = Mem0[r4_3 + 0x00000004:word32]
r4_3: orig: r4
    def:  r4_3 = dwArg04
    uses: branch r4_3 == 0x00000000 m1Base
          r4_5 = Mem0[r4_3 + 0x00000004:word32]
r4_4: orig: r4
    def:  r4_4 = 0x00000000
    uses: r4_19 = PHI(r4_12, r4_4)
r4_5: orig: r4
    def:  r4_5 = Mem0[r4_3 + 0x00000004:word32]
    uses: dwLoc04_15 = r4_5
          call ProcedureBuilder (retsize: 0;)	uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6	defs: r3_10,r4_11,r63_8
r63_6: orig: r63
    def:  r63_6 = fp - 0x00000004
    uses: call ProcedureBuilder (retsize: 0;)	uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6	defs: r3_10,r4_11,r63_8
Mem7: orig: Mem0
    def:  dwLoc04_15 = r4_5
r63_8: orig: r63
    def:  call ProcedureBuilder (retsize: 0;)	uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6	defs: r3_10,r4_11,r63_8
    uses: r63_13 = r63_8 + 0x00000004
r3:r3
    def:  def r3
    uses: call ProcedureBuilder (retsize: 0;)	uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6	defs: r3_10,r4_11,r63_8
          r3_18 = PHI(r3_10, r3)
r3_10: orig: r3
    def:  call ProcedureBuilder (retsize: 0;)	uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6	defs: r3_10,r4_11,r63_8
    uses: r3_18 = PHI(r3_10, r3)
r4_11: orig: r4
    def:  call ProcedureBuilder (retsize: 0;)	uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6	defs: r3_10,r4_11,r63_8
    uses: r4_12 = r4_11 + 0x00000001
r4_12: orig: r4
    def:  r4_12 = r4_11 + 0x00000001
    uses: r4_19 = PHI(r4_12, r4_4)
r63_13: orig: r63
    def:  r63_13 = r63_8 + 0x00000004
    uses: r63_20 = PHI(r63_13, r63_1)
dwArg04:Stack +0004
    def:  def dwArg04
    uses: r4_3 = dwArg04
          call ProcedureBuilder (retsize: 0;)	uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6	defs: r3_10,r4_11,r63_8
dwLoc04_15: orig: dwLoc04
    def:  dwLoc04_15 = r4_5
    uses: call ProcedureBuilder (retsize: 0;)	uses: dwArg04,dwLoc04_15,r3,r4_5,r63_6	defs: r3_10,r4_11,r63_8
          dwLoc04_16 = PHI(dwLoc04_15, dwLoc04)
dwLoc04_16: orig: dwLoc04
    def:  dwLoc04_16 = PHI(dwLoc04_15, dwLoc04)
    uses: use dwLoc04_16
dwLoc04:Local -0004
    def:  def dwLoc04
    uses: dwLoc04_16 = PHI(dwLoc04_15, dwLoc04)
r3_18: orig: r3
    def:  r3_18 = PHI(r3_10, r3)
    uses: use r3_18
r4_19: orig: r4
    def:  r4_19 = PHI(r4_12, r4_4)
    uses: use r4_19
r63_20: orig: r63
    def:  r63_20 = PHI(r63_13, r63_1)
    uses: use r63_20
// ProcedureBuilder
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
            var sExp =
            #region Expected
@"a_0: orig: a
    def:  a_0 = 0x00000003
    uses: b_1 = a_0
b_1: orig: b
    def:  b_1 = a_0
// ProcedureBuilder
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
            #endregion

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
            var sExp =
            #region Expected
@"a:a
    def:  def a
    uses: Mem1[0x00123400:word32] = a
Mem1: orig: Mem0
    def:  Mem1[0x00123400:word32] = a
// ProcedureBuilder
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
            #endregion

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
            
            var sExp =
            #region Expected
                @"a:a
    def:  def a
    uses: branch a == 0x00000000 m_2
b_1: orig: b
    def:  b_1 = 0xFFFFFFFF
    uses: b_2 = PHI(b, b_1)
b_2: orig: b
    def:  b_2 = PHI(b, b_1)
    uses: return b_2
b:b
    def:  def b
    uses: b_2 = PHI(b, b_1)
// ProcedureBuilder
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
            #endregion

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
            var sExp =
            #region Expected
@"eax:eax
    def:  def eax
    uses: eax_2 = Mem0[eax:word32]
Mem0:Global memory
    def:  def Mem0
    uses: eax_2 = Mem0[eax:word32]
eax_2: orig: eax
    def:  eax_2 = Mem0[eax:word32]
    uses: ah_3 = SLICE(eax_2, byte, 8) (alias)
ah_3: orig: ah
    def:  ah_3 = SLICE(eax_2, byte, 8) (alias)
    uses: Mem4[0x00001234:byte] = ah_3
Mem4: orig: Mem0
    def:  Mem4[0x00001234:byte] = ah_3
// ProcedureBuilder
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
            #endregion

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
@"Mem0:Global memory
    def:  def Mem0
    uses: ecx_1 = Mem0[0x00542300:word32]
          branch Mem0[0x00010042:bool] mBranch2
ecx_1: orig: ecx
    def:  ecx_1 = Mem0[0x00542300:word32]
    uses: ecx_4 = DPB(ecx_1, cl_3, 0) (alias)
ecx_2: orig: ecx
    def:  ecx_2 = 0x00000020
    uses: ecx_5 = PHI(ecx_4, ecx_2)
cl_3: orig: cl
    def:  cl_3 = 0x2A
ecx_4: orig: ecx
    def:  ecx_4 = DPB(ecx_1, cl_3, 0) (alias)
    uses: ecx_5 = PHI(ecx_4, ecx_2)
ecx_5: orig: ecx
    def:  ecx_5 = PHI(ecx_4, ecx_2)
    uses: Mem6[0x00010232:word32] = ecx_5
Mem6: orig: Mem0
    def:  Mem6[0x00010232:word32] = ecx_5
// ProcedureBuilder
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
            #region Expected@"Mem0:Global memory
    def:  def Mem0
    uses: eax_1 = Mem0[0x00543200:word32]
          edx_2 = Mem0[0x00543208:word32]
eax_1: orig: eax
    def:  eax_1 = Mem0[0x00543200:word32]
    uses: eax_3 = eax_1 + edx_2
edx_2: orig: edx
    def:  edx_2 = Mem0[0x00543208:word32]
    uses: eax_3 = eax_1 + edx_2
eax_3: orig: eax
    def:  eax_3 = eax_1 + edx_2
    uses: Mem4[0x00642300:word32] = eax_3
Mem4: orig: Mem0
    def:  Mem4[0x00642300:word32] = eax_3
// ProcedureBuilder
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
            #region Expected@"Mem0:Global memory
    def:  def Mem0
    uses: edx_1 = Mem0[0x00543200:word32]
edx_1: orig: edx
    def:  edx_1 = Mem0[0x00543200:word32]
    uses: dl_2 = (byte) edx_1 (alias)
dl_2: orig: dl
    def:  dl_2 = (byte) edx_1 (alias)
    uses: Mem3[0x00642300:byte] = dl_2
Mem3: orig: Mem0
    def:  Mem3[0x00642300:byte] = dl_2
    uses: edx_4 = Mem3[0x00543208:word32]
edx_4: orig: edx
    def:  edx_4 = Mem3[0x00543208:word32]
    uses: dl_5 = (byte) edx_4 (alias)
dl_5: orig: dl
    def:  dl_5 = (byte) edx_4 (alias)
    uses: Mem6[0x00642308:byte] = dl_5
Mem6: orig: Mem0
    def:  Mem6[0x00642308:byte] = dl_5
// ProcedureBuilder
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
@"ebx:ebx
    def:  def ebx
    uses: C_2 = os_service(ebx, out ebx_1)
ebx_1: orig: ebx
    def:  C_2 = os_service(ebx, out ebx_1)
    uses: Mem3[0x00123400:word32] = ebx_1
C_2: orig: C
    def:  C_2 = os_service(ebx, out ebx_1)
Mem3: orig: Mem0
    def:  Mem3[0x00123400:word32] = ebx_1
// ProcedureBuilder
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
@"es:es
    def:  def es
    uses: es_bx_3 = Mem0[es:bx:word32]
bx:bx
    def:  def bx
    uses: es_bx_3 = Mem0[es:bx:word32]
Mem0:Global memory
    def:  def Mem0
    uses: es_bx_3 = Mem0[es:bx:word32]
          bx_6 = Mem0[es_4:bx_5 + 0x0010:word32]
es_bx_3: orig: es_bx
    def:  es_bx_3 = Mem0[es:bx:word32]
    uses: es_4 = SLICE(es_bx_3, word16, 16) (alias)
          bx_5 = (word16) es_bx_3 (alias)
es_4: orig: es
    def:  es_4 = SLICE(es_bx_3, word16, 16) (alias)
    uses: bx_6 = Mem0[es_4:bx_5 + 0x0010:word32]
          use es_4
bx_5: orig: bx
    def:  bx_5 = (word16) es_bx_3 (alias)
    uses: bx_6 = Mem0[es_4:bx_5 + 0x0010:word32]
bx_6: orig: bx
    def:  bx_6 = Mem0[es_4:bx_5 + 0x0010:word32]
    uses: use bx_6
// ProcedureBuilder
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
@"eax:eax
    def:  def eax
    uses: eax_2 = Mem0[eax:word32]
Mem0:Global memory
    def:  def Mem0
    uses: eax_2 = Mem0[eax:word32]
eax_2: orig: eax
    def:  eax_2 = Mem0[eax:word32]
    uses: al_3 = (byte) eax_2 (alias)
al_3: orig: al
    def:  al_3 = (byte) eax_2 (alias)
    uses: Mem4[0x00123100:byte] = al_3
          Mem5[0x00123108:byte] = al_3
Mem4: orig: Mem0
    def:  Mem4[0x00123100:byte] = al_3
Mem5: orig: Mem0
    def:  Mem5[0x00123108:byte] = al_3
// ProcedureBuilder
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

        private Identifier Reg(int n)
        {
            string name = string.Format("r{0}", n);
            return new Identifier(
                name,
                PrimitiveType.Word32,
                new RegisterStorage(name, n, 0, PrimitiveType.Word32));
        }

        [Test(Description ="Emulates calling an imported API Win32 on MIPS")]
        public void Ssa_ConstantPropagation()
        {
            // 0x00031234
            //this.importReferences
            var sExp =
@"r13_0: orig: r13
    def:  r13_0 = 0x00030000
r12_1: orig: r12
    def:  r12_1 = ImportedFunc
    uses: r14_2 = ImportedFunc(r6)
r14_2: orig: r14
    def:  r14_2 = ImportedFunc(r6)
r6:r6
    def:  def r6
    uses: r14_2 = ImportedFunc(r6)
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r6
	// succ:  l1
l1:
	r13_0 = 0x00030000
	r12_1 = ImportedFunc
	r14_2 = ImportedFunc(r6)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            var addr = Address.Ptr32(0x00031234);
            importReferences.Add(addr, new NamedImportReference(
                addr, "COREDLL.DLL", "fnFoo"));
            importResolver.Stub(i => i.ResolveToImportedProcedureConstant(
                Arg<Statement>.Is.Anything,
                Arg<Constant>.Matches(c => c.ToUInt32() == 0x00031234)))
                .Return(new ProcedureConstant(
                    PrimitiveType.Pointer32,
                    new ExternalProcedure(
                        "ImportedFunc",
                        new ProcedureSignature(
                            Reg(14), Reg(6)))));

            RunTest(sExp, m =>
            {
                var r13 = m.Reg32("r13", 13);
                var r12 = m.Reg32("r12", 12);
                m.Assign(r13, 0x00030000);
                m.Assign(r12, m.LoadDw(m.IAdd(r13, 0x1234)));
                m.Call(r12, 0);
                m.Return();
            });
        }

        [Test(Description = "Make sure SSA state behaves correctly in presence of loops.")]
        public void SsaWhileLoop()
        {
            var sExp =
            #region Expected
@"eax_0: orig: eax
    def:  eax_0 = 0x00000000
    uses: eax_3 = PHI(eax_0, eax_5)
ebx_1: orig: ebx
    def:  ebx_1 = PHI(ebx, ebx_6)
    uses: SCZ_2 = cond(ebx_1 - 0x00000000)
          eax_5 = eax_3 + Mem0[ebx_1:word32]
          ebx_6 = Mem0[ebx_1 + 0x00000004:word32]
SCZ_2: orig: SCZ
    def:  SCZ_2 = cond(ebx_1 - 0x00000000)
    uses: branch Test(NE,SCZ_2) l2Body
eax_3: orig: eax
    def:  eax_3 = PHI(eax_0, eax_5)
    uses: eax_5 = eax_3 + Mem0[ebx_1:word32]
          return eax_3
eax_5: orig: eax
    def:  eax_5 = eax_3 + Mem0[ebx_1:word32]
    uses: eax_3 = PHI(eax_0, eax_5)
ebx_6: orig: ebx
    def:  ebx_6 = Mem0[ebx_1 + 0x00000004:word32]
    uses: ebx_1 = PHI(ebx, ebx_6)
ebx:ebx
    def:  def ebx
    uses: ebx_1 = PHI(ebx, ebx_6)
Mem0:Global memory
    def:  def Mem0
// ProcedureBuilder
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
	eax_5 = eax_3 + Mem0[ebx_1:word32]
	ebx_6 = Mem0[ebx_1 + 0x00000004:word32]
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
@"Mem0:Global memory
    def:  def Mem0
    uses: cx_1 = Mem0[0x1234:word16]
cx_1: orig: cx
    def:  cx_1 = Mem0[0x1234:word16]
    uses: Mem2[0x00001236:word16] = cx_1
Mem2: orig: Mem0
    def:  Mem2[0x00001236:word16] = cx_1
    uses: es_cx_3 = Mem2[0x00001238:word32]
es_cx_3: orig: es_cx
    def:  es_cx_3 = Mem2[0x00001238:word32]
    uses: es_cx_5 = DPB(es_cx_3, 0x2D, 0) (alias)
cl_4: orig: cl
    def:  cl_4 = 0x2D
    uses: use cl_4
es_cx_5: orig: es_cx
    def:  es_cx_5 = DPB(es_cx_3, 0x2D, 0) (alias)
    uses: cx_7 = (word16) es_cx_5 (alias)
cx_7: orig: cx
    def:  cx_7 = (word16) es_cx_5 (alias)
    uses: cx_8 = DPB(cx_7, cl_4, 0) (alias)
cx_8: orig: cx
    def:  cx_8 = DPB(cx_7, cl_4, 0) (alias)
    uses: use cx_8
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def Mem0
	// succ:  m0
m0:
	cx_1 = Mem0[0x1234:word16]
	Mem2[0x00001236:word16] = cx_1
	es_cx_3 = Mem2[0x00001238:word32]
	cl_4 = 0x2D
	es_cx_5 = DPB(es_cx_3, 0x2D, 0) (alias)
	cx_7 = (word16) es_cx_5 (alias)
	cx_8 = DPB(cx_7, cl_4, 0) (alias)
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
@"r1_0: orig: r1
    def:  r1_0 = PHI(r1, r1_3)
    uses: branch r1_0 == 0x00000000 m3done
          r1_3 = r1_0 + Mem0[r2:word32]
Mem0:Global memory
    def:  def Mem0
r1_3: orig: r1
    def:  r1_3 = r1_0 + Mem0[r2:word32]
    uses: r1_0 = PHI(r1, r1_3)
          use r1_3
r1:r1
    def:  def r1
    uses: r1_0 = PHI(r1, r1_3)
r2:r2
    def:  def r2
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r1
	def r2
	def Mem0
	// succ:  m0
m0:
	r1_0 = PHI(r1, r1_3)
	branch r1_0 == 0x00000000 m3done
	// succ:  m1notdone m3done
m1notdone:
	r1_3 = r1_0 + Mem0[r2:word32]
	goto m0
	// succ:  m0
m3done:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use r1_3
";
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

        [Test()]
        public void SsaAlias2()
        {
            var sExp =
@"si:si
    def:  def si
    uses: bl_2 = Mem0[si:byte]
Mem0:Global memory
    def:  def Mem0
    uses: bl_2 = Mem0[si:byte]
bl_2: orig: bl
    def:  bl_2 = Mem0[si:byte]
    uses: SCZO_3 = cond(bl_2 - 0x02)
SCZO_3: orig: SCZO
    def:  SCZO_3 = cond(bl_2 - 0x02)
    uses: branch Test(UGT,SCZO_3) m2
bh_4: orig: bh
    def:  bh_4 = 0x00
bx:bx
    def:  def bx
    uses: bx_6 = DPB(bx, bl_2, 0) (alias)
bx_6: orig: bx
    def:  bx_6 = DPB(bx, bl_2, 0) (alias)
    uses: bx_7 = DPB(bx_6, bh_4, 8) (alias)
bx_7: orig: bx
    def:  bx_7 = DPB(bx_6, bh_4, 8) (alias)
    uses: bx_8 = bx_7 + bx_7
          bx_8 = bx_7 + bx_7
bx_8: orig: bx
    def:  bx_8 = bx_7 + bx_7
    uses: Mem9[bx_8:word16] = 0x0000
Mem9: orig: Mem0
    def:  Mem9[bx_8:word16] = 0x0000
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def si
	def Mem0
	def bx
	// succ:  m0
m0:
	bl_2 = Mem0[si:byte]
	bx_6 = DPB(bx, bl_2, 0) (alias)
	SCZO_3 = cond(bl_2 - 0x02)
	branch Test(UGT,SCZO_3) m2
	// succ:  m1 m2
m1:
	bh_4 = 0x00
	bx_7 = DPB(bx_6, bh_4, 8) (alias)
	bx_8 = bx_7 + bx_7
	Mem9[bx_8:word16] = 0x0000
	// succ:  m2
m2:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest(sExp, m =>
            {
                var bl = m.Reg8("bl", 3, 0);
                var bh = m.Reg8("bh", 3, 8);
                var bx = m.Reg16("bx", 3);
                var si = m.Reg16("si", 6);
                var SCZO = m.Flags("SCZO");

                m.Label("m0");
                m.Assign(bl, m.LoadB(si));
                m.Assign(SCZO, m.Cond(m.ISub(bl, 2)));
                m.BranchIf(m.Test(ConditionCode.UGT, SCZO), "m2");

                m.Label("m1");
                m.Assign(bh, 0);
                m.Assign(bx, m.IAdd(bx, bx));
                m.Store(bx, m.Word16(0));

                m.Label("m2");
                m.Return();
            });


        }
    }
}

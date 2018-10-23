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
using Reko.Core.Code;

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
        private ProgramDataFlow programFlow;
        private bool addUseInstructions;
        private IImportResolver importResolver;
        private SsaTransform sst;

        private Identifier r1;
        private Identifier r2;
        private Identifier r3;
        private Identifier r4;

        [SetUp]
        public void Setup()
        {
            this.pb = new ProgramBuilder();
            this.importReferences = pb.Program.ImportReferences;
            this.addUseInstructions = false;
            this.importResolver = MockRepository.GenerateStub<IImportResolver>();
            this.r1 = new Identifier("r1", PrimitiveType.Word32, new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
            this.r2 = new Identifier("r2", PrimitiveType.Word32, new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));
            this.r3 = new Identifier("r3", PrimitiveType.Word32, new RegisterStorage("r3", 3, 0, PrimitiveType.Word32));
            this.r4 = new Identifier("r4", PrimitiveType.Word32, new RegisterStorage("r4", 4, 0, PrimitiveType.Word32));
            this.programFlow = new ProgramDataFlow();
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var proc = pb.Add("proc1", builder);
            RunTest(sExp);
        }

        private void RunTest(string sExp)
        {
            var program = pb.Program;
            var project = new Project
            {
                Programs = { program }
            };
            var listener = new FakeDecompilerEventListener();
            var arch = new FakeArchitecture();
            var platform = new FakePlatform(null, arch);

            // Register r1 is assumed to always be implicit when calling
            // another procedure.
            var implicitRegs = new HashSet<RegisterStorage>
            {
                arch.GetRegister(1)
            };
            program.Platform = platform;
            program.SegmentMap = new SegmentMap(
                Address.Ptr32(0x0000),
                new ImageSegment(
                    ".text",
                    Address.Ptr32(0),
                    0x40000,
                    AccessMode.ReadWriteExecute));
            this.importResolver.Replay();

            var writer = new StringWriter();
            foreach (var proc in this.pb.Program.Procedures.Values)
            {
                var sst = new SsaTransform(
                    this.pb.Program,
                    proc, 
                    new HashSet<Procedure>(),
                    importResolver,
                    programFlow);
                var ssa = sst.Transform();
                if (this.addUseInstructions)
                {
                    sst.AddUsesToExitBlock();
                    sst.RemoveDeadSsaIdentifiers();
                }
                sst.SsaState.Write(writer);
                proc.Write(false, writer);
                writer.WriteLine("======");
                ssa.Validate(s => { ssa.Dump(true); Assert.Fail(s); });
            }
            var sActual = writer.ToString();
            if (sActual != sExp)
                Debug.Print(sActual);
            Assert.AreEqual(sExp, sActual);
        }

        private void RunTest_FrameAccesses(string sExp, Action<ProcedureBuilder> builder)
        {
            pb.Add("proc1", builder);
            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();

            var program = this.pb.Program;
            RunTest_FrameAccesses(sExp);
        }

        private void RunTest_FrameAccesses(string sExp)
        {
            var listener = new FakeDecompilerEventListener();
            var program = pb.BuildProgram();
            var platform = new FakePlatform(null, program.Architecture)
            {
                Test_CreateTrashedRegisters = () =>
                    new HashSet<RegisterStorage>()
                {
                    (RegisterStorage)r1.Storage,
                    (RegisterStorage)r2.Storage,
                    (RegisterStorage)r3.Storage,
                    (RegisterStorage)r4.Storage,
                    program.Architecture.StackRegister,
                }
            };
            program.Platform = platform;
            var writer = new StringWriter();
            foreach (var proc in program.Procedures.Values)
            {
                // Perform initial transformation.
                var sst = new SsaTransform(
                    this.pb.Program,
                    proc, 
                    new HashSet<Procedure>(),
                    importResolver,
                    programFlow);
                sst.Transform();

                // Propagate values and simplify the results.
                // We hope the the sequence
                //   esp = fp - 4
                //   mov [esp-4],eax
                // will become
                //   esp_2 = fp - 4
                //   mov [fp - 8],eax

            var vp = new ValuePropagator(this.pb.Program.SegmentMap, sst.SsaState, importResolver, listener);
                vp.Transform();

                sst.RenameFrameAccesses = true;
                sst.Transform();
                sst.AddUsesToExitBlock();
                sst.RemoveDeadSsaIdentifiers();

                sst.SsaState.Write(writer);
                proc.Write(false, writer);
                writer.WriteLine("======");
            }
            var sActual = writer.ToString();
            if (sActual != sExp)
            {
                Debug.Print("<< Expected ========");
                Debug.Print(sExp);
                Debug.Print(">> Actual ==========");
                Debug.Print(sActual);
            }
            Assert.AreEqual(sExp, sActual);
        }

        private Procedure Given_Procedure(string name, Action<ProcedureBuilder> builder)
        {
            return pb.Add(name, builder);
        }

        private RegisterStorage Given_FpuStackRegister(string name)
        {
            var top = new RegisterStorage(name, 123, 0, PrimitiveType.Byte);
            var arch = (FakeArchitecture)pb.Program.Architecture;
            arch.FpuStackRegister = top;
            return arch.FpuStackRegister;
        }

        private MemoryIdentifier Given_FpuStackBase(string name)
        {
            var stg = new MemoryStorage("fpu", StorageDomain.Register + 456);
            var ST = new MemoryIdentifier(name, PrimitiveType.Ptr32, stg);
            var arch = (FakeArchitecture)pb.Program.Architecture;
            arch.FpuStackBase = ST;
            return arch.FpuStackBase;
        }

        private void Given_ProcedureFlow(string name, Func<Procedure, ProcedureFlow> flowBuilder)
        {
            var proc = Given_Procedure(name, m =>
            {
            });
            this.programFlow.ProcedureFlows[proc] = flowBuilder(proc);
        }

        private void When_RunSsaTransform()
        {
            var program = pb.BuildProgram();
            var proc = program.Procedures.Values.First();
            this.sst = new SsaTransform(
                program,
                proc,
                new HashSet<Procedure>(),
                importResolver,
                programFlow);
            sst.Transform();
            sst.SsaState.Validate(s => Assert.Fail(s));
        }

        private void When_RenameFrameAccesses()
        {
            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.SsaState.Validate(s => Assert.Fail(s));
        }

        private void AssertProcedureCode(string expected)
        {
            var proc = this.pb.Program.Procedures.Values.First();
            var writer = new StringWriter();
            proc.WriteBody(false, writer);
            var actual = writer.ToString();
            if (actual != expected)
            {
                Debug.Print(actual);
            }
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SsaSimple()
        {
            var sExp =
            #region Expected
@"r1:r1
    def:  def r1
    uses: r1_3 = r1 + r2
r2:r2
    def:  def r2
    uses: r1_3 = r1 + r2
r1_3: orig: r1
    def:  r1_3 = r1 + r2
    uses: use r1_3
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	def r2
	// succ:  l1
l1:
	r1_3 = r1 + r2
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_3
======
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
    uses: r63_2 = fp
          r63_3 = fp - 0x00000004
          r63_6 = fp - 0x00000008
r63_2: orig: r63
    def:  r63_2 = fp
r63_3: orig: r63
    def:  r63_3 = fp - 0x00000004
r1:r1
    def:  def r1
    uses: dwLoc04_13 = r1
Mem5: orig: Mem0
r63_6: orig: r63
    def:  r63_6 = fp - 0x00000008
    uses: use r63_6
r2:r2
    def:  def r2
    uses: dwLoc08_14 = r2
Mem8: orig: Mem0
r1_9: orig: r1
    def:  r1_9 = dwLoc04_13
    uses: r1_11 = r1_9 + r2_10
r2_10: orig: r2
    def:  r2_10 = dwLoc08_14
    uses: r1_11 = r1_9 + r2_10
          use r2_10
r1_11: orig: r1
    def:  r1_11 = r1_9 + r2_10
    uses: Mem12[0x00010008:word32] = r1_11
          use r1_11
Mem12: orig: Mem0
    def:  Mem12[0x00010008:word32] = r1_11
dwLoc04_13: orig: dwLoc04
    def:  dwLoc04_13 = r1
    uses: r1_9 = dwLoc04_13
dwLoc08_14: orig: dwLoc08
    def:  dwLoc08_14 = r2
    uses: r2_10 = dwLoc08_14
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def r1
	def r2
	// succ:  l1
l1:
	r63_2 = fp
	r63_3 = fp - 0x00000004
	dwLoc04_13 = r1
	r63_6 = fp - 0x00000008
	dwLoc08_14 = r2
	r1_9 = dwLoc04_13
	r2_10 = dwLoc08_14
	r1_11 = r1_9 + r2_10
	Mem12[0x00010008:word32] = r1_11
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_11
	use r2_10
	use r63_6
======
";
            addUseInstructions = true;
            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r1);
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r2);
                m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
                m.Assign(r2, m.Mem32(sp));
                m.Assign(r1, m.IAdd(r1, r2));
                m.MStore(m.Word32(0x010008), r1);
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
    uses: r63_2 = fp
          r63_3 = fp - 0x00000004
          bp_6 = fp - 0x00000004
          r63_13 = fp
r63_2: orig: r63
    def:  r63_2 = fp
r63_3: orig: r63
    def:  r63_3 = fp - 0x00000004
bp:bp
    def:  def bp
    uses: dwLoc04_14 = bp
Mem5: orig: Mem0
bp_6: orig: bp
    def:  bp_6 = fp - 0x00000004
SZC_7: orig: SZC
    def:  SZC_7 = cond(wArg04 - 0x0003)
    uses: branch Test(GE,SZC_7) ge3
          C_18 = SLICE(SZC_7, bool, 2) (alias)
          S_21 = SLICE(SZC_7, bool, 0) (alias)
          Z_23 = SLICE(SZC_7, bool, 1) (alias)
r1_8: orig: r1
    def:  r1_8 = 0x00000001
    uses: r1_19 = PHI(r1_9, r1_8)
r1_9: orig: r1
    def:  r1_9 = 0x00000000
    uses: r1_19 = PHI(r1_9, r1_8)
wArg04:Stack +0004
    def:  def wArg04
    uses: SZC_7 = cond(wArg04 - 0x0003)
dwLoc04_14: orig: dwLoc04
    def:  dwLoc04_14 = bp
    uses: bp_12 = dwLoc04_14
bp_12: orig: bp
    def:  bp_12 = dwLoc04_14
    uses: use bp_12
r63_13: orig: r63
    def:  r63_13 = fp
    uses: use r63_13
C_18: orig: C
    def:  C_18 = SLICE(SZC_7, bool, 2) (alias)
    uses: use C_18
r1_19: orig: r1
    def:  r1_19 = PHI(r1_9, r1_8)
    uses: use r1_19
S_21: orig: S
    def:  S_21 = SLICE(SZC_7, bool, 0) (alias)
    uses: use S_21
Z_23: orig: Z
    def:  Z_23 = SLICE(SZC_7, bool, 1) (alias)
    uses: use Z_23
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def bp
	def wArg04
	goto l1
	// succ:  l1
done:
	r1_19 = PHI(r1_9, r1_8)
	bp_12 = dwLoc04_14
	r63_13 = fp
	return
	// succ:  proc1_exit
ge3:
	r1_8 = 0x00000001
	goto done
	// succ:  done
l1:
	r63_2 = fp
	r63_3 = fp - 0x00000004
	dwLoc04_14 = bp
	bp_6 = fp - 0x00000004
	SZC_7 = cond(wArg04 - 0x0003)
	C_18 = SLICE(SZC_7, bool, 2) (alias)
	S_21 = SLICE(SZC_7, bool, 0) (alias)
	Z_23 = SLICE(SZC_7, bool, 1) (alias)
	branch Test(GE,SZC_7) ge3
	// succ:  l2 ge3
l2:
	r1_9 = 0x00000000
	goto done
	// succ:  done
proc1_exit:
	use bp_12
	use C_18
	use r1_19
	use r63_13
	use S_21
	use Z_23
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Reg32("bp", 5);
                var r1 = m.Reg32("r1", 1);
                var sz = m.Architecture.GetFlagGroup("SZC");
                var cr = m.Frame.EnsureFlagGroup(sz);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, bp);
                m.Assign(bp, sp);
                m.Assign(cr, m.Cond(m.ISub(m.Mem16(m.IAdd(bp, 8)), 0x3)));
                m.BranchIf(m.Test(ConditionCode.GE, cr), "ge3");

                m.Assign(r1, 0);
                m.Goto("done");

                m.Label("ge3");
                m.Assign(r1, 1);

                m.Label("done");
                m.Assign(bp, m.Mem32(sp));
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
        }

        [Test]
        public void SsaDiamondFrame()
        {
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_2 = fp
          r63_3 = fp - 0x00000004
          bp_6 = fp - 0x00000004
          r63_14 = fp
r63_2: orig: r63
    def:  r63_2 = fp
r63_3: orig: r63
    def:  r63_3 = fp - 0x00000004
bp:bp
    def:  def bp
    uses: dwLoc04_15 = bp
Mem5: orig: Mem0
bp_6: orig: bp
    def:  bp_6 = fp - 0x00000004
SZC_7: orig: SZC
    def:  SZC_7 = wArg04 - 0x0003
    uses: branch Test(GE,SZC_7) ge3
          C_21 = SLICE(SZC_7, bool, 2) (alias)
          S_25 = SLICE(SZC_7, bool, 0) (alias)
          Z_27 = SLICE(SZC_7, bool, 1) (alias)
Mem8: orig: Mem0
    uses: Mem12 = PHI(Mem10, Mem8)
r1_9: orig: r1
    def:  r1_9 = 0x00000001
    uses: r1_22 = PHI(r1, r1_9)
Mem10: orig: Mem0
    uses: Mem12 = PHI(Mem10, Mem8)
dwLoc04_15: orig: dwLoc04
    def:  dwLoc04_15 = bp
    uses: bp_13 = dwLoc04_15
Mem12: orig: Mem0
    def:  Mem12 = PHI(Mem10, Mem8)
bp_13: orig: bp
    def:  bp_13 = dwLoc04_15
    uses: use bp_13
r63_14: orig: r63
    def:  r63_14 = fp
    uses: use r63_14
wArg04:Stack +0004
    def:  def wArg04
    uses: SZC_7 = wArg04 - 0x0003
wArg04_17: orig: wArg04
    def:  wArg04_17 = -3
wArg04_18: orig: wArg04
    def:  wArg04_18 = 0x0003
C_21: orig: C
    def:  C_21 = SLICE(SZC_7, bool, 2) (alias)
    uses: use C_21
r1_22: orig: r1
    def:  r1_22 = PHI(r1, r1_9)
    uses: use r1_22
r1:r1
    def:  def r1
    uses: r1_22 = PHI(r1, r1_9)
S_25: orig: S
    def:  S_25 = SLICE(SZC_7, bool, 0) (alias)
    uses: use S_25
Z_27: orig: Z
    def:  Z_27 = SLICE(SZC_7, bool, 1) (alias)
    uses: use Z_27
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def bp
	def wArg04
	def r1
	goto l1
	// succ:  l1
done:
	r1_22 = PHI(r1, r1_9)
	Mem12 = PHI(Mem10, Mem8)
	bp_13 = dwLoc04_15
	r63_14 = fp
	return
	// succ:  proc1_exit
ge3:
	wArg04_17 = -3
	r1_9 = 0x00000001
	goto done
	// succ:  done
l1:
	r63_2 = fp
	r63_3 = fp - 0x00000004
	dwLoc04_15 = bp
	bp_6 = fp - 0x00000004
	SZC_7 = wArg04 - 0x0003
	C_21 = SLICE(SZC_7, bool, 2) (alias)
	S_25 = SLICE(SZC_7, bool, 0) (alias)
	Z_27 = SLICE(SZC_7, bool, 1) (alias)
	branch Test(GE,SZC_7) ge3
	// succ:  l2 ge3
l2:
	wArg04_18 = 0x0003
	goto done
	// succ:  done
proc1_exit:
	use bp_13
	use C_21
	use r1_22
	use r63_14
	use S_25
	use Z_27
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Reg32("bp", 5);
                var r1 = m.Reg32("r1", 1);
                var flags = m.Architecture.GetFlagGroup("SZC");
                var cr = m.Frame.EnsureFlagGroup(flags);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, bp);
                m.Assign(bp, sp);
                m.Assign(cr, m.ISub(m.Mem16(m.IAdd(bp, 8)), 0x3));
                m.BranchIf(m.Test(ConditionCode.GE, cr), "ge3");

                m.MStore(m.IAdd(bp, 8), m.Word16(3));
                m.Goto("done");

                m.Label("ge3");
                m.MStore(m.IAdd(bp, 8), Constant.Int16(-3));
                m.Assign(r1, 1);

                m.Label("done");
                m.Assign(bp, m.Mem32(sp));
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
        }

        [Test]
        public void SsaSubroutine()
        {
            var sExp =
            #region Expected
@"// Adder
// Return size: 0
define Adder
Adder_entry:
Adder_exit:
======
r1_1: orig: r1
    def:  r1_1 = 0x00000003
r2_2: orig: r2
    def:  r2_2 = 0x00000004
    uses: use r2_2
r1_3: orig: r1
    def:  call Adder (retsize: 4;)	defs: r1:r1_3
    uses: Mem4[0x00012300:word32] = r1_3
          use r1_3
Mem4: orig: Mem0
    def:  Mem4[0x00012300:word32] = r1_3
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	r1_1 = 0x00000003
	r2_2 = 0x00000004
	call Adder (retsize: 4;)
		defs: r1:r1_3
	Mem4[0x00012300:word32] = r1_3
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_3
	use r2_2
======
";
            #endregion

            addUseInstructions = true;
            RunTest(sExp, m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);

                // Simulate the creation of a subroutine.
                var procSub = this.pb.Add("Adder", mm => { });
                var procSubFlow = new ProcedureFlow(m.Procedure) { Trashed = { r1.Storage } };
                programFlow.ProcedureFlows.Add(procSub, procSubFlow);

                m.Assign(r1, 3);
                m.Assign(r2, 4);
                m.Call(procSub, 4);
                m.MStore(m.Word32(0x012300), r1);
                m.Return();
            });
        }

        [Test]
        public void SsaCallSubroutineWithStackParameters()
        {
            var sExp =
            #region Expected
@"// Adder
// Return size: 0
define Adder
Adder_entry:
Adder_exit:
======
r63:r63
    def:  def r63
    uses: r63_2 = r63 - 0x00000004
r63_2: orig: r63
    def:  r63_2 = r63 - 0x00000004
    uses: Mem3[r63_2:word32] = 0x0000002A
          call Adder (retsize: 4;)	uses: r1:r1_4,Stack +0004:Mem3[r63_2:word32]	defs: r1:r1_5
Mem3: orig: Mem0
    def:  Mem3[r63_2:word32] = 0x0000002A
    uses: call Adder (retsize: 4;)	uses: r1:r1_4,Stack +0004:Mem3[r63_2:word32]	defs: r1:r1_5
r1_4: orig: r1
    def:  r1_4 = 0x00000018
    uses: call Adder (retsize: 4;)	uses: r1:r1_4,Stack +0004:Mem3[r63_2:word32]	defs: r1:r1_5
r1_5: orig: r1
    def:  call Adder (retsize: 4;)	uses: r1:r1_4,Stack +0004:Mem3[r63_2:word32]	defs: r1:r1_5
    uses: Mem6[0x00012300:word32] = r1_5
Mem6: orig: Mem0
    def:  Mem6[0x00012300:word32] = r1_5
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r63
	// succ:  l1
l1:
	r63_2 = r63 - 0x00000004
	Mem3[r63_2:word32] = 0x0000002A
	r1_4 = 0x00000018
	call Adder (retsize: 4;)
		uses: r1:r1_4,Stack +0004:Mem3[r63_2:word32]
		defs: r1:r1_5
	Mem6[0x00012300:word32] = r1_5
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1 = m.Register(1);
                var r2 = m.Register(2);
                var sp = m.Frame.EnsureRegister(m.Procedure.Architecture.StackRegister);

                // Simulate the creation of a subroutine.
                var procSub = this.pb.Add("Adder", mm => { });
                var procSubFlow = new ProcedureFlow(m.Procedure) {
                    BitsUsed = {
                        { new StackArgumentStorage(4, PrimitiveType.Word32), new BitRange(0,32) },
                        { r1.Storage, new BitRange(0, 32) }
                    },
                    Trashed = { r1.Storage } };
                programFlow.ProcedureFlows.Add(procSub, procSubFlow);

                m.Assign(sp, m.ISub(sp, 4));    // push an argument on the stack...
                m.MStore(sp, m.Word32(42));
                m.Assign(r1, m.Word32(24));     // ..and one in a register.
                m.Call(procSub, 4);
                m.MStore(m.Word32(0x012300), r1);
                m.Return();
            });

        }

        [Test]
        public void SsaLocalDiamond()
        {
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_2 = fp
          r63_3 = fp - 0x00000004
          bp_6 = fp - 0x00000004
          r63_17 = fp
r63_2: orig: r63
    def:  r63_2 = fp
r63_3: orig: r63
    def:  r63_3 = fp - 0x00000004
bp:bp
    def:  def bp
    uses: dwLoc04_18 = bp
Mem5: orig: Mem0
bp_6: orig: bp
    def:  bp_6 = fp - 0x00000004
Mem7: orig: Mem0
CZS_8: orig: CZS
    def:  CZS_8 = wArg04 - 0x0003
    uses: branch Test(GE,CZS_8) ge3
          C_26 = SLICE(CZS_8, bool, 2) (alias)
          S_28 = SLICE(CZS_8, bool, 0) (alias)
          Z_30 = SLICE(CZS_8, bool, 1) (alias)
r1:r1
    def:  def r1
    uses: dwLoc0C_21 = r1
          dwLoc0C_22 = r1
Mem10: orig: Mem0
    uses: Mem13 = PHI(Mem11, Mem10)
Mem11: orig: Mem0
    uses: Mem13 = PHI(Mem11, Mem10)
dwLoc0C_19: orig: dwLoc0C
    def:  dwLoc0C_19 = 0x00000000
Mem13: orig: Mem0
    def:  Mem13 = PHI(Mem11, Mem10)
r1_14: orig: r1
    def:  r1_14 = dwLoc0C_23
    uses: use r1_14
dwLoc04_18: orig: dwLoc04
    def:  dwLoc04_18 = bp
    uses: bp_16 = dwLoc04_18
bp_16: orig: bp
    def:  bp_16 = dwLoc04_18
    uses: use bp_16
r63_17: orig: r63
    def:  r63_17 = fp
    uses: use r63_17
wArg04:Stack +0004
    def:  def wArg04
    uses: CZS_8 = wArg04 - 0x0003
dwLoc0C_21: orig: dwLoc0C
    def:  dwLoc0C_21 = r1
    uses: dwLoc0C_23 = PHI(dwLoc0C_22, dwLoc0C_21)
dwLoc0C_22: orig: dwLoc0C
    def:  dwLoc0C_22 = r1
    uses: dwLoc0C_23 = PHI(dwLoc0C_22, dwLoc0C_21)
dwLoc0C_23: orig: dwLoc0C
    def:  dwLoc0C_23 = PHI(dwLoc0C_22, dwLoc0C_21)
    uses: r1_14 = dwLoc0C_23
C_26: orig: C
    def:  C_26 = SLICE(CZS_8, bool, 2) (alias)
    uses: use C_26
S_28: orig: S
    def:  S_28 = SLICE(CZS_8, bool, 0) (alias)
    uses: use S_28
Z_30: orig: Z
    def:  Z_30 = SLICE(CZS_8, bool, 1) (alias)
    uses: use Z_30
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def bp
	def r1
	def wArg04
	goto l1
	// succ:  l1
done:
	dwLoc0C_23 = PHI(dwLoc0C_22, dwLoc0C_21)
	Mem13 = PHI(Mem11, Mem10)
	r1_14 = dwLoc0C_23
	bp_16 = dwLoc04_18
	r63_17 = fp
	return
	// succ:  proc1_exit
ge3:
	dwLoc0C_21 = r1
	goto done
	// succ:  done
l1:
	r63_2 = fp
	r63_3 = fp - 0x00000004
	dwLoc04_18 = bp
	bp_6 = fp - 0x00000004
	dwLoc0C_19 = 0x00000000
	CZS_8 = wArg04 - 0x0003
	C_26 = SLICE(CZS_8, bool, 2) (alias)
	S_28 = SLICE(CZS_8, bool, 0) (alias)
	Z_30 = SLICE(CZS_8, bool, 1) (alias)
	branch Test(GE,CZS_8) ge3
	// succ:  l2 ge3
l2:
	dwLoc0C_22 = r1
	goto done
	// succ:  done
proc1_exit:
	use bp_16
	use C_26
	use r1_14
	use r63_17
	use S_28
	use Z_30
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Reg32("bp", 5);
                var r1 = m.Reg32("r1", 1);
                var flags = m.Architecture.GetFlagGroup("CZS");
                var cr = m.Frame.EnsureFlagGroup(flags);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, bp);
                m.Assign(bp, sp);
                m.MStore(m.IAdd(bp, -8), m.Word32(0));
                m.Assign(cr, m.ISub(m.Mem16(m.IAdd(bp, 8)), 0x3));
                m.BranchIf(m.Test(ConditionCode.GE, cr), "ge3");

                m.MStore(m.IAdd(bp, -8), r1);
                m.Goto("done");

                m.Label("ge3");
                m.MStore(m.IAdd(bp, -8), r1);

                m.Label("done");
                m.Assign(r1, m.Mem32(m.IAdd(bp, -8)));
                m.Assign(bp, m.Mem32(sp));
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
    uses: SZ_2 = cond(esi & esi)
          SZ_2 = cond(esi & esi)
SZ_2: orig: SZ
    def:  SZ_2 = cond(esi & esi)
    uses: Z_4 = SLICE(SZ_2, bool, 2) (alias)
C_3: orig: C
    def:  C_3 = false
    uses: CZ_5 = C_3 | Z_4 (alias)
Z_4: orig: Z
    def:  Z_4 = SLICE(SZ_2, bool, 2) (alias)
    uses: CZ_5 = C_3 | Z_4 (alias)
CZ_5: orig: CZ
    def:  CZ_5 = C_3 | Z_4 (alias)
    uses: al_6 = Test(ULE,CZ_5)
al_6: orig: al
    def:  al_6 = Test(ULE,CZ_5)
// proc1
// Return size: 0
define proc1
proc1_entry:
	def esi
	// succ:  l1
l1:
	SZ_2 = cond(esi & esi)
	Z_4 = SLICE(SZ_2, bool, 2) (alias)
	C_3 = false
	CZ_5 = C_3 | Z_4 (alias)
	al_6 = Test(ULE,CZ_5)
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var sz = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("SZ"));
                var cz = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("CZ"));
                var c = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("C"));
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
          call r3 (retsize: 4;)	uses: r1:r1,r2:r2_7,r3:r3	defs: r1:r1_6,r2:r2_9,r3:r3_10
r2_2: orig: r2
    def:  r2_2 = 0x00000010
    uses: r2_7 = PHI(r2, r2_2)
r3:r3
    def:  def r3
    uses: call r3 (retsize: 4;)	uses: r1:r1,r2:r2_7,r3:r3	defs: r1:r1_6,r2:r2_9,r3:r3_10
          call r3 (retsize: 4;)	uses: r1:r1,r2:r2_7,r3:r3	defs: r1:r1_6,r2:r2_9,r3:r3_10
r1_6: orig: r1
    def:  call r3 (retsize: 4;)	uses: r1:r1,r2:r2_7,r3:r3	defs: r1:r1_6,r2:r2_9,r3:r3_10
    uses: use r1_6
r2_7: orig: r2
    def:  r2_7 = PHI(r2, r2_2)
    uses: call r3 (retsize: 4;)	uses: r1:r1,r2:r2_7,r3:r3	defs: r1:r1_6,r2:r2_9,r3:r3_10
r2:r2
    def:  def r2
    uses: r2_7 = PHI(r2, r2_2)
r2_9: orig: r2
    def:  call r3 (retsize: 4;)	uses: r1:r1,r2:r2_7,r3:r3	defs: r1:r1_6,r2:r2_9,r3:r3_10
    uses: use r2_9
r3_10: orig: r3
    def:  call r3 (retsize: 4;)	uses: r1:r1,r2:r2_7,r3:r3	defs: r1:r1_6,r2:r2_9,r3:r3_10
    uses: use r3_10
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	def r3
	def r2
	// succ:  l1
l1:
	branch r1 true
	// succ:  l2 true
l2:
	r2_2 = 0x00000010
	// succ:  true
true:
	r2_7 = PHI(r2, r2_2)
	call r3 (retsize: 4;)
		uses: r1:r1,r2:r2_7,r3:r3
		defs: r1:r1_6,r2:r2_9,r3:r3_10
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_6
	use r2_9
	use r3_10
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var r3 = m.Register("r3");
                m.BranchIf(r1, "true");
                m.Assign(r2, m.Word32(16));
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
    uses: r63_2 = fp
          r63_7 = fp - 0x00000004
          call proc1 (retsize: 0;)	uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04	defs: r3:r3_11,r4:r4_12,r63:r63_9
r63_2: orig: r63
    def:  r63_2 = fp
    uses: r63_19 = PHI(r63_14, r63_2)
Mem0:Mem
    def:  def Mem0
    uses: r4_6 = Mem0[r4_4 + 0x00000004:word32]
r4_4: orig: r4
    def:  r4_4 = dwArg04
    uses: branch r4_4 == 0x00000000 m1Base
          r4_6 = Mem0[r4_4 + 0x00000004:word32]
r4_5: orig: r4
    def:  r4_5 = 0x00000000
    uses: r4_18 = PHI(r4_13, r4_5)
r4_6: orig: r4
    def:  r4_6 = Mem0[r4_4 + 0x00000004:word32]
    uses: dwLoc04_16 = r4_6
          call proc1 (retsize: 0;)	uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04	defs: r3:r3_11,r4:r4_12,r63:r63_9
r63_7: orig: r63
    def:  r63_7 = fp - 0x00000004
Mem8: orig: Mem0
r63_9: orig: r63
    def:  call proc1 (retsize: 0;)	uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04	defs: r3:r3_11,r4:r4_12,r63:r63_9
    uses: r63_14 = r63_9 + 0x00000004
r3:r3
    def:  def r3
    uses: call proc1 (retsize: 0;)	uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04	defs: r3:r3_11,r4:r4_12,r63:r63_9
          r3_17 = PHI(r3_11, r3)
r3_11: orig: r3
    def:  call proc1 (retsize: 0;)	uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04	defs: r3:r3_11,r4:r4_12,r63:r63_9
    uses: r3_17 = PHI(r3_11, r3)
r4_12: orig: r4
    def:  call proc1 (retsize: 0;)	uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04	defs: r3:r3_11,r4:r4_12,r63:r63_9
    uses: r4_13 = r4_12 + 0x00000001
r4_13: orig: r4
    def:  r4_13 = r4_12 + 0x00000001
    uses: r4_18 = PHI(r4_13, r4_5)
r63_14: orig: r63
    def:  r63_14 = r63_9 + 0x00000004
    uses: r63_19 = PHI(r63_14, r63_2)
dwArg04:Stack +0004
    def:  def dwArg04
    uses: r4_4 = dwArg04
          call proc1 (retsize: 0;)	uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04	defs: r3:r3_11,r4:r4_12,r63:r63_9
dwLoc04_16: orig: dwLoc04
    def:  dwLoc04_16 = r4_6
    uses: call proc1 (retsize: 0;)	uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04	defs: r3:r3_11,r4:r4_12,r63:r63_9
r3_17: orig: r3
    def:  r3_17 = PHI(r3_11, r3)
    uses: use r3_17
r4_18: orig: r4
    def:  r4_18 = PHI(r4_13, r4_5)
    uses: use r4_18
r63_19: orig: r63
    def:  r63_19 = PHI(r63_14, r63_2)
    uses: use r63_19
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def Mem0
	def r3
	def dwArg04
	// succ:  l1
l1:
	r63_2 = fp
	r4_4 = dwArg04
	branch r4_4 == 0x00000000 m1Base
	// succ:  m0Induction m1Base
m0Induction:
	r4_6 = Mem0[r4_4 + 0x00000004:word32]
	r63_7 = fp - 0x00000004
	dwLoc04_16 = r4_6
	call proc1 (retsize: 0;)
		uses: r3:r3,r4:r4_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_16,Stack +0008:dwArg04
		defs: r3:r3_11,r4:r4_12,r63:r63_9
	r4_13 = r4_12 + 0x00000001
	r63_14 = r63_9 + 0x00000004
	goto m2Done
	// succ:  m2Done
m1Base:
	r4_5 = 0x00000000
	// succ:  m2Done
m2Done:
	r63_19 = PHI(r63_14, r63_2)
	r4_18 = PHI(r4_13, r4_5)
	r3_17 = PHI(r3_11, r3)
	return
	// succ:  proc1_exit
proc1_exit:
	use r3_17
	use r4_18
	use r63_19
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r3 = m.Reg32("r3", 3);
                var r4 = m.Reg32("r4", 4);
                m.Assign(sp, m.Frame.FramePointer);   // Establish frame.
                m.Assign(r4, m.Mem32(m.IAdd(sp, 4)));
                m.BranchIf(m.Eq0(r4), "m1Base");

                m.Label("m0Induction");
                m.Assign(r4, m.Mem32(m.IAdd(r4, 4)));
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r4);
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
@"a_1: orig: a
    def:  a_1 = 0x00000003
    uses: b_2 = a_1
b_2: orig: b
    def:  b_2 = a_1
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	a_1 = 0x00000003
	b_2 = a_1
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var regA = RegisterStorage.Reg32("a", 0);
                var regB = RegisterStorage.Reg32("b", 1);
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
    uses: Mem2[0x00123400:word32] = a
Mem2: orig: Mem0
    def:  Mem2[0x00123400:word32] = a
// proc1
// Return size: 0
define proc1
proc1_entry:
	def a
	// succ:  l1
l1:
	Mem2[0x00123400:word32] = a
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            this.addUseInstructions = true;
            RunTest(sExp, m =>
            {
                var a = m.Reg32("a", 0);
                m.MStore(m.Word32(0x123400), a);
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
b_2: orig: b
    def:  b_2 = 0xFFFFFFFF
    uses: b_3 = PHI(b, b_2)
b_3: orig: b
    def:  b_3 = PHI(b, b_2)
    uses: return b_3
b:b
    def:  def b
    uses: b_3 = PHI(b, b_2)
// proc1
// Return size: 0
define proc1
proc1_entry:
	def a
	def b
	// succ:  l1
l1:
	branch a == 0x00000000 m_2
	// succ:  m_1 m_2
m_1:
	b_2 = 0xFFFFFFFF
	// succ:  m_2
m_2:
	b_3 = PHI(b, b_2)
	return b_3
	// succ:  proc1_exit
proc1_exit:
======
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
    uses: eax_3 = Mem0[eax:word32]
Mem0:Mem
    def:  def Mem0
    uses: eax_3 = Mem0[eax:word32]
eax_3: orig: eax
    def:  eax_3 = Mem0[eax:word32]
    uses: ah_4 = SLICE(eax_3, byte, 8) (alias)
          use eax_3
ah_4: orig: ah
    def:  ah_4 = SLICE(eax_3, byte, 8) (alias)
    uses: Mem5[0x00001234:byte] = ah_4
Mem5: orig: Mem0
    def:  Mem5[0x00001234:byte] = ah_4
// proc1
// Return size: 0
define proc1
proc1_entry:
	def eax
	def Mem0
	// succ:  l1
l1:
	eax_3 = Mem0[eax:word32]
	ah_4 = SLICE(eax_3, byte, 8) (alias)
	Mem5[0x00001234:byte] = ah_4
	return
	// succ:  proc1_exit
proc1_exit:
	use eax_3
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var regEax = new RegisterStorage("eax", 0, 0, PrimitiveType.Word32);
                var regAh = new RegisterStorage("ah", 0, 8, PrimitiveType.Byte);
                var eax = m.Frame.EnsureRegister(regEax);
                var ah = m.Frame.EnsureRegister(regAh);
                m.Assign(eax, m.Mem32(eax));
                m.MStore(m.Word32(0x1234), ah);
                m.Return();
            });
        }

        [Test]
        public void SsaAliasedPhi()
        {
            var sExp =
            #region Expected
@"Mem0:Mem
    def:  def Mem0
    uses: ecx_2 = Mem0[0x00542300:word32]
          branch Mem0[0x00010042:bool] mBranch2
ecx_2: orig: ecx
    def:  ecx_2 = Mem0[0x00542300:word32]
    uses: ecx_6 = DPB(ecx_2, cl_4, 0) (alias)
ecx_3: orig: ecx
    def:  ecx_3 = 0x00000020
    uses: ecx_5 = PHI(ecx_6, ecx_3)
cl_4: orig: cl
    def:  cl_4 = 0x2A
    uses: ecx_6 = DPB(ecx_2, cl_4, 0) (alias)
ecx_5: orig: ecx
    def:  ecx_5 = PHI(ecx_6, ecx_3)
    uses: Mem7[0x00010232:word32] = ecx_5
ecx_6: orig: ecx
    def:  ecx_6 = DPB(ecx_2, cl_4, 0) (alias)
    uses: ecx_5 = PHI(ecx_6, ecx_3)
Mem7: orig: Mem0
    def:  Mem7[0x00010232:word32] = ecx_5
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  l1
l1:
	ecx_2 = Mem0[0x00542300:word32]
	branch Mem0[0x00010042:bool] mBranch2
	// succ:  mBranch1 mBranch2
mBranch1:
	cl_4 = 0x2A
	ecx_6 = DPB(ecx_2, cl_4, 0) (alias)
	goto mCommon
	// succ:  mCommon
mBranch2:
	ecx_3 = 0x00000020
	// succ:  mCommon
mCommon:
	ecx_5 = PHI(ecx_6, ecx_3)
	Mem7[0x00010232:word32] = ecx_5
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion
            RunTest(sExp, m =>
            {
                var ecx = m.Frame.EnsureRegister(new RegisterStorage("ecx", 1, 0, PrimitiveType.Word32));
                var cl = m.Frame.EnsureRegister(new RegisterStorage("cl", 1, 0, PrimitiveType.Byte));

                m.Assign(ecx, m.Mem32(m.Word32(0x542300)));
                m.BranchIf(m.Mem(PrimitiveType.Bool, m.Word32(0x10042)), "mBranch2");

                m.Label("mBranch1");
                m.Assign(cl, 42);
                m.Goto("mCommon");

                m.Label("mBranch2");
                m.Assign(ecx, 32);

                m.Label("mCommon");
                m.MStore(m.Word32(0x10232), ecx);
                m.Return();
            });
        }

        [Test(Description = "Multiple assignments in the same block")]
        public void SsaManyAssignments()
        {
            var sExp =
            #region Expected
@"Mem0:Mem
    def:  def Mem0
    uses: eax_2 = Mem0[0x00543200:word32]
          edx_3 = Mem0[0x00543208:word32]
eax_2: orig: eax
    def:  eax_2 = Mem0[0x00543200:word32]
    uses: eax_4 = eax_2 + edx_3
edx_3: orig: edx
    def:  edx_3 = Mem0[0x00543208:word32]
    uses: eax_4 = eax_2 + edx_3
eax_4: orig: eax
    def:  eax_4 = eax_2 + edx_3
    uses: Mem5[0x00642300:word32] = eax_4
Mem5: orig: Mem0
    def:  Mem5[0x00642300:word32] = eax_4
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  l1
l1:
	eax_2 = Mem0[0x00543200:word32]
	edx_3 = Mem0[0x00543208:word32]
	eax_4 = eax_2 + edx_3
	Mem5[0x00642300:word32] = eax_4
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var edx = m.Frame.EnsureRegister(new RegisterStorage("edx", 2, 0, PrimitiveType.Word32));
                var eax = m.Frame.EnsureRegister(new RegisterStorage("eax", 0, 0, PrimitiveType.Word32));

                m.Assign(eax, m.Mem32(m.Word32(0x543200)));
                m.Assign(edx, m.Mem32(m.Word32(0x543208)));
                m.Assign(eax, m.IAdd(eax, edx));
                m.MStore(m.Word32(0x642300), eax);
                m.Return();
            });
        }

        [Test(Description = "Multiple assignments in the same block with aliases")]
        public void SsaManyAssignmentsWithAliases()
        {
            var sExp =
            #region Expected
@"Mem0:Mem
    def:  def Mem0
    uses: edx_2 = Mem0[0x00543200:word32]
edx_2: orig: edx
    def:  edx_2 = Mem0[0x00543200:word32]
    uses: dl_3 = (byte) edx_2 (alias)
dl_3: orig: dl
    def:  dl_3 = (byte) edx_2 (alias)
    uses: Mem4[0x00642300:byte] = dl_3
Mem4: orig: Mem0
    def:  Mem4[0x00642300:byte] = dl_3
    uses: edx_5 = Mem4[0x00543208:word32]
edx_5: orig: edx
    def:  edx_5 = Mem4[0x00543208:word32]
    uses: dl_6 = (byte) edx_5 (alias)
dl_6: orig: dl
    def:  dl_6 = (byte) edx_5 (alias)
    uses: Mem7[0x00642308:byte] = dl_6
Mem7: orig: Mem0
    def:  Mem7[0x00642308:byte] = dl_6
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  l1
l1:
	edx_2 = Mem0[0x00543200:word32]
	dl_3 = (byte) edx_2 (alias)
	Mem4[0x00642300:byte] = dl_3
	edx_5 = Mem4[0x00543208:word32]
	dl_6 = (byte) edx_5 (alias)
	Mem7[0x00642308:byte] = dl_6
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var edx = m.Frame.EnsureRegister(new RegisterStorage("edx", 2, 0, PrimitiveType.Word32));
                var dl = m.Frame.EnsureRegister(new RegisterStorage("dl", 2, 0, PrimitiveType.Byte));

                m.Assign(edx, m.Mem32(m.Word32(0x543200)));
                m.MStore(m.Word32(0x642300), dl);
                m.Assign(edx, m.Mem32(m.Word32(0x543208)));
                m.MStore(m.Word32(0x642308), dl);
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
    uses: C_3 = os_service(ebx, out ebx_2)
ebx_2: orig: ebx
    def:  C_3 = os_service(ebx, out ebx_2)
    uses: Mem4[0x00123400:word32] = ebx_2
C_3: orig: C
    def:  C_3 = os_service(ebx, out ebx_2)
Mem4: orig: Mem0
    def:  Mem4[0x00123400:word32] = ebx_2
// proc1
// Return size: 0
define proc1
proc1_entry:
	def ebx
	// succ:  l1
l1:
	C_3 = os_service(ebx, out ebx_2)
	Mem4[0x00123400:word32] = ebx_2
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var ebx = m.Reg32("ebx", 2);
                var C = m.Flags("C");
                var func = new ExternalProcedure("os_service", FunctionType.Func(C, ebx));

                m.Assign(C, m.Fn(func, ebx, m.Out(ebx.DataType, ebx)));
                m.MStore(m.Word32(0x123400), ebx);
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
    uses: es_bx_4 = Mem0[es:bx:word32]
bx:bx
    def:  def bx
    uses: es_bx_4 = Mem0[es:bx:word32]
Mem0:Mem
    def:  def Mem0
    uses: es_bx_4 = Mem0[es:bx:word32]
          bx_7 = Mem0[es_bx_4 + 0x0010:word32]
es_bx_4: orig: es_bx
    def:  es_bx_4 = Mem0[es:bx:word32]
    uses: es_5 = SLICE(es_bx_4, word16, 16) (alias)
          bx_6 = (word16) es_bx_4 (alias)
          bx_7 = Mem0[es_bx_4 + 0x0010:word32]
es_5: orig: es
    def:  es_5 = SLICE(es_bx_4, word16, 16) (alias)
    uses: use es_5
bx_6: orig: bx
    def:  bx_6 = (word16) es_bx_4 (alias)
bx_7: orig: bx
    def:  bx_7 = Mem0[es_bx_4 + 0x0010:word32]
    uses: use bx_7
// proc1
// Return size: 0
define proc1
proc1_entry:
	def es
	def bx
	def Mem0
	// succ:  l1
l1:
	es_bx_4 = Mem0[es:bx:word32]
	es_5 = SLICE(es_bx_4, word16, 16) (alias)
	bx_6 = (word16) es_bx_4 (alias)
	bx_7 = Mem0[es_bx_4 + 0x0010:word32]
	return
	// succ:  proc1_exit
proc1_exit:
	use bx_7
	use es_5
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var es = m.Reg16("es", 10);
                var bx = m.Reg16("bx", 3);
                var es_bx = m.Frame.EnsureSequence(es.Storage, bx.Storage, PrimitiveType.SegPtr32);

                m.Assign(es_bx, m.SegMem(PrimitiveType.Word32, es, bx));
                m.Assign(bx, m.SegMem(PrimitiveType.Word32, es, m.IAdd(bx, 16)));
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
    uses: eax_3 = Mem0[eax:word32]
Mem0:Mem
    def:  def Mem0
    uses: eax_3 = Mem0[eax:word32]
eax_3: orig: eax
    def:  eax_3 = Mem0[eax:word32]
    uses: al_4 = (byte) eax_3 (alias)
al_4: orig: al
    def:  al_4 = (byte) eax_3 (alias)
    uses: Mem5[0x00123100:byte] = al_4
          Mem6[0x00123108:byte] = al_4
Mem5: orig: Mem0
    def:  Mem5[0x00123100:byte] = al_4
Mem6: orig: Mem0
    def:  Mem6[0x00123108:byte] = al_4
// proc1
// Return size: 0
define proc1
proc1_entry:
	def eax
	def Mem0
	// succ:  l1
l1:
	eax_3 = Mem0[eax:word32]
	al_4 = (byte) eax_3 (alias)
	Mem5[0x00123100:byte] = al_4
	Mem6[0x00123108:byte] = al_4
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var eax = m.Frame.EnsureRegister(new RegisterStorage("eax", 0, 0, PrimitiveType.Word32));
                var al = m.Frame.EnsureRegister(new RegisterStorage("al", 0, 0, PrimitiveType.Byte));

                m.Assign(eax, m.Mem32(eax));
                m.MStore(m.Word32(0x123100), al);            // store the low-order byte
                m.MStore(m.Word32(0x123108), al);            // ...twice.
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

        [Test(Description = "Emulates calling an imported API Win32 on MIPS")]
        public void Ssa_ConstantPropagation()
        {
            // 0x00031234
            //this.importReferences
            var sExp =
@"r13_1: orig: r13
    def:  r13_1 = 0x00030000
r12_2: orig: r12
    def:  r12_2 = ImportedFunc
r6:r6
    def:  def r6
    uses: r14_4 = ImportedFunc(r6)
r14_4: orig: r14
    def:  r14_4 = ImportedFunc(r6)
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r6
	// succ:  l1
l1:
	r13_1 = 0x00030000
	r12_2 = ImportedFunc
	r14_4 = ImportedFunc(r6)
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            var addr = Address.Ptr32(0x00031234);
            importReferences.Add(addr, new NamedImportReference(
                addr, "COREDLL.DLL", "fnFoo"));
            importResolver.Stub(i => i.ResolveToImportedProcedureConstant(
                Arg<Statement>.Is.Anything,
                Arg<Constant>.Matches(c => c.ToUInt32() == 0x00031234)))
                .Return(new ProcedureConstant(
                    PrimitiveType.Ptr32,
                    new ExternalProcedure(
                        "ImportedFunc",
                        FunctionType.Func(Reg(14), Reg(6)))));

            RunTest(sExp, m =>
            {
                var r13 = m.Reg32("r13", 13);
                var r12 = m.Reg32("r12", 12);
                m.Assign(r13, 0x00030000);
                m.Assign(r12, m.Mem32(m.IAdd(r13, 0x1234)));
                m.Call(r12, 0);
                m.Return();
            });
        }

        [Test(Description = "Make sure SSA state behaves correctly in presence of loops.")]
        public void SsaWhileLoop()
        {
            var sExp =
            #region Expected
@"eax_1: orig: eax
    def:  eax_1 = 0x00000000
    uses: eax_5 = PHI(eax_1, eax_7)
ebx_2: orig: ebx
    def:  ebx_2 = PHI(ebx, ebx_8)
    uses: SCZ_3 = cond(ebx_2 - 0x00000000)
          eax_7 = eax_5 + Mem0[ebx_2:word32]
          ebx_8 = Mem0[ebx_2 + 0x00000004:word32]
SCZ_3: orig: SCZ
    def:  SCZ_3 = cond(ebx_2 - 0x00000000)
    uses: Z_4 = SLICE(SCZ_3, bool, 1) (alias)
Z_4: orig: Z
    def:  Z_4 = SLICE(SCZ_3, bool, 1) (alias)
    uses: branch Test(NE,Z_4) l2Body
eax_5: orig: eax
    def:  eax_5 = PHI(eax_1, eax_7)
    uses: eax_7 = eax_5 + Mem0[ebx_2:word32]
          return eax_5
eax_7: orig: eax
    def:  eax_7 = eax_5 + Mem0[ebx_2:word32]
    uses: eax_5 = PHI(eax_1, eax_7)
ebx_8: orig: ebx
    def:  ebx_8 = Mem0[ebx_2 + 0x00000004:word32]
    uses: ebx_2 = PHI(ebx, ebx_8)
ebx:ebx
    def:  def ebx
    uses: ebx_2 = PHI(ebx, ebx_8)
Mem0:Mem
    def:  def Mem0
    uses: eax_7 = eax_5 + Mem0[ebx_2:word32]
          ebx_8 = Mem0[ebx_2 + 0x00000004:word32]
// proc1
// Return size: 0
define proc1
proc1_entry:
	def ebx
	def Mem0
	// succ:  l1
l1:
	eax_1 = 0x00000000
	goto l3Head
	// succ:  l3Head
l2Body:
	eax_7 = eax_5 + Mem0[ebx_2:word32]
	ebx_8 = Mem0[ebx_2 + 0x00000004:word32]
	// succ:  l3Head
l3Head:
	eax_5 = PHI(eax_1, eax_7)
	ebx_2 = PHI(ebx, ebx_8)
	SCZ_3 = cond(ebx_2 - 0x00000000)
	Z_4 = SLICE(SCZ_3, bool, 1) (alias)
	branch Test(NE,Z_4) l2Body
	// succ:  l4Exit l2Body
l4Exit:
	return eax_5
	// succ:  proc1_exit
proc1_exit:
======
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
                m.Assign(eax, m.IAdd(eax, m.Mem32(ebx)));
                m.Assign(ebx, m.Mem32(m.IAdd(ebx, 4)));

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
@"Mem0:Mem
    def:  def Mem0
    uses: cx_2 = Mem0[0x1234:word16]
cx_2: orig: cx
    def:  cx_2 = Mem0[0x1234:word16]
    uses: Mem3[0x00001236:word16] = cx_2
Mem3: orig: Mem0
    def:  Mem3[0x00001236:word16] = cx_2
    uses: es_cx_4 = Mem3[0x00001238:word32]
es_cx_4: orig: es_cx
    def:  es_cx_4 = Mem3[0x00001238:word32]
    uses: cx_6 = (word16) es_cx_4 (alias)
          es_8 = SLICE(es_cx_4, word16, 16) (alias)
cl_5: orig: cl
    def:  cl_5 = 0x2D
    uses: cx_7 = DPB(cx_6, cl_5, 0) (alias)
cx_6: orig: cx
    def:  cx_6 = (word16) es_cx_4 (alias)
    uses: cx_7 = DPB(cx_6, cl_5, 0) (alias)
cx_7: orig: cx
    def:  cx_7 = DPB(cx_6, cl_5, 0) (alias)
    uses: use cx_7
es_8: orig: es
    def:  es_8 = SLICE(es_cx_4, word16, 16) (alias)
    uses: use es_8
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  m0
m0:
	cx_2 = Mem0[0x1234:word16]
	Mem3[0x00001236:word16] = cx_2
	es_cx_4 = Mem3[0x00001238:word32]
	cx_6 = (word16) es_cx_4 (alias)
	es_8 = SLICE(es_cx_4, word16, 16) (alias)
	cl_5 = 0x2D
	cx_7 = DPB(cx_6, cl_5, 0) (alias)
	return
	// succ:  proc1_exit
proc1_exit:
	use cx_7
	use es_8
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var es = m.Reg16("es", 2);
                var cx = m.Reg16("cx", 1);
                var cl = m.Reg8("cl", 1);
                var es_cx = m.Frame.EnsureSequence(es.Storage, cx.Storage, PrimitiveType.SegPtr32);

                m.Label("m0");
                m.Assign(cx, m.Mem16(m.Word16(0x1234)));
                m.MStore(m.Word32(0x1236), cx);
                m.Assign(es_cx, m.Mem32(m.Word32(0x1238)));
                m.Assign(cl, m.Byte(45));
                m.Return();
            });
        }

        [Test(Description = "A variable carried around uselessly in a loop.")]
        public void SsaLoopCarriedVariable()
        {
            var sExp =
            #region Expected
@"r1_1: orig: r1
    def:  r1_1 = PHI(r1, r1_4)
    uses: branch r1_1 == 0x00000000 m3done
          r1_4 = r1_1 + Mem0[r2:word32]
          use r1_1
r1_4: orig: r1
    def:  r1_4 = r1_1 + Mem0[r2:word32]
    uses: r1_1 = PHI(r1, r1_4)
r1:r1
    def:  def r1
    uses: r1_1 = PHI(r1, r1_4)
r2:r2
    def:  def r2
    uses: r1_4 = r1_1 + Mem0[r2:word32]
Mem0:Mem
    def:  def Mem0
    uses: r1_4 = r1_1 + Mem0[r2:word32]
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	def r2
	def Mem0
	// succ:  m0
m0:
	r1_1 = PHI(r1, r1_4)
	branch r1_1 == 0x00000000 m3done
	// succ:  m1notdone m3done
m1notdone:
	r1_4 = r1_1 + Mem0[r2:word32]
	goto m0
	// succ:  m0
m3done:
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_1
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);

                m.Label("m0");
                m.BranchIf(m.Eq0(r1), "m3done");

                m.Label("m1notdone");
                m.Assign(r1, m.IAdd(r1, m.Mem32(r2)));
                m.Goto("m0");

                m.Label("m3done");
                m.Return();
            });
        }

        [Test]
        public void SsaAliasedRegisters()
        {
            var sExp =
            #region Expected
@"Mem0:Mem
    def:  def Mem0
    uses: bl_2 = Mem0[0x1234:word16]
bl_2: orig: bl
    def:  bl_2 = Mem0[0x1234:word16]
    uses: bx_5 = DPB(bx, bl_2, 0) (alias)
bh_3: orig: bh
    def:  bh_3 = 0x00
bx:bx
    def:  def bx
    uses: bx_5 = DPB(bx, bl_2, 0) (alias)
bx_5: orig: bx
    def:  bx_5 = DPB(bx, bl_2, 0) (alias)
    uses: bx_6 = DPB(bx_5, 0x00, 8) (alias)
bx_6: orig: bx
    def:  bx_6 = DPB(bx_5, 0x00, 8) (alias)
    uses: Mem7[0x1236:word16] = bx_6
          use bx_6
Mem7: orig: Mem0
    def:  Mem7[0x1236:word16] = bx_6
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	def bx
	// succ:  m0
m0:
	bl_2 = Mem0[0x1234:word16]
	bx_5 = DPB(bx, bl_2, 0) (alias)
	// succ:  m1
m1:
	bh_3 = 0x00
	bx_6 = DPB(bx_5, 0x00, 8) (alias)
	Mem7[0x1236:word16] = bx_6
	return
	// succ:  proc1_exit
proc1_exit:
	use bx_6
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var bx = m.Frame.EnsureRegister(new RegisterStorage("bx", 3, 0, PrimitiveType.Word16));
                var bh = m.Frame.EnsureRegister(new RegisterStorage("bh", 3, 8, PrimitiveType.Byte));
                var bl = m.Frame.EnsureRegister(new RegisterStorage("bl", 3, 0, PrimitiveType.Byte));

                m.Label("m0");
                m.Assign(bl, m.Mem16(m.Word16(0x1234)));
                m.Label("m1");
                m.Assign(bh, 0);
                m.MStore(m.Word16(0x1236), bx);
                m.Return();
            });
        }

        [Test]
        public void SsaAliasedRegistersWithPhi()
        {
            var sExp =
            #region Expected
@"Mem0:Mem
    def:  def Mem0
    uses: bl_2 = Mem0[0x1234:word16]
bl_2: orig: bl
    def:  bl_2 = Mem0[0x1234:word16]
    uses: branch bl_2 > 3 m2
          bx_5 = DPB(bx, bl_2, 0) (alias)
bh_3: orig: bh
    def:  bh_3 = 0x00
bx:bx
    def:  def bx
    uses: bx_5 = DPB(bx, bl_2, 0) (alias)
bx_5: orig: bx
    def:  bx_5 = DPB(bx, bl_2, 0) (alias)
    uses: bx_6 = DPB(bx_5, 0x00, 8) (alias)
          bx_8 = PHI(bx_5, bx_6)
bx_6: orig: bx
    def:  bx_6 = DPB(bx_5, 0x00, 8) (alias)
    uses: Mem7[0x1236:word16] = bx_6
          bx_8 = PHI(bx_5, bx_6)
Mem7: orig: Mem0
    def:  Mem7[0x1236:word16] = bx_6
bx_8: orig: bx
    def:  bx_8 = PHI(bx_5, bx_6)
    uses: use bx_8
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	def bx
	// succ:  m0
m0:
	bl_2 = Mem0[0x1234:word16]
	bx_5 = DPB(bx, bl_2, 0) (alias)
	branch bl_2 > 3 m2
	// succ:  m1 m2
m1:
	bh_3 = 0x00
	bx_6 = DPB(bx_5, 0x00, 8) (alias)
	Mem7[0x1236:word16] = bx_6
	// succ:  m2
m2:
	bx_8 = PHI(bx_5, bx_6)
	return
	// succ:  proc1_exit
proc1_exit:
	use bx_8
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var bx = m.Frame.EnsureRegister(new RegisterStorage("bx", 3, 0, PrimitiveType.Word16));
                var bh = m.Frame.EnsureRegister(new RegisterStorage("bh", 3, 8, PrimitiveType.Byte));
                var bl = m.Frame.EnsureRegister(new RegisterStorage("bl", 3, 0, PrimitiveType.Byte));

                m.Label("m0");
                m.Assign(bl, m.Mem16(m.Word16(0x1234)));
                m.BranchIf(m.Gt(bl, 3), "m2");

                m.Label("m1");
                m.Assign(bh, 0);
                m.MStore(m.Word16(0x1236), bx);

                m.Label("m2");
                m.Return();
            });
        }

        [Test]
        public void SsaAlias2()
        {
            var sExp =
@"si:si
    def:  def si
    uses: bl_3 = Mem0[si:byte]
Mem0:Mem
    def:  def Mem0
    uses: bl_3 = Mem0[si:byte]
bl_3: orig: bl
    def:  bl_3 = Mem0[si:byte]
    uses: SCZO_4 = cond(bl_3 - 0x02)
          bx_7 = DPB(bx, bl_3, 0) (alias)
SCZO_4: orig: SCZO
    def:  SCZO_4 = cond(bl_3 - 0x02)
    uses: branch Test(UGT,SCZO_4) m2
bh_5: orig: bh
    def:  bh_5 = 0x00
    uses: bx_8 = DPB(bx_7, bh_5, 8) (alias)
bx:bx
    def:  def bx
    uses: bx_7 = DPB(bx, bl_3, 0) (alias)
bx_7: orig: bx
    def:  bx_7 = DPB(bx, bl_3, 0) (alias)
    uses: bx_8 = DPB(bx_7, bh_5, 8) (alias)
bx_8: orig: bx
    def:  bx_8 = DPB(bx_7, bh_5, 8) (alias)
    uses: bx_9 = bx_8 + bx_8
          bx_9 = bx_8 + bx_8
bx_9: orig: bx
    def:  bx_9 = bx_8 + bx_8
    uses: Mem10[bx_9:word16] = 0x0000
Mem10: orig: Mem0
    def:  Mem10[bx_9:word16] = 0x0000
// proc1
// Return size: 0
define proc1
proc1_entry:
	def si
	def Mem0
	def bx
	// succ:  m0
m0:
	bl_3 = Mem0[si:byte]
	bx_7 = DPB(bx, bl_3, 0) (alias)
	SCZO_4 = cond(bl_3 - 0x02)
	branch Test(UGT,SCZO_4) m2
	// succ:  m1 m2
m1:
	bh_5 = 0x00
	bx_8 = DPB(bx_7, bh_5, 8) (alias)
	bx_9 = bx_8 + bx_8
	Mem10[bx_9:word16] = 0x0000
	// succ:  m2
m2:
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            RunTest(sExp, m =>
            {
                var bl = m.Reg8("bl", 3, 0);
                var bh = m.Reg8("bh", 3, 8);
                var bx = m.Reg16("bx", 3);
                var si = m.Reg16("si", 6);
                var SCZO = m.Flags("SCZO");

                m.Label("m0");
                m.Assign(bl, m.Mem8(si));
                m.Assign(SCZO, m.Cond(m.ISub(bl, 2)));
                m.BranchIf(m.Test(ConditionCode.UGT, SCZO), "m2");

                m.Label("m1");
                m.Assign(bh, 0);
                m.Assign(bx, m.IAdd(bx, bx));
                m.MStore(bx, m.Word16(0));

                m.Label("m2");
                m.Return();
            });
        }

        [Test]
        public void SsaFlags()
        {
            var sExp =
            #region Expected
@"r1:r1
    def:  def r1
    uses: SZ_2 = cond(r1)
SZ_2: orig: SZ
    def:  SZ_2 = cond(r1)
    uses: S_4 = SLICE(SZ_2, bool, 0) (alias)
          Z_5 = SLICE(SZ_2, bool, 1) (alias)
C_3: orig: C
    def:  C_3 = false
    uses: use C_3
S_4: orig: S
    def:  S_4 = SLICE(SZ_2, bool, 0) (alias)
    uses: use S_4
Z_5: orig: Z
    def:  Z_5 = SLICE(SZ_2, bool, 1) (alias)
    uses: use Z_5
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	// succ:  m0
m0:
	SZ_2 = cond(r1)
	S_4 = SLICE(SZ_2, bool, 0) (alias)
	Z_5 = SLICE(SZ_2, bool, 1) (alias)
	C_3 = false
	return
	// succ:  proc1_exit
proc1_exit:
	use C_3
	use S_4
	use Z_5
======
";
            #endregion
            RunTest_FrameAccesses(sExp, m =>
            {
                var SZ = m.Flags("SZ");
                var C = m.Flags("C");
                var r1 = m.Reg32("r1", 1);

                m.Label("m0");
                m.Assign(SZ, m.Cond(r1));
                m.Assign(C, Constant.Bool(false));
                m.Return();
            });
        }

        [Test]
        public void SsaDefineSequence()
        {
            var sExp =
            #region Expected
@"r1:r1
    def:  def r1
    uses: r2_r1_2 = r1 *s 1431655765
          r2_3 = SLICE(r1 *s 1431655765, word32, 32) (alias)
r2_r1_2: orig: r2_r1
    def:  r2_r1_2 = r1 *s 1431655765
    uses: r1_5 = (word32) r2_r1_2 (alias)
r2_3: orig: r2
    def:  r2_3 = SLICE(r1 *s 1431655765, word32, 32) (alias)
    uses: Mem4[0x00040000:word32] = r2_3
          use r2_3
Mem4: orig: Mem0
    def:  Mem4[0x00040000:word32] = r2_3
r1_5: orig: r1
    def:  r1_5 = (word32) r2_r1_2 (alias)
    uses: use r1_5
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	// succ:  l1
l1:
	r2_r1_2 = r1 *s 1431655765
	r2_3 = SLICE(r1 *s 1431655765, word32, 32) (alias)
	r1_5 = (word32) r2_r1_2 (alias)
	Mem4[0x00040000:word32] = r2_3
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_5
	use r2_3
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var c = Constant.Int32(0x55555555);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var r2_r1 = m.Frame.EnsureSequence(r2.Storage, r1.Storage, PrimitiveType.Word64);

                m.Assign(r2_r1, m.SMul(r1, c));
                m.MStore(m.Word32(0x0040000), r2);
                m.Return();
            });
        }

        [Test]
        public void SsaWhile_TwoLoopExits()
        {
            var sExp =
            #region Expected
@"Mem1: orig: Mem0
    def:  Mem1 = PHI(Mem0, Mem3)
    uses: branch Mem1[0x00004010:bool] m4
          branch Mem1[0x00004011:bool] m4
r1_2: orig: r1
    def:  r1_2 = 0x00000003
Mem3: orig: Mem0
    def:  Mem3[0x00004020:bool] = true
    uses: Mem1 = PHI(Mem0, Mem3)
Mem0:Mem
    def:  def Mem0
    uses: Mem1 = PHI(Mem0, Mem3)
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  m1
m1:
	Mem1 = PHI(Mem0, Mem3)
	branch Mem1[0x00004010:bool] m4
	// succ:  m2 m4
m2:
	branch Mem1[0x00004011:bool] m4
	// succ:  m3 m4
m3:
	Mem3[0x00004020:bool] = true
	goto m1
	// succ:  m1
m4:
	r1_2 = 0x00000003
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);

                m.Label("m1");
                m.BranchIf(m.Mem(PrimitiveType.Bool, m.Word32(0x04010)), "m4");

                m.Label("m2");
                m.BranchIf(m.Mem(PrimitiveType.Bool, m.Word32(0x04011)), "m4");

                m.Label("m3");
                m.MStore(m.Word32(0x04020), Constant.True());
                m.Goto("m1");

                m.Label("m4");
                m.Assign(r1, 3);
                m.Return();
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void SsaPartialRegisters()
        {
            var sExp =
            #region Expected
@"Mem0:Mem
    def:  def Mem0
    uses: ax_2 = Mem0[0x00002000:word16]
          bx_3 = Mem0[0x00002002:word16]
ax_2: orig: ax
    def:  ax_2 = Mem0[0x00002000:word16]
bx_3: orig: bx
    def:  bx_3 = Mem0[0x00002002:word16]
    uses: bh_8 = SLICE(bx_3, byte, 8) (alias)
          bx_6 = PHI(bx_3, bx_7)
al_5: orig: al
    def:  al_5 = bh_8
    uses: return al_5
bx_6: orig: bx
    def:  bx_6 = PHI(bx_3, bx_7)
    uses: bx_7 = DPB(bx_6, bh_8, 8) (alias)
bx_7: orig: bx
    def:  bx_7 = DPB(bx_6, bh_8, 8) (alias)
    uses: branch bx_7 >= 0x0000 m0
          bx_6 = PHI(bx_3, bx_7)
bh_8: orig: bh
    def:  bh_8 = SLICE(bx_3, byte, 8) (alias)
    uses: al_5 = bh_8
          bx_7 = DPB(bx_6, bh_8, 8) (alias)
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  l1
l1:
	ax_2 = Mem0[0x00002000:word16]
	bx_3 = Mem0[0x00002002:word16]
	bh_8 = SLICE(bx_3, byte, 8) (alias)
	// succ:  m0
m0:
	bx_6 = PHI(bx_3, bx_7)
	bx_7 = DPB(bx_6, bh_8, 8) (alias)
	al_5 = bh_8
	branch bx_7 >= 0x0000 m0
	// succ:  m1 m0
m1:
	return al_5
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var al = m.Frame.EnsureRegister(new RegisterStorage("al", 0, 0, PrimitiveType.Byte));
                var bl = m.Frame.EnsureRegister(new RegisterStorage("bl", 3, 0, PrimitiveType.Byte));
                var ah = m.Frame.EnsureRegister(new RegisterStorage("ah", 0, 8, PrimitiveType.Byte));
                var bh = m.Frame.EnsureRegister(new RegisterStorage("bh", 3, 8, PrimitiveType.Byte));
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
                var bx = m.Frame.EnsureRegister(new RegisterStorage("bx", 3, 0, PrimitiveType.Word16));

                m.Assign(ax, m.Mem(ax.DataType, m.Word32(0x2000)));
                m.Assign(bx, m.Mem(bx.DataType, m.Word32(0x2002)));

                m.Label("m0");
                m.Assign(al, bh);
                m.BranchIf(m.Ge(bx, 0), "m0");

                m.Label("m1");
                m.Return(al);
            });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void SsaSingleConditionCode()
        {
            var sExp =
 @"r3:r3
    def:  def r3
    uses: Z_2 = cond(r3)
Z_2: orig: Z
    def:  Z_2 = cond(r3)
    uses: r3_3 = (int32) Test(EQ,Z_2)
r3_3: orig: r3
    def:  r3_3 = (int32) Test(EQ,Z_2)
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r3
	// succ:  l1
l1:
	Z_2 = cond(r3)
	r3_3 = (int32) Test(EQ,Z_2)
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            RunTest(sExp, m =>
            {
                var Z = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("Z"));
                var r3 = m.Register(3);

                m.Assign(Z, m.Cond(r3));
                m.Assign(r3, m.Cast(PrimitiveType.Int32, m.Test(ConditionCode.EQ, Z)));
                m.Return();
            });
        }


        [Test]
        [Category(Categories.UnitTests)]
        public void SsaConditionCodeExactMatch()
        {
            var sExp =
@"r3:r3
    def:  def r3
    uses: CZ_2 = cond(r3)
CZ_2: orig: CZ
    def:  CZ_2 = cond(r3)
    uses: r3_3 = (int32) Test(ULE,CZ_2)
r3_3: orig: r3
    def:  r3_3 = (int32) Test(ULE,CZ_2)
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r3
	// succ:  l1
l1:
	CZ_2 = cond(r3)
	r3_3 = (int32) Test(ULE,CZ_2)
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            RunTest(sExp, m =>
            {
                var CZ = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("CZ"));
                var r3 = m.Register(3);

                m.Assign(CZ, m.Cond(r3));
                m.Assign(r3, m.Cast(PrimitiveType.Int32, m.Test(ConditionCode.ULE, CZ)));
                m.Return();
            });
        }

        [Test(Description = "The flag group being used is a subset of the definition")]
        [Category(Categories.UnitTests)]
        public void SsaConditionCode_UseSubset()
        {
            var sExp =
    @"r3:r3
    def:  def r3
    uses: SCZ_2 = cond(r3)
SCZ_2: orig: SCZ
    def:  SCZ_2 = cond(r3)
    uses: SZ_3 = SLICE(SCZ_2, bool, 1) (alias)
SZ_3: orig: SZ
    def:  SZ_3 = SLICE(SCZ_2, bool, 1) (alias)
    uses: r3_4 = (int32) Test(LE,SZ_3)
r3_4: orig: r3
    def:  r3_4 = (int32) Test(LE,SZ_3)
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r3
	// succ:  l1
l1:
	SCZ_2 = cond(r3)
	SZ_3 = SLICE(SCZ_2, bool, 1) (alias)
	r3_4 = (int32) Test(LE,SZ_3)
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            RunTest(sExp, m =>
            {
                var SCZ = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("SCZ"));
                var SZ = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("SZ"));
                var r3 = m.Register(3);

                m.Assign(SCZ, m.Cond(r3));
                m.Assign(r3, m.Cast(PrimitiveType.Int32, m.Test(ConditionCode.LE, SZ)));
                m.Return();
            });
        }

        [Test(Description = "Ensures proper aliasing behavior with flag groups")]
        [Category(Categories.UnitTests)]
        public void SsaFlagGroupAliasing()
        {
            var sExp =
@"r1:r1
    def:  def r1
    uses: SCZ_2 = cond(r1)
SCZ_2: orig: SCZ
    def:  SCZ_2 = cond(r1)
    uses: Z_3 = SLICE(SCZ_2, bool, 1) (alias)
          SZ_4 = SLICE(SCZ_2, bool, 1) (alias)
Z_3: orig: Z
    def:  Z_3 = SLICE(SCZ_2, bool, 1) (alias)
    uses: branch Test(EQ,Z_3) mZero
SZ_4: orig: SZ
    def:  SZ_4 = SLICE(SCZ_2, bool, 1) (alias)
    uses: branch Test(LT,SZ_4) mLessThan
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	// succ:  l1
l1:
	SCZ_2 = cond(r1)
	Z_3 = SLICE(SCZ_2, bool, 1) (alias)
	SZ_4 = SLICE(SCZ_2, bool, 1) (alias)
	branch Test(EQ,Z_3) mZero
	// succ:  l2 mZero
l2:
	branch Test(LT,SZ_4) mLessThan
	// succ:  l3 mLessThan
l3:
	return
	// succ:  proc1_exit
mLessThan:
	return
	// succ:  proc1_exit
mZero:
	goto mLessThan
	// succ:  mLessThan
proc1_exit:
======
";

            RunTest(sExp, m =>
            {
                var SCZ = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("SCZ"));
                var Z = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("Z"));
                var SZ = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("SZ"));
                var r1 = m.Register(1);

                m.Assign(SCZ, m.Cond(r1));
                m.BranchIf(m.Test(ConditionCode.EQ, Z), "mZero");
                m.BranchIf(m.Test(ConditionCode.LT, SZ), "mLessThan");
                m.Return();
                m.Label("mZero");
                m.Label("mLessThan");
                m.Return();
            });
        }

        [Test]
        public void SsaHiwordLoword()
        {
            var sExp =
            #region Expected
                @"Mem0:Mem
    def:  def Mem0
    uses: al_2 = Mem0[0x1234:byte]
al_2: orig: al
    def:  al_2 = Mem0[0x1234:byte]
    uses: Mem4[0x1236:byte] = al_2 *u ah_3
ah_3: orig: ah
    def:  ah_3 = 0x03
    uses: Mem4[0x1236:byte] = al_2 *u ah_3
Mem4: orig: Mem0
    def:  Mem4[0x1236:byte] = al_2 *u ah_3
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  l1
l1:
	al_2 = Mem0[0x1234:byte]
	ah_3 = 0x03
	Mem4[0x1236:byte] = al_2 *u ah_3
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var al = m.Frame.EnsureRegister(new RegisterStorage("al", 0, 0, PrimitiveType.Byte));
                var ah = m.Frame.EnsureRegister(new RegisterStorage("ah", 0, 8, PrimitiveType.Byte));

                m.Assign(al, m.Mem8(m.Word16(0x1234)));
                m.Assign(ah, 3);
                m.MStore(m.Word16(0x1236), m.UMul(al, ah));
                m.Return();
            });
        }

        [Test]
        public void SsaFpuReturn()
        {
            var sExp =
            #region Expected
@"Top_1: orig: Top
    def:  Top_1 = 0x00
Top_2: orig: Top
    def:  Top_2 = 0xFF
    uses: use Top_2
ST3: orig: ST
rLoc1_4: orig: rLoc1
    def:  rLoc1_4 = 2.0
    uses: use rLoc1_4
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	Top_1 = 0x00
	Top_2 = 0xFF
	rLoc1_4 = 2.0
	return
	// succ:  proc1_exit
proc1_exit:
	use rLoc1_4
	use Top_2
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var ST = new MemoryIdentifier("ST", PrimitiveType.Ptr32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));
                var Top = m.Frame.EnsureRegister(new RegisterStorage("Top", 76, 0, PrimitiveType.Byte));

                m.Assign(Top, 0);
                m.Assign(Top, m.ISub(Top, 1));
                m.Emit(new Store(
                    new MemoryAccess(ST, Top, PrimitiveType.Real64),
                    Constant.Real64(2.0)));
                m.Return();
            });
        }

        [Test(Description = "Merge variables in stack frame")]
        [Category(Categories.UnitTests)]
        public void SsaJoinSsaPieces()
        {
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_2 = fp
          r63_3 = fp - 0x00000002
          r63_5 = fp - 0x00000004
r63_2: orig: r63
    def:  r63_2 = fp
r63_3: orig: r63
    def:  r63_3 = fp - 0x00000002
Mem4: orig: Mem0
r63_5: orig: r63
    def:  r63_5 = fp - 0x00000004
    uses: use r63_5
Mem6: orig: Mem0
r1_7: orig: r1
    def:  r1_7 = dwLoc04_10
    uses: use r1_7
wLoc02_8: orig: wLoc02
    def:  wLoc02_8 = 0x1234
    uses: dwLoc04_10 = SEQ(wLoc02_8, wLoc04_9) (alias)
wLoc04_9: orig: wLoc04
    def:  wLoc04_9 = 0x5678
    uses: dwLoc04_10 = SEQ(wLoc02_8, wLoc04_9) (alias)
dwLoc04_10: orig: dwLoc04
    def:  dwLoc04_10 = SEQ(wLoc02_8, wLoc04_9) (alias)
    uses: r1_7 = dwLoc04_10
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	// succ:  l1
l1:
	r63_2 = fp
	r63_3 = fp - 0x00000002
	wLoc02_8 = 0x1234
	r63_5 = fp - 0x00000004
	wLoc04_9 = 0x5678
	dwLoc04_10 = SEQ(wLoc02_8, wLoc04_9) (alias)
	r1_7 = dwLoc04_10
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_7
	use r63_5
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(sp, m.Frame.FramePointer);
                // Push two word16's on stack
                m.Assign(sp, m.ISub(sp, 2));
                m.MStore(sp, m.Word16(0x1234));
                m.Assign(sp, m.ISub(sp, 2));
                m.MStore(sp, m.Word16(0x5678));
                m.Assign(r1, m.Mem32(sp));
                m.Return();
            });
        }

        [Test(Description = "Ignore dead storage")]
        [Category(Categories.UnitTests)]
        public void SsaLoadDpb()
        {
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_2 = fp
          r63_3 = fp - 0x00000004
r63_2: orig: r63
    def:  r63_2 = fp
r63_3: orig: r63
    def:  r63_3 = fp - 0x00000004
    uses: use r63_3
r1:r1
    def:  def r1
    uses: dwLoc04_8 = r1
Mem5: orig: Mem0
Mem6: orig: Mem0
t2_7: orig: t2
    def:  t2_7 = wLoc04_9
dwLoc04_8: orig: dwLoc04
    def:  dwLoc04_8 = r1
wLoc04_9: orig: wLoc04
    def:  wLoc04_9 = 0x0000
    uses: t2_7 = wLoc04_9
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def r1
	// succ:  l1
l1:
	r63_2 = fp
	r63_3 = fp - 0x00000004
	dwLoc04_8 = r1
	wLoc04_9 = 0x0000
	t2_7 = wLoc04_9
	return
	// succ:  proc1_exit
proc1_exit:
	use r63_3
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var t2 = m.Temp(PrimitiveType.Word16, "t2");
                m.Assign(sp, m.Frame.FramePointer);
                // Push word32 on stack -- really only to make space
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r1);
                m.MStore(sp, m.Word16(0));
                m.Assign(t2, m.Mem16(sp));
                m.Return();
            });
        }
        // 
        // W: [----------]
        // R: [----------]
        // R = W

        // W: [----------]
        // R: [----]
        // R = slice(W)

        // W1: [-hi-]
        // W2:       [-lo-]
        // R:  [----------]
        // R = SEQ(W1,W2)

        // W1: [----------]
        // W2: [----]
        // R:  [----------]
        // R = DPB(W1, W2)

        // W1:       [----------]
        // W2: [----------]
        // R:        [----------]
        // R = DPB(W1, SLICE(W2))

        // W1:       [----------]
        // W2:  [----------]
        // W3:      [---------]
        // R      [

        [Test(Description = "Ignore dead storage")]
        [Category(Categories.UnitTests)]
        public void SsaSubregisterAssignments()
        {
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
    uses: r63_2 = fp
r63_2: orig: r63
    def:  r63_2 = fp
    uses: use r63_2
Mem0:Mem
    def:  def Mem0
    uses: ax_4 = Mem0[0x00123400:word16]
ax_4: orig: ax
    def:  ax_4 = Mem0[0x00123400:word16]
    uses: Mem5[0x00123402:word16] = ax_4
          ax_10 = DPB(ax_4, al_6, 8) (alias)
Mem5: orig: Mem0
    def:  Mem5[0x00123402:word16] = ax_4
    uses: al_6 = Mem5[0x00123404:byte]
          ah_7 = Mem5[0x00123405:byte]
al_6: orig: al
    def:  al_6 = Mem5[0x00123404:byte]
    uses: Mem8[0x00123406:byte] = al_6
          ax_10 = DPB(ax_4, al_6, 8) (alias)
ah_7: orig: ah
    def:  ah_7 = Mem5[0x00123405:byte]
    uses: Mem9[0x00123407:byte] = ah_7
          ax_11 = DPB(ax_10, ah_7, 0) (alias)
Mem8: orig: Mem0
    def:  Mem8[0x00123406:byte] = al_6
Mem9: orig: Mem0
    def:  Mem9[0x00123407:byte] = ah_7
ax_10: orig: ax
    def:  ax_10 = DPB(ax_4, al_6, 8) (alias)
    uses: ax_11 = DPB(ax_10, ah_7, 0) (alias)
ax_11: orig: ax
    def:  ax_11 = DPB(ax_10, ah_7, 0) (alias)
    uses: use ax_11
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def Mem0
	// succ:  l1
l1:
	r63_2 = fp
	ax_4 = Mem0[0x00123400:word16]
	Mem5[0x00123402:word16] = ax_4
	al_6 = Mem5[0x00123404:byte]
	ax_10 = DPB(ax_4, al_6, 8) (alias)
	ah_7 = Mem5[0x00123405:byte]
	ax_11 = DPB(ax_10, ah_7, 0) (alias)
	Mem8[0x00123406:byte] = al_6
	Mem9[0x00123407:byte] = ah_7
	return
	// succ:  proc1_exit
proc1_exit:
	use ax_11
	use r63_2
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
                var ah = m.Frame.EnsureRegister(new RegisterStorage("ah", 0, 0, PrimitiveType.Byte));
                var al = m.Frame.EnsureRegister(new RegisterStorage("al", 0, 8, PrimitiveType.Byte));
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(ax, m.Mem16(m.Word32(0x00123400)));
                m.MStore(m.Word32(0x00123402), ax);
                m.Assign(al, m.Mem8(m.Word32(0x00123404)));
                m.Assign(ah, m.Mem8(m.Word32(0x00123405)));
                m.MStore(m.Word32(0x00123406), al);
                m.MStore(m.Word32(0x00123407), ah);
                m.Return();
            });
        }

        [Test]
        public void SsaTransBlockLiveness()
        {
            //$TODO: the generation of 
            //           eax_13 = DPB(eax_12, al_3, 8) (alias)
            // although not incorrect is redundant. Investigate ways
            // to get rid of it.
            var sExp =
            #region Expected
@"eax_1: orig: eax
    def:  eax_1 = 0x00000004
    uses: eax_13 = DPB(eax_1, al_3, 8) (alias)
Mem2: orig: Mem0
    def:  Mem2[0x00123400:word32] = 0x00000004
    uses: al_3 = Mem2[0x00123408:byte]
al_3: orig: al
    def:  al_3 = Mem2[0x00123408:byte]
    uses: SZC_4 = cond(al_3 - 0x30)
          SZC_5 = cond(al_3 - 0x39)
          eax_13 = DPB(eax_1, al_3, 8) (alias)
          eax_14 = DPB(eax_13, al_3, 8) (alias)
SZC_4: orig: SZC
    def:  SZC_4 = cond(al_3 - 0x30)
    uses: branch Test(LT,SZC_4) m4_not_number
          C_11 = SLICE(SZC_4, bool, 2) (alias)
          S_21 = SLICE(SZC_4, bool, 0) (alias)
          Z_25 = SLICE(SZC_4, bool, 1) (alias)
SZC_5: orig: SZC
    def:  SZC_5 = cond(al_3 - 0x39)
    uses: branch Test(GT,SZC_5) m4_not_number
          C_9 = SLICE(SZC_5, bool, 2) (alias)
          S_19 = SLICE(SZC_5, bool, 0) (alias)
          Z_23 = SLICE(SZC_5, bool, 1) (alias)
al_6: orig: al
    def:  al_6 = 0x00
    uses: eax_17 = DPB(eax_16, al_6, 8) (alias)
al_7: orig: al
    def:  al_7 = 0x01
    uses: eax_15 = DPB(eax_14, al_7, 8) (alias)
C_8: orig: C
    def:  C_8 = PHI(C_9, C_10)
    uses: use C_8
C_9: orig: C
    def:  C_9 = SLICE(SZC_5, bool, 2) (alias)
    uses: C_10 = PHI(C_11, C_9)
          C_8 = PHI(C_9, C_10)
C_10: orig: C
    def:  C_10 = PHI(C_11, C_9)
    uses: C_8 = PHI(C_9, C_10)
C_11: orig: C
    def:  C_11 = SLICE(SZC_4, bool, 2) (alias)
    uses: C_10 = PHI(C_11, C_9)
eax_12: orig: eax
    def:  eax_12 = PHI(eax_15, eax_17)
    uses: use eax_12
eax_13: orig: eax
    def:  eax_13 = DPB(eax_1, al_3, 8) (alias)
    uses: eax_14 = DPB(eax_13, al_3, 8) (alias)
          eax_16 = PHI(eax_13, eax_14)
eax_14: orig: eax
    def:  eax_14 = DPB(eax_13, al_3, 8) (alias)
    uses: eax_15 = DPB(eax_14, al_7, 8) (alias)
          eax_16 = PHI(eax_13, eax_14)
eax_15: orig: eax
    def:  eax_15 = DPB(eax_14, al_7, 8) (alias)
    uses: eax_12 = PHI(eax_15, eax_17)
eax_16: orig: eax
    def:  eax_16 = PHI(eax_13, eax_14)
    uses: eax_17 = DPB(eax_16, al_6, 8) (alias)
eax_17: orig: eax
    def:  eax_17 = DPB(eax_16, al_6, 8) (alias)
    uses: eax_12 = PHI(eax_15, eax_17)
S_18: orig: S
    def:  S_18 = PHI(S_19, S_20)
    uses: use S_18
S_19: orig: S
    def:  S_19 = SLICE(SZC_5, bool, 0) (alias)
    uses: S_20 = PHI(S_21, S_19)
          S_18 = PHI(S_19, S_20)
S_20: orig: S
    def:  S_20 = PHI(S_21, S_19)
    uses: S_18 = PHI(S_19, S_20)
S_21: orig: S
    def:  S_21 = SLICE(SZC_4, bool, 0) (alias)
    uses: S_20 = PHI(S_21, S_19)
Z_22: orig: Z
    def:  Z_22 = PHI(Z_23, Z_24)
    uses: use Z_22
Z_23: orig: Z
    def:  Z_23 = SLICE(SZC_5, bool, 1) (alias)
    uses: Z_24 = PHI(Z_25, Z_23)
          Z_22 = PHI(Z_23, Z_24)
Z_24: orig: Z
    def:  Z_24 = PHI(Z_25, Z_23)
    uses: Z_22 = PHI(Z_23, Z_24)
Z_25: orig: Z
    def:  Z_25 = SLICE(SZC_4, bool, 1) (alias)
    uses: Z_24 = PHI(Z_25, Z_23)
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	eax_1 = 0x00000004
	Mem2[0x00123400:word32] = 0x00000004
	al_3 = Mem2[0x00123408:byte]
	eax_13 = DPB(eax_1, al_3, 8) (alias)
	eax_14 = DPB(eax_13, al_3, 8) (alias)
	SZC_4 = cond(al_3 - 0x30)
	C_11 = SLICE(SZC_4, bool, 2) (alias)
	S_21 = SLICE(SZC_4, bool, 0) (alias)
	Z_25 = SLICE(SZC_4, bool, 1) (alias)
	branch Test(LT,SZC_4) m4_not_number
	// succ:  m1_maybe_number m4_not_number
m1_maybe_number:
	SZC_5 = cond(al_3 - 0x39)
	C_9 = SLICE(SZC_5, bool, 2) (alias)
	S_19 = SLICE(SZC_5, bool, 0) (alias)
	Z_23 = SLICE(SZC_5, bool, 1) (alias)
	branch Test(GT,SZC_5) m4_not_number
	// succ:  m2_number m4_not_number
m2_number:
	al_7 = 0x01
	eax_15 = DPB(eax_14, al_7, 8) (alias)
	return
	// succ:  proc1_exit
m4_not_number:
	Z_24 = PHI(Z_25, Z_23)
	S_20 = PHI(S_21, S_19)
	eax_16 = PHI(eax_13, eax_14)
	C_10 = PHI(C_11, C_9)
	al_6 = 0x00
	eax_17 = DPB(eax_16, al_6, 8) (alias)
	return
	// succ:  proc1_exit
proc1_exit:
	Z_22 = PHI(Z_23, Z_24)
	S_18 = PHI(S_19, S_20)
	eax_12 = PHI(eax_15, eax_17)
	C_8 = PHI(C_9, C_10)
	use C_8
	use eax_12
	use S_18
	use Z_22
======
";
#endregion
            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var eax = m.Frame.EnsureRegister(new RegisterStorage("eax", 0, 0, PrimitiveType.Word32));
                var al = m.Frame.EnsureRegister(new RegisterStorage("al", 0, 8, PrimitiveType.Byte));
                var SZC = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("SZC"));

                m.Assign(eax, 4);
                m.MStore(m.Word32(0x00123400), eax);
                m.Assign(al, m.Mem8(m.Word32(0x00123408)));
                m.Assign(SZC, m.Cond(m.ISub(al, 0x30)));
                m.BranchIf(m.Test(ConditionCode.LT, SZC), "m4_not_number");

                m.Label("m1_maybe_number");
                m.Assign(SZC, m.Cond(m.ISub(al, 0x39)));
                m.BranchIf(m.Test(ConditionCode.GT, SZC), "m4_not_number");

                m.Label("m2_number");
                m.Assign(al, 1);
                m.Return();

                m.Label("m4_not_number");
                m.Assign(al, 0);
                m.Return();
            });
        }

        [Test]
        public void SsaUsesAfterTrivialPhiRemoving()
        {
            Given_Procedure("proc", m =>
            {
                var a = m.Reg32("a", 0);
                var b = m.Reg32("b", 1);

                m.Label("init");
                m.Assign(a, m.Mem32(m.Word32(0x1234)));
                m.Assign(b, 0);
                m.BranchIf(m.Le(a, 10), "head");

                m.Label("check_failed");
                m.Assign(a, m.IAdd(a, 1));
                m.Goto("done");

                m.Label("head");
                m.BranchIf(m.Ge(b, a), "done");

                m.Label("loop");
                m.Assign(b, m.IAdd(b, 1));
                m.Goto("head");

                m.Label("done");
                m.MStore(m.Word32(0x5678), a);
                m.Return();
            });

            When_RunSsaTransform();

            var expected =
            #region Expected
@"proc_entry:
	def Mem0
	goto init
check_failed:
	a_7 = a_2 + 0x00000001
done:
	a_8 = PHI(a_7, a_2)
	Mem9[0x00005678:word32] = a_8
	return
head:
	b_4 = PHI(b_3, b_6)
	branch b_4 >= a_2 done
	goto loop
init:
	a_2 = Mem0[0x00001234:word32]
	b_3 = 0x00000000
	branch a_2 <= 0x0000000A head
	goto check_failed
loop:
	b_6 = b_4 + 0x00000001
	goto head
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);

    }

        [Test]
        public void SsaFPUBlockStateAfterTrivialPhiRemoving()
        {
            Given_Procedure("proc", m =>
            {
                var rLocal = m.Frame.EnsureFpuStackVariable(
                    -8,
                    PrimitiveType.Real32);
                var a = m.Reg32("a", 0);

                m.Label("init");
                m.Assign(rLocal, a);
                m.Assign(a, m.Mem32(m.Word32(0x100)));
                m.Goto("looptest");

                m.Label("again");
                m.BranchIf(m.Ne(a, 0), "looptest");

                m.Label("failed");
                m.Assign(a, 0xFF);
                m.Goto("exit");

                m.Label("looptest");
                m.Assign(a, m.IAdd(a, m.Mem32(a)));
                m.BranchIf(m.Ne(a, 0x20), "again");

                m.Label("done");
                m.Assign(a, rLocal);

                m.Label("exit");
                m.MStore(m.Word32(0x300), rLocal);
                m.Return();
            });

            When_RunSsaTransform();

            var expected =
            #region Expected
@"proc_entry:
	def a
	def Mem0
	goto init
again:
	branch a_7 != 0x00000000 looptest
	goto failed
done:
	a_10 = rLoc8_2
exit:
	Mem12[0x00000300:real32] = rLoc8_2
	return
failed:
	a_8 = 0x000000FF
	goto exit
init:
	rLoc8_2 = a
	a_4 = Mem0[0x00000100:word32]
looptest:
	a_5 = PHI(a_4, a_7)
	a_7 = a_5 + Mem0[a_5:word32]
	branch a_7 != 0x00000020 again
	goto done
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test(Description = "Verifies that the data type of a register parameter was not overwritten.")]
        public void Ssa_KeepSignatureRegisterType()
        {
            var proc = Given_Procedure(nameof(Ssa_KeepSignatureRegisterType), m => {
                var r2 = m.Reg32("r2", 2);
                m.MStore(m.Word32(0x00123400), r2);
                m.Return();
            });
            proc.Signature = FunctionType.Action(
                new Identifier("r2", PrimitiveType.Real32, proc.Architecture.GetRegister("r2")));

            When_RunSsaTransform();

            var ass = proc.Statements
                .Select(stm => stm.Instruction as Assignment)
                .Where(instr => instr != null)
                .Single();
            Assert.AreEqual("r2_2 = r2", ass.ToString());
            // verify that data type of register was not overwritten
            Assert.AreEqual("word32", ass.Dst.DataType.ToString());
            Assert.AreEqual("real32", ass.Src.DataType.ToString());
        }

        [Test(Description = "Verifies that the user can override register names.")]
        public void SsaUserSignatureWithRegisterArgs()
        {
            var proc = Given_Procedure("test", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.MStore(m.Word32(0x123400), m.Cast(PrimitiveType.Byte, r1));
                m.MStore(m.Word32(0x123404), m.Cast(PrimitiveType.Real32, r2));
                m.Return();
            });
            proc.Signature = FunctionType.Action(
                    new Identifier[] {
                        new Identifier("r2", PrimitiveType.Char, r1.Storage),  // perverse but legal.
                        new Identifier("r1", PrimitiveType.Real32, r2.Storage)
                    });
            var sExp =
            #region Expected
@"test_entry:
	def r2
	r1_2 = r2
	def r1
	r2_5 = r1
l1:
	Mem3[0x00123400:byte] = (byte) r1_2
	Mem6[0x00123404:real32] = (real32) r2_5
	return
test_exit:
";
            #endregion

            When_RunSsaTransform();

            AssertProcedureCode(sExp);
        }

        [Test]
        public void SsaReal64Arg()
        {
            var proc = Given_Procedure("proc", m =>
            {
                var a = m.Reg32("a", 0);
                var b = m.Reg32("b", 1);
                var fp = m.Frame.FramePointer;

                m.Label("body");
                m.Assign(a, m.Mem32(m.IAdd(fp, 4)));
                m.Assign(b, m.Mem32(m.IAdd(fp, 8)));
                m.MStore(m.Word32(0x5678), a);
                m.MStore(m.Word32(0x567C), b);
                m.Return();
            });
            proc.Signature = FunctionType.Action(
                new Identifier(
                    "doubleArg",
                    PrimitiveType.Real64,
                    new StackArgumentStorage(4, PrimitiveType.Real64))
            );

            When_RunSsaTransform();
            When_RenameFrameAccesses();

            var expected =
            #region Expected
@"proc_entry:
	def fp
	def Mem0
	def doubleArg
	dwArg04_8 = SLICE(doubleArg, word32, 0)
	dwArg08_9 = SLICE(doubleArg, word32, 32)
body:
	a_3 = dwArg04_8
	b_4 = dwArg08_9
	Mem5[0x00005678:word32] = a_3
	Mem6[0x0000567C:word32] = b_4
	return
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);

        }

        [Test]
        public void SsaGlobals()
        {
            var proc = Given_Procedure("proc", m =>
            {
                var global = Identifier.Global("gbl", PrimitiveType.Word32);

                m.Label("body");
                m.MStore(m.Word32(0x5678), m.Mem32(m.Word32(0x1234)));
                m.MStore(m.Word32(0x1234), global);
                m.Return();
            });

            When_RunSsaTransform();

            var expected =
            #region Expected
@"proc_entry:
	def Mem0
	def gbl
body:
	Mem2[0x00005678:word32] = Mem0[0x00001234:word32]
	Mem4[0x00001234:word32] = gbl
	return
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);

        }

        [Test]
        public void SsaFpuCallDefsShouldBeIds()
        {
            var topReg = Given_FpuStackRegister("Top");
            var ST = Given_FpuStackBase("ST");
            Given_Procedure("main", m =>
            {
                var Top = m.Frame.EnsureRegister(topReg);
                m.Label("body");
                m.Assign(Top, 0);
                m.Call("fn", 4);
                m.MStore(m.Word32(0x1234), m.Mem(ST, PrimitiveType.Real64, Top));
                m.Assign(Top, m.IAdd(Top, 1));
                m.Return();
            });
            Given_ProcedureFlow("fn", p => new ProcedureFlow(p)
            {
                Trashed =
                {
                    new FpuStackStorage(-1, PrimitiveType.Real64)
                },
                Constants =
                {
                    { topReg, Constant.SByte(-1)}
                }
            });

            When_RunSsaTransform();

            var expected =
            #region Expected
@"main_entry:
body:
	Top_1 = 0x00
	call fn (retsize: 4;)
		defs: FPU -1:rRet0_2
	ST3[Top_1 - 0x01:real64] = rRet0_2
	Top_4 = Top_1 - 1
	Mem5[0x00001234:real64] = ST3[Top_4:real64]
	Top_6 = Top_4 + 0x01
	return
main_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        public void SsaOverlappingStackDefs()
        {
            var proc = Given_Procedure("proc", m =>
            {
                var fp = m.Frame.FramePointer;

                m.Label("body");
                m.MStore(m.ISub(fp, 8), m.Word64(0x5678));
                m.MStore(m.ISub(fp, 8), m.Word32(0x1234));
                m.MStore(m.ISub(fp, 8), m.IAdd(m.Mem32(m.ISub(fp, 8)), 1));
                m.MStore(m.Word32(0x567C), m.Mem64(m.ISub(fp, 8)));
                m.Return();
            });

            When_RunSsaTransform();
            When_RenameFrameAccesses();

            var expected =
            #region Expected
@"proc_entry:
	def fp
body:
	qwLoc08_6 = 0x0000000000005678
	dwLoc08_7 = 0x00001234
	dwLoc08_8 = dwLoc08_7 + 0x00000001
	qwLoc08_9 = DPB(qwLoc08_6, dwLoc08_8, 0) (alias)
	Mem5[0x0000567C:word64] = qwLoc08_9
	return
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);

        }

        [Test]
        public void SsaLoopWithBreakAndReturn()
        {
            var proc = Given_Procedure("proc", m =>
            {
                var a = m.Reg32("a", 0);

                m.Label("body");
                m.Assign(a, m.Mem32(m.Word32(0x1234)));

                m.Label("head");
                m.BranchIf(m.Fn(m.Mem32(m.Word32(0xF))), "firstCondition");

                m.Label("break");
                m.MStore(m.Word32(0x5001), m.Word32(0x1111));
                m.Goto("follow");

                m.Label("firstCondition");
                m.BranchIf(m.Fn(m.Mem32(m.Word32(0x1))), "thirdCondition");

                m.Label("secondCondition");
                m.BranchIf(m.Fn(m.Mem32(m.Word32(0x2))), "thirdCondition");

                m.Label("firstAndSecondFailed");
                m.Goto("failed");

                m.Label("thirdCondition");
                m.BranchIf(m.Fn(m.Mem32(m.Word32(0x3))), "success");

                m.Label("failed");
                m.MStore(m.Word32(0x5002), m.Word32(0x2222));
                m.Goto("head");

                m.Label("success");
                m.MStore(m.Word32(0x567C), m.Word32(0x3333));
                m.Goto("return");

                m.Label("follow");
                m.MStore(m.Word32(0x567C), m.Word32(0x4444));

                m.Label("return");
                m.MStore(m.Word32(0x567C), a);
                m.Return();
            });

            When_RunSsaTransform();

            var expected =
            #region Expected
@"proc_entry:
	def Mem0
body:
	a_2 = Mem0[0x00001234:word32]
	goto head
break:
	Mem7[0x00005001:word32] = 0x00001111
	goto follow
failed:
	Mem6[0x00005002:word32] = 0x00002222
	goto head
firstAndSecondFailed:
	goto failed
firstCondition:
	branch Mem3[0x00000001:word32]() thirdCondition
	goto secondCondition
follow:
	Mem8[0x0000567C:word32] = 0x00004444
	goto return
head:
	Mem3 = PHI(Mem0, Mem6)
	branch Mem3[0x0000000F:word32]() firstCondition
	goto break
return:
	Mem13[0x0000567C:word32] = a_2
	return
secondCondition:
	branch Mem3[0x00000002:word32]() thirdCondition
	goto firstAndSecondFailed
success:
	Mem5[0x0000567C:word32] = 0x00003333
	goto return
thirdCondition:
	branch Mem3[0x00000003:word32]() success
	goto failed
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);

        }
    }
}
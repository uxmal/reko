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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        private IProcessorArchitecture arch;
        private ProgramBuilder pb;
        private Dictionary<Address, ImportReference> importReferences;
        private ProgramDataFlow programFlow;
        private bool addUseInstructions;
        private Mock<IDynamicLinker> dynamicLinker;
        private SsaTransform sst;
        private CallingConvention fakeCc;
        private HashSet<RegisterStorage> trashedRegs;

        private Identifier r1;
        private Identifier r2;
        private Identifier r3;
        private Identifier r4;

        [SetUp]
        public void Setup()
        {
            this.addUseInstructions = false;
            this.dynamicLinker = new Mock<IDynamicLinker>();
            this.r1 = new Identifier("r1", PrimitiveType.Word32, new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
            this.r2 = new Identifier("r2", PrimitiveType.Word32, new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));
            this.r3 = new Identifier("r3", PrimitiveType.Word32, new RegisterStorage("r3", 3, 0, PrimitiveType.Word32));
            this.r4 = new Identifier("r4", PrimitiveType.Word32, new RegisterStorage("r4", 4, 0, PrimitiveType.Word32));

            var arch = new FakeArchitecture();
            Given_Architecture(arch);

            this.fakeCc = new FakeCallingConvention(
                    new[] { r1.Storage, r2.Storage },
                    new[] { r2.Storage, r3.Storage });
            this.trashedRegs = new HashSet<RegisterStorage> {
                    (RegisterStorage)r2.Storage,
                    (RegisterStorage)r3.Storage
                };



            this.programFlow = new ProgramDataFlow();
        }

        private void Given_Architecture(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.pb = new ProgramBuilder(this.arch);
            this.importReferences = pb.Program.ImportReferences;
        }

        private void Given_BigEndianArchitecture()
        {
            var arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Endianness).Returns(EndianServices.Big);
            arch.Setup(a => a.Name).Returns("fake-be-arch");
            arch.Setup(a => a.CreateFrame()).Returns(() => new Frame(PrimitiveType.Ptr32));
            Given_Architecture(arch.Object);
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
            platform.Test_GetCallingConvention = s => fakeCc;
            platform.Test_CreateTrashedRegisters = () => trashedRegs;

            // Register r1 is assumed to always be implicit when calling
            // another procedure.
            var implicitRegs = new HashSet<RegisterStorage>
            {
                arch.GetRegister("r1")
            };
            program.Platform = platform;
            program.SegmentMap = new SegmentMap(
                Address.Ptr32(0x0000),
                new ImageSegment(
                    ".text",
                    Address.Ptr32(0),
                    0x40000,
                    AccessMode.ReadWriteExecute));

            var writer = new StringWriter();
            foreach (var proc in this.pb.Program.Procedures.Values)
            {
                var sst = new SsaTransform(
                    this.pb.Program,
                    proc,
                    new HashSet<Procedure>(),
                    dynamicLinker.Object,
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
                Console.WriteLine(sActual);
            Assert.AreEqual(sExp, sActual);
        }

        private void RunTest_FrameAccesses(string sExp, Action<ProcedureBuilder> builder)
        {
            pb.Add("proc1", builder);
            var dynamicLinker = new Mock<IDynamicLinker>();

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
                },
                Test_GetCallingConvention = s => fakeCc
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
                    dynamicLinker.Object,
                    programFlow);
                sst.Transform();

                // Propagate values and simplify the results.
                // We hope the the sequence
                //   esp = fp - 4
                //   mov [esp-4],eax
                // will become
                //   esp_2 = fp - 4
                //   mov [fp - 8],eax

                var vp = new ValuePropagator(
                    this.pb.Program.SegmentMap,
                    sst.SsaState,
                    program.CallGraph,
                    dynamicLinker.Object,
                    listener);
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
                Console.WriteLine("<< Expected ========");
                Console.WriteLine(sExp);
                Console.WriteLine(">> Actual ==========");
                Console.WriteLine(sActual);
            }
            Assert.AreEqual(sExp, sActual);
        }

        private Procedure Given_Procedure(string name, Action<ProcedureBuilder> builder)
        {
            return pb.Add(name, builder);
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
                dynamicLinker.Object,
                programFlow);
            sst.Transform();
            sst.SsaState.Validate(s => Assert.Fail(s));
        }

        private void When_RenameFrameAccesses()
        {
            sst.RenameFrameAccesses = true;
            sst.Transform();
            sst.SsaState.Dump(true);
            sst.SsaState.Validate(s => Assert.Fail(s));
        }

        private void When_AddUsesToExitBlock()
        {
            sst.AddUsesToExitBlock();
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
                Console.WriteLine(actual);
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
    uses: r1_19 = PHI((r1_9, l2), (r1_8, ge3))
r1_9: orig: r1
    def:  r1_9 = 0x00000000
    uses: r1_19 = PHI((r1_9, l2), (r1_8, ge3))
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
    def:  r1_19 = PHI((r1_9, l2), (r1_8, ge3))
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
	r1_19 = PHI((r1_9, l2), (r1_8, ge3))
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
    uses: Mem12 = PHI((Mem10, l2), (Mem8, ge3))
r1_9: orig: r1
    def:  r1_9 = 0x00000001
    uses: r1_22 = PHI((r1, l2), (r1_9, ge3))
Mem10: orig: Mem0
    uses: Mem12 = PHI((Mem10, l2), (Mem8, ge3))
dwLoc04_15: orig: dwLoc04
    def:  dwLoc04_15 = bp
    uses: bp_13 = dwLoc04_15
Mem12: orig: Mem0
    def:  Mem12 = PHI((Mem10, l2), (Mem8, ge3))
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
    def:  r1_22 = PHI((r1, l2), (r1_9, ge3))
    uses: use r1_22
r1:r1
    def:  def r1
    uses: r1_22 = PHI((r1, l2), (r1_9, ge3))
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
	r1_22 = PHI((r1, l2), (r1_9, ge3))
	Mem12 = PHI((Mem10, l2), (Mem8, ge3))
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
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");

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
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var sp = m.Frame.EnsureRegister(m.Procedure.Architecture.StackRegister);

                // Simulate the creation of a subroutine.
                var procSub = this.pb.Add("Adder", mm => { });
                var procSubFlow = new ProcedureFlow(m.Procedure)
                {
                    BitsUsed = {
                        { new StackArgumentStorage(4, PrimitiveType.Word32), new BitRange(0,32) },
                        { r1.Storage, new BitRange(0, 32) }
                    },
                    Trashed = { r1.Storage }
                };
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
    uses: Mem13 = PHI((Mem11, l2), (Mem10, ge3))
Mem11: orig: Mem0
    uses: Mem13 = PHI((Mem11, l2), (Mem10, ge3))
dwLoc0C_19: orig: dwLoc0C
    def:  dwLoc0C_19 = 0x00000000
Mem13: orig: Mem0
    def:  Mem13 = PHI((Mem11, l2), (Mem10, ge3))
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
    uses: dwLoc0C_23 = PHI((dwLoc0C_22, l2), (dwLoc0C_21, ge3))
dwLoc0C_22: orig: dwLoc0C
    def:  dwLoc0C_22 = r1
    uses: dwLoc0C_23 = PHI((dwLoc0C_22, l2), (dwLoc0C_21, ge3))
dwLoc0C_23: orig: dwLoc0C
    def:  dwLoc0C_23 = PHI((dwLoc0C_22, l2), (dwLoc0C_21, ge3))
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
	dwLoc0C_23 = PHI((dwLoc0C_22, l2), (dwLoc0C_21, ge3))
	Mem13 = PHI((Mem11, l2), (Mem10, ge3))
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
          call r3 (retsize: 4;)	uses: r1:r1,r3:r3	defs: r1:r1_6,r2:r2_7,r3:r3_8
r2_2: orig: r2
    def:  r2_2 = 0x00000010
r3:r3
    def:  def r3
    uses: call r3 (retsize: 4;)	uses: r1:r1,r3:r3	defs: r1:r1_6,r2:r2_7,r3:r3_8
          call r3 (retsize: 4;)	uses: r1:r1,r3:r3	defs: r1:r1_6,r2:r2_7,r3:r3_8
r1_6: orig: r1
    def:  call r3 (retsize: 4;)	uses: r1:r1,r3:r3	defs: r1:r1_6,r2:r2_7,r3:r3_8
    uses: use r1_6
r2_7: orig: r2
    def:  call r3 (retsize: 4;)	uses: r1:r1,r3:r3	defs: r1:r1_6,r2:r2_7,r3:r3_8
    uses: use r2_7
r3_8: orig: r3
    def:  call r3 (retsize: 4;)	uses: r1:r1,r3:r3	defs: r1:r1_6,r2:r2_7,r3:r3_8
    uses: use r3_8
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	def r3
	// succ:  l1
l1:
	branch r1 true
	// succ:  l2 true
l2:
	r2_2 = 0x00000010
	// succ:  true
true:
	call r3 (retsize: 4;)
		uses: r1:r1,r3:r3
		defs: r1:r1_6,r2:r2_7,r3:r3_8
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_6
	use r2_7
	use r3_8
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
          call proc1 (retsize: 0;)	uses: r2:r2_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_15,Stack +0008:dwArg04	defs: r2:r2_11,r3:r3_10,r63:r63_9
r63_2: orig: r63
    def:  r63_2 = fp
    uses: r63_19 = PHI((r63_13, m0Induction), (r63_2, m1Base))
Mem0:Mem
    def:  def Mem0
    uses: r2_6 = Mem0[r2_4 + 0x00000004:word32]
r2_4: orig: r2
    def:  r2_4 = dwArg04
    uses: branch r2_4 == 0x00000000 m1Base
          r2_6 = Mem0[r2_4 + 0x00000004:word32]
r2_5: orig: r2
    def:  r2_5 = 0x00000000
    uses: r2_16 = PHI((r2_12, m0Induction), (r2_5, m1Base))
r2_6: orig: r2
    def:  r2_6 = Mem0[r2_4 + 0x00000004:word32]
    uses: dwLoc04_15 = r2_6
          call proc1 (retsize: 0;)	uses: r2:r2_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_15,Stack +0008:dwArg04	defs: r2:r2_11,r3:r3_10,r63:r63_9
r63_7: orig: r63
    def:  r63_7 = fp - 0x00000004
Mem8: orig: Mem0
r63_9: orig: r63
    def:  call proc1 (retsize: 0;)	uses: r2:r2_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_15,Stack +0008:dwArg04	defs: r2:r2_11,r3:r3_10,r63:r63_9
    uses: r63_13 = r63_9 + 0x00000004
r3_10: orig: r3
    def:  call proc1 (retsize: 0;)	uses: r2:r2_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_15,Stack +0008:dwArg04	defs: r2:r2_11,r3:r3_10,r63:r63_9
    uses: r3_17 = PHI((r3_10, m0Induction), (r3, m1Base))
r2_11: orig: r2
    def:  call proc1 (retsize: 0;)	uses: r2:r2_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_15,Stack +0008:dwArg04	defs: r2:r2_11,r3:r3_10,r63:r63_9
    uses: r2_12 = r2_11 + 0x00000001
r2_12: orig: r2
    def:  r2_12 = r2_11 + 0x00000001
    uses: r2_16 = PHI((r2_12, m0Induction), (r2_5, m1Base))
r63_13: orig: r63
    def:  r63_13 = r63_9 + 0x00000004
    uses: r63_19 = PHI((r63_13, m0Induction), (r63_2, m1Base))
dwArg04:Stack +0004
    def:  def dwArg04
    uses: r2_4 = dwArg04
          call proc1 (retsize: 0;)	uses: r2:r2_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_15,Stack +0008:dwArg04	defs: r2:r2_11,r3:r3_10,r63:r63_9
dwLoc04_15: orig: dwLoc04
    def:  dwLoc04_15 = r2_6
    uses: call proc1 (retsize: 0;)	uses: r2:r2_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_15,Stack +0008:dwArg04	defs: r2:r2_11,r3:r3_10,r63:r63_9
r2_16: orig: r2
    def:  r2_16 = PHI((r2_12, m0Induction), (r2_5, m1Base))
    uses: use r2_16
r3_17: orig: r3
    def:  r3_17 = PHI((r3_10, m0Induction), (r3, m1Base))
    uses: use r3_17
r3:r3
    def:  def r3
    uses: r3_17 = PHI((r3_10, m0Induction), (r3, m1Base))
r63_19: orig: r63
    def:  r63_19 = PHI((r63_13, m0Induction), (r63_2, m1Base))
    uses: use r63_19
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def Mem0
	def dwArg04
	def r3
	// succ:  l1
l1:
	r63_2 = fp
	r2_4 = dwArg04
	branch r2_4 == 0x00000000 m1Base
	// succ:  m0Induction m1Base
m0Induction:
	r2_6 = Mem0[r2_4 + 0x00000004:word32]
	r63_7 = fp - 0x00000004
	dwLoc04_15 = r2_6
	call proc1 (retsize: 0;)
		uses: r2:r2_6,r63:fp - 0x00000004,Stack +0000:dwLoc04_15,Stack +0008:dwArg04
		defs: r2:r2_11,r3:r3_10,r63:r63_9
	r2_12 = r2_11 + 0x00000001
	r63_13 = r63_9 + 0x00000004
	goto m2Done
	// succ:  m2Done
m1Base:
	r2_5 = 0x00000000
	// succ:  m2Done
m2Done:
	r63_19 = PHI((r63_13, m0Induction), (r63_2, m1Base))
	r3_17 = PHI((r3_10, m0Induction), (r3, m1Base))
	r2_16 = PHI((r2_12, m0Induction), (r2_5, m1Base))
	return
	// succ:  proc1_exit
proc1_exit:
	use r2_16
	use r3_17
	use r63_19
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r3 = m.Register((RegisterStorage)this.r3.Storage);
                var r2 = m.Register((RegisterStorage)this.r2.Storage);
                m.Assign(sp, m.Frame.FramePointer);   // Establish frame.
                m.Assign(r2, m.Mem32(m.IAdd(sp, 4)));
                m.BranchIf(m.Eq0(r2), "m1Base");

                m.Label("m0Induction");
                m.Assign(r2, m.Mem32(m.IAdd(r2, 4)));
                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r2);
                m.Call(m.Procedure, 0);
                m.Assign(r2, m.IAdd(r2, 1));
                m.Assign(sp, m.IAdd(sp, 4));

                m.Goto("m2Done");

                m.Label("m1Base");
                m.Assign(r2, 0);

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
    uses: b_3 = PHI((b, l1), (b_2, m_1))
b_3: orig: b
    def:  b_3 = PHI((b, l1), (b_2, m_1))
    uses: return b_3
b:b
    def:  def b
    uses: b_3 = PHI((b, l1), (b_2, m_1))
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
	b_3 = PHI((b, l1), (b_2, m_1))
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
    uses: ecx_24_8_6 = SLICE(ecx_2, word24, 8) (alias)
ecx_3: orig: ecx
    def:  ecx_3 = 0x00000020
    uses: ecx_5 = PHI((ecx_7, mBranch1), (ecx_3, mBranch2))
cl_4: orig: cl
    def:  cl_4 = 0x2A
    uses: ecx_7 = SEQ(ecx_24_8_6, cl_4) (alias)
ecx_5: orig: ecx
    def:  ecx_5 = PHI((ecx_7, mBranch1), (ecx_3, mBranch2))
    uses: Mem8[0x00010232:word32] = ecx_5
ecx_24_8_6: orig: ecx_24_8
    def:  ecx_24_8_6 = SLICE(ecx_2, word24, 8) (alias)
    uses: ecx_7 = SEQ(ecx_24_8_6, cl_4) (alias)
ecx_7: orig: ecx
    def:  ecx_7 = SEQ(ecx_24_8_6, cl_4) (alias)
    uses: ecx_5 = PHI((ecx_7, mBranch1), (ecx_3, mBranch2))
Mem8: orig: Mem0
    def:  Mem8[0x00010232:word32] = ecx_5
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  l1
l1:
	ecx_2 = Mem0[0x00542300:word32]
	ecx_24_8_6 = SLICE(ecx_2, word24, 8) (alias)
	branch Mem0[0x00010042:bool] mBranch2
	// succ:  mBranch1 mBranch2
mBranch1:
	cl_4 = 0x2A
	ecx_7 = SEQ(ecx_24_8_6, cl_4) (alias)
	goto mCommon
	// succ:  mCommon
mBranch2:
	ecx_3 = 0x00000020
	// succ:  mCommon
mCommon:
	ecx_5 = PHI((ecx_7, mBranch1), (ecx_3, mBranch2))
	Mem8[0x00010232:word32] = ecx_5
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            Given_Architecture(new X86ArchitectureFlat32("x86-real-16"));
            RunTest(sExp, m =>
            {
                var ecx = m.Register(Registers.ecx);
                var cl = m.Register(Registers.cl);

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
    uses: dl_3 = SLICE(edx_2, byte, 0) (alias)
dl_3: orig: dl
    def:  dl_3 = SLICE(edx_2, byte, 0) (alias)
    uses: Mem4[0x00642300:byte] = dl_3
Mem4: orig: Mem0
    def:  Mem4[0x00642300:byte] = dl_3
    uses: edx_5 = Mem4[0x00543208:word32]
edx_5: orig: edx
    def:  edx_5 = Mem4[0x00543208:word32]
    uses: dl_6 = SLICE(edx_5, byte, 0) (alias)
dl_6: orig: dl
    def:  dl_6 = SLICE(edx_5, byte, 0) (alias)
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
	dl_3 = SLICE(edx_2, byte, 0) (alias)
	Mem4[0x00642300:byte] = dl_3
	edx_5 = Mem4[0x00543208:word32]
	dl_6 = SLICE(edx_5, byte, 0) (alias)
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
          bx_6 = SLICE(es_bx_4, word16, 0) (alias)
          bx_7 = Mem0[es_bx_4 + 0x0010:word32]
es_5: orig: es
    def:  es_5 = SLICE(es_bx_4, word16, 16) (alias)
    uses: use es_5
bx_6: orig: bx
    def:  bx_6 = SLICE(es_bx_4, word16, 0) (alias)
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
	bx_6 = SLICE(es_bx_4, word16, 0) (alias)
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
                var es_bx = m.Frame.EnsureSequence(PrimitiveType.SegPtr32, es.Storage, bx.Storage);

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
    uses: al_4 = SLICE(eax_3, byte, 0) (alias)
al_4: orig: al
    def:  al_4 = SLICE(eax_3, byte, 0) (alias)
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
	al_4 = SLICE(eax_3, byte, 0) (alias)
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
                addr, "COREDLL.DLL", "fnFoo", SymbolType.ExternalProcedure));
            dynamicLinker.Setup(i => i.ResolveToImportedValue(
                It.IsAny<Statement>(),
                It.Is<Constant>(c => c.ToUInt32() == 0x00031234)))
                .Returns(new ProcedureConstant(
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
    uses: eax_5 = PHI((eax_1, l1), (eax_7, l2Body))
ebx_2: orig: ebx
    def:  ebx_2 = PHI((ebx, l1), (ebx_8, l2Body))
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
    def:  eax_5 = PHI((eax_1, l1), (eax_7, l2Body))
    uses: eax_7 = eax_5 + Mem0[ebx_2:word32]
          return eax_5
eax_7: orig: eax
    def:  eax_7 = eax_5 + Mem0[ebx_2:word32]
    uses: eax_5 = PHI((eax_1, l1), (eax_7, l2Body))
ebx_8: orig: ebx
    def:  ebx_8 = Mem0[ebx_2 + 0x00000004:word32]
    uses: ebx_2 = PHI((ebx, l1), (ebx_8, l2Body))
ebx:ebx
    def:  def ebx
    uses: ebx_2 = PHI((ebx, l1), (ebx_8, l2Body))
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
	eax_5 = PHI((eax_1, l1), (eax_7, l2Body))
	ebx_2 = PHI((ebx, l1), (ebx_8, l2Body))
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
    uses: ch_6 = SLICE(es_cx_4, byte, 8) (alias)
          es_8 = SLICE(es_cx_4, selector, 16) (alias)
cl_5: orig: cl
    def:  cl_5 = 0x2D
    uses: cx_7 = SEQ(ch_6, cl_5) (alias)
ch_6: orig: ch
    def:  ch_6 = SLICE(es_cx_4, byte, 8) (alias)
    uses: cx_7 = SEQ(ch_6, cl_5) (alias)
cx_7: orig: cx
    def:  cx_7 = SEQ(ch_6, cl_5) (alias)
    uses: use cx_7
es_8: orig: es
    def:  es_8 = SLICE(es_cx_4, selector, 16) (alias)
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
	ch_6 = SLICE(es_cx_4, byte, 8) (alias)
	es_8 = SLICE(es_cx_4, selector, 16) (alias)
	cl_5 = 0x2D
	cx_7 = SEQ(ch_6, cl_5) (alias)
	return
	// succ:  proc1_exit
proc1_exit:
	use cx_7
	use es_8
======
";
            #endregion

            Given_Architecture(new X86ArchitectureFlat32("x86-real-16"));

            RunTest_FrameAccesses(sExp, m =>
            {
                var es = m.Register(Registers.es);
                var cx = m.Register(Registers.cx);
                var cl = m.Register(Registers.cl);
                var es_cx = m.Frame.EnsureSequence(PrimitiveType.SegPtr32, es.Storage, cx.Storage);

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
    def:  r1_1 = PHI((r1, proc1_entry), (r1_4, m1notdone))
    uses: branch r1_1 == 0x00000000 m3done
          r1_4 = r1_1 + Mem0[r2:word32]
          use r1_1
r1_4: orig: r1
    def:  r1_4 = r1_1 + Mem0[r2:word32]
    uses: r1_1 = PHI((r1, proc1_entry), (r1_4, m1notdone))
r1:r1
    def:  def r1
    uses: r1_1 = PHI((r1, proc1_entry), (r1_4, m1notdone))
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
	r1_1 = PHI((r1, proc1_entry), (r1_4, m1notdone))
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
    uses: bx_4 = SEQ(bh_3, bl_2) (alias)
bh_3: orig: bh
    def:  bh_3 = 0x00
    uses: bx_4 = SEQ(bh_3, bl_2) (alias)
bx_4: orig: bx
    def:  bx_4 = SEQ(bh_3, bl_2) (alias)
    uses: Mem5[0x1236:word16] = bx_4
Mem5: orig: Mem0
    def:  Mem5[0x1236:word16] = bx_4
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  m0
m0:
	bl_2 = Mem0[0x1234:word16]
	// succ:  m1
m1:
	bh_3 = 0x00
	bx_4 = SEQ(bh_3, bl_2) (alias)
	Mem5[0x1236:word16] = bx_4
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion
            Given_Architecture(new X86ArchitectureFlat32("x86-real-16"));
            RunTest(sExp, m =>
            {
                var bx = m.Frame.EnsureRegister(Registers.bx);
                var bh = m.Frame.EnsureRegister(Registers.bh);
                var bl = m.Frame.EnsureRegister(Registers.bl);

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
    uses: bl_2 = Mem0[0x1234:byte]
bl_2: orig: bl
    def:  bl_2 = Mem0[0x1234:byte]
    uses: branch bl_2 > 3 m2
          bx_4 = (uint16) (uint8) bl_2 (alias)
          bx_8 = SEQ(bh, bl_2) (alias)
bh_3: orig: bh
    def:  bh_3 = 0x00
bx_4: orig: bx
    def:  bx_4 = (uint16) (uint8) bl_2 (alias)
    uses: Mem5[0x1236:word16] = bx_4
          bx_6 = PHI((bx_8, m0), (bx_4, m1))
Mem5: orig: Mem0
    def:  Mem5[0x1236:word16] = bx_4
bx_6: orig: bx
    def:  bx_6 = PHI((bx_8, m0), (bx_4, m1))
    uses: use bx_6
bh:bh
    def:  def bh
    uses: bx_8 = SEQ(bh, bl_2) (alias)
bx_8: orig: bx
    def:  bx_8 = SEQ(bh, bl_2) (alias)
    uses: bx_6 = PHI((bx_8, m0), (bx_4, m1))
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	def bh
	// succ:  m0
m0:
	bl_2 = Mem0[0x1234:byte]
	bx_8 = SEQ(bh, bl_2) (alias)
	branch bl_2 > 3 m2
	// succ:  m1 m2
m1:
	bh_3 = 0x00
	bx_4 = (uint16) (uint8) bl_2 (alias)
	Mem5[0x1236:word16] = bx_4
	// succ:  m2
m2:
	bx_6 = PHI((bx_8, m0), (bx_4, m1))
	return
	// succ:  proc1_exit
proc1_exit:
	use bx_6
======
";
            #endregion

            Given_Architecture(new X86ArchitectureFlat32("x86-real-16"));

            RunTest_FrameAccesses(sExp, m =>
            {
                var bx = m.Register(Registers.bx);
                var bh = m.Register(Registers.bh);
                var bl = m.Register(Registers.bl);

                m.Label("m0");
                m.Assign(bl, m.Mem8(m.Word16(0x1234)));
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
            #region Expected
@"si:si
    def:  def si
    uses: bl_3 = Mem0[si:byte]
Mem0:Mem
    def:  def Mem0
    uses: bl_3 = Mem0[si:byte]
bl_3: orig: bl
    def:  bl_3 = Mem0[si:byte]
    uses: SCZO_4 = cond(bl_3 - 0x02)
          bx_6 = SEQ(bh_5, bl_3) (alias)
SCZO_4: orig: SCZO
    def:  SCZO_4 = cond(bl_3 - 0x02)
    uses: branch Test(UGT,SCZO_4) m2
bh_5: orig: bh
    def:  bh_5 = 0x00
    uses: bx_6 = SEQ(bh_5, bl_3) (alias)
bx_6: orig: bx
    def:  bx_6 = SEQ(bh_5, bl_3) (alias)
    uses: bx_7 = bx_6 + bx_6
          bx_7 = bx_6 + bx_6
bx_7: orig: bx
    def:  bx_7 = bx_6 + bx_6
    uses: Mem8[bx_7:word16] = 0x0000
Mem8: orig: Mem0
    def:  Mem8[bx_7:word16] = 0x0000
// proc1
// Return size: 0
define proc1
proc1_entry:
	def si
	def Mem0
	// succ:  m0
m0:
	bl_3 = Mem0[si:byte]
	SCZO_4 = cond(bl_3 - 0x02)
	branch Test(UGT,SCZO_4) m2
	// succ:  m1 m2
m1:
	bh_5 = 0x00
	bx_6 = SEQ(bh_5, bl_3) (alias)
	bx_7 = bx_6 + bx_6
	Mem8[bx_7:word16] = 0x0000
	// succ:  m2
m2:
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            Given_Architecture(new X86ArchitectureFlat32("x86-real-16"));
            RunTest(sExp, m =>
            {
                var bl = m.Register(Registers.bl);
                var bh = m.Register(Registers.bh);
                var bx = m.Register(Registers.bx);
                var si = m.Register(Registers.si);
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
    uses: r1_5 = SLICE(r2_r1_2, word32, 0) (alias)
r2_3: orig: r2
    def:  r2_3 = SLICE(r1 *s 1431655765, word32, 32) (alias)
    uses: Mem4[0x00040000:word32] = r2_3
          use r2_3
Mem4: orig: Mem0
    def:  Mem4[0x00040000:word32] = r2_3
r1_5: orig: r1
    def:  r1_5 = SLICE(r2_r1_2, word32, 0) (alias)
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
	r1_5 = SLICE(r2_r1_2, word32, 0) (alias)
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
                var r2_r1 = m.Frame.EnsureSequence(PrimitiveType.Word64, r2.Storage, r1.Storage);

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
    def:  Mem1 = PHI((Mem0, proc1_entry), (Mem3, m3))
    uses: branch Mem1[0x00004010:bool] m4
          branch Mem1[0x00004011:bool] m4
r1_2: orig: r1
    def:  r1_2 = 0x00000003
Mem3: orig: Mem0
    def:  Mem3[0x00004020:bool] = true
    uses: Mem1 = PHI((Mem0, proc1_entry), (Mem3, m3))
Mem0:Mem
    def:  def Mem0
    uses: Mem1 = PHI((Mem0, proc1_entry), (Mem3, m3))
// proc1
// Return size: 0
define proc1
proc1_entry:
	def Mem0
	// succ:  m1
m1:
	Mem1 = PHI((Mem0, proc1_entry), (Mem3, m3))
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
          bl_9 = SLICE(bx_3, byte, 0) (alias)
al_5: orig: al
    def:  al_5 = bh_8
    uses: return al_5
bx_7: orig: bx
    def:  bx_7 = SEQ(bh_8, bl_9) (alias)
    uses: branch bx_7 >= 0x0000 m0
bh_8: orig: bh
    def:  bh_8 = SLICE(bx_3, byte, 8) (alias)
    uses: al_5 = bh_8
          bx_7 = SEQ(bh_8, bl_9) (alias)
bl_9: orig: bl
    def:  bl_9 = SLICE(bx_3, byte, 0) (alias)
    uses: bx_7 = SEQ(bh_8, bl_9) (alias)
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
	bl_9 = SLICE(bx_3, byte, 0) (alias)
	// succ:  m0
m0:
	al_5 = bh_8
	bx_7 = SEQ(bh_8, bl_9) (alias)
	branch bx_7 >= 0x0000 m0
	// succ:  m1 m0
m1:
	return al_5
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            Given_Architecture(new X86ArchitectureFlat32("x86-real-16"));
            RunTest(sExp, m =>
            {
                var al = m.Register(Registers.al);
                var bl = m.Register(Registers.bl);
                var ah = m.Register(Registers.ah);
                var bh = m.Register(Registers.bh);
                var ax = m.Register(Registers.ax);
                var bx = m.Register(Registers.bx);

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
                var r3 = m.Register("r3");

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
                var r3 = m.Register("r3");

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
                var r3 = m.Register("r3");

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
                var r1 = m.Register("r1");

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
        // R = SEQ(W2, SLICE(W1))

        // W1:       [----------]
        // W2: [----------]
        // R:        [----------]
        // R = SEQ(SLICE(W2), SLICE(W1))


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
Mem5: orig: Mem0
    def:  Mem5[0x00123402:word16] = ax_4
    uses: al_6 = Mem5[0x00123404:byte]
          ah_7 = Mem5[0x00123405:byte]
al_6: orig: al
    def:  al_6 = Mem5[0x00123404:byte]
    uses: Mem8[0x00123406:byte] = al_6
          ax_10 = SEQ(ah_7, al_6) (alias)
ah_7: orig: ah
    def:  ah_7 = Mem5[0x00123405:byte]
    uses: Mem9[0x00123407:byte] = ah_7
          ax_10 = SEQ(ah_7, al_6) (alias)
Mem8: orig: Mem0
    def:  Mem8[0x00123406:byte] = al_6
Mem9: orig: Mem0
    def:  Mem9[0x00123407:byte] = ah_7
ax_10: orig: ax
    def:  ax_10 = SEQ(ah_7, al_6) (alias)
    uses: use ax_10
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
	ah_7 = Mem5[0x00123405:byte]
	Mem8[0x00123406:byte] = al_6
	Mem9[0x00123407:byte] = ah_7
	ax_10 = SEQ(ah_7, al_6) (alias)
	return
	// succ:  proc1_exit
proc1_exit:
	use ax_10
	use r63_2
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var ax = m.Frame.EnsureRegister(new RegisterStorage("ax", 0, 0, PrimitiveType.Word16));
                var ah = m.Frame.EnsureRegister(new RegisterStorage("ah", 0, 8, PrimitiveType.Byte));
                var al = m.Frame.EnsureRegister(new RegisterStorage("al", 0, 0, PrimitiveType.Byte));
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
    uses: eax_24_8_13 = SLICE(eax_1, word24, 8) (alias)
Mem2: orig: Mem0
    def:  Mem2[0x00123400:word32] = 0x00000004
    uses: al_3 = Mem2[0x00123408:byte]
al_3: orig: al
    def:  al_3 = Mem2[0x00123408:byte]
    uses: SCZ_4 = cond(al_3 - 0x30)
          SCZ_5 = cond(al_3 - 0x39)
SCZ_4: orig: SCZ
    def:  SCZ_4 = cond(al_3 - 0x30)
    uses: branch Test(LT,SCZ_4) m4_not_number
          C_11 = SLICE(SCZ_4, bool, 1) (alias)
          S_20 = SLICE(SCZ_4, bool, 0) (alias)
          Z_24 = SLICE(SCZ_4, bool, 2) (alias)
SCZ_5: orig: SCZ
    def:  SCZ_5 = cond(al_3 - 0x39)
    uses: branch Test(GT,SCZ_5) m4_not_number
          C_9 = SLICE(SCZ_5, bool, 1) (alias)
          S_18 = SLICE(SCZ_5, bool, 0) (alias)
          Z_22 = SLICE(SCZ_5, bool, 2) (alias)
al_6: orig: al
    def:  al_6 = 0x00
    uses: eax_16 = SEQ(eax_24_8_13, al_6) (alias)
al_7: orig: al
    def:  al_7 = 0x01
    uses: eax_14 = SEQ(eax_24_8_13, al_7) (alias)
C_8: orig: C
    def:  C_8 = PHI((C_9, m2_number), (C_10, m4_not_number))
    uses: use C_8
C_9: orig: C
    def:  C_9 = SLICE(SCZ_5, bool, 1) (alias)
    uses: C_10 = PHI((C_11, l1), (C_9, m1_maybe_number))
          C_8 = PHI((C_9, m2_number), (C_10, m4_not_number))
C_10: orig: C
    def:  C_10 = PHI((C_11, l1), (C_9, m1_maybe_number))
    uses: C_8 = PHI((C_9, m2_number), (C_10, m4_not_number))
C_11: orig: C
    def:  C_11 = SLICE(SCZ_4, bool, 1) (alias)
    uses: C_10 = PHI((C_11, l1), (C_9, m1_maybe_number))
eax_12: orig: eax
    def:  eax_12 = PHI((eax_14, m2_number), (eax_16, m4_not_number))
    uses: use eax_12
eax_24_8_13: orig: eax_24_8
    def:  eax_24_8_13 = SLICE(eax_1, word24, 8) (alias)
    uses: eax_14 = SEQ(eax_24_8_13, al_7) (alias)
          eax_16 = SEQ(eax_24_8_13, al_6) (alias)
eax_14: orig: eax
    def:  eax_14 = SEQ(eax_24_8_13, al_7) (alias)
    uses: eax_12 = PHI((eax_14, m2_number), (eax_16, m4_not_number))
eax_16: orig: eax
    def:  eax_16 = SEQ(eax_24_8_13, al_6) (alias)
    uses: eax_12 = PHI((eax_14, m2_number), (eax_16, m4_not_number))
S_17: orig: S
    def:  S_17 = PHI((S_18, m2_number), (S_19, m4_not_number))
    uses: use S_17
S_18: orig: S
    def:  S_18 = SLICE(SCZ_5, bool, 0) (alias)
    uses: S_19 = PHI((S_20, l1), (S_18, m1_maybe_number))
          S_17 = PHI((S_18, m2_number), (S_19, m4_not_number))
S_19: orig: S
    def:  S_19 = PHI((S_20, l1), (S_18, m1_maybe_number))
    uses: S_17 = PHI((S_18, m2_number), (S_19, m4_not_number))
S_20: orig: S
    def:  S_20 = SLICE(SCZ_4, bool, 0) (alias)
    uses: S_19 = PHI((S_20, l1), (S_18, m1_maybe_number))
Z_21: orig: Z
    def:  Z_21 = PHI((Z_22, m2_number), (Z_23, m4_not_number))
    uses: use Z_21
Z_22: orig: Z
    def:  Z_22 = SLICE(SCZ_5, bool, 2) (alias)
    uses: Z_23 = PHI((Z_24, l1), (Z_22, m1_maybe_number))
          Z_21 = PHI((Z_22, m2_number), (Z_23, m4_not_number))
Z_23: orig: Z
    def:  Z_23 = PHI((Z_24, l1), (Z_22, m1_maybe_number))
    uses: Z_21 = PHI((Z_22, m2_number), (Z_23, m4_not_number))
Z_24: orig: Z
    def:  Z_24 = SLICE(SCZ_4, bool, 2) (alias)
    uses: Z_23 = PHI((Z_24, l1), (Z_22, m1_maybe_number))
// proc1
// Return size: 0
define proc1
proc1_entry:
	// succ:  l1
l1:
	eax_1 = 0x00000004
	eax_24_8_13 = SLICE(eax_1, word24, 8) (alias)
	Mem2[0x00123400:word32] = 0x00000004
	al_3 = Mem2[0x00123408:byte]
	SCZ_4 = cond(al_3 - 0x30)
	C_11 = SLICE(SCZ_4, bool, 1) (alias)
	S_20 = SLICE(SCZ_4, bool, 0) (alias)
	Z_24 = SLICE(SCZ_4, bool, 2) (alias)
	branch Test(LT,SCZ_4) m4_not_number
	// succ:  m1_maybe_number m4_not_number
m1_maybe_number:
	SCZ_5 = cond(al_3 - 0x39)
	C_9 = SLICE(SCZ_5, bool, 1) (alias)
	S_18 = SLICE(SCZ_5, bool, 0) (alias)
	Z_22 = SLICE(SCZ_5, bool, 2) (alias)
	branch Test(GT,SCZ_5) m4_not_number
	// succ:  m2_number m4_not_number
m2_number:
	al_7 = 0x01
	eax_14 = SEQ(eax_24_8_13, al_7) (alias)
	return
	// succ:  proc1_exit
m4_not_number:
	Z_23 = PHI((Z_24, l1), (Z_22, m1_maybe_number))
	S_19 = PHI((S_20, l1), (S_18, m1_maybe_number))
	C_10 = PHI((C_11, l1), (C_9, m1_maybe_number))
	al_6 = 0x00
	eax_16 = SEQ(eax_24_8_13, al_6) (alias)
	return
	// succ:  proc1_exit
proc1_exit:
	Z_21 = PHI((Z_22, m2_number), (Z_23, m4_not_number))
	S_17 = PHI((S_18, m2_number), (S_19, m4_not_number))
	eax_12 = PHI((eax_14, m2_number), (eax_16, m4_not_number))
	C_8 = PHI((C_9, m2_number), (C_10, m4_not_number))
	use C_8
	use eax_12
	use S_17
	use Z_21
======
";
            #endregion
            Given_Architecture(new X86ArchitectureFlat32("x86-real-16"));

            RunTest_FrameAccesses(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var eax = m.Register(Registers.eax);
                var al = m.Register(Registers.al);
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
	a_8 = PHI((a_7, check_failed), (a_2, head))
	Mem9[0x00005678:word32] = a_8
	return
head:
	b_4 = PHI((b_3, init), (b_6, loop))
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
	a_5 = PHI((a_4, init), (a_7, again))
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
	def r1
	r1_2 = r2
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

        /// <summary>
        /// The signature (which is canonical), specifies that the stack argument is byte-wide.
        /// However, since that byte is word-aligned, compilers commonly read the whole word.
        /// </summary>
        [Test]
        [Category(Categories.FailedTests)]
        public void SsaByteArg()
        {
            var proc = Given_Procedure("proc", m =>
            {
                var a = m.Reg32("a", 0);
                var fp = m.Frame.FramePointer;

                m.Label("body");
                m.Assign(a, m.Mem32(m.IAdd(fp, 4)));
                m.Assign(a, m.And(a, 0xFF));
                m.MStore(m.Word32(0x5678), a);
                m.Return();
            });
            proc.Signature = FunctionType.Action(
                new Identifier(
                    "byteArg",
                    PrimitiveType.Byte,
                    new StackArgumentStorage(4, PrimitiveType.Byte))
            );

            When_RunSsaTransform();
            When_RenameFrameAccesses();

            var expected =
            #region Expected
@"proc_entry:
	def fp
	def Mem0
	def byteArg
	def dwArg04
	dwArg04_8 = DPB(dwArg04, byteArg, 0)
body:
	a_3 = dwArg04_8
	a_4 = a_3 & 0x000000FF
	Mem5[0x00005678:word32] = a_4
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
            var arch = new FakeArchitecture();
            var topReg = arch.FpuStackRegister;
            var ST = arch.FpuStackBase;
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
	dwLoc04_9 = SLICE(qwLoc08_6, word32, 32) (alias)
	dwLoc08_7 = 0x00001234
	dwLoc08_8 = dwLoc08_7 + 0x00000001
	qwLoc08_10 = SEQ(dwLoc04_9, dwLoc08_8) (alias)
	Mem5[0x0000567C:word64] = qwLoc08_10
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
	Mem3 = PHI((Mem0, body), (Mem6, failed))
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

        [Test]
        public void SsaIfThenLoop_AddUsesToExitBlock()
        {
            var proc = Given_Procedure("proc", m =>
            {
                var a = m.Reg32("a", 0);

                m.Label("body");
                m.Assign(a, m.Mem32(m.Word32(0x1234)));
                m.BranchIf(m.Fn(m.Mem32(m.Word32(0x1))), "return");

                m.Label("head");
                m.BranchIf(m.Fn(m.Mem32(m.Word32(0x2))), "return");

                m.Label("loopBody");
                m.MStore(m.Word32(0x5002), m.Word32(0x2222));
                m.Goto("head");

                m.Label("return");
                m.MStore(m.Word32(0x567C), a);
                m.Assign(a, m.Mem32(m.Word32(0x5002)));
                m.Return();
            });

            When_RunSsaTransform();
            When_AddUsesToExitBlock();

            var expected =
            #region Expected
@"proc_entry:
	def Mem0
body:
	a_2 = Mem0[0x00001234:word32]
	branch Mem0[0x00000001:word32]() return
head:
	Mem3 = PHI((Mem0, body), (Mem8, loopBody))
	branch Mem3[0x00000002:word32]() return
loopBody:
	Mem8[0x00005002:word32] = 0x00002222
	goto head
return:
	Mem6[0x0000567C:word32] = a_2
	a_7 = Mem6[0x00005002:word32]
	return
proc_exit:
	use a_7
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        public void Ssa96BitStackLocal()
        {
            Given_Architecture(new FakeArchitecture
            {
                Test_Endianness = EndianServices.Big
            });
            var proc = Given_Procedure(nameof(Ssa96BitStackLocal), m =>
            {
                var r1 = m.Reg32("r1", 1);
                var fp0 = m.Register(new RegisterStorage("fp0", 16, 0, PrimitiveType.CreateWord(96)));
                var sp = m.Register(m.Architecture.StackRegister);
                var fp = m.Frame.FramePointer;
                m.Assign(sp, fp);
                m.Assign(sp, m.ISubS(fp, 12));  // make space on stack
                m.Assign(r1, m.Mem32(m.Word32(0x00123400)));
                m.MStore(m.ISubS(fp, 12), r1);
                m.Assign(r1, m.Mem32(m.Word32(0x00123404)));
                m.MStore(m.ISubS(fp, 8), r1);
                m.Assign(r1, m.Mem32(m.Word32(0x00123408)));
                m.MStore(m.ISubS(fp, 4), r1);

                m.Assign(fp0, m.Mem(fp0.DataType, m.ISubS(fp, 12)));
                m.Return();
            });

            When_RunSsaTransform();
            When_RenameFrameAccesses();

            var expected =
            #region Expected
@"Ssa96BitStackLocal_entry:
	def fp
	def Mem0
l1:
	r63_2 = fp
	r63_3 = fp - 12
	r1_5 = Mem0[0x00123400:word32]
	dwLoc0C_12 = r1_5
	r1_7 = Mem6[0x00123404:word32]
	dwLoc08_13 = r1_7
	r1_9 = Mem8[0x00123408:word32]
	dwLoc04_14 = r1_9
	nLoc0C_15 = SEQ(dwLoc0C_12, dwLoc08_13, dwLoc04_14) (alias)
	fp0_11 = nLoc0C_15
	return
Ssa96BitStackLocal_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        public void SsaDpb()
        {
            Given_Architecture(new X86ArchitectureFlat32("x86-real-16"));
            var proc = Given_Procedure(nameof(SsaDpb), m =>
            {
                var bx = m.Register(Registers.bx);
                var bl = m.Register(Registers.bl);
                var bh = m.Register(Registers.bh);

                m.Assign(bl, m.Mem8(Address.Ptr16(0x1234)));
                m.Assign(bh, m.Mem8(Address.Ptr16(0x1235)));
                m.MStore(bx, m.Word16(0x0042)); 
                m.Return();
            });

            When_RunSsaTransform();

            var expected =
            #region Expected
@"SsaDpb_entry:
	def Mem0
l1:
	bl_2 = Mem0[0x1234:byte]
	bh_3 = Mem0[0x1235:byte]
	bx_4 = SEQ(bh_3, bl_2) (alias)
	Mem5[bx_4:word16] = 0x0042
	return
SsaDpb_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void SsaOverlappedStackIntervals()
        {
            var proc = Given_Procedure(nameof(SsaOverlappedStackIntervals), m =>
            {
                var fp = m.Frame.FramePointer;
                m.MStore(m.ISubS(fp, 8), m.Word64(0x1234567800000000));
                m.MStore(m.Word32(0xAB), m.Mem64(m.ISubS(fp, 8)));
                m.MStore(m.ISubS(fp, 4), m.Word32(0x12340000));
                m.MStore(m.Word32(0xCD), m.Mem32(m.ISubS(fp, 4)));
                m.Return();
            });

            When_RunSsaTransform();
            When_RenameFrameAccesses();

            var expected =
            #region Expected
@"SsaOverlappedStackIntervals_entry:
	def fp
l1:
	qwLoc08_6 = 0x1234567800000000
	Mem3[0x000000AB:word64] = qwLoc08_6
	dwLoc04_7 = 0x12340000
	Mem5[0x000000CD:word32] = dwLoc04_7
	return
SsaOverlappedStackIntervals_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void SsaLocalStackSlice()
        {
            var proc = Given_Procedure(nameof(SsaLocalStackSlice), m =>
            {
                var fp = m.Frame.FramePointer;
                var b = m.Reg8("byte", 1);
                m.Label("b_init");
                m.BranchIf(m.Eq(b, 0), "b1");

                m.Label("b0");
                m.Assign(b, m.Byte(0x0));
                m.Goto("finalize");

                m.Label("b1");
                m.Assign(b, m.Byte(0x1));

                m.Label("finalize");
                m.MStore(m.ISubS(fp, 4), b);
                m.Assign(b, m.And(m.Mem32(m.ISubS(fp, 4)), 0xFF));
                m.Return();
            });

            When_RunSsaTransform();
            When_RenameFrameAccesses();

            var expected =
            #region Expected
@"SsaLocalStackSlice_entry:
	def byte
	def fp
	def nLoc03
b_init:
	branch byte == 0x00 b1
b0:
	byte_3 = 0x00
	goto finalize
b1:
	byte_2 = 0x01
finalize:
	byte_4 = PHI((byte_3, b0), (byte_2, b1))
	bLoc04_9 = byte_4
	dwLoc04_12 = SEQ(nLoc03, bLoc04_9) (alias)
	byte_8 = dwLoc04_12 & 0x000000FF
	return
SsaLocalStackSlice_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void SsaLocalStackCommonSequence()
        {
            var proc = Given_Procedure(nameof(SsaLocalStackCommonSequence), m =>
            {
                var fp = m.Frame.FramePointer;
                var a = m.Reg16("a", 1);
                var b = m.Reg16("b", 2);
                m.Label("b_init");
                m.MStore(m.ISubS(fp, 2), a);
                m.MStore(m.ISubS(fp, 4), b);
                m.BranchIf(m.Eq(b, 0), "b1");

                m.Label("b0");
                m.MStore(m.Word32(0xA), m.Mem32(m.ISubS(fp, 4)));
                m.Goto("finalize");

                m.Label("b1");
                m.MStore(m.Word32(0xB), m.Mem32(m.ISubS(fp, 4)));

                m.Label("finalize");
                m.Return();
            });

            When_RunSsaTransform();
            When_RenameFrameAccesses();

            var expected =
            #region Expected
@"SsaLocalStackCommonSequence_entry:
	def a
	def fp
	def b
b_init:
	wLoc02_8 = a
	wLoc04_9 = b
	dwLoc04_10 = SEQ(wLoc02_8, wLoc04_9) (alias)
	branch b == 0x0000 b1
b0:
	Mem7[0x0000000A:word32] = dwLoc04_10
	goto finalize
b1:
	Mem6[0x0000000B:word32] = dwLoc04_10
finalize:
	return
SsaLocalStackCommonSequence_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void SsaDoNotSearchImportedProcInFpuStack()
        {
            // ResolveToImportedValue always throws exception in X86 real mode
            dynamicLinker.Setup(
                i => i.ResolveToImportedValue(
                    It.IsAny<Statement>(),
                    It.IsAny<Constant>()))
                .Throws(
                new NotSupportedException(
                    "Must pass segment:offset to make a segmented address."));
            var proc = Given_Procedure(
                nameof(SsaDoNotSearchImportedProcInFpuStack),
                m =>
            {
                var a = m.Reg32("a", 1);
                var stStg = new RegisterStorage(
                    "FakeST", 2, 0, PrimitiveType.Word32);
                var st = new MemoryIdentifier(
                    stStg.Name, stStg.DataType, stStg);
                m.Label("init");
                m.Assign(a, m.Mem(st, PrimitiveType.Word32, m.Word32(0x1)));
                m.Return();
            });

            When_RunSsaTransform();

            var expected =
            #region Expected
@"SsaDoNotSearchImportedProcInFpuStack_entry:
	def FakeST
init:
	a_2 = FakeST[0x00000001:word32]
	return
SsaDoNotSearchImportedProcInFpuStack_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        private void MakeEndiannessCheck(ProcedureBuilder m)
        {
            var fp = m.Frame.FramePointer;
            var r0 = m.Reg32("r0", 0);
            var tmp = m.Frame.CreateTemporary(PrimitiveType.Byte);
            m.MStore(m.ISub(fp, 4), m.Word16(0x0001));
            m.Assign(tmp, m.Mem8(m.ISub(fp, 4)));
            m.Assign(r0, m.Dpb(r0, tmp, 0));
            m.Return();
        }

        [Test(Description = "GitHub issue #727: https://github.com/uxmal/reko/issues/727")]
        [Category(Categories.UnitTests)]
        public void SsaEndiannessCheck_LE()
        {
            Given_Architecture(new FakeArchitecture
            {
                Test_Endianness = EndianServices.Little
            });
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
Mem2: orig: Mem0
v3_3: orig: v3
    def:  v3_3 = bLoc04_7
    uses: r0_5 = DPB(r0, v3_3, 0)
r0:r0
    def:  def r0
    uses: r0_5 = DPB(r0, v3_3, 0)
r0_5: orig: r0
    def:  r0_5 = DPB(r0, v3_3, 0)
    uses: use r0_5
wLoc04_6: orig: wLoc04
    def:  wLoc04_6 = 0x0001
    uses: bLoc04_7 = SLICE(wLoc04_6, byte, 0) (alias)
bLoc04_7: orig: bLoc04
    def:  bLoc04_7 = SLICE(wLoc04_6, byte, 0) (alias)
    uses: v3_3 = bLoc04_7
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def r0
	// succ:  l1
l1:
	wLoc04_6 = 0x0001
	bLoc04_7 = SLICE(wLoc04_6, byte, 0) (alias)
	v3_3 = bLoc04_7
	r0_5 = DPB(r0, v3_3, 0)
	return
	// succ:  proc1_exit
proc1_exit:
	use r0_5
======
";
            #endregion

            this.RunTest_FrameAccesses(sExp, MakeEndiannessCheck);
        }

        [Test(Description = "GitHub issue #727: https://github.com/uxmal/reko/issues/727")]
        [Category(Categories.UnitTests)]
        public void SsaEndiannessCheck_BE()
        {
            Given_Architecture(new FakeArchitecture
            {
                Test_Endianness = EndianServices.Big
            });

            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
Mem2: orig: Mem0
v3_3: orig: v3
    def:  v3_3 = bLoc04_7
    uses: r0_5 = DPB(r0, v3_3, 0)
r0:r0
    def:  def r0
    uses: r0_5 = DPB(r0, v3_3, 0)
r0_5: orig: r0
    def:  r0_5 = DPB(r0, v3_3, 0)
    uses: use r0_5
wLoc04_6: orig: wLoc04
    def:  wLoc04_6 = 0x0001
    uses: bLoc04_7 = SLICE(wLoc04_6, byte, 8) (alias)
bLoc04_7: orig: bLoc04
    def:  bLoc04_7 = SLICE(wLoc04_6, byte, 8) (alias)
    uses: v3_3 = bLoc04_7
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def r0
	// succ:  l1
l1:
	wLoc04_6 = 0x0001
	bLoc04_7 = SLICE(wLoc04_6, byte, 8) (alias)
	v3_3 = bLoc04_7
	r0_5 = DPB(r0, v3_3, 0)
	return
	// succ:  proc1_exit
proc1_exit:
	use r0_5
======
";
            #endregion

            RunTest_FrameAccesses(sExp, MakeEndiannessCheck);
        }

        [Test]
        public void SsaSequenceInALoop()
        {
            var proc = Given_Procedure(nameof(SsaSequenceInALoop), m =>
            {
                var r1 = m.Reg32("r1", 1);
                var f1 = m.Reg64("f1", 16);
                var fp0 = m.Register(new RegisterStorage("fp0", 16, 0, PrimitiveType.CreateWord(96)));
                var fp = m.Frame.FramePointer;
                m.MStore(m.ISubS(fp, 8), m.Word32(0x3FF00000));
                m.MStore(m.ISubS(fp, 4), m.Word32(0));
                m.Assign(r1, 0);

                m.Label("m0");
                m.Assign(f1, m.Mem64(m.ISubS(fp, 8)));
                m.Assign(f1, m.FMul(f1, m.Cast(PrimitiveType.Real64, r1)));
                m.MStore(m.ISubS(fp, 8), f1);
                m.Assign(r1, m.IAddS(r1, 1));
                m.BranchIf(m.Ne(r1, 10), "m0");

                m.Label("m1");
                m.Return();
            });

            When_RunSsaTransform();
            When_RenameFrameAccesses();

            var expected =
            #region Expected
@"SsaSequenceInALoop_entry:
	def fp
l1:
	dwLoc08_12 = 0x3FF00000
	dwLoc04_13 = 0x00000000
	r1_4 = 0x00000000
	qwLoc08_16 = SEQ(dwLoc04_13, dwLoc08_12) (alias)
m0:
	qwLoc08_14 = PHI((qwLoc08_16, l1), (qwLoc08_15, m0))
	r1_8 = PHI((r1_4, l1), (r1_11, m0))
	Mem6 = PHI((Mem3, l1), (Mem10, m0))
	f1_7 = qwLoc08_14
	f1_9 = f1_7 * (real64) r1_8
	qwLoc08_15 = f1_9
	r1_11 = r1_8 + 1
	branch r1_11 != 0x0000000A m0
m1:
	return
SsaSequenceInALoop_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        public void SsaRepeatedAliasedReadsFromStack()
        {
            var sExp =
@"fp:fp
    def:  def fp
    uses: r63_2 = fp
r63_2: orig: r63
    def:  r63_2 = fp
    uses: use r63_2
Mem0:Mem
    def:  def Mem0
r1_4: orig: r1
    def:  r1_4 = dwArg08
    uses: use r1_4
tmp1_5: orig: tmp1
    def:  tmp1_5 = wArg08_8
tmp2_6: orig: tmp2
    def:  tmp2_6 = wArg08_8
dwArg08:Stack +0008
    def:  def dwArg08
    uses: r1_4 = dwArg08
          wArg08_8 = SLICE(dwArg08, word16, 0) (alias)
wArg08_8: orig: wArg08
    def:  wArg08_8 = SLICE(dwArg08, word16, 0) (alias)
    uses: tmp1_5 = wArg08_8
          tmp2_6 = wArg08_8
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def Mem0
	def dwArg08
	wArg08_8 = SLICE(dwArg08, word16, 0) (alias)
	// succ:  l1
l1:
	r63_2 = fp
	r1_4 = dwArg08
	tmp1_5 = wArg08_8
	tmp2_6 = wArg08_8
	return
	// succ:  proc1_exit
proc1_exit:
	use r1_4
	use r63_2
======
";
            RunTest_FrameAccesses(sExp, m =>
            {
                var fp = m.Frame.FramePointer;
                var r1 = m.Reg32("r1", 1);
                var sp = m.Register(m.Architecture.StackRegister);
                var tmp1 = m.Temp(PrimitiveType.Word16, "tmp1");
                var tmp2 = m.Temp(PrimitiveType.Word16, "tmp2");
                m.Assign(sp, fp);
                m.Assign(r1, m.Mem32(m.IAdd(sp, 8)));
                m.Assign(tmp1, m.Mem16(m.IAdd(sp, 8)));
                m.Assign(tmp2, m.Mem16(m.IAdd(sp, 8)));
                m.Return();
            });
        }

        [Test]
        public void SsaAliasedArgumentRead()
        {
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
Mem0:Mem
    def:  def Mem0
si_3: orig: si
    def:  si_3 = wArg02
    uses: use si_3
es_bx_4: orig: es_bx
    def:  es_bx_4 = ptrArg02_7
    uses: bx_8 = SLICE(es_bx_4, word16, 0) (alias)
          es_9 = SLICE(es_bx_4, word16, 16) (alias)
wArg02:Stack +0002
    def:  def wArg02
    uses: si_3 = wArg02
          ptrArg02_7 = SEQ(wArg04, wArg02) (alias)
wArg04:Stack +0004
    def:  def wArg04
    uses: ptrArg02_7 = SEQ(wArg04, wArg02) (alias)
ptrArg02_7: orig: ptrArg02
    def:  ptrArg02_7 = SEQ(wArg04, wArg02) (alias)
    uses: es_bx_4 = ptrArg02_7
bx_8: orig: bx
    def:  bx_8 = SLICE(es_bx_4, word16, 0) (alias)
    uses: use bx_8
es_9: orig: es
    def:  es_9 = SLICE(es_bx_4, word16, 16) (alias)
    uses: use es_9
// proc1
// Return size: 0
define proc1
proc1_entry:
	def fp
	def Mem0
	def wArg02
	def wArg04
	// succ:  l1
l1:
	si_3 = wArg02
	ptrArg02_7 = SEQ(wArg04, wArg02) (alias)
	// succ:  m1
m1:
	es_bx_4 = ptrArg02_7
	bx_8 = SLICE(es_bx_4, word16, 0) (alias)
	es_9 = SLICE(es_bx_4, word16, 16) (alias)
	return
	// succ:  proc1_exit
proc1_exit:
	use bx_8
	use es_9
	use si_3
======
";
            #endregion

            RunTest_FrameAccesses(sExp, m =>
            {
                var fp = m.Frame.FramePointer;
                var si = m.Reg16("si", 6);
                var bx = m.Reg16("bx", 0);
                var es = m.Reg16("es", 12);
                var es_bx = m.Frame.EnsureSequence(PrimitiveType.SegPtr32, es.Storage, bx.Storage);

                m.Assign(si, m.Mem16(m.IAdd(fp, 2)));

                m.Label("m1");
                m.Assign(es_bx, m.Mem(PrimitiveType.SegPtr32, m.IAdd(fp, 2)));
                m.Return();
            });
        }

        // If the procedure has a valid signature, we bind all return statements using the 
        // storage of the return value in the signature.
        [Test]
        [Ignore("This approach doesn't seem to be working")]
        public void SsaReturnInstruction_ValidSignature()
        {
            var sExp =
            #region Expected
@"Mem0:Mem
    def:  def Mem0
    uses: al_2 = Mem0[0x00123400:byte]
al_2: orig: al
    def:  al_2 = Mem0[0x00123400:byte]
    uses: return al_2
// proc1
// Return size: 0
byte proc1()
proc1_entry:
	def Mem0
	// succ:  l1
l1:
	al_2 = Mem0[0x00123400:byte]
	return al_2
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                var al = m.Reg8("al", 0);
                m.Procedure.Signature = FunctionType.Func(
                    new Identifier("", PrimitiveType.Byte, al.Storage));

                m.Assign(al, m.Mem8(m.Word32(0x00123400)));
                m.Return();
            });
        }

        [Test]
        public void SsaIrreducibleRegion()
        {
            var sExp =
            #region Expected
@"r1:r1
    def:  def r1
    uses: branch r1 == 0x00000000 m2
          r1_2 = r1 (alias)
          r1_5 = r1 (alias)
r1_2: orig: r1
    def:  r1_2 = r1 (alias)
    uses: Mem4[r2_3:word32] = r1_2
r2_3: orig: r2
    def:  r2_3 = r2 (alias)
    uses: Mem4[r2_3:word32] = r1_2
Mem4: orig: Mem0
    def:  Mem4[r2_3:word32] = r1_2
r1_5: orig: r1
    def:  r1_5 = r1 (alias)
    uses: Mem8[r2_6:word32] = r1_5
r2_6: orig: r2
    def:  r2_6 = r2 (alias)
    uses: Mem8[r2_6:word32] = r1_5
r2:r2
    def:  def r2
    uses: r2_3 = r2 (alias)
          r2_6 = r2 (alias)
Mem8: orig: Mem0
    def:  Mem8[r2_6:word32] = r1_5
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	def r2
	// succ:  l1
l1:
	branch r1 == 0x00000000 m2
	// succ:  m1 m2
m1:
	r2_3 = r2 (alias)
	r1_2 = r1 (alias)
	Mem4[r2_3:word32] = r1_2
	// succ:  m2
m2:
	r2_6 = r2 (alias)
	r1_5 = r1 (alias)
	Mem8[r2_6:word32] = r1_5
	goto m1
	// succ:  m1
proc1_exit:
======
";
            #endregion
            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.BranchIf(m.Eq0(r1), "m2");

                m.Label("m1");
                m.MStore(r2, r1);
                m.Goto("m2");

                m.Label("m2");
                m.MStore(r2, r1);
                m.Goto("m1");
            });
        }

        [Test]
        public void SsaHell_Registers()
        {
            var sExp =
            #region Expected
@"r2_1: orig: r2
    def:  r2_1 = 0x00123400
    uses: call r1 (retsize: 0;)	uses: r1:r1,r2:r2_1,r4:r4_2	defs: r2:r2_4
r4_2: orig: r4
    def:  r4_2 = 0x00BCDE00
    uses: call r1 (retsize: 0;)	uses: r1:r1,r2:r2_1,r4:r4_2	defs: r2:r2_4
r1:r1
    def:  def r1
    uses: call r1 (retsize: 0;)	uses: r1:r1,r2:r2_1,r4:r4_2	defs: r2:r2_4
          call r1 (retsize: 0;)	uses: r1:r1,r2:r2_1,r4:r4_2	defs: r2:r2_4
r2_4: orig: r2
    def:  call r1 (retsize: 0;)	uses: r1:r1,r2:r2_1,r4:r4_2	defs: r2:r2_4
// proc1
// Return size: 0
define proc1
proc1_entry:
	def r1
	// succ:  l1
l1:
	r2_1 = 0x00123400
	r4_2 = 0x00BCDE00
	call r1 (retsize: 0;)
		uses: r1:r1,r2:r2_1,r4:r4_2
		defs: r2:r2_4
	return
	// succ:  proc1_exit
proc1_exit:
======
";
            #endregion

            RunTest(sExp, m =>
            {
                m.Frame.EnsureIdentifier(r1.Storage);
                m.Frame.EnsureIdentifier(r2.Storage);
                m.Frame.EnsureIdentifier(r4.Storage);

                m.Assign(r2, m.Word32(0x00123400));
                m.Assign(r4, m.Word32(0x00BCDE00));
                m.Call(r1, 0);
                m.Return();
            });
        }

        [Test]
        public void Ssa_BigEndianSlicing_Parameter()
        {
            var sExp =
            #region Expected
@"fp:fp
    def:  def fp
Mem0:Mem
    def:  def Mem0
Mem3: orig: Mem0
    def:  Mem3[0x00123400:word32] = dwArg08_6
Mem4: orig: Mem0
    def:  Mem4[0x00123404:word32] = dwArg0C_7
rArg08:Stack +0008
    def:  def rArg08
    uses: dwArg08_6 = SLICE(rArg08, word32, 32)
          dwArg0C_7 = SLICE(rArg08, word32, 0)
dwArg08_6: orig: dwArg08
    def:  dwArg08_6 = SLICE(rArg08, word32, 32)
    uses: Mem3[0x00123400:word32] = dwArg08_6
dwArg0C_7: orig: dwArg0C
    def:  dwArg0C_7 = SLICE(rArg08, word32, 0)
    uses: Mem4[0x00123404:word32] = dwArg0C_7
// proc1
// Return size: 0
void proc1(real64 rArg08)
proc1_entry:
	def fp
	def Mem0
	def rArg08
	dwArg08_6 = SLICE(rArg08, word32, 32)
	dwArg0C_7 = SLICE(rArg08, word32, 0)
	// succ:  l1
l1:
	Mem3[0x00123400:word32] = dwArg08_6
	Mem4[0x00123404:word32] = dwArg0C_7
proc1_exit:
======
";
            #endregion 
            Given_BigEndianArchitecture();
            RunTest_FrameAccesses(sExp, m =>
            {
                m.Procedure.Signature = FunctionType.Action(
                    new Identifier("rArg08", PrimitiveType.Real64, new StackArgumentStorage(8, PrimitiveType.Word64)));
                // Slices the little-endian high word of rArg08
                m.MStore(m.Word32(0x00123400), m.Mem32(m.IAddS(m.Procedure.Frame.FramePointer, 8)));
                m.MStore(m.Word32(0x00123404), m.Mem32(m.IAddS(m.Procedure.Frame.FramePointer, 12)));
            });
        }
    }
}
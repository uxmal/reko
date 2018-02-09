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

namespace Reko.UnitTests.Analysis
{
    /// <summary>
    /// These tests are making sure that we can re-run SsaTransform
    /// on a procedure that already has been transformed once.
    /// </summary>
    [TestFixture]
    public class SsaTransformTests
    {
        private ProgramBuilder pb;
        private ProgramDataFlow programFlow;
        private Dictionary<Address, ImportReference> importReferences;

        [SetUp]
        public void Setup()
        {
            this.pb = new ProgramBuilder();
            this.programFlow = new ProgramDataFlow();
            this.importReferences = new Dictionary<Address, ImportReference>();
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
            var listener = new FakeDecompilerEventListener();
            var importResolver = new ImportResolver(
                project,
                this.pb.Program,
                listener);
            var arch = new FakeArchitecture();
            
            var platform = new FakePlatform(null, arch);

            // Register r1 is assumed to always be implicit when calling
            // another procedure.
            var implicitRegs = new HashSet<RegisterStorage>
            {
                arch.GetRegister(1)
            };
            Debug.Print("GetRegister(1) {0}", arch.GetRegister(1));
            this.pb.Program.Platform = platform;
            this.pb.Program.Platform = new FakePlatform(null, new FakeArchitecture());
            this.pb.Program.SegmentMap = new SegmentMap(
                Address.Ptr32(0x0000),
                new ImageSegment(
                    ".text",
                    Address.Ptr32(0), 
                    0x40000,
                    AccessMode.ReadWriteExecute));

            // Perform the initial transformation
            var ssa = new SsaTransform(programFlow, proc, importResolver, dg, implicitRegs);

            // Propagate values and simplify the results.
            // We hope the the sequence
            //   esp = fp - 4
            //   mov [esp-4],eax
            // will become
            //   esp_2 = fp - 4
            //   mov [fp - 8],eax

            var vp = new ValuePropagator(this.pb.Program.Architecture, ssa.SsaState, listener);
            vp.Transform();

            ssa.RenameFrameAccesses = true;
            ssa.AddUseInstructions = true;
            ssa.Transform();

            var writer = new StringWriter();
            proc.Write(false, writer);
            var sActual = writer.ToString();
            if (sActual != sExp)
                Debug.Print(sActual);
            Assert.AreEqual(sExp, sActual);
            ssa.SsaState.CheckUses(s => Assert.Fail(s));
        }

        private void RunTest2(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder(this.pb.Program.Architecture);
            builder(pb);
            var proc = pb.Procedure;

            var alias = new Aliases(proc, this.pb.Program.Architecture);
            alias.Transform();
            var ssa = new SsaTransform2();
            ssa.Transform(proc);

            var writer = new StringWriter();
            proc.Write(false, writer);
            var sActual = writer.ToString();
            if (sActual != sExp)
                Debug.Print(sActual);
            Assert.AreEqual(sExp, sActual);
        }

        [Test]
        public void SsarSimple()
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
	use r2
";
            RunTest(sExp, m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                m.Assign(r1, m.IAdd(r1, r2));
                m.Return();
            });
        }

        [Test]
        public void SsarStackLocals()
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
	Mem11[0x00010008:word32] = r1_10
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use dwLoc04_12
	use dwLoc08_13
	use fp
	use Mem11
	use r1_10
	use r2_9
	use r63_5
";
            RunTest(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r1);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r2);
                m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
                m.Assign(r2, m.Mem32(sp));
                m.Assign(r1, m.IAdd(r1, r2));
                m.Store(m.Word32(0x010008), r1);
                m.Return();
            });
        }

        [Test]
        public void SsarDiamond()
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
	r1_7 = PHI(r1_10, r1_11)
	bp_8 = dwLoc04_12
	r63_9 = fp
	return
	// succ:  ProcedureBuilder_exit
ge3:
	r1_11 = 0x00000001
	goto done
	// succ:  done
l1:
	r63_1 = fp
	r63_2 = fp - 0x00000004
	dwLoc04_12 = bp
	bp_5 = fp - 0x00000004
	CZS_6 = wArg04 - 0x0003
	branch Test(GE,CZS_6) ge3
	// succ:  l2 ge3
l2:
	r1_10 = 0x00000000
	goto done
	// succ:  done
ProcedureBuilder_exit:
	use bp_8
	use CZS_6
	use dwLoc04_12
	use fp
	use r1_7
	use r63_9
";
            RunTest(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1", 1);
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var cr = m.Frame.EnsureFlagGroup(flags, 0x3, "CZS", PrimitiveType.Byte);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, bp);
                m.Assign(bp, sp);
                m.Assign(cr, m.ISub(m.Mem16(m.IAdd(bp, 8)), 0x3));
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
        public void SsarDiamondFrame()
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
	wArg04_16 = PHI(wArg04_17, wArg04_18)
	r1_7 = PHI(r1, r1_13)
	bp_8 = dwLoc04_14
	r63_9 = fp
	return
	// succ:  ProcedureBuilder_exit
ge3:
	wArg04_18 = -3
	r1_13 = 0x00000001
	goto done
	// succ:  done
l1:
	r63_1 = fp
	r63_2 = fp - 0x00000004
	dwLoc04_14 = bp
	bp_5 = fp - 0x00000004
	CZS_6 = wArg04 - 0x0003
	branch Test(GE,CZS_6) ge3
	// succ:  l2 ge3
l2:
	wArg04_17 = 0x0003
	goto done
	// succ:  done
ProcedureBuilder_exit:
	use bp_8
	use CZS_6
	use dwLoc04_14
	use fp
	use r1_7
	use r63_9
";
            RunTest(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1", 1);
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var cr = m.Frame.EnsureFlagGroup(flags, 0x3, "CZS", PrimitiveType.Byte);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, bp);
                m.Assign(bp, sp);
                m.Assign(cr, m.ISub(m.Mem16(m.IAdd(bp, 8)), 0x3));
                m.BranchIf(m.Test(ConditionCode.GE, cr), "ge3");

                m.Store(m.IAdd(bp, 8), m.Word16(3));
                m.Goto("done");

                m.Label("ge3");
                m.Store(m.IAdd(bp, 8), Constant.Int16(-3));
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
            Storage r1_ = null;
            // Simulate the creation of a subroutine.
            var procSub = this.pb.Add("Adder", m =>
            {
                r1_ = m.Register(1).Storage;
            });

            var procSubFlow = new ProcedureFlow2 { Trashed = { r1_ } };
            programFlow.ProcedureFlows2.Add(procSub, procSubFlow);

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
	use Mem3
	use r1_2
	use r2_1
";
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
	dwLoc0C_17 = PHI(dwLoc0C_18, dwLoc0C_19)
	r1_8 = dwLoc0C_17
	bp_9 = dwLoc04_14
	r63_10 = fp
	return
	// succ:  ProcedureBuilder_exit
ge3:
	dwLoc0C_19 = r1
	goto done
	// succ:  done
l1:
	r63_1 = fp
	r63_2 = fp - 0x00000004
	dwLoc04_14 = bp
	bp_5 = fp - 0x00000004
	dwLoc0C_15 = 0x00000000
	CZS_7 = wArg04 - 0x0003
	branch Test(GE,CZS_7) ge3
	// succ:  l2 ge3
l2:
	dwLoc0C_18 = r1
	goto done
	// succ:  done
ProcedureBuilder_exit:
	use bp_9
	use CZS_7
	use dwLoc04_14
	use dwLoc0C_17
	use fp
	use r1_8
	use r63_10
";
            RunTest(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1", 1);
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var cr = m.Frame.EnsureFlagGroup(flags, 0x3, "CZS", PrimitiveType.Byte);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, bp);
                m.Assign(bp, sp);
                m.Store(m.IAdd(bp, -8), m.Word32(0));
                m.Assign(cr, m.ISub(m.Mem16(m.IAdd(bp, 8)), 0x3));
                m.BranchIf(m.Test(ConditionCode.GE, cr), "ge3");

                m.Store(m.IAdd(bp, -8), r1);
                m.Goto("done");

                m.Label("ge3");
                m.Store(m.IAdd(bp, -8), r1);

                m.Label("done");
                m.Assign(r1, m.Mem32(m.IAdd(bp,-8)));
                m.Assign(bp, m.Mem32(sp));
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
	SZ_1 = cond(esi)
	C_2 = false
	CZ_3 = false (alias)
	al_4 = Test(ULE,false)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use al_4
	use C_2
	use CZ_3
	use esi
	use SZ_1
";
            RunTest(sExp, m =>
            {
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var scz = m.Frame.EnsureFlagGroup(flags, 7, "SZ", PrimitiveType.Byte);
                var cz = m.Frame.EnsureFlagGroup(flags, 3, "CZ", PrimitiveType.Byte);
                var c = m.Frame.EnsureFlagGroup(flags, 1, "C", PrimitiveType.Bool);
                var al = m.Reg8("al", 0);
                var esi = m.Reg32("esi", 6);
                m.Assign(scz, m.Cond(m.And(esi, esi)));
                m.Assign(c, Constant.False());
                m.Alias(cz, c);
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
	def r3
	def r2
	// succ:  l1
l1:
	branch r1 true
	// succ:  l2 true
l2:
	r2_4 = 0x00000010
	// succ:  true
true:
	call r3 (retsize: 4;)
		uses: r2,r3
		defs: r2_2,r3_3
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use r1
	use r2_2
	use r3_3
";
            RunTest(sExp, m =>
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

        [Test]
        public void Ssa2_Simple_DefUse()
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
            RunTest2(sExp, m =>
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
        public void Ssa2_UseUndefined()
        {
            var sExp = @"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def a
	def Mem0
	// succ:  l1
l1:
	Mem0[0x00123400:word32] = a
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest2(sExp, m =>
            {
                var a = m.Reg32("a", 0);
                m.Store(m.Word32(0x123400), a);
                m.Return();
            });
        }

        [Test]
        public void Ssa2_IfThen()
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
            RunTest2(sExp, m =>
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
        public void Ssa2_RegisterAlias()
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
	ah_3 = SLICE(eax_2, byte, 8)
	Mem0[0x00001234:byte] = ah_3
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            RunTest2(sExp, m =>
            {
                var regEax = new RegisterStorage("eax", 0, 0, PrimitiveType.Word32);
                var regAh = new RegisterStorage("ah", 0, 8, PrimitiveType.Byte);
                var eax = m.Frame.EnsureRegister(regEax);
                var ah = m.Frame.EnsureRegister(regAh);
                m.Assign(eax, m.Mem32(eax));
                m.Store(m.Word32(0x1234), ah);
                m.Return();
            });
        }

        [Test(Description ="Emulates calling an imported API Win32 on MIPS")]
        public void Ssa_ConstantPropagation()
        {
            var sExp =
@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def Mem0
	// succ:  l1
l1:
	r13_0 = 0x00030000
	r12_2 = Mem0[0x00031234:word32]
	call r12_2 (retsize: 0;)
		uses: r12_2,r13_0
		defs: r12_4,r13_3
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use Mem0
	use r12_4
	use r13_3
";
            var addr = Address.Ptr32(0x00031234);
            importReferences.Add(addr, new NamedImportReference(
                addr, "COREDLL.DLL", "fnFoo"));
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
    }
}

#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

        [SetUp]
        public void Setup()
        {
            this.pb = new ProgramBuilder();
            this.programFlow = new ProgramDataFlow();
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder(this.pb.Program.Architecture);
            builder(pb);
            var proc = pb.Procedure;
            var dg = new DominatorGraph<Block>(proc.ControlGraph, proc.EntryBlock);

            // Perform the initial transformation
            var ssa = new SsaTransform(programFlow, proc, dg);

            // Propagate values and simplify the results.
            // We hope the the sequence
            //   esp = fp - 4
            //   mov [esp-4],eax
            // will become
            //   esp_2 = fp - 4
            //   mov [fp - 8],eax

            var vp = new ValuePropagator(this.pb.Program.Architecture, ssa.SsaState.Identifiers, proc);
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
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
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
	use wArg04
";
            RunTest(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
                var flags = m.Architecture.GetFlagGroup(1).FlagRegister;
                var cr = m.Frame.EnsureFlagGroup(flags, 0x3, "CZS", PrimitiveType.Byte);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, bp);
                m.Assign(bp, sp);
                m.Assign(cr, m.ISub(m.LoadW(m.IAdd(bp, 8)), 0x3));
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
	use wArg04_16
";
            RunTest(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
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
		uses: r1_0,r2_1
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
	use wArg04
";
            RunTest(sExp, m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var bp = m.Frame.CreateTemporary("bp", sp.DataType);
                var r1 = m.Reg32("r1");
                var r2 = m.Reg32("r2");
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
                m.Assign(r1, m.LoadDw(m.IAdd(bp,-8)));
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
                var al = m.Reg8("al");
                var esi = m.Reg32("esi");
                m.Assign(scz, m.Cond(m.And(esi, esi)));
                m.Assign(c, Constant.False());
                m.Emit(new AliasAssignment(cz, c));
                m.Assign(al, m.Test(ConditionCode.ULE, cz));
                m.Return();
            });
        }
    }
}

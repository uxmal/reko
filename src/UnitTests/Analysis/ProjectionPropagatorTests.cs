
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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Arch.Z80;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Registers = Reko.Arch.Z80.Registers;
using X86Registers = Reko.Arch.X86.Registers;
using Reko.Arch.X86;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class ProjectionPropagatorTests
    {
        private IProcessorArchitecture arch;

        private void RunTest(string sExp, IProcessorArchitecture arch, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder(arch);
            builder(m);
            var sac = new SegmentedAccessClassifier(m.Ssa);
            sac.Classify();
            var prpr = new ProjectionPropagator(m.Ssa, sac);
            prpr.Transform();
            var sw = new StringWriter();
            m.Ssa.Procedure.WriteBody(false, sw);
            sw.Flush();
            m.Ssa.Dump(true);
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        private void Given_X86_16_Arch()
        {
            this.arch = new Reko.Arch.X86.X86ArchitectureReal("x86-real-16");
        }

        private void Given_X86_64_Arch()
        {
            this.arch = new Reko.Arch.X86.X86ArchitectureFlat64("x86-protected-64");
        }

        private void Given_Z80_Arch()
        {
            this.arch = new Z80ProcessorArchitecture("z80");
        }

        /// <summary>
        /// Fuse together subregisters that compose into a larger register.
        /// </summary>
        [Test]
        public void Prjpr_Simple_Subregisters()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def hl
l1:
	h = SLICE(hl, byte, 8)
	l = SLICE(hl, byte, 0)
	de_1 = hl
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_Z80_Arch();
            RunTest(sExp, arch, m =>
            {
                var h = m.Reg("h", Z80Registers.h); 
                var l = m.Reg("l", Z80Registers.l);
                var de_1 = m.Reg("de_1", Z80Registers.de);

                m.Def(h);
                m.Def(l);
                m.Assign(de_1, m.Seq(h, l));
                m.Return();
            });
        }


        /// <summary>
        /// Fuse together subregisters that compose into a larger register.
        /// </summary>
        [Test]
        public void Prjpr_Simple_Subregisters_slices()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	def hl
	l = (byte) hl
	h = SLICE(hl, ui8, 8)
	de_1 = hl
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_Z80_Arch();
            RunTest(sExp, arch, m =>
            {
                var hl = m.Reg("hl", Z80Registers.hl);
                var h = m.Reg("h", Z80Registers.h);
                var l = m.Reg("l", Z80Registers.l);
                var de_1 = m.Reg("de_1", Z80Registers.de);

                m.Def(hl);
                m.Assign(l, m.Cast(PrimitiveType.Byte, hl));
                m.Assign(h, m.Slice(hl, 8, 8));
                m.Assign(de_1, m.Seq(h, l));
                m.Return();
            });
        }


        /// <summary>
        /// Fuse together subregisters that compose into a larger register,
        /// used in two branches.
        /// </summary>
        [Test]
        public void Prjpr_Simple_Subregisters_TwoBranches()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def hl
l1:
	h = SLICE(hl, byte, 8)
	l = SLICE(hl, byte, 0)
	branch a == 0x00 m2azero
m1anotzero:
	de_1 = hl
	goto m3done
m2azero:
	de_2 = hl
m3done:
	de_3 = PHI((de_1, m1anotzero), (de_2, m2azero))
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_Z80_Arch();
            RunTest(sExp, arch, m =>
            {
                var a = m.Reg("a", Z80Registers.a);
                var h = m.Reg("h", Z80Registers.h);
                var l = m.Reg("l", Z80Registers.l);
                var de_1 = m.Reg("de_1", Z80Registers.de);
                var de_2 = m.Reg("de_2", Z80Registers.de);
                var de_3 = m.Reg("de_3", Z80Registers.de);

                m.Def(h);
                m.Def(l);
                m.BranchIf(m.Eq0(a), "m2azero");

                m.Label("m1anotzero");
                m.Assign(de_1, m.Seq(h, l));
                m.Goto("m3done");

                m.Label("m2azero");
                m.Assign(de_2, m.Seq(h, l));

                m.Label("m3done");
                m.Phi(de_3, (de_1, "m1anotzero"), (de_2, "m2azero"));
                m.Return();
            });
        }


        /// <summary>
        /// Fuse together a register pair.
        /// </summary>
        [Test]
        public void Prjpr_Simple_RegisterPair()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def hl_de
l1:
	hl = SLICE(hl_de, word16, 16)
	de = SLICE(hl_de, word16, 0)
	hl_de_1 = hl_de
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_Z80_Arch();
            RunTest(sExp, arch, m =>
            {
                var hl = m.Reg("hl", Z80Registers.hl);
                var de = m.Reg("de", Z80Registers.de);
                var hl_de = m.Frame.EnsureSequence(PrimitiveType.Word32, hl.Storage, de.Storage);
                var hl_de_1 = m.Ssa.Identifiers.Add(new Identifier("hl_de_1", hl_de.DataType, hl_de.Storage), null, null, false).Identifier;

                m.Def(hl);
                m.Def(de);
                m.Assign(hl_de_1, m.Seq(hl, de));
                m.Return();
            });
        }

        [Test]
        public void Prjpr_Phi_Subregisters()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def hl
l1:
	h = SLICE(hl, byte, 8)
	l = SLICE(hl, byte, 0)
	hl_8 = hl (alias)
loop:
	hl_9 = PHI((hl_8, l1), (hl_1, loop))
	h_4 = SLICE(hl_9, byte, 8) (alias)
	l_5 = SLICE(hl_9, byte, 0) (alias)
	hl_1 = hl_9 << 0x01
	h_2 = SLICE(hl_1, byte, 8)
	l_3 = (byte) hl_1
	goto loop
xit:
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_Z80_Arch();
            RunTest(sExp, arch, m =>
            {
                var h = m.Reg("h", Z80Registers.h);
                var l = m.Reg("l", Z80Registers.l);
                var hl_1 = m.Reg("hl_1", Z80Registers.de);
                var h_2 = m.Reg("h_2", Z80Registers.h);
                var l_3 = m.Reg("l_3", Z80Registers.l);
                var h_4 = m.Reg("h_4", Z80Registers.h);
                var l_5 = m.Reg("l_5", Z80Registers.l);

                m.Def(h);
                m.Def(l);

                m.Label("loop");
                m.Phi(h_4, (h, "l1"), (h_2, "loop"));
                m.Phi(l_5, (l, "l1"), (l_3, "loop"));
                m.Assign(hl_1, m.Shl(m.Seq(h_4, l_5), 1));
                m.Assign(h_2, m.Slice(PrimitiveType.Byte, hl_1, 8));
                m.Assign(l_3, m.Cast(PrimitiveType.Byte, hl_1));
                m.Goto("loop");

                m.Label("xit");
                m.Return();
            });
        }

        // Only fuse slices if they are adjacent. If the fused slices
        // don't cover the underlying storage, we must emit a new slice.
        [Test]
        public void Prjpr_Adjacent_Slices()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	def rcx
	t1 = SLICE(rcx, byte, 12)
	t2 = SLICE(rcx, byte, 4)
	cx_3 = SLICE(rcx, word16, 4)
	return
SsaProcedureBuilder_exit:
";
            #endregion
            Given_X86_64_Arch();
            RunTest(sExp, arch, m =>
            {
                var rcx = m.Reg("rcx", X86Registers.rcx);
                var cx_3 = m.Reg("cx_3", X86Registers.cx);
                var t1 = m.Temp("t1", new TemporaryStorage("t1", 2, PrimitiveType.Byte));
                var t2 = m.Temp("t2", new TemporaryStorage("t2", 2, PrimitiveType.Byte));
                m.Def(rcx);
                // The slices below are adjacent.
                m.Assign(t1, m.Slice(PrimitiveType.Byte, rcx, 12));
                m.Assign(t2, m.Slice(PrimitiveType.Byte, rcx, 4));
                m.Assign(cx_3, m.Seq(t1, t2));
                m.Return();
            });
    }

        // Only fuse slices if they are adjacent. If the fused slices
        // don't cover the underlying storage, we must emit a new slice.
        [Test]
        public void Prjpr_4_adjacent_Slices()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	def rcx
	t1 = SLICE(rcx, byte, 28)
	t2 = SLICE(rcx, byte, 20)
	t3 = SLICE(rcx, byte, 12)
	t4 = SLICE(rcx, byte, 4)
	ecx_3 = SLICE(rcx, word32, 4)
	return
SsaProcedureBuilder_exit:
";
            #endregion
            Given_X86_64_Arch();
            RunTest(sExp, arch, m =>
            {
                var rcx = m.Reg("rcx", X86Registers.rcx);
                var ecx_3 = m.Reg("ecx_3", X86Registers.cx);
                var t1 = m.Temp("t1", new TemporaryStorage("t1", 2, PrimitiveType.Byte));
                var t2 = m.Temp("t2", new TemporaryStorage("t2", 2, PrimitiveType.Byte));
                var t3 = m.Temp("t3", new TemporaryStorage("t3", 2, PrimitiveType.Byte));
                var t4 = m.Temp("t4", new TemporaryStorage("t4", 2, PrimitiveType.Byte));
                m.Def(rcx);
                // The slices below are adjacent.
                m.Assign(t1, m.Slice(PrimitiveType.Byte, rcx, 28));
                m.Assign(t2, m.Slice(PrimitiveType.Byte, rcx, 20));
                m.Assign(t3, m.Slice(PrimitiveType.Byte, rcx, 12));
                m.Assign(t4, m.Slice(PrimitiveType.Byte, rcx, 4));
                m.Assign(ecx_3, m.Seq(t1, t2, t3, t4));
                m.Return();
            });
        }

        // Only fuse slices if they are adjacent.
        [Test]
        public void Prjpr_Nonadjacent_Slices()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	def rcx
	t1 = SLICE(rcx, byte, 32)
	t2 = SLICE(rcx, byte, 0)
	cx_3 = SEQ(t1, t2)
	return
SsaProcedureBuilder_exit:
";
            #endregion
            Given_X86_64_Arch();
            RunTest(sExp, arch, m =>
            {
                var rcx = m.Reg("rcx", X86Registers.rcx);
                var cx_3 = m.Reg("cx_3", X86Registers.cx);
                var t1 = m.Temp("t1", new TemporaryStorage("t1", 2, PrimitiveType.Byte));
                var t2 = m.Temp("t2", new TemporaryStorage("t2", 2, PrimitiveType.Byte));
                m.Def(rcx);
                // The slices below are not adjacent.
                m.Assign(t1, m.Slice(PrimitiveType.Byte, rcx, 32));
                m.Assign(t2, m.Slice(PrimitiveType.Byte, rcx, 0));
                m.Assign(cx_3, m.Seq(t1, t2));
                m.Return();
            });
        }

        [Test]
        public void Prjpr_Call_To_Sequence()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	call <invalid> (retsize: 2;)
		defs: hl:hl_4
	h_1 = SLICE(hl_4, byte, 8) (alias)
	l_2 = SLICE(hl_4, byte, 0) (alias)
	Mem2[0x1234:cui16] = hl_4
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_Z80_Arch();
            RunTest(sExp, arch, m =>
            {
                var h_1 = m.Reg("h_1", Z80Registers.h);
                var l_2 = m.Reg("l_2", Z80Registers.l);
                m.Call("dont_care", 2,
                    new Identifier[0],
                    new[] { h_1, l_2 });
                m.MStore(m.Word16(0x1234), m.Seq(h_1, l_2));
                m.Return();
            });
        }

        [Test]
        public void Prjpr_Call_To_Sequence_Multiple_Uses()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	call <invalid> (retsize: 2;)
		defs: hl:hl_5
	h_1 = SLICE(hl_5, byte, 8) (alias)
	l_2 = SLICE(hl_5, byte, 0) (alias)
	Mem2[0x1234:cui16] = hl_5
	Mem3[0x1236:cui16] = hl_5
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_Z80_Arch();
            RunTest(sExp, arch, m =>
            {
                var h_1 = m.Reg("h_1", Z80Registers.h);
                var l_2 = m.Reg("l_2", Z80Registers.l);
                m.Call("dont_care", 2,
                    new Identifier[0],
                    new[] { h_1, l_2 });
                m.MStore(m.Word16(0x1234), m.Seq(h_1, l_2));
                m.MStore(m.Word16(0x1236), m.Seq(h_1, l_2));
                m.Return();
            });
        }

        [Test]
        public void Prjpr_Fuse_RegisterSequence()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	def ds
	es_bx_1 = Mem5[ds:0x1234:ptr32]
	es_2 = SLICE(es_bx_1, selector, 16) (alias)
	bx_3 = SLICE(es_bx_1, word16, 0) (alias)
	Mem6[es_bx_1 + 4:byte] = 0x03
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_X86_16_Arch();
            RunTest(sExp, arch, m =>
            {
                var ds = m.Reg("ds", X86Registers.ds);
                var es_bx_1 = m.SeqId("es_bx_1", PrimitiveType.Ptr32, X86Registers.es, X86Registers.bx);
                var es_2 = m.Reg("es_2", X86Registers.es);
                var bx_3 = m.Reg("bx_3", X86Registers.bx);
                var ax_4 = m.Reg("ax_4", X86Registers.ax);

                m.Def(ds);
                m.Assign(es_bx_1, m.SegMem(PrimitiveType.Ptr32, ds, m.Word16(0x1234)));
                m.Alias(es_2, m.Slice(es_2.DataType, es_bx_1, 16));
                m.Alias(bx_3, m.Slice(bx_3.DataType, es_bx_1, 0));
                m.SStore(es_2, m.IAddS(bx_3, 4), m.Byte(3));
                m.Return();
            });
        }

        /// <summary>
        /// We must be careful not to fuse register sequences if they're used elsewhere.
        /// </summary>
        [Test]
        [Ignore("Needs more thought and possibly a deeper analysis")]
        public void Prjpr_Defs_UsedElsewhere()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	def ds
	def si
	def di
	ax_1 = Mem5[ds:0x1234:ptr32]
	es_2 = SLICE(es_bx_1, selector, 16) (alias)
	bx_3 = SLICE(es_bx_1, word16, 0) (alias)
	Mem6[es_bx_1 + 4:byte] = 0x03
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_X86_16_Arch();
            RunTest(sExp, arch, m =>
            {
                var ds = m.Reg("ds", X86Registers.ds);
                var si = m.Reg("si", X86Registers.si);
                var di = m.Reg("di", X86Registers.di);
                var ax_1 = m.Reg("ax_1", X86Registers.di);
                var bx_2 = m.Reg("bx_2", X86Registers.di);

                m.Def(ds);
                m.Def(si);
                m.Def(di);
                m.Assign(ax_1, m.SegMem16(ds, si));
                m.Assign(bx_2, m.SegMem16(ds, di));
                m.Return();
            });
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        public void Prprj_ReturnedRegisterPair()
        {
            string sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def ds
l1:
	es_ax_1 = Mem4[ds:0x1234:word32]
	ax_2 = (word16) es_ax_1 (alias)
	es_3 = SLICE(es_ax_1, word16, 16) (alias)
	return
SsaProcedureBuilder_exit:
	use ax_2
	use es_3";
            #endregion
            Given_X86_16_Arch();
            RunTest(sExp, arch, m =>
            {
                var ds = m.Reg("ds", X86Registers.ds);
                var es_ax_1 = m.SeqId("es_ax_1", PrimitiveType.Word32, X86Registers.ds, X86Registers.ax);
                var ax_2 = m.Reg("ax_2", X86Registers.ax);
                var es_3 = m.Reg("es_3", X86Registers.es);
                m.AddDefToEntryBlock(ds);
                m.Assign(es_ax_1, m.SegMem(PrimitiveType.Word32, ds, m.Word16(0x1234)));
                m.Alias(ax_2, m.Cast(PrimitiveType.Word16, es_ax_1));
                m.Alias(es_3, m.Slice(PrimitiveType.Word16, es_ax_1, 16));
                m.Return();
                m.AddUseToExitBlock(ax_2);
                m.AddUseToExitBlock(es_3);
            });
        }

        [Test]
        public void Prjpr_Phi_StackVars()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
m1:
	wLoc06_1 = Mem7[0x1200:word16]
	wLoc08_1 = Mem8[0x1202:word16]
	dwLoc08_13 = SEQ(wLoc08_1, wLoc06_1) (alias)
	branch Mem9[0x1100:byte] == 0x00 m3
m2:
	wLoc06_2 = Mem10[0x1204:word16]
	wLoc08_2 = Mem11[0x1206:word16]
	dwLoc08_14 = SEQ(wLoc08_2, wLoc06_2) (alias)
m3:
	dwLoc08_15 = PHI((dwLoc08_13, m1), (dwLoc08_14, m2))
	wLoc08_3 = SLICE(dwLoc08_15, word16, 0) (alias)
	wLoc06_3 = SLICE(dwLoc08_15, word16, 16) (alias)
	ptrLoc06_1 = dwLoc08_15
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_X86_16_Arch();
            RunTest(sExp, arch, m =>
            {
                var wLoc06_1 = m.Local16("wLoc06_1", -6);
                var wLoc06_2 = m.Local16("wLoc06_2", -6);
                var wLoc06_3 = m.Local16("wLoc06_3", -6);

                var wLoc08_1 = m.Local16("wLoc08_1", -8);
                var wLoc08_2 = m.Local16("wLoc08_2", -8);
                var wLoc08_3 = m.Local16("wLoc08_3", -8);
                var ptrLoc06_1 = m.Local32("ptrLoc06_1", -6);

                m.Label("m1");
                m.Assign(wLoc06_1, m.Mem16(m.Word16(0x1200)));
                m.Assign(wLoc08_1, m.Mem16(m.Word16(0x1202)));
                m.BranchIf(m.Eq0(m.Mem8(m.Word16(0x1100))), "m3");

                m.Label("m2");
                m.Assign(wLoc06_2, m.Mem16(m.Word16(0x1204)));
                m.Assign(wLoc08_2, m.Mem16(m.Word16(0x1206)));

                m.Label("m3");
                m.Phi(wLoc08_3, (wLoc08_1, "m1"), (wLoc08_2, "m2"));
                m.Phi(wLoc06_3, (wLoc06_1, "m1"), (wLoc06_2, "m2"));
                m.Assign(ptrLoc06_1, m.Seq(wLoc08_3, wLoc06_3));
                m.Return();
            });
        }

        [Test]
        public void Prjpr_Phi_StackVars_AvoidSlices()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
m1:
	ptrLoc06_1 = Mem8[0x1200:word32]
	wLoc06_1 = SLICE(ptrLoc06_1, word16, 0) (alias)
	wLoc08_1 = SLICE(ptrLoc06_1, word16, 16) (alias)
	branch Mem9[0x1100:byte] == 0x00 m3
m2:
	wLoc06_2 = Mem10[0x1204:word16]
	wLoc08_2 = Mem11[0x1206:word16]
	dwLoc08_13 = SEQ(wLoc08_2, wLoc06_2) (alias)
m3:
	dwLoc08_14 = PHI((ptrLoc06_1, m1), (dwLoc08_13, m2))
	wLoc08_3 = SLICE(dwLoc08_14, word16, 0) (alias)
	wLoc06_3 = SLICE(dwLoc08_14, word16, 16) (alias)
	ptrLoc06_2 = dwLoc08_14
	return
SsaProcedureBuilder_exit:
";
            #endregion

            Given_X86_16_Arch();
            RunTest(sExp, arch, m =>
            {
                var wLoc06_1 = m.Local16("wLoc06_1", -6);
                var wLoc06_2 = m.Local16("wLoc06_2", -6);
                var wLoc06_3 = m.Local16("wLoc06_3", -6);

                var wLoc08_1 = m.Local16("wLoc08_1", -8);
                var wLoc08_2 = m.Local16("wLoc08_2", -8);
                var wLoc08_3 = m.Local16("wLoc08_3", -8);
                var ptrLoc06_1 = m.Local32("ptrLoc06_1", -6);
                var ptrLoc06_2 = m.Local32("ptrLoc06_2", -6);

                m.Label("m1");
                m.Assign(ptrLoc06_1, m.Mem32(m.Word16(0x1200)));
                m.Alias(wLoc06_1, m.Slice(PrimitiveType.Word16, ptrLoc06_1, 0));
                m.Alias(wLoc08_1, m.Slice(PrimitiveType.Word16, ptrLoc06_1, 16));
                m.BranchIf(m.Eq0(m.Mem8(m.Word16(0x1100))), "m3");

                m.Label("m2");
                m.Assign(wLoc06_2, m.Mem16(m.Word16(0x1204)));
                m.Assign(wLoc08_2, m.Mem16(m.Word16(0x1206)));

                m.Label("m3");
                m.Phi(wLoc08_3, (wLoc08_1, "m1"), (wLoc08_2, "m2"));
                m.Phi(wLoc06_3, (wLoc06_1, "m1"), (wLoc06_2, "m2"));
                m.Assign(ptrLoc06_2, m.Seq(wLoc08_3, wLoc06_3));
                m.Return();
            });
        }
    }
}

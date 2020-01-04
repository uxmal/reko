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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UnalignedMemoryAccessFuserTests : AnalysisTestBase
    {
        private ProcedureBuilder m;
        private Mock<IProcessorArchitecture> arch;
        private IDynamicLinker dynamicLinker;
        private FakeDecompilerEventListener listener;
        private Program program;

        [SetUp]
        public void Setup()
        {
            arch = new Mock<IProcessorArchitecture>();
            dynamicLinker = new Mock<IDynamicLinker>().Object;
            listener = new FakeDecompilerEventListener();
            m = new ProcedureBuilder();
            program = new Program();
        }

        private SsaState RunTest(ProcedureBuilder m)
        {
            var proc = m.Procedure;
            var gr = proc.CreateBlockDominatorGraph();
            var sst = new SsaTransform(
                program,
                proc,
                new HashSet<Procedure>(),
                dynamicLinker,
                new ProgramDataFlow());
            var ssa = sst.Transform();
            var ufuser = new UnalignedMemoryAccessFuser(ssa);
            ufuser.Transform();
            return ssa;
        }

        private void AssertStringsEqual(string sExp, SsaState ssa)
        {
            var sw = new StringWriter();
            ssa.Write(sw);
            ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Console.WriteLine("{0}", sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void __lwl(Identifier reg, Expression mem)
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);

            m.Assign(
                reg,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwL, PrimitiveType.Word32, 2),
                    reg, mem));
        }

        private void __lwr(Identifier reg, Expression mem)
        {
            m.Assign(
                reg,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwR, PrimitiveType.Word32, 2),
                    reg, mem));
        }

        private void __swl(Expression mem, Expression reg)
        {
            var app = m.Fn(
                    new PseudoProcedure(PseudoProcedure.SwL, PrimitiveType.Word32, 2),
                    mem, reg);
            if (mem is Identifier id)
            {
                m.Assign(id, app);
            }
            else
            {
                m.Store(mem, app);
            }
        }

        private void __swr(Expression mem, Expression reg)
        {
            var app = m.Fn(
                    new PseudoProcedure(PseudoProcedure.SwR, PrimitiveType.Word32, 2),
                    mem, reg);
            if (mem is Identifier id)
            {
                m.Assign(id, app);
            }
            else
            {
                m.Store(mem, app);
            }
        }

        [Test]
        public void UfuserMipsLittleEndianUnalignedWordLoad()
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);

            __lwl(r8, m.Mem32(m.IAdd(r4, 0x2B)));
            __lwr(r8, m.Mem32(m.IAdd(r4, 0x28)));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r8_4 = r8
r4:r4
    def:  def r4
    uses: r8_5 = Mem0[r4 + 0x00000028:word32]
Mem0:Mem
    def:  def Mem0
    uses: r8_5 = Mem0[r4 + 0x00000028:word32]
r8_4: orig: r8
    def:  r8_4 = r8
r8_5: orig: r8
    def:  r8_5 = Mem0[r4 + 0x00000028:word32]
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r8
	def r4
	def Mem0
	// succ:  l1
l1:
	r8_5 = Mem0[r4 + 0x00000028:word32]
ProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void UfuserMipsLittleEndianUnalignedWordLoad_0_offset()
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);

            __lwl(r8, m.Mem32(m.IAdd(r4, 0x3)));
            __lwr(r8, m.Mem32(r4));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r8_4 = r8
r4:r4
    def:  def r4
    uses: r8_5 = Mem0[r4:word32]
Mem0:Mem
    def:  def Mem0
    uses: r8_5 = Mem0[r4:word32]
r8_4: orig: r8
    def:  r8_4 = r8
r8_5: orig: r8
    def:  r8_5 = Mem0[r4:word32]
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r8
	def r4
	def Mem0
	// succ:  l1
l1:
	r8_5 = Mem0[r4:word32]
ProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void UfuserMipsBigEndianUnalignedWordLoad()
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);

            __lwl(r8, m.Mem32(m.IAdd(r4, 0xA5E4)));
            __lwr(r8, m.Mem32(m.IAdd(r4, 0xA5E7)));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r8_4 = r8
r4:r4
    def:  def r4
    uses: r8_5 = Mem0[r4 + 0x0000A5E4:word32]
Mem0:Mem
    def:  def Mem0
    uses: r8_5 = Mem0[r4 + 0x0000A5E4:word32]
r8_4: orig: r8
    def:  r8_4 = r8
r8_5: orig: r8
    def:  r8_5 = Mem0[r4 + 0x0000A5E4:word32]
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r8
	def r4
	def Mem0
	// succ:  l1
l1:
	r8_5 = Mem0[r4 + 0x0000A5E4:word32]
ProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void UfuserMipsLittleEndianUnalignedWordLoad_Coalesced()
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);

            m.Assign(
                r8,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwR, PrimitiveType.Word32, 2),
                    m.Fn(
                        new PseudoProcedure(PseudoProcedure.LwL, PrimitiveType.Word32, 2),
                        r8,
                        m.Mem32(m.IAdd(r4, 0x2B))),
                    m.Mem32(m.IAdd(r4, 0x28))));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
r4:r4
    def:  def r4
    uses: r8_4 = Mem0[r4 + 0x00000028:word32]
Mem0:Mem
    def:  def Mem0
    uses: r8_4 = Mem0[r4 + 0x00000028:word32]
r8_4: orig: r8
    def:  r8_4 = Mem0[r4 + 0x00000028:word32]
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r8
	def r4
	def Mem0
	// succ:  l1
l1:
	r8_4 = Mem0[r4 + 0x00000028:word32]
ProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, ssa);
        }



        [Test]
        public void UfuserMipsLittleEndianUnalignedWordStore()
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);

            __swl(m.Mem32(m.IAdd(r4, 0x2B)), r8);
            __swr(m.Mem32(m.IAdd(r4, 0x28)), r8);
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r4:r4
    def:  def r4
    uses: Mem5[r4 + 0x00000028:word32] = r8
Mem0:Mem
    def:  def Mem0
r8:r8
    def:  def r8
    uses: Mem5[r4 + 0x00000028:word32] = r8
Mem4: orig: Mem0
Mem5: orig: Mem0
    def:  Mem5[r4 + 0x00000028:word32] = r8
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r4
	def Mem0
	def r8
	// succ:  l1
l1:
	Mem5[r4 + 0x00000028:word32] = r8
ProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test(Description = "Fuse a sequence of stores, as seen in a real MIPS binary")]
        public void UfuserLittleEndianSequence()
        {
            var r8 = m.Reg32("r8", 8);
            var r14 = m.Reg32("r14", 14);
            var r13 = m.Reg32("r13", 13);
            var r9 = m.Reg32("r9", 9);
            var r4 = m.Reg32("r4", 4);

            __swl(m.Mem32(m.IAdd(r8, 0x13)), r14);
            __swl(m.Mem32(m.IAdd(r8, 0x17)), r13);
            __swl(m.Mem32(m.IAdd(r8, 0x1B)), m.Word32(0x00));
            __swl(m.Mem32(m.IAdd(r8, 0x1F)), m.Word32(0x00));
            __swl(m.Mem32(m.IAdd(r8, 0x2B)), m.Word32(0x00));
            __swl(m.Mem32(m.IAdd(r8, 0x2F)), r9);
            __swl(m.Mem32(m.IAdd(r8, 0x33)), m.Word32(0x00));
            m.Assign(r4, m.IAdd(r8, 0x0010));
            __swr(m.Mem32(m.IAdd(r8, 0x10)), r14);
            __swr(m.Mem32(m.IAdd(r8, 0x14)), r13);
            __swr(m.Mem32(m.IAdd(r8, 0x18)), m.Word32(0x00));
            __swr(m.Mem32(m.IAdd(r8, 0x1C)), m.Word32(0x00));
            __swr(m.Mem32(m.IAdd(r8, 0x28)), m.Word32(0x00));
            __swr(m.Mem32(m.IAdd(r8, 44)), r9);
            __swr(m.Mem32(m.IAdd(r8, 0x30)), m.Word32(0x00));
            m.Return();

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r4_13 = r8 + 0x00000010
          Mem14[r8 + 0x00000010:word32] = r14
          Mem15[r8 + 0x00000014:word32] = r13
          Mem16[r8 + 0x00000018:word32] = 0x00000000
          Mem17[r8 + 0x0000001C:word32] = 0x00000000
          Mem18[r8 + 0x00000028:word32] = 0x00000000
          Mem19[r8 + 0x0000002C:word32] = r9
          Mem20[r8 + 0x00000030:word32] = 0x00000000
Mem0:Mem
    def:  def Mem0
r14:r14
    def:  def r14
    uses: Mem14[r8 + 0x00000010:word32] = r14
Mem4: orig: Mem0
r13:r13
    def:  def r13
    uses: Mem15[r8 + 0x00000014:word32] = r13
Mem6: orig: Mem0
Mem7: orig: Mem0
Mem8: orig: Mem0
Mem9: orig: Mem0
r9:r9
    def:  def r9
    uses: Mem19[r8 + 0x0000002C:word32] = r9
Mem11: orig: Mem0
Mem12: orig: Mem0
r4_13: orig: r4
    def:  r4_13 = r8 + 0x00000010
Mem14: orig: Mem0
    def:  Mem14[r8 + 0x00000010:word32] = r14
Mem15: orig: Mem0
    def:  Mem15[r8 + 0x00000014:word32] = r13
Mem16: orig: Mem0
    def:  Mem16[r8 + 0x00000018:word32] = 0x00000000
Mem17: orig: Mem0
    def:  Mem17[r8 + 0x0000001C:word32] = 0x00000000
Mem18: orig: Mem0
    def:  Mem18[r8 + 0x00000028:word32] = 0x00000000
Mem19: orig: Mem0
    def:  Mem19[r8 + 0x0000002C:word32] = r9
Mem20: orig: Mem0
    def:  Mem20[r8 + 0x00000030:word32] = 0x00000000
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r8
	def Mem0
	def r14
	def r13
	def r9
	// succ:  l1
l1:
	r4_13 = r8 + 0x00000010
	Mem14[r8 + 0x00000010:word32] = r14
	Mem15[r8 + 0x00000014:word32] = r13
	Mem16[r8 + 0x00000018:word32] = 0x00000000
	Mem17[r8 + 0x0000001C:word32] = 0x00000000
	Mem18[r8 + 0x00000028:word32] = 0x00000000
	Mem19[r8 + 0x0000002C:word32] = r9
	Mem20[r8 + 0x00000030:word32] = 0x00000000
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test(Description = "Fuses a SWL/SWR pair assuming no writes are done to the words used by the 2 memory accesses")]
        public void UfuserAggressiveLittleEndianConstantStores()
        {
            var r8 = m.Reg32("r8", 8);
            __swl(m.Mem32(m.IAdd(r8, 0x13)), m.Word32(0x12345678));
            __swl(m.Mem32(m.IAdd(r8, 0x17)), m.Word32(0x9ABCDEF0u));
            __swr(m.Mem32(m.IAdd(r8, 0x10)), m.Word32(0x12345678));
            __swr(m.Mem32(m.IAdd(r8, 0x14)), m.Word32(0x9ABCDEF0u));
            m.Return();

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: Mem5[r8 + 0x00000010:word32] = 0x12345678
          Mem6[r8 + 0x00000014:word32] = 0x9ABCDEF0
Mem0:Mem
    def:  def Mem0
Mem3: orig: Mem0
Mem4: orig: Mem0
Mem5: orig: Mem0
    def:  Mem5[r8 + 0x00000010:word32] = 0x12345678
Mem6: orig: Mem0
    def:  Mem6[r8 + 0x00000014:word32] = 0x9ABCDEF0
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r8
	def Mem0
	// succ:  l1
l1:
	Mem5[r8 + 0x00000010:word32] = 0x12345678
	Mem6[r8 + 0x00000014:word32] = 0x9ABCDEF0
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void Ufuser_Store_MemoryAccessWithZeroOffset()
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);

            __swl(m.Mem32(m.IAdd(r4, 3)), r8);
            __swr(m.Mem32(r4), r8);
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r4:r4
    def:  def r4
    uses: Mem5[r4:word32] = r8
Mem0:Mem
    def:  def Mem0
r8:r8
    def:  def r8
    uses: Mem5[r4:word32] = r8
Mem4: orig: Mem0
Mem5: orig: Mem0
    def:  Mem5[r4:word32] = r8
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r4
	def Mem0
	def r8
	// succ:  l1
l1:
	Mem5[r4:word32] = r8
ProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
    }

        [Test]
        public void Ufuser_Load_MemoryAccessWithZeroOffset()
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);

            __lwl(r8, m.Mem32(m.IAdd(r4, 3)));
            __lwr(r8, m.Mem32(r4));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r8_4 = r8
r4:r4
    def:  def r4
    uses: r8_5 = Mem0[r4:word32]
Mem0:Mem
    def:  def Mem0
    uses: r8_5 = Mem0[r4:word32]
r8_4: orig: r8
    def:  r8_4 = r8
r8_5: orig: r8
    def:  r8_5 = Mem0[r4:word32]
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r8
	def r4
	def Mem0
	// succ:  l1
l1:
	r8_5 = Mem0[r4:word32]
ProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void Ufuser_Store_Bigendian()
        {
            var r4 = m.Reg32("r4", 4);
            var r8 = m.Reg32("r8", 8);
            var loc40 = m.Local32("loc40", -0x40);
            var loc3D = m.Local32("loc3D", -0x3D);

            __swl(loc40, r8);
            __swr(loc3D, r8);
            arch.Setup(a => a.Endianness).Returns(EndianServices.Big);
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"loc40:Local -0040
    def:  def loc40
r8:r8
    def:  def r8
    uses: loc40_3 = r8
loc40_3: orig: loc40
    def:  loc40_3 = __swl(loc40, r8)
    uses: bLoc3D_5 = SLICE(loc40_3, byte, 24) (alias)
nLoc3C:Local -003C
    def:  def nLoc3C
    uses: loc3D_6 = SEQ(nLoc3C, bLoc3D_5) (alias)
bLoc3D_5: orig: bLoc3D
    def:  bLoc3D_5 = SLICE(loc40_3, byte, 24) (alias)
    uses: loc3D_6 = SEQ(nLoc3C, bLoc3D_5) (alias)
loc3D_6: orig: loc3D
    def:  loc3D_6 = SEQ(nLoc3C, bLoc3D_5) (alias)
loc3D_7: orig: loc3D
    def:  loc40_3 = r8
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def loc40
	def r8
	def nLoc3C
	// succ:  l1
l1:
	bLoc3D_5 = SLICE(loc40_3, byte, 24) (alias)
	loc3D_6 = SEQ(nLoc3C, bLoc3D_5) (alias)
	loc40_3 = r8
ProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }
    }

}

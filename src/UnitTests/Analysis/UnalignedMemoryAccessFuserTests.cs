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
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UnalignedMemoryAccessFuserTests : AnalysisTestBase
    {
        private MockRepository mr;
        private SsaProcedureBuilder m;
        private IProcessorArchitecture arch;
        private IImportResolver importResolver;
        private FakeDecompilerEventListener listener;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            arch = mr.Stub<IProcessorArchitecture>();
            importResolver = mr.Stub<IImportResolver>();
            listener = new FakeDecompilerEventListener();
            m = new SsaProcedureBuilder();
        }

        private SsaState RunTest(ProcedureBuilder m)
        {
            mr.ReplayAll();

            var proc = m.Procedure;
            var gr = proc.CreateBlockDominatorGraph();
            var sst = new SsaTransform(new ProgramDataFlow(), proc, importResolver, gr, new HashSet<RegisterStorage>());
            var ssa = sst.SsaState;

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
                Debug.Print("{0}", sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void __lwl(Identifier reg, Expression mem)
        {
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
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

            __lwl(r8, m.Mem32(m.IAdd(r4, 0x2B)));
            __lwr(r8, m.Mem32(m.IAdd(r4, 0x28)));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r8_3 = r8
r4:r4
    def:  def r4
    uses: r8_5 = Mem3[r4 + 0x00000028:word32]
Mem2:Global memory
    def:  def Mem2
r8_3: orig: r8
    def:  r8_3 = r8
Mem3:Global memory
    def:  def Mem3
    uses: r8_5 = Mem3[r4 + 0x00000028:word32]
r8_5: orig: r8
    def:  r8_5 = Mem3[r4 + 0x00000028:word32]
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r8
	def r4
	def Mem2
	def Mem3
	// succ:  l1
l1:
	r8_5 = Mem3[r4 + 0x00000028:word32]
SsaProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void UfuserMipsLittleEndianUnalignedWordLoad_0_offset()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

            __lwl(r8, m.Mem32(m.IAdd(r4, 0x3)));
            __lwr(r8, m.Mem32(r4));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r8_3 = r8
r4:r4
    def:  def r4
    uses: r8_5 = Mem3[r4:word32]
Mem2:Global memory
    def:  def Mem2
r8_3: orig: r8
    def:  r8_3 = r8
Mem3:Global memory
    def:  def Mem3
    uses: r8_5 = Mem3[r4:word32]
r8_5: orig: r8
    def:  r8_5 = Mem3[r4:word32]
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r8
	def r4
	def Mem2
	def Mem3
	// succ:  l1
l1:
	r8_5 = Mem3[r4:word32]
SsaProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }


        [Test]
        public void UfuserMipsBigEndianUnalignedWordLoad()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

            __lwl(r8, m.Mem32(m.IAdd(r4, 0xA5E4)));
            __lwr(r8, m.Mem32(m.IAdd(r4, 0xA5E7)));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r8_3 = r8
r4:r4
    def:  def r4
    uses: r8_5 = Mem2[r4 + 0x0000A5E4:word32]
Mem2:Global memory
    def:  def Mem2
    uses: r8_5 = Mem2[r4 + 0x0000A5E4:word32]
r8_3: orig: r8
    def:  r8_3 = r8
Mem3:Global memory
    def:  def Mem3
r8_5: orig: r8
    def:  r8_5 = Mem2[r4 + 0x0000A5E4:word32]
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r8
	def r4
	def Mem2
	def Mem3
	// succ:  l1
l1:
	r8_5 = Mem2[r4 + 0x0000A5E4:word32]
SsaProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void UfuserMipsLittleEndianUnalignedWordLoad_Coalesced()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

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
    uses: r8_4 = Mem3[r4 + 0x00000028:word32]
Mem2:Global memory
    def:  def Mem2
Mem3:Global memory
    def:  def Mem3
    uses: r8_4 = Mem3[r4 + 0x00000028:word32]
r8_4: orig: r8
    def:  r8_4 = Mem3[r4 + 0x00000028:word32]
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r8
	def r4
	def Mem2
	def Mem3
	// succ:  l1
l1:
	r8_4 = Mem3[r4 + 0x00000028:word32]
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, ssa);
        }



        [Test]
        public void UfuserMipsLittleEndianUnalignedWordStore()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

            __swl(m.Mem32(m.IAdd(r4, 0x2B)), r8);
            __swr(m.Mem32(m.IAdd(r4, 0x28)), r8);
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r4:r4
    def:  def r4
    uses: Mem5[r4 + 0x00000028:word32] = r8
Mem2:Global memory
    def:  def Mem2
r8:r8
    def:  def r8
    uses: Mem5[r4 + 0x00000028:word32] = r8
Mem3: orig: Mem2
Mem3:Global memory
    def:  def Mem3
Mem5: orig: Mem3
    def:  Mem5[r4 + 0x00000028:word32] = r8
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r4
	def Mem2
	def r8
	def Mem3
	// succ:  l1
l1:
	Mem5[r4 + 0x00000028:word32] = r8
SsaProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test(Description = "Fuse a sequence of stores, as seen in a real MIPS binary")]
        public void UfuserLittleEndianSequence()
        {
            var r8 = m.Reg32("r8");
            var r14 = m.Reg32("r14");
            var r13 = m.Reg32("r13");
            var r9 = m.Reg32("r9");
            var r4 = m.Reg32("r4");

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
    uses: r4_18 = r8 + 0x00000010
          Mem20[r8 + 0x00000010:word32] = r14
          Mem22[r8 + 0x00000014:word32] = r13
          Mem24[r8 + 0x00000018:word32] = 0x00000000
          Mem26[r8 + 0x0000001C:word32] = 0x00000000
          Mem28[r8 + 0x00000028:word32] = 0x00000000
          Mem30[r8 + 0x0000002C:word32] = r9
          Mem32[r8 + 0x00000030:word32] = 0x00000000
Mem5:Global memory
    def:  def Mem5
r14:r14
    def:  def r14
    uses: Mem20[r8 + 0x00000010:word32] = r14
Mem3: orig: Mem5
Mem6:Global memory
    def:  def Mem6
r13:r13
    def:  def r13
    uses: Mem22[r8 + 0x00000014:word32] = r13
Mem6: orig: Mem6
Mem7:Global memory
    def:  def Mem7
Mem8: orig: Mem7
Mem8:Global memory
    def:  def Mem8
Mem10: orig: Mem8
Mem9:Global memory
    def:  def Mem9
Mem12: orig: Mem9
Mem10:Global memory
    def:  def Mem10
r9:r9
    def:  def r9
    uses: Mem30[r8 + 0x0000002C:word32] = r9
Mem15: orig: Mem10
Mem11:Global memory
    def:  def Mem11
Mem17: orig: Mem11
r4_18: orig: r4
    def:  r4_18 = r8 + 0x00000010
Mem12:Global memory
    def:  def Mem12
Mem20: orig: Mem12
    def:  Mem20[r8 + 0x00000010:word32] = r14
Mem13:Global memory
    def:  def Mem13
Mem22: orig: Mem13
    def:  Mem22[r8 + 0x00000014:word32] = r13
Mem14:Global memory
    def:  def Mem14
Mem24: orig: Mem14
    def:  Mem24[r8 + 0x00000018:word32] = 0x00000000
Mem15:Global memory
    def:  def Mem15
Mem26: orig: Mem15
    def:  Mem26[r8 + 0x0000001C:word32] = 0x00000000
Mem16:Global memory
    def:  def Mem16
Mem28: orig: Mem16
    def:  Mem28[r8 + 0x00000028:word32] = 0x00000000
Mem17:Global memory
    def:  def Mem17
Mem30: orig: Mem17
    def:  Mem30[r8 + 0x0000002C:word32] = r9
Mem18:Global memory
    def:  def Mem18
Mem32: orig: Mem18
    def:  Mem32[r8 + 0x00000030:word32] = 0x00000000
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r8
	def Mem5
	def r14
	def Mem6
	def r13
	def Mem7
	def Mem8
	def Mem9
	def Mem10
	def r9
	def Mem11
	def Mem12
	def Mem13
	def Mem14
	def Mem15
	def Mem16
	def Mem17
	def Mem18
	// succ:  l1
l1:
	r4_18 = r8 + 0x00000010
	Mem20[r8 + 0x00000010:word32] = r14
	Mem22[r8 + 0x00000014:word32] = r13
	Mem24[r8 + 0x00000018:word32] = 0x00000000
	Mem26[r8 + 0x0000001C:word32] = 0x00000000
	Mem28[r8 + 0x00000028:word32] = 0x00000000
	Mem30[r8 + 0x0000002C:word32] = r9
	Mem32[r8 + 0x00000030:word32] = 0x00000000
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test(Description = "Fuses a SWL/SWR pair assuming no writes are done to the words used by the 2 memory accesses")]
        public void UfuserAggressiveLittleEndianConstantStores()
        {
            var r8 = m.Reg32("r8");
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
    uses: Mem6[r8 + 0x00000010:word32] = 0x12345678
          Mem8[r8 + 0x00000014:word32] = 0x9ABCDEF0
Mem1:Global memory
    def:  def Mem1
Mem2: orig: Mem1
Mem2:Global memory
    def:  def Mem2
Mem4: orig: Mem2
Mem3:Global memory
    def:  def Mem3
Mem6: orig: Mem3
    def:  Mem6[r8 + 0x00000010:word32] = 0x12345678
Mem4:Global memory
    def:  def Mem4
Mem8: orig: Mem4
    def:  Mem8[r8 + 0x00000014:word32] = 0x9ABCDEF0
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r8
	def Mem1
	def Mem2
	def Mem3
	def Mem4
	// succ:  l1
l1:
	Mem6[r8 + 0x00000010:word32] = 0x12345678
	Mem8[r8 + 0x00000014:word32] = 0x9ABCDEF0
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void Ufuser_Store_MemoryAccessWithZeroOffset()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

            __swl(m.Mem32(m.IAdd(r4, 3)), r8);
            __swr(m.Mem32(r4), r8);
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r4:r4
    def:  def r4
    uses: Mem5[r4:word32] = r8
Mem2:Global memory
    def:  def Mem2
r8:r8
    def:  def r8
    uses: Mem5[r4:word32] = r8
Mem3: orig: Mem2
Mem3:Global memory
    def:  def Mem3
Mem5: orig: Mem3
    def:  Mem5[r4:word32] = r8
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r4
	def Mem2
	def r8
	def Mem3
	// succ:  l1
l1:
	Mem5[r4:word32] = r8
SsaProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }

        [Test]
        public void Ufuser_Load_MemoryAccessWithZeroOffset()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

            __lwl(r8, m.Mem32(m.IAdd(r4, 3)));
            __lwr(r8, m.Mem32(r4));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r8_3 = r8
r4:r4
    def:  def r4
    uses: r8_5 = Mem3[r4:word32]
Mem2:Global memory
    def:  def Mem2
r8_3: orig: r8
    def:  r8_3 = r8
Mem3:Global memory
    def:  def Mem3
    uses: r8_5 = Mem3[r4:word32]
r8_5: orig: r8
    def:  r8_5 = Mem3[r4:word32]
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def r8
	def r4
	def Mem2
	def Mem3
	// succ:  l1
l1:
	r8_5 = Mem3[r4:word32]
SsaProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }


        [Test]
        public void Ufuser_Store_Bigendian()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");
            var loc40 = m.Local32("loc40", -0x40);
            var loc3D = m.Local32("loc3D", -0x3D);

            __swl(loc40, r8);
            __swr(loc3D, r8);
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"loc40:Local -0040
    def:  def loc40
r8:r8
    def:  def r8
    uses: loc40_2 = r8
loc40_2: orig: loc40
    def:  loc40_2 = __swl(loc40, r8)
loc3D:Local -003D
    def:  def loc3D
loc3D_4: orig: loc3D
    def:  loc40_2 = r8
// SsaProcedureBuilder
// Return size: 0
void SsaProcedureBuilder()
SsaProcedureBuilder_entry:
	def loc40
	def r8
	def loc3D
	// succ:  l1
l1:
	loc40_2 = r8
SsaProcedureBuilder_exit:
";
            #endregion 
            AssertStringsEqual(sExp, ssa);
        }
    }

}

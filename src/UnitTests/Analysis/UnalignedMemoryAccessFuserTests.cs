#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

        [Test]
        public void UfuserMipsLittleEndianUnalignedWordLoad()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

            m.Assign(
                r8,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwL, PrimitiveType.Word32, 2),
                    r8,
                    m.LoadDw(m.IAdd(r4, 0x2B))));
            m.Assign(
                r8,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwR, PrimitiveType.Word32, 2),
                    r8,
                    m.LoadDw(m.IAdd(r4, 0x28))));
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
	r8_3 = r8
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

            m.Assign(
                r8,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwL, PrimitiveType.Word32, 2),
                    r8,
                    m.LoadDw(m.IAdd(r4, 0x3))));
            m.Assign(
                r8,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwR, PrimitiveType.Word32, 2),
                    r8,
                    m.LoadDw(r4)));
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
	r8_3 = r8
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

            m.Assign(
                r8,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwL, PrimitiveType.Word32, 2),
                    r8,
                    m.LoadDw(m.IAdd(r4, 0xA5E4))));
            m.Assign(
                r8,
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwR, PrimitiveType.Word32, 2),
                    r8,
                    m.LoadDw(m.IAdd(r4, 0xA5E7))));
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
	r8_3 = r8
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
                        m.LoadDw(m.IAdd(r4, 0x2B))),
                    m.LoadDw(m.IAdd(r4, 0x28))));
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

        private void __lwl(Expression mem, Expression reg)
        {
            m.SideEffect(
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwL, PrimitiveType.Word32, 2),
                    mem, reg));
        }

        private void __lwr(Expression mem, Expression reg)
        {
            m.SideEffect(
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.LwR, PrimitiveType.Word32, 2),
                    mem, reg));
        }

        private void __swl(Expression mem, Expression reg)
        {
            m.SideEffect(
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.SwL, PrimitiveType.Word32, 2),
                    mem, reg));
        }

        private void __swr(Expression mem, Expression reg)
        {
            m.SideEffect(
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.SwR, PrimitiveType.Word32, 2),
                    mem, reg));
        }

        [Test]
        public void UfuserMipsLittleEndianUnalignedWordStore()
        {
            var r4 = m.Reg32("r4");
            var r8 = m.Reg32("r8");

            m.SideEffect(
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.SwL, PrimitiveType.Word32, 2),
                    m.LoadDw(m.IAdd(r4, 0x2B)),
                    r8));
            m.SideEffect(
                m.Fn(
                    new PseudoProcedure(PseudoProcedure.SwR, PrimitiveType.Word32, 2),
                    m.LoadDw(m.IAdd(r4, 0x28)),
                    r8));
            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r4:r4
    def:  def r4
    uses: Mem3[r4 + 0x00000028:word32] = r8
Mem2:Global memory
    def:  def Mem2
r8:r8
    def:  def r8
    uses: Mem3[r4 + 0x00000028:word32] = r8
Mem3:Global memory
    def:  def Mem3
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
	Mem3[r4 + 0x00000028:word32] = r8
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

            __swl(m.LoadDw(m.IAdd(r8, 0x13)), r14);
            __swl(m.LoadDw(m.IAdd(r8, 0x17)), r13);
            __swl(m.LoadDw(m.IAdd(r8, 0x1B)), m.Word32(0x00));
            __swl(m.LoadDw(m.IAdd(r8, 0x1F)), m.Word32(0x00));
            __swl(m.LoadDw(m.IAdd(r8, 0x2B)), m.Word32(0x00));
            __swl(m.LoadDw(m.IAdd(r8, 0x2F)), r9);
            __swl(m.LoadDw(m.IAdd(r8, 0x33)), m.Word32(0x00));
            m.Assign(r4, m.IAdd(r8, 0x0010));
            __swr(m.LoadDw(m.IAdd(r8, 0x10)), r14);
            __swr(m.LoadDw(m.IAdd(r8, 0x14)), r13);
            __swr(m.LoadDw(m.IAdd(r8, 0x18)), m.Word32(0x00));
            __swr(m.LoadDw(m.IAdd(r8, 0x1C)), m.Word32(0x00));
            __swr(m.LoadDw(m.IAdd(r8, 0x28)), m.Word32(0x00));
            __swr(m.LoadDw(m.IAdd(r8, 44)), r9);
            __swr(m.LoadDw(m.IAdd(r8, 0x30)), m.Word32(0x00));
            m.Return();

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: r4_11 = r8 + 0x00000010
          Mem12[r8 + 0x00000010:word32] = r14
          Mem13[r8 + 0x00000014:word32] = r13
          Mem14[r8 + 0x00000018:word32] = 0x00000000
          Mem15[r8 + 0x0000001C:word32] = 0x00000000
          Mem16[r8 + 0x00000028:word32] = 0x00000000
          Mem17[r8 + 0x0000002C:word32] = r9
          Mem18[r8 + 0x00000030:word32] = 0x00000000
Mem5:Global memory
    def:  def Mem5
r14:r14
    def:  def r14
    uses: Mem12[r8 + 0x00000010:word32] = r14
Mem6:Global memory
    def:  def Mem6
r13:r13
    def:  def r13
    uses: Mem13[r8 + 0x00000014:word32] = r13
Mem7:Global memory
    def:  def Mem7
Mem8:Global memory
    def:  def Mem8
Mem9:Global memory
    def:  def Mem9
Mem10:Global memory
    def:  def Mem10
r9:r9
    def:  def r9
    uses: Mem17[r8 + 0x0000002C:word32] = r9
Mem11:Global memory
    def:  def Mem11
r4_11: orig: r4
    def:  r4_11 = r8 + 0x00000010
Mem12:Global memory
    def:  def Mem12
Mem13:Global memory
    def:  def Mem13
Mem14:Global memory
    def:  def Mem14
Mem15:Global memory
    def:  def Mem15
Mem16:Global memory
    def:  def Mem16
Mem17:Global memory
    def:  def Mem17
Mem18:Global memory
    def:  def Mem18
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
	r4_11 = r8 + 0x00000010
	Mem12[r8 + 0x00000010:word32] = r14
	Mem13[r8 + 0x00000014:word32] = r13
	Mem14[r8 + 0x00000018:word32] = 0x00000000
	Mem15[r8 + 0x0000001C:word32] = 0x00000000
	Mem16[r8 + 0x00000028:word32] = 0x00000000
	Mem17[r8 + 0x0000002C:word32] = r9
	Mem18[r8 + 0x00000030:word32] = 0x00000000
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
            __swl(m.LoadDw(m.IAdd(r8, 0x13)), m.Word32(0x12345678));
            __swl(m.LoadDw(m.IAdd(r8, 0x17)), m.Word32(0x9ABCDEF0u));
            __swr(m.LoadDw(m.IAdd(r8, 0x10)), m.Word32(0x12345678));
            __swr(m.LoadDw(m.IAdd(r8, 0x14)), m.Word32(0x9ABCDEF0u));
            m.Return();

            var ssa = RunTest(m);
            var sExp =
            #region Expected
@"r8:r8
    def:  def r8
    uses: Mem3[r8 + 0x00000010:word32] = 0x12345678
          Mem4[r8 + 0x00000014:word32] = 0x9ABCDEF0
Mem1:Global memory
    def:  def Mem1
Mem2:Global memory
    def:  def Mem2
Mem3:Global memory
    def:  def Mem3
Mem4:Global memory
    def:  def Mem4
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
	Mem3[r8 + 0x00000010:word32] = 0x12345678
	Mem4[r8 + 0x00000014:word32] = 0x9ABCDEF0
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion
            AssertStringsEqual(sExp, ssa);
        }
    }

}

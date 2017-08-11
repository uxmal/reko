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

    }
}

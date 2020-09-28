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
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class SequenceIdentifierGeneratorTests
    {
        private Mock<IDynamicLinker> dynamicLinker;

        [SetUp]
        public void Setup()
        {
            this.dynamicLinker = new Mock<IDynamicLinker>();
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder();
            builder(pb);
            var sst = new SsaTransform(
                new Program(), 
                pb.Procedure, 
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                null);
            sst.Transform();

            var seqgen = new SequenceIdentifierGenerator(sst);
            seqgen.Transform();

            DeadCode.Eliminate(sst.SsaState);

            var sw = new StringWriter();
            sst.SsaState.Write(sw);
            sst.SsaState.Procedure.Write(false, sw);
            var sResult = sw.ToString();
            if (sExp != sResult)
            {
                Console.Write(sw.ToString());
            }
            Assert.AreEqual(sExp, sResult);
        }

        [Test]
        public void Seqgen_Simple()
        {
            var sExp =
            #region Expected
@"r2:r2
r1:r1
Mem3: orig: Mem0
    def:  Mem3[0x00002000:word64] = r2_r1
r2_r1:Sequence r2:r1
    def:  def r2_r1
    uses: Mem3[0x00002000:word64] = r2_r1
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r2_r1
	// succ:  l1
l1:
	Mem3[0x00002000:word64] = r2_r1
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);

                m.MStore(m.Word32(0x2000), m.Seq(r2, r1));
            });
        }

        [Test]
        public void Seqgen_MultipleReferences()
        {
            var sExp =
            #region Expected
@"r2:r2
r1:r1
Mem3: orig: Mem0
    def:  Mem3[0x00002000:word64] = r2_r1
Mem4: orig: Mem0
    def:  Mem4[0x00002008:word64] = r2_r1
r2_r1:Sequence r2:r1
    def:  def r2_r1
    uses: Mem3[0x00002000:word64] = r2_r1
          Mem4[0x00002008:word64] = r2_r1
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def r2_r1
	// succ:  l1
l1:
	Mem3[0x00002000:word64] = r2_r1
	Mem4[0x00002008:word64] = r2_r1
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);

                m.MStore(m.Word32(0x2000), m.Seq(r2, r1));
                m.MStore(m.Word32(0x2008), m.Seq(r2, r1));
            });
        }

        [Test]
        public void Seqgen_SliceCastIdentifier()
        {
            var sExp =
            #region Expected
@"Mem0:Mem
    def:  def Mem0
    uses: r2_r1_2 = Mem0[0x2000:word64]
r2_r1_2: orig: r2_r1
    def:  r2_r1_2 = Mem0[0x2000:word64]
    uses: Mem6[r3 + 0x00002000:word64] = r2_r1_2
r2_3: orig: r2
r1_4: orig: r1
r3:r3
    def:  def r3
    uses: Mem6[r3 + 0x00002000:word64] = r2_r1_2
Mem6: orig: Mem0
    def:  Mem6[r3 + 0x00002000:word64] = r2_r1_2
Mem7: orig: Mem0
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def Mem0
	def r3
	// succ:  l1
l1:
	r2_r1_2 = Mem0[0x2000:word64]
	Mem6[r3 + 0x00002000:word64] = r2_r1_2
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                var r3 = m.Reg32("r3", 3);
                var r2_r1 = m.Frame.EnsureSequence(PrimitiveType.Word64, r2.Storage, r1.Storage);

                m.Assign(r2_r1, m.Mem(r2_r1.DataType, m.Word16(0x2000)));
                m.MStore(m.IAdd(r3, 0x2000), m.Cast(r1.DataType, r2_r1));
                m.MStore(m.IAdd(r3, 0x2004), m.Slice(PrimitiveType.Word32, r2_r1, 32));
                m.Return();
            });
        }
    }
}

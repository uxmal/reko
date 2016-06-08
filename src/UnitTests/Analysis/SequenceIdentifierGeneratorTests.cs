#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class SequenceIdentifierGeneratorTests
    {
        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var pb = new ProcedureBuilder();
            builder(pb);
            var sst = new SsaTransform2(null, pb.Procedure, null, null);
            sst.Transform();

            var seqgen = new SequenceIdentifierGenerator(sst);
            seqgen.Transform();

            DeadCode.Eliminate(pb.Procedure, sst.SsaState);

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
    def:  Mem3[0x00002000:uipr64] = r2_r1
r2_r1:Sequence r2:r1
    def:  def r2_r1
    uses: Mem3[0x00002000:uipr64] = r2_r1
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r2_r1
	// succ:  l1
l1:
	Mem3[0x00002000:uipr64] = r2_r1
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);

                m.Store(m.Word32(0x2000), m.Seq(r2, r1));
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
    def:  Mem3[0x00002000:uipr64] = r2_r1
Mem4: orig: Mem0
    def:  Mem4[0x00002008:uipr64] = r2_r1
r2_r1:Sequence r2:r1
    def:  def r2_r1
    uses: Mem3[0x00002000:uipr64] = r2_r1
          Mem4[0x00002008:uipr64] = r2_r1
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def r2_r1
	// succ:  l1
l1:
	Mem3[0x00002000:uipr64] = r2_r1
	Mem4[0x00002008:uipr64] = r2_r1
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);

                m.Store(m.Word32(0x2000), m.Seq(r2, r1));
                m.Store(m.Word32(0x2008), m.Seq(r2, r1));
            });
        }
    }
}

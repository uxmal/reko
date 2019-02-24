#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;


namespace Reko.UnitTests.Evaluation
{
    [TestFixture]
    public class ConstDivisionImplementedByMultiplicationTests
    {
        private void AssertSmallConst(string sExp, int shift, uint mult)
        {
            var m = new ProcedureBuilder();
            var c = Constant.Int32((int)mult);
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);
            var r2_r1 = m.Frame.EnsureSequence(r2.Storage, r1.Storage, PrimitiveType.Word64);

            var ass = m.Assign(r2_r1, m.SMul(r1, c));
            m.Alias(r2, m.Slice(PrimitiveType.Word32, r2_r1, 32));
            if (shift != 0)
                m.Assign(r2, m.Sar(r2, shift));

            var proc = m.Procedure;
            var ssa = new SsaTransform(
                null, 
                proc,
                null,
                proc.CreateBlockDominatorGraph(),
                new HashSet<RegisterStorage>()).Transform();
            var ctx = new SsaEvaluationContext(null, ssa.Identifiers, null);
            var rule = new ConstDivisionImplementedByMultiplication(ssa);
            ctx.Statement = proc.EntryBlock.Succ[0].Statements[0];
            Assert.IsTrue(rule.Match(ass));
            var instr = rule.TransformInstruction();
            Assert.AreEqual(sExp, instr.Src.ToString());
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> bld)
        {
            var m = new ProcedureBuilder();
            bld(m);
            var proc = m.Procedure;
            var alias = new Aliases(proc);
            alias.Transform();
            var ssa = new SsaTransform(
                null,
                proc,
                null,
                proc.CreateBlockDominatorGraph(),
                new HashSet<RegisterStorage>()).Transform();
            var segmentMap = new SegmentMap(Address.Ptr32(0));
            var vp = new ValuePropagator(segmentMap, ssa, new CallGraph(), null, null);
            vp.Transform();
            var rule = new ConstDivisionImplementedByMultiplication(ssa);
            rule.Transform();
            var sw = new StringWriter();
            proc.Write(false, sw);
            if (sExp != sw.ToString())
            {
                Debug.Print("{0}", sw);
                Assert.AreEqual(sExp, sw.ToString());
            }
        }

        [Test]
        public void Cdiv_SmallConstants()
        {
            AssertSmallConst("r1 / 15", 3, 0x88888889);
            AssertSmallConst("r1 / 14", 3, 0x92492493);
            AssertSmallConst("r1 / 13", 2, 0x4ec4ec4f);
            AssertSmallConst("r1 / 12", 1, 0x2aaaaaab);
            AssertSmallConst("r1 / 11", 1, 0x2e8ba2e9);
            AssertSmallConst("r1 / 10", 2, 0x66666667);
            AssertSmallConst("r1 / 9", 1, 0x38e38e39);
            AssertSmallConst("r1 / 7", 2, 0x92492493);
            AssertSmallConst("r1 / 6", 0, 0x2aaaaaab);
            AssertSmallConst("r1 / 5", 1, 0x66666667);
            AssertSmallConst("r1 / 3", 0, 0x55555556);
            AssertSmallConst("r1 / 1000", 6, 0x10624dd3);
        }

        [Test]
        public void Cdiv_TwoThirds()
        {
            AssertSmallConst("r1 * 2 / 3", 0, 0xAAAAAAAA);
        }

        private void Frac(int num, int denom)
        {
            var q = (double)num / denom;
            var n = (uint) Math.Round(q * Math.Pow(2.0, 32));

            var rat = ConstDivisionImplementedByMultiplication.FindBestRational(n);
            Debug.Print("{0}/{1} - {2}", num, denom, rat);
            if (num != rat.Numerator || 
               denom != rat.Denominator)
            {
                Debug.Print("***** inexact *****");
            }
        }

        [Test]
        public void Cdiv_ContinuedFraction()
        {
            Frac(1, 3);
            Frac(2, 3);
            Frac(1, 5);
            Frac(2, 5);
            Frac(3, 5);
            Frac(4, 5);
            Frac(1, 7);
            Frac(2, 7);
            Frac(3, 7);
            Frac(4, 7);
            Frac(5, 7);
            Frac(6, 7);
            Frac(1, 22);
            Frac(3, 100);
            Frac(1, 1000);
            Frac(7, 1000);
            Frac(13, 10000);
            Frac(7, 100000);
        }

        [Test]
        public void Cdiv_DivBy7_Issue_554()
        {
            var sExp =
            #region 
@"// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	def ecx
	// succ:  l1
l1:
	edx_0 = 0x24924925
	eax_2 = ecx
	edx_eax_3 = ecx *u 0x24924925
	edx_4 = ecx / 7
	eax_5 = ecx
	eax_6 = ecx - edx_4
	eax_7 = eax_6 >>u 0x01
	eax_8 = (eax_6 >>u 0x01) + edx_4
	eax_9 = eax_8 >>u 0x02
	edx_eax_10 = SEQ(edx_4, eax_8 >>u 0x02) (alias)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var eax = m.Reg32("eax", 0);
                var ecx = m.Reg32("ecx", 1);
                var edx = m.Reg32("edx", 2);
                var r2_r0 = m.Frame.EnsureSequence(edx.Storage, eax.Storage, PrimitiveType.Word64);
                m.Assign(edx, 0x24924925);
                m.Assign(eax, ecx);
                m.Assign(r2_r0, m.UMul(edx, eax));
                m.Assign(eax, ecx);
                m.Assign(eax, m.ISub(eax, edx));
                m.Assign(eax, m.Shr(eax, 1));
                m.Assign(eax, m.IAdd(eax, edx));
                m.Assign(eax, m.Shr(eax, 2));
                m.Return();
            });
 //8048490: 8b 4c 24 04     mov   0x4(% esp),%ecx
 //8048494: ba 25 49 92 24  mov   $0x24924925,%edx
 //8048499: 89 c8           mov   %ecx,%eax
 //804849b: f7 e2           mul   %edx
 //804849d: 89 c8           mov   %ecx,%eax
 //804849f: 29 d0           sub   %edx,%eax
 //80484a1: d1 e8           shr   1,%eax
 //80484a3: 01 d0           add   %edx,%eax
 //80484a5: c1 e8 02        shr   $0x2,%eax
 //80484a8: c3 ret
        }
    }
}

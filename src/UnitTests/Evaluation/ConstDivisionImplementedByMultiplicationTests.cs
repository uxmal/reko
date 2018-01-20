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
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var ctx = new SsaEvaluationContext(null, ssa.Identifiers);
            var rule = new ConstDivisionImplementedByMultiplication(ssa);
            ctx.Statement = proc.EntryBlock.Succ[0].Statements[0];
            Assert.IsTrue(rule.Match(ass));
            ass = rule.TransformInstruction();
            Assert.AreEqual(sExp, ass.Src.ToString());
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
    }
}

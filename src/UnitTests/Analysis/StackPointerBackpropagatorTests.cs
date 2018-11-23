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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class StackPointerBackpropagatorTests
    {
        private Program program;

        [SetUp]
        public void Setup()
        {
            var arch = new FakeArchitecture();
            var platform = new FakePlatform(null, arch);
            this.program = new Program
            {
                Architecture = arch,
                Platform  = platform,
            };
        }

        private void AssertStringsEqual(string sExp, SsaState ssa)
        {
            var sw = new StringWriter();
            ssa.Procedure.WriteBody(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.Print("{0}", sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void RunTest(SsaState ssa)
        {
            var spbp = new StackPointerBackpropagator(ssa);
            spbp.BackpropagateStackPointer();
            ssa.Validate(s => Assert.Fail(s));
        }

        [Test]
        public void Spbp_LinearProcedure()
        {
            var m = new SsaProcedureBuilder(nameof(Spbp_LinearProcedure));

            var fp = m.Ssa.Identifiers.Add(m.Frame.FramePointer, null, null, false).Identifier;
            var sp = m.RegisterStorage("sp", PrimitiveType.Word32);
            m.Architecture.StackRegister = sp;
            var sp_1 = m.Reg("sp_1", m.Architecture.StackRegister);
            var sp_2 = m.Reg("sp_2", m.Architecture.StackRegister);
            var sp_3 = m.Reg("sp_3", m.Architecture.StackRegister);

            m.AddDefToEntryBlock(fp);

            m.Assign(sp_1, m.ISub(fp, m.Int32(4)));
            // Indirect call = hell node
            var ci = m.Call(m.Mem32(m.Word32(0x2)), 4,
                new[] { sp_1 },
                new[] { sp_2 });
            m.Assign(sp_3, m.IAdd(sp_2, m.Int32(4)));
            m.Return();

            m.AddUseToExitBlock(sp_3);

            RunTest(m.Ssa);

            var sExp =
            #region Expected
@"Spbp_LinearProcedure_entry:
	def fp
l1:
	sp_1 = fp - 4
	call Mem4[0x00000002:word32] (retsize: 4;)
		uses: sp:sp_1
	sp_2 = fp - 4
	sp_3 = sp_2 + 4
	return
Spbp_LinearProcedure_exit:
	use sp_3
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

        [Test(Description = "This mirrors real world code which has more than one epilog")]
        public void Spbp_TwoExits()
        {
            var m = new SsaProcedureBuilder(nameof(Spbp_TwoExits));

            var fp = m.Ssa.Identifiers.Add(m.Frame.FramePointer, null, null, false).Identifier;
            var sp = m.RegisterStorage("sp", PrimitiveType.Word32);
            m.Architecture.StackRegister = sp;
            var sp_1 = m.Reg("sp_1", m.Architecture.StackRegister);
            var sp_2 = m.Reg("sp_2", m.Architecture.StackRegister);
            var sp_3 = m.Reg("sp_3", m.Architecture.StackRegister);
            var sp_4 = m.Reg("sp_4", m.Architecture.StackRegister);
            var sp_5 = m.Reg("sp_5", m.Architecture.StackRegister);
            var sp_6 = m.Reg("sp_6", m.Architecture.StackRegister);

            m.AddDefToEntryBlock(fp);

            m.Assign(sp_1, m.ISub(fp, m.Int32(4)));
            m.BranchIf(m.Eq0(m.Mem32(m.Word32(0x1))), "m_eq0");

            m.Label("m_ne0");
            // Indirect call = hell node
            m.Call(m.Mem32(m.Word32(0x4)), 4,
                new[] { sp_1 },
                new[] { sp_2 });
            m.Assign(sp_3, m.IAdd(sp_2, m.Int32(4)));
            m.Return();

            m.Label("m_eq0");
            // Indirect call = hell node
            m.Call(m.Mem32(m.Word32(0x8)), 4,
                new[] { sp_1 },
                new[] { sp_4 });
            m.Assign(sp_5, m.IAdd(sp_4, m.Int32(4)));
            m.Return();

            m.AddPhiToExitBlock(sp_6, sp_3, sp_5);
            m.AddUseToExitBlock(sp_6);

            RunTest(m.Ssa);

            var sExp =
            #region Expected
@"Spbp_TwoExits_entry:
	def fp
l1:
	sp_1 = fp - 4
	branch Mem7[0x00000001:word32] == 0x00000000 m_eq0
	goto m_ne0
m_eq0:
	call Mem9[0x00000008:word32] (retsize: 4;)
		uses: sp:sp_1
	sp_4 = fp - 4
	sp_5 = sp_4 + 4
	return
m_ne0:
	call Mem8[0x00000004:word32] (retsize: 4;)
		uses: sp:sp_1
	sp_2 = fp - 4
	sp_3 = sp_2 + 4
	return
Spbp_TwoExits_exit:
	sp_6 = PHI(sp_3, sp_5)
	use sp_6
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

    }
}

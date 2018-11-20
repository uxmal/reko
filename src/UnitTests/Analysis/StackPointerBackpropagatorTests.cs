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
            ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.Print("{0}", sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private SsaState RunTest(SsaState ssa)
        {
            var spbp = new StackPointerBackpropagator(ssa);
            spbp.BackpropagateStackPointer();
            return ssa;
        }

        [Test]
        public void Spbp_LinearProcedure()
        {
            var m = new SsaProcedureBuilder(nameof(Spbp_LinearProcedure));

            var fp = m.Ssa.Identifiers.Add(m.Frame.FramePointer, null, null, false).Identifier;
            var r63_1 = m.Reg("r63_1", m.Architecture.StackRegister);
            var r63_2 = m.Reg("r63_2", m.Architecture.StackRegister);
            var r63_3 = m.Reg("r63_3", m.Architecture.StackRegister);
            var r63_4 = m.Reg("r63_4", m.Architecture.StackRegister);
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");
            var r1_1 = m.Reg("r1_1", (RegisterStorage) r1.Storage);
            var r1_2 = m.Reg("r1_2", (RegisterStorage) r1.Storage);
            var r2_1 = m.Reg("r2_1", (RegisterStorage) r2.Storage);

            m.AddDefToEntryBlock(fp);
            m.AddDefToEntryBlock(r1);
            m.AddDefToEntryBlock(r2);

            m.Assign(r63_1, fp);
            m.Assign(r63_2, m.ISub(r63_1, m.Int32(4)));
            m.MStore(r63_2, r1);
            var ci = m.Call(r2, 4,      // Indirect call = hell node
                new[] { r1, r2, r63_2 },
                new[] { r1_1, r2_1, r63_3 });
            m.Assign(r1_2, m.Mem32(r63_3));
            m.Assign(r63_4, m.IAdd(r63_3, m.Int32(4)));
            m.Return();

            m.AddUseToExitBlock(r1_2);
            m.AddUseToExitBlock(r2_1);
            m.AddUseToExitBlock(r63_4);

            SsaState ssa = RunTest(m.Ssa);

            var sExp =
            #region Expected
@"// Spbp_LinearProcedure
// Return size: 0
define Spbp_LinearProcedure
Spbp_LinearProcedure_entry:
	def fp
	def r1
	def r2
	// succ:  l1
l1:
	r63_1 = fp
	r63_2 = r63_1 - 4
	Mem10[r63_2:word32] = r1
	call r2 (retsize: 4;)
		uses: r1:r1,r2:r2,r63:r63_2
		defs: r1:r1_1,r2:r2_1
	r63_3 = fp - 4
	r1_2 = Mem11[r63_3:word32]
	r63_4 = r63_3 + 4
	return
	// succ:  Spbp_LinearProcedure_exit
Spbp_LinearProcedure_exit:
	use r1_2
	use r2_1
	use r63_4
";
            #endregion
            AssertStringsEqual(sExp, ssa);
        }

        [Test(Description = "This mirrors real world code which has more than one epilog")]
        [Ignore("It would be nice if this passed. I'm leaving the code as is, but it will need to be fixed up")]
        public void Spbp_TwoExits()
        {
            var m = new SsaProcedureBuilder(nameof(Spbp_TwoExits));

            var fp = m.Ssa.Identifiers.Add(m.Frame.FramePointer, null, null, false).Identifier;
            var r63_1 = m.Reg("r63_1", m.Architecture.StackRegister);
            var r63_2 = m.Reg("r63_2", m.Architecture.StackRegister);
            var r63_3 = m.Reg("r63_3", m.Architecture.StackRegister);
            var r63_4 = m.Reg("r63_4", m.Architecture.StackRegister);
            var r63_5 = m.Reg("r63_5", m.Architecture.StackRegister);
            var r63_6 = m.Reg("r63_6", m.Architecture.StackRegister);
            var r63_7 = m.Reg("r63_7", m.Architecture.StackRegister);
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");
            var r3 = m.Reg32("r3");
            var r1_1 = m.Reg("r1_1", (RegisterStorage) r1.Storage);
            var r1_2 = m.Reg("r1_2", (RegisterStorage) r1.Storage);
            var r1_3 = m.Reg("r1_3", (RegisterStorage) r1.Storage);
            var r1_4 = m.Reg("r1_4", (RegisterStorage) r1.Storage);
            var r1_5 = m.Reg("r1_5", (RegisterStorage) r1.Storage);
            var r2_1 = m.Reg("r2_1", (RegisterStorage) r2.Storage);
            var r2_2 = m.Reg("r2_2", (RegisterStorage) r2.Storage);
            var r2_3 = m.Reg("r2_3", (RegisterStorage) r2.Storage);
            var r3_1 = m.Reg("r3_1", (RegisterStorage) r3.Storage);
            var r3_2 = m.Reg("r3_2", (RegisterStorage) r3.Storage);
            var r3_3 = m.Reg("r3_3", (RegisterStorage) r3.Storage);

            m.AddDefToEntryBlock(fp);
            m.AddDefToEntryBlock(r1);
            m.AddDefToEntryBlock(r2);
            m.AddDefToEntryBlock(r3);

            m.Assign(r63_1, fp);
            m.Assign(r63_2, m.ISub(r63_1, m.Int32(4)));
            m.MStore(r63_2, r1);
            m.BranchIf(m.Eq0(r3), "m_eq0");

            m.Label("m_ne0");
            m.Call(m.Mem32(m.IAdd(r2, 4)), 4,      // Indirect call = hell node
                new[] { r1, r2, r3, r63_2 },
                new[] { r1_1, r2_1, r3_1, r63_3 });
            m.Assign(r1_2, m.Mem32(r63_3));
            m.Assign(r63_4, m.IAdd(r63_3, m.Int32(4)));
            m.Return();

            m.Label("m_eq0");
            m.Call(m.Mem32(m.IAdd(r2, 8)), 4,      // Indirect call = hell node
                new[] { r1, r2, r3, r63_2 },
                new[] { r1_3, r2_2, r3_2, r63_5 });
            m.Assign(r1_4, m.Mem32(r63_5));
            m.Assign(r63_6, m.IAdd(r63_5, m.Int32(4)));
            m.Return();

            m.Phi(r1_5, r1_2, r1_4);
            m.Phi(r2_3, r2_1, r2_3);
            m.Phi(r3_3, r3_1, r2_3);
            m.Phi(r63_6, r63_3, r63_5);
            m.AddUseToExitBlock(r1_5);
            m.AddUseToExitBlock(r2_3);
            m.AddUseToExitBlock(r3_3);
            m.AddUseToExitBlock(r63_6);

            SsaState ssa = RunTest(m.Ssa);

            var sExp =
            #region Expected
@"// Spbp_TwoExits
// Return size: 0
define Spbp_TwoExits
Spbp_TwoExits_entry:
	def fp
	def r1
	def r2
	def r3
	// succ:  l1
l1:
	r63_1 = fp
	r63_2 = r63_1 - 4
	Mem22[r63_2:word32] = r1
	branch r3 == 0x00000000 m_eq0
	goto m_ne0
	// succ:  m_ne0 m_eq0
m_eq0:
	call Mem22[r2 + 0x00000008:word32] (retsize: 4;)
		uses: r1:r1,r2:r2,r3:r3,r63:r63_3
		defs: r1:r1_9,r2:r2_10,r3:r3_11,r63:r63_8
	r63_14 = fp - 4
	r1_12 = Mem5[r63_8:word32]
	r63_13 = r63_8 + 4
	return
	// succ:  ProcedureBuilder_exit
m_ne0:
	call Mem5[r2 + 0x00000004:word32] (retsize: 4;)
		uses: r1:r1,r2:r2,r3:r3,r63:r63_3
		defs: r1:r1_15,r2:r2_16,r3:r3_17
	r63_14 = fp - 4
	r1_18 = Mem5[r63_14:word32]
	r63_19 = r63_14 + 4
	return
	// succ:  Spbp_TwoExits_exit
Spbp_TwoExits_exit:
	r63_23 = PHI(r63_19, r63_13)
	r3_22 = PHI(r3_17, r3_11)
	r2_21 = PHI(r2_16, r2_10)
	r1_20 = PHI(r1_18, r1_12)
	use r1_20
	use r2_21
	use r3_22
	use r63_23
";
            #endregion
            AssertStringsEqual(sExp, ssa);
        }

    }
}

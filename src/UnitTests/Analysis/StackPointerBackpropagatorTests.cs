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

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class StackPointerBackpropagatorTests
    {
        private Program program;
        private RegisterStorage sp;
        private Identifier fp;

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
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void RunTest(SsaState ssa)
        {
            var spbp = new StackPointerBackpropagator(ssa);
            spbp.BackpropagateStackPointer();
            ssa.Validate(s => Assert.Fail(s));
        }

        private void Given_StackPointer(SsaProcedureBuilder m)
        {
            this.sp = m.RegisterStorage("sp", PrimitiveType.Word32);
            m.Architecture.StackRegister = sp;
        }

        [Test]
        public void Spbp_LinearProcedure()
        {
            var m = new SsaProcedureBuilder(nameof(Spbp_LinearProcedure));

            var fp = m.Ssa.Identifiers.Add(m.Frame.FramePointer, null, null, false).Identifier;
            Given_StackPointer(m);
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
	sp_3 = fp
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

            Given_FramePointer(m);
            Given_StackPointer(m);
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

            m.AddPhiToExitBlock(sp_6, (sp_3, "m_ne0"), (sp_5, "m_eq0"));
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
	sp_5 = fp
	return
m_ne0:
	call Mem8[0x00000004:word32] (retsize: 4;)
		uses: sp:sp_1
	sp_2 = fp - 4
	sp_3 = fp
	return
Spbp_TwoExits_exit:
	sp_6 = PHI((sp_3, m_ne0), (sp_5, m_eq0))
	use sp_6
";
            #endregion
            AssertStringsEqual(sExp, m.Ssa);
        }

        private void Given_FramePointer(SsaProcedureBuilder m)
        {
            this.fp = m.Ssa.Identifiers.Add(m.Frame.FramePointer, null, null, false).Identifier;
        }

        [Test]
        public void Spbp_SpaceOnStack()
        {
            var m = new SsaProcedureBuilder(nameof(Spbp_SpaceOnStack));

            var fp = m.Ssa.Identifiers.Add(m.Frame.FramePointer, null, null, false).Identifier;
            Given_StackPointer(m);
            var sp_1 = m.Reg("sp_1", sp);
            var sp_2 = m.Reg("sp_2", sp);
            var sp_3 = m.Reg("sp_3", sp);
            var sp_4 = m.Reg("sp_4", sp);
            var sp_5 = m.Reg("sp_5", sp);
            var sp_6 = m.Reg("sp_6", sp);
            var sp_7 = m.Reg("sp_7", sp);
            var sp_8 = m.Reg("sp_8", sp);
            var a = m.Reg("a", new RegisterStorage("a", 1, 0, PrimitiveType.Word32));
            var b = m.Reg("b", new RegisterStorage("b", 2, 0, PrimitiveType.Word32));
            var a_1 = m.Reg("a_1", (RegisterStorage) a.Storage);
            var b_1 = m.Reg("b_1", (RegisterStorage) b.Storage);
            m.AddDefToEntryBlock(fp);

            m.Assign(sp_2, m.ISub(fp, 4));  // space for a
            m.MStore(sp_2, a);
            m.Assign(sp_3, m.ISub(fp, 4));  // space for b
            m.MStore(sp_3, b);
            m.Assign(sp_4, m.ISub(fp, 40)); // 40 bytes of stack space
            m.MStore(sp_4, m.Word32(0xDEADBABE));
            m.Call(m.Mem32(m.Word32(0x00123400)), 4,
                new[] { sp_4 },
                new[] { sp_5 });
            m.Assign(sp_6, m.IAdd(sp_5, 40));
            m.Assign(b_1, m.Mem32(sp_6));
            m.Assign(sp_7, m.IAdd(sp_6, 4));
            m.Assign(a_1, m.Mem32(sp_7));
            m.Assign(sp_8, m.IAdd(sp_7, 4));
            m.Return();
            m.AddUseToExitBlock(sp_8);

            RunTest(m.Ssa);

            var sExp =
            #region Expected
@"Spbp_SpaceOnStack_entry:
	def fp
l1:
	sp_2 = fp - 0x00000004
	Mem13[sp_2:word32] = a
	sp_3 = fp - 0x00000004
	Mem14[sp_3:word32] = b
	sp_4 = fp - 0x00000028
	Mem15[sp_4:word32] = 0xDEADBABE
	call Mem16[0x00123400:word32] (retsize: 4;)
		uses: sp:sp_4
	sp_5 = fp - 48
	sp_6 = fp - 8
	b_1 = Mem17[sp_6:word32]
	sp_7 = fp - 4
	a_1 = Mem18[sp_7:word32]
	sp_8 = fp
	return
Spbp_SpaceOnStack_exit:
	use sp_8
";
            #endregion

            this.AssertStringsEqual(sExp, m.Ssa);
        }
    }
}

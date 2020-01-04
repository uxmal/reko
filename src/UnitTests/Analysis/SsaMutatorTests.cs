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
using Reko.UnitTests.Mocks;
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
    public class SsaMutatorTests
    {
        private SsaProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new SsaProcedureBuilder();
        }

        public void RunSsaMutator(Action<SsaMutator> action)
        {
            var ssam = new SsaMutator(m.Ssa);
            action(ssam);
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        private void AssertProcedureCode(string expected)
        {
            ProcedureCodeVerifier.AssertCode(m.Ssa.Procedure, expected);
            }

        [Test]
        public void SsamCallBypass()
        {
            var sp = m.Architecture.StackRegister;
            var sp_1 = m.Reg("sp_1", sp);
            var sp_5 = m.Reg("sp_5", sp);
            var a = m.Reg32("a");
            var fp = m.Reg32("fp");
            m.Assign(sp_1, fp);
            var uses = new Identifier[] { a, sp_1 };
            var defines = new Identifier[] { sp_5 };
            var stmCall = m.Call(a, 4, uses, defines);

            RunSsaMutator(ssam =>
            {
                var call = (CallInstruction)stmCall.Instruction;
                ssam.AdjustRegisterAfterCall(stmCall, call, sp, 0);
            });

            var sExp = @"
sp_1 = fp
call a (retsize: 4;)
	uses: a:a,r63:sp_1
sp_5 = sp_1
";
            AssertProcedureCode(sExp);
        }
    }
}

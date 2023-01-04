#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Rtl
{
    [TestFixture]
    public class RtlInstructionMatcherTests
    {
        private RtlEmitter m;
        private List<RtlInstruction> instrs = default!;
        private Identifier r1 = Identifier.Create(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));

        [SetUp]
        public void Setup()
        {
            instrs = new List<RtlInstruction>();
            m = new RtlEmitter(instrs);
        }

        private ExpressionMatch When_Match(RtlInstructionMatcher pattern)
        {
            return pattern.Match(instrs.Last());
        }

        [Test]
        public void Rtlm_MatchConst()
        {
            var pattern = RtlInstructionMatcher.Build(m => m.Assign(m.AnyId("a"), m.AnyConst("c")));

            m.Assign(r1, m.Word32(0x42));

            var match = When_Match(pattern);
            Assert.IsTrue(match.Success);
            Assert.AreEqual(r1, match.CapturedExpression("a"));
            Assert.AreEqual("0x42<32>", match.CapturedExpression("c").ToString());
        }
    }
}

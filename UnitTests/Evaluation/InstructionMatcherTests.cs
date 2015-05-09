#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Evaluation
{
    [TestFixture]
    public class InstructionMatcherTests
    {
        private ProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
        }

        private Identifier RegW(string name)
        {
            return m.Frame.EnsureRegister(new RegisterStorage(name, 0, PrimitiveType.Word16));
        }

        [Test]
        public void MatchAssignment()
        {
            var ax = RegW("ax");
            var pattern = m.Assign(ax, m.IAdd(ax, ExpressionMatcher.AnyConstant("c")));

            var instrmatcher = new InstructionMatcher(pattern);
            Assert.IsTrue(instrmatcher.Match(
                m.Assign(ax, m.IAdd(ax, m.Word16(42)))));

            Assert.AreEqual("0x002A", instrmatcher.CapturedExpressions("c").ToString());
        }
    }
}

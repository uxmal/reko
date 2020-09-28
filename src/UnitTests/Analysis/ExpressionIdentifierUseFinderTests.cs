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
using Reko.UnitTests.Mocks;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    public class ExpressionIdentifierUseFinderTests
    {
        private ProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            this.m = new ProcedureBuilder();
        }

        [Test]
        public void FindNone()
        {
            Assert.AreEqual(0, ExpressionIdentifierUseFinder.Find(null, m.Int32(3)).Count);
        }

        [Test]
        public void FindId()
        {
            Assert.AreEqual(1, ExpressionIdentifierUseFinder.Find(null, m.Local(PrimitiveType.Word16, "loc3")).Count);
        }

        [Test]
        public void FindIdsInBinOp()
        {
            Assert.AreEqual(2, ExpressionIdentifierUseFinder.Find(null, m.IAdd(
                m.Local32("loc2"),
                m.Local32("loc1"))).Count);
        }

        [Test]
        public void FindNoOutParams()
        {
            Assert.AreEqual(0, ExpressionIdentifierUseFinder.Find(null, m.Fn("foo", m.Out(PrimitiveType.Ptr32, m.Local32("tmp")))).Count);
        }
    }
}

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

using NUnit.Framework;
using Decompiler.Analysis;
using Decompiler.UnitTests.Mocks;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    public class UsedIdentifierFinderTests
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
            Assert.AreEqual(0, UsedIdentifierFinder.Find(null, m.Int32(3)).Count);
        }

        [Test]
        public void FindId()
        {
            Assert.AreEqual(1, UsedIdentifierFinder.Find(null, m.Local(PrimitiveType.Word16, "loc3")).Count);
        }

        [Test]
        public void FindIdsInBinOp()
        {
            Assert.AreEqual(2, UsedIdentifierFinder.Find(null, m.IAdd(
                m.Local32("loc2"),
                m.Local32("loc1"))).Count);
        }

        [Test]
        public void FindNoOutParams()
        {
            Assert.AreEqual(0, UsedIdentifierFinder.Find(null, m.Fn("foo", m.Out(PrimitiveType.Pointer32, m.Local32("tmp")))).Count);
        }
    }
}

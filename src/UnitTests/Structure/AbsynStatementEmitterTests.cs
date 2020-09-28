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

using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Code;
using Reko.Structure;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Structure
{
    [TestFixture]
    public class AbsynStatementEmitterTests
    {
        ProcedureBuilder m;
        List<AbsynStatement> stms;
        AbsynStatementEmitter emitter;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
            stms = new List<AbsynStatement>();
            emitter = new AbsynStatementEmitter(stms);
        }

        [Test]
        public void DeclarationWithExpression()
        {
            emitter.EmitStatement(m.Declare(m.Local32("dwLoc14"), m.Word32(1)));
            Assert.AreEqual(1, stms.Count);
            AbsynDeclaration decl = (AbsynDeclaration) stms[0];
            Assert.IsNotNull(decl.Expression);

        }

        [Test]
        public void DeclarationWithoutExpression()
        {
            emitter.EmitStatement(m.Declare(m.Local32("dwLoc14"), null));
            Assert.AreEqual(1, stms.Count);
            AbsynDeclaration decl = (AbsynDeclaration) stms[0];
            Assert.IsNull(decl.Expression);

        }

        [Test]
        public void StripDeclarations()
        {
            emitter.StripDeclarations = true;
            emitter.EmitStatement(m.Declare(m.Local32("dwLoc14"), null));
            emitter.EmitStatement(m.Declare(m.Local32("dwLoc18"), m.Word32(1)));
            Assert.AreEqual(1, stms.Count);
            AbsynAssignment ass= (AbsynAssignment) stms[0];
            Assert.AreEqual("dwLoc18 = 0x00000001;", ass.ToString());
        }

    }
}

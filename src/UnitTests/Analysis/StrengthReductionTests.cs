#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.UnitTests.Mocks;
using Reko.UnitTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class StrengthReductionTests
    {
        private SsaIdentifierCollection sids;

        [SetUp]
        public void Setup()
        {
            sids = new SsaIdentifierCollection();
        }

        [Test]
        public void SrtSimpleLoop()
        {
            Procedure proc = BuildSimpleLoop();

            var dom = proc.CreateBlockDominatorGraph();
            SsaTransform ssa = new SsaTransform(new ProgramDataFlow(), proc, null, dom, new HashSet<RegisterStorage>());
            LinearInductionVariableFinder lif = new LinearInductionVariableFinder(proc, ssa.SsaState.Identifiers, dom);
            lif.Find();

            Assert.AreEqual(1, lif.InductionVariables.Count, "Should have found one induction variable");
            Assert.AreEqual(1, lif.Contexts.Count);
            LinearInductionVariableContext ctx = lif.Contexts[lif.InductionVariables[0]];

            StrengthReduction str = new StrengthReduction(ssa.SsaState,lif.InductionVariables[0], ctx);
            str.ClassifyUses();
            Assert.AreEqual(1, str.IncrementedUses.Count);
            str.ModifyUses();
            Assert.AreEqual("(0x00003000 0x00000004 0x00007000)", lif.InductionVariables[0].ToString());
            using (FileUnitTester fut = new FileUnitTester("Analysis/SrtSimpleLoop.txt"))
            {
                proc.Write(false, fut.TextWriter);
                fut.AssertFilesEqual();
            }
        }

        private Procedure BuildSimpleLoop()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier p = m.Local32("p");
            m.Assign(p, 0);

            m.Label("loop");
            m.BranchIf(m.Eq(p, 0x4000), "done");
            m.Store(m.IAdd(p, 0x3000), m.Int32(0));
            m.Assign(p, m.IAdd(p, 4));
            m.Goto("loop");

            m.Label("done");
            m.Return();
            return m.Procedure;
        }

        private SsaIdentifier AddSid(string name)
        {
            Identifier id = new Identifier(name, null, null);
            SsaIdentifier sid = sids.Add(id, new Statement(0, new DefInstruction(id), null), null, false);
            return sid;
        }
    }
}

/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.UnitTests.Mocks;
using Decompiler.UnitTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Analysis
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
            SsaTransform ssa = new SsaTransform(proc, dom, false);
            proc.Write(false, Console.Out);
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
            ProcedureMock m = new ProcedureMock();
            Identifier p = m.Local32("p");
            m.Assign(p, 0);

            m.Label("loop");
            m.BranchIf(m.Eq(p, 0x4000), "done");
            m.Store(m.Add(p, 0x3000), m.Int32(0));
            m.Assign(p, m.Add(p, 4));
            m.Jump("loop");

            m.Label("done");
            m.Return();
            m.Procedure.RenumberBlocks();
            return m.Procedure;
        }

        private SsaIdentifier AddSid(string name)
        {
            Identifier id = new Identifier(name, -1, null, null);
            SsaIdentifier sid = sids.Add(id, new Statement(new DefInstruction(id), null), null, false);
            return sid;
        }
    }
}

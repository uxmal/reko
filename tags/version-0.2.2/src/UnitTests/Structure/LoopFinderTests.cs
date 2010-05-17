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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class LoopFinderTests : StructureTestBase
    {
        private ProcedureStructure proc;

        [Test]
        public void WhileGoto_TagNodes()
        {
            RunTest(new MockWhileGoto());
            StringWriter sw = new StringWriter();
            proc.Write(sw);
            Console.WriteLine(sw.ToString());
            Assert.AreEqual("LoopHead", proc.Ordering[6].Block.Name);
            Interval i = proc.DerivedGraphs[0].Intervals[1];
            Assert.AreEqual("LoopHead", i.Header.Name);

            LoopFinder lf = new LoopFinder(proc.Ordering[6], proc.Ordering[2], proc.Ordering);
            HashedSet<StructureNode> loopNodes = lf.FindNodesInLoop(i.FindIntervalNodes(0));
            Assert.AreEqual(3, loopNodes.Count);

        }

        [Test]
        public void CaseJumpsBack_LatchNode()
        {
            RunTest(new MockCaseJumpsBack());

        }

        [Test]
        public void Reg00013()
        {
            Program prog = RewriteProgram("Fragments/regressions/r00013.asm", new Address(0x800, 0));
            ProcedureStructureBuilder psb = new ProcedureStructureBuilder(prog.Procedures.Values[0]);
            proc = psb.Build();
            psb.AnalyzeGraph();

            LoopFinder lf = new LoopFinder(proc.Ordering[23], proc.Ordering[0], proc.Ordering);
            HashedSet<StructureNode> intervalNodes = proc.Nodes[23].Interval.FindIntervalNodes(0);
            HashedSet<StructureNode> loopNodes = lf.FindNodesInLoop(intervalNodes);
            proc.Dump();
            Loop loop = lf.DetermineLoopType(loopNodes);
            Assert.IsTrue(loop is TestlessLoop);
        }
        

        private void RunTest(ProcedureMock m)
        {
            m.Procedure.RenumberBlocks();
            ProcedureStructureBuilder psb = new ProcedureStructureBuilder(m.Procedure);
            proc = psb.Build();
        }
    }
}

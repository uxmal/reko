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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class DominatorGraphTests : AnalysisTestBase
    {
        [Test]
        [Ignore("scanning-development")]
        public void DgWhileGoto()
        {
			RunFileTest_x86_real("Fragments/while_goto.asm", "Analysis/DgWhileGoto.txt");
        }

        protected override void RunTest(Program program, TextWriter writer)
        {
            foreach (var proc in program.Procedures.Values)
            {
                writer.WriteLine("===========");
                proc.Write(false, writer);
                writer.WriteLine("== Predecessors");
                foreach (var block in proc.ControlGraph.Blocks.OrderBy(block => block.Name))
                {
                    writer.Write("  {0}:", block.Name);
                    foreach (var df in block.Pred)
                    {
                        writer.Write(" {0}", df.Name);
                    }
                    writer.WriteLine();
                }
                writer.WriteLine("== Immediate dominators");
                var gr = proc.CreateBlockDominatorGraph();
                gr.Write(writer);
                writer.WriteLine("== Dominance frontiers");
                foreach (var block in proc.ControlGraph.Blocks.OrderBy(block => block.Name))
                {
                    writer.Write("  {0}:", block.Name);
                    foreach (var df in gr.DominatorFrontier(block).OrderBy(b => b.Name))
                    {
                        writer.Write(" {0}", df.Name);
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}

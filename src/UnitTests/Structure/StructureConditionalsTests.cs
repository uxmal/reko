/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class StructureConditionalsTests
    {
        private string nl = Environment.NewLine;

        [Test]
        public void IfStatement()
        {
            string sExp = 
                "node 0: entry: \"CmpMock_entry\"" + nl +
                "    pred:" + nl +
                "    GraphType: SeqStructType" + nl +
                "    succ:  1" + nl +
                "node 1: entry: \"l1\"" + nl +
                "    pred: 0" + nl +
                "    GraphType: CondStructType" + nl +
                "    follow: 3" + nl + 
                "    succ:  2 3" + nl +
                "node 2: entry: \"l2\"" + nl +
                "    pred: 1" + nl +
                "    GraphType: SeqStructType" + nl +
                "    succ:  3" + nl +
                "node 3: entry: \"skip\"" + nl +
                "    pred: 1 2" + nl +
                "    GraphType: SeqStructType" + nl +
                "    succ:  4" + nl +
                "node 4: entry: \"CmpMock_exit\"" + nl +
                "    pred: 3" + nl +
                "    GraphType: SeqStructType" + nl +
                "    succ: " + nl;
            RunTest(new CmpMock(), sExp);
        }

        [Test]
        public void UnstructuredIfs()
        {
            string sExp =
"node 0: entry: \"UnstructuredIfsMock_entry\"" + nl +
"    pred:" + nl +
"    GraphType: SeqStructType" + nl +
"    succ:  1" + nl +
"node 1: entry: \"l1\"" + nl +
"    pred: 0" + nl +
"    GraphType: CondStructType" + nl +
"    follow: 4" + nl +
"    succ:  2 6" + nl +
"node 6: entry: \"then1\"" + nl +
"    pred: 1" + nl +
"    GraphType: CondStructType" + nl +
"    follow: 4" + nl +
"    succ:  7 3" + nl +
"node 7: entry: \"l2\"" + nl +
"    pred: 6" + nl +
"    GraphType: SeqStructType" + nl +
"    succ:  4" + nl +
"node 2: entry: \"else1\"" + nl +
"    pred: 1" + nl +
"    GraphType: SeqStructType" + nl +
"    succ:  3" + nl +
"node 3: entry: \"inside\"" + nl +
"    pred: 2 6" + nl +
"    GraphType: SeqStructType" + nl +
"    succ:  4" + nl +
"node 4: entry: \"done\"" + nl +
"    pred: 3 7" + nl +
"    GraphType: SeqStructType" + nl +
"    succ:  5" + nl +
"node 5: entry: \"UnstructuredIfsMock_exit\"" + nl +
"    pred: 4" + nl +
"    GraphType: SeqStructType" + nl +
"    succ: " + nl;
            RunTest(new UnstructuredIfsMock(), sExp);
        }

        private void RunTest(ProcedureMock pm, string sExp)
        {
            ProcedureStructure proc = new ProcedureStructure(pm.Procedure);
            StructureAnalysis sa = new StructureAnalysis(proc);
            sa.BuildDerivedSequences();
            PostDominatorGraph g = new PostDominatorGraph();
            g.FindImmediatePostDominators(proc);
            sa.StructureConditionals(g);
            StringWriter sw = new StringWriter();
            proc.Write(sw);
            string s = sw.ToString();
            if (s != sExp)
            {
                Console.WriteLine(Stringify(s));
            }
            Assert.AreEqual(sExp, s);
        }

        private string Stringify(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            string sep = "\" + nl +\n\"";
            
            for (int i = 0; i < s.Length; ++i)
            {
                char c = s[i];
                if (c == '\"')
                {
                    sb.Append("\\\"");
                }
                else if (c == '\r')
                {   
                    ;
                }
                else if (c == '\n')
                {
                    sb.Append(sep);
                }
                else
                    sb.Append(c);
            }
            sb.Append("\"");
            return sb.ToString();
        }
    }
}

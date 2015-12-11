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
using Reko.Analysis;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class RegisterPreservationTests
    {

        private void AssertProgram(string sExp, ProgramBuilder pb)
        {
            var sw = new StringWriter();
            pb.Program.Procedures.Values.First().Write(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
                Debug.WriteLine(sActual);
            Assert.AreEqual(sExp, sw.ToString());
        }

        public void RunTest(ProgramBuilder pb)
        {
            var program = pb.BuildProgram();
            foreach (var proc in program.Procedures.Values)
            {
                var flow = new ProgramDataFlow(program);
                Aliases alias = new Aliases(proc, program.Architecture, flow);
                alias.Transform();

                // Transform the procedure to SSA state. When encountering 'call' instructions,
                // they can be to functions already visited. If so, they have a "ProcedureFlow" 
                // associated with them. If they have not been visited, or are computed destinations
                // (e.g. vtables) they will have no "ProcedureFlow" associated with them yet, in
                // which case the the SSA treats the call as a "hell node".
                var doms = proc.CreateBlockDominatorGraph();
                var sst = new SsaTransform(flow, proc, doms);
                var ssa = sst.SsaState;
            }
        }
        [Test]
        public void Regp_Simple()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m =>
            {
                m.Return();
            });
            RunTest(pb);

            var sExp = "@@@";
            AssertProgram(sExp, pb);

        }
    }
}

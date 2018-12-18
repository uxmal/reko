
#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.Z80;
using Reko.Core;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class ProjectionPropagatorTests
    {
        private void RunTest(string sExp, IProcessorArchitecture arch, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder();
            builder(m);
            var prpr = new ProjectionPropagator(arch, m.Ssa);
            prpr.Transform();
            var sw = new StringWriter();
            m.Ssa.Procedure.WriteBody(false, sw);
            sw.Flush();
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                Assert.AreEqual(sExp, sActual);
            }
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        /// <summary>
        /// Fuse together subregisters that compose into a larger register
        /// </summary>
        [Test]
        public void Prjpr_Simple_Subregisters()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
	def hl
l1:
	de_1 = hl
	return
SsaProcedureBuilder_exit:
";
            #endregion

            var arch = new Z80ProcessorArchitecture("z80");
            RunTest(sExp, arch, m =>
            {
                var h = m.Reg("h", Registers.h); 
                var l = m.Reg("l", Registers.l);
                var de_1 = m.Reg("de_1", Registers.de);

                m.Def(h);
                m.Def(l);
                m.Assign(de_1, m.Seq(h, l));
                m.Return();
            });
        }
    }
}

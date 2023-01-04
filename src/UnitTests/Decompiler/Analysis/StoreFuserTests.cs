#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class StoreFuserTests
    {
        private FakeArchitecture arch;

        public StoreFuserTests()
        {
            this.arch = new FakeArchitecture(new ServiceContainer());
        }

        private void RunTest(string sExp, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder(arch);
            builder(m);

            var stfu = new StoreFuser(m.Ssa);
            stfu.Transform();

            var sw = new StringWriter();
            m.Ssa.Procedure.WriteBody(false, sw);
            sw.Flush();
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                m.Ssa.Dump(true);
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        [Test]
        [Ignore(Categories.AnalysisDevelopment)]
        public void Stfu_LittleEndian()
        {
            var sExp =
            #region Expected
@"SsaProcedureBuilder_entry:
l1:
	id = Mem3[0x123400<32>:word32]
	idLo = SLICE(id, word16, 0) (alias)
	ilHi = SLICE(id, word16, 16) (alias)
	Mem4[0x123404<32>:word32] = id
SsaProcedureBuilder_exit:
";
            #endregion
            RunTest(sExp, m =>
            {
                var id = m.Reg32("id");
                var idLo = m.Reg16("idLo");
                var idHi = m.Reg16("ilHi");

                m.Assign(id, m.Mem32(m.Word32(0x00123400)));
                m.Alias(idLo, m.Slice(id, idLo.DataType, 0));
                m.Alias(idHi, m.Slice(id, idHi.DataType, 16));
                m.MStore(m.Word32(0x00123404), idLo);
                m.MStore(m.Word32(0x00123406), idHi);
            });
        }
    }
}

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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Output;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    [Ignore("Wait and see if LLVM is good enough.")]
    public class JsonFormatterTests
    {
        private void RunTest(string sExp, string procName, Action<ProcedureBuilder> fn)
        {
            var pb = new ProcedureBuilder(procName);
            fn(pb);

            var sw = new StringWriter();
            var llvm = new JsonFormatter(sw);
            llvm.WriteProcedure(new KeyValuePair<Address, Procedure>(Address.Ptr32(0x123400), pb.Procedure));
            llvm.Flush();

            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                Debug.Print(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void JsonFmt_EmptyProc()
        {
            var sExp =
@"{ 
        name: 'empty'
        blocks: [
            { 
                'name': 'empty_entry'
                'succ': 'l1'
            },
            {
                'name': 'l1',
                'stms': [
                    [ 'ret' ]
                 ]
            }
        ]
     }
}
";
            RunTest(sExp, "empty", m =>
            {
                m.Return();
            });
        }

    }
}

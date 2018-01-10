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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Structure
{
    [TestFixture]
    public class ProcedurePrettifierTests
    {
        private void RunTest(string sExp, Action<AbsynCodeEmitter> gen)
        {
            var proc = new Procedure("test", new Frame(PrimitiveType.Pointer32));
            proc.Body = new List<AbsynStatement>();
            var m = new AbsynCodeEmitter(proc.Body);
            gen(m);
            var trrm = new ProcedurePrettifier(proc);
            trrm.Transform();
            var sw = new StringWriter();
            GenCode(proc, sw);
            if (sExp != sw.ToString())
            {
                Debug.Print("{0}", sw);
                Assert.AreEqual(sExp, sw.ToString());
            }
        }

        private void GenCode(Procedure proc, StringWriter sw)
        {
            sw.WriteLine("{0}()", proc.Name);
            sw.WriteLine("{");

            var cf = new CodeFormatter(new TextFormatter(sw) { UseTabs = false });
            cf.WriteStatementList(proc.Body);

            sw.WriteLine("}");
        }

        [Test]
        public void PP_EmptyThen()
        {
            var sExp =
            #region Expected
@"test()
{
    if (!test)
        test = true;
}
";
            #endregion
            var id = new Identifier("test", PrimitiveType.Bool, null);
            RunTest(sExp, m =>
            {
                m.If(id,
                    t =>
                    {

                    },
                    e =>
                    {
                        e.Assign(id, Constant.True());
                    });
            });
        }
    }
}

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
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.Structure;
using Reko.UnitTests.Mocks;
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
    public class TailReturnRemoverTests
    {
        private void RunTest(string sExp, Action<AbsynCodeEmitter> gen)
        {
            var arch = new FakeArchitecture();
            var proc = new Procedure(arch, "test", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
            proc.Body = new List<AbsynStatement>();
            var m = new AbsynCodeEmitter(proc.Body);
            gen(m);
            var trrm = new TailReturnRemover(proc);
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

            CodeFormatter cf = new CodeFormatter(new TextFormatter(sw) { UseTabs = false });
            cf.WriteStatementList(proc.Body);

            sw.WriteLine("}");
        }

        [Test]
        public void Trr_Simple()
        {
            var sExp =
            #region Expected
@"test()
{
}
";
#endregion
            RunTest(sExp, m =>
                m.Return());
        }

        [Test(Description = "Return statements not in tail position shouldn't be eliminated.")]
        public void Trr_NotTail()
        {
            var sExp =
            #region Expected
@"test()
{
    if (id)
        return;
    fn();
}
";
            #endregion
            RunTest(sExp, m =>
            {
                var id = new Identifier("id", PrimitiveType.Bool, null);
                var fn = new Identifier("fn", PrimitiveType.Ptr32, null);
                m.If(id, t =>
                {
                    t.Return();
                });
                m.SideEffect(m.Fn(fn));
                m.Return();
            });
        }

        [Test(Description = "Clean up multiple redundant return statements")]
        public void Trr_MultipleReturns()
        {
            var sExp =
            #region Expected
@"test()
{
    fn();
}
";
            #endregion
            RunTest(sExp, m =>
            {
                var fn = new Identifier("fn", PrimitiveType.Ptr32, null);
                m.SideEffect(m.Fn(fn));
                m.Return();
                m.Return();
                m.Return();
            });

        }

        [Test(Description = "Clean up multiple redundant return statements")]
        public void Trr_IfThenBranch()
        {
            var sExp =
            #region Expected
@"test()
{
    if (id)
        fn();
}
";
            #endregion
            RunTest(sExp, m =>
            {
                var id = new Identifier("id", PrimitiveType.Bool, null);
                var fn = new Identifier("fn", PrimitiveType.Ptr32, null);
                m.If(id, t =>
                {
                    t.SideEffect(m.Fn(fn));
                    t.Return();
                });
                m.Return();
            });
        }
    }
}

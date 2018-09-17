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
    public class ForLoopRewriterTests
    {
        private void RunTest(string sExp, Action<AbsynCodeEmitter> gen)
        {
            var proc = new Procedure(null, "test", new Frame(PrimitiveType.Ptr32));
            proc.Body = new List<AbsynStatement>();
            var m = new AbsynCodeEmitter(proc.Body);
            gen(m);
            var flr = new ForLoopRewriter(proc);
            flr.Transform();
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

        private Identifier Id(string name, DataType dt)
        {
            var tmp = new TemporaryStorage(name, -42, dt);
            return new Identifier(name, dt, tmp);
        }

        [Test]
        public void Flr_While()
        {
            var sExp =
            #region Expected
@"test()
{
    for (i = 0; i <= 42; i = i + 1)
        foo(i);
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var foo = Id("foo", PrimitiveType.Ptr32);
                m.Assign(i, m.Int32(0));
                m.While(m.Le(i, 42), b =>
                {
                    b.SideEffect(b.Fn(foo, i));
                    b.Assign(i, b.IAdd(i, 1));
                });
            });
        }

        [Test]
        public void Flr_WhileWithPrematureExit()
        {
            var sExp =
            #region Expected
@"test()
{
    int32 limit = isqrt(x) + 1;
    for (i = 2; i < limit; i = i + 1)
    {
        if (x % i == 0)
            return 0;
    }
    return 1;
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var x = Id("x", PrimitiveType.Int32);
                var isqrt = Id("isqrt", PrimitiveType.Ptr32);
                var limit = Id("limit", PrimitiveType.Int32);
                m.Declare(limit, m.IAdd(m.Fn(isqrt, x), m.Int32(1)));
                m.Assign(i, m.Int32(2));
                m.While(m.Lt(i, limit), w =>
                {
                    w.If(w.Eq0(m.Mod(x, i)), wif =>
                    {
                        wif.Return(m.Int32(0));
                    });
                    w.Assign(i, w.IAdd(i, w.Int32(1)));
                });
                m.Return(m.Int32(1));
            });
        }
    }
}

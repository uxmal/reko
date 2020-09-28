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
    public class ForLoopRewriterTests
    {
        private void RunTest(string sExp, Action<AbsynCodeEmitter> gen)
        {
            var proc = new Procedure(new FakeArchitecture(), "test", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
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
                    b.Assign(i, b.IAddS(i, 1));
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
                m.Declare(limit, m.IAddS(m.Fn(isqrt, x), 1));
                m.Assign(i, m.Int32(2));
                m.While(m.Lt(i, limit), w =>
                {
                    w.If(w.Eq0(m.Mod(x, i)), wif =>
                    {
                        wif.Return(m.Int32(0));
                    });
                    w.Assign(i, w.IAddS(i, 1));
                });
                m.Return(m.Int32(1));
            });
        }

        /// <summary>
        /// If the loop increment doesn't postdominate the loop body statements,
        /// don't rewrite.
        /// </summary>
        [Test]
        public void Flr_WhileWithPrematureExit_DontRewrite()
        {
            var sExp =
            #region Expected
@"test()
{
    int32 limit = isqrt(x) + 1;
    i = 2;
    while (i < limit)
    {
        i = i + 1;
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
                m.Declare(limit, m.IAddS(m.Fn(isqrt, x), 1));
                m.Assign(i, m.Int32(2));
                m.While(m.Lt(i, limit), w =>
                {
                    w.Assign(i, w.IAddS(i, 1));
                    w.If(w.Eq0(m.Mod(x, i)), wif =>
                    {
                        wif.Return(m.Int32(0));
                    });
                });
                m.Return(m.Int32(1));
            });
        }

        /// <summary>
        /// If there is a use of the loop variable after it is incremented,
        /// don't rewrite.
        /// </summary>
        [Test]
        public void Flr_WhileWithUsesAfterIncrement_DontRewrite()
        {
            var sExp =
            #region Expected
@"test()
{
    i = 2;
    while (i < limit)
    {
        i = i + 1;
        sum = i;
    }
    return 1;
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var x = Id("x", PrimitiveType.Int32);
                var sum = Id("sum", PrimitiveType.Int32);
                var isqrt = Id("isqrt", PrimitiveType.Ptr32);
                var limit = Id("limit", PrimitiveType.Int32);
                m.Assign(i, m.Int32(2));
                m.While(m.Lt(i, limit), w =>
                {
                    w.Assign(i, w.IAddS(i, 1));
                    w.Assign(sum, i);       // uses i after it was incremented.
                });
                m.Return(m.Int32(1));
            });
        }

        /// <summary>
        /// If the loop variable is used between its initialization and the 
        /// is a use of the loop variable after it is incremented,
        /// don't rewrite.
        /// </summary>
        [Test]
        public void Flr_InitializerUsedOutsideLoop()
        {
            var sExp =
            #region Expected
@"test()
{
    i = 2;
    a = i;
    for (; i < limit; i = i + 1)
        foo(i);
    return 1;
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var a = Id("a", PrimitiveType.Int32);
                var sum = Id("sum", PrimitiveType.Int32);
                var foo = Id("foo", PrimitiveType.Ptr32);
                var limit = Id("limit", PrimitiveType.Int32);
                m.Assign(i, m.Int32(2));
                m.Assign(a, i);
                m.While(m.Lt(i, limit), w =>
                {
                    w.SideEffect(m.Fn(foo, i));
                    w.Assign(i, m.IAddS(i, 1));
                });
                m.Return(m.Int32(1));
            });
        }

        /// <summary>
        /// If the loop variable is used between its initialization and the 
        /// is a use of the loop variable after it is incremented,
        /// don't rewrite.
        /// </summary>
        [Test]
        public void Flr_InitializedByDeclaration()
        {
            var sExp =
            #region Expected
@"test()
{
    int32 i;
    for (i = 2; i < limit; i = i + 1)
        foo(i);
    return 1;
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var sum = Id("sum", PrimitiveType.Int32);
                var foo = Id("foo", PrimitiveType.Ptr32);
                var limit = Id("limit", PrimitiveType.Int32);
                m.Declare(i, m.Int32(2));
                m.While(m.Lt(i, limit), w =>
                {
                    w.SideEffect(m.Fn(foo, i));
                    w.Assign(i, m.IAddS(i, 1));
                });
                m.Return(m.Int32(1));
            });
        }

        [Test]
        public void Flr_SideEffectBlocksInitializer()
        {
            var sExp =
            #region Expected
@"test()
{
    i = 0;
    foo();
    for (; i < n; i = i + 1)
        ;
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var n = Id("n", PrimitiveType.Int32);
                var foo = Id("foo", PrimitiveType.Ptr32);

                m.Assign(i, m.Int32(0));
                m.SideEffect(m.Fn(foo));
                m.While(m.Lt(i, n), w =>
                {
                    w.Assign(i, w.IAddS(i, 1));
                });
            });
            /*i = 0;
            foo();
            while(i < n)
            {
            i++;
            }
            can not be rewritten to For.
            */
        }

        [Test]
        public void Flr_SideEffectBlocksUpdate()
        {
            var sExp =
            #region Expected
@"test()
{
    i = 0;
    while (i < n)
    {
        i = i + 1;
        foo();
    }
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var n = Id("n", PrimitiveType.Int32);
                var foo = Id("foo", PrimitiveType.Ptr32);

                m.Assign(i, m.Int32(0));
                m.While(m.Lt(i, n), w =>
                {
                    w.Assign(i, w.IAddS(i, 1));
                    w.SideEffect(w.Fn(foo));
                });
            });
        }

        [Test]
        public void Flr_DoWhile_CantProveLoopTraversedOnce()
        {
            var sExp =
            #region Expected
@"test()
{
    i = 0;
    do
    {
        foo();
        i = i + 1;
    } while (i < n);
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var n = Id("n", PrimitiveType.Int32);
                var foo = Id("foo", PrimitiveType.Ptr32);

                m.Assign(i, m.Int32(0));
                m.DoWhile(w =>
                    {
                        w.SideEffect(w.Fn(foo));
                        w.Assign(i, w.IAddS(i, 1));
                    },
                    m.Lt(i, n));
            });
        }

        [Test]
        public void Flr_DoWhile_ProveLoopTraversedOnce()
        {
            var sExp =
            #region Expected
@"test()
{
    for (i = 0; i != 42; i = i + 1)
        foo();
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var n = Id("n", PrimitiveType.Int32);
                var foo = Id("foo", PrimitiveType.Ptr32);

                m.Assign(i, m.Int32(0));
                m.DoWhile(w =>
                {
                    w.SideEffect(w.Fn(foo));
                    w.Assign(i, w.IAddS(i, 1));
                },
                    m.Ne(i, m.Int32(42)));
            });
        }

        [Test]
        public void Flr_RewriteIfGuardedDoWhile_Type1()
        {
            var sExp =
            #region Expected
@"test()
{
    for (; i != n; i = i + 1)
        foo(i);
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var n = Id("n", PrimitiveType.Int32);
                var foo = Id("foo", PrimitiveType.Ptr32);

                m.If(m.Ne(i, n), t =>
                {
                    t.DoWhile(d =>
                    {
                        d.SideEffect(d.Fn(foo, i));
                        d.Assign(i, d.IAddS(i, 1));
                    },
                    t.Ne(i, n));
                });
            });
        }

        [Test]
        public void Flr_RewriteIfGuardedDoWhile_Type2()
        {
            var sExp =
            #region Expected
@"test()
{
    for (i = 0; i < ptr.Limit; i = i + 1)
        foo(i);
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var ptr = Id("ptr", PrimitiveType.Ptr32);
                var foo = Id("foo", PrimitiveType.Ptr32);
                var field = new StructureField(4, PrimitiveType.Int32, "Limit");
                var complex = m.Field(PrimitiveType.Int32, ptr, field);

                m.Assign(i, m.Int32(0));
                m.If(m.Ne0(complex), t =>
                {
                    t.DoWhile(d =>
                    {
                        d.SideEffect(d.Fn(foo, i));
                        d.Assign(i, d.IAddS(i, 1));
                    },
                    t.Gt(complex, i));
                });
            });
        }

        [Test]
        public void Flr_CastingLoopVariable()
        {
            var sExp =
            #region Expected
@"test()
{
    for (i = 0; (real32) i <= arg; i = i + 1)
        foo(i);
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var i = Id("i", PrimitiveType.Int32);
                var arg = Id("arg", PrimitiveType.Real32);
                var foo = Id("foo", PrimitiveType.Ptr32);
                m.Assign(i, m.Int32(0));
                m.While(m.Le(m.Cast(PrimitiveType.Real32, i), arg), b =>
                {
                    b.SideEffect(b.Fn(foo, i));
                    b.Assign(i, b.IAddS(i, 1));
                });
            });
        }
    }
}

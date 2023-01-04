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

namespace Reko.UnitTests.Decompiler.Structure
{
    [TestFixture]
    public class IfCascadeToSwitchRewriterTests
    {
        private void RunTest(string sExp, Action<AbsynCodeEmitter> gen)
        {
            var arch = new FakeArchitecture();
            var proc = new Procedure(arch, "test", Address.Ptr32(0x00123400), arch.CreateFrame());
            proc.Body = new List<AbsynStatement>();
            var m = new AbsynCodeEmitter(proc.Body);
            gen(m);
            var ifsw = new IfCascadeToSwitchRewriter(proc);
            ifsw.Transform();
            var sw = new StringWriter();
            GenCode(proc, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void GenCode(Procedure proc, StringWriter sw)
        {
            sw.WriteLine("");
            sw.WriteLine("{0}()", proc.Name);
            sw.WriteLine("{");

            var cf = new AbsynCodeFormatter(new TextFormatter(sw) { UseTabs = false });
            cf.WriteStatementList(proc.Body);

            sw.WriteLine("}");
        }

        [Test]
        public void Ifsw_ThreeNeCases()
        {
            var sExp =
            #region Expected
@"
test()
{
    switch (a)
    {
    case 0x01:
        return 0x01;
    case 0x02:
        return 0x02;
    case 0x03:
        return 0x03;
    default:
        return 0x00;
    }
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var a = Identifier.Create(new RegisterStorage("a", 0, 0, PrimitiveType.Word32));
                m.If(m.Ne(a, 1), t =>
                {
                    t.If(t.Ne(a, 2), t =>
                    {
                        t.If(t.Ne(a, 3), t =>
                        {
                            t.Return(t.Word32(0));
                        }, e =>
                        {
                            e.Return(e.Word32(3));
                        });
                    }, e =>
                    {
                        e.Return(e.Word32(2));
                    });
                }, e =>
                {
                    e.Return(e.Word32(1));
                });
            });
        }

        [Test]
        public void Ifsw_ThreeEqCases()
        {
            var sExp =
            #region Expected
@"
test()
{
    switch (a)
    {
    case 0x01:
        return 0x01;
    case 0x02:
        return 0x02;
    case 0x03:
        return 0x03;
    default:
        return 0x00;
    }
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var a = Identifier.Create(new RegisterStorage("a", 0, 0, PrimitiveType.Word32));
                m.If(m.Eq(a, 1), t =>
                {
                    t.Return(t.Word32(1));
                }, e =>
                {
                    e.If(e.Eq(a, 2), t =>
                    {
                        t.Return(t.Word32(2));
                    }, e =>
                    {
                        e.If(e.Eq(a, 3), t =>
                        {
                            t.Return(t.Word32(3));
                        }, e =>
                        {
                            e.Return(e.Word32(0));
                        });
                    });
                });
            });
        }

        [Test]
        public void Ifsw_EmptyDefault()
        {
            var sExp =
            #region
                @"
test()
{
    switch (r3)
    {
    case 0:
        *r2 = 64;
        break;
    case 1:
        *r2 = 66;
        break;
    }
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var r2 = Identifier.Create(new RegisterStorage("r2", 2, 0, PrimitiveType.Int32));
                var r3 = Identifier.Create(new RegisterStorage("r3", 3, 0, PrimitiveType.Int32));
                m.If(m.Ne(r3, 0), t =>
                {
                    t.If(t.Eq(r3, 1), t =>
                    {
                        t.Assign(t.Deref(r2), t.Int32(0x42));
                    });
                }, e =>
                {
                    e.Assign(m.Deref(r2), e.Int32(0x40));
                });
            });
        }

        [Test]
        public void Ifsw_LogicalOr()
        {
            var sExp =
            #region
                @"
test()
{
    switch (r3)
    {
    case 0:
        *r2 = 64;
        break;
    case 1:
    case 4:
        *r2 = 66;
        break;
    }
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var r2 = Identifier.Create(new RegisterStorage("r2", 2, 0, PrimitiveType.Int32));
                var r3 = Identifier.Create(new RegisterStorage("r3", 3, 0, PrimitiveType.Int32));
                m.If(m.Ne(r3, 0), t =>
                {
                    t.If(m.Cor(t.Eq(r3, 1), t.Eq(r3, 4)), t =>
                    {
                        t.Assign(t.Deref(r2), t.Int32(0x42));
                    });
                }, e =>
                {
                    e.Assign(m.Deref(r2), e.Int32(0x40));
                });
            });
        }

        [Test]
        public void Ifsw_LogicalAnd()
        {
            var sExp =
            #region
@"
test()
{
    switch (r3)
    {
    case 0:
    case 1:
    case 2:
        *r2 = 64;
        break;
    case 4:
        *r2 = 66;
        break;
    }
}
";
            #endregion

            RunTest(sExp, m =>
            {
                var r2 = Identifier.Create(new RegisterStorage("r2", 2, 0, PrimitiveType.Int32));
                var r3 = Identifier.Create(new RegisterStorage("r3", 3, 0, PrimitiveType.Int32));
                m.If(m.Cand(m.Cand(m.Ne(r3, 0), m.Ne(r3, 1)), m.Ne(r3, 2)), t =>
                {
                    t.If(t.Eq(r3, 4), t =>
                    {
                        t.Assign(t.Deref(r2), t.Int32(0x42));
                    });
                }, e =>
                {
                    e.Assign(m.Deref(r2), e.Int32(0x40));
                });
            });
        }
    }
}

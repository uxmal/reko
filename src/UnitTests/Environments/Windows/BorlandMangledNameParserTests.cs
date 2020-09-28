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
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class BorlandMangledNameParserTests
    {
        private void RunTest(string expected, string parse)
        {
            var p = new BorlandMangledNamedParser(parse);
            var sp = p.Parse();
            var sb = new StringBuilder();
            Assert.AreEqual(expected, new TestSerializedTypeRenderer(sb).Render(p.Modifier, p.Scope, sp.Item1, sp.Item2));
        }

        [Test]
        public void Bmnp_Method()
        {
            RunTest(
                "foo::myfunc(anotherClass *)",
                "@foo@myfunc$qn12anotherClass");
        }

        [Test]
        public void Bmnp_intArg()
        {
            RunTest(
                "foo(int16_t)",
                "@foo$qi");
        }

        [Test]
        public void Bmnp_NoArg()
        {
            RunTest(
                "foo()",
                "@foo$qv");
        }

        // There are binaries with truncated symbols; rather than die we do a best effort. Users can always always override this
        // by providing complete external metadata.
        [Test]
        public void Bmnp_TruncatedName()
        {
            RunTest(
                "foo::bar(Point ^, FrobnicatorApp_truncated *)",
                "@foo@0bar$qmx5Pointn17FrobnicatorApp");
        }

        // Sometimes a mangled argument type refers to a previous argument type.
        [Test]
        public void Bmnp_BackReference()
        {
            RunTest(
                "foo::bar(int16_t, Point *, Point *)",
                "@foo@bar$qin5Pointt2");
        }

        [Test]
        public void Bmnp_SignedChar()
        {
            RunTest(
              "foo::bar(int8_t *)",
              "@foo@bar$qnzc");
        }

        [Test]
        public void Bmnp_Specials_dtor()
        {
            RunTest("State::~State()", "@State@0$bdtr$qv");
        }

        [Test]
        public void Bmnp_Unsigned_short()
        {
            RunTest("Fnord::DoState(uint16_t)", "@Fnord@0DoState$qus");
        }
    }
}

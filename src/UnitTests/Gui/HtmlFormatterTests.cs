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

using Reko.Core.Output;
using Reko.Gui;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    public class HtmlFormatterTests
    {
        private StringWriter sb;
        private HtmlFormatter hf;

        [SetUp]
        public void Setup()
        {
            sb = new StringWriter();
            hf = new HtmlFormatter(sb);
        }

        [Test]
        public void WriteKeyword()
        {
            hf.WriteKeyword("char");
            Assert.AreEqual("<span class=\"kw\">char</span>",sb.ToString());
        }

        [Test]
        public void WriteEscapedCharacters()
        {
            hf.Write("a = (b < c ? 3 : d->foo & 0xFF);");
            Assert.AreEqual("a&nbsp;=&nbsp;(b&nbsp;&lt;&nbsp;c&nbsp;?&nbsp;3&nbsp;:&nbsp;d-&gt;foo&nbsp;&amp;&nbsp;0xFF);", sb.ToString());
        }

        [Test]
        public void EscapeFormatString()
        {
            hf.Write("a = {0}->{1}", "b->c", "d & 3");
            Assert.AreEqual("a&nbsp;=&nbsp;b-&gt;c-&gt;d&nbsp;&amp;&nbsp;3", sb.ToString());
        }

        [Test]
        public void Newline()
        {
            hf.Write("line1");
            hf.WriteLine();
            hf.Write("line2");
            Assert.AreEqual("line1<br />"+Environment.NewLine+"line2", sb.ToString());
        }

        [Test]
        public void WriteHyperlink()
        {
            hf.WriteHyperlink("Click <me>", "foo(\"bar\")");
            Assert.AreEqual("<a href=\"foo(&quot;bar&quot;)\">Click&nbsp;&lt;me&gt;</a>", sb.ToString());
        }
    }
}

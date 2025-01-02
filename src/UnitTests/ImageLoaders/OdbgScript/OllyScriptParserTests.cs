#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.ImageLoaders.OdbgScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.OdbgScript
{
    [TestFixture]
    public class OllyScriptParserTests
    {
        private OllyScriptParser parser;
        private Mock<IOdbgScriptHost> host;
        private Mock<IFileSystemService> fsSvc;

        [SetUp]
        public void Setup()
        {
            this.parser = null;
            this.host = new Mock<IOdbgScriptHost>();
            this.fsSvc = new Mock<IFileSystemService>();
        }

        private void Given_Parser(string script)
        {
            this.parser = new OllyScriptParser(new StringReader(script), "test", host.Object, fsSvc.Object);
        }

        [Test]
        public void Osp_Lex_Label()
        {
            Given_Parser("foo:");
            var script = parser.ParseScript();
            Assert.AreEqual(1, script.Lines.Count);
            Assert.AreEqual(0, script.Labels["foo"]);
        }

        [Test]
        public void Osp_Lex_EmptyLine()
        {
            Given_Parser("\n");
            var script = parser.ParseScript();
            Assert.AreEqual(1, script.Lines.Count);
            Assert.AreEqual(0, script.Labels.Count);
        }

        [Test]
        public void Osp_MultilineComment()
        {
            Given_Parser(
@"cmd /* ignore three lines
1
2
*/
cmd2
");
            var script = parser.ParseScript();
            Assert.AreEqual("cmd", script.Lines[0].Command);
            Assert.AreEqual(0, script.Lines[0].Args.Length);
            Assert.AreEqual("cmd2", script.Lines[4].Command);
            Assert.AreEqual(5, script.Lines.Count);
        }

        [Test]
        public void Osp_HexString()
        {
            Given_Parser("foo #2A??3B#");
            var script = parser.ParseScript();
            Assert.AreEqual("1: foo HexString(\"2A??3B\")", script.Lines[0].ToString());
        }

        [Test]
        public void Osp_Interpolated()
        {
            Given_Parser("foo $\"interpolated {var}\"");
            var script = parser.ParseScript();
            Assert.AreEqual("1: foo Interpolate(\"interpolated {var}\")", script.Lines[0].ToString());
        }

        [Test]
        public void Osp_SegmentedAddress()
        {
            Given_Parser("foo 0B00:0C00");
            var script = parser.ParseScript();
            Assert.AreEqual("1: foo SEQ(0xB00<u64>, 0xC00<u64>)", script.Lines[0].ToString());
        }
    }
}

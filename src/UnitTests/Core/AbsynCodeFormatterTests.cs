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
using Reko.Core.Expressions;
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class AbsynCodeFormatterTests
    {
        [Test]
        public void Acf_HexEntropy()
        {
            var c = Constant.Int32(0x7FF);
            var sw = new StringWriter();
            var acf = new AbsynCodeFormatter(new TextFormatter(sw));
            c.Accept(acf);
            Assert.AreEqual("0x07FF", sw.ToString());
        }

        [Test]
        public void Acf_DecEntropy()
        {
            var c = Constant.Int32(1000);
            var sw = new StringWriter();
            var acf = new AbsynCodeFormatter(new TextFormatter(sw));
            c.Accept(acf);
            Assert.AreEqual("1000", sw.ToString());
        }

        [Test]
        public void Acf_SmallInteger()
        {
            var c = Constant.Int32(10);
            var sw = new StringWriter();
            var acf = new AbsynCodeFormatter(new TextFormatter(sw));
            c.Accept(acf);
            Assert.AreEqual("0x0A", sw.ToString());
        }

        [Test]
        public void Acf_EvenPower()
        {
            var c = Constant.UInt32(unchecked((uint)-256));
            var sw = new StringWriter();
            var acf = new AbsynCodeFormatter(new TextFormatter(sw));
            c.Accept(acf);
            Assert.AreEqual("~0xFF", sw.ToString());
        }

        [Test]
        public void Acf_NegativeSigned()
        {
            var c = Constant.Int32(-256);
            var sw = new StringWriter();
            var acf = new AbsynCodeFormatter(new TextFormatter(sw));
            c.Accept(acf);
            Assert.AreEqual("-0x0100", sw.ToString());
        }

        [Test]
        public void Acf_NegativeSigned_SmallInt()
        {
            var c = Constant.Word32(-4);
            var sw = new StringWriter();
            var acf = new AbsynCodeFormatter(new TextFormatter(sw));
            c.Accept(acf);
            Assert.AreEqual("~0x03", sw.ToString());
        }

        [Test]
        public void Acf_Reg00001()
        {
            var c = Constant.Word32(0x00001000);
            var sw = new StringWriter();
            var acf = new AbsynCodeFormatter(new TextFormatter(sw));
            c.Accept(acf);
            Assert.AreEqual("0x1000", sw.ToString());
        }
    }
}

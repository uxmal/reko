#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

#if DEBUG
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.Win32
{
    [TestFixture]
    public class MsPrintfFormatParserTests
    {
        private  MsPrintfFormatParser parser;

        private void ParseChar32(string formatString)
        {
            this.parser = new MsPrintfFormatParser(formatString, false, 4, 4, 4);
            parser.Parse();
        }

        [Test]
        public void MPFP_Empty()
        {
            ParseChar32("");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void MPFP_NoFormat()
        {
            ParseChar32("Hello world");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void MPFP_LiteralPercent()
        {
            ParseChar32("H%%ello world");
            Assert.AreEqual(0, parser.ArgumentTypes.Count);
        }

        [Test]
        public void MPFP_32_Decimal()
        {
            ParseChar32("Total: %d");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int32", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void MPFP_32_Char()
        {
            ParseChar32("'%c'");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("char", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void MPFP_32_TwoFormats()
        {
            ParseChar32("%c%x");
            Assert.AreEqual(2, parser.ArgumentTypes.Count);
            Assert.AreEqual("char", parser.ArgumentTypes[0].ToString());
            Assert.AreEqual("uint32", parser.ArgumentTypes[1].ToString());

        }

        [Test]
        public void MPFP_32_Short()
        {
            ParseChar32("%hd");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int16", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void MPFP_32_I64()
        {
            ParseChar32("%I64x");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("uint64", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void MPFP_32_longlong()
        {
            ParseChar32("%lli");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("int64", parser.ArgumentTypes[0].ToString());
        }

        [Test]
        public void MPFP_32_Pointer()
        {
            ParseChar32("%08p");
            Assert.AreEqual(1, parser.ArgumentTypes.Count);
            Assert.AreEqual("ptr32", parser.ArgumentTypes[0].ToString());
        }
    }
}
#endif
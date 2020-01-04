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
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class PrettyPrinterTests
    {
        private StringWriter sw;
        private PrettyPrinter pp;
        private const char ESCAPE = '$';
        private readonly string nl = Environment.NewLine;

        [SetUp]
        public void Setup()
        {
            sw = new StringWriter();
        }

        [Test]
        public void SmallString()
        {
            pp = new PrettyPrinter(sw, 4);
            PrettyPrint("hi");
            Assert.AreEqual("hi", sw.ToString());
        }

        [Test]
        public void ShortStringWithConnectedBreaks()
        {
            pp = new PrettyPrinter(sw, 4);
            PrettyPrint("a$cb");
            Assert.AreEqual("ab", sw.ToString());
        }

        [Test]
        public void LongStringWithConnectedBreaks()
        {
            pp = new PrettyPrinter(sw, 4);
            PrettyPrint("a$cb$cc$cd$ce");
            Assert.AreEqual("a" + nl + "b" + nl + "c" + nl + "d" + nl + "e", sw.ToString());
        }

        [Test]
        public void LongStringWithOptionalBreaks()
        {
            pp = new PrettyPrinter(sw, 4);
            PrettyPrint("a$ob$oc$od$oe");
            Assert.AreEqual("abcd" + nl + "e", sw.ToString());
        }

        [Test]
        public void Indent()
        {
            pp = new PrettyPrinter(sw, 4);
            PrettyPrint("a$tb$oc$od$oe$of$og");
            Assert.AreEqual("abcd" + nl + "  ef" + nl + "  g", sw.ToString());
        }

        private void PrettyPrint(string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == ESCAPE && i < str.Length - 1)
                {
                    ++i;
                    switch (str[i])
                    {
                    case '{': 
                        pp.BeginGroup();
                        break;
                    case '}': 
                        pp.EndGroup();
                        break;
                    case 't': 
                        pp.Indent(2);
                        break;
                    case 'b':   
                        pp.Outdent(2);
                        break;
                    case 'n':  
                        pp.ForceLineBreak();
                        break;
                    case 'o':
                        pp.OptionalLineBreak();
                        break;
                    case 'c':
                        pp.ConnectedLineBreak();
                        break;
                    }
                }
                else
                {
                    char ch = str[i];
                    pp.PrintCharacter(ch);
                }
            }
            pp.Flush();
        }

    }
}

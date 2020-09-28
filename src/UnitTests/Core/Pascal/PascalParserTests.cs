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
using Reko.Core.Pascal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Pascal
{

    [TestFixture]
    public class PascalParserTests
    {
        private PascalParser parser;

        private void Given_Parser(string src)
        {
            var lexer = new PascalLexer(new StringReader(src));
            this.parser = new PascalParser(lexer);
        }


        [Test]
        public void PParser_Function_WithInline()
        {
            var src = "my_unit; interface FuNction foo(quux : ^Integer; var bar : Integer) : boolean; INLINE $BADD,$FACE; end.";
            Given_Parser(src);
            var decls = parser.ParseUnit();
            Assert.AreEqual("function foo(quux : integer^; var bar : integer) : boolean; inline $BADD, $FACE", decls[0].ToString());
        }


        [Test]
        public void PParser_Variant_Record()
        {
            var src =
@"RECORD
    CASE INTEGER OF
    1:
        (top: INTEGER;
         left: INTEGER;
         bottom: INTEGER;
         right: INTEGER);
    2:
       (topLeft: Point;
        botRight: Point);
END";
            Given_Parser(src);
            var record = parser.ParseType();
            Assert.AreEqual(
                "record case integer of " +
                    "1 : (top : integer;left : integer;bottom : integer;right : integer); " +
                    "2 : (topLeft : Point;botRight : Point) end",
                record.ToString());
        }

        [Test]
        public void PParser_Variant_EnumTag()
        {
            var src =
@"RECORD
case AEArrayType OF
kAEDataArray: 
    (AEDataArray: Array[0..0] OF Integer);
END";
            Given_Parser(src);
            var record = parser.ParseType();
            Assert.AreEqual(
                "record case AEArrayType of " +
                    "kAEDataArray : (AEDataArray : array[0..0]) " +
                "end",
                record.ToString());
        }

        [Test]
        public void PParser_Record_FieldsWithSameType()
        {
            var src =
@"RECORD
    a,b : INTEGER
END";
            Given_Parser(src);
            var record = parser.ParseType();
            Assert.AreEqual(
                "record " +
                    "a, b : integer " +
                "end",
                record.ToString());
        }
    }
}

#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Hll.Pascal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Hll.Pascal
{

    [TestFixture]
    public class PascalParserTests
    {
        private PascalParser parser;

        private void Given_Parser(string src)
        {
            var lexer = new PascalLexer(new StringReader(src), true);
            this.parser = new PascalParser(lexer);
        }
        private void Given_ParserNoNesting(string src)
        {
            var lexer = new PascalLexer(new StringReader(src), false);
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
        public void PParser_Function_WithCdecl()
        {
            var src = "my_unit; interface FUNCTION myFn({CONST}VAR theAEDesc: AEDesc): Size; C; end.";
            Given_Parser(src);
            var decls = parser.ParseUnit();
            Assert.AreEqual("function myFn(var theAEDesc : AEDesc) : Size; C", decls[0].ToString());
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

        [Test]
        public void PParser_ProcedurePtr_Type_Definition()
        {
            var src =
@"PROCEDURE(sourceCCB: TPCCB);
";
            Given_Parser(src);
            var procPtr = parser.ParseType();
            Assert.AreEqual("procedure(sourceCCB : TPCCB)", procPtr.ToString());
        }

        [Test]
        public void PParser_FunctionPtr_Type_Definition()
        {
            var src =
            "FUNCTION(typeCode: DescType; dataPtr: UNIV Ptr; dataSize: Size; toType: DescType; handlerRefcon: LONGINT; VAR result: AEDesc): OSErr";
            Given_Parser(src);
            var funcPtr = parser.ParseType();
            Assert.AreEqual("function(typeCode : DescType; dataPtr : Ptr; dataSize : Size; toType : DescType; handlerRefcon : longint; var result : AEDesc) : OSErr", funcPtr.ToString());
        }

        [Test]
        public void PParser_Div_Expression()
        {
            var src ="d/200";
            Given_Parser(src);
            var exp = parser.ParseExp();
            Assert.AreEqual("(d / 200)", exp.ToString());
        }

        [Test]
        public void PParser_C_Style_Ellipsis()
        {
            var src = 
@" myunit; 
interface 
PROCEDURE printf(format: Str; ...); C;
end.";
            Given_Parser(src);
            var decls = parser.ParseUnit();
            Assert.AreEqual("procedure printf(format : Str; ...); C", decls[0].ToString());
        }

        [Test]
        public void PParser_nested_comments()
        {
            var src =
@"myunit;
interface 
const c = $0001; { (1 << 0); (* nest *) }
end.";

            Given_Parser(src);
            var decls = parser.ParseUnit();
            Assert.AreEqual("const c = 1", decls[0].ToString());
        }

        [Test]
        public void PParser_comment_match_delimiters()
        {
            var src =
@"
myunit; interface
const
    a = 0;   (* simple *)
    b = 42;     { (thing *) } 
    c = $7E;   (* ASCII code for '}' *)
end.
";
            Given_Parser(src);
            var decls = parser.ParseUnit();
            Assert.AreEqual("const a = 0", decls[0].ToString());
            Assert.AreEqual("const b = 42", decls[1].ToString());
            Assert.AreEqual("const c = 126", decls[2].ToString());
        }

        [Test]
        public void PParser_comment_no_nesting()
        {
            var src =
@"
myunit; interface
const
    a = 0;   { (* simple *) }
    b = 42;     { (thing *) } 
end.
";
            Given_ParserNoNesting(src);
            var decls = parser.ParseUnit();
            Assert.AreEqual("const a = 0", decls[0].ToString());
            Assert.AreEqual("const b = 42", decls[1].ToString());
        }

        [Test]
        public void PParser_Parenthesized_Exp()
        {
            Given_Parser("(a + b)");
            var e = parser.ParseExp();
            Assert.AreEqual("(a + b)", e.ToString());
        }

        [Test]
        public void PParser_Constant_with_type()
        {
            var src = 
@"myunit; interface 
CONST pi : double_t = 3.1415926535;
end.";
            var culture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("ru");
                Given_Parser(src);
                var decls = parser.ParseUnit();
                Assert.AreEqual("const pi : double_t = 3.1415926535", decls[0].ToString());
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
            }
        }

        [Test]
        public void PParser_Object()    // MPW extension
        {
            var src =
@"OBJECT
    FUNCTION  a: TObject;
    PROCEDURE b;
    END;
";
            Given_Parser(src);
            var t = parser.ParseType();
            var sExp = @"object function a : TObject; procedure b end";
            Assert.AreEqual(sExp, t.ToString());
        }
    }
}

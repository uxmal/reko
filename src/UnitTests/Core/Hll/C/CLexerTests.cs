#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Hll.C;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Hll.C
{
    [TestFixture]
    public class CLexerTests
    {
        private CLexer lex;

        private void Lex(string str)
        {
            this.lex = new CLexer(new StringReader(str), CLexer.GccKeywords);
        }

        private void AssertToken(CTokenType exp)
        {
            var token = lex.Read();
            Assert.AreEqual(exp, token.Type);
        }

        public void AssertToken(CTokenType expType, object expectedValue)
        {
            var token = lex.Read();
            Assert.AreEqual(expType, token.Type);
            Assert.AreEqual(expectedValue, token.Value);
        }

        public void ExpectLexerError()
        {
            try
            {
                lex.Read();
                Assert.Fail("Expected lexer to throw an exception.");
            } catch(NotImplementedException)
            {
            }
        }

        [Test]
        public void CLexer_Eatws()
        {
            Lex(" ");
            AssertToken(CTokenType.EOF);
        }

        [Test]
        public void CLexer_IntegerConstant()
        {
            Lex("  3");
            AssertToken(CTokenType.NumericLiteral, 3);
            AssertToken(CTokenType.EOF);
        }

        [Test]
        public void CLexer_Zero()
        {
            Lex("  0");
            AssertToken(CTokenType.NumericLiteral, 0);
            AssertToken(CTokenType.EOF);
        }
        [Test]
        public void CLexer_HexNumber()
        {
            Lex(" 0x32");
            AssertToken(CTokenType.NumericLiteral, 50);
        }

        [Test]
        public void CLexer_Identifier()
        {
            Lex("  i32");
            AssertToken(CTokenType.Id, "i32");
        }

        [Test]
        public void CLexer_LineDirective()
        {
            Lex(" #line");
            AssertToken(CTokenType.Hash);
            AssertToken(CTokenType.Id, "line");
        }

        [Test]
        public void CLexer_StringLiteral()
        {
            Lex("\"Hello\"");
            AssertToken(CTokenType.StringLiteral, @"Hello");
        }

        [Test]
        public void CLexer_EscapedQuote()
        {
            Lex("\"He said \\\"Hello\\\"\"");
            AssertToken(CTokenType.StringLiteral, @"He said ""Hello""");
        }

        [Test]
        public void CLexer_EscapedBackslash()
        {
            Lex("\"\\\\\"");
            AssertToken(CTokenType.StringLiteral, @"\");
        }

        [Test]
        public void CLexer_Pragma()
        {
            Lex("  #pragma ");
            AssertToken(CTokenType.Hash);
            AssertToken(CTokenType.Id, "pragma");
        }

        [Test]
        public void CLexer_Parens()
        {
            Lex("()3");
            AssertToken(CTokenType.LParen);
            AssertToken(CTokenType.RParen);
            AssertToken(CTokenType.NumericLiteral);
        }

        [Test]
        public void CLexer_Colon_Semicolon()
        {
            Lex(":;");
            AssertToken(CTokenType.Colon);
            AssertToken(CTokenType.Semicolon);
        }

        [Test]
        public void CLexer_typedef_unsigned_signed_long()
        {
            Lex("typedef unsigned signed long");
            AssertToken(CTokenType.Typedef);
            AssertToken(CTokenType.Unsigned);
            AssertToken(CTokenType.Signed);
            AssertToken(CTokenType.Long);
        }

        [Test]
        public void Int_Char_Short()
        {
            Lex("int char short");
            AssertToken(CTokenType.Int);
            AssertToken(CTokenType.Char);
            AssertToken(CTokenType.Short);
        }

        [Test]
        public void CLexer_Star()
        {
            Lex("*");
            AssertToken(CTokenType.Star);
        }

        [Test]
        public void CLexer_Comma()
        {
            Lex(",");
            AssertToken(CTokenType.Comma);
        }

        [Test]
        public void CLexer_Ignore_FormFeed()
        {
            Lex("\f,");
            AssertToken(CTokenType.Comma);
        }

        [Test]
        public void CLexer_Float_Double()
        {
            Lex(" float double ");
            AssertToken(CTokenType.Float);
            AssertToken(CTokenType.Double);
        }

        [Test]
        public void CLexer_Const_Void()
        {
            Lex(" const  void");
            AssertToken(CTokenType.Const);
            AssertToken(CTokenType.Void);
        }

        [Test]
        public void CLexer_Int64()
        {
            Lex("__int64");
            AssertToken(CTokenType.__Int64);
        }

        [Test]
        public void CLexer_Braces_Return()
        {
            Lex(" { return } ");
            AssertToken(CTokenType.LBrace);
            AssertToken(CTokenType.Return);
            AssertToken(CTokenType.RBrace);
        }

        [Test]
        public void CLexer_struct_union_class()
        {
            Lex(" struct union class");
            AssertToken(CTokenType.Struct);
            AssertToken(CTokenType.Union);
            AssertToken(CTokenType.Class);
        }

        [Test]
        public void CLexer_stdcall_cdecl_fastcall()
        {
            Lex(" __stdcall __cdecl __fastcall");
            AssertToken(CTokenType.__Stdcall);
            AssertToken(CTokenType.__Cdecl);
            AssertToken(CTokenType.__Fastcall);
        }

        [Test]
        public void CLexer_asm()
        {
            Lex(" __asm");
            AssertToken(CTokenType.__Asm);
        }

        [Test]
        public void CLexer_Brackets()
        {
            Lex(" []");
            AssertToken(CTokenType.LBracket);
            AssertToken(CTokenType.RBracket);
        }

        [Test]
        public void CLexer_Assign_Eq()
        {
            Lex(" = == ");
            AssertToken(CTokenType.Assign);
            AssertToken(CTokenType.Eq);
            AssertToken(CTokenType.EOF);
        }

        [Test]
        public void CLexer_SkipToNextLine_Lf()
        {
            Lex(" sdfsfds\nHello");
            lex.SkipToNextLine();
            AssertToken(CTokenType.Id, "Hello");
        }

        [Test]
        public void CLexer_SkipToNextLine_CrLf()
        {
            Lex(" sdfsfds\r\nHello");
            lex.SkipToNextLine();
            AssertToken(CTokenType.Id, "Hello");
        }

        [Test]
        public void CLexer_SkipToNextLine_Cr()
        {
            Lex(" sdfsfds\rHello");
            lex.SkipToNextLine();
            AssertToken(CTokenType.Id, "Hello");
        }

        [Test]
        public void CLexer_StorageClasses_BinOr()
        {
            Lex("auto | register | static | extern | typedef ");
            AssertToken(CTokenType.Auto);
            AssertToken(CTokenType.Pipe);
            AssertToken(CTokenType.Register);
            AssertToken(CTokenType.Pipe);
            AssertToken(CTokenType.Static);
            AssertToken(CTokenType.Pipe);
            AssertToken(CTokenType.Extern);
            AssertToken(CTokenType.Pipe);
            AssertToken(CTokenType.Typedef);
        }

        [Test]
        public void CLexer_Ampersand()
        {
            Lex("&");
            AssertToken(CTokenType.Ampersand);
        }

        [Test]
        public void CLexer_Dot()
        {
            Lex(".");
            AssertToken(CTokenType.Dot);
        }

        [Test]
        public void CLexer_Arrow()
        {
            Lex("->");
            AssertToken(CTokenType.Arrow);
        }

        [Test]
        public void CLexer_Inc_Dec()
        {
            Lex("++ --");
            AssertToken(CTokenType.Increment);
            AssertToken(CTokenType.Decrement);
        }

        [Test]
        public void CLexer_RealLiterals()
        {
            Lex("0.0 1e7 -3.25F 3e-3");
            AssertToken(CTokenType.RealLiteral, 0.0);
            AssertToken(CTokenType.RealLiteral, 1e7);
            AssertToken(CTokenType.RealLiteral, -3.25F);
            AssertToken(CTokenType.RealLiteral, 3e-3);
        }

        [Test]
        public void CLexer_Plus_Minus_Tilde_Bang()
        {
            Lex("+ - ~ !");
            AssertToken(CTokenType.Plus);
            AssertToken(CTokenType.Minus);
            AssertToken(CTokenType.Tilde);
            AssertToken(CTokenType.Bang);
        }

        [Test]
        public void CLexer_AssignmentTokens()
        {
            Lex("= *= /= %= += -= <<= >>= &= |= ^=");
            AssertToken(CTokenType.Assign);
            AssertToken(CTokenType.MulAssign);
            AssertToken(CTokenType.DivAssign);
            AssertToken(CTokenType.ModAssign);
            AssertToken(CTokenType.PlusAssign);
            AssertToken(CTokenType.MinusAssign);
            AssertToken(CTokenType.ShlAssign);
            AssertToken(CTokenType.ShrAssign);
            AssertToken(CTokenType.AndAssign);
            AssertToken(CTokenType.OrAssign);
            AssertToken(CTokenType.XorAssign);
        }

        [Test]
        public void CLexer_CharConstants()
        {
            Lex("'a' '\\''");
            AssertToken(CTokenType.CharLiteral, 'a');
            AssertToken(CTokenType.CharLiteral, '\'');
        }

        [Test]
        public void CLexer_LineNumber()
        {
            Lex(" a\nb ");
            Assert.AreEqual(1, lex.LineNumber);
            AssertToken(CTokenType.Id, "a");
            Assert.AreEqual(1, lex.LineNumber);
            AssertToken(CTokenType.NewLine);
            Assert.AreEqual(2, lex.LineNumber);
            AssertToken(CTokenType.Id, "b");
            Assert.AreEqual(2, lex.LineNumber);
        }

        [Test]
        public void CLexer_Hexnumber()
        {
            Lex("0x12");
            AssertToken(CTokenType.NumericLiteral, 0x12);
        }

        [Test]
        public void CLexer_ZeroComma()
        {
            Lex("0,1,");
            AssertToken(CTokenType.NumericLiteral, 0);
            AssertToken(CTokenType.Comma);
            AssertToken(CTokenType.NumericLiteral, 1);
            AssertToken(CTokenType.Comma);
        }

        [Test]
        public void CLexer_declspec()
        {
            Lex("__declspec");
            AssertToken(CTokenType.__Declspec);
        }

        [Test]
        public void CLexer_Ellipsis()
        {
            Lex("...");
            AssertToken(CTokenType.Ellipsis);
        }

        [Test]
        public void CLexer__success()
        {
            Lex("__success");
            AssertToken(CTokenType.__Success);
        }

        [Test]
        public void CLexer_ColonColon()
        {
            Lex("::");
            AssertToken(CTokenType.ColonColon);
        }

        [Test]
        public void CLexer_LineComment()
        {
            Lex("// foo\nid");
            AssertToken(CTokenType.NewLine);
            AssertToken(CTokenType.Id);
        }

        [Test]
        public void CLexer_Bool_C()
        {
            Lex("_Bool");   // This is a C keyword
            AssertToken(CTokenType._Bool);
        }

        [Test]
        public void CLexer_Bool_CPlusPlus()
        {
            Lex("bool");   // This is a C++ keyword
            AssertToken(CTokenType.Bool);
        }

        [Test]
        public void CLexer_SingleLineComment()
        {
            Lex("a // foo\r\n b");
            AssertToken(CTokenType.Id, "a");
            AssertToken(CTokenType.NewLine);
            AssertToken(CTokenType.Id, "b");
        }

        [Test]
        public void CLexer_Octal()
        {
            Lex("0123, 0, 056");
            AssertToken(CTokenType.NumericLiteral, 83);
            AssertToken(CTokenType.Comma);
            AssertToken(CTokenType.NumericLiteral, 0);
            AssertToken(CTokenType.Comma);
            AssertToken(CTokenType.NumericLiteral, 46);
        }

        [Test]
        public void CLexer_Hex_with_suffix()
        {
            Lex("0x42uL,0x44\r\n,0x48");
            AssertToken(CTokenType.NumericLiteral, 0x42);
            AssertToken(CTokenType.Comma);
            AssertToken(CTokenType.NumericLiteral, 0x44);
            AssertToken(CTokenType.NewLine);
            AssertToken(CTokenType.Comma);
            AssertToken(CTokenType.NumericLiteral, 0x48);
        }

        [Test]
        public void CLexer_decimal_with_suffix()
        {
            Lex("42uL,44,\r\n");
            AssertToken(CTokenType.NumericLiteral, 42);
            AssertToken(CTokenType.Comma);
            AssertToken(CTokenType.NumericLiteral, 44);
            AssertToken(CTokenType.Comma);
        }

        [Test]
        public void CLexer_wide_chaar_literal()
        {
            Lex("L'a' nix");
            AssertToken(CTokenType.WideCharLiteral, 'a');
            AssertToken(CTokenType.Id, "nix");
        }


        [Test]
        public void CLexer_L()
        {
            Lex("L;L");
            AssertToken(CTokenType.Id, "L");
            AssertToken(CTokenType.Semicolon);
            AssertToken(CTokenType.Id, "L");
        }

        [Test]
        public void CLexer_PreprocessingDirective()
        {
            Lex(" #define");
            AssertToken(CTokenType.Hash);
            AssertToken(CTokenType.Id, "define");
        }

        [Test]
        public void CLexer_Macos_NewLine()
        {
            Lex("hello\rworld");
            AssertToken(CTokenType.Id, "hello");
            AssertToken(CTokenType.NewLine);
            AssertToken(CTokenType.Id, "world");
            AssertToken(CTokenType.EOF);
        }

        [Test]
        public void CLexer_Unix_NewLine()
        {
            Lex("hello\nworld");
            AssertToken(CTokenType.Id, "hello");
            AssertToken(CTokenType.NewLine);
            AssertToken(CTokenType.Id, "world");
            AssertToken(CTokenType.EOF);
        }

        [Test]
        public void CLexer_DEC_NewLine()
        {
            Lex("hello\nworld");
            AssertToken(CTokenType.Id, "hello");
            AssertToken(CTokenType.NewLine);
            AssertToken(CTokenType.Id, "world");
            AssertToken(CTokenType.EOF);
        }

        [Test]
        public void CLexer_LineComment_WithNewline()
        {
            Lex("a // hello\nb");
            AssertToken(CTokenType.Id, "a");
            AssertToken(CTokenType.NewLine);
            AssertToken(CTokenType.Id, "b");
            AssertToken(CTokenType.EOF);
        }

        [Test]
        public void CLexer_Negative_HexLiteral()
        {
            Lex("  -0x4");
            AssertToken(CTokenType.NumericLiteral, -4);
        }
    }
}

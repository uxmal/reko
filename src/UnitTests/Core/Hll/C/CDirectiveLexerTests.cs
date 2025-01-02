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
    public class CDirectiveLexerTests
    {
        private CDirectiveLexer lexer;
        private ParserState state;

        [SetUp]
        public void Setup()
        {
            this.state = new ParserState();
        }

        private void Lex(string text)
        {
            lexer = new CDirectiveLexer(state, new CLexer(new StringReader(text), CLexer.GccKeywords));
        }

        private void LexMsvc(string text)
        {
            lexer = new CDirectiveLexer(state, new CLexer(new StringReader(text), CLexer.MsvcKeywords));
        }

        private void Expect(CTokenType expectedType)
        {
            var token = lexer.Read();
            Assert.AreEqual(expectedType, token.Type);
        }

        private void Expect(CTokenType expectedType, object expectedValue)
        {
            var token = lexer.Read();
            Assert.AreEqual(expectedType, token.Type);
            Assert.AreEqual(expectedValue, token.Value);
        }

        public void ExpectLexerError()
        {
            try
            {
                lexer.Read();
                Assert.Fail("Expected lexer to throw an exception.");
            }
            catch (FormatException)
            {
            }
        }

        [Test]
        public void CDirectiveLexer_NotTerminated_ReturnEof()
        {
            Lex("#line 1 \"foo.h\"");
            Expect(CTokenType.EOF);
        }

        [Test]
        public void CDirectiveLexer_LineDirective_ReturnFollowingToken()
        {
            Lex("#line 1 \"foo.h\"\na");
            Expect(CTokenType.Id, "a");
        }

        [Test]
        public void CDirectiveLexer_Pack_Push()
        {
            Assert.AreEqual(0, state.Alignment);
            Lex("#pragma pack(push,4)");
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
            Assert.AreEqual(4, state.Alignment);
        }

        [Test]
        public void CDirectiveLexer_Pack_Push_Pop()
        {
            Assert.AreEqual(0, state.Alignment);
            Lex("#pragma pack(push,4)\nx\n#pragma pack(pop)");
            Assert.AreEqual(CTokenType.Id, lexer.Read().Type);
            Assert.AreEqual(4, state.Alignment);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
            Assert.AreEqual(0, state.Alignment);
        }

        [Test]
        public void CDirectiveLexer_Pragma_Keywords()
        {
            Lex("__pragma(warning(push)) __pragma(warning(disable : 4548)) x");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_Pragma_deprecated()
        {
            Lex("#pragma deprecated (Foo) x");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_Pragma_comment_lib()
        {
            Lex("#pragma comment(lib, \"foo.lib\")\nx");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_InvalidPreprocessingDirective()
        {
            Lex(" hello # world");
            Assert.AreEqual(CTokenType.Id, lexer.Read().Type);
            ExpectLexerError();
        }

        [Test]
        public void CDirectiveLexer_Preprocessing_directives_must_start_line()
        {
            Lex(" hello # define X 42");
            Assert.AreEqual(CTokenType.Id, lexer.Read().Type);
            ExpectLexerError();
        }

        [Test]
        public void CDirectiveLexer_Pragma_region()
        {
            Lex("#pragma region stuf stuff stuff\r\nx");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_NewPackSyntax()
        {
            Lex(
                "#pragma pack(4)\r\n" +
                "#pragma pack()\r\nx");
            Assert.AreEqual("x", lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_define_constant()
        {
            Lex(
                "// Constant coming up\r\n" +
                "# define X 42\r\nX");
            Assert.AreEqual(42, lexer.Read().Value);
        }

        [Test]
        public void CDirectiveLexer_define_multiple_tokens()
        {
            Lex(
                "// Multiple tokens coming up\r\n" +
                "# define X (A B)\r\nX");
            Assert.AreEqual(CTokenType.LParen, lexer.Read().Type);
            Assert.AreEqual("A", lexer.Read().Value);
            Assert.AreEqual("B", lexer.Read().Value);
            Assert.AreEqual(CTokenType.RParen, lexer.Read().Type);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_undef()
        {
            Lex(
                "#define omg lol\r\n" +
                "omg wtf\r\n" +
                "#undef omg\r\n" +
                "omg hi");
            Assert.AreEqual("lol", lexer.Read().Value);
            Assert.AreEqual("wtf", lexer.Read().Value);
            Assert.AreEqual("omg", lexer.Read().Value);
            Assert.AreEqual("hi", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_undef_nonexistent()
        {
            Lex(
                "/* does not exist */\r\n" +
                "#undef macro\r\nmacro");
            Assert.AreEqual("macro", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_undef_comments()
        {
            Lex(
                "#define xxx yyy\r\n" +
                "aaa xxx bbb\r\n" +
                "#/*xd*/undef/* asdf */xxx// comment;?\r\n" +
                "ccc xxx ddd");
            Assert.AreEqual("aaa", lexer.Read().Value);
            Assert.AreEqual("yyy", lexer.Read().Value);
            Assert.AreEqual("bbb", lexer.Read().Value);
            Assert.AreEqual("ccc", lexer.Read().Value);
            Assert.AreEqual("xxx", lexer.Read().Value);
            Assert.AreEqual("ddd", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_undef_multiple_tokens()
        {
            Lex(
                "#define token1 something\r\n" +
                "#define token2 something_else\r\n" +
                "// should probably warn\r\n" +
                "#undef token1 token2\r\n" +
                "token1 token2");
            Assert.AreEqual("token1", lexer.Read().Value);
            Assert.AreEqual("something_else", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_multiline()
        {
            Lex(
@"#pragma read_only_file;
#pragma pack( push, 1 )
typedef int");
            Assert.AreEqual(CTokenType.Typedef, lexer.Read().Type);
            Assert.AreEqual(1, state.Alignment);
            Assert.AreEqual(CTokenType.Int, lexer.Read().Type);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_msvc_pragma_pack_push()
        {
            LexMsvc(@"
        __pragma(pack(push, 8))
");
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
            Assert.AreEqual(8, state.Alignment);
        }

        [Test]
        public void CDirectiveLexer_ifdef()
        {
            LexMsvc(@"
#define DOIT
#ifdef DOIT
foo
#endif
bar
");
            Assert.AreEqual("foo", lexer.Read().Value);
            Assert.AreEqual("bar", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_ifdef_not_defined()
        {
            LexMsvc(@"
#ifdef DOIT
foo
#endif
bar
");
            Assert.AreEqual("bar", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_ifndef()
        {
            LexMsvc(@"
#ifndef DOIT
foo
#endif
bar
");
            Assert.AreEqual("foo", lexer.Read().Value);
            Assert.AreEqual("bar", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_ifdef_else()
        {
            LexMsvc(@"
#ifdef DOIT
doit
#else
dontdoit
#endif
bar
");
            Assert.AreEqual("dontdoit", lexer.Read().Value);
            Assert.AreEqual("bar", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_nested_ifdefs()
        {
            LexMsvc(@"
#ifdef A
a
# ifdef B
ab
# else
anotb
# endif
posta
#else
b
# ifdef C
bc
# else
bnotc
# endif
postb
#endif
done
");
            Assert.AreEqual("b", lexer.Read().Value);
            Assert.AreEqual("bnotc", lexer.Read().Value);
            Assert.AreEqual("postb", lexer.Read().Value);
            Assert.AreEqual("done", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_various_extra_tokens()
        {
            LexMsvc(@"
#define X 0
#undef Y
#ifdef Y qwer
bad
#else 1.23
good
# ifndef X
bad2
# else hi qqq
good2
# endif
#endif -1111
done
");
            Assert.AreEqual("good", lexer.Read().Value);
            Assert.AreEqual("good2", lexer.Read().Value);
            Assert.AreEqual("done", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_dont_define_twice()
        {
            LexMsvc(@"
#ifdef X
# define Y 3
#else 
# define Y 42
#endif
Y
");
            Assert.AreEqual(42, lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_redefine()
        {
            LexMsvc(@"
#define Y 3
#define Y 42
Y
");
            Assert.AreEqual(42, lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_nested_defines()
        {
            LexMsvc(@"
#define A ( 3 )
#define B ( A )
B");
            Assert.AreEqual(CTokenType.LParen, lexer.Read().Type);
            Assert.AreEqual(CTokenType.LParen, lexer.Read().Type);
            Assert.AreEqual(3, lexer.Read().Value);
            Assert.AreEqual(CTokenType.RParen, lexer.Read().Type);
            Assert.AreEqual(CTokenType.RParen, lexer.Read().Type);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }

        [Test]
        public void CDirectiveLexer_reko_macro()
        {
            Lex(
                "#ifdef __REKO_DECOMPILER__\r\n" +
                "# define result1 good1\r\n" +
                "#else\r\n" +
                "# define result1 bad1\r\n" +
                "#endif\r\n" +
                "#ifndef __REKO_DECOMPILER__\r\n" +
                "# define result2 bad2\r\n" +
                "#else\r\n" +
                "# define result2 good2\r\n" +
                "#endif\r\n" +
                "result1 result2");
            Assert.AreEqual("good1", lexer.Read().Value);
            Assert.AreEqual("good2", lexer.Read().Value);
            Assert.AreEqual(CTokenType.EOF, lexer.Read().Type);
        }
    }
}

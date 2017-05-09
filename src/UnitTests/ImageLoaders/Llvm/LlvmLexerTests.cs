#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.ImageLoaders.LLVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.Llvm
{
    [TestFixture]
    public class LlvmLexerTests
    {
        private LLVMLexer lex;

        private void CreateLexer(string str)
        {
            this.lex = new LLVMLexer(new StringReader(str));
        }

        [Test]
        public void LLLex_EOF()
        {
            CreateLexer("");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.EOF, tok.Type);
        }

        [Test]
        public void LLLex_Comment()
        {
            CreateLexer("; hello\r\n");
            var tok = lex.GetToken();
            Assert.AreEqual(TokenType.Comment, tok.Type);
            Assert.AreEqual(" hello", tok.Value);
        }
    }
/*
; clang -emit-llvm -S -o foo.ll foo.cpp

; ModuleID = 'foo.cpp'
source_filename = "foo.cpp"
target datalayout = "e-m:e-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-gnu"

%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }
%struct.node = type { %struct.node*, i32 }
%class.number = type { i32, i32 }
%struct.variant = type { i32, %union.anon }
%union.anon = type { i8* }

@.str = private unnamed_addr constant [5 x i8] c"zero\00", align 1
@.str.1 = private unnamed_addr constant [4 x i8] c"one\00", align 1
*/
}

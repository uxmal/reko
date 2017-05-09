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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.Llvm
{
    [TestFixture]
    public class LlvmParserTests
    {
        private string llir;
        private string sExp;

        public void Setup()
        {
            this.llir = null;
            this.sExp = null;
        }

        private void RunInstrTest()
        {
            var rdr = new StringReader(llir);
            var parser = new LLVMParser(new LLVMLexer(rdr));
            var instr = parser.ParseInstruction();
            Assert.AreEqual(sExp, instr.ToString());
        }

        private void RunTypeTest()
        {
            var rdr = new StringReader(llir);
            var parser = new LLVMParser(new LLVMLexer(rdr));
            var type = parser.ParseType();
            Assert.AreEqual(sExp, type.ToString());
        }


        private void RunModuleTest()
        {
            var rdr = new StringReader(llir);
            var parser = new LLVMParser(new LLVMLexer(rdr));
            var module = parser.ParseModule();
            Assert.AreEqual(sExp, module.ToString());
        }

        [Test]
        public void LLParser_br()
        {
            llir = "  br label %14";
            sExp = "br label %14";
            RunInstrTest();
        }

        [Test]
        public void LLParser_arrayType()
        {
            llir = "[42 x i8]";
            sExp = "[42 x i8]";
            RunTypeTest();
        }

        [Test(Description = "Sample taken from http://llvm.org/docs/LangRef.html#module-structure")]
        public void LLParser_Module()
        {
            llir =
@"; Declare the string constant as a global constant.
@.str = private unnamed_addr constant [13 x i8] c""hello world\0A\00""

; External declaration of the puts function
declare i32 @puts(i8* nocapture) nounwind

; Definition of main function
define i32 @main() {   ; i32()*
  ; Convert [13 x i8]* to i8  *...
  %cast210 = getelementptr [13 x i8], [13 x i8]* @.str, i64 0, i64 0

  ; Call puts function to write out the string to stdout.
  call i32 @puts(i8* %cast210)
  ret i32 0
}";
            sExp =
@"@.str = private unnamed_addr constant [13 x i8] c""hello world\0A\00""

declare i32 @puts(i8*)

define i32 @main() {
    %cast210 = getelementptr [13 x i8], [13 x i8]* @.str, i64 0, i64 0
    call i32 @puts(i8* %cast210)
    ret i32 0
}
";
            RunModuleTest();
        }

    }
}

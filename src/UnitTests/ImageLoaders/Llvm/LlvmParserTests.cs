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
using Reko.Core.Output;
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
        private readonly string nl = Environment.NewLine;

        [SetUp]
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

        [Test]
        public void LLParser_localtypedef()
        {
            llir = "%point = type { i32, i32 }";
            sExp = "%point = type { i32, i32 }" + nl;
            RunModuleTest();
        }

        [Test]
        public void LLParser_recursive_type()
        {
            llir = "%tree = type { i32, %tree, %tree }";
            sExp = "%tree = type { i32, %tree, %tree }" + nl;
            RunModuleTest();
        }

        [Test] 
        public void LLParser_global_constant()
        {
            llir = "@.str = private unnamed_addr constant [5 x i8] c\"zero\\00\", align 1";
            sExp = "@.str = private unnamed_addr constant [5 x i8] c\"zero\\00\", align 1" + nl;
            RunModuleTest();
        }

        [Test]
        public void LLParser_global_initialized()
        {
            llir = "@ptr_size = global i32 4, align 4";
            sExp = "@ptr_size = global i32 4, align 4" + nl;
            RunModuleTest();
        }

        [Test]
        public void LLParser_external_global()
        {
            llir = "@stderr = external global %struct._IO_FILE*, align 8";
            sExp = "@stderr = external global %struct._IO_FILE*, align 8" + nl;
            RunModuleTest();
        }

        [Test]
        public void LLParser_store()
        {
            llir = "store i32 %0, i32* %3, align 4";
            sExp = "store i32 %0, i32* %3, align 4";
            RunInstrTest();
        }

        [Test]
        public void LLParser_load()
        {
            llir = "%4 = load i32, i32* %3, align 4";
            sExp = "%4 = load i32, i32* %3, align 4";
            RunInstrTest();
        }

        [Test]
        public void LLParser_switch()
        {
            llir = "switch i32 %4, label %9 [ i32 0, label %5 i32 1, label %6 i32 2, label %7 ]";
            sExp =
                "switch i32 %4, label %9 [" + nl +
                "    i32 0, label %5" + nl +
                "    i32 1, label %6" + nl +
                "    i32 2, label %7" + nl +
                "]";
            RunInstrTest();
        }

        [Test]
        public void LLParser_getelementptr_expr()
        {
            llir = "store i8*getelementptr inbounds([5 x i8], [5 x i8]* @.str, i32 0, i32 0), i8** %2, align 8";
            sExp = "store i8* getelementptr inbounds ([5 x i8], [5 x i8]* @.str, i32 0, i32 0), i8** %2, align 8";
            RunInstrTest();
        }

        [Test]
        public void LLParser_phi()
        {
            llir = "%15 = phi i1 [ false, %5 ], [ %13, %8 ]";
            sExp = "%15 = phi i1 [false, %5], [%13, %8]";
            RunInstrTest();
        }

        [Test]
        public void LLParser_pfn()
        {
            llir = "%pfn = type void (i32) *";
            sExp = "%pfn = type void (i32)*" + nl;
            RunModuleTest();
        }

        [Test]
        public void LLParser_inttoptr()
        {
            llir = "%28 = inttoptr i64 %17 to i64 (%class.number*)*";
            sExp = "%28 = inttoptr i64 %17 to i64 (%class.number*)*";
            RunInstrTest();
        }

        [Test]
        public void LLParser_call()
        {
            llir = "%31 = call i64 %30(i64* %16)";
            sExp = "%31 = call i64 %30(i64* %16)";
            RunInstrTest();
        }

        [Test]
        public void LLParser_add()
        {
            llir = "%2 = add nuw nsw i32 %1, 4";
            sExp = "%2 = add nuw nsw i32 %1, 4";
            RunInstrTest();
        }

        [Test]
        public void LLParser_printf()
        {
            llir = "%17 = call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([3 x i8], [3 x i8]* @.str.16, i32 0, i32 0), i8* %16)";
            sExp = "%17 = call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([3 x i8], [3 x i8]* @.str.16, i32 0, i32 0), i8* %16)";
            RunInstrTest();
        }

        [Test]
        public void LLParser_metadata()
        {
            llir = "!llvm.ident = !{!0}";
            sExp = "";
            RunModuleTest();
        }

        [Test]
        public void LLParser_parameter_attributes()
        {
            llir =
@"define zeroext i1 @prev_char(i8 signext) #0 {
    ret i1 false
}";
            sExp =
@"define zeroext i1 @prev_char(i8 signext) {
    ret i1 false
}
";
            RunModuleTest();
        }

        [Test]
        public void LLParser_sext()
        {
            llir = "%2 = sext i8 %1 to i32";
            sExp = "%2 = sext i8 %1 to i32";
            RunInstrTest();
        }

        [Test]
        public void LLParser_call_ret_signext()
        {
            llir = "%8 = call signext i8 @next_char()";
            sExp = "%8 = call signext i8 @next_char()";
            RunInstrTest();
        }

        [Test]
        public void LLParser_call_param_signext()
        {
            llir = "%35 = call zeroext i1 @prev_char(i8 signext 47)";
            sExp = "%35 = call zeroext i1 @prev_char(i8 signext 47)";
            RunInstrTest();
        }

        [Test]
        public void LLParser_select()
        {
            llir = "%10 = select i1 %9, i32 1, i32 0";
            sExp = "%10 = select i1 %9, i32 1, i32 0";
            RunInstrTest();
        }

        [Test]
        public void LLParser_call_true_argument()
        {
            llir = "call void @branch(i1 zeroext true)";
            sExp = "call void @branch(i1 zeroext true)";
            RunInstrTest();
        }

        [Test]
        public void LLParser_opaque_type()
        {
            llir = "%ccache = type opaque";
            sExp = "%ccache = type opaque" + nl;
            RunModuleTest();
        }

        [Test]
        public void LLParser_empty_struct()
        {
            llir = "{}";
            sExp = "{}";
            RunTypeTest();
        }

        [Test]
        public void LLParser_struct_literal()
        {
            llir = "@var = global %var_t {i32 1, i64 -108}";
            sExp = "@var = global %var_t {i32 1, i64 -108}" + nl;
            RunModuleTest();
        }

        [Test]
        public void LLParser_bitcast_expr()
        {
            llir = "store void ()* (i8*, i8*)* bitcast (i8* (i8*, i8*)* @dlsym to void ()* (i8*, i8*)*), void ()* (i8*, i8*)** %7, align 8";
            sExp = "store void ()* (i8*, i8*)* bitcast (i8* (i8*, i8*)* @dlsym to void ()* (i8*, i8*)*), void ()* (i8*, i8*)** %7, align 8";
            RunInstrTest();
        }

        [Test]
        public void LLParser_funcdef_internal()
        {
            llir =
@"define internal void @frob(i8*) {
    ret void
}";
            sExp =
@"define void @frob(i8*) {
    ret void
}
";
            RunModuleTest();
        }

        [Test]
        public void LLParser_fcmp()
        {
            llir = "%761 = fcmp olt x86_fp80 %760, 0xK00000000000000000000";
            sExp = "%761 = fcmp olt x86_fp80 %760, 0xK00000000000000000000";
            RunInstrTest();
        }

        [Test]
        public void LLParser_fmul()
        {
            llir = "%797 = fmul double %796, 1.000000e-01";
            sExp = "%797 = fmul double %796, 1.000000e-01";
            RunInstrTest();
        }

        [Test]
        public void LLParser_target_datalayout()
        {
            llir = "target datalayout = \"layout spec\"";
            sExp = "target datalayout = \"layout spec\"" + nl;
            RunModuleTest();
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

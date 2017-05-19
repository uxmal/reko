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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
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
    public class ProgramBuilderTests
    {
        private Dictionary<string, Identifier> globals;

        [SetUp]
        public void Setup()
        {
            this.globals = new Dictionary<string, Identifier>();
        }

        public void Global(string name, DataType dt)
        {
            globals.Add(name, new Identifier(name, dt, new MemoryStorage()));
        }

        private Procedure RunFuncTest(params string[] lines)
        {
            var parser = new LLVMParser(new StringReader(
                string.Join(Environment.NewLine, lines)));
            var fn = parser.ParseFunctionDefinition();
            var pb = new ProgramBuilder(PrimitiveType.Pointer32);
            foreach (var de in globals)
            {
                pb.Globals.Add(de.Key, de.Value);
            }
            var proc = pb.RegisterFunction(fn);
            pb.TranslateFunction(fn);
            return proc;
        }

        private void AssertProc(string sExp, Procedure proc)
        {
            var sw = new StringWriter();
            proc.Write(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.Print(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void LLPB_ReturnVoid()
        {
            var proc = RunFuncTest(
                "define i32 @foo(i8*,i32) {",
                "   ret void",
                "}");
            var sExp =
@"// foo
// Return size: 0
word32 foo(byte * arg0, word32 arg1)
foo_entry:
	// succ:  l2
l2:
	return
	// succ:  foo_exit
foo_exit:
";
            AssertProc(sExp, proc);
        }

        [Test]
        public void LLPB_ReturnConst()
        {
            var proc = RunFuncTest(
                "define i32 @foo(i8*,i32) {",
                "   ret i32 3",
                "}");

            var sExp =
@"// foo
// Return size: 0
word32 foo(byte * arg0, word32 arg1)
foo_entry:
	// succ:  l2
l2:
	return 0x00000003
	// succ:  foo_exit
foo_exit:
";
            AssertProc(sExp, proc);
        }



        [Test]
        public void LLPB_Add()
        {
            var proc = RunFuncTest(
                "define i32 @foo(i32) {",
                "   %2 = add i32 %0, 3",
                "   ret i32 %2",
                "}");
            var sExp =
@"// foo
// Return size: 0
word32 foo(word32 arg0)
foo_entry:
	// succ:  l1
l1:
	loc2 = arg0 + 0x00000003
	return loc2
	// succ:  foo_exit
foo_exit:
";
            AssertProc(sExp, proc);
        }

        [Test]
        public void LLPB_Branches()
        {
            Global("curch", PrimitiveType.Char);
            Global("curln", PrimitiveType.Int32);
            Global("input", PrimitiveType.Pointer32);
            Global("fgetc", PrimitiveType.Pointer32);
            var proc = RunFuncTest(
@"define signext i8 @next_char() #0 {
  %1 = load i8, i8* @curch, align 1
  %2 = sext i8 %1 to i32
  %3 = icmp eq i32 %2, 10
  br i1 %3, label %4, label %7

 ; <label>:4:                                      ; preds = %0
  %5 = load i32, i32* @curln, align 4
  %6 = add nsw i32 %5, 1
  store i32 %6, i32* @curln, align 4
  br label %7

; <label>:7:                                      ; preds = %4, %0
  %8 = load %struct._IO_FILE*, %struct._IO_FILE** @input, align 8
  %9 = call i32 @fgetc(%struct._IO_FILE* %8)
  %10 = trunc i32 %9 to i8
  store i8 %10, i8* @curch, align 1
  ret i8 %10
}");
            var sExp =
@"// next_char
// Return size: 0
byte next_char()
next_char_entry:
	// succ:  l0
l0:
	loc1 = Mem0[curch:byte]
	loc2 = (int32) loc1
	loc3 = loc2 == 0x0000000A
	branch loc3 l4
	goto l7
	// succ:  l7 l4
l4:
	loc5 = Mem0[curln:word32]
	loc6 = loc5 + 0x00000001
	Mem0[curln:word32] = loc6
	// succ:  l7
l7:
	loc8 = Mem0[input:(ptr struct._IO_FILE)]
	loc9 = fgetc(loc8)
	loc10 = (byte) loc9
	Mem0[curch:byte] = loc10
	return loc10
	// succ:  next_char_exit
next_char_exit:
";
            AssertProc(sExp, proc);
        }

        [Test]
        public void LLPB_GetElementPtr()
        {
            Global("puts", PrimitiveType.Pointer32);
            Global("msg", new Pointer(new ArrayType(PrimitiveType.Char, 13), 4));
            var proc = RunFuncTest( 
@"define i32 @foo() { 
  ; Convert [13 x i8]* to i8  *...
  %1 = getelementptr [13 x i8], [13 x i8]* @msg, i64 0, i64 0

  call i32 @puts(i8* %1)
  ret i32 0
}");
            var sExp =
@"// foo
// Return size: 0
word32 foo()
foo_entry:
	// succ:  l0
l0:
	loc1 = &(*msg)[0x0000000000000000]
	puts(loc1)
	return 0x00000000
	// succ:  foo_exit
foo_exit:
";
            AssertProc(sExp, proc);

        }

        [Ignore("Only works on @uxmal's machine right now")]
        [Test]
        public void LLPB_File()
        {
            using (var rdr = File.OpenText(@"D:\dev\uxmal\reko\LLVM\more_llvm\more_llvm\c4\c4.ll"))
            {
                var parser = new LLVMParser(new LLVMLexer(rdr));
                var module = parser.ParseModule();
                var pb = new ProgramBuilder(PrimitiveType.Pointer32);
                Program program = pb.BuildProgram(module);
            }
        }
    }
}

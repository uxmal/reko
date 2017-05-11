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
        private FunctionDefinition Func(params string[] lines)
        {
            var parser = new LLVMParser(new LLVMLexer(new StringReader(
                string.Join(Environment.NewLine, lines))));
            var fn = parser.ParseFunctionDefinition();
            return fn;
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
        public void LLPB_RegisterSignature()
        {
            var fn = Func(
                "define i32 @foo(i8*,i32) {",
                "   ret void",
                "}");

            var pb = new ProgramBuilder(PrimitiveType.Pointer32);
            pb.RegisterFunction(fn);
            pb.TranslateFunction(fn);

            var proc = pb.Functions.Values.First().Procedure;
            var sExp =
@"// foo
// Return size: 0
word32 foo(byte * %0, word32 %1)
foo_entry:
	// succ:  l2
l2:
foo_exit:
";
            AssertProc(sExp, proc);
        }
    }
}

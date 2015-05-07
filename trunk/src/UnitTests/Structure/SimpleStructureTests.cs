#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Output;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using Decompiler.UnitTests.TestCode;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class SimpleStructureTests : StructureTestBase
    {
        private void RunTest(string sourceFilename, string outFilename)
        {
            RunTest16(sourceFilename, outFilename, Address.SegPtr(0xC00, 0));
        }

        private void RunTest(ProcedureBuilder mock, string outFilename)
        {
            Procedure proc = mock.Procedure;
            using (FileUnitTester fut = new FileUnitTester(outFilename))
            {
                ControlFlowGraphCleaner cfgc = new ControlFlowGraphCleaner(proc);
                cfgc.Transform();
                proc.Write(false, fut.TextWriter);
                fut.TextWriter.WriteLine();

                StructureAnalysis sa = new StructureAnalysis(proc);
                sa.Structure();
                CodeFormatter fmt = new CodeFormatter(new TextFormatter(fut.TextWriter));
                fmt.Write(proc);
                fut.TextWriter.WriteLine("===========================");

                fut.AssertFilesEqual();
            }
        }

        private void RunTest16(string sourceFilename, string outFilename, Address addrBase)
        {
            using (FileUnitTester fut = new FileUnitTester(outFilename))
            {
                RewriteProgramMsdos(sourceFilename, addrBase);
                foreach (Procedure proc in program.Procedures.Values)
                {
                    var cfgc = new ControlFlowGraphCleaner(program.Procedures.Values[0]);
                    cfgc.Transform();
                    proc.Write(false, fut.TextWriter);
                    fut.TextWriter.WriteLine();

                    var sa = new StructureAnalysis(proc);
                    sa.Structure();
                    var fmt = new CodeFormatter(new TextFormatter(fut.TextWriter));
                    fmt.Write(proc);
                    fut.TextWriter.WriteLine("===========================");
                }
                fut.AssertFilesEqual();
            }
        }

        private void RunTest32(string sourceFilename, string outFilename)
        {
            RunTest32(sourceFilename, outFilename, Address.Ptr32(0x00400000));
        }

        private void RunTest32(string sourceFilename, string outFilename, Address addrBase)
        {
            using (FileUnitTester fut = new FileUnitTester(outFilename))
            {
                RewriteProgram32(sourceFilename, addrBase);
                foreach (Procedure proc in program.Procedures.Values)
                {
                    var cfgc = new ControlFlowGraphCleaner(program.Procedures.Values[0]);
                    cfgc.Transform();
                    proc.Write(false, fut.TextWriter);
                    fut.TextWriter.WriteLine();

                    var sa = new StructureAnalysis(proc);
                    sa.Structure();
                    var fmt = new CodeFormatter(new TextFormatter(fut.TextWriter));
                    fmt.Write(proc);
                    fut.TextWriter.WriteLine("===========================");
                }
                fut.AssertFilesEqual();
            }
        }

        private void RunTest32(ProcedureBuilder mock, string outFilename)
        {
            Procedure proc = mock.Procedure;
            using (FileUnitTester fut = new FileUnitTester(outFilename))
            {
                ControlFlowGraphCleaner cfgc = new ControlFlowGraphCleaner(proc);
                cfgc.Transform();
                proc.Write(false, fut.TextWriter);
                fut.TextWriter.WriteLine();

                StructureAnalysis sa = new StructureAnalysis(proc);
                sa.Structure();
                CodeFormatter fmt = new CodeFormatter(new TextFormatter(fut.TextWriter));
                fmt.Write(proc);
                fut.TextWriter.WriteLine("===========================");

                fut.AssertFilesEqual();
            }
        }


        private void RunTest(string expected, Program program)
        {
            var sw = new StringWriter();
            foreach (var proc in program.Procedures.Values)
            {
                var cfgc = new ControlFlowGraphCleaner(proc);
                cfgc.Transform();
                var sa = new StructureAnalysis(proc);
                sa.Structure();
                var fmt = new CodeFormatter(new TextFormatter(sw));
                fmt.Write(proc);
                sw.WriteLine("===");
            }
            Console.WriteLine(sw);
            Assert.AreEqual(expected, sw.ToString());
        }

        private void RunTest32(string expected, Program program)
        {
            var sw = new StringWriter();
            foreach (var proc in program.Procedures.Values)
            {
                var cfgc = new ControlFlowGraphCleaner(proc);
                cfgc.Transform();
                var sa = new StructureAnalysis(proc);
                sa.Structure();
                var fmt = new CodeFormatter(new TextFormatter(sw));
                fmt.Write(proc);
                sw.WriteLine("===");
            }
            Console.WriteLine(sw);
            Assert.AreEqual(expected, sw.ToString());
        }

        [Test]
        public void StrIf()
        {
            RunTest("Fragments/if.asm", "Structure/StrIf.txt");
        }

        [Test]
        public void StrFactorialReg()
        {
            RunTest("Fragments/factorial_reg.asm", "Structure/StrFactorialReg.txt");
        }

        [Test]
        public void StrFactorial()
        {
            RunTest("Fragments/factorial.asm", "Structure/StrFactorial.txt");
        }

        [Test]
        public void StrWhileBreak()
        {
            RunTest("Fragments/while_break.asm", "Structure/StrWhileBreak.txt");
        }

        [Test]
        public void StrWhileRepeat()
        {
            RunTest("Fragments/while_repeat.asm", "Structure/StrWhileRepeat.txt");
        }

        [Test]
        public void StrForkInLoop()
        {
            RunTest("Fragments/forkedloop.asm", "Structure/StrForkInLoop.txt");
        }

        [Test]
        public void StrNestedIf()
        {
            RunTest("Fragments/nested_ifs.asm", "Structure/StrNestedIf.txt");
        }

        [Test]
        public void StrNestedLoops()
        {
            RunTest("Fragments/matrix_addition.asm", "Structure/StrNestedLoop.txt");
        }

        [Test]
        public void StrReg00006()
        {
            RunTest16("Fragments/regressions/r00006.asm", "Structure/StrReg00006.txt", Address.Ptr32(0x100048B0));
        }


        [Test]
        [Ignore("Not quite ready yet")]
        public void StrNonreducible()
        {
            RunTest("Fragments/nonreducible.asm", "Structure/StrNonreducible.txt");
        }

        [Test]
        public void StrWhileGoto()
        {
            RunTest("Fragments/while_goto.asm", "Structure/StrWhileGoto.txt");
        }

        [Test]
        public void StrIfElseIf()
        {
            RunTest(new MockIfElseIf(), "Structure/StrIfElseIf.txt");
        }

        [Test]
        public void StrReg00011()
        {
            RunTest(new Reg00011Mock(), "Structure/StrReg00011.txt");
        }

        [Test]
        public void StrReg00013()
        {
            RunTest("Fragments/regressions/r00013.asm", "Structure/StrReg00013.txt");
        }

        [Test]
        public void StrManyIncrements()
        {
            RunTest(new ManyIncrements(), "Structure/StrManyIncrements.txt");
        }

        [Test]
        public void StrFragmentTest()
        {
            RewriteX86Fragment(@"
.i386
push ebp
mov ebp,esp
cmp dword ptr [ebp+8],0
jz done

mov dword ptr [0x123234],0x6423

done:
pop ebp
ret
", Address.Ptr32(0x00400000));
            RunTest(@"void fn00400000(word32 dwArg04)
{
	if (dwArg04 != 0x00000000)
		Mem11[0x00123234:word32] = 0x00006423;
	return;
}
===
", program);

        }
    }
}
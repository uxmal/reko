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

using Reko.Core;
using Reko.Core.Output;
using Reko.Structure;
using Reko.UnitTests.Mocks;
using Reko.UnitTests.TestCode;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Structure
{
    [TestFixture]
    public class SimpleStructureTests : StructureTestBase
    {
        private void RunTest(string sourceFilename, string outFilename)
        {
            RunTestMsdos(sourceFilename, outFilename, Address.SegPtr(0xC00, 0));
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

                var sa = new StructureAnalysis(new FakeDecompilerEventListener(), program, proc);
                sa.Structure();
                CodeFormatter fmt = new CodeFormatter(new TextFormatter(fut.TextWriter));
                fmt.Write(proc);
                fut.TextWriter.WriteLine("===========================");

                fut.AssertFilesEqual();
            }
        }

        private void RunTestMsdos(string sourceFilename, string outFilename, Address addrBase)
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

                    var sa = new StructureAnalysis(new FakeDecompilerEventListener(), program, proc);
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
            var program = RewriteProgram32(sourceFilename, addrBase);
            using (FileUnitTester fut = new FileUnitTester(outFilename))
            {
                foreach (Procedure proc in program.Procedures.Values)
                {
                    var cfgc = new ControlFlowGraphCleaner(program.Procedures.Values[0]);
                    cfgc.Transform();
                    proc.Write(false, fut.TextWriter);
                    fut.TextWriter.WriteLine();

                    var sa = new StructureAnalysis(new FakeDecompilerEventListener(), program, proc);
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

                var sa = new StructureAnalysis(new FakeDecompilerEventListener(), program, proc);
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
                var sa = new StructureAnalysis(new FakeDecompilerEventListener(), program, proc);
                sa.Structure();
                var fmt = new CodeFormatter(new TextFormatter(sw));
                fmt.Write(proc);
                sw.WriteLine("===");
            }
            try
            {
                Assert.AreEqual(expected, sw.ToString());
            } catch
            {
                Console.WriteLine(sw);
                throw;
            }
        }

        private void RunTest32(string expected, Program program)
        {
            var sw = new StringWriter();
            foreach (var proc in program.Procedures.Values)
            {
                var cfgc = new ControlFlowGraphCleaner(proc);
                cfgc.Transform();
                var sa = new StructureAnalysis(new FakeDecompilerEventListener(), program, proc);
                sa.Structure();
                var fmt = new CodeFormatter(new TextFormatter(sw));
                fmt.Write(proc);
                sw.WriteLine("===");
            }
            try
            {
                Assert.AreEqual(expected, sw.ToString());
            }
            catch
            {
                Console.WriteLine(sw);
                throw;
            }
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrIf()
        {
            RunTest("Fragments/if.asm", "Structure/StrIf.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrFactorialReg()
        {
            RunTest("Fragments/factorial_reg.asm", "Structure/StrFactorialReg.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrFactorial()
        {
            RunTest("Fragments/factorial.asm", "Structure/StrFactorial.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrWhileBreak()
        {
            RunTest("Fragments/while_break.asm", "Structure/StrWhileBreak.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrWhileRepeat()
        {
            RunTest("Fragments/while_repeat.asm", "Structure/StrWhileRepeat.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrForkInLoop()
        {
            RunTest("Fragments/forkedloop.asm", "Structure/StrForkInLoop.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrNestedIf()
        {
            RunTest("Fragments/nested_ifs.asm", "Structure/StrNestedIf.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrNestedLoops()
        {
            RunTest("Fragments/matrix_addition.asm", "Structure/StrNestedLoop.txt");
        }

        [Test]
        [Ignore(Categories.IntegrationTests)]
        public void StrReg00006()
        {
            RunTest32("Fragments/regressions/r00006.asm", "Structure/StrReg00006.txt", Address.Ptr32(0x100048B0));
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrNonreducible()
        {
            RunTest("Fragments/nonreducible.asm", "Structure/StrNonreducible.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrWhileGoto()
        {
            RunTest("Fragments/while_goto.asm", "Structure/StrWhileGoto.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrIfElseIf()
        {
            RunTest(new MockIfElseIf(), "Structure/StrIfElseIf.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrReg00011()
        {
            RunTest(new Reg00011Mock(), "Structure/StrReg00011.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrReg00013()
        {
            RunTest("Fragments/regressions/r00013.asm", "Structure/StrReg00013.txt");
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrManyIncrements()
        {
            RunTest(new ManyIncrements(), "Structure/StrManyIncrements.txt");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void StrFragmentTest()
        {
            RewriteX86RealFragment(@"
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
		Mem9[0x00123234:word32] = 0x00006423;
}
===
", program);

        }

        [Test]
        [Category(Categories.UnitTests)]
        public void StrReg00001()
        {
            var program = RewriteX86_32Fragment(
                Fragments.Regressions.Reg00001.Text,
                Address.Ptr32(0x00100000));
            var sExp =
@"void fn00100000(word32 dwArg00, word32 dwArg04)
{
	Mem11[0x02000000:word32] = fn0010000C(dwArg00, dwArg04);
}
===
word32 fn0010000C(word32 dwArg04, word32 dwArg08)
{
	word32 ecx_11 = Mem7[dwArg04 + 0x0000003C:word32] + dwArg04;
	word32 esi_19 = (word32) Mem18[ecx_11 + 0x00000006:word16];
	word32 edx_20 = 0x00000000;
	word32 eax_23 = (word32) Mem7[ecx_11 + 0x00000014:word16] + 0x00000012 + ecx_11;
	if (esi_19 >u 0x00000000)
	{
		do
		{
			word32 ecx_31 = Mem22[eax_23 + 0x0000000C:word32];
			if (dwArg08 >=u ecx_31 && dwArg08 <u Mem22[eax_23 + 0x00000008:word32] + ecx_31)
				return eax_23;
			++edx_20;
			eax_23 += 0x00000028;
		} while (edx_20 <u esi_19);
	}
	eax_23 = 0x00000000;
	return eax_23;
}
===
";
            RunTest(sExp, program);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void StrInfiniteLoop()
        {
            var pm = new ProgramBuilder();
            pm.Add("haltForever", m =>
            {
                m.Label("lupe");
                m.Goto("lupe");
            });
            var sExp =
@"define haltForever
{
	while (true)
		;
}
===
";
            RunTest(sExp, pm.Program);
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void StrReg00568()
        {
            RunTest("Fragments/regressions/r00568.asm", "Structure/StrReg00568.txt");
        }
    }
}